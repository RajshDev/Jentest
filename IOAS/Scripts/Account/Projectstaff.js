$(function () {
    var projectcatlist = 'Getprojectstafflist',
        Editprojectcate = 'EditProjectstaffcate',
        Deleteproject = 'Deleteprojectstaff';
    var db;
    getprojectlist();
    $('#projectstaff').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (pro) {
                    return (!filter.ProjectstaffCategory || pro.ProjectstaffCategory.toLowerCase().indexOf(filter.ProjectstaffCategory.toLowerCase()) > -1)

                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "ProjectstaffcategoryId", title: "Projectstaffcategory Id", editing: false, visible: false },
                    { type: "text", name: "ProjectstaffCategory", title: "Project staff Category", editing: false },

                    {
                        type: "control",
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    }

        ],
        onItemEditing: function (args) {
            if (args.item.ProjectstaffcategoryId > 0) {
                var projId = args.item.ProjectstaffcategoryId;

            }
            $('#createproj,#btnUpdate').show();
            $('#btnSave,#gridlist').hide();
            $.ajax({
                type: "POST",
                url: Editprojectcate,
                data: { Projid: projId },
                success: function (result) {
                    $('#txtprojectstaff').val(result.ProjectstaffCategory);
                    $('#txtprojectstaffcatid').val(result.ProjectstaffcategoryId);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.ProjectstaffcategoryId > 0) {
                var projId = args.item.ProjectstaffcategoryId;
            }

            $.ajax({
                type: "POST",
                url: Deleteproject,
                data: { Projid: projId },
                success: function (result) {

                    if (result == 1) {

                        $('#createproj,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        getprojectlist();
                        $('#deletemodal').modal('show');
                    }

                    else {

                        $('#createproj,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        getprojectlist();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#projectstaff").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: projectcatlist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {

            $("#projectstaff").jsGrid({
                data: result
            });

        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
     function getprojectlist() {
        $.ajax({

            type: "GET",
            url: projectcatlist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#projectstaff").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    }
});