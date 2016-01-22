using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.ViewModels
{
    public class CampaignDetailsPageViewModel
    {
        public Campaign Campaign { get; set; }
        public int CodesRemaining { get; set; }
        public ICollection<ItemRequestViewModel> Requests { get; set; }

        public CampaignDetailsPageViewModel(Campaign camp)
        {
            Campaign = camp;
            CodesRemaining = (from c in db.DiscountCodes
                              where c.CampaignID.Equals(Campaign.CampaignID)
                              select c).Count();

            Requests = new List<ItemRequestViewModel>() { };
            foreach(var req in Campaign.ItemRequests)
            {
                ItemRequestViewModel rev = new ItemRequestViewModel(req);
                Requests.Add(rev);
            }
        }

        private BRContext db = new BRContext();
    }

    public class ItemRequestViewModel : CustomerSummaryViewModel
    {
        public ItemRequest Request { get; set; }        

        public ItemRequestViewModel(ItemRequest req)
        {
            Request = req;
            Customer = req.Customer;
            TotalRequests = Customer.Requests.Count();
            CompletedReviews = Customer.Reviews.Where(x => x.Reviewed.Equals(true)).Count();
            OpenReviews = Customer.Reviews.Where(x => x.Reviewed.Equals(false)).Count();
            VideoReviews = Customer.Reviews.Where(x => x.Reviewed.Equals(true) && x.VideoReview.Equals(true)).Count();
            PhotoReviews = Customer.Reviews.Where(x => x.Reviewed.Equals(true) && x.PhotoReview.Equals(true)).Count();

            int stars = 0;
            int words = 0;
            int count = 0;
            foreach (var r in Customer.Reviews.Where(x => x.Reviewed.Equals(true)))
            {
                count = count += 1;
                stars = stars + r.ProductRating.GetValueOrDefault();
                words = words + r.ReviewLength;
            }
            if (count > 0)
            {
                AvgWordCount = words / count;
                AvgProductRating = stars / count;
            }            
        }
    }
}
