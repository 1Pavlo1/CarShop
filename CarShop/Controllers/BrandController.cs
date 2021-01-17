using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using Microsoft.AspNetCore.Authorization;
using CarShop.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Executive)]
    public class BrandController : Controller
    {
        private readonly CarShopContext _context;

        public BrandController(CarShopContext dataBase)
        {
            _context = dataBase;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if(ModelState.IsValid)
            {
                await _context.AddAsync(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                return NotFound();
            
            _context.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                return NotFound();

            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Update(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(brand);
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }       
    }
}