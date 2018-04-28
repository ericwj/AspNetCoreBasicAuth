using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Basic.Events {
	/// <summary>The base class for the context object of event raised through an <see cref="IBasicAuthenticationEvents"/> instance.</summary>
	public class BasicAuthenticationEventContext : ResultContext<BasicAuthenticationOptions> {
		
		public BasicAuthenticationEventContext(
			ClaimsPrincipal principal,
			AuthenticationProperties properties,
			HttpContext context,
			AuthenticationScheme scheme,
			BasicAuthenticationOptions options)
			: base(context, scheme, options) {
            Principal = principal;
            Properties = properties;
        }

		public BasicAuthenticationCredential Credential { get; set; }
		public Exception Exception { get; set; }
	}
}
