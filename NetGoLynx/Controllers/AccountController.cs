using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models.Configuration.Authentication;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Controller for interacting with accounts
    /// </summary>
    [Route("_/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet("Login")]
        public async Task<IActionResult> LoginAsync()
        {
            return View();
        }

        [HttpGet("Google")]
        public async Task<IActionResult> LoginGoogleAsync()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ListAsync", "Redirect")
                },
                Google.AuthenticationScheme);
        }

        [HttpGet("GitHub")]
        public async Task<IActionResult> LoginGitHubAsync()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ListAsync", "Redirect"),
                },
                GitHub.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("ListAsync", "Redirect");
        }
    }
}
