using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;

namespace IOAS.GenericServices
{
    public class RequirementMasterService
    {
        #region Designation

        public static int CreateDesignation(DesignationModel model)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.DesignationId == null)
                        {
                            var chkdesignationcode = context.tblRCTDesignation.Where(x => x.DesignationCode == model.DesignationCode.ToUpper() && x.TypeOfAppointment == model.TypeOfAppointment).FirstOrDefault();
                            var chkdesignationname = context.tblRCTDesignation.Where(x => x.Designation == model.Designation.ToUpper() && x.TypeOfAppointment == model.TypeOfAppointment).FirstOrDefault();
                            if (chkdesignationcode != null)
                            {
                                return 3;
                            }
                            if (chkdesignationname != null)
                            {
                                return 4;
                            }
                            tblRCTDesignation adddesignation = new tblRCTDesignation();
                            adddesignation.TypeOfAppointment = model.TypeOfAppointment;
                            adddesignation.DesignationCode = model.DesignationCode.ToUpper();
                            adddesignation.Designation = model.Designation.ToUpper();
                            if (model.TypeOfAppointment == 4)
                                adddesignation.Department = model.DepartmentId;
                            adddesignation.PayStructureMinMum = model.PayStructureMinMum;
                            adddesignation.PayStructureMaximum = model.PayStructureMaximum;
                            adddesignation.CrtdUser = model.UserId;
                            adddesignation.CrtdTs = DateTime.Now;
                            if (model.IsNotValidDesignation == "Yes")
                                adddesignation.IsNotValid = true;
                            else
                                adddesignation.IsNotValid = false;
                            if (model.HRA == "Yes")
                            {
                                adddesignation.HRA = true;
                                adddesignation.HRABasic = model.HRABasic;
                            }
                            else
                            {
                                adddesignation.HRA = false;
                                adddesignation.HRABasic = null;
                            }
                            if (model.Medical == "Yes")
                            {
                                adddesignation.Medical = true;
                                adddesignation.MedicalDeduction = model.MedicalDeduction;
                            }
                            else
                            {
                                adddesignation.Medical = false;
                                adddesignation.MedicalDeduction = null;
                            }
                            if (model.GateScore == "Yes")
                                adddesignation.GateScore = true;
                            else
                                adddesignation.GateScore = false;
                            if (model.IsConsulandFellowship == "CONSPay")
                                adddesignation.ConsolidatedPay = true;
                            if (model.IsConsulandFellowship == "FellowPay")
                                adddesignation.FellowshipPay = true;
                            adddesignation.AgeLimit = model.AgeLimit;
                            adddesignation.AnnualIncrement = model.AnnualIncrement;
                            adddesignation.Status = 1;
                            adddesignation.RecordStatus = "Active";
                            if (model.TypeOfAppointment == 3)
                            {
                                if (model.IsSCST == "Yes")
                                {
                                    adddesignation.IsSCST = true;
                                    adddesignation.SCSTAgeLimit = model.SCSTAgeLimit;
                                }
                                else
                                    adddesignation.IsSCST = false;
                            }
                            context.tblRCTDesignation.Add(adddesignation);
                            context.SaveChanges();
                            int designationId = adddesignation.DesignationId;
                            if (model.Detail != null)
                            {
                                foreach (var item in model.Detail)
                                {
                                    if (item.Qualification != null)
                                    {
                                        tblRCTDesignationDetail adddetail = new tblRCTDesignationDetail();
                                        adddetail.DesignationId = designationId;
                                        adddetail.Qualification = item.Qualification;
                                        adddetail.Marks = item.Marks;
                                        adddetail.CGPA = item.CGPA;
                                        adddetail.RelevantExperience = item.RelevantExperience;
                                        adddetail.IsCurrentVersion = true;
                                        adddetail.CrtdTs = DateTime.Now;
                                        adddetail.CrtdUser = model.UserId;
                                        //if (item.QualificationCourse.Contains(0))
                                        //    adddetail.IsSelectAll = true;
                                        context.tblRCTDesignationDetail.Add(adddetail);
                                        context.SaveChanges();
                                        int designationdetailid = adddetail.DesignationDetailId;
                                        if (item.QualificationCourse[0] != null)
                                        {
                                            for (int i = 0; i < item.QualificationCourse.Length; i++)
                                            {


                                                tblRCTQualificationDetail addqualification = new tblRCTQualificationDetail();
                                                addqualification.Designationid = designationId;
                                                addqualification.DesignationDetailId = designationdetailid;
                                                addqualification.QualificationId = item.Qualification;
                                                addqualification.CourseId = item.QualificationCourse[i];
                                                addqualification.CrtdTs = DateTime.Now;
                                                addqualification.CrtdUser = model.UserId;
                                                context.tblRCTQualificationDetail.Add(addqualification);
                                                context.SaveChanges();

                                            }
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            return 1;
                        }
                        else
                        {
                            var updatedesignation = context.tblRCTDesignation.Where(x => x.DesignationId == model.DesignationId).FirstOrDefault();
                            if (updatedesignation != null)
                            {
                                if (model.Status == 1)
                                {
                                    tblRCTDesignationLog adddesignationlog = new tblRCTDesignationLog();
                                    adddesignationlog.DesignationId = updatedesignation.DesignationId;
                                    adddesignationlog.TypeOfAppointment = updatedesignation.TypeOfAppointment;
                                    adddesignationlog.DesignationCode = updatedesignation.DesignationCode;
                                    adddesignationlog.Department = updatedesignation.Department;
                                    adddesignationlog.PayStructureMinMum = updatedesignation.PayStructureMinMum;
                                    adddesignationlog.PayStructureMaxiMum = updatedesignation.PayStructureMaximum;
                                    adddesignationlog.HRA = updatedesignation.HRA;
                                    adddesignationlog.HRABasic = updatedesignation.HRABasic;
                                    adddesignationlog.Medical = updatedesignation.Medical;
                                    adddesignationlog.MedicalDeducation = updatedesignation.MedicalDeduction;
                                    adddesignationlog.AgeLimit = updatedesignation.AgeLimit;
                                    adddesignationlog.AnnualIncrement = updatedesignation.AnnualIncrement;
                                    adddesignationlog.Status = updatedesignation.Status;
                                    adddesignationlog.CrtdTs = DateTime.Now;
                                    adddesignationlog.CrtdUser = model.UserId;
                                    adddesignationlog.GateScore = updatedesignation.GateScore;
                                    adddesignationlog.FellowshipPay = updatedesignation.FellowshipPay;
                                    adddesignationlog.ConsolidatedPay = updatedesignation.ConsolidatedPay;
                                    adddesignationlog.IsNotValid = updatedesignation.IsNotValid;
                                    adddesignationlog.IsSCST = updatedesignation.IsSCST;
                                    adddesignationlog.SCSTAgeLimit = updatedesignation.SCSTAgeLimit;
                                    context.tblRCTDesignationLog.Add(adddesignationlog);
                                    context.SaveChanges();

                                    if (model.TypeOfAppointment == 4)
                                        updatedesignation.Department = model.DepartmentId;
                                    updatedesignation.PayStructureMinMum = model.PayStructureMinMum;
                                    updatedesignation.PayStructureMaximum = model.PayStructureMaximum;
                                    updatedesignation.UptdTs = DateTime.Now;
                                    updatedesignation.UptdUser = model.UserId;
                                    if (model.IsNotValidDesignation == "Yes")
                                        updatedesignation.IsNotValid = true;
                                    else
                                        updatedesignation.IsNotValid = false;
                                    if (model.HRA == "Yes")
                                    {
                                        updatedesignation.HRA = true;
                                        updatedesignation.HRABasic = model.HRABasic;
                                    }
                                    else
                                    {
                                        updatedesignation.HRA = false;
                                        updatedesignation.HRABasic = null;
                                    }
                                    if (model.Medical == "Yes")
                                    {
                                        updatedesignation.Medical = true;
                                        updatedesignation.MedicalDeduction = model.MedicalDeduction;
                                    }
                                    else
                                    {
                                        updatedesignation.Medical = false;
                                        updatedesignation.MedicalDeduction = null;
                                    }
                                    if (model.GateScore == "Yes")
                                        updatedesignation.GateScore = true;
                                    else
                                        updatedesignation.GateScore = false;
                                    updatedesignation.ConsolidatedPay = false;
                                    updatedesignation.FellowshipPay = false;
                                    if (model.IsConsulandFellowship == "CONSPay")
                                        updatedesignation.ConsolidatedPay = true;
                                    if (model.IsConsulandFellowship == "FellowPay")
                                        updatedesignation.FellowshipPay = true;
                                    updatedesignation.AgeLimit = model.AgeLimit;
                                    updatedesignation.AnnualIncrement = model.AnnualIncrement;
                                    updatedesignation.Status = model.Status;
                                    if (model.TypeOfAppointment == 3)
                                    {
                                        if (model.IsSCST == "Yes")
                                        {
                                            updatedesignation.IsSCST = true;
                                            updatedesignation.SCSTAgeLimit = model.SCSTAgeLimit;
                                        }
                                        else
                                        {
                                            updatedesignation.IsSCST = false;
                                            updatedesignation.SCSTAgeLimit = null;
                                        }
                                    }
                                    context.SaveChanges();
                                    var arrlist = model.Detail.Select(x => x.DesignationDetailId ?? 0).ToArray();

                                    context.tblRCTDesignationDetail.Where(x => x.DesignationId == model.DesignationId && !arrlist.Contains(x.DesignationDetailId) && x.IsCurrentVersion != false)
                                    .ToList()
                                    .ForEach(m =>
                                    {
                                        m.IsCurrentVersion = false;
                                        m.UptdTs = DateTime.Now;
                                        m.UptdUser = model.UserId;
                                    });
                                    //context.tblRCTQualificationDetail.Where(x => x.Designationid == model.DesignationId && !arrlist.Contains(x.DesignationDetailId ?? 0) && x.IsCurrentVersion != false);
                                    foreach (var item in model.Detail)
                                    {
                                        var desquery = context.tblRCTDesignationDetail.FirstOrDefault(m => m.DesignationDetailId == item.DesignationDetailId);
                                        if (desquery == null)
                                        {
                                            tblRCTDesignationDetail adddes = new tblRCTDesignationDetail();
                                            adddes.DesignationId = model.DesignationId;
                                            adddes.Qualification = item.Qualification;
                                            adddes.Marks = item.Marks;
                                            adddes.CGPA = item.CGPA;
                                            adddes.RelevantExperience = item.RelevantExperience;
                                            adddes.IsCurrentVersion = true;
                                            adddes.CrtdTs = DateTime.Now;
                                            adddes.CrtdUser = model.UserId;
                                            //if (item.QualificationCourse.Contains(0))
                                            //    adddes.IsSelectAll = true;
                                            context.tblRCTDesignationDetail.Add(adddes);
                                            context.SaveChanges();
                                            int designationdetailid = adddes.DesignationDetailId;
                                            if (item.QualificationCourse != null)
                                            {
                                                for (int i = 0; i < item.QualificationCourse.Length; i++)
                                                {

                                                    tblRCTQualificationDetail addqualification = new tblRCTQualificationDetail();
                                                    addqualification.Designationid = model.DesignationId;
                                                    addqualification.DesignationDetailId = designationdetailid;
                                                    addqualification.QualificationId = item.Qualification;
                                                    addqualification.CourseId = item.QualificationCourse[i];
                                                    addqualification.CrtdTs = DateTime.Now;
                                                    addqualification.CrtdUser = model.UserId;
                                                    context.tblRCTQualificationDetail.Add(addqualification);
                                                    context.SaveChanges();

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item.DesignationDetailId != null)
                                            {
                                                desquery.DesignationId = model.DesignationId;
                                                desquery.Qualification = item.Qualification;
                                                desquery.Marks = item.Marks;
                                                desquery.CGPA = item.CGPA;
                                                desquery.RelevantExperience = item.RelevantExperience;
                                                desquery.UptdTs = DateTime.Now;
                                                desquery.UptdUser = model.UserId;
                                                //if (item.QualificationCourse.Contains(0))
                                                //    desquery.IsSelectAll = true;
                                                context.SaveChanges();
                                                int ddlids = item.DesignationDetailId ?? 0;
                                                var query = (from dd in context.tblRCTQualificationDetail
                                                             where (dd.Designationid == model.DesignationId && dd.DesignationDetailId == ddlids)
                                                             select dd).ToList();
                                                if (query.Count > 0)
                                                {
                                                    context.tblRCTQualificationDetail.RemoveRange(query);
                                                    context.SaveChanges();

                                                }
                                                if (item.QualificationCourse != null)
                                                {
                                                    for (int i = 0; i < item.QualificationCourse.Length; i++)
                                                    {

                                                        tblRCTQualificationDetail addqualification = new tblRCTQualificationDetail();
                                                        addqualification.Designationid = model.DesignationId;
                                                        addqualification.DesignationDetailId = item.DesignationDetailId;
                                                        addqualification.QualificationId = item.Qualification;
                                                        addqualification.CourseId = item.QualificationCourse[i];
                                                        addqualification.CrtdTs = DateTime.Now;
                                                        addqualification.CrtdUser = model.UserId;
                                                        context.tblRCTQualificationDetail.Add(addqualification);
                                                        context.SaveChanges();

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    updatedesignation.Status = model.Status;
                                    context.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            return 2;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.ErrorMsg = ex.Message;
                        return -1;
                    }
                }
            }
        }

        public static DesignationModel EditDesignation(int desingnationId)
        {
            DesignationModel editmodel = new DesignationModel();
            List<DesignationDetailModel> list = new List<DesignationDetailModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from ds in context.tblRCTDesignation
                                 where ds.DesignationId == desingnationId
                                 select ds).FirstOrDefault();
                    var querydetail = (from dds in context.tblRCTDesignationDetail
                                       where dds.DesignationId == desingnationId && dds.IsCurrentVersion == true
                                       select dds).ToList();
                    if (query != null)
                    {
                        editmodel.DesignationId = query.DesignationId;
                        editmodel.TypeOfAppointment = query.TypeOfAppointment;
                        editmodel.TypeofAccountName = Common.GetCodeControlnameCommon(query.TypeOfAppointment ?? 0, "Appointmenttype");
                        editmodel.DesignationCode = query.DesignationCode;
                        editmodel.Designation = query.Designation;
                        editmodel.DepartmentId = query.Department;
                        editmodel.PayStructureMinMum = query.PayStructureMinMum;
                        editmodel.PayStructureMaximum = query.PayStructureMaximum;
                        if (query.IsNotValid != true)
                            editmodel.IsNotValidDesignation = "No";
                        else
                            editmodel.IsNotValidDesignation = "Yes";
                        if (query.HRA != true)
                            editmodel.HRA = "No";
                        else
                            editmodel.HRA = "Yes";
                        editmodel.HRABasic = query.HRABasic;
                        if (query.Medical != true)
                            editmodel.Medical = "No";
                        else
                            editmodel.Medical = "Yes";
                        if (query.IsSCST != true)
                            editmodel.IsSCST = "No";
                        else
                            editmodel.IsSCST = "Yes";
                        editmodel.SCSTAgeLimit = query.SCSTAgeLimit;
                        if (query.GateScore != true)
                            editmodel.GateScore = "No";
                        else
                            editmodel.GateScore = "Yes";
                        if (query.ConsolidatedPay == true)
                            editmodel.IsConsulandFellowship = "CONSPay";
                        if (query.FellowshipPay == true)
                            editmodel.IsConsulandFellowship = "FellowPay";

                        editmodel.MedicalDeduction = query.MedicalDeduction;
                        editmodel.AgeLimit = query.AgeLimit;
                        editmodel.AnnualIncrement = query.AnnualIncrement;
                        editmodel.Status = query.Status;
                        if (querydetail.Count > 0)
                        {
                            for (int i = 0; i < querydetail.Count; i++)
                            {
                                int ddid = querydetail[i].DesignationDetailId;
                                int qulid = querydetail[i].Qualification ?? 0;

                                var courselist = (from co in context.tblRCTQualificationDetail
                                                  where co.DesignationDetailId == ddid && co.QualificationId == qulid
                                                  select co.CourseId).ToArray();
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new DesignationDetailModel()
                                {
                                    DesignationDetailId = querydetail[i].DesignationDetailId,
                                    Marks = querydetail[i].Marks,
                                    CGPA = querydetail[i].CGPA,
                                    Qualification = querydetail[i].Qualification,
                                    QualificationCourse = courselist,
                                    //courseselected= courselist,
                                    ddlList = datalist,
                                    RelevantExperience = querydetail[i].RelevantExperience
                                });
                            }
                        }
                    }
                    editmodel.Detail = list;

                }
                return editmodel;
            }
            catch (Exception ex)
            {
                return editmodel;
            }
        }
        public static SearchdesignationModel GetDesignationList(SearchdesignationModel model, int page, int pageSize)
        {
            SearchdesignationModel searchdata = new SearchdesignationModel();
            try
            {
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;

                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    List<DesignationModel> list = new List<DesignationModel>();
                    var query = (from ds in context.tblRCTDesignation
                                 join cc in context.tblCodeControl on ds.TypeOfAppointment equals cc.CodeValAbbr
                                 orderby ds.DesignationId descending
                                 where (cc.CodeName == "Appointmenttype" && ds.RecordStatus != "InActive")
                                       && (cc.CodeValDetail.Contains(model.TypeofAccountName) || model.TypeofAccountName == null)
                                       && (ds.DesignationCode.Contains(model.DesignationCode) || model.DesignationCode == null)
                                       && (ds.Designation.Contains(model.Designation) || model.Designation == null)
                                       && (ds.PayStructureMinMum == model.PayStructureMinMum || model.PayStructureMinMum == null)
                                       && (ds.PayStructureMaximum == model.PayStructureMaximum || model.PayStructureMaximum == null)
                                       && (ds.HRA == model.HRA || model.HRA == null)
                                       && (ds.Medical == model.Medical || model.Medical == null)
                                       && (ds.Status == model.Status || model.Status == null)
                                 select new
                                 {
                                     cc.CodeValDetail,
                                     ds.DesignationCode,
                                     ds.Designation,
                                     ds.PayStructureMinMum,
                                     ds.PayStructureMaximum,
                                     ds.HRA,
                                     ds.Medical,
                                     ds.Status,
                                     ds.DesignationId,
                                     ds.RecordStatus
                                 }
                                      ).Skip(skiprec).Take(pageSize).ToList();
                    searchdata.TotalRecords = (from ds in context.tblRCTDesignation
                                               join cc in context.tblCodeControl on ds.TypeOfAppointment equals cc.CodeValAbbr
                                               orderby ds.DesignationId descending
                                               where (cc.CodeName == "Appointmenttype" && ds.RecordStatus != "InActive")
                                                && (cc.CodeValDetail.Contains(model.TypeofAccountName) || model.TypeofAccountName == null)
                                                     && (ds.DesignationCode.Contains(model.DesignationCode) || model.DesignationCode == null)
                                                     && (ds.Designation.Contains(model.Designation) || model.Designation == null)
                                                     && (ds.PayStructureMinMum == model.PayStructureMinMum || model.PayStructureMinMum == null)
                                                     && (ds.PayStructureMaximum == model.PayStructureMaximum || model.PayStructureMaximum == null)
                                                     && (ds.HRA == model.HRA || model.HRA == null)
                                                     && (ds.Medical == model.Medical || model.Medical == null)
                                                     && (ds.Status == model.Status || model.Status == null)
                                               select new
                                               {
                                                   cc.CodeValDetail,
                                                   ds.DesignationCode,
                                                   ds.Designation,
                                                   ds.PayStructureMinMum,
                                                   ds.PayStructureMaximum,
                                                   ds.HRA,
                                                   ds.Medical,
                                                   ds.Status,
                                                   ds.DesignationId,
                                                   ds.RecordStatus
                                               }
                                      ).Count();

                    if (query.Count > 0)
                    {
                        int sno = 0;
                        if (page == 1)
                        {
                            sno = 1;
                        }
                        else
                        {
                            sno = ((page - 1) * pageSize) + 1;
                        }

                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new DesignationModel()
                            {
                                SNo = sno + i,
                                DesignationId = query[i].DesignationId,
                                TypeofAccountName = query[i].CodeValDetail,
                                DesignationCode = query[i].DesignationCode,
                                Designation = query[i].Designation,
                                PayStructureMinMum = query[i].PayStructureMinMum,
                                PayStructureMaximum = query[i].PayStructureMaximum,
                                HRAType = query[i].HRA == true ? "Yes" : "No",
                                MedicalType = query[i].Medical == true ? "Yes" : "No",
                                Status = query[i].Status,
                                RecordStatus = query[i].RecordStatus
                            });
                        }
                    }
                    searchdata.Designationlist = list;
                }
                return searchdata;
            }
            catch (Exception ex)
            {
                return searchdata;
            }
        }

        #endregion

        #region Statutory
        public static int CreateStatutory(StatutoryModel model)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.StatutoryId == null)
                        {
                            bool IsvalidePeriod = false;
                            var chkprvvaludate = (from sck in context.tblRCTStatutory
                                                  where sck.Status != "InActive"
                                                  select sck
                                                ).ToList();
                            if (chkprvvaludate.Count > 0)
                            {
                                for (int i = 0; i < chkprvvaludate.Count; i++)
                                {
                                    if (model.ValueDate <= chkprvvaludate[i].EndDate)
                                    {
                                        IsvalidePeriod = true;
                                        break;
                                    }
                                    if (model.ValueDate.Value.AddDays(-1) <= chkprvvaludate[i].EndDate)
                                    {
                                        IsvalidePeriod = true;
                                        break;
                                    }
                                }
                            }
                            if (IsvalidePeriod == true)
                                return 3;
                            var updateclosedate = context.tblRCTStatutory.Where(x => x.IsCurrentVersion == true && x.Status != "InActive").FirstOrDefault();
                            if (updateclosedate != null)
                            {
                                updateclosedate.EndDate = model.ValueDate.Value.AddDays(-1);
                                updateclosedate.IsCurrentVersion = false;
                                updateclosedate.UptdTs = DateTime.Now;
                                updateclosedate.UptdUser = model.UserId;
                                context.SaveChanges();
                            }
                            tblRCTStatutory addStatutory = new tblRCTStatutory();
                            addStatutory.PFEmployeePercentage = model.PFEmployeePercentage;
                            addStatutory.PFEmployerPercentage = model.PFEmployerPercentage;
                            addStatutory.ESICEmployeePercentage = model.ESICEmployeePercentage;
                            addStatutory.ESICEmployerPercentage = model.ESICEmployerPercentage;
                            addStatutory.LWFEmployeeContribution = model.LWFEmployeeContribution;
                            addStatutory.LWFEmployerContribution = model.LWFEmployerContribution;
                            addStatutory.ValueDate = model.ValueDate;
                            addStatutory.IsCurrentVersion = true;
                            addStatutory.CrtdTs = DateTime.Now;
                            addStatutory.CrtdUser = model.UserId;
                            addStatutory.Status = "Active";
                            addStatutory.PFEmployeeAmount = model.PFEmployeeAmount;
                            addStatutory.ESICEmployeePhysicalAmount = model.ESICPhysicalAmount;
                            addStatutory.ESICEmployeegeneralamount = model.ESICGeneralAmount;
                            context.tblRCTStatutory.Add(addStatutory);
                            context.SaveChanges();
                            transaction.Commit();

                        }
                        return 1;
                        //else
                        //{
                        //    var updatequery = context.tblRCTStatutory.Where(x => x.StatutoryId == model.StatutoryId).FirstOrDefault();
                        //    if(updatequery!=null)
                        //    {
                        //        tblRCTStatutoryLog addStatutoryLog = new tblRCTStatutoryLog();
                        //        addStatutoryLog.StatutoryId = updatequery.StatutoryId;
                        //        addStatutoryLog.PFEmployeePercentage = updatequery.PFEmployeePercentage;
                        //        addStatutoryLog.PFEmployerPercentage = updatequery.PFEmployerPercentage;
                        //        addStatutoryLog.ESICEmployeePercentage = updatequery.ESICEmployeePercentage;
                        //        addStatutoryLog.ESICEmployerPercentage = updatequery.ESICEmployerPercentage;
                        //        addStatutoryLog.LWFEmployeeContribution = updatequery.LWFEmployeeContribution;
                        //        addStatutoryLog.LWFEmployerContribution = updatequery.LWFEmployerContribution;
                        //        addStatutoryLog.ValueDate = updatequery.ValueDate;
                        //        addStatutoryLog.CrtdTs = DateTime.Now;
                        //        addStatutoryLog.CrtdUser = model.UserId;
                        //        context.tblRCTStatutoryLog.Add(addStatutoryLog);
                        //        context.SaveChanges();
                        //        updatequery.PFEmployeePercentage = model.PFEmployeePercentage;
                        //        updatequery.PFEmployerPercentage = model.PFEmployerPercentage;
                        //        updatequery.ESICEmployeePercentage = model.ESICEmployeePercentage;
                        //        updatequery.ESICEmployerPercentage = model.ESICEmployerPercentage;
                        //        updatequery.LWFEmployeeContribution = model.LWFEmployeeContribution;
                        //        updatequery.LWFEmployerContribution = model.LWFEmployerContribution;
                        //        updatequery.ValueDate = model.ValueDate;
                        //        updatequery.UptdTs = DateTime.Now;
                        //        updatequery.UptdUser = model.UserId;
                        //        context.SaveChanges();
                        //    }
                        //    transaction.Commit();
                        //    return 2;
                        //}

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }

