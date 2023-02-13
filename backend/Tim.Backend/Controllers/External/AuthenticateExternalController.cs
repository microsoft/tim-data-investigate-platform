// <copyright file="AuthenticateExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System.Threading;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Serilog;
    using Tim.Backend.Models.User;
    using Tim.Backend.Providers.Helpers;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// AuthenticateExternalController authenticates the user and provides a JWT token.
    /// </summary>
    [Route("/external/user")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateExternalController : Controller
    {
        private readonly IOptions<AuthConfiguration> m_authConfig;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateExternalController"/> class.
        /// </summary>
        /// <param name="authConfig">Represents appropriate configurations.</param>
        public AuthenticateExternalController(IOptions<AuthConfiguration> authConfig)
        {
            m_authConfig = authConfig;
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Authenticate the user credentials.
        /// </summary>
        /// <param name="authenticateRequest">User credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns the JWT token.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Authenticate(
           [FromBody] AuthenticateRequest authenticateRequest,
           CancellationToken cancellationToken)
        {
            authenticateRequest.Validate();

            if (authenticateRequest.Username == m_authConfig.Value.Username && AuthHelper.TimeConstantStringCompare(authenticateRequest.Password, m_authConfig.Value.Password))
            {
                var token = AuthHelper.GenerateJwtToken(authenticateRequest.Username, m_authConfig.Value.SigningKey);
                return Ok(new AuthenticateResponse
                {
                    Username = authenticateRequest.Username,
                    Token = token,
                });
            }

            m_logger.Information("Auth failed.", "AuthenticateExternalController-Authenticate");

            return Problem(statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}
