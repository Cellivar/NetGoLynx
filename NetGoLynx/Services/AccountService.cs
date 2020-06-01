using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetGoLynx.Data;
using NetGoLynx.Models;
using NetGoLynx.Models.Configuration;

namespace NetGoLynx.Services
{
    public class AccountService : IAccountService
    {
        private readonly RedirectContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AccountSettings _accountSettings;

        public AccountService(
            RedirectContext context,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _accountSettings = configuration.GetSection("AccountSettings").Get<AccountSettings>();
        }

        /// <summary>
        /// Get an account by username
        /// </summary>
        /// <param name="name">The name to get the account by.</param>
        /// <returns>The account with that username, or null.</returns>
        public async Task<IAccount> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Name == name);

            SetAdminFlag(ref account);
            return account;
        }

        public async Task<IAccount> Get(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var account = await _context.Accounts.FindAsync(id);

            SetAdminFlag(ref account);
            return account;
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

            SetAdminFlag(ref account);

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

        private void SetAdminFlag(ref Account account)
        {
            if (account != null)
            {
                account.Access = _accountSettings.IsAdmin(account) ? AccessType.Admin : account.Access;
            }
        }
    }
}
