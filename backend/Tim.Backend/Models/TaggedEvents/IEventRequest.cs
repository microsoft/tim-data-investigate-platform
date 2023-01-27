// <copyright file="IEventRequest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    /// <summary>
    /// Interface for event request.
    /// </summary>
    public interface IEventRequest
    {
        /// <summary>
        /// Gets or sets event Id.
        /// </summary>
        string EventId { get; set; }

        /// <summary>
        /// Generated Event.
        /// </summary>
        /// <returns>Generated Event object.</returns>
        IEvent GenerateEvent();

        /// <summary>
        /// Validates if expected values are populated.
        /// </summary>
        void Validate();
    }
}
