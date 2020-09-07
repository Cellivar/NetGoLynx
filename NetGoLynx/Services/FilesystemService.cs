using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Services
{
    public class FilesystemService : IFilesystemService
    {
        private readonly RedirectContext _context;
        private readonly IAccountService _accountService;
        private readonly Lazy<IAccount> _lazyUser;

        private readonly IConfigurationSection _databaseConnectionString;

        private IAccount User
        {
            get { return _lazyUser.Value; }
        }

        public FilesystemService(
            RedirectContext context,
            IAccountService accountService,
            IConfiguration config)
        {
            _context = context;
            _accountService = accountService;

            _databaseConnectionString = config.GetSection("ConnectionStrings").GetChildren().First();

            _lazyUser = new Lazy<IAccount>(() => _accountService.GetFromCurrentUser().Result);
        }

        public async Task<(bool result, string message)> BackupSqliteDatabase()
        {
            if (User.Access != AccessType.Admin)
            {
                return (false, "Unauthorized user");
            }

            if (_databaseConnectionString.Key.ToUpper() != "SQLITE"
                || _databaseConnectionString.Key.ToUpper() != "DEFAULTCONNECTION")
            {
                return (false, "Database is not a SQLite database");
            }

            var dbFile = _databaseConnectionString.Value.Split("=").Last();

            await _context.Database.ExecuteSqlRawAsync($".backup {dbFile}.BAK");

            return (true, dbFile + ".BAK");
        }
    }
}
