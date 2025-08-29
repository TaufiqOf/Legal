using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Legal.Api.WebApi.Controllers;

/// <summary>
/// Controller for executing read-only queries and data retrieval operations across different modules.
/// Provides endpoints for query execution, file downloads, and query discovery.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly ILogger<QueryController> _logger;
    private readonly RequestHandler _requestHandler;

    /// <summary>
    /// Initializes a new instance of the QueryController.
    /// </summary>
    /// <param name="logger">Logger for recording operation logs and errors</param>
    /// <param name="requestHandler">Handler for processing query requests</param>
    public QueryController(ILogger<QueryController> logger, RequestHandler requestHandler)
    {
        _logger = logger;
        _requestHandler = requestHandler;
    }

    /// <summary>
    /// Executes a data query for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="commandModel">JSON object containing the query request with RequestName and Parameter properties</param>
    /// <returns>Query execution result with data and metadata</returns>
    /// <response code="200">Query executed successfully</response>
    /// <response code="400">Invalid request or query execution failed</response>
    /// <example>
    /// POST /api/Query/Execute/ADMIN
    /// {
    ///   "RequestName": "GetUsers",
    ///   "Parameter": {
    ///     "Page": 1,
    ///     "PageSize": 10,
    ///     "SearchTerm": "john",
    ///     "SortBy": "LastName",
    ///     "SortDescending": false
    ///   }
    /// }
    /// </example>
    [HttpPost("Execute/{moduleName}")]
    public async Task<ActionResult> Execute(string moduleName, [FromBody] JObject commandModel)
    {
        try
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var resultModel = await _requestHandler.Execute(moduleName, commandModel, cancellationToken);
            if (resultModel.Success == false)
            {
                return BadRequest(resultModel);
            }
            return Ok(resultModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of all available queries for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <returns>List of query names available in the specified module</returns>
    /// <response code="200">Successfully retrieved query list</response>
    /// <response code="400">Invalid module name or error retrieving queries</response>
    /// <example>
    /// GET /api/Query/ListAll/ADMIN
    /// Returns: ["GetUsers", "GetUserById", "GetUserRoles", "SearchUsers"]
    /// </example>
    [HttpGet("ListAll/{moduleName}")]
    public async Task<ActionResult> ListAll(string moduleName)
    {
        try
        {
            return Ok(await RequestHandler.ListQuery(moduleName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific query including its parameter and response models.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="name">The name of the query (without "Handler" suffix)</param>
    /// <returns>JSON object containing query details, parameter model schema, and response model schema</returns>
    /// <response code="200">Successfully retrieved query details</response>
    /// <response code="400">Invalid module name, query name, or error retrieving details</response>
    /// <example>
    /// GET /api/Query/Detail/ADMIN/GetUsers
    /// Returns:
    /// {
    ///   "CommandName": "GetUsers",
    ///   "ParameterModel": {
    ///     "Page": "Int32",
    ///     "PageSize": "Int32",
    ///     "SearchTerm": "String",
    ///     "SortBy": "String"
    ///   },
    ///   "ResponseModel": {
    ///     "Items": "IEnumerable",
    ///     "TotalCount": "Int32",
    ///     "Page": "Int32"
    ///   }
    /// }
    /// </example>
    [HttpGet("Detail/{moduleName}/{name}")]
    public async Task<ActionResult> Detail(string moduleName, string name)
    {
        try
        {
            return Ok(await RequestHandler.QueryDetails(moduleName, name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Executes a query that returns a downloadable file for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="commandModel">JSON object containing the download query request</param>
    /// <returns>File download with appropriate content type and filename</returns>
    /// <response code="200">File download initiated successfully</response>
    /// <response code="400">Invalid request or file generation failed</response>
    /// <example>
    /// POST /api/Query/download/ADMIN
    /// {
    ///   "RequestName": "ExportUsers",
    ///   "Parameter": {
    ///     "Format": "CSV",
    ///     "DateRange": {
    ///       "StartDate": "2024-01-01",
    ///       "EndDate": "2024-12-31"
    ///     }
    ///   }
    /// }
    /// </example>
    [HttpPost("download/{moduleName}")]
    public async Task<ActionResult> Download(string moduleName, [FromBody] JObject commandModel)
    {
        try
        {
            dynamic? responseModel =
                await _requestHandler.Execute(moduleName, commandModel, HttpContext.RequestAborted);

            if (responseModel.Result is FileResponseModel downloadResponseModel)
            {
                return File(
                    fileStream: downloadResponseModel.FileStream,
                    contentType: downloadResponseModel.ContentType,
                    fileDownloadName: downloadResponseModel.FileName);
            }

            return BadRequest("Invalid DownloadResponseModel");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a specific file by ID using a predefined query handler.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="requestName">The name of the query handler to execute</param>
    /// <param name="id">The unique identifier of the file to retrieve</param>
    /// <returns>File content with appropriate content type and filename</returns>
    /// <response code="200">File retrieved successfully</response>
    /// <response code="400">Invalid request or file not found</response>
    /// <response code="404">File not found</response>
    /// <example>
    /// GET /api/Query/ADMIN/GetUserAvatar/file/123
    /// Returns the avatar file for user with ID 123
    /// </example>
    [HttpGet("{moduleName}/{requestName}/file/{id}")]
    public async Task<ActionResult> GetFile(string moduleName, string requestName, string id)
    {
        try
        {
            JObject keyValuePairs = new()
            {
                { "RequestName", requestName },
                { "Parameter",  JToken.FromObject(new IdParameterModel() { Id = id }) }
            };

            dynamic? responseModel =
                await _requestHandler.Execute(moduleName, keyValuePairs, HttpContext.RequestAborted);
            FileResponseModel? fileToReturn = responseModel?.Result as FileResponseModel;

            if (fileToReturn == null)
            {
                return Problem(detail: responseModel?.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return File(fileToReturn.FileStream, fileToReturn.ContentType, fileToReturn.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}