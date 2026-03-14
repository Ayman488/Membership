using Microsoft.AspNetCore.Mvc;
using Membership.Data;
using Membership.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;

namespace Membership.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _context;

        public SuperAdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var admins = _context.Admins.ToList();
            var allUsers = _context.Users.ToList();

            // إنشاء كائن من الـ ViewModel وتعبئته بالبيانات
            var viewModel = new SuperAdminIndexViewModel
            {
                Admins = admins,
                ActiveUsers = allUsers.Where(u => u.IsActive).ToList(),
                PendingUsers = allUsers.Where(u => !u.IsActive).ToList()
            };

            return View(viewModel); // إرسال الموديل للـ View
        }

        [HttpPost]
        public IActionResult PromoteUserToAdmin(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            var existingAdmin = _context.Admins.FirstOrDefault(a => a.Email == user.Email);
            if (existingAdmin != null)
            {
                TempData["ErrorMessage"] = "هذا المستخدم هو مشرف بالفعل!";
                return RedirectToAction("Index");
            }

            user.IsActive = true;
            _context.Users.Update(user);

            var newAdmin = new Admin
            {
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                PasswordHash = "123456",
                Role = "Admin"
            };

            _context.Admins.Add(newAdmin);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"تمت ترقية {user.FirstName} لمشرف بنجاح.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ActivateUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            user.IsActive = true;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "تم تفعيل الحساب بنجاح.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create() => User.IsInRole("SuperAdmin") ? View() : Forbid();

        [HttpPost]
        public IActionResult Create(string name, string email, string password)
        {
            var admin = new Admin { Name = name, Email = email, PasswordHash = password, Role = "Admin" };
            _context.Admins.Add(admin);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PromoteToSuperAdmin(int id)
        {
            var newSuper = _context.Admins.Find(id);
            var currentSuper = _context.Admins.FirstOrDefault(a => a.Email == User.Identity.Name);
            if (newSuper != null && currentSuper != null)
            {
                currentSuper.Role = "Admin";
                newSuper.Role = "SuperAdmin";
                _context.SaveChanges();
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpPost]
        public IActionResult DeleteAdmin(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            var admin = _context.Admins.Find(id);
            if (admin == null || admin.Email == User.Identity.Name) return Forbid();
            _context.Admins.Remove(admin);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            return user == null ? NotFound() : View(user);
            // هنا نستخدم موديل User العادي مباشرة في الـ EditUser View
        }

        [HttpPost]
        public IActionResult EditUser(int id, string college, string yearOfStudy)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            user.College = college;
            user.YearOfStudy = yearOfStudy;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}