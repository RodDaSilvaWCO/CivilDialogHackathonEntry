using CivilDialog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog
{
    public interface ICivilDialogHubService
    {
        Task StartConnectionAsync();
        Task StopConnectionAsync();

        void RegisterMessageHandler();

        event EventHandler<HubEventArgs>? ForumBrowse;
        event EventHandler<HubEventArgs>? ForumBrowseThreads;
        event EventHandler<HubEventArgs>? AddThreadToForum;
        event EventHandler<HubEventArgs>? PostReplyStart;
        event EventHandler<HubEventArgs>? PostNewMessageStart;
        event EventHandler<HubEventArgs>? AddThreadPostStart;
        event EventHandler<HubEventArgs>? AddThreadStart;
        event EventHandler<HubEventArgs>? ForumSelect;
        event EventHandler<HubEventArgs>? ForumThreadSelect;

        Task AddThreadToForumAsync( string threadTitle );

        Task PostNewMessageAsync(string message);

        Task PostReplyAsync(string message);

        string User { get; }
    }
}
