// <copyright file="EventComment.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;

    /// <summary>
    /// Object that represent Event comment.
    /// </summary>
    public class EventComment : IEvent
    {
        /// <summary>
        /// Gets or sets Event Id.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the time record was created.
        /// </summary>
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets who created the record.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets comments.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets determination.
        /// </summary>
        public string Determination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether comment is deleted or not.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets the name for ingestion mapping.
        /// </summary>
        /// <returns>EventCommentMapping.</returns>
        public string GetMapping()
        {
            return "EventCommentMapping";
        }

        /// <summary>
        /// Gets the name for the comment table.
        /// </summary>
        /// <returns>EventComment.</returns>
        public string GetName()
        {
            return "EventComment";
        }
    }
}
