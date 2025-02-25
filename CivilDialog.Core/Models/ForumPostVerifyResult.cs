using CivilDialog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog.Core.Models
{
    public class ForumPostVerifyResult : IForumPostVerifyResult
    {
        #region Member Fields
        #endregion

        #region Constructors
        public ForumPostVerifyResult(string response) 
        {
            CanPost = false;
            Justification = "";
            FallacyExplaination = "";
            FallacyType = "";
            AlternateSuggestion = "No rewrite suggstion could be provide.";
            ErrorMessage = "";
            CompleteResponse = response;
        }
        #endregion

        #region Public Interface
        public string CompleteResponse { get; set; }
        public bool CanPost { get; set; }
        public string Justification { get; set; }
        public string FallacyExplaination { get; set; }
        public string FallacyType { get; set; }
        public string AlternateSuggestion { get; set; }
        public string ErrorMessage { get; set; }
        public bool WasError { get; set; }
        public int TimeToFirstTokenInMilliSeconds { get; set; }
        public int TimeToFullReportInMilliSeconds { get; set; }
        #endregion

        #region Helpers
        #endregion

    }
}
