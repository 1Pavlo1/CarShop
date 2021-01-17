using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Models.ViewModel
{
    public class CarViewModel
    {
        public Car Car { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Model> Models { get; set; }
        public IEnumerable<Currency> Currencies { get; set; }

        private List<Currency> CurrencyList = new List<Currency>();

        private List<Currency> CreateList()
        {
            CurrencyList.Add(new Currency("USD", "USD"));
            CurrencyList.Add(new Currency("UAH", "UAH"));
            CurrencyList.Add(new Currency("EUR", "EUR"));
            return CurrencyList;
        }
        public CarViewModel()
        {
            Currencies = CreateList();
        }
    }

    public class Currency
    {
        public String Id { get; set; }
        public String Name { get; set; }

        public Currency (String id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
