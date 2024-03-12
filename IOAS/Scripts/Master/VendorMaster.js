$(function () {
    var GetVendorlist = 'GetVendorMaster',
        EditVendorlist = 'EditVendorlist',
        Deleteagencylist = 'DeleteInternalAgency';
    var db;
    $('#vendorList').jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 10,
        editing: false,
        filtering: true,
        pageLoading: true,
        autoload: true,
        controller: {

            loadData: function (filter) {
                //return $.grep(db, function (vendor) {

                //    return (!filter.Name || vendor.Name.toLowerCase().indexOf(filter.Name.toLowerCase()) > -1)
                //    && (!filter.VendorCode || vendor.VendorCode.toLowerCase().indexOf(filter.VendorCode.toLowerCase()) > -1)
                //    && (!filter.CountryName || vendor.CountryName.toLowerCase().indexOf(filter.CountryName.toLowerCase()) > -1);


                //});
                var searchData = [];
                searchData = {
                    INVendorSearchname: filter.Name||null,
                    INVendorsearchCode: filter.VendorCode||null,
                    INStatus: filter.Status||null,
                    INBankName: filter.BankName||null,
                    INAccountNumber:filter.AccountNumber||null,
                    EXCountryName: $('#ddlSeaCountry').val(),
                    EXVendorSearchname: $('#txtsrchname').val(),
                    EXINVendorsearchCode: $('#ddlSearchVendorCode').val()

                },
                 filter.model = searchData;
                var deferred = $.Deferred();
                $.ajax({
                    type: "post",
                    url: GetVendorlist,
                    data: JSON.stringify(filter),
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {

                        var da = {
                            data: result.VendorList,
                            itemsCount: result.TotalRecords
                        }
                        deferred.resolve(da);
                    },
                    error: function (err) {
                        console.log("error : " + err);
                    }

                });
                return deferred.promise();

            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, align: "left", width: "70px", filtering: false },
                    { type: "number", name: "VendorId", title: "Vendor Id", editing: false, visible: false },
                    { type: "text", name: "Name", title: "Name", editing: false },
                    { type: "text", name: "VendorCode", title: "Vendor Code" },
                    { type: "text", name: "CountryName", title: "Country Name", editing: false, filtering: false },
                    { type: "text", name: "BankName", title: "Bank Name" },
                    { type: "text", name: "AccountNumber", title: "Account Number" },
                  {  type:"text", name: "Status", title: "Status", align: "left", width: 60 },
             {
                 type: "control", editButton: false, deleteButton: false, title: "Action", width: 80,
                 itemTemplate: function (value, item) {
                     var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                     if (item.Status == "Open") {
                         statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "Submit for approval", name: "Submit for approval" }, { id: "View", name: "View" }]
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
                             editVendor(item.VendorId)
                         }
                         else if (selVal == "Submit for approval") {
                             var choice = confirm("Are you sure, Do you want to submit this vendor for approval process?");
                             if (choice === true) {
                                 $.ajax({
                                     type: "POST",
                                     url: 'VendorWFInit',
                                     data: {
                                         vendorId: item.VendorId
                                     },
                                     success: function (result) {
                                         if (result.status == true) {
                                             $('#alertSuccess').html("Vendor details has been submitted for approval process successfully.");
                                             $('#Success').modal('toggle');
                                             GetVendorAllList();
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
                             var url = 'VendorManagementView?Vendorid=' + item.VendorId;
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
    $("#vendorList").jsGrid("option", "filtering", false);
    //$.ajax({

    //    type: "GET",
    //    url: GetVendorlist,
    //    data: "",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (result) {

    //        $("#vendorList").jsGrid({
    //            data: result
    //        });
    //        db = result;
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});
    function editVendor(vendorid) {
        $('#vendorMaster,#vendorhead1,#vendorhead2,#vendorhead3,#vendorhead4,#vendorhead5,#vendorhead6,#btnUpdate,#Divbutton').show();

        $('#btnSave,#gridlist,#addnewpage').hide();
        $.ajax({
            type: "POST",
            url: EditVendorlist,
            data: { vendorId: vendorid },
            success: function (result) {

                $('#ddlVondorctry').val(result.Nationality);
                //if (result.Nationality == 2)
                //{
                //    $("#vendorhead3,#vendorhead5,#vendorhead6").hide();
                //}
                if (result.ClearanceAgency_f == true) {
                    $('#ClearanceAgency_f').attr('checked', true);
                }
                if (result.TravelAgency_f == true) {
                    $('#TravelAgency_f').attr('checked', true);
                }
                if (result.isGSTVendor == true) {
                    $('#isGSTVendor').attr('checked', true);
                }

                $('#txtVendorCode').val(result.PFMSVendorCode);
                $('#txtVendorId').val(result.VendorId);
                $('#txtVondorName').val(result.Name);
                $('#hiddVendorName').val(result.Name);
                $('#hiddAddress').val(result.Address);
                $('#txtAddress').val(result.Address);
                $('#txtEmail').val(result.Email);
                $('#hiddEmailAddress').val(result.Email);
                $('#txtContactPerson').val(result.ContactPerson);
                $('#hiddContactPerson').val(result.ContactPerson);
                $('#txtPhoneNumber').val(result.PhoneNumber);
                $('#txtMobileNum').val(result.MobileNumber);
                $('#txtCity').val(result.City);
                $('#txtPinCode').val(result.PinCode);
                $('#ddlcountry').val(result.CountryId);
                $('#ddlstate').val(result.StateId);
                $('#txtStateCode').val(result.StateCode);
                $('#txtRegisteredName').val(result.RegisteredName);
                $('#hiddRegName').val(result.RegisteredName);
                $('#txtPANNumber').val(result.PAN);
                $('#txtTANNumber').val(result.TAN);
                $('#IsYes').val();
                $('#IsNo').val();
                $('#txtReason').val(result.Reason);
                $('#txtGSTIN').val(result.GSTIN);
                $('#txtAcctHolderName').val(result.AccountHolderName);
                $('#txtBankName').val(result.BankName);
                $('#txtBranch').val(result.Branch);
                $('#txtIfscCode').val(result.IFSCCode);
                $('#ddlBankcountry').val(result.BankCountry);
                $('#txtAcctNum').val(result.AccountNumber);
                $('#txtBankAddress').val(result.BankAddress);
                $('#txtABANumber').val(result.ABANumber);
                $('#txtSortCode').val(result.SortCode);
                $('#txtIBAN').val(result.IBAN);
                $('#txtBankNature').val(result.BankNature);
                $('#txtBankEmail').val(result.BankEmailId);
                $('#txtMICRCode').val(result.MICRCode);
                $('#txtSwiftBICCode').val(result.SWIFTorBICCode);
                $('#txtReverseTaxReason').val(result.ReverseTaxReason);
                $('#ddlcategoryservice').val(result.ServiceCategory);
                $('#hiddServiceType').val(result.ServiceType);
                if ($('#ddlcategoryservice').val() == 1) {
                    $('#divservice').show();
                    $('#ddlservicetype').val(result.ServiceType);

                }
                else {
                    $('#divsupplier').show();
                    $('#ddlSupplierType').val(result.SupplierType);
                }
                $('#txtcetrificataeNum').val(result.CertificateNumber);
                $('#txtvaildateprd').val(result.ValidityPeriod);
              
                if (result.GSTExempted == true) {
                   
                    $('#IsYes').attr('checked', true);
                    
                    $('#divGST').hide();
                    $('#divReson').show();
                }
                else {
                    $('#IsNo').attr('checked', true);
                    
                    $('#divGST').show();
                    $('#divReson').hide();
                }
                if (result.ReverseTax == true) {
                    $('#IsRevYes').attr('checked', true);
                    $('#divtaxresason').show();
                }
                else {
                    $('#IsRevNo').attr('checked', true);
                    $('#divtaxresason').hide();
                }
                if (result.TDSExcempted == true) {
                    $('#IsTdsYes').attr('checked', true);
                    $('#divcetnum').show();
                    $('#divVal').show();
                }
                else {
                    $('#IsTdsNo').attr('checked', true);
                    $('#divcetnum').hide();
                    $('#divVal').hide();
                }
                if (result.Nationality == 1) {
                    $("#lblemail").addClass("required");
                    $("#txtEmail").prop('required', true);
                    $('#stateDiv').show();
                    $('#countryDiv').hide();
                    $('#abaDiv').hide();
                    $('#swiftCode').hide();
                    $('#stateCodeDiv').show();
                    $('#txtcetrificataeNum').prop('required', true);
                    $("#divPAN").show();
                    $("#DivTan").show();
                    $("#txtIfscCode").prop('required', true);
                    $("#lblifsc").addClass("required");
                    $("#ifsccode").show();
                    $("#hiddenBankcountry").hide();
                    //$("#divReson").show();
                    //$("#lblservice").addClass("required");
                    // $("#ddlcategoryservice").prop('required', true);
                    $("#vendorhead3,#vendorhead5,vendorhead6").show();
                }
                else {
                    $("#lblemail").removeClass("required");
                    $("#txtEmail").prop('required', false);
                    $('#stateDiv').hide();
                    $('#countryDiv').show();
                    $('#abaDiv').show();
                    $('#swiftCode').show();
                    $('#stateCodeDiv').hide();
                    $("#divPAN").hide();
                    $("#DivTan").hide();
                    $("#divReson").hide();
                    $("#ifsccode").hide();
                    $("#hiddenBankcountry").show();
                    $("#lblservice").removeClass("required");
                    $("#ddlcategoryservice").prop('required', false);
                    $("#ddlcountry option[value='128']").remove();
                    $("#vendorhead3,#vendorhead5,#vendorhead6").hide();

                }
                //$('#ddlVondorctry,#txtVondorName,#txtAddress,#txtEmail,#txtContactPerson,#txtPhoneNumber,#txtMobileNum,#ddlcountry,#ddlstate').prop('disabled', true);
                //$('#txtStateCode,#txtRegisteredName,#txtPANNumber,#txtTANNumber,#txtReason,#txtGSTIN,#txtVendorCode').prop('disabled', true);
                var Docgstname = result.GSTDocumentName;
                var gstAttachname = result.GSTAttachName;
                var gstDoctype = result.GSTDocumentType;
                var gstDocpath = result.GSTDocPath;
                var gstDocID = result.GSTDocumentId;

                $.each(Docgstname, function (i, doc) {
                    if (i == 0) {
                        document.getElementsByName('GSTDocumentType')[0].value = gstDoctype[0];
                        document.getElementsByName('GSTDocumentId')[0].value = gstDocID[0];
                        document.getElementsByName('GSTAttachName')[0].value = gstAttachname[0];
                        document.getElementsByClassName('link1')[0].text = Docgstname[0];
                        document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + gstDocpath[0] + "&filepath=~%2FContent%2FGstDocument%2F";
                    }
                    else {
                        var cln = $("#DocprimaryGSTDiv").clone().find("input").val("").end();
                        $(cln).find('.dis-none').removeClass('dis-none');
                        $('#DocdivGSTContent').append(cln)
                        document.getElementsByName('GSTDocumentType')[i].value = gstDoctype[i];
                        document.getElementsByName('GSTDocumentId')[i].value = gstDocID[i];
                        document.getElementsByName('GSTAttachName')[i].value = gstAttachname[i];
                        document.getElementsByClassName('link1')[i].text = Docgstname[i];
                        document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + gstDocpath[i] + "&filepath=~%2FContent%2FAgencyDocument%2F";
                    }
                });
                var Docvendorname = result.VendorDocumentName;
                var vendorAttachname = result.VendorAttachName;
                var vendorDoctype = result.VendorDocumentType;
                var vendorDocpath = result.VendorDocPath;
                var vendorDocID = result.VendorDocumentId;
                $.each(Docvendorname, function (i, doc) {
                    if (i == 0) {
                        document.getElementsByName('VendorDocumentType')[0].value = vendorDoctype[0];
                        document.getElementsByName('VendorDocumentId')[0].value = vendorDocID[0];
                        document.getElementsByName('VendorAttachName')[0].value = vendorAttachname[0];
                        document.getElementsByClassName('link2')[0].text = Docvendorname[0];
                        document.getElementsByClassName('link2')[0].href = "ShowDocument?file=" + vendorDocpath[0] + "&filepath=~%2FContent%2FGstDocument%2F";
                    }
                    else {
                        var cln = $("#DocprimaryVendorDiv").clone().find("input").val("").end();
                        $(cln).find('.dis-none').removeClass('dis-none');
                        $('#DocdivVendorContent').append(cln)
                        document.getElementsByName('VendorDocumentType')[i].value = vendorDoctype[i];
                        document.getElementsByName('VendorDocumentId')[i].value = vendorDocID[i];
                        document.getElementsByName('VendorAttachName')[i].value = vendorAttachname[i];
                        document.getElementsByClassName('link2')[i].text = Docvendorname[i];
                        document.getElementsByClassName('link2')[i].href = "ShowDocument?file=" + vendorDocpath[i] + "&filepath=~%2FContent%2FReverseTaxDocument%2F";
                    }
                });
                var Doctdsname = result.TDSDocumentName;
                var tdsAttachname = result.TDSAttachName;
                var tdsDoctype = result.TDSDocumentType;
                var tdsDocpath = result.TDSDocPath;
                var tdsDocID = result.TDSDocumentId;
                $.each(Doctdsname, function (i, doc) {
                    if (i == 0) {
                        document.getElementsByName('TDSDocumentType')[0].value = tdsDoctype[0];
                        document.getElementsByName('TDSDocumentId')[0].value = tdsDocID[0];
                        document.getElementsByName('TDSAttachName')[0].value = tdsAttachname[0];
                        document.getElementsByClassName('link3')[0].text = Doctdsname[0];
                        document.getElementsByClassName('link3')[0].href = "ShowDocument?file=" + tdsDocpath[0] + "&filepath=~%2FContent%2FTDSDocument%2F";
                    }
                    else {
                        var cln = $("#DocprimaryTDSDiv").clone().find("input").val("").end();
                        $(cln).find('.dis-none').removeClass('dis-none');
                        $('#DocdivTDSContent').append(cln)
                        document.getElementsByName('TDSDocumentType')[i].value = tdsDoctype[i];
                        document.getElementsByName('TDSDocumentId')[i].value = tdsDocID[i];
                        document.getElementsByName('TDSAttachName')[i].value = tdsAttachname[i];
                        document.getElementsByClassName('link3')[i].text = Doctdsname[i];
                        document.getElementsByClassName('link3')[i].href = "ShowDocument?file=" + tdsDocpath[i] + "&filepath=~%2FContent%2FTDSDocument%2F";
                    }
                });
                var tdssection = result.Section;
                var tdsdetailid = result.VendorTDSDetailId;

                var tdsnature = result.NatureOfIncome;
                var tdspercentage = result.TDSPercentage;
                $.each(tdssection, function (i, doc) {

                    if (i == 0) {
                        document.getElementsByName('VendorTDSDetailId')[0].value = tdsdetailid[0];
                        document.getElementsByName('Section')[0].value = tdssection[0];
                        document.getElementsByName('NatureOfIncome')[0].value = tdsnature[0];
                        document.getElementsByName('TDSPercentage')[0].value = tdspercentage[0];
                    }
                    else {
                        var cln = $('#tbodyPO tr:nth-child(2)').clone().find("input").val("").end();
                        $(cln).find('.dis-none').removeClass('dis-none');
                        $('#tbodyPO').append(cln);
                        document.getElementsByName('VendorTDSDetailId')[i].value = tdsdetailid[i];
                        document.getElementsByName('Section')[i].value = tdssection[i];
                        document.getElementsByName('NatureOfIncome')[i].value = tdsnature[i];
                        document.getElementsByName('TDSPercentage')[i].value = tdspercentage[i];
                    }
                });
            },
            error: function (err) {
                console.log("error1 : " + err);
            }
        });

    }
    function GetVendorAllList() {
        //$.ajax({

        //    type: "GET",
        //    url: GetVendorlist,
        //    data: "",
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (result) {

        //        $("#vendorList").jsGrid({
        //            data: result
        //        });
        //        db = result;
        //    },
        //    error: function (err) {
        //        console.log("error : " + err);
        //    }

        //});
        var input = [];
        input = {
            EXCountryName: $('#ddlSeaCountry').val(),
            EXVendorSearchname: $('#txtsrchname').val(),
            EXINVendorsearchCode: $('#ddlSearchVendorCode').val()
        }
        $("#vendorList").jsGrid("search", input, pageIndex = 1, pageSize = 10);
    }
    $('#btnSrchUser').on('click', function () {

        //var input = {
        //    VendorSearchname: $('#txtsrchname').val(),
        //    VendorsearchCode: $('#ddlSearchVendorCode').val(),
        //    VendorCountry: $('#ddlSeaCountry').val()
        //}

        //$.ajax({
        //    type: "Get",
        //    url: GetVendorlist,
        //    data: input,
        //    dataType: "json",
        //    success: function (result) {

        //        $("#vendorList").jsGrid({ data: result });
        //        $('#gridlist').show();

        //    },
        //    error: function (err) {
        //        console.log("error : " + err);
        //    }

        //});
        var input = [];
        input = {
            INVendorSearchname:null,
            INVendorsearchCode:null,
            INStatus:null,
            INBankName:null,
            INAccountNumber:null,
            EXCountryName: $('#ddlSeaCountry').val(),
            EXVendorSearchname: $('#txtsrchname').val(),
            EXINVendorsearchCode: $('#ddlSearchVendorCode').val()
        }
        $("#vendorList").jsGrid("search", input, pageIndex = 1, pageSize = 10);
    });

    $('#btnResetSrchUser').on('click', function () {
        $('#txtsrchname').val('');
        $('#ddlSearchVendorCode').val('');
        $('.selectpicker').selectpicker('refresh');
        $('#ddlSeaCountry').prop("selectedIndex", 0);
        GetVendorAllList();
    });

});