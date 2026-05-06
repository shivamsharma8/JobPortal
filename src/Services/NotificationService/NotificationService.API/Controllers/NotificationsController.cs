using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Services;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Produces("application/json")]
[Authorize]
public class NotificationsController(NotificationAppService notificationService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications(CancellationToken ct)
    {
        var result = await notificationService.GetMyNotificationsAsync(currentUser.UserId, ct);
        return Ok(result.Value);
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(UnreadCountResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var result = await notificationService.GetUnreadCountAsync(currentUser.UserId, ct);
        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        var result = await notificationService.MarkAsReadAsync(id, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound();
    }

    [HttpPatch("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await notificationService.MarkAllReadAsync(currentUser.UserId, ct);
        return Ok(new { message = "All notifications marked as read." });
    }
}
