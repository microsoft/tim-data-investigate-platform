// <copyright file="AuthenticateRequest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.User
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Request to authenticate a user.
    /// </summary>
    [JsonObject]
    public sealed class AuthenticateRequest
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Validates the arguments in the request and throws an exception when error is found.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if any argument is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(Username))
            {
                throw new ArgumentException("Argument must be specified", nameof(Username));
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException("Argument must be specified", nameof(Password));
            }
        }
    }
}
