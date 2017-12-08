using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SAAS_AIMS.Models;
using AIMS.Data.DataObjects.Entities.Role;
using AIMS.Data.DataContext.DataContext.RoleDataContext;
using System.Data.Entity;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Services.RandomStringGenerator;
using WebMatrix.WebData;
using Microsoft.Owin.Security.DataProtection;
using System.Net.Mail;

namespace SAAS_AIMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly RoleDataContext _roledatacontext;

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AppUserDataContext())))
        {
            _roledatacontext = new RoleDataContext();

            this.UserManager.UserValidator = new UserValidator<ApplicationUser>(this.UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("AIMS");
            this.UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordReset"));

        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        #region custom methods
        [HttpGet]
        [Authorize]
        public ActionResult RoleIndex()
        {
            var role = from m in _roledatacontext.Roles
                       select m;
            return View("RoleIndex", role);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateRole()
        {
            return View("CreateRole");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                if (role.Title == "Superuser")
                {
                    ModelState.AddModelError("Impossible", "The role 'Superuser' already exists!");
                    TempData["Impossible"] = "The role 'Superuser' already exists!";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                    return RedirectToAction("RoleIndex");
                }
                else
                {
                    _roledatacontext.Roles.Add(role);
                    _roledatacontext.SaveChanges();
                    TempData["Success"] = "Role successfully created! ";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                }
                return RedirectToAction("RoleIndex");
            }
            return View("CreateRole", role);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> EditRole(long id)
        {
            var role = await _roledatacontext.Roles.FindAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            if (role.Title == "Superuser")
            {
                TempData["Impossible"] = "The role 'Superuser' cannot be modified!";
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("RoleIndex");
            }
            return View("EditRole", role);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                _roledatacontext.Entry(role).State = EntityState.Modified;
                _roledatacontext.SaveChanges();
                TempData["Success"] = "Role successfully modified! ";
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("RoleIndex");
            }
            return View("EditRole", role);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Permissions(long id)
        {
            var role = await _roledatacontext.Roles.FindAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return PartialView("Permissions", role);
        }

        [Authorize]
        public async Task<ActionResult> DeleteRole(long id)
        {
            var role = await _roledatacontext.Roles.FindAsync(id);
            if (role.Title == "Superuser")
            {
                TempData["Impossible"] = "The role 'Superuser' cannot be deleted! ";
                TempData["NotificationType"] = NotificationType.Delete.ToString();
                return RedirectToAction("RoleIndex");
            }
            else
            {
                _roledatacontext.Roles.Remove(role);
                await _roledatacontext.SaveChangesAsync();
                TempData["Success"] = "Role successfully deleted!";
                TempData["NotificationType"] = NotificationType.Delete.ToString();
            }
            return RedirectToAction("RoleIndex");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UserIndex()
        {
            var context = new AppUserDataContext();
            var user = context.Users.ToList();
            return View("UserIndex", user);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [Authorize]
        public ActionResult Register()
        {
            ViewBag.Role = new SelectList(_roledatacontext.Roles, "ID", "Title");
            ViewBag.hash = new RandomStringGenerator().GenerateString();
            return PartialView();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var _context = new AppUserDataContext();
                var _role = new RoleDataContext();
                var role = await _role.Roles.FindAsync(model.RoleID);
                var super = _context.Users.Where(s => s.Role.Title == "Superuser").ToArray();

                if (_context.Users.Any(s => s.Email == model.Email))
                {
                    ModelState.AddModelError("", "This e-mail is already in use!");
                }
                if (role.Title == "Superuser" && super.Length == 1)
                {
                    ModelState.AddModelError("", "There can only be one superuser!");
                }
                else
                {
                    var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, RoleID = model.RoleID };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var message = new MailMessage();
                        message.Priority = MailPriority.High;
                        message.From = new MailAddress("no-reply@override.dev", "Override");
                        message.Subject = "Login Details";

                        message.To.Add(new MailAddress(model.Email));
                        
                        var url = Url.Action("Login", "Account", new {}, protocol: Request.Url.Scheme);

                        var emailBody =
                        "<div>" +
                        "<h3 style='font-size: 30px; text-align:center;'><strong>ASSOCIATION INFORMATION MANAGEMENT SYSTEM</strong></h3>" +
                        "<div style='position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px; padding-top: 5px;'>" +
                            "<h4 style='font-size: 18px; text-align:justify;'>You have been added to the Association Information Management System. </h4>" + 
                            "<p style='font-size: 18px; text-align:justify;'>Your username is " + model.Email + " and your password is " + model.Password +
                            ". Please login and change your password by clicking <a href=\"" + url + "\">here</a></p>" +
                        "<footer style='font-size: 18px; text-align:center;'>" +
                            "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                        message.Body = string.Format(emailBody);
                        message.IsBodyHtml = true;

                        using (var smtp = new SmtpClient())
                        {
                            await smtp.SendMailAsync(message);
                        }
                
                        TempData["Success"] = "User account successfully created! Check email for login credentials.";
                        TempData["NotificationType"] = NotificationType.Create.ToString();
                        return Json(new { success = true });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Role = new SelectList(_roledatacontext.Roles, "ID", "Title", model.RoleID);
            return PartialView(model);
        }

        [Authorize]
        public ActionResult RemoveUser(string UserId)
        {
            AppUserDataContext app = new AppUserDataContext();
            var user = app.Users.Find(UserId);
            if (user.Role.Title == "Superuser")
            {
                TempData["Impossible"] = "This user cannot be removed!";
                TempData["NotificationType"] = NotificationType.Delete.ToString();
            }
            else
            {
                app.Users.Remove(user);
                app.SaveChanges();
                TempData["Success"] = "User account successfully removed!";
                TempData["NotificationType"] = NotificationType.Delete.ToString();
            }
            return RedirectToAction("UserIndex");
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        //
        // GET: /Account/LostPassword
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            return View();
        }

        //
        // POST: /Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LostPassword(LostPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                //Send email start
                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("overreid@gmail.com", "AIMS");
                message.To.Add(new MailAddress(user.Email));
                var emailBody =
                "<div>" +
                    "<h3 style='font-size: 30px; text-align:center;'><strong>ASSOCIATION INFORMATION MANAGEMENT SYSTEM</strong></h3>" +
                    "<div style='position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px; padding-top: 5px;'>" +
                        "<h4 style='font-size: 18px; text-align:justify;'>Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></h4>" +
                    "<footer style='font-size: 18px; text-align:center;'>" +
                        "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";
                message.Subject = "Reset Password";
                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                } 
                //Send email end

                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.ReturnToken = code;
            model.UserId = userId;
            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = UserManager.ResetPasswordAsync(model.UserId, model.ReturnToken, model.Password).Result;
                if (result.Succeeded)
                {
                    ViewBag.Message = "Successfully Changed!";
                    return View("ResetSuccess");
                }
                else
                {
                    ViewBag.Message = "Something went horribly wrong!";
                    return View("ResetFailure");
                }
            }
            return View(model);
        }

        #endregion

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    Session["UserID"] = user.Id;
                    Session["role"] = user.Role;
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult FirstRegistration()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FirstRegistration(RegisterViewModel model, Role role)
        {

            if (ModelState.IsValid)
            {
                var rolevar = new Role
                {
                    Title = "Superuser",
                    CanManageSessions = true,
                    CanManageMembers = true,
                    CanManageMeetings = true,
                    CanManageIncome = true,
                    CanManageExpenses = true,
                    CanManageExecutives = true,
                    CanManageEvents = true,
                    CanManageBudget = true,
                    CanManageMemos = true,
                    CanManageProjects = true,
                    CanManageUsers = true
                };

                _roledatacontext.Roles.Add(rolevar);
                _roledatacontext.SaveChanges();

                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, RoleID = rolevar.ID };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    Session["UserID"] = user.Id;
                    Session["role"] = user.Role;
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}