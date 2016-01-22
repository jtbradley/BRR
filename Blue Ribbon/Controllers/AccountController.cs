using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Blue_Ribbon.Models;
using Blue_Ribbon.DAL;
using System.Data.Entity;
using RestSharp;
using RestSharp.Authenticators;
using System.Web.Configuration;

namespace Blue_Ribbon.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private BRContext db = new BRContext();
        private ApplicationDbContext appcontext = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //customer logs in with email and password. But authentication is by AmazonID as username
            //this looks up amazonid and passes it as username for authentication. Returns
            //Invalid Login if email is not in db.
            string username;
            try
            {
                username = (from c in appcontext.Users
                            where c.Email.Equals(model.Email)
                            select c.UserName).Single().ToString();
            }
            catch
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, shouldLockout: false);

            // Authenticate user first, then check if confirmed email. Log back out if not confirmed.
            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(username);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ViewBag.Message = "You must have a confirmed email to log on. We just re-sent the confirmation link to " +
                                    "the email you signed-up with.";
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    var response = SendEmailConfirmation(user.Email, callbackUrl, false);
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return View("Login");
                }
            }


            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        ////Not allowing local registration. Must use OAuth.
        //// GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SellerRegister()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //Check to see if we can read their profile RSS feed. If not, then id is invalid. Return to page and ask again.
            try
            {
                ParsedFeed profilecheck = new ParsedFeed(model.CustomerID);
            }
            catch
            {
                ViewBag.Error = "There is a problem with that Amazon Profile ID. Please check and try again.";
                return View("Register", model);
            }

            if (db.Customers.Where(u => u.CustomerID == model.CustomerID).Any())
            {
                ViewBag.Error = "That Amazon Profile ID is already in use.";
                return View("Register", model);
            }


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.CustomerID, Email = model.Email, CustomerID = model.CustomerID };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //We will not sign in customer immediately. They have to confirm email.
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    Customer customer = new Customer();
                    customer.Email = model.Email;
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    customer.CustomerID = model.CustomerID;
                    customer.LastReviewCheck = DateTime.Now;
                    customer.JoinDate = DateTime.Now;
                    customer.Qualified = true;
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Email Confirmation
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    var response = SendEmailConfirmation(user.Email, callbackUrl, false);

                    // Uncomment to debug locally 
                    // TempData["ViewBagLink"] = callbackUrl;

                    if (model.BeSeller)
                    {
                        await UserManager.AddToRoleAsync(user.Id, "seller");
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("CreateSeller", "Seller");
                    }
                    else
                    {
                        //use this for TESTING ONLY
                        //await UserManager.AddToRoleAsync(user.Id, "campaignManager");

                        ////USE THIS FOR LIVE SITE
                        await UserManager.AddToRoleAsync(user.Id, "customer");
                    }

                    return RedirectToAction("Welcome", "Dashboard", new { message = "confirmmail" });

                    //return RedirectToAction("Welcome", "Dashboard");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username;
                try
                {
                    username = (from c in appcontext.Users
                                where c.Email.Equals(model.Email)
                                select c.UserName).Single().ToString();
                }
                catch
                {
                    return View("ForgotPasswordConfirmation");
                }

                var user = await UserManager.FindByNameAsync(username);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                if (user.EmailConfirmed == false)
                {
                    string emailcode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var emailcallbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = emailcode }, protocol: Request.Url.Scheme);
                    var response = SendEmailConfirmation(user.Email, emailcallbackurl, true);
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
             

                //Assemble message first.
                var body = "<h2>Reset your password for Blue Ribbons Review!</h2>" +
                    "<p><strong>Use the following link to reset your password:</strong></p>" +
                    "<p><a href='" + callbackUrl + "'>Reset Password for Blue Ribbons Review!</p>";

                //SendMessage
                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator =
                        new HttpBasicAuthenticator("api",
                                                   WebConfigurationManager.AppSettings["mailSecretKey"]);
                RestRequest request = new RestRequest();
                request.AddParameter("domain",
                                     "blueribbonsreview.com", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";

                //Who will be displayed as sender, put info here!
                request.AddParameter("from", "Blue Ribbons Review <do-not-reply@blueribbonsreview.com>");

                //Who will be receiving messages, put email here.
                request.AddParameter("to", model.Email);

                request.AddParameter("subject", "Blue Ribbons Review - Password Reset");
                request.AddParameter("html", body);
                request.Method = Method.POST;
                var result = client.Execute(request);



                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username;
            try
            {
                username = (from c in appcontext.Users
                            where c.Email.Equals(model.Email)
                            select c.UserName).Single().ToString();
            }
            catch
            {
                return View("ForgotPasswordConfirmation");
            }

            var user = await UserManager.FindByNameAsync(username);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
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
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
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


            //Using social media login, lookup local site user account.
            //Check if email has been confirmed and redirect to login page if not.
            ApplicationUser socialuser = await UserManager.FindAsync(loginInfo.Login);
            if (socialuser != null)
            {
                if (socialuser.EmailConfirmed == false)
                {
                    ViewBag.SocialMessage = "You must have a confirmed email to log on. We just re-sent the confirmation link to " +
                        "the email you signed-up with.";

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(socialuser.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = socialuser.Id, code = code }, protocol: Request.Url.Scheme);
                    var response = SendEmailConfirmation(socialuser.Email, callbackUrl, false);

                    return View("Login");
                }
            }

            // Sign in the user with this external login provider if the user already has a login
            //This will only be reached if email is confirmed.
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);            

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
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
                 return RedirectToAction("Index", "Dashboard");
            }
            //Check to see if we can read their profile RSS feed. If not, then id is invalid. Return to page and ask again.
            try
            {
                ParsedFeed profilecheck = new ParsedFeed(model.CustomerID);
            }
            catch
            {
                ViewBag.Error = "There is a problem with that Amazon Profile ID. Please check and try again.";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.CustomerID, Email = model.Email, CustomerID = model.CustomerID };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user.Id, "customer");

                        //We're not logging in customer until they verify email
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        Customer customer = new Customer();
                        customer.Email = model.Email;
                        customer.FirstName = model.FirstName;
                        customer.LastName = model.LastName;
                        customer.CustomerID = model.CustomerID;
                        customer.LastReviewCheck = DateTime.Now;
                        customer.JoinDate = DateTime.Now;
                        customer.Qualified = true;
                        db.Customers.Add(customer);
                        db.SaveChanges();

                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                        var response = SendEmailConfirmation(user.Email, callbackUrl, false);

                        // Uncomment to debug locally 
                        // TempData["ViewBagLink"] = callbackUrl;



                        return RedirectToAction("Welcome", "Dashboard", new { message = "confirmmail" });

                        //return RedirectToAction("Welcome", "Dashboard");

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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult ModifyPermissions(string customerID)
        {
            UserRoleViewModel permissions = new UserRoleViewModel();
            permissions.RoleChoices = appcontext.Roles.Select(m => new SelectListItem
            {
                Value = m.Id,
                Text = m.Name
            });


            var cust = (from customer in db.Customers
                        where customer.CustomerID == customerID
                        select customer).First();
            permissions.AmazonID = cust.CustomerID;
            permissions.FirstName = cust.FirstName;
            permissions.Lastname = cust.LastName;
            permissions.Email = cust.Email;

            var user = (from a in appcontext.Users
                        where a.CustomerID == customerID
                        select a).First();
            permissions.SelectedRoleIds = user.Roles.Select(m => m.RoleId).ToList();
            return View(permissions);
        }
        [HttpPost]
        public async Task<ActionResult> ModifyPermissions(string AmazonID, string SelectedRoleIds)
        {

            ApplicationUser user = (from a in appcontext.Users
                                    where a.CustomerID == AmazonID
                                    select a).First();

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            foreach(var u in userRoles)
            {
                await UserManager.RemoveFromRoleAsync(user.Id, u);
            }

            if(SelectedRoleIds == "1")
            {
                await UserManager.AddToRoleAsync(user.Id, "campaignManager");
            }

            if (SelectedRoleIds == "2")
            {
                await UserManager.AddToRoleAsync(user.Id, "customer");
            }

            if (SelectedRoleIds == "3")
            {
                await UserManager.AddToRoleAsync(user.Id, "seller");
            }

            return RedirectToAction("Index", "Customer");
        }


        public bool EditAmazonID(string oldID, string newID)
        {
            bool success = false;
            ApplicationUser user = (from a in appcontext.Users
                                    where a.CustomerID == oldID
                                    select a).First();

            user.CustomerID = newID;
            appcontext.SaveChanges();
            success = true;
            return success;
        }

        public bool EditEmail(string newemail, string userid)
        {
            bool success = false;
            ApplicationUser user = (from a in appcontext.Users
                                    where a.CustomerID == userid
                                    select a).First();
            user.Email = newemail;
            appcontext.Entry(user).State = EntityState.Modified;
            appcontext.SaveChanges();
            success = true;
            return success;
        }

        public IRestResponse SendEmailConfirmation(string email, string url, bool confirmFirst)
        {

            string body;

            if (confirmFirst)
            {
                body = "<h2>Please confirm your email first.</h2>" +
                    "<p><strong>You attempted to reset/change your password, but we need you to confirm "+
                    "your email first with the following link. You can reset your password after your email has been "+
                    "confirmed by.</strong></p>" +
                    "<p><a href='" + url + "'>Confirm and Activate Account Now!</p>";

            }
            else
            {
                body = "<h2>Thank you for signing up with Blue Ribbons Review!</h2>" +
                    "<p><strong>Please confirm your account with the following link</strong></p>" +
                    "<p><a href='" + url + "'>Confirm and Activate Account Now!</p>";
            }


            //SendMessage
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               WebConfigurationManager.AppSettings["mailSecretKey"]);
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "blueribbonsreview.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            //Who will be displayed as sender, put info here!
            request.AddParameter("from", "Blue Ribbons Review <do-not-reply@blueribbonsreview.com>");

            //Who will be receiving messages, put email here.
            request.AddParameter("to", email);

            request.AddParameter("subject", "Blue Ribbons Review - Please confirm your email.");
            request.AddParameter("html", body);
            request.Method = Method.POST;
            return client.Execute(request);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
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