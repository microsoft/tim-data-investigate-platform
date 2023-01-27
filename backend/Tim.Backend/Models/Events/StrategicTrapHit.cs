// <copyright file="StrategicTrapHit.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object that saves results from executed saved queries.
    /// </summary>
    public class StrategicTrapHit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrategicTrapHit"/> class.
        /// </summary>
        public StrategicTrapHit()
        {
        }

        /// <summary>
        /// Gets or sets Id of the query.
        /// </summary>
        public Guid QueryId { get; set; }

        /// <summary>
        /// Gets or sets Id of the query run.
        /// </summary>
        public Guid QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets Event entity Id.
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the query exection date in UTC.
        /// </summary>
        public DateTime? QueryExecutionDatetTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the row of the events.
        /// </summary>
        public IDictionary<string, object> EventAsJson { get; set; }

        /// <summary>
        /// Gets or sets the time of when event happened.
        /// </summary>
        public DateTime? EntityTime { get; set; }
    }
}
