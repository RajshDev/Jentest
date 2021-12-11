var loadItemURL = 'GetSRBItemForDeactivate'
var db;
$(function () {

    var UpdateStatusURL = 'SRBItemDeactivate';
    var statusList;

    $("#SRBDeactivationList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (ow) {

                    return (!filter.ItemName || ow.ItemName.toLowerCase().indexOf(filter.ItemName.toLowerCase()) > -1)
                    && (!filter.ItemCategory || ow.ItemCategory.toLowerCase().indexOf(filter.ItemCategory.toLowerCase()) > -1)
                    && (!filter.ItemNumber || ow.ItemNumber.toLowerCase().indexOf(filter.ItemNumber.toLowerCase()) > -1)

                });
            }

        },

        fields: [
            { name: "slNo", title: "S.No", editing: false, width: 50 },
            { type: "number", name: "SRBDetailId", title: "Id", visible: false },
             { type: "text", name: "ItemCategory", title: "Category", editing: false },
            { type: "text", name: "ItemName", title: "Item Name", editing: false },
            { type: "text", name: "ItemNumber", title: "Item Number", editing: false },
            { name: "Quantity", title: "Quantity", editing: false },
            { name: "ItemValue", title: "Item Value", editing: false },
            { name: "Status", title: "Status", editing: false },
            {
                type: "control", deleteButton: false, editButton: false, width: 120, title: "Action",

                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Active") {
                        statusList = [{ id: "", name: "Select Action" }, { id: "Scrap", name: "Dispose" }, { id: "Buyback", name: "Buyback" }]
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
                        model = {
                            SRBDetailId: item.SRBDetailId,
                            Status: selVal
                        }
                        if (selVal == "Scrap") {
                            var choice = confirm("Do you want to dispose this item. You can't undo this action.");
                            if (choice === true) {
                                $.ajax({
                                    type: "POST",
                                    url: UpdateStatusURL,
                                    data: model,
                                    success: function (result) {
                                        if (result == true) {
                                            $('#alertSuccess').html("Item has been disposed successfully.");
                                            $('#Success').modal('toggle');
                                            loadDetails();
                                        } else if (result == false) {
                                            $('#FailedAlert').html("Something went wrong please contact administrator");
                                            $('#Failed').modal('toggle');
                                            loadDetails();
                                        }
                                    },
                                    error: function (err) {
                                        console.log("error1 : " + err);
                                    }
                                });
                            }
                        } else if (selVal == "Buyback") {
                            $.ajax({
                                type: "POST",
                                url: UpdateStatusURL,
                                data: model,
                                success: function (result) {
                                    $('#popupBuyback').html(result);
                                    $('#BuybackModal').modal('toggle');
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
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
    $("#SRBDeactivationList").jsGrid("option", "filtering", false);
    loadDetails();

});
var loadDetails = function loadDetails() {
    $.ajax({
        type: "GET",
        url: loadItemURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#SRBDeactivationList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
};