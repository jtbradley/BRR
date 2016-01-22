using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Blue_Ribbon.Models;
using Blue_Ribbon.DAL;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using Blue_Ribbon.AmazonAPI;

namespace Blue_Ribbon.Controllers
{

    public class DashboardController : Controller
    {
        ApplicationDbContext dba = new ApplicationDbContext();
        private BRContext db = new BRContext();

        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.Name;

            Customer selectedCust = (from cust in db.Customers
                                     where cust.CustomerID.Equals(userId)
                                     select cust).First();

            CheckForCompletedReviews check = new CheckForCompletedReviews(selectedCust);
            check.Check();
            selectedCust.LastReviewCheck = DateTime.Now;
            db.Entry(selectedCust).State = EntityState.Modified;
            db.SaveChanges();


            return View(selectedCust);
        }

        public ActionResult Carousel()
        {
            
            List<Campaign> products = (from camp in db.Campaigns
                                       where camp.OpenCampaign == true
                                       select camp).ToList();

            return PartialView(products);
        }

        public ActionResult GetFormPartial(string email, string name)
        {
            ContactFormViewModel newMessage = new ContactFormViewModel();
            newMessage.Email = email;
            newMessage.Name = name;
            newMessage.LoggedIn = User.Identity.IsAuthenticated;
            newMessage.AmazonID = User.Identity.Name;

            return PartialView("_ContactFormPartial",newMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ContactForm(ContactFormViewModel newmessage)
        {
            var response = newmessage.SendForm();

            var data = new
            {
                message = "Message Sent!"
            };

            return Json(data);
        }

        public ActionResult Welcome(string message)
        {
            if(message == "confirmmail")
            {
                ViewBag.Message = "Please check your email and confirm your account. You must be confirmed "
                                + "before you can log in start shopping!";
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}