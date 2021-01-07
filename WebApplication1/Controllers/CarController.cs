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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CarShop.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Executive)]
    public class CarController : Controller
    {
        private CarShopContext _dataBase;
        private IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public CarViewModel CarVM { get; set; }
        public CarController(CarShopContext dataBase, IWebHostEnvironment webHostEnvironment)
        {
            _dataBase = dataBase;
            _webHostEnvironment = webHostEnvironment;
            CarVM = new CarViewModel()
            {
                Brands = _dataBase.Brands.ToList(),
                Models = _dataBase.Models.ToList(),
                Car = new Models.Car()
            };
        }
        public IActionResult Index()
        {
            var cars = _dataBase.Cars.Include(b => b.Brand).Include(m=>m.Model);
            return View(cars.ToList());
        }
        public IActionResult Create()
        {
            return View(CarVM);
        }
        [HttpPost, ActionName("Create")]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                CarVM.Brands = _dataBase.Brands.ToList();
                CarVM.Models = _dataBase.Models.ToList();
                return View(CarVM);
            }

            _dataBase.Add(CarVM.Car);
            _dataBase.SaveChanges();

            var carId = CarVM.Car.Id;
            var imgRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var savedCar = _dataBase.Cars.Find(carId);

            if(files.Count!=0)
            {
                var imagePath = @"images\car\";
                var extension = Path.GetExtension(files[0].FileName);
                var relativeImagePath = imagePath + carId + extension;
                var absImagePath = Path.Combine(imgRootPath, relativeImagePath);

                using(var fileStream = new FileStream(absImagePath, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                savedCar.ImagePath = relativeImagePath;
                _dataBase.SaveChanges();
            }
            else
            {
                savedCar.ImagePath = @"images\car\notfound.jpg";
                _dataBase.SaveChanges();
            }
           
            return RedirectToAction("Index");
        }
        //public IActionResult Edit(int id)
        //{
        //    ModelVM.Model = _dataBase.Models.Include(b => b.Brand).SingleOrDefault(b => b.Id == id);

        //    if (ModelVM.Model == null)
        //        return NotFound();

        //    return View(ModelVM);
        //}
        //[HttpPost, ActionName("Edit")]
        //public IActionResult EditPost()
        //{
        //    if (!ModelState.IsValid)
        //        return View(ModelVM);

        //    _dataBase.Update(ModelVM.Model);
        //    _dataBase.SaveChanges();

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var car = _dataBase.Cars.Find(id);

            if (car == null)
                return NotFound();

            _dataBase.Cars.Remove(car);
            _dataBase.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}