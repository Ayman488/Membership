using System.Collections.Generic;

namespace Membership.Models
{
 
    public class SuperAdminIndexViewModel
    {
        public List<Admin> Admins { get; set; }
        public List<User> ActiveUsers { get; set; }
        public List<User> PendingUsers { get; set; }
    }
}