using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.ViewModels
{
    public class VendorIndexViewModel
    {
        public ICollection<VendorSummaryViewModel> Vendors { get; set; }


        public VendorIndexViewModel(string query)
        {
            Vendors = new List<VendorSummaryViewModel> { };
            var _vendors = from v in db.Vendors
                            select v;


            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(query))
            {
                _vendors = _vendors.Where(s => s.Name.ToUpper().Contains(query.ToUpper()) ||
                s.Email.ToUpper().Contains(query.ToUpper()) || s.ContactName.ToUpper().Contains(query.ToUpper()) || 
                s.VendorNotes.ToUpper().Contains(query.ToUpper())).OrderBy(n => n.Name);
            }

            foreach (var v in _vendors.ToList())
            {
                VendorSummaryViewModel vs = new VendorSummaryViewModel(v);
                Vendors.Add(vs);
            }
        }

        public VendorIndexViewModel(List<Vendor> topvendors)
        {
            Vendors = new List<VendorSummaryViewModel> { };

            foreach (var v in topvendors)
            {
                VendorSummaryViewModel vs = new VendorSummaryViewModel(v);
                Vendors.Add(vs);
            }
        }

        private BRContext db = new BRContext();
    }

    public class SingleVendorViewModel
    {
        public Vendor Vendor { get; set; }
        public ICollection<VendorSummaryViewModel> vendorsummary { get; set; }
        public CampaignIndexViewModel vendorcampaigns { get; set; }

        public SingleVendorViewModel(Vendor vendor)
        {
            Vendor = vendor;
            vendorsummary = new List<VendorSummaryViewModel> { };
            vendorsummary.Add(new VendorSummaryViewModel(vendor));

            var _camps = (from c in db.Campaigns
                          where c.VendorID.Equals(vendor.VendorId)
                          select c).ToList();

            vendorcampaigns = new CampaignIndexViewModel(_camps);

        }
        private BRContext db = new BRContext();
    }


    public class VendorSummaryViewModel
    {
        public Vendor vendor { get; set; }
        public int TotalCampaigns { get; set; }
        public int OpenCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }
        public int TotalReviews { get; set; }
        public int OpenReviews { get; set; }
        public int CompletedReviews { get; set; }
        public double AvgProductRating { get; set; }
        public int AvgWordCount { get; set; }

        public VendorSummaryViewModel(Vendor ven)
        {
            vendor = ven;

            var _camps = from c in db.Campaigns
                         where c.VendorID.Equals(vendor.VendorId)
                         select c;

            TotalCampaigns = _camps.Count();
            OpenCampaigns = _camps.Where(x => x.OpenCampaign.Equals(true)).Count();
            CompletedCampaigns = _camps.Where(x => x.OpenCampaign.Equals(false)).Count();

            var _revs = from r in db.Reviews
                        where r.Campaign.VendorID.Equals(vendor.VendorId)
                        select r;

            TotalReviews = _revs.Count();
            OpenReviews = _revs.Where(x => x.Reviewed.Equals(false)).Count();
            CompletedReviews = _revs.Where(x => x.Reviewed.Equals(true)).Count();

            _revs = _revs.Where(x => x.Reviewed.Equals(true));

            int stars = 0;
            int words = 0;
            int count = 0;
            foreach (var r in _revs.Where(x => x.Reviewed.Equals(true)))
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


        private BRContext db = new BRContext();
    }
}
