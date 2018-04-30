using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    public static class BasicAuthentication
    {
		public const string DefaultAuthenticationScheme = "Basic";
		public const string DefaultRealm = "Basic Realm";
		public const string AuthorizationHeader = "Authorization";
		public const string WwwAuthenticateHeader = "WWW-Authenticate";
		public const string HeaderValuePrefix = "Basic ";
		public const string ChallengeFormat = "Basic realm=\"{0}\"";
		public static readonly char[] PasswordSeparators = new[] { ':' };

		public static IEnumerable<string> GetWwwAuthenticateHeaderValues(StringValues values) {
			var scoic = StringComparison.OrdinalIgnoreCase;
			return
				from header in values
				where header.StartsWith(HeaderValuePrefix, scoic)
				select header;
        }
		public static bool TryGetAuthorizationHeaderValues(this IHeaderDictionary headers, out IEnumerable<string> values) {
            if (headers.TryGetValue(AuthorizationHeader, out var stringValues))
            {
                values = GetWwwAuthenticateHeaderValues(stringValues);
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }

		public static bool TryParseHeaderCredentials(this IEnumerable<string> values, out BasicAuthenticationCredential credential, out Exception firstError) {
			credential = null;
			Exception error = null;
			firstError = null;
			foreach (var value in values) {
				if (BasicAuthenticationCredential.TryParse(value, out credential, out error)) {
					firstError = null;
					return true;
				} else if (firstError == null)
					firstError = error;
			}
			return credential != null;
        }

		public static ClaimsPrincipal CreateClaimsPrincipal(
			BasicAuthenticationCredential credential, 
			string issuer,
			string authenticationScheme) {
			var id = new Claim(
				ClaimTypes.NameIdentifier,
				credential.Username,
				ClaimValueTypes.String,
				issuer);
			var name = new Claim(
				ClaimTypes.Name,
				credential.Username,
				ClaimValueTypes.String,
				issuer);
			var identity = new ClaimsIdentity(authenticationScheme);
            identity.AddClaim(id);
            identity.AddClaim(name);
			return new ClaimsPrincipal(identity);
		}
	}
}
