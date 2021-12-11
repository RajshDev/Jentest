$(function () {
    var getInstuiteURL = "GetInstuitelist", Editinstitute = "GetEditInstuitelist", Deleteinstitute = "Deleteinstitute";
    var db;
    GetInstuitelist();
    $("#Institutelist").jsGrid({
        autoload: true,
        paging: true,
        pageIndex: 1,
        pageSize: 10,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (institute) {

                    return (!filter.Institutename || institute.Institutename.toLowerCase().indexOf(filter.Institutename.toLowerCase()) > -1)
                    && (!filter.Countryname || institute.Countryname.toLowerCase().indexOf(filter.Countryname.toLowerCase()) > -1)
                    && (!filter.Location || institute.Location.toLowerCase().indexOf(filter.Location.toLowerCase()) > -1)
                    && (!filter.State || institute.State.toLowerCase().indexOf(filter.State.toLowerCase()) > -1);

                });
            }

        },
        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width: "50px" },
            { type: "number", name: "InstituteId", title: "Institute Id", visible: false },
            { type: "text", name: "Institutename", title: "Institute name", editing: false,width:"200px" },
            { type: "text", name: "Countryname", title: "Country name", editing: false },
            { type: "text", name: "Location", title: "Location", editing: false },
            { type: "text", name: "State", title: "State", editing: false },
            
            {

                name: "logo",title:"Logo",

                itemTemplate: function (val, item) {
                    if (val == null) {
                        return $("<img>").attr("src", "../Content/IOASContent/img/Image_placeholder.png").css({ height: 50, width: 50 })
                    }
                    else {
                        return $("<img>").attr("src", "../Content/InstituteLogo/" + val).css({ height: 50, width: 50 })
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
            if (args.item.InstituteId > 0) {
                var Instituteid = args.item.InstituteId;
            }
            $.ajax({
                type: "POST",
                url: Editinstitute,
                data: { instituteid: Instituteid },
                success: function (result) {
                    $("#txtinstitutename").val(result.Institutename);
                    $("#txtinstitutecode").val(result.InstituteCode);
                    $("#txtinstituteid").val(result.InstituteId);
                    $("#txtaddress1").val(result.Address1);
                    $("#txtaddress2").val(result.Address2);
                    $("#txtcity").val(result.City);
                    $("#txtstate").val(result.State);
                    $("#txtzipcode").val(result.zipCode);
                    $("#country").val(result.selCountry);
                    $("#txtfirstname").val(result.FirstName);
                    $("#txtlastname").val(result.lastName);
                    $("#txtdesignation").val(result.contactDES);
                    $("#txtmobileno").val(result.ContactMobile);
                    $("#txtemail").val(result.Email);
                    //$("#linklogo").text(result.logo);
                    $('#btnSave,#addnewpage,#Institutelist').hide();
                    $('#btnupdate,#inscreate,#btndisplay').show();
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.InstituteId > 0) {
                var instituteid = args.item.InstituteId;
            }
            $.ajax({
                type: "POST",
                url: Deleteinstitute,
                data: { Instituteid: instituteid },
                success: function (result) {
                   
                    $('#Deletedmodal').modal('show');
                    
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#Institutelist").jsGrid("option", "filtering", false);
    $.ajax({
        type: "GET",
        url: getInstuiteURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
           
            $("#Institutelist").jsGrid({ data: result });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function GetInstuitelist(){
        $.ajax({
            type: "GET",
            url: getInstuiteURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                
                $("#Institutelist").jsGrid({ data: result });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    }
});