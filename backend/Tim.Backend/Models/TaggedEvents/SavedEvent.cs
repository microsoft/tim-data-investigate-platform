// <copyright file="SavedEvent.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class represents a saved event.
    /// </summary>
    public class SavedEvent : IEvent
    {
        /// <summary>
        /// Gets or sets an event id.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the time the event occured.
        /// </summary>
        public DateTime? EventTime { get; set; }

        /// <summary>
        /// Gets or sets the time when the Saved event object was created.
        /// </summary>
        public DateTime? DateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the value of who created the event.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the raw value of the event properties.
        /// </summary>
        public Dictionary<string, object> EventAsJson { get; set; }

        /// <summary>
        /// Gets mapping name for SavedEvent.
        /// </summary>
        /// <returns>SavedEventMapping.</returns>
        public string GetMapping()
        {
            return "SavedEventMapping";
        }

        /// <summary>
        /// Gets name for SavedEvent object.
        /// </summary>
        /// <returns>SavedEvent.</returns>
        public string GetName()
        {
            return "SavedEvent";
        }
    }
}
