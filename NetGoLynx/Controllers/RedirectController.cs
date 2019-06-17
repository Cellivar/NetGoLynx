using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: netgolynxapi/Test
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Redirect>>> GetRedirects()
        {
            return await _context.Redirects.ToListAsync();
        }

        // GET: netgolynxapi/Test/5
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
                .FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // PUT: netgolynxapi/Test/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRedirect(int id, Redirect redirect)
        {
            if (id != redirect.RedirectId)
            {
                return BadRequest();
            }

            _context.Entry(redirect).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RedirectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: netgolynxapi/Test
        [HttpPost]
        public async Task<ActionResult<Redirect>> PostRedirect(Redirect redirect)
        {
            _context.Redirects.Add(redirect);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRedirect", new { id = redirect.RedirectId }, redirect);
        }

        // DELETE: netgolynxapi/Test/5
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

        private bool RedirectExists(int id)
        {
            return _context.Redirects.Any(e => e.RedirectId == id);
        }
    }
}
