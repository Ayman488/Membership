using System.ComponentModel.DataAnnotations;

namespace Membership.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string gender { get; set; }
        public string Email { get; set; }
        public string StudentNumber {  get; set; }
        public string PhoneNumber { get; set; }
        public string University { get; set; }
        public string College { get; set; } // كلية
        public string YearOfStudy { get; set; }
        public string? StudentCartPhoto { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
