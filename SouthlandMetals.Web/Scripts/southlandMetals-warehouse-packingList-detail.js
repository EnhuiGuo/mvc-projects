$(document).ready(function () {

    $('#collapseWarehouse').addClass("in");
    $('#collapseWarehouse').attr("aria-expanded", "true");

    $('#warehouseLink').addClass("category-current-link");
    $('#packingListLink').addClass("current-link");

    if (isClosed) {
        $('#editPackingListBtn').hide();
        $('#deleteRfqBtn').hide();
        $('#editIcon').hide();
        $('#deletePackingListBtn').hide();
    }

    var partsTable = $('#parts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [3, 'asc'],
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

    $("#closePackingListBtn").click(function () {
        $.confirm({
            text: 'Are you sure you want to close this Packing List?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'POST',
                    url: "/SouthlandMetals/Operations/Warehouse/ClosePackingList",
                    data: { "packingListId": packingListId },
                    success: function (result) {
                        if (result.Success) {
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
            },
            cancel: function (button) {

            }
        });
    });

    $("#editPackingListBtn").click(function () {
        window.location.href = '/SouthlandMetals/Operations/Warehouse/EditPackingList?packingListId=' + packingListId + '';
    });

    $("#printPackingListBtn").click(function () {
        window.location.href = '/SouthlandMetals/Operations/Report/PackingListReport?packingListId=' + packingListId + '';
    });

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("packingListId", packingListId);

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
                url: "/SouthlandMetals/Operations/Report/SendPackingListEmail",
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

    $("#deletePackingListBtn").click(function () {
        $.confirm({
            text: 'Are you sure you want to delete this Packing List?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/Warehouse/DeletePackingList",
                    data: { "packingListId": packingListId },
                    success: function (result) {
                        if (result.Success) {
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
            },
            cancel: function (button) {

            }
        });
    });

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
});