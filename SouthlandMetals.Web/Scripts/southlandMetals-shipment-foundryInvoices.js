﻿$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#foundryInvoicesLink').addClass("current-link");

    var invoices = [];

    var invoicesTable = $('#invoices').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        select: {
            style: 'os'
        },
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "data": invoices,
        "order":[1,'asc'],
        "columns": [
        { "data": "FoundryInvoiceId", "title": "FoundryInvoiceId", "visible": false },
        { "data": "InvoiceNumber", "title": "Number", "class": "center" },
        { "data": "ScheduledPaymentDate", "name": "ScheduledPaymentDate", "title": "Scheduled Payment Date", "class": "center", render: dateFlag },
        { "data": "ActualPaymentDate", "name": "ActualPaymentDate", "title": "Actual Payment Date", "class": "center", render: dateFlag },
        { "data": "FoundryId", "title": "FoundryId", "visible": false },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "InvoiceAmount", "title": "Total", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 7,
            "title": "View",
            "width": "8%", "targets": 7,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewBtn'></span>"
        }]
    });

    $('#invoices tbody').on('click', '#viewBtn', function () {
        var invoice = invoicesTable.row($(this).parents('tr')).data();
        var childData = invoicesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (invoice != null) {
            data = invoice;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Shipment/FoundryInvoice?foundryInvoiceId=" + data.FoundryInvoiceId, target = "_self");
    });

    $("#unscheduled").change(function () {
        if (this.checked) {
            $('#fromDate').prop('readonly', true);
            $('#toDate').prop('readonly', true);
        }
        else {
            $('#fromDate').prop('readonly', false);
            $('#toDate').prop('readonly', false);
        }
    });

    $(document).on('click', '#searchBtn', function () {

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Shipment/SearchFoundryInvoices",
            data: {
                InvoiceNumber: $('#invoiceNumber').val(),
                FoundryId: $('#foundryId').val(),
                FromDate: $('#fromDate').val(),
                ToDate: $('#toDate').val(),
                Unscheduled: $('#unscheduled').prop("checked")
            },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                if (result.FoundryInvoices.length == 0) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                       '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                       '<strong>Warning!</strong>&nbsp;Can not find Foundry Invoices!</div>');
                }
                else {
                    $('#invoices').DataTable().clear().draw();
                    $('#invoices').DataTable().rows.add(result.FoundryInvoices); // Add new data
                    $('#invoices').DataTable().columns.adjust().draw();
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $(document).on('click', '#resetBtn', function () {
        $('#invoiceNumber').val("");
        $('#foundryId').val($("#foundryId option:first").val());
        $('#unscheduled').prop("checked", false).change();
        $('#fromDate').val(new Date().toLocaleDateString());
        $('#toDate').val(new Date().toLocaleDateString());
    });
});