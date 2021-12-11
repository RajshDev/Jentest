using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class ProposalReportViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Department { get; set; }
        public string PI { get; set; }
        public string ProposalTitle { get; set; }
        public string SponsoringAgency { get; set; }
        public decimal ProposalValue { get; set; }
        public DateTime Inputdate { get; set; }
    }
}