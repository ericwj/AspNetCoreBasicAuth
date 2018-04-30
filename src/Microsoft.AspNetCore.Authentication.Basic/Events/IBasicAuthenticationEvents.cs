// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Basic.Events
{
    /// <summary>Defines the interface required for implementors to completely override all
    /// basic authentication events.</summary>
    public interface IBasicAuthenticationEvents
    {
        /// <summary>
        /// Occurs when a (fatal) failure occurs processing the basic authentication pipeline.
        /// </summary>
        /// <param name="context">The context for the current request.</param>
        /// <returns>A task.</returns>
        Task AuthenticationFailed(BasicAuthenticationEventContext context);
        /// <summary>Occurs after the basic authentication credentials are obtained.</summary>
        /// <param name="context">The context for the current request.</param>
        /// <returns>A task.</returns>
        Task CredentialReceived(BasicAuthenticationEventContext context);
        /// <summary>Occurs when an authentication challenge is requested by the application.</summary>
        /// <param name="context">The context for the current request.</param>
        /// <returns>A task.</returns>
        Task Challenge(BasicAuthenticationEventContext context);
        /// <summary>Occurs when <see cref="IAuthenticationService.ForbidAsync(HttpContext, string, AuthenticationProperties)"/>
        /// is requested by the application.</summary>
        Task Forbid(BasicAuthenticationEventContext context);
    }
}
