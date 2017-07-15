$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#customerLink').addClass("current-link");

    var priceSheetList = [];

    $.each(orderParts, function (n, orderPart) {
        if ($.inArray(orderPart.PriceSheetNumber, priceSheetList) == -1)
            priceSheetList.push(orderPart.PriceSheetNumber);
    });

    updatePriceSheetTextArea();

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show();
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
        { "data": "PriceSheetPartId", "title": "PartId", "visible": false },
        { "data": "AvailableQuantity", "title": "Av Qty", "class": "center" },
        { "data": "CustomerOrderQuantity", "title": "Quantity", "class": "center" },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "PriceSheetNumber", "title": "Price Sheet", "class": "center" },
        { "data": "EstArrivalDate", "name": "EstArrivalDate", "title": "Arrival Date", "class": "center", render: dateFlag },
        { "data": "ReceiptQuantity", "title": "Receipt Quantity", "class": "center" },
        { "data": "Price", "title": "Unit Price", "class": "center", "render": dollarFlag },
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
                    window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/EditCustomerOrder?customerOrderId=' + customerOrderId + '';
                },
                cancel: function (button) {

                }
            });
        }
        else {
            window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/EditCustomerOrder?customerOrderId=' + customerOrderId + '';
        }
    });

    $(document).on('click', '#printBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Report/CustomerOrderReport?customerOrderId=' + customerOrderId + '';
    });

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("customerOrderId", customerOrderId);

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
                url: "/SouthlandMetals/Operations/Report/SendCustomerOrderEmail",
                dataType: "json",
                contentType: false,
                data: formData,
                cache: false,
                processData: false,
                success: function () {
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
            text: 'Are you sure you want to delete ' + poNumber + '?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/PurchaseOrder/DeleteCustomerOrder",
                    data: { "customerOrderId": customerOrderId },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/CustomerOrders';
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

    function updatePriceSheetTextArea() {

        $('#priceSheets').html("");
        $('#priceSheets').empty();
        $.each(priceSheetList, function (n, priceSheet) {
            if (n > 0) {
                $('#priceSheets').append(", " + priceSheet);
            }
            else {
                $('#priceSheets').append(priceSheet);
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