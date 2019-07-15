using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetGoLynx.Controllers
{
    [Route("_/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> LoginGoogleAsync()
        {
            return NotFound();
            //return Challenge(
            //    new AuthenticationProperties {
            //        RedirectUri = Url.Action()
            //    },
            //    GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginGitHubAsync()
        {
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            return NotFound();
        }
    }
}
