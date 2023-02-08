// <copyright file="KustoConfiguration.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Azure Resources Section represents service configurations.
    /// </summary>
    public class KustoConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoConfiguration"/> class.
        /// </summary>
        public KustoConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets the kusto query ingest url for the main service cluster.
        /// </summary>
        [Required]
        public string IngestKustoClusterUri { get; set; } = Environment.GetEnvironmentVariable("KUSTO_INGEST_URL");

        /// <summary>
        /// Gets or sets the kusto app secret key.
        /// </summary>
        [Required]
        public string KustoAppKey { get; set; } = Environment.GetEnvironmentVariable("KUSTO_APP_KEY");

        /// <summary>
        /// Gets or sets the kusto App Id used to query kusto for not OBO query executions.
        /// </summary>
        [Required]
        public string KustoAppId { get; set; } = Environment.GetEnvironmentVariable("KUSTO_CLIENT_APP_ID");

        /// <summary>
        /// Gets or sets the kusto database for the main service cluster.
        /// </summary>
        public string KustoDatabase { get; set; } = Environment.GetEnvironmentVariable("KUSTO_DATABASE_NAME") ?? "Research";

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
