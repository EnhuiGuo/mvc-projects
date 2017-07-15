$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");
    $('#accountCodesLink').addClass("current-link");

    var accountCodesTable = $('#accountCodes').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 10,
        "lengthChange": false,
        "data": accountCodes,
        "order": [2, 'asc'],
        "columns": [
        { "data": "AccountCodeId", "title": "AccountCodeId", "visible": false },
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "BucketName", "title": "Bucket", "class": "center" },
        { "data": "Description", "title": "Description", "class": "center" },
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

    $('#accountCodes tbody').on('click', '#editBtn', function () {
        var accountCode = accountCodesTable.row($(this).parents('tr')).data();
        var childData = accountCodesTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (accountCode != null) {
            data = accountCode;
        }
        else {
            data = childData;
        }
        _EditAccountCode(data.AccountCodeId);
    });

    $(document).on('click', '#updateAccountCodeBtn', function () {
        event.preventDefault();

        if (!$("#editAccountCodeForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editAccountCodeForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#editAccountCodeForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                AccountCodeId: $('#editAccountCodeId').val(),
                CustomerId: $('#editCustomerId').val(),
                BucketId: $('#editBucketId').val(),
                Description: $('#editAccountCodeDescription').val()
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/AccountCode/EditAccountCode",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#accountCodes').DataTable().clear().draw();
                        $('#accountCodes').DataTable().rows.add(result.AccountCodes); // Add new data
                        $('#accountCodes').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editAccountCodeModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelEditAccountCodeBtn').on('click', function () {
        $('#editAccountCodeModal').modal('hide');
    });
});

function _EditAccountCode(accountCodeId) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/AccountCode/_EditAccountCode",
        data: { "accountCodeId": accountCodeId },
        success: function (result) {

            $('#editAccountCodeDiv').html('');
            $('#editAccountCodeDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#editAccountCodeModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
