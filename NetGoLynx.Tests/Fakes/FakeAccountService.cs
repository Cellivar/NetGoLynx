using System.Security.Claims;
using System.Threading.Tasks;
using NetGoLynx.Models;
using NetGoLynx.Services;

namespace NetGoLynx.Tests.Fakes
{
    public class FakeAccountService : IAccountService
    {
        public Task<(IAccount account, bool created)> Create(Account account)
        {
            return Task.Run(() => (GetAccount(), true));
        }

        public Task<IAccount> Get(string name)
        {
            return Task.Run(() => GetAccount(name));
        }

        public Task<IAccount> Get(int id)
        {
            return Task.Run(() => GetAccount());
        }

        public Task<IAccount> Get(ClaimsPrincipal claimsPrincipal)
        {
            return Task.Run(() => GetAccount());
        }

        public Task<IAccount> GetFromCurrentUser()
        {
            return Task.Run(() => GetAccount());
        }

        public Task<IAccount> GetOrCreate(string name)
        {
            return Task.Run(() => GetAccount(name));
        }

        public Task<IAccount> GetOrCreate(ClaimsPrincipal claimsPrincipal)
        {
            return Task.Run(() => GetAccount());
        }

        private IAccount GetAccount(string name = "FakeAccount")
        {
            return new Account()
            {
                Name = name,
                Access = AccessType.Admin,
                AccountId = 42
            };
        }
    }
}
