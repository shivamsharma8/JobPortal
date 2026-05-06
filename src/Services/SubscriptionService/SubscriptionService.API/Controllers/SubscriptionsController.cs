using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Services;

namespace SubscriptionService.API.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
[Produces("application/json")]
public class SubscriptionsController(SubscriptionAppService subscriptionService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet("my")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySubscription(CancellationToken ct)
    {
        var result = await subscriptionService.GetMySubscriptionAsync(currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.Error.Code, result.Error.Message });
    }

    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionRequest request, CancellationToken ct)
    {
        var result = await subscriptionService.SubscribeAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    [HttpPost("payment")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentRequest request, CancellationToken ct)
    {
        var result = await subscriptionService.RecordPaymentAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    [HttpGet("billing")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBillingHistory(CancellationToken ct)
    {
        var result = await subscriptionService.GetBillingHistoryAsync(currentUser.UserId, ct);
        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<SubscriptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSubscriptions(CancellationToken ct)
    {
        var result = await subscriptionService.GetAllSubscriptionsAsync(ct);
        return Ok(result.Value);
    }
}
