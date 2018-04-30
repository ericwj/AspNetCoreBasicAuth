// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
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
    /// <summary>The authentication handler that handles incoming Basic Authentication headers.</summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        /// <summary>Creates an authentication handler that handles incoming Basic Authentication headers.</summary>
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> monitor,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(monitor, logger, encoder, clock) { }

        /// <summary>Gets or sets the events that can be used to customize incoming
        /// basic authentication requests or outgoing challenges.</summary>
        protected new IBasicAuthenticationEvents Events {
            get => (IBasicAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>Sets the outgoing header value of the <see cref="BasicAuthentication.WwwAuthenticateHeader"/> header,
        /// sets the response status code to <see cref="HttpStatusCode.Unauthorized"/>
        /// and calls <see cref="IBasicAuthenticationEvents.Challenge(BasicAuthenticationEventContext)"/>.</summary>
        protected async Task SetAuthenticateHeader(AuthenticationProperties properties)
        {
            var challenge = string.Format(BasicAuthentication.ChallengeFormat, Options.Realm);
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.Headers.Add(BasicAuthentication.WwwAuthenticateHeader, challenge);
            var context = new BasicAuthenticationEventContext(null, properties, Context, Scheme, Options);
            await Events.Challenge(context);
        }

        /// <summary>Creates the default <see cref="IBasicAuthenticationEvents"/>
        /// as a <see cref="BasicAuthenticationEvents"/> instance.</summary>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new BasicAuthenticationEvents());

        /// <summary>Handle <see cref="IAuthenticationService.ChallengeAsync(Http.HttpContext, string, AuthenticationProperties)"/> by calling <see cref="SetAuthenticateHeader(AuthenticationProperties)"/>.</summary>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            => SetAuthenticateHeader(properties);

        /// <summary>Handle <see cref="IAuthenticationService.ForbidAsync(Http.HttpContext, string, AuthenticationProperties)"/> by 
        /// setting the response status code to <see cref="HttpStatusCode.Forbidden"/> and calling
        /// <see cref="IBasicAuthenticationEvents.Forbid(BasicAuthenticationEventContext)"/></summary>
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new BasicAuthenticationEventContext(null, properties, Context, Scheme, Options);
            return Events.Forbid(context);
        }

        /// <summary>The authentication handler method.</summary>
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
                    if (!valid.TryParseHeaderCredentials(
                        Options.AllowEmptyUsername,
                        Options.AllowEmptyPassword,
                        out credential,
                        out var firstError))
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
