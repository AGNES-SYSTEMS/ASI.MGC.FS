//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASI.MGC.FS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRODUCTMASTER
    {
        public PRODUCTMASTER()
        {
            this.MRVREFERENCEs = new HashSet<MRVREFERENCE>();
        }
    
        public string PROD_CODE_PM { get; set; }
        public string DESCRIPTION_PM { get; set; }
        public Nullable<decimal> CUR_QTY_PM { get; set; }
        public Nullable<decimal> RATE_PM { get; set; }
        public Nullable<decimal> SELLINGPRICE_RM { get; set; }
        public string STATUS_PM { get; set; }
        public string PURCHSEUNIT_PM { get; set; }
        public string SALESUNIT_PM { get; set; }
        public string UNIT_PR { get; set; }
    
        public virtual ICollection<MRVREFERENCE> MRVREFERENCEs { get; set; }
    }
}
