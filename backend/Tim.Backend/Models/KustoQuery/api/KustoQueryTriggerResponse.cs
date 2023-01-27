// <copyright file="KustoQueryTriggerResponse.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery.Api
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Response for query actions.
    /// </summary>
    [JsonObject]
    public class KustoQueryTriggerResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryTriggerResponse"/> class.
        /// </summary>
        /// <param name="id">Guid created for the triggered query run record.</param>
        /// <param name="message">Message is something went wrong.</param>
        public KustoQueryTriggerResponse(Guid id, string message)
        {
            QueryRunId = id.ToString();
            Message = message;
        }

        /// <summary>
        /// Gets or Sets the query run Id.
        /// </summary>
        [JsonProperty("queryRunId")]
        public string QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets the error message if something went wrong. Message will be blank if operation is succesful.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
