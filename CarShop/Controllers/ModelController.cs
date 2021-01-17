using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CarShop.Controllers.Resources;
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
        private readonly CarShopContext _context;
        private readonly IMapper _mapper;

        [BindProperty]
        public ModelViewModel ModelVM { get; set; }
        public ModelController(CarShopContext dataBase, IMapper mapper)
        {
            _context = dataBase;
            _mapper = mapper;

            ModelVM = new ModelViewModel()
            {
                Brands = _context.Brands.ToList(),
                Model = new Models.Model()
            };
        }
        public IActionResult Index()
        {
            var model = _context.Models.Include(b=>b.Brand);
            return View(model);
        }
        public IActionResult Create()
        {
            return View(ModelVM);
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if(!ModelState.IsValid)
                return View(ModelVM);

            await _context.AddAsync(ModelVM.Model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            ModelVM.Model = await _context.Models.Include(b => b.Brand).SingleOrDefaultAsync(b => b.Id == id);

            if (ModelVM.Model == null)
                return NotFound();

            return View(ModelVM);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (!ModelState.IsValid)
                return View(ModelVM);

            _context.Update(ModelVM.Model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Models.FindAsync(id);

            if (model == null)
                return NotFound();

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet("api/models/{brandId}")]
        public async Task<IEnumerable<ModelResources>> Models(int brandId)
        {
            var models = await _context.Models.ToListAsync();
            var modelResources = _mapper.Map<List<Model>, List<ModelResources>>(models)
                                 .Where(m => m.BrandId == brandId);

            return modelResources;
        }
    }
}