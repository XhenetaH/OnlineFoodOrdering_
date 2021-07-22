using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var currentUser = _db.ApplicationUser.Where(m => m.Id == claim.Value).FirstOrDefault();
            ViewBag.Name = currentUser.Name;

            var orderCount = _db.Order.Select(m => m.Id).Count();
            var menuCount = _db.MenuItem.Select(m => m.Id).Count();
            var userCount = _db.ApplicationUser.Select(m => m.Id).Count();
            var categoryCount = _db.Category.Select(m => m.Id).Count();
           
            ViewBag.OrderCount = orderCount;
            ViewBag.MenuCount = menuCount;
            ViewBag.UserCount = userCount;
            ViewBag.CategoryCount = categoryCount;
            var monthList = Enumerable.Range(1, DateTime.Today.Month ).Select(m => new DateTime(DateTime.Today.Year, m, 1));
            var monthListNames = Enumerable.Range(1, DateTime.Today.Month ).Select(m => new DateTime(DateTime.Today.Year, m, 1).ToString("MMMM")).ToList();

            List<double> salesList = new List<double>();
            List<double> userList = new List<double>();

            foreach (var item in monthList)
            {
                salesList.Add(_db.Order.Count(m => m.OrderDate.Month == item.Month && m.Status.Contains(SD.StatusCompleted)));
                userList.Add(_db.ApplicationUser.Count(m => m.RegisterDate.Month == item.Month));
            }
            ViewBag.Months = monthListNames;
            ViewBag.Sales = salesList;
            ViewBag.Users = userList;
            return View();
        }
    }
}
