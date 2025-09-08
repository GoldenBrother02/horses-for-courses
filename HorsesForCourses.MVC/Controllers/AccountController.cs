


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Service;

namespace HorsesForCourses.MVC;

public class AccountController : Controller
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
        var id = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(id));
        return Redirect("../Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("../Home");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterAccountViewModel account)
    {
        var user = AppUser.From(account.Name, account.Email, account.Pass, account.PassConfirm);
        await _service.CreateUser(user);
        await Login(user.Email.ToString());
        return Redirect("../Home");
    }
}