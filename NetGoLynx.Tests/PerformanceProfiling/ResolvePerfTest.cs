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

        private (TimeSpan Time, TResult Result) Time<TResult>(Func<TResult> toTime)
        {
            var timer = Stopwatch.StartNew();
            var result = toTime();
            timer.Stop();
            return (timer.Elapsed, result);
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

                // Warm up the database model. Assert to make sure it doesn't get optimized out.
                var (warmup, model) = Time(() => context.Redirects.FirstOrDefaultAsync());
                Assert.IsNotNull(model, "Failed to warm up context??");
                TestContext.WriteLine($"Warmup call took {warmup.TotalMilliseconds}ms");

                var redirectController = new RedirectApiController(context);

                var (timer, result) = Time(() => redirectController.GetRedirectEntry(redirect2.Name).Result);

                Assert.AreEqual(redirect2.Target, result.Target, "Acquired entry is not correct??");
                Assert.IsTrue(timer.TotalMilliseconds < 250, $"Target resolution time was too slow at {timer.TotalMilliseconds}ms.");
                TestContext.WriteLine($"Test call took {timer.TotalMilliseconds}ms");
            }
        }

        [TestMethod]
        public void ConfirmRepeatResolutionTimeBelowPerfThreshold()
        {
            var dbOps = new DbContextOptionsBuilder<RedirectContext>()
                .UseInMemoryDatabase(databaseName: "Perf_test_repeat_resolver")
                .Options;

            var redirect1 = GetValidRedirect();

            using (var context = new RedirectContext(dbOps))
            {
                context.Add(redirect1);
                context.SaveChanges();

                var redirectController = new RedirectApiController(context);

                var (warmup, model) = Time(() => redirectController.GetRedirectEntry(redirect1.Name).Result);
                Assert.IsNotNull(model, "Failed to warm up context??");
                TestContext.WriteLine($"Warmup call took {warmup.TotalMilliseconds}ms");

                var (timer, result) = Time(() => redirectController.GetRedirectEntry(redirect1.Name).Result);

                Assert.AreEqual(redirect1.Target, result.Target, "Acquired entry is not correct??");
                Assert.IsTrue(timer.TotalMilliseconds < 5, $"Target resolution time was too slow at {timer.TotalMilliseconds}ms.");
                TestContext.WriteLine($"Test call took {timer.TotalMilliseconds}ms");
            }
        }
    }
}
