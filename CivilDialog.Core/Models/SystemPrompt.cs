using CivilDialog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilDialog.Core.Models
{
    public class SystemPrompt : ISystemPrompt
    {
        public SystemPrompt(string systemPrompt)
        {
            Prompt = systemPrompt;
        }
        public string Prompt { get; set; }
    }
}
