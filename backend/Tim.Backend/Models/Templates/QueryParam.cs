// <copyright file="QueryParam.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Templates
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Define Query params.
    /// </summary>
    public class QueryParam : IValidatableObject
    {
        /// <summary>
        /// Gets or sets object.
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the param is optional.
        /// </summary>
        public bool? Optional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there param are multiple params.
        /// </summary>
        public bool? Multiple { get; set; }

        /// <summary>
        /// Gets or sets query param hint.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Gets or sets query param values.
        /// </summary>
        public IEnumerable<string> Values { get; set; }

        /// <summary>
        /// Validate that all required fields are present as expected.
        /// </summary>
        /// <param name="validationContext">validation context.</param>
        /// <returns>Any located errors.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == "array" && (Values == null || !Values.Any()))
            {
                yield return new ValidationResult("'values' field is required if type is 'array'");
            }
        }
    }
}
