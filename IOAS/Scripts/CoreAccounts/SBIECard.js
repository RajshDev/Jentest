$(function () {
    //Declare Proposal List
    var getSBIECardList = 'LoadSBIEcardList',
        editEcard = 'EditSBICard',
        viewEcard = 'ViewSBICard';
     
    var dbSBIECard;
    GetSBIECardlist();

   // jsGrid.fields.date = DateField;
    
    $("#gridSBIECardList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbSBIECard, function (invoice) {

                    return (!filter.SBIEcardNumber || invoice.SBIEcardNumber.toLowerCase().indexOf(filter.SBIEcardNumber.toLowerCase()) > -1)
                    && (!filter.Projecttitle || invoice.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                    && (!filter.NameofPI || invoice.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || invoice.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                        && (!filter.SBIEcardPjctDetlsNumber || invoice.SBIEcardPjctDetlsNumber.toLowerCase().indexOf(filter.SBIEcardPjctDetlsNumber.toLowerCase()) > -1)
                    && (!filter.TotalValueofCard || invoice.TotalValueofCard.toLowerCase().indexOf(filter.TotalValueofCard.toLowerCase()) > -1)
                    && (!filter.Status || invoice.Status.toLowerCase().indexOf(filter.Status.toLowerCase()) > -1)
                    ;
                });
            }
        },
        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "10px", editing: false },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false, editing: false },
            { type: "number", name: "SBIEcardId", title: "SBI Ecard Id", visible: false, editing: false },
            { type: "number", name: "SBIEcardProjectDetailsId", title: "SBI Ecard Project Details Id", visible: false, editing: false },
            //{ type: "text", name: "CardExpryDte", editing: false, title: "Card Valid till", align: "center", width: "60px" },
            { type: "text", name: "SBIEcardNumber", editing: false, title: "SBI Prepaid ECard Number", width: "70px" },
           // { type: "text", name: "SBIEcardPjctDetlsNumber", editing: false, title: "SBI Prepaid Reference Number", width: "70px" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "90px" },            
            { type: "decimal", name: "CurrentProjectAllotmentValue", title: "Allocated Amount", editing: false, width: "55px" },
            { name: "Status", title: "Status", editing: false, width: "55px" },
            { name: "IsRecoupmentpending", title: "IsRecoupmentPending", visible: false, editing: false, width: "35px" },
          //  { type: "select", name: "Action", title: "Action", editing: false, width: "55px" },
          //{ name: "Type", type: "select", items: ["Select","Approve"], validate: "required" },
        {
            type: "control", editButton: false, deleteButton: false, width: "60px", 
            itemTemplate: function (value, item) {
                var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                if (item.Status == "Open") {
                    statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "Submit for Approval", name: "Submit for Approval" }, { id: "View", name: "View" }]
                }
                else if (item.Status == "Approved") {
                    statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }, { id: "Bill Booking", name: "Bill Booking" }]
                }
                else {
                    statusList = [{ id: "", name: "Select Action" }, { id: "View", name: "View" }]
                }
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
                        // cancel editing of the row of item with field 'ID' = 0
                        var url = 'SBIECard?ProjectId=' + item.ProjectID + '&CardID=' + item.SBIEcardId;
                        window.location.href = url;
                        //if (item.SBIEcardId > 0) {
                        //    var EcardId = item.SBIEcardId;
                        //    var ProjectId = item.ProjectID;
                        //}
                        //$('#addnewpage').hide();
                        //$('#gridlist').hide();
                        //// $('#saveproposal').hide();
                        //$("#SBIPrepaidECard").show();
                        //$('#NewSBIPrepaidECard').show();
                        //$('#addnewpage').hide();
                        ////  $('#updateproposal').show();
                        //$.ajax({
                        //    type: "POST",
                        //    url: editEcard,
                        //    data: { SBICardId: EcardId, ProjectId: ProjectId },
                        //    //contentType: "application/json; charset=utf-8",
                        //    //dataType: "json",

                        //    success: function (result) {
                        //        
                        //        $("#ExistingSBIPrepaidECard").hide();
                        //        $("#NewSBIPrepaidECard").show();
                        //        $("#SBIPrepaidECard").show();
                        //        $('input[name="CardDocfilepath"]').val(result.CardDocfilepath);
                        //        $('input[name="SBIEcardId"]').val(result.SBIEcardId);
                        //        $('input[name="ProjectID"]').val(result.ProjectID);
                        //        $('input[name="CardExpryDte"]').val(result.CardExpryDte);
                        //        $("#CardExpdate").html(result.CardExpryDte);
                        //        $("#txtPIname").html(result.NameofPI);
                        //        $('input[name="NameofPI"]').val(result.NameofPI);
                        //        $("#txtPIdepartment").html(result.PIDepartmentName);
                        //        $("#txtprepaidcardnumber").val(result.SBIEcardNumber);
                        //        $("#txttotalcardvalue").val(result.TotalValueofCard);
                        //        $("#txtcurrentfinyear").html(result.CurrentFinancialYear);
                        //        $("#txtDateofbirth").val(result.DOB);
                        //        $("#txtPIGender").val(result.PIGender);
                        //        $("#txtCurrentProjectcardvalue").val(result.CurrentProjectAllotmentValue);
                        //        $('input[name="PIGender"]').val(result.PIGender);
                        //        $("#txtPIMobile").val(result.PIMobile);
                        //        $("#txtPIMothermaidenname").val(result.MothersMaiden);
                        //        $("#txtStatebankac").val(result.StateBankACNumber);
                        //        $("#txtPIAddressline1").val(result.PIAddressLine1);
                        //        $("#txtPIAddressline2").val(result.PIAddressLine2);
                        //        $("#txtPICity").val(result.PICity);
                        //        $("#txtPIState").val(result.PIstate);
                        //        $("#txtPIDistrict").val(result.PIdistrict);
                        //        $("#txtPIPincode").val(result.PIPincode);
                        //        $("#txtPIFathername").val(result.FatherFirstName);
                        //        $("#txtPIPAN").val(result.PAN);
                        //        $("#docview").html(result.DocumentDetail);
                        //        //document.getElementsByClassName('prooflink')[0].text = result.docfilename;
                        //        //document.getElementsByClassName('prooflink')[0].href = "ShowDocument?file=" + result.CardDocfilepath + "&filepath=~%2FContent%2FSupportDocuments%2F";

                        //    },
                        //    error: function (err) {
                        //        console.log("error1 : " + err);
                        //    }
                        //});
                    }
                    //else if (selVal == "Submit for approval") {
                        
                    //    var approveurl = 'SBIECardProjectApprove'; 
                    //    var choice = confirm("Are you sure, Do you want to submit this card for approval process?");
                    //    if (choice === true) {
                            
                    //        $.ajax({
                    //            type: "GET",
                    //            url: approveurl,
                    //            data: { prjctdetailsid: item.SBIEcardProjectDetailsId },
                    //            contentType: "application/json; charset=utf-8",
                    //            success: function (result) {
                                    
                    //                if (result.status == true) {
                    //                    $('#alertSuccess').html("SBI ECard Project has been approved successfully.");
                    //                    $('#Success').modal('toggle');
                    //                    loadDetails();
                    //                } else if (result.status == false) {
                    //                    $('#FailedAlert').html("Something went wrong please contact administrator");
                    //                    $('#Failed').modal('toggle');
                    //                }
                    //            },
                    //            error: function (err) {
                    //                console.log("error1 : " + err);
                    //            }
                    //        });
                    //        $.ajax({
                    //            type: "GET",
                    //            url: getSBIECardList,
                    //            data: param = "",
                    //            contentType: "application/json; charset=utf-8",
                    //            success: function (result) {
                    //                // dataProposal = result;
                    //                $("#gridSBIECardList").jsGrid({ data: result });
                    //                $('#gridlist').show();
                    //                $('#addnewpage').show();
                    //            },
                    //            error: function (err) {
                    //                console.log("error : " + err);
                    //            }
                    //        });
                    //    }
                        //}
                    else if (selVal == "Submit for Approval") {
                        var choice = confirm("Are you sure, Do you want to submit this bill for approval process?");
                        if (choice === true) {
                            var approveurl = 'SBIECardProjectWFInit';
                            $.ajax({
                                type: "POST",
                                url: approveurl,
                                data: {
                                    billId: item.SBIEcardId
                                },
                                success: function (result) {
                                    if (result.status == true) {
                                        $('#alertSuccess').html("SBI Card has been Submitted for approval process successfully.");
                                        $('#Success').modal('toggle');
                                        $("#gridSBIECardList").jsGrid({ data: result });
                                        //$('#gridlist').show();
                                    } else if (result.status == false) {
                                        $('#FailedAlert').html(result.msg);
                                        $('#Failed').modal('toggle');
                                    }
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                        }
                    }
            
                    else if (selVal == "View") {
                        var url = 'SBICardView?CardProjectId=' + item.SBIEcardProjectDetailsId;
                        //var url = 'SBICardView?ProjectId=' + item.ProjectID + '&CardID=' + item.SBIEcardId;
                        window.location.href = url;
                    }
                    else if (selVal == "Bill Booking") {
                        var url = 'SBIECardRecoupment?SBICardRecoupId=' + item.RecoupId + '&SBICardProjectDetailsId=' + item.SBIEcardProjectDetailsId;
                        window.location.href = url;
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

    $("#gridSBIECardList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    $.ajax({
        type: "GET",
        url: getSBIECardList,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
           // dataProposal = result;
            $("#gridSBIECardList").jsGrid({ data: result });            
            $('#gridlist').show();
            $('#addnewpage').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
    
    function GetSBIECardlist() {

        $.ajax({
            type: "GET",
            url: getSBIECardList,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridSBIECardList").jsGrid({ data: result });               
                $('#gridlist').show();
                $('#addnewpage').show();
               // $('#popupFilter').show();
                dbSBIECard = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }

});



var selectPickerApiElement = function (el, choice, options, select) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
    } else if (choice == "empty") {
        $(el).find('.selectpicker').children().remove();
        $(el).find('.selectpicker').append("<option value=''>Select any</option>");
    } else {
        var selectOptionsLength = $(el).find('.selectpicker').children().length;
        for (var i = 1 ; i <= selectOptionsLength ; i++) {
            if (options == $(el).find('.selectpicker').children().eq(i).val()) {
                $(el).find('.selectpicker').children().eq(i).remove();
                break;
            } else {
                continue;
            }

        }

    }
    $(el).find('select').selectpicker('refresh');
    return $(el).children().first().unwrap();

}