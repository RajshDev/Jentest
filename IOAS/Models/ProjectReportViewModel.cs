using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class ProjectReportViewModel
    {
       public int ProjecttypeId { get; set; }
        public int Projecttype { get; set; }
        public int ReportId { get; set; }
        [Required]
        [Display(Name = "Report Group")]
        public string Reportname { get; set; }
        public int Month { get; set; }
        public int year { get; set; }
        public int Departmentid { get; set; }
        public string PIDepartment { get; set; }
        public string Projectnumber { get; set; }
        public string Projecttitle { get; set; }
        public DateTime SanctionOrderDate { get; set; }
        public decimal SanctionValue { get; set; }
        public string Remarks { get; set; }
        public string PIName { get; set; }
        public string AgencyRegisteredName { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class monthmodel
    {
        public int yearid { get; set; }
        public int yearnumber { get; set; }
    }
    public class ProjectProposalReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ProposalType { get; set; }
        public string ProposalNumber { get; set; }
        public string FirstName { get; set; }
        public string ProposalTitle { get; set; }
       public string DepartmentCode { get; set; }
        public DateTime InwardDate{ get; set; }
        public DateTime Crtd_TS { get; set; }
        public int ProjectType { get; set; }
        public string Noofdays { get; set; }
    }
}