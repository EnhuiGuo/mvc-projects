$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#partStatusLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var partStatesTable = $('#partStates').DataTable({
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
        "data": partStates,
        "columns": [
        { "data": "PartStatusId", "title": "PartStatusId", "visible": false },
        { "data": "PartStatusDescription", "title": "Description", "class": "center" },
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

    $('#addPartStatus').on('click', function () {
        _AddPartStatus();
    });

    $('#partStates tbody').on('click', '#editBtn', function () {
        var partStatus = partStatesTable.row($(this).parents('tr')).data();
        var childData = partStatesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (partStatus != null) {
            data = partStatus;
        }
        else {
            data = childData;
        }
        _EditPartStatus(data);
    });

    $(document).on('click', '#savePartStatusBtn', function () {
        event.preventDefault();

        if (!$("#addPartStatusForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addPartStatusForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PartStatusDescription: $('#partStatusDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddPartStatus",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#partStates').DataTable().clear().draw();
                        $('#partStates').DataTable().rows.add(result.PartStates); // Add new data
                        $('#partStates').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addPartStatusModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updatePartStatusBtn', function () {
        event.preventDefault();

        if (!$("#editPartStatusForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editPartStatusForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PartStatusId: $('#editPartStatusId').val(),
                PartStatusDescription: $('#editPartStatusDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditPartStatus",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#partStates').DataTable().clear().draw();
                        $('#partStates').DataTable().rows.add(result.PartStates); // Add new data
                        $('#partStates').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editPartStatusModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddPartStatusBtn').on('click', function () {
        $('#addPartStatusModal').modal('hide');
    });

    $('#cancelEditPartStatusBtn').on('click', function () {
        $('#editPartStatusModal').modal('hide');
    });
});

function _AddPartStatus() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddPartStatus",
        success: function (result) {

            $('#addPartStatusDiv').html('');
            $('#addPartStatusDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addPartStatusModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPartStatus(partStatus) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditPartStatus",
        data: { "partStatusId": partStatus.PartStatusId },
        contentType: "application/json",
        success: function (result) {

            $('#editPartStatusDiv').html('');
            $('#editPartStatusDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (partStatus.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editPartStatusModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActivePartStates",
        success: function (result) {
            $('#partStates').DataTable().clear().draw();
            $('#partStates').DataTable().rows.add(result.PartStates); // Add new data
            $('#partStates').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactivePartStates",
        success: function (result) {
            $('#partStates').DataTable().clear().draw();
            $('#partStates').DataTable().rows.add(result.PartStates); // Add new data
            $('#partStates').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};