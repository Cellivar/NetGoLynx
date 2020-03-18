using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: InternalsVisibleTo("NetGoLynx.Tests")]

namespace NetGoLynx
{
    /// <summary>
    /// Program container
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entrypoint
        /// </summary>
        /// <param name="args">Arguments list</param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                // Ensure the database is up to date
                var context = scope.ServiceProvider.GetRequiredService<Data.RedirectContext>();
                context.Database.Migrate();
            }

            host.Run();
        }

        /// <summary>
        /// Asp.net entrypoint method
        /// </summary>
        /// <param name="args">Arguments list</param>
        /// <returns>The constructed <see cref="IWebHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
