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
    
    public partial class sysschemaarticles
    {
        public int artid { get; set; }
        public string creation_script { get; set; }
        public string description { get; set; }
        public string dest_object { get; set; }
        public string name { get; set; }
        public int objid { get; set; }
        public int pubid { get; set; }
        public byte pre_creation_cmd { get; set; }
        public int status { get; set; }
        public byte type { get; set; }
        public byte[] schema_option { get; set; }
        public string dest_owner { get; set; }
    }
}
