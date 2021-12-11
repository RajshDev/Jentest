//**************************************************************************************************************************************//
//                                                      PROCESS GUIDELINE                                                               //
//**************************************************************************************************************************************//

//GLOBAL VARIABLES
var unselectUser = [], selectUser = [];
$(function () {

    //DECLARE LOCAL VARIABLES
    var getProcessFlowDetailsURL = 'GetProcessFlowList',
        getUserCount = 'GetUserCounts',
        addProcessFlowDetailsURL = 'AddProcessFlow',
        insertProcessGuidelineURL = 'InsertProcessGuideline',
        updateProcessFlowDetailsURL = 'UpdateProcessFlow',
        deleteProcessFlowDetailsURL = 'DeleteProcessFlow',
        getUnassignedUserDetailsURL = 'GetProcessFlowUserDetails',
        getApproverListURL = 'GetApproverList',
        getStatusURL = 'GetStatus',
        addApproverDetailsURL = 'AddApproverDetails',
        getAllApproverListURL = 'GetAllApproverList',
        loadControlsURL = 'LoadControls',
        getProcessGuideLineListURL = 'GetProcessGuideLineList';
        deletePGLWorkflowURL = 'DeletePGLWorkflow',
        mapProcessflowUserURL = 'MapProcessflowUser';
        unmapProcessflowUserURL = 'UnmapProcessflowUser';
        var db1, dataUnassignedUser, dataAssignedUser, pglDetailId = 0, pglId = 0, approverList, statusList, documentList;
    
    //LOAD EVENT
    $.ajax({
        type: "GET",
        url: loadControlsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            approverList = result.ApproverList;
            statusList = result.StatusList;
            documentList = result.DocumentList;
            approverCategory = result.ApproverType;
            var optionSrch = $("#selectFunctionSrch"), option = $("#selectFunction");
            optionSrch.append($("<option />").val(-1).text('Select All'));
            var functionIndex = "<option value='0'>Select Function</option>";
            option.html(functionIndex);
            $.each(result.FunctionList, function () {
                optionSrch.append($("<option />").val(this.FunctionId).text(this.FunctionName));
                option.append($("<option />").val(this.FunctionId).text(this.FunctionName));
            });
            $("#btnAddNew,#btnFilterPGL").css('display', 'block');
            $("#btnClosePGL").css('display', 'none');
            Search();
        },
        error: function (err) {
            console.log("Src:ProcessGuideline.js;method:Load Event;error:" + err);
        }
    });

    //SEARCH GRID DECLARATION
    $("#gridProcessGuidelineSearch").jsGrid({
        autoload: true,
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        data: db1,
        editing: false,
        selecting: true,
        fields: [
            { type: "number", name: "SlNo", title: "S.No", visible: true, align: "left" },
            { type: "number", name: "FunctionId", title: "FunctionId", visible: false },
            { type: "text", name: "FunctionName", title: "Function Name" },
            { type: "text", name: "ProcessName", title: "Flow" },
            {
                name: "ProcessGuidelineId",
                title: "Action",
                itemTemplate: function (value, item) {
                    return $("<a>").attr("href", "#").attr("process", item.ProcessName).attr("functionId", item.FunctionId).text('Select').on("click", function () {
                        $('#spanAssignedUser,#spanApprover').html('');
                        $("#btnAddNew,#btnFilterPGL").css('display', 'none');
                        $("#btnClosePGL").css('display', 'block');
                        $("#btnSave").attr("disabled", false);
                        $('#numUsersCount').text(0);
                        $('#numApproverCount').text(0);
                        pglId = value;
                        pglDetailId = 0;
                        $('#txtProcess').val($(this).attr("process"));
                        $('#divOuterPGL').css('display', 'none');
                        $('#divInnerPGL').css('display', 'block');
                        $('#selectFunction').val($(this).attr("functionId"));
                        GetFlowList();
                        GetProcessFlowList();
                        GetApproverFlowList();
                        GetUserProcessflowList();
                        return false;
                    });
                }
            }
        ]
    });
    var unadb;
    //UNASSIGNED USER GRID DECLARATION
    $("#gridUnassigneduser").jsGrid({

        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(unadb, function (user) {
                    return (!filter.UserName || user.UserName.toLowerCase().indexOf(filter.UserName.toLowerCase()) > -1)
                });
            }

        },
        fields: [
            { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
            { type: "number", name: "UserId", title: "UserId", visible: false },
            { type: "text", name: "UserName", title: "User Name" },
            
            {
                name: "UserFlag",
                headerTemplate: function () {
                    return $("<input>").attr("type", "checkbox")
                            .on("change", function () {
                                var chkValue = $(this).is(":checked");
                                $('.singleChkUnassigned').each(function () {
                                    this.checked = chkValue;
                                });
                                selectUser = [];
                                if ($(this).is(":checked")) {
                                    $('input[unassignkey]').each(function () {
                                        selectUser.push($(this).attr('unassignkey'));
                                    });
                                }
                            });
                },
                itemTemplate: function (_, item) {
                    return $("<input>").attr("type", "checkbox")
                            .prop("checked", false)
                            .attr("class", "singleChkUnassigned")
                            .attr("unassignkey", item.UserId)
                            .on("change", function () {
                                if ($(this).is(":checked")) {
                                    selectUser.push(item.UserId);
                                }
                                else {
                                    selectUser.pop(item.UserId);
                                }
                            });
                },
                align: "center",
                width: 50
            },
             {
                 type: "control", width: "100px",
                 editButton: false,
                 deleteButton:false,
            _createFilterSwitchButton: function () {
                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
        }
        },
        ]
    });
    $("#gridUnassigneduser").jsGrid("option", "filtering", false);
    //ASSIGNED USER GRID DECLARATION
    var assuser;
    $("#gridAssignedUser").jsGrid({
       
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(assuser, function (an) {
                    return (!filter.UserName || an.UserName.toLowerCase().indexOf(filter.UserName.toLowerCase()) > -1)
                });
            }

        },
        fields: [
            { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
            { type: "number", name: "UserId", title: "UserId", visible: false },
            { type: "text", name: "UserName", title: "User Name" },
            {
                name: "UserFlag", title: "Select All",
                headerTemplate: function () {
                    return $("<input>").attr("type", "checkbox")
                            .on("change", function () {
                                var chkValue = $(this).is(":checked");
                                $('.singleChkAssigned').each(function () {
                                    this.checked = chkValue;
                                });
                                unselectUser = [];
                                if ($(this).is(":checked")) {
                                    $('input[assignkey]').each(function () {
                                        unselectUser.push($(this).attr('assignkey'));
                                    });
                                }
                            });
                },
                itemTemplate: function (_, item) {
                    return $("<input>").attr("type", "checkbox")
                            .prop("checked", !(item.UserFlag))
                            .attr("class", "singleChkAssigned")
                            .attr("assignkey", item.UserId)
                            .on("change", function () {
                                if ($(this).is(":checked")) {
                                    unselectUser.push(item.UserId);
                                }
                                else {
                                    unselectUser.pop(item.UserId);
                                }
                            });
                },
                align: "center",
                width: 50
            },
            {
                type: "control", width: "100px",
                editButton: false,
                deleteButton: false,
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                }
            },
        ]

    });
    $("#gridAssignedUser").jsGrid("option", "filtering", false);
    //PROCESS FLOW GRID DECLARATION
    //$("#gridProcessFlow").jsGrid({
    //    autoload: true,
    //    data: db1,
    //    paging: true,
    //    pageIndex: 1,
    //    pageSize: 4,
    //    editing: true,
    //    selecting: false,
    //    controller: {
    //        insertItem: function (item) {
    //            var input = {
    //                ProcessGuidelineid: pglId,
    //                ProcessGuidelineDetailId: pglDetailId,
    //                FlowTitle: item.FlowTitle
    //            };
    //            $.ajax({
    //                type: "POST",
    //                url: addProcessFlowDetailsURL,
    //                data: input,
    //            }).done(function (response) {
    //                GetProcessFlowList();
    //                return;
    //            }).fail(function (error) {
    //                console.log("Src:ProcessGuideline.js;method:gridProcessFlow-insertItem;error:" + error);
    //            });
    //        },
    //        updateItem: function (item) {
    //            $.ajax({
    //                type: "POST",
    //                url: updateProcessFlowDetailsURL,
    //                data: item
    //            }).done(function (response) {
    //                GetProcessFlowList();
    //                return;
    //            }).fail(function (error) {
    //                console.log("Src:ProcessGuideline.js;method:gridProcessFlow-updateItem;error:" + error);
    //            });
    //        },
    //        deleteItem: function (item) {
    //            $.ajax({
    //                type: "POST",
    //                url: deleteProcessFlowDetailsURL,
    //                data: { processGuidelineDetailId: item.ProcessGuidelineDetailId }
    //            }).done(function (response) {
    //                GetProcessFlowList();
    //                GetApproverFlowList();
    //                GetUserProcessflowList();
    //                return;
    //            }).fail(function (error) {
    //                console.log("Src:ProcessGuideline.js;method:gridProcessFlow-deleteItem;error:" + error);
    //            });
    //        }
    //    },
    //    fields: [
    //        { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
    //        {
    //            type: "text", name: "FlowTitle", title: "Flow Title", validate: {
    //                message: "Flow Title is required", validator: function (value) {
    //                    return !(value == undefined || value == "");
    //                }
    //            }
    //        },
    //        {
    //            name: "UserCount", title: "User Count"
    //        },
    //        {
    //            type: "control",
    //            editButton: true,
    //            deleteButton: true,
    //            selectButton: true,
    //            width: 50,
    //            headerTemplate: function () {
    //                var grid = this._grid;
    //                var isInserting = grid.inserting;
    //                var $button = $("<input>").attr("type", "button")
    //                    .addClass([this.buttonClass, this.modeButtonClass, this.insertModeButtonClass].join(" "))
    //                    .on("click", function () {
    //                        if (pglId != 0) {
    //                            isInserting = !isInserting;
    //                            grid.option("inserting", isInserting);
    //                        }
    //                        else {
    //                            alert("Please save process in header");
    //                        }
    //                    });
    //                return $button;
    //            }
    //        },
    //        {
    //            width: 50,
    //            itemTemplate: function (value, item) {
    //                return $("<a>").attr("href", "#").text("Select")
    //                        .css('color', 'white')//todo
    //                        .on("click", function () {
    //                            //Select button event - load related user
    //                            $('#spanAssignedUser,#spanApprover').html(item.FlowTitle);
    //                            pglDetailId = item.ProcessGuidelineDetailId;
    //                            selectUser = [];
    //                            unselectUser = [];
    //                            GetUserProcessflowList();
    //                            GetApproverFlowList();
    //                            return false;
    //                        });
    //            },
    //            editTemplate: function (value, item) {
    //                return $("<a>").attr("href", "#").css('color', 'white').text("Select").on("click", function () {
    //                    return false;
    //                });
    //            }
    //        }
    //    ]
    //});

    //MULISELECT START

    var MultiselectField = function (config) {
        jsGrid.Field.call(this, config);
    };
    MultiselectField.prototype = new jsGrid.Field({
        items: [],
        textField: "",
        itemTemplate: function (value) {
            return $.makeArray(value).join(", ");
        },
        _createSelect: function (selected) {
            var textField = this.textField;
            var $result = $("<select>").prop("multiple", true);

            $.each(this.items, function (_, item) {
                var value = item[textField];
                var $opt = $("<option>").text(value);
                if ($.inArray(value, selected) > -1) {
                    $opt.attr("selected", "selected");
                }
                $result.append($opt);
            });

            return $result;
        },

        insertTemplate: function () {
            var insertControl = this._insertControl = this._createSelect();

            //setTimeout(function () {
            //    insertControl.multiselect({
            //        minWidth: 140
            //    });
            //});

            return insertControl;
        },

        editTemplate: function (value) {
            var editControl = this._editControl = this._createSelect(value);
            //setTimeout(function () {
            //    editControl.multiselect({
            //        minWidth: 140
            //    });
            //});

            return editControl;
        },

        insertValue: function () {
            return this._insertControl.find("option:selected").map(function () {
                return this.selected ? $(this).text() : null;
            });
        },

        editValue: function () {
            var retItems = this._editControl.find("option:selected").map(function () {
                return this.selected ? $(this).text() : null;
            });
            return retItems;
        }

    });
    jsGrid.fields.multiselect = MultiselectField;

    //MULTISELECT END

    //APPROVELIST GRID DECLARATION
    $("#gridApproverList").jsGrid({
       
        data: db1,
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        selecting: true,
        deleteConfirm: "Do you really want to delete ?",
        rowClass: function (item, itemIndex) {
            return "client-" + itemIndex;
        },
        controller: {
            insertItem: function (item) {
               
                
                if (pglDetailId!=0) {
                   
                    var inputDoc = [];
                    $.each(item.SelectedDocument, function (index, value) {
                        inputDoc.push(value);
                    });
                    //var appList = [];
                    //$.each(item.ApproverList, function (index, value) {
                    //    appList.push(value);
                    //});
                    
                    model = {
                        ProcessguidlineworkflowId: 0,
                        processguidlineId: pglId,
                        ProcessGuidelineDetailId: pglDetailId,
                        ApproverLevel: item.ApproverLevel,
                        UserId: item.UserId,
                        StatusId: item.StatusId,
                        ApproveFlag: item.ApproveFlag,
                        RejectFlag: item.RejectFlag,
                        ClarifyFlag: item.ClarifyFlag,
                        MarkFlag: item.MarkFlag,
                        DocumentId: item.DocumentId,
                        SelectedDocument: inputDoc,
                        //ApproverList: $('#appSelectIndex').val(),
                        DocumentType: 'PDF',
                        IsRequired: true,
                        UUID: 'Hardvalue'
                    }
                    
                    $.ajax({
                        type: "POST",
                        url: addApproverDetailsURL,
                        data: model
                       
                    }).done(function (response) {
                        GetApproverFlowList();
                        GetUserCountlist(pglDetailId);
                        return;
                    }).fail(function (error) {
                        console.log("Src:ProcessGuideline.js;method:gridApproverList-insertItem;error:" + error);
                    });
                }
                else {
                    alert("Please select any process flow");
                   
                }
            },
            updateItem: function (item) {
               
                var inputDoc = [];
                $.each(item.SelectedDocument, function (index, value) {
                    inputDoc.push(value);
                });
                //var appList = [];
                //$.each(item.ApproverList, function (index, value) {
                //    appList.push(value);
                //});
                model = {
                    ProcessguidlineworkflowId: item.ProcessguidlineworkflowId,
                    processguidlineId: pglId,
                    ProcessGuidelineDetailId: pglDetailId,
                    ApproverLevel: item.ApproverLevel,
                    UserId: item.UserId,
                    StatusId: item.StatusId,
                    ApproveFlag: item.ApproveFlag,
                    RejectFlag: item.RejectFlag,
                    ClarifyFlag: item.ClarifyFlag,
                    MarkFlag: item.MarkFlag,
                    DocumentId: item.DocumentId,
                    SelectedDocument: inputDoc,
                    DocumentType: 'PDF',
                    IsRequired: true,
                    UUID: 'Hardvalue'
                }
                $.ajax({
                    type: "POST",
                    url: addApproverDetailsURL,
                    data: model
                }).done(function (response) {
                    GetApproverFlowList();
                    return;
                }).fail(function (error) {
                    console.log("Src:ProcessGuideline.js;method:gridApproverList-updateItem;error:" + error);
                });
                
            },
            deleteItem: function (item) {
                $.ajax({
                    type: "POST",
                    url: deletePGLWorkflowURL,
                    data: { processguidlineworkflowId: item.ProcessguidlineworkflowId },
                    success: function (result) {
                        if (result == 1) {
                            alert("Deleted successfully");
                            GetUserCountlist(pglDetailId);
                        }
                        else {
                            alert("Deleted failed");
                        }
                    },
                    error: function (err) {
                        console.log("Src:ProcessGuideline.js;method:gridApproverList-deleteItem;error" + err + ".");
                    }
                });
            }
        },
        fields: [
            { type: "number", name: "ProcessguidlineworkflowId", title: "Id", visible: false },
            {
                type: "number", name: "ApproverLevel", title: "Level", min: 1, width: 100, validate: {
                    message: "Level should be greater than 1",
                    validator: function (value, item) {
                        var retVal = true;
                        if (value == undefined)
                            retVal = false;
                        else if (value < 1)
                            retVal = false;
                        return retVal;
                            
                            
                    }
                }
            },
          
            {
                type: "select", name: "UserId", title: "Approver", items: approverList, valueField: "ApproverId", textField: "ApproverName", validate: { message: "Approver is required", validator: function (value) { return value > 0; } }
            },
            
            { type: "select", name: "StatusId", title: "Status",width: 100, items: statusList, valueField: "StatusId", textField: "StatusName", validate: { message: "Status is required", validator: function (value) { return true; } } },
            {
                name: "SelectedDocument", type: "multiselect", width: 100, align: "center", items: documentList, textField: "DocumentName", title: "Attachments", validate: {
                    message: "Attachment is required",
                    validator: function (value, item) {
                        return item.SelectedDocument.length > 0;
                    }
                }
            },
            
            {
               name: "ApproveFlag", type: "checkbox", title: "Approve", sorting: false, width:100, 
            },
            {
                name: "RejectFlag", type: "checkbox", title: "Reject", sorting: false, width: 100
            },
            {
                name: "ClarifyFlag", type: "checkbox", title: "Clarify", sorting: false, width: 100
            },
            {
                name: "MarkFlag", type: "checkbox", title: "Mark", sorting: false, width: 100
            },
            //{
            //    type: "select", title: "Action", items: approverCategory, valueField: "ApproverFlagId", textField: "ApproverFlagname", name: "ApproverList",
             
            //    insertTemplate: function (value, item) {
                 
            //        var $select = jsGrid.fields.select.prototype.insertTemplate.call(this);
                  
            //        $select.addClass('form-control selectpicker');
            //        $select.attr({ multiple: "multiple", id: "appSelectIndex"});
                  
            //        setTimeout(function () {
                       
            //            $select.selectpicker('refresh');
            //            $select.selectpicker('render');
                        
            //        });
            //        $select.val(0);
                    
            //        return $select;
                    
            //    },
                
            //    editTemplate: function (value, item) {
            //        var $select = jsGrid.fields.select.prototype.editTemplate.call(this);
            //        $select.addClass('form-control selectpicker');
                   
            //        $select.attr({ multiple: "multiple", id: "appSelectIndex"});

            //        setTimeout(function () {
            //            $select.selectpicker({

            //            });
            //            //$select.selectpicker('refresh');
            //            //$select.selectpicker('render');

            //        });
            //        console.log(item.ApproverList);
            //        $.each(item.ApproverList, function (index, itemData) {
            //            if (itemData != 0) {
            //                console.log(itemData);
            //                $select.val(itemData);
            //            }
            //        });
            //        return $select;
            //    },
            //    itemTemplate: function (value, item) {
            //        var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                   
            //        var $customSelect = $("<select>").addClass("form-control")
                            
            //            .attr("multiple", "multiple").prop("selectedIndex", 0)
                   
            //        //console.log($customSelect);
            //        $.each(approverCategory, function (index, itemData) {
            //            $customSelect.append($('<option/>', {
            //                value: itemData.ApproverFlagId,
            //                text: itemData.ApproverFlagname
            //            }));
                        
            //        });
                    
            //        return $result.add($customSelect);
                    
            //    }
            //},
          
             
            

            {
                type: "control",
                editButton: true,
                deleteButton: true,
                width: 100,
                headerTemplate: function (args) {
                    var grid = this._grid;
                    var isInserting = grid.inserting;
                    var $button = $("<input>").attr("type", "button")
                        .addClass([this.buttonClass, this.modeButtonClass, this.insertModeButtonClass].join(" "))
                        .on("click", function () {
                            if (pglId != 0) {
                                isInserting = !isInserting;
                                grid.option("inserting", isInserting);
                            }
                            else {
                                alert("Please save process in header");
                            }

                        });
                    return $button;
                },
                
            }
        ],
        onRefreshed: function () {
            var $gridData = $("#gridApproverList .jsgrid-grid-body tbody");
            $gridData.sortable({
                update: function (e, ui) {
                    // array of indexes
                    var clientIndexRegExp = /\s*client-(\d+)\s*/;
                    var indexes = $.map($gridData.sortable("toArray", { attribute: "class" }), function (classes) {
                        return clientIndexRegExp.exec(classes)[1];
                    });
                    alert("Reordered indexes: " + indexes.join(", "));
                    // arrays of items
                    var items = $.map($gridData.find("tr"), function (row) {
                        return $(row).data("JSGridItem");
                    });
                    console && console.log("Reordered items", items);
                }
            });
        }
    });
    
    $('#btnAddNew').off("click").on("click", function () {
        $("#btnAddNew,#btnFilterPGL").css('display', 'none');
        $("#btnClosePGL").css('display', 'block');
        $('#btnSave').attr('disabled', false);
        pglId = 0;
        $('#selectFunction,#selectFlow').prop('selectedIndex', 0);
        $('#txtProcess').val('');
        $('#divOuterPGL').css('display', 'none');
        $('#divInnerPGL').css('display', 'block');
        $("#gridProcessFlow").jsGrid({ data: {} });
        $("#gridUnassigneduser").jsGrid({ data: {} });
        $("#gridAssignedUser").jsGrid({ data: {} });
        $("#gridApproverList").jsGrid({ data: {} });
        $('#numUsersCount').text(0);
        $('#numApproverCount').text(0);
        unselectUser = [];
        selectUser = [];
        $('#spanAssignedUser,#spanApprover').html("Assigned Users :");
    });

    $('#btnClosePGL').on("click", function () {
        $('#divOuterPGL').css('display', 'block');
        $('#divInnerPGL').css('display', 'none');
        $("#btnAddNew,#btnFilterPGL").css('display', 'block');
        $("#btnClosePGL").css('display', 'none');
    });

    $('#btnUnmapUser').off("click").on("click", function () {
        var input = [];
        $.each(unselectUser, function (index, value) {
            input.push({ ProcessGuidelineDetailId: pglDetailId, UserId: value });
        });
        if (input.length > 0) {
            $.ajax({
                type: "POST",
                url: unmapProcessflowUserURL,
                data: JSON.stringify(input),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response.result > 0) {
                    GetProcessFlowList();
                    GetUserProcessflowList();
                    GetUserCountlist(pglDetailId);
                    unselectUser = [];
                }
                else if (response.result == -1) {
                    console.log("Src:ProcessGuideline.js;method:btnUnmapUser;error");
                }
            });
        }
        else {
            alert("Please select atleast one users");
        }
        return false;
    });

    $('#btnSave').off("click").on("click", function () {
        var errorMsg = ValidationOnSave();
      
        if (errorMsg.length==0) {
            var input = { ProcessGuidelineId: pglId, FunctionId: $('#selectFunction').val(), ProcessName: $('#txtProcess').val() };
            
            $.ajax({
                type: "POST",
                url: insertProcessGuidelineURL,
                data: { model: input },
                //contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                   var msg = result.result;
                   if (msg >=0) {
                        var mode = pglId == 0 ? "Add" : "Update";
                        pglId = result.result;
                        Search();
                    }
                    else if (msg == -2)
                    {
                        $('#alert').html("This Process Name already Exits try Another Name");
                        $('#Validation').modal('toggle');
                    }
                    else if (msg == -1)
                    {
                        $('#FailedAlert').html("Somthing Went to wrong Please Contact Admin!");
                        $('#Failed').modal('toggle');
                    }
                    if (mode == "Add") {
                        $('#btnSave').attr('disabled', true);
                        $('#alertSuccess').html("Saved successfully");
                        $('#Success').modal('toggle');

                    }
                    else if(mode=="Update") {
                        $('#btnSave').attr('disabled', true);
                        $('#alertSuccess').html("Updated successfully");
                        $('#Success').modal('toggle');
                    }
                },
                error: function (errMsg) {
                    alert(errMsg);
                }
            });
        }
        else {
            $('#alert').html(errorMsg);
            $('#Validation').modal('toggle');
        }
    });

    $('#btnResetSave').off("click").on("click", function () {
        pglId = 0;
        $('#selectFunction').prop('selectedIndex', 0);
        $('#txtProcess').val('');
    });

    $('#btnSearchPGL').off('click').on('click', function () {
        Search();
    });

    $('#btnResetSrchPGL').off('click').on('click', function () {
        $('#selectFunctionSrch').prop('selectedIndex', 0);
        $('#txtProcessSrch').val('');
        Search();
    });

    $('#btnMapUser').off('click').on('click', function () {
      var input = [];
        $.each(selectUser, function (index, value) {
            input.push({ ProcessGuidelineDetailId: pglDetailId, UserId: value });
        });
        if (input.length > 0) {
            $.ajax({
                type: "POST",
                url: mapProcessflowUserURL,
                data: JSON.stringify(input),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response.result > 0) {
                    GetProcessFlowList();
                    GetUserProcessflowList();
                    GetFlowListGrid(pglDetailId);
                    selectUser = [];
                    $('#myModal').modal('hide');
                }
                else if (response.result == -1) {
                    console.log("Src:ProcessGuideline.js;method:btnMapUser;error");
                }
            });
        }
        else {
            alert("Please select atleast one users");
        }
        return false;
    });

    $('a[name="btnClosePopupPGL"],button[name="btnClosePopupPGL"]').click(function () {
        $('#popupFilterPGL').css("display", "none");
    });

    $('#btnFilterPGL').off("click").on("click", function () {
        $('#popupFilterPGL').css("display", "Block");
    });

    function Search() {
        var input = { functionId: $('#selectFunctionSrch').val(), processName: $('#txtProcessSrch').val() };
        $.ajax({
            type: "Get",
            url: getProcessGuideLineListURL,
            data: input,
            dataType: "json",
            success: function (result) {
                $("#gridProcessGuidelineSearch").jsGrid({ data: result });
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:Search;error" + err);
            }
        });
    };
    var GetProcessFlowList = function () {
        $.ajax({
            type: "GET",
            url: getProcessFlowDetailsURL,
            data: { processGuidelineId: pglId },
            success: function (result) {
                dataProcessFlow = result;
                $("#gridProcessFlow").jsGrid({ data: result });
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:GetProcessFlowList;error" + err);
            }
        });
    };
    var GetApproverFlowList = function () {
        $.ajax({
            type: "GET",
            url: getAllApproverListURL,
            data: { processheaderid: pglId, processDetailId: pglDetailId },
            success: function (result) {
                dataProcessFlow = result;
                $("#gridApproverList").jsGrid({ data: result });
                
                
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:GetApproverFlowList;error" + err);
            }
        });
    };
    var GetUserProcessflowList = function () {
        
        $.ajax({
            type: "GET",
            url: getUnassignedUserDetailsURL,
            data: { processGuidelineDetailId: pglDetailId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var assingedRows = $.grep(result, function (n, i) {
                    return n.UserFlag == true;
                });
                var unAssingedRows = $.grep(result, function (n, i) {
                    return n.UserFlag == false;
                });
                $("#gridAssignedUser").jsGrid({ data: assingedRows });
                $("#gridUnassigneduser").jsGrid({ data: unAssingedRows });
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:GetUserProcessflowList;error" + err + ".");
            }
        });
    };
    var ValidationOnSave = function () {
        var msg = "";
        if ($.trim($('#txtProcess').val()) == "") {
            msg = "Process Guide Name";
            return msg;
        }
        else if ($('#selectFunction').val() == 0) {
            msg = "Select any one Function name";
            return msg;
        }
        else
        {
            return msg;
        }
    };
    $('#btnAddFlow').on('click', function () {
        var errorMsg = ValidationOnSave();
        if (errorMsg.length == 0) {
            var flowname = $('#txtAddFlow').val();
           if (flowname != "") {
                var input = {
                    ProcessGuidelineid: pglId,
                    ProcessGuidelineDetailId: pglDetailId,
                    FlowTitle: flowname
                };
                $.ajax({
                    type: "POST",
                    url: addProcessFlowDetailsURL,
                    data: input,
                }).done(function (response) {
                    var msg = response.result
                    if (msg > 0) {
                        GetFlowList();
                        $('#txtAddFlow').val('');
                        $('#alertSuccess').html("Flow Add SuccessFully");
                        $('#Success').modal('toggle');
                        $('#addFlowModal').modal('hide');
                        return;
                    }
                    else if (msg == -2)
                    {
                        $('#txtAddFlow').val('');
                        $('#alert').html("This Flow name already Exits try Another Name");
                        $('#Validation').modal('toggle');
                    }
                    else if (msg == -1)
                    {
                        $('#FailedAlert').html("Somthing Went to wrong Please Contact Admin!");
                        $('#Failed').modal('toggle');
                    }
                }).fail(function (error) {
                    console.log("Src:ProcessGuideline.js;method:gridProcessFlow-insertItem;error:" + error);
                });
            }
            else
            {
                $('#alert').html("Please Enter Flow name");
                $('#Validation').modal('toggle');
            }
        }
        else
        {
            $('#alert').html(errorMsg);
            $('#Validation').modal('toggle');
        }
    });
    function GetFlowList()
    {
        
        $.ajax({
            type: "GET",
            url: getProcessFlowDetailsURL,
            data: { processGuidelineId: pglId },
            success: function (result) {
                var flowindex = "<option value='0'>Select Flow</option>";
                $("#selectFlow").html(flowindex).show();
                $.each(result, function (i, flow) {
                    $("#selectFlow").append(
                        $('<option></option>').val(flow.ProcessGuidelineDetailId).html(flow.FlowTitle));
                });
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:GetProcessFlowList;error" + err);
            }
        });
    }
   
    $("#selectFlow").on('change', function () {
        pglDetailId = $("#selectFlow").val();
       
        GetFlowListGrid(pglDetailId);
           
            //GetUserProcessflowList();
    });
    function GetFlowListGrid(pglDetailId)
    {
        if (pglDetailId != 0) {
            //pglDetailId = Flowid;
            $.ajax({
                type: "GET",
                url: getUnassignedUserDetailsURL,
                data: { processGuidelineDetailId: pglDetailId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var assingedRows = $.grep(result, function (n, i) {
                        return n.UserFlag == true;
                    });
                    var unAssingedRows = $.grep(result, function (n, i) {
                        return n.UserFlag == false;
                    });
                    $("#gridAssignedUser").jsGrid({ data: assingedRows });
                    $("#gridUnassigneduser").jsGrid({ data: unAssingedRows });
                    unadb = unAssingedRows;
                    assuser = assingedRows;
                    GetUserCountlist(pglDetailId);
                    GetApproverFlowLists(pglDetailId);
                },
                error: function (err) {
                    console.log("Src:ProcessGuideline.js;method:GetUserProcessflowList;error" + err + ".");
                }
            });
        }
        else {
            $("#gridAssignedUser").jsGrid({ data: {} });
            $("#gridUnassigneduser").jsGrid({ data: {} });
            $('#numUsersCount').text(0);
            $('#spanAssignedUser,#spanApprover').html("");
            $('#numApproverCount').text(0);
        }
    }
    function GetUserCountlist(pglDetailId)
    {
        if (pglDetailId != 0) {
            $.ajax({
                type: "GET",
                url: getUserCount,
                data: { pgDetailId: pglDetailId },
                success: function (result) {
                    $('#numUsersCount').text(result.UserCount);
                    $('#spanAssignedUser,#spanApprover').html(result.FlowTitle);
                    $('#numApproverCount').text(result.ApproverCount);
                },
                error: function (err) {
                    console.log("Src:ProcessGuideline.js;method:GetProcessFlowList;error" + err);
                }
            });
        }
        else
        {
            $('#numUsersCount').text(0);
            $('#numApproverCount').text(0);
        }
    }
    var GetApproverFlowLists = function (flowid) {
        $.ajax({
            type: "GET",
            url: getAllApproverListURL,
            data: { processheaderid: pglId, processDetailId: flowid },
            success: function (result) {
                dataProcessFlow = result;
                $("#gridApproverList").jsGrid({ data: result });
            },
            error: function (err) {
                console.log("Src:ProcessGuideline.js;method:GetApproverFlowList;error" + err);
            }
        });
    };
});
