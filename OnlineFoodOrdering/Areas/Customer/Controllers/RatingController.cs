using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;

namespace OnlineFoodOrdering.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RatingController(ApplicationDbContext db)
        {
            this._db = db;
        }

        //Get
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating(Ratings ratingmodel)
        {           
            
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ratingmodel.UserId = claim.Value;
                _db.Ratings.Add(ratingmodel);
                _db.SaveChanges();
            return PartialView(ratingmodel);
        }
    }
}