using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.Models
{

    public enum ReviewType
    {
        Text = 0,
        Photo = 1,
        Video = 2
    }

    public class ItemRequest
    {
        public int ItemRequestID { get; set; }
        public int CampaignID { get; set; }
        public string CustomerID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; } 
        public bool ActiveRequest { get; set; }
        public ReviewType ReviewType { get; set; }
        public bool Selected { get; set; }
       
        public virtual Campaign Campaign { get; set; }
        public virtual Customer Customer { get; set; }       

        public ItemRequest()
        {
            RequestDate = DateTime.Now;
            ActiveRequest = true;
        }

    }
}