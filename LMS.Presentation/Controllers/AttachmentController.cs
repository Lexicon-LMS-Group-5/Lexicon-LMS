using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System.Security.Claims;

[Route("api")]
[ApiController]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public AttachmentsController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
    }

    [HttpPost("activities/{activityId:int}/attachments")]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AttachmentReadDto>> UploadAttachment(
        int activityId,
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("Id")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (file is null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        var dto = new AttachmentUpsertDto
        {
            ActivityId = activityId,
            File = file
        };

        var result = await serviceManager.AttachmentService.UploadAttachmentAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetAttachmentsByActivityId), new { activityId }, result);
    }

    [HttpGet("activities/{activityId:int}/attachments")]
    public async Task<ActionResult<List<AttachmentReadDto>>> GetAttachmentsByActivityId(
        int activityId,
        CancellationToken ct)
    {
        var result = await serviceManager.AttachmentService.GetAttachmentsByActivityIdAsync(activityId, ct);
        return Ok(result);
    }

    [HttpGet("attachments/{attachmentId:int}/download")]
    public async Task<IActionResult> DownloadAttachment(int attachmentId, CancellationToken ct)
    {
        var attachment = await serviceManager.AttachmentService.GetAttachmentByIdAsync(attachmentId, ct);

        if (!System.IO.File.Exists(attachment.FilePath))
            return NotFound("File not found on server.");

        var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
            ? "application/octet-stream"
            : attachment.ContentType;

        var fileBytes = await System.IO.File.ReadAllBytesAsync(attachment.FilePath, ct);
        return File(fileBytes, contentType, attachment.FileName);
    }

    [HttpDelete("attachments/{attachmentId:int}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId, CancellationToken ct)
    {
        await serviceManager.AttachmentService.DeleteAttachmentAsync(attachmentId, ct);
        return NoContent();
    }
}