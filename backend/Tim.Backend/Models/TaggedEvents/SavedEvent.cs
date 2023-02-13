// <copyright file="SavedEvent.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Class represents a saved event.
    /// </summary>
    public class SavedEvent : IKustoEvent
    {
        /// <summary>
        /// Gets or sets the primary identifier specific to this event e.g. ReportGuid.
        /// </summary>
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp associated with this event e.g. ReportTime.
        /// </summary>
        [JsonProperty("eventTime")]
        public DateTime? EventTime { get; set; }

        /// <summary>
        /// Gets or sets when this saved event was created.
        /// </summary>
        [JsonProperty("dateTimeUtc")]
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the user that created this saved event.
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a complex object that holds all the raw data for this event.
        /// </summary>
        [JsonProperty("eventAsJson")]
        public Dictionary<string, object> EventAsJson { get; set; }

        /// <summary>
        /// Validates the arguments in the request and throws an exception when error is found.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if any argument is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(EventId))
            {
                throw new ArgumentException("Argument must be specified", nameof(EventId));
            }

            if (EventTime == default)
            {
                throw new ArgumentException("Argument must be specified", nameof(EventTime));
            }

            if (string.IsNullOrEmpty(CreatedBy))
            {
                throw new ArgumentException("Argument must be specified", nameof(CreatedBy));
            }

            if (EventAsJson == null || EventAsJson.Count == 0)
            {
                throw new ArgumentException("Argument must be specified", nameof(EventAsJson));
            }
        }
    }
}
