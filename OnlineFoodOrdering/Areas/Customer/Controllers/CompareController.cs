using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Models.ViewModels;

namespace OnlineFoodOrdering.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CompareController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CompareController(ApplicationDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CompareViewModel compareList = new CompareViewModel()
            {
                CompareList = _db.Compare.Include(m => m.MenuItem.Category).Where(c => c.ApplicationUserId == claim.Value).ToList(),
            };                       
            return View(compareList);
        }


        public async Task<IActionResult> Remove(int compareId)
        {
            var compareItem = await _db.Compare.FirstOrDefaultAsync(c => c.Id == compareId);
            _db.Compare.Remove(compareItem);
            await _db.SaveChangesAsync();
            

            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(CompareViewModel CompareObject)
        {
           
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
               
                ShoppingCart shop = new ShoppingCart();
                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value && c.MenuItemId == CompareObject.Compare.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    var mn = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == CompareObject.Compare.MenuItemId);
                    
                    shop.ApplicationUserId = claim.Value;
                    shop.Count = 1;
                    shop.MenuItemId = mn.Id;
                    await _db.ShoppingCart.AddAsync(shop);
                }
                else
                {
                    return RedirectToAction("Index","Cart");
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == shop.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", count);

                return RedirectToAction("Index", "Cart");
            }
            else
            {
               
                return View();
            }

        }
    }
}