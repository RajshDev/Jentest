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
    
    public partial class tblRCTOTHPaymentDeductionUpload
    {
        public int OTHPaymentDeductionUploadId { get; set; }
        public string PaymentDeductionMonthYear { get; set; }
        public string Guid { get; set; }
        public string DocName { get; set; }
        public string ActualName { get; set; }
        public Nullable<System.DateTime> Crtd_Ts { get; set; }
        public Nullable<int> Crtd_By { get; set; }
        public string Status { get; set; }
        public Nullable<int> UpdtUser { get; set; }
        public Nullable<System.DateTime> UpdtTs { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
    }
}
