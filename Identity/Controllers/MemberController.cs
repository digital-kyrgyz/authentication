using Identity.Models;
using Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Identity.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Identity.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            UserVm userVm = user.Adapt<UserVm>();
            return View(userVm);
        }

        [HttpGet]
        public IActionResult UserEdit()
        {
            AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            UserVm userVm = user.Adapt<UserVm>();
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserVm userVm, IFormFile userPicture)
        {
            ModelState.Remove("Password");
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        user.Picture = "/UserPicture/" + fileName;
                    }
                }
                user.UserName = userVm.UserName;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;
                user.City = userVm.City;
                user.Gender = (int)userVm.Gender;
                user.BirthDay = userVm.BirthDay;

               IdentityResult result =  await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    ViewBag.Success = "true";
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(userVm);
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeVm passwordVm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                
                    bool exist = _userManager.CheckPasswordAsync(user, passwordVm.PasswordOld).Result;
                    if (exist)
                    {
                        IdentityResult result = _userManager.ChangePasswordAsync(user, passwordVm.PasswordOld, passwordVm.PasswordNew).Result;
                        if (result.Succeeded)
                        {
                            _userManager.UpdateSecurityStampAsync(user);
                            _signInManager.SignOutAsync();
                            _signInManager.PasswordSignInAsync(user, passwordVm.PasswordNew, true, false);

                            ViewBag.Success = "true";
                        }
                        else
                        {
                            foreach (var item in result.Errors)
                                ModelState.AddModelError("", item.Description);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Эски сыр туура эмес");
                    }
            }
            return View(passwordVm);
        }
    }
}