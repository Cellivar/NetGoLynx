using System.Runtime.CompilerServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Asp.net entrypoint method
        /// </summary>
        /// <param name="args">Arguments list</param>
        /// <returns>The constructed <see cref="IWebHostBuilder"/></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
