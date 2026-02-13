using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace tasks.Models
{
    public class TranslatorViewModel
    {
        public string SourceLanguage { get; set; } = "uk";
        public string TargetLanguage { get; set; } = "en";
        public string Text { get; set; }
        public string TranslatedText { get; set; }
    }
}
