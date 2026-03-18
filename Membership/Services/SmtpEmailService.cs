using Membership.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Membership.Services
{
    public class SmtpEmailService : IEmailService // تم التعديل
    {
        private readonly EmailSettings _emailSettings; // تم التعديل
        private readonly ILogger<SmtpEmailService> _logger; // تم التعديل

        public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger) // تم التعديل
        {
            _emailSettings = emailSettings.Value; // تم التعديل
            _logger = logger; // تم التعديل
        }

        public async Task<bool> SendActivationEmailAsync(string toEmail, string studentName) // تم التعديل
        {
            var subject = "تم تفعيل حسابك بنجاح"; // تم التعديل
            var body = $@"<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8' />
</head>
<body style='margin:0;padding:20px;background-color:#f4f6fb;font-family:Tahoma,Arial,sans-serif;'>
    <div style='max-width:650px;margin:0 auto;background:#ffffff;border-radius:14px;overflow:hidden;border:1px solid #e5e7eb;'>
        <div style='background:#1f4c8b;padding:20px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:30px;font-weight:800;'>✅ تم التفعيل والقبول</h1>
        </div>
        <div style='padding:28px 24px;'>
            <p style='margin:0 0 12px;color:#111827;font-size:24px;font-weight:700;'>مرحباً {studentName}</p>
            <p style='margin:0 0 16px;color:#374151;font-size:20px;line-height:1.9;'>
                نود إبلاغك بأنه <strong style='color:#065f46;'>تم تفعيل عضويتك بنجاح</strong> في اتحاد الطلبة السوريين في سكاريا.
            </p>
            <div style='margin:18px 0;padding:14px;background:#ecfeff;border:1px solid #a5f3fc;border-radius:10px;color:#0c4a6e;font-size:18px;font-weight:700;'>
                أهلاً بك بيننا 💙
            </div>
            <p style='margin:0;color:#4b5563;font-size:16px;'>مع التحية، إدارة اتحاد الطلبة السوريين في سكاريا.</p>
        </div>
    </div>
</body>
</html>"; // تم التعديل

            return await SendEmailCoreAsync(toEmail, subject, body, true); // تم التعديل نسخة 2
        }

        public async Task<bool> SendCustomEmailAsync(string toEmail, string studentName, string customMessage) // تم التعديل نسخة 2
        {
            var safeName = WebUtility.HtmlEncode(studentName); // تم التعديل نسخة 2
            var safeMessage = WebUtility.HtmlEncode(customMessage).Replace("\n", "<br/>"); // تم التعديل نسخة 2
            var subject = "رسالة من اتحاد الطلبة السوريين في سكاريا"; // تم التعديل نسخة 2
            var body = $@"<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8' />
</head>
<body style='margin:0;padding:20px;background-color:#f4f6fb;font-family:Tahoma,Arial,sans-serif;'>
    <div style='max-width:650px;margin:0 auto;background:#ffffff;border-radius:14px;overflow:hidden;border:1px solid #e5e7eb;'>
        <div style='background:#1f4c8b;padding:20px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:28px;font-weight:800;'>📩 رسالة جديدة</h1>
        </div>
        <div style='padding:28px 24px;'>
            <p style='margin:0 0 12px;color:#111827;font-size:22px;font-weight:700;'>مرحباً {safeName}</p>
            <div style='margin:0 0 16px;color:#374151;font-size:18px;line-height:1.9;background:#f9fafb;padding:16px;border-radius:10px;border:1px solid #e5e7eb;'>
                {safeMessage}
            </div>
            <p style='margin:0;color:#4b5563;font-size:16px;'>مع التحية، إدارة اتحاد الطلبة السوريين في سكاريا.</p>
        </div>
    </div>
</body>
</html>"; // تم التعديل نسخة 2

            return await SendEmailCoreAsync(toEmail, subject, body, true); // تم التعديل نسخة 2
        }

        private async Task<bool> SendEmailCoreAsync(string toEmail, string subject, string body, bool isBodyHtml) // تم التعديل نسخة 2
        {
            try // تم التعديل نسخة 2
            {
                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port) // تم التعديل نسخة 2
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password), // تم التعديل نسخة 2
                    EnableSsl = _emailSettings.EnableSsl // تم التعديل نسخة 2
                };

                using var message = new MailMessage // تم التعديل نسخة 2
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName), // تم التعديل نسخة 2
                    Subject = subject, // تم التعديل نسخة 2
                    Body = body, // تم التعديل نسخة 2
                    IsBodyHtml = isBodyHtml // تم التعديل نسخة 2
                };

                message.To.Add(toEmail); // تم التعديل نسخة 2
                await smtpClient.SendMailAsync(message); // تم التعديل نسخة 2
                return true; // تم التعديل نسخة 2
            }
            catch (Exception ex) // تم التعديل نسخة 2
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail); // تم التعديل نسخة 2
                return false; // تم التعديل نسخة 2
            }
        }
    }
}