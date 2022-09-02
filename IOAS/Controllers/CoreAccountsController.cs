﻿using ClosedXML.Excel;
using IOAS.DataModel;
using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IOAS.Controllers
{
    [Authorized]
   
    public class CoreAccountsController : Controller
    {
        
        CoreAccountsService coreAccountService = new CoreAccountsService();
        //  private static readonly Object lockObj = new Object();
        StaffPaymentService payment = new StaffPaymentService();
        AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
        FinOp fo = new FinOp(System.DateTime.Now);
        DateTime Today = System.DateTime.Now;
        private static readonly Object ProjectlockObj = new Object();
        private static readonly Object POWfInitlockObj = new Object();
        private static readonly Object TravellockObj = new Object();
        private static readonly Object TravelBilllockObj = new Object();
        private static readonly Object SBIBilllockObj = new Object();
        private static readonly Object SBICardBilllockObj = new Object();
        private static readonly Object SBICardRecBilllockObj = new Object();
        private static readonly Object TMPadvlockObj = new Object();
        private static readonly Object TMPSettlockObj = new Object();
        private static readonly Object TMPSettWFInitlockObj = new Object();
        private static readonly Object SummerInWFInitlockObj = new Object();
        private static readonly Object PartTimeWFInitlockObj = new Object();
        private static readonly Object CLPWFInitlockObj = new Object();
        private static readonly Object ImprestWFInitlockObj = new Object();
        private static readonly Object ImprestEnhanWFInitlockObj = new Object();
        private static readonly Object ImprestBillWFInitlockObj = new Object();
        private static readonly Object ImprestRecWFInitlockObj = new Object();
        private static readonly Object JournalWFInitlockObj = new Object();
        private static readonly Object REMWFInitlockObj = new Object();
        private static readonly Object PFTWFInitlockObj = new Object();
        private static readonly Object PDTWFInitlockObj = new Object();
        private static readonly Object ContraWFInitlockObj = new Object();
        private static readonly Object DISWFInitlockObj = new Object();
        private static readonly Object NBLWFInitlockObj = new Object();
        private static readonly Object GVRWFInitlockObj = new Object();
        private static readonly Object GVRApprovelockObj = new Object();
        private static readonly Object FRMWFInitlockObj = new Object();
        private static readonly Object OtherReceiptWFInitlockObj = new Object();
        private static readonly Object HeadCreditWFInitlockObj = new Object();
        private static readonly Object LCopenWFInitlockObj = new Object();
        private static readonly Object LCAmdWFInitlockObj = new Object();
        private static readonly Object LCRetWFInitlockObj = new Object();
        private static readonly Object HONWFInitlockObj = new Object();
        private static readonly Object TDSWFInitlockObj = new Object();
        private static readonly Object FSSWFInitlockObj = new Object();
        private static readonly Object MDYWFInitlockObj = new Object();
        private static readonly Object OHARWFInitlockObj = new Object();
        private static readonly Object PaymentVerifyWFInitlockObj = new Object();
        private static readonly Object PaymentWFInitlockObj = new Object();
        private static readonly Object AVOApprovelockObj = new Object();
        private static readonly Object AVOWFInitlockObj = new Object();
        public static readonly object ReceiptBUlockObj = new Object();
        private static readonly Object lockInvoiceBOArequestObj = new Object();
        #region Payment
        #region Purchase Order
        #region Advance

        public ActionResult AdvanceBillPaymentList()
        {
            //ViewBag.processGuideLineId = 1006;
            //var fe = FlowEngine.Init(1006, 1, 4, "BillId");
            //fe.ProcessInit();
            return View();

        }


        public ActionResult AdvanceBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidateBillOnEdit(billId, "ADV"))
                {
                    model = coreAccountService.GetBillDetails(billId);

                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(29);
                model.InclusiveOfTax_f = false;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }
        public ActionResult AdvanceBillPaymentView(int billId, bool Pfinit = false)
        {
            try
            {
                BillEntryModelViewModel model = new BillEntryModelViewModel();
                model = coreAccountService.GetBillViewDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(29, "Others", amt ?? 0);

                //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                model.PFInit = Pfinit;
                model.InclusiveOfTax_f = false;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdvanceBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();

                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                ModelState.Remove("InvoiceNumber");
                ModelState.Remove("InvoiceDate");
                ModelState.Remove("BankHead");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdvanceBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvanceBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been added successfully.";

                        return RedirectToAction("AdvanceBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been updated successfully.";
                        return RedirectToAction("AdvanceBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Advance Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    //else if (result == -4)
                    //    TempData["errMsg"] = "Same invoice number exists for this Vendor.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();

                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdvanceBillPayment(BillEntryModel model)
        {
            decimal ttlAdvAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;

            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                ttlAdvAmt += advAmt;
                ttlAdvAmt = Math.Round(ttlAdvAmt, 2, MidpointRounding.AwayFromZero);
            }

            if (ttlAdvAmt != commitmentAmt)
                msg = "There is a mismatch between the Total advance value and allocated commitment value. Please update the value to continue.";
            //if (ttlAdvAmt != ttlExpAmt)
            //    msg = "There is a mismatch between the requested advance value and transaction value. Please update the value to continue.";
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }

        #endregion
        #region Part
        public ActionResult AdvancePartBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                 ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidatePartBillOnEdit(billId, true))
                {
                    model = coreAccountService.GetBillDetails(billId);

                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(29);

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        public ActionResult AdvancePartBillPaymentView(int billId, bool Pfinit = false)
        {
            try
            {

                BillEntryModelViewModel model = new BillEntryModelViewModel();
                model = coreAccountService.GetBillViewDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(30, "Others", amt ?? 0);

                //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                model.PFInit = Pfinit;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdvancePartBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }

                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdvancePartBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvancePartBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Part Bill has been added successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Part Bill has been updated successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Advance Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    //else if (result == -4)
                    //    TempData["errMsg"] = "Same invoice number exists for this vendor.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdvancePartBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                ttlAdvAmt += advAmt;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            ttlGSTElgAmt = Math.Round(ttlGSTElgAmt, 2, MidpointRounding.AwayFromZero);
            netAdvAmt = Math.Round(netAdvAmt, 2, MidpointRounding.AwayFromZero);
            netDrAmt = Math.Round(netDrAmt, 2, MidpointRounding.AwayFromZero);
            if ((netAdvAmt - ttlGSTElgAmt) != commitmentAmt)
                msg = "There is a mismatch between the part payment total value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";

            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            if (99 < model.AdvancePercentage)
                msg = msg == "Valid" ? "Part percentage should not be greater than 99%." : msg + "<br /> Part percentage should not be greater than 99%.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        public ActionResult PartBillPaymentList()
        {
            return View();

        }

        public ActionResult PartBillPaymentView(int billId, bool Pfinit = false)
        {
            try
            {

                BillEntryModelViewModel model = new BillEntryModelViewModel();

                model = coreAccountService.GetBillViewDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(30, "Others", amt ?? 0);

                //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                model.PFInit = Pfinit;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }
        public ActionResult PartBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                //ViewBag.AdvPctList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidatePartBillOnEdit(billId))
                {
                    model = coreAccountService.GetBillDetails(billId);

                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                    //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(30);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        [HttpPost]
        public ActionResult PartBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidatePartBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.PartBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Payment Bill has been added successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Payment Bill has been updated successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId);

                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        private string ValidatePartBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                ttlAdvAmt += advAmt;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            ttlGSTElgAmt = Math.Round(ttlGSTElgAmt, 2, MidpointRounding.AwayFromZero);
            netAdvAmt = Math.Round(netAdvAmt, 2, MidpointRounding.AwayFromZero);
            netDrAmt = Math.Round(netDrAmt, 2, MidpointRounding.AwayFromZero);
            if ((netAdvAmt - ttlGSTElgAmt) != commitmentAmt)
                msg = "There is a mismatch between the part payment total value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";
            //if (!model.InclusiveOfTax_f && gst != "NotEligible" && ttlAdvAmt != netCrAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the part payment value and credit value. Please update the value to continue." : msg + "<br />There is a mismatch between the part payment value and credit value. Please update the value to continue.";
            //else if ((model.InclusiveOfTax_f || gst == "NotEligible") && netAdvAmt != netCrAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the part payment total value and credit value. Please update the value to continue." : msg + "<br />There is a mismatch between the part payment total value and credit value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            string poNumber = Common.GetBillPONumber(Convert.ToInt32(model.PONumber));
            if (Common.GetBillRMNGPercentage(poNumber, model.VendorId, model.BillId) < model.AdvancePercentage)
                msg = msg == "Valid" ? "Part percentage should not be greater than remaining percentage." : msg + "<br /> Part percentage should not be greater than remaining percentage.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        #endregion
        #region Settlement

        public ActionResult SettlementBillPaymentList()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public ActionResult SettlementBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetCodeControlList("SettlementType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();
                //coreAccountService.BillBackEndEntry(31, Common.GetUserid(User.Identity.Name));
                if (billId > 0 && Common.ValidateBillOnEdit(billId, "STM"))
                {
                    model = coreAccountService.GetBillDetails(billId);
                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                    ViewBag.invoiceTaxAmt = model.InvoiceTaxAmount.ToString();
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(31);

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        public ActionResult SettlementBillPaymentView(int billId, bool Pfinit = false)
        {
            try
            {

                BillEntryModelViewModel model = new BillEntryModelViewModel();
                model = coreAccountService.GetBillViewDetails(billId);
                ViewBag.invoiceTaxAmt = model.InvoiceTaxAmount.ToString();
                var amt = model.BillAmount + model.BillTaxAmount;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(31, "Others", amt ?? 0);
                model.PFInit = Pfinit;
                //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult SettlementBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetCodeControlList("SettlementType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.invoiceTaxAmt = model.InvoiceTaxAmount.ToString();
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                ModelState.Remove("AdvancePercentage");
                ModelState.Remove("InvoiceNumber");
                ModelState.Remove("InvoiceDate");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSettlementBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SettlementBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been added successfully.";
                        return RedirectToAction("SettlementBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been updated successfully.";
                        return RedirectToAction("SettlementBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.POTypeList = Common.GetCodeControlList("PO Type");
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);

                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.invoiceTaxAmt = model.InvoiceTaxAmount.ToString();
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        private string ValidateSettlementBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.PaymentType != 2)
            {
                foreach (var item in model.PODetail)
                {
                    decimal advAmt = item.TotalAmount ?? 0;
                    decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                    //ttlAdvAmt += advAmt;
                    //netAdvAmt += advAmt + advTax;
                    if (item.IsTaxEligible)
                        ttlGSTElgAmt = ttlGSTElgAmt + advTax;
                }
                ttlAdvAmt = model.InvoiceAmount ?? 0;
                netAdvAmt = ttlAdvAmt + (model.InvoiceTaxAmount ?? 0);
            }
            else
            {
                foreach (var item in model.PODetail)
                {
                    if (item.IsTaxEligible)
                    {
                        decimal advAmt = item.TotalAmount ?? 0;
                        decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                        ttlGSTElgAmt = ttlGSTElgAmt + advTax;
                    }
                }
                ttlAdvAmt = (model.InvoiceAmount ?? 0) - (model.hiddenSettAmt ?? 0);
                netAdvAmt = ttlAdvAmt + (Convert.ToDecimal(model.InvoiceTaxAmount) - Convert.ToDecimal(model.hiddenSettTaxAmt));
            }
            ttlGSTElgAmt = Math.Round(ttlGSTElgAmt, 2, MidpointRounding.AwayFromZero);
            netAdvAmt = Math.Round(netAdvAmt, 2, MidpointRounding.AwayFromZero);
            netAdvAmt = netAdvAmt - ttlGSTElgAmt;
            if (netAdvAmt != commitmentAmt)
                msg = "There is a mismatch between the settlement value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != (model.InvoiceAmount + model.InvoiceTaxAmount) && !model.RCM_f)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and invoice value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and invoice value. Please update the value to continue.";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";
            //if (gst == "NotEligible" && netCrAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "Total Credit and Total Commitment Values are not equal." : msg + "<br />Total Credit and Total Commitment Values are not equal.";
            //if (model.PaymentType == 2)
            //{
            //    var data = Common.ValidateSettlement(Common.GetBillPONumber(model.selPONumber ?? 0), model.VendorId, ttldeductAmt, ttlExpAmt, model.BillId);
            //    if (data != "Valid")
            //        msg = msg == "Valid" ? data : msg + "<br /> " + data;
            //}
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        #endregion
        #region Common
        private string ValidateDuplicateBillInvoiceNumber(string typeCode, int refid)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                try
                {
                    var Qry = context.tblBillEntry.Where(m => m.BillId == refid).FirstOrDefault();
                    int vendorid = Qry.VendorId ?? 0;

                    if (typeCode == "STM")
                    {

                        var InvoiceQry = context.tblBillInvoiceDetail.Where(m => m.BillId == refid).Select(m => m.InvoiceNumber).ToList();
                        int QryBillid = (from a in context.tblBillEntry
                                         join b in context.tblBillInvoiceDetail on a.BillId equals b.BillId
                                         where a.VendorId == vendorid && a.Status != "InActive" && a.Status != "Rejected" && a.BillId != refid
                                         && InvoiceQry.Contains(b.InvoiceNumber) && a.TransactionTypeCode == typeCode
                                         select a.BillId).FirstOrDefault();
                        if (QryBillid > 0)
                            return "Duplicate";

                        else
                            return msg;
                    }
                    else if (typeCode == "PTM")
                    {
                        var InvoiceQry = context.tblBillEntry.Where(m => m.BillId == refid).Select(m => m.InvoiceNumber).FirstOrDefault();
                        int QryBillid = (from a in context.tblBillEntry
                                         where a.VendorId == vendorid && a.Status != "InActive" && a.BillId != refid
                                         && InvoiceQry == a.InvoiceNumber && a.TransactionTypeCode == typeCode
                                         select a.BillId).FirstOrDefault();
                        if (QryBillid > 0)
                            return "Duplicate";

                        else
                            return msg;
                    }
                    return msg;
                }
                catch (Exception ex)
                {
                    Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                    return "Not Valid";
                }
            }

        }
        [HttpGet]
        public JsonResult LoadvendorList(string term)
        {
            try
            {
                var data = Common.LoadAutoCompleteVendor(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetVendorList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteVendor(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadDISAdhocStaffList(string term)
        {
            try
            {
                var data = Common.GetDISAutoCompleteAdhocStaffWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetBillPaymentList(string typeCode, AdvanceBillSearchModel model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetBillPaymentList(typeCode, model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PreviousBillHistory(int vendorId)
        {
            try
            {
                var data = coreAccountService.GetBillHistoryList(vendorId);
                ViewBag.data = data;
                ViewBag.TtlAmt = data.Sum(m => m.BillAmount);
                return PartialView();
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception();
            }
        }

        [HttpPost]
        public ActionResult POWFInit(int billId, string transTypeCode)
        {
            try
            {

                lock (POWfInitlockObj)
                {
                    if (Common.ValidateBillOnEdit(billId, transTypeCode))
                    {
                        if (ValidateDuplicateBillInvoiceNumber(transTypeCode, billId) == "Valid")
                        {

                            int userId = Common.GetUserid(User.Identity.Name);
                            bool cStatus = coreAccountService.BillCommitmentBalanceUpdate(billId, false, false, userId, transTypeCode);
                            if (!cStatus)
                                return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                            bool status = coreAccountService.POWFInit(billId, userId, transTypeCode);
                            if (!status)
                                coreAccountService.BillCommitmentBalanceUpdate(billId, true, false, userId, transTypeCode);
                            return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { status = false, msg = "This bill contain invoice number" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBillPaymentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetBillPaymentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetVendorDetails(int vendorId, bool poNumberRequired = false, string transTypeCode = "", bool TDSRequired = false)
        {
            try
            {
                var output = coreAccountService.GetVendorDetails(vendorId);
                if (poNumberRequired)
                    output.PONumberList = Common.GetBillPONumberList(vendorId, null, transTypeCode);
                if (TDSRequired)
                    output.TDSList = Common.GetVendorTDSList(vendorId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTypeOfServiceList(int type)
        {
            try
            {
                object output = Common.GetTypeOfServiceList(type);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetServiceTypeDetail(int serviceType)
        {
            try
            {
                var data = Common.GetServiceDetail(serviceType);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchCommitments(DateTime? fromDate, DateTime? toDate, int? projectType, int projectId, string keyword, int commitmentType = 0, int ProjectClassification = 0)
        {
            try
            {
                object output = coreAccountService.SearchCommitments(fromDate, toDate, projectType, projectId, keyword, commitmentType, ProjectClassification);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public JsonResult GetHeadwiseExpenditure(int projectId)
        {
            try
            {
                ProjectService _ps = new ProjectService();
                object output = _ps.getProjectSummaryDetails(projectId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetSelectedCommitmentDetails(Int32[] selCommitment)
        {
            try
            {
                selCommitment = selCommitment.Distinct().ToArray();
                object output = coreAccountService.GetSelectedCommitmentDetails(selCommitment);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTransactionDetails(string typeCode, bool interstate_f = false, bool eligibilityCheck_f = false, int deductionCategoryId = 0, string tSubCode = "1", List<int?> TDSDetailId = null)
        {
            try
            {
                object output = coreAccountService.GetTransactionDetails(deductionCategoryId, interstate_f, typeCode, tSubCode, eligibilityCheck_f, TDSDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAddNewExpenseDetails(string typeCode, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetAddNewExpenseDetails(typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAccountHead(Int32 accountGroupId, bool? isBank = null)
        {
            try
            {
                object output = Common.GetAccountHeadList(accountGroupId, 0, "", "", isBank);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetBillPODetails(Int32 billId, bool isSettlement = false, int settlId = 0)
        {
            try
            {
                var output = coreAccountService.GetBillPODetails(billId);
                var typeList = Common.GetTypeOfServiceList(output.BillType ?? 0);
                if (!isSettlement)
                {
                    var rmnBal = Common.GetBillRMNGPercentage(output.PONumber, output.VendorId, settlId);
                    //rmnBal -= 1;
                    return Json(new { data = output, rmnBal = rmnBal, typeList = typeList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = Common.GetBillPaidAndRMNGAmt(output.PONumber, output.VendorId);
                    return Json(new { data = output, billAmt = data.Item3, billTaxAmt = data.Item4, settAmt = data.Item1, settTaxAmt = data.Item2, typeList = typeList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetTransactionType(int accountGroupId, int accountHeadId, string typeCode, string tSubCode = "1")
        {
            try
            {
                var output = Common.GetTransactionType(accountGroupId, accountHeadId, typeCode, tSubCode);
                return Json(new { tType = output.Item1, isJv = output.Item2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion
        #region Travel
        #region Advance

        public ActionResult TravelAdvancePaymentList()
        {
            return View();
        }


        public ActionResult TravelAdvancePayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                TravelAdvanceModel model = new TravelAdvanceModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Open"))
                {
                    model = coreAccountService.GetTravelAdvanceDetails(travelBillId);

                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult TravelAdvancePaymentView(int travelBillId, bool Pfinit = false)
        {
            try
            {

                TravelAdvanceModel model = new TravelAdvanceModel();
                model.PFInit = Pfinit;
                model = coreAccountService.GetTravelAdvanceDetailsView(travelBillId);
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(36, "Others", model.AdvanceValue ?? 0);
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult TravelAdvancePayment(TravelAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvanceTravelBillIU(model, logged_in_user);
                    if (model.TravelBillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been added successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been updated successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        public ActionResult TravelAdvanceBillEntry(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.CountryList = Common.getCountryList();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TravelAdvanceBillEntryModel model = new TravelAdvanceBillEntryModel();

                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Entry"))
                {
                    var advModel = coreAccountService.GetTravelAdvanceDetails(travelBillId);
                    model = JsonConvert.DeserializeObject<TravelAdvanceBillEntryModel>(JsonConvert.SerializeObject(advModel));

                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(36);
                }
                else if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Approval"))
                {
                    model = coreAccountService.GetTravelAdvanceBillEntryDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();

                }
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult TravelAdvanceBillEntryView(int travelBillId, bool Pfinit = false)
        {
            try
            {

                TravelAdvanceBillEntryModel model = new TravelAdvanceBillEntryModel();

                model = coreAccountService.GetTravelAdvanceBillEntryDetailsView(travelBillId);
                model.CreditorType = "PI / Clearance agent / Student";
                model.PFInit = Pfinit;
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(36, "Others", model.AdvanceValue ?? 0);

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult TravelAdvanceBillEntry(TravelAdvanceBillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "PI / Clearance agent / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTADPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TravelADVBillEntryIU(model, logged_in_user);
                    if (result == 1)
                    {
                        TempData["succMsg"] = "Travel advance bill entry has been added successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else if (result == 2)
                    {
                        TempData["succMsg"] = "Travel advance bill entry has been updated successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.CountryList = Common.getCountryList();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult DeleteTravelAdvanceBill(int travelBillId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.DeleteTravelAdvanceBill(travelBillId, userId);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult TravelAdvanceBillWFInit(int travelBillId, string transCode)
        {
            try
            {
                lock (TravellockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    //if (transCode == "TAD")
                    //{
                    //    bool status = coreAccountService.TravelAdvanceBillWFInit(travelBillId, userId, transCode);
                    //    return Json(status, JsonRequestBehavior.AllowGet);
                    //}
                    if (transCode == "TAD" && Common.ValidateTravelBillStatus(travelBillId, transCode, "Pending Bill Approval"))
                    {
                        bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, false, userId, "TAD");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TravelAdvanceBillWFInit(travelBillId, userId, transCode);
                        if (!status)
                            coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, userId, "TAD");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (Common.ValidateTravelBillStatus(travelBillId, transCode, "Open"))
                    {
                        bool reversed = false;
                        if (transCode == "TST")
                            reversed = Common.TSTBillIsReceipt(travelBillId);
                        bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, reversed, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TravelAdvanceBillWFInit(travelBillId, userId, transCode);
                        if (!status)
                            coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, reversed, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult _TravelADVCommitment(int travelBillId)
        //{
        //    CommitmentModel model = new CommitmentModel();
        //    ViewBag.CommitmentType = Common.getCommitmentType();
        //    ViewBag.Purpose = Common.getPurpose();
        //    ViewBag.Currency = Common.getCurrency();
        //    ViewBag.BudgetHead = Common.getBudgetHead();
        //    ViewBag.ProjectType = Common.getprojecttype();
        //    ViewBag.AccountGroup = Common.getAccountGroup();
        //    ViewBag.AccountHead = Common.getBudgetHead();
        //    ViewBag.Vendor = Common.getVendor();
        //    model = coreAccountService.GetTABillDetailsForCommitment(travelBillId);
        //    if (model.selRequestRefrence == 0)
        //        throw new Exception();
        //    var Data = Common.getProjectNo(model.selProjectType);
        //    ViewBag.ProjectNo = Data.Item1;
        //    model.TravelBillId = travelBillId;
        //    model.CommitmentNo = "0";
        //    model.commitmentValue = 0;
        //    model.currencyRate = 0;
        //    return PartialView(model);

        //}
        //[HttpPost]
        //public ActionResult _TravelADVCommitment(CommitmentModel model)
        //{
        //    try
        //    {
        //        var UserId = Common.GetUserid(User.Identity.Name);
        //        coreAccountService _AS = new coreAccountService();
        //        int result = 0;
        //        if (ModelState.IsValid)
        //        {
        //            result = _AS.SaveCommitDetails(model, UserId, true);
        //            if (result > 0)
        //            {
        //                coreAccountService.UpdateTAStatusOnBookCommitment(model.TravelBillId ?? 0, UserId, result);
        //                TempData["succMsg"] = "Commitment has been booked successfully";
        //            }
        //            else
        //            {
        //                TempData["errMsg"] = "Something went wrong please contact administrator";
        //            }
        //        }
        //        else
        //        {
        //            string messages = string.Join("<br />", ModelState.Values
        //                               .SelectMany(x => x.Errors)
        //                               .Select(x => x.ErrorMessage));

        //            TempData["errMsg"] = messages;
        //        }
        //        return RedirectToAction("TravelAdvancePaymentList");
        //    }
        //    catch (Exception ex)
        //    {

        //        TempData["errMsg"] = "Something went wrong please contact administrator";
        //        return RedirectToAction("TravelAdvancePaymentList");
        //    }
        //}
        [HttpPost]
        public ActionResult TravelAdvanceBillApproved(int travelBillId)
        {
            try
            {
                lock (TravelBilllockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    //if (Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Approval"))
                    //{
                    //    bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, false, userId, "TAD");
                    //    if (!cStatus)
                    //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    //    bool status = coreAccountService.TravelAdvanceBillApproved(travelBillId, userId);
                    //    if (!status)
                    //        coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, userId, "TAD");
                    //    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    //}
                    if (Common.ValidateTravelBillStatus(travelBillId, "TAD", "Open"))
                    {
                        bool status = coreAccountService.TravelAdvanceBillApproved(travelBillId, userId);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        private string ValidateTADPayment(TravelAdvanceBillEntryModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal netADVAmt = model.AdvanceValue ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal validCmtAmt = netADVAmt - ttlGSTElg;
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }

            if (validCmtAmt != commitmentAmt)
                msg = "There is a mismatch between the requested advance value and allocated commitment value. Please update the value to continue.";
            if (model.AdvanceValue != paymentBUAmt)
                msg = msg == "Valid" ? "There is a mismatch between the requested advance value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the requested advance value and payment break up total value. Please update the value to continue.";
            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region Settlement
        public ActionResult TravelSettlementPaymentList()
        {
            return View();
        }

        public ActionResult TravelSettlementPayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                TravelSettlementModel model = new TravelSettlementModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TST", "Open"))
                {
                    model = coreAccountService.GetTravelSettlementDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);

                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(43);
                    model.NeedUpdateTransDetail = true;
                }
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult TravelSettlementPayment(TravelSettlementModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                if (model.TravelBillId > 0)
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);

                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                model.CreditorType = "PI / Clearance agent / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (model.OverallExpense < model.AdvanceAmount)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                }
                else if (model.OverallExpense == model.AdvanceAmount)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.ExpenseDetail.Count(); i++)
                    {
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
                        ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
                        ModelState.Remove("ExpenseDetail[" + i + "].Amount");
                        ModelState.Remove("ExpenseDetail[" + i + "].IsJV");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTSTPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TravelSettlementIU(model, logged_in_user);
                    if (model.TravelBillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Travel settlement has been added successfully.";
                        return RedirectToAction("TravelSettlementPaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Travel settlement has been updated successfully.";
                        return RedirectToAction("TravelSettlementPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                if (model.TravelBillId > 0)
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);

                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        [HttpGet]
        public JsonResult GetTravelAdvanceDetails(int travelBillId)
        {
            try
            {
                object output = coreAccountService.GetTravelAdvanceDetailsForSettlement(travelBillId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        public ActionResult TravelSettlementPaymentView(int travelBillId, bool Pfinit = false)
        {
            try
            {
                int pgId = 0;
                var data = Common.GetTravelCommitmentProjectDetail(travelBillId);
                int pType = data.Item1;
                int sponCate = data.Item2;

                ViewBag.disabled = "disabled";
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();

                TravelSettlementModel model = new TravelSettlementModel();

                model = coreAccountService.GetTravelSettlementDetailsView(travelBillId);
                if (pType == 1 && sponCate == 1)
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(43, "PFMS", model.OverallExpense ?? 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(43, "Others", model.OverallExpense ?? 0);

                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                if (model.TravelBillId > 0)
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);
                model.PFInit = Pfinit;
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpGet]
        public JsonResult GetTravellerDailyAllowance(int countryId)
        {
            try
            {
                decimal output = Common.GetTravellerDailyAllowance(countryId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        private string ValidateTSTPayment(TravelSettlementModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal overallExp = model.OverallExpense ?? 0;
            decimal piAdvAmt = model.AdvanceValueWOClearanceAgent ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);

            if (piAdvAmt < overallExp)
            {
                if (model.CommitmentAmount != model.CommitmentDetail.Sum(m => m.PaymentAmount))
                {
                    msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                    return msg;
                }
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                decimal validCmtAmt = overallExp - piAdvAmt - ttlGSTElg;
                if (validCmtAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if ((overallExp - piAdvAmt) != paymentBUAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payable value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payable value and payment break up total value. Please update the value to continue.";

            }
            else if (piAdvAmt > model.OverallExpense)
            {
                if (model.CommitmentAmount != model.CommitmentDetail.Sum(m => m.ReversedAmount))
                {
                    msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                    return msg;
                }
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount < item.ReversedAmount)
                        msg = msg == "Valid" ? "Commitment reversed value should not be less than booked value." : msg + "<br /> Commitment reversed value should not be less than booked value.";
                }
                decimal validCmtAmt = piAdvAmt - overallExp + ttlGSTElg;
                if (validCmtAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue.";
                if (0 != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry. You can't give Payment Break Up value." : msg + "<br /> Not a valid entry. You can't give Payment Break Up value.";
            }
            else
            {
                if (0 != (model.CommitmentAmount ?? 0))
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if (0 != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry. You can't give Payment Break Up value." : msg + "<br /> Not a valid entry. You can't give Payment Break Up value.";
            }
            if (netCrAmt != model.PayableValue)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;

            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region Domestic Travel
        public ActionResult DomesticTravelPaymentList()
        {
            return View();
        }
        public ActionResult DomesticTravelPayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                DomesticTravelBillEntryModel model = new DomesticTravelBillEntryModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "DTV", "Open"))
                {
                    model = coreAccountService.GetDomesticTravelDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(57);
                    model.NeedUpdateTransDetail = true;
                }
                model.CreditorType = "PI / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult DomesticTravelPayment(DomesticTravelBillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "PI / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateDTVPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    if (model.InvoiceAttachment != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string taxprooffilename = Path.GetFileName(model.InvoiceAttachment.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.DomesticTravelIU(model, logged_in_user);
                    if (model.TravelBillId == null && result > 0)
                    {
                        TempData["succMsg"] = "Domestic travel bill entry has been added successfully.";
                        return RedirectToAction("DomesticTravelPaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Domestic travel bill entry has been updated successfully.";
                        return RedirectToAction("DomesticTravelPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                //ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        public ActionResult DomesticTravelPaymentView(int travelBillId, bool Pfinit = false)
        {
            try
            {
                var data = Common.GetTravelCommitmentProjectDetail(travelBillId);
                int pType = data.Item1;
                int sponCate = data.Item2;

                DomesticTravelBillEntryModel model = new DomesticTravelBillEntryModel();
                model = coreAccountService.GetDomesticTravelDetailsView(travelBillId);
                if (pType == 1 && sponCate == 1)
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(57, "PFMS", model.OverallExpense ?? 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(57, "Others", model.OverallExpense ?? 0);

                model.PFInit = Pfinit;
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                model.CreditorType = "PI / Student";
                ViewBag.disabled = "disabled";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateDTVPayment(DomesticTravelBillEntryModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal overAllExp = model.BreakUpDetail.Select(m => m.ProcessedAmount).Sum() ?? 0;
            decimal payBUTtl = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            payBUTtl = payBUTtl + (model.PaymentTDSAmount ?? 0);
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal validCmtAmt = overAllExp - ttlGSTElg;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;

            if (validCmtAmt != commitmentAmt || overAllExp == 0)
                msg = "There is a mismatch between the overall expense value and allocated commitment value. Please update the value to continue.";
            if (payBUTtl != overAllExp)
                msg = msg == "Valid" ? "There is a mismatch between the payable value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payable value and payment break up total value. Please update the value to continue.";
            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (netCrAmt != model.OverallExpense)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            //var gPayee = model.PaymentBreakDetail.GroupBy(m => m.UserId);
            //if (gPayee.Count() != model.PaymentBreakDetail.Count())
            //    msg = msg == "Valid" ? "Selected payee already exist. Please select a different payee." : msg + "<br /> Selected payee already exist. Please select a different payee.";
            //var gBreakUp = model.BreakUpDetail.GroupBy(m => m.ExpenseTypeId);
            //if (gBreakUp.Count() != model.BreakUpDetail.Count())
            //    msg = msg == "Valid" ? "Selected expense type already exist. Please select a different expense type." : msg + "<br /> Selected expense type already exist. Please select a different expense type.";
            //var gTraveller = model.TravelerDetail.GroupBy(m => m.UserId);
            //if (gTraveller.Count() != model.TravelerDetail.Count())
            //    msg = msg == "Valid" ? "Selected traveller already exist. Please select a different traveller." : msg + "<br /> Selected traveller already exist. Please select a different traveller.";

            return msg;
        }
        #endregion
        #region Common
        [HttpPost]
        public JsonResult GetTravelBillList(string typeCode, SearchViewModel model, int pageIndex, int pageSize, DateFilterModel RequestedDate, DateFilterModel TravelstrDate, DateFilterModel TravelfinDate)
        {
            try
            {
                object output = coreAccountService.GetTravelBillList(typeCode, model, pageIndex, pageSize, RequestedDate, TravelstrDate, TravelfinDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetTravelBillList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTravelBillList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadPIList(string term)
        {
            try
            {
                var data = Common.GetAutoCompletePIWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadFacultyList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteFacultyDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadACProjectStaff(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteTandMWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadAccountHead(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteAccountHead(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadBankAccountHead(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteBankAccountHead(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadTypeOfServiceList(string term, int? type = null)
        {
            try
            {
                var data = Common.GetAutoCompleteTypeOfServiceList(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadClearanceAgentList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteClearanceAgent(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadTravelAgencyList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteTravelAgency(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadStudentList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteStudentList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult GetTravelCommitmentDetails(int travelBillId)
        {
            var data = coreAccountService.GetTravelCommitmentDetails(travelBillId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadProjectList(string term, int? type = null, int? classification = null)
        {
            try
            {
                lock (ProjectlockObj)
                {
                    var data = Common.GetAutoCompleteProjectList(term, type);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPIADVList(int PI)
        {
            try
            {
                var data = Common.GetTravelADVList(null, PI);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadTandMList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteTandMWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadAdhocStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteAdhocStaffWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadInstituteStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteInstituteStaffWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion

        #region SBI Card
        public ActionResult SBIECardList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }

        public ActionResult SBIECard(int ProjectId = 0, int CardID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                model.SBIEcardId = CardID;
                model.ProjectID = ProjectId;
                model.TotalValueofCard = 0;
                var projectdetails = Common.GetPIdetailsbyProject(ProjectId);
                model.NameofPI = projectdetails.PIName;
                model.PIDepartmentName = projectdetails.PIDepartment;
                model.PIId = projectdetails.PIId;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (ProjectId > 0 && CardID > 0 && Common.ValidateSBICardPjctdtlsOnEdit(ProjectId, "ECD"))
                {
                    model = coreAccountService.EditProjectCardDetails(CardID);
                    model.SBIEcardId = CardID;
                }
                else if (ProjectId > 0 && CardID == 0)
                {
                    string validationMsg = Common.ValidateSBICardPjctAddition(ProjectId, CardID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(39);
                }
                else if (ProjectId == 0 && CardID == 0)
                {
                    return RedirectToAction("SBIECardList", "CoreAccounts");
                }
                else
                    model.CreditorType = "PI";
                model.CheckListDetail = Common.GetCheckedList(39);
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult SBIECard(SBIECardModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSBIECardBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }
                    if (model.CurrentProjectAllotmentValue > 10000)
                    {
                        TempData["errMsg"] = "The amount allocated per project cannot be greater than Rs 10000";
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }
                    var SBIECardID = _ps.CreateSBIECard(model, loggedinuserid);
                    if (SBIECardID > 0)
                    {
                        var SBIECardNumber = Common.getSBIEcardnumber(SBIECardID);
                        ViewBag.succMsg = "Project added to SBI Prepaid Card with Card number - " + SBIECardNumber + ".";
                    }
                    if (SBIECardID == -2)
                    {
                        var SBIECardNumber = Common.getSBIEcardnumber(model.SBIEcardId);
                        ViewBag.succMsg = SBIECardNumber + " - Card and Project details updated.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }

        private string ValidateSBIECardBillPayment(SBIECardModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            return msg;
        }
        public ActionResult ExistingSBICard(int CardID = 0, int ProjectId = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Gender = Common.getGender();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                var projectdetails = Common.GetPIdetailsbyProject(ProjectId);
                SBIECardModel model = new SBIECardModel();
                model.NameofPI = projectdetails.PIName;
                model.PIDepartmentName = projectdetails.PIDepartment;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (ProjectId == 0 && CardID == 0)
                {
                    return RedirectToAction("SBIECardList", "CoreAccounts");
                }
                string validationMsg = Common.ValidateSBICardPjctAddition(ProjectId, CardID);
                if (validationMsg != "Valid")
                {
                    ViewBag.errMsg = validationMsg;
                    return View(model);
                }

                model = coreAccountService.GetCardandPjctDetailsbyID(CardID, ProjectId);
                model.CreditorType = "PI";
                model.NeedUpdateTransDetail = true;
                model.CheckListDetail = Common.GetCheckedList(39);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadSBIEcardList()
        {
            try
            {
                object output = coreAccountService.GetSBIEcardList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProject(string PIUserId)
        {
            try
            {
                PIUserId = PIUserId == "" ? "0" : PIUserId;
                var locationdata = coreAccountService.GetProjectList(Convert.ToInt32(PIUserId));
                return Json(locationdata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult LoadProjectCardDetails(string ProjectID)
        {
            try
            {
                ProjectID = ProjectID == "" ? "0" : ProjectID;
                object output = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchSBIECardList(SBIECardSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchSBICardList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchSBICardRecoupmentList(SBIECardSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchSBICardRecoupmentList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult SBICardView(int CardProjectId = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.EditProjectCardDetails(CardProjectId);
                decimal amount = model.TotalValueofCard ?? 0;
                TempData["viewMode"] = "ViewOnly";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(39, "Others", amount);
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        public ActionResult ExistingPIDetails(int PIID = 0, int ProjectId = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Gender = Common.getGender();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                SBIECardModel model = new SBIECardModel();
                model = coreAccountService.GetCardPIDetailsbyID(PIID, ProjectId);
                return View("ExistingSBICard", model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult SBIECardProjectApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.SBIECardProjectApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SBIECardProjectWFInit(int BillId)
        {
            try
            {
                lock (SBIBilllockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateSBICardStatus(BillId, "Open"))
                    {
                        var transCode = "ECD";
                        bool status = coreAccountService.SBICardWFInit(BillId, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion     
        #region SBI Bill Booking
        [HttpGet]
        public JsonResult GetEcardRecoupmentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetEcardRecoupmentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult SBIECardRecoupmentList()
        {
            return View();
        }
        public ActionResult SBIECardRecoupment(int SBICardRecoupId = 0, int SBICardProjectDetailsId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                model.SBIEcardProjectDetailsId = SBICardProjectDetailsId;

                if (SBICardRecoupId > 0)
                {
                    model = coreAccountService.GetSBIECardRecoupmentDetails(SBICardRecoupId);

                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                {
                    model = coreAccountService.GetSBIECardDetails(SBICardProjectDetailsId);
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(46);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        public ActionResult SBIECardRecoupmentView(int SBICardRecoupId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                if (SBICardRecoupId > 0)
                {
                    model = coreAccountService.GetSBIECardRecoupmentDetails(SBICardRecoupId);

                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                decimal amount = model.RecoupmentValue ?? 0;
                var type = model.ProjectCategory;
                TempData["viewMode"] = "ViewOnly";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(46, type, amount);
                ViewBag.disabled = "disabled";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult SBIECardRecoupment(SBIECardModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                // ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSBIECardRecoupmentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SBIECardRecoupment(model, logged_in_user);
                    if (model.RecoupmentId == 0 && result > 0)
                    {
                        ViewBag.succMsg = "Recoupment has been added successfully.";

                    }
                    else if (model.RecoupmentId > 0 && result > 0)
                    {
                        ViewBag.succMsg = "Recoupment has been updated successfully.";
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;

                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateSBIECardRecoupmentBillPayment(SBIECardModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;
            decimal balanceincard = model.PendingBillsRecoupValue ?? 0;
            decimal currrecoupvalue = model.RecoupmentValue ?? 0;
            decimal diff = (model.NetPayableValue - currrecoupvalue) ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (diff > balanceincard)
            {
                msg = msg == "Valid" ? "Total Bill amount cannot be greater than available imprest value" : msg + "<br /> Total Bill amount cannot be greater than available imprest value";
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;            
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt < commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpPost]
        public ActionResult SBIECardRecoupmentApprove(int recoupmentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.SBIECardRecoupmentBalanceUpdate(recoupmentId, false, false, userId, "ECR");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.SBIECardRecoupmentBillApproved(recoupmentId, userId);
                if (!status)
                    coreAccountService.SBIECardRecoupmentBalanceUpdate(recoupmentId, true, false, userId, "ECR");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SBIECardBillWFInit(int BillId)
        {
            try
            {
                lock (SBICardBilllockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateSBICardBillStatus(BillId, "Open"))
                    {
                        var transCode = "ECR";

                        bool cStatus = coreAccountService.SBICardBillCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.SBICardBillWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.SBICardBillCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpGet]
        //public JsonResult GetSBICardTransactionDetails(string typeCode, int deductionCategoryId = 0, string tSubCode = "1")
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetSBICardTransactionDetails(deductionCategoryId, typeCode, tSubCode);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        #endregion
        #region SBI Card Recoupment
        public ActionResult SBICardBillsRecoupList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ApproveSBICardBillsRecoup(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveSBICardBillRecoupment(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult SBICardBillsRecoupment(int id = 0, int BillRecoupid = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                SBIECardBillRecoupModel model = new SBIECardBillRecoupModel();
                if (id == 0 && BillRecoupid > 0 && Common.ValidateSBICardBillRecoupStatus(BillRecoupid, "Open"))
                {
                    model = coreAccountService.GetSBICardBillRecoupDetails(BillRecoupid);

                }
                if (id > 0 && BillRecoupid == 0 && Common.ValidateSBICardBillStatus(id, "Approved"))
                {
                    model = coreAccountService.GetSBICardBillDetails(id);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult SBICardBillsRecoupment(SBIECardBillRecoupModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();

                foreach (var item in model.CrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                foreach (var item in model.DrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSBICardBillsRecoupment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SBICardBillRecoupIU(model, logged_in_user);
                    if (model.SBIECardBillRecoupId == null && result > 0)
                    {
                        TempData["succMsg"] = "SBI Card Bills Recoupment has been added successfully.";
                        return RedirectToAction("SBICardBillsRecoupList");
                    }
                    else if (model.SBIECardBillRecoupId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "SBI Card Bills Recoupment has been updated successfully.";
                        return RedirectToAction("SBICardBillsRecoupList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =

                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        public ActionResult SBICardBillsRecoupmentView(int id)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ViewBag.disabled = "disabled";

                SBIECardBillRecoupModel model = new SBIECardBillRecoupModel();
                model = coreAccountService.GetSBICardBillRecoupDetails(id);
                decimal amount = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(126, "Others", amount);

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateSBICardBillsRecoupment(SBIECardBillRecoupModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.AccountHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.AccountHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";
            foreach (var item in model.CrDetail)
            {
                int headId = item.AccountHeadId ?? 0;
                decimal balAmt = Common.GetBankClosingBalance(headId);
                if (balAmt < item.Amount)
                {
                    msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
                    break;
                }
            }
            return msg;
        }
        [HttpPost]
        public JsonResult LoadSBICardBillRecoupmentList(SearchSBIECardMaster model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetSBICardBillRecoupList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SBICardBillRecoupmentWFInit(int BillId)
        {
            try
            {
                lock (SBICardRecBilllockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateSBICardBillRecoupStatus(BillId, "Open"))
                    {
                        var transCode = "ECBR";

                        bool status = coreAccountService.SBICardBillRecoupWFInit(BillId, userId, transCode);

                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Temporary

        #region Advance
        public ActionResult TemporaryAdvancePayment(int tmpadvanceId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(40);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model.CreditorType = "PI";
                if (tmpadvanceId > 0 && Common.ValidateTempAdvBillOnEdit(tmpadvanceId, "TMP"))
                {
                    model = coreAccountService.GetTemporaryAdvanceDetails(tmpadvanceId);

                }
                else
                    model.CheckListDetail = Common.GetCheckedList(40);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }



        public ActionResult TemporaryAdvancePaymentView(int tmpadvanceId, bool Pfinit = false)
        {

            try
            {

                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model.CreditorType = "PI";
                model = coreAccountService.GetTemporaryAdvanceDetailsView(tmpadvanceId);
                decimal amount = model.TemporaryAdvanceValue ?? 0;
                //var pGId = Common.GetTMPprocessGuideLineId(model.TemporaryAdvanceId, Advvalue);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";
                //ViewBag.processGuideLineId = pGId;
                var type = model.ProjectCategory;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(40, "Others", amount);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        [HttpPost]
        public ActionResult TemporaryAdvancePayment(TemporaryAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(41);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTempAdvancePayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateTemporaryAdvance(model, logged_in_user);
                    if (model.TemporaryAdvanceId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Temporary Advance has been added successfully.";
                        return RedirectToAction("TemporaryAdvancePaymentList");
                    }
                    else if (model.TemporaryAdvanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Temporary Advance has been updated successfully.";
                        return RedirectToAction("TemporaryAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateTempAdvancePayment(TemporaryAdvanceModel model)
        {
            //decimal ttlAdvAmt = 0;
            //string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        public ActionResult TemporaryAdvancePaymentList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetTemporaryAdvanceList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTemporaryAdvanceList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult TemporayAdvanceApprove(string tempAdvId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.TempAdvCommitmentBalanceUpdate(Convert.ToInt32(tempAdvId), false, false, userId, "TMP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.TempAdvApprove(Convert.ToInt32(tempAdvId), userId);
                if (!status)
                    coreAccountService.TempAdvCommitmentBalanceUpdate(Convert.ToInt32(tempAdvId), true, false, userId, "TMP");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIProject(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = Common.getProjectListofPI(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIDetails(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = Common.getProjectListofPI(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectDetails(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = Common.GetProjectsDetails(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAdvanceDetails(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = Common.getprojectadvancedetails(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTemporaryADVCommitmentDetails(int temporaryAdvanceId)
        {
            var data = coreAccountService.GetTemporaryADVCommitmentDetails(temporaryAdvanceId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTempAdvTransactionDetails(string typeCode, int deductionCategoryId = 0, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetTempAdvTransactionDetails(deductionCategoryId, typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PendingSettlementDetails(int Projectid)
        {
            TemporaryAdvanceModel model = new TemporaryAdvanceModel();
            model = coreAccountService.GetPendingAdvanceDetails(Projectid);
            return PartialView(model);
        }
        [HttpGet]
        public JsonResult SearchTemporaryAdvanceList(TempAdvSearchFieldModel model)
        {
            object output = coreAccountService.SearchTempAdvList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult GetTemporaryAdvanceList(string typeCode, TemporaryAdvSearchModel model, int pageIndex, int pageSize, DateFilterModel RequestedDate)
        {
            try
            {
                object output = coreAccountService.GetTemporaryAdvanceList(typeCode, model, pageIndex, pageSize, RequestedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult TemporaryAdvanceWFInit(int BillId)
        {
            try
            {
                lock (TMPadvlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateTempAdvStatus(BillId, "Open"))
                    {
                        var transCode = "TMP";
                        bool cStatus = coreAccountService.TempAdvCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TempAdvanceWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.TempAdvCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Settlement
        public ActionResult TemporaryAdvanceSettlementList()
        {
            return View();
        }

        public ActionResult TemporaryAdvanceSettlement(int TempAdvId = 0, int TempAdvsettlId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                if (TempAdvId > 0 && TempAdvsettlId == 0)
                {
                    string validationMsg = Common.ValidateTempAdvforSettlement(TempAdvId);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return RedirectToAction("TemporaryAdvanceSettlementList", "CoreAccounts");
                    }
                    model = coreAccountService.GetTempAdvanceDetails(TempAdvId);
                    model.NeedUpdateTransDetail = true;
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                if (TempAdvsettlId > 0 && Common.ValidateTempAdvSettlementStatus(TempAdvsettlId, "TMS", "Open"))
                {
                    model = coreAccountService.GetTemporaryAdvanceSettlDetails(TempAdvsettlId);

                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(47);

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        public ActionResult TemporaryAdvanceSettlementView(int TempAdvsettlId, bool Pfinit = false)
        {
            try
            {

                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model = coreAccountService.GetTemporaryAdvanceSettlDetailsView(TempAdvsettlId);

                model.PFInit = Pfinit;

                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(47, "Others", model.TotalExpenseValue ?? 0);

                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult TemporaryAdvanceSettlement(TemporaryAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                // ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                // ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (model.InvoiceBUTotal == model.TemporaryAdvanceValue)
                {
                    for (int i = 0; i < model.ExpenseDetail.Count(); i++)
                    {
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
                        ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
                        ModelState.Remove("ExpenseDetail[" + i + "].Amount");
                        ModelState.Remove("ExpenseDetail[" + i + "].IsJV");
                    }
                }
                decimal ttlInvWOGSTElg = Convert.ToDecimal(model.InvoiceBUTotal) - Convert.ToDecimal(model.GSTOffsetTotal);
                ttlInvWOGSTElg = Math.Round(ttlInvWOGSTElg, 2, MidpointRounding.AwayFromZero);
                if (ttlInvWOGSTElg == model.TemporaryAdvanceValue)
                {
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                }
                ModelState.Remove("AmountofItem");
                ModelState.Remove("Particulars");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTempSettlementBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TemporaryAdvanceSettlement(model, logged_in_user);
                    if (model.TempAdvSettlId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been added successfully.";
                        return RedirectToAction("TemporaryAdvanceSettlementList");
                    }
                    else if (model.TempAdvSettlId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been updated successfully.";
                        return RedirectToAction("TemporaryAdvanceSettlementList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        [HttpGet]
        public JsonResult GetTempAdvSettlList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTempAdvSettlList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult TemporaryAdvanceSettlementApproved(int id)
        {
            try
            {
                lock (TMPSettlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateTempAdvSettlementStatus(id, "TMS", "Open"))
                    {
                        bool cStatus = coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, false, userId, "TMS");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TempAdvSettlementApproved(id, userId);
                        if (!status)
                            coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, true, userId, "TMS");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult TemporaryAdvanceSettlementWFInit(int id)
        {
            try
            {
                lock (TMPSettWFInitlockObj)
                {
                    if (Common.ValidateTempAdvSettlementStatus(id, "TMS", "Open"))
                    {
                        int logged_in_user = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, false, logged_in_user, "TMS");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.TempAdvSettlWFInit(id, logged_in_user);
                        if (!result.Item1)
                            coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, true, logged_in_user, "TMS");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        private string ValidateTempSettlementBillPayment(TemporaryAdvanceModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlInvWOGSTElg = Convert.ToDecimal(model.InvoiceBUTotal) - Convert.ToDecimal(model.GSTOffsetTotal);
            ttlInvWOGSTElg = Math.Round(ttlInvWOGSTElg, 2, MidpointRounding.AwayFromZero);
            //decimal billAmt = model.AmountofItem.Sum() ?? 0;
            decimal advAmt = model.TemporaryAdvanceValue ?? 0;
            decimal Amt = 0;

            Amt = model.CommitmentDetail.Sum(m => m.ReversedAmount) ?? 0;
            if (Amt > 0)
                Amt = Amt;
            else
                Amt = model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0;
            if (model.CommitmentAmount != Amt)
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (ttlInvWOGSTElg > advAmt)
            {
                decimal paymentVal = ttlInvWOGSTElg - advAmt;
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (paymentVal != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value.Please update the value to continue.";

            }
            else if (ttlInvWOGSTElg < advAmt)
            {
                decimal reversedVal = advAmt - ttlInvWOGSTElg;
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.ReversedAmount > item.PaymentAmount)
                        msg = msg == "Valid" ? "Commitment reversed value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (reversedVal != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue.";

            }
            else
            {
                if (0 != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";

            }
            if (model.InvoiceBUTotal > advAmt && crAmt != model.InvoiceBUTotal)
                msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            else if (model.InvoiceBUTotal < advAmt && crAmt != advAmt)
                msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            else if (model.InvoiceBUTotal == advAmt && crAmt != advAmt)
                msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != crAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);

            return msg;

        }
        [HttpPost]
        public JsonResult GetTempAdvSettlList(string typeCode, TemporaryAdvSearchModel model, int pageIndex, int pageSize, DateFilterModel RequestedDate)
        {
            try
            {
                object output = coreAccountService.GetTempAdvSettlList(typeCode, model, pageIndex, pageSize, RequestedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #endregion
        #region SummerInternship 
        public ActionResult SummerInternshipStudentList()
        {
            return View();
        }
        public ActionResult SummerInternshipStudent(int internId = 0)
        {
            try
            {
                var emptyList = new List<SummerInternshipModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(44);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SummerInternshipModel model = new SummerInternshipModel();
                model.CreditorType = "Student";
                if (internId > 0 && Common.ValidateSummerInternshipOnEdit(internId))
                {
                    model = coreAccountService.GetSummerInternshipDetails(internId);

                }
                else
                    model.CheckListDetail = Common.GetCheckedList(44);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpGet]
        public ActionResult SummerInternshipApprove(int InternId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.SummerInternshipBalanceUpdate(InternId, false, false, userId, "SMI");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.SummerInternshipBillApproved(InternId, userId);
                if (!status)
                    coreAccountService.SummerInternshipBalanceUpdate(InternId, true, false, userId, "SMI");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SummerInternshipStudentView(int internId, bool Pfinit = false)
        {

            try
            {

                SummerInternshipModel model = new SummerInternshipModel();
                model = coreAccountService.GetSummerInternshipDetailsView(internId);
                decimal amount = model.TotalStipendValue ?? 0;
                var type = model.ProjectCategory;
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(44, type, amount);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult SummerInternshipStudent(SummerInternshipModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(44);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSummerInternship(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateSummerInternship(model, logged_in_user);
                    if (model.SummrInternStudentId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Summer Internship Student has been added successfully.";
                        return RedirectToAction("SummerInternshipStudentList");
                    }
                    else if (model.SummrInternStudentId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Summer Internship Student has been updated successfully.";
                        return RedirectToAction("SummerInternshipStudentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateSummerInternship(SummerInternshipModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public JsonResult GetSummerInternshipStudentList()
        {
            try
            {
                object output = coreAccountService.GetSummerInternshipStudentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SearchSummerInternshipList(int pageIndex, int pageSize, SearchSummerInternshipModel model)
        {
            try
            {
                object output = coreAccountService.SearchSummerInternshipList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult SummerInternshipWFInit(int BillId)
        {
            try
            {
                lock (SummerInWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateSummerInternshipStatus(BillId, "Open"))
                    {
                        var transCode = "SMI";
                        bool cStatus = coreAccountService.SummerInternshipBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.SummerInternshipWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.SummerInternshipBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region PartTime 
        public ActionResult PartTimeStudentList()
        {
            return View();
        }
        public ActionResult PartTimeStudent(int internId = 0)
        {
            try
            {
                var emptyList = new List<PartTimePaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.SessionList = Common.GetCodeControlList("Session");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(56);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                PartTimePaymentModel model = new PartTimePaymentModel();
                model.CreditorType = "PI";
                if (internId > 0 && Common.ValidatePartTimePaymentOnEdit(internId))
                {
                    model = coreAccountService.GetPartTimePaymentDetails(internId);

                }
                else
                    model.CheckListDetail = Common.GetCheckedList(56);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }


        public ActionResult PartTimeStudentView(int internId, bool Pfinit = false)
        {

            try
            {


                PartTimePaymentModel model = new PartTimePaymentModel();

                model = coreAccountService.GetPartTimePaymentDetailsView(internId);

                ViewBag.disabled = "Disabled";

                decimal amount = model.TotalStipendValue ?? 0;
                var type = model.ProjectCategory;
                TempData["viewMode"] = "ViewOnly";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(56, type, amount);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult PartTimeStudent(PartTimePaymentModel model)
        {
            try
            {
                var emptyList = new List<PartTimePaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.SessionList = Common.GetCodeControlList("Session");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(56);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidatePartTimeStudent(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    foreach (var item in model.StudentDetails)
                    {
                        if (item.Name != null)
                        {
                            var stipendval = item.StipendValueperHour;
                            var session = item.Session;
                            var hrs = item.Duration;
                            if ((session == 1 || session == 2) && (stipendval < 100 || stipendval > 1000))
                            {
                                TempData["errMsg"] = "Stipend value cannot be less than Rs 100 or greater than Rs 1000";
                                return View(model);
                            }
                            if (session == 1 && hrs > 40)
                            {
                                TempData["errMsg"] = "Permited working hrs for Academic session is 40 hrs only.";
                                return View(model);
                            }
                            if (session == 2 && hrs > 160)
                            {
                                TempData["errMsg"] = "Permited working hrs for Non Academic session is 160 hrs only.";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreatePartTimePayment(model, logged_in_user);
                    if (model.PartTimePaymentId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Time Student has been added successfully.";
                        return RedirectToAction("PartTimeStudentList");
                    }
                    else if (model.PartTimePaymentId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Time Student has been updated successfully.";
                        return RedirectToAction("PartTimeStudentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidatePartTimeStudent(PartTimePaymentModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public JsonResult GetPartTimeStudentList()
        {
            try
            {
                object output = coreAccountService.GetPartTimeStudentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchPartTimePaymentList(PartTimePaymentSearchFieldModel model)
        {
            object output = coreAccountService.SearchPartTimePaymentList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPartTimePaymentTransactionDetails(string typeCode, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetPartTimePaymentTransactionDetails(typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadStudentByDepartment(string Departmentname)
        {
            var locationdata = coreAccountService.getStudentListbyDepartment(Departmentname);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadStudentDetails(string RollNo)
        {
            var locationdata = coreAccountService.getStudentDetails(RollNo);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PartPaymentApprove(int paymentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.PartPaymentBalanceUpdate(paymentId, false, false, userId, "PTP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.PartPaymentBillApproved(paymentId, userId);
                if (!status)
                    coreAccountService.PartPaymentBalanceUpdate(paymentId, true, false, userId, "PTP");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SearchPartTimePaymentList(int pageIndex, int pageSize, SearchPartTimePaymentModel model)
        {
            try
            {
                object output = coreAccountService.SearchPartTimePaymentList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult PartTimePaymentWFInit(int BillId)
        {
            try
            {
                lock (PartTimeWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateParttimepaymentStatus(BillId, "Open"))
                    {
                        var transCode = "PTP";
                        bool cStatus = coreAccountService.PartPaymentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.PartPaymentWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.PartPaymentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ClearancePayment
        public ActionResult ClearancePayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ClearanceAgentList = Common.GetClearanceAgentList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetSettlementTypeList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ClearancePaymentEntryModel model = new ClearancePaymentEntryModel();
                if (billId > 0 && Common.ValidateClearancePaymentOnEdit(billId, "CLP"))
                {
                    model = coreAccountService.GetClearancePaymentDetails(billId);

                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                    ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                    ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(45);
                model.CreditorType = "Clearance Agent";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }
        public ActionResult ClearancePaymentView(int billId, bool Pfinit = false)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;

                ClearancePaymentEntryModel model = new ClearancePaymentEntryModel();
                model = coreAccountService.GetClearancePaymentDetailsView(billId);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                //ViewBag.disabled = "Disabled";
                decimal amt = model.BillAmount + model.BillTaxAmount ?? 0;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(45, "Others", amt);
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                TempData["viewMode"] = "ViewOnly";
                model.CreditorType = "Clearance Agent";
                model.PFInit = Pfinit;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult ClearancePayment(ClearancePaymentEntryModel model)
        {
            try
            {
                model.CreditorType = "Clearance Agent";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ClearanceAgentList = Common.GetClearanceAgentList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetSettlementTypeList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();

                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateClearancePayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ClearancePaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Clearance Payment Bill has been added successfully.";
                        return RedirectToAction("ClearancePaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Clearance Payment Bill has been updated successfully.";
                        return RedirectToAction("ClearancePaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Clearance Payment Bill already exists for this PO Number with the Clearance Agent.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();

                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        [HttpPost]
        public JsonResult ValidateDuplicateCLPBillInvoiceNumber(int BillId, int ClearanceAgentId, string invNo)
        {
            string msg = "Valid";

            using (var context = new IOASDBEntities())
            {
                try
                {
                    bool result = Common.CheckIsExistsInvoiceNo(BillId, ClearanceAgentId, invNo);
                    if (result)
                        msg = "Duplicate Invoice Number found";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }

        }
        public ActionResult ClearancePaymentList()
        {
            return View();

        }

        private string ValidateClearancePayment(ClearancePaymentEntryModel model)
        {
            decimal netAdvAmt = 0, ttlGSTElgAmt = 0;
            string msg = "Valid";
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlInvGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlInvGSTElg = Math.Round(ttlInvGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = item.TotalAmount ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            //netAdvAmt = Math.Round(netAdvAmt, 2, MidpointRounding.AwayFromZero);
            ttlGSTElgAmt = Math.Round(ttlGSTElgAmt, 2, MidpointRounding.AwayFromZero);
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal netCrAmt = model.CreditorAmount ?? 0;
            if (paymentBUAmt > Math.Ceiling(netAdvAmt) || paymentBUAmt < Math.Floor(netAdvAmt))
                msg = "Not a valid entry.The Payable value and Payment Break Up Total value are not equal.";
            if (ttlGSTElgAmt != ttlInvGSTElg)
                msg = msg == "Valid" ? "Not a valid entry. The PO tax eligible value and invoice tax eligible value are not equal." : msg + "<br /> Not a valid entry. The PO tax eligible value and invoice tax eligible value are not equal.";
            if ((paymentBUAmt - ttlInvGSTElg) != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the bill value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the bill value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVDrVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            decimal ttlJVDr = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVCr = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            ttlJVDr += model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlJVDr != ttlJVCr)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (netCrAmt != paymentBUAmt)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and payable value. Please update the value to continue." : msg + "<br /> There is a mismatch between the credit value and payable value. Please update the value to continue.";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";

            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            return msg;
        }
        [HttpPost]
        public JsonResult GetClearancePaymentList(string typeCode, ClearancePaymentSearchModel model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetClearancePaymentList(typeCode, model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ClearancePaymentApprove(string CLPId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), false, false, userId, "CLP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ClearancePaymentApprove(Convert.ToInt32(CLPId), userId);
                if (!status)
                    coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), true, false, userId, "CLP");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ClearancePaymentWFInit(int CLPId)
        {
            try
            {
                lock (CLPWFInitlockObj)
                {
                    if (Common.ValidateClearancePaymentOnEdit(CLPId, "CLP"))
                    {

                        int userId = Common.GetUserid(User.Identity.Name);

                        bool cStatus = coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), false, false, userId, "CLP");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.ClearancePaymentWFInit(CLPId, userId);
                        if (!status)
                            coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), true, false, userId, "CLP");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetClearanceAgentDetails(int agentId, bool poNumberRequired = false, string transTypeCode = "", bool TDSRequired = false)
        {
            try
            {
                var output = coreAccountService.GetClearanceAgentDetails(agentId);
                if (poNumberRequired)
                    output.PONumberList = Common.GetClearancePaymentPONumberList(agentId, transTypeCode);
                if (TDSRequired)
                    output.TDSList = Common.GetClearanceAgentTDSList(agentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PreviousCLPBillHistory(int agentId)
        {
            try
            {
                ViewBag.data = coreAccountService.GetCLPBillHistoryList(agentId);
                return PartialView();
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception();
            }
        }
        public ActionResult _CLPCommitment()
        {
            CommitmentModel model = new CommitmentModel();
            ViewBag.CommitmentType = Common.getCommitmentType();
            ViewBag.Purpose = Common.getPurpose();
            ViewBag.Currency = Common.getCurrency();
            ViewBag.BudgetHead = Common.getBudgetHead();
            ViewBag.ProjectType = Common.getprojecttype();
            //   ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.AccountGroup = Common.getAccountGroup();
            ViewBag.AccountHead = Common.getBudgetHead();
            ViewBag.ClearanceAgent = Common.getClearanceAgent();
            //model = coreAccountService.GetTempAdvDetailsForCommitment(temporaryAdvanceId);
            //if (model.selRequestRefrence == 0)
            //    throw new Exception();
            // model.TemporaryAdvanceId = temporaryAdvanceId;
            model.CommitmentNo = "0";
            model.commitmentValue = 0;
            model.currencyRate = 0;
            return PartialView(model);

        }


        [HttpGet]
        public JsonResult GetCLPServiceTypeDetail(int serviceType)
        {
            try
            {
                var data = Common.GetCLPServiceDetail(serviceType);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult _CLPSaveCommitment(CommitmentModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            AccountService _AS = new AccountService();
            var output = _AS.SaveCommitDetails(model, UserId, true);
            // object output = coreAccountService.SearchSummerInternshipList(model);
            //object output = "";
            return Json(output.Item1, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadCommitmentList(Int32[] CommitmentId)
        {
            try
            {
                CommitmentId = CommitmentId.Distinct().ToArray();
                object output = Common.GetCommitmentlistbyId(CommitmentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        //public JsonResult LoadCommitmentList(Int32[] CommitmentId)
        //{
        //    CommitmentId = CommitmentId.Distinct().ToArray();
        //    var projectData = Common.GetCommitmentlistbyId(CommitmentId);
        //    return Json(projectData, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region BankImprest 
        #region Imprest Payment
        public ActionResult ImprestPaymentList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }
        public ActionResult ImprestPayment(int PIID = 0, int ImpID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.TotalValueofCard = 0;
                model.CurrentImprestValue = 0;
                model.PIId = PIID;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (ImpID > 0 && Common.ValidateImprestOnEdit(ImpID, "IMP"))
                {

                    model = coreAccountService.EditImprestPaymentDetails(ImpID);
                    model.CreditorType = "PI";
                }
                else if (PIID > 0 && ImpID == 0)
                {
                    string validationMsg = Common.ValidateImprestonAddition(PIID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                else if (PIID == 0 && ImpID == 0)
                {
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ImprestPaymentModel model = new ImprestPaymentModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult LoadImprestPaymentList(SearchImpresrtMaster model, int pageIndex, int pageSize)
        {
            try
            {
                object output = coreAccountService.GetImprestPaymentList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult ImprestPayment(ImprestPaymentModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateIMPaymentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }

                    var ImprestCardID = _ps.CreateImprestPayment(model, loggedinuserid);
                    if (ImprestCardID > 0)
                    {
                        var CardNumber = Common.getImprestcardnumber(ImprestCardID);
                        ViewBag.succMsg = "Imprest payment added to Imprest Card with Card number - " + CardNumber + ".";
                    }
                    else if (ImprestCardID == -2)
                    {
                        var CardNumber = Common.getImprestcardnumber(model.ImprestcardId);
                        ViewBag.succMsg = CardNumber + " - Imprest Payment details updated.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }
        private string ValidateIMPaymentBillPayment(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            var amt = model.ImprestValue;
            var sancamt = model.TotalProjectsValue;
            var previmptotal = model.TotalPrevImprestValue;
            var imprestpercent = (sancamt * 20) / 100;
            decimal? balance = 0;
            //if (imprestpercent < 500000)
            //{
            //    balance = imprestpercent - previmptotal;
            //    if (balance < amt)
            //    {
            //        msg = "Imprest amount claimed cannot be greater than 20% of the total projects value.";
            //    }
            //}
            //if (imprestpercent > 500000)
            //{
            //    balance = 500000 - previmptotal;
            //    if (balance < amt)
            //    {
            //        msg = "Imprest amount claimed cannot be greater than Rs 500000.";
            //    }
            //}

            return msg;
        }

        [HttpGet]
        public JsonResult LoadImprestPaymentList()
        {
            try
            {
                object output = coreAccountService.GetImprestPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult LoadImprestProjectDetails(string PIID)
        {
            try
            {
                PIID = PIID == "" ? "0" : PIID;
                object output = coreAccountService.GetProjectdetailsbyPI(Convert.ToInt32(PIID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchImprestPaymentList(ImprestPaymentSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchImprestPaymentList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        public ActionResult ImprestPaymentView(int ImpID = 0, bool Pfinit = false)
        {
            try
            {

                ImprestPaymentModel model = new ImprestPaymentModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.ViewImprestPaymentDetails(ImpID);
                ViewBag.disabled = "Disabled";
                decimal amount = model.ImprestValue ?? 0;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(50, "Others", amount);
                //ViewBag.processGuideLineId = 30;
                model.CreditorType = "PI";
                TempData["viewMode"] = "ViewOnly";
                model.PFInit = Pfinit;
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpGet]
        public JsonResult ImprestPaymentProjectApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.ImprestPaymentApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ImprestPaymentWFInit(int BillId)
        {
            try
            {
                lock (ImprestWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateImprestPaymentStatus(BillId, "Open"))
                    {
                        var transCode = "IMP";
                        //bool cStatus = coreAccountService.SummerInternshipBalanceUpdate(BillId, false, false, userId, transCode);
                        //if (!cStatus)
                        //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.ImprestPaymentWFInit(BillId, userId, transCode);
                        //if (!status)
                        //    coreAccountService.SummerInternshipBalanceUpdate(BillId, true, true, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetImprestLedgerId(string ImprestACNumber)
        {
            try
            {
                object output = Common.GetImprestLedgerId(ImprestACNumber);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Imprest Enhancement
        public ActionResult ImprestEnhancementList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }
        public ActionResult ImprestEnhancement(int PIID = 0, int IMEID = 0, int ImpID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.TotalValueofCard = 0;
                model.CurrentImprestValue = 0;
                model.PIId = PIID;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (PIID > 0 && IMEID > 0 && Common.ValidateImprestEnhanceOnEdit(IMEID, "IME"))
                {


                    model = coreAccountService.EditImprestEnhanceDetails(IMEID);
                    model.CreditorType = "PI";
                }
                else if (PIID > 0 && IMEID == 0 && ImpID > 0)
                {
                    string validationMsg = Common.ValidateImprestonAddition(PIID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model = coreAccountService.GetIMPEnhancedetailsbyPI(PIID, ImpID);
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                else if (PIID == 0 && IMEID == 0)
                {
                    TempData["errMsg"] = "Imprest Account does not exist.";
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                    return View(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ImprestPaymentModel model = new ImprestPaymentModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult ImprestEnhancement(ImprestPaymentModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateIMPaymentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }

                    var ImprestCardID = _ps.CreateImprestEnhance(model, loggedinuserid);
                    if (ImprestCardID > 0)
                    {
                        var CardNumber = Common.getImprestcardnumber(ImprestCardID);
                        ViewBag.succMsg = "Imprest Enhancement done successfully";
                    }
                    else if (ImprestCardID == -2)
                    {
                        var CardNumber = Common.getImprestcardnumber(model.ImprestcardId);
                        ViewBag.succMsg = "Imprest Enhancement updated successfully.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }
        private string ValidateImpEnhanceBill(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            var amt = model.ImprestValue;
            var sancamt = model.TotalProjectsValue;
            var previmptotal = model.TotalPrevImprestValue;
            var imprestpercent = (sancamt * 20) / 100;
            decimal? balance = 0;
            if (imprestpercent < 500000)
            {
                balance = imprestpercent - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than 20% of the total projects value.";
                }
            }
            if (imprestpercent > 500000)
            {
                balance = 500000 - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than Rs 500000.";
                }
            }

            return msg;
        }
        [HttpGet]
        public JsonResult LoadImprestEnhanceList()
        {
            try
            {
                object output = coreAccountService.GetImprestEnhancementList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult LoadImprestEnhanceDetails(string PIID)
        {
            try
            {
                PIID = PIID == "" ? "0" : PIID;
                object output = coreAccountService.GetProjectdetailsbyPI(Convert.ToInt32(PIID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult SearchImprestEnhanceList(SearchImpresrtMaster model, int pageIndex, int pageSize, DateFilterModel RequestedDate)
        {
            try
            {
                object output = coreAccountService.SearchImprestEnhanceList(model, pageIndex, pageSize, RequestedDate);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ImprestEnhancementView(int IMEID = 0, bool Pfinit = false)
        {
            try
            {

                ImprestPaymentModel model = new ImprestPaymentModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.ViewImprestEnhanceDetails(IMEID);
                //ViewBag.disabled = "Disabled";
                model.PFInit = Pfinit;
                model.CreditorType = "PI";
                TempData["viewMode"] = "ViewOnly";
                decimal amount = model.ImprestValue ?? 0;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(72, "Others", amount);

                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpGet]
        public JsonResult ImprestEnhanceApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.ImprestEnhancementApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ImprestEnhanceWFInit(int BillId)
        {
            try
            {
                lock (ImprestEnhanWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateImprestEnhanceStatus(BillId, "Open"))
                    {
                        var transCode = "IME";
                        bool status = coreAccountService.ImprestEnhanceWFInit(BillId, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetImprestMasterTransactionDetails(int IMPmasterId)
        {
            try
            {
                object output = coreAccountService.GetImprestMasterTransactionDetails(IMPmasterId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region Imprest Recoupment
        [HttpGet]
        public JsonResult GetImprestRecoupmentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetImprestRecoupmentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ImprestPaymentRecoupmentList()
        {
            return View();
        }
        public ActionResult ImprestPaymentRecoupment(int ImprestRecoupId = 0, int ImprestCardId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(49);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.ImprestcardId = ImprestCardId;

                if (ImprestRecoupId > 0)
                {
                    model = coreAccountService.GetImprestRecoupmentDetails(ImprestRecoupId);

                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                {
                    model = coreAccountService.GetImprestCardDetails(ImprestCardId);
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(49);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }

        }

        public ActionResult ImprestRecoupmentView(int ImprestRecoupId = 0, bool Pfinit = false)
        {
            try
            {

                ImprestPaymentModel model = new ImprestPaymentModel();
                if (ImprestRecoupId > 0)
                {
                    model = coreAccountService.GetImprestRecoupmentDetailsView(ImprestRecoupId);
                }
                //ViewBag.disabled = "Disabled";
                TempData["viewMode"] = "ViewOnly";
                decimal amount = model.RecoupmentValue ?? 0;
                var type = model.ProjectCategory;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(49, type, amount);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImprestPaymentRecoupment(ImprestPaymentModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;

                //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(49);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                // ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateImprestRecoupmentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ImprestRecoupment(model, logged_in_user);
                    if (model.RecoupmentId == 0 && result > 0)
                    {
                        ViewBag.succMsg = "Imprest Bill Booking done successfully.";

                    }
                    else if (model.RecoupmentId > 0 && result > 0)
                    {
                        ViewBag.succMsg = "Imprest Bill Booking has been updated successfully.";
                    }
                    //else if (result == -2)
                    //    TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                // ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;


                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateImprestRecoupmentBillPayment(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;
            decimal balanceinimp = model.PendingBillsRecoupValue ?? 0;
            decimal currrecoupvalue = model.RecoupmentValue ?? 0;
            decimal billbookedtotal = (model.NetPayableValue - currrecoupvalue) ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (billbookedtotal > balanceinimp)
            {
                msg = msg == "Valid" ? "Total Bill amount cannot be greater than available imprest value" : msg + "<br /> Total Bill amount cannot be greater than available imprest value";
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;            
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt < commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public ActionResult ImprestRecoupmentApprove(int recoupmentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.ImprestRecoupmentBalanceUpdate(recoupmentId, false, false, userId, "IMR");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ImprestRecoupmentBillApproved(recoupmentId, userId);
                if (!status)
                    coreAccountService.ImprestRecoupmentBalanceUpdate(recoupmentId, true, false, userId, "IMR");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetPIdetailsbyProject(string PIId)
        {
            try
            {
                object output = Common.GetPIdetailsbyProject(Convert.ToInt32(PIId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult SearchImprestBillBookingList(SearchImpresrtMaster model, int pageIndex, int pageSize, DateFilterModel RequestedDate)
        {
            try
            {
                object output = coreAccountService.SearchImprestBillBookingList(model, pageIndex, pageSize, RequestedDate);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult ImprestBillBookingWFInit(int BillId)
        {
            try
            {
                lock (ImprestBillWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateImprestBillBookingStatus(BillId, "Open"))
                    {
                        var transCode = "IMR";
                        bool cStatus = coreAccountService.ImprestRecoupmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.ImprestBillBookingWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.ImprestRecoupmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Imprest Bills Recoupment
        public ActionResult ImprestBillsRecoupList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetImprestBillsRecoupList()
        {
            try
            {
                object output = coreAccountService.GetImprestRecoupBillList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveImprestBillsRecoup(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveImprestBillRecoupment(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ImprestBillsRecoupment(int id = 0, int BillRecoupid = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ImprestBillRecoupModel model = new ImprestBillRecoupModel();
                if (id == 0 && BillRecoupid > 0 && Common.ValidateImprestBillRecoupStatus(BillRecoupid, "Open"))
                {
                    model = coreAccountService.GetIMPBillRecoupDetails(BillRecoupid);

                }
                if (id > 0 && BillRecoupid == 0 && Common.ValidateImprestBillStatus(id, "Approved"))
                {
                    model = coreAccountService.GetIMPBillDetails(id);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ImprestBillsRecoupment(ImprestBillRecoupModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();

                foreach (var item in model.CrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                foreach (var item in model.DrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateImprestBillsRecoupment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ImprestBillRecoupIU(model, logged_in_user);
                    if (model.ImprestBillRecoupId == null && result > 0)
                    {
                        TempData["succMsg"] = "Imprest Bills Recoupment has been added successfully.";
                        return RedirectToAction("ImprestBillsRecoupList");
                    }
                    else if (model.ImprestBillRecoupId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Imprest Bills Recoupment has been updated successfully.";
                        return RedirectToAction("ImprestBillsRecoupList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =

                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }


        public ActionResult ImprestBillsRecoupmentView(int id, bool Pfinit = false)
        {
            try
            {

                ImprestBillRecoupModel model = new ImprestBillRecoupModel();
                model = coreAccountService.GetIMPBillRecoupDetailsView(id);
                decimal amount = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(67, "Others", amount);
                model.PFInit = Pfinit;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateImprestBillsRecoupment(ImprestBillRecoupModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.AccountHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.AccountHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";
            //foreach (var item in model.CrDetail)
            //{
            //    int headId = item.AccountHeadId ?? 0;
            //    decimal balAmt = Common.GetBankClosingBalance(headId);
            //    if (balAmt < item.Amount)
            //    {
            //        msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
            //        break;
            //    }
            //}
            return msg;
        }
        [HttpPost]
        public JsonResult LoadImprestBillRecoupmentList(SearchImpresrtMaster model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetImprestBillRecoupList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ImprestBillRecoupWFInit(int BillId)
        {
            try
            {
                lock (ImprestRecWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateImprestBillRecoupStatus(BillId, "Open"))
                    {
                        var transCode = "IBR";

                        bool status = coreAccountService.ImprestBillRecoupWFInit(BillId, userId, transCode);

                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Imprest Close
        [HttpGet]
        public ActionResult ImprestPaymentClose(int ImpID = 0)
        {
            try
            {
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;

                ImprestPaymentModel model = new ImprestPaymentModel();
                model = CoreAccountsService.GetImprestMasterForClose(ImpID);
                model.TotalValueofCard = 0;
                model.CurrentImprestValue = 0;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();

                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ImprestPaymentModel model = new ImprestPaymentModel();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult ImprestPaymentClose(ImprestPaymentModel model)
        {
            try
            {
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                string msg = Common.ValidateImprestClose(model.ImprestcardId);
                if (msg != "valid")
                {
                    TempData["errMsg"] = msg;
                    return RedirectToAction("ImprestPaymentList");
                }
                int result = coreAccountService.CreateImprestClose(model, logged_in_user);
                if (result > 0)
                {
                    TempData["succMsg"] = "Imprest master closed successfully.";
                    return RedirectToAction("ImprestPaymentList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                    return RedirectToAction("ImprestPaymentList");
                }

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
    (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return RedirectToAction("ImprestPaymentList");
            }
        }
        public ActionResult ImprestPaymentCloseView(int ImpID = 0)
        {
            try
            {
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ImprestPaymentModel model = new ImprestPaymentModel();
                model = coreAccountService.EditImprestPaymentCloseDetails(ImpID);
                ViewBag.disabled = "Disabled";
                model.CreditorType = "PI";
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult ValidateImprestPaymentClose(int ImpID)
        {
            try
            {
                string msg = Common.ValidateImprestClose(ImpID);
                return Json(new { status = true, msg = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion
        #region Commitment
        public ActionResult _BookCommitment()
        {
            CommitmentModel model = new CommitmentModel();
            ViewBag.CommitmentType = Common.getCommitmentType();
            ViewBag.Purpose = Common.getPurpose();
            ViewBag.Currency = Common.getFRMcurrency();
            ViewBag.BudgetHead = Common.getBudgetHead();
            ViewBag.Employee = Common.GetEmployeeName();
            ViewBag.AccountHead = Common.getBudgetHead();
            ViewBag.ProjectNo = Common.getProjectNumber();
            ViewBag.Vendor = Common.getVendor();
            ViewBag.RequestRef = Common.getprojectsource();
            ViewBag.FundingBody = Common.GetFundingBody(model.SelProjectNumber);
            ViewBag.RefNo = new List<MasterlistviewModel>();
            ViewBag.SubheadList =
            ViewBag.RefNo = new List<MasterlistviewModel>();
            model.CommitmentNo = "0";
            model.commitmentValue = 0;
            model.currencyRate = 0;
            return PartialView(model);

        }

        [HttpPost]
        public JsonResult _SaveCommitment(CommitmentModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            AccountService _AS = new AccountService();
            var output = _AS.SaveCommitDetails(model, UserId, true);
            return Json(output.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Journal

        public ActionResult JournalList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetJournalList()
        {
            try
            {
                object output = coreAccountService.GetJournalList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveJournal(int journalId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveJournal(journalId, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Journal(int journalId = 0)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                JournalModel model = new JournalModel();
                if (journalId > 0 && Common.ValidateJournalStatus(journalId, "Open"))
                {
                    model = coreAccountService.GetJournalDetails(journalId);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult Journal(JournalModel model)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(false);
                    item.AccountHeadList = Common.GetAccountHeadList(headId, 0, "", "", false);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateJournal(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.JournalIU(model, logged_in_user);
                    if (model.JournalId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Journal has been added successfully.";
                        return RedirectToAction("JournalList");
                    }
                    else if (model.JournalId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Journal has been updated successfully.";
                        return RedirectToAction("JournalList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult JournalView(int journalId, bool Pfinit = false)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                JournalModel model = new JournalModel();
                model = coreAccountService.GetJournalDetails(journalId);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";

                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(55, "Others", model.CreditAmount ?? 0);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateJournal(JournalModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;

            if (ttlCrAmt != ttlDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (ttlCrAmt == 0)
                msg = msg == "Valid" ? "Please enter the valid credit and debit value." : msg + "<br /> Please enter the valid credit and debit value.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var gAH = model.ExpenseDetail.GroupBy(v => v.AccountHeadId);
            //if (model.ExpenseDetail.Count() != gAH.Count())
            //    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";

            return msg;
        }
        [HttpPost]
        public ActionResult JournalWFInit(int id)
        {
            try
            {
                lock (JournalWFInitlockObj)
                {
                    if (Common.ValidateJournalStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        var result = coreAccountService.JournalWFInit(id, userId);
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Adhoc Payment 
        public ActionResult AdhocPaymentList()
        {
            return View();
        }
        public ActionResult AdhocPayment(int adhocId = 0)
        {
            try
            {
                var emptyList = new List<AdhocPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                //ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                //ViewBag.Department = Common.getDepartment();
                //ViewBag.Student = Common.GetStudentList();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentMode");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(59);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AdhocPaymentModel model = new AdhocPaymentModel();
                model.CreditorType = "PI/Student/Others";
                if (adhocId > 0 && Common.ValidateAdhocPaymentOnEdit(adhocId))
                {
                    model = coreAccountService.GetAdhocPaymentDetails(adhocId);

                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(59);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }


        public ActionResult AdhocPaymentView(int adhocId, bool Pfinit = false)
        {

            try
            {


                AdhocPaymentModel model = new AdhocPaymentModel();

                model = coreAccountService.GetAdhocPaymentDetailsView(adhocId);
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(59, "Others", model.NetPayableValue ?? 0);
                ViewBag.disabled = "Disabled";
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdhocPayment(AdhocPaymentModel model)
        {
            try
            {
                var emptyList = new List<AdhocPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentMode");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(59);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdhocPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateAdhocPayment(model, logged_in_user);
                    if (model.AdhocId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Ad hoc payment has been added successfully.";
                        return RedirectToAction("AdhocPaymentList");
                    }
                    else if (model.AdhocId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Ad hoc payment has been updated successfully.";
                        return RedirectToAction("AdhocPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdhocPayment(AdhocPaymentModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal adhocamt = model.NetPayableValue ?? 0;
            decimal eligtax = model.EligibleTaxValue ?? 0;
            decimal paymentamt = adhocamt - eligtax;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (paymentamt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public JsonResult SearchAdhocPaymentList(AdhocPaySearchFieldModel model)
        {
            object output = coreAccountService.SearchAdhocPaymentList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AdhocPaymentApprove(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.AdhocPayCommitmentBalanceUpdate(id, false, false, userId, "REM");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.AdhocPaymentBillApproved(id, userId);
                if (!status)
                    coreAccountService.AdhocPayCommitmentBalanceUpdate(id, true, false, userId, "REM");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AdhocPaymentWFInit(int billId)
        {
            try
            {
                lock (REMWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool cStatus = coreAccountService.AdhocPayCommitmentBalanceUpdate(billId, false, false, userId, "REM");
                    if (!cStatus)
                        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    bool status = coreAccountService.AdhocPaymentWFInit(billId, userId);
                    if (!status)
                        coreAccountService.AdhocPayCommitmentBalanceUpdate(billId, true, false, userId, "REM");
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTransactionTypecode(string Paymenttype)
        {
            Paymenttype = Paymenttype == "" ? "0" : Paymenttype;
            var locationdata = Common.gettranstypecode(Convert.ToInt32(Paymenttype));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIDetailsbyName(string Name)
        {
            var locationdata = coreAccountService.getPIDetails(Name);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBankDetailsbyCategory(string Category, int UsrId)
        {
            try
            {
                //ProjectId = ProjectId == "" ? "0" : ProjectId;
                object output = coreAccountService.GetBankDetailsbyCategory(Category, UsrId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetAdhocPaymentList(ReimbursementSearchModel model, int pageIndex, int pageSize, DateFilterModel AdhocPaymentDate)
        {
            try
            {
                object output = coreAccountService.GetAdhocPaymentList(model, pageIndex, pageSize, AdhocPaymentDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Project Fund Transfer
        public ActionResult ProjectFundTransferList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetProjectFundTransferList(SearchProjectFunTransferModel model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetProjectFundTransferList(model,pageIndex,  pageSize,PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveProjectFundTransfer(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveProjectFundTransfer(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult PFTWFInit(int id)
        {
            try
            {
                lock (PFTWFInitlockObj)
                {
                    if (Common.ValidateProjectFundTransferStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        var result = coreAccountService.PFTWFInit(id, userId);
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProjectFundTransfer(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                ProjectFundTransferModel model = new ProjectFundTransferModel();
                if (id > 0 && Common.ValidateProjectFundTransferStatus(id, "Open"))
                {
                    model = coreAccountService.GetProjectFundTransferDetails(id);

                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ProjectFundTransfer(ProjectFundTransferModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BudgetHeadId = Common.getBudgetHead();

                if (ModelState.IsValid)
                {
                    string validationMsg = coreAccountService.ValidateProjectFundTransfer(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ProjectFundTransferIU(model, logged_in_user);
                    if (model.ProjectTransferId == null && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been added successfully.";
                        return RedirectToAction("ProjectFundTransferList");
                    }
                    else if (model.ProjectTransferId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been updated successfully.";
                        return RedirectToAction("ProjectFundTransferList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();

                ViewBag.BudgetHeadId = Common.getBudgetHead();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }


        public ActionResult ProjectFundTransferView(int id, bool Pfinit = false)
        {
            try
            {

                ProjectFundTransferModel model = new ProjectFundTransferModel();
                model = coreAccountService.GetProjectFundTransferDetailsView(id);
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(58, "Others", model.DebitAmount ?? 0);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        #endregion
        #region Direct Project Transfer
        public ActionResult ProjectTransferList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProjectTransferList(ProjectTransferSearch model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetProjectTransferList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveProjectTransfer(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.ProjectTransferCommitmentBalanceUpdate(id, false, false, userId, "PDT");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ApproveProjectTransfer(id, userId);
                if (!status)
                    coreAccountService.ProjectTransferCommitmentBalanceUpdate(id, true, false, userId, "PDT");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ProjectTransfer(int id = 0)
        {
            try
            {
                List<MasterlistviewModel> emptylist = new List<MasterlistviewModel>();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.SourceList = Common.GetSourceList();
                ProjectTransferModel model = new ProjectTransferModel();
                if (id > 0 && Common.ValidateProjectDirectTransferStatus(id, "Open"))
                {
                    model = coreAccountService.GetProjectTransferDetails(id);
                    ViewBag.Creditledger = Common.GetICSROHledger(true, model.CreditIcsroh_f);
                    ViewBag.Debitledger = Common.GetICSROHledger(false, model.DebitIcsroh_f);
                }
                else
                {
                    ViewBag.Creditledger = emptylist;
                    ViewBag.Debitledger = emptylist;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ProjectTransfer(ProjectTransferModel model)
        {
            try
            {
                List<MasterlistviewModel> emptylist = new List<MasterlistviewModel>();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.Creditledger = emptylist;
                ViewBag.Debitledger = emptylist;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.SourceList = Common.GetSourceList();
                if (ModelState.IsValid)
                {

                    if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
                    {
                        TempData["errMsg"] = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                        return View(model);
                    }
                    if (model.DebitAmount != model.CommitmentAmount)
                    {
                        TempData["errMsg"] = "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                        return View(model);
                    }
                    if(!Common.ValidateProjectBalanceOnReceipt(model.CreditProjectId ?? 0,0,model.DebitAmount ?? 0))
                    {
                        TempData["errMsg"] = "Should not exceed the credit project sanction value. Please update the value to continue.";
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ProjectTransferIU(model, logged_in_user);
                    if (model.ProjectTransferId == null && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been added successfully.";
                        return RedirectToAction("ProjectTransferList");
                    }
                    else if (model.ProjectTransferId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been updated successfully.";
                        return RedirectToAction("ProjectTransferList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        [HttpPost]
        public JsonResult LoadIcsrohLedger(bool Credit_f, bool ICSR_f)
        {
            var locationdata = Common.GetICSROHledger(Credit_f, ICSR_f);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckIcsrohProject(int ProjId)
        {
            try
            {
                object output = Common.IsICSROHproj(ProjId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                object output = false;
                return Json(output, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProjectTransferView(int id, bool Pfinit = false)
        {
            try
            {

                ProjectTransferModel model = new ProjectTransferModel();
                model = coreAccountService.GetProjectTransferDetailsView(id);
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(91, "Others", model.DebitAmount ?? 0);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ProjectTransferWFInit(int id)
        {
            try
            {
                lock (PDTWFInitlockObj)
                {
                    if (Common.ValidateProjectDirectTransferStatus(id, "Open"))
                    {
                       
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.ProjectTransferCommitmentBalanceUpdate(id, false, false, userId, "PDT");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.ProjectTransferWFInit(id, userId);
                        if (!result.Item1)
                            coreAccountService.ProjectTransferCommitmentBalanceUpdate(id, true, false, userId, "PDT");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PDTBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult PDTBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdatePDTBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadPDTRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "PDT" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPDTBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetPDTCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Transaction Definition
        [HttpGet]
        public ActionResult TransactionAndTaxesList()
        {
            List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            var transactiontype = Common.GetTransactionType();
            ViewBag.TransType = transactiontype;
            var subcode = Common.GetSubCode();
            ViewBag.subcode = subcode;
            var group = Common.GetAccountGroupList();
            ViewBag.Group = group;
            var head = Common.GetAccountHeadList();
            ViewBag.Head = head;
            var category = Common.GetDeductionCategory();
            ViewBag.Category = category;
            return View();
        }
        public ActionResult Transaction(string transaction, string subcode)
        {
            var model = CoreAccountsService.Transaction(transaction, subcode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddTransaction(string group, string head, string type, string isjv, string transaction, string subcode)
        {
            var model = CoreAccountsService.AddTransaction(group, head, type, isjv, transaction, subcode);
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DeleteTransaction(string transdefid)
        {
            var model = CoreAccountsService.DeleteTransaction(transdefid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Taxes(string transaction)
        {
            var model = CoreAccountsService.Taxes(transaction);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTaxes(int deheadid)
        {
            var model = CoreAccountsService.DeleteTaxes(deheadid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddTaxes(string taxgroup, string taxhead, string taxcategory, string taxinterstate, string transaction)
        {
            var model = CoreAccountsService.AddTaxes(taxgroup, taxhead, taxcategory, taxinterstate, transaction);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAccHead(string accgrp)
        {

            accgrp = accgrp == "" ? "0" : accgrp;
            var locationdata = Common.LoadGrpWiseHeadList(Convert.ToInt32(accgrp));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSubCode(string transtype)
        {

            transtype = transtype == "" ? "0" : transtype;
            var locationdata = Common.LoadSubCodeList(transtype);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Credit Note
        public ActionResult CreditNoteList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCreditNoteList()
        {
            try
            {
                object output = coreAccountService.GetCreditNoteList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveCreditNote(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveCreditNote(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult CreditNote(int creditNoteId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                CreditNoteModel model = new CreditNoteModel();
                if (creditNoteId > 0)
                {
                    model = coreAccountService.GetInvoiceDetailsForCreditNote(creditNoteId, true);

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CreditNote(CreditNoteModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                if (ModelState.IsValid)
                {
                    bool editMode = model.CreditNoteId == null ? false : true;
                    decimal avlBal = 0;
                    if (editMode)
                        avlBal = Common.GetAvailableAmtForCreditNote(model.CreditNoteId ?? 0, editMode);
                    else
                        avlBal = Common.GetAvailableAmtForCreditNote(model.InvoiceId ?? 0, editMode);
                    if (avlBal < model.TotalCreditAmount)
                    {
                        TempData["errMsg"] = "Credit note amount should not be grater than invoice balance amount.";
                        return View(model);
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreditNoteIU(model, logged_in_user);
                    if (model.CreditNoteId == null && result > 0)
                    {
                        TempData["succMsg"] = "Credit note has been added successfully.";
                        return RedirectToAction("CreditNoteList");
                    }
                    else if (model.CreditNoteId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Credit note has been updated successfully.";
                        return RedirectToAction("CreditNoteList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                return View();
            }
        }


        public ActionResult CreditNoteView(int creditNoteId, bool Pfinit = false)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();

                ViewBag.disabled = "disabled";

                CreditNoteModel model = new CreditNoteModel();
                model = coreAccountService.GetInvoiceDetailsForCreditNoteView(creditNoteId, true);
                model.PFInit = Pfinit;
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpGet]
        public JsonResult LoadInvoiceList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteInvoceNumber(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInvoiceDetail(int invoiceId)
        {
            try
            {
                var data = coreAccountService.GetInvoiceDetailsForCreditNote(invoiceId, false);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SearchCreditNoteList(int pageIndex, int pageSize, SearchCreditNoteModel model, DateFilterModel InvoiceDate)
        {
            try
            {
                object output = coreAccountService.SearchCreditNoteList(pageIndex, pageSize, model, InvoiceDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Contra
        public ActionResult ContraList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetContraList(SearchContra model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetContraList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveContra(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveContra(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Contra(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ContraModel model = new ContraModel();
                if (id > 0 && Common.ValidateContraStatus(id, "Open"))
                {
                    model = coreAccountService.GetContraDetails(id);

                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult Contra(ContraModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();

                foreach (var item in model.CrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                foreach (var item in model.DrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    //string validationMsg = ValidateContra(model);
                    //if (validationMsg != "Valid")
                    //{
                    //    TempData["errMsg"] = validationMsg;
                    //    return View(model);
                    //}
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ContraIU(model, logged_in_user);
                    if (model.ContraId == null && result > 0)
                    {
                        TempData["succMsg"] = "Contra has been added successfully.";
                        return RedirectToAction("ContraList");
                    }
                    else if (model.ContraId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Contra has been updated successfully.";
                        return RedirectToAction("ContraList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =

                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult ContraView(int id, bool Pfinit = false)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();

                ContraModel model = new ContraModel();
                model = coreAccountService.GetContraDetailsView(id);
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(64, "Others", model.DebitAmount ?? 0);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }

        private string ValidateContra(ContraModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.AccountHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.AccountHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";
            foreach (var item in model.CrDetail)
            {
                int headId = item.AccountHeadId ?? 0;
                decimal balAmt = Common.GetBankClosingBalance(headId);
                if (balAmt < item.Amount)
                {
                    msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
                    break;
                }
            }
            return msg;
        }

        [Authorized]
        public JsonResult GetAccountHeadBalance(int hdId)
        {
            object output = Common.GetBankClosingBalance(hdId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ContraWFInit(int id)
        {
            try
            {
                lock (ContraWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    var result = coreAccountService.ContraWFInit(id, userId);
                    return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Distribution 
        public ActionResult DistributionList()
        {
            return View();
        }
        public ActionResult Distribution(int distributionId = 0)
        {
            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Labcode = Common.getLabCodeProjectList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(61);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                DistributionModel model = new DistributionModel();
                model.CreditorType = "Professor/Staff";
                model.DistributionOverheads = Common.getInstituteOHPercentage();
                model.InstituteOverheadPercentage = model.DistributionOverheads.Select(m => m.OverheadPercentage).Sum() ?? 0;
                if (distributionId > 0 && Common.ValidateDistributionOnEdit(distributionId))
                {
                    model = coreAccountService.GetDistributionDetails(distributionId);

                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(61);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("CoreAccounts", "DistributionList");
            }

        }
        [HttpPost]
        public ActionResult Distribution(DistributionModel model)
        {
            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.Labcode = Common.getLabCodeProjectList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(61);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateDistribution(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateDistribution(model, logged_in_user);
                    if (model.DistributionId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Distribution payment has been added successfully.";
                        return RedirectToAction("DistributionList");
                    }
                    else if (model.DistributionId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Distribution payment has been updated successfully.";
                        return RedirectToAction("DistributionList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateDistribution(DistributionModel model)
        {
            string msg = "Valid";
            //decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal paymentamt = model.DistributionAmount ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount)))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (ttldeductAmt != ttlExpAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (paymentamt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        public ActionResult DistributionView(int distributionId, bool Pfinit = false)
        {

            try
            {
                DistributionModel model = new DistributionModel();

                model = coreAccountService.GetDistributionDetailsView(distributionId);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(61, "Others", model.FacultyDistributedTotalAmount ?? 0);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpGet]
        public JsonResult GetDistributionList()
        {
            try
            {
                object output = coreAccountService.GetDistributionList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchDistributionList(DistributionSearchFieldModel model)
        {
            object output = coreAccountService.SearchDistributionList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DistributionApprove(int DistributionId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, false, false, userId, "DIS");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.DistributionBillApproved(DistributionId, userId);
                if (!status)
                    coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, true, false, userId, "DIS");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DistributionWFInit(int DistributionId)
        {
            try
            {
                lock (DISWFInitlockObj)
                {
                    if (Common.ValidateDistributionOnEdit(DistributionId))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, false, false, userId, "DIS");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.DistributionWFInit(DistributionId, userId);
                        if (!result.Item1)
                            coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, true, false, userId, "DIS");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadDistributeProjectList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProjects(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectsummary(string ProjectId)
        {
            ProjectService _ps = new ProjectService();
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = _ps.getProjectSummary(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectDuration(string ProjectId)
        {
            CoreAccountsService _as = new CoreAccountsService();
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = _as.getProjectDuration(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectIdbyNumber(string ProjectNumber)
        {

            ProjectNumber = ProjectNumber == "" ? "0" : ProjectNumber;
            var locationdata = Common.getProjectidbynumber(ProjectNumber);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteStaffList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadProfessorList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProfList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadProjectStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProjectStaffList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStaffDetailsbyId(int EmpId, string Category)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            var locationdata = _cs.getStaffDetails(EmpId, Category);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPIDetailsbyId(string UserId)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            UserId = UserId == "" ? "0" : UserId;
            var locationdata = _cs.getPIDesigandDep(Convert.ToInt32(UserId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDistributionList(int pageIndex, int pageSize, SearchDistributionModel model, DateFilterModel DistributionDate)
        {
            try
            {
                object output = coreAccountService.GetDistributionList(pageIndex, pageSize, model, DistributionDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region  BRS
        [HttpGet]
        public ActionResult BRS(int BRSId = 0)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.BankDrReasonList = Common.GetCommonHeadList(1, 3, true);
                ViewBag.BankCrReasonList = Common.GetCommonHeadList(1, 3, false);
                ViewBag.BookDrReasonList = Common.GetCommonHeadList(2, 3, true);
                ViewBag.BookCrReasonList = Common.GetCommonHeadList(2, 3, false);
                BRSModel model = new BRSModel();
                if (BRSId > 0 && Common.ValidateBRSOnEdit(BRSId))
                {
                    model = coreAccountService.GetBRSDetails(BRSId);
                    ViewBag.Reconciled = Common.IsBRSReconciled(BRSId);
                }
                else
                    ViewBag.Reconciled = false;
                //coreAccountService.GetBRSReport(BRSId);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult BRS(BRSModel model)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.BankDrReasonList = Common.GetCommonHeadList(1, 3, true);
                ViewBag.BankCrReasonList = Common.GetCommonHeadList(1, 3, false);
                ViewBag.BookDrReasonList = Common.GetCommonHeadList(2, 3, true);
                ViewBag.BookCrReasonList = Common.GetCommonHeadList(2, 3, false);
                ViewBag.Reconciled = model.boaDetail.Any(m => m.Reconciliation_f == true);
                if (ModelState.IsValid)
                {
                    //var isNotReconcile = model.txDetail.Any(m => m.Status == "Open");
                    //if (isNotReconcile && model.Status == "Open")
                    //{
                    //    TempData["errMsg"] = "Some of the bank statement still not reconcile.";
                    //    return View(model);
                    //}
                    //else 
                    //if (model.txDetail == null || model.txDetail.Count() == 0)
                    //{
                    //    TempData["errMsg"] = "No bank statement exists.";
                    //    return View(model);
                    //}
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.BRSIU(model, logged_in_user);
                    if (result > 0)
                    {
                        TempData["succMsg"] = "BRS has been added successfully.";
                        return RedirectToAction("BRSList");
                    }
                    else if (result == -2)
                    {
                        TempData["errMsg"] = "Already reconcile for this date.";
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.BankDrReasonList = Common.GetCommonHeadList(1, 3, true);
                ViewBag.BankCrReasonList = Common.GetCommonHeadList(1, 3, false);
                ViewBag.BookDrReasonList = Common.GetCommonHeadList(2, 3, true);
                ViewBag.BookCrReasonList = Common.GetCommonHeadList(2, 3, false);
                return View(model);
            }

        }

        public ActionResult BRSList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetBRSList(SearchBRSModel model)
        {
            try
            {
                object output = coreAccountService.GetBRSList(model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult BRSView(int id)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.BankDrReasonList = Common.GetCommonHeadList(1, 3, true);
                ViewBag.BankCrReasonList = Common.GetCommonHeadList(1, 3, false);
                ViewBag.BookDrReasonList = Common.GetCommonHeadList(2, 3, true);
                ViewBag.BookCrReasonList = Common.GetCommonHeadList(2, 3, false);
                BRSModel model = new BRSModel();
                model = coreAccountService.GetBRSDetails(id, true);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public JsonResult ImportBankStatement(HttpPostedFileBase file, int bankId, int brsId = 0)
        {
            Utility _uty = new Utility();
            BRSModel model = new BRSModel();
            List<BankStatementDetailModel> list = new List<BankStatementDetailModel>();
            string msg = "Valid";
            list = coreAccountService.GetPendingBankTx(brsId, bankId);
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                string connString = "";
                string[] validFileTypes = { ".xls", ".xlsx" };
                string actName = Path.GetFileName(file.FileName);
                var guid = Guid.NewGuid().ToString();
                var docName = guid + "_" + actName;
                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/BankStatement"), docName);
                model.DocumentActualName = actName;
                model.DocumentName = docName;
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/BankStatement"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    file.SaveAs(path1);
                    file.UploadFile("BankStatement", docName);
                    //Connection String to Excel Workbook  
                    List<BankStatementDetailModel> listUpload = new List<BankStatementDetailModel>();
                    if (extension.ToLower().Trim() == ".csv")
                    {
                        DataTable dt = _uty.ConvertCSVtoDataTable(path1);
                        listUpload = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                    }
                    else if (extension.ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString);
                        listUpload = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                    }
                    else
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString);
                        listUpload = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                    }
                    if (listUpload.Count > 0)
                        list.AddRange(listUpload);
                }
                else
                {
                    msg = "Please Upload Files in .xls or .xlsx format";
                }
            }
            model.txDetail = list;
            // return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

            //  return Json(new { status = msg, data = model }, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { status = msg, data = model }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBOAPaymentDetails(DateTime toDate, int headId, int BRSId)
        {
            try
            {
                toDate = toDate.Date.AddTicks(-10001);
                var data = coreAccountService.GetBOAPaymentDetailsWithCurr(toDate, headId, BRSId);
                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult _AdhocTransaction(int indx, int headId, string txType)
        {
            try
            {
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                CommonPaymentModel model = new CommonPaymentModel();
                model = coreAccountService.GetAdhocTransaction(headId, txType);
                model.RefId = indx;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                throw ex;// new Exception(ex.Message);
            }
        }

        public FileStreamResult GetBRSReport(int BRSId = 0, int bankId = 0, DateTime? asOn = null)
        {
            MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            using (var context = new IOASDBEntities())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    BRSReportModel model = new BRSReportModel();
                    if (BRSId > 0)
                    {
                        var data = Common.GetBRSBankAndDate(BRSId);
                        bankId = data.Item1;
                        asOn = Convert.ToDateTime(data.Item2);
                    }
                    model = coreAccountService.GetBRSReport(BRSId);// coreAccountService.GetBRSReport(bankId, asOn);
                    DataSet dataset = new DataSet();
                    DataTable dtColumns1 = new DataTable();
                    DataTable dtColumns2 = new DataTable();
                    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(model.AddList);
                    dtColumns1 = JsonConvert.DeserializeObject<DataTable>(json1);
                    string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(model.LessList);
                    dtColumns2 = JsonConvert.DeserializeObject<DataTable>(json2);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter();
                    var ws = wb.Worksheets.Add("BRS");
                    //  ws.Cell(1, 4).Value = "BANK RECONCILIATION STATEMENT FOR THE MONTH OF("+ model.FromDate+" to "+model.ToDate+")";
                    ws.Cell(2, 1).Value = "";
                    var DATE = ws.Range("A2:D4");
                    DATE.Style.Font.Bold = true;
                    ws.Cell(2, 2).Value = "PARTICULARS";
                    //var PARTICULARS = ws.Cell(2, 2);
                    //PARTICULARS.Style.Font.Bold = true;
                    ws.Cell(2, 3).Value = "SUBTOTAL";
                    //var SUBTOTAL = ws.Cell(2, 3);
                    //SUBTOTAL.Style.Font.Bold = true;
                    ws.Cell(2, 4).Value = "TOTAL AMOUNT";
                    ws.Cell(4, 2).Value = "BALANCE AS PER CASH BOOK(" + model.Bank + ")  AS ON " + String.Format("{0:dd-MM-yyyy}", model.ToDate.Value.AddDays(-1));
                    ws.Cell(4, 4).Value = model.CashBookBalance;
                    var BankAmt = ws.Cell(4, 4);
                    BankAmt.Style.Font.Bold = true;
                    ws.Cell(7, 1).Value = "ADD:";
                    //ws.Cell(7, 2).Value = "To be Increased in Cash book";
                    var ADD = ws.Range("A7:B7");
                    ADD.Style.Font.Bold = true;

                    int firstrow = 8;
                    foreach (DataRow row in dtColumns1.Rows)
                    {
                        ws.Cell(firstrow, 2).Value = row["Reason"].ToString();
                        ws.Cell(firstrow, 3).Value = row["TotalUnReconcileAmount"].ToString();
                        firstrow++;
                    }
                    decimal addMap = model.AddList.Sum(m => m.TotalUnReconcileAmount) ?? 0;
                    ws.Cell(firstrow, 4).Value = addMap;
                    var AddAmt = ws.Cell(firstrow, 4);
                    AddAmt.Style.Font.Bold = true;
                    int secondrow = firstrow + 2;
                    ws.Cell(secondrow, 1).Value = "LESS:";
                    var LESS = ws.Cell(secondrow, 1);
                    LESS.Style.Font.Bold = true;
                    //ws.Cell(secondrow, 2).Value = "To be Reduced from Cash book";
                    var LESS2 = ws.Cell(secondrow, 2);
                    LESS2.Style.Font.Bold = true;
                    int thirdrow = secondrow + 1;
                    foreach (DataRow row in dtColumns2.Rows)
                    {
                        ws.Cell(thirdrow, 2).Value = row["Reason"].ToString();
                        ws.Cell(thirdrow, 3).Value = row["TotalUnReconcileAmount"].ToString();
                        thirdrow++;
                    }
                    decimal lessMap = model.LessList.Sum(m => m.TotalUnReconcileAmount) ?? 0;
                    ws.Cell(thirdrow, 4).Value = lessMap;

                    thirdrow = thirdrow + 2;
                    ws.Cell(thirdrow, 2).Value = "Unreconciled :";
                    var unRec = ws.Cell(thirdrow, 2);
                    unRec.Style.Font.Bold = true;
                    thirdrow = thirdrow + 1;
                    ws.Cell(thirdrow, 1).Value = "ADD :";
                    var addUnRecCell = ws.Cell(thirdrow, 1);
                    addUnRecCell.Style.Font.Bold = true;
                    foreach (var item in model.UnRecAddList)
                    {
                        ws.Cell(thirdrow, 2).Value = item.Reason;
                        ws.Cell(thirdrow, 3).Value = item.TotalUnReconcileAmount.ToString();
                        thirdrow++;
                    }
                    decimal addUnRec = model.UnRecAddList.Sum(m => m.TotalUnReconcileAmount) ?? 0;
                    ws.Cell(thirdrow, 4).Value = addUnRec;
                    var ttlAddUnRec = ws.Cell(thirdrow, 4);
                    ttlAddUnRec.Style.Font.Bold = true;
                    thirdrow = thirdrow + 1;
                    ws.Cell(thirdrow, 1).Value = "LESS :";
                    var lessUnRecCell = ws.Cell(thirdrow, 1);
                    lessUnRecCell.Style.Font.Bold = true;
                    foreach (var item in model.UnRecLessList)
                    {
                        ws.Cell(thirdrow, 2).Value = item.Reason;
                        ws.Cell(thirdrow, 3).Value = item.TotalUnReconcileAmount.ToString();
                        thirdrow++;
                    }
                    decimal lessUnRec = model.UnRecLessList.Sum(m => m.TotalUnReconcileAmount) ?? 0;
                    ws.Cell(thirdrow, 4).Value = lessUnRec;
                    var ttlLessUnRec = ws.Cell(thirdrow, 4);
                    ttlLessUnRec.Style.Font.Bold = true;

                    thirdrow = thirdrow + 3;
                    ws.Cell(thirdrow, 2).Value = "BALANCE AS PER BANK STATEMENT :";
                    var BankSta = ws.Cell(thirdrow, 2);
                    BankSta.Style.Font.Bold = true;
                    ws.Cell(thirdrow, 4).Value = (model.CashBookBalance ?? 0) + addMap + addUnRec - lessMap - lessUnRec;
                    //thirdrow = thirdrow + 3; 
                    //ws.Cell(thirdrow, 2).Value = "RECONCILED AMOUNT :";
                    //var recAmt = ws.Cell(thirdrow, 2);
                    //BankSta.Style.Font.Bold = true;
                    //ws.Cell(thirdrow, 4).Value = model.ReconcileAmount ?? 0;
                    var TotalAmt = ws.Cell(thirdrow, 4);
                    TotalAmt.Style.Font.Bold = true;
                    var rngTitle = ws.Range("A1:D1").Merge();
                    rngTitle.Style.Font.Bold = true;
                    rngTitle.Value = "BANK RECONCILIATION STATEMENT";

                    thirdrow = thirdrow + 2;
                    var lastBankTx = ws.Cell(thirdrow, 2);
                    lastBankTx.Style.Font.Bold = true;
                    lastBankTx.Style.Fill.BackgroundColor = XLColor.Yellow;
                    lastBankTx.Value = "Note: Bank Statement Uploaded Till DD-MM-YYYY";
                    ws.Cell(thirdrow, 3).Value = model.BankLastDateofTx;
                    thirdrow = thirdrow + 1;
                    var closeBankAmt = ws.Cell(thirdrow, 2);
                    closeBankAmt.Style.Font.Bold = true;
                    closeBankAmt.Style.Fill.BackgroundColor = XLColor.Yellow;
                    closeBankAmt.Value = "Bank Statement Closing Balance as on last date uploaded";
                    ws.Cell(thirdrow, 3).Value = model.BankClosingBalance;
                    var ds = coreAccountService.GetBRSBreakUpDetails(BRSId); //coreAccountService.GetBRSBreakUpDetails(bankId, asOn);
                    if (ds != null)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            //object sumObject = dt.Compute("Sum(Amount)", string.Empty);
                            //decimal ttl = Convert.ToDateTime(sumObject);
                            var name = Convert.ToString(dt.TableName);
                            wb.Worksheets.Add(dt, name);
                        }
                    }
                    wb.SaveAs(workStream);
                    workStream.Position = 0;

                }

            }
            return new FileStreamResult(workStream, "application/vnd.ms-excel");
        }
        public FileStreamResult GetBankStatementReport(DateTime toDate, int headId, int BRSId)
        {
            toDate = toDate.Date.AddTicks(-10001);
            var list = coreAccountService.GetBOAPaymentDetails(toDate, headId, BRSId);
            DataTable dt = new DataTable();
            string json = JsonConvert.SerializeObject(list);
            dt = JsonConvert.DeserializeObject<DataTable>(json);
            dt.Columns.Remove("BOAId");
            dt.Columns.Remove("Reason");
            dt.Columns.Remove("BOAPaymentDetailId");
            dt.Columns.Remove("Reconciliation_f");
            dt.Columns.Remove("PayeeBank");
            dt.Columns.Remove("BankHeadID");
            dt.Columns.Remove("PayeeId");
            dt.Columns.Remove("PaymentMode");
            dt.Columns.Remove("PayeeTypeId");
            dt.Columns.Remove("PayeeType");
            dt.Columns.Remove("PayeeName");
            dt.Columns.Remove("TransactionID");
            dt.Columns.Remove("TransactionStatus");
            dt.Columns.Remove("ChequeNumber");
            dt.Columns.Remove("StudentRoll");
            dt.Columns.Remove("AccountNumber");
            dt.Columns.Remove("IFSC");
            dt.Columns.Remove("Branch");
            dt.Columns.Remove("ReferenceDate");
            return coreAccountService.toSpreadSheet(dt);
        }
        #endregion
        #region Negative Balance
        [HttpPost]
        public JsonResult GetNegativeBalanceList(int pageIndex, int pageSize, SearchNegativeBalanceModel model, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetNegativeBalanceList(pageIndex, pageSize, model, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult NegativeBalanceList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetNegativeBalanceList()
        {
            try
            {
                object output = coreAccountService.GetNegativeBalanceList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveNegativeBalance(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveNegativeBalance(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult NegativeBalance(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNo = Common.getProjectNumber();
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                NegativeBalanceModel model = new NegativeBalanceModel();
                if (id > 0 && Common.ValidateNegativeBalanceStatus(id, "Open"))
                {
                    model = coreAccountService.GetNegativeBalanceDetails(id);

                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult NegativeBalance(NegativeBalanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNo = Common.getProjectNumber();
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();


                if (ModelState.IsValid)
                {
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    if (!Common.ValidateNegativeBalance(model.ProjectId.GetValueOrDefault(0), model.ClaimAmount.GetValueOrDefault(0), model.NegativeBalanceId)) // (sanctionValue < overAllAmt)
                    {
                        TempData["errMsg"] = "Negative Balance amount claimed cannot be greater than available limit. Please update the value.";
                        return View(model);
                    }
                    int result = coreAccountService.NegativeBalanceIU(model, logged_in_user);
                    if (model.NegativeBalanceId == 0 && result > 0)
                    {

                        TempData["succMsg"] = "Negative Balance has been added successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }
                    else if (model.NegativeBalanceId > 0 && result > 0)
                    {
                        ProjectService _ps = new ProjectService();
                        var details = _ps.getProjectSummary(Convert.ToInt32(model.ProjectId));
                        model.prjDetails = details;
                        TempData["succMsg"] = "Negative Balance has been updated successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult NegativeBalanceView(int id, bool Pfinit = false)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();

                NegativeBalanceModel model = new NegativeBalanceModel();
                model = coreAccountService.GetNegativeBalanceDetailsView(id);
                model.PFInit = Pfinit;
                var paymentVal = model.TotalClaimAmount ?? 0;
                int pgId = 0;
                var data = Common.GetProjectType(model.ProjectId ?? 0);
                int pType = data.Item1;
                int sponCate = data.Item2;
                if (pType == 1 && sponCate == 1)
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(66, "PFMS", paymentVal);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(66, "Others", paymentVal);
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        public ActionResult _CloseNegativeBalance(int id)
        {
            CloseNegativeBalanceModel model = new CloseNegativeBalanceModel();
            model = coreAccountService.GetNegativeBalCloseDetails(id);
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult _CloseNegativeBalance(CloseNegativeBalanceModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SaveCloseNegativeBal(model, logged_in_user);
                    if (model.NegativeBalanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Negative Balance has been closed successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }
                    else if (result == -2)
                    {
                        TempData["errMsg"] = "Negative Balance is greater than Available balance in the project. please contact administrator.";
                        return RedirectToAction("NegativeBalanceList");
                    }

                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    return RedirectToAction("NegativeBalanceList");
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return RedirectToAction("NegativeBalanceList");
            }

        }

        [HttpPost]
        public ActionResult NegativeBalanceWFInit(int billId)
        {
            try
            {
                lock (NBLWFInitlockObj)
                {
                    if (Common.ValidateNegativeBalanceStatus(billId, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool status = coreAccountService.NegativeBalanceWFInit(billId, userId);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrevNegativeBalance(int ProjectId)
        {
            NegativeBalanceModel model = new NegativeBalanceModel();
            try
            {
                model.ProjectId = ProjectId;
                model.PrevNeg = coreAccountService.GetPrevNegativeBalance(ProjectId);
                ViewBag.TtlAmt = model.PrevNeg.Sum(m => m.NegBalance);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception();
            }
        }
        [HttpGet]
        public JsonResult GetPrevNBLByProject(string PjctId)
        {
            try
            {
                object output = Common.GetPrevNBLTotalByProject(Convert.ToInt32(PjctId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region General Voucher
        public ActionResult GeneralVoucherList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetGeneralVoucherList(GeneralVoucherSearch model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetGeneralVoucherList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveGeneralVoucher(int id)
        {
            try
            {
                lock (GVRApprovelockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool cStatus = coreAccountService.GVRCommitmentBalanceUpdate(id, false, false, userId, "GVR");
                    if (!cStatus)
                        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    bool output = coreAccountService.ApproveGeneralVoucher(id, userId);
                    if (!output)
                        coreAccountService.GVRCommitmentBalanceUpdate(id, true, false, userId, "GVR");
                    return Json(output, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult GeneralVoucher(int id = 0)
        {
            try
            {
                GeneralVoucherModel model = new GeneralVoucherModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");

                var detail = Common.GetCodeControlList("PaymentCategory");
                detail.RemoveAt(2);
                detail.RemoveAt(2);
                ViewBag.PaymentCategoryList = detail;
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(true);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (id > 0 && Common.ValidateGeneralVoucherStatus(id, "Open"))
                {
                    model = coreAccountService.GetGeneralVoucherDetails(id);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                }
                else
                {
                    int[] heads = { 32, 33, 34 };
                    model.PaymentDeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public ActionResult GeneralVoucher(GeneralVoucherModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                var detail = Common.GetCodeControlList("PaymentCategory");
                detail.RemoveAt(2);
                detail.RemoveAt(2);
                ViewBag.PaymentCategoryList = detail;
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(true);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.TDSSectionList = Common.GetTdsList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                foreach (var item in model.PaymentExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (model.PaymentCategory == 1 || model.PaymentCategory == 4)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateGeneralVoucher(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.GeneralVoucherIU(model, logged_in_user);
                    if (model.VoucherId == null && result > 0)
                    {
                        TempData["succMsg"] = "General voucher has been added successfully.";
                        return RedirectToAction("GeneralVoucherList");
                    }
                    else if (model.VoucherId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "General voucher has been updated successfully.";
                        return RedirectToAction("GeneralVoucherList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                var detail = Common.GetCodeControlList("PaymentCategory");
                detail.RemoveAt(2);
                detail.RemoveAt(2);
                ViewBag.PaymentCategoryList = detail;
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(true);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TDSSectionList = Common.GetTdsList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                foreach (var item in model.PaymentExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                return View(model);
            }
        }
        public ActionResult GeneralVoucherView(int id, bool Pfinit = false)
        {
            try
            {
                GeneralVoucherModel model = new GeneralVoucherModel();

                model = coreAccountService.GetGeneralVoucherDetailsView(id);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(70, "Others", model.PaymentDebitAmount ?? 0);
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Dashboard", "Home");
            }
        }
        private string ValidateGeneralVoucher(GeneralVoucherModel model)
        {
            string msg = "Valid";
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTax = model.PaymentDeductionDetail != null ? model.PaymentDeductionDetail.Select(m => m.Amount).Sum() ?? 0 : 0;
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            //paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            decimal bankAmt = model.PaymentBankAmount ?? 0;
            //drAmt = drAmt + ttlTax;
            crAmt = crAmt + bankAmt;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (model.PaymentCategory != 1 && model.PaymentCategory != 4)
            {
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (drAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if (bankAmt != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry.The Payable value and Payment Break Up Total value are not equal." : msg + "<br /> Not a valid entry.The Payable value and Payment Break Up Total value are not equal.";
            }
            if ((drAmt + ttlTax) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var ah = model.PaymentExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpPost]
        public ActionResult GeneralVoucherWFInit(int id)
        {
            try
            {
                lock (GVRWFInitlockObj)
                {
                    if (Common.ValidateGeneralVoucherStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.GVRCommitmentBalanceUpdate(id, false, false, userId, "GVR");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.GeneralVoucherWFInit(id, userId);
                        if (!result.Item1)
                            coreAccountService.GVRCommitmentBalanceUpdate(id, true, false, userId, "GVR");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Import Payment 
        #region Foreign Remittance 
        public ActionResult ForeignRemittanceList()
        {
            return View();
        }
        public ActionResult ForeignRemittance(int foreignRemitId = 0, bool iseditdraft = false)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                //ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.ExpensesHead = Common.GetCodeControlList("ForgnRemitExpensesHead");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getFRMcurrency();
                // ViewBag.PaymentBank = Common.GetCodeControlList("DistributionType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                // ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                if (foreignRemitId > 0 && Common.ValidateForeignRemitOnEdit(foreignRemitId) && iseditdraft != true)
                {
                    model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                    //if (model.Source == 1)
                    //    ViewBag.SourceRefNumberList = Common.GetWorkflowRefNumberList();
                    //else if (model.Source == 3)
                    //{
                    //    int depId = Common.GetDepartmentId(User.Identity.Name);
                    //    ViewBag.SourceRefNumberList = Common.GetTapalRefNumberList(depId);
                    //}
                    ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                    if (model.ExpenseDetail.Count() == 0)
                    {
                        model.NeedUpdateTransDetail = true;
                    }
                    if (model.CheckListDetail.Count() == 0)
                    {
                        model.CheckListDetail = Common.GetCheckedList(76);
                    }

                }
                else if (foreignRemitId > 0 && iseditdraft == true && Common.ValidateForeignRemitOnEdit(foreignRemitId))
                {
                    model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                    ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                    model.IsEditDraft = true;
                }
                else
                {
                    model.RemitDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                    //model.CheckListDetail = Common.GetCheckedList(76);
                    //model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("ForeignRemittanceList", "CoreAccounts");
            }

        }


        public ActionResult ForeignRemittanceView(int foreignRemitId)
        {

            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                // ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.ExpensesHead = Common.GetCodeControlList("ForgnRemitExpensesHead");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                // ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                ForeignRemittanceModel model = new ForeignRemittanceModel();

                model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                //if (model.Source == 1)
                //    ViewBag.SourceRefNumberList = Common.GetWorkflowRefNumberList();
                //else if (model.Source == 3)
                //{
                //    int depId = Common.GetDepartmentId(User.Identity.Name);
                //    ViewBag.SourceRefNumberList = Common.GetTapalRefNumberList(depId);
                //}
                ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(76, "Others", model.BillAmount ?? 0);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult ForeignRemittance(ForeignRemittanceModel model)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                // ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (model.ForeignRemittanceId > 0 && model.IsEditDraft != true)
                {
                    if (ModelState.IsValid)
                    {
                        string validationMsg = ValidateForeignRemittance(model);
                        if (validationMsg != "Valid")
                        {
                            TempData["errMsg"] = validationMsg;
                            return View(model);
                        }
                        foreach (var item in model.DocumentDetail)
                        {
                            if (item.DocumentFile != null)
                            {
                                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                                string filename = Path.GetFileName(item.DocumentFile.FileName);
                                var docextension = Path.GetExtension(filename);
                                if (!allowedExtensions.Contains(docextension))
                                {
                                    TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                    return View(model);
                                }
                            }
                        }

                        int logged_in_user = Common.GetUserid(User.Identity.Name);
                        int result = coreAccountService.CreateForeignRemittance(model, logged_in_user);
                        if (model.ForeignRemittanceId == 0 && result > 0)
                        {
                            TempData["succMsg"] = "Foreign Remittance payment has been added successfully.";
                            return RedirectToAction("ForeignRemittanceList");
                        }
                        else if (model.ForeignRemittanceId > 0 && result > 0)
                        {
                            TempData["succMsg"] = "Foreign Remittance payment has been updated successfully.";
                            return RedirectToAction("ForeignRemittanceList");
                        }
                        else
                            TempData["errMsg"] = "Something went wrong please contact administrator.";

                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                    }
                }
                else
                {
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateForeignRemittance(model, logged_in_user);
                    if (model.ForeignRemittanceId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Foreign Remittance payment has been added successfully.";
                        return RedirectToAction("ForeignRemittanceList");
                    }
                    else if (model.ForeignRemittanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Foreign Remittance payment has been updated successfully.";
                        return RedirectToAction("ForeignRemittanceList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.Currency = Common.getFRMcurrency();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateForeignRemittance(ForeignRemittanceModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal paymentamt = model.ForeignRemittanceAmount ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (paymentamt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult GetForeignRemittanceList()
        {
            try
            {
                object output = coreAccountService.GetForeignRemittanceList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SearchForeignRemittanceList(int pageIndex, int pageSize, SearchForeignRemitModel model, DateFilterModel RemitDate)
        {
            try
            {
                object output = coreAccountService.SearchForeignRemittanceList(pageIndex, pageSize, model, RemitDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult ForeignRemittanceApprove(int foreignRemitId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ForeignRemittanceBillApproved(foreignRemitId, userId);
                if (!status)
                    coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, true, false, userId, "FRM");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetFRMDetailsbyProject(string ProjectId, string BankId)
        {
            try
            {
                ProjectId = ProjectId == "" ? "0" : ProjectId;
                BankId = BankId == "" ? "0" : BankId;
                object output = coreAccountService.GetFRMdetailsbyProject(Convert.ToInt32(ProjectId), Convert.ToInt32(BankId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetFRMBankDetails(string ProjectId, string BankId)
        {
            try
            {
                ProjectId = ProjectId == "" ? "0" : ProjectId;
                BankId = BankId == "" ? "0" : BankId;
                object output = coreAccountService.GetFRMBankdetails(Convert.ToInt32(ProjectId), Convert.ToInt32(BankId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetCurrencybyId(string CurrencyId)
        {
            try
            {
                CurrencyId = CurrencyId == "" ? "0" : CurrencyId;
                object output = Common.getCurrency(Convert.ToInt32(CurrencyId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ForeignRemitanceBillWFInit(int BillId, string transCode)
        {
            try
            {
                lock (FRMWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (transCode == "FRM")
                    {
                        //bool status = coreAccountService.TravelAdvanceBillWFInit(BillId, userId, transCode);
                        // return Json(status, JsonRequestBehavior.AllowGet);
                        //}
                        //else if (Common.ValidateTravelBillStatus(travelBillId, transCode, "Open"))
                        //{
                        //    bool reversed = false;
                        //    if (transCode == "TST")
                        //        reversed = Common.TSTBillIsReceipt(travelBillId);
                        bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.ForeignRemitanceWFInit(BillId, userId, transCode);
                        if (!result.Item1)
                            coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ForeignRemittanceBillofEntry(int foreignRemitId = 0, bool iseditbillofentry = false)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                //ViewBag.SourceRefNumberList = emptyList;
                //ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.ExpensesHead = Common.GetCodeControlList("ForgnRemitExpensesHead");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getFRMcurrency();
                // ViewBag.PaymentBank = Common.GetCodeControlList("DistributionType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                // ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                if (foreignRemitId > 0 && Common.ValidateForeignRemitOnBillEntryEdit(foreignRemitId) && iseditbillofentry == true)
                {
                    model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                    model.IsEditBillofEntry = true;
                    ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                }

                else
                {
                    model.RemitDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                    //model.CheckListDetail = Common.GetCheckedList(76);
                    //model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("ForeignRemittanceList", "CoreAccounts");
            }

        }
        [HttpPost]
        public ActionResult ForeignRemittanceBillofEntry(ForeignRemittanceModel model)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                // ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (model.ForeignRemittanceId > 0 && model.IsEditBillofEntry == true)
                {
                    //if (ModelState.IsValid)
                    //{                        
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.EditBillofEntry(model, logged_in_user);
                    if (model.ForeignRemittanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Foreign Remittance Bill of Entry has been updated successfully.";
                        coreAccountService.ForeignRemitBillEntryEmailSend(model.ForeignRemittanceId);
                        return RedirectToAction("ForeignRemittanceList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                    //}
                    //else
                    //{
                    //    string messages = string.Join("<br />", ModelState.Values
                    //                        .SelectMany(x => x.Errors)
                    //                        .Select(x => x.ErrorMessage));

                    //    TempData["errMsg"] = messages;
                    //}
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.CommitmentNumberList = Common.GetCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.Currency = Common.getFRMcurrency();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        //[HttpPost]
        //public ActionResult ForeignRemitanceBillApproved(int travelBillId)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            int userId = Common.GetUserid(User.Identity.Name);
        //            if (Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Approval"))
        //            {
        //                bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, false, userId, "TAD");
        //                if (!cStatus)
        //                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
        //                bool status = coreAccountService.TravelAdvanceBillApproved(travelBillId, userId);
        //                if (!status)
        //                    coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, userId, "TAD");
        //                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //                return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion
        #region LC Opening 
        public ActionResult LCOpeningList(string msg = null)
        {
            if (msg == "Success")
            {
                TempData["succMsg"] = "LC Opening has been created successfully.";
            }
            else if (msg == "Updated")
            {
                TempData["succMsg"] = "LC Opening has been updated successfully.";
            }
            else if (msg == "Established")
            {
                TempData["succMsg"] = "LC Opening has been Established successfully.";
            }
            else if (msg == "Error")
            {
                TempData["errMsg"] = "Something went wrong please try again or contact administrator.";
            }

            return View();
        }
        public ActionResult LCOpeningView(int LCdraftId = 0)
        {

            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(88);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                //ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                LCOpeningModel model = new LCOpeningModel();

                model = coreAccountService.GetLCOpeningDetails(LCdraftId);

                ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 206;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        public ActionResult LCOpening(int LCdraftId = 0, bool isEditMode = false, bool isEstablish = false, bool isEditEstablish = false)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(88);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                //
                LCOpeningModel model = new LCOpeningModel();
                model.CheckListDetail = Common.GetCheckedList(88);
                model.CreditorType = " ";
                model.NeedUpdateTransDetail = true;
                model.CreditAvailableBank = "Any Bank";
                model.CreditAvailableFor = "100% OF INVOICE VALUE DULY MARKED WITH OUR LC NO. AND DATE";
                model.Tenor42C = "At Sight";
                model.Charges71B = "ALL BANK CHARGES INSIDE INDIA ARE TO THE ACCOUNT OF APPLICANT AND OUTSIDE INDIA ARE TO THE ACCOUNT OF  BENEFICIARY";
                model.PresentationPeriod = "Documents should be presented for negotiation within 21 days from the date of shipment";

                if (LCdraftId > 0 && isEditMode == true && Common.ValidateLCOnEdit(LCdraftId))
                {
                    model = coreAccountService.GetLCOpeningDetails(LCdraftId);
                    model.IsEditMode = isEditMode;
                    ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                    model.NeedUpdateTransDetail = true;
                }
                if (LCdraftId > 0 && isEstablish == true && Common.ValidateLCOnEdit(LCdraftId))
                {
                    ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                    model = coreAccountService.GetLCEstablishDetails(LCdraftId);
                    model.IsEstablish = isEstablish;
                    ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(88);
                    return View("EstablishLC", model);
                }
                if (LCdraftId > 0 && isEditEstablish == true && Common.ValidateLCEstablishOnEdit(LCdraftId))
                {
                    ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                    model = coreAccountService.GetLCEstablishDetailsbyId(LCdraftId);
                    //model.IsEditEstablish = isEditEstablish;
                    ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                    return View("EstablishLC", model);
                }
                else
                {
                    using (var context = new IOASDBEntities())
                    {
                        var conditions = context.tblLCDraftAdditionalConditions.FirstOrDefault(m => m.Id == 1);
                        if (conditions != null)
                        {
                            model.AddtnlConditionsContent = conditions.Condition;
                        }
                    }
                    //model.RequestDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);

                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("LCOpeningList", "CoreAccounts");
            }

        }
        public ActionResult LCOpeningEstablishView(int LCdraftId = 0)
        {

            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(88);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                LCOpeningModel model = new LCOpeningModel();

                model = coreAccountService.GetLCEstablishDetailsbyId(LCdraftId);
                ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.disabled = "Disabled";

                decimal amount = model.LCDraftAmount ?? 0;
                var type = model.ProjectCategory;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(88, type, amount);
                //ViewBag.processGuideLineId = 206;
                TempData["viewMode"] = "ViewOnly";
                return View("EstablishLCView", model);


            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult LCOpening(LCOpeningModel model, string content = null, string addconditions = null)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                //ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(88);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                //ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (model.LCOpeningId > 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (model.IsEstablish == true || model.IsEditEstablish == true)
                        {
                            string validationMsg = ValidateLCOpening(model);
                            if (validationMsg != "Valid")
                            {
                                TempData["errMsg"] = validationMsg;
                                return View(model);
                            }
                        }
                        if (model.Document != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(model.Document.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                        foreach (var item in model.DocumentDetail)
                        {
                            if (item.DocumentFile != null)
                            {
                                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                                string filename = Path.GetFileName(item.DocumentFile.FileName);
                                var docextension = Path.GetExtension(filename);
                                if (!allowedExtensions.Contains(docextension))
                                {
                                    TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                    return View(model);
                                }
                            }
                        }

                        int logged_in_user = Common.GetUserid(User.Identity.Name);
                        var result = coreAccountService.CreateLCOpening(model, logged_in_user, content, addconditions);
                        if (model.LCOpeningId == 0 && result.Item1 > 0)
                        {
                            //TempData["succMsg"] = "LC Opening has been done successfully.";
                            //return RedirectToAction("LCOpeningList");
                            TempData["succMsg"] = "Success";
                            return Json(new { TempData, msg = result.Item2 }, JsonRequestBehavior.AllowGet);

                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                        }
                        else if (model.LCOpeningId > 0 && result.Item1 > 0 && model.IsEditMode == true)
                        {
                            //TempData["succMsg"] = "LC Opening has been updated successfully.";
                            //return RedirectToAction("LCOpeningList");
                            TempData["succMsg"] = "Updated";
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return Json(new { TempData, msg = result.Item2 }, JsonRequestBehavior.AllowGet);

                        }
                        else if (model.LCOpeningId > 0 && result.Item1 > 0 && (model.IsEstablish == true || model.IsEditEstablish == true))
                        {
                            ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                            //TempData["succMsg"] = "LC Draft has been Established successfully.";
                            //return RedirectToAction("LCOpeningList");
                            //TempData["succMsg"] = "Established";
                            //return Json(TempData, JsonRequestBehavior.AllowGet);
                            return RedirectToAction("LCOpeningList", new { msg = "Established" });
                        }
                        else
                            TempData["errMsg"] = result.Item2;

                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                    }
                }
                else
                {
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    var result = coreAccountService.CreateLCOpening(model, logged_in_user, content, addconditions);
                    if (model.LCOpeningId == 0 && result.Item1 > 0)
                    {
                        TempData["succMsg"] = "Success";
                        // return RedirectToAction("LCOpeningList");
                        // return Json(Url.Action("LCOpeningList", "Home"));
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return Json(new { TempData, msg = result.Item2 }, JsonRequestBehavior.AllowGet);

                    }
                    else if (model.LCOpeningId > 0 && result.Item1 > 0 && model.IsEditMode == true)
                    {
                        //TempData["succMsg"] = "LC Opening has been updated successfully.";
                        //return RedirectToAction("LCOpeningList");
                        TempData["succMsg"] = "Updated";
                        //return Json(TempData, JsonRequestBehavior.AllowGet);
                        return Json(new { TempData, msg = result.Item2 }, JsonRequestBehavior.AllowGet);

                    }

                    else
                        TempData["succMsg"] = "Error";
                    //return Json(TempData, JsonRequestBehavior.AllowGet);
                    return Json(new { TempData, msg = result.Item2 }, JsonRequestBehavior.AllowGet);

                    //TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                // ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }

        private string ValidateLCOpening(LCOpeningModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal paymentamt = model.LCDraftAmount ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (paymentamt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        //[HttpGet]
        //public JsonResult GetLCOpeningList()
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetLCOpeningList();
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //[HttpGet]
        //public JsonResult SearchLCOpeningList(LCOpeningModel model)
        //{
        //    object output = coreAccountService.SearchLCOpeningList(model);
        //    //object output = "";
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public ActionResult LCSubmitforApproval(int LCOpeningId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.LCOpeningCommitmentBalanceUpdate(LCOpeningId, false, false, userId, "LCO");

                //if (!cStatus)
                //{
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    using (var context = new IOASDBEntities())
                //    {
                //        var query = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == LCOpeningId && m.Status == "Establish LC Open" && m.TransactionTypeCode == "LCO");
                //        if (query != null)
                //        {
                //            query.Status = "Establish LC Approval Pending";
                //            query.UPTD_By = userId;
                //            query.UPTD_TS = DateTime.Now;
                //            context.SaveChanges();
                //        }
                //    }
                //}

                bool cStatus = false;
                if (Common.ValidateLCOpeningStatus(LCOpeningId, "Establish LC Open"))
                {
                    var transCode = "LCO";
                    cStatus = coreAccountService.LCOpeningCommitmentBalanceUpdate(LCOpeningId, false, false, userId, transCode);
                    if (!cStatus)
                        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    bool status = coreAccountService.LCOpeningWFInit(LCOpeningId, userId, transCode);
                    if (!status)
                        coreAccountService.LCOpeningCommitmentBalanceUpdate(LCOpeningId, true, false, userId, transCode);
                    return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult LCOpeningApprove(int LCOpeningId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.LCOpeningCommitmentBalanceUpdate(LCOpeningId, false, false, userId, "LCO");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.LCOpeningBillApproved(LCOpeningId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetLCOpeningList(int pageIndex, int pageSize, SearchLCOpeningModel model)
        {
            try
            {
                object output = coreAccountService.GetLCOpeningList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLCDocumentInfoDetails(string DespatchMode)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            DespatchMode = DespatchMode == "" ? "0" : DespatchMode;
            var locationdata = _cs.getLCDocInfo(Convert.ToInt32(DespatchMode));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetBankAccountNumber(string BankId)
        {
            try
            {
                BankId = BankId == "" ? "0" : BankId;
                object output = Common.GetBankAccountNumber(Convert.ToInt32(BankId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult LCOpeningWFInit(int BillId)
        {
            try
            {
                lock (LCopenWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateLCOpeningStatus(BillId, "Establish LC Open"))
                    {
                        var transCode = "LCO";
                        bool cStatus = coreAccountService.LCOpeningCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.LCOpeningWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.LCOpeningCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region LC Ammendment 
        public ActionResult LCAmmendmentList()
        {
            return View();
        }
        public ActionResult LCAmmendment(int LCAmmendId = 0, int LCDraftId = 0)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(89);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                LCOpeningModel model = new LCOpeningModel();
                if (LCAmmendId > 0 && Common.ValidateLCAmmendmentOnEdit(LCAmmendId))
                {
                    model = coreAccountService.GetLCAmendDetailsbyId(LCAmmendId);
                }

                else if (LCDraftId > 0 && LCAmmendId == 0)
                {
                    model = coreAccountService.GetLCAmendDetails(LCDraftId);
                    model.CheckListDetail = Common.GetCheckedList(89);
                    model.NeedUpdateTransDetail = true;
                }
                else
                {
                    //model.RequestDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                    model.CheckListDetail = Common.GetCheckedList(89);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("LCAmmendmentList", "CoreAccounts");
            }

        }

        public ActionResult LCAmendmentView(int LCAmendId)
        {

            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(89);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                LCOpeningModel model = new LCOpeningModel();

                model = coreAccountService.GetLCAmendDetailsbyId(LCAmendId);

                ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.disabled = "Disabled";
                decimal amount = model.LCDraftAmount ?? 0;
                var type = model.ProjectCategory;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(89, type, amount);
                //ViewBag.processGuideLineId = 207;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult LCAmmendment(LCOpeningModel model)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(89);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);

                if (ModelState.IsValid)
                {

                    string validationMsg = ValidateLCAmmend(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateLCAmmendment(model, logged_in_user);
                    if (model.LCAmmendmentId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "LC Amendment has been done successfully.";
                        return RedirectToAction("LCAmmendmentList");
                    }
                    else if (model.LCAmmendmentId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "LC Amendment has been updated successfully.";
                        return RedirectToAction("LCAmmendmentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateLCAmmend(LCOpeningModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            //decimal paymentamt = model.LCDraftAmount ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (paymentamt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        //[HttpGet]
        //public JsonResult GetLCAmmendList()
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetLCOpeningList();
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //[HttpGet]
        //public JsonResult SearchLCOpeningList(LCOpeningModel model)
        //{
        //    object output = coreAccountService.SearchLCOpeningList(model);
        //    //object output = "";
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public ActionResult LCAmmendSubmitforApproval(int LCAmmendId)
        {
            try
            {
                //int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.LCAmmendCommitmentBalanceUpdate(LCAmmendId, false, false, userId, "LCA");
                //if (!cStatus)
                //{
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    using (var context = new IOASDBEntities())
                //    {
                //        var query = context.tblLCAmmendment.FirstOrDefault(m => m.Id == LCAmmendId && m.Status == "Amendment Open" && m.TransactionTypeCode == "LCA");

                //        if (query != null)
                //        {
                //            query.Status = "Amendment Approval Pending";
                //            query.UPTD_By = userId;
                //            query.UPTD_TS = DateTime.Now;
                //            context.SaveChanges();
                //            var lcdraftid = query.LCOpeningId;
                //            var LCquery = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == LCAmmendId);
                //            LCquery.Status = "Amendment Approval Pending";
                //            LCquery.UPTD_By = userId;
                //            LCquery.UPTD_TS = DateTime.Now;
                //            context.SaveChanges();
                //        }
                //    }
                //}
                ////bool status = coreAccountService.LCAmmendBillApproved(LCAmmendId, userId);
                //return Json(new { status = cStatus, msg = !cStatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

                lock (LCAmdWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateLCAmmendmentStatus(LCAmmendId, "Amendment Open"))
                    {
                        var transCode = "LCA";
                        bool cStatus = coreAccountService.LCAmmendCommitmentBalanceUpdate(LCAmmendId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.LCAmmendmentWFInit(LCAmmendId, userId, transCode);
                        if (!status)
                            coreAccountService.LCAmmendCommitmentBalanceUpdate(LCAmmendId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult LCAmmendApprove(int LCAmmendId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.LCAmmendCommitmentBalanceUpdate(LCAmmendId, false, false, userId, "LCA");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.LCAmmendBillApproved(LCAmmendId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetLCAmmendList(int pageIndex, int pageSize, SearchLCOpeningModel model)
        {
            try
            {
                object output = coreAccountService.GetLCAmmendList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadLCNumberList(string term, int? type = null, int? classification = null)
        {
            try
            {
                var data = Common.GetAutoCompleteLCNumberList(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult _ViewLCDetails(int LCOpeningid)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            LCOpeningModel model = new LCOpeningModel();
            model = _cs.GetLCEstablishDetails(LCOpeningid);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult LCAmmendmentWFInit(int BillId)
        {
            try
            {
                lock (LCAmdWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateLCAmmendmentStatus(BillId, "Amendment Open"))
                    {
                        var transCode = "LCA";
                        bool cStatus = coreAccountService.LCAmmendCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.LCAmmendmentWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.LCAmmendCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region LC Retirement 
        public ActionResult LCRetirementList()
        {
            return View();
        }
        public ActionResult LCRetirement(int LCRetirementId = 0, int LCDraftId = 0)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(90);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                LCOpeningModel model = new LCOpeningModel();
                if (LCRetirementId > 0 && Common.ValidateLCRetirementOnEdit(LCRetirementId))
                {
                    model = coreAccountService.GetLCRetirementDetailsbyId(LCRetirementId);
                }

                else if (LCDraftId > 0 && LCRetirementId == 0)
                {
                    model = coreAccountService.GetLCRetirementDetails(LCDraftId);
                    model.CheckListDetail = Common.GetCheckedList(90);
                    model.NeedUpdateTransDetail = true;
                    if (model.LCRetirementId == -2)
                    {
                        TempData["errMsg"] = "LC Retirement cannot be done since Retirement has already been done for 100 % LC Value";
                        return RedirectToAction("LCRetirementList", "CoreAccounts");
                    }
                }
                else
                {
                    //model.RequestDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                    model.CheckListDetail = Common.GetCheckedList(90);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("LCRetirementList", "CoreAccounts");
            }

        }

        public ActionResult LCRetirementView(int LCRetirementId)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(90);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                LCOpeningModel model = new LCOpeningModel();

                model = coreAccountService.GetLCRetirementDetailsbyId(LCRetirementId);

                ViewBag.CommitmentNumberList = Common.GetForgnCommitmentNumberList(Convert.ToInt32(model.ProjectId));
                ViewBag.disabled = "Disabled";
                decimal amount = model.LCRetireEquivalentINRValue ?? 0;
                var type = model.ProjectCategory;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(90, type, amount);
                //ViewBag.processGuideLineId = 208;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult LCRetirement(LCOpeningModel model)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(90);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);

                if (ModelState.IsValid)
                {
                    if (model.LCRetirementId > 0 && model.ExpenseDetail != null && model.CommitmentDetail != null)
                    {
                        string validationMsg = ValidateLCRetirement(model);
                        if (validationMsg != "Valid")
                        {
                            TempData["errMsg"] = validationMsg;
                            return View(model);
                        }
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateLCRetirement(model, logged_in_user);
                    if (model.LCRetirementId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "LC Retirement has been done successfully.";
                        return RedirectToAction("LCRetirementList");
                    }
                    else if (model.LCRetirementId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "LC Retirement has been updated successfully.";
                        return RedirectToAction("LCRetirementList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult FinalRetirementofLC(int LCRetirementId = 0, int LCDraftId = 0)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();

                ViewBag.DocmentTypeList = Common.GetDocTypeList(90);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                LCOpeningModel model = new LCOpeningModel();
                if (LCDraftId == 0 && LCRetirementId > 0 && Common.ValidateFinalLCRetirementOnEdit(LCRetirementId))
                {
                    model = coreAccountService.GetFinalLCRetirementDetailsbyId(LCRetirementId);
                }

                else if (LCDraftId > 0 && LCRetirementId > 0)
                {
                    model = coreAccountService.GetLCRetirementDetailsbyId(LCRetirementId);
                    model.CheckListDetail = Common.GetCheckedList(90);
                    model.NeedUpdateTransDetail = true;
                }
                else
                {
                    //model.RequestDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                    model.CheckListDetail = Common.GetCheckedList(90);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("LCRetirementList", "CoreAccounts");
            }

        }
        [HttpPost]
        public ActionResult FinalRetirementofLC(LCOpeningModel model)
        {
            try
            {
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(90);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);

                if (ModelState.IsValid)
                {

                    string validationMsg = ValidateLCRetirement(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateFinalLCRetirement(model, logged_in_user);
                    if (model.LCRetirementId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "LC Retirement has been done successfully.";
                        return RedirectToAction("LCRetirementList");
                    }
                    else if (model.LCRetirementId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Final LC Retirement has been updated successfully.";
                        return RedirectToAction("LCRetirementList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<LCOpeningModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TypeList = Common.GetCodeControlList("LCType");
                ViewBag.CreditAvailBy = Common.GetCodeControlList("CreditAvailableBy");
                ViewBag.TransShipmentOpt = Common.GetCodeControlList("TransShipmentOption");
                ViewBag.PartShipmentOpt = Common.GetCodeControlList("PartialShipmentOption");
                ViewBag.INCOterms = Common.GetCodeControlList("TradeINCOterms");
                ViewBag.DispatchMode = Common.GetCodeControlList("LCModeofDespatch");
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateLCRetirement(LCOpeningModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            //decimal paymentamt = model.LCDraftAmount ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (paymentamt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public ActionResult LCRetirementSubmitforApproval(int LCRetirementId)
        {
            try
            {
                //int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.LCRetireCommitmentBalanceUpdate(LCRetirementId, false, false, userId, "LCR");
                //if (!cStatus)
                //{
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    using (var context = new IOASDBEntities())
                //    {
                //        var query = context.tblLCRetirement.FirstOrDefault(m => m.Id == LCRetirementId && m.Status == "Final Retirement Open" && m.TransactionTypeCode == "LCR");

                //        if (query != null)
                //        {
                //            query.Status = "Retirement Approval Pending";
                //            query.UPTD_By = userId;
                //            query.UPTD_TS = DateTime.Now;
                //            context.SaveChanges();
                //            var lcdraftid = query.LCOpeningId;
                //            var LCquery = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == LCRetirementId);
                //            LCquery.Status = "Retirement Approval Pending";
                //            LCquery.UPTD_By = userId;
                //            LCquery.UPTD_TS = DateTime.Now;
                //            context.SaveChanges();
                //        }
                //    }
                //}
                ////bool status = coreAccountService.LCRetireBillApproved(LCRetirementId, userId);
                //return Json(new { status = cStatus, msg = !cStatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

                lock (LCRetWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateLCRetirementStatus(LCRetirementId, "Final Retirement Open"))
                    {
                        var transCode = "LCR";
                        bool cStatus = coreAccountService.LCRetireCommitmentBalanceUpdate(LCRetirementId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.LCRetirementWFInit(LCRetirementId, userId, transCode);
                        if (!status)
                            coreAccountService.LCRetireCommitmentBalanceUpdate(LCRetirementId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult LCRetirementApprove(int LCRetirementId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.LCRetireCommitmentBalanceUpdate(LCRetirementId, false, false, userId, "LCR");
                //if (!cStatus)
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.LCRetireBillApproved(LCRetirementId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetLCRetirementList(int pageIndex, int pageSize, SearchLCOpeningModel model)
        {
            try
            {
                object output = coreAccountService.GetLCRetirementList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRetireLCNumberList(string term, int? type = null, int? classification = null)
        {
            try
            {
                var data = Common.GetRetireAutoCompleteLCNumberList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult _ViewAmendDetails(int LCAmmendid)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            LCOpeningModel model = new LCOpeningModel();
            model = _cs.GetLCAmendDetailsbyId(LCAmmendid);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult LCRetirementWFInit(int BillId)
        {
            try
            {
                lock (LCRetWFInitlockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateLCRetirementStatus(BillId, "Retirement Open"))
                    {
                        var transCode = "LCR";
                        bool cStatus = coreAccountService.LCRetireCommitmentBalanceUpdate(BillId, false, false, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.LCRetirementWFInit(BillId, userId, transCode);
                        if (!status)
                            coreAccountService.LCRetireCommitmentBalanceUpdate(BillId, true, false, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Bill not submited for approval" : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion



        #endregion
        #region ClaimBill
        public ActionResult ClaimBillList()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                //if (user_role == 8)
                //{
                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.ProjectNumberList = emptyList;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                var data = new PagedData<ClaimBillSearchResultModel>();
                ClaimBillListModel model = new ClaimBillListModel();
                ClaimBillSearchFieldModel srchModel = new ClaimBillSearchFieldModel();
                data = coreAccountService.GetClaimbillList(srchModel, page, pageSize);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);
                //}
                //if (user_role == 7)
                //{
                //    int page = 1;
                //    int pageSize = 5;
                //    ViewBag.PIName = Common.GetPIWithDetails();
                //    var Projecttitle = Common.GetPIProjectdetails(logged_in_userid);
                //    var projecttype = Common.getprojecttype();
                //    var invoicetype = Common.getinvoicetype();
                //    var Invoice = Common.GetInvoicedetails();
                //    ViewBag.Project = Projecttitle;
                //    ViewBag.projecttype = projecttype;
                //    ViewBag.TypeofInvoice = invoicetype;
                //    ViewBag.Invoice = Invoice;
                //    var data = new PagedData<InvoiceSearchResultModel>();
                //    InvoiceListModel model = new InvoiceListModel();
                //    ProjectService _ps = new ProjectService();
                //    InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                //    srchModel.PIName = logged_in_userid;
                //    data = _ps.GetPIInvoiceList(srchModel, page, pageSize);
                //    model.Userrole = user_role;
                //    model.SearchResult = data;
                //    return View(model);
                //}
                //return RedirectToAction("DashBoard", "Home");
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchClaimBillList(ClaimBillSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ClaimBillSearchResultModel>();
                ClaimBillListModel model = new ClaimBillListModel();
                CoreAccountsService _cs = new CoreAccountsService();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-2);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-2);
                //}

                data = _cs.GetClaimbillList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult ClaimBill(int pId = 0)
        {
            try
            {
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.AccGroupId = Common.GetAccountGroup(28);
                ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                ViewBag.state = Common.GetStatelist();
                //if (user_role != 7 && user_role != 8)
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}                    
                ClaimBillModel model = new ClaimBillModel();
                CoreAccountsService _ps = new CoreAccountsService();
                model = _ps.GetProjectDetails(pId);

                if (model.SponsoringAgency == null)
                {
                    ViewBag.errMsg = "Project is not mapped with Agency. Please map any sponosring agency to the project to continue";
                }
                if (model.ProjectID == -1)
                {
                    ViewBag.errMsg = "Project is not Active.";
                }
                if (model.AvailableBalance <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice";
                }
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice for this financial year";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult ClaimBill(ClaimBillModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.state = Common.GetStatelist();
                ViewBag.AccGroupId = Common.GetAccountGroup(28);
                ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "Claim Bill Cannot be generated. No balance available for raising Claim Bill";
                    return View(model);
                }
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");             

                var InvoiceID = coreAccountService.CreateClaimBill(model, loggedinuserid);
                if (InvoiceID == -6)
                {
                    ViewBag.errMsg = "Project is not Active.";
                    return View(model);
                }
                if (InvoiceID == -4)
                {
                    ViewBag.errMsg = "Claim Bill Cannot be generated as the Taxable value has exceeded the balance available for raising Claim Bill. Please enter correct value and try again.";
                    return View(model);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Claim Bill has been created successfully with Claim Bill Number - " + InvoiceNumber + ".";
                }
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceId);
                    ViewBag.succMsg = "Claim Bill with Claim Bill number - " + InvoiceNumber + " has been updated successfully.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        public ActionResult EditClaimBill(int ClaimBillId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.state = Common.GetStatelist();
                ViewBag.AccGroupId = Common.GetAccountGroup(28);
                ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                ClaimBillModel model = new ClaimBillModel();
                ProjectService _ps = new ProjectService();
                model = coreAccountService.GetClaimBillDetails(ClaimBillId);
                if (model.ProjectID == -6)
                {
                    ViewBag.errMsg = "Project is not Active.";
                    return View("ClaimBill", model);
                }
                return View("ClaimBill", model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        public ActionResult ClaimBillView(int ClaimBillId = 0, bool Pfinit = false)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);

                ClaimBillModel model = new ClaimBillModel();
                model = coreAccountService.GetClaimBillViewDetails(ClaimBillId);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                if (model.ProjectID == -6)
                {
                    ViewBag.errMsg = "Project is not Active.";
                    return View(model);
                }
                TempData["viewMode"] = "ViewOnly";
                return View("ClaimBillView", model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSponProjectList(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = coreAccountService.LoadSponProjecttitledetails(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadClaimBillList(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = coreAccountService.LoadClaimBillList(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadTaxpercentage(string servicetype)
        {
            servicetype = servicetype == "" ? "0" : servicetype;
            object output = coreAccountService.gettaxpercentage(Convert.ToInt32(servicetype));
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Honororium
        public ActionResult HonororiumView(int HonorId, bool Pfinit = false)
        {
            try
            {
                FinOp fo = new FinOp(System.DateTime.Now);
                DateTime Today = System.DateTime.Now;
                //ViewBag.HonDate = fo.GetAllMonths();
                var emptyList = new List<HonororiumModel>();

                HonororiumModel model = new HonororiumModel();
                model.CreditorType = "PI/Student/Others";
                model = coreAccountService.GetHonororiumViewDetails(HonorId);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(63, "Others", model.NetTotal ?? 0);
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();

            }

        }
       
        [HttpGet]
        public ActionResult Honororium(int HonorId = 0)
        {
            try
            {
                FinOp fo = new FinOp(System.DateTime.Now);
                DateTime Today = System.DateTime.Now;
                //ViewBag.HonDate = fo.GetAllMonths();
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.HonDateList = fo.GetAllMonths();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("HonorCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.HonTdsSection = Common.GetHonororiumTdsSection();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                HonororiumModel model = new HonororiumModel();
                model.CreditorType = "PI/Student/Others";
                if (HonorId > 0 && Common.ValidateHonororiumOnEdit(HonorId))
                {
                    model = coreAccountService.GetHonororiumDetails(HonorId);

                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(63);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult Honororium(HonororiumModel model)
        {
            try
            {
                FinOp fo = new FinOp(System.DateTime.Now);
                DateTime Today = System.DateTime.Now;
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("HonorCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.HonDateList = fo.GetAllMonths();
                ViewBag.HonTdsSection = Common.GetHonororiumTdsSection();
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {

                    string validationMsg = ValidateHonororium(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateHonororium(model, logged_in_user);
                    if (model.HonororiumId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Honororium has been added successfully.";
                        return RedirectToAction("HonororiumList");
                    }
                    else if (model.HonororiumId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Honororium has been updated successfully.";
                        return RedirectToAction("HonororiumList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                }

                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
    (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        public ActionResult HonororiumList()
        {
            return View();
        }
        //[HttpGet]
        //public JsonResult GetHonororiumList()
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetHonororiumList();
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public JsonResult ApprovalForHonororium(int HonorId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                lock (HONWFInitlockObj)
                {
                    bool cStatus = coreAccountService.HonororiumCommitmentBalanceUpdate(HonorId, false, false, logged_in_user, "HON");
                    if (!cStatus)
                        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    var result = coreAccountService.HonororiumWFInit(HonorId, logged_in_user);
                    if (!result.Item1)
                        coreAccountService.HonororiumCommitmentBalanceUpdate(HonorId, true, false, logged_in_user, "HON");
                    return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForHonororium(int HonorId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.HonororiumBillApproved_OF(HonorId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                //bool boaStatus = coreAccountService.HonororiumBOATransaction(HonorId);
                //var status = Common.ApprovalPendingForHonororium(HonorId, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        private string ValidateHonororium(HonororiumModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
            {
                msg = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                return msg;
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult SearchHonororiumList(honororiumSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchHonororiumList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetHonororiumList(SearchHonororium model, int pageIndex, int pageSize, DateFilterModel HonororiumDate)
        {
            try
            {
                object output = coreAccountService.GetHonororiumList(model, pageIndex, pageSize, HonororiumDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        #endregion
        #region TDS Payment
        public ActionResult TDSPayment(int TDSPaymentId = 0, bool payment = false)
        {
            try
            {
                ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
                ViewBag.SectionList = Common.Section();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                TDSPaymentModel model = new TDSPaymentModel();
                if (TDSPaymentId > 0 && Common.ValidateTDSPaymentOnEdit(TDSPaymentId))
                {
                    model = coreAccountService.GetTDSPaymentDetails(TDSPaymentId, payment);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult TDSPayment(TDSPaymentModel model)
        {
            var emptyList = new List<TDSPaymentModel>();
            ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
            ViewBag.SectionList = Common.Section();
            ViewBag.BankList = Common.GetBankAccountHeadList();
            if (ModelState.IsValid)
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateTDSPayment(model, logged_in_user);
                if (model.TDSPaymentId == 0 && result > 0)
                {
                    TempData["succMsg"] = "TDSPayment has been added successfully.";
                    return RedirectToAction("TDSPaymentList");
                }
                else if (model.TDSPaymentId > 0 && result > 0)
                {
                    TempData["succMsg"] = "TDSPayment has been updated successfully.";
                    return RedirectToAction("TDSPaymentList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSection()
        {
            var locationdata = Common.GetSection();
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTDSIncomeTaxList(DateTime fromdate, DateTime todate, int headid, int bankid, int TDSPaymentId)
        {
            try
            {
                object output = CoreAccountsService.GetTDSIncomeTaxList(fromdate, todate, headid, bankid, TDSPaymentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetTDSGSTList(DateTime fromdate, DateTime todate, int bankid, int TDSPaymentId)
        {
            try
            {
                object output = CoreAccountsService.GetTDSGSTList(fromdate, todate, bankid, TDSPaymentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult GetTDSPaymentList()
        {
            try
            {
                object output = coreAccountService.GetTDSPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalForTDSPayment(int TDSPaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForTDSPayment(TDSPaymentId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForTDSPayment(int TDSPaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.TDSPaymentBOATransaction(TDSPaymentId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);

                var status = Common.ApprovalPendingForTDSPayment(TDSPaymentId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult TDSPaymentView(int TDSPaymentId = 0, bool payment = false)
        {
            try
            {
                ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
                ViewBag.SectionList = Common.Section();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                TDSPaymentModel model = new TDSPaymentModel();
                model = coreAccountService.GetTDSPaymentDetails(TDSPaymentId, payment);
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 83;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult TDSPaymentWFInit(int id)
        {
            try
            {
                lock (TDSWFInitlockObj)
                {
                    if (Common.ValidateTDSPaymentStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        var result = coreAccountService.TDSPaymentWFInit(id, userId);
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SearchTDSPaymentList(TDSPaymentSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchTDSPaymentList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTDSPaymentList(SearchTDSPayment model, int pageIndex, int pageSize, DateFilterModel TDSPaymentDate)
        {
            try
            {
                object output = coreAccountService.GetTDSPaymentList(model, pageIndex, pageSize, TDSPaymentDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult TDSPaymentList()
        {
            ViewBag.BankList = Common.GetBankAccountHeadList();
            return View();
        }
        [HttpPost]
        public JsonResult GetTXPtransactionDetail(int BankHead = 0, int ITtds = 0)
        {
            try
            {
                object output = Common.GetTXPtransactionDetail(BankHead, ITtds);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        #endregion
        #region  FellowShip
        public ActionResult FellowShipList(int FellowId = 0, bool view = false, bool edit = false, bool revise = false, bool revisededit = false)
        {
            var emptyList = new List<FellowShipModel>();
            ViewBag.PayeeName =
            ViewBag.AvailBalance =
            ViewBag.CommitmentNumbr = emptyList;

            FellowShipModel model = new FellowShipModel();


            if (FellowId > 0 && edit == true && Common.ValidateFellowShipOnEdit(FellowId))
            {
                model = coreAccountService.GetFellowShipDetails(FellowId, view, edit, revise, revisededit);
            }
            else if (FellowId > 0)
            {
                model = coreAccountService.GetFellowShipDetails(FellowId, view, edit, revise, revisededit);
            }
            if (FellowId > 0)
            {
                //ViewBag.PayeeName = Common.GetPIname(Convert.ToInt32(model.ProjectId));
                ViewBag.CommitmentNumbr = Common.GetCommitmentNo(Convert.ToInt32(model.ProjectId));
            }
            return View(model);

        }
        [HttpPost]
        public ActionResult FellowShipList(FellowShipModel model)
        {
            var emptyList = new List<FellowShipModel>();
            ViewBag.PayeeName =
            ViewBag.AvailBalance =
            ViewBag.CommitmentNumbr = emptyList;
            if (ModelState.IsValid)
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateFellowShip(model, logged_in_user);
                if (model.FellowShipId == 0 && result > 0)
                {
                    TempData["succMsg"] = "FellowShip has been added successfully.";
                    return RedirectToAction("FellowShipList");
                }
                else if (model.FellowShipId > 0 && result > 0)
                {
                    TempData["succMsg"] = "FellowShip has been updated successfully.";
                    return RedirectToAction("FellowShipList");
                }
                else
                {
                    if (model.FellowShipId > 0 && result == -3)
                        TempData["errMsg"] = "This already Posted in Fellowship Salary.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadProjectNumber(string term, int? type = null)
        {
            try
            {
                var data = Common.GetProjectNumber(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPIname(string term)
        {
            var locationdata = Common.GetAutoCompletePIWithDetails(term);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCommitmentNo(int projid)
        {
            var locationdata = Common.GetCommitmentNo(projid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAvailableBalance(string commitmentno)
        {
            var locationdata = Common.GetAvailableBalance(commitmentno);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFellowShipList()
        {
            try
            {
                object output = coreAccountService.GetFellowShipList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalForFellowShip(int FellowId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForFellowShip(FellowId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForFellowShip(int FellowId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalPendingForFellowShip(FellowId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult InActiveForFellowShip(int FellowId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.InActiveForFellowShip(FellowId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchFellowShipList(FellowShipSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchFellowShipList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFellowShipList(SearchFellowShip model, int pageIndex, int pageSize, DateFilterModel FellowShipDate)
        {
            try
            {
                object output = coreAccountService.GetFellowShipList(model, pageIndex, pageSize, FellowShipDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        #endregion
        #region Institute Claims
        public ActionResult InstituteClaimsView(int InsClaimId = 0, bool receipt = false)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                   ViewBag.CommitmentNumbr =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteClaims model = new InstituteClaims();
                model.CreditorType = "NA";
                if (InsClaimId > 0)
                {
                    model = coreAccountService.GetInstitueClaimsDetails(InsClaimId, receipt);
                }
                if (InsClaimId > 0)
                {

                    ViewBag.CommitmentNumbr = Common.GetComitmentNo(Convert.ToInt32(model.Projectid));
                }
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        public ActionResult InstituteClaims(int InsClaimId = 0, bool receipt = false)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumbr =
                    ViewBag.AccountHeadList = emptyList;

                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteClaims model = new InstituteClaims();
                model.CreditorType = "NA";
                if (InsClaimId > 0)
                {
                    model = coreAccountService.GetInstitueClaimsDetails(InsClaimId, receipt);
                }
                if (InsClaimId > 0)
                {

                    ViewBag.CommitmentNumbr = Common.GetComitmentNo(Convert.ToInt32(model.Projectid));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult InstituteClaims(InstituteClaims model)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                 ViewBag.CommitmentNumbr =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "NA";

                int logged_in_user = Common.GetUserid(User.Identity.Name);
                if (model.ReceiptId == 1)
                {
                    bool res = coreAccountService.CreateReceiptInstitueClaims(model, logged_in_user);
                    if (model.InstituteClaimId > 0 && res == true)
                    {
                        TempData["succMsg"] = "Receipt has been added successfully.";
                        return RedirectToAction("InstituteClaimsList");
                    }
                    else
                    {
                        TempData["succMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("InstituteClaimsList");
                    }
                }
                int result = coreAccountService.CreateInstitueClaims(model, logged_in_user);
                if (model.InstituteClaimId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Institute Claims has been added successfully.";
                    return RedirectToAction("InstituteClaimsList");
                }
                else if (model.InstituteClaimId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Institute Claims has been updated successfully.";
                    return RedirectToAction("InstituteClaimsList");
                }


                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        //public ActionResult InstituteClaimsReceipt(int InsClaimId = 0)

        //{
        //    var emptyList = new List<InstituteClaims>();
        //    ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
        //    ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
        //    ViewBag.BudHed = Common.getallocationhead();
        //    ViewBag.SourceList = Common.GetSourceList();
        //    ViewBag.SourceRefNumberList = emptyList;
        //    ViewBag.PIName = Common.GetPIWithDetails();
        //    ViewBag.Project = Common.GetProjectNumberList();
        //    ViewBag.Department = Common.getDepartment();
        //    ViewBag.Student = Common.GetStudentList();
        //    ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
        //    ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
        //    ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
        //    ViewBag.SourceRefNumberList =
        //    ViewBag.AccountGroupList =
        //    ViewBag.TypeOfServiceList =
        //    ViewBag.AccountHeadList = emptyList;
        //    ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //    ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
        //    var ptypeList = Common.getprojecttype();
        //    int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
        //    ViewBag.ProjectTypeList = ptypeList;
        //    ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
        //    InstituteClaims model = new InstituteClaims();
        //    model.CreditorType = "PI/Student/Others";
        //    model.InstituteClaimId = InsClaimId;
        //    model.ReceiptValue = Common.GetClaimValue(InsClaimId);
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult InstituteClaimsReceipt(InstituteClaims model)
        //{
        //    try
        //    {
        //        var emptyList = new List<InstituteClaims>();
        //        ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
        //        ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
        //        ViewBag.BudHed = Common.getallocationhead();
        //        ViewBag.SourceList = Common.GetSourceList();
        //        ViewBag.SourceRefNumberList = emptyList;
        //        ViewBag.PIName = Common.GetPIWithDetails();
        //        ViewBag.Project = Common.GetProjectNumberList();
        //        ViewBag.Department = Common.getDepartment();
        //        ViewBag.Student = Common.GetStudentList();
        //        ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
        //        ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
        //        ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
        //        ViewBag.SourceRefNumberList =
        //        ViewBag.AccountGroupList =
        //        ViewBag.TypeOfServiceList =
        //        ViewBag.AccountHeadList = emptyList;
        //        ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //        ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
        //        var ptypeList = Common.getprojecttype();
        //        if (model.ExpenseDetail != null)
        //        {
        //            foreach (var item in model.ExpenseDetail)
        //            {
        //                int headId = item.AccountGroupId ?? 0;
        //                item.AccountGroupList = Common.GetAccountGroup(headId);
        //                item.AccountHeadList = Common.GetAccountHeadList(headId);
        //            }
        //        }
        //        int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
        //        ViewBag.ProjectTypeList = ptypeList;
        //        ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
        //        model.CreditorType = "NA";
        //        int logged_in_user = Common.GetUserid(User.Identity.Name);
        //        int result = coreAccountService.CreateReceiptInstitueClaims(model, logged_in_user);
        //        if (model.InstituteClaimId > 0 && result > 0)
        //        {
        //            TempData["succMsg"] = "Receipt has been added successfully.";
        //            return RedirectToAction("InstituteClaimsList");
        //        }

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        var emptyList = new List<MasterlistviewModel>();
        //        ViewBag.SourceList = Common.GetSourceList();
        //        ViewBag.SourceRefNumberList =
        //        ViewBag.TravellerList = emptyList;
        //        ViewBag.projecttype = Common.getprojecttype();
        //        ViewBag.CountryList = Common.getCountryList();
        //        ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //        TempData["errMsg"] = "Something went wrong please contact administrator.";
        //        return View(model);
        //    }
        //}
        public ActionResult InstituteClaimsList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetInstituteClaimsList()
        {
            try
            {
                object output = coreAccountService.GetInstitueClaimsList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalForInstituteClaims(int InsClaimId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var status = Common.ApprovalForInstituteClaims(InsClaimId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForInstituteClaims(int InsClaimId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.InstitueClaimsBOATransaction(InsClaimId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalPendingForInstituteClaims(InsClaimId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSpendBalance(int commitmentid)
        {
            var locationdata = Common.GetSpendBalance(commitmentid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetComitmentNo(int projid)
        {
            var locationdata = Common.GetComitmentNo(projid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetInstituteClaimsList(SearchInstituteClaims model, int pageIndex, int pageSize, DateFilterModel InstituteClaimDate)
        {
            try
            {
                object output = coreAccountService.GetInstitueClaimsList(model, pageIndex, pageSize, InstituteClaimDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        #endregion
        #endregion
        #region Fellowship Salary
        [HttpPost]
        public JsonResult GetFellowshipSalaryList(string Month)
        {
            try
            {
                object output = CoreAccountsService.GetFellowShipSalaryList(Month);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetEditFellowshipSalaryList(int PaymentId)
        {
            try
            {
                object output = coreAccountService.GetFellowShipSalaryDetails(PaymentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult FellowshipSalary(int FellowshipsalId = 0)
        {
            try
            {
                var emptyList = new List<FellowshipSalaryModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                FellowshipSalaryModel model = new FellowshipSalaryModel();
                model.CreditorType = "PI/Student/Others";
                if (FellowshipsalId > 0)
                {
                    model = coreAccountService.GetFellowShipSalaryDetails(FellowshipsalId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(63);
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult FellowshipSalary(FellowshipSalaryModel model)
        {
            try
            {
                var emptyList = new List<FellowshipSalaryModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                model.CreditorType = "PI/Student/Others";
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                foreach (var item in model.DocumentDetail)
                {
                    if (item.DocumentFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(item.DocumentFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                }
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateFellowshipSalary(model, logged_in_user);
                if (model.FellowshipSalId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Fellowship Salary  has been added successfully.";
                    return RedirectToAction("FellowshipSalaryList");
                }
                else if (model.FellowshipSalId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Fellowship Salary has been updated successfully.";
                    return RedirectToAction("FellowshipSalaryList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        public ActionResult FellowshipSalaryView(int FellowshipsalId = 0, bool Pfinit = false)
        {
            try
            {
                var emptyList = new List<FellowshipSalaryModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                FellowshipSalaryModel model = new FellowshipSalaryModel();
                model.CreditorType = "PI/Student/Others";
                if (FellowshipsalId > 0)
                {
                    model = coreAccountService.GetFellowShipSalaryDetails(FellowshipsalId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(63);
                    model.NeedUpdateTransDetail = true;
                }
                ViewBag.disabled = "Disabled";
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(85, "Others", model.TotalAmount);
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        public JsonResult ApprovalForFellowshipSalary(int FellowshipsalId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cStatus = coreAccountService.FellowShipSalaryCommitmentBalanceUpdate(FellowshipsalId, false, false, logged_in_user, "FSS");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                var result = coreAccountService.FellowShipSalaryWFInit(FellowshipsalId, logged_in_user);
                if (!result.Item1)
                    coreAccountService.FellowShipSalaryCommitmentBalanceUpdate(FellowshipsalId, true, false, logged_in_user, "FSS");
                return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult FellowshipSalaryWFInit(int id)
        {
            try
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                lock (FSSWFInitlockObj)
                {
                    if (Common.ValidateFellowshipsalaryStatus(id, "Open"))
                    {
                        bool cStatus = coreAccountService.FellowShipSalaryCommitmentBalanceUpdate(id, false, false, logged_in_user, "FSS");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.FellowShipSalaryWFInit(id, logged_in_user);
                        if (!result.Item1)
                            coreAccountService.FellowShipSalaryCommitmentBalanceUpdate(id, true, false, logged_in_user, "FSS");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForFellowshipSalary(int FellowshipsalId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.FellowShipSalaryBillApproved(FellowshipsalId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult FellowshipSalaryList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetFellowshipSalList()
        {
            try
            {
                object output = coreAccountService.GetFellowShipSalList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [Authorized]
        [HttpPost]
        public ActionResult GetEditFellowship(int FellowshipId = 0)
        {

            object output = CoreAccountsService.GetEditFellowship(FellowshipId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFellowshipSalList(SearchFellowshipSalary model, int pageIndex, int pageSize, DateFilterModel FellowShipDate)
        {
            try
            {
                object output = coreAccountService.GetFellowShipSalList(model, pageIndex, pageSize, FellowShipDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        #endregion
        #region Institute Salary Payment
        [HttpPost]
        public JsonResult GetInstituteSalaryPaymentList()
        {
            try
            {
                object output = CoreAccountsService.GetInstituteSalaryPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetEditInstituteSalaryPaymentList(int PaymentId = 0)
        {
            try
            {
                object output = coreAccountService.GetInstituteSalaryPaymentDetails(PaymentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult InstituteSalaryPayment(int PaymentId = 0)
        {
            try
            {
                var emptyList = new List<InstituteSalaryPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(86);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteSalaryPaymentModel model = new InstituteSalaryPaymentModel();
                model.CreditorType = "PI/Student/Others";
                if (PaymentId > 0)
                {
                    model = coreAccountService.GetInstituteSalaryPaymentDetails(PaymentId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(86);
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        public ActionResult InstituteSalaryPaymentView(int PaymentId = 0)
        {
            try
            {
                var emptyList = new List<InstituteSalaryPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(86);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteSalaryPaymentModel model = new InstituteSalaryPaymentModel();
                model.CreditorType = "PI/Student/Others";
                if (PaymentId > 0)
                {
                    model = coreAccountService.GetInstituteSalaryPaymentDetails(PaymentId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(86);
                    model.NeedUpdateTransDetail = true;
                }
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult InstituteSalaryPayment(InstituteSalaryPaymentModel model)
        {
            try
            {
                var emptyList = new List<InstituteSalaryPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(86);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                model.CreditorType = "PI/Student/Others";
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                foreach (var item in model.DocumentDetail)
                {
                    if (item.DocumentFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(item.DocumentFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                }
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateInstituteSalaryPayment(model, logged_in_user);
                if (model.InstituteSalaryPaymentId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Institute Salary Payment  has been added successfully.";
                    return RedirectToAction("InstituteSalaryPaymentList");
                }
                else if (model.InstituteSalaryPaymentId > 0 && result > 0)
                {
                    TempData["succMsg"] = " Institute Salary Payment has been updated successfully.";
                    return RedirectToAction("InstituteSalaryPaymentList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        public ActionResult InstituteSalaryPaymentList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetInstituteSalaryList()
        {
            try
            {
                object output = coreAccountService.GetInstituteSalaryList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalForInstituteSalary(int PaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {

                var status = coreAccountService.ApprovalForInstituteSalary(PaymentId, logged_in_user);

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForInstituteSalary(int PaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool status = coreAccountService.InstituteSalaryBOATransaction(PaymentId, logged_in_user);

                // return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                //bool boaStatus = coreAccountService.HonororiumBOATransaction(HonorId);
                //var status = Common.ApprovalPendingForHonororium(HonorId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetInstituteSalaryList(SearchInstituteSalary model, int pageIndex, int pageSize, DateFilterModel InstituteSalaryDate)
        {
            try
            {
                object output = coreAccountService.GetInstituteSalaryList(model, pageIndex, pageSize, InstituteSalaryDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetInstituteSalaryGridList(SearchGridInstituteSalary model, int pageIndex, int pageSize, DateFilterModel InstituteSalaryDate)
        {
            try
            {
                object output = coreAccountService.GetInstituteSalaryGridList(model, pageIndex, pageSize, InstituteSalaryDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetInstituteSalaryAmount(string Month)
        {
            try
            {
                object output = coreAccountService.GetInstituteSalaryAmount(Month);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        #endregion
        #region ManDay
        public ActionResult ManDay(int Mandayid = 0)
        {
            try
            {
                var emptyList = new List<ManDayModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymtMode = Common.GetCodeControlList("PaymentModeManDay");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ManDayModel model = new ManDayModel();
                model.CreditorType = "PI/Student/Others";
                if (Mandayid > 0 && Common.ValidateManDayOnEdit(Mandayid))
                {
                    model = coreAccountService.GetManDayDetails(Mandayid);

                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(77);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ManDay(ManDayModel model)
        {
            try
            {
                var emptyList = new List<ManDayModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }


                foreach (var item in model.DocumentDetail)
                {
                    if (item.DocumentFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(item.DocumentFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                }

                if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
                {
                    TempData["errMsg"] = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                    return View(model);
                }
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateManDay(model, logged_in_user);
                if (model.ManDayId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Mandays has been added successfully.";
                    return RedirectToAction("ManDayList");
                }
                else if (model.ManDayId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Mandays has been updated successfully.";
                    return RedirectToAction("ManDayList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);

                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        public ActionResult ManDayView(int Mandayid, bool Pfinit = false)
        {
            try
            {

                ManDayModel model = new ManDayModel();
                model.CreditorType = "PI/Student/Others";
                model = coreAccountService.GetManDayDetailsView(Mandayid);
                model.PFInit = Pfinit;
                ViewBag.disabled = "Disabled";
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(77, "Others", model.TotalAmount ?? 0);
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();

            }

        }
        [HttpGet]
        public JsonResult GetStaffname(string term)
        {
            var locationdata = Common.GetStaffname(term);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ValidateManDay(int projid, int days, DateTime monyr, int mandayid = 0)
        {
            try
            {
                object output = Common.ValidateManDay(projid, days, monyr, mandayid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public JsonResult ApprovalForManDay(int Mandayid)
        {

            try
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.ManDayCommitmentBalanceUpdate(Mandayid, false, false, logged_in_user, "MDY");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalForManDay(Mandayid, logged_in_user);
                if (!status)
                    coreAccountService.ManDayCommitmentBalanceUpdate(Mandayid, true, false, logged_in_user, "MDY");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ManDayWFInit(int id)
        {
            try
            {
                lock (MDYWFInitlockObj)
                {
                    if (Common.ValidateMandayStatus(id, "Open"))
                    {
                        int logged_in_user = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.ManDayCommitmentBalanceUpdate(id, false, false, logged_in_user, "MDY");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.ManDayWFInit(id, logged_in_user);
                        if (!result.Item1)
                            coreAccountService.ManDayCommitmentBalanceUpdate(id, true, false, logged_in_user, "MDY");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForManDay(int Mandayid)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.ManDayBillApproved(Mandayid, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                // bool boaStatus = coreAccountService.ManDayBOATransaction(Mandayid);
                // var status = Common.ApprovalPendingForManDay(Mandayid, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult ManDayList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetManDayList()
        {
            try
            {
                object output = coreAccountService.GetManDayList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult SearchManDayList(ManDaySearchFieldModel model)
        {
            object output = CoreAccountsService.SearchManDayList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetManDayList(SearchMandays model, int pageIndex, int pageSize, DateFilterModel Date)
        {
            try
            {
                object output = coreAccountService.GetManDayList(model, pageIndex, pageSize, Date);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        #endregion
        #region Overheads Posting
        public ActionResult OverheadsPostingList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetOverheadsPostingList(OverheadsPostingSearch model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetOverheadsPostingList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ApproveOverheadsPosting(int ohpid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
                //if (!cStatus)
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.OverheadsPostingBillApproved(ohpid, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult OverheadsPosting(int ProjectId = 0, int ReceiptId = 0, string ReceiptToDate = null)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                // ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                // ViewBag.ContraAccountGroupList = Common.GetOHPostingBankAccountGroup();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.ReceiptList = Common.GetReceiptList();
                ViewBag.ProjectList = Common.GetOHPAutoCompleteProjectList();
                OverheadsPostingModel model = new OverheadsPostingModel();
                //if (projecttype > 0)
                //{
                //    model = coreAccountService.GetOverheadsDetails(projecttype);
                //}
                //model.FromDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);

                if (ProjectId > 0 || ReceiptId > 0 || ReceiptToDate != null)
                {
                    model = coreAccountService.GetOverheadsDetails(ProjectId, ReceiptId, ReceiptToDate);
                }
                model.ProjectNumber = Common.getprojectnumber(ProjectId);
                model.ToDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.SrchProjectId = ProjectId;
                model.ReceiptId = ReceiptId;
                model.ProjectType = 1;
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                model.ProjectType = 1;
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpGet]
        public JsonResult GetOverheadsDetails(int ProjectId = 0, int ReceiptId = 0, string ReceiptToDate = null)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                // ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                // ViewBag.ContraAccountGroupList = Common.GetOHPostingBankAccountGroup();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.ReceiptList = Common.GetReceiptList();
                ViewBag.ProjectList = Common.GetOHPAutoCompleteProjectList();
                OverheadsPostingModel model = new OverheadsPostingModel();
                DateTime RCVDate = DateTime.Parse(ReceiptToDate);
                model = coreAccountService.GetOverheadsDetails(ProjectId, ReceiptId, ReceiptToDate);
                model.ProjectNumber = Common.getprojectnumber(ProjectId);
                //model.FromDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.ToDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                model.ProjectType = 1;

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult OverheadsPosting(OverheadsPostingModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                //  ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.Bank =
                ViewBag.ProjectNumber = emptyList;
                ViewBag.ReceiptList = Common.GetReceiptList();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.ProjectList = Common.GetOHPAutoCompleteProjectList();
                if (ModelState.IsValid)
                {

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.OverheadsPostingIU(model, logged_in_user);
                    if (model.OverheadsPostingId == null && result > 0)
                    {
                        TempData["succMsg"] = "Overheads Posting has been done successfully.";
                        return RedirectToAction("OverheadsPostingList");
                    }
                    //else if (model.OverheadsPostingId > 0 && result > 0)
                    //{
                    //    TempData["succMsg"] = "Credit note has been updated successfully.";
                    //    return RedirectToAction("CreditNoteList");
                    //}
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.SourceList = Common.GetSourceList();
                return View();
            }
        }

        public ActionResult _OverheadsPostingPIRMFShare(int Projectid, decimal RMFValue, int ReceiptId, string Todate, int srchPjctId)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            OverheadsPostingModel model = new OverheadsPostingModel();
            model = _cs.GetPIRMFShareDetails(Projectid, RMFValue);
            model.ReceiptId = ReceiptId;
            model.ToDate = Todate;
            model.SrchProjectId = srchPjctId;
            return PartialView(model);
        }
        public ActionResult _OverheadsPostingPIPCFShare(int Projectid, decimal PCFValue, int ReceiptId, string Todate, int srchPjctId)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            OverheadsPostingModel model = new OverheadsPostingModel();
            model = _cs.GetPIPCFShareDetails(Projectid, PCFValue);
            model.ReceiptId = ReceiptId;
            model.ToDate = Todate;
            model.SrchProjectId = srchPjctId;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _SavePIPCFShare(OverheadsPostingModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            CoreAccountsService _CS = new CoreAccountsService();
            object output = _CS.SavePIPCFShare(model, UserId, true);
            return RedirectToAction("OverheadsPosting", new { ProjectId = model.SrchProjectId, ReceiptId = model.ReceiptId, ReceiptToDate = model.ToDate });
        }
        [HttpPost]
        public ActionResult _SavePIRMFShare(OverheadsPostingModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            CoreAccountsService _CS = new CoreAccountsService();
            object output = _CS.SavePIRMFShare(model, UserId, true);
            return RedirectToAction("OverheadsPosting", new { ProjectId = model.SrchProjectId, ReceiptId = model.ReceiptId, ReceiptToDate = model.ToDate });
        }
        public ActionResult EditOverheadsPosting(int OHId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                // ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                // ViewBag.ContraAccountGroupList = Common.GetOHPostingBankAccountGroup();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.ReceiptList = Common.GetReceiptList();
                OverheadsPostingModel model = new OverheadsPostingModel();
                //if (projecttype > 0)
                //{
                //    model = coreAccountService.GetOverheadsDetails(projecttype);
                //}
                //model.FromDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);

                if (OHId > 0)
                {
                    model = coreAccountService.GetOverheadsDetailsbyId(OHId);
                }
                //model.ProjectNumber = Common.getprojectnumber(ProjectId);
                model.ToDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.ProjectType = 1;
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                model.ProjectType = 1;
                return View("OverheadsPosting", model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        public ActionResult OverheadsPostingView(int OHId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                // ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                // ViewBag.ContraAccountGroupList = Common.GetOHPostingBankAccountGroup();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.ReceiptList = Common.GetReceiptList();
                ViewBag.ProjectList = Common.GetOHPAutoCompleteProjectList();
                OverheadsPostingModel model = new OverheadsPostingModel();
                //if (projecttype > 0)
                //{
                //    model = coreAccountService.GetOverheadsDetails(projecttype);
                //}
                //model.FromDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);

                if (OHId > 0)
                {
                    model = coreAccountService.GetOverheadsViewDetailsbyId(OHId);
                }
                //model.ProjectNumber = Common.getprojectnumber(ProjectId);
                model.ToDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.ProjectType = 1;
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                model.ProjectType = 1;
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }

        //[HttpGet]
        //public JsonResult LoadOHProjectList(string term)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            var data = Common.GetOHPAutoCompleteProjectList(term);
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        #endregion
        #region OverHead Posting Payment Process
        public ActionResult OverHeadPaymentProcess()
        {
            OverHeadPaymentProcessModel model = new OverHeadPaymentProcessModel();
            try
            {
                ViewBag.Bank = Common.GetBankAccountHeadList();
                ViewBag.CategoryId = Common.GetCodeControlList("OHPaymentProcess");
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult OverHeadPaymentProcess(OverHeadPaymentProcessModel model)
        {

            ViewBag.Bank = Common.GetBankAccountHeadList();
            ViewBag.CategoryId = Common.GetCodeControlList("OHPaymentProcess");
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            int result = coreAccountService.CreateOverHeadPaymentProcess(model, logged_in_user);
            if (model.Id == 0 && result > 0)
            {
                TempData["succMsg"] = "Bill has been added successfully.";
                return RedirectToAction("OverHeadPaymentProcessList");
            }
            {
                TempData["errMsg"] = "Something went wrong please contact administrator.";
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult GetOverHeadPostingList(DateTime FromDate, DateTime ToDate, int Id, int Category)
        {
            try
            {
                object output = CoreAccountsService.GetOverHeadPostingList(FromDate, ToDate, Id, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public ActionResult OverHeadPaymentProcessList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetOverHeadPaymentProcessList(SearchOverHeadPaymentProcess model, int pageIndex, int pageSize, DateFilterModel Date)
        {
            try
            {
                object output = coreAccountService.GetOverHeadPaymentProcessList(model, pageIndex, pageSize, Date);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }

        #endregion
        #region PCF and Distribution Overheads Posting
        public ActionResult PCFDistributionOverheadsPostingList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetPCFDistributionOHPostingList(int pageIndex, int pageSize, SearchPCFDistributionOH model, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetDistributionOHPList(pageIndex, pageSize, model, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        //[HttpGet]
        //public ActionResult ApproveDistributionOHPosting(int ohpid)
        //{
        //    try
        //    {
        //        int userId = Common.GetUserid(User.Identity.Name);

        //        //bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
        //        //if (!cStatus)
        //        //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
        //        bool status = coreAccountService.OverheadsPostingBillApproved(ohpid, userId);
        //        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult PCFDistributionOverheadsPosting(int paymenttype = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                ViewBag.PaymentNumber = emptyList;
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                DistributionOHPostingModel model = new DistributionOHPostingModel();
                //if (paymenttype > 0)
                //{
                //    //model = coreAccountService.GetOverheadsDetails(paymenttype);
                //    if (model.Source == 1)
                //        ViewBag.SourceRefNumberList = Common.GetWorkflowRefNumberList();
                //    else if (model.Source == 3)
                //    {
                //        int depId = Common.GetDepartmentId(User.Identity.Name);
                //        ViewBag.SourceRefNumberList = Common.GetTapalRefNumberList(depId);
                //    }
                //}
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpGet]
        public JsonResult GetRCVDOHPaymentDetails(int paytype, string fromdate, string todate)
        {
            try
            {
                object output = coreAccountService.GetConsRCVDOHPaymentDetails(paytype, fromdate, todate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult PCFDistributionOverheadsPosting(DistributionOHPostingModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.Bank =
                ViewBag.ProjectNumber = emptyList;
                ViewBag.PaymentNumber = emptyList;
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                if (ModelState.IsValid)
                {

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.DistributionOHPostingIU(model, logged_in_user);
                    if (model.OverheadsPostingId == null && result > 0)
                    {
                        TempData["succMsg"] = "Overheads Posting has been done successfully.";
                        return RedirectToAction("PCFDistributionOverheadsPostingList");
                    }
                    else if (model.OverheadsPostingId == null && result == -2)
                    {
                        TempData["errMsg"] = "Overheads Posting not done since back end receipt amount against Overheads project has exceeded the sanction value.";
                    }
                    else if (model.OverheadsPostingId == null && result == -3)
                    {
                        TempData["errMsg"] = "Overheads Posting not done since back end receipt amount against the PCF Project has exceeded the sanction value.";
                    }
                    else if (model.OverheadsPostingId == null && result == -4)
                    {
                        TempData["errMsg"] = "Overheads Posting not done since back end receipt amount against the EUCO Project has exceeded the sanction value.";
                    }
                    //else if (model.OverheadsPostingId > 0 && result > 0)
                    //{
                    //    TempData["succMsg"] = "Credit note has been updated successfully.";
                    //    return RedirectToAction("CreditNoteList");
                    //}
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;

                return View();
            }
        }
        public ActionResult PCFDistributionOHPostingView(int OverheadsPostingId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.Bank = Common.GetAllBankList();
                ViewBag.ProjectNumber = Common.GetProjectNumbers();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                ViewBag.disabled = "disabled";
                DistributionOHPostingModel model = new DistributionOHPostingModel();
                model = coreAccountService.GetPCFDOHViewDetails(OverheadsPostingId);
                int paymenttype = model.PaymentTypeId ?? 0;
                ViewBag.PaymentNumber = Common.GetPaymentNumberListbyType(paymenttype);
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("PCFDistributionOverheadsPostingList", "CoreAccounts");
            }
        }
        [HttpGet]
        public JsonResult GetPCFDOHPaymentDetails(int paytype, int paynumberid)
        {
            try
            {
                object output = coreAccountService.GetPCFDOHPaymentDetails(paytype, paynumberid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPaymentNumberList(int paymenttype)
        {
            object output = coreAccountService.GetPaymentNumberList(paymenttype);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Bill Date
        [HttpGet]
        public ActionResult BillStatusChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult BillStatusChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            //model.Message = pro.UpdateBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            TempData["errMsg"] = pro.UpdateBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadAllRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "STV", "PTV", "ADV" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region TravelBill Date
        [HttpGet]
        public ActionResult TravelBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult TravelBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateTravelBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadTravelRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "DTV", "TAD", "TST" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetTravelBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetTravelCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region CLP Date
        [HttpGet]
        public ActionResult CLPBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult CLPBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateCLPBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadCLPRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "CLV" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetCLPBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetCLPCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region Honor Date
        [HttpGet]
        public ActionResult HonorBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult HonorBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateHonorBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadHonorRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "HON" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetHonorBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetHonorCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region GVR Date
        [HttpGet]
        public ActionResult GVRBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult GVRBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateGVRBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadGVRRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "GVR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetGVRBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetGVRCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Reimburs Date
        [HttpGet]
        public ActionResult ReimbursBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ReimbursBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateReimbursBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadReimbursRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "REM" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetReimbursBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetReimbursCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Receipt Date
        [HttpGet]
        public ActionResult ReceiptBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ReceiptBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateReceiptBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadReceiptRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "RCV", "RBU" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetReceiptBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetReceiptCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Invoice Date
        [HttpGet]
        public ActionResult InvoiceDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult InvoiceDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateInvoiceDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadInvoiceRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "INV" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetInvoiceDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetInvoiceCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Receipt
        public ActionResult ReceiptBreakupList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetReceiptBreakupList(ReceiptBreakupModel model, int pageIndex, int pageSize, DateFilterModel ReceiptDateStr, DateFilterModel BreakupDateStr)
        {
            try
            {
                object output = coreAccountService.GetReceiptBreakupList(model, pageIndex, pageSize, ReceiptDateStr, BreakupDateStr);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        public ActionResult ReceiptBreakup(int receiptBreakupId = 0)
        {
            ReceiptBreakupModel model = new ReceiptBreakupModel();
            if (receiptBreakupId > 0 && Common.ValidateReceiptBreakupStatus(receiptBreakupId, "Open"))
            {
                model = coreAccountService.GetReceiptBreakup(receiptBreakupId);
                return View(model);
            }
            else
                return View(model);
        }

        public ActionResult ReceiptBreakupView(int receiptBreakupId)
        {
            try
            {
                var model = coreAccountService.GetReceiptBreakup(receiptBreakupId);
                ViewBag.VwReceiptBreakupId = receiptBreakupId;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(217, "Others", 0);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ReceiptBreakupList");
            }

        }

        [HttpPost]
        public ActionResult ReceiptBreakup(ReceiptBreakupModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validate = Common.ValidateReceiptBreakup(model);
                    if (validate != "Valid")
                    {
                        var projectId = Common.GetReceiptProject(model.ReceiptId.GetValueOrDefault(0));
                        var data = Common.getAllocationHeadAndGroup(projectId);
                        foreach (var group in model.groups)
                        {
                            group.headList = data.Where(m => m.BudgetGroupId == group.BudgetGroupId).FirstOrDefault().headList;
                        }
                        TempData["errMsg"] = validate;
                        return View(model);
                    }
                    int loggedinuser = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ReceiptBreakupIU(model, loggedinuser);
                    if (model.ReceiptBreakupId == null && result > 0)
                    {
                        TempData["succMsg"] = "Receipt breakup has been added successfully.";

                        return RedirectToAction("ReceiptBreakupList");
                    }
                    else if (model.ReceiptBreakupId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Receipt breakup has been updated successfully.";
                        return RedirectToAction("ReceiptBreakupList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Receipt breakup processed for this receipt.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    var projectId = Common.GetReceiptProject(model.ReceiptId.GetValueOrDefault(0));
                    var data = Common.getAllocationHeadAndGroup(projectId);
                    foreach (var group in model.groups)
                    {
                        group.headList = data.Where(m => m.BudgetGroupId == group.BudgetGroupId).FirstOrDefault().headList;
                    }
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ReceiptBreakup(int receiptId, bool updated_f = false)
        {
            try
            {
                var data = coreAccountService.GetReceiptBreakup(receiptId, updated_f);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet]
        public JsonResult LoadReceiptListForBreakup(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteReceiptNumberForBreakup(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ReceiptBreakupWFInit(int receiptBreakupId)
        {
            try
            {
                lock (ReceiptBUlockObj)
                {
                    if (Common.ValidateReceiptBreakupStatus(receiptBreakupId, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool output = coreAccountService.ReceiptBreakupWFInit(receiptBreakupId, userId);
                        //false; 
                        return Json(new { status = output, msg = !output ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateReceipt(int ID = 0)
        {
            try
            {
                var pId = Common.getProjectID(ID);
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();
                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getFRMcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.projecttype = projecttype;
                ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(33);

                CreateReceiptModel model = new CreateReceiptModel();
                model = coreAccountService.GetReceiptDetails(pId, ID);
                if (model.ReceiptAmount > 0)
                {
                    return View(model);
                }
                if (model.ReceiptAmount <= 0 && model.TotalCreditNoteAmount == model.InvoiceValue)
                {
                    ViewBag.errMsg = "Credit Note has been raised against this Invoice for full Value.";
                    var errmsg = ViewBag.errMsg;
                    if (model.ProjectType == 1)
                    {
                        return RedirectToAction("SponReceiptList", "CoreAccounts", new { errmsg = errmsg });
                    }
                    if (model.ProjectType == 2)
                    {
                        return RedirectToAction("ConsReceiptList", "CoreAccounts", new { errmsg = errmsg });
                    }
                }
                if (model.ReceiptAmount <= 0)
                {
                    ViewBag.errMsg = "Receipt has already been created for Invoice Value or Credit Note has been raised against this Invoice.";
                    var errmsg = ViewBag.errMsg;
                    if (model.ProjectType == 1)
                    {
                        return RedirectToAction("SponReceiptList", "CoreAccounts", new { errmsg = errmsg });
                    }
                    if (model.ProjectType == 2)
                    {
                        return RedirectToAction("ConsReceiptList", "CoreAccounts", new { errmsg = errmsg });
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateReceipt(CreateReceiptModel model, FormCollection form, HttpPostedFileBase tdsfile)
        {
            try
            {
                var value = form["Buttonvalue"];
                var roleId = Common.GetRoleId(User.Identity.Name);
                var ProjectId = Convert.ToInt32(model.PIId);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                //var invoicetype = Common.getinvoicetype();
                //var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();
                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getFRMcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.projecttype = projecttype;
                //    ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(33);
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");
                if (tdsfile != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    string tdsfilename = Path.GetFileName(tdsfile.FileName);
                    var docextension = Path.GetExtension(tdsfilename);
                    if (!allowedExtensions.Contains(docextension))
                    {
                        ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                        return RedirectToAction("ProjectOpening", "Project");
                    }
                }
                CoreAccountsService _ps = new CoreAccountsService();
                int validid = Common.ValidateReceiptOnSubmit(model);
                if (validid == -1)
                {
                    ViewBag.errMsg = "Total Receipt CGST cannot exceed the total Invoice CGST.";
                    return View(model);
                }
                if (validid == -2)
                {
                    ViewBag.errMsg = "Total Receipt SGST cannot exceed the total Invoice SGST.";
                    return View(model);
                }
                if (validid == -3)
                {
                    ViewBag.errMsg = "Total Receipt IGST cannot exceed the total Invoice IGST.";
                    return View(model);
                }
                if (validid == -5)
                {
                    ViewBag.errMsg = "Invoice details could not be obtained for validation. Please try again or contact administrator";
                    return View(model);
                }
                if (validid == -6)
                {
                    ViewBag.errMsg = "Total Receipt amount cannot exceed the sanction value.";
                    return View(model);
                }
                if (validid > 0)
                {


                    var ReceiptID = _ps.CreateReceipt(model, loggedinuserid, tdsfile);
                    if (ReceiptID > 0)
                    {
                        var ReceiptNumber = Common.getreceiptnumber(ReceiptID);
                        ViewBag.succMsg = "Receipt has been created successfully with Receipt Number - " + ReceiptNumber;
                    }
                    else if (ReceiptID == -11)
                    {
                        ViewBag.errMsg = "Overhead Value has exceeded allocation available balance. Please check.";
                    }
                    else if (ReceiptID == -5)
                    {
                        if (value == "Approve")
                        {
                            var ReceiptId = Convert.ToInt32(model.ReceiptID);
                            BOAModel BOAmodel = new BOAModel();
                            BOAmodel = coreAccountService.getBOAmodeldetails(model);
                            var BOA = _ps.BOATransaction(BOAmodel);
                            if (BOA == true)
                            {
                                using (var context = new DataModel.IOASDBEntities())
                                {
                                    var Vquery = context.tblReceipt.SingleOrDefault(m => m.ReceiptId == model.ReceiptID && m.CategoryId != 16);
                                    // Vquery.BOAId = boaId;

                                    List<tblProjectTransactionlog> pLogList = new List<tblProjectTransactionlog>();
                                    pLogList.Add(new tblProjectTransactionlog()
                                    {
                                        Amount = Vquery.BankAmountDr,
                                        CRTD_TS = DateTime.Now,
                                        ProjectId = Vquery.BankAccountHeadDr,
                                        RefId = Vquery.ReceiptId,
                                        TranactionDate = DateTime.Now,
                                        TransactionTypeCode = "RCV",
                                        Type = "I"
                                    });
                                    bool updtTrans = coreAccountService.UpdateProjectTransaction(pLogList);
                                    if (updtTrans == false)
                                    {
                                        ViewBag.errMsg = "Project Transaction Log not updated.";
                                    }

                                    Vquery.Status = "Completed";
                                    context.SaveChanges();
                                    var negativebalancequery = (from v in context.tblNegativeBalance
                                                                where v.ProjectId == Vquery.ProjectId && v.Status == "Approved"
                                                                select v).ToList();

                                    if (negativebalancequery.Count() > 0)
                                    {
                                        decimal? claimamt = 0;
                                        decimal? balancewhenclosing = 0;
                                        decimal? negativebalamt = 0;
                                        decimal? adjustamount = 0;
                                        decimal? overheadsamt = Vquery.ReceiptOverheadValue;
                                        decimal? gstamt = 0;
                                        decimal? newadjustamt = 0;

                                        if (Vquery.IGST > 0 && Vquery.CGST == 0)
                                        {
                                            gstamt = Vquery.IGST;
                                        }
                                        else if (Vquery.CGST > 0 && Vquery.IGST == 0)
                                        {
                                            gstamt = Vquery.CGST + Vquery.SGST;
                                        }
                                        decimal? receiptamt = Vquery.ReceiptAmount - (gstamt);
                                        decimal? balinreceipt = receiptamt;

                                        for (int i = 0; i < negativebalancequery.Count; i++)
                                        {
                                            claimamt = negativebalancequery[i].ClaimAmount;
                                            balancewhenclosing = negativebalancequery[i].BalanceWhenClosing ?? 0;
                                            negativebalamt = negativebalancequery[i].NegativeBalanceAmount ?? 0;
                                            adjustamount = negativebalancequery[i].ReceiptAdjustmentAmount ?? 0;
                                            decimal? newnegativebalamt = 0;
                                            if (balinreceipt > 0)
                                            {
                                                if (balinreceipt >= negativebalamt)
                                                {
                                                    newadjustamt = adjustamount + negativebalamt;
                                                    negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                    negativebalancequery[i].NegativeBalanceAmount = 0;
                                                    negativebalancequery[i].Status = "Closed";
                                                    negativebalancequery[i].BalanceWhenClosing = 0;
                                                    negativebalancequery[i].ClosedDate = DateTime.Now;
                                                    negativebalancequery[i].UPTD_By = loggedinuserid;
                                                    negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                    negativebalancequery[i].ReasonForClose = "Receipt Created for full negative balance amount.";
                                                    balinreceipt = balinreceipt - negativebalamt;
                                                    context.SaveChanges();
                                                }
                                                else
                                                {
                                                    newadjustamt = adjustamount + balinreceipt;
                                                    newnegativebalamt = claimamt - (newadjustamt + balancewhenclosing);
                                                    negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                    negativebalancequery[i].NegativeBalanceAmount = newnegativebalamt;
                                                    negativebalancequery[i].UPTD_By = loggedinuserid;
                                                    negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                    balinreceipt = 0;
                                                    context.SaveChanges();
                                                }
                                                tblNegativeApprovalOffsetLog log = new tblNegativeApprovalOffsetLog();
                                                log.NegativeBalanceId = negativebalancequery[i].NegativeBalanceId;
                                                log.NegativeBalanceNumber = negativebalancequery[i].NegativeBalanceNumber;
                                                log.RefId = Vquery.ReceiptId;
                                                log.RefNumber = Vquery.ReceiptNumber;
                                                log.FunctionName = "Receipt";
                                                log.Remarks = "Negative Approval offset on Receipt Approval with Amount - " + receiptamt + "(" + Vquery.ReceiptAmount + " - " + "(" + overheadsamt + " + " + gstamt + "))";
                                                log.AmountModified = receiptamt;
                                                log.CRTD_BY = loggedinuserid;
                                                log.CRTD_TS = DateTime.Now;
                                                context.tblNegativeApprovalOffsetLog.Add(log);
                                                context.SaveChanges();
                                            }
                                            else if (balinreceipt == 0)
                                            {
                                                newadjustamt = 0;
                                                context.SaveChanges();
                                            }

                                        }

                                    }
                                    _ps.ReceiptEmailSend(ReceiptId, loggedinuser);
                                }
                                var ReceiptNumber = Common.getreceiptnumber(ReceiptId);
                                ViewBag.succMsg = "Receipt with Receipt number -" + ReceiptNumber + " has been Approved.";
                            }
                            else
                            {
                                ViewBag.errMsg = "Receipt not approved. Please try again or contact administrator";
                            }
                        }
                        else if (value != "Approve")
                        {
                            var ReceiptId = Convert.ToInt32(model.ReceiptID);
                            var ReceiptNumber = Common.getreceiptnumber(ReceiptId);
                            ViewBag.succMsg = "Receipt with Receipt number -" + ReceiptNumber + " has been updated successfully.";
                        }

                    }
                    else if (ReceiptID == -6)
                    {
                        ViewBag.errMsg = "The Bank Debit Value and Net Transaction value are not equal. Please check.";
                    }
                    else if (ReceiptID == -7)
                    {
                        ViewBag.errMsg = "The Receipt Value has exceeded the Invoice value. Please check.";
                    }
                    else if (ReceiptID == -8)
                    {
                        ViewBag.errMsg = "The Receipt Value in foreign currency has exceeded the total Invoice value in foreign currecny. Please check.";
                    }
                    else
                    {
                        ViewBag.errMsg = "Something went wrong please contact administrator";
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        public ActionResult EditProjectReceipt(int ReceiptId = 0)
        {
            try
            {
                var pId = Common.getProjectIdbyReceiptId(ReceiptId);
                //var pId = Common.getProjectID(InvoiceId);
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();
                ViewBag.SourceList = Common.GetSourceList();
                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getFRMcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.projecttype = projecttype;
                ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(33);


                CreateReceiptModel model = new CreateReceiptModel();
                model = coreAccountService.GetReceiptDetailsbyId(pId, ReceiptId);

                return View("CreateReceipt", model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchInvoiceList(ReceiptListModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                //var receiptdata = new PagedData<ReceiptSearchResultModel>();
                //ReceiptSearchFieldModel model = new ReceiptSearchFieldModel();
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                srchModel.Userrole = user_role;
                if (srchModel.InvoiceToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.InvoiceToDate;
                    srchModel.InvoiceToDate = todate.Date.AddDays(1).AddTicks(-2);
                }
                var data = coreAccountService.GetSearchInvoiceList(srchModel, page, pageSize);
                srchModel = data;
                return PartialView(srchModel);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
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
        public ActionResult InvoiceProcess(int InvoiceId = 0, string msg = null)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.projecttype = projecttype;
                ViewBag.state = Common.GetStatelist();
                ViewBag.countryList = Common.getCountryList();
                ViewBag.AccGroupId = Common.GetAccountGroup(28);
                ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                ViewBag.IndianSEZTaxCategoryList = Common.GetCodeControlList("IndianSEZTaxCategory");
                ViewBag.Currency = Common.getCurrency();
                ViewBag.succMsg = msg;
                InvoiceModel model = new InvoiceModel();

                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult InvoiceProcess(InvoiceModel model)
        {
            try
            {
                lock (lockInvoiceBOArequestObj)
                {
                 bool IsValidRequest = Common.CheckBOAPostingProjectInvoice(model.InvoiceId ?? 0);
                    if (IsValidRequest)
                    {
                        var roleId = Common.GetRoleId(User.Identity.Name);
                        var loggedinuser = User.Identity.Name;
                        var loggedinuserid = Common.GetUserid(loggedinuser);
                        var servicetype = Common.getservicetype();
                        var invoicetype = Common.getinvoicetype();
                        var projecttype = Common.getprojecttype();
                        ViewBag.typeofservice = servicetype;
                        ViewBag.TypeofInvoice = invoicetype;
                        ViewBag.projecttype = projecttype;
                        ViewBag.state = Common.GetStatelist();
                        ViewBag.countryList = Common.getCountryList();
                        ViewBag.AccGroupId = Common.GetAccountGroup(28);
                        ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                        ViewBag.IndianSEZTaxCategoryList = Common.GetCodeControlList("IndianSEZTaxCategory");
                        ViewBag.Currency = Common.getCurrency();
                        var InvoiceID = coreAccountService.CreateInvoice(model, loggedinuserid, true);
                        if (InvoiceID > 0 && model.ProjectType == 2)
                        {
                            var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                            coreAccountService.InvoiceEmailSend(InvoiceID);
                            ViewBag.succMsg = "Invoice - " + InvoiceNumber + " has been approved successfully.";
                        }
                        else if (InvoiceID > 0 && model.ProjectType == 1)
                        {
                            var InvoiceNumber = Common.getinvoicenumber(InvoiceID);

                            ViewBag.succMsg = "Invoice - " + InvoiceNumber + " has been approved successfully.";
                        }
                        else if (InvoiceID == -12)
                        {
                            ViewBag.errMsg = "server not responding .try again after sometime";
                        }
                        else
                        {
                            ViewBag.errMsg = "Something went wrong please contact administrator";
                        }
                        return View(model);
                    }
                    else
                    {
                        ViewBag.errMsg = "This Request already Approved";
                        return View(model);
                    }
            }
            }
            catch (Exception ex)
            {
                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ProjectInvoice(InvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                ViewBag.countryList = Common.getCountryList();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.projecttype = projecttype;
                ViewBag.state = Common.GetStatelist();
                ViewBag.AccGroupId = Common.GetAccountGroup(28);
                ViewBag.AccHeadId = Common.GetAccountHeadListbyGroup(28);
                ViewBag.IndianSEZTaxCategoryList = Common.GetCodeControlList("IndianSEZTaxCategory");
                ViewBag.Currency = Common.getCurrency();
                if (model.TaxableValue < 0)
                {
                    ViewBag.errMsg = "Invoice Cannot be generated. No balance available for raising Invoice";
                    return View("InvoiceProcess", model);
                }
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");   

                CoreAccountsService _ps = new CoreAccountsService();
                var InvoiceID = _ps.CreateInvoice(model, loggedinuserid, false);
                if (InvoiceID == -4)
                {
                    //ViewBag.errMsg = "Invoice Cannot be generated as the Taxable value has exceeded the balance available for raising Invoice. Please enter correct value and try again.";
                    //return View("InvoiceProcess", model);
                    TempData["errMsg"] = "Invoice Cannot be generated as the Taxable value has exceeded the balance available for raising Invoice. Please enter correct value and try again.";
                    return Json(TempData, JsonRequestBehavior.AllowGet);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    //ViewBag.succMsg = "Invoice has been created successfully with Invoice Number - " + InvoiceNumber + ".";
                    TempData["succMsg"] = "Success";
                    return Json(TempData, JsonRequestBehavior.AllowGet);
                }
                else if (InvoiceID == -12)
                {
                    ViewBag.errMsg = "server not responding .try again after sometime";
                    TempData["succMsg"] = "Not Updated";
                    return Json(TempData, JsonRequestBehavior.AllowGet);
                }
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceId);
                    // ViewBag.succMsg = "Invoice with Invoice number - " + InvoiceNumber + " has been updated successfully.";
                    //TempData["succMsg"] = "Invoice with Invoice number - " + InvoiceNumber + " has been updated successfully.";
                    TempData["succMsg"] = "Updated";
                    return Json(TempData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View("InvoiceProcess", model); ;
            }
            catch (Exception ex)
            {
                return View("InvoiceProcess", model);
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadStateCode(string state)
        {
            state = state == "" ? "0" : state;
            object output = coreAccountService.getstatecode(Convert.ToInt32(state));
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult SearchInvoiceList(InvoiceSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchInvoiceListForApproval(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }


        public ActionResult SponReceiptList(string errmsg = null)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                ViewBag.Status = Common.getreceiptstatus();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                ViewBag.errMsg = errmsg;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                ReceiptSearchFieldModel srchModel = new ReceiptSearchFieldModel();
                InvoiceSearchFieldModel srchinvoiceModel = new InvoiceSearchFieldModel();
                var pjcttype = 1;
                srchModel.ProjectType = pjcttype;
                srchinvoiceModel.ProjectType = pjcttype;
                var frmdate = DateTime.Today.AddDays(-15);
                var todate = DateTime.Now;
                model.InvoiceFromDate = frmdate;
                model.InvoiceToDate = todate;
                model.ProjectType = pjcttype;
                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);
                model = coreAccountService.GetInvoiceList(model);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [HandleError]
        public ActionResult SponSearchReceiptList(ReceiptSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-2);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-2);
                //}

                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult ConsReceiptList(string errmsg = null)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;

                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                ViewBag.Status = Common.getreceiptstatus();
                ViewBag.errMsg = errmsg;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                ReceiptSearchFieldModel srchModel = new ReceiptSearchFieldModel();
                InvoiceSearchFieldModel srchinvoiceModel = new InvoiceSearchFieldModel();
                var pjcttype = 2;
                srchModel.ProjectType = pjcttype;
                srchinvoiceModel.ProjectType = pjcttype;
                var frmdate = DateTime.Today.AddDays(-15);
                var todate = DateTime.Now;
                model.InvoiceFromDate = frmdate;
                model.InvoiceToDate = todate;
                model.ProjectType = pjcttype;
                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);
                model = coreAccountService.GetInvoiceList(model);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConsSearchReceiptList(ReceiptSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-2);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-2);
                //}

                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        [HttpPost]
        public JsonResult Loadexchangerate(string Currencyid)
        {
            Currencyid = Currencyid == "" ? "0" : Currencyid;
            object output = coreAccountService.LoadExchangerate(Convert.ToInt32(Currencyid));
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadCurrency(string CountryId)
        {
            CountryId = CountryId == "" ? "0" : CountryId;
            var locationdata = coreAccountService.getCurrency(Convert.ToInt32(CountryId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadInvoiceProcessList()
        {
            object output = coreAccountService.GetInvoiceProcessList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadInvoiceProcess(int InvoiceId)
        {
            object output = coreAccountService.GetInvoiceDetails(InvoiceId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProjectReceiptView(int ReceiptId = 0, bool Pfinit = false)
        {
            try
            {
                var pId = Common.getProjectIdbyReceiptId(ReceiptId);
                //var pId = Common.getProjectID(InvoiceId);
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var projecttype = Common.getprojecttype();
                CreateReceiptModel model = new CreateReceiptModel();
                model = coreAccountService.GetReceiptDetailsView(pId, ReceiptId);
                model.PFInit = Pfinit;
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion
        #region Other Receipts
        [HttpGet]
        public JsonResult ACInvoiceListForNegReceipt(string term, int projectId)
        {
            try
            {
                var data = Common.GetAutoCompleteInvoceListForReceipt(term, projectId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult OtherReceiptList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetOtherReceiptList()
        {
            try
            {
                object output = coreAccountService.GetOtherReceiptList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveOtherReceipt(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveOtherReceipt(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult OtherReceiptWFInit(int id)
        {
            try
            {
                //lock (OtherReceiptWFInitlockObj)
                //{
                if (Common.ValidateReceiptStatus(id, "Open"))
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool output = coreAccountService.OtherReceiptWFInit(id, userId);
                    return Json(new { status = output, msg = !output ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                // }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult OtherReceipt(int id = 0)
        //{
        //    try
        //    {
        //        OtherReceiptModel model = new OtherReceiptModel();
        //        var emptyList = new List<MasterlistviewModel>();
        //        ViewBag.NegReceiptList =
        //        ViewBag.AccountHeadList = emptyList;
        //        ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
        //        ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
        //        ViewBag.AccountGroupList = Common.GetAccountGroup(false);
        //        ViewBag.BankList = Common.GetBankAccountHeadList();
        //        ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
        //        //var ptypeList = Common.getprojecttype();
        //        //int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
        //        //ViewBag.ProjectTypeList = ptypeList;
        //        //ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
        //        if (id > 0 && Common.ValidateReceiptStatus(id, "Open"))
        //        {
        //            model = coreAccountService.GetOtherReceiptDetails(id);
        //            if (model.Category == 18)
        //                ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
        //            if (model.DeductionDetail.Count == 0)
        //            {
        //                int[] heads = { 36, 37, 38 };
        //                model.DeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.NegReceiptList = emptyList;
        //            int[] heads = { 36, 37, 38 };
        //            model.DeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Dashboard", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult OtherReceipt(OtherReceiptModel model)
        //{
        //    try
        //    {
        //        var emptyList = new List<MasterlistviewModel>();
        //        ViewBag.AccountHeadList =
        //        ViewBag.AccountHeadList =
        //        ViewBag.NegReceiptList = emptyList;
        //        ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
        //        ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
        //        ViewBag.AccountGroupList = Common.GetAccountGroup(false);
        //        ViewBag.BankList = Common.GetBankAccountHeadList();
        //        ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
        //        if (model.Category == 18)
        //        {
        //            for (int i = 0; i < model.ExpenseDetail.Count(); i++)
        //            {
        //                ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
        //                ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
        //                ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
        //                ModelState.Remove("ExpenseDetail[" + i + "].Amount");
        //            }
        //            if (model.InvoiceId > 0)
        //                ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
        //        }
        //        foreach (var item in model.ExpenseDetail)
        //        {
        //            int headId = item.AccountGroupId ?? 0;
        //            item.AccountHeadList = Common.GetAccountHeadList(headId);
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            string validationMsg = ValidateOtherReceipt(model);
        //            if (validationMsg != "Valid")
        //            {
        //                TempData["errMsg"] = validationMsg;
        //                return View(model);
        //            }
        //            if (model.file != null)
        //            {
        //                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
        //                string taxprooffilename = Path.GetFileName(model.file.FileName);
        //                var docextension = Path.GetExtension(taxprooffilename);
        //                if (!allowedExtensions.Contains(docextension))
        //                {
        //                    TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
        //                    return View(model);
        //                }
        //            }
        //            int logged_in_user = Common.GetUserid(User.Identity.Name);
        //            model.ClassificationOfReceipt = 1;
        //            int result = coreAccountService.OtherReceiptIU(model, logged_in_user);
        //            if (model.ReceiptId == null && result > 0)
        //            {
        //                TempData["succMsg"] = "Receipt has been added successfully.";
        //                return RedirectToAction("OtherReceiptList");
        //            }
        //            else if (model.ReceiptId > 0 && result > 0)
        //            {
        //                TempData["succMsg"] = "Receipt has been updated successfully.";
        //                return RedirectToAction("OtherReceiptList");
        //            }
        //            else if (result == -2)
        //            {
        //                TempData["errMsg"] = "Total receipts amount should not be greater than sanction value of the project.";
        //                return RedirectToAction("OtherReceiptList");
        //            }
        //            else if (result == -3)
        //            {
        //                TempData["errMsg"] = "Not a valid entry. Credit and Debit value are not equal.";
        //                return RedirectToAction("OtherReceiptList");
        //            }
        //            else
        //                TempData["errMsg"] = "Something went wrong please contact administrator.";

        //        }
        //        else
        //        {
        //            string messages = string.Join("<br />", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage));

        //            TempData["errMsg"] = messages;
        //        }
        //        return View(model);

        //    }
        //    catch (Exception ex)
        //    {
        //        var emptyList = new List<MasterlistviewModel>();
        //        ViewBag.AccountHeadList =
        //        ViewBag.AccountHeadList =
        //        ViewBag.NegReceiptList = emptyList;
        //        ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
        //        ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
        //        ViewBag.AccountGroupList = Common.GetAccountGroup(false);
        //        ViewBag.BankList = Common.GetBankAccountHeadList();
        //        ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
        //        if (model.InvoiceId > 0)
        //            ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
        //        foreach (var item in model.ExpenseDetail)
        //        {
        //            int headId = item.AccountGroupId ?? 0;
        //            item.AccountHeadList = Common.GetAccountHeadList(headId);
        //        }
        //        return View(model);
        //    }
        //}


        public ActionResult OtherReceipt(int id = 0)
        {
            try
            {
                OtherReceiptModel model = new OtherReceiptModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.NegReceiptList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(true);
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                //var ptypeList = Common.getprojecttype();
                //int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                //ViewBag.ProjectTypeList = ptypeList;
                //ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (id > 0 && Common.ValidateReceiptStatus(id, "Open"))
                {
                    model = coreAccountService.GetOtherReceiptDetails(id);
                    if (model.Category == 18)
                        ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
                    if (model.DeductionDetail.Count == 0)
                    {
                        int[] heads = { 36, 37, 38 };
                        model.DeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
                    }
                }
                else
                {
                    ViewBag.NegReceiptList = emptyList;
                    int[] heads = { 36, 37, 38 };
                    model.DeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public ActionResult OtherReceipt(OtherReceiptModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.NegReceiptList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(true);
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                if (model.Category == 18)
                {
                    for (int i = 0; i < model.ExpenseDetail.Count(); i++)
                    {
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
                        ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
                        ModelState.Remove("ExpenseDetail[" + i + "].Amount");
                    }
                    if (model.InvoiceId > 0)
                        ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
                }
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateOtherReceipt(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.file != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string taxprooffilename = Path.GetFileName(model.file.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    model.ClassificationOfReceipt = 1;
                    int result = coreAccountService.OtherReceiptIU(model, logged_in_user);
                    if (model.ReceiptId == null && result > 0)
                    {
                        TempData["succMsg"] = "Receipt has been added successfully.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else if (model.ReceiptId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Receipt has been updated successfully.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else if (result == -2)
                    {
                        TempData["errMsg"] = "Total receipts amount should not be greater than sanction value of the project.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else if (result == -3)
                    {
                        TempData["errMsg"] = "Not a valid entry. Credit and Debit value are not equal.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.NegReceiptList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList(false);
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                if (model.InvoiceId > 0)
                    ViewBag.NegReceiptList = Common.GetReceiptNoByInvoice(model.InvoiceId.GetValueOrDefault(0));
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                return View(model);
            }
        }

        public ActionResult OtherReceiptView(int id, bool Pfinit = false)
        {
            try
            {
                OtherReceiptModel model = new OtherReceiptModel();
                model = coreAccountService.GetOtherReceiptDetailsView(id);
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(73, "Others", model.BankAmount ?? 0);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        private string ValidateOtherReceipt(OtherReceiptModel model)
        {
            string msg = "Valid";
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTax = model.DeductionDetail != null ? model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0 : 0;

            if (model.Category == 18)
            {
                int invId = model.InvoiceId ?? 0;
                ProjectService _PS = new ProjectService();
                if (invId > 0)
                {
                    msg = Common.ValidateNegReceipt(invId, model.NegativeReceiptNo.GetValueOrDefault(0), model.ReceiptId, model.ProjectId.GetValueOrDefault(0));
                }
            }
            else if (model.Category == 15)
            {

                drAmt = drAmt + ttlTax;
                decimal bankAmt = model.BankAmount ?? 0;
                crAmt = crAmt + bankAmt;
                if (drAmt != crAmt)
                    msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
                ProjectService _PS = new ProjectService();
                var projectData = _PS.getProjectSummary(Convert.ToInt32(model.ProjectId));
                var taxableAmt = Math.Round(crAmt - ttlTax, 2, MidpointRounding.AwayFromZero);
                if (taxableAmt > projectData.NetBalance)
                    msg = "Negative Receipt Amount Should not be greater than NetBalance.";
            }
            else
            {
                drAmt = drAmt + (model.BankAmount ?? 0);
                crAmt = crAmt + ttlTax;
                if (drAmt != crAmt)
                    msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            return msg;
        }
        [HttpPost]
        public JsonResult GetOtherReceiptList(SearchViewModel model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetOtherReceiptList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetReceiptNoByInvoice(int invId)
        {
            object output = Common.GetReceiptNoByInvoice(invId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckIsReceiptOHPosted(int recId)
        {
            bool output = Common.CheckIsReceiptOHPosted(recId);
            OtherReceiptModel model = new OtherReceiptModel();
            if (output)
                model = Common.GetReversalReceiptLedger(recId);
            return Json(new { status = output, data = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Head Credit
        public ActionResult HeadCreditList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HeadCreditWFInit(int id)
        {
            try
            {
                lock (HeadCreditWFInitlockObj)
                {
                    if (Common.ValidateHeadCreditStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.HeadCreditCommitmentBalanceUpdate(id, false, false, userId, "HCR");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.HeadCreditWFInit(id, userId);
                        if (!result.Item1)
                            coreAccountService.HeadCreditCommitmentBalanceUpdate(id, true, false, userId, "HCR");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpGet]
        //public JsonResult ApproveHeadCredit(int id)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            if (Common.ValidateHeadCreditStatus(id, "Open"))
        //            {
        //                int userId = Common.GetUserid(User.Identity.Name);
        //                bool cStatus = coreAccountService.HeadCreditCommitmentBalanceUpdate(id, false, false, userId, "HCR");
        //                if (!cStatus)
        //                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
        //                var result = coreAccountService.ApproveHeadCredit(id, userId);
        //                if (result)
        //                    coreAccountService.HeadCreditCommitmentBalanceUpdate(id, true, false, userId, "HCR");
        //                return Json(new { status = result, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //                return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public ActionResult HeadCredit(int id = 0)
        {
            try
            {
                HeadCreditModel model = new HeadCreditModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.InvList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("HeadCreditCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofCredit");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "Head Credit");
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                if (id > 0 && Common.ValidateHeadCreditStatus(id, "Open"))
                {
                    model = coreAccountService.GetHeadCreditDetails(id);
                    ViewBag.InvList = Common.GetInvoiceNoList(model.BillRefNo);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public ActionResult HeadCredit(HeadCreditModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.InvList = Common.GetInvoiceNoList(model.BillRefNo);
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("HeadCreditCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofCredit");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "Head Credit");
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (Convert.ToDecimal(model.CommitmentAmount) == 0)
                {
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateHeadCredit(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.file != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string taxprooffilename = Path.GetFileName(model.file.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.HeadCreditIU(model, logged_in_user);
                    if (model.HeadCreditId == null && result > 0)
                    {
                        TempData["succMsg"] = "Head Credit has been added successfully.";
                        return RedirectToAction("HeadCreditList");
                    }
                    else if (model.HeadCreditId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Head Credit has been updated successfully.";
                        return RedirectToAction("HeadCreditList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.InvList = Common.GetInvoiceNoList(model.BillRefNo);
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("HeadCreditCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofCredit");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "Head Credit");
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                return View(model);
            }
        }
        public ActionResult HeadCreditView(int id)
        {
            try
            {
                HeadCreditModel model = new HeadCreditModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.InvList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("HeadCreditCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofCredit");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "Head Credit");
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                model = coreAccountService.GetHeadCreditDetails(id);
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(155, "Others", Convert.ToDecimal(model.CreditAmount));
                ViewBag.InvList = Common.GetInvoiceNoList(model.BillRefNo);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        private string ValidateHeadCredit(HeadCreditModel model)
        {
            string msg = "Valid";
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal? hdCr = model.CrDetail.Sum(m => m.Amount);
            if (model.CommitmentDetail != null)
            {
                decimal? cmtAmt = model.CommitmentDetail.Sum(m => m.PaymentAmount);
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (cmtAmt > 0 && model.BankAmount != cmtAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            }
            drAmt = drAmt + (model.BankAmount ?? 0);
            if (drAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (hdCr != crAmt)
            //    msg = msg == "Valid" ? "Not a valid entry. Head credit value and Debit value are not equal." : msg + "<br />Not a valid entry. Head credit value and Debit value are not equal";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpPost]
        public JsonResult GetHeadCreditList(HeadCreditSearch model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetHeadCreditList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRefernceNumber(string term, bool type = false)
        {
            try
            {
                var data = Common.GetAllPostedReferenceNumber(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadInvoiceNumber(string refNo)
        {
            try
            {
                var data = Common.GetInvoiceNoList(refNo);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Addition and Reversal     
        public ActionResult AdditionandReversalOHView(int OHReversalId = 0, bool Pfinit = false)
        {
            try
            {
                AdditionandReversalModel model = new AdditionandReversalModel();
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.Dropdown = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DropdownType = Common.GetCodeControlList("AddandRevOHType");
                model.CreditorType = "NA";
                model = coreAccountService.GetAdditionandReversalOHDetails(OHReversalId);
                ViewBag.disabled = "Disabled";
                model.PFInit = Pfinit;
                string type = "Others";
                var projecttype = Common.GetProjectType(model.ProjectId).Item1;
                if (projecttype == 2)
                    type = "Cons";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(106, type, model.TotalAmount);
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        public ActionResult AdditionandReversalOH(int OHReversalId = 0)
        {
            try
            {
                AdditionandReversalModel model = new AdditionandReversalModel();
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.Dropdown = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DropdownType = Common.GetCodeControlList("AddandRevOHType");
                model.CreditorType = "NA";
                if (OHReversalId > 0)
                {
                    model = coreAccountService.GetAdditionandReversalOHDetails(OHReversalId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(63);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult AdditionandReversalOH(AdditionandReversalModel model)
        {
            try
            {
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.Dropdown = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DropdownType = Common.GetCodeControlList("AddandRevOHType");
                model.CreditorType = "NA";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                foreach (var item in model.DocumentDetail)
                {
                    if (item.DocumentFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(item.DocumentFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                }

                if (model.Type == "1")
                {
                    if (model.CommitmentAmount != (model.CommitmentDetail.Sum(m => m.PaymentAmount) ?? 0))
                    {
                        TempData["errMsg"] = "There is a mismatch between the total commitment value and Commitment BreakUp.please contact administrator.";
                        return View(model);
                    }
                }

                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateAdditionandReversalOH(model, logged_in_user);
                if (model.AdditionandReversalId == 0 && result > 0)
                {
                    TempData["succMsg"] = "OH Reversal has been added successfully.";
                    return RedirectToAction("OHReversalList");
                }
                else if (model.AdditionandReversalId > 0 && result > 0)
                {
                    TempData["succMsg"] = "OH Reversal has been updated successfully.";
                    return RedirectToAction("OHReversalList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View(model);
            }
            catch (Exception ex)
            {

                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult OHReversalList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult SearchOHReversalList(OHReversalSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchOHReversalList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApprovalForAdditionandReversalOH(int OHReversalId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cStatus = coreAccountService.AdditionandReversalOHCommitmentBalanceUpdate(OHReversalId, false, false, logged_in_user, "OHAR");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalForReversalOH(OHReversalId, logged_in_user);
                if (!status)
                    coreAccountService.AdditionandReversalOHCommitmentBalanceUpdate(OHReversalId, true, false, logged_in_user, "OHAR");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AdditionandReversalOHWFInit(int id)
        {
            try
            {
                lock (OHARWFInitlockObj)
                {
                    if (Common.ValidateOHAddRevStatus(id, "Open"))
                    {
                        int logged_in_user = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.AdditionandReversalOHCommitmentBalanceUpdate(id, false, false, logged_in_user, "OHAR");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        var result = coreAccountService.OHaddrevWFInit(id, logged_in_user);
                        if (!result.Item1)
                            coreAccountService.AdditionandReversalOHCommitmentBalanceUpdate(id, true, false, logged_in_user, "OHAR");
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForAdditionandReversalOH(int paymentId)
        {
            bool cstatus = false;
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                cstatus = coreAccountService.AdditionandReversalOHBillApproved(paymentId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = cstatus, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LoadPCFandRMF(int projectid, decimal PCFValue, decimal RMFValue)
        {
            try
            {
                CoreAccountsService cs = new CoreAccountsService();
                var output = cs.GetPCFandRMFShareDetails(projectid, PCFValue, RMFValue);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ValidateAddition(decimal Amt, int ProjectId)
        {
            try
            {
                bool cstatus = coreAccountService.ValidateAdditon(Amt, ProjectId);
                if (!cstatus)
                    return Json(new { status = false, msg = "Amount has Exceed the Limit" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ValidateReversal(decimal Amt, int ProjectId)
        {
            try
            {
                bool cstatus = coreAccountService.ValidateReverse(Amt, ProjectId);
                if (!cstatus)
                    return Json(new { status = false, msg = "Amount has Exceed the Limit" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult LoadPCFandRMFProjDetails(int projectid, decimal PCFValue, decimal RMFValue, decimal ICSR, decimal StaffWelfare, decimal DDF)
        {
            try
            {
                CoreAccountsService cs = new CoreAccountsService();
                var output = cs.GetPCFandRMFProjDetails(projectid, PCFValue, RMFValue, ICSR, StaffWelfare, DDF);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetOHReversalList(SearchOHReversal model, int pageIndex, int pageSize, DateFilterModel OHReversalDate)
        {
            try
            {
                object output = coreAccountService.GetOHReversalList(model, pageIndex, pageSize, OHReversalDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Payment Process
        public ActionResult PaymentProcessInitList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetPaymentProcessInitList(string VoucherNumber, string Status, string UTRStatus, int pageIndex, int pageSize, DateFilterModel VoucherDate)
        {
            try
            {
                object output = coreAccountService.PaymentProcessInitList(pageIndex, pageSize, VoucherNumber, Status, UTRStatus, VoucherDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PaymentDetails(string viewType, int bankHeadId, int? boaDraftId = null, int? payeeId = null, bool isEditMode = false, int? modeOfPayment = null)
        {
            try
            {
                if (viewType != "Verify")
                    ViewBag.ViewMode = "true";
                ViewBag.ViewType = viewType != "Single" && viewType != "Verify" ? "Group" : viewType;
                var data = coreAccountService.GetPaymentDetails(viewType, bankHeadId, boaDraftId, payeeId, isEditMode, modeOfPayment);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public ActionResult PaymentProcess(int? boaDraftId = null, string mode = "I")
        {
            try
            {

                PaymentProcessVoucherModel model = new PaymentProcessVoucherModel();
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.BankHeadList = Common.GetBankAccountHeadList(false);
                if (boaDraftId != null)
                    model = coreAccountService.GetPaymentProcessVoucher(boaDraftId ?? 0);
                else
                    model.VoucherDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.BOADraftId = boaDraftId;
                model.Mode = mode;
                ViewBag.fromDate = ConfigurationManager.AppSettings["Posting_fDate"];
                ViewBag.toDate = ConfigurationManager.AppSettings["Posting_tDate"];
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        [HttpPost]
        public ActionResult PaymentProcess(PaymentProcessVoucherModel model)
        {
            try
            {
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.BankHeadList = Common.GetBankAccountHeadList(false);
                ViewBag.fromDate = ConfigurationManager.AppSettings["Posting_fDate"];
                ViewBag.toDate = ConfigurationManager.AppSettings["Posting_tDate"];
                int userId = Common.GetUserid(User.Identity.Name);
                var result = coreAccountService.BOADraftIU(model, userId);
                if (model.BOADraftId == null && result > 0)
                {
                    TempData["succMsg"] = "Payment Process has been initiated successfully.";
                    return RedirectToAction("PaymentProcessInitList");
                }
                else if (model.BOADraftId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Payment Process has been updated successfully.";
                    return RedirectToAction("PaymentProcessInitList");
                }
                else if (result == -2)
                {
                    TempData["succMsg"] = "Please verify at least one payment from the list.";
                    return View(model);
                }
                else if (result == -3)
                {
                    TempData["errMsg"] = "Some of the payment are partially verified.";
                    return View(model);
                }
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator.";

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.fromDate = ConfigurationManager.AppSettings["Posting_fDate"];
                ViewBag.toDate = ConfigurationManager.AppSettings["Posting_tDate"];
                return View(model);
            }

        }
        [HttpGet]
        public JsonResult GetPaymentProcessList(int bankHeadID, int? boaDraftId = null, bool isViewMode = false)
        {
            try
            {
                object output = coreAccountService.GetPaymentProcessList(boaDraftId, bankHeadID, isViewMode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult ExecutePaymentSP(int bankHeadID, int? boaDraftId = null)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                coreAccountService.ExecutePaymentSP(userId);
                object output = coreAccountService.GetPaymentProcessList(boaDraftId, bankHeadID, false);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpPost]
        //public ActionResult VerifyPaymentProcess(int paymentPayeeId, int modeOfPayment)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {

        //            int userId = Common.GetUserid(User.Identity.Name);
        //            bool status = coreAccountService.VerifyPaymentProcess(paymentPayeeId, modeOfPayment, userId);
        //            return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public ActionResult VerifyPaymentProcess(VerifyPaymentProcessModel model)
        {
            try
            {
                lock (PaymentVerifyWFInitlockObj)
                {
                    if (ModelState.IsValid)
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        int draftID = coreAccountService.VerifyPaymentProcess(model, userId);
                        if (draftID > 0)
                            TempData["succMsg"] = "Bill has been verified for payment process successfully.";
                        else if (draftID == -3)
                            TempData["errMsg"] = "Some of the payment are partially verified.";
                        else
                            TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("PaymentProcess", new { boaDraftId = draftID });
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                    }
                    return RedirectToAction("PaymentProcess", new { boaDraftId = Convert.ToInt32(model.DraftId) });
                }
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return RedirectToAction("PaymentProcess", new { boaDraftId = Convert.ToInt32(model.DraftId) });
            }
        }

        [HttpGet]
        public JsonResult ApproveBOADraft(int boaDraftId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool valid = coreAccountService.ValidatePaymentProcess(boaDraftId);
                if (valid)
                {
                    object output = coreAccountService.PaymentBOATransaction(boaDraftId, userId);
                    return Json(output, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult PaymentProcessView(int? BOADraftId = null, bool Pfinit = false)
        {
            using (var context = new IOASDBEntities())
            {
                PaymentProcessVoucherModel model = new PaymentProcessVoucherModel();
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.BankHeadList = Common.GetBankAccountHeadList(false);
                if (BOADraftId != null)
                    model = coreAccountService.GetPaymentProcessVoucher(BOADraftId ?? 0);
                else
                    model.VoucherDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.BOADraftId = BOADraftId;
                model.Mode = "V";
                model.PFInit = Pfinit;
                ViewBag.fromDate = ConfigurationManager.AppSettings["Posting_fDate"];
                ViewBag.toDate = ConfigurationManager.AppSettings["Posting_tDate"];
                decimal? Amt = context.tblBOADraft.Where(m => m.BOADraftId == BOADraftId).Select(m => m.TotalAmount).FirstOrDefault();
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(51, "Others", Amt ?? 0);
                return View(model);

            }

        }

        [HttpPost]
        public ActionResult PaymentProcessWFInit(int billId)
        {
            try
            {
                lock (PaymentWFInitlockObj)
                {
                    if (Common.ValidatePaymentProcessStatus(billId, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool status = coreAccountService.PaymentProcessWFInit(billId, userId);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PaymentDetailsViewMode(string viewType, int bankHeadId, int? boaDraftId = null, int? payeeId = null, bool isEditMode = false, int? modeOfPayment = null)
        {
            try
            {
                if (viewType != "Verify")
                    ViewBag.ViewMode = "true";
                ViewBag.ViewType = viewType != "Single" && viewType != "Verify" ? "Group" : viewType;
                var data = coreAccountService.GetPaymentDetails(viewType, bankHeadId, boaDraftId, payeeId, isEditMode, modeOfPayment);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet]
        public JsonResult SendPaymentFaildEmail(int boaDraftId)
        {
            try
            {

                bool valid = coreAccountService.PaymentFailedEmailSend(boaDraftId);
                //bool valid = coreAccountService.PaymentEmailBackendSend(boaDraftId);
                return Json(valid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SendUTRFaildEmail(int boaDraftId)
        {
            try
            {

                bool valid = coreAccountService.UTRFailedEmailSend(boaDraftId);
                return Json(valid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Project Summary
        public ActionResult ProjectSummary(int ProjectId = 0)
        {
            ProjSummaryModel model = new ProjSummaryModel();
            ProjectSummaryModel psModel = new ProjectSummaryModel();
            ProjectService pro = new ProjectService();
            if (ProjectId > 0)
            {
                model.Detail = pro.getProjectSummaryDetails(ProjectId);
                model.Summary = pro.getProjectSummary(ProjectId);
                model.Common = Common.GetProjectsDetails(ProjectId);
                model.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", model.Common.SancationDate);
                model.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", model.Common.CloseDate);
                model.ProjId = ProjectId;
                model.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                model.ExpAmt = model.Summary.AmountSpent;
            }
            else
                model.Summary = psModel;
            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _ProjectSummary(int ProjectId = 0, string projectNo = "")
        {
            ProjSummaryModel model = new ProjSummaryModel();
            ProjectSummaryModel psModel = new ProjectSummaryModel();
            ProjectService pro = new ProjectService();
            if (ProjectId == 0 && !String.IsNullOrEmpty(projectNo))
                ProjectId = Common.GetProjectId(projectNo);
            if (ProjectId > 0)
            {
                model.Detail = pro.getProjectSummaryDetails(ProjectId);
                model.Summary = pro.getProjectSummary(ProjectId);
                model.Common = Common.GetProjectsDetails(ProjectId);
                model.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", model.Common.SancationDate);
                model.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", model.Common.CloseDate);
                model.ProjId = ProjectId;
                model.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                model.ExpAmt = model.Summary.AmountSpent;
            }
            else
                model.Summary = psModel;
            return View(model);
        }
        #endregion
        #region Project Status 
        public JsonResult UpdateProjectStatus(int ProjectId = 0, string Status = "")
        {
            var empty = new ProjectStatusUpdateModel();
            ProjectStatusUpdateModel data = new ProjectStatusUpdateModel();
            ProjectSummaryModel psModel = new ProjectSummaryModel();
            CoreAccountsService pro = new CoreAccountsService();
            if (ProjectId > 0)
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                data.Message = pro.UpdateProjectStatus(ProjectId, Status, logged_in_user) == true ? "Success" : "Failed";
            }
            else
                data = empty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectStatusChanger()
        {
            List<MasterlistviewModel> list = new List<MasterlistviewModel>();
            ViewBag.StatusChanger = list;
            return View();
        }
        [HttpGet]
        public JsonResult LoadAllProjectNumber(string term)
        {
            try
            {
                var data = Common.GetAllProjectNumber(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetProjectStatus(int ProjectId)
        {
            try
            {
                var data = Common.GetCurrentProjectStatus(ProjectId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region PFT Date
        [HttpGet]
        public ActionResult PFTBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult PFTBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdatePFTBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadPFTRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "PFT" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public JsonResult GetPFTBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetPFTCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult DateModificationRange(string RefNo)
        {
            var Result = Common.DateModificationRange(RefNo);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Contra Date
        [HttpGet]
        public ActionResult ContraBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ContraBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateContraBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadContraRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "CTR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetContraBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetContraCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Imprest Date
        [HttpGet]
        public ActionResult ImprestBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ImprestBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateImprestBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadImprestRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "IMR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetImprestBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetImprestCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region ImprestRecoup Date
        [HttpGet]
        public ActionResult ImprestRecoupBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ImprestRecoupBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateImprestRecoupBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadImprestRecoupRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "IBR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetImprestRecoupBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetImprestRecoupCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region PTP Date
        [HttpGet]
        public ActionResult PTPBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult PTPBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdatePTPBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadPTPRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "PTP" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPTPBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetPTPCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region SMI Date
        [HttpGet]
        public ActionResult SMIBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult SMIBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateSMIBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadSMIRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "SMI" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetSMIBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetSMICurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region DIS Date
        [HttpGet]
        public ActionResult DISBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult DISBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateDISBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadDISRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "DIS" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetDISBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetDISCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region FRM Date
        [HttpGet]
        public ActionResult FRMBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult FRMBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateFRMBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadFRMRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "FRM" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetFRMBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetFRMCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region Fixed Deposit
        [HttpGet]
        public ActionResult FixedDeposit(int Fixeddepositid = 0)
        {
            FixedDepositModel model = new FixedDepositModel();
            ViewBag.acctype = Common.GetBankAccounttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.AccountHeadList = emptyList;
            ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
            ViewBag.AccountGroupList = Common.GetAccountGroup(false);
            ViewBag.DocmentTypeList = Common.GetDocTypeList(133);
            ViewBag.BankList = Common.GetBankAccountHeadList();
            if (Fixeddepositid > 0 && Common.ValidateFixeddepositOnEdit(Fixeddepositid))
            {
                model = coreAccountService.GetFixedDepositdetail(Fixeddepositid);

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult FixedDeposit(FixedDepositModel model)
        {
            try
            {
                ViewBag.acctype = Common.GetBankAccounttype();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(133);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateFixedDeposit(model, logged_in_user);
                    if (model.FixedDepositId == null && result == 1)
                    {
                        TempData["succMsg"] = "Fixed deposit has been added successfully.";
                        return RedirectToAction("FixedDepositList");
                    }
                    else if (model.FixedDepositId == null && result == 3)
                    {
                        TempData["alertMsg"] = "This Fixed Deposit Number Already Exists";
                        return RedirectToAction("FixedDepositList");
                    }
                    else if (model.FixedDepositId > 0 && result == 2)
                    {
                        TempData["succMsg"] = "Fixed deposit has been Updated successfully.";
                        return RedirectToAction("FixedDepositList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("FixedDepositList");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(133);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetAccountlist(int acctid)
        {
            object result = Common.GetAccountNumber(acctid);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAccountNumber(int bankid)
        {
            object result = Common.GetBankAccount(bankid);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FixedDepositList()
        {
            return View();
        }
        public ActionResult FixedDepositView(int Fixeddepositid = 0, bool PF = false)
        {
            FixedDepositModel model = new FixedDepositModel();
            model = coreAccountService.GetFixedDepositdetailView(Fixeddepositid);
            model.PF = PF;
            return View(model);
        }
        public JsonResult ApprovalForFixedDeposit(int FixedDepositid)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var status = Common.ApprovalForFixedDeposit(FixedDepositid, logged_in_user);
                if (!status)
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = status, msg = "Submit for Approval Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForFixedDeposit(int FixedDepositId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.FixedDepositBillApproved(FixedDepositId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                //bool boaStatus = coreAccountService.HonororiumBOATransaction(HonorId);
                //var status = Common.ApprovalPendingForHonororium(HonorId, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetFixedDepositList(SearchFixedDeposit model, int pageIndex, int pageSize, DateFilterModel FromDatetime, DateFilterModel TofromDate)
        {
            try
            {
                object output = coreAccountService.GetFixdepositList(model, pageIndex, pageSize, FromDatetime, TofromDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Fixed Deposit Close
        [HttpGet]
        public ActionResult FixedDepositClose(int FixedepositId = 0, int FixeddepositclosedId = 0)
        {
            FixedDepositClosedModel model = new FixedDepositClosedModel();
            ViewBag.acctype = Common.GetBankAccounttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.AccountHeadList = emptyList;
            ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
            ViewBag.AccountGroupList = Common.GetAccountGroup(false);
            ViewBag.DocmentTypeList = Common.GetDocTypeList(134);
            ViewBag.BankList = Common.GetBankAccountHeadList();
            if (FixedepositId > 0 && Common.ValidateFixeddepositOnClose(FixedepositId))
            {
                model = coreAccountService.GetFixedDepositDetails(FixedepositId);

            }
            if (FixeddepositclosedId > 0 && Common.ValidateFixeddepositOnCloseedit(FixeddepositclosedId))
            {
                model = coreAccountService.GetFixedDepositCloseddetail(FixeddepositclosedId);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult FixedDepositClose(FixedDepositClosedModel model)
        {
            try
            {
                ViewBag.acctype = Common.GetBankAccounttype();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(134);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateFixedDepositClose(model, logged_in_user);
                    if (model.FixedDepositClosedId == null && result == 1)
                    {
                        TempData["succMsg"] = "Fixed deposit  Closed added successfully.";
                        return RedirectToAction("FixedDepositCloseList");
                    }
                    else if (model.FixedDepositClosedId == null && result == 3)
                    {
                        TempData["alertMsg"] = "This Fixed Deposit Number Already Exists";
                        return RedirectToAction("FixedDepositCloseList");
                    }
                    else if (model.FixedDepositId > 0 && result == 2)
                    {
                        TempData["succMsg"] = "Fixed deposit Closed has been Updated successfully.";
                        return RedirectToAction("FixedDepositCloseList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("FixedDepositCloseList");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(134);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        [HttpPost]
        public JsonResult GetFixedDepositcloseList(SearchFixedDepositClose model, int pageIndex, int pageSize)
        {
            try
            {
                object output = coreAccountService.GetFixdepositCloseList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult FixedDepositCloseList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult LoadFixeddepositList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteFixedDepositNumber(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public JsonResult GetFixedDepositData(int Fixeddepositid)
        {
            try
            {
                FixedDepositClosedModel model = new FixedDepositClosedModel();
                model = coreAccountService.GetFixedDepositDetails(Fixeddepositid);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                FixedDepositClosedModel model = new FixedDepositClosedModel();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalForFixedDepositclose(int FixeddepositclosedId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var status = Common.ApprovalForFixedDepositclose(FixeddepositclosedId, logged_in_user);
                if (!status)
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = status, msg = "Submit for Approval Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForFixedDepositClose(int FixeddepositclosedId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.FixedDepositBillApprovedclosed(FixeddepositclosedId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                //bool boaStatus = coreAccountService.HonororiumBOATransaction(HonorId);
                //var status = Common.ApprovalPendingForHonororium(HonorId, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult FixedDepositCloseView(int FixeddepositclosedId = 0, bool PF = false)
        {
            FixedDepositClosedModel model = new FixedDepositClosedModel();
            model = coreAccountService.GetFixedDepositClosedetailView(FixeddepositclosedId);
            model.PF = PF;
            return View(model);
        }
        #endregion
        #region GSTOffset
        [HttpGet]
        public ActionResult GSTOffset(int GSTOffsetId = 0)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                GSTOffsetModel model = new GSTOffsetModel();
                model.CreditorType = "NA";
                if (GSTOffsetId > 0 && Common.ValidateGSTOffsetOnEdit(GSTOffsetId))
                {
                    model = coreAccountService.GetGSTOffsetDetails(GSTOffsetId);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GSTOffset(GSTOffsetModel model)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;

                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateGSTOffset(model, logged_in_user);
                    if (model.GSTOffsetId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "GSTOffset has been added successfully.";
                        return RedirectToAction("GSTOffsetList");
                    }
                    else if (model.GSTOffsetId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "GSTOffset has been updated successfully.";
                        return RedirectToAction("GSTOffsetList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        [HttpGet]
        public JsonResult GetGSTInput(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetGSTInput(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetGSTOffsetTotal(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetGSTOffsetTotal(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetGSTOutputList(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetGSTOutputList(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetTDSList(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetTDSList(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetRCMInputList(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetRCMInputList(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetRCMOutputList(DateTime fromdate, DateTime todate, int GSToffsetid = 0)
        {
            try
            {
                object output = CoreAccountsService.GetRCMOutputList(fromdate, todate, GSToffsetid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetPreviousGST()
        {
            try
            {
                object output = CoreAccountsService.GetPreviousGST();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GSTOffsetList()
        {
            return View();
        }
        public JsonResult ApprovalForGSTOffset(int GSTOffsetId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForGSTOffset(GSTOffsetId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForGSTOffset(int GSTOffsetId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.GSTOffsetBOATransaction(GSTOffsetId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalPendingForGSTOffset(GSTOffsetId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult GetGSTOffsetList()
        {
            try
            {
                object output = coreAccountService.GetGSTOffsetList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchGSTOffsetList(GSTOffsetSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchGSTOffsetList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GSTOffsetView(int GSTOffsetId = 0)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                GSTOffsetModel model = new GSTOffsetModel();
                model.CreditorType = "NA";
                model = coreAccountService.GetGSTOffsetDetails(GSTOffsetId);
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetGSTOffsetList(SearchGSTOffset model, int pageIndex, int pageSize, DateFilterModel GSTOffsetDate)
        {
            try
            {
                object output = coreAccountService.GetGSTOffsetList(model, pageIndex, pageSize, GSTOffsetDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region GST Credit
        public ActionResult GSTCredit()
        {
            GSTCredit model = new GSTCredit();
            return View(model);
        }
        [HttpPost]
        public ActionResult GSTCredit(GSTCredit model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            int Id = coreAccountService.SaveGSTCredit(model, logged_in_user);
            if (Id > 0)
            {
                TempData["succMsg"] = "It has been added successfully.";
                // model = null;
                return View(model);
            }
            else if (Id < 0)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetGSTCredit(string GSTIN = "", string InvoiceNo = "", string RefNo = "")
        {
            try
            {
                object output = coreAccountService.GetGSTCredit(GSTIN, InvoiceNo, RefNo);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult LoadGSTINList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteGSTIN(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadInvoiceNoList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteInvoiceNo(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRefNoList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteRefNo(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region ContractorBill
        public ActionResult ContractorBillList()
        {
            ContractorBillModel model = new ContractorBillModel();
            return View(model);
        }
        public ActionResult ContractorBill(int ContractorBillId = 0)
        {
            ContractorBillModel model = new ContractorBillModel();
            var finyear = Common.GetCurrentFinYearId();
            model = coreAccountService.getEditContractorBill(ContractorBillId);
            return View(model);
        }
        [HttpPost]
        public ActionResult ContractorBill(ContractorBillModel model)
        {
            int ContBilId = 0;
            if (ModelState.IsValid)
            {
                if (model.FinalAttachDoc != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    string docname = Path.GetFileName(model.FinalAttachDoc.FileName);
                    var docextension = Path.GetExtension(docname);
                    if (!allowedExtensions.Contains(docextension))
                    {
                        TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                        return RedirectToAction("Tapal");
                    }
                }
                int UserId = Common.GetUserid(User.Identity.Name);
                var Result = coreAccountService.SaveContracterBillDetails(model, UserId);
                ContBilId = Result.Item2;
                if (Result.Item1 == 1)
                {
                    TempData["succMsg"] = "Saved Sucessfully";
                }
                else if (Result.Item1 == 2)
                {
                    TempData["succMsg"] = "Updated Sucessfully";
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }
            //DownloadContractorBill(ContBilId);
            return RedirectToAction("ContractorBillList", "CoreAccounts");
        }

        [HttpPost]
        public JsonResult GetContractorBillDetails(ContractSearchModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate)
        {
            object output = coreAccountService.GetContrBillDetails(model, pageIndex, pageSize, CreatedDate);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowUploadedDocument(string file, string filepath)
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
        #region ContingentBill
        public ActionResult ContingentBillList()
        {
            return View();
        }
        public ActionResult ContingentBill(int ContingenBillId = 0)
        {
            ContingentBillModel model = new ContingentBillModel();
            var finyear = Common.GetCurrentFinYearId();
            model = coreAccountService.getEditContingetBill(ContingenBillId);
            return View(model);
        }
        [HttpPost]
        public ActionResult ContingentBill(ContingentBillModel model)
        {
            int ContgBilId = 0;
            if (ModelState.IsValid)
            {
                if (model.FinalAttachDoc != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    string docname = Path.GetFileName(model.FinalAttachDoc.FileName);
                    var docextension = Path.GetExtension(docname);
                    if (!allowedExtensions.Contains(docextension))
                    {
                        TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                        return RedirectToAction("Tapal");
                    }
                }
                int UserId = Common.GetUserid(User.Identity.Name);
                var Result = coreAccountService.SaveContingentBillDetails(model, UserId);
                ContgBilId = Result.Item2;
                if (Result.Item1 == 1)
                {
                    TempData["succMsg"] = "Saved Sucessfully";
                }
                else if (Result.Item1 == 2)
                {
                    TempData["succMsg"] = "Updated Sucessfully";
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }

            return RedirectToAction("ContingentBillList", "CoreAccounts");
        }


        [HttpPost]
        public JsonResult GetContingentBillDetails(ContingentSearchModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate)
        {
            object output = coreAccountService.GetContingentBillDetails(model, pageIndex, pageSize, CreatedDate);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region DOP Date
        [HttpGet]
        public ActionResult DOPBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult DOPBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateDOPBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadDOPRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "DOP" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetDOPBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetDOPCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region OHAR Date
        [HttpGet]
        public ActionResult OHARBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult OHARBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateOHARBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadOHARRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "OHAR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetOHARBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetOHARCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region FDC Date
        [HttpGet]
        public ActionResult FDCBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult FDCBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateFDCBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadFDCRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "FDC" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetFDCBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetFDCCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region FDT Date
        [HttpGet]
        public ActionResult FDTBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult FDTBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateFDTBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadFDTRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "FDT" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetFDTBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetFDTCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region GstOffsetDateChange
        [HttpGet]
        public ActionResult GstOffSetDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult GstOffSetDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateGstOffsetDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadGOFRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "GOF" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetGOFBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetGOFCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region TdsPaymentDateChange
        [HttpGet]
        public ActionResult TdsPaymentDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult TdsPaymentDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.TdsPaymentDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadTdsPaymentRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "TXP" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetTdsPaymentBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetTdsPaymentCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Admin Voucher
        [HttpGet]
        public ActionResult AdminVoucher(int id = 0)
        {
            GeneralVoucherModel model = new GeneralVoucherModel();
            var emptyList = new List<MasterlistviewModel>();
            //ViewBag.AccountHeadList = Common.GetAccHeadforAdminVoucher();
            ViewBag.AccountHeadList = emptyList;
            ViewBag.BankList = Common.GetBankAccountHeadList(true);
            //ViewBag.AccountGroupList = Common.GetAccGroupforAdminVoucher();
            ViewBag.AccountGroupList = Common.GetAccountGroup(false);
            ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
            ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "OH");
            ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
            ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
            ViewBag.TDSSectionList = Common.GetTdsList();
            if (id > 0)
            {
                model = coreAccountService.GetAdminVoucherDetails(id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminVoucher(GeneralVoucherModel model)
        {
            var emptyList = new List<MasterlistviewModel>();
            //ViewBag.AccountHeadList = Common.GetAccHeadforAdminVoucher();
            ViewBag.AccountHeadList = emptyList;
            ViewBag.BankList = Common.GetBankAccountHeadList(true);
            //ViewBag.AccountGroupList = Common.GetAccGroupforAdminVoucher();
            ViewBag.AccountGroupList = Common.GetAccountGroup(false);
            ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
            ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "OH");
            ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
            ViewBag.TDSSectionList = Common.GetTdsList();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
            int logged_in_user = Common.GetUserid(User.Identity.Name);

            string validationMsg = ValidateAdminVoucher(model);
            if (validationMsg != "Valid")
            {
                TempData["errMsg"] = validationMsg;
                return View(model);
            }
            int result = coreAccountService.AdminVoucherIU(model, logged_in_user);
            if (model.VoucherId == null && result > 0)
            {
                TempData["succMsg"] = "Admin voucher has been added successfully.";
                return RedirectToAction("AdminVoucherList");
            }
            else if (model.VoucherId > 0 && result > 0)
            {
                TempData["succMsg"] = "Admin voucher has been updated successfully.";
                return RedirectToAction("AdminVoucherList");
            }
            else
                TempData["errMsg"] = "Something went wrong please contact administrator.";

            return View(model);
        }
        public ActionResult AdminVoucherList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetAdminVoucherList()
        {
            try
            {
                object output = coreAccountService.GetAdminVoucherList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveAdminVoucher(int id = 0)
        {
            try
            {
                lock (AVOApprovelockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool output = coreAccountService.ApproveAdminVoucher(id, userId);
                    if (!output)
                        return Json(new { output = false, msg = "Something went wrong ,pls contact Admin." }, JsonRequestBehavior.AllowGet);
                    return Json(output, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult AdminVoucherView(int id = 0, bool Pfinit = false)
        {
            try
            {
                ViewBag.processGuideLineId = 0;
                GeneralVoucherModel model = new GeneralVoucherModel();
                var emptyList = new List<MasterlistviewModel>();
                //ViewBag.AccountHeadList = Common.GetAccHeadforAdminVoucher();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.BankList = Common.GetBankAccountHeadList(false);
                //ViewBag.AccountGroupList = Common.GetAccGroupforAdminVoucher();
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "OH");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.TDSSectionList = Common.GetTdsList();
                if (id > 0)
                {
                    model = coreAccountService.GetAdminVoucherDetails(id);
                }
                ViewBag.disabled = "Disabled";
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(151, "Others", model.PaymentDebitAmount ?? 0);
                return View("ApproveAdminView", model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        [HttpGet]
        public JsonResult SubmitforApproveAdminVoucher(int id = 0)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool output = coreAccountService.SubmitForApproveAdminVoucher(id, userId);
                if (!output)
                    return Json(new { status = output, msg = !output ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AdminVoucherWFInit(int id)
        {
            try
            {
                lock (AVOWFInitlockObj)
                {
                    if (Common.ValidateAdminvoucherStatus(id, "Open"))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        var result = coreAccountService.AdminVoucherWFInit(id, userId);
                        return Json(new { status = result.Item1, msg = !result.Item1 ? result.Item2 : "" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetAdminVoucherList(GeneralVoucherSearch model, int pageIndex, int pageSize, DateFilterModel PostedDate)
        {
            try
            {
                object output = coreAccountService.GetAdminVoucherList(model, pageIndex, pageSize, PostedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private string ValidateAdminVoucher(GeneralVoucherModel model)
        {
            string msg = "Valid";
            decimal crAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            decimal bankAmt = model.PaymentBankAmount ?? 0;
            crAmt = crAmt + bankAmt;
            if (bankAmt != paymentBUAmt)
                msg = msg == "Valid" ? "Not a valid entry.The Payable value and Payment Break Up Total value are not equal." : msg + "<br /> Not a valid entry.The Payable value and Payment Break Up Total value are not equal.";

            if (drAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var ah = model.PaymentExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            //if (ah.Count() != gAH.Count())
            //    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            //foreach (var item in model.PaymentExpenseDetail)
            //{
            //    if (item.TransactionType=="Debit")
            //    {
            //        int headId = item.AccountHeadId ?? 0;
            //        decimal balAmt = Common.GetLeadgerBalance(headId);
            //        if (balAmt < item.Amount)
            //        {
            //            msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
            //            break;
            //        }
            //    }               
            //}

            return msg;
        }
        public JsonResult GetLeadgerBalance(int AccountHeadId)
        {
            object output = Common.GetLeadgerBalance(AccountHeadId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Deligate

        public ActionResult Deligate(int Id = 0)
        {
            DeligateModel model = new DeligateModel();
            if (Id > 0)
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblUser.Where(m => m.UserId == Id).FirstOrDefault();
                    if (Qry != null)
                    {
                        model.ApproverName = Qry.FirstName + "-" + Qry.UserName;
                        model.ApproverId = Qry.UserId;
                    }
                }
                model.ListModel = coreAccountService.GetProcessFlowHistory(Id);
            }
            model.ListModel = model.ListModel ?? new List<DeligateListModel>();
            return View(model);
        }
        [HttpGet]
        public JsonResult AddDelegate(int ApproverId, int HeadId, int ProcessId, int DelegatedToId)
        {
            try
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.AddDelegate(ApproverId, HeadId, ProcessId, DelegatedToId, logged_in_user);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult RemoveDelegate(int ApproverId, int HeadId, int ProcessId, int DelegatedToId)
        {
            try
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.RemoveDelegate(ApproverId, HeadId, ProcessId, DelegatedToId, logged_in_user);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult GetDelegatedName(int ApproverId, int HeadId, int ProcessId)
        {
            try
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.GetDelegatedName(ApproverId, HeadId, ProcessId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region OHP Date
        [HttpGet]
        public ActionResult OHPBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult OHPBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateOHPBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadOHPRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "OHP" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetOHPBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetOHPCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region Form 3C
        public ActionResult Form3CList()
        {
            Form3CModel model = new Form3CModel();
            return View(model);
        }
        [HttpGet]
        public ActionResult Form3C()
        {
            ViewBag.FormType = Common.GetCodeControlList("Form3C");
            ViewBag.RecNo = Common.GetAutoCompleteReceiptNo();
            return View();
        }
        [HttpPost]
        public ActionResult Form3C(Form3CModel model)
        {

            DownloadForm3C(model);
            return RedirectToAction("Form3CList", "CoreAccounts");
        }
        [HttpGet]
        public JsonResult GetForm3C(int ProjectId)
        {
            try
            {
                Common com = new Common();
                object data = CoreAccountsService.GetForm3CData(ProjectId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public FileResult DownloadForm3C(Form3CModel model)
        {
            var App = new Microsoft.Office.Interop.Word.Application();
            var Doc = new Microsoft.Office.Interop.Word.Document();
            int UserId = Common.GetUserid(User.Identity.Name);
            int Form3cId = CoreAccountsService.SaveForm3C(model, UserId);
            model = CoreAccountsService.GetForm3C(Form3cId);
            string Path = "";
            if (model.FormTypeId == 1)
                Path = Server.MapPath("~/Content/Form3CH/Form3CH.doc");
            if (model.FormTypeId == 2)
                Path = Server.MapPath("~/Content/Form3CH/Form3CI.doc");
            if (model.FormTypeId == 3)
                Path = Server.MapPath("~/Content/Form3CH/Form3CJ.doc");
            Doc = App.Documents.Add(Template: Path);
            foreach (Microsoft.Office.Interop.Word.Field field in Doc.Fields)
            {
                var result = field.Code.Text.Split(' ').Skip(3).FirstOrDefault();
                if (result == "Name")
                {
                    field.Select();
                    App.Selection.TypeText(model.Name ?? " ");
                }
                if (result == "Address")
                {
                    field.Select();
                    App.Selection.TypeText(model.Address ?? " ");
                }
                if (result == "PAN")
                {
                    field.Select();
                    App.Selection.TypeText(model.PAN ?? " ");
                }
                if (result == "ProjectTitle")
                {
                    field.Select();
                    App.Selection.TypeText(model.ProjectTitle ?? " ");
                }

                if (result == "ProgramName")
                {
                    field.Select();
                    App.Selection.TypeText(model.ProgramName ?? " ");
                }
                if (result == "RefNo")
                {
                    field.Select();
                    App.Selection.TypeText(model.RefNo ?? " ");
                }
                if (result == "RefDate")
                {
                    field.Select();
                    App.Selection.TypeText(model.RefDate ?? " ");
                }
                if (result == "StartDate")
                {
                    field.Select();
                    App.Selection.TypeText(model.StartDate ?? " ");
                }
                if (result == "Duration")
                {
                    field.Select();
                    App.Selection.TypeText(model.Duration ?? " ");
                }
                if (result == "Assyear")
                {
                    field.Select();
                    App.Selection.TypeText(model.Assyear ?? " ");
                }
                if (result == "Cost")
                {
                    field.Select();
                    App.Selection.TypeText(model.SancValue ?? "");
                }
                if (result == "RecAmt")
                {
                    field.Select();
                    App.Selection.TypeText(model.RecAmt ?? " ");
                }
                if (result == "CheckNo")
                {
                    field.Select();
                    App.Selection.TypeText(model.CheckNo ?? " ");
                }

                if (result == "Checkdate")
                {
                    field.Select();
                    App.Selection.TypeText(model.Checkdate ?? " ");
                }
                if (result == "RecCount")
                {
                    field.Select();
                    App.Selection.TypeText(model.RecCount ?? "");
                }
                if (result == "OverallRecAmt")
                {
                    field.Select();
                    App.Selection.TypeText(model.OverallRecAmt ?? " ");
                }
                if (result == "ProjectNo")
                {
                    field.Select();
                    App.Selection.TypeText(model.ProjectNo ?? " ");
                }
                if (result == "ReceiptNo")
                {
                    field.Select();
                    App.Selection.TypeText(model.ReceiptNo ?? " ");
                }
                if (result == "DeanName")
                {
                    field.Select();
                    App.Selection.TypeText(model.DeanName ?? " ");
                }
                if (result == "NatureOfBus")
                {
                    field.Select();
                    App.Selection.TypeText(model.NatureOfBus ?? " ");
                }
                if (result == "TurnOver")
                {
                    field.Select();
                    App.Selection.TypeText(model.TurnOver ?? " ");
                }
                if (result == "Installment")
                {
                    field.Select();
                    App.Selection.TypeText(model.Installment ?? " ");
                }
                if (result == "Place")
                {
                    field.Select();
                    App.Selection.TypeText("Chennai");
                }
                if (result == "Date")
                {
                    field.Select();
                    App.Selection.TypeText(string.Format("{0:dd-MMM-yyyy}", DateTime.Now));
                }
                if (result == "AnnualResearch")
                {
                    field.Select();
                    App.Selection.TypeText(model.AnnualResearch ?? " ");
                }
                if (result == "DeductionDetails")
                {
                    field.Select();
                    App.Selection.TypeText(model.DeductionDetails ?? " ");
                }
                if (result == "AgreementDetails")
                {
                    field.Select();
                    App.Selection.TypeText(model.AgreementDetails ?? " ");
                }
            }
            var fileId = Guid.NewGuid().ToString();
            string DocPath = ""; string DocName = "";
            if (model.FormTypeId == 1)
            {
                DocPath = "~/Content/Form3CH/";
                DocName = "Form3CH.docx";
            }
            if (model.FormTypeId == 2)
            {
                DocPath = "~/Content/Form3CH/";
                DocName = "Form3CI.docx";
            }
            if (model.FormTypeId == 3)
            {
                DocPath = "~/Content/Form3CH/";
                DocName = "Form3CJ.docx";
            }

            var AddPath = Server.MapPath(DocPath);
            var docName = fileId + DocName;
            Doc.SaveAs2(FileName: AddPath + "" + docName);
            Doc.Close();
            App.Quit();

            int Result = coreAccountService.UpdateForm3CNewGeneratedDoc(Form3cId, docName, UserId);
            byte[] fileBytes = System.IO.File.ReadAllBytes(AddPath + "" + docName);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", docName);
        }
        [HttpGet]
        public JsonResult GetAutoCompleteReceiptNo(int? ProjectId = null)
        {
            object output = Common.GetAutoCompleteReceiptNo(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetReceiptDetForm3C(int Receiptid, int ProjectId)
        {
            object output = CoreAccountsService.GetForm3CRecDetail(Receiptid, ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetForm3CBillDetails(Form3CSearchModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate)
        {
            object output = coreAccountService.GetForm3CDetails(model, pageIndex, pageSize, CreatedDate);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetForm3CProjectDoc(int ProjectId)
        {
            try
            {
                object output = CoreAccountsService.GetForm3CProjectDoc(ProjectId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region AVO Date
        [HttpGet]
        public ActionResult AVOBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult AVOBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateAVOBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadAVORefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "AVO" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAVOBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetAVOCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region HCR Date
        [HttpGet]
        public ActionResult HCRBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult HCRBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateHCRDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadHCRRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "HCR" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetHCRBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetHCRCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Pay in slip
        public ActionResult PayinslipList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetPayinslipList(int pageIndex, int pageSize, SearchInvoiceModel model, DateFilterModel Invoicedatestrng, DateFilterModel PayinslipDate)
        {
            try
            {
                object output = coreAccountService.GetPayinslipList(model, pageIndex, pageSize, Invoicedatestrng, PayinslipDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult PayinslipView(int PayinslipId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.state = Common.GetStatelist();
                CreateInvoiceModel model = new CreateInvoiceModel();
                model = coreAccountService.GetPayinslip(PayinslipId);

                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View("PayinslipView", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion
        #region CommonProjectSearchModelLoadRefernceNumber
        [HttpPost]
        public JsonResult GetCommonProjectSearch(string TypeCode, int RefId, bool inv_f = false, bool commit_f = false, bool project_f = false)
        {
            var locationdata = Common.GetCommonProjectSearch(TypeCode, RefId, inv_f, commit_f, project_f);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Common

        [Authorized]
        [HttpPost]
        public JsonResult GetCurrencyCodeById(string CurrencyId)
        {
            CurrencyId = CurrencyId == "" ? "0" : CurrencyId;
            object output = Common.GetCurrencyCode(Convert.ToInt32(CurrencyId));
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptListbyProject(int ProjectId)
        {
            var receiptlist = Common.GetReceiptListbyProjectId(ProjectId);
            //var curr = GenericServices.RiskProGenServices.getCompanyCurr(CompanyName);
            var data = new { rcvlist = receiptlist };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadAllProjectNo(string term, int? type = null)
        {
            try
            {
                var data = Common.GetAllProjectNumber(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public JsonResult ValidateProjectSummary(int ProjectId, int HeadId, decimal Amt)
        {
            var result = Common.ValidateProjectSummary(ProjectId, HeadId, Amt);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region UTR
        public ActionResult UTR(int boaDraftId)
        {
            try
            {

                UTRModel model = new UTRModel();
                if (Common.CheckUTRProcessed(boaDraftId))
                {
                    TempData["errMsg"] = "UTR already processed.";
                    return RedirectToAction("PaymentProcessInitList");
                }
                model.BOADraftId = boaDraftId;
                model.BatchNumber = Common.GetPaymentBatchNumber(boaDraftId);
                //if (boaDraftId > 0)
                //    model = coreAccountService.GetPaymentProcessVoucher(boaDraftId);
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult UTR(UTRModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Common.CheckUTRProcessed(model.BOADraftId.GetValueOrDefault(0)))
                    {
                        TempData["errMsg"] = "UTR already processed.";
                        return RedirectToAction("PaymentProcessInitList");
                    }
                    model = coreAccountService.VerifyUTR(model, true);
                    if (model.IsValid == false)
                    {
                        TempData["errMsg"] = "There is a mismatch between the list showen and verify. Kindly re-verify and submit again.";
                        return View(model);
                    }
                    else if (!model.txDetail.Any(m => m.BOADraftDetailId > 0))
                    {
                        TempData["errMsg"] = "You need to match atleast one UTR.";
                        return View(model);
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.UTRIU(model, logged_in_user);
                    if (result > 0)
                    {
                        TempData["succMsg"] = "UTR has been added successfully.";
                        return RedirectToAction("PaymentProcessInitList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }

        }

        [HttpPost]
        public JsonResult ImportUTRStatement(HttpPostedFileBase file, int boaDraftId)
        {
            Utility _uty = new Utility();
            UTRModel model = new UTRModel();
            List<UTRStatementDetailModel> list = new List<UTRStatementDetailModel>();
            string msg = "Valid";
            if (file != null)
            {
                //using (var context = new IOASDBEntities())
                //{
                //    var query = context.vw_CanaraBankBulkDetails.Where(m => m.BOADraftId == 4270).ToList();


                //    List<string> txUTRDuplicateDetail = new List<string>();
                //    var dupes = query.GroupBy(x => new { x.SendertoReceiverInfo, x.BeneficiaryAccountNo, x.Amount })
                //       .Where(x => x.Skip(1).Any()).ToArray();
                //    foreach (var item in dupes)
                //    {
                //        txUTRDuplicateDetail.Add(item.Select(m => m.SendertoReceiverInfo).FirstOrDefault());
                //    }
                //}
                string extension = Path.GetExtension(file.FileName).ToLower();
                string connString = "";
                string[] validFileTypes = { ".xls", ".xlsx" };
                string actName = Path.GetFileName(file.FileName);
                var guid = Guid.NewGuid().ToString();
                var docName = guid + "_" + actName;
                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/UTRStatement"), docName);
                model.DocumentActualName = actName;
                model.DocumentName = docName;
                model.DocumentPath = "~/Content/UTRStatement";
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/UTRStatement"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    file.SaveAs(path1);
                    //file.UploadFile("UTRStatement", docName);
                    //Connection String to Excel Workbook  
                    var query = "SELECT * FROM [Sheet0$] where Name is not null and Name <> ''";
                    if (extension.ToLower().Trim() == ".csv")
                    {
                        DataTable dt = _uty.ConvertCSVtoDataTable(path1);
                        list = Converter.GetUTREntityList<UTRStatementDetailModel>(dt);
                    }
                    else if (extension.ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString, "", query);
                        list = Converter.GetUTREntityList<UTRStatementDetailModel>(dt);
                    }
                    else
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString, "", query);
                        list = Converter.GetUTREntityList<UTRStatementDetailModel>(dt);
                    }
                }
                else
                {
                    msg = "Please Upload Files in .xls or .xlsx format";
                }
                //if(path1!="")
                //    System.IO.File.Delete(path1);
            }
            model.BOADraftId = boaDraftId;
            model.txDetail = list;
            if (list.Count > 0)
                model = coreAccountService.VerifyUTR(model);
           return Json(new { status = msg, data = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Journal Date
        [HttpGet]
        public ActionResult JournalBillDateChange()
        {
            BillStatusModel model = new BillStatusModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult JournalBillDateChange(BillStatusModel model)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            var empty = new BillStatusModel();
            BillStatusModel data = new BillStatusModel();
            CoreAccountsService pro = new CoreAccountsService();
            TempData["errMsg"] = pro.UpdateJournalBillDate(model, logged_in_user) == true ? "Success" : "Failed";
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadJournalRefernceNumber(string term)
        {
            try
            {
                string[] TypeCode = { "JV" };
                var data = Common.GetAllReferenceNumber(TypeCode, term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetJournalBillDate(string RefNumber)
        {
            try
            {
                Common com = new Common();
                object data = com.GetJournalCurrentDate(RefNumber);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Test
        
        public JsonResult TestPass(int userid)
        {
            var Pass = "";
            using (var context = new IOASDBEntities())
            {
                var userquery = context.tblUser.SingleOrDefault(dup => dup.UserId == userid && dup.Status == "Active");
                if (userquery != null)
                    Pass = userquery.Password;
            }
            string temppass = Cryptography.Decrypt(Pass, "LFPassW0rd");
            return Json(temppass, JsonRequestBehavior.AllowGet);
        }

        
        #endregion

    }
}