// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Basic;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>Extension methods with a receiver of <see cref="AuthenticationBuilder"/>
    /// with which Basic Authentication can be added to the ASP.NET Core authentication pipeline.</summary>
    public static class BasicAuthenticationExtensions
    {
        /// <summary>The default configuration action does nothing.</summary>
        public static void DefaultConfigureAction(BasicAuthenticationOptions options) { }

        /// <summary>Add Basic Authentication with the default settings and an optional action with which to override these defaults.</summary>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddBasicAuthentication(BasicAuthenticationDefaults.AuthenticationScheme, configure);

        /// <summary>Adds basic authentication with a custom authentication scheme name.</summary>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddBasicAuthentication(authenticationScheme, BasicAuthenticationDefaults.DisplayName, configure);

        /// <summary>Adds basic authentication with a custom authentication scheme name and display name.</summary>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
				authenticationScheme, displayName, configure ?? DefaultConfigureAction);
    }
}
