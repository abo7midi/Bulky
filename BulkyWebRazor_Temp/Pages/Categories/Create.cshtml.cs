using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
           
        }
        public IActionResult OnPost()
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            TempData["Success"] = "Category Created Successfully";

            return RedirectToPage("Index");
        }
    }
}
