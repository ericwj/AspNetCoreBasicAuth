using System;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    /// <summary>The exception thrown if an invalid Basic authentication credential is encountered.</summary>
    public class MalformedCredentialException : Exception
    {
        private readonly BasicAuthenticationCredential _credential;

        /// <summary>Creates the exception thrown if an invalid Basic authentication credential is encountered.</summary>
        public MalformedCredentialException() { }
        /// <summary>Creates the exception thrown if an invalid Basic authentication credential is encountered.</summary>
        public MalformedCredentialException(BasicAuthenticationCredential credential) : base("Malformed credential") { _credential = credential; }
        /// <summary>Creates the exception thrown if an invalid Basic authentication credential is encountered.</summary>
        public MalformedCredentialException(BasicAuthenticationCredential credential, Exception inner) : base("Malformed credential", inner) { _credential = credential; }
    }
}
