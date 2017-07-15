$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");
    $('#customersLink').addClass("current-link");

    var isActiveFlag = function (data, type, row) {
        if (row.IsActive) {
            return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
        }
        else {
            return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
        }
    }

    var customersTable = $('#customers').DataTable({
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
        "pageLength": 20,
        "lengthChange": false,
        "order": [1, 'asc'],
        "data": customers,
        "columns": [
        { "data": "CustomerId", "title": "CustomerId", "visible": false },
        { "data": "CustomerNumber", "title": "Number", "class": "center" },
        { "data": "ShortName", "title": "Name", "class": "center" },
        { "data": "ContactName", "title": "Contact", "class": "center" },
        { "data": "SalespersonName", "title": "Salesperson", "class": "center" },
        { "data": "IsActive", "title": "Active", "class": "center", "render": isActiveFlag },
        { "title": "Order Terms", "class": "center" },
        { "title": "Detail", "class": "center" },
        ],
        "columnDefs": [
        {
            "targets": 6,
            "title": "Order Terms",
            "width": "8%", "targets": 6,
            "data": null,
            "defaultContent":
                "<span class='fa fa-edit' id='orderTermsBtn'></span>"
        },
        {
            "targets": 7,
            "title": "Detail",
            "width": "8%", "targets": 7,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailBtn'></span>"
        }]
    });

    $('#addCustomer').on('click', function () {
        _AddCustomer();
    });

    $('#customers tbody').on('click', '#addressesBtn', function () {
        var customer = customersTable.row($(this).parents('tr')).data();
        var childData = customersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customer != null) {
            data = customer;
        }
        else {
            data = childData;
        }
        _ViewCustomerAddresses(data.CustomerId);
    });

    $('#customers tbody').on('click', '#orderTermsBtn', function () {
        var customer = customersTable.row($(this).parents('tr')).data();
        var childData = customersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customer != null) {
            data = customer;
        }
        else {
            data = childData;
        }
        _OrderTerms(data.CustomerId);
    });

    $('#customers tbody').on('click', '#detailBtn', function () {
        var customer = customersTable.row($(this).parents('tr')).data();
        var childData = customersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customer != null) {
            data = customer;
        }
        else {
            data = childData;
        }
        window.open("/SouthlandMetals/Administration/Customer/Detail?customerId=" + data.CustomerId, target = "_self");
    });

    $('#customers tbody').on('click', '#editBtn', function () {
        var customer = customersTable.row($(this).parents('tr')).data();
        var childData = customersTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (customer != null) {
            data = customer;
        }
        else {
            data = childData;
        }
        _EditCustomer(data);
    });

    var addressesTable = $('#addresses').DataTable({
        dom: 'Bfrt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 25,
        "lengthChange": false,
        "order": [2, 'asc'],
        "data": addresses,
        "columns": [
        { "data": "CustomerAddressId", "title": "CustomerAddressId", "visible": false },
        { "data": "AddressCode", "title": "Address Code", "class": "center" },
        { "data": "ContactName", "title": "Contact", "class": "center" },
        { "data": "ContactPhone", "title": "Phone", "class": "center" },
        { "data": "Address1", "title": "Address", "class": "center" },
        { "data": "City", "title": "City", "class": "center" },
        { "data": "StateName", "title": "StateName", "class": "center" },
        { "data": "PostalCode", "title": "Postal Code", "class": "center" },
        { "data": "CountryName", "title": "Country", "class": "center" },
        { "data": "SiteDescription", "title": "Site", "class": "center" },
        ]
    });

    $(document).on('click', '#saveCustomerBtn', function () {
        event.preventDefault();

        if (!$("#addCustomerForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#addCustomerForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#addCustomerForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CustomerNumber: $('#customerNumber').val(),
                CustomerName: $('#customerName').val(),
                ShortName: $('#shortName').val(),
                Phone1: $('#phone1').val(),
                Phone2: $('#phone2').val(),
                Phone3: $('#phone3').val(),
                FaxNumber: $('#faxNumber').val(),
                SalespersonId: $('#salespersonId').val(),
                SalesTerritoryId: $('#salesTerritoryId').val(),
                PaymentTermId: $('#paymentTermId').val(),
                ShipmentTermId: $('#shipmentTermId').val(),
                ContactName: $('#contactName').val(),
                ContactPhone: $('#contactPhone').val(),
                ContactEmail: $('#contactEmail').val(),
                AddressCode: $('#addressCode').val(),
                Address1: $('#address1').val(),
                Address2: $('#address2').val(),
                Address3: $('#address3').val(),
                City: $('#city').val(),
                StateId: $('#stateId').val(),
                CountryId: $('#countryId').val(),
                PostalCode: $('#postalCode').val(),
                SiteId: $('#siteId').val(),
                ShipToName: $('#shipToName').val(),
                IsActive: true
            };

            $.ajax({
                type: "POST",
                url: "/SouthlandMetals/Administration/Customer/AddCustomer",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#customers').DataTable().clear().draw();
                        $('#customers').DataTable().rows.add(result.Customers); // Add new data
                        $('#customers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#addCustomerModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#updateCustomerBtn', function () {
        event.preventDefault();

        if (!$("#editCustomerForm")[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();
            $('#editCustomerForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });

            $('#editCustomerForm select[required]').each(function () {
                if (!$(this).is(':selected')) {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                CustomerId: $('#editCustomerId').val(),
                CustomerNumber: $('#editCustomerNumber').val(),
                CustomerName: $('#editCustomerName').val(),
                ShortName: $('#editShortName').val(),
                Phone1: $('#editPhone1').val(),
                Phone2: $('#editPhone2').val(),
                Phone3: $('#editPhone3').val(),
                FaxNumber: $('#editFaxNumber').val(),
                SalespersonId: $('#editSalespersonId').val(),
                SalesTerritoryId: $('#editSalesTerritoryId').val(),
                PaymentTermId: $('#editPaymentTermId').val(),
                ShipmentTermId: $('#editShipmentTermId').val(),
                ContactName: $('#editContactName').val(),
                ContactPhone: $('#editContactPhone').val(),
                ContactEmail: $('#editContactEmail').val(),
                AddressCode: $('#editAddressCode').val(),
                Address1: $('#editAddress1').val(),
                Address2: $('#editAddress2').val(),
                Address3: $('#editAddress3').val(),
                City: $('#editCity').val(),
                StateId: $('#editStateId').val(),
                CountryId: $('#editCountryId').val(),
                PostalCode: $('#editPostalCode').val(),
                SiteId: $('#editSiteId').val(),
                ShipToName: $('#editShipToName').val(),
                IsActive: $('#editIsActive').prop('checked')
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/Customer/EditCustomer",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('#customers').DataTable().clear().draw();
                        $('#customers').DataTable().rows.add(result.Customers); // Add new data
                        $('#customers').DataTable().columns.adjust().draw();
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }

                    $('#editCustomerModal').modal('hide');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $('#cancelAddCustomerBtn').on('click', function () {
        $('#addCustomerModal').modal('hide');
    });

    $('#cancelEditCustomerBtn').on('click', function () {
        $('#editCustomerModal').modal('hide');
    });

    $(document).on('click', '#updateOrderTermsBtn', function () {
        var model = {
            CustomerId: $('#customerId').val(),
            OrderTermsDescription: $('#orderTermsDescription').val()
        };

        $.ajax({
            type: "PUT",
            url: "/SouthlandMetals/Administration/Customer/EditOrderTerms",
            data: JSON.stringify(model),
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    //$('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                    //         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                    //         '<strong>Update Successful!</strong>');
                    //setTimeout(function () {
                    //    $('#alertDiv').fadeOut();
                    //}, 2000);
                }
                else {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                }

                $('#orderTermsModal').modal('hide');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $('#cancelUpdateOrderTermsBtn').on('click', function () {
        $('#orderTermsModal').modal('hide');
    });

    //var invoiceFeesTable = $('#invoiceFees').DataTable({
    //    "autoWidth": false,
    //    "searching": false,
    //    "ordering": false,
    //    "paging": false,
    //    "scrollY": 300,
    //    "scrollCollapse": true,
    //    "info": false,
    //    "order": [1, 'asc'],
    //    "data": invoiceFees,
    //    "columns": [
    //    { "data": "InvoiceFeesId", "title": "InvoiceFeesId", "visible": false },
    //    { "data": "CustomerId", "title": "CustomerId", "visible": false },
    //    { "data": "Type", "title": "Type", "class": "center" },
    //    { "data": "ItemNumber", "title": "Item Number", "class": "center" },
    //    { "data": "Description", "title": "Description", "class": "center" },
    //    { "data": "AccountNumber", "title": "Account", "class": "center" },
    //    { "title": "Edit", "class": "center" },
    //    { "title": "Delete", "class": "center" }
    //    ],
    //    "columnDefs": [{
    //        "targets": 6,
    //        "title": "Edit",
    //        "width": "10%", "targets": 6,
    //        "data": null,
    //        "defaultContent":
    //            "<span class='glyphicon glyphicon-pencil' id='editFeeBtn'></span>"
    //    },
    //    {
    //        "targets": 7,
    //        "title": "Delete",
    //        "width": "10%", "targets": 7,
    //        "data": null,
    //        "defaultContent":
    //            "<span class='glyphicon glyphicon-trash' id='deleteFeeBtn'></span>"
    //    }]
    //});

    //$('#invoiceFees tbody').on('click', '#editFeeBtn', function () {
    //    var invoiceFee = invoiceFeesTable.row($(this).parents('tr')).data();

    //});

    //$('#invoiceFees tbody').on('click', '#deleteFeeBtn', function () {
    //    var invoiceFee = invoiceFeesTable.row($(this).parents('tr')).data();

    //});

    //$('#addInvoiceFee').click(function () {

    //});
});

function _AddCustomer() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Customer/_AddCustomer",
        success: function (result) {

            $('#addCustomerDiv').html('');
            $('#addCustomerDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addCustomerModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _OrderTerms(customerId) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Customer/_OrderTerms",
        data: { "customerId": customerId },
        contentType: "application/json",
        success: function (result) {

            $('#orderTermsDiv').html('');
            $('#orderTermsDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#orderTermsModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewCustomerAddresses(customerId) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Customer/_ViewCustomerAddresses",
        data: { "customerId": customerId },
        contentType: "application/json",
        success: function (result) {

            $('#viewCustomerAddressesDiv').html('');
            $('#viewCustomerAddressesDiv').html(result);

            getCustomerAddresses(customerId);

            $('#viewCustomerAddressesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditCustomer(customer) {
    $.ajax({
        type: "GET",
        url: "/SouthlandMetals/Administration/Customer/_EditCustomer",
        data: { "customerId": customer.CustomerId },
        contentType: "application/json",
        dataType: "json",
        success: function (result) {

            $('#editCustomerDiv').html('');
            $('#editCustomerDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            if (customer.IsActive) {
                $('#editIsActive').prop('checked', true);
            }

            $('#editCustomerModal').modal('show');
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
        url: "/SouthlandMetals/Administration/Customer/GetActiveCustomers",
        success: function (result) {
            $('#customers').DataTable().clear().draw();
            $('#customers').DataTable().rows.add(result.Customers); // Add new data
            $('#customers').DataTable().columns.adjust().draw();
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
        url: "/SouthlandMetals/Administration/Customer/GetInactiveCustomers",
        success: function (result) {
            $('#customers').DataTable().clear().draw();
            $('#customers').DataTable().rows.add(result.Customers); // Add new data
            $('#customers').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function getCustomerAddresses(customerId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Administration/Customer/GetCustomerAddresses",
        success: function (result) {
            $('#addresses').DataTable().clear().draw();
            if (result.CustomerAddresses != "")
            {
                $('#addresses').DataTable().rows.add(result.CustomerAddresses); // Add new data
                $('#addresses').DataTable().columns.adjust().draw();
            }
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
