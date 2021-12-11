using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace IOAS.Models
{

    public class RolesModel
    {
        public int RoleID { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class ReportModulesModel
    {
        public int ModuleID { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }
    }

    public class SqlViewsModel
    {
        public string ID { get; set; }

        [Required]
        [Display(Name = "Table name")]
        public string name { get; set; }
    }

    public class SqlViewsPropertyModel
    {
        public string FieldId { get; set; }

        [Required]
        [Display(Name = "Field name")]
        public string fieldName { get; set; }

        public bool IsSelected { get; set; }
    }

    public class ReportFieldModel
    {
        public int ReportID { get; set; }

        [Required]
        [Display(Name = "Field name")]
        public string ReportField { get; set; }

        [Required]
        [Display(Name = "Aggregation")]
        public string Aggregation { get; set; }

        [Display(Name = "GroupBy")]
        public bool GroupBy { get; set; }

        [Display(Name = "OrderBy")]
        public bool OrderBy { get; set; }

    }

    public class FilterFieldModel
    {
        public int ReportID { get; set; }

        [Required]
        [Display(Name = "Field name")]
        public string ReportField { get; set; }

        [Required]
        [Display(Name = "FieldType")]
        public string FieldType { get; set; }

        [Display(Name = "RefTable")]
        public string RefTable { get; set; }

        [Display(Name = "RefField")]
        public string RefField { get; set; }

        [Display(Name = "IsRange")]
        public bool IsRange { get; set; }


        [Display(Name = "DType")]
        public string DType { get; set; }
    }

    public class SqlReportModel
    {

        public int ReportID { get; set; }

        [Required]
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }

        [Required]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [Display(Name = "Roles")]
        public List<RolesModel> AvailableRoles { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<RolesModel> SelectedRoles { get; set; }

        public List<ReportFieldModel> dtReportFields { get; set; }

        public List<FilterFieldModel> dtFilterFields { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public int ModuleID { get; set; }

        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime CRTD_TS { get; set; }

        public Boolean CanExport { get; set; }
        public Boolean ToExcel { get; set; }
        public Boolean ToPDF { get; set; }

    }
    //public class PagedData<T> where T : class
    //{
    //    public IEnumerable<T> Data { get; set; }
    //    public int TotalPages { get; set; }
    //    public int CurrentPage { get; set; }
    //    public int pageSize { get; set; }
    //    public int visiblePages { get; set; }
    //}

}