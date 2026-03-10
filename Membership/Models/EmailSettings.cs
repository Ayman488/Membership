namespace Membership.Models
{
    public class EmailSettings // تم التعديل
    {
        public string Host { get; set; } = string.Empty; // تم التعديل
        public int Port { get; set; } // تم التعديل
        public bool EnableSsl { get; set; } // تم التعديل
        public string Username { get; set; } = string.Empty; // تم التعديل
        public string Password { get; set; } = string.Empty; // تم التعديل
        public string FromEmail { get; set; } = string.Empty; // تم التعديل
        public string FromName { get; set; } = string.Empty; // تم التعديل
    }
}