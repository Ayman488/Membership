using Membership.Data;
using Membership.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Membership.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(AppDbContext appDbContext, ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {

                string email = user.Email.ToLower().Trim();
                if (!(email.EndsWith("@ogr.sakarya.edu.tr") || email.EndsWith("@subu.edu.tr")))
                {
                    ModelState.AddModelError("Email", "عذراً، يجب استخدام بريد إلكتروني ينتهي بـ @ogr.sakarya.edu.tr أو @subu.edu.tr فقط.");
                    return View(user);
                }


                // 1. التحقق من وجود طالب بنفس البيانات (تطابق كامل)
                // قم بتعديل الحقول أدناه بناءً على ما تعتبره "تطابقاً كاملاً" في نموذجك
                bool isDuplicate = await _appDbContext.Users.AnyAsync(u =>
                    u.FirstName == user.FirstName &&
                    u.LastName == user.LastName &&
                    u.Email == user.Email &&
                    u.PhoneNumber == user.PhoneNumber &&
                    u.StudentNumber == user.StudentNumber &&
                    u.College == user.College ); 

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "أنت منتسب بالفعل! 😊 إن كان هناك مشكلة أو تريد تحديث البيانات يرجى التواصل مع الاتحاد، ستجد روابط مواقع التواصل الاجتماعي في الصفحة الرئيسية. لتحديث الصفحة انقر على\"انضم إلينا\" ، نتمنى لك يوماً سعيداً! ✨"); return View(user);
                }

                // 2. معالجة الصورة (كودك الأصلي)
                if (photoFile != null && photoFile.Length > 0)
                {
                    // التحقق من الحجم (5 ميجا بايت كحد أقصى)
                    long maxFileSize = 5 * 1024 * 1024;
                    if (photoFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("photoFile", "عذراً، حجم الملف يجب ألا يتجاوز 5 ميغابايت.");
                        return View(user);
                    }
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(photoFile.FileName).ToLower();

                    if (!supportedTypes.Contains(extension))
                    {
                        ModelState.AddModelError("", "يرجى رفع صورة فقط (JPG, PNG).");
                        return View(user);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + extension;

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/students");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(fileStream);
                    }

                    user.StudentCartPhoto = uniqueFileName;
                }

                // 3. حفظ البيانات
                _appDbContext.Add(user);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم تسجيل طلبك بنجاح، سيتم مراجعته من قبل الإدارة في أقرب وقت.";

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}