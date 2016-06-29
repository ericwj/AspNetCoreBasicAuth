using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;

namespace Microsoft.AspNetCore.Authentication.Basic {
	public class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
	{
		public BasicAuthenticationMiddleware(
			RequestDelegate next,
			IOptions<BasicAuthenticationOptions> options,
			ILoggerFactory loggerFactory,
			UrlEncoder encoder)
			: base(next, options, loggerFactory, encoder)
		{
			if (next == null) throw new ArgumentNullException(nameof(next));
			if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
			if (encoder == null) throw new ArgumentNullException(nameof(encoder));
			if (options == null) throw new ArgumentNullException(nameof(options));

			if (Options.Events == null) Options.Events = new Events.BasicAuthenticationEvents();
			if (string.IsNullOrEmpty(Options.Realm)) Options.Realm = BasicAuthentication.DefaultRealm;
		}

		protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler()
		{
			return new BasicAuthenticationHandler();
		}
	}
}
