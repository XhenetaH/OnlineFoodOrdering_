using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models.ViewModels;

namespace OnlineFoodOrdering.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ShopController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToList(),
                Category = _db.Category.ToList()

            };
            return View(IndexVM);
        }

    }
}