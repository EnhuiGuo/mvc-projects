$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#rfqsLink').addClass("current-link");

    getSelectablePartsByCustomerAndFoundry(customerId, foundryId);

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    $('#isMachined').prop('checked', isMachined);
    $('#isirRequired').prop('checked', isirRequired);
    $('#sampleCastingAvailable').prop('checked', sampleCastingAvailable);
    $('#metalCertAvailable').prop('checked', metalCertAvailable);
    $('#cmtrRequired').prop('checked', cmtrRequired);
    $('#gaugingRequired').prop('checked', gaugingRequired);
    $('#testBarsRequired').prop('checked', testBarsRequired);

    if (isHold) {
        $('#viewHoldNotes').show();
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    var selectableParts = [];

    getSelectableCoatingTypes();
    getSelectableSpecificationMaterials();

    $('#totalWeight').text(0);

    var partTypeFlag = function (data, type, row) {
        if (row.IsRaw) {
            return '<span>Raw</span>';
        }
        else {
            return '<span>Machine</span>';
        }
    };

    var weightFlag = function (data, type, row) {
        return data + " lbs"
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
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "FoundryId", "title": "FoundryId", "visible": false },
        { "data": "IsRaw", "title": "Raw", "visible": false },
        { "data": "IsMachined", "title": "Machine", "visible": false },
        { "data": "Weight", "title": "Weight", "class": "center", "render": weightFlag },
        { "data": "AnnualUsage", "title": "Usage", "class": "center" },
        { "data": "MaterialId", "title": "MaterialId", "visible": false },
        { "data": "MaterialDescription", "title": "Material", "class": "center" },
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

    GetTotalWeight(parts);

    $('#rfqParts tbody').on('click', '#editPartBtn', function () {
        var rfqPart = rfqPartsTable.row($(this).parents('tr')).data();
        var childData = rfqPartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfqPart != null) {
            data = rfqPart;
        }
        else {
            data = childData;
        }
        _EditRfqPart(data);
    });

    $('#cancelEditRfqPartBtn').on('click', function () {
        $('#editRfqPartModal').modal('hide');
    });

    $(document).on('click', '#updateRfqPartBtn', function () {
        event.preventDefault();

        if (!$("#editRfqPartForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#editRfqPartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#editRfqPartForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });

            if (!$('#editIsRawBtn').is(':checked') && !$('#editIsMachinedBtn').is(':checked')) {
                $('#editIsRawBtn').wrap("<span></span>").parent().css({ border: "1px red solid" });
                $('#editIsMachinedBtn').wrap("<span></span>").parent().css({ border: "1px red solid" });
            }
        }
        else {
            var projectPartId = $('#editProjectPartId').val();
            var partNumber = $('#editPartNumber').val();
            var revisionNumber = $('#editRevisionNumber').val();
            var partDescription = $('#editPartDescription').val();
            var editPartCustomerId = $('#editPartCustomerId').val();
            var editPartFoundryId = $('#editPartFoundryId').val();
            var isRaw = $('#editIsRawBtn').prop('checked');
            var isMachined = $('#editIsMachinedBtn').prop('checked');
            var weight = $('#editWeight').val();
            var annualUsage = $('#editAnnualUsage').val();
            var drawing = null;
            var file = $('#editDrawing').get(0).files[0];

            $.each(parts, function (n, part) {
                if (part.PartNumber == partNumber) {
                    part.PartNumber = partNumber;
                    part.RevisionNumber = revisionNumber;
                    part.PartDescription = partDescription;
                    part.CustomerId = customerId;
                    part.FoundryId = foundryId;
                    part.IsRaw = isRaw;
                    part.IsMachined = isMachined;
                    part.Weight = weight;
                    part.AnnualUsage = annualUsage;
                }
            });

            if (file) {
                var fileName = file.name.substr(0, file.name.lastIndexOf('.')) || file.name;
                var reader = new FileReader();

                reader.onload = function (e) {
                    var contents = e.target.result;
                    contents = contents.substr(contents.indexOf(",") + 1);
                    drawing = {
                        RevisionNumber: fileName,
                        Type: file.type,
                        Length: file.size,
                        Content: contents,
                        IsLatest: true,
                        IsRaw: isRaw,
                        IsMachined: isMachined,
                        IsActive: false
                    }

                    $.each(parts, function (i, part) {
                        if (part.PartNumber == partNumber)
                            part.Drawing = drawing;
                    });
                }

                reader.readAsDataURL(file);
            }

            $('#rfqParts').DataTable().clear().draw();
            $('#rfqParts').DataTable().rows.add(parts); // Add new data
            $('#rfqParts').DataTable().columns.adjust().draw();

            $('#editRfqPartModal').modal('hide');
        }
    });

    $('#rfqParts tbody').on('click', '#deletePartBtn', function () {
        var rfqPart = rfqPartsTable.row($(this).parents('tr')).data().data();
        var childData = rfqPartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfqPart != null) {
            data = rfqPart;
        }
        else {
            data = childData;
        }

        $.confirm({
            text: 'Are you sure you want to delete ' + data.RawPartNumber + ' ?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {

                rfqPartsTable.row(rfqPart).remove().draw();
                parts.splice(parts.indexOf(data.ProjectPartId), 1);
                GetTotalWeight(parts);

                if (parts.length < 1) {
                    materialId = null;
                    materialDescription = null;
                    $('#materialId option:selected').val(null);
                    $('#materialId option:selected').text(null);
                }
            },
            cancel: function (button) {

            }
        });
    });

    $('#addPart').click(function () {
        if ($('#customerId option:selected').text() === "--Select Customer--" || $('#foundryId option:selected').text() === "--Select Foundry--") {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 '<strong>Warning!</strong>&nbsp;Please select a Customer and Foundry!</div>');
        }
        else {
            _AddRfqPart();
        }
    });

    $(document).on('click', '#fileSubmitBtn', function () {
        var fileName = $('#drawing').val().split('\\').pop();
        fileName = fileName.substr(0, fileName.lastIndexOf('.')) || fileName;
        $('#revisionNumber').val(fileName);
    });

    $(document).on('click', '#editFileSubmitBtn', function () {
        var fileName = $('#editDrawing').val().split('\\').pop();
        fileName = fileName.substr(0, fileName.lastIndexOf('.')) || fileName;
        $('#editRevisionNumber').val(fileName);
    });


    $(document).on('click', '#cancelAddRfqPartBtn', function () {
        projectPartId = null;
        partId = null;
        revisionNumber = null;

        if (parts.length < 1) {
            materialId = null;
            materialDescription = null;
            $('#materialId option:selected').val(null);
            $('#materialId option:selected').text(null);
        }

        $('#addRfqPartModal').modal('hide');
    });

    $(document).on('change', '#parts', function () {
        $(this).find(":selected").each(function () {
            var partId = $(this).val();

            getRfqPartByPart(partId);
        });
    });

    $(document).on('click', '#saveRfqPartBtn', function () {
        event.preventDefault();

        if (!$("#addRfqPartForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#addRfqPartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#addRfqPartForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });

            if (!$('#isRawBtn').is(':checked') && !$('#isMachinedBtn').is(':checked')) {
                $('#isRawBtn').wrap("<span></span>").parent().css({ border: "1px red solid" });
                $('#isMachinedBtn').wrap("<span></span>").parent().css({ border: "1px red solid" });
            }
        }
        else {
            var isRaw = $('#isRawBtn').prop('checked');
            var isNew = true;
            var isMachined = $('#isMachinedBtn').prop('checked');
            var partNumber = $('#partNumber').val();
            var weight = $('#weight').val();
            var partDescription = $('#partDescription').val();
            var customerId = $('#customerId').val();
            var foundryId = $('#foundryId').val();
            var addPartCustomerId = $('#addPartCustomerId').val();
            var addPartFoundryId = $('#addPartFoundryId').val();
            var annualUsage = $('#annualUsage').val();
            var partId = $('#parts').val();
            var revisionNumber = $('#revisionNumber').val();
            var partTypeId = $('#partTypeId').val();
            var partStatusId = $('#partStatusId').val();
            var destinationId = $('#destinationId').val();
            var surchargeId = $('#surchargeId').val();
            var subFoundryId = $('#subFoundryId').val();
            var drawing = null;
            var file = $('#drawing').get(0).files[0];

            if (partId != "") {
                isNew = false;
            }

            materialId = $('#materialId option:selected').val();
            materialDescription = $('#materialId option:selected').text();

            $('#materialId option:selected').val(materialId);
            $('#materialId').prop('disabled', true);

            if (partNumber === "") {
                $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please enter a Part Number!</div>');
                $('.partError').show();
            }
            else {
                var existingPart = parts.filter(function (n) {
                    return n.PartNumber === partNumber;
                });

                if (existingPart.length > 0) {
                    $('.partError').append('<div><strong>Warning!</strong>&nbsp;Part is already included in RFQ!</div>');
                    $('.partError').show();
                }
                else {
                    var invalidPart = false;

                    $.each(parts, function (n, part) {
                        if (part.CustomerId !== addPartCustomerId || part.FoundryId !== addPartFoundryId) {
                            invalidPart = true;
                        }
                    });

                    if (invalidPart) {
                        $('.partError').append('<div><strong>Warning!</strong>&nbsp;Unable to add part, Customer and Foundry must match!</div>');
                        $('.partError').show();
                    }
                    else {
                        parts.push({
                            'ProjectPartId': null, 'PartId': partId, 'Type': isRaw, 'PartNumber': partNumber, 'RevisionNumber': revisionNumber, 'PartDescription': partDescription,
                            'CustomerId': addPartCustomerId, 'FoundryId': addPartFoundryId, 'IsRaw': isRaw, 'IsMachined': isMachined,
                            'Weight': weight, 'AnnualUsage': annualUsage, 'MaterialId': materialId, 'MaterialDescription': materialDescription,
                            'PartTypeId': partTypeId, 'PartStatusId': partStatusId, 'DestinationId': destinationId, 'SurchargeId': surchargeId,
                            'IsNew': isNew, 'SubFoundryId': subFoundryId
                        });

                        if (file) {
                            var fileName = file.name.substr(0, file.name.lastIndexOf('.')) || file.name;
                            var reader = new FileReader();

                            reader.onload = function (e) {
                                var contents = e.target.result;
                                contents = contents.substr(contents.indexOf(",") + 1);
                                drawing = {
                                    RevisionNumber: fileName,
                                    Type: file.type,
                                    Length: file.size,
                                    Content: contents,
                                    IsLatest: true,
                                    IsRaw: isRaw,
                                    IsMachined: isMachined,
                                    IsActive: false
                                }

                                $.each(parts, function (i, part) {
                                    if (part.PartNumber == partNumber)
                                        part.Drawing = drawing;
                                });
                            }
                            reader.readAsDataURL(file);
                        }

                        $('#addPartModal').modal('hide');

                        $('#rfqParts').DataTable().clear().draw();
                        $('#rfqParts').DataTable().rows.add(parts); // Add new data
                        $('#rfqParts').DataTable().columns.adjust().draw();

                        GetTotalWeight(parts);
                    }

                    invalidPart = false;

                    $('#addRfqPartModal').modal('hide');
                }
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

    $(document).on('click', '#saveHoldNoteBtn', function () {

        holdExpirationDate = $('#holdExpirationDate').val();
        holdNotes = $('#holdNotes').val();

        if (holdExpirationDate === "" || holdNotes === "") {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();

            event.preventDefault();

            $('#addRfqHoldNoteForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
            $('#addRfqHoldNoteForm textarea[required]').each(function () {
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

            $('#addRfqCancelNoteForm textarea[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
            cancelNotes = null;
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

    $(document).on('click', '#updateRfqBtn', function () {
        event.preventDefault();

        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be requested for pricing!</div>');
        }
        else {
            if (!$("#editRfqForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#editRfqForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editRfqForm select[required]').each(function () {
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
                    RfqId: rfqId,
                    RfqNumber: rfqNumber,
                    RfqDate: $('#rfqDate').val(),
                    ProjectId: projectId,
                    ProjectName: $('#projectName').val(),
                    CustomerId: customerId,
                    SalespersonId: salespersonId,
                    FoundryId: foundryId,
                    ContactName: $('#contactName').val(),
                    CountryId: countryId,
                    Attention: $('#attention').val(),
                    PrintsSent: $('#printsSent').val(),
                    SentVia: $('#sentVia').val(),
                    ShipmentTermId: $("#shipmentTermId").val(),
                    IsMachined: isMachined,
                    Packaging: $('#packaging').val(),
                    NumberOfSamples: $('#numberOfSamples').val(),
                    Details: $('#details').val(),
                    CoatingType: $('#coatingType').val(),
                    SpecificationMaterialDescription: $('#specificationMaterial').val(),
                    ISIRRequired: $('#isirRequired').prop('checked'),
                    SampleCastingAvailable: $('#sampleCastingAvailable').prop('checked'),
                    MetalCertAvailable: $('#metalCertAvailable').prop('checked'),
                    CMTRRequired: $('#cmtrRequired').prop('checked'),
                    GaugingRequired: $('#gaugingRequired').prop('checked'),
                    TestBarsRequired: $('#testBarsRequired').prop('checked'),
                    Notes: $('#notes').val(),
                    Status: selectedVal,
                    IsOpen: $('#open').prop("checked"),
                    IsHold: $('#hold').prop("checked"),
                    IsCanceled: $('#canceled').prop("checked"),
                    HoldExpirationDate: holdExpirationDate,
                    HoldNotes: holdNotes,
                    CancelNotes: cancelNotes,
                    CanceledDate: canceledDate,
                    RfqParts: parts,
                };

                console.log(model);

                $.ajax({
                    type: 'PUT',
                    url: "/SouthlandMetals/Operations/Rfq/Edit",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    success: function (result) {
                        if (result.Success) {

                            if (result.IsHold) {
                                signalR.server.sendRoleNotification(currentUser + " has put RFQ " + rfqNumber + " on hold til " + holdExpirationDate, "Admin");
                            }
                            else if (result.IsCanceled) {
                                signalR.server.sendRoleNotification(currentUser + " has canceled RFQ " + rfqNumber, "Admin");
                            }

                            parts = [];
                            selectableParts = [];
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
            }
        }
    });

    $(document).on('click', '#cancelUpdateRfqBtn', function () {
        materialId = $('#materialId option:selected').val(null);
        materialDescription = $('#materialId option:selected').text(null);
        materialId = null;
        materialDescription = null;
        parts = [];
        selectableParts = [];
        window.history.back();
    });

    function getSelectableCoatingTypes() {
        var coatingTypes = $("#coatingType");

        coatingTypes.append($("<option />").val(null).text("--Select or Enter coating Type--"));

        $.each(selectableCoatingTypes, function (n, coatingType) {
            coatingTypes.append($("<option />").val(coatingType.Text).text(coatingType.Text));

            if (coatingType.Value == coatingTypeId) {
                $('#coatingType').val(coatingType.Text);
            }
        });

        $('#coatingType').selectize({
            create: true,
            sortField: {
                field: 'text',
                direction: 'asc',
            },
            dropdownParent: 'body',
            closeAfterSelect: true
        });
    }

    function getSelectableSpecificationMaterials() {
        var specificationMaterials = $('#specificationMaterial');

        specificationMaterials.append($("<option />").val(null).text("--Select or Enter specification Material--"));

        $.each(selectableSpecificationMaterials, function (n, specificationMaterial) {
            specificationMaterials.append($("<option />").val(specificationMaterial.Text).text(specificationMaterial.Text));
            if (specificationMaterial.Value == specificationMaterialId) {
                $('#specificationMaterial').val(specificationMaterial.Text);
            }
        });

        $('#specificationMaterial').selectize({
            create: true,
            sortField: {
                field: 'text',
                direction: 'asc',
            },
            dropdownParent: 'body',
            closeAfterSelect: true
        });
    }

    function getSelectablePartsByCustomerAndFoundry(customerId, foundryId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Part/GetSelectablePartsByCustomerAndFoundry",
            data: { "customerId": customerId, "foundryId": foundryId },
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

    function getRfqPartByPart(partId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Rfq/GetRfqPartByPart",
            data: { "partId": partId },
            contentType: "application/json",
            success: function (result) {

                $('#partNumber').val(result.PartNumber);
                $('#partDescription').val(result.PartDescription);
                $('#addPartCustomerId').val(result.CustomerId);
                $('#addPartFoundryId').val(result.FoundryId);
                $('#revisionNumber').val(result.RevisionNumber);
                $('#weight').val(result.Weight);
                $('#annualUsage').val(result.AnnualUsage);
                $('#partTypeId').val(result.PartTypeId);
                $('#partStatusId').val(result.PartStatusId);
                $('#destinationId').val(result.DestinationId);
                $('#surchargeId').val(result.SurchargeId);
                $('#subFoundryId').val(result.SubFoundryId);

                if (result.IsRaw) {
                    $('#isRawBtn').prop('checked', true);
                    $('#isMachinedBtn').prop('checked', false);
                }
                else {
                    $('#isRawBtn').prop('checked', false);
                    $('#isMachinedBtn').prop('checked', true);
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _AddRfqPart() {

        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Rfq/_AddRfqPart",
            success: function (result) {

                $('#addRfqPartDiv').html('');
                $('#addRfqPartDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#addPartCustomerId').val(customerId);
                $('#addPartFoundryId').val(foundryId);

                $('#parts').empty();

                console.log(selectableParts);

                $.each(selectableParts, function (n, part) {
                    $("#parts").append($("<option />").val(part.Value).text(part.Text));
                });


                if (materialId != null) {
                    $('#materialId option:selected').val(materialId);
                    $('#materialId option:selected').text(materialDescription);
                    $('#materialId').prop('disabled', true);
                }

                $('#addRfqPartModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _EditRfqPart(rfqPart) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Rfq/_EditRfqPart",
            success: function (result) {

                $('#editRfqPartDiv').html('');
                $('#editRfqPartDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#editProjectPartId').val(rfqPart.ProjectPartId);
                $('#editPartNumber').val(rfqPart.PartNumber);
                $('#editPartDescription').val(rfqPart.PartDescription);
                $('#editRevisionNumber').val(rfqPart.RevisionNumber);
                $('#editWeight').val(rfqPart.Weight);
                $('#editAnnualUsage').val(rfqPart.AnnualUsage);
                $('#editMaterialId option:selected').val(rfqPart.MaterialId);
                $('#editMaterialId option:selected').text(rfqPart.MaterialDescription);
                $('#editMaterialId').prop('disabled', true);

                if (rfqPart.IsRaw) {
                    $('#editIsRawBtn').prop('checked', true);
                    $('#editIsMachinedBtn').prop('checked', false);
                }
                else {
                    $('#editIsRawBtn').prop('checked', false);
                    $('#editIsMachinedBtn').prop('checked', true);
                }

                $('#editRfqPartModal').modal('show');
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