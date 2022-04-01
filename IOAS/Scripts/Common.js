function deductionEligibilityCheck(value) {
    var eligibilityCheck_f = false;
    if (value > 300000)
        eligibilityCheck_f = true;
    return eligibilityCheck_f;
}
function GetSubCodeByTDSSection(tdsSel) {
    var subcode = '1';
    if (tdsSel == "1")
        subcode = '2';
    else if (tdsSel == "17")
        subcode = '10';
    else if (tdsSel == "2")
        subcode = '3';
    else if (tdsSel == "5")
        subcode = '4';
    else if (tdsSel == "6")
        subcode = '5';
    else if (tdsSel == "15")
        subcode = '6';
        else if (tdsSel == "12")
        subcode = '7';
        else if (tdsSel == "18")
            subcode = '8';
        else if (tdsSel == "19")
            subcode = '9';
        else if (tdsSel == "27")
            subcode = '11';
        else if (tdsSel == "21")
            subcode = '12';
        else if (tdsSel == "29")
            subcode = '13';
    return subcode
}
function IsInterState(stateCode) {
    var isInterState = true;
    if (stateCode == '33')
        isInterState = false;
    return isInterState;
}
function GetIndirectPaymentSubCodeByTDSSection(tdsSel, icsrOH, rcm) {
    var subcode = '1';
    if (tdsSel != "")
        subcode = '2';
    if (tdsSel == "" && icsrOH && rcm)
        subcode = '5';
    else if (tdsSel != "" && icsrOH && rcm)
        subcode = '6';
    else if (tdsSel == "" && rcm)
        subcode = '7';
    else if (tdsSel != "" && rcm)
        subcode = '8';
    else if (tdsSel != "" && icsrOH)
        subcode = '3';
    else if (tdsSel == "" && icsrOH)
        subcode = '4';
    return subcode
}
function bootstrapSelect(el, choice, options, select, fillDafaultText, idAsName) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        if (fillDafaultText !== undefined) $(el).find('.selectpicker').append("<option value=''>Select any</option>");
        if (idAsName !== undefined) {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value='" + options[i].name + "'>" + options[i].name + "</option>");
            }
        } else {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
            }
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        if (fillDafaultText !== undefined) $(el).find('.selectpicker').append("<option value=''>Select any</option>");
        if (idAsName !== undefined) {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value='" + options[i].name + "'>" + options[i].name + "</option>");
            }
        } else {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
            }
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
function fillTransactionDetails(typeCode, tSubCode, interstate_f, eligibilityCheck_f, deductionCategoryId, tdsDetailId) {
    if (typeCode === undefined)
        return false;
    EmptyExpenseDeductionDetails();
    var ttlAdvTax = parseFloat($('#lblAdvTtlAmt').html());
    $.ajax({
        type: "GET",
        url: "../CoreAccounts/GetTransactionDetails",
        data: { "interstate_f": interstate_f, "typeCode": typeCode, "tSubCode": tSubCode, "eligibilityCheck_f": eligibilityCheck_f, "TDSDetailId": tdsDetailId, "deductionCategoryId": deductionCategoryId },
        traditional: true,
        dataType: "json",
        success: function (result) {
            $.each(result.ExpenseDetail, function (i, item) {
                if (i == 0) {
                    var trEle = $('#tbodyExpenseList tr:first');
                    $(trEle).find('input[name$=".Amount"]').val('');
                    $(trEle).find('input[name$=".TransactionType"]').val(item.TransactionType);
                    $(trEle).find('#lblTransType').text(item.TransactionType);
                    $(trEle).find("input[name='ExpenseDetail.Index']").val(i);

                    var selectGroup = $(trEle).find('select[name$=".AccountGroupId"]');
                    selectGroup.empty();
                    $.each(item.AccountGroupList, function (index, itemData) {
                        selectGroup.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });

                    var selectHead = $(trEle).find('select[name$=".AccountHeadId"]');
                    selectHead.empty();
                    $.each(item.AccountHeadList, function (index, itemData) {
                        selectHead.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });
                } else {
                    var trEleNew = $('#tbodyExpenseList tr:first').clone().find('input').val('').end();
                    $(trEleNew).find('input[name$=".Amount"]').val('');
                    $(trEleNew).find("input[name='ExpenseDetail.Index']").val(i);
                    $(trEleNew).find('input[name$=".TransactionType"]').val(item.TransactionType);
                    $(trEleNew).find('#lblTransType').text(item.TransactionType);
                    $(trEleNew).find("input,Select").each(function () {
                        $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                        $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
                    });
                    $(trEleNew).find("span[data-valmsg-for]").each(function () {
                        $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
                    });

                    var selectGroup = $(trEleNew).find('select[name$=".AccountGroupId"]');
                    selectGroup.empty();
                    $.each(item.AccountGroupList, function (index, itemData) {
                        selectGroup.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });

                    var selectHead = $(trEleNew).find('select[name$=".AccountHeadId"]');
                    selectHead.empty();
                    $.each(item.AccountHeadList, function (index, itemData) {
                        selectHead.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });
                    $('#tbodyExpenseList').append(trEleNew);
                }
            });
            $.each(result.DeductionDetail, function (i, item) {
                if (i == 0) {
                    var trEle = $('#tbodyDeductionList tr:first');
                    var amtEle = $(trEle).find('input[name$=".Amount"]');
                    if (item.TDSPercentage == null) {
                        amtEle.val('');
                    } else {
                        tds = ttlAdvTax * item.TDSPercentage / 100;
                        amtEle.val(tds);
                    }

                    amtEle.addClass('required');
                    $(trEle).find('input[name$=".AccountGroupId"]').val(item.AccountGroupId);
                    $(trEle).find('input[name$=".DeductionHeadId"]').val(item.DeductionHeadId);
                    $(trEle).find('input[name$=".DeductionHead"]').val(item.DeductionHead);
                    $(trEle).find('input[name$=".AccountGroup"]').val(item.AccountGroup);
                    $(trEle).find('td:nth-child(1)').html(item.AccountGroup);
                    $(trEle).find('td:nth-child(2)').html(item.DeductionHead);
                } else {
                    var trEleNew = $('#tbodyDeductionList tr:first').clone().find('input').val('').end();
                    //$(trEleNew).find("input[name='DeductionDetail.Index']").val(i);
                    $(trEleNew).find("input").each(function () {
                        $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                        $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
                    });
                    $(trEleNew).find("span[data-valmsg-for]").each(function () {
                        $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
                    });
                    var amtEle = $(trEleNew).find('input[name$=".Amount"]');
                    amtEle.addClass('required');
                    if (item.TDSPercentage != null) {
                        tds = ttlAdvTax * item.TDSPercentage / 100;
                        amtEle.val(tds);
                    }
                    $(trEleNew).find('input[name$=".AccountGroupId"]').val(item.AccountGroupId);
                    $(trEleNew).find('input[name$=".DeductionHeadId"]').val(item.DeductionHeadId);
                    $(trEleNew).find('input[name$=".DeductionHead"]').val(item.DeductionHead);
                    $(trEleNew).find('input[name$=".AccountGroup"]').val(item.AccountGroup);
                    $(trEleNew).find('td:nth-child(1)').html(item.AccountGroup);
                    $(trEleNew).find('td:nth-child(2)').html(item.DeductionHead);
                    $('#tbodyDeductionList').append(trEleNew);
                }
            });
            $('#NeedUpdateTransDetail').val('false');
            CalculateDeductionTotal();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
}
function fillAutoCompleteDropDown(ele, data, fillDafaultText, valueAsName) {
    if (fillDafaultText !== undefined) {
        ele.append($('<option/>', {
            value: '',
            text: 'Select any',
        }));
    }
    if (valueAsName !== undefined) {
        $.each(data, function (index, itemData) {
            ele.append($('<option/>', {
                value: itemData.label,
                text: itemData.label,
            }));
        });
    } else {
        $.each(data, function (index, itemData) {
            ele.append($('<option/>', {
                value: itemData.value,
                text: itemData.label,
            }));
        });
    }
}
function applyPaymentBUAutoComplete(ele, url, setId) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            //$(ele).closest('tr').find(".lblSelId").text(ui.item.label);
            if (setId == true) {
                $(ele).closest('tr').find("input[name$='.UserId']").val(ui.item.value);
                $(ele).closest('tr').find("input[name$='.Name']").val(ui.item.label);
            } else {
                $(ele).closest('tr').find("input[name$='.UserId']").val('0');
                $(ele).closest('tr').find("input[name$='.Name']").val(ui.item.label);
            }
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            $.getJSON(url, { term: request.term },
             function (locationdata) {
                 response(locationdata);
             });
        },
        minLength: 3
    });
}
function paymentCategoryChange(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadClearanceAgentList", true)
        //$(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        //$(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        //$(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        //var ele = $(el).closest('tr').find("input[name$='.UserId']");
        //$(ele).removeClass('required');
        //$(el).closest('tr').find("input[name$='.Name']").addClass('required');
    } else if (selCat == 4) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadTravelAgencyList", true)
       
    }
    else if (selCat == 5) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadACProjectStaff", true)
    } 
    else if (selCat == 6) {
      $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadInstituteStaffList", true)
    } 
    else if (selCat == 7) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadAdhocStaffList", true)
    }
    else if (selCat == 8) {
        $(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        $(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        $(el).closest('tr').find("input[name$='.UserId']").removeClass('required');
        $(el).closest('tr').find("input[name$='.Name']").addClass('required');
        
    } else if (selCat == 9) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/GetVendorList", true)

    } else if (selCat == '') {
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").removeClass('required');
    }
}
function applyAutoCompleteProject(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadClearanceAgentList", true)
        //$(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        //$(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        //$(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        //var ele = $(el).closest('tr').find("input[name$='.UserId']");
        //$(ele).removeClass('required');
        //$(el).closest('tr').find("input[name$='.Name']").addClass('required');
    }
}
function travelerCategoryChange(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name^='autoComplete'],input[name^='.UserId'],input[name^='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 4) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadACProjectStaff", true)
    }
    else if (selCat == 5) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadInstituteStaffList", true)
    }
    else if (selCat == 6) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadAdhocStaffList", true)
    }
    else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        $(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        var ele = $(el).closest('tr').find("input[name$='.UserId']");
        $(ele).removeClass('required');
        $(el).closest('tr').find("input[name$='.Name']").addClass('required');
    }
}
function messageBox(message, type) {
    if (type == "warning") {
        $.alert({
            icon: 'ion-alert-circled',
            title: 'Warning!',
            content: message,
            type: 'orange',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else if (type == "error") {
        $.alert({
            icon: 'ion-close-circled',
            title: 'Error!',
            content: message,
            type: 'red',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else if (type == "success") {
        $.alert({
            icon: 'ion-checkmark-circled',
            title: 'Success!',
            content: message,
            type: 'green',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else {
        $.alert({
            content: message,
        });
    }

}
function fillCommitmentSrchAndSel(result, revised) {
    EmptyCommitmentSelList();
    EmptyCommitmentSrchList();
    var commitmentId = [];
    $.each(result, function (i, item) {
        if (i == 0) {
            commitmentId.push(item.CommitmentId);
            var trEle = $('#tbodyCommitmentSrchList tr:first');
            $(trEle).find('input[name="chkCommitmentId"]').val(item.CommitmentId).prop("checked", false);
            $(trEle).find('td:nth-child(2)').html(item.CommitmentNumber);
            $(trEle).find('td:nth-child(3)').html(item.ProjectNumber);
            $(trEle).find('td:nth-child(4)').html(item.CommitmentBookedAmount);
            $(trEle).find('td:nth-child(5)').html(item.CommitmentBalanceAmount);

            var trSelEle = $('#tbodyCommitmentSelList tr:first');
            $(trSelEle).find('td:nth-child(1)').html(item.CommitmentNumber);
            $(trSelEle).find('td:nth-child(2)').html(item.ProjectNumber);
            $(trSelEle).find('td:nth-child(3)').html(item.AvailableAmount);
            $(trSelEle).find('td:nth-child(4)').html(item.HeadName);
            $(trSelEle).find("input[name='CommitmentDetail.Index']").val(i);
            if (revised) {
                $(trSelEle).find("input[name$='.PaymentAmount']").val(item.PaymentAmount);
                $(trSelEle).find('td:nth-child(6) input[name$=".ReversedAmount"]').remove();
                $(trSelEle).find('td:nth-child(6)').append('<input type="text" id="CommitmentDetail_' + i + '_ReversedAmount" name="CommitmentDetail[' + i + '].ReversedAmount" class = "form-control required" onkeypress = "return ValidateDecimalOnly(event)" onblur = "CalculateReversedAmount()" value="0">');
                $(trSelEle).find('a.removeCommitment').addClass('dis-none');
                $('#tdRecAmt').html('Reversed Amount');
            }
            else {
                $(trSelEle).find("input[name$='.PaymentAmount']").val('');
                $('#tdRecAmt').html('');
                $(trSelEle).find('td:nth-child(6) input').remove();
                $(trSelEle).find('a.removeCommitment').removeClass('dis-none');
            }
            $(trSelEle).find('input[name$=".CommitmentNumber"]').val(item.CommitmentNumber);
            $(trSelEle).find('input[name$=".ProjectNumber"]').val(item.ProjectNumber);
            $(trSelEle).find('input[name$=".AvailableAmount"]').val(item.AvailableAmount);
            $(trSelEle).find('input[name$=".HeadName"]').val(item.HeadName);
            $(trSelEle).find('input[name$=".CommitmentDetailId"]').val(item.CommitmentDetailId);
        } else {
            if (commitmentId.indexOf(item.CommitmentId) == -1) {
                commitmentId.push(item.CommitmentId);
                var trEleNew = $('#tbodyCommitmentSrchList tr:first').clone();
                $(trEleNew).find('input[name="chkCommitmentId"]').val(item.CommitmentId).prop("checked", false);
                $(trEleNew).find('td:nth-child(2)').html(item.CommitmentNumber);
                $(trEleNew).find('td:nth-child(3)').html(item.ProjectNumber);
                $(trEleNew).find('td:nth-child(4)').html(item.CommitmentBookedAmount);
                $(trEleNew).find('td:nth-child(5)').html(item.CommitmentBalanceAmount);
                $('#tbodyCommitmentSrchList').append(trEleNew);
            }
            var trSelEle = $('#tbodyCommitmentSelList tr:first').clone();
            $(trSelEle).find('td:nth-child(1)').html(item.CommitmentNumber);
            $(trSelEle).find('td:nth-child(2)').html(item.ProjectNumber);
            $(trSelEle).find('td:nth-child(3)').html(item.AvailableAmount);
            $(trSelEle).find('td:nth-child(4)').html(item.HeadName);
            if (revised) {
                $(trSelEle).find("input[name$='.PaymentAmount']").val(item.PaymentAmount);
                //$(trSelEle).find('td:nth-child(6)').append('<input type="text" id="CommitmentDetail_' + i + '_ReversedAmount" name="CommitmentDetail[' + i + '].ReversedAmount" class = "form-control required" onkeypress = "return ValidateDecimalOnly(event)" onblur = "CalculateReversedAmount()" value="0">');
                //$(trSelEle).find('a.removeCommitment').addClass('dis-none');
            }
            else
                $(trSelEle).find("input[name$='.PaymentAmount']").val('');
            $(trSelEle).find('input[name$=".CommitmentNumber"]').val(item.CommitmentNumber);
            $(trSelEle).find('input[name$=".ProjectNumber"]').val(item.ProjectNumber);
            $(trSelEle).find('input[name$=".AvailableAmount"]').val(item.AvailableAmount);
            $(trSelEle).find('input[name$=".HeadName"]').val(item.HeadName);
            $(trSelEle).find("input[name='CommitmentDetail.Index']").val(i);
            $(trSelEle).find("input").each(function () {
                $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
            });
            $(trSelEle).find("span[data-valmsg-for]").each(function () {
                $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
            });
            $(trSelEle).find('input[name$=".CommitmentDetailId"]').val(item.CommitmentDetailId);
            $('#tbodyCommitmentSelList').append(trSelEle);
        }
    });
    if (revised)
        CalculateReversedAmount();
    else
        CalculatePaymentValue();
}
function removeCommitmentAndTransValidation() {
    $('#tdRecAmt').html('');
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").removeClass('required');
        $(this).find("input[name$='.ReversedAmount']").remove();
    });
    $('#tbodyExpenseList tr').each(function () {
        $(this).find("select[name$='.AccountGroupId'],select[name$='.AccountHeadId'],input[name$='.Amount']").removeClass('required');
        $(this).find('#lblTransType').html('');
        $(this).find("select").empty();
    });
    EmptyCommitmentSelList();
    EmptyExpenseDeductionDetails();
}
function removeCommitmentValidation() {
    $('#tdRecAmt').html('');
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").removeClass('required');
        $(this).find("input[name$='.ReversedAmount']").remove();
    });
    EmptyCommitmentSelList();
}
function setCommitmentValidation() {
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").addClass('required');
    });
}
function removePaymentBUValidation() {
    $('#tbodyPaymentBU tr').each(function () {
        $(this).find("select[name$='.CategoryId'],select[name$='.ModeOfPayment'],input[name$='.PaymentAmount'],input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").removeClass('required');
    });
    $("tbodyPaymentBU input[name$='.PaymentAmount'], #PaymentBUTotal").val('0');
    EmptyPaymentBU();
}
function setPaymentBUValidation() {
    $('#tbodyPaymentBU tr').each(function () {
        $(this).find("select[name$='.CategoryId'],select[name$='.ModeOfPayment'],input[name$='.PaymentAmount']").addClass('required');
    });
}
function setCommitmentAndTransValidation() {
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").addClass('required');
    });
    $('#tbodyExpenseList tr').each(function () {
        $(this).find("select[name$='.AccountGroupId'],select[name$='.AccountHeadId'],input[name$='.Amount']").addClass('required');
    });
}
function ValidateDecimalOnly(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;

    return true;
}
function ValidateNumberOnly(e) {
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
}
function fillMasterDropDown(ele, data, fillDafaultText) {
    if (fillDafaultText !== undefined) {
        ele.append($('<option/>', {
            value: '',
            text: 'Select any',
        }));
    }
    $.each(data, function (index, itemData) {
        ele.append($('<option/>', {
            value: itemData.id,
            text: itemData.name,
        }));
    });
}
function applyProjectAutoComplete(ele, hiddenEle) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            $(hiddenEle).val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            $.getJSON("../CoreAccounts/LoadProjectList", { term: request.term },
             function (locationdata) {
                 response(locationdata);
             });
        },
        minLength: 3
    });
}
function applyAutoComplete(ele, hiddenEle, url, functionName, withParEle, serviceType, multiParam) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            $(hiddenEle).val(ui.item.value);
            if (functionName !== undefined && withParEle === undefined) {
                eval(functionName + "()");
            } else if (functionName !== undefined && withParEle !== undefined) {
                dispatch(functionName, $(ele));
            }
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            if (serviceType !== undefined) {
                $.getJSON(url, { term: request.term, type: serviceType },
                 function (locationdata) {
                     response(locationdata);
                 });
            } else if (multiParam !== undefined) {
                multiParam.term = request.term;
                $.getJSON(url, multiParam,
                 function (locationdata) {
                     response(locationdata);
                 });
            } else {
                $.getJSON(url, { term: request.term },
                 function (locationdata) {
                     response(locationdata);
                 });
            }
        },
        minLength: 3
    }).autocomplete("widget").addClass("auto-com-z-index");
}
function dispatch(fn, args) {
    fn = (typeof fn == "function") ? fn : window[fn];  // Allow fn to be a function object or the name of a global function
    return fn.apply(this, args || []);  // args is optional, use an empty array by default
}
function isEmpty(obj) {
    for (var prop in obj) {
        if (obj.hasOwnProperty(prop)) {
            return false;
        }
    }

    return JSON.stringify(obj) === JSON.stringify({});
}
//Number.prototype.toFixed = function (fractionDigits) {
//    var f = parseInt(fractionDigits) || 0;
//    if (f < -20 || f > 100) {
//        throw new RangeError("Precision of " + f + " fractional digits is out of range");
//    }
//    var x = Number(this);
//    if (isNaN(x)) {
//        return "NaN";
//    }
//    var s = "";
//    if (x <= 0) {
//        s = "-";
//        x = -x;
//    }
//    if (x >= Math.pow(10, 21)) {
//        return s + x.toString();
//    }
//    var m;
//    // 10. Let n be an integer for which the exact mathematical value of 
//    // n Ã· 10^f - x is as close to zero as possible. 
//    // If there are two such n, pick the larger n.
//    n = Math.round(x * Math.pow(10, f));

