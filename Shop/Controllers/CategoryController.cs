using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categotyRepository;

        public CategoryController(ICategoryRepository categotyRepository)
        {
            this.categotyRepository = categotyRepository;
        }

        public IActionResult Index()
        {
            List<Category> Categories = categotyRepository.GetAll().ToList();
            return View(Categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }

            if (ModelState.IsValid)
            {
                categotyRepository.Add(category);
                categotyRepository.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryDb = categotyRepository.Get(c=>c.Id==id);
            if (categoryDb == null)
            {
                return NotFound();
            }

            return View(categoryDb);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categotyRepository.Update(category);
                categotyRepository.Save();
                TempData["success"] = "Category undate successfully";
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

            Category? categoryDb = categotyRepository.Get(c => c.Id == id);
            if (categoryDb == null)
            {
                return NotFound();
            }

            return View(categoryDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? category = categotyRepository.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            categotyRepository.Delete(category);
            categotyRepository.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
