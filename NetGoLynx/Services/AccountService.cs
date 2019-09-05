using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public class AccountService : IAccountService
    {
        private RedirectContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountService(
            RedirectContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<IAccount> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return await _context.Accounts.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<IAccount> Get(int id)
        {
            if (id < 0)
            {
                return null;
            }

            return await _context.Accounts.FindAsync(id);
        }

        public async Task<IAccount> Get(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return null;
            }

            return await Get(GetEmailFromClaimsPrincipal(claimsPrincipal));
        }

        /// <summary>
        /// Gets the currently active user's account.
        /// </summary>
        /// <returns>The currently logged in user's account.</returns>
        public async Task<IAccount> GetFromCurrentUser()
        {
            return await Get(_contextAccessor?.HttpContext?.User);
        }

        public async Task<(IAccount account, bool created)> Create(Account account)
        {
            var existing = await Get(account?.Name);
            if (existing != null)
            {
                return (existing, false);
            }

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return (account, true);
        }

        public async Task<IAccount> GetOrCreate(string name)
        {
            var (account, _) = await Create(new Account() { Name = name });
            return account;
        }

        public async Task<IAccount> GetOrCreate(ClaimsPrincipal claimsPrincipal)
        {
            return await GetOrCreate(GetEmailFromClaimsPrincipal(claimsPrincipal));
        }

        private string GetEmailFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.FindFirstValue(ClaimTypes.Email);
        }
    }
}
