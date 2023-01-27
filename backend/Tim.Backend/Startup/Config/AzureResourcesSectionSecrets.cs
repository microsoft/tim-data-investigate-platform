// <copyright file="AzureResourcesSectionSecrets.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Azure Resources Section secrets represents configuration secrets that come either from secrets.yaml or env variables.
    /// </summary>
    public class AzureResourcesSectionSecrets
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureResourcesSectionSecrets"/> class.
        /// </summary>
        public AzureResourcesSectionSecrets()
        {
        }

        /// <summary>
        /// Gets or sets Redis Hosts for connecting to redis.
        /// </summary>
        [Required]
        public string RedisHosts { get; set; } = Environment.GetEnvironmentVariable("REDIS_HOSTS");

        /// <summary>
        /// Gets or sets Redis Password for connecting to redis.
        /// </summary>
        [Required]
        public string RedisPassword { get; set; } = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

        /// <summary>
        /// Gets or sets and sets database client connection.
        /// </summary>
        [Required]
        public string DatabaseConnection { get; set; } = Environment.GetEnvironmentVariable("DB_CONNECT_STRING") ?? "couchbase://localhost";

        /// <summary>
        /// Gets or sets and Sets the Db user name for the database client connection.
        /// </summary>
        [Required]
        public string DbUserName { get; set; } = Environment.GetEnvironmentVariable("DB_USER_NAME") ?? "Administrator";

        /// <summary>
        /// Gets or Sets the Db password for database client connection.
        /// </summary>
        [Required]
        public string DbUserPassword { get; set; } = Environment.GetEnvironmentVariable("DB_USER_PASSWORD") ?? "password";

        /// <summary>
        /// Gets or sets the kusto app secret key.
        /// </summary>
        [Required]
        public string KustoAppKey { get; set; } = Environment.GetEnvironmentVariable("KUSTO_APP_KEY");

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
