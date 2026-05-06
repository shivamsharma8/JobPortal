using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireConnect.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("Gateway");

    public async Task<IActionResult> Dashboard()
    {
        var response = await _client.GetAsync("/api/v1/analytics/platform");
        if (response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<object>();
            ViewBag.PlatformStats = stats;
        }
        return View();
    }

    public async Task<IActionResult> Subscriptions()
    {
        var response = await _client.GetAsync("/api/v1/subscriptions");
        if (response.IsSuccessStatusCode)
        {
            var subs = await response.Content.ReadFromJsonAsync<object>();
            return View(subs);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExportStats([FromForm] string format)
    {
        var response = await _client.GetAsync($"/api/v1/analytics/platform/export?format={format}");
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar ?? $"export.{format}";
            return File(stream, contentType, fileName);
        }
        return RedirectToAction(nameof(Dashboard));
    }
}
