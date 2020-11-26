using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;

namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryController(ApplicationDbContext db)
        {
            this._db = db;
        }
        //GET - Index
        public async  Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.ToListAsync());
        }
    }
}