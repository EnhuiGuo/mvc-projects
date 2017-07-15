$(document).ready(function () {

    $('#collapseWarehouse').addClass("in");
    $('#collapseWarehouse').attr("aria-expanded", "true");

    $('#warehouseLink').addClass("category-current-link");
    $('#packingListLink').addClass("current-link");

    getAddressesByCustomer(customerId);

    var partsTable = $('#parts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [0, 'asc'],
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
        { "data": "InvoiceNumber", "title": "Invoice Number", "class": "center" }
        ]
    });

    GetTotalPieces();
    GetTotalPallets();
    GetTotalWeight();
    GetNetWeight();
    GetGrossWeight();

    $(document).on('click', '#savePackingListBtn', function () {
        event.preventDefault();

        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be included in Packing List!</div>');
        }
        else {
            if (!$("#createPackingListForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#createPackingListForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#createPackingListForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    CustomerId: $('#customerId').val(),
                    CustomerAddressId: $('#customerAddressId').val(),
                    ShipDate: $('#shipDate').val(),
                    Freight: $('#freight').val(),
                    CarrierId: $('#carrierId').val(),
                    TrailerNumber: $('#trailerNumber').val(),
                    TrackingNumber: $('#trackingNumber').val(),
                    PalletCount: $('#palletCount').val(),
                    NetWeight: $('#netWeight').val(),
                    GrossWeight: $('#grossWeight').val(),
                    Notes: $('#notes').val(),
                    DeliveryDate: null,
                    IsClosed: $('#isClosed').val(),
                    ClosedDate: null,
                    PackingListParts: parts,
                };

                $.ajax({
                    type: "POST",
                    url: "/SouthlandMetals/Operations/Warehouse/CreatePackingList",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {

                            parts = [];

                            $.confirm({
                                text: 'Create Success, Do you want print this packing list?',
                                dialogClass: "modal-confirm",
                                confirmButton: "Yes",
                                confirmButtonClass: 'btn btn-sm',
                                cancelButton: "No",
                                cancelButtonClass: 'btn btn-sm btn-default',
                                closeIcon: false,
                                confirm: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Report/PackingListReport?packingListId=' + result.ReferenceId + '';
                                },
                                cancel: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Warehouse/PackingLists';
                                }
                            });
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

    $(document).on('click', '#cancelSavePackingListBtn', function () {
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

        $('#netWeight').val(netWeight);
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

        $('#grossWeight').val(grossWeight);
    }
});