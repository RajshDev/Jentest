using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WorkFlow;
using static IOAS.Infrastructure.ExtensionMethods;

namespace IOAS.GenericServices
{
    public class ProposalService
    {
        //Creation of Proposal (Proposal Opening) - Save data in DB 
        public int CreateProposal(CreateProposalModel model, HttpPostedFileBase[] file)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if ((model.ProposalID == null) || (model.ProposalID == 0))
                        {
                            DataModel.tblProposal create = new DataModel.tblProposal();
                            //var chkproposal = context.tblProposal.FirstOrDefault(dup => dup.ProposalTitle == model.Projecttitle && /*dup.RoleId != 3 &&*/ dup.IsDeleted != true && dup.Status != "InActive" && dup.Status != "Rejected");
                            //if (chkproposal != null)
                            //    return 0;
                            var userquery = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == model.PIname);
                            create.InwardDate = model.Proposalinwarddate;
                            create.ProposalSource = model.ProposalSource;
                            create.ProjectType = model.ProjectType;
                            create.ProposalTitle = model.Projecttitle;
                            create.Department = model.Department;
                            create.PI = model.PIname;
                            create.PIEmail = model.PIEmail;
                            create.SponsoringAgency = model.SponsoringAgency;
                            //create.ProposalValue = model.TotalValue ?? 0;
                            create.BasicValue = model.BasicValue ?? 0;
                            create.ApplicableTax = model.ApplicableTaxes ?? 0;
                            create.TentativeCloseDate = model.TentativeCloseDate;
                            create.TentativeStartDate = model.TentativeStartDate;
                            create.FinancialYear = model.FinYear;
                            if (model.IsthereAnyArtificialIntelligence == "Yes")
                            {
                                create.AI_f = true;
                                create.AI_Description = model.AIDescription;
                            }
                            else
                                create.AI_f = false;

                            create.AI_domain = model.Domainrelevant;
                            
                            
                            if (model.ProjectType == 1)
                            {
                                create.SponsoredProposalTypeCode = model.SponsoredProposalFundingType;
                                if (model.SponsoredProposalFundingType != null)
                                {
                                    int sponCategory = context.tblConsultancyFundingCategory.Where(x => x.ConsultancyFundingCategoryId == model.SponsoredProposalFundingType).Select(x => x.SponsoredCategory).FirstOrDefault() ?? 0;
                                    if (sponCategory != 0)
                                    {
                                        create.SponsoredProposalType = sponCategory;
                                    }
                                }
                            }
                           // if (model.ProposalSource == 1 || model.ProposalSource == 3)
                           // {
                                create.SourceReferenceNumber = model.RefNumber;
                           // }
                          //  if (model.ProposalSource == 2)
                            //{
                                create.SourceEmailDate = model.EmailDate;
                            //}
                            //create.DurationOfProjectYears = model.Projectdurationyears;
                            //create.DurationOfProjectMonths = model.Projectdurationmonths;
                            create.PersonApplied = model.Personapplied;
                            create.PersonAppliedInstitute = model.PersonAppliedInstitute;
                            create.PersonAppliedPlace = model.PersonAppliedPlace;
                            //create.Inputdate = model.Inputdate;
                            //create.ProposalApproveddate = model.ProposalApproveddate;
                            create.Otherinstcopi_Qust_1 = model.Otherinstcopi_Qust_1;
                            create.SanctionNumber = model.SanctionNumber;
                            create.Remarks = model.Remarks;
                            create.Status = "Open";
                            create.Crtd_Userid = model.ProposalcrtdID;
                            create.Crtd_TS = DateTime.Now;

                            if (model.ProjectSubtype != 0 && model.ProjectSubtype != null)
                            {
                                create.ProjectSubType = model.ProjectSubtype;
                            }
                            if (model.ProjectCategory != 0 && model.ProjectCategory != null)
                            {
                                create.ProjectCategory = model.ProjectCategory;
                            }
                            if (model.ProjectSubtype == 2)
                            {
                                create.Scheme = model.Schemename;
                                if (model.ProjectCategory == 1)
                                    create.ProjectSchemeCode = model.SchemeCode;
                            }
                            if (model.ProjectType == 2)
                            {
                                create.Scheme = model.Constype;
                                //create.ProjectSchemeCode = model.Constypecode;
                            }