//    if (n == 0) {
//        m = "0";
//    }
//    else {
//        // let m be the string consisting of the digits of the decimal representation of n (in order, with no leading zeroes).
//        m = n.toString();
//    }
//    if (f == 0) {
//        return s + m;
//    }
//    var k = m.length;
//    if (k <= f) {
//        var z = Math.pow(10, f + 1 - k).toString().substring(1);
//        m = z + m;
//        k = f + 1;
//    }
//    if (f > 0) {
//        var a = m.substring(0, k - f);
//        var b = m.substring(k - f);
//        m = a + "." + b;
//    }
//    return s + m;
//};


function formatNumberForDisplay(numberToFormat) {
    //numberToFormat = Math.round(numberToFormat || 0);
    var formatter = new Intl.NumberFormat('en-IN', {
        //style: 'currency',
        //currency: 'USD',
        digits: 2,
    });
    //var val = formatter.format(numberToFormat);
    //if (val == '-0.00' || val == '-0')
    // val = '0.00';
    //else
    // val;
    return formatter.format(numberToFormat);
}

Number.prototype.toFixed = function (fractionDigits) {
    var f = parseInt(fractionDigits) || 0;
    if (f < -20 || f > 100) {
        throw new RangeError("Precision of " + f + " fractional digits is out of range");
    }
    var x = Number(this);
    if (isNaN(x)) {
        return "NaN";
    }
    var s = "";
    if (x <= 0) {
        s = "-";
        x = -x;
    }
    if (x >= Math.pow(10, 21)) {
        var val = s + x.toString();
        if (val == '-0.00' || val == '-0')
            val = '0.00';
        else
            val;
        return val;
    }
    var m;
    // 10. Let n be an integer for which the exact mathematical value of
    // n Ã· 10^f - x is as close to zero as possible.
    // If there are two such n, pick the larger n.
    n = Math.round(x * Math.pow(10, f));

    if (n == 0) {
        m = "0";
    }
    else {
        // let m be the string consisting of the digits of the decimal representation of n (in order, with no leading zeroes).
        m = n.toString();
    }
    if (f == 0) {
        var val = s + m;
        if (val == '-0.00' || val == '-0')
            val = '0.00';
        else
            val;
        return val;
    }
    var k = m.length;
    if (k <= f) {
        var z = Math.pow(10, f + 1 - k).toString().substring(1);
        m = z + m;
        k = f + 1;
    }
    if (f > 0) {
        var a = m.substring(0, k - f);
        var b = m.substring(k - f);
        m = a + "." + b;
    }
    var val = s + m;
    if (val == '-0.00' || val == '-0')
        val = '0.00';
    else
        val;
    return val;
};

