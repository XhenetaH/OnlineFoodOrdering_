using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
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
        //public IActionResult Index()
        //{
        //    IndexViewModel IndexVM = new IndexViewModel()
        //    {
        //        MenuItem = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToList(),
        //        Category = _db.Category.ToList()

        //    };
        //    return View(IndexVM);
        //}

        public IActionResult Index(string searchString)
        {
            var menuItem = from m in _db.MenuItem
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                menuItem = menuItem.Include(c=>c.Category).Where(s => s.Name.Contains(searchString));
            }

            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = menuItem,
                Category = _db.Category.ToList()
            };
            return View(IndexVM);
        }
    }
}