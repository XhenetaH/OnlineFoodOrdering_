using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Models.ViewModels;
using X.PagedList;

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
        
        //public async Task<IActionResult> Index(string searchTerm, int? minimumPrice,int? maximumPrice, int? categoryID, int? sortBy)
        //{
        //    ShopViewModel model = new ShopViewModel();
        //    model.Category = await _db.Category.ToListAsync();
        //    model.MaximumPrice = await _db.MenuItem.MaxAsync(x => x.Price);
        //    model.MenuItemFeatured = await _db.MenuItem.Where(m => m.isFeatured == true).ToListAsync();

        //    var menuitems = await _db.MenuItem.ToListAsync();
        //    if(categoryID.HasValue)
        //    {
        //        menuitems = menuitems.Where(x => x.Category.Id == categoryID.Value).ToList();
        //    }
        //    if(!string.IsNullOrEmpty(searchTerm))
        //    {
        //        menuitems = menuitems.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
        //    }
        //    if(minimumPrice.HasValue)
        //    {
        //        menuitems = menuitems.Where(x => x.Price >= minimumPrice.Value).ToList();
        //    }
        //    if(maximumPrice.HasValue)
        //    {
        //        menuitems = menuitems.Where(x => x.Price <= maximumPrice.Value).ToList();
        //    }
        //    if(sortBy.HasValue)
        //    {
        //        switch(sortBy.Value)
        //        {
        //            case 2:
        //                menuitems = menuitems.OrderByDescending(x => x.Id).ToList();
        //                break;
        //            case 3:
        //                menuitems = menuitems.OrderBy(x => x.Price).ToList();
        //                break;
        //            default:
        //                menuitems = menuitems.OrderByDescending(x => x.Price).ToList();
        //                break;
        //        }
        //    }


        //    model.MenuItem = menuitems;
        //    model.SortBy = sortBy;
        //    model.CategoryId = categoryID;
        //    return View(model);






            //IQueryable<string> categoryQuery = from m in _db.MenuItem
            //                                   orderby m.Category
            //                                   select m.Category.Name;
            //var menuItem = from m in _db.MenuItem
            //               select m;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    menuItem = menuItem.Where(s => s.Name.Contains(searchString));
            //}

            //if(!string.IsNullOrEmpty(itemCategory))
            //{
            //    menuItem = menuItem.Where(x => x.Category.Name == itemCategory);
            //}
            //var shopVM = new ShopViewModel
            //{
            //    CategoryList = new SelectList(await categoryQuery.Distinct().ToListAsync()),
            //    MenuItem = await menuItem.ToListAsync(),
            //    Category = await _db.Category.Distinct().ToListAsync()
            //};
            //return View(shopVM);
        
    }
}