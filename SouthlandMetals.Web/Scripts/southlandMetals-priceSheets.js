$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#analysisLink').addClass("current-link");

    var priceSheetsTable = $('#priceSheets').DataTable({
        dom: 'Bfrt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        buttons: [
               {
                   text: 'All',
                   className: 'btn btn-sm btn-default',
                   action: function () {
                       getAllPriceSheets();
                   }
               },
               {
                   text: 'Quote',
                   className: 'btn btn-sm btn-warning',
                   action: function () {
                       getQuotePriceSheets();
                   }
               },
                {
                    text: 'Production',
                    className: 'btn btn-sm btn-danger',
                    action: function () {
                        getProductionPriceSheets();
                    }
                }
        ],
        "autoWidth": false,
        "pageLength": 20,
        "lengthChange": false,
        "order": [ [8, 'desc'], [1,'asc'] ],
        "data": priceSheets,
        "columns": [
        { "data": "PriceSheetId", "title": "PriceSheetId", "visible": false },
        { "data": "Number", "title": "Number", "class": "center" },
        { "data": "Date", "name": "Date", "title": "Date", "class": "center", render: dateFlag },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "RfqNumber", "title": "RFQNumber", "class": "center" },
        { "data": "ProjectMargin", "title": "ProjectMargin", "class": "center" },
        { "data": "WAF", "title": "WAF", "class": "center" },
        { "data": "Status", "title": "Status", "class": "center" },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "CreatedDate", "visible": false, render: dateFlag },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 9,
            "title": "View",
            "width": "8%", "targets": 9,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewBtn'></span>"
        }
        ]
    });

    $('#priceSheets tbody').on('click', '#viewBtn', function () {
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
            window.location.href = '/SouthlandMetals/Operations/Pricing/Detail?priceSheetId=' + data.PriceSheetId + '';
        else if (data.Status === "Production")
            window.location.href = '/SouthlandMetals/Operations/Pricing/Production?priceSheetId=' + data.PriceSheetId + '';
    });

    function getAllPriceSheets() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Pricing/GetAllPriceSheets",
            success: function (result) {
                $('#priceSheets').DataTable().clear().draw();
                if (result.PriceSheets != null) {
                    $('#priceSheets').DataTable().rows.add(result.PriceSheets); // Add new data
                    $('#priceSheets').DataTable().columns.adjust().draw();
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getQuotePriceSheets() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Pricing/GetQuotePriceSheets",
            success: function (result) {
                $('#priceSheets').DataTable().clear().draw();
                if (result.PriceSheets != null) {
                    $('#priceSheets').DataTable().rows.add(result.PriceSheets); // Add new data
                    $('#priceSheets').DataTable().columns.adjust().draw();
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getProductionPriceSheets() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Pricing/GetProductionPriceSheets",
            success: function (result) {
                $('#priceSheets').DataTable().clear().draw();
                if (result.PriceSheets != null) {
                    $('#priceSheets').DataTable().rows.add(result.PriceSheets); // Add new data
                    $('#priceSheets').DataTable().columns.adjust().draw();
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});