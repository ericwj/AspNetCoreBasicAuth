using Microsoft.AspNetCore.Authentication.Basic.Events;
using System;

namespace Microsoft.AspNetCore.Authentication.Basic
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public BasicAuthenticationOptions()
        {
            Events = new BasicAuthenticationEvents();
        }

		public new IBasicAuthenticationEvents Events {
            get => (IBasicAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        public string Realm { get; set; } = BasicAuthenticationDefaults.Realm;

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