                            var Sequencenumber = Common.getseqncenumber(Convert.ToInt32(model.FinYear));
                            var finYear = Common.GetFinYear(model.FinYear ?? 0);
                            //int year = (DateTime.Now.Year) % 100;
                            //var institutecode = "IITM"; /*Common.getInstituteCode(PIusername);*/
                            var projectTypeCode = model.ProjectType == 1 ? "SPON" : "CON";
                            if (Sequencenumber > 0)
                            {
                                model.ProjectNumber = projectTypeCode + "_" + finYear + "_" + Sequencenumber.ToString("000");//year + "_" + institutecode + "_" + Sequencenumber + "P";
                            }
                            else
                            {
                                model.ProjectNumber = projectTypeCode + "_" + finYear + "_" + "001";//year + "_" + institutecode + "_" + "1" + "P";
                            }
                            create.ProposalNumber = model.ProjectNumber;
                            context.tblProposal.Add(create);
                            context.SaveChanges();
                            int proposalid = create.ProposalId;
                            if (model.CoPIname != null && model.CoPIname[0] != 0)
                            {
                                for (int i = 0; i < model.CoPIname.Length; i++)
                                {

                                    DataModel.tblProposalCoPI Copi = new DataModel.tblProposalCoPI();
                                    Copi.ProposalId = proposalid;
                                    Copi.Name = model.CoPIname[i];
                                    Copi.Department = model.CoPIDepartment[i];
                                    Copi.Email = model.CoPIEmail[i];
                                    Copi.CrtdUserId = model.ProposalcrtdID;
                                    Copi.Crtd_TS = DateTime.Now;
                                    context.tblProposalCoPI.Add(Copi);
                                    context.SaveChanges();

                                }
                            }
                            if (model.OtherInstituteCoPIName != null && model.OtherInstituteCoPIName[0] != "")
                            {
                                for (int i = 0; i < model.OtherInstituteCoPIName.Length; i++)
                                {

                                    DataModel.tblOtherInstituteCoPI Copi = new DataModel.tblOtherInstituteCoPI();
                                    Copi.ProposalId = proposalid;
                                    Copi.Name = model.OtherInstituteCoPIName[i];
                                    Copi.Institution = model.CoPIInstitute[i];
                                    Copi.Department = model.OtherInstituteCoPIDepartment[i];
                                    Copi.Remarks = model.RemarksforOthrInstCoPI[i];
                                    Copi.CrtdUserId = model.ProposalcrtdID;
                                    Copi.Crtd_TS = DateTime.Now;
                                    context.tblOtherInstituteCoPI.Add(Copi);
                                    context.SaveChanges();

                                }
                            }
                            if (model.AttachName != null && model.AttachName[0] != "")
                            {
                                
                                for (int i = 0; i < model.DocType.Length; i++)
                                {
                                    string docpath = " "; var docfileId = Guid.NewGuid().ToString();
                                    if (model.WFproposalid>0 && file[i] == null)
                                    {
                                        using(var WfContext=new IOASWorkFlowEntities1())
                                        {
                                            int WfdocId = model.Docid[i];
                                            var WFDocQry = WfContext.tblProposalSupportDocuments.Where(m => m.ProposalId == model.WFproposalid && m.DocId == WfdocId).FirstOrDefault();
                                            string WFfile = WFDocQry.DocName;
                                            string WFfilepath = "Proposal/";
                                            byte[] fileData = WFfile.WFProposalDownloadFile(Common.GetDirectoryName(WFfilepath));
                                            docpath = System.IO.Path.GetFileName(WFDocQry.DocName);
                                           // byte[] bytes = System.IO.File.ReadAllBytes(localPath);
                                            HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(fileData);
                                            var docname = docfileId + "_" + docpath;

                                            /*Saving the file in server folder*/
                                            objFile.UploadFile("SupportDocuments", docname);

                                            DataModel.tblProposalSupportDocuments Document = new DataModel.tblProposalSupportDocuments();

                                            Document.ProposalId = proposalid;
                                            if (objFile != null)
                                            {
                                                Document.DocName = WFfile;
                                            }
                                            Document.AttachmentName = model.AttachName[i];
                                            Document.DocType = model.DocType[i];
                                            Document.AttachmentPath = docname;
                                            Document.DocUploadUserid = model.ProposalcrtdID;
                                            Document.DocUpload_TS = DateTime.Now;
                                            Document.IsCurrentVersion = true;
                                            context.tblProposalSupportDocuments.Add(Document);
                                            context.SaveChanges();

                                            tblSupportDocuments ProjectDocuments = new tblSupportDocuments();

                                            ProjectDocuments.ProposalId = proposalid;
                                            if (objFile != null)
                                            {
                                                ProjectDocuments.DocName = WFfile;
                                            }
                                            ProjectDocuments.AttachmentName = model.AttachName[i];
                                            ProjectDocuments.DocType = model.DocType[i];
                                            ProjectDocuments.AttachmentPath = docname;
                                            ProjectDocuments.DocUploadUserid = model.ProposalcrtdID;
                                            ProjectDocuments.DocUpload_TS = DateTime.Now;
                                            ProjectDocuments.IsCurrentVersion = true;
                                            context.tblSupportDocuments.Add(ProjectDocuments);
                                            context.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        docpath = System.IO.Path.GetFileName(file[i].FileName);
                                        var docname = docfileId + "_" + docpath;

                                        /*Saving the file in server folder*/
                                        file[i].UploadFile("SupportDocuments", docname);

                                        DataModel.tblProposalSupportDocuments Document = new DataModel.tblProposalSupportDocuments();

                                        Document.ProposalId = proposalid;
                                        if (file[i] != null)
                                        {
                                            Document.DocName = file[i].FileName;
                                        }
                                        Document.AttachmentName = model.AttachName[i];
                                        Document.DocType = model.DocType[i];
                                        Document.AttachmentPath = docname;
                                        Document.DocUploadUserid = model.ProposalcrtdID;
                                        Document.DocUpload_TS = DateTime.Now;
                                        Document.IsCurrentVersion = true;
                                        context.tblProposalSupportDocuments.Add(Document);
                                        context.SaveChanges();

                                        tblSupportDocuments ProjectDocuments = new tblSupportDocuments();

                                        ProjectDocuments.ProposalId = proposalid;
                                        if (file[i] != null)
                                        {
                                            ProjectDocuments.DocName = file[i].FileName;
                                        }
                                        ProjectDocuments.AttachmentName = model.AttachName[i];
                                        ProjectDocuments.DocType = model.DocType[i];
                                        ProjectDocuments.AttachmentPath = docname;
                                        ProjectDocuments.DocUploadUserid = model.ProposalcrtdID;
                                        ProjectDocuments.DocUpload_TS = DateTime.Now;
                                        ProjectDocuments.IsCurrentVersion = true;
                                        context.tblSupportDocuments.Add(ProjectDocuments);
                                        context.SaveChanges();
                                    }
                                   
                                    
                                    

                                    //file[i].UploadFile("SupportDocuments", docname);
                                }
                            }
                            if (model.WFproposalid > 0)
                            {
                                tblWorkFlowlog log = new tblWorkFlowlog();
                                log.Referenceid = create.ProposalId;
                                log.Referencenbr = create.ProposalNumber;
                                log.WFreferencenbr = Common.GetWFProposalNo(model.WFproposalid).name;
                                log.WFreferencetype = "Proposal";
                                log.CRTD_TS = DateTime.Now;
                                log.CRTD_BY = model.ProposalcrtdID;
                                context.tblWorkFlowlog.Add(log);
                                context.SaveChanges();
                                using (var WFContext = new IOASWorkFlowEntities1())
                                {
                                    var Qry = WFContext.tblProposal.Where(m => m.ProposalId == model.WFproposalid).FirstOrDefault();
                                    if (Qry != null)
                                    {
                                        Qry.WorkflowStatus = true;
                                        WFContext.SaveChanges();
                                    }
                                }
                            }
                            transaction.Commit();
                            return proposalid;
                        }
                        else
                        {

                            var userquery = context.tblProposal.FirstOrDefault(m => m.ProposalId == model.ProposalID);
                            if (userquery != null)
                            {
                                userquery.InwardDate = model.Proposalinwarddate;
                                userquery.ProposalSource = model.ProposalSource;
                                userquery.ProjectType = model.ProjectType;
                                userquery.ProposalTitle = model.Projecttitle;
                                userquery.Department = model.Department;
                                userquery.PI = model.PIname;
                                userquery.PIEmail = model.PIEmail;
                                userquery.SponsoringAgency = model.SponsoringAgency;
                                //userquery.ProposalValue = model.TotalValue ?? 0;
                                userquery.BasicValue = model.BasicValue ?? 0;
                                userquery.ApplicableTax = model.ApplicableTaxes ?? 0;
                                //userquery.DurationOfProjectYears = model.Projectdurationyears;
                                //userquery.DurationOfProjectMonths = model.Projectdurationmonths;
                                userquery.TentativeCloseDate = model.TentativeCloseDate;
                                userquery.TentativeStartDate = model.TentativeStartDate;
                                userquery.FinancialYear = model.FinYear;
                                if (model.IsthereAnyArtificialIntelligence == "Yes")
                                {
                                    userquery.AI_f = true;
                                    userquery.AI_Description = model.AIDescription;
                                }
                                else
                                    userquery.AI_f = false;

                                userquery.AI_domain = model.Domainrelevant;
                                if (model.ProposalSource == 1 || model.ProposalSource == 3)
                                {
                                    userquery.SourceReferenceNumber = model.RefNumber;
                                }
                                if (model.ProposalSource == 2)
                                {
                                    userquery.SourceEmailDate = model.EmailDate;
                                }
                                userquery.PersonApplied = model.Personapplied;
                                userquery.PersonAppliedInstitute = model.PersonAppliedInstitute;
                                userquery.PersonAppliedPlace = model.PersonAppliedPlace;
                                //userquery.Inputdate = model.Inputdate;
                                //userquery.ProposalApproveddate = model.ProposalApproveddate;
                                userquery.Otherinstcopi_Qust_1 = model.Otherinstcopi_Qust_1;
                                userquery.SanctionNumber = model.SanctionNumber;
                                userquery.Remarks = model.Remarks;
                                if (model.ProjectType == 1)
                                {
                                    userquery.SponsoredProposalTypeCode = model.SponsoredProposalFundingType;
                                    if (model.SponsoredProposalFundingType != null)
                                    {
                                        int sponCategory = context.tblConsultancyFundingCategory.Where(x => x.ConsultancyFundingCategoryId == model.SponsoredProposalFundingType).Select(x => x.SponsoredCategory).FirstOrDefault() ?? 0;
                                        if (sponCategory != 0)
                                        {
                                            userquery.SponsoredProposalType = sponCategory;
                                        }
                                    }
                                }

                                if (model.ProjectSubtype != 0 && model.ProjectSubtype != null)
                                {
                                    userquery.ProjectSubType = model.ProjectSubtype;
                                }
                                if (model.ProjectCategory != 0 && model.ProjectCategory != null)
                                {
                                    userquery.ProjectCategory = model.ProjectCategory;
                                }
                                if (model.ProjectSubtype == 2)
                                {
                                    userquery.Scheme = model.Schemename;
                                    if (model.ProjectCategory == 1)
                                        userquery.ProjectSchemeCode = model.SchemeCode;
                                }
                                if (model.ProjectType == 2)
                                {
                                    userquery.Scheme = model.Constype;
                                    //userquery.ProjectSchemeCode = model.Constypecode;
                                }
                                userquery.Updt_Userid = model.ProposalcrtdID;
                                userquery.Updt_TS = DateTime.Now;
                                context.SaveChanges();
                                int proposalid = userquery.ProposalId;

                                if (model.CoPIDepartment[0] != "")
                                {
                                    for (int i = 0; i < model.CoPIname.Length; i++)
                                    {
                                        if (model.CoPIDepartment[i] != "")
                                        {
                                            var copiid = model.CoPIid[i];
                                            var query = (from CoPI in context.tblProposalCoPI
                                                         where CoPI.CoPIId == copiid && CoPI.ProposalId == proposalid && CoPI.isdeleted != true
                                                         select CoPI).ToList();
                                            if (query.Count == 0)
                                            {
                                                DataModel.tblProposalCoPI Copi = new DataModel.tblProposalCoPI();
                                                Copi.ProposalId = proposalid;
                                                Copi.Name = model.CoPIname[i];
                                                Copi.Department = model.CoPIDepartment[i];
                                                Copi.Email = model.CoPIEmail[i];
                                                Copi.CrtdUserId = model.ProposalcrtdID;
                                                Copi.Crtd_TS = DateTime.Now;
                                                context.tblProposalCoPI.Add(Copi);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].Name = model.CoPIname[i];
                                                query[0].Department = model.CoPIDepartment[i];
                                                query[0].Email = model.CoPIEmail[i];
                                                query[0].UpdtUserId = model.ProposalcrtdID;
                                                query[0].Updt_TS = DateTime.Now;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    var delCOPIQuery = (from CoPI in context.tblProposalCoPI
                                                        where CoPI.ProposalId == proposalid &&
                                                        !model.CoPIname.Contains(CoPI.Name) && CoPI.isdeleted != true
                                                        select CoPI).ToList();
                                    int delCOPICount = delCOPIQuery.Count();
                                    if (delCOPICount > 0)
                                    {
                                        for (int i = 0; i < delCOPICount; i++)
                                        {
                                            delCOPIQuery[i].isdeleted = true;
                                            delCOPIQuery[i].DeletedDate = DateTime.Now;
                                            delCOPIQuery[i].DeletedUserid = model.ProposalcrtdID;
                                            context.SaveChanges();
                                        }
                                    }
                                }

                                if (model.CoPIInstitute != null && model.CoPIInstitute[0] != "")
                                {
                                    if (model.Otherinstcopi_Qust_1 == "Yes")
                                    {
                                        for (int i = 0; i < model.CoPIInstitute.Length; i++)
                                        {
                                            var copiid = model.OtherInstituteCoPIid[i];
                                            var query = (from CoPI in context.tblOtherInstituteCoPI
                                                         where CoPI.CoPIId == copiid && CoPI.ProposalId == proposalid && CoPI.IsDeleted != true
                                                         select CoPI).ToList();
                                            if (query.Count == 0)
                                            {
                                                DataModel.tblOtherInstituteCoPI Copi = new DataModel.tblOtherInstituteCoPI();
                                                Copi.ProposalId = proposalid;
                                                Copi.Institution = model.CoPIInstitute[i];
                                                Copi.Name = model.OtherInstituteCoPIName[i];
                                                Copi.Department = model.OtherInstituteCoPIDepartment[i];
                                                Copi.Remarks = model.RemarksforOthrInstCoPI[i];
                                                Copi.CrtdUserId = model.ProposalcrtdID;
                                                Copi.Crtd_TS = DateTime.Now;
                                                context.tblOtherInstituteCoPI.Add(Copi);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].Institution = model.CoPIInstitute[i];
                                                query[0].Name = model.OtherInstituteCoPIName[i];
                                                query[0].Department = model.OtherInstituteCoPIDepartment[i];
                                                query[0].Remarks = model.RemarksforOthrInstCoPI[i];
                                                query[0].UpdtUserId = model.ProposalcrtdID;
                                                query[0].Updt_TS = DateTime.Now;
                                                context.SaveChanges();
                                            }

                                        }
                                        var delOtherCOPIQuery = (from CoPI in context.tblOtherInstituteCoPI
                                                                 where CoPI.ProposalId == proposalid &&
                                                                 !model.OtherInstituteCoPIName.Contains(CoPI.Name) && CoPI.IsDeleted != true
                                                                 select CoPI).ToList();
                                        int delOtherCOPICount = delOtherCOPIQuery.Count();
                                        if (delOtherCOPICount > 0)
                                        {
                                            for (int i = 0; i < delOtherCOPICount; i++)
                                            {
                                                delOtherCOPIQuery[i].IsDeleted = true;
                                                delOtherCOPIQuery[i].DeletedDate = DateTime.Now;
                                                delOtherCOPIQuery[i].DeletedUserid = model.ProposalcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    //if (model.Otherinstcopi_Qust_1 == "No")
                                    //{
                                    //    var delQuery = (from CoPI in context.tblOtherInstituteCoPI
                                    //                    where CoPI.ProposalId == proposalid && CoPI.IsDeleted != true
                                    //                    select CoPI).ToList();
                                    //    int delCount = delQuery.Count();
                                    //    if (delCount > 0)
                                    //    {
                                    //        for (int i = 0; i < delCount; i++)
                                    //        {
                                    //            delQuery[i].IsDeleted = true;
                                    //            delQuery[i].DeletedDate = DateTime.Now;
                                    //            delQuery[i].DeletedUserid = model.ProposalcrtdID;
                                    //            context.SaveChanges();
                                    //        }
                                    //    }
                                    //}

                                }

                                if (model.Otherinstcopi_Qust_1 == "No")
                                {
                                    var delOtherCOPIQuery = (from CoPI in context.tblOtherInstituteCoPI
                                                             where CoPI.ProposalId == proposalid && CoPI.IsDeleted != true
                                                             select CoPI).ToList();
                                    int delOtherCOPICount = delOtherCOPIQuery.Count();
                                    if (delOtherCOPICount > 0)
                                    {
                                        for (int i = 0; i < delOtherCOPICount; i++)
                                        {
                                            delOtherCOPIQuery[i].IsDeleted = true;
                                            delOtherCOPIQuery[i].DeletedDate = DateTime.Now;
                                            delOtherCOPIQuery[i].DeletedUserid = model.ProposalcrtdID;
                                            context.SaveChanges();
                                        }
                                    }
                                }


                                // if any document attached is removed while editing proposal change it to IsCurrentVersion false
                                var delQuery = (from doc in context.tblProposalSupportDocuments
                                                where doc.ProposalId == proposalid &&
                                                !model.Docid.Contains(doc.DocId) && doc.IsCurrentVersion == true
                                                select doc).ToList();
                                int delCount = delQuery.Count();
                                if (delCount > 0)
                                {
                                    for (int i = 0; i < delCount; i++)
                                    {
                                        delQuery[i].IsCurrentVersion = false;
                                        context.SaveChanges();
                                    }
                                }
                                // if any document attached is removed while editing proposal change it to IsCurrentVersion false in tblSupportDocuments
                                var deldocQuery = (from doc in context.tblSupportDocuments
                                                   where doc.ProposalId == proposalid &&
                                                   !model.Docid.Contains(doc.DocId) && doc.IsCurrentVersion == true
                                                   select doc).ToList();
                                int deldocCount = deldocQuery.Count();
                                if (delCount > 0)
                                {
                                    for (int i = 0; i < delCount; i++)
                                    {
                                        deldocQuery[i].IsCurrentVersion = false;
                                        context.SaveChanges();
                                    }
                                }
                                for (int i = 0; i < model.DocType.Length; i++)
                                {

                                    var docid = model.Docid[i];
                                    var query = (from doc in context.tblProposalSupportDocuments
                                                 where (doc.DocId == docid && doc.ProposalId == proposalid && doc.IsCurrentVersion == true)
                                                 select doc).ToList();
                                    // for updating the document in tblSupportDocuments check whether any document is in the table already for the proposal
                                    var docquery = (from doc in context.tblSupportDocuments
                                                    where (doc.ProposalId == proposalid && doc.IsCurrentVersion == true)
                                                    select doc).ToList();

                                    if (query.Count == 0)
                                    {
                                        if (file[i] != null)
                                        {
                                            string docpath = " ";
                                            docpath = System.IO.Path.GetFileName(file[i].FileName);
                                            var docfileId = Guid.NewGuid().ToString();
                                            var docname = docfileId + "_" + docpath;

                                            /*Saving the file in server folder*/
                                            file[i].UploadFile("SupportDocuments", docname);

                                            DataModel.tblProposalSupportDocuments Document = new DataModel.tblProposalSupportDocuments();
                                            Document.ProposalId = proposalid;
                                            Document.DocName = file[i].FileName;
                                            Document.AttachmentName = model.AttachName[i];
                                            Document.DocType = model.DocType[i];
                                            Document.AttachmentPath = docname;
                                            Document.DocUploadUserid = model.ProposalcrtdID;
                                            Document.DocUpload_TS = DateTime.Now;
                                            Document.IsCurrentVersion = true;
                                            context.tblProposalSupportDocuments.Add(Document);
                                            context.SaveChanges();

                                            tblSupportDocuments ProjectDocument = new tblSupportDocuments();
                                            ProjectDocument.ProposalId = proposalid;
                                            ProjectDocument.DocName = file[i].FileName;
                                            ProjectDocument.AttachmentName = model.AttachName[i];
                                            ProjectDocument.DocType = model.DocType[i];
                                            ProjectDocument.AttachmentPath = docname;
                                            ProjectDocument.DocUploadUserid = model.ProposalcrtdID;
                                            ProjectDocument.DocUpload_TS = DateTime.Now;
                                            ProjectDocument.IsCurrentVersion = true;
                                            context.tblSupportDocuments.Add(ProjectDocument);
                                            context.SaveChanges();
                                            //file[i].UploadFile("SupportDocuments", docname);
                                        }

                                    }
                                    else
                                    {
                                        query[0].DocType = model.DocType[i];
                                        query[0].AttachmentName = model.AttachName[i];
                                        query[0].DocUploadUserid = model.ProposalcrtdID;
                                        query[0].DocUpload_TS = DateTime.Now;
                                        query[0].IsCurrentVersion = true;
                                        context.SaveChanges();

                                        if (docquery.Count != 0)
                                        {
                                            docquery[0].DocType = model.DocType[i];
                                            docquery[0].AttachmentName = model.AttachName[i];
                                            docquery[0].DocUploadUserid = model.ProposalcrtdID;
                                            docquery[0].DocUpload_TS = DateTime.Now;
                                            docquery[0].IsCurrentVersion = true;
                                            context.SaveChanges();
                                        }
                                    }
                                }

                                transaction.Commit();
                                return proposalid;
                            }
                            else
                            {
                                return -1;
                            }
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
        public PagedData<ProposalResultModels> GetProposal(ProposalSrchFieldsModel model, int page, int pageSize)
        {
            try
            {
                List<ProposalResultModels> proposalList = new List<ProposalResultModels>();
                var searchData = new PagedData<ProposalResultModels>();

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
                    int[] status = { 1, 2, 3 };
                    var result = (from psl in context.tblProposal
                                  join user in context.vwFacultyStaffDetails on psl.PI equals user.UserId
                                  where (psl.IsDeleted != true)
                                  && (String.IsNullOrEmpty(model.srchKeyword) || psl.ProposalNumber == model.srchKeyword
                                  || psl.ProposalTitle.Contains(model.srchKeyword))
                                  orderby psl.ProposalId descending
                                  select new { psl, user.FirstName }).Skip(skiprec).Take(pageSize).ToList();
                    //var result = query.Skip(skiprec).Take(pageSize).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            int proposalId = result[i].psl.ProposalId;
                            proposalList.Add(new ProposalResultModels()
                            {
                                proposalId = proposalId,
                                nameOfPI = result[i].FirstName,
                                PIUserID = result[i].psl.PI,
                                proposalTitle = result[i].psl.ProposalTitle,
                                proposedBudget = Convert.ToDecimal(result[i].psl.ProposalValue),
                                status = result[i].psl.Status

                            });

                        }
                        var records = (from psl in context.tblProposal
                                       join user in context.vwFacultyStaffDetails on psl.PI equals user.UserId
                                       where (psl.IsDeleted != true)
                                       && (String.IsNullOrEmpty(model.srchKeyword) || psl.ProposalNumber == model.srchKeyword
                                       || psl.ProposalTitle.Contains(model.srchKeyword))
                                       orderby psl.ProposalId descending
                                       select new { psl, user.FirstName }).Count();
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));

                    }
                }
                searchData.Data = proposalList;
                searchData.visiblePages = 10;
                searchData.CurrentPage = page;
                searchData.pageSize = pageSize;
                return searchData;

            }
            catch (Exception ex)
            {
                List<ProposalResultModels> proposalList = new List<ProposalResultModels>();
                var searchData = new PagedData<ProposalResultModels>();
                searchData.Data = proposalList;
                return searchData;
            }
        }
      
