using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    public class HomeController : Controller
    {
        private readonly RedirectController _redirectController;

        public HomeController(RedirectController redirectController)
        {
            _redirectController = redirectController;
        }

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
                    return View("Add", new NewRedirectModel(suggestedLinkName));
                default:
                case Operation.List:
                    var redirects = await _redirectController.GetRedirectEntriesAsync();
                    return View("List", new IndexModel(redirects: redirects, id: id));
            }
        }

        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLink(NewRedirectModel model)
        {
            var redirect = new Redirect()
            {
                Name = model.LinkName,
                Target = model.Target.ToString()
            };

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public enum Operation
        {
            List,
            Add,
            Delete,
        }
    }
}
