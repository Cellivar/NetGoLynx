using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;
using NetGoLynx.Services;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Primary controller for handling web-based application interation.
    /// </summary>
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IRedirectService _redirectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="redirectService"></param>
        public HomeController(IRedirectService redirectService)
        {
            _redirectService = redirectService;
        }

        /// <summary>
        /// Handler for web view interactions.
        /// </summary>
        /// <returns>Redirect to list view if logged in, login page otherwise.</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("List", "Redirect");
        }

        /// <summary>
        /// Resolve a redirect request.
        /// </summary>
        /// <param name="name">The name of the redirect to resolve.</param>
        /// <returns>A 302 redirect to the target URL for that redirect name, or an error.</returns>
        [HttpGet("{*name}", Order = int.MaxValue)]
        public async Task<IActionResult> ResolveAsync(string name)
        {
            // Trim extraneous forward slashes only off the end
            name = name.TrimEnd('/');

            var target = await _redirectService.GetRedirectTargetAsync(name);

            if (!string.IsNullOrEmpty(target))
            {
                return Redirect(target);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Logged in users get a pretty add page
                return RedirectToAction("Add", "Redirect", new { suggestedLinkName = name });
            }

            return RedirectToAction("NotFound", "Redirect");
        }

        /// <summary>
        /// Handle error pages.
        /// </summary>
        /// <returns>An error view.</returns>
        [HttpGet("/_/Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Get server information
        /// </summary>
        /// <returns>Information about the server</returns>
        [EnableCors("AllowAll")]
        [HttpGet("/_/Home/Info")]
        public IActionResult Info()
        {
            return Ok(new
            {
                service = "netgolynx",
                version = "1.5.0",
            });
        }
    }
}
