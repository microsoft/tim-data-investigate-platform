// <copyright file="CreationResultResponse.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using Newtonsoft.Json;

    /// <summary>
    /// Response for creating different events types e.g. tag, comment, saved events.
    /// </summary>
    [JsonObject]
    public record CreationResultResponse
    {
        /// <summary>
        /// Gets or sets the primary identifier specific to this event e.g. ReportGuid.
        /// </summary>
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the creation operation was successful.
        /// </summary>
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message returned if the creation failed.
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
