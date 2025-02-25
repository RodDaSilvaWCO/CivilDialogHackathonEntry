using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog.Core.Interfaces
{
    public interface IForumPostVerifyService
    {
        Task<IForumPostVerifyResult> VerifyPost(string user, string post, bool clearChatHistory = true, bool retainChatHistory = false, CancellationToken cancelToken = default);
    }
}
