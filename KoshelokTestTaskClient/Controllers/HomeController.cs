using KoshelokTestTaskClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KoshelokTestTaskClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;
        static HttpClient httpClient = new HttpClient();
        private readonly string HOST;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
            HOST = configuration.GetSection("HOST").Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string message, int number)
        {
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                Stream stream = new MemoryStream(data);
                var content = new StreamContent(stream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                using var response = await httpClient.PostAsync($"{HOST}/Messages?number={number}", content);
                string responseText = await response.Content.ReadAsStringAsync();
                return NoContent();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }

        }

        public async Task<IActionResult> AllMessage()
        {
            try
            {
                var uri = new Uri($"{HOST}/Messages");
                List<GetedMessage> res = await httpClient.GetFromJsonAsync<List<GetedMessage>>(uri);

                return View(res);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }


        }
        public IActionResult OnlineMessage()
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
