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
    
    public partial class vw_OfficeMonthlyReport
    {
        public string ProjectType { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> PIName { get; set; }
        public Nullable<decimal> BaseValue { get; set; }
        public Nullable<decimal> ApplicableTax { get; set; }
        public string PIDepartment { get; set; }
        public Nullable<decimal> Sanctionvalue { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public string FinancialYear { get; set; }
    }
}
