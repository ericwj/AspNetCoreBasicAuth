// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using Microsoft.AspNetCore.Authentication.Basic.Events;
using System;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    /// <summary>Options for Basic Authentication.</summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>Creates default options for Basic Authentication.</summary>
        public BasicAuthenticationOptions()
        {
            Events = new BasicAuthenticationEvents();
        }

        /// <summary>Gets or sets the event handler interface.</summary>
        public new IBasicAuthenticationEvents Events {
            get => (IBasicAuthenticationEvents)base.Events;
            set => base.Events = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>The realm string used to identify this server to users 
        /// when they are challenged for a username and password.</summary>
        public string Realm { get; set; } = BasicAuthenticationDefaults.Realm;

        /// <summary>Validates these options. The default implementation requires
        /// <see cref="Realm"/> to be set and not be whitespace only.</summary>
        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Realm))
                throw new ArgumentException(
					$"The option setting '{nameof(Realm)}' with value '{Realm}' is invalid. " +
					"A value is required and cannot be whitespace only.",
					nameof(Realm));

            base.Validate();
        }
    }
}
