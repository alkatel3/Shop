using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Utility;
using System.Data;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            string RoleId = db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagmentVM RoleVM = new()
            {
                ApplicationUser = db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
        
            return View(RoleVM);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = db.ApplicationUsers.Include(u=>u.Company).ToList();

            var userRoles = db.UserRoles.ToList();
            var roles = db.Roles.ToList();

            foreach(var user in users)
            {
                var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var userDb = db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (userDb.LockoutEnd != null && userDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                userDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Operation successful" });
        }
        #endregion
    }
}
