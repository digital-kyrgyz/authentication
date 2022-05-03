using Identity.Models;
using Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Mapster;
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