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
    
    public partial class tblTDSPayment
    {
        public int tblTDSPaymentId { get; set; }
        public string TDSPaymentNumber { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<int> Section { get; set; }
        public Nullable<decimal> TotalTDSIncomeTax { get; set; }
        public Nullable<decimal> TotalTDSGST { get; set; }
        public Nullable<System.DateTime> DateOfPayment { get; set; }
        public string ChellanNo { get; set; }
        public string BankReferenceNo { get; set; }
        public string Remark { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentActualName { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public string Status { get; set; }
        public Nullable<int> SubCode { get; set; }
        public Nullable<decimal> BankTransForIncometax { get; set; }
        public Nullable<decimal> TDStrans { get; set; }
        public Nullable<decimal> BankTransForGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> PaymentBank { get; set; }
    }
}
