// <copyright file="SwaggerConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Swagger configurations.
    /// </summary>
    public class SwaggerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerConfiguration"/> class.
        /// </summary>
        public SwaggerConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets API base path used by Swagger.
        /// </summary>
        public string ApiBasePath { get; set; } = Environment.GetEnvironmentVariable("API_BASE_PATH") ?? "/api";

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
        }
    }
}
