using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
    /// <summary>The base class for the context object of event raised through an <see cref="IBasicAuthenticationEvents"/> instance.</summary>
    public class BasicAuthenticationEventContext : ResultContext<BasicAuthenticationOptions>
    {

        /// <summary>Creats the base class for the context object of event raised through an <see cref="IBasicAuthenticationEvents"/> instance.</summary>
        public BasicAuthenticationEventContext(
            ClaimsPrincipal principal,
            AuthenticationProperties properties,
            HttpContext context,
            AuthenticationScheme scheme,
            BasicAuthenticationOptions options)
            : base(context, scheme, options)
        {
            Principal = principal;
            Properties = properties;
        }

        /// <summary>The incoming credential, if any.</summary>
        public BasicAuthenticationCredential Credential { get; set; }
        /// <summary>The exception that occurred, if any.</summary>
        public Exception Exception { get; set; }
    }
}
