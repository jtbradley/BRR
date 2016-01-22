using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using Blue_Ribbon.ViewModels;

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager")]
    public class ReviewController : Controller
    {
        private BRContext db = new BRContext();

        // GET: All OPEN reviews. If a review is closed, we don't need to modify it.
        public ActionResult Index(bool showAll = false, string currentSearch = null)
        {
            var reviews = from r in db.Reviews
                          where r.Reviewed.Equals(false)
                         select r;

            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(currentSearch))
            {
                reviews = reviews.Where(s => (s.Customer.FirstName.ToUpper() + " " + s.Customer.LastName.ToUpper()).Contains(currentSearch.ToUpper()) ||
                s.Customer.Email.ToUpper().Contains(currentSearch.ToUpper()) || s.CustomerID.ToUpper().Contains(currentSearch.ToUpper()) ||
                s.Campaign.Name.ToUpper().Contains(currentSearch.ToUpper()));
            }

            //Sort: reviews with issues first, then by oldest oustanding review sfirst. 
            reviews = reviews.OrderByDescending(x => x.CustomerAlert).ThenBy(x => x.SelectedDate);

            //We'll collect only the top 50 responses to help load times. But user can request all records.
            if (!showAll)
            {
                reviews = reviews.Take(50);
            }


            var reviewsCheckedTime = (from t in db.TaskLog
                                      where t.TaskDescription.Equals("AllReviewsChecked")
                                      orderby t.SuccessDatestamp descending
                                      select t.SuccessDatestamp).First();
            ViewBag.DateStamp = reviewsCheckedTime;

            return View(reviews.ToList());
        }

        // GET: Review/Details/5
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        public ActionResult CloseConfirmed(int id)
        {

            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            review.Reviewed = true;
            review.ReviewDate = DateTime.Now;
            db.Entry(review).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Index");
        }

    

        // GET: Review/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Review/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
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
