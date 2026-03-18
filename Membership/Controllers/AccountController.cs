using Microsoft.AspNetCore.Mvc;
using Membership.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Membership.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context) { _context = context; }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.PasswordHash == password);
            if (admin != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, admin.Email), new Claim(ClaimTypes.Role, admin.Role) };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));
                if (admin.Role == "SuperAdmin")
                {
                    return RedirectToAction("Index", "SuperAdmin");
                }
                else
                {
                    return RedirectToAction("AdminPage", "Admin"); // تأكد من وجود Controller بهذا الاسم
                }
            }
            ViewBag.Error = "بيانات الدخول خاطئة";
            return View();
        }

        public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync("CookieAuth"); return RedirectToAction("Login"); }
    }
}