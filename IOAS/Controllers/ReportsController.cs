using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using IOAS.GenericServices;
using IOAS.Models;
using IOAS.Filter;
using DataAccessLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using IOAS.Infrastructure;
using Newtonsoft.Json;
using IOAS.DataModel;
using System.Data.SqlClient;
using Spire.Xls;
using System.Globalization;
using System.Web;
using System.Collections;

namespace IOAS.Controllers
{
    [Authorized]
    public class ReportsController : Controller
    {
        private AdminService adminService = new AdminService();
        private CoreAccountsService coreaccountService = new CoreAccountsService();
        public ActionResult List()
        {
            try
            {
                //int page, int pageSize
                //int skiprec = 0;
                //if (page == 1)
                //{
                //    skiprec = 0;
                //}
                //else
                //{
                //    skiprec = (page - 1) * pageSize;
                //}
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

                var result = Converter.DataTableToDict(dtReport);
                var model = Converter.GetEntities<SqlReportModel>(dtReport);
                //  model= model.Skip(skiprec).Take(pageSize).ToList();
                var reports = new PagedData<SqlReportModel>();
                reports.Data = model;
                return View(reports);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult GetList( int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        int skiprec = 0;
        //        if (pageIndex == 1)
        //        {
        //            skiprec = 0;
        //        }
        //        else
        //        {
        //            skiprec = (pageIndex - 1) * pageSize;
        //        }
        //        ListDatabaseObjects db = new ListDatabaseObjects();
        //        DataSet dsReport = db.getReportDetails(-1);
        //        DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
        //        var result = Converter.DataTableToDict(dtReport);
        //        var model = Converter.GetEntities<SqlReportModel>(dtReport);
        //        var Count = model.Count();
        //        model = model.Skip(skiprec).Take(pageSize).ToList();
        //        var results = new { model = model, Count = Count };
        //        return Json(results, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public JsonResult GetList()
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                var result = Converter.DataTableToDict(dtReport);
                var model = Converter.GetEntities<SqlReportModel>(dtReport);
                //var Count = model.Count();
                // model = model.Skip(skiprec).Take(pageSize).ToList();
                var results = model;
                return Json(results, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ReportBuilder()
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            SqlReportModel model = new SqlReportModel();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();

            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            //ViewBag.Tables = vwList;
            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = Converter.GetEntityList<SqlViewsModel>(dt);
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();
            //model.AvailableFields = vwFields;
            //model.SelectedFields = new List<SqlViewsPropertyModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            return View("ReportBuilder", model);
        }


        [Authorize]
        public ActionResult EditReport(int ReportID)
        {
            SqlReportModel model = new SqlReportModel();

            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = vwList;
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            if (ReportID > 0)
            {
                DataSet dsReport = db.getReportDetails(ReportID);
                var result = new { };
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    var dtReport = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                    var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                    var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                    var dtSelectedRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                    var dtRoles = Converter.GetEntities<RolesModel>(dsReport.Tables[4]);
                    DataRow report = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                    model = Converter.DataRowTonEntity<SqlReportModel>(report);
                    model.dtReportFields = dtFields;
                    model.dtFilterFields = dtFilter;
                    ViewBag.SelectedRoles = dtSelectedRoles;
                    //Note: Exclude already selected roles from available roles.
                    ViewBag.Roles = new MultiSelectList(dtRoles, "RoleID", "RoleName"); ;
                }
            }

            return View(model);
        }


        public ActionResult ReportMenu(string ReportName)
        {
            var user = User.Identity.Name;
            if (ReportName == null)
            {
                ReportName = "";
            }
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(-1, user, ReportName);
            DataTable dtReport = (dsReport != null && dsReport.Tables.Count > 0) ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View(model);
        }

        [Authorize]
        public ActionResult ReportViewer()
        {
            int ReportID = Convert.ToInt32(@Request.QueryString["ReportID"].ToString());
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                if (Convert.ToInt32(row["ReportID"].ToString()) == ReportID)
                {
                    ViewBag.ReportName = row["ReportName"].ToString();
                }

                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View("ReportViewer", model);
        }

        [Authorize]
        public ActionResult ReportView(int ReportID)
        {
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                if (Convert.ToInt32(row["ReportID"].ToString()) == ReportID)
                {
                    ViewBag.ReportName = row["ReportName"].ToString();
                }
                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View("ReportViewer", model);
        }


        [HttpPost]
        public ActionResult ReportViewer(SqlReportModel model)
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            var dbView = db.getAllViews();
            DataTable dt = db.getAllViews();
            SqlViewsModel vwModel = new SqlViewsModel();
            List<SqlViewsPropertyModel> vwPropertyList = new List<SqlViewsPropertyModel>();

            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            foreach (DataRow row in dt.Rows)
            {
                vwList.Add(new SqlViewsModel
                {
                    ID = row["ID"].ToString(),
                    name = row["name"].ToString()
                });

            }
            ViewBag.Tables = vwList;
            ViewBag.Reports = dt;
            //WebGridClass.GetDetailsForGrid(vwList, "View List", "ID");
            return View("ReportViewer", model);
        }

        [AcceptVerbs(HttpVerbs.Get)]

        [Authorize]
        public JsonResult getReportListByUser(string ReportName)
        {
            try
            {
                var user = User.Identity.Name;
                var ReportID = -1;
                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportByUser(ReportID, user, ReportName);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

                var result = dtReport != null ? Converter.DataTableToDict(dtReport) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult getReportDetails(int ReportID)
        {
            try
            {
                var user = User.Identity.Name;

                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportDetails(ReportID);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                dynamic filters = new Dictionary<string, Object>();
                if (dtReport.Rows.Count > 0)
                {
                    var data = db.pagingReportView(ReportID, "", "", 1, 10);

                    if (dsReport != null && dsReport.Tables.Count > 0)
                    {
                        var report = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                        var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                        var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                        var dtRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                        DataRow drReport = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                        model = Converter.DataRowTonEntity<SqlReportModel>(drReport);
                        model.dtReportFields = dtFields;
                        model.dtFilterFields = dtFilter;
                        model.SelectedRoles = dtRoles;

                        DataTable dtFilters = dsReport.Tables[2];

                        for (int i = 0; i < dtFilters.Rows.Count; i++)
                        {
                            if (dtFilters.Rows[i]["FieldType"].ToString() == "Dropdown")
                            {
                                var key = dtFilters.Rows[i]["ReportField"].ToString();
                                var dtResultField = db.getFilterDetails(ReportID, key);
                                if (dtResultField != null)
                                {
                                    filters[key] = Converter.DataTableToDict(dtResultField);
                                }

                            }
                        }
                    }
                    var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                    var resultJson = new { result = result, recordCount = data.Item2, schema = model, filters = filters };
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = new List<Dictionary<string, object>>(), recordCount = 0, schema = model, filters = filters }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult getReportDetailByUser(int ReportID)
        {
            try
            {
                var user = User.Identity.Name;

                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportByUser(ReportID, user, "");
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                dynamic filters = new Dictionary<string, Object>();
                if (dtReport.Rows.Count > 0)
                {
                    var data = db.pagingReportView(ReportID, "", "", 1, 10);

                    if (dsReport != null && dsReport.Tables.Count > 0)
                    {
                        var report = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                        var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                        var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                        var dtRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                        DataRow drReport = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                        model = Converter.DataRowTonEntity<SqlReportModel>(drReport);
                        model.dtReportFields = dtFields;
                        model.dtFilterFields = dtFilter;
                        model.SelectedRoles = dtRoles;

                        DataTable dtFilters = dsReport.Tables[2];

                        for (int i = 0; i < dtFilters.Rows.Count; i++)
                        {
                            if (dtFilters.Rows[i]["FieldType"].ToString() == "Dropdown")
                            {
                                var key = dtFilters.Rows[i]["ReportField"].ToString();
                                var dtResultField = db.getFilterDetails(ReportID, key);
                                if (dtResultField != null)
                                {
                                    filters[key] = Converter.DataTableToDict(dtResultField);
                                }

                            }
                        }
                    }
                    var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                    var resultJson = new { result = result, recordCount = data.Item2, schema = model, filters = filters };
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = new List<Dictionary<string, object>>(), recordCount = 0, schema = model, filters = filters }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult searchReportDetails(int ReportID, string condition)
        {
            try
            {
                var user = User.Identity.Name;
                var criteria = "";
                int pageSize = 10;
                int pageIndex = 1;
                Dictionary<string, string> dtBetween = new Dictionary<string, string>();
                if (condition != null)
                {
                    var filter = System.Web.Helpers.Json.Decode(condition);
                    var properties = ((System.Web.Helpers.DynamicJsonObject)filter).GetDynamicMemberNames();
                    var key = "";
                    var val = "";
                    foreach (var item in properties)
                    {
                        if (item.IndexOf("ddl_") > -1 && criteria == "")
                        {
                            criteria = item.Replace("ddl_", "") + " = '" + filter[item] + "'";
                        }
                        else if (item.IndexOf("ddl_") > -1 && criteria != "")
                        {
                            criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + filter[item] + "'";
                        }

                        if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1)
                        {
                            key = item.Replace("txt", "").Replace("_from", "");
                            val = item.Replace("txt", "").Replace("_from", "") + " between '" + filter[item] + "' and '";
                            dtBetween.Add(key, val);
                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1)
                        {
                            key = item.Replace("txt", "").Replace("_to", "");
                            val = filter[item] + "'";
                            if (dtBetween[key] != null)
                            {
                                dtBetween[key] = dtBetween[key] + val;
                            }

                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1 && filter[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                        {
                            key = item.Replace("txt", "").Replace("_from", "");
                            val = "convert(varchar(12), " + item.Replace("txt", "").Replace("_from", "") + ", 103)" + " between '" + filter[item] + "' and '";
                            dtBetween.Add(key, val);
                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1 && filter[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                        {
                            key = item.Replace("txt", "").Replace("_to", "");
                            val = filter[item] + "'";
                            if (dtBetween[key] != null)
                            {
                                dtBetween[key] = dtBetween[key] + val;
                            }

                        }

                        else if (item.IndexOf("txt") > -1 && criteria == "")
                        {
                            criteria = item.Replace("txt", "") + " like '%" + filter[item] + "%'";
                        }
                        else if (item.IndexOf("txt") > -1 && criteria != "")
                        {
                            criteria = criteria + " and " + item.Replace("txt", "") + " like '%" + filter[item] + "%'";
                        }
                        else if (item == "pageSize")
                            pageSize = Convert.ToInt32(filter[item]);
                        else if (item == "pageIndex")
                            pageIndex = Convert.ToInt32(filter[item]);
                    }
                }

                foreach (var item in dtBetween.Keys)
                {
                    //criteria = criteria + dtBetween[item];
                    criteria = String.IsNullOrEmpty(criteria) ? criteria + dtBetween[item] : criteria + " and " + dtBetween[item];
                }

                ListDatabaseObjects db = new ListDatabaseObjects();
                //DataTable dtResult = new DataTable();
                //dtResult = db.getReportView(ReportID, criteria, user);
                var data = db.pagingReportView(ReportID, criteria, user, pageIndex, pageSize);
                var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result, recordCount = data.Item2 };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //private bool validateDate(string strDate)
        //{
        //    bool isValid = false;
        //    string[] format = new string[] { "dd-mm-yyyy", "dd/mm/yyyy" };
        //    foreach(var item in format)
        //    {
        //        if (DateTime.TryParseExact(strDate, item, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        //        {
        //            isValid = true;
        //        }
        //    }


        //    return isValid;
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getTables()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllTables();
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getTablesAndViews()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllTablesAndViews();
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFieldDetails(tableName);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getRefFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                string objType = db.getObjectType(tableName) != "VIEW" ? "" : "view";
                DataTable dtFields = db.getAllProperties(tableName, objType);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFilterDetails(int ReportID, string ReportField)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFilterDetails(ReportID, ReportField);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ValidateAntiForgeryToken]
        public JsonResult getRoles()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                var roles = AdminService.GetRoles();
                return Json(roles, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveReportData(SqlReportModel model)
        {
            var user = User.Identity.Name;
            var msg = "";
            if (model.ReportName == null || model.ReportName == "")
            {
                msg = "Report Name is missing";
            }
            else if (model.ModuleID == 0)
            {
                msg = "Module Name is missing";
            }
            else if (model.TableName == null || model.TableName == "")
            {
                msg = "Table Name is missing";
            }
            else if (model.SelectedRoles == null || model.SelectedRoles.Count == 0)
            {
                msg = "Please select roles";
            }
            else if (model.dtReportFields == null || model.dtReportFields.Count == 0)
            {
                msg = "Please select report fields(atleast one)";
            }
            if (msg != "")
            {
                var error = new { msg = msg };
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            ListDatabaseObjects db = new ListDatabaseObjects();
            ReportsProfileHandler prop = new ReportsProfileHandler();
            prop.ReportID = model.ReportID;
            prop.ReportName = model.ReportName;
            prop.ReportDescription = "";
            prop.TableName = model.TableName;

            prop.dtReportFields = Converter.ToDataTable<ReportFieldModel>(model.dtReportFields);
            prop.dtFilterFields = Converter.ToDataTable<FilterFieldModel>(model.dtFilterFields);
            prop.dtRoles = Converter.ToDataTable<RolesModel>(model.SelectedRoles);

            prop.IsActive = true;
            prop.RoleId = 0;
            prop.ModuleId = model.ModuleID;
            prop.UserId = AdminService.getUserByName(User.Identity.Name);
            prop.CanExport = model.CanExport;
            prop.ToPDF = model.ToPDF;
            prop.ToExcel = model.ToExcel;

            var reportId = db.AddReportDetails(prop);
            DataSet dsReport = db.getReportDetails(reportId);
            var result = new { };
            if (dsReport != null && dsReport.Tables.Count > 0)
            {
                var dtReport = Converter.DataTableToDict(dsReport.Tables[0]);
                var dtFields = Converter.DataTableToDict(dsReport.Tables[1]);
                var dtFilter = Converter.DataTableToDict(dsReport.Tables[2]);
                var dtRoles = Converter.DataTableToDict(dsReport.Tables[3]);
                var report = (dtReport != null && dtReport.Count > 0) ? dtReport[0] : null;

                var resultJson = new { Report = report, Fields = dtFields, Filter = dtFilter, Roles = dtRoles };

                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public FileStreamResult ExportData(int ReportID, string filter, string filetype)
        {
            var user = User.Identity.Name;

            var data = System.Web.Helpers.Json.Decode(filter);
            Dictionary<string, string> dtBetween = new Dictionary<string, string>();
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dtResult = new DataTable();
            bool CanExport = false;
            bool ToPDF = false;
            bool ToExcel = false;
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                CanExport = Convert.ToBoolean(dtReport.Rows[0]["CanExport"]);
                ToPDF = Convert.ToBoolean(dtReport.Rows[0]["ToPDF"]);
                ToExcel = Convert.ToBoolean(dtReport.Rows[0]["ToExcel"]);
            }
            var criteria = "";
            var properties = ((System.Web.Helpers.DynamicJsonObject)data).GetDynamicMemberNames();
            var key = "";
            var val = "";
            foreach (var item in properties)
            {
                if (item.IndexOf("ddl_") > -1 && criteria == "")
                {
                    criteria = item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                else if (item.IndexOf("ddl_") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }

                if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1)
                {
                    key = item.Replace("txt", "").Replace("_from", "");
                    val = item.Replace("txt", "").Replace("_from", "") + " between '" + data[item] + "' and '";
                    dtBetween.Add(key, val);
                }
                else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1)
                {
                    key = item.Replace("txt", "").Replace("_to", "");
                    val = data[item] + "'";
                    if (dtBetween[key] != null)
                    {
                        dtBetween[key] = dtBetween[key] + val;
                    }

                }
                else if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1 && data[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                {
                    key = item.Replace("txt", "").Replace("_from", "");
                    val = "convert(varchar(12), " + item.Replace("txt", "").Replace("_from", "") + ", 103)" + " between '" + data[item] + "' and '";
                    dtBetween.Add(key, val);
                }
                else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1 && data[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                {
                    key = item.Replace("txt", "").Replace("_to", "");
                    val = data[item] + "'";
                    if (dtBetween[key] != null)
                    {
                        dtBetween[key] = dtBetween[key] + val;
                    }

                }
                else if (item.IndexOf("txt") > -1 && criteria == "")
                {
                    criteria = item.Replace("txt", "") + " like '%" + data[item] + "%'";
                }
                else if (item.IndexOf("txt") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("txt", "") + " like '%" + data[item] + "%'";
                }
            }
            foreach (var item in dtBetween.Keys)
            {
                //criteria = criteria + dtBetween[item];
                criteria = String.IsNullOrEmpty(criteria) ? criteria + dtBetween[item] : criteria + " and " + dtBetween[item];
            }
            string rName = Common.GetReportName(ReportID);
            dtResult = db.getReportView(ReportID, criteria, user);
            if (ToPDF && filetype == "topdf")
            {
                return toPdf(dtResult);
            }
            else if (ToExcel && filetype == "toexcel")
            {
                //return coreaccountService.toSpreadSheet(dtResult, rName);
                return coreaccountService.ExportToExcel(dtResult, rName);
            }
            else
            {
                MemoryStream workStream = new MemoryStream();
                Document document = new Document();
                PdfWriter.GetInstance(document, workStream).CloseStream = false;
                string msg = "Sorry, You are not authorized to perform this action. Please contact your administrator.";

                document.Open();
                document.Add(new Paragraph(msg));
                document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return new FileStreamResult(workStream, "application/pdf");
            }

        }

        [HttpPost]
        [Authorize]
        public FileStreamResult ExportTo(int ReportID, string filter)
        {
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dtResult = new DataTable();
            var data = (filter == null) ? new { } : System.Web.Helpers.Json.Decode(filter);

            var criteria = "";
            var properties = ((System.Web.Helpers.DynamicJsonObject)data).GetDynamicMemberNames();
            foreach (var item in properties)
            {
                if (item.IndexOf("ddl_") > -1 && criteria == "")
                {
                    criteria = item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                else if (item.IndexOf("ddl_") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                if (item.IndexOf("txt") > -1 && criteria == "")
                {
                    criteria = item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
                else if (item.IndexOf("txt") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
            }
            dtResult = db.getReportView(ReportID, criteria, user);

            return toPdf(dtResult);
        }

        public FileStreamResult toPdf(DataTable dtReport)
        {
            if (dtReport != null)
            {
                string[] columnNames = (from dc in dtReport.Columns.Cast<DataColumn>()
                                        select dc.ColumnName).ToArray();
                int count = columnNames.Length;
                object[] array = new object[count];
                int cols = dtReport.Columns.Count;
                int rows = dtReport.Rows.Count;

                MemoryStream workStream = new MemoryStream();
                Document document = new Document();
                PdfWriter.GetInstance(document, workStream).CloseStream = false;

                document.Open();
                PdfPTable table = new PdfPTable(cols + 1);

                PdfPCell cellSNO = new PdfPCell(new Phrase("S.No", new Font(Font.FontFamily.HELVETICA, 14F)));
                cellSNO.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#cccccc"));
                cellSNO.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellSNO);
                var total = 0;
                var widths = new int[cols + 1];
                var AllColwidths = new float[cols + 1];
                Font font = new Font(Font.FontFamily.HELVETICA, 14F);
                var colWidth = font.GetCalculatedBaseFont(true).GetWidth("S.NO");
                total += colWidth;
                widths[0] = colWidth;
                for (int i = 0; i < cols; i++)
                {
                    var w = font.GetCalculatedBaseFont(true).GetWidth(dtReport.Columns[i].ColumnName);
                    total += w;
                    widths[i + 1] = w;
                }

                for (int i = 0; i < widths.Length; i++)
                {
                    AllColwidths[i] = (float)widths[i] / total * 100;
                }
                table.SetWidths(AllColwidths);

                //creating table headers  
                for (int i = 0; i < cols; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dtReport.Columns[i].ColumnName, new Font(Font.FontFamily.HELVETICA, 14F)));
                    cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#cccccc"));

                    //cell.Colspan = 3;
                    //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    //cell.BorderColor = new BaseColor(System.Drawing.Color.Red); //Style
                    //cell.Border = Rectangle.BOTTOM_BORDER; // | Rectangle.TOP_BORDER;
                    //cell.BorderWidthBottom = 3f;

                    //PdfPCell cellCols = new PdfPCell();
                    //Chunk chunkCols = new Chunk();
                    //cellCols.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#548B54"));
                    //Font ColFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);

                    //chunkCols = new Chunk(dtReport.Columns[i].ColumnName, ColFont);
                    //cellCols.Chunks.Add(chunkCols);

                    table.AddCell(cell);
                }


                var result = new float[cols];
                for (int k = 0; k < rows; k++)
                {
                    PdfPCell cellSNo = new PdfPCell(new Phrase((k + 1).ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    string color = (k % 2 == 0) ? "#ffffff" : "#cccccc";
                    cellSNo.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(color));
                    table.AddCell(cellSNo);
                    for (int j = 0; j < cols; j++)
                    {
                        PdfPCell cellRows = new PdfPCell(new Phrase(dtReport.Rows[k][j].ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                        cellRows.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(color));
                        Font RowFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        Chunk chunkRows = new Chunk(dtReport.Rows[k][j].ToString(), RowFont);
                        //cellRows.Chunks.Add(chunkRows);
                        cellRows.AddElement(chunkRows);
                        table.AddCell(cellRows);
                    }
                }
                Paragraph header = new Paragraph("List", FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD));
                PdfPCell headrCell = new PdfPCell(header);
                PdfPTable headerTbl = new PdfPTable(1);
                //headerTbl.TotalWidth = 300;
                headerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                headrCell.Border = 0;
                headrCell.PaddingLeft = 10;
                var Colwidth = new float[1];
                Colwidth[0] = total * 100;
                headerTbl.SetWidths(Colwidth);
                headerTbl.AddCell(headrCell);

                document.Add(headerTbl);
                document.Add(table);
                document.Close();

                //document.Open();
                //document.Add(new Paragraph("Hello World"));
                //document.Add(new Paragraph(DateTime.Now.ToString()));
                //document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return new FileStreamResult(workStream, "application/pdf");
            }
            else
            {
                return new FileStreamResult(new MemoryStream(), "application/pdf");
            }


        }

        #region CanaraBankReport
        public FileStreamResult CanaraBankReport(string Id)
        {
            try
            {
                Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                // dtResult.Columns.Add("Debting Account No");
                //dtResult.Columns.Add("12345678");
                //dtResult.Columns.Add("Ordering Customer Name");
                //dtResult.Columns.Add("ICSR");
                //dtResult.Columns.Add("Address");
                //dtResult.Columns.Add("IITM");

                dtResult = db.GetCanaraBankDetails(Id);
                return coreaccountService.toSpreadSheet(dtResult);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region ReceiptReport
        public FileStreamResult ReceiptReport(string ProjectNo)
        {
            try
            {
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetReceiptDetails(ProjectNo);
                return coreaccountService.toSpreadSheet(dtResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region FinalOutstandigReport
        public FileStreamResult FinalOutstandigReport(DateTime FromDate, DateTime ToDate)
        {
            try
            {

                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetFinalOutstandigReport(FromDate, ToDate);
                return coreaccountService.toSpreadSheet(dtResult);



                //  ReportService reportservice = new ReportService();
                // List<VendorReportModel> model = new List<VendorReportModel>();
                //  model = ReportService.GetFinalOutstandigReport();
                //  DataSet dataset = new DataSet();
                //  DataTable dtColumns = new DataTable();
                // string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                //  dtColumns = JsonConvert.DeserializeObject<DataTable>(json);
                // var adapter = new System.Data.SqlClient.SqlDataAdapter();
                // adapter.Fill(dataset);
                // dtColumns = dataset.Tables[0];
                // dtColumns = dataset.Tables[1];

                //dataset.Tables[0].TableName = "Invoice Vendor Details";
                //return toSpreadSheet(dtColumns);
                //dataset.Tables[1].TableName = "UnRegisterVendor";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region AgencyReport
        public FileStreamResult AgencyReport(DateTime FromDate, DateTime ToDate, int Category)
        {
            try
            {

                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dtResult = new DataSet();
                dtResult = db.GetAgencyReport(FromDate, ToDate, Category);
                return toSpreadSheets(dtResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public FileStreamResult toSpreadSheets(DataSet dataset)
        {
            MemoryStream workStream = new MemoryStream();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);


            if (dataset != null)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataset.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    //wb.Worksheets.Add(dtReport, "Customers");
                    wb.SaveAs(workStream);
                    workStream.Position = 0;

                }
            }
            return new FileStreamResult(workStream, "application/vnd.ms-excel");
        }
        public ActionResult GenerateCanaraBank(string Id)
        {
            try
            {
                Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetCanaraBankDetails(Id);
                var TransactionType = "";
                var BeneficiaryIFSCCode = "";
                var BeneficiaryAccountNo = "";
                var BeneficiaryName = "";
                var BeneAddressLine1 = "";
                var BeneAddressLine2 = "";
                var BeneAddressLine3 = "";
                var BeneAddressLine4 = "";
                var TxnRefNo = "";
                var Amount = "";
                var SendertoReceiverInfo = "";
                var AddInfo1 = "";
                var AddInfo2 = "";
                var AddInfo3 = "";
                var AddInfo4 = "";
                var DebitAccNo = "";
                foreach (DataRow row in dtResult.Rows)
                {
                    TransactionType = row["TransactionType"].ToString();
                    BeneficiaryIFSCCode = row["BeneficiaryIFSCCode"].ToString();
                    BeneficiaryAccountNo = row["BeneficiaryAccountNo"].ToString();
                    BeneficiaryName = row["BeneficiaryName"].ToString();
                    BeneAddressLine1 = row["BeneAddressLine1"].ToString();
                    BeneAddressLine2 = row["BeneAddressLine2"].ToString();
                    BeneAddressLine3 = row["BeneAddressLine3"].ToString();
                    BeneAddressLine4 = row["BeneAddressLine4"].ToString();
                    TxnRefNo = row["TxnRefNo"].ToString();
                    Amount = row["Amount"].ToString();
                    SendertoReceiverInfo = row["SendertoReceiverInfo"].ToString();
                    AddInfo1 = row["AddInfo1"].ToString();
                    AddInfo2 = row["AddInfo2"].ToString();
                    AddInfo3 = row["AddInfo3"].ToString();
                    AddInfo4 = row["AddInfo4"].ToString();
                    DebitAccNo = row["AccountNumber"].ToString();
                }
                Microsoft.Office.Interop.Excel.Application Extract = new Microsoft.Office.Interop.Excel.Application();
                if (Extract == null)
                {
                    ViewBag.errMsg = "Excel is not properly installed!!";
                    return RedirectToAction("ProjectExpenditure", "Manage");
                }
                int LastRecNo = 0;
                Microsoft.Office.Interop.Excel.Range Range;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlWorkBook = Extract.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                int Row = 2;
                int sno = 1;
                int PrjNo = 1;
                xlWorkSheet.Cells[Row, 1] = "Debting Acc.No :";
                Range = xlWorkSheet.Cells[Row, 1];
                Range.Font.Bold = true;
                Range.Font.Size = 12;
                xlWorkSheet.Cells[Row, 2] = "";
                //xlWorkSheet.Range[xlWorkSheet.Cells[Row, 3], xlWorkSheet.Cells[Row, 8]].Merge();
                Range = xlWorkSheet.Cells[Row, 3];
                Range.Font.Size = 12;
                // Range.Font.Color = "yellow";
                Range.Font.Size = 12;
                int RecRow = Row + 2;
                xlWorkSheet.Cells[RecRow, 1] = "TransactionType";
                xlWorkSheet.Cells[RecRow, 2] = "BeneficiaryIFSCCode";
                xlWorkSheet.Cells[RecRow, 3] = "BeneficiaryAccountNo";
                xlWorkSheet.Cells[RecRow, 4] = "BeneficiaryName";
                xlWorkSheet.Cells[RecRow, 5] = "BeneAddressLine1";
                xlWorkSheet.Cells[RecRow, 6] = "BeneAddressLine2";
                xlWorkSheet.Cells[RecRow, 7] = "BeneAddressLine3";
                xlWorkSheet.Cells[RecRow, 8] = "BeneAddressLine4";
                xlWorkSheet.Cells[RecRow, 9] = "TxnRefNo";
                xlWorkSheet.Cells[RecRow, 10] = "Amount";
                xlWorkSheet.Cells[RecRow, 11] = "SendertoReceiverInfo";
                xlWorkSheet.Cells[RecRow, 12] = "AddInfo1";
                xlWorkSheet.Cells[RecRow, 13] = "AddInfo2";
                xlWorkSheet.Cells[RecRow, 14] = "AddInfo3";
                xlWorkSheet.Cells[RecRow, 15] = "AddInfo4";
                Range = xlWorkSheet.get_Range("A" + RecRow, "T" + RecRow);
                Range.Font.Bold = true;
                //Range.Font.Color = "yellow";
                foreach (DataRow row in dtResult.Rows)
                {
                    xlWorkSheet.Cells[RecRow + 1, 1] = row["TransactionType"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 2] = row["BeneficiaryIFSCCode"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 3] = row["BeneficiaryAccountNo"];
                    xlWorkSheet.Cells[RecRow + 1, 4] = row["BeneficiaryName"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 5] = row["BeneAddressLine1"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 6] = row["BeneAddressLine2"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 7] = row["BeneAddressLine3"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 8] = row["BeneAddressLine4"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 9] = row["TxnRefNo"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 10] = row["Amount"];
                    xlWorkSheet.Cells[RecRow + 1, 11] = row["SendertoReceiverInfo"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 12] = row["AddInfo1"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 13] = row["AddInfo2"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 14] = row["AddInfo3"].ToString();
                    xlWorkSheet.Cells[RecRow + 1, 15] = row["AddInfo4"].ToString();
                    RecRow++;
                    sno++;
                    Row++;
                    PrjNo++;
                }
                LastRecNo = RecRow + 2;

                xlWorkSheet.Columns[1].AutoFit();
                xlWorkSheet.Columns[2].AutoFit();
                xlWorkSheet.Columns[3].AutoFit();
                xlWorkSheet.Columns[4].AutoFit();
                xlWorkSheet.Columns[5].AutoFit();
                xlWorkSheet.Columns[6].AutoFit();
                xlWorkSheet.Columns[7].AutoFit();
                xlWorkSheet.Columns[8].AutoFit();
                xlWorkSheet.Columns[9].AutoFit();
                xlWorkSheet.Columns[10].AutoFit();
                xlWorkSheet.Columns[11].AutoFit();
                xlWorkSheet.Columns[12].AutoFit();
                xlWorkSheet.Columns[13].AutoFit();
                xlWorkSheet.Columns[14].AutoFit();
                xlWorkSheet.Columns[15].AutoFit();
                xlWorkSheet.Columns[16].AutoFit();
                xlWorkSheet.Columns[17].AutoFit();
                Extract.DisplayAlerts = false;
                var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/CanaraBankDetails.xls");
                xlWorkBook.SaveAs(path);
                xlWorkBook.Close(true, misValue, misValue);
                Extract.Quit();
                string fileType = Common.GetMimeType(".xls");
                string filepath = "~/Content/OtherDocuments";
                string file = "CanaraBankDetails.xls";
                byte[] fileData = file.GetFileData(Server.MapPath(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=CanaraBankDetails.xls");
                return File(fileData, fileType);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public FileStreamResult GenerateAdminSalary()
        {
            try
            {
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetAdminSalary();
                return coreaccountService.toSpreadSheet(dtResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult Receipt(string Month)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        ReceiptReportModel model = new ReceiptReportModel();
                        ConsReceiptReportModel model1 = new ConsReceiptReportModel();
                        model = ReportService.GetSponserReceiptReportDetails(Month);
                        model.SchemeList = ReportService.GetSchemeReportDetails(Month);
                        model1 = ReportService.GetConsultancyReceiptReportDetails(Month);
                        DataSet dataset = new DataSet();
                        DataTable dtColumns1 = new DataTable();
                        DataTable dtColumns2 = new DataTable();
                        DataTable dtColumns3 = new DataTable();
                        DataTable dtColumns4 = new DataTable();
                        DataTable dtColumns5 = new DataTable();
                        DataTable dtColumns6 = new DataTable();
                        DataTable dtColumns7 = new DataTable();
                        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.List);
                        dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(model.MonthList);
                        dtColumns2 = JsonConvert.DeserializeObject<DataTable>(json2);
                        string json3 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.List);
                        dtColumns3 = JsonConvert.DeserializeObject<DataTable>(json3);
                        string json4 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.MonthList);
                        dtColumns4 = JsonConvert.DeserializeObject<DataTable>(json4);
                        string json5 = Newtonsoft.Json.JsonConvert.SerializeObject(model.SchemeList);
                        dtColumns5 = JsonConvert.DeserializeObject<DataTable>(json5);
                        string json6 = Newtonsoft.Json.JsonConvert.SerializeObject(model.SponScheme);
                        dtColumns6 = JsonConvert.DeserializeObject<DataTable>(json6);
                        string json7 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.ConsScheme);
                        dtColumns7 = JsonConvert.DeserializeObject<DataTable>(json7);
                        var adapter = new System.Data.SqlClient.SqlDataAdapter();
                        var ws = wb.Worksheets.Add("Sponser");
                        ws.Cell(1, 1).Value = "Month";
                        ws.Cell(1, 2).Value = Month;
                        var MonRange = ws.Range("A1:A1");
                        MonRange.Style.Font.Bold = true;
                        MonRange.Style.Font.FontSize = 12;
                        ws.Cell(4, 1).Value = "Department";
                        var DepRange = ws.Range("A4:Z4");
                        DepRange.Style.Font.Bold = true;
                        DepRange.Style.Font.FontSize = 12;
                        int montcol = 2;
                        foreach (DataRow row in dtColumns2.Rows)
                        {
                            ws.Cell(4, montcol).Value = row["Month"].ToString();
                            montcol++;
                        }
                        int firstrow = 5; int firstcol = 1;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws.Cell(firstrow, firstcol).Value = row["values"].ToString();
                            if (firstcol == model.NoOfClm)
                            {
                                firstrow++;
                                firstcol = firstcol - (model.NoOfClm - 1);
                            }
                            else
                            {
                                firstcol++;
                            }
                        }
                        firstrow = firstrow + 3;
                        ws.Cell(firstrow, 1).Value = "Summary of ICSR Receipts";
                        ws.Cell(firstrow, 2).Value = "No Of Projects for Prev Month";
                        ws.Cell(firstrow, 3).Value = "Values for Prev Month";
                        ws.Cell(firstrow, 4).Value = "No Of Projects for Curr Month";
                        ws.Cell(firstrow, 5).Value = "Values for Curr Month";
                        ws.Cell(firstrow, 6).Value = "Total Projects";
                        ws.Cell(firstrow, 7).Value = "Total Values";
                        firstrow = firstrow + 1;
                        foreach (DataRow row in dtColumns6.Rows)
                        {
                            ws.Cell(firstrow, 1).Value = row["Name"].ToString();
                            ws.Cell(firstrow, 2).Value = row["PrevNoOfProject"].ToString();
                            ws.Cell(firstrow, 3).Value = row["PrevValue"];
                            ws.Cell(firstrow, 4).Value = row["CurrNoOfProject"].ToString();
                            ws.Cell(firstrow, 5).Value = row["CurrValue"];
                            ws.Cell(firstrow, 6).Value = row["TotalNoOfProject"];
                            ws.Cell(firstrow, 7).Value = row["TotalValue"];
                            firstrow++;
                        }
                        var ws1 = wb.Worksheets.Add("Consultancy");
                        ws1.Cell(1, 1).Value = "Month";
                        ws1.Cell(1, 2).Value = Month;
                        var MonthRange = ws1.Range("A1:A1");
                        MonthRange.Style.Font.Bold = true;
                        MonthRange.Style.Font.FontSize = 12;
                        ws1.Cell(4, 1).Value = "Department";
                        var DeptRange = ws1.Range("A4:Z4");
                        DeptRange.Style.Font.Bold = true;
                        DeptRange.Style.Font.FontSize = 12;
                        int monthcol = 2;
                        foreach (DataRow row in dtColumns4.Rows)
                        {
                            ws1.Cell(4, monthcol).Value = row["Month"].ToString();
                            monthcol = monthcol + 1;
                            ws1.Cell(4, monthcol).Value = row["Month"].ToString();
                            monthcol++;
                        }

                        monthcol = 2;
                        foreach (DataRow row in dtColumns4.Rows)
                        {
                            ws1.Cell(5, monthcol).Value = "Inclusive Of Tax";
                            monthcol = monthcol + 1;
                            ws1.Cell(5, monthcol).Value = "Exclusive Of Tax";
                            monthcol++;
                        }
                        int fistrow = 6; int fisttcol = 1;
                        foreach (DataRow row in dtColumns3.Rows)
                        {
                            ws1.Cell(fistrow, fisttcol).Value = row["values"].ToString();
                            if (fisttcol == model1.NoOfClm - 1)
                            {
                                fistrow++;
                                fisttcol = fisttcol - (model1.NoOfClm - 2);
                            }
                            else
                            {
                                fisttcol++;
                            }
                        }


                        fistrow = fistrow + 3;
                        ws1.Cell(fistrow, 1).Value = "Summary of ICSR Receipts";
                        ws1.Cell(fistrow, 2).Value = "No Of Projects for Prev Month ";
                        ws1.Cell(fistrow, 3).Value = "Values for for Prev Month (Inclusive Of Tax)";
                        ws1.Cell(fistrow, 4).Value = "Values for for Prev Month (Exclusive Of Tax)";
                        ws1.Cell(fistrow, 5).Value = "No Of Projects for Curr Month ";
                        ws1.Cell(fistrow, 6).Value = "Values for Curr Month (Inclusive Of Tax)";
                        ws1.Cell(fistrow, 7).Value = "Values for Curr Month (Exclusive Of Tax)";
                        ws1.Cell(fistrow, 8).Value = "Total Projects";
                        ws1.Cell(fistrow, 9).Value = "Total Value (Inclusive Of Tax)";
                        ws1.Cell(fistrow, 10).Value = "Total Value (Exclusive Of Tax)";
                        fistrow = fistrow + 2;
                        foreach (DataRow row in dtColumns7.Rows)
                        {
                            ws1.Cell(fistrow, 1).Value = row["Name"];
                            ws1.Cell(fistrow, 2).Value = row["PrevNoOfProject"];
                            ws1.Cell(fistrow, 3).Value = row["PrevValue"];
                            ws1.Cell(fistrow, 4).Value = row["ExPrevValue"];
                            ws1.Cell(fistrow, 5).Value = row["CurrNoOfProject"];
                            ws1.Cell(fistrow, 6).Value = row["CurrValue"];
                            ws1.Cell(fistrow, 7).Value = row["ExCurrValue"];
                            ws1.Cell(fistrow, 8).Value = row["TotalNoOfProject"];
                            ws1.Cell(fistrow, 9).Value = row["TotalIncValue"];
                            ws1.Cell(fistrow, 10).Value = row["TotalExValue"];
                            fistrow++;
                        }
                        //var ws2 = wb.Worksheets.Add("Summary");
                        //ws2.Cell(3, 1).Value = "Summary of ICSR Receipts";
                        //ws2.Cell(3, 2).Value = "No Of Projects for Prev Month";
                        //ws2.Cell(3, 3).Value = "Values  for Prev Month";
                        //ws2.Cell(3, 4).Value = "No Of Projects  for Curr Month";
                        //ws2.Cell(3, 5).Value = "Values  for Curr Month";
                        //var Range = ws2.Range("A3:Z3");
                        //Range.Style.Font.Bold = true;
                        //Range.Style.Font.FontSize = 12;
                        //int sumrow = 4;
                        //foreach (DataRow row in dtColumns5.Rows)
                        //{
                        //    ws2.Cell(sumrow, 1).Value = row["Name"].ToString();
                        //    ws2.Cell(sumrow, 2).Value = row["PrevNoOfProject"];
                        //    ws2.Cell(sumrow, 3).Value = row["PrevValue"];
                        //    ws2.Cell(sumrow, 4).Value = row["CurrNoOfProject"];
                        //    ws2.Cell(sumrow, 5).Value = row["CurrValue"];
                        //    sumrow++;
                        //}

                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Receipt.xls");
                return File(workStream, fileType);
                //return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
        public FileStreamResult ReceiptOverHead(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataTable dtResult = new DataTable();
                        var ws = wb.Worksheets.Add("Consultancy ReceiptOverHeads");
                        dtResult = db.GetReceiptVoucher(FromDate, ToDate);
                        ws.Cell(1, 1).Value = "FromDate";
                        ws.Cell(1, 2).Value = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                        ws.Cell(2, 1).Value = "ToDate";
                        ws.Cell(2, 2).Value = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        var FromRange = ws.Range("A1:A1");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 12;
                        var ToRange = ws.Range("A2:A2");
                        ToRange.Style.Font.Bold = true;
                        ToRange.Style.Font.FontSize = 12;
                        ws.Cell(4, 1).Value = "ProjectNumber";
                        ws.Cell(4, 2).Value = "ProjectType";
                        ws.Cell(4, 3).Value = "DepartmentName";
                        ws.Cell(4, 4).Value = "ReceiptNumber";
                        ws.Cell(4, 5).Value = "ReceiptDate";
                        ws.Cell(4, 6).Value = "ReceiptAmount";
                        ws.Cell(4, 7).Value = "Corpus";
                        ws.Cell(4, 8).Value = "CorpusAdmin";
                        ws.Cell(4, 9).Value = "CorpusICSR";
                        ws.Cell(4, 10).Value = "Status";
                        ws.Cell(4, 11).Value = "Date";
                        var Range = ws.Range("A4:Z4");
                        Range.Style.Font.Bold = true;
                        Range.Style.Font.FontSize = 12;
                        int Firstrow = 5;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(Firstrow, 1).Value = row["ProjectNumber"].ToString();
                            ws.Cell(Firstrow, 2).Value = row["ProjectType"].ToString();
                            ws.Cell(Firstrow, 3).Value = row["DepartmentName"].ToString();
                            ws.Cell(Firstrow, 4).Value = row["ReceiptNumber"].ToString();
                            ws.Cell(Firstrow, 5).Value = row["ReceiptDate"].ToString();
                            ws.Cell(Firstrow, 6).Value = row["ReceiptAmount"].ToString();
                            ws.Cell(Firstrow, 7).Value = row["Corpus"].ToString();
                            ws.Cell(Firstrow, 8).Value = row["CorpusAdmin"].ToString();
                            ws.Cell(Firstrow, 9).Value = row["CorpusICSR"].ToString();
                            ws.Cell(Firstrow, 10).Value = row["Status"].ToString();
                            ws.Cell(Firstrow, 11).Value = row["Date"].ToString();
                            Firstrow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ReceiptOverhead.xls");
                        return File(workStream, fileType);
                        // return new FileStreamResult(workStream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ProjectTransaction(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        ProjectTransactionModel model = new ProjectTransactionModel();
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        DataTable dtColumns1 = new DataTable();
                        model = ReportService.ProjectTransaction(Id);
                        //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.Copi);
                        // dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json);
                        var ws = wb.Worksheets.Add(model.ProjectNo);
                        dtResult = db.GetProjectTransaction(Id);
                        ws.Cell(1, 1).Value = "Project Number";
                        ws.Cell(1, 2).Value = model.ProjectNo;
                        ws.Cell(2, 1).Value = "Title";
                        ws.Cell(2, 2).Value = model.Title;
                        ws.Cell(3, 1).Value = "PI";
                        ws.Cell(3, 2).Value = model.PI;
                        ws.Cell(4, 1).Value = "Total Sanction Value";
                        ws.Cell(4, 2).Value = model.SantionedValue;
                        //  ws.Cell(5, 1).Value = "Co Pi";
                        int CopiRow = 6;
                        //foreach (DataRow row in dtColumns1.Rows)
                        //{
                        //    ws.Cell(CopiRow, 1).Value = row["Copi"].ToString();
                        //    CopiRow++;
                        //}
                        var FromRange = ws.Range("A1:A1");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 12;
                        var ToRange = ws.Range("A2:A2");
                        ToRange.Style.Font.Bold = true;
                        ToRange.Style.Font.FontSize = 12;
                        var PI = ws.Range("A3:A3");
                        PI.Style.Font.Bold = true;
                        PI.Style.Font.FontSize = 12;
                        var SantionedValue = ws.Range("A4:A4");
                        SantionedValue.Style.Font.Bold = true;
                        SantionedValue.Style.Font.FontSize = 12;
                        ws.Cell(CopiRow + 1, 1).Value = "Category";
                        ws.Cell(CopiRow + 1, 2).Value = "Function Name";
                        ws.Cell(CopiRow + 1, 3).Value = "Reference No";
                        ws.Cell(CopiRow + 1, 4).Value = "Date Of Trans";
                        ws.Cell(CopiRow + 1, 5).Value = "Reference Commitment";
                        ws.Cell(CopiRow + 1, 6).Value = "Amount";
                        ws.Cell(CopiRow + 1, 7).Value = "Transation Type";
                        ws.Cell(CopiRow + 1, 8).Value = "Balance";
                        CopiRow = CopiRow + 2;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(CopiRow, 1).Value = row["Category"].ToString();
                            ws.Cell(CopiRow, 2).Value = row["FunctionName"].ToString();
                            ws.Cell(CopiRow, 3).Value = row["RefNo"].ToString();
                            ws.Cell(CopiRow, 4).Value = row["DateOfTransaction"].ToString();
                            ws.Cell(CopiRow, 5).Value = row["CommitmentNumber"].ToString();
                            ws.Cell(CopiRow, 6).Value = row["Amount"].ToString();
                            ws.Cell(CopiRow, 7).Value = row["TransType"].ToString();
                            ws.Cell(CopiRow, 8).Value = row["RunningAmtTotal"].ToString();
                            CopiRow++;
                        }
                        ws.Cell(CopiRow + 3, 7).Value = "Total Balance";
                        ws.Cell(CopiRow + 3, 8).Value = model.TotalAmt;
                        ws.Cell(CopiRow + 4, 7).Value = "Available N.B";
                        ws.Cell(CopiRow + 4, 8).Value = model.NegBal;
                        ws.Cell(CopiRow + 5, 7).Value = "Net Balance";
                        ws.Cell(CopiRow + 5, 8).Value = model.NetBalance;
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ProjectTransaction.xls");
                        return File(workStream, fileType);
                        //   return new FileStreamResult(workStream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult CashBookOld(string fdate, string tdate, int BankId)
        {
            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);

            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    ReportService reportservice = new ReportService();
                    CashBookModel model = new CashBookModel();
                    var fromdate = Convert.ToDateTime(fdate);
                    var todate = Convert.ToDateTime(tdate);
                    todate = todate.AddDays(1).AddTicks(-1001);
                    var PayAmt = (from c in context.vw_CashBookPayment
                                  where c.ReferenceDate < fromdate && c.BankHeadID == BankId
                                  select c.Amount).Sum() ?? 0;
                    //decimal PayAmt = Pay.Select(m => m.Amount).Sum() ?? 0;
                    Math.Round(PayAmt, 2);
                    var RecAmt = (from c in context.vw_CashBookReceipt
                                  where c.ReferenceDate < fromdate && c.BankHeadID == BankId
                                  select c.Amount).Sum() ?? 0;
                    //decimal RecAmt = Rec.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmt, 2);
                    var OB = (from c in context.tblHeadOpeningBalance
                              where c.AccountHeadId == BankId
                              select c).FirstOrDefault();
                    string FinalOB = "";
                    decimal COB = 0;
                    string TranType = ""; decimal? migOB = 0;
                    if (OB != null)
                        TranType = OB.TransactionType;
                    if (OB != null)
                        migOB = OB.OpeningBalance ?? 0;
                    if (TranType == "Credit")
                    {
                        COB = -migOB + (RecAmt - PayAmt) ?? 0;
                        Math.Round(COB, 2);
                    }
                    else if (TranType == "Debit") { COB = migOB + (RecAmt - PayAmt) ?? 0; Math.Round(COB, 2); }
                    else
                    {
                        COB = migOB + (RecAmt - PayAmt) ?? 0; Math.Round(COB, 2);
                    }
                    var PayCB = (from c in context.vw_CashBookPayment
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal PayAmtCB = PayCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(PayAmtCB, 2);
                    var RecCB = (from c in context.vw_CashBookReceipt
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal RecAmtCB = RecCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmtCB, 2);
                    decimal CB = 0;
                    if (COB < 0)
                    {
                        CB = COB + (RecAmtCB - PayAmtCB);
                        Math.Round(CB, 2);
                        FinalOB = -COB + " Cr";
                    }
                    else if (COB >= 0) { CB = COB + (RecAmtCB - PayAmtCB); Math.Round(CB, 2); FinalOB = COB + " Dr"; }
                    string FinalCB = "";
                    if (CB < 0) { FinalCB = -CB + " Cr"; } else { FinalCB = CB + " Dr"; }
                    model.REC = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
                    model.PAY = ReportService.CashBookPaymentRep(fromdate, todate, BankId);
                    DataSet dataset = new DataSet();
                    DataTable dtColumns1 = new DataTable();
                    DataTable dtColumns2 = new DataTable();
                    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.REC);
                    dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                    string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(model.PAY);
                    dtColumns2 = JsonConvert.DeserializeObject<DataTable>(json2);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("Cash Book");
                    ws.Cell(1, 4).Value = "Cash Book";
                    ws.Cell(3, 1).Value = "From Date :";
                    ws.Cell(3, 2).Value = String.Format("{0:dd-MMMM-yyyy}", fromdate);
                    ws.Cell(4, 1).Value = "To Date :";
                    ws.Cell(4, 2).Value = String.Format("{0:dd-MMMM-yyyy}", todate);
                    ws.Cell(5, 1).Value = "Bank Name :";
                    ws.Cell(5, 2).Value = Common.GetBankName(BankId);
                    ws.Cell(6, 1).Value = "Opening Bal :";
                    ws.Cell(6, 2).Value = FinalOB;
                    ws.Cell(7, 1).Value = "Closing Bal :";
                    ws.Cell(7, 2).Value = FinalCB;
                    ws.Cell(9, 2).Value = "Receipt";
                    ws.Cell(9, 8).Value = "Payment";
                    //ws.Cell(10, 1).Value = "Date";
                    //ws.Cell(10, 2).Value = "VoucherNumber";
                    //ws.Cell(10, 3).Value = "Batch Number";
                    //ws.Cell(10, 4).Value = "Cheque Number";
                    //ws.Cell(10, 5).Value = "Payee";
                    //ws.Cell(10, 6).Value = "Amount";
                    //ws.Cell(10, 8).Value = "Date";
                    //ws.Cell(10, 9).Value = "VoucherNumber";
                    //ws.Cell(10, 10).Value = "Batch Number";
                    //ws.Cell(10, 11).Value = "Payee";
                    //ws.Cell(10, 12).Value = "Amount";
                    int firstrow = 11; int secondrow = 11;
                    if (model.REC.Count() > 0)
                    {
                        dtColumns1.Columns.Remove("AccountHeadId");
                        dtColumns1.Columns.Remove("FromDate");
                        dtColumns1.Columns.Remove("ToDate");
                        dtColumns1.Columns.Remove("BOAPaymentDetailId");
                        dtColumns1.Columns.Remove("PayeeBank");
                        dtColumns1.Columns.Remove("BOAId");
                        dtColumns1.Columns.Remove("TransactionType");
                        dtColumns1.Columns.Remove("BankHeadID");
                        dtColumns1.Columns.Remove("PayeeName");
                        dtColumns1.Columns.Remove("TransactionTypeCode");
                        dtColumns1.Columns.Remove("BankId");

                        dtColumns1.Columns["ReferenceDate"].SetOrdinal(0);
                        dtColumns1.Columns["VoucherNumber"].SetOrdinal(1);
                        dtColumns1.Columns["TempVoucherNo"].SetOrdinal(2);
                        dtColumns1.Columns["ChequeNo"].SetOrdinal(3);
                        dtColumns1.Columns["VoucherPayee"].SetOrdinal(4);
                        dtColumns1.Columns["Amount"].SetOrdinal(5);

                        //  ws.Cell(firstrow, 1).InsertTable(dtColumns1).ShowHeaderRow = false;
                        ws.Cell(firstrow, 1).InsertTable(dtColumns1);
                    }

                    if (model.PAY.Count() > 0)
                    {
                        dtColumns2.Columns.Remove("AccountHeadId");
                        dtColumns2.Columns.Remove("FromDate");
                        dtColumns2.Columns.Remove("ToDate");
                        dtColumns2.Columns.Remove("BOAPaymentDetailId");
                        dtColumns2.Columns.Remove("PayeeBank");
                        dtColumns2.Columns.Remove("BOAId");
                        dtColumns2.Columns.Remove("TransactionType");
                        dtColumns2.Columns.Remove("BankHeadID");
                        dtColumns2.Columns.Remove("PayeeName");
                        dtColumns2.Columns.Remove("TransactionTypeCode");
                        dtColumns2.Columns.Remove("BankId");

                        dtColumns2.Columns["ReferenceDate"].SetOrdinal(0);
                        dtColumns2.Columns["VoucherNumber"].SetOrdinal(1);
                        dtColumns2.Columns["TempVoucherNo"].SetOrdinal(2);
                        dtColumns2.Columns["VoucherPayee"].SetOrdinal(3);
                        dtColumns2.Columns["Amount"].SetOrdinal(4);

                        // ws.Cell(secondrow, 8).InsertTable(dtColumns2).ShowHeaderRow = false;
                        ws.Cell(secondrow, 8).InsertTable(dtColumns2);
                    }

                    //foreach (DataRow row in dtColumns1.Rows)
                    //{
                    //    ws.Cell(firstrow, 1).Value = row["ReferenceDate"].ToString();
                    //    ws.Cell(firstrow, 2).Value = row["VoucherNumber"].ToString();
                    //    ws.Cell(firstrow, 3).Value = row["TempVoucherNo"].ToString();
                    //    ws.Cell(firstrow, 4).Value = row["ChequeNo"].ToString();
                    //    ws.Cell(firstrow, 5).Value = row["VoucherPayee"].ToString();
                    //    ws.Cell(firstrow, 6).Value = row["Amount"];

                    //    firstrow++;
                    //}
                    //foreach (DataRow row in dtColumns2.Rows)
                    //{
                    //    ws.Cell(secondrow, 8).Value = row["ReferenceDate"].ToString();
                    //    ws.Cell(secondrow, 9).Value = row["VoucherNumber"].ToString();
                    //    ws.Cell(secondrow, 10).Value = row["TempVoucherNo"].ToString();
                    //    ws.Cell(secondrow, 11).Value = row["VoucherPayee"].ToString();
                    //    ws.Cell(secondrow, 12).Value = row["Amount"];
                    //    secondrow++;
                    //}
                    var rngTitle = ws.Range("D1:D1");
                    rngTitle.Style.Fill.BackgroundColor = XLColor.Rose;
                    rngTitle.Style.Font.Bold = true;
                    rngTitle.Style.Font.FontSize = 15;
                    var FromRange = ws.Range("A3:A3");
                    FromRange.Style.Font.Bold = true;
                    FromRange.Style.Font.FontSize = 12;
                    var ToRange = ws.Range("A4:A4");
                    ToRange.Style.Font.Bold = true;
                    ToRange.Style.Font.FontSize = 12;
                    var BankRange = ws.Range("A5:A5");
                    BankRange.Style.Font.Bold = true;
                    BankRange.Style.Font.FontSize = 12;
                    var OBRange = ws.Range("A6:A6");
                    OBRange.Style.Font.Bold = true;
                    OBRange.Style.Font.FontSize = 12;
                    var CBRange = ws.Range("A7:A7");
                    CBRange.Style.Font.Bold = true;
                    CBRange.Style.Font.FontSize = 12;
                    var RECRange = ws.Range("B9:B9");
                    RECRange.Style.Font.Bold = true;
                    RECRange.Style.Font.FontSize = 15;
                    var PAYRange = ws.Range("E9:E9");
                    PAYRange.Style.Font.Bold = true;
                    PAYRange.Style.Font.FontSize = 15;
                    var Detrange = ws.Range("A10:F10");
                    Detrange.Style.Font.Bold = true;
                    Detrange.Style.Font.FontSize = 10;
                    wb.SaveAs(workStream);
                    workStream.Position = 0;

                }

            }
            string fileType = Common.GetMimeType("xls");
            Response.AddHeader("Content-Disposition", "filename=cashbook.xls");
            return File(workStream, fileType);
            //  put this application / vnd.openxmlformats - officedocument.spreadsheetml.sheet instead of application / vnd.ms - excel
            // return new FileStreamResult(workStream, "application/vnd.ms-excel");
            //return new FileStreamResult(workStream, "application/vnd.openxmlformats - officedocument.spreadsheetml.sheet");
        }
        public FileStreamResult CashBook(string fdate, string tdate, int BankId)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    ListDatabaseObjects db = new ListDatabaseObjects();
                    DataTable dtResult0 = new DataTable();
                    DataTable dtResult1 = new DataTable();
                    DataTable dtResult2 = new DataTable();
                    DateTime fromdate = Convert.ToDateTime(fdate);
                    DateTime todate = Convert.ToDateTime(tdate);
                    todate = todate.AddDays(1).AddTicks(-2001);
                    var Frm = fromdate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = todate.ToString("yyyy-MM-dd HH:mm");
                    DataSet dsReport = db.getCashBook(Frm, Todate, BankId);
                    dtResult0 = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : new DataTable();
                    dtResult1 = dsReport.Tables.Count > 0 ? dsReport.Tables[1] : new DataTable();
                    dtResult2 = dsReport.Tables.Count > 0 ? dsReport.Tables[2] : new DataTable();

                    //  dtResult1.DefaultView.Sort = "ReferenceDate ASC";
                    //  dtResult2.DefaultView.Sort = "ReferenceDate ASC";
                    var ws = wb.Worksheets.Add("Cash Book");

                    DataView dataview = dtResult1.DefaultView;
                    dataview.Sort = "ReferenceDate  ASC";
                    DataTable dt = dataview.ToTable();

                    DataView dataview2 = dtResult2.DefaultView;
                    dataview2.Sort = "ReferenceDate  ASC";
                    DataTable dt2 = dataview2.ToTable();


                    foreach (DataRow row in dtResult0.Rows)
                    {
                        ws.Cell(1, 4).Value = "Cash Book";
                        ws.Cell(3, 1).Value = "From Date :";
                        ws.Cell(3, 2).Value = String.Format("{0:dd-MMMM-yyyy}", fromdate);
                        ws.Cell(4, 1).Value = "To Date :";
                        ws.Cell(4, 2).Value = String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws.Cell(5, 1).Value = "Bank Name :";
                        ws.Cell(5, 2).Value = Common.GetBankName(BankId);
                        ws.Cell(6, 1).Value = "Opening Bal :";
                        ws.Cell(6, 2).Value = row["OpeningBal"].ToString();
                        ws.Cell(7, 1).Value = "Closing Bal :";
                        ws.Cell(7, 2).Value = row["ClosingBal"].ToString();
                    }
                    ws.Cell(9, 2).Value = "Receipt";
                    ws.Cell(9, 8).Value = "Payment";

                    int firstrow = 11; int secondrow = 11;


                    dt.Columns["ReferenceDate"].SetOrdinal(0);
                    dt.Columns["VoucherNumber"].SetOrdinal(1);
                    dt.Columns["TempVoucherNumber"].SetOrdinal(2);
                    dt.Columns["ChequeNumber"].SetOrdinal(3);
                    dt.Columns["VoucherPayee"].SetOrdinal(4);
                    dt.Columns["Amount"].SetOrdinal(5);


                    ws.Cell(firstrow, 1).InsertTable(dt);


                    dt2.Columns["ReferenceDate"].SetOrdinal(0);
                    dt2.Columns["VoucherNumber"].SetOrdinal(1);
                    dt2.Columns["TempVoucherNumber"].SetOrdinal(2);
                    dt2.Columns["VoucherPayee"].SetOrdinal(3);
                    dt2.Columns["Amount"].SetOrdinal(4);


                    ws.Cell(secondrow, 8).InsertTable(dt2);


                    var rngTitle = ws.Range("D1:D1");
                    rngTitle.Style.Fill.BackgroundColor = XLColor.Rose;
                    rngTitle.Style.Font.Bold = true;
                    rngTitle.Style.Font.FontSize = 15;
                    var FromRange = ws.Range("A3:A3");
                    FromRange.Style.Font.Bold = true;
                    FromRange.Style.Font.FontSize = 12;
                    var ToRange = ws.Range("A4:A4");
                    ToRange.Style.Font.Bold = true;
                    ToRange.Style.Font.FontSize = 12;
                    var BankRange = ws.Range("A5:A5");
                    BankRange.Style.Font.Bold = true;
                    BankRange.Style.Font.FontSize = 12;
                    var OBRange = ws.Range("A6:A6");
                    OBRange.Style.Font.Bold = true;
                    OBRange.Style.Font.FontSize = 12;
                    var CBRange = ws.Range("A7:A7");
                    CBRange.Style.Font.Bold = true;
                    CBRange.Style.Font.FontSize = 12;
                    var RECRange = ws.Range("B9:B9");
                    RECRange.Style.Font.Bold = true;
                    RECRange.Style.Font.FontSize = 15;
                    var PAYRange = ws.Range("E9:E9");
                    PAYRange.Style.Font.Bold = true;
                    PAYRange.Style.Font.FontSize = 15;
                    var Detrange = ws.Range("A10:F10");
                    Detrange.Style.Font.Bold = true;
                    Detrange.Style.Font.FontSize = 10;
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=cashbook.xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public FileStreamResult OverHeadPosting(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetOverHeadPosting(Id);
                        var ws = wb.Worksheets.Add("OverHeadPosting");
                        var FromRange = ws.Range("A1:Z1");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 12;
                        ws.Cell(1, 1).Value = "TransactionType";
                        ws.Cell(1, 2).Value = "BeneficiaryIFSCCode";
                        ws.Cell(1, 3).Value = "BeneficiaryAccountNo";
                        ws.Cell(1, 4).Value = "BeneficiaryName";
                        ws.Cell(1, 5).Value = "BeneAddressLine1";
                        ws.Cell(1, 6).Value = "BeneAddressLine2";
                        ws.Cell(1, 7).Value = "BeneAddressLine3";
                        ws.Cell(1, 8).Value = "BeneAddressLine4";
                        ws.Cell(1, 9).Value = "TxnRefNo";
                        ws.Cell(1, 10).Value = "Amount";
                        ws.Cell(1, 11).Value = "SendertoReceiverInfo";
                        ws.Cell(1, 12).Value = "AddInfo1";
                        ws.Cell(1, 13).Value = "AddInfo2";
                        ws.Cell(1, 14).Value = "AddInfo3";
                        ws.Cell(1, 15).Value = "AddInfo4";
                        int Firstrow = 2;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(Firstrow, 1).Value = row["TransactionType"].ToString();
                            ws.Cell(Firstrow, 2).Value = row["BeneficiaryIFSCCode"].ToString();
                            ws.Cell(Firstrow, 3).Value = row["BeneficiaryAccountNo"].ToString();
                            ws.Cell(Firstrow, 4).Value = row["BeneficiaryName"].ToString();
                            ws.Cell(Firstrow, 5).Value = "";
                            ws.Cell(Firstrow, 6).Value = "";
                            ws.Cell(Firstrow, 7).Value = "";
                            ws.Cell(Firstrow, 8).Value = "";
                            ws.Cell(Firstrow, 9).Value = row["TxnRefNo"].ToString();
                            ws.Cell(Firstrow, 10).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 11).Value = row["SendertoReceiverInfo"].ToString();
                            ws.Cell(Firstrow, 12).Value = "";
                            ws.Cell(Firstrow, 13).Value = "";
                            ws.Cell(Firstrow, 14).Value = "";
                            ws.Cell(Firstrow, 15).Value = "";
                            Firstrow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=OverheadPosting.xls");
                return File(workStream, fileType);
                // return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public FileStreamResult CanaraBankForSalary(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetCanaraBankForSalary(Id);
                        var ws = wb.Worksheets.Add("Canara Bank");
                        ws.Cell(1, 1).Value = "Txn Type";
                        ws.Cell(1, 2).Value = "Account Number";
                        ws.Cell(1, 3).Value = "Branch Code";
                        ws.Cell(1, 4).Value = "Txn Code";
                        ws.Cell(1, 5).Value = "Txn Date";
                        ws.Cell(1, 6).Value = "Dr / Cr";
                        ws.Cell(1, 7).Value = "Value Date";
                        ws.Cell(1, 8).Value = "Txn CCY";
                        ws.Cell(1, 8).Value = "Amt LCY";
                        ws.Cell(1, 9).Value = "Amt TCY";
                        ws.Cell(1, 10).Value = "Rate Con";
                        ws.Cell(1, 11).Value = "Ref No";
                        ws.Cell(1, 12).Value = "Ref Doc No";
                        ws.Cell(1, 13).Value = "Transaction Description";
                        ws.Cell(1, 14).Value = "Benef IC";
                        ws.Cell(1, 15).Value = "Benef Name";
                        ws.Cell(1, 16).Value = "Benef Add 1";
                        ws.Cell(1, 17).Value = "Benef Add 2";
                        ws.Cell(1, 18).Value = "Benef Add 3";
                        ws.Cell(1, 19).Value = "Benef City";
                        ws.Cell(1, 20).Value = "Benef State";
                        ws.Cell(1, 21).Value = "Benef Cntry";
                        ws.Cell(1, 22).Value = "Benef Zip";
                        ws.Cell(1, 23).Value = "Option";
                        ws.Cell(1, 24).Value = "Issuer Code";
                        ws.Cell(1, 25).Value = "Payable At";
                        ws.Cell(1, 26).Value = "Flg FDT";
                        ws.Cell(1, 27).Value = "MIS Account Number";
                        ws.Cell(1, 28).Value = "~~END~~";
                        ws.Cell(1, 29).Value = "~~END~~";
                        int Firstrow = 2;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(Firstrow, 1).Value = "1";
                            ws.Cell(Firstrow, 2).Value = row["AccountNumber"].ToString();
                            ws.Cell(Firstrow, 3).Value = "2722";
                            ws.Cell(Firstrow, 4).Value = "1408";
                            ws.Cell(Firstrow, 5).Value = row["ReferenceDate"].ToString();
                            ws.Cell(Firstrow, 6).Value = "C";
                            ws.Cell(Firstrow, 7).Value = row["ReferenceDate"].ToString();
                            ws.Cell(Firstrow, 8).Value = "104";
                            ws.Cell(Firstrow, 9).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 10).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 11).Value = "1.00";
                            ws.Cell(Firstrow, 12).Value = "0";
                            ws.Cell(Firstrow, 13).Value = "0";
                            ws.Cell(Firstrow, 14).Value = "Salary Credit";
                            ws.Cell(Firstrow, 15).Value = "";
                            ws.Cell(Firstrow, 16).Value = "";
                            ws.Cell(Firstrow, 16).Value = "";
                            ws.Cell(Firstrow, 17).Value = "";
                            ws.Cell(Firstrow, 18).Value = "";
                            ws.Cell(Firstrow, 19).Value = "";
                            ws.Cell(Firstrow, 20).Value = "";
                            ws.Cell(Firstrow, 21).Value = "";
                            ws.Cell(Firstrow, 22).Value = "";
                            ws.Cell(Firstrow, 23).Value = "30";
                            ws.Cell(Firstrow, 24).Value = "0";
                            ws.Cell(Firstrow, 25).Value = "0";
                            ws.Cell(Firstrow, 26).Value = "N";
                            ws.Cell(Firstrow, 27).Value = "";
                            ws.Cell(Firstrow, 28).Value = "~~END~~";
                            ws.Cell(Firstrow, 29).Value = "~~END~~";
                            Firstrow++;
                        }
                        ws.Cell(Firstrow + 1, 8).Value = "MIS Account Number";
                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=CanaraBankForSalary.xls");
                return File(workStream, fileType);
                // return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public FileStreamResult NonCanaraBankForSalary(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetNonCanaraBankForSalary(Id);
                        var ws = wb.Worksheets.Add("Sheet1");
                        ws.Cell(1, 1).Value = "Txn Type";
                        ws.Cell(1, 2).Value = "Account Number";
                        ws.Cell(1, 3).Value = "Bank Name";
                        ws.Cell(1, 4).Value = "Bank IFSC Code";
                        ws.Cell(1, 5).Value = "Name";
                        ws.Cell(1, 6).Value = "Branch Code";
                        ws.Cell(1, 7).Value = "Txn Code";
                        ws.Cell(1, 8).Value = "Txn Date";
                        ws.Cell(1, 9).Value = "Dr / Cr";
                        ws.Cell(1, 10).Value = "Value Date";
                        ws.Cell(1, 11).Value = "Txn CCY";
                        ws.Cell(1, 12).Value = "Amt LCY";
                        ws.Cell(1, 13).Value = "Amt TCY";
                        ws.Cell(1, 14).Value = "Rate Con";
                        ws.Cell(1, 15).Value = "Ref No";
                        ws.Cell(1, 16).Value = "Ref Doc No";
                        ws.Cell(1, 17).Value = "Transaction Description";
                        ws.Cell(1, 18).Value = "Benef IC";
                        ws.Cell(1, 19).Value = "Benef Name";
                        ws.Cell(1, 20).Value = "Benef Add 1";
                        ws.Cell(1, 21).Value = "Benef Add 2";
                        ws.Cell(1, 22).Value = "Benef Add 3";
                        ws.Cell(1, 23).Value = "Benef City";
                        ws.Cell(1, 24).Value = "Benef State";
                        ws.Cell(1, 25).Value = "Benef Cntry";
                        ws.Cell(1, 26).Value = "Benef Zip";
                        ws.Cell(1, 27).Value = "Option";
                        ws.Cell(1, 28).Value = "Issuer Code";
                        ws.Cell(1, 29).Value = "Payable At";
                        ws.Cell(1, 30).Value = "Flg FDT";
                        ws.Cell(1, 31).Value = "MIS Account Number";
                        ws.Cell(1, 32).Value = "~~END~~";
                        ws.Cell(1, 33).Value = "~~END~~";
                        int Firstrow = 2;
                        decimal? TotalAmt = 0;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(Firstrow, 1).Value = "1";
                            ws.Cell(Firstrow, 2).Value = row["AccountNumber"].ToString();
                            ws.Cell(Firstrow, 3).Value = row["BankName"].ToString();
                            ws.Cell(Firstrow, 4).Value = row["IFSCCode"].ToString();
                            ws.Cell(Firstrow, 5).Value = row["FirstName"].ToString();
                            ws.Cell(Firstrow, 6).Value = "2722";
                            ws.Cell(Firstrow, 7).Value = "1408";
                            ws.Cell(Firstrow, 8).Value = row["ReferenceDate"].ToString();
                            ws.Cell(Firstrow, 9).Value = "C";
                            ws.Cell(Firstrow, 10).Value = row["ReferenceDate"].ToString();
                            ws.Cell(Firstrow, 11).Value = "104";
                            ws.Cell(Firstrow, 12).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 13).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 14).Value = "1.00";
                            ws.Cell(Firstrow, 15).Value = "0";
                            ws.Cell(Firstrow, 16).Value = "0";
                            ws.Cell(Firstrow, 17).Value = "Salary Credit";
                            ws.Cell(Firstrow, 18).Value = "";
                            ws.Cell(Firstrow, 19).Value = "";
                            ws.Cell(Firstrow, 20).Value = "";
                            ws.Cell(Firstrow, 21).Value = "";
                            ws.Cell(Firstrow, 22).Value = "";
                            ws.Cell(Firstrow, 23).Value = "";
                            ws.Cell(Firstrow, 24).Value = "";
                            ws.Cell(Firstrow, 25).Value = "";
                            ws.Cell(Firstrow, 26).Value = "";
                            ws.Cell(Firstrow, 27).Value = "30";
                            ws.Cell(Firstrow, 28).Value = "0";
                            ws.Cell(Firstrow, 29).Value = "0";
                            ws.Cell(Firstrow, 30).Value = "N";
                            ws.Cell(Firstrow, 31).Value = "";
                            ws.Cell(Firstrow, 32).Value = "~~END~~";
                            ws.Cell(Firstrow, 33).Value = "~~END~~";

                            Firstrow++;
                        }
                        ws.Cell(Firstrow + 1, 12).Value = "Total";
                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=NonCanaraBankForSalary.xls");
                return File(workStream, fileType);
                //return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public FileStreamResult AnnualAccountReport(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                ToDate = ToDate.AddDays(1).AddSeconds(-2);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var context = new IOASDBEntities())
                {
                    SqlParameter[] ReportParam = new SqlParameter[2];
                    ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
                    ReportParam[0].Value = Frm;
                    ReportParam[1] = new SqlParameter("@Date2", SqlDbType.DateTime);
                    ReportParam[1].Value = Todate;
                    context.Database.CommandTimeout = 1800;
                    context.Database.ExecuteSqlCommand("exec AnnualAccounts @Date, @Date2", ReportParam);
                    using (var connection = Common.getConnection())
                    {
                        connection.Open();
                        var command = new System.Data.SqlClient.SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1800;
                        command.CommandText = "select * from tblOverallAnnualAccounts";
                        var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                        var dataset = new DataSet();
                        adapter.Fill(dataset);
                        DataTable dtColumns = new DataTable();
                        dtColumns = dataset.Tables[0];
                        return coreaccountService.toSpreadSheet(dtColumns);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ExpenditureReportwithBankAmount(DateTime FromDate, DateTime ToDate)
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsTrasaction = db.GetExpenditureReportwithBankAmount(FromDate, ToDate);
                return toSpreadSheets(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult PCFOverHeadPosting(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetPCFOverHeadPosting(Id);
                        var ws = wb.Worksheets.Add("OverHeadPosting");
                        var FromRange = ws.Range("A1:Z1");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 12;
                        ws.Cell(1, 1).Value = "TransactionType";
                        ws.Cell(1, 2).Value = "BeneficiaryIFSCCode";
                        ws.Cell(1, 3).Value = "BeneficiaryAccountNo";
                        ws.Cell(1, 4).Value = "BeneficiaryName";
                        ws.Cell(1, 5).Value = "BeneAddressLine1";
                        ws.Cell(1, 6).Value = "BeneAddressLine2";
                        ws.Cell(1, 7).Value = "BeneAddressLine3";
                        ws.Cell(1, 8).Value = "BeneAddressLine4";
                        ws.Cell(1, 9).Value = "TxnRefNo";
                        ws.Cell(1, 10).Value = "Amount";
                        ws.Cell(1, 11).Value = "SendertoReceiverInfo";
                        ws.Cell(1, 12).Value = "AddInfo1";
                        ws.Cell(1, 13).Value = "AddInfo2";
                        ws.Cell(1, 14).Value = "AddInfo3";
                        ws.Cell(1, 15).Value = "AddInfo4";
                        int Firstrow = 2;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(Firstrow, 1).Value = row["TransactionType"].ToString();
                            ws.Cell(Firstrow, 2).Value = row["BeneficiaryIFSCCode"].ToString();
                            ws.Cell(Firstrow, 3).Value = row["BeneficiaryAccountNo"].ToString();
                            ws.Cell(Firstrow, 4).Value = row["BeneficiaryName"].ToString();
                            ws.Cell(Firstrow, 5).Value = "";
                            ws.Cell(Firstrow, 6).Value = "";
                            ws.Cell(Firstrow, 7).Value = "";
                            ws.Cell(Firstrow, 8).Value = "";
                            ws.Cell(Firstrow, 9).Value = row["TxnRefNo"].ToString();
                            ws.Cell(Firstrow, 10).Value = row["Amount"].ToString();
                            ws.Cell(Firstrow, 11).Value = row["SendertoReceiverInfo"].ToString();
                            ws.Cell(Firstrow, 12).Value = "";
                            ws.Cell(Firstrow, 13).Value = "";
                            ws.Cell(Firstrow, 14).Value = "";
                            ws.Cell(Firstrow, 15).Value = "";
                            Firstrow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=PCFOverHeadPosting.xls");
                return File(workStream, fileType);
                // return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public FileStreamResult ImprestRecoupment(string Id)
        {
            try
            {
                Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                // dtResult.Columns.Add("Debting Account No");
                //dtResult.Columns.Add("12345678");
                //dtResult.Columns.Add("Ordering Customer Name");
                //dtResult.Columns.Add("ICSR");
                //dtResult.Columns.Add("Address");
                //dtResult.Columns.Add("IITM");

                dtResult = db.GetImprestRecoupment(Id);
                return coreaccountService.toSpreadSheet(dtResult);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Contra Bank Advice
        public FileStreamResult ContraBankAdvice(string Id)
        {
            try
            {
                Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetContraBankAdvice(Id);
                return coreaccountService.toSpreadSheet(dtResult);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Direct Fund Transfer Bank Advice
        public FileStreamResult DirectFundTransferBankAdvice(string Id)
        {
            try
            {
                Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtResult = new DataTable();
                dtResult = db.GetDirectFundTransferBankAdvice(Id);
                return coreaccountService.toSpreadSheet(dtResult);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        public FileStreamResult InputInGSToffset(DateTime FromDate, DateTime ToDate, int GSToffsetid = 0)
        {
            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    GSTOffsetModel model = new GSTOffsetModel();
                    CoreAccountsService db = new CoreAccountsService();
                    DataTable dtResult = new DataTable();
                    model.GSTOffsetInput = CoreAccountsService.GetGSTInputList(FromDate, ToDate, GSToffsetid);

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.GSTOffsetInput);
                    dtResult = JsonConvert.DeserializeObject<DataTable>(json);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("GSTOffsetInput");
                    ws.Cell(1, 4).Value = "GSToffsetInput";
                    ws.Cell(3, 1).Value = "From Date :";
                    ws.Cell(3, 2).Value = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                    ws.Cell(4, 1).Value = "To Date :";
                    ws.Cell(4, 2).Value = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                    ws.Cell(7, 1).Value = "VendorName";
                    ws.Cell(7, 2).Value = "InvoiceNumber";
                    ws.Cell(7, 3).Value = "GSTIN";
                    ws.Cell(7, 4).Value = "Amount";
                    ws.Cell(7, 5).Value = "Interstate";
                    int Firstrow = 8;
                    foreach (DataRow row in dtResult.Rows)
                    {
                        ws.Cell(Firstrow, 1).Value = row["InputHead"].ToString();
                        ws.Cell(Firstrow, 2).Value = row["InputNumber"].ToString();
                        ws.Cell(Firstrow, 3).Value = row["InputTransaction"].ToString();
                        ws.Cell(Firstrow, 4).Value = row["InputAmount"].ToString();
                        ws.Cell(Firstrow, 5).Value = row["ProjNo"].ToString();
                        Firstrow++;
                    }
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
            }
            string fileType = Common.GetMimeType("xls");
            Response.AddHeader("Content-Disposition", "filename=InputinGSToffset.xls");
            return File(workStream, fileType);
            // return new FileStreamResult(workStream, "application/vnd.ms-excel");
        }
        public FileStreamResult OutputInGSToffset(DateTime FromDate, DateTime ToDate, int GSToffsetid = 0)
        {
            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    GSTOffsetModel model = new GSTOffsetModel();
                    CoreAccountsService db = new CoreAccountsService();
                    DataTable dtResult = new DataTable();
                    model.GSTOffsetOutput = CoreAccountsService.GetGSTOutputList(FromDate, ToDate, GSToffsetid);

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.GSTOffsetOutput);
                    dtResult = JsonConvert.DeserializeObject<DataTable>(json);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("GSTOffsetInput");
                    ws.Cell(1, 4).Value = "GSToffsetInput";
                    ws.Cell(3, 1).Value = "From Date :";
                    ws.Cell(3, 2).Value = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                    ws.Cell(4, 1).Value = "To Date :";
                    ws.Cell(4, 2).Value = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                    ws.Cell(7, 1).Value = "Transaction";
                    ws.Cell(7, 2).Value = "ProjectNo";
                    ws.Cell(7, 3).Value = "Ref No";
                    ws.Cell(7, 4).Value = "Head Name";
                    ws.Cell(7, 5).Value = "Credit";
                    ws.Cell(7, 6).Value = "Debit";
                    int Firstrow = 8;
                    foreach (DataRow row in dtResult.Rows)
                    {
                        ws.Cell(Firstrow, 1).Value = row["OutputTransaction"].ToString();
                        ws.Cell(Firstrow, 2).Value = row["ProjNo"].ToString();
                        ws.Cell(Firstrow, 3).Value = row["OutputNumber"].ToString();
                        ws.Cell(Firstrow, 4).Value = row["OutputHead"].ToString();
                        ws.Cell(Firstrow, 5).Value = row["OutputCredit"].ToString();
                        ws.Cell(Firstrow, 6).Value = row["OutputDebit"].ToString();
                        Firstrow++;
                    }
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
            }
            string fileType = Common.GetMimeType("xls");
            Response.AddHeader("Content-Disposition", "filename=OutputInGSToffset.xls");
            return File(workStream, fileType);
        }
        public FileStreamResult TDSReceivableInGSToffset(DateTime FromDate, DateTime ToDate, int GSToffsetid = 0)
        {
            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    GSTOffsetModel model = new GSTOffsetModel();
                    CoreAccountsService db = new CoreAccountsService();
                    DataTable dtResult = new DataTable();
                    model.GSTOffsetTDS = CoreAccountsService.GetTDSList(FromDate, ToDate, GSToffsetid);
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.GSTOffsetTDS);
                    dtResult = JsonConvert.DeserializeObject<DataTable>(json);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("TDSReceivable");
                    ws.Cell(1, 4).Value = "TDSReceivable";
                    ws.Cell(3, 1).Value = "From Date :";
                    ws.Cell(3, 2).Value = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                    ws.Cell(4, 1).Value = "To Date :";
                    ws.Cell(4, 2).Value = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                    ws.Cell(7, 1).Value = "Transaction";
                    ws.Cell(7, 2).Value = "ProjectNo";
                    ws.Cell(7, 3).Value = "Ref No";
                    ws.Cell(7, 4).Value = "Head	";
                    ws.Cell(7, 5).Value = "Debit	";
                    ws.Cell(7, 6).Value = "Credit";


                    int Firstrow = 8;
                    foreach (DataRow row in dtResult.Rows)
                    {
                        ws.Cell(Firstrow, 1).Value = row["TDSTransaction"].ToString();
                        ws.Cell(Firstrow, 2).Value = row["ProjNo"].ToString();
                        ws.Cell(Firstrow, 3).Value = row["TDSNumber"].ToString();
                        ws.Cell(Firstrow, 4).Value = row["TDSHead"].ToString();
                        ws.Cell(Firstrow, 5).Value = row["TDSDebit"].ToString();
                        ws.Cell(Firstrow, 6).Value = row["TDSCredit"].ToString();

                        Firstrow++;
                    }
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
            }
            string fileType = Common.GetMimeType("xls");
            Response.AddHeader("Content-Disposition", "filename=TDSReceivableInGSToffset.xls");
            return File(workStream, fileType);
        }
        public FileStreamResult OverheadReport()
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsTrasaction = db.GetOverhead();
                return toSpreadSheets(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ITTDSPayment(DateTime FromDate, DateTime ToDate, int BankId, int Accid)
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dsTrasaction = db.GetITTDSPayment(FromDate, ToDate, BankId, Accid);
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult GSTTDSPayment(DateTime FromDate, DateTime ToDate, int BankId)
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dsTrasaction = db.GetGSTTDSPayment(FromDate, ToDate, BankId);
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult OHReversalBankAdvice(int Id)
        {
            try
            {
                //  Id = Regex.Replace(Id, "/", "");
                var user = User.Identity.Name;
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dtResult = new DataSet();
                dtResult = db.GetOHReversal(Id);
                return toSpreadSheets(dtResult);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public FileStreamResult TDSPayment(string Id)
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dsTrasaction = db.GetTDSPayment(Id);
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult NegativeBalance(int NegId)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        ProjectService Projserv = new ProjectService();
                        CoreAccountsService CorServ = new CoreAccountsService();
                        NegativeBalanceModel model = new NegativeBalanceModel();
                        ProjectSummaryModel SumModel = new ProjectSummaryModel();
                        NegativeBalanceModel NegModel = new NegativeBalanceModel();
                        NegModel = CorServ.GetNegativeBalanceDetails(NegId);
                        int ProjectId = NegModel.ProjectId ?? 0;
                        SumModel = Projserv.getProjectSummary(ProjectId);
                        var ws = wb.Worksheets.Add("Prev Negative Balance");
                        ws.Cell(1, 1).Value = "NEGATIVE BALANCE APPROVAL FOR SPONSORED PROJECTS";
                        // ws.Cell("A1").Value = "Merged Row(1) of Range (A1:B1)";
                        ws.Range("A1:B1").Row(1).Merge();
                        ws.Cell(2, 1).Value = "NAME( " + SumModel.PIname + " )";
                        ws.Range("A2:B2").Row(1).Merge();
                        ws.Cell(4, 1).Value = "Project No:";
                        ws.Cell(4, 2).Value = SumModel.ProjectNo;
                        ws.Cell(5, 1).Value = "End Date";
                        ws.Cell(5, 2).Value = SumModel.CloseDate;
                        ws.Cell(6, 1).Value = "Date:";
                        ws.Cell(6, 2).Value = string.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                        ws.Cell(8, 2).Value = "(Amount in Rs)";
                        ws.Cell(9, 1).Value = "Project Cost";
                        ws.Cell(9, 2).Value = SumModel.SanctionedValue;
                        ws.Cell(10, 1).Value = "Amount Received So Far";
                        ws.Cell(10, 2).Value = SumModel.TotalGrantReceived;
                        ws.Cell(11, 1).Value = "Yet to be received";
                        ws.Cell(11, 2).Value = SumModel.SanctionedValue - SumModel.TotalGrantReceived;
                        ws.Cell(12, 1).Value = "Current Balance in the Project";
                        ws.Cell(12, 2).Value = SumModel.AvailableBalanceinProject;

                        var firstCell = ws.Cell(8, 1);
                        var lastCell = ws.Cell(12, 2);
                        var rang = ws.Range(firstCell, lastCell);
                        rang.Clear(XLClearOptions.AllFormats);
                        var table = rang.CreateTable();
                        table.Theme = XLTableTheme.TableStyleLight12;
                        table.ShowAutoFilter = false;
                        ws.Cell(15, 1).Value = "Project Number";
                        ws.Cell(15, 2).Value = "Start Date";
                        ws.Cell(15, 3).Value = "Close Date";
                        ws.Cell(15, 4).Value = "Project Cost";
                        ws.Cell(15, 5).Value = "Receipt";
                        //ws.Cell(15, 6).Value = "Expenditure";
                        //ws.Cell(15, 7).Value = "Commitment No";
                        ws.Cell(15, 6).Value = "Available Balance";
                        //ws.Cell(15, 9).Value = "Negative Balance";
                        //ws.Cell(15, 10).Value = "Balance";
                        int firstrow = 16;


                        List<PrevNegativeBalanceModel> list = coreaccountService.GetPrevNegativeBalance(ProjectId);
                        foreach (var item in list)
                        {
                            ws.Cell(firstrow, 1).Value = item.ProjectNo;
                            ws.Cell(firstrow, 2).Value = string.Format("{0:ddd dd-MMM-yyyy}", item.StartDate);
                            ws.Cell(firstrow, 3).Value = string.Format("{0:ddd dd-MMM-yyyy}", item.CloseDate);
                            ws.Cell(firstrow, 4).Value = item.ProjCost;
                            ws.Cell(firstrow, 5).Value = item.Receipt;
                            //ws.Cell(firstrow, 6).Value = item.Exp;
                            //ws.Cell(firstrow, 7).Value = item.Commit;
                            ws.Cell(firstrow, 6).Value = item.Net;
                            //ws.Cell(firstrow, 9).Value = item.NegBalance;
                            //ws.Cell(firstrow, 10).Value = item.Balance;
                            firstrow++;
                        }
                        ws.Cell(firstrow + 1, 6).Value = list.Sum(m => m.Net);

                        var firstCell1 = ws.Cell(15, 1);
                        var lastCell1 = ws.Cell(firstrow + 1, 6);
                        var range1 = ws.Range(firstCell1, lastCell1);
                        range1.Clear(XLClearOptions.AllFormats);
                        var table1 = range1.CreateTable();
                        table1.Theme = XLTableTheme.TableStyleLight12;
                        table1.ShowAutoFilter = false;
                        int secondrow = firstrow + 3;
                        ws.Cell(secondrow, 1).Value = "PI’s Request";
                        ws.Cell(secondrow + 1, 1).Value = "PI has requested negative balance of Rs." + NegModel.NegativeBalanceAmount + " towards project expenses.";
                        ws.Cell(secondrow + 3, 1).Value = "RECOMMENDATION";
                        decimal ToatlAmt = (NegModel.NegativeBalanceAmount) - (list.Sum(m => m.Net)) ?? 0;
                        ws.Cell(secondrow + 4, 1).Value = "STEO may kindly decide to approve a total negative balance up to Rs." + ToatlAmt;
                        ws.Cell(secondrow + 6, 1).Value = "Dean has approved via system.";
                        ws.Cell(secondrow + 9, 1).Value = "Dean";
                        ws.Cell(secondrow + 9, 2).Value = "Director";
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=NegativeBalance.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region TDS Refund
        public FileStreamResult Form26Q(int Finyear, int Quator)
        {
            try
            {

                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                        FinOp fac = new FinOp(System.DateTime.Now);
                        int CurrentYear = fromdate.Year;
                        int PreviousYear = fromdate.Year - 1;
                        int NextYear = fromdate.Year + 1;
                        string PreYear = PreviousYear.ToString();
                        string NexYear = NextYear.ToString();
                        string CurYear = CurrentYear.ToString();
                        DateTime[] QuarDate; DateTime Date1 = new DateTime();
                        DateTime Date2 = new DateTime(); DateTime Date3 = new DateTime();
                        DateTime startDate; DateTime endDate;
                        if (fromdate.Month > 2)
                        {
                            startDate = new DateTime(fromdate.Year, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        else
                        {
                            startDate = new DateTime((fromdate.Year) - 1, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        if (Quator == 1)
                        {
                            Date1 = new DateTime(startDate.Year, 4, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 5, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 6, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 2)
                        {
                            Date1 = new DateTime(startDate.Year, 7, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 8, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 9, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 3)
                        {
                            Date1 = new DateTime(startDate.Year, 10, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 11, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 12, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 4)
                        {
                            Date1 = new DateTime(endDate.Year, 1, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(endDate.Year, 2, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(endDate.Year, 3, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        ReportService reportservice = new ReportService();
                        /////////  first month sheet  ////////
                        Form24QModel model = new Form24QModel();
                        CoreAccountsService.ExecuteTDSPaymentSP();
                        model = ReportService.GetForm26Q(Date1);
                        DataSet dataset = new DataSet();
                        DataTable dtColumns1 = new DataTable();
                        DataTable dtColumns2 = new DataTable();
                        DataTable dtColumns3 = new DataTable();
                        DataTable dtColumns4 = new DataTable();
                        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94C);
                        dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94I);
                        dtColumns2 = JsonConvert.DeserializeObject<DataTable>(json2);
                        string json3 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94J);
                        dtColumns3 = JsonConvert.DeserializeObject<DataTable>(json3);
                        string json4 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94H);
                        dtColumns4 = JsonConvert.DeserializeObject<DataTable>(json4);
                        var ws = wb.Worksheets.Add("DEDUCTORDETAILS");
                        ws.Cell(1, 1).Value = "TAX DEDUCTION ACCOUNT NO";
                        ws.Cell(1, 2).Value = "CHEI04464F";
                        ws.Cell(2, 1).Value = "PERMANENT ACCOUNT NO";
                        ws.Cell(2, 2).Value = "AAAAI3615G";
                        ws.Cell(3, 1).Value = "FINANCIAL YEAR";
                        ws.Cell(3, 2).Value = startDate.Year + "-" + endDate.Year;
                        ws.Cell(4, 1).Value = "ASSESSMENT YEAR";
                        ws.Cell(4, 2).Value = startDate.Year + "-" + endDate.Year;
                        ws.Cell(5, 1).Value = "HAS ANY STATEMENT BEEN FILED EARLIER FOR THIS QUARTER";
                        ws.Cell(5, 2).Value = "NO";
                        ws.Cell(6, 1).Value = "IF ANSWER IS 'YES', THEN PROVISIONAL RPT NO OF ORIGINAL STATEMENT";
                        ws.Cell(6, 2).Value = "-NA-";
                        ws.Cell(7, 1).Value = "NAME OF THE DEDUCTOR";
                        ws.Cell(7, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY ";
                        ws.Cell(8, 1).Value = "TYPE OF DEDUCTOR";
                        ws.Cell(8, 2).Value = "GOVERNMENT";
                        ws.Cell(9, 1).Value = "BRANCH/DIVISION(if any)";
                        ws.Cell(9, 2).Value = "NO";
                        ws.Cell(10, 1).Value = "ADDRESS 1";
                        ws.Cell(10, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws.Cell(11, 1).Value = "ADDRESS 2";
                        ws.Cell(11, 2).Value = "MADRAS";
                        ws.Cell(12, 1).Value = "ADDRESS 3";
                        ws.Cell(12, 2).Value = "CHENNAI-36";
                        ws.Cell(13, 1).Value = "ADDRESS 4";
                        ws.Cell(13, 2).Value = "";
                        ws.Cell(14, 1).Value = "ADDRESS 5";
                        ws.Cell(14, 2).Value = "";
                        ws.Cell(15, 1).Value = "STATE";
                        ws.Cell(15, 2).Value = "TAMIL NADU";
                        ws.Cell(16, 1).Value = "PINCODE";
                        ws.Cell(16, 2).Value = "600036";
                        ws.Cell(17, 1).Value = "TELEPHONE NO";
                        ws.Cell(17, 2).Value = "22578355";
                        ws.Cell(18, 1).Value = "EMAIL";
                        ws.Cell(18, 2).Value = "";
                        ws.Cell(19, 1).Value = "NAME OF THE PERSON RESPONSIBLE FOR DEDUCTION OF TAX";
                        ws.Cell(19, 2).Value = "";
                        ws.Cell(20, 1).Value = "ADDRESS 1";
                        ws.Cell(20, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws.Cell(21, 1).Value = "ADDRESS 2";
                        ws.Cell(21, 2).Value = "MADRAS";
                        ws.Cell(22, 1).Value = "ADDRESS 3";
                        ws.Cell(22, 2).Value = "CHENNAI-36";
                        ws.Cell(23, 1).Value = "ADDRESS 4";
                        ws.Cell(23, 2).Value = "";
                        ws.Cell(24, 1).Value = "ADDRESS 5";
                        ws.Cell(24, 2).Value = "";
                        ws.Cell(25, 1).Value = "STATE";
                        ws.Cell(25, 2).Value = "TAMIL NADU";
                        ws.Cell(26, 1).Value = "PINCODE";
                        ws.Cell(26, 2).Value = "600036";
                        ws.Cell(27, 1).Value = "TELEPHONE NO";
                        ws.Cell(27, 2).Value = "22578355";
                        ws.Cell(28, 1).Value = "EMAIL";
                        ws.Cell(28, 2).Value = "";
                        ws.Range("A1:B28").Style.Font.Bold = true;
                        ////
                        var ws1 = wb.Worksheets.Add("CHALLAN DETAILS");

                        ////////Sheet 3 ///////
                        var ws2 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date1));
                        ws2.Cell(1, 1).Value = "Sr No.";
                        ws2.Cell(1, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws2.Cell(1, 3).Value = "Section Under Payment Made";
                        ws2.Cell(1, 4).Value = "PAN of the Deductee";
                        ws2.Cell(1, 5).Value = "Name of the deductee";
                        ws2.Cell(1, 6).Value = "Date of Payment/credit";
                        ws2.Cell(1, 7).Value = "Amount Paid/Credited Rs.";
                        ws2.Cell(1, 8).Value = "Paid by Book Entry or otherwise";
                        ws2.Cell(1, 9).Value = "TDS Rs.";
                        ws2.Cell(1, 10).Value = "Surcharge Rs.";
                        ws2.Cell(1, 11).Value = "Education Cess Rs.";
                        ws2.Cell(1, 12).Value = "Total tax deducted (421+422+423)";
                        ws2.Cell(1, 13).Value = "Total tax deposited Rs.";
                        ws2.Cell(1, 14).Value = "Interest";
                        ws2.Cell(1, 15).Value = "Others";
                        ws2.Cell(1, 16).Value = "Total (425+Interest+  Others)";
                        ws2.Cell(1, 17).Value = "BSR Code";
                        ws2.Cell(1, 18).Value = "Challan No.";
                        ws2.Cell(1, 19).Value = "Date of Deposit";
                        ws2.Cell(1, 20).Value = "Rate at which deducted";
                        ws2.Cell(1, 21).Value = "Reason for non-deduction/lower deduction";
                        ws2.Cell(2, 1).Value = "(414)";
                        ws2.Cell(2, 2).Value = "(415)";
                        ws2.Cell(2, 3).Value = "(415A)";
                        ws2.Cell(2, 4).Value = "(416)";
                        ws2.Cell(2, 5).Value = "(417)";
                        ws2.Cell(2, 6).Value = "(418)";
                        ws2.Cell(2, 7).Value = "(419)";
                        ws2.Cell(2, 8).Value = "(420)";
                        ws2.Cell(2, 9).Value = "(421)";
                        ws2.Cell(2, 10).Value = "(422)";
                        ws2.Cell(2, 11).Value = "(423)";
                        ws2.Cell(2, 12).Value = "(424)";
                        ws2.Cell(2, 13).Value = "(425)";
                        ws2.Cell(2, 14).Value = "(425A)";
                        ws2.Cell(2, 15).Value = "(425B)";
                        ws2.Cell(2, 16).Value = "(425C)";
                        ws2.Cell(2, 17).Value = "(425D)";
                        ws2.Cell(2, 18).Value = "(425E)";
                        ws2.Cell(2, 19).Value = "(426)";
                        ws2.Cell(2, 20).Value = "(427)";
                        ws2.Cell(2, 21).Value = "(428)";
                        int ws2firstrow = 3; int ws2firstcol = 1;
                        //  List<PrevNegativeBalanceModel> list = coreaccountService.GetPrevNegativeBalance(ProjectId);
                        foreach (var item in model.TDS94C)
                        {

                            ws2.Cell(ws2firstrow, 1).Value = item.SlNo;
                            ws2.Cell(ws2firstrow, 2).Value = "";
                            ws2.Cell(ws2firstrow, 3).Value = "94C";
                            ws2.Cell(ws2firstrow, 4).Value = item.PAN;
                            ws2.Cell(ws2firstrow, 5).Value = item.Name;
                            ws2.Cell(ws2firstrow, 6).Value = item.DateofPayment;
                            ws2.Cell(ws2firstrow, 7).Value = item.AmountPaid;
                            ws2.Cell(ws2firstrow, 8).Value = "";
                            ws2.Cell(ws2firstrow, 9).Value = item.TDS;
                            ws2.Cell(ws2firstrow, 10).Value = "-";
                            ws2.Cell(ws2firstrow, 11).Value = "-";
                            ws2.Cell(ws2firstrow, 12).Value = item.TDS;
                            ws2.Cell(ws2firstrow, 13).Value = item.TDS;
                            ws2.Cell(ws2firstrow, 14).Value = "-";
                            ws2.Cell(ws2firstrow, 15).Value = "-";
                            ws2.Cell(ws2firstrow, 16).Value = item.TDS;
                            ws2.Cell(ws2firstrow, 17).Value = item.BSRCode;
                            ws2.Cell(ws2firstrow, 18).Value = item.ChallenNo;
                            ws2.Cell(ws2firstrow, 19).Value = item.DateofPayment;
                            ws2.Cell(ws2firstrow, 20).Value = item.Rate;
                            ws2.Cell(ws2firstrow, 21).Value = "-NA-";
                            ws2firstrow++;
                        }
                        int ws2Total = ws2firstrow + 1;
                        ws2.Cell(ws2Total, 5).Value = "TOTAL (SEC.194C)";
                        ws2.Cell(ws2Total, 7).Value = model.TDS94C.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2Total, 9).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 12).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 13).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 16).Value = model.TDS94C.Sum(m => m.TDS);

                        //var firstCell = ws2.FirstCellUsed();
                        //var lastCell = ws2.LastCellUsed();
                        //var range = ws2.Range(firstCell, lastCell);
                        //range.Clear(XLClearOptions.AllFormats);
                        //var table = range.CreateTable();
                        //table.Theme = XLTableTheme.TableStyleLight12;
                        //table.ShowAutoFilter = false;

                        int ws2secondrow = ws2Total + 2;
                        ws2.Cell(ws2secondrow, 1).Value = "Sr No.";
                        ws2.Cell(ws2secondrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws2.Cell(ws2secondrow, 3).Value = "Section Under Payment Made";
                        ws2.Cell(ws2secondrow, 4).Value = "PAN of the Deductee";
                        ws2.Cell(ws2secondrow, 5).Value = "Name of the deductee";
                        ws2.Cell(ws2secondrow, 6).Value = "Date of Payment/credit";
                        ws2.Cell(ws2secondrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws2.Cell(ws2secondrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws2.Cell(ws2secondrow, 9).Value = "TDS Rs.";
                        ws2.Cell(ws2secondrow, 10).Value = "Surcharge Rs.";
                        ws2.Cell(ws2secondrow, 11).Value = "Education Cess Rs.";
                        ws2.Cell(ws2secondrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws2.Cell(ws2secondrow, 13).Value = "Total tax deposited Rs.";
                        ws2.Cell(ws2secondrow, 14).Value = "Interest";
                        ws2.Cell(ws2secondrow, 15).Value = "Others";
                        ws2.Cell(ws2secondrow, 16).Value = "Total (425+Interest+  Others)";
                        ws2.Cell(ws2secondrow, 17).Value = "BSR Code";
                        ws2.Cell(ws2secondrow, 18).Value = "Challan No.";
                        ws2.Cell(ws2secondrow, 19).Value = "Date of Deposit";
                        ws2.Cell(ws2secondrow, 20).Value = "Rate at which deducted";
                        ws2.Cell(ws2secondrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws2.Cell(ws2secondrow + 1, 1).Value = "(414)";
                        ws2.Cell(ws2secondrow + 1, 2).Value = "(415)";
                        ws2.Cell(ws2secondrow + 1, 3).Value = "(415A)";
                        ws2.Cell(ws2secondrow + 1, 4).Value = "(416)";
                        ws2.Cell(ws2secondrow + 1, 5).Value = "(417)";
                        ws2.Cell(ws2secondrow + 1, 6).Value = "(418)";
                        ws2.Cell(ws2secondrow + 1, 7).Value = "(419)";
                        ws2.Cell(ws2secondrow + 1, 8).Value = "(420)";
                        ws2.Cell(ws2secondrow + 1, 9).Value = "(421)";
                        ws2.Cell(ws2secondrow + 1, 10).Value = "(422)";
                        ws2.Cell(ws2secondrow + 1, 11).Value = "(423)";
                        ws2.Cell(ws2secondrow + 1, 12).Value = "(424)";
                        ws2.Cell(ws2secondrow + 1, 13).Value = "(425)";
                        ws2.Cell(ws2secondrow + 1, 14).Value = "(425A)";
                        ws2.Cell(ws2secondrow + 1, 15).Value = "(425B)";
                        ws2.Cell(ws2secondrow + 1, 16).Value = "(425C)";
                        ws2.Cell(ws2secondrow + 1, 17).Value = "(425D)";
                        ws2.Cell(ws2secondrow + 1, 18).Value = "(425E)";
                        ws2.Cell(ws2secondrow + 1, 19).Value = "(426)";
                        ws2.Cell(ws2secondrow + 1, 20).Value = "(427)";
                        ws2.Cell(ws2secondrow + 1, 21).Value = "(428)";
                        int ws2secondrowlist = ws2secondrow + 2;

                        foreach (var item in model.TDS94I)
                        {
                            ws2.Cell(ws2secondrowlist, 1).Value = item.SlNo;
                            ws2.Cell(ws2secondrowlist, 2).Value = "";
                            ws2.Cell(ws2secondrowlist, 3).Value = "94I";
                            ws2.Cell(ws2secondrowlist, 4).Value = item.PAN;
                            ws2.Cell(ws2secondrowlist, 5).Value = item.Name;
                            ws2.Cell(ws2secondrowlist, 6).Value = item.DateofPayment;
                            ws2.Cell(ws2secondrowlist, 7).Value = item.AmountPaid;
                            ws2.Cell(ws2secondrowlist, 8).Value = "";
                            ws2.Cell(ws2secondrowlist, 9).Value = item.TDS;
                            ws2.Cell(ws2secondrowlist, 10).Value = "-";
                            ws2.Cell(ws2secondrowlist, 11).Value = "-";
                            ws2.Cell(ws2secondrowlist, 12).Value = item.TDS;
                            ws2.Cell(ws2secondrowlist, 13).Value = item.TDS;
                            ws2.Cell(ws2secondrowlist, 14).Value = "-";
                            ws2.Cell(ws2secondrowlist, 15).Value = "-";
                            ws2.Cell(ws2secondrowlist, 16).Value = item.TDS;
                            ws2.Cell(ws2secondrowlist, 17).Value = item.BSRCode;
                            ws2.Cell(ws2secondrowlist, 18).Value = item.ChallenNo;
                            ws2.Cell(ws2secondrowlist, 19).Value = item.DateofPayment;
                            ws2.Cell(ws2secondrowlist, 20).Value = item.Rate;
                            ws2.Cell(ws2secondrowlist, 21).Value = "-NA-";
                            ws2secondrowlist++;
                        }
                        int ws2ITotal = ws2secondrowlist + 1;
                        ws2.Cell(ws2ITotal, 5).Value = "TOTAL (SEC.194I)";
                        ws2.Cell(ws2ITotal, 7).Value = model.TDS94I.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2ITotal, 9).Value = model.TDS94I.Sum(m => m.TDS);
                        ws2.Cell(ws2ITotal, 12).Value = model.TDS94I.Sum(m => m.TDS);
                        ws2.Cell(ws2ITotal, 13).Value = model.TDS94I.Sum(m => m.TDS);
                        ws2.Cell(ws2ITotal, 16).Value = model.TDS94I.Sum(m => m.TDS);



                        //var firstCell1 = ws2.Cell(ws2Total + 2, 1);
                        //var lastCell1 = ws2.Cell(ws2ITotal, 21);
                        //var range1 = ws2.Range(firstCell1, lastCell1);
                        //range1.Clear(XLClearOptions.AllFormats);
                        //var table1 = range1.CreateTable();
                        //table1.Theme = XLTableTheme.TableStyleLight12;
                        //table1.ShowAutoFilter = false;

                        int ws2thirdrow = ws2ITotal + 2;
                        ws2.Cell(ws2thirdrow, 1).Value = "Sr No.";
                        ws2.Cell(ws2thirdrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws2.Cell(ws2thirdrow, 3).Value = "Section Under Payment Made";
                        ws2.Cell(ws2thirdrow, 4).Value = "PAN of the Deductee";
                        ws2.Cell(ws2thirdrow, 5).Value = "Name of the deductee";
                        ws2.Cell(ws2thirdrow, 6).Value = "Date of Payment/credit";
                        ws2.Cell(ws2thirdrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws2.Cell(ws2thirdrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws2.Cell(ws2thirdrow, 9).Value = "TDS Rs.";
                        ws2.Cell(ws2thirdrow, 10).Value = "Surcharge Rs.";
                        ws2.Cell(ws2thirdrow, 11).Value = "Education Cess Rs.";
                        ws2.Cell(ws2thirdrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws2.Cell(ws2thirdrow, 13).Value = "Total tax deposited Rs.";
                        ws2.Cell(ws2thirdrow, 14).Value = "Interest";
                        ws2.Cell(ws2thirdrow, 15).Value = "Others";
                        ws2.Cell(ws2thirdrow, 16).Value = "Total (425+Interest+  Others)";
                        ws2.Cell(ws2thirdrow, 17).Value = "BSR Code";
                        ws2.Cell(ws2thirdrow, 18).Value = "Challan No.";
                        ws2.Cell(ws2thirdrow, 19).Value = "Date of Deposit";
                        ws2.Cell(ws2thirdrow, 20).Value = "Rate at which deducted";
                        ws2.Cell(ws2thirdrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws2.Cell(ws2thirdrow + 1, 1).Value = "(414)";
                        ws2.Cell(ws2thirdrow + 1, 2).Value = "(415)";
                        ws2.Cell(ws2thirdrow + 1, 3).Value = "(415A)";
                        ws2.Cell(ws2thirdrow + 1, 4).Value = "(416)";
                        ws2.Cell(ws2thirdrow + 1, 5).Value = "(417)";
                        ws2.Cell(ws2thirdrow + 1, 6).Value = "(418)";
                        ws2.Cell(ws2thirdrow + 1, 7).Value = "(419)";
                        ws2.Cell(ws2thirdrow + 1, 8).Value = "(420)";
                        ws2.Cell(ws2thirdrow + 1, 9).Value = "(421)";
                        ws2.Cell(ws2thirdrow + 1, 10).Value = "(422)";
                        ws2.Cell(ws2thirdrow + 1, 11).Value = "(423)";
                        ws2.Cell(ws2thirdrow + 1, 12).Value = "(424)";
                        ws2.Cell(ws2thirdrow + 1, 13).Value = "(425)";
                        ws2.Cell(ws2thirdrow + 1, 14).Value = "(425A)";
                        ws2.Cell(ws2thirdrow + 1, 15).Value = "(425B)";
                        ws2.Cell(ws2thirdrow + 1, 16).Value = "(425C)";
                        ws2.Cell(ws2thirdrow + 1, 17).Value = "(425D)";
                        ws2.Cell(ws2thirdrow + 1, 18).Value = "(425E)";
                        ws2.Cell(ws2thirdrow + 1, 19).Value = "(426)";
                        ws2.Cell(ws2thirdrow + 1, 20).Value = "(427)";
                        ws2.Cell(ws2thirdrow + 1, 21).Value = "(428)";
                        int ws2thirdrowlist = ws2thirdrow + 2;
                        foreach (var item in model.TDS94J)

                        {
                            ws2.Cell(ws2thirdrowlist, 1).Value = item.SlNo;
                            ws2.Cell(ws2thirdrowlist, 2).Value = "";
                            ws2.Cell(ws2thirdrowlist, 3).Value = "94J";
                            ws2.Cell(ws2thirdrowlist, 4).Value = item.PAN;
                            ws2.Cell(ws2thirdrowlist, 5).Value = item.Name;
                            ws2.Cell(ws2thirdrowlist, 6).Value = item.DateofPayment;
                            ws2.Cell(ws2thirdrowlist, 7).Value = item.AmountPaid;
                            ws2.Cell(ws2thirdrowlist, 8).Value = "";
                            ws2.Cell(ws2thirdrowlist, 9).Value = item.TDS;
                            ws2.Cell(ws2thirdrowlist, 10).Value = "-";
                            ws2.Cell(ws2thirdrowlist, 11).Value = "-";
                            ws2.Cell(ws2thirdrowlist, 12).Value = item.TDS;
                            ws2.Cell(ws2thirdrowlist, 13).Value = item.TDS;
                            ws2.Cell(ws2thirdrowlist, 14).Value = "-";
                            ws2.Cell(ws2thirdrowlist, 15).Value = "-";
                            ws2.Cell(ws2thirdrowlist, 16).Value = item.TDS;
                            ws2.Cell(ws2thirdrowlist, 17).Value = item.BSRCode;
                            ws2.Cell(ws2thirdrowlist, 18).Value = item.ChallenNo;
                            ws2.Cell(ws2thirdrowlist, 19).Value = item.DateofPayment;
                            ws2.Cell(ws2thirdrowlist, 20).Value = item.Rate;
                            ws2.Cell(ws2thirdrowlist, 21).Value = "-NA-";
                            ws2thirdrowlist++;
                        }
                        int ws2JTotal = ws2thirdrowlist + 1;
                        ws2.Cell(ws2JTotal, 5).Value = "TOTAL (SEC.194J)";
                        ws2.Cell(ws2JTotal, 7).Value = model.TDS94J.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2JTotal, 9).Value = model.TDS94J.Sum(m => m.TDS);
                        ws2.Cell(ws2JTotal, 12).Value = model.TDS94J.Sum(m => m.TDS);
                        ws2.Cell(ws2JTotal, 13).Value = model.TDS94J.Sum(m => m.TDS);
                        ws2.Cell(ws2JTotal, 16).Value = model.TDS94J.Sum(m => m.TDS);

                        //var firstCell2 = ws2.Cell(ws2ITotal + 2, 1);
                        //var lastCell2 = ws2.Cell(ws2JTotal, 21);
                        //var range2 = ws2.Range(firstCell2, lastCell2);
                        //range2.Clear(XLClearOptions.AllFormats);
                        //var table2 = range2.CreateTable();
                        //table2.Theme = XLTableTheme.TableStyleLight12;
                        //table2.ShowAutoFilter = false;

                        int ws2fourthrow = ws2JTotal + 2;
                        ws2.Cell(ws2fourthrow, 1).Value = "Sr No.";
                        ws2.Cell(ws2fourthrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws2.Cell(ws2fourthrow, 3).Value = "Section Under Payment Made";
                        ws2.Cell(ws2fourthrow, 4).Value = "PAN of the Deductee";
                        ws2.Cell(ws2fourthrow, 5).Value = "Name of the deductee";
                        ws2.Cell(ws2fourthrow, 6).Value = "Date of Payment/credit";
                        ws2.Cell(ws2fourthrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws2.Cell(ws2fourthrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws2.Cell(ws2fourthrow, 9).Value = "TDS Rs.";
                        ws2.Cell(ws2fourthrow, 10).Value = "Surcharge Rs.";
                        ws2.Cell(ws2fourthrow, 11).Value = "Education Cess Rs.";
                        ws2.Cell(ws2fourthrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws2.Cell(ws2fourthrow, 13).Value = "Total tax deposited Rs.";
                        ws2.Cell(ws2fourthrow, 14).Value = "Interest";
                        ws2.Cell(ws2fourthrow, 15).Value = "Others";
                        ws2.Cell(ws2fourthrow, 16).Value = "Total (425+Interest+  Others)";
                        ws2.Cell(ws2fourthrow, 17).Value = "BSR Code";
                        ws2.Cell(ws2fourthrow, 18).Value = "Challan No.";
                        ws2.Cell(ws2fourthrow, 19).Value = "Date of Deposit";
                        ws2.Cell(ws2fourthrow, 20).Value = "Rate at which deducted";
                        ws2.Cell(ws2fourthrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws2.Cell(ws2fourthrow + 1, 1).Value = "(414)";
                        ws2.Cell(ws2fourthrow + 1, 2).Value = "(415)";
                        ws2.Cell(ws2fourthrow + 1, 3).Value = "(415A)";
                        ws2.Cell(ws2fourthrow + 1, 4).Value = "(416)";
                        ws2.Cell(ws2fourthrow + 1, 5).Value = "(417)";
                        ws2.Cell(ws2fourthrow + 1, 6).Value = "(418)";
                        ws2.Cell(ws2fourthrow + 1, 7).Value = "(419)";
                        ws2.Cell(ws2fourthrow + 1, 8).Value = "(420)";
                        ws2.Cell(ws2fourthrow + 1, 9).Value = "(421)";
                        ws2.Cell(ws2fourthrow + 1, 10).Value = "(422)";
                        ws2.Cell(ws2fourthrow + 1, 11).Value = "(423)";
                        ws2.Cell(ws2fourthrow + 1, 12).Value = "(424)";
                        ws2.Cell(ws2fourthrow + 1, 13).Value = "(425)";
                        ws2.Cell(ws2fourthrow + 1, 14).Value = "(425A)";
                        ws2.Cell(ws2fourthrow + 1, 15).Value = "(425B)";
                        ws2.Cell(ws2fourthrow + 1, 16).Value = "(425C)";
                        ws2.Cell(ws2fourthrow + 1, 17).Value = "(425D)";
                        ws2.Cell(ws2fourthrow + 1, 18).Value = "(425E)";
                        ws2.Cell(ws2fourthrow + 1, 19).Value = "(426)";
                        ws2.Cell(ws2fourthrow + 1, 20).Value = "(427)";
                        ws2.Cell(ws2fourthrow + 1, 21).Value = "(428)";
                        int ws2fourthrowlist = ws2fourthrow + 2;
                        foreach (var item in model.TDS94H)

                        {
                            ws2.Cell(ws2fourthrowlist, 1).Value = item.SlNo;
                            ws2.Cell(ws2fourthrowlist, 2).Value = "";
                            ws2.Cell(ws2fourthrowlist, 3).Value = "94H";
                            ws2.Cell(ws2fourthrowlist, 5).Value = item.Name;
                            ws2.Cell(ws2fourthrowlist, 6).Value = item.DateofPayment;
                            ws2.Cell(ws2fourthrowlist, 7).Value = item.AmountPaid;
                            ws2.Cell(ws2fourthrowlist, 8).Value = "";
                            ws2.Cell(ws2fourthrowlist, 9).Value = item.TDS;
                            ws2.Cell(ws2fourthrowlist, 10).Value = "-";
                            ws2.Cell(ws2fourthrowlist, 11).Value = "-";
                            ws2.Cell(ws2fourthrowlist, 12).Value = item.TDS;
                            ws2.Cell(ws2fourthrowlist, 13).Value = item.TDS;
                            ws2.Cell(ws2fourthrowlist, 14).Value = "-";
                            ws2.Cell(ws2fourthrowlist, 15).Value = "-";
                            ws2.Cell(ws2fourthrowlist, 16).Value = item.TDS;
                            ws2.Cell(ws2fourthrowlist, 17).Value = item.BSRCode;
                            ws2.Cell(ws2fourthrowlist, 18).Value = item.ChallenNo;
                            ws2.Cell(ws2fourthrowlist, 19).Value = item.DateofPayment;
                            ws2.Cell(ws2fourthrowlist, 20).Value = item.Rate;
                            ws2.Cell(ws2fourthrowlist, 21).Value = "-NA-";
                            ws2fourthrowlist++;
                        }
                        int ws2HTotal = ws2fourthrowlist + 1;
                        ws2.Cell(ws2HTotal, 5).Value = "TOTAL (SEC.194H)";
                        ws2.Cell(ws2HTotal, 7).Value = model.TDS94H.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2HTotal, 9).Value = model.TDS94H.Sum(m => m.TDS);
                        ws2.Cell(ws2HTotal, 12).Value = model.TDS94H.Sum(m => m.TDS);
                        ws2.Cell(ws2HTotal, 13).Value = model.TDS94H.Sum(m => m.TDS);
                        ws2.Cell(ws2HTotal, 16).Value = model.TDS94H.Sum(m => m.TDS);


                        //var firstCell3 = ws2.Cell(ws2JTotal + 2, 1);
                        //var lastCell3 = ws2.Cell(ws2HTotal, 21);
                        //var range3 = ws2.Range(firstCell3, lastCell3);
                        //range3.Clear(XLClearOptions.AllFormats);
                        //var table3 = range3.CreateTable();
                        //table3.Theme = XLTableTheme.TableStyleLight12;
                        //table3.ShowAutoFilter = false;

                        ////////////////////////////////

                        /////////  second month sheet  ////////
                        Form24QModel model1 = new Form24QModel();
                        model1 = ReportService.GetForm26Q(Date2);
                        DataTable dtColumns5 = new DataTable();
                        DataTable dtColumns6 = new DataTable();
                        DataTable dtColumns7 = new DataTable();
                        DataTable dtColumns8 = new DataTable();
                        string json5 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94C);
                        dtColumns5 = JsonConvert.DeserializeObject<DataTable>(json5);
                        string json6 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94I);
                        dtColumns6 = JsonConvert.DeserializeObject<DataTable>(json6);
                        string json7 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94J);
                        dtColumns7 = JsonConvert.DeserializeObject<DataTable>(json7);
                        string json8 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94H);
                        dtColumns8 = JsonConvert.DeserializeObject<DataTable>(json8);
                        var ws3 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date2));
                        ws3.Cell(1, 1).Value = "Sr No.";
                        ws3.Cell(1, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws3.Cell(1, 3).Value = "Section Under Payment Made";
                        ws3.Cell(1, 4).Value = "PAN of the Deductee";
                        ws3.Cell(1, 5).Value = "Name of the deductee";
                        ws3.Cell(1, 6).Value = "Date of Payment/credit";
                        ws3.Cell(1, 7).Value = "Amount Paid/Credited Rs.";
                        ws3.Cell(1, 8).Value = "Paid by Book Entry or otherwise";
                        ws3.Cell(1, 9).Value = "TDS Rs.";
                        ws3.Cell(1, 10).Value = "Surcharge Rs.";
                        ws3.Cell(1, 11).Value = "Education Cess Rs.";
                        ws3.Cell(1, 12).Value = "Total tax deducted (421+422+423)";
                        ws3.Cell(1, 13).Value = "Total tax deposited Rs.";
                        ws3.Cell(1, 14).Value = "Interest";
                        ws3.Cell(1, 15).Value = "Others";
                        ws3.Cell(1, 16).Value = "Total (425+Interest+  Others)";
                        ws3.Cell(1, 17).Value = "BSR Code";
                        ws3.Cell(1, 18).Value = "Challan No.";
                        ws3.Cell(1, 19).Value = "Date of Deposit";
                        ws3.Cell(1, 20).Value = "Rate at which deducted";
                        ws3.Cell(1, 21).Value = "Reason for non-deduction/lower deduction";
                        ws3.Cell(2, 1).Value = "(414)";
                        ws3.Cell(2, 2).Value = "(415)";
                        ws3.Cell(2, 3).Value = "(415A)";
                        ws3.Cell(2, 4).Value = "(416)";
                        ws3.Cell(2, 5).Value = "(417)";
                        ws3.Cell(2, 6).Value = "(418)";
                        ws3.Cell(2, 7).Value = "(419)";
                        ws3.Cell(2, 8).Value = "(420)";
                        ws3.Cell(2, 9).Value = "(421)";
                        ws3.Cell(2, 10).Value = "(422)";
                        ws3.Cell(2, 11).Value = "(423)";
                        ws3.Cell(2, 12).Value = "(424)";
                        ws3.Cell(2, 13).Value = "(425)";
                        ws3.Cell(2, 14).Value = "(425A)";
                        ws3.Cell(2, 15).Value = "(425B)";
                        ws3.Cell(2, 16).Value = "(425C)";
                        ws3.Cell(2, 17).Value = "(425D)";
                        ws3.Cell(2, 18).Value = "(425E)";
                        ws3.Cell(2, 19).Value = "(426)";
                        ws3.Cell(2, 20).Value = "(427)";
                        ws3.Cell(2, 21).Value = "(428)";
                        int ws3firstrow = 3; int ws3firstcol = 1;
                        foreach (DataRow row in dtColumns5.Rows)
                        {
                            ws3.Cell(ws3firstrow, 1).Value = row["SlNo"].ToString();
                            ws3.Cell(ws3firstrow, 2).Value = "";
                            ws3.Cell(ws3firstrow, 3).Value = "94C";
                            ws3.Cell(ws3firstrow, 4).Value = row["PAN"].ToString();
                            ws3.Cell(ws3firstrow, 5).Value = row["Name"].ToString();
                            ws3.Cell(ws3firstrow, 6).Value = row["DateofPayment"];
                            ws3.Cell(ws3firstrow, 7).Value = row["AmountPaid"].ToString();
                            ws3.Cell(ws3firstrow, 8).Value = "";
                            ws3.Cell(ws3firstrow, 9).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 10).Value = "-";
                            ws3.Cell(ws3firstrow, 11).Value = "-";
                            ws3.Cell(ws3firstrow, 12).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 13).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 14).Value = "-";
                            ws3.Cell(ws3firstrow, 15).Value = "-";
                            ws3.Cell(ws3firstrow, 16).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 17).Value = row["BSRCode"].ToString();
                            ws3.Cell(ws3firstrow, 18).Value = row["ChallenNo"].ToString();
                            ws3.Cell(ws3firstrow, 19).Value = row["DateofPayment"];
                            ws3.Cell(ws3firstrow, 20).Value = row["Rate"].ToString();
                            ws3.Cell(ws3firstrow, 21).Value = "-NA-";
                            ws3firstrow++;
                        }
                        int ws3Total = ws3firstrow + 1;
                        ws3.Cell(ws3Total, 5).Value = "TOTAL (SEC.194C)";
                        ws3.Cell(ws3Total, 7).Value = model1.TDS94C.Sum(m => m.AmountPaid);
                        ws3.Cell(ws3Total, 9).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws3.Cell(ws3Total, 12).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws3.Cell(ws3Total, 13).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws3.Cell(ws3Total, 16).Value = model1.TDS94C.Sum(m => m.TDS);

                        //var firstCell4 = ws3.FirstCellUsed();
                        //var lastCell4 = ws3.LastCellUsed();
                        //var range4 = ws3.Range(firstCell4, lastCell4);
                        //range4.Clear(XLClearOptions.AllFormats);
                        //var table4 = range4.CreateTable();
                        //table4.Theme = XLTableTheme.TableStyleLight12;
                        //table4.ShowAutoFilter = false;

                        int ws3secondrow = ws3Total + 2;
                        ws3.Cell(ws3secondrow, 1).Value = "Sr No.";
                        ws3.Cell(ws3secondrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws3.Cell(ws3secondrow, 3).Value = "Section Under Payment Made";
                        ws3.Cell(ws3secondrow, 4).Value = "PAN of the Deductee";
                        ws3.Cell(ws3secondrow, 5).Value = "Name of the deductee";
                        ws3.Cell(ws3secondrow, 6).Value = "Date of Payment/credit";
                        ws3.Cell(ws3secondrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws3.Cell(ws3secondrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws3.Cell(ws3secondrow, 9).Value = "TDS Rs.";
                        ws3.Cell(ws3secondrow, 10).Value = "Surcharge Rs.";
                        ws3.Cell(ws3secondrow, 11).Value = "Education Cess Rs.";
                        ws3.Cell(ws3secondrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws3.Cell(ws3secondrow, 13).Value = "Total tax deposited Rs.";
                        ws3.Cell(ws3secondrow, 14).Value = "Interest";
                        ws3.Cell(ws3secondrow, 15).Value = "Others";
                        ws3.Cell(ws3secondrow, 16).Value = "Total (425+Interest+  Others)";
                        ws3.Cell(ws3secondrow, 17).Value = "BSR Code";
                        ws3.Cell(ws3secondrow, 18).Value = "Challan No.";
                        ws3.Cell(ws3secondrow, 19).Value = "Date of Deposit";
                        ws3.Cell(ws3secondrow, 20).Value = "Rate at which deducted";
                        ws3.Cell(ws3secondrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws3.Cell(ws3secondrow + 1, 1).Value = "(414)";
                        ws3.Cell(ws3secondrow + 1, 2).Value = "(415)";
                        ws3.Cell(ws3secondrow + 1, 3).Value = "(415A)";
                        ws3.Cell(ws3secondrow + 1, 4).Value = "(416)";
                        ws3.Cell(ws3secondrow + 1, 5).Value = "(417)";
                        ws3.Cell(ws3secondrow + 1, 6).Value = "(418)";
                        ws3.Cell(ws3secondrow + 1, 7).Value = "(419)";
                        ws3.Cell(ws3secondrow + 1, 8).Value = "(420)";
                        ws3.Cell(ws3secondrow + 1, 9).Value = "(421)";
                        ws3.Cell(ws3secondrow + 1, 10).Value = "(422)";
                        ws3.Cell(ws3secondrow + 1, 11).Value = "(423)";
                        ws3.Cell(ws3secondrow + 1, 12).Value = "(424)";
                        ws3.Cell(ws3secondrow + 1, 13).Value = "(425)";
                        ws3.Cell(ws3secondrow + 1, 14).Value = "(425A)";
                        ws3.Cell(ws3secondrow + 1, 15).Value = "(425B)";
                        ws3.Cell(ws3secondrow + 1, 16).Value = "(425C)";
                        ws3.Cell(ws3secondrow + 1, 17).Value = "(425D)";
                        ws3.Cell(ws3secondrow + 1, 18).Value = "(425E)";
                        ws3.Cell(ws3secondrow + 1, 19).Value = "(426)";
                        ws3.Cell(ws3secondrow + 1, 20).Value = "(427)";
                        ws3.Cell(ws3secondrow + 1, 21).Value = "(428)";
                        int ws3secondrowlist = ws3secondrow + 2;
                        foreach (DataRow row in dtColumns6.Rows)
                        {
                            ws3.Cell(ws3secondrowlist, 1).Value = row["SlNo"].ToString();
                            ws3.Cell(ws3secondrowlist, 2).Value = "";
                            ws3.Cell(ws3secondrowlist, 3).Value = "94I";
                            ws3.Cell(ws3secondrowlist, 4).Value = row["PAN"].ToString();
                            ws3.Cell(ws3secondrowlist, 5).Value = row["Name"].ToString();
                            ws3.Cell(ws3secondrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3secondrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws3.Cell(ws3secondrowlist, 8).Value = "";
                            ws3.Cell(ws3secondrowlist, 9).Value = row["TDS"].ToString();
                            ws3.Cell(ws3secondrowlist, 10).Value = "-";
                            ws3.Cell(ws3secondrowlist, 11).Value = "-";
                            ws3.Cell(ws3secondrowlist, 12).Value = row["TDS"].ToString();
                            ws3.Cell(ws3secondrowlist, 13).Value = row["TDS"].ToString();
                            ws3.Cell(ws3secondrowlist, 14).Value = "-";
                            ws3.Cell(ws3secondrowlist, 15).Value = "-";
                            ws3.Cell(ws3secondrowlist, 16).Value = row["TDS"].ToString();
                            ws3.Cell(ws3secondrowlist, 17).Value = row["BSRCode"].ToString();
                            ws3.Cell(ws3secondrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws3.Cell(ws3secondrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3secondrowlist, 20).Value = row["Rate"].ToString();
                            ws3.Cell(ws3secondrowlist, 21).Value = "-NA-";
                            ws3secondrowlist++;
                        }
                        int ws3ITotal = ws3secondrowlist + 1;
                        ws3.Cell(ws3ITotal, 5).Value = "TOTAL (SEC.194I)";
                        ws3.Cell(ws3ITotal, 7).Value = model1.TDS94I.Sum(m => m.AmountPaid);
                        ws3.Cell(ws3ITotal, 9).Value = model1.TDS94I.Sum(m => m.TDS);
                        ws3.Cell(ws3ITotal, 12).Value = model1.TDS94I.Sum(m => m.TDS);
                        ws3.Cell(ws3ITotal, 13).Value = model1.TDS94I.Sum(m => m.TDS);
                        ws3.Cell(ws3ITotal, 16).Value = model1.TDS94I.Sum(m => m.TDS);


                        //var firstCell5 = ws3.Cell(ws3Total + 2, 1);
                        //var lastCell5 = ws3.Cell(ws3ITotal, 21);
                        //var range5 = ws3.Range(firstCell5, lastCell5);
                        //range5.Clear(XLClearOptions.AllFormats);
                        //var table5 = range5.CreateTable();
                        //table5.Theme = XLTableTheme.TableStyleLight12;
                        //table5.ShowAutoFilter = false;

                        int ws3thirdrow = ws3ITotal + 2;
                        ws3.Cell(ws3thirdrow, 1).Value = "Sr No.";
                        ws3.Cell(ws3thirdrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws3.Cell(ws3thirdrow, 3).Value = "Section Under Payment Made";
                        ws3.Cell(ws3thirdrow, 4).Value = "PAN of the Deductee";
                        ws3.Cell(ws3thirdrow, 5).Value = "Name of the deductee";
                        ws3.Cell(ws3thirdrow, 6).Value = "Date of Payment/credit";
                        ws3.Cell(ws3thirdrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws3.Cell(ws3thirdrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws3.Cell(ws3thirdrow, 9).Value = "TDS Rs.";
                        ws3.Cell(ws3thirdrow, 10).Value = "Surcharge Rs.";
                        ws3.Cell(ws3thirdrow, 11).Value = "Education Cess Rs.";
                        ws3.Cell(ws3thirdrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws3.Cell(ws3thirdrow, 13).Value = "Total tax deposited Rs.";
                        ws3.Cell(ws3thirdrow, 14).Value = "Interest";
                        ws3.Cell(ws3thirdrow, 15).Value = "Others";
                        ws3.Cell(ws3thirdrow, 16).Value = "Total (425+Interest+  Others)";
                        ws3.Cell(ws3thirdrow, 17).Value = "BSR Code";
                        ws3.Cell(ws3thirdrow, 18).Value = "Challan No.";
                        ws3.Cell(ws3thirdrow, 19).Value = "Date of Deposit";
                        ws3.Cell(ws3thirdrow, 20).Value = "Rate at which deducted";
                        ws3.Cell(ws3thirdrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws3.Cell(ws3thirdrow + 1, 1).Value = "(414)";
                        ws3.Cell(ws3thirdrow + 1, 2).Value = "(415)";
                        ws3.Cell(ws3thirdrow + 1, 3).Value = "(415A)";
                        ws3.Cell(ws3thirdrow + 1, 4).Value = "(416)";
                        ws3.Cell(ws3thirdrow + 1, 5).Value = "(417)";
                        ws3.Cell(ws3thirdrow + 1, 6).Value = "(418)";
                        ws3.Cell(ws3thirdrow + 1, 7).Value = "(419)";
                        ws3.Cell(ws3thirdrow + 1, 8).Value = "(420)";
                        ws3.Cell(ws3thirdrow + 1, 9).Value = "(421)";
                        ws3.Cell(ws3thirdrow + 1, 10).Value = "(422)";
                        ws3.Cell(ws3thirdrow + 1, 11).Value = "(423)";
                        ws3.Cell(ws3thirdrow + 1, 12).Value = "(424)";
                        ws3.Cell(ws3thirdrow + 1, 13).Value = "(425)";
                        ws3.Cell(ws3thirdrow + 1, 14).Value = "(425A)";
                        ws3.Cell(ws3thirdrow + 1, 15).Value = "(425B)";
                        ws3.Cell(ws3thirdrow + 1, 16).Value = "(425C)";
                        ws3.Cell(ws3thirdrow + 1, 17).Value = "(425D)";
                        ws3.Cell(ws3thirdrow + 1, 18).Value = "(425E)";
                        ws3.Cell(ws3thirdrow + 1, 19).Value = "(426)";
                        ws3.Cell(ws3thirdrow + 1, 20).Value = "(427)";
                        ws3.Cell(ws3thirdrow + 1, 21).Value = "(428)";
                        int ws3thirdrowlist = ws3thirdrow + 2;
                        foreach (DataRow row in dtColumns7.Rows)
                        {
                            ws3.Cell(ws3thirdrowlist, 1).Value = row["SlNo"].ToString();
                            ws3.Cell(ws3thirdrowlist, 2).Value = "";
                            ws3.Cell(ws3thirdrowlist, 3).Value = "94J";
                            ws3.Cell(ws3thirdrowlist, 4).Value = row["PAN"].ToString();
                            ws3.Cell(ws3thirdrowlist, 5).Value = row["Name"].ToString();
                            ws3.Cell(ws3thirdrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3thirdrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws3.Cell(ws3thirdrowlist, 8).Value = "";
                            ws3.Cell(ws3thirdrowlist, 9).Value = row["TDS"].ToString();
                            ws3.Cell(ws3thirdrowlist, 10).Value = "-";
                            ws3.Cell(ws3thirdrowlist, 11).Value = "-";
                            ws3.Cell(ws3thirdrowlist, 12).Value = row["TDS"].ToString();
                            ws3.Cell(ws3thirdrowlist, 13).Value = row["TDS"].ToString();
                            ws3.Cell(ws3thirdrowlist, 14).Value = "-";
                            ws3.Cell(ws3thirdrowlist, 15).Value = "-";
                            ws3.Cell(ws3thirdrowlist, 16).Value = row["TDS"].ToString();
                            ws3.Cell(ws3thirdrowlist, 17).Value = row["BSRCode"].ToString();
                            ws3.Cell(ws3thirdrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws3.Cell(ws3thirdrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3thirdrowlist, 20).Value = row["Rate"].ToString();
                            ws3.Cell(ws3thirdrowlist, 21).Value = "-NA-";
                            ws3thirdrowlist++;
                        }
                        int ws3JTotal = ws3thirdrowlist + 1;
                        ws3.Cell(ws3JTotal, 5).Value = "TOTAL (SEC.194J)";
                        ws3.Cell(ws3JTotal, 7).Value = model1.TDS94J.Sum(m => m.AmountPaid);
                        ws3.Cell(ws3JTotal, 9).Value = model1.TDS94J.Sum(m => m.TDS);
                        ws3.Cell(ws3JTotal, 12).Value = model1.TDS94J.Sum(m => m.TDS);
                        ws3.Cell(ws3JTotal, 13).Value = model1.TDS94J.Sum(m => m.TDS);
                        ws3.Cell(ws3JTotal, 16).Value = model1.TDS94J.Sum(m => m.TDS);

                        //var firstCell6 = ws3.Cell(ws3ITotal + 2, 1);
                        //var lastCell6 = ws3.Cell(ws3JTotal, 21);
                        //var range6 = ws3.Range(firstCell6, lastCell6);
                        //range6.Clear(XLClearOptions.AllFormats);
                        //var table6 = range6.CreateTable();
                        //table6.Theme = XLTableTheme.TableStyleLight12;
                        //table6.ShowAutoFilter = false;

                        int ws3fourthrow = ws3JTotal + 2;
                        ws3.Cell(ws3fourthrow, 1).Value = "Sr No.";
                        ws3.Cell(ws3fourthrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws3.Cell(ws3fourthrow, 3).Value = "Section Under Payment Made";
                        ws3.Cell(ws3fourthrow, 4).Value = "PAN of the Deductee";
                        ws3.Cell(ws3fourthrow, 5).Value = "Name of the deductee";
                        ws3.Cell(ws3fourthrow, 6).Value = "Date of Payment/credit";
                        ws3.Cell(ws3fourthrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws3.Cell(ws3fourthrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws3.Cell(ws3fourthrow, 9).Value = "TDS Rs.";
                        ws3.Cell(ws3fourthrow, 10).Value = "Surcharge Rs.";
                        ws3.Cell(ws3fourthrow, 11).Value = "Education Cess Rs.";
                        ws3.Cell(ws3fourthrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws3.Cell(ws3fourthrow, 13).Value = "Total tax deposited Rs.";
                        ws3.Cell(ws3fourthrow, 14).Value = "Interest";
                        ws3.Cell(ws3fourthrow, 15).Value = "Others";
                        ws3.Cell(ws3fourthrow, 16).Value = "Total (425+Interest+  Others)";
                        ws3.Cell(ws3fourthrow, 17).Value = "BSR Code";
                        ws3.Cell(ws3fourthrow, 18).Value = "Challan No.";
                        ws3.Cell(ws3fourthrow, 19).Value = "Date of Deposit";
                        ws3.Cell(ws3fourthrow, 20).Value = "Rate at which deducted";
                        ws3.Cell(ws3fourthrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws3.Cell(ws3fourthrow + 1, 1).Value = "(414)";
                        ws3.Cell(ws3fourthrow + 1, 2).Value = "(415)";
                        ws3.Cell(ws3fourthrow + 1, 3).Value = "(415A)";
                        ws3.Cell(ws3fourthrow + 1, 4).Value = "(416)";
                        ws3.Cell(ws3fourthrow + 1, 5).Value = "(417)";
                        ws3.Cell(ws3fourthrow + 1, 6).Value = "(418)";
                        ws3.Cell(ws3fourthrow + 1, 7).Value = "(419)";
                        ws3.Cell(ws3fourthrow + 1, 8).Value = "(420)";
                        ws3.Cell(ws3fourthrow + 1, 9).Value = "(421)";
                        ws3.Cell(ws3fourthrow + 1, 10).Value = "(422)";
                        ws3.Cell(ws3fourthrow + 1, 11).Value = "(423)";
                        ws3.Cell(ws3fourthrow + 1, 12).Value = "(424)";
                        ws3.Cell(ws3fourthrow + 1, 13).Value = "(425)";
                        ws3.Cell(ws3fourthrow + 1, 14).Value = "(425A)";
                        ws3.Cell(ws3fourthrow + 1, 15).Value = "(425B)";
                        ws3.Cell(ws3fourthrow + 1, 16).Value = "(425C)";
                        ws3.Cell(ws3fourthrow + 1, 17).Value = "(425D)";
                        ws3.Cell(ws3fourthrow + 1, 18).Value = "(425E)";
                        ws3.Cell(ws3fourthrow + 1, 19).Value = "(426)";
                        ws3.Cell(ws3fourthrow + 1, 20).Value = "(427)";
                        ws3.Cell(ws3fourthrow + 1, 21).Value = "(428)";
                        int ws3fourthrowlist = ws3fourthrow + 2;
                        foreach (DataRow row in dtColumns8.Rows)
                        {
                            ws3.Cell(ws3fourthrowlist, 1).Value = row["SlNo"].ToString();
                            ws3.Cell(ws3fourthrowlist, 2).Value = "";
                            ws3.Cell(ws3fourthrowlist, 3).Value = "94H";
                            ws3.Cell(ws3fourthrowlist, 5).Value = row["Name"].ToString();
                            ws3.Cell(ws3fourthrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3fourthrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws3.Cell(ws3fourthrowlist, 8).Value = "";
                            ws3.Cell(ws3fourthrowlist, 9).Value = row["TDS"].ToString();
                            ws3.Cell(ws3fourthrowlist, 10).Value = "-";
                            ws3.Cell(ws3fourthrowlist, 11).Value = "-";
                            ws3.Cell(ws3fourthrowlist, 12).Value = row["TDS"].ToString();
                            ws3.Cell(ws3fourthrowlist, 13).Value = row["TDS"].ToString();
                            ws3.Cell(ws3fourthrowlist, 14).Value = "-";
                            ws3.Cell(ws3fourthrowlist, 15).Value = "-";
                            ws3.Cell(ws3fourthrowlist, 16).Value = row["TDS"].ToString();
                            ws3.Cell(ws3fourthrowlist, 17).Value = row["BSRCode"].ToString();
                            ws3.Cell(ws3fourthrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws3.Cell(ws3fourthrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3fourthrowlist, 20).Value = row["Rate"].ToString();
                            ws3.Cell(ws3fourthrowlist, 21).Value = "-NA-";
                            ws3fourthrowlist++;
                        }
                        int ws3HTotal = ws3fourthrowlist + 1;
                        ws3.Cell(ws3HTotal, 5).Value = "TOTAL (SEC.194H)";
                        ws3.Cell(ws3HTotal, 7).Value = model1.TDS94H.Sum(m => m.AmountPaid);
                        ws3.Cell(ws3HTotal, 9).Value = model1.TDS94H.Sum(m => m.TDS);
                        ws3.Cell(ws3HTotal, 12).Value = model1.TDS94H.Sum(m => m.TDS);
                        ws3.Cell(ws3HTotal, 13).Value = model1.TDS94H.Sum(m => m.TDS);
                        ws3.Cell(ws3HTotal, 16).Value = model1.TDS94H.Sum(m => m.TDS);

                        //var firstCell7 = ws3.Cell(ws3JTotal + 2, 1);
                        //var lastCell7 = ws3.Cell(ws3HTotal, 21);
                        //var range7 = ws3.Range(firstCell7, lastCell7);
                        //range7.Clear(XLClearOptions.AllFormats);
                        //var table7 = range7.CreateTable();
                        //table7.Theme = XLTableTheme.TableStyleLight12;
                        //table7.ShowAutoFilter = false;

                        //////////////////////

                        /////////  third month sheet  ////////
                        Form24QModel model2 = new Form24QModel();
                        model2 = ReportService.GetForm26Q(Date3);
                        DataTable dtColumns9 = new DataTable();
                        DataTable dtColumns10 = new DataTable();
                        DataTable dtColumns11 = new DataTable();
                        DataTable dtColumns12 = new DataTable();
                        string json9 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94C);
                        dtColumns9 = JsonConvert.DeserializeObject<DataTable>(json9);
                        string json10 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94I);
                        dtColumns10 = JsonConvert.DeserializeObject<DataTable>(json10);
                        string json11 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94J);
                        dtColumns11 = JsonConvert.DeserializeObject<DataTable>(json11);
                        string json12 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94H);
                        dtColumns12 = JsonConvert.DeserializeObject<DataTable>(json12);
                        var ws4 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date3));
                        ws4.Cell(1, 1).Value = "Sr No.";
                        ws4.Cell(1, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws4.Cell(1, 3).Value = "Section Under Payment Made";
                        ws4.Cell(1, 4).Value = "PAN of the Deductee";
                        ws4.Cell(1, 5).Value = "Name of the deductee";
                        ws4.Cell(1, 6).Value = "Date of Payment/credit";
                        ws4.Cell(1, 7).Value = "Amount Paid/Credited Rs.";
                        ws4.Cell(1, 8).Value = "Paid by Book Entry or otherwise";
                        ws4.Cell(1, 9).Value = "TDS Rs.";
                        ws4.Cell(1, 10).Value = "Surcharge Rs.";
                        ws4.Cell(1, 11).Value = "Education Cess Rs.";
                        ws4.Cell(1, 12).Value = "Total tax deducted (421+422+423)";
                        ws4.Cell(1, 13).Value = "Total tax deposited Rs.";
                        ws4.Cell(1, 14).Value = "Interest";
                        ws4.Cell(1, 15).Value = "Others";
                        ws4.Cell(1, 16).Value = "Total (425+Interest+  Others)";
                        ws4.Cell(1, 17).Value = "BSR Code";
                        ws4.Cell(1, 18).Value = "Challan No.";
                        ws4.Cell(1, 19).Value = "Date of Deposit";
                        ws4.Cell(1, 20).Value = "Rate at which deducted";
                        ws4.Cell(1, 21).Value = "Reason for non-deduction/lower deduction";
                        ws4.Cell(2, 1).Value = "(414)";
                        ws4.Cell(2, 2).Value = "(415)";
                        ws4.Cell(2, 3).Value = "(415A)";
                        ws4.Cell(2, 4).Value = "(416)";
                        ws4.Cell(2, 5).Value = "(417)";
                        ws4.Cell(2, 6).Value = "(418)";
                        ws4.Cell(2, 7).Value = "(419)";
                        ws4.Cell(2, 8).Value = "(420)";
                        ws4.Cell(2, 9).Value = "(421)";
                        ws4.Cell(2, 10).Value = "(422)";
                        ws4.Cell(2, 11).Value = "(423)";
                        ws4.Cell(2, 12).Value = "(424)";
                        ws4.Cell(2, 13).Value = "(425)";
                        ws4.Cell(2, 14).Value = "(425A)";
                        ws4.Cell(2, 15).Value = "(425B)";
                        ws4.Cell(2, 16).Value = "(425C)";
                        ws4.Cell(2, 17).Value = "(425D)";
                        ws4.Cell(2, 18).Value = "(425E)";
                        ws4.Cell(2, 19).Value = "(426)";
                        ws4.Cell(2, 20).Value = "(427)";
                        ws4.Cell(2, 21).Value = "(428)";
                        int ws4firstrow = 3; int ws4firstcol = 1;
                        foreach (DataRow row in dtColumns9.Rows)
                        {
                            ws4.Cell(ws4firstrow, 1).Value = row["SlNo"].ToString();
                            ws4.Cell(ws4firstrow, 2).Value = "";
                            ws4.Cell(ws4firstrow, 3).Value = "94C";
                            ws4.Cell(ws4firstrow, 4).Value = row["PAN"].ToString();
                            ws4.Cell(ws4firstrow, 5).Value = row["Name"].ToString();
                            ws4.Cell(ws4firstrow, 6).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4firstrow, 7).Value = row["AmountPaid"].ToString();
                            ws4.Cell(ws4firstrow, 8).Value = "";
                            ws4.Cell(ws4firstrow, 9).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 10).Value = "-";
                            ws4.Cell(ws4firstrow, 11).Value = "-";
                            ws4.Cell(ws4firstrow, 12).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 13).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 14).Value = "-";
                            ws4.Cell(ws4firstrow, 15).Value = "-";
                            ws4.Cell(ws4firstrow, 16).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 17).Value = row["BSRCode"].ToString();
                            ws4.Cell(ws4firstrow, 18).Value = row["ChallenNo"].ToString();
                            ws4.Cell(ws4firstrow, 19).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4firstrow, 20).Value = row["Rate"].ToString();
                            ws4.Cell(ws4firstrow, 21).Value = "-NA-";
                            ws4firstrow++;
                        }
                        int ws4Total = ws4firstrow + 1;
                        ws4.Cell(ws4Total, 5).Value = "TOTAL (SEC.194C)";
                        ws4.Cell(ws4Total, 7).Value = model2.TDS94C.Sum(m => m.AmountPaid);
                        ws4.Cell(ws4Total, 9).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws4.Cell(ws4Total, 12).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws4.Cell(ws4Total, 13).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws4.Cell(ws4Total, 16).Value = model2.TDS94C.Sum(m => m.TDS);

                        //var firstCell8 = ws4.FirstCellUsed();
                        //var lastCell8 = ws4.LastCellUsed();
                        //var range8 = ws4.Range(firstCell8, lastCell8);
                        //range8.Clear(XLClearOptions.AllFormats);
                        //var table8 = range8.CreateTable();
                        //table8.Theme = XLTableTheme.TableStyleLight12;
                        //table8.ShowAutoFilter = false;

                        int ws4secondrow = ws4Total + 2;
                        ws4.Cell(ws4secondrow, 1).Value = "Sr No.";
                        ws4.Cell(ws4secondrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws4.Cell(ws4secondrow, 3).Value = "Section Under Payment Made";
                        ws4.Cell(ws4secondrow, 4).Value = "PAN of the Deductee";
                        ws4.Cell(ws4secondrow, 5).Value = "Name of the deductee";
                        ws4.Cell(ws4secondrow, 6).Value = "Date of Payment/credit";
                        ws4.Cell(ws4secondrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws4.Cell(ws4secondrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws4.Cell(ws4secondrow, 9).Value = "TDS Rs.";
                        ws4.Cell(ws4secondrow, 10).Value = "Surcharge Rs.";
                        ws4.Cell(ws4secondrow, 11).Value = "Education Cess Rs.";
                        ws4.Cell(ws4secondrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws4.Cell(ws4secondrow, 13).Value = "Total tax deposited Rs.";
                        ws4.Cell(ws4secondrow, 14).Value = "Interest";
                        ws4.Cell(ws4secondrow, 15).Value = "Others";
                        ws4.Cell(ws4secondrow, 16).Value = "Total (425+Interest+  Others)";
                        ws4.Cell(ws4secondrow, 17).Value = "BSR Code";
                        ws4.Cell(ws4secondrow, 18).Value = "Challan No.";
                        ws4.Cell(ws4secondrow, 19).Value = "Date of Deposit";
                        ws4.Cell(ws4secondrow, 20).Value = "Rate at which deducted";
                        ws4.Cell(ws4secondrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws4.Cell(ws4secondrow + 1, 1).Value = "(414)";
                        ws4.Cell(ws4secondrow + 1, 2).Value = "(415)";
                        ws4.Cell(ws4secondrow + 1, 3).Value = "(415A)";
                        ws4.Cell(ws4secondrow + 1, 4).Value = "(416)";
                        ws4.Cell(ws4secondrow + 1, 5).Value = "(417)";
                        ws4.Cell(ws4secondrow + 1, 6).Value = "(418)";
                        ws4.Cell(ws4secondrow + 1, 7).Value = "(419)";
                        ws4.Cell(ws4secondrow + 1, 8).Value = "(420)";
                        ws4.Cell(ws4secondrow + 1, 9).Value = "(421)";
                        ws4.Cell(ws4secondrow + 1, 10).Value = "(422)";
                        ws4.Cell(ws4secondrow + 1, 11).Value = "(423)";
                        ws4.Cell(ws4secondrow + 1, 12).Value = "(424)";
                        ws4.Cell(ws4secondrow + 1, 13).Value = "(425)";
                        ws4.Cell(ws4secondrow + 1, 14).Value = "(425A)";
                        ws4.Cell(ws4secondrow + 1, 15).Value = "(425B)";
                        ws4.Cell(ws4secondrow + 1, 16).Value = "(425C)";
                        ws4.Cell(ws4secondrow + 1, 17).Value = "(425D)";
                        ws4.Cell(ws4secondrow + 1, 18).Value = "(425E)";
                        ws4.Cell(ws4secondrow + 1, 19).Value = "(426)";
                        ws4.Cell(ws4secondrow + 1, 20).Value = "(427)";
                        ws4.Cell(ws4secondrow + 1, 21).Value = "(428)";
                        int ws4secondrowlist = ws4secondrow + 2;
                        foreach (DataRow row in dtColumns10.Rows)
                        {
                            ws4.Cell(ws4secondrowlist, 1).Value = row["SlNo"].ToString();
                            ws4.Cell(ws4secondrowlist, 2).Value = "";
                            ws4.Cell(ws4secondrowlist, 3).Value = "94I";
                            ws4.Cell(ws4secondrowlist, 4).Value = row["PAN"].ToString();
                            ws4.Cell(ws4secondrowlist, 5).Value = row["Name"].ToString();
                            ws4.Cell(ws4secondrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4secondrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws4.Cell(ws4secondrowlist, 8).Value = "";
                            ws4.Cell(ws4secondrowlist, 9).Value = row["TDS"].ToString();
                            ws4.Cell(ws4secondrowlist, 10).Value = "-";
                            ws4.Cell(ws4secondrowlist, 11).Value = "-";
                            ws4.Cell(ws4secondrowlist, 12).Value = row["TDS"].ToString();
                            ws4.Cell(ws4secondrowlist, 13).Value = row["TDS"].ToString();
                            ws4.Cell(ws4secondrowlist, 14).Value = "-";
                            ws4.Cell(ws4secondrowlist, 15).Value = "-";
                            ws4.Cell(ws4secondrowlist, 16).Value = row["TDS"].ToString();
                            ws4.Cell(ws4secondrowlist, 17).Value = row["BSRCode"].ToString();
                            ws4.Cell(ws4secondrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws4.Cell(ws4secondrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4secondrowlist, 20).Value = row["Rate"].ToString();
                            ws4.Cell(ws4secondrowlist, 21).Value = "-NA-";
                            ws4secondrowlist++;
                        }
                        int ws4ITotal = ws4secondrowlist + 1;
                        ws4.Cell(ws4ITotal, 5).Value = "TOTAL (SEC.194I)";
                        ws4.Cell(ws4ITotal, 7).Value = model2.TDS94I.Sum(m => m.AmountPaid);
                        ws4.Cell(ws4ITotal, 9).Value = model2.TDS94I.Sum(m => m.TDS);
                        ws4.Cell(ws4ITotal, 12).Value = model2.TDS94I.Sum(m => m.TDS);
                        ws4.Cell(ws4ITotal, 13).Value = model2.TDS94I.Sum(m => m.TDS);
                        ws4.Cell(ws4ITotal, 16).Value = model2.TDS94I.Sum(m => m.TDS);

                        //var firstCell9 = ws4.Cell(ws4Total + 2, 1);
                        //var lastCell9 = ws4.Cell(ws4ITotal, 21);
                        //var range9 = ws4.Range(firstCell9, lastCell9);
                        //range9.Clear(XLClearOptions.AllFormats);
                        //var table9 = range9.CreateTable();
                        //table9.Theme = XLTableTheme.TableStyleLight12;
                        //table9.ShowAutoFilter = false;

                        int ws4thirdrow = ws4ITotal + 2;
                        ws4.Cell(ws4thirdrow, 1).Value = "Sr No.";
                        ws4.Cell(ws4thirdrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws4.Cell(ws4thirdrow, 3).Value = "Section Under Payment Made";
                        ws4.Cell(ws4thirdrow, 4).Value = "PAN of the Deductee";
                        ws4.Cell(ws4thirdrow, 5).Value = "Name of the deductee";
                        ws4.Cell(ws4thirdrow, 6).Value = "Date of Payment/credit";
                        ws4.Cell(ws4thirdrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws4.Cell(ws4thirdrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws4.Cell(ws4thirdrow, 9).Value = "TDS Rs.";
                        ws4.Cell(ws4thirdrow, 10).Value = "Surcharge Rs.";
                        ws4.Cell(ws4thirdrow, 11).Value = "Education Cess Rs.";
                        ws4.Cell(ws4thirdrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws4.Cell(ws4thirdrow, 13).Value = "Total tax deposited Rs.";
                        ws4.Cell(ws4thirdrow, 14).Value = "Interest";
                        ws4.Cell(ws4thirdrow, 15).Value = "Others";
                        ws4.Cell(ws4thirdrow, 16).Value = "Total (425+Interest+  Others)";
                        ws4.Cell(ws4thirdrow, 17).Value = "BSR Code";
                        ws4.Cell(ws4thirdrow, 18).Value = "Challan No.";
                        ws4.Cell(ws4thirdrow, 19).Value = "Date of Deposit";
                        ws4.Cell(ws4thirdrow, 20).Value = "Rate at which deducted";
                        ws4.Cell(ws4thirdrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws4.Cell(ws4thirdrow + 1, 1).Value = "(414)";
                        ws4.Cell(ws4thirdrow + 1, 2).Value = "(415)";
                        ws4.Cell(ws4thirdrow + 1, 3).Value = "(415A)";
                        ws4.Cell(ws4thirdrow + 1, 4).Value = "(416)";
                        ws4.Cell(ws4thirdrow + 1, 5).Value = "(417)";
                        ws4.Cell(ws4thirdrow + 1, 6).Value = "(418)";
                        ws4.Cell(ws4thirdrow + 1, 7).Value = "(419)";
                        ws4.Cell(ws4thirdrow + 1, 8).Value = "(420)";
                        ws4.Cell(ws4thirdrow + 1, 9).Value = "(421)";
                        ws4.Cell(ws4thirdrow + 1, 10).Value = "(422)";
                        ws4.Cell(ws4thirdrow + 1, 11).Value = "(423)";
                        ws4.Cell(ws4thirdrow + 1, 12).Value = "(424)";
                        ws4.Cell(ws4thirdrow + 1, 13).Value = "(425)";
                        ws4.Cell(ws4thirdrow + 1, 14).Value = "(425A)";
                        ws4.Cell(ws4thirdrow + 1, 15).Value = "(425B)";
                        ws4.Cell(ws4thirdrow + 1, 16).Value = "(425C)";
                        ws4.Cell(ws4thirdrow + 1, 17).Value = "(425D)";
                        ws4.Cell(ws4thirdrow + 1, 18).Value = "(425E)";
                        ws4.Cell(ws4thirdrow + 1, 19).Value = "(426)";
                        ws4.Cell(ws4thirdrow + 1, 20).Value = "(427)";
                        ws4.Cell(ws4thirdrow + 1, 21).Value = "(428)";
                        int ws4thirdrowlist = ws4thirdrow + 2;
                        foreach (DataRow row in dtColumns11.Rows)
                        {
                            ws4.Cell(ws4thirdrowlist, 1).Value = row["SlNo"].ToString();
                            ws4.Cell(ws4thirdrowlist, 2).Value = "";
                            ws4.Cell(ws4thirdrowlist, 3).Value = "94J";
                            ws4.Cell(ws4thirdrowlist, 4).Value = row["PAN"].ToString();
                            ws4.Cell(ws4thirdrowlist, 5).Value = row["Name"].ToString();
                            ws4.Cell(ws4thirdrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4thirdrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws4.Cell(ws4thirdrowlist, 8).Value = "";
                            ws4.Cell(ws4thirdrowlist, 9).Value = row["TDS"].ToString();
                            ws4.Cell(ws4thirdrowlist, 10).Value = "-";
                            ws4.Cell(ws4thirdrowlist, 11).Value = "-";
                            ws4.Cell(ws4thirdrowlist, 12).Value = row["TDS"].ToString();
                            ws4.Cell(ws4thirdrowlist, 13).Value = row["TDS"].ToString();
                            ws4.Cell(ws4thirdrowlist, 14).Value = "-";
                            ws4.Cell(ws4thirdrowlist, 15).Value = "-";
                            ws4.Cell(ws4thirdrowlist, 16).Value = row["TDS"].ToString();
                            ws4.Cell(ws4thirdrowlist, 17).Value = row["BSRCode"].ToString();
                            ws4.Cell(ws4thirdrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws4.Cell(ws4thirdrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4thirdrowlist, 20).Value = row["Rate"].ToString();
                            ws4.Cell(ws4thirdrowlist, 21).Value = "-NA-";
                            ws4thirdrowlist++;
                        }
                        int ws4JTotal = ws4thirdrowlist + 1;
                        ws4.Cell(ws4JTotal, 5).Value = "TOTAL (SEC.194J)";
                        ws4.Cell(ws4JTotal, 7).Value = model2.TDS94J.Sum(m => m.AmountPaid);
                        ws4.Cell(ws4JTotal, 9).Value = model2.TDS94J.Sum(m => m.TDS);
                        ws4.Cell(ws4JTotal, 12).Value = model2.TDS94J.Sum(m => m.TDS);
                        ws4.Cell(ws4JTotal, 13).Value = model2.TDS94J.Sum(m => m.TDS);
                        ws4.Cell(ws4JTotal, 16).Value = model2.TDS94J.Sum(m => m.TDS);

                        //var firstCell10 = ws4.Cell(ws4ITotal + 2, 1);
                        //var lastCell10 = ws4.Cell(ws4JTotal, 21);
                        //var range10 = ws4.Range(firstCell10, lastCell10);
                        //range10.Clear(XLClearOptions.AllFormats);
                        //var table10 = range10.CreateTable();
                        //table10.Theme = XLTableTheme.TableStyleLight12;
                        //table10.ShowAutoFilter = false;

                        int ws4fourthrow = ws4JTotal + 2;
                        ws4.Cell(ws4fourthrow, 1).Value = "Sr No.";
                        ws4.Cell(ws4fourthrow, 2).Value = "Deductee Code (01-Company 02-Others)";
                        ws4.Cell(ws4fourthrow, 3).Value = "Section Under Payment Made";
                        ws4.Cell(ws4fourthrow, 4).Value = "PAN of the Deductee";
                        ws4.Cell(ws4fourthrow, 5).Value = "Name of the deductee";
                        ws4.Cell(ws4fourthrow, 6).Value = "Date of Payment/credit";
                        ws4.Cell(ws4fourthrow, 7).Value = "Amount Paid/Credited Rs.";
                        ws4.Cell(ws4fourthrow, 8).Value = "Paid by Book Entry or otherwise";
                        ws4.Cell(ws4fourthrow, 9).Value = "TDS Rs.";
                        ws4.Cell(ws4fourthrow, 10).Value = "Surcharge Rs.";
                        ws4.Cell(ws4fourthrow, 11).Value = "Education Cess Rs.";
                        ws4.Cell(ws4fourthrow, 12).Value = "Total tax deducted (421+422+423)";
                        ws4.Cell(ws4fourthrow, 13).Value = "Total tax deposited Rs.";
                        ws4.Cell(ws4fourthrow, 14).Value = "Interest";
                        ws4.Cell(ws4fourthrow, 15).Value = "Others";
                        ws4.Cell(ws4fourthrow, 16).Value = "Total (425+Interest+  Others)";
                        ws4.Cell(ws4fourthrow, 17).Value = "BSR Code";
                        ws4.Cell(ws4fourthrow, 18).Value = "Challan No.";
                        ws4.Cell(ws4fourthrow, 19).Value = "Date of Deposit";
                        ws4.Cell(ws4fourthrow, 20).Value = "Rate at which deducted";
                        ws4.Cell(ws4fourthrow, 21).Value = "Reason for non-deduction/lower deduction";
                        ws4.Cell(ws4fourthrow + 1, 1).Value = "(414)";
                        ws4.Cell(ws4fourthrow + 1, 2).Value = "(415)";
                        ws4.Cell(ws4fourthrow + 1, 3).Value = "(415A)";
                        ws4.Cell(ws4fourthrow + 1, 4).Value = "(416)";
                        ws4.Cell(ws4fourthrow + 1, 5).Value = "(417)";
                        ws4.Cell(ws4fourthrow + 1, 6).Value = "(418)";
                        ws4.Cell(ws4fourthrow + 1, 7).Value = "(419)";
                        ws4.Cell(ws4fourthrow + 1, 8).Value = "(420)";
                        ws4.Cell(ws4fourthrow + 1, 9).Value = "(421)";
                        ws4.Cell(ws4fourthrow + 1, 10).Value = "(422)";
                        ws4.Cell(ws4fourthrow + 1, 11).Value = "(423)";
                        ws4.Cell(ws4fourthrow + 1, 12).Value = "(424)";
                        ws4.Cell(ws4fourthrow + 1, 13).Value = "(425)";
                        ws4.Cell(ws4fourthrow + 1, 14).Value = "(425A)";
                        ws4.Cell(ws4fourthrow + 1, 15).Value = "(425B)";
                        ws4.Cell(ws4fourthrow + 1, 16).Value = "(425C)";
                        ws4.Cell(ws4fourthrow + 1, 17).Value = "(425D)";
                        ws4.Cell(ws4fourthrow + 1, 18).Value = "(425E)";
                        ws4.Cell(ws4fourthrow + 1, 19).Value = "(426)";
                        ws4.Cell(ws4fourthrow + 1, 20).Value = "(427)";
                        ws4.Cell(ws4fourthrow + 1, 21).Value = "(428)";
                        int ws4fourthrowlist = ws4fourthrow + 2;
                        foreach (DataRow row in dtColumns12.Rows)
                        {
                            ws4.Cell(ws4fourthrowlist, 1).Value = row["SlNo"].ToString();
                            ws4.Cell(ws4fourthrowlist, 2).Value = "";
                            ws4.Cell(ws4fourthrowlist, 3).Value = "94H";
                            ws4.Cell(ws4fourthrowlist, 5).Value = row["Name"].ToString();
                            ws4.Cell(ws4fourthrowlist, 6).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4fourthrowlist, 7).Value = row["AmountPaid"].ToString();
                            ws4.Cell(ws4fourthrowlist, 8).Value = "";
                            ws4.Cell(ws4fourthrowlist, 9).Value = row["TDS"].ToString();
                            ws4.Cell(ws4fourthrowlist, 10).Value = "-";
                            ws4.Cell(ws4fourthrowlist, 11).Value = "-";
                            ws4.Cell(ws4fourthrowlist, 12).Value = row["TDS"].ToString();
                            ws4.Cell(ws4fourthrowlist, 13).Value = row["TDS"].ToString();
                            ws4.Cell(ws4fourthrowlist, 14).Value = "-";
                            ws4.Cell(ws4fourthrowlist, 15).Value = "-";
                            ws4.Cell(ws4fourthrowlist, 16).Value = row["TDS"].ToString();
                            ws4.Cell(ws4fourthrowlist, 17).Value = row["BSRCode"].ToString();
                            ws4.Cell(ws4fourthrowlist, 18).Value = row["ChallenNo"].ToString();
                            ws4.Cell(ws4fourthrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4fourthrowlist, 20).Value = row["Rate"].ToString();
                            ws4.Cell(ws4fourthrowlist, 21).Value = "-NA-";
                            ws4fourthrowlist++;
                        }
                        int ws4HTotal = ws4fourthrowlist + 1;
                        ws4.Cell(ws4HTotal, 5).Value = "TOTAL (SEC.194H)";
                        ws4.Cell(ws4HTotal, 7).Value = model2.TDS94H.Sum(m => m.AmountPaid);
                        ws4.Cell(ws4HTotal, 9).Value = model2.TDS94H.Sum(m => m.TDS);
                        ws4.Cell(ws4HTotal, 12).Value = model2.TDS94H.Sum(m => m.TDS);
                        ws4.Cell(ws4HTotal, 13).Value = model2.TDS94H.Sum(m => m.TDS);
                        ws4.Cell(ws4HTotal, 16).Value = model2.TDS94H.Sum(m => m.TDS);

                        //var firstCell11 = ws4.Cell(ws4JTotal + 2, 1);
                        //var lastCell11 = ws4.Cell(ws4HTotal, 21);
                        //var range11 = ws4.Range(firstCell11, lastCell11);
                        //range11.Clear(XLClearOptions.AllFormats);
                        //var table11 = range11.CreateTable();
                        //table11.Theme = XLTableTheme.TableStyleLight12;
                        //table11.ShowAutoFilter = false;

                        //////////////////////

                        //////Sheet 2 ///////

                        ws1.Cell("D1").Value = "DETAILS OF TDS DEDUCTED FROM  " + String.Format("{0:MMMM}", Date1) + " " + startDate.Year + " TO " + String.Format("{0:MMMM}", Date3) + " " + endDate.Year + " - QTR " + Common.ToRoman(Quator);
                        ws1.Cell("D1").Style.Font.Bold = true;
                        ws1.Range("D1:F1").Row(1).Merge();
                        ws1.Cell(3, 1).Value = "SI.NO";
                        ws1.Cell(3, 2).Value = "SECTION CODE";
                        ws1.Cell(3, 3).Value = "TDS Rs.";
                        ws1.Cell(3, 4).Value = "SURCHARGE Rs.";
                        ws1.Cell(3, 5).Value = "EDUCATION CESS Rs.";
                        ws1.Cell(3, 6).Value = "INTEREST Rs.";
                        ws1.Cell(3, 7).Value = "OTHERS Rs.";
                        ws1.Cell(3, 8).Value = "TOTAL TAX DEPOSITED Rs.(403+404+405+406+407)";
                        ws1.Cell(3, 9).Value = "CHEQUE/DD NO";
                        ws1.Cell(3, 10).Value = "BSR CODE";
                        ws1.Cell(3, 11).Value = "DATE ON WHICH TAX DEPOSITED";
                        ws1.Cell(3, 12).Value = "TRANSFER VOUCHER/ CHALLAN SERIAL NO";
                        ws1.Cell(3, 13).Value = "WHETHER TDS DEPOSITED BY BOOK ENTRY? YES/NO";

                        ws1.Cell(4, 1).Value = "(401)";
                        ws1.Cell(4, 2).Value = "(402)";
                        ws1.Cell(4, 3).Value = "(403)";
                        ws1.Cell(4, 4).Value = "(404)";
                        ws1.Cell(4, 5).Value = "(405)";
                        ws1.Cell(4, 6).Value = "(406)";
                        ws1.Cell(4, 7).Value = "(407)";
                        ws1.Cell(4, 8).Value = "(408)";
                        ws1.Cell(4, 9).Value = "(409)";
                        ws1.Cell(4, 10).Value = "(410)";
                        ws1.Cell(4, 11).Value = "(411)";
                        ws1.Cell(4, 12).Value = "(412)";
                        ws1.Cell(4, 13).Value = "(413)";

                        ws1.Cell(5, 1).Value = "1";
                        ws1.Cell(5, 2).Value = "94C";
                        ws1.Cell(5, 3).Value = model.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(5, 4).Value = "-";
                        ws1.Cell(5, 5).Value = "-";
                        ws1.Cell(5, 6).Value = "-";
                        ws1.Cell(5, 7).Value = "-";
                        ws1.Cell(5, 8).Value = model.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(5, 9).Value = model.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(5, 10).Value = model.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(5, 11).Value = model.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(5, 12).Value = model.TDS94C.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(5, 13).Value = "NO";



                        ws1.Cell(6, 1).Value = "2";
                        ws1.Cell(6, 2).Value = "94I";
                        ws1.Cell(6, 3).Value = model.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(6, 4).Value = "-";
                        ws1.Cell(6, 5).Value = "-";
                        ws1.Cell(6, 6).Value = "-";
                        ws1.Cell(6, 7).Value = "-";
                        ws1.Cell(6, 8).Value = model.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(6, 9).Value = model.TDS94I.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(6, 10).Value = model.TDS94I.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(6, 11).Value = model.TDS94I.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(6, 12).Value = model.TDS94I.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(6, 13).Value = "NO";

                        ws1.Cell(7, 1).Value = "3";
                        ws1.Cell(7, 2).Value = "94J";
                        ws1.Cell(7, 3).Value = model.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(7, 4).Value = "-";
                        ws1.Cell(7, 5).Value = "-";
                        ws1.Cell(7, 6).Value = "-";
                        ws1.Cell(7, 7).Value = "-";
                        ws1.Cell(7, 8).Value = model.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(7, 9).Value = model.TDS94J.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(7, 10).Value = model.TDS94J.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(7, 11).Value = model.TDS94J.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(7, 12).Value = model.TDS94J.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(7, 13).Value = "NO";

                        ws1.Cell(8, 1).Value = "4";
                        ws1.Cell(8, 2).Value = "94H";
                        ws1.Cell(8, 3).Value = model.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(8, 4).Value = "-";
                        ws1.Cell(8, 5).Value = "-";
                        ws1.Cell(8, 6).Value = "-";
                        ws1.Cell(8, 7).Value = "-";
                        ws1.Cell(8, 8).Value = model.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(8, 9).Value = model.TDS94H.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(8, 10).Value = model.TDS94H.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(8, 11).Value = model.TDS94H.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(8, 12).Value = model.TDS94H.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(8, 13).Value = "NO";
                        //////
                        ws1.Cell(9, 1).Value = "5";
                        ws1.Cell(9, 2).Value = "94C";
                        ws1.Cell(9, 3).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(9, 4).Value = "-";
                        ws1.Cell(9, 5).Value = "-";
                        ws1.Cell(9, 6).Value = "-";
                        ws1.Cell(9, 7).Value = "-";
                        ws1.Cell(9, 8).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(9, 9).Value = model1.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(9, 10).Value = model1.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(9, 11).Value = model1.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(9, 12).Value = model1.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(9, 13).Value = "NO";

                        ws1.Cell(10, 1).Value = "6";
                        ws1.Cell(10, 2).Value = "94I";
                        ws1.Cell(10, 3).Value = model1.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(10, 4).Value = "-";
                        ws1.Cell(10, 5).Value = "-";
                        ws1.Cell(10, 6).Value = "-";
                        ws1.Cell(10, 7).Value = "-";
                        ws1.Cell(10, 8).Value = model1.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(10, 9).Value = model1.TDS94I.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(10, 10).Value = model1.TDS94I.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(10, 11).Value = model1.TDS94I.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(10, 12).Value = model1.TDS94I.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(10, 13).Value = "NO";

                        ws1.Cell(11, 1).Value = "7";
                        ws1.Cell(11, 2).Value = "94J";
                        ws1.Cell(11, 3).Value = model1.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(11, 4).Value = "-";
                        ws1.Cell(11, 5).Value = "-";
                        ws1.Cell(11, 6).Value = "-";
                        ws1.Cell(11, 7).Value = "-";
                        ws1.Cell(11, 8).Value = model1.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(11, 9).Value = model1.TDS94J.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(11, 10).Value = model1.TDS94J.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(11, 11).Value = model1.TDS94J.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(11, 12).Value = model1.TDS94J.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(11, 13).Value = "NO";

                        ws1.Cell(12, 1).Value = "8";
                        ws1.Cell(12, 2).Value = "94H";
                        ws1.Cell(12, 3).Value = model1.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(12, 4).Value = "-";
                        ws1.Cell(12, 5).Value = "-";
                        ws1.Cell(12, 6).Value = "-";
                        ws1.Cell(12, 7).Value = "-";
                        ws1.Cell(12, 8).Value = model1.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(12, 9).Value = model1.TDS94H.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(12, 10).Value = model1.TDS94H.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(12, 11).Value = model1.TDS94H.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(12, 12).Value = model1.TDS94H.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(12, 13).Value = "NO";

                        /////
                        ws1.Cell(13, 1).Value = "9";
                        ws1.Cell(13, 2).Value = "94C";
                        ws1.Cell(13, 3).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(13, 4).Value = "-";
                        ws1.Cell(13, 5).Value = "-";
                        ws1.Cell(13, 6).Value = "-";
                        ws1.Cell(13, 7).Value = "-";
                        ws1.Cell(13, 8).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(13, 9).Value = model2.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(13, 10).Value = model2.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(13, 11).Value = model2.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(13, 12).Value = model2.TDS94C.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(13, 13).Value = "NO";

                        ws1.Cell(14, 1).Value = "10";
                        ws1.Cell(14, 2).Value = "94I";
                        ws1.Cell(14, 3).Value = model2.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(14, 4).Value = "-";
                        ws1.Cell(14, 5).Value = "-";
                        ws1.Cell(14, 6).Value = "-";
                        ws1.Cell(14, 7).Value = "-";
                        ws1.Cell(14, 8).Value = model2.TDS94I.Sum(m => m.TDS);
                        ws1.Cell(14, 9).Value = model2.TDS94I.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(14, 10).Value = model2.TDS94I.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(14, 11).Value = model2.TDS94I.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(14, 12).Value = model2.TDS94I.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(14, 13).Value = "NO";

                        ws1.Cell(15, 1).Value = "11";
                        ws1.Cell(15, 2).Value = "94J";
                        ws1.Cell(15, 3).Value = model2.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(15, 4).Value = "-";
                        ws1.Cell(15, 5).Value = "-";
                        ws1.Cell(15, 6).Value = "-";
                        ws1.Cell(15, 7).Value = "-";
                        ws1.Cell(15, 8).Value = model2.TDS94J.Sum(m => m.TDS);
                        ws1.Cell(15, 9).Value = model2.TDS94J.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(15, 10).Value = model2.TDS94J.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(15, 11).Value = model2.TDS94J.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(15, 12).Value = model2.TDS94J.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(15, 13).Value = "NO";

                        ws1.Cell(16, 1).Value = "12";
                        ws1.Cell(16, 2).Value = "94H";
                        ws1.Cell(16, 3).Value = model2.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(16, 4).Value = "-";
                        ws1.Cell(16, 5).Value = "-";
                        ws1.Cell(16, 6).Value = "-";
                        ws1.Cell(16, 7).Value = "-";
                        ws1.Cell(16, 8).Value = model2.TDS94H.Sum(m => m.TDS);
                        ws1.Cell(16, 9).Value = model2.TDS94H.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(16, 10).Value = model2.TDS94H.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(16, 11).Value = model2.TDS94H.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(16, 12).Value = model2.TDS94H.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(16, 13).Value = "NO";
                        /////////////////////
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Form26Q.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult Form27Q(int Finyear, int Quator)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                        FinOp fac = new FinOp(System.DateTime.Now);
                        int CurrentYear = fromdate.Year;
                        int PreviousYear = fromdate.Year - 1;
                        int NextYear = fromdate.Year + 1;
                        string PreYear = PreviousYear.ToString();
                        string NexYear = NextYear.ToString();
                        string CurYear = CurrentYear.ToString();
                        DateTime[] QuarDate; DateTime Date1 = new DateTime();
                        DateTime Date2 = new DateTime(); DateTime Date3 = new DateTime();
                        DateTime startDate; DateTime endDate;
                        if (fromdate.Month > 2)
                        {
                            startDate = new DateTime(fromdate.Year, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        else
                        {
                            startDate = new DateTime((fromdate.Year) - 1, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        if (Quator == 1)
                        {
                            Date1 = new DateTime(startDate.Year, 4, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 5, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 6, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 2)
                        {
                            Date1 = new DateTime(startDate.Year, 7, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 8, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 9, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 3)
                        {
                            Date1 = new DateTime(startDate.Year, 10, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(startDate.Year, 11, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(startDate.Year, 12, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        if (Quator == 4)
                        {
                            Date1 = new DateTime(endDate.Year, 1, 01);
                            Date1 = Date1.AddMonths(1).AddDays(-1);
                            Date2 = new DateTime(endDate.Year, 2, 01);
                            Date2 = Date2.AddMonths(1).AddDays(-1);
                            Date3 = new DateTime(endDate.Year, 3, 01);
                            Date3 = Date3.AddMonths(1).AddDays(-1);
                        }
                        ReportService reportservice = new ReportService();

                        /////////  first month sheet  ////////
                        Form24QModel model = new Form24QModel();
                        model = ReportService.GetForm27Q(Date1);
                        DataSet dataset = new DataSet();
                        DataTable dtColumns1 = new DataTable();
                        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94C);
                        dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        var ws = wb.Worksheets.Add("DEDUCTORDETAILS");
                        ws.Cell(1, 1).Value = "TAX DEDUCTION ACCOUNT NO";
                        ws.Cell(1, 2).Value = "CHEI04464F";
                        ws.Cell(2, 1).Value = "PERMANENT ACCOUNT NO";
                        ws.Cell(2, 2).Value = "AAAAI3615G";
                        ws.Cell(3, 1).Value = "FINANCIAL YEAR";
                        ws.Cell(3, 2).Value = startDate.Year + "-" + endDate.Year;
                        ws.Cell(4, 1).Value = "ASSESSMENT YEAR";
                        ws.Cell(4, 2).Value = startDate.Year + "-" + endDate.Year;
                        ws.Cell(5, 1).Value = "HAS ANY STATEMENT BEEN FILED EARLIER FOR THIS QUARTER";
                        ws.Cell(5, 2).Value = "NO";
                        ws.Cell(6, 1).Value = "IF ANSWER IS 'YES', THEN PROVISIONAL RPT NO OF ORIGINAL STATEMENT";
                        ws.Cell(6, 2).Value = "-NA-";
                        ws.Cell(7, 1).Value = "NAME OF THE DEDUCTOR";
                        ws.Cell(7, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY ";
                        ws.Cell(8, 1).Value = "TYPE OF DEDUCTOR";
                        ws.Cell(8, 2).Value = "GOVERNMENT";
                        ws.Cell(9, 1).Value = "BRANCH/DIVISION(if any)";
                        ws.Cell(9, 2).Value = "NO";
                        ws.Cell(10, 1).Value = "ADDRESS 1";
                        ws.Cell(10, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws.Cell(11, 1).Value = "ADDRESS 2";
                        ws.Cell(11, 2).Value = "MADRAS";
                        ws.Cell(12, 1).Value = "ADDRESS 3";
                        ws.Cell(12, 2).Value = "CHENNAI-36";
                        ws.Cell(13, 1).Value = "ADDRESS 4";
                        ws.Cell(13, 2).Value = "";
                        ws.Cell(14, 1).Value = "ADDRESS 5";
                        ws.Cell(14, 2).Value = "";
                        ws.Cell(15, 1).Value = "STATE";
                        ws.Cell(15, 2).Value = "TAMIL NADU";
                        ws.Cell(16, 1).Value = "PINCODE";
                        ws.Cell(16, 2).Value = "600036";
                        ws.Cell(17, 1).Value = "TELEPHONE NO";
                        ws.Cell(17, 2).Value = "22578355";
                        ws.Cell(18, 1).Value = "EMAIL";
                        ws.Cell(18, 2).Value = "";
                        ws.Cell(19, 1).Value = "NAME OF THE PERSON RESPONSIBLE FOR DEDUCTION OF TAX";
                        ws.Cell(19, 2).Value = "";
                        ws.Cell(20, 1).Value = "ADDRESS 1";
                        ws.Cell(20, 2).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws.Cell(21, 1).Value = "ADDRESS 2";
                        ws.Cell(21, 2).Value = "MADRAS";
                        ws.Cell(22, 1).Value = "ADDRESS 3";
                        ws.Cell(22, 2).Value = "CHENNAI-36";
                        ws.Cell(23, 1).Value = "ADDRESS 4";
                        ws.Cell(23, 2).Value = "";
                        ws.Cell(24, 1).Value = "ADDRESS 5";
                        ws.Cell(24, 2).Value = "";
                        ws.Cell(25, 1).Value = "STATE";
                        ws.Cell(25, 2).Value = "TAMIL NADU";
                        ws.Cell(26, 1).Value = "PINCODE";
                        ws.Cell(26, 2).Value = "600036";
                        ws.Cell(27, 1).Value = "TELEPHONE NO";
                        ws.Cell(27, 2).Value = "22578355";
                        ws.Cell(28, 1).Value = "EMAIL";
                        ws.Cell(28, 2).Value = "";

                        //////Sheet 2 ///////

                        var ws1 = wb.Worksheets.Add("CHALLAN DETAILS");
                        ws1.Cell("D1").Value = "DETAILS OF TDS DEDUCTED FROM  " + String.Format("{0:MMMM}", Date1) + " " + startDate.Year + " TO " + String.Format("{0:MMMM}", Date3) + " " + endDate.Year + " - QTR " + Common.ToRoman(Quator);
                        ws1.Cell("D1").Style.Font.Bold = true;
                        ws1.Range("D1:F1").Row(1).Merge();
                        ws1.Cell(3, 1).Value = "SI.NO";
                        ws1.Cell(3, 2).Value = "SECTION CODE";
                        ws1.Cell(3, 3).Value = "TDS Rs.";
                        ws1.Cell(3, 4).Value = "SURCHARGE Rs.";
                        ws1.Cell(3, 5).Value = "EDUCATION CESS Rs.";
                        ws1.Cell(3, 6).Value = "INTEREST Rs.";
                        ws1.Cell(3, 7).Value = "OTHERS Rs.";
                        ws1.Cell(3, 8).Value = "TOTAL TAX DEPOSITED Rs.(403+404+405+406+407)";
                        ws1.Cell(3, 9).Value = "CHEQUE/DD NO";
                        ws1.Cell(3, 10).Value = "BSR CODE";
                        ws1.Cell(3, 11).Value = "DATE ON WHICH TAX DEPOSITED";
                        ws1.Cell(3, 12).Value = "TRANSFER VOUCHER/ CHALLAN SERIAL NO";
                        ws1.Cell(3, 13).Value = "WHETHER TDS DEPOSITED BY BOOK ENTRY? YES/NO";

                        ws1.Cell(4, 1).Value = "(401)";
                        ws1.Cell(4, 2).Value = "(402)";
                        ws1.Cell(4, 3).Value = "(403)";
                        ws1.Cell(4, 4).Value = "(404)";
                        ws1.Cell(4, 5).Value = "(405)";
                        ws1.Cell(4, 6).Value = "(406)";
                        ws1.Cell(4, 7).Value = "(407)";
                        ws1.Cell(4, 8).Value = "(408)";
                        ws1.Cell(4, 9).Value = "(409)";
                        ws1.Cell(4, 10).Value = "(410)";
                        ws1.Cell(4, 11).Value = "(411)";
                        ws1.Cell(4, 12).Value = "(412)";
                        ws1.Cell(4, 13).Value = "(413)";




                        ////////Sheet 3 ///////
                        var ws2 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date1));
                        ws2.Cell(1, 1).Value = "ANNEXURE – DEDUCTEE WISE BREAK-UP OF TDS";
                        ws2.Cell(2, 1).Value = "[Please use separate Annexure for each line item in the table at S. No. 4 of main Form 27Q]";
                        ws2.Cell(3, 1).Value = "Details of amounts paid/credited during the quarter ended        (DD-MM-YYYY) and of tax deducted at source";
                        ws2.Range("A1:U1").Row(1).Merge();
                        ws2.Range("A1:U1").Style.Font.Bold = true;
                        ws2.Range("A2:U2").Row(1).Merge();
                        ws2.Range("A2:U2").Style.Font.Bold = true;
                        ws2.Range("A3:U3").Row(1).Merge();
                        ws2.Range("A3:U3").Style.Font.Bold = true;

                        ws2.Cell(5, 1).Value = "BSR code of the branch/Receipt Number of Form No.24G   ";
                        ws2.Cell(6, 1).Value = "Date on which tax deposited / Transfer Voucher date (dd-mm-yyyy) Challan Serial No.";
                        ws2.Cell(7, 1).Value = "Challan Serial Number/DDO Serial No.of Form No.24G";
                        ws2.Cell(8, 1).Value = "Amount as per Challan";
                        ws2.Cell(9, 1).Value = "Total TDS to be allocated among deductees as in the verification of Col. 726";
                        ws2.Cell(10, 1).Value = "Interest Other";

                        ws2.Cell(5, 10).Value = "Name of Deductor";
                        ws2.Cell(5, 11).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws2.Cell(7, 10).Value = "TAN";
                        ws2.Cell(7, 11).Value = "CHEI04464F";

                        ws2.Cell(17, 1).Value = "SI.NO";
                        ws2.Cell(17, 2).Value = "Deductee reference number provided  by the deductor, if available";
                        ws2.Cell(17, 3).Value = "Deductee code (01- Company 02-Other than company)";
                        ws2.Cell(17, 4).Value = "PAN of the Deductee";
                        ws2.Cell(17, 5).Value = "Name of The deductee";
                        ws2.Cell(17, 6).Value = "Section code (see Note 1)";
                        ws2.Cell(17, 7).Value = "Date of payment or credit (dd/mm/y yyy)";
                        ws2.Cell(17, 8).Value = "Amount paid or credited";
                        ws2.Cell(17, 9).Value = "Tax";
                        ws2.Cell(17, 10).Value = "Surcharge";
                        ws2.Cell(17, 11).Value = "Education Cess";
                        ws2.Cell(17, 12).Value = "Total tax deducted [722+723+724]";
                        ws2.Cell(17, 13).Value = "Total tax deposited";
                        ws2.Cell(17, 14).Value = "Date of deduction (dd/mm/yy yy)";
                        ws2.Cell(17, 15).Value = "Rate at which deducted";
                        ws2.Cell(17, 16).Value = "Reason for non- deduction/ Lower deduction/ No/Higher Deductin (see note 2)";
                        ws2.Cell(17, 17).Value = "Number of the certificate issued by the Assessing Officer for non-deduction / lower Deduction (see note 3)";
                        ws2.Cell(17, 18).Value = "Whether the rate of TDS is as per IT Act (a)DTAA(b)(see note 4)";
                        ws2.Cell(17, 19).Value = "Nature of Remittance (see note 5)";
                        ws2.Cell(17, 20).Value = "Unique Acknowledg ement of the correspondi ng Form No. 15CA, if available";
                        ws2.Cell(17, 21).Value = "Country to which remittance   is made (see note 6)";
                        ws2.Cell(17, 22).Value = "Email id of deductee";
                        ws2.Cell(17, 23).Value = "Contact number of deductee";
                        ws2.Cell(17, 24).Value = "Address of deductee in country of residence";
                        ws2.Cell(17, 25).Value = "Tax Identification Number/Unique Identification Number of deductee";

                        ws2.Cell(18, 1).Value = "[714]";
                        ws2.Cell(18, 2).Value = "[715]";
                        ws2.Cell(18, 3).Value = "[716]";
                        ws2.Cell(18, 4).Value = "[717]";
                        ws2.Cell(18, 5).Value = "[718]";
                        ws2.Cell(18, 6).Value = "[719]";
                        ws2.Cell(18, 7).Value = "[720]";
                        ws2.Cell(18, 8).Value = "[721]";
                        ws2.Cell(18, 9).Value = "[722]";
                        ws2.Cell(18, 10).Value = "[723]";
                        ws2.Cell(18, 11).Value = "[724]";
                        ws2.Cell(18, 12).Value = "[725]";
                        ws2.Cell(18, 13).Value = "[726]";
                        ws2.Cell(18, 14).Value = "[727]";
                        ws2.Cell(18, 15).Value = "[728]";
                        ws2.Cell(18, 16).Value = "[729]";
                        ws2.Cell(18, 17).Value = "[730]";
                        ws2.Cell(18, 18).Value = "[731]";
                        ws2.Cell(18, 19).Value = "[732]";
                        ws2.Cell(18, 20).Value = "[733]";
                        ws2.Cell(18, 21).Value = "[734]";
                        ws2.Cell(18, 22).Value = "[735]";
                        ws2.Cell(18, 23).Value = "[736]";
                        ws2.Cell(18, 24).Value = "[737]";
                        ws2.Cell(18, 25).Value = "[738]";

                        int ws2firstrow = 19; int ws2firstcol = 1;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws2.Cell(ws2firstrow, 1).Value = row["SlNo"].ToString();
                            ws2.Cell(ws2firstrow, 2).Value = "";
                            ws2.Cell(ws2firstrow, 3).Value = "02";
                            ws2.Cell(ws2firstrow, 4).Value = row["PAN"].ToString();
                            ws2.Cell(ws2firstrow, 5).Value = row["Name"].ToString();
                            ws2.Cell(ws2firstrow, 6).Value = "195";
                            ws2.Cell(ws2firstrow, 7).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 8).Value = row["AmountPaid"].ToString();
                            ws2.Cell(ws2firstrow, 9).Value = row["TDS"].ToString();
                            ws2.Cell(ws2firstrow, 10).Value = "-";
                            ws2.Cell(ws2firstrow, 11).Value = "-";
                            ws2.Cell(ws2firstrow, 12).Value = row["TDS"].ToString();
                            ws2.Cell(ws2firstrow, 13).Value = row["TDS"].ToString();
                            ws2.Cell(ws2firstrow, 14).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 15).Value = row["Rate"].ToString();
                            ws2.Cell(ws2firstrow, 16).Value = "NA";
                            ws2.Cell(ws2firstrow, 17).Value = "NIL";
                            ws2.Cell(ws2firstrow, 18).Value = "IT Act (A)";
                            ws2.Cell(ws2firstrow, 19).Value = "FEES FOR TECHNICAL SERVICES/ FEES FOR INCLUDED SERVICES";
                            ws2.Cell(ws2firstrow, 20).Value = "NA";
                            ws2.Cell(ws2firstrow, 21).Value = "";
                            ws2.Cell(ws2firstrow, 22).Value = "";
                            ws2.Cell(ws2firstrow, 23).Value = "";
                            ws2.Cell(ws2firstrow, 24).Value = "";
                            ws2.Cell(ws2firstrow, 25).Value = "";
                            ws2firstrow++;
                        }
                        int ws2Total = ws2firstrow + 1;
                        ws2.Cell(ws2Total, 5).Value = "TOTAL (SEC.195)";
                        ws2.Cell(ws2Total, 7).Value = model.TDS94C.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2Total, 9).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 12).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 13).Value = model.TDS94C.Sum(m => m.TDS);
                        //ws2.Cell(ws2Total, 16).Value = model.TDS94C.Sum(m => m.TDS);

                        int ws2Id = ws2Total + 4;
                        ws2.Cell(ws2Id, 1).Value = "Verification";
                        ws2.Cell(ws2Id + 1, 1).Value = "I, ………………………………………………………………………, hereby certify that all the particulars furnished above are correct and complete";
                        ws2.Cell(ws2Id + 2, 1).Value = "Place:     …………………..";
                        ws2.Cell(ws2Id + 3, 1).Value = "Date:      ………………….. Note:";
                        ws2.Cell(ws2Id + 4, 1).Value = "1.  Mention section codes as per Annexure 2.";
                        ws2.Cell(ws2Id + 5, 1).Value = "2.  Mention the code as per Annexure 3.";
                        ws2.Cell(ws2Id + 6, 1).Value = "Signature of the person responsible for deducting tax at source";
                        ws2.Cell(ws2Id + 7, 1).Value = "Name and designation of the person responsible for deducting tax at source";
                        ws2.Cell(ws2Id + 8, 1).Value = "3.  Mandatory to mention certificate no. in case of lower or no deduction as per column no. 729.";
                        ws2.Cell(ws2Id + 9, 1).Value = "4.  If rate of TDS is as per Income Tax Act mention “A” and if rate of TDS is as per DTAA then mention “B”";
                        ws2.Cell(ws2Id + 10, 1).Value = "5.  Mention nature of remittance as per Annexure 4.";
                        ws2.Cell(ws2Id + 11, 1).Value = "6.  Mention the country of the residence of the deductee as per Annexure 5.";
                        ws2.Cell(ws2Id + 12, 1).Value = "7.  If Grossing up has been done mention “Y”, else mention as “N”.";

                        //var firstCell = ws2.FirstCellUsed();
                        //var lastCell = ws2.LastCellUsed();
                        //var range = ws2.Range(firstCell, lastCell);
                        //range.Clear(XLClearOptions.AllFormats);
                        //var table = range.CreateTable();
                        //table.Theme = XLTableTheme.TableStyleLight12;
                        //table.ShowAutoFilter = false;

                        ////////////////////////////////

                        /////////  second month sheet  ////////
                        Form24QModel model1 = new Form24QModel();
                        model1 = ReportService.GetForm27Q(Date2);
                        DataTable dtColumns5 = new DataTable();
                        string json5 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94C);
                        dtColumns5 = JsonConvert.DeserializeObject<DataTable>(json5);
                        var ws3 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date2));
                        ws3.Cell(1, 1).Value = "ANNEXURE – DEDUCTEE WISE BREAK-UP OF TDS";
                        ws3.Cell(2, 1).Value = "[Please use separate Annexure for each line item in the table at S. No. 4 of main Form 27Q]";
                        ws3.Cell(3, 1).Value = "Details of amounts paid/credited during the quarter ended        (DD-MM-YYYY) and of tax deducted at source";
                        ws3.Range("A1:U1").Row(1).Merge();
                        ws3.Range("A1:U1").Style.Font.Bold = true;
                        ws3.Range("A2:U2").Row(1).Merge();
                        ws3.Range("A2:U2").Style.Font.Bold = true;
                        ws3.Range("A3:U3").Row(1).Merge();
                        ws3.Range("A3:U3").Style.Font.Bold = true;

                        ws3.Cell(5, 1).Value = "BSR code of the branch/Receipt Number of Form No.24G   ";
                        ws3.Cell(6, 1).Value = "Date on which tax deposited / Transfer Voucher date (dd-mm-yyyy) Challan Serial No.";
                        ws3.Cell(7, 1).Value = "Challan Serial Number/DDO Serial No.of Form No.24G";
                        ws3.Cell(8, 1).Value = "Amount as per Challan";
                        ws3.Cell(9, 1).Value = "Total TDS to be allocated among deductees as in the verification of Col. 726";
                        ws3.Cell(10, 1).Value = "Interest Other";

                        ws3.Cell(5, 10).Value = "Name of Deductor";
                        ws3.Cell(5, 11).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws3.Cell(7, 10).Value = "TAN";
                        ws3.Cell(7, 11).Value = "CHEI04464F";

                        ws3.Cell(17, 1).Value = "SI.NO";
                        ws3.Cell(17, 2).Value = "Deductee reference number provided  by the deductor, if available";
                        ws3.Cell(17, 3).Value = "Deductee code (01- Company 02-Other than company)";
                        ws3.Cell(17, 4).Value = "PAN of the Deductee";
                        ws3.Cell(17, 5).Value = "Name of The deductee";
                        ws3.Cell(17, 6).Value = "Section code (see Note 1)";
                        ws3.Cell(17, 7).Value = "Date of payment or credit (dd/mm/y yyy)";
                        ws3.Cell(17, 8).Value = "Amount paid or credited";
                        ws3.Cell(17, 9).Value = "Tax";
                        ws3.Cell(17, 10).Value = "Surcharge";
                        ws3.Cell(17, 11).Value = "Education Cess";
                        ws3.Cell(17, 12).Value = "Total tax deducted [722+723+724]";
                        ws3.Cell(17, 13).Value = "Total tax deposited";
                        ws3.Cell(17, 14).Value = "Date of deduction (dd/mm/yy yy)";
                        ws3.Cell(17, 15).Value = "Rate at which deducted";
                        ws3.Cell(17, 16).Value = "Reason for non- deduction/ Lower deduction/ No/Higher Deductin (see note 2)";
                        ws3.Cell(17, 17).Value = "Number of the certificate issued by the Assessing Officer for non-deduction / lower Deduction (see note 3)";
                        ws3.Cell(17, 18).Value = "Whether the rate of TDS is as per IT Act (a)DTAA(b)(see note 4)";
                        ws3.Cell(17, 19).Value = "Nature of Remittance (see note 5)";
                        ws3.Cell(17, 20).Value = "Unique Acknowledg ement of the correspondi ng Form No. 15CA, if available";
                        ws3.Cell(17, 21).Value = "Country to which remittance   is made (see note 6)";
                        ws3.Cell(17, 22).Value = "Email id of deductee";
                        ws3.Cell(17, 23).Value = "Contact number of deductee";
                        ws3.Cell(17, 24).Value = "Address of deductee in country of residence";
                        ws3.Cell(17, 25).Value = "Tax Identification Number/Unique Identification Number of deductee";

                        ws3.Cell(18, 1).Value = "[714]";
                        ws3.Cell(18, 2).Value = "[715]";
                        ws3.Cell(18, 3).Value = "[716]";
                        ws3.Cell(18, 4).Value = "[717]";
                        ws3.Cell(18, 5).Value = "[718]";
                        ws3.Cell(18, 6).Value = "[719]";
                        ws3.Cell(18, 7).Value = "[720]";
                        ws3.Cell(18, 8).Value = "[721]";
                        ws3.Cell(18, 9).Value = "[722]";
                        ws3.Cell(18, 10).Value = "[723]";
                        ws3.Cell(18, 11).Value = "[724]";
                        ws3.Cell(18, 12).Value = "[725]";
                        ws3.Cell(18, 13).Value = "[726]";
                        ws3.Cell(18, 14).Value = "[727]";
                        ws3.Cell(18, 15).Value = "[728]";
                        ws3.Cell(18, 16).Value = "[729]";
                        ws3.Cell(18, 17).Value = "[730]";
                        ws3.Cell(18, 18).Value = "[731]";
                        ws3.Cell(18, 19).Value = "[732]";
                        ws3.Cell(18, 20).Value = "[733]";
                        ws3.Cell(18, 21).Value = "[734]";
                        ws3.Cell(18, 22).Value = "[735]";
                        ws3.Cell(18, 23).Value = "[736]";
                        ws3.Cell(18, 24).Value = "[737]";
                        ws3.Cell(18, 25).Value = "[738]";
                        int ws3firstrow = 19; int ws3firstcol = 1;
                        foreach (DataRow row in dtColumns5.Rows)
                        {
                            ws3.Cell(ws3firstrow, 1).Value = row["SlNo"].ToString();
                            ws3.Cell(ws3firstrow, 2).Value = "";
                            ws3.Cell(ws3firstrow, 3).Value = "02";
                            ws3.Cell(ws3firstrow, 4).Value = row["PAN"].ToString();
                            ws3.Cell(ws3firstrow, 5).Value = row["Name"].ToString();
                            ws3.Cell(ws3firstrow, 6).Value = "195";
                            ws3.Cell(ws3firstrow, 7).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3firstrow, 8).Value = row["AmountPaid"].ToString();
                            ws3.Cell(ws3firstrow, 9).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 10).Value = "-";
                            ws3.Cell(ws3firstrow, 11).Value = "-";
                            ws3.Cell(ws3firstrow, 12).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 13).Value = row["TDS"].ToString();
                            ws3.Cell(ws3firstrow, 14).Value = row["DateofPayment"].ToString();
                            ws3.Cell(ws3firstrow, 15).Value = row["Rate"].ToString();
                            ws3.Cell(ws3firstrow, 16).Value = "NA";
                            ws3.Cell(ws3firstrow, 17).Value = "NIL";
                            ws3.Cell(ws3firstrow, 18).Value = "IT Act (A)";
                            ws3.Cell(ws3firstrow, 19).Value = "FEES FOR TECHNICAL SERVICES/ FEES FOR INCLUDED SERVICES";
                            ws3.Cell(ws3firstrow, 20).Value = "NA";
                            ws3.Cell(ws3firstrow, 21).Value = "";
                            ws3.Cell(ws3firstrow, 22).Value = "";
                            ws3.Cell(ws3firstrow, 23).Value = "";
                            ws3.Cell(ws3firstrow, 24).Value = "";
                            ws3.Cell(ws3firstrow, 25).Value = "";

                            ws3firstrow++;
                        }
                        int ws3Total = ws3firstrow + 1;
                        ws3.Cell(ws3Total, 5).Value = "TOTAL (SEC.195)";
                        ws3.Cell(ws3Total, 7).Value = model1.TDS94C.Sum(m => m.AmountPaid);
                        ws3.Cell(ws3Total, 9).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws3.Cell(ws3Total, 12).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws3.Cell(ws3Total, 13).Value = model1.TDS94C.Sum(m => m.TDS);
                        //ws3.Cell(ws3Total, 16).Value = model1.TDS94C.Sum(m => m.TDS);


                        int ws3Id = ws3Total + 4;
                        ws3.Cell(ws3Id, 1).Value = "Verification";
                        ws3.Cell(ws3Id + 1, 1).Value = "I, ………………………………………………………………………, hereby certify that all the particulars furnished above are correct and complete";
                        ws3.Cell(ws3Id + 2, 1).Value = "Place:     …………………..";
                        ws3.Cell(ws3Id + 3, 1).Value = "Date:      ………………….. Note:";
                        ws3.Cell(ws3Id + 4, 1).Value = "1.  Mention section codes as per Annexure 2.";
                        ws3.Cell(ws3Id + 5, 1).Value = "2.  Mention the code as per Annexure 3.";
                        ws3.Cell(ws3Id + 6, 1).Value = "Signature of the person responsible for deducting tax at source";
                        ws3.Cell(ws3Id + 7, 1).Value = "Name and designation of the person responsible for deducting tax at source";
                        ws3.Cell(ws3Id + 8, 1).Value = "3.  Mandatory to mention certificate no. in case of lower or no deduction as per column no. 729.";
                        ws3.Cell(ws3Id + 9, 1).Value = "4.  If rate of TDS is as per Income Tax Act mention “A” and if rate of TDS is as per DTAA then mention “B”";
                        ws3.Cell(ws3Id + 10, 1).Value = "5.  Mention nature of remittance as per Annexure 4.";
                        ws3.Cell(ws3Id + 11, 1).Value = "6.  Mention the country of the residence of the deductee as per Annexure 5.";
                        ws3.Cell(ws3Id + 12, 1).Value = "7.  If Grossing up has been done mention “Y”, else mention as “N”.";

                        //var firstCell4 = ws3.FirstCellUsed();
                        //var lastCell4 = ws3.LastCellUsed();
                        //var range4 = ws3.Range(firstCell4, lastCell4);
                        //range4.Clear(XLClearOptions.AllFormats);
                        //var table4 = range4.CreateTable();
                        //table4.Theme = XLTableTheme.TableStyleLight12;
                        //table4.ShowAutoFilter = false;

                        //////////////////////

                        /////////  third month sheet  ////////
                        Form24QModel model2 = new Form24QModel();
                        model2 = ReportService.GetForm27Q(Date3);
                        DataTable dtColumns9 = new DataTable();
                        string json9 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94C);
                        dtColumns9 = JsonConvert.DeserializeObject<DataTable>(json9);
                        var ws4 = wb.Worksheets.Add(String.Format("{0:MMMM}", Date3));
                        ws4.Cell(1, 1).Value = "ANNEXURE – DEDUCTEE WISE BREAK-UP OF TDS";
                        ws4.Cell(2, 1).Value = "[Please use separate Annexure for each line item in the table at S. No. 4 of main Form 27Q]";
                        ws4.Cell(3, 1).Value = "Details of amounts paid/credited during the quarter ended        (DD-MM-YYYY) and of tax deducted at source";
                        ws4.Range("A1:U1").Row(1).Merge();
                        ws4.Range("A1:U1").Style.Font.Bold = true;
                        ws4.Range("A2:U2").Row(1).Merge();
                        ws4.Range("A2:U2").Style.Font.Bold = true;
                        ws4.Range("A3:U3").Row(1).Merge();
                        ws4.Range("A3:U3").Style.Font.Bold = true;

                        ws4.Cell(5, 1).Value = "BSR code of the branch/Receipt Number of Form No.24G   ";
                        ws4.Cell(6, 1).Value = "Date on which tax deposited / Transfer Voucher date (dd-mm-yyyy) Challan Serial No.";
                        ws4.Cell(7, 1).Value = "Challan Serial Number/DDO Serial No.of Form No.24G";
                        ws4.Cell(8, 1).Value = "Amount as per Challan";
                        ws4.Cell(9, 1).Value = "Total TDS to be allocated among deductees as in the verification of Col. 726";
                        ws4.Cell(10, 1).Value = "Interest Other";

                        ws4.Cell(5, 10).Value = "Name of Deductor";
                        ws4.Cell(5, 11).Value = "INDIAN INSTITUTE OF TECHNOLOGY";
                        ws4.Cell(7, 10).Value = "TAN";
                        ws4.Cell(7, 11).Value = "CHEI04464F";

                        ws4.Cell(17, 1).Value = "SI.NO";
                        ws4.Cell(17, 2).Value = "Deductee reference number provided  by the deductor, if available";
                        ws4.Cell(17, 3).Value = "Deductee code (01- Company 02-Other than company)";
                        ws4.Cell(17, 4).Value = "PAN of the Deductee";
                        ws4.Cell(17, 5).Value = "Name of The deductee";
                        ws4.Cell(17, 6).Value = "Section code (see Note 1)";
                        ws4.Cell(17, 7).Value = "Date of payment or credit (dd/mm/y yyy)";
                        ws4.Cell(17, 8).Value = "Amount paid or credited";
                        ws4.Cell(17, 9).Value = "Tax";
                        ws4.Cell(17, 10).Value = "Surcharge";
                        ws4.Cell(17, 11).Value = "Education Cess";
                        ws4.Cell(17, 12).Value = "Total tax deducted [722+723+724]";
                        ws4.Cell(17, 13).Value = "Total tax deposited";
                        ws4.Cell(17, 14).Value = "Date of deduction (dd/mm/yy yy)";
                        ws4.Cell(17, 15).Value = "Rate at which deducted";
                        ws4.Cell(17, 16).Value = "Reason for non- deduction/ Lower deduction/ No/Higher Deductin (see note 2)";
                        ws4.Cell(17, 17).Value = "Number of the certificate issued by the Assessing Officer for non-deduction / lower Deduction (see note 3)";
                        ws4.Cell(17, 18).Value = "Whether the rate of TDS is as per IT Act (a)DTAA(b)(see note 4)";
                        ws4.Cell(17, 19).Value = "Nature of Remittance (see note 5)";
                        ws4.Cell(17, 20).Value = "Unique Acknowledg ement of the correspondi ng Form No. 15CA, if available";
                        ws4.Cell(17, 21).Value = "Country to which remittance   is made (see note 6)";
                        ws4.Cell(17, 22).Value = "Email id of deductee";
                        ws4.Cell(17, 23).Value = "Contact number of deductee";
                        ws4.Cell(17, 24).Value = "Address of deductee in country of residence";
                        ws4.Cell(17, 25).Value = "Tax Identification Number/Unique Identification Number of deductee";

                        ws4.Cell(18, 1).Value = "[714]";
                        ws4.Cell(18, 2).Value = "[715]";
                        ws4.Cell(18, 3).Value = "[716]";
                        ws4.Cell(18, 4).Value = "[717]";
                        ws4.Cell(18, 5).Value = "[718]";
                        ws4.Cell(18, 6).Value = "[719]";
                        ws4.Cell(18, 7).Value = "[720]";
                        ws4.Cell(18, 8).Value = "[721]";
                        ws4.Cell(18, 9).Value = "[722]";
                        ws4.Cell(18, 10).Value = "[723]";
                        ws4.Cell(18, 11).Value = "[724]";
                        ws4.Cell(18, 12).Value = "[725]";
                        ws4.Cell(18, 13).Value = "[726]";
                        ws4.Cell(18, 14).Value = "[727]";
                        ws4.Cell(18, 15).Value = "[728]";
                        ws4.Cell(18, 16).Value = "[729]";
                        ws4.Cell(18, 17).Value = "[730]";
                        ws4.Cell(18, 18).Value = "[731]";
                        ws4.Cell(18, 19).Value = "[732]";
                        ws4.Cell(18, 20).Value = "[733]";
                        ws4.Cell(18, 21).Value = "[734]";
                        ws4.Cell(18, 22).Value = "[735]";
                        ws4.Cell(18, 23).Value = "[736]";
                        ws4.Cell(18, 24).Value = "[737]";
                        ws4.Cell(18, 25).Value = "[738]";
                        int ws4firstrow = 19; int ws4firstcol = 1;
                        foreach (DataRow row in dtColumns9.Rows)
                        {
                            ws4.Cell(ws4firstrow, 1).Value = row["SlNo"].ToString();
                            ws4.Cell(ws4firstrow, 2).Value = "";
                            ws4.Cell(ws4firstrow, 3).Value = "02";
                            ws4.Cell(ws4firstrow, 4).Value = row["PAN"].ToString();
                            ws4.Cell(ws4firstrow, 5).Value = row["Name"].ToString();
                            ws4.Cell(ws4firstrow, 6).Value = "195";
                            ws4.Cell(ws4firstrow, 7).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4firstrow, 8).Value = row["AmountPaid"].ToString();
                            ws4.Cell(ws4firstrow, 9).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 10).Value = "-";
                            ws4.Cell(ws4firstrow, 11).Value = "-";
                            ws4.Cell(ws4firstrow, 12).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 13).Value = row["TDS"].ToString();
                            ws4.Cell(ws4firstrow, 14).Value = row["DateofPayment"].ToString();
                            ws4.Cell(ws4firstrow, 15).Value = row["Rate"].ToString();
                            ws4.Cell(ws4firstrow, 16).Value = "NA";
                            ws4.Cell(ws4firstrow, 17).Value = "NIL";
                            ws4.Cell(ws4firstrow, 18).Value = "IT Act (A)";
                            ws4.Cell(ws4firstrow, 19).Value = "FEES FOR TECHNICAL SERVICES/ FEES FOR INCLUDED SERVICES";
                            ws4.Cell(ws4firstrow, 20).Value = "NA";
                            ws4.Cell(ws4firstrow, 21).Value = "";
                            ws4.Cell(ws4firstrow, 22).Value = "";
                            ws4.Cell(ws4firstrow, 23).Value = "";
                            ws4.Cell(ws4firstrow, 24).Value = "";
                            ws4.Cell(ws4firstrow, 25).Value = "";
                            ws4firstrow++;
                        }
                        int ws4Total = ws4firstrow + 1;
                        ws4.Cell(ws4Total, 5).Value = "TOTAL (SEC.195)";
                        ws4.Cell(ws4Total, 7).Value = model2.TDS94C.Sum(m => m.AmountPaid);
                        ws4.Cell(ws4Total, 9).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws4.Cell(ws4Total, 12).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws4.Cell(ws4Total, 13).Value = model2.TDS94C.Sum(m => m.TDS);
                        //ws4.Cell(ws4Total, 16).Value = model2.TDS94C.Sum(m => m.TDS);

                        int ws4Id = ws4Total + 4;
                        ws4.Cell(ws4Id, 1).Value = "Verification";
                        ws4.Cell(ws4Id + 1, 1).Value = "I, ………………………………………………………………………, hereby certify that all the particulars furnished above are correct and complete";
                        ws4.Cell(ws4Id + 2, 1).Value = "Place:     …………………..";
                        ws4.Cell(ws4Id + 3, 1).Value = "Date:      ………………….. Note:";
                        ws4.Cell(ws4Id + 4, 1).Value = "1.  Mention section codes as per Annexure 2.";
                        ws4.Cell(ws4Id + 5, 1).Value = "2.  Mention the code as per Annexure 3.";
                        ws4.Cell(ws4Id + 6, 1).Value = "Signature of the person responsible for deducting tax at source";
                        ws4.Cell(ws4Id + 7, 1).Value = "Name and designation of the person responsible for deducting tax at source";
                        ws4.Cell(ws4Id + 8, 1).Value = "3.  Mandatory to mention certificate no. in case of lower or no deduction as per column no. 729.";
                        ws4.Cell(ws4Id + 9, 1).Value = "4.  If rate of TDS is as per Income Tax Act mention “A” and if rate of TDS is as per DTAA then mention “B”";
                        ws4.Cell(ws4Id + 10, 1).Value = "5.  Mention nature of remittance as per Annexure 4.";
                        ws4.Cell(ws4Id + 11, 1).Value = "6.  Mention the country of the residence of the deductee as per Annexure 5.";
                        ws4.Cell(ws4Id + 12, 1).Value = "7.  If Grossing up has been done mention “Y”, else mention as “N”.";



                        ws1.Cell(5, 1).Value = "1";
                        ws1.Cell(5, 2).Value = "195";
                        ws1.Cell(5, 3).Value = model.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(5, 4).Value = "-";
                        ws1.Cell(5, 5).Value = "-";
                        ws1.Cell(5, 6).Value = "-";
                        ws1.Cell(5, 7).Value = "-";
                        ws1.Cell(5, 8).Value = model.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(5, 9).Value = model.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(5, 10).Value = model.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(5, 11).Value = model.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(5, 12).Value = model.TDS94C.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(5, 13).Value = "NO";

                        ws1.Cell(6, 1).Value = "1";
                        ws1.Cell(6, 2).Value = "195";
                        ws1.Cell(6, 3).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(6, 4).Value = "-";
                        ws1.Cell(6, 5).Value = "-";
                        ws1.Cell(6, 6).Value = "-";
                        ws1.Cell(6, 7).Value = "-";
                        ws1.Cell(6, 8).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(6, 9).Value = model1.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(6, 10).Value = model1.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(6, 11).Value = model1.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(6, 12).Value = model1.TDS94C.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(6, 13).Value = "NO";

                        ws1.Cell(7, 1).Value = "1";
                        ws1.Cell(7, 2).Value = "195";
                        ws1.Cell(7, 3).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(7, 4).Value = "-";
                        ws1.Cell(7, 5).Value = "-";
                        ws1.Cell(7, 6).Value = "-";
                        ws1.Cell(7, 7).Value = "-";
                        ws1.Cell(7, 8).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws1.Cell(7, 9).Value = model2.TDS94C.Select(m => m.ChallenNo).FirstOrDefault();
                        ws1.Cell(7, 10).Value = model2.TDS94C.Select(m => m.BSRCode).FirstOrDefault();
                        ws1.Cell(7, 11).Value = model2.TDS94C.Select(m => m.DateofPayment).FirstOrDefault();
                        ws1.Cell(7, 12).Value = model2.TDS94C.Select(m => m.ChallenNo).FirstOrDefault(); ;
                        ws1.Cell(7, 13).Value = "NO";
                        //var firstCell8 = ws4.FirstCellUsed();
                        //var lastCell8 = ws4.LastCellUsed();
                        //var range8 = ws4.Range(firstCell8, lastCell8);
                        //range8.Clear(XLClearOptions.AllFormats);
                        //var table8 = range8.CreateTable();
                        //table8.Theme = XLTableTheme.TableStyleLight12;
                        //table8.ShowAutoFilter = false;
                        //////////////////////

                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Form27Q.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult Form24Q(int Finyear, int Quator)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                        FinOp fac = new FinOp(System.DateTime.Now);
                        int CurrentYear = fromdate.Year;
                        int PreviousYear = fromdate.Year - 1;
                        int NextYear = fromdate.Year + 1;
                        string PreYear = PreviousYear.ToString();
                        string NexYear = NextYear.ToString();
                        string CurYear = CurrentYear.ToString();
                        DateTime[] QuarDate; DateTime Date1 = new DateTime();
                        DateTime Date2 = new DateTime(); DateTime Date3 = new DateTime();
                        DateTime Date4 = new DateTime();
                        DateTime Date5 = new DateTime(); DateTime Date6 = new DateTime();
                        DateTime startDate; DateTime endDate;
                        if (fromdate.Month > 2)
                        {
                            startDate = new DateTime(fromdate.Year, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        else
                        {
                            startDate = new DateTime((fromdate.Year) - 1, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        if (Quator == 1)
                        {
                            Date1 = new DateTime(startDate.Year, 4, 01);
                            Date2 = new DateTime(startDate.Year, 5, 01);
                            Date3 = new DateTime(startDate.Year, 6, 01);
                        }
                        if (Quator == 2)
                        {
                            Date1 = new DateTime(startDate.Year, 7, 01);
                            Date2 = new DateTime(startDate.Year, 8, 01);
                            Date3 = new DateTime(startDate.Year, 9, 01);
                        }
                        if (Quator == 3)
                        {
                            Date1 = new DateTime(startDate.Year, 10, 01);
                            Date2 = new DateTime(startDate.Year, 11, 01);
                            Date3 = new DateTime(startDate.Year, 12, 01);
                        }
                        if (Quator == 4)
                        {
                            Date1 = new DateTime(endDate.Year, 1, 01);
                            Date2 = new DateTime(endDate.Year, 2, 01);
                            Date3 = new DateTime(endDate.Year, 3, 01);
                        }
                        Date4 = Date1.AddMonths(1).AddTicks(-1);
                        Date5 = Date2.AddMonths(1).AddTicks(-1);
                        Date6 = Date3.AddMonths(1).AddTicks(-1);
                        ReportService reportservice = new ReportService();
                        /////////  first month sheet  ////////
                        Form24QModel model = new Form24QModel();
                        Form24QModel model1 = new Form24QModel();
                        Form24QModel model2 = new Form24QModel();
                        model = ReportService.GetForm24Q(Date1, Date4);
                        model1 = ReportService.GetForm24Q(Date2, Date5);
                        model2 = ReportService.GetForm24Q(Date3, Date6);
                        DataSet dataset = new DataSet();
                        DataTable dtColumns1 = new DataTable();
                        DataTable dtColumns2 = new DataTable();
                        DataTable dtColumns3 = new DataTable();
                        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.TDS94C);
                        dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.TDS94C);
                        dtColumns2 = JsonConvert.DeserializeObject<DataTable>(json2);
                        string json3 = Newtonsoft.Json.JsonConvert.SerializeObject(model2.TDS94C);
                        dtColumns3 = JsonConvert.DeserializeObject<DataTable>(json3);
                        var ws1 = wb.Worksheets.Add("CHALLAN DETAILS");
                        ws1.Cell(1, 1).Value = "SI.NO";
                        ws1.Cell(1, 2).Value = "TDS Rs.";
                        ws1.Cell(1, 3).Value = "SURCHARGE Rs.";
                        ws1.Cell(1, 4).Value = "EDUCATION CESS Rs.";
                        ws1.Cell(1, 5).Value = "INTEREST Rs.";
                        ws1.Cell(1, 6).Value = "OTHERS Rs.";
                        ws1.Cell(1, 7).Value = "TOTAL TAX DEPOSITED Rs.(302+303+304+305+306)";
                        ws1.Cell(1, 8).Value = "CHEQUE/DD NO";
                        ws1.Cell(1, 9).Value = "BSR CODE";
                        ws1.Cell(1, 10).Value = "DATE ON WHICH TAX DEPOSITED";
                        ws1.Cell(1, 11).Value = "TRANSFER VOUCHER/ CHALLAN SERIAL NO";
                        ws1.Cell(1, 12).Value = "WHETHER TDS DEPOSITED BY BOOK ENTRY? YES/NO";
                        ws1.Cell(2, 1).Value = "(301)";
                        ws1.Cell(2, 2).Value = "(302)";
                        ws1.Cell(2, 3).Value = "(303)";
                        ws1.Cell(2, 4).Value = "(304)";
                        ws1.Cell(2, 5).Value = "(305)";
                        ws1.Cell(2, 6).Value = "(306)";
                        ws1.Cell(2, 7).Value = "(307)";
                        ws1.Cell(2, 8).Value = "(308)";
                        ws1.Cell(2, 9).Value = "(309)";
                        ws1.Cell(2, 10).Value = "(310)";
                        ws1.Cell(2, 11).Value = "(311)";
                        ws1.Cell(2, 12).Value = "(312)";
                        int ws1firstrow = 3; int ws1firstcol = 1;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws1.Cell(ws1firstrow, 1).Value = row["PaymentMonth"].ToString();
                            ws1.Cell(ws1firstrow, 2).Value = row["TDS"].ToString();
                            ws1.Cell(ws1firstrow, 3).Value = "";
                            ws1.Cell(ws1firstrow, 4).Value = row["EducationFees"].ToString();
                            ws1.Cell(ws1firstrow, 5).Value = "";
                            ws1.Cell(ws1firstrow, 6).Value = "";
                            ws1.Cell(ws1firstrow, 7).Value = row["Deduction"].ToString();
                            ws1.Cell(ws1firstrow, 8).Value = row["ChallenNo"].ToString();
                            ws1.Cell(ws1firstrow, 9).Value = row["BSRCode"].ToString();
                            ws1.Cell(ws1firstrow, 10).Value = row["DateofPayment"].ToString();
                            ws1.Cell(ws1firstrow, 11).Value = row["ChallenNo"].ToString();
                            ws1.Cell(ws1firstrow, 12).Value = "";

                            ws1firstrow++;
                        }
                        ////////Sheet 3 ///////
                        var ws2 = wb.Worksheets.Add("challan wise Deduction Breakup");
                        ws2.Cell(1, 1).Value = "PaymentMonth";
                        ws2.Cell(1, 2).Value = "Employee reference no. provided by employer";
                        ws2.Cell(1, 3).Value = "PAN of the Employee";
                        ws2.Cell(1, 4).Value = "Name of the employee";
                        ws2.Cell(1, 5).Value = "Date of Payment/credit";
                        ws2.Cell(1, 6).Value = "Taxable amount on which tax deducted Rs.";
                        ws2.Cell(1, 7).Value = "TDS Rs.";
                        ws2.Cell(1, 8).Value = "Surcharge Rs.";
                        ws2.Cell(1, 9).Value = "Educational Cess Rs.";
                        ws2.Cell(1, 10).Value = "Total tax deducted (319+320+321)";
                        ws2.Cell(1, 11).Value = "Total tax deposited Rs.";
                        ws2.Cell(1, 12).Value = "Interest";
                        ws2.Cell(1, 13).Value = "Others";
                        ws2.Cell(1, 14).Value = "Total (322+Interest+ Others)";
                        ws2.Cell(1, 15).Value = "BSR Code";
                        ws2.Cell(1, 16).Value = "Challan Serial No.";
                        ws2.Cell(1, 17).Value = "Date on which tax Deposited";
                        ws2.Cell(1, 18).Value = "Date of deduction";
                        ws2.Cell(1, 19).Value = "Date of Deposit";
                        ws2.Cell(1, 20).Value = "Reason for non-deduction/lower deduction*";
                        ws2.Cell(2, 1).Value = "(313)";
                        ws2.Cell(2, 2).Value = "(314)";
                        ws2.Cell(2, 3).Value = "(315)";
                        ws2.Cell(2, 4).Value = "(316)";
                        ws2.Cell(2, 5).Value = "(317)";
                        ws2.Cell(2, 6).Value = "(318)";
                        ws2.Cell(2, 7).Value = "(319)";
                        ws2.Cell(2, 8).Value = "(320)";
                        ws2.Cell(2, 9).Value = "(321)";
                        ws2.Cell(2, 10).Value = "(322)";
                        ws2.Cell(2, 11).Value = "(323)";
                        ws2.Cell(2, 12).Value = "(323A)";
                        ws2.Cell(2, 13).Value = "(323B)";
                        ws2.Cell(2, 14).Value = "(323C)";
                        ws2.Cell(2, 15).Value = "(323D)";
                        ws2.Cell(2, 16).Value = "(323E)";
                        ws2.Cell(2, 17).Value = "(323F)";
                        ws2.Cell(2, 18).Value = "(324)";
                        ws2.Cell(2, 19).Value = "(325)";
                        ws2.Cell(2, 20).Value = "(326)";
                        int ws2firstrow = 3; int ws2firstcol = 1;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws2.Cell(ws2firstrow, 1).Value = row["PaymentMonth"].ToString();
                            ws2.Cell(ws2firstrow, 2).Value = row["EmployeeId"].ToString();
                            ws2.Cell(ws2firstrow, 3).Value = row["PAN"].ToString();
                            ws2.Cell(ws2firstrow, 4).Value = row["Name"].ToString();
                            ws2.Cell(ws2firstrow, 5).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 6).Value = row["AmountPaid"].ToString();
                            ws2.Cell(ws2firstrow, 7).Value = row["TDS"].ToString();
                            ws2.Cell(ws2firstrow, 8).Value = "0";
                            ws2.Cell(ws2firstrow, 9).Value = row["EducationFees"].ToString();
                            ws2.Cell(ws2firstrow, 10).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2firstrow, 11).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2firstrow, 12).Value = "0";
                            ws2.Cell(ws2firstrow, 13).Value = "0";
                            ws2.Cell(ws2firstrow, 14).Value = "0";
                            ws2.Cell(ws2firstrow, 15).Value = row["BSRCode"].ToString();
                            ws2.Cell(ws2firstrow, 16).Value = row["ChallenNo"].ToString();
                            ws2.Cell(ws2firstrow, 17).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 18).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 19).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2firstrow, 20).Value = "";
                            ws2firstrow++;
                        }
                        int ws2Total = ws2firstrow + 1;
                        ws2.Cell(ws2Total, 5).Value = "TOTAL";
                        ws2.Cell(ws2Total, 11).Value = model.TDS94C.Sum(m => m.Deduction);
                        ws2.Cell(ws2Total, 6).Value = model.TDS94C.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2Total, 7).Value = model.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2Total, 9).Value = model.TDS94C.Sum(m => m.EducationFees);

                        int ws2secondrowlist = ws2Total + 2;
                        foreach (DataRow row in dtColumns2.Rows)
                        {
                            ws2.Cell(ws2secondrowlist, 1).Value = row["SlNo"].ToString();
                            ws2.Cell(ws2secondrowlist, 2).Value = row["EmployeeId"].ToString();
                            ws2.Cell(ws2secondrowlist, 3).Value = row["PAN"].ToString();
                            ws2.Cell(ws2secondrowlist, 4).Value = row["Name"].ToString();
                            ws2.Cell(ws2secondrowlist, 5).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2secondrowlist, 6).Value = row["AmountPaid"].ToString();
                            ws2.Cell(ws2secondrowlist, 7).Value = row["TDS"].ToString();
                            ws2.Cell(ws2secondrowlist, 8).Value = "0";
                            ws2.Cell(ws2secondrowlist, 9).Value = row["EducationFees"].ToString();
                            ws2.Cell(ws2secondrowlist, 10).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2secondrowlist, 11).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2secondrowlist, 12).Value = "0";
                            ws2.Cell(ws2secondrowlist, 13).Value = "0";
                            ws2.Cell(ws2secondrowlist, 14).Value = "0";
                            ws2.Cell(ws2secondrowlist, 15).Value = row["BSRCode"].ToString();
                            ws2.Cell(ws2secondrowlist, 16).Value = row["ChallenNo"].ToString();
                            ws2.Cell(ws2secondrowlist, 17).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2secondrowlist, 18).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2secondrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2secondrowlist, 20).Value = "";
                            ws2secondrowlist++;
                        }
                        int ws2ITotal = ws2secondrowlist + 1;
                        ws2.Cell(ws2ITotal, 5).Value = "TOTAL";
                        ws2.Cell(ws2ITotal, 11).Value = model1.TDS94C.Sum(m => m.Deduction);
                        ws2.Cell(ws2ITotal, 6).Value = model1.TDS94C.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2ITotal, 7).Value = model1.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2ITotal, 9).Value = model1.TDS94C.Sum(m => m.EducationFees);
                        //var firstCell1 = ws2.Cell(ws2Total + 2, 1);
                        //var lastCell1 = ws2.Cell(ws2ITotal, 20);
                        //var range1 = ws2.Range(firstCell1, lastCell1);
                        //range1.Clear(XLClearOptions.AllFormats);
                        //var table1 = range1.CreateTable();
                        //table1.Theme = XLTableTheme.TableStyleLight12;
                        //table1.ShowAutoFilter = false;


                        int ws2thirdrowlist = ws2ITotal + 2;
                        foreach (DataRow row in dtColumns3.Rows)
                        {
                            ws2.Cell(ws2thirdrowlist, 1).Value = row["SlNo"].ToString();
                            ws2.Cell(ws2thirdrowlist, 2).Value = row["EmployeeId"].ToString();
                            ws2.Cell(ws2thirdrowlist, 3).Value = row["PAN"].ToString();
                            ws2.Cell(ws2thirdrowlist, 4).Value = row["Name"].ToString();
                            ws2.Cell(ws2thirdrowlist, 5).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2thirdrowlist, 6).Value = row["AmountPaid"].ToString();
                            ws2.Cell(ws2thirdrowlist, 7).Value = row["TDS"].ToString();
                            ws2.Cell(ws2thirdrowlist, 8).Value = "0";
                            ws2.Cell(ws2thirdrowlist, 9).Value = row["EducationFees"].ToString();
                            ws2.Cell(ws2thirdrowlist, 10).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2thirdrowlist, 11).Value = row["Deduction"].ToString();
                            ws2.Cell(ws2thirdrowlist, 12).Value = "0";
                            ws2.Cell(ws2thirdrowlist, 13).Value = "0";
                            ws2.Cell(ws2thirdrowlist, 14).Value = "0";
                            ws2.Cell(ws2thirdrowlist, 15).Value = row["BSRCode"].ToString();
                            ws2.Cell(ws2thirdrowlist, 16).Value = row["ChallenNo"].ToString();
                            ws2.Cell(ws2thirdrowlist, 17).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2thirdrowlist, 18).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2thirdrowlist, 19).Value = row["DateofPayment"].ToString();
                            ws2.Cell(ws2thirdrowlist, 20).Value = "";
                            ws2thirdrowlist++;
                        }
                        int ws2JTotal = ws2thirdrowlist + 1;
                        ws2.Cell(ws2JTotal, 5).Value = "TOTAL";
                        ws2.Cell(ws2JTotal, 11).Value = model2.TDS94C.Sum(m => m.Deduction);
                        ws2.Cell(ws2JTotal, 6).Value = model2.TDS94C.Sum(m => m.AmountPaid);
                        ws2.Cell(ws2JTotal, 7).Value = model2.TDS94C.Sum(m => m.TDS);
                        ws2.Cell(ws2JTotal, 9).Value = model2.TDS94C.Sum(m => m.EducationFees);

                        //var firstCell2 = ws2.Cell(ws2ITotal + 2, 1);
                        //var lastCell2 = ws2.Cell(ws2JTotal, 20);
                        //var range2 = ws2.Range(firstCell2, lastCell2);
                        //range2.Clear(XLClearOptions.AllFormats);
                        //var table2 = range2.CreateTable();
                        //table2.Theme = XLTableTheme.TableStyleLight12;
                        //table2.ShowAutoFilter = false;

                        //var firstCell = ws2.FirstCellUsed();
                        //var lastCell = ws2.LastCellUsed();
                        //var range = ws2.Range(firstCell, lastCell);
                        //range.Clear(XLClearOptions.AllFormats);
                        //var table = range.CreateTable();
                        //table.Theme = XLTableTheme.TableStyleLight12;
                        //table.ShowAutoFilter = false;
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Form24Q.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Form()
        {
            BillStatusModel model = new BillStatusModel();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
            ViewBag.Finyr = Common.GetAllFinancial();
            ViewBag.FormType = Common.GetCodeControlList("Form24Type");
            ViewBag.FormPeriod = Common.GetCodeControlList("Form24Period");
            model.AttachmentDetail = model.AttachmentDetail ?? new List<AttachmentDetailModel>();
            model.AttachmentDetail2 = model.AttachmentDetail2 ?? new List<AttachmentDetailModel2>();
            return View(model);
        }
        [HttpPost]
        public ActionResult Form(BillStatusModel model)
        {
            ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
            ViewBag.Finyr = Common.GetAllFinancial();
            ViewBag.FormType = Common.GetCodeControlList("Form24Type");
            ViewBag.FormPeriod = Common.GetCodeControlList("Form24Period");

            int logged_in_user = Common.GetUserid(User.Identity.Name);
            if (model.FinancialYear > 0 && model.Period > 0 && model.FormId > 0)
            {
                model = coreaccountService.GetFormDocument(model, 1, logged_in_user);
            }
            else
            {
                model.AttachmentDetail = model.AttachmentDetail ?? new List<AttachmentDetailModel>();
                model.AttachmentDetail2 = model.AttachmentDetail2 ?? new List<AttachmentDetailModel2>();
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult GetFormDoc(int FinancialYear, int FormId, int Period)
        {
            try
            {
                BillStatusModel model = new BillStatusModel();
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                model.Period = Period;
                model.FormId = FormId;
                model.FinancialYear = FinancialYear;
                model = coreaccountService.GetFormDocument(model, 2, logged_in_user);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult DeleteFormDoc(int Id)
        {
            try
            {
                var data = CoreAccountsService.DeleteFormDoc(Id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ShowFormDoc(string file, string filepath)
        {
            try
            {

                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.DownloadFile(Common.GetDirectoryName(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }
        #endregion

        public DataTable GetOverAllProjectDetailsList(AnnualAccountsModel model)
        {
            //ToDate = ToDate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                context.Database.CommandTimeout = 1000;
                if ((model.FromDate != null || model.ToDate != null))
                {
                    model.FromDate = model.FromDate ?? new DateTime(2019, 4, 1);
                    model.ToDate = model.ToDate ?? DateTime.Now;
                    model.ToDate = model.ToDate.Value.AddDays(1).AddTicks(-2);
                }
                if ((model.Fin_Year == true && (model.FromDate == null || model.ToDate == null)) || model.Fin_Year == false)
                {
                    if (model.FinYear != null)
                    {
                        var Qry = context.tblFinYear.Where(m => m.FinYearId == model.FinYear).FirstOrDefault();
                        model.FromDate = Qry.StartDate;
                        model.ToDate = Qry.EndDate;
                    }
                    else
                    {
                        model.FromDate = new DateTime(2019, 4, 1);
                        model.ToDate = DateTime.Now;
                    }

                }
                model.PI = model.PI ?? 0;
                model.ProjId = model.ProjId ?? 0;
                model.Department = model.Department ?? "null";
                model.SchemeCodeId = model.SchemeCodeId ?? "null";
                model.AgencyNameId = model.AgencyNameId ?? "null";
                if (model.ProjType == 3 || model.ProjType == 6)
                    model.ProjType = model.ProjType ?? 0;
                else
                    model.ProjType = 0;
                if (model.ProjType == 4 || model.ProjType == 5)
                    model.ProjClass = model.ProjClass ?? 0;
                else
                    model.ProjClass = model.ProjClass ?? 0;
                if (model.ProjType == 1 || model.ProjType == 2)
                    model.SponCat = model.ProjType == 2 ? "2" : "1";
                else
                    model.SponCat = "0";
                var Frm = model.FromDate.Value.ToString("yyyy-MM-dd HH:mm");
                var Todate = model.ToDate.Value.ToString("yyyy-MM-dd HH:mm");
                SqlParameter[] ReportParam = new SqlParameter[10];
                ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
                ReportParam[0].Value = Frm;
                ReportParam[1] = new SqlParameter("@Date2", SqlDbType.DateTime);
                ReportParam[1].Value = Todate;
                ReportParam[2] = new SqlParameter("@ProjId", SqlDbType.Int);
                ReportParam[2].Value = model.ProjId;
                ReportParam[3] = new SqlParameter("@ProjType", SqlDbType.Int);
                ReportParam[3].Value = model.ProjType;
                ReportParam[4] = new SqlParameter("@PIName", SqlDbType.Int);
                ReportParam[4].Value = model.PI;
                ReportParam[5] = new SqlParameter("@Dep", SqlDbType.VarChar, 8000);
                ReportParam[5].Value = model.Department;
                ReportParam[6] = new SqlParameter("@ProjClass", SqlDbType.Int);
                ReportParam[6].Value = model.ProjClass;
                ReportParam[7] = new SqlParameter("@ProjCat", SqlDbType.VarChar, 8000);
                ReportParam[7].Value = model.SponCat;
                ReportParam[8] = new SqlParameter("@Scheme", SqlDbType.VarChar, 8000);
                ReportParam[8].Value = model.SchemeCodeId;
                ReportParam[9] = new SqlParameter("@Agency", SqlDbType.VarChar, 8000);
                ReportParam[9].Value = model.AgencyNameId;

                List<string> Param = new List<string>();
                //string[] Param;
                string Paramlist;

                if (model.ProjectNumber == true)
                    Param.Add("ProjectNumber");
                if (model.ProposalNumber == true)
                    Param.Add("ProposalNumber");
                if (model.ProjectType == true)
                    Param.Add("ProjectType");
                if (model.PIName_f == true)
                    Param.Add("PIName");
                if (model.SanctionOrderNumber == true)
                    Param.Add("SanctionOrderNumber");
                if (model.ProjectTitle == true)
                    Param.Add("ProjectTitle");
                if (model.StartDate == true)
                    Param.Add("StartDate");
                if (model.ClosureDate == true)
                    Param.Add("ClosureDate");
                if (model.ProjectClassification == true)
                    Param.Add("ProjectClassification");
                if (model.sanctionValue == true)
                    Param.Add("sanctionValue");
                if (model.SponReceipt == true)
                    Param.Add("SponReceipt");
                if (model.SponExp == true)
                    Param.Add("SponExp");
                if (model.OpeningBalance == true)
                    Param.Add("OpeningBalance");
                if (model.Receipts == true)
                    Param.Add("Receipts");
                if (model.TotalExpenditure == true)
                    Param.Add("TotalExpenditure");
                if (model.ClosingBalance == true)
                    Param.Add("ClosingBalance");
                if (model.NegativeApprove == true)
                    Param.Add("NegativeApprove");
                if (model.Commitment == true)
                    Param.Add("Commitment");
                if (model.Agency == true)
                    Param.Add("AgencyName");
                if (model.Scheme == true)
                    Param.Add("ProjectSchemeCode");

                List<string> Param1 = new List<string>();
                if (model.AlloHead != null)
                    Param1 = model.AlloHead.ToList();
                List<string> Param2 = new List<string>();
                if (model.ExpHead != null)
                    Param2 = model.ExpHead.ToList();
                string[] Param3 = Param.ToArray();
                //List<string> TotalParam = new List<string>();
                string[] TotalParam = Param3.Union(Param1).Union(Param2).ToArray();
                Paramlist = String.Join(", ", TotalParam);
                try
                {
                    DataTable dtColumns = new DataTable();
                    context.Database.ExecuteSqlCommand("exec OverallProjectDetails @Date, @Date2, @ProjId, @ProjType, @PIName, @Dep, @ProjClass, @ProjCat, @Scheme, @Agency", ReportParam);


                    using (var connection = Common.getConnection())
                    {
                        connection.Open();
                        //SqlHelper.ExecuteSP(connection, CommandType.StoredProcedure, "OverallProjectDetails", ReportParam);
                        var command = new System.Data.SqlClient.SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        if (Paramlist != null && Paramlist != "" && model.AllDetails != true)
                        {
                            if (model.Status == 1 && model.Balance == 1)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance > 0";
                            else if (model.Status == 1 && model.Balance == 2)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance < 0";
                            else if (model.Status == 1 && model.Balance == 3)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance = 0";
                            else if (model.Status == 2 && model.Balance == 2)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance < 0";
                            else if (model.Status == 2 && model.Balance == 3)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance = 0";
                            else if (model.Status == 2 && model.Balance == 1)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance > 0";
                            else if (model.Status == 1)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "'";
                            else if (model.Status == 2)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "'";
                            else if (model.Balance == 1)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where  ClosingBalance > 0";
                            else if (model.Balance == 2)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where  ClosingBalance < 0";
                            else if (model.Balance == 3)
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts where  ClosingBalance = 0";
                            else
                                command.CommandText = "select " + Paramlist + " from tblAnnualAccounts";
                        }
                        //command.CommandText = "select "+ Paramlist + " from tblAnnualAccounts";
                        else
                        {
                            if (model.Status == 1 && model.Balance == 1)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance > 0";
                            else if (model.Status == 1 && model.Balance == 2)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance < 0";
                            else if (model.Status == 1 && model.Balance == 3)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance = 0";
                            else if (model.Status == 2 && model.Balance == 2)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance < 0";
                            else if (model.Status == 2 && model.Balance == 3)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance = 0";
                            else if (model.Status == 2 && model.Balance == 1)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "' and ClosingBalance > 0";
                            else if (model.Status == 1)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate> '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "'";
                            else if (model.Status == 2)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where ClosureDate< '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm") + "'";
                            else if (model.Balance == 1)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing, 0 as Allo_Outsourcing, Allo_InternalTransfer, Allo_TransfertoOtherInstitute, Allo_PIFellowship,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing , InternalTransfer, ransfertoOtherInstitute, PIFellowship  ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where  ClosingBalance > 0";
                            else if (model.Balance == 2)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing   ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where  ClosingBalance < 0";
                            else if (model.Balance == 3)
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing   ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts where  ClosingBalance = 0";
                            else
                                command.CommandText = "SELECT ProjectNumber ,ProjectType ,SanctionOrderNumber ,ProjectTitle ,PIName ,StartDate,ClosureDate,sanctionValue ,SponReceipt ,SponExp,OpeningBalance ,Allo_Staff,Allo_Consumables,Allo_Contingencies,Allo_Travel ,Allo_Component,Allo_Overheads,Allo_Equipment ,Allo_Others  ,Allo_PublicationMonograph  ,Allo_WorkshopSymposium,Allo_ScientificSocialResponsibility ,Allo_IPFee,Allo_Miscellaneous,Allo_FieldTestingDemoTraining,Allo_Software ,Allo_ConsumablesTravelContingencies ,Allo_ConsumablesTravel ,Allo_Refundofunspentbalance ,Allo_Refundofunspentinterest ,Allo_FieldWork ,Allo_EquipmentandStudyMaterial ,Allo_PublicationofReport ,Allo_OverheadsContingencies,Allo_Consultants ,Allo_ConsumablesandAccessories ,Allo_ConsultancyFees  ,Allo_ConsumablesTravelContingenciesOtherCost ,Allo_UnspentBalance ,Allo_InterestRefund ,Allo_Distribution ,Allo_Outsourcing,Receipts,Staff ,Consumables,Contingencies ,Travel,Component ,Overheads,Equipment,Others,PublicationMonograph ,WorkshopSymposium ,ScientificSocialResponsibility ,IPFee ,Miscellaneous ,FieldTestingDemoTraining ,Software,ConsumablesTravelContingencies,ConsumablesTravel ,Refundofunspentbalance ,Refundofunspentinterest,FieldWork,EquipmentandStudyMaterial  ,PublicationofReport ,OverheadsContingencies ,Consultants ,ConsumablesandAccessories ,ConsultancyFees,ConsumablesTravelContingenciesOtherCost,UnspentBalance,InterestRefund ,Distribution ,Outsourcing   ,TotalExpenditure ,ClosingBalance ,NegativeApprove ,Commitment ,ProjectSchemeCode  ,AgencyName,ProposalNumber,ProjectClassification FROM tblAnnualAccounts";

                        }
                        //command.CommandText = "select * from tblAnnualAccounts";

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                        var dataset = new DataSet();
                        adapter.Fill(dataset);
                        dtColumns = dataset.Tables[0];
                        return dtColumns;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public FileStreamResult ProjectTransactionReport(int Id)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        ProjectTransactionModel model = new ProjectTransactionModel();
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        DataTable dtColumns1 = new DataTable();
                        model = ReportService.ProjectTransaction(Id);

                        var ws = wb.Worksheets.Add(model.ProjectNo);
                        dtResult = db.GetProjectTransactionPayment(Id);
                        dtColumns1 = db.GetProjectTransactionReceipt(Id);
                        ws.Cell(1, 1).Value = "Project Number";
                        ws.Cell(1, 2).Value = model.ProjectNo;
                        ws.Cell(2, 1).Value = "Title";
                        ws.Cell(2, 2).Value = model.Title;
                        ws.Cell(3, 1).Value = "PI";
                        ws.Cell(3, 2).Value = model.PI;
                        ws.Cell(4, 1).Value = "Total Sanction Value";
                        ws.Cell(4, 2).Value = model.SantionedValue;



                        var FromRange = ws.Range("A1:A1");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 12;
                        var ToRange = ws.Range("A2:A2");
                        ToRange.Style.Font.Bold = true;
                        ToRange.Style.Font.FontSize = 12;
                        var PI = ws.Range("A3:A3");
                        PI.Style.Font.Bold = true;
                        PI.Style.Font.FontSize = 12;
                        var SantionedValue = ws.Range("A4:A4");
                        SantionedValue.Style.Font.Bold = true;
                        SantionedValue.Style.Font.FontSize = 12;

                        ws.Cell(6, 3).Value = "Payment";
                        ws.Cell(6, 13).Value = "Receipt";

                        ws.Cell(7, 1).Value = "Reference Number";
                        ws.Cell(7, 2).Value = "Payee Name";
                        ws.Cell(7, 3).Value = "PO No";
                        ws.Cell(7, 4).Value = "Po Date";
                        ws.Cell(7, 5).Value = "Amount";
                        ws.Cell(7, 6).Value = "Commitment No";
                        ws.Cell(7, 7).Value = "Head Name";
                        ws.Cell(7, 8).Value = "Date";
                        ws.Cell(7, 9).Value = "IT TDS";
                        ws.Cell(7, 10).Value = "GST TDS";
                        ws.Cell(7, 11).Value = "Remarks";


                        ws.Cell(7, 14).Value = "Receipt Type";
                        ws.Cell(7, 15).Value = "Receipt Number";
                        ws.Cell(7, 16).Value = "Invoice Number";
                        ws.Cell(7, 17).Value = "Amount";
                        ws.Cell(7, 18).Value = "Agency Name";
                        ws.Cell(7, 19).Value = "Date";
                        ws.Cell(7, 20).Value = "Remarks";
                        int CopiRow = 8;

                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(CopiRow, 1).Value = row["ReferenceNumber"].ToString();
                            ws.Cell(CopiRow, 2).Value = row["PayeeName"].ToString();
                            ws.Cell(CopiRow, 3).Value = row["PONumber"].ToString();
                            ws.Cell(CopiRow, 4).Value = row["PODate"].ToString();
                            ws.Cell(CopiRow, 5).Value = row["PaidAmount"].ToString();
                            ws.Cell(CopiRow, 6).Value = row["CommitmentNo"].ToString();
                            ws.Cell(CopiRow, 7).Value = row["HeadName"].ToString();
                            ws.Cell(CopiRow, 8).Value = row["PaymentDate"].ToString();
                            ws.Cell(CopiRow, 9).Value = row["TDSAmount"].ToString();
                            ws.Cell(CopiRow, 10).Value = row["TDSGST"].ToString();
                            ws.Cell(CopiRow, 11).Value = row["Remarks"].ToString();
                            CopiRow++;
                        }
                        int CopiRow2 = 8;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws.Cell(CopiRow2, 14).Value = row["ReceiptType"].ToString();
                            ws.Cell(CopiRow2, 15).Value = row["ReceiptNumber"].ToString();
                            ws.Cell(CopiRow2, 16).Value = row["InvoiceNumber"].ToString();
                            ws.Cell(CopiRow2, 17).Value = row["Amount"].ToString();
                            ws.Cell(CopiRow2, 18).Value = row["AgencyName"].ToString();
                            ws.Cell(CopiRow2, 19).Value = row["Date"].ToString();
                            ws.Cell(CopiRow2, 20).Value = row["Remarks"].ToString();

                            CopiRow2++;
                        }
                        if (CopiRow2 > CopiRow)
                        {
                            ws.Cell(CopiRow2 + 3, 7).Value = "Total Balance";
                            ws.Cell(CopiRow2 + 3, 8).Value = model.TotalAmt;
                            ws.Cell(CopiRow2 + 4, 7).Value = "Available N.B";
                            ws.Cell(CopiRow2 + 4, 8).Value = model.NegBal;
                            ws.Cell(CopiRow2 + 5, 7).Value = "Net Balance";
                            ws.Cell(CopiRow2 + 5, 8).Value = model.NetBalance;

                        }
                        else
                        {
                            ws.Cell(CopiRow + 3, 7).Value = "Total Balance";
                            ws.Cell(CopiRow + 3, 8).Value = model.TotalAmt;
                            ws.Cell(CopiRow + 4, 7).Value = "Available N.B";
                            ws.Cell(CopiRow + 4, 8).Value = model.NegBal;
                            ws.Cell(CopiRow + 5, 7).Value = "Net Balance";
                            ws.Cell(CopiRow + 5, 8).Value = model.NetBalance;
                        }

                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ProjectTransaction.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult OverAllProjectDetails()
        {
            ViewBag.AnnAccProjectType = Common.GetCodeControlList("AnnAccProjectType");
            ViewBag.AnnAccProjStatus = Common.GetCodeControlList("AnnAccProjStatus");
            ViewBag.AnnAccProjBalance = Common.GetCodeControlList("AnnAccProjBalance");
            ViewBag.AnnAccProjClass = Common.GetCodeControlList("ProjectClassification");
            ViewBag.AnnAccFinYear = Common.GetFinYearList();
            ViewBag.AnnAccAllHead = Common.GetAlloHeadForAnnAccounts();
            ViewBag.AnnAccExpHead = Common.GetExpHeadForAnnAccounts();
            ViewBag.AnnAccDept = Common.GetDepartmentList();
            return View();
        }
        [HttpPost]
        public ActionResult OverAllProjectDetails(AnnualAccountsModel model)
        {
            ViewBag.AnnAccProjectType = Common.GetCodeControlList("AnnAccProjectType");
            ViewBag.AnnAccProjStatus = Common.GetCodeControlList("AnnAccProjStatus");
            ViewBag.AnnAccProjBalance = Common.GetCodeControlList("AnnAccProjBalance");
            ViewBag.AnnAccFinYear = Common.GetFinYearList();
            ViewBag.AnnAccAllHead = Common.GetAlloHeadForAnnAccounts();
            ViewBag.AnnAccExpHead = Common.GetExpHeadForAnnAccounts();
            ViewBag.AnnAccDept = Common.GetDepartmentList();
            //GetOverAllProjectDetails(model);
            DataTable dsTrasaction = GetOverAllProjectDetailsList(model);
            return coreaccountService.toSpreadSheet(dsTrasaction);
        }
        public FileStreamResult GSTR(string fdate, string tdate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetICSRB2B(fromdate, todate);
                        DataTable dtResult0 = new DataTable();
                        dtResult0 = db.GetInvoicenonTN(fromdate, todate);
                        var B2B = db.GetB2B(fromdate, todate);
                        var RCM = db.GetRCM(fromdate, todate);
                        var TDSRec = db.GetTDSRece(fromdate, todate);
                        var ws = wb.Worksheets.Add("FORM GSTR-3B");

                        ws.Cell(1, 1).Value = "FORM GSTR - 3B FOR THE MONTH OF" + String.Format("{0:MMMM}", fromdate);
                        ws.Cell(3, 1).Value = "3.1 Details of Outward Supplies and inward supplies liable to reverse charge";
                        ws.Cell(4, 1).Value = "Nature of Supplies";
                        ws.Cell(4, 2).Value = "Total Taxable value";
                        ws.Cell(4, 3).Value = "Integrated Tax";
                        ws.Cell(4, 4).Value = "Central Tax";
                        ws.Cell(4, 5).Value = "State / UT Tax";
                        ws.Cell(5, 1).Value = "(a) Outward taxable  supplies  (other than zero rated, nil rated and exempted)";
                        ws.Cell(5, 2).Value = B2B.Item1;
                        ws.Cell(5, 3).Value = B2B.Item2;
                        ws.Cell(5, 4).Value = B2B.Item3;
                        ws.Cell(5, 5).Value = B2B.Item4;
                        ws.Cell(6, 1).Value = "(b) Outward taxable  supplies  (zero rated )";
                        ws.Cell(6, 2).Value = B2B.Item5;
                        ws.Cell(6, 3).Value = "";
                        ws.Cell(6, 4).Value = "";
                        ws.Cell(6, 5).Value = "";
                        ws.Cell(7, 1).Value = "(c) Other outward supplies (Nil rated, exempted)";
                        ws.Cell(7, 2).Value = B2B.Item6;
                        ws.Cell(7, 3).Value = "";
                        ws.Cell(7, 4).Value = "";
                        ws.Cell(7, 5).Value = "";
                        ws.Cell(8, 1).Value = "(d) Inward supplies (liable to reverse charge)";
                        ws.Cell(8, 2).Value = "";
                        ws.Cell(8, 3).Value = "";
                        ws.Cell(8, 4).Value = RCM.Item1;
                        ws.Cell(8, 5).Value = RCM.Item2;
                        ws.Cell(9, 1).Value = "(e) Non-GST outward supplies";
                        ws.Cell(9, 2).Value = "";
                        ws.Cell(9, 3).Value = "";
                        ws.Cell(9, 4).Value = "";
                        ws.Cell(9, 5).Value = "";
                        ws.Cell(11, 1).Value = "3.2 Of the supplies shown in 3.1 (a)  above, details of inter-State supplies made to unregistered persons, composition taxable persons and UIN holders";
                        ws.Cell(12, 2).Value = "Place of Supply (State/UT)";
                        ws.Cell(12, 3).Value = "Total Taxable value";
                        ws.Cell(12, 4).Value = "Amount of Integrated Tax";
                        ws.Cell(13, 1).Value = "Supplies made to Unregistered Persons";
                        int Itclist = 14;
                        foreach (DataRow row in dtResult0.Rows)
                        {
                            ws.Cell(Itclist, 2).Value = row["PlaceOfSupply"].ToString();
                            ws.Cell(Itclist, 3).Value = row["TaxableValue"].ToString();
                            ws.Cell(Itclist, 4).Value = row["IGSTAmount"].ToString();
                            Itclist++;
                        }
                        ws.Cell(Itclist + 1, 1).Value = "4. Eligible ITC";
                        ws.Cell(Itclist + 2, 1).Value = "Details";
                        ws.Cell(Itclist + 2, 2).Value = "Integrated Tax";
                        ws.Cell(Itclist + 2, 3).Value = "Central Tax";
                        ws.Cell(Itclist + 2, 4).Value = "State / UT Tax";
                        ws.Cell(Itclist + 3, 1).Value = "(A) ITC Available (whether in full or part)";
                        ws.Cell(Itclist + 4, 1).Value = "(1)  Import of goods";
                        ws.Cell(Itclist + 5, 1).Value = "(2)  Import of services";
                        ws.Cell(Itclist + 6, 1).Value = "(3)  Inward supplies liable to reverse charge (other than 1 & 2 above)";
                        ws.Cell(Itclist + 6, 3).Value = RCM.Item3;
                        ws.Cell(Itclist + 6, 4).Value = RCM.Item4;
                        ws.Cell(Itclist + 7, 1).Value = "(4)  Inward supplies from ISD";
                        ws.Cell(Itclist + 8, 1).Value = "(5)  All other ITC";
                        ws.Cell(Itclist + 8, 2).Value = RCM.Item5;
                        ws.Cell(Itclist + 8, 3).Value = RCM.Item6;
                        ws.Cell(Itclist + 8, 4).Value = RCM.Item7;
                        ws.Cell(Itclist + 9, 1).Value = "(B) ITC Reversed";
                        ws.Cell(Itclist + 10, 1).Value = "(1) As per rules 42 & 43 of CGST Rules";
                        ws.Cell(Itclist + 11, 1).Value = "(2) Others";
                        ws.Cell(Itclist + 12, 1).Value = "(C) Net ITC Available (A) – (B)";
                        ws.Cell(Itclist + 12, 2).Value = RCM.Item5;
                        ws.Cell(Itclist + 12, 3).Value = RCM.Item6 + RCM.Item3;
                        ws.Cell(Itclist + 12, 4).Value = RCM.Item7 + RCM.Item4;
                        ws.Cell(Itclist + 13, 1).Value = "(D) Ineligible ITC";
                        ws.Cell(Itclist + 14, 1).Value = "(1) As per section 17(5)";
                        ws.Cell(Itclist + 15, 1).Value = "(2) Others";
                        ws.Cell(Itclist + 17, 1).Value = "5. Values of exempt, nil-rated and non-GST  inward supplies";
                        ws.Cell(Itclist + 18, 1).Value = "Nature of supplies";
                        ws.Cell(Itclist + 18, 2).Value = "Inter-State supplies";
                        ws.Cell(Itclist + 18, 3).Value = "Intra-State supplies";
                        ws.Cell(Itclist + 19, 1).Value = "From a supplier under composition scheme, Exempt and Nil rated supply";
                        ws.Cell(Itclist + 20, 1).Value = "Non GST supply";
                        ws.Cell(Itclist + 22, 1).Value = "6.1 Payment of tax";
                        ws.Cell(Itclist + 23, 1).Value = "Description";
                        ws.Cell(Itclist + 23, 2).Value = "Tax payable";
                        ws.Cell(Itclist + 23, 3).Value = "Integrated Tax";
                        ws.Cell(Itclist + 23, 4).Value = "Central Tax";
                        ws.Cell(Itclist + 23, 5).Value = "State / UT Tax";
                        ws.Cell(Itclist + 23, 6).Value = "Tax /Cess paid in cash";
                        ws.Cell(Itclist + 24, 1).Value = "Integrated Tax";
                        ws.Cell(Itclist + 24, 2).Value = B2B.Item1;
                        ws.Cell(Itclist + 24, 3).Value = RCM.Item5;
                        ws.Cell(Itclist + 24, 6).Value = B2B.Item1 - RCM.Item5;
                        ws.Cell(Itclist + 25, 1).Value = "Central Tax";
                        ws.Cell(Itclist + 25, 2).Value = B2B.Item5 + B2B.Item6;
                        ws.Cell(Itclist + 25, 4).Value = RCM.Item6 + RCM.Item3;
                        ws.Cell(Itclist + 25, 6).Value = (B2B.Item5 + B2B.Item6) - (RCM.Item6 + RCM.Item3);
                        ws.Cell(Itclist + 26, 1).Value = "State / UT Tax";
                        ws.Cell(Itclist + 26, 2).Value = B2B.Item5 + B2B.Item6;
                        ws.Cell(Itclist + 26, 5).Value = RCM.Item6 + RCM.Item3;
                        ws.Cell(Itclist + 26, 6).Value = (B2B.Item5 + B2B.Item6) - (RCM.Item6 + RCM.Item3);
                        ws.Cell(Itclist + 28, 1).Value = "TDS CREDITED IN THE ELECTRONIC CASH LEDGER";
                        ws.Cell(Itclist + 28, 2).Value = "Taxable Value";
                        ws.Cell(Itclist + 28, 3).Value = "Integrated Tax";
                        ws.Cell(Itclist + 28, 4).Value = "Central Tax";
                        ws.Cell(Itclist + 28, 5).Value = "State/UT Tax";
                        ws.Cell(Itclist + 28, 6).Value = "Total Tax Credit";
                        ws.Cell(Itclist + 29, 1).Value = "ICSR";
                        ws.Cell(Itclist + 29, 2).Value = TDSRec.Item1;
                        ws.Cell(Itclist + 29, 3).Value = TDSRec.Item2;
                        ws.Cell(Itclist + 29, 4).Value = TDSRec.Item3;
                        ws.Cell(Itclist + 29, 5).Value = TDSRec.Item4;
                        ws.Cell(Itclist + 29, 6).Value = TDSRec.Item2 + TDSRec.Item3 + TDSRec.Item4;
                        var ws1 = wb.Worksheets.Add("ICSR_b2b");
                        ws1.Cell(1, 1).Value = "GSTIN/UIN of Recipient";
                        ws1.Cell(1, 2).Value = "Receiver Name";
                        ws1.Cell(1, 3).Value = "Invoice Number";
                        ws1.Cell(1, 4).Value = "Invoice date";
                        ws1.Cell(1, 5).Value = "Invoice Value";
                        ws1.Cell(1, 6).Value = "Place Of Supply";
                        ws1.Cell(1, 7).Value = "Reverse Charge";
                        ws1.Cell(1, 8).Value = "Applicable % of Tax Rate";
                        ws1.Cell(1, 9).Value = "Invoice Type";
                        ws1.Cell(1, 10).Value = "E-Commerce GSTIN";
                        ws1.Cell(1, 11).Value = "Rate";
                        ws1.Cell(1, 12).Value = "Taxable Value";
                        ws1.Cell(1, 13).Value = "Cess Amount";
                        ws1.Cell(1, 14).Value = "SAC Code";
                        ws1.Cell(1, 15).Value = "CGST";
                        ws1.Cell(1, 16).Value = "SGST";
                        ws1.Cell(1, 17).Value = "IGST";
                        int Firstrow = 2;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws1.Cell(Firstrow, 1).Value = row["GSTIN"].ToString();
                            ws1.Cell(Firstrow, 2).Value = row["ReceiverName"].ToString();
                            ws1.Cell(Firstrow, 3).Value = row["InvoiceNumber"].ToString();
                            ws1.Cell(Firstrow, 4).Value = row["InvoiceDate"].ToString();
                            ws1.Cell(Firstrow, 5).Value = row["TotalInvoiceValue"].ToString();
                            ws1.Cell(Firstrow, 6).Value = row["PlaceOfSupply"].ToString();
                            ws1.Cell(Firstrow, 7).Value = row["ReverseCharge"].ToString();
                            ws1.Cell(Firstrow, 8).Value = row["ApplicableOfTaxRate"].ToString();
                            ws1.Cell(Firstrow, 9).Value = row["InvoiceType"].ToString();
                            ws1.Cell(Firstrow, 10).Value = row["ECommerceGSTIN"].ToString();
                            ws1.Cell(Firstrow, 11).Value = row["TaxRate"].ToString();
                            ws1.Cell(Firstrow, 12).Value = row["TaxableValue"].ToString();
                            ws1.Cell(Firstrow, 13).Value = row["CessAmount"].ToString();
                            ws1.Cell(Firstrow, 14).Value = row["SACcode"].ToString();
                            ws1.Cell(Firstrow, 15).Value = row["CGSTAmount"].ToString();
                            ws1.Cell(Firstrow, 16).Value = row["SGSTAmount"].ToString();
                            ws1.Cell(Firstrow, 17).Value = row["IGSTAmount"].ToString();
                            Firstrow++;
                        }
                        //sheet 3
                        DataTable dtResult1 = new DataTable();
                        dtResult1 = db.GetICSRB2CL(fromdate, todate);
                        var ws2 = wb.Worksheets.Add("ICSR_b2cl");
                        ws2.Cell(1, 1).Value = "Invoice Number";
                        ws2.Cell(1, 2).Value = "Invoice date";
                        ws2.Cell(1, 3).Value = "Invoice Value";
                        ws2.Cell(1, 4).Value = "Place Of Supply";
                        ws2.Cell(1, 5).Value = "Applicable % of Tax Rate";
                        ws2.Cell(1, 6).Value = "Rate";
                        ws2.Cell(1, 7).Value = "Taxable Value";
                        ws2.Cell(1, 8).Value = "Cess Amount";
                        ws2.Cell(1, 9).Value = "E-Commerce GSTIN";
                        ws2.Cell(1, 10).Value = "Sale from Bonded WH";
                        ws2.Cell(1, 11).Value = "SAC Code";
                        ws2.Cell(1, 12).Value = "IGST";
                        int Secondrow = 2;
                        foreach (DataRow row in dtResult1.Rows)
                        {
                            ws2.Cell(Secondrow, 1).Value = row["InvoiceNumber"].ToString();
                            ws2.Cell(Secondrow, 2).Value = row["InvoiceDate"].ToString();
                            ws2.Cell(Secondrow, 3).Value = row["TotalInvoiceValue"].ToString();
                            ws2.Cell(Secondrow, 4).Value = row["PlaceOfSupply"].ToString();
                            ws2.Cell(Secondrow, 5).Value = "";
                            ws2.Cell(Secondrow, 6).Value = row["TaxRate"].ToString();
                            ws2.Cell(Secondrow, 7).Value = row["TaxableValue"].ToString();
                            ws2.Cell(Secondrow, 8).Value = "";
                            ws2.Cell(Secondrow, 9).Value = "";
                            ws2.Cell(Secondrow, 10).Value = "N";
                            ws2.Cell(Secondrow, 11).Value = row["SACcode"].ToString();
                            ws2.Cell(Secondrow, 12).Value = row["IGSTAmount"].ToString();
                            Secondrow++;
                        }

                        //sheet 4
                        DataTable dtResult2 = new DataTable();
                        dtResult2 = db.GetICSRB2CS(fromdate, todate);
                        var ws3 = wb.Worksheets.Add("ICSR_b2cs");
                        ws3.Cell(1, 1).Value = "Type";
                        ws3.Cell(1, 2).Value = "Place Of Supply";
                        ws3.Cell(1, 3).Value = "Applicable % of Tax Rate";
                        ws3.Cell(1, 4).Value = "Rate";
                        ws3.Cell(1, 5).Value = "Taxable Value";
                        ws3.Cell(1, 6).Value = "Cess Amount";
                        ws3.Cell(1, 7).Value = "E-Commerce GSTIN";
                        ws3.Cell(1, 8).Value = "SAC Code";
                        ws3.Cell(1, 9).Value = "CGST";
                        ws3.Cell(1, 10).Value = "SGST";
                        ws3.Cell(1, 11).Value = "IGST";
                        ws3.Cell(1, 12).Value = "Invoice value";
                        ws3.Cell(1, 13).Value = "Invoice No";
                        int Thirdrow = 2;
                        foreach (DataRow row in dtResult2.Rows)
                        {
                            ws3.Cell(Thirdrow, 1).Value = row["Type"].ToString();
                            ws3.Cell(Thirdrow, 2).Value = row["PlaceOfSupply"].ToString();
                            ws3.Cell(Thirdrow, 3).Value = "";
                            ws3.Cell(Thirdrow, 4).Value = row["TaxRate"].ToString();
                            ws3.Cell(Thirdrow, 5).Value = row["TaxableValue"].ToString();
                            ws3.Cell(Thirdrow, 6).Value = "";
                            ws3.Cell(Thirdrow, 7).Value = "";
                            ws3.Cell(Thirdrow, 8).Value = row["SACcode"].ToString();
                            ws3.Cell(Thirdrow, 9).Value = row["CGSTAmount"].ToString();
                            ws3.Cell(Thirdrow, 10).Value = row["SGSTAmount"].ToString();
                            ws3.Cell(Thirdrow, 11).Value = row["IGSTAmount"].ToString();
                            ws3.Cell(Thirdrow, 12).Value = row["TotalInvoiceValue"].ToString();
                            ws3.Cell(Thirdrow, 13).Value = row["InvoiceNumber"].ToString();
                            Thirdrow++;
                        }
                        //sheet 5
                        DataTable dtResult3 = new DataTable();
                        dtResult3 = db.GetExport(fromdate, todate);
                        var ws4 = wb.Worksheets.Add("EXPORT");
                        ws4.Cell(1, 1).Value = "Export Type";
                        ws4.Cell(1, 2).Value = "Invoice Number";
                        ws4.Cell(1, 3).Value = "Invoice date";
                        ws4.Cell(1, 4).Value = "Invoice Value";
                        ws4.Cell(1, 5).Value = "Port Code";
                        ws4.Cell(1, 6).Value = "Shipping Bill Number";
                        ws4.Cell(1, 7).Value = "Shipping Bill Date";
                        ws4.Cell(1, 8).Value = "Applicable % of Tax Rate";
                        ws4.Cell(1, 9).Value = "Rate";
                        ws4.Cell(1, 10).Value = "Taxable Value";
                        ws4.Cell(1, 11).Value = "Cess Amount";
                        ws4.Cell(1, 12).Value = "SAC Code";
                        int Fouthrow = 2;
                        foreach (DataRow row in dtResult3.Rows)
                        {
                            ws4.Cell(Fouthrow, 1).Value = row["ExportType"].ToString();
                            ws4.Cell(Fouthrow, 2).Value = row["InvoiceNumber"].ToString();
                            ws4.Cell(Fouthrow, 3).Value = row["InvoiceDate"].ToString();
                            ws4.Cell(Fouthrow, 4).Value = row["TotalInvoiceValue"].ToString();
                            ws4.Cell(Fouthrow, 5).Value = "";
                            ws4.Cell(Fouthrow, 6).Value = "";
                            ws4.Cell(Fouthrow, 7).Value = "";
                            ws4.Cell(Fouthrow, 8).Value = "";
                            ws4.Cell(Fouthrow, 9).Value = "0";
                            ws4.Cell(Fouthrow, 10).Value = row["TaxableValue"].ToString();
                            ws4.Cell(Fouthrow, 11).Value = "";
                            ws4.Cell(Fouthrow, 12).Value = row["SACCode"].ToString();
                            Fouthrow++;
                        }
                        //sheet 6
                        DataTable dtResult4 = new DataTable();
                        dtResult4 = db.GetExempted(fromdate, todate);
                        var ws5 = wb.Worksheets.Add("EXEMPTED");
                        ws5.Cell(1, 1).Value = "Description";
                        ws5.Cell(1, 2).Value = "Nil Rated Supplies";
                        ws5.Cell(1, 3).Value = "Exempted(other than nil rated/non GST supply)";
                        ws5.Cell(1, 4).Value = "Non-GST Supplies";
                        ws5.Cell(1, 5).Value = "SAC code";
                        ws5.Cell(1, 6).Value = "Invoice No.";
                        ws5.Cell(1, 7).Value = "Invoice Date";
                        int Fifthrow = 2;
                        foreach (DataRow row in dtResult4.Rows)
                        {
                            ws5.Cell(Fifthrow, 1).Value = row["Description"].ToString();
                            ws5.Cell(Fifthrow, 2).Value = "";
                            ws5.Cell(Fifthrow, 3).Value = row["Exempted"].ToString();
                            ws5.Cell(Fifthrow, 4).Value = "";
                            ws5.Cell(Fifthrow, 5).Value = row["SACcode"].ToString();
                            ws5.Cell(Fifthrow, 6).Value = row["InvoiceNumber"].ToString();
                            ws5.Cell(Fifthrow, 7).Value = row["InvoiceDate"].ToString();
                            Fifthrow++;
                        }
                        //sheet 7
                        DataTable dtResult5 = new DataTable();
                        dtResult5 = db.GetCNreg(fromdate, todate);
                        var ws6 = wb.Worksheets.Add("CNreg");
                        ws6.Cell(1, 1).Value = "GSTIN/UIN of Recipient";
                        ws6.Cell(1, 2).Value = "Receiver Name";
                        ws6.Cell(1, 3).Value = "Invoice/Advance Receipt Number";
                        ws6.Cell(1, 4).Value = "Invoice/Advance Receipt date";
                        ws6.Cell(1, 5).Value = "Note/Refund Voucher Number";
                        ws6.Cell(1, 6).Value = "Note/Refund Voucher date";
                        ws6.Cell(1, 7).Value = "Document Type";
                        ws6.Cell(1, 8).Value = "Place Of Supply";
                        ws6.Cell(1, 9).Value = "Note/Refund Voucher Value";
                        ws6.Cell(1, 10).Value = "Applicable % of Tax Rate";
                        ws6.Cell(1, 11).Value = "Rate";
                        ws6.Cell(1, 12).Value = "Taxable Value";
                        ws6.Cell(1, 13).Value = "Cess Amount";
                        ws6.Cell(1, 14).Value = "Pre GST";
                        ws6.Cell(1, 15).Value = "CGST";
                        ws6.Cell(1, 16).Value = "SGST";
                        ws6.Cell(1, 17).Value = "IGST";
                        int Sixrow = 2;
                        foreach (DataRow row in dtResult5.Rows)
                        {
                            ws6.Cell(Sixrow, 1).Value = row["GSTIN"].ToString();
                            ws6.Cell(Sixrow, 2).Value = row["ReceiverName"].ToString();
                            ws6.Cell(Sixrow, 3).Value = row["InvoiceNumber"].ToString();
                            ws6.Cell(Sixrow, 4).Value = row["InvoiceDate"].ToString();
                            ws6.Cell(Sixrow, 5).Value = row["CreditNoteNumber"].ToString();
                            ws6.Cell(Sixrow, 6).Value = row["CreditNoteDate"].ToString();
                            ws6.Cell(Sixrow, 7).Value = "C";
                            ws6.Cell(Sixrow, 8).Value = row["PlaceOfSupply"].ToString();
                            ws6.Cell(Sixrow, 9).Value = row["RefundVoucherValue"].ToString();
                            ws6.Cell(Sixrow, 10).Value = "";
                            ws6.Cell(Sixrow, 11).Value = row["Rate"].ToString();
                            ws6.Cell(Sixrow, 12).Value = row["Taxablevalue"].ToString();
                            ws6.Cell(Sixrow, 13).Value = "";
                            ws6.Cell(Sixrow, 14).Value = "N";
                            ws6.Cell(Sixrow, 15).Value = row["CGST"].ToString();
                            ws6.Cell(Sixrow, 16).Value = row["SGST"].ToString();
                            ws6.Cell(Sixrow, 17).Value = row["IGST"].ToString();
                            Sixrow++;
                        }
                        //sheet 8
                        DataTable dtResult6 = new DataTable();
                        dtResult6 = db.GetCNunreg(fromdate, todate);
                        var ws7 = wb.Worksheets.Add("CNunreg");
                        ws7.Cell(1, 1).Value = "UR Type";
                        ws7.Cell(1, 2).Value = "Note/Refund Voucher Number";
                        ws7.Cell(1, 3).Value = "Note/Refund Voucher date";
                        ws7.Cell(1, 4).Value = "Document Type";
                        ws7.Cell(1, 5).Value = "Invoice/Advance Receipt Number";
                        ws7.Cell(1, 6).Value = "Invoice/Advance Receipt date";
                        ws7.Cell(1, 7).Value = "Place Of Supply";
                        ws7.Cell(1, 8).Value = "Note/Refund Voucher Value";
                        ws7.Cell(1, 9).Value = "Applicable % of Tax Rate";
                        ws7.Cell(1, 10).Value = "Rate";
                        ws7.Cell(1, 11).Value = "Taxable Value";
                        ws7.Cell(1, 12).Value = "Cess Amount";
                        ws7.Cell(1, 13).Value = "Pre GST";
                        ws7.Cell(1, 14).Value = "CGST";
                        ws7.Cell(1, 15).Value = "SGST";
                        ws7.Cell(1, 16).Value = "IGST";
                        int Sevenrow = 2;
                        foreach (DataRow row in dtResult6.Rows)
                        {
                            ws7.Cell(Sevenrow, 1).Value = row["URType"].ToString();
                            ws7.Cell(Sevenrow, 2).Value = row["CreditNoteNumber"].ToString();
                            ws7.Cell(Sevenrow, 3).Value = row["CreditNoteDate"].ToString();
                            ws7.Cell(Sevenrow, 4).Value = row["DocumentType"].ToString();
                            ws7.Cell(Sevenrow, 5).Value = row["InvoiceNumber"].ToString();
                            ws7.Cell(Sevenrow, 6).Value = row["InvoiceDate"].ToString();
                            ws7.Cell(Sevenrow, 7).Value = row["PlaceOfSupply"].ToString();
                            ws7.Cell(Sevenrow, 8).Value = row["RefundVoucherValue"].ToString();
                            ws7.Cell(Sevenrow, 9).Value = "";
                            ws7.Cell(Sevenrow, 10).Value = row["Rate"].ToString();
                            ws7.Cell(Sevenrow, 11).Value = row["Taxablevalue"].ToString();
                            ws7.Cell(Sevenrow, 12).Value = "";
                            ws7.Cell(Sevenrow, 13).Value = "N";
                            ws7.Cell(Sevenrow, 14).Value = row["CGST"].ToString();
                            ws7.Cell(Sevenrow, 15).Value = row["SGST"].ToString();
                            ws7.Cell(Sevenrow, 16).Value = row["IGST"].ToString();
                            Sevenrow++;
                        }
                        //sheet 9
                        DataTable dtResult7 = new DataTable();
                        dtResult7 = db.GetSACfinal(fromdate, todate);
                        var ws8 = wb.Worksheets.Add("SAC_FINAL");
                        ws8.Cell(1, 1).Value = "HSN";
                        ws8.Cell(1, 2).Value = "Description";
                        ws8.Cell(1, 3).Value = "UQC";
                        ws8.Cell(1, 4).Value = "Total Quantity";
                        ws8.Cell(1, 5).Value = "Total Value";
                        ws8.Cell(1, 6).Value = "Taxable Value";
                        ws8.Cell(1, 7).Value = "Integrated Tax Amount";
                        ws8.Cell(1, 8).Value = "Central Tax Amount";
                        ws8.Cell(1, 9).Value = "State/UT Tax Amount";
                        ws8.Cell(1, 10).Value = "Cess Amount";
                        int Eightrow = 2;
                        foreach (DataRow row in dtResult7.Rows)
                        {
                            ws8.Cell(Eightrow, 1).Value = row["HSN"].ToString();
                            ws8.Cell(Eightrow, 2).Value = row["Description"].ToString();
                            ws8.Cell(Eightrow, 3).Value = "OTH-OTHERS";
                            ws8.Cell(Eightrow, 4).Value = row["TotalQuantity"].ToString();
                            ws8.Cell(Eightrow, 5).Value = row["TotalValue"].ToString();
                            ws8.Cell(Eightrow, 6).Value = row["TaxableValue"].ToString();
                            ws8.Cell(Eightrow, 7).Value = row["IntegratedTaxAmount"].ToString();
                            ws8.Cell(Eightrow, 8).Value = row["CentralTaxAmount"].ToString();
                            ws8.Cell(Eightrow, 9).Value = row["StateTaxAmount"].ToString();
                            ws8.Cell(Eightrow, 10).Value = "";
                            Eightrow++;
                        }
                        //sheet 9
                        var InvoiceDoc = Common.GetInvoiceDoc(fromdate, todate);
                        var CreditnoteDoc = Common.GetCreditnoteDoc(fromdate, todate);
                        var ws9 = wb.Worksheets.Add("Doc");
                        ws9.Cell(1, 1).Value = "Nature of Document";
                        ws9.Cell(1, 2).Value = "Sr. No. From";
                        ws9.Cell(1, 3).Value = "Sr. No. To";
                        ws9.Cell(1, 4).Value = "Total Number";
                        ws9.Cell(1, 5).Value = "Cancelled";
                        ws9.Cell(1, 6).Value = "Total";
                        int Ninerow = 2;
                        ws9.Cell(Ninerow, 1).Value = "Invoices for outward supply";
                        ws9.Cell(Ninerow, 2).Value = InvoiceDoc.Item1;
                        ws9.Cell(Ninerow, 3).Value = InvoiceDoc.Item2;
                        ws9.Cell(Ninerow, 4).Value = InvoiceDoc.Item3;
                        ws9.Cell(Ninerow, 5).Value = InvoiceDoc.Item4;
                        ws9.Cell(Ninerow, 6).Value = InvoiceDoc.Item5;
                        ws9.Cell(Ninerow + 1, 1).Value = "Credit Note";
                        ws9.Cell(Ninerow + 1, 2).Value = CreditnoteDoc.Item1;
                        ws9.Cell(Ninerow + 1, 3).Value = CreditnoteDoc.Item2;
                        ws9.Cell(Ninerow + 1, 4).Value = CreditnoteDoc.Item3;
                        ws9.Cell(Ninerow + 1, 5).Value = CreditnoteDoc.Item4;
                        ws9.Cell(Ninerow + 1, 6).Value = CreditnoteDoc.Item5;

                        //sheet 10
                        DataTable dtResult8 = new DataTable();
                        dtResult8 = db.GetEligibleITC(fromdate, todate);
                        var ws10 = wb.Worksheets.Add("Eligible ITC");
                        ws10.Cell(1, 1).Value = "BR NUMBER";
                        ws10.Cell(1, 2).Value = "GSTIN";
                        ws10.Cell(1, 3).Value = "Name of Supplier";
                        ws10.Cell(1, 4).Value = "Invoice No.";
                        ws10.Cell(1, 5).Value = "Invoice Date";
                        ws10.Cell(1, 6).Value = "HSN/SAC";
                        ws10.Cell(1, 7).Value = "Taxable value";
                        ws10.Cell(1, 8).Value = "Rate";
                        ws10.Cell(1, 9).Value = "CGST";
                        ws10.Cell(1, 10).Value = "SGST";
                        ws10.Cell(1, 11).Value = "IGST";

                        int Tenrow = 2;
                        foreach (DataRow row in dtResult8.Rows)
                        {
                            ws10.Cell(Tenrow, 1).Value = "";
                            ws10.Cell(Tenrow, 2).Value = row["GSTIN"].ToString();
                            ws10.Cell(Tenrow, 3).Value = row["VendorName"].ToString();
                            ws10.Cell(Tenrow, 4).Value = row["InvoiceNumber"].ToString();
                            ws10.Cell(Tenrow, 5).Value = row["InvoiceDate"].ToString();
                            ws10.Cell(Tenrow, 6).Value = row["HSNSAC"].ToString();
                            ws10.Cell(Tenrow, 7).Value = row["Amount"].ToString();
                            ws10.Cell(Tenrow, 8).Value = row["TaxPercentage"].ToString();
                            ws10.Cell(Tenrow, 9).Value = row["CGST"].ToString();
                            ws10.Cell(Tenrow, 10).Value = row["SGST"].ToString();
                            ws10.Cell(Tenrow, 11).Value = row["IGST"].ToString();

                            Tenrow++;
                        }

                        //sheet 11
                        DataTable dtResult9 = new DataTable();
                        dtResult9 = db.GetRCMReceivable(fromdate, todate);
                        var ws11 = wb.Worksheets.Add("RCMReceivable");
                        ws11.Cell(1, 1).Value = "Date";
                        ws11.Cell(1, 2).Value = "Consignee/Buyer";
                        ws11.Cell(1, 3).Value = "Voucher Type";
                        ws11.Cell(1, 4).Value = "Vch No.";
                        ws11.Cell(1, 5).Value = "GSTIN/UIN";
                        ws11.Cell(1, 6).Value = "Value";
                        ws11.Cell(1, 7).Value = "Gross Total";
                        ws11.Cell(1, 8).Value = "CGST @ 9%";
                        ws11.Cell(1, 9).Value = "SGST @ 9%";
                        int Elevenrow = 2;
                        foreach (DataRow row in dtResult9.Rows)
                        {
                            ws11.Cell(Elevenrow, 1).Value = row["PostedDate"].ToString();
                            ws11.Cell(Elevenrow, 2).Value = row["VendorName"].ToString();
                            ws11.Cell(Elevenrow, 3).Value = "Finance & Accounts";
                            ws11.Cell(Elevenrow, 4).Value = row["RefNo"].ToString();
                            ws11.Cell(Elevenrow, 5).Value = "NA";
                            ws11.Cell(Elevenrow, 6).Value = row["Amount"].ToString();
                            ws11.Cell(Elevenrow, 7).Value = row["TotalValue"].ToString();
                            ws11.Cell(Elevenrow, 8).Value = row["CGST"].ToString();
                            ws11.Cell(Elevenrow, 9).Value = row["SGST"].ToString();
                            Elevenrow++;
                        }

                        //sheet 11
                        DataTable dtResult10 = new DataTable();
                        dtResult10 = db.GetRCMPayable(fromdate, todate);
                        var ws12 = wb.Worksheets.Add("RCMPayable");
                        ws12.Cell(1, 1).Value = "Date";
                        ws12.Cell(1, 2).Value = "Consignee/Buyer";
                        ws12.Cell(1, 3).Value = "Voucher Type";
                        ws12.Cell(1, 4).Value = "Vch No.";
                        ws12.Cell(1, 5).Value = "GSTIN/UIN";
                        ws12.Cell(1, 6).Value = "Value";
                        ws12.Cell(1, 7).Value = "Gross Total";
                        ws12.Cell(1, 8).Value = "CGST @ 9%";
                        ws12.Cell(1, 9).Value = "SGST @ 9%";
                        int Tewvrow = 2;
                        foreach (DataRow row in dtResult10.Rows)
                        {
                            ws12.Cell(Tewvrow, 1).Value = row["PostedDate"].ToString();
                            ws12.Cell(Tewvrow, 2).Value = row["VendorName"].ToString();
                            ws12.Cell(Tewvrow, 3).Value = "Finance & Accounts";
                            ws12.Cell(Tewvrow, 4).Value = row["RefNo"].ToString();
                            ws12.Cell(Tewvrow, 5).Value = "NA";
                            ws12.Cell(Tewvrow, 6).Value = row["Amount"].ToString();
                            ws12.Cell(Tewvrow, 7).Value = row["TotalValue"].ToString();
                            ws12.Cell(Tewvrow, 8).Value = row["CGST"].ToString();
                            ws12.Cell(Tewvrow, 9).Value = row["SGST"].ToString();
                            Tewvrow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=GSTR.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public FileStreamResult ProjectSchemeBalance(string tdate)
        {
            try
            {

                var ToDate = Convert.ToDateTime(tdate);
                ToDate = ToDate.AddDays(1).AddTicks(-2);
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsTrasaction = db.GetProjectSchemeBalance(ToDate);
                return toSpreadSheets(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ProjectScheme(string fdate, string tdate)
        {
            try
            {
                var fromdate = Convert.ToDateTime(fdate);
                var ToDate = Convert.ToDateTime(tdate);
                ToDate = ToDate.AddDays(1).AddTicks(-2);
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsTrasaction = db.GetProjectScheme(fromdate, ToDate);
                return toSpreadSheets(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult YearwiseFinancialReport(int ProjectId, int Year)
        {

            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    ReportService reportservice = new ReportService();
                    ProjectService projectservice = new ProjectService();
                    ProjectSummaryDetailModel model = new ProjectSummaryDetailModel();
                    ProvisionalStatementReportModel model1 = new ProvisionalStatementReportModel();
                    ProjectSummaryModel model2 = new ProjectSummaryModel();
                    var Datelist = Common.GetProjectExpYear(ProjectId);
                    var date = Datelist.Where(m => m.Id == Year).FirstOrDefault();
                    model = reportservice.getProjectSummaryDetails(ProjectId, date.FromDate, date.ToDate, Year);
                    model1 = ReportService.GetProvisionalStatement(Convert.ToString(date.FromDate), Convert.ToString(date.ToDate), ProjectId);
                    model2 = projectservice.getProjectSummary(ProjectId);
                    DataSet dataset = new DataSet();
                    DataTable dtColumns = new DataTable();
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.HeadWise);
                    dtColumns = JsonConvert.DeserializeObject<DataTable>(json);

                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("Financial Report");
                    ws.Cell(2, 1).Value = "PROJECT NUMBER";
                    ws.Cell(2, 2).Value = model2.ProjectNo;
                    ws.Cell(3, 1).Value = "PROJECT TYPE";
                    ws.Cell(3, 2).Value = model2.ProjectType;
                    ws.Cell(4, 1).Value = "PROJECT TITLE";
                    ws.Cell(4, 2).Value = model2.ProjectTittle;
                    ws.Cell(5, 1).Value = "PI NAME";
                    ws.Cell(5, 2).Value = model2.PIname;
                    ws.Cell(6, 1).Value = "START DATE";
                    ws.Cell(6, 2).Value = model2.StartDate;
                    ws.Cell(7, 1).Value = "Close DATE";
                    ws.Cell(7, 2).Value = model2.CloseDate;


                    ws.Cell(10, 1).Value = "BUDGET HEADS";
                    ws.Cell(10, 2).Value = "BUDGET ALLOCATION";
                    ws.Cell(10, 3).Value = "EXPENDITURE";
                    ws.Cell(10, 4).Value = "COMMITMENT";
                    ws.Cell(10, 5).Value = "EXPENDITURE INCLUSIVE OF COMMITMENTS";
                    ws.Cell(10, 6).Value = "HEAD - WISE BALANCE FOR THE YEAR";
                    int firstrow = 11;
                    foreach (DataRow row in dtColumns.Rows)
                    {
                        ws.Cell(firstrow, 1).Value = row["AllocationHeadName"].ToString();
                        ws.Cell(firstrow, 2).Value = row["Amount"].ToString();
                        ws.Cell(firstrow, 3).Value = row["Expenditure"].ToString();
                        ws.Cell(firstrow, 4).Value = row["BalanceAmount"].ToString();
                        ws.Cell(firstrow, 5).Value = row["Total"].ToString();
                        ws.Cell(firstrow, 6).Value = row["Available"];
                        firstrow++;
                    }
                    ws.Cell(firstrow + 2, 1).Value = "(I)  OPENEING BALANCE";
                    ws.Cell(firstrow + 2, 2).Value = model1.OB;
                    ws.Cell(firstrow + 3, 1).Value = "(II)  GRANTS RECEIVED FOR THE YEAR";
                    ws.Cell(firstrow + 3, 2).Value = model1.Receipt;
                    ws.Cell(firstrow + 4, 1).Value = "(III)  EXPENDITURE TILL DATE";
                    ws.Cell(firstrow + 4, 2).Value = model2.TotalExpenditure;
                    ws.Cell(firstrow + 5, 1).Value = "(IV) COMMITMENT TILL DATE";
                    ws.Cell(firstrow + 5, 2).Value = model2.PreviousCommitment;
                    ws.Cell(firstrow + 6, 1).Value = "(V) EXPENDITURE INCLUSIVE OF COMMITMENTS";
                    ws.Cell(firstrow + 6, 2).Value = model2.TotalExpenditure + model2.PreviousCommitment;
                    ws.Cell(firstrow + 7, 1).Value = "(VI)  CLOSING BALANCE AS ON DATE";
                    ws.Cell(firstrow + 7, 2).Value = (model1.OB + model1.Receipt) - (model2.TotalExpenditure + model2.PreviousCommitment);
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=YearwiseFinancialReport.xls");
                return File(workStream, fileType);
            }
        }
        public FileStreamResult DaywiseFinancialReport(int ProjectId, string tdate)
        {

            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            var todate = Convert.ToDateTime(tdate);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    ReportService reportservice = new ReportService();
                    ProjectService projectservice = new ProjectService();
                    ProjectSummaryDetailModel model = new ProjectSummaryDetailModel();


                    model = reportservice.getProjectSummaryDetails(ProjectId, todate);

                    DataSet dataset = new DataSet();
                    DataTable dtColumns = new DataTable();
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(model.HeadWise);
                    dtColumns = JsonConvert.DeserializeObject<DataTable>(json);

                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("Financial Report");
                    ws.Cell(2, 1).Value = "PROJECT NUMBER";
                    ws.Cell(2, 2).Value = model.PrjSummary.ProjectNo;
                    ws.Cell(3, 1).Value = "PROJECT TYPE";
                    ws.Cell(3, 2).Value = model.PrjSummary.ProjectType;
                    ws.Cell(4, 1).Value = "PROJECT TITLE";
                    ws.Cell(4, 2).Value = model.PrjSummary.ProjectTittle;
                    ws.Cell(5, 1).Value = "PI NAME";
                    ws.Cell(5, 2).Value = model.PrjSummary.PIname;
                    ws.Cell(6, 1).Value = "START DATE";
                    ws.Cell(6, 2).Value = model.PrjSummary.StartDate;
                    ws.Cell(7, 1).Value = "Close DATE";
                    ws.Cell(7, 2).Value = model.PrjSummary.CloseDate;


                    ws.Cell(10, 1).Value = "BUDGET HEADS";
                    ws.Cell(10, 2).Value = "BUDGET ALLOCATION";
                    ws.Cell(10, 3).Value = "EXPENDITURE";
                    ws.Cell(10, 4).Value = "COMMITMENT";
                    ws.Cell(10, 5).Value = "EXPENDITURE INCLUSIVE OF COMMITMENTS";
                    ws.Cell(10, 6).Value = "HEAD - WISE BALANCE";
                    int firstrow = 11;
                    foreach (DataRow row in dtColumns.Rows)
                    {
                        ws.Cell(firstrow, 1).Value = row["AllocationHeadName"].ToString();
                        ws.Cell(firstrow, 2).Value = row["Amount"].ToString();
                        ws.Cell(firstrow, 3).Value = row["Expenditure"].ToString();
                        ws.Cell(firstrow, 4).Value = row["BalanceAmount"].ToString();
                        ws.Cell(firstrow, 5).Value = row["Total"].ToString();
                        ws.Cell(firstrow, 6).Value = row["Available"];
                        firstrow++;
                    }

                    ws.Cell(firstrow + 3, 1).Value = "(I)   TOTAL GRANTS RECEIVED";
                    ws.Cell(firstrow + 3, 2).Value = model.PrjSummary.TotalGrantReceived;
                    ws.Cell(firstrow + 4, 1).Value = "(II)  EXPENDITURE TILL DATE";
                    ws.Cell(firstrow + 4, 2).Value = model.PrjSummary.TotalExpenditure;
                    ws.Cell(firstrow + 5, 1).Value = "(III) COMMITMENT TILL DATE";
                    ws.Cell(firstrow + 5, 2).Value = model.PrjSummary.PreviousCommitment;
                    ws.Cell(firstrow + 6, 1).Value = "(IV) EXPENDITURE INCLUSIVE OF COMMITMENTS";
                    ws.Cell(firstrow + 6, 2).Value = model.PrjSummary.TotalExpenditure + model.PrjSummary.PreviousCommitment;
                    ws.Cell(firstrow + 7, 1).Value = "(V)  CLOSING BALANCE AS ON DATE";
                    ws.Cell(firstrow + 7, 2).Value = (model.PrjSummary.TotalGrantReceived) - (model.PrjSummary.TotalExpenditure + model.PrjSummary.PreviousCommitment);
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=DaywiseFinancialReport.xls");
                return File(workStream, fileType);
            }
        }

        public FileStreamResult NIRFReport(string fdate, string tdate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetReceiptsSummaryNIRF(fromdate, todate);
                        DataTable dtResult1 = new DataTable();
                        dtResult1 = db.GetReceiptsSummaryTotal(fromdate, todate);
                        DataTable dtResult2 = new DataTable();
                        dtResult2 = db.GetConsReceiptNIRF(fromdate, todate);
                        DataTable dtResult3 = new DataTable();
                        dtResult3 = db.GetSponReceiptNIRF(fromdate, todate);
                        DataTable dtResult4 = new DataTable();
                        dtResult4 = db.GetCapex(fromdate, todate);
                        DataTable dtResult5 = new DataTable();
                        dtResult5 = db.GetCapexList(fromdate, todate);
                        DataTable dtResult6 = new DataTable();
                        dtResult6 = db.GetOpex(fromdate, todate);
                        DataTable dtResult7 = new DataTable();
                        dtResult7 = db.GetOpexList(fromdate, todate);

                        DataTable dtResult8 = new DataTable();
                        dtResult8 = db.GetConsReceiptNONNIRF(fromdate, todate);

                        DataTable dtResult9 = new DataTable();
                        dtResult9 = db.GetSponReceiptNONNIRF(fromdate, todate);
                        var ws = wb.Worksheets.Add("Receipts Summary");
                        ws.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws.Range("A1:C1").Row(1).Merge();
                        ws.Range("A1:O1").Style.Font.Bold = true;
                        ws.Cell(2, 1).Value = "Receipts " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws.Range("A2:O2").Row(1).Merge();
                        ws.Range("A2:O1").Style.Font.Bold = true;
                        ws.Cell(4, 2).Value = "Consultancy";
                        ws.Range("B4:E4").Row(1).Merge();
                        ws.Cell(4, 13).Value = "Sponsored";
                        ws.Range("M4:P4").Row(1).Merge();
                        ws.Range("A4:Z4").Style.Font.Bold = true;
                        ws.Range("A5:Z5").Style.Font.Bold = true;
                        ws.Range("A1:A50").Style.Font.Bold = true;
                        ws.Cell(5, 1).Value = "Department";
                        ws.Cell(5, 2).Value = "IC";
                        ws.Cell(5, 3).Value = "RB";
                        ws.Cell(5, 4).Value = "RC";
                        ws.Cell(5, 5).Value = "TT";
                        ws.Cell(5, 6).Value = "ET";
                        ws.Cell(5, 7).Value = "IT";
                        ws.Cell(5, 8).Value = "CR";
                        ws.Cell(5, 9).Value = "CN";
                        ws.Cell(5, 10).Value = "CT";
                        ws.Cell(5, 11).Value = "ConsTotal";
                        ws.Cell(5, 13).Value = "PFMS";
                        ws.Cell(5, 14).Value = "NONPFMS";
                        ws.Cell(5, 15).Value = "Imprints";
                        ws.Cell(5, 16).Value = "UAY";
                        ws.Cell(5, 17).Value = "SponTotal";
                        ws.Cell(5, 19).Value = "GrandTotal";
                        int summaryrow = 6;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(summaryrow, 1).Value = row["Department"].ToString();
                            ws.Cell(summaryrow, 2).Value = row["IC"].ToString();
                            ws.Cell(summaryrow, 3).Value = row["RB"].ToString();
                            ws.Cell(summaryrow, 4).Value = row["RC"].ToString();
                            ws.Cell(summaryrow, 5).Value = row["TT"].ToString();
                            ws.Cell(summaryrow, 6).Value = row["ET"].ToString();
                            ws.Cell(summaryrow, 7).Value = row["IT"].ToString();
                            ws.Cell(summaryrow, 8).Value = row["CR"].ToString();
                            ws.Cell(summaryrow, 9).Value = row["CN"].ToString();
                            ws.Cell(summaryrow, 10).Value = row["CT"].ToString();
                            ws.Cell(summaryrow, 11).Value = row["ConsTotal"].ToString();
                            ws.Cell(summaryrow, 13).Value = row["PFMS"].ToString();
                            ws.Cell(summaryrow, 14).Value = row["NONPFMS"].ToString();
                            ws.Cell(summaryrow, 15).Value = row["Imprints"].ToString();
                            ws.Cell(summaryrow, 16).Value = row["UAY"].ToString();
                            ws.Cell(summaryrow, 17).Value = row["SponTotal"].ToString();
                            ws.Cell(summaryrow, 19).Value = row["GrandTotal"].ToString();
                            summaryrow++;
                        }
                        ws.Cell(summaryrow, 1).Value = "Grand Total";
                        foreach (DataRow row in dtResult1.Rows)
                        {

                            ws.Cell(summaryrow, 2).Value = row["IC"].ToString();
                            ws.Cell(summaryrow, 3).Value = row["RB"].ToString();
                            ws.Cell(summaryrow, 4).Value = row["RC"].ToString();
                            ws.Cell(summaryrow, 5).Value = row["TT"].ToString();
                            ws.Cell(summaryrow, 6).Value = row["ET"].ToString();
                            ws.Cell(summaryrow, 7).Value = row["IT"].ToString();
                            ws.Cell(summaryrow, 8).Value = row["CR"].ToString();
                            ws.Cell(summaryrow, 9).Value = row["CN"].ToString();
                            ws.Cell(summaryrow, 10).Value = row["CT"].ToString();
                            ws.Cell(summaryrow, 11).Value = row["ConsTotal"].ToString();
                            ws.Cell(summaryrow, 13).Value = row["PFMS"].ToString();
                            ws.Cell(summaryrow, 14).Value = row["NONPFMS"].ToString();
                            ws.Cell(summaryrow, 15).Value = row["Imprints"].ToString();
                            ws.Cell(summaryrow, 16).Value = row["UAY"].ToString();
                            ws.Cell(summaryrow, 17).Value = row["SponTotal"].ToString();
                            ws.Cell(summaryrow, 19).Value = row["GrandTotal"].ToString();
                            summaryrow++;
                        }
                        var ws1 = wb.Worksheets.Add("Cons Receipts");
                        ws1.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws1.Range("A1:C1").Row(1).Merge();
                        ws1.Range("A1:O1").Style.Font.Bold = true;
                        ws1.Cell(2, 1).Value = "Consultancy Receipts " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws1.Range("A2:O2").Row(1).Merge();
                        ws1.Range("A2:O1").Style.Font.Bold = true;
                        ws1.Range("A4:Z4").Style.Font.Bold = true;
                        ws1.Cell(4, 1).Value = "ProjectNumber";
                        ws1.Cell(4, 2).Value = "Department";
                        ws1.Cell(4, 3).Value = "PIName";
                        ws1.Cell(4, 4).Value = "CoPi";
                        ws1.Cell(4, 5).Value = "Agency";
                        ws1.Cell(4, 6).Value = "ProjectTitle";
                        ws1.Cell(4, 7).Value = "Amount";
                        ws1.Cell(4, 8).Value = "Type";
                        ws1.Cell(4, 9).Value = "Receipt Number";
                        int consrecrow = 5;
                        foreach (DataRow row in dtResult2.Rows)
                        {
                            ws1.Cell(consrecrow, 1).Value = row["ProjectNumber"].ToString();
                            ws1.Cell(consrecrow, 2).Value = row["Department"].ToString();
                            ws1.Cell(consrecrow, 3).Value = row["PIName"].ToString();
                            ws1.Cell(consrecrow, 4).Value = row["CoPi"].ToString();
                            ws1.Cell(consrecrow, 5).Value = row["Agency"].ToString();
                            ws1.Cell(consrecrow, 6).Value = row["ProjectTitle"].ToString();
                            ws1.Cell(consrecrow, 7).Value = row["Amount"].ToString();
                            ws1.Cell(consrecrow, 8).Value = row["Type"].ToString();
                            ws1.Cell(consrecrow, 9).Value = row["ReceiptNumber"].ToString();
                            consrecrow++;
                        }
                        var ws2 = wb.Worksheets.Add("Spon Receipts");
                        ws2.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws2.Range("A1:C1").Row(1).Merge();
                        ws2.Range("A1:O1").Style.Font.Bold = true;
                        ws2.Cell(2, 1).Value = "Sponsored Receipts " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws2.Range("A2:O2").Row(1).Merge();
                        ws2.Range("A2:O1").Style.Font.Bold = true;
                        ws2.Range("A4:Z4").Style.Font.Bold = true;
                        ws2.Cell(4, 1).Value = "ProjectNumber";
                        ws2.Cell(4, 2).Value = "Department";
                        ws2.Cell(4, 3).Value = "PIName";
                        ws2.Cell(4, 4).Value = "CoPi";
                        ws2.Cell(4, 5).Value = "Agency";
                        ws2.Cell(4, 6).Value = "SanctionOrderDate";
                        ws2.Cell(4, 7).Value = "SanctionOrderNumber";
                        ws2.Cell(4, 8).Value = "ProjectTitle";
                        ws2.Cell(4, 9).Value = "ProjectType";
                        ws2.Cell(4, 10).Value = "Type";
                        ws2.Cell(4, 11).Value = "Amount";
                        ws2.Cell(4, 12).Value = "ReceiptNumber";
                        int sponrecrow = 5;
                        foreach (DataRow row in dtResult3.Rows)
                        {
                            ws2.Cell(sponrecrow, 1).Value = row["ProjectNumber"].ToString();
                            ws2.Cell(sponrecrow, 2).Value = row["Department"].ToString();
                            ws2.Cell(sponrecrow, 3).Value = row["PIName"].ToString();
                            ws2.Cell(sponrecrow, 4).Value = row["CoPi"].ToString();
                            ws2.Cell(sponrecrow, 5).Value = row["Agency"].ToString();
                            ws2.Cell(sponrecrow, 6).Value = row["SanctionOrderDate"].ToString();
                            ws2.Cell(sponrecrow, 7).Value = row["SanctionOrderNumber"].ToString();
                            ws2.Cell(sponrecrow, 8).Value = row["ProjectTitle"].ToString();
                            ws2.Cell(sponrecrow, 9).Value = row["ProjectType"].ToString();
                            ws2.Cell(sponrecrow, 10).Value = row["Type"].ToString();
                            ws2.Cell(sponrecrow, 11).Value = row["Amount"].ToString();
                            ws2.Cell(sponrecrow, 12).Value = row["ReceiptNumber"].ToString();
                            sponrecrow++;
                        }
                        var ws3 = wb.Worksheets.Add("Capex");
                        ws3.Cell(1, 1).Value = "ICSR - Capital Expenses Details " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws3.Range("A1:C1").Row(1).Merge();
                        ws3.Range("A1:O1").Style.Font.Bold = true;
                        ws3.Range("A4:Z4").Style.Font.Bold = true;
                        ws3.Cell(4, 1).Value = "HEAD OF EXPENDITURE";
                        ws3.Cell(4, 2).Value = "SPON";
                        ws3.Cell(4, 3).Value = "CONS";
                        ws3.Cell(4, 4).Value = "ICSROH";
                        ws3.Cell(4, 5).Value = "PCF";
                        ws3.Cell(4, 6).Value = "RMF";
                        ws3.Cell(4, 7).Value = "Total";
                        int capexrow = 5;
                        foreach (DataRow row in dtResult4.Rows)
                        {
                            ws3.Cell(capexrow, 1).Value = row["HeadName"].ToString();
                            ws3.Cell(capexrow, 2).Value = row["Spon"].ToString();
                            ws3.Cell(capexrow, 3).Value = row["Cons"].ToString();
                            ws3.Cell(capexrow, 4).Value = row["ICSROH"].ToString();
                            ws3.Cell(capexrow, 5).Value = row["PCF"].ToString();
                            ws3.Cell(capexrow, 6).Value = row["RMF"].ToString();
                            ws3.Cell(capexrow, 7).Value = row["Total"].ToString();

                            capexrow++;
                        }
                        var ws4 = wb.Worksheets.Add("Capex List");
                        ws4.Cell(1, 1).Value = "ICSR - Capital Expenses Details " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws4.Range("A1:C1").Row(1).Merge();
                        ws4.Range("A1:O1").Style.Font.Bold = true;
                        ws4.Range("A4:Z4").Style.Font.Bold = true;
                        ws4.Cell(4, 1).Value = "BillNumber";
                        ws4.Cell(4, 2).Value = "ProjectNumber";
                        ws4.Cell(4, 3).Value = "ProjectType";
                        ws4.Cell(4, 4).Value = "CommitmentNumber";
                        ws4.Cell(4, 5).Value = "HeadName";
                        ws4.Cell(4, 6).Value = "AmountSpent";
                        ws4.Cell(4, 7).Value = "Date";
                        int capexlistrow = 5;
                        foreach (DataRow row in dtResult5.Rows)
                        {
                            ws4.Cell(capexlistrow, 1).Value = row["BillNumber"].ToString();
                            ws4.Cell(capexlistrow, 2).Value = row["ProjectNumber"].ToString();
                            ws4.Cell(capexlistrow, 3).Value = row["ProjectType"].ToString();
                            ws4.Cell(capexlistrow, 4).Value = row["CommitmentNumber"].ToString();
                            ws4.Cell(capexlistrow, 5).Value = row["HeadName"].ToString();
                            ws4.Cell(capexlistrow, 6).Value = row["AmountSpent"].ToString();
                            ws4.Cell(capexlistrow, 7).Value = row["Date"].ToString();

                            capexlistrow++;
                        }

                        var ws5 = wb.Worksheets.Add("Opex");
                        ws5.Cell(1, 1).Value = "ICSR - Operational Expenses Details " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws5.Range("A1:C1").Row(1).Merge();
                        ws5.Range("A1:O1").Style.Font.Bold = true;
                        ws5.Range("A4:Z4").Style.Font.Bold = true;
                        ws5.Cell(4, 1).Value = "HEAD OF EXPENDITURE";
                        ws5.Cell(4, 2).Value = "SPON";
                        ws5.Cell(4, 3).Value = "CONS";
                        ws5.Cell(4, 4).Value = "ICSROH";
                        ws5.Cell(4, 5).Value = "PCF";
                        ws5.Cell(4, 6).Value = "RMF";
                        ws5.Cell(4, 7).Value = "Total";
                        int opexrow = 5;
                        foreach (DataRow row in dtResult6.Rows)
                        {
                            ws5.Cell(opexrow, 1).Value = row["HeadName"].ToString();
                            ws5.Cell(opexrow, 2).Value = row["Spon"].ToString();
                            ws5.Cell(opexrow, 3).Value = row["Cons"].ToString();
                            ws5.Cell(opexrow, 4).Value = row["ICSROH"].ToString();
                            ws5.Cell(opexrow, 5).Value = row["PCF"].ToString();
                            ws5.Cell(opexrow, 6).Value = row["RMF"].ToString();
                            ws5.Cell(opexrow, 7).Value = row["Total"].ToString();
                            opexrow++;
                        }

                        var ws6 = wb.Worksheets.Add("Opex List");
                        ws6.Cell(1, 1).Value = "ICSR - Operational Expenses Details " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws6.Range("A1:C1").Row(1).Merge();
                        ws6.Range("A1:C1").Style.Font.Bold = true;

                        ws6.Range("A4:Z4").Style.Font.Bold = true;
                        ws6.Cell(4, 1).Value = "BillNumber";
                        ws6.Cell(4, 2).Value = "ProjectNumber";
                        ws6.Cell(4, 3).Value = "ProjectType";
                        ws6.Cell(4, 4).Value = "CommitmentNumber";
                        ws6.Cell(4, 5).Value = "HeadName";
                        ws6.Cell(4, 6).Value = "Amount";
                        ws6.Cell(4, 7).Value = "Date";
                        int opexlistrow = 5;
                        foreach (DataRow row in dtResult7.Rows)
                        {
                            ws6.Cell(opexlistrow, 1).Value = row["BillNumber"].ToString();
                            ws6.Cell(opexlistrow, 2).Value = row["ProjectNumber"].ToString();
                            ws6.Cell(opexlistrow, 3).Value = row["ProjectType"].ToString();
                            ws6.Cell(opexlistrow, 4).Value = row["CommitmentNumber"].ToString();
                            ws6.Cell(opexlistrow, 5).Value = row["HeadName"].ToString();
                            ws6.Cell(opexlistrow, 6).Value = row["Amount"].ToString();
                            ws6.Cell(opexlistrow, 7).Value = row["Date"].ToString();

                            opexlistrow++;
                        }

                        //NON
                        var ws7 = wb.Worksheets.Add("Cons NON NIRF Receipts");
                        ws7.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws7.Range("A1:C1").Row(1).Merge();
                        ws7.Range("A1:O1").Style.Font.Bold = true;
                        ws7.Cell(2, 1).Value = "Consultancy Receipts " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws7.Range("A2:O2").Row(1).Merge();
                        ws7.Range("A2:O1").Style.Font.Bold = true;
                        ws7.Range("A4:Z4").Style.Font.Bold = true;
                        ws7.Cell(4, 1).Value = "ProjectNumber";
                        ws7.Cell(4, 2).Value = "Department";
                        ws7.Cell(4, 3).Value = "PIName";
                        ws7.Cell(4, 4).Value = "CoPi";
                        ws7.Cell(4, 5).Value = "Agency";
                        ws7.Cell(4, 6).Value = "ProjectTitle";
                        ws7.Cell(4, 7).Value = "Amount";
                        ws7.Cell(4, 8).Value = "Type";
                        ws7.Cell(4, 9).Value = "Receipt Number";
                        int consnonrecrow = 5;
                        foreach (DataRow row in dtResult8.Rows)
                        {
                            ws7.Cell(consnonrecrow, 1).Value = row["ProjectNumber"].ToString();
                            ws7.Cell(consnonrecrow, 2).Value = row["Department"].ToString();
                            ws7.Cell(consnonrecrow, 3).Value = row["PIName"].ToString();
                            ws7.Cell(consnonrecrow, 4).Value = row["CoPi"].ToString();
                            ws7.Cell(consnonrecrow, 5).Value = row["Agency"].ToString();
                            ws7.Cell(consnonrecrow, 6).Value = row["ProjectTitle"].ToString();
                            ws7.Cell(consnonrecrow, 7).Value = row["Amount"].ToString();
                            ws7.Cell(consnonrecrow, 8).Value = row["Type"].ToString();
                            ws7.Cell(consnonrecrow, 9).Value = row["ReceiptNumber"].ToString();
                            consnonrecrow++;
                        }
                        var ws8 = wb.Worksheets.Add("Spon NON NIRF Receipts");
                        ws8.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws8.Range("A1:C1").Row(1).Merge();
                        ws8.Range("A1:O1").Style.Font.Bold = true;
                        ws8.Cell(2, 1).Value = "Sponsored Receipts " + String.Format("{0:dd-MMMM-yyyy}", fromdate) + " - " + String.Format("{0:dd-MMMM-yyyy}", todate);
                        ws8.Range("A2:O2").Row(1).Merge();
                        ws8.Range("A2:O1").Style.Font.Bold = true;
                        ws8.Range("A4:Z4").Style.Font.Bold = true;
                        ws8.Cell(4, 1).Value = "ProjectNumber";
                        ws8.Cell(4, 2).Value = "Department";
                        ws8.Cell(4, 3).Value = "PIName";
                        ws8.Cell(4, 4).Value = "CoPi";
                        ws8.Cell(4, 5).Value = "Agency";
                        ws8.Cell(4, 6).Value = "SanctionOrderDate";
                        ws8.Cell(4, 7).Value = "SanctionOrderNumber";
                        ws8.Cell(4, 8).Value = "ProjectTitle";
                        ws8.Cell(4, 9).Value = "ProjectType";
                        ws8.Cell(4, 10).Value = "Type";
                        ws8.Cell(4, 11).Value = "Amount";
                        ws8.Cell(4, 12).Value = "ReceiptNumber";
                        int sponnonrecrow = 5;
                        foreach (DataRow row in dtResult9.Rows)
                        {
                            ws8.Cell(sponnonrecrow, 1).Value = row["ProjectNumber"].ToString();
                            ws8.Cell(sponnonrecrow, 2).Value = row["Department"].ToString();
                            ws8.Cell(sponnonrecrow, 3).Value = row["PIName"].ToString();
                            ws8.Cell(sponnonrecrow, 4).Value = row["CoPi"].ToString();
                            ws8.Cell(sponnonrecrow, 5).Value = row["Agency"].ToString();
                            ws8.Cell(sponnonrecrow, 6).Value = row["SanctionOrderDate"].ToString();
                            ws8.Cell(sponnonrecrow, 7).Value = row["SanctionOrderNumber"].ToString();
                            ws8.Cell(sponnonrecrow, 8).Value = row["ProjectTitle"].ToString();
                            ws8.Cell(sponnonrecrow, 9).Value = row["ProjectType"].ToString();
                            ws8.Cell(sponnonrecrow, 10).Value = row["Type"].ToString();
                            ws8.Cell(sponnonrecrow, 11).Value = row["Amount"].ToString();
                            ws8.Cell(sponnonrecrow, 12).Value = row["ReceiptNumber"].ToString();
                            sponnonrecrow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=NIRF.xls");
                        return File(workStream, fileType);
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ReceiptTransfer(string Month)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                FinOp fac = new FinOp(System.DateTime.Now);
                var fdate = fac.GetMonthFirstDate(Month);
                var tdate = fac.GetMonthLastDate(Month);
                var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetReceiptTransfer(fromdate, todate);
                        DataTable dtResult1 = new DataTable();
                        dtResult1 = db.GetReceiptTransferList(fromdate, todate);
                        var ws = wb.Worksheets.Add("Summary");
                        ws.Cell(1, 1).Value = "Receipts Transfer " + fromdate + " - " + todate;
                        ws.Range("A1:C1").Row(1).Merge();
                        ws.Range("A1:O1").Style.Font.Bold = true;
                        ws.Cell(4, 1).Value = "FROMBANK";
                        ws.Cell(4, 2).Value = "TOBANK";
                        ws.Cell(4, 3).Value = "AMOUNT";
                        ws.Range("A4:Z4").Style.Font.Bold = true;
                        int summaryrow = 5;
                        foreach (DataRow row in dtResult.Rows)
                        {
                            ws.Cell(summaryrow, 1).Value = row["FROMBANK"].ToString();
                            ws.Cell(summaryrow, 2).Value = row["TOBANK"].ToString();
                            ws.Cell(summaryrow, 3).Value = row["AMOUNT"].ToString();
                            summaryrow++;
                        }
                        var ws1 = wb.Worksheets.Add("BreakUp");
                        ws1.Cell(1, 1).Value = "Receipts Transfer " + fromdate + " - " + todate;
                        ws1.Cell(4, 1).Value = "InvoiceNumber";
                        ws1.Cell(4, 2).Value = "InvoiceDate";
                        ws1.Cell(4, 3).Value = "ProjectNumber";
                        ws1.Cell(4, 4).Value = "ProjectType";
                        ws1.Cell(4, 5).Value = "ProjectBank";
                        ws1.Cell(4, 6).Value = "ReceivedAmount";
                        ws1.Cell(4, 7).Value = "FundReceivedBank";
                        ws1.Cell(4, 8).Value = "ReceiptDate";
                        int BreakUprow = 5;
                        foreach (DataRow row in dtResult1.Rows)
                        {
                            ws1.Cell(BreakUprow, 1).Value = row["InvoiceNumber"].ToString();
                            ws1.Cell(BreakUprow, 2).Value = row["InvoiceDate"].ToString();
                            ws1.Cell(BreakUprow, 3).Value = row["ProjectNumber"].ToString();
                            ws1.Cell(BreakUprow, 4).Value = row["ProjectType"].ToString();
                            ws1.Cell(BreakUprow, 5).Value = row["ProjectBank"].ToString();
                            ws1.Cell(BreakUprow, 6).Value = row["ReceivedAmount"].ToString();
                            ws1.Cell(BreakUprow, 7).Value = row["FundReceivedBank"].ToString();
                            ws1.Cell(BreakUprow, 8).Value = row["ReceiptDate"].ToString();
                            BreakUprow++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ReceiptTransfer.xls");
                        return File(workStream, fileType);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult Annexuresalary(int Finyear)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                DataTable dtColumns = new DataTable();
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                        FinOp fac = new FinOp(System.DateTime.Now);
                        int CurrentYear = fromdate.Year;
                        int PreviousYear = fromdate.Year - 1;
                        int NextYear = fromdate.Year + 1;
                        string PreYear = PreviousYear.ToString();
                        string NexYear = NextYear.ToString();
                        string CurYear = CurrentYear.ToString();
                        DateTime startDate; DateTime endDate;
                        if (fromdate.Month > 2)
                        {
                            startDate = new DateTime(fromdate.Year, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        else
                        {
                            startDate = new DateTime((fromdate.Year) - 1, 3, 1);
                            endDate = startDate.AddYears(1).AddMonths(-1);
                        }
                        using (var connection = Common.getConnection())
                        {
                            decimal cessPct = Convert.ToDecimal(System.Web.Configuration.WebConfigurationManager.AppSettings["Cess_Percentage"]);
                            decimal CommonExemption = Convert.ToDecimal(System.Web.Configuration.WebConfigurationManager.AppSettings["Adhoc_Common_Exemption"]);

                            //ToDate = ToDate.AddDays(1).AddTicks(-2);
                            //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                            //var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                            connection.Open();
                            var command = new System.Data.SqlClient.SqlCommand();
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "select PayBill,sum(Basic)as Basic,sum(HRA)as HRA,sum(TotalSalary)as TotalSalary," + CommonExemption + " as CommonExemption ,case when sum(ProfTax) > 2500  then 2500 else isnull(sum(ProfTax), 0) end as ProfTax,(sum(TotalSalary) -" + CommonExemption + " - (case when sum(ProfTax) > 2500  then 2500 else isnull(sum(ProfTax), 0) end)) as Total25,sum(DeclarationId1) as DeclarationId1,sum(DeclarationId2) as DeclarationId2,sum(DeclarationId3) as DeclarationId3,sum(DeclarationId4) as DeclarationId4,sum(DeclarationId5) as DeclarationId5,sum(DeclarationId6) as DeclarationId6,sum(DeclarationId7) as DeclarationId7,sum(DeclarationId8) as DeclarationId8,sum(DeclarationId9) as DeclarationId9,sum(DeclarationId10) as DeclarationId10,sum(DeclarationId11) as DeclarationId11,sum(DeclarationId12) as DeclarationId12,sum(DeclarationId13) as DeclarationId13,sum(DeclarationId14) as DeclarationId14, sum(DeclarationId15) as DeclarationId15,sum(DeclarationId16) as DeclarationId16,sum(DeclarationId17) as DeclarationId17,sum(DeclarationId18) as DeclarationId18,sum(DeclarationId19) as DeclarationId19,sum(DeclarationId20) as DeclarationId20,sum(DeclarationId21) as DeclarationId21,sum(DeclarationId22) as DeclarationId22,sum(DeclarationId23) as DeclarationId23,sum(DeclarationId24) as DeclarationId24,sum(DeclarationId25) as DeclarationId25,sum(DeclarationId26) as DeclarationId26,sum(DeclarationId27) as DeclarationId27,sum(DeclarationId28) as DeclarationId28,sum(DeclarationId29) as DeclarationId29,sum(DeclarationId30) as DeclarationId30,sum(DeclarationId31) as DeclarationId31,sum(DeclarationId32) as DeclarationId32,sum(DeclarationId33) as DeclarationId33,sum(DeclarationId34) as DeclarationId34,sum(DeclarationId35) as DeclarationId35,sum(DeclarationId36) as DeclarationId36,sum(DeclarationId37) as DeclarationId37,sum(DeclarationId38) as DeclarationId38,sum(DeclarationId39) as DeclarationId39,sum(DeclarationId40) as DeclarationId40,sum(DeclarationId41) as DeclarationId41,sum(DeclarationId42) as DeclarationId42,sum(DeclarationId43) as DeclarationId43,sum(DeclarationId44) as DeclarationId44, sum(DeclarationId45) as DeclarationId45, sum(ITdecTotal) as ITdecTotal,isnull((select top 1 TaxableIncome from tblSalaryPayment where a.PayBill = PayBill and TaxableIncome > 0 order by PaymentId desc),0)as TaxableIncome,(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0)*(100 / 10" + cessPct + ")) as Tax,case when(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc), 0) * (100 / 10" + cessPct + ")) > 12500 then 0 else -12500 end as col63,isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0) as col65,(((isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0)*(100 / 10" + cessPct + ")))+( case when(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc), 0) * (100 / 10" + cessPct + ")) > 12500 then 0 else -12500 end)+(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0))) as col66,(((isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0)*(100 / 104)))+( case when(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc), 0) * (100 / 10" + cessPct + ")) > 12500 then 0 else -12500 end)+(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0))) as col74, isnull((select sum(monthlyTax) from tblSalaryPayment where a.PayBill = PayBill and Tax > 0),0) as col75,(((((isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0)*(100 / 10" + cessPct + ")))+( case when(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc), 0) * (100 / 10" + cessPct + ")) > 12500 then 0 else -12500 end)+(isnull((select top 1 Tax from tblSalaryPayment where a.PayBill = PayBill and Tax > 0 order by PaymentId desc),0))))-(isnull((select sum(monthlyTax) from tblSalaryPayment where a.PayBill = PayBill and Tax > 0),0))) as col77 from vw_AnnuxSalary as a where a.PaidDate >='" + startDate + "' and a.PaidDate <= '" + endDate + "' group by a.PayBill ";
                            command.CommandTimeout = 180;
                            var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                            var dataset = new DataSet();
                            adapter.Fill(dataset);
                            dtColumns = dataset.Tables[0];
                            //DataView dv1 = dtColumns.DefaultView;
                            //dv1.RowFilter = " AMOUNT >0";
                            //dtColumns = dv1.ToTable();
                            // dtColumns = dataset.Tables[0];
                        }
                        var ws = wb.Worksheets.Add("Annexuresalary");
                        ws.Range("A2:GZ2").Style.Font.Bold = true;
                        ws.Range("A1:GZ1").Style.Font.Bold = true;
                        ws.Cell(2, 1).Value = "Emp. ID.";
                        ws.Cell(1, 2).Value = "Gross Salary";
                        ws.Range("B1:D1").Row(1).Merge();
                        ws.Cell(2, 2).Value = "Total amount of salary , excluding amount required  to shown in columns 3 and 4.";
                        ws.Cell(2, 3).Value = "HOUSE RENT (80GG) Allowance and Other Allowances to the extent Chargeable to Tax (see sec 10(13A) read with rule 2A)";
                        ws.Cell(2, 4).Value = "Value of perquisites and amount accretion to Employees PF Account(Total 4=84+85+86+87+88+89+90)";
                        ws.Cell(1, 5).Value = "Exemption u/s 10";
                        ws.Range("E1:F1").Row(1).Merge();
                        ws.Cell(2, 5).Value = "  Description (1)";
                        ws.Cell(2, 6).Value = "Amount (1)";
                        ws.Cell(1, 7).Value = "Exemption u/s 10";
                        ws.Range("G1:H1").Row(1).Merge();
                        ws.Cell(2, 7).Value = "Description (2)";
                        ws.Cell(2, 8).Value = "Amount (2)";
                        ws.Cell(1, 9).Value = "Exemption u/s 10";
                        ws.Range("I1:J1").Row(1).Merge();
                        ws.Cell(2, 9).Value = " Description (3)";
                        ws.Cell(2, 10).Value = "Amount (3)";
                        ws.Cell(1, 11).Value = "Exemption u/s 10";
                        ws.Range("K1:L1").Row(1).Merge();
                        ws.Cell(2, 11).Value = "Description (4)";
                        ws.Cell(2, 12).Value = "Amount (4)";
                        ws.Cell(1, 13).Value = "Exemption u/s 10";
                        ws.Range("M1:N1").Row(1).Merge();
                        ws.Cell(2, 13).Value = "Description (5)";
                        ws.Cell(2, 14).Value = "Amount (5)";
                        ws.Cell(1, 15).Value = "Exemption u/s 10";
                        ws.Range("O1:P1").Row(1).Merge();
                        ws.Cell(2, 15).Value = "Description (6)";
                        ws.Cell(2, 16).Value = "Amount (6)";
                        ws.Cell(1, 17).Value = "Exemption u/s 10";
                        ws.Range("Q1:R1").Row(1).Merge();
                        ws.Cell(2, 17).Value = "Description (7)";
                        ws.Cell(2, 18).Value = "Amount (7)";
                        ws.Cell(1, 19).Value = "Exemption u/s 10";
                        ws.Range("S1:T1").Row(1).Merge();
                        ws.Cell(2, 19).Value = " Description (8)";
                        ws.Cell(2, 20).Value = "Amount (8)";
                        ws.Cell(2, 21).Value = "Gross Total of 'Total Exemption  under section 10' under  associated ' Salary Details - Section 10 Detail ' ";
                        ws.Cell(2, 22).Value = "Total Salary ";
                        ws.Cell(1, 23).Value = "Deduction u/s 16";
                        ws.Range("W1:X1").Row(1).Merge();
                        ws.Cell(2, 23).Value = "";
                        ws.Cell(2, 24).Value = "Tax on Employment(PT)";
                        ws.Cell(2, 25).Value = "Gross Total of 'Total Deduction under section 16' under  associated ' Salary Details  - Section 16 Detail '  ";
                        ws.Cell(2, 26).Value = "Income chargeable under the head Current Salaries ";
                        ws.Cell(2, 27).Value = "Taxable Salary from previous Employment";
                        ws.Cell(2, 28).Value = "Income ( including loss from house property) under any head other than income under the head 'salaries' offered for TDS ";
                        ws.Cell(2, 29).Value = "Gross Total Income ";
                        ws.Cell(1, 30).Value = "Chapter VIA Details (1)";
                        ws.Range("AD1:AF1").Row(1).Merge();
                        ws.Cell(2, 30).Value = "Particulars";
                        ws.Cell(2, 31).Value = "Gross Amount";
                        ws.Cell(2, 32).Value = "Deductible Amount";
                        ws.Cell(1, 33).Value = "Chapter VIA Details (2)";
                        ws.Range("AG1:AI1").Row(1).Merge();
                        ws.Cell(2, 33).Value = "Particulars";
                        ws.Cell(2, 34).Value = "Gross Amount";
                        ws.Cell(2, 35).Value = "Deductible Amount";
                        ws.Cell(1, 36).Value = "Chapter VIA Details (3)";
                        ws.Range("AJ1:AL1").Row(1).Merge();
                        ws.Cell(2, 36).Value = "Particulars";
                        ws.Cell(2, 37).Value = "Gross Amount";
                        ws.Cell(2, 38).Value = "Deductible Amount";
                        ws.Cell(1, 39).Value = "Chapter VIA Details (4)";
                        ws.Range("AM1:AO1").Row(1).Merge();
                        ws.Cell(2, 39).Value = "Particulars";
                        ws.Cell(2, 40).Value = "Gross Amount";
                        ws.Cell(2, 41).Value = "Deductible Amount";
                        ws.Cell(1, 42).Value = "Chapter VIA Details (5)";
                        ws.Range("AP1:AR1").Row(1).Merge();
                        ws.Cell(2, 42).Value = "Particulars";
                        ws.Cell(2, 43).Value = "Gross Amount";
                        ws.Cell(2, 44).Value = "Deductible Amount";
                        ws.Cell(1, 45).Value = "Chapter VIA Details (6)";
                        ws.Range("AS1:AU1").Row(1).Merge();
                        ws.Cell(2, 45).Value = "Particulars";
                        ws.Cell(2, 46).Value = "Gross Amount";
                        ws.Cell(2, 47).Value = "Deductible Amount";
                        ws.Cell(1, 48).Value = "Chapter VIA Details (7)";
                        ws.Range("AV1:AX1").Row(1).Merge();
                        ws.Cell(2, 48).Value = "Particulars";
                        ws.Cell(2, 49).Value = "Gross Amount";
                        ws.Cell(2, 50).Value = "Deductible Amount";
                        ws.Cell(1, 51).Value = "Chapter VIA Details (8)";
                        ws.Range("AY1:BA1").Row(1).Merge();
                        ws.Cell(2, 51).Value = "Particulars";
                        ws.Cell(2, 52).Value = "Gross Amount";
                        ws.Cell(2, 53).Value = "Deductible Amount";
                        ws.Cell(1, 54).Value = "Chapter VIA Details (9)";
                        ws.Range("BB1:BD1").Row(1).Merge();
                        ws.Cell(2, 54).Value = "Particulars";
                        ws.Cell(2, 55).Value = "Gross Amount";
                        ws.Cell(2, 56).Value = "Deductible Amount";
                        ws.Cell(1, 57).Value = "Chapter VIA Details (10)";
                        ws.Range("BE1:BG1").Row(1).Merge();
                        ws.Cell(2, 57).Value = "Particulars";
                        ws.Cell(2, 58).Value = "Gross Amount";
                        ws.Cell(2, 59).Value = "Deductible Amount";
                        ws.Cell(1, 60).Value = "Chapter VIA Details (11)";
                        ws.Range("BH1:BJ1").Row(1).Merge();
                        ws.Cell(2, 60).Value = "Particulars";
                        ws.Cell(2, 61).Value = "Gross Amount";
                        ws.Cell(2, 62).Value = "Deductible Amount";
                        ws.Cell(1, 63).Value = "Chapter VIA Details (12)";
                        ws.Range("BK1:BM1").Row(1).Merge();
                        ws.Cell(2, 63).Value = "Particulars";
                        ws.Cell(2, 64).Value = "Gross Amount";
                        ws.Cell(2, 65).Value = "Deductible Amount";
                        ws.Cell(1, 66).Value = "Chapter VIA Details (13)";
                        ws.Range("BN1:BP1").Row(1).Merge();
                        ws.Cell(2, 66).Value = "Particulars";
                        ws.Cell(2, 67).Value = "Gross Amount";
                        ws.Cell(2, 68).Value = "Deductible Amount";
                        ws.Cell(1, 69).Value = "Chapter VIA Details (14)";
                        ws.Range("BQ1:BS1").Row(1).Merge();
                        ws.Cell(2, 69).Value = "Particulars";
                        ws.Cell(2, 70).Value = "Gross Amount";
                        ws.Cell(2, 71).Value = "Deductible Amount";
                        ws.Cell(1, 72).Value = "Chapter VIA Details (15)";
                        ws.Range("BT1:BV1").Row(1).Merge();
                        ws.Cell(2, 72).Value = "Particulars";
                        ws.Cell(2, 73).Value = "Gross Amount";
                        ws.Cell(2, 74).Value = "Deductible Amount";
                        ws.Cell(1, 75).Value = "Chapter VIA Details (16)";
                        ws.Range("BW1:BY1").Row(1).Merge();
                        ws.Cell(2, 75).Value = "Particulars";
                        ws.Cell(2, 76).Value = "Gross Amount";
                        ws.Cell(2, 77).Value = "Deductible Amount";
                        ws.Cell(1, 78).Value = "Chapter VIA Details (17)";
                        ws.Range("BZ1:CB1").Row(1).Merge();
                        ws.Cell(2, 78).Value = "Particulars";
                        ws.Cell(2, 79).Value = "Gross Amount";
                        ws.Cell(2, 80).Value = "Deductible Amount";
                        ws.Cell(1, 81).Value = "Chapter VIA Details (18)";
                        ws.Range("CC1:CE1").Row(1).Merge();
                        ws.Cell(2, 81).Value = "Particulars";
                        ws.Cell(2, 82).Value = "Gross Amount";
                        ws.Cell(2, 83).Value = "Deductible Amount";
                        ws.Cell(1, 84).Value = "Chapter VIA Details (19)";
                        ws.Range("CF1:CH1").Row(1).Merge();
                        ws.Cell(2, 84).Value = "Particulars";
                        ws.Cell(2, 85).Value = "Gross Amount";
                        ws.Cell(2, 86).Value = "Deductible Amount";
                        ws.Cell(1, 87).Value = "Chapter VIA Details (20)";
                        ws.Range("CI1:CK1").Row(1).Merge();
                        ws.Cell(2, 87).Value = "Particulars";
                        ws.Cell(2, 88).Value = "Gross Amount";
                        ws.Cell(2, 89).Value = "Deductible Amount";
                        ws.Cell(1, 90).Value = "Chapter VIA Details (21)";
                        ws.Range("CL1:CN1").Row(1).Merge();
                        ws.Cell(2, 90).Value = "Particulars";
                        ws.Cell(2, 91).Value = "Gross Amount";
                        ws.Cell(2, 92).Value = "Deductible Amount";
                        ws.Cell(1, 93).Value = "Chapter VIA Details (22)";
                        ws.Range("CO1:CQ1").Row(1).Merge();
                        ws.Cell(2, 93).Value = "Particulars";
                        ws.Cell(2, 94).Value = "Gross Amount";
                        ws.Cell(2, 95).Value = "Deductible Amount";
                        ws.Cell(1, 96).Value = "Chapter VIA Details (23)";
                        ws.Range("CR1:CT1").Row(1).Merge();
                        ws.Cell(2, 96).Value = "Particulars";
                        ws.Cell(2, 97).Value = "Gross Amount";
                        ws.Cell(2, 98).Value = "Deductible Amount";
                        ws.Cell(1, 99).Value = "Chapter VIA Details (24)";
                        ws.Range("CU1:CW1").Row(1).Merge();
                        ws.Cell(2, 99).Value = "Particulars";
                        ws.Cell(2, 100).Value = "Gross Amount";
                        ws.Cell(2, 101).Value = "Deductible Amount";
                        ws.Cell(1, 102).Value = "Chapter VIA Details (25)";
                        ws.Range("CX1:CZ1").Row(1).Merge();
                        ws.Cell(2, 102).Value = "Particulars";
                        ws.Cell(2, 103).Value = "Gross Amount";
                        ws.Cell(2, 104).Value = "Deductible Amount";
                        ws.Cell(1, 105).Value = "Chapter VIA Details (26)";
                        ws.Range("DA1:DC1").Row(1).Merge();
                        ws.Cell(2, 105).Value = "Particulars";
                        ws.Cell(2, 106).Value = "Gross Amount";
                        ws.Cell(2, 107).Value = "Deductible Amount";
                        ws.Cell(1, 108).Value = "Chapter VIA Details (27)";
                        ws.Range("DD1:DF1").Row(1).Merge();
                        ws.Cell(2, 108).Value = "Particulars";
                        ws.Cell(2, 109).Value = "Gross Amount";
                        ws.Cell(2, 110).Value = "Deductible Amount";
                        ws.Cell(1, 111).Value = "Chapter VIA Details (28)";
                        ws.Range("DG1:DI1").Row(1).Merge();
                        ws.Cell(2, 111).Value = "Particulars";
                        ws.Cell(2, 112).Value = "Gross Amount";
                        ws.Cell(2, 113).Value = "Deductible Amount";
                        ws.Cell(1, 114).Value = "Chapter VIA Details (29)";
                        ws.Range("DJ1:DL1").Row(1).Merge();
                        ws.Cell(2, 114).Value = "Particulars";
                        ws.Cell(2, 115).Value = "Gross Amount";
                        ws.Cell(2, 116).Value = "Deductible Amount";
                        ws.Cell(1, 117).Value = "Chapter VIA Details (30)";
                        ws.Range("DM1:DO1").Row(1).Merge();
                        ws.Cell(2, 117).Value = "Particulars";
                        ws.Cell(2, 118).Value = "Gross Amount";
                        ws.Cell(2, 119).Value = "Deductible Amount";
                        ws.Cell(1, 120).Value = "Chapter VIA Details (31)";
                        ws.Range("DP1:DR1").Row(1).Merge();
                        ws.Cell(2, 120).Value = "Particulars";
                        ws.Cell(2, 121).Value = "Gross Amount";
                        ws.Cell(2, 122).Value = "Deductible Amount";
                        ws.Cell(1, 123).Value = "Chapter VIA Details (32)";
                        ws.Range("DS1:DU1").Row(1).Merge();
                        ws.Cell(2, 123).Value = "Particulars";
                        ws.Cell(2, 124).Value = "Gross Amount";
                        ws.Cell(2, 125).Value = "Deductible Amount";
                        ws.Cell(1, 126).Value = "Chapter VIA Details (33)";
                        ws.Range("DV1:DX1").Row(1).Merge();
                        ws.Cell(2, 126).Value = "Particulars";
                        ws.Cell(2, 127).Value = "Gross Amount";
                        ws.Cell(2, 128).Value = "Deductible Amount";
                        ws.Cell(1, 129).Value = "Chapter VIA Details (34)";
                        ws.Range("DY1:EA1").Row(1).Merge();
                        ws.Cell(2, 129).Value = "Particulars";
                        ws.Cell(2, 130).Value = "Gross Amount";
                        ws.Cell(2, 131).Value = "Deductible Amount";
                        ws.Cell(1, 132).Value = "Chapter VIA Details (35)";
                        ws.Range("EB1:ED1").Row(1).Merge();
                        ws.Cell(2, 132).Value = "Particulars";
                        ws.Cell(2, 133).Value = "Gross Amount";
                        ws.Cell(2, 134).Value = "Deductible Amount";
                        ws.Cell(1, 135).Value = "Chapter VIA Details (36)";
                        ws.Range("EE1:EG1").Row(1).Merge();
                        ws.Cell(2, 135).Value = "Particulars";
                        ws.Cell(2, 136).Value = "Gross Amount";
                        ws.Cell(2, 137).Value = "Deductible Amount";
                        ws.Cell(1, 138).Value = "Chapter VIA Details (37)";
                        ws.Range("EH1:EJ1").Row(1).Merge();
                        ws.Cell(2, 138).Value = "Particulars";
                        ws.Cell(2, 139).Value = "Gross Amount";
                        ws.Cell(2, 140).Value = "Deductible Amount";
                        ws.Cell(1, 141).Value = "Chapter VIA Details (38)";
                        ws.Range("EK1:EM1").Row(1).Merge();
                        ws.Cell(2, 141).Value = "Particulars";
                        ws.Cell(2, 142).Value = "Gross Amount";
                        ws.Cell(2, 143).Value = "Deductible Amount";
                        ws.Cell(1, 144).Value = "Chapter VIA Details (39)";
                        ws.Range("EN1:EP1").Row(1).Merge();
                        ws.Cell(2, 144).Value = "Particulars";
                        ws.Cell(2, 145).Value = "Gross Amount";
                        ws.Cell(2, 146).Value = "Deductible Amount";
                        ws.Cell(1, 147).Value = "Chapter VIA Details (40)";
                        ws.Range("EQ1:ES1").Row(1).Merge();
                        ws.Cell(2, 147).Value = "Particulars";
                        ws.Cell(2, 148).Value = "Gross Amount";
                        ws.Cell(2, 149).Value = "Deductible Amount";
                        ws.Cell(1, 150).Value = "Chapter VIA Details (41)";
                        ws.Range("ET1:EV1").Row(1).Merge();
                        ws.Cell(2, 150).Value = "Particulars";
                        ws.Cell(2, 151).Value = "Gross Amount";
                        ws.Cell(2, 152).Value = "Deductible Amount";
                        ws.Cell(1, 153).Value = "Chapter VIA Details (42)";
                        ws.Range("EW1:EY1").Row(1).Merge();
                        ws.Cell(2, 153).Value = "Particulars";
                        ws.Cell(2, 154).Value = "Gross Amount";
                        ws.Cell(2, 155).Value = "Deductible Amount";
                        ws.Cell(1, 156).Value = "Chapter VIA Details (43)";
                        ws.Range("EZ1:FB1").Row(1).Merge();
                        ws.Cell(2, 156).Value = "Particulars";
                        ws.Cell(2, 157).Value = "Gross Amount";
                        ws.Cell(2, 158).Value = "Deductible Amount";
                        ws.Cell(1, 159).Value = "Chapter VIA Details (44)";
                        ws.Range("FC1:FE1").Row(1).Merge();
                        ws.Cell(2, 159).Value = "Particulars";
                        ws.Cell(2, 160).Value = "Gross Amount";
                        ws.Cell(2, 161).Value = "Deductible Amount";
                        ws.Cell(1, 162).Value = "Chapter VIA Details (45)";
                        ws.Range("FF1:FH1").Row(1).Merge();
                        ws.Cell(2, 162).Value = "Particulars";
                        ws.Cell(2, 163).Value = "Gross Amount";
                        ws.Cell(2, 164).Value = "Deductible Amount";
                        ws.Cell(2, 165).Value = "Gross Total of 'Amount deductible under provisions of chapter VI-A' under  associated ' Salary Details  - Chapter VIA Detail ' ";
                        ws.Cell(2, 166).Value = "Total Taxable Income";
                        ws.Cell(2, 167).Value = "Income Tax on Total Income";
                        ws.Cell(2, 168).Value = "Income Tax Credit u/s 87A";
                        ws.Cell(2, 169).Value = "Surcharge";
                        ws.Cell(2, 170).Value = "Education Cess";
                        ws.Cell(2, 171).Value = "Total Income Tax Payable";
                        ws.Cell(2, 172).Value = "Income Tax Relief";
                        ws.Cell(1, 173).Value = "TDS made in Previous Employment";
                        ws.Range("FQ1:FS1").Row(1).Merge();
                        ws.Cell(2, 173).Value = "Income Tax";
                        ws.Cell(2, 174).Value = "Surcharge";
                        ws.Cell(2, 175).Value = "Cess";
                        ws.Cell(1, 176).Value = "TDS made without Deduction details";
                        ws.Range("FT1:FV1").Row(1).Merge();
                        ws.Cell(2, 176).Value = "Income Tax";
                        ws.Cell(2, 177).Value = "Surcharge";
                        ws.Cell(2, 178).Value = "Cess";
                        ws.Cell(2, 179).Value = "Net Income Tax payable";
                        ws.Cell(2, 180).Value = "TDS deducted From Employee u/s 192 (1)";
                        ws.Cell(2, 181).Value = "Tax paid by the employer on behalf of the employee u/s 192 (1A) on perquisites u/s 17(2)";
                        ws.Cell(2, 182).Value = "Tax Payable/Refundable ";
                        ws.Cell(1, 183).Value = "Perquisites.";
                        ws.Range("GA1:GN1").Row(1).Merge();
                        ws.Cell(2, 183).Value = "Perk- Where accommodation is unfurnished";
                        ws.Cell(2, 184).Value = "Perk-Furnished-Value as if accommodation is unfurnished";
                        ws.Cell(2, 185).Value = "Perk-Furnished-Cost of furniture ";
                        ws.Cell(2, 186).Value = "Perk-Furnished-Furniture Rentals";
                        ws.Cell(2, 187).Value = "Perk-Furnished-Perquisite value of furniture (10% of 57)  + Perk-Furnished-Furniture Rentals";
                        ws.Cell(2, 188).Value = "Perk-Furnished-Total ";
                        ws.Cell(2, 189).Value = "Rent, if any, paid by employee ";
                        ws.Cell(2, 190).Value = "Value of perquisite   ";
                        ws.Cell(2, 191).Value = "Perquisite value of conveyance/car ";
                        ws.Cell(2, 192).Value = "Remuneration paid by employer for domestic and personal services provided to the employee";
                        ws.Cell(2, 193).Value = "Value of free or concessional passages on home leave and other traveling to the extent chargeable to tax ";
                        ws.Cell(2, 194).Value = "Estimated value of any other benefit or amenity provided by the employer free of cost or at concessional rate not included in the preceding columns";
                        ws.Cell(2, 195).Value = "Employer's contribution to recognised provident fund in excess of 12% of employee's salary";
                        ws.Cell(2, 196).Value = "Interest credited to the assessee's account in recognised PF Fund in excess of the rate fixed by central govt.";
                        ws.Cell(2, 197).Value = "Sum Total of Other Recoveries from Employee - N.A";
                        int salrows = 3;
                        foreach (DataRow row in dtColumns.Rows)
                        {
                            ws.Cell(salrows, 1).Value = row["PayBill"].ToString();
                            ws.Cell(salrows, 2).Value = row["Basic"].ToString();
                            ws.Cell(salrows, 3).Value = row["HRA"].ToString();
                            ws.Cell(salrows, 4).Value = "0";
                            ws.Cell(salrows, 5).Value = "0";
                            ws.Cell(salrows, 6).Value = "0";
                            ws.Cell(salrows, 7).Value = "0";
                            ws.Cell(salrows, 8).Value = "0";
                            ws.Cell(salrows, 9).Value = "0";
                            ws.Cell(salrows, 10).Value = "0";
                            ws.Cell(salrows, 11).Value = "0";
                            ws.Cell(salrows, 12).Value = "0";
                            ws.Cell(salrows, 13).Value = "0";
                            ws.Cell(salrows, 14).Value = "0";
                            ws.Cell(salrows, 15).Value = "0";
                            ws.Cell(salrows, 16).Value = "0";
                            ws.Cell(salrows, 17).Value = "0";
                            ws.Cell(salrows, 18).Value = "0";
                            ws.Cell(salrows, 19).Value = "0";
                            ws.Cell(salrows, 20).Value = "0";
                            ws.Cell(salrows, 21).Value = "0";
                            ws.Cell(salrows, 22).Value = row["TotalSalary"].ToString();
                            ws.Cell(salrows, 23).Value = row["CommonExemption"].ToString();
                            ws.Cell(salrows, 24).Value = row["ProfTax"].ToString();
                            ws.Cell(salrows, 25).Value = row["Total25"].ToString();
                            ws.Cell(salrows, 26).Value = row["Total25"].ToString();
                            ws.Cell(salrows, 27).Value = "0";
                            ws.Cell(salrows, 28).Value = "0";
                            ws.Cell(salrows, 29).Value = row["Total25"].ToString();
                            ws.Cell(salrows, 30).Value = "80C - 5 Years of Fixed Deposit in Scheduled Bank";
                            ws.Cell(salrows, 31).Value = row["DeclarationId1"].ToString();
                            ws.Cell(salrows, 32).Value = row["DeclarationId1"].ToString();
                            ws.Cell(salrows, 33).Value = "80C - Children Tuition Fees";
                            ws.Cell(salrows, 34).Value = row["DeclarationId2"].ToString();
                            ws.Cell(salrows, 35).Value = row["DeclarationId2"].ToString();
                            ws.Cell(salrows, 36).Value = "80CCC - Contribution to Pension Fund";
                            ws.Cell(salrows, 37).Value = row["DeclarationId3"].ToString();
                            ws.Cell(salrows, 38).Value = row["DeclarationId3"].ToString();
                            ws.Cell(salrows, 39).Value = "80C - Deposit in NSC";
                            ws.Cell(salrows, 40).Value = row["DeclarationId4"].ToString();
                            ws.Cell(salrows, 41).Value = row["DeclarationId4"].ToString();
                            ws.Cell(salrows, 42).Value = "80C - Deposit in NSS";
                            ws.Cell(salrows, 43).Value = row["DeclarationId5"].ToString();
                            ws.Cell(salrows, 44).Value = row["DeclarationId5"].ToString();
                            ws.Cell(salrows, 45).Value = "80C - Deposit in Post Office Savings Schemes";
                            ws.Cell(salrows, 46).Value = row["DeclarationId6"].ToString();
                            ws.Cell(salrows, 47).Value = row["DeclarationId6"].ToString();
                            ws.Cell(salrows, 48).Value = "80C - Equity Linked Savings Scheme ( ELSS )";
                            ws.Cell(salrows, 49).Value = row["DeclarationId7"].ToString();
                            ws.Cell(salrows, 50).Value = row["DeclarationId7"].ToString();
                            ws.Cell(salrows, 51).Value = "80C - Infrastructure Bonds";
                            ws.Cell(salrows, 52).Value = row["DeclarationId8"].ToString();
                            ws.Cell(salrows, 53).Value = row["DeclarationId8"].ToString();
                            ws.Cell(salrows, 54).Value = "80C - Interest on NSC Reinvested";
                            ws.Cell(salrows, 55).Value = row["DeclarationId9"].ToString();
                            ws.Cell(salrows, 56).Value = row["DeclarationId9"].ToString();
                            ws.Cell(salrows, 57).Value = "80C - Kisan Vikas Patra (KVP)";
                            ws.Cell(salrows, 58).Value = row["DeclarationId10"].ToString();
                            ws.Cell(salrows, 59).Value = row["DeclarationId10"].ToString();
                            ws.Cell(salrows, 60).Value = "80C - Life Insurance Premium";
                            ws.Cell(salrows, 61).Value = row["DeclarationId11"].ToString();
                            ws.Cell(salrows, 62).Value = row["DeclarationId11"].ToString();
                            ws.Cell(salrows, 63).Value = "80C - Long term Infrastructure Bonds";
                            ws.Cell(salrows, 64).Value = row["DeclarationId12"].ToString();
                            ws.Cell(salrows, 65).Value = row["DeclarationId12"].ToString();
                            ws.Cell(salrows, 66).Value = "80C - Mutual Funds";
                            ws.Cell(salrows, 67).Value = row["DeclarationId13"].ToString();
                            ws.Cell(salrows, 68).Value = row["DeclarationId13"].ToString();
                            ws.Cell(salrows, 69).Value = "80C - NABARD Rural Bonds";
                            ws.Cell(salrows, 70).Value = row["DeclarationId14"].ToString();
                            ws.Cell(salrows, 71).Value = row["DeclarationId14"].ToString();
                            ws.Cell(salrows, 72).Value = "80C - National Pension Scheme";
                            ws.Cell(salrows, 73).Value = row["DeclarationId15"].ToString();
                            ws.Cell(salrows, 74).Value = row["DeclarationId15"].ToString();
                            ws.Cell(salrows, 75).Value = "80C - Post office time deposit for 5 years";
                            ws.Cell(salrows, 76).Value = row["DeclarationId16"].ToString();
                            ws.Cell(salrows, 77).Value = row["DeclarationId16"].ToString();
                            ws.Cell(salrows, 78).Value = "80C - Pradhan Mantri Suraksha Bima Yojana";
                            ws.Cell(salrows, 79).Value = row["DeclarationId17"].ToString();
                            ws.Cell(salrows, 80).Value = row["DeclarationId17"].ToString();
                            ws.Cell(salrows, 81).Value = "80C - Public Provident Fund";
                            ws.Cell(salrows, 82).Value = row["DeclarationId18"].ToString();
                            ws.Cell(salrows, 83).Value = row["DeclarationId18"].ToString();
                            ws.Cell(salrows, 84).Value = "80C - Repayment of Housing loan(Principal amount)";
                            ws.Cell(salrows, 85).Value = row["DeclarationId19"].ToString();
                            ws.Cell(salrows, 86).Value = row["DeclarationId19"].ToString();
                            ws.Cell(salrows, 87).Value = "80C - Stamp duty and Registration charges";
                            ws.Cell(salrows, 88).Value = row["DeclarationId20"].ToString();
                            ws.Cell(salrows, 89).Value = row["DeclarationId20"].ToString();
                            ws.Cell(salrows, 90).Value = "80C - Sukanya Samriddhi Yojana";
                            ws.Cell(salrows, 91).Value = row["DeclarationId21"].ToString();
                            ws.Cell(salrows, 92).Value = row["DeclarationId21"].ToString();
                            ws.Cell(salrows, 93).Value = "80C - Unit Linked Insurance Premium (ULIP)";
                            ws.Cell(salrows, 94).Value = row["DeclarationId22"].ToString();
                            ws.Cell(salrows, 95).Value = row["DeclarationId22"].ToString();
                            ws.Cell(salrows, 96).Value = "80D - Preventive Health Checkup - Dependant Parents";
                            ws.Cell(salrows, 97).Value = row["DeclarationId23"].ToString();
                            ws.Cell(salrows, 98).Value = row["DeclarationId23"].ToString();
                            ws.Cell(salrows, 99).Value = "80D - Medical Bills - Very Senior Citizen";
                            ws.Cell(salrows, 100).Value = row["DeclarationId24"].ToString();
                            ws.Cell(salrows, 101).Value = row["DeclarationId24"].ToString();
                            ws.Cell(salrows, 102).Value = "80D - Medical Insurance Premium";
                            ws.Cell(salrows, 103).Value = row["DeclarationId25"].ToString();
                            ws.Cell(salrows, 104).Value = row["DeclarationId25"].ToString();
                            ws.Cell(salrows, 105).Value = "80D - Medical Insurance Premium - Dependant Parents";
                            ws.Cell(salrows, 106).Value = row["DeclarationId26"].ToString();
                            ws.Cell(salrows, 107).Value = row["DeclarationId26"].ToString();
                            ws.Cell(salrows, 108).Value = "80D - Preventive Health Check-up";
                            ws.Cell(salrows, 109).Value = row["DeclarationId27"].ToString();
                            ws.Cell(salrows, 110).Value = row["DeclarationId27"].ToString();
                            ws.Cell(salrows, 111).Value = "80EE - Additional Interest on housing loan borrowed as on 1st Apr 2016";
                            ws.Cell(salrows, 112).Value = row["DeclarationId28"].ToString();
                            ws.Cell(salrows, 113).Value = row["DeclarationId28"].ToString();
                            ws.Cell(salrows, 114).Value = "80CCD1(B) - Contribution to NPS 2015";
                            ws.Cell(salrows, 115).Value = row["DeclarationId29"].ToString();
                            ws.Cell(salrows, 116).Value = row["DeclarationId29"].ToString();
                            ws.Cell(salrows, 117).Value = "80DDB - Medical Treatment (Specified Disease only)- Senior Citizen";
                            ws.Cell(salrows, 118).Value = row["DeclarationId30"].ToString();
                            ws.Cell(salrows, 119).Value = row["DeclarationId30"].ToString();
                            ws.Cell(salrows, 120).Value = "80DDB - Medical Treatment (Specified Disease only)- Very Senior Citizen";
                            ws.Cell(salrows, 121).Value = row["DeclarationId31"].ToString();
                            ws.Cell(salrows, 122).Value = row["DeclarationId31"].ToString();
                            ws.Cell(salrows, 123).Value = "80TTB - Interest on Deposits in Savings Account, FDs, Post Office And Cooperative Society for Senior Citizen";
                            ws.Cell(salrows, 124).Value = row["DeclarationId32"].ToString();
                            ws.Cell(salrows, 125).Value = row["DeclarationId32"].ToString();
                            ws.Cell(salrows, 126).Value = "80G - Donation - 100% Exemption";
                            ws.Cell(salrows, 127).Value = row["DeclarationId33"].ToString();
                            ws.Cell(salrows, 128).Value = row["DeclarationId33"].ToString();
                            ws.Cell(salrows, 129).Value = "80G - Donation - 50% Exemption";
                            ws.Cell(salrows, 130).Value = row["DeclarationId34"].ToString();
                            ws.Cell(salrows, 131).Value = row["DeclarationId34"].ToString();
                            ws.Cell(salrows, 132).Value = "80G - Donation - Children Education";
                            ws.Cell(salrows, 133).Value = row["DeclarationId35"].ToString();
                            ws.Cell(salrows, 134).Value = row["DeclarationId35"].ToString();
                            ws.Cell(salrows, 135).Value = "80G - Donation - Political Parties";
                            ws.Cell(salrows, 136).Value = row["DeclarationId36"].ToString();
                            ws.Cell(salrows, 137).Value = row["DeclarationId36"].ToString();
                            ws.Cell(salrows, 138).Value = "80TTA - Interest on Deposits in Savings Account, FDs, Post Office And Cooperative Society";
                            ws.Cell(salrows, 139).Value = row["DeclarationId37"].ToString();
                            ws.Cell(salrows, 140).Value = row["DeclarationId37"].ToString();
                            ws.Cell(salrows, 141).Value = "80E - Interest on Loan of higher Self education";
                            ws.Cell(salrows, 142).Value = row["DeclarationId38"].ToString();
                            ws.Cell(salrows, 143).Value = row["DeclarationId38"].ToString();
                            ws.Cell(salrows, 144).Value = "80DD - Medical Treatment / Insurance of handicapped Dependant";
                            ws.Cell(salrows, 145).Value = row["DeclarationId39"].ToString();
                            ws.Cell(salrows, 146).Value = row["DeclarationId39"].ToString();
                            ws.Cell(salrows, 147).Value = "80DD - Medical Treatment / Insurance of handicapped Dependant (Severe)";
                            ws.Cell(salrows, 148).Value = row["DeclarationId40"].ToString();
                            ws.Cell(salrows, 149).Value = row["DeclarationId40"].ToString();
                            ws.Cell(salrows, 150).Value = "80DDB - Medical Treatment ( Specified Disease only )";
                            ws.Cell(salrows, 151).Value = row["DeclarationId41"].ToString();
                            ws.Cell(salrows, 152).Value = row["DeclarationId41"].ToString();
                            ws.Cell(salrows, 153).Value = "80U - Permanent Physical disability (Above 40%)";
                            ws.Cell(salrows, 154).Value = row["DeclarationId42"].ToString();
                            ws.Cell(salrows, 155).Value = row["DeclarationId42"].ToString();
                            ws.Cell(salrows, 156).Value = "80U - Permanent Physical disability (Below 40%)";
                            ws.Cell(salrows, 157).Value = row["DeclarationId43"].ToString();
                            ws.Cell(salrows, 158).Value = row["DeclarationId43"].ToString();
                            ws.Cell(salrows, 159).Value = "80CCG - Rajiv Gandhi Equity Scheme";
                            ws.Cell(salrows, 160).Value = row["DeclarationId44"].ToString();
                            ws.Cell(salrows, 161).Value = row["DeclarationId44"].ToString();
                            ws.Cell(salrows, 162).Value = "80GG - Deduction for Rent Paid";
                            ws.Cell(salrows, 163).Value = row["DeclarationId45"].ToString();
                            ws.Cell(salrows, 164).Value = row["DeclarationId45"].ToString();
                            ws.Cell(salrows, 165).Value = row["ITdecTotal"].ToString();
                            ws.Cell(salrows, 166).Value = row["TaxableIncome"].ToString();
                            ws.Cell(salrows, 167).Value = row["Tax"].ToString();
                            ws.Cell(salrows, 168).Value = row["col63"].ToString();
                            ws.Cell(salrows, 169).Value = "0";
                            ws.Cell(salrows, 170).Value = row["col65"].ToString();
                            ws.Cell(salrows, 171).Value = row["col66"].ToString();
                            ws.Cell(salrows, 172).Value = "0";
                            ws.Cell(salrows, 173).Value = "0";
                            ws.Cell(salrows, 174).Value = "0";
                            ws.Cell(salrows, 175).Value = "0";
                            ws.Cell(salrows, 176).Value = "0";
                            ws.Cell(salrows, 177).Value = "0";
                            ws.Cell(salrows, 178).Value = "0";
                            ws.Cell(salrows, 179).Value = row["col74"].ToString();
                            ws.Cell(salrows, 180).Value = row["col75"].ToString();
                            ws.Cell(salrows, 181).Value = "0";
                            ws.Cell(salrows, 182).Value = row["col77"].ToString();
                            ws.Cell(salrows, 183).Value = "0";
                            ws.Cell(salrows, 184).Value = "0";
                            ws.Cell(salrows, 185).Value = "0";
                            ws.Cell(salrows, 186).Value = "0";
                            ws.Cell(salrows, 187).Value = "0";
                            ws.Cell(salrows, 188).Value = "0";
                            ws.Cell(salrows, 189).Value = "0";
                            ws.Cell(salrows, 190).Value = "0";
                            ws.Cell(salrows, 191).Value = "0";
                            ws.Cell(salrows, 192).Value = "0";
                            ws.Cell(salrows, 193).Value = "0";
                            ws.Cell(salrows, 194).Value = "0";
                            ws.Cell(salrows, 195).Value = "0";
                            ws.Cell(salrows, 196).Value = "0";
                            ws.Cell(salrows, 197).Value = "0";
                            salrows++;
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=AnnexureSalary.xls");
                        return File(workStream, fileType);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult LedgerBalance(int AccId, string fdate, string tdate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                using (var context = new IOASDBEntities())
                {
                    string AccountHead = context.tblAccountHead.Where(m => m.AccountHeadId == AccId).Select(m => m.AccountHead).FirstOrDefault();
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataSet dataset = new DataSet();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetLedgerBalDebit(AccId, fromdate, todate);
                        DataTable dtResult1 = new DataTable();
                        dtResult1 = db.GetLedgerBalCredit(AccId, fromdate, todate);
                        var Qry = db.GetLedgerBalAmt(AccId, fromdate, todate);
                        decimal crOB = 0; decimal drOB = 0; decimal crCB = 0; decimal drCB = 0;
                        if (Qry.Item3 > Qry.Item4)
                            crOB = Qry.Item3 - Qry.Item4;
                        else
                            drOB = Qry.Item4 - Qry.Item3;
                        decimal ttlCr = Qry.Item1 + crOB;
                        decimal ttlDr = Qry.Item2 + drOB;

                        if (ttlCr > ttlDr)
                            crCB = ttlCr - ttlDr;
                        else
                            drCB = ttlDr - ttlCr;


                        var ws = wb.Worksheets.Add("Ledger Balance");
                        ws.Cell(1, 1).Value = "Ledger Balance ( " + fromdate + " to " + todate + " )";
                        ws.Range("A1:C1").Row(1).Merge();
                        ws.Range("A1:O1").Style.Font.Bold = true;
                        ws.Cell(2, 1).Value = AccountHead;
                        ws.Range("A2:C2").Row(1).Merge();
                        ws.Range("A2:O2").Style.Font.Bold = true;
                        ws.Cell(4, 1).Value = "Debit";
                        ws.Range("A4:D4").Row(1).Merge();
                        ws.Range("A4:O4").Style.Font.Bold = true;
                        ws.Range("A5:O5").Style.Font.Bold = true;

                        ws.Cell(5, 1).Value = "Date";
                        ws.Cell(5, 2).Value = "Reference Number";
                        ws.Cell(5, 3).Value = "Particulars";
                        ws.Cell(5, 4).Value = "Amount";
                        var rngTitle = ws.Range("E5:F5000");
                        rngTitle.Style.Fill.BackgroundColor = XLColor.Rose;
                        if (drOB > 0)
                        {
                            ws.Cell(6, 3).Value = "Opening Balance";
                            ws.Cell(6, 4).Value = drOB;
                        }
                        ws.Cell(4, 7).Value = "Credit";
                        ws.Range("G4:J4").Row(1).Merge();
                        ws.Cell(5, 7).Value = "Date";
                        ws.Cell(5, 8).Value = "Reference Number";
                        ws.Cell(5, 9).Value = "Particulars";
                        ws.Cell(5, 10).Value = "Amount";
                        if (crOB > 0)
                        {
                            ws.Cell(6, 9).Value = "Opening Balance";
                            ws.Cell(6, 10).Value = crOB;
                        }

                        int Debitrow = 7;

                        DataView dataview = dtResult.DefaultView;
                        dataview.Sort = "PostedDate  ASC";
                        DataTable dt = dataview.ToTable();

                        DataView dataview1 = dtResult1.DefaultView;
                        dataview1.Sort = "PostedDate  ASC";
                        DataTable dt1 = dataview1.ToTable();

                        foreach (DataRow row in dt.Rows)
                        {

                            ws.Cell(Debitrow, 1).Value = row["PostedDate"].ToString();
                            ws.Cell(Debitrow, 2).Value = row["RefNumber"].ToString();
                            ws.Cell(Debitrow, 3).Value = row["FunctionName"].ToString();
                            ws.Cell(Debitrow, 4).Value = row["Amount"].ToString();
                            Debitrow++;
                        }
                        int Creditrow = 7;
                        foreach (DataRow row in dt1.Rows)
                        {
                            ws.Cell(Creditrow, 7).Value = row["PostedDate"].ToString();
                            ws.Cell(Creditrow, 8).Value = row["RefNumber"].ToString();
                            ws.Cell(Creditrow, 9).Value = row["FunctionName"].ToString();
                            ws.Cell(Creditrow, 10).Value = row["Amount"].ToString();
                            Creditrow++;
                        }

                        if (crCB > drCB)
                        {
                            ws.Cell(Debitrow, 3).Value = "Closing Balance";
                            ws.Cell(Debitrow, 4).Value = crCB;
                            ws.Cell(Debitrow + 1, 3).Value = "Total";
                            ws.Cell(Debitrow + 1, 4).Value = ttlDr + (crCB);
                            ws.Cell(Creditrow + 1, 9).Value = "Total";
                            ws.Cell(Creditrow + 1, 10).Value = ttlCr;
                        }
                        else
                        {

                            ws.Cell(Debitrow + 1, 3).Value = "Total";
                            ws.Cell(Debitrow + 1, 4).Value = ttlDr;
                            ws.Cell(Creditrow, 9).Value = "Closing Balance";
                            ws.Cell(Creditrow, 10).Value = drCB;
                            ws.Cell(Creditrow + 1, 9).Value = "Total";
                            ws.Cell(Creditrow + 1, 10).Value = ttlCr + (drCB);
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=LedgerBalance.xls");
                        return File(workStream, fileType);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public FileStreamResult BalanceSheet1(int Finyear)
        //{
        //    try
        //    {
        //        decimal CurrSource = 0; decimal PrevSource = 0;
        //        decimal CurrApp = 0; decimal PrevApp = 0;
        //        MemoryStream workStream = new MemoryStream();
        //        byte[] byteInfo = workStream.ToArray();
        //        workStream.Write(byteInfo, 0, byteInfo.Length);
        //        using (var context = new IOASDBEntities())
        //        {
        //            using (XLWorkbook wb = new XLWorkbook())
        //            {
        //                ListDatabaseObjects db = new ListDatabaseObjects();
        //                DataSet dataset = new DataSet();
        //                DataTable dtResult = new DataTable();
        //                var CurrYearQry = context.tblFinYear.Where(m => m.FinYearId == Finyear).FirstOrDefault();
        //                DateTime CurrFromDate = Convert.ToDateTime(CurrYearQry.StartDate);
        //                DateTime CurrToDate = Convert.ToDateTime(CurrYearQry.EndDate);
        //                DateTime Date = CurrYearQry.StartDate.Value.AddDays(-1);
        //                var PrevYearQry = context.tblFinYear.Where(m => m.EndDate == Date).FirstOrDefault();
        //                DateTime PrevFromDate = Convert.ToDateTime(PrevYearQry.StartDate);
        //                DateTime PrevToDate = Convert.ToDateTime(PrevYearQry.EndDate);
        //                var ws = wb.Worksheets.Add("BALANCE SHEET ");


        //                decimal CurrRec = 0; decimal PrevRec = 0;
        //                var ws1 = wb.Worksheets.Add("INCOME & EXPENDITURE");
        //                ws1.Cell(1, 1).Value = "INDUSTRIAL CONSULTANCY  & SPONSORED RESEARCH";
        //                ws1.Range("A1:C1").Row(1).Merge();
        //                ws1.Range("A1:O1").Style.Font.Bold = true;
        //                ws1.Range("A4:O4").Style.Font.Bold = true;
        //                ws1.Range("A1:A100").Style.Font.Bold = true;
        //                ws1.Cell(4, 1).Value = "PARTICULARS";
        //                ws1.Cell(5, 1).Value = "INCOME:";
        //                ws1.Cell(5, 2).Value = CurrYearQry.Year;
        //                ws1.Cell(5, 3).Value = PrevYearQry.Year;

        //                ws1.Cell(6, 1).Value = "Invoice against Receipt:";
        //                var CurrIR = db.GetBalanceSheetReceipt(1, CurrFromDate, CurrToDate);
        //                ws1.Cell(6, 2).Value = CurrIR.Item1;
        //                CurrRec = CurrRec + CurrIR.Item1;
        //                var PrevIR = db.GetBalanceSheetReceipt(1, PrevFromDate, PrevToDate);
        //                ws1.Cell(6, 3).Value = PrevIR.Item1;
        //                PrevRec = PrevRec + PrevIR.Item1;
        //                ws1.Cell(7, 1).Value = "Other Receipt:";
        //                var CurrOR = db.GetBalanceSheetReceipt(2, CurrFromDate, CurrToDate);
        //                ws1.Cell(7, 2).Value = CurrOR.Item1;
        //                CurrRec = CurrRec + CurrOR.Item1;
        //                var PrevOR = db.GetBalanceSheetReceipt(2, PrevFromDate, PrevToDate);
        //                ws1.Cell(7, 3).Value = PrevOR.Item1;
        //                PrevRec = PrevRec + PrevOR.Item1;

        //                ws1.Cell(8, 1).Value = "Back end Receipt:";
        //                var CurrBR = db.GetBalanceSheetReceipt(3, CurrFromDate, CurrToDate);
        //                ws1.Cell(8, 2).Value = CurrBR.Item1;
        //                CurrRec = CurrRec + CurrBR.Item1;
        //                var PrevBR = db.GetBalanceSheetReceipt(3, PrevFromDate, PrevToDate);
        //                ws1.Cell(8, 3).Value = PrevBR.Item1;
        //                PrevRec = PrevRec + PrevBR.Item1;

        //                ws1.Cell(9, 1).Value = "TOTAl(A)";
        //                ws1.Cell(9, 2).Value = CurrRec;
        //                ws1.Cell(9, 3).Value = PrevRec;

        //                decimal CurrExp = 0; decimal PrevExp = 0;
        //                ws1.Cell(11, 1).Value = "EXPENDITURE:";
        //                ws1.Cell(12, 1).Value = "STAFF SALARY:";
        //                var currStaff = db.GetBalanceSheetEXP(1, CurrFromDate, CurrToDate);
        //                ws1.Cell(12, 2).Value = currStaff.Item1;
        //                CurrExp = CurrExp + currStaff.Item1;
        //                var PrevStaff = db.GetBalanceSheetEXP(1, PrevFromDate, PrevToDate);
        //                ws1.Cell(12, 3).Value = PrevStaff.Item1;
        //                PrevExp = PrevExp + PrevStaff.Item1;
        //                ws1.Cell(13, 1).Value = "CONSUMBLES";
        //                var currCons = db.GetBalanceSheetEXP(2, CurrFromDate, CurrToDate);
        //                ws1.Cell(13, 2).Value = currCons.Item1;
        //                CurrExp = CurrExp + currCons.Item1;
        //                var PrevCons = db.GetBalanceSheetEXP(2, PrevFromDate, PrevToDate);
        //                ws1.Cell(13, 3).Value = PrevCons.Item1;
        //                PrevExp = PrevExp + PrevCons.Item1;
        //                ws1.Cell(14, 1).Value = "CONTINGENCY";
        //                var currCon = db.GetBalanceSheetEXP(3, CurrFromDate, CurrToDate);
        //                ws1.Cell(14, 2).Value = currCon.Item1;
        //                CurrExp = CurrExp + currCon.Item1;
        //                var PrevCon = db.GetBalanceSheetEXP(3, PrevFromDate, PrevToDate);
        //                ws1.Cell(14, 3).Value = PrevCon.Item1;
        //                PrevExp = PrevExp + PrevCon.Item1;
        //                ws1.Cell(15, 1).Value = "TRAVEL";
        //                var currTra = db.GetBalanceSheetEXP(4, CurrFromDate, CurrToDate);
        //                ws1.Cell(15, 2).Value = currTra.Item1;
        //                CurrExp = CurrExp + currTra.Item1;
        //                var PrevTra = db.GetBalanceSheetEXP(4, PrevFromDate, PrevToDate);
        //                ws1.Cell(15, 3).Value = PrevTra.Item1;
        //                PrevExp = PrevExp + PrevTra.Item1;
        //                ws1.Cell(15, 1).Value = "COMPONENTS";
        //                var currCom = db.GetBalanceSheetEXP(5, CurrFromDate, CurrToDate);
        //                ws1.Cell(15, 2).Value = currCom.Item1;
        //                CurrExp = CurrExp + currCom.Item1;
        //                var PrevCom = db.GetBalanceSheetEXP(5, PrevFromDate, PrevToDate);
        //                ws1.Cell(15, 3).Value = PrevCom.Item1;
        //                PrevExp = PrevExp + PrevCom.Item1;

        //                ws1.Cell(16, 1).Value = "OVERHEADS";
        //                var currOver = db.GetBalanceSheetEXP(6, CurrFromDate, CurrToDate);
        //                ws1.Cell(16, 2).Value = currOver.Item1;
        //                CurrExp = CurrExp + currOver.Item1;
        //                var PrevOver = db.GetBalanceSheetEXP(6, PrevFromDate, PrevToDate);
        //                ws1.Cell(16, 3).Value = PrevOver.Item1;
        //                PrevExp = PrevExp + PrevOver.Item1;

        //                ws1.Cell(17, 1).Value = "EQUIPMENT";
        //                var currEqu = db.GetBalanceSheetEXP(7, CurrFromDate, CurrToDate);
        //                ws1.Cell(17, 2).Value = currEqu.Item1;
        //                CurrExp = CurrExp + currEqu.Item1;
        //                var PrevEqu = db.GetBalanceSheetEXP(7, PrevFromDate, PrevToDate);
        //                ws1.Cell(17, 3).Value = PrevEqu.Item1;
        //                PrevExp = PrevExp + PrevEqu.Item1;

        //                ws1.Cell(18, 1).Value = "OTHER EXPENSES";
        //                var currOth = db.GetBalanceSheetEXP(8, CurrFromDate, CurrToDate);
        //                ws1.Cell(18, 2).Value = currOth.Item1;
        //                CurrExp = CurrExp + currOth.Item1;
        //                var PrevOth = db.GetBalanceSheetEXP(8, PrevFromDate, PrevToDate);
        //                ws1.Cell(18, 3).Value = PrevOth.Item1;
        //                PrevExp = PrevExp + PrevOth.Item1;

        //                ws1.Cell(19, 1).Value = "MISCELLANEOUS";
        //                ws1.Cell(19, 2).Value = currOth.Item2;
        //                CurrExp = CurrExp + currOth.Item2;
        //                ws1.Cell(19, 3).Value = PrevOth.Item2;
        //                PrevExp = PrevExp + PrevOth.Item2;

        //                ws1.Cell(20, 1).Value = "Total(B)";
        //                ws1.Cell(20, 2).Value = CurrExp;
        //                ws1.Cell(20, 3).Value = PrevExp;

        //                ws1.Cell(21, 1).Value = "BALANCE BEING SURPLUS /(DEFICIT) CARRIED TO CAPITAL FUND (A-B)";
        //                ws1.Cell(21, 2).Value = CurrRec - CurrExp;
        //                ws1.Cell(21, 3).Value = PrevRec - PrevExp;

        //                ws.Cell(1, 1).Value = "INDUSTRIAL CONSULTANCY  & SPONSORED RESEARCH";
        //                ws.Range("A1:C1").Row(1).Merge();
        //                ws.Range("A1:O1").Style.Font.Bold = true;
        //                ws.Range("A4:O4").Style.Font.Bold = true;
        //                ws.Range("A1:A100").Style.Font.Bold = true;
        //                ws.Cell(4, 1).Value = "SOURCES OF FUNDS";
        //                ws.Cell(4, 2).Value = CurrYearQry.Year;
        //                ws.Cell(4, 3).Value = PrevYearQry.Year;
        //                ws.Cell(5, 1).Value = "CAPITAL FUND:";
        //                ws.Cell(6, 1).Value = "OPENING BALANCE";
        //                ws.Cell(6, 2).Value = "0";
        //                ws.Cell(6, 3).Value = "0";
        //                ws.Cell(7, 1).Value = "SURPLUS";
        //                ws.Cell(7, 2).Value = CurrRec - CurrExp;
        //                CurrSource = CurrSource + (CurrRec - CurrExp);
        //                ws.Cell(7, 3).Value = PrevRec - PrevExp;
        //                PrevSource = PrevSource + (PrevRec - PrevExp);
        //                ws.Cell(8, 1).Value = "Non Current Liability";
        //                ws.Cell(9, 1).Value = "Project Funds";
        //                var currNCL = db.GetBalanceSheetSource(20, CurrFromDate, CurrToDate);
        //                ws.Cell(9, 2).Value = currNCL.Item1;
        //                CurrSource = CurrSource + currNCL.Item1;
        //                var PrevNCL = db.GetBalanceSheetSource(20, PrevFromDate, PrevToDate);
        //                ws.Cell(9, 3).Value = PrevNCL.Item1;
        //                PrevSource = PrevSource + PrevNCL.Item1;

        //                ws.Cell(10, 1).Value = "Current Liability";
        //                ws.Cell(11, 1).Value = "Duties and Taxes";
        //                var currDT = db.GetBalanceSheetSource(11, CurrFromDate, CurrToDate);
        //                ws.Cell(11, 2).Value = currDT.Item1;
        //                CurrSource = CurrSource + currDT.Item1;
        //                var PrevDT = db.GetBalanceSheetSource(11, PrevFromDate, PrevToDate);
        //                ws.Cell(11, 3).Value = PrevDT.Item1;
        //                PrevSource = PrevSource + PrevDT.Item1;

        //                ws.Cell(12, 1).Value = "Expenses Payable";
        //                var currEP = db.GetBalanceSheetSource(18, CurrFromDate, CurrToDate);
        //                ws.Cell(12, 2).Value = currEP.Item1;
        //                CurrSource = CurrSource + currEP.Item1;
        //                var PrevEP = db.GetBalanceSheetSource(18, PrevFromDate, PrevToDate);
        //                ws.Cell(12, 3).Value = PrevEP.Item1;
        //                PrevSource = PrevSource + PrevEP.Item1;

        //                ws.Cell(13, 1).Value = " Creditor";
        //                var currC = db.GetBalanceSheetSource(19, CurrFromDate, CurrToDate);
        //                ws.Cell(13, 2).Value = currC.Item1;
        //                CurrSource = CurrSource + currC.Item1;
        //                var PrevC = db.GetBalanceSheetSource(19, PrevFromDate, PrevToDate);
        //                ws.Cell(13, 3).Value = PrevC.Item1;
        //                PrevSource = PrevSource + PrevC.Item1;

        //                ws.Cell(14, 1).Value = "Total";
        //                ws.Cell(14, 2).Value = CurrSource;
        //                ws.Cell(14, 3).Value = PrevSource;


        //                ws.Cell(16, 1).Value = "APPLICATION OF FUNDS";
        //                ws.Cell(16, 2).Value = CurrYearQry.Year;
        //                ws.Cell(16, 3).Value = PrevYearQry.Year;
        //                ws.Cell(17, 1).Value = "FIXED ASSETS:";
        //                ws.Cell(18, 1).Value = "TANGIBLE ASSETS";
        //                var currTA = db.GetBalanceSheetApp(5, CurrFromDate, CurrToDate);
        //                ws.Cell(18, 2).Value = currTA.Item1;
        //                CurrApp = CurrApp + currTA.Item1;
        //                var PrevTA = db.GetBalanceSheetApp(5, PrevFromDate, PrevToDate);
        //                ws.Cell(18, 3).Value = PrevTA.Item1;
        //                PrevApp = PrevApp + PrevTA.Item1;

        //                ws.Cell(19, 1).Value = "INTANGIBLE ASSETS";
        //                var currITA = db.GetBalanceSheetApp(6, CurrFromDate, CurrToDate);
        //                ws.Cell(19, 2).Value = currITA.Item1;
        //                CurrApp = CurrApp + currITA.Item1;
        //                var PrevITA = db.GetBalanceSheetApp(6, PrevFromDate, PrevToDate);
        //                ws.Cell(19, 3).Value = PrevITA.Item1;
        //                PrevApp = PrevApp + PrevITA.Item1;

        //                ws.Cell(20, 1).Value = "Non Current Assets";
        //                ws.Cell(21, 1).Value = "Investments";
        //                var currI = db.GetBalanceSheetApp(7, CurrFromDate, CurrToDate);
        //                ws.Cell(21, 2).Value = currI.Item1;
        //                CurrApp = CurrApp + currI.Item1;
        //                var PrevI = db.GetBalanceSheetApp(7, PrevFromDate, PrevToDate);
        //                ws.Cell(21, 3).Value = PrevI.Item1;
        //                PrevApp = PrevApp + PrevI.Item1;

        //                ws.Cell(22, 1).Value = "Project Payable Fund";
        //                var currPF = db.GetBalanceSheetApp(33, CurrFromDate, CurrToDate);
        //                ws.Cell(22, 2).Value = currPF.Item1;
        //                CurrApp = CurrApp + currPF.Item1;
        //                var PrevPF = db.GetBalanceSheetApp(33, PrevFromDate, PrevToDate);
        //                ws.Cell(22, 3).Value = PrevPF.Item1;
        //                PrevApp = PrevApp + PrevPF.Item1;

        //                ws.Cell(23, 1).Value = "Current Assets";
        //                ws.Cell(24, 1).Value = "Accounts Receivable";
        //                var currAR = db.GetBalanceSheetApp(34, CurrFromDate, CurrToDate);
        //                ws.Cell(24, 2).Value = currAR.Item1;
        //                CurrApp = CurrApp + currAR.Item1;
        //                var PrevAR = db.GetBalanceSheetApp(34, PrevFromDate, PrevToDate);
        //                ws.Cell(24, 3).Value = PrevAR.Item1;
        //                PrevApp = PrevApp + PrevAR.Item1;

        //                ws.Cell(25, 1).Value = "Project Negative Balance ";
        //                var currPNB = db.GetBalanceSheetApp(35, CurrFromDate, CurrToDate);
        //                ws.Cell(25, 2).Value = currPNB.Item1;
        //                CurrApp = CurrApp + currPNB.Item1;
        //                var PrevPNB = db.GetBalanceSheetApp(35, PrevFromDate, PrevToDate);
        //                ws.Cell(25, 3).Value = PrevPNB.Item1;
        //                PrevApp = PrevApp + PrevPNB.Item1;

        //                ws.Cell(26, 1).Value = "Project Advance";
        //                var currPA = db.GetBalanceSheetApp(36, CurrFromDate, CurrToDate);
        //                ws.Cell(26, 2).Value = currPA.Item1;
        //                CurrApp = CurrApp + currPA.Item1;
        //                var PrevPA = db.GetBalanceSheetApp(36, PrevFromDate, PrevToDate);
        //                ws.Cell(26, 3).Value = PrevPA.Item1;
        //                PrevApp = PrevApp + PrevPA.Item1;

        //                ws.Cell(27, 1).Value = "Other Advances";
        //                var currOA = db.GetBalanceSheetApp(37, CurrFromDate, CurrToDate);
        //                ws.Cell(27, 2).Value = currOA.Item1;
        //                CurrApp = CurrApp + currOA.Item1;
        //                var PrevOA = db.GetBalanceSheetApp(37, PrevFromDate, PrevToDate);
        //                ws.Cell(27, 3).Value = PrevOA.Item1;
        //                PrevApp = PrevApp + PrevOA.Item1;

        //                ws.Cell(28, 1).Value = "Bank Accounts";
        //                var currBA = db.GetBalanceSheetApp(38, CurrFromDate, CurrToDate);
        //                ws.Cell(28, 2).Value = currBA.Item1;
        //                CurrApp = CurrApp + currBA.Item1;
        //                var PrevBA = db.GetBalanceSheetApp(38, PrevFromDate, PrevToDate);
        //                ws.Cell(28, 3).Value = PrevBA.Item1;
        //                PrevApp = PrevApp + PrevBA.Item1;

        //                ws.Cell(29, 1).Value = "Cash";
        //                var currCa = db.GetBalanceSheetApp(39, CurrFromDate, CurrToDate);
        //                ws.Cell(29, 2).Value = currCa.Item1;
        //                CurrApp = CurrApp + currCa.Item1;
        //                var PrevCa = db.GetBalanceSheetApp(39, PrevFromDate, PrevToDate);
        //                ws.Cell(29, 3).Value = PrevCa.Item1;
        //                PrevApp = PrevApp + PrevCa.Item1;

        //                ws.Cell(30, 1).Value = "Total";
        //                ws.Cell(30, 2).Value = CurrApp;
        //                ws.Cell(30, 3).Value = PrevApp;


        //                DataTable dtResult0 = new DataTable();
        //                dtResult0 = db.GetBalanceSheetMisc(CurrFromDate, CurrToDate);
        //                DataTable dtResult1 = new DataTable();
        //                dtResult1 = db.GetBalanceSheetMisc(PrevFromDate, PrevToDate);
        //                var ws2 = wb.Worksheets.Add(CurrYearQry.Year + " - Misc");
        //                ws2.Cell(1, 1).Value = "BillNumber";
        //                ws2.Cell(1, 2).Value = "ProjectNumber";
        //                ws2.Cell(1, 3).Value = "HeadName";
        //                ws2.Cell(1, 4).Value = "AmountSpent";
        //                ws2.Cell(1, 5).Value = "CommitmentDate";
        //                int CurrYearrow = 2;
        //                foreach (DataRow row in dtResult0.Rows)
        //                {
        //                    ws2.Cell(CurrYearrow, 1).Value = row["BillNumber"].ToString();
        //                    ws2.Cell(CurrYearrow, 2).Value = row["ProjectNumber"].ToString();
        //                    ws2.Cell(CurrYearrow, 3).Value = row["HeadName"].ToString();
        //                    ws2.Cell(CurrYearrow, 4).Value = row["AmountSpent"].ToString();
        //                    ws2.Cell(CurrYearrow, 5).Value = row["CommitmentDate"].ToString();
        //                    CurrYearrow++;
        //                }
        //                var ws3 = wb.Worksheets.Add(PrevYearQry.Year + " - Misc");
        //                ws3.Cell(1, 1).Value = "BillNumber";
        //                ws3.Cell(1, 2).Value = "ProjectNumber";
        //                ws3.Cell(1, 3).Value = "HeadName";
        //                ws3.Cell(1, 4).Value = "AmountSpent";
        //                ws3.Cell(1, 5).Value = "CommitmentDate";
        //                int PrevYearrow = 2;
        //                foreach (DataRow row in dtResult1.Rows)
        //                {

        //                    ws3.Cell(PrevYearrow, 1).Value = row["BillNumber"].ToString();
        //                    ws3.Cell(PrevYearrow, 2).Value = row["ProjectNumber"].ToString();
        //                    ws3.Cell(PrevYearrow, 3).Value = row["HeadName"].ToString();
        //                    ws3.Cell(PrevYearrow, 4).Value = row["AmountSpent"].ToString();
        //                    ws3.Cell(PrevYearrow, 5).Value = row["CommitmentDate"].ToString();
        //                    PrevYearrow++;
        //                }
        //                wb.SaveAs(workStream);
        //                workStream.Position = 0;
        //                string fileType = Common.GetMimeType("xls");
        //                Response.AddHeader("Content-Disposition", "filename=BalanceSheet.xls");
        //                return File(workStream, fileType);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public FileStreamResult TrailBalance(string tdate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                //var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                todate = todate.AddDays(1).AddTicks(-2001);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataTable dtResult = new DataTable();
                        dtResult = db.GetTrailBalance(todate);
                        decimal ttlCr = dtResult.AsEnumerable().Sum(c => c.Field<decimal>("Credit"));
                        decimal ttlDr = dtResult.AsEnumerable().Sum(c => c.Field<decimal>("Debit"));
                        var ws = wb.Worksheets.Add("Trial Balance");
                        ws.Cell(1, 1).Value = "Trial Balance ( As on " + todate + " )";
                        ws.Range("A1:C1").Row(1).Merge();
                        ws.Range("A1:O1").Style.Font.Bold = true;
                        ws.Range("A4:O4").Style.Font.Bold = true;
                        ws.Cell(4, 1).Value = "Accounts";
                        ws.Cell(4, 2).Value = "Groups";
                        ws.Cell(4, 3).Value = "AccountHead";
                        ws.Cell(4, 4).Value = "Debit";
                        ws.Cell(4, 5).Value = "Credit";
                        int trailrow = 5;
                        foreach (DataRow row in dtResult.Rows)
                        {

                            ws.Cell(trailrow, 1).Value = row["Accounts"].ToString();
                            ws.Cell(trailrow, 2).Value = row["Groups"].ToString();
                            ws.Cell(trailrow, 3).Value = row["AccountHead"].ToString();
                            ws.Cell(trailrow, 4).Value = row["Debit"].ToString();
                            ws.Cell(trailrow, 5).Value = row["Credit"].ToString();
                            trailrow++;
                        }
                        //var Qry = db.GetTrailBalanceTotal(todate);
                        ws.Cell(trailrow, 3).Value = "Total";
                        ws.Cell(trailrow, 4).Value = ttlDr;
                        ws.Cell(trailrow, 5).Value = ttlCr;
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=TrailBalance.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult RFreport(int FinYear)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        RFreportModel model = new RFreportModel();
                        model = ReportService.GetAngencywiseRF(FinYear);
                        RFreportModel model1 = new RFreportModel();
                        model1 = ReportService.GetDepartwiseRF(FinYear);
                        RFreportModel model2 = new RFreportModel();
                        model2 = ReportService.GetRFsummary(FinYear);


                        var ws = wb.Worksheets.Add("Exp_Summary_RF");
                        ws.Cell(1, 1).Value = "Research Fund Expenditure Summary";
                        ws.Range("A1:G1").Row(1).Merge();
                        ws.Range("A1:A1").Style.Font.Bold = true;
                        ws.Cell(2, 1).Value = "Spent Year";
                        ws.Range("A2:A2").Style.Font.Bold = true;
                        ws.Range("A2:G2").Row(1).Merge();
                        ws.Cell(3, 1).Value = "Project Year";
                        ws.Range("A3:Z3").Style.Font.Bold = true;

                        int Yearsrow = 2;
                        for (int a = 0; a < model2.Yearlist.Count(); a++)
                        {

                            ws.Cell(3, Yearsrow).Value = model2.Yearlist[a].Year; ;
                            Yearsrow++;
                        }
                        //ws.Cell(3, Yearsrow).Value = "Grand Total";
                        int firstrows = 4;
                        for (int b = 0; b < model2.Yearlist.Count(); b++)
                        {
                            int firstcols = 2;
                            ws.Cell(firstrows, 1).Value = model2.Yearlist[b].Year;
                            ws.Range("A" + firstrows + ":A" + firstrows).Style.Font.Bold = true;
                            for (int c = 0; c < model2.YearValuelist[b].values.Count(); c++)
                            {
                                ws.Cell(firstrows, firstcols).Value = String.Format("{0:0.00}", model2.YearValuelist[b].values[c]);
                                firstcols++;
                            }
                            firstrows++;
                        }

                        ws.Range("A1:A50").Style.Font.Bold = true;


                        var ws1 = wb.Worksheets.Add("Agency Wise_RF");
                        ws1.Cell(1, 1).Value = "INDIAN INSTITUTE OF TECHNOLOGY MADRAS";
                        ws1.Range("A1:G1").Row(1).Merge();
                        ws1.Range("A1:A1").Style.Font.Bold = true;
                        ws1.Cell(2, 1).Value = "Research Fund Department wise Summary";
                        ws1.Range("A2:A2").Style.Font.Bold = true;
                        ws1.Range("A2:G2").Row(1).Merge();

                        int Yearsrows = 2;

                        for (int d = 0; d < model2.Yearlist.Count(); d++)
                        {

                            ws1.Cell(3, Yearsrows).Value = model2.Yearlist[d].Year;
                            Yearsrows = Yearsrows + 2;
                            Yearsrows++;
                        }

                        //  ws1.Cell(3, Yearsrows).Value = "Grand Total in lakhs";
                        ws1.Range("A3:AZ3").Style.Font.Bold = true;
                        ws1.Cell(4, 1).Value = "Agency Code";
                        ws1.Range("A4:AZ4").Style.Font.Bold = true;
                        int Agencylabelrow = 2;
                        //int AgencyCount = Years.Length + 3;

                        for (int e = 0; e < model2.Yearlist.Count(); e++)
                        {
                            ws1.Cell(4, Agencylabelrow).Value = "No.";
                            Agencylabelrow = Agencylabelrow + 1;
                            ws1.Cell(4, Agencylabelrow).Value = "Sanction Amt in lakhs";
                            Agencylabelrow = Agencylabelrow + 1;
                            ws1.Cell(4, Agencylabelrow).Value = "Act.Exp in lakhs";
                            //  e = e + 2;
                            Agencylabelrow++;
                        }



                        int Agencyrow = 5;


                        for (int f = 0; f < model.Agencylist.Count(); f++)
                        {
                            int Agencycol = 2;
                            ws1.Cell(Agencyrow, 1).Value = model.Agencylist[f].Agency;
                            ws1.Range("A" + Agencyrow + ":A" + Agencyrow).Style.Font.Bold = true;
                            for (int g = 0; g < model.AgencyValuelist[f].values.Count(); g++)
                            {
                                ws1.Cell(Agencyrow, Agencycol).Value = String.Format("{0:0.00}", model.AgencyValuelist[f].values[g]);
                                Agencycol++;
                            }
                            Agencyrow++;
                        }

                        ws1.Range("A1:A50").Style.Font.Bold = true;


                        var ws2 = wb.Worksheets.Add("Department Wise_RF");
                        ws2.Cell(1, 1).Value = "INDIAN INSTITUTE OF TECHNOLOGY MADRAS";
                        ws2.Range("A1:AB1").Row(1).Merge();
                        ws2.Range("A1:A50").Style.Font.Bold = true;
                        ws2.Cell(2, 1).Value = "Research Fund Department wise Summary";
                        ws2.Range("A2:AB2").Row(1).Merge();
                        ws2.Range("A2:AZ2").Style.Font.Bold = true;

                        //ws2.Cell(3, 1).Value = "NFIG";
                        //ws2.Range("A3:D3").Row(1).Merge();
                        ws2.Range("A3:AZ3").Style.Font.Bold = true;



                        int ws2Agencyrows = 2;

                        for (int h = 0; h < model.Agencylist.Count(); h++)
                        {
                            string AgencyCode = model.Agencylist[h].Agency;
                            ws2.Cell(3, ws2Agencyrows).Value = AgencyCode;
                            ws2Agencyrows = ws2Agencyrows + 2;
                            ws2Agencyrows++;
                        }
                        // ws2.Cell(3, ws2Agencyrows + 2).Value = "Grand Total in lakhs";
                        ws2.Range("A3:AZ3").Style.Font.Bold = true;
                        ws2.Cell(4, 1).Value = "Department";
                        ws2.Range("A4:AZ4").Style.Font.Bold = true;

                        int Deparrow = 2;
                        //int DepaCount = Agency.Length + 3;

                        for (int i = 1; i < model.Agencylist.Count(); i++)
                        {
                            ws2.Cell(4, Deparrow).Value = "No.";
                            Deparrow = Deparrow + 1;
                            ws2.Cell(4, Deparrow).Value = "PC in lakhs";
                            Deparrow = Deparrow + 1;
                            ws2.Cell(4, Deparrow).Value = "Exp in lakhs";
                            // i = i + 2;
                            Deparrow++;
                        }

                        ws2.Cell(4, Deparrow).Value = "No.";
                        Deparrow = Deparrow + 1;
                        ws2.Cell(4, Deparrow).Value = "PC in lakhs";
                        Deparrow = Deparrow + 1;
                        ws2.Cell(4, Deparrow).Value = "Exp in lakhs";
                        int Deprow = 5;


                        for (int j = 0; j < model1.Departlist.Count(); j++)
                        {
                            int Depcol = 2;
                            ws2.Cell(Deprow, 1).Value = model1.Departlist[j].Departments;
                            ws2.Range("A" + Deprow + ":A" + Deprow).Style.Font.Bold = true;
                            for (int k = 0; k < model1.DepartValuelist[j].values.Count(); k++)
                            {
                                ws2.Cell(Deprow, Depcol).Value = String.Format("{0:0.00}", model1.DepartValuelist[j].values[k]);
                                Depcol++;
                            }
                            Deprow++;
                        }
                        ws2.Range("A1:A50").Style.Font.Bold = true;


                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataTable dtResultExp = new DataTable();
                        DataTable dtResultProj = new DataTable();
                        string Financialyear = context.tblFinYear.Where(m => m.FinYearId == FinYear).Select(m => m.Year).FirstOrDefault();
                        dtResultExp = db.GetRFExpenditure(Convert.ToInt32(Financialyear));
                        dtResultProj = db.GetRFProject(Convert.ToInt32(Financialyear));
                        var ws3 = wb.Worksheets.Add("Project BreakUp");
                        //dtResultProj.Columns["ProjectNumber"].SetOrdinal(0);
                        //dtResultProj.Columns["SanctionValue"].SetOrdinal(1);
                        //dtResultProj.Columns["ProjectDate"].SetOrdinal(2);
                        //dtResultProj.Columns["ProjectFinYear"].SetOrdinal(3);
                        //dtResultProj.Columns["EmployeeName"].SetOrdinal(4);
                        //dtResultProj.Columns["DepartmentName"].SetOrdinal(5);
                        //dtResultProj.Columns["DepartmentCode"].SetOrdinal(6);
                        //dtResultProj.Columns["AgencyName"].SetOrdinal(7);
                        //dtResultProj.Columns["AgencyCode"].SetOrdinal(8);
                        //dtResultProj.Columns["ReceiptAmount"].SetOrdinal(9);
                        ws3.Cell(1, 1).InsertTable(dtResultProj);

                        var ws4 = wb.Worksheets.Add("Expenditure BreakUp");
                        //dtResultExp.Columns["FunctionName"].SetOrdinal(0);
                        //dtResultExp.Columns["ProjectNumber"].SetOrdinal(1);
                        //dtResultExp.Columns["EmployeeName"].SetOrdinal(2);
                        //dtResultExp.Columns["DepartmentName"].SetOrdinal(3);
                        //dtResultExp.Columns["DepartmentCode"].SetOrdinal(4);
                        //dtResultExp.Columns["AgencyName"].SetOrdinal(5);
                        //dtResultExp.Columns["AgencyCode"].SetOrdinal(6);
                        //dtResultExp.Columns["BillNumber"].SetOrdinal(7);
                        //dtResultExp.Columns["Amount"].SetOrdinal(8);
                        //dtResultExp.Columns["ExpDate"].SetOrdinal(9);
                        //dtResultExp.Columns["ExpYear"].SetOrdinal(10);
                        //dtResultExp.Columns["CommitmentNumber"].SetOrdinal(11);
                        //dtResultExp.Columns["HeadName"].SetOrdinal(12);
                        ws4.Cell(1, 1).InsertTable(dtResultExp);

                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=RF.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ICSROH1(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    //var FinQry = context.tblFinYear.Where(m => m.FinYearId == Year).FirstOrDefault();
                    //DateTime FromDate = Convert.ToDateTime(FinQry.StartDate);
                    //DateTime ToDate = Convert.ToDateTime(FinQry.EndDate);
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ReportService reportservice = new ReportService();
                        ICSROHReportModel model = new ICSROHReportModel();
                        ICSROHReportModel model1 = new ICSROHReportModel();
                        model = ReportService.GetICSROH(FromDate, ToDate);
                        model1 = ReportService.GetICSROHList(FromDate, ToDate);
                        var ws = wb.Worksheets.Add("ICSROH Income & Exp");
                        ws.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws.Range("A1:C1").Row(1).Merge();
                        ws.Cell(2, 1).Value = "INCOME  AND EXPENDITURE STATEMENT FOR THE FINANCIAL YEAR " + FromDate.Year + " - " + ToDate.Year;
                        ws.Range("A2:C2").Row(1).Merge();

                        ws.Cell(4, 1).Value = "Income ";
                        ws.Cell(4, 2).Value = "Amount";
                        ws.Cell(4, 3).Value = "%";
                        ws.Cell(5, 1).Value = "CONS Corpus ";
                        ws.Cell(5, 2).Value = model.CONSCorpus;
                        ws.Cell(5, 3).Value = model.CONSCorpusPct;
                        ws.Cell(6, 1).Value = "CONS Dist. ";
                        ws.Cell(6, 2).Value = model.CONSDist;
                        ws.Cell(6, 3).Value = model.CONSDistPct;
                        ws.Cell(7, 1).Value = "Interest ";
                        ws.Cell(7, 2).Value = model.Interest;
                        ws.Cell(7, 3).Value = model.InterestPct;
                        ws.Cell(8, 1).Value = "SPON Corpus ";
                        ws.Cell(8, 2).Value = model.SPONCorpus;
                        ws.Cell(8, 3).Value = model.SPONCorpusPct;
                        ws.Cell(9, 1).Value = "Other Income";
                        ws.Cell(9, 2).Value = model.OtherIncome;
                        ws.Cell(9, 3).Value = model.OtherIncomePct;
                        ws.Cell(10, 1).Value = "Total Income ";
                        ws.Cell(10, 2).Value = model.TotalIncome;
                        ws.Cell(10, 3).Value = model.TotalIncomePct;
                        ws.Cell(11, 1).Value = "Salary ";
                        ws.Cell(11, 2).Value = model.Salary;
                        ws.Cell(11, 3).Value = model.SalaryPct;
                        ws.Cell(12, 1).Value = "Interest ";
                        ws.Cell(12, 2).Value = model.InterestExp;
                        ws.Cell(12, 3).Value = model.InterestExpPct;
                        ws.Cell(13, 1).Value = "Repairs & Maint. ";
                        ws.Cell(13, 2).Value = model.RepairsMaint;
                        ws.Cell(13, 3).Value = model.RepairsMaintPct;
                        ws.Cell(14, 1).Value = "IT Equipment";
                        ws.Cell(14, 2).Value = model.ITEquipment;
                        ws.Cell(14, 3).Value = model.ITEquipmentPct;
                        ws.Cell(15, 1).Value = "Other Expenses";
                        ws.Cell(15, 2).Value = model.OtherExpenses;
                        ws.Cell(15, 3).Value = model.OtherExpensesPct;
                        ws.Cell(16, 1).Value = "Total Expenses";
                        ws.Cell(16, 2).Value = model.TotalExpenses;
                        ws.Cell(16, 3).Value = model.TotalExpensesPct;
                        ws.Cell(17, 1).Value = "Net Income";
                        ws.Cell(17, 2).Value = model.NetIncome;
                        ws.Cell(17, 3).Value = model.NetIncomePct;


                        var ws1 = wb.Worksheets.Add("ICSROH I&E");
                        ws1.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws1.Range("A1:C1").Row(1).Merge();
                        ws1.Cell(2, 1).Value = "INCOME  AND EXPENDITURE STATEMENT FOR THE FINANCIAL YEAR " + FromDate.Year + " - " + ToDate.Year;
                        ws1.Range("A2:C2").Row(1).Merge();
                        int Year0 = FromDate.Year;
                        int Year1 = ToDate.Year;
                        ws1.Cell(4, 1).Value = "Income ";
                        ws1.Cell(4, 2).Value = "Apr-" + Year0 + " ";
                        ws1.Cell(4, 3).Value = "May-" + Year0 + " ";
                        ws1.Cell(4, 4).Value = "jun-" + Year0 + " ";
                        ws1.Cell(4, 5).Value = "jul-" + Year0 + " ";
                        ws1.Cell(4, 6).Value = "Aug-" + Year0 + " ";
                        ws1.Cell(4, 7).Value = "Sep-" + Year0 + " ";
                        ws1.Cell(4, 8).Value = "Oct-" + Year0 + " ";
                        ws1.Cell(4, 9).Value = "Nov-" + Year0 + " ";
                        ws1.Cell(4, 10).Value = "Dec-" + Year0 + " ";
                        ws1.Cell(4, 11).Value = "Jan-" + Year1 + " ";
                        ws1.Cell(4, 12).Value = "Feb-" + Year1 + " ";
                        ws1.Cell(4, 13).Value = "Mar-" + Year1 + " ";
                        ws1.Cell(4, 14).Value = " Grand Total";
                        ws1.Cell(5, 1).Value = "CONS Corpus ";
                        int CONSCorpusrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.CONSCorpus.Count(); j++)
                        {
                            ws1.Cell(5, CONSCorpusrow).Value = String.Format("{0:0.00}", model1.ICSROHlist.CONSCorpus[j]);
                            CONSCorpusrow++;
                        }
                        ws1.Cell(5, CONSCorpusrow).Value = model1.CONSCorpus;
                        ws1.Cell(6, 1).Value = "CONS Dist";
                        int CONSDistrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.CONSDist.Count(); j++)
                        {
                            ws1.Cell(6, CONSDistrow).Value = String.Format("{0:0.00}", model1.ICSROHlist.CONSDist[j]);
                            CONSDistrow++;
                        }

                        ws1.Cell(6, CONSDistrow).Value = model1.CONSDist;
                        ws1.Cell(7, 1).Value = "Interest";
                        int Interestrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.Interest.Count(); j++)
                        {
                            ws1.Cell(7, Interestrow).Value = String.Format("{0:0.00}", model1.ICSROHlist.Interest[j]);
                            Interestrow++;
                        }
                        ws1.Cell(7, Interestrow).Value = model1.Interest;
                        ws1.Cell(8, 1).Value = "SPON Corpus";
                        int SPONCorpusrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.SPONCorpus.Count(); j++)
                        {
                            ws1.Cell(8, SPONCorpusrow).Value = model1.ICSROHlist.SPONCorpus[j].ToString();
                            SPONCorpusrow++;
                        }
                        ws1.Cell(8, SPONCorpusrow).Value = model1.SPONCorpus;
                        ws1.Cell(9, 1).Value = "Other Income";
                        int OtherIncomerow = 2;
                        for (int j = 0; j < model1.ICSROHlist.OtherIncome.Count(); j++)
                        {
                            ws1.Cell(9, OtherIncomerow).Value = model1.ICSROHlist.OtherIncome[j].ToString();
                            OtherIncomerow++;
                        }
                        ws1.Cell(9, OtherIncomerow).Value = model1.OtherIncome;
                        ws1.Cell(10, 1).Value = "Total Income";
                        int TotalIncomerow = 2;
                        for (int j = 0; j < model1.ICSROHlist.TotalIncome.Count(); j++)
                        {
                            ws1.Cell(10, TotalIncomerow).Value = model1.ICSROHlist.TotalIncome[j].ToString();
                            TotalIncomerow++;
                        }
                        ws1.Cell(10, TotalIncomerow).Value = model1.TotalIncome;
                        ws1.Cell(12, 1).Value = "Expenses";
                        ws1.Cell(13, 1).Value = "Salary";
                        int Salaryrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.Salary.Count(); j++)
                        {
                            ws1.Cell(13, Salaryrow).Value = model1.ICSROHlist.Salary[j].ToString();
                            Salaryrow++;
                        }
                        ws1.Cell(13, Salaryrow).Value = model1.Salary;
                        ws1.Cell(14, 1).Value = "Interest";
                        int InterestExprow = 2;
                        for (int j = 0; j < model1.ICSROHlist.InterestExp.Count(); j++)
                        {
                            ws1.Cell(14, InterestExprow).Value = model1.ICSROHlist.InterestExp[j].ToString();
                            InterestExprow++;
                        }
                        ws1.Cell(14, InterestExprow).Value = model1.InterestExp;
                        ws1.Cell(15, 1).Value = "Repairs & Maint";
                        int Repairsrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.RepairsMaint.Count(); j++)
                        {
                            ws1.Cell(15, Repairsrow).Value = model1.ICSROHlist.RepairsMaint[j].ToString();
                            Repairsrow++;
                        }
                        ws1.Cell(15, Repairsrow).Value = model1.RepairsMaint;
                        ws1.Cell(16, 1).Value = "IT Equipment";
                        int Equipmentrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.ITEquipment.Count(); j++)
                        {
                            ws1.Cell(16, Equipmentrow).Value = model1.ICSROHlist.ITEquipment[j].ToString();
                            Equipmentrow++;
                        }
                        ws1.Cell(16, Equipmentrow).Value = model1.ITEquipment;
                        ws1.Cell(17, 1).Value = "Other Expenses";
                        int Expensesrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.OtherExpenses.Count(); j++)
                        {
                            ws1.Cell(17, Expensesrow).Value = model1.ICSROHlist.OtherExpenses[j].ToString();
                            Expensesrow++;
                        }
                        ws1.Cell(17, Expensesrow).Value = model1.OtherExpenses;
                        ws1.Cell(18, 1).Value = "Total Expenses";
                        int TotalExpensesrow = 2;
                        for (int j = 0; j < model1.ICSROHlist.TotalExpenses.Count(); j++)
                        {
                            ws1.Cell(18, TotalExpensesrow).Value = model1.ICSROHlist.TotalExpenses[j].ToString();
                            TotalExpensesrow++;
                        }
                        ws1.Cell(18, TotalExpensesrow).Value = model1.TotalExpenses;
                        ws1.Cell(20, 1).Value = "Net Income";
                        int NetIncomerow = 2;
                        for (int j = 0; j < model1.ICSROHlist.NetIncome.Count(); j++)
                        {
                            ws1.Cell(20, NetIncomerow).Value = model1.ICSROHlist.NetIncome[j].ToString();
                            NetIncomerow++;
                        }
                        ws1.Cell(20, NetIncomerow).Value = model1.NetIncome;


                        var ws2 = wb.Worksheets.Add("ICSROH Income Break Up");
                        ws2.Cell(1, 1).Value = "Type";
                        ws2.Cell(1, 2).Value = "RefNumber";
                        ws2.Cell(1, 3).Value = "Amount";
                        ws2.Cell(1, 4).Value = "Date";
                        ws2.Cell(1, 5).Value = "Description";
                        int incomeBreakuprow = 2;
                        ListDatabaseObjects db1 = new ListDatabaseObjects();
                        DataTable dtResult5 = new DataTable();
                        ToDate = ToDate.AddDays(1).AddTicks(-1);
                        dtResult5 = db1.GetICSROHincome(FromDate, ToDate);
                        foreach (DataRow row in dtResult5.Rows)
                        {

                            ws2.Cell(incomeBreakuprow, 1).Value = row["Type"].ToString();
                            ws2.Cell(incomeBreakuprow, 2).Value = row["RefNumber"].ToString();
                            ws2.Cell(incomeBreakuprow, 3).Value = row["Amount"].ToString();
                            ws2.Cell(incomeBreakuprow, 4).Value = row["Date"].ToString();
                            ws2.Cell(incomeBreakuprow, 5).Value = row["Description"].ToString();
                            incomeBreakuprow++;
                        }


                        var ws3 = wb.Worksheets.Add("ICSROH Exp Break Up");
                        ws3.Cell(1, 1).Value = "Type";
                        ws3.Cell(1, 2).Value = "BillNumber";
                        ws3.Cell(1, 3).Value = "Amount";
                        ws3.Cell(1, 4).Value = "Date";
                        ws3.Cell(1, 5).Value = "HeadName";
                        ws3.Cell(1, 6).Value = "StaffName";
                        ws3.Cell(1, 7).Value = "StaffID";
                        ws3.Cell(1, 8).Value = "PaybillNo";
                        int expBreakuprow = 2;
                        DataTable dtResult6 = new DataTable();
                        dtResult6 = db1.GetICSROHexp(FromDate, ToDate);
                        foreach (DataRow row in dtResult6.Rows)
                        {

                            ws3.Cell(expBreakuprow, 1).Value = row["Type"].ToString();
                            ws3.Cell(expBreakuprow, 2).Value = row["BillNumber"].ToString();
                            ws3.Cell(expBreakuprow, 3).Value = row["Ammount"].ToString();
                            ws3.Cell(expBreakuprow, 4).Value = row["Date"].ToString();
                            ws3.Cell(expBreakuprow, 5).Value = row["HeadName"].ToString();
                            ws3.Cell(expBreakuprow, 6).Value = row["StaffName"].ToString();
                            ws3.Cell(expBreakuprow, 7).Value = row["StaffID"].ToString();
                            ws3.Cell(expBreakuprow, 8).Value = row["PaybillNo"].ToString();
                            expBreakuprow++;
                        }

                        var ws4 = wb.Worksheets.Add("ICSROH ledger  Exp Break Up");
                        ws4.Cell(1, 1).Value = "RefNumber";
                        ws4.Cell(1, 2).Value = "AccountHead";
                        ws4.Cell(1, 3).Value = "Amount";
                        int ledgerexpBreakuprow = 2;
                        DataTable dtResult7 = new DataTable();
                        dtResult7 = db1.GetICSROHLedgerexp(FromDate, ToDate);
                        foreach (DataRow row in dtResult7.Rows)
                        {

                            ws4.Cell(ledgerexpBreakuprow, 1).Value = row["RefNumber"].ToString();
                            ws4.Cell(ledgerexpBreakuprow, 2).Value = row["AccountHead"].ToString();
                            ws4.Cell(ledgerexpBreakuprow, 3).Value = row["Amount"].ToString();
                            ledgerexpBreakuprow++;
                        }

                        //workStream.Position = 0;
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Form3CH/ICSROH.xlsx");
                        wb.SaveAs(path);

                        Workbook book = new Workbook();
                        book.LoadFromFile(path);
                        Worksheet sheet = book.Worksheets["ICSROH Income & Exp"];
                        //  Worksheet sheet = ;
                        //Add chart and set chart data range  
                        Chart chart = sheet.Charts.Add(ExcelChartType.ColumnClustered);
                        chart.DataRange = sheet.Range["A4:B16"];
                        chart.SeriesDataFromRange = false;
                        //Chart border  
                        chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;
                        chart.ChartArea.Border.Color = System.Drawing.Color.Green;
                        //Chart position  
                        chart.LeftColumn = 2;
                        chart.TopRow = 19;
                        chart.RightColumn = 15;
                        chart.BottomRow = 29;
                        //Chart title  
                        chart.ChartTitle = "IC & SR INCOME  AND EXPENDITURE SUMMARY CHART FOR THE FINANCIAL YEAR " + FromDate.Year + " - " + ToDate.Year + "(RS. in Lakhs)";
                        chart.ChartTitleArea.Font.FontName = "Calibri";
                        chart.ChartTitleArea.Font.Size = 13;
                        chart.ChartTitleArea.Font.IsBold = true;
                        //Chart axis  
                        chart.PrimaryCategoryAxis.Title = "";
                        chart.PrimaryCategoryAxis.Font.Color = System.Drawing.Color.Brown;
                        chart.PrimaryValueAxis.Title = "";
                        chart.PrimaryValueAxis.HasMajorGridLines = false;
                        chart.PrimaryValueAxis.MaxValue = 10000;
                        chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                        //Chart legend  
                        chart.Legend.Position = LegendPositionType.Right;

                        ///2

                        Worksheet sheet1 = book.Worksheets["ICSROH I&E"];
                        //  Worksheet sheet = ;
                        //Add chart and set chart data range  
                        Chart chart1 = sheet1.Charts.Add(ExcelChartType.ColumnClustered);
                        chart1.DataRange = sheet1.Range["A4:N20"];
                        chart1.SeriesDataFromRange = false;
                        //Chart border  
                        chart1.ChartArea.Border.Weight = ChartLineWeightType.Medium;
                        chart1.ChartArea.Border.Color = System.Drawing.Color.Green;
                        //Chart position  
                        chart1.LeftColumn = 2;
                        chart1.TopRow = 27;
                        chart1.RightColumn = 20;
                        chart1.BottomRow = 39;
                        //Chart title  
                        chart1.ChartTitle = "IC & SR INCOME  AND EXPENDITURE SUMMARY CHART FOR THE FINANCIAL YEAR" + FromDate.Year + " - " + ToDate.Year + "(RS. in Lakhs)";
                        chart1.ChartTitleArea.Font.FontName = "Calibri";
                        chart1.ChartTitleArea.Font.Size = 13;
                        chart1.ChartTitleArea.Font.IsBold = true;
                        //Chart axis  
                        chart1.PrimaryCategoryAxis.Title = "";
                        chart1.PrimaryCategoryAxis.Font.Color = System.Drawing.Color.Brown;
                        chart1.PrimaryValueAxis.Title = "";
                        chart1.PrimaryValueAxis.HasMajorGridLines = false;
                        chart1.PrimaryValueAxis.MaxValue = 10000;
                        chart1.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                        //Chart legend  
                        chart1.Legend.Position = LegendPositionType.Right;


                        book.SaveToStream(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ICSROH.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public FileStreamResult InterestRefund(int Year)
        {
            try
            {
                DataTable dt = new DataTable();
                //dt = ReportService.GetInterestRefundYear(Year);
                //return coreaccountService.toSpreadSheet(dt);

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dsTrasaction = db.InterestRefund(Year);
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fdate"></param>
        /// <param name="tdate"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public FileStreamResult SOE(string fdate, string tdate, int ProjectId)
        {
            try
            {
                CultureInfo Indian = new CultureInfo("hi-IN");
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                var fromdate = Convert.ToDateTime(fdate);
                var todate = Convert.ToDateTime(tdate);
                DateTime EndDate; DateTime StartDate;

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        string AgencyCode = "";
                        SOEmodel model = new SOEmodel();
                        var ProjQry = context.tblProject.Where(m => m.ProjectId == ProjectId).FirstOrDefault();
                        int AgencyId = 0;
                        AgencyId = ProjQry.SponsoringAgency ?? 0;

                        AgencyCode = context.tblAgencyMaster.Where(m => m.AgencyId == AgencyId).Select(m => m.AgencyCode).FirstOrDefault();
                        ProjectService Projserv = new ProjectService();
                        ReportService Repserv = new ReportService();
                        ProjectSummaryModel SumModel = new ProjectSummaryModel();
                        SumModel = Projserv.getProjectSummary(ProjectId);

                        int NoOfyr = 0;
                        DateTime ProjectCloseDAte = Common.GetProjectCloseDate(ProjectId);
                        if (ProjQry.ActualStartDate > fromdate)
                            fromdate = Convert.ToDateTime(ProjQry.ActualStartDate);
                        if (ProjectCloseDAte < todate)
                            todate = ProjectCloseDAte;

                        DateTime frm = new DateTime(fromdate.Year, 4, 1);
                        DateTime to = new DateTime(todate.Year, 3, 31);
                        ProjectSummaryDetailModel SummDet = new ProjectSummaryDetailModel();
                        SummDet = Repserv.getProjectSummaryDetailsSOE(ProjectId, fromdate, todate, 0);

                        if (fromdate < frm && todate > to)
                        {
                            // todate = todate.AddYears(1);
                            //  fromdate = fromdate.AddYears(-1);
                            NoOfyr = (todate.Year + 1) - (fromdate.Year - 1);

                        }
                        else if (fromdate < frm)
                        {
                            //fromdate = fromdate.AddYears(-1);
                            NoOfyr = todate.Year - (fromdate.Year - 1);

                        }
                        else if (todate > to)
                        {
                            //todate = todate.AddYears(1);
                            NoOfyr = (todate.Year + 1) - fromdate.Year;

                        }
                        else
                            NoOfyr = (todate.Year) - (fromdate.Year);

                        DateTime FROM = fromdate; DateTime TO = todate;
                        if (ProjQry.TentativeStartDate > fromdate)
                            FROM = Convert.ToDateTime(ProjQry.ActualStartDate);
                        if (ProjectCloseDAte < todate)
                            TO = ProjectCloseDAte;

                        var ws = wb.Worksheets.Add("SOE");
                        ws.Cell(1, 7).Value = "Annexure-II";
                        ws.Cell(2, 1).Value = "REQUEST FOR ANNUAL INSTALMENT WITH UP-TO-DATE STATEMENT OF EXPENDITURE";
                        ws.Range("A2:I2").Row(1).Merge();
                        ws.Range("A1:J2").Style.Font.Bold = true;
                        ws.Cell(5, 1).Value = "1";
                        ws.Cell(5, 2).Value = "Sanctioned Order No & date  ";
                        ws.Cell(5, 3).Value = SumModel.SanctionOrderNo + " &  ";
                        ws.Cell(5, 4).Value = String.Format("{0:ddd dd-MMM-yyyy}", ProjQry.SanctionOrderDate);


                        ws.Cell(7, 1).Value = "2";
                        ws.Cell(7, 2).Value = "Name of the PI";
                        ws.Cell(7, 3).Value = SumModel.PIname;

                        ws.Cell(9, 1).Value = "3";
                        ws.Cell(9, 2).Value = "Total Project Cost";
                        ws.Cell(9, 3).Value = String.Format(Indian, "{0:N}", ProjQry.BaseValue);

                        ws.Cell(11, 1).Value = "4";
                        ws.Cell(11, 2).Value = "Revised Project Cost";
                        ws.Cell(12, 2).Value = "(if any)";
                        if (ProjQry.SanctionValue > ProjQry.BaseValue)
                            ws.Cell(11, 3).Value = String.Format(Indian, "{0:N}", ProjQry.SanctionValue);
                        else
                            ws.Cell(11, 3).Value = "0";

                        ws.Cell(14, 1).Value = "5";
                        ws.Cell(14, 2).Value = "Date of Commencement";
                        ws.Cell(14, 3).Value = SumModel.StartDate;

                        ws.Cell(16, 1).Value = "6";
                        ws.Cell(16, 2).Value = "Statement of Expenditure";
                        ws.Cell(17, 2).Value = "(Month wise expenditure incurred during current financial year)";
                        ws.Cell(17, 3).Value = "";

                        ws.Cell(19, 2).Value = "Month & Year";
                        ws.Cell(19, 3).Value = "Expenditure incurred/Committed";
                        ws.Range("A19:J19").Style.Font.Bold = true;
                        int YrCount;
                        int MonthYearRow = 20;
                        int StartYear = 0;
                        for (int i = 0; i < NoOfyr; i++)
                        {
                            //int StartYear;
                            int EndYear = 0;
                            StartYear = StartYear + 1;
                            if (i == 0)
                                StartYear = (fromdate.AddYears(i)).Year;

                            if (i == 1)
                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    StartYear = fromdate.Year;


                            if (i == 0)
                            {
                                StartDate = new DateTime(StartYear, fromdate.Month, fromdate.Day);
                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    EndYear = StartYear;
                                else
                                    EndYear = StartYear + 1;
                            }
                            //else if(i == 1)
                            //{
                            //    StartDate = new DateTime(StartYear, 4, 1);
                            //    if (3 >= fromdate.Month && fromdate.Month >= 1)
                            //        EndYear = fromdate.Year;
                            //    else
                            //        EndYear = StartYear + 1;
                            //}

                            else
                            {
                                StartDate = new DateTime(StartYear, 4, 1);
                                EndYear = StartYear + 1;
                            }
                            int m = i + 1;
                            if (m == NoOfyr)
                            {
                                EndDate = new DateTime(EndYear, todate.Month, todate.Day);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }
                            else
                            {
                                EndDate = new DateTime(EndYear, 3, 31);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }
                            MonthandYearExp MonthandYear = new MonthandYearExp();
                            MonthandYear = ReportService.GetMonthandYearExp(StartDate, EndDate, ProjectId);

                            if (i == 0)
                                ws.Cell(MonthYearRow, 2).Value = Common.Ordinal(i + 1) + " Year (" + FROM.Day + "-" + FROM.Month + "-" + FROM.Year + " to  31-03-" + EndDate.Year + ")";
                            else if (m == NoOfyr)
                                ws.Cell(MonthYearRow, 2).Value = Common.Ordinal(i + 1) + " Year (01-04-" + StartDate.Year + " to  " + TO.Day + "-" + TO.Month + "-" + TO.Year + ")";
                            else
                                ws.Cell(MonthYearRow, 2).Value = Common.Ordinal(i + 1) + " Year (01-04-" + StartDate.Year + "  to  31-03-" + EndDate.Year + ")";
                            ws.Cell(MonthYearRow, 3).Value = String.Format(Indian, "{0:N}", MonthandYear.Amount);
                            MonthYearRow++;

                        }
                        ws.Range("B" + MonthYearRow + ":J" + MonthYearRow).Style.Font.Bold = true;
                        ws.Cell(MonthYearRow, 2).Value = "Total";
                        ws.Cell(MonthYearRow, 3).Value = String.Format(Indian, "{0:N}", ReportService.GetMonthandYearExp(fromdate.AddDays(1).AddTicks(-2), todate.AddDays(1).AddTicks(-2), ProjectId).Amount);

                        ws.Range("A19:C" + (MonthYearRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A19:C" + (MonthYearRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A19:C" + (MonthYearRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A19:C" + (MonthYearRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;


                        ws.Cell(MonthYearRow + 3, 1).Value = "7";
                        ws.Cell(MonthYearRow + 3, 2).Value = "Grant received in each year";
                        ws.Range("A" + (MonthYearRow + 3) + ":J" + (MonthYearRow + 3)).Style.Font.Bold = true;
                        int MonthYearRCVrow = MonthYearRow + 4;
                        string totarec = "a";
                        DateTime IOASStartDate = new DateTime(2019, 4, 1);
                        bool Interset_f = false;
                        int Alphabets = 0;
                        for (int j = 0; j < NoOfyr; j++)
                        {
                            //int StartYear;
                            int EndYear = 0;
                            StartYear = StartYear + 1;
                            if (j == 0)
                                StartYear = (fromdate.AddYears(j)).Year;

                            if (j == 1)
                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    StartYear = fromdate.Year;


                            if (j == 0)
                            {
                                StartDate = new DateTime(StartYear, fromdate.Month, fromdate.Day);
                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    EndYear = StartYear;
                                else
                                    EndYear = StartYear + 1;
                            }
                            //else if (j == 1)
                            //{
                            //    StartDate = new DateTime(StartYear, 4, 1);
                            //    if (3 >= fromdate.Month && fromdate.Month >= 1)
                            //        EndYear = fromdate.Year;
                            //    else
                            //        EndYear = StartYear + 1;
                            //}
                            else
                            {
                                StartDate = new DateTime(StartYear, 4, 1);
                                EndYear = StartYear + 1;
                            }

                            int m = j + 1;
                            if (m == NoOfyr)
                            {
                                EndDate = new DateTime(EndYear, todate.Month, todate.Day);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }

                            else
                            {
                                EndDate = new DateTime(EndYear, 3, 31);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }

                            MonthandYearExp MonthandYearRCV = new MonthandYearExp();
                            MonthandYearRCV = ReportService.GetMonthandYearRCV(StartDate, EndDate, ProjectId);
                            if (j == 0)
                                ws.Cell(MonthYearRCVrow, 2).Value = Common.Ordinal(j + 1) + " Year (" + FROM.Day + "-" + FROM.Month + "-" + FROM.Year + " to  31-03-" + EndDate.Year + ")";
                            else if (m == NoOfyr)
                                ws.Cell(MonthYearRCVrow, 2).Value = Common.Ordinal(j + 1) + " Year ( 01-04-" + StartDate.Year + " to  " + TO.Day + "-" + TO.Month + "-" + TO.Year + ")";
                            else
                                ws.Cell(MonthYearRCVrow, 2).Value = Common.Ordinal(j + 1) + " Year (01-04-" + StartDate.Year + "  to  31-03-" + EndDate.Year + ")";

                            ws.Cell(MonthYearRCVrow, 3).Value = String.Format(Indian, "{0:N}", MonthandYearRCV.Amount);
                            ws.Cell(MonthYearRCVrow, 1).Value = Common.GetSmallAlphabets(Alphabets);

                            if (m != NoOfyr)
                                totarec += "+" + Common.GetSmallAlphabets(Alphabets + 1);

                            if (EndDate < IOASStartDate && (EndDate.Year == 2019) && (EndDate.Month == 3))
                            {
                                Alphabets++;
                                MonthYearRCVrow++;
                                ws.Cell(MonthYearRCVrow, 1).Value = Common.GetSmallAlphabets(Alphabets);
                                //if (m != NoOfyr)
                                // totarec += "+" + Common.GetSmallAlphabets(Alphabets + 1);
                                ws.Cell(MonthYearRCVrow, 2).Value = "Interest earned upto 31-03-2019";
                                ws.Cell(MonthYearRCVrow, 3).Value = String.Format(Indian, "{0:N}", Common.GetPreviousInterestAmount(ProjectId));

                            }
                            else if (((StartDate.Year == 2019) && (StartDate.Month > 3)) || (StartDate.Year > 2019))
                            {
                                Alphabets++;
                                MonthYearRCVrow++;
                                ws.Cell(MonthYearRCVrow, 1).Value = Common.GetSmallAlphabets(Alphabets);
                                if (m != NoOfyr)
                                    totarec += "+" + Common.GetSmallAlphabets(Alphabets + 1);
                                ws.Cell(MonthYearRCVrow, 2).Value = "Interest earned for " + Common.Ordinal(j + 1) + " Year";
                                ws.Cell(MonthYearRCVrow, 3).Value = String.Format(Indian, "{0:N}", ReportService.GetMonthandYearRCV(StartDate, EndDate, ProjectId).IntersetAmount);

                            }
                            Alphabets++;
                            MonthYearRCVrow++;
                        }
                        MonthandYearExp ttlrecmodel = new MonthandYearExp();
                        ttlrecmodel = ReportService.GetMonthandYearRCV(fromdate.AddDays(1).AddTicks(-2), todate.AddDays(1).AddTicks(-2), ProjectId);
                        // ws.Cell(MonthYearRCVrow, 1).Value = Common.GetSmallAlphabets(NoOfyr);
                        totarec += "+" + Common.GetSmallAlphabets(NoOfyr);
                        //ws.Cell(MonthYearRCVrow, 2).Value = "Interest, if any";
                        //ws.Cell(MonthYearRCVrow, 3).Value = String.Format(Indian, "{0:N}", ttlrecmodel.IntersetAmount);
                        ws.Cell(MonthYearRCVrow, 2).Value = "Total (" + totarec + ")";
                        ws.Range("A" + (MonthYearRow + 3) + ":C" + (MonthYearRCVrow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (MonthYearRow + 3) + ":C" + (MonthYearRCVrow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (MonthYearRow + 3) + ":C" + (MonthYearRCVrow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (MonthYearRow + 3) + ":C" + (MonthYearRCVrow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (MonthYearRCVrow) + ":J" + (MonthYearRCVrow)).Style.Font.Bold = true;
                        ws.Range("A1:A50").Style.Font.Bold = true;
                        decimal ttlrec = 0;
                        ttlrec = ttlrecmodel.IntersetAmount + ttlrecmodel.Amount;
                        ws.Cell((MonthYearRCVrow), 3).Value = String.Format(Indian, "{0:N}", ttlrec);

                        var ws1 = wb.Worksheets.Add("SOE-2");
                        int MonthYearRCVrow2 = -5;
                        ws1.Cell(MonthYearRCVrow2 + 6, 1).Value = "STATEMENT OF EXPENDITURE";
                        ws1.Cell(MonthYearRCVrow2 + 7, 1).Value = "(to be submitted financial yearwise - " + FROM.Day + "-" + FROM.Month + "-" + FROM.Year + " to " + TO.Day + "-" + TO.Month + "-" + TO.Year + ")";
                        ws1.Columns(MonthYearRCVrow2 + 9, 15).AdjustToContents();
                        ws1.Range("A1:J1").Row(1).Merge();
                        ws1.Range("A2:J2").Row(1).Merge();
                        ws1.Range("A1:J1").Style.Font.Bold = true;
                        ws1.Range("A2:J2").Style.Font.Bold = true;
                        ws1.Range("A4:P4").Style.Font.Bold = true;
                        ws1.Cell(MonthYearRCVrow2 + 9, 1).Value = "Sl No ";
                        ws1.Cell(MonthYearRCVrow2 + 9, 2).Value = "Sanctioned Heads ";
                        ws1.Cell(MonthYearRCVrow2 + 9, 3).Value = "Total funds allocated (indicate sanctioned or revised) ";
                        ws1.Cell(MonthYearRCVrow2 + 10, 1).Value = "(I) ";
                        ws1.Cell(MonthYearRCVrow2 + 10, 2).Value = "(II) ";
                        ws1.Cell(MonthYearRCVrow2 + 10, 3).Value = "(III) ";
                        ws1.Cell(4, 1).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 2).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 3).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 4).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 5).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 6).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 7).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 8).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 9).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 10).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 11).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 12).Style.Alignment.WrapText = true;
                        ws1.Cell(4, 13).Style.Alignment.WrapText = true;
                        int SummDetrow = MonthYearRCVrow2 + 11;
                        ProjectSummaryDetailModel SummDetail = new ProjectSummaryDetailModel();
                        SummDetail = Projserv.getProjectSummaryDetails(ProjectId);
                        int l = 1;

                        foreach (var item in SummDetail.HeadWise)
                        {

                            ws1.Cell(SummDetrow, 1).Value = l;
                            ws1.Cell(SummDetrow, 2).Value = String.Format(Indian, "{0:N}", item.AllocationHeadName);
                            ws1.Cell(SummDetrow, 3).Value = String.Format(Indian, "{0:N}", item.Amount);
                            SummDetrow++;
                            l++;
                        }
                        ws1.Range("B" + (MonthYearRCVrow2 + 11) + ":B" + SummDetrow).Style.Font.Bold = true;
                        string captotal = Common.GetCapitalAlphabets(SummDetrow);
                        ws1.Range("A" + SummDetrow + ":J" + SummDetrow).Style.Font.Bold = true;
                        ws1.Cell(SummDetrow, 2).Value = "Total";
                        ws1.Cell(SummDetrow, 3).Value = String.Format(Indian, "{0:N}", SummDetail.HeadWise.Sum(m => m.Amount));

                        int SummDetExprow = 0;

                        int expcol = 4;
                        for (int k = 0; k < NoOfyr; k++)
                        {
                            // int StartYear;
                            int EndYear = 0;
                            StartYear = StartYear + 1;
                            if (k == 0)
                                StartYear = (fromdate.AddYears(k)).Year;

                            if (k == 1)

                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    StartYear = fromdate.Year;

                            if (k == 0)
                            {
                                StartDate = new DateTime(StartYear, fromdate.Month, fromdate.Day);
                                if (3 >= fromdate.Month && fromdate.Month >= 1)
                                    EndYear = StartYear;
                                else
                                    EndYear = StartYear + 1;
                            }
                            //else if (k == 1)
                            //{
                            //    StartDate = new DateTime(StartYear, 4, 1);
                            //    if (3 >= fromdate.Month && fromdate.Month >= 1)
                            //        EndYear = fromdate.Year;
                            //    else
                            //        EndYear = StartYear + 1;

                            //}
                            else
                            {
                                StartDate = new DateTime(StartYear, 4, 1);
                                EndYear = StartYear + 1;
                            }


                            int m = k + 1;
                            if (m == NoOfyr)
                            {
                                EndDate = new DateTime(EndYear, todate.Month, todate.Day);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }

                            else
                            {
                                EndDate = new DateTime(EndYear, 3, 31);
                                EndDate = EndDate.AddDays(1).AddTicks(-2);
                            }

                            if (k == 0)
                            {
                                ws1.Cell(MonthYearRCVrow2 + 9, expcol).Value = Common.Ordinal(k + 1) + " year (" + FROM.Day + "-" + FROM.Month + "-" + FROM.Year + " to 31-03-" + EndDate.Year + ")";
                                ws1.Cell(MonthYearRCVrow2 + 10, expcol).Value = "(" + Common.ToRoman(expcol) + ")";
                            }
                            else if (m == NoOfyr)
                            {
                                ws1.Cell(MonthYearRCVrow2 + 9, expcol).Value = Common.Ordinal(k + 1) + " year (01-04-" + StartDate.Year + " to " + TO.Day + "-" + TO.Month + "-" + TO.Year + ")";
                                ws1.Cell(MonthYearRCVrow2 + 10, expcol).Value = "(" + Common.ToRoman(expcol) + ")";
                            }
                            else
                            {
                                ws1.Cell(MonthYearRCVrow2 + 9, expcol).Value = Common.Ordinal(k + 1) + " year (01-04-" + StartDate.Year + " to 31-03-" + EndDate.Year + ")";
                                ws1.Cell(MonthYearRCVrow2 + 10, expcol).Value = "(" + Common.ToRoman(expcol) + ")";
                            }
                            SummDetExprow = MonthYearRCVrow2 + 11;
                            ProjectSummaryDetailModel SummDetExp = new ProjectSummaryDetailModel();
                            SummDetExp = Repserv.getProjectSummaryDetailsSOE(ProjectId, StartDate, EndDate, 0);
                            foreach (var item in SummDetExp.HeadWise)
                            {
                                ws1.Cell(SummDetExprow, expcol).Value = String.Format(Indian, "{0:N}", item.Expenditure);
                                SummDetExprow++;
                            }
                            ws1.Cell(SummDetExprow, expcol).Value = String.Format(Indian, "{0:N}", SummDetExp.HeadWise.Sum(n => n.Expenditure));
                            expcol++;

                        }

                        int CommitExprow = MonthYearRCVrow2 + 11;
                        foreach (var item in SummDet.HeadWise)
                        {
                            ws1.Cell(CommitExprow, expcol).Value = String.Format(Indian, "{0:N}", item.Expenditure);
                            ws1.Cell(CommitExprow, expcol + 1).Value = String.Format(Indian, "{0:N}", (item.Amount - item.Expenditure));
                            ws1.Cell(CommitExprow, expcol + 3).Value = String.Format(Indian, "{0:N}", item.BalanceAmount);
                            CommitExprow++;
                        }
                        int Totalexp = expcol - 4;
                        string TotalexpStr = "";
                        for (int i = 1; i < Totalexp; i++)
                        {
                            int id = 4 + i;
                            TotalexpStr += "+" + Common.ToRoman(id);
                        }
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol).Value = "Total Expenditure (" + Common.ToRoman((expcol)) + "= IV " + TotalexpStr + ")";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol).Value = "(" + Common.ToRoman(expcol) + ")";
                        ws1.Cell(SummDetExprow, expcol).Value = String.Format(Indian, "{0:N}", SummDet.HeadWise.Sum(m => m.Expenditure));
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 1).Value = "ExpenseBalance as on date (" + Common.ToRoman((expcol + 1)) + " = III -" + Common.ToRoman((expcol)) + ")";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 1).Value = "(" + Common.ToRoman((expcol + 1)) + ")";
                        ws1.Cell(SummDetExprow, expcol + 1).Value = String.Format(Indian, "{0:N}", (SummDet.HeadWise.Sum(m => m.Amount) - SummDet.HeadWise.Sum(m => m.Expenditure)));
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 2).Value = "Project Balance Excluding Comitment";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 2).Value = "(" + Common.ToRoman((expcol + 2)) + ")";
                        ws1.Cell(SummDetExprow, expcol + 2).Value = String.Format(Indian, "{0:N}", (ttlrec - SummDet.HeadWise.Sum(m => m.Expenditure)));
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 3).Value = "Comitment";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 3).Value = "(" + Common.ToRoman((expcol + 3)) + ")";
                        ws1.Cell(SummDetExprow, expcol + 3).Value = String.Format(Indian, "{0:N}", SummDet.HeadWise.Sum(m => m.BalanceAmount));
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 4).Value = "Project Balance including Comitment  (Receipt - Total Exp)";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 4).Value = "(" + Common.ToRoman((expcol + 4)) + ")";
                        ws1.Cell(SummDetExprow, expcol + 4).Value = String.Format(Indian, "{0:N}", (ttlrec - SummDet.HeadWise.Sum(m => m.Expenditure) - SummDet.HeadWise.Sum(m => m.BalanceAmount)));
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 5).Value = "Requirement of funds upto 31st March next year";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 5).Value = "(" + Common.ToRoman((expcol + 5)) + ")";
                        ws1.Cell(MonthYearRCVrow2 + 9, expcol + 6).Value = "Remarks (if any)";
                        ws1.Cell(MonthYearRCVrow2 + 10, expcol + 6).Value = "(" + Common.ToRoman((expcol + 6)) + ")";

                        ws1.Cell(MonthYearRCVrow2 + 9, 15).Style.Alignment.WrapText = true;
                        ws1.Range("A" + (MonthYearRCVrow2 + 9) + ":" + Common.GetCapitalAlphabets(expcol + 5) + (SummDetExprow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws1.Range("A" + (MonthYearRCVrow2 + 9) + ":" + Common.GetCapitalAlphabets(expcol + 5) + (SummDetExprow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws1.Range("A" + (MonthYearRCVrow2 + 9) + ":" + Common.GetCapitalAlphabets(expcol + 5) + (SummDetExprow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws1.Range("A" + (MonthYearRCVrow2 + 9) + ":" + Common.GetCapitalAlphabets(expcol + 5) + (SummDetExprow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;

                        ws1.Cell(SummDetrow + 2, 1).Value = "Name & Signature";
                        ws1.Range("A" + (SummDetrow + 2) + ":B" + (SummDetrow + 2)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 3, 1).Value = "Principal Investigator : ";
                        ws1.Range("A" + (SummDetrow + 3) + ":B" + (SummDetrow + 3)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 2, 3).Value = "Signature of Finance Officer";
                        ws1.Range("C" + (SummDetrow + 2) + ":D" + (SummDetrow + 2)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 3, 3).Value = "Date : ";
                        ws1.Cell(SummDetrow + 2, 5).Value = "Signature of Deputy Registrar";
                        ws1.Range("E" + (SummDetrow + 2) + ":F" + (SummDetrow + 2)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 3, 5).Value = "Date : ";
                        ws1.Cell(SummDetrow + 2, 7).Value = "Signature of Head of Institute";
                        ws1.Range("G" + (SummDetrow + 2) + ":H" + (SummDetrow + 2)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 3, 7).Value = "Date : ";
                        ws1.Cell(SummDetrow + 4, 1).Value = "Date:";
                        ws1.Cell(SummDetrow + 5, 1).Value = "*DOS- Date of Start of Project";
                        ws1.Range("E" + (SummDetrow + 5) + ":F" + (SummDetrow + 5)).Row(1).Merge();
                        ws1.Cell(SummDetrow + 6, 1).Value = "Note:";

                        ws1.Range("A" + (SummDetrow + 2) + ":J" + (SummDetrow + 6)).Style.Font.Bold = true;
                        ws1.Cell(SummDetrow + 7, 2).Value = "1. Expenditure under the  sanctioned heads, at any point of time, should not exceed funds allocated under that head, without, ";
                        ws1.Cell(SummDetrow + 8, 2).Value = "prior approval of " + AgencyCode + " i.e. Figures in Column (" + Common.ToRoman((expcol)) + ") should not exceed corresponding figures in column (III)";
                        ws1.Cell(SummDetrow + 9, 2).Value = "2. Utilization Certificate (Annexure III) for each financial year ending 31 st March has to be enclosed along with request for";
                        ws1.Cell(SummDetrow + 10, 2).Value = "carry-forward permission to the next financial year.";

                        ws1.Range("B" + (SummDetrow + 7) + ":N" + (SummDetrow + 7)).Row(1).Merge();
                        ws1.Range("B" + (SummDetrow + 8) + ":N" + (SummDetrow + 8)).Row(1).Merge();
                        ws1.Range("B" + (SummDetrow + 9) + ":N" + (SummDetrow + 9)).Row(1).Merge();
                        ws1.Range("B" + (SummDetrow + 10) + ":N" + (SummDetrow + 10)).Row(1).Merge();

                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=SOE.xls");
                        return File(workStream, fileType);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult Oldreceipts(int ProjectId = 0)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = ReportService.GetOldreceipts(ProjectId);
                return coreaccountService.toSpreadSheet(dt);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }

        }
        public FileStreamResult OldExpenditure(int ProjectId = 0)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = ReportService.GetOldExpenditure(ProjectId);
                return coreaccountService.toSpreadSheet(dt);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }

        }
        public FileStreamResult BRSReconcileReport(DateTime FromDate, DateTime ToDate, int BankId)
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dsTrasaction = db.GetBRSReconcile(FromDate, ToDate, BankId);
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ProjectExpSPReport()
        {
            try
            {

                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsTrasaction = db.GetProjectExpExceed();
                dsTrasaction.Tables[0].TableName = "Allocation Exceed";
                dsTrasaction.Tables[1].TableName = "SanctionValue Exceed";
                return coreaccountService.toSpreadSheet(dsTrasaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult LedgerBalance1()
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                var fromdate = Convert.ToDateTime("2019-04-01");
                var todate = Convert.ToDateTime("2020-03-31");
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var data = context.tblAccountHead.ToList();
                        foreach (var hd in data)
                        {
                            ListDatabaseObjects db = new ListDatabaseObjects();
                            DataSet dataset = new DataSet();
                            DataTable dtResult = new DataTable();
                            dtResult = db.GetLedgerBalDebit(hd.AccountHeadId, fromdate, todate);
                            DataTable dtResult1 = new DataTable();
                            dtResult1 = db.GetLedgerBalCredit(hd.AccountHeadId, fromdate, todate);
                            var Qry = db.GetLedgerBalAmt(hd.AccountHeadId, fromdate, todate);
                            decimal crOB = 0; decimal drOB = 0; decimal crCB = 0; decimal drCB = 0;
                            if (Qry.Item3 > Qry.Item4)
                                crOB = Qry.Item3 - Qry.Item4;
                            else
                                drOB = Qry.Item4 - Qry.Item3;
                            decimal ttlCr = Qry.Item1 + crOB;
                            decimal ttlDr = Qry.Item2 + drOB;

                            if (ttlCr > ttlDr)
                                crCB = ttlCr - ttlDr;
                            else
                                drCB = ttlDr - ttlCr;

                            string sheetname = Regex.Replace(hd.AccountHead, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                            sheetname = sheetname.Length > 31 ? sheetname.Substring(0, 30) : sheetname;
                            var ws = wb.Worksheets.Add(sheetname);
                            ws.Cell(1, 1).Value = "Ledger Balance ( " + fromdate + " to " + todate + " )";
                            ws.Range("A1:C1").Row(1).Merge();
                            ws.Range("A1:O1").Style.Font.Bold = true;
                            ws.Cell(2, 1).Value = hd.AccountHead;
                            ws.Range("A2:C2").Row(1).Merge();
                            ws.Range("A2:O2").Style.Font.Bold = true;
                            ws.Cell(4, 1).Value = "Debit";
                            ws.Range("A4:D4").Row(1).Merge();
                            ws.Range("A4:O4").Style.Font.Bold = true;
                            ws.Range("A5:O5").Style.Font.Bold = true;

                            ws.Cell(5, 1).Value = "Date";
                            ws.Cell(5, 2).Value = "Reference Number";
                            ws.Cell(5, 3).Value = "Particulars";
                            ws.Cell(5, 4).Value = "Amount";
                            var rngTitle = ws.Range("E5:F5000");
                            rngTitle.Style.Fill.BackgroundColor = XLColor.Rose;
                            if (drOB > 0)
                            {
                                ws.Cell(6, 3).Value = "Opening Balance";
                                ws.Cell(6, 4).Value = drOB;
                            }
                            ws.Cell(4, 7).Value = "Credit";
                            ws.Range("G4:J4").Row(1).Merge();
                            ws.Cell(5, 7).Value = "Date";
                            ws.Cell(5, 8).Value = "Reference Number";
                            ws.Cell(5, 9).Value = "Particulars";
                            ws.Cell(5, 10).Value = "Amount";
                            if (crOB > 0)
                            {
                                ws.Cell(6, 9).Value = "Opening Balance";
                                ws.Cell(6, 10).Value = crOB;
                            }

                            int Debitrow = 7;
                            foreach (DataRow row in dtResult.Rows)
                            {

                                ws.Cell(Debitrow, 1).Value = row["PostedDate"].ToString();
                                ws.Cell(Debitrow, 2).Value = row["RefNumber"].ToString();
                                ws.Cell(Debitrow, 3).Value = row["FunctionName"].ToString();
                                ws.Cell(Debitrow, 4).Value = row["Amount"].ToString();
                                Debitrow++;
                            }
                            int Creditrow = 7;
                            foreach (DataRow row in dtResult1.Rows)
                            {
                                ws.Cell(Creditrow, 7).Value = row["PostedDate"].ToString();
                                ws.Cell(Creditrow, 8).Value = row["RefNumber"].ToString();
                                ws.Cell(Creditrow, 9).Value = row["FunctionName"].ToString();
                                ws.Cell(Creditrow, 10).Value = row["Amount"].ToString();
                                Creditrow++;
                            }

                            if (crCB > drCB)
                            {
                                ws.Cell(Debitrow, 3).Value = "Closing Balance";
                                ws.Cell(Debitrow, 4).Value = crCB;
                                ws.Cell(Debitrow + 1, 3).Value = "Total";
                                ws.Cell(Debitrow + 1, 4).Value = ttlDr + (crCB);
                                ws.Cell(Creditrow + 1, 9).Value = "Total";
                                ws.Cell(Creditrow + 1, 10).Value = ttlCr;
                            }
                            else
                            {

                                ws.Cell(Debitrow + 1, 3).Value = "Total";
                                ws.Cell(Debitrow + 1, 4).Value = ttlDr;
                                ws.Cell(Creditrow, 9).Value = "Closing Balance";
                                ws.Cell(Creditrow, 10).Value = drCB;
                                ws.Cell(Creditrow + 1, 9).Value = "Total";
                                ws.Cell(Creditrow + 1, 10).Value = ttlCr + (drCB);
                            }

                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        return new FileStreamResult(workStream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult ProfitandLoss(DateTime Date)
        {
            try
            {

                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);

                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();


                        var CurrYearQry = context.tblFinYear.Where(m => m.StartDate <= Date && Date <= m.EndDate).FirstOrDefault();
                        DateTime CurrFromDate = Convert.ToDateTime(CurrYearQry.StartDate);
                        DateTime CurrToDate = Date;
                        DateTime Date2 = CurrYearQry.StartDate.Value.AddDays(-1);
                        var PrevYearQry = context.tblFinYear.Where(m => m.EndDate == Date2).FirstOrDefault();
                        DateTime PrevFromDate = Convert.ToDateTime(PrevYearQry.StartDate);
                        DateTime PrevToDate = Convert.ToDateTime(PrevYearQry.EndDate);

                        DataSet dataset = new DataSet();
                        DataTable dtIncomeCurr = new DataTable();
                        DataTable dtExpenseCurr = new DataTable();
                        DataTable dtIncomePrev = new DataTable();
                        DataTable dtExpensePrev = new DataTable();


                        var ws = wb.Worksheets.Add("Profit & Loss Account");

                        var icsrlogo = Server.MapPath(@"~/Content/InstituteLogo/icsr_logo.png");

                        var iitlogo = Server.MapPath(@"~/Content/InstituteLogo/37616b81-4f25-4a72-a506-d28385afe5bf_IITMadras.png");
                        ws.Column(1).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(iitlogo)
                        .MoveTo(ws.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws.Column(5).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(icsrlogo)
                        .MoveTo(ws.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws.Range("B1:D1").Row(1).Merge();
                        ws.Range("B1:D1").Style.Font.Bold = true;
                        ws.Range("A7:A100").Style.Font.Bold = true;
                        ws.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws.Range("B2:D2").Row(1).Merge();
                        ws.Range("B2:D2").Style.Font.Bold = true;
                        ws.Cell(3, 2).Value = "Chennai - 600 036";
                        ws.Range("B3:D3").Row(1).Merge();
                        ws.Range("B3:D3").Style.Font.Bold = true;
                        ws.Cell(5, 2).Value = "Income Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", Date);
                        ws.Range("B5:D5").Row(1).Merge();
                        ws.Range("B5:D5").Style.Font.Bold = true;
                        ws.Cell(7, 1).Value = "Revenue";
                        ws.Cell(7, 3).Value = "Notes";
                        ws.Cell(7, 4).Value = PrevYearQry.Year;
                        ws.Cell(7, 5).Value = CurrYearQry.Year;
                        ws.Range("A7:E7").Style.Font.Bold = true;



                        dtIncomeCurr = db.GetProfitandLossIncome(CurrFromDate, CurrToDate);
                        dtExpenseCurr = db.GetProfitandLossExpense(CurrFromDate, CurrToDate);
                        dtIncomePrev = db.GetProfitandLossIncome(PrevFromDate, PrevToDate);
                        dtExpensePrev = db.GetProfitandLossExpense(PrevFromDate, PrevToDate);
                        int firstrow = 8;
                        decimal PrevIncome = 0;
                        decimal CurrIncome = 0;
                        DataRow[] draIncomePrev = new DataRow[dtIncomePrev.Rows.Count];
                        dtIncomePrev.Rows.CopyTo(draIncomePrev, 0);

                        for (int i = 0; i < draIncomePrev.Count(); i++)
                        {

                            if (i != 0)
                            {
                                if (draIncomePrev.Count() > i + 1)
                                {
                                    if (draIncomePrev[i - 1].Field<string>("Groups") != draIncomePrev[i].Field<string>("Groups"))
                                    {
                                        ws.Cell(firstrow, 1).Value = draIncomePrev[i].Field<string>("Groups");
                                    }
                                }
                            }
                            else
                                ws.Cell(firstrow, 1).Value = draIncomePrev[i].Field<string>("Groups");
                            ws.Cell(firstrow, 2).Value = draIncomePrev[i].Field<string>("AccountHead");
                            ws.Cell(firstrow, 4).Value = draIncomePrev[i].Field<decimal>("Amount");
                            PrevIncome += (Convert.ToDecimal(draIncomePrev[i].Field<decimal>("Amount")));
                            firstrow++;
                        }

                        //foreach (DataRow row in dtIncomePrev.Rows)
                        //{
                        //    ws.Cell(firstrow, 1).Value = row["Groups"].ToString();
                        //    ws.Cell(firstrow, 2).Value = row["AccountHead"].ToString();
                        //    ws.Cell(firstrow, 4).Value = row["Amount"].ToString();
                        //    PrevIncome += (Convert.ToDecimal(row["Amount"].ToString()));
                        //    firstrow++;
                        //}

                        firstrow = 8;
                        foreach (DataRow row in dtIncomeCurr.Rows)
                        {
                            ws.Cell(firstrow, 5).Value = row["Amount"].ToString();
                            CurrIncome += (Convert.ToDecimal(row["Amount"].ToString()));
                            firstrow++;
                        }
                        ws.Cell(firstrow, 1).Value = "Total Revenues";
                        ws.Cell(firstrow, 4).Value = PrevIncome;
                        ws.Cell(firstrow, 5).Value = CurrIncome;
                        ws.Range("A" + firstrow + ":E" + firstrow).Style.Font.Bold = true;

                        ws.Cell(firstrow + 2, 1).Value = "Expenses";
                        ws.Cell(firstrow + 2, 3).Value = "Notes";
                        ws.Cell(firstrow + 2, 4).Value = PrevYearQry.Year;
                        ws.Cell(firstrow + 2, 5).Value = CurrYearQry.Year;
                        ws.Range("A" + (firstrow + 2) + ":E" + (firstrow + 2)).Style.Font.Bold = true;

                        int Secondrow = firstrow + 3;
                        decimal PrevExpense = 0;
                        decimal CurrExpense = 0;

                        DataRow[] draExpensePrev = new DataRow[dtExpensePrev.Rows.Count];
                        dtExpensePrev.Rows.CopyTo(draExpensePrev, 0);

                        for (int i = 0; i < draExpensePrev.Count(); i++)
                        {

                            if (i != 0)
                            {
                                if (draExpensePrev.Count() > i + 1)
                                {
                                    if (draExpensePrev[i - 1].Field<string>("Groups") != draExpensePrev[i].Field<string>("Groups"))
                                    {
                                        ws.Cell(Secondrow, 1).Value = draExpensePrev[i].Field<string>("Groups");
                                    }
                                }
                            }
                            else
                                ws.Cell(Secondrow, 1).Value = draExpensePrev[i].Field<string>("Groups");
                            ws.Cell(Secondrow, 2).Value = draExpensePrev[i].Field<string>("AccountHead");
                            ws.Cell(Secondrow, 4).Value = draExpensePrev[i].Field<decimal>("Amount");
                            PrevExpense += (Convert.ToDecimal(draExpensePrev[i].Field<decimal>("Amount")));
                            Secondrow++;
                        }
                        //foreach (DataRow row in dtExpensePrev.Rows)
                        //{
                        //    ws.Cell(Secondrow, 1).Value = row["Groups"].ToString();
                        //    ws.Cell(Secondrow, 2).Value = row["AccountHead"].ToString();
                        //    ws.Cell(Secondrow, 4).Value = row["Amount"].ToString();
                        //    PrevExpense += (Convert.ToDecimal(row["Amount"].ToString()));
                        //    Secondrow++;
                        //}
                        Secondrow = firstrow + 3;
                        foreach (DataRow row in dtExpenseCurr.Rows)
                        {
                            ws.Cell(Secondrow, 5).Value = row["Amount"].ToString();
                            CurrExpense += (Convert.ToDecimal(row["Amount"].ToString()));
                            Secondrow++;
                        }
                        ws.Cell(Secondrow, 1).Value = "Total Expenses";
                        ws.Cell(Secondrow, 4).Value = PrevExpense;
                        ws.Cell(Secondrow, 5).Value = CurrExpense;
                        ws.Range("A" + Secondrow + ":E" + Secondrow).Style.Font.Bold = true;

                        ws.Cell(Secondrow + 2, 1).Value = "Net Income";
                        ws.Cell(Secondrow + 2, 4).Value = PrevIncome - PrevExpense;
                        ws.Cell(Secondrow + 2, 5).Value = CurrIncome - CurrExpense;
                        ws.Range("A" + (Secondrow + 2) + ":E" + (Secondrow + 2)).Style.Font.Bold = true;

                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=ProfitandLoss.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
   (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
        }
        public FileStreamResult ReceiptsandPayment(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();


                        DataSet dataset = new DataSet();


                        DataSet dsReport = db.getRandP(FromDate, ToDate);
                        DataTable dtResult0 = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : new DataTable();


                        var ws = wb.Worksheets.Add("Receipts and Payment");

                        var icsrlogo = Server.MapPath(@"~/Content/InstituteLogo/icsr_logo.png");

                        var iitlogo = Server.MapPath(@"~/Content/InstituteLogo/37616b81-4f25-4a72-a506-d28385afe5bf_IITMadras.png");
                        ws.Column(1).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(iitlogo)
                        .MoveTo(ws.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws.Column(5).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(icsrlogo)
                        .MoveTo(ws.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws.Range("B1:D1").Row(1).Merge();
                        ws.Range("B1:D1").Style.Font.Bold = true;
                        ws.Range("A7:A100").Style.Font.Bold = true;
                        ws.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws.Range("B2:D2").Row(1).Merge();
                        ws.Range("B2:D2").Style.Font.Bold = true;
                        ws.Cell(3, 2).Value = "Chennai - 600 036";
                        ws.Range("B3:D3").Row(1).Merge();
                        ws.Range("B3:D3").Style.Font.Bold = true;


                        ws.Range("B5:D5").Row(1).Merge();
                        ws.Range("B5:D5").Style.Font.Bold = true;

                        ws.Range("A1:A500").Style.Font.Bold = true;
                        ws.Cell(7, 1).Value = "Bank Opening Balance";
                        ws.Cell(7, 2).Value = "Amount";

                        DataTable dtDataTable = db.GetPayementForReceiptsandPaymentForRandP(FromDate, ToDate);

                        DateTime FromDate2 = FromDate.AddDays(-1);
                        DataTable dtDataBankOB = db.GetCommonBankBalance(FromDate2);
                        decimal ImprestOB = db.GetImprestBankBalance(FromDate2);

                        int firstrow = 8;
                        decimal BankOB = 0;
                        foreach (DataRow row in dtDataBankOB.Rows)
                        {
                            ws.Cell(firstrow, 1).Value = row["AccountHead"].ToString();
                            ws.Cell(firstrow, 2).Value = row["Amount"].ToString();
                            BankOB += (Convert.ToDecimal(row["Amount"].ToString()));
                            firstrow++;
                        }

                        ws.Cell(firstrow, 1).Value = "Imprest Account";
                        ws.Cell(firstrow, 2).Value = ImprestOB;
                        ws.Cell(firstrow + 1, 1).Value = "Bank Opening Balance";
                        ws.Cell(firstrow + 1, 2).Value = BankOB + ImprestOB;


                        ws.Cell(firstrow + 3, 1).Value = "Receipts";
                        int Secondrow = firstrow + 4;
                        decimal receiptTotal = 0;
                        foreach (DataRow row in dtResult0.Rows)
                        {
                            if (row["Type"].ToString() == "Receipt")
                            {
                                ws.Cell(Secondrow, 1).Value = row["AccountHead"].ToString();
                                ws.Cell(Secondrow, 2).Value = row["Amount"].ToString();
                                receiptTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                                Secondrow++;
                            }

                        }
                        ws.Cell(Secondrow + 1, 1).Value = "Receipt Total";
                        ws.Cell(Secondrow + 1, 2).Value = receiptTotal;

                        ws.Cell(Secondrow + 3, 1).Value = "Payment";

                        decimal paymentTotal = 0;
                        decimal paymentHeadTotal = 0;
                        int Headrow = Secondrow + 4;
                        foreach (DataRow row in dtDataTable.Rows)
                        {

                            ws.Cell(Headrow, 1).Value = row["AccountHead"].ToString();
                            ws.Cell(Headrow, 2).Value = row["Amount"].ToString();
                            paymentHeadTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            Headrow++;


                        }
                        int thirdrow = Headrow;
                        foreach (DataRow row in dtResult0.Rows)
                        {
                            if (row["Type"].ToString() == "Payment")
                            {
                                ws.Cell(thirdrow, 1).Value = row["AccountHead"].ToString();
                                ws.Cell(thirdrow, 2).Value = row["Amount"].ToString();
                                paymentTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                                thirdrow++;
                            }

                        }
                        ws.Cell(thirdrow + 1, 1).Value = "Payment Total";
                        ws.Cell(thirdrow + 1, 2).Value = paymentTotal + paymentHeadTotal;

                        DataTable dtDataBankCB = db.GetCommonBankBalance(ToDate);
                        ws.Cell(thirdrow + 3, 1).Value = "Bank Closing Balance";

                        int fouthrow = thirdrow + 4;
                        decimal BankCB = 0;
                        foreach (DataRow row in dtDataBankCB.Rows)
                        {
                            ws.Cell(fouthrow, 1).Value = row["AccountHead"].ToString();
                            ws.Cell(fouthrow, 2).Value = row["Amount"].ToString();
                            BankCB += (Convert.ToDecimal(row["Amount"].ToString()));
                            fouthrow++;
                        }
                        decimal ImprestCB = db.GetImprestBankBalance(ToDate);
                        ws.Cell(fouthrow, 1).Value = "Imprest Account";
                        ws.Cell(fouthrow, 2).Value = ImprestCB;
                        ws.Cell(fouthrow + 1, 1).Value = "Bank Closing Balance";
                        ws.Cell(fouthrow + 1, 2).Value = BankCB + ImprestCB;



                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=ReceiptsandPayment.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
   (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
        }
        //     public FileStreamResult BalanceSheet(DateTime Date)
        //     {
        //         try
        //         {
        //             MemoryStream workStream = new MemoryStream();
        //             byte[] byteInfo = workStream.ToArray();
        //             workStream.Write(byteInfo, 0, byteInfo.Length);
        //             using (var context = new IOASDBEntities())
        //             {
        //                 using (XLWorkbook wb = new XLWorkbook())
        //                 {
        //                     ListDatabaseObjects db = new ListDatabaseObjects();

        //                     var icsrlogo = Server.MapPath(@"~/Content/InstituteLogo/icsr_logo.png");

        //                     var iitlogo = Server.MapPath(@"~/Content/InstituteLogo/37616b81-4f25-4a72-a506-d28385afe5bf_IITMadras.png");

        //                     var ws = wb.Worksheets.Add("Balance Sheet");
        //                     var ws1 = wb.Worksheets.Add("Notes to B-S");
        //                     var ws2 = wb.Worksheets.Add("Profit & Loss Account");
        //                     var ws3 = wb.Worksheets.Add("Receipts and Payment");
        //                     var ws4 = wb.Worksheets.Add("Prev Year Advance");
        //                     var ws5 = wb.Worksheets.Add("Current year Advance");


        //                     ws.Column(1).Width = 30;
        //                     ws.Row(1).Height = 50;
        //                     ws.AddPicture(iitlogo)
        //                     .MoveTo(ws.Cell(1, 1).Address)
        //                     .Scale(0.5);
        //                     ws.Column(5).Width = 30;
        //                     ws.Row(1).Height = 50;
        //                     ws.AddPicture(icsrlogo)
        //                     .MoveTo(ws.Cell(1, 5).Address)
        //                     .Scale(0.5);
        //                     ws.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
        //                     ws.Range("B1:D1").Row(1).Merge();
        //                     ws.Range("B1:D1").Style.Font.Bold = true;
        //                     ws.Range("A7:A100").Style.Font.Bold = true;
        //                     ws.Cell(2, 2).Value = "Indian Institute of Technology Madras";
        //                     ws.Range("B2:D2").Row(1).Merge();
        //                     ws.Range("B2:D2").Style.Font.Bold = true;
        //                     ws.Cell(3, 2).Value = "Chennai - 600 036";
        //                     ws.Range("B3:D3").Row(1).Merge();
        //                     ws.Range("B3:D3").Style.Font.Bold = true;
        //                     ws.Cell(5, 2).Value = "Balance Sheet for the period " + String.Format("{0:dd-MMMM-yyyy}", Date);
        //                     ws.Range("B5:D5").Row(1).Merge();
        //                     ws.Range("B5:D5").Style.Font.Bold = true;


        //                     //ReceiptsandPayment


        //                     ws3.Column(1).Width = 30;
        //                     ws3.Row(1).Height = 50;
        //                     ws3.AddPicture(iitlogo)
        //                     .MoveTo(ws3.Cell(1, 1).Address)
        //                     .Scale(0.5);
        //                     ws3.Column(5).Width = 30;
        //                     ws3.Row(1).Height = 50;
        //                     ws3.AddPicture(icsrlogo)
        //                     .MoveTo(ws3.Cell(1, 5).Address)
        //                     .Scale(0.5);
        //                     ws3.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
        //                     ws3.Range("B1:D1").Row(1).Merge();
        //                     ws3.Range("B1:D1").Style.Font.Bold = true;
        //                     ws3.Range("A7:A100").Style.Font.Bold = true;
        //                     ws3.Cell(2, 2).Value = "Indian Institute of Technology Madras";
        //                     ws3.Range("B2:D2").Row(1).Merge();
        //                     ws3.Range("B2:D2").Style.Font.Bold = true;
        //                     ws3.Cell(3, 2).Value = "Chennai - 600 036";
        //                     ws3.Range("B3:D3").Row(1).Merge();
        //                     ws3.Range("B3:D3").Style.Font.Bold = true;
        //                     ws3.Cell(5, 2).Value = "Receipts & Payments Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", Date);
        //                     ws3.Range("B5:D5").Row(1).Merge();
        //                     ws3.Range("B5:D5").Style.Font.Bold = true;
        //                     ws3.Cell(7, 1).Value = "Receipts";
        //                     ws3.Cell(7, 2).Value = "Notes";
        //                     ws3.Cell(7, 3).Value = Date.Year;
        //                     //ws3.Cell(7, 4).Value = CurrYearQry.Year;
        //                     ws3.Range("A7:E7").Style.Font.Bold = true;

        //                     DataTable dtReceiptsCurr = new DataTable();
        //                     DataTable dtPaymentCurr = new DataTable();

        //                     dtReceiptsCurr = db.GetReceiptsForReceiptsandPayment(Date);
        //                     dtPaymentCurr = db.GetPayementForReceiptsandPayment(Date);
        //                     int Recfirstrow = 8;
        //                     decimal RecCurrIncome = 0;
        //                     foreach (DataRow row in dtReceiptsCurr.Rows)
        //                     {
        //                         ws3.Cell(Recfirstrow, 1).Value = row["AccountHead"].ToString();
        //                         ws3.Cell(Recfirstrow, 2).Value = "";
        //                         ws3.Cell(Recfirstrow, 3).Value = row["Amount"].ToString();
        //                         RecCurrIncome += (Convert.ToDecimal(row["Amount"].ToString()));
        //                         Recfirstrow++;
        //                     }

        //                     Recfirstrow = Recfirstrow + 1;

        //                     ws3.Cell(Recfirstrow, 1).Value = "Total Revenues";
        //                     ws3.Cell(Recfirstrow, 3).Value = RecCurrIncome;
        //                     ws3.Range("A" + Recfirstrow + ":E" + Recfirstrow).Style.Font.Bold = true;

        //                     ws3.Cell(Recfirstrow + 2, 1).Value = "Payment";
        //                     ws3.Cell(Recfirstrow + 2, 2).Value = "Notes";
        //                     ws3.Cell(Recfirstrow + 2, 3).Value = Date.Year;
        //                     ws3.Range("A" + (Recfirstrow + 2) + ":E" + (Recfirstrow + 2)).Style.Font.Bold = true;

        //                     int RecSecondrow = Recfirstrow + 3;
        //                     decimal RecCurrExpense = 0;
        //                     foreach (DataRow row in dtPaymentCurr.Rows)
        //                     {
        //                         ws3.Cell(RecSecondrow, 1).Value = row["AccountHead"].ToString();
        //                         ws3.Cell(RecSecondrow, 2).Value = "";
        //                         ws3.Cell(RecSecondrow, 3).Value = row["Amount"].ToString();
        //                         RecCurrExpense += (Convert.ToDecimal(row["Amount"].ToString()));
        //                         RecSecondrow++;
        //                     }


        //                     DataTable dtColumnsPrevYrAdvanceandSett = new DataTable();
        //                     dtColumnsPrevYrAdvanceandSett = db.GetPrevYearAdvanceandSett();

        //                     int PrevYrAdvanceandSettrow = 1;
        //                     ws4.Cell(PrevYrAdvanceandSettrow, 1).Value = "Ref Number";
        //                     ws4.Cell(PrevYrAdvanceandSettrow, 2).Value = "Amount";
        //                     decimal PrevYrAdvanceandSettAmt = 0;
        //                     foreach (DataRow row in dtColumnsPrevYrAdvanceandSett.Rows)
        //                     {
        //                         ws4.Cell(PrevYrAdvanceandSettrow, 1).Value = row["RefNumber"].ToString();
        //                         ws4.Cell(PrevYrAdvanceandSettrow, 2).Value = row["Amount"].ToString();
        //                         PrevYrAdvanceandSettAmt += (Convert.ToDecimal(row["Amount"].ToString()));
        //                         PrevYrAdvanceandSettrow++;
        //                     }
        //                     DataTable dtColumnsCurrYrAdvanceOus = new DataTable();
        //                     dtColumnsCurrYrAdvanceOus = db.GetcurrentyearAdvanceOustanding();

        //                     int CurrYrAdvanceOustrow = 1;
        //                     ws5.Cell(CurrYrAdvanceOustrow, 1).Value = "Ref Number";
        //                     ws5.Cell(CurrYrAdvanceOustrow, 2).Value = "Amount";
        //                     decimal CurrYrAdvanceOusAmt = 0;
        //                     foreach (DataRow row in dtColumnsCurrYrAdvanceOus.Rows)
        //                     {
        //                         ws5.Cell(CurrYrAdvanceOustrow, 1).Value = row["RefNumber"].ToString();
        //                         ws5.Cell(CurrYrAdvanceOustrow, 2).Value = row["Amount"].ToString();
        //                         CurrYrAdvanceOusAmt += (Convert.ToDecimal(row["Amount"].ToString()));
        //                         CurrYrAdvanceOustrow++;
        //                     }
        //                     ws3.Cell(RecSecondrow +1, 1).Value = "Previous year Advance";
        //                     ws3.Cell(RecSecondrow+1, 3).Value = PrevYrAdvanceandSettAmt;
        //                     ws3.Cell(RecSecondrow +2, 1).Value = "Current year Advance";
        //                     ws3.Cell(RecSecondrow + 2, 3).Value = -CurrYrAdvanceOusAmt;

        //                     RecSecondrow = RecSecondrow + 3;
        //                     ws3.Cell(RecSecondrow, 1).Value = "Total Paymnet";
        //                     ws3.Cell(RecSecondrow, 3).Value = RecCurrExpense - CurrYrAdvanceOusAmt+ PrevYrAdvanceandSettAmt;
        //                     ws3.Range("A" + RecSecondrow + ":E" + RecSecondrow).Style.Font.Bold = true;


        //                     ws3.Cell(RecSecondrow + 2, 1).Value = "Project Fund";
        //                     ws3.Cell(RecSecondrow + 2, 3).Value = RecCurrIncome -( RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt);
        //                     ws3.Range("A" + (RecSecondrow + 2) + ":E" + (RecSecondrow + 2)).Style.Font.Bold = true;

        //                     //Profit and Loss
        //                     DataTable dtIncomeCurr = new DataTable();
        //                     DataTable dtExpenseCurr = new DataTable();




        //                     ws2.Column(1).Width = 30;
        //                     ws2.Row(1).Height = 50;
        //                     ws2.AddPicture(iitlogo)
        //                     .MoveTo(ws2.Cell(1, 1).Address)
        //                     .Scale(0.5);
        //                     ws2.Column(5).Width = 30;
        //                     ws2.Row(1).Height = 50;
        //                     ws2.AddPicture(icsrlogo)
        //                     .MoveTo(ws2.Cell(1, 5).Address)
        //                     .Scale(0.5);
        //                     ws2.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
        //                     ws2.Range("B1:D1").Row(1).Merge();
        //                     ws2.Range("B1:D1").Style.Font.Bold = true;
        //                     ws2.Range("A7:A100").Style.Font.Bold = true;
        //                     ws2.Cell(2, 2).Value = "Indian Institute of Technology Madras";
        //                     ws2.Range("B2:D2").Row(1).Merge();
        //                     ws2.Range("B2:D2").Style.Font.Bold = true;
        //                     ws2.Cell(3, 2).Value = "Chennai - 600 036";
        //                     ws2.Range("B3:D3").Row(1).Merge();
        //                     ws2.Range("B3:D3").Style.Font.Bold = true;
        //                     ws2.Cell(5, 2).Value = "Income Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", Date);
        //                     ws2.Range("B5:D5").Row(1).Merge();
        //                     ws2.Range("B5:D5").Style.Font.Bold = true;


        //                     ws2.Cell(7, 1).Value = "Revenue";
        //                     ws2.Cell(7, 3).Value = "Notes";
        //                     ws2.Cell(7, 4).Value = Date.Year;
        //                     ws2.Range("A7:E7").Style.Font.Bold = true;



        //                     dtIncomeCurr = db.GetProfitandLossIncome(Date);
        //                     dtExpenseCurr = db.GetProfitandLossExpense(Date);
        //                     int firstrow = 8;
        //                     decimal CurrIncome = 0;
        //                     DataRow[] draIncomeCurr = new DataRow[dtIncomeCurr.Rows.Count];
        //                     dtIncomeCurr.Rows.CopyTo(draIncomeCurr, 0);

        //                     for (int i = 0; i < draIncomeCurr.Count(); i++)
        //                     {

        //                         if (i != 0)
        //                         {
        //                             if (draIncomeCurr.Count() > i + 1)
        //                             {
        //                                 if (draIncomeCurr[i - 1].Field<string>("Groups") != draIncomeCurr[i].Field<string>("Groups"))
        //                                 {
        //                                     ws2.Cell(firstrow, 1).Value = draIncomeCurr[i].Field<string>("Groups");
        //                                 }
        //                             }
        //                         }
        //                         else
        //                             ws2.Cell(firstrow, 1).Value = draIncomeCurr[i].Field<string>("Groups");
        //                         ws2.Cell(firstrow, 2).Value = draIncomeCurr[i].Field<string>("AccountHead");
        //                         ws2.Cell(firstrow, 4).Value = draIncomeCurr[i].Field<decimal>("Amount");
        //                         CurrIncome += (Convert.ToDecimal(draIncomeCurr[i].Field<decimal>("Amount")));
        //                         firstrow++;
        //                     }


        //                     firstrow = firstrow + 1;

        //                     ws2.Cell(firstrow, 1).Value = "Total Revenues";
        //                     ws2.Cell(firstrow, 4).Value = CurrIncome;
        //                     ws2.Range("A" + firstrow + ":E" + firstrow).Style.Font.Bold = true;

        //                     ws2.Cell(firstrow + 2, 1).Value = "Expenses";
        //                     ws2.Cell(firstrow + 2, 3).Value = "Notes";
        //                     ws2.Cell(firstrow + 2, 4).Value = Date.Year;
        //                     ws2.Range("A" + (firstrow + 2) + ":E" + (firstrow + 2)).Style.Font.Bold = true;

        //                     int Secondrow = firstrow + 3;
        //                     decimal CurrExpense = 0;

        //                     DataRow[] draExpenseCurr = new DataRow[dtExpenseCurr.Rows.Count];
        //                     dtExpenseCurr.Rows.CopyTo(draExpenseCurr, 0);

        //                     for (int i = 0; i < draExpenseCurr.Count(); i++)
        //                     {

        //                         if (i != 0)
        //                         {
        //                             if (draExpenseCurr.Count() > i + 1)
        //                             {
        //                                 if (draExpenseCurr[i - 1].Field<string>("Groups") != draExpenseCurr[i].Field<string>("Groups"))
        //                                 {
        //                                     ws2.Cell(Secondrow, 1).Value = draExpenseCurr[i].Field<string>("Groups");
        //                                 }
        //                             }
        //                         }
        //                         else
        //                             ws2.Cell(Secondrow, 1).Value = draExpenseCurr[i].Field<string>("Groups");
        //                         ws2.Cell(Secondrow, 2).Value = draExpenseCurr[i].Field<string>("AccountHead");
        //                         ws2.Cell(Secondrow, 4).Value = draExpenseCurr[i].Field<decimal>("Amount");
        //                         CurrExpense += (Convert.ToDecimal(draExpenseCurr[i].Field<decimal>("Amount")));
        //                         Secondrow++;
        //                     }

        //                     Secondrow = Secondrow + 1;




        //                     ws2.Cell(Secondrow, 1).Value = "Total Expenses";
        //                     ws2.Cell(Secondrow, 4).Value = CurrExpense;
        //                     ws2.Range("A" + Secondrow + ":E" + Secondrow).Style.Font.Bold = true;

        //                     ws2.Cell(Secondrow + 2, 1).Value = "Net Income";
        //                     ws2.Cell(Secondrow + 2, 4).Value = CurrIncome - CurrExpense;
        //                     ws2.Range("A" + (Secondrow + 2) + ":E" + (Secondrow + 2)).Style.Font.Bold = true;



        //                     BalanceSheetModel model = new BalanceSheetModel();
        //                     DataTable dtColumns = new DataTable();
        //                     string json = "";

        //                     ws1.Cell(1, 3).Value = Date.Year;
        //                   //  ws1.Cell(1, 5).Value = CurrYearQry.Year;
        //                     ws1.Cell(2, 1).Value = "Particulars";
        //                     ws1.Cell(2, 2).Value = "Notes";
        //                     ws1.Cell(2, 3).Value = "Amount";
        //                     ws1.Cell(2, 4).Value = "Gross Amount";

        //                     //Owner's Equity
        //                     ws1.Cell(4, 1).Value = "Owner's Equity";
        //                     ws1.Cell(5, 1).Value = "Capital Account";
        //                     ws1.Cell(5, 2).Value = "1";
        //                     model = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 372 }, false, Date);
        //                     ws1.Cell(5, 3).Value = model.CurrGrossAmt;
        //                     ws1.Cell(5, 4).Value = model.CurrGrossAmt;
        //                     //BalanceSheetModel modelA1 = ReportService.GetBalanceSheetNote(new int[] { 0 }, 372, false, Date);
        //                     //ws1.Cell(5, 5).Value = modelA1.CurrGrossAmt;
        //                     //ws1.Cell(5, 6).Value = modelA1.CurrGrossAmt;


        //                     //Retained earnings  (P and L)
        //                     ws1.Cell(7, 1).Value = "Retained earnings";
        //                     ws1.Cell(7, 2).Value = "2";
        //                   //  BalanceSheetModel modelA2 = ReportService.GetRetainedEarnings(Date);
        //                     ws1.Cell(7, 3).Value = CurrIncome - CurrExpense;
        //                     ws1.Cell(7, 4).Value = CurrIncome - CurrExpense;
        //                     //BalanceSheetModel modelA3 = ReportService.GetRetainedEarnings(CurrFromDate, CurrToDate);
        //                     //ws1.Cell(7, 5).Value = modelA3.GrossAmt;
        //                     //ws1.Cell(7, 6).Value = modelA3.GrossAmt;


        //                     //Long-Term Liabilities  (Receipts and Payments)
        //                     ws1.Cell(9, 1).Value = "Long-Term Liabilities";
        //                     ws1.Cell(10, 1).Value = "Project Fund ";
        //                     ws1.Cell(10, 2).Value = "3";
        //                  //   BalanceSheetModel modelA4 = ReportService.GetProjectFund(Date);
        //                     ws1.Cell(10, 3).Value = RecCurrIncome - (RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt);

        //                     BalanceSheetModel modelFF = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 443 }, false, Date);
        //                     ws1.Cell(11, 1).Value = "Foreign Currency Fluctuation";
        //                     ws1.Cell(11, 3).Value = modelFF.CurrGrossAmt;
        //                     ws1.Cell(11, 4).Value = (RecCurrIncome - (RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt)) + (modelFF.CurrDecGrossAmt);

        //                     //BalanceSheetModel modelA5 = ReportService.GetProjectFund(CurrFromDate, CurrToDate);
        //                     //ws1.Cell(10, 5).Value = modelA5.GrossAmt;
        //                     //ws1.Cell(10, 6).Value = modelA5.GrossAmt;

        //                     //Current Liabilities
        //                     ws1.Cell(12, 1).Value = "Current Liabilities";
        //                     ws1.Cell(13, 1).Value = "Accounts payable";
        //                     ws1.Cell(13, 2).Value = "4";
        //                     BalanceSheetModel model1 = ReportService.GetBalanceSheetNote(new int[] { 18, 19 }, new int[] { 0 }, false, Date);
        //                     string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.Note);
        //                     DataTable dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
        //                     int Prevrow = 14;
        //                     foreach (DataRow row in dtColumns1.Rows)
        //                     {
        //                         ws1.Cell(Prevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(Prevrow, 3).Value = row["CurrAmount"].ToString();
        //                         Prevrow++;
        //                     }
        //                     ws1.Cell(Prevrow, 4).Value = model1.CurrGrossAmt;


        //                     //Short-term loans
        //                     int Shorttermrow = Prevrow + 2;
        //                     ws1.Cell(Shorttermrow, 1).Value = "Short-term loans";
        //                     ws1.Cell(Shorttermrow, 2).Value = "5";

        //                     //Income taxes payable
        //                     int IncometaxesPrevrow = Shorttermrow + 2;
        //                     ws1.Cell(IncometaxesPrevrow, 1).Value = "Income taxes payable";
        //                     ws1.Cell(IncometaxesPrevrow, 2).Value = "6";
        //                     BalanceSheetModel model3 = ReportService.GetBalanceSheetNote(new int[] { 15, 17 }, new int[] { 0 }, false, Date);
        //                     string json3 = Newtonsoft.Json.JsonConvert.SerializeObject(model3.Note);
        //                     DataTable dtColumns3 = JsonConvert.DeserializeObject<DataTable>(json3);
        //                     IncometaxesPrevrow = IncometaxesPrevrow + 1;
        //                     foreach (DataRow row in dtColumns3.Rows)
        //                     {
        //                         ws1.Cell(IncometaxesPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(IncometaxesPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         IncometaxesPrevrow++;
        //                     }
        //                     ws1.Cell(IncometaxesPrevrow, 4).Value = model3.CurrGrossAmt;


        //                     //GST payable
        //                     int GSTpayablePrevrow = IncometaxesPrevrow + 2;
        //                     ws1.Cell(GSTpayablePrevrow, 1).Value = "GST payable";
        //                     ws1.Cell(GSTpayablePrevrow, 2).Value = "7";
        //                     BalanceSheetModel model5 = ReportService.GetBalanceSheetNote(new int[] { 13,14, 16,63 }, new int[] { 0 }, false, Date);
        //                     string json5 = Newtonsoft.Json.JsonConvert.SerializeObject(model5.Note);
        //                     DataTable dtColumns5 = JsonConvert.DeserializeObject<DataTable>(json5);
        //                     GSTpayablePrevrow = GSTpayablePrevrow + 1;
        //                     foreach (DataRow row in dtColumns5.Rows)
        //                     {
        //                         ws1.Cell(GSTpayablePrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(GSTpayablePrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         GSTpayablePrevrow++;
        //                     }
        //                     ws1.Cell(GSTpayablePrevrow, 4).Value = model5.CurrGrossAmt;


        //                     //Current portion of long-term debt
        //                     int Currentportionrow = GSTpayablePrevrow + 2;
        //                     ws1.Cell(Currentportionrow, 1).Value = "Current portion of long-term debt";
        //                     ws1.Cell(Currentportionrow, 2).Value = "8";

        //                     //Fixed (Long-Term) Assets
        //                     int FixedLongTermrow = Currentportionrow + 3;
        //                     ws1.Cell(Currentportionrow, 1).Value = "Fixed (Long-Term) Assets";

        //                     //Long-term investments
        //                     int LongterminvestmentsPrevrow = Currentportionrow + 2;
        //                     ws1.Cell(LongterminvestmentsPrevrow, 1).Value = "Long-term investments";
        //                     ws1.Cell(LongterminvestmentsPrevrow, 2).Value = "9";
        //                     BalanceSheetModel model7 = ReportService.GetBalanceSheetNote(new int[] { 7 }, new int[] { 0 }, true, Date);
        //                     string json7 = Newtonsoft.Json.JsonConvert.SerializeObject(model7.Note);
        //                     DataTable dtColumns7 = JsonConvert.DeserializeObject<DataTable>(json7);
        //                     LongterminvestmentsPrevrow = LongterminvestmentsPrevrow + 1;
        //                     foreach (DataRow row in dtColumns7.Rows)
        //                     {
        //                         ws1.Cell(LongterminvestmentsPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(LongterminvestmentsPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         LongterminvestmentsPrevrow++;
        //                     }
        //                     ws1.Cell(LongterminvestmentsPrevrow, 4).Value = model7.CurrGrossAmt;


        //                     //Property, plant, and equipment
        //                     int PropertyplantPrevrow = LongterminvestmentsPrevrow + 2;
        //                     ws1.Cell(PropertyplantPrevrow, 1).Value = "Property, plant, and equipment";
        //                     ws1.Cell(PropertyplantPrevrow, 2).Value = "10";
        //                     BalanceSheetModel model9 = ReportService.GetBalanceSheetNote(new int[] { 5 }, new int[] { 435,439}, true, Date);
        //                     string json9 = Newtonsoft.Json.JsonConvert.SerializeObject(model9.Note);
        //                     DataTable dtColumns9 = JsonConvert.DeserializeObject<DataTable>(json9);
        //                     PropertyplantPrevrow = PropertyplantPrevrow + 1;
        //                     foreach (DataRow row in dtColumns9.Rows)
        //                     {
        //                         ws1.Cell(PropertyplantPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(PropertyplantPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         PropertyplantPrevrow++;
        //                     }
        //                     ws1.Cell(PropertyplantPrevrow, 4).Value = model9.CurrGrossAmt;

        //                     //Intangible assets
        //                     int Intangibleassetsrow = PropertyplantPrevrow + 2;
        //                     ws1.Cell(Intangibleassetsrow, 1).Value = "Intangible assets";
        //                     ws1.Cell(Intangibleassetsrow, 2).Value = "11";

        //                     //Current Assets
        //                     int CurrentAssetsrow = Intangibleassetsrow + 2;
        //                     ws1.Cell(CurrentAssetsrow, 1).Value = "Current Assets";

        //                     //Bank 
        //                     int BankPrevrow = CurrentAssetsrow + 2;
        //                     ws1.Cell(BankPrevrow, 1).Value = "Bank";
        //                     ws1.Cell(BankPrevrow, 2).Value = "12";
        //                     BalanceSheetModel model11 = ReportService.GetBalanceSheetNote(new int[] { 38 }, new int[] { 0 }, true, Date);
        //                     string json11 = Newtonsoft.Json.JsonConvert.SerializeObject(model11.Note);
        //                     DataTable dtColumns11 = JsonConvert.DeserializeObject<DataTable>(json11);
        //                     BankPrevrow = BankPrevrow + 1;
        //                     foreach (DataRow row in dtColumns11.Rows)
        //                     {
        //                         ws1.Cell(BankPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(BankPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         BankPrevrow++;
        //                     }
        //                     ws1.Cell(BankPrevrow, 4).Value = model11.CurrGrossAmt;

        //                     //Imprest 
        //                     int ImprestPrevrow = BankPrevrow + 2;
        //                     ws1.Cell(ImprestPrevrow, 1).Value = "Imprest";
        //                     ws1.Cell(ImprestPrevrow, 2).Value = "13";
        //                     BalanceSheetModel model13 = ReportService.GetBalanceSheetNote(new int[] { 61 }, new int[] { 0 }, true, Date);
        //                     string json13 = Newtonsoft.Json.JsonConvert.SerializeObject(model13.Note);
        //                     DataTable dtColumns13 = JsonConvert.DeserializeObject<DataTable>(json13);
        //                     ImprestPrevrow = ImprestPrevrow + 1;
        //                     foreach (DataRow row in dtColumns13.Rows)
        //                     {
        //                         ws1.Cell(ImprestPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(ImprestPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         ImprestPrevrow++;
        //                     }
        //                     ws1.Cell(ImprestPrevrow, 4).Value = model13.CurrGrossAmt;



        //                     //Accounts receivable 
        //                     int AccountsreceivablePrevrow = ImprestPrevrow + 2;
        //                     ws1.Cell(AccountsreceivablePrevrow, 1).Value = "Accounts receivable";
        //                     ws1.Cell(AccountsreceivablePrevrow, 2).Value = "14";
        //                     BalanceSheetModel model15 = ReportService.GetBalanceSheetNote(new int[] { 40, 34, 41,62 }, new int[] { 0 }, true, Date);
        //                     string json15 = Newtonsoft.Json.JsonConvert.SerializeObject(model15.Note);
        //                     DataTable dtColumns15 = JsonConvert.DeserializeObject<DataTable>(json15);
        //                     AccountsreceivablePrevrow = AccountsreceivablePrevrow + 1;
        //                     foreach (DataRow row in dtColumns15.Rows)
        //                     {
        //                         ws1.Cell(AccountsreceivablePrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(AccountsreceivablePrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         AccountsreceivablePrevrow++;
        //                     }
        //                     ws1.Cell(AccountsreceivablePrevrow, 4).Value = model15.CurrGrossAmt;

        //                     //Prepaid expenses
        //                     int Prepaidexpensesrow = AccountsreceivablePrevrow + 2;
        //                     ws1.Cell(Prepaidexpensesrow, 1).Value = "Prepaid expenses";
        //                     ws1.Cell(Prepaidexpensesrow, 2).Value = "15";

        //                     //Vendor Advance  
        //                     int VendorAdvancePrevrow = Prepaidexpensesrow + 2;
        //                     ws1.Cell(VendorAdvancePrevrow, 1).Value = "Vendor Advance";
        //                     ws1.Cell(VendorAdvancePrevrow, 2).Value = "16";
        //                     BalanceSheetModel model17 = ReportService.GetBalanceSheetNote(new int[] { 36 }, new int[] { 0 }, true, Date);
        //                     string json17 = Newtonsoft.Json.JsonConvert.SerializeObject(model17.Note);
        //                     DataTable dtColumns17 = JsonConvert.DeserializeObject<DataTable>(json17);
        //                     VendorAdvancePrevrow = VendorAdvancePrevrow + 1;
        //                     foreach (DataRow row in dtColumns17.Rows)
        //                     {
        //                         ws1.Cell(VendorAdvancePrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(VendorAdvancePrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         VendorAdvancePrevrow++;
        //                     }
        //                     ws1.Cell(VendorAdvancePrevrow, 4).Value = model17.CurrGrossAmt;

        //                     //Short-term investments
        //                     int Shortterminvestmentsrow = VendorAdvancePrevrow + 2;
        //                     ws1.Cell(Shortterminvestmentsrow, 1).Value = "Short-term investments";
        //                     ws1.Cell(Shortterminvestmentsrow, 2).Value = "17";

        //                     //Other  
        //                     int OtherPrevrow = Shortterminvestmentsrow + 2;
        //                     ws1.Cell(OtherPrevrow, 1).Value = "Other";
        //                     ws1.Cell(OtherPrevrow, 2).Value = "18";
        //                     BalanceSheetModel model19 = ReportService.GetBalanceSheetNote(new int[] { 37, 42 }, new int[] { 0 }, true, Date);
        //                     string json19 = Newtonsoft.Json.JsonConvert.SerializeObject(model19.Note);
        //                     DataTable dtColumns19 = JsonConvert.DeserializeObject<DataTable>(json19);
        //                     OtherPrevrow = OtherPrevrow + 1;
        //                     foreach (DataRow row in dtColumns19.Rows)
        //                     {
        //                         ws1.Cell(OtherPrevrow, 1).Value = row["Head"].ToString();
        //                         ws1.Cell(OtherPrevrow, 3).Value = row["CurrAmount"].ToString();
        //                         OtherPrevrow++;
        //                     }
        //                     ws1.Cell(OtherPrevrow, 4).Value = model19.CurrGrossAmt;

        //                     ws1.Cell(OtherPrevrow + 1, 1).Value = "Tax Deposits";
        //                     BalanceSheetModel model21 = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 9 }, true, Date);
        //                     ws1.Cell(OtherPrevrow + 1, 3).Value = model21.CurrGrossAmt;
        //                     ws1.Cell(OtherPrevrow + 1, 4).Value = model21.CurrGrossAmt;



        //                     //First Sheet

        //                    // decimal PrevownerEq = 0; decimal PrevLongTerm = 0; decimal PrevCurrentLiabilities = 0;
        //                     decimal CurrownerEq = 0; decimal CurrLongTerm = 0; decimal CurrCurrentLiabilities = 0;
        //                   //  decimal PrevTotalLiability = 0;
        //                     decimal CurrTotalLiability = 0;

        //                   //  decimal PrevFixedAssets = 0; decimal PrevCurrentAssets = 0; decimal PrevOtherAssets = 0;
        //                     decimal CurrFixedAssets = 0; decimal CurrCurrentAssets = 0; decimal CurrOtherAssets = 0;
        //                    // decimal PrevTotalAssets = 0;
        //                     decimal CurrTotalAssets = 0;
        //                   var revDataForBalanceSheet = db.GetPrevDataForBalanceSheet();
        //                     ws.Cell(7, 1).Value = "Current Liabilities";
        //                     ws.Cell(7, 2).Value = "Notes";
        //                     ws.Cell(7, 3).Value = Date.Year;
        //                     ws.Range("A7:E7").Style.Font.Bold = true;

        //                     ws.Cell(8, 1).Value = "Owner's Equity";
        //                     ws.Cell(9, 1).Value = "ICSR Capital Fund";
        //                     ws.Cell(9, 2).Value = "1";
        //                     ws.Cell(9, 3).Value = model.CurrDecGrossAmt + revDataForBalanceSheet.Item1;

        //                     ws.Cell(10, 1).Value = "Retained earnings";
        //                     ws.Cell(10, 2).Value = "2";
        //                     ws.Cell(10, 3).Value = CurrIncome - CurrExpense;

        //                     CurrownerEq = model.CurrDecGrossAmt + revDataForBalanceSheet.Item1 + (CurrIncome - CurrExpense);
        //                     ws.Cell(11, 1).Value = "Total owner's equity";
        //                     if (CurrownerEq > 0)
        //                         ws.Cell(11, 3).Value = Convert.ToString(CurrownerEq);
        //                     else if (CurrownerEq == 0)
        //                         ws.Cell(11, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrownerEq);
        //                         ws.Cell(11, 3).Value = "(" + Amt + ")";
        //                     }




        //                     ws.Cell(12, 1).Value = "Long-Term Liabilities";
        //                     ws.Cell(13, 1).Value = "Project Fund ";
        //                     ws.Cell(13, 2).Value = "3";
        //                     ws.Cell(13, 3).Value = RecCurrIncome - (RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt);
        //                     ws.Cell(14, 1).Value = "Prev Year Project Fund ";
        //                     ws.Cell(14, 2).Value = "";
        //                     ws.Cell(14, 3).Value = revDataForBalanceSheet.Item2;
        //                     ws.Cell(15, 1).Value = "Foreign Currency Fluctuation";
        //                     ws.Cell(15, 2).Value = "";
        //                     ws.Cell(15, 3).Value = modelFF.CurrGrossAmt;
        //                     ws.Cell(16, 1).Value = "Total long-term liabilities";
        //                     ws.Cell(16, 3).Value = (RecCurrIncome - (RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt)) + revDataForBalanceSheet.Item2 + (modelFF.CurrDecGrossAmt);

        //                     CurrLongTerm = (RecCurrIncome - (RecCurrExpense - CurrYrAdvanceOusAmt + PrevYrAdvanceandSettAmt)) + revDataForBalanceSheet.Item2 + (modelFF.CurrDecGrossAmt);

        //                     BalanceSheetModel modelPCR = ReportService.GetBalanceSheetNote(new int[] { 0}, new int[] { 54 }, false, Date);

        //                     ws.Cell(17, 1).Value = "Current Liabilities";
        //                     ws.Cell(18, 1).Value = "Project Clearance Receivable";
        //                     ws.Cell(18, 3).Value = modelPCR.CurrGrossAmt;
        //                     ws.Cell(19, 1).Value = "Accounts payable";
        //                     ws.Cell(19, 2).Value = "4";
        //                     ws.Cell(19, 3).Value = model1.CurrGrossAmt;
        //                     ws.Cell(20, 1).Value = "Short-term loans";
        //                     ws.Cell(20, 2).Value = "5";
        //                     ws.Cell(20, 3).Value = "-";
        //                     ws.Cell(21, 1).Value = "Income taxes payable";
        //                     ws.Cell(21, 2).Value = "6";
        //                     ws.Cell(21, 3).Value = model3.CurrGrossAmt;
        //                     ws.Cell(22, 1).Value = "GST payable";
        //                     ws.Cell(22, 2).Value = "7";
        //                     ws.Cell(22, 3).Value = model5.CurrGrossAmt;
        //                     ws.Cell(23, 1).Value = "Current portion of long-term debt";
        //                     ws.Cell(23, 2).Value = "8";
        //                     ws.Cell(23, 3).Value = "-";
        //                     ws.Cell(24, 1).Value = "Total current liabilities";

        //                     CurrCurrentLiabilities = modelPCR.CurrDecGrossAmt + model1.CurrDecGrossAmt + model3.CurrDecGrossAmt + model5.CurrDecGrossAmt;

        //                     if (CurrCurrentLiabilities > 0)
        //                         ws.Cell(24, 3).Value = Convert.ToString(CurrCurrentLiabilities);
        //                     else if (CurrCurrentLiabilities == 0)
        //                         ws.Cell(24, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrCurrentLiabilities);
        //                         ws.Cell(24, 3).Value = "(" + Amt + ")";
        //                     }


        //                     ws.Cell(25, 1).Value = "Total Liabilities and Owner's Equity";
        //                     CurrTotalLiability = CurrownerEq + CurrLongTerm + CurrCurrentLiabilities;

        //                     if (CurrTotalLiability > 0)
        //                         ws.Cell(25, 3).Value = Convert.ToString(CurrTotalLiability);
        //                     else if (CurrTotalLiability == 0)
        //                         ws.Cell(25, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrTotalLiability);
        //                         ws.Cell(25, 3).Value = "(" + Amt + ")";
        //                     }



        //                     ws.Cell(26, 1).Value = "Assets";
        //                     ws.Cell(26, 2).Value = "Notes";
        //                     ws.Cell(26, 3).Value = Date.Year;
        //                     ws.Range("A7:E7").Style.Font.Bold = true;

        //                     ws.Cell(27, 1).Value = "Fixed (Long-Term) Assets";
        //                     ws.Cell(28, 1).Value = "Long-term investments";
        //                     ws.Cell(28, 2).Value = "9";
        //                     ws.Cell(28, 3).Value = model7.CurrGrossAmt;
        //                     ws.Cell(29, 1).Value = "Property, plant, and equipment";
        //                     ws.Cell(29, 2).Value = "10";
        //                     ws.Cell(29, 3).Value = model9.CurrGrossAmt;
        //                     ws.Cell(30, 1).Value = "Intangible assets";
        //                     ws.Cell(30, 2).Value = "11";
        //                     ws.Cell(30, 3).Value = "-";
        //                     ws.Cell(31, 1).Value = "Total fixed assets";
        //                     CurrFixedAssets = model7.CurrDecGrossAmt + model9.CurrDecGrossAmt;
        //                     if (CurrFixedAssets > 0)
        //                         ws.Cell(31, 3).Value = Convert.ToString(CurrFixedAssets);
        //                     else if (CurrFixedAssets == 0)
        //                         ws.Cell(31, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrFixedAssets);
        //                         ws.Cell(31, 3).Value = "(" + Amt + ")";
        //                     }



        //                     BalanceSheetModel modelPCP = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 418 }, true, Date);

        //                     ws.Cell(32, 1).Value = "Current Assets";
        //                     ws.Cell(33, 1).Value = "Project Clearance Payable";
        //                     ws.Cell(33, 3).Value = modelPCP.CurrGrossAmt;
        //                     ws.Cell(34, 1).Value = "Bank";
        //                     ws.Cell(34, 2).Value = "12";
        //                     ws.Cell(34, 3).Value = model11.CurrGrossAmt;
        //                     ws.Cell(35, 1).Value = "Imprest";
        //                     ws.Cell(35, 2).Value = "13";
        //                     ws.Cell(35, 3).Value = model13.CurrGrossAmt;
        //                     ws.Cell(36, 1).Value = "Accounts receivable";
        //                     ws.Cell(36, 2).Value = "14";
        //                     ws.Cell(36, 3).Value = model15.CurrGrossAmt;
        //                     ws.Cell(37, 1).Value = "Prepaid expenses";
        //                     ws.Cell(37, 2).Value = "15";
        //                     ws.Cell(37, 3).Value = "-";
        //                     ws.Cell(38, 1).Value = "Vendor Advance ";
        //                     ws.Cell(38, 2).Value = "16";
        //                     ws.Cell(38, 3).Value = model17.CurrGrossAmt;
        //                     ws.Cell(39, 1).Value = "Short-term investments";
        //                     ws.Cell(39, 2).Value = "17";
        //                     ws.Cell(39, 3).Value = "-";
        //                     ws.Cell(40, 1).Value = "Total current assets";
        //                      CurrCurrentAssets = modelPCP.CurrDecGrossAmt + model11.CurrDecGrossAmt + model13.CurrDecGrossAmt + model15.CurrDecGrossAmt + model17.CurrDecGrossAmt;

        //                     if (CurrCurrentAssets > 0)
        //                         ws.Cell(40, 3).Value = Convert.ToString(CurrCurrentAssets);
        //                     else if (CurrCurrentAssets == 0)
        //                         ws.Cell(40, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrCurrentAssets);
        //                         ws.Cell(40, 3).Value = "(" + Amt + ")";
        //                     }



        //                     ws.Cell(41, 1).Value = "Other Assets";
        //                     ws.Cell(42, 1).Value = "Other";
        //                     ws.Cell(42, 2).Value = "18";
        //                     ws.Cell(43, 1).Value = "Total Other Assets";
        //                     CurrOtherAssets = model19.CurrDecGrossAmt + model21.CurrDecGrossAmt;
        //                     if (CurrOtherAssets > 0)
        //                     {
        //                         ws.Cell(42, 3).Value = Convert.ToString(CurrOtherAssets);
        //                         ws.Cell(43, 3).Value = Convert.ToString(CurrOtherAssets);
        //                     }
        //                     else if (CurrOtherAssets == 0)
        //                     {
        //                         ws.Cell(42, 3).Value = "-";
        //                         ws.Cell(43, 3).Value = "-";
        //                     }
        //                     else
        //                     {
        //                         var Amt = -(CurrOtherAssets);
        //                         ws.Cell(42, 3).Value = "(" + Amt + ")";
        //                         ws.Cell(43, 3).Value = "(" + Amt + ")";
        //                     }

        //                     ws.Cell(46, 1).Value = "Total Assets";
        //                   //  PrevTotalAssets = PrevFixedAssets + PrevCurrentAssets + PrevOtherAssets;
        //                     CurrTotalAssets = CurrFixedAssets + CurrCurrentAssets + CurrOtherAssets;
        //                     if (CurrTotalAssets > 0)
        //                         ws.Cell(46, 3).Value = Convert.ToString(CurrTotalAssets);
        //                     else if (CurrTotalAssets == 0)
        //                         ws.Cell(46, 3).Value = "-";
        //                     else
        //                     {
        //                         var Amt = -(CurrTotalAssets);
        //                         ws.Cell(46, 3).Value = "(" + Amt + ")";
        //                     }


        //                     // First Sheet End



        //                     ws.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //                     ws1.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //                     ws2.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //                     ws3.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //                     //End

        //                     wb.SaveAs(workStream);
        //                     workStream.Position = 0;

        //                 }

        //             }
        //             string fileType = Common.GetMimeType("xls");
        //             Response.AddHeader("Content-Disposition", "filename=Balance Sheet.xls");
        //             return File(workStream, fileType);
        //         }
        //         catch (Exception ex)
        //         {
        //             Infrastructure.IOASException.Instance.HandleMe(
        //(object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
        //             return null;
        //         }
        //     }
        public ActionResult CashFlowStatement()
        {
            return View();
        }
        public FileStreamResult CashFlow(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();
                        DataTable Receipt = db.GetCashFlow(FromDate, ToDate, "'1','2','3','4'");
                        DataTable Expense = db.GetCashFlow(FromDate, ToDate, "'5','6','7','8','9','10','11'");
                        DataTable Sale = db.GetCashFlow(FromDate, ToDate, "'12','13'");
                        DataTable Purchase = db.GetCashFlow(FromDate, ToDate, "'14','15'");
                        DataTable Repayment = db.GetCashFlow(FromDate, ToDate, "'16'");
                        DataTable Paid = db.GetCashFlow(FromDate, ToDate, "'17'");

                        var ws = wb.Worksheets.Add("Cash Flow Statment");

                        var icsrlogo = Server.MapPath(@"~/Content/InstituteLogo/icsr_logo.png");

                        var iitlogo = Server.MapPath(@"~/Content/InstituteLogo/37616b81-4f25-4a72-a506-d28385afe5bf_IITMadras.png");
                        ws.Column(1).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(iitlogo)
                        .MoveTo(ws.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws.Column(5).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(icsrlogo)
                        .MoveTo(ws.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws.Range("B1:D1").Row(1).Merge();
                        ws.Range("B1:D1").Style.Font.Bold = true;
                        ws.Range("A7:A100").Style.Font.Bold = true;
                        ws.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws.Range("B2:D2").Row(1).Merge();
                        ws.Range("B2:D2").Style.Font.Bold = true;
                        ws.Cell(3, 2).Value = "Chennai - 600 036";
                        ws.Range("B3:D3").Row(1).Merge();
                        ws.Range("B3:D3").Style.Font.Bold = true;
                        ws.Cell(5, 2).Value = "Cash Flow Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        ws.Range("B5:D5").Row(1).Merge();
                        ws.Range("B5:D5").Style.Font.Bold = true;


                        ws.Cell(7, 1).Value = "Period Beginning";
                        ws.Cell(7, 2).Value = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                        ws.Cell(8, 1).Value = "Period Ending";
                        ws.Cell(8, 2).Value = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        ws.Cell(9, 1).Value = "Cash at Beginning of Year";
                        ws.Cell(9, 2).Value = db.GetBankBalance(FromDate);
                        ws.Cell(10, 1).Value = "Cash at End of Year";
                        ws.Cell(10, 2).Value = db.GetBankBalance(ToDate);

                        ws.Cell(12, 1).Value = "Operations";
                        ws.Cell(13, 1).Value = "Cash receipts from";

                        int ReceiptRow = 14; decimal ReceiptTotal = 0;
                        foreach (DataRow row in Receipt.Rows)
                        {
                            ws.Cell(ReceiptRow, 1).Value = row["Label"].ToString();
                            ws.Cell(ReceiptRow, 2).Value = row["Amount"].ToString();
                            ReceiptTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            ReceiptRow++;
                        }
                        ws.Cell(18, 1).Value = "Total";
                        ws.Cell(18, 2).Value = ReceiptTotal;

                        ws.Cell(19, 1).Value = "Cash paid for";
                        int ExpenseRow = 20; decimal ExpenseTotal = 0;
                        foreach (DataRow row in Expense.Rows)
                        {
                            ws.Cell(ExpenseRow, 1).Value = row["Label"].ToString();
                            ws.Cell(ExpenseRow, 2).Value = row["Amount"].ToString();
                            ExpenseTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            ExpenseRow++;
                        }
                        ws.Cell(27, 1).Value = "Total";
                        ws.Cell(27, 2).Value = ExpenseTotal;
                        ws.Cell(28, 1).Value = "Net Cash Flow from Operations";
                        ws.Cell(28, 2).Value = ReceiptTotal - ExpenseTotal;

                        ws.Cell(30, 1).Value = "Investing Activities";
                        ws.Cell(31, 1).Value = "Cash receipts from";
                        int SaleRow = 32; decimal SaleTotal = 0;
                        foreach (DataRow row in Sale.Rows)
                        {
                            ws.Cell(SaleRow, 1).Value = row["Label"].ToString();
                            ws.Cell(SaleRow, 2).Value = row["Amount"].ToString();
                            SaleTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            SaleRow++;
                        }
                        ws.Cell(34, 1).Value = "Total";
                        ws.Cell(34, 2).Value = SaleTotal;

                        ws.Cell(35, 1).Value = "Cash paid for";
                        int PurchaseRow = 36; decimal PurchaseTotal = 0;
                        foreach (DataRow row in Purchase.Rows)
                        {
                            ws.Cell(PurchaseRow, 1).Value = row["Label"].ToString();
                            ws.Cell(PurchaseRow, 2).Value = row["Amount"].ToString();
                            PurchaseTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            PurchaseRow++;
                        }
                        ws.Cell(38, 1).Value = "Total";
                        ws.Cell(38, 2).Value = PurchaseTotal;

                        ws.Cell(39, 1).Value = "Net Cash Flow from Investing Activities";
                        ws.Cell(39, 2).Value = SaleTotal - PurchaseTotal;


                        ws.Cell(41, 1).Value = "Financing Activities";
                        ws.Cell(42, 1).Value = "Cash receipts from";
                        int RepaymentRow = 43; decimal RepaymentTotal = 0;
                        foreach (DataRow row in Repayment.Rows)
                        {
                            ws.Cell(RepaymentRow, 1).Value = row["Label"].ToString();
                            ws.Cell(RepaymentRow, 2).Value = row["Amount"].ToString();
                            RepaymentTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            RepaymentRow++;
                        }
                        ws.Cell(44, 1).Value = "Total";
                        ws.Cell(44, 2).Value = RepaymentTotal;

                        ws.Cell(45, 1).Value = "Cash paid for";
                        int PaidRow = 47; decimal PaidTotal = 0;
                        foreach (DataRow row in Paid.Rows)
                        {
                            ws.Cell(PaidRow, 1).Value = row["Label"].ToString();
                            ws.Cell(PaidRow, 2).Value = row["Amount"].ToString();
                            PaidTotal += (Convert.ToDecimal(row["Amount"].ToString()));
                            PaidRow++;
                        }
                        ws.Cell(48, 1).Value = "Total";
                        ws.Cell(48, 2).Value = PaidTotal;

                        ws.Cell(49, 1).Value = "Net Cash Flow from Financing Activities";
                        ws.Cell(49, 2).Value = RepaymentTotal - PaidTotal;

                        ws.Cell(51, 1).Value = "Net Cash Flow";
                        ws.Cell(51, 2).Value = (ReceiptTotal - ExpenseTotal) + (SaleTotal - PurchaseTotal) + (RepaymentTotal - PaidTotal);



                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Cash Flow Statement.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
   (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
        }
        public FileStreamResult BalanceSheet(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        ListDatabaseObjects db = new ListDatabaseObjects();

                        var icsrlogo = Server.MapPath(@"~/Content/InstituteLogo/icsr_logo.png");

                        var iitlogo = Server.MapPath(@"~/Content/InstituteLogo/37616b81-4f25-4a72-a506-d28385afe5bf_IITMadras.png");

                        var ws = wb.Worksheets.Add("Balance Sheet");
                        var ws1 = wb.Worksheets.Add("Notes to B-S");
                        var ws2 = wb.Worksheets.Add("Profit & Loss Account");
                        var ws3 = wb.Worksheets.Add("Receipts and Payment");



                        ws.Column(1).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(iitlogo)
                        .MoveTo(ws.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws.Column(5).Width = 30;
                        ws.Row(1).Height = 50;
                        ws.AddPicture(icsrlogo)
                        .MoveTo(ws.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws.Range("B1:D1").Row(1).Merge();
                        ws.Range("B1:D1").Style.Font.Bold = true;
                        ws.Range("A7:A100").Style.Font.Bold = true;
                        ws.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws.Range("B2:D2").Row(1).Merge();
                        ws.Range("B2:D2").Style.Font.Bold = true;
                        ws.Cell(3, 2).Value = "Chennai - 600 036";
                        ws.Range("B3:D3").Row(1).Merge();
                        ws.Range("B3:D3").Style.Font.Bold = true;
                        ws.Cell(5, 2).Value = "Balance Sheet for the period " + String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        ws.Range("B5:D5").Row(1).Merge();
                        ws.Range("B5:D5").Style.Font.Bold = true;


                        //ReceiptsandPayment


                        ws3.Column(1).Width = 30;
                        ws3.Row(1).Height = 50;
                        ws3.AddPicture(iitlogo)
                        .MoveTo(ws3.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws3.Column(5).Width = 30;
                        ws3.Row(1).Height = 50;
                        ws3.AddPicture(icsrlogo)
                        .MoveTo(ws3.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws3.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws3.Range("B1:D1").Row(1).Merge();
                        ws3.Range("B1:D1").Style.Font.Bold = true;
                        ws3.Range("A7:A100").Style.Font.Bold = true;
                        ws3.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws3.Range("B2:D2").Row(1).Merge();
                        ws3.Range("B2:D2").Style.Font.Bold = true;
                        ws3.Cell(3, 2).Value = "Chennai - 600 036";
                        ws3.Range("B3:D3").Row(1).Merge();
                        ws3.Range("B3:D3").Style.Font.Bold = true;
                        ws3.Cell(5, 2).Value = "Receipts & Payments Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        ws3.Range("B5:D5").Row(1).Merge();
                        ws3.Range("B5:D5").Style.Font.Bold = true;
                        ws3.Cell(7, 1).Value = "Receipts";
                        ws3.Cell(7, 2).Value = "Notes";
                        ws3.Cell(7, 3).Value = "Amount";
                        ws3.Range("A7:E7").Style.Font.Bold = true;


                        DataTable dtPaymentNonICSRCurr = new DataTable();
                        DataTable dtPaymentICSRCurr = new DataTable();


                        var RecData = db.GetReceiptsForReceiptsandPayment(FromDate, ToDate);
                        DataSet PayData = db.GetPayementForReceiptsandPayment(FromDate, ToDate);

                        dtPaymentNonICSRCurr = PayData.Tables[0];
                        dtPaymentICSRCurr = PayData.Tables[1];


                        decimal RecCurrIncome = 0;

                        ws3.Cell(8, 1).Value = "Project Receipt";
                        ws3.Cell(8, 2).Value = "";
                        ws3.Cell(8, 3).Value = RecData.Item2;
                        ws3.Cell(9, 1).Value = "ICSR Receipt";
                        ws3.Cell(9, 2).Value = "";
                        ws3.Cell(9, 3).Value = RecData.Item1;


                        RecCurrIncome = RecData.Item1 + RecData.Item2;



                        ws3.Cell(11, 1).Value = "Total Revenues";
                        ws3.Cell(11, 3).Value = RecCurrIncome;
                        ws3.Range("A" + 11 + ":E" + 11).Style.Font.Bold = true;

                        ws3.Cell(13, 1).Value = "Payment";
                        ws3.Cell(13, 2).Value = "Notes";
                        ws3.Cell(13, 3).Value = "Amount";
                        ws3.Range("A" + 13 + ":E" + 13).Style.Font.Bold = true;

                        ws3.Cell(15, 1).Value = "Project Expense";

                        int RecSecondrow = 16;
                        decimal PayNonICSRCurrExpense = 0;
                        foreach (DataRow row in dtPaymentNonICSRCurr.Rows)
                        {
                            ws3.Cell(RecSecondrow, 1).Value = row["AccountHead"].ToString();
                            ws3.Cell(RecSecondrow, 2).Value = "";
                            ws3.Cell(RecSecondrow, 3).Value = row["Amount"].ToString();
                            PayNonICSRCurrExpense += (Convert.ToDecimal(row["Amount"].ToString()));
                            RecSecondrow++;
                        }

                        ws3.Cell(RecSecondrow, 1).Value = "ICSR Expense";

                        int RecICSRSecondrow = RecSecondrow + 1;
                        decimal PayICSRCurrExpense = 0;
                        foreach (DataRow row in dtPaymentICSRCurr.Rows)
                        {
                            ws3.Cell(RecICSRSecondrow, 1).Value = row["AccountHead"].ToString();
                            ws3.Cell(RecICSRSecondrow, 2).Value = "";
                            ws3.Cell(RecICSRSecondrow, 3).Value = row["Amount"].ToString();
                            PayICSRCurrExpense += (Convert.ToDecimal(row["Amount"].ToString()));
                            RecICSRSecondrow++;
                        }


                        RecICSRSecondrow = RecICSRSecondrow + 2;
                        ws3.Cell(RecICSRSecondrow, 1).Value = "Total Paymnet";
                        ws3.Cell(RecICSRSecondrow, 3).Value = PayICSRCurrExpense + PayNonICSRCurrExpense;
                        ws3.Range("A" + RecICSRSecondrow + ":E" + RecICSRSecondrow).Style.Font.Bold = true;

                        BalanceSheetModel modelIEXP = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 387 }, true, FromDate, ToDate);
                        BalanceSheetModel modelIREC = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 388 }, false, FromDate, ToDate);

                        int Internal = RecICSRSecondrow + 1;
                        ws3.Cell(Internal, 1).Value = "Internal Receipt";
                        ws3.Cell(Internal, 3).Value = modelIREC.CurrGrossAmt;
                        ws3.Cell(Internal + 1, 1).Value = "Internal Expense";
                        ws3.Cell(Internal + 1, 3).Value = modelIEXP.CurrGrossAmt;



                        ws3.Cell(Internal + 2, 1).Value = "Project Fund";
                        ws3.Cell(Internal + 2, 3).Value = (RecCurrIncome - modelIREC.CurrDecGrossAmt) - ((PayICSRCurrExpense + PayNonICSRCurrExpense) - modelIEXP.CurrDecGrossAmt);
                        ws3.Range("A" + (Internal + 2) + ":E" + (Internal + 2)).Style.Font.Bold = true;

                        //Profit and Loss
                        DataTable dtIncomeCurr = new DataTable();
                        DataTable dtExpenseCurr = new DataTable();




                        ws2.Column(1).Width = 30;
                        ws2.Row(1).Height = 50;
                        ws2.AddPicture(iitlogo)
                        .MoveTo(ws2.Cell(1, 1).Address)
                        .Scale(0.5);
                        ws2.Column(5).Width = 30;
                        ws2.Row(1).Height = 50;
                        ws2.AddPicture(icsrlogo)
                        .MoveTo(ws2.Cell(1, 5).Address)
                        .Scale(0.5);
                        ws2.Cell(1, 2).Value = "Centre for Industrial Consultancy & Sponsored Research (IC&SR)";
                        ws2.Range("B1:D1").Row(1).Merge();
                        ws2.Range("B1:D1").Style.Font.Bold = true;
                        ws2.Range("A7:A100").Style.Font.Bold = true;
                        ws2.Cell(2, 2).Value = "Indian Institute of Technology Madras";
                        ws2.Range("B2:D2").Row(1).Merge();
                        ws2.Range("B2:D2").Style.Font.Bold = true;
                        ws2.Cell(3, 2).Value = "Chennai - 600 036";
                        ws2.Range("B3:D3").Row(1).Merge();
                        ws2.Range("B3:D3").Style.Font.Bold = true;
                        ws2.Cell(5, 2).Value = "Income Statement for the period " + String.Format("{0:dd-MMMM-yyyy}", ToDate);
                        ws2.Range("B5:D5").Row(1).Merge();
                        ws2.Range("B5:D5").Style.Font.Bold = true;


                        ws2.Cell(7, 1).Value = "Revenue";
                        ws2.Cell(7, 3).Value = "Notes";
                        ws2.Cell(7, 4).Value = "Amount";
                        ws2.Range("A7:E7").Style.Font.Bold = true;



                        dtIncomeCurr = db.GetProfitandLossIncome(FromDate, ToDate);
                        dtExpenseCurr = db.GetProfitandLossExpense(FromDate, ToDate);
                        int firstrow = 8;
                        decimal CurrIncome = 0;
                        DataRow[] draIncomeCurr = new DataRow[dtIncomeCurr.Rows.Count];
                        dtIncomeCurr.Rows.CopyTo(draIncomeCurr, 0);

                        for (int i = 0; i < draIncomeCurr.Count(); i++)
                        {

                            if (i != 0)
                            {
                                if (draIncomeCurr.Count() > i + 1)
                                {
                                    if (draIncomeCurr[i - 1].Field<string>("Groups") != draIncomeCurr[i].Field<string>("Groups"))
                                    {
                                        ws2.Cell(firstrow, 1).Value = draIncomeCurr[i].Field<string>("Groups");
                                    }
                                }
                            }
                            else
                                ws2.Cell(firstrow, 1).Value = draIncomeCurr[i].Field<string>("Groups");
                            ws2.Cell(firstrow, 2).Value = draIncomeCurr[i].Field<string>("AccountHead");
                            ws2.Cell(firstrow, 4).Value = draIncomeCurr[i].Field<decimal>("Amount");
                            CurrIncome += (Convert.ToDecimal(draIncomeCurr[i].Field<decimal>("Amount")));
                            firstrow++;
                        }


                        firstrow = firstrow + 1;

                        ws2.Cell(firstrow, 1).Value = "Total Revenues";
                        ws2.Cell(firstrow, 4).Value = CurrIncome;
                        ws2.Range("A" + firstrow + ":E" + firstrow).Style.Font.Bold = true;

                        ws2.Cell(firstrow + 2, 1).Value = "Expenses";
                        ws2.Cell(firstrow + 2, 3).Value = "Notes";
                        ws2.Cell(firstrow + 2, 4).Value = "Amount";
                        ws2.Range("A" + (firstrow + 2) + ":E" + (firstrow + 2)).Style.Font.Bold = true;

                        int Secondrow = firstrow + 3;
                        decimal CurrExpense = 0;

                        DataRow[] draExpenseCurr = new DataRow[dtExpenseCurr.Rows.Count];
                        dtExpenseCurr.Rows.CopyTo(draExpenseCurr, 0);

                        for (int i = 0; i < draExpenseCurr.Count(); i++)
                        {

                            if (i != 0)
                            {
                                if (draExpenseCurr.Count() > i + 1)
                                {
                                    if (draExpenseCurr[i - 1].Field<string>("Groups") != draExpenseCurr[i].Field<string>("Groups"))
                                    {
                                        ws2.Cell(Secondrow, 1).Value = draExpenseCurr[i].Field<string>("Groups");
                                    }
                                }
                            }
                            else
                                ws2.Cell(Secondrow, 1).Value = draExpenseCurr[i].Field<string>("Groups");
                            ws2.Cell(Secondrow, 2).Value = draExpenseCurr[i].Field<string>("AccountHead");
                            ws2.Cell(Secondrow, 4).Value = draExpenseCurr[i].Field<decimal>("Amount");
                            CurrExpense += (Convert.ToDecimal(draExpenseCurr[i].Field<decimal>("Amount")));
                            Secondrow++;
                        }

                        Secondrow = Secondrow + 1;




                        ws2.Cell(Secondrow, 1).Value = "Total Expenses";
                        ws2.Cell(Secondrow, 4).Value = CurrExpense;
                        ws2.Range("A" + Secondrow + ":E" + Secondrow).Style.Font.Bold = true;

                        ws2.Cell(Secondrow + 2, 1).Value = "Net Income";
                        ws2.Cell(Secondrow + 2, 4).Value = CurrIncome - CurrExpense;
                        ws2.Range("A" + (Secondrow + 2) + ":E" + (Secondrow + 2)).Style.Font.Bold = true;



                        BalanceSheetModel model = new BalanceSheetModel();
                        DataTable dtColumns = new DataTable();



                        ws1.Cell(2, 1).Value = "Particulars";
                        ws1.Cell(2, 2).Value = "Notes";
                        ws1.Cell(2, 3).Value = "Amount";
                        ws1.Cell(2, 4).Value = "Gross Amount";

                        //Owner's Equity
                        ws1.Cell(4, 1).Value = "Owner's Equity";
                        ws1.Cell(5, 1).Value = "Capital Account";
                        ws1.Cell(5, 2).Value = "1";
                        model = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 372 }, false, FromDate, ToDate);
                        ws1.Cell(5, 3).Value = model.CurrGrossAmt;
                        ws1.Cell(5, 4).Value = model.CurrGrossAmt;



                        //Retained earnings  (P and L)
                        ws1.Cell(7, 1).Value = "Retained earnings";
                        ws1.Cell(7, 2).Value = "2";

                        ws1.Cell(7, 3).Value = CurrIncome - CurrExpense;
                        ws1.Cell(7, 4).Value = CurrIncome - CurrExpense;



                        //Long-Term Liabilities  (Receipts and Payments)
                        ws1.Cell(9, 1).Value = "Long-Term Liabilities";
                        ws1.Cell(10, 1).Value = "Project Fund ";
                        ws1.Cell(10, 2).Value = "3";



                        ws1.Cell(11, 1).Value = "Project Receipts";
                        ws1.Cell(12, 1).Value = "Internal Receipt";
                        ws1.Cell(13, 1).Value = "Project Expense";
                        ws1.Cell(14, 1).Value = "Internal Expenses";
                        ws1.Cell(15, 1).Value = "Foreign Currency Fluctuation";
                        BalanceSheetModel PR = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 136 }, false, FromDate, ToDate);
                        BalanceSheetModel IR = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 388 }, false, FromDate, ToDate);
                        BalanceSheetModel PE = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 134 }, true, FromDate, ToDate);
                        BalanceSheetModel IE = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 387 }, true, FromDate, ToDate);
                        BalanceSheetModel FC = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 443 }, false, FromDate, ToDate);
                        ws1.Cell(11, 3).Value = PR.CurrGrossAmt;
                        ws1.Cell(12, 3).Value = IR.CurrGrossAmt;
                        ws1.Cell(13, 3).Value = PE.CurrGrossAmt;
                        ws1.Cell(14, 3).Value = IE.CurrGrossAmt;
                        ws1.Cell(15, 3).Value = FC.CurrGrossAmt;

                        ws1.Cell(16, 4).Value = ((PR.CurrDecGrossAmt + IR.CurrDecGrossAmt) - (PE.CurrDecGrossAmt + IE.CurrDecGrossAmt)) + FC.CurrDecGrossAmt;



                        //Current Liabilities
                        ws1.Cell(18, 1).Value = "Current Liabilities";
                        ws1.Cell(19, 1).Value = "Accounts payable";
                        ws1.Cell(19, 2).Value = "4";
                        BalanceSheetModel model1 = ReportService.GetBalanceSheetNote(new int[] { 18, 19 }, new int[] { 0 }, false, FromDate, ToDate);
                        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model1.Note);
                        DataTable dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        int Prevrow = 20;
                        foreach (DataRow row in dtColumns1.Rows)
                        {
                            ws1.Cell(Prevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(Prevrow, 3).Value = row["CurrAmount"].ToString();
                            Prevrow++;
                        }
                        ws1.Cell(Prevrow, 4).Value = model1.CurrGrossAmt;


                        //Short-term loans
                        int Shorttermrow = Prevrow + 2;
                        ws1.Cell(Shorttermrow, 1).Value = "Short-term loans";
                        ws1.Cell(Shorttermrow, 2).Value = "5";

                        //Income taxes payable
                        int IncometaxesPrevrow = Shorttermrow + 2;
                        ws1.Cell(IncometaxesPrevrow, 1).Value = "Income taxes payable";
                        ws1.Cell(IncometaxesPrevrow, 2).Value = "6";
                        BalanceSheetModel model3 = ReportService.GetBalanceSheetNote(new int[] { 15, 17 }, new int[] { 0 }, false, FromDate, ToDate);
                        string json3 = Newtonsoft.Json.JsonConvert.SerializeObject(model3.Note);
                        DataTable dtColumns3 = JsonConvert.DeserializeObject<DataTable>(json3);
                        IncometaxesPrevrow = IncometaxesPrevrow + 1;
                        foreach (DataRow row in dtColumns3.Rows)
                        {
                            ws1.Cell(IncometaxesPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(IncometaxesPrevrow, 3).Value = row["CurrAmount"].ToString();
                            IncometaxesPrevrow++;
                        }
                        ws1.Cell(IncometaxesPrevrow, 4).Value = model3.CurrGrossAmt;


                        //GST payable
                        int GSTpayablePrevrow = IncometaxesPrevrow + 2;
                        ws1.Cell(GSTpayablePrevrow, 1).Value = "GST payable";
                        ws1.Cell(GSTpayablePrevrow, 2).Value = "7";
                        BalanceSheetModel model5 = ReportService.GetBalanceSheetNote(new int[] { 13, 14, 16, 63 }, new int[] { 0 }, false, FromDate, ToDate);
                        string json5 = Newtonsoft.Json.JsonConvert.SerializeObject(model5.Note);
                        DataTable dtColumns5 = JsonConvert.DeserializeObject<DataTable>(json5);
                        GSTpayablePrevrow = GSTpayablePrevrow + 1;
                        foreach (DataRow row in dtColumns5.Rows)
                        {
                            ws1.Cell(GSTpayablePrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(GSTpayablePrevrow, 3).Value = row["CurrAmount"].ToString();
                            GSTpayablePrevrow++;
                        }
                        ws1.Cell(GSTpayablePrevrow, 4).Value = model5.CurrGrossAmt;


                        //Current portion of long-term debt
                        int Currentportionrow = GSTpayablePrevrow + 2;
                        ws1.Cell(Currentportionrow, 1).Value = "Current portion of long-term debt";
                        ws1.Cell(Currentportionrow, 2).Value = "8";

                        //Fixed (Long-Term) Assets
                        int FixedLongTermrow = Currentportionrow + 3;
                        ws1.Cell(Currentportionrow, 1).Value = "Fixed (Long-Term) Assets";

                        //Long-term investments
                        int LongterminvestmentsPrevrow = Currentportionrow + 2;
                        ws1.Cell(LongterminvestmentsPrevrow, 1).Value = "Long-term investments";
                        ws1.Cell(LongterminvestmentsPrevrow, 2).Value = "9";
                        BalanceSheetModel model7 = ReportService.GetBalanceSheetNote(new int[] { 7 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json7 = Newtonsoft.Json.JsonConvert.SerializeObject(model7.Note);
                        DataTable dtColumns7 = JsonConvert.DeserializeObject<DataTable>(json7);
                        LongterminvestmentsPrevrow = LongterminvestmentsPrevrow + 1;
                        foreach (DataRow row in dtColumns7.Rows)
                        {
                            ws1.Cell(LongterminvestmentsPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(LongterminvestmentsPrevrow, 3).Value = row["CurrAmount"].ToString();
                            LongterminvestmentsPrevrow++;
                        }
                        ws1.Cell(LongterminvestmentsPrevrow, 4).Value = model7.CurrGrossAmt;


                        //Property, plant, and equipment
                        int PropertyplantPrevrow = LongterminvestmentsPrevrow + 2;
                        ws1.Cell(PropertyplantPrevrow, 1).Value = "Property, plant, and equipment";
                        ws1.Cell(PropertyplantPrevrow, 2).Value = "10";
                        BalanceSheetModel model9 = ReportService.GetBalanceSheetNote(new int[] { 5 }, new int[] { 435, 439 }, true, FromDate, ToDate);
                        string json9 = Newtonsoft.Json.JsonConvert.SerializeObject(model9.Note);
                        DataTable dtColumns9 = JsonConvert.DeserializeObject<DataTable>(json9);
                        PropertyplantPrevrow = PropertyplantPrevrow + 1;
                        foreach (DataRow row in dtColumns9.Rows)
                        {
                            ws1.Cell(PropertyplantPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(PropertyplantPrevrow, 3).Value = row["CurrAmount"].ToString();
                            PropertyplantPrevrow++;
                        }
                        ws1.Cell(PropertyplantPrevrow, 4).Value = model9.CurrGrossAmt;

                        //Intangible assets
                        int Intangibleassetsrow = PropertyplantPrevrow + 2;
                        ws1.Cell(Intangibleassetsrow, 1).Value = "Intangible assets";
                        ws1.Cell(Intangibleassetsrow, 2).Value = "11";

                        //Current Assets
                        int CurrentAssetsrow = Intangibleassetsrow + 2;
                        ws1.Cell(CurrentAssetsrow, 1).Value = "Current Assets";

                        //Bank 
                        int BankPrevrow = CurrentAssetsrow + 2;
                        ws1.Cell(BankPrevrow, 1).Value = "Bank";
                        ws1.Cell(BankPrevrow, 2).Value = "12";
                        BalanceSheetModel model11 = ReportService.GetBalanceSheetNote(new int[] { 38 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json11 = Newtonsoft.Json.JsonConvert.SerializeObject(model11.Note);
                        DataTable dtColumns11 = JsonConvert.DeserializeObject<DataTable>(json11);
                        BankPrevrow = BankPrevrow + 1;
                        foreach (DataRow row in dtColumns11.Rows)
                        {
                            ws1.Cell(BankPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(BankPrevrow, 3).Value = row["CurrAmount"].ToString();
                            BankPrevrow++;
                        }
                        ws1.Cell(BankPrevrow, 4).Value = model11.CurrGrossAmt;

                        //Imprest 
                        int ImprestPrevrow = BankPrevrow + 2;
                        ws1.Cell(ImprestPrevrow, 1).Value = "Imprest";
                        ws1.Cell(ImprestPrevrow, 2).Value = "13";
                        BalanceSheetModel model13 = ReportService.GetBalanceSheetNote(new int[] { 61 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json13 = Newtonsoft.Json.JsonConvert.SerializeObject(model13.Note);
                        DataTable dtColumns13 = JsonConvert.DeserializeObject<DataTable>(json13);
                        ImprestPrevrow = ImprestPrevrow + 1;
                        foreach (DataRow row in dtColumns13.Rows)
                        {
                            ws1.Cell(ImprestPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(ImprestPrevrow, 3).Value = row["CurrAmount"].ToString();
                            ImprestPrevrow++;
                        }
                        ws1.Cell(ImprestPrevrow, 4).Value = model13.CurrGrossAmt;



                        //Accounts receivable 
                        int AccountsreceivablePrevrow = ImprestPrevrow + 2;
                        ws1.Cell(AccountsreceivablePrevrow, 1).Value = "Accounts receivable";
                        ws1.Cell(AccountsreceivablePrevrow, 2).Value = "14";
                        BalanceSheetModel model15 = ReportService.GetBalanceSheetNote(new int[] { 40, 34, 41, 62 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json15 = Newtonsoft.Json.JsonConvert.SerializeObject(model15.Note);
                        DataTable dtColumns15 = JsonConvert.DeserializeObject<DataTable>(json15);
                        AccountsreceivablePrevrow = AccountsreceivablePrevrow + 1;
                        foreach (DataRow row in dtColumns15.Rows)
                        {
                            ws1.Cell(AccountsreceivablePrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(AccountsreceivablePrevrow, 3).Value = row["CurrAmount"].ToString();
                            AccountsreceivablePrevrow++;
                        }
                        ws1.Cell(AccountsreceivablePrevrow, 4).Value = model15.CurrGrossAmt;

                        //Prepaid expenses
                        int Prepaidexpensesrow = AccountsreceivablePrevrow + 2;
                        ws1.Cell(Prepaidexpensesrow, 1).Value = "Prepaid expenses";
                        ws1.Cell(Prepaidexpensesrow, 2).Value = "15";

                        //Vendor Advance  
                        int VendorAdvancePrevrow = Prepaidexpensesrow + 2;
                        ws1.Cell(VendorAdvancePrevrow, 1).Value = "Vendor Advance";
                        ws1.Cell(VendorAdvancePrevrow, 2).Value = "16";
                        BalanceSheetModel model17 = ReportService.GetBalanceSheetNote(new int[] { 36 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json17 = Newtonsoft.Json.JsonConvert.SerializeObject(model17.Note);
                        DataTable dtColumns17 = JsonConvert.DeserializeObject<DataTable>(json17);
                        VendorAdvancePrevrow = VendorAdvancePrevrow + 1;
                        foreach (DataRow row in dtColumns17.Rows)
                        {
                            ws1.Cell(VendorAdvancePrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(VendorAdvancePrevrow, 3).Value = row["CurrAmount"].ToString();
                            VendorAdvancePrevrow++;
                        }
                        ws1.Cell(VendorAdvancePrevrow, 4).Value = model17.CurrGrossAmt;

                        //Short-term investments
                        int Shortterminvestmentsrow = VendorAdvancePrevrow + 2;
                        ws1.Cell(Shortterminvestmentsrow, 1).Value = "Short-term investments";
                        ws1.Cell(Shortterminvestmentsrow, 2).Value = "17";

                        //Other  
                        int OtherPrevrow = Shortterminvestmentsrow + 2;
                        ws1.Cell(OtherPrevrow, 1).Value = "Other";
                        ws1.Cell(OtherPrevrow, 2).Value = "18";
                        BalanceSheetModel model19 = ReportService.GetBalanceSheetNote(new int[] { 37, 42 }, new int[] { 0 }, true, FromDate, ToDate);
                        string json19 = Newtonsoft.Json.JsonConvert.SerializeObject(model19.Note);
                        DataTable dtColumns19 = JsonConvert.DeserializeObject<DataTable>(json19);
                        OtherPrevrow = OtherPrevrow + 1;
                        foreach (DataRow row in dtColumns19.Rows)
                        {
                            ws1.Cell(OtherPrevrow, 1).Value = row["Head"].ToString();
                            ws1.Cell(OtherPrevrow, 3).Value = row["CurrAmount"].ToString();
                            OtherPrevrow++;
                        }
                        ws1.Cell(OtherPrevrow, 4).Value = model19.CurrGrossAmt;

                        ws1.Cell(OtherPrevrow + 1, 1).Value = "Tax Deposits";
                        BalanceSheetModel model21 = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 9 }, true, FromDate, ToDate);
                        ws1.Cell(OtherPrevrow + 1, 3).Value = model21.CurrGrossAmt;
                        ws1.Cell(OtherPrevrow + 1, 4).Value = model21.CurrGrossAmt;



                        //First Sheet


                        decimal CurrownerEq = 0; decimal CurrLongTerm = 0; decimal CurrCurrentLiabilities = 0;

                        decimal CurrTotalLiability = 0;


                        decimal CurrFixedAssets = 0; decimal CurrCurrentAssets = 0; decimal CurrOtherAssets = 0;

                        decimal CurrTotalAssets = 0;
                        var revDataForBalanceSheet = db.GetPrevDataForBalanceSheet(FromDate);
                        ws.Cell(7, 1).Value = "Current Liabilities";
                        ws.Cell(7, 2).Value = "Notes";
                        ws.Cell(7, 3).Value = "Amount";
                        ws.Range("A7:E7").Style.Font.Bold = true;

                        ws.Cell(8, 1).Value = "Owner's Equity";
                        ws.Cell(9, 1).Value = "ICSR Capital Fund";
                        ws.Cell(9, 2).Value = "1";
                        ws.Cell(9, 3).Value = model.CurrDecGrossAmt + revDataForBalanceSheet.Item1;

                        ws.Cell(10, 1).Value = "Retained earnings";
                        ws.Cell(10, 2).Value = "2";
                        ws.Cell(10, 3).Value = CurrIncome - CurrExpense;

                        CurrownerEq = model.CurrDecGrossAmt + revDataForBalanceSheet.Item1 + (CurrIncome - CurrExpense);
                        ws.Cell(11, 1).Value = "Total owner's equity";
                        if (CurrownerEq > 0)
                            ws.Cell(11, 3).Value = Convert.ToString(CurrownerEq);
                        else if (CurrownerEq == 0)
                            ws.Cell(11, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrownerEq);
                            ws.Cell(11, 3).Value = "(" + Amt + ")";
                        }




                        ws.Cell(12, 1).Value = "Long-Term Liabilities";
                        ws.Cell(13, 1).Value = "Project Fund ";
                        ws.Cell(13, 2).Value = "3";
                        ws.Cell(13, 3).Value = ((PR.CurrDecGrossAmt + IR.CurrDecGrossAmt) - (PE.CurrDecGrossAmt + IE.CurrDecGrossAmt));
                        ws.Cell(14, 1).Value = "Prev Year Project Fund ";
                        ws.Cell(14, 2).Value = "";
                        ws.Cell(14, 3).Value = revDataForBalanceSheet.Item2;
                        ws.Cell(15, 1).Value = "Foreign Currency Fluctuation";
                        ws.Cell(15, 2).Value = "";
                        ws.Cell(15, 3).Value = FC.CurrGrossAmt;
                        ws.Cell(16, 1).Value = "Total long-term liabilities";
                        ws.Cell(16, 3).Value = ((PR.CurrDecGrossAmt + IR.CurrDecGrossAmt) - (PE.CurrDecGrossAmt + IE.CurrDecGrossAmt)) + (FC.CurrDecGrossAmt);

                        CurrLongTerm = ((PR.CurrDecGrossAmt + IR.CurrDecGrossAmt) - (PE.CurrDecGrossAmt + IE.CurrDecGrossAmt)) + (FC.CurrDecGrossAmt);

                        BalanceSheetModel modelPCR = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 54 }, false, FromDate, ToDate);

                        ws.Cell(17, 1).Value = "Current Liabilities";
                        ws.Cell(18, 1).Value = "Project Clearance Receivable";
                        ws.Cell(18, 3).Value = modelPCR.CurrGrossAmt;
                        ws.Cell(19, 1).Value = "Accounts payable";
                        ws.Cell(19, 2).Value = "4";
                        ws.Cell(19, 3).Value = model1.CurrGrossAmt;
                        ws.Cell(20, 1).Value = "Short-term loans";
                        ws.Cell(20, 2).Value = "5";
                        ws.Cell(20, 3).Value = "-";
                        ws.Cell(21, 1).Value = "Income taxes payable";
                        ws.Cell(21, 2).Value = "6";
                        ws.Cell(21, 3).Value = model3.CurrGrossAmt;
                        ws.Cell(22, 1).Value = "GST payable";
                        ws.Cell(22, 2).Value = "7";
                        ws.Cell(22, 3).Value = model5.CurrGrossAmt;
                        ws.Cell(23, 1).Value = "Current portion of long-term debt";
                        ws.Cell(23, 2).Value = "8";
                        ws.Cell(23, 3).Value = "-";
                        ws.Cell(24, 1).Value = "Total current liabilities";

                        CurrCurrentLiabilities = modelPCR.CurrDecGrossAmt + model1.CurrDecGrossAmt + model3.CurrDecGrossAmt + model5.CurrDecGrossAmt;

                        if (CurrCurrentLiabilities > 0)
                            ws.Cell(24, 3).Value = Convert.ToString(CurrCurrentLiabilities);
                        else if (CurrCurrentLiabilities == 0)
                            ws.Cell(24, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrCurrentLiabilities);
                            ws.Cell(24, 3).Value = "(" + Amt + ")";
                        }


                        ws.Cell(25, 1).Value = "Total Liabilities and Owner's Equity";
                        CurrTotalLiability = CurrownerEq + CurrLongTerm + CurrCurrentLiabilities;

                        if (CurrTotalLiability > 0)
                            ws.Cell(25, 3).Value = Convert.ToString(CurrTotalLiability);
                        else if (CurrTotalLiability == 0)
                            ws.Cell(25, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrTotalLiability);
                            ws.Cell(25, 3).Value = "(" + Amt + ")";
                        }



                        ws.Cell(26, 1).Value = "Assets";
                        ws.Cell(26, 2).Value = "Notes";
                        ws.Cell(26, 3).Value = "Amount";
                        ws.Range("A7:E7").Style.Font.Bold = true;

                        ws.Cell(27, 1).Value = "Fixed (Long-Term) Assets";
                        ws.Cell(28, 1).Value = "Long-term investments";
                        ws.Cell(28, 2).Value = "9";
                        ws.Cell(28, 3).Value = model7.CurrGrossAmt;
                        ws.Cell(29, 1).Value = "Property, plant, and equipment";
                        ws.Cell(29, 2).Value = "10";
                        ws.Cell(29, 3).Value = model9.CurrGrossAmt;
                        ws.Cell(30, 1).Value = "Intangible assets";
                        ws.Cell(30, 2).Value = "11";
                        ws.Cell(30, 3).Value = "-";
                        ws.Cell(31, 1).Value = "Total fixed assets";
                        CurrFixedAssets = model7.CurrDecGrossAmt + model9.CurrDecGrossAmt;
                        if (CurrFixedAssets > 0)
                            ws.Cell(31, 3).Value = Convert.ToString(CurrFixedAssets);
                        else if (CurrFixedAssets == 0)
                            ws.Cell(31, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrFixedAssets);
                            ws.Cell(31, 3).Value = "(" + Amt + ")";
                        }



                        BalanceSheetModel modelPCP = ReportService.GetBalanceSheetNote(new int[] { 0 }, new int[] { 418 }, true, FromDate, ToDate);

                        ws.Cell(32, 1).Value = "Current Assets";
                        ws.Cell(33, 1).Value = "Project Clearance Payable";
                        ws.Cell(33, 3).Value = modelPCP.CurrGrossAmt;
                        ws.Cell(34, 1).Value = "Bank";
                        ws.Cell(34, 2).Value = "12";
                        ws.Cell(34, 3).Value = model11.CurrGrossAmt;
                        ws.Cell(35, 1).Value = "Imprest";
                        ws.Cell(35, 2).Value = "13";
                        ws.Cell(35, 3).Value = model13.CurrGrossAmt;
                        ws.Cell(36, 1).Value = "Accounts receivable";
                        ws.Cell(36, 2).Value = "14";
                        ws.Cell(36, 3).Value = model15.CurrGrossAmt;
                        ws.Cell(37, 1).Value = "Prepaid expenses";
                        ws.Cell(37, 2).Value = "15";
                        ws.Cell(37, 3).Value = "-";
                        ws.Cell(38, 1).Value = "Vendor Advance ";
                        ws.Cell(38, 2).Value = "16";
                        ws.Cell(38, 3).Value = model17.CurrGrossAmt;
                        ws.Cell(39, 1).Value = "Short-term investments";
                        ws.Cell(39, 2).Value = "17";
                        ws.Cell(39, 3).Value = "-";
                        ws.Cell(40, 1).Value = "Total current assets";
                        CurrCurrentAssets = modelPCP.CurrDecGrossAmt + model11.CurrDecGrossAmt + model13.CurrDecGrossAmt + model15.CurrDecGrossAmt + model17.CurrDecGrossAmt;

                        if (CurrCurrentAssets > 0)
                            ws.Cell(40, 3).Value = Convert.ToString(CurrCurrentAssets);
                        else if (CurrCurrentAssets == 0)
                            ws.Cell(40, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrCurrentAssets);
                            ws.Cell(40, 3).Value = "(" + Amt + ")";
                        }



                        ws.Cell(41, 1).Value = "Other Assets";
                        ws.Cell(42, 1).Value = "Other";
                        ws.Cell(42, 2).Value = "18";
                        ws.Cell(43, 1).Value = "Total Other Assets";
                        CurrOtherAssets = model19.CurrDecGrossAmt + model21.CurrDecGrossAmt;
                        if (CurrOtherAssets > 0)
                        {
                            ws.Cell(42, 3).Value = Convert.ToString(CurrOtherAssets);
                            ws.Cell(43, 3).Value = Convert.ToString(CurrOtherAssets);
                        }
                        else if (CurrOtherAssets == 0)
                        {
                            ws.Cell(42, 3).Value = "-";
                            ws.Cell(43, 3).Value = "-";
                        }
                        else
                        {
                            var Amt = -(CurrOtherAssets);
                            ws.Cell(42, 3).Value = "(" + Amt + ")";
                            ws.Cell(43, 3).Value = "(" + Amt + ")";
                        }

                        ws.Cell(46, 1).Value = "Total Assets";
                        //  PrevTotalAssets = PrevFixedAssets + PrevCurrentAssets + PrevOtherAssets;
                        CurrTotalAssets = CurrFixedAssets + CurrCurrentAssets + CurrOtherAssets;
                        if (CurrTotalAssets > 0)
                            ws.Cell(46, 3).Value = Convert.ToString(CurrTotalAssets);
                        else if (CurrTotalAssets == 0)
                            ws.Cell(46, 3).Value = "-";
                        else
                        {
                            var Amt = -(CurrTotalAssets);
                            ws.Cell(46, 3).Value = "(" + Amt + ")";
                        }


                        // First Sheet End



                        ws.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws1.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws2.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws3.Range(4, 3, 200, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        //End

                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }

                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=Balance Sheet.xls");
                return File(workStream, fileType);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
   (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
        }
        public FileStreamResult ICSROH(DateTime Date)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DateTime FromDate = new DateTime(Date.Year, Date.Month, 1);
                        DateTime ToDate = FromDate.AddMonths(1).AddDays(-1);

                        var finQry = context.tblFinYear.Where(m => m.StartDate >= Date || Date <= m.EndDate).FirstOrDefault();

                        ListDatabaseObjects db = new ListDatabaseObjects();

                        DateTime yearFromDate = Convert.ToDateTime(finQry.StartDate);
                        DateTime yearToDate = Date;

                        DataTable dtResult = new DataTable();
                        // dtResult = db.GetICSROH_2(FromDate, Date, "Income");
                        DataTable dtResult1 = new DataTable();
                        //   dtResult1 = db.GetICSROH_2(FromDate, Date, "Expenses");

                        //  var ws = wb.Worksheets.Add("ICSROH Income & Exp");
                        var ws1 = wb.Worksheets.Add("ICSROH Income & Exp");
                        var ws2 = wb.Worksheets.Add("ICSROH I&E Break Up");
                        ws2.Cell(1, 1).Value = "Type";
                        ws2.Cell(1, 2).Value = "Groups";
                        ws2.Cell(1, 3).Value = "Account Head";
                        ws2.Cell(1, 4).Value = "RefNumber";
                        ws2.Cell(1, 5).Value = "Credit";
                        ws2.Cell(1, 6).Value = "Debit";
                        ws2.Cell(1, 7).Value = "Date";
                        ws2.Cell(1, 8).Value = "Remarks";
                        int ledgerexpBreakuprow = 2;
                        DataTable dtResult7 = new DataTable();
                        dtResult7 = db.GetICSROHBreakUp_2(yearFromDate, Date);
                        foreach (DataRow row in dtResult7.Rows)
                        {

                            ws2.Cell(ledgerexpBreakuprow, 1).Value = row["Type"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 2).Value = row["Groups"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 3).Value = row["AccountHead"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 4).Value = row["RefNumber"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 5).Value = row["Credit"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 6).Value = row["Debit"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 7).Value = row["PostedDate"].ToString();
                            ws2.Cell(ledgerexpBreakuprow, 8).Value = row["Remarks"].ToString();
                            ledgerexpBreakuprow++;
                        }


                        ws1.Cell(1, 1).Value = "CENTRE FOR INDUSTRIAL CONSULTANCY & SPONSORED RESEARCH";
                        ws1.Range("A1:C1").Row(1).Merge();
                        ws1.Range("A1:C1").Style.Font.Bold = true;
                        ws1.Range("A1:A100").Style.Font.Bold = true;
                        ws1.Range("A1:A100").Style.Font.Bold = true;
                        ws1.Cell(2, 1).Value = "INCOME  AND EXPENDITURE STATEMENT ";
                        ws1.Range("A2:C2").Row(1).Merge();
                        ws1.Range("A2:C2").Style.Font.Bold = true;
                        ws1.Cell(4, 1).Value = "Income";
                        ws1.Range("A4:C4").Row(1).Merge();
                        ws1.Range("A4:C4").Style.Font.Bold = true;


                        int mainRow = 6;
                        int secSheetAmountCol = 4;
                        int fcon = Date.Month > 3 ? (Date.Month - 3) : (Date.Month + 9);
                        int ficon = Date.Month > 4 ? (Date.Month - 4) : (Date.Month + 8);

                        for (int i = 0; i < fcon; i++)
                        {
                            decimal Amt = 0;
                            int secSheetHeadRow = 6;
                            int secSheetAmountRow = 6;
                            FromDate = yearFromDate.AddMonths(i);
                            ToDate = FromDate.AddMonths(1);
                            ToDate = ToDate.AddTicks(-1);
                            if (i == ficon)
                                ToDate = Date.AddDays(1).AddTicks(-1);
                            DataTable dt = new DataTable();
                            dt = db.GetICSROH_2(FromDate, ToDate, "Income");

                            if (i == 0)
                            {
                                string month = FromDate.ToString("MMMM");
                                ws1.Cell(5, 3).Value = month;

                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    if (k == 0)
                                        ws1.Cell(secSheetHeadRow, 1).Value = dt.Rows[k]["Groups"].ToString();
                                    if (k != 0)
                                    {
                                        if (dt.Rows.Count > k + 1)
                                        {
                                            if (dt.Rows[k - 1]["Groups"].ToString() != dt.Rows[k]["Groups"].ToString())
                                            {
                                                ws1.Cell(secSheetHeadRow, 1).Value = dt.Rows[k]["Groups"].ToString();
                                            }
                                        }
                                    }
                                    ws1.Cell(secSheetHeadRow, 2).Value = dt.Rows[k]["AccountHead"].ToString();
                                    ws1.Cell(secSheetHeadRow, 3).Value = dt.Rows[k]["Amount"].ToString();
                                    secSheetHeadRow++;
                                    mainRow++;
                                    Amt += Convert.ToDecimal(dt.Rows[k]["Amount"]);
                                }
                                ws1.Cell(secSheetHeadRow + 1, 2).Value = "Total";
                                ws1.Cell(secSheetHeadRow + 1, 3).Value = Amt;
                            }
                            else
                            {
                                string month = FromDate.ToString("MMMM");
                                ws1.Cell(5, secSheetAmountCol).Value = month;
                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    ws1.Cell(secSheetAmountRow, secSheetAmountCol).Value = dt.Rows[k]["Amount"].ToString();
                                    secSheetAmountRow++;

                                    Amt += Convert.ToDecimal(dt.Rows[k]["Amount"]);
                                }
                                ws1.Cell(secSheetAmountRow + 1, secSheetAmountCol).Value = Amt;
                            }


                            if (i != 0)
                                secSheetAmountCol++;
                        }


                        ws1.Cell(mainRow + 3, 1).Value = "Expense";
                        ws1.Range("A" + (mainRow + 3) + ":C4").Style.Font.Bold = true;
                        ws1.Range("A" + (mainRow + 3) + ":C4").Row(1).Merge();

                        int ExpsecSheetAmountCol = 4;
                        int dd = Date.Month - 3;
                        for (int i = 0; i < fcon; i++)
                        {
                            int ExpsecSheetHeadRow = mainRow + 4;
                            int ExpsecSheetAmountRow = mainRow + 4;
                            decimal Amt = 0;
                            FromDate = yearFromDate.AddMonths(i);
                            ToDate = FromDate.AddMonths(1);
                            ToDate = ToDate.AddTicks(-1);
                            if (i == ficon)
                                ToDate = Date.AddDays(1).AddTicks(-1);
                            DataTable dt = new DataTable();
                            dt = db.GetICSROH_2(FromDate, ToDate, "Expenses");

                            if (i == 0)
                            {

                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    if (k == 0)
                                        ws1.Cell(ExpsecSheetHeadRow, 1).Value = dt.Rows[k]["Groups"].ToString();
                                    if (k != 0)
                                    {
                                        if (dt.Rows.Count > k + 1)
                                        {
                                            if (dt.Rows[k - 1]["Groups"].ToString() != dt.Rows[k]["Groups"].ToString())
                                            {
                                                ws1.Cell(ExpsecSheetHeadRow, 1).Value = dt.Rows[k]["Groups"].ToString();
                                            }
                                        }
                                    }

                                    ws1.Cell(ExpsecSheetHeadRow, 2).Value = dt.Rows[k]["AccountHead"].ToString();
                                    ws1.Cell(ExpsecSheetHeadRow, 3).Value = dt.Rows[k]["Amount"].ToString();
                                    ExpsecSheetHeadRow++;
                                    Amt += Convert.ToDecimal(dt.Rows[k]["Amount"]);

                                }
                                ws1.Cell(ExpsecSheetHeadRow + 1, 2).Value = "Total";
                                ws1.Cell(ExpsecSheetHeadRow + 1, 3).Value = Amt;
                            }
                            else
                            {

                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    ws1.Cell(ExpsecSheetAmountRow, ExpsecSheetAmountCol).Value = dt.Rows[k]["Amount"].ToString();
                                    ExpsecSheetAmountRow++;
                                    Amt += Convert.ToDecimal(dt.Rows[k]["Amount"]);
                                }
                                ws1.Cell(ExpsecSheetAmountRow + 1, ExpsecSheetAmountCol).Value = Amt;
                            }


                            if (i != 0)
                                ExpsecSheetAmountCol++;

                        }


                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=ICSROH.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public FileStreamResult GetDaywiseTapal(DateTime Date)
        //{
        //    try
        //    {

        //        ListDatabaseObjects db = new ListDatabaseObjects();
        //        DataTable dsTrasaction = db.GetDaywiseTapal(Date);
        //        return coreaccountService.toSpreadSheet(dsTrasaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;Ft
        //    }
        //}
        DataTable FullOuterJoinDataTables(params DataTable[] datatables) // supports as many datatables as you need.
        {
            DataTable result = datatables.First().Clone();

            var commonColumns = result.Columns.OfType<DataColumn>();

            foreach (var dt in datatables.Skip(1))
            {
                commonColumns = commonColumns.Intersect(dt.Columns.OfType<DataColumn>(), new DataColumnComparer());
            }

            result.PrimaryKey = commonColumns.ToArray();

            foreach (var dt in datatables)
            {
                result.Merge(dt, false, MissingSchemaAction.AddWithKey);
            }

            return result;
        }

        /* also create this class */
        public class DataColumnComparer : IEqualityComparer<DataColumn>
        {
            public bool Equals(DataColumn x, DataColumn y) { return x.Caption == y.Caption; }

            public int GetHashCode(DataColumn obj) { return obj.Caption.GetHashCode(); }

        }
        public FileStreamResult GetDaywiseTapal(DateTime Date)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        var finQry = context.tblFinYear.Where(m => m.StartDate >= Date || Date <= m.EndDate).FirstOrDefault();

                        ListDatabaseObjects db = new ListDatabaseObjects();

                        DateTime FromDate = Convert.ToDateTime(finQry.StartDate);
                        DateTime ToDate = Convert.ToDateTime(finQry.EndDate);
                        DataSet PayData = db.GetDaywiseTapal(FromDate, ToDate, Date);

                        DataTable curr = PayData.Tables[0];
                        DataTable prev = PayData.Tables[1];
                        var results = FullOuterJoinDataTables(curr, prev);



                        //var results = from table1 in curr.AsEnumerable()
                        //              join table2 in prev.AsEnumerable() on (string)table1["Name"] equals (string)table2["Name"] into g
                        //              from H in g.DefaultIfEmpty()
                        //              select new
                        //              {
                        //                 t1_Name= (string)table1["Name"],
                        //                  t1_TapalReceived = (int)table1["TapalReceived"],
                        //                  t1_Processed = (int)table1["Processed"],
                        //                  t1_Paid = (int)table1["Paid"],
                        //                  t1_Pending = (int)table1["Pending"],

                        //                  t2_Name = H == null ?  "":(string)H["Name"],
                        //                  t2_TapalReceived = H == null ? 0 : (int)H["TapalReceived"],
                        //                  t2_Processed = H == null ? 0 : (int)H["Processed"],
                        //                  t2_Paid = H == null ? 0 : (int)H["Paid"],
                        //                  t2_Pending = H == null ? 0 : (int)H["Pending"],
                        //              };

                        var ws = wb.Worksheets.Add("Tapal");


                        ws.Cell(1, 1).Value = "Date";
                        ws.Cell(1, 2).Value = String.Format("{0:ddd dd-MMM-yyyy}", Date);
                        ws.Range("B1:E1").Row(1).Merge();
                        ws.Cell(1, 6).Value = "Year To Date (YTD) Apr to Mar";
                        ws.Range("F1:I1").Row(1).Merge();
                        ws.Range("A1:I2").Style.Font.Bold = true;



                        ws.Cell(2, 1).Value = "Name";
                        ws.Cell(2, 2).Value = "Tapal Received";
                        ws.Cell(2, 3).Value = "Processed";
                        ws.Cell(2, 4).Value = "Paid";
                        ws.Cell(2, 5).Value = "Pending";


                        ws.Cell(2, 6).Value = "Tapal Received";
                        ws.Cell(2, 7).Value = "Processed";
                        ws.Cell(2, 8).Value = "Paid";
                        ws.Cell(2, 9).Value = "Pending";
                        int listrow = 3;
                        int t1_TapalReceived = 0; int t1_Processed = 0;
                        int t1_Paid = 0; int t1_Pending = 0;
                        int t2_TapalReceived = 0; int t2_Processed = 0;
                        int t2_Paid = 0; int t2_Pending = 0;
                        foreach (DataRow row in results.Rows)
                        {
                            ws.Cell(listrow, 1).Value = row["Name"].ToString();
                            ws.Cell(listrow, 2).Value = row["TapalReceived"] == DBNull.Value ? 0 : row["TapalReceived"];
                            ws.Cell(listrow, 3).Value = row["Processed"] == DBNull.Value ? 0 : row["Processed"];
                            ws.Cell(listrow, 4).Value = row["Paid"] == DBNull.Value ? 0 : row["Paid"];
                            ws.Cell(listrow, 5).Value = row["Pending"] == DBNull.Value ? 0 : row["Pending"];


                            ws.Cell(listrow, 6).Value = row["T2TapalReceived"] == DBNull.Value ? 0 : row["T2TapalReceived"];
                            ws.Cell(listrow, 7).Value = row["T2Processed"] == DBNull.Value ? 0 : row["T2Processed"];
                            ws.Cell(listrow, 8).Value = row["T2Paid"] == DBNull.Value ? 0 : row["T2Paid"];
                            ws.Cell(listrow, 9).Value = row["T2Pending"] == DBNull.Value ? 0 : row["T2Pending"];


                            t1_TapalReceived += (Convert.ToInt32(row["TapalReceived"] == DBNull.Value ? 0 : row["TapalReceived"]));
                            t1_Processed += (Convert.ToInt32(row["Processed"] == DBNull.Value ? 0 : row["Processed"]));
                            t1_Paid += (Convert.ToInt32(row["Paid"] == DBNull.Value ? 0 : row["Paid"]));
                            t1_Pending += (Convert.ToInt32(row["Pending"] == DBNull.Value ? 0 : row["Pending"]));


                            t2_TapalReceived += (Convert.ToInt32(row["T2TapalReceived"] == DBNull.Value ? 0 : row["T2TapalReceived"]));
                            t2_Processed += (Convert.ToInt32(row["T2Processed"] == DBNull.Value ? 0 : row["T2Processed"]));
                            t2_Paid += (Convert.ToInt32(row["T2Paid"] == DBNull.Value ? 0 : row["T2Paid"]));
                            t2_Pending += (Convert.ToInt32(row["T2Pending"] == DBNull.Value ? 0 : row["T2Pending"]));
                            listrow++;
                        }
                        ws.Cell(listrow, 1).Value = "Total";
                        ws.Cell(listrow, 2).Value = t1_TapalReceived;
                        ws.Cell(listrow, 3).Value = t1_Processed;
                        ws.Cell(listrow, 4).Value = t1_Paid;
                        ws.Cell(listrow, 5).Value = t1_Pending;

                        ws.Cell(listrow, 6).Value = t2_TapalReceived;
                        ws.Cell(listrow, 7).Value = t2_Processed;
                        ws.Cell(listrow, 8).Value = t2_Paid;
                        ws.Cell(listrow, 9).Value = t2_Pending;

                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        string fileType = Common.GetMimeType("xls");
                        Response.AddHeader("Content-Disposition", "filename=Day wise Tapal.xls");
                        return File(workStream, fileType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
