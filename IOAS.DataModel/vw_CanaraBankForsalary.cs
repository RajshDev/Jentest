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
    
    public partial class vw_CanaraBankForsalary
    {
        public int BOAPaymentDetailId { get; set; }
        public int SalaryId { get; set; }
        public string ReferenceNumber { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<System.DateTime> ReferenceDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string FirstName { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> PayBill { get; set; }
    }
}
