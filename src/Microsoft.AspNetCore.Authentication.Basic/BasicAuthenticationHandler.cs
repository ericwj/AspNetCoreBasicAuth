using Microsoft.AspNetCore.Authentication.Basic.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> monitor,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(monitor, logger, encoder, clock) { }

        protected new IBasicAuthenticationEvents Events {
            get => (IBasicAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        private async Task SetAuthenticateHeader(AuthenticationProperties properties)
        {
            var challenge = string.Format(BasicAuthentication.ChallengeFormat, Options.Realm);
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.Headers.Add(BasicAuthentication.WwwAuthenticateHeader, challenge);
            var context = new BasicAuthenticationEventContext(null, properties, Context, Scheme, Options);
            await Options.Events.Challenge(context);
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new BasicAuthenticationEvents());

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            => SetAuthenticateHeader(properties);

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            ClaimsPrincipal principal = null;
            BasicAuthenticationCredential credential = null;
            var properties = new AuthenticationProperties();
            try
            {
                if (credential == null)
                {
                    if (!Request.Headers.TryGetAuthorizationHeaderValues(out var valid))
                        return AuthenticateResult.Fail("No authorization header.");
                    if (!valid.Any())
                        return AuthenticateResult.Fail("No authorization header.");
                    if (!valid.TryParseHeaderCredentials(out credential, out var firstError))
                        return AuthenticateResult.Fail(firstError);
                }
                principal = BasicAuthentication.CreateClaimsPrincipal(
                    credential, Options.ClaimsIssuer ?? Options.Realm, Scheme.Name);
                var gotcred = new BasicAuthenticationEventContext(principal, properties, Context, Scheme, Options);
                await Events.CredentialReceived(gotcred);
                if (false == gotcred.Result is null)
                    return gotcred.Result;

                var ticket = new AuthenticationTicket(principal, properties, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                var nojoy = new BasicAuthenticationEventContext(principal, properties, Context, Scheme, Options)
                {
                    Exception = ex
                };
                await Options.Events.AuthenticationFailed(nojoy);
                if (false == nojoy.Result is null)
                    return nojoy.Result;
                return AuthenticateResult.Fail(ex);
            }
        }
    }
}
