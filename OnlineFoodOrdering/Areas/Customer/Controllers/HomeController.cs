using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim!=null)
            {
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                var price =(from shp in _db.ShoppingCart
                            join mnt in _db.MenuItem on shp.MenuItemId equals mnt.Id
                            where shp.ApplicationUserId == claim.Value
                            select new
                            {
                                pricee = mnt.Price * shp.Count
                            }).ToList();
                var sum = price.Sum(x => x.pricee);
                HttpContext.Session.SetInt32("ssCartCount", cnt);
                HttpContext.Session.SetString("ssCartPrice", sum.ToString());
            }

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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCartViewModel CartObject)
        {
            CartObject.ShoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ShoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ShoppingCart.ApplicationUserId && c.MenuItemId == CartObject.ShoppingCart.MenuItemId).FirstOrDefaultAsync();

                if(cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(CartObject.ShoppingCart);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.ShoppingCart.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ShoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", count);

                return RedirectToAction("Index");
            }
            else
            {
                var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartObject.ShoppingCart.MenuItemId).FirstOrDefaultAsync();
                ShoppingCartViewModel shopObj = new ShoppingCartViewModel()
                {
                    ShoppingCart = new ShoppingCart()
                    {
                        MenuItem = menuItemFromDb,
                        MenuItemId = menuItemFromDb.Id
                    }
                };
                return View(shopObj);
            }
            
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

        public IActionResult Search()
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
