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
    
    public partial class vw_CommitmentReport
    {
        public string ProjectNumber { get; set; }
        public int ProjectId { get; set; }
        public string CommitmentNumber { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string CommitmentType { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> BookedValue { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> CommitmentDate { get; set; }
        public string TapalNo { get; set; }
    }
}
