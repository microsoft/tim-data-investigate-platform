// <copyright file="EventTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;

    /// <summary>
    /// Class that represents tagging.
    /// </summary>
    public class EventTag : IEvent
    {
        /// <summary>
        /// Gets or sets event id that will be tagged.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the time taggingt took place.
        /// </summary>
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets who created the tag.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tag is deleted or not.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets mapping value for ingestion.
        /// </summary>
        /// <returns>EventTagMapping.</returns>
        public string GetMapping()
        {
            return "EventTagMapping";
        }

        /// <summary>
        /// Gets Name where tags will be written.
        /// </summary>
        /// <returns>EventTag.</returns>
        public string GetName()
        {
            return "EventTag";
        }
    }
}
