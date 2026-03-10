namespace Membership.Services
{
    public interface IEmailService // تم التعديل
    {
        Task<bool> SendActivationEmailAsync(string toEmail, string studentName); // تم التعديل
        Task<bool> SendCustomEmailAsync(string toEmail, string studentName, string customMessage); // تم التعديل نسخة 2
    }
}