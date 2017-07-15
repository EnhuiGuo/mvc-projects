$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");

    $('#collapseAdminOrders').addClass("in");
    $('#collapseAdminOrders').attr("aria-expanded", "true");

    $('#adminOrdersLink').addClass("category-current-link");
    $('#shipmentTermsLink').addClass("current-link");

    $('#countryId').on('change', function () {
        var countryId = $('#countryId').val();

        getShipmentTermsByCountry(countryId);
    });

    $('#updateShipmentTermsBtn').on('click', function () {
        event.preventDefault();

        if (!$('#shipmentForm')[0].checkValidity()) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');

            $('#shipmentForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#shipmentForm select[required]').each(function () {
                if ($(this).val() === "--Select Country--") {
                    $(this).addClass("form-error");
                }
            });

            $('#shipmentForm textarea[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CountryId: $('#countryId').val(),
                CountryName: $("#countryId option:selected").text(),
                ShipmentTerms: $('#shipmentTerms').val(),
                PrintTerms: $('#printTerms').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/PurchaseOrder/EditShipmentTerms",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                              'Shipment Terms have been updated successfully!</div>');

                        $('.alert').delay(3000).hide("fast");
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

    $('#cancelEditShipmentTermsBtn').on('click', function () {
        $('#shipmentTerms').val(originalShipmentTerms);
        $('#printTerms').val(originalPrintTerms);
    });

});

function getShipmentTermsByCountry(countryId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/PurchaseOrder/GetShipmentTermsByCountry",
        data: { 'countryId': countryId },
        contentType: "application/json",
        dataType: "json",
        success: function (result) {
            $('#shipmentTerms').val(result.ShipmentTerms);
            if (result.PrintTerms) {
                $('#printTerms').prop('checked', true);
            }

            originalShipmentTerms = result.ShipmentTerms;
            originalPrintTerms = result.PrintTerms;
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

