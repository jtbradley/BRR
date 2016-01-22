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
    public class GeneralMessageViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

    
        public IRestResponse SendForm()
        {
            //Assemble message first.
            var body = "<h2>General Form Submission</h2><hr/>" +
                "<p>Contact Name: {0}</p>" +
                "<p>Email: {1}</p>" +
                "<p>Message:<BR> {2}</p>";
            body = string.Format(body, Name, Email, Message);


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
            request.AddParameter("from", "Site Form - General <do-not-reply@blueribbonsreview.com>");

            //Who will be receiving messages, put email here.
            request.AddParameter("to", "amber@blueribbonsreview.com");

            request.AddParameter("subject", "General Contact Form Submission");
            request.AddParameter("html", body);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
}
