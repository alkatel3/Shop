using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Utility;
using System.Data;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork UoW;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> Companys = UoW.Company.GetAll().ToList();
            return View(Companys);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id==0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                var Company = UoW.Company.Get(p => p.Id == id);
                return View(Company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
 
                if (company.Id == 0)
                {
                    UoW.Company.Add(company);
                }
                else
                {

                    UoW.Company.Update(company);
                }

                UoW.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companys = UoW.Company.GetAll().ToList();
            return Json(new { data = Companys });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var Company = UoW.Company.Get(p => p.Id == id);
            if (Company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            UoW.Company.Remove(Company);
            UoW.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
