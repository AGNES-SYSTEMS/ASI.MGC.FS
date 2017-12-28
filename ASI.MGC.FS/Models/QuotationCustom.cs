namespace ASI.MGC.FS.Models
{
    public class QuotationCustom
    {
        public string PrCode { get; set; }
        public string PrDesc { get; set; }
        public string JobId { get; set; }
        public string JobDesc { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
    }
}