﻿$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#foundryLink').addClass("current-link");


    var customerOrderList = [];
    $.each(orderParts, function (i, orderPart) {
        if ($.inArray(orderPart.PONumber, customerOrderList) == -1)
            customerOrderList.push(orderPart.PONumber);
    });
    updateCustomerOrderTextArea();


    var shipCodeList = [];
    $.each(orderParts, function (i, orderPart) {
        if ($.inArray(orderPart.ShipCode, shipCodeList) == -1)
            shipCodeList.push(orderPart.ShipCode);
    });
    updateShipCodeTextArea();

    $('#isConfirmed').prop('checked', isConfirmed);

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show()
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }
    else if (isComplete) {
        $('#deleteIcon').hide();
        $('#deleteOrderBtn').hide();
    }

    $.each(orderParts, function (i, orderPart) {
        if (orderPart.EstArrivalDate != null) {
            orderPart.EstArrivalDate = new Date(parseInt(orderPart.EstArrivalDate.substr(6))).toLocaleDateString();
        }

        if (orderPart.ShipDate != null) {
            orderPart.ShipDate = new Date(parseInt(orderPart.ShipDate.substr(6))).toLocaleDateString();
        }
    });

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };

    var orderPartsTable = $('#orderParts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 475,
        "scrollCollapse": true,
        "order": [[6, 'desc'], [3, 'asc']],
        "data": orderParts,
        "columns": [
        { "data": "CustomerOrderPartId", "title": "PartId", "visible": false },
        { "data": "AvailableQuantity", "title": "Avail. Qty", "class": "center" },
        { "data": "FoundryOrderQuantity", "title": "Quantity", "class": "center" },
        { "data": "PartNumber", "title": "Part Number", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "PONumber", "title": "Customer Order", "class": "center" },
        { "data": "ShipCode", "title": "Ship Code", "class": "center" },
        { "data": "ShipCodeNotes", "title": "Ship Code Notes", "class": "center" },
        { "data": "EstArrivalDate", "name": "EstArrivalDate", "title": "Est Arrival Date", "class": "center", render: dateFlag },
        { "data": "ShipDate", "name": "ShipDate", "title": "Ship Date", "class": "center", render: dateFlag },
        { "data": "ReceiptDate", "name": "ReceiptDate", "title": "Receipt Date", "class": "center", render: dateFlag },
        { "data": "Cost", "title": "Unit Cost", "class": "center", "render": dollarFlag },
        ]
    });

    $(document).on('click', '#editOrderBtn', function () {
        if (isComplete) {
            $.confirm({
                text: 'This order is complete, are you sure you want to continue?',
                dialogClass: "modal-confirm",
                confirmButton: "Yes",
                confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                cancelButton: "No",
                cancelButtonClass: 'btn btn-sm btn-default',
                closeIcon: false,
                confirm: function (button) {
                    window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/EditFoundryOrder?foundryOrderId=' + foundryOrderId + '';
                },
                cancel: function (button) {

                }
            });
        }
        else {
            window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/EditFoundryOrder?foundryOrderId=' + foundryOrderId + '';
        }
    });

    $(document).on('click', '#printBtn', function () {
        event.preventDefault();
        window.location.href = '/SouthlandMetals/Operations/Report/FoundryOrderReport?foundryOrderId=' + foundryOrderId + '';
    });

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("foundryOrderId", foundryOrderId);

        if (toEmail == "") {
            $('#emailError').html('<div class="alert alert-danger alert-dismissable">' +
               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
        }
        else if (!isValidEmailAddress(toEmail) || (copyEmail != "" && !isValidEmailAddress(copyEmail))) {
            $('#emailError').html('<div class="alert alert-danger alert-dismissable">' +
               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
               '<strong>Warning!</strong>&nbsp;Email Address is not right!</div>');
        }
        else {

            $.ajax({
                type: 'POST',
                url: "/SouthlandMetals/Operations/Report/SendFoundryOrderEmail",
                dataType: "json",
                contentType: false,
                data: formData,
                cache: false,
                processData: false,
                success: function (result) {
                    if (result.Success) {
                        $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                           '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                           '<strong>Success!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                           '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                           '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });

            $('#emailModal').modal('toggle');
        }
    });

    $(document).on('click', '#deleteOrderBtn', function () {
        $.confirm({
            text: 'Are you sure you want to delete ' + orderNumber + '?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/PurchaseOrder/DeleteFoundryOrder",
                    data: { "foundryOrderId": foundryOrderId },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrders';
                        }
                        else {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                               '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                        }
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

    $(document).on('click', '#viewHoldNotes', function () {
        _ViewHoldNotes();
    });

    $(document).on('click', '#viewCancelNotes', function () {
        _ViewCancelNotes();
    });

    function updateCustomerOrderTextArea() {
        $('#customerOrders').html("");
        $('#customerOrders').empty();
        $.each(customerOrderList, function (n, customerOrder) {
            if (n == 0) {
                $('#customerOrders').append(customerOrder);
            }
            else {
                $('#customerOrders').append(", " + customerOrder);
            }
        });
    };

    function updateShipCodeTextArea() {

        $('#shipCodes').html("");
        $('#shipCodes').empty();
        $.each(shipCodeList, function (n, shipCode) {
            if (n > 0) {
                $('#shipCodes').append(", " + shipCode);
            }
            else {
                $('#shipCodes').append(shipCode);
            }
        });
    };

    function _ViewHoldNotes() {
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

    function _ViewCancelNotes() {

        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_ViewCancelNotes",
            data: {
                "CancelNotes": cancelNotes,
                "CanceledDateStr": canceledDate
            },
            contentType: "application/json",
            success: function (result) {

                $('#viewCancelNotesDiv').html('');
                $('#viewCancelNotesDiv').html(result);

                $('#viewCancelNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});