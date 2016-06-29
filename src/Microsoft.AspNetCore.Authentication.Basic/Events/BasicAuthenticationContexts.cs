using System;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Basic.Events {
	/// <summary>The base class for the context object of event raised through an <see cref="IBasicAuthenticationEvents"/> instance.</summary>
	public class BasicAuthenticationContextBase : BaseControlContext {
		public BasicAuthenticationContextBase(HttpContext context, BasicAuthenticationOptions options) : base(context) {
			if (options == null) throw new ArgumentNullException(nameof(options));
			Options = options;
		}
		public BasicAuthenticationOptions Options { get; }
	}
	/// <summary>The context passed to an <see cref="IBasicAuthenticationEvents.AuthenticationFailed(AuthenticationFailedContext)"/> event.</summary>
	public class AuthenticationFailedContext : BasicAuthenticationContextBase {
		public AuthenticationFailedContext(HttpContext context, BasicAuthenticationOptions options, Exception error)
			: base(context, options) { Error = error; }
		/// <summary>Provides error information about the authentication failure.
		/// The exception is of type <see cref="MalformedCredentialException"/> when the
		/// authorization header value could not be parsed.</summary>
		public Exception Error { get; set; }
	}
	/// <summary>The context passed to an <see cref="IBasicAuthenticationEvents.Challenge(BasicChallengeContext)"/> event.</summary>
	public class BasicChallengeContext : BasicAuthenticationContextBase {
		public BasicChallengeContext(HttpContext context, BasicAuthenticationOptions options)
			: base(context, options) { }
	}
	/// <summary>The context passed to an <see cref="IBasicAuthenticationEvents.CredentialReceiving(CredentialReceivingContext)"/> event.</summary>
	public class CredentialReceivingContext : BasicAuthenticationContextBase {
		public CredentialReceivingContext(HttpContext context, BasicAuthenticationOptions options)
			: base(context, options) { }
		/// <summary>The credential to use for the current request.
		/// The value is <langword>null</langword> initially and may be set in the event handler.
		/// If it remains unset, the normal parsing of Authorization headers will take place.
		/// If it is set in the event handler, it will not be altered before the
		/// <see cref="IBasicAuthenticationEvents.CredentialReceived(CredentialReceivedContext)"/> event.</summary>
		public BasicAuthenticationCredential Credential { get; set; }
	}
	/// <summary>The context passed to an <see cref="IBasicAuthenticationEvents.CredentialReceived(CredentialReceivedContext)"/> event.</summary>
	public class CredentialReceivedContext : BasicAuthenticationContextBase {
		public CredentialReceivedContext(HttpContext context, BasicAuthenticationOptions options, BasicAuthenticationCredential credential)
			: base(context, options) { Credential = credential; }
		/// <summary>The credential obtained from the current request.</summary>
		public BasicAuthenticationCredential Credential { get; set; }
	}
}
