using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Authentication.Basic.Events;
using System.Security.Claims;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Authentication.Basic {
	public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions> {

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

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
			BasicAuthenticationCredential credential = null;
            var principal = new ClaimsPrincipal();
            var properties = new AuthenticationProperties();
            try
            {
				var gotcred = new BasicAuthenticationEventContext(principal, properties, Context, Scheme, Options);
				await Options.Events.CredentialReceiving(gotcred);

				credential = gotcred.Credential;
				if (credential == null) {
                    if (!Request.Headers.TryGetAuthorizationHeaderValues(out var valid))
                        return AuthenticateResult.Fail("No authorization header.");
                    if (!valid.Any())
						return AuthenticateResult.Fail("No authorization header.");
                    if (!valid.TryParseHeaderCredentials(out credential, out var firstError))
                        return AuthenticateResult.Fail(firstError);
                }
                gotcred.Credential = credential;

				var issuer = Options.ClaimsIssuer ?? Options.Realm;
                var ticket = new AuthenticationTicket(
                    principal: BasicAuthentication.CreateClaimsPrincipal(credential, issuer, Scheme.Name),
                    properties: properties,
                    authenticationScheme: Scheme.Name);
				await Options.Events.CredentialReceived(gotcred);
				if (gotcred.Result.Succeeded)
					return AuthenticateResult.Success(gotcred.Result.Ticket);

				return AuthenticateResult.Success(ticket);
			} catch (Exception ex) {
				var nojoy = new BasicAuthenticationEventContext(principal, properties, Context, Scheme, Options) {
					Exception = ex
				};
				await Options.Events.AuthenticationFailed(nojoy);
				if (nojoy.Result.Succeeded)
					return AuthenticateResult.Success(nojoy.Result.Ticket);
				return AuthenticateResult.Fail(ex);
			}
		}
	}
}
