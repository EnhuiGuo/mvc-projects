$(document).ready(function () {

    var openPackingListsTable = $('#openPackingLists').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": openPackingLists,
        "columns": [
        { "data": "PackingListId", "title": "PackingListId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "Created", "class": "center", render: dateFlag },
        { "data": "CustomerAddress", "title": "Ship To", "class": "center" },
        { "data": "CarrierName", "title": "Carrier", "class": "center" },
        { "data": "ShipDate", "name": "ShipDate", "title": "Ship Date", "class": "center", render: dateFlag },
        { "title": "View", "class": "center" },
        { "title": "Close", "class": "center" }
        ],
        "columnDefs": [
            {
                "targets": 6,
                "title": "View",
                "width": "8%", "targets": 6,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewPackingListBtn'></span>"
            },
            {
                "targets": 7,
                "title": "Delete",
                "width": "8%", "targets": 7,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-remove glyphicon-large' id='closePackingListBtn'></span>"
            }
        ]
    });

    var onHoldProjectsTable = $('#onHoldProjects').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": onHoldProjects,
        "columns": [
        { "data": "ProjectId", "title": "ProjectId", "visible": false },
        { "data": "ProjectName", "title": "Name", "class": "center" },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "class": "center", render: dateFlag },
        { "title": "Hold Notes", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 3,
                 "title": "Notes",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='projectNotesBtn'></span>"
             },
            {
                "targets": 4,
                "title": "View",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewProjectBtn'></span>"
            }
        ]
    });

    var onHoldRfqsTable = $('#onHoldRfqs').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": onHoldRfqs,
        "columns": [
        { "data": "RfqId", "title": "RfqId", "visible": false },
        { "data": "RfqNumber", "title": "Number", "class": "center" },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "class": "center", render: dateFlag },
        { "title": "Hold Notes", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 3,
                 "title": "Notes",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='rfqNotesBtn'></span>"
             },
            {
                "targets": 4,
                "title": "View",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewRfqBtn'></span>"
            }
        ]
    });

    var onHoldQuotesTable = $('#onHoldQuotes').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": onHoldQuotes,
        "columns": [
        { "data": "QuoteId", "title": "QuoteId", "visible": false },
        { "data": "QuoteNumber", "title": "Number", "class": "center" },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "class": "center", render: dateFlag },
        { "title": "Hold Notes", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 3,
                 "title": "Notes",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='quoteNotesBtn'></span>"
             },
            {
                "targets": 4,
                "title": "View",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewQuoteBtn'></span>"
            }
        ]
    });

    var onHoldCustomerOrdersTable = $('#onHoldCustomerOrders').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": onHoldCustomerOrders,
        "columns": [
        { "data": "CustomerOrderId", "title": "CustomerOrderId", "visible": false },
        { "data": "PONumber", "title": "PO Number", "class": "center" },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "class": "center", render: dateFlag },
        { "title": "Hold Notes", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 3,
                 "title": "Notes",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='customerOrderNotesBtn'></span>"
             },
            {
                "targets": 4,
                "title": "View",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewCustomerOrderBtn'></span>"
            }
        ]
    });

    var onHoldFoundryOrdersTable = $('#onHoldFoundryOrders').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": true,
        "paging": false,
        "info": false,
        "scrollY": 375,
        "scrollX": true,
        "scrollCollapse": true,
        "order": [[2, 'desc'], [1, 'asc']],
        "data": onHoldFoundryOrders,
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "class": "center", render: dateFlag },
        { "title": "Hold Notes", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 3,
                 "title": "Notes",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='foundryOrderNotesBtn'></span>"
             },
            {
                "targets": 4,
                "title": "View",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewFoundryOrderBtn'></span>"
            }
        ]
    });


    $('#openPackingLists tbody').on('click', '#closePackingListBtn', function () {
        var packingList = openPackingListsTable.row($(this).parents('tr')).data();
        var childData = openPackingListsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (packingList != null) {
            data = packingList;
        }
        else {
            data = childData;
        }

        $.confirm({
            text: 'Are you sure you want to close this Packing List?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'POST',
                    url: "/SouthlandMetals/Operations/Warehouse/ClosePackingList",
                    data: { "packingListId": data.PackingListId },
                    complete: function () {

                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });
            },
            cancel: function (button) {

            }
        });
    });

    $('#openPackingLists tbody').on('click', '#viewPackingListBtn', function () {
        var packingList = openPackingListsTable.row($(this).parents('tr')).data();
        var childData = openPackingListsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (packingList != null) {
            data = packingList;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Warehouse/PackingListDetail?packingListId=" + data.PackingListId, target = "_self");
    });

    $('#onHoldProjects tbody').on('click', '#projectNotesBtn', function () {

        var project = onHoldProjectsTable.row($(this).parents('tr')).data();
        var childData = onHoldProjectsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (project != null) {
            data = project;
        }
        else {
            data = childData;
        }

        var holdExpirationDate = dateFlag(data.HoldExpirationDate);

        _ViewHoldNotes(data.HoldNotes, holdExpirationDate);

    });

    $('#onHoldProjects tbody').on('click', '#viewProjectBtn', function () {
        var project = onHoldProjectsTable.row($(this).parents('tr')).data();
        var childData = onHoldProjectsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (project != null) {
            data = project;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Project/Summary?projectId=" + data.ProjectId, target = "_self");
    });

    $('#onHoldRfqs tbody').on('click', '#rfqNotesBtn', function () {
        var rfq = onHoldRfqsTable.row($(this).parents('tr')).data();
        var childData = onHoldRfqsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfq != null) {
            data = rfq;
        }
        else {
            data = childData;
        }

        var holdExpirationDate = dateFlag(data.HoldExpirationDate);

        _ViewHoldNotes(data.HoldNotes, holdExpirationDate);

    });

    $('#onHoldRfqs tbody').on('click', '#viewRfqBtn', function () {
        var rfq = onHoldRfqsTable.row($(this).parents('tr')).data();
        var childData = onHoldRfqsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfq != null) {
            data = rfq;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/Rfq/Detail?rfqId=' + data.RfqId + '';
    });

    $('#onHoldQuotes tbody').on('click', '#quoteNotesBtn', function () {
        var quote = onHoldQuotesTable.row($(this).parents('tr')).data();
        var childData = onHoldQuotesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (quote != null) {
            data = quote;
        }
        else {
            data = childData;
        }

        var holdExpirationDate = dateFlag(data.HoldExpirationDate);

        _ViewHoldNotes(data.HoldNotes, holdExpirationDate);

    });

    $('#onHoldQuotes tbody').on('click', '#viewQuoteBtn', function () {
        var quote = onHoldQuotesTable.row($(this).parents('tr')).data();
        var childData = onHoldQuotesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (quote != null) {
            data = quote;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/Quote/Detail?quoteId=' + data.QuoteId;
    });

    $('#onHoldCustomerOrders tbody').on('click', '#customerOrderNotesBtn', function () {
        var customerOrder = onHoldCustomerOrdersTable.row($(this).parents('tr')).data();
        var childData = onHoldCustomerOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customerOrder != null) {
            data = customerOrder;
        }
        else {
            data = childData;
        }

        var holdExpirationDate = dateFlag(data.HoldExpirationDate);

        _ViewHoldNotes(data.HoldNotes, holdExpirationDate);

    });

    $('#onHoldCustomerOrders tbody').on('click', '#viewCustomerOrderBtn', function () {
        var customerOrder = onHoldCustomerOrdersTable.row($(this).parents('tr')).data();
        var childData = onHoldCustomerOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customerOrder != null) {
            data = customerOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/CustomerOrderDetail?customerOrderId=' + data.CustomerOrderId;
    });

    $('#onHoldFoundryOrders tbody').on('click', '#foundryOrderNotesBtn', function () {
        var foundryOrder = onHoldFoundryOrdersTable.row($(this).parents('tr')).data();
        var childData = onHoldFoundryOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (foundryOrder != null) {
            data = foundryOrder;
        }
        else {
            data = childData;
        }

        var holdExpirationDate = dateFlag(data.HoldExpirationDate);

        _ViewHoldNotes(data.HoldNotes, holdExpirationDate);

    });

    $('#onHoldFoundryOrders tbody').on('click', '#viewFoundryOrderBtn', function () {
        var foundryOrder = onHoldFoundryOrdersTable.row($(this).parents('tr')).data();
        var childData = onHoldFoundryOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (foundryOrder != null) {
            data = foundryOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + data.FoundryOrderId;
    });

    function _ViewHoldNotes(holdNotes, holdExpirationDate) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_ViewHoldNotes",
            data: {
                "HoldNotes": holdNotes,
                "HoldExpirationDateStr": holdExpirationDate
            },
            contentType: "application/json",
            success: function (result) {

                $('#viewHoldNotesDiv').html('');
                $('#viewHoldNotesDiv').html(result);

                $('#viewHoldNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});


$(document).ready(function () {

    // Reference the auto-generated proxy for the hub.  

    var notification = $.connection.notificationHub;

    // Client side method for receiving the list of notifications on the connected event from the server
    notification.client.refreshNotification = function (data) {
        $("#notificationTab").empty();

        $('#noti_Counter').empty();

        $('#noti_Counter')
          .css({ opacity: 0 })
          .text(data.length)              // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
          .animate({ top: '11px', opacity: 1 }, 500);


        $("#cntNotifications").text(data.length);
        for (var i = 0; i < data.length; i++) {
            $("#notificationTab").append("<tr> <td> " + data[i].NotificationId + "</td> <td>" + data[i].Text + "</td> <td>" + data[i].CreatedDate + "</td></tr>");
        }
    }

    //Client side method which will be invoked from the Global.asax.cs file. 
    notification.client.addLatestNotification = function (data) {
        $("#cntNotifications").text($("#cntNotifications").text() + 1);
        $("#notificationTab").append("<tr> <td> " + data.NotificationId + "</td> <td>" + data.Text + "</td> <td>" + data.CreatedDate + "</td></tr>");
    }

    // Start the connection.
    $.connection.hub.start().done(function () {

        //When the send button is clicked get the text and user name and send it to server. 
        $("#btnSend").click(function () {
            notification.server.sendNotification($("#text").val(), $("#userName").val());
        });
    });
});