$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseAdminShipments').addClass("in");
    $('#collapseAdminShipments').attr("aria-expanded", "true");

    $('#adminShipmentsLink').addClass("category-current-link");
    $('#portLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var portsTable = $('#ports').DataTable({
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
        "data": ports,
        "columns": [
        { "data": "PortId", "title": "PortId", "visible": false },
        { "data": "PortName", "title": "Name", "class": "center" },
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

    $('#addPort').on('click', function () {
        _AddPort();
    });

    $('#ports tbody').on('click', '#editBtn', function () {
        var port = portsTable.row($(this).parents('tr')).data();
        var childData = portsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (port != null) {
            data = port;
        }
        else {
            data = childData;
        }
        _EditPort(data);
    });

    $(document).on('click', '#savePortBtn', function () {
        event.preventDefault();

        if (!$("#addPortForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addPortForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PortName: $('#portName').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Shipment/AddPort",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#ports').DataTable().clear().draw();
                        $('#ports').DataTable().rows.add(result.Ports); // Add new data
                        $('#ports').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addPortModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updatePortBtn', function () {
        event.preventDefault();

        if (!$("#editPortForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editPortForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PortId: $('#editPortId').val(),
                PortName: $('#editPortName').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Shipment/EditPort",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#ports').DataTable().clear().draw();
                        $('#ports').DataTable().rows.add(result.Ports); // Add new data
                        $('#ports').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editPortModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddPortBtn').on('click', function () {
        $('#addPortModal').modal('hide');
    });

    $('#cancelEditPortBtn').on('click', function () {
        $('#editPortModal').modal('hide');
    });
});

function _AddPort() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Shipment/_AddPort",
        success: function (result) {

            $('#addPortDiv').html('');
            $('#addPortDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addPortModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPort(port) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Shipment/_EditPort",
        data: { "portId": port.PortId },
        contentType: "application/json",
        success: function (result) {

            $('#editPortDiv').html('');
            $('#editPortDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (port.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editPortModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Shipment/GetActivePorts",
        success: function (result) {
            $('#ports').DataTable().clear().draw();
            $('#ports').DataTable().rows.add(result.Ports); // Add new data
            $('#ports').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Shipment/GetInactivePorts",
        success: function (result) {
            $('#ports').DataTable().clear().draw();
            $('#ports').DataTable().rows.add(result.Ports); // Add new data
            $('#ports').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
