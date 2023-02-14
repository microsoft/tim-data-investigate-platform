// <copyright file="SavedEvent.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Class represents a saved event.
    /// </summary>
    public class SavedEvent : IKustoEvent
    {
        /// <summary>
        /// Gets or sets the primary identifier specific to this event e.g. ReportGuid.
        /// </summary>
        [Required]
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp associated with this event e.g. ReportTime.
        /// </summary>
        [Required]
        [JsonProperty("eventTime")]
        public DateTime? EventTime { get; set; }

        /// <summary>
        /// Gets or sets when this saved event was created.
        /// </summary>
        [JsonProperty("dateTimeUtc")]
        public DateTime? DateTimeUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user that created this saved event.
        /// </summary>
        [Required]
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a complex object that holds all the raw data for this event.
        /// </summary>
        [Required]
        [JsonProperty("eventAsJson")]
        public Dictionary<string, object> EventAsJson { get; set; }
    }
}
