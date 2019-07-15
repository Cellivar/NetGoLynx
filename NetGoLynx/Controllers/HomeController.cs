using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Primary controller for handling web-based application interation.
    /// </summary>
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly RedirectApiController _redirectApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="redirectApi"></param>
        public HomeController(RedirectApiController redirectApi)
        {
            _redirectApi = redirectApi;
        }

        /// <summary>
        /// Handler for web view interactions.
        /// </summary>
        /// <returns>Redirect to list view if logged in, login page otherwise.</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("ListAsync", "Redirect");
        }

        /// <summary>
        /// Resolve a redirect request.
        /// </summary>
        /// <param name="name">The name of the redirect to resolve.</param>
        /// <returns>A 302 redirect to the target URL for that redirect name, or an error.</returns>
        [HttpGet("{*name}", Order = int.MaxValue)]
        public async Task<IActionResult> ResolveAsync(string name)
        {
            var redirect = await _redirectApi.GetRedirectEntry(name);

            var target = redirect?.Target ??
                Url.Action("AddAsync", "Redirect", new { suggestedLinkName = name });

            return Redirect(target);
        }

        /// <summary>
        /// Handle error pages.
        /// </summary>
        /// <returns>An error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