        public static List<CreateProposalModel> GetProposalDetails()
        {
            List<CreateProposalModel> proposal = new List<CreateProposalModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProposal
                             join user in context.vwFacultyStaffDetails on P.PI equals user.UserId
                             //join user in context.tblUser on P.PI equals user.UserId
                             //join dept in context.tblPIDepartmentMaster on P.Department equals dept.DepartmentId
                             orderby P.ProposalId descending
                             where P.IsDeleted != true
                             select new { P, user.FirstName, user.EmployeeId, user.DepartmentName }).Take(100).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        proposal.Add(new CreateProposalModel()
                        {
                            Sno = i + 1,
                            ProposalID = query[i].P.ProposalId,
                            ProposalNumber = query[i].P.ProposalNumber,
                            Status = query[i].P.Status,
                            Projecttitle = query[i].P.ProposalTitle,
                            BasicValue = query[i].P.BasicValue,
                            NameofPI = query[i].FirstName,
                            EmpCode = query[i].EmployeeId,
                            PIDepartmentName = query[i].DepartmentName,
                            Prpsalinwrddate = String.Format("{0:s}", query[i].P.InwardDate)
                        });
                    }
                }
            }
            return proposal;
        }
        public static CreateProposalModel EditProposal(int ProposalId)
        {
            try
            {
                CreateProposalModel editProposal = new CreateProposalModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProposal
                                 where (P.ProposalId == ProposalId)
                                 select P).FirstOrDefault();
                    var CoPIquery = (from CoPI in context.tblProposalCoPI
                                     where CoPI.ProposalId == ProposalId && CoPI.isdeleted != true
                                     select CoPI).ToList();
                    var OtherInstituteCoPIquery = (from CoPI in context.tblOtherInstituteCoPI
                                                   where CoPI.ProposalId == ProposalId && CoPI.IsDeleted != true
                                                   select CoPI).ToList();
                    var SupportDocquery = (from Doc in context.tblProposalSupportDocuments
                                           where (Doc.ProposalId == ProposalId && Doc.IsCurrentVersion == true)
                                           select Doc).ToList();
                    if (query != null)
                    {
                        //int deptID = query.Department ?? 0;
                        int projecttype = query.ProjectType ?? 0;
                        editProposal.ProposalID = ProposalId;
                        editProposal.ProposalNumber = query.ProposalNumber;
                        editProposal.ProjectType = Convert.ToInt32(query.ProjectType);
                        editProposal.Prpsalinwrddate = String.Format("{0:dd}", (DateTime)query.InwardDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.InwardDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.InwardDate);
                        editProposal.Proposalinwarddate = (DateTime)query.InwardDate;
                        editProposal.ProposalSource = query.ProposalSource;
                        editProposal.Projecttitle = query.ProposalTitle;
                        editProposal.Department = query.Department;
                        editProposal.PIname = Convert.ToInt32(query.PI);
                        editProposal.PIEmail = query.PIEmail;
                        editProposal.SponsoringAgencyName = Common.GetAgencyName(query.SponsoringAgency ?? 0);
                        editProposal.SponsoringAgency = query.SponsoringAgency;
                        editProposal.Schemename = Convert.ToInt32(query.Scheme);
                        editProposal.Personapplied = query.PersonApplied;
                        editProposal.Remarks = query.Remarks;
                        editProposal.ProjectSubtype = Convert.ToInt32(query.ProjectSubType);
                        editProposal.ProjectCategory = Convert.ToInt32(query.ProjectCategory);
                        //editProposal.Inptdate = String.Format("{0:dd}", (DateTime)query.Inputdate) + "-" + String.Format("{0:MMMM}", (DateTime)query.Inputdate) + "-" + String.Format("{0:yyyy}", (DateTime)query.Inputdate);
                        //editProposal.Inputdate = (DateTime)query.Inputdate;
                        //editProposal.TotalValue = query.ProposalValue;
                        editProposal.BasicValue = query.BasicValue;
                        editProposal.ApplicableTaxes = query.ApplicableTax;
                        editProposal.TentativeCloseDate = query.TentativeCloseDate;
                        editProposal.TentativeStartDate = query.TentativeStartDate;
                        editProposal.FinYear = query.FinancialYear;
                        editProposal.SponsoredProposalFundingType = query.SponsoredProposalTypeCode;
                        editProposal.SponsoredSchemes = Common.getSchemeCodeName(query.ProjectSchemeCode ?? 0);
                       // if (query.ProposalSource == 1 || query.ProposalSource == 3)
                       // {
                            editProposal.RefNumber = query.SourceReferenceNumber;
                      //  }
                      //  if (query.ProposalSource == 2)
                       // {
                            editProposal.EmailDate = query.SourceEmailDate;
                       // }
                        //editProposal.Projectdurationyears = query.DurationOfProjectYears;
                        //editProposal.Projectdurationmonths = query.DurationOfProjectMonths;
                        //if (query.ProposalApproveddate != null)
                        //{
                        //    editProposal.PrpsalAprveddate = String.Format("{0:dd}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ProposalApproveddate);
                        //    editProposal.ProposalApproveddate = (DateTime)query.ProposalApproveddate;
                        //}
                        editProposal.PersonAppliedInstitute = query.PersonAppliedInstitute;
                        editProposal.PersonAppliedPlace = query.PersonAppliedPlace;
                        editProposal.SchemeCode = query.ProjectSchemeCode;
                        editProposal.Otherinstcopi_Qust_1 = query.Otherinstcopi_Qust_1;
                        editProposal.SanctionNumber = query.SanctionNumber;
                        editProposal.SchemeList = AccountService.getcategory(projecttype);
                        editProposal.CategoryList = AccountService.getcategorybyprojecttype(projecttype);

                        editProposal.MasterPIListDepWise = AccountService.getPIList(query.Department);
                        if (query.AI_f != true)
                            editProposal.IsthereAnyArtificialIntelligence = "No";
                        else
                            editProposal.IsthereAnyArtificialIntelligence = "Yes";

                        editProposal.Domainrelevant = query.AI_domain;
                        editProposal.AIDescription = query.AI_Description;
                    }
                    if (CoPIquery.Count > 0)
                    {
                        int[] _prposalid = new int[CoPIquery.Count];
                        int[] _copiid = new int[CoPIquery.Count];
                        string[] _copidepartment = new string[CoPIquery.Count];
                        int[] _copiname = new int[CoPIquery.Count];
                        string[] _copiemail = new string[CoPIquery.Count];
                        List<MasterlistviewModel>[] _piList = new List<MasterlistviewModel>[CoPIquery.Count];
                        for (int i = 0; i < CoPIquery.Count; i++)
                        {
                            _prposalid[i] = Convert.ToInt32(CoPIquery[i].ProposalId);
                            _copidepartment[i] = CoPIquery[i].Department;
                            _copiname[i] = Convert.ToInt32(CoPIquery[i].Name);
                            _copiid[i] = Convert.ToInt32(CoPIquery[i].CoPIId);
                            _copiemail[i] = CoPIquery[i].Email;
                            _piList[i] = AccountService.getPIList(CoPIquery[i].Department);
                        }
                        editProposal.CoPIDepartment = _copidepartment;
                        editProposal.CoPIname = _copiname;
                        editProposal.CoPIEmail = _copiemail;
                        editProposal.CoPIid = _copiid;
                        editProposal.PIListDepWise = _piList;
                    }
                    if (OtherInstituteCoPIquery.Count > 0)
                    {
                        int[] _prposalid = new int[OtherInstituteCoPIquery.Count];
                        int[] _copiid = new int[OtherInstituteCoPIquery.Count];
                        string[] _copiinstitute = new string[OtherInstituteCoPIquery.Count];
                        string[] _copidepartment = new string[OtherInstituteCoPIquery.Count];
                        string[] _copiname = new string[OtherInstituteCoPIquery.Count];
                        string[] _copiremarks = new string[OtherInstituteCoPIquery.Count];
                        for (int i = 0; i < OtherInstituteCoPIquery.Count; i++)
                        {
                            _prposalid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].ProposalId);
                            _copiinstitute[i] = OtherInstituteCoPIquery[i].Institution;
                            _copidepartment[i] = OtherInstituteCoPIquery[i].Department;
                            _copiname[i] = OtherInstituteCoPIquery[i].Name;
                            _copiid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].CoPIId);
                            _copiremarks[i] = OtherInstituteCoPIquery[i].Remarks;
                        }
                        editProposal.CoPIInstitute = _copiinstitute;
                        editProposal.OtherInstituteCoPIDepartment = _copidepartment;
                        editProposal.OtherInstituteCoPIName = _copiname;
                        editProposal.RemarksforOthrInstCoPI = _copiremarks;
                        editProposal.OtherInstituteCoPIid = _copiid;
                    }
                    if (SupportDocquery.Count > 0)
                    {
                        int[] _docid = new int[SupportDocquery.Count];
                        int[] _doctype = new int[SupportDocquery.Count];
                        string[] _docname = new string[SupportDocquery.Count];
                        string[] _attachname = new string[SupportDocquery.Count];
                        string[] _docpath = new string[SupportDocquery.Count];
                        for (int i = 0; i < SupportDocquery.Count; i++)
                        {
                            _docid[i] = Convert.ToInt32(SupportDocquery[i].DocId);
                            _doctype[i] = Convert.ToInt32(SupportDocquery[i].DocType);
                            _docname[i] = SupportDocquery[i].DocName;
                            _docpath[i] = SupportDocquery[i].AttachmentPath;
                            _attachname[i] = SupportDocquery[i].AttachmentName;
                        }
                        editProposal.DocType = _doctype;
                        editProposal.AttachName = _attachname;
                        editProposal.DocName = _docname;
                        editProposal.DocPath = _docpath;
                        editProposal.Docid = _docid;
                    }
                    return editProposal;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int DeleteProposal(int ProposalId, int logged_in_userId)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblProposal
                                 where D.ProposalId == ProposalId
                                 select D).FirstOrDefault();
                    if (query != null)
                    {
                        query.IsDeleted = true;
                        query.Updt_TS = DateTime.Now;
                        query.Updt_Userid = logged_in_userId;
                        context.SaveChanges();
                    }
                    context.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static PagedData<CreateProposalModel> SearchProposalList(int? ProjectType, string ProposalNumber, string PslNumber, int? PIname, DateTime? Fromdate, DateTime? Todate, string FunctionStatus, string Projecttitle, string NameofPI, string EmpCode, DateFilterModel inwardDate, int page,int pageSize)
        {
            try
            {
                var proposal = new PagedData<CreateProposalModel>();
                List<CreateProposalModel> list = new List<CreateProposalModel>();
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
                    var prequery = from p in context.tblProposal
                                join us in context.vwFacultyStaffDetails on p.PI equals us.UserId
                                select new ProposalPredicate
                                {
                                    psl = p,
                                    u = us
                                };
                    var predicate = PredicateBuilder.BaseAnd<ProposalPredicate>();
                    predicate = predicate.And(d => d.psl.IsDeleted != true);
                    if (!string.IsNullOrEmpty(PslNumber))
                        predicate = predicate.And(d => d.psl.ProposalNumber.Contains(PslNumber));
                    if (ProjectType != null)
                        predicate = predicate.And(d => d.psl.ProjectType == ProjectType);
                    if (PIname != null)
                        predicate = predicate.And(d => d.psl.PI == PIname);
                    if (Fromdate != null && Todate != null)
                    {
                        Todate = Todate.Value.Date.AddDays(1).AddTicks(-2);
                        predicate = predicate.And(d => d.psl.Crtd_TS >= Fromdate && d.psl.Crtd_TS <= Todate);
                    }
                    if (!string.IsNullOrEmpty(FunctionStatus))
                        predicate = predicate.And(d => d.psl.Status == FunctionStatus);

                    if (!string.IsNullOrEmpty(ProposalNumber))
                        predicate = predicate.And(d => d.psl.ProposalNumber.Contains(ProposalNumber));

                    if (!string.IsNullOrEmpty(Projecttitle))
                        predicate = predicate.And(d => d.psl.ProposalTitle.Contains(Projecttitle));

                    if (!string.IsNullOrEmpty(NameofPI))
                        predicate = predicate.And(d => d.u.FirstName.Contains(NameofPI));

                    if (!string.IsNullOrEmpty(EmpCode))
                        predicate = predicate.And(d => d.u.EmployeeId.Contains(EmpCode));

                    if (inwardDate.from != null && inwardDate.to != null)
                    {
                        inwardDate.to = inwardDate.to.Value.Date.AddDays(1).AddTicks(-2);

                        predicate = predicate.And(d => d.psl.InwardDate >= inwardDate.from && d.psl.InwardDate <= inwardDate.to);
                    }

                    var query = prequery.Where(predicate).OrderByDescending(m => m.psl.ProposalId).Skip(skiprec).Take(pageSize).ToList();
                    proposal.TotalRecords = prequery.Where(predicate).Count();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new CreateProposalModel()
                                {
                                    Sno = i + 1,
                                    ProposalID = query[i].psl.ProposalId,
                                    ProposalNumber = query[i].psl.ProposalNumber,
                                    Projecttitle = query[i].psl.ProposalTitle,
                                    BasicValue = query[i].psl.BasicValue,
                                    NameofPI = query[i].u.FirstName,
                                    EmpCode = query[i].u.EmployeeId,
                                    PIDepartmentName = query[i].u.DepartmentName,
                                    Prpsalinwrddate = String.Format("{0:s}", query[i].psl.InwardDate),
                                    Status = query[i].psl.Status
                                });
                        }
                    }
                }
                proposal.Data = list;
                return proposal;
            }
            catch (Exception ex)
            {
                return new PagedData<CreateProposalModel>();
            }
        }
        public bool ProposalWFInit(int proposalId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblProposal.FirstOrDefault(m => m.ProposalId == proposalId && m.Status == "Open");
                    if (query != null)
                    {
                        
                        var fw = FlowEngine.Init(6, logged_in_user, proposalId, "ProposalId");
                        string url = "/Proposal/ViewProposal?ProposalId=" + proposalId + "&Pfinit=true";
                        fw.FunctionId(9);
                        fw.ReferenceNumber(query.ProposalNumber);
                        fw.ActionLink(url);
                        fw.FailedMethod("ProposalWFInitFailure");
                        fw.ClarifyMethod("ProposalWFInitClarify");
                        fw.SuccessMethod("ProposalWFInitSuccess");
                        fw.ProcessInit();

                        if (String.IsNullOrEmpty(fw.errorMsg))
                        {
                            
                            query.Status = "Submit for approval";
                            query.Updt_Userid = logged_in_user;
                            query.Updt_TS = DateTime.Now;
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
        public static ViewProposalModel ViewProposal(int ProposalId)
        {
            ViewProposalModel viewProposal = new ViewProposalModel();
            List<CoInvestigatorModel> listcopi = new List<CoInvestigatorModel>();
            List<CoInvestigatorOtherModel> othercopi = new List<CoInvestigatorOtherModel>();
            List<ProposalDocument> proposaldoc = new List<ProposalDocument>();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProposal
                                 where (P.ProposalId == ProposalId)
                                 select P).FirstOrDefault();
                    var CoPIquery = (from CoPI in context.tblProposalCoPI
                                     where CoPI.ProposalId == ProposalId && CoPI.isdeleted != true
                                     select CoPI).ToList();
                    var OtherInstituteCoPIquery = (from CoPI in context.tblOtherInstituteCoPI
                                                   where CoPI.ProposalId == ProposalId && CoPI.IsDeleted != true
                                                   select CoPI).ToList();
                    var SupportDocquery = (from Doc in context.tblProposalSupportDocuments
                                           from DN in context.tblDocument
                                           where (Doc.ProposalId == ProposalId && Doc.IsCurrentVersion == true)
                                           && DN.DocumentId == Doc.DocType
                                           select new { Doc, DN }).ToList();
                    if (query != null)
                    {
                        int agyid = query.SponsoringAgency ?? 0;
                        var agencydetails = context.tblAgencyMaster.Where(x => x.AgencyId == agyid).FirstOrDefault();
                        //int deptID = query.Department ?? 0;
                        int projecttype = query.ProjectType ?? 0;
                        viewProposal.ProposalID = ProposalId;
                        viewProposal.ProposalNumber = query.ProposalNumber;
                        viewProposal.ProjectType = Convert.ToInt32(query.ProjectType);
                        viewProposal.ProjectTypeName = Common.GetCodeControlnameCommon(query.ProjectType ?? 0, "Projecttype");
                        if (query.InwardDate != null)
                            viewProposal.Prpsalinwrddate = String.Format("{0:dd}", (DateTime)query.InwardDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.InwardDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.InwardDate);
                        viewProposal.Proposalinwarddate = (DateTime)query.InwardDate;
                        viewProposal.ProposalSource = query.ProposalSource;
                        viewProposal.SourceName = Common.GetCodeControlnameCommon(query.ProposalSource ?? 0, "Source");
                        viewProposal.Projecttitle = query.ProposalTitle;
                        if (query.AI_f != true)
                            viewProposal.IsthereAnyArtificialIntelligence = "No";
                        else
                            viewProposal.IsthereAnyArtificialIntelligence = "Yes";
                        viewProposal.AIDescription = query.AI_Description;
                        viewProposal.Domainrelevant = query.AI_domain;


                        viewProposal.PIname = Convert.ToInt32(query.PI);
                        viewProposal.PIName = Common.GetPIName(query.PI ?? 0);
                        string pidept = context.tblFacultyDetail.Where(x => x.CastEmployeeId == viewProposal.PIname).Select(x => x.DepartmentName).FirstOrDefault();
                        viewProposal.Department = pidept;
                        viewProposal.PIEmail = query.PIEmail;
                        viewProposal.SponsoringAgencyName = Common.GetAgencyName(query.SponsoringAgency ?? 0);
                        viewProposal.SponsoringAgency = query.SponsoringAgency;
                        viewProposal.AgencyName = agencydetails.AgencyName;
                        viewProposal.Schemename = Convert.ToInt32(query.Scheme);
                        viewProposal.SchemeCodeName = Common.getSchemeName(query.Scheme ?? 0);
                        viewProposal.Personapplied = query.PersonApplied;
                        viewProposal.Remarks = query.Remarks;
                        viewProposal.ProjectSubtype = Convert.ToInt32(query.ProjectSubType);
                        viewProposal.ProjectSubtypeName = Common.GetCodeControlnameCommon(query.ProjectSubType ?? 0, "ProjectSubtype");
                        viewProposal.ProjectCategory = Convert.ToInt32(query.ProjectCategory);
                        if (query.ProjectType == 1 && query.ProjectSubType == 2)
                            viewProposal.ProjectCategoryName = Common.GetCodeControlnameCommon(query.ProjectCategory ?? 0, "SponProjectCategory");
                        if (query.ProjectType == 2)
                            viewProposal.ProjectCategoryName = Common.GetCodeControlnameCommon(query.ProjectCategory ?? 0, "ConsultancyProjectSubtype");
                        //editProposal.Inptdate = String.Format("{0:dd}", (DateTime)query.Inputdate) + "-" + String.Format("{0:MMMM}", (DateTime)query.Inputdate) + "-" + String.Format("{0:yyyy}", (DateTime)query.Inputdate);
                        //editProposal.Inputdate = (DateTime)query.Inputdate;
                        //editProposal.TotalValue = query.ProposalValue;
                        viewProposal.BasicValue = query.BasicValue;
                        viewProposal.ApplicableTaxes = query.ApplicableTax;
                        viewProposal.TentativeCloseDate = query.TentativeCloseDate;
                        if (query.TentativeCloseDate != null)
                            viewProposal.CloseDate = String.Format("{0:dd}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeCloseDate);
                        viewProposal.TentativeStartDate = query.TentativeStartDate;
                        if (query.TentativeStartDate != null)
                            viewProposal.StartDate = String.Format("{0:dd}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeStartDate);
                        viewProposal.FinYear = query.FinancialYear;
                        viewProposal.FinYearType = Common.GetFinYear(query.FinancialYear ?? 0);
                        //viewProposal.ConsultancyType = Common.getSchemeName(query.Scheme ?? 0);
                        viewProposal.ConsultancyType = Common.GetConsultancyFundingType(query.Scheme ?? 0);
//if (query.ProposalSource == 1 || query.ProposalSource == 3)
                       // {
                            viewProposal.RefNumber = query.SourceReferenceNumber;
                        //}
                        //if (query.ProposalSource == 2)
                       // {
                            viewProposal.EmailDate = query.SourceEmailDate;
                        if (query.SourceEmailDate != null)
                            viewProposal.SourceEmailDate = String.Format("{0:dd}", (DateTime)query.SourceEmailDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.SourceEmailDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.SourceEmailDate);
                        //}
                        //editProposal.Projectdurationyears = query.DurationOfProjectYears;
                        //editProposal.Projectdurationmonths = query.DurationOfProjectMonths;
                        //if (query.ProposalApproveddate != null)
                        //{
                        //    editProposal.PrpsalAprveddate = String.Format("{0:dd}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ProposalApproveddate);
                        //    editProposal.ProposalApproveddate = (DateTime)query.ProposalApproveddate;
                        //}
                        viewProposal.PersonAppliedInstitute = query.PersonAppliedInstitute;
                        viewProposal.PersonAppliedPlace = query.PersonAppliedPlace;
                        viewProposal.SchemeCode = query.ProjectSchemeCode;
                        viewProposal.Otherinstcopi_Qust_1 = query.Otherinstcopi_Qust_1;
                        viewProposal.SanctionNumber = query.SanctionNumber;
                        viewProposal.SponsoredSchemes = Common.getSchemeCodeName(query.ProjectSchemeCode ?? 0);
                        viewProposal.SchemeList = AccountService.getcategory(projecttype);
                        viewProposal.CategoryList = AccountService.getcategorybyprojecttype(projecttype);
                        viewProposal.SponsoredTypeCode = Common.GetSponsoredTypeName(query.SponsoredProposalTypeCode ?? 0);
                        viewProposal.MasterPIListDepWise = AccountService.getPIList(query.Department);

                    }
                    if (CoPIquery.Count > 0)
                    {

                        //int[] _prposalid = new int[CoPIquery.Count];
                        //int[] _copiid = new int[CoPIquery.Count];
                        //string[] _copidepartment = new string[CoPIquery.Count];
                        //int[] _copiname = new int[CoPIquery.Count];
                        //string[] _copiemail = new string[CoPIquery.Count];
                        //List<MasterlistviewModel>[] _piList = new List<MasterlistviewModel>[CoPIquery.Count];
                        for (int i = 0; i < CoPIquery.Count; i++)
                        {
                            int copiid = CoPIquery[i].Name;
                            var pidepts = context.tblFacultyDetail.Where(x => x.CastEmployeeId == copiid).Select(x => x.DepartmentName).FirstOrDefault();
                            var copiname = Common.GetPIName(CoPIquery[i].Name);
                            listcopi.Add(new CoInvestigatorModel()
                            {
                                COPIName = copiname,
                                DepartMentOfCOPI = pidepts,
                                COPIEmailId = CoPIquery[i].Email
                            });
                            //    _prposalid[i] = Convert.ToInt32(CoPIquery[i].ProposalId);
                            //    _copidepartment[i] = CoPIquery[i].Department;
                            //    _copiname[i] = Convert.ToInt32(CoPIquery[i].Name);
                            //    _copiid[i] = Convert.ToInt32(CoPIquery[i].CoPIId);
                            //    _copiemail[i] = CoPIquery[i].Email;
                            //    _piList[i] = AccountService.getPIList(CoPIquery[i].Department);
                            //}
                            //editProposal.CoPIDepartment = _copidepartment;
                            //editProposal.CoPIname = _copiname;
                            //editProposal.CoPIEmail = _copiemail;
                            //editProposal.CoPIid = _copiid;
                            //editProposal.PIListDepWise = _piList;
                        }
                    }
                    if (OtherInstituteCoPIquery.Count > 0)
                    {
                        //int[] _prposalid = new int[OtherInstituteCoPIquery.Count];
                        //int[] _copiid = new int[OtherInstituteCoPIquery.Count];
                        //string[] _copiinstitute = new string[OtherInstituteCoPIquery.Count];
                        //string[] _copidepartment = new string[OtherInstituteCoPIquery.Count];
                        //string[] _copiname = new string[OtherInstituteCoPIquery.Count];
                        //string[] _copiremarks = new string[OtherInstituteCoPIquery.Count];
                        for (int i = 0; i < OtherInstituteCoPIquery.Count; i++)
                        {
                            othercopi.Add(new CoInvestigatorOtherModel()
                            {
                                InstituteName = OtherInstituteCoPIquery[i].Institution,
                                InstituteCOPIDEP = OtherInstituteCoPIquery[i].Department,
                                NameOfCOPI = OtherInstituteCoPIquery[i].Name,
                                Remarks = OtherInstituteCoPIquery[i].Remarks
                            });
                            //_prposalid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].ProposalId);
                            //_copiinstitute[i] = OtherInstituteCoPIquery[i].Institution;
                            //_copidepartment[i] = OtherInstituteCoPIquery[i].Department;
                            //_copiname[i] = OtherInstituteCoPIquery[i].Name;
                            //_copiid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].CoPIId);
                            //_copiremarks[i] = OtherInstituteCoPIquery[i].Remarks;
                            //}
                            //editProposal.CoPIInstitute = _copiinstitute;
                            //editProposal.OtherInstituteCoPIDepartment = _copidepartment;
                            //editProposal.OtherInstituteCoPIName = _copiname;
                            //editProposal.RemarksforOthrInstCoPI = _copiremarks;
                            //editProposal.OtherInstituteCoPIid = _copiid;
                        }
                    }
                    if (SupportDocquery.Count > 0)
                    {
                        int[] _docid = new int[SupportDocquery.Count];
                        int[] _doctype = new int[SupportDocquery.Count];
                        string[] _docname = new string[SupportDocquery.Count];
                        string[] _attachname = new string[SupportDocquery.Count];
                        string[] _docpath = new string[SupportDocquery.Count];
                        for (int i = 0; i < SupportDocquery.Count; i++)
                        {
                            proposaldoc.Add(new ProposalDocument()
                            {
                                AttachmentTypeName = SupportDocquery[i].DN.DocumentName,
                                AttachmentName = SupportDocquery[i].Doc.AttachmentName,
                                AttachmentFileName = SupportDocquery[i].Doc.DocName,
                                AttachmentFilePath = SupportDocquery[i].Doc.AttachmentPath
                            });
                            //    _docid[i] = Convert.ToInt32(SupportDocquery[i].DocId);
                            //    _doctype[i] = Convert.ToInt32(SupportDocquery[i].DocType);
                            //    _docname[i] = SupportDocquery[i].DocName;
                            //    _docpath[i] = SupportDocquery[i].AttachmentPath;
                            //    _attachname[i] = SupportDocquery[i].AttachmentName;
                            //}
                            //editProposal.DocType = _doctype;
                            //editProposal.AttachName = _attachname;
                            //editProposal.DocName = _docname;
                            //editProposal.DocPath = _docpath;
                            //editProposal.Docid = _docid;
                        }

                    }
                }
                viewProposal.CopiList = listcopi;
                viewProposal.OtherCOPIList = othercopi;
                viewProposal.DocumentList = proposaldoc;
                return viewProposal;
            }
            catch (Exception ex)
            {
                return viewProposal;
            }
        }

        //WorkFlow Proposal
        public static CreateProposalModel EditWFProposal(int WFproposalid)
        {
            try
            {
                CreateProposalModel editProposal = new CreateProposalModel();
                using (var context = new IOASWorkFlowEntities1())
                {
                    var query = (from P in context.tblProposal
                                 where (P.ProposalId == WFproposalid)
                                 select P).FirstOrDefault();
                    var CoPIquery = (from CoPI in context.tblProposalCoPI
                                     where CoPI.ProposalId == WFproposalid && CoPI.isdeleted != true
                                     select CoPI).ToList();
                    var OtherInstituteCoPIquery = (from CoPI in context.tblOtherInstituteCoPI
                                                   where CoPI.ProposalId == WFproposalid && CoPI.IsDeleted != true
                                                   select CoPI).ToList();
                    var SupportDocquery = (from Doc in context.tblProposalSupportDocuments
                                           where (Doc.ProposalId == WFproposalid )
                                           select Doc).ToList();
                    if (query != null)
                    {
                        //int deptID = query.Department ?? 0;
                        int projecttype = query.ProjectType ?? 0;
                        //editProposal.ProposalID = ProposalId;
                        editProposal.ProposalNumber = query.ProposalNumber;
                        editProposal.ProjectType = Convert.ToInt32(query.ProjectType);
                        if (query.InwardDate != null)
                        {
                            editProposal.Prpsalinwrddate = String.Format("{0:dd}", (DateTime)query.InwardDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.InwardDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.InwardDate);
                            editProposal.Proposalinwarddate = (DateTime)query.InwardDate;
                        }

                        editProposal.ProposalSource = query.ProposalSource;
                        editProposal.Projecttitle = query.ProposalTitle;
                       
                        if (query.PI != null)
                            editProposal.PIname = Convert.ToInt32(query.PI);
                        int Pi = Convert.ToInt32(query.PI);
                        editProposal.Department = Common.GetDepartmentCode(Pi);
                        editProposal.PIEmail = query.PIEmail;
                        if (query.SponsoringAgency != null)
                            editProposal.SponsoringAgencyName = Common.GetAgencyName(query.SponsoringAgency ?? 0);
                        editProposal.SponsoringAgency = query.SponsoringAgency;
                        if (query.Scheme != null)
                            editProposal.Schemename = Convert.ToInt32(query.Scheme);
                        editProposal.Personapplied = query.PersonApplied;
                        editProposal.Remarks = query.Remarks;
                        if (query.ProjectSubType != null)
                            editProposal.ProjectSubtype = Convert.ToInt32(query.ProjectSubType);
                        if (query.ProjectCategory != null)
                            editProposal.ProjectCategory = Convert.ToInt32(query.ProjectCategory);
                        //editProposal.Inptdate = String.Format("{0:dd}", (DateTime)query.Inputdate) + "-" + String.Format("{0:MMMM}", (DateTime)query.Inputdate) + "-" + String.Format("{0:yyyy}", (DateTime)query.Inputdate);
                        //editProposal.Inputdate = (DateTime)query.Inputdate;
                        //editProposal.TotalValue = query.ProposalValue;
                        editProposal.BasicValue = query.BasicValue;
                        editProposal.ApplicableTaxes = query.ApplicableTax;
                        editProposal.TentativeCloseDate = query.TentativeCloseDate;
                        editProposal.TentativeStartDate = query.TentativeStartDate;
                        if (query.FinancialYear != null)
                            editProposal.FinYear = Common.GetFinancialYearId(Convert.ToString(query.FinancialYear));
                        editProposal.WFproposalid = WFproposalid;
                      //  if (query.ProposalSource == 1 || query.ProposalSource == 3)
                       // {
                            editProposal.RefNumber = WFproposalid;
                      //  }
                      //  if (query.ProposalSource == 2)
                      //  {
                            editProposal.EmailDate = query.SourceEmailDate;
                      //  }
                        //editProposal.Projectdurationyears = query.DurationOfProjectYears;
                        //editProposal.Projectdurationmonths = query.DurationOfProjectMonths;
                        //if (query.ProposalApproveddate != null)
                        //{
                        //    editProposal.PrpsalAprveddate = String.Format("{0:dd}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ProposalApproveddate);
                        //    editProposal.ProposalApproveddate = (DateTime)query.ProposalApproveddate;
                        //}
                        editProposal.PersonAppliedInstitute = query.PersonAppliedInstitute;
                        editProposal.PersonAppliedPlace = query.PersonAppliedPlace;
                        editProposal.SchemeCode = query.ProjectSchemeCode;
                       // editProposal.Otherinstcopi_Qust_1 = query.Otherinstcopi_Qust_1;
                        editProposal.SanctionNumber = query.SanctionNumber;

                        if (query.AI_f != true)
                            editProposal.IsthereAnyArtificialIntelligence = "No";
                        else
                            editProposal.IsthereAnyArtificialIntelligence = "Yes";

                        editProposal.Domainrelevant = query.AI_domain;
                        editProposal.AIDescription = query.AI_Description;
                        editProposal.SchemeList = AccountService.getcategory(projecttype);
                        editProposal.CategoryList = AccountService.getcategorybyprojecttype(projecttype);


                        if (query.Department != null)
                            editProposal.MasterPIListDepWise = AccountService.getPIList(query.Department);

                    }
                    if (CoPIquery.Count > 0)
                    {
                        int[] _prposalid = new int[CoPIquery.Count];
                        int[] _copiid = new int[CoPIquery.Count];
                        string[] _copidepartment = new string[CoPIquery.Count];
                        int[] _copiname = new int[CoPIquery.Count];
                        string[] _copiemail = new string[CoPIquery.Count];
                        List<MasterlistviewModel>[] _piList = new List<MasterlistviewModel>[CoPIquery.Count];
                        for (int i = 0; i < CoPIquery.Count; i++)
                        {
                            _prposalid[i] = Convert.ToInt32(CoPIquery[i].ProposalId);
                           int CoPI= Convert.ToInt32(CoPIquery[i].Name);
                            _copidepartment[i] = Common.GetDepartmentCode(CoPI); ;
                            _copiname[i] = Convert.ToInt32(CoPIquery[i].Name);
                            _copiid[i] = Convert.ToInt32(CoPIquery[i].CoPIId);
                            _copiemail[i] = CoPIquery[i].Email;
                            _piList[i] = AccountService.getPIList(CoPIquery[i].Department);
                        }
                        editProposal.CoPIDepartment = _copidepartment;
                        editProposal.CoPIname = _copiname;
                        editProposal.CoPIEmail = _copiemail;
                        editProposal.CoPIid = _copiid;
                        editProposal.PIListDepWise = _piList;
                    }
                    if (OtherInstituteCoPIquery.Count > 0)
                    {
                        editProposal.Otherinstcopi_Qust_1 = "Yes";
                        int[] _prposalid = new int[OtherInstituteCoPIquery.Count];
                        int[] _copiid = new int[OtherInstituteCoPIquery.Count];
                        string[] _copiinstitute = new string[OtherInstituteCoPIquery.Count];
                        string[] _copidepartment = new string[OtherInstituteCoPIquery.Count];
                        string[] _copiname = new string[OtherInstituteCoPIquery.Count];
                        string[] _copiremarks = new string[OtherInstituteCoPIquery.Count];
                        for (int i = 0; i < OtherInstituteCoPIquery.Count; i++)
                        {
                            _prposalid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].ProposalId);
                            _copiinstitute[i] = OtherInstituteCoPIquery[i].Institution;
                            _copidepartment[i] = OtherInstituteCoPIquery[i].Department;
                            _copiname[i] = OtherInstituteCoPIquery[i].Name;
                            _copiid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].CoPIId);
                            _copiremarks[i] = OtherInstituteCoPIquery[i].Remarks;
                        }
                        editProposal.CoPIInstitute = _copiinstitute;
                        editProposal.OtherInstituteCoPIDepartment = _copidepartment;
                        editProposal.OtherInstituteCoPIName = _copiname;
                        editProposal.RemarksforOthrInstCoPI = _copiremarks;
                        editProposal.OtherInstituteCoPIid = _copiid;
                    }
                    if (SupportDocquery.Count > 0)
                    {
                        int[] _docid = new int[SupportDocquery.Count];
                        int[] _doctype = new int[SupportDocquery.Count];
                        string[] _docname = new string[SupportDocquery.Count];
                        string[] _attachname = new string[SupportDocquery.Count];
                        string[] _docpath = new string[SupportDocquery.Count];
                        int[] _WfDocId = new int[SupportDocquery.Count];
                       
                         for (int i = 0; i < SupportDocquery.Count; i++)
                        {
                            _docid[i] = Convert.ToInt32(SupportDocquery[i].DocId);
                            _doctype[i] = Convert.ToInt32(SupportDocquery[i].DocType);
                            _docname[i] = SupportDocquery[i].DocName;
                            _docpath[i] = SupportDocquery[i].DocName;
                            _attachname[i] = "WorkFlow Proposal";
                            
                        }
                        editProposal.DocType = _doctype;
                        editProposal.AttachName = _attachname;
                        editProposal.DocName = _docname;
                        editProposal.DocPath = _docpath;
                        editProposal.Docid = _docid;
                    }
                    return editProposal;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       

    }
}