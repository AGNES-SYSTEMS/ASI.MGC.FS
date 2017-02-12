using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASI.MGC.FS.Models
{
    public class DayEndOperationModel
    {
        [Required]
        public string DayFrom { get; set; }
        [Required]
        public string DayTo { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string DocumentNo { get; set; }
        public string LastDocumentNo { get; set; }
        [Required]
        public string LastUpdateDate { get; set; }
        [Required]
        public string JobTotal { get; set; }
        [Required]
        public string SalesTotal { get; set; }
        [Required]
        public string DiscountTotal { get; set; }
        [Required]
        public string ShippingTotal { get; set; }
    }
}