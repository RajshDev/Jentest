//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IOAS.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblRCTOSGOtherDetail
    {
        public int OtherDetailsId { get; set; }
        public Nullable<int> OSGId { get; set; }
        public string OthersName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public Nullable<int> UpdtUser { get; set; }
        public Nullable<System.DateTime> UpdtTS { get; set; }
        public string Status { get; set; }
    }
}
