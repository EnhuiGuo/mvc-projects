$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#specificationLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var specificationMaterialTable = $('#specificationMaterial').DataTable({
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
        "data": specificationMaterial,
        "order":[1,'asc'],
        "columns": [
        { "data": "SpecificationMaterialId", "title": "SpecificationMaterialId", "visible": false },
        { "data": "SpecificationDescription", "title": "Description", "class": "center" },
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

    $('#addSpecification').on('click', function () {
        _AddSpecificationMaterial();
    });

    $('#specificationMaterial tbody').on('click', '#editBtn', function () {
        var specificationMaterial = specificationMaterialTable.row($(this).parents('tr')).data();
        var childData = specificationMaterialTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (specificationMaterial != null) {
            data = specificationMaterial;
        }
        else {
            data = childData;
        }
        _EditSpecificationMaterial(data);
    });

    $(document).on('click', '#saveSpecificationBtn', function () {
        event.preventDefault();

        if (!$("#addSpecMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addSpecMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                SpecificationDescription: $('#specificationDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddSpecificationMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#specificationMaterial').DataTable().clear().draw();
                        $('#specificationMaterial').DataTable().rows.add(result.SpecificationMaterials); // Add new data
                        $('#specificationMaterial').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addSpecificationModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateSpecificationBtn', function () {
        event.preventDefault();

        if (!$("#editSpecMaterialForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editSpecMaterialForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                SpecificationMaterialId: $('#editSpecificationMaterialId').val(),
                SpecificationDescription: $('#editSpecificationDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditSpecificationMaterial",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#specificationMaterial').DataTable().clear().draw();
                        $('#specificationMaterial').DataTable().rows.add(result.SpecificationMaterials); // Add new data
                        $('#specificationMaterial').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editSpecificationModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddSpecificationBtn').on('click', function () {
        $('#addSpecificationModal').modal('hide');
    });

    $('#cancelEditSpecificationBtn').on('click', function () {
        $('#editSpecificationModal').modal('hide');
    });
});

function _AddSpecificationMaterial() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddSpecificationMaterial",
        success: function (result) {

            $('#addSpecificationDiv').html('');
            $('#addSpecificationDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addSpecificationModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditSpecificationMaterial(specificationMaterial) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Part/_EditSpecificationMaterial",
        data: { "specificationMaterialId": specificationMaterial.SpecificationMaterialId },
        contentType: "application/json",
        success: function (result) {

            $('#editSpecificationDiv').html('');
            $('#editSpecificationDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (specificationMaterial.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editSpecificationModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActiveSpecifications",
        success: function (result) {
            $('#specificationMaterial').DataTable().clear().draw();
            $('#specificationMaterial').DataTable().rows.add(result.SpecificationMaterials); // Add new data
            $('#specificationMaterial').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactiveSpecifications",
        success: function (result) {
            $('#specificationMaterial').DataTable().clear().draw();
            $('#specificationMaterial').DataTable().rows.add(result.SpecificationMaterials); // Add new data
            $('#specificationMaterial').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
