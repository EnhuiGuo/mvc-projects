$(document).ready(function () {

    var firstShipDate = true;
    var firstArrivalDate = true;

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#foundryLink').addClass("current-link");

    $('#editIsConfirmed').prop('checked', isConfirmed);

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show()
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    var customerOrders = [];

    var OrderPart = function (fpId, cpId, oId, aQuantity, oQuantity, pNumber, pDescription, oNumber, sCode, sNotes, aDate, sDate, uCost, uPrice, pId, ppId, isScheduled, received, receiptDate, rQuantity) {
        var self = this;

        self.foundryOrderPartId = ko.observable(fpId);
        self.partId = ko.observable(pId);
        self.projectPartId = ko.observable(ppId);
        self.customerOrderPartId = ko.observable(cpId);
        self.customerOrderId = ko.observable(oId);
        self.availableQuantity = ko.observable(aQuantity);
        self.orderQuantity = ko.observable(oQuantity);
        self.partNumber = ko.observable(pNumber);
        self.partDescription = ko.observable(pDescription);
        self.poNumber = ko.observable(oNumber);
        self.shipCode = ko.observable(sCode);
        self.shipCodeNotes = ko.observable(sNotes);
        self.estArrivalDate = ko.observable(aDate);
        self.shipDate = ko.observable(sDate);
        self.isScheduled = ko.observable(isScheduled);
        self.hasBeenReceived = ko.observable(received);
        self.receiptDate = ko.observable(receiptDate);
        self.receiptQuantity = ko.observable(rQuantity);
        self.unitPrice = ko.observable(uPrice);
        self.unitCost = ko.observable(uCost);
        self.extendedPrice = ko.computed(function () {
            if (self.orderQuantity() != undefined &&
                    self.orderQuantity() !== "") {
                var extPrice = parseFloat(self.orderQuantity()) * parseFloat(self.unitPrice());
                var precisionExtPrice = parseFloat(Math.round(extPrice * 100) / 100).toFixed(2);
                return precisionExtPrice;
            }
            else {
                return "";
            }
        });
        self.extendedCost = ko.computed(function () {
            if (self.orderQuantity() != undefined &&
                    self.orderQuantity() !== "") {
                var extCost = parseFloat(self.orderQuantity()) * parseFloat(self.unitCost());
                var precisionExtCost = parseFloat(Math.round(extCost * 100) / 100).toFixed(2);
                return precisionExtCost;
            }
            else {
                return "";
            }
        });

        self.editShipCodeAndNotes = function (part) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Operations/PurchaseOrder/_EditShipCodeAndNotes",
                success: function () {

                    $('#shipCodeCustomerOrderPartId').val(part.customerOrderPartId());
                    $('#shipCodeNumber').val(part.shipCode());
                    $('#shipCodeNotes').val(part.shipCodeNotes());

                    $('.successAlert').hide();
                    $('.errorAlert').hide();

                    $('#editShipCodeNotesModal').modal('show');
                },
                error: function (req, err) {
                    console.log('Error ' + err.responseText);
                }
            });
        };

        self.Selected = ko.observable(false);
    };

    var FoundryOrderViewModel = function () {
        var self = this;

        self.customerOrderId = ko.observable();
        self.customerOrders = ko.observableArray(customerOrders);

        self.orderTypeDescription = ko.observable(orderTypeDescription);

        self.customerOrderPartId = ko.observable();
        self.customerOrderId = ko.observable();
        self.availableQuantity = ko.observable();
        self.orderQuantity = ko.observable();
        self.partNumber = ko.observable();
        self.partDescription = ko.observable();
        self.poNumber = ko.observable();
        self.shipCode = ko.observable(null);
        self.estArrivalDate = ko.observable(estArrivalDate);
        self.shipDate = ko.observable(shipDate);
        self.receiptDate = ko.observable();
        self.receiptQuantity = ko.observable();
        self.unitPrice = ko.observable();
        self.unitCost = ko.observable();
        self.extendedPrice = ko.observable();
        self.extendedCost = ko.observable();
        self.Selected = ko.observable(false);

        self.editQuantity = ko.observable();
        self.editPartNumber = ko.observable();
        self.editPartDescription = ko.observable();
        self.editUnitCost = ko.observable();

        self.selectedPart = ko.observable();

        self.orderParts = ko.observableArray([]);

        var orderPartsTable = $('#orderParts').DataTable({
            "autoWidth": false,
            "searching": false,
            "ordering": false,
            "paging": false,
            "info": false,
            "scrollY": 475,
            "scrollX": true,
            "scrollCollapse": true
        });

        self.multiSelectableOptions = {
            helper: function (event, $item) {
                var dbId = $item.parent().attr('id'),
                    itemId = $item.attr('id'),
                    db = sm['partsList' + dbId];

                if (!$item.hasClass('selected')) {
                    ko.utils.arrayForEach(db(), function (item) {
                        item.Selected(item.customerOrderPartId() == itemId);
                    });
                }
            }
        };

        self.selectProcedure = function (array, $data, event) {
            if (!event.ctrlKey && !event.metaKey && !event.shiftKey && event.target.nodeName != "INPUT") {
                $data.Selected(true);
                ko.utils.arrayForEach(array, function (item) {
                    if (item !== $data) {
                        item.Selected(false);
                    }
                });
            }
            else if (event.shiftKey && !event.ctrlKey && self._lastSelectyedIndex > -1) {
                var myIdenx = array.indexOf($data);
                if (myIndex > self._lastSelectedIndex) {
                    for (var i = self._lastSelectedIndex; i <= myIdenx; i++) {
                        array[i].Selected(true);
                    }
                }
                else if (myIndex < self._lastSelectedIndex) {
                    for (var i = myIndex; i <= self._lastSelectedIndex; i++) {
                        array[i].Selected(true);
                    }
                }
            }
            else if (event.ctrlKey && !event.shiftKey) {
                $data.Selected(!$data.Selected());
            }
            self._lastSelectedIndex = array.indexOf($data);
        }

        $.each(orderParts, function (n, part) {

            var formattedPrice = parseFloat(part.Price).toFixed(2);
            var formattedCost = parseFloat(part.Cost).toFixed(2);

            self.orderParts.push(new OrderPart(part.FoundryOrderPartId, part.CustomerOrderPartId, part.CustomerOrderId, part.AvailableQuantity, part.FoundryOrderQuantity, part.PartNumber, part.PartDescription, part.PONumber,
                                               part.ShipCode, part.ShipCodeNotes, part.EstArrivalDate, part.ShipDate, formattedCost, formattedPrice, part.PartId, part.ProjectPartId, part.IsScheduled, part.HasBeenReceived, part.ReceiptDateStr, part.ReceiptQuantity));

        });

        self.selectAll = function (list) {
            ko.utils.arrayForEach(list(), function (item) {
                item.Selected(true);
            });
        };

        self.deselectAll = function (list) {
            ko.utils.arrayForEach(list(), function (item) {
                item.Selected(false);
            });
        };

        self.remove = function () {

            $.confirm({
                text: "Do you want delete all selected parts?",
                dialogClass: "modal-confirm",
                confirmButton: "Yes",
                confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                cancelButton: "No",
                cancelButtonClass: 'btn btn-sm btn-default',
                closeIcon: false,
                confirm: function (button) {

                    var partsToRemove = ko.utils.arrayFilter(self.orderParts(), function (part) {
                        return part.Selected() === true && part.hasBeenReceived() === false;
                    });

                    self.orderParts.removeAll(partsToRemove);
                    updateCustomerOrderTextArea();
                    updateShipCodeTextArea();

                    var receivedParts = ko.utils.arrayFilter(self.orderParts(), function (part) {
                        return part.Selected() === true && part.hasBeenReceived() === true;
                    });

                    if (receivedParts !== null && receivedParts.length > 0) {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Some parts were not removed because they have already been received.</div>');
                    }
                },
                cancel: function (button) {

                }
            });
        };

        var customerOrderTable = $('#customerOrderTable').DataTable({
            dom: 'Bfrt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
            select: {
                style: 'os'
            },
            buttons: [
                {
                    text: 'Select All',
                    className: 'btn btn-sm btn-primary',
                    action: function () {
                        customerOrderTable.rows().select();

                    }
                },
                {
                    text: 'Deselect All',
                    className: 'btn btn-sm btn-danger',
                    action: function () {
                        customerOrderTable.rows().deselect();
                    }
                }
            ],
            "autoWidth": false,
            "pageLength": 25,
            "lengthChange": false,
            "data": customerOrders,
            "columns": [
                { "data": "CustomerOrderId", "title": "CustomerOrderId", "visible": false },
                { "data": "PONumber", "title": "PO", "class": "center" },
                { "data": "OrderTypeDescription", "title": "Type", "class": "center" },
                { "data": "DueDate", "title": "Due Date", "class": "center" }
            ],
        });

        self.orderCostTotal = ko.observable();

        self.orderCostTotal = ko.computed(function () {
            var total = 0;

            for (var i = 0; i < self.orderParts().length; i++) {
                if (self.orderParts()[i].extendedCost() != undefined &&
                    self.orderParts()[i].extendedCost() !== "") {
                    total += parseFloat(self.orderParts()[i].extendedCost());
                }
            }

            return parseFloat(Math.round(total * 100) / 100).toFixed(2);
        });

        self.shipCode.subscribe(function (value) {

            if (self.orderParts().length > 0) {

                $.confirm({
                    text: "Would you like to change all Ship Codes to the entered code?",
                    dialogClass: "modal-confirm",
                    confirmButton: "Yes",
                    confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                    cancelButton: "No",
                    cancelButtonClass: 'btn btn-sm btn-default',
                    closeIcon: false,
                    confirm: function (button) {
                        for (var i = 0; i < self.orderParts().length; i++) {
                            self.orderParts()[i].shipCode(value);
                        }
                    },
                    cancel: function (button) {

                    }
                });
            }
        });

        self.shipDate.subscribe(function (date) {

            if (!firstShipDate) {
                if (self.orderParts().length > 0) {

                    $.confirm({
                        text: "Would you like to change all Ship Dates to the selected date?",
                        dialogClass: "modal-confirm",
                        confirmButton: "Yes",
                        confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                        cancelButton: "No",
                        cancelButtonClass: 'btn btn-sm btn-default',
                        closeIcon: false,
                        confirm: function (button) {
                            date = date.toLocaleDateString();

                            for (var i = 0; i < self.orderParts().length; i++) {
                                self.orderParts()[i].shipDate(date);
                                self.orderParts()[i].isScheduled(true);
                            }
                        },
                        cancel: function (button) {

                        }
                    });
                }
            }
            firstShipDate = false;
        });

        self.estArrivalDate.subscribe(function (date) {

            if (!firstArrivalDate) {
                if (self.orderParts().length > 0) {

                    $.confirm({
                        text: "Would you like to change all Est Arrival Dates to the selected date?",
                        dialogClass: "modal-confirm",
                        confirmButton: "Yes",
                        confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                        cancelButton: "No",
                        cancelButtonClass: 'btn btn-sm btn-default',
                        closeIcon: false,
                        confirm: function (button) {
                            date = date.toLocaleDateString();

                            for (var i = 0; i < self.orderParts().length; i++) {
                                self.orderParts()[i].estArrivalDate(date);
                            }
                        },
                        cancel: function (button) {

                        }
                    });
                }
            }
            firstArrivalDate = false;
        });

        $('#addParts').on('click', function () {
            if (projectId == null) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please Choice the project first!</div>');
            }
            else {
                _AddParts();
            }
        });

        $(document).on('click', '#getCustomerOrderBtn', function () {
            if ($('#fromDate').val() != "" && $('#toDate').val() && $('#fromDate').val() > $('#toDate').val()) {
                alert("'from date' can not after 'to date'");
            }
            else {
                getOrdersByProject(projectId, $('#fromDate').val(), $('#toDate').val());
            }
        });

        $(document).on('click', '#choiceCustomerOrderBtn', function () {
            var selectedCustomerOrders = $('#customerOrderTable').DataTable().rows('.selected').data();

            $.each(selectedCustomerOrders, function (i, customerOrder) {
                getPartsByCustomerOrder(customerOrder.CustomerOrderParts);
            });

            updateCustomerOrderTextArea();
            updateShipCodeTextArea();

            $('#addPartsModal').modal('hide');
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
            else if (selectedVal === 'Completed') {
                completedDate = new Date();
            }
        });

        $("#editIsConfirmed").change(function () {
            if (this.checked) {
                confirmedDate = new Date();
            }
            else {
                confirmedDate = null;
            }
        });

        $(document).on('click', '#saveHoldNoteBtn', function () {

            holdExpirationDate = $('#holdExpirationDate').val();
            holdNotes = $('#holdNotes').val();

            if (holdExpirationDate === "" || holdNotes === "") {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('.errorAlert').show();

                event.preventDefault();

                $('#addOrderHoldNoteForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#addOrderHoldNoteForm textarea[required]').each(function () {
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
            $('#addHoldNotesModal').modal('hide');
        });

        $(document).on('click', '#saveCancelNoteBtn', function () {
            event.preventDefault();
            cancelNotes = $('#cancelNotes').val();
            canceledDate = new Date();
            if (cancelNotes === "") {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('.errorAlert').show();

                event.preventDefault();

                $('#addOrderCancelNoteForm textarea[required]').each(function () {
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

        $(document).on('click', '#updateFoundryOrderBtn', function () {

            event.preventDefault();

            if (self.orderParts().length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Parts to be ordered!</div>');
            }
            else {
                if (!$("#foundryOrderForm")[0].checkValidity()) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                    $('#foundryOrderForm input[required]').each(function () {
                        if ($(this).val() === "") {
                            $(this).addClass("form-error");
                        }
                    });

                    $('#foundryOrderForm select[required]').each(function () {
                        if ($(this).val() === "") {
                            $(this).addClass("form-error");
                        }
                    });
                }
                else {
                    var orderParts = [];
                    var noShipDatesParts = [];

                    $.each(self.orderParts(), function (n, part) {
                        orderParts.push({
                            'PartId': part.partId(), 'ProjectPartId': part.projectPartId(), 'Quantity': part.orderQuantity(), 'PartNumber': part.partNumber(),
                            'PartDescription': part.partDescription(), 'ShipCode': part.shipCode(), 'ShipCodeNotes': part.shipCodeNotes(), 'EstArrivalDate': part.estArrivalDate(), 'ShipDate': part.shipDate(),
                            'Cost': part.unitCost(), 'Price': part.unitPrice(), 'CustomerOrderPartId': part.customerOrderPartId(), 'FoundryOrderPartId': part.foundryOrderPartId(),
                            'IsScheduled': part.isScheduled(), 'HasBeenReceived': part.hasBeenReceived(), 'ReceiptDate': part.receiptDate(), 'ReceiptQuantity': part.receiptQuantity()
                        });

                        if (part.shipCode() != null && part.shipCode() != "" && part.shipCode() != "N/A" && (part.shipDate() == null || part.shipDate() == "" || part.shipDate() == "N/A")) {
                            noShipDatesParts.push(part);
                        }
                    });

                    if (noShipDatesParts.length > 0) {
                        var alertString = "";
                        $.each(noShipDatesParts, function (i, part) {
                            alertString += (part.partNumber() + " ");
                        })
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                            '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                            '<strong>Warning!</strong>&nbsp;' + alertString + 'do not have shipDate</div>');
                    }
                    else {
                        var model = {
                            FoundryOrderId: foundryOrderId,
                            OrderNumber: orderNumber,
                            CustomerId: customerId,
                            ProjectId: projectId,
                            CustomerAddressId: $('#editCustomerAddressId').val(),
                            FoundryId: foundryId,
                            SiteId: $('#editSiteId').val(),
                            ShipmentTermsId: shipmentTermsId,
                            OrderDate: $('#poDate').val(),
                            ShipDate: self.shipDate(),
                            EstArrivalDate: self.estArrivalDate(),
                            DueDate: $('#editDueDate').val(),
                            PortDate: $('#editPortDate').val(),
                            OrderNotes: $('#editOrderNotes').val(),
                            ShipVia: $('#shipVia').val(),
                            TransitDays: $('#editTransitDays').val(),
                            IsConfirmed: $('#editIsConfirmed').prop('checked'),
                            IsHold: $('#hold').prop("checked"),
                            HoldExpirationDate: holdExpirationDate,
                            HoldNotes: holdNotes,
                            ConfirmedDate: confirmedDate,
                            IsCanceled: $('#canceled').prop("checked"),
                            CanceledDate: canceledDate,
                            CancelNotes: cancelNotes,
                            IsComplete: $('#completed').prop("checked"),
                            CompletedDate: completedDate,
                            IsOpen: $('#open').prop("checked"),
                            IsSample: isSample,
                            IsTooling: isTooling,
                            IsProduction: isProduction,
                            FoundryOrderParts: orderParts
                        };

                        $.ajax({
                            type: "PUT",
                            url: "/SouthlandMetals/Operations/PurchaseOrder/EditFoundryOrder",
                            data: JSON.stringify(model),
                            contentType: "application/json",
                            dataType: "json",
                            success: function (result) {
                                if (result.Success) {
                                    orderParts = [];
                                    shipmentTermsId = null;
                                    if (result.IsHold) {
                                        signalR.server.sendRoleNotification(currentUser + " has put Foundry Order " + orderNumber + " on hold til " + holdExpirationDate, "Admin");
                                    }

                                    if (result.IsCanceled) {
                                        signalR.server.sendRoleNotification(currentUser + " has canceled Foundry Order " + orderNumber, "Admin");
                                    }
                                    window.location.href = "/SouthlandMetals/Operations/PurchaseOrder/FoundryOrders"
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
            }
        });

        $(document).on('click', '#cancelOrderUpdateBtn', function () {
            window.history.back();
        });

        function _AddParts() {
            $.ajax({
                type: "GET",
                cache: "false",
                url: "/SouthlandMetals/Operations/PurchaseOrder/_AddCustomerOrderParts",
                success: function (result) {
                    $('.successAlert').hide();
                    $('.errorAlert').hide();
                    $('#addPartsModal').modal('show');
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
                url: "/SouthlandMetals/Notes/_AddHoldNotes",
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
                url: "/SouthlandMetals/Notes/_ViewCancelNotes",
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

        function getOrdersByProject(projectId, fromDate, toDate) {

            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Operations/PurchaseOrder/GetOrdersByProject",
                data: { 'projectId': projectId, 'fromDate': fromDate, 'toDate': toDate, 'orderType': self.orderTypeDescription() },
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    customerOrders = result;
                    $.each(customerOrders, function (i, customerOrder) {
                        if (customerOrder.DueDate != null) {
                            customerOrder.DueDate = new Date(parseInt(customerOrder.DueDate.substr(6))).toLocaleDateString();
                        }
                    });
                    $('#customerOrderTable').DataTable().clear().draw();
                    $('#customerOrderTable').DataTable().rows.add(customerOrders); // Add new data
                    $('#customerOrderTable').DataTable().columns.adjust().draw();
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        };

        function getPartsByCustomerOrder(orderParts) {

            if (orderParts != null && orderParts.length > 0) {
                $.each(orderParts, function (n, part) {
                    var estArrivalDate, shipDate;
                    var formattedPrice = parseFloat(part.Price).toFixed(2);
                    var formattedCost = parseFloat(part.Cost).toFixed(2);
                    if (part.EstArrivalDate != null) {
                        estArrivalDate = new Date(parseInt(part.EstArrivalDate.substr(6))).toLocaleDateString();
                    }
                    if (part.ShipDate != null) {
                        shipDate = new Date(parseInt(part.ShipDate.substr(6))).toLocaleDateString();
                    }

                    var existingPart = self.orderParts().filter(function (orderPart) {
                        return orderPart.customerOrderPartId() == part.CustomerOrderPartId;
                    });

                    if (existingPart.length == 0) {
                        self.orderParts.push(new OrderPart(part.FoundryOrderPartId, part.CustomerOrderPartId, part.CustomerOrderId, part.AvailableQuantity, part.CustomerOrderQuantity, part.PartNumber, part.PartDescription, part.PONumber,
                                   "N/A", null, estArrivalDate, shipDate, formattedCost, formattedPrice, part.PartId, part.ProjectPartId, false, false, null, null));
                    }
                });

                $('#orderParts').DataTable().columns.adjust();
            }
        };


        function addCustomerOrderRecords(customerOrder) {
            if (customerOrder.CustomerOrderParts.length > 0) {

                var index = -1;
            }
        };

        updateCustomerOrderTextArea();

        function updateCustomerOrderTextArea() {

            $('#customerOrders').html("");
            $('#customerOrders').empty();

            var customerOrderList = self.orderParts().map(function (part) { return part.poNumber(); });

            customerOrderList = customerOrderList.filter(function (orderNumber, i, customerOrderList) {
                return i == customerOrderList.indexOf(orderNumber);
            });

            $.each(customerOrderList, function (n, customerOrder) {
                if (n == 0) {
                    $('#customerOrders').append(customerOrder);
                }
                else {
                    $('#customerOrders').append(", " + customerOrder);
                }
            });
        };


        updateShipCodeAndNotes = function () {

            var customerOrderPartId = $('#shipCodeCustomerOrderPartId').val();
            var shipCode = $('#shipCodeNumber').val();
            var shipCodeNotes = $('#shipCodeNotes').val();

            if (shipCode == "") {
                $('.errorAlert').show();
                $('#shipCodeForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#shipCodeForm select[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                $.confirm({
                    text: "Would you like to apply this ship code to all parts?",
                    dialogClass: "modal-confirm",
                    confirmButton: "Yes",
                    confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                    cancelButton: "No",
                    cancelButtonClass: 'btn btn-sm btn-default',
                    closeIcon: false,
                    confirm: function (button) {
                        for (var i = 0; i < self.orderParts().length; i++) {
                            self.orderParts()[i].shipCode(shipCode);
                            self.orderParts()[i].shipCodeNotes(shipCodeNotes);
                        }
                        $('#editShipCodeNotesModal').modal('hide');
                        updateShipCodeTextArea();
                    },
                    cancel: function (button) {
                        for (var i = 0; i < self.orderParts().length; i++) {
                            if (self.orderParts()[i].customerOrderPartId() == customerOrderPartId) {
                                self.orderParts()[i].shipCode(shipCode);
                                self.orderParts()[i].shipCodeNotes(shipCodeNotes);
                            }
                        }
                        $('#editShipCodeNotesModal').modal('hide');
                        updateShipCodeTextArea();
                    }
                });
            }
        };

        updateShipCodeTextArea();

        function updateShipCodeTextArea() {

            $('#shipCodes').html("");
            $('#shipCodes').empty();

            var shipCodeList = self.orderParts().map(function (part) { return part.shipCode(); });

            shipCodeList = shipCodeList.filter(function (shipCode, i, shipCodeList) {
                return i == shipCodeList.indexOf(shipCode);
            });

            $.each(shipCodeList, function (n, shipCode) {
                if (n == 0) {
                    $('#shipCodes').append(shipCode);
                }
                else {
                    $('#shipCodes').append(", " + shipCode);
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
                $(this).datepicker('hide'); //hide the datepicker after selection
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
            else {
                if ($(element).val() !== "") {
                    var dateType = $(element).attr('class');

                    if (dateType.includes("est-arrival-date")) {

                        $.confirm({
                            text: "Would you like to change all Est Arrival Dates to the selected date?",
                            dialogClass: "modal-confirm",
                            confirmButton: "Yes",
                            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                            cancelButton: "No",
                            cancelButtonClass: 'btn btn-sm btn-default',
                            closeIcon: false,
                            confirm: function (button) {
                                for (var i = 0; i < fm.orderParts().length; i++) {
                                    fm.orderParts()[i].estArrivalDate($(element).val());
                                }
                            },
                            cancel: function (button) {

                            }
                        });
                    }
                    else if (dateType.includes("ship-date")) {

                        $.confirm({
                            text: "Would you like to change all Ship Dates to the selected date?",
                            dialogClass: "modal-confirm",
                            confirmButton: "Yes",
                            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                            cancelButton: "No",
                            cancelButtonClass: 'btn btn-sm btn-default',
                            closeIcon: false,
                            confirm: function (button) {
                                for (var i = 0; i < fm.orderParts().length; i++) {
                                    fm.orderParts()[i].shipDate($(element).val());
                                    fm.orderParts()[i].isScheduled(true);
                                }
                            },
                            cancel: function (button) {

                            }
                        });
                    }
                }
            }
        }
    };

    $('#newShipCodeNumber').on('click', function () {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/PurchaseOrder/newShipCodeNumber",
            success: function (data) {

                var shipCodes = fm.orderParts().map(function (part) {
                    return parseInt(part.shipCode().replace(/[^0-9\.]/g, ''), 10);
                });

                var maxShipCodeNumber = Math.max.apply(Math, shipCodes);
                if (parseInt(data) <= maxShipCodeNumber) {
                    $('#shipCodeNumber').val(customerId[0] + (maxShipCodeNumber + 1));
                }
                else {
                    $('#shipCodeNumber').val(customerId[0] + data);
                }
            },
            error: function (req, err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    var fm = new FoundryOrderViewModel();
    ko.applyBindings(fm);

    $('#orderParts').DataTable().columns.adjust();
});



