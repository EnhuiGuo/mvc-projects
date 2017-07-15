$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseAdminShipments').addClass("in");
    $('#collapseAdminShipments').attr("aria-expanded", "true");

    $('#adminShipmentsLink').addClass("category-current-link");
    $('#vesselLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var vesselsTable = $('#vessels').DataTable({
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
        "data": vessels,
        "columns": [
        { "data": "VesselId", "title": "VesselId", "visible": false },
        { "data": "VesselName", "title": "Name", "class": "center" },
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

    $('#addVessel').on('click', function () {
        _AddVessel();
    });

    $('#vessels tbody').on('click', '#editBtn', function () {
        var vessel = vesselsTable.row($(this).parents('tr')).data();
        var childData = vesselsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (vessel != null) {
            data = vessel;
        }
        else {
            data = childData;
        }
        _EditVessel(data);
    });

    $(document).on('click', '#saveVesselBtn', function () {
        event.preventDefault();

        if (!$("#addVesselForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addVesselForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                VesselName: $('#vesselName').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Shipment/AddVessel",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#vessels').DataTable().clear().draw();
                        $('#vessels').DataTable().rows.add(result.Vessels); // Add new data
                        $('#vessels').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                        '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addVesselModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateVesselBtn', function () {
        event.preventDefault();

        if (!$("#editVesselForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editVesselForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                VesselId: $('#editVesselId').val(),
                VesselName: $('#editVesselName').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Shipment/EditVessel",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#vessels').DataTable().clear().draw();
                        $('#vessels').DataTable().rows.add(result.Vessels); // Add new data
                        $('#vessels').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                        '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editVesselModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddVesselBtn').on('click', function () {
        $('#addVesselModal').modal('hide');
    });

    $('#cancelEditVesselBtn').on('click', function () {
        $('#editVesselModal').modal('hide');
    });
});

function _AddVessel() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Shipment/_AddVessel",
        success: function (result) {

            $('#addVesselDiv').html('');
            $('#addVesselDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addVesselModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditVessel(vessel) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Shipment/_EditVessel",
        data: { "vesselId": vessel.VesselId },
        contentType: "application/json",
        success: function (result) {

            $('#editVesselDiv').html('');
            $('#editVesselDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (vessel.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editVesselModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Shipment/GetActiveVessels",
        success: function (result) {
            $('#vessels').DataTable().clear().draw();
            $('#vessels').DataTable().rows.add(result.Vessels); // Add new data
            $('#vessels').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Shipment/GetInactiveVessels",
        success: function (result) {
            $('#vessels').DataTable().clear().draw();
            $('#vessels').DataTable().rows.add(result.Vessels); // Add new data
            $('#vessels').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
