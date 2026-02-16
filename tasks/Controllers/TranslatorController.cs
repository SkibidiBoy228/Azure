using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using tasks.Models;

namespace tasks.Controllers
{
    public class TranslatorController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public TranslatorController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var model = new TranslatorViewModel
            {
                SourceLanguage = HttpContext.Session.GetString("SourceLanguage") ?? "uk",
                TargetLanguage = HttpContext.Session.GetString("TargetLanguage") ?? "en",
                Text = HttpContext.Session.GetString("Text")
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Translate(TranslatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            HttpContext.Session.SetString("SourceLanguage", model.SourceLanguage);
            HttpContext.Session.SetString("TargetLanguage", model.TargetLanguage);
            HttpContext.Session.SetString("Text", model.Text ?? "");

            var key = _configuration["AzureTranslator:Key"];
            var endpoint = _configuration["AzureTranslator:Endpoint"];
            var region = _configuration["AzureTranslator:Region"];

            if (string.IsNullOrEmpty(key))
            {
                model.TranslatedText = "Сервіс перекладу тимчасово недоступний";
                return View("Index", model);
            }

            var route = $"/translate?api-version=3.0&from={model.SourceLanguage}&to={model.TargetLanguage}&toScript=Latn";
            var url = endpoint + route;

            var body = new object[] { new { Text = model.Text } };
            var requestBody = JsonSerializer.Serialize(body);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", region);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 600)
                {
                    model.TranslatedText = "Сервіс перекладу тимчасово недоступний";
                }
                else
                {
                    model.TranslatedText = "Помилка перекладу";
                }

                return View("Index", model);
            }

            var result = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(result);
            var root = doc.RootElement[0];

            var translation = root.GetProperty("translations")[0];

            var translatedText = translation
                .GetProperty("text")
                .GetString();

            if (translation.TryGetProperty("transliteration", out var transliteration))
            {
                model.TargetTransliterationText = transliteration
                    .GetProperty("text")
                    .GetString();
            }

            if (root.TryGetProperty("sourceText", out var sourceText) &&
                sourceText.TryGetProperty("transliteration", out var sourceTranslit))
            {
                model.SourceTransliterationText = sourceTranslit
                    .GetProperty("text")
                    .GetString();
            }
            var source = model.Text ?? "";

            if (source.Length > 40)
            {
                model.TranslatedText = $"[{source}\n{translatedText}]";
            }
            else
            {
                model.TranslatedText = $"[{source} - {translatedText}]";
            }

            return View("Index", model);
        }



        [HttpPost]
        public async Task<IActionResult> Swap(TranslatorViewModel model)
        {

            var temp = model.SourceLanguage;
            model.SourceLanguage = model.TargetLanguage;
            model.TargetLanguage = temp;


            if (string.IsNullOrWhiteSpace(model.TranslatedText))
            {
                ModelState.Clear();
                return View("Index", model);
            }

            model.Text = model.TranslatedText;
            model.TranslatedText = null;

            ModelState.Clear();

            return await Translate(model);
        }

    }
}
