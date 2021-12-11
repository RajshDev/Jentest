using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class ProjectExpReportSPprocessModel
    {
        public string status { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
    }
    public class TapalReportViewModel
    {
       
        public DateTime fromdate { get; set; }
       
        public DateTime todate { get; set; }
        public int departmentid { get; set; }
        public int roleid { get; set; }
        public int id { get; set; }
        
        public DateTime InwardDateTime { get; set; }
        public string TapalAction { get; set; }
        public string OutwardDateTime { get; set; }
        public string MarkTo { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public int TapalId { get; set; }
    }
    public class DailyBalanceVerificationModel
    {
        public Nullable<DateTime> Date { get; set; }
        public int Type { get; set; }
        public string Project { get; set; }
        public int ProjectId { get; set; }
        public string ProjectNo { get; set; }
        public string PI { get; set; }
        public string TypeName { get; set; }
        public decimal AvailBalance { get; set; }
        public decimal TotalAvailBalance { get; set; }
        public decimal TotalSanction { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal TotalNegativeBalance { get; set; }
        public decimal TotalExpent { get; set; }
        public decimal TotalCommitment { get; set; }
        public decimal OB { get; set; }
        public string CurrDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Receipt { get; set; }
        public decimal Commitment { get; set; }
        public decimal NegativeBalance { get; set; }
        public decimal Expent { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal PreviousCommitment { get; set; }
        public decimal ApprovedNegativeBalance { get; set; }
        public decimal NetBalance { get; set; }
        public decimal TotalDistribution { get; set; }
        public decimal Distribution { get; set; }
        public string ProjClassification { get; set; }
        public string ReportDate { get; set; }
        public decimal TlOB { get; set; }

    }
}