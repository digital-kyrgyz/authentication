using Identity.Models;
using Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
            : base(userManager, signInManager, null)
        {

        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
            return View();
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
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
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Аккаунттунуз белгилуу убакытка блоктолгон. Сураныч бир аздан кийин кайра кириниз.");
                        return View(signIn);
                    };

                    if (_userManager.IsEmailConfirmedAsync(user).Result == false)
                    {
                        ModelState.AddModelError("", "Сиздин э-почтаныз тастыктала элек. Э-почтанызды караныз");
                        return View(signIn);
                    }

                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, signIn.Password, signIn.RememberMe, false);
                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);

                        if (TempData["returnUrl"] != null)
                        {
                            return Redirect(TempData["returnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);

                        int fail = await _userManager.GetAccessFailedCountAsync(user);
                        ModelState.AddModelError("", $"{fail} жолу ийгиликсиз кируу");
                        if (fail == 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                            ModelState.AddModelError("", "Аккаунттунуз 20 минутка блоктолду");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Туура эмес э-почта же сыр соз");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Бул э-почта табылганы жок");
                }
            }
            return View(signIn);
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
                if(_userManager.Users.Any(u=>u.PhoneNumber == userVm.PhoneNumber))
                {
                    ModelState.AddModelError("", "Мындай номер симтемада бар");
                    return View(userVm);
                }
                AppUser user = new AppUser();
                user.UserName = userVm.UserName;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;

                IdentityResult result = await _userManager.CreateAsync(user, userVm.Password);
                if (result.Succeeded)
                {
                    string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string link = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = user.Id,
                        token = confirmationToken,

                    }, protocol: HttpContext.Request.Scheme);

                    Helper.EmailConfirmation.EmailСonfirmSend(link, user.Email);

                    return RedirectToAction("SignIn");
                }
                else
                {
                    AddModelError(result);
                }
            }
            return View(userVm);
        }

        [HttpGet]
        public new void SignOut()
        {
            _signInManager.SignOutAsync();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordVm passwordVm)
        {
            var user = _userManager.FindByEmailAsync(passwordVm.Email).Result;
            if (user != null)
            {
                string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token = passwordResetToken,
                }, HttpContext.Request.Scheme);
                //www.usman.kg/home/resetpasswordconfirm?userid=lskdjf=sldkfdlsf
                Helper.PasswordReset.PasswordResetSendEmail(passwordResetLink, user.Email);
                ViewBag.Status = "succes";
            }
            else
            {
                ModelState.AddModelError("", "Системада мындай колдонуучу жок");
            }
            return View(passwordVm);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("NewPassword")] ResetPasswordVm passwordVm)
        {
            if (TempData["token"] != null)
            {
                string token = TempData["token"].ToString();
                string userId = TempData["userId"].ToString();
                AppUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    IdentityResult result = await _userManager.ResetPasswordAsync(user, token, passwordVm.NewPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        ViewBag.Status = "success";
                    }
                    else
                    {
                        AddModelError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Бир ката чыкты. Бир аздан кийин кайра кирип корунуз");
                }
            }
            else
            {
                ModelState.AddModelError("", "Бул токен жараксыз");
            }
            return View(passwordVm);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                ViewBag.Status = "Э-почтаныз тастыкталды";

            else
                ViewBag.Status = "Бир ката чыкты кайра кирип корунуз";

            return View();
        }

        [HttpGet]
        public IActionResult FacebookSignIn(string ReturnUrl)
        {
            string redirectUrl = Url.Action("Response", new { redirectUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }

        [HttpGet]
        public IActionResult GoogleSignIn(string ReturnUrl)
        {
            string redirectUrl = Url.Action("Response", new { redirectUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [HttpGet]
        public IActionResult MicrosoftSignIn(string ReturnUrl)
        {
            string redirectUrl = Url.Action("Response", new { redirectUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
            return new ChallengeResult("Microsoft", properties);
        }

        [HttpGet]
        public async new Task<IActionResult> Response(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {

                return RedirectToAction("SignIn", "Home");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                if (result.Succeeded)
                    return Redirect(ReturnUrl);

                else
                {
                    AppUser user = new AppUser();
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string externalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(' ', '-').ToLower() + externalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    AppUser userOld = await _userManager.FindByEmailAsync(user.Email);

                    if (userOld == null)
                    {
                        IdentityResult createResult = await _userManager.CreateAsync(user);

                        if (createResult.Succeeded)
                        {
                            IdentityResult loginResult = await _userManager.AddLoginAsync(user, info);

                            if (loginResult.Succeeded)
                            {
                                //await _signInManager.SignInAsync(user, true);
                                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                AddModelError(createResult);
                            }
                        }
                        else
                        {
                            AddModelError(createResult);
                        }
                    }
                    else
                    {
                        IdentityResult loginResult = await _userManager.AddLoginAsync(userOld, info);
                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                        return Redirect(ReturnUrl);
                    }
                }
            }
            List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();
            return View("Error", errors);
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}