        public static StatutoryModel ViewStatutory(int statutoryId)
        {
            StatutoryModel Viewmodel = new StatutoryModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblRCTStatutory.Where(x => x.StatutoryId == statutoryId).FirstOrDefault();
                    if (query != null)
                    {
                        Viewmodel.StatutoryId = query.StatutoryId;
                        Viewmodel.PFEmployeePercentage = query.PFEmployeePercentage;
                        Viewmodel.PFEmployerPercentage = query.PFEmployerPercentage;
                        Viewmodel.ESICEmployeePercentage = query.ESICEmployeePercentage;
                        Viewmodel.ESICEmployerPercentage = query.ESICEmployerPercentage;
                        Viewmodel.LWFEmployeeContribution = query.LWFEmployeeContribution;
                        Viewmodel.LWFEmployerContribution = query.LWFEmployerContribution;
                        Viewmodel.PFEmployeeAmount = query.PFEmployeeAmount;
                        Viewmodel.ESICPhysicalAmount = query.ESICEmployeePhysicalAmount;
                        Viewmodel.ESICGeneralAmount = query.ESICEmployeegeneralamount;
                        if (query.ValueDate != null)
                            Viewmodel.ValueDateType = string.Format("{0:dd}", (DateTime)query.ValueDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.ValueDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.ValueDate);
                        if (query.EndDate != null)
                            Viewmodel.EndDateType = string.Format("{0:dd}", (DateTime)query.EndDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.EndDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.EndDate);
                    }
                }
                return Viewmodel;
            }
            catch (Exception ex)
            {
                return Viewmodel;
            }
        }

        public static SearchStatutoryModel GetStatutoryList(SearchStatutoryModel model, int page, int pageSize, DateFilterModel ValueDateType, DateFilterModel EndDateType)
        {
            SearchStatutoryModel searchstatu = new SearchStatutoryModel();
            List<StatutoryModel> list = new List<StatutoryModel>();
            try
            {
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                if (ValueDateType.to != null)
                    ValueDateType.to = ValueDateType.to.Value.Date.AddDays(1).AddTicks(-2);
                if (EndDateType.to != null)
                    EndDateType.to = EndDateType.to.Value.Date.AddDays(1).AddTicks(-2);
                using (var context = new IOASDBEntities())
                {
                    var query = (from ss in context.tblRCTStatutory
                                 orderby ss.StatutoryId descending
                                 where ss.Status != "InActive"
                                 && ((ss.ValueDate >= ValueDateType.@from && ss.ValueDate <= ValueDateType.to) || (ValueDateType.@from == null && ValueDateType.to == null))
                                 && ((ss.EndDate >= EndDateType.@from && ss.EndDate <= EndDateType.to) || (EndDateType.@from == null && EndDateType.to == null))
                                 select new { ss.StatutoryId, ss.ValueDate, ss.EndDate, ss.Status, ss.IsCurrentVersion }).Skip(skiprec).Take(pageSize).ToList();
                    searchstatu.TotalRecords = (from ss in context.tblRCTStatutory
                                                orderby ss.StatutoryId descending
                                                where ss.Status != "InActive"
                                                  && ((ss.ValueDate >= ValueDateType.@from && ss.ValueDate <= ValueDateType.to) || (ValueDateType.@from == null && ValueDateType.to == null))
                               && ((ss.EndDate >= EndDateType.@from && ss.EndDate <= EndDateType.to) || (EndDateType.@from == null && EndDateType.to == null))
                                                select new { ss.StatutoryId, ss.ValueDate, ss.EndDate, ss.Status }).Count();
                    if (query.Count > 0)
                    {
                        int sno = 0;
                        if (page == 1)
                        {
                            sno = 1;
                        }
                        else
                        {
                            sno = ((page - 1) * pageSize) + 1;
                        }
                        for (int i = 0; i < query.Count; i++)
                        {
                            string enddate = string.Empty;
                            if (query[i].EndDate != null)
                            {
                                enddate = String.Format("{0:s}", query[i].EndDate);
                            }

                            list.Add(new StatutoryModel()
                            {
                                SNo = sno + i,
                                StatutoryId = query[i].StatutoryId,
                                ValueDateType = String.Format("{0:s}", query[i].ValueDate),
                                EndDateType = enddate,
                                Status = query[i].Status,
                                CurrDate = String.Format("{0:s}", DateTime.Now),
                                FromDate = query[i].ValueDate,
                                CurrentDate = DateTime.Now,
                                IsActive = query[i].IsCurrentVersion ?? false,
                            });
                        }
                    }

                }
                searchstatu.StatutoryList = list;
                return searchstatu;
            }
            catch (Exception ex)
            {
                return searchstatu;
            }
        }

        #endregion

        #region ProfessionalTax
        public static int CreateProfessionalTax(ProfessionalTaxModel model)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.ProfessionalTaxId == null)
                        {
                            bool IsvalidePeriod = false;
                            var chkprvvaludate = (from Pck in context.tblRCTProfessionalTax
                                                  where Pck.Status != "InActive"
                                                  select Pck
                                                ).ToList();
                            if (chkprvvaludate.Count > 0)
                            {
                                for (int i = 0; i < chkprvvaludate.Count; i++)
                                {
                                    if (model.ValueDate <= chkprvvaludate[i].EndDate)
                                    {
                                        IsvalidePeriod = true;
                                        break;
                                    }
                                    if (model.ValueDate.Value.AddDays(-1) <= chkprvvaludate[i].EndDate)
                                    {
                                        IsvalidePeriod = true;
                                        break;
                                    }
                                }
                            }
                            if (IsvalidePeriod == true)
                                return 3;
                            var updateclsdate = context.tblRCTProfessionalTax.Where(x => x.IsCurrentVersion == true && x.Status != "InActive").FirstOrDefault();
                            if (updateclsdate != null)
                            {
                                updateclsdate.EndDate = model.ValueDate.Value.AddDays(-1);
                                updateclsdate.IsCurrentVersion = false;
                                updateclsdate.UptdTs = DateTime.Now;
                                updateclsdate.UptdUser = model.UserId;
                                context.SaveChanges();
                            }
                            tblRCTProfessionalTax addproftax = new tblRCTProfessionalTax();
                            addproftax.MonthySalary35 = model.MonthySalary35;
                            addproftax.MonthySalary35to5 = model.MonthySalary35to5;
                            addproftax.MonthySalary5to75 = model.MonthySalary5to75;
                            addproftax.MonthySalary75to10 = model.MonthySalary75to10;
                            addproftax.MonthySalary10to12 = model.MonthySalary10to12;
                            addproftax.MonthySalaryAbove12 = model.MonthySalaryAbove12;
                            addproftax.ValueDate = model.ValueDate;
                            addproftax.IsCurrentVersion = true;
                            addproftax.CrtdTs = DateTime.Now;
                            addproftax.CrtdUser = model.UserId;
                            addproftax.Status = "Active";
                            context.tblRCTProfessionalTax.Add(addproftax);
                            context.SaveChanges();
                            transaction.Commit();
                            return 1;
                        }
                        return 1;
                        //else
                        //{
                        //    var updateproftax = context.tblRCTProfessionalTax.Where(x => x.ProfessionalTaxId == model.ProfessionalTaxId).FirstOrDefault();
                        //    if(updateproftax!=null)
                        //    {
                        //        tblRCTProfessionalTaxLog addproftaxlog = new tblRCTProfessionalTaxLog();
                        //        addproftaxlog.ProfessionalTaxId = updateproftax.ProfessionalTaxId;
                        //        addproftaxlog.MonthySalary35 = updateproftax.MonthySalary35;
                        //        addproftaxlog.MonthySalary35to5 = updateproftax.MonthySalary35to5;
                        //        addproftaxlog.MonthySalary5to75 = updateproftax.MonthySalary5to75;
                        //        addproftaxlog.MonthySalary75to10 = updateproftax.MonthySalary75to10;
                        //        addproftaxlog.MonthySalary10to12 = model.MonthySalary10to12;
                        //        addproftaxlog.MonthySalaryAbove12 = updateproftax.MonthySalaryAbove12;
                        //        addproftaxlog.ValueDate = updateproftax.ValueDate;
                        //        addproftaxlog.CrtdTs = DateTime.Now;
                        //        addproftaxlog.CrtdUser = model.UserId;
                        //        context.tblRCTProfessionalTaxLog.Add(addproftaxlog);
                        //        context.SaveChanges();
                        //        updateproftax.MonthySalary35 = model.MonthySalary35;
                        //        updateproftax.MonthySalary35to5 = model.MonthySalary35to5;
                        //        updateproftax.MonthySalary5to75 = model.MonthySalary5to75;
                        //        updateproftax.MonthySalary75to10 = model.MonthySalary75to10;
                        //        updateproftax.MonthySalary10to12 = model.MonthySalary10to12;
                        //        updateproftax.MonthySalaryAbove12 = model.MonthySalaryAbove12;
                        //        updateproftax.ValueDate = model.ValueDate;
                        //        updateproftax.UptdTs = DateTime.Now;
                        //        updateproftax.UptdUser = model.UserId;
                        //        context.SaveChanges();
                        //    }
                        //    transaction.Commit();
                        //    return 2;
                        //}
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }
        public static ProfessionalTaxModel ViewProftax(int professionalTaxId)
        {
            ProfessionalTaxModel Viewmodel = new ProfessionalTaxModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblRCTProfessionalTax.Where(x => x.ProfessionalTaxId == professionalTaxId).FirstOrDefault();
                    if (query != null)
                    {
                        Viewmodel.ProfessionalTaxId = query.ProfessionalTaxId;
                        Viewmodel.MonthySalary35 = query.MonthySalary35;
                        Viewmodel.MonthySalary35to5 = query.MonthySalary35to5;
                        Viewmodel.MonthySalary5to75 = query.MonthySalary5to75;
                        Viewmodel.MonthySalary75to10 = query.MonthySalary75to10;
                        Viewmodel.MonthySalary10to12 = query.MonthySalary10to12;
                        Viewmodel.MonthySalaryAbove12 = query.MonthySalaryAbove12;
                        if (query.ValueDate != null)
                            Viewmodel.ValueDateType = string.Format("{0:dd}", (DateTime)query.ValueDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.ValueDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.ValueDate);
                        if (query.EndDate != null)
                            Viewmodel.EndDateType = string.Format("{0:dd}", (DateTime)query.EndDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.EndDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.EndDate);
                    }
                }
                return Viewmodel;
            }
            catch (Exception ex)
            {
                return Viewmodel;
            }
        }
        public static SearchProfessionalTaxModel GetProfessiontaxList(SearchProfessionalTaxModel model, int page, int pageSize, DateFilterModel ValueDateType, DateFilterModel EndDateType)
        {
            SearchProfessionalTaxModel searchpft = new SearchProfessionalTaxModel();
            List<ProfessionalTaxModel> list = new List<ProfessionalTaxModel>();
            try
            {
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                if (ValueDateType.to != null)
                    ValueDateType.to = ValueDateType.to.Value.Date.AddDays(1).AddTicks(-2);
                if (EndDateType.to != null)
                    EndDateType.to = EndDateType.to.Value.Date.AddDays(1).AddTicks(-2);
                using (var context = new IOASDBEntities())
                {
                    var query = (from PT in context.tblRCTProfessionalTax
                                 orderby PT.ProfessionalTaxId descending
                                 where PT.Status != "InActive"
                                 && ((PT.ValueDate >= ValueDateType.@from && PT.ValueDate <= ValueDateType.to) || (ValueDateType.@from == null && ValueDateType.to == null))
                                 && ((PT.EndDate >= EndDateType.@from && PT.EndDate <= EndDateType.to) || (EndDateType.@from == null && EndDateType.to == null))
                                 select new { PT.ProfessionalTaxId, PT.ValueDate, PT.EndDate, PT.Status }).Skip(skiprec).Take(pageSize).ToList();
                    searchpft.TotalRecords = (from PT in context.tblRCTProfessionalTax
                                              orderby PT.ProfessionalTaxId descending
                                              where PT.Status != "InActive"
                                              && ((PT.ValueDate >= ValueDateType.@from && PT.ValueDate <= ValueDateType.to) || (ValueDateType.@from == null && ValueDateType.to == null))
                                              && ((PT.EndDate >= EndDateType.@from && PT.EndDate <= EndDateType.to) || (EndDateType.@from == null && EndDateType.to == null))
                                              select new { PT.ProfessionalTaxId, PT.ValueDate, PT.EndDate, PT.Status }).Count();
                    if (query.Count > 0)
                    {
                        int sno = 0;
                        if (page == 1)
                        {
                            sno = 1;
                        }
                        else
                        {
                            sno = ((page - 1) * pageSize) + 1;
                        }
                        for (int i = 0; i < query.Count; i++)
                        {
                            string enddate = string.Empty;
                            if (query[i].EndDate != null)
                            {
                                enddate = String.Format("{0:s}", query[i].EndDate);
                            }
                            list.Add(new ProfessionalTaxModel()
                            {
                                SNo = sno + i,
                                ProfessionalTaxId = query[i].ProfessionalTaxId,
                                ValueDateType = String.Format("{0:s}", query[i].ValueDate),
                                EndDateType = enddate,
                                Status = query[i].Status
                            });
                        }
                    }

                }
                searchpft.proftaxList = list;
                return searchpft;
            }
            catch (Exception ex)
            {
                return searchpft;
            }
        }
        #endregion

        #region CommiteMember
        public static int IsValidMember(MemberModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.MemberType == 1)
                    {
                        var activeQuery = (from c in context.tblRCTMember
                                           where c.TypeOfMember == 1 && c.Status == "Active"
                                           && model.FromDate <= c.Effectivedate
                                           select c).ToList();
                        if ((activeQuery.Count == 1 || activeQuery.Count == 0) && model.FromDate == DateTime.Now.Date)
                            return 1;

                        var openQuery = (from c in context.tblRCTMember
                                         where c.TypeOfMember == 1 && (c.Status == "Open" || c.Status == "Active")
                                         && (model.MemberId == null || model.MemberId != c.MemberId)
                                         && model.FromDate <= c.Effectivedate
                                         select c).ToList();
                        if ((openQuery.Count == 1 || openQuery.Count == 0) && model.FromDate > DateTime.Now.Date)
                            return 1;

                        return 2;
                    }
                    else
                    {
                        if (context.tblRCTMember.Any(x => x.TypeOfMember == 2 && x.Status != "InActive" && model.FromDate <= x.Effectivedate && (model.MemberId == null || model.MemberId != x.MemberId)))
                            return 3;

                        return 1;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static int CreateMember(MemberModel model)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.MemberId == null)
                        {
                            var res = IsValidMember(model);
                            if (res > 1)
                                return res;
                            if (res == 1)
                            {
                                var dupQuery = (from c in context.tblRCTMember
                                                where c.EmployeeId == model.EmployeeId && c.Status == "Active"
                                                select c).FirstOrDefault();
                                if (dupQuery != null)
                                    return 4;

                                var piQuery = (from pi in context.vwFacultyStaffDetails
                                               where pi.UserId == model.EmployeeId
                                               select pi).FirstOrDefault();
                                if (piQuery != null)
                                {
                                    tblRCTMember addmember = new tblRCTMember();
                                    addmember.TypeOfMember = model.MemberType;
                                    addmember.EmployeeNo = piQuery.EmployeeId;
                                    addmember.EmployeeName = piQuery.FirstName;
                                    addmember.EmployeeDesignation = piQuery.Designation;
                                    addmember.DepartmentCode = piQuery.DepartmentCode;
                                    addmember.DepartmentName = piQuery.DepartmentName;
                                    addmember.Email = piQuery.Email;
                                    addmember.ContactNumber = piQuery.ContactNumber;
                                    addmember.EmployeeId = model.EmployeeId;
                                    addmember.FromDate = model.FromDate;
                                    addmember.ToDate = model.ToDate;
                                    addmember.Effectivedate = model.ToDate;
                                    addmember.Status = "Open";
                                    if (model.FromDate <= DateTime.Now.Date)
                                        addmember.Status = "Active";
                                    addmember.CrtdUser = model.UserId;
                                    addmember.CrtdTS = DateTime.Now;
                                    context.tblRCTMember.Add(addmember);
                                    context.SaveChanges();
                                }
                                transaction.Commit();
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            var uptQuery = context.tblRCTMember.Where(x => x.MemberId == model.MemberId && x.Status != "InActive").FirstOrDefault();
                            if (uptQuery != null)
                            {
                                DateTime curr = DateTime.Now.Date;
                                if (uptQuery.Status == "Open")
                                {
                                    var res = IsValidMember(model);
                                    if (res == 1)
                                    {
                                        uptQuery.FromDate = model.FromDate;
                                        uptQuery.Effectivedate = model.ToDate;
                                        if (model.FromDate == curr)
                                            uptQuery.Status = "Active";
                                    }
                                    else
                                    {
                                        return res;
                                    }
                                }
                                else
                                {
                                    if (model.ToDate <= uptQuery.Effectivedate && model.ToDate < curr)
                                        uptQuery.Status = "InActive";
                                    uptQuery.Effectivedate = model.ToDate;
                                }
                                uptQuery.UpdtTS = DateTime.Now;
                                uptQuery.UpdtUser = model.UserId;
                                context.SaveChanges();
                                transaction.Commit();
                                if (uptQuery.Status == "Active")
                                    return 6;
                                else if (uptQuery.Status == "InActive")
                                    return 5;
                                else
                                    return 8;
                            }

                        }
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }

        public static MemberModel EditMemberModel(int MemberId)
        {
            MemberModel editmember = new MemberModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblRCTMember.Where(x => x.MemberId == MemberId).FirstOrDefault();
                    if (query != null)
                    {
                        editmember.MemberId = query.MemberId;
                        editmember.MemberType = query.TypeOfMember;
                        editmember.EmployeeId = query.EmployeeId;
                        editmember.EmployeeName = query.EmployeeName;
                        editmember.FromDate = query.FromDate;
                        //editmember.ToDate = query.ToDate;
                        editmember.ToDate = query.Effectivedate;
                        editmember.FrmDate = string.Format("{0:dd/MMMM/yyyy}", query.FromDate);
                        //editmember.TostrDate = string.Format("{0:dd/MMMM/yyyy}", query.ToDate);
                        editmember.TostrDate = string.Format("{0:dd/MMMM/yyyy}", query.Effectivedate);
                        editmember.MemberStatus = query.Status;
                    }
                }
                return editmember;
            }
            catch (Exception ex)
            {
                return editmember;
            }
        }

        public static SearchMemberModel GetMemberList(SearchMemberModel model, int page, int pageSize, DateFilterModel FrmDate, DateFilterModel TostrDate)
        {
            SearchMemberModel serchmem = new SearchMemberModel();
            List<MemberModel> list = new List<MemberModel>();
            int skiprec = 0;
            if (page == 1)
            {
                skiprec = 0;
            }
            else
            {
                skiprec = (page - 1) * pageSize;
            }
            if (FrmDate.to != null)
                FrmDate.to = FrmDate.to.Value.Date.AddDays(1).AddTicks(-2);
            if (TostrDate.to != null)
                TostrDate.to = TostrDate.to.Value.Date.AddDays(1).AddTicks(-2);
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from m in context.tblRCTMember
                                 join cc in context.tblCodeControl on m.TypeOfMember equals cc.CodeValAbbr
                                 orderby m.MemberId descending
                                 where cc.CodeName == "MemberType"
                                 && (cc.CodeValDetail.Contains(model.MemberTypeName) || model.MemberTypeName == null)
                                 && (m.EmployeeNo.Contains(model.EmployeeCode) || model.EmployeeCode == null)
                                 && (m.EmployeeName.Contains(model.EmployeeName) || model.EmployeeName == null)
                                 && (m.FromDate >= FrmDate.@from && m.FromDate <= FrmDate.to || (FrmDate.@from == null && FrmDate.to == null))
                                 && (m.ToDate >= TostrDate.@from && m.ToDate <= TostrDate.to || (TostrDate.@from == null && TostrDate.to == null))
                                 && (m.Status.Contains(model.Status) || model.Status == null)
                                 select new { m, cc }).Skip(skiprec).Take(pageSize).ToList();
                    serchmem.TotalRecords = (from m in context.tblRCTMember
                                             join cc in context.tblCodeControl on m.TypeOfMember equals cc.CodeValAbbr
                                             orderby m.MemberId descending
                                             where cc.CodeName == "MemberType"
                                             && (cc.CodeValDetail.Contains(model.MemberTypeName) || model.MemberTypeName == null)
                                             && (m.EmployeeNo.Contains(model.EmployeeCode) || model.EmployeeCode == null)
                                             && (m.EmployeeName.Contains(model.EmployeeName) || model.EmployeeName == null)
                                             && (m.FromDate >= FrmDate.@from && m.FromDate <= FrmDate.to || (FrmDate.@from == null && FrmDate.to == null))
                                             && (m.ToDate >= TostrDate.@from && m.ToDate <= TostrDate.to || (TostrDate.@from == null && TostrDate.to == null))
                                             && (m.Status.Contains(model.Status) || model.Status == null)
                                             select new { m, cc }).Count();
                    if (query.Count > 0)
                    {
                        int sno = 0;
                        if (page == 1)
                        {
                            sno = 1;
                        }
                        else
                        {
                            sno = ((page - 1) * pageSize) + 1;
                        }
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MemberModel()
                            {
                                SNo = sno + i,
                                MemberId = query[i].m.MemberId,
                                MemberTypeName = query[i].cc.CodeValDetail,
                                EmployeeName = query[i].m.EmployeeName,
                                EmployeeCode = query[i].m.EmployeeNo,
                                FrmDate = String.Format("{0:s}", query[i].m.FromDate),
                                TostrDate = String.Format("{0:s}", query[i].m.Effectivedate),
                                Status = query[i].m.Status
                            });
                        }
                    }
                }
                serchmem.memlist = list;
                return serchmem;
            }
            catch (Exception ex)
            {
                return serchmem;
            }
        }

        public static int InActiveMember(int MemberId, int UserId)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var chkstatus = context.tblRCTMember.Where(x => x.MemberId == MemberId && x.Status == "Active").FirstOrDefault();
                    if (chkstatus != null)
                    {
                        var InvStatus = (from inv in context.tblRCTMember
                                         where (inv.MemberId == MemberId)
                                         select inv).FirstOrDefault();
                        if (InvStatus != null)
                        {
                            InvStatus.ToDate = DateTime.Now;
                            InvStatus.UpdtTS = DateTime.Now;
                            InvStatus.UpdtUser = UserId;
                            InvStatus.Status = "InActive";
                            context.SaveChanges();
                        }
                        return 1;
                    }
                    return 2;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static MemberViewModel GetMemberView(int MemberId)
        {
            MemberViewModel viewmodel = new MemberViewModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblRCTMember.Where(x => x.MemberId == MemberId).FirstOrDefault();
                    if (query != null)
                    {
                        viewmodel.MemberType = Common.GetCodeControlnameCommon(query.TypeOfMember ?? 0, "MemberType");
                        viewmodel.EmployeeNo = query.EmployeeNo;
                        viewmodel.EmployeeName = query.EmployeeName;
                        viewmodel.EmployeeDesignation = query.EmployeeDesignation;
                        viewmodel.EmployeeDepartmentCode = query.DepartmentCode;
                        viewmodel.EmployeeDepartmentName = query.DepartmentName;
                        viewmodel.EmpEmail = query.Email;
                        viewmodel.ConNumber = query.ContactNumber;
                        viewmodel.FrmDate = string.Format("{0:dd}", (DateTime)query.FromDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.FromDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.FromDate);
                        viewmodel.ToStrDate = string.Format("{0:dd}", (DateTime)query.ToDate) + "-" + string.Format("{0:MMMM}", (DateTime)query.ToDate) + "-" + string.Format("{0:yyyy}", (DateTime)query.ToDate);
                        viewmodel.ActualendDate = string.Format("{0:dd-MMMM-yyyy}", query.Effectivedate);
                    }
                }
                return viewmodel;
            }
            catch (Exception ex)
            {

                return viewmodel;
            }
        }

        #endregion

        #region OutSourceing AgencyMaster
        public static int CreateOutsourceagency(AgencySalaryMasterModel model)
        {
            int resid = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            if (model.SalaryAgencyId > 0)
                            {
                                var querylog = (from s in context.tblSalaryAgencyMaster
                                                where s.SalaryAgencyId == model.SalaryAgencyId
                                                select s).FirstOrDefault();
                                if (querylog != null)
                                {
                                    tblSalaryAgencyMasterLog log = new tblSalaryAgencyMasterLog();
                                    log.SalaryAgencyId = querylog.SalaryAgencyId;
                                    log.AgencyName = querylog.AgencyName;
                                    log.ContactPerson = querylog.ContactPerson;
                                    log.ContactNumber = querylog.ContactNumber;
                                    log.ContactEmail = querylog.ContactEmail;
                                    log.Address = querylog.Address;
                                    log.GSTIN = querylog.GSTIN;
                                    log.GSTPercentage = querylog.GSTPercentage;
                                    log.PAN = querylog.PAN;
                                    log.StateId = querylog.StateId;
                                    log.BankName = querylog.BankName;
                                    log.AccountNumber = querylog.AccountNumber;
                                    log.BranchName = querylog.BranchName;
                                    log.IFSCCode = querylog.IFSCCode;
                                    log.District = querylog.District;
                                    log.PinCode = querylog.PinCode;
                                    log.Attachment = querylog.Attachment;
                                    log.Agencyfee = querylog.Agencyfee;
                                    log.Insurance = querylog.Insurance;
                                    log.GSTExcepted = querylog.GSTExcepted;
                                    log.GSTCertificateNo = querylog.GSTCertificateNo;
                                    log.GSTValidity = querylog.GSTValidity;
                                    log.TDSExcepted = querylog.TDSExcepted;
                                    log.TDSCertificateNo = querylog.TDSCertificateNo;
                                    log.TDSValidity = querylog.TDSValidity;
                                    log.CCMail = querylog.CCMail;
                                    log.Lastupdate_TS = querylog.Lastupdate_TS;
                                    log.LastupdatedUserid = querylog.LastupdatedUserid;
                                    log.Crtd_TS = DateTime.Now;
                                    log.Crtd_UserId = model.UserId;
                                    context.tblSalaryAgencyMasterLog.Add(log);
                                    context.SaveChanges();

                                    if (model.Agencyfee != querylog.Agencyfee)
                                    {
                                        tblSalaryAgencyComponentLog ComponentLog = new tblSalaryAgencyComponentLog();
                                        ComponentLog.SalaryAgencyId = model.SalaryAgencyId;
                                        ComponentLog.ComponentId = 1;
                                        ComponentLog.ComponentValue = model.Agencyfee;
                                        ComponentLog.OldComponentValue = querylog.Agencyfee;
                                        ComponentLog.Created_Id = model.UserId;
                                        ComponentLog.Created_TS = DateTime.Now;
                                        context.tblSalaryAgencyComponentLog.Add(ComponentLog);
                                        context.SaveChanges();
                                    }
                                    if (model.Insurance != querylog.Insurance)
                                    {
                                        tblSalaryAgencyComponentLog ComponentLog = new tblSalaryAgencyComponentLog();
                                        ComponentLog.SalaryAgencyId = model.SalaryAgencyId;
                                        ComponentLog.ComponentId = 2;
                                        ComponentLog.ComponentValue = model.Insurance;
                                        ComponentLog.OldComponentValue = querylog.Insurance;
                                        ComponentLog.Created_Id = model.UserId;
                                        ComponentLog.Created_TS = DateTime.Now;
                                        context.tblSalaryAgencyComponentLog.Add(ComponentLog);
                                        context.SaveChanges();
                                    }
                                    if (model.GSTPercentage != querylog.GSTPercentage)
                                    {
                                        tblSalaryAgencyComponentLog ComponentLog = new tblSalaryAgencyComponentLog();
                                        ComponentLog.SalaryAgencyId = model.SalaryAgencyId;
                                        ComponentLog.ComponentId = 3;
                                        ComponentLog.ComponentValue = model.GSTPercentage;
                                        ComponentLog.OldComponentValue = querylog.GSTPercentage;
                                        ComponentLog.Created_Id = model.UserId;
                                        ComponentLog.Created_TS = DateTime.Now;
                                        context.tblSalaryAgencyComponentLog.Add(ComponentLog);
                                        context.SaveChanges();
                                    }
                                }

                            }

                            if (model.SalaryAgencyId == null)
                            {
                                var checkagencyname = (from cc in context.tblSalaryAgencyMaster
                                                       where cc.AgencyName == model.AgencyName && cc.Status != "InActive"
                                                       select new { cc.GSTIN, cc.PAN }).FirstOrDefault();
                                var checkgstno = context.tblSalaryAgencyMaster.Where(x => x.GSTIN == model.GSTIN && x.Status != "InActive").FirstOrDefault();
                                if (checkgstno != null)
                                    return 3;
                                var checkpanno = context.tblSalaryAgencyMaster.Where(x => x.PAN == model.PAN && x.Status != "InActive").FirstOrDefault();
                                if (checkpanno != null)
                                    return 4;
                                if (checkagencyname != null)
                                {
                                    if (checkagencyname.GSTIN == model.GSTIN)
                                        return 5;
                                    if (checkagencyname.PAN == model.PAN)
                                        return 6;
                                }
                                var sqnbr = (from S in context.tblSalaryAgencyMaster
                                             select S.SeqNbr
                                       ).Max();
                                tblSalaryAgencyMaster addnewagency = new tblSalaryAgencyMaster();
                                addnewagency.AgencyName = model.AgencyName;
                                addnewagency.ContactPerson = model.ContactPerson;
                                addnewagency.ContactNumber = model.ContactNumber;
                                addnewagency.ContactEmail = model.ContactEmail;
                                addnewagency.Address = model.Address;
                                addnewagency.Crtd_TS = DateTime.Now;
                                addnewagency.Crtd_UserId = model.UserId;
                                addnewagency.AgencyCode = "SAM/" + (Convert.ToInt32(sqnbr) + 1).ToString("D6");
                                addnewagency.SeqNbr = (Convert.ToInt32(sqnbr) + 1);
                                addnewagency.Status = "Active";
                                addnewagency.GSTIN = model.GSTIN;
                                addnewagency.PAN = model.PAN;
                                addnewagency.StateId = model.StateId;
                                addnewagency.GSTPercentage = model.GSTPercentage;
                                addnewagency.BankName = model.BankName;
                                addnewagency.BranchName = model.BranchName;
                                addnewagency.AccountNumber = model.AccountNumber;
                                addnewagency.IFSCCode = model.IFSCCode;
                                addnewagency.District = model.District;
                                addnewagency.PinCode = model.PinCode;
                                addnewagency.Agencyfee = model.Agencyfee;
                                addnewagency.Insurance = model.Insurance;
                                addnewagency.GSTExcepted = model.GSTExcepted;
                                addnewagency.GSTCertificateNo = model.GSTCertificateNo;
                                addnewagency.GSTValidity = model.GSTValidity;
                                addnewagency.TDSExcepted = model.TDSExcepted;
                                addnewagency.TDSCertificateNo = model.TDSCertificateNo;
                                addnewagency.TDSValidity = model.TDSValidity;
                                addnewagency.CCMail = model.CCMail;
                                context.tblSalaryAgencyMaster.Add(addnewagency);
                                context.SaveChanges();
                                int salaId = addnewagency.SalaryAgencyId;
                                foreach (var item in model.AgencyDocList)
                                {
                                    if (item.DocumentName != null)
                                    {
                                        tblSalaryAgencyMasterDoc adddoc = new tblSalaryAgencyMasterDoc();
                                        adddoc.DocumentName = item.DocumentName;
                                        adddoc.SalaryAgencyId = salaId;
                                        adddoc.CRTD_By = model.UserId;
                                        adddoc.CRTD_TS = DateTime.Now;
                                        adddoc.Status = "Active";
                                        if (item.Document != null)
                                        {
                                            string actName = System.IO.Path.GetFileName(item.Document.FileName);
                                            var guid = Guid.NewGuid().ToString();
                                            var docName = guid + "_" + actName;
                                            //model.Document.UploadFile("Requirement", docName);
                                            item.Document.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + docName));
                                            adddoc.DocumentFileName = actName;
                                            adddoc.DocumentFilePath = docName;
                                        }
                                        context.tblSalaryAgencyMasterDoc.Add(adddoc);
                                        context.SaveChanges();
                                    }
                                }
                                resid = 1;


                            }
                            else
                            {
                                var updateagency = context.tblSalaryAgencyMaster.Where(x => x.SalaryAgencyId == model.SalaryAgencyId).FirstOrDefault();
                                if (updateagency != null)
                                {

                                    updateagency.AgencyName = model.AgencyName;
                                    updateagency.ContactPerson = model.ContactPerson;
                                    updateagency.ContactNumber = model.ContactNumber;
                                    updateagency.ContactEmail = model.ContactEmail;
                                    updateagency.Address = model.Address;
                                    updateagency.GSTPercentage = model.GSTPercentage;
                                    updateagency.Lastupdate_TS = DateTime.Now;
                                    updateagency.LastupdatedUserid = model.UserId;
                                    updateagency.GSTIN = model.GSTIN;
                                    updateagency.PAN = model.PAN;
                                    updateagency.StateId = model.StateId;
                                    updateagency.BankName = model.BankName;
                                    updateagency.AccountNumber = model.AccountNumber;
                                    updateagency.BranchName = model.BranchName;
                                    updateagency.IFSCCode = model.IFSCCode;
                                    updateagency.District = model.District;
                                    updateagency.PinCode = model.PinCode;
                                    updateagency.Agencyfee = model.Agencyfee;
                                    updateagency.Insurance = model.Insurance;
                                    updateagency.GSTExcepted = model.GSTExcepted;
                                    updateagency.GSTCertificateNo = model.GSTCertificateNo;
                                    updateagency.GSTValidity = model.GSTValidity;
                                    updateagency.TDSExcepted = model.TDSExcepted;
                                    updateagency.TDSCertificateNo = model.TDSCertificateNo;
                                    updateagency.TDSValidity = model.TDSValidity;
                                    updateagency.CCMail = model.CCMail;
                                    context.SaveChanges();
                                    var othdoc = model.AgencyDocList.Select(x => x.DocumentId ?? 0).ToArray();
                                    context.tblSalaryAgencyMasterDoc.Where(x => x.SalaryAgencyId == model.SalaryAgencyId && !othdoc.Contains(x.SalaryDocumentId) && x.Status != "InActive")
                                    .ToList()
                                    .ForEach(m =>
                                    {
                                        m.Status = "InActive";
                                        m.UPDT_TS = DateTime.Now;
                                        m.UPDT_By = model.UserId;
                                    });
                                    context.SaveChanges();
                                    if (model.AgencyDocList.Count > 0)
                                    {
                                        foreach (var item in model.AgencyDocList)
                                        {
                                            var changedoc = context.tblSalaryAgencyMasterDoc.Where(x => x.SalaryAgencyId == model.SalaryAgencyId && x.SalaryDocumentId == item.DocumentId).FirstOrDefault();
                                            if (changedoc != null)
                                            {
                                                changedoc.DocumentName = item.DocumentName;
                                                changedoc.UPDT_By = model.UserId;
                                                changedoc.UPDT_TS = DateTime.Now;
                                                if (item.Document != null)
                                                {
                                                    string actName = System.IO.Path.GetFileName(item.Document.FileName);
                                                    var guid = Guid.NewGuid().ToString();
                                                    var docName = guid + "_" + actName;
                                                    //model.Document.UploadFile("Requirement", docName);
                                                    item.Document.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + docName));
                                                    changedoc.DocumentFileName = actName;
                                                    changedoc.DocumentFilePath = docName;
                                                }
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                tblSalaryAgencyMasterDoc adddoc = new tblSalaryAgencyMasterDoc();
                                                adddoc.DocumentName = item.DocumentName;
                                                adddoc.SalaryAgencyId = model.SalaryAgencyId;
                                                adddoc.CRTD_By = model.UserId;
                                                adddoc.CRTD_TS = DateTime.Now;
                                                adddoc.Status = "Active";
                                                if (item.Document != null)
                                                {
                                                    string actName = System.IO.Path.GetFileName(item.Document.FileName);
                                                    var guid = Guid.NewGuid().ToString();
                                                    var docName = guid + "_" + actName;
                                                    //model.Document.UploadFile("Requirement", docName);
                                                    item.Document.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + docName));
                                                    adddoc.DocumentFileName = actName;
                                                    adddoc.DocumentFilePath = docName;
                                                }
                                                context.tblSalaryAgencyMasterDoc.Add(adddoc);
                                                context.SaveChanges();
                                            }

                                        }
                                    }

                                    resid = 2;


                                }
                            }
                            transaction.Commit();
                            return resid;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ErrorHandler errlog = new ErrorHandler();
                            errlog.SendErrorToText(ex);
                            return -1;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler errlog = new ErrorHandler();
                errlog.SendErrorToText(ex);
                return -1;
            }
        }
        public static AgencySalaryMasterModel EditOutsourceagency(int SalaryId)
        {
            AgencySalaryMasterModel editmodel = new AgencySalaryMasterModel();
            List<AgencySalaryDocModel> list = new List<AgencySalaryDocModel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    var editquery = context.tblSalaryAgencyMaster.Where(x => x.SalaryAgencyId == SalaryId).FirstOrDefault();
                    if (editquery != null)
                    {
                        editmodel.SalaryAgencyId = SalaryId;
                        editmodel.AgencyName = editquery.AgencyName;
                        editmodel.ContactPerson = editquery.ContactPerson;
                        editmodel.ContactNumber = editquery.ContactNumber;
                        editmodel.ContactEmail = editquery.ContactEmail;
                        editmodel.Address = editquery.Address;
                        editmodel.GSTIN = editquery.GSTIN;
                        editmodel.GSTPercentage = editquery.GSTPercentage;
                        editmodel.PAN = editquery.PAN;
                        editmodel.StateId = editquery.StateId;
                        editmodel.BankName = editquery.BankName;
                        editmodel.AccountNumber = editquery.AccountNumber;
                        editmodel.BranchName = editquery.BranchName;
                        editmodel.IFSCCode = editquery.IFSCCode;
                        editmodel.District = editquery.District;
                        editmodel.PinCode = editquery.PinCode;
                        editmodel.DocumentPath = editquery.Attachment;
                        editmodel.Agencyfee = editquery.Agencyfee;
                        editmodel.Insurance = editquery.Insurance;
                        editmodel.GSTExcepted = editquery.GSTExcepted ?? false;
                        editmodel.GSTCertificateNo = editquery.GSTCertificateNo;
                        editmodel.GSTValidity = editquery.GSTValidity;
                        editmodel.TDSExcepted = editquery.TDSExcepted ?? false;
                        editmodel.TDSCertificateNo = editquery.TDSCertificateNo;
                        editmodel.TDSValidity = editquery.TDSValidity;
                        editmodel.CCMail = editquery.CCMail;
                        var querydoc = context.tblSalaryAgencyMasterDoc.Where(x => x.SalaryAgencyId == SalaryId && x.Status != "InActive").ToList();
                        if (querydoc.Count > 0)
                        {
                            for (int i = 0; i < querydoc.Count; i++)
                            {
                                list.Add(new AgencySalaryDocModel()
                                {
                                    DocumentId = querydoc[i].SalaryDocumentId,
                                    DocumentName = querydoc[i].DocumentName,
                                    DocumentPath = querydoc[i].DocumentFilePath
                                });
                            }
                        }
                    }
                    editmodel.AgencyDocList = list;
                }
                return editmodel;
            }
            catch (Exception ex)
            {
                ErrorHandler errlog = new ErrorHandler();
                errlog.SendErrorToText(ex);
                return editmodel;
            }
        }
        public static AgencySalaryMasterSearchModel GetAgencySalarySearch(AgencySalaryMasterSearchModel model, int page, int pageSize, DateFilterModel ModifyDate)
        {
            AgencySalaryMasterSearchModel Searchmodel = new AgencySalaryMasterSearchModel();
            List<AgencySalaryMasterModel> Searchlist = new List<AgencySalaryMasterModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (page == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (page - 1) * pageSize;
                    }
                    var prequery = (from agy in context.tblSalaryAgencyMaster
                                    where agy.Status != "InActive"
                                    select new AgencySalaryMasterModel()
                                    {
                                        SalaryAgencyId = agy.SalaryAgencyId,
                                        AgencyName = agy.AgencyName,
                                        ContactPerson = agy.ContactPerson,
                                        GSTIN = agy.GSTIN,
                                        AgencyCode = agy.AgencyCode,
                                        Status = agy.Status,
                                        LastUpdatedDate = agy.Lastupdate_TS == null ? agy.Crtd_TS : agy.Lastupdate_TS,
                                    });
                    var predicate = PredicateBuilder.BaseAnd<AgencySalaryMasterModel>();
                    if (!string.IsNullOrEmpty(model.SearchAgencyCode))
                        predicate = predicate.And(d => d.AgencyCode.Contains(model.SearchAgencyCode));
                    if (!string.IsNullOrEmpty(model.SearchAgencyname))
                        predicate = predicate.And(d => d.AgencyName.Contains(model.SearchAgencyname));
                    if (!string.IsNullOrEmpty(model.SearchContactperson))
                        predicate = predicate.And(d => d.ContactPerson.Contains(model.SearchContactperson));
                    if (!string.IsNullOrEmpty(model.SearchGSTINnumber))
                        predicate = predicate.And(d => d.GSTIN.Contains(model.SearchGSTINnumber));
                    if (!string.IsNullOrEmpty(model.SearchStatus))
                        predicate = predicate.And(d => d.Status.Contains(model.SearchStatus));
                    if (ModifyDate.@from != null && ModifyDate.to != null)
                    {
                        ModifyDate.to = ModifyDate.to.Value.Date.AddDays(1).AddTicks(-2);
                        predicate = predicate.And(d => d.LastUpdatedDate >= ModifyDate.from && d.LastUpdatedDate <= ModifyDate.to);
                    }
                    var qryList = prequery.Where(predicate).OrderByDescending(m => m.SalaryAgencyId).Skip(skiprec).Take(pageSize).ToList();
                    Searchmodel.TotalRecords = prequery.Where(predicate).Count();
                    if (qryList.Count > 0)
                    {
                        int sno = 0;
                        if (page == 1)
                        {
                            sno = 1;
                        }
                        else
                        {
                            sno = ((page - 1) * pageSize) + 1;
                        }
                        for (int i = 0; i < qryList.Count; i++)
                        {
                            Searchlist.Add(new AgencySalaryMasterModel()
                            {
                                SNo = sno + i,
                                AgencyCode = qryList[i].AgencyCode,
                                AgencyName = qryList[i].AgencyName,
                                ContactPerson = qryList[i].ContactPerson,
                                GSTIN = qryList[i].GSTIN,
                                Status = qryList[i].Status,
                                SalaryAgencyId = qryList[i].SalaryAgencyId,
                                ModifyDate = String.Format("{0: dd-MMMM-yyyy}", qryList[i].LastUpdatedDate),
                            });
                        }
                    }
                }
                Searchmodel.List = Searchlist;
                return Searchmodel;
            }
            catch (Exception ex)
            {
                ErrorHandler errlog = new ErrorHandler();
                errlog.SendErrorToText(ex);
                return Searchmodel;
            }
        }

        #endregion

        #region Amendment law

        public static int OSGSalaryLog(int componentvalueId, int componentId, List<string> employeeids, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (employeeids.Count > 0)
                    {
                        var querylog = (from sck in context.tblRCTOutsourcing
                                        join cc in context.tblRCTSalaryCalcDetails on sck.OSGID equals cc.ID
                                        where sck.Status == "Verification Completed" && sck.IsActiveNow == true && cc.IsCurrentVersion == true && cc.Status == "Active"
                                        && employeeids.Contains(sck.EmployeersID)
                                        select cc).ToList();
                        foreach (var item in querylog)
                        {
                            tblRCTSalaryCalcDetailsLog log = new tblRCTSalaryCalcDetailsLog();
                            log.SalaryDetailsId = item.SalaryDetailsId;
                            log.ID = item.ID;
                            log.AppointType = item.AppointType;
                            log.TypeCode = item.TypeCode;
                            log.EmpNo = item.EmpNo;
                            log.EmpType = item.EmpType;
                            log.RecommendSalary = item.RecommendSalary;
                            log.Salutation = item.Salutation;
                            log.EmpName = item.EmpName;
                            log.EmpDesignation = item.EmpDesignation;
                            log.PhysicallyHandicapped = item.PhysicallyHandicapped;
                            log.PFBasicWages = item.PFBasicWages;
                            log.EmployeePF = item.EmployeePF;
                            log.EmployeeESIC = item.EmployeeESIC;
                            log.EmpProfessionalTax = item.EmpProfessionalTax;
                            log.EmpTotalDeduction = item.EmpTotalDeduction;
                            log.EmpNetSalary = item.EmpNetSalary;
                            log.EmployerPF = item.EmployerPF;
                            log.EmployerInsurance = item.EmployerInsurance;
                            log.EmployerESIC = item.EmployerESIC;
                            log.EmployerTotalContribution = item.EmployerTotalContribution;
                            log.EmployerCTC = item.EmployerCTC;
                            log.EmployerAgencyFee = item.EmployerAgencyFee;
                            log.EmployerGST = item.EmployerGST;
                            log.EmployerCTCWithAgencyFee = item.EmployerCTCWithAgencyFee;
                            log.TotalCostPerMonth = item.TotalCostPerMonth;
                            log.CrtdTS = item.CrtdTS;
                            log.CrtdUserId = item.CrtdUserId;
                            log.UpdtTS = item.UpdtTS;
                            log.UpdtUserId = item.UpdtUserId;
                            log.LogCrtdTS = DateTime.Now;
                            log.LogCrtdUserId = userId;
                            log.Status = item.Status;
                            log.IsCurrentVersion = item.IsCurrentVersion;
                            log.SalaryStructureDocPath = item.SalaryStructureDocPath;
                            log.OrderId = item.OrderId;
                            log.FromDate = item.FromDate;
                            log.ToDate = item.ToDate;
                            log.StatutoryId = item.StatutoryId;
                            log.ComponentId = componentId;
                            log.ComponentValueId = componentvalueId;
                            context.tblRCTSalaryCalcDetailsLog.Add(log);
                            context.SaveChanges();
                        }
                    }
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static bool EmployeeHistoryLog(int OrderId)
        {
            bool status = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    OrderModel model = new OrderModel();
                    if (context.tblRCTOrderHistory.Any(x => x.OrderId == OrderId))
                        return false;
                    var query = (from o in context.tblOrder
                                 from od in context.tblOrderDetail
                                 from vw in context.vw_RCTOverAllApplicationEntry
                                 where o.OrderId == od.OrderId && o.OrderId == vw.OrderId
                                 && o.OrderId == OrderId
                                 select new { od, o, vw }).FirstOrDefault();
                    if (query != null)
                    {
                        model.EmployeeID = query.vw.EmployeersID;
                        model.ApplicationID = query.vw.ApplicationId ?? 0;
                        model.OrderID = query.o.OrderId;
                        model.OrderType = query.o.OrderType ?? 0;
                        model.OrderTypestr = query.vw.ApplicationType;
                        model.Category = query.vw.Category;
                        model.FromDate = query.o.FromDate;
                        model.ToDate = query.o.ToDate;
                        model.Salary = query.o.Basic;
                        model.HRA = query.o.HRA ?? 0;
                        model.MedicalAmmount = query.vw.MedicalAmmount ?? 0;
                        model.DesignationId = query.vw.DesignationId;
                        model.ProjectId = query.vw.ProjectId;
                        model.OrderDate = query.vw.ApplicationReceiveDate;
                    }


                    int?[] exceptedType = { 7, 8, 10 };

                    var QryDatas = context.tblRCTOrderHistory
                        .Where(m => m.EmployeeId == model.EmployeeID && m.ApplicationId == model.ApplicationID
                        && m.AppointmentType == model.Category && m.IsCanceled != true
                        && !exceptedType.Contains(m.OrderTypeId) && !exceptedType.Contains(model.OrderType)
                        && m.EffectiveTo != m.ActualAppointmentEndDate
                        && ((m.ActualAppointmentEndDate == null && m.EffectiveTo >= model.FromDate) || (m.ActualAppointmentEndDate != null && m.ActualAppointmentEndDate >= model.FromDate))
                        ).ToList();

                    if (!exceptedType.Contains(model.OrderType))
                    {
                        if (QryDatas.Count > 0)
                        {
                            foreach (var data in QryDatas)
                            {
                                data.isHRAUpdate = false;
                                if (model.FromDate > data.EffectiveFrom)
                                    data.ActualAppointmentEndDate = model.FromDate.Value.AddDays(-1);
                                else
                                    data.ActualAppointmentEndDate = model.FromDate;
                                context.SaveChanges();
                            }
                        }
                    }

                    tblRCTOrderHistory his = new tblRCTOrderHistory();
                    his.ApplicationId = model.ApplicationID;
                    his.AppointmentType = model.Category;
                    his.Basic = model.Salary;
                    his.DesignationId = model.DesignationId;
                    if (model.OrderType != 9)
                        his.EffectiveFrom = model.FromDate;
                    his.EffectiveTo = model.ToDate;
                    his.EmployeeId = model.EmployeeID;
                    his.OrderId = model.OrderID;
                    his.OrderTypeId = model.OrderType;
                    his.OrderType = model.OrderTypestr;
                    his.ProjectId = model.ProjectId;
                    his.OrderDate = model.OrderDate;
                    context.tblRCTOrderHistory.Add(his);
                    context.SaveChanges();
                    status = true;
                }
                return status;
            }
            catch (Exception ex)
            {
                return status;
            }
        }

        public static int UpdateOSGSalaryComp(int componentId, int UserId)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (componentId > 0)
                        {
                            List<int> orderhis = new List<int>();

                            DateTime date = DateTime.Now.Date;
                            if (componentId == 14)//PT amendment
                            {
                                var queryPT = context.tblRCTProfessionalTax.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                                if (queryPT != null)
                                {
                                    DateTime valueDate = queryPT.ValueDate ?? DateTime.Now;
                                    var queryemp = (from M in context.tblRCTOutsourcing
                                                    from D in context.tblRCTSalaryCalcDetails
                                                    where M.OSGID == D.ID && M.Status == "Verification Completed" && M.IsActiveNow == true && D.IsCurrentVersion == true && D.Status == "Active"
                                                    && (M.AppointmentStartdate <= valueDate && M.AppointmentEnddate >= valueDate)
                                                    && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == queryPT.ProfessionalTaxId && x.ComponentId == componentId && x.ID == M.OSGID))
                                                    select new { M, D }).ToList();
                                    if (queryemp.Count == 0)
                                        return -1;
                                    List<string> Employees = queryemp.Select(x => x.M.EmployeersID).ToList();
                                    var res = OSGSalaryLog(queryPT.ProfessionalTaxId, componentId, Employees, UserId);
                                    if (res != 1)
                                        return 0;
                                    foreach (var item in queryemp)
                                    {
                                        int orderId = 0;
                                        int ID = item.M.OSGID;
                                        int orderwiseseqId = (from O in context.tblOrder
                                                              where O.OrderType == componentId
                                                              select O.OrderwiseSeqId).Max() ?? 0;
                                        orderwiseseqId = orderwiseseqId == 0 ? 1 : orderwiseseqId + 1;
                                        int seqId = (from O in context.tblOrder select O.SeqId).Max() ?? 0;
                                        int number = seqId == 0 ? 1 : seqId + 1;
                                        string value = number.ToString("D4");
                                        value = "PT" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + value;

                                        bool updated_f = false;
                                        if (queryPT.ValueDate <= DateTime.Now.Date)
                                            updated_f = true;
                                        item.D.IsCurrentVersion = updated_f == true ? false : true; //pre Dails
                                        if (item.D.IsCurrentVersion == false)
                                        {
                                            item.D.UpdtTS = DateTime.Now;
                                            item.D.UpdtUserId = UserId;
                                        }

                                        tblOrder oQuery = new tblOrder();
                                        oQuery.AppointmentId = item.M.OSGID;
                                        oQuery.AppointmentType = 3;
                                        oQuery.OrderDate = queryPT.ValueDate;
                                        oQuery.OrderType = componentId;
                                        oQuery.FromDate = queryPT.ValueDate;
                                        oQuery.ToDate = item.M.AppointmentEnddate;
                                        oQuery.Status = "Completed";
                                        oQuery.Basic = item.M.Salary;
                                        oQuery.OldProjectId = item.M.ProjectId;
                                        oQuery.NewProjectId = item.M.ProjectId;
                                        oQuery.OldDesignation = item.M.DesignationId;
                                        oQuery.NewDesignation = item.M.DesignationId;
                                        oQuery.CrtdTS = DateTime.Now;
                                        oQuery.CrtdUser = UserId;
                                        oQuery.OrderNo = value;
                                        oQuery.SeqId = number;
                                        oQuery.OrderwiseSeqId = orderwiseseqId;
                                        oQuery.isUpdated = updated_f;
                                        context.tblOrder.Add(oQuery);
                                        context.SaveChanges();
                                        orderId = oQuery.OrderId;
                                        orderhis.Add(orderId);
                                        tblOrderDetail oDQuery = new tblOrderDetail();
                                        oDQuery.OrderId = orderId;
                                        oDQuery.Comments = "Back end PT entry professionalTaxId_" + queryPT.ProfessionalTaxId;
                                        context.tblOrderDetail.Add(oDQuery);
                                        context.SaveChanges();

                                        decimal salary = item.M.Salary ?? 0;
                                        decimal profesTax = 0;

                                        if (salary <= 3500)
                                            profesTax = queryPT.MonthySalary35 ?? 0;
                                        else if (salary > 3500 && salary <= 5000)
                                            profesTax = queryPT.MonthySalary35to5 ?? 0;
                                        else if (salary > 5000 && salary <= 7500)
                                            profesTax = queryPT.MonthySalary5to75 ?? 0;
                                        else if (salary > 7500 && salary <= 10000)
                                            profesTax = queryPT.MonthySalary75to10 ?? 0;
                                        else if (salary > 10000 && salary <= 12500)
                                            profesTax = queryPT.MonthySalary10to12 ?? 0;
                                        else if (salary > 12500)
                                            profesTax = queryPT.MonthySalaryAbove12 ?? 0;
                                        if (item.M.PhysicallyChallenged == "Yes")
                                            profesTax = 0;

                                        decimal ttldeductions = (item.D.EmployeePF ?? 0) + (item.D.EmployeeESIC ?? 0) + profesTax;
                                        decimal empnetsalary = salary - ttldeductions;

                                        tblRCTSalaryCalcDetails SalaryCal = new tblRCTSalaryCalcDetails();
                                        SalaryCal.ID = item.M.OSGID;
                                        SalaryCal.AppointType = "Outsourcing";
                                        SalaryCal.TypeCode = "OSG";
                                        SalaryCal.RecommendSalary = item.D.RecommendSalary;
                                        SalaryCal.Salutation = item.D.Salutation;
                                        SalaryCal.EmpName = item.D.EmpName;
                                        SalaryCal.EmpDesignation = item.D.EmpDesignation;
                                        SalaryCal.PhysicallyHandicapped = item.D.PhysicallyHandicapped;
                                        SalaryCal.PFBasicWages = item.D.PFBasicWages;
                                        SalaryCal.EmployeePF = item.D.EmployeePF;
                                        SalaryCal.EmployeeESIC = item.D.EmployeeESIC;
                                        SalaryCal.EmpProfessionalTax = profesTax;//
                                        SalaryCal.EmpTotalDeduction = ttldeductions;//
                                        SalaryCal.EmpNetSalary = empnetsalary;//
                                        SalaryCal.EmployerPF = item.D.EmployerPF;
                                        SalaryCal.EmployerInsurance = item.D.EmployerInsurance;
                                        SalaryCal.EmployerESIC = item.D.EmployerESIC;
                                        SalaryCal.EmployerCTC = item.D.EmployerCTC;
                                        SalaryCal.EmployerAgencyFee = item.D.EmployerAgencyFee;
                                        SalaryCal.EmployerGST = item.D.EmployerGST;
                                        SalaryCal.EmployerCTCWithAgencyFee = item.D.EmployerCTCWithAgencyFee;
                                        SalaryCal.TotalCostPerMonth = item.D.TotalCostPerMonth;
                                        SalaryCal.EmployerTotalContribution = item.D.EmployerTotalContribution;
                                        SalaryCal.CrtdTS = DateTime.Now;
                                        SalaryCal.CrtdUserId = UserId;
                                        SalaryCal.Status = "Active";
                                        SalaryCal.IsCurrentVersion = updated_f;
                                        SalaryCal.OrderId = orderId;
                                        SalaryCal.FromDate = queryPT.ValueDate;
                                        SalaryCal.ToDate = item.M.AppointmentEnddate;
                                        SalaryCal.StatutoryId = item.D.StatutoryId;
                                        SalaryCal.GSTPercentage = item.D.GSTPercentage;
                                        SalaryCal.AgencyFeePercentage = item.D.AgencyFeePercentage;
                                        context.tblRCTSalaryCalcDetails.Add(SalaryCal);
                                        context.SaveChanges();

                                    }
                                }
                            }
                            if (componentId == 12)///Statutory Amendment
                            {
                                var query = context.tblRCTStatutory.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                                if (query != null)
                                {
                                    var queryemp = (from mast in context.tblRCTOutsourcing
                                                    join det in context.tblRCTSalaryCalcDetails on mast.OSGID equals det.ID
                                                    where mast.Status == "Verification Completed" && mast.IsActiveNow == true && det.IsCurrentVersion == true && det.Status == "Active"
                                                    && (mast.AppointmentStartdate <= query.ValueDate && mast.AppointmentEnddate >= query.ValueDate)
                                                    && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == query.StatutoryId && x.ComponentId == componentId && x.ID == mast.OSGID))
                                                    select new { mast, det }).ToList();
                                    if (queryemp.Count == 0)
                                        return -1;
                                    List<string> Employees = queryemp.Select(x => x.mast.EmployeersID).ToList();
                                    int res = OSGSalaryLog(query.StatutoryId, componentId, Employees, UserId);
                                    if (res != 1)
                                        return 0;
                                    foreach (var item in queryemp)
                                    {
                                        decimal salary = item.mast.Salary ?? 0;
                                        var LWFAmount = query.LWFEmployerContribution;
                                        var EmployeerLWFAmount = query.LWFEmployerContribution;
                                        DateTime appointmentStart = item.mast.AppointmentStartdate ?? DateTime.Now;
                                        DateTime appointmentEnd = item.mast.AppointmentEnddate ?? DateTime.Now;

                                        if (query.ValueDate.Value.Year < appointmentEnd.Year)
                                            EmployeerLWFAmount = LWFAmount;
                                        else if (query.ValueDate.Value.Month == 12 || appointmentEnd.Month == 12)
                                            EmployeerLWFAmount = LWFAmount;

                                        decimal emppfpercent = (query.PFEmployeePercentage ?? 0) / 100;
                                        decimal emplyrpfpercent = (query.PFEmployerPercentage ?? 0) / 100;

                                        decimal empESICpercent = (query.ESICEmployeePercentage ?? 0) / 100;
                                        decimal emplyrESICpercent = (query.ESICEmployerPercentage ?? 0) / 100;

                                        //decimal empESICamt = Math.Ceiling(empESICpercent * salary);
                                        //decimal emplyrESICamt = Math.Ceiling(emplyrESICpercent * salary);

                                        decimal pfSlab = query.PFEmployeeAmount ?? 0;
                                        decimal genESICSlab = query.ESICEmployeegeneralamount ?? 0;
                                        decimal phESICSlab = query.ESICEmployeePhysicalAmount ?? 0;

                                        decimal emppfbasicwages = item.det.PFBasicWages ?? 0;
                                        if (emppfbasicwages > pfSlab && emppfbasicwages > 0)
                                            emppfbasicwages = pfSlab;
                                        var isphysichandc = item.mast.PhysicallyChallenged;
                                        var typeofappoint = item.mast.TypeofAppointment;
                                        decimal empPF = Math.Round(emppfbasicwages * emppfpercent);
                                        decimal employerPF = Math.Round(emppfbasicwages * emplyrpfpercent);
                                        decimal empESIC = Math.Ceiling(salary * empESICpercent);
                                        decimal employerESIC = Math.Ceiling(salary * emplyrESICpercent);

                                        if (salary > genESICSlab)
                                        {
                                            empESIC = 0;
                                            employerESIC = 0;
                                        }
                                        if (typeofappoint == 2)
                                        {
                                            empPF = 0;
                                            employerPF = 0;
                                        }
                                        decimal profesTax = item.det.EmpProfessionalTax ?? 0;
                                        decimal employerIns = item.det.EmployerInsurance ?? 0;
                                        decimal agencyfeeper = item.det.AgencyFeePercentage ?? 0;
                                        decimal agencyfeeval = agencyfeeper / 100;
                                        decimal ttldeductions = empPF + empESIC + profesTax;
                                        decimal empnetsalary = salary - ttldeductions;
                                        decimal emplyrttlContr = employerPF + employerESIC + employerIns;
                                        decimal employeeCTC = salary + emplyrttlContr;
                                        decimal oldemployeeCTC = item.det.EmployerCTC ?? 0;
                                        decimal diffemployeeCTC = employeeCTC - oldemployeeCTC;
                                        decimal agencyFee = Math.Round(employeeCTC * agencyfeeval);
                                        decimal ctcwithagencyfee = employeeCTC + agencyFee;
                                        decimal gstpercent = item.det.GSTPercentage ?? 0;
                                        decimal salaryGSTamt = Math.Round(ctcwithagencyfee * (gstpercent / 100));
                                        decimal totalCTC = ctcwithagencyfee + salaryGSTamt;

                                        int orderId = 0;
                                        int ID = item.mast.OSGID;
                                        int orderwiseseqId = (from O in context.tblOrder
                                                              where O.OrderType == componentId
                                                              select O.OrderwiseSeqId).Max() ?? 0;
                                        orderwiseseqId = orderwiseseqId == 0 ? 1 : orderwiseseqId + 1;
                                        int seqId = (from O in context.tblOrder
                                                     select O.SeqId).Max() ?? 0;
                                        int number = seqId == 0 ? 1 : seqId + 1;
                                        string value = number.ToString("D4");
                                        value = "ST" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + value;

                                        bool updated_f = false;
                                        if (query.ValueDate <= DateTime.Now.Date)
                                            updated_f = true;

                                        item.det.IsCurrentVersion = updated_f == true ? false : true; //pre details
                                        if (item.det.IsCurrentVersion == false)
                                        {
                                            item.det.UpdtTS = DateTime.Now;
                                            item.det.UpdtUserId = UserId;
                                        }
                                        tblOrder oQuery = new tblOrder();
                                        oQuery.AppointmentId = item.mast.OSGID;
                                        oQuery.AppointmentType = 3;
                                        oQuery.OrderDate = query.ValueDate;
                                        oQuery.OrderType = componentId;
                                        oQuery.FromDate = query.ValueDate;
                                        oQuery.ToDate = item.mast.AppointmentEnddate;
                                        oQuery.Status = "Completed";
                                        oQuery.Basic = item.mast.Salary;
                                        oQuery.OldProjectId = item.mast.ProjectId;
                                        oQuery.NewProjectId = item.mast.ProjectId;
                                        oQuery.OldDesignation = item.mast.DesignationId;
                                        oQuery.NewDesignation = item.mast.DesignationId;
                                        oQuery.CrtdTS = DateTime.Now;
                                        oQuery.CrtdUser = UserId;
                                        oQuery.OrderNo = value;
                                        oQuery.SeqId = number;
                                        oQuery.OrderwiseSeqId = orderwiseseqId;
                                        oQuery.isUpdated = updated_f;
                                        context.tblOrder.Add(oQuery);
                                        context.SaveChanges();
                                        orderId = oQuery.OrderId;
                                        orderhis.Add(orderId);

                                        tblOrderDetail oDQuery = new tblOrderDetail();
                                        oDQuery.OrderId = orderId;
                                        oDQuery.Comments = "Back end statutory entry statutoryId_" + query.StatutoryId;
                                        context.tblOrderDetail.Add(oDQuery);
                                        context.SaveChanges();


                                        tblRCTSalaryCalcDetails SalaryCal = new tblRCTSalaryCalcDetails();
                                        SalaryCal.ID = item.mast.OSGID;
                                        SalaryCal.AppointType = "Outsourcing";
                                        SalaryCal.TypeCode = "OSG";
                                        SalaryCal.RecommendSalary = item.det.RecommendSalary;
                                        SalaryCal.Salutation = item.det.Salutation;
                                        SalaryCal.EmpName = item.det.EmpName;
                                        SalaryCal.EmpDesignation = item.det.EmpDesignation;
                                        SalaryCal.PhysicallyHandicapped = item.det.PhysicallyHandicapped;
                                        SalaryCal.PFBasicWages = emppfbasicwages;//
                                        SalaryCal.EmployeePF = empPF;//
                                        SalaryCal.EmployeeESIC = empESIC;//
                                        SalaryCal.EmpProfessionalTax = item.det.EmpProfessionalTax;
                                        SalaryCal.EmpTotalDeduction = ttldeductions;//
                                        SalaryCal.EmpNetSalary = empnetsalary;//
                                        SalaryCal.EmployerPF = employerPF;//
                                        SalaryCal.EmployerInsurance = item.det.EmployerInsurance;
                                        SalaryCal.EmployerESIC = employerESIC;//
                                        SalaryCal.EmployerCTC = employeeCTC;//
                                        SalaryCal.EmployerAgencyFee = agencyFee;
                                        SalaryCal.EmployerGST = salaryGSTamt;
                                        SalaryCal.EmployerCTCWithAgencyFee = ctcwithagencyfee;
                                        SalaryCal.TotalCostPerMonth = totalCTC;
                                        SalaryCal.CrtdTS = DateTime.Now;
                                        SalaryCal.CrtdUserId = UserId;
                                        SalaryCal.Status = "Active";
                                        SalaryCal.IsCurrentVersion = updated_f;
                                        SalaryCal.EmployerTotalContribution = emplyrttlContr;
                                        SalaryCal.OrderId = orderId;
                                        SalaryCal.FromDate = query.ValueDate;
                                        SalaryCal.ToDate = item.mast.AppointmentEnddate;
                                        SalaryCal.StatutoryId = query.StatutoryId;
                                        SalaryCal.GSTPercentage = item.det.GSTPercentage;
                                        SalaryCal.AgencyFeePercentage = item.det.AgencyFeePercentage;
                                        context.tblRCTSalaryCalcDetails.Add(SalaryCal);
                                        context.SaveChanges();
                                    }
                                }
                            }

                            if (componentId == 13)
                            {
                                var agncy = context.tblSalaryAgencyMaster.Where(x => x.SalaryAgencyId == 2).FirstOrDefault();

                                var queryPIEligibleEmp = (from mast in context.tblRCTOutsourcing
                                                          join det in context.tblRCTSalaryCalcDetails on mast.OSGID equals det.ID
                                                          where mast.Status == "Verification Completed" && mast.IsActiveNow == true && det.IsCurrentVersion == true && det.Status == "Active"
                                                          && (mast.AppointmentStartdate <= agncy.Lastupdate_TS && mast.AppointmentEnddate >= agncy.Lastupdate_TS)
                                                          && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == agncy.UpdateSeqNo && x.ComponentId == componentId && x.ID == mast.OSGID))
                                                          select new { mast, det }).ToList();
                                if (queryPIEligibleEmp.Count == 0)
                                    return -1;
                                List<string> Employees = queryPIEligibleEmp.Select(x => x.mast.EmployeersID).ToList();
                                int res = OSGSalaryLog(agncy.UpdateSeqNo ?? 0, componentId, Employees, UserId);
                                if (res != 1)
                                    return 0;
                                foreach (var item in queryPIEligibleEmp)
                                {

                                    //var agncyId = item.mast.VendorId;
                                    decimal salary = item.mast.Salary ?? 0;
                                    decimal employerPF = item.det.EmployerPF ?? 0;
                                    decimal employerESIC = item.det.EmployerESIC ?? 0;
                                    var typeofappoint = item.mast.TypeofAppointment;
                                    decimal employerIns = agncy.Insurance ?? 0;

                                    //decimal profesTax = item.det.EmpProfessionalTax ?? 0;
                                    decimal agencyfeeper = agncy.Agencyfee ?? 0;
                                    decimal agencyfeeval = agencyfeeper / 100;
                                    if (typeofappoint == 2)
                                        employerIns = 0;
                                    decimal emplyrttlContr = employerPF + employerESIC + employerIns;
                                    decimal employeeCTC = salary + emplyrttlContr;
                                    decimal oldemployeeCTC = item.det.EmployerCTC ?? 0;
                                    decimal diffemployeeCTC = employeeCTC - oldemployeeCTC;
                                    decimal agencyFee = Math.Round(employeeCTC * agencyfeeval);
                                    //decimal agencyFee = osglist[i].cc.EmployerAgencyFee ?? 0;
                                    decimal ctcwithagencyfee = employeeCTC + agencyFee;
                                    //decimal gstpercent = item.det.GSTPercentage ?? 0;
                                    decimal gstpercent = agncy.GSTPercentage ?? 0;
                                    decimal salaryGSTamt = Math.Round(ctcwithagencyfee * (gstpercent / 100));
                                    decimal totalCTC = ctcwithagencyfee + salaryGSTamt;

                                    int orderId = 0;
                                    int ID = item.mast.OSGID;
                                    int orderwiseseqId = (from O in context.tblOrder
                                                          where O.OrderType == componentId
                                                          select O.OrderwiseSeqId).Max() ?? 0;
                                    orderwiseseqId = orderwiseseqId == 0 ? 1 : orderwiseseqId + 1;
                                    int seqId = (from O in context.tblOrder
                                                 select O.SeqId).Max() ?? 0;
                                    int number = seqId == 0 ? 1 : seqId + 1;
                                    string value = number.ToString("D4");
                                    value = "GST" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + value;

                                    bool updated_f = false;
                                    if (agncy.Lastupdate_TS.Value.Date <= DateTime.Now.Date)
                                        updated_f = true;

                                    item.det.IsCurrentVersion = updated_f == true ? false : true; //pre details
                                    if (item.det.IsCurrentVersion == false)
                                    {
                                        item.det.UpdtTS = DateTime.Now;
                                        item.det.UpdtUserId = UserId;
                                    }

                                    tblOrder oQuery = new tblOrder();
                                    oQuery.AppointmentId = item.mast.OSGID;
                                    oQuery.AppointmentType = 3;
                                    oQuery.OrderDate = agncy.Lastupdate_TS.Value.Date;
                                    oQuery.OrderType = componentId;
                                    oQuery.FromDate = agncy.Lastupdate_TS.Value.Date;
                                    oQuery.ToDate = item.mast.AppointmentEnddate;
                                    oQuery.Status = "Completed";
                                    oQuery.Basic = item.mast.Salary;
                                    oQuery.OldProjectId = item.mast.ProjectId;
                                    oQuery.NewProjectId = item.mast.ProjectId;
                                    oQuery.OldDesignation = item.mast.DesignationId;
                                    oQuery.NewDesignation = item.mast.DesignationId;
                                    oQuery.CrtdTS = DateTime.Now;
                                    oQuery.CrtdUser = UserId;
                                    oQuery.OrderNo = value;
                                    oQuery.SeqId = number;
                                    oQuery.OrderwiseSeqId = orderwiseseqId;
                                    oQuery.isUpdated = updated_f;
                                    context.tblOrder.Add(oQuery);
                                    context.SaveChanges();
                                    orderId = oQuery.OrderId;
                                    orderhis.Add(orderId);

                                    tblOrderDetail oDQuery = new tblOrderDetail();
                                    oDQuery.OrderId = orderId;
                                    oDQuery.Comments = "Back end agency entry salaryagencyId_" + agncy.SalaryAgencyId;
                                    context.tblOrderDetail.Add(oDQuery);
                                    context.SaveChanges();

                                    tblRCTSalaryCalcDetails SalaryCal = new tblRCTSalaryCalcDetails();
                                    SalaryCal.ID = item.mast.OSGID;
                                    SalaryCal.AppointType = "Outsourcing";
                                    SalaryCal.TypeCode = "OSG";
                                    SalaryCal.RecommendSalary = item.det.RecommendSalary;
                                    SalaryCal.Salutation = item.det.Salutation;
                                    SalaryCal.EmpName = item.det.EmpName;
                                    SalaryCal.EmpDesignation = item.det.EmpDesignation;
                                    SalaryCal.PhysicallyHandicapped = item.det.PhysicallyHandicapped;
                                    SalaryCal.PFBasicWages = item.det.PFBasicWages;
                                    SalaryCal.EmployeePF = item.det.EmployeePF;
                                    SalaryCal.EmployeeESIC = item.det.EmployeeESIC;
                                    SalaryCal.EmpProfessionalTax = item.det.EmpProfessionalTax;
                                    SalaryCal.EmpTotalDeduction = item.det.EmpTotalDeduction;
                                    SalaryCal.EmpNetSalary = item.det.EmpNetSalary;
                                    SalaryCal.EmployerPF = employerPF;
                                    SalaryCal.EmployerInsurance = employerIns;//
                                    SalaryCal.EmployerESIC = employerESIC;
                                    SalaryCal.EmployerCTC = employeeCTC;//
                                    SalaryCal.EmployerAgencyFee = agencyFee;//
                                    SalaryCal.EmployerGST = salaryGSTamt;//
                                    SalaryCal.EmployerCTCWithAgencyFee = ctcwithagencyfee;//
                                    SalaryCal.TotalCostPerMonth = totalCTC;//
                                    SalaryCal.CrtdTS = DateTime.Now;
                                    SalaryCal.CrtdUserId = UserId;
                                    SalaryCal.Status = "Active";
                                    SalaryCal.IsCurrentVersion = updated_f;
                                    SalaryCal.OrderId = orderId;
                                    SalaryCal.FromDate = agncy.Lastupdate_TS.Value.Date;
                                    SalaryCal.ToDate = item.mast.AppointmentEnddate;
                                    SalaryCal.StatutoryId = item.det.StatutoryId;
                                    SalaryCal.GSTPercentage = item.det.GSTPercentage;
                                    SalaryCal.AgencyFeePercentage = agencyfeeper;
                                    SalaryCal.EmployerTotalContribution = emplyrttlContr;
                                    context.tblRCTSalaryCalcDetails.Add(SalaryCal);
                                    context.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            if (orderhis.Count > 0)
                            {
                                foreach (int orderid in orderhis)
                                    EmployeeHistoryLog(orderid);
                            }
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        public static Tuple<List<OSGSalaryDetailModel>, List<OSGSalaryDetailModel>> UpdateOSGSalaryList(int componentId)
        {
            using (var context = new IOASDBEntities())
            {
                List<OSGSalaryDetailModel> newlist = new List<OSGSalaryDetailModel>();
                List<OSGSalaryDetailModel> oldlist = new List<OSGSalaryDetailModel>();
                try
                {
                    if (componentId > 0)
                    {
                        DateTime date = DateTime.Now.Date;
                        var satQuery = context.tblRCTStatutory.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                        var ptQuery = context.tblRCTProfessionalTax.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                        var agQuery = context.tblSalaryAgencyMaster.Where(x => x.SalaryAgencyId == 2).FirstOrDefault();

                        //string[] expid = new string[] { "VS10875", "VS10595" };
                        var querymast = (from sck in context.tblRCTOutsourcing
                                         join cc in context.tblRCTSalaryCalcDetails on sck.OSGID equals cc.ID
                                         from d in context.tblRCTDesignation
                                         from S in context.tblRCTStatutory
                                         where sck.DesignationId == d.DesignationId && cc.StatutoryId == S.StatutoryId &&
                                         sck.Status == "Verification Completed" && sck.IsActiveNow == true && cc.IsCurrentVersion == true && cc.Status == "Active"
                                         //&& expid.Contains(sck.EmployeersID)
                                         select new { sck, cc, d.Designation, S.LWFEmployerContribution }).ToList();
                        //bool IsvalidePeriod = false;
                        if (componentId == 14)
                        {
                            querymast = (from sck in context.tblRCTOutsourcing
                                         join cc in context.tblRCTSalaryCalcDetails on sck.OSGID equals cc.ID
                                         from d in context.tblRCTDesignation
                                         from S in context.tblRCTStatutory
                                         where sck.DesignationId == d.DesignationId && cc.StatutoryId == S.StatutoryId &&
                                         sck.Status == "Verification Completed" && sck.IsActiveNow == true && cc.IsCurrentVersion == true && cc.Status == "Active"
                                         && (sck.AppointmentStartdate <= ptQuery.ValueDate && sck.AppointmentEnddate >= ptQuery.ValueDate)
                                         && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == ptQuery.ProfessionalTaxId && x.ComponentId == componentId && x.ID == sck.OSGID))
                                         select new { sck, cc, d.Designation, S.LWFEmployerContribution }).ToList();
                        }
                        if (componentId == 12)
                        {
                            querymast = (from sck in context.tblRCTOutsourcing
                                         join cc in context.tblRCTSalaryCalcDetails on sck.OSGID equals cc.ID
                                         from d in context.tblRCTDesignation
                                         from S in context.tblRCTStatutory
                                         where sck.DesignationId == d.DesignationId && cc.StatutoryId == S.StatutoryId &&
                                         sck.Status == "Verification Completed" && sck.IsActiveNow == true && cc.IsCurrentVersion == true && cc.Status == "Active"
                                         && (sck.AppointmentStartdate <= satQuery.ValueDate && sck.AppointmentEnddate >= satQuery.ValueDate)
                                         && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == satQuery.StatutoryId && x.ComponentId == componentId && x.ID == sck.OSGID))
                                         select new { sck, cc, d.Designation, S.LWFEmployerContribution }).ToList();
                        }
                        if (componentId == 13)
                        {

                            DateTime agValuedt = agQuery.Lastupdate_TS.Value.Date;
                            querymast = (from sck in context.tblRCTOutsourcing
                                         join cc in context.tblRCTSalaryCalcDetails on sck.OSGID equals cc.ID
                                         from d in context.tblRCTDesignation
                                         from S in context.tblRCTStatutory
                                         where sck.DesignationId == d.DesignationId && cc.StatutoryId == S.StatutoryId && sck.Status == "Verification Completed"
                                         && sck.IsActiveNow == true && cc.IsCurrentVersion == true && cc.Status == "Active"
                                         && (sck.AppointmentStartdate <= agValuedt && sck.AppointmentEnddate >= agValuedt)
                                         && (!context.tblRCTSalaryCalcDetailsLog.Any(x => x.ComponentValueId == agQuery.UpdateSeqNo && x.ComponentId == componentId && x.ID == sck.OSGID))
                                         select new { sck, cc, d.Designation, S.LWFEmployerContribution }).ToList();
                        }

                        if (querymast.Count > 0)
                        {
                            for (int i = 0; i < querymast.Count(); i++)
                            {
                                decimal salary = querymast[i].sck.Salary ?? 0;
                                decimal profesTax = querymast[i].cc.EmpProfessionalTax ?? 0;

                                var LWFAmount = satQuery.LWFEmployerContribution ?? 0;
                                var isphysichandc = querymast[i].sck.PhysicallyChallenged;
                                var typeofappoint = querymast[i].sck.TypeofAppointment;
                                decimal EmployeerLWFAmount = querymast[i].LWFEmployerContribution ?? 0;
                                decimal differenceamount = 0;
                                decimal ttldeductions = querymast[i].cc.EmpTotalDeduction ?? 0;
                                decimal empnetsalary = querymast[i].cc.EmpNetSalary ?? 0;
                                //decimal employerIns = osglist[i].cc.EmployerInsurance ?? 0;
                                decimal emplyrttlContr = querymast[i].cc.EmployerTotalContribution ?? 0;
                                decimal employeeCTC = querymast[i].cc.EmployerCTC ?? 0;


                                decimal emppfbasicwages = querymast[i].cc.PFBasicWages ?? 0;
                                decimal empPF = querymast[i].cc.EmployeePF ?? 0;
                                decimal employerPF = querymast[i].cc.EmployerPF ?? 0;
                                decimal empESIC = querymast[i].cc.EmployeeESIC ?? 0;
                                decimal employerESIC = querymast[i].cc.EmployerESIC ?? 0;
                                decimal employerIns = querymast[i].cc.EmployerInsurance ?? 0;

                                if (componentId == 14)
                                {
                                    profesTax = 0;
                                    if (salary <= 3500)
                                        profesTax = ptQuery.MonthySalary35 ?? 0;
                                    else if (salary > 3500 && salary <= 5000)
                                        profesTax = ptQuery.MonthySalary35to5 ?? 0;
                                    else if (salary > 5000 && salary <= 7500)
                                        profesTax = ptQuery.MonthySalary5to75 ?? 0;
                                    else if (salary > 7500 && salary <= 10000)
                                        profesTax = ptQuery.MonthySalary75to10 ?? 0;
                                    else if (salary > 10000 && salary <= 12500)
                                        profesTax = ptQuery.MonthySalary10to12 ?? 0;
                                    else if (salary > 12500)
                                        profesTax = ptQuery.MonthySalaryAbove12 ?? 0;
                                    if (isphysichandc == "Yes")
                                        profesTax = 0;

                                    ttldeductions = empPF + empESIC + profesTax;
                                    empnetsalary = salary - ttldeductions;
                                }
                                if (componentId == 12)
                                {
                                    DateTime appointmentStart = querymast[i].sck.AppointmentStartdate ?? DateTime.Now;
                                    DateTime appointmentEnd = querymast[i].sck.AppointmentEnddate ?? DateTime.Now;

                                    decimal emppfpercent = (satQuery.PFEmployeePercentage ?? 0) / 100;
                                    decimal emplyrpfpercent = (satQuery.PFEmployerPercentage ?? 0) / 100;

                                    decimal empESICpercent = (satQuery.ESICEmployeePercentage ?? 0) / 100;
                                    decimal emplyrESICpercent = (satQuery.ESICEmployerPercentage ?? 0) / 100;

                                    decimal pfSlab = satQuery.PFEmployeeAmount ?? 0;
                                    decimal genESICSlab = satQuery.ESICEmployeegeneralamount ?? 0;
                                    decimal phESICSlab = satQuery.ESICEmployeePhysicalAmount ?? 0;

                                    if (emppfbasicwages > pfSlab && emppfbasicwages > 0)
                                        emppfbasicwages = pfSlab;
                                    empPF = Math.Round(emppfbasicwages * emppfpercent);
                                    employerPF = Math.Round(emppfbasicwages * emplyrpfpercent);
                                    empESIC = Math.Ceiling(salary * empESICpercent);
                                    employerESIC = Math.Ceiling(salary * emplyrESICpercent);
                                    if (salary <= pfSlab)
                                    {
                                        empPF = Math.Round(emppfbasicwages * emppfpercent);
                                        employerPF = Math.Round(emppfbasicwages * emplyrpfpercent);
                                        //emppfbasicwages = salary;
                                    }
                                    if (salary > genESICSlab)
                                    {
                                        empESIC = 0;
                                        employerESIC = 0;
                                    }
                                    if (typeofappoint == 2)
                                    {
                                        empPF = 0;
                                        employerPF = 0;
                                        employerIns = 0;
                                    }
                                    if (isphysichandc == "Yes")
                                    {
                                        profesTax = 0;
                                    }

                                    ttldeductions = empPF + empESIC + profesTax;
                                    empnetsalary = salary - ttldeductions;
                                    //decimal employerIns = osglist[i].cc.EmployerInsurance ?? 0;
                                    emplyrttlContr = employerPF + employerESIC + employerIns;
                                    var oldCTC = employeeCTC;
                                    employeeCTC = salary + emplyrttlContr;
                                    differenceamount = employeeCTC - oldCTC;
                                    differenceamount += (LWFAmount - EmployeerLWFAmount);

                                    if (satQuery.ValueDate.Value.Year < appointmentEnd.Year)
                                        EmployeerLWFAmount = LWFAmount;
                                    else if (satQuery.ValueDate.Value.Month == 12 || appointmentEnd.Month == 12)
                                        EmployeerLWFAmount = LWFAmount;
                                }


                                decimal agencyFee = querymast[i].cc.EmployerAgencyFee ?? 0;
                                decimal ctcwithagencyfee = querymast[i].cc.EmployerCTCWithAgencyFee ?? 0;
                                decimal gstpercent = querymast[i].cc.GSTPercentage ?? 0;
                                decimal salaryGSTamt = querymast[i].cc.EmployerGST ?? 0;
                                decimal totalCTC = querymast[i].cc.TotalCostPerMonth ?? 0;

                                if (componentId == 13) //Agency amentment
                                {
                                    decimal agencyfeeper = agQuery.Agencyfee ?? 0;
                                    decimal agencyfeeval = agencyfeeper / 100;
                                    decimal agencyIns = agQuery.Insurance ?? 0;
                                    if (typeofappoint == 2)
                                        employerIns = 0;
                                    if (employerIns > 0)
                                    {
                                        differenceamount = agencyIns - employerIns;
                                        employerIns = agencyIns;
                                    }
                                    emplyrttlContr = employerPF + employerESIC + employerIns;
                                    employeeCTC = salary + emplyrttlContr;
                                    agencyFee = Math.Round(employeeCTC * agencyfeeval);
                                    ctcwithagencyfee = employeeCTC + agencyFee;
                                    gstpercent = agQuery.GSTPercentage ?? 0;
                                    salaryGSTamt = Math.Round(ctcwithagencyfee * (gstpercent / 100));
                                    totalCTC = ctcwithagencyfee + salaryGSTamt;
                                }

                                newlist.Add(new OSGSalaryDetailModel()
                                {
                                    EmployeeNo = querymast[i].sck.EmployeersID,
                                    EmpType = querymast[i].sck.TypeofAppointment == 1 ? "Full time" : "Part time",
                                    EmpName = querymast[i].sck.Name,
                                    EmpDesig = querymast[i].Designation,
                                    PhysicalyHandicaped = querymast[i].sck.PhysicallyChallenged,
                                    EmpPFBasicWages = emppfbasicwages,
                                    EmployeePF = empPF,
                                    EmployeeESIC = empESIC,
                                    RecommendedSalary = salary,
                                    EmployeeProfessionalTax = profesTax,
                                    EmployeeTtlDeduct = ttldeductions,
                                    EmployeeNetSalary = empnetsalary,
                                    EmployerPF = employerPF,
                                    EmployerIns = employerIns,
                                    EmployerESIC = employerESIC,
                                    EmployerTtlContribute = emplyrttlContr,
                                    EmployeeCTC = employeeCTC,
                                    AgencyFee = agencyFee,
                                    SalaryGST = salaryGSTamt,
                                    CTCwithAgencyFee = ctcwithagencyfee,
                                    TotalCTC = totalCTC,
                                    LWFAmount = EmployeerLWFAmount,
                                    DifferenceAmount = differenceamount
                                });
                                oldlist.Add(new OSGSalaryDetailModel()
                                {
                                    EmployeeNo = querymast[i].sck.EmployeersID,
                                    EmpType = querymast[i].sck.TypeofAppointment == 1 ? "Full time" : "Part time",
                                    EmpName = querymast[i].sck.Name,
                                    EmpDesig = querymast[i].Designation,
                                    PhysicalyHandicaped = querymast[i].sck.PhysicallyChallenged,
                                    EmpPFBasicWages = querymast[i].cc.PFBasicWages,
                                    EmployeePF = querymast[i].cc.EmployeePF,
                                    EmployeeESIC = querymast[i].cc.EmployeeESIC,
                                    RecommendedSalary = salary,
                                    EmployeeProfessionalTax = querymast[i].cc.EmpProfessionalTax,
                                    EmployeeTtlDeduct = querymast[i].cc.EmpTotalDeduction,
                                    EmployeeNetSalary = querymast[i].cc.EmpNetSalary,
                                    EmployerPF = querymast[i].cc.EmployerPF,
                                    EmployerIns = querymast[i].cc.EmployerInsurance,
                                    EmployerESIC = querymast[i].cc.EmployerESIC,
                                    EmployerTtlContribute = querymast[i].cc.EmployerTotalContribution,
                                    EmployeeCTC = querymast[i].cc.EmployerCTC,
                                    AgencyFee = querymast[i].cc.EmployerAgencyFee,
                                    SalaryGST = querymast[i].cc.EmployerGST,
                                    CTCwithAgencyFee = querymast[i].cc.EmployerCTCWithAgencyFee,
                                    TotalCTC = querymast[i].cc.TotalCostPerMonth,
                                    LWFAmount = querymast[i].LWFEmployerContribution,
                                });
                            }
                        }
                    }
                    return Tuple.Create(newlist, oldlist);
                }
                catch (Exception ex)
                {
                    return Tuple.Create(newlist, oldlist);
                }
            }
        }

        public static DataTable ExportSalaryList(List<OSGSalaryDetailModel> list)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                dtColumns = ReportService.ToDataTable(list);
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}