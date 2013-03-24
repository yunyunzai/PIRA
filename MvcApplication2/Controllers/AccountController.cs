using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MvcApplication2.Filters;
using MvcApplication2.Models;
using System.Data.SqlClient;
using System.Globalization;


namespace MvcApplication2.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private UsersContext db = new UsersContext();
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
            
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                // TESTING sql query to add garbage user into UserProfile table
                /* SqlConnection conn=null;
                 try
                 {
                     conn = new SqlConnection("server=yunyunzai;database=PIRADatabase;Integrated Security=SSPI;");
                    
                     SqlCommand cmd = new SqlCommand();
                     cmd.CommandText = "SET IDENTITY_INSERT UserProfile ON ";
                     cmd.Connection = conn;
                     conn.Open();
                     cmd.Prepare();
                     cmd.ExecuteNonQuery();

                     cmd = new SqlCommand();
                     cmd.CommandText = "insert into UserProfile(UserId,UserName,IsActive) values(@UserId,@UserName,@IsActive)";
                     SqlParameter id = new SqlParameter();
                     id.SqlDbType = SqlDbType.Int;
                     id.ParameterName = "@UserId";
                     id.Value = 67;
                     SqlParameter name = new SqlParameter();
                     name.SqlDbType = SqlDbType.NVarChar;
                     name.ParameterName = "@UserName";
                     name.Size = 56;
                     name.Value = "lololol";
                     SqlParameter isActive = new SqlParameter();
                     isActive.SqlDbType = SqlDbType.Bit;
                     isActive.ParameterName = "@IsActive";
                     isActive.Value = true;

                     cmd.Parameters.Add(id);
                     cmd.Parameters.Add(name);
                     cmd.Parameters.Add(isActive);
                     cmd.Connection = conn;
                     //conn.Open();
                     cmd.Prepare();
                     cmd.ExecuteNonQuery();

                     cmd = new SqlCommand();
                     cmd.CommandText = "SET IDENTITY_INSERT UserProfile OFF ";
                     cmd.Connection = conn;
                     cmd.Prepare();
                     cmd.ExecuteNonQuery();
                 }
                 finally
                 {
                     if (conn != null)
                     {
                         conn.Close();
                     }
                 }*/
                // TESTING CODE to get logged in user name
                MembershipUser user = Membership.GetUser(model.UserName);
                if (user == null)
                {
                    throw new InvalidOperationException("User [" +
                        User.Identity.Name + " ] not found.");
                }
                System.Diagnostics.Debug.WriteLine("Logged in user name: " + user.UserName);
                int userID = int.Parse(user.ProviderUserKey.ToString());
                if (!CheckUserActivation(userID))
                {
                    ModelState.AddModelError("", "User " + userID + " is no longer activated");
                    return View(model);
                }
                if (user.LastPasswordChangedDate.AddDays(42) < DateTime.Now)
                {
                    Server.Transfer("~/Account/_ChangePasswordPartial");
                }
                else
                {
                    UsersContext uc = new UsersContext();
                    var info = from table1 in uc.UserProfiles
                               join table2 in uc.UsersInRoles on table1.UserId equals table2.UserId
                               join table3 in uc.Roles on table2.RoleId equals table3.RoleId
                               select new { table1.UserId, table2.RoleId, table3.RoleName };

                    var roleAdmin = new List<String>();
                    var roleDI = new List<String>();
                    var roleRep = new List<String>();
                    foreach (var i in info)
                    {
                        if (i.UserId == userID)
                        {
                            if (i.RoleName == "Admin")
                                roleAdmin.Add(i.RoleName);
                            if (i.RoleName == "DISpecialist")
                                roleDI.Add(i.RoleName);
                            if (i.RoleName == "Reporter")
                                roleRep.Add(i.RoleName);
                        }
                    }
                    if (roleAdmin.Contains("Admin"))
                        return RedirectToAction("Admin", "Admin");
                    if (!roleAdmin.Contains("Admin") && roleDI.Contains("DISpecialist"))
                        return RedirectToAction("DISpecialist", "DISpecialist");
                    if (!roleAdmin.Contains("Admin") && !roleDI.Contains("DISpecialist") && roleRep.Contains("Reporter"))
                        return RedirectToAction("Reporter", "Reporter");
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [Role(Roles="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterModel rm = new RegisterModel();
            ListAllRoles();
            ListAllGroups();
            return View(rm);
        }




        [Authorize]
        public ActionResult InvalidAccess()
        {


            return View();
        }








        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, FormCollection collection, string [] selectedRoles, string [] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try

                {
                   
                    // @@@ SETTING initial isactive to true: Activate the account upon register
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { IsActivated = true , Email = model.Email, FirstName = model.FirstName,
                    LastName = model.LastName});
                    //Calls helper methods which manually update the relevant tables using navigation properties
                    UpdateUserRoles(selectedRoles, WebSecurity.GetUserId(model.UserName));
                    UpdateUserGroups(selectedGroups, WebSecurity.GetUserId(model.UserName));
                    //No need to log in on registration.
                   // WebSecurity.Login(model.UserName, model.Password);
                  
                  
                    return RedirectToAction("Index", "Users");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void UpdateUserRoles(string[] selectedRoles, int id)
        {
            var userprofile = db.UserProfiles
                .Include(i => i.UsersInRoles)
                .Where(i=>i.UserId == id)
                .Single();

            if (selectedRoles == null)
            {
                userprofile.UsersInRoles = new List<UsersInRoles>();
            }
            else
            {
                var roleQuery = db.Roles;
                var viewModelRole = new List<ViewModels.Roles>();
                foreach (var role in roleQuery)
                {
                    viewModelRole.Add(new ViewModels.Roles
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        IsSuperUser = role.IsSuperUser,
                        IsAssigned = false
                    });
                }

                var sel_roles = new HashSet<string>(selectedRoles);
                //    var userRoles = new HashSet<int>(userprofile.UsersInRoles.Select(c => c.RoleId));


                //if caller is from User/Create
                foreach (var role in viewModelRole)
                {
                    if (sel_roles.Contains(role.RoleId.ToString()))
                    {
                        UsersInRoles uir = new UsersInRoles
                        {
                            UserId = userprofile.UserId,
                            RoleId = role.RoleId
                        };
                        db.UsersInRoles.Add(uir);
                        db.SaveChanges();
                    }
                }


            }
        }
                

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
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
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
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
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }


        private bool CheckUserActivation(int id)
        {
            var userprofile = db.UserProfiles
                .Single(i => i.UserId == id);

           bool? actStatus = userprofile.IsActivated;
           if (actStatus == true)
           {
               
               return false;
           }
           else return true;
        }
        private void UpdateUserGroups(string[] selectedGroups, int id)
        {
            var userprofile = db.UserProfiles
                .Include(i => i.UserBelongsGroup)
                .Where(i => i.UserId == id)
                .Single();
            if (selectedGroups == null)
            {
                userprofile.UserBelongsGroup = new List<UserBelongsGroup>();
            }
            else
            {
                var groupQuery = db.UserGroup;
                var viewModelGroup = new List<ViewModels.UserGroup>();
                foreach (var group in groupQuery)
                {
                    viewModelGroup.Add(new ViewModels.UserGroup
                    {
                        Name = group.Name,
                        Abbreviate = group.Abbreviate,
                        // IsAssigned = false
                    });
                }

                var sel_groups = new HashSet<string>(selectedGroups);
                //   var usergroups = new HashSet<string>(userprofile.UserBelongsGroup.Select(c => c.Abbreviate));
                foreach (var g in viewModelGroup)
                {
                    if (sel_groups.Contains(g.Abbreviate.ToString()))
                    {
                        UserBelongsGroup ubg = new UserBelongsGroup
                        {
                            UserId = id,
                            Abbreviate = g.Abbreviate
                        };
                        db.UserBelongsGroup.Add(ubg);
                        db.SaveChanges();
                    }

                }
            }


        }


        private void ListAllRoles()
        {
            var roleQuery = db.Roles;
            var viewModel = new List<MvcApplication2.ViewModels.Roles>();
            foreach (var role in roleQuery)
            {
                viewModel.Add(new ViewModels.Roles
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    IsSuperUser = role.IsSuperUser,
                    IsAssigned = false

                });
            }

            ViewBag.ViewModelRoles = viewModel;
        }

        private void ListAllGroups()
        {
            var groupQuery = db.UserGroup;
            var viewModel = new List<MvcApplication2.ViewModels.UserGroup>();
            foreach (var group in groupQuery)
            {
                viewModel.Add(new ViewModels.UserGroup
                {
                    Abbreviate = group.Abbreviate,
                    Name = group.Name


                });
            }
            ViewBag.ViewModelGroups = viewModel;

        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
