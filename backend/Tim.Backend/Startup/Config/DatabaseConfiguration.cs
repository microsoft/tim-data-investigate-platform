﻿// <copyright file="DatabaseConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// BucketName bucket, scope and collection names.
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
        public string DbUserPassword { get; set; } = Environment.GetEnvironmentVariable("DB_USER_PASSWORD");

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [Required]
        public string BucketName { get; set; } = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "default";

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
