using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShowRazor_Temp.Data;
using ShowRazor_Temp.Models;

namespace ShowRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext db;

        public Category Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet(int? id)
        {
            if(id!=null && id != 0)
            {
                Category = db.Categories.Find(id);
            }


        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                db.Categories.Update(Category);
                db.SaveChanges();
                TempData["success"] = "Category undate successfully";
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
