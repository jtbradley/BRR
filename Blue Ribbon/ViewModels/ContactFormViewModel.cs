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

namespace Blue_Ribbon.Models
{
    public class ContactFormViewModel
    {
        public bool LoggedIn { get; set; }
        public string AmazonID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool ReviewedItem { get; set; }
        public string ReviewedItemASIN { get; set; }
        
        public bool DiscountNeeded { get; set; }
        public string DiscountNeededASIN { get; set; }

        public string Message { get; set; }




        //Note, we're returning an IRestResponse, but we're not doing anything with it.
        //If you need debugging, you can read response contents for more information.
        public IRestResponse SendForm()
        {
            //Assemble message first.
            string revitem = "";
            string codeitem = "";

            if (ReviewedItem)
            {
                revitem = "<hr><p> Customer states they already reviewed item #" + ReviewedItemASIN +
                    " but it is still showing on dashboard.</p>";
            }
            if (DiscountNeeded)
            {
                codeitem = "<hr><p> Customer is waiting for discount code on item #" + DiscountNeededASIN + ".</p>";
            }

            var body = "<h2>Email From:</h2>" +
                "<p>{0} ({1})<br>" +
                "Amazon ID: {2}</p>" +
                revitem + codeitem +
                "<hr><p>General Message:</p><p>{3}</p><hr>";
            body = string.Format(body, Name, Email, AmazonID, Message);


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
            request.AddParameter("from", "Site Form - Customer Inquiry <do-not-reply@blueribbonsreview.com>");

            //Who will be receiving messages, put email here.
            request.AddParameter("to", "amber@blueribbonsreview.com");

            request.AddParameter("subject", "Customer Contact Form Submission");
            request.AddParameter("html", body);
            request.Method = Method.POST;
            return client.Execute(request);
        }


        public MailMessage ContactFormEmail()
        {
            var newEmail = new MailMessage();

            string revitem = "";
            string codeitem = "";

            if (ReviewedItem)
            {
                revitem = "<hr><p> Customer states they already reviewed item #" + ReviewedItemASIN +
                    " but it is still showing on dashboard.</p>";
            }
            if (DiscountNeeded)
            {
                codeitem = "<hr><p> Customer is waiting for discount code on item #" + DiscountNeededASIN + ".</p>";
            }

            var body = "<h2>Email From:</h2>" +
                "<p>{0} ({1})<br>"+
                "Amazon ID: {2}</p>" +
                revitem + codeitem +
                "<hr><p>General Message:</p><p>{3}</p><hr>";

            newEmail.To.Add(new MailAddress("jeflux@gmail.com"));  // person to receive customer contact
            newEmail.From = new MailAddress("test@jeflux.com");  // sending account
            newEmail.Subject = "Customer Contact Form Submission";
            newEmail.Body = string.Format(body, Name, Email, AmazonID, Message);
            newEmail.IsBodyHtml = true;


            return newEmail;
        }


        public SmtpClient GenerateSmtpClient()
        {
            var smtp = new SmtpClient();
            var credential = new NetworkCredential();
            credential.UserName = "test@jeflux.com";  // replace with contactform sender email
            credential.Password = "Test123!";  // replace with valid value


            smtp.Credentials = credential;
            smtp.Host = "smtp.fatcow.com";
            smtp.Port = 587;
            //smtp.EnableSsl = true;

            return smtp;
        }

       
    }
}
