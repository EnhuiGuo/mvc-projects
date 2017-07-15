$(document).ready(function () {
    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#schedulingLink').addClass("current-link");

    var customerOrderDifferent = $('#foundryOrderDifferent').DataTable({
        "columns": [
        { "data": "PONumber", "title": "PO Number", "class": "center" },
        {
            "data": "ShipDateChange", "title": "Ship Date Change", "class": "center",
            render: function (data, type) {
                if (type == "display") {
                    if (data == null)
                        return "No Change";
                }
                return data;
            }
        },
        ],
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
    });

    $(document).on('click', '#fileSubmitBtn', function () {

        var target = document.getElementById('spinnerDiv')
        var spinner = new Spinner(opts).spin(target);

        $("#page-content-wrapper").css({ opacity: 0.5 });
        $('#spinnerDiv').show();

        var myform = document.getElementById("attachment");
        var formData = new FormData(myform);

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/PurchaseOrder/ImportSchedule",
            dataType: "json",
            contentType: false,
            data: formData,
            async: true,
            cache: false,
            processData: false,
            success: function (result) {
                if (!result.Success) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                               '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');

                    $("#page-content-wrapper").css({ opacity: 1.0 });
                    spinner.stop(target);
                    $('#spinnerDiv').hide();

                    $('#foundryOrderDifferent').addClass('hidden');
                    $('#foundryOrderDifferent').DataTable().clear().draw();
                }
                else {

                    $("#page-content-wrapper").css({ opacity: 1.0 });
                    spinner.stop(target);
                    $('#spinnerDiv').hide();

                    $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 'Parts and Orders have been scheduled successfully.</div>');

                    $('.alert').delay(3000).hide("fast");

                    if (result.FoundryOrderDifferents.length > 0) {

                        $('#foundryOrderDifferent').removeClass('hidden');
                        $('#foundryOrderDifferent').DataTable().clear().draw();
                        $('#foundryOrderDifferent').DataTable().rows.add(result.FoundryOrderDifferents); // Add new data
                        $('#foundryOrderDifferent').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#foundryOrderDifferent').addClass('hidden');
                        $('#foundryOrderDifferent').DataTable().clear().draw();

                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                    '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                    '<strong>No Update!</strong>!</div>');
                    }
                }

                $('input').val("");
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });
});