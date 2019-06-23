using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;
using NetGoLynx.Models.Home;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Primary controller for handling web-based application interation.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly RedirectController _redirectController;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="redirectController"></param>
        public HomeController(RedirectController redirectController)
        {
            _redirectController = redirectController;
        }

        /// <summary>
        /// Handler for web view interactions.
        /// </summary>
        /// <param name="op">The requested operation.</param>
        /// <param name="suggestedLinkName">A redirect name to suggest.</param>
        /// <param name="id">A redirect ID to highlight.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(
            Operation op = Operation.List,
            string suggestedLinkName = "",
            int id = -1)
        {
            switch (op)
            {
                case Operation.Add:
                    return View("Add", new AddModel(suggestedLinkName));
                case Operation.Delete:
                    return View("Delete", new ListModel(id: id, name: suggestedLinkName));
                default:
                case Operation.List:
                    var redirects = await _redirectController.GetRedirectEntriesAsync();
                    return View("List", new ListModel(redirects: redirects, id: id));
            }
        }

        /// <summary>
        /// Handler for requesting a new link.
        /// </summary>
        /// <param name="model">The <see cref="AddModel"/> object to use. Must be valid.</param>
        /// <returns>A view with the result of the add operation.</returns>
        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLink(AddModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Invalid options, confirm form validation";
                return View("Add", model);
            }

            var redirect = model.ToRedirect();

            var result = await _redirectController.TryCreateRedirectAsync(redirect);
            switch (result.Result)
            {
                case RedirectController.OperationResult.Success:
                    return await Index(Operation.List, id: result.Redirect.RedirectId);
                case RedirectController.OperationResult.Conflict:
                    model.ErrorMessage = "A link with that name already exists.";
                    return View("Add", model);
                default:
                    model.ErrorMessage = "Unknown problem happened. Try again?";
                    return View("Add", model);
            }
        }

        /// <summary>
        /// Resolve a redirect request.
        /// </summary>
        /// <param name="name">The name of the redirect to resolve.</param>
        /// <returns>A 302 redirect to the target URL for that redirect name, or an error.</returns>
        [HttpGet]
        [Route("{*name}")]
        public async Task<IActionResult> ResolveAsync(string name)
        {
            var redirect = await _redirectController.GetRedirectEntry(name);

            if (redirect == null)
            {
                return RedirectToAction("Index", new { op = Operation.Add, suggestedLinkName = name });
            }

            return Redirect(redirect.Target);
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

        /// <summary>
        /// Types of request operations.
        /// </summary>
        public enum Operation
        {
            /// <summary>
            /// List redirects
            /// </summary>
            List,

            /// <summary>
            /// Add a redirect
            /// </summary>
            Add,

            /// <summary>
            /// Delete a redirect
            /// </summary>
            Delete,
        }
    }
}
