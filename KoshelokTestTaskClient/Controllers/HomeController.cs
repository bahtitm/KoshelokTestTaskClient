using KoshelokTestTaskClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KoshelokTestTaskClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static HttpClient httpClient = new HttpClient();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string message,int number)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            Stream stream = new MemoryStream(data);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using var response = await httpClient.PostAsync($"https://localhost:7268/Messages?number={number}", content);
            string responseText = await response.Content.ReadAsStringAsync();
            return NoContent();
        }

        public async Task<IActionResult> Privacy()
        {
            try
            {
                var uri = new Uri($"https://localhost:7268/Messages");
                List<GetedMessage> res = await httpClient.GetFromJsonAsync<List<GetedMessage>>(uri);

                return View(res);
            }
            catch (Exception)
            {

                throw;
            }
           
          
        }
        public IActionResult OnlineMessage(string st)
        {
            var people = new List<string> { { st }, "Sam", "Bob" };
            return View(people);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
