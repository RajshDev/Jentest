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
    
    public partial class syncobj_0x4235333444354544
    {
        public int AgencyDocumentId { get; set; }
        public Nullable<int> AgencyId { get; set; }
        public string AgencyDocument { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentName { get; set; }
        public Nullable<int> DocumentType { get; set; }
        public Nullable<bool> IsCurrentVersion { get; set; }
        public Nullable<int> DocumentUploadUserId { get; set; }
        public Nullable<System.DateTime> DocumentUpload_Ts { get; set; }
    }
}
