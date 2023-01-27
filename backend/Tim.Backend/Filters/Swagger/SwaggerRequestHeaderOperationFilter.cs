// <copyright file="SwaggerRequestHeaderOperationFilter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Filters.Swagger
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Operation filter for Swagger generation for documenting request headers.
    /// </summary>
    public class SwaggerRequestHeaderOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply filter to add parameter info for actions marked with <seealso cref="SwaggerRequestHeaderAttribute"/>.
        /// </summary>
        /// <param name="operation">Swagger operation metadata.</param>
        /// <param name="context">Context for the action representing the operation.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var requestHeaderAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<SwaggerRequestHeaderAttribute>();

            if (requestHeaderAttributes.Any())
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                foreach (var attribute in requestHeaderAttributes)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = attribute.Name,
                        In = ParameterLocation.Header,
                        Description = attribute.Description,
                        Required = attribute.Required,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                        },
                    });
                }
            }
        }
    }
}
