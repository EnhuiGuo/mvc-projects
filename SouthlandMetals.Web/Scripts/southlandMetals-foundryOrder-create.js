$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#foundryLink').addClass("current-link");

    var shipmentTermsId = null;
    var customerOrders = [];
    var customerOrderList = [];
    var duplicatePartString = "";

    var OrderNumber = function (number) {
        var self = this;

        self.poNumber = ko.observable(number);
    };

    var OrderPart = function (cpId, oId, aQuantity, oQuantity, pNumber, pDescription, oNumber, uCost, uPrice, pId, ppId) {
        var self = this;

        self.partId = ko.observable(pId);
        self.projectPartId = ko.observable(ppId);
        self.customerOrderPartId = ko.observable(cpId);
        self.customerOrderId = ko.observable(oId)
        self.availableQuantity = ko.observable(aQuantity);
        self.orderQuantity = ko.observable(oQuantity);
        self.partNumber = ko.observable(pNumber);
        self.partDescription = ko.observable(pDescription);
        self.poNumber = ko.observable(oNumber);
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

        self.Selected = ko.observable(false);
    };

    var FoundryOrderViewModel = function () {
        var self = this;

        self.isSample = ko.observable(false);
        self.isTooling = ko.observable(false);
        self.isProduction = ko.observable(false);

        self.projectId = ko.observable();
        self.projects = ko.observableArray([]);

        self.customerOrders = ko.observableArray([]);

        self.orderTypeId = ko.observable();
        self.orderTypes = ko.observableArray(orderTypes);

        self.orderTypeId.subscribe(function (orderTypeId) {
            if (orderTypeId == "Sample") {
                self.isSample(true);
                self.isTooling(false);
                self.isProduction(false);
            }
            else if (orderTypeId == "Tooling") {
                self.isSample(false);
                self.isTooling(true);
                self.isProduction(false);
            }
            else if (orderTypeId == "Production") {
                self.isSample(false);
                self.isTooling(false);
                self.isProduction(true);
            }
        });

        self.customerId = ko.observable();
        self.addressId = ko.observable();
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

        self.customerOrderPartId = ko.observable();
        self.customerOrderId = ko.observable()
        self.availableQuantity = ko.observable();
        self.orderQuantity = ko.observable();
        self.partNumber = ko.observable();
        self.partDescription = ko.observable();
        self.poNumber = ko.observable();
        self.unitPrice = ko.observable();
        self.unitCost = ko.observable();
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
                    updateCustomerOrderTextArea();
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
                            'CustomerOrderPartId': part.customerOrderPartId(), 'CustomerOrderId': part.customerOrderId(), 'AvailableQuantity': part.orderQuantity(), 'Quantity': part.orderQuantity(), 'PartNumber': part.partNumber(),
                            'PartDescription': part.partDescription(), 'PONumber': part.poNumber(),'Cost': part.unitCost(), 'Price': part.unitPrice(), 'PartId': part.partId(), 'ProjectPartId': part.projectPartId(),
                        });
                    });

                    if (self.customerId() != null) {
                        var model = {
                            OrderNumber: $('#orderNumber').val(),
                            CustomerId: self.customerId(),
                            CustomerAddressId: self.addressId(),
                            FoundryId: $('#foundryId').val(),
                            SiteId: self.siteId(),
                            OrderDate: $('#poDate').val(),
                            OrderNotes: $('#orderNotes').val(),
                            ShipmentTermsId: shipmentTermsId,
                            ProjectId: self.projectId(),
                            ShipVia: $('#shipVia').val(),
                            IsSample: self.isSample(),
                            IsTooling: self.isTooling(),
                            IsProduction: self.isProduction(),
                            IsOpen: true,
                            FoundryOrderParts: orderParts
                        };

                        var target = document.getElementById('spinnerDiv');
                        var spinner = new Spinner(opts).spin(target);

                        $("#page-content-wrapper").css({ opacity: 0.5 });
                        $('#spinnerDiv').show();

                        $.ajax({
                            type: "POST",
                            url: "/SouthlandMetals/Operations/PurchaseOrder/CreateFoundryOrder",
                            data: JSON.stringify(model),
                            contentType: "application/json",
                            dataType: "json",
                            success: function (result) {
                                if (result.Success) {

                                    orderParts = [];
                                    shipmentTermsId = null;

                                    $.confirm({
                                        text: 'Create Success, Do you want print this Foundry Order?',
                                        dialogClass: "modal-confirm",
                                        confirmButton: "Yes",
                                        confirmButtonClass: 'btn btn-sm',
                                        cancelButton: "No",
                                        cancelButtonClass: 'btn btn-sm btn-default',
                                        closeIcon: false,
                                        confirm: function (button) {
                                            window.location.href = '/SouthlandMetals/Operations/Report/FoundryOrderReport?foundryOrderId=' + result.ReferenceId + '';
                                        },
                                        cancel: function (button) {
                                            window.location.href = "/SouthlandMetals/Operations/PurchaseOrder/FoundryOrders";
                                        }
                                    });
                                }
                                else {
                                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                               '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                                }

                                $("#page-content-wrapper").css({ opacity: 1.0 });
                                spinner.stop(target);
                                $('#spinnerDiv').hide();
                            },
                            error: function (err) {
                                console.log('Error ' + err.responseText);
                            }
                        });
                    }
                }
            }
        });

        $("#addParts").on('click', function () {
            var projectId = self.projectId();
            var orderTypeId = self.orderTypeId();
            if (projectId == null) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please select a project!</div>');
            }
            else if (orderTypeId == null) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                   '<strong>Warning!</strong>&nbsp;Please select a Order Type!</div>');
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
                getPartsByCustomerOrder(customerOrder.CustomerOrderParts);
            });

            updateCustomerOrderTextArea();
            $('#addPartsModal').modal('hide');

            if (duplicatePartString !== "") {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;The following parts were not added, because of duplication..' + duplicatePartString + '</div>');
            }

            duplicatePartString = "";
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
                data: { 'projectId': projectId, 'fromDate': fromDate, 'toDate': toDate, 'orderType': self.orderTypeId() },
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
            if (orderParts.length > 0) {
                $.each(orderParts, function (n, part) {

                    var existingPart = self.orderParts().filter(function (n) {
                        return n.partNumber() === part.PartNumber;
                    });

                    if (existingPart.length > 0) {
                        duplicatePartString += part.PartNumber.toString() + " ";
                    }
                    else {
                        var formattedPrice = parseFloat(part.Price).toFixed(2);
                        var formattedCost = parseFloat(part.Cost).toFixed(2);

                        self.orderParts.push(new OrderPart(part.CustomerOrderPartId, part.CustomerOrderId, part.AvailableQuantity, part.FoundryOrderQuantity, part.PartNumber, part.PartDescription, part.PONumber,
                                                           formattedCost, formattedPrice, part.PartId, part.ProjectPartId));
                    }
                });

                $('#orderParts').DataTable().columns.adjust();
            }
        };

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
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
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
                    if (result.length > 0) {
                        if (self.sites().length > 0) {
                            self.sites.removeAll();
                            self.sites.splice(0, self.sites().length);
                        }

                        self.sites(result);
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
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
                }
            }
        }
    };

    var fm = new FoundryOrderViewModel();
    ko.applyBindings(fm);
});



