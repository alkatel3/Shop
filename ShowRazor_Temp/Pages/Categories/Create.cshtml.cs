using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShowRazor_Temp.Data;
using ShowRazor_Temp.Models;

namespace ShowRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext db;

        public Category Category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            db.Categories.Add(Category);
            db.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToPage("Index");
        }
    }
}
