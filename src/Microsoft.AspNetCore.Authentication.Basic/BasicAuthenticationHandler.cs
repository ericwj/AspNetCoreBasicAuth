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

namespace Microsoft.AspNetCore.Authentication.Basic {
	public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions> {
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
			BasicAuthenticationCredential credential = null;
			try {
				var receivingCtx = new CredentialReceivingContext(Context, Options);
				await Options.Events.CredentialReceiving(receivingCtx);
				if (receivingCtx.HandledResponse)
					return AuthenticateResult.Success(receivingCtx.Ticket);
				if (receivingCtx.Skipped)
					return AuthenticateResult.Success(ticket: null);

				credential = receivingCtx.Credential;
				if (credential == null) {
					IEnumerable<string> valid = null;
					if (!Request.Headers.TryGetAuthorizationHeaderValues(out valid))
						return AuthenticateResult.Fail("No authorization header.");
					if (!valid.Any())
						return AuthenticateResult.Fail("No basic authorization header.");
					Exception firstError;
					if (!valid.TryParseHeaderCredentials(out credential, out firstError))
						return AuthenticateResult.Fail(firstError);
				}

				var issuer = Options.ClaimsIssuer ?? Options.Realm;
				var scheme = Options.AuthenticationScheme;
				var receivedCtx = new CredentialReceivedContext(Context, Options, credential) {
					Ticket = new AuthenticationTicket(
						principal: BasicAuthentication.CreateClaimsPrincipal(credential, issuer, scheme),
						properties: new AuthenticationProperties(),
						authenticationScheme: Options.AuthenticationScheme)
				};
				await Options.Events.CredentialReceived(receivedCtx);
				if (receivedCtx.HandledResponse)
					return AuthenticateResult.Success(receivedCtx.Ticket);
				if (receivedCtx.Skipped)
					return AuthenticateResult.Success(ticket: null);

				return AuthenticateResult.Success(receivedCtx.Ticket);
			} catch (Exception ex) {
				var failedCtx = new AuthenticationFailedContext(Context, Options, ex) {
					Error = ex
				};
				await Options.Events.AuthenticationFailed(failedCtx);
				if (failedCtx.HandledResponse)
					return AuthenticateResult.Success(failedCtx.Ticket);
				if (failedCtx.Skipped)
					return AuthenticateResult.Fail(ex);

				return AuthenticateResult.Fail(ex);
			}
		}

		protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context) {
			switch (context.Behavior) {
				case ChallengeBehavior.Forbidden:
					Response.StatusCode = (int)HttpStatusCode.Forbidden;
					return false;
				case ChallengeBehavior.Automatic:
				case ChallengeBehavior.Unauthorized:
				default:
					await SetAuthenticateHeader();
					return false;
			}
		}

		private async Task SetAuthenticateHeader() {
			Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			var challengeCtx = new BasicChallengeContext(Context, Options);
			var challengeValue = string.Format(BasicAuthentication.ChallengeFormat, Options.Realm);
			Response.Headers.AppendCommaSeparatedValues(
				key: BasicAuthentication.WwwAuthenticateHeader,
				values: new[] { challengeValue });
			await Options.Events.Challenge(challengeCtx);
			Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		}

		protected override Task HandleSignOutAsync(SignOutContext context) {
			throw new NotSupportedException();
		}

		protected override Task HandleSignInAsync(SignInContext context) {
			throw new NotSupportedException();
		}
	}
}
