using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Ribbon.Models
{
    public class DiscountCode
    {
        public int DiscountCodeID { get; set; }
        public int CampaignID { get; set; }
        public string Code { get; set; }
    }
}
