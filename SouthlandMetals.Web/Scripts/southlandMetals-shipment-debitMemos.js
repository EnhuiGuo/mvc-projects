$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#debitMemoLink').addClass("current-link");

    var statusFlag = function (data, type, row) {
        if (row.IsOpen) {
            return '<span>Open</span>';
        }
        else if (row.Closed) {
            return '<span>Closed</span>';
        }
    }

    var debitMemosTable = $('#debitMemos').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "order": [[10, 'desc'], [1, 'asc']],
        "data": debitMemos,
        "columns": [
        { "data": "DebitMemoId", "title": "DebitMemoId", "visible": false },
        { "data": "DebitMemoNumber", "title": "Number", "class": "center" },
        { "data": "InvoiceNumber", "title": "Invoice", "class": "center" },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "DebitMemoDate", "name": "DebitMemoDate", "title": "Date", "class": "center", render: dateFlag },
        { "data": "DebitAmount", "title": "Amount", "class": "center" },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "CreditMemoId", "title": "CreditMemoId", "visible": false },
        { "data": "CreditMemoNumber", "title": "Credit Memo", "class": "center" },
        { "data": "Status", "title": "Status", "class": "center", "render": statusFlag },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "CreatedDate", "visible": false, render: dateFlag },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 11,
            "title": "View",
            "width": "8%", "targets": 11,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewBtn'></span>"
        },
         {
             "targets": 8,
             "render": function (data, type, row, meta) {
                 if (type === 'display') {
                     return $('<a class="red">')
                     .attr('href', '/SouthlandMetals/Operations/Shipment/CreditMemo?creditMemoId=' + row.CreditMemoId)
                     .text(data)
                     .wrap('<div></div>')
                     .parent()
                     .html();
                 }
                 else {
                     return data;
                 }
             }
         }
        ]
    });

    $('#debitMemos tbody').on('click', '#viewBtn', function () {
        var debitMemo = debitMemosTable.row($(this).parents('tr')).data();
        var childData = debitMemosTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (debitMemo != null) {
            data = debitMemo;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Operations/Shipment/DebitMemoDetail?debitMemoId=" + data.DebitMemoId, target = "_self");
    });

    $(document).on('click', '#searchBtn', function () {

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Shipment/SearchDebitMemos",
            data: {
                DebitMemoNumber: $('#debitMemoNumber').val(),
                RmaNumber: $('#rmaNumber').val(),
                InvoiceNumber: $('#invoiceNumber').val(),
                CustomerId: $('#customerId').val(),
                FoundryId: $('#foundryId').val(),
                PartNumber: $('#partNumber').val(),
                FromDate: $('#fromDate').val(),
                ToDate: $('#toDate').val()
            },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                $('#debitMemos').DataTable().clear().draw();
                $('#debitMemos').DataTable().rows.add(result.DebitMemos); // Add new data
                $('#debitMemos').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $(document).on('click', '#resetBtn', function () {
        $('#debitMemoNumber').val("");
        $('#rmaNumber').val("");
        $('#invoiceNumber').val("");
        $('#customerId').val($("#customerId option:first").val());
        $('#foundryId').val($("#foundryId option:first").val());
        $('#partNumber').val("");
        $('#fromDate').val(new Date().toLocaleDateString());
        $('#toDate').val(new Date().toLocaleDateString());
    });

    $(document).on('click', '#printOpenMemosBtn', function () {

        var printMemos = debitMemosTable.rows().data();

        if (printMemos.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
              'No DebitMemos</div>');
        }
        else {

            var form = $('<form></form>');

            form.attr("method", "post");
            form.attr("action", "/SouthlandMetals/Operations/Report/OpenDebitMemoReport");

            $.each(printMemos, function (i, memo) {
                if (memo.IsOpen) {
                    $.each(memo, function (key, value) {
                        var field = $('<input></input>');

                        field.attr("type", "hidden");
                        field.attr("name", "[" + i + "]." + key);
                        field.attr("value", value);

                        form.append(field);
                    });
                }
            });

            $(document.body).append(form);
            form.submit();
        }
    });

    $(document).on('click', '#addDebitMemo', function () {

        var foundryInvoiceId = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();

        window.location.href = '/SouthlandMetals/Operations/Shipment/CreateDebitMemo?foundryInvoiceId=' + foundryInvoiceId + '';
    });

    $('input[name=Status]').change(function () {
        if (this.value == 'All') {
            getAllDebitMemos();
        }
        else if (this.value == 'Open') {
            getOpenDebitMemos();
        }
        else if (this.value == 'Closed') {
            getClosedDebitMemos();
        }
    });

    function getAllDebitMemos() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/GetAllDebitMemos",
            success: function (result) {
                $('#debitMemos').DataTable().clear().draw();
                $('#debitMemos').DataTable().rows.add(result.DebitMemos); // Add new data
                $('#debitMemos').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getOpenDebitMemos() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/GetOpenDebitMemos",
            success: function (result) {
                $('#debitMemos').DataTable().clear().draw();
                $('#debitMemos').DataTable().rows.add(result.DebitMemos); // Add new data
                $('#debitMemos').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getClosedDebitMemos() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/GetClosedDebitMemos",
            success: function (result) {
                $('#debitMemos').DataTable().clear().draw();
                $('#debitMemos').DataTable().rows.add(result.DebitMemos); // Add new data
                $('#debitMemos').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});