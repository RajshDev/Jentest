$(function () {
    var GetConsultantMaster = 'GetConsultantMaster',
        EditConsultantMasterlist = 'EditConsultantMasterlist',
        Deleteagencylist = 'DeleteInternalAgency';
    var db;
    $('#consultantList').jsGrid({
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
                    INConsultantSearchname: filter.Consultant_Name || null,
                    INConsultantsearchID: filter.ConsultantEmpId || null,
                    INStatus: filter.Status || null,
                    INCountry: filter.CountryName || null,
                    INConsultantCategory: filter.ConsultantCategory || null,
                    EXCountryName: $('#ddlSeaCountry').val(),
                    EXConsultantSearchname: $('#txtsrcnsltname').val(),
                    EXINConsultantsearchCode: $('#ddlSearchcnsltCode').val()

                },
                    filter.model = searchData;
                var deferred = $.Deferred();
                $.ajax({
                    type: "post",
                    url: GetConsultantMaster,
                    data: JSON.stringify(filter),
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {

                        var da = {
                            data: result.ConsultantList,
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
            { type: "number", name: "Consultant_MasterId", title: "Consultant MasterId", editing: false, visible: false },
            { type: "text", name: "Consultant_Name", title: "Name", editing: false },
            { type: "text", name: "ConsultantEmpId", title: "Consultant Id", align: "left", editing: false},                       
            { type: "text", name: "CountryName", title: "Country Name", editing: false },
            { type: "text", name: "ConsultantCategory", title: "Consultant Category", editing: false, },            
            { type: "text", name: "Status", title: "Status", align: "left", width: 60 },
            {
                type: "control", editButton: false, deleteButton: false, title: "Action", width: 80,
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Active") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "View", name: "View" }]
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
                            
                            editVendor(item.Consultant_MasterId)
                        }                        

                        else if (selVal == "View") {
                            //var url = '@Url.Action("ConsultantMasterView", "Requirement")?MasterId=' + item.Consultant_MasterId;
                            
                            var url = 'ConsultantMasterView?MasterId=' + item.Consultant_MasterId;
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
    $("#consultantList").jsGrid("option", "filtering", false);
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
    function editVendor(Consultant_MasterId) {
        $('#vendorMaster,#vendorhead1,#vendorhead4,#vendorhead5,#btnUpdate,#Divbutton').show();
        $('#ddlcnsltctry,#ddlcnsltCategory').attr("disabled", true); 
        $('#btnSave,#gridlist,#addnewpage').hide();
        $.ajax({
            type: "POST",
            url: EditConsultantMasterlist,
            data: { consultantMasterId: Consultant_MasterId },
            success: function (result) {
                
                $('#ddlcnsltctry').val(result.Consultant_Nationality);
                $('#ddlcnsltCategory').val(result.Consultant_Category);
                
                $('#txtNationality').val(result.Consultant_Nationality);
                $('#txtCategory').val(result.Consultant_Category);
                //$('#txtVendorCode').val(result.PFMSVendorCode);
                $('#txtVendorId').val(result.Consultant_MasterId);
                $('#Consultant_EmpId').val(result.Consultant_EmpId); 
                $('#consultantempid').val(result.Consultant_EmpId);                            
                $('#txtAcctHolderName').val(result.Consultant_AccountHolderName);
                //$('#txtBankName').val(result.Consultant_BankName);
                $('#Consultant_BankName').val(result.Consultant_BankName);
                $('#Consultant_FBankName').val(result.Consultant_FBankName);
                applyAutoComplete($('#Consultant_BankName'), $('#BankId'), "../Requirement/LoadBankNameList", undefined, undefined);
                applyAutoComplete($('#Consultant_FBankName'), $('#BankId'), "../Requirement/LoadBankNameList", undefined, undefined);
                $('#txtBranch').val(result.Consultant_Branch);
                $('#txtIfscCode').val(result.Consultant_IFSC);
                $('#ddlBankcountry').val(result.Consultant_BankCountry);

                $('#txtAcctNum').val(result.Consultant_AccountNumber);
                $('#txtFrAcctNum').val(result.Consultant_FrAccountNumber);
                $('#txtBankAddress').val(result.Consultant_BankAddress);
                $('#txtABANumber').val(result.Consultant_ABANumber);
                $('#txtSortCode').val(result.Consultant_SortCode);
                $('#txtIBAN').val(result.Consultant_IBAN);
                $('#txtBankNature').val(result.Consultant_BankNature);
                $('#txtBankEmail').val(result.Consultant_BankEmailId);
                $('#txtMICRCode').val(result.Consultant_MICRCode);
                $('#txtSwiftBICCode').val(result.Consultant_SWIFTorBICCode);
                $("#divempId").show();                     
                if (result.Consultant_Nationality == 1 && result.Consultant_Category == 1) {

                    
                    $('#indfirmdiv').hide();
                    $('#vendorhead2').show();

                    $('#frginddiv').hide();
                    $('#frgfirmdiv').hide();

                    $('#abaDiv').hide();
                    $('#swiftCode').hide();
                    $('#stateCodeDiv').show();

                    $("#hiddenInBankname").show();
                    $("#hiddenFrBankname").hide();
                    $("#hiddenInAccno").show();
                    $("#hiddenFrAccno").hide();

                    $('#Consultant_EmpId').val(result.Consultant_EmpId);
                    $('#consultantempid').val(result.Consultant_EmpId);  
                    $('#txtIConsultantName').val(result.Consultant_Name);

                    $('#hiddVendorName').val(result.Consultant_Name);

                    $('#ddlIcnsltSalutation').val(result.Consultant_Salutation);
                    //$('#ddlIcnsltGender').val(result.Consultant_Gender);
                    //$('#lblSex').val(result.Consultant_Gender);
                    
                    Professional(result.Consultant_Gender);
                    var strDate = new Date(parseInt(result.Consultant_DOB.replace(/(^.*\()|([+-].*$)/g, '')));                    
                    //$('#txIcnslttDOB').datepicker('setDate', strDate);
                    $('#Consultant_DOB').datepicker('setDate', strDate);
                    //$('#txIcnslttDOB').datepicker(result.Consultant_DOB, formatDate: 'dd/mm/yy');
                    $('#txtIPhoneNumber').val(result.Consultant_ContactNumber);
                    $('#txtIEmail').val(result.Consultant_Email);
                    $('#txtIAadhaarno').val(result.Consultant_AadhaarNo);
                    
                    //$('#txtIPANNumber').attr('readonly',true);
                    $('#txtIPANNumber').attr('disabled', 'disabled');
                    $('#txtIPANNumber').val(result.Consultant_PanNo);
                    $('#txtIAddress').val(result.Consultant_Address);
                    $('#hiddAddress').val(result.Consultant_Address);
                    $('#hiddEmailAddress').val(result.Consultant_Email); 
                    //$('#txtContactPerson').val(result.ContactPerson);
                    //$('#hiddContactPerson').val(result.ContactPerson);
                    $('#txtICity').val(result.Consultant_City);
                    //$('#txtICity').val(result.Consultant_City);
                    $('#txtIconsltquali').val(result.Consultant_Qualification);
                    $('#ddlIstate').val(result.Consultant_StateId);
                    $('#txtIPinCode').val(result.Consultant_Pincode);
                    $('#txtIconsltexp').val(result.Consultant_Experience);
                    CheckIEmployeestate();
                    //$('#txtIStateCode').val(result.Consultant_StateCode);
                    $('#txtIServiceAddress').val(result.Consultant_ServiceAddress);

                    if (result.PersonImagePath != null) {
                        $('#PersonImagePath').val(result.PersonImagePath)
                        $('#displayImg').attr("src", "/Account/ShowDocument?file=" + result.PersonImagePath + "&filepath=RCTEmployeeImages");
                    }
                    if (result.isSameAsAddress == true) {
                        $("#isSameAsAddress").prop("checked", true);
                    }
                    else {
                        $("#isSameAsAddress").prop("checked", false);
                    }
                    
                    //$('#txtIGSTNumber').removeAttr('readonly');
                    //$('#IsGST').removeAttr('readonly');
                    
                    if (result.IsGST == true) {
                        //$("#IsGST").attr('disabled', true);
                        $("input[name='IsGST']").val(result.IsGST);
                        $('#txtIGSTNumber').val(result.GSTIN);                       
                        
                        $('#txtIGSTNumber').attr('disabled', 'disabled');
                        $("input[name='IsGST']").prop("checked", true);
                        var gstNumber = $('#txtIGSTNumber').val();
                        $('#hiddGSTIN').val(gstNumber);
                        
                    }
                    else {
                        //$("#IsGST").prop("checked", false);
                        $("input[name='IsGST']").prop("checked", false);
                        $('#txtIGSTNumber').val(result.GSTIN);
                        //$("#IsGST").removeAttr('disabled');
                        //$("input[name='IsGST']").val(result.IsGST);
                        //$("#IsGST").val(result.IsGST);
                    }
                    if (result.GSTIN == "" || result.GSTIN == null) {
                        $("#IsGST").removeAttr('disabled');
                    }
                    //else {
                    //    $("#IsGST").removeAttr('disabled');
                    //}
                    
                    

                    //CheckIEmployeepanno();
                    

                    $("#lblifsc").addClass("required");
                    $("#ifsccode").show();
                    $("#hiddenBankcountry").hide();
                    //$("#lblservice").addClass("required");
                    //$("#ddlcategoryservice").prop('required', true);
                    $("#vendorhead5").show();

                }
                else if (result.Consultant_Nationality == 2 && result.Consultant_Category == 2) {

                    //$("#lblemail").removeClass("required");
                    //$("#txtEmail").prop('required', false);
                    //$('#txtFFConsultantName,#ddlFFcnsltSalutation').prop('disabled', true);
                    $('#indfirmdiv').hide();
                    $('#vendorhead2').hide();

                    $('#frginddiv').hide();
                    $('#frgfirmdiv').show();


                    //$('#countryDiv').show();
                    $('#abaDiv').show();
                    $('#swiftCode').show();


                    $("#ifsccode").hide();
                    $("#hiddenBankcountry").show();
                    $("#lblservice").removeClass("required");
                    $("#ddlcategoryservice").prop('required', false);
                    $("#ddlcountry option[value='128']").remove();
                    $("#vendorhead5").show();

                    $("#hiddenInBankname").hide();
                    $("#hiddenFrBankname").show();
                    $("#hiddenInAccno").hide();
                    $("#hiddenFrAccno").show();

                    $('#txtFFConsultantName').val(result.Consultant_FFName);
                    $('#hiddVendorName').val(result.Consultant_FFName);
                    $('#ddlFFcnsltSalutation').val(result.Consultant_FFSalutation);                    
                    $('#txtFFPhoneNumber').val(result.Consultant_FFContactNumber);
                    $('#txtFFEmail').val(result.Consultant_FFEmail);
                    $('#txtFFAddress').val(result.Consultant_FFAddress);
                    $('#hiddFFAddress').val(result.Consultant_FFAddress);
                    $('#ddlFFcountry').val(result.Consultant_FFCountry);
                    $('#hiddEmailAddress').val(result.Consultant_FFEmail);                    
                    $('#txtFFCity').val(result.Consultant_FFCity);                    
                    $('#txtFFPinCode').val(result.Consultant_FFPincode);
                    //$('#chkisSameAsFFAddress').val(result.isSameAsFFAddress);
                    $('#txtFFServiceAddress').val(result.Consultant_FFServiceAddress);
                    $('#txtFFTin').val(result.Consultant_FFTIN);
                               
                    if (result.isSameAsFFAddress == true) {
                        $("#isSameAsFFAddress").prop("checked", true);
                    }
                    else {
                        $("#isSameAsFFAddress").prop("checked", false);
                    }

                
                //$('#hiddRegName').val(result.RegisteredName);

                }
                else if (result.Consultant_Nationality == 1 && result.Consultant_Category == 2) {

                    
                    $('#indfirmdiv').show();
                    $('#vendorhead2').hide();

                    $('#frginddiv').hide();
                    $('#frgfirmdiv').hide();
                    

                    $('#abaDiv').hide();
                    $('#swiftCode').hide();


                    $("#ifsccode").show();
                    $("#hiddenBankcountry").hide();
                    $("#lblservice").removeClass("required");
                    $("#ddlcategoryservice").prop('required', false);
                    $("#ddlcountry option[value='128']").remove();
                    $("#vendorhead5").show();

                    $("#hiddenInBankname").show();
                    $("#hiddenFrBankname").hide();
                    $("#hiddenInAccno").show();
                    $("#hiddenFrAccno").hide();
                   
                    $('#txtIFConsultantName').val(result.Consultant_IFName);


                    $('#hiddVendorName').val(result.Consultant_IFName);

                    $('#ddlIFcnsltSalutation').val(result.Consultant_IFSalutation);                                        
                    $('#txtIFPhoneNumber').val(result.Consultant_IFContactNumber);
                    $('#txtIFEmail').val(result.Consultant_IFEmail);
                    $('#txtIFPANNumber').attr('disabled', 'disabled');
                    $('#txtIFPANNumber').val(result.Consultant_IFPanNo);
                    //$('#txtIFGSTNumber').removeAttr('readonly');
                    //$('#chkIFIsGST').removeAttr('readonly');
                    
                    
                    $('#txtIFAddress').val(result.Consultant_IFAddress);
                    $('#hiddIFAddress').val(result.Consultant_IFAddress);
                    $('#hiddEmailAddress').val(result.Consultant_IFEmail);
                    //$('#txtContactPerson').val(result.ContactPerson);
                    //$('#hiddContactPerson').val(result.ContactPerson);
                    $('#txtIFCity').val(result.Consultant_IFCity);
                    $('#txtIFStateCode').val(result.Consultant_IFStateCode);
                   
                    $('#ddlIFstate').val(result.Consultant_IFStateId);
                    $('#txtIFPinCode').val(result.Consultant_IFPincode);   
                    //$('#chkisSameAsIFAddress').val(result.isSameAsIFAddress);
                    $('#txtIFServiceAddress').val(result.Consultant_IFServiceAddress);
               
                    if (result.isSameAsIFAddress == true) {
                        $("#isSameAsIFAddress").prop("checked", true);
                    }
                    else {
                        $("#isSameAsIFAddress").prop("checked", false);
                    }
                    if (result.IsGSTIF == true) {
                        
                        $("input[name='IsGSTIF']").val(result.IsGSTIF);                       
                        $('#txtIFGSTNumber').val(result.GSTINIF);
                        $("input[name='IsGSTIF']").prop("checked", true);
                        $('#txtIFGSTNumber').attr('disabled', 'disabled');
                        var gstNumber = $('#txtIFGSTNumber').val();
                        $('#hiddGSTINIF').val(gstNumber);
                        //$("input[name='IsGSTIF']").val(result.IsGSTIF);
                    }
                    else {
                        
                        $("input[name='IsGSTIF']").prop("checked", false);
                        $('#txtIFGSTNumber').val(result.GSTIN);
                        //$("#IsGST").removeAttr('disabled');
                        //$("input[name='IsGSTIF']").val(result.IsGST);
                    }
                    if (result.IsGSTIF == "" || result.IsGSTIF == null) {
                        $("#IsGSTIF").removeAttr('disabled');
                    }
                   
                   
                    
                    //CheckEmployeepanno();
                }
                else if (result.Consultant_Nationality == 2 && result.Consultant_Category == 1) {

                    //$("#lblemail").removeClass("required");
                    //$("#txtEmail").prop('required', false);
                    //$('#txtFIConsultantName,#ddlFIcnsltSalutation,#ddlFIcnsltGender,#txFIcnslttDOB').prop('disabled', true);
                    $('#indfirmdiv').hide();
                    $('#vendorhead2').hide();

                    $('#frginddiv').show();
                    $('#frgfirmdiv').hide();

                    $('#stateDiv').hide();
                    //$('#countryDiv').show();
                    $('#abaDiv').show();
                    $('#swiftCode').show();

                    $("#ifsccode").hide();
                    $("#hiddenBankcountry").show();
                    $("#lblservice").removeClass("required");
                    $("#ddlcategoryservice").prop('required', false);
                    $("#ddlcountry option[value='128']").remove();
                    $("#vendorhead5").show();

                    $("#hiddenInBankname").hide();
                    $("#hiddenFrBankname").show();
                    $("#hiddenInAccno").hide();
                    $("#hiddenFrAccno").show();

                    $('#txtFIConsultantName').val(result.Consultant_FIName);


                    $('#hiddVendorName').val(result.Consultant_FIName);

                    
                    $('#ddlFIcnsltSalutation').val(result.Consultant_FISalutation);                   
                    ProfessionalFI(result.Consultant_FIGender);
                    var strDate = new Date(parseInt(result.Consultant_fi_DOB.replace(/(^.*\()|([+-].*$)/g, '')));                    
                    $('#Consultant_fi_DOB').datepicker('setDate', strDate);
                    //$('#txFIcnslttDOB').val(result.Consultant_fi_DOB);
                    $('#txtFIPhoneNumber').val(result.Consultant_FIContactNumber);
                    $('#txtFIEmail').val(result.Consultant_FIEmail);
                    $('#txtFIAddress').val(result.Consultant_FIAddress);
                    $('#hiddFIAddress').val(result.Consultant_FIAddress);
                    $('#hiddEmailAddress').val(result.Consultant_FIEmail);
                    //$('#txtContactPerson').val(result.ContactPerson);
                    //$('#hiddContactPerson').val(result.ContactPerson);
                    $('#txtFICity').val(result.Consultant_FICity);
                    $('#txtFIconsltquali').val(result.Consultant_FIQualification);

                    if (result.isSameAsFIAddress == true) {
                        $("#isSameAsFIAddress").prop("checked", true);
                    }
                    else {
                        $("#isSameAsFIAddress").prop("checked", false);
                    }

                    $('#txtFIPinCode').val(result.Consultant_FIPincode);
                    $('#txtFIconsltexp').val(result.Consultant_FIExperience);
                    //$('#chkisSameAsFIAddress').val(result.isSameAsFIAddress);
                    $('#txtFIServiceAddress').val(result.Consultant_FIServiceAddress);
                    $('#txtFITin').val(result.Consultant_FITIN);
                    $('#ddlFIcountry').val(result.Consultant_FICountry);
                    if (result.PersonImageFIPath != null) {
                        $('#PersonImageFIPath').val(result.PersonImageFIPath)
                        $('#displayFIImg').attr("src", "/Account/ShowDocument?file=" + result.PersonImageFIPath + "&filepath=RCTEmployeeImages");
                    }
                                       
                }

              
                //var Docvendorname = result.VendorDocumentName;
                var vendorAttachFilename = result.AttachmentFileName;
                var vendorAttachname = result.AttachmentName;
                //var vendorDoctype = result.VendorDocumentType;
                var vendorDocpath = result.AttachmentPath;

                var vendorDocID = result.ConsultantDocumentID;
              //  const fileInput = document.querySelector('input[type="file"]');
                //var len = document.getElementsByName('AttachmentName').length;
                //if (len == 1)
                //    $("#firstremoveButton").removeClass('dis-none');
                $.each(vendorAttachname, function (i, doc) {
                    if (i == 0) {
                        //document.getElementsByName('VendorDocumentType')[0].value = vendorDoctype[0];
                        document.getElementsByName('ConsultantDocumentID')[0].value = vendorDocID[0];
                        document.getElementsByName('AttachmentName')[0].value = vendorAttachname[0];
                        document.getElementsByClassName('link2')[0].text = vendorDocpath[0];
                        document.getElementsByName('vendordocfile')[0].value = vendorDocpath[0]
                        //document.getElementsByClassName('ConsultantFile')[0].text = vendorDocpath[0];
                        
                        document.getElementsByClassName('link2')[0].href = "/Account/ShowDocument?file=" + vendorDocpath[0] + "&filepath=~%2FContent%2FRequirement%2F";
                    }
                    else {
                        $("#firstremoveButton").removeClass('dis-none');
                        var cln = $("#DocprimaryVendorDiv").clone().find("input").val("").end();
                        $(cln).find('.dis-none').removeClass('dis-none');
                        $('#DocdivVendorContent').append(cln)
                        //document.getElementsByName('VendorDocumentType')[i].value = vendorDoctype[i];
                        document.getElementsByName('ConsultantDocumentID')[i].value = vendorDocID[i];
                        document.getElementsByName('AttachmentName')[i].value = vendorAttachname[i];                        
                        document.getElementsByClassName('link2')[i].text = vendorDocpath[i];
                        document.getElementsByName('vendordocfile')[i].value = vendorDocpath[i]
                        //document.getElementsByClassName('ConsultantFile')[i].text = vendorDocpath[i];
                        document.getElementsByClassName('link2')[i].href = "/Account/ShowDocument?file=" + vendorDocpath[i] + "&filepath=~%2FContent%2FRequirement%2F";
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
            EXConsultantSearchname: $('#txtsrcnsltname').val(),
            EXINConsultantsearchCode: $('#ddlSearchcnsltCode').val()
        }
        $("#consultantList").jsGrid("search", input, pageIndex = 1, pageSize = 10);
    }
    $('#btnSrchUser').on('click', function () {

        //var input = {
        //    VendorSearchname: $('#txtsrcnsltname').val(),
        //    VendorsearchCode: $('#ddlSearchcnsltCode').val(),
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
            INVendorSearchname: null,
            INVendorsearchCode: null,
            INCountry: null,
            INStatus: null,
            INBankName: null,
            INAccountNumber: null,
            EXCountryName: $('#ddlSeaCountry').val(),
            EXConsultantSearchname: $('#txtsrcnsltname').val(),
            EXINConsultantsearchCode: $('#ddlSearchcnsltCode').val()
        }
        $("#consultantList").jsGrid("search", input, pageIndex = 1, pageSize = 10);
    });

    $('#btnResetSrchUser').on('click', function () {
        $('#txtsrcnsltname').val('');
        $('#ddlSearchcnsltCode').val('');
        $('#ddlSeaCountry').val('');
        $('.selectpicker').selectpicker('refresh');
        $('#ddlSeaCountry').prop("selectedIndex", 0);
        GetVendorAllList();
    });

});