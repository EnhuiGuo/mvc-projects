$(document).ready(function () {

    var needScheduledInvoicesTable = $('#needScheduledInvoices').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "data": invoices,
        "order": [[2, 'asc'], [1, 'asc']],
        "columns": [
        { "data": "FoundryInvoiceId", "title": "FoundryInvoiceId", "visible": false },
        { "data": "InvoiceNumber", "title": "Number", "class": "center" },
        { "data": "CreateDate", "name": "CreateDate", "title": "Create Date", "class": "center", render: dateFlag },
        { "data": "FoundryId", "title": "FoundryId", "visible": false },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "InvoiceAmount", "title": "Total", "class": "center" },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 6,
            "title": "View",
            "width": "8%", "targets": 6,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='invoiceViewBtn'></span>"
        }]
    });

    $('#needScheduledInvoices tbody').on('click', '#invoiceViewBtn', function () {
        var invoice = needScheduledInvoicesTable.row($(this).parents('tr')).data();
        window.open("/SouthlandMetals/Accounting/Invoicing/EditFoundryInvoice?foundryInvoiceId=" + invoice.FoundryInvoiceId, target = "_self");
    });
});