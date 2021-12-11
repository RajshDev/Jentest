using Foolproof;
using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Models
{
    public class ProposalModel
    {
        public ProposalSrchFieldsModel srchField { get; set; }
        public PagedData<ProposalResultModels> resultField { get; set; }
    }
    public class ProposalSrchFieldsModel
    {
        public string srchKeyword { get; set; }
        public Nullable<int> selMinistry { get; set; }
        public Nullable<int> Institute { get; set; }
        public Nullable<int> Industry { get; set; }
        public Nullable<int> Proposalstatus { get; set; }
    }
    public class ProposalResultModels
    {
        public int proposalId { get; set; }
        public string proposalTitle { get; set; }
        public Nullable<int> ProposalType { get; set; }
        public string nameOfPI { get; set; }
        public string instituteOfPI { get; set; }
        public string DEC_Domain { get; set; }
        public string DEC_Group { get; set; }
        public string Doc_Name { get; set; }
        public string ProsNumber { get; set; }
        public string MHRD { get; set; }
        public string Ministry { get; set; }
        public string Industry { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal proposedBudget { get; set; }
        public string status { get; set; }
        public string statusString { get; set; }
        public bool isReviewAllocated { get; set; }
        public bool isReviewed { get; set; }

        public Nullable<int> PIUserID { get; set; }
    }
    public class ProposalPredicate
    {
        public tblProposal psl { get; set; }
        public vwFacultyStaffDetails u { get; set; }
    }
    public class CreateProposalModel
    {
        public string SponsoredSchemes { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public int Sno { get; set; }
        public string Projecttitle { get; set; }
        [Required]
        [Display(Name = "PI Name")]
        public Nullable<int> PIname { get; set; }
        public int ProjectType { get; set; }
        [Required]
        [Display(Name = "Financial Year")]
        public Nullable<int> FinYear { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.ProposalSource == 1 || this.ProposalSource == 3);
            }
        }
      //  [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
       // [Display(Name = "Ref. Number")]
        public Nullable<int> RefNumber { get; set; }

        // [RequiredIf("ProposalSource", 2, ErrorMessage = "Email Date field is required")]
        [Required]
        [Display(Name = "Email Date")]
        public Nullable<DateTime> EmailDate { get; set; }
        public Nullable<int> ProjectCategory { get; set; }
        [RequiredIf("ProjectType", "1", ErrorMessage = "Project Sub Type field is required")]
        public Nullable<int> ProjectSubtype { get; set; }
        public string NameofPI { get; set; }
        public string EmpCode { get; set; }
        [Required]
        [Display(Name = "Agency")]
        public Nullable<int> SponsoringAgency { get; set; }
        public int ProposalcrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BasicValue { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 99.99, ErrorMessage = "Applicable Tax should not be greater than 100 percentage")]
        public Nullable<Decimal> ApplicableTaxes { get; set; }
        public int[] Docid { get; set; }
        public string[] DocPath { get; set; }
        public int[] DocType { get; set; }
        public string[] DocName { get; set; }
        public string[] AttachName { get; set; }
      
        // public HttpPostedFileBase[] file { get; set; }
        public List<UploadDocumentDetailsList> Upload { get; set; }

        public Nullable<int> ProposalSource { get; set; }
        [Required]
        public string Department { get; set; }
        public string PIDepartmentName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Proposalinwarddate { get; set; }
        public string Prpsalinwrddate { get; set; }
        public string PIEmail { get; set; }
        public string workphone { get; set; }
        public int[] CoPIname { get; set; }
        public int[] CoPIid { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string[] CoPIEmail { get; set; }
        public string[] CoPIDepartment { get; set; }
        public List<MasterlistviewModel>[] PIListDepWise { get; set; }
        public List<MasterlistviewModel>[] OtherInstCoPIListDepWise { get; set; }
        public List<MasterlistviewModel> MasterPIListDepWise { get; set; }
        [RequiredIf("ProjectSubtype", 2, ErrorMessage = "Project Scheme field is required")]
        public int? Schemename { get; set; }
        [RequiredIf("ProjectType", "2", ErrorMessage = "Consultancy Type field is required")]
        public Nullable<int> Constype { get; set; }
        public string Personapplied { get; set; }
        public string Remarks { get; set; }
        public ProposalSearchFieldModel SearchField { get; set; }
        public PagedData<ProposalSearchResultModel> SearchResult { get; set; }
        public string[] CoPIInstitute { get; set; }
        public string[] OtherInstituteCoPIDepartment { get; set; }
        public string[] OtherInstituteCoPIName { get; set; }
        public int[] OtherInstituteCoPIid { get; set; }
        public string[] RemarksforOthrInstCoPI { get; set; }
        public string Otherinstcopi_Qust_1 { get; set; }
        public string IsthereAnyArtificialIntelligence { get; set; }
        [RequiredIf("IsthereAnyArtificialIntelligence", "Yes", ErrorMessage = "AI Description Required")]
        [StringLength(maximumLength: 200)]
        public string AIDescription { get; set; }
        //[RequiredIf("IsthereAnyArtificialIntelligence", "Yes", ErrorMessage = "domain relevant Required")]
        [Required]
        [Display(Name = "Domain relevant")]
        public string Domainrelevant { get; set; }
        [Required]
        [Display(Name = "Tentative Start Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeStartDate { get; set; }
        [Required]
        [Display(Name = "Tentative Close Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeCloseDate { get; set; }
        public string PersonAppliedInstitute { get; set; }
        public string PersonAppliedPlace { get; set; }
        public bool PFMSSchemeCode
        {
            get
            {
                return (this.ProjectSubtype == 2 && this.ProjectCategory == 1);
            }
        }
        //[RequiredIf("PFMSSchemeCode", true, ErrorMessage = "Scheme Code field is required")]
        public Nullable<int> SchemeCode { get; set; }
        public string Constypecode { get; set; }
        public string SanctionNumber { get; set; }
        public List<MasterlistviewModel> SchemeList { get; set; }
        public List<CodeControllistviewModel> CategoryList { get; set; }
        public string Status { get; set; }
        public string FilterStatus { get; set; }
        public string SponsoringAgencyName { get; set; }
        public int WFproposalid { get; set; }
        [RequiredIf("ProjectType", "1", ErrorMessage = "Sponsored Funding Type")]
        public Nullable<int> SponsoredProposalFundingType { get; set; }
    }

    public class ProposalSearchResultModel
    {
        public Nullable<int> Proposald { get; set; }
        public Nullable<int> ProposalType { get; set; }
       
    }

    public class ProposalSearchFieldModel
    {

        public Nullable<int> ProposalType { get; set; }
        public Nullable<int> PIName { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string FunctionStatus { get; set; }
    }
    public class UploadDocumentDetailsList
    {
        public int ProposalID { get; set; }
        public string DocType { get; set; }
        public string DocName { get; set; }
        public string DocDes { get; set; }
        public string linkUload { get; set; }

    }
    public class ReviewSubmitModel
    {
        public Nullable<Int32> pId { get; set; }
        [Required(ErrorMessage = "Comments field is required")]
        [AllowHtml]
        public string comments { get; set; }
        [Required(ErrorMessage = "Document type field is required")]
        public HttpPostedFileBase reviewDoc { get; set; }
        [Required(ErrorMessage = "Upload document field is required")]
        public Nullable<Int32> docType { get; set; }
    }
    public class EditProposalModel
    {
        public int ProposalID { get; set; }
        public string Projecttitle { get; set; }
        [Required]
        [Display(Name = "Type of Project")]
        public Nullable<int> selProjectType { get; set; }
        public string ProjectType { get; set; }
        public int PIname { get; set; }
        public string NameofPI { get; set; }
        public string ProposalPhase { get; set; }
        public int ProposalupdtID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Budget { get; set; }
        [StringLength(25)]
        public string Mobile { get; set; }
        public string[] DocPath { get; set; }
        public Nullable<int>[] DocType { get; set; }
        public string[] DocDescrip { get; set; }
        public string[] DocName { get; set; }
        public string[] DocCrtdUserid { get; set; }
        public Nullable<DateTime>[] DocCrtd_TS { get; set; }

        public HttpPostedFileBase[] file { get; set; }
        public List<UploadDocumentDetailsList> Upload { get; set; }
        public Nullable<int> seltypeIndstryname { get; set; }
        [Display(Name = "Ministry")]
        public Nullable<int> selMinistry { get; set; }
        public Nullable<int> selDomain { get; set; }
        public string MHRD { get; set; }
        [Required]
        public Nullable<int> Department { get; set; }
        public string Departmentname { get; set; }
        [Required]
        public Nullable<Int32> Institute { get; set; }
        public string Institutename { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Startdate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Enddate { get; set; }
        public string DocPathname { get; set; }
        public Nullable<int> DocTypename { get; set; }
        public string DocDescripname { get; set; }

    }

    public class ViewProposalModel
    {
        public Nullable<int> ProposalID { get; set; }
        public int Sno { get; set; }
        public string Projecttitle { get; set; }
        [Required]
        public int PIname { get; set; }
        public int ProjectType { get; set; }
        [Required]
        [Display(Name = "Financial Year")]
        public Nullable<int> FinYear { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.ProposalSource == 1 || this.ProposalSource == 3);
            }
        }
        //[RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        //[Display(Name = "Ref. Number")]
        public Nullable<int> RefNumber { get; set; }

        [RequiredIf("ProposalSource", 2, ErrorMessage = "Email Date field is required")]
        [Display(Name = "Email Date")]
        public Nullable<DateTime> EmailDate { get; set; }
        public Nullable<int> ProjectCategory { get; set; }
        [RequiredIf("ProjectType", "1", ErrorMessage = "Project Sub Type field is required")]
        public Nullable<int> ProjectSubtype { get; set; }
        public string NameofPI { get; set; }
        public string EmpCode { get; set; }
        public Nullable<int> SponsoringAgency { get; set; }
        public int ProposalcrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BasicValue { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 99.99, ErrorMessage = "Applicable Tax should not be greater than 100 percentage")]
        public Nullable<Decimal> ApplicableTaxes { get; set; }
        public int[] Docid { get; set; }
        public string[] DocPath { get; set; }
        public int[] DocType { get; set; }
        public string[] DocName { get; set; }
        public string[] AttachName { get; set; }
        // public HttpPostedFileBase[] file { get; set; }
        public List<UploadDocumentDetailsList> Upload { get; set; }

        public Nullable<int> ProposalSource { get; set; }
        [Required]
        public string Department { get; set; }
        public string PIDepartmentName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Proposalinwarddate { get; set; }
        public string Prpsalinwrddate { get; set; }
        public string PIEmail { get; set; }
        public string workphone { get; set; }
        public int[] CoPIname { get; set; }
        public int[] CoPIid { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string[] CoPIEmail { get; set; }
        public string[] CoPIDepartment { get; set; }
        public List<MasterlistviewModel>[] PIListDepWise { get; set; }
        public List<MasterlistviewModel>[] OtherInstCoPIListDepWise { get; set; }
        public List<MasterlistviewModel> MasterPIListDepWise { get; set; }
        [RequiredIf("ProjectSubtype", 2, ErrorMessage = "Project Scheme field is required")]
        public int? Schemename { get; set; }
        [RequiredIf("ProjectType", "2", ErrorMessage = "Consultancy Type field is required")]
        public Nullable<int> Constype { get; set; }
        public string Personapplied { get; set; }
        public string Remarks { get; set; }
        public ProposalSearchFieldModel SearchField { get; set; }
        public PagedData<ProposalSearchResultModel> SearchResult { get; set; }
        public string[] CoPIInstitute { get; set; }
        public string[] OtherInstituteCoPIDepartment { get; set; }
        public string[] OtherInstituteCoPIName { get; set; }
        public int[] OtherInstituteCoPIid { get; set; }
        public string[] RemarksforOthrInstCoPI { get; set; }
        public string Otherinstcopi_Qust_1 { get; set; }
        [Required]
        [Display(Name = "Tentative Start Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeStartDate { get; set; }
        [Required]
        [Display(Name = "Tentative Close Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeCloseDate { get; set; }
        public string PersonAppliedInstitute { get; set; }
        public string PersonAppliedPlace { get; set; }
        public bool PFMSSchemeCode
        {
            get
            {
                return (this.ProjectSubtype == 2 && this.ProjectCategory == 1);
            }
        }
        //[RequiredIf("PFMSSchemeCode", true, ErrorMessage = "Scheme Code field is required")]
        public Nullable<int> SchemeCode { get; set; }
        public string Constypecode { get; set; }
        public string SanctionNumber { get; set; }
        public List<MasterlistviewModel> SchemeList { get; set; }
        public List<CodeControllistviewModel> CategoryList { get; set; }
        public string Status { get; set; }
        public string FilterStatus { get; set; }
        public string SponsoringAgencyName { get; set; }
        public string FinYearType { get; set; }
        public string SourceName { get; set; }
        public string InwardDates { get; set; }
        public string ApprovalDates { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectCategoryName { get; set; }
        public string AgencyName { get; set; }
        public string ConsultancyTypeName { get; set; }
        public string StartDate { get; set; }
        public string CloseDate { get; set; }
        public string DepartmentName { get; set; }
        public string PIName { get; set; }
        public string SchemeCodeName { get; set; }
        public string ProjectSubtypeName { get; set; }
        public string SourceEmailDate { get; set; }
        public string ConsultancyType { get; set; }
        public string SponsoredSchemes { get; set; }
        public bool PFInit { get; set; }
        public string SponsoredTypeCode { get; set; }
        public List<CoInvestigatorModel> CopiList { get; set; }
        public List<CoInvestigatorOtherModel> OtherCOPIList { get; set; }
        public List<ProposalDocument> DocumentList { get; set; }
        public string IsthereAnyArtificialIntelligence { get; set; }
        public string AIDescription { get; set; }
        public string Domainrelevant { get; set; }
    }

    public class CoInvestigatorModel
    {
        public string DepartMentOfCOPI { get; set; }
        public string COPIName { get; set; }
        public string COPIEmailId { get; set; }
    }

    public class CoInvestigatorOtherModel
    {
        public string InstituteName { get; set; }
        public string InstituteCOPIDEP { get; set; }
        public string NameOfCOPI { get; set; }
        public string Remarks { get; set; }
    }

    public class ProposalDocument
    {
        public string AttachmentTypeName { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentFileName { get; set; }
        public string AttachmentFilePath { get; set; }
    }

    
   



}