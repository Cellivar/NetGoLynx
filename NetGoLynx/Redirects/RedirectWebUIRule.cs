using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace NetGoLynx.Redirects
{
    /// <summary>
    /// Rule for redirecting API and UI requests to a known host.
    /// </summary>
    public class RedirectWebUIRule : IRule
    {
        private readonly Uri _hostToRedirectTo;
        private readonly ILogger _logger;
        private readonly string _redirectScheme;
        private readonly string _redirectHost;
        private readonly bool _skipPortCheck;

        private readonly PathString _apiPrefix = new PathString("/_");
        private readonly PathString _healthcheckPrefix = new PathString("/_/health");

        /// <summary>
        /// Instantiates a new instance of the <see cref="RedirectWebUIRule"/> class.
        /// </summary>
        /// <param name="hostToRedirectTo">The URI to redirect to if not already on that host.</param>
        /// <param name="logger">The logger to use for logging</param>
        public RedirectWebUIRule(Uri hostToRedirectTo, ILogger logger)
        {
            _hostToRedirectTo = hostToRedirectTo;
            _logger = logger;

            // Precalculate some details to save time on actual redirect analysis.
            _redirectScheme = _hostToRedirectTo.Scheme.ToLowerInvariant();
            _redirectHost = _hostToRedirectTo.Host.ToLowerInvariant();

            // The port on the request will be omitted if it matches the scheme's default port.
            _skipPortCheck = (_redirectScheme == "http" && _hostToRedirectTo.Port == 80)
                || (_redirectScheme == "https" && _hostToRedirectTo.Port == 443);
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

            if (RequestMatchesExpectedHost(request))
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            _logger.LogDebug($"{nameof(RedirectWebUIRule)}: Redirect from {request.Scheme}://{request.Host.Host}:{request.Host.Port} to {_redirectScheme}://{_redirectHost}:{_hostToRedirectTo.Port}");

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

        private bool RequestMatchesExpectedHost(HttpRequest request)
        {
            return (request.Host.Host.ToLowerInvariant() == _redirectHost)
                && (request.Scheme.ToLowerInvariant() == _redirectScheme)
                && (_skipPortCheck || (request.Host.Port == _hostToRedirectTo.Port));
        }
    }
}
