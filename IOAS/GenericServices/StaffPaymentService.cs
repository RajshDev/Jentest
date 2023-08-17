using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using IOAS.Infrastructure;
using System.Data.Entity;
using System.Configuration;
using System.Data;
using System.Web.Configuration;

namespace IOAS.GenericServices
{

    public class StaffPaymentService
    {
        FinOp fo = new FinOp(DateTime.Now);
        CoreAccountsService coreAccounts = new CoreAccountsService();

        public List<PaymentTypeModel> GetPaymentType()
        {
            try
            {
                List<PaymentTypeModel> model = new List<PaymentTypeModel>();
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == "SalaryPaymentType"
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).ToList();
                    if (payType.Count > 0)
                    {
                        for (int i = 0; i < payType.Count; i++)
                        {
                            model.Add(new PaymentTypeModel
                            {
                                PaymentType = payType[i].CodeValDetail,
                                PaymentTypeId = payType[i].CodeValAbbr
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<EmpITDeclarationModel> GetITEmpDeclarations(string EMpId, int Finyearid)
        {
            try
            {
                var model = new List<EmpITDeclarationModel>();
                var searchData = new PagedData<EmpITDeclarationModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from AI in context.tblITDeclaration
                                 join E in context.tblEmpITDeclaration on new { DeclarationID = AI.DeclarationID, Id = EMpId, FinYearId = Finyearid }
                                 equals new { DeclarationID = E.DeclarationID ?? 0, Id = E.EmpId, FinYearId = Finyearid }
                                 into EmpDeclaration
                                 from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where (Emp.EmpId == EMpId ||(Emp == null))
                                 orderby AI.DeclarationID
                                 select new
                                 {
                                     AI.DeclarationID,
                                     AI.SNO,
                                     AI.SectionName,
                                     AI.SectionCode,
                                     AI.Particulars,
                                     AI.MaxLimit,
                                     AI.Age,
                                     AI.CreatedAt,
                                     AI.UpdatedAt,
                                     AI.CreatedBy,
                                     AI.UpdatedBy,
                                     SectionID = (Emp.SectionID == null ? 0 : Emp.SectionID),
                                     Emp.Amount
                                 });
                    var records = query.ToList();
                    if (records.Count == 0)
                    {
                        var qry = (from AI in context.tblITDeclaration
                                   orderby AI.DeclarationID
                                   select new
                                   {
                                       AI.DeclarationID,
                                       AI.SNO,
                                       AI.SectionName,
                                       AI.SectionCode,
                                       AI.Particulars,
                                       AI.MaxLimit,
                                       AI.Age,
                                       AI.CreatedAt,
                                       AI.UpdatedAt,
                                       AI.CreatedBy,
                                       AI.UpdatedBy
                                   });
                        var items = qry.ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            model.Add(new EmpITDeclarationModel
                            {
                                SectionID = 0,
                                DeclarationID = items[i].DeclarationID,
                                SectionName = Convert.ToString(items[i].SectionName),
                                SectionCode = Convert.ToString(items[i].SectionCode),
                                Particulars = items[i].Particulars,
                                MaxLimit = Convert.ToDecimal(items[i].MaxLimit),
                                Age = Convert.ToInt32(items[i].Age),
                                Amount = Convert.ToDecimal(0)

                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                    }

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITDeclarationModel
                            {
                                SectionID = records[i].SectionID,
                                DeclarationID = records[i].DeclarationID,
                                SectionName = Convert.ToString(records[i].SectionName),
                                SectionCode = Convert.ToString(records[i].SectionCode),
                                Particulars = records[i].Particulars,
                                MaxLimit = Convert.ToDecimal(records[i].MaxLimit),
                                Age = Convert.ToInt32(records[i].Age),
                                Amount = Convert.ToDecimal(records[i].Amount)

                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                        //recordCount = records.Count;
                        //searchData.Data = model;
                        //searchData.TotalRecords = records.Count;
                        //searchData.pageSize = records.Count;
                        //searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / searchData.pageSize));
                    }
                }
                //return searchData;

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpITOtherIncomeModel> GetITEmpOtherIncome(string EmpId, int Finyearid)
        {
            try
            {
                var model = new List<EmpITOtherIncomeModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from OI in context.tblEmpOtherIncome
                                     //join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                     //from Emp in EmpDeclaration.DefaultIfEmpty()
                                 where OI.EmpId == EmpId && OI.FinYearId == Finyearid
                                 orderby OI.EmpNo
                                 select new
                                 {
                                     OI.ID,
                                     OI.EmpNo,
                                     OI.Amount,
                                     OI.EligibleAmount,
                                     OI.SubmittedOn,
                                     OI.CreatedAt,
                                     OI.UpdatedAt,
                                     OI.CreatedBy,
                                     OI.UpdatedBy,
                                     OI.Particulars
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITOtherIncomeModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn),
                                Particulars = records[i].Particulars

                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpITOtherIncomeModel> GetITEmpOtherIncomecurrentFinYear(string EmpId)
        {
            try
            {
                var model = new List<EmpITOtherIncomeModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from OI in context.tblEmpOtherIncome
                                 join fn in context.tblFinYear on OI.FinYearId equals fn.FinYearId
                                 where OI.EmpId == EmpId && fn.CurrentYearFlag == true
                                 orderby OI.EmpNo
                                 select new
                                 {
                                     OI.ID,
                                     OI.EmpNo,
                                     OI.Amount,
                                     OI.EligibleAmount,
                                     OI.SubmittedOn,
                                     OI.CreatedAt,
                                     OI.UpdatedAt,
                                     OI.CreatedBy,
                                     OI.UpdatedBy,
                                     OI.Particulars
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITOtherIncomeModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn),
                                Particulars = records[i].Particulars

                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public string ITEmpDeclarationIU(EmpITDeductionModel EmpITmodel, int Finyearid)
        {
            try
            {
                string EmpId = EmpITmodel.EmpInfo.EmployeeID;
                List<EmpITDeclarationModel> ItModel = new List<EmpITDeclarationModel>();
                ItModel = EmpITmodel.ItList;
                using (var context = new IOASDBEntities())
                {
                    var remarkQry = context.tblITDeclarationRemarks
                     .FirstOrDefault(it => it.PayBill == EmpId);
                    if (remarkQry == null)
                    {
                        tblITDeclarationRemarks remark = new tblITDeclarationRemarks();
                        remark.Remarks = EmpITmodel.Remarks;
                        remark.PayBill = EmpId;
                        context.tblITDeclarationRemarks.Add(remark);
                        context.SaveChanges();
                    }
                    else
                    {
                        remarkQry.Remarks = EmpITmodel.Remarks;
                        remarkQry.PayBill = EmpId;
                        context.SaveChanges();
                    }
                    tblEmpITDeclaration taxExemp = new tblEmpITDeclaration();
                    int Secid = 0;
                    for (int i = 0; i < ItModel.Count; i++)
                    {
                        EmpITDeclarationModel model = new EmpITDeclarationModel();
                        model = ItModel[i];
                        model.EmpNo = EmpITmodel.EmpInfo.EmployeeID;

                        var SectionID = model.SectionID;
                        if (SectionID > 0)
                        {
                            var record = context.tblEmpITDeclaration
                            .FirstOrDefault(it => it.SectionID == SectionID && it.EmpId == EmpId && it.FinYearId == Finyearid);
                            if (record != null)
                            {
                                record.EmpId = EmpId;
                                record.DeclarationID = model.DeclarationID;
                                record.SectionName = model.SectionName;
                                record.SectionCode = model.SectionCode;
                                record.Particulars = model.Particulars;
                                record.MaxLimit = model.MaxLimit;
                                record.Amount = model.Amount;
                                record.Age = model.Age;
                                record.UpdatedAt = System.DateTime.Now;
                                record.UpdatedBy = model.UpdatedBy;
                                context.SaveChanges();
                            }
                            else
                            {
                                taxExemp.EmpNo = model.EmpNo;
                                taxExemp.DeclarationID = model.DeclarationID;
                                taxExemp.SectionName = model.SectionName;
                                taxExemp.SectionCode = model.SectionCode;
                                taxExemp.Particulars = model.Particulars;
                                taxExemp.MaxLimit = model.MaxLimit;
                                taxExemp.Age = model.Age;
                                taxExemp.Amount = model.Amount;
                                taxExemp.EmpId = EmpId;
                                taxExemp.CreatedAt = System.DateTime.Now;
                                taxExemp.CreatedBy = model.CreatedBy;
                                taxExemp.FinYearId = Finyearid;
                                context.tblEmpITDeclaration.Add(taxExemp);
                                context.SaveChanges();
                                Secid = taxExemp.SectionID;
                            }
                        }
                        else
                        {
                            taxExemp.EmpNo = model.EmpNo;
                            taxExemp.DeclarationID = model.DeclarationID;
                            taxExemp.SectionName = model.SectionName;
                            taxExemp.SectionCode = model.SectionCode;
                            taxExemp.Particulars = model.Particulars;
                            taxExemp.MaxLimit = model.MaxLimit;
                            taxExemp.Age = model.Age;
                            taxExemp.Amount = model.Amount;
                            taxExemp.EmpId = EmpId;
                            taxExemp.CreatedAt = System.DateTime.Now;
                            taxExemp.CreatedBy = model.CreatedBy;
                            taxExemp.FinYearId = Finyearid;
                            context.tblEmpITDeclaration.Add(taxExemp);
                            context.SaveChanges();
                            Secid = taxExemp.SectionID;
                        }
                    }
                    if (EmpITmodel.DocumentDetail != null)
                    {
                        foreach (var item in EmpITmodel.DocumentDetail)
                        {
                            if (item.DocumentFile != null)
                            {
                                var docQuery = context.tblEmpITDeclarationDoc.FirstOrDefault(m => m.DocumentId == item.DocumentDetailId);
                                if (docQuery == null)
                                {
                                    tblEmpITDeclarationDoc Doc = new tblEmpITDeclarationDoc();
                                    string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                    var guid = Guid.NewGuid().ToString();
                                    var docName = guid + "_" + actName;
                                    item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/CoGetITExemptionntent/OtherDocuments/" + docName));
                                    Doc.Documentpath = actName;
                                    Doc.DocumentName = docName;
                                    Doc.DeclarationID = item.DocumentType;
                                    Doc.EmpId = EmpId;
                                    Doc.FinYearId = Finyearid;
                                    context.tblEmpITDeclarationDoc.Add(Doc);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    if (item.DocumentFile != null)
                                    {
                                        string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                        var guid = Guid.NewGuid().ToString();
                                        var docName = guid + "_" + actName;
                                        item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/" + docName));
                                        docQuery.Documentpath = actName;
                                        docQuery.DocumentName = docName;
                                    }
                                    docQuery.DeclarationID = item.DocumentType;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    var rec = context.tblEmpITDeclaration
                    .FirstOrDefault(it => it.EmpId == EmpId && it.FinYearId == Finyearid);
                    if (rec == null)
                    {
                        if (EmpITmodel.ItOtherIncome != null)
                        {
                            foreach (var incomItem in EmpITmodel.ItOtherIncome)
                            {
                                tblEmpOtherIncome incom = new tblEmpOtherIncome();
                                incom.EmpId = EmpId;
                                incom.Particulars = incomItem.Particulars;
                                incom.Amount = incomItem.Amount;
                                incom.Remarks = incomItem.Remarks;
                                incom.FinYearId = Finyearid;
                                context.tblEmpOtherIncome.Add(incom);
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        context.tblEmpOtherIncome.RemoveRange(context.tblEmpOtherIncome.Where(m => m.EmpId == EmpId && m.FinYearId == Finyearid));
                        context.SaveChanges();
                        if (EmpITmodel.ItOtherIncome != null)
                        {
                            foreach (var incomItem in EmpITmodel.ItOtherIncome)
                            {
                                tblEmpOtherIncome incom = new tblEmpOtherIncome();
                                incom.EmpId = EmpId;
                                incom.Particulars = incomItem.Particulars;
                                incom.Amount = incomItem.Amount;
                                incom.Remarks = incomItem.Remarks;
                                incom.FinYearId = Finyearid;
                                context.tblEmpOtherIncome.Add(incom);
                                context.SaveChanges();
                            }
                        }
                    }
                    context.Dispose();

                }
                string msg = "IT declaration updated successfully";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
        public List<tblCodeControl> GetStatusFields(string CodeName)
        {
            try
            {
                List<tblCodeControl> model = new List<tblCodeControl>();
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == CodeName
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).ToList();
                    if (payType.Count > 0)
                    {
                        for (int i = 0; i < payType.Count; i++)
                        {
                            model.Add(new tblCodeControl
                            {
                                CodeValDetail = payType[i].CodeValDetail,
                                CodeValAbbr = payType[i].CodeValAbbr
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public decimal GetITExemption(string EmpId)        {            try            {                decimal Total = 0;                using (var context = new IOASDBEntities())                {                    var EightyCTotal = (from E in context.tblEmpITDeclaration                                        join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID                                        where E.EmpId == EmpId && (IT.SectionCode == "80C" || IT.SectionCode == "80CCC")                                        select new                                        {                                            E.EmpNo,                                            E.Amount,                                            E.MaxLimit                                        }).Sum(i => i.Amount);                    var NonEightyCTotal = (from E in context.tblEmpITDeclaration                                           join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID                                           where E.EmpId == EmpId && (IT.SectionCode != "80C" && IT.SectionCode != "80CCC")                                           select new                                           {                                               E.EmpNo,                                               E.Amount,                                               E.MaxLimit                                           }).Sum(i => i.Amount);                    if (EightyCTotal > 150000)                    {                        Total = 150000 + Convert.ToDecimal(NonEightyCTotal);                    }                    else                    {                        Total = Convert.ToDecimal(EightyCTotal) + Convert.ToDecimal(NonEightyCTotal);                    }                }                Total += Convert.ToDecimal(WebConfigurationManager.AppSettings["Adhoc_Common_Exemption"]);                return Total;            }            catch (Exception ex)            {                Console.WriteLine(ex.ToString());                return 0;            }        }

        public decimal GetITExemptionCurrentFinyear(string EmpId)        {            try            {                decimal Total = 0;                //using (var context = new IOASDBEntities())                //{                //    var EightyCTotal = (from E in context.tblEmpITDeclaration                //                        join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID                //                        join Fn in context.tblFinYear on E.FinYearId equals Fn.FinYearId                //                        where E.EmpId == EmpId && Fn.CurrentYearFlag == true && (IT.SectionCode == "80C" || IT.SectionCode == "80CCC")                //                        select new                //                        {                //                            E.EmpNo,                //                            E.Amount,                //                            E.MaxLimit                //                        }).Sum(i => i.Amount);                //    var NonEightyCTotal = (from E in context.tblEmpITDeclaration                //                           join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID                //                           join Fn in context.tblFinYear on E.FinYearId equals Fn.FinYearId                //                           where E.EmpId == EmpId && Fn.CurrentYearFlag == true && (IT.SectionCode != "80C" && IT.SectionCode != "80CCC")                //                           select new                //                           {                //                               E.EmpNo,                //                               E.Amount,                //                               E.MaxLimit                //                           }).Sum(i => i.Amount);                //    if (EightyCTotal > 150000)                //    {                //        Total = 150000 + Convert.ToDecimal(NonEightyCTotal);                //    }                //    else                //    {                //        Total = Convert.ToDecimal(EightyCTotal) + Convert.ToDecimal(NonEightyCTotal);                //    }                //}                Total += Convert.ToDecimal(WebConfigurationManager.AppSettings["Adhoc_Common_Exemption"]);                return Total;            }            catch (Exception ex)            {                Console.WriteLine(ex.ToString());                return 0;            }        }

        public List<EmpITSOPModel> GetITEmpSOP()
        {
            try
            {
                var model = new List<EmpITSOPModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from sop in context.tblEmpSOPIncome
                                     //join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                     //from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where sop.EmpNo == empNo
                                 orderby sop.EmpNo
                                 select new
                                 {
                                     sop.ID,
                                     sop.EmpNo,
                                     sop.LenderName,
                                     sop.LenderPAN,
                                     sop.Amount,
                                     sop.EligibleAmount,
                                     sop.SubmittedOn,
                                     sop.CreatedAt,
                                     sop.UpdatedAt,
                                     sop.CreatedBy,
                                     sop.UpdatedBy
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITSOPModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                LenderName = Convert.ToString(records[i].LenderName),
                                LenderPAN = records[i].LenderPAN,
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpITOtherIncomeModel> GetITEmpOtherIncome()
        {
            try
            {
                var model = new List<EmpITOtherIncomeModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from OI in context.tblEmpOtherIncome
                                     //join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                     //from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where sop.EmpNo == empNo
                                 orderby OI.EmpNo
                                 select new
                                 {
                                     OI.ID,
                                     OI.EmpNo,
                                     OI.Amount,
                                     OI.EligibleAmount,
                                     OI.SubmittedOn,
                                     OI.CreatedAt,
                                     OI.UpdatedAt,
                                     OI.CreatedBy,
                                     OI.UpdatedBy
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITOtherIncomeModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public string ITDeclarationIU(ITDeclarationModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    tblITDeclaration taxExemp = new tblITDeclaration();
                    var DeclarationID = model.DeclarationID;
                    if (DeclarationID > 0)
                    {
                        var record = context.tblITDeclaration.SingleOrDefault(it => it.DeclarationID == DeclarationID);

                        record.SectionName = model.SectionName;
                        record.SectionCode = model.SectionCode;
                        record.Particulars = model.Particulars;
                        record.MaxLimit = model.MaxLimit;
                        record.Age = model.Age;
                        record.UpdatedAt = System.DateTime.Now;
                        record.UpdatedBy = model.UpdatedBy;
                    }
                    else
                    {
                        taxExemp.SectionName = model.SectionName;
                        taxExemp.SectionCode = model.SectionCode;
                        taxExemp.Particulars = model.Particulars;
                        taxExemp.MaxLimit = model.MaxLimit;
                        taxExemp.Age = model.Age;
                        taxExemp.CreatedAt = System.DateTime.Now;
                        taxExemp.CreatedBy = model.CreatedBy;
                        context.tblITDeclaration.Add(taxExemp);
                    }
                    context.SaveChanges();
                    context.Dispose();
                }
                string msg = "";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
        public PagedData<ITDeclarationModel> GetITDeclarations()
        {
            try
            {
                var model = new List<ITDeclarationModel>();
                var searchData = new PagedData<ITDeclarationModel>();
                int recordCount = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from AI in context.tblITDeclaration
                                 join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                 from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where Emp.DeclarationID == AI.DeclarationID
                                 orderby AI.DeclarationID
                                 select new
                                 {
                                     AI.DeclarationID,
                                     AI.SNO,
                                     AI.SectionName,
                                     AI.SectionCode,
                                     AI.Particulars,
                                     AI.MaxLimit,
                                     AI.Age,
                                     AI.CreatedAt,
                                     AI.UpdatedAt,
                                     AI.CreatedBy,
                                     AI.UpdatedBy,
                                     Emp.Amount
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new ITDeclarationModel
                            {
                                DeclarationID = records[i].DeclarationID,
                                SNO = Convert.ToInt32(records[i].SNO),
                                SectionName = Convert.ToString(records[i].SectionName),
                                SectionCode = Convert.ToString(records[i].SectionCode),
                                Particulars = records[i].Particulars,
                                MaxLimit = Convert.ToDecimal(records[i].MaxLimit),
                                Age = Convert.ToInt32(records[i].Age)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                        recordCount = records.Count;
                        searchData.Data = model;
                        searchData.TotalRecords = records.Count;
                        searchData.pageSize = records.Count;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / searchData.pageSize));
                    }
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public EmployeeDetailsModel GetEmpInfo(string EmpNo)
        {
            try
            {
                EmployeeDetailsModel model = new EmployeeDetailsModel();
                using (var context = new IOASDBEntities())
                {
                    var record = (from AI in context.VWAppointInfo
                                  where AI.EmployeeID == EmpNo
                                  orderby AI.EmployeeID
                                  select new
                                  {
                                      AI.EmployeeID,
                                      AI.EmployeeName,
                                      AI.MeetingID,
                                      AI.CandidateID,
                                      AI.DOB,
                                      AI.DesignationCode,
                                      AI.DesignationName,
                                      AI.AppointmentDate,
                                      AI.ToDate,
                                      AI.RelieveDate,
                                      AI.BasicSalary,
                                      AI.PermanentAddress,
                                      AI.CommunicationAddress,
                                      AI.MobileNumber,
                                      AI.EmailID,
                                      AI.BankName,
                                      AI.BranchName,
                                      AI.BankAccountNo,
                                      AI.IFSC_Code,
                                      AI.OutSourcingCompany,
                                      AI.OrderID,
                                      AI.OrderType,
                                      AI.ProjectNo,
                                      AI.FromDate,
                                      AI.DetailToDate,
                                      AI.GrossSalary,
                                      AI.CostToProject,
                                      AI.CommitmentNo,
                                      AI.Remarks
                                  }).SingleOrDefault();
                    if (record != null)
                    {
                        model.EmployeeID = record.EmployeeID;
                        model.EmployeeName = record.EmployeeName;
                        model.MeetingID = Convert.ToString(record.MeetingID);
                        model.CandidateID = Convert.ToString(record.CandidateID);
                        model.DesignationCode = record.DesignationCode;
                        model.DesignationName = record.DesignationName;
                        model.AppointmentDate = record.AppointmentDate;
                        model.ToDate = record.ToDate;
                        model.RelieveDate = Convert.ToDateTime(record.RelieveDate);
                        model.BasicSalary = record.BasicSalary;
                        model.PermanentAddress = record.PermanentAddress;
                        model.BankName = record.BankName;
                        model.BranchName = record.BranchName;
                        model.BankAccountNo = record.BankAccountNo;
                        model.IFSC_Code = record.IFSC_Code;
                        model.OutSourcingCompany = record.OutSourcingCompany;
                        model.OrderID = record.OrderID;
                        model.OrderType = record.OrderType;
                        model.ProjectNo = record.ProjectNo;
                        model.FromDate = record.FromDate;
                        model.DetailToDate = record.DetailToDate;
                        model.GrossSalary = record.GrossSalary;
                        model.CostToProject = record.CostToProject;
                        model.CommitmentNo = record.CommitmentNo;
                        model.Remarks = record.Remarks;
                        model.EmailID = record.EmailID;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedData<SalaryModel> GetEmployeeSalary(int page, int pageSize, int PaymentHeadId)
        {
            try
            {
                var searchData = new PagedData<SalaryModel>();
                List<SalaryModel> model = new List<SalaryModel>();
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
                    var query = (from sm in context.tblSalaryPayment
                                 join AI in context.vw_RCTAdhocEmployeeMaster on sm.PayBill equals AI.EmployeeId
                                 join CC in context.tblCodeControl on sm.ModeOfPayment equals CC.CodeValAbbr
                                 where sm.PaymentHeadId == PaymentHeadId && CC.CodeName == "SalaryPaymentType"
                                 orderby sm.EmployeeId
                                 select new
                                 {
                                     EmployeeName = AI.NAME,
                                     sm.PaymentId,
                                     sm.EmployeeId,
                                     sm.EmpNo,
                                     sm.Basic,
                                     sm.HRA,
                                     sm.MA,
                                     sm.DA,
                                     sm.Conveyance,
                                     sm.Deduction,
                                     sm.Tax,
                                     sm.ProfTax,
                                     sm.TaxableIncome,
                                     sm.NetSalary,
                                     sm.MonthSalary,
                                     sm.MonthlyTax,
                                     sm.AnnualSalary,
                                     sm.AnnualExemption,
                                     sm.PaidDate,
                                     sm.PaymentMonthYear,
                                     sm.PaymentCategory,
                                     sm.PaidAmount,
                                     sm.Status,
                                     sm.IsPaid,
                                     sm.ModeOfPayment,
                                     sm.TypeOfPayBill,
                                     CC.CodeValDetail
                                 });

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new SalaryModel
                            {
                                EmployeeId = records[i].EmployeeId,
                                EmployeeName = records[i].EmployeeName,
                                PaymentId = records[i].PaymentId,
                                Basic = Convert.ToDecimal(records[i].Basic),
                                HRA = Convert.ToDecimal(records[i].HRA),
                                MA = Convert.ToDecimal(records[i].MA),
                                DA = Convert.ToDecimal(records[i].DA),
                                Conveyance = Convert.ToDecimal(records[i].Conveyance),
                                Deduction = Convert.ToDecimal(records[i].Deduction),
                                Tax = Convert.ToDecimal(records[i].Tax),
                                ProfTax = Convert.ToDecimal(records[i].Basic),
                                TaxableIncome = Convert.ToDecimal(records[i].TaxableIncome),
                                NetSalary = Convert.ToDecimal(records[i].NetSalary),
                                MonthSalary = Convert.ToDecimal(records[i].MonthSalary),
                                MonthlyTax = Convert.ToDecimal(records[i].MonthlyTax),
                                AnnualSalary = Convert.ToDecimal(records[i].AnnualSalary),
                                AnnualExemption = Convert.ToDecimal(records[i].AnnualExemption),
                                PaidAmount = Convert.ToDecimal(records[i].PaidAmount),
                                PaidDate = Convert.ToDateTime(records[i].PaidDate),
                                ModeOfPayment = Convert.ToInt32(records[i].ModeOfPayment),
                                ModeOfPaymentName = records[i].CodeValDetail,
                                Status = records[i].Status
                            });
                        }

                        searchData.Data = model;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }

                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<ProjectCommitmentModel> GetProjectCommitment(string PaymentMonthYear, int PaymentHeadId)
        {
            try
            {

                var model = new List<ProjectCommitmentModel>();

                using (var context = new IOASDBEntities())
                {


                    var query = (from SP in context.tblSalaryPayment
                                 join Emp in context.vw_RCTAdhocEmployeeMaster on SP.PayBill equals Emp.EmployeeId
                                 //join Com in context.tblCommitment on Emp.commitmentNo equals Com.CommitmentNumber 
                                 //from C in (from CM in Commit
                                 //           where CM.CommitmentNumber == Emp.commitmentNo
                                 //           select CM).DefaultIfEmpty()
                                 where SP.PaymentMonthYear == PaymentMonthYear && SP.PaymentHeadId == PaymentHeadId
                                 group SP by new
                                 {
                                     ProjectNo = SP.ProjectNo,
                                     CommitmentNo = Emp.commitmentNo,
                                     SP.Basic,
                                     SP.Conveyance,
                                     SP.DA,
                                     SP.HRA,
                                     SP.MA,
                                     SP.EmployeeId
                                 } into Sal
                                 select new
                                 {
                                     Sal.Key.ProjectNo,
                                     Sal.Key.CommitmentNo,
                                     //SalaryPaid = Sal.Sum(i => (i.Basic + i.Conveyance + i.DA + i.HRA + i.MA))
                                     SalaryPaid = Sal.Sum(i => (i.NetSalary - i.OtherAllowance))
                                 });
                    var records = query.ToList();
                    decimal balanceAfter = 0;
                    decimal SalaryToBePaid = 0;
                    decimal CurrentBalance = 0;
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            var commitmentNo = string.IsNullOrEmpty(records[i].CommitmentNo) ? "" : records[i].CommitmentNo;
                            var commitment = (from C in context.tblCommitment
                                              join CD in context.tblCommitmentDetails on C.CommitmentId equals CD.CommitmentId

                                              where C.CommitmentNumber == commitmentNo && C.Status == "Active"
                                              select new
                                              {
                                                  C.CommitmentNumber,
                                                  CommitmentAmount = CD.Amount,
                                                  CommitmentBalance = CD.BalanceAmount
                                              }).SingleOrDefault();
                            if (commitment != null)
                            {
                                CurrentBalance = Convert.ToDecimal(commitment.CommitmentBalance);
                                SalaryToBePaid = Convert.ToDecimal(records[i].SalaryPaid);
                                balanceAfter = CurrentBalance - SalaryToBePaid;
                            }
                            else
                            {
                                CurrentBalance = 0;
                                SalaryToBePaid = Convert.ToDecimal(records[i].SalaryPaid);
                                balanceAfter = CurrentBalance - SalaryToBePaid;
                            }

                            model.Add(new ProjectCommitmentModel
                            {
                                MakePayment = true,
                                ProjectNo = records[i].ProjectNo,
                                CommitmentNo = records[i].CommitmentNo,
                                SalaryToBePaid = SalaryToBePaid,
                                CurrentBalance = CurrentBalance,
                                BalanceAfter = balanceAfter,
                                IsBalanceAavailable = (balanceAfter > 0)

                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public PagedData<ProjectCommitmentModel> GetProjectCommitment(int PaymentHeadId, int pageIndex, int pageSize, string commitmentNo, string projectNo)
        {

            var searchData = new PagedData<ProjectCommitmentModel>();
            var model = new List<ProjectCommitmentModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    int skiprec = 0;

                    if (pageIndex == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (pageIndex - 1) * pageSize;
                    }
                    var predicate = PredicateBuilder.BaseAnd<ProjectCommitmentModel>();
                    if (!string.IsNullOrEmpty(commitmentNo))
                        predicate = predicate.And(d => d.CommitmentNo.Contains(commitmentNo));
                    if (!string.IsNullOrEmpty(projectNo))
                        predicate = predicate.And(d => d.ProjectNo.Contains(projectNo));
                    var query = (from c in context.tblAdhocSalaryCommitmentDetail
                                 join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                 //join SP in context.tblSalaryPayment on c.PaymentId equals SP.PaymentId
                                 join SP in context.tblProject on com.ProjectId equals SP.ProjectId
                                 where c.PaymentHeadId == PaymentHeadId && c.Status == "Active"
                                 group new
                                 {
                                     SP,
                                     com,
                                     c
                                 } by new { com.CommitmentNumber, SP.ProjectNumber, com.CommitmentBalance } into g
                                 //group c by c.CommitmentDetailId into g
                                 select new ProjectCommitmentModel
                                 {
                                     CommitmentNo = g.Key.CommitmentNumber,
                                     ProjectNo = g.Key.ProjectNumber,
                                     CommitmentBalance = g.Key.CommitmentBalance,
                                     Amount = g.Select(m => m.c.Amount).Sum(),
                                     BalanceAfter = g.Key.CommitmentBalance - g.Select(m => m.c.Amount).Sum() ?? 0
                                 });
                    var records = query.Where(predicate).OrderBy(m => m.BalanceAfter)
                        .AsEnumerable()
                        .Select((x, index) => new ProjectCommitmentModel()
                        {
                            SlNo = index + 1,
                            MakePayment = true,
                            CommitmentNo = x.CommitmentNo,
                            ProjectNo = x.ProjectNo,
                            CurrentBalance = x.CommitmentBalance ?? 0,
                            SalaryToBePaid = x.Amount ?? 0,
                            BalanceAfter = x.CommitmentBalance - x.Amount ?? 0,
                            IsBalanceAavailable = (x.CommitmentBalance - x.Amount) > 0
                        }).Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.Where(predicate).ToList().Count();
                    //decimal balanceAfter = 0;
                    //decimal bookedValue = 0;
                    //decimal CurrentBalance = 0;
                    //if (records.Count > 0)
                    //{
                    //    for (int i = 0; i < records.Count; i++)
                    //    {
                    //        CurrentBalance = Convert.ToDecimal(records[i].CommitmentBalance);
                    //        bookedValue = Convert.ToDecimal(records[i].Amount);
                    //        balanceAfter = CurrentBalance - bookedValue;

                    //        model.Add(new ProjectCommitmentModel
                    //        {
                    //            SlNo = skiprec + i + 1,
                    //            MakePayment = true,
                    //            ProjectNo = records[i].ProjectNo,
                    //            CommitmentNo = records[i].CommitmentNo,
                    //            SalaryToBePaid = bookedValue,
                    //            CurrentBalance = CurrentBalance,
                    //            BalanceAfter = balanceAfter,
                    //            IsBalanceAavailable = (balanceAfter > 0)

                    //        });
                    //    }

                    //}
                    searchData.Data = records;
                    searchData.TotalRecords = recordCount;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }


        public decimal GetCommitmentTotalAmount(int PaymentHeadId)
        {
            try
            {
                decimal Total = 0;
                using (var context = new IOASDBEntities())
                {
                    //Total = (from c in context.tblAdhocSalaryCommitmentDetail
                    //         join SP in context.tblSalaryPayment on c.PaymentId equals SP.PaymentId
                    //         where c.PaymentHeadId == PaymentHeadId
                    //         select new
                    //         {
                    //             SP.NetSalary,
                    //             SP.OtherAllowance
                    //         })
                    //         .AsEnumerable()
                    //         .Select(m => Convert.ToDecimal(m.NetSalary) - Convert.ToDecimal(m.OtherAllowance)
                    //         ).Sum();
                    Total = context.tblAdhocSalaryCommitmentDetail.Where(m => m.PaymentHeadId == PaymentHeadId).Sum(m => m.Amount ?? 0);
                }

                return Total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
        #region  AgencySalary
        public List<AgencySalaryModel> getAgencySalaryList()
        {
            try
            {
                List<AgencySalaryModel> list = new List<AgencySalaryModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from d in context.tblAgencySalary
                            orderby d.AgencySalaryId descending
                            select new
                            {
                                d.AgencySalaryId,
                                d.PaymentNo,
                                d.MonthYearStr,
                                d.DateOfPayment,
                                d.TotalEmployees,
                                d.NetPayable,
                                d.Status,
                                d.VendorId
                            })
                                 .AsEnumerable()
                                 .Select((x, index) => new AgencySalaryModel()
                                 {
                                     SlNo = index + 1,
                                     AgencySalaryID = x.AgencySalaryId,
                                     PaymentNo = x.PaymentNo,
                                     AgencyName = x.VendorId > 0 ? Common.GetSalaryAgencyName(x.VendorId ?? 0) : "",
                                     MonthYear = x.MonthYearStr,
                                     DateOfPayment = String.Format("{0:dd-MMMM-yyyy}", x.DateOfPayment),
                                     TotalEmployee = x.TotalEmployees ?? 0,
                                     TotalAmount = x.NetPayable ?? 0,
                                     Status = x.Status
                                 }).ToList();

                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<AgencySalaryModel>();
            }
        }
        public bool ValidateAgencySalaryStatus(int agencySalaryId, string[] status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == agencySalaryId && status.Contains(m.Status));
                    if (query != null)
                        isValid = true;
                }
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public AgencySalaryModel GetAgencySalaryDetails(int agencySalaryId)
        {
            try
            {
                //UpdateSalaryCalculation(agencySalaryId);
                AgencySalaryModel bill = new AgencySalaryModel();
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == agencySalaryId);
                    if (query != null)
                    {
                        bill.AgencySalaryID = query.AgencySalaryId;
                        bill.GST = query.GST;
                        bill.MonthYear = query.MonthYearStr;
                        bill.NetAmount = query.NetAmount;
                        bill.NetPayable = query.NetPayable;
                        bill.PaymentNo = query.PaymentNo;
                        bill.ServiceCharge = query.ServiceCharge;
                        bill.AgencyId = query.VendorId;
                        //bill.SGST = query.SGST;
                        var agencyQuery = context.tblSalaryAgencyMaster.Where(x => x.SalaryAgencyId == bill.AgencyId).Select(x => new { x.Agencyfee, x.AgencyName }).FirstOrDefault();
                        if (agencyQuery != null)
                        {
                            bill.displayServiceCharge = agencyQuery.Agencyfee;
                            bill.VendorName = agencyQuery.AgencyName;
                        }
                        bill.TotalAmount = query.TotalAmount;
                        bill.CommitmentAmount = context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == agencySalaryId).Sum(m => m.Amount);
                        bill.ExpenseAmount = query.ExpenseAmount;
                        bill.DeductionAmount = query.DeductionAmount;
                        bill.CheckListVerified_By = query.CheckListVerified_By;
                        bill.CheckListVerifierName = Common.GetUserFirstName(query.CheckListVerified_By ?? 0);

                        var otherQuery = (from other in context.tblAgencySalaryOtherAllowance
                                          join det in context.tblEmpOtherAllowance on other.EmpOtherAllowanceId equals det.id
                                          where other.AgencySalaryId == agencySalaryId
                                          select new { other.Amount, det.ComponentName }).ToList();
                        if (otherQuery.Count > 0)
                        {
                            bill.TotalDistributionAmount = otherQuery.Where(m => m.ComponentName == "Distribution").Sum(m => m.Amount) ?? 0;
                            bill.TotalHonororiumAmount = otherQuery.Where(m => m.ComponentName == "Honorarium").Sum(m => m.Amount) ?? 0;
                            bill.TotalMandaysAmount = otherQuery.Where(m => m.ComponentName == "Mandays").Sum(m => m.Amount) ?? 0;
                            bill.TotalFellowshipAmount = otherQuery.Where(m => m.ComponentName == "FellowshipSalary").Sum(m => m.Amount) ?? 0;
                        }
                        else
                        {
                            bill.TotalDistributionAmount = 0;
                            bill.TotalFellowshipAmount = 0;
                            bill.TotalHonororiumAmount = 0;
                            bill.TotalMandaysAmount = 0;
                        }

                        bill.CommitmentDetail = (from c in context.tblAgencySalaryCommitmentDetail
                                                 join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                                 join p in context.tblProject on com.ProjectId equals p.ProjectId
                                                 join head in context.tblBudgetHead on det.AllocationHeadId equals head.BudgetHeadId
                                                 orderby det.ComitmentDetailId descending
                                                 where c.AgencySalaryId == agencySalaryId && c.VerifiedSalaryId == 0 && c.Status == "Active"
                                                 select new BillCommitmentDetailModel()
                                                 {
                                                     CommitmentDetailId = c.CommitmentDetailId,
                                                     CommitmentNumber = com.CommitmentNumber,
                                                     ProjectNumber = p.ProjectNumber,
                                                     ProjectId = com.ProjectId,
                                                     HeadName = head.HeadName,
                                                     AvailableAmount = det.BalanceAmount ?? 0,
                                                     PaymentAmount = c.Amount,
                                                     BillCommitmentDetailId = c.AgencySalaryCommitmentDetailId
                                                 }).ToList();

                        bill.ExpenseDetail = (from e in context.tblAgencySalaryTransactionDetail
                                              where e.AgencySalaryId == agencySalaryId && e.Status == "Active"
                                              select new
                                              {
                                                  e.AccountHeadId,
                                                  e.Amount,
                                                  e.AccountGroupId,
                                                  e.AgencySalaryTransactionDetailId,
                                                  e.TransactionType,
                                                  e.IsJV_f
                                              })
                                              .AsEnumerable()
                                              .Select((x) => new BillExpenseDetailModel()
                                              {
                                                  AccountHeadId = x.AccountHeadId,
                                                  Amount = x.Amount,
                                                  TransactionType = x.TransactionType,
                                                  AccountGroupList = Common.GetAccountGroup(x.AccountGroupId ?? 0),
                                                  AccountGroupId = x.AccountGroupId,
                                                  AccountHeadList = Common.GetAccountHeadList(x.AccountGroupId ?? 0, x.AccountHeadId ?? 0, "1", "SLA"),
                                                  BillExpenseDetailId = x.AgencySalaryTransactionDetailId,
                                                  IsJV = x.IsJV_f ?? false
                                              }).ToList();

                        bill.DeductionDetail = (from d in context.tblAgencySalaryDeductionDetail
                                                join dh in context.tblDeductionHead on d.DeductionHeadId equals dh.DeductionHeadId
                                                join hd in context.tblAccountHead on dh.AccountHeadId equals hd.AccountHeadId
                                                join g in context.tblAccountGroup on hd.AccountGroupId equals g.AccountGroupId
                                                where d.AgencySalaryId == agencySalaryId && d.Status == "Active"
                                                select new BillDeductionDetailModel()
                                                {
                                                    AccountGroupId = d.AccountGroupId,
                                                    BillDeductionDetailId = d.AgencySalaryDeductionDetailId,
                                                    Amount = d.Amount,
                                                    DeductionHeadId = d.DeductionHeadId,
                                                    AccountGroup = g.AccountGroup,
                                                    DeductionHead = hd.AccountHead
                                                }).ToList();

                        bill.CheckListDetail = (from ck in context.tblAgencySalaryCheckDetail
                                                join chkf in context.tblFunctionCheckList on ck.FunctionCheckListId equals chkf.FunctionCheckListId
                                                where ck.AgencySalaryId == agencySalaryId && ck.Status == "Active"
                                                select new CheckListModel()
                                                {
                                                    CheckList = chkf.CheckList,
                                                    FunctionCheckListId = ck.FunctionCheckListId,
                                                    IsChecked = true
                                                }).ToList();
                        bill.DocumentDetail = (from d in context.tblAgencySalaryDocumentDetail
                                               where d.AgencySalaryId == agencySalaryId && d.Status == "Active"
                                               select new AttachmentDetailModel()
                                               {
                                                   DocumentActualName = d.DocumentActualName,
                                                   DocumentDetailId = d.AgencySalaryDocumentDetailId,
                                                   DocumentName = d.DocumentName,
                                                   DocumentPath = "~/Content/OtherDocuments",
                                                   DocumentType = d.DocumentType,
                                                   Remarks = d.Remarks
                                               }).ToList();

                    }
                }
                return bill;
            }
            catch (Exception ex)
            {
                return new AgencySalaryModel();
            }
        }
        public static List<AgencySalaryModel> SearchAgencySalaryList(AgencySearchFieldModel model)
        {
            List<AgencySalaryModel> honor = new List<AgencySalaryModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var y = Common.Getmonth();
                    var predicate = PredicateBuilder.BaseAnd<tblAgencySalary>();
                    if (!string.IsNullOrEmpty(model.PaymentNo))
                        predicate = predicate.And(d => d.PaymentNo == model.PaymentNo);
                    if (model.FromDate != null && model.ToDate != null)
                    {
                        model.ToDate = model.ToDate.Value.Date.AddDays(1).AddTicks(-2);
                        predicate = predicate.And(d => d.Crtd_TS >= model.FromDate && d.Crtd_TS <= model.ToDate);
                    }
                    var query = context.tblAgencySalary.Where(predicate).OrderByDescending(m => m.AgencySalaryId).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            honor.Add(new AgencySalaryModel()
                            {
                                SlNo = i + 1,
                                AgencySalaryID = query[i].AgencySalaryId,
                                PaymentNo = query[i].PaymentNo,
                                MonthYear = query[i].MonthYearStr,
                                DateOfPayment = String.Format("{0:dd-MMMM-yyyy}", query[i].DateOfPayment),
                                TotalEmployee = query[i].TotalEmployees ?? 0,
                                TotalAmount = query[i].TotalAmount ?? 0,
                                Status = query[i].Status
                            });
                        }
                    }
                    return honor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedData<AgencyStaffDetailsModel> GetAgencyEmployeeSalary(int vendorId, int page, int pageSize, int AgencySalaryID, string MonthYear, string EmployeeId, string Name)
        {
            var searchData = new PagedData<AgencyStaffDetailsModel>();
            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
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
                    if (AgencySalaryID != 0)
                        MonthYear = Common.getAgencySalaryPaymentMonth(AgencySalaryID);
                    DateTime payStartMonth = Common.GetMonthFirstDate(MonthYear);
                    DateTime payEndMonth = Common.GetMonthLastDate(MonthYear);
                    var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear && m.VendorId == vendorId);
                    if (AgencySalaryID == 0)
                    {
                        if (queryMYExists)
                        {
                            searchData.Data = model;
                            searchData.TotalRecords = 0;
                            searchData.TotalPages = 0;
                            return searchData;
                        }
                    }

                    var query = (from p in context.tblRCTPayroll
                                 join sm in context.vw_RCTOSGPayroll on p.RCTPayrollId equals sm.RCTPayrollId
                                 where p.Status == "Requested for salary processing" && p.SalaryMonth == MonthYear && p.AppointmentType == "OSG"
                                 && p.VendorId == vendorId
                                 && (String.IsNullOrEmpty(Name) || sm.Employee_Name.Contains(Name))
                                 && (String.IsNullOrEmpty(EmployeeId) || sm.Employee_ID.Contains(EmployeeId))
                                 && !context.tblAgencyVerifiedSalary.Any(m => m.RCTPayrollDetailId == sm.RCTPayrollDetailId)
                                 orderby sm.Employee_ID
                                 select sm);
                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {

                            model.Add(new AgencyStaffDetailsModel()
                            {
                                SlNo = skiprec + i + 1,
                                EmployeeId = records[i].Employee_ID,
                                PayrollDetailId = records[i].RCTPayrollDetailId,
                                Name = records[i].Employee_Name,
                                BasicSalary = Convert.ToDecimal(records[i].Recommended_Salary),
                                GrossTotal = Convert.ToDecimal(records[i].CTC_CM)
                            });
                        }
                    }
                    searchData.Data = model;
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }
        public DataSet GetAgencyEmployeeSalary(int AgencySalaryID, string MonthYear)
        {
            DataSet ds = new DataSet();
            DataTable dtUnverify = new DataTable("UnVerify");
            dtUnverify.Columns.Add("EmployeeId", typeof(String));
            dtUnverify.Columns.Add("Name", typeof(String));
            dtUnverify.Columns.Add("Project No", typeof(String));
            dtUnverify.Columns.Add("Designation", typeof(String));
            dtUnverify.Columns.Add("BasicSalary", typeof(Decimal));
            dtUnverify.Columns.Add("GrossTotal", typeof(Decimal));

            DataTable dtVerify = new DataTable("Verified");
            dtVerify.Columns.Add("EmployeeId", typeof(String));
            dtVerify.Columns.Add("Name", typeof(String));
            dtVerify.Columns.Add("Project No", typeof(String));
            dtVerify.Columns.Add("Designation", typeof(String));
            dtVerify.Columns.Add("BasicSalary", typeof(Decimal));
            dtVerify.Columns.Add("GrossTotal", typeof(Decimal));
            try
            {

                if (AgencySalaryID != 0)
                    MonthYear = Common.getAgencySalaryPaymentMonth(AgencySalaryID);
                DateTime payStartMonth = Common.GetMonthFirstDate(MonthYear);
                DateTime payEndMonth = Common.GetMonthLastDate(MonthYear);
                using (var context = new IOASDBEntities())
                {

                    //var records = (from sm in context.vwAppointmentMaster
                    //               //join ad in context.vwAppointmentDetails on sm.EmployeeId equals ad.EmployeeId
                    //               join sd in context.vwSalaryDetails on sm.EmployeeId equals sd.EmployeeId
                    //               where sm.Remarks != "StopPayment" //&& sm.status == "Active"
                    //                && (sm.RelieveDate == null || sm.RelieveDate >= payStartMonth)
                    //                && sm.AppointmentDate <= payEndMonth
                    //                && (AgencySalaryID == 0 || !context.tblAgencyVerifiedSalary.Any(m => m.AgencySalaryId == AgencySalaryID && m.EmployeeID == sm.EmployeeId))
                    //               orderby sm.EmployeeId
                    //               select new
                    //               {
                    //                   sm,
                    //                   sd.GrossTotal
                    //               }).ToList();
                    var records = (from p in context.tblRCTPayroll
                                   join sm in context.vw_RCTOSGPayroll on p.RCTPayrollId equals sm.RCTPayrollId
                                   where p.Status == "Requested for salary processing" && p.SalaryMonth == MonthYear && p.AppointmentType == "OSG"
                                    && !context.tblAgencyVerifiedSalary.Any(m => m.RCTPayrollDetailId == sm.RCTPayrollDetailId)
                                   orderby sm.Employee_ID
                                   select sm).ToList();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            var row = dtUnverify.NewRow();
                            row["EmployeeId"] = records[i].Employee_ID;
                            row["Name"] = records[i].Employee_Name;
                            row["Project No"] = records[i].Project_Number;
                            row["Designation"] = records[i].Designation;
                            row["BasicSalary"] = Convert.ToDecimal(records[i].Recommended_Salary);
                            row["GrossTotal"] = Convert.ToDecimal(records[i].CTC_CM);
                            dtUnverify.Rows.Add(row);
                        }
                    }

                    if (AgencySalaryID != 0)
                    {
                        var queryVerified = (from C in context.tblAgencyVerifiedSalary
                                             join sm in context.vw_RCTOSGPayroll on C.RCTPayrollDetailId equals sm.RCTPayrollDetailId
                                             where C.AgencySalaryId == AgencySalaryID
                                             orderby C.EmployeeID
                                             select new
                                             {
                                                 C,
                                                 DesignationName = sm.Designation,
                                                 BasicSalary = sm.Recommended_Salary,
                                                 ProjectNo = sm.Project_Number
                                             }).ToList();
                        if (queryVerified.Count > 0)
                        {
                            for (int i = 0; i < queryVerified.Count; i++)
                            {
                                var row = dtVerify.NewRow();
                                row["EmployeeId"] = queryVerified[i].C.EmployeeID;
                                row["Name"] = queryVerified[i].C.EmployeeName;
                                row["Project No"] = queryVerified[i].ProjectNo;
                                row["Designation"] = queryVerified[i].DesignationName;
                                row["BasicSalary"] = Convert.ToDecimal(queryVerified[i].BasicSalary);
                                row["GrossTotal"] = Convert.ToDecimal(queryVerified[i].C.Netsalary);
                                dtVerify.Rows.Add(row);
                            }
                        }
                    }
                }
                ds.Tables.Add(dtUnverify);
                ds.Tables.Add(dtVerify);
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ds;
            }
        }
        public PagedData<AgencyStaffDetailsModel> GetVerifiedEmployeeSalary(int pageIndex, int pageSize, int AgencySalaryId, string EmployeeId, string Name)
        {
            var searchData = new PagedData<AgencyStaffDetailsModel>();
            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
            try
            {
                int skiprec = 0;

                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var TodayDate = DateTime.Now.Month;
                    var query = (from C in context.tblAgencyVerifiedSalary
                                 where C.AgencySalaryId == AgencySalaryId
                                 && (String.IsNullOrEmpty(EmployeeId) || C.EmployeeID.Contains(EmployeeId))
                                 && (String.IsNullOrEmpty(Name) || C.EmployeeName.Contains(Name))
                                 orderby C.EmployeeID
                                 select C);

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new AgencyStaffDetailsModel()
                            {
                                SlNo = skiprec + i + 1,
                                EmployeeId = records[i].EmployeeID,
                                Name = records[i].EmployeeName,
                                AgencySalaryID = records[i].AgencySalaryId,
                                NetSalary = records[i].Netsalary,
                                VerifiedSalaryId = records[i].VerifiedSalaryId
                            });
                        }
                    }
                    searchData.Data = model;
                    searchData.TotalRecords = recordCount;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }
        public PagedData<BillCommitmentDetailModel> GetAgencySalaryCommitmentDetail(int pageIndex, int pageSize, int AgencySalaryId, string commitmentNo, string projectNo, string headName)
        {
            var searchData = new PagedData<BillCommitmentDetailModel>();
            List<BillCommitmentDetailModel> model = new List<BillCommitmentDetailModel>();
            try
            {
                int skiprec = 0;

                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var query = (from c in context.tblAgencySalaryCommitmentDetail
                                 join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                 join p in context.tblProject on com.ProjectId equals p.ProjectId
                                 join head in context.tblBudgetHead on det.AllocationHeadId equals head.BudgetHeadId
                                 orderby det.ComitmentDetailId descending
                                 where c.AgencySalaryId == AgencySalaryId && c.Status == "Active"
                                 && (String.IsNullOrEmpty(commitmentNo) || com.CommitmentNumber.Contains(commitmentNo))
                                 && (String.IsNullOrEmpty(projectNo) || p.ProjectNumber.Contains(projectNo))
                                 && (String.IsNullOrEmpty(headName) || head.HeadName.Contains(headName))
                                 select new
                                 {
                                     c.CommitmentDetailId,
                                     com.CommitmentNumber,
                                     p.ProjectNumber,
                                     com.ProjectId,
                                     head.HeadName,
                                     det.BalanceAmount,
                                     c.Amount
                                 });

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new BillCommitmentDetailModel()
                            {
                                SlNo = skiprec + i + 1,
                                CommitmentDetailId = records[i].CommitmentDetailId,
                                CommitmentNumber = records[i].CommitmentNumber,
                                ProjectNumber = records[i].ProjectNumber,
                                ProjectId = records[i].ProjectId,
                                HeadName = records[i].HeadName,
                                AvailableAmount = records[i].BalanceAmount ?? 0,
                                PaymentAmount = records[i].Amount
                            });
                        }
                    }
                    searchData.Data = model;
                    searchData.TotalRecords = recordCount;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }
        public AgencyStaffDetailsModel getEmployeeSalaryDetails(int AgencySalaryID, int payrollDetailId)
        {
            AgencyStaffDetailsModel model = new AgencyStaffDetailsModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qrySalary = (from sm in context.vw_RCTOSGPayroll
                                     join d in context.tblRCTPayrollDetail on sm.RCTPayrollDetailId equals d.RCTPayrollDetailId
                                     where sm.RCTPayrollDetailId == payrollDetailId
                                     select new { sm, d }).FirstOrDefault();
                    if (qrySalary != null)
                    {
                        model.PayrollDetailId = qrySalary.sm.RCTPayrollDetailId;
                        model.EmployeeId = qrySalary.sm.Employee_ID;
                        model.Name = qrySalary.sm.Employee_Name;
                        model.BasicSalary = qrySalary.sm.Recommended_Salary;
                        model.HRA = qrySalary.d.HRA;
                        model.Bonus = 0;
                        model.SpecialAllowance = 0;
                        model.PF = Math.Round(qrySalary.sm.EmployeePF_CM.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        model.ESI = Math.Round(qrySalary.sm.EmployeeESIC_CM.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        model.IncomeTax = Math.Round(qrySalary.sm.EmployeePT.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        decimal ttlDeduct = model.PF.GetValueOrDefault(0) + model.ESI.GetValueOrDefault(0) + model.IncomeTax.GetValueOrDefault(0);
                        model.TotalDeduction = Math.Round(ttlDeduct, 2, MidpointRounding.AwayFromZero);
                        model.GrossSalary = qrySalary.sm.Gross_Salary;
                        model.NetSalary = qrySalary.sm.CTC_CM;
                        model.AgencySalaryID = AgencySalaryID;
                        model.EmployerESI = Math.Round(qrySalary.sm.EmployerESIC_CM.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        model.EmployerPF = Math.Round(qrySalary.sm.EmployerPF_CM.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        model.Insurance = Math.Round(qrySalary.sm.InsuranceAmount_CM.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                        decimal contribution = qrySalary.sm.EmployerESIC_CM.GetValueOrDefault(0) + qrySalary.sm.EmployerPF_CM.GetValueOrDefault(0) + qrySalary.sm.InsuranceAmount_CM.GetValueOrDefault(0);
                        model.EmployerContribution = Math.Round(contribution, 2, MidpointRounding.AwayFromZero);
                        model.GrossTotal = qrySalary.sm.CTC_CM.GetValueOrDefault(0);
                        model.ProjectNo = qrySalary.sm.Project_Number;
                        model.CommitmentNo = qrySalary.sm.Commitment_Number;
                        model.PayrollDetailId = payrollDetailId;
                        var otherDet = context.tblEmpOtherAllowance.Where(m => m.EmployeeIdStr == model.EmployeeId && (m.IsPaid == false || m.IsPaid == null) && m.Category == 8).ToList();

                        model.otherDetail = otherDet;
                        model.OtherExpTotal = otherDet.Sum(m => m.Amount);

                        List<SalaryBreakUpDetailsModel> list = new List<SalaryBreakUpDetailsModel>();
                        if (qrySalary.d.Spl_Allowance > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Spl_Allowance,
                                HeadId = 1,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.Transport_Allowance > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Transport_Allowance,
                                HeadId = 2,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.PF_Revision > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.PF_Revision,
                                HeadId = 5,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.ESIC_Revision > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.ESIC_Revision,
                                HeadId = 6,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.Round_off > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Round_off,
                                HeadId = 8,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.Arrears > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Arrears,
                                HeadId = 9,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.OthersPay > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.OthersPay,
                                HeadId = 10,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.HRA_Arrears > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.HRA_Arrears,
                                HeadId = 203,
                                HeadList = Common.GetCommonHeadList(1, 1),
                                IsTaxable = true,
                                TypeId = 1
                            });
                        if (qrySalary.d.Loss_Of_Pay > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Loss_Of_Pay,
                                HeadId = 110,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.Contribution_to_PF > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Contribution_to_PF,
                                HeadId = 3,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.Recovery > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Recovery,
                                HeadId = 4,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.HRA_Recovery > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.HRA_Recovery,
                                HeadId = 204,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.OthersDeduction > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.OthersDeduction,
                                HeadId = 7,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.Professional_tax > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Professional_tax,
                                HeadId = 108,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = true,
                                TypeId = 2
                            });
                        if (qrySalary.d.Medical_Recovery > 0)
                            list.Add(new SalaryBreakUpDetailsModel()
                            {
                                Amount = qrySalary.d.Medical_Recovery,
                                HeadId = 123,
                                HeadList = Common.GetCommonHeadList(1, 2),
                                IsAffectProjectExp = true,
                                IsTaxable = false,
                                TypeId = 2
                            });
                        model.breakUpDetail = list;
                    }
                }
                return model;
            }
            catch (Exception)
            {

                return model;
            }
        }

        public AgencySalaryModel VerifyEmployeeDetails(AgencyVerifyEmployeeModel model, int UserID)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string payMonthStr = model.MonthYear;
                        if (model.AgencySalaryID != 0)
                            payMonthStr = Common.getAgencySalaryPaymentMonth(model.AgencySalaryID);
                        var qrySalary = (from sm in context.vw_RCTOSGPayroll
                                         where sm.RCTPayrollDetailId == model.PayrollDetailId
                                         select sm).FirstOrDefault();
                        if (qrySalary != null)
                        {

                            decimal NetSalary = qrySalary.CTC_CM.GetValueOrDefault(0);
                            var otherQuery = context.tblEmpOtherAllowance.Where(m => m.EmployeeIdStr == model.EmployeeId && (m.IsPaid == false || m.IsPaid == null) && m.Category == 8).ToList();
                            decimal otherAmt = otherQuery.Sum(m => m.Amount ?? 0);

                            decimal netsal = NetSalary + otherAmt;
                            netsal = Math.Round(netsal, 2, MidpointRounding.AwayFromZero);
                            var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == model.AgencySalaryID select C).FirstOrDefault();
                            if (qryAgency == null)
                            {
                                var PayMentNo = Common.getAgencySalarySequenceNumber();
                                tblAgencySalary Agency = new tblAgencySalary();
                                Agency.PaymentNo = PayMentNo.Item1;
                                Agency.SqquenceNo = PayMentNo.Item2;
                                Agency.DateOfPayment = DateTime.Now;
                                Agency.TotalAmount = netsal;
                                Agency.TotalEmployees = 1;
                                Agency.Status = "Open";
                                Agency.PaymentMonthYear = Common.GetMonthFirstDate(model.MonthYear);
                                Agency.MonthYearStr = model.MonthYear;
                                Agency.Crtd_TS = DateTime.Now;
                                Agency.Crtd_UserId = UserID;
                                context.tblAgencySalary.Add(Agency);
                                context.SaveChanges();
                                model.AgencySalaryID = Agency.AgencySalaryId;
                            }

                            tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                            var EmpName = qrySalary.Employee_Name;
                            Salary.EmployeeID = model.EmployeeId;
                            Salary.AgencySalaryId = model.AgencySalaryID;
                            Salary.EmployeeName = EmpName;
                            Salary.IsVerified = true;
                            Salary.MonthYear = model.MonthYear;
                            Salary.Netsalary = netsal;
                            Salary.RCTPayrollDetailId = model.PayrollDetailId;
                            //Salary.OrderId = qrySalary.OrderID;
                            Salary.Crtd_TS = DateTime.Now;
                            Salary.Crtd_UserId = UserID;
                            context.tblAgencyVerifiedSalary.Add(Salary);
                            context.SaveChanges();
                            int verifiedSalaryId = Salary.VerifiedSalaryId;

                            if (otherQuery.Count > 0)
                            {
                                foreach (var item in otherQuery)
                                {
                                    tblAgencySalaryOtherAllowance OA = new tblAgencySalaryOtherAllowance();
                                    OA.Amount = item.Amount;
                                    OA.AgencySalaryId = model.AgencySalaryID;
                                    OA.EmployeeID = item.EmployeeIdStr;
                                    OA.VerifiedSalaryId = verifiedSalaryId;
                                    OA.EmpOtherAllowanceId = item.id;
                                    context.tblAgencySalaryOtherAllowance.Add(OA);

                                    otherQuery.Where(m => m.id == item.id)
                                    .FirstOrDefault()
                                    .IsPaid = true;
                                    context.SaveChanges();
                                }
                            }
                            foreach (var item in model.buDetail)
                            {
                                tblAgencySalaryBreakUpDetail BU = new tblAgencySalaryBreakUpDetail();
                                BU.Amount = item.Amount;
                                BU.CategoryId = item.TypeId;
                                BU.HeadId = item.HeadId;
                                BU.Remarks = item.Remarks;
                                BU.VerifiedSalaryId = verifiedSalaryId;
                                context.tblAgencySalaryBreakUpDetail.Add(BU);
                                context.SaveChanges();
                            }
                            var qryCommitment = (from c in context.tblCommitment
                                                 join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                 where c.CommitmentNumber == qrySalary.Commitment_Number
                                                 && c.Status == "Active"
                                                 select new { cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                            if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                            {
                                tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                com.AgencySalaryId = model.AgencySalaryID;
                                com.Amount = NetSalary;
                                com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                com.CRTD_By = UserID;
                                com.CRTD_TS = DateTime.Now;
                                com.Status = "Active";
                                com.VerifiedSalaryId = verifiedSalaryId;
                                context.tblAgencySalaryCommitmentDetail.Add(com);
                                context.SaveChanges();
                            }
                            else
                                return data;
                            transaction.Commit();
                            data = UpdateSalaryCalculation(model.AgencySalaryID, UserID);
                            data.AgencySalaryID = model.AgencySalaryID;
                            return data;
                        }
                        else
                            return data;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return data;
                    }
                }
            }
        }
        public AgencySalaryModel VerifyAllEmployeeDetails(int AgencySalaryID, string MonthYear, int UserID)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";
                if (AgencySalaryID == 0)
                {
                    if (String.IsNullOrEmpty(MonthYear))
                        return data;
                    var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear);
                    if (queryMYExists)
                    {
                        data.Status = "Salary process already initiated for " + MonthYear;
                    }
                    var qrySalary = (from p in context.tblRCTPayroll
                                     join sm in context.vw_RCTOSGPayroll on p.RCTPayrollId equals sm.RCTPayrollId
                                     join d in context.tblRCTPayrollDetail on sm.RCTPayrollDetailId equals d.RCTPayrollDetailId
                                     where p.Status == "Requested for salary processing" && p.SalaryMonth == MonthYear && p.AppointmentType == "OSG"
                                      && !context.tblAgencyVerifiedSalary.Any(m => m.RCTPayrollDetailId == sm.RCTPayrollDetailId)
                                     orderby sm.Employee_ID
                                     select sm).ToList();
                    if (qrySalary.Count > 0)
                    {
                        var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == AgencySalaryID select C).FirstOrDefault();
                        if (qryAgency == null)
                        {
                            var PayMentNo = Common.getAgencySalarySequenceNumber();
                            tblAgencySalary Agency = new tblAgencySalary();
                            Agency.PaymentNo = PayMentNo.Item1;
                            Agency.SqquenceNo = PayMentNo.Item2;
                            Agency.DateOfPayment = DateTime.Now;
                            Agency.TotalAmount = 0;
                            Agency.TotalEmployees = 1;
                            Agency.PaymentMonthYear = Common.GetMonthFirstDate(MonthYear);
                            Agency.MonthYearStr = MonthYear;
                            Agency.Crtd_TS = DateTime.Now;
                            Agency.Crtd_UserId = UserID;
                            context.tblAgencySalary.Add(Agency);
                            context.SaveChanges();
                            AgencySalaryID = Agency.AgencySalaryId;
                        }
                        foreach (var item in qrySalary)
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    decimal grossTtl = Convert.ToDecimal(item.CTC_CM);

                                    decimal NetSalary = grossTtl;// item.GrossTotal;
                                    var otherQuery = context.tblEmpOtherAllowance.Where(m => m.EmployeeIdStr == item.Employee_ID && (m.IsPaid == false || m.IsPaid == null) && m.Category == 8).ToList();
                                    decimal otherAmt = otherQuery.Sum(m => m.Amount ?? 0);
                                    var qryCommitment = (from c in context.tblCommitment
                                                         join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                         where c.CommitmentNumber == item.Commitment_Number
                                                         && c.Status == "Active"
                                                         select new { cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                                    if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                                    {
                                        decimal netsal = NetSalary + otherAmt;
                                        netsal = Math.Round(netsal, 2, MidpointRounding.AwayFromZero);


                                        tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                                        var EmpName = Common.getAgencyEmployeeName(item.Employee_ID);
                                        Salary.EmployeeID = item.Employee_ID;
                                        Salary.AgencySalaryId = AgencySalaryID;
                                        Salary.EmployeeName = EmpName;
                                        Salary.IsVerified = true;
                                        Salary.MonthYear = MonthYear;
                                        Salary.Netsalary = netsal;
                                        Salary.RCTPayrollDetailId = item.RCTPayrollDetailId;
                                        //Salary.OrderId = item.OrderID;
                                        Salary.Crtd_TS = DateTime.Now;
                                        Salary.Crtd_UserId = UserID;
                                        context.tblAgencyVerifiedSalary.Add(Salary);
                                        context.SaveChanges();
                                        int verifiedSalaryId = Salary.VerifiedSalaryId;

                                        if (otherQuery.Count > 0)
                                        {
                                            foreach (var det in otherQuery)
                                            {
                                                tblAgencySalaryOtherAllowance OA = new tblAgencySalaryOtherAllowance();
                                                OA.Amount = det.Amount;
                                                OA.AgencySalaryId = AgencySalaryID;
                                                OA.EmployeeID = det.EmployeeIdStr;
                                                OA.VerifiedSalaryId = verifiedSalaryId;
                                                OA.EmpOtherAllowanceId = det.id;
                                                context.tblAgencySalaryOtherAllowance.Add(OA);

                                                otherQuery.Where(m => m.id == det.id)
                                                .FirstOrDefault()
                                                .IsPaid = true;
                                                context.SaveChanges();
                                            }
                                        }

                                        tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                        com.AgencySalaryId = AgencySalaryID;
                                        com.Amount = NetSalary;
                                        com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                        com.CRTD_By = UserID;
                                        com.CRTD_TS = DateTime.Now;
                                        com.Status = "Active";
                                        com.VerifiedSalaryId = verifiedSalaryId;
                                        context.tblAgencySalaryCommitmentDetail.Add(com);
                                        context.SaveChanges();
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return data;
                                }
                            }

                        }
                    }
                    else
                        return data;
                }
                else
                {
                    //string payMonthStr = Common.getAgencySalaryPaymentMonth(AgencySalaryID);
                    var slaryData = Common.getAgencySalaryMonthVendorId(AgencySalaryID);
                    string payMonthStr = slaryData.Item2;
                    int vendorId = slaryData.Item1;
                    var qrySalary = (from p in context.tblRCTPayroll
                                     join sm in context.vw_RCTOSGPayroll on p.RCTPayrollId equals sm.RCTPayrollId
                                     join d in context.tblRCTPayrollDetail on sm.RCTPayrollDetailId equals d.RCTPayrollDetailId
                                     where p.Status == "Requested for salary processing" && p.SalaryMonth == payMonthStr && p.AppointmentType == "OSG"
                                      && !context.tblAgencyVerifiedSalary.Any(m => m.RCTPayrollDetailId == sm.RCTPayrollDetailId)
                                 && p.VendorId == vendorId
                                     orderby sm.Employee_ID
                                     select sm).ToList();
                    if (qrySalary.Count > 0)
                    {
                        foreach (var item in qrySalary)
                        {
                            var otherQuery = context.tblEmpOtherAllowance.Where(m => m.EmployeeIdStr == item.Employee_ID && (m.IsPaid == false || m.IsPaid == null) && m.Category == 8).ToList();
                            decimal otherAmt = otherQuery.Sum(m => m.Amount ?? 0);
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    decimal grossTtl = Convert.ToDecimal(item.CTC_CM);

                                    decimal NetSalary = grossTtl;// item.GrossTotal;
                                    var qryCommitment = (from c in context.tblCommitment
                                                         join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                         where c.CommitmentNumber == item.Commitment_Number
                                                         && c.Status == "Active"
                                                         select new { cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                                    if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                                    {
                                        decimal netsal = NetSalary + otherAmt;
                                        netsal = Math.Round(netsal, 2, MidpointRounding.AwayFromZero);

                                        tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                                        Salary.EmployeeID = item.Employee_ID;
                                        Salary.AgencySalaryId = AgencySalaryID;
                                        Salary.EmployeeName = item.Employee_Name;
                                        Salary.IsVerified = true;
                                        Salary.MonthYear = payMonthStr;
                                        Salary.Netsalary = netsal;
                                        Salary.RCTPayrollDetailId = item.RCTPayrollDetailId;
                                        //Salary.OrderId = item.OrderID;
                                        Salary.Crtd_TS = DateTime.Now;
                                        Salary.Crtd_UserId = UserID;
                                        context.tblAgencyVerifiedSalary.Add(Salary);
                                        context.SaveChanges();
                                        int verifiedSalaryId = Salary.VerifiedSalaryId;

                                        if (otherQuery.Count > 0)
                                        {
                                            foreach (var det in otherQuery)
                                            {
                                                tblAgencySalaryOtherAllowance OA = new tblAgencySalaryOtherAllowance();
                                                OA.Amount = det.Amount;
                                                OA.AgencySalaryId = AgencySalaryID;
                                                OA.EmployeeID = det.EmployeeIdStr;
                                                OA.VerifiedSalaryId = verifiedSalaryId;
                                                OA.EmpOtherAllowanceId = det.id;
                                                context.tblAgencySalaryOtherAllowance.Add(OA);

                                                otherQuery.Where(m => m.id == det.id)
                                                .FirstOrDefault()
                                                .IsPaid = true;
                                                context.SaveChanges();
                                            }
                                        }

                                        tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                        com.AgencySalaryId = AgencySalaryID;
                                        com.Amount = NetSalary;
                                        com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                        com.CRTD_By = UserID;
                                        com.CRTD_TS = DateTime.Now;
                                        com.Status = "Active";
                                        com.VerifiedSalaryId = verifiedSalaryId;
                                        context.tblAgencySalaryCommitmentDetail.Add(com);
                                        context.SaveChanges();
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return data;
                                }
                            }
                        }
                    }
                    else
                        return data;
                }
                data = UpdateSalaryCalculation(AgencySalaryID, UserID);
                data.AgencySalaryID = AgencySalaryID;
                return data;
            }
        }
        public AgencySalaryModel DeleteVerifiedEmployee(int VerifiedSalaryId, int userId)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int agencyId = 0;
                        var qrySalary = context.tblAgencyVerifiedSalary.FirstOrDefault(c => c.VerifiedSalaryId == VerifiedSalaryId);
                        if (qrySalary != null)
                        {
                            decimal NetSalary = qrySalary.Netsalary ?? 0;
                            agencyId = Convert.ToInt32(qrySalary.AgencySalaryId);
                            var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == agencyId select C).FirstOrDefault();
                            if (qryAgency != null)
                            {
                                NetSalary = (qryAgency.TotalAmount ?? 0) - NetSalary;
                                qryAgency.TotalAmount = NetSalary;
                                qryAgency.TotalEmployees = qryAgency.TotalEmployees - 1;
                                context.SaveChanges();
                            }
                            else
                                return data;
                        }
                        else
                            return data;
                        var arryEmpId = context.tblAgencySalaryOtherAllowance.Where(m => m.VerifiedSalaryId == VerifiedSalaryId)
                                  .Select(m => m.EmpOtherAllowanceId)
                                  .ToArray();
                        if (arryEmpId.Count() > 0)
                        {
                            context.tblEmpOtherAllowance.Where(m => arryEmpId.Contains(m.id))
                                 .ToList()
                                  .ForEach(m =>
                                  {
                                      m.IsPaid = false;
                                  });
                            context.tblAgencySalaryOtherAllowance.RemoveRange(context.tblAgencySalaryOtherAllowance.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        }
                        context.tblAgencyVerifiedSalary.RemoveRange(context.tblAgencyVerifiedSalary.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.tblAgencySalaryBreakUpDetail.RemoveRange(context.tblAgencySalaryBreakUpDetail.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.tblAgencySalaryCommitmentDetail.RemoveRange(context.tblAgencySalaryCommitmentDetail.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.SaveChanges();
                        transaction.Commit();
                        data = UpdateSalaryCalculation(agencyId, userId);
                        return data;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return data;
                    }
                }
            }
        }
        private AgencySalaryModel UpdateSalaryCalculation(int agencySalId, int userId)
        {
            AgencySalaryModel data = new AgencySalaryModel();
            data.Status = "error";
            using (var context = new IOASDBEntities())
            {
                var qryAgency = context.tblAgencySalary.FirstOrDefault(C => C.AgencySalaryId == agencySalId);
                if (qryAgency != null)
                {
                    var qryDet = context.tblAgencyVerifiedSalary.Where(m => m.AgencySalaryId == agencySalId);
                    decimal ttlSal = qryDet.Sum(m => m.Netsalary) ?? 0;
                    if (ttlSal > 0)
                    {
                        var otherQuery = (from other in context.tblAgencySalaryOtherAllowance
                                          join det in context.tblEmpOtherAllowance on other.EmpOtherAllowanceId equals det.id
                                          where other.AgencySalaryId == agencySalId
                                          select new { other.Amount, det.ComponentName }).ToList();

                        decimal scPct = Convert.ToDecimal(ConfigurationManager.AppSettings["TandM_ServiceCharge"]);
                        decimal sc = ttlSal * scPct / 100;
                        sc = Math.Round(sc, 2, MidpointRounding.AwayFromZero);
                        decimal netSal = ttlSal + sc;
                        decimal GST = netSal * 18 / 100;
                        GST = Math.Round(GST, 2, MidpointRounding.AwayFromZero);
                        //decimal CGST = GST / 2;
                        decimal payable = netSal + GST;
                        payable = Math.Round(payable, 2, MidpointRounding.AwayFromZero);
                        //CGST = Math.Round(CGST, 2, MidpointRounding.AwayFromZero);

                        qryAgency.GST = GST;
                        qryAgency.ServiceCharge = sc;
                        qryAgency.NetAmount = netSal;
                        qryAgency.TotalAmount = ttlSal;
                        qryAgency.NetPayable = payable;
                        qryAgency.Status = "Init";
                        qryAgency.Lastupdate_TS =
                        qryAgency.DateOfPayment = DateTime.Now;
                        qryAgency.LastupdatedUserid = userId;
                        qryAgency.TotalEmployees = qryDet.Count();

                        data.GST = GST;
                        data.ServiceCharge = sc;
                        data.NetAmount = netSal;
                        data.NetPayable = payable;
                        data.TotalAmount = ttlSal;
                        data.CommitmentAmount = context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == agencySalId).Sum(m => m.Amount);
                        if (otherQuery.Count > 0)
                        {
                            data.TotalDistributionAmount = otherQuery.Where(m => m.ComponentName == "Distribution").Sum(m => m.Amount) ?? 0;
                            data.TotalHonororiumAmount = otherQuery.Where(m => m.ComponentName == "Honorarium").Sum(m => m.Amount) ?? 0;
                            data.TotalMandaysAmount = otherQuery.Where(m => m.ComponentName == "Mandays").Sum(m => m.Amount) ?? 0;
                            data.TotalFellowshipAmount = otherQuery.Where(m => m.ComponentName == "FellowshipSalary").Sum(m => m.Amount) ?? 0;
                        }
                        else
                        {
                            data.TotalDistributionAmount = 0;
                            data.TotalFellowshipAmount = 0;
                            data.TotalHonororiumAmount = 0;
                            data.TotalMandaysAmount = 0;
                        }
                        data.Status = "success";
                        context.SaveChanges();
                    }
                    else
                    {
                        qryAgency.GST = 0;
                        qryAgency.ServiceCharge = 0;
                        qryAgency.NetAmount = 0;
                        qryAgency.NetPayable = 0;
                        qryAgency.Status = "Init";

                        data.GST = 0;
                        data.ServiceCharge = 0;
                        data.NetAmount = 0;
                        data.NetPayable = 0;
                        data.TotalAmount = 0;
                        data.CommitmentAmount = 0;
                        data.TotalDistributionAmount = 0;
                        data.TotalFellowshipAmount = 0;
                        data.TotalHonororiumAmount = 0;
                        data.TotalMandaysAmount = 0;
                        data.Status = "success";
                        context.SaveChanges();
                    }
                }
                return data;
            }
        }
        public int CreateSalaryAgency(AgencySalaryModel model, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.AgencySalaryID > 0)
                        {
                            int AgencyId = model.AgencySalaryID ?? 0;
                            var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == model.AgencySalaryID);
                            if (query != null)
                            {
                                query.LastupdatedUserid = LoggedInUser;
                                query.Lastupdate_TS = DateTime.Now;
                                query.CheckListVerified_By = model.CheckListVerified_By;
                                query.DeductionAmount = model.DeductionDetail != null ? model.DeductionDetail.Sum(m => m.Amount) : 0;
                                query.ExpenseAmount = model.ExpenseDetail.Sum(m => m.Amount);
                                query.Status = "Open";

                                context.tblAgencySalaryDeductionDetail.RemoveRange(context.tblAgencySalaryDeductionDetail.Where(m => m.AgencySalaryId == AgencyId));
                                context.SaveChanges();
                                if (model.DeductionDetail != null)
                                {
                                    foreach (var item in model.DeductionDetail)
                                    {
                                        if (item.Amount != null && item.Amount != 0)
                                        {
                                            if (item.AccountGroupId == null)
                                                return -1;
                                            tblAgencySalaryDeductionDetail deduction = new tblAgencySalaryDeductionDetail();
                                            deduction.AccountGroupId = item.AccountGroupId;
                                            deduction.Amount = item.Amount;
                                            deduction.AgencySalaryId = AgencyId;
                                            deduction.CRTD_By = LoggedInUser;
                                            deduction.CRTD_TS = DateTime.Now;
                                            deduction.DeductionHeadId = item.DeductionHeadId;
                                            deduction.Status = "Active";
                                            context.tblAgencySalaryDeductionDetail.Add(deduction);
                                            context.SaveChanges();
                                        }
                                    }
                                }

                                context.tblAgencySalaryTransactionDetail.RemoveRange(context.tblAgencySalaryTransactionDetail.Where(m => m.AgencySalaryId == model.AgencySalaryID));
                                context.SaveChanges();
                                foreach (var item in model.ExpenseDetail)
                                {
                                    tblAgencySalaryTransactionDetail ASTran = new tblAgencySalaryTransactionDetail();
                                    ASTran.AgencySalaryId = AgencyId;
                                    ASTran.AccountGroupId = item.AccountGroupId;
                                    ASTran.AccountHeadId = item.AccountHeadId;
                                    ASTran.Amount = item.Amount;
                                    ASTran.TransactionType = item.TransactionType;
                                    ASTran.CRTD_By = LoggedInUser;
                                    ASTran.IsJV_f = item.IsJV;
                                    ASTran.CRTD_TS = DateTime.Now;
                                    ASTran.Status = "Active";
                                    context.tblAgencySalaryTransactionDetail.Add(ASTran);
                                    context.SaveChanges();
                                }
                                context.tblAgencySalaryCommitmentDetail.RemoveRange(context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == AgencyId && m.VerifiedSalaryId == 0));
                                context.SaveChanges();
                                foreach (var item in model.CommitmentDetail)
                                {
                                    if (item.CommitmentDetailId == null)
                                        return -3;
                                    tblAgencySalaryCommitmentDetail ASComm = new tblAgencySalaryCommitmentDetail();
                                    ASComm.AgencySalaryId = AgencyId;
                                    ASComm.CommitmentDetailId = item.CommitmentDetailId;
                                    ASComm.Amount = item.PaymentAmount;
                                    ASComm.CRTD_By = LoggedInUser;
                                    ASComm.VerifiedSalaryId = 0;
                                    ASComm.CRTD_TS = DateTime.Now;
                                    ASComm.Status = "Active";
                                    context.tblAgencySalaryCommitmentDetail.Add(ASComm);
                                    context.SaveChanges();
                                }
                                context.tblAgencySalaryCheckDetail.RemoveRange(context.tblAgencySalaryCheckDetail.Where(m => m.AgencySalaryId == AgencyId));
                                context.SaveChanges();
                                foreach (var item in model.CheckListDetail)
                                {
                                    if (item.IsChecked)
                                    {
                                        tblAgencySalaryCheckDetail ASCheck = new tblAgencySalaryCheckDetail();
                                        ASCheck.Verified_By = model.CheckListVerified_By;
                                        ASCheck.FunctionCheckListId = item.FunctionCheckListId;
                                        ASCheck.AgencySalaryId = AgencyId;
                                        ASCheck.CRTD_By = LoggedInUser;
                                        ASCheck.CRTD_TS = DateTime.Now;
                                        ASCheck.Status = "Active";
                                        context.tblAgencySalaryCheckDetail.Add(ASCheck);
                                        context.SaveChanges();
                                    }
                                }
                                var arrList = model.DocumentDetail.Select(m => m.DocumentDetailId ?? 0).ToArray();
                                context.tblAgencySalaryDocumentDetail.Where(x => x.AgencySalaryId == model.AgencySalaryID && !arrList.Contains(x.AgencySalaryDocumentDetailId) && x.Status != "InActive")
                                .ToList()
                                .ForEach(m =>
                                {
                                    m.Status = "InActive";
                                    m.UPDT_By = LoggedInUser;
                                    m.UPDT_TS = DateTime.Now;
                                });
                                foreach (var item in model.DocumentDetail)
                                {
                                    var docQuery = context.tblAgencySalaryDocumentDetail.FirstOrDefault(m => m.AgencySalaryDocumentDetailId == item.DocumentDetailId);
                                    if (docQuery == null)
                                    {
                                        tblAgencySalaryDocumentDetail ASdoc = new tblAgencySalaryDocumentDetail();
                                        string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                        var guid = Guid.NewGuid().ToString();
                                        var docName = guid + "_" + actName;
                                        item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/" + docName));
                                        ASdoc.CRTD_By = LoggedInUser;
                                        ASdoc.CRTD_TS = DateTime.Now;
                                        ASdoc.DocumentActualName = actName;
                                        ASdoc.DocumentName = docName;
                                        ASdoc.DocumentType = item.DocumentType;
                                        ASdoc.Remarks = item.Remarks;
                                        ASdoc.AgencySalaryId = AgencyId;
                                        ASdoc.Status = "Active";
                                        context.tblAgencySalaryDocumentDetail.Add(ASdoc);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        if (item.DocumentFile != null)
                                        {
                                            string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                            var guid = Guid.NewGuid().ToString();
                                            var docName = guid + "_" + actName;
                                            item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/" + docName));
                                            docQuery.DocumentActualName = actName;
                                            docQuery.DocumentName = docName;
                                        }
                                        docQuery.UPDT_By = LoggedInUser;
                                        docQuery.UPDT_TS = DateTime.Now;
                                        docQuery.DocumentType = item.DocumentType;
                                        docQuery.Remarks = item.Remarks;
                                        context.SaveChanges();
                                    }
                                }
                                transaction.Commit();
                                return AgencyId;
                            }
                            else
                            {
                                return -2;
                            }
                        }
                        else
                            return -1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }
        public string ValidateAgencyVerify(string EmployeeID, int payrollDetailId, int AgencySalaryID, string MonthYear, AgencyVerifyEmployeeModel model = null, bool finalize = false)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (AgencySalaryID == 0)
                    {
                        if (String.IsNullOrEmpty(MonthYear))
                            return "Something went wrong, Please contact administrator";
                        var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear);
                        if (queryMYExists)
                            return "Salary process already initiated for " + MonthYear;

                        //var queryEmpExists = context.tblAgencyVerifiedSalary.Any(m => m.MonthYear == MonthYear && m.EmployeeID == EmployeeID);
                        //if (queryEmpExists)
                        // return "Salary process already initiated for this employee.";
                    }
                    else
                    {
                        var queryEmpExist = context.tblAgencyVerifiedSalary.Any(m => m.AgencySalaryId == AgencySalaryID && m.RCTPayrollDetailId == payrollDetailId);
                        if (queryEmpExist)
                            return "Salary process already initiated to " + EmployeeID;

                        //var queryEmpExists = context.tblAgencyVerifiedSalary.Any(m => m.MonthYear == MonthYear && m.EmployeeID == EmployeeID && m.AgencySalaryId == AgencySalaryID);
                        //if (queryEmpExists)
                        // return "Salary process already initiated for this employee.";

                    }
                    var checkCommitment = (from sm in context.vw_RCTOSGPayroll
                                           join c in context.tblCommitment on sm.Commitment_Number equals c.CommitmentNumber
                                           where sm.RCTPayrollDetailId == payrollDetailId
                                           && c.Status == "Active"
                                           select new { c.CommitmentBalance, c.CommitmentNumber, sm.CTC_CM }).FirstOrDefault();
                    if (checkCommitment != null)
                    {
                        decimal grossTtl = Convert.ToDecimal(checkCommitment.CTC_CM);

                        if (grossTtl > checkCommitment.CommitmentBalance && finalize)
                            return "Employee net salary is greater than commitment available balance.";
                    }
                    else
                        return "Commitment does not exists for this user " + EmployeeID;
                    return "Valid";
                }
            }
            catch (Exception ex)
            {
                return "Something went wrong, Please contact administrator";
            }
        }
        public bool SLACommitmentBalanceUpdate(Int32 billId, bool revoke, bool isReversed, int uId, string tCode)
        {
            try
            {
                BOAModel model = new BOAModel();
                List<BillCommitmentDetailModel> txList = new List<BillCommitmentDetailModel>();
                using (var context = new IOASDBEntities())
                {

                    txList = (from c in context.tblAgencySalaryCommitmentDetail
                              join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                              join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                              where c.AgencySalaryId == billId && c.Status == "Active"
                              select new BillCommitmentDetailModel()
                              {
                                  CommitmentDetailId = c.CommitmentDetailId,
                                  PaymentAmount = c.Amount,
                                  CommitmentId = com.CommitmentId,
                                  ReversedAmount = c.Amount
                              }).ToList();
                    if (txList.Count > 0)
                        return coreAccounts.UpdateCommitmentBalance(txList, revoke, isReversed, uId, billId, tCode);
                    else
                        return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ApproveSLA(int billId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == billId && m.Status == "Open");
                    if (query != null)
                    {
                        query.Status = "Completed";
                        query.LastupdatedUserid = logged_in_user;
                        query.Lastupdate_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region Employee Projection
        public static AdhocEmployeeModel GetEmployeeDetails(string empId)
        {
            AdhocEmployeeModel model = new AdhocEmployeeModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    DateTime fromdate = new DateTime(2019, 2, 3);
                    int CurrentYear = fromdate.Year;
                    int PreviousYear = fromdate.Year - 1;
                    int NextYear = fromdate.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;
                    if (fromdate.Month > 2)
                        FinYear = CurYear.Substring(2) + NexYear.Substring(2);
                    else
                        FinYear = PreYear.Substring(2) + CurYear.Substring(2);
                    var Qry = (from emp in context.vw_RCTAdhocEmployeeMaster
                                   //join bank in context.tblStaffBankAccount on new { cate = "AdhocStaff", ids = emp.EmployeeId } equals
                                   //new { cate = bank.Category, ids = bank.EmployeeId } into g
                                   //from bank in g.DefaultIfEmpty()
                               where emp.EmployeeId == empId
                               select new
                               {
                                   emp
                                   // ,bank
                               }).FirstOrDefault();
                    if (Qry != null)
                    {
                        if (Qry.emp != null)
                        {
                            model.DateOfBirth = String.Format("{0:ddd dd-MMM-yyyy}", Qry.emp.DOB);
                            model.Gender = Qry.emp.Gender;
                            model.DEPARTMENT = Qry.emp.DEPARTMENT;
                            model.Designation = Qry.emp.Designation;
                            model.RelDate = String.Format("{0:ddd dd-MMM-yyyy}", Qry.emp.RelieveDate);
                            model.ExtenDate = String.Format("{0:ddd dd-MMM-yyyy}", Qry.emp.ExtensionDate);
                            model.EmployeeName = Qry.emp.EmployeeId + '-' + Qry.emp.NAME;

                            model.PAN = Qry.emp.PANNo;
                            model.AccountNo = Qry.emp.BankAccountNumber;

                        }
                        //if (Qry.bank != null)
                        //{
                        //    model.PAN = Qry.bank.PAN;
                        //    model.AccountNo = Qry.bank.AccountNumber;
                        //}
                    }

                    model.EmployeeID = empId;
                    model.FinYear = FinYear;

                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }


        public List<AgencyStaffDetailsModel> projectionsalarydetails(List<AgencyStaffDetailsModel> salarydetailstemp)
        {


            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
            var groupedsalarydetailsList = salarydetailstemp.GroupBy(u => u.MonthandYear).Select(grp => new AgencyStaffDetailsModel()
            {
                AddNew_f = grp.First().AddNew_f,
                AgencySalaryID = grp.First().AgencySalaryID,
                BasicSalary = grp.Sum(s => s.BasicSalary),
                Bonus = grp.First().Bonus,
                CommitmentNo = grp.First().CommitmentNo,
                Designation = grp.First().Designation,
                ESI = grp.First().ESI,
                EmployeeId = grp.First().EmployeeId,
                EmployerContribution = grp.First().EmployerContribution,
                EmployerESI = grp.First().EmployerESI,
                EmployerPF = grp.First().EmployerPF,
                ExtensionDate = grp.First().ExtensionDate,
                GrossSalary = grp.Sum(s => s.GrossSalary),
                GrossTotal = grp.Sum(s => s.GrossTotal),
                HRA = grp.First().HRA,
                IncomeTax = grp.First().IncomeTax,
                Insurance = grp.First().Insurance,
                IsVerified = grp.First().IsVerified,
                LLP = grp.First().LLP,
                MA = grp.First().MA,
                MA2 = grp.First().MA2,
                MiscPay = grp.First().MiscPay,
                MiscRecovery = grp.First().MiscRecovery,
                MedicalRecovery = grp.First().MedicalRecovery,
                MonthandYear = grp.First().MonthandYear,
                Name = grp.First().Name,
                NetSalary = grp.First().NetSalary,
                OtherExpTotal = grp.First().OtherExpTotal,
                OtherPay = grp.First().OtherPay,
                PF = grp.First().PF,
                PayrollDetailId = grp.First().PayrollDetailId,
                ProjectNo = grp.First().ProjectNo,
                RelieveDate = grp.First().RelieveDate,
                Remarks = grp.First().Remarks,
                SlNo = grp.First().SlNo,
                SpecialAllowance = grp.First().SpecialAllowance,
                TotalDeduction = grp.First().TotalDeduction,
                VerifiedBy = grp.First().VerifiedBy,
                VerifiedSalaryId = grp.First().VerifiedSalaryId,
            }).ToList();

            model.AddRange(groupedsalarydetailsList);

            return model;
        }


        public List<AgencyStaffDetailsModel> GetEmployeesSalaryDetails(string empId, int Finyear, string monthYear = "")
        {
            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
            AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
            AdhocSalaryProcess asp = new AdhocSalaryProcess();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                    int CurrentYear = fromdate.Year;

                    FinOp fp = new FinOp(CurrentYear);
                    DateTime finEndDate = fp.SalFinEnd();
                    DateTime salFinStartDate = fp.SalFinStart();
                    var finMonthYearList = fp.GetAllSalaryMonths();
                    string finStartMonth = fp.SalStartMonth();
                    DateTime salaryEndDate;

                    string month = "";
                    var SalaryMaxMonth = context.tblSalaryPayment.Where(m => m.TypeOfPayBill == 1 && m.PayBill == empId).OrderByDescending(m => m.PaymentHeadId).FirstOrDefault();
                    var UserDate = context.vw_RCTAdhocEmployeeMaster.Where(M => M.EmployeeId == empId).OrderByDescending(m => m.RelieveDate ?? DateTime.MaxValue).ThenByDescending(x => x.ExtensionDate).FirstOrDefault();
                    DateTime? UserRelieveDate = UserDate.RelieveDate == null ? UserDate.ExtensionDate : UserDate.RelieveDate;
                    if (SalaryMaxMonth != null)
                    {
                        month = SalaryMaxMonth.PaymentMonthYear;
                        var Qry = (from bank in context.tblSalaryPayment
                                   join sal in context.tblSalaryPaymentHead on bank.PaymentHeadId equals sal.PaymentHeadId
                                   join user in context.tblUser on bank.CreatedBy equals user.UserId into g
                                   from user in g.DefaultIfEmpty()
                                   where bank.PayBill == empId &&
                                   (string.IsNullOrEmpty(monthYear) || bank.PaymentMonthYear != monthYear)
                                   && finMonthYearList.Contains(bank.PaymentMonthYear) && sal.TypeOfPayBill == 1
                                   select new
                                   {
                                       bank.Basic,
                                       bank.MonthlyTax,
                                       bank.ProfTax,
                                       bank.OtherAllowance,
                                       bank.PaymentId,
                                       bank.MA,
                                       bank.HRA,
                                       bank.Remarks,
                                       bank.MedicalRecovery,
                                       bank.PaymentMonthYear,
                                       bank.Deduction,
                                       bank.DirectAllowance,
                                       bank.GrossTotal,
                                       bank.NetSalary,
                                       user
                                   }).ToList();
                        if (Qry != null)
                        {
                            for (int k = 0; k < Qry.Count; k++)
                            {
                                int PaymentId = Qry[k].PaymentId;
                                var AdhocBrk = (from ad in context.tblAdhocSalaryBreakUpDetail
                                                where ad.PaymentId == PaymentId
                                                select new { ad }).ToList();
                                decimal HRA = Qry[k].HRA ?? 0; decimal? Basic = Qry[k].Basic ?? 0; decimal MA = Qry[k].MA ?? 0;
                                decimal QryMedicalRecovery = Qry[k].MedicalRecovery ?? 0;
                                decimal? MiscPay = AdhocBrk.Where(m => m.ad.CategoryId == 1).Sum(m => m.ad.Amount) ?? 0;
                                decimal? MiscRecovery = AdhocBrk.Where(m => m.ad.HeadId != 123 && m.ad.HeadId != 110).Sum(m => m.ad.Amount) ?? 0;
                                decimal? MedicalRecovery = AdhocBrk.Where(m => m.ad.HeadId == 123).Sum(m => m.ad.Amount) ?? 0;
                                decimal? LLP = AdhocBrk.Where(m => m.ad.HeadId == 110).Sum(m => m.ad.Amount) ?? 0;
                                model.Add(new AgencyStaffDetailsModel()
                                {
                                    RelieveDate = UserDate.RelieveDate,
                                    ExtensionDate = UserDate.ExtensionDate,
                                    MonthandYear = Qry[k].PaymentMonthYear,
                                    MiscPay = MiscPay,
                                    BasicSalary = Basic,
                                    MA = MA,
                                    MA2 = QryMedicalRecovery,
                                    HRA = HRA,
                                    GrossSalary = Qry[k].GrossTotal ?? 0,
                                    IncomeTax = Qry[k].MonthlyTax ?? 0,
                                    MiscRecovery = MiscRecovery,
                                    MedicalRecovery = MedicalRecovery,
                                    LLP = LLP,
                                    PF = Qry[k].ProfTax ?? 0,
                                    TotalDeduction = Qry[k].Deduction ?? 0,
                                    GrossTotal = Qry[k].NetSalary ?? 0,
                                    OtherPay = Qry[k].OtherAllowance ?? 0,
                                    Remarks = Qry[k].Remarks,
                                    VerifiedBy = Qry[k].user == null ? "" : Qry[k].user.FirstName
                                });
                            }
                        }
                    }

                    //Current month
                    if (!string.IsNullOrEmpty(monthYear))
                    {
                        month = monthYear;
                        var queryOtherLine = context.tblRCTPayrollProcessDetail.Where(m => m.EmployeeId == empId && m.SalaryMonth == monthYear && m.SalaryType == 1 && m.ProcessStatus == "Active").ToList();

                        decimal MiscPay = 0,
                         MiscRecovery = 0,
                         MedicalRecovery = 0,
                         OtherRecovery = 0,
                         LLP = 0;
                        queryOtherLine.ForEach(m =>
                        {
                            MiscPay = m.Spl_Allowance.GetValueOrDefault(0) + m.Transport_Allowance.GetValueOrDefault(0)
                            + m.PF_Revision.GetValueOrDefault(0) + m.ESIC_Revision.GetValueOrDefault(0) + m.Round_off.GetValueOrDefault(0)
                            + m.Arrears.GetValueOrDefault(0) + m.OthersPay.GetValueOrDefault(0) + m.HRA_Arrears.GetValueOrDefault(0);
                            OtherRecovery = m.Contribution_to_PF.GetValueOrDefault(0) + m.HRA_Recovery.GetValueOrDefault(0)
                            + m.OthersDeduction.GetValueOrDefault(0) + m.Professional_tax.GetValueOrDefault(0);
                            MiscRecovery = m.Recovery.GetValueOrDefault(0);
                            MedicalRecovery = m.Medical_Recovery.GetValueOrDefault(0);
                            LLP = m.Loss_Of_Pay.GetValueOrDefault(0);
                        });

                        decimal currMonthBasic = queryOtherLine.Sum(m => m.CurrentBasic) ?? 0;
                        decimal currBasicHra = queryOtherLine.Sum(m => m.CurrentHRA) ?? 0;
                        decimal currBasicMA = queryOtherLine.Sum(m => m.CurrentMedical) ?? 0;
                        var otherAllow = asp.GetEmpOtherAllowance(empId);
                        var otherAllowAmt = otherAllow != null ? otherAllow.Sum(m => m.Amount) : 0;
                        model.Add(new AgencyStaffDetailsModel()
                        {
                            MonthandYear = monthYear,
                            MiscPay = MiscPay,
                            BasicSalary = currMonthBasic,
                            MA = 0,
                            MA2 = currBasicMA,
                            HRA = currBasicHra,
                            GrossSalary = currMonthBasic + currBasicHra + MiscPay + otherAllowAmt,
                            IncomeTax = 0,
                            MiscRecovery = MiscRecovery,
                            MedicalRecovery = MedicalRecovery,
                            LLP = LLP,
                            PF = 0,
                            TotalDeduction = MiscRecovery + LLP + currBasicMA + MedicalRecovery + OtherRecovery,
                            GrossTotal = ((currMonthBasic + otherAllowAmt + currBasicHra + MiscPay) - (MiscRecovery + LLP + currBasicMA + MedicalRecovery + OtherRecovery)),
                            OtherPay = otherAllowAmt
                        });

                    }
                    /////// Future Month Salary Data  ////////
                    DateTime nextMonthStartDate = Common.GetNextMonthFirstDate(month, 1);
                    if (finEndDate > nextMonthStartDate)
                    {
                        List<string> futurMonth = new List<string>();
                        var query = context.vw_RCTAdhocEmployeeMaster.Where(m => m.EmployeeId == empId &&
                     m.AppointmentDate <= finEndDate && ((m.ExtensionDate >= nextMonthStartDate && m.RelieveDate == null && m.ActualAppointmentEndDate == null)
                                        || (m.RelieveDate >= nextMonthStartDate && m.RelieveDate != null) || (m.ActualAppointmentEndDate >= nextMonthStartDate && m.ActualAppointmentEndDate != null && m.RelieveDate == null))).ToList();
                        foreach (var item in query)
                        {
                            DateTime tDate;
                            decimal Basic = item.Basic.GetValueOrDefault(0);
                            decimal HRA = item.HRA.GetValueOrDefault(0);
                            decimal MA = item.Medical.GetValueOrDefault(0);
                            if (item.RelieveDate != null)
                            {
                                tDate = Convert.ToDateTime(item.RelieveDate);
                            }
                            else if (item.ActualAppointmentEndDate != null)
                            {
                                tDate = Convert.ToDateTime(item.ActualAppointmentEndDate);
                            }
                            else
                            {
                                tDate = Convert.ToDateTime(item.ExtensionDate);
                            }
                            if (finEndDate < tDate)
                            {
                                salaryEndDate = finEndDate;
                            }
                            else
                            {
                                salaryEndDate = tDate;
                            }
                            var _asp = new AdhocSalaryProcess();
                            var listOfMonthDays = _asp.GetMonthNumberOfDays(nextMonthStartDate, salaryEndDate);
                            listOfMonthDays.Reverse();
                            foreach (var m in listOfMonthDays)
                            {
                                if (!futurMonth.Contains(m.MonthYear))
                                    futurMonth.Add(m.MonthYear);
                                int salMonthDays = m.TotalDays;
                                int noOfDays = m.TotalPresentDays;

                                if (salMonthDays == noOfDays)
                                {
                                    model.Add(new AgencyStaffDetailsModel()
                                    {
                                        RelieveDate = UserDate.RelieveDate,
                                        ExtensionDate = UserDate.ExtensionDate,
                                        MonthandYear = m.MonthYear,
                                        MiscPay = 0,
                                        BasicSalary = Basic,
                                        MA = 0,
                                        MA2 = MA,
                                        HRA = HRA,
                                        GrossSalary = Basic + HRA,
                                        IncomeTax = 0,
                                        MiscRecovery = 0,
                                        LLP = 0,
                                        PF = 0,
                                        TotalDeduction = MA,
                                        GrossTotal = Basic + HRA - MA,
                                        OtherPay = 0
                                    });
                                }
                                else
                                {
                                    decimal elgAmount = Basic + HRA;
                                    decimal partialSal = (elgAmount / salMonthDays) * noOfDays;
                                    model.Add(new AgencyStaffDetailsModel()
                                    {
                                        RelieveDate = UserDate.RelieveDate,
                                        ExtensionDate = UserDate.ExtensionDate,
                                        MonthandYear = m.MonthYear,
                                        MiscPay = 0,
                                        BasicSalary = partialSal,
                                        MA = 0,
                                        MA2 = 0,
                                        HRA = HRA,
                                        GrossSalary = partialSal,
                                        IncomeTax = 0,
                                        MiscRecovery = 0,
                                        LLP = 0,
                                        PF = 0,
                                        TotalDeduction = 0,
                                        GrossTotal = partialSal,
                                        OtherPay = 0
                                    });
                                }
                            }
                        }
                        // extenion tenure not stared
                        var currDate = DateTime.Now;
                        var query1 = (from o in context.tblOrder
                                      join ste in context.tblRCTSTE on o.AppointmentId equals ste.STEID
                                      where o.OrderType == 3 && o.Status == "Completed" &&
                                       o.FromDate > currDate && o.AppointmentType == 2 && ste.EmployeersID == empId &&
                                                      o.FromDate <= finEndDate && o.ToDate >= nextMonthStartDate
                                      select o).ToList();
                        foreach (var item in query1)
                        {
                            DateTime tDate = Convert.ToDateTime(item.ToDate);
                            decimal Basic = item.Basic.GetValueOrDefault(0);
                            decimal HRA = item.HRA.GetValueOrDefault(0);
                            decimal MA = item.MedicalAmount.GetValueOrDefault(0);
                            DateTime nexttenuredate = item.ActualAppointmentStartDate ?? nextMonthStartDate;

                            if (finEndDate < tDate)
                            {
                                salaryEndDate = finEndDate;
                            }
                            else
                            {
                                salaryEndDate = tDate;
                            }
                            var _asp = new AdhocSalaryProcess();

                            var listOfMonthDays = _asp.GetMonthNumberOfDays(nexttenuredate, salaryEndDate);
                            //var listOfMonthDays = _asp.GetMonthNumberOfDays(nextMonthStartDate, salaryEndDate);                           

                            listOfMonthDays.Reverse();
                            foreach (var m in listOfMonthDays)
                            {
                                if (!futurMonth.Contains(m.MonthYear))
                                    futurMonth.Add(m.MonthYear);
                                int salMonthDays = m.TotalDays;
                                int noOfDays = m.TotalPresentDays;

                                if (salMonthDays == noOfDays)
                                {
                                    model.Add(new AgencyStaffDetailsModel()
                                    {
                                        RelieveDate = UserDate.RelieveDate,
                                        ExtensionDate = UserDate.ExtensionDate,
                                        MonthandYear = m.MonthYear,
                                        MiscPay = 0,
                                        BasicSalary = Basic,
                                        MA = 0,
                                        MA2 = MA,
                                        HRA = HRA,
                                        GrossSalary = (Basic) + (0) + (HRA) + (0),
                                        IncomeTax = 0,
                                        MiscRecovery = 0,
                                        LLP = 0,
                                        PF = 0,
                                        TotalDeduction = MA + 0 + 0 + (0) + (0),
                                        GrossTotal = ((Basic + 0 + HRA + 0) - (MA + 0 + 0 + (0) + (0))),
                                        OtherPay = 0
                                    });
                                }
                                else
                                {
                                    decimal elgAmount = Basic + HRA;
                                    decimal partialSal = (elgAmount / salMonthDays) * noOfDays;
                                    model.Add(new AgencyStaffDetailsModel()
                                    {
                                        RelieveDate = UserDate.RelieveDate,
                                        ExtensionDate = UserDate.ExtensionDate,
                                        MonthandYear = m.MonthYear,
                                        MiscPay = 0,
                                        BasicSalary = partialSal,
                                        MA = 0,
                                        MA2 = 0,
                                        HRA = HRA,
                                        GrossSalary = partialSal,
                                        IncomeTax = 0,
                                        MiscRecovery = 0,
                                        LLP = 0,
                                        PF = 0,
                                        TotalDeduction = 0,
                                        GrossTotal = partialSal,
                                        OtherPay = 0
                                    });
                                }
                            }
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public List<AgencyStaffDetailsModel> GetEmployeesSupplymentarySalaryDetails(string empId, int Finyear)
        {
            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
            AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    DateTime fromdate = context.tblFinYear.Where(m => m.FinYearId == Finyear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;
                    var Qry = (from bank in context.tblSalaryPayment
                               join sal in context.tblSalaryPaymentHead on bank.PaymentHeadId equals sal.PaymentHeadId
                               join user in context.tblUser on bank.CreatedBy equals user.UserId into g
                               from user in g.DefaultIfEmpty()
                               where bank.PayBill == empId && sal.TypeOfPayBill == 2
                               select new
                               {
                                   bank.Basic,
                                   bank.MonthlyTax,
                                   bank.ProfTax,
                                   bank.OtherAllowance,
                                   bank.PaymentId,
                                   bank.PaymentMonthYear,
                                   bank.MA,
                                   bank.HRA,
                                   bank.Remarks,
                                   user
                               }).ToList();
                    if (Qry != null)
                    {
                        for (int k = 0; k < Qry.Count; k++)
                        {
                            int PaymentId = Qry[k].PaymentId;
                            var AdhocBrk = (from ad in context.tblAdhocSalaryBreakUpDetail
                                            where ad.PaymentId == PaymentId
                                            select new { ad }).ToList();
                            decimal HRA = Qry[k].HRA ?? 0; decimal? Basic = Qry[k].Basic ?? 0; decimal MA = Qry[k].MA ?? 0;
                            decimal? MiscPay = AdhocBrk.Where(m => m.ad.CategoryId == 1).Sum(m => m.ad.Amount) ?? 0;
                            decimal? MiscRecovery = AdhocBrk.Where(m => m.ad.HeadId == 4).Sum(m => m.ad.Amount) ?? 0;
                            decimal? MedicalRecovery = AdhocBrk.Where(m => m.ad.HeadId == 123).Sum(m => m.ad.Amount) ?? 0;
                            decimal? LLP = AdhocBrk.Where(m => m.ad.HeadId == 110).Sum(m => m.ad.Amount) ?? 0;
                            model.Add(new AgencyStaffDetailsModel()
                            {
                                MonthandYear = Qry[k].PaymentMonthYear,
                                MiscPay = MiscPay,
                                BasicSalary = Basic,
                                MA = MA,
                                HRA = HRA,
                                GrossSalary = (Basic) + (MA) + (HRA) + (MiscPay),
                                IncomeTax = Qry[k].MonthlyTax ?? 0,
                                MiscRecovery = MiscRecovery,
                                LLP = LLP,
                                MedicalRecovery = MedicalRecovery,
                                PF = Qry[k].ProfTax ?? 0,
                                TotalDeduction = MA + MiscRecovery + LLP + (Qry[k].MonthlyTax ?? 0) + (Qry[k].ProfTax ?? 0),
                                GrossTotal = ((Basic + MA + HRA + MiscPay) - (MA + MiscRecovery + LLP + (Qry[k].MonthlyTax ?? 0) + (Qry[k].ProfTax ?? 0))),
                                OtherPay = Qry[k].OtherAllowance ?? 0,
                                Remarks = Qry[k].Remarks,
                                VerifiedBy = Qry[k].user == null ? "" : Qry[k].user.FirstName
                            });
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public ITDeclarationViewModel GetDeclarationDetails(string EmpId)
        {
            ITDeclarationViewModel model = new ITDeclarationViewModel();
            List<ITDeclarationModel> declarList = new List<ITDeclarationModel>();
            List<EmpITOtherIncomeModel> otherIncomeList = new List<EmpITOtherIncomeModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    declarList = (from AI in context.tblEmpITDeclaration
                                  where AI.EmpId == EmpId && AI.Amount > 0
                                  orderby AI.SectionCode
                                  select new ITDeclarationModel
                                  {
                                      Particulars = AI.Particulars,
                                      MaxLimit = AI.MaxLimit ?? 0,
                                      Amount = AI.Amount ?? 0
                                      //SectionCode = AI.SectionCode,
                                      //SectionName = AI.SectionName
                                  }).ToList();
                    var commonDedut = Convert.ToDecimal(WebConfigurationManager.AppSettings["Adhoc_Common_Exemption"]);
                    declarList.Add(new ITDeclarationModel()
                    {
                        Particulars = "Standard deduction",
                        Amount = commonDedut,
                        MaxLimit = commonDedut
                    });

                    otherIncomeList = (from AI in context.tblEmpOtherIncome
                                       where AI.EmpId == EmpId && AI.Amount > 0
                                       orderby AI.ID
                                       select new EmpITOtherIncomeModel
                                       {
                                           Particulars = AI.Particulars,
                                           Remarks = AI.Remarks,
                                           Amount = AI.Amount ?? 0
                                       }).ToList();
                    model.declaration = declarList;
                    model.otherIncome = otherIncomeList;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.declaration = declarList;
                model.otherIncome = otherIncomeList;
                return model;
            }
        }

        public string GetDeclarationRemarks(string empNo)
        {
            string remarks = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var remarkQry = context.tblITDeclarationRemarks
                      .FirstOrDefault(it => it.PayBill == empNo);
                    if (remarkQry != null)
                        remarks = remarkQry.Remarks;
                    return remarks;
                }
            }
            catch (Exception ex)
            {
                return remarks;
            }
        }
        public List<EmpOfficeOrderModel> GetOfficeOrderList(string PayBillNo)
        {
            try
            {

                List<EmpOfficeOrderModel> list = new List<EmpOfficeOrderModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from b in context.VWOfficeOrder
                            orderby b.CreatedDate descending
                            where b.paybillNo == PayBillNo
                            select new
                            {
                                b.BasicPay,
                                b.CreatedDate,
                                b.fileno,
                                b.FromDate,
                                b.HRA,
                                b.Medical,
                                b.Torder,
                                b.ProjectNo,
                                b.RelievedDate,
                                b.Todate,
                                b.DesignationCode
                            })
                                 .AsEnumerable()
                                 .Select((x, index) => new EmpOfficeOrderModel()
                                 {
                                     FileNo = x.fileno,
                                     Basic = x.BasicPay,
                                     Crt_TS = String.Format("{0:dd-MM-yyyy}", x.CreatedDate),
                                     FromDate = String.Format("{0:dd-MM-yyyy}", x.FromDate),
                                     HRA = x.HRA,
                                     MA = x.Medical,
                                     OrderType = x.Torder,
                                     ProjectNo = x.ProjectNo,
                                     Designation = x.DesignationCode,
                                     RelieveDate = String.Format("{0:dd-MM-yyyy}", x.RelievedDate),
                                     ToDate = String.Format("{0:dd-MM-yyyy}", x.Todate)
                                 }).ToList();

                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<EmpOfficeOrderModel>();
            }
        }
        #endregion
    }

    public class FinOp
    {
        private string StartingMonth = "Apr";
        private string SalStartingMonth = "Mar";
        private int StartingDate = 1;
        private string EndingMonth = "Mar";
        private string SalEndingMonth = "Feb";
        private int EndingDate = 31;

        private string _SalStartMonth = "";
        private DateTime _FinStart;
        private DateTime _FinEnd;
        private DateTime _FinSalStart;
        private DateTime _FinSalEnd;
        private DateTime _Today = DateTime.Today;

        string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        private DateTime FinStartDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                dtResult = new DateTime(_Today.Year - 1, month, StartingDate);
            }
            else
            {
                dtResult = new DateTime(_Today.Year, month, StartingDate);
            }
            return dtResult;
        }
        private DateTime FinEndDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = _Today.Year + 1;
            if (month >= CurrentMonth && _Today.Year == CurrentYear)
            {
                dtResult = new DateTime(_Today.Year, month, EndingDate);
            }
            else
            {
                dtResult = new DateTime(nextYear, month, EndingDate);
            }
            return dtResult;
        }
        private DateTime SalFinStartDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(SalStartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                dtResult = new DateTime(_Today.Year - 1, month, StartingDate);
            }
            else
            {
                dtResult = new DateTime(_Today.Year, month, StartingDate);
            }
            return dtResult;
        }
        private DateTime SalFinEndDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(SalEndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = _Today.Year + 1;
            if (month >= CurrentMonth && _Today.Year == CurrentYear)
            {
                int noEd = 28;
                if (_Today.Year % 4 != 0)
                    noEd = 29;
                dtResult = new DateTime(_Today.Year, month, noEd);
            }
            else
            {
                int noEd = 28;
                if (nextYear % 4 != 0)
                    noEd = 29;
                dtResult = new DateTime(nextYear, month, noEd);
            }
            return dtResult;
        }
        private void FinStart(int year)
        {
            //DateTime.ParseExact(StartingMonth, "MMMM", CultureInfo.CurrentCulture).Month;
            int month = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            //int CurrentMonth = _Today.Month;
            //if (month > CurrentMonth)
            //{
            //    _FinStart = new DateTime(year - 1, month, StartingDate);
            //}
            //else
            //{
            _FinStart = new DateTime(year, month, StartingDate);
            //}

        }
        private void FinEnd(int year)
        {
            int month = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            //int CurrentMonth = _Today.Month;
            //int CurrentYear = _Today.Year;
            //int nextYear = year + 1;
            //if (month >= CurrentMonth && year == CurrentYear)
            //{
            _FinEnd = new DateTime(year, month, EndingDate);
            //}
            //else
            //{
            //    _FinEnd = new DateTime(nextYear, month, EndingDate);
            //}
        }
        private void SalFinStart(int year)
        {
            //DateTime.ParseExact(StartingMonth, "MMMM", CultureInfo.CurrentCulture).Month;
            int month = DateTime.ParseExact(SalStartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            //int CurrentMonth = _Today.Month;
            //if (month > CurrentMonth)
            //{
            //    _FinSalStart = new DateTime(year - 1, month, StartingDate);
            //}
            //else
            //{
            _FinSalStart = new DateTime(year, month, StartingDate);
            _SalStartMonth = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + year.ToString();
            //}

        }
        private void SalFinEnd(int year)
        {
            int month = DateTime.ParseExact(SalEndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            //int CurrentMonth = _Today.Month;
            //int CurrentYear = _Today.Year;
            //int nextYear = year + 1;
            //if (month >= CurrentMonth && year == CurrentYear)
            //{
            int noEd = DateTime.DaysInMonth(year, month);
            _FinSalEnd = new DateTime(year, month, noEd);
            //}
            //else
            //{
            //    int noEd = DateTime.DaysInMonth(nextYear, month);
            //    _FinSalEnd = new DateTime(nextYear, month, noEd);
            //}
        }

        public DateTime FinStart()
        {
            return _FinStart;
        }
        public DateTime FinEnd()
        {
            return _FinEnd;
        }
        public DateTime SalFinStart()
        {
            return _FinSalStart;
        }
        public string SalStartMonth()
        {
            return _SalStartMonth;
        }
        public DateTime SalFinEnd()
        {
            return _FinSalEnd;
        }
        public FinOp(int year)
        {
            FinStart(year);
            FinEnd(year + 1);
            SalFinStart(year);
            SalFinEnd(year + 1);
        }
        public FinOp(string MonthYear, int paybillType)
        {
            int salFinYear = 0;
            int finYear = 0;
            string[] dt = MonthYear.Split('-');
            if (dt.Length > 0)
            {
                int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                int year = Convert.ToInt32(dt[1].Trim());
                int salStartMonth = DateTime.ParseExact(SalStartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                if (month >= startMonth)
                {
                    finYear = year;
                }
                else if (month < startMonth)
                {
                    finYear = year - 1;
                }
                if (month > salStartMonth || (month == salStartMonth && paybillType == 1))
                {
                    salFinYear = year;
                }
                else //if (month < salStartMonth)
                {
                    salFinYear = year - 1;
                }
            }
            FinStart(finYear);
            FinEnd(finYear + 1);
            SalFinStart(salFinYear);
            SalFinEnd(salFinYear + 1);
        }
        public FinOp(DateTime dt)
        {
            int year = System.DateTime.Now.Year;
            int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            if (dt.Month >= startMonth)
            {
                year = dt.Year;
            }
            else if (dt.Month < startMonth)
            {
                year = dt.Year - 1;
            }
            FinStart(year);
            FinEnd(year);
            SalFinStart(year);
            SalFinEnd(year);
        }


        //----Given by RajKumar----
        public FinOp(DateTime dt, bool payroll)
        {
            int year = System.DateTime.Now.Year;
            int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            if (dt.Month >= startMonth)
            {
                year = dt.Year;
            }
            else if (dt.Month < startMonth)
            {
                year = dt.Year - 1;
            }
            FinStartpayroll(year);
            FinEndpayroll(year);
            SalFinStartpayroll(year);
            SalFinEndpayroll(year);
        }
        private void FinEndpayroll(int year)
        {
            int month = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = year + 1;
            if (month >= CurrentMonth && year == CurrentYear)
            {
                _FinEnd = new DateTime(year, month, EndingDate);
            }
            else
            {
                _FinEnd = new DateTime(nextYear, month, EndingDate);
            }
        }

        private void FinStartpayroll(int year)
        {
            //DateTime.ParseExact(StartingMonth, "MMMM", CultureInfo.CurrentCulture).Month;
            int month = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                _FinStart = new DateTime(year - 1, month, StartingDate);
            }
            else
            {
                _FinStart = new DateTime(year, month, StartingDate);
            }

        }

        private void SalFinStartpayroll(int year)
        {
            //DateTime.ParseExact(StartingMonth, "MMMM", CultureInfo.CurrentCulture).Month;
            int month = DateTime.ParseExact(SalStartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                _FinSalStart = new DateTime(year - 1, month, StartingDate);
            }
            else
            {
                _FinSalStart = new DateTime(year, month, StartingDate);
            }

        }

        private void SalFinEndpayroll(int year)
        {
            int month = DateTime.ParseExact(SalEndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = year + 1;
            if (month >= CurrentMonth && year == CurrentYear)
            {
                int noEd = DateTime.DaysInMonth(year, month);
                _FinSalEnd = new DateTime(year, month, noEd);
            }
            else
            {
                int noEd = DateTime.DaysInMonth(nextYear, month);
                _FinSalEnd = new DateTime(nextYear, month, noEd);
            }
        }

        //-----End-----


        public FinOp getFinPeriod(DateTime start, DateTime end)
        {
            DateTime startDt;
            DateTime endDt;
            startDt = start;
            if (start > end)
            {
                startDt = end;
                endDt = start;
            }
            FinOp period = new FinOp(startDt);

            return period;
        }

        public Dictionary<string, string> GetAllMonths()
        {
            Dictionary<string, string> monthYear = new Dictionary<string, string>(12);

            DateTime dtStart = FinStartDate();
            DateTime dtEnd = FinEndDate();
            int prevStartYear = dtStart.Year - 1;
            int prevStartMonth = dtEnd.Month;
            int prevEndYear = dtStart.Year;
            int prevEndMonth = dtStart.Month;

            for (int i = prevStartMonth; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevStartYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i < prevEndMonth; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevEndYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = dtStart.Month; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtStart.Year.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i <= dtEnd.Month; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtEnd.Year.ToString();
                monthYear.Add(key, key);
            }
            return monthYear;
        }
        public Dictionary<string, string> GetSalAllMonths()
        {
            Dictionary<string, string> monthYear = new Dictionary<string, string>(12);

            DateTime dtStart = SalFinStartDate();
            DateTime dtEnd = SalFinEndDate();
            int prevStartYear = dtStart.Year - 1;
            int prevStartMonth = dtEnd.Month;
            int prevEndYear = dtStart.Year;
            int prevEndMonth = dtStart.Month;

            for (int i = prevStartMonth; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevStartYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i < prevEndMonth; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevEndYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = dtStart.Month; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtStart.Year.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i <= dtEnd.Month; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtEnd.Year.ToString();
                monthYear.Add(key, key);
            }
            return monthYear;
        }
        public List<string> GetAllSalMonths()
        {
            List<string> monthYear = new List<string>();

            DateTime dtStart = SalFinStartDate(); // _FinSalStart; is changed by riyaz
            int nxtStartYear = dtStart.Year + 1;
            int currEndYear = dtStart.Year;
            int currmonth = DateTime.Now.Month;
            var ss =currmonth-1;
            for (int i = currmonth; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + currEndYear.ToString();
                monthYear.Add(key);   
            }
            for (int i = 1; i <= ss; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + nxtStartYear.ToString();
                monthYear.Add(key);
            }
            return monthYear;
        }
        public List<string> GetAllSalaryMonths()
        {
            List<string> monthYear = new List<string>();

            DateTime dtStart = _FinSalStart;
            int nxtStartYear = dtStart.Year + 1;
            int currEndYear = dtStart.Year;

            for (int i = 3; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + currEndYear.ToString();
                monthYear.Add(key);
            }
            for (int i = 1; i <= 2; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + nxtStartYear.ToString();
                monthYear.Add(key);
            }
            return monthYear;
        }
        public List<string> GetAllSalMonths(string MonthYear)
        {
            string[] dt = MonthYear.Split('-');
            int finYear = 0;
            if (dt.Length > 0)
            {
                int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                if (month >= startMonth)
                {
                    finYear = DateTime.ParseExact(dt[1].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Year;
                }
                else if (month < startMonth)
                {
                    finYear = DateTime.ParseExact(dt[1].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Year - 1;
                }
            }
            List<string> monthYear = new List<string>();

            int nxtStartYear = finYear + 1;
            int currEndYear = finYear;

            for (int i = 3; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + currEndYear.ToString();
                monthYear.Add(key);
            }
            for (int i = 1; i <= 2; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + nxtStartYear.ToString();
                monthYear.Add(key);
            }
            return monthYear;
        }
        public List<string> GetPTPeriod(string MonthYear)
        {
            List<string> monthYear = new List<string>();
            string[] dt = MonthYear.Split('-');
            if (dt.Length > 0)
            {
                int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                DateTime dtStart = SalFinStart();
                int nxtStartYear = dtStart.Year + 1;
                int currEndYear = dtStart.Year;

                if (month >= 3 && month <= 8)
                {
                    for (int i = 3; i <= 8; i++)
                    {
                        var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + currEndYear.ToString();
                        monthYear.Add(key);
                    }
                }
                else
                {
                    for (int i = 9; i <= 12; i++)
                    {
                        var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + currEndYear.ToString();
                        monthYear.Add(key);
                    }
                    for (int i = 1; i <= 2; i++)
                    {
                        var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + nxtStartYear.ToString();
                        monthYear.Add(key);
                    }
                }
            }
            return monthYear;
        }
        public string GetCurrentMonthYear()
        {
            int month = _Today.Month;
            int year = _Today.Year;
            string currentMonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + year.ToString();

            return currentMonthYear;
        }
        public string GetFinPeriod(string MonthYear)
        {
            string finPeriod = string.Empty;
            string[] dt = MonthYear.Split('-');
            if (dt.Length > 0)
            {
                int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                int year = Convert.ToInt32(dt[1].Trim().Substring(dt[1].Trim().Length - 2));
                int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                if (month >= startMonth)
                {
                    finPeriod = year.ToString() + (year + 1).ToString();
                }
                else if (month < startMonth)
                {
                    finPeriod = (year - 1).ToString() + year.ToString();
                }
            }
            return finPeriod;
        }

        public string GetPreviousMonth(string MonthYear)
        {
            string preMonthYear = string.Empty;
            string preYear = string.Empty;
            string[] dt = MonthYear.Split('-');
            if (dt.Length > 0)
            {
                int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                int year = Convert.ToInt32(dt[1].Trim());
                int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
                if (month == startMonth)
                {
                    preYear = (year - 1).ToString();
                }
                else
                    preYear = year.ToString();
                if (month == 1)
                    month = 12;
                else
                    month = month - 1;
                preMonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + preYear;
            }
            return preMonthYear;
        }
        //public DateTime GetMonthLastDate(string MonthYear)
        //{
        //    try
        //    {
        //        DateTime lastDate = DateTime.Now;
        //        string[] dt = MonthYear.Split('-');
        //        if (dt.Length > 0)
        //        {
        //            int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
        //            int year = Convert.ToInt32(dt[1].Trim());
        //            lastDate = new DateTime(year, month,
        //                            DateTime.DaysInMonth(year, month));
        //        }
        //        return lastDate;
        //    }
        //    catch (Exception ex)
        //    {
        //        return DateTime.Now;
        //    }
        //}

        //public DateTime GetMonthFirstDate(string MonthYear)
        //{
        //    try
        //    {
        //        DateTime firstDate = DateTime.Now;
        //        string[] dt = MonthYear.Split('-');
        //        if (dt.Length > 0)
        //        {
        //            int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
        //            int year = Convert.ToInt32(dt[1].Trim());
        //            firstDate = new DateTime(year, month, 1);
        //        }
        //        return firstDate;
        //    }
        //    catch (Exception ex)
        //    {
        //        return DateTime.Now;
        //    }
        //}

        public List<KeyValuePair<string, int>> GetListMonthDays(DateTime start, DateTime end)
        {
            try
            {
                List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
                DateTime lastDate = DateTime.Now;
                DateTime tempDate = DateTime.Now;
                DateTime monthStartDate;
                DateTime monthEndDate;
                string paymonthYear = "";

                int startMonth = start.Month;
                int endMonth = end.Month;
                if (start > end)
                {
                    tempDate = start;
                    start = end;
                    end = tempDate;
                    startMonth = start.Month;
                    endMonth = end.Month;
                }
                if (start == end)
                {
                    return list;
                }

                for (int i = startMonth; i < endMonth; i++)
                {
                    monthStartDate = new DateTime(start.Year, start.Month, start.Day);
                    monthEndDate = new DateTime(start.Year, start.Month,
                                    DateTime.DaysInMonth(start.Year, start.Month));
                    if (start.Year == end.Year)
                    {

                    }

                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + start.Year.ToString();
                    int diff = (monthEndDate.Day - monthStartDate.Day);
                    var keyValue = new KeyValuePair<string, int>(paymonthYear, diff);
                    list.Add(keyValue);

                }


                //// If From Date's Day is bigger than borrow days from previous month
                //// & then subtract.
                //if (start.Day > end.Day)
                //{
                //    objDateTimeToDate = objDateTimeToDate.AddMonths(-1);
                //    int nMonthDays = DateTime.DaysInMonth(objDateTimeToDate.Year, objDateTimeToDate.Month);
                //    m_nDays = objDateTimeToDate.Day + nMonthDays - objDateTimeFromDate.Day;

                //}

                //string[] dt = MonthYear.Split('-');
                //if (dt.Length > 0)
                //{
                //    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                //    int year = Convert.ToInt32(dt[1].Trim());
                //    lastDate = new DateTime(year, month, 1);
                //}
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool IsFutureMonthYear(string MonthYear)
        {
            try
            {
                DateTime dtStart = FinStartDate();
                DateTime dtEnd = FinEndDate();

                bool isValid = false;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int pre_startMonth = DateTime.ParseExact(StartingMonth.ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int pre_EndMonth = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int nxt_startMonth = DateTime.ParseExact(EndingMonth.ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int nxt_EndMonth = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    if (_Today.Year == Convert.ToInt32(dt[1].Trim()) && month <= _Today.Month)
                    {
                        isValid = true;
                    }
                    else if (Convert.ToInt32(dt[1].Trim()) < _Today.Year && month <= 12)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }

                    //int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    //if (_Today.Year <= Convert.ToInt32(dt[1].Trim()) && month <= _Today.Month)
                    //{
                    //    isValid = true;
                    //}
                }
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<KeyValuePair<int, string>> GetAllMonths(int year)
        {
            for (int i = 1; i <= 12; i++)
            {
                yield return new KeyValuePair<int, string>(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + year.ToString());
            }
        }

        public string GetCurrentMonthYear(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            string currentMonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + year.ToString();
            return currentMonthYear;
        }
        public DateTime GetMonthLastDate(string MonthYear)
        {
            try
            {
                DateTime lastDate = DateTime.Now;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int year = Convert.ToInt32(dt[1].Trim());
                    lastDate = new DateTime(year, month,
                                    DateTime.DaysInMonth(year, month));
                }
                return lastDate;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        public DateTime GetMonthFirstDate(string MonthYear)
        {
            try
            {
                DateTime firstDate = DateTime.Now;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int year = Convert.ToInt32(dt[1].Trim());
                    firstDate = new DateTime(year, month, 1);
                }
                return firstDate;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

    }

    public class DateOp
    {
        private int m_nDays = -1;
        private int m_nWeek;
        private int m_nMonth = -1;
        private int m_nYear;

        private DateTime Today = System.DateTime.Now;

        public int Days
        {
            get
            {
                return m_nDays;
            }
        }

        public int Weeks
        {
            get
            {
                return m_nWeek;
            }
        }

        public int Months
        {
            get
            {
                return m_nMonth;
            }
        }

        public int Years
        {
            get
            {
                return m_nYear;
            }
        }

        public void GetDiff(DateTime objDateTimeFromDate, DateTime objDateTimeToDate)
        {
            if (objDateTimeFromDate.Date > objDateTimeToDate.Date)
            {
                DateTime objDateTimeTemp = objDateTimeFromDate;
                objDateTimeFromDate = objDateTimeToDate;
                objDateTimeToDate = objDateTimeTemp;
            }

            if (objDateTimeFromDate == objDateTimeToDate)
            {
                //textBoxDifferenceDays.Text = " Same dates";
                //textBoxAllDifference.Text = " Same dates";
                return;
            }

            // If From Date's Day is bigger than borrow days from previous month
            // & then subtract.
            if (objDateTimeFromDate.Day > objDateTimeToDate.Day)
            {
                objDateTimeToDate = objDateTimeToDate.AddMonths(-1);
                int nMonthDays = DateTime.DaysInMonth(objDateTimeToDate.Year, objDateTimeToDate.Month);
                m_nDays = objDateTimeToDate.Day + nMonthDays - objDateTimeFromDate.Day;

            }

            // If From Date's Month is bigger than borrow 12 Month 
            // & then subtract.
            if (objDateTimeFromDate.Month > objDateTimeToDate.Month)
            {
                objDateTimeToDate = objDateTimeToDate.AddYears(-1);
                m_nMonth = objDateTimeToDate.Month + 12 - objDateTimeFromDate.Month;

            }

            //Below are best cases - simple subtraction
            if (m_nDays == -1)
            {
                m_nDays = objDateTimeToDate.Day - objDateTimeFromDate.Day;
            }

            if (m_nMonth == -1)
            {
                m_nMonth = objDateTimeToDate.Month - objDateTimeFromDate.Month;
            }

            m_nYear = objDateTimeToDate.Year - objDateTimeFromDate.Year;
            m_nWeek = m_nDays / 7;
            m_nDays = (m_nDays % 7) + (m_nWeek * 7);

            if (m_nYear > 0)
            {
                m_nMonth = m_nMonth + (m_nYear * 12);
            }
        }


        public int GetMonth(DateTime dt)
        {
            return dt.Month;
        }

        public int GetYear(DateTime dt)
        {
            return dt.Year;
        }

        public int CurrentMonth()
        {
            return Today.Month;
        }
        public int CurrentYear()
        {
            return Today.Year;
        }
        public int NextYear()
        {
            return Today.Year + 1;
        }
    }

    public class AdhocSalaryProcess
    {
        //FinOp fo = new FinOp(System.DateTime.Now);
        StaffPaymentService payment = new StaffPaymentService();
        CoreAccountsService coreAccountService = new CoreAccountsService();

        string EmployeeNo { get; set; }
        string EmployeeName { get; set; }
        string DepartmentCode { get; set; }
        Int32 EmployeeCategory { get; set; }
        public void setFilter(string EmployeeNo, string EmployeeName, string DepartmentCode, int EmployeeCategory)
        {
            this.EmployeeNo = EmployeeNo;
            this.EmployeeName = EmployeeName;
            this.DepartmentCode = DepartmentCode;
            this.EmployeeCategory = EmployeeCategory;
        }

        public Dictionary<Tuple<int, int>, int> GetNumberOfDays(DateTime start, DateTime end)
        {
            // assumes end > start
            Dictionary<Tuple<int, int>, int> ret = new Dictionary<Tuple<int, int>, int>();
            DateTime date = end;
            string paymonthYear = "";

            while (date > start)
            {
                if (date.Year == start.Year && date.Month == start.Month)
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(
                        Tuple.Create<int, int>(date.Year, date.Month),
                        (date - start).Days + 1);
                    break;
                }
                else
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(
                        Tuple.Create<int, int>(date.Year, date.Month),
                        date.Day);
                    date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
                }
            }
            return ret;
        }
        public List<MonthOfDayModel> GetMonthNumberOfDays(DateTime start, DateTime end)
        {
            // assumes end > start
            List<MonthOfDayModel> ret = new List<MonthOfDayModel>();
            DateTime date = end;
            string paymonthYear = "";

            while (date >= start)
            {
                if (date.Year == start.Year && date.Month == start.Month)
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(new MonthOfDayModel()
                    {
                        MonthYear = paymonthYear,
                        TotalDays = DateTime.DaysInMonth(date.Year, date.Month),
                        TotalPresentDays = (date - start).Days + 1
                    });
                    break;
                }
                else
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(new MonthOfDayModel()
                    {
                        MonthYear = paymonthYear,
                        TotalDays = DateTime.DaysInMonth(date.Year, date.Month),
                        TotalPresentDays = date.Day
                    });
                    date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
                }
            }
            return ret;
        }

        private SalaryModel CalculateIT(SalaryModel model, bool verify = false)
        {
            try
            {
                decimal taxAmount = 0;
                decimal monthTaxRound = 0;
                decimal taxPayable = 0;
                decimal cess = 0;
                decimal cessPct = Convert.ToDecimal(WebConfigurationManager.AppSettings["Cess_Percentage"]);

                bool isNotFirst = Common.CheckSalaryProcessed(model.EmployeeId, model.PaymentMonthYear, model.TypeOfPayBill);
                //if (!isNotFirst)
                //{
                //    var data = Common.GetMainSalaryOtherOrderDetail(model.EmployeeId, model.EmployeeId);
                //    if (data.Item1)
                //    {
                //        model.TaxableIncome = model.TaxableIncome + data.Item2;
                //        model.AnnualSalary = model.AnnualSalary + data.Item2;
                //        model.AnnualTaxableSalary = model.AnnualTaxableSalary + data.Item2;
                //    }
                //}
                //else 
                if (isNotFirst && verify && model.MonthlyTax > 0)
                {
                    throw new Exception("Already one line item verify against this employee. please deduct the tax postion againts that first one.");
                }
                else if (isNotFirst)
                {
                    model.TaxPayable = 0;
                    model.Cess = 0;
                    model.MonthlyTax = 0;
                    return model;
                }
                if (model != null && model.taxSlab != null)
                {
                    foreach (var band in model.taxSlab)
                    {
                        if (model.TaxableIncome > band.RangeFrom)
                        {
                            var taxableAtThisRate = Math.Min(band.RangeTo - band.RangeFrom, model.TaxableIncome - band.RangeFrom);
                            var taxThisBand = (taxableAtThisRate / 100) * band.Percentage;
                            taxAmount += taxThisBand;
                        }
                    }
                }
                decimal ttlCESS = taxAmount * cessPct / 100;
                decimal ttlTaxAmt = taxAmount + ttlCESS;
                model.Tax = ttlTaxAmt;
                if (verify && model.MonthlyTax > 0)
                {
                    cessPct = 100 + cessPct;
                    taxPayable = model.MonthlyTax * 100 / cessPct;
                    taxPayable = Math.Round(taxPayable, MidpointRounding.AwayFromZero);
                    cess = Math.Round(model.MonthlyTax - taxPayable, MidpointRounding.AwayFromZero);
                    model.Cess = cess;
                    model.TaxPayable = taxPayable;
                    return model;
                }
                else if (verify)
                {
                    return model;
                }
                ttlTaxAmt = ttlTaxAmt - model.PreviousIT;
                monthTaxRound = ttlTaxAmt / model.ProjectedNoMonths;
                if (monthTaxRound > 0)
                {
                    cess = ttlCESS / model.ProjectedNoMonths;
                    model.MonthlyTax = Math.Round(monthTaxRound, MidpointRounding.AwayFromZero);
                    model.Cess = Math.Round(cess, MidpointRounding.AwayFromZero);
                    model.TaxPayable = model.MonthlyTax - model.Cess;
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        private decimal CalculatePT(string monthYear, string empId, decimal currentMonthPay, int typeOfPaybill)
        {
            try
            {
                decimal pt = 0;
                decimal prevPT = 0;
                decimal taxableIncome = currentMonthPay;
                using (var context = new IOASDBEntities())
                {
                    FinOp fo = new FinOp(monthYear, typeOfPaybill);
                    var periodMonthYearList = fo.GetPTPeriod(monthYear);
                    string finStartMonth = fo.SalStartMonth();
                    var prevSalQuery = (from sp in context.tblSalaryPayment
                                        join ph in context.tblSalaryPaymentHead on sp.PaymentHeadId equals ph.PaymentHeadId
                                        where sp.PayBill == empId && sp.PTExempted != true
                                        && (ph.PaymentMonthYear != monthYear ||
                                    (ph.PaymentMonthYear == monthYear && ph.TypeOfPayBill != typeOfPaybill))
                                        && periodMonthYearList.Contains(ph.PaymentMonthYear) && ph.Status == "Approval Pending"
                                        && (ph.PaymentMonthYear != finStartMonth || (ph.PaymentMonthYear == finStartMonth && ph.TypeOfPayBill != 2))
                                        select sp).ToList();
                    if (prevSalQuery.Count > 0)
                    {
                        decimal preTaxableDA = 0;
                        decimal prevBasic = prevSalQuery.Sum(m => m.Basic) ?? 0;
                        decimal prevHRA = prevSalQuery.Sum(m => m.HRA) ?? 0;
                        prevPT = prevSalQuery.Sum(m => m.ProfTax) ?? 0;
                        var payHeadIds = prevSalQuery.Select(m => m.PaymentId).ToArray();
                        decimal preTaxableOA = (from soa in context.tblAdhocSalaryOtherAllowance
                                                join OA in context.tblEmpOtherAllowance on soa.EmpOtherAllowanceId equals OA.id
                                                join CC in context.tblCodeControl on OA.ComponentName equals CC.CodeValDetail
                                                where CC.CodeName == "OtherAllowance"
                                                && CC.CodeDescription == "True" && payHeadIds.Contains(soa.PaymentId ?? 0)
                                                select soa.Amount).Sum() ?? 0;
                        var preTaxableDAQry = (from da in context.tblAdhocSalaryBreakUpDetail
                                               join cc in context.tblCommonHeads on da.HeadId equals cc.HeadId
                                               where payHeadIds.Contains(da.PaymentId ?? 0)
                                               select new { da.Amount, cc.GroupId, cc.Common_f }).ToList();
                        if (preTaxableDAQry.Count > 0)
                        {
                            decimal taxableDA = preTaxableDAQry.Where(m => m.GroupId == 1).Sum(m => m.Amount) ?? 0;
                            decimal taxableDD = preTaxableDAQry.Where(m => m.GroupId == 2 && m.Common_f == true).Sum(m => m.Amount) ?? 0;
                            preTaxableDA = taxableDA - taxableDD;
                        }
                        taxableIncome += preTaxableDA + prevBasic + preTaxableOA + prevHRA;

                    }
                    var records = (from TS in context.tblTaxSlab
                                   where TS.RangeFrom <= taxableIncome && TS.RangeTo >= taxableIncome
                                   && TS.TaxType == "PT"
                                   select TS).FirstOrDefault();
                    if (records != null)
                    {
                        decimal toBeDeduct = Convert.ToDecimal(records.Percentage);
                        if (toBeDeduct > prevPT)
                            pt = toBeDeduct - prevPT;
                    }

                }
                return pt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
        public SalaryModel GetSalaryDetail_(SalaryModel model, bool verify = false)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    decimal Basic = model.ActualBasic;
                    decimal MA = model.ActualMA;
                    decimal HRA = model.ActualHRA;

                    string MonthYear = model.PaymentMonthYear;
                    DateTime salaryStartDate = Common.GetMonthFirstDate(MonthYear);
                    DateTime salaryEndDate;
                    DateTime nextMonthStartDate = Common.GetNextMonthFirstDate(MonthYear, model.TypeOfPayBill);
                    FinOp fp = new FinOp(model.PaymentMonthYear, model.TypeOfPayBill);
                    DateTime finEndDate = fp.SalFinEnd();
                    DateTime salFinStartDate = fp.SalFinStart();
                    int age = Common.CalculateAge(model.DateOfBirth, salFinStartDate.AddDays(-1));
                    model.taxSlab = GetTaxSlab(model.Gender, age);
                    var finMonthYearList = fp.GetAllSalaryMonths();
                    string finStartMonth = fp.SalStartMonth();
                    decimal amount = 0;
                    decimal taxableAmount = 0;
                    decimal monthSalary = Basic + HRA;
                    decimal currentMonthSalay = model.Basic + model.HRA;
                    model.ProjectedNoMonths = 1;
                    DateTime currDate = DateTime.Now;
                    if (finEndDate > nextMonthStartDate)
                    {
                        List<string> futurMonth = new List<string>();
                        var query = context.vw_RCTAdhocEmployeeMaster.Where(m => m.EmployeeId == model.EmployeeId &&
                     m.AppointmentDate <= finEndDate && ((m.ExtensionDate >= nextMonthStartDate && m.RelieveDate == null && m.ActualAppointmentEndDate == null)
                                        || (m.RelieveDate >= nextMonthStartDate && m.RelieveDate != null) || (m.ActualAppointmentEndDate >= nextMonthStartDate && m.ActualAppointmentEndDate != null && m.RelieveDate == null))).ToList();
                        foreach (var item in query)
                        {
                            DateTime tDate;
                            decimal induBasic = item.Basic.GetValueOrDefault(0);
                            decimal induHRA = item.HRA.GetValueOrDefault(0);
                            if (item.RelieveDate != null)
                            {
                                tDate = Convert.ToDateTime(item.RelieveDate);
                            }
                            else if (item.ActualAppointmentEndDate != null)
                            {
                                tDate = Convert.ToDateTime(item.ActualAppointmentEndDate);
                            }
                            else
                            {
                                tDate = Convert.ToDateTime(item.ExtensionDate);
                            }
                            if (finEndDate < tDate)
                            {
                                salaryEndDate = finEndDate;
                            }
                            else
                            {
                                salaryEndDate = tDate;
                            }
                            var listOfMonthDays = GetMonthNumberOfDays(nextMonthStartDate, salaryEndDate);
                            //int projectedMonth = listOfMonthDays.Count + 1;
                            //if (model.ProjectedNoMonths < projectedMonth)
                            // model.ProjectedNoMonths = projectedMonth;
                            foreach (var m in listOfMonthDays)
                            {
                                if (!futurMonth.Contains(m.MonthYear))
                                    futurMonth.Add(m.MonthYear);
                                int salMonthDays = m.TotalDays;
                                int noOfDays = m.TotalPresentDays;

                                if (salMonthDays == noOfDays)
                                {
                                    taxableAmount += induBasic + induHRA;
                                }
                                else
                                {
                                    decimal elgAmount = induBasic + induHRA;
                                    decimal partialSal = (elgAmount / salMonthDays) * noOfDays;
                                    taxableAmount = taxableAmount + partialSal;
                                }
                            }
                        }

                        // extenion tenure not stared
                        var query1 = (from o in context.tblOrder
                                      join ste in context.tblRCTSTE on o.AppointmentId equals ste.STEID
                                      where o.OrderType == 3 && o.Status == "Completed" &&
                                       o.FromDate > currDate && o.AppointmentType == 2 && ste.EmployeersID == model.EmployeeId &&
                                                      o.FromDate <= finEndDate && o.ToDate >= nextMonthStartDate
                                      select o).ToList();
                        foreach (var item in query1)
                        {
                            DateTime tDate = Convert.ToDateTime(item.ToDate);
                            decimal induBasic = item.Basic.GetValueOrDefault(0);
                            decimal induHRA = item.HRA.GetValueOrDefault(0);

                            if (finEndDate < tDate)
                            {
                                salaryEndDate = finEndDate;
                            }
                            else
                            {
                                salaryEndDate = tDate;
                            }

                            DateTime nexttenuredate = Convert.ToDateTime(item.FromDate);

                            if (Convert.ToDateTime(item.FromDate).Month == salaryStartDate.Month)
                            {
                                nexttenuredate = nextMonthStartDate;
                            }

                            var listOfMonthDays = GetMonthNumberOfDays(nexttenuredate, salaryEndDate);


                            //var listOfMonthDays = GetMonthNumberOfDays(nextMonthStartDate, salaryEndDate);



                            //int projectedMonth = listOfMonthDays.Count + 1;
                            //if (model.ProjectedNoMonths < projectedMonth)
                            // model.ProjectedNoMonths = projectedMonth;
                            foreach (var m in listOfMonthDays)
                            {
                                if (!futurMonth.Contains(m.MonthYear))
                                    futurMonth.Add(m.MonthYear);
                                int salMonthDays = m.TotalDays;
                                int noOfDays = m.TotalPresentDays;

                                if (salMonthDays == noOfDays)
                                {
                                    taxableAmount += induBasic + induHRA;
                                }
                                else
                                {
                                    decimal elgAmount = induBasic + induHRA;
                                    decimal partialSal = (elgAmount / salMonthDays) * noOfDays;
                                    taxableAmount = taxableAmount + partialSal;
                                }
                            }
                        }

                        int projectedMonth = futurMonth.Count + 1;
                        if (model.ProjectedNoMonths < projectedMonth)
                            model.ProjectedNoMonths = projectedMonth;
                    }
                    var queryOtherLine = context.tblRCTPayrollProcessDetail.Where(m => m.EmployeeId == model.EmployeeId && m.RCTPayrollProcessDetailId != model.PayrollProDetId && m.SalaryMonth == MonthYear && m.SalaryType == model.TypeOfPayBill && m.ProcessStatus == "Active").ToList();
                    var queryOtherLineTaxable = queryOtherLine.Where(m => m.TaxExempted != true).ToList();
                    decimal currMonthOtherDA = 0;
                    if (queryOtherLineTaxable.Count > 0)
                        foreach (var itemDA in queryOtherLineTaxable)
                        {
                            if (itemDA.Spl_Allowance > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.Spl_Allowance.GetValueOrDefault(0);
                            if (itemDA.Transport_Allowance > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.Transport_Allowance.GetValueOrDefault(0);
                            if (itemDA.PF_Revision > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.PF_Revision.GetValueOrDefault(0);
                            if (itemDA.ESIC_Revision > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.ESIC_Revision.GetValueOrDefault(0);
                            if (itemDA.Round_off > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.Round_off.GetValueOrDefault(0);
                            if (itemDA.Arrears > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.Arrears.GetValueOrDefault(0);
                            if (itemDA.OthersPay > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.OthersPay.GetValueOrDefault(0);
                            if (itemDA.HRA_Arrears > 0)
                                currMonthOtherDA = currMonthOtherDA + itemDA.HRA_Arrears.GetValueOrDefault(0);

                            if (itemDA.Loss_Of_Pay > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(110);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.Loss_Of_Pay.GetValueOrDefault(0);
                            }
                            if (itemDA.Contribution_to_PF > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(3);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.Contribution_to_PF.GetValueOrDefault(0);
                            }
                            if (itemDA.Recovery > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(4);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.Recovery.GetValueOrDefault(0);
                            }
                            if (itemDA.HRA_Recovery > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(204);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.HRA_Recovery.GetValueOrDefault(0);
                            }
                            if (itemDA.OthersDeduction > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(7);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.OthersDeduction.GetValueOrDefault(0);
                            }
                            if (itemDA.Professional_tax > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(108);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.Professional_tax.GetValueOrDefault(0);
                            }
                            if (itemDA.Medical_Recovery > 0)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(123);
                                if (isAffect)
                                    currMonthOtherDA = currMonthOtherDA - itemDA.Medical_Recovery.GetValueOrDefault(0);
                            }
                        }
                    decimal currMonthOtherBasicHraIT_e = (queryOtherLine.Where(m => m.TaxExempted == false).Sum(m => m.CurrentBasic) ?? 0) + (queryOtherLine.Where(m => m.TaxExempted == false).Sum(m => m.CurrentHRA) ?? 0);
                    decimal currBasicHra = model.Basic + model.HRA;
                    amount = taxableAmount;
                    taxableAmount += currBasicHra;
                    decimal taxable = 0;
                    decimal taxablOA = 0;
                    decimal deduct = 0;
                    decimal ttlOtherAllow = 0;
                    decimal ttlDirectAllow = 0;
                    decimal ttlAffectableProjExp = 0;
                    decimal preTaxableDA = 0, preDA = 0, prevBasic = 0, prevHRA = 0, prevTaxableBasic = 0, prevTaxableHRA = 0, preOA = 0, preTaxableOA = 0;
                    string paybill = model.EmployeeId;// Common.GetPayBill(model.EmployeeId);
                    if (model.TypeOfPayBill != 2)
                        model.OtherAllowance = GetEmpOtherAllowance(paybill);
                    if (model.OtherAllowance != null && model.OtherAllowance.Count > 0)
                    {
                        for (int i = 0; i < model.OtherAllowance.Count; i++)
                        {
                            ttlOtherAllow = ttlOtherAllow + model.OtherAllowance[i].Amount;
                            if (model.OtherAllowance[i].taxable == true)
                            {
                                taxablOA = taxablOA + model.OtherAllowance[i].Amount;
                            }
                        }
                    }
                    var modelBU = model.DirectAllowance;
                    if (modelBU != null && modelBU.buDetail != null)
                    {
                        deduct = modelBU.buDetail.Where(m => m.TypeId == 2).Sum(m => m.Amount) ?? 0;
                        foreach (var item in modelBU.buDetail)
                        {
                            decimal amt = item.Amount ?? 0;
                            if (item.IsTaxable && item.TypeId == 1)
                            {
                                taxablOA = taxablOA + amt;
                            }
                            if (item.TypeId == 1)
                            {
                                ttlDirectAllow = ttlDirectAllow + amt;
                            }
                            if (item.TypeId == 2)
                            {
                                bool isAffect = Common.GetCommonHeadFlag(Convert.ToInt32(item.HeadId));
                                if (isAffect)
                                    ttlAffectableProjExp = ttlAffectableProjExp + amt;
                            }
                        }
                    }

                    var prevSalQuery = (from sp in context.tblSalaryPayment
                                        join ph in context.tblSalaryPaymentHead on sp.PaymentHeadId equals ph.PaymentHeadId
                                        where sp.PayBill == paybill //&& sp.TaxExempted != true
                                        && finMonthYearList.Contains(ph.PaymentMonthYear) && ph.Status == "Approval Pending"
                                        && (ph.PaymentMonthYear != MonthYear
                                        ||
                                        (ph.PaymentMonthYear == MonthYear && ph.TypeOfPayBill != model.TypeOfPayBill))
                                        && (ph.PaymentMonthYear != finStartMonth || (ph.PaymentMonthYear == finStartMonth && ph.TypeOfPayBill != 2))
                                        select sp).ToList();
                    if (prevSalQuery.Count > 0)
                    {
                        var payIds = prevSalQuery.Select(m => m.PaymentId).ToArray();
                        prevBasic = prevSalQuery.Sum(m => m.Basic) ?? 0;
                        prevHRA = prevSalQuery.Sum(m => m.HRA) ?? 0;
                        prevTaxableBasic = prevSalQuery.Where(m => m.TaxExempted != true).Sum(m => m.Basic) ?? 0;
                        prevTaxableHRA = prevSalQuery.Where(m => m.TaxExempted != true).Sum(m => m.HRA) ?? 0;
                        //prevMA = prevSalQuery.Sum(m => m.MA) ?? 0;
                        preTaxableOA = (from soa in context.tblAdhocSalaryOtherAllowance
                                        join OA in context.tblEmpOtherAllowance on soa.EmpOtherAllowanceId equals OA.id
                                        join CC in context.tblCodeControl on OA.ComponentName equals CC.CodeValDetail
                                        join sp in context.tblSalaryPayment on soa.PaymentId equals sp.PaymentId
                                        where CC.CodeName == "OtherAllowance" && CC.CodeDescription == "True"
                                        && payIds.Contains(soa.PaymentId ?? 0) && sp.TaxExempted != true
                                        select soa.Amount).Sum() ?? 0;
                        preOA = (from soa in context.tblAdhocSalaryOtherAllowance
                                 join OA in context.tblEmpOtherAllowance on soa.EmpOtherAllowanceId equals OA.id
                                 join CC in context.tblCodeControl on OA.ComponentName equals CC.CodeValDetail
                                 //join sp in context.tblSalaryPayment on soa.PaymentId equals sp.PaymentId
                                 where CC.CodeName == "OtherAllowance" && payIds.Contains(soa.PaymentId ?? 0)
                                 select soa.Amount).Sum() ?? 0;
                        var preTaxableDAQry = (from da in context.tblAdhocSalaryBreakUpDetail
                                               join sp in context.tblSalaryPayment on da.PaymentId equals sp.PaymentId
                                               join cc in context.tblCommonHeads on da.HeadId equals cc.HeadId
                                               where payIds.Contains(da.PaymentId ?? 0)
                                               select new { da.Amount, cc.GroupId, cc.Common_f, sp.TaxExempted }).ToList();
                        if (preTaxableDAQry.Count > 0)
                        {
                            decimal DAllow = preTaxableDAQry.Where(m => m.GroupId == 1).Sum(m => m.Amount) ?? 0;
                            decimal DD = preTaxableDAQry.Where(m => m.GroupId == 2).Sum(m => m.Amount) ?? 0;
                            preDA = DAllow - DD;
                            decimal taxableDA = preTaxableDAQry.Where(m => m.GroupId == 1 && m.TaxExempted != true).Sum(m => m.Amount) ?? 0;
                            decimal taxableDD = preTaxableDAQry.Where(m => m.GroupId == 2 && m.TaxExempted != true && m.Common_f == true).Sum(m => m.Amount) ?? 0;
                            preTaxableDA = taxableDA - taxableDD;
                        }
                    }

                    model.CurrentMonthSalary = currentMonthSalay;
                    model.MonthSalary = monthSalary;
                    var data = GetPreviousPTandIT(paybill, model.PaymentMonthYear, model.TypeOfPayBill);
                    model.PreviousPT = data.Item1;
                    model.PreviousIT = data.Item2;
                    model.PreviousGross = data.Item3;
                    model.AnnualExemption = payment.GetITExemptionCurrentFinyear(paybill);
                    model.AnnualExemption = model.AnnualExemption; //+ model.PreviousPT;
                    if (model.TaxExempted == true)
                        model.AnnualTaxableSalary = prevTaxableBasic + currMonthOtherBasicHraIT_e + prevTaxableHRA + preTaxableOA + preTaxableDA;
                    else
                        model.AnnualTaxableSalary = taxableAmount + currMonthOtherBasicHraIT_e + taxablOA + prevTaxableBasic + prevTaxableHRA + preTaxableOA + preTaxableDA + currMonthOtherDA - ttlAffectableProjExp;
                    model.AnnualSalary = taxableAmount + currMonthOtherBasicHraIT_e + ttlOtherAllow + ttlDirectAllow + prevBasic + prevHRA + preOA + preDA + currMonthOtherDA - ttlAffectableProjExp;
                    taxable = model.AnnualTaxableSalary - model.AnnualExemption;
                    StaffPaymentService sps = new StaffPaymentService();
                    var otherIncome = sps.GetITEmpOtherIncomecurrentFinYear(paybill);
                    decimal otherInc = otherIncome != null ? otherIncome.Sum(m => m.Amount) : 0;
                    taxable = taxable + otherInc;
                    model.ProjectedSalary = amount;// + taxablOA;
                    model.TaxableIncome = taxable < 0 ? 0 : taxable;
                    model.OtherAllowanceAmount = ttlOtherAllow;
                    model.DirectAllowanceAmount = ttlDirectAllow;
                    model.AffectableProjExp = ttlAffectableProjExp;

                    //if (model.ITElegible_c == true)
                    //{
                    //    model = CalculateIT(model, verify);
                    //}

                    decimal taxmaxvalue = 700000;
                    if (model.ITElegible_c == true && model.TaxableIncome > taxmaxvalue)
                    {
                        model = CalculateIT(model, verify);
                    }

                    
                    decimal maxPTDeduction = Convert.ToDecimal(WebConfigurationManager.AppSettings["PT_Deduction_MaxLimit"]);
                    if (model.PTElegible_c == true && !verify && maxPTDeduction > model.PreviousPT)
                    {
                        decimal currMonthOtherBasicHraPT_e = (queryOtherLine.Where(m => m.PTExempted == false).Sum(m => m.CurrentBasic) ?? 0) + (queryOtherLine.Where(m => m.PTExempted == false).Sum(m => m.CurrentHRA) ?? 0);
                        decimal currElgPT = currBasicHra + currMonthOtherBasicHraPT_e + taxablOA - ttlAffectableProjExp;
                        model.ProfTax = CalculatePT(MonthYear, paybill, currElgPT, model.TypeOfPayBill);
                    }
                    model.Deduction = deduct + model.MA + model.ProfTax + model.MonthlyTax;
                    model.NetSalary = Math.Round(model.CurrentMonthSalary + ttlOtherAllow + ttlDirectAllow - model.Deduction, MidpointRounding.AwayFromZero);//+ otherAllowance;
                    model.GrossSalary = model.CurrentMonthSalary + ttlOtherAllow + ttlDirectAllow;
                    model.StaffCommitmentAmount = Math.Round(model.CurrentMonthSalary + ttlDirectAllow - ttlAffectableProjExp, MidpointRounding.AwayFromZero);
                    return model;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<EmployeeDepartmentModel> GetDepartments()
        {
            try
            {
                List<EmployeeDepartmentModel> model = new List<EmployeeDepartmentModel>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from E in context.vw_RCTAdhocEmployeeMaster
                                   select new
                                   {
                                       E.departmentcode,
                                       E.DEPARTMENT
                                   }).Distinct().ToList();
                    if (records != null)
                    {
                        foreach (var item in records)
                        {
                            model.Add(new EmployeeDepartmentModel
                            {
                                Code = item.departmentcode,
                                Department = item.DEPARTMENT
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<MasterlistviewModel> GetEmployeeCategory(string refType = "")
        {
            try
            {
                List<MasterlistviewModel> model = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    model = (from E in context.vwAdhocEmployeeCategory
                             where string.IsNullOrEmpty(refType) || E.SalaryRef == "NA" || E.SalaryRef == refType
                             select new MasterlistviewModel
                             {
                                 id = E.id,
                                 name = E.category
                             }).ToList();

                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private string GeneratePaymentNo()
        {
            try
            {
                string paymentNo = "";
                DateTime today = DateTime.Now;
                //int mon = DateTime.ParseExact(today.Month.ToString(), "MMM", CultureInfo.CurrentCulture).Month;

                //string year = Convert.ToString(today.Year);
                //string month = Convert.ToString(mon);
                //string date = Convert.ToString(today.Date);

                paymentNo = "AD-" + today.ToString("yyyyMMddHHmmssffff");

                return paymentNo;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string GetStatusFieldById(string CodeName, int statusCode)
        {
            try
            {
                string statusText = "";
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == CodeName && PT.CodeValAbbr == statusCode
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).SingleOrDefault();
                    if (payType != null)
                    {
                        statusText = payType.CodeValDetail;
                    }
                }
                return statusText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public decimal GetInstituteHospital(string EmployeeId)
        {
            decimal hos = 0;
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var records = (from S in context.vwPaymaster
                                   where S.FileNo == EmployeeId && S.paytype == 5
                                   select S).FirstOrDefault();
                    if (records != null)
                    {
                        hos = Convert.ToDecimal(records.Amount);
                    }
                }
                return hos;
            }
            catch (Exception ex)
            {
                return hos;
            }
        }
        //public Tuple<decimal, decimal, decimal> GetPreviousPTandIT(string EmployeeId, string paymentMonth, int typeOfPayBill)
        //{
        //    decimal pt = 0, it = 0, gross = 0;
        //    try
        //    {
        //        FinOp fo = new FinOp(paymentMonth, typeOfPayBill);
        //        List<string> finPeriod = fo.GetAllSalMonths();
        //        int paybill = Common.GetPayBill(EmployeeId);
        //        using (var context = new IOASDBEntities())
        //        {
        //            if (paybill != 0)
        //            {
        //                var records = (from S in context.tblSalaryPayment
        //                                   //join ph in context.tblSalaryPaymentHead on S.PaymentHeadId equals ph.PaymentHeadId
        //                               where S.PayBill == paybill && finPeriod.Contains(S.PaymentMonthYear)
        //                               group S by S.PayBill into g
        //                               select new
        //                               {
        //                                   it = g.Sum(x => x.MonthlyTax),
        //                                   pt = g.Sum(x => x.ProfTax),
        //                                   basic = g.Sum(x => x.Basic),
        //                                   hra = g.Sum(x => x.HRA),
        //                                   otherAllow = g.Sum(x => x.OtherAllowance),
        //                                   dirAllow = g.Sum(x => x.DirectAllowance),
        //                               }).FirstOrDefault();
        //                if (records != null)
        //                {
        //                    pt = records.pt ?? 0;
        //                    it = records.it ?? 0;
        //                    gross = Convert.ToDecimal(records.basic) + Convert.ToDecimal(records.hra) + Convert.ToDecimal(records.otherAllow) + Convert.ToDecimal(records.dirAllow);
        //                }
        //            }
        //        }
        //        return Tuple.Create(pt, it, gross);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Tuple.Create((Decimal)0, (Decimal)0, (Decimal)0);
        //    }
        //}
        public Tuple<decimal, decimal, decimal> GetPreviousPTandIT(string empId, string paymentMonth, int typeOfPayBill)
        {
            decimal pt = 0, it = 0, gross = 0;
            try
            {
                FinOp fo = new FinOp(paymentMonth, typeOfPayBill);
                List<string> finPeriod = fo.GetAllSalaryMonths();
                string finStartMonth = fo.SalStartMonth();
                using (var context = new IOASDBEntities())
                {
                    var records = (from S in context.tblSalaryPayment
                                   where S.PayBill == empId && finPeriod.Contains(S.PaymentMonthYear)
                                    && (S.PaymentMonthYear != paymentMonth ||
                                    (S.PaymentMonthYear == paymentMonth && S.TypeOfPayBill != typeOfPayBill))
                                   && (S.PaymentMonthYear != finStartMonth || (S.PaymentMonthYear == finStartMonth && S.TypeOfPayBill != 2))
                                   group S by S.PayBill into g
                                   select new
                                   {
                                       it = g.Sum(x => x.MonthlyTax),
                                       pt = g.Sum(x => x.ProfTax),
                                       basic = g.Sum(x => x.Basic),
                                       hra = g.Sum(x => x.HRA),
                                       otherAllow = g.Sum(x => x.OtherAllowance),
                                       dirAllow = g.Sum(x => x.DirectAllowance),
                                   }).FirstOrDefault();
                    if (records != null)
                    {
                        pt = records.pt ?? 0;
                        it = records.it ?? 0;
                        gross = Convert.ToDecimal(records.basic) + Convert.ToDecimal(records.hra) + Convert.ToDecimal(records.otherAllow) + Convert.ToDecimal(records.dirAllow);
                    }
                }
                return Tuple.Create(pt, it, gross);
            }
            catch (Exception ex)
            {
                return Tuple.Create((Decimal)0, (Decimal)0, (Decimal)0);
            }
        }
        public List<TaxSlab> GetTaxSlab(string gender, int age)
        {
            try
            {
                List<TaxSlab> model = new List<TaxSlab>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from TS in context.tblTaxSlab
                                   where TS.IsCurrent == true//TS.Gender == gender && TS.AgeFrom <= age && TS.AgeTo >= age
                                   && TS.TaxType == "IT"
                                   select TS).ToList();

                    if (records != null)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new TaxSlab
                            {
                                id = records[i].id,
                                RangeFrom = Convert.ToDecimal(records[i].RangeFrom),
                                RangeTo = Convert.ToDecimal(records[i].RangeTo),
                                Percentage = Convert.ToDecimal(records[i].Percentage),
                              //  Gender = Convert.ToString(records[i].Gender),
                             //   Age = Convert.ToInt32(records[i].Age),
                                IsCurrent = Convert.ToBoolean(records[i].IsCurrent)
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryPaymentHead GetSalayPaymentHead(int PaymentHeadId)
        {
            try
            {
                SalaryPaymentHead salaryHead = new SalaryPaymentHead();
                List<AdhocEmployeeModel> employees = new List<AdhocEmployeeModel>();
                SalaryModel salary = new SalaryModel();

                using (var context = new IOASDBEntities())
                {

                    if (PaymentHeadId > 0)
                    {
                        salaryHead = context.tblSalaryPaymentHead
                            .Where(x => x.PaymentHeadId == PaymentHeadId)
                            .Select(SP => new SalaryPaymentHead
                            {
                                PaymentHeadId = SP.PaymentHeadId,
                                PaymentMonthYear = SP.PaymentMonthYear,
                                PaidDate = (DateTime)SP.PaidDate,
                                //PaidAmount = (decimal)(SP.PaidAmount == null ? 0 : SP.Amount),
                                Status = SP.Status,
                                PaymentNo = (string)(SP.PaymentNo),
                                Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                                TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill)
                            }).SingleOrDefault();

                        //var records = (from SP in context.tblSalaryPayment
                        //               join AI in context.vw_RCTAdhocEmployeeMaster on SP.PayBillId equals AI.paybill_id
                        //               // To check the employees to date is greater than the end of the pay bill month for main salary.
                        //               where SP.PaymentHeadId == PaymentHeadId
                        //               orderby AI.paybill_id
                        //               select new
                        //               {
                        //                   EmployeeID = AI.FileNo,
                        //                   BasicSalary = AI.BasicPay,
                        //                   FromDate = AI.AppointmentDate,
                        //                   ToDate = AI.ExtensionDate,
                        //                   EmployeeName = AI.NAME,
                        //                   AI.paybill_id,
                        //                   AI.paybill_no,
                        //                   AI.NAME,
                        //                   AI.DOB,
                        //                   AI.Gender,
                        //                   AI.departmentcode,
                        //                   AI.DEPARTMENT,
                        //                   AI.AppointmentDate,
                        //                   AI.RelieveDate,
                        //                   AI.ExtensionDate,
                        //                   AI.BasicPay,
                        //                   AI.HRA,
                        //                   AI.Medical,
                        //                   ProjectNo = AI.PROJECTNO,
                        //                   CommitmentNo = AI.commitmentNo,
                        //                   PaymentId = (int?)SP.PaymentId,
                        //                   PaymentHeadId = (int?)SP.PaymentHeadId,
                        //                   IsPaid = (bool?)SP.IsPaid,
                        //                   SalaryDetails = SP
                        //               }).Take(5).ToList();

                        //if (records != null)
                        //{
                        //    for (int i = 0; i < records.Count; i++)
                        //    {
                        //        salary.Basic = Convert.ToDecimal(records[i].SalaryDetails.Basic);
                        //        salary.AnnualSalary = Convert.ToDecimal(records[i].SalaryDetails.AnnualSalary);
                        //        salary.MonthSalary = Convert.ToDecimal(records[i].SalaryDetails.MonthSalary);
                        //        salary.Tax = Convert.ToDecimal(records[i].SalaryDetails.Tax);
                        //        salary.TaxableIncome = Convert.ToDecimal(records[i].SalaryDetails.TaxableIncome);
                        //        salary.MonthlyTax = Convert.ToDecimal(records[i].SalaryDetails.MonthlyTax);
                        //        salary.NetSalary = Convert.ToDecimal(records[i].SalaryDetails.NetSalary);
                        //        salary.ModeOfPayment = Convert.ToInt32(records[i].SalaryDetails.ModeOfPayment);
                        //        salary.CurrentMonthSalary = Convert.ToInt32(records[i].SalaryDetails.MonthSalary);
                        //        employees.Add(new AdhocEmployeeModel
                        //        {
                        //            MakePayment = true,
                        //            EmployeeID = records[i].EmployeeID,
                        //            EmployeeName = records[i].EmployeeName,
                        //            AppointmentDate = Convert.ToDateTime(records[i].AppointmentDate),
                        //            ToDate = Convert.ToDateTime(records[i].ToDate),
                        //            RelieveDate = Convert.ToDateTime(records[i].RelieveDate),
                        //            BasicPay = Convert.ToDecimal(salary.Basic),
                        //            PROJECTNO = records[i].ProjectNo,
                        //            FromDate = Convert.ToDateTime(records[i].FromDate),
                        //            commitmentNo = records[i].CommitmentNo,
                        //            SalaryDetail = salary,
                        //            ModeOfPayment = (int)records[i].SalaryDetails.ModeOfPayment
                        //        });
                        //    }
                        //}
                    }
                    salaryHead.AdhocEmployees = employees;
                }

                return salaryHead;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                SalaryPaymentHead salaryHead = new SalaryPaymentHead();
                List<AdhocEmployeeModel> employees = new List<AdhocEmployeeModel>();
                salaryHead.AdhocEmployees = employees;
                return salaryHead;
            }
        }


        public AdhocEmployeeModel GetMainSalaryEmployeeDetails(string empId, int payrollProDetId, string PaymentMonthYear, int typeOfPay, bool isNotInMonth, AgencyVerifyEmployeeModel modelBU = null, decimal PT = 0, decimal IT = 0, decimal MA = 0, decimal IH = 0, bool verify = false, decimal annualTax = 0, decimal currbasic = 0, decimal currhra = 0, bool IsPension = false)
        {
            try
            {
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var record = new AdhocEmployeeDetailModel();
                    if (isNotInMonth)
                        record = (from AI in context.vw_RCTAdhocEmployeeMaster
                                  where AI.EmployeeId == empId
                                  //orderby AI.paybill_id
                                  select new AdhocEmployeeDetailModel
                                  {
                                      FileNo = AI.EmployeeId,
                                      ExtensionDate = AI.ExtensionDate,
                                      NAME = AI.NAME,
                                      //paybill_id = AI.paybill_id,
                                      //paybill_no = AI.paybill_no,
                                      DOB = AI.DOB,
                                      Gender = AI.Gender,
                                      departmentcode = AI.departmentcode,
                                      DEPARTMENT = AI.DEPARTMENT,
                                      AppointmentDate = AI.AppointmentDate,
                                      RelieveDate = AI.RelieveDate,
                                      BasicPay = AI.Basic,
                                      HRA = AI.HRA,
                                      Medical = AI.Medical,
                                      PROJECTNO = AI.PROJECTNO,
                                      commitmentNo = AI.commitmentNo,
                                      DesignationCode = AI.DesignationCode,
                                      InclusiveMedical = AI.InclusiveMedical,
                                      currentHra = 0,
                                      currentMedical = 0,
                                      currentPay = 0,
                                      TaxExempted = AI.TaxExempted,
                                      PTExempted = AI.PTExempted
                                  }).FirstOrDefault();
                    else
                        record = (from AI in context.tblRCTPayrollProcessDetail
                                  where AI.RCTPayrollProcessDetailId == payrollProDetId
                                  select new AdhocEmployeeDetailModel
                                  {
                                      PayrollProDetId = AI.RCTPayrollProcessDetailId,
                                      FileNo = AI.EmployeeId,
                                      ExtensionDate = AI.AppointmentEndDate,
                                      NAME = AI.CandidateName,
                                      DOB = AI.DOB,
                                      Gender = AI.Gender,
                                      departmentcode = AI.DepartmentCode,
                                      DEPARTMENT = AI.DepartmentName,
                                      AppointmentDate = AI.AppointmentStartDate,
                                      RelieveDate = AI.RelieveDate,
                                      BasicPay = AI.Basic,
                                      HRA = AI.HRA,
                                      Medical = AI.Medical,
                                      PROJECTNO = AI.ProjectNumber,
                                      commitmentNo = AI.CommitmentNumber,
                                      DesignationCode = AI.DesignationCode,
                                      InclusiveMedical = AI.MedicalInclusive_f,
                                      currentHra = AI.CurrentHRA,
                                      currentMedical = AI.CurrentMedical,
                                      currentPay = AI.CurrentBasic,
                                      TaxExempted = AI.TaxExempted,
                                      PTExempted = AI.PTExempted,
                                      Spl_Allowance = AI.Spl_Allowance,
                                      Transport_Allowance = AI.Transport_Allowance,
                                      OthersDeduction = AI.OthersDeduction,
                                      PF_Revision = AI.PF_Revision,
                                      ESIC_Revision = AI.ESIC_Revision,
                                      Round_off = AI.Round_off,
                                      Arrears = AI.Arrears,
                                      OthersPay = AI.OthersPay,
                                      Contribution_to_PF = AI.Contribution_to_PF,
                                      Recovery = AI.Recovery,
                                      Professional_tax = AI.Professional_tax,
                                      Loss_Of_Pay = AI.Loss_Of_Pay,
                                      Medical_Recovery = AI.Medical_Recovery,
                                      NoOfDaysPresent = AI.TotalDays,
                                      HRA_Arrears = AI.HRA_Arrears,
                                      HRA_Recovery = AI.HRA_Recovery
                                  }).FirstOrDefault();

                    if (record != null)
                    {
                        SalaryModel salaryModel = new SalaryModel();
                        DateTime frmDate = new DateTime();
                        DateTime tDate = new DateTime();
                        //if (record.Gender != null && record.Gender.ToLower() == "m")
                        //{
                        salaryModel.Gender = record.Gender;
                        //}
                        //else if (record.Gender != null && record.Gender.ToLower() == "f")
                        //{
                        //    salaryModel.Gender = "Female";
                        //}
                        DateTime lastDate = Common.GetMonthLastDate(PaymentMonthYear);
                        DateTime startDate = Common.GetMonthFirstDate(PaymentMonthYear);
                        if (record.RelieveDate != null)
                        {
                            salaryModel.ToDate = Convert.ToDateTime(record.RelieveDate);
                            tDate = Convert.ToDateTime(record.RelieveDate);
                        }
                        else
                        {
                            salaryModel.ToDate = Convert.ToDateTime(record.ExtensionDate);
                            if (record.ExtensionDate <= lastDate && record.ExtensionDate >= startDate)
                                tDate = Convert.ToDateTime(record.ExtensionDate);
                            else
                                tDate = lastDate;
                        }
                        if (record.AppointmentDate <= lastDate && record.AppointmentDate >= startDate)
                            frmDate = Convert.ToDateTime(record.AppointmentDate);
                        else
                            frmDate = startDate;
                        decimal basic = Convert.ToDecimal(record.BasicPay);
                        decimal ma = Convert.ToDecimal(record.Medical);
                        salaryModel.Basic = verify ? currbasic : Convert.ToDecimal(record.currentPay);
                        salaryModel.HRA = verify ? currhra : Convert.ToDecimal(record.currentHra);
                        salaryModel.MA = verify ? MA : Convert.ToDecimal(record.currentMedical);
                        salaryModel.ActualBasic = basic; // record.InclusiveMedical == true ? basic - ma : basic;
                        salaryModel.ActualHRA = Convert.ToDecimal(record.HRA);
                        salaryModel.ActualMA = ma;
                        //decimal instHos = salaryModel.MA > 0 ? (salaryModel.MA / 175) * 30 : 0;
                        //salaryModel.InstituteHospital = verify ? IH : instHos;
                        salaryModel.DA = 0;
                        salaryModel.PayrollProDetId = record.PayrollProDetId;
                        salaryModel.Conveyance = 0;
                        salaryModel.ProfTax = PT;
                        salaryModel.MonthlyTax = IT;
                        salaryModel.Tax = annualTax;
                        salaryModel.EmployeeId = record.FileNo;
                        salaryModel.TypeOfPayBill = 1;
                        salaryModel.NoOfDaysPresent = record.NoOfDaysPresent.GetValueOrDefault(0);
                        salaryModel.FromDate = Convert.ToDateTime(record.AppointmentDate);
                        if (record.DOB == null)
                            return null;
                        salaryModel.DateOfBirth = Convert.ToDateTime(record.DOB);
                        salaryModel.DOB = String.Format("{0:dd-MMM-yyyy}", record.DOB);
                        salaryModel.PaymentMonthYear = PaymentMonthYear;
                        salaryModel.TaxExempted = record.TaxExempted ?? false;
                        salaryModel.PTExempted = record.PTExempted ?? false;
                        salaryModel.TypeOfPayBill = typeOfPay;
                        if (isNotInMonth)
                        {
                            salaryModel.ITElegible_c = salaryModel.TaxExempted == true ? false : true;
                            salaryModel.PTElegible_c = salaryModel.PTExempted == true ? false : true;
                        }
                        else
                        {
                            if (salaryModel.TaxExempted == false)
                            {
                                var maxOne = context.tblRCTPayrollProcessDetail.Where(m => m.EmployeeId == salaryModel.EmployeeId && m.SalaryMonth == PaymentMonthYear && m.SalaryType == salaryModel.TypeOfPayBill && m.TaxExempted == false && m.ProcessStatus == "Active").OrderByDescending(m => m.CurrentBasic).FirstOrDefault();
                                if (maxOne.RCTPayrollProcessDetailId != salaryModel.PayrollProDetId)
                                    salaryModel.ITElegible_c = false;
                            }
                            else
                                salaryModel.ITElegible_c = false;
                            if (salaryModel.PTExempted == false)
                            {
                                var maxOne = context.tblRCTPayrollProcessDetail.Where(m => m.EmployeeId == salaryModel.EmployeeId && m.SalaryMonth == PaymentMonthYear && m.SalaryType == salaryModel.TypeOfPayBill && m.TaxExempted == false && m.ProcessStatus == "Active").OrderByDescending(m => m.CurrentBasic).FirstOrDefault();
                                if (maxOne.RCTPayrollProcessDetailId != salaryModel.PayrollProDetId)
                                    salaryModel.PTElegible_c = false;
                            }
                            else
                                salaryModel.PTElegible_c = false;
                        }
                        if (!verify)
                        {
                            AgencyVerifyEmployeeModel mBU = new AgencyVerifyEmployeeModel();
                            List<SalaryBreakUpDetailsModel> list = new List<SalaryBreakUpDetailsModel>();
                            if (record.Spl_Allowance > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Spl_Allowance,
                                    HeadId = 1,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.Transport_Allowance > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Transport_Allowance,
                                    HeadId = 2,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.PF_Revision > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.PF_Revision,
                                    HeadId = 5,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.ESIC_Revision > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.ESIC_Revision,
                                    HeadId = 6,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.Round_off > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Round_off,
                                    HeadId = 8,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.Arrears > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Arrears,
                                    HeadId = 9,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.OthersPay > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.OthersPay,
                                    HeadId = 10,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.HRA_Arrears > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.HRA_Arrears,
                                    HeadId = 203,
                                    HeadList = Common.GetCommonHeadList(1, 1),
                                    IsTaxable = true,
                                    TypeId = 1
                                });
                            if (record.Loss_Of_Pay > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Loss_Of_Pay,
                                    HeadId = 110,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.Contribution_to_PF > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Contribution_to_PF,
                                    HeadId = 3,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.Recovery > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Recovery,
                                    HeadId = 4,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.HRA_Recovery > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.HRA_Recovery,
                                    HeadId = 204,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.OthersDeduction > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.OthersDeduction,
                                    HeadId = 7,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.Professional_tax > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Professional_tax,
                                    HeadId = 108,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = true,
                                    TypeId = 2
                                });
                            if (record.Medical_Recovery > 0)
                                list.Add(new SalaryBreakUpDetailsModel()
                                {
                                    Amount = record.Medical_Recovery,
                                    HeadId = 123,
                                    HeadList = Common.GetCommonHeadList(1, 2),
                                    IsAffectProjectExp = true,
                                    IsTaxable = false,
                                    TypeId = 2
                                });
                            if (list.Count > 0)
                            {
                                mBU.buDetail = list;
                                salaryModel.DirectAllowance = mBU;
                            }
                        }
                        else
                        {
                            salaryModel.DirectAllowance = modelBU;
                        }
                        var salary = GetSalaryDetail_(salaryModel, verify);
                        salary.ProjectNo = record.PROJECTNO;
                        salary.PaymentMonthYear = PaymentMonthYear;
                        salary.taxSlab = null;


                        model.MakePayment = true;
                        model.EmployeeID = record.FileNo;
                        model.EmployeeName = record.NAME;
                        model.DesignationCode = record.DesignationCode;
                        model.AppointmentDate = Convert.ToDateTime(record.AppointmentDate);
                        model.ToDate = tDate;
                        model.RelieveDate = Convert.ToDateTime(record.RelieveDate);
                        model.PROJECTNO = record.PROJECTNO;
                        model.FromDate = frmDate;
                        model.commitmentNo = record.commitmentNo;
                        model.SalaryDetail = salary;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedData<SalaryPaymentHead> ListSalayPayment(int page, int pageSize, SalaryPaymentHead srchModel)
        {
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
                var searchData = new PagedData<SalaryPaymentHead>();
                List<SalaryPaymentHead> salaryHead = new List<SalaryPaymentHead>();

                decimal totalSalary = 0;

                using (var context = new IOASDBEntities())
                {
                    var query = (from SP in context.tblSalaryPaymentHead
                                 join CC in context.tblCodeControl on SP.TypeOfPayBill equals CC.CodeValAbbr
                                 where CC.CodeName == "PayOfBill"
                                 && (String.IsNullOrEmpty(srchModel.TypeOfPayBillText) || CC.CodeValDetail.Contains(srchModel.TypeOfPayBillText))
                                 && (String.IsNullOrEmpty(srchModel.PaymentNo) || SP.PaymentNo.Contains(srchModel.PaymentNo))
                                 && (String.IsNullOrEmpty(srchModel.PaymentMonthYear) || SP.PaymentMonthYear.Contains(srchModel.PaymentMonthYear))
                                 && SP.Pensioner_f != true
                                 orderby SP.PaymentHeadId descending
                                 select new
                                 {
                                     PaymentHeadId = SP.PaymentHeadId,
                                     PaymentNo = (string)(SP.PaymentNo),
                                     Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                                     TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill),
                                     TypeOfPayBillText = CC.CodeValDetail,
                                     PaidDate = (DateTime)SP.PaidDate,
                                     Status = SP.Status,
                                     PaymentMonthYear = SP.PaymentMonthYear,
                                     ProjectNo = SP.ProjectNo
                                 });
                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();
                    if (recordCount > 0)
                    {
                        for (var k = 0; k < records.Count; k++)
                        {
                            salaryHead.Add(new SalaryPaymentHead
                            {
                                SlNo = skiprec + k + 1,
                                PaymentHeadId = records[k].PaymentHeadId,
                                PaymentNo = records[k].PaymentNo,
                                Amount = records[k].Amount,
                                TypeOfPayBill = records[k].TypeOfPayBill,
                                PaidDateText = String.Format("{0:dd-MMM-yyyy}", records[k].PaidDate),
                                PaymentMonthYear = records[k].PaymentMonthYear,
                                ProjectNo = records[k].ProjectNo,
                                Status = records[k].Status,
                                TypeOfPayBillText = records[k].TypeOfPayBillText
                            });
                        }
                    }
                    searchData.Data = salaryHead;
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));

                }


                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryTransaction GetSalaryTransaction(int PaymentHeadId, string PaymentMonthYear)
        {
            try
            {
                CoreAccountsService coreAccountService = new CoreAccountsService();
                SalaryTransaction model = new SalaryTransaction();
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;

                using (var context = new IOASDBEntities())
                {
                    var record = (from ST in context.tblSalaryTransaction
                                  where ST.PaymentHeadId == PaymentHeadId
                                  select ST).SingleOrDefault();
                    if (record != null)
                    {
                        model.TotalCredit = Convert.ToDecimal(record.TotalCredit);
                        model.TotalDebit = Convert.ToDecimal(record.TotalDebit);

                        model.PaymentHeadId = record.PaymentHeadId;
                        model.TransactionId = record.TransactionId;
                        model.PaymentNo = record.PaymentNo;
                        model.PostedDate = Convert.ToDateTime(record.PostedDate);
                        model.ApprovedDate = Convert.ToDateTime(record.ApprovedDate);
                        model.SalaryType = Convert.ToInt32(record.SalaryType);
                        model.TotalAmount = Convert.ToDecimal(record.TotalAmount);
                        model.TotalTaxAmount = Convert.ToDecimal(record.TotalTaxAmount);
                        model.CommitmentAmount = Convert.ToDecimal(record.CommitmentAmount);
                        model.ExpenseAmount = Convert.ToDecimal(record.ExpenseAmount);
                        model.DeductionAmount = Convert.ToDecimal(record.DeductionAmount);

                        model.Status = record.Status;
                        model.PaymentType = Convert.ToInt32(record.PaymentType);
                    }

                    var records = (from STD in context.tblSalaryTransactionDetail
                                   where STD.PaymentHeadId == PaymentHeadId
                                   select STD).ToList();
                    if (records.Count > 0)
                    {
                        List<SalaryTransactionDetail> detail = new List<SalaryTransactionDetail>();
                        List<BillExpenseDetailModel> expDetail = new List<BillExpenseDetailModel>();
                        for (var i = 0; i < records.Count; i++)
                        {
                            detail.Add(new SalaryTransactionDetail
                            {
                                TransactionDetailId = records[i].TransactionDetailId,
                                TransactionId = records[i].TransactionId,
                                PaymentHeadId = records[i].PaymentHeadId,
                                AccountGroupId = Convert.ToInt32(records[i].AccountGroupId),
                                AccountHeadId = Convert.ToInt32(records[i].AccountHeadId),
                                PaymentNo = records[i].PaymentNo,
                                PostedDate = Convert.ToDateTime(records[i].PostedDate),
                                SalaryType = Convert.ToInt32(records[i].SalaryType),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                TransactionType = Convert.ToString(records[i].TransactionType),
                                Status = record.Status,
                                PaymentType = Convert.ToInt32(records[i].PaymentType)
                            });
                            //model.ExpenseDetail[i].Amount = records[i].Amount;
                            expDetail.Add(new BillExpenseDetailModel
                            {
                                BillExpenseDetailId = records[i].TransactionDetailId,
                                AccountGroupId = Convert.ToInt32(records[i].AccountGroupId),
                                AccountHeadId = Convert.ToInt32(records[i].AccountHeadId),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                TransactionType = Convert.ToString(records[i].TransactionType),
                                AccountGroupList = Common.GetAccountGroup(records[i].AccountGroupId ?? 0),
                                AccountHeadList = Common.GetAccountHeadList(records[i].AccountGroupId ?? 0, records[i].AccountHeadId ?? 0, "1", "SAL")
                            });
                        }
                        model.ExpenseDetail = expDetail;
                        model.detail = detail;
                    }

                    var paymentHead = context.tblSalaryPaymentHead
                        .Where(x => x.PaymentHeadId == PaymentHeadId)
                        .Select(SP => new SalaryPaymentHead
                        {
                            PaymentHeadId = SP.PaymentHeadId,
                            PaymentNo = (string)(SP.PaymentNo),
                            Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                            TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill)
                            //PaidDate = (DateTime)SP.PaidDate,
                            //PaymentMonthYear = SP.PaymentMonthYear,
                            //ProjectNo = SP.ProjectNo
                        }).SingleOrDefault();

                    if (paymentHead != null)
                    {
                        model.PaymentHeadId = paymentHead.PaymentHeadId;
                        model.PaymentNo = paymentHead.PaymentNo;
                        model.Amount = context.tblSalaryPayment.Where(m => m.PaymentHeadId == model.PaymentHeadId).Sum(m => m.GrossTotal) ?? 0; //paymentHead.Amount;
                        model.TypeOfPayBill = paymentHead.TypeOfPayBill;
                    }
                }


                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public string SalaryTransactionIU(SalaryTransaction HeaderModel, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    tblSalaryPayment salary = new tblSalaryPayment();
                    tblSalaryTransaction trans = new tblSalaryTransaction();
                    tblSalaryTransactionDetail detail = new tblSalaryTransactionDetail();
                    var transactionId = 0;
                    var salaryTransaction = context.tblSalaryTransaction
                        .SingleOrDefault(it => it.PaymentHeadId == HeaderModel.PaymentHeadId);
                    if (salaryTransaction != null && salaryTransaction.PaymentHeadId > 0)
                    {

                        salaryTransaction.PaymentHeadId = HeaderModel.PaymentHeadId;
                        salaryTransaction.PaymentNo = HeaderModel.PaymentNo;
                        //salaryTransaction.TransactionTypeCode = HeaderModel.TransactionTypeCode;
                        //salaryTransaction.PostedDate = Convert.ToDateTime(HeaderModel.PaidDate);
                        //salaryTransaction.ApprovedDate = HeaderModel.;
                        salaryTransaction.TotalAmount = HeaderModel.Amount;
                        salaryTransaction.TotalCredit = HeaderModel.TotalCredit;
                        salaryTransaction.TotalDebit = HeaderModel.TotalDebit;
                        //salaryTransaction.TotalTaxAmount = HeaderModel.Amount;
                        salaryTransaction.UpdatedAt = DateTime.Now;
                        salaryTransaction.UpdatedBy = userId;
                        salaryTransaction.Status = "open";
                        context.SaveChanges();
                        transactionId = salaryTransaction.TransactionId;
                    }
                    else
                    {
                        trans.PaymentHeadId = HeaderModel.PaymentHeadId;
                        //trans.PaymentNo = HeaderModel.PaymentNo;
                        //trans.TransactionTypeCode = HeaderModel.TransactionTypeCode;
                        //trans.PostedDate = Convert.ToDateTime(HeaderModel.PaidDate);
                        //trans.ApprovedDate = HeaderModel.;
                        trans.TotalAmount = HeaderModel.Amount;
                        trans.TotalCredit = HeaderModel.TotalCredit;
                        trans.TotalDebit = HeaderModel.TotalDebit;
                        //trans.TotalTaxAmount = HeaderModel.Amount;
                        trans.UpdatedAt = DateTime.Now;
                        trans.CreatedBy = userId;
                        trans.Status = "open";
                        context.tblSalaryTransaction.Add(trans);
                        context.SaveChanges();
                        transactionId = trans.TransactionId;
                    }

                    var details = HeaderModel.ExpenseDetail;
                    context.tblSalaryTransactionDetail.RemoveRange(context.tblSalaryTransactionDetail.Where(m => m.PaymentHeadId == HeaderModel.PaymentHeadId));
                    context.SaveChanges();
                    for (int i = 0; i < details.Count; i++)
                    {
                        detail.PaymentHeadId = HeaderModel.PaymentHeadId;
                        detail.AccountGroupId = details[i].AccountGroupId;
                        detail.AccountHeadId = details[i].AccountHeadId;
                        detail.Amount = details[i].Amount;
                        detail.TransactionId = transactionId;
                        detail.TransactionType = details[i].TransactionType;
                        context.tblSalaryTransactionDetail.Add(detail);
                        context.SaveChanges();
                    }

                    context.Dispose();

                }
                string msg = "Salary details saved successfully";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public int EmployeeSalaryIU(SalaryPaymentHead HeaderModel, string EmpNo, int payrollProDetId, int ModeOfPayment, int userId, decimal basic, decimal hra, decimal tax, decimal PT, decimal MA, decimal IH, decimal annualTax, AgencyVerifyEmployeeModel buModel, string commitmentNo, string remarks, bool isNotInMain = false)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        List<SalaryModel> SalaryList = new List<SalaryModel>();
                        SalaryList = HeaderModel.Salary;
                        tblSalaryPaymentHead salaryHead = new tblSalaryPaymentHead();
                        commitmentNo = commitmentNo.Trim();
                        if (HeaderModel.PaymentHeadId > 0)
                        {
                            var paymentHead = context.tblSalaryPaymentHead
                                .SingleOrDefault(it => it.PaymentHeadId == HeaderModel.PaymentHeadId);
                            //paymentHead.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                            //paymentHead.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                            if (paymentHead.FinYear == null)
                            {
                                var fo = new FinOp(HeaderModel.PaymentMonthYear, HeaderModel.TypeOfPayBill);
                                string finPeriod = fo.GetFinPeriod(HeaderModel.PaymentMonthYear);
                                paymentHead.FinYear = Common.GetFinancialYearId(finPeriod);
                            }
                            paymentHead.ProjectNo = HeaderModel.ProjectNo;
                            paymentHead.CommitmentNo = HeaderModel.CommitmentNo;
                            paymentHead.Amount = HeaderModel.Amount;
                            paymentHead.PaidAmount = HeaderModel.PaidAmount;
                            paymentHead.UpdatedAt = DateTime.Now;
                            paymentHead.UpdatedBy = userId;
                            //paymentHead.Status = HeaderModel.Status;
                            context.SaveChanges();
                            HeaderModel.PaymentNo = paymentHead.PaymentNo;
                        }
                        else
                        {
                            //var queryExists = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentMonthYear == HeaderModel.PaymentMonthYear && m.TypeOfPayBill == HeaderModel.TypeOfPayBill);
                            //if (queryExists != null)
                            return -1;

                            //var paymentNo = GeneratePaymentNo();
                            //salaryHead.PaymentNo = paymentNo;
                            //salaryHead.CommitmentNo = HeaderModel.CommitmentNo;
                            //salaryHead.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                            //salaryHead.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                            //salaryHead.ProjectNo = HeaderModel.ProjectNo;
                            //salaryHead.Amount = HeaderModel.Amount;
                            //salaryHead.PaidAmount = HeaderModel.PaidAmount;
                            //salaryHead.PaidDate = DateTime.Now;
                            //salaryHead.CreatedAt = DateTime.Now;
                            //salaryHead.Pensioner_f = HeaderModel.Pensioner_f;
                            //salaryHead.CreatedBy = userId;
                            //salaryHead.Status = "Open";
                            //context.tblSalaryPaymentHead.Add(salaryHead);
                            //context.SaveChanges();
                            //HeaderModel.PaymentHeadId = salaryHead.PaymentHeadId;
                            //HeaderModel.PaymentNo = salaryHead.PaymentNo;
                        }
                        SalaryModel model = new SalaryModel();
                        var paybill = GetStatusFieldById("PayOfBill", HeaderModel.TypeOfPayBill);
                        var employee = new AdhocEmployeeModel();

                        //if (HeaderModel.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
                        //{
                        //    employee = GetMainSalaryEmployeeDetails(EmpNo, payrollProDetId, HeaderModel.PaymentMonthYear, 2, isNotInMain, buModel, PT, tax, MA, IH, true, annualTax, basic, hra);
                        //}
                        //else
                        //{
                        employee = GetMainSalaryEmployeeDetails(EmpNo, payrollProDetId, HeaderModel.PaymentMonthYear, HeaderModel.TypeOfPayBill, isNotInMain, buModel, PT, tax, MA, IH, true, annualTax, basic, hra);
                        //}
                        model = employee.SalaryDetail;

                        tblSalaryPayment salary = new tblSalaryPayment();
                        var queryCmt = new BillCommitmentDetailModel();
                        queryCmt = (from c in context.tblCommitment
                                    join p in context.tblProject on c.ProjectId equals p.ProjectId
                                    join det in context.tblCommitmentDetails on c.CommitmentId equals det.CommitmentId
                                    where c.CommitmentNumber == commitmentNo
                                    && c.Status == "Active"
                                    select new BillCommitmentDetailModel { ProjectNumber = p.ProjectNumber, AvailableAmount = det.BalanceAmount, CommitmentId = c.CommitmentId, CommitmentDetailId = det.ComitmentDetailId }).FirstOrDefault();

                        if (model.ProjectNo != queryCmt.ProjectNumber)
                        {
                            return -5;
                        }
                        if (queryCmt != null && model.StaffCommitmentAmount <= queryCmt.AvailableAmount && model.StaffCommitmentAmount >= 0)
                        {
                            salary.EmpNo = model.EmployeeId;
                            salary.EmployeeId = model.EmployeeId;
                            salary.ProjectNo = model.ProjectNo;
                            salary.Basic = model.Basic;
                            salary.HRA = model.HRA;
                            salary.MA = 0;
                            salary.MedicalRecovery = model.MA;
                            salary.DA = model.DA;
                            salary.Conveyance = model.Conveyance;
                            salary.Deduction = model.Deduction;
                            salary.Tax = model.Tax;
                            salary.ProfTax = model.ProfTax;
                            salary.TaxableIncome = model.TaxableIncome;
                            salary.AnnualSalary = model.AnnualSalary;
                            salary.AnnualTaxableSalary = model.AnnualTaxableSalary;
                            salary.AnnualExemption = model.AnnualExemption;
                            salary.NetSalary = model.NetSalary;
                            salary.GrossTotal = model.GrossSalary;
                            salary.MonthSalary = model.CurrentMonthSalary;
                            salary.CurrentMonthSalary = model.CurrentMonthSalary;
                            salary.InstituteHospital = model.InstituteHospital;
                            salary.OtherAllowance = model.OtherAllowanceAmount;
                            salary.DirectAllowance = model.DirectAllowanceAmount;
                            salary.MonthlyTax = model.MonthlyTax;
                            salary.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                            salary.ModeOfPayment = ModeOfPayment;
                            salary.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                            //salary.PayBillId = model.paybill_id;
                            //salary.PayBillNo = model.paybill_no;
                            salary.PayBill = model.EmployeeId;
                            salary.PaidDate = System.DateTime.Now;
                            salary.CreatedAt = System.DateTime.Now;
                            salary.CreatedBy = userId;
                            salary.Status = "Open";
                            salary.TaxExempted = model.TaxExempted;
                            salary.PTExempted = model.PTExempted;
                            salary.Remarks = remarks;
                            salary.PaymentHeadId = HeaderModel.PaymentHeadId;
                            salary.RCTPayrollProcessDetailId = model.PayrollProDetId;
                            context.tblSalaryPayment.Add(salary);
                            context.SaveChanges();
                            model.PaymentId = salary.PaymentId;
                            //}
                            //if (model.InstituteHospital > 0)
                            //{
                            //    int maHead = 0;
                            //    int projectId = Common.GetProjectId(model.ProjectNo);
                            //    decimal insHos = (model.MA / 175) * 30;
                            //    string contMsg = Common.ValidateCommitment(projectId, 3, insHos);
                            //    if (contMsg != "Valid")
                            //    {
                            //        string consMsg = Common.ValidateCommitment(projectId, 2, insHos);
                            //        if (consMsg != "Valid")
                            //        {
                            //            string otherMsg = Common.ValidateCommitment(projectId, 8, insHos);
                            //            if (otherMsg != "Valid")
                            //            {
                            //                //string staffMsg = Common.ValidateCommitment(projectId, 1, insHos);
                            //                //if (staffMsg != "Valid")
                            //                return -4;
                            //                //else
                            //                //    maHead = 1;
                            //            }
                            //            else
                            //                maHead = 8;
                            //        }
                            //        else
                            //            maHead = 2;
                            //    }
                            //    else
                            //        maHead = 3;
                            //    CommitmentModel cModel = new CommitmentModel();
                            //    cModel.selAllocationHead = maHead;
                            //    cModel.selCommitmentType = 1;
                            //    cModel.selFundingBody = 0;
                            //    cModel.SelProjectNumber = projectId;
                            //    cModel.selPurpose = 1;
                            //    cModel.selRequestRefrence = 2;
                            //    cModel.commitmentValue = model.InstituteHospital;
                            //    cModel.EmployeeId = model.EmployeeId;
                            //    cModel.Remarks = model.PaymentMonthYear;
                            //    var cRespo = coreAccountService.CreateCommitDetails(cModel, userId);
                            //    if (cRespo.Status != "Success")
                            //        return -4;
                            //    tblAdhocSalaryCommitmentDetail asc = new tblAdhocSalaryCommitmentDetail();
                            //    asc.Amount = model.InstituteHospital;
                            //    asc.CommitmentDetailId = cRespo.commitmentDetialId;
                            //    asc.CRTD_By = userId;
                            //    asc.CRTD_TS = DateTime.Now;
                            //    asc.PaymentHeadId = HeaderModel.PaymentHeadId;
                            //    asc.PaymentId = model.PaymentId;
                            //    asc.Contingency_f = true;
                            //    asc.Status = "Active";
                            //    context.tblAdhocSalaryCommitmentDetail.Add(asc);
                            //    context.SaveChanges();
                            //}

                            foreach (var item in buModel.buDetail)
                            {
                                tblAdhocSalaryBreakUpDetail BU = new tblAdhocSalaryBreakUpDetail();
                                BU.Amount = item.Amount;
                                BU.CategoryId = item.TypeId;
                                BU.HeadId = item.HeadId;
                                BU.Remarks = item.Remarks;
                                BU.IsTaxable = item.IsTaxable;
                                BU.PaymentId = model.PaymentId;
                                BU.IsAffectProjectExp = item.TypeId == 2 ? Common.GetCommonHeadFlag(item.HeadId ?? 0) : false;
                                context.tblAdhocSalaryBreakUpDetail.Add(BU);
                                context.SaveChanges();
                            }
                            if (model.StaffCommitmentAmount > 0)
                            {
                                tblAdhocSalaryCommitmentDetail com = new tblAdhocSalaryCommitmentDetail();
                                com.Amount = model.StaffCommitmentAmount;
                                com.CommitmentDetailId = queryCmt.CommitmentDetailId;
                                com.CRTD_By = userId;
                                com.CRTD_TS = DateTime.Now;
                                com.PaymentHeadId = HeaderModel.PaymentHeadId;
                                com.PaymentId = model.PaymentId;
                                com.Status = "Active";
                                context.tblAdhocSalaryCommitmentDetail.Add(com);
                                context.SaveChanges();
                            }
                            if (model.OtherAllowance != null && model.OtherAllowance.Count > 0)
                            {
                                foreach (var item in model.OtherAllowance)
                                {

                                    var otherPayment = context.tblEmpOtherAllowance.FirstOrDefault(m => m.id == item.id);

                                    if (otherPayment != null)
                                    {
                                        otherPayment.PaymentHeadId = HeaderModel.PaymentHeadId;
                                        otherPayment.PaymentNo = HeaderModel.PaymentNo;
                                        otherPayment.IsPaid = true;
                                        context.SaveChanges();
                                    }

                                    tblAdhocSalaryOtherAllowance OA = new tblAdhocSalaryOtherAllowance();
                                    OA.Amount = item.Amount;
                                    OA.PaymentHeadId = HeaderModel.PaymentHeadId;
                                    OA.EmployeeID = item.EmployeeId;
                                    OA.PaymentId = model.PaymentId;
                                    OA.EmpOtherAllowanceId = item.id;
                                    context.tblAdhocSalaryOtherAllowance.Add(OA);

                                }
                            }
                        }
                        else if (queryCmt == null)
                            return -2;
                        else if (queryCmt != null && model.StaffCommitmentAmount > queryCmt.AvailableAmount)
                            return -3;

                        var totalAmount = context.tblSalaryPayment.Where(m => m.PaymentHeadId == HeaderModel.PaymentHeadId).Sum(m => m.NetSalary) ?? 0;
                        var payhead = context.tblSalaryPaymentHead.SingleOrDefault(x => x.PaymentHeadId == HeaderModel.PaymentHeadId);
                        if (payhead != null)
                        {
                            payhead.Amount = totalAmount;
                            context.SaveChanges();
                        }


                        transaction.Commit();
                        return HeaderModel.PaymentHeadId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.ToString());
                        return 0;
                    }
                }
            }
        }
        public AdhocOtherAllowDetailModel GetAdhocOtherExpBreakUp(int PaymentHeadId)
        {
            try
            {
                AdhocOtherAllowDetailModel data = new AdhocOtherAllowDetailModel();
                using (var context = new IOASDBEntities())
                {
                    var otherQuery = (from other in context.tblAdhocSalaryOtherAllowance
                                      join det in context.tblEmpOtherAllowance on other.EmpOtherAllowanceId equals det.id
                                      where other.PaymentHeadId == PaymentHeadId
                                      select new { other.Amount, det.ComponentName }).ToList();
                    if (otherQuery.Count > 0)
                    {
                        data.TotalDistributionAmount = otherQuery.Where(m => m.ComponentName == "Distribution").Sum(m => m.Amount) ?? 0;
                        data.TotalHonororiumAmount = otherQuery.Where(m => m.ComponentName == "Honororium").Sum(m => m.Amount) ?? 0;
                        data.TotalMandaysAmount = otherQuery.Where(m => m.ComponentName == "Mandays").Sum(m => m.Amount) ?? 0;
                        data.TotalFellowshipAmount = otherQuery.Where(m => m.ComponentName == "FellowshipSalary").Sum(m => m.Amount) ?? 0;
                    }
                    else
                    {
                        data.TotalDistributionAmount = 0;
                        data.TotalFellowshipAmount = 0;
                        data.TotalHonororiumAmount = 0;
                        data.TotalMandaysAmount = 0;
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                return new AdhocOtherAllowDetailModel();
            }
        }



        public int updateverify(int PayrollProDetId)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var record = context.tbl_sal_edit_opt.Where(x => x.PayrollProDetId == PayrollProDetId).SingleOrDefault();
                    if (record != null)
                    {
                        record.Verified_status = true;
                        context.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }



        }
        public int RemoveVerifiedEmployee(int PaymentHeadId, string EmployeeId, int userId, bool verify, int? paymentId)
        {
            try
            {
                int code = 0;
                using (var context = new IOASDBEntities())
                {
                    //var record = context.tblSalaryPayment.Where(x => x.PaymentHeadId == PaymentHeadId && x.EmployeeId == EmployeeId).SingleOrDefault();
                    var record = context.tblSalaryPayment.Where(x => x.PaymentId == paymentId).SingleOrDefault();
                    if (record != null)
                    {
                        context.tblSalaryPayment.Remove(record);
                        context.tblAdhocSalaryBreakUpDetail.RemoveRange(context.tblAdhocSalaryBreakUpDetail.Where(m => m.PaymentId == paymentId));
                        var contQuery = context.tblAdhocSalaryCommitmentDetail.Where(m => m.Contingency_f == true).FirstOrDefault();
                        if (contQuery != null)
                            coreAccountService.CloseCommitment(contQuery.CommitmentDetailId ?? 0, record.EmployeeId + " PaymentId - " + record.PaymentId.ToString());
                        context.tblAdhocSalaryCommitmentDetail.RemoveRange(context.tblAdhocSalaryCommitmentDetail.Where(m => m.PaymentId == paymentId));
                        context.SaveChanges();

                        var arryEmpId = context.tblAdhocSalaryOtherAllowance.Where(m => m.PaymentId == paymentId)
                                  .Select(m => m.EmpOtherAllowanceId)
                                  .ToArray();
                        if (arryEmpId.Count() > 0)
                        {
                            context.tblEmpOtherAllowance.Where(m => arryEmpId.Contains(m.id))
                                 .ToList()
                                  .ForEach(m =>
                                  {
                                      m.IsPaid = false;
                                      m.PaymentHeadId = null;
                                      m.PaymentNo = null;
                                  });
                            context.tblAdhocSalaryOtherAllowance.RemoveRange(context.tblAdhocSalaryOtherAllowance.Where(c => c.PaymentId == paymentId));
                        }

                        var removeid = context.tbl_sal_edit_opt.Where(x => x.PayrollProDetId == record.RCTPayrollProcessDetailId).SingleOrDefault();
                        if (removeid != null)
                        {
                            removeid.Verified_status = false;
                            context.SaveChanges();
                        }

                        //var otherPayment = (from OA in context.tblEmpOtherAllowance
                        //                    where OA.EmployeeIdStr == EmployeeId &&
                        //                    OA.PaymentHeadId == PaymentHeadId
                        //                    select OA).ToList();

                        //if (otherPayment != null)
                        //{
                        //    for (int j = 0; j < otherPayment.Count; j++)
                        //    {
                        //        otherPayment[j].PaymentHeadId = null;
                        //        otherPayment[j].PaymentNo = null;
                        //        context.SaveChanges();
                        //    }
                        //}
                        var totalAmount = context.tblSalaryPayment.Where(m => m.PaymentHeadId == PaymentHeadId).Sum(m => m.NetSalary) ?? 0;
                        var payhead = context.tblSalaryPaymentHead.SingleOrDefault(x => x.PaymentHeadId == PaymentHeadId);
                        if (payhead != null)
                        {
                            payhead.Amount = totalAmount;
                            payhead.UpdatedBy = userId;
                            payhead.UpdatedAt = DateTime.Now;
                            context.SaveChanges();
                        }
                    }

                    context.Dispose();
                    code = 1;
                }

                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
        public List<AccountHeadViewModel> GetAccountHead(string input)
        {
            List<AccountHeadViewModel> head = new List<AccountHeadViewModel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    head = (from AH in context.tblAccountHead
                            where AH.AccountHeadCode == input
                            select new AccountHeadViewModel
                            {
                                AccountHeadId = AH.AccountHeadId,
                                AccountHeadCode = AH.AccountHeadCode,
                                AccountHead = AH.AccountHead
                            }).ToList();

                }
                return head;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return head;
            }
        }
        public List<Accountgroupmodel> GetAccountGroup(string input)
        {
            List<Accountgroupmodel> group = new List<Accountgroupmodel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    group = (from AG in context.tblAccountGroup
                             where AG.AccountGroupCode == input
                             select new Accountgroupmodel
                             {
                                 AccountGroupId = AG.AccountGroupId,
                                 AccountGroupCode = AG.AccountGroupCode,
                                 AccountGroup = AG.AccountGroup
                             }).ToList();

                }
                return group;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return group;
            }
        }
        public string UpdateSalaryPayment(int PaymentHeadId, string currentStatus, string newStatus, int userId)        {            try            {                string msg = "";                using (var context = new IOASDBEntities())                {                    using (var transaction = context.Database.BeginTransaction())                    {                        List<BillCommitmentDetailModel> txList = new List<BillCommitmentDetailModel>();                        bool result = false;                        try                        {                            var payNo = "";                            var pyamentHead = (from PH in context.tblSalaryPaymentHead                                               where PH.PaymentHeadId == PaymentHeadId && PH.Status == currentStatus                                               select PH).SingleOrDefault();                            if (pyamentHead != null)                            {                                pyamentHead.Status = newStatus;                                pyamentHead.UpdatedBy = userId;                                pyamentHead.UpdatedAt = DateTime.Now;                                context.SaveChanges();                                payNo = pyamentHead.PaymentNo;
                                //var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                                //int processGuideLineId = Process.ProcessGuidelineId;
                                //var fe = FlowEngine.Init(processGuideLineId, userId, PaymentHeadId, "PaymentHeadId");
                                //fe.ProcessInit();
                            }                            if (newStatus == "Approval Pending")                            {
                                //var query = (from c in context.tblAdhocSalaryCommitmentDetail
                                // group c by c.CommitmentDetailId into grp
                                // join det in context.tblCommitmentDetails on grp.FirstOrDefault().CommitmentDetailId equals det.ComitmentDetailId
                                // join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                // where grp.FirstOrDefault().PaymentHeadId == PaymentHeadId && grp.FirstOrDefault().Status == "Active"
                                // select new
                                // {
                                // detailId = grp.Key,
                                // commitmentId = com.CommitmentId,
                                // amount = grp.Sum(m => m.Amount)
                                // }).ToList();
                                //if (query.Count > 0)
                                //{
                                // for (int i = 0; i < query.Count; i++)
                                // {
                                // txList.Add(new BillCommitmentDetailModel()
                                // {
                                // CommitmentDetailId = query[i].detailId,
                                // PaymentAmount = query[i].amount,
                                // CommitmentId = query[i].commitmentId,
                                // ReversedAmount = query[i].amount
                                // });
                                // }
                                //}
                                txList = (from c in context.tblAdhocSalaryCommitmentDetail                                          join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId                                          join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId                                          where c.PaymentHeadId == PaymentHeadId && c.Status == "Active"                                          select new BillCommitmentDetailModel()                                          {                                              CommitmentDetailId = c.CommitmentDetailId,                                              PaymentAmount = c.Amount,                                              CommitmentId = com.CommitmentId,                                              ReversedAmount = c.Amount                                          }).ToList();                                result = coreAccountService.UpdateCommitmentBalance(txList, false, false, userId, PaymentHeadId, "SAL");                                if (!result)                                {                                    transaction.Rollback();                                    return "There is a mismatch between the allocated available value and allocated commitment value.";                                }                                BOAModel model = new BOAModel();                                CoreAccountsService coreAccounts = new CoreAccountsService();                                List<BOATransactionModel> txDet = new List<BOATransactionModel>();                                model.TempVoucherNumber = pyamentHead.PaymentNo;                                model.PostedDate = DateTime.Now;                                model.VoucherType = 3;                                model.VoucherNumber = pyamentHead.PaymentNo;                                model.BOAValue = pyamentHead.Amount;                                model.RefNumber = pyamentHead.PaymentNo;                                model.RefTransactionCode = "SAL";                                model.TransactionTypeCode = "SAL";                                txDet = (from exp in context.tblSalaryTransactionDetail                                         where exp.PaymentHeadId == PaymentHeadId                                         select new BOATransactionModel()                                         {                                             AccountHeadId = exp.AccountHeadId,                                             Amount = exp.Amount,                                             TransactionType = exp.TransactionType                                         }).ToList();                                var bankHeadDet = (from exp in context.tblSalaryTransactionDetail                                                   join h in context.tblAccountHead on exp.AccountHeadId equals h.AccountHeadId                                                   where exp.PaymentHeadId == PaymentHeadId && h.Bank_f == true && exp.Amount > 0                                                   select exp).FirstOrDefault();

                                //var paymentQuery = (from sp in context.tblSalaryPayment
                                // join b in context.vwAdhocBankDetails on sp.EmployeeId equals b.Fileno into g
                                // from b in g.DefaultIfEmpty()
                                // where sp.PaymentHeadId == PaymentHeadId
                                // select new
                                // {
                                // sp.NetSalary,
                                // sp.CreatedAt,
                                // sp.ModeOfPayment,
                                // b,
                                // sp.EmployeeId,
                                // sp.EmpNo,
                                // sp.ProjectNo
                                // }).ToList();
                                //int count = paymentQuery.Count();
                                //if (count > 0)
                                //{
                                List<BOAPaymentDetailModel> BOAPaymentDetail = new List<BOAPaymentDetailModel>();
                                // for (int i = 0; i < count; i++)
                                // {
                                // var data = Common.GetProjectType(paymentQuery[i].ProjectNo);
                                // int bankHd = 146;
                                // if (data.Item3 == 4)
                                // bankHd = 146;
                                // else if (data.Item3 == 2)
                                // bankHd = 151;
                                // else if (data.Item3 == 3)
                                // bankHd = 152;
                                // else if (data.Item1 == 2)
                                // bankHd = 149;
                                // else if (data.Item1 == 1 && data.Item2 == 1)
                                // bankHd = 148;
                                // else if (data.Item1 == 1)
                                // bankHd = 20;
                                // BOAPaymentDetail.Add(new BOAPaymentDetailModel()
                                // {
                                // TransactionType = "Credit",
                                // BankHeadID = bankHd,
                                // Amount = paymentQuery[i].NetSalary,
                                // ReferenceNumber = payNo,
                                // ReferenceDate = paymentQuery[i].CreatedAt,
                                // PaymentMode = paymentQuery[i].ModeOfPayment,
                                // PayeeBank = paymentQuery[i].b != null ? paymentQuery[i].b.BankName : "",
                                // StudentRoll = paymentQuery[i].EmployeeId,
                                // Reconciliation_f = false,
                                // PayeeName = paymentQuery[i].EmpNo,
                                // PayeeType = "Adhoc Salary"
                                // });
                                // }
                                // model.BOAPaymentDetail = BOAPaymentDetail;
                                //}
                                BOAPaymentDetail.Add(new BOAPaymentDetailModel()                                {                                    TransactionType = "Credit",                                    BankHeadID = bankHeadDet.AccountHeadId,                                    Amount = bankHeadDet.Amount,                                    ReferenceNumber = payNo,                                    ReferenceDate = DateTime.Now,                                    PaymentMode = 2,                                    PayeeBank = "",                                    StudentRoll = "",                                    Reconciliation_f = false,                                    PayeeName = "Adhoc Salary " + pyamentHead.PaymentMonthYear,                                    PayeeType = "Adhoc Salary"                                });                                model.BOAPaymentDetail = BOAPaymentDetail;                                model.BOATransaction = txDet;                                bool boaTx = coreAccounts.BOATransaction(model);                                if (!boaTx)                                    coreAccountService.UpdateCommitmentBalance(txList, true, false, userId, PaymentHeadId, "SAL");                                if (!result || !boaTx)                                {                                    transaction.Rollback();                                    return msg;                                }                                context.tblCommitmentLog.Where(x => x.TransactionTypeCode == "SAL" && x.RefId == PaymentHeadId)                               .ToList()                               .ForEach(m =>                               {                                   m.CRTD_TS = model.PostedDate;                                   m.Posted_f = true;                               });                                context.SaveChanges();                            }

                            //context.Dispose();
                            msg = "Updated successfully";                            transaction.Commit();                        }                        catch (Exception ex)                        {                            IOASException.Instance.HandleMe(this, ex);                            if (result)                                coreAccountService.UpdateCommitmentBalance(txList, true, false, userId, PaymentHeadId, "SAL");                            transaction.Rollback();                            return ex.ToString();                        }                    }                }                return msg;            }            catch (Exception ex)            {                Console.WriteLine(ex.ToString());
                //transaction.Rollback();
                return ex.ToString();            }        }

        public AdhocEmployeeModel GetEmployeeByEmpId(int EmployeeId)
        {
            try
            {
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var record = (from AI in context.vwCombineStaffDetails
                                  where AI.ID == EmployeeId
                                  orderby AI.ID
                                  select new
                                  {
                                      AI.EmployeeId,
                                      AI.Name,
                                      AI.EmployeeDesignation,
                                      AI.DepartmentName,
                                      AI.ID
                                  }).FirstOrDefault();
                    if (record != null)
                    {
                        model.EmployeeID = Convert.ToString(record.ID);
                        model.EmployeeName = record.EmployeeId + "-" + record.ID + "-" + record.Name;
                        model.DesignationCode = record.DepartmentName;
                        model.Designation = record.EmployeeDesignation;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public PagedList<AdhocEmployeeModel> GetEmployeesByPaymentHead(int PaymentHeadId, string ModeOfPaymentName, int page, int pageSize)
        {
            try
            {
                int skiprec = 0;

                if (page > 1)
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedList<AdhocEmployeeModel>();

                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                AdhocEmployeeModel salary = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from SP in context.tblSalaryPayment
                                 from emp in context.vw_RCTAdhocEmployeeMaster.Where(m => m.EmployeeId == SP.EmployeeId).OrderByDescending(m => m.AppointmentDate).Take(1)
                                 join CC in context.tblCodeControl on SP.ModeOfPayment equals CC.CodeValAbbr
                                 where SP.PaymentHeadId == PaymentHeadId && CC.CodeName == "SalaryPaymentType"
                                 && (String.IsNullOrEmpty(ModeOfPaymentName) || CC.CodeValDetail.Contains(ModeOfPaymentName))
                                 && (String.IsNullOrEmpty(EmployeeName) || emp.NAME.Contains(EmployeeName))
                                 && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
                                 && (String.IsNullOrEmpty(DepartmentCode) || emp.departmentcode.Contains(DepartmentCode) || emp.DEPARTMENT.Contains(DepartmentCode))
                                 // && (EmployeeCategory == 0 || emp.employeeCategory == EmployeeCategory)
                                 orderby emp.NAME
                                 select new
                                 {
                                     emp.EmployeeId,
                                     CandidateName = emp.NAME,
                                     CommitmentNumber = SP.CommitmentNo,
                                     ProjectNumber = SP.ProjectNo,
                                     DepartmentCode = emp.departmentcode,
                                     DepartmentName = emp.DEPARTMENT,
                                     SP.NetSalary,
                                     SP.Basic,
                                     CC.CodeValDetail,
                                     SP.GrossTotal,
                                     SP.PaymentId,
                                     emp.DesignationCode
                                 }).ToList();
                    var recordCount = query.Count();
                    int sno = 0;
                    if (page == 1)
                        sno = 1;
                    else
                        sno = skiprec + 1;
                    query = query.Skip(skiprec).Take(pageSize).ToList();
                    list = query
                    .AsEnumerable()
                    .Select((x, index) => new AdhocEmployeeModel()
                    {
                        SlNo = sno + index,
                        MakePayment = true,
                        EmployeeID = x.EmployeeId,
                        EmployeeName = x.CandidateName,
                        commitmentNo = x.CommitmentNumber,
                        PROJECTNO = x.ProjectNumber,
                        departmentcode = x.DepartmentCode,
                        DEPARTMENT = x.DepartmentName,
                        NetSalary = x.NetSalary,
                        BasicPay = Convert.ToDecimal(x.Basic),
                        ModeOfPaymentName = x.CodeValDetail,
                        GrossTotal = x.GrossTotal,
                        PaymentId = x.PaymentId,
                        DesignationCode = x.DesignationCode
                    }).ToList();

                    //var recordCount = query.Count();
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    searchData.Data = list;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public PagedList<AdhocEmployeeModel> GetSalaryEmployees(string PaymentMonthYear, int PaymentHeadId, int typeOfPay, int page, int pageSize)
        {
            try
            {
                int skiprec = 0;

                if (page > 1)
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedList<AdhocEmployeeModel>();

                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                AdhocEmployeeModel salary = new AdhocEmployeeModel();
                DateTime lastDate = Common.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = Common.GetMonthFirstDate(PaymentMonthYear);
                using (var context = new IOASDBEntities())
                {

                    var count_query = context.tbl_sal_edit_opt.Where(t => t.typeofpay == typeOfPay).Count();
                    if (count_query == 0)
                    {
                        var exe_sp = context.Database.ExecuteSqlCommand("sp_sla_edit_opt @p0,@p1", typeOfPay, PaymentMonthYear);
                    }


                    var query = (from emp in context.tbl_sal_edit_opt
                                 where
                                    (String.IsNullOrEmpty(EmployeeName) || emp.CandidateName.Contains(EmployeeName))
                                      && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
                                      && (String.IsNullOrEmpty(DepartmentCode) || emp.DepartmentCode.Contains(DepartmentCode) || emp.DepartmentName.Contains(DepartmentCode))
                                      && emp.Verified_status == false && emp.typeofpay == typeOfPay
                                 orderby emp.CandidateName
                                 select new
                                 {
                                     emp.EmployeeId,
                                     emp.CandidateName,
                                     emp.CommitmentNumber,
                                     emp.ProjectNumber,
                                     emp.DepartmentCode,
                                     emp.DepartmentName,
                                     emp.Basic,
                                     emp.DesignationCode,
                                     emp.IsNotInMain,
                                     emp.PayrollProDetId
                                 });


                    //var query = (from emp in context.tblRCTPayrollProcessDetail
                    //             where (String.IsNullOrEmpty(EmployeeName) || emp.CandidateName.Contains(EmployeeName))
                    //           && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
                    //           && (String.IsNullOrEmpty(DepartmentCode) || emp.DepartmentCode.Contains(DepartmentCode) || emp.DepartmentName.Contains(DepartmentCode))
                    //           && !context.tblSalaryPayment.Any(m => m.RCTPayrollProcessDetailId == emp.RCTPayrollProcessDetailId)
                    //           && emp.SalaryMonth == PaymentMonthYear && emp.SalaryType == typeOfPay && emp.ProcessStatus == "Active"
                    //             orderby emp.CandidateName
                    //             select new
                    //             {
                    //                 emp.EmployeeId,
                    //                 emp.CandidateName,
                    //                 emp.CommitmentNumber,
                    //                 emp.ProjectNumber,
                    //                 emp.DepartmentCode,
                    //                 emp.DepartmentName,
                    //                 emp.Basic,
                    //                 emp.DesignationCode,
                    //                 IsNotInMain = false,
                    //                 PayrollProDetId = emp.RCTPayrollProcessDetailId
                    //             }).Concat(from emp in context.vw_RCTAdhocEmployeeMaster
                    //                       where context.tblEmpOtherAllowance.Any(m => m.EmployeeIdStr == emp.EmployeeId && (m.IsPaid == null || m.IsPaid == false)) && typeOfPay != 2
                    //                       && ((emp.IITMPensioner_f == true && typeOfPay == 0) || (emp.IITMPensioner_f == false && typeOfPay != 0))
                    //            && (String.IsNullOrEmpty(EmployeeName) || emp.NAME.Contains(EmployeeName))
                    //            && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
                    //            && (String.IsNullOrEmpty(DepartmentCode) || emp.departmentcode.Contains(DepartmentCode) || emp.DEPARTMENT.Contains(DepartmentCode))
                    //            && !context.tblRCTPayrollProcessDetail.Any(m => m.SalaryMonth == PaymentMonthYear && m.SalaryType == typeOfPay && m.ProcessStatus == "Active" && m.EmployeeId == emp.EmployeeId)
                    //                   && !context.tblSalaryPayment.Any(m => m.PaymentMonthYear == PaymentMonthYear && m.TypeOfPayBill == typeOfPay && m.EmployeeId == emp.EmployeeId)
                    //                       orderby emp.NAME
                    //                       select new
                    //                       {
                    //                           emp.EmployeeId,
                    //                           CandidateName = emp.NAME,
                    //                           CommitmentNumber = emp.commitmentNo,
                    //                           ProjectNumber = emp.PROJECTNO,
                    //                           DepartmentCode = emp.departmentcode,
                    //                           DepartmentName = emp.DEPARTMENT,
                    //                           emp.Basic,
                    //                           emp.DesignationCode,
                    //                           IsNotInMain = true,
                    //                           PayrollProDetId = 0
                    //                       });
                    var querylist = query.ToList();
                    var recordCount = querylist.Count();
                    querylist = querylist.Skip(skiprec).Take(pageSize).ToList();

                    int sno = 0;
                    if (page == 1)
                        sno = 1;
                    else
                        sno = skiprec + 1;
                    list = querylist
                                 .AsEnumerable()
                                 .Select((x, index) => new AdhocEmployeeModel()
                                 {
                                     SlNo = sno + index,
                                     MakePayment = true,
                                     EmployeeID = x.EmployeeId,
                                     EmployeeName = x.CandidateName,
                                     commitmentNo = x.CommitmentNumber,
                                     PROJECTNO = x.ProjectNumber,
                                     departmentcode = x.DepartmentCode,
                                     DEPARTMENT = x.DepartmentName,
                                     IsNotInMain = x.IsNotInMain,
                                     DesignationCode = x.DesignationCode,
                                     BasicPay = Convert.ToDecimal(x.Basic),
                                     PayrollProDetId = x.PayrollProDetId
                                 }).ToList();

                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    searchData.Data = list;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //public PagedList<AdhocEmployeeModel> GetSalaryEmployees(string PaymentMonthYear, int PaymentHeadId, int typeOfPay, int page, int pageSize)
        //{
        //    try
        //    {
        //        int skiprec = 0;

        //        if (page > 1)
        //        {
        //            skiprec = (page - 1) * pageSize;
        //        }
        //        var searchData = new PagedList<AdhocEmployeeModel>();

        //        List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
        //        AdhocEmployeeModel salary = new AdhocEmployeeModel();
        //        DateTime lastDate = Common.GetMonthLastDate(PaymentMonthYear);
        //        DateTime startDate = Common.GetMonthFirstDate(PaymentMonthYear);
        //        using (var context = new IOASDBEntities())
        //        {
        //            var query = (from emp in context.tblRCTPayrollProcessDetail
        //                         where (String.IsNullOrEmpty(EmployeeName) || emp.CandidateName.Contains(EmployeeName))
        //                       && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
        //                       && (String.IsNullOrEmpty(DepartmentCode) || emp.DepartmentCode.Contains(DepartmentCode) || emp.DepartmentName.Contains(DepartmentCode))
        //                       && !context.tblSalaryPayment.Any(m => m.RCTPayrollProcessDetailId == emp.RCTPayrollProcessDetailId)
        //                       && emp.SalaryMonth == PaymentMonthYear && emp.SalaryType == typeOfPay && emp.ProcessStatus == "Active"
        //                         orderby emp.CandidateName
        //                         select new
        //                         {
        //                             emp.EmployeeId,
        //                             emp.CandidateName,
        //                             emp.CommitmentNumber,
        //                             emp.ProjectNumber,
        //                             emp.DepartmentCode,
        //                             emp.DepartmentName,
        //                             emp.Basic,
        //                             emp.DesignationCode,
        //                             IsNotInMain = false,
        //                             PayrollProDetId = emp.RCTPayrollProcessDetailId
        //                         }).Concat(from emp in context.vw_RCTAdhocEmployeeMaster
        //                                   where context.tblEmpOtherAllowance.Any(m => m.EmployeeIdStr == emp.EmployeeId && (m.IsPaid == null || m.IsPaid == false)) && typeOfPay != 2
        //                                   && ((emp.IITMPensioner_f == true && typeOfPay == 0) || (emp.IITMPensioner_f == false && typeOfPay != 0))
        //                        && (String.IsNullOrEmpty(EmployeeName) || emp.NAME.Contains(EmployeeName))
        //                        && (String.IsNullOrEmpty(EmployeeNo) || emp.EmployeeId.Contains(EmployeeNo))
        //                        && (String.IsNullOrEmpty(DepartmentCode) || emp.departmentcode.Contains(DepartmentCode) || emp.DEPARTMENT.Contains(DepartmentCode))
        //                        && !context.tblRCTPayrollProcessDetail.Any(m => m.SalaryMonth == PaymentMonthYear && m.SalaryType == typeOfPay && m.ProcessStatus == "Active" && m.EmployeeId == emp.EmployeeId)
        //                               && !context.tblSalaryPayment.Any(m => m.PaymentMonthYear == PaymentMonthYear && m.TypeOfPayBill == typeOfPay && m.EmployeeId == emp.EmployeeId)
        //                                   orderby emp.NAME
        //                                   select new
        //                                   {
        //                                       emp.EmployeeId,
        //                                       CandidateName = emp.NAME,
        //                                       CommitmentNumber = emp.commitmentNo,
        //                                       ProjectNumber = emp.PROJECTNO,
        //                                       DepartmentCode = emp.departmentcode,
        //                                       DepartmentName = emp.DEPARTMENT,
        //                                       emp.Basic,
        //                                       emp.DesignationCode,
        //                                       IsNotInMain = true,
        //                                       PayrollProDetId = 0
        //                                   });

        //            list = query
        //                         .AsEnumerable()
        //                         .Select((x, index) => new AdhocEmployeeModel()
        //                         {
        //                             SlNo = index + 1,
        //                             MakePayment = true,
        //                             EmployeeID = x.EmployeeId,
        //                             EmployeeName = x.CandidateName,
        //                             commitmentNo = x.CommitmentNumber,
        //                             PROJECTNO = x.ProjectNumber,
        //                             departmentcode = x.DepartmentCode,
        //                             DEPARTMENT = x.DepartmentName,
        //                             IsNotInMain = x.IsNotInMain,
        //                             DesignationCode = x.DesignationCode,
        //                             BasicPay = Convert.ToDecimal(x.Basic),
        //                             PayrollProDetId = x.PayrollProDetId
        //                         }).Skip(skiprec).Take(pageSize).ToList();

        //            var recordCount = query.Count();
        //            searchData.TotalRecords = recordCount;
        //            searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
        //            searchData.Data = list;
        //        }
        //        return searchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        return null;
        //    }
        //}


        public List<EmpOtherAllowance> GetEmpOtherAllowance(string EmployeeId)
        {
            try
            {
                List<EmpOtherAllowance> model = new List<EmpOtherAllowance>();
                using (var context = new IOASDBEntities())
                {
                    //int paybill = Common.GetPayBill(EmployeeId);
                    var records = (from OA in context.tblEmpOtherAllowance
                                   join CC in context.tblCodeControl on OA.ComponentName equals CC.CodeValDetail
                                   where OA.EmployeeIdStr == EmployeeId && OA.IsPaid == false && CC.CodeName == "OtherAllowance"
                                   select new
                                   {
                                       EmployeeId = OA.EmployeeIdStr,
                                       ComponentName = OA.ComponentName,
                                       Amount = OA.Amount,
                                       deduction = OA.deduction,
                                       Status = OA.Status,
                                       IsPaid = OA.IsPaid,
                                       taxable = CC.CodeDescription,
                                       id = OA.id
                                   }).ToList();
                    if (records != null)
                    {
                        foreach (var item in records)
                        {
                            model.Add(new EmpOtherAllowance
                            {
                                EmployeeId = item.EmployeeId,
                                ComponentName = item.ComponentName,
                                Amount = Convert.ToDecimal(item.Amount),
                                deduction = Convert.ToBoolean(item.deduction),
                                Status = item.Status,
                                id = item.id,
                                IsPaid = Convert.ToBoolean(item.IsPaid),
                                taxable = item.taxable.ToLower() == "true" ? true : false
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<AutoCompleteModel> GetACAdhocEmpList(string term, string typeOfPayBill, string PaymentMonthYear)
        {
            try
            {
                List<AutoCompleteModel> det = new List<AutoCompleteModel>();
                DateTime lastDate = Common.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = Common.GetMonthFirstDate(PaymentMonthYear);
                int payTypeId = Common.GetCodeControlAbbrId("PayOfBill", typeOfPayBill);
                using (var context = new IOASDBEntities())
                {
                    var query = (from emp in context.vw_RCTAdhocEmployeeMaster
                                 where (emp.EmployeeId.Contains(term) || emp.NAME.Contains(term))
                                 orderby emp.NAME
                                 select new
                                 {
                                     EmployeeID = emp.EmployeeId,
                                     EmployeeName = emp.NAME
                                 }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            det.Add(new AutoCompleteModel()
                            {
                                value = query[i].EmployeeID,
                                label = query[i].EmployeeID + "-" + query[i].EmployeeName
                            });
                        }
                    }
                }

                return det;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public DataSet GetEmployeesSalaryList(string PaymentMonthYear, int PaymentHeadId, int typeOfPay)
        {
            DataSet ds = new DataSet();
            try
            {
                DataTable dtUnVerify = new DataTable("UnVerified");
                DataTable dtVerify = new DataTable("Verified");
                DateTime lastDate = Common.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = Common.GetMonthFirstDate(PaymentMonthYear);
                //dtUnVerify.Columns.Add("EmployeeId", typeof(String));
                dtUnVerify.Columns.Add("EmployeeId", typeof(String));
                dtUnVerify.Columns.Add("Name", typeof(String));
                dtUnVerify.Columns.Add("Dept", typeof(String));
                dtUnVerify.Columns.Add("Designation", typeof(string));
                dtUnVerify.Columns.Add("DesignationCode", typeof(string));
                dtUnVerify.Columns.Add("FromDate", typeof(DateTime));
                dtUnVerify.Columns.Add("ToDate", typeof(DateTime));
                dtUnVerify.Columns.Add("BankName", typeof(String));
                dtUnVerify.Columns.Add("BankAccountNo", typeof(String));
                dtUnVerify.Columns.Add("IFSC_Code", typeof(String));
                //dtUnVerify.Columns.Add("Branch", typeof(String));
                dtUnVerify.Columns.Add("DOB", typeof(DateTime));
                dtUnVerify.Columns.Add("PAN", typeof(String));
                dtUnVerify.Columns.Add("PreviouseSalary", typeof(Decimal));
                dtUnVerify.Columns.Add("PreviousPT", typeof(Decimal));
                dtUnVerify.Columns.Add("PreviousIT", typeof(Decimal));
                dtUnVerify.Columns.Add("ProjectedSalary", typeof(Decimal));
                dtUnVerify.Columns.Add("ProjectNo", typeof(String));
                dtUnVerify.Columns.Add("CommitmentNo", typeof(String));
                dtUnVerify.Columns.Add("CommitmentBalance", typeof(Decimal));
                dtUnVerify.Columns.Add("Basic", typeof(Decimal));
                //dtUnVerify.Columns.Add("MISS_PAY", typeof(Decimal));
                dtUnVerify.Columns.Add("HRA", typeof(Decimal));
                //dtUnVerify.Columns.Add("MISS_HRA_PAY", typeof(Decimal));
                dtUnVerify.Columns.Add("MedicalAllowance", typeof(Decimal));
                dtUnVerify.Columns.Add("GrossSalary", typeof(Decimal));
                dtUnVerify.Columns.Add("IncomeTax", typeof(Decimal));
                //dtUnVerify.Columns.Add("MISS_REC", typeof(Decimal));
                dtUnVerify.Columns.Add("MedicalInsurance", typeof(Decimal));
                //dtUnVerify.Columns.Add("LOP", typeof(Decimal));
                dtUnVerify.Columns.Add("PROF_TAX", typeof(Decimal));
                dtUnVerify.Columns.Add("TotalDeducation", typeof(Decimal));
                dtUnVerify.Columns.Add("NetPay", typeof(Decimal));
                dtUnVerify.Columns.Add("OtherPay", typeof(Decimal));
                dtUnVerify.Columns.Add("Remarks", typeof(String));

                dtVerify.Columns.Add("Employee No", typeof(String));
                dtVerify.Columns.Add("Name", typeof(String));
                dtVerify.Columns.Add("DOB", typeof(DateTime));
                dtVerify.Columns.Add("Bank", typeof(String));
                //dtVerify.Columns.Add("Branch", typeof(String));
                dtVerify.Columns.Add("IFSC", typeof(String));
                dtVerify.Columns.Add("Account Number", typeof(String));
                dtVerify.Columns.Add("Designation Code", typeof(string));
                //dtVerify.Columns.Add("PayBill", typeof(String));
                dtVerify.Columns.Add("Project No", typeof(String));
                dtVerify.Columns.Add("Commitment No", typeof(String));
                dtVerify.Columns.Add("Dept", typeof(String));
                dtVerify.Columns.Add("Basic Salary", typeof(Decimal));
                dtVerify.Columns.Add("HRA", typeof(Decimal));
                dtVerify.Columns.Add("Medical", typeof(Decimal));
                dtVerify.Columns.Add("PT", typeof(Decimal));
                dtVerify.Columns.Add("IT", typeof(Decimal));
                dtVerify.Columns.Add("Direct Allowance", typeof(Decimal));
                dtVerify.Columns.Add("Deduction", typeof(Decimal));
                dtVerify.Columns.Add("Gross Salary", typeof(Decimal));
                dtVerify.Columns.Add("Net Salary", typeof(Decimal));
                dtVerify.Columns.Add("Distribution", typeof(Decimal));
                dtVerify.Columns.Add("Fellowship", typeof(Decimal));
                dtVerify.Columns.Add("Honorarium", typeof(Decimal));
                dtVerify.Columns.Add("Mandays", typeof(Decimal));
                dtVerify.Columns.Add("Debit Bank", typeof(String));
                using (var context = new IOASDBEntities())
                {
                    var list = (from emp in context.tblRCTPayrollProcessDetail
                                join des in context.tblRCTDesignation on emp.DesignationId equals des.DesignationId
                                where emp.SalaryMonth == PaymentMonthYear && emp.SalaryType == typeOfPay && emp.ProcessStatus == "Active"
                                && !context.tblSalaryPayment.Any(m => m.RCTPayrollProcessDetailId == emp.RCTPayrollProcessDetailId)
                                orderby emp.CandidateName
                                select new
                                {
                                    MakePayment = true,
                                    EmployeeID = emp.EmployeeId,
                                    EmployeeName = emp.CandidateName,
                                    commitmentNo = emp.CommitmentNumber,
                                    PROJECTNO = emp.ProjectNumber,
                                    departmentcode = emp.DepartmentCode,
                                    DEPARTMENT = emp.DepartmentName,
                                    DOB = emp.DOB,
                                    DesignationCode = emp.DesignationCode,
                                    BankName = emp.BankName,
                                    IFSC = emp.IFSC,
                                    BankAccountNumber = emp.AccountNo,
                                    des.Designation,
                                    emp.RCTPayrollProcessDetailId,
                                    IsNotInMain = false
                                })
                                .Concat(from emp in context.vw_RCTAdhocEmployeeMaster/*.AsNoTracking()*/
                                        where context.tblEmpOtherAllowance.Any(m => m.EmployeeIdStr == emp.EmployeeId && (m.IsPaid == null || m.IsPaid == false)) && typeOfPay != 2
                                        && ((emp.IITMPensioner_f == true && typeOfPay == 0) || (emp.IITMPensioner_f == false && typeOfPay != 0))
                                             && !context.tblRCTPayrollProcessDetail.Any(m => m.SalaryMonth == PaymentMonthYear && m.SalaryType == typeOfPay && m.ProcessStatus == "Active" && m.EmployeeId == emp.EmployeeId)
                                             && !context.tblSalaryPayment.Any(m => m.PaymentMonthYear == PaymentMonthYear && m.TypeOfPayBill == typeOfPay && m.EmployeeId == emp.EmployeeId)
                                        orderby emp.NAME
                                        select new
                                        {
                                            MakePayment = true,
                                            EmployeeID = emp.EmployeeId,
                                            EmployeeName = emp.NAME,
                                            commitmentNo = emp.commitmentNo,
                                            PROJECTNO = emp.PROJECTNO,
                                            departmentcode = emp.departmentcode,
                                            DEPARTMENT = emp.DEPARTMENT,
                                            //paybill_id = emp.paybill,
                                            DOB = emp.DOB,
                                            DesignationCode = emp.DesignationCode,
                                            emp.BankName,
                                            IFSC = emp.IFSCCode,
                                            BankAccountNumber = emp.BankAccountNumber,
                                            emp.Designation,
                                            RCTPayrollProcessDetailId = 0,
                                            IsNotInMain = true
                                        })
                                .ToList();

                    for (int i = 0; i < list.Count; i++)
                    {
                       
                        var salary = GetMainSalaryEmployeeDetails(list[i].EmployeeID, list[i].RCTPayrollProcessDetailId, PaymentMonthYear, typeOfPay, list[i].IsNotInMain);

                        if (salary != null && salary.SalaryDetail != null)
                        {
                            decimal ttlDedut = salary.SalaryDetail.MonthlyTax + salary.SalaryDetail.Medical + salary.SalaryDetail.ProfTax;
                            var row = dtUnVerify.NewRow();
                            row["EmployeeId"] = list[i].EmployeeID;
                            row["Name"] = list[i].EmployeeName;
                            row["ProjectNo"] = list[i].PROJECTNO;
                            row["CommitmentNo"] = list[i].commitmentNo;
                            row["Dept"] = list[i].DEPARTMENT;
                            row["Basic"] = Convert.ToDecimal(salary.SalaryDetail.Basic);
                            row["GrossSalary"] = Convert.ToDecimal(salary.SalaryDetail.GrossSalary);
                            row["NetPay"] = Convert.ToDecimal(salary.SalaryDetail.NetSalary);
                            row["FromDate"] = salary.FromDate;
                            row["ToDate"] = salary.ToDate;
                            //row["MISS_PAY"] = salary.SalaryDetail.Mics_Pay;
                            //row["MISS_HRA_PAY"] = salary.SalaryDetail.Mics_HRA_Pay;
                            //row["MISS_REC"] = salary.SalaryDetail.Mics_Recovery;
                            //row["LOP"] = salary.SalaryDetail.LOP;
                            row["ProjectedSalary"] = salary.SalaryDetail.ProjectedSalary;
                            row["HRA"] = salary.SalaryDetail.HRA;
                            row["MedicalAllowance"] = 0;
                            row["MedicalInsurance"] = salary.SalaryDetail.MA;
                            row["TotalDeducation"] = salary.SalaryDetail.Deduction;
                            row["CommitmentBalance"] = Common.GetCommitmentBalance(list[i].commitmentNo);
                            row["IncomeTax"] = salary.SalaryDetail.MonthlyTax;
                            row["PROF_TAX"] = salary.SalaryDetail.ProfTax;
                            //row["PayBill"] = list[i].paybill_id;
                            row["DOB"] = list[i].DOB;
                            row["OtherPay"] = salary.SalaryDetail.OtherAllowanceAmount + salary.SalaryDetail.DirectAllowanceAmount;
                            row["PreviouseSalary"] = salary.SalaryDetail.PreviousGross;
                            row["PreviousPT"] = salary.SalaryDetail.PreviousPT;
                            row["PreviousIT"] = salary.SalaryDetail.PreviousIT;
                            row["DesignationCode"] = list[i].DesignationCode;
                            row["Designation"] = list[i].Designation;
                            row["Remarks"] = "";

                            row["BankName"] = list[i].BankName;
                            row["IFSC_Code"] = list[i].IFSC;
                            row["BankAccountNo"] = list[i].BankAccountNumber;
                            //row["PAN"] = list[i].bank.PAN;
                            dtUnVerify.Rows.Add(row);
                        }
                       
                    }

                    var queryVerified = (from SP in context.tblSalaryPayment
                                             //  join emp in context.vw_RCTAdhocEmployeeMaster on SP.EmployeeId equals emp.EmployeeId //into SalDet                                         
                                         where SP.PaymentHeadId == PaymentHeadId
                                         orderby SP.EmployeeId
                                         select new
                                         {
                                             MakePayment = true,
                                             EmployeeID = SP.EmployeeId,
                                             //  EmployeeName = emp.NAME,
                                             PROJECTNO = SP.ProjectNo,
                                             //  departmentcode = emp.departmentcode,
                                             //  DEPARTMENT = emp.DEPARTMENT,
                                             NetSalary = SP.NetSalary,
                                             PaymentId = SP.PaymentId,
                                             basic = SP.Basic,
                                             gross = SP.GrossTotal,
                                             //  DOB = emp.DOB,
                                             //  DesignationCode = emp.DesignationCode,
                                             //  emp.BankAccountNumber,
                                             //  emp.BankName,
                                             //  emp.IFSCCode,
                                             SP.HRA,
                                             SP.MedicalRecovery,
                                             SP.ProfTax,
                                             SP.MonthlyTax,
                                             SP.DirectAllowance,
                                             SP.Deduction,
                                             SP.RCTPayrollProcessDetailId
                                         }).ToList();

                    for (int i = 0; i < queryVerified.Count; i++)
                    {
                        var EmployeeID = queryVerified[i].EmployeeID;
                        var empname = string.Empty;
                        var DEP = string.Empty;
                        var DOfB = DateTime.Now;
                        var DesCode = string.Empty;
                        var BankName = string.Empty;
                        var IFSCCode = string.Empty;
                        var BankAccountNumber = string.Empty;
                        if (queryVerified[i].RCTPayrollProcessDetailId == null)
                        {
                            var EmployeeMasterQry = context.vw_RCTAdhocEmployeeMaster.Where(m => m.EmployeeId == EmployeeID).OrderByDescending(m => m.EffectiveFrom).FirstOrDefault();
                            if (EmployeeMasterQry != null)
                            {
                                empname = EmployeeMasterQry.NAME;
                                DEP = EmployeeMasterQry.DEPARTMENT;
                                DOfB = EmployeeMasterQry.DOB ?? DateTime.Now;
                                DesCode = EmployeeMasterQry.DesignationCode;
                                BankName = EmployeeMasterQry.BankName;
                                IFSCCode = EmployeeMasterQry.IFSCCode;
                                BankAccountNumber = EmployeeMasterQry.BankAccountNumber;
                            }
                        }
                        else
                        {
                            int rctpaydetailid = queryVerified[i].RCTPayrollProcessDetailId ?? 0;
                            var rctpayrolldetails = context.tblRCTPayrollProcessDetail.Where(x => x.RCTPayrollProcessDetailId == rctpaydetailid).FirstOrDefault();
                            if (rctpayrolldetails != null)
                            {
                                empname = rctpayrolldetails.CandidateName;
                                DEP = rctpayrolldetails.DepartmentName;
                                DOfB = rctpayrolldetails.DOB ?? DateTime.Now;
                                DesCode = rctpayrolldetails.DesignationCode;
                                BankName = rctpayrolldetails.BankName;
                                IFSCCode = rctpayrolldetails.IFSC;
                                BankAccountNumber = rctpayrolldetails.AccountNo;
                            }
                        }
                        int paymentId = queryVerified[i].PaymentId;
                        var oaQuery = (from soa in context.tblAdhocSalaryOtherAllowance
                                       join oa in context.tblEmpOtherAllowance on soa.EmpOtherAllowanceId equals oa.id
                                       where soa.PaymentId == paymentId
                                       select new { oa.ComponentName, soa.Amount }).ToList();
                        decimal? disTtl = oaQuery.Where(m => m.ComponentName == "Distribution").Sum(m => m.Amount);
                        decimal? fellowTtl = oaQuery.Where(m => m.ComponentName == "FellowshipSalary").Sum(m => m.Amount);
                        decimal? honTtl = oaQuery.Where(m => m.ComponentName == "Honororium").Sum(m => m.Amount);
                        decimal? manDayTtl = oaQuery.Where(m => m.ComponentName == "Mandays").Sum(m => m.Amount);

                        var data = Common.GetProjectType(queryVerified[i].PROJECTNO);
                        string bankHd = "Canara Bank-03872-ICSR OH";
                        if (data.Item3 == 4)
                            bankHd = "Canara Bank-03872-ICSR OH";
                        else if (data.Item3 == 2)
                            bankHd = "Canara Bank-01742-PCF";
                        else if (data.Item3 == 3)
                            bankHd = "Canara Bank-08484-RMF";
                        else if (data.Item1 == 2)
                            bankHd = "Canara Bank-16162-Consultancy";
                        else if (data.Item1 == 1 && data.Item2 == 1)
                            bankHd = "Canara Bank-16150-PFMS";
                        else if (data.Item1 == 1)
                            bankHd = "Canara Bank-01741-NON-PFMS";
                        var row = dtVerify.NewRow();
                        row["Employee No"] = queryVerified[i].EmployeeID;
                        row["Name"] = empname;
                        row["Project No"] = queryVerified[i].PROJECTNO;
                        //row["Commitment No"] = queryVerified[i].commitmentNo;
                        row["Dept"] = DEP;
                        row["Basic Salary"] = Convert.ToDecimal(queryVerified[i].basic);
                        row["Gross Salary"] = Convert.ToDecimal(queryVerified[i].gross);
                        row["Net Salary"] = Convert.ToDecimal(queryVerified[i].NetSalary);
                        row["Distribution"] = Convert.ToDecimal(disTtl);
                        row["Fellowship"] = Convert.ToDecimal(fellowTtl);
                        row["Honorarium"] = Convert.ToDecimal(honTtl);
                        row["Mandays"] = Convert.ToDecimal(manDayTtl);
                        row["Net Salary"] = Convert.ToDecimal(queryVerified[i].NetSalary);
                        row["HRA"] = Convert.ToDecimal(queryVerified[i].HRA);
                        row["Medical"] = Convert.ToDecimal(queryVerified[i].MedicalRecovery);
                        row["PT"] = Convert.ToDecimal(queryVerified[i].ProfTax);
                        row["IT"] = Convert.ToDecimal(queryVerified[i].MonthlyTax);
                        row["Direct Allowance"] = Convert.ToDecimal(queryVerified[i].DirectAllowance);
                        row["Deduction"] = Convert.ToDecimal(queryVerified[i].Deduction);
                        row["Debit Bank"] = bankHd;
                        row["DOB"] = DOfB;
                        row["Designation Code"] = DesCode;

                        row["Bank"] = BankName;
                        row["IFSC"] = IFSCCode;
                        row["Account Number"] = BankAccountNumber;

                        dtVerify.Rows.Add(row);

                    }
                }
                ds.Tables.Add(dtUnVerify);
                ds.Tables.Add(dtVerify);
                return ds;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                Console.WriteLine(ex.ToString());
                return ds;
            }
        }



        public bool TestUpdateSalaryPayment(int PaymentHeadId = 147, string currentStatus = "Approval Pending", string newStatus = "Approval Pending", int userId = 55)        {            try            {                string msg = "";                using (var context = new IOASDBEntities())                {                    using (var transaction = context.Database.BeginTransaction())                    {                        List<BillCommitmentDetailModel> txList = new List<BillCommitmentDetailModel>();                        bool result = false;                        try                        {                            var payNo = "";                            var pyamentHead = (from PH in context.tblSalaryPaymentHead                                               where PH.PaymentHeadId == PaymentHeadId && PH.Status == currentStatus                                               select PH).SingleOrDefault();                            if (pyamentHead != null)                            {                                pyamentHead.Status = newStatus;                                pyamentHead.UpdatedBy = userId;                                pyamentHead.UpdatedAt = DateTime.Now;                                context.SaveChanges();                                payNo = pyamentHead.PaymentNo;
                                //var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                                //int processGuideLineId = Process.ProcessGuidelineId;
                                //var fe = FlowEngine.Init(processGuideLineId, userId, PaymentHeadId, "PaymentHeadId");
                                //fe.ProcessInit();
                            }                            if (newStatus == "Approval Pending")                            {
                                //var query = (from c in context.tblAdhocSalaryCommitmentDetail
                                // group c by c.CommitmentDetailId into grp
                                // join det in context.tblCommitmentDetails on grp.FirstOrDefault().CommitmentDetailId equals det.ComitmentDetailId
                                // join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                // where grp.FirstOrDefault().PaymentHeadId == PaymentHeadId && grp.FirstOrDefault().Status == "Active"
                                // select new
                                // {
                                // detailId = grp.Key,
                                // commitmentId = com.CommitmentId,
                                // amount = grp.Sum(m => m.Amount)
                                // }).ToList();
                                //if (query.Count > 0)
                                //{
                                // for (int i = 0; i < query.Count; i++)
                                // {
                                // txList.Add(new BillCommitmentDetailModel()
                                // {
                                // CommitmentDetailId = query[i].detailId,
                                // PaymentAmount = query[i].amount,
                                // CommitmentId = query[i].commitmentId,
                                // ReversedAmount = query[i].amount
                                // });
                                // }
                                //}
                                txList = (from c in context.tblAdhocSalaryCommitmentDetail                                          join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId                                          join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId                                          where c.PaymentHeadId == PaymentHeadId && c.Status == "Active"                                          select new BillCommitmentDetailModel()                                          {                                              CommitmentDetailId = c.CommitmentDetailId,                                              PaymentAmount = c.Amount,                                              CommitmentId = com.CommitmentId,                                              ReversedAmount = c.Amount                                          }).ToList();                                result = true;
                                //result = coreAccountService.UpdateCommitmentBalance(txList, false, false, userId, PaymentHeadId, "SAL");
                                //if (!result)
                                //{
                                //    transaction.Rollback();
                                //    return false;
                                //}
                                BOAModel model = new BOAModel();                                CoreAccountsService coreAccounts = new CoreAccountsService();                                List<BOATransactionModel> txDet = new List<BOATransactionModel>();                                model.TempVoucherNumber = pyamentHead.PaymentNo;                                model.PostedDate = DateTime.Now;                                model.VoucherType = 3;                                model.VoucherNumber = pyamentHead.PaymentNo;                                model.BOAValue = pyamentHead.Amount;                                model.RefNumber = pyamentHead.PaymentNo;                                model.RefTransactionCode = "SAL";                                model.TransactionTypeCode = "SAL";                                txDet = (from exp in context.tblSalaryTransactionDetail                                         where exp.PaymentHeadId == PaymentHeadId                                         select new BOATransactionModel()                                         {                                             AccountHeadId = exp.AccountHeadId,                                             Amount = exp.Amount,                                             TransactionType = exp.TransactionType                                         }).ToList();                                var bankHeadDet = (from exp in context.tblSalaryTransactionDetail                                                   join h in context.tblAccountHead on exp.AccountHeadId equals h.AccountHeadId                                                   where exp.PaymentHeadId == PaymentHeadId && h.Bank_f == true && exp.Amount > 0                                                   select exp).FirstOrDefault();

                                //var paymentQuery = (from sp in context.tblSalaryPayment
                                // join b in context.vwAdhocBankDetails on sp.EmployeeId equals b.Fileno into g
                                // from b in g.DefaultIfEmpty()
                                // where sp.PaymentHeadId == PaymentHeadId
                                // select new
                                // {
                                // sp.NetSalary,
                                // sp.CreatedAt,
                                // sp.ModeOfPayment,
                                // b,
                                // sp.EmployeeId,
                                // sp.EmpNo,
                                // sp.ProjectNo
                                // }).ToList();
                                //int count = paymentQuery.Count();
                                //if (count > 0)
                                //{
                                List<BOAPaymentDetailModel> BOAPaymentDetail = new List<BOAPaymentDetailModel>();
                                // for (int i = 0; i < count; i++)
                                // {
                                // var data = Common.GetProjectType(paymentQuery[i].ProjectNo);
                                // int bankHd = 146;
                                // if (data.Item3 == 4)
                                // bankHd = 146;
                                // else if (data.Item3 == 2)
                                // bankHd = 151;
                                // else if (data.Item3 == 3)
                                // bankHd = 152;
                                // else if (data.Item1 == 2)
                                // bankHd = 149;
                                // else if (data.Item1 == 1 && data.Item2 == 1)
                                // bankHd = 148;
                                // else if (data.Item1 == 1)
                                // bankHd = 20;
                                // BOAPaymentDetail.Add(new BOAPaymentDetailModel()
                                // {
                                // TransactionType = "Credit",
                                // BankHeadID = bankHd,
                                // Amount = paymentQuery[i].NetSalary,
                                // ReferenceNumber = payNo,
                                // ReferenceDate = paymentQuery[i].CreatedAt,
                                // PaymentMode = paymentQuery[i].ModeOfPayment,
                                // PayeeBank = paymentQuery[i].b != null ? paymentQuery[i].b.BankName : "",
                                // StudentRoll = paymentQuery[i].EmployeeId,
                                // Reconciliation_f = false,
                                // PayeeName = paymentQuery[i].EmpNo,
                                // PayeeType = "Adhoc Salary"
                                // });
                                // }
                                // model.BOAPaymentDetail = BOAPaymentDetail;
                                //}
                                BOAPaymentDetail.Add(new BOAPaymentDetailModel()                                {                                    TransactionType = "Credit",                                    BankHeadID = bankHeadDet.AccountHeadId,                                    Amount = bankHeadDet.Amount,                                    ReferenceNumber = payNo,                                    ReferenceDate = DateTime.Now,                                    PaymentMode = 2,                                    PayeeBank = "",                                    StudentRoll = "",                                    Reconciliation_f = false,                                    PayeeName = "Adhoc Salary " + pyamentHead.PaymentMonthYear,                                    PayeeType = "Adhoc Salary"                                });                                model.BOAPaymentDetail = BOAPaymentDetail;                                model.BOATransaction = txDet;                                bool boaTx = coreAccounts.BOATransaction(model);                                if (!boaTx)                                    coreAccountService.UpdateCommitmentBalance(txList, true, false, userId, PaymentHeadId, "SAL");                                if (!result || !boaTx)                                {                                    transaction.Rollback();                                    return false;                                }                                context.tblCommitmentLog.Where(x => x.TransactionTypeCode == "SAL" && x.RefId == PaymentHeadId)                               .ToList()                               .ForEach(m =>                               {                                   m.CRTD_TS = model.PostedDate;                                   m.Posted_f = true;                               });                                context.SaveChanges();                            }

                            //context.Dispose();
                            msg = "Updated successfully";                            transaction.Commit();                        }                        catch (Exception ex)                        {                            IOASException.Instance.HandleMe(this, ex);                            if (result)                                coreAccountService.UpdateCommitmentBalance(txList, true, false, userId, PaymentHeadId, "SAL");                            transaction.Rollback();                            return false;                        }                    }                }                return true;            }            catch (Exception ex)            {                Console.WriteLine(ex.ToString());
                //transaction.Rollback();
                return false;            }        }


    }


}