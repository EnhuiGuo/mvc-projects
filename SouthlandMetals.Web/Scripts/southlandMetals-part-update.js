$(document).ready(function () {
    $('#operationsLink').addClass("category-current-link");
    $('#collapseOpsParts').addClass("in");
    $('#collapseOpsParts').attr("aria-expanded", "true");

    $('#opsPartsLink').addClass("category-current-link");
    $('#updateLink').addClass("current-link");

    var updatedParts = [];

    var partsTable = $('#parts').DataTable({
        dom: 'frt' +
            "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 20,
        "lengthChange": false,
        "order": [1, 'asc'],
        "data": updatedParts,
        "columns": [
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "PartNumber", "title": "Part Number", "class": "center" },
        { "data": "PriceChange", "title": "Price", "class": "center" },
        { "data": "CostChange", "title": "Cost", "class": "center" },
        { "data": "WeightChange", "title": "Weight", "class": "center" },
        { "title": "Detail", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 5,
            "title": "Details",
            "width": "8%", "targets": 5,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailBtn'></span>"
        }]
    });

    $('#parts tbody').on('click', '#detailBtn', function () {
        var part = partsTable.row($(this).parents('tr')).data();
        var childData = partsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (part != null) {
            data = part;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Part/Detail?partId=" + data.PartId, target = "_self");
    });

    $(document).on('click', '#fileSubmitBtn', function () {

        var target = document.getElementById('spinnerDiv')
        var spinner = new Spinner(opts).spin(target);

        $("#page-content-wrapper").css({ opacity: 0.5 });
        $('#spinnerDiv').show();

        var myform = document.getElementById("attachment");
        var formData = new FormData(myform);

        var updatePrice = $('#updatePrice').prop('checked');
        var updateCost = $('#updateCost').prop('checked');
        var updateWeight = $('#updateWeight').prop('checked');

        formData.append('UpdatePrice', updatePrice);
        formData.append('UpdateCost', updateCost);
        formData.append('UpdateWeight', updateWeight);

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Part/ImportPartRateAndMeasurement",
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

                    $('#parts').addClass('hidden');
                    $('#parts').DataTable().clear().draw();
                }
                else {

                    $("#page-content-wrapper").css({ opacity: 1.0 });
                    spinner.stop(target);
                    $('#spinnerDiv').hide();

                    $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  'Parts have been updated successfully.</div>');

                    $('.alert').delay(3000).hide("fast");

                    if (result.UpdatedParts != null && result.UpdatedParts.length > 0) {

                        $('#parts').removeClass('hidden');
                        $('#parts').DataTable().clear().draw();
                        $('#parts').DataTable().rows.add(result.UpdatedParts); // Add new data
                        $('#parts').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#parts').addClass('hidden');
                        $('#parts').DataTable().clear().draw();

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