$(function () {
    var Getdepartmentrolelist = 'GetDepartmentrole',
        Editrolelist = 'GetEditRolelist',
        Deleterolelist = 'Deleterolelist';

    $('#rolelist').jsGrid({
        autoload: true,
        paging: true,
        editing: true,
        pageIndex: 1,
        pageSize: 5,
        
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left",width:"70px" },
                    { type: "text", name: "Roleid", title: "Role Id", editing: false,visible:false },
                    { type: "text", name: "Rolename", title: "Role Name", editing: false },
                    { type: "number", name: "Departmentid", title: "Department Id", visible: false },
                    { type: "text", name: "Departmentname", title: "Department Name", editing: false },
                    { type: "control" }

        ],
        onItemEditing: function (args) {
            if (args.item.Roleid > 0) {
                var Roleid = args.item.Roleid;
               
            }
            $('#Createrole,#btnUpdate').show();
            $('#btnSave,#Addrolegrid').hide();
            $.ajax({
                type: "POST",
                url: Editrolelist,
                data: { RoleId: Roleid },
                success: function (result) {
                   
                    $('#txtroleid').val(result.Roleid);
                    $('#txtrolename').val(result.Rolename);
                    $('#ddldepartment').val(result.Departmentid);
                    
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.Departmentid > 0) {
                var Roleid = args.item.Roleid;
            }
            
            $.ajax({
                type: "POST",
                url: Deleterolelist,
                data: { RoleId: Roleid },
                success: function (result) {
                    if (result == 1) {
                       
                        $('#Createrole,#btnUpdate,#btnSave').hide();
                        $('#Addrolegrid').show();
                        Getrolelist();
                        $('#deletedmodal').modal('show');
                    }
                    else if (result == 2) {
                       
                       $('#Createrole,#btnUpdate,#btnSave').hide();
                       $('#Addrolegrid').show();
                       Getrolelist();
                       $('#rolewarrning').modal('show');
                    }
                    else {
                        
                        $('#Createrole,#btnUpdate,#btnSave').hide();
                        $('#Addrolegrid').show();
                        Getrolelist();
                        $('#errormodal').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
        
    });
    $.ajax({

        type: "GET",
        url: Getdepartmentrolelist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            $("#rolelist").jsGrid({
                data: result
             });

        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getrolelist() {

        $.ajax({

            type: "GET",
            url: Getdepartmentrolelist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                $("#rolelist").jsGrid({
                    data: result
                });

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });


    }
    $('#btnSrchRole').on('click', function () {
        var input = {
            SearchRole: $('#txtsrchRole').val(),
            SearchDepartment: $('#ddlSeaDept').val()
        }
        $.ajax({

            type: "GET",
            url: Getdepartmentrolelist,
            data: input,
            dataType: "json",
            success: function (result) {
                $("#rolelist").jsGrid({
                    data: result
                });

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchRole').on('click', function () {
        $('#txtsrchRole').val('');
        $('#ddlSeaDept').prop("selectedIndex", 0);
        Getrolelist();
    });
});

