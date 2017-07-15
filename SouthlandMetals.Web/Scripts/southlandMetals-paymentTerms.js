$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseParts').addClass("in");
    $('#collapseParts').attr("aria-expanded", "true");

    $('#adminPartsLink').addClass("category-current-link");
    $('#paymentTermLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var paymentTermsTable = $('#paymentTerms').DataTable({
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
        "data": paymentTerms,
        "columns": [
        { "data": "PaymentTermId", "title": "PaymentTermId", "visible": false },
        { "data": "PaymentTermDescription", "title": "Description", "class": "center" },
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
        }
        ]
    });

    $('#addPaymentTerm').on('click', function () {
        _AddPaymentTerm();
    });

    $('#paymentTerms tbody').on('click', '#editBtn', function () {
        var paymentTerm = paymentTermsTable.row($(this).parents('tr')).data();
        var childData = paymentTermsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (paymentTerm != null) {
            data = paymentTerm;
        }
        else {
            data = childData;
        }
        _EditPaymentTerm(data);
    });

    $(document).on('click', '#savePaymentTermBtn', function () {
        event.preventDefault();

        if (!$("#addPaymentTermForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addPaymentTermForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PaymentTermDescription: $('#paymentTermDescription').val(),
                IsActive: $('#isActive').val()
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Part/AddPaymentTerm",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#paymentTerms').DataTable().clear().draw();
                        $('#paymentTerms').DataTable().rows.add(result.PaymentTerms); // Add new data
                        $('#paymentTerms').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addPaymentTermModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updatePaymentTermBtn', function () {
        event.preventDefault();

        if (!$("#editPaymentTermForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editPaymentTermForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                PaymentTermId: $('#editPaymentTermId').val(),
                PaymentTermDescription: $('#editPaymentTermDescription').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Part/EditPaymentTerm",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#paymentTerms').DataTable().clear().draw();
                        $('#paymentTerms').DataTable().rows.add(result.PaymentTerms); // Add new data
                        $('#paymentTerms').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    $('#editPaymentTermModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddPaymentTermBtn').on('click', function () {
        $('#addPaymentTermModal').modal('hide');
    });

    $('#cancelEditPaymentTermBtn').on('click', function () {
        $('#editPaymentTermModal').modal('hide');
    });
});

function _AddPaymentTerm() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_AddPaymentTerm",
        success: function (result) {

            $('#addPaymentTermDiv').html('');
            $('#addPaymentTermDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addPaymentTermModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPaymentTerm(paymentTerm) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Part/_EditPaymentTerm",
        data: { "paymentTermId": paymentTerm.PaymentTermId },
        contentType: "application/json",
        success: function (result) {

            $('#editPaymentTermDiv').html('');
            $('#editPaymentTermDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (paymentTerm.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editPaymentTermModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Part/GetActivePaymentTerms",
        success: function (result) {
            $('#paymentTerms').DataTable().clear().draw();
            $('#paymentTerms').DataTable().rows.add(result.PaymentTerms); // Add new data
            $('#paymentTerms').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Part/GetInactivePaymentTerms",
        success: function (result) {
            $('#paymentTerms').DataTable().clear().draw();
            $('#paymentTerms').DataTable().rows.add(result.PaymentTerms); // Add new data
            $('#paymentTerms').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};