﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Utility;


namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View( _db.ApplicationUser.Where(u => u.Id != claim.Value).ToList());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if(applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}