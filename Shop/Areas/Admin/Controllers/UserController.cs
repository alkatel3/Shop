using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUnitOfWork UoW;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UoW = unitOfWork;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            RoleManagmentVM RoleVM = new()
            {
                ApplicationUser = UoW.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = UoW.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = userManager.GetRolesAsync(UoW.ApplicationUser.Get(u => u.Id == userId))
                .GetAwaiter().GetResult().FirstOrDefault();
        
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string oldRole = userManager.GetRolesAsync(UoW.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = UoW.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);

            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                UoW.ApplicationUser.Update(applicationUser);
                UoW.Save();
                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(oldRole==SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    UoW.ApplicationUser.Update(applicationUser);
                    UoW.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = UoW.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach(var user in users)
            {
                user.Role = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
            var userDb = UoW.ApplicationUser.Get(u => u.Id == id);
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
            UoW.ApplicationUser.Update(userDb);
            UoW.Save();

            return Json(new { success = true, message = "Operation successful" });
        }
        #endregion
    }
}
