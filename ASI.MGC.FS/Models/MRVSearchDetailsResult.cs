using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASI.MGC.FS.Models
{
    public class MRVSearchDetailsResult
    {
        public string MRVNO_SD { get; set; }
        public DateTime MRVDate { get; set; }
        public string JOBNO_SD { get; set; }
        public string PRCODE_SD { get; set; }
        public string JOBID_SD { get; set; }
        public int QTY_SD { get; set; }
        public decimal RATE_SD { get; set; }
        public decimal Amount { get; set; }
        public decimal DISCOUNT_SD { get; set; }
        public decimal SHIPPINGCHARGES_SD { get; set; }
        public DateTime SALEDATE_SD { get; set; }
        public string USERID_SD { get; set; }
        public decimal CASHTOTAL_SD { get; set; }
        public decimal CREDITTOTAL_SD { get; set; }
        public string CASHRVNO_SD { get; set; }
        public string INVNO_SD { get; set; }
        public string CREDITACCODE_SD { get; set; }
        public string LPONO_SD { get; set; }
        public string DAYENDDOC_NO { get; set; }
    }
}