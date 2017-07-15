$(document).ready(function () {
   
    $('#operationsLink').addClass("category-current-link");
    $('#collapseOpsParts').addClass("in");
    $('#collapseOpsParts').attr("aria-expanded", "true");

    $('#opsPartsLink').addClass("category-current-link");
    $('#detailsLink').addClass("current-link");

    var parts = [];

    var partsTable = $('#parts').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "order": [1, 'asc'],
        "data": parts,
        "columns": [
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "FoundryId", "title": "FoundryId", "visible": false },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "HtsNumberId", "title": "HtsNumberId", "visible": false },
        { "data": "HtsNumber", "title": "HtsNumber", "class": "center" },
        { "data": "PartStatusId", "title": "PartStatusId", "visible": false },
        { "data": "PartStatusDescription", "title": "Status", "class": "center" },
        { "data": "PartTypeId", "title": "PartTypeId", "visible": false },
        { "data": "PartTypeDescription", "title": "Type", "class": "center" },
        { "title": "Detail", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 13,
            "title": "Details",
            "width": "8%", "targets": 13,
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

    $(document).on('click', '#searchBtn', function () {

        var target = document.getElementById('spinnerDiv');
        var spinner = new Spinner(opts).spin(target);

        $("#page-content-wrapper").css({ opacity: 0.5 });
        $('#spinnerDiv').show();

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Part/SearchParts",
            data: {
                PartNumber: $('#partNumber').val(),
                ProjectId: $('#projectId').val(),
                CustomerId: $('#customerId').val(),
                FoundryId: $('#foundryId').val(),
                HtsNumberId: $('#htsNumberId').val(),
                PartStatusId: $('#partStatusId').val(),
                PartTypeId: $('#partTypeId').val()
            },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {

                $("#page-content-wrapper").css({ opacity: 1.0 });
                spinner.stop(target);
                $('#spinnerDiv').hide();

                $('#parts').DataTable().clear().draw();
                $('#parts').DataTable().rows.add(result); // Add new data
                $('#parts').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });
});