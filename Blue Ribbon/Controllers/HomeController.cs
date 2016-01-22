using Blue_Ribbon.AmazonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.Models;
using Blue_Ribbon.DAL;
using Blue_Ribbon.ViewModels;
using System.Threading.Tasks;

namespace Blue_Ribbon.Controllers
{
    public class HomeController : Controller
    {
        private BRContext db = new BRContext();

        public ActionResult Index()
        {
            var campaigns = (from s in db.Campaigns
                            where s.OpenCampaign == true
                            where s.DailyLimitReached == false
                            orderby s.CalculatedDiscount descending
                            select s).Take(3);

            return View(campaigns.ToList());
        }


        public ActionResult Contact()
        {
            GeneralMessageViewModel contact = new GeneralMessageViewModel();

            return View(contact);
        }
        public ActionResult FAQs()
        {
            
            return View();
        }
        
        public ActionResult Review()
        {
            //Example frame is pulling products with no reviews on Amazon
            //Manually set ASIN for product with good reviews.
            //Campaign campaign = (from Campaign s in db.Campaigns
            //                      where s.OpenCampaign == true
            //                      orderby s.SalePriceNumerical
            //                     select s).First();

            //string[] ASIN = new string[] { campaign.ASIN };

            string[] ASIN = new string[] { "B002M782UO" };
            LookupByASIN itemExample = new LookupByASIN(ASIN);
            string reviewExampleFrame = itemExample.ReviewsFrame();
            ViewBag.Frame = reviewExampleFrame;
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult TermsOfService()
        {
            return View();
        }

        public ActionResult DMCA()
        {
            return View();
        }
        public ActionResult Seller()
        {
            SellerFormViewModel contact = new SellerFormViewModel();
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SellerForm(SellerFormViewModel newmessage)
        {
            var response = newmessage.SendForm();

            var data = new
            {
                message = "Message Sent!"
            };

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GeneralMessageForm(GeneralMessageViewModel newmessage)
        {
            var response = newmessage.SendForm();

            var data = new
            {
                message = "Message Sent!"
            };

            return Json(data);
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