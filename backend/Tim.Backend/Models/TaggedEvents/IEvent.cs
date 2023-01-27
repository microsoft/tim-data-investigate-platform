// <copyright file="IEvent.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents
{
    /// <summary>
    /// Event interface.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets name for a given object.
        /// </summary>
        /// <returns>Name of the object.</returns>
        public string GetName();

        /// <summary>
        /// Gets mapping of a given object.
        /// </summary>
        /// <returns>Mapping name of the object.</returns>
        public string GetMapping();
    }
}
