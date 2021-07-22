using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Utility;
using System.Linq;


namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            
            var listOfCoupons = _db.Coupon.ToList();
            return View(listOfCoupons);
        }

        //GET - Create
        public IActionResult Create()
        {
            return View();
        }

        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if(ModelState.IsValid)
            {               
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var coupon =  _db.Coupon.Find(id);
            if (coupon == null)
                return NotFound();
            return View("Edit",coupon);
        }

       // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Coupon coup)
        {
            var coupon = await _db.Coupon.FindAsync(id);
            if (ModelState.IsValid)
            {
                
                coupon.Name = coup.Name;
                coupon.MinimumAmount = coup.MinimumAmount;
                coupon.Discount = coup.Discount;
                coupon.IsActive = coup.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Edit",coupon);
        }

        //GET - Details
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var coupon = _db.Coupon.Find(id);
            if (coupon == null)
                return NotFound();
            return View("Details",coupon);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var coupon = _db.Coupon.Find(id);
            if (coupon == null)
                return NotFound();
            return View("Delete",coupon);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var coupon = await _db.Coupon.FindAsync(id);
            if (coupon == null)
                return View();
            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}