using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class CrystalReportService
    {
        public static List<ProposalRepotViewModels> Getproposal(ProposalRepotViewModels model)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {
                    List<ProposalRepotViewModels> datelistproposal = new List<ProposalRepotViewModels>();

                    if (model.keysearch != null)
                    {
                       var query = (from PR in context.tblProposal
                                 from PI in context.tblUser
                                 from PD in context.vwFacultyStaffDetails
                                 where (PR.InwardDate >= model.FromDate && PR.InwardDate <= model.ToDate && PI.UserId == PR.PI && PR.Department == PD.DepartmentCode
                                 && PR.ProjectType == model.ProjecttypeId) &&
                                 (PI.FirstName.Contains(model.keysearch) || PR.ProposalTitle.Contains(model.keysearch) || PD.DepartmentCode.Contains(model.keysearch) || PD.DepartmentName.Contains(model.keysearch) || PR.ProposalNumber.Contains(model.keysearch))
                                 select new { PR.ProposalNumber, PI.FirstName, PR.ProposalTitle, PD.DepartmentName, PR.InwardDate, PR.Crtd_TS, PR.DurationOfProjectYears, PR.DurationOfProjectMonths }).ToList();
                        
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                datelistproposal.Add(new ProposalRepotViewModels()
                                {

                                    Proposalnumber = query[i].ProposalNumber,
                                    PI = query[i].FirstName,
                                    ProposalTitle = query[i].ProposalTitle,
                                    Department = query[i].DepartmentName,
                                    InwardDate = (DateTime)query[i].InwardDate,
                                    Crtd_TS = (DateTime)query[i].Crtd_TS,
                                    Durationofprojectyears = (Int32)query[i].DurationOfProjectYears,
                                    Durationofprojectmonths = (Int32)query[i].DurationOfProjectMonths
                                });
                            }
                        }
                        return datelistproposal;
                    }
                    else
                    {
                        var query = (from PR in context.tblProposal
                                     from PI in context.tblUser
                                     from PD in context.vwFacultyStaffDetails
                                     where (PR.InwardDate >= model.FromDate && PR.InwardDate <= model.ToDate && PI.UserId == PR.PI && PR.Department == PD.DepartmentCode
                                     && PR.ProjectType == model.ProjecttypeId) 
                                     select new { PR.ProposalNumber, PI.FirstName, PR.ProposalTitle, PD.DepartmentName, PR.InwardDate, PR.Crtd_TS, PR.DurationOfProjectYears, PR.DurationOfProjectMonths }).ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                datelistproposal.Add(new ProposalRepotViewModels()
                                {

                                    Proposalnumber = query[i].ProposalNumber,
                                    PI = query[i].FirstName,
                                    ProposalTitle = query[i].ProposalTitle,
                                    Department = query[i].DepartmentName,
                                    InwardDate = (DateTime)query[i].InwardDate,
                                    Crtd_TS = (DateTime)query[i].Crtd_TS,
                                    Durationofprojectyears = (Int32)query[i].DurationOfProjectYears,
                                    Durationofprojectmonths = (Int32)query[i].DurationOfProjectMonths
                                });
                            }
                        }
                        return datelistproposal;
                    }
                }
            }
            catch (Exception ex)
            {
                List<ProposalRepotViewModels> datelistproposal = new List<ProposalRepotViewModels>();
                return datelistproposal;
            }
        }
    }
}