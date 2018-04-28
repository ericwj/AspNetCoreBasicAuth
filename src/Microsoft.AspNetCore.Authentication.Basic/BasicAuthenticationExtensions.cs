using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class BasicAuthenticationExtensions
    {
        private static void DefaultConfigureAction(BasicAuthenticationOptions options) { }

        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddBasicAuthentication(BasicAuthenticationDefaults.AuthenticationScheme, configure);

        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddBasicAuthentication(authenticationScheme, BasicAuthenticationDefaults.DisplayName, configure);

        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
				authenticationScheme, displayName, configure ?? DefaultConfigureAction);
    }
}