function formatDate(date) {
    var months = ['january', 'february', 'march', 'april', 'may', 'june', 'july', 'august', 'september', 'october', 'november', 'december'];
    var day = date.getDate();
    var year = date.getFullYear();
    var month = months[date.getMonth()];
    return day + "-" + month + "-" + year;
}

function applyAutoCompleteDesignation(ele, hiddenEle, url, functionName, withParEle, serviceType, multiParam) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            $(hiddenEle).val(ui.item.value);
            if (functionName !== undefined && withParEle === undefined) {
                eval(functionName + "()");
            } else if (functionName !== undefined && withParEle !== undefined) {
                dispatch(functionName, $(ele));
            }
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            if (serviceType !== undefined) {
                $.getJSON(url, { term: request.term, type: serviceType },
                function (locationdata) {
                    response(locationdata);
                });
            } else if (multiParam !== undefined) {
                multiParam.term = request.term;
                $.getJSON(url, multiParam,
                function (locationdata) {
                    response(locationdata);
                });
            } else {
                $.getJSON(url, { term: request.term },
                function (locationdata) {
                    response(locationdata);
                });
            }
        },
        minLength: 2
    }).autocomplete("widget").addClass("auto-com-z-index");
}

//Recruitment

function monthCount(startdate, enddate) {
    var arrmonth = [];
    arrmonth = dateRange(startdate.getFullYear() + '-' + parseInt(startdate.getMonth() + 1) + '-' + startdate.getDate(), enddate.getFullYear() + '-' + parseInt(enddate.getMonth() + 1) + '-' + enddate.getDate())
    var count = 0;
    for (var i = 0; i < arrmonth.length; i++) {
        var lastCount = arrmonth.length - 1;
        if (i != 0 && i != lastCount)
            count += 1;
    }
    return count;
}

