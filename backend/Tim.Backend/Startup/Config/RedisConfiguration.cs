// <copyright file="RedisConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Redis configurations.
    /// </summary>
    public class RedisConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConfiguration"/> class.
        /// </summary>
        public RedisConfiguration()
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
        public string RedisPassword { get; set; } = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? string.Empty;

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
