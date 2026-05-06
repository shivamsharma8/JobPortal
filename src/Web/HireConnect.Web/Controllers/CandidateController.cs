using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireConnect.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class CandidateController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("Gateway");

    public async Task<IActionResult> Dashboard()
    {
        // Get applications
        var response = await _client.GetAsync("/api/v1/applications/my");
        if (response.IsSuccessStatusCode)
        {
            var apps = await response.Content.ReadFromJsonAsync<object>(); // DTO mapping left for view logic
            ViewBag.Applications = apps;
        }
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var response = await _client.GetAsync("/api/v1/profiles/me");
        if (response.IsSuccessStatusCode)
        {
            var profile = await response.Content.ReadFromJsonAsync<object>();
            return View(profile);
        }
        return View();
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> SearchJobs([FromQuery] string q, [FromQuery] string location)
    {
        var response = await _client.GetAsync($"/api/v1/jobs/search?q={q}&location={location}");
        if (response.IsSuccessStatusCode)
        {
            var jobs = await response.Content.ReadFromJsonAsync<object>();
            return View(jobs);
        }
        return View();
    }

    [HttpPost("apply/{jobId:guid}")]
    public async Task<IActionResult> Apply(Guid jobId)
    {
        var response = await _client.PostAsJsonAsync($"/api/v1/applications", new { JobId = jobId, ResumeUrl = "http://example.com/resume.pdf" });
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Successfully applied!";
        }
        return RedirectToAction(nameof(SearchJobs));
    }
}
