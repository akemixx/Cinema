using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaA.Models
{
    public class User : IdentityUser
    {
        public string NameOfUser { get; set; }
        public double Bonuses { get; set; }
    }
}
