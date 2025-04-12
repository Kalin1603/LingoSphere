using System.Diagnostics;
using LingoSphere.Models;
using Microsoft.AspNetCore.Mvc;

namespace LingoSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly List<string> mostUsedLanguages = new List<string>
        {
            "English",
            "Spanish",
            "French",
            "German",
            "Italian",
            "Chinese",
            "Japanese",
            "Russian",
            "Portuguese",
            "Arabic",
            "Hindi",
            "Bengali",
            "Korean",
            "Turkish",
            "Vietnamese",
            "Urdu",
            "Persian",
            "Swahili"
        };


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration = null, HttpClient httpClient = null)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetGPTResponses (string query, string selectedLanguage)
        {
            var apiKey = _configuration["GeminiAI:ApiKey"];

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
