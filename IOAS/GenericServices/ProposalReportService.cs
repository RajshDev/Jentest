using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class ProposalReportService
    {
        public static List<ProposalReportViewModel>GetFundingnewproposal(ProposalReportViewModel model)
        {
            try
            {
                List<ProposalReportViewModel> list = new List<ProposalReportViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProposal
                                 from A in context.tblAgencyMaster
                                 from U in context.vwFacultyStaffDetails
                                 //from PI in context.tblPIDepartmentMaster
                                 //from U in context.tblUser
                                 where (P.SponsoringAgency == A.AgencyId && P.PI == U.UserId && P.Inputdate.Value.Month==model.Month&&P.Inputdate.Value.Year==model.Year)
                                 select new { U.FirstName, U.DepartmentName, P.ProposalTitle, A.AgencyCode, P.ProposalValue,P.Inputdate }).ToList();
                    if(query.Count>0)
                    {
                        for(int i=0;i<query.Count;i++)
                        {
                            list.Add(new ProposalReportViewModel() {
                                Department=query[i].DepartmentName,
                                PI=query[i].FirstName,
                                ProposalTitle=query[i].ProposalTitle,
                                ProposalValue=(Decimal)query[i].ProposalValue,
                                Inputdate= (DateTime)query[i].Inputdate,
                                SponsoringAgency=query[i].AgencyCode
                            });
                        }
                    }
                    return list;
                }
            }
            catch(Exception ex)
            {
                List<ProposalReportViewModel> list = new List<ProposalReportViewModel>();
                return list;
            }
        }
    }

}