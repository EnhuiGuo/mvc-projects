$(document).ready(function () {

    $('#collapseWarehouse').addClass("in");
    $('#collapseWarehouse').attr("aria-expanded", "true");

    $('#warehouseLink').addClass("category-current-link");
    $('#packingListLink').addClass("current-link");

    var notifications = [];

    var partsTable = $('#parts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [2, 'asc'],
        "data": parts,
        "columns": [
        { "data": "PackingListPartId", "title": "PackingListPartId", "visible": false },
        { "data": "PackingListId", "title": "PackingListId", "visible": false },
        { "data": "ShipCode", "title": "Ship Code", "class": "center" },
        { "data": "PartNumber", "title": "Part Number", "class": "center" },
        { "data": "PalletNumber", "title": "Pallet Number", "class": "center" },
        { "data": "PalletQuantity", "title": "Pallet Quantity", "class": "center" },
        { "data": "PalletWeight", "title": "Pallet Weight", "class": "center" },
        { "data": "PalletTotal", "title": "Total Pallets", "class": "center" },
        { "data": "TotalPalletQuantity", "title": "Total Pallet Quantity", "class": "center" },
        { "data": "PONumber", "title": "PO Number", "class": "center" },
        { "data": "InvoiceNumber", "title": "Invoice Number", "class": "center" },
        { "title": "Edit", "class": "center" },
        { "title": "Delete", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 11,
            "title": "Edit",
            "width": "8%", "targets": 11,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editPartBtn'></span>"
        },
        {
            "targets": 12,
            "title": "Delete",
            "width": "8%", "targets": 12,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-trash' id='deletePartBtn'></span>"
        }]
    });

    GetTotalPieces();
    GetTotalPallets();
    GetTotalWeight();
    GetNetWeight();
    GetGrossWeight();

    $('#parts tbody').on('click', '#editPartBtn', function () {
        var part = partsTable.row($(this).parents('tr')).data();
        var childData = partsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (part != null) {
            data = part;
        }
        else {
            data = childData;
        }
        _EditPackingListPart(data);
    });

    $('#cancelEditPackingListPartBtn').on('click', function () {
        $('#editPackingListPartModal').modal('hide');
    });

    $(document).on('click', '#updatePackingListPartBtn', function () {
        event.preventDefault();

        if (!$("#editPackingListPartForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#editPackingListPartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#editPackingListPartForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var partId = $('#editPartId').val();
            var partNumber = $("#editPartId option:selected").text();
            var palletNumber = $('#editPalletNumber').val();
            var palletQuantity = $('#editPalletQuantity').val();
            var palletWeight = $('#editPalletWeight').val();
            var palletTotal = $('#editPalletTotal').val();
            var totalPalletQuantity = $('#editTotalPalletQuantity').val();
            var customerOrderId = $('#editPONumber').val();
            var poNumber = $("#editPONumber option:selected").text();
            var invoiceNumber = $('#editInvoiceNumber').val();
            var packingListPartId = $('#editPackingListPartId').val();

            $.each(parts, function (n, part) {
                if (part.PackingListPartId == packingListPartId) {
                    part.PartId == partId;
                    part.PartNumber = partNumber;
                    part.PalletNumber = palletNumber;
                    part.PalletQuantity = parseInt(palletQuantity, 10);
                    part.PalletWeight = parseFloat(palletWeight);
                    part.PalletTotal = parseFloat(palletTotal);
                    part.TotalPalletQuantity = parseFloat(totalPalletQuantity);

                    if (part.CustomerOrderId != customerOrderId) {

                        var message = "PackingList part " + partNumber + "had change the PONumber to " + poNumber + ".";
                        notifications.push(message);
                        part.CustomerOrderId = customerOrderId;
                        part.PONumber = poNumber;
                    }
                    part.InvoiceNumber = invoiceNumber;
                    part.IsNew = part.IsNew;
                }
            });

            $('#parts').DataTable().clear().draw();
            $('#parts').DataTable().rows.add(parts); // Add new data
            $('#parts').DataTable().columns.adjust().draw();

            $('#editPackingListPartModal').modal('hide');

            GetTotalPieces();
            GetTotalPallets();
            GetTotalWeight();
            GetNetWeight();
            GetGrossWeight();
        }
    });

    $('#parts tbody').on('click', '#deletePartBtn', function () {
        var part = partsTable.row($(this).parents('tr')).data();
        var childData = partsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (part != null) {
            data = part;
        }
        else {
            data = childData;
        }

        $.confirm({
            text: 'Are you sure you want to delete ' + data.PartNumber + ' ?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {

                partsTable.row(data).remove().draw();
                for (var i = 0; i < parts.length; i++) {
                    if (data.PackingListPartId == parts[i].PackingListPartId)
                        parts.splice(i, 1);
                }

                GetTotalPieces();
                GetTotalPallets();
                GetTotalWeight();
                GetNetWeight();
                GetGrossWeight();
            },
            cancel: function (button) {

            }
        });
    });

    $(document).on('click', '#updatePackingListBtn', function () {
        event.preventDefault();
        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be included in Packing List!</div>');
        }
        else {
            if (!$("#editPackingListForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#editPackingListForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editPackingListForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    PackingListId: packingListId,
                    CustomerId: customerId,
                    CustomerAddressId: $('#editCustomerAddressId').val(),
                    ShipDate: $('#editShipDate').val(),
                    Freight: $('#editFreight').val(),
                    NetWeight: $('#editNetWeight').val(),
                    GrossWeight: $('#editGrossWeight').val(),
                    CarrierId: $('#editCarrierId').val(),
                    TrailerNumber: $('#editTrailerNumber').val(),
                    TrackingNumber: $('#editTrackingNumber').val(),
                    Notes: $('#editNotes').val(),
                    DeliveryDate: $('#editDeliveryDate').val(),
                    IsClosed: $('#isClosed').val(),
                    ClosedDate: null,
                    PackingListParts: parts,
                };

                $.ajax({
                    type: "PUT",
                    url: "/SouthlandMetals/Operations/Warehouse/EditPackingList",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {

                            parts = [];

                            if (notifications != null) {
                                $.each(notifications, function (i, message) {
                                    signalR.server.sendRoleNotification(message, "Admin");
                                });
                            }

                            window.location.href = '/SouthlandMetals/Operations/Warehouse/PackingLists';
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
            }
        }
    });

    $(document).on('click', '#cancelEditPackingListBtn', function () {
        parts = [];
        window.history.back();
    });

    $('#customerId').change(function () {
        customerId = $('#customerId').val();
        if (customerId == "--Select Customer--") {
            $('#customerAddress').val("");
        }
        else {
            getAddressesByCustomer(customerId);
        }
    });

    $(document).on('change', '#editPartId', function () {
        var partId = $('#editPartId').val();
        getPONumberByPart(partId);
    });

    function getPONumberByPart(partId) {
        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/PurchaseOrder/GetSelectableCustomerOrderByPart",
            data: { "partId": partId },
            success: function (result) {

                $('#editPONumber').empty();

                $.each(result, function (n, term) {
                    $("#editPONumber").append($("<option />").val(term.Value).text(term.Text));
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    }

    function getAddressesByCustomer(customerId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/Customer/GetAddressesByCustomer",
            data: { "customerId": customerId },
            contentType: "application/json",
            success: function (result) {

                $('#customerAddressId').empty();

                $.each(result, function (n, term) {
                    $("#customerAddressId").append($("<option />").val(term.Value).text(term.Text));
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function GetTotalPieces() {
        var totalPieces = 0;

        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                totalPieces += parts[i].TotalPalletQuantity;
            }
        }

        $('#totalPieces').text(totalPieces);
    };

    function GetTotalPallets() {
        var totalPallets = 0;

        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                totalPallets += parts[i].PalletTotal;
            }
        }

        $('#totalPallets').text(totalPallets);
    };

    function GetTotalWeight() {
        var totalWeight = 0;

        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                totalWeight += parts[i].PalletWeight;
            }
        }

        totalWeight = CurrencyFormatted(totalWeight);

        $('#totalWeight').text(totalWeight);
    }

    function GetNetWeight() {
        var netWeight = 0;

        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                netWeight += (parts[i].PartWeight * parts[i].PartQuantity);
            }
        }

        netWeight = CurrencyFormatted(netWeight);

        $('#editNetWeight').val(netWeight);
    }

    function GetGrossWeight() {

        var grossWeight = 0;

        console.log(parts);

        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                grossWeight += (parts[i].PartWeight * parts[i].PartQuantity + parts[i].PalletWeight * parts[i].TotalPalletQuantity);
            }
        }

        grossWeight = CurrencyFormatted(grossWeight);

        $('#editGrossWeight').val(grossWeight);
    }

    function _EditPackingListPart(part) {

        $.ajax({
            type: "GET",
            cache: false,
            data: { "partId": part.PartId },
            url: "/SouthlandMetals/Operations/Warehouse/_EditPackingListPart",
            success: function (result) {

                $('#editPackingListPartDiv').html('');
                $('#editPackingListPartDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#editPartId').val(part.PartId);
                $('#editPalletNumber').val(part.PalletNumber);
                $('#editPalletQuantity').val(part.PalletQuantity);
                $('#editPalletWeight').val(part.PalletWeight);
                $('#editPalletTotal').val(part.PalletTotal);
                $('#editTotalPalletQuantity').val(part.TotalPalletQuantity);
                $('#editPONumber').val(part.CustomerOrderId);
                $('#editInvoiceNumber').val(part.InvoiceNumber);
                $('#editPackingListPartId').val(part.PackingListPartId)

                $('#editPackingListPartModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});