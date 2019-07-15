using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models.RedirectModels;

namespace NetGoLynx.Controllers
{
    [Route("_/[controller]")]
    public class RedirectController : Controller
    {
        private readonly RedirectApiController _redirectController;

        public RedirectController(RedirectApiController redirectController)
        {
            _redirectController = redirectController;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAsync(int highlightId = -1)
        {
            var redirects = await _redirectController.GetRedirectEntriesAsync();
            return View("List", new ListModel(redirects: redirects, id: highlightId));
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddAsync(string suggestedLinkName = "")
        {
            return View("Add", new RedirectMetadata(suggestedLinkName));
        }

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

            var result = await _redirectController.TryCreateRedirectAsync(redirect);
            switch (result.Result)
            {
                case RedirectApiController.OperationResult.Success:
                    return await ListAsync(highlightId: result.Redirect.RedirectId);
                case RedirectApiController.OperationResult.Conflict:
                    model.ErrorMessage = "A link with that name already exists.";
                    return View("Add", model);
                default:
                    model.ErrorMessage = "Unknown problem happened. Try again?";
                    return View("Add", model);
            }
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteAsync(RedirectMetadata model)
        {
            // Confirm the referenced ID is real
            var redirect = await _redirectController.GetRedirect(model.RedirectId);
            if (redirect.Value == null)
            {
                return BadRequest();
            }

            return View("Delete", new RedirectMetadata(redirect.Value));
        }
    }
}
