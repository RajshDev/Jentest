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
    
    public partial class tblProjectFundingBody
    {
        public int FundingBodyId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> IndProjectFundingGovtBody { get; set; }
        public Nullable<int> IndProjectFundingNonGovtBody { get; set; }
        public Nullable<int> ForgnProjectFundingGovtBody { get; set; }
        public Nullable<int> ForgnProjectFundingNonGovtBody { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public Nullable<int> CrtdUserId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> UpdtUserId { get; set; }
        public Nullable<System.DateTime> UpdtTS { get; set; }
    }
}
