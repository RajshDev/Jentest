$(function () {
    //Declare Proposal List

    var getProjectDetailsURL = 'GetClosedProjectList';
    // Get Proposal List for modal Popup
    
    var dbProject;
    GetProjectlist();
    //Get Project List grid
    $("#gridProjectList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbProject, function (project) {

                    return (!filter.ProjectNumber || project.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                   && (!filter.Projecttitle || project.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                   && (!filter.SponsoringAgencyName || project.SponsoringAgencyName.toLowerCase().indexOf(filter.SponsoringAgencyName.toLowerCase()) > -1)
                   && (!filter.NameofPI || project.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                   && (!filter.EmpCode || project.EmpCode.toLowerCase().indexOf(filter.EmpCode.toLowerCase()) > -1);

                });
            }

        },

        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width: "25px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "50px" },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false, width: "60px" },
            { type: "number", name: "Budget", title: "Budget Value", editing: false, width: "60px" },
            { type: "text", name: "SponsoringAgencyName", title: "Agency Name", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "60px" },
            //{ type: "text", name: "PIDepartmentName", title: "Department of PI", editing: false, width: "75px" },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "50px" },

            {
                type: "control", width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                }
            },
        ],
        
      
    });

    $("#gridProjectList").jsGrid("option", "filtering", false);
    $.ajax({
        type: "GET",
        url: getProjectDetailsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            // dataProposal = result;
            $("#gridProjectList").jsGrid({ data: result });
            $('#projectopening').hide();
            $('#gridlist').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });

    function GetProjectlist() {

        $.ajax({
            type: "GET",
            url: getProjectDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridProjectList").jsGrid({ data: result });
                $('#projectopening').hide();
                $('#gridlist').show();
                dbProject = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
   

});