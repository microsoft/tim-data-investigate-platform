// <copyright file="EventTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Class that represents tagging an event.
    /// </summary>
    public class EventTag : IKustoEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier associated with the saved event.
        /// </summary>
        [Required]
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets when this tag was created or modified.
        /// </summary>
        [JsonProperty("dateTimeUtc")]
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the user that made this request.
        /// </summary>
        [Required]
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the tag to that will be attached to the related saved event.
        /// </summary>
        [Required]
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tag was deleted or not.
        /// </summary>
        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
