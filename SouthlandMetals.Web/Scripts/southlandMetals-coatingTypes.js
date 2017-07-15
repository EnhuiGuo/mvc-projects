$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#coatingTypeLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var coatingTypesTable = $('#coatingTypes').DataTable({
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
        "data": coatingTypes,
        "columns": [
        { "data": "CoatingTypeId", "title": "CoatingTypeId", "visible": false },
        { "data": "CoatingTypeDescription", "title": "Description", "class": "center" },
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

    $('#addCoatingType').on('click', function () {
        _AddCoatingType();
    });

    $('#coatingTypes tbody').on('click', '#editBtn', function () {
        var coatingType = coatingTypesTable.row($(this).parents('tr')).data();
        var childData = coatingTypesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (coatingType != null) {
            data = coatingType;
        }
        else {
            data = childData;
        }
        _EditCoatingType(data);
    });

    $(document).on('click', '#saveCoatingTypeBtn', function () {
        event.preventDefault();

        if (!$("#addCoatingTypeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addCoatingTypeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CoatingTypeDescription: $('#coatingTypeDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddCoatingType",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#coatingTypes').DataTable().clear().draw();
                        $('#coatingTypes').DataTable().rows.add(result.CoatingTypes); // Add new data
                        $('#coatingTypes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addCoatingTypeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateCoatingTypeBtn', function () {
        event.preventDefault();

        if (!$("#editCoatingTypeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editCoatingTypeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CoatingTypeId: $('#editCoatingTypeId').val(),
                CoatingTypeDescription: $('#editCoatingTypeDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditCoatingType",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#coatingTypes').DataTable().clear().draw();
                        $('#coatingTypes').DataTable().rows.add(result.CoatingTypes); // Add new data
                        $('#coatingTypes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                        '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editCoatingTypeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddCoatingTypeBtn').on('click', function () {
        $('#addCoatingTypeModal').modal('hide');
    });

    $('#cancelEditCoatingTypeBtn').on('click', function () {
        $('#editCoatingTypeModal').modal('hide');
    });
});

function _AddCoatingType() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddCoatingType",
        success: function (result) {

            $('#addCoatingTypeDiv').html('');
            $('#addCoatingTypeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addCoatingTypeModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditCoatingType(coatingType) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Part/_EditCoatingType",
        data: { "coatingTypeId": coatingType.CoatingTypeId },
        contentType: "application/json",
        success: function (result) {

            $('#editCoatingTypeDiv').html('');
            $('#editCoatingTypeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (coatingType.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editCoatingTypeModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveCoatingTypes",
        success: function (result) {
            $('#coatingTypes').DataTable().clear().draw();
            $('#coatingTypes').DataTable().rows.add(result.CoatingTypes); // Add new data
            $('#coatingTypes').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveCoatingTypes",
        success: function (result) {
            $('#coatingTypes').DataTable().clear().draw();
            $('#coatingTypes').DataTable().rows.add(result.CoatingTypes); // Add new data
            $('#coatingTypes').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
