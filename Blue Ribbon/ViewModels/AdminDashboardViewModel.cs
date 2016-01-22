using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int OpenCampaigns { get; set; }
        public int ClosedCampaigns { get; set; }
        public int Customers { get; set;}
        public int UnqualifiedCustomers { get; set; }
        public int Vendors { get; set; }
        public int ActiveRequests { get; set; }
        public int CodesNeeded { get; set; }
        public int TotalReviewsDone { get; set; }
        public int NotificationsToday { get; set; }

        public CustomerIndexViewModel TopReviewers { get; set; }
        public VendorIndexViewModel TopVendors { get; set; }
        public CampaignIndexViewModel TopActiveCampaigns { get; set; }

        public AdminDashboardViewModel()
        {
            OpenCampaigns = (from a in db.Campaigns
                             where a.OpenCampaign.Equals(true)
                             select a).Count();

            ClosedCampaigns = (from b in db.Campaigns
                               where b.OpenCampaign.Equals(false)
                               select b).Count();

            Customers = (from c in db.Customers
                         select c).Count();

            UnqualifiedCustomers = (from d in db.Customers
                                    where d.Qualified.Equals(false)
                                    select d).Count();

            Vendors = (from e in db.Vendors
                       select e).Count();

            var _cust = (from f in db.Customers
                            where f.Qualified.Equals(true)
                            orderby f.Reviews.Count() descending
                            select f).Take(5).ToList();
            TopReviewers = new CustomerIndexViewModel(_cust);

            var _topvendors = (from g in db.Campaigns
                              orderby g.Reviews.Count() descending
                              select g.Vendor).Take(5).ToList();
            TopVendors = new VendorIndexViewModel(_topvendors);

            var _topcamps = (from h in db.Campaigns
                                  where h.OpenCampaign.Equals(true)
                                  orderby h.Reviews.Count() descending
                                  select h).Take(5).ToList();

            TopActiveCampaigns = new CampaignIndexViewModel(_topcamps);

            ActiveRequests = (from i in db.ItemRequests
                              where i.ActiveRequest.Equals(true)
                              select i).Count();
            CodesNeeded = (from j in db.Reviews
                           where j.Reviewed.Equals(false)
                           where j.DiscountCode.Equals(null)
                           select j).Count();

            TotalReviewsDone = (from j in db.Reviews
                           where j.Reviewed.Equals(true)
                           select j).Count();

            //Notifications count for today
            DateTime today = DateTime.Now.Date;
            NotificationsToday = (from n in db.NotificationLog
                                  where n.LogTimestamp > today
                                  select n).Count();


        }

        BRContext db = new BRContext();

    }
}
