using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("{*url}")]
        public IActionResult Resolve(string url)
        {
            return Content($"Request: {url}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
