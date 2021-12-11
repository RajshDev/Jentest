$(function () {
    //Declare Proposal List
    var getInvoiceProcessList = 'LoadInvoiceProcessList',
     EditInvoice = 'EditInvoice',
     DeleteInvoice = 'DeleteInvoice';
    //var dbInvoice;
    //GetInvoicelist();

    // jsGrid.fields.date = DateField;

    $("#gridInvoiceList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbInvoice, function (invoice) {

                    return (!filter.InvoiceNumber || invoice.InvoiceNumber.toLowerCase().indexOf(filter.InvoiceNumber.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || invoice.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.NameofPI || invoice.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                    && (!filter.InvoiceValue || invoice.InvoiceValue.toLowerCase().indexOf(filter.InvoiceValue.toLowerCase()) > -1)
                    && (!filter.InvoiceStatus || invoice.InvoiceStatus.toLowerCase().indexOf(filter.InvoiceStatus.toLowerCase()) > -1)
                    && (!filter.InvoiceDate.from || new Date(invoice.InvoiceDate) >= filter.InvoiceDate.from)
                  && (!filter.InvoiceDate.to || new Date(invoice.InvoiceDate) <= filter.InvoiceDate.to);
                });
            }
        },
        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            { type: "number", name: "ProjectId", title: "Project Id", visible: false },
            { type: "number", name: "InvoiceId", title: "Invoice Id", visible: false },
            { type: "text", name: "Invoicedatestrng", editing: false, title: "Invoice Date", align: "center", width: "60px" },
            { type: "text", name: "InvoiceNumber", editing: false, title: "Invoice Number", width: "60px" },
            { type: "text", name: "InvoiceTypeName", editing: false, title: "Invoice Type", width: "60px" },
            { type: "text", name: "CurrencyCode", editing: false, title: "Currency", width: "60px" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "80px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "decimal", name: "TotalInvoiceValue", title: "Invoice Value", editing: false, width: "55px" },
            { type: "text", name: "InvoiceStatus", title: "Status", editing: false, width: "55px" },
          //  { type: "select", name: "Action", title: "Action", editing: false, width: "55px" },
          //{ name: "Type", type: "select", items: ["Select","Approve"], validate: "required" },
        {

            name: "InvoiceId",
            title: "Action",
            width: "60px",
            itemTemplate: function (value, item) {
                return $("<a>").attr("href", "javascript:void(0)").attr("class", "btn btn-primary").attr("value", "Select").text('Approve').on("click", function () {
                    var invoiceid = item.InvoiceId;
                    var InvoiceDetails = 'LoadInvoiceProcess';
                    $.ajax({
                        type: "POST",
                        url: InvoiceDetails,
                        data: { InvoiceId: invoiceid },
                        //contentType: "application/json; charset=utf-8",
                        //dataType: "json",

                        success: function (result) {
                            $('#IRNApplicationDate,#txtIRNApplicationDate').removeClass('hasDatepicker');
                            var Item = result.InvoiceDateStr.split('-');
                            var Date1 = new Date(parseInt(Item[2]), parseInt(Item[1]) - 1, parseInt(Item[0]));
                            $('#IRNApplicationDate,#txtIRNApplicationDate').datepicker({ dateFormat: 'dd-MM-yy', minDate: Date1 });
                            $('#ProjectInvoice').show();
                            $('#gridlist').hide();
                            $("#Invdate").html(result.Invoicedatestrng);
                            $('#divInd,#divInd').addClass('dis-none');
                            if (result.InvoiceType != 1)
                                $('#divInd').removeClass('dis-none');
                            else
                                $('#divFore').removeClass('dis-none');
                            $('#AgencyCountryId').val(result.AgencyCountryId);
                            $('input[name="CurrentAvailPjctBalance"]').val(result.CurrentAvailPjctBalance);
                            $('input[name="Invoicedatestrng"]').html(result.Invoicedatestrng);
                            $('input[name="Invoicedatestrng"]').val(result.Invoicedatestrng);
                            $('input[name="InvoiceDate"]').val(result.InvoiceDate);
                            $('input[name="InvoiceId"]').val(result.InvoiceId);
                            $('input[name="InvoiceNumber"]').val(result.InvoiceNumber);
                            $('input[name="ProjectID"]').val(result.ProjectID);
                            $('input[name="ProjectType"]').val(result.ProjectType);
                            $('select[name="ServiceType"]').val(result.ServiceType);
                            $('input[name="TaxStatus"]').val(result.TaxStatus);
                            $('#InvoiceType').val(result.InvoiceType);
                            $('input[name="AvailableBalance"]').val(result.AvailableBalance);
                            $('input[name="Sanctionvalue"]').val(result.Sanctionvalue);
                            $('select[name="AgencystateId"]').val(result.AgencystateId);
                            $('#txtCommunicationAddress').val(result.CommunicationAddress);
                            $('#txtSponsoringAgencyName').val(result.SponsoringAgencyName);
                            $('#txtAgencyStateCode').val(result.Agncystatecode);
                            $('#txtGSTIN').val(result.GSTNumber);
                            $('#txtAgencyPAN').val(result.PAN);
                            $('#txttotalreceiptval').html(result.TotalReceiptValue);
                            $('#txttotalopenreceiptval').html(result.TotalOpenBalReceiptValue);
                            $('#txttotalopeninvval').html(result.TotalOpenInvoiceValue);
                            if (result.InvoiceType == 1) {
                                $('#divForeCurrVal,#divForeCurr').removeClass('dis-none');
                            } else {
                                $('#divForeCurrVal,#divForeCurr').addClass('dis-none');
                            }

                            $('input[name="PONumber"]').val(result.PONumber);
                            $('#txtprojectnumber').html(result.ProjectNumber);
                            if (result.ProjectType == 2) {
                                $('#divinstalment').removeClass('dis-none');
                                $('#divinvtype').removeClass('dis-none');
                                $('#divNonExpTAN').removeClass('dis-none');
                                $('#divGSTIN').removeClass('dis-none');
                                $('#divPAN').removeClass('dis-none');
                                //if (result.InvoiceType != 1)
                                //{
                                //    $('input[name="TAN"]').attr('required',true);
                                //    $('input[name="PAN"]').attr('required', true);
                                //    if (result.InvoiceType != 4)
                                //    {
                                //        $('input[name="GSTNumber"]').attr('required', true);
                                //    }                                    
                                //}
                                
                            } else {
                                $('#divinstalment').addClass('dis-none');
                                $('#divinvtype').addClass('dis-none');
                                $('#divNonExpTAN').addClass('dis-none');
                                $('#divGSTIN').addClass('dis-none');
                                $('#divPAN').addClass('dis-none');  
                                //$('input[name="TAN"]').attr('required',false);
                                //$('input[name="PAN"]').attr('required', false);
                                //$('input[name="GSTNumber"]').attr('required', false);
                                
                            }
                            if (result.ProjectClassification == 4) {
                                $('#Accgroup').removeClass('dis-none');
                                $('#AccId').removeClass('dis-none');
                            } else {
                                $('#AccId').addClass('dis-none');
                                $('#Accgroup').addClass('dis-none');
                            }
                            $('#txtPIname').html(result.NameofPI);
                            $('#txtPIdepartment').html(result.PIDepartmentName);
                            $('#txtsanctionordernumber').html(result.SanctionOrderNumber);
                            $('#txtsanctionvalue').html(result.Sanctionvalue);
                            $('#txtcurrentfinyear').html(result.CurrentFinancialYear);
                            //$('#txtinvoicetype').val(result.InvoiceTypeName);
                            //  $('#txtservicetype').val(result.ServiceTypeName);
                            $('#txtSACNumber').val(result.SACNumber);

                            $('#txtdescriptionofservice').val(result.DescriptionofServices);
                            $('#taxablevalue').val(result.TaxableValue);
                            $('#totalinvoicevalue').val(result.TotalInvoiceValue);
                            $('input[name="CurrentInvoiceValue"]').val(result.TaxableValue);
                            $('#txtForeignCurrencyValue').val(result.ForeignCurrencyValue);
                            $('#AllocatedForeignCurrencyValue').val(result.AllocatedForeignCurrencyValue);
                            $('#PrevinvForeignCurrencyValue').val(result.PrevinvForeignCurrencyValue);
                            $('#txtCurrencyCode').text(result.CurrencyCode);
                            $('#lblInvNo').text(result.InvoiceNumber);
                            //$('#CurrencyCode').val(result.CurrencyCode);
                            $('input[name="CurrencyCode"]').val(result.CurrencyCode);
                            $('#SelCurr').val(result.SelCurr);
                            $('input[name="TotalInvoiceValue"]').val(result.TotalInvoiceValue);
                            $('#totalinvoicevalue').html(result.TotalInvoiceValue);
                            $('#instalmentnumber').val(result.Instalmentnumber);
                            $('input[name="Instlmntyr"]').val(result.Instlmntyr);
                            $('#SGSTamount').val(result.SGST);
                            $('#SGSTpercent').val(result.SGSTPercentage);
                            $('input[name="SGSTPercentage"]').val(result.SGSTPercentage);
                            $('#CGSTamount').val(result.CGST);
                            $('#CGSTpercent').val(result.CGSTPercentage);
                            $('input[name="CGSTPercentage"]').val(result.CGSTPercentage);
                            $('#IGSTamount').val(result.IGST);
                            $('#IGSTpercent').val(result.IGSTPercentage);
                            $('input[name="IGSTPercentage"]').val(result.IGSTPercentage);
                            $('input[name="TotalTaxValue"]').val(result.TotalTaxValue);
                            $('#Totaltaxpercent').val(result.TotalTaxpercentage);

                            $('input[name="SponsoringAgency"]').val(result.SponsoringAgency);
                            $('select[name="AccountGroupId"]').val(result.AccountGroupId);
                            $('select[name="AccountHeadId"]').val(result.AccountHeadId);
                            //$('#txtAgencyRegname').html(result.SponsoringAgencyName);
                            //$('#txtAgencyAddress').html(result.Agencyregaddress);
                            $('#Agencydistrict').val(result.Agencydistrict);
                            $('#AgencyPincode').val(result.AgencyPincode);

                            //$('#txtState').html(result.Agencystate);
                            //$('#txtStatecode').html(result.Agencystatecode);
                            //$('#txtGSTIN').html(result.GSTNumber);
                            //$('#txtPAN').html(result.PAN);

                            $('#TAN').val(result.TAN);
                            $('#Agencycontactperson').val(result.Agencycontactperson);
                            $('#AgencycontactpersonEmail').val(result.AgencycontactpersonEmail);
                            $('#Agencycontactpersonmobile').val(result.Agencycontactpersonmobile);

                            $('#ForeignAgencyPlace').val(result.ForeignAgencyPlace);
                            $('#IndianSEZTaxCategory').val(result.IndianSEZTaxCategory);
                            changeInvoiceType();
                            loadServiceDetails();
                        },

                        error: function (err) {
                            console.log("error1 : " + err);
                        }

                    });
                    $("#AddNewEntryModel").modal('hide');

                });
            }
        }

        ],


    });

    $("#gridInvoiceList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    $.ajax({
        type: "GET",
        url: getInvoiceProcessList,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            // dataProposal = result;
            $("#gridInvoiceList").jsGrid({ data: result });
            $('#ProjectInvoice').hide();
            $('#gridlist').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });

    function GetInvoicelist() {

        $.ajax({
            type: "GET",
            url: getInvoiceDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridInvoiceList").jsGrid({ data: result });
                $('#ProjectInvoice').hide();
                $('#gridlist').show();
                $('#addnewpage').show();
                // $('#popupFilter').show();
                dbInvoice = result;
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