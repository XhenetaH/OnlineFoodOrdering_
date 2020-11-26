using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;

namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET 
        public async Task<IActionResult> Index()
        {

            return View(await _db.Category.ToListAsync());
        }

        //GET - Create
        public IActionResult Create()
        {
            return View();
        }
        
        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}