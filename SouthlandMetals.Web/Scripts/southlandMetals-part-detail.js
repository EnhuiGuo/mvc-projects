$(document).ready(function () {
    $('#operationsLink').addClass("category-current-link");
    $('#collapseOpsParts').addClass("in");
    $('#collapseOpsParts').attr("aria-expanded", "true");

    $('#opsPartsLink').addClass("category-current-link");
    $('#detailsLink').addClass("current-link");

    $('#isActive').prop('checked', isActive);
    $('#isRawA').prop('checked', isRaw);
    $('#isRawB').prop('checked', isRaw);
    $('#isMachinedA').prop('checked', isMachined);
    $('#isMachinedB').prop('checked', isMachined);
    $('#isFamilyPattern').prop('checked', isFamilyPattern);

    if (isProjectPart) {
        $('#partStatusId').attr('readonly', true);
    }

    $(document).on('click', '#updatePartBtn', function () {
        event.preventDefault();

        if (!$("#updatePartForm")[0].checkValidity()) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('#updatePartForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#updatePartForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                ProjectPartId: $('#projectPartId').val(),
                PartId: $('#partId').val(),
                PartNumber: $('#partNumber').val(),
                PartDescription: $('#partDescription').val(),
                ProjectId: $('#projectId').val(),
                CustomerId: $('#customerId').val(),
                FoundryId: $('#foundryId').val(),
                PartTypeId: $('#partTypeId').val(),
                HtsNumberId: $('#htsNumberId').val(),
                ShipmentTermId: $('#shipmentTermId').val(),
                PaymentTermId: $('#paymentTermId').val(),
                IsMachined: isMachined,
                IsRaw: isRaw,
                PartStatusId: $('#partStatusId').val(),
                DestinationId: $('#destinationId').val(),
                Weight: $('#weight').val(),
                Cost: $('#cost').val(),
                Price: $('#price').val(),
                AdditionalCost: $('#additionalCost').val(),
                SurchargeId: $('#surchargeId').val(),
                PalletQuantity: $('#palletQuantity').val(),
                PalletWeight: $('#palletWeight').val(),
                QuantityOnHand: $('#quantityOnHand').val(),
                CustomerAddressId: $('#customerAddressId').val(),
                SiteId: $('#siteId').val(),
                CoatingTypeId: $('#coatingTypeId').val(),
                FixtureCost: $('#fixtureCost').val(),
                FixturePrice: $('#fixturePrice').val(),
                PatternDate: $('#patternDate').val(),
                PatternCost: $('#patternCost').val(),
                PatternPrice: $('#patternPrice').val(),
                PatternMaterialId: $('#patternMaterialId').val(),
                FoundryOrderId: $('#foundryOrderId').val(),
                ToolingDescription: $('#toolingDescription').val(),
                Notes: $('#notes').val(),
                MaterialId: $('#materialId').val(),
                MaterialSpecificationId: materialSpecificationId,
                IsFamilyPattern: $('#isFamilyPattern').prop('checked'),
                AccountCode: $('#accountCode').val(),
                AnnualUsage: $('#annualUsage').val(),
                IsProjectPart: isProjectPart
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Operations/Part/EditPart",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {

                        signalR.server.sendRoleNotification("Part " + model.PartNumber + " has been updated.", "Admin");
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
    });

    $(document).on('click', '#cancelUpdatePartBtn', function () {
        window.history.back();
    });

    $(document).on('click', '#fileSubmitBtn', function () {

        var myform = document.getElementById("attachment");
        var formData = new FormData(myform);

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Part/GetDrawingImageData",
            dataType: "json",
            contentType: false,
            data: formData,
            async: true,
            cache: false,
            processData: false,
            success: function (result) {
                if (!result.Success) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');

                    $('#drawing').val("");
                }
                else {
                    if (drawings != null && drawings.length > 0)
                    {
                        var existingDrawing = drawings.filter(function (n) {
                            return n.RevisionNumber === result.RevisionNumber;
                        });
                        if (existingDrawing.length > 0) {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                         '<strong>Warning!</strong>&nbsp;Duplicate drawing!</div>');

                            $('#drawing').val("");
                        }
                        else {
                            addDrawing(formData);
                        }
                    }
                    else {
                        addDrawing(formData);
                    }
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    function addDrawing(formData) {
        console.log(formData);
        formData: {
                partId: $('#partId').val()
        };
        var lastId = $('#drawingGallery').find(">:first-child").attr('id');

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Part/AddDrawing",
            dataType: "json",
            data: formData,
            contentType: false,
            async: true,
            cache: false,
            processData: false,
            success: function (result) {
                if (result.Success) {
                    var drawingString = '<div id="' + result.DrawingId + '" class="col-md-3">' +
                                                  '<a href="/SouthlandMetals/Operations/Part/GetDrawing/?drawingId=' + result.DrawingId + '&isProject=' + result.IsProject + '" target="_blank"><img src="/Content/images/southland_pdf_48.png" class="img-responsive" alt="pdf" /></a>' +
                                                  '<div style="display: inline-block;">' +
                                                      '<span class="pull-left">' + result.RevisionNumber + '</span>&nbsp;&nbsp;' +
                                                      '<span id="' + result.DrawingId + '" class="hidden">' + result.DrawingId + '</span>' +
                                                      '<span style="padding-top: 4px;" aria-hidden="true" class="glyphicon glyphicon-trash pull-right" onclick="deleteDrawing(\'' + result.DrawingId + '\')"></span>' +
                                                  '</div>' +
                                              '</div>';
                    drawings.push({ 'RevisionNumber': result.RevisionNumber });
                    $("#" + lastId + "").before(drawingString);
                    $('#drawing').val("");
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
    };

    window.deleteDrawing = function (drawingId) {
        $.confirm({
            text: 'Are you sure you want to delete the drawing?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function () {
                var partId = $('#partId').val();
                $.ajax({
                    type: "DELETE",
                    url: "/SouthlandMetals/Operations/Part/DeleteDrawing",
                    data: { 'partId': partId, 'drawingId': drawingId },
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            drawings.splice(drawings.indexOf(drawingId), 1);
                            $("#" + drawingId).remove();
                        }
                        else {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                '<strong>Warning!</strong>&nbsp;Unable to delete Drawing!</div>');
                        }
                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });
            },
            cancel: function () {

            }
        });
    };

    $('#customerId').on('change', function () {

        var customerId = this.value;

        $.ajax({
            type: 'GET',
            url: "/SouthlandMetals/Administration/Customer/GetAddressesAndAccountCodesByCustomer",
            dataType: "json",
            data: { "customerId": customerId },
            success: function (result) {

                $('#customerAddressId').empty();

                $.each(result.SelectableCustomerAddresses, function (n, term) {
                    $("#customerAddressId").append($("<option />").val(term.Value).text(term.Text));
                });

                $('#accountCodeId').empty();

                $.each(result.SelectableAccountCodes, function (n, term) {
                    $("#accountCodeId").append($("<option />").val(term.Value).text(term.Text));
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $('#foundryOrderId').on('change', function () {
        var foundryOrderId = this.value;

        window.location.href = '/SouthlandMetals/Operations/PurchaseOrder/FoundryOrderDetail?foundryOrderId=' + foundryOrderId + '';
    });
});
