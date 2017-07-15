$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#rfqsLink').addClass("current-link");

    if (hasPriceSheet) {
        $('#editRfqBtn').hide();
        $('#deleteRfqBtn').hide();
        $('#editIcon').hide();
        $('#deleteIcon').hide();
    }

    $("#includeRaw").prop('checked', true);
    $("#includeMachined").prop('checked', true);

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show();
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    $('#isMachined').prop('checked', isMachined);
    $('#isirRequired').prop('checked', isirRequired);
    $('#sampleCastingAvailable').prop('checked', sampleCastingAvailable);
    $('#metalCertAvailable').prop('checked', metalCertAvailable);
    $('#cmtrRequired').prop('checked', cmtrRequired);
    $('#gaugingRequired').prop('checked', gaugingRequired);
    $('#testBarsRequired').prop('checked', testBarsRequired);

    ToEditPage = function () {
        window.location.href = '/SouthlandMetals/Operations/Rfq/Edit?rfqId=' + rfqId + '';
    }

    Print = function () {
        window.location.href = '/SouthlandMetals/Operations/Report/RfqReport?rfqId=' + rfqId + '';
    }

    $(document).on('click', '#sendEmailBtn', function () {

        event.preventDefault();

        var toEmail = $('#toEmailAddress').val();
        var copyEmail = $('#copyEmailAddress').val();

        var emailForm = document.getElementById("emailForm");
        var formData = new FormData(emailForm);

        formData.append("rfqId", rfqId);

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
                url: "/SouthlandMetals/Operations/Report/SendRfqEmail",
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

    ToPriceSheet = function () {
        if (priceSheet == "N/A") {
            if (isHold || isCanceled) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                              '<strong>Warning!</strong>&nbsp;This order has been canceled, unable to generate to quote.</div>');
            }
            else {
                window.location.href = '/SouthlandMetals/Operations/Pricing/Quote?rfqId=' + rfqId + '&includeRaw=' + $("#includeRaw").prop('checked') + '&includeMachined=' + $("#includeMachined").prop('checked') + '';
            }
        }
        else {
            window.location.href = '/SouthlandMetals/Operations/Pricing/Detail?priceSheetId=' + priceSheetId + '';
        }
    }

    $("#historyBtn").click(function () {
        window.location.href = '/SouthlandMetals/Operations/Rfq/History?rfqId=' + rfqId + '';
    });

    $("#deleteRfqBtn").click(function () {
        $.confirm({
            text: 'Are you sure you want to delete ' + $('#rfqNumber').text() + '?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Operations/Rfq/Delete",
                    data: { "rfqId": rfqId },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = '/SouthlandMetals/Operations/Rfq';
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

    var parts = allParts;

    var partTypeFlag = function (data, type, row) {
        if (row.IsRaw) {
            return '<span>Raw</span>';
        }
        else {
            return '<span>Machine</span>';
        }
    };

    var rfqPartsTable = $('#rfqParts').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [3, 'asc'],
        "data": parts,
        "columns": [
        { "data": "ProjectPartId", "title": "ProjectPartId", "visible": false },
        { "data": "PartId", "title": "PartId", "visible": false },
        { "data": "Type", "title": "Type", "class": "center", "render": partTypeFlag },
        { "data": "PartNumber", "title": "Number", "class": "center" },
        { "data": "RevisionNumber", "title": "Revision", "class": "center" },
        { "data": "PartDescription", "title": "Description", "class": "center" },
        { "data": "IsRaw", "title": "Raw", "visible": false },
        { "data": "IsMachined", "title": "Machine", "visible": false },
        { "data": "Weight", "title": "Weight", "class": "center", "render": weightFlag },
        { "data": "AnnualUsage", "title": "Usage", "class": "center" },
        { "data": "MaterialId", "title": "MaterialId", "visible": false },
        { "data": "MaterialDescription", "title": "Material", "class": "center" }
        ]
    });

    GetTotalWeight(parts);

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
    };
});