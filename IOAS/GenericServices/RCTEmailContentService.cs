using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using IOAS.Controllers;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using ICSREMP.DataModel;
namespace IOAS.GenericServices
{
    public class RCTEmailContentService
    {
        ErrorHandler WriteLog = new ErrorHandler();

        private static string OfferLetter = WebConfigurationManager.AppSettings["RCTOfferLetter"];
        private static string ChecklistPath = WebConfigurationManager.AppSettings["RCTCheckListPath"];
        public static System.Globalization.CultureInfo Indian = new System.Globalization.CultureInfo("hi-IN");

        //public static string ConvertStringToTitleCase(string text)
        //{
        //    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        //    TextInfo textInfo = cultureInfo.TextInfo;
        //    if (!string.IsNullOrEmpty(text))
        //        return textInfo.ToTitleCase(text.ToLower());
        //    else
        //        return "";
        //}

        private static List<string> getDefaultCC(string Apptype)
        {
            string defaultCC = string.Empty;
            List<string> list = new List<string>();
            defaultCC = WebConfigurationManager.AppSettings["RCTSTECCMail"];
            if (Apptype == "STE")
                defaultCC = WebConfigurationManager.AppSettings["RCTSTECCMail"];
            else if (Apptype == "OSG")
                defaultCC = WebConfigurationManager.AppSettings["RCTOSGCCMail"];
            list = new List<string>(defaultCC.Split(','));
            return list;
        }

        private static string AppointmentFullName(int Apptype)
        {
            using (var cntx = new IOASDBEntities())
            {
                return cntx.tblCodeControl.FirstOrDefault(m => m.CodeID == Apptype).CodeValDetail;
            }
        }

        private static string MsOrPhD(int Appid, int Apptype)
        {
            using (var cntx = new IOASDBEntities())
            {
                var MsPhdType = 0;
                if (Apptype == 2)
                    MsPhdType = cntx.tblRCTSTE.FirstOrDefault(x => x.STEID == Appid).MsPhdType ?? 0;
                else if (Apptype == 3)
                    MsPhdType = cntx.tblRCTOutsourcing.FirstOrDefault(x => x.OSGID == Appid).MsPhdType ?? 0;
                var query = (from c in cntx.tblCodeControl
                             where c.CodeValAbbr == MsPhdType && c.CodeName == "MsPhd"
                             select c.CodeValDetail).FirstOrDefault();
                return query == null ? "" : query;
            }
        }

        private static bool isExistingEmployee(int Appid, int Apptype)
        {
            using (var cntx = new IOASDBEntities())
            {
                if (Apptype == 1)
                    return cntx.tblRCTConsultantAppointment.Any(x => x.ConsultantAppointmentId == Appid && x.EmployeeCategory == "Old Employee" && !string.IsNullOrEmpty(x.OldNumber));
                else if (Apptype == 2)
                    return cntx.tblRCTSTE.Any(x => x.STEID == Appid && x.EmployeeCategory == "Old Employee" && !string.IsNullOrEmpty(x.OldNumber));
                else if (Apptype == 3)
                    return cntx.tblRCTOutsourcing.Any(x => x.OSGID == Appid && x.EmployeeCategory == "Old Employee" && !string.IsNullOrEmpty(x.OldNumber));
                return false;
            }
        }

        private static string getOfferLetter(int Appid, string Apptype, int? OrderId = null)
        {
            using (var cntx = new IOASDBEntities())
            {
                var OfferDate = DateTime.Now;
                if (Apptype == "CON" && OrderId == null)
                    OfferDate = cntx.tblRCTConsultantAppointment.FirstOrDefault(x => x.ConsultantAppointmentId == Appid).OfferDate ?? DateTime.Now;
                else if (Apptype == "STE" && OrderId == null)
                    OfferDate = cntx.tblRCTSTE.FirstOrDefault(x => x.STEID == Appid).OfferDate ?? DateTime.Now;
                else if (Apptype == "OSG" && OrderId == null)
                    OfferDate = cntx.tblRCTOutsourcing.FirstOrDefault(x => x.OSGID == Appid).OfferDate ?? DateTime.Now;
                else
                    OfferDate = cntx.tblOrderDetail.FirstOrDefault(x => x.OrderId == OrderId).OfferDate ?? DateTime.Now;

                if (OfferDate != null)
                    return string.Format("{0:dd-MMMM-yyyy}", OfferDate);

                return "";
            }
        }

