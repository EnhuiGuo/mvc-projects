$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#shipmentTrackingLink').addClass("current-link");

    var bols = [];
    var shipmentId = null;
    var selectedOrders = [];

    var PurchaseOrder = function (oId, oNumber, code, receiptDate) {
        var self = this;

        self.foundryOrderId = ko.observable(oId);
        self.orderNumber = ko.observable(oNumber);
        self.shipCode = ko.observable(code);
        self.receiptDate = ko.observable(receiptDate);
        self.selected = ko.observable(false);
    };

    var ReceivingViewModel = function () {
        var self = this;

        self.shipmentId = ko.observable();
        self.foundryOrderId = ko.observable();
        self.orderNumber = ko.observable();
        self.shipCode = ko.observable();
        self.receiptDate = ko.observable(receiptDate);
        self.selected = ko.observable();

        self.purchaseOrders = ko.observableArray([]);

        self.selectAll = function () {
            $.each(self.purchaseOrders(), function (n, order) {
                order.selected(true);
            });
        };

        self.deselectAll = function () {
            $.each(self.purchaseOrders(), function (n, order) {
                order.selected(false);
            });
            selectedOrders = [];
        };

        self.cancelReceiving = function () {
            self.shipmentId(null);

            if (self.purchaseOrders().length > 0) {
                self.purchaseOrders.removeAll();
                self.purchaseOrders.splice(0, self.purchaseOrders().length);
            }

            selectedOrders = [];
        };

        self.receive = function () {
            getAllSelected();

            if (selectedOrders.length < 1) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;No orders have been selected to receive!</div>');
                $('.errorAlert').show();
            }
            else {
                var model = {
                    PurchaseOrders: selectedOrders
                };

                var target = document.getElementById('spinnerDiv');
                var spinner = new Spinner(opts).spin(target);

                $("#page-content-wrapper").css({ opacity: 0.5 });
                $('#spinnerDiv').show();

                $.ajax({
                    type: "POST",
                    url: "/SouthlandMetals/Operations/Shipment/ReceiveShipment",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {

                        $("#page-content-wrapper").css({ opacity: 1.0 });
                        spinner.stop(target);
                        $('#spinnerDiv').hide();

                        $('#alertDiv').html('<div class="alert alert-success alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 'Orders have been received successfully.</div>');

                        $('.alert').delay(3000).hide("fast");
                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });

                self.shipmentId(null);

                if (self.purchaseOrders().length > 0) {
                    self.purchaseOrders.removeAll();
                    self.purchaseOrders.splice(0, self.purchaseOrders().length);
                }

                selectedOrders = [];

                $('#receivingModal').modal('hide');
            }
        };

        var purchaseOrdersTable = $('#purchaseOrders').DataTable({
            "lengthChange": false,
            "searching": false,
            "paging": false,
            "info": false,
            "ordering": false,
        });

        function getAllSelected() {
            $.each(self.purchaseOrders(), function (n, order) {
                if (order.selected()) {
                    selectedOrders.push({ 'FoundryOrderId': order.foundryOrderId(), 'OrderNumber': order.orderNumber(), 'ShipCode': order.shipCode(), 'ReceiptDate': order.receiptDate() })
                }
            });
        };

        function getPurchaseOrdersByShipmentId(shipmentId) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Operations/Shipment/GetPurchaseOrdersByShipmentId",
                data: { "shipmentId": shipmentId },
                dataType: "json",
                contentType: "application/json",
                success: function (result) {
                    if (self.purchaseOrders().length > 0) {
                        self.purchaseOrders.removeAll();
                        self.purchaseOrders.splice(0, self.purchaseOrders().length);
                    }

                    if (result.PurchaseOrders.length > 0) {
                        $.each(result.PurchaseOrders, function (n, order) {
                            self.purchaseOrders.push(new PurchaseOrder(order.FoundryOrderId, order.OrderNumber, order.ShipCode, receiptDate));
                        });
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        };

        $(document).on('click', '#filterBtn', function () {

            $.ajax({
                type: "GET",
                url: "/SouthlandMetals/Operations/Shipment/SearchShipments",
                contentType: "application/json",
                dataType: "json",
                success: function (shipments) {

                    var status = $('input[name=Status]:checked').val();

                    if (status == 'Open') {
                        shipments = jQuery.grep(shipments, function (a) {
                            return !a.IsCompleted;
                        });
                    }
                    else if (status == 'Completed') {
                        shipments = jQuery.grep(shipments, function (a) {
                            return a.IsCompleted;
                        });
                    }

                    var value = $('#searchVesselId').val();
                    var vessel = $("#searchVesselId option[value='" + value + "']").text();
                    if (vessel != "") {
                        shipments = jQuery.grep(shipments, function (a) {
                            return a.VesselName == vessel;
                        });
                    }

                    var customer = $('#customerId').val();
                    if (customer != "--Select Customer--") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.CustomerId == customer;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var orderNumber = $('#purchaseOrder').val();
                    if (orderNumber != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.OrderNumber == orderNumber;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var invoiceNumber = $('#invoiceNumber').val();
                    if (invoiceNumber != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.InvoiceNumber == invoiceNumber;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var shipCode = $('#shipCode').val();
                    if (shipCode != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.ShipCode == shipCode;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var foundryId = $('#foundryId').val();
                    if (foundryId != "--Select Foundry--") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.FoundryId == foundryId;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var bolNumber = $('#bolNumber').val();
                    if (bolNumber != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.BolNumber == bolNumber;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var customsNumber = $('#customsNumber').val();
                    if (customsNumber != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {
                                    return b.CustomsNumber == customsNumber;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    var containerNumber = $('#containerNumber').val();
                    if (containerNumber != "") {
                        shipments = jQuery.grep(shipments, function (shipment) {
                            if (shipment.BillsOfLading != null) {
                                var bols = jQuery.grep(shipment.BillsOfLading, function (b) {

                                    for (var i = 0; i < b.Containers.length; i++) {
                                        if (b.Containers[i].ContainerNumber != containerNumber) {
                                            b.Containers.splice(i, 1);
                                        }
                                    }
                                    return b.Containers.length > 0;
                                });
                                shipment.BillsOfLading = bols;
                                return shipment.BillsOfLading.length > 0;
                            }
                        });
                    }

                    $('#shipments').DataTable().clear().draw();
                    $('#shipments').DataTable().rows.add(shipments);
                    $('#shipments').DataTable().columns.adjust().draw();

                    $('#bols').DataTable().clear().draw();

                    if (shipmentsTable.rows().count() > 0) {
                        shipmentsTable.row(0).select();
                        var shipmentId = shipmentsTable.row(0).data().ShipmentId;
                        getBolsByShipmenId(shipmentId);
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        });

        $(document).on('click', '#resetBtn', function () {
            $('#customerId>option:eq(0)').prop('selected', true);
            $('#purchaseOrder').val('');
            $('#invoiceNumber').val('');
            $('#shipCode').val('');
            $('#foundryId>option:eq(0)').prop('selected', true);
            $('#containerNumber').val('');
            $('#searchVesselId>option:eq(0)').prop('selected', true);
            $('#bolNumber').val('');
            $('#customsNumber').val('');

            $('#customerId').focus();
        });

        var shipmentsTable = $('#shipments').DataTable({
            "autoWidth": false,
            "pageLength": 10,
            "paging": false,
            "lengthChange": false,
            "order": [4, "desc"],
            "data": shipments,
            "info": false,
            "scrollY": 250,
            "scrollCollapse": true,
            "order": [[7, 'desc'], [2, 'asc']],
            "columns": [
            { "data": "ShipmentId", "title": "ShipmentId", "visible": false },
            { "data": "CarrierId", "title": "CarrierId", "visible": false },
            { "data": "VesselName", "title": "Vessel", "class": "center" },
            { "data": "PortName", "title": "Port", "class": "center" },
            { "data": "DepartureDate", "name": "DepartureDate", "title": "Departure Date", "class": "center", render: dateFlag },
            { "data": "EstArrivalDate", "name": "EstArrivalDate", "title": "Est.Arrival Date", "class": "center", render: dateFlag },
            { "data": "CompletedDate", "name": "CompletedDate", "title": "Completed Date", "class": "center", render: dateFlag },
            { "data": "CreatedDate", "name": "CreatedDate", "title": "CreatedDate", "visible": false, render: dateFlag },
            { "title": "Edit", "class": "center" },
            { "title": "Receive", "class": "center" },
            { "title": "Delete", "class": "center" },
            ],
            "columnDefs": [{
                "targets": 8,
                "title": "Edit",
                "width": "8%", "targets": 8,
                "data": null,
                "defaultContent":
                    "<span id='shipmentEditBtn' class='glyphicon glyphicon-pencil glyphicon-large'></span>"
            },
            {
                "targets": 9,
                "title": "Receive",
                "width": "8%", "targets": 9,
                "data": null,
                "defaultContent":
                        "<span id='shipmentReceiveBtn' class='fa fa-download'></span>"
            },
            {
                "targets": 10,
                "title": "Delete",
                "width": "8%", "targets": 10,
                "data": null,
                "defaultContent":
                        "<span id='shipmentDeleteBtn' class='glyphicon glyphicon-remove glyphicon-large'></span>"
            }
            ]
        });

        if (shipments.length > 0) {
            shipmentsTable.row(0).select();
            var shipmentId = shipmentsTable.row(0).data().ShipmentId;
            getBolsByShipmenId(shipmentId);
        }

        shipmentsTable.on('page', function () {
            var info = shipmentsTable.page.info();
            var startIndex = info.start;
            shipmentsTable.row(startIndex).select();
            var shipmentId = shipmentsTable.row(startIndex).data().ShipmentId;
            getBolsByShipmenId(shipmentId);
        });

        $('#shipments tbody').on('click', 'tr', function () {
            shipmentsTable.rows().deselect();
            shipmentsTable.row(this).select();
            shipmentId = shipmentsTable.row(this).data().ShipmentId;
            getBolsByShipmenId(shipmentId);
        });

        $('#shipments tbody').on('click', '#shipmentEditBtn', function () {
            var shipment = shipmentsTable.row($(this).parents('tr')).data();
            var childData = shipmentsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (shipment != null) {
                data = shipment;
            }
            else {
                data = childData;
            }
            _EditShipment(data.ShipmentId);
        });

        $('#shipments tbody').on('click', '#shipmentReceiveBtn', function () {
            var shipment = shipmentsTable.row($(this).parents('tr')).data();
            var childData = shipmentsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (shipment != null) {
                data = shipment;
            }
            else {
                data = childData;
            }
            self.shipmentId(data.ShipmentId);
            getPurchaseOrdersByShipmentId(data.ShipmentId);
            _Receiving();
        });

        $('#shipments tbody').on('click', '#shipmentDeleteBtn', function () {
            var shipment = shipmentsTable.row($(this).parents('tr')).data();
            var childData = shipmentsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (shipment != null) {
                data = shipment;
            }
            else {
                data = childData;
            }
            var canDelete = canDeleteShipment(data.ShipmentId);
            if (canDelete) {
                $.confirm({
                    text: 'Are you sure you want to delete this Shipment?',
                    dialogClass: "modal-confirm",
                    confirmButton: "Yes",
                    confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                    cancelButton: "No",
                    cancelButtonClass: 'btn btn-sm btn-default',
                    closeIcon: false,
                    confirm: function (button) {
                        deleteShipment(data.ShipmentId);
                    },
                    cancel: function (button) {

                    }
                });
            }
        });

        var analyzedFlag = function (data, type, row) {
            if (row.HasBeenAnalyzed) {
                return '<span class="glyphicon glyphicon-ok glyphicon-large"></span>';
            }
            else {
                return '<span class="glyphicon glyphicon-remove glyphicon-large"></span>';
            }
        }

        var bolsTable = $('#bols').DataTable({
            "autoWidth": false,
            "paging": false,
            "lengthChange": false,
            "info": false,
            "scrollY": 250,
            "scrollCollapse": true,
            "order": [[6, 'desc'], [2, 'asc']],
            "data": bols,
            "columns": [
            { "data": "BillOfLadingId", "title": "BillOfLadingId", "visible": false },
            { "data": "ShipmentId", "title": "ShipmentId", "visible": false },
            { "data": "BolNumber", "title": "Number", "class": "center" },
            { "data": "FoundryName", "title": "Foundry", "class": "center" },
            { "data": "Description", "title": "Description", "class": "center" },
            { "data": "HasBeenAnalyzed", "title": "Analyzed", "class": "center", "render": analyzedFlag },
            { "data": "CreatedDate", "title": "CreatedDate", "visible": false },
            { "title": "Edit", "class": "center" },
            { "title": "View", "class": "center" }
            ],
            "columnDefs": [
                {
                    "targets": 7,
                    "title": "Edit",
                    "width": "8%", "targets": 7,
                    "data": null,
                    "defaultContent":
                        "<span id='bolEditBtn' class='glyphicon glyphicon-pencil glyphicon-large'></span>"
                },
                {
                    "targets": 8,
                    "title": "View",
                    "width": "8%", "targets": 8,
                    "data": null,
                    "defaultContent":
                        "<span id='bolViewBtn' class='glyphicon glyphicon-info-sign glyphicon-large'></span>"
                }
            ]
        });

        $('#bols tbody').on('click', '#bolEditBtn', function () {
            var bol = bolsTable.row($(this).parents('tr')).data();
            var childData = bolsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (bol != null) {
                data = bol;
            }
            else {
                data = childData;
            }
            if (data.HasBeenAnalyzed) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                              '<strong>Warning!</strong>&nbsp;Unable to edit, bill of lading and foundry invoice have been processed for payment.</div>');
            }
            else {
                window.open("/SouthlandMetals/Operations/Shipment/EditBillofLading?bolId=" + data.BillOfLadingId, target = "_self");
            }
        });

        $('#bols tbody').on('click', '#bolViewBtn', function () {
            var bol = bolsTable.row($(this).parents('tr')).data();
            var childData = bolsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (bol != null) {
                data = bol;
            }
            else {
                data = childData;
            }
            window.open("/SouthlandMetals/Operations/Shipment/BillofLadingDetail?bolId=" + data.BillOfLadingId, target = "_self");
        });

        $('#addShipment').on('click', function () {
            _AddShipment();
        });

        $(document).on('click', '#saveShipmentBtn', function () {
            event.preventDefault();

            if (!$("#addShipmentForm")[0].checkValidity()) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('.errorAlert').show();
                $('#addShipmentForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#addShipmentForm select[required]').each(function () {
                    if (!$(this).is(':selected')) {
                        $(this).addClass("form-error");
                    }
                });

                $('#addShipmentForm textarea[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    CarrierId: $('#carrierId option:selected').val(),
                    VesselId: $('#vesselId option:selected').val(),
                    PortId: $('#portId option:selected').val(),
                    DepartureDate: $('#departureDate').val(),
                    EstArrivalDate: $('#estArrivalDate').val(),
                    ShipmentNotes: $('#shipmentNotes').val()
                };

                $.ajax({
                    type: "POST",
                    url: "/SouthlandMetals/Operations/Shipment/AddShipment",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result) {
                            $('#shipments').DataTable().clear().draw();
                            $('#shipments').DataTable().rows.add(result.Shipments); // Add new data
                            $('#shipments').DataTable().columns.adjust().draw();

                            $('#addShipmentModal').modal('hide');
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

        $(document).on('click', '#editShipmentBtn', function () {
            event.preventDefault();

            if (!$('#editShipmentForm')[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');

                $('#editShipmentForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editShipmentForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#editShipmentForm textarea[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var model = {
                    ShipmentId: $('#editShipmentId').val(),
                    CarrierId: $('#editCarrierId').val(),
                    VesselId: $('#editVesselId').val(),
                    PortId: $('#editPortId').val(),
                    DepartureDate: $('#editDepartureDate').val(),
                    EstArrivalDate: $('#editEstArrivalDate').val(),
                    ShipmentNotes: $('#editShipmentNotes').val(),
                    IsComplete: isComplete,
                    CompletedDate: completedDate
                };

                $.ajax({
                    type: "PUT",
                    url: "/SouthlandMetals/Operations/Shipment/EditShipment",
                    data: JSON.stringify(model),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            $('#shipments').DataTable().clear().draw();
                            $('#shipments').DataTable().rows.add(result);
                            $('#shipments').DataTable().columns.adjust().draw();

                            $('#editShipmentModal').modal('hide');
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

        $('#addBol').on('click', function () {
            window.open("/SouthlandMetals/Operations/Shipment/CreateBillofLading?shipmentId=" + shipmentId, target = "_self");
        });

        $('#cancelAddShipmentBtn').on('click', function () {
            $('#addShipmentModal').modal('hide');
        });

        $('#cancelEditShipmentBtn').on('click', function () {
            $('#editShipmentModal').modal('hide');
        });

        $('#cancelAddBolBtn').on('click', function () {
            $('#addBolModal').modal('hide');
        });

        $('#cancelEditBolBtn').on('click', function () {
            $('#editBolModal').modal('hide');
        });

        function canDeleteShipment(shipmentId) {
            var success = false;
            $.ajax({
                type: 'GET',
                cache: false,
                async: false,
                url: "/SouthlandMetals/Operations/Shipment/ValidateShipmentDelete",
                data: { "shipmentId": shipmentId },
                success: function (result) {

                    if (!result.Success) {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                    else {
                        success = true;
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                },
            });

            return success;
        }

        function deleteShipment(shipmentId) {
            $.ajax({
                type: 'DELETE',
                url: "/SouthlandMetals/Operations/Shipment/DeleteShipment",
                data: { "shipmentId": shipmentId },
                success: function (result) {
                    if (result.Success) {
                        window.location.href = '/SouthlandMetals/Operations/Shipment/Tracking';
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
    };

    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {};
            $(element).datepicker(options);

            //handle the field changing
            ko.utils.registerEventHandler(element, "change", function () {
                var observable = valueAccessor();
                observable($(element).datepicker("getDate"));
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).datepicker("destroy");
            });

        },
        //update the control when the view model changes
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                current = $(element).datepicker("getDate");

            if (value - current !== 0) {
                $(element).datepicker("setDate", value);
            }
        }
    };

    var rm = new ReceivingViewModel();
    ko.applyBindings(rm);
});

function _AddShipment() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_AddShipment",
        success: function (result) {
            $('#addShipmentDiv').html('');
            $('#addShipmentDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#departureDate').val(null);
            $('#estArrivalDate').val(null);

            GetDatePicker();

            $('#addShipmentModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditShipment(shipmentId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_EditShipment",
        data: { "shipmentId": shipmentId },
        success: function (result) {

            $('#editShipmentDiv').html('');
            $('#editShipmentDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            GetDatePicker();

            $('#editShipmentModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _AddBol() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_AddBol",
        success: function (result) {

            $('#addBolDiv').html('');
            $('#addBolDiv').html(result);

            $('#addBolModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditBol(bolId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_EditBol",
        data: { "bolId": bolId },
        dataType: "json",
        success: function (result) {

            $('#editBolDiv').html('');
            $('#editBolDiv').html(result);

            $('#editBolModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function getBolsByShipmenId(shipmentId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/GetBolsByShipmentId",
        data: { "shipmentId": shipmentId },
        dataType: "json",
        success: function (result) {
            bols = result.BillsOfLading;
            if (bols.length > 0) {
                $('#bols').DataTable().clear().draw();
                $('#bols').DataTable().rows.add(bols); // Add new data
                $('#bols').DataTable().columns.adjust().draw();
            }
            else {
                $('#bols').DataTable().clear().draw();
            }
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _AddBolPart() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_AddBolPart",
        success: function (result) {

            $('#addBolPartDiv').html('');
            $('#addBolPartDiv').html(result);

            $('#addBolPartModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _Receiving() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_Receiving",
        success: function (result) {
            $('#receivingModal').modal('show');

            $('.successAlert').hide();
            $('.errorAlert').hide();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};