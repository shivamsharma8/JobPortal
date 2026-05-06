using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HireConnect.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HireConnect.Web.Controllers;

public class AuthController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("Gateway");

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new { model.Email, model.Password });
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result == null) return View(model);

        await SignInUser(result.AccessToken);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", new 
        { 
            model.Email, 
            model.Password, 
            Role = Enum.Parse<int>(model.Role) // Assuming Role Enum in Auth Service: Candidate = 1, Recruiter = 2
        });
        
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Registration failed.");
            return View(model);
        }

        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claims = new List<Claim>(jwtToken.Claims);
        
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };

        authProperties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = token }
        });

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);
    }
}

public record AuthResponse(string AccessToken, string RefreshToken, int ExpiresIn);
