using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASI.MGC.FS.Models
{
    public class CustomAllocationDetails
    {
        public string AlCode { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Narration { get; set; }
    }
    public class CustomJvAllocationDetails
    {
        public string AlCode { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string Narration { get; set; }
    }
}