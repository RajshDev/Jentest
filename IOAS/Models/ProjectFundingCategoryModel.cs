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
        public CoPiDetailsModel NameofPI { get; set; }
        public string ProjTitle { get; set; }
        public int ProjId { get; set; }
        public string  ProjectNumber { get; set; }
        public decimal SanctionValue { get; set; }
        public string ActualStartDate { get; set; }
        public string ActualCloseDate { get; set; }
        public string AgencyName { get; set; }
        public List<RODetailsListModel> RODetails { get; set; }
        
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
}