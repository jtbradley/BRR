using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Blue_Ribbon.Models
{
    public class Vendor
    {
        [Display(Name = "Company Name")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorId { get; set; }


        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string VendorNotes { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoinDate { get; set; }



        public virtual Customer Customer { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }

    }
}