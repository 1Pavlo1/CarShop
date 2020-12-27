using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;

namespace CarShop.Controllers
{
    public class BrandController : Controller
    {
        private CarShopContext _dataBase;

        public BrandController(CarShopContext dataBase)
        {
            _dataBase = dataBase;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if(ModelState.IsValid)
            {
                _dataBase.Add(brand);
                _dataBase.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }
        public IActionResult Index()
        {
            return View(_dataBase.Brands.ToList());
        }
    }
}