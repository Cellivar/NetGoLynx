using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace NetGoLynx.Redirects
{
    /// <summary>
    /// Rule for redirecting API and UI requests to a known host.
    /// </summary>
    public class RedirectWebUIRule : IRule
    {
        private readonly Uri _hostToRedirectTo;
        private readonly string _redirectScheme;
        private readonly string _redirectHost;

        private readonly PathString _apiPrefix = new PathString("/_");
        private readonly PathString _healthcheckPrefix = new PathString("/_/health");

        /// <summary>
        /// Instantiates a new instance of the <see cref="RedirectWebUIRule"/> class.
        /// </summary>
        /// <param name="hostToRedirectTo">The URI to redirect to if not already on that host.</param>
        public RedirectWebUIRule(Uri hostToRedirectTo)
        {
            _hostToRedirectTo = hostToRedirectTo;
            _redirectScheme = _hostToRedirectTo.Scheme.ToLowerInvariant();
            _redirectHost = _hostToRedirectTo.Host.ToLowerInvariant();
        }

        /// <inheritdoc/>
        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            // If the request doesn't have the API prefix it shouldn't be redirected.
            if (!request.Path.StartsWithSegments(_apiPrefix) || request.Path.StartsWithSegments(_healthcheckPrefix))
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            // If the request matches the expected host it shouldn't be redirected.
            if ((request.Host.Host.ToLowerInvariant() == _redirectHost)
                && request.Host.Port == _hostToRedirectTo.Port
                && (request.Scheme == _redirectScheme))
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            var destination = new UriBuilder(request.GetDisplayUrl())
            {
                Host = _hostToRedirectTo.Host,
                Port = _hostToRedirectTo.Port,
                Scheme = _hostToRedirectTo.Scheme
            };

            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
            response.Headers[HeaderNames.Location] = destination.ToString();
            context.Result = RuleResult.EndResponse;
        }
    }
}
