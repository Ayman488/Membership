using Membership.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using ClosedXML.Excel;

namespace Membership.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public AdminController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
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
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await _appDbContext.SaveChangesAsync();
            }
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
                user.IsActive = false; // إعادة الحالة لغير فعال
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
