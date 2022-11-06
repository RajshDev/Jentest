$(function () {
    //Declare RO List
    /*var getRODetailsURL = 'GetROList';

    EditEnhancement = 'EditProjectenhancement';
    EditExtension = 'EditProjectextension';
    delEnhancement = 'DeleteEnhancement';*/
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
    //RO List

    $("#gridProjectFundingCategoryList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        pageLoading: true,
        autoload: true,
        controller: {
            loadData: function (filter) {
                var searchData = [];
                searchData = {
                    ProjectNumber: filter.ProjectNumber,
                    RONumber: filter.RONumber,
                    /*RODate: filter.RODate,*/
                    ROProjValue: filter.ROProjValue,
                    /*ROBalanceValue: filter.ROBalanceValue,*/
                    Status: filter.Status,
                    /*ROId: filter.ROId,*/
                    ProjId: filter.ProjId,
                    PIdName: filter.PIdName,
                    BankName: filter.BankName,
                    ROAprvId: filter.ROAprvId
                },
                    filter.model = searchData;
                var deferred = $.Deferred();
                $.ajax({
                    type: "post",
                    url: 'GetROList',
                    data: JSON.stringify(filter),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        
                        var da = {
                            data: result.list,
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
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            //{ type: "number", name: "ROId", title: "RO Id", visible: false },
            { type: "number", name: "ProjId", title: "Project ID", visible: false },
            { type: "text", name: "RONumber", title: "RO Number", visible: false },
            //{ type: "date", name: "PrsntDueDate", title: "RO Date", width: 100, align: "center" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIdName", title: "PI Name", align: "left", editing: false, width: "60px" },
            { type: "text", name: "BankName", title: "Bank Name", align: "left", editing: false, width: "65px" },
            { type: "text", name: "ROProjValue", title: "RO Project value", editing: false, width: "50px" },
            { type: "text", name: "ROAprvId", title: "RO Project Approval Id", visible: false},
            { type: "text", name: "Status", title: "Status", editing: false, width: "55px" },

            {
                type: "control", editButton: false, deleteButton: false, width: "100px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                },
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Open") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "Edit", name: "Edit" }, { id: "View", name: "View" },  { id: "Submit for approval", name: "Submit for approval" }]

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
                        if (selVal == "View") {
                            var url = 'ViewRODetails?ProjectId= ' + item.ProjId + '&aprvdId=' + item.ROAprvId; 
                            window.location.href = url;

                        } if (selVal == "Edit") {
                            /*editEnhance(item.ProjectEnhancementID);*/
                            var url = 'CreateRO?ProjectId= ' + item.ProjId + '&aprvdId=' + item.ROAprvId;
                                window.location.href = url;

                        }
                        else if(selVal == "Submit for approval") {
                                var choice = confirm("Are you sure, Do you want to submit this project RO for approval process?");
                                if (choice === true) {
                                    $.ajax({
                                        type: "POST",
                                        url: 'ProjectROWFInit',
                                        data: {
                                            AprvdId: item.ROAprvId,
                                            ProjId: item.ProjId
                                        },
                                        success: function (result) {
                                            if (result == true) {
                                                $('#alertSuccess').html("Project RO has been submitted for approval process successfully.");
                                                $('#Success').modal('toggle');
                                                GetROlist();
                                            } else if (result == false) {
                                                $('#FailedAlert').html("Something went wrong please contact administrator");
                                                $('#Failed').modal('toggle');
                                            }
                                        },
                                        error: function (err) {
                                            console.log("error1 : " + err);
                                        }
                                    });
                                }
                        }

                        $(this).val("");
                        // return false;
                        e.stopPropagation();
                    });
                    return $result.add($customSelect);
                },
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                },
            }
            //},
        ],
    });

    function GetROlist() {
        var input = [];
        input = {
            ProjectNumber: null,
            RONumber: null,
            ROProjValue: null,
            ROBalanceValue: null,
            Status: null,
            ROId: null,
            ProjId: null,
            PIdName: null,
            BankName: null

        },

            $("#gridProjectFundingCategoryList").jsGrid("search", input, pageIndex = 1, pageSize = 5);

    }
    $("#gridProjectFundingCategoryList").jsGrid("option", "filtering", false);
    });