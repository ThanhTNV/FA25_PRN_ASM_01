using ASM_01.BusinessLayer.Services;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM_01.WebApp.Controllers;

public class AuthController(SimpleAuthService authService) : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
    {
        try
        {
            var dto = authService.Login(loginViewModel.Username, loginViewModel.Password);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString()),
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(ClaimTypes.Role, dto.Role)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // This writes the encrypted cookie to the response
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true // keep after browser closes (optional)
                                        // ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) // optional override
                });

            return RedirectToAction("Index", "Home");

        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }
    }

    public IActionResult Denied()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
