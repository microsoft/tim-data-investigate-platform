// <copyright file="EventCommentRequest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Request to create, modify or delete a comment associated with a saved event.
    /// </summary>
    [JsonObject]
    public sealed class EventCommentRequest : IEventRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier associated with the saved event.
        /// </summary>
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets when this comment was created or modified.
        /// </summary>
        [JsonProperty("dateTimeUtc")]
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the user that made this request.
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a short description that will be attached to the related saved event.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets whether the associated saved event is considered malicious, suspicious etc.
        /// </summary>
        [JsonProperty("determination")]
        public string Determination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this comment was deleted or not.
        /// </summary>
        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Generates a new event comment.
        /// </summary>
        /// <returns>Event comment object.</returns>
        public IEvent GenerateEvent()
        {
            return new EventComment
            {
                EventId = EventId,
                DateTimeUtc = DateTimeUtc,
                CreatedBy = CreatedBy,
                Comment = Comment,
                Determination = Determination,
                IsDeleted = IsDeleted,
            };
        }

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

            if (string.IsNullOrEmpty(CreatedBy))
            {
                throw new ArgumentException("Argument must be specified", nameof(CreatedBy));
            }

            if (string.IsNullOrEmpty(Determination))
            {
                throw new ArgumentException("Argument must be specified", nameof(Determination));
            }
        }
    }
}
