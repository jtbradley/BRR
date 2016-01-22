using Blue_Ribbon.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Blue_Ribbon.Models
{
    public class Campaign
    {
        //Since this is an Amazon-Specific product, we'll use ASIN as ID
        [Display(Name = "Campaign #")]
        public int CampaignID { get; set; }
        public string ASIN { get; set; }
        public int VendorID { get; set; }
        public string Name { get; set; }
        public bool OpenCampaign { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool TextValid { get; set; }
        public int TextGoal { get; set; }
        public bool PhotoValid { get; set; }
        public int PhotoGoal { get; set; }
        public bool VideoValid { get; set; }
        public int VideoGoal { get; set; }
        public int CurrentRequests { get; set; }
        public int ReviewsStillNeeded { get; set; }
        public int DailyLimit { get; set; }
        public bool DailyLimitReached { get; set; }
        public Dictionary<string, int> ReviewGoals { get; set; }
        public Dictionary<string, List<int>> CampaignStats { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string RetailPrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public string SalePrice { get; set; }
        public double SalePriceNumerical { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CloseDate { get; set; }
        [Display(Name = "Special price")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public double CalculatedDiscount { get; set; }
        public string VendorsPurchaseInstructions { get; set; }
        public string VendorsPurchaseURL { get; set; }
        public string Discount
        {
            get
            {                
                return String.Format("{0:C}", CalculatedDiscount);
            }
        }

        public string displayAsCurrency(double price)
        {
            return String.Format("{0:C}", price);
        }
        public string AmazonUrl
            {
            get
            {
                return String.Format("http://www.amazon.com/dp/{0}/?tag={1}", ASIN, WebConfigurationManager.AppSettings["associateTag"]);
            }
        }

        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ItemRequest> ItemRequests { get; set; }

        public Campaign()
        {
            StartDate = DateTime.Now;
            OpenCampaign = true;
            RetailPrice = "0";
            TextGoal = 0;
            TextValid = true;
            PhotoGoal = 0;
            PhotoValid = true;
            VideoGoal = 0;
            VideoValid = true;
            DailyLimit = 100;
            DailyLimitReached = false;

        }

        private Dictionary<string, List<dynamic>> ReviewTypes()
        {
            Dictionary<string, List<dynamic>> validtypes = new Dictionary<string, List<dynamic>> {
                {"Text" ,new List<dynamic> { TextValid, TextGoal } },
                {"Photo" ,new List<dynamic> { PhotoValid, PhotoGoal } },
                {"Video" ,new List<dynamic> { VideoValid, VideoGoal } },
            };

            return validtypes;
        }


        public List<SelectListItem> GetReviewTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            Dictionary<string, List<dynamic>> validtypes = ReviewTypes();
            foreach (var item in validtypes.Keys)
            {
                string convertedName;
                if (item == "Text")
                {
                    convertedName = "Well-Written";
                }
                else
                {
                    convertedName = item;
                }

                if (validtypes[item][0] == true)
                {
                    types.Add(new SelectListItem
                    {
                        Text = convertedName,
                        Value = item
                    });
                }
            }
            return types;
        }

        public void SetNumericalPrices()
        {
            CalculatedDiscount = Math.Round(double.Parse(RetailPrice) - double.Parse(SalePrice), 2);
            SalePriceNumerical = double.Parse(SalePrice);
        }
        public Dictionary<string, List<int>> UpdateCampaignStats()
        {
            int totalgoal = 0;
            Dictionary<string, List<dynamic>> validtypes = ReviewTypes();
            Dictionary<string, List<int>> CampaignStats = new Dictionary<string, List<int>> { };

            foreach (var item in validtypes.Keys)
            {
                CampaignStats.Add(item, new List<int> { validtypes[item][1], 0, 0, 0, 0 });
                    totalgoal = totalgoal + validtypes[item][1];
                }

            CampaignStats.Add("Total", new List<int> { totalgoal, 0, 0, 0, 0 });
            
            if (ItemRequests != null)
            {
                foreach (var item in ItemRequests)
                {
                    if (item.ActiveRequest)
                    {
                        CampaignStats[item.ReviewType.ToString()][1]++;
                    }
                }
            }

            if (Reviews != null)
            {
                foreach (var item in Reviews)
                {
                    if (item.Reviewed == false)
                    {
                        CampaignStats[item.ReviewTypeExpected.ToString()][2]++;
                    }
                    else
                    {
                        CampaignStats[item.ReviewTypeExpected.ToString()][3]++;
                    }                    
                }
            }

            foreach (var item in CampaignStats.Keys)
            {
                CampaignStats[item][4] = CampaignStats[item][0] - CampaignStats[item][2] - CampaignStats[item][3];
                
                if (item != "Total")
                {
                    CampaignStats["Total"][1] = CampaignStats["Total"][1] + CampaignStats[item][1];
                    CampaignStats["Total"][2] = CampaignStats["Total"][2] + CampaignStats[item][2];
                    CampaignStats["Total"][3] = CampaignStats["Total"][3] + CampaignStats[item][3];
                    CampaignStats["Total"][4] = CampaignStats["Total"][4] + CampaignStats[item][4];
                }
            }
            ReviewsStillNeeded = CampaignStats["Total"][4];
            CurrentRequests = CampaignStats["Total"][1];
            return CampaignStats;
        }

        private BRContext db = new BRContext();
    }
}