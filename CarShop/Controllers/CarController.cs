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
        private readonly CarShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public CarViewModel CarVM { get; set; }
        public CarController(CarShopContext dataBase, IWebHostEnvironment webHostEnvironment)
        {
            _context = dataBase;
            _webHostEnvironment = webHostEnvironment;
            CarVM = new CarViewModel()
            {
                Brands = _context.Brands.ToList(),
                Models = _context.Models.ToList(),
                Car = new Models.Car()
            };
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string sortOrder, int pageNumber = 1, int pageSize = 5)
        {
            ViewBag.CurrenrSortOrder = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.PriceSortParam = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            int unsizedValues = (pageNumber * pageSize) - pageSize;
           
            var cars = from c in _context.Cars.Include(b => b.Brand).Include(m => m.Model)
                       select c;
            
            var carCount = await cars.CountAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Brand.Name.Contains(searchString));
                carCount = await cars.CountAsync();
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
                Data = await cars.AsNoTracking().ToListAsync(),
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
        public async Task<IActionResult> CreatePost()
        {        
            if (!ModelState.IsValid)
            {
                CarVM.Brands = await _context.Brands.ToListAsync();
                CarVM.Models = await _context.Models.ToListAsync();
                return View(CarVM);
            }

            await _context.AddAsync(CarVM.Car);

            UploadImage();

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            CarVM.Car = await _context.Cars.SingleOrDefaultAsync(c => c.Id == id);            

            CarVM.Models = _context.Models.Where(m => m.BrandId == CarVM.Car.BrandId);

            if (CarVM.Car == null)
                return NotFound();

            return View(CarVM);
        }
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost()        
        {
            if (!ModelState.IsValid)
            {
                CarVM.Brands = await _context.Brands.ToListAsync();
                CarVM.Models = await _context.Models.ToListAsync();
                return View(CarVM);
            }            
            _context.Update(CarVM.Car);

            UploadImage();

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ViewDetails(int id)
        {
            CarVM.Car = await _context.Cars.SingleOrDefaultAsync(c => c.Id == id);

            if (CarVM.Car == null)
                return NotFound();

            return View(CarVM);
        }

        private async void UploadImage()
        {
            var carId = CarVM.Car.Id;
            var imgRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var savedCar = await _context.Cars.FindAsync(carId);

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