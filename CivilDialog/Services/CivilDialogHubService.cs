using System.Diagnostics;
using CivilDialog.Models;
using Microsoft.AspNetCore.SignalR.Client;
namespace CivilDialog.Services
{
   public class CivilDialogHubService : ICivilDialogHubService
    {
        private HubConnection _connection;
        private string _connectionId = null!;
        private string _user = null!;
        private string _forumId = null!;
        private string _forumTitle = null!;
        private string _threadId = null!;   
        private string _threadTitle = null!;
        private string _messageId = null!;
        private string _postBeingRepliedTo = null!;

        public event EventHandler<HubEventArgs>? AddThreadToForum = null!;
        public event EventHandler<HubEventArgs>? ForumSelect = null!;
        public event EventHandler<HubEventArgs>? ForumThreadSelect = null!;
        public event EventHandler<HubEventArgs>? PostReplyStart = null!;
        public event EventHandler<HubEventArgs>? PostNewMessageStart = null!;
        public event EventHandler<HubEventArgs>? AddThreadPostStart = null!;
        public event EventHandler<HubEventArgs>? AddThreadStart = null!;
        public event EventHandler<HubEventArgs>? ForumBrowse = null!;
        public event EventHandler<HubEventArgs>? ForumBrowseThreads = null!;


        public CivilDialogHubService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://CivilDialog.org/nodeHub") // Replace with your SignalR server URL
                .Build();

         
            _connection.Closed += async (error) =>
            {
                Console.WriteLine("Connection closed. Reconnecting...");
                await Task.Delay(new Random().Next(0, 5) * 1000); // Random delay before reconnect
                await _connection.StartAsync(); // Reconnect
            };
        }

        public string ConnectionId => _connectionId;
        public string User => _user;
        public string ForumId => _forumId;

        public string ForumTitle => _forumTitle;   
        public string ThreadId => _threadId;
        public string ThreadTitle => _threadTitle;
        public string PostId => _messageId;
        public string PostBeingRepliedTo => _postBeingRepliedTo;



        public async Task StartConnectionAsync()
        {
            try
            {
                await _connection.StartAsync();
                //Console.WriteLine("SignalR connection started.");
            }
            catch (Exception ex)
            {
                Debug.Print($"**** CivilDialog.CivilDialogHubService.StartConnectionAsync() - unable to establish connection with CivilDialog website (Hub): {ex}");
                //Console.WriteLine($"Error starting connection: {ex.Message}");
            }
        }

        public async Task StopConnectionAsync()
        {
            try
            {
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {

                Debug.Print($"**** CivilDialog.CivilDialogHubService.StartConnectionAsync() - unable to establish connection with CivilDialog website (Hub): {ex}");
            }
        }


        public void RegisterMessageHandler()
        {
            _connection.On<string>("RegisterConnectionId", (connectionId) =>
            {
                // NOTE:  This is called back from the Hub as soon as this client connects to the Hub
                //        in order to allow this client to learn the connectionId the Hub is tracking it by.
                _connectionId = connectionId;
            });


            _connection.On<string>("UserLoggedIn", async (user) =>
            {
                // NOTE:  This is called back from the Hub when a user logs in so that the client can learn the identity of the user session.
                _user = user;
                // NOTE:  We immediately call back to the Hub passing the user and the connectionId learned from OnRegisterConnectionId() above
                //        so that the Hub can map the connectionId to the user.
                await _connection.SendAsync("OnMapConnectionUserToConnectionId", user, _connectionId);

            });


            _connection.On<string, string, string>("OnForumSelect",
                async (user, connectionId, forumId) =>
            {
                // NOTE:  This is called back from the Hub when a selects a forum.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _threadId = null!;
                _messageId = null!;
                _postBeingRepliedTo = null!;
                if (ForumSelect != null)
                {
                    ForumSelect(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;
            });


            _connection.On<string, string, string, string>("OnAddThreadToForum", async (user, connectionId, forumId, forumTitle) =>
            {
                // NOTE:  This is called back from the Hub when a selects a forum.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _forumTitle = forumTitle;
                _threadId = null!;
                _messageId = null!;
                _postBeingRepliedTo = null!;
                if (AddThreadToForum != null)
                {
                    AddThreadToForum(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;
            });


            _connection.On<string, string >("OnForumBrowse", async (user, connectionId) =>
            {
                // NOTE:  This is called back from the Hub when a user selects a forum.
                _connectionId = connectionId;
                _user = user;
                _forumId = null!;
                _threadId = null!;
                _messageId = null!;
                _postBeingRepliedTo = null!;
                if (ForumBrowse != null)
                {
                    ForumBrowse(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;
            });


            _connection.On<string, string, string>("OnForumBrowseThreads", async (user, connectionId, forumId) =>
            {
                // NOTE:  This is called back from the Hub when a user selects a forum.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _threadId = null!;
                _messageId = null!;
                _postBeingRepliedTo = null!;
                if (ForumBrowseThreads != null)
                {
                    ForumBrowseThreads(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;
            });


            _connection.On<string, string, string, string>("OnForumThreadSelect", async (user, connectionId, forumId, threadId ) =>
            {
                // NOTE:  This is called back from the Hub when a user selects a forum.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _threadId = threadId;
                _messageId = null!;
                _postBeingRepliedTo = null!;
                if (ForumThreadSelect != null)
                {
                    ForumThreadSelect(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;
            });


            _connection.On<string,string,string, string, string, string>("OnPostReplyStart", async(user, connectionId, forumId, threadId, postId, postBeingRepliedTo ) =>
            {
                // NOTE:  This is called back from the Hub when a attempts to reply to a post.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _threadId = threadId;
                _messageId = postId;
                _postBeingRepliedTo = postBeingRepliedTo;
                if (PostReplyStart != null)
                {
                    PostReplyStart(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;

            });


            _connection.On<string, string, string, string, string>("X", async (user, connectionId, forumId, threadId, threadTitle) =>
            {
                // NOTE:  This is called back from the Hub when a attempts to reply to a post.
                _connectionId = connectionId;
                _user = user;
                _forumId = forumId;
                _threadId = threadId;
                _messageId = null!;
                _threadTitle = threadTitle;
                _postBeingRepliedTo = null!;
                if (PostNewMessageStart != null)
                {
                    PostNewMessageStart(this, new HubEventArgs { CivilDialogHubService = this });
                }
                await Task.CompletedTask;

            });

        }




        #region Calls back to website (Hub)
        public async Task AddThreadToForumAsync(string threadTitle)
        {
            await _connection.SendAsync("OnAddThreadToForum", _user, _connectionId, _forumId, threadTitle);
        }

        public async Task PostNewMessageAsync(string message)
        {
            await _connection.SendAsync("OnPostNewMessage", _user, _connectionId, _forumId, _threadId, message);
        }


        public async Task PostReplyAsync(string message)
        {
            await _connection.SendAsync("OnPostReplyMessage", _user, _connectionId, _forumId, _threadId, _messageId, message);
        }
        #endregion Calls back to website (Hub)
    }
}