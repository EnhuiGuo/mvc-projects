$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#foundryLink').addClass("current-link");

    $('input[name=Status]').change(function () {
        if (this.value == 'All') {
            showAll();
        }
        else if (this.value == 'Open') {
            showOpen();
        }
        else if (this.value == 'On Hold') {
            showHold();
        }
        else if (this.value == 'Completed') {
            showCompleted();
        }
        else if (this.value == 'Canceled') {
            showCanceled();
        }
        else if (this.value == 'UnConfirmed') {
            showUnConfirmed();
        }
    });

    var statusFlag = function (data, type, row) {
        if (row.IsOpen) {
            return '<span>Open</span>';
        }
        else if (row.IsHold) {
            return '<span>On Hold</span>';
        }
        else if (row.IsCanceled) {
            return '<span>Canceled</span>';
        }
        else if (row.IsComplete) {
            return '<span>Complete</span>';
        }
        else {
            return '<span>N/A</span>';
        }
    }

    var confirmFlag = function (data, type, row) {
        if (row.IsConfirmed) {
            return '<span>Yes</span>';
        }
        else {
            return '<span>No</span>';
        }
    }

    var foundryOrdersTable = $('#foundryOrders').DataTable({
        dom: 'Bfrt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        buttons: [
               {
                   text: 'All',
                   className: 'btn btn-sm btn-default',
                   action: function () {
                       showAll();
                   }
               },
               {
                   text: 'Sample',
                   className: 'btn btn-sm btn-default',
                   action: function () {
                       showSample();
                   }
               },
               {
                   text: 'Tooling',
                   className: 'btn btn-sm btn-default',
                   action: function () {
                       showTooling();
                   }
               },
               {
                   text: 'Production',
                   className: 'btn btn-sm btn-default',
                   action: function () {
                       showProduction();
                   }
               },

        ],
        "autoWidth": false,
        "orderable": false,
        "order": [[9, 'desc'], [2, 'asc']],
        "pageLength": 20,
        "lengthChange": false,
        select: { style: 'os', selector: '.select' },
        "data": foundryOrders,
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderTypeDescription", "title": "Order Type", "class": "center" },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "DueDate", "name": "DueDate", "title": "Due Date", "class": "center", render: dateFlag },
        { "data": "ShipDate", "name": "ShipDate", "title": "Ship Date", "class": "center", render: dateFlag },
        { "data": "Status", "title": "Status", "class": "center", "render": statusFlag },
        { "data": "Confimed", "title": "Confirmed", "class": "center", "render": confirmFlag },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "CreatedDate", "visible": false, render: dateFlag },
	    { "title": "Ship Codes", "class": "center tubiao",},
        { "title": "View", "class": "center",}
        ],
        "columnDefs": [{
            "targets": 10,
            "title": "Ship Codes",
            "width": "8%", "targets": 10,
            "data": null,
            "defaultContent":
                "<span id='viewShipCode' class='glyphicon glyphicon-th-list glyphicon-large'></span>"
            }, {
            "targets": 11,
            "title": "View",
            "width": "8%", "targets": 11,
            "data": null,
            "defaultContent":
                "<span id='viewBtn' class='glyphicon glyphicon-info-sign glyphicon-large'></span>"
        }]
    });

    $('#addOrder').on('click', function () {
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/CreateFoundryOrder';
    });

    $('#foundryOrders tbody').on('click', '#viewShipCode', function () {
        var foundryOrder = foundryOrdersTable.row($(this).parents('tr')).data();
        var childData = foundryOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (foundryOrder != null) {
            data = foundryOrder;
        }
        else {
            data = childData;
        }
        _ViewShipCodes(data.FoundryOrderId);
    });

    $('#foundryOrders tbody').on('click', '#viewBtn', function () {
        var foundryOrder = foundryOrdersTable.row($(this).parents('tr')).data();
        var childData = foundryOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (foundryOrder != null) {
            data = foundryOrder;
        }
        else {
            data = childData;
        }

        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + data.FoundryOrderId;
    });

    $('#printOpenFoundryOrderBtn').click(function () {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/PurchaseOrder/_PrintOpenFoundryOrders",
            success: function (result) {

                $('#viewPrintDiv').html('');
                $('#viewPrintDiv').html(result);

                $('#viewOrdersPrintModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    print = function () {
        var foundryId = $('#foundryId').val();
        var customerId = $('#customerId').val();
        var orderTypeId = $('#orderTypeId').val();

        if (foundryId == "--Select Foundry--")
            foundryId = '';

        if (customerId == "--Select Customer--")
            customerId = '';

        if (orderTypeId == "--Select Order Type--")
            orderTypeId = '';

        window.location.href = '/SouthlandMetals/Operations/Report/OpenFoundryOrderReport?foundryId=' + foundryId + '&customerId=' + customerId + '&orderTypeId=' + orderTypeId + '';
    }

    function showAll() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsOpen) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsHold) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsComplete) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsCanceled) {
                    orders.push(order);
                }
            });
        }
        else {
            orders = foundryOrders;
        }
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    };

    function showSample() {

        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsOpen && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsHold && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsComplete && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsCanceled && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(foundryOrders, function (i, order) {
                if (order.IsSample) {
                    orders.push(order);
                }
            });
        }
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    };

    function showTooling() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsOpen && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsHold && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsComplete && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsCanceled && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(foundryOrders, function (i, order) {
                if (order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    };

    function showProduction() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsOpen && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsHold && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsComplete && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(foundryOrders, function (i, order) {
                if (order.IsCanceled && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(foundryOrders, function (i, order) {
                if (order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();

    };

    function showOpen() {
        var orders = [];
        $.each(foundryOrders, function (i, order) {
            if (order.IsOpen) {
                orders.push(order);
            }
        });
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    }

    function showHold() {
        var orders = [];
        $.each(foundryOrders, function (i, order) {
            if (order.IsHold) {
                orders.push(order);
            }
        });
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    }

    function showCompleted() {
        var orders = [];
        $.each(foundryOrders, function (i, order) {
            if (order.IsComplete) {
                orders.push(order);
            }
        });
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    }

    function showCanceled() {
        var orders = [];
        $.each(foundryOrders, function (i, order) {
            if (order.IsCanceled) {
                orders.push(order);
            }
        });
        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    }

    function showUnConfirmed() {
        var orders = [];
        $.each(foundryOrders, function (i, order) {
            if (!order.IsConfirmed) {
                orders.push(order);
            }
        });

        $('#foundryOrders').DataTable().clear().draw();
        $('#foundryOrders').DataTable().rows.add(orders); // Add new data
        $('#foundryOrders').DataTable().columns.adjust().draw();
    }

    $('#open').prop("checked", true);
    showOpen();
});

function _ViewShipCodes(foundryOrderId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/PurchaseOrder/_ViewShipCodes",
        data: { "foundryOrderId": foundryOrderId },
        success: function (result) {

            $('#viewShipCodesDiv').html('');
            $('#viewShipCodesDiv').html(result);

            $('#viewShipCodesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};