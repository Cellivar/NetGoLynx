using System;
using System.Diagnostics;
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

        private int _redirectCount;

        private int RedirectCount
        {
            get
            {
                _redirectCount++;
                return _redirectCount;
            }
        }

        public TestContext TestContext { get; set; }

        private Redirect GetValidRedirect()
        {
            var count = RedirectCount;
            return new Redirect()
            {
                Description = "This is a test valid redirect object " + count,
                Name = "test_redirect" + count,
                Target = "https://google.com/search?q=" + count
            };
        }

        [TestMethod]
        public void ConfirmResolutionTimeBelowPerfThreshold()
        {
            var dbOps = new DbContextOptionsBuilder<RedirectContext>()
                .UseInMemoryDatabase(databaseName: "Perf_test_resolver")
                .Options;

            var redirect1 = GetValidRedirect();
            var redirect2 = GetValidRedirect();

            using (var context = new RedirectContext(dbOps))
            {
                context.Add(redirect1);
                context.Add(redirect2);
                context.SaveChanges();

                var warmupTimer = Stopwatch.StartNew();
                // Warm up the database model. Assert to make sure it doesn't get optimized out.
                var model = context.Redirects.FirstOrDefaultAsync();
                warmupTimer.Stop();
                Assert.IsNotNull(model, "Failed to warm up context??");
                TestContext.WriteLine($"Warmup call took {warmupTimer.ElapsedMilliseconds}ms");

                var redirectController = new RedirectController(context);

                var timer = Stopwatch.StartNew();
                var result = redirectController.GetRedirectEntry(redirect2.Name).Result;
                timer.Stop();

                Assert.AreEqual(redirect2.Target, result.Target, "Acquired entry is not correct??");
                Assert.IsTrue(timer.ElapsedMilliseconds < 250, $"Target resolution time was too slow at {timer.ElapsedMilliseconds}ms.");
                TestContext.WriteLine($"Test call took {timer.ElapsedMilliseconds}ms");
            }
        }
    }
}
