using System.Diagnostics;
// Add other necessary using statements:
using LingoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks; // Make sure this is included
using System; // Make sure this is included

namespace LingoSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // private readonly IConfiguration _configuration; // Still unused, can be removed unless needed elsewhere
        private readonly HttpClient _httpClient;
        private readonly List<string> mostUsedLanguages = new List<string>
        {
            "English", "Spanish", "French", "German", "Italian", "Chinese", "Japanese",
            "Russian", "Portuguese", "Arabic", "Hindi", "Bengali", "Korean", "Turkish",
            "Vietnamese", "Urdu", "Persian", "Swahili"
        };

        // Constructor (remove IConfiguration if not used)
        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            ViewBag.Languages = new SelectList(mostUsedLanguages);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenAIGPT(string query, string selectedLanguage)
        {
            var languageCodes = new Dictionary<string, string>
            {
                {"English", "en"}, {"Spanish", "es"}, {"French", "fr"}, {"German", "de"}, {"Italian", "it"},
                {"Chinese", "zh"}, {"Japanese", "ja"}, {"Russian", "ru"}, {"Portuguese", "pt"}, {"Arabic", "ar"},
                {"Hindi", "hi"}, {"Bengali", "bn"}, {"Korean", "ko"}, {"Turkish", "tr"}, {"Vietnamese", "vi"},
                {"Urdu", "ur"}, {"Persian", "fa"}, {"Swahili", "sw"}
            };

            if (!languageCodes.TryGetValue(selectedLanguage, out var langCode))
            {
                _logger.LogWarning("Invalid language selected: {SelectedLanguage}", selectedLanguage);
                // *** FIX 1: Pass the ErrorViewModel ***
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var parameters = new Dictionary<string, string>
            {
                { "q", query },
                { "source", "en" },
                { "target", langCode },
                { "format", "text" }
            };
            var content = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _httpClient.PostAsync("https://libretranslate.com/translate", content);
                response.EnsureSuccessStatusCode(); // This will throw an HttpRequestException on non-2xx status codes

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LibreTranslateResponse>(json);

                ViewBag.Translation = result?.TranslatedText ?? "No translation found.";
            }
            catch (Exception ex) // Catches EnsureSuccessStatusCode failures and other exceptions
            {
                _logger.LogError(ex, "Translation failed for query '{Query}' to language '{SelectedLanguage}'.", query, selectedLanguage);
                // *** FIX 2: Pass the ErrorViewModel ***
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            // On success, return to Index view
            ViewBag.Languages = new SelectList(mostUsedLanguages);
            return View("Index");
        }

        public class LibreTranslateResponse
        {
            [JsonProperty("translatedText")]
            public string TranslatedText { get; set; } = string.Empty; // Good practice to initialize
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // This action remains correct
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
