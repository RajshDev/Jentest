$(function () {
    var consultancyfundinglist = 'GetConsfundinglist',
        Editconsultancyfunding = 'Editconscategoryfunding',
        Deleteconsfunding = 'Deleteconsfundingcategory';
    var db;
    Getconsultancyfunding();
    $('#consfundlist').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (cons) {
                    return (!filter.ConsultancyFundingcategory || cons.ConsultancyFundingcategory.toLowerCase().indexOf(filter.ConsultancyFundingcategory.toLowerCase()) > -1)

                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "ConsultancyFundingcategoryid", title: "ConsultancyFundingcategory Id", editing: false, visible: false },
                    { type: "text", name: "ConsultancyFundingcategory", title: "Consultancy funding category", editing: false },

                    {
                        type: "control",
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    }

        ],
        onItemEditing: function (args) {
            if (args.item.ConsultancyFundingcategoryid > 0) {
                var consid = args.item.ConsultancyFundingcategoryid;

            }
            $('#createfunding,#btnUpdate').show();
            $('#btnSave,#gridlist').hide();
            $.ajax({
                type: "POST",
                url: Editconsultancyfunding,
                data: { fundingId: consid },
                success: function (result) {
                    $('#txtalloctionid').val(result.ConsultancyFundingcategoryid);
                    $('#txtconsfunding').val(result.ConsultancyFundingcategory);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.ConsultancyFundingcategoryid > 0) {
                var consid = args.item.ConsultancyFundingcategoryid;
            }

            $.ajax({
                type: "POST",
                url: Deleteconsfunding,
                data: { fundingId: consid },
                success: function (result) {

                    if (result == 1) {

                        $('#createfunding,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getconsultancyfunding();
                        $('#deletemodal').modal('show');
                    }

                    else {

                        $('#createfunding,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getconsultancyfunding();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#consfundlist").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: consultancyfundinglist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (result) {

            $("#consfundlist").jsGrid({
                data: result
            });

        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getconsultancyfunding() {
        $.ajax({
            type: "GET",
            url: consultancyfundinglist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (result) {

                $("#consfundlist").jsGrid({
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