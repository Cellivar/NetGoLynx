using System.Collections.Generic;

namespace NetGoLynx.Models.Configuration
{
    /// <summary>
    /// Model for Account-related settings.
    /// </summary>
    public class AccountSettings
    {
        private HashSet<string> _adminList;

        private string _whitelist;

        /// <summary>
        /// Gets or sets the comma-separated list of accounts that have admin.
        /// </summary>
        public string AdminList
        {
            get => _whitelist;
            set
            {
                _whitelist = value;
                _adminList = new HashSet<string>(_whitelist.Split(','));
            }
        }

        /// <summary>
        /// Determine if an account is an admin.
        /// </summary>
        /// <param name="account">The account to check</param>
        /// <returns>True if the username exactly matches an entry in the whitelist.</returns>
        public bool IsAdmin(Account account)
        {
            return account != null && _adminList.Contains(account.Name);
        }
    }
}
