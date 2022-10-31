using System;
using System.Collections.Generic;
using IOAS.DataModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class ProjectFundingCategoryModel
    {
        
    }

    public class CreateROModel {
        /*RO List Region*/
        public int Sno { get; set; }
        public string RONumber { get; set; }
        public Nullable<decimal> ROProjValue { get; set; }
        public Nullable<decimal> ROBalanceValue { get; set; }
        public string Status { get; set; }
        public DateTime RODate { get; set; }
        public Nullable<int> ROId { get; set; }
        //public string PrsntDueDate { get; set; }
        public string PIdName { get; set; }
        public string BankName { get; set; }
        public string PIEmpId { get; set; }
        public string PIName { get; set; }
        public bool PFInit { get; set; }
        public Nullable<int> ROAprvId { get; set; }

        /*RO  Region*/
        public CoPiDetailsModel NameofPI { get; set; }
        public string ProjTitle { get; set; }
        public int ProjId { get; set; }
        public string  ProjectNumber { get; set; }
        public Nullable<decimal> SanctionValue { get; set; }
        public string ActualStartDate { get; set; }
        public string ActualCloseDate { get; set; }
        public string AgencyName { get; set; }
        public List<RODetailsListModel> RODetails { get; set; }
        public RODetailsListModel TempRODetails { get; set; }
        public Nullable<decimal> TotalEditedValue { get; set; }
        public Nullable<decimal> TotalNewValue { get; set; }

    }
    public class RODetailsListModel
    {
        
        public string RONumber { get; set; }
        public string TempRONumber { get; set; }
        public Nullable<decimal> ExistingValue { get; set; }
        public Nullable<decimal> EditedValue { get; set; }
        public Nullable<decimal> NewValue { get; set; }
        public int RO_Id { get; set; }

    }

    public class RODetailSearch
    {
        public string ProjectNumber { get; set; }
        public string RONumber { get; set; }
        public Nullable<decimal> ROExistingValue { get; set; }
        public Nullable<decimal> ROEditedValue { get; set; }
        public string Status { get; set; }
        public DateTime RODate { get; set; }
        public List<CreateROModel> list { get; set; }
        public int TotalRecords { get; set; }

        /*RO List Region for PI & Bank detail*/
        public string PIdName { get; set; }
        public string BankName { get; set; }
        public string PIEmpId { get; set; }
        public string PIName { get; set; }

    }



}