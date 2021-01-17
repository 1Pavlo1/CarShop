using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Extensions
{
    public class ValidTimeSpan : RangeAttribute
    {
        public ValidTimeSpan(int startYear):base(startYear, DateTime.Today.Year)
        {

        }
    }
}
