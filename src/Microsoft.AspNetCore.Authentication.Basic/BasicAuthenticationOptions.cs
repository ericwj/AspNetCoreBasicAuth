// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using Microsoft.AspNetCore.Authentication.Basic.Events;
using System;
using System.Net;

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

        /// <summary>Indicates whether null, empty or whitespace-only usernames are allowed.</summary>
        public bool AllowEmptyUsername { get; set; }
        /// <summary>Indicates whether null, empty or whitespace-only passwords are allowed.</summary>
        public bool AllowEmptyPassword { get; set; }
        /// <summary>Indicates whether challenges are allowed to be sent over HTTP.
        /// <para>If false, challenges will be converted to Forbidden if the scheme is not HTTPS.</para>
        /// <para>Configure redirection to HTTPS and/or HSTS to re-enable challenges for 
        /// requests that start over HTTP and add authentication to the ASP.NET pipeline after these features
        /// are added to ensure that HTTPS redirection happens before authentication.</para></summary>
        public bool AllowChallengeOverInsecureTransport { get; set; }
        /// <summary>The reason phrase written when challenges are converted to 
        /// <see cref="HttpStatusCode.Forbidden"/> status if the request was over HTTP.</summary>
        public string InsecureChallengeDeniedReasonPhrase { get; set; }
            = BasicAuthenticationDefaults.InsecureChallengeDeniedReasonPhrase;
        
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