        public static List<CheckListEmailModel> getDevNormsDetails(int appid, string apptype, int? orderid = null)
        {
            List<CheckListEmailModel> list = new List<CheckListEmailModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from o in context.vw_RCTDeviationDetails
                                 where o.appid == appid && o.apptype == apptype && o.isCurrentVersion == true
                                 && o.IsChecked == true && (o.OrderId == orderid || orderid == null)
                                 select o).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var sno = i + 1;
                            list.Add(new CheckListEmailModel()
                            {
                                SNo = sno + 1,
                                checklistId = query[i].DeviationCheckListId,
                                devScenarios = query[i].CheckList,
                                actNorms = query[i].ActualNorms,
                                devinNorms = query[i].DeviationNorms,
                                CurrentVersion = query[i].isCurrentVersion ?? false,
                                OrderType = query[i].OrderType,
                                FromDate = query[i].FromDate == null ? "" : string.Format("{0:dd-MMMM-YYYY}", query[i].FromDate),
                                ToDate = query[i].ToDate == null ? "" : string.Format("{0:dd-MMMM-YYYY}", query[i].ToDate),
                            });
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public static List<CheckListEmailModel> getDevNormsList(int appid, string apptype)
        {
            List<CheckListEmailModel> list = new List<CheckListEmailModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    list = (from o in context.vw_RCTDeviationDetails
                            where o.appid == appid && o.apptype == apptype && o.isCurrentVersion == true
                            orderby o.RowNumber
                            group o by o.OrderType into grp
                            select new { grp }).AsEnumerable().Select((x, index) => new CheckListEmailModel()
                            {
                                SNo = index + 1,
                                devScenarios = x.grp == null ? "" : string.Join(", ", x.grp.Select(m => m.CheckList).ToArray()),
                                OrderType = x.grp.Select(m => m.OrderType).FirstOrDefault() == null ? "" : x.grp.Select(m => m.OrderType).FirstOrDefault(),
                                FromDate = x.grp.Select(m => m.FromDate).FirstOrDefault() == null ? "" : string.Format("{0:dd-MMMM-yyyy}", x.grp.Select(m => m.FromDate).FirstOrDefault()),
                                ToDate = x.grp.Select(m => m.ToDate).FirstOrDefault() == null ? "" : string.Format("{0:dd-MMMM-yyyy}", x.grp.Select(m => m.ToDate).FirstOrDefault()),
                            }).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public static List<CheckListEmailModel> getDevNormsDetails(CheckDevationModel model)
        {
            List<CheckListEmailModel> list = new List<CheckListEmailModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var desquery = (from des in context.tblRCTDesignation
                                    where des.DesignationId == model.DesignationId
                                    select des).FirstOrDefault();
                    var query = (from vw in context.vw_RCTOverAllApplicationEntry
                                 where vw.ApplicationId == model.AppId && vw.Category == model.AppType
                                 && vw.ApplicationType == "New"
                                 select new { vw.BasicPay }).FirstOrDefault();
                    string IITExperience = RequirementService.IITExperienceInWording(model.OldEmployee);
                    int SNo = 1;
                    string OrderType = model.OrderType;
                    if (string.IsNullOrEmpty(model.OrderType))
                        OrderType = "Appointment";
                    if (model.devChecklist.Count > 0 && desquery != null)
                    {
                        foreach (var item in model.devChecklist)
                        {
                            if (item.checklistId == 31 && OrderType == "Extension")
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per the norms, the minimum period of extension is 1 month and maximum is one year",
                                    devinNorms = OrderType + " requested is for " + (model.AppointmentEndDate.Subtract(model.AppointmentStartDate).TotalDays + 1) + " days, which is below the norms."
                                });
                            }
                            else if (item.checklistId == 31)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per the norms, the minimum period of 1 month and maximum is one year",
                                    devinNorms = OrderType + " requested is for " + (model.AppointmentEndDate.Subtract(model.AppointmentStartDate).TotalDays + 1) + " days, which is below the norms."
                                });
                            }
                            else if (item.checklistId == 34 && model.AppType == "OSG")
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per ICSR norms the maximum age limit for the post of " + desquery.Designation + " is " + Convert.ToString(desquery.AgeLimit ?? 0) + " (unreserved category) and " + Convert.ToString(desquery.SCSTAgeLimit ?? 0) + " (reserved category).",
                                    devinNorms = "The Candidate age is " + model.CheckAge + " which is above the norms."
                                });
                            }
                            else if (item.checklistId == 34 && model.AppType != "OSG")
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Maximum Age limit for " + desquery.Designation + " as per IC & SR Norms is age " + Convert.ToString(desquery.AgeLimit ?? 0),
                                    devinNorms = "The Candidate age is " + model.CheckAge + " which is above the norms."
                                });
                            }
                            else if (item.checklistId == 35 && model.AppType != "STE")
                            {
                                string devnormscourse = string.Empty;
                                if (model.QualificationId.Length > 0)
                                {
                                    IList<string> strings = new List<string>();

                                    for (int i = 0; i < model.QualificationId.Length; i++)
                                    {
                                        var courceId = model.DisciplineId[i];
                                        var marktype = model.MasrksType[i];
                                        var masrks = model.Masrks[i];
                                        devnormscourse += (from c in context.tblRCTCourseList
                                                           where c.CourseId == courceId
                                                           select c.CourseName + " qualification with").FirstOrDefault();
                                        devnormscourse += masrks + " " + (marktype == 1 ? "%" : "CGPA") + " ";
                                    }
                                }

                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Please refer the IC&SR outsourcing norms link --> https://icsr.iitm.ac.in/file/pdf/annexure55.pdf",
                                    devinNorms = "The Candidate has " + devnormscourse + ",which is below the IC&SR norms"
                                });
                            }
                            else if (item.checklistId == 35 && model.AppType == "STE")
                            {
                                string devnormscourse = string.Empty;
                                if (model.QualificationId.Length > 0)
                                {
                                    IList<string> strings = new List<string>();

                                    for (int i = 0; i < model.QualificationId.Length; i++)
                                    {
                                        var courceId = model.DisciplineId[i];
                                        var marktype = model.MasrksType[i];
                                        var masrks = model.Masrks[i];
                                        devnormscourse += (from c in context.tblRCTCourseList
                                                           where c.CourseId == courceId
                                                           select c.CourseName + " qualification with").FirstOrDefault();
                                        devnormscourse += masrks + " " + (marktype == 1 ? "%" : "CGPA") + " ";
                                    }
                                }

                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Please refer the IC&SR norms link --> https://icsr.iitm.ac.in/file/pdf/annexure3.pdf , https://icsr.iitm.ac.in/file/pdf/annexure56.pdf",
                                    devinNorms = "The Candidate has " + devnormscourse + ",which is below the IC&SR norms"
                                });
                            }
                            else if (item.checklistId == 37)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per the ICSR norms, Minimum Salary Rs." + string.Format(Indian, "{0:N0}", desquery.PayStructureMinMum) + "/- and Maximum salary Rs." + string.Format(Indian, "{0:N0}", desquery.PayStructureMaximum ?? 0) + "/-",
                                    devinNorms = "Salary recommended for amount Rs." + string.Format(Indian, "{0:N0}", model.ChekSalary) + " /- per month, which is above the norms for the said post."
                                });
                            }
                            else if (item.checklistId == 38 && model.AppType == "STE")
                            {
                                string devnormscourse = "";
                                if (model.QualificationId.Length > 0)
                                {
                                    var maxVal = model.QualificationId.Where(x => x.Value != 4).Max() ?? 4;
                                    var index = Array.FindIndex(model.QualificationId, row => row.Value == maxVal);
                                    var courceId = model.DisciplineId[index];
                                    devnormscourse += (from c in context.tblRCTCourseList
                                                       where c.CourseId == courceId
                                                       select c.CourseName).FirstOrDefault();
                                }

                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Please refer the IC&SR norms link --> https://icsr.iitm.ac.in/file/pdf/annexure3.pdf , https://icsr.iitm.ac.in/file/pdf/annexure56.pdf",
                                    devinNorms = "The relevant experience of the candidate in IIT: " + IITExperience + " and in other organizations: " + model.Experienceinwordings + " with qualification as: " + devnormscourse
                                });
                            }
                            else if (item.checklistId == 38 && model.AppType == "OSG")
                            {
                                string devnormscourse = "";
                                if (model.QualificationId.Length > 0)
                                {
                                    var maxVal = model.QualificationId.Where(x => x.Value != 4).Max() ?? 4;
                                    var index = Array.FindIndex(model.QualificationId, row => row.Value == maxVal);
                                    var courceId = model.DisciplineId[index];
                                    devnormscourse += (from c in context.tblRCTCourseList
                                                       where c.CourseId == courceId
                                                       select c.CourseName).FirstOrDefault();
                                }

                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Please refer the IC&SR outsourcing norms link --> https://icsr.iitm.ac.in/file/pdf/annexure55.pdf",
                                    devinNorms = "The relevant experience of the candidate in IIT: " + IITExperience + " and in other organizations: " + model.Experienceinwordings + " with qualification as: " + devnormscourse
                                });
                            }
                            else if (item.checklistId == 39)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "The requested manpower " + desquery.Designation + " should be available in the project",
                                    devinNorms = model.Comments
                                });
                            }
                            else if (item.checklistId == 40)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "An amount of Rs." + string.Format(Indian, "{0:N0}", model.CommitmentAmount) + "/- is required in this project to Process the request.",
                                    devinNorms = model.Comments
                                });
                            }
                            else if (item.checklistId == 41)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "For " + desquery.Designation + " post, GATE Score or NET-UGC is required as per the Fellowship norms.",
                                    devinNorms = "Candidate does not have GATE Score or NET-UGC."
                                });
                            }
                            else if (item.checklistId == 42)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per IC & SR norms, maximum increments is upto 8 increments i.e Rs." + (desquery.AnnualIncrement * 8) + "/-",
                                    devinNorms = "The requested enhancement value for the staff is Rs." + string.Format(Indian, "{0:N0}", (model.ChekSalary - query.BasicPay)) + "/-, which is above the norms."
                                });
                            }
                            else if (item.checklistId == 43)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "Trainee period exceeds beyond 6 month condition",
                                    devinNorms = "Candidate period exceeds 6 month"
                                });
                            }
                            else if (item.checklistId == 32 || item.checklistId == 33)
                            {
                                list.Add(new CheckListEmailModel()
                                {
                                    SNo = SNo,
                                    CheckList = item.CheckList,
                                    devScenarios = item.CheckList,
                                    checklistId = item.checklistId ?? 0,
                                    actNorms = "As per the IC&SR norms, Staff service in IITM should not exceed more than five years without one year break.",
                                    devinNorms = model.Comments
                                });
                            }
                            SNo++;
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public static string SendPIReminderMail(int appid, string category, int? orderid = null, bool getbody_f = false)
        {
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var querymast = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                     where S.ApplicationId == appid && S.Category == category
                                     && (S.OrderId == orderid || orderid == null)
                                     select new
                                     {
                                         S.ToMail,
                                         S.bcc,
                                         S.Category,
                                         S.ApplicationId,
                                         S.OrderId,
                                         Name = S.ProfessionalType + " " + S.CandidateName
                                     }).FirstOrDefault();
                    if (querymast != null)
                    {
                        EmailModel emodel = new EmailModel();
                        List<string> ccMail = new List<string>();
                        string body = string.Empty;
                        int emailcount = 0;
                        if (!string.IsNullOrEmpty(querymast.bcc))
                            ccMail = new List<string>(Regex.Split((querymast.bcc), ","));
                        getDefaultCC(category).ForEach(mailid => { ccMail.Add(mailid); });
                        if (category == "STE")
                        {
                            emailcount = (from el in IOAScontext.tblRCTSTEEmailLog
                                          where el.TypeofMail == 3 && el.IsSend == true && el.STEID == appid
                                           && (el.OrderId != null && el.OrderId == orderid || orderid == null)
                                          select el).Count() + 1;

                            var querymail = (from q in IOAScontext.tblRCTSTEEmailLog
                                             where q.STEID == appid && (q.OrderId != null && q.OrderId == orderid || orderid == null)
                                             && q.TypeofMail == 2
                                             orderby q.EmailId descending
                                             select q).FirstOrDefault();
                            if (querymail != null)
                            {
                                body = querymail.Body;
                            }
                        }
                        else if (category == "CON")
                        {
                            emailcount = (from el in IOAScontext.tblRCTConsutantAppEmailLog
                                          where el.TypeofMail == 3 && el.IsSend == true && el.ConsultantAppointmentId == appid
                                          && (el.OrderId != null && el.OrderId == orderid || orderid == null)
                                          select el).Count() + 1;
                            var querymail = (from q in IOAScontext.tblRCTConsutantAppEmailLog
                                             where q.ConsultantAppointmentId == appid && (q.OrderId != null && q.OrderId == orderid || orderid == null)
                                             && q.TypeofMail == 2
                                             orderby q.ConAPPEmailId descending
                                             select q).FirstOrDefault();
                            if (querymail != null)
                            {
                                body = querymail.Body;
                            }
                        }
                        else
                        {
                            emailcount = (from el in IOAScontext.tblRCTOSGEmailLog
                                          where el.TypeofMail == 3 && el.IsSend == true && el.OSGID == appid
                                          && (el.OrderId != null && el.OrderId == orderid || orderid == null)
                                          select el).Count() + 1;
                            var querymail = (from q in IOAScontext.tblRCTOSGEmailLog
                                             where q.OSGID == appid && (q.OrderId != null && q.OrderId != null && q.OrderId == orderid || orderid == null)
                                             && q.TypeofMail == 2
                                             orderby q.EmailId descending
                                             select q).FirstOrDefault();
                            if (querymail != null)
                            {
                                body = querymail.Body;
                            }
                        }

                        if (getbody_f)
                            return body;

                        emodel.subject = "Application status for " + querymast.Name;
                        if (emailcount == 3)
                            emodel.subject += " - Final Reminder";
                        else if (emailcount == 3)
                            return "false";
                        else
                            emodel.subject += " - Reminder " + emailcount;

                        emodel.cc = ccMail;
                        emodel.toMail = querymast.ToMail;
                        EmailBuilder _eb = new EmailBuilder();
                        var isSend = _eb.RCTSendEmail(emodel, body);
                        if (querymast.Category == "STE")
                        {
                            tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = body;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            var userName = System.Web.HttpContext.Current.User.Identity.Name;
                            var Userid = Common.GetUserid(userName);
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = Userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 3;
                            EmailStatus.STEID = querymast.ApplicationId;
                            EmailStatus.OrderId = querymast.OrderId;
                            IOAScontext.tblRCTSTEEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return "true";
                        }
                        else if (querymast.Category == "CON")
                        {
                            tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = body;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            var userName = System.Web.HttpContext.Current.User.Identity.Name;
                            var Userid = Common.GetUserid(userName);
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = Userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 3;
                            EmailStatus.ConsultantAppointmentId = querymast.ApplicationId;
                            EmailStatus.OrderId = querymast.OrderId;
                            IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return "true";
                        }
                        else
                        {
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = body;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            var userName = System.Web.HttpContext.Current.User.Identity.Name;
                            var Userid = Common.GetUserid(userName);
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = Userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 3;
                            EmailStatus.OSGID = querymast.ApplicationId ?? 0;
                            EmailStatus.OrderId = querymast.OrderId;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return "true";
                        }
                    }
                }
                return "false";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        #region OSG Mails

        public static int SendMailForOSG(int OSGID, int UserID, string sendsalrystruct, bool isdeviation, string Commments = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var query = (from A in IOAScontext.tblRCTOutsourcing
                                 from C in IOAScontext.tblCodeControl
                                 from d in IOAScontext.tblRCTDesignation
                                 where C.CodeValAbbr == A.ProfessionalType && C.CodeName == "RCTProfessional"
                                 && A.DesignationId == d.DesignationId && A.OSGID == OSGID
                                 select new
                                 {
                                     A.EmployeeCategory,
                                     A.ProjectId,
                                     A.TypeofAppointment,
                                     A.ApplicationReceiveDate,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     A.OldNumber,
                                     A.ToMail,
                                     A.ApplicationNumber,
                                     A.Salary,
                                     A.bcc,
                                     C.CodeValDetail,
                                     A.Name,
                                     d.Designation
                                 }).FirstOrDefault();
                    var salrycal = (from SM in IOAScontext.tblRCTSalaryCalcDetails where SM.ID == OSGID && SM.IsCurrentVersion == true select SM).FirstOrDefault();
                    if (query != null && salrycal != null)
                    {
                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<EmailAttachmentModel> AttachmentList = new List<EmailAttachmentModel>();
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addcc.Add(mailid); });
                        string paystructure = " status of ";
                        if (sendsalrystruct == "SST")
                            paystructure = "_Pay structure approval for ";
                        ackmodel.PersonName = query.CodeValDetail + "" + query.Name.ToUpper();
                        if (query.EmployeeCategory == "Old Employee" && !string.IsNullOrEmpty(query.OldNumber) && isdeviation == true)
                            ackmodel.subject = "Deviation(s) from the norms in the Re-appointment of " + ackmodel.PersonName + " - Outsourcing";
                        else if (isdeviation == true)
                            ackmodel.subject = "Deviation(s) from the norms in the application of " + ackmodel.PersonName + " - Outsourcing";
                        else if (query.EmployeeCategory == "Old Employee" && !string.IsNullOrEmpty(query.OldNumber) && isdeviation == false)
                            ackmodel.subject = "Re-appointment" + paystructure + "" + ackmodel.PersonName + " - Outsourcing";
                        else if (isdeviation == false)
                            ackmodel.subject = "Application" + paystructure + "" + ackmodel.PersonName + " - Outsourcing";
                        ackmodel.toMail = query.ToMail;
                        ackmodel.cc = addcc;
                        ackmodel.ApplicationNumber = query.ApplicationNumber;
                        ackmodel.DesignationName = query.Designation;
                        ackmodel.ApplicationReceiveDate = String.Format("{0:dd-MMMM-yyyy}", query.ApplicationReceiveDate);
                        ackmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.AppointmentType = Common.GetCodeControlName(query.TypeofAppointment ?? 0, "OSGAppointmenttype");
                        ackmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0);
                        ackmodel.BasicPay = string.Format(Indian, "{0:N0}", query.Salary ?? 0);
                        ackmodel.IsDeviation = isdeviation;
                        ackmodel.SendSlryStruct = sendsalrystruct;
                        ackmodel.checkdetails = RCTEmailContentService.getDevNormsDetails(OSGID, "OSG");
                        ackmodel.DAName = Common.GetUserFirstName(UserID);
                        ackmodel.Comments = Commments;
                        emodel = ackmodel;
                        var bodyResp = _eb.RunCompile("RCTOSGApplicationack.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var Attachments = "";
                            RCTReportMasterController _reportC = new RCTReportMasterController();
                            string SalaryStructure = _reportC.GenerateSalaryStructure(OSGID);
                            Attachments = SalaryStructure;
                            AttachmentList.Add(new EmailAttachmentModel
                            {
                                actualName = Attachments,
                                displayName = ackmodel.PersonName + "_Paystructure.pdf"
                            });
                            Uri uri = new Uri(SalaryStructure);
                            string actName = uri.Segments.Last();
                            salrycal.SalaryStructureDocPath = actName;
                            IOAScontext.SaveChanges();
                            if (sendsalrystruct == "SST")
                            {
                                emodel.attachmentlist = AttachmentList;
                            }
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            //string actName = SalaryStructure.Split('/').Last();

                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            if (emodel.attachmentlist != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachmentlist.Select(x => x.actualName).ToList());
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = UserID;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 1;
                            EmailStatus.OSGID = OSGID;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static Tuple<int, string> SendOSGOfferReleaseMail(int OSGID, int UserID, bool isBody_f = false, int? OrderId = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == OSGID && x.Category == "OSG" && x.isSend == true && x.OfferCategory == "OfferLetter" && (x.OrderId == OrderId || (OrderId == null && x.OrderId == null))))
                        return Tuple.Create(0, "Offer release mail already send");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == OSGID
                                 && S.Category == "OSG" && (S.OrderId == OrderId || OrderId == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     Name = S.ProfessionalType + "" + S.CandidateName,
                                     S.ProfessionalType,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     P.ProjectNumber,
                                     P.ProjectTitle,
                                     S.crtdUserId,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.ApplicationReceiveDate,
                                     S.TypeofAppointment,
                                     S.TypeofAppointmentinInt,
                                     S.ApplicationType,
                                     S.EmployeersID,
                                     S.Email,
                                     S.Category,
                                     S.isMsPhd,
                                     S.PIName,
                                     vw.DepartmentName,
                                     S.BasicPay,
                                     S.ContactNumber,
                                     S.PhysicallyChallenged
                                 }).FirstOrDefault();
                    decimal? OldSalary = 0;
                    var salrycal = (from SM in IOAScontext.tblRCTSalaryCalcDetails
                                    where SM.ID == OSGID && (SM.OrderId == OrderId || OrderId == null)
                                    select SM).FirstOrDefault();
                    if (query != null && salrycal != null)
                    {


                        var queryvendor = (from A in IOAScontext.tblRCTOutsourcing
                                           where A.OSGID == OSGID
                                           select new
                                           {
                                               A.VendorId,
                                               A.AppointmentStartdate,
                                               A.AppointmentEnddate,
                                               A.Salary,
                                               A.ResumeFile,
                                           }).FirstOrDefault();
                        var venQuery = IOAScontext.tblSalaryAgencyMaster.Where(m => m.SalaryAgencyId == queryvendor.VendorId).Select(c => new { c.ContactEmail, c.CCMail }).FirstOrDefault();

                        OldSalary = queryvendor.Salary;
                        var queryLog = (from A in IOAScontext.tblRCTOSGLog
                                        where A.Orderid == OrderId
                                        select A.Salary).FirstOrDefault();
                        if (queryLog != null)
                            OldSalary = queryLog;

                        var queryodr = (from A in IOAScontext.tblOrder
                                        where A.OrderId == OrderId
                                        select A).FirstOrDefault();
                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        string toMail = null;
                        if (venQuery != null)
                        {
                            toMail = venQuery.ContactEmail;
                            if (venQuery.CCMail != null)
                            {
                                var bcc = venQuery.CCMail.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addcc.Add(mailid); });
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.PersonName = query.Name;
                        if (query.ApplicationType == "New")
                        {
                            if (IOAScontext.tblRCTOutsourcing.Any(x => x.EmployeersID == query.EmployeersID && x.Status == "Relieved"))
                            {
                                ackmodel.FillFields = "Re - appointment";
                                ackmodel.subject = "Re - appointment of " + ackmodel.PersonName + " – Outsourcing.";
                            }
                            else
                            {
                                ackmodel.FillFields = "New appointment";
                                ackmodel.subject = "Application of " + ackmodel.PersonName + " – Outsourcing.";
                            }
                        }
                        else if (query.ApplicationType == "Extension")
                        {
                            ackmodel.subject = "Revision of pay for " + ackmodel.PersonName + " – Outsourcing.";
                        }
                        else if (query.ApplicationType == "Enhancement")
                        {
                            ackmodel.subject = "Enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                            ackmodel.FillFields = "enhancement";
                            if (queryodr.Basic < OldSalary)
                            {
                                ackmodel.subject = "Revision of pay for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "revision of pay";
                            }
                            else if (queryodr.OldProjectId != queryodr.NewProjectId && queryodr.OldDesignation == queryodr.NewDesignation && queryodr.Basic == OldSalary)
                            {
                                ackmodel.subject = "Extension for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "extension";
                            }
                            else if (queryodr.OldProjectId != queryodr.NewProjectId && queryodr.OldDesignation == queryodr.NewDesignation && queryodr.Basic > OldSalary)
                            {
                                ackmodel.subject = "Extension with enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "extension with enhancement";
                            }
                            else if (queryodr.OldProjectId != queryodr.NewProjectId && queryodr.OldDesignation != queryodr.NewDesignation && queryodr.Basic > OldSalary)
                            {
                                ackmodel.subject = "Extension cum enhancement with change of designation for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "extension cum enhancement with change of designation";
                            }
                            else if (queryodr.OldProjectId != queryodr.NewProjectId && queryodr.OldDesignation != queryodr.NewDesignation && queryodr.Basic == OldSalary)
                            {
                                ackmodel.subject = "Extension with change of designation for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "extension cum enhancement with change of designation";
                            }
                            else if (queryodr.OldDesignation != queryodr.NewDesignation && queryodr.OldProjectId == queryodr.NewProjectId && queryodr.isExtended == false && queryodr.Basic == OldSalary)
                            {
                                ackmodel.subject = "Change in designation for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "change of designation";
                            }
                            else if (queryodr.OldDesignation != queryodr.NewDesignation && queryodr.OldProjectId == queryodr.NewProjectId && queryodr.isExtended == false && queryodr.Basic > OldSalary)
                            {
                                ackmodel.subject = "Change in designation with enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "change of designation with enhancement";
                            }
                            else if (queryodr.OldDesignation != queryodr.NewDesignation && queryodr.OldProjectId == queryodr.NewProjectId && queryodr.isExtended == true && queryodr.Basic > OldSalary)
                            {
                                ackmodel.subject = "Extension cum enhancement with change of designation for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "extension cum enhancement with change of designation";
                            }

                            else if (queryodr.OldDesignation != queryodr.NewDesignation && queryodr.OldProjectId == queryodr.NewProjectId && queryodr.isExtended == true && queryodr.Basic == OldSalary)
                            {
                                ackmodel.subject = "Change in designation with extension for " + ackmodel.PersonName + " – Outsourcing.";
                                ackmodel.FillFields = "change of designation with extension";
                            }
                        }
                        else if (query.ApplicationType == "Change of project")
                        {
                            ackmodel.subject = "Change of project for " + ackmodel.PersonName + " – Outsourcing.";
                        }

                        ackmodel.toMail = toMail;
                        ackmodel.cc = addcc;
                        ackmodel.ApplicationNumber = query.ApplicationNo;
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.ApplicationReceiveDate = String.Format("{0:dd-MMMM-yyyy}", query.ApplicationReceiveDate);
                        ackmodel.AppointmentType = Common.GetCodeControlName(query.TypeofAppointmentinInt ?? 0, "OSGAppointmenttype");
                        ackmodel.ProjectNumber = query.ProjectNumber;
                        ackmodel.BasicPay = string.Format(Indian, "{0:N0}", query.BasicPay ?? 0);
                        ackmodel.PFBasicwages = string.Format(Indian, "{0:N0}", salrycal.PFBasicWages ?? 0);
                        ackmodel.GrossPay = string.Format(Indian, "{0:N0}", salrycal.RecommendSalary ?? 0);
                        ackmodel.Ordertype = query.ApplicationType;
                        ackmodel.EmployeeNum = string.IsNullOrEmpty(query.EmployeersID) ? "Yet to create" : query.EmployeersID;
                        ackmodel.MobileNumber = query.ContactNumber;
                        ackmodel.PhysicallyChallenged = query.PhysicallyChallenged;
                        ackmodel.Email = query.Email;
                        if (salrycal.PFBasicWages > 0)
                            ackmodel.PFEligiblity = "Yes";
                        else
                            ackmodel.PFEligiblity = "No";

                        if (salrycal.EmployeeESIC > 0)
                            ackmodel.ESICEligiblity = "Yes";
                        else
                            ackmodel.ESICEligiblity = "No";

                        ackmodel.DAName = Common.GetUserFirstName(UserID);
                        emodel = ackmodel;
                        var bodyResp = _eb.RunCompile("OSGOfferReleaseTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (isBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);

                            List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                            if (query.ApplicationType == "New" && !string.IsNullOrEmpty(queryvendor.ResumeFile))
                            {
                                var strem = queryvendor.ResumeFile.DownloadFile("Requirement");
                                AttachmentList.Add(new ByteEmailAttachmentModel
                                {
                                    dataByte = strem,
                                    displayName = "Resume"
                                });
                                emodel.attachmentByte = AttachmentList;
                            }


                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            var queryoffer = IOAScontext.tblRCTOfferDetails.Where(m => m.ApplicationId == OSGID && m.Category == "OSG" && m.OfferCategory == "OfferLetter" && (m.OrderId == OrderId || (OrderId == null && m.OrderId == null)) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = UserID;
                                queryoffer.UPTD_TS = DateTime.Now;
                                queryoffer.isSend = isSend;
                            }
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            EmailStatus.Attachment = queryvendor.ResumeFile;
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = UserID;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 16;
                            EmailStatus.OSGID = OSGID;
                            EmailStatus.OrderId = OrderId;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return Tuple.Create(1, bodyResp.Item2);
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        public static Tuple<int, string> SendOSGAppointmentorderMail(int OSGID, int UserID, string OfferCategory, bool isBody_f = false, int? OrderId = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    if (IOAScontext.tblRCTOfferDetails.Any(m => m.ApplicationId == OSGID && m.Category == "OSG" && m.OfferCategory == OfferCategory && (m.OrderId == OrderId || (OrderId == null && m.OrderId == null)) && m.isSend == true))
                        return Tuple.Create(0, "Office Order Already sent");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == OSGID
                                 && S.Category == "OSG" && (S.OrderId == OrderId || OrderId == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     S.CandidateName,
                                     S.ProfessionalType,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     P.ProjectNumber,
                                     P.ProjectTitle,
                                     S.crtdUserId,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.ApplicationReceiveDate,
                                     S.TypeofAppointment,
                                     S.TypeofAppointmentinInt,
                                     S.ApplicationType,
                                     S.EmployeersID,
                                     S.Email,
                                     S.Category,
                                     S.isMsPhd,
                                     S.PIName,
                                     vw.DepartmentName,
                                     S.BasicPay,
                                     S.ContactNumber,
                                     S.PhysicallyChallenged
                                 }).FirstOrDefault();


                    if (query != null)
                    {
                        var salrycal = (from SM in IOAScontext.tblRCTSalaryCalcDetails
                                        where SM.ID == OSGID && (SM.OrderId == OrderId || OrderId == null)
                                        select SM).FirstOrDefault();
                        decimal OldSalary = 0;

                        var queryVendor = (from A in IOAScontext.tblRCTOutsourcing
                                           where A.OSGID == OSGID
                                           select A).FirstOrDefault();
                        var venQuery = IOAScontext.tblSalaryAgencyMaster.Where(m => m.SalaryAgencyId == queryVendor.VendorId).Select(c => new { c.ContactEmail, c.CCMail }).FirstOrDefault();
                        OldSalary = queryVendor.Salary ?? 0;

                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<CheckListEmailModel> chk = new List<CheckListEmailModel>();

                        List<string> addcc = new List<string>();
                        List<string> AttachmentList = new List<string>();

                        string toMail = null;
                        if (venQuery != null)
                        {
                            toMail = venQuery.ContactEmail;
                            if (venQuery.CCMail != null)
                            {
                                var bcc = venQuery.CCMail.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addcc.Add(mailid); });
                        var OrderdetQry = (from A in IOAScontext.tblOrderDetail
                                           from O in IOAScontext.tblOrder
                                           where A.OrderId == O.OrderId && A.OrderId == OrderId
                                           select new { A, O }).FirstOrDefault();
                        if (OrderdetQry != null)
                        {
                            if (OrderdetQry.O.Status == "Completed" && OrderdetQry.O.isUpdated == true)
                            {
                                var queryLog = (from A in IOAScontext.tblRCTOSGLog
                                                where A.Orderid == OrderId
                                                orderby A.OSGLogID descending
                                                select A).FirstOrDefault();
                                if (queryLog != null)
                                    OldSalary = queryLog.Salary ?? 0;
                            }

                        }
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.PersonName = query.ProfessionalType + "" + query.CandidateName.ToUpper();
                        if (query.ApplicationType == "New" && IOAScontext.tblRCTOutsourcing.Any(x => x.EmployeersID == query.EmployeersID && x.Status == "Relieved"))
                        {
                            ackmodel.subject = "Re - appointment of " + query.CandidateName.ToUpper() + " – Outsourcing.";
                        }
                        else if (query.ApplicationType == "New")
                        {
                            ackmodel.subject = "Application of " + query.CandidateName.ToUpper() + " – Outsourcing.";
                        }
                        else if (query.ApplicationType == "Extension" && OldSalary > query.BasicPay)
                        {
                            ackmodel.subject = "Revision of pay for " + query.CandidateName.ToUpper() + " – Outsourcing.";
                            ackmodel.FillFields = "appointment";
                        }
                        else if (query.ApplicationType == "Extension" && OldSalary < query.BasicPay)
                        {
                            ackmodel.subject = "Extension and enhancement for " + query.CandidateName.ToUpper() + " – Outsourcing.";
                            ackmodel.FillFields = "extension cum enhancement";
                        }
                        else if (query.ApplicationType == "Extension" && OldSalary == query.BasicPay)
                        {
                            ackmodel.subject = "Extension for " + query.CandidateName.ToUpper() + " – Outsourcing.";
                            ackmodel.FillFields = "term extension";
                        }

                        else if (query.ApplicationType == "Enhancement")
                        {
                            if (OrderdetQry.O.isExtended == true)
                                ackmodel.Extended_f = true;

                            ackmodel.subject = "Enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                            ackmodel.FillFields = "enhancement";
                            //Revision of pay
                            if (OrderdetQry.O.Basic < OldSalary)
                                ackmodel.subject = "Revision of pay for " + ackmodel.PersonName + " – Outsourcing.";
                            //Project Change only
                            else if (OrderdetQry.O.OldProjectId != OrderdetQry.O.NewProjectId && OrderdetQry.O.OldDesignation == OrderdetQry.O.NewDesignation && OrderdetQry.O.Basic == OldSalary)
                                ackmodel.subject = "Extension for " + ackmodel.PersonName + " – Outsourcing.";
                            //Project Change and salary increment
                            else if (OrderdetQry.O.OldProjectId != OrderdetQry.O.NewProjectId && OrderdetQry.O.OldDesignation == OrderdetQry.O.NewDesignation && OrderdetQry.O.Basic > OldSalary)
                                ackmodel.subject = "Extension with enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                            //Project Change ,designation change and salary increment
                            else if (OrderdetQry.O.OldProjectId != OrderdetQry.O.NewProjectId && OrderdetQry.O.OldDesignation != OrderdetQry.O.NewDesignation && OrderdetQry.O.Basic > OldSalary)
                                ackmodel.subject = "Extension cum enhancement with change in designation for " + ackmodel.PersonName + " – Outsourcing.";
                            //Designation change only
                            else if (OrderdetQry.O.OldDesignation != OrderdetQry.O.NewDesignation && OrderdetQry.O.OldProjectId == OrderdetQry.O.NewProjectId && OrderdetQry.O.isExtended == false && OrderdetQry.O.Basic == OldSalary)
                                ackmodel.subject = "Change in designation for " + ackmodel.PersonName + " – Outsourcing.";
                            //Designation change and salary increment
                            else if (OrderdetQry.O.OldDesignation != OrderdetQry.O.NewDesignation && OrderdetQry.O.OldProjectId == OrderdetQry.O.NewProjectId && OrderdetQry.O.isExtended == false && OrderdetQry.O.Basic > OldSalary)
                                ackmodel.subject = "Change in designation with enhancement for " + ackmodel.PersonName + " – Outsourcing.";
                            //Designation change with extension and salary increment
                            else if (OrderdetQry.O.OldDesignation != OrderdetQry.O.NewDesignation && OrderdetQry.O.OldProjectId == OrderdetQry.O.NewProjectId && OrderdetQry.O.isExtended == true && OrderdetQry.O.Basic > OldSalary)
                                ackmodel.subject = "Extension cum enhancement with change in designation for " + ackmodel.PersonName + " – Outsourcing.";
                            //Designation change with extension
                            else if (OrderdetQry.O.OldDesignation != OrderdetQry.O.NewDesignation && OrderdetQry.O.OldProjectId == OrderdetQry.O.NewProjectId && OrderdetQry.O.isExtended == true && OrderdetQry.O.Basic == OldSalary)
                                ackmodel.subject = "Change in designation with extension for " + ackmodel.PersonName + " – Outsourcing.";
                        }
                        else if (query.ApplicationType == "Change of project")
                        {
                            ackmodel.subject = "Change of project for " + query.CandidateName.ToUpper() + " – Outsourcing.";
                        }
                        else if (query.ApplicationType == "Amendment")
                        {
                            if (IOAScontext.tblOrder.Any(x => x.AppointmentId == OSGID && x.AppointmentType == 3 && x.Status == "Completed" && x.isUpdated == true))
                            {
                                int?[] exptype = new int?[] { 1, 2, 3, 4 };
                                var queryorder = (from o in IOAScontext.tblOrder
                                                  where o.AppointmentId == OSGID && o.AppointmentType == 3
                                                  && o.Status == "Completed" && o.isUpdated == true && exptype.Contains(o.OrderType)
                                                  orderby o.OrderId descending
                                                  select o).FirstOrDefault();

                                if (queryorder != null)
                                {
                                    if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfficeOrder" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                    {
                                        ackmodel.FillFields = "appointment letter";
                                    }
                                    else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "Order" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                    {
                                        ackmodel.FillFields = "appointment letter";
                                    }
                                    else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfferLetter" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                    {
                                        ackmodel.FillFields = "offer letter";
                                    }
                                }
                                else
                                {
                                    ackmodel.FillFields = "offer letter";
                                }
                            }
                            else
                            {
                                ackmodel.FillFields = "offer letter";
                            }
                            ackmodel.subject = "Revised " + ackmodel.FillFields + " for " + query.CandidateName.ToUpper() + " – Outsourcing.";

                            ackmodel.PrevFromDate = String.Format("{0:dd-MMMM-yyyy}", queryVendor.ActualAppointmentStartDate);
                            ackmodel.PrevToDate = String.Format("{0:dd-MMMM-yyyy}", queryVendor.ActualAppointmentEndDate);
                        }
                        else if (query.ApplicationType == "Relieving")
                        {
                            var xs = IOAScontext.tblRCTOSGEmailLog.Where(x => x.OrderId == OrderId && x.Subject.Contains("Initiate Relieving process for") && x.TypeofMail == 12).ToList();
                            if (OrderdetQry.O.Status == "Open" && (IOAScontext.tblRCTOSGEmailLog.Where(x => x.OrderId == OrderId && x.Subject.Contains("Initiate Relieving process for") && x.TypeofMail == 12).ToList().Count > 0))
                                return Tuple.Create(0, "Relieve order already sent");
                            ackmodel.RelievingDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                            ackmodel.IsNoduesubmited = OrderdetQry.A.NOCDocSubmitted;
                            if (OrderdetQry.O.Status == "Open")
                                ackmodel.subject = "Initiate Relieving process for " + query.CandidateName.ToUpper() + " from Outsourcing.";
                            else if (OrderdetQry.O.Status == "Completed" && OrderdetQry.A.RelievingMode != 3 && (IOAScontext.tblRCTOSGEmailLog.Where(x => x.OrderId == OrderId && x.Subject.Contains("Initiate Relieving process for") && x.TypeofMail == 12).ToList().Count == 0))
                                ackmodel.subject = "Relieving and release of service certificate " + query.CandidateName.ToUpper() + " - Outsourcing.";
                            else
                                ackmodel.subject = "Release of service certificate for " + ackmodel.PersonName;
                        }
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.toMail = toMail;
                        ackmodel.cc = addcc;
                        ackmodel.ApplicationNumber = query.ApplicationNo;
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.ProjectNumber = query.ProjectNumber;
                        ackmodel.Ordertype = query.ApplicationType;
                        ackmodel.EmployeeNum = query.EmployeersID;
                        ackmodel.DAName = Common.GetUserFirstName(UserID);

                        ackmodel.AppointmentType = query.TypeofAppointment;
                        ackmodel.EmployeeNum = queryVendor.EmployeersID;
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.Email = queryVendor.Email;
                        ackmodel.MobileNumber = queryVendor.ContactNumber;
                        ackmodel.PhysicallyChallenged = queryVendor.PhysicallyChallenged;
                        if (salrycal != null)
                        {
                            ackmodel.BasicPay = string.Format(Indian, "{0:N0}", salrycal.RecommendSalary ?? 0);
                            ackmodel.PFBasicwages = string.Format(Indian, "{0:N0}", salrycal.PFBasicWages ?? 0);
                            if (salrycal != null)
                            {
                                if (salrycal.PFBasicWages > 0)
                                    ackmodel.PFEligiblity = "Yes";
                                else
                                    ackmodel.PFEligiblity = "No";
                                if (salrycal.EmployeeESIC > 0)
                                    ackmodel.ESICEligiblity = "Yes";
                                else
                                    ackmodel.ESICEligiblity = "No";
                            }
                        }
                        emodel = ackmodel;
                        var bodyResp = _eb.RunCompile("OSGAppointOrderTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (isBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            var queryoffer = IOAScontext.tblRCTOfferDetails.Where(m => m.ApplicationId == OSGID && m.Category == "OSG" && m.OfferCategory == OfferCategory && (m.OrderId == OrderId || (OrderId == null && m.OrderId == null)) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = UserID;
                                queryoffer.UPTD_TS = DateTime.Now;
                                if (OrderdetQry != null && OrderdetQry.O.Status == "Completed" && OrderdetQry.O.OrderType == 9)
                                    queryoffer.isSend = isSend;
                                else
                                    queryoffer.isSend = isSend;
                            }
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = UserID;
                            EmailStatus.IsSend = isSend;
                            if (query.ApplicationType != "Relieving")
                            {
                                EmailStatus.TypeofMail = 16;
                            }
                            else
                            {
                                EmailStatus.TypeofMail = 12;
                            }
                            EmailStatus.OSGID = OSGID;
                            EmailStatus.OrderId = OrderId;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            if (query.ApplicationType == "Amendment")
                                RCTEmailContentService.SendOSGAmendmentMail(OrderId ?? 0, UserID);
                            return Tuple.Create(1, bodyResp.Item2);
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        public static Tuple<int, string> SendOSGAmendmentMail(int OrderId, int UserID)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 where S.ProjectId == P.ProjectId && S.OrderId == OrderId
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     S.CandidateName,
                                     S.ProfessionalType,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     P.ProjectNumber,
                                     P.ProjectTitle,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.TypeofAppointment,
                                 }).FirstOrDefault();

                    if (query != null)
                    {

                        List<string> addcc = new List<string>();
                        var queryVendor = (from A in IOAScontext.tblRCTOutsourcing
                                           where A.OSGID == query.ApplicationId
                                           select A).FirstOrDefault();
                        var venQuery = (from SM in IOAScontext.tblSalaryAgencyMaster where SM.SalaryAgencyId == queryVendor.VendorId select SM).FirstOrDefault();
                        string toMail = null;
                        if (venQuery != null)
                        {
                            toMail = venQuery.ContactEmail;
                            if (venQuery.CCMail != null)
                            {
                                var bcc = venQuery.CCMail.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addcc.Add(mailid); });
                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.PersonName = query.ProfessionalType + "" + query.CandidateName.ToUpper();
                        if (IOAScontext.tblOrder.Any(x => x.AppointmentId == query.ApplicationId && x.AppointmentType == 3 && x.Status == "Completed" && x.isUpdated == true))
                        {
                            int?[] exptype = new int?[] { 1, 2, 3, 4 };
                            var queryorder = (from o in IOAScontext.tblOrder
                                              where o.AppointmentId == query.ApplicationId && o.AppointmentType == 3
                                              && o.Status == "Completed" && o.isUpdated == true && exptype.Contains(o.OrderType)
                                              orderby o.OrderId descending
                                              select o).FirstOrDefault();

                            if (queryorder != null)
                            {
                                if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfficeOrder" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                {
                                    ackmodel.FillFields = "appointment letter";
                                }
                                else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "Order" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                {
                                    ackmodel.FillFields = "appointment letter";
                                }
                                else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfferLetter" && x.isSend == true && x.OrderId == queryorder.OrderId))
                                {
                                    ackmodel.FillFields = "offer letter";
                                }
                            }
                            else
                            {
                                ackmodel.FillFields = "offer letter";
                            }
                        }
                        else
                        {
                            ackmodel.FillFields = "offer letter";
                        }
                        ackmodel.subject = "Revised " + ackmodel.FillFields + " for " + query.CandidateName.ToUpper() + " – Outsourcing.";
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        ackmodel.toMail = toMail;
                        ackmodel.cc = addcc;
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.ProjectNumber = query.ProjectNumber;
                        ackmodel.DAName = Common.GetUserFirstName(UserID);
                        ackmodel.AppointmentType = query.TypeofAppointment;
                        emodel = ackmodel;
                        var bodyResp = _eb.RunCompile("OSGAppointOrderTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = UserID;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 6;
                            EmailStatus.OSGID = query.ApplicationId;
                            EmailStatus.OrderId = OrderId;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return Tuple.Create(1, bodyResp.Item2);
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        public static int AcknowleadgementMailForOSGOrders(int orderid, int userid, string sendsalrystruct = null)
        {
            int res = 0;
            int AppID = 0, ProjectID = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    List<string> addBcc = new List<string>();
                    List<EmailAttachmentModel> AttachmentList = new List<EmailAttachmentModel>();
                    var query = (from A in IOAScontext.tblRCTOutsourcing
                                 from C in IOAScontext.tblCodeControl
                                 from O in IOAScontext.tblOrder
                                 from M in IOAScontext.tblOrderMaster
                                 from D in IOAScontext.tblRCTDesignation
                                 where O.NewDesignation == D.DesignationId && A.OSGID == O.AppointmentId
                                 && O.OrderType == M.CodeID
                                 && C.CodeValAbbr == A.ProfessionalType && C.CodeName == "RCTProfessional" && O.OrderId == orderid
                                 select new
                                 {
                                     O.AppointmentId,
                                     O.NewProjectId,
                                     O.OldProjectId,
                                     A.bcc,
                                     O.OrderDate,
                                     C.CodeValDetail,
                                     A.Name,
                                     O.FromDate,
                                     O.ToDate,
                                     O.Basic,
                                     A.EmployeersID,
                                     A.TypeofAppointment,
                                     A.ContactNumber,
                                     D.Designation,
                                     A.ToMail,
                                     A.Email,
                                     A.PhysicallyChallenged,
                                     O.CrtdUser,
                                     A.Salary,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     O.NewDesignation,
                                     O.OldDesignation,
                                     ordertype = M.CodeDescription
                                 }).FirstOrDefault();
                    var salquery = (from A in IOAScontext.tblRCTSalaryCalcDetails
                                    where A.OrderId == orderid
                                    select A).FirstOrDefault();

                    if (query != null && salquery != null)
                    {
                        int? crtduser = query.CrtdUser;
                        ProjectID = query.NewProjectId == null ? query.OldProjectId ?? 0 : query.NewProjectId ?? 0;
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addBcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addBcc.Add(mailid); });
                        NotePIModel ackmodel = new NotePIModel();
                        ackmodel.ApplicationReceiveDate = String.Format("{0:dd-MMMM-yyyy}", query.OrderDate);
                        ackmodel.ProjectNumber = Common.GetProjectNumber(ProjectID);
                        ackmodel.Ordertype = query.ordertype;
                        ackmodel.SendSlryStruct = sendsalrystruct;
                        ackmodel.IsDeviation = false;
                        ackmodel.PersonName = query.CodeValDetail + "" + query.Name.ToUpper();
                        ackmodel.DesignationName = query.Designation;
                        ackmodel.AppointmentType = Common.GetCodeControlName(query.TypeofAppointment ?? 0, "OSGAppointmenttype");
                        ackmodel.FromDate = String.Format("{0:dd-MMMM-yyyy}", query.FromDate);
                        ackmodel.ToDate = String.Format("{0:dd-MMMM-yyyy}", query.ToDate);
                        ackmodel.BasicPay = string.Format(Indian, "{0:N0}", query.Basic ?? 0);
                        ackmodel.EmployeeNum = query.EmployeersID;
                        ackmodel.DesignationName = query.Designation;
                        ackmodel.Email = query.Email;
                        ackmodel.MobileNumber = query.ContactNumber;
                        ackmodel.PhysicallyChallenged = query.PhysicallyChallenged;
                        ackmodel.DAName = Common.GetUserFirstName(userid);
                        string paystructure = " status";
                        if (sendsalrystruct == "SST")
                            paystructure = "_Pay structure approval";

                        if (query.Basic != query.Salary)
                            ackmodel.SalaryDiff_f = true;
                        if (query.NewDesignation != query.OldDesignation)
                            ackmodel.DesignationDiff_f = true;
                        if (query.AppointmentEnddate < query.FromDate)
                            ackmodel.Extended_f = true;

                        if (salquery.PFBasicWages > 0)
                            ackmodel.PFEligiblity = "Yes";
                        else
                            ackmodel.PFEligiblity = "No";
                        if (salquery.EmployeeESIC > 0)
                            ackmodel.ESICEligiblity = "Yes";
                        else
                            ackmodel.ESICEligiblity = "No";

                        if (ackmodel.Ordertype == "Change of project")
                        {
                            ackmodel.subject = "Change of Project" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                        }
                        else if (ackmodel.Ordertype == "Extension")
                        {
                            ackmodel.FillFields = "term extension";
                            ackmodel.subject = "Extension" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                            if (query.Basic < query.Salary)
                            {
                                ackmodel.subject = "Revision of pay" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "revision of pay";
                            }
                            else if (query.Basic > query.Salary)
                            {
                                ackmodel.subject = "Extension cum enhancement" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "extension cum enhancement";
                            }
                        }
                        else if (ackmodel.Ordertype == "Enhancement")
                        {
                            ackmodel.FillFields = "salary enhancement";
                            ackmodel.subject = "Enhancement" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";

                            if (query.Basic < query.Salary)
                            {
                                ackmodel.subject = "Revision of pay" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "appointment";
                            }
                            else if (query.NewProjectId != query.OldProjectId)
                            {
                                ackmodel.subject = "Extension with Change of project request" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "extension with change of project";
                            }
                            else if (query.NewDesignation != query.OldDesignation && ackmodel.Extended_f == true && ackmodel.SalaryDiff_f == true)
                            {
                                ackmodel.subject = "Extension cum Enhancement with Change of designation" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "change of designation";
                            }
                            else if (query.NewDesignation != query.OldDesignation && ackmodel.Extended_f == true)
                            {
                                ackmodel.subject = "Extension with Change of designation" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "change of designation";
                            }
                            else if (query.NewDesignation != query.OldDesignation && ackmodel.Extended_f == false && ackmodel.SalaryDiff_f == true)
                            {
                                ackmodel.subject = "Change of designation with enhancement" + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "change of designation";
                            }
                            else if (query.NewDesignation != query.OldDesignation && ackmodel.Extended_f == false)
                            {
                                ackmodel.subject = "Change of designation " + paystructure + " for " + ackmodel.PersonName + " – Outsourcing";
                                ackmodel.FillFields = "change of designation";
                            }
                        }

                        RCTReportMasterController _reportC = new RCTReportMasterController();
                        string SalaryStructure = _reportC.GenerateSalaryStructure(query.AppointmentId ?? 0, orderid);
                        Uri uri = new Uri(SalaryStructure);
                        string actName = uri.Segments.Last();
                        salquery.SalaryStructureDocPath = actName;
                        IOAScontext.SaveChanges();
                        var Attachments = SalaryStructure;
                        AttachmentList.Add(new EmailAttachmentModel
                        {
                            actualName = Attachments,
                            displayName = ackmodel.PersonName + "_Paystructure.pdf"
                        });

                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        List<string> addcc = new List<string>();
                        ackmodel.toMail = query.ToMail;
                        ackmodel.cc = addBcc;
                        emodel = ackmodel;
                        if (sendsalrystruct == "SST")
                            emodel.attachmentlist = AttachmentList;

                        var bodyResp = _eb.RunCompile("OSGOrdersAcknowledgements.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            EmailStatus.Attachment = Attachments;
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 6;
                            EmailStatus.OSGID = AppID;
                            EmailStatus.OrderId = orderid;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static int DeviationMailForOSGOrders(int orderid, int userid, string comments = null, string sendsalrystruct = null)
        {
            int res = 0;
            int AppID = 0, ProjectID = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();

                    List<string> addcc = new List<string>();
                    List<EmailAttachmentModel> AttachmentList = new List<EmailAttachmentModel>();
                    var query = (from A in context.tblOrder
                                 from D in context.tblRCTDesignation
                                 from M in context.tblRCTOutsourcing
                                 from Md in context.tblOrderMaster
                                 from C in context.tblCodeControl
                                 where C.CodeValAbbr == M.ProfessionalType && C.CodeName == "RCTProfessional"
                                 && A.OrderType == Md.CodeID
                                 && A.NewDesignation == D.DesignationId && M.OSGID == A.AppointmentId && A.OrderId == orderid
                                 select new
                                 {
                                     A.AppointmentId,
                                     A.FromDate,
                                     A.ToDate,
                                     A.Basic,
                                     A.CrtdUser,
                                     A.OldProjectId,
                                     A.NewProjectId,
                                     A.OldDesignation,
                                     A.NewDesignation,
                                     D.Designation,
                                     M.AppointmentStartdate,
                                     M.AppointmentEnddate,
                                     M.bcc,
                                     M.ToMail,
                                     M.TypeofAppointment,
                                     M.DesignationId,
                                     M.ProjectId,
                                     M.Salary,
                                     C.CodeValDetail,
                                     M.Name,
                                     A.isExtended,
                                     ordertype = Md.CodeDescription
                                 }).FirstOrDefault();

                    var salquery = (from A in context.tblRCTSalaryCalcDetails
                                    where A.OrderId == orderid
                                    select A).FirstOrDefault();

                    if (query != null && salquery != null)
                    {
                        AppID = query.AppointmentId ?? 0;
                        ProjectID = query.NewProjectId == null ? query.OldProjectId ?? 0 : query.NewProjectId ?? 0;

                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC("OSG").ForEach(mailid => { addcc.Add(mailid); });
                        if (query.Basic != query.Salary)
                            npmodel.SalaryDiff_f = true;
                        if (query.NewDesignation != query.OldDesignation)
                            npmodel.DesignationDiff_f = true;
                        if (query.AppointmentEnddate < query.FromDate)
                            npmodel.Extended_f = true;
                        RCTReportMasterController _reportC = new RCTReportMasterController();
                        string SalaryStructure = _reportC.GenerateSalaryStructure(query.AppointmentId ?? 0, orderid);
                        Uri uri = new Uri(SalaryStructure);
                        string actName = uri.Segments.Last();
                        salquery.SalaryStructureDocPath = actName;
                        context.SaveChanges();
                        npmodel.PersonName = query.CodeValDetail + "" + query.Name;
                        AttachmentList.Add(new EmailAttachmentModel
                        {
                            actualName = SalaryStructure,
                            displayName = npmodel.PersonName + "_Paystructure.pdf"
                        });
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.FromDate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.ToDate);
                        npmodel.Comments = comments;
                        npmodel.IsDeviation = true;
                        npmodel.Salary = query.Basic ?? 0;
                        npmodel.SendSlryStruct = sendsalrystruct;
                        npmodel.DesignationName = query.Designation;
                        npmodel.AppointmentType = Common.GetCodeControlName(query.TypeofAppointment ?? 0, "OSGAppointmenttype");
                        npmodel.checkdetails = RCTEmailContentService.getDevNormsDetails(AppID, "OSG", orderid);
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.Ordertype = query.ordertype;
                        npmodel.ProjectNumber = Common.GetProjectNumber(ProjectID);
                        if (npmodel.Ordertype == "Change of project")
                            npmodel.subject = "Deviation(s) from the norms in the change of project request for " + npmodel.PersonName + " - Outsourcing";
                        else if (npmodel.Ordertype == "Extension")
                        {
                            npmodel.subject = "Deviation(s) from the norms in the term extension for " + npmodel.PersonName + " - Outsourcing";
                            npmodel.FillFields = "term extension";
                            if (query.Salary > query.Basic)
                            {
                                npmodel.subject = "Deviation(s) from the norms in the revision of pay for " + npmodel.PersonName + " - Outsourcing";
                                npmodel.FillFields = "revision of pay";
                            }
                            else if (query.Salary < query.Basic)
                            {
                                npmodel.subject = "Deviation(s) from the norms in the term extension cum enhancement for " + npmodel.PersonName + " - Outsourcing";
                                npmodel.FillFields = "extension cum enhancement";
                            }
                        }
                        else if (npmodel.Ordertype == "Enhancement")
                        {
                            npmodel.subject = "Deviation(s) from the norms in the enhancement request for " + npmodel.PersonName + " – Outsourcing";
                            npmodel.FillFields = "salary enhancement";
                            if (query.Salary > query.Basic)
                            {
                                npmodel.FillFields = "appointment";
                                npmodel.subject = "Deviation(s) from the norms in the revision of pay for " + npmodel.PersonName + " – Outsourcing";
                            }
                            else if (query.ProjectId != query.NewProjectId)
                            {
                                npmodel.FillFields = "extension with change of project";
                                npmodel.subject = "Deviation(s) from the norms in the Extnesion with Change of project request for " + npmodel.PersonName + " – Outsourcing";
                            }
                            else if (query.NewDesignation != query.OldDesignation)
                            {
                                if (npmodel.Extended_f == true && npmodel.SalaryDiff_f == true)
                                {
                                    npmodel.subject = "Deviation(s) from the norms in the Extension cum Enhancement with Change of designation for " + npmodel.PersonName + " – Outsourcing";
                                    npmodel.FillFields = "extension cum enhancement with change of designation";
                                }
                                else if (npmodel.Extended_f == true)
                                {
                                    npmodel.subject = "Deviation(s) from the norms in the Extension with Change of designation for " + npmodel.PersonName + " – Outsourcing";
                                    npmodel.FillFields = "extension with change of designation";
                                }
                                else if (npmodel.Extended_f == false && npmodel.SalaryDiff_f == true)
                                {
                                    npmodel.subject = "Deviation(s) from the norms in the Change of designation with enhancement for " + npmodel.PersonName + " – Outsourcing";
                                    npmodel.FillFields = "change of designation with enhancement";
                                }
                                else if (npmodel.Extended_f == false)
                                {
                                    npmodel.subject = "Deviation(s) from the norms in the Change of designation for " + npmodel.PersonName + " – Outsourcing";
                                    npmodel.FillFields = "change of designation";
                                }
                            }

                        }
                        emodel = npmodel;
                        if (sendsalrystruct == "SST")
                            emodel.attachmentlist = AttachmentList;
                        var bodyResp = _eb.RunCompile("OSGOrdersDeviation.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            EmailStatus.Attachment = SalaryStructure;
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 7;
                            EmailStatus.OSGID = AppID;
                            EmailStatus.OrderId = orderid;
                            context.tblRCTOSGEmailLog.Add(EmailStatus);
                            context.SaveChanges();
                        }
                        res = 1;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        #endregion

        #region STE Mails

        public static int AcknowledgementMailForSTE(int appid, int userid)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                    var query = (from A in context.vw_RCTOverAllApplicationEntry
                                 where A.ApplicationId == appid && A.Category == "STE"
                                 select new
                                 {
                                     A.bcc,
                                     A.ProfessionalType,
                                     A.ToMail,
                                     A.Email,
                                     A.ApplicationType,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     A.CandidateName,
                                     A.crtdUserId,
                                     A.ProjectId,
                                     A.ConsolidatedPay,
                                     A.TypeofAppointment,
                                     A.BasicPay,
                                     A.Fellowship,
                                     A.PostRecommended,
                                     A.ApplicationId
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        addcc.Add(query.Email);
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC("STE").ForEach(mailid => { addcc.Add(mailid); });
                        npmodel.subject = "ICSR - Application status of " + query.ProfessionalType + " " + query.CandidateName;
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0); ;
                        npmodel.Ordertype = query.ApplicationType.ToLower();
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.TypeofAppointment = query.TypeofAppointment;
                        npmodel.Paytype = query.ConsolidatedPay == true ? "Consolidated pay" : query.Fellowship == true ? "Fellowship pay" : "";
                        npmodel.BasicPay = string.Format("{0:#,##0.############}", query.BasicPay);
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTSTEAckTemplate.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 1;
                            EmailStatus.STEID = query.ApplicationId ?? 0;
                            context.tblRCTSTEEmailLog.Add(EmailStatus);
                            context.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static int DeviationMailForSTE(int appid, int userid, string comments = null)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                    var query = (from A in context.vw_RCTOverAllApplicationEntry
                                 where A.ApplicationId == appid && A.Category == "STE"
                                 select new
                                 {
                                     A.bcc,
                                     A.ProfessionalType,
                                     A.ToMail,
                                     A.ApplicationType,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     A.CandidateName,
                                     A.crtdUserId,
                                     A.ProjectId,
                                     A.TypeofAppointment,
                                     A.BasicPay,
                                     A.PostRecommended,
                                     A.ApplicationId
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC("STE").ForEach(mailid => { addcc.Add(mailid); });
                        npmodel.subject = "Deviation(s) from the norms in the application of " + query.ProfessionalType + " " + query.CandidateName;
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.AppointmentType = query.TypeofAppointment;
                        npmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0); ;
                        npmodel.Ordertype = query.ApplicationType.ToLower();
                        npmodel.checkdetails = getDevNormsDetails(query.ApplicationId ?? 0, "STE");
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.Comments = comments;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTSTEDevTemplate.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 2;
                            EmailStatus.STEID = query.ApplicationId ?? 0;
                            context.tblRCTSTEEmailLog.Add(EmailStatus);
                            context.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        #endregion

        public static int AcknowledgementForOrders(int orderid, int userid, string comments = null)
        {
            int res = 0;
            int AppID = 0, AppType = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                    var query = (from A in context.vw_RCTOverAllApplicationEntry
                                 where A.OrderId == orderid
                                 select new
                                 {
                                     A.bcc,
                                     A.ProfessionalType,
                                     A.ToMail,
                                     A.Email,
                                     A.ApplicationType,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     A.CandidateName,
                                     A.crtdUserId,
                                     A.ProjectId,
                                     A.TypeofAppointment,
                                     A.BasicPay,
                                     A.PostRecommended,
                                     A.ApplicationId,
                                     A.AppointmentType,
                                     A.ConsolidatedPay,
                                     A.Fellowship,
                                     A.isMsPhd,
                                     A.Category
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        AppID = query.ApplicationId ?? 0;
                        AppType = query.AppointmentType ?? 0;
                        addcc.Add(query.Email);
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        var queryodr = (from o in context.tblOrder
                                        from vw in context.vw_RCTOverAllApplicationEntry
                                        where o.AppointmentId == vw.ApplicationId && o.AppointmentType == vw.AppointmentType && vw.ApplicationType == "New" && o.OrderId == orderid
                                        select new
                                        {
                                            NewSalary = o.Basic,
                                            OldSalary = vw.BasicPay,
                                            o.NewDesignation,
                                            o.OldDesignation,
                                            o.NewProjectId,
                                            o.OldProjectId
                                        }).FirstOrDefault();

                        if (query.ApplicationType == "Extension" && (queryodr.NewSalary == queryodr.OldSalary || queryodr.NewSalary > queryodr.OldSalary))
                        {
                            npmodel.subject = "Term extension status for " + query.ProfessionalType + " " + query.CandidateName;
                        }
                        else if (query.ApplicationType == "Extension" && queryodr.NewSalary < queryodr.OldSalary)
                        {
                            npmodel.subject = "Revision of pay for " + query.ProfessionalType + " " + query.CandidateName;
                            npmodel.Offer_f = true;
                            npmodel.FillFields = "revision of pay";
                        }
                        else if (query.ApplicationType == "Enhancement" && (queryodr.NewSalary == queryodr.OldSalary || queryodr.OldSalary < queryodr.NewSalary) && queryodr.NewDesignation == queryodr.OldDesignation && queryodr.NewProjectId == queryodr.OldProjectId)
                        {
                            npmodel.subject = "Salary enhancement status for " + query.ProfessionalType + " " + query.CandidateName;
                        }
                        else if (query.ApplicationType == "Enhancement" && queryodr.OldSalary > queryodr.NewSalary && queryodr.NewDesignation == queryodr.OldDesignation && queryodr.NewProjectId == queryodr.OldProjectId)
                        {
                            npmodel.subject = "Revision of pay for " + query.ProfessionalType + " " + query.CandidateName;
                            npmodel.FillFields = "revision of pay";
                            npmodel.Offer_f = true;
                        }
                        else
                        {
                            npmodel.subject = query.ApplicationType + " status for " + query.ProfessionalType + " " + query.CandidateName;
                            npmodel.Offer_f = true;
                        }

                        if (query.ApplicationType == "Enhancement" && queryodr.NewDesignation != queryodr.OldDesignation)
                            npmodel.FillFields = "change of designation";
                        if (query.ApplicationType == "Enhancement" && queryodr.NewProjectId != queryodr.OldProjectId)
                            npmodel.FillFields = "change of project";
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0); ;
                        npmodel.Ordertype = query.ApplicationType.ToLower();
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.Paytype = query.ConsolidatedPay == true ? "Consolidated pay" : query.Fellowship == true ? "Fellowship pay" : "";
                        npmodel.BasicPay = string.Format("{0:#,##0.############}", query.BasicPay);
                        npmodel.isMSPhd = query.isMsPhd ?? false;
                        npmodel.TypeofAppointment = query.TypeofAppointment;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTOrderAckTemplate.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            if (AppType == 1)
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 6;
                                EmailStatus.OrderId = orderid;
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId ?? 0;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            if (AppType == 2)
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 6;
                                EmailStatus.STEID = query.ApplicationId ?? 0;
                                EmailStatus.OrderId = orderid;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                        }
                        res = 1;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static int DeviationMailForOrders(int orderid, int userid, string comments = null)
        {
            int res = 0;
            int AppID = 0, AppType = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                    var query = (from A in context.vw_RCTOverAllApplicationEntry
                                 where A.OrderId == orderid
                                 select new
                                 {
                                     A.bcc,
                                     A.ProfessionalType,
                                     A.ToMail,
                                     A.ApplicationType,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     A.CandidateName,
                                     A.crtdUserId,
                                     A.ProjectId,
                                     A.TypeofAppointment,
                                     A.BasicPay,
                                     A.PostRecommended,
                                     A.ApplicationId,
                                     A.AppointmentType,
                                     A.Category,
                                     A.ConsolidatedPay,
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        AppID = query.ApplicationId ?? 0;
                        AppType = query.AppointmentType ?? 0;
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        npmodel.subject = "Deviation from the norms in the term " + query.ApplicationType.ToLower() + " for " + query.ProfessionalType + " " + query.CandidateName;
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0); ;
                        npmodel.Ordertype = query.ApplicationType.ToLower();
                        npmodel.checkdetails = getDevNormsDetails(query.ApplicationId ?? 0, query.Category, orderid);
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.Paytype = query.ConsolidatedPay == true ? "Consolidated Pay" : "Fellowship Pay";
                        npmodel.Comments = comments;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("DeviationMailForExEnh.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            if (AppType == 1)
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 7;
                                EmailStatus.OrderId = orderid;
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId ?? 0;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            if (AppType == 2)
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 7;
                                EmailStatus.STEID = query.ApplicationId ?? 0;
                                EmailStatus.OrderId = orderid;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            if (AppType == 3)
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 7;
                                EmailStatus.OSGID = query.ApplicationId ?? 0;
                                EmailStatus.OrderId = orderid;
                                context.tblRCTOSGEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                        }
                        res = 1;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static Tuple<int, string> SendMailForCancelApp(int appid, string apptype, int userid, bool getBody_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where S.PIId == vw.UserId && S.ProjectId == P.ProjectId
                                 && S.ApplicationId == appid && S.Category == apptype
                                 && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     Name = S.ProfessionalType + " " + S.CandidateName,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.Email,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     P.ProjectNumber,
                                     P.ProjectTitle,
                                     S.crtdUserId,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.TypeofAppointment,
                                     vw.DepartmentName,
                                     S.Category,
                                     vw.EmployeeId,
                                     S.Status,
                                     S.ApplicationType
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        var querycancellog = (from l in IOAScontext.tblRCTCancelApplicationLog
                                              where l.ApplicationId == query.ApplicationId && l.AppointmentType == query.AppointmentType
                                              && (l.OrderId == orderid || orderid == null)
                                              select l).FirstOrDefault();
                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel npmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        if (query.Category != "OSG")
                            addcc.Add(query.Email);
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "OfficeOrder" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && (querycancellog.PreStatus == "Verification Completed" || querycancellog.PreStatus == "Completed"))
                        {
                            npmodel.Cancel_f = "Office Order";
                            npmodel.subject = "ICSR - Office order is cancelled " + query.Name;
                            var OfficeOrderDate = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "OfficeOrder" && (x.OrderId == orderid || (orderid == null && x.OrderId == null))).Select(x => x.UPTD_TS).FirstOrDefault();
                            npmodel.OfficeOrderDate = string.Format("{0:dd-MMMM-yyyy}", OfficeOrderDate ?? DateTime.Now);
                        }
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "OfferLetter" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && (querycancellog.PreStatus == "Verification Completed" || querycancellog.PreStatus == "Completed"))
                        {
                            npmodel.Cancel_f = "Office Order";
                            npmodel.subject = "ICSR - Offer letter is cancelled " + query.Name;
                            var OfficeOrderDate = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "OfferLetter" && (x.OrderId == orderid || (orderid == null && x.OrderId == null))).Select(x => x.UPTD_TS).FirstOrDefault();
                            npmodel.OfficeOrderDate = string.Format("{0:dd-MMMM-yyyy}", OfficeOrderDate ?? DateTime.Now);
                        }
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "Order" && x.isSend == true && x.OrderId == orderid) && querycancellog != null && querycancellog.PreStatus == "Completed")
                        {
                            npmodel.Cancel_f = query.ApplicationType + " Order";
                            npmodel.subject = "ICSR - " + query.ApplicationType + " order is cancelled " + query.Name;
                            var OfficeOrderDate = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "Order" && x.OrderId == orderid).Select(x => x.UPTD_TS).FirstOrDefault();
                            npmodel.OfficeOrderDate = string.Format("{0:dd-MMMM-yyyy}", OfficeOrderDate ?? DateTime.Now);
                        }
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == apptype && x.OfferCategory == "OfferLetter" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && querycancellog.PreStatus == "Awaiting Verification")
                        {
                            npmodel.subject = "ICSR - Offer cancellation for " + query.Name + "(" + query.TypeofAppointment + ")";
                            npmodel.Cancel_f = "Verification level";
                        }
                        else
                            npmodel.subject = "ICSR - Cancellation of application for " + query.Name;

                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.Name;
                        npmodel.ProjectNumber = query.ProjectNumber;
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.Department = query.DepartmentName;
                        npmodel.Apptype = query.Category;
                        npmodel.TypeofAppointment = query.TypeofAppointment;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("ApplicationCancel.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);

                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            if (query.AppointmentType == 2)
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 4; //Application Cancel
                                EmailStatus.STEID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTSTEEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                            }
                            else if (query.AppointmentType == 1)
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 4;
                                EmailStatus.OrderId = query.OrderId;
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId;
                                IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                            }
                            else if (query.AppointmentType == 3)
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 4;
                                EmailStatus.OSGID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                            }
                            if (isSend)
                                return Tuple.Create(1, "");
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                RequirementService rct = new RequirementService();
                Infrastructure.IOASException.Instance.HandleMe(rct, ex);
                return Tuple.Create(0, "");
            }
        }

        public static Tuple<int, string> SendVendorCancellationMail(int OSGID, int userid, bool getBody_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 where S.ApplicationId == OSGID && S.Category == "OSG"
                                 && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     Name = S.ProfessionalType + " " + S.CandidateName,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.Status,
                                     S.TypeofAppointment,
                                     S.Category
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        var querycancellog = (from m in IOAScontext.tblRCTCancelApplicationLog
                                              where m.ApplicationId == query.ApplicationId && m.AppointmentType == 3
                                              && (m.OrderId == orderid || orderid == null)
                                              select m).FirstOrDefault();

                        var queryVendor = (from A in IOAScontext.tblRCTOutsourcing
                                           where A.OSGID == query.ApplicationId
                                           select A).FirstOrDefault();

                        var venQuery = (from SM in IOAScontext.tblSalaryAgencyMaster where SM.SalaryAgencyId == queryVendor.VendorId select SM).FirstOrDefault();

                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel npmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        if (querycancellog.PreStatus == "Draft" || querycancellog.PreStatus == "Note to PI" || querycancellog.PreStatus == "Open")
                            npmodel.Cancel_f = "application";

                        if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfficeOrder" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && (querycancellog.PreStatus == "Verification Completed" || querycancellog.PreStatus == "Completed"))
                            npmodel.Cancel_f = "appointment letter";
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfferLetter" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && (querycancellog.PreStatus == "Verification Completed" || querycancellog.PreStatus == "Completed"))
                            npmodel.Cancel_f = "appointment letter";
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "Order" && x.isSend == true && x.OrderId == orderid) && querycancellog != null && querycancellog.PreStatus == "Completed")
                            npmodel.Cancel_f = "appointment letter";
                        else if (IOAScontext.tblRCTOfferDetails.Any(x => x.ApplicationId == query.ApplicationId && x.Category == "OSG" && x.OfferCategory == "OfferLetter" && x.isSend == true && (x.OrderId == orderid || (orderid == null && x.OrderId == null))) && querycancellog != null && querycancellog.PreStatus == "Awaiting Verification")
                            npmodel.Cancel_f = "offer letter";

                        npmodel.subject = "ICSR - Cancellation of " + npmodel.Cancel_f + " for " + query.Name + " - Outsourcing";
                        npmodel.toMail = venQuery.ContactEmail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.Name;
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.TypeofAppointment = query.TypeofAppointment;
                        npmodel.Apptype = "OSG";
                        npmodel.vendorMail_f = true;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("ApplicationCancel.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 4;
                            EmailStatus.OSGID = query.ApplicationId;
                            EmailStatus.OrderId = query.OrderId;
                            IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            return Tuple.Create(1, "");
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                RequirementService rct = new RequirementService();
                Infrastructure.IOASException.Instance.HandleMe(rct, ex);
                return Tuple.Create(0, "");
            }
        }

        //Manually trigger mail only
        public static Tuple<int, string> SendMailForOfferLetter(int appid, string category, bool getBoby_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {

                    var querycheck = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == appid && x.Category == category && x.OfferCategory == "OfferLetter" && (x.OrderId == orderid || (x.OrderId == null && orderid == null)) && x.isSend == true).FirstOrDefault();
                    if (querycheck != null)
                        return Tuple.Create(0, "Offer Letter already sent");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == appid
                                 && S.Category == category && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     S.CandidateName,
                                     S.ProfessionalType,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     P.ProjectNumber,
                                     P.ProjectTitle,
                                     S.crtdUserId,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.TypeofAppointment,
                                     S.ApplicationType,
                                     S.EmployeersID,
                                     S.Email,
                                     S.Category,
                                     S.isMsPhd,
                                     S.PIName,
                                     vw.DepartmentName
                                 }).FirstOrDefault();

                    if (query != null)
                    {
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        List<string> addcc = new List<string>();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        if (!getBoby_f)
                        {
                            if (category == "STE")
                                attachementModel = RCTReportMasterController.GenerateOfferLetter(appid, orderid ?? 0);
                            if (category == "CON")
                                attachementModel = RCTReportMasterController.GenerateConsultantOfferLetter(appid, orderid ?? 0);
                            attachementModel.displayName = query.CandidateName + "_" + attachementModel.displayName;
                            AttachmentList.Add(attachementModel);
                        }
                        List<string> CheckList = new List<string>();
                        if (isExistingEmployee(appid, query.AppointmentType ?? 0) && query.ApplicationType == "New")
                            CheckList.Add(ChecklistPath + "Existing staff checklist.pdf");
                        else if (query.ApplicationType == "New")
                            CheckList.Add(ChecklistPath + "New joinee checklist.pdf");
                        else
                            CheckList.Add(ChecklistPath + "Existing staff checklist.pdf");

                        ackmodel.attachment = CheckList;
                        addcc.Add(query.ToMail);
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        ackmodel.FillFields = "appointment";
                        if (query.ApplicationType.Contains("Change of project"))
                            ackmodel.FillFields = "change of project";
                        else if (query.ApplicationType.Contains("Enhancement"))
                        {
                            if (IOAScontext.tblOrder.Any(x => x.OrderId == orderid && x.NewProjectId != x.OldProjectId))
                                ackmodel.FillFields = "change of project";
                            else if (IOAScontext.tblOrder.Any(x => x.OrderId == orderid && x.NewDesignation != x.OldDesignation))
                                ackmodel.FillFields = "change of designation";
                            else
                                ackmodel.FillFields = "appointment";
                        }

                        ackmodel.subject = "Offer letter_" + ackmodel.FillFields + " of " + query.ProfessionalType + " " + query.CandidateName;
                        ackmodel.toMail = query.Email;
                        ackmodel.cc = addcc;
                        ackmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        ackmodel.ProjectNumber = query.ProjectNumber;
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.TypeofAppointment = query.TypeofAppointment;
                        ackmodel.isMSPhd = query.isMsPhd ?? false;
                        ackmodel.MSPhD = MsOrPhD(appid, query.AppointmentType ?? 0);
                        ackmodel.PIName = query.PIName;
                        ackmodel.Department = query.DepartmentName;
                        ackmodel.isExistingEmployee = true;
                        if (!isExistingEmployee(appid, query.AppointmentType ?? 0) && query.ApplicationType == "New")
                            ackmodel.isExistingEmployee = false;
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RequirementOfferLetter.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBoby_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            emodel.attachmentByte = AttachmentList;

                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            //var queryoffer = IOAScontext.tblRCTOfferDetails.Where(m => m.ApplicationId == appid && m.Category == category && m.OfferCategory == "OfferLetter" && (orderid == null || m.OrderId == orderid) && m.isSend != true).FirstOrDefault();
                            var queryoffer = IOAScontext.tblRCTOfferDetails.Where(m => m.ApplicationId == appid && m.Category == category && m.OfferCategory == "OfferLetter" && (m.OrderId == orderid || (orderid == null && m.OrderId == null)) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = Userid;
                                queryoffer.UPTD_TS = DateTime.Now;
                                queryoffer.isSend = isSend;
                                IOAScontext.SaveChanges();
                            }

                            if (query.OrderId > 0)
                            {
                                var queryOrder = IOAScontext.tblOrderDetail.FirstOrDefault(m => m.OrderId == query.OrderId);
                                if (queryOrder != null)
                                {
                                    queryOrder.OfferLetter = attachementModel.actualName;
                                    queryOrder.OfferDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else if (query.Category == "STE")
                            {
                                var qry = (from s in IOAScontext.tblRCTSTE
                                           where s.STEID == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfferLetter = attachementModel.actualName;
                                    qry.OfferDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else if (query.Category == "CON")
                            {
                                var qry = (from s in IOAScontext.tblRCTConsultantAppointment
                                           where s.ConsultantAppointmentId == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfferLetter = attachementModel.actualName;
                                    qry.OfferDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }

                            //Email Log
                            if (query.Category == "STE")
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                EmailStatus.Attachment = attachementModel.actualName != null ? attachementModel.actualName : "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 5;
                                EmailStatus.STEID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTSTEEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                                res = 1;
                            }
                            else if (query.Category == "CON")
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                EmailStatus.Attachment = attachementModel.actualName != null ? attachementModel.actualName : "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 5;
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                                res = 1;
                            }
                        }
                    }
                }
                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, ex.Message);
            }
        }

        //Manually trigger mail only
        public static Tuple<int, string> SendMailForOfficeOrder(int appid, string category, bool getBody_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var querycheck = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == appid && x.Category == category && x.OfferCategory == "OfficeOrder" && (x.OrderId == orderid || (x.OrderId == null && orderid == null)) && x.isSend == true).FirstOrDefault();
                    if (querycheck != null)
                        return Tuple.Create(0, "Office Order Already sent");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == appid
                                 && S.Category == category && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     Name = S.ProfessionalType + " " + S.CandidateName,
                                     S.CandidateName,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     S.AppointmentStartdate,
                                     S.ApplicationType,
                                     S.Email,
                                     S.Category,
                                     S.PIName,
                                     S.crtdUserId,
                                 }).FirstOrDefault();

                    if (query != null)
                    {
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<string> attachmentList = new List<string>();
                        ByteEmailAttachmentModel attachmodel = new ByteEmailAttachmentModel();
                        List<ByteEmailAttachmentModel> AttachList = new List<ByteEmailAttachmentModel>();
                        addcc.Add(query.ToMail);
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        ackmodel.subject = "Office order for " + query.Name;
                        ackmodel.toMail = query.Email;
                        ackmodel.cc = addcc;
                        ackmodel.ApplicationNumber = RequirementService.getOfferDetails(query.ApplicationId ?? 0, query.Category, orderid, "OfferLetter");
                        ackmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.PIName = query.PIName;
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTOfficeOrder.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            RCTReportMasterController rptMast = new RCTReportMasterController();
                            attachmodel = rptMast.RCTOfficeOrderPrint(appid, category, orderid);
                            attachmodel.displayName = query.CandidateName + "_" + attachmodel.displayName;
                            AttachList.Add(attachmodel);
                            var orderdoc = attachmodel.actualName;
                            emodel.attachmentByte = AttachList;
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            var queryoffer = IOAScontext.tblRCTOfferDetails.OrderByDescending(o => o.OfferDetailId).Where(m => m.ApplicationId == appid && m.Category == category && m.OfferCategory == "OfficeOrder" && ((orderid == null && m.OrderId == null) || m.OrderId == orderid) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = Userid;
                                queryoffer.UPTD_TS = DateTime.Now;
                                queryoffer.isSend = isSend;
                                IOAScontext.SaveChanges();
                            }

                            if (query.Category == "STE" && query.OrderId == null)
                            {
                                var qry = (from s in IOAScontext.tblRCTSTE
                                           where s.STEID == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else if (query.Category == "CON" && query.OrderId == null)
                            {
                                var qry = (from s in IOAScontext.tblRCTConsultantAppointment
                                           where s.ConsultantAppointmentId == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else if (query.Category == "OSG" && query.OrderId == null)
                            {
                                var qry = (from s in IOAScontext.tblRCTOutsourcing
                                           where s.OSGID == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    //qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else
                            {
                                var qry = (from s in IOAScontext.tblOrderDetail
                                           where s.OrderId == query.OrderId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }

                            //Email Log
                            if (query.Category == "STE")
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                EmailStatus.Attachment = orderdoc;

                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 16;
                                EmailStatus.STEID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTSTEEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                                res = 1;
                            }
                            else if (query.Category == "CON")
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                EmailStatus.Attachment = orderdoc;
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 16;
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                                res = 1;
                            }
                            else if (query.Category == "OSG")
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                EmailStatus.Attachment = orderdoc;
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 16;
                                EmailStatus.OSGID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                IOAScontext.tblRCTOSGEmailLog.Add(EmailStatus);
                                IOAScontext.SaveChanges();
                                res = 1;
                            }
                        }
                    }
                }
                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, ex.Message);
            }
        }

        //Manually trigger mail only for hra order
        public static Tuple<int, string> SendMailForHRA(int orderId, int logged_in_userId, bool Ack_f = false, bool Send_f = false, bool isgetBody_f = false)
        {
            List<string> addcc = new List<string>();
            List<string> AttachmentList = new List<string>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (orderId > 0)
                    {
                        if (context.tblRCTOfferDetails.Any(x => x.OrderId == orderId && x.isSend == true && x.OfferCategory == "Order"))
                            return Tuple.Create(0, "HRA order already sent");

                        var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                                   where A.OrderId == orderId
                                   select new
                                   {
                                       A.bcc,
                                       A.ProfessionalType,
                                       A.ToMail,
                                       A.ApplicationType,
                                       A.AppointmentStartdate,
                                       A.AppointmentEnddate,
                                       A.CandidateName,
                                       A.crtdUserId,
                                       A.ProjectId,
                                       A.TypeofAppointment,
                                       A.BasicPay,
                                       A.PostRecommended,
                                       A.ApplicationId,
                                       A.AppointmentType,
                                       A.HRA,
                                       A.OrderId,
                                       A.Email
                                   }).FirstOrDefault();
                        if (Qry != null)
                        {
                            EmailBuilder _eb = new EmailBuilder();
                            EmailModel emodel = new EmailModel();
                            NotePIModel npmodel = new NotePIModel();
                            if (Send_f)
                                addcc.Add(Qry.ToMail);
                            else
                                addcc.Add(Qry.Email);
                            if (Qry.bcc != null)
                            {
                                var bcc = Qry.bcc.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                            getDefaultCC("STE").ForEach(mailid => { addcc.Add(mailid); });
                            npmodel.PersonName = Qry.ProfessionalType + "" + Qry.CandidateName.ToUpper();

                            DateTime? HRAFromDate = DateTime.Now;
                            DateTime? HRAToDate = DateTime.Now;
                            if (Qry.ApplicationType == "HRA Cancellation")
                            {
                                var queryste = context.tblRCTSTE.FirstOrDefault(m => m.STEID == Qry.ApplicationId);
                                if (queryste != null)
                                {
                                    HRAFromDate = queryste.AppointmentStartdate;
                                    HRAToDate = queryste.AppointmentEnddate;
                                    var querypreodr = (from O in context.tblOrder
                                                       where O.AppointmentId == Qry.ApplicationId && O.AppointmentType == 2
                                                       && O.OrderType == 5 && O.Status.Contains("Complete") && O.isUpdated == true
                                                       && O.FromDate >= queryste.AppointmentStartdate && O.ToDate <= queryste.AppointmentEnddate
                                                       orderby O.OrderId descending
                                                       select O).FirstOrDefault();
                                    if (querypreodr != null)
                                    {
                                        HRAFromDate = querypreodr.FromDate ?? DateTime.Now;
                                        HRAToDate = querypreodr.ToDate ?? DateTime.Now;
                                    }
                                }
                            }

                            if (Ack_f)
                            {
                                npmodel.subject = "ICSR - House Rent Allowances status for " + npmodel.PersonName;
                            }
                            else if (Send_f)
                            {
                                npmodel.subject = "ICSR - House Rent Allowance order for " + npmodel.PersonName;
                            }
                            else if (Qry.ApplicationType == "HRA Cancellation" && HRAFromDate == Qry.AppointmentStartdate)
                            {
                                npmodel.HRAFullCancel_f = true;
                                npmodel.subject = "ICSR - HRA cancellation for " + npmodel.PersonName;
                            }
                            else if (Qry.ApplicationType == "HRA Cancellation")
                            {
                                npmodel.subject = "ICSR - House Rent Allowance stopped for " + npmodel.PersonName;
                            }
                            if (Send_f)
                                npmodel.toMail = Qry.Email;
                            else
                                npmodel.toMail = Qry.ToMail;
                            npmodel.cc = addcc;
                            npmodel.AppointmentStartDate = string.Format("{0:dd-MMM-yyyy}", Qry.AppointmentStartdate);
                            npmodel.AppointmentEndDate = string.Format("{0:dd-MMM-yyyy}", Qry.AppointmentEnddate);
                            npmodel.TypeofAppointment = Qry.TypeofAppointment;
                            npmodel.ProjectNumber = Common.GetProjectNumber(Qry.ProjectId ?? 0);
                            npmodel.DesignationName = Qry.PostRecommended;
                            npmodel.AppointmentType = Qry.ApplicationType;
                            npmodel.Ack_f = Ack_f;
                            npmodel.Send_f = Send_f;
                            npmodel.BasicPay = string.Format("{0:#,##0.############}", Qry.HRA);
                            npmodel.DAName = Common.GetUserFirstName(logged_in_userId);
                            emodel = npmodel;
                            var bodyResp = _eb.RunCompile("RCTHRAMailTemplate.cshtml", "", npmodel, typeof(NotePIModel));
                            if (bodyResp.Item1)
                            {
                                if (isgetBody_f)
                                    return Tuple.Create(-1, bodyResp.Item2);
                                if (Qry.ApplicationType == "HRA Booking" && Send_f)
                                {
                                    ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                                    List<ByteEmailAttachmentModel> listmodel = new List<ByteEmailAttachmentModel>();
                                    RCTReportMasterController _reportC = new RCTReportMasterController();
                                    model = _reportC.HRAOrderPrint(orderId);
                                    model.displayName = Qry.CandidateName.ToUpper() + "_" + model.displayName;
                                    listmodel.Add(model);
                                    emodel.attachmentByte = listmodel;
                                    string docName = model.actualName;
                                    if (!string.IsNullOrEmpty(docName))
                                    {
                                        var query = (from od in context.tblOrderDetail
                                                     where od.OrderId == orderId
                                                     select od).FirstOrDefault();
                                        if (query != null)
                                        {
                                            query.OfficeOrder = docName;
                                            query.OfficeOrderDate = DateTime.Now;
                                            context.SaveChanges();
                                        }
                                    }
                                }

                                var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                                var queryoffer = context.tblRCTOfferDetails.Where(m => m.OfferCategory == "Order" && m.OrderId == orderId && m.isSend != true).FirstOrDefault();
                                if (queryoffer != null)
                                {
                                    queryoffer.UPTD_USER = logged_in_userId;
                                    queryoffer.UPTD_TS = DateTime.Now;
                                    queryoffer.isSend = isSend;
                                    context.SaveChanges();
                                }
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = logged_in_userId;
                                EmailStatus.IsSend = isSend;
                                if (Ack_f)
                                    EmailStatus.TypeofMail = 6;
                                else
                                    EmailStatus.TypeofMail = 5;
                                EmailStatus.STEID = Qry.ApplicationId;
                                EmailStatus.OrderId = Qry.OrderId;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                                return Tuple.Create(1, "");
                            }
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        //Manually trigger mail
        public static Tuple<int, string> SendRelievingOrder(int orderId, int logged_in_userId, bool getbody_f = false)
        {
            List<string> addcc = new List<string>();
            List<string> AttachmentList = new List<string>();
            var res = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (orderId > 0)
                    {
                        if (context.tblRCTOfferDetails.Any(x => x.isSend == true && x.OfferCategory == "Order" && x.OrderId == orderId))
                            return Tuple.Create(0, "Relieve order already sent");

                        var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                                   from od in context.tblOrderDetail
                                   where A.OrderId == od.OrderId && A.OrderId == orderId
                                   select new
                                   {
                                       A.bcc,
                                       A.ProfessionalType,
                                       A.ToMail,
                                       A.ApplicationType,
                                       A.AppointmentStartdate,
                                       A.AppointmentEnddate,
                                       A.CandidateName,
                                       A.crtdUserId,
                                       A.ProjectId,
                                       A.TypeofAppointment,
                                       A.BasicPay,
                                       A.PostRecommended,
                                       A.ApplicationId,
                                       A.AppointmentType,
                                       A.ConsolidatedPay,
                                       A.Fellowship,
                                       A.Status,
                                       A.OrderId,
                                       od,
                                       A.Email,
                                       A.Category
                                   }).FirstOrDefault();
                        if (Qry != null)
                        {
                            if (Qry.Status == "Open" && Qry.AppointmentType == 1 && context.tblRCTConsutantAppEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count > 0)
                                return Tuple.Create(0, "Relieve order already sent");
                            else if (Qry.Status == "Open" && Qry.AppointmentType == 2 && context.tblRCTSTEEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count > 0)
                                return Tuple.Create(0, "Relieve order already sent");
                            else if (Qry.Status == "Open" && Qry.AppointmentType == 3 && context.tblRCTOSGEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count > 0)
                                return Tuple.Create(0, "Relieve order already sent");
                            EmailBuilder _eb = new EmailBuilder();
                            EmailModel emodel = new EmailModel();
                            NotePIModel ackmodel = new NotePIModel();
                            addcc.Add(Qry.ToMail);
                            if (Qry.bcc != null)
                            {
                                var bcc = Qry.bcc.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                            getDefaultCC(Qry.Category).ForEach(mailid => { addcc.Add(mailid); });
                            ackmodel.PersonName = Qry.ProfessionalType + " " + Qry.CandidateName;

                            if (Qry.Status == "Open")
                                ackmodel.subject = "Relieving order for " + ackmodel.PersonName;
                            else if (Qry.Status == "Completed" && ((Qry.AppointmentType == 1 && context.tblRCTConsutantAppEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0) || (Qry.AppointmentType == 2 && context.tblRCTSTEEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0) || (Qry.AppointmentType == 3 && context.tblRCTOSGEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0)))
                                ackmodel.subject = "Relieving order / Service certificate for " + ackmodel.PersonName;
                            else
                                ackmodel.subject = "Service certificate for " + ackmodel.PersonName;
                            string ProjectNo = Common.getprojectnumber(Qry.ProjectId ?? 0);
                            ackmodel.toMail = Qry.Email;
                            ackmodel.cc = addcc;
                            ackmodel.DesignationName = Qry.PostRecommended;
                            ackmodel.ProjectNumber = ProjectNo;
                            ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentStartdate);
                            ackmodel.DAName = Common.GetUserFirstName(logged_in_userId);
                            ackmodel.RelievingMode = Qry.od.RelievingMode ?? 0;
                            emodel = ackmodel;
                            var bodyResp = _eb.RunCompile("RCTRelieveOrder.cshtml", "", ackmodel, typeof(NotePIModel));
                            if (bodyResp.Item1)
                            {
                                if (getbody_f)
                                    return Tuple.Create(-1, bodyResp.Item2);
                                string docName = "";
                                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                                List<ByteEmailAttachmentModel> listmodel = new List<ByteEmailAttachmentModel>();
                                RCTReportMasterController _reportC = new RCTReportMasterController();
                                if ((Qry.AppointmentType == 1 && context.tblRCTConsutantAppEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0) || (Qry.AppointmentType == 2 && context.tblRCTSTEEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0) || (Qry.AppointmentType == 3 && context.tblRCTOSGEmailLog.Where(x => x.OrderId == orderId && x.TypeofMail == 12).ToList().Count == 0))
                                {
                                    model = _reportC.PrintRelievingOrder(orderId);
                                    model.displayName = Qry.CandidateName + "_" + model.displayName;
                                    listmodel.Add(model);
                                    docName = model.actualName;
                                }
                                if (!string.IsNullOrEmpty(docName))
                                {
                                    var query = (from o in context.tblOrder
                                                 from od in context.tblOrderDetail
                                                 where o.OrderId == od.OrderId && o.OrderId == orderId
                                                 select new { od }).FirstOrDefault();
                                    if (query != null)
                                    {
                                        query.od.OfficeOrder = docName;
                                        query.od.OfficeOrderDate = DateTime.Now;
                                        context.SaveChanges();
                                    }
                                }

                                if (Qry.od.RelievingMode != 3 && Qry.Status == "Completed")
                                {
                                    model = _reportC.PrintServiceCertificate(orderId);
                                    model.displayName = Qry.CandidateName + "_" + model.displayName;
                                    listmodel.Add(model);
                                }
                                emodel.attachmentByte = listmodel;
                                var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                                if (Qry.Status == "Completed" || (Qry.Status == "Open" && Qry.od.RelievingMode == 3))
                                {
                                    var queryoffer = context.tblRCTOfferDetails.Where(m => m.OfferCategory == "Order" && m.OrderId == orderId && m.isSend != true).FirstOrDefault();
                                    if (queryoffer != null)
                                    {
                                        queryoffer.UPTD_USER = logged_in_userId;
                                        queryoffer.UPTD_TS = DateTime.Now;
                                        queryoffer.isSend = isSend;
                                        context.SaveChanges();
                                    }
                                }
                                if (Qry.AppointmentType == 1)
                                {
                                    tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 12;
                                    EmailStatus.ConsultantAppointmentId = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, bodyResp.Item2);
                                }
                                else if (Qry.AppointmentType == 2)
                                {
                                    tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 12;
                                    EmailStatus.STEID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTSTEEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, bodyResp.Item2);
                                }
                                else if (Qry.AppointmentType == 3)
                                {
                                    tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 12;
                                    EmailStatus.OSGID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTOSGEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, bodyResp.Item2);
                                }
                            }
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        //Send Order for extension,enhancement and amendment Manually trigger mail
        public static Tuple<int, string> SendOrder(int orderId, int logged_in_userId, bool isgetbody_f = false)
        {
            List<string> addcc = new List<string>();
            var res = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (orderId > 0)
                    {
                        if (context.tblRCTOfferDetails.Any(x => x.isSend == true && x.OfferCategory == "Order" && x.OrderId == orderId))
                            return Tuple.Create(0, "Order already sent");

                        var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                                   from P in context.tblProject
                                   where A.ProjectId == P.ProjectId && A.OrderId == orderId
                                   select new
                                   {
                                       A.ProfessionalType,
                                       A.ToMail,
                                       A.bcc,
                                       A.CandidateName,
                                       A.PIName,
                                       A.ApplicationType,
                                       A.Email,
                                       A.AppointmentType,
                                       A.ApplicationId,
                                       A.AppointmentStartdate,
                                       A.AppointmentEnddate,
                                       A.crtdUserId,
                                       P.ProjectNumber,
                                       A.OrderId,
                                       A.Category
                                   }).FirstOrDefault();
                        if (Qry != null)
                        {
                            EmailBuilder _eb = new EmailBuilder();
                            EmailModel emodel = new EmailModel();
                            NotePIModel ackmodel = new NotePIModel();
                            ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                            List<ByteEmailAttachmentModel> listmodel = new List<ByteEmailAttachmentModel>();
                            addcc.Add(Qry.ToMail);
                            if (Qry.bcc != null)
                            {
                                var bcc = Qry.bcc.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                            getDefaultCC(Qry.Category).ForEach(mailid => { addcc.Add(mailid); });
                            ackmodel.PersonName = Qry.ProfessionalType + " " + Qry.CandidateName;
                            ackmodel.subject = Qry.ApplicationType + " order for " + ackmodel.PersonName;
                            ackmodel.toMail = Qry.Email;
                            ackmodel.cc = addcc;
                            ackmodel.ProjectNumber = Qry.ProjectNumber;
                            ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentStartdate);
                            ackmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentEnddate);
                            ackmodel.DAName = Common.GetUserFirstName(logged_in_userId);
                            ackmodel.AppointmentType = Qry.ApplicationType;
                            ackmodel.PIName = Qry.PIName;

                            emodel = ackmodel;
                            var bodyResp = _eb.RunCompile("RCTSendOrderTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                            if (bodyResp.Item1)
                            {
                                if (isgetbody_f)
                                    return Tuple.Create(-1, bodyResp.Item2);
                                var OrderPath = "";
                                RCTReportMasterController _reportC = new RCTReportMasterController();
                                if (Qry.ApplicationType == "Extension")
                                {
                                    model = _reportC.GenerateExtensionOrder(orderId);
                                    OrderPath = model.actualName;
                                }
                                else if (Qry.ApplicationType == "Enhancement")
                                {
                                    model = _reportC.GenerateEnhancementOrder(orderId);
                                    OrderPath = model.actualName;
                                }
                                else if (Qry.ApplicationType == "Amendment" && Qry.Category != "OSG")
                                {
                                    model = _reportC.GenerateAmendmentOrder(orderId);
                                    OrderPath = model.actualName;
                                }
                                model.displayName = Qry.CandidateName + "_" + model.displayName;
                                listmodel.Add(model);
                                emodel.attachmentByte = listmodel;
                                if (!string.IsNullOrEmpty(OrderPath))
                                {
                                    var orderdetail = (from prj in context.tblOrderDetail
                                                       where prj.OrderId == orderId
                                                       select prj).FirstOrDefault();
                                    if (orderdetail != null)
                                    {
                                        orderdetail.OfficeOrderDate = DateTime.Now;
                                        orderdetail.OfficeOrder = OrderPath;
                                        context.SaveChanges();
                                    }
                                }
                                var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                                var queryoffer = context.tblRCTOfferDetails.Where(m => m.OfferCategory == "Order" && m.OrderId == orderId && m.isSend != true).FirstOrDefault();
                                if (queryoffer != null)
                                {
                                    queryoffer.UPTD_USER = logged_in_userId;
                                    queryoffer.UPTD_TS = DateTime.Now;
                                    queryoffer.isSend = isSend;
                                    context.SaveChanges();
                                }

                                if (Qry.AppointmentType == 1)
                                {
                                    tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 5;
                                    EmailStatus.ConsultantAppointmentId = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, "");
                                }
                                else if (Qry.AppointmentType == 2)
                                {
                                    tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 5;
                                    EmailStatus.STEID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTSTEEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, "");
                                }
                                else if (Qry.AppointmentType == 3)
                                {
                                    tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 5;
                                    EmailStatus.OSGID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTOSGEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return Tuple.Create(1, "");
                                }
                            }
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        public static bool SendSPLOPMail(int orderId, int logged_in_userId, bool isRelease = false)
        {
            List<string> addcc = new List<string>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                               from O in context.tblOrderDetail
                               where A.OrderId == O.OrderId && A.OrderId == orderId
                               select new
                               {
                                   A.bcc,
                                   A.ProfessionalType,
                                   A.ToMail,
                                   A.ApplicationType,
                                   A.AppointmentStartdate,
                                   A.AppointmentEnddate,
                                   A.CandidateName,
                                   A.crtdUserId,
                                   A.ProjectId,
                                   A.TypeofAppointment,
                                   A.BasicPay,
                                   A.PostRecommended,
                                   A.ApplicationId,
                                   A.Category,
                                   A.OrderId,
                                   O.RejoinDate
                               }).FirstOrDefault();
                    if (Qry != null)
                    {
                        EmailBuilder _eb = new EmailBuilder();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();

                        if (Qry.bcc != null)
                        {
                            var bcc = Qry.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(Qry.Category).ForEach(mailid => { addcc.Add(mailid); });
                        string ProjectNo = Common.getprojectnumber(Qry.ProjectId ?? 0);
                        ackmodel.PersonName = Qry.ProfessionalType + " " + Qry.CandidateName;
                        ackmodel.DesignationName = Qry.PostRecommended;
                        ackmodel.ProjectNumber = ProjectNo;
                        ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentStartdate);
                        ackmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentEnddate);
                        ackmodel.IsRelease = isRelease;
                        ackmodel.DAName = Common.GetUserFirstName(Qry.crtdUserId ?? 0);
                        ackmodel.Apptype = Qry.Category;
                        if (isRelease == true)
                        {
                            if (Qry.ApplicationType == "Stop Payment")
                            {
                                if (Qry.Category == "OSG")
                                {
                                    ackmodel.subject = "Stop payment for " + ackmodel.PersonName + "- Outsourcing";
                                    ackmodel.MailType = 10;
                                    ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.RejoinDate);
                                }
                                else
                                {
                                    ackmodel.subject = "Release of Salary for " + ackmodel.PersonName;
                                    ackmodel.MailType = 10;
                                    ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.RejoinDate);
                                }
                            }
                            else
                            {
                                ackmodel.subject = "Leave on Loss of Pay for " + ackmodel.PersonName;
                                ackmodel.MailType = 11;
                            }
                        }
                        else
                        {
                            ackmodel.subject = "Stop payment for " + ackmodel.PersonName;
                            ackmodel.MailType = 10;
                        }
                        ackmodel.toMail = Qry.ToMail;
                        ackmodel.cc = addcc;
                        emodel = ackmodel;
                        var bodyResp = _eb.RunCompile("RCTSPLOPTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            if (Qry.Category == "CON")
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = logged_in_userId;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = ackmodel.MailType;
                                EmailStatus.ConsultantAppointmentId = Qry.ApplicationId;
                                EmailStatus.OrderId = Qry.OrderId;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                                return true;
                            }
                            else if (Qry.Category == "STE")
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = logged_in_userId;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = ackmodel.MailType;
                                EmailStatus.STEID = Qry.ApplicationId;
                                EmailStatus.OrderId = Qry.OrderId;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                                return true;
                            }
                            else if (Qry.Category == "OSG")
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = logged_in_userId;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = ackmodel.MailType;
                                EmailStatus.OSGID = Qry.ApplicationId;
                                EmailStatus.OrderId = Qry.OrderId;
                                context.tblRCTOSGEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SendMaterbityLeaveMail(int orderId, int logged_in_userId, bool isRejoin = false)
        {
            List<string> addcc = new List<string>();
            List<string> AttachmentList = new List<string>();
            var res = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (orderId > 0)
                    {
                        var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                                   from O in context.tblOrderDetail
                                   where A.OrderId == O.OrderId && A.OrderId == orderId
                                   select new
                                   {
                                       A.bcc,
                                       A.ProfessionalType,
                                       A.ToMail,
                                       A.ApplicationType,
                                       A.AppointmentStartdate,
                                       A.AppointmentEnddate,
                                       A.CandidateName,
                                       A.crtdUserId,
                                       A.ProjectId,
                                       A.TypeofAppointment,
                                       A.BasicPay,
                                       A.PostRecommended,
                                       A.ApplicationId,
                                       A.Category,
                                       A.OrderId,
                                       O.RejoinDate
                                   }).FirstOrDefault();
                        if (Qry != null)
                        {
                            EmailBuilder _eb = new EmailBuilder();
                            EmailModel emodel = new EmailModel();
                            NotePIModel ackmodel = new NotePIModel();

                            if (Qry.bcc != null)
                            {
                                var bcc = Qry.bcc.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                            getDefaultCC(Qry.Category).ForEach(mailid => { addcc.Add(mailid); });
                            //emodel.toMail = getPIDetails(ProjectID ?? 0).Email;
                            string ProjectNo = Common.getprojectnumber(Qry.ProjectId ?? 0);
                            ackmodel.PersonName = Qry.ProfessionalType + " " + Qry.CandidateName;
                            ackmodel.DesignationName = Qry.PostRecommended;
                            ackmodel.ProjectNumber = ProjectNo;
                            ackmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentEnddate);
                            ackmodel.DAName = Common.GetUserFirstName(Qry.crtdUserId ?? 0);
                            ackmodel.Apptype = Qry.Category;
                            ackmodel.IsRelease = isRejoin;
                            if (isRejoin == true)
                            {
                                ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.RejoinDate);
                                if (Qry.Category == "OSG")
                                    ackmodel.subject = "Re-joining confirmation after maternity leave for " + ackmodel.PersonName + " - Outsourcing";
                                else
                                    ackmodel.subject = "Re-joining confirmation after maternity leave for " + ackmodel.PersonName;
                            }
                            else
                            {
                                ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentStartdate);

                                if (Qry.Category == "OSG")
                                    ackmodel.subject = "Maternity leave for " + ackmodel.PersonName + " - Outsourcing";
                                else
                                    ackmodel.subject = "Maternity leave for " + ackmodel.PersonName;
                            }
                            ackmodel.toMail = Qry.ToMail;
                            ackmodel.cc = addcc;
                            emodel = ackmodel;
                            var bodyResp = _eb.RunCompile("RCTMaternityleave.cshtml", "", ackmodel, typeof(NotePIModel));
                            if (bodyResp.Item1)
                            {
                                var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                                if (Qry.Category == "CON")
                                {
                                    tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 13;
                                    EmailStatus.ConsultantAppointmentId = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return true;
                                }
                                else if (Qry.Category == "STE")
                                {
                                    tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 13;
                                    EmailStatus.STEID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTSTEEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return true;
                                }
                                else if (Qry.Category == "OSG")
                                {
                                    tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                    EmailStatus.ToMail = emodel.toMail;
                                    EmailStatus.Subject = emodel.subject;
                                    EmailStatus.Body = bodyResp.Item2;
                                    if (emodel.cc != null)
                                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                                    else
                                        EmailStatus.Cc = "";
                                    if (emodel.bcc != null)
                                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                    else
                                        EmailStatus.Bcc = "";
                                    if (emodel.attachment != null)
                                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                    else
                                        EmailStatus.Attachment = "";
                                    EmailStatus.CRTD_Ts = DateTime.Now;
                                    EmailStatus.CRTD_By = logged_in_userId;
                                    EmailStatus.IsSend = isSend;
                                    EmailStatus.TypeofMail = 13;
                                    EmailStatus.OSGID = Qry.ApplicationId;
                                    EmailStatus.OrderId = Qry.OrderId;
                                    context.tblRCTOSGEmailLog.Add(EmailStatus);
                                    context.SaveChanges();
                                    return true;
                                }
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Tuple<int, string> SendVerificationReminder(int AppId, string Apptype, bool isBody = false, int? OrderId = null)
        {
            string res = string.Empty;
            int emailcount = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<CheckListEmailModel> chk = new List<CheckListEmailModel>();
                    var query = (from S in context.vw_RCTOverAllApplicationEntry
                                 from P in context.tblProject
                                 from vw in context.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == AppId
                                 && S.Category == Apptype && (S.OrderId == OrderId || OrderId == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     Name = S.ProfessionalType + " " + S.CandidateName,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     S.AppointmentStartdate,
                                     S.AppointmentEnddate,
                                     S.ApplicationType,
                                     S.Email,
                                     S.Category,
                                     S.PIName,
                                     S.crtdUserId,
                                     P.ProjectNumber
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        if (Apptype == "STE")
                        {
                            emailcount = (from el in context.tblRCTSTEEmailLog
                                          where el.TypeofMail == 9 && el.IsSend == true && el.STEID == AppId
                                          && (el.OrderId == OrderId || OrderId == null)
                                          select el).Count() + 1;
                        }
                        if (Apptype == "CON")
                        {
                            emailcount = (from el in context.tblRCTConsutantAppEmailLog
                                          where el.TypeofMail == 9 && el.IsSend == true && el.ConsultantAppointmentId == AppId
                                          && (el.OrderId == OrderId || OrderId == null)
                                          select el).Count() + 1;
                        }
                        else if (Apptype == "OSG")
                        {
                            emailcount = (from el in context.tblRCTOSGEmailLog
                                          where el.TypeofMail == 9 && el.IsSend == true && el.OSGID == AppId
                                          && (el.OrderId == OrderId || OrderId == null)
                                          select el).Count() + 1;
                        }

                        if (emailcount >= 3)
                            return Tuple.Create(emailcount, "Already - Final reminder sent to candidate");


                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        if (query.Email != null)
                            addcc.Add(query.Email);
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        npmodel.subject = "ICSR - Offer cancellation reminder email for " + query.Name;
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.ProjectNumber = query.ProjectNumber;
                        npmodel.ApplicationNumber = query.ApplicationNo;
                        npmodel.PersonName = query.Name;
                        npmodel.OrderDate = getOfferLetter(AppId, Apptype, OrderId);
                        npmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.OrderId = OrderId ?? 0;
                        npmodel.AppId = AppId;
                        npmodel.Apptype = Apptype;
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTVerificationReminder.cshtml", "", npmodel, typeof(NotePIModel));

                        if (isBody)
                            return Tuple.Create(-1, bodyResp.Item2);

                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            if (Apptype == "STE")
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                                var Userid = Common.GetUserid(userName);
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 9; //Remaindar mail Status
                                EmailStatus.STEID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();

                            }
                            else if (Apptype == "OSG")
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                                var Userid = Common.GetUserid(userName);
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 9; //Remaindar mail Status
                                EmailStatus.OSGID = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTOSGEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            else if (Apptype == "CON")
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                                var Userid = Common.GetUserid(userName);
                                EmailStatus.CRTD_By = Userid;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 9; //Remaindar mail Status
                                EmailStatus.ConsultantAppointmentId = query.ApplicationId;
                                EmailStatus.OrderId = query.OrderId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            if (emailcount == 1)
                                return Tuple.Create(emailcount, "Reminder mail sent to Candidate");
                            else if (emailcount == 2)
                                return Tuple.Create(emailcount, "Final reminder sent to Candidate");
                        }
                    }
                }
                return Tuple.Create(emailcount, "");

            }
            catch (Exception ex)
            {
                return Tuple.Create(emailcount, "");
            }
        }


        public static int SendTermEndMail(int AppId, string AppType, int UserID)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {


                    var blockQuery = (from c in context.tblRCTStopMail
                                      where c.ApplicationId == AppId && c.Appointmenttype == AppType
                                      && c.Status == "Active" && c.Respond_f != true
                                      select c).Any();
                    if (blockQuery)
                        return -1;

                    int Count = 0;
                    if (AppType == "STE")
                    {
                        Count = (from el in context.tblRCTSTEEmailLog
                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                 && el.STEID == AppId
                                 select el).Count() + 1;
                    }
                    if (AppType == "CON")
                    {
                        Count = (from el in context.tblRCTConsutantAppEmailLog
                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                 && el.ConsultantAppointmentId == AppId
                                 select el).Count() + 1;
                    }
                    if (AppType == "OSG")
                    {

                        Count = (from el in context.tblRCTOSGEmailLog
                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                 && el.OSGID == AppId
                                 select el).Count() + 1;
                    }
                    if (Count >= 4)
                        return Count + 1;

                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    var query = (from vw in context.vw_RCTOverAllApplicationEntry
                                 from p in context.tblProject
                                 where vw.ProjectId == p.ProjectId && vw.ApplicationId == AppId
                                 && vw.Category == AppType && vw.ApplicationType == "New"
                                 select new
                                 {
                                     vw.EmployeeNo,
                                     vw.PostRecommended,
                                     vw.BasicPay,
                                     vw.ProjectNumber,
                                     vw.ProjectId,
                                     vw.AppointmentEnddate,
                                     Name = vw.ProfessionalType + "" + vw.CandidateName,
                                     vw.ApplicationId,
                                     vw.Category,
                                     vw.bcc,
                                     vw.ToMail,
                                     vw.Email
                                 }).FirstOrDefault();


                    if (query != null)
                    {
                        int emailcount = 0;
                        if (query.Category == "STE")
                        {
                            emailcount = (from el in context.tblRCTSTEEmailLog
                                          where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                          && el.STEID == query.ApplicationId
                                          select el).Count() + 1;
                        }
                        if (query.Category == "CON")
                        {
                            emailcount = (from el in context.tblRCTConsutantAppEmailLog
                                          where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                          && el.ConsultantAppointmentId == query.ApplicationId
                                          select el).Count() + 1;
                        }
                        if (query.Category == "OSG")
                        {

                            emailcount = (from el in context.tblRCTOSGEmailLog
                                          where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                          && el.OSGID == query.ApplicationId
                                          select el).Count() + 1;
                        }

                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        getDefaultCC(query.Category).ForEach(mailid => { addcc.Add(mailid); });
                        var Reminder = "";
                        if (emailcount == 2)
                            npmodel.subject = "ICSR - Term end reminder of " + query.Name + " Reminder 2";
                        else if (emailcount == 1)
                            npmodel.subject = "ICSR - Term end reminder of " + query.Name + " Reminder 1";
                        else
                        {
                            npmodel.subject = "ICSR - Final term end reminder email for " + query.Name;
                            Reminder = "Reminder";
                            emailcount = 3;
                        }

                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.DesignationName = query.PostRecommended;
                        npmodel.PersonName = query.Name;
                        npmodel.ProjectNumber = query.ProjectNumber;
                        npmodel.EmployeeNum = query.EmployeeNo;
                        npmodel.BasicPay = string.Format(Indian, "{0:N0}", query.BasicPay);
                        npmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        var ProjectClouseDate = Common.GetProjectCloseDate(query.ProjectId ?? 0);
                        npmodel.ProjectEndDate = string.Format("{0:dd-MMMM-yyyy}", ProjectClouseDate);
                        npmodel.IITMExperiencedes = RequirementService.IITExperience(AppId, AppType, query.EmployeeNo);
                        npmodel.IITMExperience = RequirementService.IITExperienceInWording(query.EmployeeNo);
                        npmodel.DAName = Common.GetUserFirstName(UserID);
                        npmodel.Apptype = query.Category;
                        if (query.Category == "CON")
                        {
                            List<EmailAttachmentModel> AttachmentList = new List<EmailAttachmentModel>();
                            AttachmentList.Add(new EmailAttachmentModel
                            {
                                actualName = ChecklistPath + "Norms for Engagement of Consultant.pdf",
                                displayName = "Norms for Engagement of Consultant.pdf"
                            });
                            AttachmentList.Add(new EmailAttachmentModel
                            {
                                actualName = ChecklistPath + "CEI - Form A.pdf",
                                displayName = "CEI - Form A.pdf"
                            });
                            AttachmentList.Add(new EmailAttachmentModel
                            {
                                actualName = ChecklistPath + "CEI - Form B.pdf",
                                displayName = "CEI - Form B.pdf"
                            });
                            AttachmentList.Add(new EmailAttachmentModel
                            {
                                actualName = ChecklistPath + "CEI - Form C.pdf",
                                displayName = "CEI - Form C.pdf"
                            });
                            npmodel.attachmentlist = AttachmentList;
                        }
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTTermEndReminder.cshtml", "", npmodel, typeof(NotePIModel));
                        if (Reminder == "Reminder")
                        {
                            DateTime? ReminderDate1 = DateTime.Now;
                            DateTime? ReminderDate2 = DateTime.Now;

                            if (query.Category == "STE")
                            {
                                ReminderDate1 = (from el in context.tblRCTSTEEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.STEID == query.ApplicationId
                                                 select el.CRTD_Ts).FirstOrDefault();
                                ReminderDate2 = (from el in context.tblRCTSTEEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.STEID == query.ApplicationId
                                                 orderby el.EmailId descending
                                                 select el.CRTD_Ts).FirstOrDefault();
                            }
                            if (query.Category == "CON")
                            {

                                ReminderDate1 = (from el in context.tblRCTConsutantAppEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.ConsultantAppointmentId == query.ApplicationId
                                                 select el.CRTD_Ts).FirstOrDefault();
                                ReminderDate2 = (from el in context.tblRCTConsutantAppEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.ConsultantAppointmentId == query.ApplicationId
                                                 orderby el.ConAPPEmailId descending
                                                 select el.CRTD_Ts).FirstOrDefault();
                            }
                            if (query.Category == "OSG")
                            {


                                ReminderDate1 = (from el in context.tblRCTOSGEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.OSGID == query.ApplicationId
                                                 select el.CRTD_Ts).FirstOrDefault();

                                ReminderDate2 = (from el in context.tblRCTOSGEmailLog
                                                 where el.TypeofMail == 8 && el.IsSend == true && (el.Respondtermend_f == false || el.Respondtermend_f == null)
                                                 && el.OSGID == query.ApplicationId
                                                 orderby el.EmailId descending
                                                 select el.CRTD_Ts).FirstOrDefault();
                            }

                            npmodel.Reminder1 = string.Format("{0:dd-MMMM-yyyy}", ReminderDate1);
                            npmodel.Reminder2 = string.Format("{0:dd-MMMM-yyyy}", ReminderDate2);
                            bodyResp = _eb.RunCompile("RCTermEndFinalReminder.cshtml", "", npmodel, typeof(NotePIModel));
                        }
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);

                            if (AppType == "STE")
                            {
                                tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = UserID;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 8; //Remaindar mail Status
                                EmailStatus.STEID = AppId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTSTEEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            else if (AppType == "CON")
                            {
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = UserID;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 8; //Remaindar mail Status
                                EmailStatus.ConsultantAppointmentId = AppId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                            else if (AppType == "OSG")
                            {
                                tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = UserID;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 8; //Remaindar mail Status
                                EmailStatus.OSGID = AppId;
                                EmailStatus.IsManual_f = true;
                                context.tblRCTOSGEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                            }
                        }
                        res = emailcount;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }


        #region PayRoll

        public static int SendMailForPayrollAttachment(int Payrollid)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid
                                 select pah).FirstOrDefault();
                    if (query != null)
                    {
                        ReportsController RPT = new ReportsController();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        RCTReportMasterController rct = new RCTReportMasterController();
                        //attachementModel;
                        byte[] attach = rct.ExportAdhocPayrollDetailMail(Payrollid);
                        string disname = string.Empty;
                        string actname = string.Empty;
                        string salarytypename = string.Empty;
                        if (query.SalaryType == 1)
                        {
                            disname = "PAYROLL DATA Adhoc Main FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc Main FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "Main";
                        }
                        else if (query.SalaryType == 0)
                        {
                            disname = "PAYROLL DATA Adhoc Pensioner FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc Pensioner FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "Pensioner";
                        }
                        else
                        {
                            disname = "PAYROLL DATA Adhoc supplementary FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc supplementary FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "supplementary";
                        }
                        AttachmentList.Add(new ByteEmailAttachmentModel()
                        {
                            dataByte = attach,
                            displayName = disname,
                            actualName = actname
                        });
                        ackmodel.FillFields = salarytypename + " salary process for the month of " + query.SalaryMonth + " has been initiated and available for further process from your end.  Please find the enclosed salary excel report for the same.";
                        ackmodel.subject = "Adhoc Payroll Export Data Month " + query.SalaryMonth;
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        ackmodel.toMail = WebConfigurationManager.AppSettings["RCTPayrollToMail"];                        string ccMail = WebConfigurationManager.AppSettings["RCTSTEPayrollCCMail"];                        string[] arrCCMail = ccMail.Split(',');                        ackmodel.cc = new List<string>(arrCCMail);                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTAdhocPayrollAttachement.cshtml", "", ackmodel, typeof(NotePIModel));
                        emodel.attachmentByte = AttachmentList;
                        var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                        if (isSend == true)
                            res = 1;
                        else
                            res = 2;
                        tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                        EmailStatus.ToMail = emodel.toMail;
                        EmailStatus.Subject = emodel.subject;
                        EmailStatus.Body = bodyResp.Item2;
                        if (emodel.cc != null)
                            EmailStatus.Cc = string.Join(", ", emodel.cc);
                        else
                            EmailStatus.Cc = "";
                        if (emodel.bcc != null)
                            EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                        else
                            EmailStatus.Bcc = "";
                        EmailStatus.CRTD_Ts = DateTime.Now;
                        EmailStatus.CRTD_By = Userid;
                        EmailStatus.IsSend = isSend;
                        EmailStatus.TypeofMail = 15;
                        context.tblRCTSTEEmailLog.Add(EmailStatus);
                        context.SaveChanges();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res = -1;
            }
        }

        public static int SendMailStartProcessingPayrollAttachment(int Payrollid, byte[] attach)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid
                                 select pah).FirstOrDefault();
                    if (query != null)
                    {
                        ReportsController RPT = new ReportsController();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        RCTReportMasterController rct = new RCTReportMasterController();
                        //attachementModel;
                        string disname = string.Empty;
                        string actname = string.Empty;
                        string salarytypename = string.Empty;
                        if (query.SalaryType == 1)
                        {
                            disname = "PAYROLL DATA Adhoc Main FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc Main FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "Main";
                        }
                        else if(query.SalaryType == 0)
                        {
                            disname = "PAYROLL DATA Adhoc Pensioner FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc Pensioner FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "Pensioner";
                        }
                        else
                        {
                            disname = "PAYROLL DATA Adhoc supplementary FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            actname = "PAYROLL DATA Adhoc supplementary FOR THE MONTH OF " + query.SalaryMonth + ".xlsx";
                            salarytypename = "supplementary";
                        }
                        AttachmentList.Add(new ByteEmailAttachmentModel()
                        {
                            dataByte = attach,
                            displayName = disname,
                            actualName = actname
                        });
                        if (query.Status == "Requested for salary processing")
                            ackmodel.FillFields = salarytypename + " salary process for the month of " + query.SalaryMonth + " has been initiated and available for further process from your end.  Please find the enclosed salary excel report for the same.";
                        else if (query.Status == "Overwritten")
                            ackmodel.FillFields = "We have made changes for few records in the " + salarytypename + " salary for the month of " + query.SalaryMonth + " and the process is re-initiated for further process at your end. Please find the enclosed modified list for your reference.";
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.subject = "Adhoc Payroll Export Data Month " + query.SalaryMonth;
                        ackmodel.toMail = WebConfigurationManager.AppSettings["RCTPayrollToMail"];                        string ccMail = WebConfigurationManager.AppSettings["RCTSTEPayrollCCMail"];                        string[] arrCCMail = ccMail.Split(',');                        ackmodel.cc = new List<string>(arrCCMail);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTAdhocPayrollAttachement.cshtml", "", ackmodel, typeof(NotePIModel));

                        emodel.attachmentByte = AttachmentList;
                        var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                        if (isSend == true)
                            res = 1;
                        else
                            res = 2;
                        tblRCTSTEEmailLog EmailStatus = new tblRCTSTEEmailLog();
                        EmailStatus.ToMail = emodel.toMail;
                        EmailStatus.Subject = emodel.subject;
                        EmailStatus.Body = bodyResp.Item2;
                        if (emodel.cc != null)
                            EmailStatus.Cc = string.Join(", ", emodel.cc);
                        else
                            EmailStatus.Cc = "";
                        if (emodel.bcc != null)
                            EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                        else
                            EmailStatus.Bcc = "";
                        //EmailStatus.Attachment = 

                        EmailStatus.CRTD_Ts = DateTime.Now;
                        EmailStatus.CRTD_By = Userid;
                        EmailStatus.IsSend = isSend;
                        EmailStatus.TypeofMail = 15;
                        //EmailStatus.STEID = query.ApplicationId;
                        //EmailStatus.OrderId = query.OrderId;
                        context.tblRCTSTEEmailLog.Add(EmailStatus);
                        context.SaveChanges();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res = -1;
            }
        }

        public static int SendMailForPayrollOSGAccounts(int Payrollid, byte[] attach = null)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid
                                 select pah).FirstOrDefault();
                    var vendordetails = (from ven in context.tblSalaryAgencyMaster
                                         where ven.Status == "Active"
                                         select ven).FirstOrDefault();
                    if (query != null)
                    {
                        ReportsController RPT = new ReportsController();
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        RCTReportMasterController rct = new RCTReportMasterController();
                        //attachementModel;
                        if (attach == null)
                            attach = rct.DownloadExportOSGPayrollDetailMail(Payrollid);

                        AttachmentList.Add(new ByteEmailAttachmentModel()
                        {
                            dataByte = attach,
                            displayName = "PAYROLL DATA FOR THE OSG MONTH OF " + query.SalaryMonth + ".xls",
                            actualName = "PAYROLL DATA FOR THE OSG MONTH OF " + query.SalaryMonth + ".xls"
                        });
                        ackmodel.FillFields = "Please find the enclosed payroll data for the month " + query.SalaryMonth + " for your further process";
                        ackmodel.subject = "PAYROLL DATA FOR THE OSG MONTH OF " + query.SalaryMonth;
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);

                        ackmodel.toMail = WebConfigurationManager.AppSettings["RCTPayrollToMail"];                        string ccMail = WebConfigurationManager.AppSettings["RCTOSGPayrollCCMail"];                        string[] arrCCMail = ccMail.Split(',');                        ackmodel.cc = new List<string>(arrCCMail);
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTAdhocPayrollAttachement.cshtml", "", ackmodel, typeof(NotePIModel));

                        emodel.attachmentByte = AttachmentList;
                        var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                        if (isSend == true)
                            res = 1;
                        else
                            res = 2;
                        tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                        EmailStatus.ToMail = emodel.toMail;
                        EmailStatus.Subject = emodel.subject;
                        EmailStatus.Body = bodyResp.Item2;
                        if (emodel.cc != null)
                            EmailStatus.Cc = string.Join(", ", emodel.cc);
                        else
                            EmailStatus.Cc = "";
                        if (emodel.bcc != null)
                            EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                        else
                            EmailStatus.Bcc = "";
                        if (emodel.attachment != null)
                            EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                        else
                            EmailStatus.Attachment = "";
                        EmailStatus.CRTD_Ts = DateTime.Now;

                        EmailStatus.CRTD_By = Userid;
                        EmailStatus.IsSend = isSend;
                        EmailStatus.TypeofMail = 15;
                        //EmailStatus.OSGID = query.ApplicationId ?? 0;
                        context.tblRCTOSGEmailLog.Add(EmailStatus);
                        context.SaveChanges();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return -1;
            }
        }
        public static int SendMailForPayrollOSGVendor(int Payrollid)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var payQuery = (from pah in context.tblRCTPayroll
                                    where pah.RCTPayrollId == Payrollid
                                    select pah).FirstOrDefault();
                    //Mapping vendor id
                    var venQuery = (from ven in context.tblSalaryAgencyMaster
                                    where ven.SalaryAgencyId == 2
                                    select ven).FirstOrDefault();

                    if (payQuery != null && venQuery != null)
                    {
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        RCTReportMasterController rct = new RCTReportMasterController();
                        //attachementModel;
                        byte[] attach = rct.DownloadExportOSGPayrollDetailMail(Payrollid);

                        AttachmentList.Add(new ByteEmailAttachmentModel()
                        {
                            dataByte = attach,
                            displayName = "PAYROLL DATA FOR THE OSG MONTH OF " + payQuery.SalaryMonth + ".xls",
                            actualName = "PAYROLL DATA FOR THE OSG MONTH OF " + payQuery.SalaryMonth + ".xls"
                        });
                        ackmodel.FillFields = "Please find the enclosed payroll data for the month " + payQuery.SalaryMonth + " for your further process";
                        ackmodel.subject = "PAYROLL DATA FOR THE OSG MONTH OF " + payQuery.SalaryMonth;
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);

                        ackmodel.toMail = venQuery.ContactEmail;                        string ccMail = WebConfigurationManager.AppSettings["RCTOSGPayrollCCMail"];                        string[] arrCCMail = ccMail.Split(',');                        ackmodel.cc = new List<string>(arrCCMail);
                        if (!string.IsNullOrEmpty(venQuery.CCMail))
                        {
                            string[] arrVendorCCMail = venQuery.CCMail.Split(',');
                            new List<string>(arrVendorCCMail).ForEach(mailid =>
                            {
                                ackmodel.cc.Add(mailid);
                            });
                        }

                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTAdhocPayrollAttachement.cshtml", "", ackmodel, typeof(NotePIModel));

                        emodel.attachmentByte = AttachmentList;
                        var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                        if (isSend == true)
                            res = 1;
                        else
                            res = 2;
                        tblRCTOSGEmailLog EmailStatus = new tblRCTOSGEmailLog();
                        EmailStatus.ToMail = emodel.toMail;
                        EmailStatus.Subject = emodel.subject;
                        EmailStatus.Body = bodyResp.Item2;
                        if (emodel.cc != null)
                            EmailStatus.Cc = string.Join(", ", emodel.cc);
                        else
                            EmailStatus.Cc = "";
                        if (emodel.bcc != null)
                            EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                        else
                            EmailStatus.Bcc = "";
                        if (emodel.attachment != null)
                            EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                        else
                            EmailStatus.Attachment = "";
                        EmailStatus.CRTD_Ts = DateTime.Now;

                        EmailStatus.CRTD_By = Userid;
                        EmailStatus.IsSend = isSend;
                        EmailStatus.TypeofMail = 15;
                        //EmailStatus.OSGID = query.ApplicationId ?? 0;
                        context.tblRCTOSGEmailLog.Add(EmailStatus);
                        context.SaveChanges();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return -1;
            }
        }

        #endregion

        #region EmployeePortalEmail
        public static int SendMailProjectStaffNewuser(int ProjectStaffId, ICSRExternalEntities EmployeeContext, string toMail)
        {
            int res = 0;
            try
            {
                //using (EmployeeContext = new ICSRExternalEntities())
                //{
                var query = (from prjs in EmployeeContext.tblProjectStaffUser
                             where prjs.ProjectStaffId == ProjectStaffId
                             select prjs).FirstOrDefault();

                if (query != null)
                {
                    EmailModel emodel = new EmailModel();
                    NotePIModel ackmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                    ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                    //ackmodel.FillFields = ;
                    ackmodel.PersonName = query.Name;
                    ackmodel.UserName = query.UserName;
                    ackmodel.Password = query.Password;
                    ackmodel.subject = "Login credentials for the employee portal";
                    ackmodel.toMail = toMail;
                    string Category = query.UserName.Contains("IC") ? "STE" : query.UserName.Contains("VS") ? "OSG" : query.UserName.Contains("CS") ? "CON" : "";
                    getDefaultCC(Category).ForEach(mailid => { addcc.Add(mailid); });
                    ackmodel.cc = addcc;
                    emodel = ackmodel;
                    EmailBuilder _eb = new EmailBuilder();
                    var bodyResp = _eb.RunCompile("RCTEmployeeNewMail.cshtml", "", ackmodel, typeof(NotePIModel));

                    emodel.attachmentByte = AttachmentList;
                    var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                    if (isSend == true)
                        res = 1;
                    else
                        res = 2;
                    tblEmailLog EmailStatus = new tblEmailLog();
                    EmailStatus.ToMail = emodel.toMail;
                    EmailStatus.Subject = emodel.subject;
                    EmailStatus.Body = bodyResp.Item2;
                    if (emodel.cc != null)
                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                    else
                        EmailStatus.Cc = "";
                    if (emodel.bcc != null)
                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                    else
                        EmailStatus.Bcc = "";
                    if (emodel.attachment != null)
                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                    else
                        EmailStatus.Attachment = "";
                    EmailStatus.CRTD_Ts = DateTime.Now;
                    var userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var Userid = Common.GetUserid(userName);
                    EmailStatus.CRTD_By = Userid;
                    EmailStatus.IsSend = isSend;
                    EmailStatus.ProjectStaffId = ProjectStaffId;
                    EmployeeContext.tblEmailLog.Add(EmailStatus);
                    EmployeeContext.SaveChanges();
                }
                //}
                return res;
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return -1;
            }
        }
        public static int SendMailProjectStaffResetPassword(string EmployeeId, ICSRExternalEntities EmployeeContext, string toMail)
        {
            int res = 0;
            try
            {
                //using (EmployeeContext = new ICSRExternalEntities())
                //{
                var query = (from prjs in EmployeeContext.tblProjectStaffUser
                             where prjs.UserName == EmployeeId && prjs.Status != "InActive"
                             select prjs).FirstOrDefault();

                if (query != null)
                {
                    EmailModel emodel = new EmailModel();
                    NotePIModel ackmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                    ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                    //ackmodel.FillFields = "Your password has been reset successfully by the Administrator.";
                    ackmodel.UserName = query.UserName;
                    ackmodel.Password = query.Password;
                    ackmodel.PersonName = query.Name;
                    ackmodel.subject = "Login credentials for the employee portal";
                    string Category = query.UserName.Contains("IC") ? "STE" : query.UserName.Contains("VS") ? "OSG" : query.UserName.Contains("CS") ? "CON" : "";
                    getDefaultCC(Category).ForEach(mailid => { addcc.Add(mailid); });
                    ackmodel.cc = addcc;
                    ackmodel.toMail = toMail;
                    emodel = ackmodel;
                    EmailBuilder _eb = new EmailBuilder();
                    var bodyResp = _eb.RunCompile("RCTEmployeeNewuserMail.cshtml", "", ackmodel, typeof(NotePIModel));

                    emodel.attachmentByte = AttachmentList;
                    var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                    if (isSend == true)
                        res = 1;
                    else
                        res = 2;
                    tblEmailLog EmailStatus = new tblEmailLog();
                    EmailStatus.ToMail = emodel.toMail;
                    EmailStatus.Subject = emodel.subject;
                    EmailStatus.Body = bodyResp.Item2;
                    if (emodel.cc != null)
                        EmailStatus.Cc = string.Join(", ", emodel.cc);
                    else
                        EmailStatus.Cc = "";
                    if (emodel.bcc != null)
                        EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                    else
                        EmailStatus.Bcc = "";
                    if (emodel.attachment != null)
                        EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                    else
                        EmailStatus.Attachment = "";
                    EmailStatus.CRTD_Ts = DateTime.Now;
                    var userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var Userid = Common.GetUserid(userName);
                    EmailStatus.CRTD_By = Userid;
                    EmailStatus.IsSend = isSend;
                    EmailStatus.ProjectStaffId = query.ProjectStaffId;
                    EmployeeContext.tblEmailLog.Add(EmailStatus);
                    EmployeeContext.SaveChanges();
                }
                //}
                return res;
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return -1;
            }
        }
        #endregion

        #region CON Mail

        public static int AcknowledgementMailForCON(int appid, int userid)
        {
            int res = 0;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = new EmailModel();
                    NotePIModel npmodel = new NotePIModel();
                    List<string> addcc = new List<string>();
                    var query = (from A in context.tblRCTConsultantAppointment
                                 from D in context.tblRCTDesignation
                                 from C in context.tblCodeControl
                                 where A.DesignationId == D.DesignationId && A.ProfessionalType == C.CodeValAbbr
                                 && C.CodeName == "RCTProfessional" && A.ConsultantAppointmentId == appid
                                 select new
                                 {
                                     A.Bcc,
                                     A.ToMail,
                                     A.AppointmentStartdate,
                                     A.AppointmentEnddate,
                                     Name = C.CodeValDetail + " " + A.Name,
                                     A.CrtdUser,
                                     A.ProjectId,
                                     A.Salary,
                                     A.Fellowship,
                                     D.Designation,
                                     A.Email
                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        if (!string.IsNullOrEmpty(query.Bcc))
                            addcc = new List<string>(query.Bcc.Split(','));
                        npmodel.subject = "Consultant engagement status of " + query.Name;
                        npmodel.toMail = query.ToMail;
                        npmodel.cc = addcc;
                        npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentEnddate);
                        npmodel.PersonName = query.Name;
                        npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0); ;
                        npmodel.DAName = Common.GetUserFirstName(userid);
                        npmodel.DesignationName = query.Designation;
                        npmodel.BasicPay = string.Format(Indian, "{0:N0}", query.Salary);
                        emodel = npmodel;
                        var bodyResp = _eb.RunCompile("RCTCONAckTemplate.cshtml", "", npmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            if (emodel.attachment != null)
                                EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                            else
                                EmailStatus.Attachment = "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 1;
                            EmailStatus.ConsultantAppointmentId = appid;
                            context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                            context.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return res;
            }
        }

        public static Tuple<int, string> SendCONOfferLetter(int appid, bool getBoby_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {

                    var querycheck = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == appid && x.Category == "CON" && x.OfferCategory == "OfferLetter" && (x.OrderId == orderid || (x.OrderId == null && orderid == null)) && x.isSend == true).FirstOrDefault();
                    if (querycheck != null)
                        return Tuple.Create(0, "Offer Letter already sent");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == appid
                                 && S.Category == "CON" && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.CandidateName,
                                     S.ProfessionalType,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     P.ProjectNumber,
                                     S.crtdUserId,
                                     S.Email,
                                     S.PIName,
                                     vw.DepartmentName
                                 }).FirstOrDefault();

                    if (query != null)
                    {
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        ByteEmailAttachmentModel attachementModel = new ByteEmailAttachmentModel();
                        List<string> addcc = new List<string>();
                        List<ByteEmailAttachmentModel> AttachmentList = new List<ByteEmailAttachmentModel>();
                        if (!getBoby_f)
                        {
                            attachementModel = RCTReportMasterController.GenerateConsultantOfferLetter(appid, orderid ?? 0);
                            attachementModel.displayName = query.CandidateName + "_" + attachementModel.displayName;
                            AttachmentList.Add(attachementModel);
                        }
                        List<string> CheckList = new List<string>();
                        CheckList.Add(ChecklistPath + "Joining report for consultant.pdf");
                        ackmodel.attachment = CheckList;
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        ackmodel.subject = "Consultant Engagement letter for " + query.ProfessionalType + " " + query.CandidateName;
                        ackmodel.toMail = query.Email;
                        ackmodel.cc = addcc;
                        ackmodel.PersonName = query.ProfessionalType + " " + query.CandidateName;
                        ackmodel.ProjectNumber = query.ProjectNumber;
                        ackmodel.PIName = query.PIName;
                        ackmodel.toMail = query.ToMail;
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.PIName = query.PIName;
                        ackmodel.Department = query.DepartmentName;
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("ConsultantOfferLetter.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBoby_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            emodel.attachmentByte = AttachmentList;

                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                            var queryoffer = IOAScontext.tblRCTOfferDetails.Where(m => m.ApplicationId == appid && m.Category == "CON" && m.OfferCategory == "OfferLetter" && (m.OrderId == orderid || (orderid == null && m.OrderId == null)) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = Userid;
                                queryoffer.UPTD_TS = DateTime.Now;
                                queryoffer.isSend = isSend;
                                IOAScontext.SaveChanges();
                            }

                            var mastquery = (from s in IOAScontext.tblRCTConsultantAppointment where s.ConsultantAppointmentId == appid select s).FirstOrDefault();
                            if (mastquery != null && orderid == null)
                            {
                                mastquery.OfferLetter = attachementModel.actualName;
                                mastquery.OfferDate = DateTime.Now;
                                IOAScontext.SaveChanges();
                            }
                            else
                            {
                                var detquery = (from s in IOAScontext.tblOrderDetail where s.OrderId == orderid select s).FirstOrDefault();
                                if (detquery != null)
                                {
                                    detquery.OfferLetter = attachementModel.actualName;
                                    detquery.OfferDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            EmailStatus.Attachment = attachementModel.actualName != null ? attachementModel.actualName : "";
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = Userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 5;
                            EmailStatus.ConsultantAppointmentId = appid;
                            //EmailStatus.OrderId = query.OrderId;
                            IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, ex.Message);
            }
        }

        public static Tuple<int, string> SendCONOrder(int orderId, int logged_in_userId, bool isgetbody_f = false)
        {
            List<string> addcc = new List<string>();
            var res = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (orderId > 0)
                    {
                        if (context.tblRCTOfferDetails.Any(x => x.isSend == true && x.OfferCategory == "Order" && x.OrderId == orderId))
                            return Tuple.Create(0, "Order already sent");

                        var Qry = (from A in context.vw_RCTOverAllApplicationEntry
                                   from P in context.tblProject
                                   from vw in context.vwFacultyStaffDetails
                                   where P.PIName == vw.UserId && A.ProjectId == P.ProjectId && A.OrderId == orderId
                                   select new
                                   {
                                       A.ProfessionalType,
                                       A.ToMail,
                                       A.CandidateName,
                                       A.PIName,
                                       A.ApplicationType,
                                       A.Email,
                                       A.AppointmentType,
                                       A.ApplicationId,
                                       A.AppointmentStartdate,
                                       A.AppointmentEnddate,
                                       A.crtdUserId,
                                       P.ProjectNumber,
                                       A.OrderId,
                                       A.PostRecommended,
                                       vw.DepartmentName
                                   }).FirstOrDefault();
                        if (Qry != null)
                        {
                            EmailBuilder _eb = new EmailBuilder();
                            EmailModel emodel = new EmailModel();
                            NotePIModel ackmodel = new NotePIModel();
                            ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                            List<ByteEmailAttachmentModel> listmodel = new List<ByteEmailAttachmentModel>();
                            if (Qry.ToMail != null)
                            {
                                var bcc = Qry.ToMail.Split(',');
                                foreach (var bccEmail in bcc)
                                    addcc.Add(bccEmail.Trim());
                            }
                            ackmodel.PersonName = Qry.ProfessionalType + " " + Qry.CandidateName;
                            ackmodel.subject = Qry.ApplicationType + " order for " + ackmodel.PersonName;
                            if (Qry.ApplicationType == "Amendment")
                                ackmodel.subject = "Consultant amendment order for " + ackmodel.PersonName;
                            if (Qry.ApplicationType == "Relieving")
                                ackmodel.subject = "Relieving confirmation - " + ackmodel.PersonName;
                            ackmodel.toMail = Qry.Email;
                            ackmodel.cc = addcc;
                            ackmodel.ProjectNumber = Qry.ProjectNumber;
                            ackmodel.AppointmentStartDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentStartdate);
                            ackmodel.AppointmentEndDate = string.Format("{0:dd-MMMM-yyyy}", Qry.AppointmentEnddate);
                            ackmodel.DAName = Common.GetUserFirstName(logged_in_userId);
                            ackmodel.AppointmentType = Qry.ApplicationType;
                            ackmodel.PIName = Qry.PIName;
                            ackmodel.Department = Qry.DepartmentName;
                            ackmodel.DesignationName = Qry.PostRecommended;
                            ackmodel.ApplicationNumber = RequirementService.getOfferDetails(Qry.ApplicationId ?? 0, "CON", orderId);
                            var OfferDate = context.tblOrderDetail.Where(x => x.OrderId == orderId).Select(x => x.OfficeOrderDate).FirstOrDefault();
                            ackmodel.OrderDate = string.Format("{0:dd-MMMM-yyyy}", OfferDate ?? DateTime.Now);
                            emodel = ackmodel;
                            var bodyResp = _eb.RunCompile("RCTCONOrderTemplate.cshtml", "", ackmodel, typeof(NotePIModel));
                            if (bodyResp.Item1)
                            {
                                if (isgetbody_f)
                                    return Tuple.Create(-1, bodyResp.Item2);
                                var OrderPath = "";
                                RCTReportMasterController _reportC = new RCTReportMasterController();
                                if (Qry.ApplicationType == "Extension")
                                {
                                    model = _reportC.GenerateExtensionOrder(orderId);
                                    OrderPath = model.actualName;
                                }
                                else if (Qry.ApplicationType == "Enhancement")
                                {
                                    model = _reportC.GenerateEnhancementOrder(orderId);
                                    OrderPath = model.actualName;
                                }
                                else if (Qry.ApplicationType == "Amendment")
                                {
                                    model = _reportC.GenerateAmendmentOrder(orderId);
                                    OrderPath = model.actualName;
                                }

                                if (!string.IsNullOrEmpty(OrderPath))
                                {
                                    model.displayName = Qry.CandidateName + "_" + model.displayName;
                                    listmodel.Add(model);
                                    emodel.attachmentByte = listmodel;
                                    var orderdetail = (from prj in context.tblOrderDetail
                                                       where prj.OrderId == orderId
                                                       select prj).FirstOrDefault();
                                    if (orderdetail != null)
                                    {
                                        orderdetail.OfficeOrderDate = DateTime.Now;
                                        orderdetail.OfficeOrder = OrderPath;
                                        context.SaveChanges();
                                    }
                                }
                                var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);
                                var queryoffer = context.tblRCTOfferDetails.Where(m => m.OfferCategory == "Order" && m.OrderId == orderId && m.isSend != true).FirstOrDefault();
                                if (queryoffer != null)
                                {
                                    queryoffer.UPTD_USER = logged_in_userId;
                                    queryoffer.UPTD_TS = DateTime.Now;
                                    queryoffer.isSend = isSend;
                                    context.SaveChanges();
                                }
                                tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                                EmailStatus.ToMail = emodel.toMail;
                                EmailStatus.Subject = emodel.subject;
                                EmailStatus.Body = bodyResp.Item2;
                                if (emodel.cc != null)
                                    EmailStatus.Cc = string.Join(", ", emodel.cc);
                                else
                                    EmailStatus.Cc = "";
                                if (emodel.bcc != null)
                                    EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                                else
                                    EmailStatus.Bcc = "";
                                if (emodel.attachment != null)
                                    EmailStatus.Attachment = string.Join(", ", emodel.attachment);
                                else
                                    EmailStatus.Attachment = "";
                                EmailStatus.CRTD_Ts = DateTime.Now;
                                EmailStatus.CRTD_By = logged_in_userId;
                                EmailStatus.IsSend = isSend;
                                EmailStatus.TypeofMail = 5;
                                EmailStatus.ConsultantAppointmentId = Qry.ApplicationId;
                                EmailStatus.OrderId = Qry.OrderId;
                                context.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                                context.SaveChanges();
                                return Tuple.Create(1, "");
                            }
                        }
                    }
                }
                return Tuple.Create(0, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, "");
            }
        }

        public static Tuple<int, string> SendCONOfficeOrder(int appid, bool getBody_f = false, int? orderid = null)
        {
            int res = 0;
            try
            {
                using (var IOAScontext = new IOASDBEntities())
                {
                    var querycheck = IOAScontext.tblRCTOfferDetails.Where(x => x.ApplicationId == appid && x.Category == "CON" && x.OfferCategory == "OfficeOrder" && (x.OrderId == orderid || (x.OrderId == null && orderid == null)) && x.isSend == true).FirstOrDefault();
                    if (querycheck != null)
                        return Tuple.Create(0, "Office Order Already sent");

                    var query = (from S in IOAScontext.vw_RCTOverAllApplicationEntry
                                 from P in IOAScontext.tblProject
                                 from vw in IOAScontext.vwFacultyStaffDetails
                                 where P.PIName == vw.UserId && S.ProjectId == P.ProjectId && S.ApplicationId == appid
                                 && S.Category == "CON" && (S.OrderId == orderid || orderid == null)
                                 select new
                                 {
                                     S.OrderId,
                                     S.ApplicationId,
                                     S.AppointmentType,
                                     Name = S.ProfessionalType + " " + S.CandidateName,
                                     S.CandidateName,
                                     S.PostRecommended,
                                     S.ToMail,
                                     S.bcc,
                                     S.OrderNo,
                                     S.ApplicationNo,
                                     S.AppointmentStartdate,
                                     S.ApplicationType,
                                     S.Email,
                                     S.Category,
                                     S.PIName,
                                     S.crtdUserId,
                                 }).FirstOrDefault();

                    if (query != null)
                    {
                        EmailModel emodel = new EmailModel();
                        NotePIModel ackmodel = new NotePIModel();
                        List<string> addcc = new List<string>();
                        List<string> attachmentList = new List<string>();
                        ByteEmailAttachmentModel attachmodel = new ByteEmailAttachmentModel();
                        List<ByteEmailAttachmentModel> AttachList = new List<ByteEmailAttachmentModel>();
                        if (query.bcc != null)
                        {
                            var bcc = query.bcc.Split(',');
                            foreach (var bccEmail in bcc)
                                addcc.Add(bccEmail.Trim());
                        }
                        ackmodel.subject = "Engagement order for " + query.Name;
                        ackmodel.toMail = query.Email;
                        ackmodel.cc = addcc;
                        ackmodel.ApplicationNumber = RequirementService.getOfferDetails(query.ApplicationId ?? 0, query.Category, orderid, "OfferLetter");
                        ackmodel.toMail = query.ToMail;
                        ackmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", query.AppointmentStartdate);
                        var userName = System.Web.HttpContext.Current.User.Identity.Name;
                        var Userid = Common.GetUserid(userName);
                        ackmodel.DAName = Common.GetUserFirstName(Userid);
                        ackmodel.DesignationName = query.PostRecommended;
                        ackmodel.PIName = query.PIName;
                        emodel = ackmodel;
                        EmailBuilder _eb = new EmailBuilder();
                        var bodyResp = _eb.RunCompile("RCTCONOfficeOrder.cshtml", "", ackmodel, typeof(NotePIModel));
                        if (bodyResp.Item1)
                        {
                            if (getBody_f)
                                return Tuple.Create(-1, bodyResp.Item2);
                            RCTReportMasterController rptMast = new RCTReportMasterController();
                            attachmodel = rptMast.RCTOfficeOrderPrint(appid, "CON", orderid);
                            attachmodel.displayName = query.CandidateName + "_" + attachmodel.displayName;
                            AttachList.Add(attachmodel);
                            var orderdoc = attachmodel.actualName;
                            emodel.attachmentByte = AttachList;
                            var isSend = _eb.RCTSendEmail(emodel, bodyResp.Item2);


                            var queryoffer = IOAScontext.tblRCTOfferDetails.OrderByDescending(o => o.OfferDetailId).Where(m => m.ApplicationId == appid && m.Category == "CON" && m.OfferCategory == "OfficeOrder" && ((orderid == null && m.OrderId == null) || m.OrderId == orderid) && m.isSend != true).FirstOrDefault();
                            if (queryoffer != null)
                            {
                                queryoffer.UPTD_USER = Userid;
                                queryoffer.UPTD_TS = DateTime.Now;
                                queryoffer.isSend = isSend;
                                IOAScontext.SaveChanges();
                            }

                            if (query.Category == "CON" && query.OrderId == null)
                            {
                                var qry = (from s in IOAScontext.tblRCTConsultantAppointment
                                           where s.ConsultantAppointmentId == query.ApplicationId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }
                            else
                            {
                                var qry = (from s in IOAScontext.tblOrderDetail
                                           where s.OrderId == query.OrderId
                                           select s).FirstOrDefault();
                                if (qry != null)
                                {
                                    qry.OfficeOrder = orderdoc;
                                    qry.OfficeOrderDate = DateTime.Now;
                                    IOAScontext.SaveChanges();
                                }
                            }

                            tblRCTConsutantAppEmailLog EmailStatus = new tblRCTConsutantAppEmailLog();
                            EmailStatus.ToMail = emodel.toMail;
                            EmailStatus.Subject = emodel.subject;
                            EmailStatus.Body = bodyResp.Item2;
                            if (emodel.cc != null)
                                EmailStatus.Cc = string.Join(", ", emodel.cc);
                            else
                                EmailStatus.Cc = "";
                            if (emodel.bcc != null)
                                EmailStatus.Bcc = string.Join(", ", emodel.bcc);
                            else
                                EmailStatus.Bcc = "";
                            EmailStatus.Attachment = orderdoc;
                            EmailStatus.CRTD_Ts = DateTime.Now;
                            EmailStatus.CRTD_By = Userid;
                            EmailStatus.IsSend = isSend;
                            EmailStatus.TypeofMail = 16;
                            EmailStatus.ConsultantAppointmentId = query.ApplicationId;
                            EmailStatus.OrderId = query.OrderId;
                            IOAScontext.tblRCTConsutantAppEmailLog.Add(EmailStatus);
                            IOAScontext.SaveChanges();
                            res = 1;
                        }
                    }
                }
                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, ex.Message);
            }
        }

        #endregion

    }
}