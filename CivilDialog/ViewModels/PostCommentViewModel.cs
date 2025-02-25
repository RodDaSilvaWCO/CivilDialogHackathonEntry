
using CivilDialog.Services;
using CivilDialog.Models;

using CivilDialog.Core.Interfaces;

namespace CivilDialog.ViewModels
{
    public partial class PostCommentViewModel : MainViewModel, IDisposable
    {

        #region Field Members
        PostCommentService _service;
        ICivilDialogHubService _civilDialogHubService;
        IForumPostVerifyService _forumPostVerifyService;
        
        #endregion Field Members

        #region Constructors
        public PostCommentViewModel(
            PostCommentService service, 
            ICivilDialogHubService civilDialogHubService, 
            IForumPostVerifyService fourmPostVerifyService )
        {
            _service = service;
            _civilDialogHubService = civilDialogHubService;
            _forumPostVerifyService = fourmPostVerifyService;
            _civilDialogHubService.AddThreadToForum += OnAddThreadToForum;
            _civilDialogHubService.PostReplyStart += OnPostReplyStarted;
            _civilDialogHubService.PostNewMessageStart += OnPostNewMessageStarted;
            _civilDialogHubService.ForumSelect += OnForumSelect;
            _civilDialogHubService.ForumThreadSelect += OnForumThreadSelect;
            _civilDialogHubService.ForumBrowse += OnForumBrowse;
            _civilDialogHubService.ForumBrowseThreads += OnForumBrowseThreads;
            PostCommand = new Command(async () => await PostAsync());
            //_chatAppManager = chatAppManager;
            ResetUI();
        }
        #endregion Constructors

        #region IDisposable Implementation
        public void Dispose()
        {
            _civilDialogHubService.AddThreadToForum -= OnAddThreadToForum;
            _civilDialogHubService.PostReplyStart -= OnPostReplyStarted;
            _civilDialogHubService.PostNewMessageStart -= OnPostNewMessageStarted;
            _civilDialogHubService.ForumSelect -= OnForumSelect;
            _civilDialogHubService.ForumThreadSelect -= OnForumThreadSelect;
            _civilDialogHubService.ForumBrowse -= OnForumBrowse;
            _civilDialogHubService.ForumBrowseThreads -= OnForumBrowseThreads;
            

        }
        #endregion IDisposable

        #region Public Properties

        public int WebViewRow { get; set; } = 0;
        public string PostText { get; set; } = string.Empty;

        public string SendButtonColor
        {
            get
            {
                if (IsEnabled)
                    return "#D0922C";
                else
                    return "#D9D9D9";
            }
        }

        public Editor TheEditor { get; set; } = null!;

        public ForumState CurrentForumState { get; set; } = ForumState.BrowsingForums;

        public string PlaceHolderText { get; set; } = "";
        public bool IsEnabled { get; set; } = false;
        public bool IsVisible { get; set; } = true;

        public string WebViewSource { get; set; } = "https://CivilDialog.org/";

        public Command PostCommand { get; }

        #endregion  Public Properties

