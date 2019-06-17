using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    public class HomeController : Controller
    {
        private readonly RedirectContext _context;
        private readonly RedirectController _redirectController;

        public HomeController(RedirectContext context, RedirectController redirectController)
        {
            _context = context;
            _redirectController = redirectController;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(
            IndexModel.Operation operation = IndexModel.Operation.List,
            string suggestedLinkName = "")
        {
            return View(new IndexModel(operation, suggestedLinkName));
        }

        [HttpGet]
        [Route("{*name}")]
        public async System.Threading.Tasks.Task<IActionResult> ResolveAsync(string name)
        {
            var redirect = await _redirectController.GetRedirectEntry(name);

            if (redirect == null)
            {
                return RedirectToAction("Index", new { operation = IndexModel.Operation.Add, suggestedLinkName = name });
            }

            return Redirect(redirect.Target);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
