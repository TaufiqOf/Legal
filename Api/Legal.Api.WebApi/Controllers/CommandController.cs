using Legal.Service.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Legal.Api.WebApi.Controllers;

/// <summary>
/// Controller for executing business commands across different modules.
/// Provides endpoints for command execution, file uploads, and command discovery.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;
    private readonly RequestHandler _requestHandler;

    /// <summary>
    /// Initializes a new instance of the CommandController.
    /// </summary>
    /// <param name="logger">Logger for recording operation logs and errors</param>
    /// <param name="requestHandler">Handler for processing command requests</param>
    public CommandController(ILogger<CommandController> logger, RequestHandler requestHandler)
    {
        _logger = logger;
        _requestHandler = requestHandler;
    }

    /// <summary>
    /// Executes a business command for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="commandModel">JSON object containing the command request with RequestName and Parameter properties</param>
    /// <returns>Command execution result with success status and response data</returns>
    /// <response code="200">Command executed successfully</response>
    /// <response code="400">Invalid request or command execution failed</response>
    /// <example>
    /// POST /api/Command/Execute/ADMIN
    /// {
    ///   "RequestName": "CreateUser",
    ///   "Parameter": {
    ///     "Email": "user@example.com",
    ///     "Password": "SecurePassword123",
    ///     "FirstName": "John",
    ///     "LastName": "Doe"
    ///   }
    /// }
    /// </example>
    [HttpPost("Execute/{moduleName}")]
    public async Task<ActionResult> Execute(string moduleName, [FromBody] JObject commandModel)
    {
        try
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var response = await _requestHandler.Execute(moduleName, commandModel, cancellationToken);
            if (response.Success == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Executes a command with file upload capabilities for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <returns>Command execution result including uploaded file information</returns>
    /// <response code="200">Command and file upload executed successfully</response>
    /// <response code="400">Invalid request or command execution failed</response>
    /// <remarks>
    /// The request must be sent as multipart/form-data with:
    /// - data: JSON string containing the command request
    /// - files: One or more files to upload
    /// 
    /// Example form data:
    /// - data: {"RequestName": "UploadDocument", "Parameter": {"DocumentType": "Contract"}}
    /// - file: [attached files]
    /// </remarks>
    [HttpPost("Upload/{moduleName}")]
    [Consumes(contentType: "multipart/form-data")]
    public async Task<ActionResult> Upload(string moduleName)
    {
        try
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var from = Request.Form;
            JObject commandModel = JObject.Parse(Request.Form["data"]);

            return Ok(await _requestHandler.Execute(moduleName, commandModel, cancellationToken, from));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of all available commands for the specified module.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <returns>List of command names available in the specified module</returns>
    /// <response code="200">Successfully retrieved command list</response>
    /// <response code="400">Invalid module name or error retrieving commands</response>
    /// <example>
    /// GET /api/Command/ListAll/ADMIN
    /// Returns: ["CreateUser", "EditUser", "DeleteUser", "ChangePassword"]
    /// </example>
    [HttpGet("ListAll/{moduleName}")]
    public async Task<ActionResult> ListAll(string moduleName)
    {
        try
        {
            return Ok(await RequestHandler.ListCommands(moduleName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific command including its parameter and response models.
    /// </summary>
    /// <param name="moduleName">The name of the module (e.g., "ADMIN", "SHOP", "CHAT")</param>
    /// <param name="name">The name of the command (without "Handler" suffix)</param>
    /// <returns>JSON object containing command details, parameter model schema, and response model schema</returns>
    /// <response code="200">Successfully retrieved command details</response>
    /// <response code="400">Invalid module name, command name, or error retrieving details</response>
    /// <example>
    /// GET /api/Command/Detail/ADMIN/CreateUser
    /// Returns:
    /// {
    ///   "CommandName": "CreateUser",
    ///   "ParameterModel": {
    ///     "Email": "String",
    ///     "Password": "String",
    ///     "FirstName": "String",
    ///     "LastName": "String"
    ///   },
    ///   "ResponseModel": {
    ///     "Id": "Int32",
    ///     "Email": "String",
    ///     "FirstName": "String",
    ///     "LastName": "String"
    ///   }
    /// }
    /// </example>
    [HttpGet("Detail/{moduleName}/{name}")]
    public async Task<ActionResult> Detail(string moduleName, string name)
    {
        try
        {
            return Ok(await RequestHandler.CommandDetails(moduleName, name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}