using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetGoLynx.Data;
using NetGoLynx.Models.Configuration.Authentication;
using Newtonsoft.Json.Linq;

namespace NetGoLynx
{
    /// <summary>
    /// Startup configuration class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the available services.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.AddMemoryCache();

            services.AddDbContext<RedirectContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Conditionally chain together calls to add auth providers.
            var auth = services.AddAuthentication();

            var googleConfig = Configuration.GetSection("Authentication:Google").Get<Google>();
            if (googleConfig.Enabled)
            {
                auth = auth.AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                });
            }

            var githubConfig = Configuration.GetSection("Authentication:GitHub").Get<GitHub>();
            if (githubConfig.Enabled)
            {
                auth = auth.AddOAuth("GitHub", options =>
                {
                    var githubAuthNSection = Configuration.GetSection("Authentication:GitHub");

                    options.ClientId = githubConfig.ClientId;
                    options.ClientSecret = githubConfig.ClientSecret;
                    options.CallbackPath = new PathString("/signin-github");

                    options.AuthorizationEndpoint = githubConfig.AuthorizationEndpoint;
                    options.TokenEndpoint = githubConfig.TokenEndpoint;
                    options.UserInformationEndpoint = githubConfig.UserInformationEndpoint;

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");
                    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user);
                        }
                    };
                });
            }

            var dbOptions = services.BuildServiceProvider().GetRequiredService<DbContextOptions<RedirectContext>>();

            // Kick off an the database context load
            Task.Run(() =>
            {
                using (var context = new RedirectContext(dbOptions))
                {
                    var warmup = context.Redirects.FirstOrDefaultAsync();
                }
            });
        }

        /// <summary>
        /// Configures the internal application configuration.
        /// </summary>
        /// <param name="app">The app builder to configure.</param>
        /// <param name="env">The environment information to use.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
