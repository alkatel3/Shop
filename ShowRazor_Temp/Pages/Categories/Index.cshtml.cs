using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShowRazor_Temp.Data;
using ShowRazor_Temp.Models;

namespace ShowRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext db;
        public List<Category> Categories { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet()
        {
            Categories = db.Categories.ToList();
        }
    }
}
