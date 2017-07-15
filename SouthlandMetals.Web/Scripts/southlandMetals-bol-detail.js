$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#shipmentTrackingLink').addClass("current-link");

    $('#isCustomsLiquidated').prop('checked', isCustomsLiquidated);
    $('#hasLcl').prop('checked', hasLcl);
    $('#hasDoorMove').prop('checked', hasDoorMove);
    $('#hasArrivalNotice').prop('checked', hasArrivalNotice);
    $('#hasOriginals').prop('checked', hasOriginals);

    if (hasBeenAnalyzed) {
        $('#deleteBillOfLadingBtn').hide();
        $('#deleteIcon').hide();
    }
    else {
        $('#paymentBtn').hide();
        $('#paymentIcon').hide();
    }

    if (hasBeenProcessed) {
        $('#paymentBtn').hide();
        $('#paymentIcon').hide();
        $('#deleteBillOfLadingBtn').hide();
        $('#deleteIcon').hide();
    }

    var Part = function (pId, pNumber, oId, oNumber, cId, cNumber, availableQty, quantity, fopId) {
        var self = this;

        self.partId = ko.observable(pId);
        self.partNumber = ko.observable(pNumber);
        self.orderId = ko.observable(oId);
        self.orderNumber = ko.observable(oNumber);
        self.containerId = ko.observable(cId);
        self.containerNumber = ko.observable(cNumber);
        self.availableQuantity = ko.observable(availableQty);
        self.quantity = ko.observable(quantity);
        self.foundryOrderPartId = ko.observable(fopId);
    };

    function loadTables() {
        $('#containers').DataTable().clear().draw();
        $('#containers').DataTable().rows.add(containers); // Add new data
        $('#containers').DataTable().columns.adjust().draw();

        $('#containerParts').DataTable().clear().draw();
        $('#containerParts').DataTable().rows.add(parts); // Add new data
        $('#containerParts').DataTable().columns.adjust().draw();

        $('#purchaseOrders').DataTable().clear().draw();
        $('#purchaseOrders').DataTable().rows.add(purchaseOrders); // Add new data
        $('#purchaseOrders').DataTable().columns.adjust().draw();

        $('#buckets').DataTable().clear().draw();
        $('#buckets').DataTable().rows.add(buckets); // Add new data
        $('#buckets').DataTable().columns.adjust().draw();
    };

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    }

    var bucketsTable = $('#buckets').DataTable({
        "autoWidth": false,
        "width": "75%",
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "scrollCollapse": true,
        "info": false,
        "ordering": [2, 'asc'],
        "data": buckets,
        "columns": [
        { "data": "BucketId", "title": "BucketId", "visible": false },
        { "data": "FoundryInvoiceId", "title": "FoundryInvoiceId", "visible": false },
        { "data": "BucketName", "title": "Bucket", "class": "center" },
        { "data": "BucketValue", "title": "Value", "class": "center", "render": dollarFlag }
        ]
    });

    var containersTable = $('#containers').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "scrollCollapse": true,
        "info": false,
        "data": containers,
        "order": [1, 'asc'],
        "columns": [
        { "data": "ContainerId", "title": "ContainerId", "visible": false },
        { "data": "ContainerNumber", "title": "Number", "class": "center" },
        { "title": "Contents", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Contents",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                  "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='contentsBtn'></span>"
        }
        ]
    });

    $('#containers tbody').on('click', '#contentsBtn', function () {
        var container = containersTable.row($(this).parents('tr')).data();
        var childData = containersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (container != null) {
            data = container;
        }
        else {
            data = childData;
        }

        var parts = containerParts.filter(function (n) {
            return n.ContainerId === data.ContainerId;
        });

        $('#containerParts').DataTable().clear().draw();
        $('#containerParts').DataTable().rows.add(parts); // Add new data
        $('#containerParts').DataTable().columns.adjust().draw();

        $('#billOfLadingTabs li:nth-child(1)').removeClass("active");
        $('#containersTab').removeClass("active");
        $('#billOfLadingTabs li:nth-child(2)').addClass("active")
        $('#partsTab').addClass("active");

        $('#containerParts').DataTable().columns.adjust().draw();
    });


    var containerPartsTable = $('#containerParts').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "data": containerParts,
        "scrollY": "150px",
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "columns": [
        { "data": "ContainerPartId", "title": "ContainerPartId", "visible": false },
        { "data": "FoundryOrderPartId", "title": "FoundryOrderPartId", "visible": false },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Purchase Order", "class": "center" },
        { "data": "ShipCode", "title": "Ship Code", "class": "center" },
        { "data": "ContainerId", "title": "ContainerId", "visible": false },
        { "data": "AvailableQuantity", "title": "AvailableQuantity", "visible": false },
        { "data": "Quantity", "title": "Quantity", "class": "center" },
        { "title": "Detail", "class": "center" },
        ],
        "columnDefs": [{
            "targets": 9,
            "title": "Detail",
            "width": "8%", "targets": 9,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailBtn'></span>"
        },
        ]
    });

    $('#deleteBillOfLadingBtn').click(function () {

        if (canDeleteBol(bolId))
        {
            $.confirm({
                text: 'Are you sure you want to delete ' + $('#bolNumber').val() + '?',
                dialogClass: "modal-confirm",
                confirmButton: "Yes",
                confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                cancelButton: "No",
                cancelButtonClass: 'btn btn-sm btn-default',
                closeIcon: false,
                confirm: function (button) {
                    $.ajax({
                        type: 'DELETE',
                        url: "/SouthlandMetals/Operations/Shipment/DeleteBillOfLading",
                        data: { "billOfLadingId": bolId },
                        complete: function () {
                            window.location.href = '/SouthlandMetals/Operations/Shipment/Tracking';
                        },
                        error: function (err) {
                            console.log('Error ' + err.responseText);
                        }
                    });
                },
                cancel: function (button) {

                }
            });
        }
    });

    $('#containerParts tbody').on('click', '#detailBtn', function () {
        var containerPart = containerPartsTable.row($(this).parents('tr')).data();
        var childData = containerPartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (containerPart != null) {
            data = containerPart;
        }
        else {
            data = childData;
        }

        var model = $.grep(containerParts, function (part, i) {
            return part.ContainerPartId == data.ContainerPartId;
        });

        _ViewContainerPartDetail(model[0]);
    });

    var purchaseOrdersTable = $('#purchaseOrders').DataTable({
        "autoWidth": false,
        "searching": false,
        "paging": false,
        "scrollY": "200px",
        "scrollCollapse": true,
        "info": false,
        "data": purchaseOrders,
        "order": [[3,'desc'], [1,'asc']],
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "data": "EstArrivalDate", "name": "EstArrivalDate", "title": "ETA", "class": "center", render: dateFlag },
        ],
        "columnDefs": [
        {
            "targets": 3,
            "title": "Contents",
            "width": "8%", "targets": 3,
            "data": null,
            "defaultContent":
            "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='contentsBtn'></span>"
        }
        ]
    });

    $(document).on('click', '#printBtn', function () {
        if(canShipmentAnalysis(bolId))
            window.location.href = '/SouthlandMetals/Operations/Report/ShipmentAnalysisReport?bolId=' + bolId + '';
    });

    $('#purchaseOrders tbody').on('click', '#contentsBtn', function () {
        var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
        var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (purchaseOrder != null) {
            data = purchaseOrder;
        }
        else {
            data = childData;
        }

        var parts = containerParts.filter(function (n) {
            return n.FoundryOrderId == data.FoundryOrderId;
        });

        $('#containerParts').DataTable().clear().draw();
        $('#containerParts').DataTable().rows.add(parts); // Add new data
        $('#containerParts').DataTable().columns.adjust().draw();

        $('#billOfLadingTabs li:nth-child(3)').removeClass("active");
        $('#purchaseOrdersTab').removeClass("active");
        $('#billOfLadingTabs li:nth-child(2)').addClass("active")
        $('#partsTab').addClass("active");

        $('#containerParts').DataTable().columns.adjust().draw();
    });

    $('#purchaseOrders tbody').on('click', '#notesBtn', function () {
        var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
        var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (purchaseOrder != null) {
            data = purchaseOrder;
        }
        else {
            data = childData;
        }
        _ViewShipCodeNotes(data.ShipCodeNotes);
    });

    var palletsTable = $('#pallets').DataTable({
        "autoWidth": false,
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "scrollCollapse": true,
        "info": false,
        "data": pallets,
        "order": [1, 'asc'],
        "columns": [
        { "data": "PalletNumber", "title": "Number", "class": "center" },
        { "title": "Contents", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 1,
            "title": "Contents",
            "width": "8%", "targets": 1,
            "data": null,
            "defaultContent":
                  "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='palletsBtn'></span>"
        }
        ]
    });

    $('#pallets tbody').on('click', '#palletsBtn', function () {
        var pallet = palletsTable.row($(this).parents('tr')).data();
        var childData = palletsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (pallet != null) {
            data = pallet;
        }
        else {
            data = childData;
        }

        var parts = containerParts.filter(function (n) {
            return n.PalletNumber === data.PalletNumber;
        });

        $('#containerParts').DataTable().clear().draw();
        $('#containerParts').DataTable().rows.add(parts); // Add new data
        $('#containerParts').DataTable().columns.adjust().draw();

        $('#billOfLadingTabs li:nth-child(5)').removeClass("active");
        $('#palletTab').removeClass("active");
        $('#billOfLadingTabs li:nth-child(2)').addClass("active")
        $('#partsTab').addClass("active");

        $('#containerParts').DataTable().columns.adjust().draw();
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $('#containerParts').DataTable().columns.adjust().draw();
        $('#containers').DataTable().columns.adjust().draw();
        $('#purchaseOrders').DataTable().columns.adjust().draw();
        $('#buckets').DataTable().columns.adjust().draw();
        $('#pallets').DataTable().columns.adjust().draw();
    });

    function canDeleteBol(bolId) {
        var success = false;
        $.ajax({
            type: 'GET',
            cache: false,
            async: false,
            url: "/SouthlandMetals/Operations/Shipment/ValidateBillofLadingDelete",
            data: { "bolId": bolId },
            success: function (result) {

                if (!result.Success) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                          '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                          '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                }
                else {
                    success = true;
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            },
        });

        return success;
    }

    function canShipmentAnalysis(bolId) {
        var success = false;
        $.ajax({
            type: 'GET',
            cache: false,
            async: false,
            url: "/SouthlandMetals/Operations/Shipment/ValidateShipmentAnalysis",
            data: { "bolId": bolId },
            success: function (result) {
                if (!result.Success) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                          '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                          '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                }
                else {
                    success = true;
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            },
        });

        return success;
    }
});

function _ViewShipCodeNotes(notes) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_ViewShipCodeNotes",
        data: { "notes": notes },
        success: function (result) {

            $('#viewShipCodeNotesDiv').html('');
            $('#viewShipCodeNotesDiv').html(result);

            $('#viewShipCodeNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};


function _ViewContainerPartDetail(model) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_ViewContainerPartDetails",
        data: model,
        success: function (result) {

            $('#viewContainerPartDetailDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#viewContainerPartDetailModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
