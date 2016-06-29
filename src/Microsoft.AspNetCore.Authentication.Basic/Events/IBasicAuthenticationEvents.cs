using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
    public interface IBasicAuthenticationEvents
    {
		/// <summary>
		/// Occurs when a (fatal) failure occurs processing the basic authentication pipeline.
		/// </summary>
		/// <param name="context">The context for the current request.</param>
		/// <returns>A task.</returns>
		Task AuthenticationFailed(AuthenticationFailedContext context);
		/// <summary>
		/// Provides the handler of the event a chance to replace the basic authentication credentials
		/// and to obtain them from another source than the standard Authorization header.
		/// </summary>
		/// <param name="context">The <see cref="CredentialReceivingContext"/> carries the context
		/// for the current request. Its <see cref="CredentialReceivingContext.Credential"/> is null
		/// upon receiving this event. These credentials may be set by this event, in which case
		/// the normal parsing of the Authorization header does not take place.
		/// </param>
		/// <returns>A task.</returns>
		Task CredentialReceiving(CredentialReceivingContext context);
		/// <summary>Occurs after the basic authentication credentials are obtained.</summary>
		/// <param name="context">The context for the current request.</param>
		/// <returns>A task.</returns>
		Task CredentialReceived(CredentialReceivedContext context);
		/// <summary>Occurs when an authentication challenge is requested by the application.</summary>
		/// <param name="context">The context for the current request.</param>
		/// <returns>A task.</returns>
		Task Challenge(BasicChallengeContext context);
    }
}
