$(function () {
    var allocationlist = 'GetAccountHeadList',
        Editaccounthead = 'EditAccountHead',
        Deleteaccounthead = 'DeleteAccountHead';
    var db;
    Getallocation();
    $('#headlist').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (alloc) {
                    return (!filter.AccountHead || alloc.AccountHead.toLowerCase().indexOf(filter.AccountHead.toLowerCase()) > -1)
                 });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "AccountHeadId", title: "Account HeadId", editing: false, visible: false },
                    { type: "text", name: "AccountHead", title: "Account Head", editing: false },
                    {  name: "AccountHeadCode", title: "Account Head Code", editing: false },
                    
                    {
                        type: "control",
                        editButton: false,
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    }

        ],
        //onItemEditing: function (args) {
        //    if (args.item.AccountHeadId > 0) {
        //        var accheadid = args.item.AccountHeadId;
               
        //    }
        //    $('#createhead,#btnUpdate').show();
        //    $('#btnSave,#gridlist').hide();
        //    $.ajax({
        //        type: "POST",
        //        url: Editaccounthead,
        //        data: { headid: accheadid },
        //        success: function (result) {
        //            console.log(result);
                   
        //            $("#txtacctheadid").val(result.AccountHeadId);
        //            $('.selectpicker').selectpicker('val', result.AccountGroupId);
        //            $("#txtaccthead").val(result.AccountHead);
        //            $("#txtacctheadcode").val(result.AccountHeadCode);
        //        },
        //        error: function (err) {
        //            console.log("error1 : " + err);
        //        }
        //    });

        //},
        onItemDeleting: function (args) {
            if (args.item.AccountHeadId > 0) {
                var accheadid = args.item.AccountHeadId;
            }

            $.ajax({
                type: "POST",
                url: Deleteaccounthead,
                data: { headid: accheadid },
                success: function (result) {

                    if (result == 1) {

                        $('#createhead,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getallocation();
                        $('#deletemodal').modal('show');
                    }
                    else if(result==2)
                    {
                        $('#createhead,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getallocation();
                        $('#alert').html("Your not Authorised to delete this head");
                        $('#Validation').modal('show');
                    }
                    else {

                        $('#createhead,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getallocation();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#headlist").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: allocationlist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
       
        success: function (result) {
            $("#headlist").jsGrid({
                data: result,
                type: "control",
                
            });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getallocation() {
        $.ajax({
            type: "GET",
            url: allocationlist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            
            success: function (result) {

                $("#headlist").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    }
    $('#btnSrchHead').on('click', function () {

        var input = {
            AccountHeadSearch: $('#txtAcctHeadSearch').val(),
            AccountHeadCodeSearch: $('#txtAcctCodeSearch').val(),
            AccountGroupIdSearch: $('#ddlAcctGroupSearch').val()
        }
        $.ajax({
            type: "GET",
            url: allocationlist,
            data:input,
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (result) {

                $("#headlist").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchHead').on('click', function () {
        $('#txtAcctHeadSearch,#txtAcctCodeSearch').val('');
        $('#ddlAcctGroupSearch').prop('selectedIndex', 0);
        Getallocation();
    });
});