// <copyright file="AccountController.Windows.cs" author="Mohammad Younes">
// Copyright 2013 Mohammad Younes.
// 
// Released under the MIT license
// http://opensource.org/licenses/MIT
//
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MixedAuth.Models;
using Microsoft.AspNet.Identity.Owin;

namespace MixedAuth.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        //
        // POST: /Account/WindowsLogin
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> WindowsLogin(string email, string returnUrl)
        {
            if (!Request.LogonUserIdentity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var externalLoginInfo = GetWindowsLoginInfo();

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(externalLoginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                if (string.IsNullOrEmpty(email))
                    email = Request.LogonUserIdentity.Name;

                var appUser = new ApplicationUser() { UserName = email, Email = email };
                var result = await UserManager.CreateAsync(appUser);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(appUser.Id, externalLoginInfo.Login);
                    if (result.Succeeded)
                    {
                        
                        await SignInAsync(appUser, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = "Windows";
                return View("WindowsLoginConfirmation", new WindowsLoginConfirmationViewModel {Email = email });
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }


        //
        // POST: /Account/WindowsLogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void WindowsLogOff()
        {
            AuthenticationManager.SignOut();
        }

        //
        // POST: /Account/LinkWindowsLogin
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LinkWindowsLogin()
        {
            string userId = HttpContext.ReadUserId();

            //didn't get here through handler
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            HttpContext.Items.Remove("windows.userId");

            //not authenticated.
            var loginInfo = GetWindowsLoginInfo();
            if (loginInfo == null)
                return RedirectToAction("ManageLogins", "Manage");

            //add linked login
            var result = await UserManager.AddLoginAsync(userId, loginInfo.Login);

            //sign the user back in.
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
                await SignInManager.SignInAsync(user, false, false);

            if (result.Succeeded)
                return RedirectToAction("ManageLogins", "Manage");

            return RedirectToAction("ManageLogins", "Manage", new { Message = MixedAuth.Controllers.ManageController.ManageMessageId.Error });
        }

        #region helpers
        private ExternalLoginInfo GetWindowsLoginInfo()
        {
            if (!Request.LogonUserIdentity.IsAuthenticated)
                return null;

            //return new UserLoginInfo("Windows", Request.LogonUserIdentity.User.ToString());
            return new ExternalLoginInfo() {
                Login = new UserLoginInfo("Windows", Request.LogonUserIdentity.User.ToString())             
            };
        }
        #endregion
    }
}