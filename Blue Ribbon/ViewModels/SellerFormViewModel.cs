using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Blue_Ribbon.ViewModels
{
    public class SellerFormViewModel
    {
        [Required]
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string ProductLink { get; set; }
        public string Message { get; set; }


        //Note, we're returning an IRestResponse, but we're not doing anything with it.
        //If you need debugging, you can read response contents for more information.
        public IRestResponse SendForm()
        {
            //Assemble message first.
            var body = "<h2>Seller Form Submission</h2><hr/>" +
                "<p>Contact Name: {0}</p>" +
                "<p>Company Name: {1}</p>" +
                "<p>Phone Number: {2}</p>" +
                "<p>Email: {3}</p>" +
                "<p>Product Link: {4}</p>" +
                "<p>Message:<BR> {5}</p>";
            body = string.Format(body, ContactName, CompanyName, Phone, Email, ProductLink, Message);


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
            request.AddParameter("from", "Site Form - Seller Inquiry <do-not-reply@blueribbonsreview.com>");

            //Who will be receiving messages, put email here.
            request.AddParameter("to", "amber@blueribbonsreview.com");

            request.AddParameter("subject", "Seller Contact Form Submission");
            request.AddParameter("html", body);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
}
