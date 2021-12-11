$(function () {
    var Schemelist = 'Getschemelist',
        Editschemelist = 'Editscheme',
        Deleteschemelist = 'deletescheme';
    var db;
    getschemelist();
    $('#Schemelist').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (pro) {
                    return (!filter.SchemeName || pro.SchemeName.toLowerCase().indexOf(filter.SchemeName.toLowerCase()) > -1)
                           && (!filter.Schemecode || pro.Schemecode.toLowerCase().indexOf(filter.Schemecode.toLowerCase()) > -1)
                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "SchemeId", title: "Scheme Id", editing: false, visible: false },
                    { type: "text", name: "SchemeName", title: "Scheme name", editing: false },
                    { type: "text", name: "Schemecode", title: "Scheme code", editing: false },
                    

                    {
                        type: "control",
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    }

        ],
        onItemEditing: function (args) {
            if (args.item.SchemeId > 0) {
                var schid = args.item.SchemeId;

            }
            $('#createscheme,#btnUpdate').show();
            $('#btnSave,#gridlist').hide();
            $.ajax({
                type: "POST",
                url: Editschemelist,
                data: { schemeid: schid },
                success: function (result) {
                    $('#txtscheme').val(result.SchemeName);
                    $('#txtschemeid').val(result.SchemeId);
                    $('#txtschemecode').val(result.Schemecode);
                    $('#ddlproject').val(result.ProjectType);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.SchemeId > 0) {
                var schid = args.item.SchemeId;
            }

            $.ajax({
                type: "POST",
                url: Deleteschemelist,
                data: { schemeid: schid },
                success: function (result) {

                    if (result == 1) {

                        $('#createscheme,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        getschemelist();
                        $('#deletemodal').modal('show');
                    }

                    else {

                        $('#createscheme,#btnUpdate,#btnSave').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        getschemelist();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#Schemelist").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: Schemelist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {

            $("#Schemelist").jsGrid({
                data: result
            });

        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
    function getschemelist() {
        $.ajax({

            type: "GET",
            url: Schemelist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#Schemelist").jsGrid({
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