$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#collapseInventory').addClass("in");
    $('#collapseInventory').attr("aria-expanded", "true");

    $('#inventoryLink').addClass("category-current-link");
    $('#planningInventoryLink').addClass("current-link");

    var parts = [];

    var partsTable = $('#parts').DataTable({
        dom: 'Bfrt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        buttons: [
            'excelHtml5'
        ],
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "order": [1, 'asc'],
        "data": parts,
        "columns": [
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "QuantityOnHand", "title": "On Hand", "class": "center" },
        { "data": "OnOrderQuantity", "title": "On Order", "class": "center" },
        { "data": "MinimumQuantity", "title": "Minimum", "class": "center" },
        { "data": "SafetyQuantity", "title": "Safety Stock", "class": "center" },
        { "data": "LeadTime", "title": "Lead Time", "class": "center" },
        { "data": "TransitTime", "title": "Transit Time", "class": "center" },
        { "data": "ReorderPoint", "title": "Reorder Point", "class": "center" },
        { "data": "ReorderQuantity", "title": "Reorder Qty", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 10,
            "title": "Details",
            "width": "8%", "targets": 10,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailsBtn'></span>"
        }]
    });

    $('#parts tbody').on('click', '#detailsBtn', function () {
        var part = partsTable.row($(this).parents('tr')).data();
        var childData = partsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (part != null) {
            data = part;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Part/Details?partId=" + data.PartId, target = "_self");
    });

    $(document).on('click', '#filterBtn', function () {

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Inventory/FilterPartPlanning",
            data: {
                CustomerId: $('#customerId').val(),
                FoundryId: $('#foundryId').val(),
                PartNumber: $('#partNumber').val(),
                ZeroOnHand: $('#zeroOnHand').prop('checked'),
                BelowMinimum: $('#belowMin').prop('checked'),
                BelowReorderPoint: $('#belowReorderPoint').prop('checked')
            },
            contentType: "application/json",
            success: function (result) {
                $('#parts').DataTable().clear().draw();
                $('#parts').DataTable().rows.add(result); // Add new data
                $('#parts').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $(document).on('click', '#suggestedPOsBtn', function () {
        window.open("/SouthlandMetals/Operations/Inventory/SuggestedFoundryOrders");
    });

    $(document).on('click', '#resetBtn', function () {

        $('#partNumber').val("");

        $('#customerId>option:eq(0)').prop('selected', true);
        $('#foundryId>option:eq(0)').prop('selected', true);
        $('#zeroOnHand').removeAttr('checked');
        $('#belowMin').removeAttr('checked');
        $('#belowReorderPoint').removeAttr('checked');

        $('#partNumber').focus();
    });
});