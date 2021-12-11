
$(function () {
    var getPIlist = 'GetPIlist', EditPIlist = 'EditPIlist', DeletePIlist = 'DeletePIlist';
    var db;
    GetPIList();
    $("#PIlist").jsGrid({
       paging: true,
        pageIndex: 1,
        pageSize: 10,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (user) {

                    return (!filter.Firstname || user.Firstname.toLowerCase().indexOf(filter.Firstname.toLowerCase()) > -1)
                    && (!filter.Email || user.Email.toLowerCase().indexOf(filter.Email.toLowerCase()) > -1)
                    && (!filter.DepartmentName || user.DepartmentName.toLowerCase().indexOf(filter.DepartmentName.toLowerCase()) > -1);

                });
            }

        },
        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width: "50px", filtering: false },
            { type: "number", name: "Userid", title: "UserId", visible: false },
            { type: "text", name: "Firstname", title: "Name", editing: false, width: "130px" },
            { type: "number", name: "RoleId", title: "Role Id", visible: false },
            { type: "text", name: "Email", title: "Email", editing: false, width: "150px" },
            { type: "number", name: "DepartmentId", title: "Department Id", visible: false },
            { type: "text", name: "DepartmentName", title: "Department name", editing: false, width: "170px" },
            
            {

                name: "Image", width: "100px",

                itemTemplate: function (val, item) {
                    if (val == null) {
                        return $("<img>").attr("src", "../Content/IOASContent/img/Image_placeholder.png").css({ height: 50, width: 50 })
                    }
                    else {
                        return $("<img>").attr("src", "../Content/UserImage/" + val).css({ height: 50, width: 50 })
                    }
                },

            },


        {
            type: "control", width: "100px",
            _createFilterSwitchButton: function () {
                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
            }
        },



        ],
        onItemEditing: function (args) {

            // cancel editing of the row of item with field 'ID' = 0

            if (args.item.Userid > 0) {
                var userid = args.item.Userid;
            }

            $.ajax({
                type: "POST",
                url: EditPIlist,
                data: { UserId: userid },
                success: function (result) {
                    
                    $("#txtfirstname").val(result.Firstname);
                    $("#txtlastname").val(result.Lastname);
                    $("#ddlgender").val(result.Gender);
                    $("#Dateofbirth").val(result.DateOfbrith);
                    $("#txtemail").val(result.Email);
                    $("#txtuserid").val(result.UserId);
                    $("#ddlinstuitelist").val(result.InstituteId);
                    $("#ddldesignation").val(result.Designation);
                    $('#txtempcode').val(result.EMPCode);
                    $("#ddldepartment").val(result.Department);
                    $('#gridlist,#btnSave,#addnewpage').hide();
                    $('#createuser,#btnupdate').show();
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
        onItemDeleting: function (args) {
            if (args.item.Userid > 0) {
                var userid = args.item.Userid;
            }
            $.ajax({
                type: "POST",
                url: DeletePIlist,
                data: { UserId: userid },
                success: function (result) {
                    $('#gridlist').show();
                    $('#addnewpage').show();
                    $('#deletemodal').modal('show');

                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#PIlist").jsGrid("option", "filtering", false);
    $.ajax({
        type: "GET",
        url: getPIlist,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            
            $("#PIlist").jsGrid({ data: result });

            $('#gridlist').show();
            $('#addnewpage').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function GetPIList() {
        $.ajax({
            type: "GET",
            url: getPIlist,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#PIlist").jsGrid({ data: result });
                db = result;

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    }
});
