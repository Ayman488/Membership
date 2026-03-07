using System.ComponentModel.DataAnnotations;

namespace Membership.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        // كلمة السر (مشفّرة)
        public string PasswordHash { get; set; }

        // الدور (لا يأتي من الواجهة)
        public string Role { get; set; }
    }
}
