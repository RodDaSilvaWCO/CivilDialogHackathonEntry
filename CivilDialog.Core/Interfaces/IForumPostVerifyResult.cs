using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog.Core.Interfaces
{
    public interface IForumPostVerifyResult
    {
        bool CanPost { get; set; }
        string Justification { get; set; }
        string FallacyExplaination { get; set; }
        string FallacyType { get; set; } 
        string AlternateSuggestion { get; set; }
        string ErrorMessage { get; set; }
        bool WasError { get; set; } 
        string CompleteResponse { get; set; }

        int TimeToFirstTokenInMilliSeconds { get; set; }
        int TimeToFullReportInMilliSeconds { get; set; }
    }
}
