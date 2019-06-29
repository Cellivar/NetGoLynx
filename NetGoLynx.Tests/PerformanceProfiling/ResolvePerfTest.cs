using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGoLynx.Controllers;
using NetGoLynx.Data;
using NetGoLynx.Models;

namespace NetGoLynx.Tests.PerformanceProfiling
{
    [TestClass]
    public class ResolvePerfTest
    {
        private TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }

        private Redirect GetValidRedirect()
        {
            return new Redirect()
            {
                Description = "This is a test valid redirect object",
                Name = "test_redirect",
                Target = "https://google.com"
            };
        }

        [TestMethod]
        public async Task ConfirmResolutionTimeBelowPerfThreshold()
        {
            var dbOps = new DbContextOptionsBuilder<RedirectContext>()
                .UseInMemoryDatabase(databaseName: "Perf_test_resolver")
                .Options;

            var testRedirect = GetValidRedirect();

            using (var context = new RedirectContext(dbOps))
            {
                context.Add(testRedirect);
                context.SaveChanges();
            }

            // New context to avoid performance interactions.
            using (var context = new RedirectContext(dbOps))
            {
                var redirectController = new RedirectController(context);

                var timer = Stopwatch.StartNew();
                var result = redirectController.GetRedirectEntry(testRedirect.Name).Result;
                timer.Stop();

                Assert.AreEqual(testRedirect.Target, result.Target, "Acquired entry is not correct??");
                Assert.IsTrue(timer.ElapsedMilliseconds < 250, $"Target resolution time was too slow at {timer.ElapsedMilliseconds}ms.");
            }
        }
    }
}
