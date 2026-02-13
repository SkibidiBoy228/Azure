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
            HttpContext.Session.SetString("SourceLanguage", model.SourceLanguage);
            HttpContext.Session.SetString("TargetLanguage", model.TargetLanguage);
            HttpContext.Session.SetString("Text", model.Text ?? "");

            if (string.IsNullOrWhiteSpace(model.Text))
            {
                return View("Index", model);
            }

            var key = _configuration["AzureTranslator:Key"];
            var endpoint = _configuration["AzureTranslator:Endpoint"];
            var region = _configuration["AzureTranslator:Region"];

            var route = $"/translate?api-version=3.0&from={model.SourceLanguage}&to={model.TargetLanguage}";
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
                model.TranslatedText = "Ошибка перевода: " + response.StatusCode;
                return View("Index", model);
            }

            var result = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(result);
            model.TranslatedText = doc.RootElement[0]
                                      .GetProperty("translations")[0]
                                      .GetProperty("text")
                                      .GetString();

            return View("Index", model);
        }
    }
}
