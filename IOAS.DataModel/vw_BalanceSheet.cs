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
    
    public partial class vw_BalanceSheet
    {
        public int Id { get; set; }
        public string Accounts { get; set; }
        public string Groups { get; set; }
        public string AccountHead { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public int AccountGroupId { get; set; }
    }
}
