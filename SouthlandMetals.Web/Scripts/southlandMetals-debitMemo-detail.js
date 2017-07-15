$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#debitMemoLink').addClass("current-link");

    $('.emailError').hide();

    if (status === "Open") {
        $('#open').prop('checked', true);
    }
    else if (status === "Closed") {
        $('#closed').prop('checked', true);
        $('#editMemoBtn').hide();
        $('#deleteMemoBtn').hide();
        $('#editIcon').hide();
        $('#deleteIcon').hide();
    }

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };

    var debitMemoItemsTable = $('#debitMemoItems').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [3, 'asc'],
        "data": items,
        "columns": [
        { "data": "DebitMemoItemId", "title": "DebitMemoItemId", "visible": false },
        { "data": "DebitMemoId", "title": "DebitMemoId", "visible": false },
        { "data": "PartNumber", "title": "Part Number", "class": "center" },
        { "data": "ItemQuantity", "title": "Quantity", "class": "center" },
        { "data": "ItemDescription", "title": "Description", "class": "center" },
        { "data": "ItemCost", "title": "Unit Cost", "class": "center", "render": dollarFlag },
        { "data": "ExtendedCost", "title": "Extended Cost", "class": "center", "render": dollarFlag },
        { "data": "DateCode", "name": "DateCode", "title": "Date Code", "class": "center", "render": dateFlag },
        { "data": "Reason", "title": "Reason", "class": "center" },
        ]
    });

    $(document).on('click', '#editMemoBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Shipment/EditDebitMemo?debitMemoId=' + debitMemoId + '';
    });

    $(document).on('click', '#printMemoBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Report/DebitMemoReport?debitMemoId=' + debitMemoId + '';
    });

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("debitMemoId", debitMemoId);

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
                url: "/SouthlandMetals/Operations/Report/SendDebitMemoEmail",
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

    $(document).on('click', '#deleteMemoBtn', function () {
        $.confirm({
            text: 'Are you sure you want to delete ' + debitMemoNumber + '?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/Shipment/DeleteDebitMemo",
                    data: { "debitMemoId": debitMemoId },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = '/SouthlandMetals/Operations/Shipment/DebitMemos';
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
});