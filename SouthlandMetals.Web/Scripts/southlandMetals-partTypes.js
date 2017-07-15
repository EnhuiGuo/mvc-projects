$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#partTypeLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var partTypesTable = $('#partTypes').DataTable({
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
        "data": partTypes,
        "columns": [
        { "data": "PartTypeId", "title": "PartTypeId", "visible": false },
        { "data": "PartTypeDescription", "title": "Description", "class": "center" },
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

    $('#addPartType').on('click', function () {
        _AddPartType();
    });

    $('#partTypes tbody').on('click', '#editBtn', function () {
        var partType = partTypesTable.row($(this).parents('tr')).data();
        var childData = partTypesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (partType != null) {
            data = partType;
        }
        else {
            data = childData;
        }
        _EditPartType(data);
    });

    $(document).on('click', '#savePartTypeBtn', function () {
        event.preventDefault();

        if (!$("#addPartTypeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addPartTypeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PartTypeDescription: $('#partTypeDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddPartType",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#partTypes').DataTable().clear().draw();
                        $('#partTypes').DataTable().rows.add(result.PartTypes); // Add new data
                        $('#partTypes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    $('#addPartTypeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updatePartTypeBtn', function () {
        event.preventDefault();

        if (!$("#editPartTypeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editPartTypeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PartTypeId: $('#editPartTypeId').val(),
                PartTypeDescription: $('#editPartTypeDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditPartType",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#partTypes').DataTable().clear().draw();
                        $('#partTypes').DataTable().rows.add(result.PartTypes); // Add new data
                        $('#partTypes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editPartTypeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddPartTypeBtn').on('click', function () {
        $('#addPartTypeModal').modal('hide');
    });

    $('#cancelEditPartTypeBtn').on('click', function () {
        $('#editPartTypeModal').modal('hide');
    });
});

function _AddPartType() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddPartType",
        success: function (result) {

            $('#addPartTypeDiv').html('');
            $('#addPartTypeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addPartTypeModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPartType(partType) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditPartType",
        data: { "partTypeId": partType.PartTypeId },
        contentType: "application/json",
        success: function (result) {

            $('#editPartTypeDiv').html('');
            $('#editPartTypeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (partType.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editPartTypeModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActivePartTypes",
        success: function (result) {
            $('#partTypes').DataTable().clear().draw();
            $('#partTypes').DataTable().rows.add(result.PartTypes); // Add new data
            $('#partTypes').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactivePartTypes",
        success: function (result) {
            $('#partTypes').DataTable().clear().draw();
            $('#partTypes').DataTable().rows.add(result.PartTypes); // Add new data
            $('#partTypes').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};