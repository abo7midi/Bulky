using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCaegoryList= _db.Categories.ToList();
            return View(objCaegoryList);
        }

        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //if(category.Name == category.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The Display ordeer cannot exact match the Name.");
            //}
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }
            //Same Result
            Category? categoryFromDb = _db.Categories.Find(id);
            //Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            //Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (categoryFromDb==null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            //if(category.Name == category.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The Display ordeer cannot exact match the Name.");
            //}
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
                _db.Categories.Remove(category);
                _db.SaveChanges();
            TempData["Success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
            
        }
    }
}
