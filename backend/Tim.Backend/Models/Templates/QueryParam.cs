// <copyright file="QueryParam.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Templates
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Define Query params.
    /// </summary>
    public class QueryParam : IValidatableObject
    {
        /// <summary>
        /// Gets or sets object.
        /// </summary>
        [JsonProperty("default")]
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        [Required]
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the param is optional.
        /// </summary>
        [JsonProperty("optional")]
        public bool? Optional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there param are multiple params.
        /// </summary>
        [JsonProperty("multiple")]
        public bool? Multiple { get; set; }

        /// <summary>
        /// Gets or sets query param hint.
        /// </summary>
        [JsonProperty("hint")]
        public string Hint { get; set; }

        /// <summary>
        /// Gets or sets query param values.
        /// </summary>
        [JsonProperty("values")]
        public IEnumerable<string> Values { get; set; }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == "array" && (Values == null || !Values.Any()))
            {
                yield return new ValidationResult("'values' field is required if type is 'array'");
            }
        }
    }
}
