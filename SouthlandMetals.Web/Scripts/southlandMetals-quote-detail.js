$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#quotesLink').addClass("current-link");

    $('#isMachined').prop('checked', isMachined);
    $('#isMachined').prop('disabled', true);

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show();
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    $("#canceled").change(function () {
        if (this.checked) {
            $('#editQuoteBtn').hide();
            $('#deleteQuoteBtn').hide();
            $('#editIcon').hide();
            $('#deleteIcon').hide();
        }
    });

    $("#complete").change(function () {
        if (this.checked) {
            $('#editQuoteBtn').hide();
            $('#deleteQuoteBtn').hide();
            $('#editIcon').hide();
            $('#deleteIcon').hide();
        }
    });

    var parts = allParts;

    var weightFlag = function (data, type, row) {
        return data + " lbs"
    };

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };

    var quotePartsTable = $('#quoteParts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [2, 'asc'],
        "data": parts,
        "columns": [
        { "data": "ProjectPartId", "title": "ProjectPartId", "visible": false },
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "RevisionNumber", "title": "Revision", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "Weight", "title": "Weight", "class": "center", "render": weightFlag },
        { "data": "AnnualUsage", "title": "Usage", "class": "center" },
        { "data": "Price", "title": "Total Invoice Price", "class": "center", "render": dollarFlag },
        { "data": "Cost", "title": "Cost", "visible": false, "render": dollarFlag },
        { "data": "PatternPrice", "title": "Casting Tool Price", "class": "center", "render": dollarFlag },
        { "data": "PatternCost", "title": "PatternCost", "visible": false, "render": dollarFlag },
        { "data": "FixturePrice", "title": "Machine Fixture Price", "class": "center", "render": dollarFlag },
        { "data": "FixtureCost", "title": "FixtureCost", "visible": false, "render": dollarFlag }
        ]
    });

    EditQuote = function () {
        window.location.href = '/SouthlandMetals/Operations/Quote/Edit?quoteId=' + quoteId;
    }

    $(document).on('click', '#deleteQuoteBtn', function () {
        $.confirm({
            text: 'Are you sure you want to delete this Quote?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/Quote/Delete",
                    data: { "quoteId": quoteId },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = '/SouthlandMetals/Operations/Quote';
                        }
                        else {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                               '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                        }
                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });
            },
            cancel: function (button) {

            }
        });
    });

    PrintQuote = function () {
        window.location.href = '/SouthlandMetals/Operations/Report/QuoteReport?quoteId=' + quoteId + '';
    }

    HistQuote = function () {
        if (quoteId != "") {
            window.location.href = '/SouthlandMetals/Operations/Quote/History?quoteId=' + quoteId + '';
        }
    }

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("quoteId", quoteId);

        if (toEmail == "") {
            $('#emailError').html('<div class="alert alert-danger alert-dismissable">' +
               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
        }
        else if (!isValidEmailAddress(toEmail) || (copyEmail != "" && !isValidEmailAddress(copyEmail))) {
            $('#emailError').html('<div class="alert alert-danger alert-dismissable">' +
               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
               '<strong>Warning!</strong>&nbsp;Email Address is not right!</div>');
        }
        else {

            $.ajax({
                type: 'POST',
                url: "/SouthlandMetals/Operations/Report/SendQuoteEmail",
                dataType: "json",
                contentType: false,
                data: formData,
                cache: false,
                processData: false,
                success: function (result) {
                    if (result.Success) {
                        $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                           '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                           '<strong>Success!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                           '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                           '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });

            $('#emailModal').modal('toggle');
        }
    });

    GoProductionPriceSheet = function () {
        if (projectId != null && priceSheetId != null) {
            window.location.href = '/SouthlandMetals/Operations/Pricing/ConvertToProduction?priceSheetId=' + priceSheetId + '&quoteId=' + quoteId + '';
        }
        else {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                               '<strong>Warning!</strong>&nbsp; this quote does not have price sheet</div>');
        }
    }

    $(document).on('click', '#viewHoldNotes', function () {
        _ViewHoldNotes();
    });

    $(document).on('click', '#viewCancelNotes', function () {
        _ViewCancelNotes();
    });

    function _ViewHoldNotes() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_ViewHoldNotes",
            data: {
                "HoldNotes": holdNotes,
                "HoldExpirationDateStr": holdExpirationDate
            },
            contentType: "application/json",
            success: function (result) {

                $('#viewHoldNotesDiv').html('');
                $('#viewHoldNotesDiv').html(result);

                $('#viewHoldNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _ViewCancelNotes() {

        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_ViewCancelNotes",
            data: {
                "CancelNotes": cancelNotes,
                "CanceledDateStr": canceledDate
            },
            contentType: "application/json",
            success: function (result) {

                $('#viewCancelNotesDiv').html('');
                $('#viewCancelNotesDiv').html(result);

                $('#viewCancelNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    }
});