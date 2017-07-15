$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#creditMemoLink').addClass("current-link");

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };

    var creditMemoItemsTable = $('#creditMemoItems').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "data": items,
        "columns": [
        { "data": "CreditMemoItemId", "title": "CreditMemoItemId", "visible": false },
        { "data": "CreditMemoId", "title": "CreditMemoId", "visible": false },
        { "data": "ItemQuantity", "title": "Quantity", "class": "center" },
        { "data": "ItemDescription", "title": "Description", "class": "center" },
        { "data": "ItemPrice", "title": "Unit Price", "class": "center", "render": dollarFlag },
        { "data": "ExtendedPrice", "title": "Extended Price", "class": "center", "render": dollarFlag }
        ]
    });

    $(document).on('click', '#editMemoBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Shipment/CreditMemoEdit?creditMemoId=' + creditMemoId + '';
    });

    $(document).on('click', '#printMemoBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Report/CreditMemoReport?creditMemoId=' + creditMemoId + '';
    });

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("creditMemoId", creditMemoId);

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
                url: "/SouthlandMetals/Operations/Report/SendCreditMemoEmail",
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
});