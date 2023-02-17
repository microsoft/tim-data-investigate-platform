// <copyright file="AuthConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Authentication configurations.
    /// </summary>
    public class AuthConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthConfiguration"/> class.
        /// </summary>
        public AuthConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets the signing key for the JWT token.
        /// </summary>
        [Required]
        public string SigningKey { get; set; } = Environment.GetEnvironmentVariable("SIGNING_KEY");

        /// <summary>
        /// Gets or sets the expected username to authenticate.
        /// </summary>
        [Required]
        public string Username { get; set; } = Environment.GetEnvironmentVariable("AUTH_USERNAME");

        /// <summary>
        /// Gets or sets the expected password to authenticate.
        /// </summary>
        [Required]
        public string Password { get; set; } = Environment.GetEnvironmentVariable("AUTH_PASSWORD");

        /// <summary>
        /// Gets or sets the client authority i.e. the tenant id.
        /// </summary>
        [Required]
        public string ClientAuthority { get; set; } = Environment.GetEnvironmentVariable("AUTH_TENANT_ID");

        /// <summary>
        /// Gets or sets the client app id.
        /// </summary>
        [Required]
        public string ClientId { get; set; } = Environment.GetEnvironmentVariable("AUTH_CLIENT_ID");

        /// <summary>
        /// Gets or sets the client app secret.
        /// </summary>
        public string ClientSecret { get; set; } = Environment.GetEnvironmentVariable("AUTH_CLIENT_SECRET");

        /// <summary>
        /// Ensures that all required values are populated.
        /// </summary>
        /// <exception cref="AggregateException">Throws exception if values are not populated. </exception>
        public void Validate()
        {
            var errors = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, new ValidationContext(this), errors, true);

            if (!isValid)
            {
                throw new AggregateException(errors.Select(e => new ValidationException(e.ErrorMessage)));
            }

            if (SigningKey.Length < 16)
            {
                throw new ArgumentException("Must be at least 16 characters long", nameof(SigningKey));
            }
        }
    }
}
