$(function () {
    var GetAccessRights='AccessRights',
        AddAccessRights = 'AccessRightsadd';
    $('#roleaccessgrid').jsGrid({
        autoload: true,
        paging: true,
        editing: true,
        pageIndex: 1,
        pageSize: 3,
        controller: {
            insertItem: function (item) {
                input = {
                    Roleid:item.Roleid,
                    Read: item.Read,
                    Write: item.Write,
                    Delete: item.Delete,
                    Approve:item.Approve
                },
                $.ajax({
                    type: "Post",
                    url: AddAccessRights,
                    data: input,
                    dataType: "json",
                    success: function (data) {
                        $('#ddlfunction,#ddldepartment').prop("selectedIndex", 0);

                        $("#tbllist tbody tr>").remove();
                        alert('Add successfully')
                    }
                });
            },
        },
        fields: [
                   { type: "number", name: "sno", title: "S.No", align: "left", editing: false },
                   { type: "number", name: "Roleid", title: "Role Id", editing: false, align: "left" },
                   { type: "text", name: "Rolename", title: "Role Name", editing: false },
                   { type: "checkbox", name: "Read", title: "Read", editing: true},
                    { type: "checkbox", name: "Write", title: "Write", editing: true },
                   { type: "checkbox", name: "Delete", title: "Delete", editing: true },
                   { type: "checkbox", name: "Approve", title: "Approve", editing: true },
                    {type:"control"}
        ],
        
    });
   
    
});