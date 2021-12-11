$(function () {
    var SRBItemcategory = 'SRBItemcategorylist', Editsrbitemcategory = 'editsrbitemcategory', Deletesrbcategory = 'deletesrbcategory';
    var db;
    GetSRBitemlist();
    $('#SRBitemcate').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (srbitmcat) {
                    return (!filter.Category || srbitmcat.Category.toLowerCase().indexOf(filter.Category.toLowerCase()) > -1)
                            
                });
            }

        },
    
        fields: [
                   { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                   { type: "number", name: "SRBItemCategotyId", title: "SRBItemCategotyId", editing: false, visible: false },
                   { type: "text", name: "Category", title: "Item category", editing: false },
                    { type: "checkbox", name: "Asset_f", title: "Asset_f", editing: false, filtering: false },

                   {
                       type: "control",
                       _createFilterSwitchButton: function () {
                           return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                       }
                   }

        ],
        onItemEditing: function (args) {
            debugger;
            if (args.item.SRBItemCategotyId > 0) {
                var srbitmid = args.item.SRBItemCategotyId;

            }
            $('#Srbitemcate,#btnUpdate').show();
            $('#btnSave,#gridlist').hide();
            $.ajax({
                type: "POST",
                url: Editsrbitemcategory,
                data: { srbitmcateid: srbitmid },
                success: function (result) {
                    console.log(result);
                    $('#txtsrbcategoryid').val(result.SRBItemCategotyId);
                    $('#txtsrbcategoryname').val(result.Category);
                    //$('#chkassetflag').val(result.Asset_f);
                    if (result.Asset_f == false) {
                        $('#chkassetflag').prop('checked', false);
                    }
                    else {
                        $('#chkassetflag').prop('checked', true);
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.SRBItemCategotyId > 0) {
                var srbitmid = args.item.SRBItemCategotyId;
            }

            $.ajax({
                type: "POST",
                url: Deletesrbcategory,
                data: { srbitmcateid: srbitmid },
                success: function (result) {

                    if (result == 1) {

                        $('#Srbitemcate,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        GetSRBitemlist();
                        $('#deletemodal').modal('show');
                    }

                    else {

                        $('#Srbitemcate,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        GetSRBitemlist();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#SRBitemcate").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: SRBItemcategory,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {

            $("#SRBitemcate").jsGrid({
                data: result
            });

        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
    function GetSRBitemlist()
    {
        $.ajax({

            type: "GET",
            url: SRBItemcategory,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#SRBitemcate").jsGrid({
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