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
    
    public partial class tblRCTSTEDeviationCheckDetail
    {
        public int DeviationCheckDetailId { get; set; }
        public Nullable<int> STEID { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTs { get; set; }
        public Nullable<int> UptdUser { get; set; }
        public Nullable<System.DateTime> UptdTs { get; set; }
        public string Status { get; set; }
        public Nullable<int> DeviationCheckListId { get; set; }
        public Nullable<bool> isCurrentVersion { get; set; }
        public Nullable<bool> IsChecked { get; set; }
    }
}
