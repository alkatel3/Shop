using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Models.ViewModels;
using System.Collections.Generic;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController:Controller
    {
        private readonly IUnitOfWork UoW;

        public ProductController(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> products = UoW.Product.GetAll().ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = UoW.Category
            .GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            ProductVM productVM = new ProductVM
            {
                CategoryList = UoW.Category
                .GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                UoW.Product.Add(productVM.Product);
                UoW.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM = new ProductVM
                {
                    CategoryList = UoW.Category
                    .GetAll().Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                };
                return View(productVM);
            }
        }

        public IActionResult Edit(int? id)
        {
            if(id==null && id == 0)
            {
                return NotFound();
            }

            Product? productDb = UoW.Product.Get(p => p.Id == id);
            if (productDb == null)
            {
                return NotFound();
            }

            return View(productDb);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                UoW.Product.Update(product);
                UoW.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }

            Product? productDb = UoW.Product.Get(p => p.Id == id);
            if (productDb == null)
            {
                return NotFound();
            }

            return View(productDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? productDb = UoW.Product.Get(p => p.Id == id);
            if (productDb == null)
            {
                return NotFound();
            }

            UoW.Product.Delete(productDb);
            UoW.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
