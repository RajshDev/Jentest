$(function () {
    var Getagencylist = 'GetInternalAgency',
        Editagencylist = 'EditInternalAgency',
        Deleteagencylist = 'DeleteInternalAgency';
    var db;
    $('#Internalagencylist').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 10,
        filtering: true,
       
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (internalagency) {

                    return (!filter.InternalAgencyName || internalagency.InternalAgencyName.toLowerCase().indexOf(filter.InternalAgencyName.toLowerCase()) > -1)
                    && (!filter.InternalAgencyContactPerson || internalagency.InternalAgencyContactPerson.toLowerCase().indexOf(filter.InternalAgencyContactPerson.toLowerCase()) > -1)
                    && (!filter.InternalAgencyType || internalagency.InternalAgencyType.toLowerCase().indexOf(filter.InternalAgencyType.toLowerCase()) > -1)
                    && (!filter.InternalConatactEmail || internalagency.InternalConatactEmail.toLowerCase().indexOf(filter.InternalConatactEmail.toLowerCase()) > -1);

                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "InternalAgencyId", title: "Agency Id", editing: false, visible: false },
                    { type: "text", name: "InternalAgencyName", title: "Agency Name", editing: false },
                    //{ type: "number", name: "InternalAgencyContactPerson", title: "Contact Person" },
                    { type: "text", name: "InternalAgencyType", title: "Agency Type", editing: false },
                    //{ type: "text", name: "InternalConatactEmail", title: "Contact Email", editing: false },
                       {
                           type: "control", editButton: false, deleteButton: false, title: "Action", width: 80,
                           itemTemplate: function (value, item) {
                               var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                               statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "Delete", name: "Delete" }]
                               var $customSelect = $("<select>")
                                   .attr("class", "form-control").prop("selectedIndex", "")

                               $.each(statusList, function (index, itemData) {
                                   $customSelect.append($('<option/>', {

                                       value: itemData.id,
                                       text: itemData.name
                                   }));
                               });

                               $customSelect.change(function (e) {
                                   var selVal = $(this).val();

                                   if (selVal == "Edit") {
                                       $('#createagency,#btnUpdate,#internalagncy').show();
                                       $('#btnSave,#gridlist,#addnewpage').hide();
                                       $.ajax({
                                           type: "POST",
                                           url: Editagencylist,
                                           data: { agencyId: item.InternalAgencyId },
                                           success: function (result) {
                                               $("#txtinagencyid").val(result.InternalAgencyId);
                                               $("#txtinagencyname").val(result.InternalAgencyName);
                                               $("#txtagencycode").val(result.InternalAgencyCode);
                                               $("#txtinconperson").val(result.InternalAgencyContactPerson);
                                               $("#txtinconnum").val(result.InternalAgencyContactNumber);
                                               $("#txtinconemail").val(result.InternalConatactEmail);
                                               $("#txtinconadd").val(result.InternalAgencyAddress);
                                               $("#txtinagnregname").val(result.InternalAgencyRegisterName);
                                               $("#txtinagnregadd").val(result.InternalAgencyRegisterAddress);
                                               $("#txtinagndist").val(result.InternalDistrict);
                                               $("#txtinagnpincode").val(result.InternalPincode);
                                               $("#txtinagnstate").val(result.InternalAgencyState);
                                               $("#ddlProjectType").val(result.ProjectType);
                                               var Docname = result.DocumentName;
                                               var Attachname = result.AttachName;
                                               var Doctype = result.DocumentType;
                                               var Docpath = result.DocPath;
                                               var DocID = result.DocumentId;
                                               $.each(Docname, function (i, doc) {
                                                   if (i == 0) {
                                                       document.getElementsByName('DocumentType')[0].value = Doctype[0];
                                                       document.getElementsByName('DocumentId')[0].value = DocID[0];
                                                       document.getElementsByName('AttachName')[0].value = Attachname[0];
                                                       document.getElementsByClassName('link1')[0].text = Docname[0];
                                                       document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                                                   }
                                                   else {
                                                       var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
                                                       $(cln).find('.dis-none').removeClass('dis-none');
                                                       $('#DocdivContent').append(cln)
                                                       document.getElementsByName('DocumentType')[i].value = Doctype[i];
                                                       document.getElementsByName('DocumentId')[i].value = DocID[i];
                                                       document.getElementsByName('AttachName')[i].value = Attachname[i];
                                                       document.getElementsByClassName('link1')[i].text = Docname[i];
                                                       document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FAgencyDocument%2F";
                                                   }
                                               });

                                           },
                                           error: function (err) {
                                               console.log("error1 : " + err);
                                           }
                                       });

                                   } else if (selVal == "Delete") {
                                       var choice = confirm("Are you sure, Do you want to delete ?");
                                       if (choice === true) {
                                           $.ajax({
                                               type: "POST",
                                               url: Deleteagencylist,
                                               data: { agencyId: item.InternalAgencyId },
                                               success: function (result) {
                                                   if (result == 1) {

                                                       $('#createagency,#btnUpdate,#btnSave,#internalagncy').hide();
                                                       $('#gridlist').show();
                                                       $('#addnewpage').show();
                                                       Getagencylists();

                                                       $('#deletemodal').modal('show');
                                                   }

                                                   else {

                                                       $('#createagency,#btnUpdate,#btnSave,#categoryagy').hide();
                                                       $('#gridlist').show();
                                                       $('#addnewpage').show();
                                                       Getinternalagencylist();
                                                       $('#deletemodalerror').modal('show');
                                                   }

                                               },
                                               error: function (err) {
                                                   console.log("error1 : " + err);
                                               }
                                           });
                                       }
                                   }
                                   $(this).val("");
                                   return false;
                                   e.stopPropagation();
                               });

                               return $result.add($customSelect);
                           },

                           _createFilterSwitchButton: function () {
                               return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                           }
                       }

        ],
       
    });
    $("#Internalagencylist").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: Getagencylist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {

            $("#Internalagencylist").jsGrid({
                data: result
            });
            db = result;
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
    function Getinternalagencylist() {
        $.ajax({

            type: "GET",
            url: Getagencylist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#Internalagencylist").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
  
    }
    $('#btnSrchUser').on('click', function () {
       
        var input = {
            SearchAgencyName: $('#txtsrchname').val(),
            SearchAgencyCode: $('#txtSeaAgencyCode').val()
           
        }

        $.ajax({
            type: "Get",
            url: Getagencylist,
            data: input,
            dataType: "json",
            success: function (result) {
                console.log(result);
                $("#Internalagencylist").jsGrid({ data: result });
                //$('#gridlist').show();

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchUser').on('click', function () {
        $('#txtsrchname,#txtSeaAgencyCode').val('');
       
        Getinternalagencylist();
    });
});