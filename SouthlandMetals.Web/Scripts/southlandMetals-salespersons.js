$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");
    $('#salesLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var salespersonsTable = $('#salespersons').DataTable({
        dom: 'Bfrt' +
            "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        buttons: [
               {
                   text: 'Active',
                   className: 'btn btn-sm btn-success',
                   action: function () {
                       showActive();
                   }
               },
               {
                   text: 'Inactive',
                   className: 'btn btn-sm btn-danger',
                   action: function () {
                       showInactive();
                   }
               }
        ],
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "data": salespersons,
        "order":[1,'asc'],
        "columns": [
        { "data": "SalespersonId", "title": "SalespersonId", "visible": false },
        { "data": "SalespersonName", "title": "Name", "class": "center" },
        { "data": "Phone1", "title": "Phone", "class": "center" },
        { "data": "IsActive", "title": "Active", "class": "center", "render": isActiveFlag },
        { "title": "Customers", "class": "center" },
        { "title": "Detail", "class": "center" }
        ],
        "columnDefs": [
        {
            "targets": 3,
            "width": "8%", "targets": 3
        },
        {
            "targets": 4,
            "title": "Customers",
            "width": "8%", "targets": 4,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-usd glyphicon-large' id='customersBtn'></span>"
        },
        {
            "targets": 5,
            "title": "Detail",
            "width": "8%", "targets": 5,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailBtn'></span>"
        }]
    });

    $('#salespersons tbody').on('click', '#customersBtn', function () {
        var salesperson = salespersonsTable.row($(this).parents('tr')).data();
        var childData = salespersonsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (salesperson != null) {
            data = salesperson;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Administration/Sales/Customers?salespersonId=" + data.SalespersonId, target = "_self");
    });

    $('#salespersons tbody').on('click', '#detailBtn', function () {
        var salesperson = salespersonsTable.row($(this).parents('tr')).data();
        var childData = salespersonsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (salesperson != null) {
            data = salesperson;
        }
        else {
            data = childData;
        }

        window.open("/SouthlandMetals/Administration/Sales/Detail?salespersonId=" + data.SalespersonId, target = "_self");
    });
});

function showActive() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Sales/GetActiveSalespersons",
        success: function (result) {
            $('#salespersons').DataTable().clear().draw();
            $('#salespersons').DataTable().rows.add(result.Salespersons); // Add new data
            $('#salespersons').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function showInactive() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Sales/GetInactiveSalespersons",
        success: function (result) {
            $('#salespersons').DataTable().clear().draw();
            $('#salespersons').DataTable().rows.add(result.Salespersons); // Add new data
            $('#salespersons').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
