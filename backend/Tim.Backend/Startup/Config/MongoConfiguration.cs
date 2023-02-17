// <copyright file="MongoConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Mongo DB configurations.
    /// </summary>
    public class MongoConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConfiguration"/> class.
        /// </summary>
        public MongoConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets Mongo DB connection string.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        [Required]
        public string DatabaseName { get; set; } = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

        /// <summary>
        /// Gets or sets a value indicating whether cosmosdb is being used.
        /// </summary>
        public bool WithCosmosDb { get; set; } =
            string.Equals(Environment.GetEnvironmentVariable("MONGO_WITH_COSMOSDB"), "true", StringComparison.OrdinalIgnoreCase);

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
