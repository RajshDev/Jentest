using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace IOAS.Models
{
    public class EmailModel
    {
        public string toMail { get; set; }
        public List<string> cc { get; set; }
        public List<string> bcc { get; set; }
        public List<string> attachment { get; set; }
        public string name { get; set; }
        public string subject { get; set; }
        public string baseUrl { get; set; } = WebConfigurationManager.AppSettings["EmailImgSourse"];
        public List<ByteEmailAttachmentModel> attachmentByte { get; set; }
        public List<EmailAttachmentModel> attachmentlist { get; set; }

    }
    public class PaymentEmailModel : EmailModel
    {
        public string mode { get; set; }
        public string refNo { get; set; }
        public string batchNo { get; set; }
        public string beneficiary { get; set; }
        public int refId { get; set; }
        public string accountNumber { get; set; }
        public string bank { get; set; }
        public string ifsc { get; set; }
        public string date { get; set; }
        public string billNarration { get; set; }
        public string paymentNarration { get; set; }
        public string amount { get; set; }
        public Nullable<int> modeOfPayment { get; set; }
        public Nullable<int> bankId { get; set; }
        public Nullable<int> payeeTypeId { get; set; }
        public Nullable<int> payeeId { get; set; }
        public string rollNo { get; set; }
        public string TransactionTypeCode { get; set; }
        public string ttlInvoiceAmount { get; set; }
        public string poNumber { get; set; }
        public string tdsITSection { get; set; }
        public string tdsITAmount { get; set; }
        public string tdsGSTAmount { get; set; }
        public string[] projectNumber { get; set; }
        public string UTRNo { get; set; }
        public List<PaymentNotificationInvDetailModel> invDetails { get; set; }
    }
    public class PaymentNotificationInvDetailModel
    {
        public string invoiceNumber { get; set; }
        public string invoiceDate { get; set; }
        public string invoiceAmount { get; set; }
        public decimal invAmount { get; set; }
    }
    public class ReceiptEmailModel : EmailModel
    {
        public string invoiceNumber { get; set; }
        public string projectNumber { get; set; }
        public string typeofPayment { get; set; }
        public string invoiceDate { get; set; }

        public Nullable<int> projectId { get; set; }

        public Nullable<int> PIId { get; set; }
        public string NameofPI { get; set; }
        public string PIEmail { get; set; }
        public string AgencyRegName { get; set; }
        public string AgencyContactEmail { get; set; }
        public Nullable<decimal> ReceiptAmount { get; set; }
        public string ReceiptAmountinWords { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectType { get; set; }
    }
    public class NegativeBalEmailModel : EmailModel
    {
        public string amount { get; set; }
        public string totalamount { get; set; }
        public string projectno { get; set; }
        public string piname { get; set; }
        public List<PrevNegativeBalanceModel> prevlist { get; set; }

    }
    public class UAYEmailModel : EmailModel
    {
        public string commitmentAmount { get; set; }
        public string crt_Name { get; set; }
        public string projectNo { get; set; }
        public string commitmentNo { get; set; }
        public string date { get; set; }

    }
    public class ForeignRemitEmailModel : EmailModel
    {
        public string invoiceNumber { get; set; }
        public string projectNumber { get; set; }
        public string poNo { get; set; }
        public string commitmentNumber { get; set; }
        public string portfolioName { get; set; }
        public string typeofPayment { get; set; }
        public string beneficiary { get; set; }
        public string invoiceDate { get; set; }
        public string remittanceCurrency { get; set; }
        public string remittanceAmount { get; set; }
        public string remittanceNumber { get; set; }
        public Nullable<int> beneficiaryId { get; set; }
        public Nullable<int> projectId { get; set; }
        public Nullable<int> comittmentId { get; set; }
        public Nullable<int> typeofPaymentId { get; set; }
        public Nullable<int> PIId { get; set; }
        public string NameofPI { get; set; }
        public string PIEmail { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string PaymentDate { get; set; }
        public string FOTTRefNumber { get; set; }
        public string UserRefNumber { get; set; }
        public string BillofentryNumber { get; set; }
        public string BillofentryDate { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
    }
    public class NotePIModel : EmailModel
    {
        public string ApplicationNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectTitle { get; set; }
        public string PIName { get; set; }
        public string PersonName { get; set; }
        public string DesignationName { get; set; }
        public string AppointmentType { get; set; }
        public string ApplicationReceiveDate { get; set; }
        public string BasicPay { get; set; }
        public string GrossPay { get; set; }
        public string PFBasicwages { get; set; }
        public decimal Salary { get; set; }
        public string OrderDate { get; set; }
        public string AppointmentStartDate { get; set; }
        public string AppointmentEndDate { get; set; }
        public string ProjectEndDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Body { get; set; }

        public List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
        public string Comments { get; set; }
        public string Ordertype { get; set; }
        public string TypeofAppointment { get; set; }
        public string DAName { get; set; }
        public bool isMSPhd { get; set; }
        public string MSPhD { get; set; }
        public string Department { get; set; }
        public string FillFields { get; set; }
        public Nullable<int> MailType { get; set; }
        public bool isExistingEmployee { get; set; }
        public string Paytype { get; set; }
        public bool Ack_f { get; set; }
        public bool Send_f { get; set; }
        public bool HRAFullCancel_f { get; set; }
        public bool IsRelease { get; set; }
        public int RelievingMode { get; set; }
        public string Cancel_f { get; set; }
        public string OfficeOrderDate { get; set; }
        public bool IsDeviation { get; set; }
        public string SendSlryStruct { get; set; }
        public int OrderId { get; set; }
        public string Apptype { get; set; }
        public int AppId { get; set; }
        public string EmployeeNum { get; set; }
        public string PFEligiblity { get; set; }
        public string ESICEligiblity { get; set; }
        public string MobileNumber { get; set; }
        public string PhysicallyChallenged { get; set; }
        public string PrevFromDate { get; set; }
        public string PrevToDate { get; set; }
        public string RelievingDate { get; set; }
        public bool IsNoduesubmited { get; set; }
        public bool isProjectChange { get; set; }
        public bool isDesignationChange { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Offer_f { get; set; }
        public bool SalaryDiff_f { get; set; }
        public bool Extended_f { get; set; }
        public string Appointment { get; set; }
        public string Email { get; set; }
        public bool vendorMail_f { get; set; }
        public decimal IITMExperiencedes { get; set; }
        public string IITMExperience { get; set; }
        public string Reminder1 { get; set; }
        public string Reminder2 { get; set; }
        public bool DesignationDiff_f { get; set; }
        public bool OnlyDesignationDiff_f { get; set; }

    }

    public class ApplicationAckModel : EmailModel
    {
        public string ApplicatntName { get; set; }
        public string ApplicationNumber { get; set; }
        public string DesignationName { get; set; }
        public string ApplicationReceiveDate { get; set; }
        public string EffectiveDate { get; set; }
        public string Body { get; set; }
        public decimal BasicPay { get; set; }
        public string AppointmentType { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectTitle { get; set; }
        public string OrderType { get; set; }
        public List<ByteEmailAttachmentModel> attachmentByte { get; set; }
        public string SendSlryStruct { get; set; }
        public bool IsDeviation { get; set; }
        public List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
        public string PFBasicwages { get; set; }

    }
    public class ByteEmailAttachmentModel
    {
        public byte[] dataByte { get; set; }
        public string displayName { get; set; }
        public string actualName { get; set; }
    }

    public class EmailAttachmentModel
    {
        public string displayName { get; set; }
        public string actualName { get; set; }
    }
}