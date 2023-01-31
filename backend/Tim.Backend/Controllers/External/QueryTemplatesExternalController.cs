// <copyright file="QueryTemplatesExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Tim.Backend.Models.Templates;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// API endpoints for working with query templates.
    /// </summary>
    [Route("/external/templates/queries")]
    [ApiController]
    [Authorize]
    public class QueryTemplatesExternalController : ControllerBase
    {
        private readonly IDatabaseClient m_dbClient;
        private readonly DatabaseConfiguration m_configs;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTemplatesExternalController"/> class.
        /// </summary>
        /// <param name="idbClient">Database client.</param>
        /// <param name="dbConfigs">Configuration for database collection names.</param>
        public QueryTemplatesExternalController(
            IDatabaseClient idbClient,
            IOptions<DatabaseConfiguration> dbConfigs)
        {
            m_dbClient = idbClient;
            m_configs = dbConfigs.Value;
        }

        /// <summary>
        /// Returns all query templates matching the criteria specified in the parameters.
        /// </summary>
        /// <param name="cancellationToken">Cacellation Token.</param>
        /// <param name="since">Date since action happened.</param>
        /// <param name="includeDeleted">Include or do not include deleted template queries.</param>
        /// <returns>List of template queries that meet condition.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<QueryTemplate>>> Get(CancellationToken cancellationToken, DateTime? since, bool includeDeleted)
        {
            var results = await m_dbClient.GetItems<QueryTemplate>(m_configs.QueryTemplatesContainerName);

            // TODO: should investigate how we can do this better using the db, this is a placeholder functionality for now
            return Ok(results.Where(each => each.IsDeleted == includeDeleted && each.Updated > since));
        }

        /// <summary>
        /// Returns a specific template by UUID.
        /// </summary>
        /// <param name="uuid">Guid of a specific query template.</param>
        /// <returns>Query template or not found.</returns>
        [HttpGet("{uuid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QueryTemplate>> Get(Guid uuid)
        {
            var result = await m_dbClient.GetItem<QueryTemplate>(uuid.ToString(), m_configs.QueryTemplatesContainerName);

            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Creates a new query template.
        /// </summary>
        /// <param name="template">Details of template that should be created.</param>
        /// <returns>Status of query template creation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] QueryTemplate template)
        {
            if (await m_dbClient.GetItem<QueryTemplate>(template.Uuid.ToString(), m_configs.QueryTemplatesContainerName) != null)
            {
                return BadRequest("A template with this UUID already exists");
            }

            template.CreatedBy = User.Identity.Name;
            template.UpdatedBy = User.Identity.Name;
            template.Updated = DateTime.UtcNow;

#pragma warning disable IDE0058 // Expression value is never used
            await m_dbClient.AddOrUpdateItem<QueryTemplate>(template.Uuid.ToString(), JsonConvert.SerializeObject(template), m_configs.QueryTemplatesContainerName);
#pragma warning restore IDE0058 // Expression value is never used

            return Ok();
        }

        /// <summary>
        /// Updates an existing template.
        /// </summary>
        /// <param name="uuid">Uuid for existing query template.</param>
        /// <param name="template">Details which should be updated within existing query template.</param>
        /// <returns>Success when updated, error otherwise.</returns>
        [HttpPut("{uuid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(Guid uuid, [FromBody] QueryTemplate template)
        {
            if (template.Uuid != uuid)
            {
                return BadRequest("UUIDs don't match");
            }

            template.UpdatedBy = User.Identity.Name;
            template.Updated = DateTime.UtcNow;

#pragma warning disable IDE0058 // Expression value is never used
            await m_dbClient.AddOrUpdateItem<QueryTemplate>(template.Uuid.ToString(), JsonConvert.SerializeObject(template), m_configs.QueryTemplatesContainerName);
#pragma warning restore IDE0058 // Expression value is never used

            return Ok();
        }

        /// <summary>
        /// Updates a single value within a template.
        /// </summary>
        /// <param name="uuid">Uuid of existing query template.</param>
        /// <param name="patchDocument">Value that needs to be updated.</param>
        /// <returns>Status of request.</returns>
        [HttpPatch("{uuid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Patch(Guid uuid, [FromBody] JsonPatchDocument<QueryTemplate> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("Missing patch");
            }

            var template = await m_dbClient.GetItem<QueryTemplate>(uuid.ToString(), m_configs.QueryTemplatesContainerName);

            if (template == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(template, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest("Patch results in an invalid template");
            }

            template.UpdatedBy = User.Identity.Name;
            template.Updated = DateTime.UtcNow;

#pragma warning disable IDE0058 // Expression value is never used
            await m_dbClient.AddOrUpdateItem<QueryTemplate>(template.Uuid.ToString(), JsonConvert.SerializeObject(template), m_configs.QueryTemplatesContainerName);
#pragma warning restore IDE0058 // Expression value is never used

            return NoContent();
        }

        /// <summary>
        /// Deletes an existing template. Note that this doesn't actually delete the template from the database, it simply sets "IsDeleted" to true.
        /// </summary>
        /// <param name="uuid">Uuid of existing query templated.</param>
        /// <returns>Status of request.</returns>
        [HttpDelete("{uuid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(Guid uuid)
        {
            var template = await m_dbClient.GetItem<QueryTemplate>(uuid.ToString(), m_configs.QueryTemplatesContainerName);
            template.UpdatedBy = User.Identity.Name;
            template.Updated = DateTime.UtcNow;
            template.IsDeleted = true;

#pragma warning disable IDE0058 // Expression value is never used
            await m_dbClient.AddOrUpdateItem<QueryTemplate>(template.Uuid.ToString(), JsonConvert.SerializeObject(template), m_configs.QueryTemplatesContainerName);
#pragma warning restore IDE0058 // Expression value is never used

            return NoContent();
        }
    }
}
