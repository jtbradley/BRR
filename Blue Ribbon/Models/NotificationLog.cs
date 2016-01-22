using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.Models
{
    public class NotificationLog
    {
        public int NotificationLogID { get; set; }
        public int CampaignID { get; set; }
        public DateTime LogTimestamp { get; set; }
        public string Message { get; set; }

        public virtual Campaign Campaign { get; set; }

    }
}