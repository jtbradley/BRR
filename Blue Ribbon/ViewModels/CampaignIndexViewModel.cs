using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Blue_Ribbon.ViewModels
{
    public class CampaignIndexViewModel
    {
        public ICollection<CampaignOverviewViewModel> Campaigns { get; set; }
        public int AlertRequests { get; set; }
        public int AlertCodes { get; set; }
        public List<SelectListItem> ActiveClosedSelector { get; set; }


        public CampaignIndexViewModel(string sortOrder, string query, string active)
        {
            Campaigns = new List<CampaignOverviewViewModel>() { };
            AlertCodes = 0;
            AlertRequests = 0;

            var campaigns = from s in db.Campaigns
                            select s;

            if (active != "all")
            {
                if(active == "closed")
                {
                    campaigns = campaigns.Where(x => x.OpenCampaign.Equals(false));
                }
                else
                {
                    campaigns = campaigns.Where(x => x.OpenCampaign.Equals(true));
                }
            }

            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(query))
            {
                campaigns = campaigns.Where(s => s.Name.ToUpper().Contains(query.ToUpper()) ||
                s.Vendor.Name.ToUpper().Contains(query.ToUpper()) || s.Description.ToUpper().Contains(query.ToUpper()));
            }


            foreach (var item in campaigns.ToList())
            {
                CampaignOverviewViewModel summary = new CampaignOverviewViewModel();
                summary.Campaign = item;
                summary.OpenRequests = (from c in db.ItemRequests
                                        where c.CampaignID.Equals(item.CampaignID)
                                        where c.ActiveRequest.Equals(true)
                                        select c).Count();
                AlertRequests = AlertRequests + summary.OpenRequests;

                summary.CompletedReviews = (from c in db.Reviews
                                            where c.CampaignID.Equals(item.CampaignID)
                                            where c.Reviewed.Equals(true)
                                            select c).Count();

                summary.OpenReviews = (from c in db.Reviews
                                        where c.CampaignID.Equals(item.CampaignID)
                                        where c.Reviewed.Equals(false)
                                        select c).Count();

                summary.CodesNeeded = (from c in db.Reviews
                                       where c.CampaignID.Equals(item.CampaignID)
                                       where c.Reviewed.Equals(false)
                                       where c.DiscountCode.Equals(null)
                                       select c).Count();
                AlertCodes = AlertCodes + summary.CodesNeeded;
                summary.ReviewsStillNeeded = item.TextGoal + item.PhotoGoal + item.VideoGoal - summary.OpenReviews - summary.CompletedReviews;
                Campaigns.Add(summary);
            }

            switch (sortOrder)
            {
                case "requests_desc":
                    Campaigns = Campaigns.OrderByDescending(s => s.OpenRequests).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "requests_asc":
                    Campaigns = Campaigns.OrderBy(s => s.OpenRequests).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "revs_needed_desc":
                    Campaigns = Campaigns.OrderByDescending(s => s.Campaign.ReviewsStillNeeded).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "revs_needed":
                    Campaigns = Campaigns.OrderBy(s => s.Campaign.ReviewsStillNeeded).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "Vendor":
                    Campaigns = Campaigns.OrderByDescending(s => s.Campaign.Vendor.Name).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "vendor_desc":
                    Campaigns = Campaigns.OrderBy(s => s.Campaign.Vendor.Name).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "codes_desc":
                    Campaigns = Campaigns.OrderByDescending(s => s.CodesNeeded).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;

                case "codes":
                    Campaigns = Campaigns.OrderBy(s => s.CodesNeeded).ThenByDescending(s => s.Campaign.OpenCampaign).ToList();
                    break;
            }

            ActiveClosedSelector = new List<SelectListItem>() { };
            ActiveClosedSelector.Add(new SelectListItem
            {
                Text = "All",
                Value = "all",
                Selected = active == "all"
            });
            ActiveClosedSelector.Add(new SelectListItem
            {
                Text = "Open Campaigns Only",
                Value = "open",
                Selected = active == "open"
            });
            ActiveClosedSelector.Add(new SelectListItem
            {
                Text = "Closed Campaigns Only",
                Value = "closed",
                Selected = active == "closed"
            });
        }

        public CampaignIndexViewModel(List<Campaign> topcamps)
        {
            Campaigns = new List<CampaignOverviewViewModel>() { };

            foreach (var item in topcamps)
            {
                CampaignOverviewViewModel summary = new CampaignOverviewViewModel();
                summary.Campaign = item;
                summary.OpenRequests = (from c in db.ItemRequests
                                        where c.CampaignID.Equals(item.CampaignID)
                                        where c.ActiveRequest.Equals(true)
                                        select c).Count();

                summary.CompletedReviews = (from c in db.Reviews
                                            where c.CampaignID.Equals(item.CampaignID)
                                            where c.Reviewed.Equals(true)
                                            select c).Count();

                summary.OpenReviews = (from c in db.Reviews
                                       where c.CampaignID.Equals(item.CampaignID)
                                       where c.Reviewed.Equals(false)
                                       select c).Count();

                summary.CodesNeeded = (from c in db.Reviews
                                       where c.CampaignID.Equals(item.CampaignID)
                                       where c.Reviewed.Equals(false)
                                       where c.DiscountCode.Equals(null)
                                       select c).Count();
                summary.ReviewsStillNeeded = item.TextGoal + item.PhotoGoal + item.VideoGoal - summary.OpenReviews - summary.CompletedReviews;

                Campaigns.Add(summary);
            }
        }

        private BRContext db = new BRContext();

    }

    public class CampaignOverviewViewModel
    {
        public Campaign Campaign { get; set; }
        public int OpenRequests { get; set; }
        public int CompletedReviews { get; set; }
        public int OpenReviews { get; set; }
        public int CodesNeeded { get; set; }
        public int ReviewsStillNeeded { get; set; }
    }
}
