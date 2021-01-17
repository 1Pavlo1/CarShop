using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem sli = new SelectListItem
            {
                Text = "-----Select-----",
                Value = "0"
            };
            list.Add(sli);

            foreach (var item in items)
            {
                sli = new SelectListItem
                {
                    Text = item.GetPropertyValue("Name"),
                    Value = item.GetPropertyValue("Id")
                };
                list.Add(sli);
            }
            return list;
        }
    }
}
