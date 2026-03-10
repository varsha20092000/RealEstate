using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace RealEstate.Web.Controllers;

public class AuthController : Controller
{
    private readonly HttpClient _http;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient();
        _http.BaseAddress = new Uri("https://localhost:7056/");
    }

    // GET /Auth/Login
    public IActionResult Login() => View();

    // GET /Auth/Register
    public IActionResult Register() => View();

    // POST /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var payload = new { email, password };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            var token = data.GetProperty("token").GetString();
            var fullName = data.GetProperty("fullName").GetString();
            var role = data.GetProperty("role").GetString();
            var userId = data.GetProperty("userId").GetString();
            HttpContext.Session.SetString("Token", token!);
            HttpContext.Session.SetString("FullName", fullName!);
            HttpContext.Session.SetString("Role", role!);
            HttpContext.Session.SetString("UserId", userId!);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid email or password!";
        return View();
    }

    // POST /Auth/Register
    [HttpPost]
    public async Task<IActionResult> Register(
        string fullName, string email,
        string password, string role)
    {
        var payload = new { fullName, email, password, role };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            var token = data.GetProperty("token").GetString();
            var name = data.GetProperty("fullName").GetString();
            var userRole = data.GetProperty("role").GetString();

            var registeredUserId = data.GetProperty("userId").GetString();
            HttpContext.Session.SetString("Token", token!);
            HttpContext.Session.SetString("FullName", name!);
            HttpContext.Session.SetString("Role", userRole!);
            HttpContext.Session.SetString("UserId", registeredUserId!);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Registration failed. Try again!";
        return View();
    }

    // GET /Auth/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}