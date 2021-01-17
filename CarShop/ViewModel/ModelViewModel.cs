using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Models.ViewModel
{
    public class ModelViewModel
    {
        public Model Model { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<SelectListItem> CSelectListItem (IEnumerable<Brand> items)
        {
            List<SelectListItem> brandList = new List<SelectListItem>();
            SelectListItem sli = new SelectListItem
            {
                Text = "-----Select-----",
                Value = "0"
            };
            brandList.Add(sli);
            
            foreach(var item in items)
            {
                sli = new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                };
                brandList.Add(sli);
            }
            return brandList;
        }
    }
}
