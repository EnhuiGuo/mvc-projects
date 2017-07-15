$(document).ready(function () {

    var tableParts = parts

    function typeFlag(data) {
        if (data) {
            return "New";
        }
        else {
            return "Active";
        }
    }

    var partsTable = $('#parts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 200,
        "scrollCollapse": true,
        "order": [2, 'asc'],
        "data": tableParts,
        "columns": [
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "IsProject", "title": "Type", "class": "center", render: typeFlag },
        { "title": "Detail", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 4,
            "title": "Detail",
            "width": "8%", "targets": 4,
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

        if (data.IsProject) {
            window.open("/SouthlandMetals/Operations/Part/Detail?partId=" + data.ProjectPartId, target = "_self");
        }
        else {
            window.open("/SouthlandMetals/Operations/Part/Detail?partId=" + data.PartId, target = "_self");
        }
    });

    $(document).on('click', 'input[name=Status]', function () {
        var selectedVal = "";
        var selected = $("input[name='Status']:checked");
        if (selected.length > 0) {
            selectedVal = selected.val();
        }

        if (selectedVal === 'On Hold') {
            $('#projectHoldDiv').show();
            $('#projectCancelDiv').hide();
        }
        else if (selectedVal === 'Canceled') {
            $('#projectHoldDiv').hide();
            $('#projectCancelDiv').show();
        }
        else {
            $('#projectHoldDiv').hide();
            $('#projectCancelDiv').hide();
        }
    });

    $('input[name=Type]').change(function () {
        if (this.value == 'All') {
            tableParts = parts
        }
        else if (this.value == 'New') {
            tableParts = parts.filter(function (part) {
                return part.IsProject == true;
            });
        }
        else if (this.value == 'Active') {
            tableParts = parts.filter(function (part) {
                return part.IsProject == false;
            });
        }

        $('#parts').DataTable().clear().draw();
        if (tableParts != null) {
            $('#parts').DataTable().rows.add(tableParts);
            $('#parts').DataTable().columns.adjust().draw();
        }
    });

    $(document).on('click', '#editStatusBtn', function () {
        var projectId = $('#projectId').val();
        $.confirm({
            text: 'Are you sure you want to update Project status? <br /> <br /> This will update all associated RFQs, Quotes, Price Sheets!',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {

                _EditStatus();
            },
            cancel: function (button) {

            }
        });
    });

    $(document).on('click', '#updateStatusBtn', function () {
        var projectId = $('#projectId').val();
        var selectedVal = "";
        var selected = $("input[name='Status']:checked");
        holdExpirationDate = null;
        holdNote = null;
        cancelNote = null;

        if (selected.length > 0) {
            selectedVal = selected.val();
        }

        if (selectedVal === 'On Hold') {
            holdExpirationDate = $('#holdExpirationDate').val();
            holdNote = $('#holdNote').val();
        }
        else if (selectedVal === 'Cancel') {
            cancelNote = $('#cancelNote').val();
        }

        if (selectedVal === "On Hold" || selectedVal === "Cancel") {
            var canEdit = validateUpdate(projectId);

            if (!canEdit) {
                var message = "This project has parts that have been received, unable to put on hold or cancel."
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;' + message + '</div>');
            }
            else {
                var model = {
                    ProjectId: $('#projectId').val(),
                    Status: selectedVal,
                    HoldExpirationDate: holdExpirationDate,
                    HoldNotes: holdNote,
                    CancelNotes: cancelNote
                };

                $.ajax({
                    type: "PUT",
                    url: "/SouthlandMetals/Operations/Project/EditStatus",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            $('#projectStatus').text(result.Status);

                            if (result.IsHold) {
                                signalR.server.sendRoleNotification(currentUser + " has put Project " + projectName + " on hold til " + holdExpirationDate, "Admin");
                            }
                            else if (result.IsCanceled) {
                                signalR.server.sendRoleNotification(currentUser + " has canceled Project " + projectName, "Admin");
                            }

                            holdExpirationDate = null;
                            holdNote = null;
                            cancelNote = null;
                        }
                        else {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                       '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                       '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                        }

                        $('#editStatusModal').modal('hide');
                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });
            }
        }
    });

    $('#cancelEditStatusBtn').on('click', function () {
        $('#editStatusModal').modal('hide');
    });

    $('#addNote').on('click', function () {
        var projectId = $('#projectId').val();
        _AddNote(projectId);
    });

    $('#cancelNoteBtn').on('click', function () {
        $('#addNoteModal').modal('hide');
    });

    $(document).on('click', '#saveNoteBtn', function () {
        if (!$("#addProjectNoteForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addProjectNoteForm textarea[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                ProjectId: $('#projectId').val(),
                Note: $('#note').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Operations/Project/AddProjectNote",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#notes').append('<p>Note: ' + result.Note +
                       '<span style="padding-top: 4px;" aria-hidden="true" class="glyphicon glyphicon-trash pull-right" onclick="deleteProjectNote(\'' + result.ProjectNoteId + '\');"></span>' +
                       '<br />' + result.CreatedBy + ' ' + result.CreatedDate + '</p>');

                        $('#addNoteModal').modal('hide');
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
    });

    window.deleteProjectNote = function (projectNoteId) {
        $.confirm({
            text: 'Are you sure you want to delete this note?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function () {
                $.ajax({
                    type: "Delete",
                    url: "/SouthlandMetals/Operations/Project/DeleteProjectNote",
                    data: { 'projectNoteId': projectNoteId },
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            $("#" + projectNoteId).remove();
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
            cancel: function () {

            }
        });
    };

    $(document).on('click', '#viewHoldNotesBtn', function () {
        _ViewProjectHoldNotes();
    });

    $(document).on('click', '#viewCancelNotesBtn', function () {
        _ViewProjectCancelNotes();
    });

    $("#deleteProjectBtn").click(function () {
        $.confirm({
            text: 'Are you sure you want to delete?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {

                if (foundryOrders != null && foundryOrders.length > 0) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;There are Foundry Orders entered against this Project, unable to delete!</div>');
                }
                else {
                    var projectId = $('#projectId').val();
                    $.ajax({
                        type: 'Delete',
                        url: "/SouthlandMetals/Operations/Project/Delete",
                        data: { "projectId": projectId },
                        success: function (result) {
                            if (result.Success) {
                                window.location.href = '/SouthlandMetals/Operations/Project';
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
            },
            cancel: function (button) {

            }
        });
    });

    var rfqsTable = $('#rfqs').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": rfqs,
        "columns": [
        { "data": "RfqId", "title": "RfqId", "visible": false },
        { "data": "RfqNumber", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='rfqBtn'></span>"
        }]
    });

    $('#rfqs tbody').on('click', '#rfqBtn', function () {
        var rfq = rfqsTable.row($(this).parents('tr')).data();
        var childData = rfqsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfq != null) {
            data = rfq;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/Rfq/Detail?rfqId=' + data.RfqId + '';
    });

    var priceSheetsTable = $('#priceSheets').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": priceSheets,
        "columns": [
        { "data": "PriceSheetId", "title": "PriceSheetId", "visible": false },
        { "data": "Number", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='priceSheetBtn'></span>"
        }]
    });

    $('#priceSheets tbody').on('click', '#priceSheetBtn', function () {
        var priceSheet = priceSheetsTable.row($(this).parents('tr')).data();
        var childData = priceSheetsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (priceSheet != null) {
            data = priceSheet;
        }
        else {
            data = childData;
        }

        if (data.Status === "Quote")
            window.location.href = '/SouthlandMetals/Operations/Pricing/Detail?priceSheetNumber=' + data.Number + '';
        else if (data.Status === "Production")
            window.location.href = '/SouthlandMetals/Operations/Pricing/Production?priceSheetId=' + data.PriceSheetId + '&number=' + data.Number + '';
    });

    var quotesTable = $('#quotes').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": quotes,
        "columns": [
        { "data": "QuoteId", "title": "QuoteId", "visible": false },
        { "data": "QuoteNumber", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='quoteBtn'></span>"
        }]
    });

    $('#quotes tbody').on('click', '#quoteBtn', function () {
        var quote = quotesTable.row($(this).parents('tr')).data();
        var childData = quotesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (quote != null) {
            data = quote;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/Quote/Detail?quoteId=' + data.QuoteId;
    });


    var sampleOrdersTable = $('#sampleOrders').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": sampleOrders,
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='sampleOrderBtn'></span>"
        }]
    });

    $('#sampleOrders tbody').on('click', '#sampleOrderBtn', function () {
        var sampleOrder = sampleOrdersTable.row($(this).parents('tr')).data();
        var childData = sampleOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (sampleOrder != null) {
            data = sampleOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + data.FoundryOrderId;
    });

    var toolingOrdersTable = $('#toolingOrders').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": toolingOrders,
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='toolingOrderBtn'></span>"
        }]
    });

    $('#toolingOrders tbody').on('click', '#toolingOrderBtn', function () {
        var toolingOrder = toolingOrdersTable.row($(this).parents('tr')).data();
        var childData = toolingOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (toolingOrder != null) {
            data = toolingOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + data.FoundryOrderId;
    });

    var productionOrdersTable = $('#productionOrders').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "info": false,
        "scrollY": 250,
        "scrollCollapse": true,
        "order": [1, 'asc'],
        "data": productionOrders,
        "columns": [
        { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
        { "data": "OrderNumber", "title": "Number", "class": "center" },
        { "title": "Details", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 2,
            "title": "Details",
            "width": "8%", "targets": 2,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='productionOrderBtn'></span>"
        }]
    });

    $('#productionOrders tbody').on('click', '#productionOrderBtn', function () {
        var productionOrder = productionOrdersTable.row($(this).parents('tr')).data();
        var childData = productionOrdersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (productionOrder != null) {
            data = productionOrder;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + data.FoundryOrderId;
    });

    $(document).on('click', '#shipmentBtn', function () {
        window.location.href = '/SouthlandMetals/Operations/Shipment/Tracking' + _self;
    });
});

