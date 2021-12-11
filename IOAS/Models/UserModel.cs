using System;
using System.Collections.Generic;
using System.Configuration;

namespace IOAS.Models
{
    public class EmailTemplate
    {

        public static string mailid = ConfigurationManager.AppSettings["maillId"];
        public static string password = ConfigurationManager.AppSettings["maillPassword"];
        public static string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"];
        //public static string gmailid = ConfigurationManager.AppSettings["gmaillId"];
        //public static string gmailpassword = ConfigurationManager.AppSettings["gmaillPassword"];
        //public static string gmailsmtp = ConfigurationManager.AppSettings["smtpAddress"];        
        public static string adminmaillId = ConfigurationManager.AppSettings["adminmaillId"];
        public static int portNumber = Convert.ToInt32(ConfigurationManager.AppSettings["portNumber"]);
        public static bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
        public static string errorIntimationMailID = ConfigurationManager.AppSettings["errorIntimationMailID"];
        public static string disclaimer = ConfigurationManager.AppSettings["disclaimercontent"];
    }
    public class PagedData<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int pageSize { get; set; }
        public int visiblePages { get; set; }
        public int TotalRecords { get; set; }
    }
    public class PagedList<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int pageSize { get; set; }
        public int visiblePages { get; set; }
        public int TotalRecords { get; set; }
    }
    public class MasterlistviewModel
    {
        public Nullable<int> id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }
    public class AutoCompleteModel
    {
        public string value { get; set; }
        public string label { get; set; }
        public string desc { get; set; }
        public string icon { get; set; }
    }
    public class CodeControllistviewModel
    {
        public int codevalAbbr { get; set; }
        public string CodeName { get; set; }
        public string CodeValDetail { get; set; }
    }
    public class CountryListViewModel
    {
        public Nullable<int> CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }

    public class MenuListViewModel
    {
        public int Moduleid { get; set; }
        public string Modulename { get; set; }
        public string ModuleIconName { get; set; }
        public List<submodulemenuviewmodel> submodule { get; set; }

    }
    public class submodulemenuviewmodel
    {
        public int Menugroupid { get; set; }
        public string Menugroupname { get; set; }
        public List<SubmenuViewModel> Submenu { get; set; }
    }
    public class SubmenuViewModel
    {
        public int FunctionId { get; set; }
        public string Functioname { get; set; }
        public string Actionname { get; set; }
        public string Controllername { get; set; }


    }

    public class RoleAccessDetailModel
    {
        public int RoleId { get; set; }
        public bool IsAdd { get; set; }
        public bool IsRead { get; set; }
        public bool IsApprove { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
    public class TempAdvlistviewModel
    {
        public Nullable<int> id { get; set; }
        public string name { get; set; }
        public string pidepartment { get; set; }
        public string PIname { get; set; }
        public Nullable<int> count { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string code { get; set; }
    }
    public class SearchViewModel
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string PIName { get; set; }
        public Nullable<decimal> EstimatedAmount { get; set; }
        public Nullable<decimal> AdvanceRequested { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }
        public int TotalRecords { get; set; }

        public List<TravelAdvanceModel> Travellist { get; set; }
        public List<OtherReceiptModel> Otherrecipt { get; set; }

    }
}