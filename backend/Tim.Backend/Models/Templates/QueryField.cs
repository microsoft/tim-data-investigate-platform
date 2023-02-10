// <copyright file="QueryField.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Templates
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines query field params.
    /// </summary>
    public class QueryField : IValidatableObject
    {
        /// <summary>
        /// Gets or sets query field type.
        /// </summary>
        [Required]
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets query field origin.
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets query field regex.
        /// </summary>
        [JsonProperty("regex")]
        public string Regex { get; set; }

        /// <summary>
        /// Validate that all required fields are present as expected.
        /// </summary>
        /// <param name="validationContext">validation context.</param>
        /// <returns>Any located errors.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == "multiple" && string.IsNullOrWhiteSpace(From))
            {
                yield return new ValidationResult("'from' field is required if type is 'multiple'");
            }
            else if (Type == "match" && string.IsNullOrWhiteSpace(Regex))
            {
                yield return new ValidationResult("'regex' field is required if type is 'match'");
            }
        }
    }
}
