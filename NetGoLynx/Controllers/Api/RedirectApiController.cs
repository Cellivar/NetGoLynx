using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetGoLynx.Models;
using NetGoLynx.Services;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Controller for managing redirect objects.
    /// </summary>
    [ApiVersion("2019-07-01")]
    [Route("_/api/v1/[controller]")]
    [ApiController]
    public class RedirectApiController : ControllerBase
    {
        private readonly IRedirectService _redirectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectApiController"/> class.
        /// </summary>
        public RedirectApiController(
            IRedirectService redirectService)
        {
            _redirectService = redirectService;
        }

        /// <summary>
        /// Get an individual redirect by Id.
        /// </summary>
        /// <param name="id">The Id of the redirect to lookup.</param>
        /// <returns>The corresponding redirect, or an error.</returns>
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetRedirect(int id)
        {
            var redirect = await _redirectService.GetAsync(id);

            if (redirect == null)
            {
                return NotFound();
            }

            return Ok(redirect);
        }

        /// <summary>
        /// Get an individual redirect by name.
        /// </summary>
        /// <param name="name">The name of the redirect to lookup.</param>
        /// <returns>The corresponding redirect, or an error.</returns>
        [HttpGet("[action]/{name}")]
        public async Task<ActionResult<IRedirect>> GetRedirect(string name)
        {
            var redirect = await GetRedirectEntry(name);

            if (redirect == null)
            {
                return NotFound();
            }

            return Ok(redirect);
        }

        /// <summary>
        /// Get a redirect by its name
        /// </summary>
        /// <param name="name">The name of the redirect to look for</param>
        /// <returns>The found redirect, or null.</returns>
        internal async Task<IRedirect> GetRedirectEntry(string name)
        {
            return await _redirectService.GetAsync(name);
        }

        /// <summary>
        /// Createa a new redirect.
        /// </summary>
        /// <param name="redirect">The redirect to create.</param>
        /// <returns>The new redirect that was created.</returns>
        [HttpPost]
        public async Task<ActionResult<IRedirect>> PostRedirect(Redirect redirect)
        {
            var success = await _redirectService.TryCreateAsync(redirect);

            if (success)
            {
                return CreatedAtAction("GetRedirect", new { id = redirect.RedirectId }, redirect);
            }

            if (redirect.RedirectId == -1)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new { message = "Redirect name already exists." });
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Unknown server error!" });
        }

        /// <summary>
        /// Delete a redirect by its Id.
        /// </summary>
        /// <param name="id">The Id of the redirect to delete.</param>
        /// <returns>The redirect that was deleted, or an error.</returns>
        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<IRedirect>> DeleteRedirect(int id = -1)
        {
            if (id == -1)
            {
                return NotFound();
            }

            var (success, redirect) = await _redirectService.DeleteAsync(id);
            if (success)
            {
                return Ok(redirect);
            }

            return NotFound();
        }

        /// <summary>
        /// The result of an operation.
        /// </summary>
        public enum OperationResult
        {
            /// <summary>
            /// The operation succeeded
            /// </summary>
            Success,

            /// <summary>
            /// The operation failed due to an existing item conflict
            /// </summary>
            Conflict,

            /// <summary>
            /// The operation failed due to not finding an item
            /// </summary>
            NotFound,
        }
    }
}