function dateRange(startDate, endDate) {
    var start = startDate.split('-');
    var end = endDate.split('-');
    var startYear = parseInt(start[0]);
    var endYear = parseInt(end[0]);
    var months = [];
    for (var i = startYear; i <= endYear; i++) {
        var endMonth = i != endYear ? 11 : parseInt(end[1]) - 1;
        var startMon = i === startYear ? parseInt(start[1]) - 1 : 0;
        for (var j = startMon; j <= endMonth; j = j > 12 ? j % 12 || 11 : j + 1) {
            var month = j + 1;
            var displayMonth = month < 10 ? '0' + month : month;
            months.push([displayMonth]);
        }
    }
    return months;
}

function endOfMonth(date) {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0);
}

function getCalDateDetails(startDate, endDate) {
    var startdt = new Date(startDate.split('-').join('/'));
    var enddt   = new Date(endDate.split('-').join('/'));
    var count   = monthCount(startdt, enddt);
    var startdatemonthend = parseInt(endOfMonth(startdt).getDate());
    var enddatemonthend   = parseInt(endOfMonth(enddt).getDate());
    var startworkingdays = 0, endworkingdays = 0;
    if (startdt.getMonth() == enddt.getMonth() && startdt.getFullYear() == enddt.getFullYear()) {
        startworkingdays = parseInt(enddt.getDate()) - parseInt(startdt.getDate()) + 1;
    }
    else {
        startworkingdays = startdatemonthend - parseInt(startdt.getDate()) + 1;
        endworkingdays = enddt.getDate();
    }
    var startyear  = startdt.getFullYear();
    var endyear    = enddt.getFullYear();
    var startmonth = startdt.getMonth() + 1;
    var endmonth   = enddt.getMonth() + 1;
    return { starttotaldays: startdatemonthend, endtotaldays: enddatemonthend, startworkingdays: startworkingdays, endworkingdays: endworkingdays, monthcount: count, startdateyear: startyear, enddateyear: endyear, startdatemonth: startmonth, enddatemonth: endmonth };
}
