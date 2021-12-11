
$(function () {
    var Getdepartmentlist = 'GetDepartmentlist',
        Editdepartment = 'GetEditDepartmentlist',
        Deletedeprtment = 'DeleteDepartment';
    var db;
    Getdepartment();
    $('#departmentgrid').jsGrid({
        paging: true,
        editing: true,
        pageIndex: 1,
        pageSize: 5,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (dept) {
                    return (!filter.Departmentname || dept.Departmentname.toLowerCase().indexOf(filter.Departmentname.toLowerCase()) > -1)
                        && (!filter.HOD || dept.HOD.toLowerCase().indexOf(filter.HOD.toLowerCase()) > -1)
                });
            }

        },
       fields: [
                    { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width: "50px", filtering: false },
                    { type: "number", name: "Departmentid", title: "Department Id", visible: false },
                    { type: "text", name: "Departmentname", title: "Department Name", editing: false},
                    { type: "text", name: "HOD", title: "HOD", editing: false },
                    {
                        type: "control",
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    },
                   
        ],
         
        onItemEditing: function (args) {
            if (args.item.Departmentid > 0) {
                var deptId = args.item.Departmentid;
            }
            $('#Adddepartment').show();
            $('#tblrowid').hide();
            $('#addnewstatusbar').hide();
            $('#btnSave').hide();
            $('#btnUpdate').show();
            $.ajax({
                type: "POST",
                url: Editdepartment,
                data: { DepartmentId: deptId },
                success: function (result) {
                    $('#txtdeptid').val(result.Departmentid);
                    $('#txtdepartment').val(result.Departmentname);
                    $('#txthod').val(result.HOD);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.Departmentid > 0) {
                var deptId = args.item.Departmentid;
            }
            $.ajax({
                type: "POST",
                url: Deletedeprtment,
                data: { DepartmentId: deptId },
                success: function (result) {
                   if (result == 1)
                    {
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        Getdepartment();
                       $('#deletemodal').modal('show');
                       
                    }
                    else if(result==2)
                    {
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        Getdepartment();
                        $('#warringmodal').modal('show');
                       
                    }
                    else {
                       
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        $('#Errormodal').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },

       
    });
    $("#departmentgrid").jsGrid("option", "filtering", false);
    $.ajax({
      
        type: "GET",
        url: Getdepartmentlist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            
            $("#departmentgrid").jsGrid({
                data: result
                
               
            });
            
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getdepartment() {

        $.ajax({

            type: "GET",
            url: Getdepartmentlist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#departmentgrid").jsGrid({
                    data: result


                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
    $('#btnSrchDept').on('click', function () {
        var input = {
            SearchDepartment: $('#txtsrchDept').val(),
            SearchHead: $('#txtHead').val()
        }
        $.ajax({

            type: "GET",
            url: Getdepartmentlist,
            data: input,
            dataType: "json",
            success: function (result) {
                $("#departmentgrid").jsGrid({
                    data: result
                });

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchDept').on('click', function () {
        $('#txtsrchDept,#txtHead').val('');
        Getdepartment();
    });
});
