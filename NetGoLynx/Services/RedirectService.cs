using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly RedirectContext _context;
        private readonly IAccountService _accountService;
        private readonly Lazy<IAccount> _lazyUser;

        private IAccount User
        {
            get { return _lazyUser.Value; }
        }

        public RedirectService(
            RedirectContext context,
            IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;

            _lazyUser = new Lazy<IAccount>(() => _accountService.GetFromCurrentUser().Result);
        }

        public async Task<string> GetRedirectTargetAsync(string name)
        {
            var redirect = await _context.Redirects.FirstOrDefaultAsync(r => r.Name == name);

            return redirect?.Target;
        }

        /// <summary>
        /// Gets all redirects an account may acquire.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IRedirect>> GetAsync()
        {
            if (User == null)
            {
                return null;
            }

            if (User.MayView(null))
            {
                // Admins can see anything, even null things!
                return _context.Redirects;
            }
            else
            {
                return await _context.Redirects.Where(r => r.AccountId == User.AccountId)
                    .ToListAsync();
            }
        }

        public async Task<IRedirect> GetAsync(int id)
        {
            if (User == null)
            {
                return null;
            }
            var redirect = await _context.Redirects.FindAsync(id);

            if (!User.MayView(redirect))
            {
                return null;
            }

            return redirect;
        }

        public async Task<IRedirect> GetAsync(string name)
        {
            if (User == null)
            {
                return null;
            }
            var redirect = await _context.Redirects.FirstOrDefaultAsync(r => r.Name == name);

            if (!User.MayView(redirect))
            {
                return null;
            }

            return redirect;
        }

        public async Task<bool> TryCreateAsync(Redirect redirect)
        {
            if (await ExistsAsync(redirect.Name))
            {
                redirect.RedirectId = -1;
                return false;
            }

            if (User?.AccountId == null)
            {
                return false;
            }

            redirect.AccountId = User.AccountId;
            _context.Redirects.Add(redirect);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Redirects.AnyAsync(r => r.RedirectId == id);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Redirects.AnyAsync(r => r.Name == name);
        }

        public async Task<(bool result, IRedirect redirect)> DeleteAsync(int id)
        {
            if (User == null)
            {
                return (false, null);
            }

            if (!(await GetAsync(id) is Redirect redirect) || !User.MayView(redirect))
            {
                return (false, null);
            }

            _context.Redirects.Remove(redirect);
            await _context.SaveChangesAsync();

            return (true, redirect);
        }
    }
}
