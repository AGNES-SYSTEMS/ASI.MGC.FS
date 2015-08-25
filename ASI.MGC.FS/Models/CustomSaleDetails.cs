using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASI.MGC.FS.Models
{
    public class CustomSaleDetails
    {
        public string JobNo { get; set; }
        public string PRCode { get; set; }
        public string SWCode { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public double Rate { get; set; }
        public double CashAmount { get; set; }
        public double Discount { get; set; }
        public double ShipChrg { get; set; }
    }
}