using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class ProjectReportService
    {
        public static List<ProjectReportViewModel> Getdeptwiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 orderby P.PIDepartment
                                 where (P.CrtdTS.Value.Month == model.Month && P.CrtdTS.Value.Year == model.year && P.PIName == D.UserId && P.PIDepartment == D.DepartmentCode && P.ProjectType == model.Projecttype && P.Status != "InActive" && P.ProjectSubType != 1)
                                 select new { D.DepartmentName, P.PIDepartment, P.ProjectNumber, P.ProjectTitle, P.SanctionOrderDate, P.SanctionValue, P.Remarks,P.ApplicableTax,P.BaseValue }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            decimal tattax = 0;
                            if (query[i].ApplicableTax != 0)
                            {
                                tattax = Convert.ToDecimal(query[i].BaseValue+(query[i].BaseValue/100) * (query[i].ApplicableTax));
                            }
                            else
                            {
                                tattax = query[i].BaseValue ?? 0;
                            }

                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid=(Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                Projectnumber = query[i].ProjectNumber,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionOrderDate = query[i].SanctionOrderDate ?? DateTime.Now,
                                SanctionValue = tattax,
                                Remarks = query[i].Remarks
                            });
                        }
                    }
                }
                return Getspon;
            }
            catch (Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }


        public static List<ProjectReportViewModel> Getfacultywiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 orderby D.FirstName
                                 where (P.CrtdTS.Value.Month == model.Month && P.CrtdTS.Value.Year == model.year && P.PIDepartment == D.DepartmentCode && P.ProjectType == model.Projecttype && P.PIName == D.UserId && P.Status != "InActive"&&P.ProjectSubType!=1)
                                 select new { D.DepartmentName, P.PIDepartment, P.PIName, D.FirstName, P.AgencyRegisteredName, P.ProjectTitle, P.SanctionValue,P.ApplicableTax,P.BaseValue }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            decimal tattax = 0;
                            if (query[i].ApplicableTax != 0)
                            {
                                tattax = Convert.ToDecimal(query[i].BaseValue + (query[i].BaseValue / 100) * (query[i].ApplicableTax));
                            }
                            else
                            {
                                tattax = query[i].BaseValue ?? 0;
                            }
                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid = (Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                PIName = query[i].FirstName,
                                AgencyRegisteredName = query[i].AgencyRegisteredName,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionValue = tattax,

                            });
                        }
                    }
                }
                return Getspon;
            }
            catch (Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }
        public static List<ProjectReportViewModel> Getagencywiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 from A in context.tblAgencyMaster
                                 orderby D.FirstName
                                 where (P.CrtdTS.Value.Month == model.Month && P.CrtdTS.Value.Year == model.year && P.PIDepartment == D.DepartmentCode && P.ProjectType == model.Projecttype && P.PIName == D.UserId && P.Status != "InActive"
                                 && P.SponsoringAgency == A.AgencyId&&P.ProjectSubType!=1)
                                 select new { D.DepartmentName, P.PIDepartment, P.PIName, D.FirstName, A.AgencyName, P.ProjectTitle, P.SanctionValue,P.ApplicableTax,P.BaseValue }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            decimal tattax = 0;
                            if (query[i].ApplicableTax != 0)
                            {
                                tattax = Convert.ToDecimal(query[i].BaseValue + (query[i].BaseValue / 100) * (query[i].ApplicableTax));
                            }
                            else
                            {
                                tattax = query[i].BaseValue ?? 0;
                            }
                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid = (Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                PIName = query[i].FirstName,
                                AgencyRegisteredName = query[i].AgencyName,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionValue = tattax,

                            });
                        }
                    }
                }
                return Getspon;
            }
            catch (Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }

        //public static List<ProjectProposalReport> GetProjectProposal(ProjectProposalReport model)
        //{
        //    try
        //    {
        //        List<ProjectProposalReport> getlist = new List<ProjectProposalReport>();
        //        using (var context = new IOASDBEntities())
        //        {
        //            var query = (from c in context.vw_ProjectProposal
        //                         where c.Crtd_TS >= model.FromDate && c.Crtd_TS <= model.ToDate
        //                         select new { }).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

    }
}