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
    
    public partial class MESUserLoginDetail
    {
        public int ID { get; set; }
        public System.Guid UserID { get; set; }
        public System.DateTime LoginDate { get; set; }
        public System.TimeSpan LoginTime { get; set; }
        public string LoginDuration { get; set; }
        public string MacAddress { get; set; }
        public string Browser { get; set; }
        public bool IsMobileDevice { get; set; }
        public string MobileDeviceManufacturer { get; set; }
        public string MobileDeviceModal { get; set; }
        public string UserIdentityName { get; set; }
        public string UserHostAddress { get; set; }
        public string UserHostName { get; set; }
        public bool IsActive { get; set; }
    
        public virtual MESUser MESUser { get; set; }
    }
}
