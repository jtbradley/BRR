using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Blue_Ribbon.ViewModels
{
    public class SendDiscountViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CustomerID { get; set; }
        public string PromoCode { get; set; }
        public string ItemName { get; set; }
        public string PurchaseURL { get; set; }
        public string PurchaseInstructions { get; set; }
        public string ImageUrl { get; set; }
        public MailMessage newEmail { get; set; }
        public SmtpClient smtp { get; set; }
        
        public SendDiscountViewModel(Review review)
        {
            CustomerID = review.CustomerID;
            var _cust = (from c in db.Customers
                    where c.CustomerID.Equals(CustomerID)
                    select c).Single();

            Email = _cust.Email;
            Name = _cust.FirstName;
            PromoCode = review.DiscountCode;
            var _campaign = (from x in db.Campaigns
                             where x.CampaignID.Equals(review.CampaignID)
                             select x).Single();

            if (String.IsNullOrEmpty(_campaign.VendorsPurchaseURL))
            {
                PurchaseURL = _campaign.AmazonUrl;
            }
            else
            {
                PurchaseURL = _campaign.VendorsPurchaseURL;
            }
            PurchaseInstructions = _campaign.VendorsPurchaseInstructions;
            if (!String.IsNullOrEmpty(PurchaseInstructions))
            {
                PurchaseInstructions = PurchaseInstructions.Replace("\r", "<br>");
                PurchaseInstructions = PurchaseInstructions.Replace("\n", "<br>");
            }


            ItemName = _campaign.Name;
            ImageUrl = _campaign.ImageUrl;
        }


        //Note, we're returning an IRestResponse, but we're not doing anything with it.
        //If you need debugging, you can read response contents for more information.
        public IRestResponse SendCode()
        {
            //Two formats for letter to customer. One if the seller provided custom instructions, and one if there
            //is just a URL to give customer.
            string body;
            if (PurchaseInstructions != null)
            {
                body = "<div style='background-color:#272264;border-radius:10px; color:white'><img src='http://www.blueribbonsreview.com/images/br_logo_white_text.png' style='max-width:200px;float:right; margin:12px' />" +
                    "<h2 style='padding-top:35px;padding-left:15px;'>{0},<br/> You've been selected to be a reviewer!</h2><div style='clear:both;'/></div></div>" +
                    "<p style='color:black;width:66%;'><strong>{1}</strong></p>" +
                    "<img src='{2}' style='max-width:200px;float:right' />" +
                    "<h3 style='color:black;width:66%;margin-right:0px;'>Please purchase the item within 24 hours of this notice as some discount codes expire:</h3></div>" +
                    "<a style='display:inline-block;padding:6px 12px;color:white;border-radius: 6px;background-color:#07A8B5;text-decoration:none;' href={3}><b>Purchase Item</b></a>" +
                    "<p style='color:black;'><strong><i>The seller has requested you use the following instructions to purchase the item on Amazon.com. This " +
                    "helps us continue to offer amazing discounts on great products:</i></strong></p>" +
                    "<div style='background-color:lightgrey;border-radius:6px;display:inline-block;color:black;padding:3px 12px;'>" +
                    "<p style='color:black;'><h3>{4}</h3></p>" +
                    "</div>" +
                    "<h2 style='color:black;'>Use Promo Code: {5}</h2>";
                body = string.Format(body, Name, ItemName, ImageUrl, PurchaseURL, PurchaseInstructions, PromoCode);
            
            }  else {
                 body = "<div style='background-color:#272264;border-radius:10px; color:white'><img src='http://www.blueribbonsreview.com/images/br_logo_white_text.png' style='max-width:200px;float:right; margin:12px' />" +
                       "<h2 style='padding-top:35px;padding-left:15px;'>{0},<br/> You've been selected to be a reviewer!</h2><div style='clear:both;'/></div></div>" +
                        "<h2 style='color:black;width:66%;'><strong>{1}</strong></h2>" +
                       "<img src='{3}' style='max-width:200px;float:right' />" +
                       "<h3 style='color:black;width:66%;margin-right:0px;'>Please purchase the item within 24 hours of this notice as some discount codes expire:</h3></div>" +
                       "<a style='display:inline-block;padding:6px 12px;color:white;border-radius: 6px;background-color:#07A8B5;text-decoration:none;' href={4}><b>Purchase Item</b></a>" +
                       "<h2 style='color:black;'>Use Promo Code: {2}</h2>";
                body = string.Format(body, Name, ItemName, PromoCode, ImageUrl, PurchaseURL );
            }

            //SendMessage
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               WebConfigurationManager.AppSettings["mailSecretKey"]);
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "blueribbonsreview.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            //Who will be displayed as sender, put info here!
            request.AddParameter("from", "Blue Ribbons Review <do-not-reply@blueribbonsreview.com>");

            //Who will be receiving messages, put email here.
            request.AddParameter("to", Email);

            request.AddParameter("subject", ("Congratulations! You've been selected!" + " - " + ItemName));
            request.AddParameter("html", body);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        private BRContext db = new BRContext();
    }
}
