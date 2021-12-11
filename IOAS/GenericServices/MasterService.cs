using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class MasterService
    {
        public static VendorMasterViewModel EditVendor(int vendorId)        {            try            {                using (var context = new IOASDBEntities())                {                    VendorMasterViewModel editVendor = new VendorMasterViewModel();                    var query = (from V in context.tblVendorMaster                                 where (V.VendorId == vendorId && V.Status == "Open")                                 select V).FirstOrDefault();                    var Gstfile = (from G in context.tblGstDocument                                   where G.VendorId == vendorId && G.IsCurrentVersion != true                                   select G).ToList();                    var vendorfile = (from vf in context.tblReverseTaxDocument                                      where vf.VendorId == vendorId && vf.IsCurrentVersion != true                                      select vf).ToList();                    var tdsFile = (from TF in context.tblTDSDocument                                   where TF.VendorId == vendorId && TF.IsCurrentVersion != true                                   select TF).ToList();                    var tdsdetail = (from TD in context.tblVendorTDSDetail                                     where TD.VendorId == vendorId                                     select TD).ToList();                    if (query != null)                    {                        editVendor.VendorId = query.VendorId;                        editVendor.Nationality = Convert.ToInt32(query.Nationality);                        editVendor.PFMSVendorCode = query.PFMSVendorCode;                        editVendor.Name = query.Name;                        editVendor.Address = query.Address;                        editVendor.Email = query.Email;                        editVendor.ContactPerson = query.ContactPerson;                        editVendor.PhoneNumber = query.PhoneNumber;                        editVendor.MobileNumber = query.MobileNumber;                        editVendor.City = query.City;                        editVendor.PinCode = query.Pincode;                        editVendor.CountryId = Convert.ToInt32(query.Country);                        editVendor.StateId = Convert.ToInt32(query.StateId);                        editVendor.StateCode = Convert.ToInt32(query.StateCode);                        editVendor.RegisteredName = query.RegisteredName;                        editVendor.PAN = query.PAN;                        editVendor.TAN = query.TAN;                        editVendor.GSTExempted = Convert.ToBoolean(query.GSTExempted);                        editVendor.Reason = query.Reason;                        editVendor.GSTIN = query.GSTIN;                        editVendor.AccountHolderName = query.AccountHolderName;                        editVendor.BankName = query.BankName;                        editVendor.Branch = query.Branch;                        editVendor.IFSCCode = query.IFSC;                        editVendor.AccountNumber = query.AccountNumber;                        editVendor.BankAddress = query.BankAddress;                        editVendor.ABANumber = query.ABANumber;                        editVendor.SortCode = query.SortCode;                        editVendor.IBAN = query.IBAN;                        editVendor.SWIFTorBICCode = query.SWIFTorBICCode;                        editVendor.BankNature = query.BankNature;                        editVendor.BankEmailId = query.BankEmailId;                        editVendor.MICRCode = query.MICRCode;                        editVendor.ServiceCategory = Convert.ToInt32(query.Category);                        editVendor.ServiceType = Convert.ToInt32(query.ServiceType);                        editVendor.SupplierType = Convert.ToInt32(query.SupplyType);                        editVendor.ReverseTax = Convert.ToBoolean(query.ReverseTax);                        editVendor.TDSExcempted = Convert.ToBoolean(query.TDSExcempted);                        editVendor.ReverseTaxReason = query.ReasonForReservieTax;                        editVendor.CertificateNumber = query.CertificateNumber;                        editVendor.ValidityPeriod = Convert.ToInt32(query.ValidityPeriod);                        editVendor.BankCountry = query.BankCountry ?? 0;                        var ClrQry = context.tblClearanceAgentMaster.Where(m => m.ClearanceAgentCode == query.VendorCode && m.Status != "InActive").FirstOrDefault();                        if (ClrQry != null)                        {                            editVendor.ClearanceAgency_f = true;                            editVendor.TravelAgency_f = ClrQry.IsTravelAgency ?? false;                            ClrQry.Nationality = Convert.ToInt32(query.Nationality);                            ClrQry.Name = query.Name;                            ClrQry.Address = query.Address;                            ClrQry.Email = query.Email;                            ClrQry.ContactPerson = query.ContactPerson;                            ClrQry.PhoneNumber = query.PhoneNumber;                            ClrQry.MobileNumber = query.MobileNumber;                            ClrQry.Country = Convert.ToInt32(query.Country);                            ClrQry.StateId = Convert.ToInt32(query.StateId);                            ClrQry.StateCode = Convert.ToInt32(query.StateCode);                            ClrQry.RegisteredName = query.RegisteredName;                            ClrQry.PAN = query.PAN;                            ClrQry.TAN = query.TAN;                            ClrQry.GSTExempted = Convert.ToBoolean(query.GSTExempted);                            ClrQry.Reason = query.Reason;                            ClrQry.GSTIN = query.GSTIN;                            ClrQry.AccountHolderName = query.AccountHolderName;                            ClrQry.BankName = query.BankName;                            ClrQry.Branch = query.Branch;                            ClrQry.IFSC = query.IFSC;                            ClrQry.AccountNumber = query.AccountNumber;                            ClrQry.BankAddress = query.BankAddress;                            ClrQry.ABANumber = query.ABANumber;                            ClrQry.SortCode = query.SortCode;                            ClrQry.IBAN = query.IBAN;                            ClrQry.SWIFTorBICCode = query.SWIFTorBICCode;
                            ClrQry.ReverseTax = Convert.ToBoolean(query.ReverseTax);                            ClrQry.TDSExcempted = Convert.ToBoolean(query.TDSExcempted);
                            ClrQry.CertificateNumber = query.CertificateNumber;                            ClrQry.ValidityPeriod = Convert.ToInt32(query.ValidityPeriod);                            context.SaveChanges();                        }                        if (Gstfile.Count > 0)                        {                            int[] _docid = new int[Gstfile.Count];                            int[] _doctype = new int[Gstfile.Count];                            string[] _docname = new string[Gstfile.Count];                            string[] _attchname = new string[Gstfile.Count];                            string[] _docpath = new string[Gstfile.Count];                            for (int i = 0; i < Gstfile.Count; i++)                            {                                _docid[i] = Convert.ToInt32(Gstfile[i].GstVendorDocumentId);                                _doctype[i] = Convert.ToInt32(Gstfile[i].GstDocumentType);                                _docname[i] = Gstfile[i].GstVendorDocument;                                _attchname[i] = Gstfile[i].GstAttachmentName;                                _docpath[i] = Gstfile[i].GstAttachmentPath;                            }                            editVendor.GSTDocumentId = _docid;                            editVendor.GSTDocumentType = _doctype;                            editVendor.GSTDocumentName = _docname;                            editVendor.GSTAttachName = _attchname;                            editVendor.GSTDocPath = _docpath;                        }                        if (vendorfile.Count > 0)                        {                            int[] _docid = new int[vendorfile.Count];                            int[] _doctype = new int[vendorfile.Count];                            string[] _docname = new string[vendorfile.Count];                            string[] _attchname = new string[vendorfile.Count];                            string[] _docpath = new string[vendorfile.Count];                            for (int i = 0; i < vendorfile.Count; i++)                            {                                _docid[i] = Convert.ToInt32(vendorfile[i].RevereseTaxDocumentId);                                _doctype[i] = Convert.ToInt32(vendorfile[i].TaxDocumentType);                                _docname[i] = vendorfile[i].TaxDocument;                                _attchname[i] = vendorfile[i].TaxAttachmentName;                                _docpath[i] = vendorfile[i].TaxAttachmentPath;                            }                            editVendor.VendorDocumentId = _docid;                            editVendor.VendorDocumentType = _doctype;                            editVendor.VendorDocumentName = _docname;                            editVendor.VendorAttachName = _attchname;                            editVendor.VendorDocPath = _docpath;                        }                        if (tdsFile.Count > 0)                        {                            int[] _docid = new int[tdsFile.Count];                            int[] _doctype = new int[tdsFile.Count];                            string[] _docname = new string[tdsFile.Count];                            string[] _attchname = new string[tdsFile.Count];                            string[] _docpath = new string[tdsFile.Count];                            for (int i = 0; i < tdsFile.Count; i++)                            {                                _docid[i] = Convert.ToInt32(tdsFile[i].VendorIdentityDocumentId);                                _doctype[i] = Convert.ToInt32(tdsFile[i].IdentityVendorDocumentType);                                _docname[i] = tdsFile[i].IdentityVendorDocument;                                _attchname[i] = tdsFile[i].IdentityAttachmentName;                                _docpath[i] = tdsFile[i].IdentityVendorAttachmentPath;                            }                            editVendor.TDSDocumentId = _docid;                            editVendor.TDSDocumentType = _doctype;                            editVendor.TDSDocumentName = _docname;                            editVendor.TDSAttachName = _attchname;                            editVendor.TDSDocPath = _docpath;                        }                        if (tdsdetail.Count > 0)                        {                            int[] _tdsdetilid = new int[tdsdetail.Count];                            int[] _tdssection = new int[tdsdetail.Count];                            string[] _income = new string[tdsdetail.Count];                            decimal[] _percentage = new decimal[tdsdetail.Count];                            for (int i = 0; i < tdsdetail.Count; i++)                            {                                _tdsdetilid[i] = Convert.ToInt32(tdsdetail[i].VendorTDSDetailId);                                _tdssection[i] = Convert.ToInt32(tdsdetail[i].Section);                                _income[i] = tdsdetail[i].NatureOfIncome;                                _percentage[i] = Convert.ToDecimal(tdsdetail[i].TDSPercentage);                            }                            editVendor.VendorTDSDetailId = _tdsdetilid;                            editVendor.Section = _tdssection;                            editVendor.NatureOfIncome = _income;                            editVendor.TDSPercentage = _percentage;                        }                    }                    return editVendor;                }            }            catch (Exception ex)            {                Infrastructure.IOASException.Instance.HandleMe((object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);                VendorMasterViewModel editVendor = new VendorMasterViewModel();                return editVendor;            }        }
        public static int VendorMaster(VendorMasterViewModel model)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {

                        if (model.VendorId == null || model.VendorId == 0)
                        {

                            tblVendorMaster regvendor = new tblVendorMaster();
                            if (model.PFMSVendorCode != null)
                            {
                                var chkvendor = context.tblVendorMaster.FirstOrDefault(M => M.PFMSVendorCode == model.PFMSVendorCode && M.Status != "InActive");
                                if (chkvendor != null)
                                    return 4;

                            }
                            if (model.AccountNumber != null)
                            {
                                var chkAccNo = context.tblVendorMaster.FirstOrDefault(M => M.AccountNumber == model.AccountNumber && M.Status != "InActive");
                                if (chkAccNo != null)
                                    return 2;
                            }
                            var sqnbr = (from S in context.tblVendorMaster
                                         select S.SeqNbr
                                       ).Max();
                            regvendor.Nationality = model.Nationality;
                            regvendor.VendorCode = 'V' + (Convert.ToInt32(sqnbr) + 1).ToString("00000");
                            regvendor.SeqNbr = (Convert.ToInt32(sqnbr) + 1);
                            regvendor.PFMSVendorCode = model.PFMSVendorCode;
                            regvendor.Name = model.Name;
                            regvendor.Address = model.Address;
                            regvendor.Email = model.Email;
                            regvendor.ContactPerson = model.ContactPerson;
                            regvendor.PhoneNumber = model.PhoneNumber;
                            regvendor.MobileNumber = model.MobileNumber;
                            regvendor.City = model.City;
                            regvendor.Pincode = model.PinCode;
                            if (model.CountryId != null)
                            {
                                regvendor.Country = model.CountryId;

                            }
                            else
                            {
                                regvendor.Country = 128;
                            }
                            if (model.StateId != 0)
                            {
                                regvendor.StateId = model.StateId;
                            }
                            regvendor.RegisteredName = model.RegisteredName;
                            regvendor.PAN = model.PAN;
                            regvendor.TAN = model.TAN;
                            regvendor.GSTExempted = model.GSTExempted;
                            regvendor.Reason = model.Reason;
                            regvendor.GSTIN = model.GSTIN;
                            regvendor.AccountHolderName = model.AccountHolderName;
                            regvendor.BankName = model.BankName;
                            regvendor.Branch = model.Branch;
                            regvendor.IFSC = model.IFSCCode;
                            regvendor.AccountNumber = model.AccountNumber;
                            regvendor.BankAddress = model.BankAddress;
                            regvendor.ABANumber = model.ABANumber;
                            regvendor.SortCode = model.SortCode;
                            regvendor.IBAN = model.IBAN;
                            regvendor.SWIFTorBICCode = model.SWIFTorBICCode;
                            regvendor.Category = model.ServiceCategory;
                            regvendor.ServiceType = model.ServiceType;
                            regvendor.SupplyType = model.SupplierType;
                            regvendor.ReverseTax = model.ReverseTax;
                            regvendor.TDSExcempted = model.TDSExcempted;
                            regvendor.ReasonForReservieTax = model.ReverseTaxReason;
                            regvendor.CertificateNumber = model.CertificateNumber;
                            regvendor.ValidityPeriod = model.ValidityPeriod;
                            regvendor.BankNature = model.BankNature;
                            regvendor.BankEmailId = model.BankEmailId;
                            regvendor.MICRCode = model.MICRCode;
                            regvendor.CRTD_UserID = model.UserId;
                            regvendor.CRTD_TS = DateTime.Now;
                            regvendor.Status = "Open";
                            regvendor.StateCode = model.StateCode;
                            regvendor.BankCountry = model.BankCountry;
                            context.tblVendorMaster.Add(regvendor);
                            context.SaveChanges();
                            var vendoId = regvendor.VendorId;
                            if (model.ClearanceAgency_f == true || model.TravelAgency_f == true)
                            {

                                tblClearanceAgentMaster Clr = new tblClearanceAgentMaster();
                                Clr.Nationality = regvendor.Nationality;
                                Clr.ClearanceAgentCode = regvendor.VendorCode;
                                Clr.SeqNbr = regvendor.SeqNbr;
                                Clr.Name = regvendor.Name;
                                Clr.Address = regvendor.Address;
                                Clr.Email = regvendor.Email;
                                Clr.ContactPerson = regvendor.ContactPerson;
                                Clr.PhoneNumber = regvendor.PhoneNumber;
                                Clr.MobileNumber = regvendor.MobileNumber;
                                Clr.Country = regvendor.Country;
                                Clr.StateId = regvendor.StateId;
                                Clr.RegisteredName = regvendor.RegisteredName;
                                Clr.PAN = regvendor.PAN;
                                Clr.TAN = regvendor.TAN;
                                Clr.GSTExempted = regvendor.GSTExempted;
                                Clr.Reason = regvendor.Reason;
                                Clr.GSTIN = regvendor.GSTIN;
                                Clr.AccountHolderName = regvendor.AccountHolderName;
                                Clr.BankName = regvendor.BankName;
                                Clr.Branch = regvendor.Branch;
                                Clr.IFSC = regvendor.IFSC;
                                Clr.AccountNumber = regvendor.AccountNumber;
                                Clr.BankAddress = regvendor.BankAddress;
                                Clr.ABANumber = regvendor.ABANumber;
                                Clr.SortCode = regvendor.SortCode;
                                Clr.IBAN = regvendor.IBAN;
                                Clr.SWIFTorBICCode = regvendor.SWIFTorBICCode;
                                Clr.ReverseTax = regvendor.ReverseTax;
                                Clr.TDSExcempted = regvendor.TDSExcempted;
                                Clr.CertificateNumber = regvendor.CertificateNumber;
                                Clr.ValidityPeriod = regvendor.ValidityPeriod;
                                Clr.CRTD_UserID = regvendor.CRTD_UserID;
                                Clr.CRTD_TS = DateTime.Now;
                                Clr.Status = "Active";
                                Clr.StateCode = regvendor.StateCode;
                                Clr.IsTravelAgency = model.TravelAgency_f;
                                context.tblClearanceAgentMaster.Add(Clr);
                                context.SaveChanges();
                            }
                            if (model.GSTDocumentType != null)
                            {
                                if (model.GSTDocumentType.Length > 0)
                                {

                                    for (int i = 0; i < model.GSTDocumentType.Length; i++)
                                    {
                                        if (model.GSTAttachName[0] != "" && model.GSTFile[i] != null)
                                        {
                                            //if (query.Count == 0)
                                            //{
                                            string docgstpath = "";
                                            docgstpath = System.IO.Path.GetFileName(model.GSTFile[i].FileName);
                                            var docgstfileId = Guid.NewGuid().ToString();
                                            var docgstname = docgstfileId + "_" + docgstpath;

                                            /*Saving the file in server folder*/
                                            model.GSTFile[i].UploadFile("GstDocument", docgstname);
                                            tblGstDocument document = new tblGstDocument();
                                            document.VendorId = vendoId;
                                            if (model.GSTFile[i] != null)
                                            {
                                                document.GstVendorDocument = model.GSTFile[i].FileName;

                                            }
                                            document.GstAttachmentPath = docgstname;
                                            document.GstAttachmentName = model.GSTAttachName[i];
                                            document.GstDocumentType = model.GSTDocumentType[i];
                                            document.GstDocumentUploadUserId = model.UserId;
                                            document.GstDocumentUpload_Ts = DateTime.Now;
                                            context.tblGstDocument.Add(document);
                                            context.SaveChanges();

                                        }
                                    }
                                }
                            }
                            if (model.VendorDocumentType != null)
                            {
                                if (model.VendorDocumentType.Length > 0)
                                {
                                    for (int i = 0; i < model.VendorDocumentType.Length; i++)
                                    {
                                        if (model.VendorAttachName[i] != "" && model.VendorFile[i] != null)
                                        {
                                            //var docid = model.VendorDocumentId[i];

                                            //if (query.Count == 0)
                                            //{
                                            string doctaxpath = "";
                                            doctaxpath = System.IO.Path.GetFileName(model.VendorFile[i].FileName);
                                            var doctaxfileId = Guid.NewGuid().ToString();
                                            var doctaxname = doctaxfileId + "_" + doctaxpath;

                                            /*Saving the file in server folder*/
                                            model.VendorFile[i].UploadFile("ReverseTaxDocument", doctaxname);
                                            tblReverseTaxDocument document = new tblReverseTaxDocument();
                                            document.VendorId = vendoId;
                                            if (model.VendorFile[i] != null)
                                            {
                                                document.TaxDocument = model.VendorFile[i].FileName;

                                            }
                                            document.TaxAttachmentPath = doctaxname;
                                            document.TaxAttachmentName = model.VendorAttachName[i];
                                            document.TaxDocumentType = model.VendorDocumentType[i];
                                            document.IsCurrentVersion = false;
                                            document.TaxDocumentUploadUserId = model.UserId;
                                            document.TaxDocumentUpload_Ts = DateTime.Now;
                                            context.tblReverseTaxDocument.Add(document);
                                            context.SaveChanges();
                                            //}
                                            //else
                                            //{
                                            //    query[0].TaxDocumentType = model.GSTDocumentType[i];
                                            //    query[0].TaxAttachmentName = model.GSTAttachName[i];
                                            //    query[0].TaxDocumentUploadUserId = model.UserId;
                                            //    query[0].TaxDocumentUpload_Ts = DateTime.Now;
                                            //    query[0].IsCurrentVersion = false;
                                            //    context.SaveChanges();
                                            //}
                                        }
                                    }

                                }
                            }
                            if (model.TDSDocumentType != null)
                            {
                                if (model.TDSDocumentType.Length > 0)
                                {

                                    for (int i = 0; i < model.TDSDocumentType.Length; i++)
                                    {
                                        if (model.TDSAttachName[i] != "" && model.TDSFile[i] != null)
                                        {


                                            //if (query.Count == 0)
                                            //{
                                            string doctdspath = "";
                                            doctdspath = System.IO.Path.GetFileName(model.TDSFile[i].FileName);
                                            var doctdsfileId = Guid.NewGuid().ToString();
                                            var doctdsname = doctdsfileId + "_" + doctdspath;

                                            /*Saving the file in server folder*/
                                            model.TDSFile[i].UploadFile("TDSDocument", doctdsname);

                                            tblTDSDocument document = new tblTDSDocument();
                                            document.VendorId = vendoId;
                                            if (model.TDSFile[i] != null)
                                            {
                                                document.IdentityVendorDocument = model.TDSFile[i].FileName;

                                            }
                                            document.IdentityVendorAttachmentPath = doctdsname;
                                            document.IdentityAttachmentName = model.VendorAttachName[i];
                                            document.IdentityVendorDocumentType = model.VendorDocumentType[i];
                                            document.IsCurrentVersion = false;
                                            document.IdentityDocumentUploadUserId = model.UserId;
                                            document.IdentityDocumentUpload_Ts = DateTime.Now;
                                            context.tblTDSDocument.Add(document);
                                            context.SaveChanges();

                                        }
                                    }

                                }
                            }
                            if (model.Section != null)
                            {
                                if (model.Section[0] != 0)
                                {
                                    for (int i = 0; i < model.Section.Length; i++)
                                    {
                                        tblVendorTDSDetail tdsapplicable = new tblVendorTDSDetail();
                                        tdsapplicable.VendorId = vendoId;
                                        tdsapplicable.Section = model.Section[i];
                                        tdsapplicable.NatureOfIncome = model.NatureOfIncome[i];
                                        tdsapplicable.TDSPercentage = model.TDSPercentage[i];
                                        context.tblVendorTDSDetail.Add(tdsapplicable);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            transaction.Commit();
                            return 1;

                        }
                        else
                        {
                            var chkvendor = context.tblVendorMaster.FirstOrDefault(M => M.VendorId == model.VendorId);
                            if (chkvendor != null)
                            {
                                chkvendor.PFMSVendorCode = model.PFMSVendorCode;
                                chkvendor.Name = model.Name;
                                chkvendor.Address = model.Address;
                                chkvendor.Email = model.Email;
                                chkvendor.ContactPerson = model.ContactPerson;
                                chkvendor.PhoneNumber = model.PhoneNumber;
                                chkvendor.MobileNumber = model.MobileNumber;
                                chkvendor.StateId = model.StateId;
                                chkvendor.StateCode = model.StateCode;
                                chkvendor.City = model.City;
                                chkvendor.Pincode = model.PinCode;
                                chkvendor.RegisteredName = model.RegisteredName;
                                chkvendor.PAN = model.PAN;
                                chkvendor.TAN = model.TAN;
                                chkvendor.GSTExempted = model.GSTExempted;
                                chkvendor.Reason = model.Reason;
                                chkvendor.GSTIN = model.GSTIN;
                                chkvendor.AccountHolderName = model.AccountHolderName;
                                chkvendor.BankName = model.BankName;
                                chkvendor.Branch = model.Branch;
                                chkvendor.IFSC = model.IFSCCode;
                                chkvendor.AccountNumber = model.AccountNumber;
                                chkvendor.BankAddress = model.BankAddress;
                                chkvendor.ABANumber = model.ABANumber;
                                chkvendor.SortCode = model.SortCode;
                                chkvendor.IBAN = model.IBAN;
                                chkvendor.SWIFTorBICCode = model.SWIFTorBICCode;
                                chkvendor.BankNature = model.BankNature;
                                chkvendor.BankEmailId = model.BankEmailId;
                                chkvendor.MICRCode = model.MICRCode;
                                chkvendor.Category = model.ServiceCategory;
                                chkvendor.ServiceType = model.ServiceType;
                                chkvendor.SupplyType = model.SupplierType;
                                chkvendor.CertificateNumber = model.CertificateNumber;
                                chkvendor.ValidityPeriod = model.ValidityPeriod;
                                chkvendor.UPDT_UserID = model.UserId;
                                chkvendor.UPDT_TS = DateTime.Now;
                                chkvendor.ReverseTax = model.ReverseTax;
                                chkvendor.TDSExcempted = model.TDSExcempted;
                                chkvendor.ReasonForReservieTax = model.ReverseTaxReason;
                                chkvendor.BankCountry = model.BankCountry;
                                if (model.CountryId != null)
                                {
                                    chkvendor.Country = model.CountryId;

                                }
                                else
                                {
                                    chkvendor.Country = 128;
                                }
                                context.SaveChanges();
                                var ClrQry = context.tblClearanceAgentMaster.Where(m => m.ClearanceAgentCode == chkvendor.VendorCode && m.Status != "InActive").FirstOrDefault();

                                if (model.ClearanceAgency_f == true || model.TravelAgency_f == true)
                                {
                                    if (ClrQry == null)
                                    {
                                        tblClearanceAgentMaster Clr = new tblClearanceAgentMaster();
                                        Clr.Nationality = chkvendor.Nationality;
                                        Clr.ClearanceAgentCode = chkvendor.VendorCode;
                                        Clr.SeqNbr = chkvendor.SeqNbr;
                                        Clr.Name = chkvendor.Name;
                                        Clr.Address = chkvendor.Address;
                                        Clr.Email = chkvendor.Email;
                                        Clr.ContactPerson = chkvendor.ContactPerson;
                                        Clr.PhoneNumber = chkvendor.PhoneNumber;
                                        Clr.MobileNumber = chkvendor.MobileNumber;
                                        Clr.Country = chkvendor.Country;
                                        Clr.StateId = chkvendor.StateId;
                                        Clr.RegisteredName = chkvendor.RegisteredName;
                                        Clr.PAN = chkvendor.PAN;
                                        Clr.TAN = chkvendor.TAN;
                                        Clr.GSTExempted = chkvendor.GSTExempted;
                                        Clr.Reason = chkvendor.Reason;
                                        Clr.GSTIN = chkvendor.GSTIN;
                                        Clr.AccountHolderName = chkvendor.AccountHolderName;
                                        Clr.BankName = chkvendor.BankName;
                                        Clr.Branch = chkvendor.Branch;
                                        Clr.IFSC = chkvendor.IFSC;
                                        Clr.AccountNumber = chkvendor.AccountNumber;
                                        Clr.BankAddress = chkvendor.BankAddress;
                                        Clr.ABANumber = chkvendor.ABANumber;
                                        Clr.SortCode = chkvendor.SortCode;
                                        Clr.IBAN = chkvendor.IBAN;
                                        Clr.SWIFTorBICCode = chkvendor.SWIFTorBICCode;
                                        Clr.ReverseTax = chkvendor.ReverseTax;
                                        Clr.TDSExcempted = chkvendor.TDSExcempted;
                                        Clr.CertificateNumber = chkvendor.CertificateNumber;
                                        Clr.ValidityPeriod = chkvendor.ValidityPeriod;
                                        Clr.CRTD_UserID = chkvendor.CRTD_UserID;
                                        Clr.CRTD_TS = DateTime.Now;
                                        Clr.Status = "Active";
                                        Clr.StateCode = chkvendor.StateCode;
                                        Clr.IsTravelAgency = model.TravelAgency_f;
                                        context.tblClearanceAgentMaster.Add(Clr);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        //tblClearanceAgentMaster Clr = new tblClearanceAgentMaster();
                                        ClrQry.Nationality = chkvendor.Nationality;
                                        ClrQry.ClearanceAgentCode = chkvendor.VendorCode;
                                        ClrQry.SeqNbr = chkvendor.SeqNbr;
                                        ClrQry.Name = chkvendor.Name;
                                        ClrQry.Address = chkvendor.Address;
                                        ClrQry.Email = chkvendor.Email;
                                        ClrQry.ContactPerson = chkvendor.ContactPerson;
                                        ClrQry.PhoneNumber = chkvendor.PhoneNumber;
                                        ClrQry.MobileNumber = chkvendor.MobileNumber;
                                        ClrQry.Country = chkvendor.Country;
                                        ClrQry.StateId = chkvendor.StateId;
                                        ClrQry.RegisteredName = chkvendor.RegisteredName;
                                        ClrQry.PAN = chkvendor.PAN;
                                        ClrQry.TAN = chkvendor.TAN;
                                        ClrQry.GSTExempted = chkvendor.GSTExempted;
                                        ClrQry.Reason = chkvendor.Reason;
                                        ClrQry.GSTIN = chkvendor.GSTIN;
                                        ClrQry.AccountHolderName = chkvendor.AccountHolderName;
                                        ClrQry.BankName = chkvendor.BankName;
                                        ClrQry.Branch = chkvendor.Branch;
                                        ClrQry.IFSC = chkvendor.IFSC;
                                        ClrQry.AccountNumber = chkvendor.AccountNumber;
                                        ClrQry.BankAddress = chkvendor.BankAddress;
                                        ClrQry.ABANumber = chkvendor.ABANumber;
                                        ClrQry.SortCode = chkvendor.SortCode;
                                        ClrQry.IBAN = chkvendor.IBAN;
                                        ClrQry.SWIFTorBICCode = chkvendor.SWIFTorBICCode;
                                        ClrQry.ReverseTax = chkvendor.ReverseTax;
                                        ClrQry.TDSExcempted = chkvendor.TDSExcempted;
                                        ClrQry.CertificateNumber = chkvendor.CertificateNumber;
                                        ClrQry.ValidityPeriod = chkvendor.ValidityPeriod;
                                        ClrQry.CRTD_UserID = chkvendor.CRTD_UserID;
                                        ClrQry.CRTD_TS = DateTime.Now;
                                        ClrQry.Status = "Active";
                                        ClrQry.StateCode = chkvendor.StateCode;
                                        ClrQry.IsTravelAgency = model.TravelAgency_f;
                                        context.SaveChanges();
                                    }
                                }

                                if(model.GSTDocumentType != null)
                                {
                                    if (model.GSTDocumentType.Length > 0)
                                    {
                                        var deldocument = (from RD in context.tblGstDocument
                                                           where RD.VendorId == model.VendorId && !model.GSTDocumentId.Contains(RD.GstVendorDocumentId)
                                                           && RD.IsCurrentVersion != true
                                                           select RD).ToList();
                                        int delCount = deldocument.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                deldocument[i].IsCurrentVersion = true;
                                                deldocument[i].GstDocumentUploadUserId = model.UserId;
                                                deldocument[i].GstDocumentUpload_Ts = DateTime.Now;
                                                context.SaveChanges();
                                            }
                                        }

                                        for (int i = 0; i < model.GSTDocumentType.Length; i++)
                                        {
                                            if (model.GSTDocumentType[i] != 0)
                                            {
                                                var docid = model.GSTDocumentId[i];
                                                var query = (from G in context.tblGstDocument
                                                             where (G.GstVendorDocumentId == docid && G.VendorId == model.VendorId && G.IsCurrentVersion != true)
                                                             select G).ToList();
                                                if (query.Count == 0)
                                                {
                                                    if (model.GSTFile[i] != null)
                                                    {
                                                        string docgstpath = "";
                                                        docgstpath = System.IO.Path.GetFileName(model.GSTFile[i].FileName);
                                                        var docgstfileId = Guid.NewGuid().ToString();
                                                        var docgstname = docgstfileId + "_" + docgstpath;

                                                        /*Saving the file in server folder*/
                                                        model.GSTFile[i].UploadFile("GstDocument", docgstname);
                                                        tblGstDocument document = new tblGstDocument();
                                                        document.VendorId = model.VendorId;
                                                        if (model.GSTFile[i] != null)
                                                        {
                                                            document.GstVendorDocument = model.GSTFile[i].FileName;

                                                        }
                                                        document.GstAttachmentPath = docgstname;
                                                        document.GstAttachmentName = model.GSTAttachName[i];
                                                        document.GstDocumentType = model.GSTDocumentType[i];
                                                        document.GstDocumentUploadUserId = model.UserId;
                                                        document.GstDocumentUpload_Ts = DateTime.Now;
                                                        context.tblGstDocument.Add(document);
                                                        context.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    if (model.GSTFile[i] != null)
                                                    {
                                                        string docgstpath = "";
                                                        docgstpath = System.IO.Path.GetFileName(model.GSTFile[i].FileName);
                                                        var docgstfileId = Guid.NewGuid().ToString();
                                                        var docgstname = docgstfileId + "_" + docgstpath;

                                                        /*Saving the file in server folder*/
                                                        model.GSTFile[i].UploadFile("GstDocument", docgstname);
                                                        query[0].GstVendorDocument = model.GSTFile[i].FileName;
                                                        query[0].GstAttachmentPath = docgstname;
                                                    }
                                                    query[0].GstDocumentType = model.GSTDocumentType[i];
                                                    query[0].GstAttachmentName = model.GSTAttachName[i];
                                                    query[0].GstDocumentUploadUserId = model.UserId;
                                                    query[0].GstDocumentUpload_Ts = DateTime.Now;
                                                    query[0].IsCurrentVersion = false;
                                                    context.SaveChanges();
                                                }
                                            }
                                        }



                                    }

                                }

                                if(model.VendorDocumentType != null)
                                {
                                    if (model.VendorDocumentType.Length > 0)
                                    {
                                        var deldocument = (from RD in context.tblReverseTaxDocument
                                                           where RD.VendorId == model.VendorId && !model.VendorDocumentId.Contains(RD.RevereseTaxDocumentId)
                                                           && RD.IsCurrentVersion != true
                                                           select RD).ToList();
                                        int delCount = deldocument.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                deldocument[i].IsCurrentVersion = true;
                                                deldocument[i].TaxDocumentUploadUserId = model.UserId;
                                                deldocument[i].TaxDocumentUpload_Ts = DateTime.Now;
                                                context.SaveChanges();
                                            }
                                        }
                                        for (int i = 0; i < model.VendorDocumentType.Length; i++)
                                        {
                                            if (model.VendorDocumentType[i] != 0)
                                            {
                                                var docid = model.VendorDocumentId[i];
                                                var query = (from T in context.tblReverseTaxDocument
                                                             where (T.RevereseTaxDocumentId == docid && T.VendorId == model.VendorId && T.IsCurrentVersion != true)
                                                             select T).ToList();
                                                if (query.Count == 0)
                                                {
                                                    if (model.VendorFile[i] != null)
                                                    {
                                                        string doctaxpath = "";
                                                        doctaxpath = System.IO.Path.GetFileName(model.VendorFile[i].FileName);
                                                        var doctaxfileId = Guid.NewGuid().ToString();
                                                        var doctaxname = doctaxfileId + "_" + doctaxpath;

                                                        /*Saving the file in server folder*/
                                                        model.VendorFile[i].UploadFile("ReverseTaxDocument", doctaxname);
                                                        tblReverseTaxDocument document = new tblReverseTaxDocument();
                                                        document.VendorId = model.VendorId;
                                                        if (model.VendorFile[i] != null)
                                                        {
                                                            document.TaxDocument = model.VendorFile[i].FileName;

                                                        }
                                                        document.TaxAttachmentPath = doctaxname;
                                                        document.TaxAttachmentName = model.VendorAttachName[i];
                                                        document.TaxDocumentType = model.VendorDocumentType[i];
                                                        document.IsCurrentVersion = false;
                                                        document.TaxDocumentUploadUserId = model.UserId;
                                                        document.TaxDocumentUpload_Ts = DateTime.Now;
                                                        context.tblReverseTaxDocument.Add(document);
                                                        context.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    if (model.VendorFile[i] != null)
                                                    {
                                                        string doctaxpath = "";
                                                        doctaxpath = System.IO.Path.GetFileName(model.VendorFile[i].FileName);
                                                        var doctaxfileId = Guid.NewGuid().ToString();
                                                        var doctaxname = doctaxfileId + "_" + doctaxpath;

                                                        /*Saving the file in server folder*/
                                                        model.VendorFile[i].UploadFile("ReverseTaxDocument", doctaxname);
                                                        query[0].TaxDocument = model.VendorFile[i].FileName;
                                                        query[0].TaxAttachmentPath = doctaxname;
                                                    }
                                                    query[0].TaxDocumentType = model.VendorDocumentType[i];
                                                    query[0].TaxAttachmentName = model.VendorAttachName[i];
                                                    query[0].TaxDocumentUploadUserId = model.UserId;
                                                    query[0].TaxDocumentUpload_Ts = DateTime.Now;
                                                    query[0].IsCurrentVersion = false;
                                                    context.SaveChanges();
                                                }
                                            }
                                        }

                                    }
                                }
                               


                                if(model.TDSDocumentType != null)
                                {
                                    if (model.TDSDocumentType.Length > 0)
                                    {
                                        var deldocument = (from RD in context.tblTDSDocument
                                                           where RD.VendorId == model.VendorId && !model.TDSDocumentId.Contains(RD.VendorIdentityDocumentId)
                                                           && RD.IsCurrentVersion != true
                                                           select RD).ToList();
                                        int delCount = deldocument.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                deldocument[i].IsCurrentVersion = true;
                                                deldocument[i].IdentityDocumentUploadUserId = model.UserId;
                                                deldocument[i].IdentityDocumentUpload_Ts = DateTime.Now;
                                                context.SaveChanges();
                                            }
                                        }
                                        for (int i = 0; i < model.TDSDocumentType.Length; i++)
                                        {
                                            if (model.TDSDocumentType[i] != 0)
                                            {
                                                var docid = model.TDSDocumentId[i];
                                                var query = (from T in context.tblTDSDocument
                                                             where (T.VendorIdentityDocumentId == docid && T.VendorId == model.VendorId && T.IsCurrentVersion != true)
                                                             select T).ToList();
                                                if (query.Count == 0)
                                                {
                                                    if (model.TDSFile[i] != null)
                                                    {
                                                        string doctdspath = "";
                                                        doctdspath = System.IO.Path.GetFileName(model.TDSFile[i].FileName);
                                                        var doctdsfileId = Guid.NewGuid().ToString();
                                                        var doctdsname = doctdsfileId + "_" + doctdspath;

                                                        /*Saving the file in server folder*/
                                                        model.TDSFile[i].UploadFile("TDSDocument", doctdsname);
                                                        tblTDSDocument document = new tblTDSDocument();
                                                        document.VendorId = model.VendorId;

                                                        document.IdentityVendorDocument = model.TDSFile[i].FileName;


                                                        document.IdentityVendorAttachmentPath = doctdsname;
                                                        document.IdentityAttachmentName = model.TDSAttachName[i];
                                                        document.IdentityVendorDocumentType = model.TDSDocumentType[i];
                                                        document.IsCurrentVersion = false;
                                                        document.IdentityDocumentUploadUserId = model.UserId;
                                                        document.IdentityDocumentUpload_Ts = DateTime.Now;
                                                        context.tblTDSDocument.Add(document);
                                                        context.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    if (model.TDSFile[i] != null)
                                                    {
                                                        string doctdspath = "";
                                                        doctdspath = System.IO.Path.GetFileName(model.TDSFile[i].FileName);
                                                        var doctdsfileId = Guid.NewGuid().ToString();
                                                        var doctdsname = doctdsfileId + "_" + doctdspath;

                                                        /*Saving the file in server folder*/
                                                        model.TDSFile[i].UploadFile("TDSDocument", doctdsname);
                                                        query[0].IdentityVendorDocument = model.TDSFile[i].FileName;
                                                        query[0].IdentityVendorAttachmentPath = doctdsname;
                                                    }
                                                    query[0].IdentityVendorDocumentType = model.TDSDocumentType[i];
                                                    query[0].IdentityAttachmentName = model.TDSAttachName[i];
                                                    query[0].IdentityDocumentUploadUserId = model.UserId;
                                                    query[0].IdentityDocumentUpload_Ts = DateTime.Now;
                                                    query[0].IsCurrentVersion = false;
                                                    context.SaveChanges();
                                                }
                                            }
                                        }

                                    }
                                }
                                

                                if(model.Section != null)
                                {
                                    if (model.Section[0] != 0)
                                    {
                                        var delvendortds = (from VTD in context.tblVendorTDSDetail
                                                            where VTD.VendorId == model.VendorId && !model.VendorTDSDetailId.Contains(VTD.VendorTDSDetailId)
                                                            select VTD).ToList();
                                        int delCount = delvendortds.Count();
                                        if (delCount > 0)
                                        {
                                            context.tblVendorTDSDetail.RemoveRange(delvendortds);
                                            context.SaveChanges();
                                        }
                                        for (int i = 0; i < model.Section.Length; i++)
                                        {

                                            var tdsid = model.VendorTDSDetailId[i];
                                            var query = (from T in context.tblVendorTDSDetail
                                                         where (T.VendorTDSDetailId == tdsid && T.VendorId == model.VendorId)
                                                         select T).ToList();
                                            if (query.Count == 0)
                                            {

                                                tblVendorTDSDetail tdsapplicable = new tblVendorTDSDetail();
                                                tdsapplicable.VendorId = model.VendorId;
                                                tdsapplicable.Section = model.Section[i];
                                                tdsapplicable.NatureOfIncome = model.NatureOfIncome[i];
                                                tdsapplicable.TDSPercentage = model.TDSPercentage[i];
                                                context.tblVendorTDSDetail.Add(tdsapplicable);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].VendorId = model.VendorId;
                                                query[0].Section = model.Section[i];
                                                query[0].NatureOfIncome = model.NatureOfIncome[i];
                                                query[0].TDSPercentage = model.TDSPercentage[i];
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                               
                            }
                            transaction.Commit();
                            return 3;
                        }


                    }
                    catch (Exception ex)
                    {
                        Infrastructure.IOASException.Instance.HandleMe(
(object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                        transaction.Rollback();
                        return -1;
                    }


                }
            }

        }
        public static VendorMasterViewModel RevisedEditvendor(int vendorId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    VendorMasterViewModel editVendor = new VendorMasterViewModel();
                    var query = (from V in context.tblVendorMaster
                                 where (V.VendorId == vendorId && V.Status == "Active")
                                 select V).FirstOrDefault();
                    var Gstfile = (from G in context.tblGstDocument
                                   where G.VendorId == vendorId && G.IsCurrentVersion != true
                                   select G).ToList();
                    var vendorfile = (from vf in context.tblReverseTaxDocument
                                      where vf.VendorId == vendorId && vf.IsCurrentVersion != true
                                      select vf).ToList();
                    var tdsFile = (from TF in context.tblTDSDocument
                                   where TF.VendorId == vendorId && TF.IsCurrentVersion != true
                                   select TF).ToList();
                    var tdsdetail = (from TD in context.tblVendorTDSDetail
                                     where TD.VendorId == vendorId
                                     select TD).ToList();

                    if (query != null)
                    {
                        editVendor.VendorId = query.VendorId;
                        editVendor.Nationality = Convert.ToInt32(query.Nationality);
                        editVendor.PFMSVendorCode = query.PFMSVendorCode;
                        editVendor.Name = query.Name;
                        editVendor.Address = query.Address;
                        editVendor.Email = query.Email;
                        editVendor.ContactPerson = query.ContactPerson;
                        editVendor.PhoneNumber = query.PhoneNumber;
                        editVendor.MobileNumber = query.MobileNumber;
                        editVendor.City = query.City;
                        editVendor.PinCode = query.Pincode;
                        editVendor.CountryId = Convert.ToInt32(query.Country);
                        editVendor.StateId = Convert.ToInt32(query.StateId);
                        editVendor.StateCode = Convert.ToInt32(query.StateCode);
                        editVendor.RegisteredName = query.RegisteredName;
                        editVendor.PAN = query.PAN;
                        editVendor.TAN = query.TAN;
                        editVendor.GSTExempted = Convert.ToBoolean(query.GSTExempted);
                        editVendor.Reason = query.Reason;
                        editVendor.GSTIN = query.GSTIN;
                        editVendor.AccountHolderName = query.AccountHolderName;
                        editVendor.BankName = query.BankName;
                        editVendor.Branch = query.Branch;
                        editVendor.IFSCCode = query.IFSC;
                        editVendor.AccountNumber = query.AccountNumber;
                        editVendor.BankAddress = query.BankAddress;
                        editVendor.ABANumber = query.ABANumber;
                        editVendor.SortCode = query.SortCode;
                        editVendor.IBAN = query.IBAN;
                        editVendor.SWIFTorBICCode = query.SWIFTorBICCode;
                        editVendor.BankNature = query.BankNature;
                        editVendor.BankEmailId = query.BankEmailId;
                        editVendor.MICRCode = query.MICRCode;
                        editVendor.ServiceCategory = Convert.ToInt32(query.Category);
                        editVendor.ServiceType = Convert.ToInt32(query.ServiceType);
                        editVendor.SupplierType = Convert.ToInt32(query.SupplyType);
                        editVendor.ReverseTax = Convert.ToBoolean(query.ReverseTax);
                        editVendor.TDSExcempted = Convert.ToBoolean(query.TDSExcempted);
                        editVendor.ReverseTaxReason = query.ReasonForReservieTax;
                        editVendor.CertificateNumber = query.CertificateNumber;
                        editVendor.ValidityPeriod = Convert.ToInt32(query.ValidityPeriod);
                        editVendor.BankCountry = query.BankCountry ?? 0;
                        var ClrQry = context.tblClearanceAgentMaster.Where(m => m.ClearanceAgentCode == query.VendorCode && m.Status != "InActive").FirstOrDefault();
                        if (ClrQry != null)
                        {
                            editVendor.ClearanceAgency_f = true;
                            editVendor.TravelAgency_f = ClrQry.IsTravelAgency ?? false;
                        }
                        if (Gstfile.Count > 0)
                        {
                            int[] _docid = new int[Gstfile.Count];
                            int[] _doctype = new int[Gstfile.Count];
                            string[] _docname = new string[Gstfile.Count];
                            string[] _attchname = new string[Gstfile.Count];
                            string[] _docpath = new string[Gstfile.Count];
                            for (int i = 0; i < Gstfile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(Gstfile[i].GstVendorDocumentId);
                                _doctype[i] = Convert.ToInt32(Gstfile[i].GstDocumentType);
                                _docname[i] = Gstfile[i].GstVendorDocument;
                                _attchname[i] = Gstfile[i].GstAttachmentName;
                                _docpath[i] = Gstfile[i].GstAttachmentPath;
                            }
                            editVendor.GSTDocumentId = _docid;
                            editVendor.GSTDocumentType = _doctype;
                            editVendor.GSTDocumentName = _docname;
                            editVendor.GSTAttachName = _attchname;
                            editVendor.GSTDocPath = _docpath;
                        }
                        if (vendorfile.Count > 0)
                        {
                            int[] _docid = new int[vendorfile.Count];
                            int[] _doctype = new int[vendorfile.Count];
                            string[] _docname = new string[vendorfile.Count];
                            string[] _attchname = new string[vendorfile.Count];
                            string[] _docpath = new string[vendorfile.Count];
                            for (int i = 0; i < vendorfile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(vendorfile[i].RevereseTaxDocumentId);
                                _doctype[i] = Convert.ToInt32(vendorfile[i].TaxDocumentType);
                                _docname[i] = vendorfile[i].TaxDocument;
                                _attchname[i] = vendorfile[i].TaxAttachmentName;
                                _docpath[i] = vendorfile[i].TaxAttachmentPath;
                            }
                            editVendor.VendorDocumentId = _docid;
                            editVendor.VendorDocumentType = _doctype;
                            editVendor.VendorDocumentName = _docname;
                            editVendor.VendorAttachName = _attchname;
                            editVendor.VendorDocPath = _docpath;
                        }
                        if (tdsFile.Count > 0)
                        {
                            int[] _docid = new int[tdsFile.Count];
                            int[] _doctype = new int[tdsFile.Count];
                            string[] _docname = new string[tdsFile.Count];
                            string[] _attchname = new string[tdsFile.Count];
                            string[] _docpath = new string[tdsFile.Count];
                            for (int i = 0; i < tdsFile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(tdsFile[i].VendorIdentityDocumentId);
                                _doctype[i] = Convert.ToInt32(tdsFile[i].IdentityVendorDocumentType);
                                _docname[i] = tdsFile[i].IdentityVendorDocument;
                                _attchname[i] = tdsFile[i].IdentityAttachmentName;
                                _docpath[i] = tdsFile[i].IdentityVendorAttachmentPath;
                            }
                            editVendor.TDSDocumentId = _docid;
                            editVendor.TDSDocumentType = _doctype;
                            editVendor.TDSDocumentName = _docname;
                            editVendor.TDSAttachName = _attchname;
                            editVendor.TDSDocPath = _docpath;
                        }
                        if (tdsdetail.Count > 0)
                        {
                            int[] _tdsdetilid = new int[tdsdetail.Count];
                            int[] _tdssection = new int[tdsdetail.Count];
                            string[] _income = new string[tdsdetail.Count];
                            decimal[] _percentage = new decimal[tdsdetail.Count];
                            for (int i = 0; i < tdsdetail.Count; i++)
                            {
                                _tdsdetilid[i] = Convert.ToInt32(tdsdetail[i].VendorTDSDetailId);
                                _tdssection[i] = Convert.ToInt32(tdsdetail[i].Section);
                                _income[i] = tdsdetail[i].NatureOfIncome;
                                _percentage[i] = Convert.ToDecimal(tdsdetail[i].TDSPercentage);
                            }
                            editVendor.VendorTDSDetailId = _tdsdetilid;
                            editVendor.Section = _tdssection;
                            editVendor.NatureOfIncome = _income;
                            editVendor.TDSPercentage = _percentage;
                        }
                    }
                    return editVendor;
                }
            }
            catch (Exception ex)
            {
                VendorMasterViewModel editVendor = new VendorMasterViewModel();
                return editVendor;
            }
        }

        public static VendorSearchModel GetVendorList(VendorSearchModel model, int page, int pageSize)
        {
            try
            {
                VendorSearchModel list = new VendorSearchModel();
                List<VendorMasterViewModel> getVendor = new List<VendorMasterViewModel>();
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
                    var query = (from V in context.tblVendorMaster
                                 join C in context.tblCountries on V.Country equals C.countryID into CO
                                 from cl in CO.DefaultIfEmpty()
                                 orderby V.VendorId descending
                                 where V.Status != "InActive"
                                 && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                 && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                 && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                 && (V.Status.Contains(model.INStatus) || model.INStatus == null)
                                 && (V.BankName.Contains(model.INBankName) || model.INBankName == null)
                                 && (V.AccountNumber.Contains(model.INAccountNumber) || model.INAccountNumber == null)
                                 select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName, V.Status, V.BankName, V.AccountNumber }).Skip(skiprec).Take(pageSize).ToList();
                    list.TotalRecords = (from V in context.tblVendorMaster
                                         join C in context.tblCountries on V.Country equals C.countryID into CO
                                         from cl in CO.DefaultIfEmpty()
                                         orderby V.VendorId descending
                                         where V.Status != "InActive"
                                        && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                  && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                  && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                   && (V.Status.Contains(model.INStatus) || model.INStatus == null)
                                    && (V.BankName.Contains(model.INBankName) || model.INBankName == null)
                                 && (V.AccountNumber.Contains(model.INAccountNumber) || model.INAccountNumber == null)
                                         select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName }).Count();
                    if (query.Count > 0)
                    {

                        for (int i = 0; i < query.Count; i++)
                        {
                            var countrylist = "";
                            if (query[i].Country == 128)
                            {
                                countrylist = "INDIA";
                            }
                            else
                            {
                                countrylist = query[i].countryName;
                            }
                            int sno = i + 1;
                            getVendor.Add(new VendorMasterViewModel()
                            {
                                sno = sno,
                                VendorId = query[i].VendorId,
                                Name = query[i].Name,
                                VendorCode = query[i].VendorCode,
                                CountryName = countrylist,
                                Status = query[i].Status,
                                BankName = query[i].BankName,
                                AccountNumber = query[i].AccountNumber
                            });
                        }
                    }
                    list.VendorList = getVendor;
                    return list;
                }
            }
            catch (Exception ex)
            {
                VendorSearchModel list = new VendorSearchModel();
                return list;
            }
        }
        public static VendorSearchModel GetRevisedVendorList(VendorSearchModel model, int page, int pageSize)
        {
            try
            {
                VendorSearchModel list = new VendorSearchModel();
                List<VendorMasterViewModel> getVendor = new List<VendorMasterViewModel>();
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
                    var query = (from V in context.tblVendorMaster
                                 join C in context.tblCountries on V.Country equals C.countryID into CO
                                 from cl in CO.DefaultIfEmpty()
                                 orderby V.VendorId descending
                                 where V.Status == "Active"
                                 && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                 && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                 && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                  && (V.Status.Contains(model.INStatus) || model.INStatus == null)
                                 select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName, V.Status }).Skip(skiprec).Take(pageSize).ToList();
                    list.TotalRecords = (from V in context.tblVendorMaster
                                         join C in context.tblCountries on V.Country equals C.countryID into CO
                                         from cl in CO.DefaultIfEmpty()
                                         orderby V.VendorId descending
                                         where V.Status == "Active"
                                        && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                  && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                  && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                   && (V.Status.Contains(model.INStatus) || model.INStatus == null)
                                         select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName }).Count();
                    if (query.Count > 0)
                    {

                        for (int i = 0; i < query.Count; i++)
                        {
                            var countrylist = "";
                            if (query[i].Country == 128)
                            {
                                countrylist = "INDIA";
                            }
                            else
                            {
                                countrylist = query[i].countryName;
                            }
                            int sno = i + 1;
                            getVendor.Add(new VendorMasterViewModel()
                            {
                                sno = sno,
                                VendorId = query[i].VendorId,
                                Name = query[i].Name,
                                VendorCode = query[i].VendorCode,
                                CountryName = countrylist,
                                Status = query[i].Status
                            });
                        }
                    }
                    list.VendorList = getVendor;
                    return list;
                }
            }
            catch (Exception ex)
            {
                VendorSearchModel list = new VendorSearchModel();
                return list;
            }
        }
        public static SearchBankMaster GetBankList(SearchBankMaster model, int page, int pageSize)
        {
            try
            {

                SearchBankMaster searchbank = new SearchBankMaster();
                List<BankMasterListModel> list = new List<BankMasterListModel>();
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
                    list = (from b in context.tblStaffBankAccount

                            where
                            (b.Category.Contains(model.CategorySearch) || model.CategorySearch == null)
                            && (b.Name.Contains(model.Name) || model.Name == null)
                            && (b.AccountNumber.Contains(model.AccountNumber) || model.AccountNumber == null)
                            && (b.BankName.Contains(model.BankName) || model.BankName == null)
                            orderby b.StaffBankId descending
                            select new
                            {
                                b.StaffBankId,
                                b.Category,
                                b.Name,
                                b.AccountNumber,
                                b.BankName
                            }).AsEnumerable()
                                     .Select((x, index) => new BankMasterListModel()
                                     {
                                         SNo = index + 1,
                                         StaffBankId = x.StaffBankId,
                                         Category = x.Category,
                                         Name = x.Name,
                                         AccountNumber = x.AccountNumber,
                                         BankName = x.BankName
                                     }).Skip(skiprec).Take(pageSize).ToList();
                    searchbank.TotalRecords = (from b in context.tblStaffBankAccount
                                               where
                                                (b.Category.Contains(model.CategorySearch) || model.CategorySearch == null)
                                               && (b.Name.Contains(model.Name) || model.Name == null)
                                               && (b.AccountNumber.Contains(model.AccountNumber) || model.AccountNumber == null)
                                                && (b.BankName.Contains(model.BankName) || model.BankName == null)
                                               orderby b.StaffBankId descending
                                               select new
                                               {
                                                   b.StaffBankId,
                                                   b.Category,
                                                   b.Name,
                                                   b.AccountNumber,
                                                   b.BankName
                                               }).Count();
                    searchbank.listbank = list;

                }
                return searchbank;
            }
            catch (Exception ex)
            {
                SearchBankMaster searchbank = new SearchBankMaster();
                return searchbank;
            }
        }


        public static bool VendorWFInit(int vendorId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblVendorMaster.FirstOrDefault(m => m.VendorId == vendorId && m.Status == "Open");
                    if (query != null)
                    {
                        var fw = FlowEngine.Init(160, logged_in_user, vendorId, "VendorId");
                        string url = string.Empty;
                        int functionId = 41;
                        url = "/Master/VendorManagementView?vendorId=" + vendorId;
                        fw.FunctionId(functionId);
                        fw.ReferenceNumber(query.VendorCode);
                        fw.ActionLink(url);
                        fw.FailedMethod("VendorWFInitFailure");
                        fw.ClarifyMethod("VendorWFInitClarify");
                        fw.SuccessMethod("VendorWFInitSuccess");
                        fw.ProcessInit();
                        if (String.IsNullOrEmpty(fw.errorMsg))
                        {
                            query.Status = "Submit for approval";
                            query.UPDT_UserID = logged_in_user;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static VendorManagementModel GetVendorManagementDetails(int VendorId)
        {
            VendorManagementModel model = new VendorManagementModel();
            List<GSTDocumentModel> gstlist = new List<GSTDocumentModel>();
            List<TDSDocumentModel> tdslist = new List<TDSDocumentModel>();
            List<TaxDocumentModel> taxlist = new List<TaxDocumentModel>();
            List<VendorTDSDetails> List = new List<VendorTDSDetails>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblVendorMaster.Where(m => m.VendorId == VendorId).FirstOrDefault();
                    model.VendorName = Qry.Name;
                    model.VedorId = Qry.VendorId;
                    model.Category = Qry.Nationality == 1 ? "India" : "Foreign";
                    model.PFMSCode = Qry.PFMSVendorCode;
                    model.Address = Qry.Address;
                    model.Email = Qry.Email;
                    model.ContactPerson = Qry.ContactPerson;
                    model.MobileNo = Qry.MobileNumber;
                    model.PhoneNo = Qry.PhoneNumber;
                    model.Country = context.tblCountries.Where(m => m.countryID == Qry.Country).Select(m => m.countryName).FirstOrDefault();
                    int? StateId = Qry.StateId;
                    model.State = context.tblStateMaster.Where(m => m.StateId == StateId).Select(m => m.StateName).FirstOrDefault();
                    model.StateCode = context.tblStateMaster.Where(m => m.StateId == StateId).Select(m => m.StateCode).FirstOrDefault();
                    model.City = Qry.City;
                    model.PinCode = Convert.ToString(Qry.Pincode);
                    model.RegName = Qry.RegisteredName;
                    model.TAN = Qry.TAN;
                    model.PAN = Qry.PAN;
                    model.GSTIN = Qry.GSTIN;
                    model.TaxExpReason = Qry.Reason;
                    model.GSTExp = Qry.GSTExempted ?? false;
                    model.TDSExp = Qry.TDSExcempted ?? false;
                    model.RevTax = Qry.ReverseTax ?? false;
                    model.AccHolderName = Qry.AccountHolderName;
                    model.AccNo = Qry.AccountNumber;
                    model.IFSC = Qry.IFSC;
                    model.BankName = Qry.BankName;
                    model.BankAddress = Qry.BankAddress;
                    model.Branch = Qry.Branch;
                    model.BankEmail = Qry.BankEmailId;
                    model.BankNature = Qry.BankNature;
                    model.MICRCode = Qry.MICRCode;
                    model.ABANumber = Qry.ABANumber;
                    model.SortCode = Qry.SortCode;
                    model.IBAN = Qry.IBAN;
                    model.MICRCode = Qry.MICRCode;
                    model.SWiftCode = Qry.SWIFTorBICCode;
                    model.BankCountry = Common.GetVendorbankCountry(Qry.BankCountry ?? 0);
                    model.ServiceCategory = context.tblCodeControl.Where(m => m.CodeName == "DeductionCategory" && m.CodeValAbbr == Qry.Category).Select(m => m.CodeValDetail).FirstOrDefault();
                    int? TaxId = Qry.SupplyType == null ? Qry.ServiceType : Qry.SupplyType;
                    model.Type = context.tblTaxMaster.Where(m => m.TaxMasterId == TaxId).Select(m => m.ServiceType).FirstOrDefault();
                    model.ReverseTaxReson = Qry.ReasonForReservieTax;
                    model.CertificateNo = Qry.CertificateNumber;
                    model.ValidityReson = Convert.ToString(Qry.ValidityPeriod);
                    var ClrQry = context.tblClearanceAgentMaster.Where(m => m.ClearanceAgentCode == Qry.VendorCode && m.Status != "InActive").FirstOrDefault();
                    if (ClrQry != null)
                    {
                        model.ClearanceAgency_f = true;
                        model.TravelAgency_f = ClrQry.IsTravelAgency ?? false;
                    }
                    var GstQry = (from gd in context.tblGstDocument
                                  join DN in context.tblDocument on gd.GstDocumentType equals DN.DocumentId into g
                                  from d in g.DefaultIfEmpty()
                                  where gd.VendorId == VendorId && gd.IsCurrentVersion != true
                                  select new { gd.GstVendorDocumentId, gd.GstVendorDocument, gd.GstAttachmentPath, gd.GstAttachmentName, d.DocumentName }
                                     ).ToList();

                    if (GstQry != null)
                    {
                        for (int i = 0; i < GstQry.Count; i++)
                        {
                            gstlist.Add(new GSTDocumentModel()
                            {
                                GstDocumentid = GstQry[i].GstVendorDocumentId,
                                GstDocumentName = GstQry[i].GstVendorDocument,
                                GstAttachementName = GstQry[i].GstAttachmentName,
                                GstDocumenttypename = GstQry[i].DocumentName,
                                GstDocumentPath = GstQry[i].GstAttachmentPath
                            });
                        }
                    }

                    var TdsDocQry = (from td in context.tblTDSDocument
                                     join DN in context.tblDocument on td.IdentityVendorDocumentType equals DN.DocumentId into g
                                     from d in g.DefaultIfEmpty()
                                     where td.VendorId == VendorId && td.IsCurrentVersion != true
                                     select new { td.VendorIdentityDocumentId, td.IdentityVendorDocument, td.IdentityVendorAttachmentPath, td.IdentityAttachmentName, d.DocumentName }
                                                         ).ToList();
                    if (TdsDocQry != null)
                    {
                        for (int i = 0; i < TdsDocQry.Count; i++)
                        {
                            tdslist.Add(new TDSDocumentModel()
                            {
                                TdsDocumentid = TdsDocQry[i].VendorIdentityDocumentId,
                                TdsDocumentName = TdsDocQry[i].IdentityVendorDocument,
                                TdsAttachementName = TdsDocQry[i].IdentityAttachmentName,
                                TdsDocumenttypename = TdsDocQry[i].DocumentName,
                                TdsDocumentPath = TdsDocQry[i].IdentityVendorAttachmentPath
                            });
                        }
                    }
                    var TaxDocQry = (from tax in context.tblReverseTaxDocument
                                     join DN in context.tblDocument on tax.TaxDocumentType equals DN.DocumentId into g
                                     from d in g.DefaultIfEmpty()
                                     where tax.VendorId == VendorId && tax.IsCurrentVersion != true
                                     select new { tax.RevereseTaxDocumentId, tax.TaxDocument, tax.TaxAttachmentPath, tax.TaxAttachmentName, d.DocumentName }).ToList();
                    if (TaxDocQry != null)
                    {
                        for (int i = 0; i < TaxDocQry.Count; i++)
                        {
                            taxlist.Add(new TaxDocumentModel()
                            {
                                TaxDocumentid = TaxDocQry[i].RevereseTaxDocumentId,
                                TaxDocumentName = TaxDocQry[i].TaxDocument,
                                TaxAttachementName = TaxDocQry[i].TaxAttachmentName,
                                TaxDocumenttypename = TaxDocQry[i].DocumentName,
                                TaxDocumentPath = TaxDocQry[i].TaxAttachmentPath
                            });
                        }
                    }
                    var TDSQry = context.tblVendorTDSDetail.Where(m => m.VendorId == VendorId).ToList();
                    if (TDSQry != null)
                    {
                        for (int i = 0; i < TDSQry.Count; i++)
                        {
                            int? Sec = TDSQry[i].Section;
                            List.Add(new VendorTDSDetails
                            {
                                Section = context.tblTDSMaster.Where(m => m.TdsMasterId == Sec).Select(m => m.Section).FirstOrDefault(),
                                NatureOfIncome = TDSQry[i].NatureOfIncome,
                                TDSPercentage = String.Format("{0:0.##}", TDSQry[i].TDSPercentage) + "%"
                            });
                        }
                    }
                    model.List = List;
                    model.GstDocument = gstlist;
                    model.TdsDocument = tdslist;
                    model.TaxDocument = taxlist;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static int InternalAgency(InternalAgencyViewModel model)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        int agencyid = 0;
                        if (model.InternalAgencyId == null)
                        {

                            tblAgencyMaster reginternalagncy = new tblAgencyMaster();
                            var chkagency = context.tblAgencyMaster.FirstOrDefault(M => M.AgencyName == model.InternalAgencyName && M.Status == "Active" && M.AgencyType == 1);
                            if (chkagency != null)
                                return 2;
                            var chkcode = context.tblAgencyMaster.FirstOrDefault(C => C.AgencyCode == model.InternalAgencyCode && C.Status == "Active" && C.AgencyType == 1);
                            if (chkcode != null)
                                return 4;
                            var Sqnbr = (from IA in context.tblAgencyMaster
                                         where IA.AgencyType == 1
                                         select IA.SeqNbr).Max();
                            reginternalagncy.AgencyName = model.InternalAgencyName;
                            reginternalagncy.AgencyCode = model.InternalAgencyCode;
                            reginternalagncy.ContactPerson = model.InternalAgencyContactPerson;
                            reginternalagncy.ContactNumber = model.InternalAgencyContactNumber;
                            reginternalagncy.ContactEmail = model.InternalConatactEmail;
                            reginternalagncy.Address = model.InternalAgencyAddress;
                            reginternalagncy.AgencyRegisterName = model.InternalAgencyRegisterName;
                            reginternalagncy.AgencyRegisterAddress = model.InternalAgencyRegisterAddress;
                            reginternalagncy.District = model.InternalDistrict;
                            reginternalagncy.PinCode = model.InternalPincode;
                            reginternalagncy.State = model.InternalAgencyState;
                            reginternalagncy.Crtd_TS = DateTime.Now;
                            reginternalagncy.Crtd_UserId = model.InternalAgencyUserId;
                            reginternalagncy.AgencyType = 1;
                            reginternalagncy.Status = "Active";
                            reginternalagncy.SeqNbr = (Convert.ToInt32(Sqnbr) + 1);
                            reginternalagncy.ProjectTypeId = model.ProjectType;
                            context.tblAgencyMaster.Add(reginternalagncy);
                            context.SaveChanges();
                            agencyid = reginternalagncy.AgencyId;
                            if (model.AttachName[0] != null && model.AttachName[0] != "")
                            {
                                for (int i = 0; i < model.DocumentType.Length; i++)
                                {
                                    string docpath = "";
                                    docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                    var docfileId = Guid.NewGuid().ToString();
                                    var docname = docfileId + "_" + docpath;

                                    /*Saving the file in server folder*/
                                    model.File[i].UploadFile("AgencyDocument", docname);
                                    tblAgencyDocument document = new tblAgencyDocument();
                                    document.AgencyId = agencyid;
                                    if (model.File[i] != null)
                                    {
                                        document.AgencyDocument = model.File[i].FileName;

                                    }
                                    document.AttachmentPath = docname;
                                    document.AttachmentName = model.AttachName[i];
                                    document.DocumentType = model.DocumentType[i];
                                    document.IsCurrentVersion = true;
                                    document.DocumentUploadUserId = model.UserId;
                                    document.DocumentUpload_Ts = DateTime.Now;
                                    context.tblAgencyDocument.Add(document);
                                    context.SaveChanges();
                                }

                            }
                            transaction.Commit();
                            return 1;
                        }
                        else
                        {
                            var reginternalupdate = context.tblAgencyMaster.Where(UI => UI.AgencyId == model.InternalAgencyId).FirstOrDefault();
                            if (reginternalupdate != null)
                            {
                                reginternalupdate.AgencyName = model.InternalAgencyName;
                                reginternalupdate.AgencyCode = model.InternalAgencyCode;
                                reginternalupdate.ContactPerson = model.InternalAgencyContactPerson;
                                reginternalupdate.ContactNumber = model.InternalAgencyContactNumber;
                                reginternalupdate.ContactEmail = model.InternalConatactEmail;
                                reginternalupdate.Address = model.InternalAgencyAddress;
                                reginternalupdate.AgencyRegisterName = model.InternalAgencyRegisterName;
                                reginternalupdate.AgencyRegisterAddress = model.InternalAgencyRegisterAddress;
                                reginternalupdate.District = model.InternalDistrict;
                                reginternalupdate.PinCode = model.InternalPincode;
                                reginternalupdate.State = model.InternalAgencyState;
                                reginternalupdate.Lastupdate_TS = DateTime.Now;
                                reginternalupdate.LastupdatedUserid = model.InternalAgencyUserId;
                                reginternalupdate.ProjectTypeId = model.ProjectType;
                                reginternalupdate.AgencyType = 1;
                                reginternalupdate.Status = "Active";
                                context.SaveChanges();
                                if (model.AttachName[0] != null && model.AttachName[0] != "")
                                {
                                    var deldocument = (from RD in context.tblAgencyDocument
                                                       where RD.AgencyId == model.InternalAgencyId &&
                                                       !model.DocumentId.Contains(RD.AgencyDocumentId) && RD.IsCurrentVersion == true
                                                       select RD).ToList();
                                    int delCount = deldocument.Count();
                                    if (delCount > 0)
                                    {
                                        for (int i = 0; i < delCount; i++)
                                        {
                                            deldocument[i].IsCurrentVersion = false;
                                            context.SaveChanges();
                                        }
                                    }
                                    for (int i = 0; i < model.DocumentType.Length; i++)
                                    {
                                        if (model.DocumentType[i] != 0)
                                        {
                                            var docid = model.DocumentId[i];
                                            var query = (from D in context.tblAgencyDocument
                                                         where (D.AgencyDocumentId == docid && D.AgencyId == model.InternalAgencyId && D.IsCurrentVersion == true)
                                                         select D).ToList();
                                            if (query.Count == 0)
                                            {
                                                string docpath = "";
                                                docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                                var docfileId = Guid.NewGuid().ToString();
                                                var docname = docfileId + "_" + docpath;

                                                /*Saving the file in server folder*/
                                                model.File[i].UploadFile("AgencyDocument", docname);
                                                tblAgencyDocument document = new tblAgencyDocument();
                                                document.AgencyId = model.InternalAgencyId;
                                                if (model.File[i] != null)
                                                {
                                                    document.AgencyDocument = model.File[i].FileName;

                                                }
                                                document.AttachmentPath = docname;
                                                document.AttachmentName = model.AttachName[i];
                                                document.DocumentType = model.DocumentType[i];
                                                document.IsCurrentVersion = true;
                                                document.DocumentUploadUserId = model.UserId;
                                                document.DocumentUpload_Ts = DateTime.Now;
                                                context.tblAgencyDocument.Add(document);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].DocumentType = model.DocumentType[i];
                                                query[0].AttachmentName = model.AttachName[i];
                                                query[0].DocumentUploadUserId = model.UserId;
                                                query[0].DocumentUpload_Ts = DateTime.Now;
                                                query[0].IsCurrentVersion = true;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                transaction.Commit();
                            }

                            return 3;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var Msg = ex;
                        return -1;
                    }
                }
            }

        }
        public static List<InternalAgencyViewModel> GetInternalAgency(InternalAgencyViewModel model)
        {
            try
            {
                List<InternalAgencyViewModel> Internallist = new List<InternalAgencyViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from IA in context.tblAgencyMaster
                                 from C in context.tblCodeControl
                                 where IA.Status == "Active" && IA.AgencyType == 1 && IA.AgencyType == C.CodeValAbbr && C.CodeName == "SponsoredProjectSubtype"
                                 && (IA.AgencyName.Contains(model.SearchAgencyName) || IA.GSTIN.Contains(model.SearchAgencyName) || model.SearchAgencyName == null)
                                 && (IA.AgencyCode.Contains(model.SearchAgencyCode) || model.SearchAgencyCode == null)

                                 select new { IA.GSTIN, IA.AgencyId, IA.AgencyName, IA.ContactPerson, IA.ContactEmail, C.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Internallist.Add(new InternalAgencyViewModel()
                            {
                                sno = i + 1,
                                InternalAgencyId = query[i].AgencyId,
                                InternalAgencyName = query[i].AgencyName + " - " + query[i].GSTIN,
                                InternalAgencyContactPerson = query[i].ContactPerson,
                                InternalConatactEmail = query[i].ContactEmail,
                                InternalAgencyType = query[i].CodeValDetail
                            });
                        }
                    }
                    return Internallist;
                }
            }
            catch (Exception ex)
            {
                List<InternalAgencyViewModel> Internallist = new List<InternalAgencyViewModel>();
                return Internallist;
            }
        }
        public static InternalAgencyViewModel EditInternalAgency(int agencyId)
        {
            try
            {
                InternalAgencyViewModel agency = new InternalAgencyViewModel();
                using (var context = new IOASDBEntities())
                {
                    var filenamelist = (from F in context.tblAgencyDocument
                                        where F.AgencyId == agencyId && F.IsCurrentVersion == true
                                        select new { F.AgencyDocumentId, F.AttachmentPath, F.AgencyDocument, F.AttachmentName, F.DocumentType }).ToList();
                    var query = (from IA in context.tblAgencyMaster
                                 where (IA.AgencyId == agencyId)
                                 select new
                                 {
                                     IA.AgencyId,
                                     IA.AgencyName,
                                     IA.AgencyCode,
                                     IA.ContactPerson,
                                     IA.ContactNumber,
                                     IA.ContactEmail,
                                     IA.Address,
                                     IA.AgencyRegisterName,
                                     IA.AgencyRegisterAddress,
                                     IA.District,
                                     IA.PinCode,
                                     IA.State,
                                     IA.ProjectTypeId

                                 }).FirstOrDefault();
                    if (query != null)
                    {
                        agency.InternalAgencyId = query.AgencyId;
                        agency.InternalAgencyName = query.AgencyName;
                        agency.InternalAgencyCode = query.AgencyCode;
                        agency.InternalAgencyContactPerson = query.ContactPerson;
                        agency.InternalAgencyContactNumber = query.ContactNumber;
                        agency.InternalConatactEmail = query.ContactEmail;
                        agency.InternalAgencyAddress = query.Address;
                        agency.InternalAgencyRegisterName = query.AgencyRegisterName;
                        agency.InternalAgencyRegisterAddress = query.AgencyRegisterAddress;
                        agency.InternalDistrict = query.District;
                        agency.InternalPincode = query.PinCode;
                        agency.InternalAgencyState = query.State;
                        agency.ProjectType = Convert.ToInt32(query.ProjectTypeId);
                        if (filenamelist.Count > 0)
                        {
                            int[] _docid = new int[filenamelist.Count];
                            int[] _doctype = new int[filenamelist.Count];
                            string[] _docname = new string[filenamelist.Count];
                            string[] _attchname = new string[filenamelist.Count];
                            string[] _docpath = new string[filenamelist.Count];
                            for (int i = 0; i < filenamelist.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(filenamelist[i].AgencyDocumentId);
                                _doctype[i] = Convert.ToInt32(filenamelist[i].DocumentType);
                                _docname[i] = filenamelist[i].AgencyDocument;
                                _attchname[i] = filenamelist[i].AttachmentName;
                                _docpath[i] = filenamelist[i].AttachmentPath;
                            }
                            agency.DocumentId = _docid;
                            agency.DocumentType = _doctype;
                            agency.DocumentName = _docname;
                            agency.AttachName = _attchname;
                            agency.DocPath = _docpath;
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {
                InternalAgencyViewModel agency = new InternalAgencyViewModel();
                return agency;
            }
        }
        public static int DeleteInternalAgency(int agencyId, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var internalAgency = context.tblAgencyMaster.Where(D => D.AgencyId == agencyId).FirstOrDefault();
                    if (internalAgency != null)
                    {
                        internalAgency.Status = "InActive";
                        internalAgency.Lastupdate_TS = DateTime.Now;
                        internalAgency.LastupdatedUserid = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static string InternalAgencyCode()
        {
            try
            {
                string internalagycode = "";
                using (var context = new IOASDBEntities())
                {
                    var maxsqn = (from IA in context.tblAgencyMaster
                                  where IA.AgencyType == 1
                                  select IA.SeqNbr).Max();
                    internalagycode = "I" + (Convert.ToInt32(maxsqn) + 1).ToString("00");
                }
                return internalagycode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static List<MasterlistviewModel> GetVendorCode()
        {
            try
            {
                List<MasterlistviewModel> vendorcode = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from V in context.tblVendorMaster
                                 where V.Status == "Active"
                                 select new { V.VendorId, V.VendorCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            vendorcode.Add(new MasterlistviewModel()
                            {
                                id = query[i].VendorId,
                                name = query[i].VendorCode
                            });
                        }
                    }
                }
                return vendorcode;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> vendorcode = new List<MasterlistviewModel>();
                return vendorcode;
            }
        }

        public static TdsSectionModel GetSectiontds(int sectionId)
        {
            try
            {
                TdsSectionModel tds = new TdsSectionModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from T in context.tblTDSMaster
                                 where (T.TdsMasterId == sectionId)
                                 select new { T.NatureOfIncome, T.Percentage }).FirstOrDefault();
                    if (query != null)
                    {
                        tds.NatureOfIncome = query.NatureOfIncome;
                        tds.Percentage = Convert.ToDecimal(query.Percentage);
                    }
                }
                return tds;
            }
            catch (Exception ex)
            {
                TdsSectionModel tds = new TdsSectionModel();
                return tds;
            }
        }

        public static List<LedgerOBBalanceModel> GetAccountWiseHead(int accounttypid)
        {
            try
            {
                List<LedgerOBBalanceModel> headlist = new List<LedgerOBBalanceModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from vw in context.vwAccountGroupinHeadwise
                                 where (vw.AccountCategoryId == accounttypid && vw.AccountheadStatus == "Active")
                                 select new { vw.Groups, vw.AccountHead, vw.OpeningBal, vw.HeadOpeningBalanceId, vw.FinacialYearId, vw.AccountHeadId }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            headlist.Add(new LedgerOBBalanceModel()
                            {
                                sno = i + 1,
                                AccountGroupName = query[i].Groups,
                                AccountHeadName = query[i].AccountHead,
                                CurrentOpeningBalance = Convert.ToDecimal(query[i].OpeningBal),
                                HeadOpeningBalanceId = Convert.ToInt32(query[i].HeadOpeningBalanceId),
                                FinalYearId = Convert.ToInt32(query[i].FinacialYearId),
                                AccountHeadId = Convert.ToInt32(query[i].AccountHeadId)
                            });
                        }
                    }
                }
                return headlist;
            }
            catch (Exception ex)
            {
                List<LedgerOBBalanceModel> headlist = new List<LedgerOBBalanceModel>();
                return headlist;
            }
        }
        public static LedgerOBBalanceModel GetOpeningBalance(int accheadid)
        {
            try
            {
                LedgerOBBalanceModel model = new LedgerOBBalanceModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from Q in context.vwAccountGroupinHeadwise
                                 where (Q.AccountHeadId == accheadid)
                                 select new { Q.OpeningBal, Q.FinacialYearId, Q.AccountHeadId, Q.HeadOpeningBalanceId }).FirstOrDefault();
                    if (query != null)
                    {
                        model.PopupCurrentOpeningBalance = Convert.ToDecimal(query.OpeningBal);
                        model.FinalYearId = Convert.ToInt32(query.FinacialYearId);
                        model.AccountHeadId = query.AccountHeadId;
                        model.HeadOpeningBalanceId = Convert.ToInt32(query.HeadOpeningBalanceId);
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                LedgerOBBalanceModel model = new LedgerOBBalanceModel();
                return model;
            }
        }
        public static int AddOpeningBalanceLedger(LedgerOBBalanceModel model, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            String Encpassword = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                            var chckuser = context.tblUser.Where(ch => ch.UserName == Username && ch.Password == Encpassword).FirstOrDefault();
                            if (chckuser != null)
                            {
                                if (model.Userid == 1)
                                {
                                    LedgerOBBalanceModel addOBLeg = new LedgerOBBalanceModel();
                                    if (model.HeadOpeningBalanceId == 0)
                                    {
                                        tblHeadOpeningBalance addHB = new tblHeadOpeningBalance();
                                        addHB.FinYearId = model.FinalYearId;
                                        addHB.AccountHeadId = model.AccountHeadId;
                                        addHB.OpeningBalance = model.PopModeifiedOpeningBalance;
                                        addHB.CRTD_TS = DateTime.Now;
                                        addHB.CRTD_By = model.Userid;
                                        addHB.Status = "Active";
                                        context.tblHeadOpeningBalance.Add(addHB);
                                        context.SaveChanges();
                                        transaction.Commit();
                                        return 1;
                                    }
                                    else
                                    {
                                        var chkhOB = context.tblHeadOpeningBalance.Where(H => H.HeadOpeningBalanceId == model.HeadOpeningBalanceId).FirstOrDefault();
                                        if (chkhOB != null)
                                        {
                                            chkhOB.FinYearId = model.FinalYearId;
                                            chkhOB.AccountHeadId = model.AccountHeadId;
                                            chkhOB.OpeningBalance = model.PopModeifiedOpeningBalance;
                                            chkhOB.UPTD_TS = DateTime.Now;
                                            chkhOB.UPTD_By = model.Userid;
                                            context.SaveChanges();
                                            tblUpdateLedgerOBLog addlog = new tblUpdateLedgerOBLog();
                                            addlog.HeadId = model.HeadOpeningBalanceId;
                                            addlog.OBUpdated_Ts = DateTime.Now;
                                            addlog.OBUpdatedUserId = model.Userid;
                                            addlog.OBLedgerOldValue = model.PopupCurrentOpeningBalance;
                                            addlog.OBLedgerCurrentValue = model.PopModeifiedOpeningBalance;
                                            context.tblUpdateLedgerOBLog.Add(addlog);
                                            context.SaveChanges();

                                        }
                                        transaction.Commit();
                                        return 2;
                                    }
                                }
                                else
                                {
                                    return 4;
                                }
                            }
                            else
                            {
                                return 3;
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return -1;

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return -1;
            }
        }

        public static int CreatBankAccount(BankAccountMaster model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    if (model.StaffBankId == null)
                    {

                        if (model.Userid != null)
                        {
                            var query = context.tblStaffBankAccount.Where(M => M.AccountNumber == model.AccountNumber).FirstOrDefault();
                            if (query == null)
                            {
                                if (model.Category == 1)
                                {
                                    var id = Convert.ToInt32(model.Userid);
                                    var exstinguser = context.tblStaffBankAccount.Where(e => e.UserId == id && e.Category == "Professor").FirstOrDefault();
                                    if (exstinguser != null)
                                        return 5;
                                    var vwCombine = (from Vw in context.tblFacultyDetail
                                                     where Vw.CastEmployeeId == id
                                                     select Vw).FirstOrDefault();
                                    tblStaffBankAccount create = new tblStaffBankAccount();
                                    create.Category = "Professor";
                                    create.EmployeeId = vwCombine.EmployeeId;
                                    create.UserId = vwCombine.CastEmployeeId;
                                    create.Name = vwCombine.EmployeeName;
                                    create.Department = vwCombine.DepartmentName;
                                    create.BankType = model.BankType;
                                    create.AccountNumber = model.AccountNumber;
                                    create.IFSCCode = model.IFSCCode;
                                    create.Crts = DateTime.Now;
                                    create.CRTD_By = model.CreateUser;
                                    create.Branch = model.Branch;
                                    create.BankName = model.BankName;
                                    context.tblStaffBankAccount.Add(create);
                                    context.SaveChanges();
                                }
                                else if (model.Category == 2)
                                {
                                    var exstinguser = context.tblStaffBankAccount.Where(e => e.EmployeeId == model.Userid && e.Category == "Student").FirstOrDefault();
                                    if (exstinguser != null)
                                        return 5;
                                    var vwCombine = (from Vw in context.tblStudentDetail
                                                     where Vw.RollNumber == model.Userid
                                                     select Vw).FirstOrDefault();
                                    tblStaffBankAccount create = new tblStaffBankAccount();
                                    create.Category = "Student";
                                    create.EmployeeId = vwCombine.RollNumber;
                                    create.UserId = vwCombine.StudentDetailsId;
                                    create.Name = vwCombine.StudentName;
                                    create.Department = vwCombine.DepartmentName;
                                    create.BankType = model.BankType;
                                    create.AccountNumber = model.AccountNumber;
                                    create.IFSCCode = model.IFSCCode;
                                    create.Crts = DateTime.Now;
                                    create.CRTD_By = model.CreateUser;
                                    create.Branch = model.Branch;
                                    create.BankName = model.BankName;
                                    context.tblStaffBankAccount.Add(create);
                                    context.SaveChanges();
                                }
                                else if (model.Category == 3)
                                {
                                    var id = Convert.ToInt32(model.Userid);
                                    var exstinguser = context.tblStaffBankAccount.Where(e => e.UserId == id && e.Category == "ProjectStaff").FirstOrDefault();
                                    if (exstinguser != null)
                                        return 5;
                                    var vwCombine = (from Vw in context.tblProjectStaffDetail
                                                     where Vw.CastEmployeeId == id
                                                     select Vw).FirstOrDefault();
                                    tblStaffBankAccount create = new tblStaffBankAccount();
                                    create.Category = "ProjectStaff";
                                    create.EmployeeId = vwCombine.EmployeeId;
                                    create.UserId = vwCombine.CastEmployeeId;
                                    create.Name = vwCombine.EmployeeName;
                                    create.Department = vwCombine.DepartmentName;
                                    create.BankType = model.BankType;
                                    create.AccountNumber = model.AccountNumber;
                                    create.IFSCCode = model.IFSCCode;
                                    create.Crts = DateTime.Now;
                                    create.CRTD_By = model.CreateUser;
                                    create.Branch = model.Branch;
                                    create.BankName = model.BankName;
                                    context.tblStaffBankAccount.Add(create);
                                    context.SaveChanges();
                                }
                                else if (model.Category == 4)
                                {
                                    var id = Convert.ToInt32(model.Userid);
                                    var exstinguser = context.tblStaffBankAccount.Where(e => e.UserId == id && e.Category == "Staff").FirstOrDefault();
                                    if (exstinguser != null)
                                        return 5;
                                    var vwCombine = (from Vw in context.tblStaffDetail
                                                     where Vw.CastEmployeeId == id
                                                     select Vw).FirstOrDefault();
                                    tblStaffBankAccount create = new tblStaffBankAccount();
                                    create.Category = "Staff";
                                    create.EmployeeId = vwCombine.EmployeeId;
                                    create.UserId = vwCombine.CastEmployeeId;
                                    create.Name = vwCombine.EmployeeName;
                                    create.Department = vwCombine.DepartmentName;
                                    create.BankType = model.BankType;
                                    create.AccountNumber = model.AccountNumber;
                                    create.IFSCCode = model.IFSCCode;
                                    create.Crts = DateTime.Now;
                                    create.CRTD_By = model.CreateUser;
                                    create.Branch = model.Branch;
                                    create.BankName = model.BankName;
                                    context.tblStaffBankAccount.Add(create);
                                    context.SaveChanges();
                                }
                                else if (model.Category == 5)
                                {
                                    var id = Convert.ToInt32(model.Userid);
                                    var exstinguser = context.tblStaffBankAccount.Where(e => e.UserId == id && e.Category == "AdhocStaff").FirstOrDefault();
                                    if (exstinguser != null)
                                        return 5;
                                    var vwCombine = (from Vw in context.tblProjectAdhocStaffDetails
                                                     where Vw.CastEmployeeId == id
                                                     select Vw).FirstOrDefault();
                                    tblStaffBankAccount create = new tblStaffBankAccount();
                                    create.Category = "AdhocStaff";
                                    create.EmployeeId = vwCombine.EmployeeId;
                                    create.UserId = vwCombine.CastEmployeeId;
                                    create.Name = vwCombine.EmployeeName;
                                    create.Department = vwCombine.DepartmentName;
                                    create.BankType = model.BankType;
                                    create.AccountNumber = model.AccountNumber;
                                    create.IFSCCode = model.IFSCCode;
                                    create.Crts = DateTime.Now;
                                    create.CRTD_By = model.CreateUser;
                                    create.Branch = model.Branch;
                                    create.BankName = model.BankName;
                                    context.tblStaffBankAccount.Add(create);
                                    context.SaveChanges();
                                }
                                else
                                {

                                }
                                return 1;
                            }
                            else
                            {
                                return 4;
                            }
                        }
                        else
                        {
                            return 3;
                        }


                    }
                    else
                    {
                        var UpdateAccount = context.tblStaffBankAccount.Where(m => m.StaffBankId == model.StaffBankId).FirstOrDefault();
                        if (model.Category == 1)
                        {

                            var id = Convert.ToInt32(model.Userid);
                            var vwCombine = (from Vw in context.tblFacultyDetail
                                             where Vw.CastEmployeeId == id
                                             select Vw).FirstOrDefault();

                            UpdateAccount.Category = "Professor";
                            UpdateAccount.EmployeeId = vwCombine.EmployeeId;
                            UpdateAccount.UserId = vwCombine.CastEmployeeId;
                            UpdateAccount.Name = vwCombine.EmployeeName;
                            UpdateAccount.Department = vwCombine.DepartmentName;
                            UpdateAccount.BankType = model.BankType;
                            UpdateAccount.AccountNumber = model.AccountNumber;
                            UpdateAccount.IFSCCode = model.IFSCCode;
                            UpdateAccount.UPDT_TS = DateTime.Now;
                            UpdateAccount.UPDT_By = model.CreateUser;
                            UpdateAccount.Branch = model.Branch;
                            UpdateAccount.BankName = model.BankName;
                            context.SaveChanges();
                        }
                        else if (model.Category == 2)
                        {
                            var vwCombine = (from Vw in context.tblStudentDetail
                                             where Vw.RollNumber == model.Userid
                                             select Vw).FirstOrDefault();

                            UpdateAccount.Category = "Student";
                            UpdateAccount.EmployeeId = vwCombine.RollNumber;
                            UpdateAccount.UserId = vwCombine.StudentDetailsId;
                            UpdateAccount.Name = vwCombine.StudentName;
                            UpdateAccount.Department = vwCombine.DepartmentName;
                            UpdateAccount.BankType = model.BankType;
                            UpdateAccount.AccountNumber = model.AccountNumber;
                            UpdateAccount.IFSCCode = model.IFSCCode;
                            UpdateAccount.UPDT_TS = DateTime.Now;
                            UpdateAccount.UPDT_By = model.CreateUser;
                            UpdateAccount.Branch = model.Branch;
                            UpdateAccount.BankName = model.BankName;
                            context.SaveChanges();
                        }
                        else if (model.Category == 3)
                        {
                            var id = Convert.ToInt32(model.Userid);
                            var vwCombine = (from Vw in context.tblProjectStaffDetail
                                             where Vw.CastEmployeeId == id
                                             select Vw).FirstOrDefault();

                            UpdateAccount.Category = "ProjectStaff";
                            UpdateAccount.EmployeeId = vwCombine.EmployeeId;
                            UpdateAccount.UserId = vwCombine.CastEmployeeId;
                            UpdateAccount.Name = vwCombine.EmployeeName;
                            UpdateAccount.Department = vwCombine.DepartmentName;
                            UpdateAccount.BankType = model.BankType;
                            UpdateAccount.AccountNumber = model.AccountNumber;
                            UpdateAccount.IFSCCode = model.IFSCCode;
                            UpdateAccount.UPDT_TS = DateTime.Now;
                            UpdateAccount.UPDT_By = model.CreateUser;
                            UpdateAccount.Branch = model.Branch;
                            UpdateAccount.BankName = model.BankName;
                            context.SaveChanges();
                        }
                        else if (model.Category == 4)
                        {
                            var id = Convert.ToInt32(model.Userid);
                            var vwCombine = (from Vw in context.tblStaffDetail
                                             where Vw.CastEmployeeId == id
                                             select Vw).FirstOrDefault();

                            UpdateAccount.Category = "Staff";
                            UpdateAccount.EmployeeId = vwCombine.EmployeeId;
                            UpdateAccount.UserId = vwCombine.CastEmployeeId;
                            UpdateAccount.Name = vwCombine.EmployeeName;
                            UpdateAccount.Department = vwCombine.DepartmentName;
                            UpdateAccount.BankType = model.BankType;
                            UpdateAccount.AccountNumber = model.AccountNumber;
                            UpdateAccount.IFSCCode = model.IFSCCode;
                            UpdateAccount.UPDT_TS = DateTime.Now;
                            UpdateAccount.UPDT_By = model.CreateUser;
                            UpdateAccount.Branch = model.Branch;
                            UpdateAccount.BankName = model.BankName;
                            context.SaveChanges();
                        }
                        else if (model.Category == 5)
                        {
                            var id = Convert.ToInt32(model.Userid);
                            var vwCombine = (from Vw in context.tblProjectAdhocStaffDetails
                                             where Vw.CastEmployeeId == id
                                             select Vw).FirstOrDefault();

                            UpdateAccount.Category = "AdhocStaff";
                            UpdateAccount.EmployeeId = vwCombine.EmployeeId;
                            UpdateAccount.UserId = vwCombine.CastEmployeeId;
                            UpdateAccount.Name = vwCombine.EmployeeName;
                            UpdateAccount.Department = vwCombine.DepartmentName;
                            UpdateAccount.BankType = model.BankType;
                            UpdateAccount.AccountNumber = model.AccountNumber;
                            UpdateAccount.IFSCCode = model.IFSCCode;
                            UpdateAccount.UPDT_TS = DateTime.Now;
                            UpdateAccount.UPDT_By = model.CreateUser;
                            UpdateAccount.Branch = model.Branch;
                            UpdateAccount.BankName = model.BankName;
                            context.SaveChanges();
                        }
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static BankAccountMaster EditBankMaster(int StaffBankid)
        {
            try
            {
                BankAccountMaster edit = new BankAccountMaster();
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblStaffBankAccount.Where(m => m.StaffBankId == StaffBankid).FirstOrDefault();
                    if (query != null)
                    {
                        if (query.Category == "Professor")
                        {
                            edit.Category = 1;
                            edit.AccountantName = query.EmployeeId + "-" + query.Name;
                            edit.Userid = query.UserId.ToString();
                            edit.BankType = query.BankType ?? 0;
                            edit.AccountNumber = query.AccountNumber;
                            edit.IFSCCode = query.IFSCCode;
                            edit.Branch = query.Branch;
                            edit.BankName = query.BankName;
                            edit.StaffBankId = query.StaffBankId;
                        }
                        else if (query.Category == "Student")
                        {
                            edit.Category = 2;
                            edit.AccountantName = query.EmployeeId + "-" + query.Name;
                            edit.Userid = query.EmployeeId;
                            edit.BankType = query.BankType ?? 0;
                            edit.AccountNumber = query.AccountNumber;
                            edit.IFSCCode = query.IFSCCode;
                            edit.Branch = query.Branch;
                            edit.BankName = query.BankName;
                            edit.StaffBankId = query.StaffBankId;
                        }
                        else if (query.Category == "ProjectStaff")
                        {
                            edit.Category = 3;
                            edit.AccountantName = query.EmployeeId + "-" + query.Name;
                            edit.Userid = query.UserId.ToString();
                            edit.BankType = query.BankType ?? 0;
                            edit.AccountNumber = query.AccountNumber;
                            edit.IFSCCode = query.IFSCCode;
                            edit.Branch = query.Branch;
                            edit.BankName = query.BankName;
                            edit.StaffBankId = query.StaffBankId;
                        }
                        else if (query.Category == "Staff")
                        {
                            edit.Category = 4;
                            edit.AccountantName = query.EmployeeId + "-" + query.Name;
                            edit.Userid = query.UserId.ToString();
                            edit.BankType = query.BankType ?? 0;
                            edit.AccountNumber = query.AccountNumber;
                            edit.IFSCCode = query.IFSCCode;
                            edit.Branch = query.Branch;
                            edit.BankName = query.BankName;
                            edit.StaffBankId = query.StaffBankId;
                        }
                        else if (query.Category == "AdhocStaff")
                        {
                            edit.Category = 5;
                            edit.AccountantName = query.EmployeeId + "-" + query.Name;
                            edit.Userid = query.UserId.ToString();
                            edit.BankType = query.BankType ?? 0;
                            edit.AccountNumber = query.AccountNumber;
                            edit.IFSCCode = query.IFSCCode;
                            edit.Branch = query.Branch;
                            edit.BankName = query.BankName;
                            edit.StaffBankId = query.StaffBankId;
                        }
                    }
                }
                return edit;
            }
            catch (Exception ex)
            {
                BankAccountMaster edit = new BankAccountMaster();
                return edit;
            }
        }
        #region AdhocPICreation

        public static int CreateAdhocPI(AdhocPICreationModel model, int UserId)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.FacultyDetailsId == null)
                        {
                            var Sqnbr = (from sq in context.tblFacultyDetail
                                         select sq.SqNbr).Max();
                            var chkemp = context.tblFacultyDetail.Where(x => x.EmployeeId == model.PICode.ToUpper()).FirstOrDefault();
                            if (chkemp != null)
                                return 3;
                            tblFacultyDetail addPI = new tblFacultyDetail();
                            addPI.EmployeeName = model.PIName;
                            addPI.EmployeeId = model.PICode.ToUpper();
                            addPI.EmployeeDesignation = model.Designation;
                            addPI.DepartmentCode = model.DepartmentCode.ToUpper();
                            addPI.DepartmentName = model.DepartmentName.ToUpper();
                            addPI.Email = model.Email;
                            string id = Convert.ToInt32((Sqnbr) + 1).ToString("10000000");
                            addPI.CastEmployeeId = Convert.ToInt32(id);
                            addPI.SqNbr = Convert.ToInt32((Sqnbr) + 1);
                            addPI.Adhoc_f = true;
                            addPI.CRTD_BY = UserId;
                            addPI.CRTD_TS = DateTime.Now;
                            context.tblFacultyDetail.Add(addPI);
                            context.SaveChanges();
                            transaction.Commit();
                            return 1;
                        }
                        else
                        {
                            var facupdate = context.tblFacultyDetail.Where(x => x.FacultyDetailsId == model.FacultyDetailsId).FirstOrDefault();
                            if (facupdate != null)
                            {
                                facupdate.EmployeeName = model.PIName;
                                facupdate.EmployeeId = model.PICode.ToUpper();
                                facupdate.EmployeeDesignation = model.Designation;
                                facupdate.DepartmentCode = model.DepartmentCode.ToUpper();
                                facupdate.DepartmentName = model.DepartmentName.ToUpper();
                                facupdate.Email = model.Email;
                                facupdate.UPTD_BY = UserId;
                                facupdate.UPTD_TS = DateTime.Now;
                            }
                            context.SaveChanges();
                            transaction.Commit();
                            return 2;
                        }
                    }
                    catch (Exception ex)
                    {
                        return -1;
                    }
                }
            }
        }

        public static AdhocPISearchModel GetAdhocPIList(AdhocPISearchModel model, int page, int pageSize)
        {
            AdhocPISearchModel adhocsearch = new AdhocPISearchModel();
            int skiprec = 0;
            if (page == 1)
            {
                skiprec = 0;
            }
            else
            {
                skiprec = (page - 1) * pageSize;
            }
            try
            {
                List<AdhocPICreationModel> searchlist = new List<AdhocPICreationModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from ad in context.tblFacultyDetail
                                 where ad.Adhoc_f == true
                                 && (ad.EmployeeId.Contains(model.PICode) || model.PICode == null)
                                 && (ad.EmployeeName.Contains(model.PIName) || model.PIName == null)
                                 && (ad.DepartmentName.Contains(model.Department) || model.Department == null)
                                 && (ad.Email.Contains(model.Email) || model.Email == null)
                                 orderby ad.FacultyDetailsId descending
                                 select new { ad.FacultyDetailsId, ad.EmployeeId, ad.EmployeeName, ad.DepartmentName, ad.Email }).Skip(skiprec).Take(pageSize).ToList();
                    adhocsearch.TotalRecords = (from ad in context.tblFacultyDetail
                                                where ad.Adhoc_f == true
                                                && (ad.EmployeeId.Contains(model.PICode) || model.PICode == null)
                                                && (ad.EmployeeName.Contains(model.PIName) || model.PIName == null)
                                                && (ad.DepartmentName.Contains(model.Department) || model.Department == null)
                                                && (ad.Email.Contains(model.Email) || model.Email == null)
                                                orderby ad.FacultyDetailsId descending
                                                select new { ad.FacultyDetailsId, ad.EmployeeId, ad.EmployeeName, ad.DepartmentName, ad.Email }).Count();
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
                            searchlist.Add(new AdhocPICreationModel()
                            {
                                SNo = sno + i,
                                FacultyDetailsId = query[i].FacultyDetailsId,
                                PICode = query[i].EmployeeId,
                                PIName = query[i].EmployeeName,
                                DepartmentName = query[i].DepartmentName,
                                Email = query[i].Email
                            });
                        }
                    }
                    adhocsearch.list = searchlist;
                }
                return adhocsearch;
            }
            catch (Exception ex)
            {
                return adhocsearch;
            }
        }
        public static AdhocPICreationModel EditPICreation(int adhocId)
        {
            AdhocPICreationModel editModel = new AdhocPICreationModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFacultyDetail.Where(x => x.FacultyDetailsId == adhocId).FirstOrDefault();
                    if (query != null)
                    {
                        editModel.FacultyDetailsId = query.FacultyDetailsId;
                        editModel.PICode = query.EmployeeId;
                        editModel.PIName = query.EmployeeName;
                        editModel.DepartmentCode = query.DepartmentCode.Trim();
                        editModel.DepartmentName = query.DepartmentName;
                        editModel.Designation = query.EmployeeDesignation;
                        editModel.Email = query.Email;
                    }
                }
                return editModel;
            }
            catch (Exception ex)
            {
                return editModel;
            }
        }
        #endregion


    }
}