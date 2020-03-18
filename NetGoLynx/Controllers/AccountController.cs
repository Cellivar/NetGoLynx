using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models.Accounts;
using NetGoLynx.Models.Configuration.Authentication;
using NetGoLynx.Services;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Controller for interacting with accounts
    /// </summary>
    [Route("_/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly string[] _availableSchemes;

        public AccountController(IAccountService service, IAuthenticationSchemeProvider authProvider)
        {
            _accountService = service;
            _availableSchemes = authProvider.GetAllSchemesAsync().Result.Select(s => s.DisplayName).ToArray();
        }

        /// <summary>
        /// Displays the login selection page
        /// </summary>
        /// <returns>The login selection view.</returns>
        [HttpGet("Login")]
        public async Task<IActionResult> LoginAsync()
        {
            return View(new LoginModel(_availableSchemes));
        }

        /// <summary>
        /// Log a user into Google.
        /// </summary>
        /// <returns>A redirect to the list view page.</returns>
        [HttpGet("Google")]
        public async Task<IActionResult> LoginGoogleAsync()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("List", "Redirect")
                },
                Google.AuthenticationScheme);
        }

        /// <summary>
        /// Log a user into GitHub.
        /// </summary>
        /// <returns>A redirect to the list view page.</returns>
        [HttpGet("GitHub")]
        public async Task<IActionResult> LoginGitHubAsync()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("LoginSuccess", "Account"),
                },
                GitHub.AuthenticationScheme);
        }

        /// <summary>
        /// Confirm a user is logged into the system as a redirect target.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Success")]
        public async Task<IActionResult> LoginSuccessAsync()
        {
            // Ensure the user exists in the database.
            await _accountService.GetOrCreate(User);

            return RedirectToAction("List", "Redirect");
        }

        /// <summary>
        /// Log a user out.
        /// </summary>
        /// <returns>A redirect to the list view page.</returns>
        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("List", "Redirect");
        }
    }
}
