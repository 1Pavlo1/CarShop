using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CarShop.Extensions;

namespace CarShop.Models
{
    public class Car
    {
        public int Id { get; set; }

        public Brand Brand { get; set; }
        
        [RegularExpression("^[1-9]*$", ErrorMessage = "Select brand")]
        public int? BrandId { get; set; }

        public Model Model { get; set; }
        
        [RegularExpression("^[1-9]*$", ErrorMessage = "Select model")]
        public int? ModelId { get; set; }

        [Required(ErrorMessage = "Type year made")]
        [ValidTimeSpan(1930, ErrorMessage = "Invalid year")]
        public int Year { get; set; }
        
        [Required(ErrorMessage = "Type mileage")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid mileage")]
        public int Mileage { get; set; }
        
        [Required(ErrorMessage = "Type price")]
        public int Price { get; set; }
        
        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "Select currency")]        
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Type seller name")]
        public string SellerName { get; set; }
       
        [Required(ErrorMessage = "Type seller phone")]
        public string SellerPhone { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string SellerEmail { get; set; }
    }
}
