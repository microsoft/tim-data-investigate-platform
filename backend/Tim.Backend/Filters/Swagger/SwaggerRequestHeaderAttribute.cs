// <copyright file="SwaggerRequestHeaderAttribute.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Filters.Swagger
{
    using System;

    /// <summary>
    /// Attribute to indicate to generate documentation for a request header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SwaggerRequestHeaderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerRequestHeaderAttribute"/> class to
        /// document a request header in generated Swagger.
        /// </summary>
        /// <param name="name">Name of the header.</param>
        public SwaggerRequestHeaderAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets name of the header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets description for the request header.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets a value indicating whether the request header is required.
        /// </summary>
        public bool Required { get; init; }
    }
}
