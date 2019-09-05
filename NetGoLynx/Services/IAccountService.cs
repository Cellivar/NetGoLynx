using System.Security.Claims;
using System.Threading.Tasks;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public interface IAccountService
    {
        Task<IAccount> Get(string name);

        Task<IAccount> Get(int id);

        Task<IAccount> Get(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Gets the currently active user's account.
        /// </summary>
        /// <returns>The currently logged in user's account.</returns>
        Task<IAccount> GetFromCurrentUser();

        Task<(IAccount account, bool created)> Create(Account account);

        Task<IAccount> GetOrCreate(string name);

        Task<IAccount> GetOrCreate(ClaimsPrincipal claimsPrincipal);
    }
}
