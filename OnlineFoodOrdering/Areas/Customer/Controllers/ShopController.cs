using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index(string sortOrder, string searchTerm, int? categoryID)
        {
            ViewBag.LatestItems = String.IsNullOrEmpty(sortOrder) ? "latest_item" : "";
            ViewBag.HighPrice = String.IsNullOrEmpty(sortOrder) ? "high_price" : "";
            ViewBag.LowPrice = String.IsNullOrEmpty(sortOrder) ? "low_price" : "";

            ShopViewModel model = new ShopViewModel();

            var menuitem = from s in _db.MenuItem
                           select s;
            model.MenuItem = menuitem;
            model.Category = _db.Category.ToList();
            model.MenuItemFeatured = _db.MenuItem.Where(m => m.isFeatured == true).ToList();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                model.MenuItem = menuitem.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }
            if(categoryID.HasValue)
            {
                model.MenuItem = menuitem.Where(x => x.Category.Id == categoryID.Value).ToList();
            }   
            switch (sortOrder)
            {
                case "latest_item":
                    model.MenuItem = menuitem.OrderByDescending(s => s.Id).ToList();
                    break;
                case "high_price":
                    model.MenuItem = menuitem.OrderByDescending(p => p.Price).ToList();
                    break;
                case "low_price":
                    model.MenuItem = menuitem.OrderBy(p => p.Price).ToList();
                    break;
            }
            return View(model);
        }
                
    }
}