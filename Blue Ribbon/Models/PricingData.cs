using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.Models
{
    public class PricingStructure
    {
        public int PricingStructureId { get; set; }
        public double T1 { get; set; }
        public double T2 { get; set; }
        public double T3 { get; set; }
        public double P1 { get; set; }
        public double P2 { get; set; }
        public double P3 { get; set; }
        public double V1 { get; set; }
        public double V2 { get; set; }
        public double V3 { get; set; }
    }
}