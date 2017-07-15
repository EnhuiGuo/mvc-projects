$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#rfqsLink').addClass("current-link");

    var parts = [];
    var selectableParts = [];
    var materialId = null;
    var materialDescription = null;
    $('#totalWeight').text(0);
    var projectName = null;
    var countryId = null;

    getSelectableProjects();
    getSelectableCoatingTypes();
    getSelectableSpecificationMaterials();

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
        { "data": "Weight", "title": "Weight", "class": "center", weightFlag },
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
                if (part.PartNumber === partNumber) {
                    part.PartNumber = partNumber;
                    part.RevisionNumber = revisionNumber;
                    part.PartDescription = partDescription;
                    part.CustomerId = editPartCustomerId;
                    part.FoundryId = editPartFoundryId;
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
        var rfqPart = rfqPartsTable.row($(this).parents('tr')).data();
        var childData = rfqPartsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (rfqPart != null) {
            data = rfqPart;
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

                rfqPartsTable.row(rfqPart).remove().draw();
                parts.splice(parts.indexOf(data.PartNumber), 1);
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
            partId = $(this).val();
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
            var partId = $('#parts').val();
            var isNew = true;
            var isRaw = $('#isRawBtn').prop('checked');
            var isMachined = $('#isMachinedBtn').prop('checked');
            var partNumber = $('#partNumber').val();
            var weight = $('#weight').val();
            var partDescription = $('#partDescription').val();
            var customerId = $('#customerId').val();
            var foundryId = $('#foundryId').val();
            var addPartCustomerId = $('#addPartCustomerId').val();
            var addPartFoundryId = $('#addPartFoundryId').val();
            var annualUsage = $('#annualUsage').val();
            var revisionNumber = $('#revisionNumber').val();
            materialId = $('#materialId option:selected').val();
            materialDescription = $('#materialId option:selected').text();
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

    $(document).on('click', '#saveRfqBtn', function () {
        event.preventDefault();

        if (parts.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Parts to be requested for pricing!</div>');
        }
        else {
            if (!$("#createRfqForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#createRfqForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#createRfqForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    ProjectName: $('#projects').val(),
                    RfqNumber: rfqNumber,
                    RfqDate: $('#rfqDate').val(),
                    CustomerId: $('#customerId').val(),
                    SalespersonName: $('#salespersonName').val(),
                    SalespersonId: $('#salespersonId').val(),
                    FoundryId: $('#foundryId').val(),
                    ContactName: $('#contactName').val(),
                    CountryName: $('#countryName').val(),
                    CountryId: countryId,
                    Attention: $('#attention').val(),
                    PrintsSent: $('#printsSent').val(),
                    SentVia: $('#sentVia').val(),
                    ShipmentTermId: $("#shipmentTermId").val(),
                    IsMachined: $("#isMachined").prop('checked'),
                    Packaging: $('#packaging').val(),
                    NumberOfSamples: $('#numberOfSamples').val(),
                    Details: $('#details').val(),
                    MaterialId: materialId,
                    CoatingType: $('#coatingType').val(),
                    SpecificationMaterialDescription: $('#specificationMaterial').val(),
                    ISIRRequired: $('#isirRequired').prop('checked'),
                    SampleCastingAvailable: $('#sampleCastingAvailable').prop('checked'),
                    MetalCertAvailable: $('#metalCertAvailable').prop('checked'),
                    CMTRRequired: $('#cmtrRequired').prop('checked'),
                    GaugingRequired: $('#gaugingRequired').prop('checked'),
                    TestBarsRequired: $('#testBarsRequired').prop('checked'),
                    Notes: $('#notes').val(),
                    IsOpen: true,
                    HoldExpirationDate: null,
                    HoldNotes: null,
                    CanceledDate: null,
                    CancelNotes: null,
                    RfqParts: parts
                };

                var target = document.getElementById('spinnerDiv');
                var spinner = new Spinner(opts).spin(target);

                $("#page-content-wrapper").css({ opacity: 0.5 });
                $('#spinnerDiv').show();

                $.ajax({
                    type: "POST",
                    url: "/SouthlandMetals/Operations/Rfq/Create",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {

                        $("#page-content-wrapper").css({ opacity: 1.0 });
                        spinner.stop(target);
                        $('#spinnerDiv').hide();

                        if (result.Success) {

                            partId = null;
                            projectPartId = null;
                            revisionNumber = null;

                            parts = [];
                            selectableParts = [];
                            drawings = [];

                            $.confirm({
                                text: 'Create Success, Do you want print this RFQ?',
                                dialogClass: "modal-confirm",
                                confirmButton: "Yes",
                                confirmButtonClass: 'btn btn-sm',
                                cancelButton: "No",
                                cancelButtonClass: 'btn btn-sm btn-default',
                                closeIcon: false,
                                confirm: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Report/RfqReport?rfqId=' + result.ReferenceId + '';
                                },
                                cancel: function (button) {
                                    window.location.href = '/SouthlandMetals/Operations/Rfq';
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
    });

    $(document).on('click', '#cancelSaveRfqBtn', function () {

        materialId = $('#materialId option:selected').val(null);
        materialDescription = $('#materialId option:selected').text(null);
        materialId = null;
        materialDescription = null;
        selectableParts = [];
        window.history.back();
    });

    $('#projects').change(function () {

        $.ajax({
            type: 'GET',
            url: "/SouthlandMetals/Operations/Project/GetProject",
            dataType: "json",
            data: { "projectName": $(this).val() },
            success: function (result) {
                if (result) {

                    if (result.SelectableCustomers !== null && result.SelectableCustomers.length > 0) {
                        $("#customerId").empty();
                        $("#customerId").append($("<option />").val(null).text("--Select Customer--"));
                        $.each(result.SelectableCustomers, function (n, customer) {
                            $("#customerId").append($("<option />").val(customer.Value).text(customer.Text));
                        });
                    }

                    if (result.SelectableFoundries !== null && result.SelectableFoundries.length > 0) {
                        $("#foundryId").empty();
                        $("#foundryId").append($("<option />").val(null).text("--Select Foundry--"));
                        $.each(result.SelectableFoundries, function (n, foundry) {
                            $("#foundryId").append($("<option />").val(foundry.Value).text(foundry.Text));
                        });
                    }
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $('#customerId').change(function () {
        var customerId = $('#customerId').val();
        if (customerId == "--Select Customer--") {
            $("#contactName").val("");
        }
        else {
            $.ajax({
                type: 'GET',
                url: "/SouthlandMetals/Administration/Customer/GetCustomer",
                dataType: "json",
                data: { "customerId": customerId },
                success: function (data) {
                    if (data == null) {
                        $('#contactName').val("N/A");
                        $('#salespersonName').val("N/A");
                    }
                    else {
                        $('#contactName').val(data.ContactName);
                        $('#salespersonName').val(data.SalespersonName);
                        $('#salespersonId').val(data.SalespersonId);
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#foundryId').change(function () {
        var customerId = $('#customerId').val();
        if (customerId == "--Select Customer--") {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 '<strong>Warning!</strong>&nbsp;Please select a Customer!</div>');
        }

        var foundryId = $('#foundryId').val();
        if (foundryId == "--Select Foundry--") {
            $('#countryName').val("");
            countryId = null;
        }
        else {
            $.ajax({
                type: 'GET',
                url: "/SouthlandMetals/Administration/Foundry/GetCountryByFoundry",
                dataType: "json",
                data: { "foundryId": foundryId },
                success: function (result) {
                    if (result != null) {
                        $('#countryName').val(result.CountryName);
                        countryId = result.CountryId;
                        getSelectablePartsByCustomerAndFoundry(customerId, foundryId);
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

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

                if (materialId === null) {
                    $('#materialId option:selected').val(result.MaterialId);
                    $('#materialId option:selected').text(result.MaterialDescription);
                    $('#materialId').prop('disabled', true);

                    materialId = result.MaterialId;
                    materialDescription = result.MaterialDescription;
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getSelectableProjects() {
        $.ajax({
            type: 'GET',
            url: "/SouthlandMetals/Operations/Project/GetSelectableProjects",
            cache: false,
            success: function (result) {

                var projects = $("#projects");

                projects.append($("<option />").val(null).text("--Select or Enter Project--"));

                $.each(result, function (n, project) {
                    projects.append($("<option />").val(project.Text).text(project.Text));
                });

                $('#projects').selectize({
                    create: true,
                    sortField: {
                        field: 'text',
                        direction: 'asc',
                    },
                    dropdownParent: 'body',
                    closeAfterSelect: true
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getSelectableCoatingTypes() {
        var coatingTypes = $("#coatingType");

        coatingTypes.append($("<option />").val(null).text("--Select or Enter Coating Type--"));

        $.each(selectableCoatingTypes, function (n, coatingType) {
            coatingTypes.append($("<option />").val(coatingType.Text).text(coatingType.Text));
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

        specificationMaterials.append($("<option />").val(null).text("--Select or Enter Specification Material--"));

        $.each(selectableSpecificationMaterials, function (n, specificationMaterial) {
            specificationMaterials.append($("<option />").val(specificationMaterial.Text).text(specificationMaterial.Text));
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

                $('#addPartCustomerId').val($('#customerId').val());
                $('#addPartFoundryId').val($('#foundryId').val());

                $('#parts').empty();

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
                $('#editPartCustomerId').val(rfqPart.CustomerId);
                $('#editPartFoundryId').val(rfqPart.FoundryId);
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
});