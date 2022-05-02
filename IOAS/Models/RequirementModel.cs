using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace IOAS.Models
{

    #region Annoncement Model
    public class AnnouncementMasterModel
    {
        public int SNo { get; set; }
        public string RefNo { get; set; }
        public string Status { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> SelProjectNumber { get; set; }
        public Nullable<int> AnnouncementID { get; set; }
        public bool IsAnnouncementCreate
        {
            get
            {
                return (this.StatusID == null || this.StatusID == 1);
            }
        }

        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Announcement Title field is required")]
        [Display(Name = "Announcement Title")]
        public string AnnouncementTitle { get; set; }
        public string AnnouncementType { get; set; }
        public Nullable<int> AnnouncementStatus { get; set; }
        public string strAnnouncementStatus { get; set; }
        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The DAComments field is required")]
        public string DAComments { get; set; }
        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Mode Of Request field is required")]
        public string ModeOfRequest { get; set; }
        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Announcement Request By field is required")]
        public string AnnouncementRequestBy { get; set; }
        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Announcement Category field is required")]
        public string AnnouncementCategory { get; set; }
        public bool ValidAnnouncementPlatform_f
        {
            get
            {
                return (this.IsAnnouncementCreate && (this.Naukri == false && this.indeed == false && this.linkedin == false && this.Recruitmentportal == false && this.Others == false));
            }
        }

        public bool Naukri { get; set; }
        public bool indeed { get; set; }
        public bool linkedin { get; set; }
        public bool Recruitmentportal { get; set; }
        public bool Others { get; set; }
        [RequiredIf("ValidAnnouncementPlatform_f", true, ErrorMessage = "Please select any one")]
        public Nullable<bool> Platform_f { get; set; }

        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Request Receive Date field is required")]
        [Display(Name = "Request Receive Date")]
        public Nullable<DateTime> RequestReceiveDate { get; set; }
        [RequiredIf("IsAnnouncementCreate", true, ErrorMessage = "The Announcement Closure Date field is required")]
        [Display(Name = "Announcement Closure Date")]
        public Nullable<DateTime> AnnouncementClosureDate { get; set; }
        public string strRequestReceiveDate { get; set; }
        public string strAnnouncementClosureDate { get; set; }
        [Display(Name = "Interview schedule date")]
        [RequiredIf("StatusID", 6, ErrorMessage = "The Interview Date field is required")]
        public Nullable<DateTime> InterviewDate { get; set; }
        public Nullable<DateTime> OfferletterGenerationdate { get; set; }
        public string strInterviewDate { get; set; }
        public string strOfferletterGenerationdate { get; set; }
        public List<DesignationDetailsModel> DesignationDetails { get; set; } = new List<DesignationDetailsModel>();
        public List<CommiteeMemberDetailsModel> CommiteeMemberDetails { get; set; }
        public List<AnnouncementMailModel> CandidateList { get; set; }

        public bool Designationwise_f
        {
            get
            {
                return ((this.StatusID == null || this.StatusID == 1) && this.AnnouncementCategory == "Designation wise");
            }
        }
        [RequiredIf("Designationwise_f", true, ErrorMessage = "The PI name field is required")]
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string PIDepartment { get; set; }
        public DateTime DraftDate { get; set; }
        public DateTime ShortlistedDate { get; set; }
        public DateTime SelectedlistDate { get; set; }
        public string strDraftDate { get; set; }
        public string strShortlistedDate { get; set; }
        public string strSelectedlistDate { get; set; }
        public AnnouncementSearchFieldModel SearchField { get; set; }
        public string Remarks { get; set; }
        public string Remarks2 { get; set; }
        public HttpPostedFileBase RemarkDocument { get; set; }
        public HttpPostedFileBase DeanNote { get; set; }
        public HttpPostedFileBase ShortlistDeanNote { get; set; }
        public HttpPostedFileBase SelectionlistDeanNote { get; set; }

        public string RemarkDocumentDoc { get; set; }
        public bool InterviewDateClosed { get; set; }
        public ProjectDetails DraftProjectDetails { get; set; }
        public string PrijectTitle { get; set; }
        public Nullable<int> ProjectId { get; set; }

        public string PortalsPlatform { get; set; }
        public bool isRepost { get; set; }

        public bool isDeannote { get; set; }
        public bool isShortlist { get; set; }
        public bool isSelectionlist { get; set; }
        public string Button { get; set; }
        public bool isNotIncludeHeader { get; set; }
        public string FlowApprover { get; set; }
        public string FlowApprover2 { get; set; }
        public string FlowApprover3 { get; set; }

        public string DeanNoteDocPath { get; set; }
        public string ShortlistDeanNoteDocPath { get; set; }
        public string SelectionlistDeanNoteDocPath { get; set; }



        //public ValidAnnouncementDetail validDetails { get; set; }
    }
    //public class ValidAnnouncementDetail
    //{
    //    public Nullable<int> StatusID { get; set; }

    //    public bool IsValidDetail
    //    {
    //        get
    //        {
    //            return (this.StatusID == null || this.StatusID == 1);
    //        }
    //    }
    //}
    public class DesignationDetailsModel/* : ValidAnnouncementDetail*/
    {
        public Nullable<int> DestinationDeatailID { get; set; }
        //[RequiredIf("IsValidDetail", true, ErrorMessage = "The Designation Code field is required")]
        public string DesignationCode { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public string Designation { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectID { get; set; }
        //[RequiredIf("IsValidDetail", true, ErrorMessage = "The Vacancies field is required")]
        public Nullable<int> Vacancies { get; set; }
        //[RequiredIf("IsValidDetail", true, ErrorMessage = "The Qualification field is required")]
        public string Qualification { get; set; }
        //[RequiredIf("IsValidDetail", true, ErrorMessage = "The MinSalary field is required")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Min Salary")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> MinSalary { get; set; }
        //[RequiredIf("IsValidDetail", true, ErrorMessage = "The MaxSalary field is required")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Max Salary")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> MaxSalary { get; set; }
        public Nullable<decimal> FixedSalary { get; set; }
        public Nullable<int> TotalApplicant { get; set; }
        public Nullable<int> ShortlistedApplicant { get; set; }
        public Nullable<int> SelectedApplicant { get; set; }
        public string TotalApplicantDoc { get; set; }
        public string ShortlistedApplicantDoc { get; set; }
        public string SelectedApplicantDoc { get; set; }
        public HttpPostedFileBase TotalApplicantFile { get; set; }
        public HttpPostedFileBase ShortlistedApplicantFile { get; set; }
        public HttpPostedFileBase SelectedApplicantFile { get; set; }
        public string MinSalarystr { get; set; }
        public string MaxSalarystr { get; set; }

    }
    public class CommiteeMemberDetailsModel
    {
        public Nullable<int> CommiteeMemberDetailID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string PIName { get; set; }
        public string PIDesignation { get; set; }
        public string PIDepartment { get; set; }
        public string PIInstitution { get; set; }
    }
    public class ProjectDetails
    {
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectType { get; set; }
        public string SponsoringAgency { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectClosureDate { get; set; }
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string PICode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PIDepartmentCode { get; set; }
        public string PIDepartmentName { get; set; }
        public string PIDesignation { get; set; }

    }
    public class SearchAnnouncementModel
    {
        public string SearchINAdvertisementNo { get; set; }
        public string Status { get; set; }
        public string AnnouncementTitle { get; set; }
        public string AnnouncementCategory { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINStatus { get; set; }
        public string SearchKeyword { get; set; }
        public string PIName { get; set; }

        public int TotalRecords { get; set; }
        public List<AnnouncementMasterModel> AnnouncementList { get; set; }
    }
    public class AnnouncementSearchFieldModel
    {
        public string SearchINAdvertisementNo { get; set; }
        public string Status { get; set; }
        public string AnnouncementTitle { get; set; }
        public string SearchKeyword { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class AnnouncementMailModel
    {
        public string Name { get; set; }
        public string DateofBirth { get; set; }
        public string Age { get; set; }
        public string EmailID { get; set; }
        public string PhoneNumber { get; set; }
        public string TotalExperience { get; set; }
        public string UnderGraduationdegree { get; set; }
        public string UGSpecialization { get; set; }
        public string PostGraduationDegree { get; set; }
        public string PGspecialization { get; set; }
        public string Gender { get; set; }
        public string PermanentAddress { get; set; }
        public string CurrentLocation { get; set; }
        public string Designation { get; set; }
        public string PaySalary { get; set; }


    }
    public class FillDesignationModel
    {
        public int DestinationID { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public string MinSalary { get; set; }
        public string MaxSalary { get; set; }
        public string AgeLimit { get; set; }
        public string Qualification { get; set; }
        public string Mark { get; set; }
        public string Experience { get; set; }
        public string Department { get; set; }
        public bool HRA { get; set; }
        public decimal HRAPercentage { get; set; }
        public bool Medical { get; set; }
        public decimal MedicalAmount { get; set; }
        public bool GateScore { get; set; }
        public string SalaryLevel { get; set; }
        public int SalaryLevelId { get; set; }
        public string SalaryLevelDescription { get; set; }

    }

    #endregion


    //public class STEModel
    //{
    //    public int STEId { get; set; }
    //    [Display(Name = "Type of appointment")]
    //    public Nullable<int> TypeofappointmentId { get; set; }
    //    [Display(Name = "Professional")]
    //    public Nullable<int> ProfessionalId { get; set; }
    //    public string Professional { get; set; }

    //    public string EmployeeType { get; set; }
    //    public string OldEmployeeNumber { get; set; }
    //    public Nullable<int> OldEmpId { get; set; }
    //    public string NIDNumber { get; set; }
    //    public string PersonImagePath { get; set; }
    //    public HttpPostedFileBase PersonImage { get; set; }
    //    public HttpPostedFileBase JoinungReport { get; set; }
    //    public HttpPostedFileBase Resume { get; set; }
    //    public string ResumeFilePath { get; set; }
    //    public string ResumeFileName { get; set; }

    //    [Display(Name = "Name")]
    //    public string Name { get; set; }
    //    [Display(Name = "Father’s / Husband’s Name")]
    //    public string Nameoftheguardian { get; set; }
    //    [Range(100000000000, 999999999999, ErrorMessage = "Aadhaar number should not exceed 12 characters")]
    //    public long? aadharnumber { get; set; }
    //    [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
    //    [Display(Name = "PAN No")]
    //    public string PAN { get; set; }
    //    [Display(Name = "Date of Birth")]
    //    public Nullable<DateTime> DateofBirth { get; set; }
    //    public string strDateofBirth { get; set; }

    //    public int Age { get; set; }
    //    public int Sex { get; set; }
    //    public int Caste { get; set; }
    //    [MaxLength(10)]
    //    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
    //    [Display(Name = "Contact Number")]
    //    public string ContactNumber { get; set; }
    //    [MaxLength(10)]
    //    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Alternate Phone Number")]
    //    [Display(Name = "Alternative Contact No")]
    //    public string AlternativeContactNumber { get; set; }
    //    [EmailAddress]
    //    public string Email { get; set; }
    //    public bool isSameasPermanentAddress { get; set; }
    //    [Display(Name = "Permanent Address")]
    //    public string PermanentAddress { get; set; }
    //    public string PresentAddress { get; set; }
    //    public Nullable<int> BloodGroup { get; set; }
    //    public Nullable<int> BloodGroupRH { get; set; }
    //    public string StaffCategory { get; set; }
    //    public string StaffCategoryID { get; set; }
    //    public int Nationality { get; set; }
    //    public string NationalityID { get; set; }
    //    public int PhysicallyChallenged { get; set; }
    //    public string RelatedIITMadras { get; set; }
    //    public string Relationship { get; set; }
    //    public string RelationshipName { get; set; }
    //    public Nullable<DateTime> ApplicationEntryDate { get; set; }
    //    [Display(Name = "Application Receive Date")]
    //    public Nullable<DateTime> ApplicationReceiveDate { get; set; }
    //    public bool ConsolidatedPay { get; set; }
    //    public bool Fellowship { get; set; }
    //    public int IITMPensionerCSIRStaff { get; set; }
    //    public string PPONo { get; set; }
    //    public Nullable<int> CSIRStaff { get; set; }
    //    public bool MsPhd { get; set; }
    //    public Nullable<int> MsPhdType { get; set; }
    //    public string PhdDetail { get; set; }
    //    public HttpPostedFileBase[] PIJustificationFile { get; set; }
    //    public HttpPostedFileBase PIJustificationFile2 { get; set; }
    //    public string[] PIJustificationFilePath { get; set; }
    //    public string[] PIJustificationFileName { get; set; }
    //    public string PIJustificationFilePath2 { get; set; }
    //    public bool PIJustification { get; set; }
    //    public string PIJustificationRemarks { get; set; }
    //    public string Remarks { get; set; }
    //    public string BankAccountNo { get; set; }
    //    public Nullable<int> BankId { get; set; }
    //    public string BankName { get; set; }
    //    [MaxLength(11)]
    //    [RegularExpression("[A-Z|a-z]{4}[0][a-zA-Z0-9]{6}", ErrorMessage = "Invalid IFSC Code")]
    //    [Display(Name = "IFSC Code")]
    //    public string IFSCCode { get; set; }
    //    public string BankBranch { get; set; }
    //    public Nullable<int> ProjectId { get; set; }
    //    public string ProjectNumber { get; set; }
    //    public string DesignationCode { get; set; }
    //    public Nullable<decimal> MinSalary { get; set; }
    //    public Nullable<decimal> MaxSalary { get; set; }

    //    public string Designation { get; set; }
    //    public Nullable<int> DesignationId { get; set; }
    //    public int Medical { get; set; }
    //    [Range(0, 9999999999999999.99)]
    //    public decimal MedicalAmmount { get; set; }
    //    public decimal DesignaionMedicalAmount { get; set; }

    //    public string ApplicationNo { get; set; }

    //    [Display(Name = "Appointment start date")]
    //    public Nullable<DateTime> Appointmentstartdate { get; set; }
    //    [Display(Name = "Appointment End Date")]
    //    public Nullable<DateTime> AppointmentEndDate { get; set; }
    //    [Display(Name = "Salary")]
    //    [Range(0, 9999999999999999.99)]
    //    public Nullable<decimal> Salary { get; set; }
    //    public string SalaryPayHigh { get; set; }
    //    [Range(0, 9999999999999999.99)]
    //    public decimal HRA { get; set; }
    //    [Range(0, 9999999999999999.99)]
    //    public Nullable<decimal> CommitmentAmount { get; set; }
    //    public string Comments { get; set; }
    //    public string VerificationRemarks { get; set; }
    //    public string Note { get; set; }
    //    public string CommiteeMember1 { get; set; }
    //    public Nullable<int> CommiteeMemberId1 { get; set; }
    //    public string CommiteeMember2 { get; set; }
    //    public Nullable<int> CommiteeMemberId2 { get; set; }
    //    public string ChairpersonName { get; set; }
    //    public Nullable<int> ChairpersonNameId { get; set; }
    //    public string FlowofMail { get; set; }
    //    public string isHaveGateScore { get; set; }
    //    [Range(0, 100, ErrorMessage = "Value should be between 0 and 100")]
    //    public Nullable<decimal> GateScore { get; set; }
    //    public bool isHRA { get; set; }
    //    public Nullable<int> UserIdD { get; set; }
    //    public List<STEEducationModel> EducationDetail { get; set; }
    //    public List<STEExperienceModel> ExperienceDetail { get; set; }
    //    public List<DeviationModel> DeviationDetail { get; set; }
    //    public List<CheckListModel> CheckListDetail { get; set; }
    //    public List<STEJustificationDoc> JustificationDoc { get; set; }
    //    public List<STENotes> Notes { get; set; }
    //    public string Status { get; set; }
    //    public bool isDraftbtn { get; set; }
    //    public string bcc { get; set; }
    //    public string EmployeeWorkplace { get; set; }

    //    public string ToMail { get; set; }
    //    public bool bccSaved { get; set; }
    //    public int SNo { get; set; }
    //    public string DepartmentName { get; set; }
    //    public string OfferDate { get; set; }
    //    [MaxLength(10)]
    //    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
    //    [Display(Name = "Contact Number")]
    //    public string EmergencyContactNo { get; set; }
    //    public DateTime OfferLetterDate { get; set; }
    //    public DateTime ActualDate { get; set; }
    //    public HttpPostedFileBase CantidateSignature { get; set; }
    //    public string CantidateSignatureFilePath { get; set; }
    //    public int EmployeeTypeCatecory { get; set; }
    //    public int GovProof { get; set; }
    //    public bool isVerifiedGovProof { get; set; }
    //    public string STEAppNo { get; set; }
    //    public string PIName { get; set; }
    //    public string Category { get; set; }
    //    public string CondidateName { get; set; }
    //    public Nullable<int> EmailRemaindarCount { get; set; }
    //    public string EmployeeId { get; set; }
    //    public STEViewModel PrjModel { get; set; }
    //    public ProjectDetails ProjectDetail { get; set; }
    //    public DateTime RelievingDate { get; set; }
    //    public string CommitmentNo { get; set; }
    //    public bool Ordergenerate { get; set; }
    //    public List<string> PIJustificationCommands { get; set; }
    //    public decimal BasicPay { get; set; }
    //    public string isConsolidatePay { get; set; }
    //    public string Typeofappointment { get; set; }
    //    public decimal HRAPercentage { get; set; }
    //    public bool isGovAgencyFund { get; set; }
    //    public string FlowApprover { get; set; }
    //    public string PayType { get; set; }
    //    public bool VerifyProfile { get; set; }
    //    public bool isCommitmentRejection { get; set; }

    //    //OutSourcing
    //    public string VendorName { get; set; }
    //    public List<OtherDetailModel> OtherDetail { get; set; }
    //    public SalaryCalcModel Salarycalc { get; set; }
    //    public Nullable<int> SalaryCalcId { get; set; }
    //    public string EmpType { get; set; }
    //    public string EmpName { get; set; }
    //    public string EmpDesig { get; set; }
    //    public string PhysicalyHandicaped { get; set; }
    //    public Nullable<decimal> EmpPFBasicWages { get; set; }
    //    public Nullable<decimal> EmployeePF { get; set; }
    //    public Nullable<decimal> EmployeeESIC { get; set; }
    //    public Nullable<decimal> RecommendedSalary { get; set; }
    //    public Nullable<decimal> EmployeeProfessionalTax { get; set; }
    //    public Nullable<decimal> EmployeeTtlDeduct { get; set; }
    //    public Nullable<decimal> EmployeeNetSalary { get; set; }
    //    public Nullable<decimal> EmployerPF { get; set; }
    //    public Nullable<decimal> EmployerIns { get; set; }
    //    public Nullable<decimal> EmployerESIC { get; set; }
    //    public Nullable<decimal> EmployerTtlContribute { get; set; }
    //    public Nullable<decimal> EmployeeCTC { get; set; }
    //    public Nullable<decimal> AgencyFee { get; set; }
    //    public Nullable<decimal> SalaryGST { get; set; }
    //    public Nullable<decimal> CTCwithAgencyFee { get; set; }
    //    public Nullable<int> EmpSalutation { get; set; }
    //    public Nullable<int> VendorId { get; set; }
    //    public Nullable<decimal> TotalCTC { get; set; }
    //    public string AppType { get; set; }

    //    public List<OtherDocModel> OtherDocList { get; set; }

    //}

    public class STEModel
    {
        public int STEId { get; set; }
        [Required]
        [Display(Name = "Type of appointment")]
        public Nullable<int> TypeofappointmentId { get; set; }
        [Required]
        [Display(Name = "Professional")]
        public Nullable<int> ProfessionalId { get; set; }
        public string Professional { get; set; }
        [Required]
        public string EmployeeType { get; set; }
        [RequiredIf("EmployeeType", "Old Employee", ErrorMessage = "Please enter old employee number")]
        public string OldEmployeeNumber { get; set; }
        //[RequiredIf("EmployeeType", "New Employee", ErrorMessage = "Please enter NIDNumber")]
        public string NIDNumber { get; set; }
        public string OldEmpId { get; set; }
        public string PersonImagePath { get; set; }
        public HttpPostedFileBase PersonImage { get; set; }
        public HttpPostedFileBase JoinungReport { get; set; }
        public HttpPostedFileBase Resume { get; set; }
        public string ResumeFilePath { get; set; }
        public string ResumeFileName { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Father’s / Husband’s Name")]
        public string Nameoftheguardian { get; set; }
        [RequiredIf("PAN", null, ErrorMessage = "Please enter aadhaar number")]
        [Range(100000000000, 999999999999, ErrorMessage = "Aadhaar number should not exceed 12 characters")]
        public long? aadharnumber { get; set; }
        [RequiredIf("aadharnumber", null, ErrorMessage = "Please enter pan number")]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN No")]
        public string PAN { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public Nullable<DateTime> DateofBirth { get; set; }
        public string strDateofBirth { get; set; }
        public int Age { get; set; }
        [Required]
        public Nullable<int> Sex { get; set; }
        [Required]
        public Nullable<int> Caste { get; set; }
        [Required]
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Alternate Phone Number")]
        [Display(Name = "Alternative Contact No")]
        public string AlternativeContactNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool isSameasPermanentAddress { get; set; }
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        [Required]
        public string PresentAddress { get; set; }
        public Nullable<int> BloodGroup { get; set; }
        public Nullable<int> BloodGroupRH { get; set; }
        [Required]
        public string StaffCategory { get; set; }
        public string StaffCategoryID { get; set; }
        [Required]
        public Nullable<int> Nationality { get; set; }
        public string NationalityID { get; set; }
        [Required]
        public Nullable<int> PhysicallyChallenged { get; set; }
        public string RelatedIITMadras { get; set; }
        public string Relationship { get; set; }
        public string RelationshipName { get; set; }
        public Nullable<DateTime> ApplicationEntryDate { get; set; }
        [Required]
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool Fellowship { get; set; }
        [Required]
        public Nullable<int> IITMPensionerCSIRStaff { get; set; }
        [RequiredIf("IITMPensionerCSIRStaff", 1, ErrorMessage = "Please enter PPONo")]
        public string PPONo { get; set; }
        [RequiredIf("IITMPensionerCSIRStaff", 2, ErrorMessage = "Please select CSIRStaff")]
        public Nullable<int> CSIRStaff { get; set; }
        public bool MsPhd { get; set; }
        public Nullable<int> MsPhdType { get; set; }
        public bool MsPhdTypeValid
        {
            get
            {
                return (this.MsPhdType == 1 || this.MsPhdType == 2);
            }
        }
        [RequiredIf("MsPhdTypeValid", true, ErrorMessage = "Please enter M.S/Ph.D roll number")]
        public string PhdDetail { get; set; }
        public HttpPostedFileBase[] PIJustificationFile { get; set; }
        public string PIJustificationRemarks { get; set; }
        public string BankAccountNo { get; set; }
        public Nullable<int> BankId { get; set; }
        public string BankName { get; set; }
        [MaxLength(11)]
        [RegularExpression("[A-Z|a-z]{4}[0][a-zA-Z0-9]{6}", ErrorMessage = "Invalid IFSC Code")]
        [Display(Name = "IFSC Code")]
        public string IFSCCode { get; set; }
        public string BankBranch { get; set; }
        [Required]
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string DesignationCode { get; set; }
        public Nullable<decimal> MinSalary { get; set; }
        public Nullable<decimal> MaxSalary { get; set; }
        public string Designation { get; set; }
        [Required]
        public Nullable<int> DesignationId { get; set; }
        [Required]
        public Nullable<int> Medical { get; set; }
        public decimal MedicalAmmount { get; set; }
        public decimal DesignaionMedicalAmount { get; set; }
        public string ApplicationNo { get; set; }
        [Required]
        [Display(Name = "Appointment start date")]
        public Nullable<DateTime> Appointmentstartdate { get; set; }
        [Required]
        [Display(Name = "Appointment End Date")]
        public Nullable<DateTime> AppointmentEndDate { get; set; }
        [Required]
        [Display(Name = "Salary")]
        public Nullable<decimal> Salary { get; set; }
        public string SalaryPayHigh { get; set; }
        public decimal HRA { get; set; }
        [Required]
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string Comments { get; set; }
        public string CommiteeMember1 { get; set; }
        public Nullable<int> CommiteeMemberId1 { get; set; }
        public string CommiteeMember2 { get; set; }
        public Nullable<int> CommiteeMemberId2 { get; set; }
        public string ChairpersonName { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
        public string FlowofMail { get; set; }
        public string isHaveGateScore { get; set; }
        [RequiredIf("isHaveGateScore", "Yes", ErrorMessage = "Please enter GateScore")]
        [Range(0, 100, ErrorMessage = "Value should be between 0 and 100")]
        public Nullable<decimal> GateScore { get; set; }
        public bool isHRA { get; set; }
        public Nullable<int> UserIdD { get; set; }
        public List<STEEducationModel> EducationDetail { get; set; }
        public List<STEExperienceModel> ExperienceDetail { get; set; }
        public List<DeviationModel> DeviationDetail { get; set; }
        public List<CheckListModel> CheckListDetail { get; set; }
        public List<STEJustificationDoc> JustificationDoc { get; set; }
        public List<PIJustificationModel> PIJustificationDocDetail { get; set; } = new List<PIJustificationModel>();
        public List<STENotes> Notes { get; set; }
        public string Status { get; set; }
        public bool isDraftbtn { get; set; }
        [RegularExpression("^([\\w+-.%]+@[\\w-.]+\\.[A-Za-z]{2,6},?)+$", ErrorMessage = "Invalid CC Mail Example:abc@mail.com,abx@mail.in,abz@mail.com")]
        public string bcc { get; set; }
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid To Mail")]
        public string ToMail { get; set; }
        public string EmployeeWorkplace { get; set; }
        public bool bccSaved { get; set; }
        public int SNo { get; set; }
        public string DepartmentName { get; set; }
        public string OfferDate { get; set; }
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Contact Number")]
        public string EmergencyContactNo { get; set; }
        public DateTime OfferLetterDate { get; set; }
        public DateTime ActualDate { get; set; }
        public HttpPostedFileBase CantidateSignature { get; set; }
        public string CantidateSignatureFilePath { get; set; }
        public int EmployeeTypeCatecory { get; set; }
        public int GovProof { get; set; }
        public bool isVerifiedGovProof { get; set; }
        public string STEAppNo { get; set; }
        public string PIName { get; set; }
        public string Category { get; set; }
        public string CondidateName { get; set; }
        public Nullable<int> EmailRemaindarCount { get; set; }
        public string EmployeeId { get; set; }
        public STEViewModel PrjModel { get; set; }
        public ProjectDetails ProjectDetail { get; set; }
        public DateTime RelievingDate { get; set; }
        public string CommitmentNo { get; set; }
        public bool Ordergenerate { get; set; }
        public List<string> PIJustificationCommands { get; set; }
        public decimal BasicPay { get; set; }
        [Display(Name = "Consolidated Pay or Fellowship Pay")]
        [Required]
        public string isConsolidatePay { get; set; }
        public string Typeofappointment { get; set; }
        public decimal HRAPercentage { get; set; }
        public bool isGovAgencyFund { get; set; }
        public string FlowApprover { get; set; }
        public string PayType { get; set; }
        public bool VerifyProfile { get; set; }
        public bool isCommitmentRejection { get; set; }
        public string ApplicationRefNo { get; set; }
        public Nullable<int> RequestedByPI { get; set; }
        public string AutoFillRequstedbyPI { get; set; }
        //OutSourcing
        public string VendorName { get; set; }
        public List<OtherDetailModel> OtherDetail { get; set; }
        public SalaryCalcModel Salarycalc { get; set; }
        public Nullable<int> SalaryCalcId { get; set; }
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<int> EmpSalutation { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public Nullable<int> StatutoryId { get; set; }
        public string AppType { get; set; }
        public List<OtherDocModel> OtherDocList { get; set; }
        public string VendorCode { get; set; }
        public Nullable<decimal> LWFAmount { get; set; }
        public string SendSalaryStructure { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public Nullable<int> WfId { get; set; }

    }

    public class STEEducationModel
    {
        public Nullable<int> EducationId { get; set; }
        public string Education { get; set; }
        [Required]
        [Display(Name = "Qualification")]
        public Nullable<int> QualificationId { get; set; }
        [Required]
        [Display(Name = "Decipline")]
        public Nullable<int> DisciplineId { get; set; }
        public string Discipline { get; set; }
        [Required]
        [Display(Name = "Institution")]
        public string Institution { get; set; }
        [Display(Name = "Year of Passing")]
        public Nullable<int> YearofPassing { get; set; }
        [Required]
        [Display(Name = "Mark Type")]
        public Nullable<int> MarkType { get; set; }
        public string strMarkType { get; set; }
        [Required]
        [Display(Name = "Marks")]
        [Range(0, 99.99, ErrorMessage = "Value should be between 0 and 100")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> Marks { get; set; }
        [Display(Name = "Division Class Obtained")]
        public string DivisionClassObtained { get; set; }
        public HttpPostedFileBase Certificate { get; set; }
        public string CertificatePath { get; set; }
        public string CertificateName { get; set; }
        public string Remarks { get; set; }
        public string GateScore { get; set; }
        public string Score { get; set; }
        public List<MasterlistviewModel> DisiplineList { get; set; }
        public bool Verify { get; set; }
        public Nullable<int> StatutoryId { get; set; }

    }

    public class STEExperienceModel
    {

        public bool expValid
        {
            get
            {
                return (this.ExperienceTypeId != null || !string.IsNullOrEmpty(this.Organisation) || !string.IsNullOrEmpty(this.DesignationautoComplete) || this.FromDate != null || this.ToDate != null || this.SalaryDrawn != null);
            }
        }
        public bool ExperienceTypeexpValid
        {
            get
            {
                return (this.expValid && this.ExperienceTypeId == null);
            }
        }
        public bool OrganisationValid
        {
            get
            {
                return (this.expValid && string.IsNullOrEmpty(this.Organisation));
            }
        }
        public bool DesignationautoCompleteValid
        {
            get
            {
                return (this.expValid && string.IsNullOrEmpty(this.DesignationautoComplete));
            }
        }
        public bool FromDateValid
        {
            get
            {
                return (this.expValid && this.FromDate == null);
            }
        }
        public bool ToDateValid
        {
            get
            {
                return (this.expValid && this.ToDate == null);
            }
        }
        public bool SalaryDrawnValid
        {
            get
            {
                return (this.expValid && this.SalaryDrawn == null);
            }
        }

        public Nullable<int> ExperienceId { get; set; }

        [Display(Name = "Type")]
        [RequiredIf("ExperienceTypeexpValid", true, ErrorMessage = "Please select ExperienceTypeId")]
        public Nullable<int> ExperienceTypeId { get; set; }
        public string ExperienceType { get; set; }

        [Display(Name = "Organisation")]
        [RequiredIf("OrganisationValid", true, ErrorMessage = "Please enter organisation")]
        public string Organisation { get; set; }
        [RequiredIf("DesignationautoCompleteValid", true, ErrorMessage = "Please enter designation")]
        public string DesignationautoComplete { get; set; }
        public string DesignationNames { get; set; }
        public Nullable<int> DesignationListId { get; set; }
        [RequiredIf("FromDateValid", true, ErrorMessage = "Please enter fromdate")]
        public Nullable<DateTime> FromDate { get; set; }
        [RequiredIf("ToDateValid", true, ErrorMessage = "Please enter todate")]
        public Nullable<DateTime> ToDate { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        [RequiredIf("SalaryDrawnValid", true, ErrorMessage = "Please enter salary drawn")]
        public Nullable<decimal> SalaryDrawn { get; set; }
        public HttpPostedFileBase ExperienceFile { get; set; }
        public string ExperienceFilePath { get; set; }
        public string ExperienceFileName { get; set; }
        public string Remarks { get; set; }
        public bool Verify { get; set; }
    }

    public class STEViewModel
    {
        public int STEId { get; set; }
        public string EmployeeType { get; set; }
        public string PIName { get; set; }
        public string EmployeeID { get; set; }

        public string OldNumber { get; set; }
        public string NIDNumber { get; set; }
        public int TypeofappointmentID { get; set; }
        public string Typeofappointment { get; set; }
        public string ProfessionalId { get; set; }
        public string AadhaarNumber { get; set; }
        public string PANNo { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Caste { get; set; }
        public string ContactNo { get; set; }
        public string AlternativeContactNo { get; set; }
        public string EmergencyContactNo { get; set; }
        public string Email { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string BloodGroup { get; set; }
        public string BloodGroupRH { get; set; }
        public string Nationality { get; set; }
        public string PhysicallyChanged { get; set; }
        public string RelasionIIT { get; set; }
        public string Relationship { get; set; }
        public string RelationName { get; set; }
        public string PayType { get; set; }
        public string IITMPensionerOrCSIRStaff { get; set; }
        public string PPONo { get; set; }

        public string CSIRStaff { get; set; }
        public string StaffCategory { get; set; }
        public string ApplicationEntryDate { get; set; }
        public string ApplicationReceiveDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool Fellowship { get; set; }
        public int IITMPensionerCSIRStaff { get; set; }
        public bool MsPhd { get; set; }
        public string MsOrPhd { get; set; }
        public string PhdDetail { get; set; }
        public List<STEJustificationDoc> Attachments { get; set; }
        public List<STEJustificationDoc> PIJustificationDocuments { get; set; } = new List<STEJustificationDoc>();
        public List<STENotes> Notes { get; set; }
        public List<string> PIJustificationCommands { get; set; } = new List<string>();

        public Nullable<int> RoleId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string PersonImagePath { get; set; }
        public string SingnaturePath { get; set; }

        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }
        public int Medical { get; set; }
        public string MedicalINWordings { get; set; }
        public decimal MedicalAmmount { get; set; }
        public string ApplicationNo { get; set; }
        public string Appointmentstartdate { get; set; }
        public Nullable<DateTime> AppointmentFromdate { get; set; }

        public string ActualAppointmentstartdate { get; set; }

        public string AppointmentEndDate { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> BasicPay { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public string Comments { get; set; }
        public string Note { get; set; }
        public string CommiteeMember1 { get; set; }
        public Nullable<int> CommiteeMemberId1 { get; set; }
        public string CommiteeMember2 { get; set; }
        public Nullable<int> CommiteeMemberId2 { get; set; }
        public string ChairpersonName { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; }

        public string Status { get; set; }
        public ProjectDetails ProjectDetailsModel { get; set; }
        public string BankAC { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string isHaveGateScore { get; set; }
        public string GateScore { get; set; }
        public List<STEEducationModel> EducationDetail { get; set; }
        public List<STEExperienceModel> ExperienceDetail { get; set; }
        public RecruitCommitRequestModel CommitReqModel { get; set; }
        public ProjSummaryModel Projsummary { get; set; }
        public decimal ConsolidatedPayPerMonth { get; set; }
        public bool Adminintif { get; set; }
        public DateTime offerDate { get; set; }
        public bool isCommiteeRejection { get; set; }
        public string CommitteeRemark { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        public string CommitteeApprovedBy { get; set; }
        public bool isGovAgencyNoFund { get; set; }
        public bool isFundAvailable { get; set; }
        public string FlowApprover { get; set; }
        public bool VerifyProfile { get; set; }
        public bool CommitmentRejection { get; set; }
        public string CommitmentRemark { get; set; }
        public string appType { get; set; }
        public int appid { get; set; }
        public int orderid { get; set; }
        public string CancelReason { get; set; }
        public string CancelDocument { get; set; }
        public string EmployeeWorkplace { get; set; }
        public string ApplicationRefNo { get; set; }
        public string AutoFillRequstedbyPI { get; set; }
        public string PayeeType { get; set; }
        public string ToMail { get; set; }
        public string CCMail { get; set; }
        //OutSourcing
        public List<OtherDetailModel> OtherDetail { get; set; }
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<int> EmpSalutation { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public Nullable<int> ordertype { get; set; }
        public List<SevicesListModel> EmployeeWorkExperience { get; set; }
        public Nullable<int> StatutoryId { get; set; }
        public Nullable<decimal> GSTPrecentage  { get; set; }
        public Nullable<decimal> AgencyFeePrecentage { get; set; }
        public Nullable<decimal> EmployeeESICPrecentage { get; set; }
        public Nullable<decimal> EmployerESICPrecentage { get; set; }
        public Nullable<decimal> LWFEmployerContribution { get; set; }
        public Nullable<decimal> CommitmentAvailableBalance { get; set; }
        public string PIJustificationRemarks { get; set; }
        public HttpPostedFileBase[] PIJustificationFile { get; set; }
        public string List_f { get; set; }
        public bool MailSent_f { get; set; }
        public string IITMExperience { get; set; }

    }

    public class STEVerificationModel
    {
        public Nullable<int> STEId { get; set; }
        [Display(Name = "Type of appointment")]
        public Nullable<int> TypeofappointmentId { get; set; }
        [RequiredIf("TypeofappointmentId", 4, ErrorMessage = "Other gov-agencies roll number field is required")]
        public string RollNumber { get; set; }
        public string PersonImagePath { get; set; }
        public HttpPostedFileBase PersonImage { get; set; }
        [Required]
        [Display(Name = "Joining report")]
        public HttpPostedFileBase JoiningReport { get; set; }
        public HttpPostedFileBase Resume { get; set; }
        public string JoiningReportPath { get; set; }
        public string JoiningReportFileName { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Range(100000000000, 999999999999, ErrorMessage = "Aadhaar number should not exceed 12 characters")]
        public long? aadharnumber { get; set; }
        [Required]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN No")]
        public string PAN { get; set; }
        [Required]
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [Required]
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Emergency Contact Number")]
        public string EmergencyContactNo { get; set; }

        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Alternate Phone Number")]
        [Display(Name = "Alternative Contact No")]
        public string AlternativeContactNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool isSameasPermanentAddress { get; set; }
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public string StaffCategory { get; set; }
        public string PPONo { get; set; }
        public int CSIRStaff { get; set; }
        public bool MsPhd { get; set; }
        public Nullable<int> MsPhdType { get; set; }
        public string PhdDetail { get; set; }
        [Required]
        public string BankAccountNo { get; set; }
        [Required]

        [RegularExpression("^[a-zA-Z ,-@&]+$", ErrorMessage = "Invalid data entry")]
        public string BankName { get; set; }
        public Nullable<int> BankId { get; set; }

        [Required]
        [MaxLength(11)]
        [RegularExpression("[A-Z|a-z]{4}[0][a-zA-Z0-9]{6}", ErrorMessage = "Invalid IFSC Code")]
        [Display(Name = "IFSC Code")]
        public string IFSCCode { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string Designation { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public decimal MedicalAmmount { get; set; }
        public string ApplicationNo { get; set; }
        public string Appointmentstartdate { get; set; }
        public string AppointmentEndDate { get; set; }
        public decimal HRA { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string CommitmentNo { get; set; }
        public string ResumeFileName { get; set; }
        public string ResumeFilePath { get; set; }
        public List<STEEducationModel> EducationDetail { get; set; }
        public List<STEExperienceModel> ExperienceDetail { get; set; }
        public List<STENotes> Notes { get; set; }
        public List<STENotes> DAComments { get; set; }

        public string Status { get; set; }
        public int SNo { get; set; }
        public string DepartmentName { get; set; }
        public string OfferDate { get; set; }
        [Range(0, 100, ErrorMessage = "GateScore should be between 0 and 100")]
        public Nullable<decimal> GateScore { get; set; }

        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Emergency Contact Number")]
        public string ContactNo { get; set; }

        [Display(Name = "Blood Group")]
        public Nullable<int> BloodGroup { get; set; }
        [Display(Name = "Blood GroupRH")]
        public Nullable<int> BloodGroupRH { get; set; }
        [Required]
        public Nullable<DateTime> ActualDate { get; set; }
        public HttpPostedFileBase CantidateSignature { get; set; }
        public string CantidateSignatureFilePath { get; set; }
        public int EmployeeTypeCatecory { get; set; }
        [Required]
        [Display(Name = "Government Proof")]
        public Nullable<int> GovProof { get; set; }
        public Nullable<bool> isVerifiedGovProof { get; set; } = true;
        public Nullable<int> EmailRemaindarCount { get; set; }
        public List<string> PIJustificationCommands { get; set; }
        public string Typeofappointment { get; set; }
        public string PayType { get; set; }
        public string VerificationRemarks { get; set; }
        public List<OtherDocModel> OtherDocList { get; set; }

        //Order Verification Model
        public Nullable<int> OrderId { get; set; }
        public string ApplicationType { get; set; }
        public string Expericence { get; set; }
        public string IITMExpericence { get; set; }
        public string Qualification { get; set; }
        public string DateofBirth { get; set; }
        public AttachmentDetailModel Attachments { get; set; }

        public bool isWithdraw { get; set; }
        public decimal Salary { get; set; }
        public Nullable<DateTime> OfferActualDate { get; set; }
        public string EmployeeNo { get; set; }
        public string OldEmployeeNumber { get; set; }
        public string DateofJoining { get; set; }
        public Nullable<DateTime> JoiningDate { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Invalid data entry")]
        public string EmployeeWorkplace { get; set; }
        public string VendorName { get; set; }
        public List<OtherDetailModel> OtherDetail { get; set; }
        public SalaryCalcModel Salarycalc { get; set; }
        public Nullable<int> SalaryCalcId { get; set; }
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<int> EmpSalutation { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public string isHaveGateScore { get; set; }
        public Nullable<int> StatutoryId { get; set; }
        public bool SendOffer_f { get; set; }
        public string RequestedfromPI { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }
        public bool Cancel_f { get; set; }
    }

    public class SearchSTEVerificationModel
    {
        public string SearchInApplicationNo { get; set; }
        public string SearchInPAN { get; set; }
        public string SearchInName { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public Nullable<DateTime> MeetingDate { get; set; }
        public string SearchINStatus { get; set; }
        public List<STEVerificationModel> VerificationList { get; set; }
        public int TotalRecords { get; set; }
        public string Applicationtype { get; set; }

        public Nullable<DateTime> OfferActualDate { get; set; }
        public string EmployeeNo { get; set; }
        public string DateofJoining { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<DateTime> JoingDate { get; set; }
    }

    public class STEJustificationDoc
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class STENotes
    {
        public string PICommends { get; set; }
        public string DAComments { get; set; }

        public string HRNote { get; set; }

    }

    public class DeviationModel
    {
        public bool Appointmenttenureshouldbeminimum1monthto1year { get; set; }
        public bool AppointmentcanbeMaximumfor5years { get; set; }
        public bool Oneyearbreakrequiredtocontinuefurtherserviceafter5years { get; set; }
        public bool Deviationinage { get; set; }
        public bool DeviationinQualification { get; set; }
        public bool DeviationinDesignation { get; set; }
        public bool DeviationinSalary { get; set; }
        public bool DeviationinworkExperience { get; set; }
        public bool NoManPower { get; set; }
        public bool NoFundAvailable { get; set; }
    }

    public class CheckDevationModel
    {
        public string PersonName { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int>[] QualificationId { get; set; }
        public Nullable<int>[] DisciplineId { get; set; }
        public Nullable<int> CheckAge { get; set; }
        public Nullable<decimal> ChekSalary { get; set; }
        public Nullable<decimal> Experience { get; set; }
        public decimal CommitmentAmount { get; set; }
        public Nullable<int>[] MasrksType { get; set; }
        public Nullable<decimal>[] Masrks { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime AppointmentStartDate { get; set; }
        public DateTime AppointmentEndDate { get; set; }
        public DateTime AppointmentReciveDate { get; set; }
        public string StaffCatecory { get; set; }
        public int PhysicallyChanged { get; set; }
        public string OrderType { get; set; }
        public string AppointmentType { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<decimal> BasicPay { get; set; }
        public string Comments { get; set; }
        public decimal GateScore { get; set; }
        public Nullable<int> AppId { get; set; }
        public string AppType { get; set; }
        public Nullable<int> TypeOfAppointment { get; set; }
        public string Paytype { get; set; }
        public bool PaymentthroughAgency { get; set; }
        public Nullable<int> Caste { get; set; }
        public string OldEmployee { get; set; }
        public string SendSalaryStructure { get; set; }
        public bool MsPhd { get; set; }
        public List<CheckListEmailModel> devChecklist { get; set; } = new List<CheckListEmailModel>();
        public string Experienceinwordings { get; set; }
    }

    public class CheckListEmailModel
    {
        public int SNo { get; set; }
        public string CheckList { get; set; }
        public Nullable<int> checklistId { get; set; }
        public string devScenarios { get; set; }
        public string actNorms { get; set; }
        public string devinNorms { get; set; }

        public bool CurrentVersion { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OrderType { get; set; }


    }

    public class STEListModel
    {
        public int STEId { get; set; }
        public string Email { get; set; }
        public Nullable<int> MsPhdType { get; set; }
        public string ProjectNumber { get; set; }
        public int SNo { get; set; }
        public string STEAppNo { get; set; }
        public string PIName { get; set; }
        public string Category { get; set; }
        public string CandidateName { get; set; }
        public Nullable<int> EmailRemaindarCount { get; set; }
        public bool isGovAgencyFund { get; set; }
        public bool isCommitmentRejection { get; set; }
        public bool SendOffer_f { get; set; }
        public string AppType { get; set; }
        public string Status { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> RoleId { get; set; }
        public bool Cancel_f { get; set; }
        public Nullable<DateTime> AppointmentStartDate { get; set; }
    }

    public class STESearchModel
    {
        public string STEAppNo { get; set; }
        public string PIName { get; set; }
        public string PIEmail { get; set; }
        public string CandidateName { get; set; }
        public string Category { get; set; }
        public string ProjectNumber { get; set; }
        public string Status { get; set; }
        public List<STEListModel> conList { get; set; }
        public int TotalRecords { get; set; }
        public string TypeofAppointment { get; set; }
    }

    #region Employee Master
    public class EmployeeMaster
    {
        public int SNo { get; set; }
        public string ApplicationNumber { get; set; }
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public int TypeCodeID { get; set; }

        public string EmployeeId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string strDateofBirth { get; set; }
        public string strDateofJoining { get; set; }
        public DateTime DateofBirth { get; set; }
        public DateTime DateofJoining { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string CurrentProject { get; set; }
        public string PresentAddress { get; set; }
        public string ProjectNumber { get; set; }
        public int gender { get; set; }

        public bool isHRABooking { get; set; }
        public bool isHRACancellation { get; set; }
        public bool isMaternity { get; set; }
        public string DesignationName { get; set; }
        public int TypeOfAppointment { get; set; }
        public int CSIRStaffMode { get; set; }
        public bool PaymentThroughAgency { get; set; }
        public bool RequestRelieving { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UserId { get; set; }
        public bool Cancel_f { get; set; }
        public Nullable<DateTime> AppointmentStartDate { get; set; }
    }
    public class SearchEmployeeModel
    {
        public int SearchInProjectId { get; set; }
        public string SearchInCategory { get; set; }
        public string SearchInPhone { get; set; }
        public string SearchInProjectNumber { get; set; }
        public string SearchInPAN { get; set; }
        public string SearchInName { get; set; }
        public string SearchInEmployeeId { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public List<EmployeeMaster> List { get; set; }
        public int TotalRecords { get; set; }
        public string SearchDesignationName { get; set; }
    }

    #endregion

    public class RCTViewDocumentsModel
    {
        public string PIName { get; set; }
        public string Name { get; set; }
        public string EmployeeID { get; set; }
        public List<OtherDocModel> OtherDocModel { get; set; }

        public List<CheckListEmailModel> DeviationList { get; set; }
    }

    public class RCTViewDocumentListModel
    {
        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentPath { get; set; }
        public string FormDate { get; set; }
        public string ToDate { get; set; }
    }
    public class OtherDocModel
    {
        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentPath { get; set; }
        public string FormDate { get; set; }
        public string ToDate { get; set; }
        public string DocumentCatecory { get; set; }
        public List<RCTViewDocumentListModel> DocumentList { get; set; }
        public HttpPostedFileBase Document { get; set; }
    }

    public class OrderModel
    {
        public int ApplicationID { get; set; }
        public int OrderID { get; set; }
        public string TypeCode { get; set; }
        public string EmployeeID { get; set; }
        public string Typeofappointment { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public decimal GateScore { get; set; }

        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool Fellowship { get; set; }
        public string isConsolidatePay { get; set; }
        public string staffcategory { get; set; }
        public string PhysicallyChanged { get; set; }
        public HttpPostedFileBase PILetter { get; set; }
        public string PILetterPath { get; set; }
        public string PILetterFileName { get; set; }
        public string PIJustificationRemarks { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public string OtherDesignation { get; set; }

        public Nullable<int> DesignationId { get; set; }
        public int Medical { get; set; }
        public string ApplicationNo { get; set; }
        [Display(Name = "Appointment start date")]
        public Nullable<DateTime> Appointmentstartdate { get; set; }
        [Display(Name = "Appointment End Date")]
        public Nullable<DateTime> AppointmentEndDate { get; set; }
        [Display(Name = "Salary")]
        public Nullable<decimal> Salary { get; set; }
        public Nullable<decimal> MedicalAmmount { get; set; }
        public bool isMedical { get; set; }
        public string IsGSTapplicable { get; set; }
        public Nullable<decimal> GST { get; set; }
        public bool isHRA { get; set; }
        public bool IsMsPhd { get; set; }
        public string PhdDetail { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public decimal HRAPercentage { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string FlowofMail { get; set; }
        public List<CheckListModel> CheckListDetail { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string IITMPensionerCSIRStaff { get; set; }
        public Recruitoldmastdetails OldPrjDetailModel { get; set; }
        public string Qualification { get; set; }
        public decimal ExperienceInDes { get; set; }
        public string Experience { get; set; }
        public Nullable<DateTime> WithdrawTillDate { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public bool isWithdrawCommitment { get; set; }

        public bool WithdrawnFullAmount { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public int RequestReference { get; set; }
        public int OrderType { get; set; }
        public string OrderTypestr { get; set; }

        public int TypeofappointmentId { get; set; }

        public string ReferenceNo { get; set; }
        public int SourceReferenceNumber { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }

        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> OfferDate { get; set; }
        public bool Rejoin { get; set; }
        public Nullable<DateTime> RejoinDate { get; set; }

        public bool Adminintif { get; set; }
        public string listf { get; set; }
        public int AdmininitView { get; set; }
        public string MedicalInclude { get; set; }
        public string ApplicationRcvDate { get; set; }
        public string AppointmentstrDate { get; set; }
        public string AppointmenttoDate { get; set; }
        public string IITMExperience { get; set; }
        public string FlowApprover { get; set; }
        public HttpPostedFileBase[] PIJustificationFile { get; set; }
        public List<STEJustificationDoc> JustificationDoc { get; set; }
        public bool isDADashboard { get; set; }
        public bool DA_f { get; set; }
        public string List_f { get; set; }
        public HttpPostedFileBase RejoiningLetter { get; set; }
        public string RejoiningLetterPath { get; set; }
        public string RejoiningLetterName { get; set; }
        public Nullable<int> SalaryCalcId { get; set; }
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<int> EmpSalutation { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> StatutoryId { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public Nullable<decimal> OldTotalCTC { get; set; }
        public Nullable<int> PhysicallyChallenged { get; set; }
        public Nullable<decimal> LWFAmount { get; set; }
        public string SendSalaryStructure { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<DateTime> OrderDate { get; set; }
        public Nullable<bool> PFBasic_f { get; set; }
        public Nullable<bool> EmployerIns_f { get; set; }
        public Nullable<bool> EmployerESIC_f { get; set; }
        public string PayType { get; set; }
        public Nullable<DateTime> ArrearOrDeductionTillDate { get; set; }
        public decimal ArrearOrDeductionAmount { get; set; }
        public string strArrearOrDeductionTillDate { get; set; }
        public Nullable<int> RequestedByPI { get; set; }
        public string AutoFillRequstedbyPI { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }
        public string ToMail { get; set; }
        public string CCMail { get; set; }
        public bool MailSent_f { get; set; }
        public bool InitByPI_f { get; set; }

        public string CommiteeMember1 { get; set; }
        public Nullable<int> CommiteeMemberId1 { get; set; }
        public string CommiteeMember2 { get; set; }
        public Nullable<int> CommiteeMemberId2 { get; set; }
        public string ChairpersonName { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
    }

    public class HRAOrderModel
    {
        public int ApplicationID { get; set; }
        public int OrderID { get; set; }
        public string TypeCode { get; set; }
        public string EmployeeID { get; set; }
        public string Typeofappointment { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool Fellowship { get; set; }
        public string staffcategory { get; set; }
        public string PhysicallyChanged { get; set; }
        public HttpPostedFileBase PILetter { get; set; }
        public string PILetterPath { get; set; }
        public string PILetterFileName { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public int Medical { get; set; }
        public string ApplicationNo { get; set; }
        [Display(Name = "Salary")]
        public Nullable<decimal> Salary { get; set; }
        public decimal MedicalAmmount { get; set; }
        public bool isMedical { get; set; }
        public bool isHRA { get; set; }
        public bool IsMsPhd { get; set; }
        public string PhdDetail { get; set; }
        public decimal HRA { get; set; }
        public decimal HRAPercentage { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string FlowofMail { get; set; }
        public string Status { get; set; }
        public string IITMPensionerCSIRStaff { get; set; }
        public Recruitoldmastdetails Appointmentdetails { get; set; }
        public string Qualification { get; set; }
        public decimal ExperienceInDes { get; set; }
        public string Experience { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string Remarks { get; set; }
        public int RequestReference { get; set; }
        public int OrderType { get; set; }
        public int TypeofappointmentId { get; set; }
        public string ReferenceNo { get; set; }
        public int SourceReferenceNumber { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public HttpPostedFileBase HRAForm { get; set; }
        public string HRAFormPath { get; set; }
        public string HRAFormName { get; set; }
        public Nullable<DateTime> ArrearOrDeductionTillDate { get; set; }
        public decimal ArrearOrDeductionAmount { get; set; }
        public string strArrearOrDeductionTillDate { get; set; }

        public int AddressProof { get; set; }
        public HttpPostedFileBase ProofAddress { get; set; }
        public string AddressProofPath { get; set; }
        public string AddressProofName { get; set; }
        public string HRAFromDate { get; set; }
        public string HRAToDate { get; set; }
        public bool isHRAFullTenure { get; set; }

        public bool Adminintif { get; set; }
        public bool HRAAdmf { get; set; }
        public string IITMExperience { get; set; }
        public string List_f { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }
    }


    public class AmendmentOrderModel
    {
        public int ApplicationID { get; set; }
        public int OrderID { get; set; }
        public string TypeCode { get; set; }
        public string EmployeeID { get; set; }
        public string Typeofappointment { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool Fellowship { get; set; }
        public string staffcategory { get; set; }
        public string PhysicallyChanged { get; set; }
        public HttpPostedFileBase PILetter { get; set; }
        public string PILetterPath { get; set; }
        public string PILetterFileName { get; set; }
        public string PIJustificationRemarks { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public int Medical { get; set; }
        public string ApplicationNo { get; set; }
        [Display(Name = "Salary")]
        public Nullable<decimal> Salary { get; set; }
        public decimal MedicalAmmount { get; set; }
        public bool isMedical { get; set; }
        public Nullable<decimal> GST { get; set; }
        public bool isHRA { get; set; }
        public bool IsMsPhd { get; set; }
        public string PhdDetail { get; set; }
        public decimal GateScore { get; set; }
        public decimal HRA { get; set; }
        public decimal HRAPercentage { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string Status { get; set; }
        public Recruitoldmastdetails Appointmentdetails { get; set; }
        public ProjectDetails ProjectDetails { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public bool isWithdrawCommitment { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public int RequestReference { get; set; }
        public int OrderType { get; set; }
        public string OrderTypestr { get; set; }

        public int TypeofappointmentId { get; set; }
        public string ReferenceNo { get; set; }
        public int SourceReferenceNumber { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> OfferDate { get; set; }
        public bool Adminintif { get; set; }
        public string IITMExperience { get; set; }
        public bool isDADashboard { get; set; }
        public Nullable<int> SalaryCalcId { get; set; }
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<int> EmpSalutation { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public Nullable<decimal> OldTotalCTC { get; set; }
        public Nullable<decimal> LWFAmount { get; set; }
        public string List_f { get; set; }
    }

    public class RelievingModel
    {
        public int ApplicationID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string TypeCode { get; set; }
        public string EmployeeID { get; set; }
        public string Typeofappointment { get; set; }
        public string Name { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The Application Receive Date field is required")]
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "Upload PI letter")]
        public HttpPostedFileBase PILetter { get; set; }
        public string PILetterPath { get; set; }
        public string PILetterFileName { get; set; }
        public Nullable<int> ProjectId { get; set; }
        [Display(Name = "Appointment End Date")]
        public Nullable<DateTime> AppointmentEndDate { get; set; }
        public string Status { get; set; }
        public Recruitoldmastdetails Appointmentdetails { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public bool isDisabledInput { get; set; }
        public int RequestReference { get; set; }
        public int OrderType { get; set; }
        public string ReferenceNo { get; set; }
        public int SourceReferenceNumber { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The Relieving Mode field is required")]
        public int RelievingMode { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The Relieving Date field is required")]
        public Nullable<DateTime> RelievingDate { get; set; }
        public int CommitmentOption { get; set; }
        public string ForenoonOrAfternoon { get; set; }
        public HttpPostedFileBase[] NODuesFile { get; set; }
        public string[] NODuesFilePath { get; set; }
        public string[] NODuesFileName { get; set; }
        public int AdmininitView { get; set; }
        public int list { get; set; }
        public bool Adminintif { get; set; }
        public Nullable<bool> Spcomerelieving_f { get; set; }
        //public Nullable<bool> CommitmentBalance { get; set; }
        public string List_f { get; set; }
        public bool InitByPI_f { get; set; }
        public string PIRemarks { get; set; }
        public string PINoDuesRemarks { get; set; }

    }

    public class StopaymentlosspayModel
    {
        public int ApplicationID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string TypeCode { get; set; }
        public string EmployeeID { get; set; }
        public string Typeofappointment { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string DateofBirth { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The application receive date is required")]
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public string ApplicationRevDate { get; set; }
        public bool ConsolidatedPay { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "Upload PI letter")]
        public HttpPostedFileBase PILetter { get; set; }
        public string PILetterPath { get; set; }
        public string PILetterFileName { get; set; }
        public string PIJustificationRemarks { get; set; }
        public string ApplicationNo { get; set; }
        public bool IsMsPhd { get; set; }
        public string Status { get; set; }
        public string IITMPensionerCSIRStaff { get; set; }
        public Recruitoldmastdetails Appointmentdetails { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public int RequestReference { get; set; }
        public Nullable<int> OrderType { get; set; }
        public int TypeofappointmentId { get; set; }
        public string ReferenceNo { get; set; }
        public int SourceReferenceNumber { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The From date field is required")]
        public Nullable<DateTime> FromDate { get; set; }
        public string FromDateStr { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The To date field is required")]
        public Nullable<DateTime> ToDate { get; set; }
        public string ToDateStr { get; set; }

        //[RequiredIf("Status", "Approved", ErrorMessage = "Please check rejoin")]
        public bool Rejoin { get; set; }
        [RequiredIf("Status", "Approved", ErrorMessage = "Please enter rejoining date")]
        public Nullable<DateTime> RejoinDate { get; set; }
        [RequiredIf("Status", "Approved", ErrorMessage = "Please enter rejoining letter")]
        public HttpPostedFileBase RejoiningLetter { get; set; }
        public string RejoiningLetterPath { get; set; }
        public string RejoiningLetterName { get; set; }

        [RequiredIf("OrderID", null, ErrorMessage = "The from meridiem is required")]
        public Nullable<int> FromMeridiem { get; set; }
        [RequiredIf("OrderID", null, ErrorMessage = "The to meridiem field is required")]
        public Nullable<int> ToMeridiem { get; set; }
        public string lblFromMeridiem { get; set; }
        public string lblToMeridiem { get; set; }
        //[RequiredIf("OrderID", null, ErrorMessage = "The signature field is required")]
        public Nullable<int> Signature { get; set; }
        public string lblSignature { get; set; }
        public bool Adminintif { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<DateTime> ArrearOrDeductionTillDate { get; set; }
        public decimal ArrearOrDeductionAmount { get; set; }
        public string strArrearOrDeductionTillDate { get; set; }
        public string List_f { get; set; }
        public Nullable<int> OrderRequestId { get; set; }
        public bool InitByPI_f { get; set; }
        public string PIRemarks { get; set; }

    }

    public class Recruitoldmastdetails
    {
        public string ProjectNumber { get; set; }
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectType { get; set; }
        public string SponsoringAgency { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectClosureDate { get; set; }
        public string AppointmentStartDate { get; set; }
        public DateTime AppointmentStartDt { get; set; }
        public string AppointmentClosureDate { get; set; }
        public string PIName { get; set; }
        public Nullable<int> PIId { get; set; }
        public string PICode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PIDepartmentCode { get; set; }
        public string PIDepartmentName { get; set; }
        public string PIDesignation { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public int Medical { get; set; }
        public string MedicalText { get; set; }
        public decimal MedicalAmmount { get; set; }
        public Nullable<decimal> GST { get; set; }
        public bool isHRA { get; set; }
        public decimal HRA { get; set; }
        public decimal HRAPercentage { get; set; }
        public decimal Salary { get; set; }
        public decimal EmployeeCTC { get; set; }
        public decimal CommitmentAmmount { get; set; }
        public string IsGSTapplicable { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        public string CommitmentNo { get; set; }
        public int CommitmentId { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }


    }

    public class RecruitCommitRequestModel
    {
        public int CommitmentRequestId { get; set; }
        public string ReferenceNumber { get; set; }
        public string CommitmentRequestNumber { get; set; }
        public string TypeofAppointment { get; set; }
        public string AppointmentTypeCode { get; set; }
        public string CandidateName { get; set; }
        public string CandidateDesignation { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<decimal> TotalSalaryAmount { get; set; }
        public Nullable<decimal> BasicPayAmount { get; set; }
        public Nullable<decimal> PrevBookedCommitAmount { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> RequestedAmount { get; set; }
        public Nullable<decimal> AllowedAmount { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<int> AllocationHeadId { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string CrtdTS { get; set; }
        public string UpdTS { get; set; }
        public Nullable<int> CrtdUserId { get; set; }
        public Nullable<int> UpdtUserId { get; set; }
        public string CommitmentNumber { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public Nullable<decimal> AddCommitmentAmount { get; set; }
        public int Reason { get; set; }
        public int LogTypeId { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpNumber { get; set; }
        public Nullable<int> CommitmentBookedId { get; set; }
        public string RequestType { get; set; }
        public string ActionStartDate { get; set; }
        public string ActionEndDate { get; set; }
        public Nullable<decimal> CommitmentBalanceAmount { get; set; }
        public string ApplicationType { get; set; }
        public Nullable<int> OrderTypeId { get; set; }
    }

    public class CommitReqstSearchModel
    {
        public string ApplNo { get; set; }
        public string AppointType { get; set; }
        public string PIName { get; set; }
        public string PIEmail { get; set; }
        public string CandidateName { get; set; }
        public string CandidateDesignation { get; set; }
        public string Category { get; set; }
        public string ProjectNumber { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<decimal> TotalSalaryAmount { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string Status { get; set; }
        public List<CommitmentRequestListModel> commitreqList { get; set; }
        public string RequestType { get; set; }
        public int TotalRecords { get; set; }
    }

    public class CommitmentRequestListModel
    {
        public int SNo { get; set; }
        public string EmpNumber { get; set; }
        public int CommitmentRequestId { get; set; }
        public string ReferenceNumber { get; set; }
        public string CommitmentRequestNumber { get; set; }
        public string TypeofAppointment { get; set; }
        public string AppointmentTypeCode { get; set; }
        public string CandidateName { get; set; }
        public string CandidateDesignation { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<decimal> TotalSalaryAmount { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<int> AllocationHeadId { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
        public string Status { get; set; }
        public string CrtdTS { get; set; }
        public string UpdTS { get; set; }
        public Nullable<int> CrtdUserId { get; set; }
        public Nullable<int> UpdtUserId { get; set; }
        public bool IsHRA_f { get; set; }
        public bool IsNewAppointment_f { get; set; }
        public bool IsExtension_f { get; set; }
        public bool IsEnhancement_f { get; set; }
        public bool IsChangeofProject_f { get; set; }
        public string OrderNumber { get; set; }
        public string CommitmentNumber { get; set; }
        public string RequestType { get; set; }

    }

    public class OrderListModel
    {
        public int SNo { get; set; }
        public string ApplicationNumber { get; set; }
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int MaternityOrderID { get; set; }

        public string CategoryName { get; set; }
        public string Designation { get; set; }
        public int TypeCodeID { get; set; }
        public string EmployeeId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string RelievingType { get; set; }
        public bool isGenarateRelieveOrder { get; set; }
        public bool isGenarateFinalSettlement { get; set; }
        public bool isGenarateServiceCertificate { get; set; }

        public bool isSubmittedNOC { get; set; }
        public string ProjectNumber { get; set; }
        public string TypeCategory { get; set; }
        public decimal Salary { get; set; }
        public bool isLossOfPay { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public int EmailRemaindarCount { get; set; }
        public int DueforTenureEnd { get; set; }
        public string PIName { get; set; }
        public string ProjectNo { get; set; }
        public Nullable<DateTime> AppointmentStartdate { get; set; }
        public Nullable<DateTime> AppointmentEnddate { get; set; }
        public Nullable<int> CodevalId { get; set; }
        public Nullable<int> OrderType { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string DepartmentName { get; set; }
        public bool Is_Clarified { get; set; }
        public bool Is_GovAgency { get; set; }
        public bool Is_CommitmentReject { get; set; }
        public string SendMailType { get; set; }
        public string TypeofAppointmentName { get; set; }
        public string EmployeeDateofBirth { get; set; }
        public Nullable<DateTime> DateofBirth { get; set; }
        public string EmployeeEmail { get; set; }
        public Nullable<int> stopmailId { get; set; }
        public bool Is_InitByPI { get; set; }
    }

    //public class ByteEmailAttachmentModel
    //{
    //    public byte[] dataByte { get; set; }
    //    public string displayName { get; set; }
    //    public string actualName { get; set; }
    //}

    public class SearchOrderModel
    {
        public string SearchInProjectNo { get; set; }
        public int SearchInProjectId { get; set; }
        public string SearchInCategory { get; set; }
        public string SearchInStatus { get; set; }
        public string SearchInDesignation { get; set; }
        public string SearchInProjectNumber { get; set; }
        public string SearchInName { get; set; }
        public string SearchInEmployeeId { get; set; }
        public List<OrderListModel> List { get; set; }
        public int TotalRecords { get; set; }
        public string relievingType { get; set; }
        public string TypeCategory { get; set; }
        public string SearchPIname { get; set; }
        public string SearchTypeofAppointment { get; set; }
        public string DepartmentName { get; set; }
    }

    public class ApplicationSearchListModel
    {
        public string ApplicationNo { get; set; }
        public string ApplicationType { get; set; }
        public string PIName { get; set; }
        public string PIEmail { get; set; }
        public string CondidateName { get; set; }
        public string Category { get; set; }
        public string ProjectNumber { get; set; }
        public string Status { get; set; }
        public List<ApplicationListModel> ApplicationList { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ApplicationListModel
    {
        public int SNo { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string ApplicationNumber { get; set; }
        public string Category { get; set; }
        public string ApplicationType { get; set; }
        public string CondidateName { get; set; }
        public string PIName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public Nullable<int> EmailRemaindarCount { get; set; }

        public string ProjectNumber { get; set; }
        public string Designation { get; set; }
        public string EmployeeNo { get; set; }

        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UserId { get; set; }
    }

    public class ProjectExtentionEnhmentSearchListModel
    {
        public string EmployeeNo { get; set; }
        public string ExtensionEnhCategory { get; set; }
        public string EmployeeName { get; set; }
        public string ProjectNumber { get; set; }
        public string ApplicationCategory { get; set; }
        public string Status { get; set; }
        public List<ProjectExtentionEnhmentListModel> ExtandEnhList { get; set; }
        public int TotalRecords { get; set; }
        public string DepartmentName { get; set; }
        public string SearchTypeofAppointmentName { get; set; }

    }

    public class ProjectExtentionEnhmentListModel
    {
        public int SNo { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeDept { get; set; }
        public string ExtensionEnhCategory { get; set; }
        public string EmployeeName { get; set; }
        public string ProjectNumber { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationCategory { get; set; }
        public string StrFrmDate { get; set; }
        public string StrtoDate { get; set; }
        public string Status { get; set; }
        public bool isCommitmentRejection { get; set; }
        public Nullable<bool> SendOffer_f { get; set; }
        public Nullable<bool> SentOffer_f { get; set; }
        public string TypeofAppointmentName { get; set; }
        public bool InitByPI_f { get; set; }
        public bool Cancel_f { get; set; }
    }

    public class SearchChangeofProjectModel
    {
        public string EmployeeNo { get; set; }
        public string EmployeeDept { get; set; }
        public string Category { get; set; }
        public string EmployeeName { get; set; }
        public string OldProjectNumber { get; set; }
        public string NewProjectNumber { get; set; }
        public List<ChangeofProjectListModel> Changelist { get; set; }
        public int TotalRecords { get; set; }
        public string Status { get; set; }
    }
    public class ChangeofProjectListModel
    {
        public int SNo { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string ApplicationCategory { get; set; }
        public int ApplicationId { get; set; }
        public string EmployeeNo { get; set; }
        public string Category { get; set; }
        public string EmployeeName { get; set; }
        public string OldProjectNumber { get; set; }
        public string NewProjectNumber { get; set; }
        public string Status { get; set; }
        public Nullable<bool> SendOffer_f { get; set; }
        public bool InitByPI_f { get; set; }
        public bool Cancel_f { get; set; }


    }

    //public class GenerateOrdersModel
    //{
    //    public string ApplicantName { get; set; }
    //    public string ApplicationNo { get; set; }
    //    public string OrderNo { get; set; }
    //    public string EmployeeNo { get; set; }
    //    public string PresentAddress { get; set; }
    //    public string Designation { get; set; }
    //    public ProjectDetails ProjectDetail { get; set; }
    //    public string ContactNumber { get; set; }
    //    public string Email { get; set; }
    //    public decimal Pay { get; set; }
    //    public decimal HRA { get; set; }
    //    public bool isConsolidatePay { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //    public string AmendmentFromDate { get; set; }
    //    public string AmendmentToDate { get; set; }
    //    public string OfferLetterDate { get; set; }
    //    public string RequestReceiveDate { get; set; }

    //    public string CommitmentNo { get; set; }
    //    public string RelievingDate { get; set; }
    //    public string RelievingDueDate { get; set; }
    //    public bool isDuplicate { get; set; }

    //}

    public class GenerateOrdersModel
    {
        public string ApplicantName { get; set; }
        public string ApplicationNo { get; set; }
        public string OrderNo { get; set; }
        public string EmployeeNo { get; set; }
        public string PresentAddress { get; set; }
        public string Designation { get; set; }
        public ProjectDetails ProjectDetail { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public decimal Pay { get; set; }
        public decimal HRA { get; set; }
        public Nullable<decimal> MedicalAmount { get; set; }
        public bool isConsolidatePay { get; set; }
        public string PayType { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AmendmentFromDate { get; set; }
        public string AmendmentToDate { get; set; }
        public string OfferLetterDate { get; set; }
        public string OrderDate { get; set; }
        public Nullable<bool> PaymentThroughAgency_f { get; set; }
        public Nullable<bool> MsPhd_f { get; set; }
        public string MsPhdType { get; set; }
        public string RollNumber { get; set; }
        public Nullable<int> Gender { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceOrder { get; set; }

        public string RequestReceiveDate { get; set; }

        public string CommitmentNo { get; set; }
        public string RelievingDate { get; set; }
        public string RelievingDueDate { get; set; }
        public Nullable<int> RelieveMode { get; set; }
        public bool isDuplicate { get; set; }

        public string applicationtype { get; set; }
        public string GSTApplicable { get; set; }
    }

    public class RCTCommitEmailModel : EmailModel
    {
        public string commitmentAmount { get; set; }
        public string crt_Name { get; set; }
        public string projectNo { get; set; }
        public string commitmentNo { get; set; }
        public string date { get; set; }
        public string RequestType { get; set; }
        public string TypeofAction { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public Nullable<decimal> salary { get; set; }
    }

    public class RCTPopupModel
    {
        public string EmployeeID { get; set; }
        public string CandidateName { get; set; }
        public List<RCTPopupListModel> ExperienceList { get; set; }
        public string EmployeeMode { get; set; }
        public string Category { get; set; }
        public string DateOfBirth { get; set; }
        public string AsOnDate { get; set; }
        public List<AppointmenttypeExperienceModel> AppointmenttypeWiseExperience { get; set; }

    }
    public class RCTPopupListModel
    {
        public string ProjectNo { get; set; }
        public string Designation { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime EffectFromDate { get; set; }
        public DateTime EffectToDate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Salary { get; set; }
        public decimal Hra { get; set; }
        public decimal PFBasicWages { get; set; }
        public string OfferLetter { get; set; }
        public string OfficeOrder { get; set; }
        public string OfferLetterActualName { get; set; }
        public string OfficeOrderActualName { get; set; }
        public string OfferLetterPath { get; set; }
        public string OrderType { get; set; }
        public string JoiningReport { get; set; }
        public string JoiningReportActualName { get; set; }
        public string Status { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public string TypeofAppointment { get; set; }
        public bool isExtended { get; set; }
        public bool isUpdated { get; set; }
        public int AppId { get; set; }
        public string AppType { get; set; }
        public int OrderId { get; set; }
        public bool SendOfficeOrder_f { get; set; }
        public bool SendOrder_f { get; set; }
        public bool SendOfferLetter_f { get; set; }
        public string CommitmentNumber { get; set; }
        public string SalaryLevel { get; set; }

    }

    public class ViewStaffAllocationListModel
    {
        public string Category { get; set; }
        public int NoOfStaffs { get; set; }
        public int AllottedStaffs { get; set; }
        public int StaffVacancy { get; set; }
    }
    public class AllocatedStaffListModel
    {
        public string Designation { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Salary { get; set; }
        public decimal HRA { get; set; }
        public string EmployeeNo { get; set; }
    }
    public class ViewStaffAllocationModel
    {
        public List<ViewStaffAllocationListModel> StaffAllocationList { get; set; }
        public List<AllocatedStaffListModel> AllocatedStaffList { get; set; }

    }
    //public class RelieveCertificateModel
    //{
    //    public string HRName { get; set; }
    //    public string EmployeeID { get; set; }
    //    public string CandidateName { get; set; }
    //    public string AgencyName { get; set; }
    //    public string Designation { get; set; }
    //    public string OrderNo { get; set; }
    //    public string PIName { get; set; }
    //    public string Department { get; set; }
    //    public string RelieveDate { get; set; }
    //    public decimal SalaryPerMonth { get; set; }
    //    public string ProjectTitle { get; set; }
    //    public List<SevicesListModel> ExperienceList { get; set; }
    //    public string InitialGender { get; set; }
    //    public bool isDuplicate { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //    public bool isForenoon { get; set; }
    //}





    public class RelieveCertificateModel
    {
        public string HRName { get; set; }
        public string EmployeeID { get; set; }
        public string CandidateName { get; set; }
        public string AgencyName { get; set; }
        public string Designation { get; set; }
        public string OrderNo { get; set; }
        public string PIName { get; set; }
        public string Department { get; set; }
        public string RelieveDate { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public string ProjectTitle { get; set; }
        public List<SevicesListModel> ExperienceList { get; set; }
        public string InitialGender { get; set; }
        public bool isDuplicate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool isForenoon { get; set; }
        public string Paytype { get; set; }
        public string HisHer { get; set; }
        public string HeShe { get; set; }
        public string HimHer { get; set; }

        public Nullable<int> Gender { get; set; }
    }

    //public class RelievingSeviceCertificateModel
    //{
    //    public string HRName { get; set; }
    //    public string EmployeeID { get; set; }
    //    public string CandidateName { get; set; }
    //    public string AgencyName { get; set; }
    //    public string Designation { get; set; }
    //    public string OrderNo { get; set; }
    //    public string PIName { get; set; }
    //    public string Department { get; set; }
    //    public string RelieveDate { get; set; }
    //    public decimal SalaryPerMonth { get; set; }
    //    public string FromDate { get; set; }
    //    public string ToDate { get; set; }
    //    public string ProjectTitle { get; set; }
    //    public List<RelievingSeviceListModel> ExperienceList { get; set; }
    //    public bool isForenoon { get; set; }
    //    public string InitialGender { get; set; }
    //    public string EmployeeMode { get; set; }
    //    public string DateOfBirth { get; set; }
    //    public string AsOnDate { get; set; }
    //    public bool isDuplicate { get; set; }
    //    public string OfficeOrderPath { get; set; }
    //    public List<AppointmenttypeExperienceModel> AppointmenttypeWiseExperience { get; set; }

    //}

    public class SevicesListModel
    {
        public string ProjectNo { get; set; }
        public string Designation { get; set; }
        public int DesignationId { get; set; }

        public Nullable<DateTime> EffectiveFrom { get; set; }
        public Nullable<DateTime> EffectiveTo { get; set; }
        public Nullable<Decimal> Basic { get; set; }
        public Nullable<Decimal> HRA { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal Salary { get; set; }
        public string EmployeeDepartmentname { get; set; }
    }
    public class AppointmenttypeExperienceModel
    {
        public int Years { get; set; }
        public string TypeofAppointment { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public DateTime[] effectFrom { get; set; }
        public DateTime[] effectTo { get; set; }
    }

    #region Outsourcing
    public class OtherDetailModel
    {
        public Nullable<int> OtherDetailId { get; set; }
        public string OtherNames { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public HttpPostedFileBase OtherDetailFile { get; set; }
        public string OtherDetailFilePath { get; set; }
        public string OtherDetailFileName { get; set; }
        public string Remarks { get; set; }
        public string Description { get; set; }
        public bool Verify { get; set; }

    }
    public class SalaryCalcModel
    {
        public Nullable<int> SalaryCalcId { get; set; }
        public string EmployeeType { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }

        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public HttpPostedFileBase OtherDetailFile { get; set; }
        public string OtherDetailFilePath { get; set; }
        public string OtherDetailFileName { get; set; }
        public string Remarks { get; set; }
        public string Description { get; set; }
        public bool Verify { get; set; }

    }
    #endregion

    #region Common Employee Details
    public class CommonEmployeeDetails
    {
        public Nullable<int> ProfessionalId { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string VendorName { get; set; }
        public string aadharnumber { get; set; }
        public string PAN { get; set; }
        public string DateofBirth { get; set; }
        public string CandidatePhoto { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<int> Gender { get; set; }
        public Nullable<int> Caste { get; set; }
        public string ContactNumber { get; set; }
        public string AlternativeContactNumber { get; set; }
        public string EmergencyContactno { get; set; }
        public string Email { get; set; }
        public string PresentAddress { get; set; }
        public bool isSameasPermanentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public Nullable<int> BloodGroup { get; set; }
        public Nullable<int> BloodGroupRH { get; set; }
        public Nullable<int> Nationality { get; set; }
        public string PhysicallyChallenged { get; set; }
        public string RelatedIITMadras { get; set; }
        public Nullable<int> IITMPensionerCSIRStaff { get; set; }
        public bool MsPhd { get; set; }
        public Nullable<int> MsPhdType { get; set; }
        public string PhdDetail { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public bool IsGateScore { get; set; }
        public Nullable<decimal> GateScore { get; set; }
        public List<STEEducationModel> EducationDetail { get; set; }
        public List<STEExperienceModel> ExperienceDetail { get; set; }
        public List<EducationModel> ConEductiondetail { get; set; }
        public List<ExperienceModel> ConExpereience { get; set; }
        public List<OtherDetailModel> OtherDetail { get; set; }
    }
    //public class RCTOfficeOrderModel
    //{
    //    public string EmployeeId { get; set; }
    //    public string ApplicationNo { get; set; }
    //    public string Name { get; set; }
    //    public string Nameoftheguardian { get; set; }
    //    public string Appointmentstartdate { get; set; }
    //    public string AppointmentEndDate { get; set; }
    //    public string ContactNumber { get; set; }
    //    public ProjectDetails ProjectDetail { get; set; }
    //    public string Email { get; set; }
    //    public bool ConsolidatedPay { get; set; }
    //    public int ProjectId { get; set; }
    //    public string Designation { get; set; }
    //    public string OfficeOrderDate { get; set; }
    //    public decimal BasicPay { get; set; }
    //    public decimal HRA { get; set; }

    //    public DateTime OfferDate { get; set; }
    //    public string OfferletterDate { get; set; }
    //    public string TypeofAppointment { get; set; }

    //}

    public class RCTOfficeOrderModel
    {
        public string EmployeeId { get; set; }
        public string ApplicationNo { get; set; }
        public string Name { get; set; }
        public string Nameoftheguardian { get; set; }
        public string Appointmentstartdate { get; set; }
        public string AppointmentEndDate { get; set; }
        public string ContactNumber { get; set; }
        public ProjectDetails ProjectDetail { get; set; }
        public string Email { get; set; }
        public bool ConsolidatedPay { get; set; }
        public string PayType { get; set; }

        public int ProjectId { get; set; }
        public string Designation { get; set; }
        public string OfficeOrderDate { get; set; }
        public string OfferLetterDate { get; set; }
        public string OfferLetterNumber { get; set; }
        public decimal BasicPay { get; set; }
        public decimal HRA { get; set; }
        public Nullable<decimal> MedicalAmount { get; set; }
        public Nullable<bool> PaymentThroughAgency_f { get; set; }
        public Nullable<bool> MsPhd_f { get; set; }
        public string MsPhdType { get; set; }
        public string GSTType { get; set; }
        
        public DateTime OfferDate { get; set; }
        public string OfferletterDate { get; set; }
        public string TypeofAppointment { get; set; }
        public Nullable<int> TypeofAppointmentId { get; set; }
        public string appCategory { get; set; }
        public string RollNumber { get; set; }

    }
    public class GenerateIDCardModel
    {
        public string EmployeeId { get; set; }
        public string ApplicationNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nameoftheguardian { get; set; }
        public string Appointmentstartdate { get; set; }
        public string AppointmentEndDate { get; set; }
        public string ContactNumber { get; set; }
        public ProjectDetails ProjectDetail { get; set; }
        public bool ConsolidatedPay { get; set; }
        public int Age { get; set; }
        public int Sex { get; set; }

        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectNumber { get; set; }

        public string CantidateSignatureFilePath { get; set; }
        public string PersonImagePath { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
    }
    #endregion

    #region VerficationApplicationCancel
    public class SearchVerificationList
    {
        public string CondidateName { get; set; }
        public string ApplicationNo { get; set; }
        public string PIName { get; set; }
        public string Status { get; set; }
        public List<ApplicationListModel> VerfList { get; set; }
        public int TotalRecords { get; set; }

    }
    #endregion

    #region report
    public class ComitteeApprovalDetailReportModel
    {
        public string CandidateName { get; set; }
        public string PostRecommended { get; set; }
        public string ProjectNumber { get; set; }
        public string Category { get; set; }
        public int ApplicationId { get; set; }
        public DateTime AppointmentStartdate { get; set; }
        public DateTime AppointmentEnddate { get; set; }
        public DateTime CRTD_TS { get; set; }
        public decimal BasicPay { get; set; }
        public string ApplicationType { get; set; }
        public string TypeofAppointment { get; set; }
        public string PIName { get; set; }
        public string CheckList { get; set; }
        public string CommiteeApproveName { get; set; }
    }
    #endregion

    #region OtherPaymentDeduction
    public class OtherPaymentDeductionModel
    {
        public Nullable<int> OTHPayDeductionId { get; set; }

        [Required]
        [Display(Name = "Employee Number")]
        public string EmployeeNo { get; set; }
        [Required]
        [Display(Name = "Employee Number")]
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> AppointmentType { get; set; }
        public string AppointmentTypeName { get; set; }
        public Nullable<int> UserId { get; set; }
        public string AppointmentStartDate { get; set; }
        public string AppointmentEndDate { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string DateOfBirth { get; set; }
        public Nullable<int> Age { get; set; }
        public string Gender { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string Agency { get; set; }
        public string Projecttype { get; set; }
        public string PIName { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectCloseDate { get; set; }
        public string PICode { get; set; }
        public string PIEmail { get; set; }
        public string PIContactNo { get; set; }
        public string PINo { get; set; }
        public string PIDepartmentCode { get; set; }
        public string PIDepartment { get; set; }
        public string Status { get; set; }
        public Nullable<int> SNo { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string OtherPaymentNo { get; set; }



        [Required]
        [Display(Name = "Month")]
        public string Month { get; set; }
        public HttpPostedFileBase OTHAttachement { get; set; }
        public string AttachementName { get; set; }
        public string AttachmentPath { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public List<OtherPaymentDeductionDetailModel> OTHDetail { get; set; }
        public string FrmDate { get; set; }
        public string Todates { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<DateTime> FrmDateEdit { get; set; }
        public Nullable<DateTime> ToDateEdit { get; set; }
        public bool IsNoCommitment { get; set; }
        public string DonotCommitment { get; set; }
        public Nullable<DateTime> CRTD_TS { get; set; }
        public string CreatedDate { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDesription { get; set; }
    }

    public class OtherPaymentDeductionDetailModel
    {
        public Nullable<int> OTHPayDeductionDetailId { get; set; }
        public Nullable<int> OtherType { get; set; }
        public Nullable<int> PaymentDeductionType { get; set; }
        public string OtherTypeName { get; set; }
        public string PaymentdecTypename { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public List<MasterlistviewModel> PaydecList { get; set; }
    }
    public class SearchOtherPaymentModel
    {
        public string SearchEMPNo { get; set; }
        public string ProjectNo { get; set; }
        public string Appointmenttype { get; set; }
        public Nullable<int> SearchOtherType { get; set; }
        public string SearchPaymentNo { get; set; }
        public string SearchStatus { get; set; }
        public int TotalRecords { get; set; }
        public string EmployeeName { get; set; }
        public List<OtherPaymentDeductionModel> Detail { get; set; }
    }
    #endregion

    #region ConsultantAppointmentModel

    public class ConsultantAppointmentModel
    {
        public Nullable<int> ConsultantAppointmentId { get; set; }

        public Nullable<int> TypeofappointmentId { get; set; }

        public Nullable<int> PaytypeId { get; set; }

        [Required]
        [Display(Name = "Professional")]
        public Nullable<int> ProfessionalId { get; set; }
        public HttpPostedFileBase PersonDocImage { get; set; }
        public HttpPostedFileBase Resume { get; set; }
        public HttpPostedFileBase FormDocument { get; set; }
        public string PersonDocPath { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Name of the guardian")]
        public string Nameoftheguardian { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Contact Number can not exceed 10 degit")]
        public string ContactNumber { get; set; }
        public string PermanentAddress { get; set; }
        [Required]
        [Display(Name = "Temporary Address")]
        public string TemporaryAddress { get; set; }
        [Required]
        [Display(Name = "Date of Brith")]
        public Nullable<DateTime> DateofBirth { get; set; }
        public string DateBrith { get; set; }
        [Required]
        [Display(Name = "Age Limit")]
        [Range(18, 100)]
        public Nullable<int> Age { get; set; }
        [Required]
        [Display(Name = "Sex")]
        public Nullable<int> Sex { get; set; }
        [Required]
        [Display(Name = "Application Receive Date")]
        public Nullable<DateTime> ApplicationReceiveDate { get; set; }
        public string ApplicationRecvDate { get; set; }
        public Nullable<int> IITMPensionerId { get; set; }
        public Nullable<DateTime> ApplicatonEntryDate { get; set; }
        public string ApplicationEntryDt { get; set; }
        //public HttpPostedFileBase PIJustificationFile { get; set; }
        //public string PIJustificationDocument { get; set; }
        public bool PIJustification { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> MsPhdType { get; set; }
        public bool MsPhd { get; set; }
        public string PhdDetail { get; set; }

        [Display(Name = "Bank Account Number")]
        public string BankAccountNo { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public Nullable<int> BankId { get; set; }
        [Display(Name = "IFSCCode")]
        [RegularExpression("^[A-Za-z]{4}[a-zA-Z0-9]{7}$", ErrorMessage = "Invalid IFSC Code")]
        [MaxLength(11)]
        public string IFSCCode { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [RequiredIf("AadharNumber", null, ErrorMessage = "Please enter PAN Number")]
        [Display(Name = "PAN Number")]
        public string PANNo { get; set; }
        [RequiredIf("PANNo", null, ErrorMessage = "Please enter aadhaar number")]
        [Range(100000000000, 999999999999, ErrorMessage = "Aadhaar number should not exceed 12 characters")]
        public string AadharNumber { get; set; }
        [Display(Name = "Project Number")]
        public Nullable<int> ProjectId { get; set; }
        [Display(Name = "Project Number")]
        public string ProjectNumber { get; set; }
        public string Designation { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> DesignationId { get; set; }
        [Required]
        [Display(Name = "Appointment start date")]
        public Nullable<DateTime> Appointmentstartdate { get; set; }
        [Required]
        [Display(Name = "Appointment End Date")]
        public Nullable<DateTime> AppointmentEndDate { get; set; }
        public string AppointMentStrDate { get; set; }
        public string AppointMentEdDate { get; set; }
        [Required]
        [Display(Name = "Salary")]
        public Nullable<decimal> Salary { get; set; }
        public bool GSTapplicableValid
        {
            get
            {
                return (this.GSTapplicable == 1 || this.GSTapplicable == 2);
            }
        }
        [Required]
        [Display(Name = "GST applicable")]
        public Nullable<int> GSTapplicable { get; set; }
        [RequiredIf("GSTapplicableValid", true, ErrorMessage = "GST requried")]
        [Range(0, 100, ErrorMessage = "GST should be between 0 and 100")]
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        [Required]
        [Display(Name = "From Type")]
        public Nullable<int> FormId { get; set; }
        public string IsGSTapplicable { get; set; }

        public string GSTapplicabletext { get; set; }
        public string PIJustificationRemarks { get; set; }
        public HttpPostedFileBase[] PIJustificationFile { get; set; }

        //[Display(Name="Meeting Date")]
        //public Nullable<DateTime> MeetingDate { get; set; }
        [Required]
        [Display(Name = "IITMPensioner")]
        public Nullable<int> IITMPensionerorCSIRStaff { get; set; }
        public string PPONo { get; set; }
        public Nullable<int> CSIRStaff { get; set; }
        public string PensionerCSIRStaff { get; set; }
        public string IsICSRStaff { get; set; }
        public string Comments { get; set; }
        public string Note { get; set; }
        public string CommiteeMember { get; set; }
        public Nullable<int> CommiteeMemberId { get; set; }
        public string CommiteeMembers { get; set; }
        public Nullable<int> CommiteeMembersId { get; set; }
        public string ChairpersonName { get; set; }
        public Nullable<int> ChairpersonNameId { get; set; }
        public string FlowofMail { get; set; }
        public Nullable<int> UserId { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Invalid data entry")]
        public string EmployeeWorkplace { get; set; }
        public List<EducationModel> EducationDetail { get; set; } = new List<EducationModel>();
        public List<ExperienceModel> ExperienceDetail { get; set; } = new List<ExperienceModel>();
        public List<CheckListModel> CheckListDetail { get; set; } = new List<CheckListModel>();
        public List<PIJustificationModel> PIJustificationDocDetail { get; set; } = new List<PIJustificationModel>();
        public List<OtherDocModel> OtherDocList { get; set; }
        public Nullable<int> SNo { get; set; }
        public string ConsultantAppNo { get; set; }
        public string PIName { get; set; }
        public string Status { get; set; }
        public string ApplicationNumber { get; set; }
        public string Category { get; set; }
        public string CondidateName { get; set; }
        public Nullable<int> EmailRemaindarCount { get; set; }
        [RegularExpression("^([\\w+-.%]+@[\\w-.]+\\.[A-Za-z]{2,6},?)+$", ErrorMessage = "Invalid CC Mail Example:abc@mail.com,abx@mail.in,abz@mail.com")]
        public string bcc { get; set; }
        public bool bccSaved { get; set; }
        public bool isDraftbtn { get; set; }
        public ProjectDetails ProjectDetailsModel { get; set; }
        public string QualificationDetail { get; set; }
        public string Experience { get; set; }
        public List<CONNotes> Notes { get; set; }
        public Nullable<DateTime> OfferLetterDate { get; set; }
        public Nullable<DateTime> ActualDate { get; set; }
        public Nullable<int> EmployeeTypeCatecory { get; set; }
        public bool isVerifiedGovProof { get; set; }
        public Nullable<int> GovProof { get; set; }
        public bool isSameasPermanentAddress { get; set; }
        //[Required]
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Emergency Contact No")]
        public string EmergencyContactNo { get; set; }
        public string AlternativeContactNumber { get; set; }
        public HttpPostedFileBase JoiningReport { get; set; }
        public string JoiningReportPath { get; set; }
        public string JoiningReportFileName { get; set; }
        public string VerificationRemarks { get; set; }
        public string DepartmentName { get; set; }
        public string OfferDate { get; set; }
        public HttpPostedFileBase CantidateSignature { get; set; }
        public string CantidateSignatureFilePath { get; set; }
        public string CantidateSignatureFileName { get; set; }
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid To Mail")]
        public string ToMail { get; set; }
        public string EmployeeId { get; set; }
        public bool Adminintif { get; set; }
        public string EmployeeType { get; set; }

        public string isConsolidatePay { get; set; }
        public string OldEmployeeNumber { get; set; }
        public string OldEmpId { get; set; }
        public string NIDNumber { get; set; }
        public string FlowApprover { get; set; }
        public bool isCommitmentRejection { get; set; }
        public string StaffCategory { get; set; }
        public List<string> PIJustificationCommands { get; set; }
        public string ResumeFileName { get; set; }
        public string ResumeFilePath { get; set; }
        public string FormFileName { get; set; }
        public string FormFilePath { get; set; }
        public string PayType { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<DateTime> OfferActualDate { get; set; }
        public Nullable<DateTime> JoiningDate { get; set; }
        public string DateofJoining { get; set; }
        public string ApplicationType { get; set; }
        public bool SendOffer_f { get; set; }
        public string ApplicationRefNo { get; set; }
        public Nullable<int> RequestedByPI { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public string SalaryLevel { get; set; }
        public string SalaryLevelDescription { get; set; }
        public string AutoFillRequstedbyPI { get; set; }
        public string RequestedfromPI { get; set; }
        public string List_f { get; set; }
        public bool Cancel_f { get; set; }
    }

    public class PIJustificationModel
    {
        public Nullable<int> PIJustificationDocumentId { get; set; }
        public string PIJustificationDocument { get; set; }
        public string PIJustificationDocumentPath { get; set; }
        //public HttpPostedFileBase PIJustificationFile { get; set; }
    }

    public class CONNotes
    {
        public string PICommends { get; set; }
        public string HRNote { get; set; }

    }

    public class SearchCONVerificationModel
    {
        public string SearchInApplicationNo { get; set; }
        public string SearchInPAN { get; set; }
        public string SearchInName { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public Nullable<DateTime> MeetingDate { get; set; }
        public string SearchINStatus { get; set; }
        public List<ConsultantAppointmentModel> VerificationList { get; set; }
        public int TotalRecords { get; set; }
        public string DepartmentName { get; set; }
        public string Applicationtype { get; set; }

    }

    public class EducationModel
    {
        public Nullable<int> EducationId { get; set; }
        public string QualificationName { get; set; }

        [Display(Name = "Qualification")]
        public Nullable<int> QualificationId { get; set; }
        public string DisciplineName { get; set; }

        [Display(Name = "Decipline")]
        public Nullable<int> DisciplineId { get; set; }

        [Display(Name = "Institution")]
        public string Institution { get; set; }

        [Display(Name = "Year of Passing")]
        public Nullable<int> YearofPassing { get; set; }

        [Display(Name = "Mark Type")]
        public Nullable<int> MarkType { get; set; }
        public string strMarkType { get; set; }

        [Display(Name = "Marks")]
        [Range(0, 100, ErrorMessage = "Value should be between 0 and 100")]
        public Nullable<decimal> Marks { get; set; }

        [Display(Name = "Division Class Obtained")]
        public string DivisionClassObtained { get; set; }

        public HttpPostedFileBase Certificate { get; set; }
        public string Remarks { get; set; }
        public List<MasterlistviewModel> ddlList { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
    }

    public class ExperienceModel
    {
        public Nullable<int> ExperienceId { get; set; }

        [Display(Name = "Type")]
        public Nullable<int> ExperienceTypeId { get; set; }
        public string ExperienceTypeName { get; set; }

        [Display(Name = "Organisation")]
        public string Organisation { get; set; }
        public string DesignationautoComplete { get; set; }
        public string DesignationNames { get; set; }
        public Nullable<int> DesignationListId { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string EXPFromDate { get; set; }
        public string ExpToDate { get; set; }
        public Nullable<decimal> SalaryDrawn { get; set; }
        public HttpPostedFileBase ExperienceFile { get; set; }
        public string Remarks { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }

    }

    public class ConsultantSearchModel
    {
        public string ConsultantAppNo { get; set; }
        public string PIName { get; set; }
        public string PIEmail { get; set; }
        public string CondidateName { get; set; }
        public string Category { get; set; }
        public string ProjectNumber { get; set; }
        public string Status { get; set; }
        public List<ConsultantAppointmentModel> conList { get; set; }
        public int TotalRecords { get; set; }
    }

    #endregion

    public class SalryCalcPercentModel
    {
        public Nullable<int> StatutoryId { get; set; }
        public Nullable<decimal> EmployeePfPercent { get; set; }
        public Nullable<decimal> EmployerPfPercent { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> PhESICSlab { get; set; }
        public Nullable<decimal> GenESICSlab { get; set; }
        public Nullable<decimal> PFSlab { get; set; }
        public Nullable<decimal> ProfessionalTax { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> Insurance { get; set; }
        public Nullable<decimal> GSTPercentage { get; set; }
        public Nullable<decimal> LWFEmlyrAmount { get; set; }
    }


    #region Payroll Initiation

    public class PayrollInitiationModel
    {
        [Required]
        [Display(Name = "Salary Month")]
        public string SalaryMonth { get; set; }
        [Required]
        [Display(Name = "Salary Type")]
        public Nullable<int> SalaryType { get; set; }
        public Nullable<int> SalaryEmployeeCategory { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> FromInitDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> ToInitDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<DateTime> SalaryMonthDate { get; set; }
        public string Appointmenttype { get; set; }
        public Nullable<int> SNo { get; set; }
        public Nullable<int> PayrollId { get; set; }
        public string MonthStartDate { get; set; }
        public string MonthEndDate { get; set; }
        public string PayrollSalaryType { get; set; }
        public string SalaryStatus { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> PayrollmenuProcessId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorName { get; set; }
    }

    public class PayrollInitiationSearchModel
    {
        public string SearchSalaryMonth { get; set; }
        public string SearchSalaryType { get; set; }
        public string SearchSalaryStaus { get; set; }
        public Nullable<int> TotalRecords { get; set; }
        public List<PayrollInitiationModel> Payrolllist { get; set; }
    }
    #endregion

    #region RCTCommitmentDetails
    public class RCTCommitmentModel
    {

        public string RowCommitmentId { get; set; }
        public string CommitmentNumber { get; set; }
    }
    public class EmployeeBasicDetails
    {
        public string EmployeeName { get; set; }
        public string DesigantionName { get; set; }
        public string EmployeeNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string CommitmentNumber { get; set; }
        public List<RCTCommitmenttransactionDetails> CommitmentDetais { get; set; }
        public decimal TotalCommitmentAmount { get; set; }
        public decimal TotalSalaryAmount { get; set; }
        public decimal AvilableBalance { get; set; }
        public decimal CommitmentBalance { get; set; }
    }
    public class RCTCommitmenttransactionDetails
    {
        public string ApplicationType { get; set; }
        public string RequestType { get; set; }
        public string AllocationHead { get; set; }
        public decimal CommitmentAmount { get; set; }
    }



    #endregion


    public class CheckDeviationinputModel
    {
        public string Comments { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int>[] QualificationId { get; set; }
        public Nullable<int>[] DisciplineId { get; set; }
        public Nullable<decimal> SalaryAmount { get; set; }
        public Nullable<decimal> SalaryExceperience { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<int> AgeLimit { get; set; }
        public List<CheckListEmailModel> devChecklist { get; set; } = new List<CheckListEmailModel>();
    }

    #region OtherPaymentDeductionUpload
    public class OtherPaymentDeductionUploadModel
    {
        [Required]
        [Display(Name = "Month and Year")]
        public string MonthandYear { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> FromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> ToDate { get; set; }
        public HttpPostedFileBase template { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UploadId { get; set; }
        public Nullable<int> OTHUploadMasterId { get; set; }
        public string FormStrDate { get; set; }
        public string ToStrDate { get; set; }
        public Nullable<int> SNo { get; set; }
        public string Status { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class ValidatePaymentDeduction
    {

        [Display(Name = "Month and Year")]
        public string MonthandYear { get; set; }

        [Display(Name = "From Date")]
        public Nullable<DateTime> FromDate { get; set; }

        [Display(Name = "To Date")]
        public Nullable<DateTime> ToDate { get; set; }
        public HttpPostedFileBase template { get; set; }

    }
    public class SearchOTHUploadMaster
    {
        public string MonthandYear { get; set; }
        public int TotalRecords { get; set; }
        public string Status { get; set; }
        public List<OtherPaymentDeductionUploadModel> TotalList { get; set; }
    }
    #endregion


    #region ProjectTypeCount
    public class RCTReportProjectModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }

    #endregion

    #region OSGDocumentUpload
    public class OutSourcingDocumentUpload
    {

        public Nullable<int> OSGid { get; set; }
        public string EmployeeNo { get; set; }
        public Nullable<int> AppointmentType { get; set; }
        public string AppointmentTypeName { get; set; }
        public List<OSGAttachmentModel> DocumentDetail { get; set; } = new List<OSGAttachmentModel>();
    }
    public class OSGAttachmentModel
    {
        public Nullable<int> DocumentId { get; set; }
        public string DocumentType { get; set; }
        public HttpPostedFileBase Document { get; set; }
    }
    #endregion

    #region Employee Portal Model
    public class EmployeeDetailModel
    {
        public string EmployeeNumber { get; set; }
        public string Employeename { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeCurrentStatus { get; set; }
        public string EmployeeStatus { get; set; }
    }
    #endregion
}