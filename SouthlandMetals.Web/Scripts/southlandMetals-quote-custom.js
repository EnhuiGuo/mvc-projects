$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#quotesLink').addClass("current-link");

    var parts = [];
    var selectableParts = [];
    var projectPartId = null;
    var customerId = null;
    var isRaw = false;
    var isMachined = false;

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
        { "title": "Edit", "class": "center" },
        { "title": "Delete", "class": "center" }
        ],
        "columnDefs": [{
            "targets": 14,
            "title": "Edit",
            "width": "8%", "targets": 14,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editPartBtn'></span>"
        },
        {
            "targets": 15,
            "title": "Delete",
            "width": "8%", "targets": 15,
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
                parts.splice(parts.indexOf(data.ProjectPartId), 1);
            },
            cancel: function (button) {

            }
        });
    });

    $('#addPart').click(function () {
        if ($('#customerId option:selected').text() === "--Select Customer--") {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 '<strong>Warning!</strong>&nbsp;Please select a Customer!</div>');
        }
        else {
            _AddQuotePart();

            getSelectablePartsByCustomer(customerId);
        }
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
            var addPartCustomerId = $('#addPartCustomerId').val();
            var addPartFoundryId = $('#addPartFoundryId').val();
            var revisionNumber = $('#revisionNumber').val();
            var weight = $('#weight').val();
            var annualUsage = $('#annualUsage').val();
            var price = $('#price').val();
            var patternPrice = $('#patternPrice').val();
            var fixturePrice = $('#fixturePrice').val();
            var cost = $('#cost').val();
            var patternCost = $('#patternCost').val();
            var fixtureCost = $('#fixtureCost').val();

            if (partNumber === "" && partId == null) {
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
                            'ProjectPartId': projectPartId, 'PartId': partId, 'PartNumber': partNumber, 'RevisionNumber': revisionNumber,
                            'PartDescription': description, 'CustomerId': addPartCustomerId, 'FoundryId': addPartFoundryId, 'Weight': weight, 'AnnualUsage': annualUsage, 'Price': price, 'Cost': cost,
                            'PatternPrice': patternPrice, 'PatternCost': patternCost, 'FixturePrice': fixturePrice, 'FixtureCost': fixtureCost,'IsRaw': isRaw, 'IsMachined': isMachined
                        });

                        invalidPart = false;

                        $('#addPartModal').modal('hide');

                        $('#quoteParts').DataTable().clear().draw();
                        $('#quoteParts').DataTable().rows.add(parts); // Add new data
                        $('#quoteParts').DataTable().columns.adjust().draw();
                    }
                }

                invalidPart = false;

                $('#addPartModal').modal('hide');
            }
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

    CreateQuote = function () {
        event.preventDefault();

        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be quoted!</div>');
        }
        else {
            if (!$("#customQuoteForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#customQuoteForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#customQuoteForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#customQuoteForm textarea[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    ProjectId: $('#projectId').val(),
                    CustomerId: customerId,
                    CustomerAddressId: $('#customerAddressId').val(),
                    IsMachined: $('#isMachined').prop('checked'),
                    QuoteNumber: quoteNumber,
                    QuoteDateStr: $('#quoteDate').val(),
                    Validity: $('#validity').val(),
                    ContactName: $('#contactName').val(),
                    ContactCopy: $('#contactCopy').val(),
                    ShipmentTermId: $('#shipmentTermId').val(),
                    PaymentTermId: $('#paymentTermId').val(),
                    MinimumShipment: $('#minimumShipment').val(),
                    ToolingTermDescription: $('#toolingTermDescription').val(),
                    SampleLeadTime: $('#sampleLeadTime').val(),
                    ProductionLeadTime: $('#productionLeadTime').val(),
                    MaterialId: $('#materialId').val(),
                    CoatingTypeId: $('#coatingTypeId').val(),
                    HtsNumberId: $('#htsNumberId').val(),
                    Notes: $('#notes').val(),
                    IsOpen: true,
                    QuoteParts: parts
                };

                $.ajax({
                    type: 'POST',
                    url: "/SouthlandMetals/Operations/Quote/Create",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(model),
                    success: function (result) {
                        if (result.Success) {
                            selectableParts = [];

                            $.confirm({
                                text: 'Create Success, Do you want print this Quote?',
                                dialogClass: "modal-confirm",
                                confirmButton: "Yes",
                                confirmButtonClass: 'btn btn-sm',
                                cancelButton: "No",
                                cancelButtonClass: 'btn btn-sm btn-default',
                                closeIcon: false,
                                confirm: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Report/QuoteReport?quoteId=' + result.ReferenceId + '';
                                },
                                cancel: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Quote';
                                }
                            });
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
    };

    $(document).on('click', '#cancelSaveQuoteBtn', function () {
        selectableParts = [];
        window.history.back();
    });

    $('#customerId').change(function () {
        customerId = $('#customerId').val();
        if (customerId == "--Select Customer--") {
            $("#contactName").val("");
            $('#customerAddress').val("");
        }
        else {
            $.ajax({
                type: 'GET',
                url: "/SouthlandMetals/Administration/Customer/GetCustomer",
                data: { "customerId": customerId },
                contentType: "application/json",
                success: function (result) {
                    $('#contactName').val(result.ContactName);

                    $('#shipmentTermId').empty();

                    $.each(result.SelectableShipmentTerms, function (n, term) {
                        $("#shipmentTermId").append($("<option />").val(term.Value).text(term.Text));
                    });
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });

            getAddressesByCustomer(customerId);
        }
    });

    function getAddressesByCustomer(customerId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/Customer/GetAddressesByCustomer",
            data: { "customerId": customerId },
            contentType: "application/json",
            success: function (result) {

                $('#customerAddressId').empty();

                $.each(result, function (n, term) {
                    $("#customerAddressId").append($("<option />").val(term.Value).text(term.Text));
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

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
                $('#addPartFoundryId').val(result.FoundryId);
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

                projectPartId = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();

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
});
