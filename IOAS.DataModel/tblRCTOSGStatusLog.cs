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
    
    public partial class tblRCTOSGStatusLog
    {
        public int StatusLogID { get; set; }
        public Nullable<int> OSGID { get; set; }
        public string PresentStatus { get; set; }
        public string NewStatus { get; set; }
        public string preBy { get; set; }
        public Nullable<int> Crt_By { get; set; }
        public Nullable<System.DateTime> Crt_TS { get; set; }
        public Nullable<int> Upt_By { get; set; }
        public Nullable<System.DateTime> Upt_TS { get; set; }
        public string Message { get; set; }
    }
}
