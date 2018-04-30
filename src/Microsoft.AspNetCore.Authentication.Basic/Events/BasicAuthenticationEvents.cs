using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
    /// <summary>The default implementation of <see cref="IBasicAuthenticationEvents"/>.</summary>
    public class BasicAuthenticationEvents : IBasicAuthenticationEvents
    {
        /// <summary>A default event handler that does nothing.</summary>
        public static Task NoAction(BasicAuthenticationEventContext context)
            => Task.CompletedTask;

        /// <summary>Action method called when authentication failed.</summary>
        public Func<BasicAuthenticationEventContext, Task> OnAuthenticationFailed { get; set; } = NoAction;
        /// <summary>Action method called when challenges occur.</summary>
        public Func<BasicAuthenticationEventContext, Task> OnChallenge { get; set; } = NoAction;
        /// <summary>Action method called when credentials are received.</summary>
        public Func<BasicAuthenticationEventContext, Task> OnCredentialReceived { get; set; } = NoAction;
        /// <summary>Action method called when a forbid result is about to be sent.</summary>
        public Func<BasicAuthenticationEventContext, Task> OnForbid { get; set; } = NoAction;

        /// <summary>Called when authentication failed, delegates to <see cref="OnAuthenticationFailed"/>.</summary>
        public Task AuthenticationFailed(BasicAuthenticationEventContext context) => (OnAuthenticationFailed ?? NoAction).Invoke(context);
        /// <summary>Called when a challenge occurs, delegates to <see cref="OnChallenge"/>.</summary>
        public Task Challenge(BasicAuthenticationEventContext context) => (OnChallenge ?? NoAction).Invoke(context);
        /// <summary>Called when credentials are received, delegates to <see cref="OnCredentialReceived"/>.</summary>
        public Task CredentialReceived(BasicAuthenticationEventContext context) => (OnCredentialReceived ?? NoAction).Invoke(context);
        /// <summary>Called when authorization was not granted, delegates to <see cref="OnForbid"/>.</summary>
        public Task Forbid(BasicAuthenticationEventContext context) => (OnForbid ?? NoAction).Invoke(context);
    }
}
