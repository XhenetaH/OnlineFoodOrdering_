using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Models.ViewModels;
using OnlineFoodOrdering.Utility;


namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]

    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            this._db = db;
            this._hostingEnvironment = hostingEnvironment;
            
        }
        public IActionResult Index()
        {            
            var listOfMenuItem = _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).ToList();
            return View(listOfMenuItem);
        }

        //GET - Create
        public IActionResult Create()
        {
            MenuItemViewModel vm = new MenuItemViewModel()
            {
                MenuItem = new MenuItem(),
                Category = _db.Category
            };
            return View("Create",vm);
        }

        //POST - Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(MenuItemViewModel vm)
        {
            vm.Category = _db.Category;
            
            vm.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if(!ModelState.IsValid)
            {
                return View("Create",vm);
            }
            _db.MenuItem.Add(vm.MenuItem);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(vm.MenuItem.Id);
            if(files.Count>0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using(var filesStream = new FileStream(Path.Combine(uploads, vm.MenuItem.Id + extension),FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"/images/" + vm.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images/" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"/images/" + vm.MenuItem.Id + ".jpg");
                menuItemFromDb.Image = @"/images/" + vm.MenuItem.Id + ".jpg";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemViewModel vm = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id),
                
            };
            vm.SubCategory = _db.SubCategory.Where(s => s.CategoryId == vm.MenuItem.CategoryId).ToList();
            

            if(vm.MenuItem == null)
            {
                return NotFound();
            }
            return View("Edit",vm);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItemViewModel vm)
        {
            
            vm.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                vm.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == vm.MenuItem.CategoryId).ToListAsync();
                return View(vm);
            }            

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(vm.MenuItem.Id);

            if (files.Count > 0)
            {               
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);
                
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                
                using (var filesStream = new FileStream(Path.Combine(uploads, vm.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + vm.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = vm.MenuItem.Name;
            menuItemFromDb.Description = vm.MenuItem.Description;
            menuItemFromDb.Price = vm.MenuItem.Price;
            menuItemFromDb.Spicyness = vm.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = vm.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = vm.MenuItem.SubCategoryId;
            menuItemFromDb.isFeatured = vm.MenuItem.isFeatured;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemViewModel vm = new MenuItemViewModel()
            {
                Category = _db.Category
            };
            vm.MenuItem = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
            if (vm.MenuItem == null)
            {
                return NotFound();
            }
            return View("Details",vm);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemViewModel vm = new MenuItemViewModel()
            {
                Category = _db.Category
            };
            vm.MenuItem = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
            if (vm.MenuItem == null)
            {
                return NotFound();
            }
            return View("Delete",vm);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var menuItem = await _db.MenuItem.FindAsync(id);
            var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
                       
            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}