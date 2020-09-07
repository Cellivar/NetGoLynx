using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
                .AddControllersAsServices();

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new HeaderApiVersionReader("Api-Version");
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(new System.DateTime(2019, 07, 01));
            });

            services.AddMemoryCache();

            services.AddDbContext<RedirectContext>(GetDatabaseContext);

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

            AddOkta(auth);
            AddGoogle(auth);
            AddGitHub(auth);

            // Register services
            services
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<IRedirectService, RedirectService>()
                .AddTransient<INetGoLynxClaimsPrincipalFactory, NetGoLynxClaimsPrincipalFactory>()
                .AddTransient<IFilesystemService, FilesystemService>();

            services
                .TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Configures the internal application configuration.
        /// </summary>
        /// <param name="app">The app builder to configure.</param>
        /// <param name="env">The environment information to use.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/_/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void GetDatabaseContext(DbContextOptionsBuilder options)
        {
            var conStrs = Configuration.GetSection("ConnectionStrings").GetChildren();
            if (conStrs.Count() != 1)
            {
                throw new ArgumentOutOfRangeException("Must provide one (and only one) valid connection string.");
            }

            var conString = conStrs.First();
            switch (conString.Key.ToUpper())
            {
                case "DEFAULTCONNECTION":
                case "SQLITE":
                    options.UseSqlite(conString.Value);
                    break;
                case "SQLSERVER":
                    options.UseSqlServer(conString.Value);
                    break;
                case "POSTGRESQL":
                    options.UseNpgsql(conString.Value);
                    break;
                default:
                    throw new ArgumentException($"Unknown connection string {conString.Value} can't be mapped to a database provider. Check for typos and the list of supported database providers.");
            }

        }

        private void AddGoogle(AuthenticationBuilder auth)
        {
            var googleConfig = Configuration.GetSection("Authentication:Google").Get<Google>();
            if (googleConfig != null && googleConfig.Enabled)
            {
                auth = auth.AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                    options.CallbackPath = new PathString("/_/api/v1/account/signin-google");

                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = AddNetGoLynxClaims
                    };
                });
            }
        }

        private void AddGitHub(AuthenticationBuilder auth)
        {
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

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = AddNetGoLynxClaims
                    };
                });
            }
        }

        private void AddOkta(AuthenticationBuilder auth)
        {
            var oktaConfig = Configuration.GetSection("Authentication:Okta").Get<Okta>();
            if (oktaConfig != null && oktaConfig.Enabled)
            {
                auth = auth.AddOAuth(Okta.AuthenticationScheme, Okta.AuthenticationScheme, options =>
                {
                    options.ClientId = oktaConfig.ClientId;
                    options.ClientSecret = oktaConfig.ClientSecret;

                    options.AuthorizationEndpoint = oktaConfig.AuthorizationEndpoint;
                    options.TokenEndpoint = oktaConfig.TokenEndpoint;
                    options.UserInformationEndpoint = oktaConfig.UserInformationEndpoint;

                    options.CallbackPath = new PathString("/_/api/v1/account/signin-okta");

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(
                                request,
                                HttpCompletionOption.ResponseHeadersRead,
                                context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            context.RunClaimActions(user.RootElement);

                            await AddNetGoLynxClaims(context);
                        }
                    };
                });
            }
        }

        private async Task AddNetGoLynxClaims(OAuthCreatingTicketContext ctx)
        {
            var claimService = ctx.HttpContext.RequestServices.GetRequiredService<INetGoLynxClaimsPrincipalFactory>();
            ctx.Identity.AddClaims(await claimService.GetClaims(ctx.Principal));
        }
    }
}
