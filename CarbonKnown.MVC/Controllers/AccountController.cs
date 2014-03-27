using System;
using System.Resources;
using System.Web.Mvc;
using System.Web.Security;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Models;
using WebMatrix.WebData;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly ResourceManager resources = new ResourceManager(typeof (AccountController));

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Dashboard");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ReturnUrl = returnUrl;
            ModelState.AddModelError(string.Empty, resources.GetString("Login_Incorrect"));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    var profile = new UserProfile
                        {
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = model.UserName,
                        };
                    accountService.UpsertUserProfile(profile);
                    return RedirectToAction("Index", "Dashboard");
                }
                catch (MembershipCreateUserException e)
                {
                    var resultName = Enum.GetName(typeof (MembershipCreateStatus), e.StatusCode);
                    var resourceName = "MembershipCreateStatus_" + resultName;
                    ModelState.AddModelError(string.Empty, resources.GetString(resourceName));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var userProfile = accountService.GetUser(User.Identity.Name);
            var model = new PasswordModel
                {
                    ChangePassword = true,
                    Email = userProfile.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(PasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var userProfile = new UserProfile
                    {
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserId = WebSecurity.GetUserId(username),
                        UserName = username
                    };
                accountService.UpsertUserProfile(userProfile);

                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                if (model.ChangePassword)
                {
                    try
                    {
                        if (WebSecurity.Login(username, model.OldPassword))
                        {
                            WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                        }
                        else
                        {
                            ModelState.AddModelError("", resources.GetString("Manage_PasswordChangeFailed"));
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", resources.GetString("Manage_PasswordChangeFailed"));
                    }
                }

                ViewBag.StatusMessage = ModelState.IsValid
                                            ? resources.GetString("Manage_Succeeded")
                                            : resources.GetString("Manage_Failed");
            }
            return View(model);
        }
    }
}
