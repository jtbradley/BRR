using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.Models
{
    public class Customer
    {
        //Identity Model will use unique customer ID, but user will be required to enter Amazon ID to sign up:
        //Amazon ID will then be used to generate a new customer model.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Amazon ID")]
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastReviewCheck { get; set; }

        public bool Qualified { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoinDate { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ItemRequest> Requests { get; set; }

        public int DaysBeingMember { get
            {
                return (DateTime.Now - JoinDate).Days;
            }
        }
        [Display(Name ="Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string generateURL()
        {
            string productUrl = String.Format("http://www.amazon.com/gp/cdp/member-reviews/{0}/ref=pdp_new", CustomerID);
            return productUrl;
        }

        public Dictionary<string,int> getStats()
        {
            Dictionary<string, int> stats = new Dictionary<string, int> { { "reviewsdone", 0 }, { "avgtext", 0 }, { "photos", 0 }, { "videos", 0 } };
            int wordcount = 0;
            foreach(var item in Reviews)
            {
                wordcount = wordcount + item.ReviewLength;
                if (item.Reviewed == true) { stats["reviewsdone"]++; }
                if (item.PhotoReview == true) { stats["photos"]++; }
                if (item.VideoReview == true) { stats["videos"]++; }
            }
            if (stats["reviewsdone"]==0)
            {
                stats["avgtext"] = 0;
            }
            else
            {
                stats["avgtext"] = wordcount / stats["reviewsdone"];
            }

            return stats;
        }
    }
}