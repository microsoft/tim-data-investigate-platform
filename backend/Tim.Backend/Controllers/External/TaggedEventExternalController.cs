// <copyright file="TaggedEventExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Models.TaggedEvents;

    /// <summary>
    /// TaggedEventExternalControlle manages tagged event data.
    /// </summary>
    [Route("/external/taggedevents")]
    [ApiController]
    [Authorize]
    public class TaggedEventExternalController : Controller
    {
        private readonly KustoIngestClient m_ingestClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedEventExternalController"/> class.
        /// </summary>
        /// <param name="kustoClient">Kusto ingest client.</param>
        public TaggedEventExternalController(
            KustoIngestClient kustoClient)
        {
            m_ingestClient = kustoClient;
        }

        /// <summary>
        /// Endpoint for creating saved events, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of events to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("savedEvents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreationResultResponse>>> CreateSavedEvent(
            [FromBody] IEnumerable<SavedEventRequest> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation<SavedEvent>(requestArray, "SavedEvent", cancellationToken);
        }

        /// <summary>
        /// Endpoint for creating event tags, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of tags to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreationResultResponse>>> CreateEventTag(
            [FromBody] IEnumerable<EventTagRequest> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation<EventTag>(requestArray, "EventTag", cancellationToken);
        }

        /// <summary>
        /// Endpoint for creating event comments, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of comments to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreationResultResponse>>> CreateEventComment(
            [FromBody] IEnumerable<EventCommentRequest> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation<EventComment>(requestArray, "EvenComment", cancellationToken);
        }

        private async Task<ActionResult<IEnumerable<CreationResultResponse>>> EventCreation<T>(IEnumerable<IEventRequest> requestArray, string table, CancellationToken cancellationToken)
        {
            if (requestArray is null)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Missing event request body."));
            }

            var results = new List<CreationResultResponse>();

            foreach (var request in requestArray)
            {
                try
                {
                    request.Validate();
                }
                catch (ArgumentException ex)
                {
                    results.Add(new CreationResultResponse
                    {
                        EventId = request.EventId,
                        Error = ex.Message,
                        IsSuccess = false,
                    });
                    continue;
                }

                var entity = request.GenerateEvent();

                try
                {
                    await m_ingestClient.WriteAsync(new List<IEvent>() { entity }, table, cancellationToken);
                    results.Add(new CreationResultResponse
                    {
                        EventId = request.EventId,
                        IsSuccess = true,
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new CreationResultResponse
                    {
                        EventId = request.EventId,
                        Error = ex.Message,
                        IsSuccess = false,
                    });
                }
            }

            return results;
        }
    }
}
