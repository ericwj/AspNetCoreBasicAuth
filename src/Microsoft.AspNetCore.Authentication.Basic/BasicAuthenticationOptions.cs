using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Basic.Events;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Authentication.Basic {
    public class BasicAuthenticationOptions : AuthenticationOptions
    {
		public BasicAuthenticationOptions() {
			AuthenticationScheme = BasicAuthentication.DefaultAuthenticationScheme;
			AutomaticAuthenticate = true;
			AutomaticChallenge = true;
		}
        public IBasicAuthenticationEvents Events { get; set; } = new BasicAuthenticationEvents();
		public string Realm { get; set; } = BasicAuthentication.DefaultRealm;
    }
}
