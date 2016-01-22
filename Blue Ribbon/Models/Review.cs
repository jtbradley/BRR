using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.Models
{

    public class Review
    {
        public int ReviewID { get; set; }
        public int CampaignID { get; set; }
        public string CustomerID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SelectedDate { get; set; } //Date customer given discount Code
        public string DiscountCode { get; set; }
        public bool Reviewed { get; set; }
        public int? ProductRating { get; set; }
        public bool VideoReview { get; set; }
        public bool PhotoReview { get; set; }
        public int ReviewLength { get; set; }
        public string ReviewLink { get; set; }
        public ReviewType ReviewTypeExpected { get; set; }

        //if review is too short (<70 words), not verified purchase, or does not include disclaimer, we can use this to 
        //store message about review for customer to fix it.
        public string CustomerAlert { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReviewDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        
        //Note: 1/15/16 Alex requested to change from two weeks to three weeks on due dates for reviews. 
        public DateTime DueDate { get
            {
                return SelectedDate.AddDays(21);
            }
        }

        public int DaysToReview { get
            {
                DateTime dateReviewed = ReviewDate ?? default(DateTime);
                if (ReviewDate != null)
                {
                    return (dateReviewed - SelectedDate).Days;
                }
                else
                {
                    return (DateTime.Now - SelectedDate).Days;
                }
            }
        }

        public virtual Campaign Campaign { get; set; }
        public virtual Customer Customer { get; set; }


    }
}