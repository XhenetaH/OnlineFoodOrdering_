using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Utility;
using System.Linq;

namespace OnlineFoodOrdering.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET 
        public IActionResult Index()
        {
            var list = _db.Category.ToList();

            return View(list);
        }

        //GET - Create
        public IActionResult Create()
        {
            return View("Create");
        }
        
        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var category =  _db.Category.Find(id);
            if (category == null)
                return NotFound();
            return View("Edit",category);
        }
        

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var category = _db.Category.Find(id);
            if (category == null)
                return NotFound();
            return View("Delete",category);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _db.Category.FindAsync(id);
            if (category == null)
                return View();
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - Details
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var category = _db.Category.Find(id);
            if (category == null)
                return NotFound();
            return View("Details",category);
        }
    }
}