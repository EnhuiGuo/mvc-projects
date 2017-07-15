$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#customerLink').addClass("current-link");

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
    }

    var customerOrdersTable = $('#customerOrders').DataTable({
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
               }
        ],
        "autoWidth": false,
        "orderable": false,
        "pageLength": 20,
        "lengthChange": false,
        "order": [[8, 'desc'],[3, 'asc']], 
        select: { style: 'os', selector: '.select' },
        "data": customerOrders,
        "columns": [
        { "data": "CustomerOrderId", "title": "CustomerOrderId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "OrderTypeDescription", "title": "Order Type", "class": "center" },
        { "data": "PONumber", "title": "PO Number", "class": "center" },
        { "data": "PODate", "name": "PODate", "title": "Date", "class": "center", render: dateFlag },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "DueDate", "name": "DueDate", "title": "Receipt Date", "class": "center", render: dateFlag },
        { "data": null, defaultContent: "", "title": "Status", "class": "center", "render": statusFlag },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "CreatedDate", "visible": false, render: dateFlag },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 9,
            "title": "View",
            "width": "8%", "targets": 9,
            "data": null,
            "defaultContent":
                "<span id='viewBtn' class='glyphicon glyphicon-info-sign glyphicon-large'></span>"
        }]
    });

    $('#addOrder').on('click', function () {
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/CreateCustomerOrder';
    });

    $('#customerOrders tbody').on('click', '#viewBtn', function () {
        var customerOrder = customerOrdersTable.row($(this).parents('tr')).data();
        var childData = customerOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customerOrder != null) {
            data = customerOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/CustomerOrderDetail?customerOrderId=' + data.CustomerOrderId;
    });

    $('#printOpenCustomerOrdersBtn').click(function () {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/PurchaseOrder/_PrintOpenCustomerOrders",
            success: function (result) {

                $('#printOpenCustomerOrdersDiv').html('');
                $('#printOpenCustomerOrdersDiv').html(result);

                $('#openOrdersPrintModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    printOpen = function () {
        var foundryId = $('#foundryId').val();
        var customerId = $('#customerId').val();
        var orderTypeId = $('#orderTypeId').val();

        if (foundryId == "--Select Foundry--")
            foundryId = '';

        if (customerId == "--Select Customer--")
            customerId = '';

        if (orderTypeId == "--Select Order Type--")
            orderTypeId = '';

        window.location.href = '/SouthlandMetals/Operations/Report/OpenCustomerOrderReport?foundryId=' + foundryId + '&customerId=' + customerId + '&orderTypeId=' + orderTypeId + '';
    }

    $('#printUnattachedCustomerOrdersBtn').click(function () {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/PurchaseOrder/_PrintUnattachedCustomerOrders",
            success: function (result) {

                $('#printUnattachedCustomerOrdersDiv').html('');
                $('#printUnattachedCustomerOrdersDiv').html(result);

                $('#unattachedOrdersPrintModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    printUnattached = function () {
        var foundryId = $('#foundryId').val();
        var customerId = $('#customerId').val();
        var orderTypeId = $('#orderTypeId').val();

        if (foundryId == "--Select Foundry--")
            foundryId = '';

        if (customerId == "--Select Customer--")
            customerId = '';

        if (orderTypeId == "--Select Order Type--")
            orderTypeId = '';

        window.location.href = '/SouthlandMetals/Operations/Report/UnattachedCustomerOrderReport?foundryId=' + foundryId + '&customerId=' + customerId + '&orderTypeId=' + orderTypeId + '';
    }

    function showAll() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(customerOrders, function (i, order) {
                if (order.IsOpen) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(customerOrders, function (i, order) {
                if (order.IsHold) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(customerOrders, function (i, order) {
                if (order.IsComplete) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(customerOrders, function (i, order) {
                if (order.IsCanceled) {
                    orders.push(order);
                }
            });
        }
        else {
            orders = customerOrders;
        }
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();

    };

    function showSample() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open")
        {
            $.each(customerOrders, function (i, order) {
                if (order.IsOpen && order.IsSample)
                {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold")
        {
            $.each(customerOrders, function (i, order) {
                if (order.IsHold && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(customerOrders, function (i, order) {
                if (order.IsComplete && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(customerOrders, function (i, order) {
                if (order.IsCanceled && order.IsSample) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(customerOrders, function (i, order) {
                if (order.IsSample) {
                    orders.push(order);
                }
            });
        }
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();

    };

    function showTooling() {

        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(customerOrders, function (i, order) {
                if (order.IsOpen && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(customerOrders, function (i, order) {
                if (order.IsHold && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(customerOrders, function (i, order) {
                if (order.IsComplete && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(customerOrders, function (i, order) {
                if (order.IsCanceled && order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(customerOrders, function (i, order) {
                if (order.IsTooling) {
                    orders.push(order);
                }
            });
        }
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();

    };

    function showProduction() {
        var orders = [];
        var state = $("input:radio:checked").val();
        if (state == "Open") {
            $.each(customerOrders, function (i, order) {
                if (order.IsOpen && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "On Hold") {
            $.each(customerOrders, function (i, order) {
                if (order.IsHold && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Completed") {
            $.each(customerOrders, function (i, order) {
                if (order.IsComplete && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else if (state == "Canceled") {
            $.each(customerOrders, function (i, order) {
                if (order.IsCanceled && order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        else {
            $.each(customerOrders, function (i, order) {
                if (order.IsProduction) {
                    orders.push(order);
                }
            });
        }
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    };

    function showOpen()
    {
        var orders = [];
        $.each(customerOrders, function (i, order) {
            if (order.IsOpen) {
                orders.push(order);
            }
        });
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    }

    function showHold()
    {
        var orders = [];
        $.each(customerOrders, function (i, order) {
            if (order.IsHold) {
                orders.push(order);
            }
        });
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    }

    function showCompleted()
    {
        var orders = [];
        $.each(customerOrders, function (i, order) {
            if (order.IsComplete) {
                orders.push(order);
            }
        });
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    }

    function showCanceled()
    {
        var orders = [];
        $.each(customerOrders, function (i, order) {
            if (order.IsCanceled) {
                orders.push(order);
            }
        });
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    }

    function showConfirmed()
    {
        var orders = [];
        $.each(customerOrders, function (i, order) {
            if (order.IsCanceled) {
                orders.push(order);
            }
        });
        $('#customerOrders').DataTable().clear().draw();
        $('#customerOrders').DataTable().rows.add(orders); // Add new data
        $('#customerOrders').DataTable().columns.adjust().draw();
    }

    $('#open').prop("checked", true);
    showOpen();
});