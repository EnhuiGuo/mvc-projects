$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#collapseInventory').addClass("in");
    $('#collapseInventory').attr("aria-expanded", "true");

    $('#inventoryLink').addClass("category-current-link");
    $('#planningInventoryLink').addClass("current-link");

    var ordersTable = $('#orders').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "order": [1, 'asc'],
        "data": orders,
        "columns": [
        { "data": "FoundryId", "title": "FoundryId", "visible": false },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "CustomerAddressId", "title": "CustomerAddressId", "visible": false },
        { "data": "CustomerAddress", "title": "Address", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 6,
            "title": "View",
            "width": "8%", "targets": 6,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewBtn'></span>"
        }]
    });

    $('#orders tbody').on('click', '#viewBtn', function () {
        var order = ordersTable.row($(this).parents('tr')).data();
        var childData = ordersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (order != null) {
            data = order;
        }
        else {
            data = childData;
        }

        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/SuggestedFoundryOrder?' +
            'customerId=' + data.CustomerId + '&customerAddressId=' + data.CustomerAddressId + '&foundryId=' + data.FoundryId + '';
    });
});