using Blue_Ribbon.Controllers;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.AmazonAPI
{
    public class CheckForCompletedReviews
    {
        List<string> CustomersToCheck { get; set; }

        //This constructor is used to check ALL open reviews to see if review
        //has been written.
        public CheckForCompletedReviews()
        {
            CustomersToCheck = (from r in db.Reviews
                                where r.Reviewed.Equals(false)
                                select r.CustomerID).Distinct().ToList();
        }

        //This constructor is for one individual customer. Called on Customer/Dashboard controller
        public CheckForCompletedReviews(Customer customer)
        {
            CustomersToCheck = new List<string> { };
            CustomersToCheck.Add(customer.CustomerID);
        }

        //Actual Checking mechanism
        public void Check()
        {
            foreach (var ID in CustomersToCheck)
            {
                //Get list of all reviews by customer that are not done.
                List<Review> reviewFeed = (from revs in db.Reviews
                                           where revs.Reviewed.Equals(false)
                                           where revs.CustomerID.Equals(ID)
                                           select revs).ToList();

                //If there are uncompleted reviews, then get the customers
                //Amazon RSS review feed and check to see if review completed.
                if (reviewFeed.Count() > 0)
                {
                    ParsedFeed currentFeed = new ParsedFeed(ID);
                    foreach (Review r in reviewFeed)
                    {
                        foreach (ParsedReview p in currentFeed.FeedReviews)
                        {
                            if (p.ASIN == r.Campaign.ASIN)
                            {
                                r.ReviewDate = p.ReviewDate;
                                r.ProductRating = p.Rating;
                                r.VideoReview = p.HasVideo;
                                r.PhotoReview = p.HasPhoto;
                                r.ReviewLength = p.ReviewLength;
                                r.ReviewLink = p.Link;

                                //We'll mark review as complete now, but do some checks next.
                                r.Reviewed = true;

                                //Checking for various attributes.
                                if(r.ReviewLength < 70)
                                {
                                    r.Reviewed = false;
                                    r.CustomerAlert = "Reviews must be at least 70 words long. Please update your review on Amazon.";
                                }

                                // 1/20/15 Alex asked to NOT check for Verified Purchase as Amazon won't mark all discounted/free items as verified. 
                                //if (!p.VerfiedPurchase)
                                //{
                                //    r.Reviewed = false;
                                //    r.CustomerAlert = "Your review is not marked as a Verified Purchase. " +
                                //        "Reviewers must purchase/test product before reviewing.";
                                //}

                                if(r.ReviewTypeExpected.ToString() == "Photo" && p.HasPhoto == false)
                                {
                                    r.Reviewed = false;
                                    r.CustomerAlert = "You agreed to do a photo review and your review does not appear to have photos. " +
                                        "Please add at least one photo your review on Amazon.";
                                }

                                if (r.ReviewTypeExpected.ToString() == "Video" && p.HasVideo == false)
                                {
                                    r.Reviewed = false;
                                    r.CustomerAlert = "You agreed to do a video review and your review does not appear to have a video. " +
                                        "Please add a video to your review.";
                                }

                                if (!p.HasDisclaimer)
                                {
                                    r.Reviewed = false;
                                    r.CustomerAlert = "Your review does not appear to have included the disclaimer. " +
                                        "Please update your review on Amazon.";
                                }

                                db.Entry(r).State = EntityState.Modified;
                            }
                        }
                    }

                }

                db.SaveChanges();
            }
        }  

        private BRContext db = new BRContext();
    }
}
