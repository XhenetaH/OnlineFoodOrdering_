﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Models.ViewModels;

namespace OnlineFoodOrdering.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

      
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
            ShoppingCartViewModel shopObj = new ShoppingCartViewModel()
            {
                ShoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                },
                MenuItemList = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Category.Name == menuItemFromDb.Category.Name).ToListAsync()
        };


            return View(shopObj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
