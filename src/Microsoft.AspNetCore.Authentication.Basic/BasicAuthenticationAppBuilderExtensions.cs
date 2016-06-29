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
		public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app) {
			return app.UseMiddleware<BasicAuthenticationMiddleware>(new BasicAuthenticationOptions());
		}
		public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, BasicAuthenticationOptions options) {
			return app.UseMiddleware<BasicAuthenticationMiddleware>(options ?? new BasicAuthenticationOptions());
		}
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, Action<BasicAuthenticationOptions> configure = null)
        {
			var options = new BasicAuthenticationOptions();
			if (configure != null) configure(options);
			return app.UseMiddleware<BasicAuthenticationMiddleware>(options);
		}
	}
}
