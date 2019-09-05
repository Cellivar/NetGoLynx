using System.Collections.Generic;
using System.Threading.Tasks;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public interface IRedirectService
    {
        Task<string> GetRedirectTargetAsync(string name);

        Task<IEnumerable<IRedirect>> GetAsync();

        Task<IRedirect> GetAsync(int id);

        Task<IRedirect> GetAsync(string name);

        Task<bool> TryCreateAsync(Redirect redirect);

        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsAsync(string name);

        Task<(bool result, IRedirect redirect)> DeleteAsync(int id);
    }
}
