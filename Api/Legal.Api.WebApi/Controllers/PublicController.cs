using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Api.WebApi.Controllers
{
    /// <summary>
    /// Public API controller that provides unauthenticated access to specific operations.
    /// This controller allows anonymous access for public-facing functionality that doesn't require authentication.
    /// </summary>
    /// <remarks>
    /// All endpoints in this controller are marked with [AllowAnonymous] and can be accessed without authentication.
    /// This is typically used for:
    /// - Public data retrieval
    /// - File downloads that don't require authentication
    /// - Health checks or status endpoints
    /// - Guest user operations
    /// </remarks>
    [ApiController]
    [Route("api/public")]
    [AllowAnonymous]
    public class PublicController(RequestHandler requestHandler, ILogger<PublicController> logger) : ControllerBase
    {
        /// <summary>
        /// Executes a public query or command for the specified module without requiring authentication.
        /// </summary>
        /// <param name="moduleName">The module to execute the operation against (ADMIN, SHOP, or CHAT)</param>
        /// <param name="queryModel">JSON object containing the request with RequestName and Parameter properties</param>
        /// <returns>The result of the executed operation</returns>
        /// <response code="200">Operation executed successfully</response>
        /// <response code="400">Bad request - invalid parameters or operation failed</response>
        /// <response code="500">Internal server error occurred during execution</response>
        /// <example>
        /// POST /api/public/execute/ADMIN
        /// {
        ///   "RequestName": "GetPublicUserInfo",
        ///   "Parameter": {
        ///     "UserId": "12345"
        ///   }
        /// }
        /// </example>
        /// <remarks>
        /// This endpoint is designed for operations that don't require user authentication such as:
        /// - Retrieving public user profiles
        /// - Getting public content or documents
        /// - Executing guest-accessible queries
        /// - Health check operations
        /// 
        /// The operation will only succeed if the target handler is marked with [AllowAnonymous] attribute.
        /// Operations requiring authentication will throw UnauthorizedAccessException.
        /// </remarks>
        [HttpPost("execute/{moduleName}")]
        public async Task<ActionResult> QueryAsync(
            ModuleName moduleName,
            [FromBody] JObject queryModel)
        {
            try
            {
                dynamic? result = await requestHandler.Execute(
                    moduleName.ToString(),
                    queryModel,
                    HttpContext.RequestAborted);
                if (result is not null && result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, result);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves a file by ID from the specified module without requiring authentication.
        /// </summary>
        /// <param name="moduleName">The module containing the file (ADMIN, SHOP, or CHAT)</param>
        /// <param name="requestName">The name of the query handler that retrieves the file</param>
        /// <param name="id">The unique identifier of the file to retrieve</param>
        /// <returns>The requested file with appropriate content type and headers</returns>
        /// <response code="200">File retrieved successfully</response>
        /// <response code="400">Bad request - invalid parameters or file retrieval failed</response>
        /// <response code="404">File not found</response>
        /// <response code="500">Internal server error occurred during file retrieval</response>
        /// <example>
        /// GET /api/public/ADMIN/GetPublicDocument/file/abc123
        /// Returns the public document with ID "abc123" from the ADMIN module
        /// </example>
        /// <remarks>
        /// This endpoint is designed for downloading public files that don't require authentication such as:
        /// - Public documents or brochures
        /// - Company logos or public images
        /// - Public reports or data exports
        /// - Terms of service or privacy policy documents
        /// 
        /// The file access will only succeed if:
        /// 1. The target query handler is marked with [AllowAnonymous] attribute
        /// 2. The file is marked as publicly accessible in the system
        /// 3. The file exists and is not deleted
        /// 
        /// The response includes appropriate Content-Type headers and enables file download in browsers.
        /// Large files are streamed to optimize memory usage.
        /// </remarks>
        [HttpGet("{moduleName}/{requestName}/file/{id}")]
        public async Task<ActionResult> GetFile(ModuleName moduleName, string requestName, string id)
        {
            try
            {
                JObject keyValuePairs = new()
            {
                { "RequestName", requestName },
                { "Parameter",  JToken.FromObject(new IdParameterModel() { Id = id }) }
            };

                dynamic? responseModel = await requestHandler.Execute(
                    moduleName.ToString(),
                    keyValuePairs,
                    HttpContext.RequestAborted);

                if (responseModel?.Result is not FileResponseModel fileToReturn)
                {
                    return Problem(detail: responseModel?.Error, statusCode: StatusCodes.Status400BadRequest);
                }

                return File(fileToReturn.FileStream, fileToReturn.ContentType, fileToReturn.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}