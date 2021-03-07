using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;
using NetGoLynx.Services;

namespace NetGoLynx.Controllers.Api
{
    /// <summary>
    /// Controller for Account related actions.
    /// </summary>
    [ApiVersion("2019-07-01")]
    [Route("_/api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountApiController"/> class.
        /// </summary>
        /// <param name="accountService">The account service to use.</param>
        public AccountApiController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Gets an account by its name.
        /// </summary>
        /// <param name="name">The name to look up.</param>
        /// <returns>The located account, or an error.</returns>
        [HttpGet]
        public async Task<IAccount> GetAccount(string name)
        {
            return await _accountService.Get(name);
        }

        /// <summary>
        /// Gets an account by its id.
        /// </summary>
        /// <param name="id">The ID to look up.</param>
        /// <returns>The located account, or an error.</returns>
        [HttpGet]
        public async Task<IAccount> GetAccount(int id)
        {
            return await _accountService.Get(id);
        }

        /// <summary>
        /// Add an account.
        /// </summary>
        /// <param name="account">The account object to add.</param>
        /// <returns>A redirect to get the account information.</returns>
        [HttpPost]
        public async Task<ActionResult<IAccount>> AddAccount(Account account)
        {
            var (newAccount, isNew) = await _accountService.Create(account);

            switch (isNew)
            {
                case true when newAccount != null:
                    return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
                case false:
                    return StatusCode((int)HttpStatusCode.Conflict, new { message = "Account already exists with that name." });
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to create account!" });
            }
        }
    }
}
