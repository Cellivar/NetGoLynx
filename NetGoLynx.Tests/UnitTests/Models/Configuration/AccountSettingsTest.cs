using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGoLynx.Models;
using NetGoLynx.Models.Configuration;

namespace NetGoLynx.Tests.UnitTests.Models.Configuration
{
    [TestClass]
    public class AccountSettingsTest
    {
        [TestMethod]
        public void AdminUsernameReturnsTrue()
        {
            var goodAccount = new Account()
            {
                Name = "someaccount@example.com"
            };
            var badAccount = new Account()
            {
                Name = "somerando@example.com"
            };
            var settings = new AccountSettings
            {
                AdminList = goodAccount.Name
            };

            Assert.IsTrue(settings.IsAdmin(goodAccount));
            Assert.IsFalse(settings.IsAdmin(badAccount));
        }
    }
}
