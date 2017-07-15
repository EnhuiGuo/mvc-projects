$(document).ready(function () {

    var firstArrivalDate = true;

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#customerLink').addClass("current-link");

    var priceSheets = [];

    $('#viewHoldNotes').hide();
    $('#viewCancelNotes').hide();

    if (isHold) {
        $('#viewHoldNotes').show()
    }
    else if (isCanceled) {
        $('#viewCancelNotes').show();
    }

    var PriceSheetModel = function (priceSheetId, number, dueDate, totalWeight, priceSheetParts) {
        var self = this;

        self.selected = ko.observable(false);
        self.priceSheetId = ko.observable(priceSheetId);
        self.number = ko.observable(number);
        self.dueDate = ko.observable(dueDate);
        self.totalWeight = ko.observable(totalWeight);
        self.priceSheetParts = ko.observableArray([]);
        $.each(priceSheetParts, function (i, priceSheetPart) {
            self.priceSheetParts.push(priceSheetPart);
        });
    }

    var OrderPart = function (pId, ppId, pspId, psId, aQuantity, oQuantity, pNumber, pDescription, psNumber, aDate, rQuantity, uCost, uPrice, cupId) {
        var self = this;

        self.partId = ko.observable(pId);
        self.projectPartId = ko.observable(ppId);
        self.priceSheetPartId = ko.observable(pspId);
        self.priceSheetId = ko.observable(psId);
        self.availableQuantity = ko.observable(aQuantity);
        self.orderQuantity = ko.observable(oQuantity);
        self.partNumber = ko.observable(pNumber);
        self.partDescription = ko.observable(pDescription);
        self.priceSheetNumber = ko.observable(psNumber);
        self.estArrivalDate = ko.observable(aDate);
        self.receiptQuantity = ko.observable(rQuantity);
        self.unitCost = ko.observable(uCost);
        self.unitPrice = ko.observable(uPrice);
        self.customerOrderPartId = ko.observable(cupId);
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
        self.Selected = ko.observable(false);
    };

    var CustomerOrderViewModel = function () {
        var self = this;

        self.priceSheetId = ko.observable();
        self.priceSheetNumber = ko.observable();

        self.orderTypeDescription = ko.observable(orderTypeDescription);

        self.priceSheetPartId = ko.observable();
        self.priceSheetId = ko.observable();
        self.availableQuantity = ko.observable();
        self.orderQuantity = ko.observable();
        self.partNumber = ko.observable();
        self.partDescription = ko.observable();
        self.priceSheetNumber = ko.observable();
        self.estArrivalDate = ko.observable(estArrivalDate);
        self.shipDate = ko.observable(null);
        self.receiptQuantity = ko.observable();
        self.unitCost = ko.observable();
        self.unitPrice = ko.observable();
        self.extendedPrice = ko.observable();
        self.extendedCost = ko.observable();
        self.Selected = ko.observable(false);

        self.editQuantity = ko.observable();
        self.editPartNumber = ko.observable();
        self.editPartDescription = ko.observable();
        self.editUnitPrice = ko.observable();
        self.editUnitCost = ko.observable();

        self.selectedPart = ko.observable();

        self.orderParts = ko.observableArray([]);

        var orderPartsTable = $('#orderParts').DataTable({
            "autoWidth": false,
            "searching": false,
            "ordering": false,
            "paging": false,
            "info": false,
            "scrollY": true,
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
                        item.Selected(item.priceSheetPartId() == itemId);
                    });
                }
            }
        };

        self.selectProcedure = function (array, $data, event) {
            if (!event.ctrlKey && !event.metaKey && !event.shiftKey && event.target.nodeName != "INPUT")
            {
                $data.Selected(true);
                ko.utils.arrayForEach(array, function (item) {
                    if (item != $data)
                    {
                        item.Selected(false);
                    }
                });
            }
            else if (event.shiftKey && !event.ctrlKey && self._lastSelectedIndex > -1)
            {
                var myIndex = array.indexOf($data);
                if (myIndex > self._lastSelectedIndex)
                {
                    for (var i = self._lastSelectedIndex; i <= myIndex; i++)
                    {
                        array[i].Selected(true);
                    }
                }
                else if (myIndex < self._lastSelectedIndex)
                {
                    for (var i = myIndex; i <= self._lastSelectedIndex; i++)
                    {
                        array[i].Selected(true);
                    }
                }
            }
            else if (event.ctrlKey && !event.shiftKey)
            {
                $data.Selected(!$data.Selected());
            }
            self._lastSelectedIndex = array.indexOf($data);
        }

        $.each(orderParts, function (n, part) {
            var formattedPrice = parseFloat(part.Price).toFixed(2);
            var formattedCost = parseFloat(part.Cost).toFixed(2);
            var estArrivalDate = new Date(parseInt(part.EstArrivalDate.substr(6))).toLocaleDateString();

            self.orderParts.push(new OrderPart(part.PartId, part.ProjectPartId, part.PriceSheetPartId, part.PriceSheetId, part.AvailableQuantity, part.CustomerOrderQuantity, part.PartNumber, part.PartDescription,
                                               part.PriceSheetNumber, estArrivalDate, part.ReceiptQuantity, formattedCost, formattedPrice, part.CustomerOrderPartId));

            $('#orderParts').DataTable().columns.adjust();
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
            var poNumber = $('#poNumber').val();

            $.confirm({
                text: "Are you sure you want delete all selected parts?",
                dialogClass: "modal-confirm",
                confirmButton: "Yes",
                confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                cancelButton: "No",
                cancelButtonClass: 'btn btn-sm btn-default',
                closeIcon: false,
                confirm: function (button) {

                    var partsToRemove = ko.utils.arrayFilter(self.orderParts(), function (part) {
                        return part.Selected() === true;
                    });

                    self.orderParts.removeAll(partsToRemove);
                    updatePriceSheetTextArea();

                    for (var i = 0; i < partsToRemove.length > 0; i++) {
                        signalR.server.sendRoleNotification(currentUser + " has removed " + partsToRemove[i].partNumber() + " from " + poNumber, "Admin");
                    }

                    partsToRemove = [];
                },
                cancel: function (button) {

                }
            });
        };

        self.orderPriceTotal = ko.observable();

        self.orderPriceTotal = ko.computed(function () {
            var total = 0;

            for (var i = 0; i < self.orderParts().length; i++) {
                if (self.orderParts()[i].extendedPrice() != undefined &&
                    self.orderParts()[i].extendedPrice() !== "") {
                    total += parseFloat(self.orderParts()[i].extendedPrice());
                }
            }

            return parseFloat(Math.round(total * 100) / 100).toFixed(2);
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

        $(document).on('click', '#choicePriceSheetBtn', function () {

            var selectedPriceSheets = $('#priceSheetTable').DataTable().rows('.selected').data();

            $.each(selectedPriceSheets, function (i, priceSheet) {
                getPartsByPriceSheet(priceSheet.PriceSheetParts);
            });

            updatePriceSheetTextArea();
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
            holdExpirationDate = null;
            holdNotes = null;
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
                canceledDate = null;
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

        $(document).on('click', '#updateCustomerOrderBtn', function () {
            event.preventDefault();

            if (self.orderParts().length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Parts to be ordered!</div>');
            }
            else {
                if (!$("#customerOrderForm")[0].checkValidity()) {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                    $('#customerOrderForm input[required]').each(function () {
                        if ($(this).val() === "") {
                            $(this).addClass("form-error");
                        }
                    });

                    $('#customerOrderForm select[required]').each(function () {
                        if ($(this).val() === "") {
                            $(this).addClass("form-error");
                        }
                    });
                }
                else {
                    var orderParts = [];
                    $.each(self.orderParts(), function (n, part) {
                        orderParts.push({
                            'PriceSheetPartId': part.priceSheetPartId(), 'PriceSheetId': part.priceSheetId(), 'CustomerOrderQuantity': part.orderQuantity(),
                            'PartNumber': part.partNumber(), 'PartDescription': part.partDescription(), 'PriceSheetNumber': part.priceSheetNumber(),
                            'EstArrivalDate': part.estArrivalDate(), 'Cost': part.unitCost(), 'Price': part.unitPrice(), 'PartId': part.partId(),
                            'ProjectPartId': part.projectPartId(), 'CustomerOrderPartId': part.customerOrderPartId()
                        });
                    });

                    var model = {
                        CustomerOrderId: customerOrderId,
                        PONumber: poNumber,
                        PODate: poDate,
                        DueDate: $('#editDueDate').val(),
                        PortDate: $('#editPortDate').val(),
                        ShipDate: self.shipDate(),
                        EstArrivalDate: self.estArrivalDate(),
                        ProjectId: projectId,
                        CustomerId: customerId,
                        CustomerAddressId: $('#editCustomerAddressId').val(),
                        FoundryId: foundryId,
                        SiteId: $('#editSiteId').val(),
                        OrderNotes: $('#editOrderNotes').val(),
                        ShipmentTermsId: shipmentTermsId,
                        IsSample: isSample,
                        IsTooling: isTooling,
                        IsProduction: isProduction,
                        IsOpen: $('#open').prop("checked"),
                        IsHold: $('#hold').prop("checked"),
                        HoldExpirationDate: holdExpirationDate,
                        HoldNotes: holdNotes,
                        IsCanceled: $('#canceled').prop("checked"),
                        CanceledDate: canceledDate,
                        CancelNotes: cancelNotes,
                        IsComplete: $('#completed').prop("checked"),
                        CompletedDate: completedDate,
                        customerOrderParts: orderParts,
                    };

                    $.ajax({
                        type: "PUT",
                        url: "/SouthlandMetals/Operations/PurchaseOrder/EditCustomerOrder",
                        data: JSON.stringify(model),
                        contentType: "application/json",
                        dataType: "json",
                        success: function (result) {
                            if (result.Success) {
                                orderParts = [];
                                shipmentTermsId = null;
                                if (result.IsHold) {
                                    signalR.server.sendRoleNotification(currentUser + " has put Customer PO " + poNumber + " on hold til " + holdExpirationDate, "Admin");
                                }

                                if (result.IsCanceled) {
                                    signalR.server.sendRoleNotification(currentUser + " has canceled Customer PO " + poNumber, "Admin");
                                }

                                window.location.href = "/SouthlandMetals/Operations/PurchaseOrder/CustomerOrders"
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

        $(document).on('click', '#cancelOrderUpdateBtn', function () {
            window.history.back();
        });

        function getPartsByPriceSheet(priceSheetParts)
        {
            if (priceSheetParts.length > 0) {
                $.each(priceSheetParts, function (n, part) {
                    var formattedPrice = parseFloat(part.UnitPrice).toFixed(2);
                    var formattedCost = parseFloat(part.UnitCost).toFixed(2);

                    self.orderParts.push(new OrderPart(part.PartId, part.ProjectPartId, part.PriceSheetPartId, part.PriceSheetId, part.AvailableQuantity, part.CustomerOrderQuantity, part.PartNumber, part.PartDescription,
                                                       part.PriceSheetNumber, part.EstArrivalDate, null, formattedCost, formattedPrice));
                });

                $('#orderParts').DataTable().columns.adjust();
            }
        }

        var priceSheetTable = $('#priceSheetTable').DataTable({
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
                        priceSheetTable.rows().select();

                    }
                },
                {
                    text: 'Deselect All',
                    className: 'btn btn-sm btn-danger',
                    action: function () {
                        priceSheetTable.rows().deselect();
                    }
                }
            ],
            "autoWidth": false,
            "pageLength": 20,
            "lengthChange": false,
            "data": priceSheets,
            "columns": [
                { "data": "PriceSheetId", "title": "PriceSheetId", "visible": false },
                { "data": "Number", "title": "Number", "class": "center" },
                { "data": "PriceSheetType", "title": "Type", "class": "center" },
                { "data": "TotalWeight", "title": "Total Weight", "class": "center" },
                { "data": "DueDate", "title": "Due Date", "class": "center" }
            ],
        });

        updatePriceSheetTextArea()

        function updatePriceSheetTextArea() {

            $('#priceSheets').html("");
            $('#priceSheets').empty();

            var priceSheetList = self.orderParts().map(function (part) {
                return part.priceSheetNumber();
            });

            priceSheetList = priceSheetList.filter(function (priceSheetNumber, i, priceSheetList) {
                return i == priceSheetList.indexOf(priceSheetNumber);
            });

            if (priceSheetList.length > 0) {
                $.each(priceSheetList, function (n, priceSheet) {
                    if (n > 0) {
                        $('#priceSheets').append(", " + priceSheet);
                    }
                    else {
                        $('#priceSheets').append(priceSheet);
                    }
                });
            }
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
                                for (var i = 0; i < cm.orderParts().length; i++) {
                                    cm.orderParts()[i].estArrivalDate($(element).val());
                                }
                            },
                        });
                    }
                }
            }
        }
    };

    $("#addParts").on('click', function () {
        if (projectId == null) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please Choice the project first!</div>');
        }
        else {
            _AddParts();
        }
    });

    function getPriceSheetsByProject(projectId, fromDate, toDate) {

        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Pricing/GetPriceSheetsByProject",
            data: { 'projectId': projectId, 'fromDate': fromDate, 'toDate': toDate, 'orderType': orderTypeDescription },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                priceSheets = result;
                $.each(priceSheets, function (i, priceSheet) {
                    priceSheet.DueDate = new Date(parseInt(priceSheet.DueDate.substr(6))).toLocaleDateString();
                });
                $('#priceSheetTable').DataTable().clear().draw();
                $('#priceSheetTable').DataTable().rows.add(priceSheets); // Add new data
                $('#priceSheetTable').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _AddParts(projectId) {
        $.ajax({
            type: "GET",
            cache: "false",
            url: "/SouthlandMetals/Operations/PurchaseOrder/_AddPriceSheetParts",
            success: function (result) {

                $('#addPartsModal').show();

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

    $(document).on('click', '#getPriceSheetBtn', function () {
        if ($('#fromDate').val() != "" && $('#toDate').val() && $('#fromDate').val() > $('#toDate').val()) {
            alert("'from date' can not after 'to date'");
        }
        else {
            getPriceSheetsByProject(projectId, $('#fromDate').val(), $('#toDate').val());
        }
    });

    var cm = new CustomerOrderViewModel();
    ko.applyBindings(cm);

    $('#orderParts').DataTable().columns.adjust();
});



