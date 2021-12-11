var Getaccoutlist = 'GetAccountgrouplist',
    getaccountsubgroup = 'AddSubGroupCode';
var db;
$(function () {
    var getaccountgroupdelete = 'DeleteAccountGroup';
    
    $('#acctgrouplist').jsGrid({
        paging: true,
        editing: false,
        pageIndex: 1,
        pageSize: 5,
        filtering: true,
        controller: {

            loadData: function (filter) {
                console.log(filter);
                return $.grep(db, function (acctgup) {
                    return (!filter.AccountGroup || acctgup.AccountGroup.toLowerCase().indexOf(filter.AccountGroup.toLowerCase()) > -1)
                        && (!filter.Accounttypename || acctgup.Accounttypename.toLowerCase().indexOf(filter.Accounttypename.toLowerCase()) > -1)
                         && (!filter.AccountGroupCode || acctgup.AccountGroupCode === filter.AccountGroupCode)
                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "text", name: "AccountGroupId", title: "AccountGroup Id", editing: false, visible: false },
                    { type: "text", name: "AccountGroup", title: "Account Group", editing: false },
                    { type: "text", name: "AccountGroupCode", title: "Account group code", editing: false, align: "left", width: "70px" },
                    { type: "text", name: "Accounttypename", title: "Account type", editing: false },
                    //{ type: "number", name: "AccountType", title: "AccountType id", editing: false, visible: false },
                    
                    {
                        type: "control",
                        editButton: false,
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        },
                       //  itemTemplate: function (value, item) {
                       //     var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                       //     var $customButtonEdit = $("<button>")
                       //      //var $customButtonEdit = $("<button>")
                       //     //.attr("class", "ion-eye","type")
                       //          .attr({ class: "customGridDeletebutton jsgrid-button jsgrid-delete-button" })
                       //     .click(function () {
                       //         $('#createacctgroup,#btnUpdate,#btnSave').hide();
                       //         $('#btnSave,#gridlist,#addnewpage').show();
                       //         $.ajax({
                       //    type: "POST",
                       //    url: getaccountgroupdelete,
                       //    data: { acccountgrpId: item.AccountGroupId },
                          
                       //    success: function (result) {
                       //        if(result==1)
                       //        {
                       //            $('#deletewarrning').modal('show');
                       //            Getacctgroup();
                       //        }
                       //        else if(result==2)
                       //        {
                       //            $('#deletemodal').modal('show');
                       //            Getacctgroup();
                       //        }
                       //        else if(result==-1)
                       //        {
                       //            $('#Errormodal').modal('show');
                       //            Getacctgroup();
                       //        }
                       //    },

                       //    error: function (err) {
                       //        console.log("error1 : " + err);
                       //    }
                       //});
                       //e.stopPropagation();
                       //     });
                       //     return $("<div>").append($customButtonEdit).append($customButtonEdit);
                            // var $customButtonEdits = $("<button>")

                            ////.attr("class", "ion-edit")
                            //      .attr({ class: "customGridEditbutton jsgrid-button jsgrid-edit-button" })
                            //.click(function (e) {
                            //    $('#createacctgroup,#btnSave').show();
                            //    $('#gridlist').hide();
                            //    $('#addnewpage').hide();
                            //    $.ajax({
                            //        type: "Post",
                            //        url: getaccountsubgroup,
                            //        data: { accountgrpId: item.AccountGroupId },
                            //        success: function (result) {
                            //            $('#chkgroup').prop('checked', true);
                            //            $('#subgrp').show();
                            //            $('#lblacttype').hide();
                            //            $('#ddlaccounttype').hide();
                            //            $('#ddlparentgrp').text(result.parentgroupId);
                            //            $('#txtaccountgroupcode').val(result.AccountGroupCode);
                            //        },
                            //        error: function (err) {
                            //            console.log("error1 : " + err);
                            //        }
                            //    });
                              // e.stopPropagation();
                           // });
                    // return $result.add($customButton);
                             //return $("<div>").append($customButtonEdit).append($customButtonEdits);
               // }

                    }

        ],
 onItemDeleting: function (args) {
     if (args.item.AccountGroupId > 0) {
         var accgrpid = args.item.AccountGroupId;
            }

            $.ajax({
                type: "POST",
                url: getaccountgroupdelete,
                data: { acccountgrpId: accgrpid },
                success: function (result) {
                    if (result == 1) {
                        $('#deletewarrning').modal('show');
                        Getacctgroup();
                        $('#createacctgroup,#btnUpdate,#btnSave').hide();
                        $('#btnSave,#gridlist,#addnewpage').show();
                    }
                    else if (result == 2) {
                        $('#deletemodal').modal('show');
                        Getacctgroup();
                        $('#createacctgroup,#btnUpdate,#btnSave').hide();
                        $('#btnSave,#gridlist,#addnewpage').show();
                    }
                    else if (result == -1) {
                        $('#Errormodal').modal('show');
                        Getacctgroup();
                        $('#createacctgroup,#btnUpdate,#btnSave').hide();
                        $('#btnSave,#gridlist,#addnewpage').show();
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
        

    });
    $("#acctgrouplist").jsGrid("option", "filtering", false);
    Getacctgroup();
    $('#btnSrchGrp').on('click', function () {
        var input = {
            AccountGroupSearch: $('#txtAcctGroupSearch').val(),
            AccountGroupCodeSearch: $('#txtAcctGroupCodeSearch').val(),
            AccountTypeSearch: $('#ddlAcctTypeSearch').val()
        }
        $.ajax({

            type: "GET",
            url: Getaccoutlist,
            data: input,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#acctgrouplist").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchGrp').on('click', function () {
        $('#txtAcctGroupSearch,#txtAcctGroupCodeSearch').val('');
        $('#ddlAcctTypeSearch').prop('selectedIndex', 0);
        Getacctgroup();
    });
});
function Getacctgroup() {

    $.ajax({

        type: "GET",
        url: Getaccoutlist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {

            $("#acctgrouplist").jsGrid({
                data: result
            });
            db = result;
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });


}