$(document).ready(function () {

    $('#collapseOrders').addClass("in");
    $('#collapseOrders').attr("aria-expanded", "true");

    $('#ordersLink').addClass("category-current-link");
    $('#customerLink').addClass("current-link");


    var shipmentTermsId = null;
    var priceSheets = [];
    var priceSheetList = [];
    var duplicatePartString = "";

    var OrderPart = function (pId, ppId, pspId, psId, aQuantity, oQuantity, pNumber, pDescription, psNumber, uCost, uPrice) {
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
        self.unitCost = ko.observable(uCost);
        self.unitPrice = ko.observable(uPrice);
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
        self.Selected = ko.observable(false);

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
    };

    var CustomerOrderViewModel = function () {
        var self = this;

        self.isSample = ko.observable(false);
        self.isTooling = ko.observable(false);
        self.isProduction = ko.observable(false);
        self.projectId = ko.observable();
        self.projects = ko.observableArray([]);
        self.number = ko.observable();
        self.priceSheetId = ko.observable();
        self.priceSheetNumber = ko.observable();
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
        self.priceSheetPartId = ko.observable();
        self.priceSheetId = ko.observable();
        self.availableQuantity = ko.observable();
        self.orderQuantity = ko.observable();
        self.partNumber = ko.observable();
        self.partDescription = ko.observable();
        self.priceSheetNumber = ko.observable();
        self.estArrivalDate = ko.observable(null);
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
            "scrollY": 475,
            "scrollCollapse": true,
        });

        self.multiSelectableOptions = {
            helper: function (event, $item) {
                var dbId = $item.parent().attr('id'),
                  itemId = $item.attr('id'),
                  db = sm['partsList' + dbId];

                if (!$item.hasClass('selected')) {
                    ko.utils.arrayForEach(db(), function (item) {
                        //needs to be like this for string coercion
                        item.Selected(item.priceSheetPartId() == itemId);
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
            } else if (event.shiftKey && !event.ctrlKey && self._lastSelectedIndex > -1) {
                var myIndex = array.indexOf($data);
                if (myIndex > self._lastSelectedIndex) {
                    for (var i = self._lastSelectedIndex; i <= myIndex; i++) {
                        array[i].Selected(true);
                    }
                } else if (myIndex < self._lastSelectedIndex) {
                    for (var i = myIndex; i <= self._lastSelectedIndex; i++) {
                        array[i].Selected(true);
                    }
                }

            } else if (event.ctrlKey && !event.shiftKey) {
                $data.Selected(!$data.Selected());
            }
            self._lastSelectedIndex = array.indexOf($data);
        };

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
                    updatePriceSheetTextArea();

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

        $(document).on('click', '#choicePriceSheetBtn', function () {

            var selectedPriceSheets = $('#priceSheetTable').DataTable().rows('.selected').data();

            $.each(selectedPriceSheets, function (i, priceSheet) {
                getPartsByPriceSheet(priceSheet.PriceSheetParts);
            });

            updatePriceSheetTextArea();
            $('#addPartsModal').modal('hide');

            if (duplicatePartString !== "") {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                               '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                               '<strong>Warning!</strong>&nbsp;The following parts were not added, because of duplication..' + duplicatePartString + '</div>');
            }

            duplicatePartString = "";

        });

        $(document).on('click', '#saveCustomerOrderBtn', function () {
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
                            'PriceSheetPartId': part.priceSheetPartId(), 'PriceSheetId': part.priceSheetId(), 'AvailableQuantity': part.orderQuantity(), 'CustomerOrderQuantity': part.orderQuantity(), 'PartNumber': part.partNumber(),
                            'PartDescription': part.partDescription(), 'PriceSheetNumber': part.priceSheetNumber(), 'EstArrivalDate': self.estArrivalDate(),
                            'Cost': part.unitCost(), 'Price': part.unitPrice(), "PartId": part.partId(), "projectPartId": part.projectPartId(),
                        });
                    });

                    var model = {
                        PONumber: $('#poNumber').val(),
                        PODate: $('#poDate').val(),
                        DueDate: null, 
                        PortDate: null,
                        ShipDate: null,
                        EstArrivalDate: self.estArrivalDate(),
                        ProjectId: self.projectId(),
                        CustomerId: self.customerId(),
                        CustomerAddressId: self.addressId(),
                        FoundryId: $('#foundryId').val(),
                        SiteId: self.siteId(),
                        OrderNotes: $('#orderNotes').val(),
                        ShipmentTermsId: shipmentTermsId,
                        IsSample: self.isSample(),
                        IsTooling: self.isTooling(),
                        IsProduction: self.isProduction(),
                        IsOpen: true,
                        IsHold: false,
                        HoldExpirationDate: null,
                        HoldNotes: null,
                        IsCanceled: false,
                        CancelNotes: null,
                        CanceledDate: null,
                        IsComplete: false,
                        CompletedDate: null,
                        customerOrderParts: orderParts
                    };

                    $.ajax({
                        type: "POST",
                        url: "/SouthlandMetals/Operations/PurchaseOrder/CreateCustomerOrder",  
                        data: JSON.stringify(model),
                        contentType: "application/json",
                        dataType: "json",
                        success: function (result) {
                            if (result.Success) {
                                orderParts = [];
                                shipmentTermsId = null;

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

        $(document).on('click', '#cancelCustomerOrderBtn', function () {
            window.history.back();
        });

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

        function getPartsByPriceSheet(priceSheetParts) {
            if (priceSheetParts.length > 0) {
                $.each(priceSheetParts, function (n, part) {
                    var existingPart = self.orderParts().filter(function (n) {
                        return n.partNumber() === part.PartNumber;
                    });

                    if (existingPart.length > 0) {
                        duplicatePartString += part.PartNumber.toString() + " ";
                    }
                    else {
                        var formattedPrice = parseFloat(part.UnitPrice).toFixed(2);
                        var formattedCost = parseFloat(part.UnitCost).toFixed(2);

                        self.orderParts.push(new OrderPart(part.PartId, part.ProjectPartId, part.PriceSheetPartId, part.PriceSheetId, part.AvailableQuantity, part.CustomerOrderQuantity, part.PartNumber, part.PartDescription,
                                                           part.PriceSheetNumber, formattedCost, formattedPrice));
                    }
                });

                $('#orderParts').DataTable().columns.adjust();
            }
        };

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

        function updatePriceSheetTextArea() {

            $('#priceSheets').html("");
            $('#priceSheets').empty();
            
            var priceSheetList = self.orderParts().map(function (part) {
                return part.priceSheetNumber();
            });

            priceSheetList = priceSheetList.filter(function (priceSheetNumber, i, priceSheetList) {
                return i == priceSheetList.indexOf(priceSheetNumber);
            });

            $.each(priceSheetList, function (n, priceSheet) {
                if (n == 0) {
                    $('#priceSheets').append(priceSheet);
                }
                else {
                    $('#priceSheets').append(", " + priceSheet);
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
                                for (var i = 0; i < cm.orderParts().length; i++) {
                                    cm.orderParts()[i].shipDate($(element).val());
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

    $("#addParts").on('click', function () {
        var projectId = cm.projectId();
        var orderTypeId = cm.orderTypeId();
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

    function getPriceSheetsByProject(projectId, fromDate, toDate) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Pricing/GetPriceSheetsByProject",
            data: { 'projectId': projectId, 'fromDate': fromDate, 'toDate': toDate, 'orderType': cm.orderTypeId() },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                priceSheets = result;
                $.each(priceSheets, function (i, priceSheet) {
                    if (priceSheet.DueDate != null) {
                        priceSheet.DueDate = new Date(parseInt(priceSheet.DueDate.substr(6))).toLocaleDateString();
                    }
                });
                $('#priceSheetTable').DataTable().clear().draw();
                $('#priceSheetTable').DataTable().rows.add(priceSheets); // Add new data
                $('#priceSheetTable').DataTable().columns.adjust().draw();
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    }

    function _AddParts() {
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
    }

    $(document).on('click', '#getPriceSheetBtn', function () {

        if ($('#fromDate').val() != "" && $('#toDate').val() && $('#fromDate').val() > $('#toDate').val()) {
            alert("'from date' can not after 'to date'");
        }
        else {
            var projectId = cm.projectId();
            getPriceSheetsByProject(projectId, $('#fromDate').val(), $('#toDate').val());
        }
    });

    var cm = new CustomerOrderViewModel();
    ko.applyBindings(cm);
});




