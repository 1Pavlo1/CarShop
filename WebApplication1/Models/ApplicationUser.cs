using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Office Number")]
        public string TelNumber { get; set; }
       
        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}
