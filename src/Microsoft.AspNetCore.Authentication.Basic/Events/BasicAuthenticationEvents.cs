using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
	public class BasicAuthenticationEvents : IBasicAuthenticationEvents {

		public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.FromResult(0);
		public Func<BasicChallengeContext, Task> OnChallenge { get; set; } = context => Task.FromResult(0);
		public Func<CredentialReceivingContext, Task> OnCredentialReceiving { get; set; } = context => Task.FromResult(0);
		public Func<CredentialReceivedContext, Task> OnCredentialReceived { get; set; } = context => Task.FromResult(0);

		public Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);
		public Task Challenge(BasicChallengeContext context) => OnChallenge(context);
		public Task CredentialReceiving(CredentialReceivingContext context) => OnCredentialReceiving(context);
		public Task CredentialReceived(CredentialReceivedContext context) => OnCredentialReceived(context);
	}
}
