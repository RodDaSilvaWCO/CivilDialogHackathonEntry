using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using CivilDialog.Core.Interfaces;
using CivilDialog.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CivilDialog.Core.Services
{
    public class ForumPostVerifyService : IForumPostVerifyService
    {
        #region Member Fields
        private IChatCompletionService _chatService = null!;
        private Kernel _kernel = null!;
        private PromptExecutionSettings _settings = null!;
        private ChatHistory _chat = null!;
        private ISystemPrompt _prompt = null!;  
        #endregion

        #region Constructors
        public ForumPostVerifyService(IChatCompletionService chatService, Kernel kernel, PromptExecutionSettings settings, ChatHistory chat, ISystemPrompt prompt)
        {
            _chatService = chatService;
            _kernel = kernel;
            _settings = settings;
            _chat = chat;
            _prompt = prompt;
        }

        #endregion

        #region Public Interface
        public async Task<IForumPostVerifyResult> VerifyPost(string user, string post, bool clearChatHistory= true, bool retainChatHistory = false, CancellationToken cancelToken = default)
        {
            IForumPostVerifyResult result = null!;
            try
            {
                
                if(clearChatHistory)
                {
                    _chat.Clear();
                    _chat.AddSystemMessage(_prompt.Prompt);
                }
                _chat.AddUserMessage($"{user} says: {post}");
                var response = "";
                var firstTokenStopWatch = Stopwatch.StartNew();
                var fullReportStopWatch = Stopwatch.StartNew();
                var chunks = _chatService.GetStreamingChatMessageContentsAsync(_chat, _settings, _kernel, cancelToken);
                var tokenCount = 0;
                await foreach (var chunk in chunks)
                {
                    try
                    {
                        if( tokenCount++ == 0)
                        {
                            firstTokenStopWatch.Stop();
                        }
                        System.Diagnostics.Debug.Write(chunk);
                        response += chunk;
                        if(CheckIfReportComplete(response))
                        {
                            break;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        continue;  // As we have started to output tokens then ignore the cancellation.
                    }
                    catch (Exception)
                    {
                        throw;  // Unexpected exception
                    }
                }
                fullReportStopWatch.Stop();
                // Process the response
                result = ProcessAIResponse(response);
                if ( retainChatHistory)
                {
                    _chat.AddAssistantMessage(response);
                }
                result.TimeToFullReportInMilliSeconds = (int)fullReportStopWatch.ElapsedMilliseconds;
                result.TimeToFirstTokenInMilliSeconds = (int)firstTokenStopWatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        #endregion

        #region Helpers
        private IForumPostVerifyResult ProcessAIResponse(string response)
        {
            IForumPostVerifyResult result = new ForumPostVerifyResult(response);
            try
            {
                try
                {
                    // Attempt to match a <report> ... </report> sequence  
                    // EX:  <report> <d>true</d> <a>Trump is an idiot.</a> <t>Abusive ad hominem</t> <e>The statement attacks Trump's character instead of addressing an argument or claim.</e> <rw>The statement is not suitable for a logical argument.</rw> </report>�
                    //Regex regex = new Regex(@"<report>(.*?)</report>");
                    //Match match = regex.Match(response);
                    int startPos = response.IndexOf("<report>");
                    int endPos = response.IndexOf("</report>");
                    if (startPos >= 0 && endPos > startPos)
                    {
                        // We have a report
                        string report = response.Substring(startPos, endPos - startPos + 9);
                        // Process the report by attempting to parse it as xml
                        try
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(report);
                            // If we make it here we have a valid xml response
                            XmlNode? dNode = xmlDocument.SelectSingleNode($"//d");
                            bool cannotPost = false;
                            if(dNode != null && bool.TryParse(dNode.InnerText, out cannotPost))
                            {
                                result.CanPost = !cannotPost;
                                XmlNode? aNode = xmlDocument.SelectSingleNode($"//a");
                                if (aNode != null)
                                {
                                    if (string.IsNullOrEmpty(aNode.InnerText) || aNode.InnerText.ToLower() == "false")
                                    {
                                        result.Justification = "";
                                    }
                                    else
                                    {
                                        result.Justification = aNode.InnerText;
                                    }
                                }
                                XmlNode? tNode = xmlDocument.SelectSingleNode($"//t");
                                if (tNode != null)
                                {
                                    if (string.IsNullOrEmpty(tNode.InnerText) || tNode.InnerText.ToLower() == "false")
                                    { 
                                        result.FallacyType = "";
                                    }
                                    else
                                    {
                                        result.FallacyType = tNode.InnerText;
                                    }
                                }
                                XmlNode? eNode = xmlDocument.SelectSingleNode($"//e");
                                if (eNode != null)
                                {
                                    if (string.IsNullOrEmpty(eNode.InnerText) || eNode.InnerText.ToLower() == "false")
                                    { 
                                        result.FallacyExplaination = "";
                                    }
                                    else
                                    {
                                        result.FallacyExplaination = eNode.InnerText;
                                    }
                                }
                                XmlNode? rwNode = xmlDocument.SelectSingleNode($"//rw");
                                if (rwNode != null)
                                {
                                    if(string.IsNullOrEmpty(rwNode.InnerText) ||rwNode.InnerText.ToLower() == "false")
                                    {
                                        result.AlternateSuggestion = "";
                                    }
                                    else
                                    {
                                        result.AlternateSuggestion = rwNode.InnerText;
                                    }
                                }
                                #region Override possible false result.CanPost value if Justificatino, FallacyType, and FallacyExplaination are all present
                                if (!string.IsNullOrEmpty(result.Justification) && !string.IsNullOrEmpty(result.FallacyType) && !string.IsNullOrEmpty(result.FallacyExplaination))
                                {
                                    result.CanPost = false;
                                }
                                #endregion 
                            }
                        }
                        catch (Exception xmlEx)
                        {
                            // If we make it here we don't have a result that is understanable so all we can do is accept the post
                            // %TODO% : Check for profanity here
                            result.CanPost = true;
                            result.WasError = true;
                            result.ErrorMessage = xmlEx.Message;
                            result.Justification = "Did not get valid <report/> xml response";
                        }
                    }
                    else
                    {
                        // If we make it here we don't have a result that is understanable so all we can do is accept the post
                        // %TODO% : Check for profanity here
                        result.CanPost = true;
                        result.WasError = true;
                        result.ErrorMessage = "Did not get valid <report> response";
                        result.Justification = "Did not get valid <report> response";
                    }
                }
                catch (Exception regEx)
                {
                    // If we make it here we don't have a result that is understanable so all we can do is accept the post
                    // %TODO% : Check for profanity here
                    result.CanPost = true;
                    result.WasError = true;
                    result.ErrorMessage = regEx.Message;
                    result.Justification = "Did not get valid <report> response";
                }
            }
            catch (Exception generalEx)
            {
                // If we make it here we don't have a result that is understanable so all we can do is accept the post
                // %TODO% : Check for profanity here
                result.CanPost = true;
                result.WasError = true;
                result.ErrorMessage = generalEx.Message;
                result.Justification = "Unexpected error";
            }
            return result;
        }

        private bool CheckIfReportComplete(string response)
        {
            return (response.ToLower().IndexOf("<report>") >= 0 && response.ToLower().IndexOf("</report>") > 0 ) || response.ToLower().IndexOf("<report/>") >= 0;
        }

        private bool CheckForProfanity(string post)
        {
            // %TODO% : Implement a profanity check
            return false;
        }
        #endregion

    }
}
