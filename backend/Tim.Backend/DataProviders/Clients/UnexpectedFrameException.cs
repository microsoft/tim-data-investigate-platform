// <copyright file="UnexpectedFrameException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.DataProviders.Clients
{
    using System;

    /// <summary>
    /// UnexpectedFrameException.
    /// </summary>
    public class UnexpectedFrameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedFrameException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public UnexpectedFrameException(string message)
            : base(message)
        {
        }
    }
}
