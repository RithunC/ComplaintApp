using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketWebApp.Interfaces;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/attachments")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachments;

        public AttachmentsController(IAttachmentService attachments)
        {
            _attachments = attachments;
        }

        // Returns null if claim missing/invalid.
        private long? CurrentUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(id, out var uid) ? uid : (long?)null;
        }

        // POST api/attachments/{ticketId}
        // Use multipart/form-data (form-data, key=file, type=File)
        [HttpPost("{ticketId:long}")]
        public async Task<IActionResult> Upload(
            long ticketId,
            IFormFile file,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

#if DEBUG
            if (xUserId is null && !User.Identity?.IsAuthenticated == true)
                return BadRequest("X-User-Id header is required in development.");
#endif

            var currentUserId = xUserId ?? CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var res = await _attachments.UploadAsync(ticketId, currentUserId.Value, file);
            return Ok(res);
        }

        // GET api/attachments/{ticketId}
        [HttpGet("{ticketId:long}")]
        public async Task<IActionResult> GetByTicket(long ticketId)
        {
            var list = await _attachments.GetByTicketAsync(ticketId);
            return Ok(list);
        }

        // GET api/attachments/{attachmentId}/download
        [HttpGet("{attachmentId:long}/download")]
        public async Task<IActionResult> Download(long attachmentId)
        {
            var currentUserId = CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var fileResult = await _attachments.GetDownloadAsync(attachmentId, currentUserId.Value);
            if (fileResult is null)
                return NotFound();

            // Set content-disposition so browsers download with original filename
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileResult.FileName,
                Inline = false
            };
            Response.Headers["Content-Disposition"] = cd.ToString();

            return File(fileResult.Stream, fileResult.ContentType, fileResult.FileName);
        }

        // DELETE api/attachments/{attachmentId}
        [HttpDelete("{attachmentId:long}")]
        public async Task<IActionResult> Delete(long attachmentId)
        {
            var currentUserId = CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var deleted = await _attachments.DeleteAsync(attachmentId, currentUserId.Value);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}