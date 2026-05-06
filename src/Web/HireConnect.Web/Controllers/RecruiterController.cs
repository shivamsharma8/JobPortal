using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireConnect.Web.Controllers;

[Authorize(Roles = "Recruiter")]
public class RecruiterController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("Gateway");

    public async Task<IActionResult> Dashboard()
    {
        // Get dashboard stats
        var response = await _client.GetAsync("/api/v1/analytics/recruiter/dashboard");
        if (response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<object>();
            ViewBag.Stats = stats;
        }
        return View();
    }

    public async Task<IActionResult> MyJobs()
    {
        // Depending on Gateway structure, get jobs for recruiter
        var response = await _client.GetAsync("/api/v1/jobs/search?recruiterId=me"); // Assuming custom route or filtered search
        if (response.IsSuccessStatusCode)
        {
            var jobs = await response.Content.ReadFromJsonAsync<object>();
            return View(jobs);
        }
        return View();
    }

    public async Task<IActionResult> Subscriptions()
    {
        var response = await _client.GetAsync("/api/v1/subscriptions/my");
        if (response.IsSuccessStatusCode)
        {
            var sub = await response.Content.ReadFromJsonAsync<object>();
            return View(sub);
        }
        return View();
    }
}
