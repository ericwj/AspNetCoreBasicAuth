// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
namespace Microsoft.AspNetCore.Authentication.Basic
{
    /// <summary>Defaults for the Basic Authentication scheme.</summary>
    public class BasicAuthenticationDefaults
    {
        /// <summary>The default name of the scheme.</summary>
        public const string AuthenticationScheme = "Basic";
        /// <summary>The default display name of the scheme.</summary>
        public const string DisplayName = "Basic Authentication";
        /// <summary>The default realm.</summary>
        public const string Realm = "Basic Realm";
    }
}
