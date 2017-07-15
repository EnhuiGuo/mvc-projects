$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#patternLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var patternMaterialTable = $('#patternMaterial').DataTable({
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
        "data": patternMaterial,
        "columns": [
        { "data": "PatternMaterialId", "title": "PatternMaterialId", "visible": false },
        { "data": "PatternDescription", "title": "Description", "class": "center" },
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

    $('#addPattern').on('click', function () {
        _AddPatternMaterial();
    });

    $('#patternMaterial tbody').on('click', '#editBtn', function () {
        var patternMaterial = patternMaterialTable.row($(this).parents('tr')).data();
        var childData = patternMaterialTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (patternMaterial != null) {
            data = patternMaterial;
        }
        else {
            data = childData;
        }
        _EditPatternMaterial(data);
    });

    $(document).on('click', '#savePatternBtn', function () {
        event.preventDefault();

        if (!$("#addPatternMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addPatternMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PatternDescription: $('#patternDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddPatternMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#patternMaterial').DataTable().clear().draw();
                        $('#patternMaterial').DataTable().rows.add(result.PatternMaterials); // Add new data
                        $('#patternMaterial').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    $('#addPatternModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updatePatternBtn', function () {
        event.preventDefault();

        if (!$("#editPatternMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editPatternMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PatternMaterialId: $('#editPatternMaterialId').val(),
                PatternDescription: $('#editPatternDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditPatternMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#patternMaterial').DataTable().clear().draw();
                        $('#patternMaterial').DataTable().rows.add(result.PatternMaterials); // Add new data
                        $('#patternMaterial').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                        '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editPatternModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddPatternBtn').on('click', function () {
        $('#addPatternModal').modal('hide');
    });

    $('#cancelEditPatternBtn').on('click', function () {
        $('#editPatternModal').modal('hide');
    });
});

function _AddPatternMaterial() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddPatternMaterial",
        success: function (result) {

            $('#addPatternDiv').html('');
            $('#addPatternDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addPatternModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPatternMaterial(patternMaterial) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Part/_EditPatternMaterial",
        data: { "patternMaterialId": patternMaterial.PatternMaterialId },
        contentType: "application/json",
        success: function (result) {

            $('#editPatternDiv').html('');
            $('#editPatternDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (patternMaterial.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editPatternModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActivePatterns",
        success: function (result) {
            $('#patternMaterial').DataTable().clear().draw();
            $('#patternMaterial').DataTable().rows.add(result.PatternMaterials); // Add new data
            $('#patternMaterial').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactivePatterns",
        success: function (result) {
            $('#patternMaterial').DataTable().clear().draw();
            $('#patternMaterial').DataTable().rows.add(result.PatternMaterials); // Add new data
            $('#patternMaterial').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
