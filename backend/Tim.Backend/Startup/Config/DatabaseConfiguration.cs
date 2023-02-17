// <copyright file="DatabaseConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Select which database to use.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Couchbase.
        /// </summary>
        Couchbase,

        /// <summary>
        /// MongoDb.
        /// </summary>
        MongoDb,

        /// <summary>
        /// Redis.
        /// </summary>
        Redis,
    }

    /// <summary>
    /// Swagger configurations.
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfiguration"/> class.
        /// </summary>
        public DatabaseConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets the database selection to use.
        /// </summary>
        [Required]
        public DatabaseType DatabaseType { get; set; } =
            Enum.TryParse(Environment.GetEnvironmentVariable("DATABASE_TYPE"), true, out DatabaseType databaseType)
            ? databaseType
            : DatabaseType.Couchbase;

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