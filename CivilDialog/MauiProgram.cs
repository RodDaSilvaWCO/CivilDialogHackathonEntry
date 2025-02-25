using CivilDialog;
using CivilDialog.Core.Interfaces;
using CivilDialog.Core.Models;
using CivilDialog.Core.Services;
using CivilDialog.Interfaces;
using CivilDialog.Models;
using CivilDialog.Services;
using CivilDialog.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace CivilDialog
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var chatAppManager = new ChatAppManager("C:\\CivilDialog\\CivilDialogHackathonEntry\\ARM64\\Debug",
                                    "ChatApp",
                                    "--genie-config C:\\CivilDialog\\CivilDialogHackathonEntry\\ChatApp\\genie_bundle\\genie_config.json --base-dir C:\\CivilDialog\\CivilDialogHackathonEntry\\ChatApp\\genie_bundle\\");
            var runInBackGround = chatAppManager.CreateInstanceAsync();
#if WINDOWS

            builder.ConfigureLifecycleEvents(events =>
            {
                // Make sure to add "using Microsoft.Maui.LifecycleEvents;" in the top of the file
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                        switch (appWindow.Presenter)
                        {
                            case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                                //disable the max button
                                overlappedPresenter.IsMaximizable = false;
                                break;
                        }

                        //When user execute the closing method, we can make the window do not close by   e.Cancel = true;.
                        appWindow.Closing += async (s, e) =>
                        {
                            if (chatAppManager != null)
                            {
                                chatAppManager.ShutDownInstance();
                                chatAppManager.Dispose();
                                chatAppManager = null!;
                            }
                            //e.Cancel = true;
                        };
                    });
                });
            });
#endif

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });


            builder.Services.AddSingleton<NavigationService>();
            builder.Services.AddSingleton<PostCommentService>();
            builder.Services.AddSingleton<ICivilDialogHubService, CivilDialogHubService>();
            builder.Services.AddSingleton<PostCommentViewModel>();
            builder.Services.AddSingleton<IForumPostVerifyService, LLamaForumPostVerifyService>();
            //builder.Services.AddSingleton<IForumPostVerifyService, ForumPostVerifyService>();
            builder.Services.AddSingleton<IChatAppManager>(chatAppManager);


            //#region Set up Semantic Kernel for Phi3.5 
            //builder.Services.AddKernel();

            //#region Onnx Phi3.5 Local File
            //var modelPath = @"C:\Dev\AI_Models\phi3.5\Phi-3.5-mini-instruct-onnx\cpu_and_mobile\cpu-int4-awq-block-128-acc-level-4";
            //builder.Services.AddOnnxRuntimeGenAIChatCompletion(modelPath);


            
            //#endregion

            //PromptExecutionSettings settings = new feiyun0112.SemanticKernel.Connectors.OnnxRuntimeGenAI.OnnxRuntimeGenAIPromptExecutionSettings
            //{
            //    TopK = 50,
            //    Temperature = 0.9f,
            //    RepetitionPenalty = 1f,
            //    PastPresentShareBuffer = true,
            //    NumReturnSequences = 1,
            //    NumBeams = 1,
            //    NoRepeatNgramSize = 0,
            //    MinLength = 0,
            //    MaxLength = 128 * 1024,  // default = 200
            //    LengthPenalty = 1f,
            //    EarlyStopping = true,
            //    DoSample = false,
            //    DiversityPenalty = 0f //,
            //                          //FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(new List<KernelFunction>()
            //                          //            {
            //                          //                setThemeColors
            //                          //            })


            //    //,
            //    //FunctionChoiceBehavior = FunctionChoiceBehavior.None()
            //};

            //builder.Services.AddSingleton<PromptExecutionSettings>(settings);
            //ISystemPrompt systemPrompt = new SystemPrompt("You are a sophisticated text analyzer with the capability to discern nuanced forms of argumentative fallacies, specifically focusing on \"ad hominem\" attacks. Your analysis must consider the subtleties of language and context to accurately identify instances where the subject is attacked indirectly through character judgments, stereotypes, or irrelevant personal details. The input may contain multiple sentences as well as historical context, and your single analysis result should take into account all sentences written by the user across all historical context. You MUST ONLY identify ad hominem fallacies that are abusive in nature.  Also, you MUST associate an identified ad hominem with the user who said it.  You MUST ignore any ad hominem that is made in the context of a user seeking clarification of another user's input in the form of a question.  Provide a detailed report in XML format, including the following elements: <report> <d>true ONLY if an abusive ad hominem fallacy is attributed to the user who provided the most recent input, OTHERWISE false<d> <a>ONLY if an ad hominem is detected, quote verbatim the part of the sentence containing the ad hominem fallacy, OTHERWISE leave empty.</a> <t>ONLY if an ad hominem is detected, provide the type of ad hominem detected, OTHERWISE leave empty.</t> <e>ONLY if an ad hominem is detected, explain in 1 sentence at most why this is an ad hominem, OTHERWISE leave empty.</e> <rw>ONLY if an ad hominem is detected, provide a rewrite of the sentence to remove the ad hominem, maintaining the original intent but without the fallacy, OTHERWISE leave empty.</rw> </report>. You MUST STOP your answer after the report which is denoted by the first occurrence of \"</report>\" in the output.  DO NOT explain your answer or output anything other than the requested report. ONLY output the first report. Provide a detailed analysis for the following input:");
            //builder.Services.AddSingleton<ISystemPrompt>(systemPrompt);
            //ChatHistory chat = new(systemPrompt.Prompt);
            //builder.Services.AddSingleton<ChatHistory>(chat);
            //builder.Services.AddSingleton<IForumPostVerifyService, ForumPostVerifyService>();
            //#endregion



            builder.Services.AddMauiBlazorWebView();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            //builder.UseStartup<HubStartup>((e) => new HubStartup(_localNodeContext!, _globalPropertiesContext!, this, _peerIdentityManager!))
            return builder.Build();
        }
    }
}
