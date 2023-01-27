// <copyright file="AuthHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Helpers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Helpers for authentication.
    /// </summary>
    public class AuthHelper
    {
        /// <summary>
        /// Generate a JWT token.
        /// </summary>
        /// <param name="username">Username to encode in the token.</param>
        /// <param name="signingKey">Secret key for signing.</param>
        /// <returns>Returns the JWT token.</returns>
        public static string GenerateJwtToken(string username, string signingKey)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(signingKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", username) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Compare two strings in constant time.
        /// </summary>
        /// <param name="s1">First string.</param>
        /// <param name="s2">Second string.</param>
        /// <returns>True when the strings are equal, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool TimeConstantStringCompare(string s1, string s2)
        {
            var accum = s1.Length ^ s2.Length;
            var mn = Math.Min(s1.Length, s2.Length);

            for (var i = 0; i < mn; i++)
            {
                accum |= s1[i] ^ s2[i];
            }

            return accum == 0;
        }
    }
}