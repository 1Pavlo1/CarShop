using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarShop.Models;
using CarShop.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Controllers
{
    public class ModelController : Controller
    {
        private CarShopContext _dataBase;
        [BindProperty]
        public ModelViewModel ModelVM { get; set; }
        public ModelController(CarShopContext dataBase)
        {
            _dataBase = dataBase;
            ModelVM = new ModelViewModel()
            {
                Brands = _dataBase.Brands.ToList(),
                Model = new Models.Model()
            };
        }
        public IActionResult Index()
        {
            var model = _dataBase.Models.Include(b=>b.Brand);
            return View(model);
        }
        public IActionResult Create()
        {
            return View(ModelVM);
        }
        [HttpPost, ActionName("Create")]
        public IActionResult CreatePost()
        {
            if(!ModelState.IsValid)
                return View(ModelVM);

            _dataBase.Add(ModelVM.Model);
            _dataBase.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}