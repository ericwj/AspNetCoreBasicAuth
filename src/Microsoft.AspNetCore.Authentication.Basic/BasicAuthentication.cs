using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    /// <summary>Contains constants and static routines used with or that implement basic authentication.</summary>
    public static class BasicAuthentication
    {
        /// <summary>The name of the incoming authorization header.</summary>
        public const string AuthorizationHeader = "Authorization";
        /// <summary>The name of the outgoing challenge header.</summary>
        public const string WwwAuthenticateHeader = "WWW-Authenticate";
        /// <summary>The constant text in the authorization header identifying the authentication method as Basic.</summary>
        public const string HeaderValuePrefix = "Basic ";
        /// <summary>The format of the text in the challenge header with which a username and password are requested.</summary>
        public const string ChallengeFormat = "Basic realm=\"{0}\"";
        /// <summary>The character that separates username and password in the authorization header value after Base64 decoding.</summary>
        public static readonly char[] PasswordSeparators = new[] { ':' };

        /// <summary>Enumerates the outgoing authentication header values.</summary>
        public static IEnumerable<string> GetBasicAuthenticationHeaderValues(StringValues values)
        {
            var scoic = StringComparison.OrdinalIgnoreCase;
            return
                from header in values
                where header.StartsWith(HeaderValuePrefix, scoic)
                select header;
        }
        /// <summary>Attempts to get the authorization header values that match the Basic authentication scheme.</summary>
        public static bool TryGetAuthorizationHeaderValues(this IHeaderDictionary headers, out IEnumerable<string> values)
        {
            if (headers.TryGetValue(AuthorizationHeader, out var stringValues))
            {
                values = GetBasicAuthenticationHeaderValues(stringValues);
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }
        /// <summary>Attempts to parse a <see cref="BasicAuthenticationCredential"/> from the provided string values and returns the first valid one found.</summary>
        public static bool TryParseHeaderCredentials(this IEnumerable<string> values, out BasicAuthenticationCredential credential, out Exception firstError)
        {
            credential = null;
            Exception error = null;
            firstError = null;
            foreach (var value in values)
            {
                if (BasicAuthenticationCredential.TryParse(value, out credential, out error))
                {
                    firstError = null;
                    return true;
                }
                else if (firstError == null)
                    firstError = error;
            }
            return credential != null;
        }

        /// <summary>Creates a <see cref="ClaimsPrincipal"/> for the specified
        /// <see cref="BasicAuthenticationCredential"/> with the specified
        /// issuer and authentication scheme value.
        /// <para>The <see cref="ClaimsPrincipal"/> will have a single identity
        /// with a <see cref="ClaimTypes.NameIdentifier"/> and
        /// a <see cref="ClaimTypes.Name"/> claim.</para></summary>
        public static ClaimsPrincipal CreateClaimsPrincipal(
            BasicAuthenticationCredential credential,
            string issuer,
            string authenticationScheme)
        {
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
