$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseAdminShipments').addClass("in");
    $('#collapseAdminShipments').attr("aria-expanded", "true");

    $('#adminShipmentsLink').addClass("category-current-link");
    $('#carrierLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var carriersTable = $('#carriers').DataTable({
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
        "data": carriers,
        "columns": [
        { "data": "CarrierId", "title": "CarrierId", "visible": false },
        { "data": "CarrierName", "title": "Name", "class": "center" },
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

    $('#addCarrier').on('click', function () {
        _AddCarrier();
    });

    $('#carriers tbody').on('click', '#editBtn', function () {
        var carrier = carriersTable.row($(this).parents('tr')).data();
        var childData = carriersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (carrier != null) {
            data = carrier;
        }
        else {
            data = childData;
        }
        _EditCarrier(data);
    });

    $(document).on('click', '#saveCarrierBtn', function () {
        event.preventDefault();

        if (!$("#addCarrierForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addCarrierForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CarrierName: $('#carrierName').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Shipment/AddCarrier",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#carriers').DataTable().clear().draw();
                        $('#carriers').DataTable().rows.add(result.Carriers); // Add new data
                        $('#carriers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addCarrierModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateCarrierBtn', function () {
        event.preventDefault();

        if (!$("#editCarrierForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editCarrierForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CarrierId: $('#editCarrierId').val(),
                CarrierName: $('#editCarrierName').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Shipment/EditCarrier",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {

                    if (result.Success) {
                        $('#carriers').DataTable().clear().draw();
                        $('#carriers').DataTable().rows.add(result.Carriers); // Add new data
                        $('#carriers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                        '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editCarrierModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddCarrierBtn').on('click', function () {
        $('#addCarrierModal').modal('hide');
    });

    $('#cancelEditCarrierBtn').on('click', function () {
        $('#editCarrierModal').modal('hide');
    });
});

function _AddCarrier() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Shipment/_AddCarrier",
        success: function (result) {

            $('#addCarrierDiv').html('');
            $('#addCarrierDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addCarrierModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditCarrier(carrier) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Shipment/_EditCarrier",
        data: { "carrierId": carrier.CarrierId },
        contentType: "application/json",
        success: function (result) {

            $('#editCarrierDiv').html('');
            $('#editCarrierDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (carrier.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editCarrierModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Shipment/GetActiveCarriers",
        success: function (result) {
            $('#carriers').DataTable().clear().draw();
            $('#carriers').DataTable().rows.add(result.Carriers); // Add new data
            $('#carriers').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Shipment/GetInactiveCarriers",
        success: function (result) {
            $('#carriers').DataTable().clear().draw();
            $('#carriers').DataTable().rows.add(result.Carriers); // Add new data
            $('#carriers').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