function _AddNote(projectId) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_AddNote",
        data: { "projectId": projectId },
        success: function (result) {

            $('#addNoteDiv').html('');
            $('#addNoteDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addNoteModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
}

function _ViewRfqs() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewRfqs",
        success: function () {
            if (rfqs.length < 1) {
                $('#rfqs').parents('div.dataTables_wrapper').first().hide();
                $('.no-rfqs').show();
            }
            else {
                $('#rfqs').DataTable().clear().draw();
                $('#rfqs').DataTable().rows.add(rfqs); // Add new data
                $('#rfqs').DataTable().columns.adjust().draw();
            }
            $('#viewRfqsModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewToolingOrders() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewToolingOrders",
        success: function () {
            if (toolingOrders.length < 1) {
                $('#toolingOrders').parents('div.dataTables_wrapper').first().hide();
                $('.no-toolingOrders').show();
            }
            else {
                $('#toolingOrders').DataTable().clear().draw();
                $('#toolingOrders').DataTable().rows.add(toolingOrders); // Add new data
                $('#toolingOrders').DataTable().columns.adjust().draw();
            }
            //getToolingOrdersByProject(projectId);
            $('#viewToolingOrdersModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewSampleOrders() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewSampleOrders",
        success: function () {
            if (sampleOrders.length < 1) {
                $('#sampleOrders').parents('div.dataTables_wrapper').first().hide();
                $('.no-sampleOrders').show();
            }
            else {
                $('#sampleOrders').DataTable().clear().draw();
                $('#sampleOrders').DataTable().rows.add(sampleOrders); // Add new data
                $('#sampleOrders').DataTable().columns.adjust().draw();
            }
            $('#viewSampleOrdersModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewProductionPriceSheets() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewProductionPriceSheets",
        success: function () {
            if (priceSheets.length < 1) {
                $('#priceSheets').parents('div.dataTables_wrapper').first().hide();
                $('.no-priceSheets').show();
            }
            else {
                $('#priceSheets').DataTable().clear().draw();
                $('#priceSheets').DataTable().rows.add(priceSheets); // Add new data
                $('#priceSheets').DataTable().columns.adjust().draw();
            }
            $('#viewPriceSheetsModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewQuotes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewQuotes",
        success: function () {
            if (quotes.length < 1) {
                $('#quotes').parents('div.dataTables_wrapper').first().hide();
                $('.no-quotes').show();
            }
            else {
                $('#quotes').DataTable().clear().draw();
                $('#quotes').DataTable().rows.add(quotes); // Add new data
                $('#quotes').DataTable().columns.adjust().draw();
            }
            //getQuotesByProject(projectId);
            $('#viewQuotesModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewProductionOrders() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewProductionOrders",
        success: function () {
            if (productionOrders.length < 1) {
                $('#productionOrders').parents('div.dataTables_wrapper').first().hide();
                $('.no-productionOrders').show();
            }
            else {
                $('#productionOrders').DataTable().clear().draw();
                $('#productionOrders').DataTable().rows.add(productionOrders); // Add new data
                $('#productionOrders').DataTable().columns.adjust().draw();
            }
            $('#viewProductionOrdersModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function _ViewShipments() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewShipments",
        success: function () {
            if (shipments.length < 1) {
                $('#shipments').parents('div.dataTables_wrapper').first().hide();
                $('.no-shipments').show();
            }
            else {
                $('#shipments').DataTable().clear().draw();
                $('#shipments').DataTable().rows.add(shipments); // Add new data
                $('#shipments').DataTable().columns.adjust().draw();
            }
            $('#viewShipmentsModal').modal('show');
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function validateUpdate(projectId) {
    var success = false;

    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        url: "/SouthlandMetals/Operations/Project/ValidateUpdate",
        data: { "projectId": projectId },
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
        }
    });

    return success;
};

function _EditStatus() {

    var projectId = $('#projectId').val();
    var projectName = $('#projectName').val();
    var projectStatus = status;

    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_EditStatus",
        success: function (result) {

            $('#editStatusDiv').html('');
            $('#editStatusDiv').html(result);

            $('#projectId').val(projectId);
            $('#projectName').text(projectName);

            $('#projectHoldDiv').hide();
            $('#projectCancelDiv').hide();

            if (projectStatus === "Open") {
                $('#open').prop('checked', true);
            }
            else if (projectStatus === "On Hold") {
                $('#hold').prop('checked', true);
                $('#holdExpirationDate').val(holdExpirationDate);
                $('#holdNote').val(holdNote);
                $('#projectHoldDiv').show();
            }
            else if (projectStatus === "Canceled") {
                $('#canceled').prop('checked', true);
                $('#cancelNote').val(cancelNote);
                $('#projectCancelDiv').show();
            }

            GetDatePicker();

            $('#editStatusModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewProjectHoldNotes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/Notes/_ViewHoldNotes",
        data: {
            "HoldNotes": holdNote,
            "HoldExpirationDateStr": holdExpirationDate
        },
        contentType: "application/json",
        success: function (result) {

            $('#viewHoldNotesDiv').html('');
            $('#viewHoldNotesDiv').html(result);

            $('#viewHoldNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewProjectCancelNotes() {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/Notes/_ViewCancelNotes",
        data: {
            "CancelNotes": cancelNote,
            "CanceledDateStr": canceledDate
        },
        contentType: "application/json",
        success: function (result) {

            $('#viewCancelNotesDiv').html('');
            $('#viewCancelNotesDiv').html(result);

            $('#viewCancelNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

