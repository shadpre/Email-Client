using Microsoft.AspNetCore.Mvc;
using EmailClient.Api.Models;
using EmailClient.Api.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmailClient.Api.Controllers
{
    /// <summary>
    /// REST API controller for email management operations.
    /// Provides endpoints for connecting to IMAP servers, retrieving emails, and performing deletions.
    /// Follows RESTful conventions and includes proper error handling.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IImapService _imapService;

        /// <summary>
        /// Initializes the email controller with the IMAP service dependency
        /// </summary>
        /// <param name="imapService">The IMAP service interface for email operations</param>
        public EmailController(IImapService imapService)
        {
            _imapService = imapService;
        }

        /// <summary>
        /// Establishes a connection to an IMAP email server
        /// </summary>
        /// <param name="config">IMAP server configuration including host, port, and credentials</param>
        /// <returns>Success message if connection is established</returns>
        /// <response code="200">Connection successful</response>
        /// <response code="400">Invalid configuration or connection failed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("connect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Connect([FromBody] ImapConfig config)
        {
            try
            {
                if (config == null)
                {
                    return BadRequest("IMAP configuration is required");
                }

                if (string.IsNullOrWhiteSpace(config.Server) || 
                    string.IsNullOrWhiteSpace(config.Username) || 
                    string.IsNullOrWhiteSpace(config.Password))
                {
                    return BadRequest("Server, username, and password are required");
                }

                var result = await _imapService.ConnectAsync(config);
                if (result)
                {
                    return Ok("Connected successfully to IMAP server");
                }
                else
                {
                    return BadRequest("Failed to connect to IMAP server. Please check your credentials and server settings.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all emails grouped by sender with statistics
        /// </summary>
        /// <returns>Array of sender groups with email counts and total sizes</returns>
        /// <response code="200">Successfully retrieved email groups</response>
        /// <response code="400">No connection established or operation failed</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("emails-by-sender")]
        [ProducesResponseType(typeof(SenderGroup[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmailsBySender()
        {
            try
            {
                var senderGroups = await _imapService.GetEmailsBySenderAsync();
                return Ok(senderGroups);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and groups emails by sender with optional date filtering
        /// </summary>
        /// <param name="request">Request containing date filter criteria</param>
        /// <returns>List of sender groups with aggregated email information</returns>
        /// <response code="200">Successfully retrieved filtered email groups</response>
        /// <response code="400">No connection established or operation failed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("emails-by-sender/filter")]
        [ProducesResponseType(typeof(SenderGroup[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmailsBySenderWithFilter([FromBody] EmailsByFilterRequest request)
        {
            try
            {
                
                var dateFilter = request?.DateFilter;
                
                var senderGroups = await _imapService.GetEmailsBySenderAsync(dateFilter);
                return Ok(senderGroups);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes specific emails by their unique identifiers
        /// </summary>
        /// <param name="request">Request containing array of email UIDs to delete</param>
        /// <returns>Number of emails successfully deleted</returns>
        /// <response code="200">Emails deleted successfully</response>
        /// <response code="400">Invalid request or operation failed</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmails([FromBody] DeleteRequest request)
        {
            try
            {
                if (request?.EmailUids == null || !request.EmailUids.Any())
                {
                    return BadRequest("Email UIDs are required for deletion");
                }

                var deletedCount = await _imapService.DeleteEmailsAsync(request.EmailUids);
                return Ok(deletedCount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all emails from a specific sender
        /// </summary>
        /// <param name="senderEmail">Email address of the sender whose emails should be deleted</param>
        /// <returns>Number of emails successfully deleted</returns>
        /// <response code="200">Emails deleted successfully</response>
        /// <response code="400">Invalid sender email or operation failed</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete-by-sender/{senderEmail}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmailsBySender(string senderEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    return BadRequest("Sender email address is required");
                }

                var deletedCount = await _imapService.DeleteEmailsBySenderAsync(senderEmail);
                return Ok(deletedCount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes emails from a specific sender that match the date filter criteria
        /// </summary>
        /// <param name="senderEmail">Email address of the sender whose emails should be deleted</param>
        /// <param name="request">Request containing the date filter criteria</param>
        /// <returns>Number of emails successfully deleted</returns>
        /// <response code="200">Emails deleted successfully</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete-by-sender-filtered/{senderEmail}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmailsBySenderWithFilter(
            string senderEmail, 
            [FromBody] EmailsByFilterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    return BadRequest("Sender email address is required");
                }

                var deletedCount = await _imapService.DeleteEmailsBySenderWithFilterAsync(senderEmail, request?.DateFilter);
                return Ok(deletedCount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnects from the IMAP server and cleans up resources
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">Disconnected successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("disconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Disconnect()
        {
            try
            {
                _imapService.Disconnect();
                return Ok("Disconnected from IMAP server");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current processing status for long-running operations
        /// </summary>
        /// <returns>Processing status information</returns>
        /// <response code="200">Status retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("processing-status")]
        [ProducesResponseType(typeof(ProcessingStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProcessingStatus()
        {
            try
            {
                var status = _imapService.GetProcessingStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
