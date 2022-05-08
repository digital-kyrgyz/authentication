using Identity.Models;
using Identity.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(
          UserManager<AppUser> userManager,
          RoleManager<AppRole> roleManager)
          : base(userManager, null, roleManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Claims()
        {
            return View(User.Claims.ToList());
        }

        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleVm roleVm)
        {
            if (ModelState.IsValid)
            {
                AppRole appRole = new AppRole();
                appRole.Name = roleVm.Name;
                IdentityResult result = _roleManager.CreateAsync(appRole).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Admin");
                }
                else
                {
                    AddModelError(result);
                }
            }
            return View(roleVm);
        }

        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        public IActionResult Delete(string id)
        {
            if (id != null)
            {
                AppRole role = _roleManager.FindByIdAsync(id).Result;
                if (role != null)
                {
                    IdentityResult result = _roleManager.DeleteAsync(role).Result;
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles", "Admin");
                    }
                    else
                    {
                        ViewBag.Errors = "Бир ката чыкты";
                    }
                }
            }
            else
            {
                ViewBag.Errors = $"Базада мындай рол жок";
            }
            return RedirectToAction("Roles", "Admin");
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            AppRole role = _roleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                return View(role.Adapt<RoleVm>());
            }

            return RedirectToAction("Roles", "Admin");
        }

        [HttpPost]
        public IActionResult Update(RoleVm roleVm)
        {
            AppRole role = _roleManager.FindByIdAsync(roleVm.Id).Result;
            if (role != null)
            {
                role.Name = roleVm.Name;
                IdentityResult result = _roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Admin");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Мындай рол жок");
            }
            return View(roleVm);
        }

        [HttpGet]
        public IActionResult RoleAssign(string id)
        {
            TempData["userId"] = id;
            AppUser user = _userManager.FindByIdAsync(id).Result;
            ViewBag.UserName = user.UserName;
            IQueryable<AppRole> roles = _roleManager.Roles;
            List<string> userRoles = _userManager.GetRolesAsync(user).Result as List<string>;

            List<RoleAssignVm> roleAssignments = new List<RoleAssignVm>();



            foreach (var role in roles)
            {
                RoleAssignVm r = new RoleAssignVm();
                r.Id = role.Id;
                r.Name = role.Name;

                if (userRoles.Contains(role.Name))
                {
                    r.IsSelected = true;
                }
                else
                {
                    r.IsSelected = false;
                }
                roleAssignments.Add(r);
            }
            return View(roleAssignments);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignVm> roleAssigns)
        {
            AppUser user = _userManager.FindByIdAsync(TempData["userId"].ToString()).Result;
            foreach (var item in roleAssigns)
            {
                if (item.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, item.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> ResetUserPasswordByAdmin(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            ResetPasswordByAdminVm newPasswordVm = new ResetPasswordByAdminVm();
            newPasswordVm.UserId = user.Id;
            return View(newPasswordVm);
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPasswordByAdmin(ResetPasswordByAdminVm passwordVm)
        {
            AppUser user = await _userManager.FindByIdAsync(passwordVm.UserId);

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, passwordVm.NewPassword);
            //This security stamp very important field, ok!
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction("Users", "Admin");
        }
    }
}
