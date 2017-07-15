$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#surchargeLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var surchageTable = $('#surcharge').DataTable({
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
        "order": [1,'asc'],
        "data": surcharge,
        "columns": [
        { "data": "SurchargeId", "title": "SurchargeId", "visible": false },
        { "data": "SurchargeDescription", "title": "Description", "class": "center" },
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

    $('#addSurcharge').on('click', function () {
        _AddSurcharge();
    });

    $('#surcharge tbody').on('click', '#editBtn', function () {
        var surcharge = surchageTable.row($(this).parents('tr')).data();
        var childData = surchageTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (surcharge != null) {
            data = surcharge;
        }
        else {
            data = childData;
        }
        _EditSurcharge(data);
    });

    $(document).on('click', '#saveSurchargeBtn', function () {
        event.preventDefault();

        if (!$("#addSurchargeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addSurchargeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                SurchargeDescription: $('#surchargeDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddSurcharge",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#surcharge').DataTable().clear().draw();
                        $('#surcharge').DataTable().rows.add(result.Surcharges); // Add new data
                        $('#surcharge').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addSurchargeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateSurchargeBtn', function () {
        event.preventDefault();

        if (!$("#editSurchargeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editSurchargeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                SurchargeId: $('#editSurchargeId').val(),
                SurchargeDescription: $('#editSurchargeDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditSurcharge",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#surcharge').DataTable().clear().draw();
                        $('#surcharge').DataTable().rows.add(result.Surcharges); // Add new data
                        $('#surcharge').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editSurchargeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddSurchargeBtn').on('click', function () {
        $('#addSurchargeModal').modal('hide');
    });

    $('#cancelEditSurchargeBtn').on('click', function () {
        $('#editSurchargeModal').modal('hide');
    });
});

function _AddSurcharge() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddSurcharge",
        success: function (result) {

            $('#addSurchargeDiv').html('');
            $('#addSurchargeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addSurchargeModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditSurcharge(surcharge) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditSurcharge",
        data: { "surchargeId": surcharge.SurchargeId },
        contentType: "application/json",
        success: function (result) {

            $('#editSurchargeDiv').html('');
            $('#editSurchargeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (surcharge.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editSurchargeModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveSurcharge",
        success: function (result) {
            $('#surcharge').DataTable().clear().draw();
            $('#surcharge').DataTable().rows.add(result.Surcharges); // Add new data
            $('#surcharge').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveSurcharge",
        success: function (result) {
            $('#surcharge').DataTable().clear().draw();
            $('#surcharge').DataTable().rows.add(result.Surcharges); // Add new data
            $('#surcharge').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};