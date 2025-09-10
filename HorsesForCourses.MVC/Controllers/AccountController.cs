using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Service;
using System.Text.Json;

namespace HorsesForCourses.MVC;

public class AccountController : Controller
{
    private readonly IAccountService _service;
    private readonly ICoachService _coachService;

    public AccountController(IAccountService service, ICoachService Cservice)
    {
        _service = service;
        _coachService = Cservice;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        return View(model: returnUrl);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var hasher = new Pbkdf2PasswordHasher();

        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        if (!hasher.Verify(password, user.PasswordHash))
        {
            return BadRequest("Invalid Password");
        }

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.Role, user.Role) };
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
    public async Task<IActionResult> Register(RegisterAccountViewModel account, string choice)
    {
        var user = AppUser.From(account.Name, account.Email, account.Password, account.PassConfirm, choice);
        if (choice == "coach") { await _coachService.CreateCoach(new Coach(account.Name, account.Email)); }
        await _service.CreateUser(user);
        return await Login(user.Email.Value, account.Password);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string email)
    {
        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        await _service.Deleteuser(user);
        await Logout();
        return Redirect("../Home");
    }

    [HttpPost]
    public async Task<IActionResult> DownloadUserData(string email)
    {
        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        var json = JsonSerializer.Serialize(user);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", "userdata.json");
    }
}