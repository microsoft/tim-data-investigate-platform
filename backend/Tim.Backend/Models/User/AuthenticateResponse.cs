// <copyright file="AuthenticateResponse.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.User
{
    using Newtonsoft.Json;

    /// <summary>
    /// Response for authentication.
    /// </summary>
    [JsonObject]
    public class AuthenticateResponse
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
