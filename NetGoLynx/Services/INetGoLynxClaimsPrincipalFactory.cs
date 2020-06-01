using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetGoLynx.Services
{
    public interface INetGoLynxClaimsPrincipalFactory
    {
        Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal principal);
    }
}
