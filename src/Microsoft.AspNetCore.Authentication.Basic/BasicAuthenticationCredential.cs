using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.Basic {
	public class BasicAuthenticationCredential {
		public string Username { get; set; }
		public string Password { get; set; }
		public string Header { get; set; }

		public static bool TryParse(string header, out BasicAuthenticationCredential credential, out Exception error) {
			try {
				credential = Parse(header);
				error = null;
				return true;
			} catch (Exception ex) {
				credential = null;
				error = ex;
				return false;
			}
		}
		public static BasicAuthenticationCredential Parse(string header) {
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

			return credential;
		}
	}
}