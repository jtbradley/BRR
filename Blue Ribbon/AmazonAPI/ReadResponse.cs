using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blue_Ribbon.Models;
using Blue_Ribbon.AmazonAPI;
using System.Text.RegularExpressions;

namespace Blue_Ribbon.AmazonAPI
{
    public class ReadResponse
    {

        public void Populate(Campaign campaign, string[] ASIN)
        {
            campaign.ASIN = ASIN[0];
            List<dynamic> ids = new List<dynamic> { };
            LookupByASIN generateData = new LookupByASIN(ASIN);
            ItemLookupResponse response = generateData.GetData();

            foreach (var item in response.Items[0].Item)
            {
                List<dynamic> attributes = new List<dynamic>();
                campaign.Name = item.ItemAttributes.Title;
                campaign.Category = item.ItemAttributes.Binding;
                campaign.ImageUrl = item.LargeImage.URL;
                string description = "Sorry, no description available.";
                if (item.EditorialReviews != null)
                {
                    description = item.EditorialReviews[0].Content;
                    description = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
                }
                campaign.Description = description;

                //Some Items have a price AND a sale price. We want the lower of the two.
                string regularprice = item.Offers.Offer[0].OfferListing[0].Price.Amount;
                string rawprice = regularprice;
                if(item.Offers.Offer[0].OfferListing[0].SalePrice != null)
                {
                    string saleprice = item.Offers.Offer[0].OfferListing[0].SalePrice.Amount;
                    if (int.Parse(saleprice) < int.Parse(regularprice))
                    {
                        rawprice = saleprice;
                    }
                }
                campaign.RetailPrice = (float.Parse(rawprice) / 100).ToString(); ;
            }

        }


        public Campaign Update(Campaign campaign)
        {
            string[] ASIN = new string[] { campaign.ASIN };
            List<dynamic> ids = new List<dynamic> { };
            LookupByASIN generateData = new LookupByASIN(ASIN);
            ItemLookupResponse response = generateData.GetData();

            foreach (var item in response.Items[0].Item)
            {
                List<dynamic> attributes = new List<dynamic>();
                campaign.Name = item.ItemAttributes.Title;
                campaign.Category = item.ItemAttributes.Binding;
                campaign.ImageUrl = item.LargeImage.URL;
                string description = "Sorry, no description available.";
                if (item.EditorialReviews != null)
                {
                    description = item.EditorialReviews[0].Content;
                    description = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
                }
                campaign.Description = description;
                //Some Items have a price AND a sale price. We want the lower of the two.
                string regularprice = item.Offers.Offer[0].OfferListing[0].Price.Amount;
                string rawprice = regularprice;
                if (item.Offers.Offer[0].OfferListing[0].SalePrice != null)
                {
                    string saleprice = item.Offers.Offer[0].OfferListing[0].SalePrice.Amount;
                    if (int.Parse(saleprice) < int.Parse(regularprice))
                    {
                        rawprice = saleprice;
                    }
                }
                campaign.RetailPrice = (float.Parse(rawprice) / 100).ToString(); ;
            }

            return campaign;

        }
    }

}