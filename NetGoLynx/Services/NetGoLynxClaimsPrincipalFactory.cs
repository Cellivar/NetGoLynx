using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public class NetGoLynxClaimsPrincipalFactory : INetGoLynxClaimsPrincipalFactory
    {
        private readonly IAccountService _accountService;

        public NetGoLynxClaimsPrincipalFactory(
            IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal principal)
        {
            var account = await _accountService.GetOrCreate(principal);

            var accessClaim = GetAccessLevelAsync(account);

            return new[]
            {
                await accessClaim,
            };
        }

        private async Task<Claim> GetAccessLevelAsync(IAccount account)
        {
            return await Task.FromResult(
                new Claim("AccountAccessLevel", account.Access.ToString(), ClaimValueTypes.String, "NetGoLynx"));
        }
    }
}
