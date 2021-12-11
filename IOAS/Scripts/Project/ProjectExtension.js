$(function () {
    //Declare Proposal List
    debugger;
    var getProjectDetailsURL = 'GetProjectList',
        getProposalDetailsURL = 'GetProposalList',
     EditProject = 'EditProject',
     Deleteproposal = 'Deleteproposal',
     getsrchProposalDetailsURL = 'GetSearchProposalList';

    // Get Proposal List for modal Popup

    $("#gridProposalList").jsGrid({
        autoload: true,
        width: 1000,
        paging: true,
        pageIndex: 1,
        async: false,
        pageSize: 5,
        editing: true,
        text: false,
        title: "Action",
        selecting: true,

        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width:"50px" },
            { type: "number", name: "ProposalID", title: "ProposalId", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProposalNumber", title: "Proposal Number", editing: false },
            { type: "decimal", name: "Budget", title: "Proposal Value", editing: false },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "60px" },
           
       {
           name: "ProposalID",
           title: "Action",
           itemTemplate: function (value, item) {
               return $("<a>").attr("href", "#").attr("class", "btn btn-primary").attr("value", "Select").text('Select').on("click", function () {
                   var proposalid = item.ProposalID;
                   var ProposalDetails = 'Loadproposaldetailsbyid';
                   $.ajax({

                       type: "POST",
                       url: ProposalDetails,
                       data: { ProposalId: proposalid },
                       //contentType: "application/json; charset=utf-8",
                       //dataType: "json",

                       success: function (result) {
                           console.log(result);
                           debugger;
                           
                           $("#Projecttypename").val(result.ProjectTypeName);
                           $('input[name="ProposalID"]').val(result.ProposalID);
                           $('#Prjcttype').val(result.ProjectType);
                           $('#propslnum').val(result.ProposalNumber);
                           $('#prjcttitle').val(result.Projecttitle);
                           $('#projectduration').val(result.Projectduration);

                           $('#department').val(result.Department);
                           $('#PI').val(result.PIname);
                           $('#PIdesig').val(result.PIDesignation);
                           $('#snctnvalue').val(result.Sanctionvalue);
                           $('.selectpicker').selectpicker('refresh');

                           if (result.ProjectType == 0) {
                               $('#prjcttypespon').hide();
                               $('#prjcttypeconsul').hide();
                               $("#sponintrnlscheme").addClass('dis-none');
                               $("#sponextrnlscheme").addClass('dis-none');
                               $('#indprjctcategory_1').css("display", "none");
                               $('#prjctdetails1').css("display", "none");
                               $('#prjctdetails2').css("display", "none");
                           }
                           if (result.ProjectType == 1) {
                               $('#prjcttypespon').show();
                               $('#prjcttypeconsul').hide();
                               $('#indprjctcategory_1').css("display", "none");
                               $('#prjctdetails1').css("display", "block");
                               $('#prjctdetails2').css("display", "none");
                               $('#taxnumberdtls').hide();
                               $('#constaxdetails').hide();
                               $('#constaxservicemode').hide();
                           }
                           if (result.ProjectType == 2) {
                               $('#prjcttypeconsul').show();
                               $('#prjcttypespon').hide();
                               $('#indprjctcategory_1').css("display", "none");
                               $('#prjctdetails1').css("display", "none");
                               $('#prjctdetails2').css("display", "block");                              
                               $('#constaxdetails').show();
                               $('#constaxservicemode').show();
                           }


                           var CoPIName = result.CoPIname;
                           var CoPIemail = result.CoPIEmail;
                           var CoPIDep = result.CoPIDepartment;
                           var CoPIID = result.CoPIid;
                           var CoPIDesig = result.CoPIDesignation;
                           $('#divContent #primaryDiv').not(':first').remove();
                           document.getElementsByName('CoPIname')[0].value = "";
                           document.getElementsByName('CoPIDepartment')[0].value = "";
                           document.getElementsByName('CoPIDesignation')[0].value = "";
                           $.each(CoPIName, function (i, val) {
                               if (i == 0) {
                                   document.getElementsByName('CoPIname')[0].value = CoPIName[0];
                                   document.getElementsByName('CoPIDepartment')[0].value = CoPIDep[0];
                                   document.getElementsByName('CoPIDesignation')[0].value = CoPIDesig[0];
                                   
                               } else {
                                   var cln = $("#primaryDiv").clone().find("input").val("").end();
                                   //$(cln).find('.dis-none').removeClass('dis-none');
                                   $('#divContent').append(cln)
                                   //cln.find('select').selectpicker();
                                   document.getElementsByName('CoPIname')[i].value = CoPIName[i];
                                   document.getElementsByName('CoPIDepartment')[i].value = CoPIDep[i];
                                   document.getElementsByName('CoPIDesignation')[i].value = CoPIDesig[i];
                                  // cln.find('.bootstrap-select').remove();
                                   document.getElementsByClassName("selectpicker")[i].selectpicker('refresh');
                               }
                               
                           });

                           
                       },

                       error: function (err) {
                           console.log("error1 : " + err);
                       }

                   });
                   $("#AddNewEntryModel").modal('hide');
                   
               });

           }
       }
       ],
        });           

    //Declare Proposal List grid
    $("#gridProjectList").jsGrid({
        autoload: true,
        width: 1000,
        paging: true,
        pageIndex: 1,
        async: false,
        pageSize: 5,
        editing: true,
        text: false,
        title: "Action",
        selecting: true,

        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width:"25px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "50px" },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false, width: "60px" },
            { type: "decimal", name: "Budget", title: "Budget Value", editing: false, width: "60px" },
            { type: "decimal", name: "SponsoringAgencyName", title: "Agency Name", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "60px" },
            //{ type: "text", name: "PIDepartmentName", title: "Department of PI", editing: false, width: "75px" },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "50px" },
            { type: "control", editButton: false, width: "25px" }
        ],
        onItemEditing: function (args) {
            
            // cancel editing of the row of item with field 'ID' = 0

            if (args.item.ProjectID > 0) {
                var projectid = args.item.ProjectID;
            }
            $('#addnewpage').hide();
            $('#gridlist').hide();
            $('#saveproject').hide();
            $('#projectopening').show();
            $('#updateproject').show();
            $.ajax({
                type: "POST",
                url: EditProject,
                data: { ProjectId: projectid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",
               
                success: function (result) {
                    console.log(result);
                    debugger;
                    $('input[name="ProposalID"]').val(result.ProposalID);
                    $('#propslnum').val(result.ProposalNumber);
                    $('#Prjcttype').val(result.ProjectType);
                    $('#datepickerid').val(result.Prpsalinwrddate);
                    $('#txtprpslsrc').val(result.ProposalSource);
                    $('#txtprjcttitle').val(result.Projecttitle);
                    
                    $('#department').val(result.Department);
                    $('#PI').val(result.PIname);
                    $('#txtPIemail').val(result.PIEmail);
                    $('#Agncy').val(result.SponsoringAgency);
                    $('#txtprpslval').val(result.Budget);

                   
                    $('#txtprjctduratn').val(result.Projectduration);
                    $('#schemename').val(result.Schemename);
                    $('#txtpersonapplied').val(result.Personapplied);
                    $('#txtremarks').val(result.Remarks);
                   
                    var CoPIName = result.CoPIname;
                    var CoPIemail = result.CoPIEmail;
                    var CoPIDep = result.CoPIDepartment;
                    var CoPIID = result.CoPIid;
                    $.each(CoPIName, function( i, val ){
                        if(i == 0){
                            document.getElementsByName('CoPIname')[0].value = CoPIName[0];
                            document.getElementsByName('CoPIDepartment')[0].value = CoPIDep[0];
                            document.getElementsByName('CoPIEmail')[0].value = CoPIemail[0];
                            document.getElementsByName('CoPIid')[0].value = CoPIID[0];
                        }else{
                            var cln = $("#primaryDiv").clone().find("input").val("").end();
                            //$(cln).find('.dis-none').removeClass('dis-none');
                            $('#divContent').append(cln)
                            document.getElementsByName('CoPIname')[i].value = CoPIName[i];
                            document.getElementsByName('CoPIDepartment')[i].value = CoPIDep[i];
                            document.getElementsByName('CoPIEmail')[i].value = CoPIemail[i];
                            document.getElementsByName('CoPIid')[i].value = CoPIID[i];
                        }
                    });

                    var Docname = result.DocName;
                    var Attachname = result.AttachName;
                    var Doctype = result.DocType;
                    var Docpath = result.DocPath;
                    var DocID = result.Docid;
                    
                    $.each(Docname, function( i, val ){
                        if(i == 0){
                            document.getElementsByName('DocType')[0].value = Doctype[0];
                            document.getElementsByName('AttachName')[0].value = Attachname[0];
                            document.getElementsByName('Docid')[0].value = DocID[0];
                            document.getElementsByClassName('link1')[0].text = Docname[0];
                            document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                        }else{
                            var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
                           // $(cln).find('.dis-none').removeClass('dis-none');
                            $('#divContent').append(cln)
                            document.getElementsByName('DocType')[i].value = Doctype[i];
                            document.getElementsByName('AttachName')[i].value = Attachname[i];
                            document.getElementsByName('Docid')[i].value = DocID[i];
                            document.getElementsByClassName('link1')[i].text = Docname[i];
                            document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                        }

                      //    ShowDocument?file=c61479f3-93e7-48b4-84a2-7f970e02c432_UAY2017_proposal_template 1.pdf&filepath=~%2FContent%2FProposalDocuments%2F
                    });
                    
                   // Fillproposal(result.ProposalId);
                    //$('#addnewpage').hide();
                    //$('#gridlist').hide();
                    //$('#saveproposal').hide();                   
                    //$('#createproposal').show();
                    //$('#updateproposal').show();
                   // $('#updateproposal').show();
                  
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
            
           

        },
        onItemDeleting: function(args) {
            if (args.item.ProjectID > 0) {
                var projectid = args.item.ProjectID;
            }
            $.ajax({
                type: "POST",
                url: Deleteproject,
                data: { ProjectId: projectid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {

                    window.onload();
                    //$('#createuserid').hide();                    
                    $('#createproposal').hide();
                    $('#saveproposal').hide();
                    $('#updateproposal').hide();
                    $('#gridlist').show();
                    if (result == 4)
                    {
                        $('#gridlist').show();
                        $('#myModal2').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        }
    });
   

    $.ajax({
        type: "GET",
        url: getProjectDetailsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
           // dataProposal = result;
            $("#gridProjectList").jsGrid({ data: result });
            $('#createproposal').hide();
            $('#gridlist').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
     
    
    $.ajax({
        type: "GET",
        url: getProposalDetailsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            // dataProposal = result;
            $("#gridProposalList").jsGrid({ data: result });
            $('#createproposal').hide();
            $('#gridproposal').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });

    
});