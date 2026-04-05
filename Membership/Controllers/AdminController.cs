using ClosedXML.Excel;
using Membership.Data;
using Membership.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Membership.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService; 

        public AdminController(AppDbContext appDbContext, IEmailService emailService) 
        {
            _appDbContext = appDbContext;
            _emailService = emailService; 

        }
        public IActionResult AdminPage()
        {
            return View();
        }
        public async Task<IActionResult> Members()
        {
            var members = await _appDbContext.Users.ToListAsync();
            return View(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GraduateMember(int Id)
        {
            var member = await _appDbContext.Users.FindAsync(Id);
            
            if (member == null)
                return NotFound();

            if (member.Status != Models.User.MemberStatus.Graduated)
            {
                member.Status = Models.User.MemberStatus.Graduated;
                await _appDbContext.SaveChangesAsync();    
            }

            return RedirectToAction(nameof(Members));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndergraduateMember(int Id)
        {
            var member = await _appDbContext.Users.FindAsync(Id);
            
            if (member == null)
                return NotFound();

            if (member.Status != Models.User.MemberStatus.Undergraduate)
            {
                member.Status = Models.User.MemberStatus.Undergraduate;
                await _appDbContext.SaveChangesAsync();    
            }
            
            return RedirectToAction(nameof(Members));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await _appDbContext.SaveChangesAsync();
                if (!string.IsNullOrWhiteSpace(user.Email)) 
                {
                    var studentName = $"{user.FirstName} {user.LastName}".Trim(); 
                    await _emailService.SendActivationEmailAsync(user.Email, studentName);
                }
            }
            return RedirectToAction(nameof(Members));
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> SendCustomEmail(int id, string customMessage) 
        {
            var user = await _appDbContext.Users.FindAsync(id); 
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(customMessage)) 
            {
                return RedirectToAction(nameof(Members)); 
            }

            var studentName = $"{user.FirstName} {user.LastName}".Trim(); 
            await _emailService.SendCustomEmailAsync(user.Email, studentName, customMessage); 
            return RedirectToAction(nameof(Members)); 
        }



        // أكشن تعليق العضوية
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false; 
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Members));
        }


        public IActionResult PrintFile()
        {
            return View();
        }



        [HttpPost]
        public IActionResult ExportToExcel(string[] selectedFields)
        {
            // 1. جلب البيانات من قاعدة البيانات (مثال)
            var students = _appDbContext.Users
                .Where(s => s.IsActive == true)
                .ToList();

            // 2. إنشاء ملف Excel جديد
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("المنتسبين");
                var currentRow = 1;

                // 3. إنشاء العناوين (Headers) بناءً على ما اختاره الأدمن
                int column = 1;
                foreach (var field in selectedFields)
                {
                    worksheet.Cell(currentRow, column).Value = GetArabicName(field);
                    worksheet.Cell(currentRow, column).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, column).Style.Fill.BackgroundColor = XLColor.LightGray;
                    column++;
                }

                // 4. تعبئة البيانات
                foreach (var student in students)
                {
                    currentRow++;
                    column = 1;
                    foreach (var field in selectedFields)
                    {
                        // جلب قيمة الحقل من الكائن "student" برمجياً
                        var propertyValue = student.GetType().GetProperty(field)?.GetValue(student, null);
                        worksheet.Cell(currentRow, column).Value = propertyValue?.ToString();
                        column++;
                    }
                }

                // تنسيق تلقائي للأعمدة
                worksheet.Columns().AdjustToContents();

                // 5. تحويل الملف إلى Stream وإرساله للمتصفح
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students_Report.xlsx");
                }
            }
        }

        // دالة مساعدة لتحويل أسماء الحقول للعربية في ملف الإكسل
        private string GetArabicName(string fieldName)
        {
            return fieldName switch
            {
                "FirstName" => "الاسم الأول",
                "LastName" => "الاسم الأخير",
                "gender" => "الجنس",
                "Email" => "البريد الإلكتروني",
                "StudentNumber" => "الرقم الجامعي",
                "PhoneNumber" => "رقم الهاتف",
                "University" => "الجامعة",
                "College" => "الكلية",
                "YearOfStudy" => "السنة الدراسية",
                _ => fieldName
            };
        }
    }
}
