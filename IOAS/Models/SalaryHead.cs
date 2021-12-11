using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class SalaryHead
    {
        public int SalaryHeadId { get; set; }
        public string OrderType { get; set; }
        public string EmployeeId { get; set; }
        public string EmpNo { get; set; }
        public  decimal Basic { get; set; }
        public  decimal MA { get; set; }
        public  decimal HRA { get; set; }
        public  decimal EPF { get; set; }
        public  decimal MedicalInsurance { get; set; }
        public  DateTime CreatedAt { get; set; }
        public  DateTime UpdatedAt { get; set; }
        public  int CreatedBy { get; set; }
        public  int UpdatedBy { get; set; }
    }
}