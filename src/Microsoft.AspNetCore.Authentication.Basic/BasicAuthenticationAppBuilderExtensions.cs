using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;

namespace Microsoft.AspNetCore.Builder
{
    public static class BasicAuthenticationAppBuilderExtensions
    {
		public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, BasicAuthenticationOptions options = null) {
			return app.UseMiddleware<BasicAuthenticationMiddleware>(Options.Create(options ?? new BasicAuthenticationOptions()));
		}
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, Action<BasicAuthenticationOptions> configure = null)
        {
			var options = new BasicAuthenticationOptions();
			configure?.Invoke(options);
            return UseBasicAuthentication(app, options);
		}
	}
}
