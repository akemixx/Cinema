using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CinemaA.Models
{
    public class User : IdentityUser
    {
        // Real name of a user (not unique), used for greeting when user is authenticated and in emails.
        public string RealName { get; set; }
        // Bonuses for authenticated users awarded for ticket purchases.
        public double Bonuses { get; set; }
        // 
        public List<Order> Orders { get; set; }
    }
}
