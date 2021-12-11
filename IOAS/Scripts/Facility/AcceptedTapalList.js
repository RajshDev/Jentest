var getAcceptedURL = 'GetAcceptedTapalDetails'
var dbAcceptTapal;
$(function () {
    var tapalDetails = 'PopupTapalDetails';
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
            this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), dateFormat: 'dd-MM-yy', changeYear: true });
            this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), dateFormat: 'dd-MM-yy', changeYear: true });
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
    $("#AcceptedTapalList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        pageLoading: true,
        autoload: true,
        editing: false,
        filtering: true,
        // selecting: true,
        controller: {

            loadData: function (filter) {
                //return $.grep(dbAcceptTapal, function (ow) {
                //    return (!filter.TapalNo || ow.TapalNo.toLowerCase().indexOf(filter.TapalNo.toLowerCase()) > -1)
                //         &&(!filter.TapalType || ow.TapalType.toLowerCase().indexOf(filter.TapalType.toLowerCase()) > -1)
                //         &&(!filter.SenderDetails || ow.SenderDetails.toLowerCase().indexOf(filter.SenderDetails.toLowerCase()) > -1)
                //         &&(!filter.Department || ow.Department.toLowerCase().indexOf(filter.Department.toLowerCase()) > -1)
                //         &&(!filter.User || ow.User.toLowerCase().indexOf(filter.User.toLowerCase()) > -1);
                //});
                var deferred = $.Deferred();
                $.ajax({
                    type: "post",
                    url: getAcceptedURL,
                    data: filter,
                    dataType: "json",
                    success: function (datas) {
                        var da = {
                            data: datas.Data,
                            itemsCount: datas.TotalRecords
                        }

                        deferred.resolve(da);
                    }
                });
                return deferred.promise();
            }

        },

        fields: [
           { name: "slNo", title: "S.No", editing: false, width: 70 },
            {  name: "TapalId", title: "TapalId", visible: false },
            {  name: "CreateUserId", title: "Create UserId", visible: false },
              { type: "text", name: "TapalNo", title: "Tapal No", editing: false, width: 150 },
            {  name: "TapalType", title: "Tapal Type", editing: false },
            {  name: "SenderDetails", title: "Sender Details",type: "text" },
            //{ type: "text", name: "InwardDate", title: "Inward Date", editing: false },
            { type: "text", name: "Department", title: "Department", editing: false, width: 130 },
            { type: "text", name: "User", title: "User", editing: false },
            //{ type: "text",   name: "Remarks",       title: "Remarks",        editing: false },
            { type: "date", name: "OutwardDate", title: "Outward Date", editing: false },
               {name: "strAction", title: "Action", editing: false, width: 110 },
               {
                   name: "DocDetail",
                   title: "Documents",width:120,
                   itemTemplate: function (value, item) {
                       var elementDiv = $("<div>");
                       elementDiv.attr("class", "ls-dts");
                       $.each(item.DocDetail, function (index, itemData) {
                           var $link = $("<a>").attr("class", "ion-document icn").attr("href", itemData.href).attr("target", "_blank").html('');
                           elementDiv.append($link);
                       });
                       return elementDiv;
                   }
               },
               {
                   type: "control", width: 50,deleteButton: false, editButton: false,
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
                        return $result.add($customButton);
                       //return $("<div>").append($customButton).append($customButtonEdit);
                   }
               },

        ],
    });
    $("#AcceptedTapalList").jsGrid("option", "filtering", false);
    //loadDetails();

});
//var loadDetails = function loadDetails() {
//    $.ajax({
//        type: "GET",
//        url: getAcceptedURL,
//        data: param = "",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (result) {
//            dbAcceptTapal = result;
//            $("#AcceptedTapalList").jsGrid({ data: dbAcceptTapal });
//        },
//        error: function (err) {
//            console.log("error : " + err);
//        }

//    });
//};