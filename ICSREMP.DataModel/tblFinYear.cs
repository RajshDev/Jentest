//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICSREMP.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblFinYear
    {
        public int FinYearId { get; set; }
        public string Year { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> CurrentYearFlag { get; set; }
    }
}
