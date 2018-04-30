// Copyright (c) 2017-2018 Eric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.Basic {
    /// <summary>Represents a Basic Authentication credential.</summary>
    public class BasicAuthenticationCredential {
        /// <summary>The user name.</summary>
        public string Username { get; set; }
        /// <summary>The password.</summary>
        public string Password { get; set; }
        /// <summary>The original header value.</summary>
        public string Header { get; set; }

        /// <summary>Attempts to parse the specified string as a Basic Authentication credential.</summary>
        public static bool TryParse(string header, bool allowEmptyUsername, bool allowEmptyPassword, out BasicAuthenticationCredential credential, out Exception error) {
			try {
				credential = Parse(header, allowEmptyUsername, allowEmptyPassword);
				error = null;
				return true;
			} catch (Exception ex) {
				credential = null;
				error = ex;
				return false;
			}
		}
        /// <summary>Parses the specified string as a Basic Authentication credential and throws if parsing fails.</summary>
        public static BasicAuthenticationCredential Parse(string header, bool allowEmptyUsername, bool allowEmptyPassword) {
			if (header == null) throw new ArgumentNullException(nameof(header));
			if (string.IsNullOrEmpty(header)) throw new ArgumentException(nameof(header));

			var credential = new BasicAuthenticationCredential {
				Header = header
			};

			var prefix = BasicAuthentication.HeaderValuePrefix;
			if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
				var message = $"The string must start with the prefix \"{prefix}\".";
				var edi = ExceptionDispatchInfo.Capture(new FormatException(message));
				throw new MalformedCredentialException(credential, edi.SourceException);
			}

			var value = header.Substring(prefix.Length);
			string pair;
			try {
				pair = Encoding.UTF8.GetString(Convert.FromBase64String(value));
			} catch (FormatException ex) {
				throw new MalformedCredentialException(credential, ex);
			} catch (ArgumentException ex) {
				throw new MalformedCredentialException(credential, ex);
			}

			var parts = pair.Split(BasicAuthentication.PasswordSeparators, 2);
			if (parts.Length < 2)
				throw new MalformedCredentialException(credential);

			credential.Username = parts.First();
			credential.Password = parts.Last();

            if (!allowEmptyUsername && string.IsNullOrWhiteSpace(credential.Username))
                throw new MalformedCredentialException(credential);
            if (!allowEmptyPassword && string.IsNullOrWhiteSpace(credential.Password))
                throw new MalformedCredentialException(credential);

			return credential;
		}
	}
}
