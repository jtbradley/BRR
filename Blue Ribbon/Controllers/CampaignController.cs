using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using Blue_Ribbon.AmazonAPI;
using System.Collections.Generic;
using Blue_Ribbon.ViewModels;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Text;
using System.Data.Entity.Infrastructure;
using Blue_Ribbon.ScheduledTasks;

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager")]
    public class CampaignController : Controller
    {

        private BRContext db = new BRContext();


        public ActionResult AdminDashboard()
        {
            AdminDashboardViewModel dash = new AdminDashboardViewModel();

            return View(dash);
        }

        public ActionResult Index(string sortOrder, string active, string currentSearch = null)
        {

            ViewBag.Requests = sortOrder == "requests_desc" ? "requests_asc" : "requests_desc";
            ViewBag.ReviewsNeeded = sortOrder == "revs_needed" ? "revs_needed_desc" : "revs_needed";
            ViewBag.Vendor = sortOrder == "Vendor" ? "vendor_desc" : "Vendor";
            ViewBag.Codes = sortOrder == "codes" ? "codes_desc" : "codes";
            ViewBag.SearchBox = currentSearch;

            if (String.IsNullOrEmpty(sortOrder)) { sortOrder = "requests_desc"; }
            if (String.IsNullOrEmpty(active)) { active = "all"; }

            CampaignIndexViewModel IndexView = new CampaignIndexViewModel(sortOrder, currentSearch, active);

            var reviewsCheckedTime = (from t in db.TaskLog
                                      where t.TaskDescription.Equals("DailyLimitsReset")
                                      orderby t.SuccessDatestamp descending
                                      select t.SuccessDatestamp).First();

            ViewBag.DateStamp = reviewsCheckedTime;



            return View(IndexView);
        }

        public ActionResult Notifications()
        {
            var notifications = (from n in db.NotificationLog
                                 orderby n.LogTimestamp descending
                                 select n).Take(50);

            return View(notifications.ToList());
        }

        // GET: Campaign/Details/5
        public ActionResult Details(int id)
        {

            var campaign = db.Campaigns.Find(id);

            if (campaign == null)
            {
                return HttpNotFound();
            }

            //CampaignDetailsViewModel displaycampaign = new CampaignDetailsViewModel(campaign);
            CampaignDetailsPageViewModel campaignsummary = new CampaignDetailsPageViewModel(campaign);

            return View(campaignsummary);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public ActionResult Open(int id)
        {
            Campaign campaigns = db.Campaigns.Find(id);
            if (campaigns == null)
            {
                return HttpNotFound();
            }

            Campaign campaign = db.Campaigns.Find(id);
            campaign.OpenCampaign = true;
            campaign.CloseDate = null;
            db.SaveChanges();
            return RedirectToAction("Details", id);
        }

        // GET: Campaign/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string asin)
        {
            if (asin == null)
            {
                return HttpNotFound();
            }

            string[] ASIN = new string[] { asin };
            ReadResponse processor = new ReadResponse();
            Campaign campaign = new Campaign();
            processor.Populate(campaign, ASIN);

            IOrderedQueryable<Vendor> vendors = db.Vendors
            .OrderBy(i => i.Name);
            ViewBag.VendorId = new SelectList(vendors, "VendorId", "Name", campaign.VendorID);

            return View("Validate", campaign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Validated(Campaign campaign, string discountcodes)
        {

            if (ModelState.IsValid)
            {
                campaign.SetNumericalPrices();
                db.Campaigns.Add(campaign);
                db.SaveChanges();

                int id = campaign.CampaignID;

                List<string> codes = discountcodes.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
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


                return RedirectToAction("Index");
            }

            return View("Validate", campaign);
        }

        // GET: Campaign/Edit/5
        public ActionResult Edit(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ReadResponse processor = new ReadResponse();
            campaign = processor.Update(campaign);


            IOrderedQueryable<Vendor> vendors = db.Vendors
            .OrderBy(i => i.Name);
            ViewBag.VendorId = new SelectList(vendors, "VendorId", "Name", campaign.VendorID);

            var codes = from c in db.DiscountCodes
                        where c.CampaignID.Equals(id)
                        select c.Code;

            StringBuilder codelist = new StringBuilder();
            foreach (var p in codes)
            {
                codelist.Append(p);
                codelist.Append("\r\n");
            }
            ViewBag.DiscountCodes = codelist.ToString();


            return View(campaign);
        }

        // POST: Campaign/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Campaign campaign, string discountcodes)
        {
            if (ModelState.IsValid)
            {
                campaign.SetNumericalPrices();
                db.Entry(campaign).State = EntityState.Modified;

                //remove all discount codes in db first, then we'll re-add the ones 
                //submitted on edit page
                var remove = from r in db.DiscountCodes
                             where r.CampaignID.Equals(campaign.CampaignID)
                             select r;

                if (remove != null)
                {
                    db.DiscountCodes.RemoveRange(remove);
                    db.SaveChanges();
                }

                List<string> codes = discountcodes.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                foreach (var item in codes)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        DiscountCode itemcode = new DiscountCode();
                        itemcode.Code = item;
                        itemcode.CampaignID = campaign.CampaignID;
                        db.DiscountCodes.Add(itemcode);

                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Index", campaign);
        }

        // GET: Campaign/Delete/5
        public ActionResult Close(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            campaign.OpenCampaign = false;
            campaign.CloseDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PricingStructure()
        {
            PricingStructure pricing = (from p in db.PricingStructure
                                        select p).First();

            return View(pricing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PricingStructure(PricingStructure pricing)
        {
            db.Entry(pricing).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.Message = "Update Successful!";
            return View(pricing);
        }



        //This will check ALL open reviews to see if they have been completed.
        //Need to have this run at like 2 am or something because it can take a while to complete depending on 
        //number of open reviews. There are no links to this...URL entered directly in bar will activate it.
        public ActionResult CheckAllReviews()
        {
            CheckForCompletedReviews checkall = new CheckForCompletedReviews();
            checkall.Check();
            TaskLog logItem = new TaskLog();
            logItem.SuccessDatestamp = DateTime.Now;
            logItem.TaskDescription = "AllReviewsChecked";
            db.TaskLog.Add(logItem);
            db.SaveChanges();

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


        [HttpPost]
        public JsonResult RequestAction(string requestid, string accept)
        {
            bool accepted = accept == "accept" ? true : false;
            List<string> data = new List<string>();
            data.Add("requestid=" + requestid);
            data.Add("accept=" + accept);
            int id = int.Parse(requestid);
            ItemRequest request = (from ItemRequest c in db.ItemRequests
                                   where c.ItemRequestID == id
                                   select c).First();

            request.ActiveRequest = false;
            request.Selected = accepted;
            db.SaveChanges();

            string message = "Denied";

            if (accepted)
            {
                Review requestconvert = new Review();
                requestconvert.CampaignID = request.CampaignID;
                requestconvert.CustomerID = request.CustomerID;
                requestconvert.ReviewTypeExpected = request.ReviewType;
                requestconvert.SelectedDate = DateTime.Now;
                db.Reviews.Add(requestconvert);
                message = "Selected";

                db.SaveChanges();

                int cr = (from c in db.DiscountCodes
                          where c.CampaignID.Equals(request.CampaignID)
                          select c).Count();

                if (cr > 0)
                {
                    var code = (from DiscountCode dc in db.DiscountCodes
                                where dc.CampaignID.Equals(request.CampaignID)
                                select dc).First();

                    requestconvert.DiscountCode = code.Code;
                    db.DiscountCodes.Remove(code);
                    db.SaveChanges();

                    SendDiscountViewModel codeemail = new SendDiscountViewModel(requestconvert);
                    codeemail.SendCode();
                    message = "codesent";

                }
            }


            return Json(new { data, result = message });
        }


        public JsonResult ApplyCode(string reviewid, string code)
        {

            List<string> data = new List<string>();
            data.Add("id=" + reviewid);
            data.Add("code=" + code);
            int id = int.Parse(reviewid);
            Review review = (from Review c in db.Reviews
                             where c.ReviewID == id
                             select c).First();
            review.DiscountCode = code;
            db.SaveChanges();
            SendDiscountViewModel codeemail = new SendDiscountViewModel(review);
            codeemail.SendCode();

            string message = "success";

            return Json(new { data, result = message });
        }

    }
}
