namespace ASI.MGC.FS.Models
{
    public class CustomSaleDetails
    {
        public int SaleNo { get; set; }
        public string JobNo { get; set; }
        public string PrCode { get; set; }
        public string SwCode { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public double Rate { get; set; }
        public double CashAmount { get; set; }
        public double Discount { get; set; }
        public double ShipChrg { get; set; }
        public double ValueAddedTax { get; set; }
    }
}