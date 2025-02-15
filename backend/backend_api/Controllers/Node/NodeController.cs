﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using backend_api.Models.Node.Requests;
using backend_api.Models.Node.Responses;
using backend_api.Services.Node;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_api.Controllers.Node
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly INodeService _service;

        public NodeController(INodeService service)
        {
            _service = service;
        }
        /// <summary>
        ///     API endpoint for Getting all nodes in the DB
        ///     Returns all node objects
        /// </summary>
        /// <param name=""></param>
        /// <returns>IEnumerable<Node></returns>
        [HttpGet, Authorize]
        [Route("GetAllNodes")]
        public async Task<IEnumerable<Models.Node.Node>> GetAllNodes()
        {
            return await _service.GetAllNodes();
        }
        
        /// <summary>
        ///     API endpoint for getting a specific node
        ///     Returns a node response object containing
        ///     the corresponding node object.
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns>GetNodeResponse<Node></returns>
        [HttpGet, Authorize]
        [Route("GetNode")]
        public async Task<GetNodeResponse> GetNode(int nodeID)
        {
            if (nodeID >= 0 && nodeID != null)
            {
                var request = new GetNodeRequest(nodeID);
                return await _service.GetNode(request);
            }
            else
            {
                throw new Exception("Node ID cannot be null or negative");
            }
        }
        
        /// <summary>
        ///     API endpoint for editing an existing node in
        ///     the DB.
        ///     Returns a response message an HTTP status code
        ///     in an EditNodeResponse object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>EditNodeResponse</returns>
        [HttpPut, Authorize]
        [Route("EditNode")]
        public async Task<EditNodeResponse> EditNode(EditNodeRequest request)
        {
            if (request != null || request.NodeId != null || request.NodeId < 0)
            {
                return await _service.EditNode(request);
            }
            else
            {
                throw new Exception("Request is null or node ID is invalid");
            }
        }
        /// <summary>
        ///     API endpoint for creating a new node in the DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>CreateNodeResponse</returns>
        [HttpPost, Authorize]
        [Route("CreateNode")]
        public async Task<CreateNodeResponse> CreateNode(CreateNodeRequest request)
        {
            if (request != null)
            {
                RecurringJob.AddOrUpdate(() => _service.DeactivateAllNodes(null), "59 23 * * 1-6", TimeZoneInfo.Local);
                return await _service.CreateNode(request);
            }
            else
            {
                throw new Exception("Request object is null");
            }
        }
        
        /// <summary>
        ///     API endpoint for deleting a node in the DB.
        ///     Checks if the request is not null before
        ///     calling the appropriate service method.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DeleteNodeResponse</returns>

        //[Authorize]
        [HttpDelete, Authorize]
        [Route("DeleteNode")]
        public async Task<DeleteNodeResponse> DeleteNode(DeleteNodeRequest request)
        {
            if (request != null)
            {
                return await _service.DeleteNode(request);
            }
            else
            {
                throw new Exception("Request object is null");
            }
        }

        /// <summary>
        /// Activates node after covid questionnaire is submitted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut, Authorize]
        [Route("ActivateNode")]
        public async Task<ActivateNodeResponse> ActivateNode(ActivateNodeRequest request)
        {
            return await _service.ActivateNode(request);
        }

        [HttpPut, Authorize]
        [Route("SaveNodes")]
        public async Task<SaveNodesResponse> SaveNodes(SaveNodesRequest request)
        {
            return await _service.SaveNodes(request);
        }

        
    }
}