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
        // 1. ��� ���� �������
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. ������ ������ �������
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {
                // ������ ��� ������ ��� ����
                if (photoFile != null && photoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/students");

                    // ������ �� ���� ������
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(fileStream);
                    }

                    // ��� ��� ����� �� ����� ��������
                    user.StudentCartPhoto = uniqueFileName;
                }

                _appDbContext.Add(user);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // ������ ����� ������ �� ��������
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
