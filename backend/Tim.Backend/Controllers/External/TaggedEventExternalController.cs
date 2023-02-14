// <copyright file="TaggedEventExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using Tim.Backend.Models.TaggedEvents;
    using Tim.Backend.Models.TaggedEvents.Tables;
    using Tim.Backend.Providers.Kusto;

    /// <summary>
    /// TaggedEventExternalControlle manages tagged event data.
    /// </summary>
    [Route("api/taggedevents")]
    [ApiController]
    [Authorize]
    public class TaggedEventExternalController : Controller
    {
        private readonly KustoIngestClient m_ingestClient;
        private readonly IKustoTable m_savedEventTable;
        private readonly IKustoTable m_eventTagTable;
        private readonly IKustoTable m_eventCommentTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedEventExternalController"/> class.
        /// </summary>
        /// <param name="kustoClient">Kusto ingest client.</param>
        /// <param name="savedEventTable">Table spec for saved events.</param>
        /// <param name="eventTagTable">Table spec for event tags.</param>
        /// <param name="eventCommentTable">Table spec for event comments.</param>
        public TaggedEventExternalController(
            KustoIngestClient kustoClient,
            SavedEventTable savedEventTable,
            EventTagTable eventTagTable,
            EventCommentTable eventCommentTable)
        {
            m_ingestClient = kustoClient;
            m_savedEventTable = savedEventTable;
            m_eventTagTable = eventTagTable;
            m_eventCommentTable = eventCommentTable;
        }

        /// <summary>
        /// Endpoint for creating saved events, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of events to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("savedEvents")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateSavedEvent(
            [FromBody] IEnumerable<SavedEvent> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation(requestArray.ToList(), m_savedEventTable, cancellationToken);
        }

        /// <summary>
        /// Endpoint for creating event tags, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of tags to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("tags")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateEventTag(
            [FromBody] IEnumerable<EventTag> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation(requestArray.ToList(), m_eventTagTable, cancellationToken);
        }

        /// <summary>
        /// Endpoint for creating event comments, which supports bulk operations.
        /// </summary>
        /// <param name="requestArray">An array of comments to save.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Response of the request status.</returns>
        [HttpPost("comments")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateEventComment(
            [FromBody] IEnumerable<EventComment> requestArray,
            CancellationToken cancellationToken)
        {
            return await EventCreation(requestArray.ToList(), m_eventCommentTable, cancellationToken);
        }

        private async Task<ActionResult> EventCreation(IReadOnlyList<IKustoEvent> requestArray, IKustoTable kustoTable, CancellationToken cancellationToken)
        {
            if (requestArray.IsNullOrEmpty())
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Either missing request body or request list is empty."));
            }

            await m_ingestClient.WriteAsync(
                requestArray,
                kustoTable,
                cancellationToken);
            return NoContent();
        }
    }
}
