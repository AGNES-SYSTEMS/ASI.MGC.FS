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
    
    public partial class MATERIALRECEIPTMASTER
    {
        public MATERIALRECEIPTMASTER()
        {
            this.JOBMASTERs = new HashSet<JOBMASTER>();
            this.MRVREFERENCEs = new HashSet<MRVREFERENCE>();
        }
    
        public string MRVNO_MRV { get; set; }
        public Nullable<System.DateTime> MRVDATE_MRV { get; set; }
        public string CUSTOMERCODE_MRV { get; set; }
        public string CUSTOMERNAME_MRV { get; set; }
        public string ADDRESS1_MRV { get; set; }
        public string ADDRESS2_MRV { get; set; }
        public string PHONE_MRV { get; set; }
        public Nullable<System.DateTime> DOC_DATE_MRV { get; set; }
        public Nullable<System.DateTime> DELE_DATE_MRV { get; set; }
        public string EXECODE_MRV { get; set; }
        public string NOTES_MRV { get; set; }
        public string STATUS_MRV { get; set; }
        public string PHONE2_MRV { get; set; }
        public string CUSTOMERVATNO_MRV { get; set; }
    
        public virtual ICollection<JOBMASTER> JOBMASTERs { get; set; }
        public virtual ICollection<MRVREFERENCE> MRVREFERENCEs { get; set; }
    }
}
