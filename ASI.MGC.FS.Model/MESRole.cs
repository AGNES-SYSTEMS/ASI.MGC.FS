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
    
    public partial class MESRole
    {
        public MESRole()
        {
            this.MESUserRoles = new HashSet<MESUserRole>();
        }
    
        public System.Guid RoleID { get; set; }
        public string RoleName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string isActive { get; set; }
    
        public virtual ICollection<MESUserRole> MESUserRoles { get; set; }
    }
}