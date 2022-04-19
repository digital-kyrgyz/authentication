using Identity.Models;
using Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVm signIn)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(signIn.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, signIn.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Member");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Туура эмес э-почта же сыр соз");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserVm userVm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();
                user.UserName = userVm.UserName;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;

                IdentityResult result = await _userManager.CreateAsync(user, userVm.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SignIn");
                }
                else
                {
                    foreach (IdentityError item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(userVm);
        }
    }
}