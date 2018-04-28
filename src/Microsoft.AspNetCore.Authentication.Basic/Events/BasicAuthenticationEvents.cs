using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
	public class BasicAuthenticationEvents : IBasicAuthenticationEvents {

		public Func<BasicAuthenticationEventContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;
		public Func<BasicAuthenticationEventContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;
		public Func<BasicAuthenticationEventContext, Task> OnCredentialReceiving { get; set; } = context => Task.CompletedTask;
		public Func<BasicAuthenticationEventContext, Task> OnCredentialReceived { get; set; } = context => Task.CompletedTask;

		public Task AuthenticationFailed(BasicAuthenticationEventContext context) => OnAuthenticationFailed(context);
		public Task Challenge(BasicAuthenticationEventContext context) => OnChallenge(context);
		public Task CredentialReceiving(BasicAuthenticationEventContext context) => OnCredentialReceiving(context);
		public Task CredentialReceived(BasicAuthenticationEventContext context) => OnCredentialReceived(context);
	}
}
