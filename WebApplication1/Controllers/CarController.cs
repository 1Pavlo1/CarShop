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
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Http;

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
        [AllowAnonymous]
        public IActionResult Index(string searchString, string sortOrder, int pageNumber = 1, int pageSize = 5)
        {
            ViewBag.CurrenrSortOrder = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.PriceSortParam = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            int unsizedValues = (pageNumber * pageSize) - pageSize;
           
            var cars = from c in _dataBase.Cars.Include(b => b.Brand).Include(m => m.Model)
                       select c;
            
            var carCount = cars.Count();

            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Brand.Name.Contains(searchString));
                carCount = cars.Count();
            }                

            switch(sortOrder)
            {
                case "price_desc":
                    cars = cars.OrderByDescending(c => c.Price);
                    break;

                default:
                    cars = cars.OrderBy(c => c.Price);
                    break;
            }
            cars=cars
                .Skip(unsizedValues)
                .Take(pageSize);

            var pagedCarList = new PagedResult<Car>
            {
                Data = cars.AsNoTracking().ToList(),
                TotalItems = carCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(pagedCarList);
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

            UploadImage();

            _dataBase.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult Edit(int id)
        {
            CarVM.Car = _dataBase.Cars.SingleOrDefault(c => c.Id == id);            

            CarVM.Models = _dataBase.Models.Where(m => m.BrandId == CarVM.Car.BrandId);

            if (CarVM.Car == null)
                return NotFound();

            return View(CarVM);
        }
        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost()        
        {

            if (!ModelState.IsValid)
            {
                CarVM.Brands = _dataBase.Brands.ToList();
                CarVM.Models = _dataBase.Models.ToList();
                return View(CarVM);
            }            
            _dataBase.Update(CarVM.Car);

            UploadImage();

            _dataBase.SaveChanges();
            return RedirectToAction("Index");
        }
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
        [AllowAnonymous]
        public IActionResult ViewDetails(int id)
        {
            CarVM.Car = _dataBase.Cars.SingleOrDefault(c => c.Id == id);

            if (CarVM.Car == null)
                return NotFound();

            return View(CarVM);
        }

        private void UploadImage()
        {
            var carId = CarVM.Car.Id;
            var imgRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var savedCar = _dataBase.Cars.Find(carId);

            if (files.Count != 0)
            {
                var imagePath = @"images\car\";
                var extension = Path.GetExtension(files[0].FileName);
                var relativeImagePath = imagePath + carId + extension;
                var absImagePath = Path.Combine(imgRootPath, relativeImagePath);

                using (var fileStream = new FileStream(absImagePath, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                savedCar.ImagePath = relativeImagePath;
            }
        }
    }
}