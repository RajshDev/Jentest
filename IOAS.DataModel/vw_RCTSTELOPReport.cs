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
    
    public partial class vw_RCTSTELOPReport
    {
        public string EMPLOYEE_ID { get; set; }
        public string NAME { get; set; }
        public string DESIGNATION { get; set; }
        public Nullable<int> NO_OF_DAYS_LOP { get; set; }
        public Nullable<System.DateTime> LOG_TIME { get; set; }
        public string CREATED_BY { get; set; }
        public string Requested_by { get; set; }
        public Nullable<decimal> WITHDRAWN_COMMITMENT_AMOUNT { get; set; }
        public Nullable<System.DateTime> COMMITMENT_BOOKING___WITHDRAWAL_DATE { get; set; }
        public Nullable<System.DateTime> REQUEST_RECEIVED_DATE { get; set; }
        public Nullable<System.DateTime> COMPLETED__DATE { get; set; }
        public string COMPLETED_BY { get; set; }
    }
}