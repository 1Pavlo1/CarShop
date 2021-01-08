using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarShop.Helpers;
using CarShop.Models;
using CarShop.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Executive)]
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
        public IActionResult Edit(int id)
        {
            ModelVM.Model = _dataBase.Models.Include(b => b.Brand).SingleOrDefault(b => b.Id == id);

            if (ModelVM.Model == null)
                return NotFound();

            return View(ModelVM);
        }
        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
                return View(ModelVM);

            _dataBase.Update(ModelVM.Model);
            _dataBase.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var model = _dataBase.Models.Find(id);

            if (model == null)
                return NotFound();

            _dataBase.Models.Remove(model);
            _dataBase.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        [HttpGet("api/models/{brandId}")]
        public IEnumerable<Model> Models(int brandId)
        {
            return _dataBase.Models.ToList()
                   .Where(m => m.BrandId == brandId);
        }
    }
}