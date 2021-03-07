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
    public class WishListController : Controller
    {
        private readonly ApplicationDbContext _db;
        public WishListController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            WishListViewModel wishList = new WishListViewModel()
            {
                WishLista = _db.WishList.Include(m=>m.MenuItem).Where(m => m.ApplicationUserId == claim.Value).ToList()
            };       
            return View(wishList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(WishListViewModel WishListObject)
        {

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCart shop = new ShoppingCart();


                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value && c.MenuItemId == WishListObject.WishList.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    var mn = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == WishListObject.WishList.MenuItemId);

                    shop.ApplicationUserId = claim.Value;
                    shop.Count = 1;
                    shop.MenuItemId = mn.Id;
                    await _db.ShoppingCart.AddAsync(shop);
                }
                else
                {
                    return RedirectToAction("Index", "Cart");
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

        public async Task<IActionResult> Remove(int wishId)
        {
            var wishItem = await _db.WishList.FirstOrDefaultAsync(c => c.Id == wishId);
            _db.WishList.Remove(wishItem);
            await _db.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}