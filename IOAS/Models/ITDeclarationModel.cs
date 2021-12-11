using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class EmpITDeclarationModel : CommonPaymentModel
    {
        public int SectionID { get; set; }
        public int DeclarationID { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string Documentpath { get; set; }
        public string EmpNo { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }
        public string Particulars { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal Amount { get; set; }
        public int Age { get; set; }
        public System.DateTime SubmittedOn { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string FinPeriod { get; set; }

    }

    public class EmpITOtherIncomeModel
    {
        public string Remarks { get; set; }
        public string Particulars { get; set; }
        public int ID { get; set; }
        public decimal EligibleAmount { get; set; }
        public decimal Amount { get; set; }
        public int EmpId { get; set; }
        public string EmpNo { get; set; }
        public DateTime SubmittedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class EmpITDeductionModel : CommonPaymentModel
    {
        public string Remarks { get; set; }
        public AdhocEmployeeModel EmpInfo { get; set; }
        public List<EmpITDeclarationModel> ItList { get; set; }
        public List<EmpITDeclarationDocModel> ItDocs { get; set; }
        public List<EmpITSOPModel> ItSOP { get; set; }
        public List<EmpITOtherIncomeModel> ItOtherIncome { get; set; }

        public string FinPeriod { get; set; }

        public string errMsg { get; set; }
        public List<AgencyStaffDetailsModel> SalaryDet { get; set; }
        public List<AgencyStaffDetailsModel> SupplemSalaryDet { get; set; }
        public int FinancialYear { get; set; }

    }
    public class ITDeclarationModel
    {
        public int DeclarationID { get; set; }
        public int SNO { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }
        public string Particulars { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal Amount { get; set; }
        public int Age { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }

   
    public class EmpITDeclarationDocModel
    {
        public int DocumentId { get; set; }
        public int DeclarationID { get; set; }
        public int SectionID { get; set; }
        public string DocumentName { get; set; }
        public string Documentpath { get; set; }
        public string EmpNo { get; set; }
        public DateTime SubmittedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class EmpITSOPModel
    {
        public int ID { get; set; }
        public Nullable<decimal> EligibleAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpNo { get; set; }
        public Nullable<System.DateTime> SubmittedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string LenderName { get; set; }
        public string LenderPAN { get; set; }
    }
    public class ProjectionModel
    {
        public List<AgencyStaffDetailsModel> SalaryDet { get; set; }
        public List<AgencyStaffDetailsModel> SupplemSalaryDet { get; set; }

    }
    public class ITDeclarationViewModel
    {
        public List<ITDeclarationModel> declaration { get; set; }
        public List<EmpITOtherIncomeModel> otherIncome { get; set; }
    }

}