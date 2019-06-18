using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Controllers
{
    [Route("netgolynxapi/[controller]")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly RedirectContext _context;

        public RedirectController(RedirectContext context)
        {
            _context = context;
        }

        internal async Task<IEnumerable<Redirect>> GetRedirectEntriesAsync()
        {
            return await _context.Redirects.ToListAsync();
        }

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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Redirect>> DeleteRedirect(int id)
        {
            var redirect = await _context.Redirects.FindAsync(id);
            if (redirect == null)
            {
                return NotFound();
            }

            _context.Redirects.Remove(redirect);
            await _context.SaveChangesAsync();

            return redirect;
        }

        internal async Task<bool> RedirectExistsAsync(int id)
        {
            return await _context.Redirects.AnyAsync(e => e.RedirectId == id);
        }

        internal async Task<bool> RedirectExistsAsync(string name)
        {
            return await _context.Redirects.AnyAsync(r => r.Name == name);
        }

        public enum OperationResult
        {
            Success,
            Conflict,
        }
    }
}
