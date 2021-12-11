var getCommitmentURL = 'GetCommitmentDetails'
var getPrjDetails = "LoadProjectDetails";
var db;
var DateField = function (config) {
    jsGrid.Field.call(this, config);
};

DateField.prototype = new jsGrid.Field({
    sorter: function (date1, date2) {
        return new Date(date1) - new Date(date2);
    },

    itemTemplate: function (value) {
        return new Date(value).toDateString();
    },

    filterTemplate: function () {
        var now = new Date();
        this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), changeYear: true });
        this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), changeYear: true });
        return $("<div>").append(this._fromPicker).append(this._toPicker);
    },

    insertTemplate: function (value) {
        return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date() });
    },

    editTemplate: function (value) {
        return this._editPicker = $("<input>").datepicker().datepicker("setDate", new Date(value));
    },

    insertValue: function () {
        return this._insertPicker.datepicker("getDate").toISOString();
    },

    editValue: function () {
        return this._editPicker.datepicker("getDate").toISOString();
    },

    filterValue: function () {
        return {
            from: this._fromPicker.datepicker("getDate"),
            to: this._toPicker.datepicker("getDate")
        };
    }
});

jsGrid.fields.date = DateField;
$(function () {
    //var tapalDetails = 'PopupTapalDetails';
    var getEditCommitment = 'getEditCommitmentDetails';
    $("#CommitmentList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {
            loadData: function (filter) {
                return $.grep(db, function (ow) {
                    return (!filter.CommitmentType || ow.CommitmentType.toLowerCase().indexOf(filter.CommitmentType.toLowerCase()) > -1)
                    && (!filter.CommitmentNo || ow.CommitmentNo.toLowerCase().indexOf(filter.CommitmentNo.toLowerCase()) > -1)
                    && (!filter.projectNumber || ow.projectNumber.toLowerCase().indexOf(filter.projectNumber.toLowerCase()) > -1)
                    && (!filter.CreatedDate.from || new Date(ow.CreatedDate) >= filter.CreatedDate.from)
                    && (!filter.CreatedDate.to || new Date(ow.CreatedDate) <= filter.CreatedDate.to)
                    && (!filter.Status || ow.Status.toLowerCase().indexOf(filter.Status.toLowerCase()) > -1);
                });
            }

        },
        fields: [
            { name: "SlNo", title: "S.No", editing: false, width: 40 },
            { type: "text", name: "CommitmentType", title: "Type", editing: false, width: 150 },
              { type: "text", name: "projectNumber", title: "Project No", editing: false },
            { type: "text", name: "CommitmentNo", title: "Commitment No", editing: false },
            { type: "text", name: "VendorName", title: "Vendor Name", editing: false, visible: false },
            { name: "CommitmentAmount", title: "Commitment Amount", editing: false },
            { name: "AmountSpent", title: "Amount spent", editing: false, visible: false },
            { type: "date", name: "CreatedDate", title: "Created Date", editing: false },
            { type: "text", name: "Status", title: "Status", editing: false },
            {
                type: "control", width: 100,
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false)
                },
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);

                    var $customButton = $("<button>")
                        .attr("class", "ion-eye")
                        .click(function (e) {
                            $.ajax({
                                type: "POST",
                                url: tapalDetails,
                                data: { TapalId: item.TapalId },
                                success: function (result) {
                                    $("#popup").html(result);
                                    $('#notify_modal').modal('toggle');
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                            e.stopPropagation();
                        });

                    $customButton.hide();

                    var $customButtonEdit = $("<button>")

                   .attr("class", "ion-edit")
                   .click(function (e) {
                       $.ajax({
                           type: "POST",
                           url: getEditCommitment,
                           data: { "CommitmentId": item.ComitmentId },
                           success: function (result) {
                               
                               $("#addCommitment").show();
                               $("#gridCommitment").hide();
                               $("#btnSave").val("Update");
                               $("#hdnCommitId").val(result.commitmentId);
                               $("#CommitmentType").val(result.selCommitmentType);
                               $("#divAmountSpent").removeClass('menu-hide');
                               //Staff Commitment
                               if (result.selCommitmentType == 1) {
                                   $("#staffCommit").removeClass('menu-hide'); $("#geneCommit").removeClass('menu-hide');
                                   $("#purchaseCommit").addClass('menu-hide'); $("#NegativeCommit").addClass('menu-hide');
                                   $("#ForeginCommit").addClass('menu-hide'); $("#isDean").addClass('menu-hide'); $("#divLocalCurr").addClass('menu-hide');
                                   $('#AllocationValue').removeAttr('readonly');
                               }
                                   //General Commitment
                               else if (result.selCommitmentType == 2) {
                                   $("#staffCommit").addClass('menu-hide'); $("#geneCommit").removeClass('menu-hide');
                                   $("#purchaseCommit").addClass('menu-hide'); $("#NegativeCommit").addClass('menu-hide');
                                   $("#ForeginCommit").addClass('menu-hide'); $("#isDean").addClass('menu-hide'); $("#divLocalCurr").addClass('menu-hide');
                                   $('#AllocationValue').removeAttr('readonly');
                               }
                                   //Purchase Commitment
                               else if (result.selCommitmentType == 3) {
                                   $("#staffCommit").addClass('menu-hide'); $("#geneCommit").addClass('menu-hide');
                                   $("#purchaseCommit").removeClass('menu-hide'); $("#NegativeCommit").removeClass('menu-hide');
                                   $("#ForeginCommit").addClass('menu-hide'); $("#isDean").addClass('menu-hide'); $("#divLocalCurr").addClass('menu-hide');
                                   $('#AllocationValue').removeAttr('readonly');
                               }
                                   //Negative Balance Commitment
                               else if (result.selCommitmentType == 4) {
                                   $("#staffCommit").addClass('menu-hide'); $("#geneCommit").removeClass('menu-hide');
                                   $("#purchaseCommit").addClass('menu-hide'); $("#NegativeCommit").removeClass('menu-hide');
                                   $("#ForeginCommit").addClass('menu-hide'); $("#isDean").removeClass('menu-hide'); $("#divLocalCurr").addClass('menu-hide');
                                   $('#AllocationValue').removeAttr('readonly');
                               }
                                   //Commitment in Foreign Currency with exchange
                               else if (result.selCommitmentType == 5) {
                                   $("#staffCommit").addClass('menu-hide'); $("#geneCommit").addClass('menu-hide');
                                   $("#purchaseCommit").addClass('menu-hide'); $("#NegativeCommit").addClass('menu-hide');
                                   $("#ForeginCommit").removeClass('menu-hide'); $("#isDean").addClass('menu-hide'); $("#divLocalCurr").removeClass('menu-hide');
                                   $('#AllocationValue').attr('readonly');
                               } else {
                                   $("#staffCommit").addClass('menu-hide'); $("#geneCommit").addClass('menu-hide');
                                   $("#purchaseCommit").addClass('menu-hide'); $("#NegativeCommit").addClass('menu-hide');
                                   $("#ForeginCommit").addClass('menu-hide'); $("#isDean").addClass('menu-hide'); $("#divLocalCurr").addClass('menu-hide');
                                   $('#AllocationValue').removeAttr('readonly');
                               }

                               $("#selPurpose").val(result.selPurpose);
                               $("#Remarks").val(result.Remarks);
                               $("#PONumber").val(result.PONumber);
                               $("#selVendor").val(result.selVendor);
                               $("#Currency").val(result.selCurrency);
                               $("#currencyRate").val(result.currencyRate);
                               $("#AdditionalCharge").val(result.AdditionalCharge);
                               $("#ForeignCurrencyValue").val(result.ForeignCurrencyValue);
                               if (result.StartDate != null) {
                                   var sd = new Date(parseInt(result.StartDate.replace(/(^.*\()|([+-].*$)/g, '')));
                                   $("#StartDate").datepicker('setDate', sd);
                               }
                               if (result.CloseDate != null) {
                                   var cd = new Date(parseInt(result.CloseDate.replace(/(^.*\()|([+-].*$)/g, '')));
                                   $("#CloseDate").datepicker('setDate', sd);
                               }
                               $("#BasicPay").val(result.BasicPay);
                               $("#MedicalAllowance").val(result.MedicalAllowance);
                               //$("#EmployeeId").val(result.EmployeeId);
                               $('select[name=EmployeeId]').val(result.EmployeeId);
                               $('select[name=EmployeeId]').selectpicker('refresh')
                               $("#Total").val(result.Total);
                               $("#ProjectType").val(result.selProjectType);
                               $('select[name=SelProjectNumber]').val(result.SelProjectNumber);
                               $('select[name=SelProjectNumber]').selectpicker('refresh')
                               var Docname = result.DocName;
                               var Attachname = result.AttachName;
                               var Docpath = result.AttachPath;
                               $('#AttachName').val(Attachname);
                               if (Docpath != "") {
                                   $("#doclink").removeClass('menu-hide');
                                   document.getElementsByClassName('linkdoc')[0].text = Docname;
                                   document.getElementsByClassName('linkdoc')[0].href = "ShowDocument?file=" + Docpath + "&filepath=~%2FContent%2FCommitmentDocument%2F";
                               }

                               if (result.IsDeansApproval == true) {
                                   $("#IsDeansApproval").attr('checked', 'checked')
                               }
                               //$("#SelProjectNumber").val(result.SelProjectNumber);                               
                               if (result.SelProjectNumber != null) {
                                   $("#prjDetails").show();
                                   $("#spanTittle").text(result.prjDetails.ProjectTittle);
                                   $("#spanPIName").text(result.prjDetails.PIname);
                                   $("#spanSanValue").text(result.prjDetails.SanctionedValue);
                                   $("#spanTotReceipt").text(result.prjDetails.TotalReceipt);
                                   $("#spanAmtSpt").text(result.prjDetails.AmountSpent);
                                   $("#spanAvailableBal").text(result.prjDetails.AvailableBalance);
                                   $("#spanPrevious").text(result.prjDetails.PreviousCommitment);
                                   $("#spanNetBal").text(result.prjDetails.NetBalance);
                                   $("#spanNegativeBal").text(result.prjDetails.ApprovedNegativeBalance);
                                   $("#spanType").text(result.prjDetails.ProjectType);
                                   var fundingBody = result.selFundingBody;
                                   $.getJSON("GetFundingBody", { ProjectID: result.SelProjectNumber },
                         function (result) {
                             var select = $("#selFundingBody");
                             select.empty();
                             $.each(result, function (index, itemData) {
                                 select.append($('<option/>', {
                                     value: itemData.id,
                                     text: itemData.name,
                                 }));
                             });
                             $("#selFundingBody").val(fundingBody);
                         });
                                   

                                   $.getJSON("GetAllocationBasedProject", { ProjectID: result.SelProjectNumber },
                       function (Data) {
                           var select = $("#selAllocationHead");
                           select.empty();
                           $.each(Data, function (index, itemData) {
                               select.append($('<option/>', {
                                   value: itemData.id,
                                   text: itemData.name,
                               }));
                           });
                           $("#selAllocationHead").val(result.selAllocationHead);
                       });

                               }
                               
                               $("#ReqRef").val(result.selRequestRefrence);
                               $("#SourceTapalOrWorkflow").val('False');
                               if (result.selRequestRefrence == 2) {
                                   $("#divRefEmailDate").removeClass('menu-hide');
                                   $("#divRefNo").addClass('menu-hide');
                               }
                               else if (result.selRequestRefrence == 1) {
                                   $.getJSON("GetWorkflowRefNumber",
                                        function (locationdata) {
                                            var select = $("#RefNo");
                                            select.empty();
                                            $.each(locationdata, function (index, itemData) {
                                                select.append($('<option/>', {
                                                    value: itemData.name,
                                                    text: itemData.name,
                                                }));
                                            });
                                            $("#RefNo").val(result.selRefNo);
                                        });
                                   $("#divRefEmailDate").addClass('menu-hide');
                                   $("#divRefNo").removeClass('menu-hide');
                                   $("#SourceTapalOrWorkflow").val('True');
                               }
                               else if (result.selRequestRefrence == 3) {
                                   $.getJSON("GetTapalRefNumber",
                                       function (locationdata) {
                                           var select = $("#RefNo");
                                           select.empty();
                                           $.each(locationdata, function (index, itemData) {
                                               select.append($('<option/>', {
                                                   value: itemData.name,
                                                   text: itemData.name,
                                               }));
                                           });
                                           $("#RefNo").val(result.selRefNo);
                                       });

                                   $("#divRefEmailDate").addClass('menu-hide');
                                   $("#divRefNo").removeClass('menu-hide');
                                   $("#SourceTapalOrWorkflow").val('True');
                               }
                               $("#commitNo").text(result.CommitmentNo);
                               if (result.EmailDate != null) {
                                   var EmailDate = new Date(parseInt(result.EmailDate.replace(/(^.*\()|([+-].*$)/g, '')));
                                   $("#EmailDate").datepicker('setDate', EmailDate);
                               }
                               $("#commitmentValue").val(result.commitmentValue);
                               $("#btnSubmit").removeClass('menu-hide');
                               $("#AllocationValue").val(result.AllocationValue);
                              
                               
                               var CurrencyRate = result.currencyRate;
                               
                               $.getJSON("GetAllocationValue", { ProjectID: result.SelProjectNumber, AllocationID: result.selAllocationHead[0] },
                                    function (allocData) {
                                        $("#divHeadDetails").removeClass('menu-hide');
                                        $("#hdnIsYearWise").val(allocData.IsYearWise);
                                        if (allocData.IsYearWise) {
                                            $("#divSpanTA").addClass('menu-hide');
                                            $("#divSpanAFCY").removeClass('menu-hide');
                                            $("#spanAFCY").text(allocData.AllocationForCurrentYear);
                                        } else {
                                            $("#divSpanAFCY").addClass('menu-hide');
                                            $("#divSpanTA").removeClass('menu-hide');
                                            $("#spanTA").text(allocData.TotalAllocation);
                                        }
                                        $("#spanTC").text(allocData.TotalCommitmentTilDate);
                                        $("#spanTCCY").text(allocData.TotalCommitForCurrentYear);
                                    })
                           },
                           error: function (err) {
                               console.log("error1 : " + err);
                           }
                       });
                       e.stopPropagation();
                   });
                    if (item.Status != "Open") {
                        //.attr("disabled", "disabled")
                        //$customButtonEdit.("data-toggle"="tooltip" title="Hooray!"
                        $customButtonEdit.attr({
                            "data-toggle": "tooltip",
                            "title": "This commitment cannot be modified!!!",
                            "disabled": "disabled"
                        });
                    }
                    // return $result.add($customButton);
                    return $("<div>").append($customButton).append($customButtonEdit);
                }
            }
        ],
    });
    $("#CommitmentList").jsGrid("option", "filtering", false);
    loadDetails();
});
var loadDetails = function loadDetails() {
    $.ajax({
        type: "GET",
        url: getCommitmentURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#CommitmentList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
};
