$(function () {
    //Declare Proposal List

    var getProjectDetailsURL = 'GetProjectList',
        getProposalDetailsURL = 'GetProposalList',
        EditProject = 'EditProject',
        DeleteProject = 'DeleteProject',
        //Searchproject='SearchProjectList',
        getsrchProposalDetailsURL = 'GetSearchProposalList';

    // Get Proposal List for modal Popup
    //var dbProposal;
    //GetProposallist();
     $("#gridProposalList").jsGrid({

        paging: true,
        pageIndex: 1,
        pageSize: 5,
        pageLoading: true,
        autoload: true,
        editing: false,
        filtering: true,

        controller: {

            loadData: function (filter) {
                //return $.grep(dbProposal, function (proposal) {

                //    return (!filter.ProposalNumber || proposal.ProposalNumber.toLowerCase().indexOf(filter.ProposalNumber.toLowerCase()) > -1)
                //   && (!filter.Projecttitle || proposal.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                //   && (!filter.NameofPI || proposal.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                //   && (!filter.EmpCode || proposal.EmpCode.toLowerCase().indexOf(filter.EmpCode.toLowerCase()) > -1)
                //    && (!filter.BasicValue || filter.BasicValue === proposal.BasicValue);
                //});
                var SearchData = [];
                SearchData = {
                    Projecttitle: filter.Projecttitle,
                    ProposalNumber: filter.ProposalNumber,
                    BasicValue: filter.BasicValue,
                    NameofPI: filter.NameofPI,
                    EmpCode: filter.EmpCode
                }
                filter.modal = SearchData;
                var deferred = $.Deferred();
                $.ajax({
                    type: "Post",
                    url: 'GetProposalList',
                    data: JSON.stringify(filter),
                    //data: { "ProjectType": Projecttype, "ProposalNumber": Proposalnumber, "FromSOdate": FromSODate, "ToSOdate": ToSODate, },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {

                        // dataProposal = result;
                        //$("#gridProjectList").jsGrid({ data: result });
                        var da = {
                            data: result.list,
                            itemsCount: result.TotalRecords
                        }
                        deferred.resolve(da);
                        //$('#projectopening').hide();
                        //$('#gridlist').show();
                        //$('#popupFilter').hide();

                    },
                    error: function (err) {
                        console.log("error : " + err);
                    }
                });
                return deferred.promise();
            }

        },

        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "50px" },
            { type: "number", name: "ProposalID", title: "ProposalId", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProposalNumber", title: "Proposal Number", editing: false, width: "70px" },
            { type: "number", name: "BasicValue", title: "Proposal Value", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "75px" },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "60px" },

       {
           name: "ProposalID",
           title: "Action",
           width: "60px",
           itemTemplate: function (value, item) {
               return $("<a>").attr("href", "javascript:void(0)").attr("class", "btn btn-primary").attr("value", "Select").text('Select').on("click", function () {
                   var proposalid = item.ProposalID;
                   var ProposalDetails = 'Loadproposaldetailsbyid';
                   //$('#Proposalidfield').css("display", "block");
                   //$('#Projectidfield').css("display", "none");
                   $.ajax({

                       type: "POST",
                       url: ProposalDetails,
                       data: { ProposalId: proposalid },
                       //contentType: "application/json; charset=utf-8",
                       //dataType: "json",

                       success: function (result) {
                           var selectProject = $("#MainProjectId");
                           selectProject.empty();
                           selectProject.append($('<option/>', {
                               value: "",
                               text: "Select Any"
                           }));
                           $.each(result.MainProjectList, function (index, itemData) {
                               selectProject.append($('<option/>', {
                                   value: itemData.id,
                                   text: itemData.name
                               }));
                           });
                           $("#Projecttypename").val(result.ProjectTypeName);
                           $('input[name="ProposalID"]').val(result.ProposalID);
                           $('#Prjcttype').val(result.ProjectType);
                           $('#ProjectType').val(result.ProjectType);
                           $('input[name="OverheadPercentage"]').val(result.OverheadPercentage);
                           $('#propslnum').val(result.ProposalNumber);
                           $('#HiddenProposalNumber').val(result.ProposalNumber);
                           $('#prjcttitle').val(result.Projecttitle);

                           $('#department').val(result.Department);
                           $('input[name="Department"]').val(result.Department);
                           var PIselect = $("#PI");
                           PIselect.empty();
                           $.each(result.MasterPIListDepWise, function (index, itemData) {

                               PIselect.append($('<option/>', {
                                   value: itemData.id,
                                   text: itemData.name
                               }));
                           });
                           var strDate = new Date(parseInt(result.TentativeStartdate.replace(/(^.*\()|([+-].*$)/g, '')));
                           var clsDate = new Date(parseInt(result.TentativeClosedate.replace(/(^.*\()|([+-].*$)/g, '')));
                           $("#TentativeStartdate").datepicker('setDate', strDate);
                           $("#TentativeClosedate").datepicker('setDate', clsDate);
                           //$('#TentativeStartdate').val(result.TentativestrtDate);
                           //$('#Startdate').val(result.strtDate);
                           //$('#TentativeClosedate').val(result.TentativeclsDate);
                           $('#PI').val(result.PIname);
                           $('#lblFinYear').text(result.CurrentFinYear);
                           $('#finyear').val(result.FinancialYear);
                           $('input[name="PIname"]').val(result.PIname);
                           PIselect.selectpicker('refresh');
                           $('#PIEmail').val(result.PIEmail);
                           $('input[name="PIEmail"]').val(result.PIEmail);
                           $('#PIDesignation').val(result.PIDesignation);
                           //$('#snctnvalue').val(result.Sanctionvalue);
                           $('.selectpicker').selectpicker('refresh');
                           $('#sponAgencyName').val(result.SponsoringAgencyName);
                           $('#Agncy').val(result.SponsoringAgency);
                           $('input[name="SponsoringAgency"]').val(result.SponsoringAgency);
                           $('#txtagencycode').val(result.AgencyCode);
                           $('input[name="AgencyCode"]').val(result.AgencyCode);
                           $('input[name="AgencyCodeid"]').val(result.AgencyCodeid);
                           $('#tanumbr').val(result.TAN);
                           $('#HiddenTAN').val(result.TAN);
                           $('#gstnumbr').val(result.GSTNumber);
                           $('#HiddenGST').val(result.GSTNumber);
                           $('#panumbr').val(result.PAN);
                           $('#HiddenPAN').val(result.PAN);
                           $('#txtcntctpersonaddress').val(result.Agencycontactpersonaddress);
                           $('#HiddenContactAddr').val(result.Agencycontactpersonaddress);
                           $('#txtcntctpersonmobile').val(result.Agencycontactpersonmobile);
                           $('#HiddenContactMobile').val(result.Agencycontactpersonmobile);
                           $('#txtcntctpersonemail').val(result.AgencycontactpersonEmail);
                           $('#HiddenContactEmail').val(result.AgencycontactpersonEmail);
                           $('#txtagncyconperson').val(result.Agencycontactperson);
                           $('#HiddenContactPerson').val(result.Agencycontactperson);
                           $('#txtagncyregaddrss').val(result.Agencyregaddress);
                           $('#HiddenRegAddr').val(result.Agencyregaddress);
                           $('#txtagncyregname').val(result.Agencyregname);
                           $('#HiddenRegName').val(result.Agencyregname);
                           
                           $('#Domainrelevant').val(result.Domainrelevant);
                           if (result.IsthereAnyArtificialIntelligence == "Yes") {
                               $('input[name=IsthereAnyArtificialIntelligence][value=' + result.IsthereAnyArtificialIntelligence + ']').attr('checked', 'checked');
                               $('#aiDetail').removeClass("dis-none");
                               $('#AIDescription').val(result.AIDescription);
                            }
                           else {
                               $('#aiDetail').addClass("dis-none");
                           }
                           $('#ProposalApprovedDate').val(result.PrpsalApprovedDate);
                           //$('input[name="PrpsalApprovedDate"]').val(result.PrpsalApprovedDate);
                           //$('#projectdurationyear').val(result.Projectdurationyears);
                           //$('#projectdurationmonths').val(result.Projectdurationmonths);
                           //$('input[name="Projectdurationyears"]').val(result.Projectdurationyears);
                           //$('input[name="Projectdurationmonths"]').val(result.Projectdurationmonths);
                           $('#ApplicableTaxHidden').val(result.ApplicableTax);
                           $('#applicabletax').val(result.ApplicableTax);
                           $('input[name="ApplicableTaxes"]').val(result.ApplicableTaxes);

                           $('#basevalue').val(result.BaseValue);
                           $('#BaseValueHidden').val(result.BaseValue);
                           //$('#SanctionvalueHidden').val(result.Sanctionvalue);
                           $('input[name="BaseValue"]').val(result.BaseValue);

                           if (result.ProjectType == 0) {
                               $('#prjcttypespon').hide();
                               $('#prjcttypeconsul').hide();
                               $("#JointDevelopment").hide();
                               $("#sponextrnlscheme").addClass('dis-none');
                               $('#indprjctcategory_1').css("display", "none");
                               $('#prjctdetails1').css("display", "none");
                               $('#prjctdetails2').css("display", "none");
                               $('#taxnumberdtls').hide();
                               $('#constaxdetails').hide();
                               $('#constaxservicemode').hide();
                               $('#reasonfornotax').hide();
                               $('#proofofnotax').hide();
                               $('#proofofnotaxlink').hide();
                               $('#indprojectstate').hide();
                               $('#indprojectlocation').hide();
                               $('#forignprojectcntry').hide();
                               $('#forignprojectstate').hide();
                               $('#forignprojectlocation').hide();
                           }
                           if (result.ProjectType == 1) {
                               $('#SOnumber').val(result.SanctionOrderNumber);
                               $('input[name="ProjectSubType"]').val(result.ProjectSubType);
                               $('#pjctsubtyp').val(result.ProjectSubType);
                               $("#JointDevelopment").hide();
                               if (result.ProjectSubType == 1) {
                                   $("#internalagency").removeClass('dis-none');
                                   $("#prjctdetails1").addClass('dis-none');
                                   //  $("#catgoryofprjct").addClass('dis-none');                                  
                                   $('#internalschemeagency').val(result.SponsoringAgency);
                                   $('input[name="InternalSchemeFundingAgency"]').val(result.SponsoringAgency);
                                   $('#indprjctcategory_1').css("display", "none");
                               }
                               if (result.ProjectSubType == 2) {
                                   $("#sponextrnlscheme").removeClass('dis-none');
                                   if (result.Categoryofproject == 1) {
                                       $("#sponextrnlschemecode").removeClass('dis-none');
                                       $('#schemecode').val(result.SchemeCode);
                                       $('input[name="SchemeCode"]').val(result.SchemeCode);
                                   }
                                   $("#prjctdetails1").removeClass('dis-none');
                                   $('#schemename').val(result.Schemename);
                                   $('input[name="Schemename"]').val(result.Schemename);
                                   $("#indprjctcategory_1").removeClass('dis-none');
                                   $('#projectcategory').val(result.Categoryofproject);
                                   $('input[name="Projectcatgry_Qust_1"]').val(result.Categoryofproject);
                               }
                               $('#prjcttypespon').show();
                               $('#prjcttypeconsul').hide();

                               $('#prjctdetails1').css("display", "block");
                               $('#prjctdetails2').css("display", "none");
                               $('#taxnumberdtls').hide();
                               $('#constaxdetails').hide();
                               $('#constaxservicemode').hide();
                               $('#indprojectstate').hide();
                               $('#indprojectlocation').hide();
                               $('#forignprojectcntry').hide();
                               $('#forignprojectstate').hide();
                               $('#forignprojectlocation').hide();
                               $('#reasonfornotax').hide();
                               $('#proofofnotax').hide();
                               $('#proofofnotaxlink').hide();
                           }
                           if (result.ProjectType == 2) {
                               $('#prjcttypeconsul').show();
                               $('#prjcttypespon').hide();
                               $("#JointDevelopment").show();
                               $('#indprjctcategory_1').css("display", "none");
                               $('#prjctdetails1').css("display", "none");
                               $('#prjctdetails2').css("display", "block");
                               $('#taxnumberdtls').hide();
                               $('#constaxdetails').show();
                               $('#constaxservicemode').show();
                               $('#indprojectstate').hide();
                               $('#indprojectlocation').hide();
                               $('#forignprojectcntry').hide();
                               $('#forignprojectstate').hide();
                               $('#forignprojectlocation').hide();
                               $('#reasonfornotax').hide();
                               $('#proofofnotax').hide();
                               $('#proofofnotaxlink').hide();
                               $('#conspjctsubtyp').val(result.Conssubtypename);
                               $('#Conssubtypename').val(result.Conssubtypename);
                               $('select[name="ConsProjectSubType"]').val(result.ConsProjectSubType);
                               $('#conspjctfndngctgry').val(result.ConsFundingcategoryname);
                               $('input[name="ConsFundingcategoryname"]').val(result.ConsFundingcategoryname);
                               $('select[name="ConsFundingCategory"]').val(result.ConsFundingCategory);

                               $('#gstnumbr').val(result.GSTNumber);
                               $('#HiddenGST').val(result.GSTNumber);
                               $('#HiddenTAN').val(result.TAN);
                               $('#HiddenPAN').val(result.PAN);
                               $('#tanumbr').val(result.TAN);
                               $('#panumbr').val(result.PAN);

                               $('#taxservice').val(result.constaxservice);
                               if (result.constaxservice == 2) {
                                   $('#indfndgagncystate').val(result.indfundngagncystate);
                                   $('#indfndngagncylocation').val(result.indfundngagncylocation);
                                   $('#IndTaxService').val('true');
                                   $("#taxregistatus").show();
                                   $("#indprojectstate").show();
                                   $("#indprojectlocation").show();
                                   $("#forignprojectcntry").hide();
                                   $("#forignprojectstate").hide();
                                   $("#forignprojectlocation").hide();
                               }

                               if (result.constaxservice == 1) {
                                   $('#indfndgagncystate').val(result.indfundngagncystate);
                                   $('#indfndngagncylocation').val(result.indfundngagncylocation);
                                   $('#IndTaxService').val('true');
                                   $("#indprojectstate").show();
                                   $("#indprojectlocation").show();
                                   $("#forignprojectcntry").hide();
                                   $("#forignprojectstate").hide();
                                   $("#forignprojectlocation").hide();
                               }
                               if (result.constaxservice == 3) {
                                   $('#forgnfndngagncycntry').val(result.forgnfndngagncycountry);
                                   $('#forgnfndngagncystate').val(result.forgnfundngagncystate);
                                   $('#forgnfndngagncylocation').val(result.forgnfundngagncylocation);

                                   $("#indprojectstate").hide();
                                   $("#indprojectlocation").hide();

                                   $("#forignprojectcntry").show();
                                   $("#forignprojectstate").show();
                                   $("#forignprojectlocation").show();
                               }
                               if (result.constaxservice == 0 || result.constaxservice == null) {
                                   $("#indprojectstate").hide();
                                   $("#indprojectlocation").hide();

                                   $("#forignprojectcntry").hide();
                                   $("#forignprojectstate").hide();
                                   $("#forignprojectlocation").hide();
                               }
                           }

                           var Docname = result.DocName;
                           var Attachname = result.AttachName;
                           var Doctype = result.DocType;
                           var Docpath = result.DocPath;
                           var DocID = result.Docid;

                           $.each(Docname, function (i, val) {
                               if (i == 0) {
                                   document.getElementsByName('DocType')[0].value = Doctype[0];
                                   document.getElementsByName('AttachName')[0].value = Attachname[0];
                                   document.getElementsByName('Docid')[0].value = DocID[0];
                                   document.getElementsByClassName('link1')[0].text = Docname[0];
                                   document.getElementsByClassName('HiddenDocName')[i].value = Docname[i];
                                   document.getElementsByClassName('HiddenDocPath')[i].value = Docpath[i];
                                   document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FSupportDocuments%2F";
                               } else {
                                   var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
                                   $(cln).find('.dis-none').removeClass('dis-none');
                                 $(cln).find('#firstremoveButton').addClass('dis-none');
                                   $(cln).insertBefore($('#DocdivContent').find('.p-b-sm'));
                                   document.getElementsByName('DocType')[i].value = Doctype[i];
                                   document.getElementsByName('AttachName')[i].value = Attachname[i];
                                   document.getElementsByName('Docid')[i].value = DocID[i];
                                   document.getElementsByClassName('link1')[i].text = Docname[i];
                                   document.getElementsByClassName('HiddenDocName')[i].value = Docname[i];
                                   document.getElementsByClassName('HiddenDocPath')[i].value = Docpath[i];
                                   document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FSupportDocuments%2F";
                               }

                               //    ShowDocument?file=c61479f3-93e7-48b4-84a2-7f970e02c432_UAY2017_proposal_template 1.pdf&filepath=~%2FContent%2FProposalDocuments%2F
                           });

                           var CoPIName = result.CoPIname;
                           var CoPIDep = result.CoPIDepartment;
                           var CoPIEmail = result.CoPIEmail;
                           //var PIListDepWise = result.PIListDepWise;

                           $.each(CoPIName, function (i, val) {
                               if (i == 0) {
                                   var select = $("#CoPI");
                                   select.empty();
                                   select.append($('<option/>', {
                                       value: '0',
                                       text: 'Select any'
                                   }));
                                   $.each(JSON.parse(piList), function (index, itemData) {
                                       if (itemData.code == CoPIDep[0]) {
                                           select.append($('<option/>', {
                                               value: itemData.id,
                                               text: itemData.name
                                           }));
                                       }
                                   });
                                   $('#CoPI').val(CoPIName[0]);
                                   select.selectpicker('refresh');
                                   document.getElementsByName('CoPIname')[0].value = CoPIName[0];
                                   document.getElementsByName('CoPIDepartment')[0].value = CoPIDep[0];
                                   document.getElementsByName('CoPIEmail')[0].value = CoPIEmail[0];
                               } else {
                                   var cln = $("#primaryDiv").clone().find("input").val("").end();
                                   var cloneElement = $("#primaryDiv").find('#CoPI').parent().clone();
                                   var dptPIs = []
                                   $.each(JSON.parse(piList), function (index, itemData) {
                                       if (itemData.code == CoPIDep[i]) {
                                           dptPIs.push(itemData)
                                       }
                                   });
                                   cln.find('#CoPI').parent().replaceWith(selectPickerApiElement($(cloneElement), "all", dptPIs, CoPIName[i]));
                                   $(cln).find('.dis-none').removeClass('dis-none');
                                   $(cln).insertBefore($('#divContent').find('.p-b-sm'))
                                   document.getElementsByName('CoPIname')[i].value = CoPIName[i];
                                   document.getElementsByName('CoPIDepartment')[i].value = CoPIDep[i];
                                   document.getElementsByName('CoPIEmail')[i].value = CoPIEmail[i];
                               }
                           });

                           var OtherInstCoPIName = result.OtherInstituteCoPIName;
                           var OtherInstCoPIRemarks = result.RemarksforOthrInstCoPI;
                           var OtherInstCoPIInst = result.CoPIInstitute;
                           var OtherInstCoPIDep = result.OtherInstituteCoPIDepartment;
                           var OtherInstCoPIID = result.OtherInstituteCoPIid;
                           if (OtherInstCoPIName != null) {
                               $.each(OtherInstCoPIName, function (i, val) {
                                   if (i == 0) {
                                       document.getElementsByName('OtherInstituteCoPIName')[0].value = OtherInstCoPIName[0];
                                       document.getElementsByName('CoPIInstitute')[0].value = OtherInstCoPIInst[0];
                                       document.getElementsByName('OtherInstituteCoPIDepartment')[0].value = OtherInstCoPIDep[0];
                                       document.getElementsByName('RemarksforOthrInstCoPI')[0].value = OtherInstCoPIRemarks[0];
                                       document.getElementsByName('OtherInstituteCoPIid')[0].value = OtherInstCoPIID[0];
                                   } else {
                                       var cln = $("#primaryotherinstcopiDiv").clone().find("input").val("").end();
                                       var cloneElement = $("#primaryotherinstcopiDiv").find('#OtherinstituteCoPI').parent().clone();
                                       $(cln).find('.dis-none').removeClass('dis-none');
                                       $(cln).insertBefore($('#divotherinstcopiContent').find('.p-b-sm'));
                                       document.getElementsByName('OtherInstituteCoPIName')[i].value = OtherInstCoPIName[i];
                                       document.getElementsByName('CoPIInstitute')[i].value = OtherInstCoPIInst[i];
                                       document.getElementsByName('OtherInstituteCoPIDepartment')[i].value = OtherInstCoPIDep[i];
                                       document.getElementsByName('RemarksforOthrInstCoPI')[i].value = OtherInstCoPIRemarks[i];
                                       document.getElementsByName('OtherInstituteCoPIid')[i].value = OtherInstCoPIID[i];
                                   }
                               });
                           }

                           calculateProjectTotal();
                       },

                       error: function (err) {
                           console.log("error1 : " + err);
                       }

                   });
                   $("#AddNewEntryModel").modal('hide');

               });

           }
       },

                {
                    type: "control", editButton: false, deleteButton: false, width: "25px",
                    _createFilterSwitchButton: function () {
                        return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                    }
                },
        ],
    });
    var dbProject;
    //GetProjectlist();
    var DateField = function (config) {
        jsGrid.Field.call(this, config);
    };

    DateField.prototype = new jsGrid.Field({
        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            return new Date(value).toDateString();
        },

        filterTemplate: function () {
            var now = new Date();
            this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), dateFormat: 'dd-MM-yy', changeYear: true });
            this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), dateFormat: 'dd-MM-yy', changeYear: true });
            return $("<div>").append(this._fromPicker).append(this._toPicker);
        },

        insertTemplate: function (value) {
            return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date() });
        },

        editTemplate: function (value) {
            return this._editPicker = $("<input>").datepicker().datepicker("setDate", new Date(value));
        },

        insertValue: function () {
            return this._insertPicker.datepicker("getDate").toISOString();
        },

        editValue: function () {
            return this._editPicker.datepicker("getDate").toISOString();
        },

        filterValue: function () {
            return {
                from: this._fromPicker.datepicker("getDate"),
                to: this._toPicker.datepicker("getDate")
            };
        }
    });

    jsGrid.fields.date = DateField;
    //Get Project List grid

    $("#gridProjectList").jsGrid({

        paging: true,
        pageIndex: 1,
        pageSize: 5,
        pageLoading: true,
        autoload: true,
        editing: false,
        filtering: true,

        controller: {

            loadData: function (filter) {

                //   return $.grep(dbProject, function (project) {

                //       return (!filter.ProjectNumber || project.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                //      && (!filter.Projecttitle || project.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                //      && (!filter.SponsoringAgencyName || project.SponsoringAgencyName.toLowerCase().indexOf(filter.SponsoringAgencyName.toLowerCase()) > -1)
                //      && (!filter.NameofPI || project.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                //      && (!filter.EmpCode || project.EmpCode.toLowerCase().indexOf(filter.EmpCode.toLowerCase()) > -1)
                //           && (!filter.Budget || filter.Budget === project.Budget)
                //       && (!filter.PrpsalApprovedDate.from || new Date(project.PrpsalApprovedDate) >= filter.PrpsalApprovedDate.from)
                //&& (!filter.PrpsalApprovedDate.to || new Date(project.PrpsalApprovedDate) <= filter.PrpsalApprovedDate.to);
                //});
                var Projecttype = $('#srchPrjcttype').val();
                //var Proposalnumber = $('#proposalnumber').val();
                var Projectnumber = $('#projectnumber').val();
                var PIName = $('#srchPIName').val();
                //  var SearchBy = $('input[name^="SearchField.SearchBy"]:checked').val();
                var FromSODate = $('#FromSODate').val() || null;
                var ToSODate = $('#ToSODate').val() || null;
                var FromDate = $('#FromDate').val() || null;
                var ToDate = $('#ToDate').val() || null;
               
                var SearchData = [];
                SearchData = {
                    ProjectType: Projecttype,
                    EFProjectNumber: Projectnumber,
                    PIName: PIName,
                    FromSODate: FromSODate,
                    ToSODate: ToSODate,
                    FromDate: FromDate,
                    ToDate: ToDate,
                    ProjectNumber: filter.ProjectNumber,
                    ProjectTitle: filter.Projecttitle,
                    AgencyName: filter.SponsoringAgencyName,
                    NameOfPI: filter.NameofPI,
                    PICode: filter.EmpCode,
                    BudgetValue: filter.Budget,
                    Status: filter.Status,
                    SanctionNumber: filter.SanctionOrderNumber
                }

                filter.model = SearchData;

                var deferred = $.Deferred();

                $.ajax({
                    type: "Post",
                    url: 'SearchProjectList',
                    data: JSON.stringify(filter),
                    //data: { "ProjectType": Projecttype, "ProposalNumber": Proposalnumber, "FromSOdate": FromSODate, "ToSOdate": ToSODate, },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {

                        // dataProposal = result;
                        //$("#gridProjectList").jsGrid({ data: result });
                        var da = {
                            data: result.Data,
                            itemsCount: result.TotalRecords
                        }
                        deferred.resolve(da);
                        //$('#projectopening').hide();
                        //$('#gridlist').show();
                        //$('#popupFilter').hide();

                    },
                    error: function (err) {
                        console.log("error : " + err);
                    }
                });
                return deferred.promise();
            }         

        },

        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "25px" },
            { type: "number", name: "ProjectId", title: "Project Id", visible: false },
             { type: "text", name: "Projecttitle", title: "Project Title", editing: false, width: "60px" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "50px" },
            { type: "text", name: "SponsoringAgencyName", title: "Agency Name", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "60px" },
            //{ type: "text", name: "PIDepartmentName", title: "Department of PI", editing: false, width: "75px" },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "50px" },
            //{ name: "PrpsalApprovedDate", title: "Approved Date", type: "date", width: 100, align: "center" },
             { type: "text", name: "SanctionOrderNumber", title: "Sanction Order Number", editing: false, width: "50px" },
            { type: "number", name: "Budget", title: "Budget Value", editing: false, width: "60px" },
            { type: "text", name: "Status", title: "Status", editing: false },
            {
                type: "control", editButton: false, deleteButton: false, width: "100px", title: "Action",
                itemTemplate: function (value, item) {
                    console.log(item.Status);

                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Open") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "Submit for approval", name: "Submit for approval" }, { id: "View", name: "View" }]

                    }
                    if (item.Status == "InActive") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }]

                    }
                    else if (item.ProjectFundingCategory == 1 && item.Status == "Active") {
                       
                        statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }]
                    }
                    else if (item.ProjectFundingCategory == 2 && item.Status == "Active") {
                        
                        statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }, { id: "Update RO", name: "Update RO" }]
                    }
                    else if (item.Status == "Submit for approval") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }]
                    }
                  

                    

                    var $customSelect = $("<select>")
                        .attr("class", "form-control").prop("selectedIndex", "")
                    $.each(statusList, function (index, itemData) {
                        $customSelect.append($('<option/>', {

                            value: itemData.id,
                            text: itemData.name
                        }));
                    });
                
                    $customSelect.change(function (e) {

                        var selVal = $(this).val();
                        if (selVal == "Edit") {

                            $('#addnewpage').hide();
                            $('#gridlist').hide();
                            $('#saveproject').hide();
                            $('#projectopening').show();
                            $('#updateproject').show();
                            $.ajax({
                                type: "POST",
                                url: 'EditProject',
                                data: {
                                    ProjectId: item.ProjectId
                                },

                                success: function (result) {
                                    console.log(result);
                                    fillData(result);
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                        }
                        else if (selVal == "View") {
                            var url = 'ViewProject?ProjectId=' + item.ProjectId;
                            //var url = '../ProjectFunding/CreateRO' + item.ProjectId;
                            window.location.href = url;
                        }
                        else if (selVal == "Update RO") {
                            var url = '../ProjectFundingCategory/CreateRO?ProjectId=' + item.ProjectId;
                            window.location.href = url;
                        }
                        else if (selVal == "Submit for approval") {
                            var choice = confirm("Are you sure, Do you want to submit this project for approval process?");
                            if (choice === true) {
                                $.ajax({
                                    type: "POST",
                                    url: 'ProjectWFInit',
                                    data: {
                                        projectId: item.ProjectId
                                    },
                                    success: function (result) {
                                        if (result == true) {
                                            $('#alertSuccess').html("Project has been submitted for approval process successfully.");
                                            $('#Success').modal('toggle');
                                            $("#gridProjectList").jsGrid("loadData");
                                        } else if (result == false) {
                                            $('#FailedAlert').html("Something went wrong please contact administrator");
                                            $('#Failed').modal('toggle');
                                        }
                                    },
                                    error: function (err) {
                                        console.log("error1 : " + err);
                                    }
                                });
                            }
                        }

                        $(this).val("");
                        // return false;
                        e.stopPropagation();
                    });
                    return $result.add($customSelect);
                },
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                },

            },

        ],
        //onItemEditing: function (args) {

        //    // cancel editing of the row of item with field 'ID' = 0

        //    if (args.item.ProjectId > 0) {
        //        var projectid = args.item.ProjectId;
        //    }
        //    $('#addnewpage').hide();
        //    $('#gridlist').hide();
        //    $('#saveproject').hide();
        //    $('#projectopening').show();
        //    $('#updateproject').show();
        //    //$('#Proposalidfield').hide();
        //    //$('#Projectidfield').show();
        //    $.ajax({
        //        type: "POST",
        //        url: EditProject,
        //        data: { ProjectId: projectid },
        //        //contentType: "application/json; charset=utf-8",
        //        //dataType: "json",

        //        success: function (result) {
        //            var selectProject = $("#MainProjectId");
        //            selectProject.empty();
        //            selectProject.append($('<option/>', {
        //                value: "",
        //                text: "Select Any"
        //            }));
        //            $.each(result.MainProjectList, function (index, itemData) {
        //                selectProject.append($('<option/>', {
        //                    value: itemData.id,
        //                    text: itemData.name
        //                }));
        //            });
        //            fillData(result);

        //        }
        //    });

        //},
        //onItemDeleting: function(args) {
        //    if (args.item.ProjectID > 0) {
        //        var projectid = args.item.ProjectID;
        //    }
        //    $.ajax({
        //        type: "POST",
        //        url: Deleteproject,
        //        data: { ProjectId: projectid },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",

        //        success: function (result) {

        //            //window.onload();
        //            //$('#createuserid').hide();                    
        //            $('#createproposal').hide();
        //            $('#saveproposal').hide();
        //            $('#updateproposal').hide();
        //            $('#gridlist').show();
        //            if (result == 4) {
        //                $('#gridlist').show();
        //                $('#myModal2').modal('show');
        //            }
        //        },
        //        error: function (err) {
        //            console.log("error1 : " + err);
        //        }
        //    });
        //}
    });

    $("#gridProposalList").jsGrid("option", "filtering", false);
    //function GetProjectlist() {

    //    $.ajax({
    //        type: "GET",
    //        url: getProjectDetailsURL,
    //        data: param = "",
    //        contentType: "application/json; charset=utf-8",
    //        success: function (result) {
    //            // dataProposal = result;
    //            $("#gridProjectList").jsGrid({ data: result });
    //            //$('#projectopening').hide();
    //            //$('#gridlist').show();
    //            dbProject = result;
    //        },
    //        error: function (err) {
    //            console.log("error : " + err);
    //        }

    //    });

    //}
    $("#gridProjectList").jsGrid("option", "filtering", false);
    //$.ajax({
    //    type: "GET",
    //    url: getProposalDetailsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (result) {
    //        // dataProposal = result;
    //        $("#gridProposalList").jsGrid({ data: result });
    //        //$('#projectopening').hide();
    //        //$('#gridproposal').show();
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});

    //function GetProposallist() {

    //    $.ajax({
    //        type: "GET",
    //        url: getProposalDetailsURL,
    //        data: param = "",
    //        contentType: "application/json; charset=utf-8",
    //        success: function (result) {
    //            // dataProposal = result;
    //            $("#gridProposalList").jsGrid({ data: result });
    //            //$('#projectopening').hide();
    //            //$('#gridproposal').show();
    //            dbProposal = result;
    //        },
    //        error: function (err) {
    //            console.log("error : " + err);
    //        }

    //    });

    //}

});
function fillData(result) {

    $('input[name="ProposalID"]').val(result.ProposalID);
    $('input[name="ProjectID"]').val(result.ProjectID);
    //if (result.ProjectID > 0) {
    //    $('#Proposalidfield').css("display", "none");
    //    $('#Projectidfield').css("display", "block");
    //} else {
    //    $('#Proposalidfield').css("display", "block");
    //    $('#Projectidfield').css("display", "none");
    //}
    $('#projectcategory').val(result.Projectcatgry_Qust_1);    
    $('#propslnum').val(result.ProposalNumber);
    $('#HiddenProposalNumber').val(result.ProposalNumber);
    $('#projectnum').val(result.ProjectNumber);
    $('input[name="ProjectNumber"]').val(result.ProjectNumber);
    $('#lblFinYear').text(result.CurrentFinYear);
    $('#finyear').val(result.FinancialYear);
    $('#Prjcttype').val(result.ProjectType);
    $('#ProjectType').val(result.ProjectType);
    $('#Projecttypename').val(result.ProjectTypeName);
    //    $('#ProposalApprovedDate').val(result.PrpsalApprovedDate);
    $('#ProposalApprovedDate').val(result.PrpsalApprovedDate);
    //$('input[name="PrpsalApprovedDate"]').val(result.PrpsalApprovedDate);

    //$('#projectdurationyear').val(result.Projectdurationyears);
    //$('#projectdurationmonths').val(result.Projectdurationmonths);
    //$('input[name="projectdurationyear"]').val(result.Projectdurationyears);
    //$('input[name="projectdurationmonths"]').val(result.Projectdurationmonths);
    $('#ScientistName').val(result.ScientistName);
    $('#ScientistEmail').val(result.ScientistEmail);
    $('#ScientistMobile').val(result.ScientistMobile);
    $('#ScientistAddress').val(result.ScientistAddress);

    $('#SanctionOrderDate').val(result.SODate);
    $('#SODate').val(result.SODate);
    $('#SOnumber').val(result.SanctionOrderNumber);
    var strDate = new Date(parseInt(result.TentativeStartdate.replace(/(^.*\()|([+-].*$)/g, '')));
    var clsDate = new Date(parseInt(result.TentativeClosedate.replace(/(^.*\()|([+-].*$)/g, '')));
    $("#TentativeStartdate").datepicker('setDate', strDate);
    $("#TentativeClosedate").datepicker('setDate', clsDate);
    //$('#Inputdate').val(result.Inptdate);
    //$('input[name="Inptdate"]').val(result.Inptdate);
    //$('#TentativeStartdate').val(result.TentativestrtDate);
    //$('#Startdate').val(result.strtDate);
    //$('#TentativeClosedate').val(result.TentativeclsDate);
    //$('#Closedate').val(result.clsDate);
    //$('#TentativestrtDate').val(result.TentativestrtDate);
    //$('#strtDate').val(result.strtDate);
    //$('#TentativeclsDate').val(result.TentativeclsDate);
    //$('#clsDate').val(result.clsDate);
    $('#Domainrelevant').val(result.Domainrelevant);
    if (result.IsthereAnyArtificialIntelligence == "Yes") {
        $('input[name=IsthereAnyArtificialIntelligence][value=' + result.IsthereAnyArtificialIntelligence + ']').attr('checked', 'checked');
        $('#aiDetail').removeClass("dis-none");
        
        $('#AIDescription').val(result.AIDescription);
    }
    else {
        $('#aiDetail').addClass("dis-none");
    }
    if (result.IsSubProject) {
        $('#divMainProject').removeClass('dis-none');
        $('#MainProjectId').val(result.MainProjectId);
        $('#mainProjectNo').val(result.MainProjectNumber);
        

        $('#IsSubProject').prop('checked', true);
    } else {
        $('#divMainProject').addClass('dis-none');
        $('#IsSubProject').prop('checked', false);
    }

    $('#divCentre,#divLab').addClass('dis-none');
    //$('#FacultyCode').val(result.FacultyCode).attr("readonly", true);
    //if (result.FacultyCode == "2") {
    //    $('#divCentre').removeClass('dis-none');
    //    $('#SelCentre').val(result.SelCentre).attr("readonly", true);
    //}
    //else if (result.FacultyCode == "4") {
    //    $('#divLab').removeClass('dis-none');
    //    $('#SelLab').val(result.SelLab).attr("readonly", true);
    //}
    //else if (result.FacultyCode == "5") {
    //    $('#divCor').removeClass('dis-none');
    //    $('#SelCor').val(result.SelCor).attr("readonly", true);
    //}


    $('#FacultyCode').val(result.FacultyCode);
    if (result.FacultyCode == "2") {
        $('#divCentre').removeClass('dis-none');
        $('#SelCentre').val(result.SelCentre);
    }
    else if (result.FacultyCode == "4") {
        $('#divLab').removeClass('dis-none');
        $('#SelLab').val(result.SelLab);
    }
    else if (result.FacultyCode == "5") {
        $('#divCor').removeClass('dis-none');
        $('#SelCor').val(result.SelCor);
    }


    $('#ProjectClassification').val(result.ProjectClassification);
    $('#ReportClassifiCation').val(result.ReportClassifiCation);

    $('#txtprocategory').val(result.ProjectFundingCategoryId);
   
    if (result.ProjectFundingCategoryId == 2)
    {
        $('#BankDetails').removeClass("dis-none");
        $('#BankDetails').show();
        $('#BankName').val(result.bankname);
        $('#BankID').val(result.BankID);
    } else {
        $('#BankDetails').addClass("dis-none");
        $('#BankDetails').hide();
    }

    $('#SponsoredProjectTypeCode').val(result.SponsoredProjectTypeCode);
    $('#pjctsubtyp').val(result.ProjectSubType);
    $('input[name="ProjectSubType"]').val(result.ProjectSubType);

    $('#prjcttitle').val(result.Projecttitle);
    $('#PI').val(result.PIname);
    $('input[name="PIname"]').val(result.PIname);
    $('#PIEmail').val(result.PIEmail);
    $('input[name="PIEmail"]').val(result.PIEmail);
    $('#PIRMF').val(result.PIRMF);
    $('#PIPCF').val(result.PIPCF);
    $('#department').val(result.Department);
    $('input[name="Department"]').val(result.Department);
    $('#PIDesignation').val(result.PIDesignation);

    $('#constaxservicemode').hide();
    $('#sponAgencyName').val(result.SponsoringAgencyName);
    $('#Agncy').val(result.SponsoringAgency);
    $('input[name="SponsoringAgency"]').val(result.SponsoringAgency);
    $('#txtagencycode').val(result.AgencyCode);
    $('input[name="AgencyCode"]').val(result.AgencyCode);


    $('#txtcntctpersonaddress').val(result.Agencycontactpersonaddress);
    $('#HiddenContactAddr').val(result.Agencycontactpersonaddress);
    $('#txtcntctpersonmobile').val(result.Agencycontactpersonmobile);
    $('#HiddenContactMobile').val(result.Agencycontactpersonmobile);
    $('#txtcntctpersonemail').val(result.AgencycontactpersonEmail);
    $('#HiddenContactEmail').val(result.AgencycontactpersonEmail);
    $('#txtagncyconperson').val(result.Agencycontactperson);
    $('#HiddenContactPerson').val(result.Agencycontactperson);
    $('#txtagncyregaddrss').val(result.Agencyregaddress);
    $('#HiddenRegAddr').val(result.Agencyregaddress);
    $('#txtagncyregname').val(result.Agencyregname);
    $('#HiddenRegName').val(result.Agencyregname);

    $('#typeofproject').val(result.TypeofProject);
    if (result.TypeofProject == 1) {
        $('#CollabCoordinator,#CollabType').hide();
        $('#CollabAgency').hide();
        $('#CollabCoordinatorEmail').hide();
        $('#TotalCost').hide();
        $('#IITMCost').hide();
    }
    if (result.TypeofProject == 2) {
        $('#CollabCoordinator,#CollabType').show();
        $('#CollabAgency').show();
        $('#CollabCoordinatorEmail').show();
        $('#TotalCost').show();
        $('#IITMCost').show();
    }
    if (result.ProjectType == 1 && result.ProjectSubType == 1) {
        $("#internalagency").removeClass('dis-none');
        //     $("#catgoryofprjct").hide();
        $('#internalschemeagency').val(result.InternalSchemeFundingAgency);
        //  $('#internalschemeagency').val(result.SponsoringAgency);
        $('input[name="InternalSchemeFundingAgency"]').val(result.InternalSchemeFundingAgency);

        //$('#schemeagency').val(result.SchemeAgency);
        //$('#schemepersonapplied').val(result.Personapplied);
        //$('#schemepersonapplieddesig').val(result.SchemePersonAppliedDesignation);
    }
    if (result.ProjectType == 1 && result.ProjectSubType == 2) {
        $("#indprjctcategory_1").removeClass('dis-none');
        $("#sponextrnlscheme").removeClass('dis-none');
        if (result.Projectcatgry_Qust_1 == 1) {
            $("#sponextrnlschemecode").removeClass('dis-none');
            $('#schemecode').val(result.SchemeCode);
           // $('input[name="SchemeCode"]').val(result.SchemeCode);
        }
        $("#prjctdetails1").removeClass('dis-none');
        //$('#ctgoryofprjct').val(result.Categoryofproject);
        //$('input[name="Categoryofproject"]').val(result.Categoryofproject);                   
        //    $("#catgoryofprjct").show();
        $('#schemename').val(result.Schemename);
        $('input[name="Schemename"]').val(result.Schemename);
        $('input[name="Projectcatgry_Qust_1"]').val(result.Projectcatgry_Qust_1);
        $('#projectcategory').val(result.Projectcatgry_Qust_1);
        // document.getElementsByName('Projectcatgry_Qust_1')[0].value = result.Projectcatgry_Qust_1;

    }

    $('#projectduration').val(result.Projectduration);
    $('#snctnvalue').val(result.Budget);
    $('#basevalue').val(result.BaseValue);
    $('#applicabletax').val(result.ApplicableTax);
    $('#ApplicableTaxHidden').val(result.ApplicableTax);
    $('input[name="BaseValue"]').val(result.BaseValue);
    $('#BaseValueHidden').val(result.BaseValue);
    $('input[name="ApplicableTax"]').val(result.ApplicableTax);
    $('#divSanctionvalue').removeClass('dis-none');
    $('#lblSanctionValue').html(result.Sanctionvalue);
    $('input[name="Sanctionvalue"]').html(result.Sanctionvalue);
    //$('input[name="Sanctionvalue"]').val(result.Sanctionvalue);
    //$('#SanctionvalueHidden').val(result.Sanctionvalue);

    $('#prjcttitle').val(result.Projecttitle);
    $('#txtagncyregname').val(result.Agencyregname);
    $('#txtagncyregaddrss').val(result.Agencyregaddress);

    $('#totlprjctstaffs').val(result.TotalNoofProjectStaffs);
    //$('input[name="NoofJRFStaffs"]').val(result.NoofJRFStaffs);
    //$('input[name="SalaryofJRFStaffs"]').val(result.SalaryofJRFStaffs);
    //$('input[name="NoofSRFStaffs"]').val(result.NoofSRFStaffs);
    //$('input[name="SalaryofSRFStaffs"]').val(result.SalaryofSRFStaffs);
    //$('input[name="NoofRAStaffs"]').val(result.NoofRAStaffs);
    //$('input[name="SalaryofRAStaffs"]').val(result.SalaryofRAStaffs);
    //$('input[name="NoofPAStaffs"]').val(result.NoofPAStaffs);
    //$('input[name="SalaryofPAStaffs"]').val(result.SalaryofPAStaffs);
    //$('input[name="NoofPQStaffs"]').val(result.NoofPQStaffs);
    //$('input[name="SalaryofPQStaffs"]').val(result.SalaryofPQStaffs);               
    $('input[name="SumofStaffs"]').val(result.SumofStaffs);
    $('input[name="SumSalaryofStaffs"]').val(result.SumSalaryofStaffs);

    if (result.ProjectType == 1) {
        $('#prjcttypespon').show();
        $('#prjcttypeconsul').hide();
        $('#indprjctcategory_1').css("display", "none");
        $('#prjctdetails1').css("display", "block");
        $('#prjctdetails2').css("display", "none");
        $('#taxnumberdtls').hide();
        $('#constaxdetails').hide();
        $('#constaxservicemode').hide();
        $('#reasonfornotax').hide();
        $('#proofofnotax').hide();
        $('#proofofnotaxlink').hide();
        $('#indprojectstate').hide();
        $('#indprojectlocation').hide();
        $('#forignprojectcntry').hide();
        $('#forignprojectstate').hide();
        $('#forignprojectlocation').hide();
        $('#reasonfornotax').hide();
        $('#proofofnotax').hide();
        $('#proofofnotaxlink').hide();
    }

    if (result.ProjectType == 2) {
        $('#taxservice').val(result.constaxservice);
        // $('#conspjctsubtyp').val(result.ProjectSubType);
        //  $('input[name="ConsProjectSubType"]').val(result.ProjectSubType);
        //$('#ctgoryofprjct').val(result.Categoryofproject);
        //$('input[name="Categoryofproject"]').val(result.Categoryofproject);
        // $('#conspjctfndngctgry').val(result.ConsFundingCategory);
        $('#conspjctsubtyp').val(result.Conssubtypename);
        $('#Conssubtypename').val(result.Conssubtypename);
        $('select[name="ConsProjectSubType"]').val(result.ConsProjectSubType);
        $('#conspjctfndngctgry').val(result.ConsFundingcategoryname);
        //$('select[name="ConsFundingcategoryname"]').val(result.ConsFundingcategoryname);
        $('select[name="ConsFundingCategory"]').val(result.ConsFundingCategory);
        $('#IndTaxService').val('false');
        if (result.constaxservice == 2) {
            $('#indfndgagncystate').val(result.indfundngagncystate);
            $('#indfndngagncylocation').val(result.indfundngagncylocation);
            $('#IndTaxService').val('true');
            $("#taxregistatus").show();
            $("#indprojectstate").show();
            $("#indprojectlocation").show();
            $("#forignprojectcntry").hide();
            $("#forignprojectstate").hide();
            $("#forignprojectlocation").hide();
        }

        if (result.constaxservice == 1) {
            $('#indfndgagncystate').val(result.indfundngagncystate);
            $('#indfndngagncylocation').val(result.indfundngagncylocation);
            $('#IndTaxService').val('true');
            $("#indprojectstate").show();
            $("#indprojectlocation").show();
            $("#forignprojectcntry").hide();
            $("#forignprojectstate").hide();
            $("#forignprojectlocation").hide();
        }
        if (result.constaxservice == 3) {
            $('#forgnfndngagncycntry').val(result.forgnfndngagncycountry);
            $('#forgnfndngagncystate').val(result.forgnfundngagncystate);
            $('#forgnfndngagncylocation').val(result.forgnfundngagncylocation);

            $("#indprojectstate").hide();
            $("#indprojectlocation").hide();

            $("#forignprojectcntry").show();
            $("#forignprojectstate").show();
            $("#forignprojectlocation").show();
        }
        if (result.constaxservice == 0 || result.constaxservice == null) {
            $("#indprojectstate").hide();
            $("#indprojectlocation").hide();

            $("#forignprojectcntry").hide();
            $("#forignprojectstate").hide();
            $("#forignprojectlocation").hide();
        }

        $('#prjcttypeconsul').show();
        $('#prjcttypespon').hide();
        $('#indprjctcategory_1').css("display", "none");
        $('#prjctdetails1').css("display", "none");
        $('#prjctdetails2').css("display", "block");
        $('#taxnumberdtls').show();
        $('#constaxdetails').show();
        $('#constaxservicemode').show();
    }

    if (result.ProjectType == 1) {
        $('#indprjctfundedby_1').css("display", "none");
        $('#forgnprjctfundedby_1, .forgnPrjctDet').css("display", "none");
        $('#indprjctcategory_1').css("display", "none");
        $('#indprjctfundbodygovt_1').css("display", "none");
        $('#indprjctfundbodynongovt_1').css("display", "none");
        $('#forgnprjctfundbodygovt_1').css("display", "none");
        $('#forgnprjctfundbodynongovt_1').css("display", "none");
        $('.conforgnPrjctDet').css("display", "none");
        $("#indprjctfundbodygovtagncydeptnam_1").css("display", "none");
        $("#indprjctfundbodygovtdeptamt_1").css("display", "none");

        $("#indprjctfundbodygovtagncymnstrynam_1").css("display", "none");
        $("#indprjctfundbodygovtmnstryamt_1").css("display", "none");

        $("#indprjctfundbodygovtunivnam_1").css("display", "none");
        $("#indprjctfundbodygovtunivamt_1").css("display", "none");

        $("#indprjctfundbodynongovtagncyindstrynam_1").css("display", "none");
        $("#indprjctfundbodynongovtindstryamt_1").css("display", "none");

        $("#indprjctfundbodynongovtunivnam_1").css("display", "none");
        $("#indprjctfundbodynongovtunivamt_1").css("display", "none");

        $("#indprjctfundbodynongovtagncyothersnam_1").css("display", "none");
        $("#indprjctfundbodynongovtothersamt_1").css("display", "none");

        $("#forgnprjctfundbodygovtcntry_1").css("display", "none");
        $("#forgnprjctfundbodygovtagncydeptnam_1").css("display", "none");
        $("#forgnprjctfundbodygovtdeptamt_1").css("display", "none");

        $("#forgnprjctfundbodygovtunivcntry_1").css("display", "none");
        $("#forgnprjctfundbodygovtagncyunivnam_1").css("display", "none");
        $("#forgnprjctfundbodygovtunivamt_1").css("display", "none");

        $("#forgnprjctfundbodygovtothrscntry_1").css("display", "none");
        $("#forgnprjctfundbodygovtagncyothrsnam_1").css("display", "none");
        $("#forgnprjctfundbodygovtothrsamt_1").css("display", "none");

        $("#forgnprjctfundbodynongovtcntry_1 ").css("display", "none");
        $("#forgnprjctfundbodynongovtagncydeptnam_1 ").css("display", "none");
        $("#forgnprjctfundbodynongovtdeptamt_1").css("display", "none");

        $("#forgnprjctfundbodynongovtunivcntry_1").css("display", "none");
        $("#forgnprjctfundbodynongovtagncyunivnam_1 ").css("display", "none");
        $("#forgnprjctfundbodynongovtunivamt_1").css("display", "none");

        $("#forgnprjctfundbodynongovtothrscntry_1").css("display", "none");
        $("#forgnprjctfundbodynongovtagncyothrsnam_1").css("display", "none");
        $("#forgnprjctfundbodynongovtothrsamt_1").css("display", "none");
        $('#prjctfundingtype').val(result.ProjectFundingType_Qust_1);
        if (result.ProjectFundingType_Qust_1 == 1 || result.ProjectFundingType_Qust_1 == 3) {
            $('#prjctfundedby').val(result.ProjectFundedby_Qust_1);
            $('#IndAndBoth').val('True');
            $('#indprjctfundedby_1, #indprjctcategory_1').css("display", "block");
            if (result.ProjectFundedby_Qust_1 == 1) {
                $('#indprjctfundbodygovt_1').css("display", "block");
                $('#GovAndBoth').val('True');

                $('#Indfundgovtbody').val(result.IndProjectFundingGovtBody_Qust_1);
                $('input[name="IndProjectFundingGovtBodyId"]').val(result.IndProjectFundingGovtBodyId);
                var Indianfundingbody = result.IndProjectFundingGovtBody_Qust_1;

                $.each(Indianfundingbody, function (i, val) {

                    if (result.IndProjectFundingGovtBody_Qust_1[i] == 1) {
                        $('#indprjctfundbodygovtagncydeptnam_1').css("display", "block");
                        $('#indprjctfundbodygovtdeptamt_1').css("display", "block");
                        $('#IndGovFund_MHRD').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencydeptname')[0].value = result.indprjctfundbodygovt_Agencydeptname;
                        document.getElementsByName('indprjctfundbodygovt_deptAmount')[0].value = result.indprjctfundbodygovt_deptAmount;
                    }
                    else if (result.IndProjectFundingGovtBody_Qust_1[i] == 2) {

                        $('#indprjctfundbodygovtagncymnstrynam_1').css("display", "block");
                        $('#indprjctfundbodygovtmnstryamt_1').css("display", "block");
                        $('#IndGovFund_Mnstry').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencymnstryname')[0].value = result.indprjctfundbodygovt_Agencymnstryname;
                        document.getElementsByName('indprjctfundbodygovt_mnstryAmount')[0].value = result.indprjctfundbodygovt_mnstryAmount;
                    }
                    else if (result.IndProjectFundingGovtBody_Qust_1[i] == 3) {
                        $('#indprjctfundbodygovtunivnam_1').css("display", "block");
                        $('#indprjctfundbodygovtunivamt_1').css("display", "block");
                        $('#IndGovFund_Univ').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencyunivname')[0].value = result.indprjctfundbodygovt_Agencyunivname;
                        document.getElementsByName('indprjctfundbodygovt_univAmount')[0].value = result.indprjctfundbodygovt_univAmount;
                    }
                });

            } else if (result.ProjectFundedby_Qust_1 == 2) {
                $('#indprjctfundbodynongovt_1').css("display", "block");
                $('#NonGovAndBoth').val('True');

                $('#Indfundnongovtbody').val(result.IndProjectFundingNonGovtBody_Qust_1);
                $('input[name="IndProjectFundingNonGovtBodyId"]').val(result.IndProjectFundingNonGovtBodyId);
                var Indianfundingbody = result.IndProjectFundingNonGovtBody_Qust_1;

                $.each(Indianfundingbody, function (i, val) {

                    if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 1) {
                        $('#indprjctfundbodynongovtagncyindstrynam_1').css("display", "block");
                        $('#indprjctfundbodynongovtindstryamt_1').css("display", "block");
                        $('#IndNonGovFund_Indus').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_AgencyIndstryname')[0].value = result.indprjctfundbodynongovt_AgencyIndstryname;
                        document.getElementsByName('indprjctfundbodynongovt_IndstryAmount')[0].value = result.indprjctfundbodynongovt_IndstryAmount;
                    }
                    else if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 2) {

                        $('#indprjctfundbodynongovtunivnam_1').css("display", "block");
                        $('#indprjctfundbodynongovtunivamt_1').css("display", "block");
                        $('#IndNonGovFund_Univ').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_Agencyunivname')[0].value = result.indprjctfundbodynongovt_Agencyunivname;
                        document.getElementsByName('indprjctfundbodynongovt_univAmount')[0].value = result.indprjctfundbodynongovt_univAmount;
                    }
                    else if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 3) {
                        $('#indprjctfundbodynongovtagncyothersnam_1').css("display", "block");
                        $('#indprjctfundbodynongovtothersamt_1').css("display", "block");
                        $('#IndNonGovFund_Other').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_Agencyothersname')[0].value = result.indprjctfundbodynongovt_Agencyothersname;
                        document.getElementsByName('indprjctfundbodynongovt_othersAmount')[0].value = result.indprjctfundbodynongovt_othersAmount;
                    }
                });

            } else if (result.ProjectFundedby_Qust_1 == 3) {
                $('#indprjctfundbodygovt_1, #indprjctfundbodynongovt_1').css("display", "block");
                $('#GovAndBoth, #NonGovAndBoth').val('True');

                $('#Indfundgovtbody').val(result.IndProjectFundingGovtBody_Qust_1);
                $('input[name="IndProjectFundingGovtBodyId"]').val(result.IndProjectFundingGovtBodyId);
                var Indianfundingbody = result.IndProjectFundingGovtBody_Qust_1;

                $.each(Indianfundingbody, function (i, val) {

                    if (result.IndProjectFundingGovtBody_Qust_1[i] == 1) {
                        $('#indprjctfundbodygovtagncydeptnam_1').css("display", "block");
                        $('#indprjctfundbodygovtdeptamt_1').css("display", "block");
                        $('#IndGovFund_MHRD').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencydeptname')[0].value = result.indprjctfundbodygovt_Agencydeptname;
                        document.getElementsByName('indprjctfundbodygovt_deptAmount')[0].value = result.indprjctfundbodygovt_deptAmount;
                    }
                    else if (result.IndProjectFundingGovtBody_Qust_1[i] == 2) {

                        $('#indprjctfundbodygovtagncymnstrynam_1').css("display", "block");
                        $('#indprjctfundbodygovtmnstryamt_1').css("display", "block");
                        $('#IndGovFund_Mnstry').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencymnstryname')[0].value = result.indprjctfundbodygovt_Agencymnstryname;
                        document.getElementsByName('indprjctfundbodygovt_mnstryAmount')[0].value = result.indprjctfundbodygovt_mnstryAmount;
                    }
                    else if (result.IndProjectFundingGovtBody_Qust_1[i] == 3) {
                        $('#indprjctfundbodygovtunivnam_1').css("display", "block");
                        $('#indprjctfundbodygovtunivamt_1').css("display", "block");
                        $('#IndGovFund_Univ').val('True');
                        document.getElementsByName('indprjctfundbodygovt_Agencyunivname')[0].value = result.indprjctfundbodygovt_Agencyunivname;
                        document.getElementsByName('indprjctfundbodygovt_univAmount')[0].value = result.indprjctfundbodygovt_univAmount;
                    }
                });

                $('#Indfundnongovtbody').val(result.IndProjectFundingNonGovtBody_Qust_1);
                $('input[name="IndProjectFundingNonGovtBodyId"]').val(result.IndProjectFundingNonGovtBodyId);
                var IndianfundingbodyNonGov = result.IndProjectFundingNonGovtBody_Qust_1;

                $.each(IndianfundingbodyNonGov, function (i, val) {

                    if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 1) {
                        $('#indprjctfundbodynongovtagncyindstrynam_1').css("display", "block");
                        $('#indprjctfundbodynongovtindstryamt_1').css("display", "block");
                        $('#IndNonGovFund_Indus').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_AgencyIndstryname')[0].value = result.indprjctfundbodynongovt_AgencyIndstryname;
                        document.getElementsByName('indprjctfundbodynongovt_IndstryAmount')[0].value = result.indprjctfundbodynongovt_IndstryAmount;
                    }
                    else if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 2) {

                        $('#indprjctfundbodynongovtunivnam_1').css("display", "block");
                        $('#indprjctfundbodynongovtunivamt_1').css("display", "block");
                        $('#IndNonGovFund_Univ').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_Agencyunivname')[0].value = result.indprjctfundbodynongovt_Agencyunivname;
                        document.getElementsByName('indprjctfundbodynongovt_univAmount')[0].value = result.indprjctfundbodynongovt_univAmount;
                    }
                    else if (result.IndProjectFundingNonGovtBody_Qust_1[i] == 3) {
                        $('#indprjctfundbodynongovtagncyothersnam_1').css("display", "block");
                        $('#indprjctfundbodynongovtothersamt_1').css("display", "block");
                        $('#IndNonGovFund_Other').val('True');
                        document.getElementsByName('indprjctfundbodynongovt_Agencyothersname')[0].value = result.indprjctfundbodynongovt_Agencyothersname;
                        document.getElementsByName('indprjctfundbodynongovt_othersAmount')[0].value = result.indprjctfundbodynongovt_othersAmount;
                    }
                });
            }
        }
        if (result.ProjectFundingType_Qust_1 == 2 || result.ProjectFundingType_Qust_1 == 3) {
            $('#prjctforeignfundedby').val(result.ForgnProjectFundedby_Qust_1);

            $('#SelCurr').val(result.SelCurr);
            $('#ConversionRate').val(result.ConversionRate);
            $('#ForgnAndBoth').val('True');
            $('#forgnprjctfundedby_1, .forgnPrjctDet').css("display", "block");
            if (result.ForgnProjectFundedby_Qust_1 == 1) {
                $('#forgnprjctfundbodygovt_1').css("display", "block");
                $('#ForgnGovAndBoth').val('True');

                $('#Forgnfundgovtbody').val(result.ForgnProjectFundingGovtBody_Qust_1);
                $('input[name="ForgnProjectFundingGovtBodyId"]').val(result.ForgnProjectFundingGovtBodyId);
                var Foreignfundingbody = result.ForgnProjectFundingGovtBody_Qust_1;

                $.each(Foreignfundingbody, function (i, val) {

                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 1) {
                        $('#forgnprjctfundbodygovtcntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncydeptnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtdeptamt_1').css("display", "block");
                        $('#ForgnGovFund_Dep').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_country')[0].value = result.forgnprjctfundbodygovt_country;
                        document.getElementsByName('forgnprjctfundbodygovt_Agencydeptname')[0].value = result.forgnprjctfundbodygovt_Agencydeptname;
                        document.getElementsByName('forgnprjctfundbodygovt_deptAmount')[0].value = result.forgnprjctfundbodygovt_deptAmount;

                    }
                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 2) {

                        $('#forgnprjctfundbodygovtunivcntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncyunivnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtunivamt_1').css("display", "block");
                        $('#ForgnGovFund_Univ').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_univcountry')[0].value = result.forgnprjctfundbodygovt_univcountry;
                        document.getElementsByName('forgnprjctfundbodygovt_Agencyunivname')[0].value = result.forgnprjctfundbodygovt_Agencyunivname;
                        document.getElementsByName('forgnprjctfundbodygovt_univAmount')[0].value = result.forgnprjctfundbodygovt_univAmount;
                    }
                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 3) {

                        $('#forgnprjctfundbodygovtothrscntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncyothrsnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtothrsamt_1').css("display", "block");
                        $('#ForgnGovFund_Other').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_otherscountry')[0].value = result.forgnprjctfundbodygovt_otherscountry;
                        document.getElementsByName('forgnprjctfundbodygovt_othersagncyname')[0].value = result.forgnprjctfundbodygovt_othersagncyname;
                        document.getElementsByName('forgnprjctfundbodygovt_othersAmount')[0].value = result.forgnprjctfundbodygovt_othersAmount;
                    }
                });
            } else if (result.ForgnProjectFundedby_Qust_1 == 2) {
                $('#forgnprjctfundbodynongovt_1').css("display", "block");
                $('#ForgnNonGovAndBoth').val('True');

                $('#Forgnfundnongovtbody').val(result.ForgnProjectFundingNonGovtBody_Qust_1);
                $('input[name="ForgnProjectFundingNonGovtBodyId"]').val(result.ForgnProjectFundingNonGovtBodyId);
                var Foreignnongovtbody = result.ForgnProjectFundingNonGovtBody_Qust_1;
                $.each(Foreignnongovtbody, function (i, val) {
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 1) {

                        $('#forgnprjctfundbodynongovtcntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncydeptnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtdeptamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Dep').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_country')[0].value = result.forgnprjctfundbodynongovt_country;
                        document.getElementsByName('forgnprjctfundbodynongovt_Agencydeptname')[0].value = result.forgnprjctfundbodynongovt_Agencydeptname;
                        document.getElementsByName('forgnprjctfundbodynongovt_deptAmount')[0].value = result.forgnprjctfundbodynongovt_deptAmount;

                    }
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 2) {

                        $('#forgnprjctfundbodynongovtunivcntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncyunivnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtunivamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Univ').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_univcountry')[0].value = result.forgnprjctfundbodynongovt_univcountry;
                        document.getElementsByName('forgnprjctfundbodynongovt_Agencyunivname')[0].value = result.forgnprjctfundbodynongovt_Agencyunivname;
                        document.getElementsByName('forgnprjctfundbodynongovt_univAmount')[0].value = result.forgnprjctfundbodynongovt_univAmount;
                    }
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 3) {

                        $('#forgnprjctfundbodynongovtothrscntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncyothrsnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtothrsamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Other').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_otherscountry')[0].value = result.forgnprjctfundbodynongovt_otherscountry;
                        document.getElementsByName('forgnprjctfundbodynongovt_othersagncyname')[0].value = result.forgnprjctfundbodynongovt_othersagncyname;
                        document.getElementsByName('forgnprjctfundbodynongovt_othersAmount')[0].value = result.forgnprjctfundbodynongovt_othersAmount;
                    }
                });
            } else if (result.ForgnProjectFundedby_Qust_1 == 3) {
                $('#forgnprjctfundbodygovt_1, #forgnprjctfundbodynongovt_1').css("display", "block");
                $('#ForgnGovAndBoth, #ForgnNonGovAndBoth').val('True');

                $('#Forgnfundgovtbody').val(result.ForgnProjectFundingGovtBody_Qust_1);
                $('input[name="ForgnProjectFundingGovtBodyId"]').val(result.ForgnProjectFundingGovtBodyId);
                var Foreignfundingbody = result.ForgnProjectFundingGovtBody_Qust_1;

                $.each(Foreignfundingbody, function (i, val) {

                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 1) {
                        $('#forgnprjctfundbodygovtcntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncydeptnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtdeptamt_1').css("display", "block");
                        $('#ForgnGovFund_Dep').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_country')[0].value = result.forgnprjctfundbodygovt_country;
                        document.getElementsByName('forgnprjctfundbodygovt_Agencydeptname')[0].value = result.forgnprjctfundbodygovt_Agencydeptname;
                        document.getElementsByName('forgnprjctfundbodygovt_deptAmount')[0].value = result.forgnprjctfundbodygovt_deptAmount;

                    }
                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 2) {

                        $('#forgnprjctfundbodygovtunivcntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncyunivnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtunivamt_1').css("display", "block");
                        $('#ForgnGovFund_Univ').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_univcountry')[0].value = result.forgnprjctfundbodygovt_univcountry;
                        document.getElementsByName('forgnprjctfundbodygovt_Agencyunivname')[0].value = result.forgnprjctfundbodygovt_Agencyunivname;
                        document.getElementsByName('forgnprjctfundbodygovt_univAmount')[0].value = result.forgnprjctfundbodygovt_univAmount;
                    }
                    if (result.ForgnProjectFundingGovtBody_Qust_1[i] == 3) {

                        $('#forgnprjctfundbodygovtothrscntry_1').css("display", "block");
                        $('#forgnprjctfundbodygovtagncyothrsnam_1').css("display", "block");
                        $('#forgnprjctfundbodygovtothrsamt_1').css("display", "block");
                        $('#ForgnGovFund_Other').val('True');
                        document.getElementsByName('forgnprjctfundbodygovt_otherscountry')[0].value = result.forgnprjctfundbodygovt_otherscountry;
                        document.getElementsByName('forgnprjctfundbodygovt_othersagncyname')[0].value = result.forgnprjctfundbodygovt_othersagncyname;
                        document.getElementsByName('forgnprjctfundbodygovt_othersAmount')[0].value = result.forgnprjctfundbodygovt_othersAmount;
                    }
                });

                $('#Forgnfundnongovtbody').val(result.ForgnProjectFundingNonGovtBody_Qust_1);
                $('input[name="ForgnProjectFundingNonGovtBodyId"]').val(result.ForgnProjectFundingNonGovtBodyId);
                var Foreignnongovtbody = result.ForgnProjectFundingNonGovtBody_Qust_1;
                $.each(Foreignnongovtbody, function (i, val) {
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 1) {

                        $('#forgnprjctfundbodynongovtcntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncydeptnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtdeptamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Dep').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_country')[0].value = result.forgnprjctfundbodynongovt_country;
                        document.getElementsByName('forgnprjctfundbodynongovt_Agencydeptname')[0].value = result.forgnprjctfundbodynongovt_Agencydeptname;
                        document.getElementsByName('forgnprjctfundbodynongovt_deptAmount')[0].value = result.forgnprjctfundbodynongovt_deptAmount;

                    }
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 2) {

                        $('#forgnprjctfundbodynongovtunivcntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncyunivnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtunivamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Univ').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_univcountry')[0].value = result.forgnprjctfundbodynongovt_univcountry;
                        document.getElementsByName('forgnprjctfundbodynongovt_Agencyunivname')[0].value = result.forgnprjctfundbodynongovt_Agencyunivname;
                        document.getElementsByName('forgnprjctfundbodynongovt_univAmount')[0].value = result.forgnprjctfundbodynongovt_univAmount;
                    }
                    if (result.ForgnProjectFundingNonGovtBody_Qust_1[i] == 3) {

                        $('#forgnprjctfundbodynongovtothrscntry_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtagncyothrsnam_1').css("display", "block");
                        $('#forgnprjctfundbodynongovtothrsamt_1').css("display", "block");
                        $('#ForgnNonGovFund_Other').val('True');
                        document.getElementsByName('forgnprjctfundbodynongovt_otherscountry')[0].value = result.forgnprjctfundbodynongovt_otherscountry;
                        document.getElementsByName('forgnprjctfundbodynongovt_othersagncyname')[0].value = result.forgnprjctfundbodynongovt_othersagncyname;
                        document.getElementsByName('forgnprjctfundbodynongovt_othersAmount')[0].value = result.forgnprjctfundbodynongovt_othersAmount;
                    }
                });
            }
        }
    }
    $('#ConsFundTypeForgnAndBoth').val('False');
    // Consultancy - "Indian"
    if (result.ProjectType == 2 && result.ConsProjectFundingType_Qust_1 == 1) {
        // $('input[name=ConsProjectTaxType_Qust_1][value=' + result.ConsProjectTaxType_Qust_1 + ']').attr('checked', 'checked');
        $('#constaxtype').val(result.ConsProjectTaxType_Qust_1);
        $("#consultprjctsubtype").show();
        $('#indprjctcategory_1').css("display", "none");
        //  $('input[name=ProjectFundingType_Qust_1][value=' + result.ProjectFundingType_Qust_1 + ']').attr('checked', 'checked');
        $('#consprjctfundingtype').val(result.ConsProjectFundingType_Qust_1);

        $('#conspjctsubtyp').val(result.Conssubtypename);
        $('#Conssubtypename').val(result.Conssubtypename);
        $('input[name="ConsProjectSubType"]').val(result.ConsProjectSubType);
        $('#conspjctfndngctgry').val(result.ConsFundingcategoryname);
        $('input[name="ConsFundingcategoryname"]').val(result.ConsFundingcategoryname);
        $('select[name="ConsFundingCategory"]').val(result.ConsFundingCategory);
        $('#tanumbr').val(result.TAN);
        $('#HiddenTAN').val(result.TAN);
        $('#gstnumbr').val(result.GSTNumber);
        $('#HiddenGST').val(result.GSTNumber);
        $('#HiddenTAN').val(result.TAN);
        $('#HiddenPAN').val(result.PAN);
        $('#panumbr').val(result.PAN);
        $('#HiddenPAN').val(result.PAN);
        if (result.ConsProjectTaxType_Qust_1 == 2 || result.ConsProjectTaxType_Qust_1 == 3) {
            $('#reasonfornotax').show();
            $('#proofofnotax').show();
            $('#proofofnotaxlink').show();
            $('#taxnumberdtls').show();
            $('#TaxException').val('True');
        }

        if (result.ConsProjectTaxType_Qust_1 == 1) {
            $('#reasonfornotax').hide();
            $('#proofofnotax').hide();
            $('#proofofnotaxlink').hide();
            $('#taxnumberdtls').show();
            $('#TaxException').val('False');
        }
        if (result.ConsProjectTaxType_Qust_1 == null) {
            $('#reasonfornotax').hide();
            $('#proofofnotax').hide();
            $('#proofofnotaxlink').hide();
            $('#taxnumberdtls').hide();
            $('#TaxException').val('False');
        }
    }
    // Consultancy - "Foreign"
    if (result.ProjectType == 2 && (result.ConsProjectFundingType_Qust_1 == 2 || result.ConsProjectFundingType_Qust_1 == 3)) {
        $('#ConsFundTypeForgnAndBoth').val('True');
        //  $('input[name=ConsProjectTaxType_Qust_1][value=' + result.ConsProjectTaxType_Qust_1 + ']').attr('checked', 'checked');
        $('#constaxtype').val(result.ConsProjectTaxType_Qust_1);
        $('#ConsSelCurr').val(result.ConsSelCurr);
        $('#ConsConversionRate').val(result.ConsConversionRate);
        $('#ConsForiegnCurrencyRate').val(result.ConsForiegnCurrencyRate);
        $('.conforgnPrjctDet').css("display", "block");
        $("#consultprjctsubtype").hide();
        // $('input[name=ProjectFundingType_Qust_1][value=' + result.ProjectFundingType_Qust_1 + ']').attr('checked', 'checked');                    
        //$('#conspjctfndngctgry').val(result.ConsFundingCategory);
        $('#conspjctfndngctgry').val(result.ConsFundingcategoryname);
        $('input[name="ConsFundingcategoryname"]').val(result.ConsFundingcategoryname);
        $('#consprjctfundingtype').val(result.ConsProjectFundingType_Qust_1);
        $('#indprjctcategory_1').css("display", "none");
        if (result.ConsProjectTaxType_Qust_1 == 2 || result.ConsProjectTaxType_Qust_1 == 3) {
            $('#reasonfornotax').show();
            $('#proofofnotax').show();
            $('#proofofnotaxlink').show();
            $('#taxnumberdtls').show();
            $('#gstnumbr').val(result.GSTNumber);
            $('#HiddenGST').val(result.GSTNumber);
            $('#HiddenTAN').val(result.TAN);
            $('#HiddenPAN').val(result.PAN);
            $('#tanumbr').val(result.TAN);
            $('#panumbr').val(result.PAN);
        }

        if (result.ConsProjectTaxType_Qust_1 == 1) {
            $('#reasonfornotax').hide();
            $('#proofofnotax').hide();
            $('#proofofnotaxlink').hide();
            $('#taxnumberdtls').show();
            $('#gstnumbr').val(result.GSTNumber);
            $('#HiddenGST').val(result.GSTNumber);
            $('#HiddenTAN').val(result.TAN);
            $('#HiddenPAN').val(result.PAN);
            $('#panumbr').val(result.PAN);
            $('#tanumbr').val(result.TAN);
        }
    }

    if (result.ProjectType == 2) {


        if (result.ConsProjectTaxType_Qust_1 == 1 || result.ConsProjectTaxType_Qust_1 == 0) {
            $("#reasonfornotax").hide();
            $('#proofofnotax').hide();
            $('#proofofnotaxlink').hide();
            $('#taxnumberdtls').show();
            $('#gstnumbr').val(result.GSTNumber);
            $('#HiddenGST').val(result.GSTNumber);
            $('#HiddenTAN').val(result.TAN);
            $('#HiddenPAN').val(result.PAN);
            $('#panumbr').val(result.PAN);
            $('#tanumbr').val(result.TAN);
        }
        if (result.ConsProjectTaxType_Qust_1 == 2 || result.ConsProjectTaxType_Qust_1 == 3) {
            $("#reasonfornotax").show();
            $('#proofofnotax').show();
            $('#proofofnotaxlink').show();
            $('#taxnumberdtls').show();
            $('#constaxtype').val(result.ConsProjectTaxType_Qust_1);
            $('#txtreasonfornotax').val(result.ConsProjectReasonfornotax);
            $('#Docpathfornotax').val(result.Docpathfornotax);
            $('input[name="Docpathfornotax"]').val(result.Docpathfornotax);
            $('#gstnumbr').val(result.GSTNumber);
            $('#HiddenGST').val(result.GSTNumber);
            $('#HiddenTAN').val(result.TAN);
            $('#HiddenPAN').val(result.PAN);
            $('#panumbr').val(result.PAN);
            $('#tanumbr').val(result.TAN);
            var proofdocname = result.taxprooffilename;
            var proofdocpath = result.Docpathfornotax;
            //$('#proofdoclink').text = proofdocname;
            //$('#proofdoclink').href = "ShowDocument?file=" + proofdocpath + "&filepath=~%2FContent%2FSupportDocuments%2F";
            document.getElementsByClassName('prooflink')[0].text = proofdocname;
            document.getElementsByClassName('prooflink')[0].href = "ShowDocument?file=" + proofdocpath + "&filepath=~%2FContent%2FSupportDocuments%2F";
        }

    }

    $('#txtPIemail').val(result.PIEmail);
    $('#Agncy').val(result.SponsoringAgency);
    $('#txtagencycode').val(result.AgencyCode);
    $('input[name="AgencyCodeid"]').val(result.AgencyCodeid);



    $('#txtprjctduratn').val(result.Projectduration);
    $('#schemename').val(result.Schemename);
    $('#txtpersonapplied').val(result.Personapplied);
    $('#txtremarks').val(result.Remarks);
    $('.selectpicker').selectpicker('refresh');

    $('#CollaborativeProjectType').val(result.CollaborativeProjectType);
    $('#collabcoordname').val(result.Collaborativeprojectcoordinator);
    $('#collabagency').val(result.CollaborativeprojectAgency);
    $('#collabcoordemail').val(result.Collaborativeprojectcoordinatoremail);
    $('#collabtotalcost').val(result.Collaborativeprojecttotalcost);
    $('#collabIITMCost').val(result.CollaborativeprojectIITMCost);
    //$('#txtagncyconperson').val(result.Agencycontactpersonaddress);
    $('#ConsSelCurr').val(result.ConsSelCurr);
    $('#ConsConversionRate').val(result.ConsConversionRate);
    $('#ConsForiegnCurrencyRate').val(result.ConsForiegnCurrencyRate);
    $('#taxserviceGSTnum').val(result.TaxserviceGST);
    $('#taxserviceregstatus').val(result.Taxserviceregstatus);
    //$('#txtprpslval').val(result.Sanctionvalue);
    $('#basevalue').val(result.BaseValue);
    $('#applicabletax').val(result.ApplicableTax);
    $('#sumstaffs').val(result.SumofStaffs);
    $('#sumstaffssalry').val(result.SumSalaryofStaffs);
    $('#txtagncyconperson').val(result.Agencycontactperson);
    $('#txtagncycntctdesig').val(result.Agencycontactpersondesignation);
    if (result.JointDevelopment_Qust_1 == "Yes") {
        $('input[name=JointDevelopment_Qust_1][value=' + result.JointDevelopment_Qust_1 + ']').attr('checked', 'checked');
        $('#jointdevelopmentdetail').css("display", "block");
    }
    var ProjectCategoryId = result.StaffCategoryID;
    var ProjectCategory = result.CategoryofStaffs;
    var NoofProjectstaffs = result.NoofStaffs;
    var SalaryofProjectstaffs = result.SalaryofStaffs;
    if (result.InterestApplicable == "Yes") {
        $('input[name=InterestApplicable][value="' + result.InterestApplicable + '"]').attr('checked', 'checked');
        $('input[name=InterestRefund][value="' + result.InterestRefund + '"]').attr('checked', 'checked');
        $('input[name=ModeofRefund][value="' + result.ModeofRefund + '"]').attr('checked', 'checked');
        if (result.ModeofRefund == "DD") {
            $('#divDDInfav').removeClass("dis-none");
            $('#DDInFavourOf').val(result.DDInFavourOf);
        } else {
            $('#divDDInfav').addClass('dis-none');
        }
        $('#divIntrApp').removeClass("dis-none");
        $('input[name="InterestRefund"],input[name="ModeofRefund"]').addClass("validate-req");
    } else {
        $('input[name=InterestApplicable][value=No]').attr('checked', 'checked');
        $('input[name=InterestRefund]').attr('checked', false);
        $('input[name=ModeofRefund]').attr('checked', false);
        $('#DDInFavourOf').val('');
        $('#divIntrApp').addClass('dis-none');
        $('input[name="InterestRefund"],input[name="ModeofRefund"]').removeClass("validate-req");
    }
    $.each(ProjectCategory, function (i, val) {
        if (i == 0) {
            document.getElementsByName('StaffCategoryID')[0].value = ProjectCategoryId[0];
            document.getElementsByName('CategoryofStaffs')[0].value = ProjectCategory[0];
            document.getElementsByName('NoofStaffs')[0].value = NoofProjectstaffs[0];
            document.getElementsByName('SalaryofStaffs')[0].value = SalaryofProjectstaffs[0];

        } else {
            var cln = $("#primaryStaffDiv").clone().find("input").val("").end();
            $(cln).find('.dis-none').removeClass('dis-none');
            $('#divStaffContent').append(cln)
            document.getElementsByName('StaffCategoryID')[i].value = ProjectCategoryId[i];
            document.getElementsByName('CategoryofStaffs')[i].value = ProjectCategory[i];
            document.getElementsByName('NoofStaffs')[i].value = NoofProjectstaffs[i];
            document.getElementsByName('SalaryofStaffs')[i].value = SalaryofProjectstaffs[i];

        }
    });


    var Companyid = result.JointDevelopmentCompanyId;
    var Companyname = result.JointDevelopmentCompany;
    var Remarks = result.JointDevelopmentRemarks;

    $.each(Companyname, function (i, val) {
        if (i == 0) {
            document.getElementsByName('JointDevelopmentCompanyId')[0].value = Companyid[0];
            document.getElementsByName('JointDevelopmentCompany')[0].value = Companyname[0];
            document.getElementsByName('JointDevelopmentRemarks')[0].value = Remarks[0];

        } else {
            var cln = $("#primaryjoindevelopDiv").clone().find("input").val("").end();
            //$(cln).find('.dis-none').removeClass('dis-none');
            $('#divjointdevelopContent').append(cln)
            document.getElementsByName('JointDevelopmentCompanyId')[i].value = Companyid[i];
            document.getElementsByName('JointDevelopmentCompany')[i].value = Companyname[i];
            document.getElementsByName('JointDevelopmentRemarks')[i].value = Remarks[i];

        }
    });

    var CoPIName = result.CoPIname;
    var CoPIDep = result.CoPIDepartment;
    var CoPIEmail = result.CoPIEmail;
    var CoPIID = result.CoPIid;
    var CoPIRMF = result.CoPIRMF;
    var CoPIPCF = result.CoPIPCF;

    $.each(CoPIName, function (i, val) {
        if (i == 0) {
            var select = $("#CoPI");
            select.empty();
            $.each(JSON.parse(piList), function (index, itemData) {
                if (itemData.code == CoPIDep[0]) {
                    select.append($('<option/>', {
                        value: itemData.id,
                        text: itemData.name
                    }));
                }
            });
            $('#CoPI').val(CoPIName[0]);
            select.selectpicker('refresh');
            document.getElementsByName('CoPIname')[0].value = CoPIName[0];
            document.getElementsByName('CoPIDepartment')[0].value = CoPIDep[0];
            document.getElementsByName('CoPIEmail')[0].value = CoPIEmail[0];
            document.getElementsByName('CoPIid')[i].value = CoPIID[i];
            document.getElementsByName('CoPIRMF')[0].value = CoPIRMF[0];
            document.getElementsByName('CoPIPCF')[0].value = CoPIPCF[0];
        } else {
            var cln = $("#primaryDiv").clone().find("input").val("").end();
            var cloneElement = $("#primaryDiv").find('#CoPI').parent().clone();
            var dptPIs = []
            $.each(JSON.parse(piList), function (index, itemData) {
                if (itemData.code == CoPIDep[i]) {
                    dptPIs.push(itemData)
                }
            });
            cln.find('#CoPI').parent().replaceWith(selectPickerApiElement($(cloneElement), "all", dptPIs, CoPIName[i]));
            $(cln).find('.dis-none').removeClass('dis-none');
            $(cln).insertBefore($('#divContent').find('.p-b-sm'))
            document.getElementsByName('CoPIname')[i].value = CoPIName[i];
            document.getElementsByName('CoPIDepartment')[i].value = CoPIDep[i];
            document.getElementsByName('CoPIEmail')[i].value = CoPIEmail[i];
            document.getElementsByName('CoPIid')[i].value = CoPIID[i];
            document.getElementsByName('CoPIRMF')[i].value = CoPIRMF[i];
            document.getElementsByName('CoPIPCF')[i].value = CoPIPCF[i];
        }
    });

    var OtherInstCoPIName = result.OtherInstituteCoPIName;
    var OtherInstCoPIRemarks = result.RemarksforOthrInstCoPI;
    var OtherInstCoPIInst = result.CoPIInstitute;
    var OtherInstCoPIDep = result.OtherInstituteCoPIDepartment;
    var OtherInstCoPIID = result.OtherInstituteCoPIid;
    if (OtherInstCoPIName != null) {
        $.each(OtherInstCoPIName, function (i, val) {
            if (i == 0) {
                document.getElementsByName('OtherInstituteCoPIName')[0].value = OtherInstCoPIName[0];
                document.getElementsByName('CoPIInstitute')[0].value = OtherInstCoPIInst[0];
                document.getElementsByName('OtherInstituteCoPIDepartment')[0].value = OtherInstCoPIDep[0];
                document.getElementsByName('RemarksforOthrInstCoPI')[0].value = OtherInstCoPIRemarks[0];
                document.getElementsByName('OtherInstituteCoPIid')[0].value = OtherInstCoPIID[0];
            } else {
                var cln = $("#primaryotherinstcopiDiv").clone().find("input").val("").end();
                var cloneElement = $("#primaryotherinstcopiDiv").find('#OtherinstituteCoPI').parent().clone();
                $(cln).find('.dis-none').removeClass('dis-none');
                $(cln).insertBefore($('#divotherinstcopiContent').find('.p-b-sm'));
                document.getElementsByName('OtherInstituteCoPIName')[i].value = OtherInstCoPIName[i];
                document.getElementsByName('CoPIInstitute')[i].value = OtherInstCoPIInst[i];
                document.getElementsByName('OtherInstituteCoPIDepartment')[i].value = OtherInstCoPIDep[i];
                document.getElementsByName('RemarksforOthrInstCoPI')[i].value = OtherInstCoPIRemarks[i];
                document.getElementsByName('OtherInstituteCoPIid')[i].value = OtherInstCoPIID[i];
            }
        });
    }

    if (!result.IsYearWiseAllocation) {
        $(".select2-hidden-accessible").select2('destroy');
        $("#ttlallocatedvalue").val(result.Allocationtotal);
        var AllocateHead = result.Allocationhead;
        var AllocateValue = result.Allocationvalue;
        var Subhead = result.ConcatSubhead;
        var EMIValue = result.ArrayEMIValue;
        var ttlAllowVal = 0;
        $.each(AllocateHead, function (i, val) {
            var subheadStr = Subhead[i] || '';
            var subheadList = subheadStr ='' ? [] : subheadStr.split(';');
            var parseVal = parseFloat(AllocateValue[i]);
            var allocatType = '';
            if (!isNaN(parseVal))
                ttlAllowVal = ttlAllowVal + parseVal;
            else
                parseVal = 0;
            $.each(JSON.parse(allocationHeads), function (j, item) {
                if (item.id == AllocateHead[i]) {
                    allocatType = item.code
                    return false;
                }
            });
            if (i == 0) {
                document.getElementsByName('Allocationhead')[0].value = AllocateHead[0];
                document.getElementsByName('Allocationvalue')[0].value = parseVal;
                $('#lblRecurring').html(allocatType);
                var eleFirst = $('#subhead_0');// document.getElementsByName('Subhead')[0];
                $.each(subheadList, function (j, item) {
                    eleFirst.append($('<option>', {
                        value: item,
                        text: item
                    }).attr('selected', 'selected'));
                });
                //eleFirst.find("option").each(function(opt) {
                //    $(opt).attr('selected', 'selected');
                //});
            } else {
                var cln = $("#primaryAllocateDiv").clone().find("input").val("").end();
                $(cln).find('label[id ="lblRecurring"]').html(allocatType);
                $(cln).find('.dis-none').removeClass('dis-none');
                $(cln).insertBefore($('#divAllocateContent').find('.p-b-sm'));
                document.getElementsByName('Allocationhead')[i].value = AllocateHead[i];
                document.getElementsByName('Allocationvalue')[i].value = parseVal;
                cln.find(".select2-subhead").empty();
                var indx = Math.random().toString(36).substring(2) + (new Date()).getTime().toString(36);
                cln.find(".select2-subhead").attr("id", "subhead_" + indx);
                var eleSel = cln.find(".select2-subhead");
                $.each(subheadList, function (j, item) {
                    eleSel.append($('<option>', {
                        value: item,
                        text: item
                    }).attr('selected', 'selected'));
                });
                //eleSel.find("option").each(function (opt) {
                //    $(opt).attr('selected', 'selected');
                //});
            }
        });
        $('#lblTtlVal').html(ttlAllowVal);
        $('#NoOfEMI').val(result.NoOfEMI);
        $('#lblEMIVal').html('0');
        $('#divOADynEMI').html('');
        var ttlEMIVal = 0;
        if (EMIValue != null) {
            for (var i = 1; i <= EMIValue.length; i++) {
                var parseVal = parseFloat(EMIValue[i - 1]);
                if (!isNaN(parseVal))
                    ttlEMIVal = ttlEMIVal + parseVal;
                var staticDiv = $('#divStaticOAEMI').clone();
                staticDiv.find('input[name="EMIValue"]').attr("name", "ArrayEMIValue").val(parseVal);
                staticDiv.removeClass('dis-none');
                staticDiv.find('label[name="lblEMI"]').html('Installment ' + i);
                $('#divOADynEMI').append(staticDiv);
            }
        }
        $('#lblEMIVal').html(ttlEMIVal);
        $('#YWHead').addClass('dis-none');
        $('#OAHead').removeClass('dis-none');
    } else {
        $('#YWHead').removeClass('dis-none');
        $('#OAHead').addClass('dis-none');
        createYWHeadTab();
        var datYW = result.YearWiseHead;
        $('#IsYearWiseAllocation').prop('checked', true);
        $(".select2-hidden-accessible").select2('destroy');
        $.each(datYW, function (i, data) {
            var ttlAllowVal = 0;
            var ttlEMIVal = 0;
            var year = datYW[i].Year;
            $('#Year' + year + ' div[name="primaryAllocateDiv"]').not(':first').remove();
            $('#Year' + year).find('input[name$=".Year"]').val(year);
            $.each(data.AllocationValueYW, function (j, allocatVal) {
                var parseVal = parseFloat(allocatVal);
                var allocatType = '';
                if (!isNaN(parseVal))
                    ttlAllowVal = ttlAllowVal + parseVal;
                else
                    parseVal = 0;
                $.each(JSON.parse(allocationHeads), function (k, item) {
                    if (item.id == data.AllocationHeadYW[j]) {
                        allocatType = item.code
                        return false;
                    }
                });
                var subheadStr = data.ConcatSubheadYW[j] || '';
                var subheadList = subheadStr = '' ? [] : subheadStr.split(';');
                if (j == 0) {
                    $('#Year' + year).find('select[name$=".AllocationHeadYW"]:first').val(data.AllocationHeadYW[j]);
                    $('#Year' + year).find('input[name$=".AllocationValueYW"]:first').val(allocatVal);
                    $('#Year' + year).find('label[name="lblRecurring"]:first').html(allocatType);
                    var eleFirst = $('#Year' + year).find('select[name$=".SubheadYW"]:first');
                    $.each(subheadList, function (j, item) {
                        eleFirst.append($('<option>', {
                            value: item,
                            text: item
                        }).attr('selected', 'selected'));
                    });
                } else {
                    var cln = $('#Year' + year).find('div[name="primaryAllocateDiv"]:first').clone().find("input, select").val("").end();

                    cln.find(".select2-subheadYW").empty();
                    var indx = Math.random().toString(36).substring(2) + (new Date()).getTime().toString(36);
                    cln.find(".select2-subheadYW").attr("id", "subhead_" + indx)
                    var eleSel = cln.find(".select2-subheadYW");
                    $.each(subheadList, function (j, item) {
                        eleSel.append($('<option>', {
                            value: item,
                            text: item
                        }).attr('selected', 'selected'));
                    });

                    $(cln).find('label[name ="lblRecurring"]').html(allocatType);
                    $(cln).find('select[name$=".AllocationHeadYW"]').val(data.AllocationHeadYW[j]);
                    $(cln).find('input[name$=".AllocationValueYW"]').val(allocatVal);
                    $(cln).find('.dis-none').removeClass('dis-none');
                    $(cln).insertBefore($('#Year' + year + ' div[name="divAllocateContent"]').find('.p-b-sm'));
                }
            });
            $('#Year' + year).find('label[name="lblTtlYearVal"]').html(ttlAllowVal);

            $('#Year' + year).find('input[name$=".NoOfInstallment"]').val(datYW[i].NoOfInstallment);
            $('#Year' + year).find('input[name$=".EMIValueForYear"]').val(datYW[i].EMIValueForYear);
            $('#Year' + year).find('div[name="divDynEMI"]').html('');
            $.each(data.EMIValue, function (j, emiVal) {
                var parseVal = parseFloat(emiVal);
                var emiNo = j + 1;
                if (!isNaN(parseVal))
                    ttlEMIVal = ttlEMIVal + parseVal;
                else
                    parseVal = 0;

                var staticDiv = $('#divStaticEMI:first').clone();
                staticDiv.find('input[name="EMIValue"]').val(parseVal).attr("name", "YearWiseHead[" + i + "].EMIValue");
                staticDiv.removeClass('dis-none');
                staticDiv.find('label[name="lblEMI"]').html('Installment ' + emiNo);
                $('#Year' + year).find('div[name="divDynEMI"]').append(staticDiv);
            });
            $('#Year' + year).find('label[name="lblEMIYearVal"]').html(ttlEMIVal);
        });
    }

    $(".select2-subhead,.select2-subheadYW").select2({
        tags: true,
        width: '100%',
        placeholder: "Enter subhead"
    });

    calculateProjectTotal();

    var Docname = result.DocName;
    var Attachname = result.AttachName;
    var Doctype = result.DocType;
    var Docpath = result.DocPath;
    var DocID = result.Docid;
    var proj = result.MainProjectList;

    $.each(Docname, function (i, val) {

        if (i == 0) {
            document.getElementsByName('DocType')[0].value = Doctype[0];
            document.getElementsByName('AttachName')[0].value = Attachname[0];
            document.getElementsByName('Docid')[0].value = DocID[0];
            document.getElementsByClassName('link1')[0].text = Docname[0];
            document.getElementsByClassName('HiddenDocName')[0].value = Docname[0];
            document.getElementsByClassName('HiddenDocPath')[0].value = Docpath[0];
            document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FSupportDocuments%2F";

        } else {
            var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
            $(cln).find('.dis-none').removeClass('dis-none');
            $(cln).find('#firstremoveButton').addClass('dis-none');
            $(cln).insertBefore($('#DocdivContent').find('.p-b-sm'));
            document.getElementsByName('DocType')[i].value = Doctype[i];
            document.getElementsByName('AttachName')[i].value = Attachname[i];
            document.getElementsByName('Docid')[i].value = DocID[i];
            document.getElementsByClassName('link1')[i].text = Docname[i];
            document.getElementsByClassName('HiddenDocName')[i].value = Docname[i];
            document.getElementsByClassName('HiddenDocPath')[i].value = Docpath[i];
            document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FSupportDocuments%2F";
        }
    });
}
var selectPickerApiElement = function (el, choice, options, select) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
    } else if (choice == "empty") {
        $(el).find('.selectpicker').children().remove();
        $(el).find('.selectpicker').append("<option value=''>Select any</option>");
    } else {
        var selectOptionsLength = $(el).find('.selectpicker').children().length;
        for (var i = 1 ; i <= selectOptionsLength ; i++) {
            if (options == $(el).find('.selectpicker').children().eq(i).val()) {
                $(el).find('.selectpicker').children().eq(i).remove();
                break;
            } else {
                continue;
            }

        }

    }
    $(el).find('select').selectpicker('refresh');
    return $(el).children().first().unwrap();

}