using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using Blue_Ribbon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blue_Ribbon.Controllers
{
    public class ProductController : Controller
    {
        private BRContext db = new BRContext();

        public ActionResult Index(string productsearch, string sortOrder = "ageAsc")
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.ProductSearch = productsearch;
            var campaigns = from s in db.Campaigns
                            where s.OpenCampaign == true
                            select s;

            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(productsearch))
            {
                campaigns = campaigns.Where(s => s.Name.ToUpper().Contains(productsearch.ToUpper()) ||
                s.Category.ToUpper().Contains(productsearch.ToUpper()) || s.Description.ToUpper().Contains(productsearch.ToUpper()));
            }

            switch (sortOrder)
            {
                case "ageDesc":
                    campaigns = campaigns.OrderBy(s => s.StartDate);
                    break;

                case "ageAsc":
                    campaigns = campaigns.OrderByDescending(s => s.StartDate);
                    break;

                case "priceAsc":
                    campaigns = campaigns.OrderBy(s => s.SalePriceNumerical);
                    break;

                case "priceDesc":
                    campaigns = campaigns.OrderByDescending(s => s.SalePriceNumerical);
                    break;

                case "categoryAsc":
                    campaigns = campaigns.OrderBy(s => s.Category);
                    break;

                case "categoryDesc":
                    campaigns = campaigns.OrderByDescending(s => s.Category);
                    break;
            }

            return View(campaigns.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SubmitItemRequest(int campaignid, string userid, string reviewtype, string asin)
        {
            //An error is occuring that some products to not generate the id attribute and the
            //var reviewtype = "#" + asin + "reviewtype" in scripts cannot get reviewtype value.
            //I think this is happening for ASIN products that are all numerical (books). 
            //Temp fix: make reviewtype default to text.
            if (reviewtype == "undefined") { reviewtype = "Text"; }


            string thisCustomer = User.Identity.Name;
            
            //First check if customer has already received or requested this deal, fail immediately if so.
            bool alreadyrequested = db.ItemRequests.Any(i => i.Campaign.ASIN.Equals(asin) && i.CustomerID.Equals(thisCustomer));
            bool alreadyselected = db.Reviews.Any(j => j.Campaign.ASIN.Equals(asin) && j.CustomerID.Equals(thisCustomer));
            if (alreadyrequested || alreadyselected)
            {
                return Json(new { result = "Duplicate" });
            }

            //If customer hasn't requested yet, then check how many open reviews they  have
            int completedReviewsByCustomer = (from c in db.Reviews
                                              where c.CustomerID.Equals(thisCustomer)
                                              where c.Reviewed.Equals(true)
                                              select c).Count();

            int openReviewsByCustomer = (from c in db.Reviews
                                         where c.CustomerID.Equals(thisCustomer)
                                         where c.Reviewed.Equals(false)
                                         select c).Count();

            //If customer has completed three review for BlueRibbons, then they are allowed to have three open reviews
            //at any given moment. If they are new user or less than three completed reviews, they're limited to two items.
            if(completedReviewsByCustomer >=3)
            {
                if (openReviewsByCustomer >= 3)
                {
                    return Json(new { result = "Exceed3" });
                }
            } else
            {
                if (openReviewsByCustomer >= 2)
                {
                    return Json(new { result = "Exceed1" });
                }
            }

            //If customer has overdue reviews, then they're not eligible for more deals.
            var customersOpenReviews = (from c in db.Reviews
                                        where c.CustomerID.Equals(thisCustomer)
                                        where c.Reviewed.Equals(false)
                                        select c).ToList();

            foreach(var rev in customersOpenReviews)
            {
                if(rev.DueDate.Date < DateTime.Now.Date)
                {
                    return Json(new { result = "Overdue" });
                }
            }


            //If customer made it this far then we'll log their request.
            ItemRequest newrequest = new ItemRequest();
            newrequest.CampaignID = campaignid;
            newrequest.CustomerID = thisCustomer;
            newrequest.ReviewType = (ReviewType)Enum.Parse(typeof(ReviewType), reviewtype);
            db.ItemRequests.Add(newrequest);
            db.SaveChanges();

            string message = "OK";

            Campaign campaign = (from Campaign c in db.Campaigns
                                 where c.CampaignID.Equals(campaignid)
                                 select c).First();

            // Any campaign with a daily limit of 0 will only log requests and NOT auto
            //respond per alex and Amber, so we'll just stop here. 
            if (campaign.DailyLimit == 0)
            {
                return Json(new { result = message });
            }

            //With the customer's request logged, if there are available Discount Codes for the campaign, then we'll assign one automatically
            //and let customer get deal!
            int discountCodeCount = (from c in db.DiscountCodes
                      where c.CampaignID.Equals(newrequest.CampaignID)
                      select c).Count();

            if (discountCodeCount > 0)
            {
                var code = (from DiscountCode dc in db.DiscountCodes
                            where dc.CampaignID.Equals(newrequest.CampaignID)
                            select dc).First();

                //Close the item request.
                newrequest.ActiveRequest = false;
                newrequest.Selected = true;

                //Create a Review object based on item request.
                Review requestconvert = new Review();
                requestconvert.CampaignID = newrequest.CampaignID;
                requestconvert.CustomerID = newrequest.CustomerID;
                requestconvert.ReviewTypeExpected = newrequest.ReviewType;
                requestconvert.SelectedDate = DateTime.Now;
                requestconvert.DiscountCode = code.Code;
                db.Reviews.Add(requestconvert);

                db.DiscountCodes.Remove(code);
                db.SaveChanges();

                SendDiscountViewModel codeemail = new SendDiscountViewModel(requestconvert);
                codeemail.SendCode();
                object approved = new
                {
                    result = "Approved",
                    code = code.Code,
                    defaultUrl = campaign.AmazonUrl,
                    customUrl = campaign.VendorsPurchaseURL,
                    instructions = campaign.VendorsPurchaseInstructions
                };

                //After we've approved customer and applied discount code, check if discount codes are depleted.
                //If we used the last discount code, then retire campaign. Note: this will only close campaign
                //if we USED the last code. It won't affect campaigns with no pre-loaded codes. 
                if (discountCodeCount == 1)
                {
                    campaign.OpenCampaign = false;
                    campaign.CloseDate = DateTime.Now;
                    db.Entry(campaign).State = EntityState.Modified;

                    NotificationLog lastCodeLog = new NotificationLog();
                    lastCodeLog.CampaignID = campaign.CampaignID;
                    lastCodeLog.LogTimestamp = DateTime.Now;
                    lastCodeLog.Message = "Last discount code was distributed. Campaign has been Closed.";
                    db.NotificationLog.Add(lastCodeLog);

                }

                DateTime today = DateTime.Now.Date;

                //If we hit the daily deal limit, temporarily pause program. 
                //Secheduled task will reset daily counter the next morning.
                int todaysCount = (from c in db.Reviews
                                   where c.SelectedDate > today
                                   select c).Count();

                //Note, campaigns will a daily limit of 0 are managed manually. 
                if (todaysCount >= campaign.DailyLimit && campaign.DailyLimit > 0)
                {
                    campaign.DailyLimitReached = true;
                    db.Entry(campaign).State = EntityState.Modified;

                    NotificationLog limitReachedLog = new NotificationLog();
                    limitReachedLog.CampaignID = campaign.CampaignID;
                    limitReachedLog.LogTimestamp = DateTime.Now;
                    limitReachedLog.Message = "Daily limit for this campaign has been reached. Item availability will reset tomorrow.";
                    db.NotificationLog.Add(limitReachedLog);

                }

                //And we'll check the number of text reviews. If we've hit our text review goal, we'll disable text reviews
                //and force remaining review requests to be photo or video. 

                int textReviews = (from c in db.Reviews
                                   where c.CampaignID.Equals(campaign.CampaignID)
                                   where c.ReviewTypeExpected == 0
                                   select c).Count();

                if (textReviews >= campaign.TextGoal)
                {
                    campaign.TextValid = false;

                    NotificationLog textLimitReached = new NotificationLog();
                    textLimitReached.CampaignID = campaign.CampaignID;
                    textLimitReached.LogTimestamp = DateTime.Now;
                    textLimitReached.Message = "Text goal has been reached for this campaign. Text reviews have been disabled.";
                    db.NotificationLog.Add(textLimitReached);
                }

                db.SaveChanges();

                return Json(approved);
            }

            //No codes were available, so all we did was log the request for the admins. 
            return Json(new { result = message });

        }
    }
}