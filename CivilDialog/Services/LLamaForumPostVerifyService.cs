using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivilDialog.Core.Interfaces;
using CivilDialog.Core.Models;
using CivilDialog.Interfaces;

namespace CivilDialog.Services
{
    public class LLamaForumPostVerifyService : IForumPostVerifyService, IDisposable
    {
        #region Static Members
        #endregion Static Memebers

        #region Field Memebers
        IChatAppManager _chatAppManager = null!;
        #endregion Field Members


        #region Constructors
        public LLamaForumPostVerifyService(IChatAppManager chatAppManager)
        {
            Debug.WriteLine("********* ForumPostVerifyService.ctor() called.");
            _chatAppManager = chatAppManager;
        }
        #endregion Constructors


        #region IDisposable Implementation
        public void Dispose()
        {
            Debug.WriteLine("********* ForumPostVerifyService.Dispose() called.");
        }
        #endregion IDisposable


        

        #region IForumPostVerifyService Implementation
        public async Task<IForumPostVerifyResult> VerifyPost(string user, string post, bool clearChatHistory = true, bool retainChatHistory = false, CancellationToken cancelToken = default)
        {
            Debug.WriteLine($"********* ForumPostVerifyService.VerifyPost(user={user}, post={post}) called.");
            IForumPostVerifyResult result = null!;
            try
            {
                if (!_chatAppManager!.IsInitialized)
                {
                    await _chatAppManager!.CreateInstanceAsync();
                }

                var inferenceNewMessageResponse = await _chatAppManager.RunInferenceAsync($"{user} says:{post}");
                // NOTE:  We ignore the response from Llama becaues it doesn't make sense.
                result = new ForumPostVerifyResult(post);
                result.CanPost = true;
                await Task.CompletedTask;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        #endregion IForumPostVerifyService Implementation

        #region Helpers
        #endregion Helpers
    }
}
