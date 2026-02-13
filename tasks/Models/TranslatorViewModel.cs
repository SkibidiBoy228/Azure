using System.ComponentModel.DataAnnotations;

namespace tasks.Models
{
    public class TranslatorViewModel
    {
        [Required(ErrorMessage = "Введіть текст для перекладу")]
        [MinLength(2, ErrorMessage = "Текст має містити щонайменше 2 символи")]
        public string? Text { get; set; }

        public string SourceLanguage { get; set; } = "uk";
        public string TargetLanguage { get; set; } = "en";

        public string? SourceTransliterationText { get; set; }
        public string? TargetTransliterationText { get; set; }

        public string? TranslatedText { get; set; }
        public string? TransliterationText { get; set; }
    }
}
