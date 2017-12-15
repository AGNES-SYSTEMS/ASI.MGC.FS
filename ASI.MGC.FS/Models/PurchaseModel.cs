using System.ComponentModel.DataAnnotations;

namespace ASI.MGC.FS.Models
{
    public class PurchaseModel
    {
        [Required]
        public string ApCode { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string DocDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string PurchaseDate { get; set; }
        [Required]
        public string ApCodeDetail { get; set; }

        [Required]
        public string Invoice { get; set; }
        [Required]
        public decimal ShippingCharges { get; set; }
        [Required]
        public decimal Discount { get; set; }
        public decimal VAT { get; set; }
    }
}