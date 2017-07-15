$(document).ready(function () {

    function loadTables() {
        $('#buckets').DataTable().clear().draw();
        $('#buckets').DataTable().rows.add(buckets); // Add new data
        $('#buckets').DataTable().columns.adjust().draw();

        $('#debitMemos').DataTable().clear().draw();
        $('#debitMemos').DataTable().rows.add(debitMemos); // Add new data
        $('#debitMemos').DataTable().columns.adjust().draw();

        $('#creditMemos').DataTable().clear().draw();
        $('#creditMemos').DataTable().rows.add(creditMemos); // Add new data
        $('#creditMemos').DataTable().columns.adjust().draw();
    };

    var bucketsTable = $('#buckets').DataTable({
        "autoWidth": false,
        "width": "75%",
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "info": false,
        "ordering": false,
        "order":[2,'asc'],
        "data": buckets,
        "columns": [
        { "data": "BucketId", "title": "BucketId", "visible": false },
        { "data": "FoundryInvoiceId", "title": "FoundryInvoiceId", "visible": false },
        { "data": "BucketName", "title": "Bucket", "class": "center" },
        { "data": "BucketValue", "title": "Value", "class": "center" }
        ]
    });

    var debitMemosTable = $('#debitMemos').DataTable({
        "autoWidth": false,
        "width": "75%",
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "info": false,
        "ordering": false,
        "order": [1,'asc'],
        "data": debitMemos,
        "columns": [
        { "data": "DebitMemoId", "title": "DebitMemoId", "visible": false },
        { "data": "DebitMemoNumber", "title": "Number", "class": "center" },
        { "data": "DebitAmount", "title": "Amount", "class": "center" },
        { "title": "View", "class": "center" },
        ],
        "columnDefs": [{
            "targets": 3,
            "title": "View",
            "width": "8%", "targets": 3,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign' id='viewMemoBtn'></span>"
        }]
    });

    $('#debitMemos tbody').on('click', '#viewMemoBtn', function () {
        var debitMemo = debitMemosTable.row($(this).parents('tr')).data();
        var childData = debitMemosTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (debitMemo != null) {
            data = debitMemo;
        }
        else {
            data = childData;
        }
        window.location.href = '/SouthlandMetals/Operations/Shipment/DebitMemoDetail?debitMemoId=' + data.DebitMemoId + '';
    });

    var creditMemosTable = $('#creditMemos').DataTable({
        "autoWidth": false,
        "width": "75%",
        "lengthChange": false,
        "searching": false,
        "paging": false,
        "scrollY": 150,
        "info": false,
        "ordering": false,
        "order": [1,'asc'],
        "data": creditMemos,
        "columns": [
        { "data": "CreditMemoId", "title": "CreditMemoId", "visible": false },
        { "data": "CreditMemoNumber", "title": "Number", "class": "center" },
        { "data": "CreditAmount", "title": "Amount", "class": "center" }
        ]
    });

    getTotalDebits();
    getTotalCredits();
    getMemoDifference();

    $(document).on('click', '#addDebitMemo', function () {
        window.location.href = '/SouthlandMetals/Operations/Shipment/DebitMemoCreate';
    });

    function getTotalDebits() {
        var totalDebits = 0;
        if (debitMemos.length > 0) {
            for (var i = 0; i < debitMemos.length; i++) {
                totalDebits += debitMemos[i].DebitMemoTotal;
            }
        }

        totalDebits = CurrencyFormatted(totalDebits);

        $('#totalDebits').text(totalDebits);
    }

    function getTotalCredits() {
        var totalCredits = 0;
        if (creditMemos.length > 0) {
            for (var i = 0; i < creditMemos.length; i++) {
                totalCredits += creditMemos[i].CreditMemoTotal;
            }
        }

        totalCredits = CurrencyFormatted(totalCredits);

        $('#totalCredits').text(totalCredits);
    }

    function getMemoDifference() {
        var debitsTotal = $('#totalDebits').val();
        var creditsTotal = $('#totalCredits').val();
        var difference = parseFloat(debitsTotal) - parseFloat(creditsTotal);

        difference = CurrencyFormatted(difference);

        $('#memoDifference').text(difference);
    }
});