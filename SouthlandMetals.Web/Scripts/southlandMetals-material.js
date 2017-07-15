$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#materialLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var materialTable = $('#material').DataTable({
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
        "data": material,
        "columns": [
        { "data": "MaterialId", "title": "MaterialId", "visible": false },
        { "data": "MaterialDescription", "title": "Description", "class": "center" },
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

    $('#addMaterial').on('click', function () {
        _AddMaterial();
    });

    $('#material tbody').on('click', '#editBtn', function () {
        var material = materialTable.row($(this).parents('tr')).data();
        var childData = materialTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (material != null) {
            data = material;
        }
        else {
            data = childData;
        }
        _EditMaterial(data);
    });

    $(document).on('click', '#saveMaterialBtn', function () {
        event.preventDefault();

        if (!$("#addMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                MaterialDescription: $('#materialDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#material').DataTable().clear().draw();
                        $('#material').DataTable().rows.add(result.Materials); // Add new data
                        $('#material').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addMaterialModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateMaterialBtn', function () {
        event.preventDefault();

        if (!$("#editMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                MaterialId: $('#editMaterialId').val(),
                MaterialDescription: $('#editMaterialDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#material').DataTable().clear().draw();
                        $('#material').DataTable().rows.add(result.Materials); // Add new data
                        $('#material').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    $('#editMaterialModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddMaterialBtn').on('click', function () {
        $('#addMaterialModal').modal('hide');
    });

    $('#cancelEditMaterialBtn').on('click', function () {
        $('#editMaterialModal').modal('hide');
    });
});

function _AddMaterial() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddMaterial",
        success: function (result) {

            $('#addMaterialDiv').html('');
            $('#addMaterialDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addMaterialModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditMaterial(material) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Part/_EditMaterial",
        data: { "materialId": material.MaterialId },
        contentType: "application/json",
        success: function (result) {

            $('#editMaterialDiv').html('');
            $('#editMaterialDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (material.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editMaterialModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveMaterial",
        success: function (result) {
            $('#material').DataTable().clear().draw();
            $('#material').DataTable().rows.add(result.Materials); // Add new data
            $('#material').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveMaterial",
        success: function (result) {
            $('#material').DataTable().clear().draw();
            $('#material').DataTable().rows.add(result.Materials); // Add new data
            $('#material').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
