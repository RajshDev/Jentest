$(function () {

    var getOutwardURL = 'GetOutwardDetails'

    $("#OutwardList").jsGrid({
        autoload: true,
        width: 1000,
        paging: true,
        pageIndex: 1,
        async: false,
        pageSize: 5,
        editing: false,
        text: false,
        title: "Action",
        //selecting: true,

        fields: [
            { type: "number", name: "slNo",          title: "S.No",           editing: false },
            { type: "number", name: "TapalId",       title: "TapalId",        visible: false },
            { type: "text",   name: "ReceiptDt",     title: "Receipt Date",   editing: false },
            { type: "text",   name: "TapalType",     title: "Tapal Type",     editing: false },
            { type: "text",   name: "SenderDetails", title: "Sender Details", editing: false },
            { type: "text",   name: "InwardDate",    title: "Inward Date",    editing: false },
            { type: "text",   name: "Department",    title: "Department",     editing: false },
            { type: "text",   name: "User",          title: "User",           editing: false },
            { type: "text",   name: "Remarks",       title: "Remarks",        editing: false },
            { type: "text",   name: "OutwardDate",   title: "Outward Date",   editing: false },
            //{ type: "control" }

        ],
    });


    /*Get outward details*/

    $.ajax({
        type: "GET",
        url: getOutwardURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            dataProcessFlow = result;
            $("#OutwardList").jsGrid({ data: result });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
});