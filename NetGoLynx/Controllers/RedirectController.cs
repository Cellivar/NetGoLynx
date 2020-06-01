using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models.RedirectModels;
using NetGoLynx.Services;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Controller for interacting with redirects
    /// </summary>
    [Authorize]
    [Route("_/[controller]")]
    public class RedirectController : Controller
    {
        private readonly IRedirectService _redirectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectController"/> class.
        /// </summary>
        /// <param name="redirectService">The redirect API for performing operations.</param>
        public RedirectController(IRedirectService redirectService)
        {
            _redirectService = redirectService;
        }

        [HttpGet("notfound")]
        [AllowAnonymous]
        public new IActionResult NotFound()
        {
            return View();
        }

        /// <summary>
        /// Get a list of all owned redirects.
        /// </summary>
        /// <param name="highlightId">A specific redirect to highlight</param>
        /// <returns>The list view populated with redirects.</returns>
        [HttpGet("list")]
        public async Task<IActionResult> ListAsync(int highlightId)
        {
            var redirects = await _redirectService.GetAsync();
            return View("List", new ListModel(redirects: redirects, id: highlightId));
        }

        [HttpGet("listall")]
        public async Task<IActionResult> ListAllAsync()
        {
            var allRedirects = await _redirectService.GetAllWithUsernameAsync();
            return View("ListAll", new ListAllModel(redirects: allRedirects));
        }

        /// <summary>
        /// Get the add redirect page.
        /// </summary>
        /// <param name="suggestedLinkName">An optional suggested link name to add.</param>
        /// <returns>The add view</returns>
        [HttpGet("add")]
        public IActionResult Add(string suggestedLinkName = "")
        {
            return View("Add", new RedirectMetadata(suggestedLinkName));
        }

        /// <summary>
        /// Add a redirect.
        /// </summary>
        /// <param name="model">The redirect structure to add.</param>
        /// <returns>The list view if successful, otherwise an error message.</returns>
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAsync(RedirectMetadata model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Invalid options, confirm form validation";
                return View("Add", model);
            }

            var redirect = model.ToRedirect();

            var success = await _redirectService.TryCreateAsync(redirect);
            if (success)
            {
                return RedirectToAction("List", new { highlightId = redirect.RedirectId });
            }

            if (redirect.RedirectId == -1)
            {
                model.ErrorMessage = "A link with that name already exists.";
                return View("Add", model);
            }

            model.ErrorMessage = "Unknown problem happened. Try again?";
            return View("Add", model);
        }

        /// <summary>
        /// Get the delete redirect page.
        /// </summary>
        /// <param name="id">The redirect to delete</param>
        /// <returns>The delete view with information about the selected redirect.</returns>
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            // Confirm the referenced ID is real
            var redirect = await _redirectService.GetAsync(id);
            if (redirect == null)
            {
                return BadRequest();
            }

            return View("Delete", new RedirectMetadata(redirect.Name, redirect.RedirectId));
        }

        /// <summary>
        /// Delete a redirect.
        /// </summary>
        /// <param name="model">The redirect to delete</param>
        /// <returns>The list view</returns>
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(RedirectMetadata model)
        {
            await _redirectService.DeleteAsync(model.RedirectId);

            return RedirectToAction("List");
        }
    }
}
