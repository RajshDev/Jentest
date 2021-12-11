using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IOAS.DataModel;
using IOAS.Models;

namespace IOAS.GenericServices
{
    public class OfficeOrderService
    {
        public string ValidateOfficeOrder(OfficeOrderModel model)
        {
            try
            {
                string msg = "";

                if (model.OrderType == "")
                {
                    msg = "Order Type is required";
                }
                else if (model.OrderNumber == "")
                {
                    msg = "Order number is required";
                }
                else if (model.OrderFor == "")
                {
                    msg = "OrderFor is required";
                }
                else if (model.EmpNo == "" && model.EmployeeId == "")
                {
                    msg = "Please select employee";
                }
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }

        public string OfficeOrderIU(OfficeOrderModel model)
        {
            try
            {

                string msg = ValidateOfficeOrder(model);
                if (msg != "")
                {
                    return msg;
                }
                using (var context = new IOASDBEntities())
                {
                    tblOfficeOrder order = new tblOfficeOrder();
                    tblOfficeOrderDetail detail = new tblOfficeOrderDetail();
                    if (model.OrderId > 0)
                    {
                        var record = context.tblOfficeOrder.SingleOrDefault(of => of.OrderId == model.OrderId);

                        record.OrderType = model.OrderType;
                        record.OrderDate = model.OrderDate;
                        record.OrderFor = model.OrderFor;
                        record.EmployeeId = model.EmployeeId;
                        record.EmpNo = model.EmpNo;
                        record.Attachment = model.Attachment;
                        record.Remarks = model.Remarks;
                        record.StaffName = model.StaffName;
                        record.ProjectId = model.ProjectId;
                        record.OrderNumber = model.OrderNumber;
                        record.DepartmentId = model.DepartmentId;
                        record.UpdatedAt = System.DateTime.Now;
                        record.UpdatedBy = model.UpdatedBy;
                        context.SaveChanges();
                    }
                    else
                    {
                        order.OrderType = model.OrderType;
                        order.OrderDate = model.OrderDate;
                        order.OrderFor = model.OrderFor;
                        order.EmployeeId = model.EmployeeId;
                        order.EmpNo = model.EmpNo;
                        order.Attachment = model.Attachment;
                        order.Remarks = model.Remarks;
                        order.StaffName = model.StaffName;
                        order.ProjectId = model.ProjectId;
                        order.OrderNumber = model.OrderNumber;
                        order.DepartmentId = model.DepartmentId;
                        order.CreatedAt = System.DateTime.Now;
                        order.CreatedBy = model.CreatedBy;
                        context.tblOfficeOrder.Add(order);
                        model.OrderId = order.OrderId;
                        context.SaveChanges();
                    }
                    int orderDetailId = 0;
                    if (model.OrderDetail != null)
                    {
                        for (int i = 0; i < model.OrderDetail.Count; i++)
                        {
                            orderDetailId = model.OrderDetail[i].OrderDetailId;
                            if (orderDetailId > 0)
                            {
                                var orderDetail = context.tblOfficeOrderDetail
                                    .SingleOrDefault(det => det.OrderDetailId == orderDetailId && det.OrderId == model.OrderId);
                                orderDetail.SalaryHeadId = model.OrderDetail[i].SalaryHeadId;
                                orderDetail.Year = model.OrderDetail[i].Year;
                                orderDetail.RevisedSalary = model.OrderDetail[i].RevisedSalary;
                                orderDetail.ArrearFrom = model.OrderDetail[i].ArrearFrom;
                                orderDetail.ArrearPaymentDate = model.OrderDetail[i].ArrearPaymentDate;
                                orderDetail.Remarks = model.OrderDetail[i].Remarks;
                                orderDetail.IsCurrent = model.OrderDetail[i].IsCurrent;
                                orderDetail.Remarks = model.OrderDetail[i].Remarks;
                                orderDetail.UpdatedAt = System.DateTime.Now;
                                orderDetail.UpdatedBy = model.UpdatedBy;
                                context.SaveChanges();
                            }
                            else
                            {
                                detail.SalaryHeadId = model.OrderDetail[i].SalaryHeadId;
                                detail.Year = model.OrderDetail[i].Year;
                                detail.RevisedSalary = model.OrderDetail[i].RevisedSalary;
                                detail.ArrearFrom = model.OrderDetail[i].ArrearFrom;
                                detail.ArrearPaymentDate = model.OrderDetail[i].ArrearPaymentDate;
                                detail.Remarks = model.OrderDetail[i].Remarks;
                                detail.IsCurrent = model.OrderDetail[i].IsCurrent;
                                detail.Remarks = model.OrderDetail[i].Remarks;
                                detail.CreatedAt = System.DateTime.Now;
                                detail.CreatedBy = model.CreatedBy;
                                context.tblOfficeOrderDetail.Add(detail);
                                model.OrderDetail[i].OrderId = Convert.ToInt32(detail.OrderId);
                                model.OrderDetail[i].OrderDetailId = detail.OrderDetailId;
                                context.SaveChanges();
                            }
                        }
                    }

                    context.Dispose();
                }
                msg = "Saved successfully.";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        public PagedData<OfficeOrderModel> GetOfficeOrders(int userId, int page, int pageSize)
        {
            try
            {
                var model = new List<OfficeOrderModel>();
                var searchData = new PagedData<OfficeOrderModel>();
                int recordCount = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from order in context.tblOfficeOrder
                                 orderby order.OrderId
                                 select new
                                 {
                                     order.OrderId,
                                     order.OrderType,
                                     order.OrderDate,
                                     order.OrderFor,
                                     order.EmployeeId,
                                     order.EmpNo,
                                     order.Attachment,
                                     order.Remarks,
                                     order.StaffName,
                                     order.ProjectId,
                                     order.OrderNumber,
                                     order.DepartmentId,
                                     order.CreatedAt,
                                     order.CreatedBy
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new OfficeOrderModel
                            {
                                OrderId = records[i].OrderId,
                                OrderType = records[i].OrderType,
                                OrderDate = Convert.ToDateTime(records[i].OrderDate),
                                OrderFor = records[i].OrderFor,
                                EmployeeId = records[i].EmployeeId,
                                EmpNo = records[i].EmpNo,
                                Attachment = records[i].Attachment,
                                Remarks = records[i].Remarks,
                                StaffName = records[i].StaffName,
                                ProjectId = Convert.ToInt32(records[i].ProjectId),
                                OrderNumber = records[i].OrderNumber,
                                DepartmentId = Convert.ToInt32(records[i].DepartmentId),
                                CreatedAt = Convert.ToDateTime(records[i].CreatedAt),
                                CreatedBy = Convert.ToInt32(records[i].CreatedBy)
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

        public List<EmployeeDetailsModel> GetEmployeeList()
        {
            try
            {
                List<EmployeeDetailsModel> list = new List<EmployeeDetailsModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from emp in context.VWAppointInfo
                                   select  new EmployeeDetailsModel
                                   {
                                       EmployeeID = emp.EmployeeID,
                                       EmployeeName = emp.EmployeeName
                                   }).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public EmployeeDetailsModel GetEmployee(string EmployeeID)
        {
            try
            {
                EmployeeDetailsModel employee = new EmployeeDetailsModel();
                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryHead GetEmployeeSalaryHead()
        {
            try
            {
                SalaryHead SalaryHead = new SalaryHead();

                return SalaryHead;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<tblCodeControl> GetStatusType(string CodeName)
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

    }
}