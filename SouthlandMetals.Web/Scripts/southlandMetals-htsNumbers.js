$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#htsNumberLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    percentageFlag = function (data) {
        return data + "%";
    }

    var htsNumbersTable = $('#htsNumbers').DataTable({
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
        "data": htsNumbers,
        "columns": [
        { "data": "HtsNumberId", "title": "HtsNumberId", "visible": false },
        { "data": "HtsNumberDescription", "title": "Description", "class": "center" },
        { "data": "HtsNumberDutyRate", "title": "Duty Rate", "class": "center", "render": percentageFlag },
        { "data": "IsActive", "title": "Active", "class": "center", "render": isActiveFlag },
        { "title": "Edit", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 4,
            "title": "Edit",
            "width": "8%", "targets": 4,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editBtn'></span>"
        }]
    });

    $('#addHtsNumber').on('click', function () {
        _AddHtsNumber();
    });

    $('#htsNumbers tbody').on('click', '#editBtn', function () {
        var htsNumber = htsNumbersTable.row($(this).parents('tr')).data();
        var childData = htsNumbersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (htsNumber != null) {
            data = htsNumber;
        }
        else {
            data = childData;
        }
        _EditHtsNumber(data);
    });

    $(document).on('click', '#saveHtsNumberBtn', function () {
        event.preventDefault();

        if (!$("#addHtsNumberForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addHtsNumberForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                HtsNumberDescription: $('#htsNumberDescription').val(),
                HtsNumberDutyRate: $('#htsNumberDutyRate').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddHtsNumber",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#htsNumbers').DataTable().clear().draw();
                        $('#htsNumbers').DataTable().rows.add(result.HtsNumbers); // Add new data
                        $('#htsNumbers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addHtsNumberModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateHtsNumberBtn', function () {
        event.preventDefault();

        if (!$("#editHtsNumberForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editHtsNumberForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                HtsNumberId: $('#editHtsNumberId').val(),
                HtsNumberDescription: $('#editHtsNumberDescription').val(),
                HtsNumberDutyRate: $('#editHtsNumberDutyRate').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditHtsNumber",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#htsNumbers').DataTable().clear().draw();
                        $('#htsNumbers').DataTable().rows.add(result.HtsNumbers); // Add new data
                        $('#htsNumbers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editHtsNumberModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddHtsNumberBtn').on('click', function () {
        $('#addHtsNumberModal').modal('hide');
    });

    $('#cancelEditHtsNumberBtn').on('click', function () {
        $('#editHtsNumberModal').modal('hide');
    });
});

function _AddHtsNumber() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddHtsNumber",
        success: function (result) {

            $('#addHtsNumberDiv').html('');
            $('#addHtsNumberDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addHtsNumberModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditHtsNumber(htsNumber) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditHtsNumber",
        data: { "htsNumberId": htsNumber.HtsNumberId },
        contentType: "application/json",
        success: function (result) {

            $('#editHtsNumberDiv').html('');
            $('#editHtsNumberDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (htsNumber.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editHtsNumberModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveHtsNumbers",
        success: function (result) {
            $('#htsNumbers').DataTable().clear().draw();
            $('#htsNumbers').DataTable().rows.add(result.HtsNumbers); // Add new data
            $('#htsNumbers').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveHtsNumbers",
        success: function (result) {
            $('#htsNumbers').DataTable().clear().draw();
            $('#htsNumbers').DataTable().rows.add(result.HtsNumbers); // Add new data
            $('#htsNumbers').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
