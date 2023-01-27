// <copyright file="DatabaseClasses.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;

    /// <summary>
    /// Database bucket, scope and collection names.
    /// </summary>
    public class DatabaseClasses
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseClasses"/> class.
        /// </summary>
        public DatabaseClasses()
        {
        }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; } = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "default";

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string Scope { get; set; } = Environment.GetEnvironmentVariable("DATABASE_SCOPE_NAME") ?? "_default";

        /// <summary>
        /// Gets or sets the query template collection name.
        /// </summary>
        public string QueryTemplatesContainerName { get; set; } = Environment.GetEnvironmentVariable("QUERY_TEMPLATE_TABLE_NAME") ?? "QueryTemplate";

        /// <summary>
        /// Gets or sets the Saved query run collection name.
        /// </summary>
        public string QueryRunsContainerName { get; set; } = Environment.GetEnvironmentVariable("QUERY_RUN_TABLE_NAME") ?? "QueryRun";
    }
}