        #region Event Handlers
        void OnAddThreadToForum(object sender, HubEventArgs e)
        {
            CurrentForumState = ForumState.AddingThreadToForum;
            IsEnabled = true;
            PlaceHolderText = $"Enter a new Topic title for Forum '{e.CivilDialogHubService?.ForumTitle}'... ";
            //IsVisible = true;
            //OnPropertyChanged(nameof(IsVisible));
            OnPropertyChanged(nameof(PlaceHolderText));
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(SendButtonColor));
        }
        void OnPostReplyStarted(object sender, HubEventArgs e)
        {
            CurrentForumState = ForumState.PostingReply;
            IsEnabled = true;
            PlaceHolderText = "Reply to the Comment...";
            OnPropertyChanged(nameof(PlaceHolderText));
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(SendButtonColor));
        }

        void OnPostNewMessageStarted(object sender, HubEventArgs e)
        {
            CurrentForumState = ForumState.PostNewMessage;
            IsEnabled = true;
            PlaceHolderText = $"Enter a new Comment for Topic '{e.CivilDialogHubService!.ThreadTitle}'...";
            OnPropertyChanged(nameof(PlaceHolderText));
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(SendButtonColor));
        }
        void OnForumSelect(object sender, HubEventArgs e)
        {
            ResetUI();
            CurrentForumState = ForumState.BrowsingForumThreads;
        }
        void OnForumThreadSelect(object sender, HubEventArgs e)
        {
            ResetUI();
            CurrentForumState = ForumState.BrowsingForumMessages;
        }

        void OnForumBrowseThreads(object sender, HubEventArgs e)
        {
            ResetUI();
            CurrentForumState = ForumState.BrowsingForumThreads;
        }

        void OnForumBrowse(object sender, HubEventArgs e)
        {
            ResetUI();
            CurrentForumState = ForumState.BrowsingForums;
        }
        #endregion

        #region Helpers
        //private IChatAppManager CreateChatAppManagerInstance( string path, string exec, string cmdArgs )
        //{
        //    return new ChatAppManager("C:\\CivilDialog\\CivilDialogHackathonEntry\\ARM64\\Debug",
        //        "ChatApp.exe",
        //        "--genie-config C:\\CivilDialog\\CivilDialogHackathonEntry\\ChatApp\\genie_bundle\\genie_config.json --base-dir C:\\CivilDialog\\CivilDialogHackathonEntry\\ChatApp\\genie_bundle\\");
        //}


        private async Task PostAsync()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(PostText))
                return;

            try
            {
                IsBusy = true;
                //if( !_chatAppManager!.IsInitialized )
                //{
                //    await _chatAppManager!.CreateInstanceAsync();
                //}
                switch (CurrentForumState)
                {
                    case ForumState.AddingThreadToForum:
                        IsBusy = true;
                        string newThread = PostText;
                        PostText = "";
                        PlaceHolderText = "Verifying post - One moment please...";
                        OnPropertyChanged(nameof(IsBusy));
                        OnPropertyChanged(nameof(PostText));
                        OnPropertyChanged(nameof(PlaceHolderText));
                        var inferenceNewThreadResponse = await _forumPostVerifyService.VerifyPost(_civilDialogHubService.User, newThread);
                        //var inferenceNewMessageResponse = await _chatAppManager.RunInferenceAsync(newMessage);
                        if (inferenceNewThreadResponse.CanPost)
                        {
                            // %TODO - Reset _chatAppManager
                            await _civilDialogHubService.AddThreadToForumAsync(newThread);
                        }
                        else
                        {

                        }
                        ResetUI();
                        break;


                    case ForumState.PostNewMessage:
                        IsBusy = true;
                        string newMessage = PostText;
                        PostText = "";
                        PlaceHolderText = "Verifying new post - One moment please...";
                        OnPropertyChanged(nameof(IsBusy));
                        OnPropertyChanged(nameof(PostText));
                        OnPropertyChanged(nameof(PlaceHolderText));
                        var inferenceNewMessageResponse =  await _forumPostVerifyService.VerifyPost(_civilDialogHubService.User, newMessage);
                        ////var inferenceNewMessageResponse = await _chatAppManager.RunInferenceAsync(newMessage);
                        if (inferenceNewMessageResponse.CanPost)
                        {
                            await _civilDialogHubService.PostNewMessageAsync(newMessage);
                        }
                        else
                        {

                        }
                        //Debug.Print(inferenceReplyMessageResponse);
                        // %TODO% - Reset _chatAppManager
                        //await _civilDialogHubService.PostNewMessageAsync(newMessage);
                        ResetUI();
                        CurrentForumState = ForumState.BrowsingForumMessages;
                        break;


                    case ForumState.BrowsingForumThreads:
                        ResetUI();
                        CurrentForumState = ForumState.BrowsingForumThreads;
                        break; // Ignore


                    case ForumState.PostingReply:
                        IsBusy = true;
                        string replyMessage = PostText;
                        PostText = "";
                        PlaceHolderText = "Verifying reply post - One moment please...";
                        OnPropertyChanged(nameof(IsBusy));
                        OnPropertyChanged(nameof(PostText));
                        OnPropertyChanged(nameof(PlaceHolderText));
                        var inferenceReplyMessageResponse = await _forumPostVerifyService.VerifyPost(_civilDialogHubService.User, replyMessage);
                        if (inferenceReplyMessageResponse.CanPost)
                        {
                            await _civilDialogHubService.PostReplyAsync(PostText);
                        }
                        else
                        {

                        }
                        ResetUI();
                        CurrentForumState = ForumState.BrowsingForumMessages;
                        break;


                    default:  // BrowsingForms
                        ResetUI();
                        break;
                }
            }
            catch
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
            }
            await Task.CompletedTask;
        }


        private void ResetUI()
        {
            IsBusy = false;
            IsEnabled = false;
            PlaceHolderText =  string.Empty;
            CurrentForumState = ForumState.BrowsingForums;
            PostText = string.Empty;
            OnPropertyChanged(nameof(PlaceHolderText));
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(PostText));
            OnPropertyChanged(nameof(SendButtonColor));
            OnPropertyChanged(nameof(IsBusy));
        }

       
        #endregion
    }

    public enum ForumState
    {
        BrowsingForums,
        BrowsingForumThreads,
        AddingThreadToForum,
        PostNewMessage,
        PostingReply,
        BrowsingForumMessages
    }   
}
