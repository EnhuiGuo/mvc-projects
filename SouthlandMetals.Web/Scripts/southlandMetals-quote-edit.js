$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#quotesLink').addClass("current-link");

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    $('#isMachined').prop('checked', isMachined);

    var selectableParts = [];

    if (isHold) {
        $('#viewHoldNotes').show();
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    getSelectablePartsByCustomer(customerId);

    var orderedFlag = function (data, type, row) {
        if (row.CustomerOrderId !== null) {
            return '<span>Order Placed</span>';
        }
    };

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
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "Weight", "title": "Weight", "class": "center", "render": weightFlag },
        { "data": "AnnualUsage", "title": "Usage", "class": "center" },
        { "data": "Price", "title": "Total Invoice Price", "class": "center", "render": dollarFlag },
        { "data": "Cost", "title": "Cost", "visible": false, "render": dollarFlag },
        { "data": "PatternPrice", "title": "Casting Tool Price", "class": "center", "render": dollarFlag },
        { "data": "PatternCost", "title": "PatternCost", "visible": false, "render": dollarFlag },
        { "data": "FixturePrice", "title": "Machine Fixture Price", "class": "center", "render": dollarFlag },
        { "data": "FixtureCost", "title": "FixtureCost", "visible": false, "render": dollarFlag },
        { "title": "Edit", "class": "center", "render": orderedFlag },
        { "title": "Delete", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 14,
            "title": "Edit",
            "width": "10%", "targets": 14,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editPartBtn'></span>"
        },
        {
            "targets": 15,
            "title": "Delete",
            "width": "10%", "targets": 15,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-trash' id='deletePartBtn'></span>"
        }]
    });

    $('#quoteParts tbody').on('click', '#editPartBtn', function () {
        var quotePart = quotePartsTable.row($(this).parents('tr')).data();
        var childData = quotePartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (quotePart != null) {
            data = quotePart;
        }
        else {
            data = childData;
        }
        _EditQuotePart(data);
    });

    $('#cancelEditQuotePartBtn').on('click', function () {
        $('#editQuotePartModal').modal('hide');
    });

    $(document).on('click', '#updateQuotePartBtn', function () {
        event.preventDefault();

        if (!$("#editQuotePartForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#editQuotePartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var projectPartId = $('#editProjectPartId').val();
            var partNumber = $('#editPartNumber').val();
            var revisionNumber = $('#editRevisionNumber').val();
            var partDescription = $('#editPartDescription').val();
            var editPartCustomerId = $('#editPartCustomerId').val();
            var weight = $('#editWeight').val();
            var annualUsage = $('#editAnnualUsage').val();
            var price = $('#editPrice').val();
            var patternPrice = $('#editPatternPrice').val();
            var fixturePrice = $('#editFixturePrice').val();
            var cost = $('#editCost').val();
            var patternCost = $('#editPatternCost').val();
            var fixtureCost = $('#editFixtureCost').val();

            $.each(parts, function (n, part) {
                if (part.PartNumber === partNumber) {
                    part.PartNumber = partNumber;
                    part.RevisionNumber = revisionNumber;
                    part.PartDescription = partDescription;
                    part.CustomerId = editPartCustomerId;
                    part.Weight = weight;
                    part.AnnualUsage = annualUsage;
                    part.Price = price;
                    part.PatternPrice = patternPrice;
                    part.FixturePrice = fixturePrice;
                    part.Cost = cost;
                    part.PatternCost = patternCost;
                    part.FixtureCost = fixtureCost;
                }
            });

            $('#quoteParts').DataTable().clear().draw();
            $('#quoteParts').DataTable().rows.add(parts); // Add new data
            $('#quoteParts').DataTable().columns.adjust().draw();

            $('#editQuotePartModal').modal('hide');
        }
    });

    $('#quoteParts tbody').on('click', '#deletePartBtn', function () {
        var quotePart = quotePartsTable.row($(this).parents('tr')).data();
        var childData = quotePartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (quotePart != null) {
            data = quotePart;
        }
        else {
            data = childData;
        }

        $.confirm({
            text: 'Are you sure you want to delete ' + data.PartNumber + ' ?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                quotePartsTable.row(quotePart).remove().draw();
                parts.splice(parts.indexOf(data.PartNumber), 1);
            },
            cancel: function (button) {

            }
        });
    });

    $('#addPart').click(function () {
        _AddQuotePart();
    });

    $(document).on('click', '#cancelAddQuotePartBtn', function () {
        $('#addQuotePartModal').modal('hide');
    });

    $(document).on('change', '#parts', function () {
        $(this).find(":selected").each(function () {
            var partId = $(this).val();

            getPartById(partId);
        });
    });

    $(document).on('click', '#saveQuotePartBtn', function () {
        event.preventDefault();

        if (!$("#addQuotePartForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#addQuotePartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {

            var partId = $('#parts').val();
            var partNumber = $('#partNumber').val();
            var partDescription = $('#partDescription').val();
            var revisionNumber = $('#revisionNumber').val();
            var addPartCustomerId = $('#addPartCustomerId').val();
            var weight = $('#weight').val();
            var annualUsage = $('#annualUsage').val();
            var price = $('#price').val();
            var patternPrice = $('#patternPrice').val();
            var fixturePrice = $('#fixturePrice').val();
            var cost = $('#cost').val();
            var patternCost = $('#patternCost').val();
            var fixtureCost = $('#fixtureCost').val();

            if (partNumber === "") {
                $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please enter a Part Number!</div>');
                $('.partError').show();
            }
            else {
                var existingPart = parts.filter(function (n) {
                    return n.PartNumber === partNumber;
                });

                if (existingPart.length > 0) {
                    $('.partError').append('<div><strong>Warning!</strong>&nbsp;Part is already included in Quote!</div>');
                    $('.partError').show();
                }
                else {
                    var invalidPart = false;

                    $.each(parts, function (n, part) {
                        if (part.CustomerId !== addPartCustomerId) {
                            invalidPart = true;
                        }
                    });

                    if (invalidPart) {
                        $('.partError').append('<div><strong>Warning!</strong>&nbsp;Unable to add part, Customer must match!</div>');
                        $('.partError').show();
                    }
                    else {
                        parts.push({
                            'ProjectPartId': null, 'PartId': partId, 'PartNumber': partNumber, 'RevisionNumber': revisionNumber,
                            'PartDescription': partDescription, 'CustomerId': addPartCustomerId, 'Weight': weight, 'AnnualUsage': annualUsage, 'Price': price, 'Cost': cost,
                            'PatternPrice': patternPrice, 'PatternCost': patternCost, 'FixturePrice': fixturePrice, 'FixtureCost': fixtureCost,
                            'CustomerOrderId': null, 'IsRaw': isRaw, 'IsMachined': isMachined
                        });

                        $('#quoteParts').DataTable().clear().draw();
                        $('#quoteParts').DataTable().rows.add(parts); // Add new data
                        $('#quoteParts').DataTable().columns.adjust().draw();

                        invalidPart = false;

                        $('#addQuotePartModal').modal('hide');
                    }
                }

                invalidPart = false;

                $('#addQuotePartModal').modal('hide');
            }
        }
    });

    $(document).on('click', '#viewHoldNotes', function () {
        _ViewHoldNotes();
    });

    $(document).on('click', '#viewCancelNotes', function () {
        _ViewCancelNotes();
    });

    $("input[name='Status']").click(function () {
        var selectedVal = "";
        var selected = $("input[name='Status']:checked");
        if (selected.length > 0) {
            selectedVal = selected.val();
        }

        if (selectedVal === 'On Hold') {
            _AddHoldNotes();
        }
        else if (selectedVal === 'Canceled') {
            _AddCancelNotes();
        }
    });

    $("#isMachined").change(function () {
        if (this.checked) {
            $('#machining').val("Included");
        }
        else {
            $('#machining').val("Not Included");
        }
    });

    $(document).on('click', '#saveHoldNoteBtn', function () {

        holdExpirationDate = $('#holdExpirationDate').val();
        holdNotes = $('#holdNotes').val();

        if (holdExpirationDate === "" || holdNotes === "") {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();

            event.preventDefault();

            $('#addQuoteHoldNoteForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#addQuoteHoldNoteForm textarea[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            holdExpirationDate = null;
            holdNotes = null;
        }
        else {
            $('#addHoldNotesModal').modal('hide');
        }
    });

    $(document).on('click', '#cancelAddHoldNoteBtn', function () {
        holdExpirationDate = null;
        holdNotes = null;
        $('#addHoldNotesModal').modal('hide');
    });

    $(document).on('click', '#saveCancelNoteBtn', function () {

        cancelNotes = $('#cancelNotes').val();
        canceledDate = new Date();

        if (cancelNotes === "") {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();

            event.preventDefault();

            $('#addQuoteCancelNoteForm textarea[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            $('#addCancelNotesModal').modal('hide');
        }
    });

    $(document).on('click', '#cancelAddCancelNoteBtn', function () {
        cancelNotes = null;
        canceledDate = null;
        $('#addCancelNotesModal').modal('hide');
    });

    $(document).on('click', '#updateQuoteBtn', function () {
        event.preventDefault();

        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be quoted!</div>');
        }
        else {
            if (!$("#editQuoteForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#editQuoteForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editQuoteForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editQuoteForm textarea[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var selectedVal = "";
                var selected = $("input[name='Status']:checked");
                if (selected.length > 0) {
                    selectedVal = selected.val();
                }

                var model = {
                    QuoteId: quoteId,
                    ProjectId: projectId,
                    RfqId: rfqId,
                    PriceSheetId: priceSheetId,
                    QuoteNumber: quoteNumber,
                    QuoteDate: $('#quoteDate').val(),
                    Validity: $('#validity').val(),
                    ContactName: $('#contactName').val(),
                    CustomerId: customerId,
                    CustomerAddressId: customerAddressId,
                    ContactCopy: $('#contactCopy').val(),
                    ShipmentTermId: $('#shipmentTermId').val(),
                    PaymentTermId: $('#paymentTermId').val(),
                    MinimumShipment: $('#minimumShipment').val(),
                    ToolingTermDescription: $('#toolingTermDescription').val(),
                    SampleLeadTime: $('#sampleLeadTime').val(),
                    ProductionLeadTime: $('#productionLeadTime').val(),
                    MaterialId: materialId,
                    CoatingTypeId: $('#coatingTypeId').val(),
                    HtsNumberId: $('#htsNumberId').val(),
                    IsMachined: $('#isMachined').prop('checked'),
                    Notes: $('#notes').val(),
                    Status: selectedVal,
                    IsOpen: $('#open').prop("checked"),
                    IsHold: $('#hold').prop("checked"),
                    IsCanceled: $('#canceled').prop("checked"),
                    HoldExpirationDate: holdExpirationDate,
                    HoldNotes: holdNotes,
                    CancelNotes: cancelNotes,
                    CanceledDate: canceledDate,
                    QuoteParts: parts
                };

                $.ajax({
                    type: 'PUT',
                    url: "/SouthlandMetals/Operations/Quote/Edit",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            if (result.IsHold) {
                                signalR.server.sendRoleNotification(currentUser + " has put Quote " + quoteNumber + " til " + holdExpirationDate, "Admin");
                            }

                            if (result.IsCanceled) {
                                signalR.server.sendRoleNotification(currentUser + " has canceled Quote " + quoteNumber, "Admin");
                            }
                            selectableParts = [];
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
            }
        }
    });

    $(document).on('click', '#cancelUpdateQuoteBtn', function () {
        selectableParts = [];
        window.history.back();
    });

    function getSelectablePartsByCustomer(customerId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/Customer/GetSelectablePartsByCustomer",
            data: { "customerId": customerId },
            contentType: "application/json",
            success: function (result) {

                selectableParts = [];

                $.each(result, function (n, part) {
                    selectableParts.push({ Value: part.Value, Text: part.Text });
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getPartById(partId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Quote/GetQuotePartByPart",
            data: { "partId": partId },
            contentType: "application/json",
            success: function (result) {

                isRaw = result.IsRaw;
                isMachined = result.IsMachined;

                $('#partNumber').val(result.PartNumber);
                $('#partDescription').val(result.PartDescription);
                $('#addPartCustomerId').val(result.CustomerId);
                $('#revisionNumber').val(result.RevisionNumber);
                $('#weight').val(result.Weight);
                $('#annualUsage').val(result.AnnualUsage);
                $('#price').val(result.Price);
                $('#patternPrice').val(result.PatternPrice);
                $('#fixturePrice').val(result.FixturePrice);
                $('#cost').val(result.Cost);
                $('#patternCost').val(result.PatternCost);
                $('#fixtureCost').val(result.FixtureCost);
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function _AddQuotePart() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Quote/_AddQuotePart",
            success: function (result) {

                $('#addQuotePartDiv').html('');
                $('#addQuotePartDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#parts').empty();

                $.each(selectableParts, function (n, part) {
                    $("#parts").append($("<option />").val(part.Value).text(part.Text));
                });

                $('#addQuotePartModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _EditQuotePart(quotePart) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Quote/_EditQuotePart",
            success: function (result) {

                $('#editQuotePartDiv').html('');
                $('#editQuotePartDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#editPartNumber').val(quotePart.PartNumber);
                $('#editPartDescription').val(quotePart.PartDescription);
                $('#editPartCustomerId').val(quotePart.CustomerId);
                $('#editRevisionNumber').val(quotePart.RevisionNumber);
                $('#editWeight').val(quotePart.Weight);
                $('#editAnnualUsage').val(quotePart.AnnualUsage);
                $('#editPrice').val(quotePart.Price);
                $('#editPatternPrice').val(quotePart.PatternPrice);
                $('#editFixturePrice').val(quotePart.FixturePrice);
                $('#editCost').val(quotePart.Cost);
                $('#editPatternCost').val(quotePart.PatternCost);
                $('#editFixtureCost').val(quotePart.FixtureCost);

                $('#editQuotePartModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _AddHoldNotes() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_AddHoldNotes",
            success: function (result) {

                $('#addHoldNotesDiv').html('');
                $('#addHoldNotesDiv').html(result);

                $('.datepicker').datepicker({
                    format: 'm/dd/yyyy',
                    orientation: 'down'
                }).on('changeDate', function (e) {
                    $(this).datepicker('hide');
                });

                $('.successAlert').hide();
                $('.errorAlert').hide();

                $('#addHoldNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _AddCancelNotes() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Notes/_AddCancelNotes",
            success: function (result) {

                $('#addCancelNotesDiv').html('');
                $('#addCancelNotesDiv').html(result);

                $('.successAlert').hide();
                $('.errorAlert').hide();

                $('#addCancelNotesModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

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