using Blue_Ribbon.AmazonAPI;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using Blue_Ribbon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager, seller")]
    public class SellerController : Controller
    {
        private BRContext db = new BRContext();

        [AllowAnonymous]
        // GET: Seller
        public ActionResult Index(string message)
        {
            PricingStructure pricing = (from p in db.PricingStructure
                                        select p).First();

            return View(pricing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(string ASIN, string textReviews, string photoReviews, string videoReviews)
        {
            string[] asin = new string[] { ASIN };
            ReadResponse processor = new ReadResponse();
            Campaign campaign = new Campaign();
            processor.Populate(campaign, asin);

            var exists = (from v in db.Vendors
                          where v.CustomerID.Equals(User.Identity.Name)
                          select v).First();

            campaign.VendorID = exists.VendorId;

            if (String.IsNullOrEmpty(textReviews)) { textReviews = "0"; }
            if (String.IsNullOrEmpty(photoReviews)) { photoReviews = "0"; }
            if (String.IsNullOrEmpty(videoReviews)) { videoReviews = "0"; }

            campaign.TextGoal = int.Parse(textReviews);
            campaign.PhotoGoal = int.Parse(photoReviews);
            campaign.VideoGoal = int.Parse(videoReviews);


            return View(campaign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewOrder(Campaign campaign, string discountcodes)
        {

            ReviewOrderViewModel reviewOrder = new ReviewOrderViewModel();
            reviewOrder.Campaign = campaign;
            reviewOrder.TotalReviews = campaign.TextGoal + campaign.PhotoGoal + campaign.VideoGoal;
            reviewOrder.PaypalComment = String.Format("{0} High-Quality Reviews for ASIN: {1} - {2}", reviewOrder.TotalReviews, campaign.ASIN, campaign.Name);
            reviewOrder.DiscountCodes = discountcodes;

            PricingStructure pricing = (from p in db.PricingStructure
                                        select p).First();


            //Note: We're only using textReviews right now, so photo and video will always be zero. However, 
            // We want to take the number of text reviews and make 20% of them photovideo.
            if (campaign.TextGoal > 9)
            {
                double photos = campaign.TextGoal * .15;
                double videos = campaign.TextGoal * .05;

                campaign.PhotoGoal = Convert.ToInt32(Math.Round(photos));
                campaign.VideoGoal = Convert.ToInt32(Math.Round(videos));
                campaign.TextGoal = campaign.TextGoal - campaign.PhotoGoal - campaign.VideoGoal;


            }
            int textcount = campaign.TextGoal;
            int photocount = campaign.PhotoGoal;
            int videocount = campaign.VideoGoal;

            double textPrice = 0.00;
            double photoPrice = 0.00;
            double videoPrice = 0.00;

            //Note: if we were to implement Photo and Video pricing it would be here. 
            if (reviewOrder.TotalReviews > 100) {
                textPrice = reviewOrder.TotalReviews * pricing.T3;
            } else if (reviewOrder.TotalReviews > 50 && reviewOrder.TotalReviews < 101)
            {
                textPrice = reviewOrder.TotalReviews * pricing.T2;
            } else
            {
                textPrice = reviewOrder.TotalReviews * pricing.T1;
            }



            double totalPrice = (textPrice + photoPrice + videoPrice);


            reviewOrder.TotalPrice = String.Format("${0:0.00}", totalPrice);
            reviewOrder.PaypalURL = reviewOrder.SetupPayment();

            //Seller is going to leave site and go to Paypal to pay. We need to save the details so we can 
            //implement them after payment is confirmed. 
            Session.Add("NewCampaign", reviewOrder);

            return View(reviewOrder);
        }

        public ActionResult ProcessPayment(string token, string PayerID)
        {

            ReviewOrderViewModel newcampaign = Session["NewCampaign"] as ReviewOrderViewModel;
            string result = newcampaign.ProcessPayment(token, PayerID);

            if (result == "success")
            {
                if (ModelState.IsValid)
                {
                    newcampaign.Campaign.SetNumericalPrices();
                    db.Campaigns.Add(newcampaign.Campaign);
                    db.SaveChanges();

                    NotificationLog sellerCreatedCampaign = new NotificationLog();
                    sellerCreatedCampaign.CampaignID = newcampaign.Campaign.CampaignID;
                    sellerCreatedCampaign.LogTimestamp = DateTime.Now;
                    sellerCreatedCampaign.Message = "A seller used the automated tool to create a new campaign!";
                    db.NotificationLog.Add(sellerCreatedCampaign);


                    int id = newcampaign.Campaign.CampaignID;

                    List<string> codes = newcampaign.DiscountCodes.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                    foreach (var item in codes)
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            DiscountCode itemcode = new DiscountCode();
                            itemcode.Code = item;
                            itemcode.CampaignID = id;
                            db.DiscountCodes.Add(itemcode);

                        }
                    }
                    db.SaveChanges();
                }
                return View("Success", newcampaign);
            }

            return View("Index", "Seller");
        }

        public ActionResult CreateSeller()
        {
            string userId = User.Identity.Name;

            Customer selectedCust = (from cust in db.Customers
                                     where cust.CustomerID.Equals(userId)
                                     select cust).First();

            Vendor newVendor = new Vendor();
            newVendor.ContactName = selectedCust.FullName;
            newVendor.Email = selectedCust.Email;

            return View(newVendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSeller(Vendor vendor)
        {
            bool exists = (from v in db.Vendors
                           where v.CustomerID.Equals(User.Identity.Name)
                           select v).Any();

            if (exists)
            {
                return RedirectToAction("Index","Seller", new { message = "Already a Seller!" });
            }

            vendor.CustomerID = User.Identity.Name;

            if (ModelState.IsValid)
            {
                vendor.JoinDate = DateTime.Now;
                db.Vendors.Add(vendor);
                db.SaveChanges();
                return RedirectToAction("Index","Seller",new { message = "Seller Created!" });
            }

            return View(vendor);
        }


    }
}