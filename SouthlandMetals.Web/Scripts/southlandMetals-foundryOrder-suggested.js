$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#foundryLink').addClass("current-link");

    var shipmentTermsId = null;
    var customerOrders = [];
    var customerOrderList = [];
    var shipCodeList = [];

    var OrderPart = function (pId, cpId, aQuantity, oQuantity, pNumber, pDescription, oId, oNumber, sCode, aDate, sDate, uCost) {
        var self = this;

        self.partId = ko.observable(pId);
        self.customerOrderPartId = ko.observable(cpId);
        self.availableQuantity = ko.observable(aQuantity);
        self.orderQuantity = ko.observable(oQuantity);
        self.partNumber = ko.observable(pNumber);
        self.partDescription = ko.observable(pDescription);
        self.customerOrderId = ko.observable(oId)
        self.orderNumber = ko.observable(oNumber);
        self.shipCode = ko.observable(sCode);
        self.estArrivalDate = ko.observable(aDate);
        self.shipDate = ko.observable(sDate);

        self.unitCost = ko.observable(uCost);
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

    var FoundryOrderViewModel = function () {
        var self = this;

        self.projectId = ko.observable();
        self.projects = ko.observableArray([]);

        self.customerOrders = ko.observableArray([]);

        self.orderTypeId = ko.observable();
        self.orderTypes = ko.observableArray(orderTypes);

        self.orderTypeId.subscribe(function (newOrderTypeId) {
            var newOrderType = ko.utils.arrayFirst(ko.toJS(self.orderTypes), function (orderType) {
                return orderType.Value === newOrderTypeId;
            });
        }.bind(self));

        self.customerId = ko.observable(customerId);
        self.addressId = ko.observable(addressId);
        self.siteId = ko.observable();

        self.customerId.subscribe(function (customerId) {
            getProjectsByCustomer(customerId);
            getAddressesByCustomer(customerId);
            getSitesByCustomer(customerId);
            getTermsByCustomer(customerId);
        });

        self.customers = ko.observableArray(customers);
        self.addresses = ko.observableArray([]);
        self.sites = ko.observableArray([]);
        self.poDate = ko.observable();

        self.dueDate = ko.observable();
        self.portDate = ko.observable();

        self.partId = ko.observable();
        self.availableQuantity = ko.observable();
        self.orderQuantity = ko.observable();
        self.partNumber = ko.observable();
        self.partDescription = ko.observable();
        self.poNumber = ko.observable();
        self.shipCode = ko.observable();
        self.estArrivalDate = ko.observable(null);
        self.shipDate = ko.observable(null);
        self.unitCost = ko.observable();
        self.extendedCost = ko.observable();
        self.Selected = ko.observable(false);

        self.editQuantity = ko.observable();
        self.editPartNumber = ko.observable();
        self.editPartDescription = ko.observable();
        self.editUnitCost = ko.observable();

        self.selectedPart = ko.observable();

        self.orderParts = ko.observableArray([]);

        getProjectsByCustomer(customerId);
        getSitesByCustomer(customerId);
        getTermsByCustomer(customerId);

        $.each(parts, function (n, part) {
            self.orderParts.push(new OrderPart(part.PartId, null, part.OrderQuantity, part.OrderQuantity, part.PartNumber,
                                               part.PartDescription, null, null, null, formatDates(new Date()), formatDates(new Date()), part.UnitCost))
        });

        var orderPartsTable = $('#orderParts').DataTable({
            "autoWidth": false,
            "searching": false,
            "ordering": false,
            "paging": false,
            "info": false,
            "scrollY": 475,
            "scrollCollapse": true
        });

        self.multiSelectableOptions = {
            helper: function (event, $item) {
                var dbId = $item.parent().attr('id'),
                    itemId = $item.attr('id'),
                    db = sm['partsList' + dbId];

                if (!$item.hasClass('selected')) {
                    ko.utils.arrayForEach(db(), function (item) {
                        item.Selected(item.partId() == itemId);
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
            else if (event.shiftKey && !event.ctrlKey && self._lastSelectedIndex > -1) {
                var myIndex = array.indexOf($data);
                if (myIndex > self._lastSelectedIndex) {
                    for (var i = self._lastSelectedIndex; i <= myIndex; i++) {
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
                        return part.Selected() === true;
                    });
                    self.orderParts.removeAll(partsToRemove);
                    removeCustomerOrderRecords(partsToRemove);
                    removeShipCodeRecords(partsToRemove);
                    updateCustomerOrderTextArea();
                    updateShipCodeTextArea();
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
            "pageLength": 20,
            "lengthChange": false,
            "data": customerOrders,
            "columns": [
                { "data": "CustomerOrderId", "title": "Customer Order Id", "visible": false },
                { "data": "PONumber", "title": "Customer Order", "class": "center" },
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

                        var index = -1;
                        shipCodeList.some(function (shipCode, i) {
                            if (shipCode.ShipCode == value) {
                                index = i;
                                return true;
                            }
                        });

                        if (index == -1) {
                            shipCodeList.push({ 'ShipCode': value });
                        }
                        updateShipCodeTextArea();
                    },
                    cancel: function (button) {

                    }
                });
            }
        });

        self.shipDate.subscribe(function (date) {

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
                        for (var i = 0; i < self.orderParts().length; i++) {
                            self.orderParts()[i].shipDate(date);
                        }
                    },
                    cancel: function (button) {

                    }
                });
            }
        });

        self.estArrivalDate.subscribe(function (date) {

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
                        for (var i = 0; i < self.orderParts().length; i++) {
                            self.orderParts()[i].estArrivalDate(date);
                        }
                    },
                    cancel: function (button) {

                    }
                });
            }
        });

        $(document).on('click', '#saveFoundryOrderBtn', function () {
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

                    $.each(self.orderParts(), function (n, part) {
                        orderParts.push({
                            'PartId': part.partId(), 'AvailableQuantity': part.availableQuantity(), 'FoundryOrderQuantity': part.orderQuantity(), 'PartNumber': part.partNumber(),
                            'PartDescription': part.partDescription(), 'ShipCode': part.shipCode(), 'EstArrivalDate': part.estArrivalDate(), 'ShipDate': part.shipDate(),
                            'UnitCost': part.unitCost(), 'CustomerOrderPartId': part.customerOrderPartId(),
                        });
                    });

                    if (self.customerId() != null) {
                        var model = {
                            OrderTypeId: self.orderTypeId(),
                            OrderNumber: $('#orderNumber').val(),
                            PONumber: $('#poNumber').val(),
                            CustomerId: self.customerId(),
                            CustomerAddressId: self.addressId(),
                            FoundryId: $('#foundryId').val(),
                            SiteId: self.siteId(),
                            PODate: $('#poDate').val(),
                            DueDate: $('#dueDate').val(),
                            PortDate: $('#portDate').val(),
                            ShipDate: self.shipDate(),
                            EstArrivalDate: self.estArrivalDate(),
                            OrderNotes: $('#orderNotes').val(),
                            ShipmentTermsId: shipmentTermsId,
                            ShipmentTerms: $('#shipmentTerms').val(),
                            ProjectId: self.projectId(),
                            ShipVia: $('#shipVia').val(),
                            FoundryOrderParts: orderParts,
                            CustomerOrders: customerOrderList
                        };

                        $.ajax({
                            type: "POST",
                            url: "/SouthlandMetals/Operations/PurchaseOrder/CreateFoundryOrder",
                            data: JSON.stringify(model),
                            contentType: "application/json",
                            dataType: "json",
                            success: function (result) {
                                alert("Your order has been created.");
                                orderParts = [];
                                shipmentTermsId = null;
                                window.location.href = "/SouthlandMetals/Operations/PurchaseOrder/FoundryOrders"
                            },
                            error: function (req, err) {
                                console.log('Error ' + err);
                            }
                        });
                    }
                }
            }
        });

        $("#addParts").on('click', function () {
            var projectId = self.projectId();
            if (projectId == null) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please select a project!</div>');
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
                var projectId = self.projectId();
                getOrdersByProject(projectId, $('#fromDate').val(), $('#toDate').val());
            }
        });

        $(document).on('click', '#choiceCustomerOrderBtn', function () {
            var selectedCustomerOrders = $('#customerOrderTable').DataTable().rows('.selected').data();
            $.each(selectedCustomerOrders, function (i, customerOrder) {

                var index = -1;
                customerOrderList.some(function (temp, i) {
                    if (temp.CustomerOrderId == customerOrder.CustomerOrderId) {
                        index = i;
                        return true;
                    }
                });

                if (index > -1) {
                    $.confirm({
                        text: "the parts of this customer order already in the table, do you want reset?",
                        dialogClass: "modal-confirm",
                        confirmButton: "Yes",
                        confirmButtonClass: 'btn btn-sm modal-confirm-btn',
                        cancelButton: "No",
                        cancelButtonClass: 'btn btn-sm btn-default',
                        closeIcon: false,
                        confirm: function (button) {
                            getPartsByCustomerOrder(customerOrder.OrderParts);

                            addCustomerOrderRecords(customerOrder);

                            addShipCodeRecords(customerOrder.OrderParts);
                        },
                        cancel: function (button) {

                        }
                    });
                }
                else {
                    getPartsByCustomerOrder(customerOrder.OrderParts);
                    addCustomerOrderRecords(customerOrder);
                    addShipCodeRecords(customerOrder.OrderParts);
                }
            });

            updateCustomerOrderTextArea();
            updateShipCodeTextArea();

            $('#addPartsModal').modal('hide');
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
                }
            });
        }

        function getProjectsByCustomer(customerId) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Administration/Customer/GetProjectsByCustomer",
                data: { 'customerId': customerId },
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.length > 0) {
                        if (self.projects().length > 0) {
                            self.projects.removeAll();
                            self.projects.splice(0, self.projects().length);
                        }

                        self.projects(result);
                    }
                }
            });
        };

        function getOrdersByProject(projectId, fromDate, toDate) {

            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Operations/PurchaseOrder/GetOrdersByProject",
                data: { 'projectId': projectId, 'fromDate': fromDate, 'toDate': toDate },
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
                }
            });
        };

        function getPartsByCustomerOrder(orderParts) {
            if (orderParts.length > 0) {
                $.each(orderParts, function (n, part) {
                    var estArrivalDate, shipDate;
                    var formattedCost = parseFloat(part.UnitCost).toFixed(2);
                    if (part.EstArrivalDate != null) {
                        estArrivalDate = new Date(parseInt(part.EstArrivalDate.substr(6))).toLocaleDateString();
                    }
                    if (part.ShipDate != null) {
                        shipDate = new Date(parseInt(part.ShipDate.substr(6))).toLocaleDateString();
                    }

                    var index = -1;
                    self.orderParts().some(function (orderPart, i) {
                        if (orderPart.customerOrderPartId() == part.CustomerOrderPartId) {
                            index = i;
                            return true;
                        }
                    });

                    if (index > -1) {
                        self.orderParts().splice(index, 1);
                        self.orderParts.push(new OrderPart(part.PartId, part.CustomerOrderPartId, part.AvailableQuantity, part.FoundryOrderQuantity, part.PartNumber, part.PartDescription, part.CustomerOrderId, part.PONumber,
                                                           null, estArrivalDate, shipDate, formattedCost));
                    }
                    else {
                        self.orderParts.push(new OrderPart(part.PartId, part.CustomerOrderPartId, part.AvailableQuantity, part.FoundryOrderQuantity, part.PartNumber, part.PartDescription, part.CustomerOrderId, part.PONumber,
                                                           null, estArrivalDate, shipDate, formattedCost));
                    }
                });

                $('#orderParts').DataTable().columns.adjust();
            }
        };

        function addCustomerOrderRecords(customerOrder) {
            if (customerOrder.OrderParts.length > 0) {

                var index = -1;
                customerOrderList.some(function (temp, i) {
                    if (temp.CustomerOrderId == customerOrder.CustomerOrderId) {
                        index = i;
                        return true;
                    }
                });

                if (index == -1) {
                    customerOrderList.push({ 'CustomerOrderId': customerOrder.CustomerOrderId, 'PONumber': customerOrder.PONumber });
                }
            }
        };

        function addShipCodeRecords(parts) {
            $.each(parts, function (n, part) {
                if (part.ShipCode != null) {

                    var index = -1;
                    shipCodeList.some(function (shipCode, i) {
                        if (shipCode.ShipCode == part.shipCode) {
                            index = i;
                            return true;
                        }
                    });

                    if (index == -1) {
                        shipCodeList.push({ 'ShipCode': part.ShipCode });
                    }
                }
            });
        };

        function removeCustomerOrderRecords(parts) {
            $.each(parts, function (n, part) {

                var index = -1;
                self.orderParts().some(function (orderPart, i) {
                    if (orderPart.customerOrderId() == part.customerOrderId()) {
                        index = -1;
                        return true;
                    }
                });

                if (index == -1) {

                    var customerOrderIndex = -1;
                    customerOrderList.some(function (customerOrder, i) {
                        if (customerOrder.CustomerOrderId == part.customerOrderId()) {
                            customerOrderIndex = -1;
                            return true;
                        }
                    });
                    if (customerOrderIndex != -1) {
                        customerOrderList.splice(customerOrderIndex, 1);
                    }
                }
            });
        };

        function removeShipCodeRecords(parts) {
            $.each(parts, function (n, part) {

                var index = -1;
                self.orderParts().some(function (orderPart, i) {
                    if (orderPart.shipCode() == part.shipCode()) {
                        index = i;
                        return true;
                    }
                });
                if (index == -1) {

                    var shipCodeIndex = -1;
                    shipCodeList.some(function (shipCode, i) {
                        if (shipCode.ShipCode == part.shipCode()) {
                            shipCodeIndex = i;
                            return true;
                        }
                    });
                    if (shipCodeIndex != -1) {
                        shipCodeList.splice(shipCodeIndex, 1);
                    }
                }
            });
        };

        function updateCustomerOrderTextArea() {
            $('#customerOrders').html("");
            $('#customerOrders').empty();
            $.each(customerOrderList, function (n, customerOrder) {
                if (n == 0) {
                    $('#customerOrders').append(customerOrder.PONumber);
                }
                else {
                    $('#customerOrders').append(", " + customerOrder.PONumber);
                }
            });
        };

        function updateShipCodeTextArea() {
            $('#shipCodes').html("");
            $('#shipCodes').empty();
            console.log(shipCodeList);
            $.each(shipCodeList, function (n, shipCode) {
                console.log(0);
                if (n == 0) {
                    $('#shipCodes').append(shipCode.ShipCode);
                }
                else {
                    $('#shipCodes').append(", " + shipCode.ShipCode);
                }
            });
        };

        function getAddressesByCustomer(customerId) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Administration/Customer/GetAddressesByCustomer",
                data: { 'customerId': customerId },
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.length > 0) {
                        if (self.addresses().length > 0) {
                            self.addresses.removeAll();
                            self.addresses.splice(0, self.addresses().length);
                        }

                        self.addresses(result);
                    }
                }
            });
        };

        function getSitesByCustomer(customerId) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Administration/Customer/GetSitesByCustomer",
                data: { 'customerId': customerId },
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.SelectableSites.length > 0) {
                        if (self.sites().length > 0) {
                            self.sites.removeAll();
                            self.sites.splice(0, self.sites().length);
                        }

                        self.sites(result.SelectableSites);
                    }
                }
            });
        };

        function getTermsByCustomer(customerId) {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Administration/Customer/GetTermsByCustomer",
                data: { 'customerId': customerId },
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    shipmentTermsId = result.OrderTermId;
                    $('#shipmentTerms').val(result.OrderTermsDescription);
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

    var fm = new FoundryOrderViewModel();
    ko.applyBindings(fm);
});



