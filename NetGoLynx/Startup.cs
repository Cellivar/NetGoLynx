using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetGoLynx.Data;
using NetGoLynx.Models.Configuration.Authentication;
using NetGoLynx.Services;

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

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new HeaderApiVersionReader("Api-Version");
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(new System.DateTime(2019, 07, 01));
            });

            services.AddMemoryCache();

            services.AddDbContext<RedirectContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Conditionally chain together calls to add auth providers.
            var auth = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = new PathString("/_/Account/Login");
            });

            var googleConfig = Configuration.GetSection("Authentication:Google").Get<Google>();
            if (googleConfig != null && googleConfig.Enabled)
            {
                auth = auth.AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                    options.CallbackPath = new PathString("/_/api/v1/account/signin-google");

                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                });
            }

            var githubConfig = Configuration.GetSection("Authentication:GitHub").Get<GitHub>();
            if (githubConfig != null && githubConfig.Enabled)
            {
                auth = auth.AddGitHub(options =>
                {
                    options.ClientId = githubConfig.ClientId;
                    options.ClientSecret = githubConfig.ClientSecret;

                    options.UserEmailsEndpoint = githubConfig.UserEmailsEndpoint;
                    options.UserInformationEndpoint = githubConfig.UserInformationEndpoint;
                    options.AuthorizationEndpoint = githubConfig.AuthorizationEndpoint;
                    options.TokenEndpoint = githubConfig.TokenEndpoint;

                    options.CallbackPath = new PathString("/_/api/v1/account/signin-github");

                    options.Scope.Add("user:email");

                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                });
            }

            // Register services
            services
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<IRedirectService, RedirectService>();

            services
                .TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


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

            app.UseMvc();
        }
    }
}
