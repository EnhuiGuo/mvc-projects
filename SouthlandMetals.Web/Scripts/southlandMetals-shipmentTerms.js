$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#shipmentTermLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var shipmentTermsTable = $('#shipmentTerms').DataTable({
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
        "order":[1,'asc'],
        "data": shipmentTerms,
        "columns": [
        { "data": "ShipmentTermId", "title": "ShipmentTermId", "visible": false },
        { "data": "ShipmentTermDescription", "title": "Description", "class": "center" },
        { "data": "IsActive", "title": "Active", "class": "center", "render": isActiveFlag },
        { "title": "Edit", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 3,
            "title": "Edit",
            "width": "8%", "targets": 3,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil glyphicon-large' id='editBtn'></span>"
        }]
    });

    $('#addShipmentTerm').on('click', function () {
        _AddShipmentTerm();
    });

    $('#shipmentTerms tbody').on('click', '#editBtn', function () {
        var shipmentTerm = shipmentTermsTable.row($(this).parents('tr')).data();
        var childData = shipmentTermsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (shipmentTerm != null) {
            data = shipmentTerm;
        }
        else {
            data = childData;
        }
        _EditShipmentTerm(data);
    });

    $(document).on('click', '#saveShipmentTermBtn', function () {
        event.preventDefault();

        if (!$("#addShipmentTermForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addShipmentTermForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                ShipmentTermDescription: $('#shipmentTermDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddShipmentTerm",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#shipmentTerms').DataTable().clear().draw();
                        $('#shipmentTerms').DataTable().rows.add(result.ShipmentTerms); // Add new data
                        $('#shipmentTerms').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addShipmentTermModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateShipmentTermBtn', function () {
        event.preventDefault();

        if (!$("#editShipmentTermForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editShipmentTermForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                ShipmentTermId: $('#editShipmentTermId').val(),
                ShipmentTermDescription: $('#editShipmentTermDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditShipmentTerm",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#shipmentTerms').DataTable().clear().draw();
                        $('#shipmentTerms').DataTable().rows.add(result.ShipmentTerms); // Add new data
                        $('#shipmentTerms').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editShipmentTermModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddShipmentTermBtn').on('click', function () {
        $('#addShipmentTermModal').modal('hide');
    });

    $('#cancelEditShipmentTermBtn').on('click', function () {
        $('#editShipmentTermModal').modal('hide');
    });
});

function _AddShipmentTerm() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddShipmentTerm",
        success: function (result) {

            $('#addShipmentTermDiv').html('');
            $('#addShipmentTermDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addShipmentTermModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditShipmentTerm(shipmentTerm) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditShipmentTerm",
        data: { "shipmentTermId": shipmentTerm.ShipmentTermId },
        contentType: "application/json",
        success: function (result) {

            $('#editShipmentTermDiv').html('');
            $('#editShipmentTermDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (shipmentTerm.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editShipmentTermModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function showActive() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/GetActiveShipmentTerms",
        success: function (result) {
            $('#shipmentTerms').DataTable().clear().draw();
            $('#shipmentTerms').DataTable().rows.add(result.ShipmentTerms); // Add new data
            $('#shipmentTerms').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveShipmentTerms",
        success: function (result) {
            $('#shipmentTerms').DataTable().clear().draw();
            $('#shipmentTerms').DataTable().rows.add(result.ShipmentTerms); // Add new data
            $('#shipmentTerms').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};