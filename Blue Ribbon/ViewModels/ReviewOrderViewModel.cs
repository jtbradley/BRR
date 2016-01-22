using Blue_Ribbon.Models;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.ViewModels
{
    public class ReviewOrderViewModel
    {
        public string TotalPrice { get; set; }
        public int TotalReviews { get; set; }
        public string PaypalComment { get; set; }
        public string PaypalURL { get; set; }
        public Campaign Campaign { get; set; }
        public string DiscountCodes { get; set; }
        public string PaymentConfirmationID { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private string signature { get; set; }
        private string cancelUrl { get; set; }
        private string returnUrl { get; set; }
        private string appMode { get; set; }
        private string paymentUrl { get; set; }

        public ReviewOrderViewModel()
        {
            //PAYPAL API TESTING SETTINGS
            //username = "jeflux-facilitator_api1.gmail.com";
            //password = "PFAGDFXG3GVZSNB9";
            //signature = "AjDzjBSCTnygzbwwdgW8.jjIbRcnAYl3hDQuugithVWQfKB7K68ZC6bn";

            //returnUrl = "https://localhost:44300/Seller/ProcessPayment";
            //cancelUrl = "https://localhost:44300/Seller/";

            //paymentUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=";

            //appMode = "sandbox";


            //PAYPAL API LIVE SETTINGS
            username = "alex_api1.blueribbonsreview.com";
            password = "ME8WMQS2RG4LXQSN";
            signature = "AFcWxV21C7fd0v3bYYYRCpSSRl31Ai9a8dBS8PwZ5D0knbmO2QaKZTN4";

            returnUrl = "http://www.blueribbonsreview.com/Seller/ProcessPayment";
            cancelUrl = "http://www.blueribbonsreview.com/Seller/";

            paymentUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=";

            appMode = "live";

        }


        public string SetupPayment()
        {
            PaymentDetailsType paymentDetail = new PaymentDetailsType();
            CurrencyCodeType currency = (CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType));
            PaymentDetailsItemType paymentItem = new PaymentDetailsItemType();
            paymentItem.Name = PaypalComment;
            paymentItem.Amount = new BasicAmountType(currency, this.TotalPrice);
            int itemQuantity = 1;
            paymentItem.Quantity = itemQuantity;
            List<PaymentDetailsItemType> paymentItems = new List<PaymentDetailsItemType>();
            paymentItems.Add(paymentItem);
            paymentDetail.PaymentDetailsItem = paymentItems;

            paymentDetail.PaymentAction = (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
            paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType)), this.TotalPrice);
            List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
            paymentDetails.Add(paymentDetail);

            SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
            ecDetails.ReturnURL = this.returnUrl;
            ecDetails.CancelURL = this.cancelUrl;
            ecDetails.PaymentDetails = paymentDetails;

            SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
            request.Version = "104.0";
            request.SetExpressCheckoutRequestDetails = ecDetails;

            SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
            wrapper.SetExpressCheckoutRequest = request;
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", appMode);
            sdkConfig.Add("account1.apiUsername", username);
            sdkConfig.Add("account1.apiPassword", password);
            sdkConfig.Add("account1.apiSignature", signature);
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(sdkConfig);
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);

            string url = paymentUrl + setECResponse.Token;

            return url;
        }

        public string ProcessPayment(string token, string PayerID)
        {
            string result = "success";

            GetExpressCheckoutDetailsRequestType request = new GetExpressCheckoutDetailsRequestType();
            request.Version = "104.0";
            request.Token = token;
            GetExpressCheckoutDetailsReq wrapper = new GetExpressCheckoutDetailsReq();
            wrapper.GetExpressCheckoutDetailsRequest = request;
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

            sdkConfig.Add("mode", appMode);
            sdkConfig.Add("account1.apiUsername", username);
            sdkConfig.Add("account1.apiPassword", password);
            sdkConfig.Add("account1.apiSignature", signature);
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(sdkConfig);
            GetExpressCheckoutDetailsResponseType ecResponse = service.GetExpressCheckoutDetails(wrapper);

            PaymentDetailsType paymentDetail = new PaymentDetailsType();
            paymentDetail.NotifyURL = "http://replaceIpnUrl.com";
            paymentDetail.PaymentAction = (PaymentActionCodeType)EnumUtils.GetValue("Sale", typeof(PaymentActionCodeType));
            paymentDetail.OrderTotal = new BasicAmountType((CurrencyCodeType)EnumUtils.GetValue("USD", typeof(CurrencyCodeType)), this.TotalPrice);
            paymentDetail.OrderDescription = PaypalComment;
            List<PaymentDetailsType> paymentDetails = new List<PaymentDetailsType>();
            paymentDetails.Add(paymentDetail);

            DoExpressCheckoutPaymentRequestType request2 = new DoExpressCheckoutPaymentRequestType();
            request.Version = "104.0";
            DoExpressCheckoutPaymentRequestDetailsType requestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            requestDetails.PaymentDetails = paymentDetails;
            requestDetails.Token = token;
            requestDetails.PayerID = PayerID;
            request2.DoExpressCheckoutPaymentRequestDetails = requestDetails;

            DoExpressCheckoutPaymentReq wrapper2 = new DoExpressCheckoutPaymentReq();
            wrapper2.DoExpressCheckoutPaymentRequest = request2;
            Dictionary<string, string> sdkConfig2 = new Dictionary<string, string>();


            sdkConfig2.Add("mode", appMode);
            sdkConfig2.Add("account1.apiUsername", username);
            sdkConfig2.Add("account1.apiPassword", password);
            sdkConfig2.Add("account1.apiSignature", signature);
            PayPalAPIInterfaceServiceService service2 = new PayPalAPIInterfaceServiceService(sdkConfig);
            DoExpressCheckoutPaymentResponseType doECResponse = service2.DoExpressCheckoutPayment(wrapper2);

            this.PaymentConfirmationID = doECResponse.CorrelationID;

            return result;
        }
    }
}

