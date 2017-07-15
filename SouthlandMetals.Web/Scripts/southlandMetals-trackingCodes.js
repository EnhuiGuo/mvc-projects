$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#trackingCodeLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var trackingCodesTable = $('#trackingCodes').DataTable({
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
        "order":[2,'asc'],
        "data": trackingCodes,
        "columns": [
        { "data": "TrackingCodeId", "title": "TrackingCodeId", "visible": false },
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "TrackingCodeDescription", "title": "Code", "class": "center" },
        { "data": "IsActive", "title": "Active", "class": "center", "render": isActiveFlag },
        { "title": "Edit", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 5,
            "title": "Edit",
            "width": "8%", "targets": 5,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil glyphicon-large' id='editBtn'></span>"
        }]
    });

    $('#addTrackingCode').on('click', function () {
        _AddTrackingCode();
    });

    $('#trackingCodes tbody').on('click', '#editBtn', function () {
        var trackingCode = trackingCodesTable.row($(this).parents('tr')).data();
        var childData = trackingCodesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (trackingCode != null) {
            data = trackingCode;
        }
        else {
            data = childData;
        }
        _EditTrackingCode(data);
    });

    $(document).on('click', '#saveTrackingCodeBtn', function () {
        event.preventDefault();

        if (!$("#addTrackingCodeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addTrackingCodeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#addTrackingCodeForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CustomerId: $('#customerId').val(),
                TrackingCodeDescription: $('#trackingCodeDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddTrackingCode",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#trackingCodes').DataTable().clear().draw();
                        $('#trackingCodes').DataTable().rows.add(result.TrackingCodes); // Add new data
                        $('#trackingCodes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    $('#addTrackingCodeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateTrackingCodeBtn', function () {
        event.preventDefault();

        if (!$("#editTrackingCodeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editTrackingCodeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#editTrackingCodeForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                TrackingCodeId: $('#editTrackingCodeId').val(),
                CustomerId: $('#editCustomerId').val(),
                TrackingCodeDescription: $('#editTrackingCodeDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditTrackingCode",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#trackingCodes').DataTable().clear().draw();
                        $('#trackingCodes').DataTable().rows.add(result.TrackingCodes); // Add new data
                        $('#trackingCodes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editTrackingCodeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelTrackingCodeBtn').on('click', function () {
        $('#addTrackingCodeModal').modal('hide');
    });

    $('#cancelEditTrackingCodeBtn').on('click', function () {
        $('#editTrackingCodeModal').modal('hide');
    });
});

function _AddTrackingCode() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddTrackingCode",
        success: function (result) {

            $('#addTrackingCodeDiv').html('');
            $('#addTrackingCodeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addTrackingCodeModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditTrackingCode(trackingCode) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditTrackingCode",
        data: { "trackingCodeId": trackingCode.TrackingCodeId },
        contentType: "application/json",
        success: function (result) {

            $('#editTrackingCodeDiv').html('');
            $('#editTrackingCodeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (trackingCode.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editTrackingCodeModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveTrackingCodes",
        success: function (result) {
            $('#trackingCodes').DataTable().clear().draw();
            $('#trackingCodes').DataTable().rows.add(result.TrackingCodes); // Add new data
            $('#trackingCodes').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveTrackingCodes",
        success: function (result) {
            $('#trackingCodes').DataTable().clear().draw();
            $('#trackingCodes').DataTable().rows.add(result.TrackingCodes); // Add new data
            $('#trackingCodes').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
