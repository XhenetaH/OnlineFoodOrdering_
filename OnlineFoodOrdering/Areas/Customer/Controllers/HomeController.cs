using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
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

        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);
        }

        public IActionResult Index()
        {
            List<Models.Coupon> coupon = new List<Models.Coupon>();

            coupon = _db.Coupon.Where(c => c.IsActive == true).ToList();
            ViewBag.Coupons = coupon;
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItemFeatured = _db.MenuItem.Where(m=>m.isFeatured==true).ToList(),
                MenuItem = _db.MenuItem.OrderByDescending(n=>n.Id).ToList(),             
                Coupon = _db.Coupon.Where(c => c.IsActive == true).ToList()
            };



            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim!=null)
            {
                var list = _db.ShoppingCart.Include(m=>m.MenuItem).Where(u => u.ApplicationUserId == claim.Value).ToList();
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                var vishcount = _db.WishList.Where(m => m.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32("ssCartCount", cnt);
                HttpContext.Session.SetInt32("ssWishCount", vishcount);
                ViewBag.Cart = list;
            }

            return View(IndexVM);
        }

        public IActionResult Contact()
        {
            return View();
        }

       
        public IActionResult AboutUs()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Compare(IndexViewModel compareObj)
        {
            
            if(ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                compareObj.Compare.ApplicationUserId = claim.Value;

                Compare compareFromDb = await _db.Compare.Where(c => c.ApplicationUserId == compareObj.Compare.ApplicationUserId && c.MenuItemId == compareObj.Compare.MenuItemId).FirstOrDefaultAsync();
                List<Compare> compareList = await _db.Compare.ToListAsync();

                if(compareFromDb == null)
                {
                    if (compareList.Count == 3)
                    {
                        var db = _db.Compare.OrderBy(g => g.Id).Take(1);

                        _db.Compare.RemoveRange(db);

                        await _db.Compare.AddAsync(compareObj.Compare);

                    }
                    else
                    {
                        await _db.Compare.AddAsync(compareObj.Compare);                                           
                    }
                }
                await _db.SaveChangesAsync();

                return RedirectToAction("Index","Compare");
                
            }
            else
            {
                return View();
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WishList(IndexViewModel wishObject)
        {
            wishObject.WishList.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                wishObject.WishList.ApplicationUserId = claim.Value;

                WishList wishFromDb = await _db.WishList.Where(c => c.ApplicationUserId == wishObject.WishList.ApplicationUserId && c.MenuItemId == wishObject.WishList.MenuItemId).FirstOrDefaultAsync();
                

                if (wishFromDb == null)
                {
                    _db.WishList.Add(wishObject.WishList);
                }
                
                await _db.SaveChangesAsync();
                var count = _db.WishList.Where(c => c.ApplicationUserId == wishObject.WishList.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("ssWishCount", count);
            
                return RedirectToAction("Index", "WishList");

            }
            else
            {
                return View();
            }

        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(IndexViewModel CartObject)
        {
            CartObject.ShoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ShoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ShoppingCart.ApplicationUserId && c.MenuItemId == CartObject.ShoppingCart.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
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

                return RedirectToAction("Index","Cart");
            }
            else
            {

                return View();
            }

        }

        [Authorize]
        public IActionResult Details(int id)
        {
            
            var menuItemFromDb =  _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefault();
            ShoppingCartViewModel shopObj = new ShoppingCartViewModel()
            {
                ShoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                },
                MenuItemList = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Category.Name == menuItemFromDb.Category.Name).ToList()
                
            };
            if (shopObj.Ratings != null)
            {
                if (shopObj.Ratings.Count() > 0)
                {
                    var ratingSum = shopObj.Ratings.Sum(d => d.Rating);
                    ViewBag.RatingSum = ratingSum;
                    var ratingCount = shopObj.Ratings.Count();
                    ViewBag.RatingCount = ratingCount;
                }
                else
                {
                    ViewBag.RatingSum = 0;
                    ViewBag.RatingCount = 1;
                }
            }
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

        public IActionResult GetProductDetails(int Id)
        {
            ShoppingCartViewModel shpmodel = new ShoppingCartViewModel()
            {
                ShoppingCart = new ShoppingCart()
                {
                    MenuItem = _db.MenuItem.FirstOrDefault(m => m.Id == Id)
                },
                Ratings = _db.Ratings.Where(m=>m.MenuItemId == Id).ToList()
                
            };

            if (shpmodel.Ratings.Count() > 0)
            {
                var ratingSum = shpmodel.Ratings.Sum(d => d.Rating);
                ViewBag.RatingSum = ratingSum;
                var ratingCount = shpmodel.Ratings.Count();
                ViewBag.RatingCount = ratingCount;
            }
            else
            {
                ViewBag.RatingSum = 0;
                ViewBag.RatingCount = 0;
            }
            return PartialView("_ProductDetails", shpmodel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(ShoppingCartViewModel rmodel)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
                
                rmodel.Rating.UserId = claim.Value;

                _db.Ratings.Add(rmodel.Rating);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", "Home", new { id = rmodel.Rating.MenuItemId });
            
            
        }

        
    }
}
