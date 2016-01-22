using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Configuration;

namespace Blue_Ribbon.AmazonAPI
{
    public class LookupByASIN
    {
        public string[] ItemIds;

        public LookupByASIN(string[] ASINs)
        {
            this.ItemIds = ASINs;
        }

        public ItemLookupResponse GetData()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;

            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient(
                        binding,
                        new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));
            // add authentication to the ECS client
            amazonClient.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(ConfigurationManager.AppSettings["accessKeyId"], ConfigurationManager.AppSettings["secretKey"]));

            ItemLookupRequest request = new ItemLookupRequest();
            request.ItemId = this.ItemIds;
            request.IdType = ItemLookupRequestIdType.ASIN;
            request.ResponseGroup = new string[] { "Medium,Offers" };

            ItemLookup itemLookup = new ItemLookup();
            itemLookup.Request = new ItemLookupRequest[] { request };
            itemLookup.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            itemLookup.AssociateTag = ConfigurationManager.AppSettings["associateTag"];

            ItemLookupResponse response = amazonClient.ItemLookup(itemLookup);
            return response;
        }

        public string ReviewsFrame()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;

            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient(
                        binding,
                        new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));
            // add authentication to the ECS client
            amazonClient.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(ConfigurationManager.AppSettings["accessKeyId"], ConfigurationManager.AppSettings["secretKey"]));

            ItemLookupRequest request = new ItemLookupRequest();
            request.ItemId = this.ItemIds;
            request.IdType = ItemLookupRequestIdType.ASIN;
            request.ResponseGroup = new string[] { "Reviews" };

            ItemLookup itemLookup = new ItemLookup();
            itemLookup.Request = new ItemLookupRequest[] { request };
            itemLookup.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            itemLookup.AssociateTag = ConfigurationManager.AppSettings["associateTag"];

            ItemLookupResponse response = amazonClient.ItemLookup(itemLookup);
            string frameUrl = response.Items[0].Item[0].CustomerReviews.IFrameURL;
            return frameUrl;
        }
    }
}