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
    
    public partial class tblProposalSupportDocuments
    {
        public int DocId { get; set; }
        public Nullable<int> ProposalId { get; set; }
        public string DocName { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentName { get; set; }
        public Nullable<int> DocType { get; set; }
        public Nullable<int> DocUploadUserid { get; set; }
        public Nullable<System.DateTime> DocUpload_TS { get; set; }
        public Nullable<bool> IsCurrentVersion { get; set; }
    }
}
