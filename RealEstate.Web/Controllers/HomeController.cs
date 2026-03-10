using Microsoft.AspNetCore.Mvc;

namespace RealEstate.Web.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _http;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient();
        _http.BaseAddress = new Uri("https://localhost:7056/");
        _http.DefaultRequestHeaders.Add("Accept", "application/json");

    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _http.GetAsync("api/Properties/search?isFeatured=true&pageSize=6");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.PropertiesJson = content;
            }
        }
        catch
        {
            ViewBag.PropertiesJson = null;
        }
        return View();
    }
    public async Task<IActionResult> Image(string path)
    {
        try
        {
            var response = await _http.GetAsync($"https://localhost:7056{path}");
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                return File(bytes, contentType);
            }
        }
        catch { }
        return NotFound();
    }
    public async Task<IActionResult> Properties(
        string? city, string? propertyType,
        string? listingType, decimal? minPrice,
        decimal? maxPrice, int? bedrooms, int page = 1)
    {
        try
        {
            var url = $"api/Properties/search?page={page}&pageSize=9";
            if (!string.IsNullOrEmpty(city)) url += $"&city={city}";
            if (!string.IsNullOrEmpty(propertyType)) url += $"&propertyType={propertyType}";
            if (!string.IsNullOrEmpty(listingType)) url += $"&listingType={listingType}";
            if (minPrice.HasValue) url += $"&minPrice={minPrice}";
            if (maxPrice.HasValue) url += $"&maxPrice={maxPrice}";
            if (bedrooms.HasValue) url += $"&bedrooms={bedrooms}";

            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.PropertiesJson = content;
            }
        }
        catch
        {
            ViewBag.PropertiesJson = null;
        }

        ViewBag.City = city;
        ViewBag.PropertyType = propertyType;
        ViewBag.ListingType = listingType;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.Bedrooms = bedrooms;
        ViewBag.Page = page;

        return View();
    }

    public async Task<IActionResult> PropertyDetail(Guid id)
    {
        try
        {
            _http.DefaultRequestHeaders.CacheControl =
                new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
            var response = await _http.GetAsync($"api/Properties/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.PropertyJson = content;
            }
        }
        catch
        {
            ViewBag.PropertyJson = null;
        }

        ViewBag.IsLoggedIn = HttpContext.Session.GetString("Token") != null;
        ViewBag.Token = HttpContext.Session.GetString("Token");
        return View();
    }
    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("Token");
        if (token == null) return RedirectToAction("Login", "Auth");

        ViewBag.FullName = HttpContext.Session.GetString("FullName");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.Token = token;

        // Get user inquiries
        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.GetAsync("api/Inquiries");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.InquiriesJson = content;
            }

            // Get visit bookings
            var bookingsResponse = await _http.GetAsync("api/VisitBookings");
            if (bookingsResponse.IsSuccessStatusCode)
            {
                var content = await bookingsResponse.Content.ReadAsStringAsync();
                ViewBag.BookingsJson = content;
            }
        }
        catch
        {
            ViewBag.InquiriesJson = null;
            ViewBag.BookingsJson = null;
        }

        return View();
    }
    public async Task<IActionResult> AgentDashboard()
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");

        if (token == null) return RedirectToAction("Login", "Auth");
        if (role != "Agent") return RedirectToAction("Index");

        ViewBag.FullName = HttpContext.Session.GetString("FullName");
        ViewBag.Token = token;

        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Get properties
            var propsResponse = await _http.GetAsync("api/Properties/search?pageSize=50");
            if (propsResponse.IsSuccessStatusCode)
                ViewBag.PropertiesJson = await propsResponse.Content.ReadAsStringAsync();

            // Get inquiries
            var inqResponse = await _http.GetAsync("api/Inquiries");
            if (inqResponse.IsSuccessStatusCode)
                ViewBag.InquiriesJson = await inqResponse.Content.ReadAsStringAsync();

            // Get bookings
            var bookResponse = await _http.GetAsync("api/VisitBookings");
            if (bookResponse.IsSuccessStatusCode)
                ViewBag.BookingsJson = await bookResponse.Content.ReadAsStringAsync();
        }
        catch
        {
            ViewBag.PropertiesJson = null;
            ViewBag.InquiriesJson = null;
            ViewBag.BookingsJson = null;
        }

        return View();
    }
    public async Task<IActionResult> Favorites()
    {
        var token = HttpContext.Session.GetString("Token");
        if (token == null) return RedirectToAction("Login", "Auth");

        ViewBag.Token = token;

        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync("api/Favorites");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.FavoritesJson = content;
            }
        }
        catch
        {
            ViewBag.FavoritesJson = null;
        }

        return View();
    }
    public async Task<IActionResult> AdminDashboard()
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");

        if (token == null) return RedirectToAction("Login", "Auth");
        if (role != "Admin") return RedirectToAction("Index");

        ViewBag.Token = token;

        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var statsResponse = await _http.GetAsync("api/Admin/stats");
            if (statsResponse.IsSuccessStatusCode)
                ViewBag.StatsJson = await statsResponse.Content.ReadAsStringAsync();

            var usersResponse = await _http.GetAsync("api/Admin/users");
            if (usersResponse.IsSuccessStatusCode)
                ViewBag.UsersJson = await usersResponse.Content.ReadAsStringAsync();

            var agentsResponse = await _http.GetAsync("api/Agents");
            if (agentsResponse.IsSuccessStatusCode)
                ViewBag.AgentsJson = await agentsResponse.Content.ReadAsStringAsync();
        }
        catch
        {
            ViewBag.StatsJson = null;
            ViewBag.UsersJson = null;
            ViewBag.AgentsJson = null;
        }

        return View();
    }
    public async Task<IActionResult> MyInquiries()
    {
        var token = HttpContext.Session.GetString("Token");
        if (token == null) return RedirectToAction("Login", "Auth");

        ViewBag.Token = token;

        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync("api/Inquiries");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.InquiriesJson = content;
            }
        }
        catch
        {
            ViewBag.InquiriesJson = null;
        }

        return View();
    }
}