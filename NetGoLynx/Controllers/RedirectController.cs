using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    /// <summary>
    /// Controller for managing redirect objects.
    /// </summary>
    [Route("netgolynxapi/[controller]")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly RedirectContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectController"/> class.
        /// </summary>
        /// <param name="context">The redirect context to use.</param>
        public RedirectController(RedirectContext context)
        {
            _context = context;
        }

        internal async Task<IEnumerable<Redirect>> GetRedirectEntriesAsync()
        {
            return await _context.Redirects.ToListAsync();
        }

        /// <summary>
        /// Get an individual redirect by Id.
        /// </summary>
        /// <param name="id">The Id of the redirect to lookup.</param>
        /// <returns>The corresponding redirect, or an error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Redirect>> GetRedirect(int id)
        {
            var redirect = await _context.Redirects.FindAsync(id);

            if (redirect == null)
            {
                return NotFound();
            }

            return redirect;
        }

        /// <summary>
        /// Get an individual redirect by name.
        /// </summary>
        /// <param name="name">The name of the redirect to lookup.</param>
        /// <returns>The corresponding redirect, or an error.</returns>
        [HttpGet("{name}")]
        public async Task<ActionResult<Redirect>> GetRedirect(string name)
        {
            var redirect = await GetRedirectEntry(name);

            if (redirect == null)
            {
                return NotFound();
            }

            return redirect;
        }

        /// <summary>
        /// Get a redirect by its name
        /// </summary>
        /// <param name="name">The name of the redirect to look for</param>
        /// <returns>The found redirect, or null.</returns>
        internal async Task<Redirect> GetRedirectEntry(string name)
        {
            return await _context.Redirects
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        /// <summary>
        /// Createa a new redirect.
        /// </summary>
        /// <param name="redirect">The redirect to create.</param>
        /// <returns>The new redirect that was created.</returns>
        [HttpPost]
        public async Task<ActionResult<Redirect>> PostRedirect(Redirect redirect)
        {
            var result = await TryCreateRedirectAsync(redirect);

            if (result.Result == OperationResult.Conflict)
            {
                return StatusCode(409, new { message = "Redirect name already exists." });
            }

            return CreatedAtAction("GetRedirect", new { id = result.Redirect.RedirectId }, result.Redirect);
        }

        internal async Task<(Redirect Redirect, OperationResult Result)> TryCreateRedirectAsync(Redirect redirect)
        {
            if (await RedirectExistsAsync(redirect.Name))
            {
                return (null, OperationResult.Conflict);
            }

            _context.Redirects.Add(redirect);
            await _context.SaveChangesAsync();

            return (redirect, OperationResult.Success);
        }

        /// <summary>
        /// Delete a redirect by its Id.
        /// </summary>
        /// <param name="id">The Id of the redirect to delete.</param>
        /// <returns>The redirect that was deleted, or an error.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Redirect>> DeleteRedirect(int id)
        {
            var result = await DeleteRedirectEntry(id);
            switch (result.Result)
            {
                case OperationResult.NotFound:
                    return NotFound();
                case OperationResult.Success:
                default:
                    return result.Redirect;
            }
        }

        internal async Task<(Redirect Redirect, OperationResult Result)> DeleteRedirectEntry(int id)
        {
            var redirect = await _context.Redirects.FindAsync(id);
            if (redirect == null)
            {
                return (null, OperationResult.NotFound);
            }

            _context.Redirects.Remove(redirect);
            await _context.SaveChangesAsync();

            return (redirect, OperationResult.Success);
        }

        internal async Task<bool> RedirectExistsAsync(int id)
        {
            return await _context.Redirects.AnyAsync(e => e.RedirectId == id);
        }

        internal async Task<bool> RedirectExistsAsync(string name)
        {
            return await _context.Redirects.AnyAsync(r => r.Name == name);
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
