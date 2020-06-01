using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGoLynx.Data;
using NetGoLynx.Models;
using NetGoLynx.Services;

namespace NetGoLynx.Tests.UnitTests.Models.Services
{
    [TestClass]
    public class AccountServiceTests
    {
        private static readonly DbContextOptions<RedirectContext> s_dbOps =
            new DbContextOptionsBuilder<RedirectContext>()
                .UseInMemoryDatabase(databaseName: "account_service_tests")
                .Options;

        private static int s_accountCount;

        private static int AccountCount
        {
            get
            {
                s_accountCount++;
                return s_accountCount;
            }
        }

        private static Account GetValidAccount()
        {
            var count = AccountCount;
            return new Account()
            {
                Access = AccessType.Default,
                AccountId = count,
                Name = $"test_account_{count}@example.com",
            };
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            testContext.WriteLine("Adding default database entries");
            // Add some reference stuff
            using var context = new RedirectContext(s_dbOps);
            context.Add(GetValidAccount());
            context.Add(GetValidAccount());
            context.Add(new Account()
            {
                AccountId = 12345,
                Access = AccessType.Default,
                Name = "GoodAccount@example.com"
            });
            context.SaveChanges();
        }

        [TestMethod]
        public void WhitelistedAccountReturnsAsAdminViaAnyMethod()
        {
            using var context = new RedirectContext(s_dbOps);
            var configDict = new Dictionary<string, string>
            {
                {"AccountSettings:AdminList", "GoodAccount@example.com,OtherGoodAccount@example.com,moregood@example.com"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();
            var accountService = new AccountService(context, null, config);

            // Standard get methods
            Assert.AreEqual(accountService.Get("GoodAccount@example.com").Result.Access, AccessType.Admin);
            Assert.AreEqual(accountService.Get(12345).Result.Access, AccessType.Admin);

            // ClaimsPrincipal overload
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "GoodAccount@example.com")
            }, "mock"));
            Assert.AreEqual(accountService.Get(claims).Result.Access, AccessType.Admin);

            // Create operation
            var newAccount = new Account()
            {
                Access = AccessType.Default,
                Name = "OtherGoodAccount@example.com"
            };
            var (account, created) = accountService.Create(newAccount).Result;
            Assert.IsTrue(created);
            Assert.AreEqual(account.Access, AccessType.Admin);

            Assert.AreEqual(accountService.GetOrCreate("moregood@example.com").Result.Access, AccessType.Admin);
        }

        [TestMethod]
        public void RegularAccountDoesntReturnAsAdmin()
        {
            using var context = new RedirectContext(s_dbOps);
            var configDict = new Dictionary<string, string>
            {
                {"AccountSettings:AdminList", "GoodAccount@example.com"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();
            var accountService = new AccountService(context, null, config);

            Assert.AreNotEqual(accountService.Get("test_account_1@example.com").Result.Access, AccessType.Admin);
        }
    }
}
