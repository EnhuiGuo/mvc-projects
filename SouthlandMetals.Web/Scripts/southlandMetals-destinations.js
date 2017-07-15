$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseAdminShipments').addClass("in");
    $('#collapseAdminShipments').attr("aria-expanded", "true");

    $('#adminShipmentsLink').addClass("category-current-link");
    $('#destinationLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var destinationTable = $('#destinations').DataTable({
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
        "order": [1, 'asc'],
        "data": destinations,
        "columns": [
        { "data": "DestinationId", "title": "DestinationId", "visible": false },
        { "data": "DestinationDescription", "title": "Description", "class": "center" },
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

    $('#addDestination').on('click', function () {
        _AddDestination();
    });

    $('#destinations tbody').on('click', '#editBtn', function () {
        var destination = destinationTable.row($(this).parents('tr')).data();
        var childData = destinationTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (destination != null) {
            data = destination;
        }
        else {
            data = childData;
        }
        _EditDestination(data);
    });

    $(document).on('click', '#saveDestinationBtn', function () {
        event.preventDefault();

        if (!$("#addDestinationForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addDestinationForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                DestinationDescription: $('#destinationDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Shipment/AddDestination",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {

                    if (result.Success) {
                        $('#destinations').DataTable().clear().draw();
                        $('#destinations').DataTable().rows.add(result.Destinations); // Add new data
                        $('#destinations').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addDestinationModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateDestinationBtn', function () {
        event.preventDefault();

        if (!$("#editDestinationForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editDestinationForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                DestinationId: $('#editDestinationId').val(),
                DestinationDescription: $('#editDestinationDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Shipment/EditDestination",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {

                    if (result.Success) {
                        $('#destinations').DataTable().clear().draw();
                        $('#destinations').DataTable().rows.add(result.Destinations); // Add new data
                        $('#destinations').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editDestinationModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddDestinationBtn').on('click', function () {
        $('#addDestinationModal').modal('hide');
    });

    $('#cancelEditDestinationBtn').on('click', function () {
        $('#editDestinationModal').modal('hide');
    });
});

function _AddDestination() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Shipment/_AddDestination",
        success: function (result) {

            $('#addDestinationDiv').html('');
            $('#addDestinationDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addDestinationModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditDestination(destination) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Shipment/_EditDestination",
        data: { "destinationId": destination.DestinationId },
        contentType: "application/json",
        success: function (result) {

            $('#editDestinationDiv').html('');
            $('#editDestinationDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (destination.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editDestinationModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Shipment/GetActiveDestinations",
        success: function (result) {
            $('#destinations').DataTable().clear().draw();
            $('#destinations').DataTable().rows.add(result.Destinations); // Add new data
            $('#destinations').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Shipment/GetInactiveDestinations",
        success: function (result) {
            $('#destinations').DataTable().clear().draw();
            $('#destinations').DataTable().rows.add(result.Destinations); // Add new data
            $('#destinations').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
