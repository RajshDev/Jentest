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
    
    public partial class tblRCTConsultantStatuslog
    {
        public int CONSStatusLogID { get; set; }
        public int CONSAppID { get; set; }
        public string CurrentStatus { get; set; }
        public string NewStatus { get; set; }
        public Nullable<int> Crtd_By { get; set; }
        public Nullable<System.DateTime> Crtd_Ts { get; set; }
        public Nullable<int> Uptd_By { get; set; }
        public Nullable<System.DateTime> Uptd_Ts { get; set; }
        public string Remarks { get; set; }
    }
}