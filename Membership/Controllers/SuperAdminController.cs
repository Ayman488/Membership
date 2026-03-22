using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Membership.Data;
using Membership.Models;
using Membership.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Membership.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // تم التعديل


        public SuperAdminController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            user.IsActive = true;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "تم تفعيل الحساب بنجاح.";
            if (!string.IsNullOrWhiteSpace(user.Email)) // تم التعديل
            {
                var studentName = $"{user.FirstName} {user.LastName}".Trim(); // تم التعديل
                await _emailService.SendActivationEmailAsync(user.Email, studentName); // تم التعديل
            }
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
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int UserId, string FirstName, string LastName, string Email,
                               string StudentNumber, string PhoneNumber, string University,
                               string College, string YearOfStudy)
        {
            // 1. جلب الكائن الأصلي من قاعدة البيانات
            var user = _context.Users.Find(UserId);

            if (user == null)
            {
                return NotFound();
            }

            // 2. التحديث الذكي: (نحدث فقط إذا كانت القيمة الجديدة ليست فارغة)

            if (!string.IsNullOrWhiteSpace(FirstName))
                user.FirstName = FirstName;

            if (!string.IsNullOrWhiteSpace(LastName))
                user.LastName = LastName;

            if (!string.IsNullOrWhiteSpace(Email))
                user.Email = Email;

            if (!string.IsNullOrWhiteSpace(StudentNumber))
                user.StudentNumber = StudentNumber;

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
                user.PhoneNumber = PhoneNumber;

            if (!string.IsNullOrWhiteSpace(University))
                user.University = University;

            if (!string.IsNullOrWhiteSpace(College))
                user.College = College;

            if (!string.IsNullOrWhiteSpace(YearOfStudy))
                user.YearOfStudy = YearOfStudy;

            // 3. حفظ التغييرات فقط للحقول التي تم تعديلها
            _context.SaveChanges();

            // يمكنك إضافة رسالة نجاح هنا باستخدام TempData لتعلم الطالب بنجاح العملية
            TempData["SuccessMessage"] = "تم تحديث البيانات بنجاح!";

            return RedirectToAction("Index");
        }


        //تمت اضافته 
        [HttpGet]
        public IActionResult UploadStudents()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadStudents(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "يرجى اختيار ملف إكسل صحيح.";
                return RedirectToAction("Index");
            }

            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // قراءة أول صفحة
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تجاوز صف العناوين

                        foreach (var row in rows)
                        {
                            // 1. قراءة البريد الإلكتروني أولاً للتحقق منه
                            string email = row.Cell(4).GetValue<string>()?.Trim();

                            if (string.IsNullOrEmpty(email)) continue; // تخطى الأسطر الفارغة

                            // 2. التحقق هل هذا الإيميل موجود مسبقاً في قاعدة البيانات؟
                            bool isDuplicate = _context.Users.Any(u => u.Email == email);

                            if (isDuplicate)
                            {
                                // إذا كان موجوداً، نتخطى هذا السطر وننتقل للطالب التالي
                                continue;
                            }

                            // 3. إذا لم يكن مكرراً، نقوم بإنشاء الكائن وإضافته
                            var user = new User
                            {
                                FirstName = row.Cell(1).GetValue<string>(),
                                LastName = row.Cell(2).GetValue<string>(),
                                gender = row.Cell(3).GetValue<string>(),
                                Email = email,
                                StudentNumber = row.Cell(5).GetValue<string>(),
                                PhoneNumber = row.Cell(6).GetValue<string>(),
                                University = row.Cell(7).GetValue<string>(),
                                College = row.Cell(8).GetValue<string>(),
                                IsActive = true,
                                YearOfStudy = "1",
                                Status = Membership.Models.User.MemberStatus.Undergraduate
                            };

                            _context.Users.Add(user);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["SuccessMessage"] = "تم رفع بيانات الطلاب بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء الرفع: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GraduateMember(int Id)
        {
            var member = await _context.Users.FindAsync(Id);

            if (member == null)
                return NotFound();

            if (member.Status != Models.User.MemberStatus.Graduated)
            {
                member.Status = Models.User.MemberStatus.Graduated;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UndergraduateMember(int Id)
        {
            var member = await _context.Users.FindAsync(Id);

            if (member == null)
                return NotFound();

            if (member.Status != Models.User.MemberStatus.Undergraduate)
            {
                member.Status = Models.User.MemberStatus.Undergraduate;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false; // إعادة الحالة لغير فعال
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GraduatedStudents()
        {
            // جلب جميع الطلاب الذين تم تحويل حالتهم إلى "خريج"
            var graduatedUsers = _context.Users
                .Where(u => u.Status == Membership.Models.User.MemberStatus.Graduated)
                .ToList();

            return View(graduatedUsers);
        }

        [HttpPost] // تم التعديل نسخة 2
        [ValidateAntiForgeryToken] // تم التعديل نسخة 2
        public async Task<IActionResult> SendCustomEmail(int id, string customMessage) // تم التعديل نسخة 2
        {
            var user = await _context.Users.FindAsync(id); // تم التعديل نسخة 2
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(customMessage)) // تم التعديل نسخة 2
            {
                return RedirectToAction(nameof(Members)); // تم التعديل نسخة 2
            }

            var studentName = $"{user.FirstName} {user.LastName}".Trim(); // تم التعديل نسخة 2
            await _emailService.SendCustomEmailAsync(user.Email, studentName, customMessage); // تم التعديل نسخة 2
            return RedirectToAction(nameof(Members)); // تم التعديل نسخة 2
        }


    }
}