using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IOAS.Models
{
    public class OfficeOrderModel
    {
        public int OrderId { get; set; }
        public string OrderType { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }
        public string OrderFor { get; set; }
        public string EmployeeId { get; set; }
        public string EmpNo { get; set; }
        public string Attachment { get; set; }
        public string Remarks { get; set; }
        public string StaffName { get; set; }
        public int ProjectId { get; set; }
        public string OrderNumber { get; set; }
        public string BankAccount { get; set; }
        public string Designation { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime RelievingDate { get; set; }
        public decimal PayableAmount { get; set; }
        public int StaffID { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public  int UpdatedBy { get; set; }

        public List<OfficeOrderDetailModel> OrderDetail { get; set; }

        public EmployeeDetailsModel EmpInfo { get; set; }

        public SalaryHead SalaryHead { get; set; }
    }

    public class OfficeOrderDetailModel
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int SalaryHeadId { get; set; }
        public int Year { get; set; }
        public decimal RevisedSalary { get; set; }
        public DateTime ArrearFrom { get; set; }
        public DateTime ArrearPaymentDate { get; set; }
        public string Remarks { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}