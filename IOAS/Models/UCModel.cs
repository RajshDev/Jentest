using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Models
{
    public class UCModel : BuilderModel
    {
        public Nullable<Int32> TypeOfUC { get; set; }
        public Int32 UCId { get; set; }
        public Nullable<Int32> ProjectId { get; set; }

        public Nullable<decimal> TotalExpenditure { get; set; }
        public string ProjectNumber { get; set; }
        public string Remarks { get; set; }
        [AllowHtml]
        public string UCRawFile { get; set; }
        public Nullable<Int32> FinancialYear { get; set; }
        public List<UCCommitmentModel> CommitDetails { get; set; } = new List<UCCommitmentModel>();
        public List<UCExpenditureModel> ExpDetails { get; set; } = new List<UCExpenditureModel>();
    }
    public class BuilderModel
    {
        public Nullable<Int32> TemplateId { get; set; }
        public string TemplateName { get; set; }
        [AllowHtml]
        public string TemplateData { get; set; }
    }
    public class UCProjectInfoModel
    {
        public Nullable<decimal> TotalSanctionValue { get; set; }
        public Nullable<decimal> TotalReceipt { get; set; }
        public Nullable<decimal> FinYearTotalAllocationValue { get; set; }
        public Nullable<decimal> FinYearTotalReceipt { get; set; }
        public String PIName { get; set; }
        public List<String> CoPIName { get; set; }
    }
    public class UCCommitmentModel
    {
        public Nullable<Int32> CommitmentId { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<Int32> AllocationHeadId { get; set; }
        public string CommitmentNumber { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> AvailableBalance { get; set; }
    }

    public class UCExpenditureModel
    {
        public Nullable<Int32> AllocationHeadId { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<decimal> ExpenditureAsPerBook { get; set; }
        public Nullable<decimal> ExpenditureAsPerUC { get; set; }
        public Nullable<decimal> CommitmentTreatedAsExp { get; set; }
        public Nullable<decimal> UCCommitmentWithdrawn { get; set; }
        public Nullable<decimal> UCCommitmentUtilized { get; set; }
        public Nullable<decimal> UCCommitmentUnutilized { get; set; }

    }
    public class UCListModel
    {
        public Nullable<Int32> SlNo { get; set; }
        public Nullable<Int32> UCId { get; set; }
        public Nullable<Int32> ProjectId { get; set; }
        public Nullable<Int32> TypeofUC { get; set; }
        public string UCNumber { get; set; }
        public Nullable<DateTime> UCDate { get; set; }
        public string UCDateStr { get; set; }
        public string FinYear { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string ProjectNumber { get; set; }
        public bool IsEditable { get; set; }
    }
    public class SearchUCModel : CommonJSGridFilterModel
    {
        public Nullable<int> ProjectId { get; set; }
        public string UCNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string FinYear { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> UCFrom { get; set; }
        public Nullable<DateTime> UCTo { get; set; }
    }
    public class CommonJSGridFilterModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
    public class UCComponentModel
    {
        public IDictionary<string, object> basic { get; set; }
        public List<YearWiseUCComponentModel> finYearwise { get; set; }
        public List<YearWiseUCComponentModel> yearwise { get; set; }
    }
    public class YearWiseUCComponentModel
    {
        public IDictionary<string, object> yearMaster { get; set; }
        public List<YearWiseDetailUCComponentModel> yearDetail { get; set; }
    }
    public class YearWiseDetailUCComponentModel
    {
        public IDictionary<string, object> heads { get; set; }
    }
}