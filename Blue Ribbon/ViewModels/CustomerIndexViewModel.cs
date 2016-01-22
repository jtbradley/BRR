using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.ViewModels
{

    public class CustomerIndexViewModel
    {
        public ICollection<CustomerSummaryViewModel> Customers { get; set; }
        

        public CustomerIndexViewModel(bool showAll, string query)
        {
            Customers = new List<CustomerSummaryViewModel>() { };

            var _cust = from c in db.Customers
                        select c;
            
            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(query))
            {
                _cust = _cust.Where(s => (s.FirstName.ToUpper() +" " +s.LastName.ToUpper()).Contains(query.ToUpper()) ||
                s.Email.ToUpper().Contains(query.ToUpper()) || s.CustomerID.ToUpper().Contains(query.ToUpper()));
            }

            //Sort before taking all or just 50. Just using hard-coded sort order for now.
            _cust = _cust.OrderByDescending(x => x.Reviews.Count);

            //After executing search query, limit to 50 by default. If user selected showAll checkbox, this will be 
            //skilled and all customers will be shown (longer load time).
            if (!showAll)
            {
                _cust = _cust.Take(50);
            }

            foreach (var item in _cust.ToList())
            {
                CustomerSummaryViewModel summary = new CustomerSummaryViewModel(item);
                Customers.Add(summary);
            }

        }

        public CustomerIndexViewModel(List<Customer> _cust)
        {
            Customers = new List<CustomerSummaryViewModel>() { };

            foreach (var item in _cust)
            {
                CustomerSummaryViewModel summary = new CustomerSummaryViewModel(item);
                Customers.Add(summary);
            }

        }
        private BRContext db = new BRContext();
    }

    public class CustomerSummaryViewModel
    {
        public Customer Customer { get; set; }
        public int TotalRequests { get; set; }
        public int CompletedReviews { get; set; }
        public int OpenReviews { get; set; }
        public int VideoReviews { get; set; }
        public int PhotoReviews { get; set; }
        public double AvgProductRating { get; set; }
        public double AvgWordCount { get; set; }

        private BRContext db = new BRContext();


        public CustomerSummaryViewModel()
        {
        }

        public CustomerSummaryViewModel(Customer cust)
        {
            Customer = cust;
            TotalRequests = cust.Requests.Count();
            CompletedReviews = cust.Reviews.Where(x => x.Reviewed.Equals(true)).Count();
            OpenReviews = cust.Reviews.Where(x => x.Reviewed.Equals(false)).Count();
            VideoReviews = cust.Reviews.Where(x => x.Reviewed.Equals(true) && x.VideoReview.Equals(true)).Count();
            PhotoReviews = cust.Reviews.Where(x => x.Reviewed.Equals(true) && x.PhotoReview.Equals(true)).Count();

            int stars = 0;
            int words = 0;
            int count = 0;
            foreach(var r in cust.Reviews.Where(x => x.Reviewed.Equals(true)))
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
