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
    
    public partial class vw_TravelInvoice
    {
        public string InvoiceNumber { get; set; }
        public int TravelBillId { get; set; }
        public string BillNumber { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
