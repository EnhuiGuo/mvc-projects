$(document).ready(function () {
    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#shipmentTrackingLink').addClass("current-link");

    var containers = [];
    var containerId = null;
    var containerNumber = null;
    var purchaseOrders = [];
    var containerParts = [];
    var pallets = [];

    var Part = function (pId, pNumber, oId, oNumber, cId, cNumber, availableQty, quantity, cost, shipCode, fopId, weight) {
        var self = this;

        self.partId = ko.observable(pId);
        self.partNumber = ko.observable(pNumber);
        self.orderId = ko.observable(oId);
        self.orderNumber = ko.observable(oNumber);
        self.containerId = ko.observable(cId);
        self.containerNumber = ko.observable(cNumber);
        self.availableQuantity = ko.observable(availableQty);
        self.quantity = ko.observable(quantity);
        self.cost = ko.observable(cost);
        self.shipCode = ko.observable(shipCode);
        self.palletNumber = ko.observable();
        self.foundryOrderPartId = ko.observable(fopId);
        self.weight = ko.observable(weight);
    };

    var Container = function (cId, cNumber) {
        var self = this;

        self.containerId = ko.observable(cId);
        self.containerNumber = ko.observable(cNumber);
    };

    var BolViewModel = function () {
        var self = this;

        self.partId = ko.observable();
        self.partNumber = ko.observable();
        self.availableQuantity = ko.observable();
        self.quantity = ko.observable();

        self.orderId = ko.observable();
        self.orderNumber = ko.observable();
        self.orders = ko.observableArray(orders);

        self.parts = ko.observableArray([]);

        self.containerId = ko.observable();
        self.containerNumber = ko.observable();
        self.containers = ko.observableArray([]);

        self.containerId.subscribe(function (newContainerId) {
            var newContainer = ko.utils.arrayFirst(ko.toJS(self.containers), function (container) {
                return container.Value === newContainerId;
            });
            if (newContainerId != null) {
                self.containerNumber(newContainer.Text);
            }
            else {
                self.containerNumber("");
            }
        }.bind(self));

        self.orderId.subscribe(function (id) {
            $.ajax({
                type: "GET",
                cache: false,
                contentType: "application/json",
                url: "/SouthlandMetals/Operations/PurchaseOrder/GetPartsByFoundryOrder",
                data: { 'foundryOrderId': id },
                dataType: "json",
                success: function (result) {
                    if (self.parts().length > 0) {
                        self.parts.removeAll();
                        self.parts.splice(0, self.parts().length);
                    }

                    if (result.length > 0) {
                        $.each(result, function (n, part) {
                            if (containerParts.length > 0) {
                                for (var i = 0; i < containerParts.length; i++) {
                                    var availableQty = 0;
                                    if (part.FoundryOrderPartId === containerParts[i].FoundryOrderPartId) {
                                        availableQty = parseInt(part.AvailableQuantity) - parseInt(containerParts[i].Quantity);
                                        break;
                                    }
                                    else {
                                        availableQty = part.AvailableQuantity;
                                    }
                                }
                                self.parts.push(new Part(part.PartId, part.PartNumber, part.FoundryOrderId, part.OrderNumber, null, null, availableQty, availableQty, part.Cost, part.ShipCode, part.FoundryOrderPartId, part.Weight));
                            }
                            else {
                                self.parts.push(new Part(part.PartId, part.PartNumber, part.FoundryOrderId, part.OrderNumber, null, null, part.AvailableQuantity, part.AvailableQuantity, part.Cost, part.ShipCode, part.FoundryOrderPartId, part.Weight));
                            }
                        });
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        });

        var buckets = [];

        var bucketsTable = $('#buckets').DataTable({
            "autoWidth": false,
            "lengthChange": false,
            "searching": false,
            "paging": false,
            "info": false,
            "scrollY": 250,
            "scrollX": true,
            "scrollCollapse": true,
            "data": buckets,
            "order": [2, 'asc'],
            "columns": [
            { "data": "BucketId", "title": "BucketId", "visible": false },
            { "data": "FoundryInvoiceId", "title": "FoundryInvoiceId", "visible": false },
            { "data": "BucketName", "title": "Name", "class": "center" },
            { "data": "BucketValue", "title": "Value", "class": "center" },
            { "title": "Remove", "class": "center" },
            ],
            "columnDefs": [{
                "targets": 4,
                "title": "Contents",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                      "<span class='glyphicon glyphicon-trash glyphicon-large' id='removeBucketBtn'></span>"
            },
            ]
        });

        $('#bucketValue').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).trigger("enterKey");
            }
        });

        $(document).bind('enterKey', '#bucketValue', function () {
            var bucketName = $('#bucketName').val();
            var bucketValue = $('#bucketValue').val();

            if (bucketName !== '' || bucketValue !== '') {

                buckets.push({
                    'BucketId': null, 'FoundryInvoiceId': null, 'BucketName': bucketName, 'BucketValue': bucketValue
                });

                $('#buckets').DataTable().clear().draw();
                $('#buckets').DataTable().rows.add(buckets); // Add new data
                $('#buckets').DataTable().columns.adjust().draw();

                $('#bucketName').val('');
                $('#bucketValue').val('');
                $('#bucketName').focus();
            }
            else {
                alert("Please enter a bucket name and value!");
            }
        });

        $(document).on('click', '#removeBucketBtn', function () {
            var bucketDelete = bucketsTable.row($(this).parents('tr')).data();
            var childData = bucketsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (bucketDelete != null) {
                data = bucketDelete;
            }
            else {
                data = childData;
            }

            bucketsTable.row($(this).parents('tr')).remove().draw();

            for (var i = 0; i < buckets.length; i++) {
                if (buckets[i].BucketName === data.BucketName) {
                    buckets.splice(i, 1);
                }
            }
        });

        $(document).on('click', '#saveContainerBtn', function () {
            event.preventDefault();

            if (!$("#addContainerForm")[0].checkValidity()) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('.errorAlert').show();
                $('#addContainerForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                // then to call it, plus stitch in '4' in the third group
                var containerId = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
                var containerNumber = $('#containerNumber').val();

                self.containers.push({ Value: containerId, Text: containerNumber });

                containers.push({
                    'ContainerId': containerId, 'ContainerNumber': containerNumber,
                });

                $('#containers').DataTable().clear().draw();
                $('#containers').DataTable().rows.add(containers); // Add new data
                $('#containers').DataTable().columns.adjust().draw();

                $('#addContainerModal').modal('hide');
            }
        });

        self.saveParts = function () {
            event.preventDefault();

            $('#addPartsModal select').removeClass("form-error");

            if (self.containerId() == null || $('#orders').val() == "") {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp; Required!</div>');
                $('.errorAlert').show();

                if (self.containerId() == null) {
                    $("#containersBox").addClass("form-error");
                }

                if ($('#orders').val() == "") {
                    $("#orders").addClass("form-error");
                }
            }
            else if (self.parts().length < 1) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;There are no parts to be added!</div>');
                $('.errorAlert').show();
            }
            else {
                if (containerParts.length > 0) {
                    var found = false;
                    for (var i = 0; i < self.parts().length; i++) {
                        if (self.parts()[i].quantity() != 0 && self.parts()[i].quantity() <= self.parts()[i].availableQuantity()) {
                            for (var j = 0; j < containerParts.length; j++) {
                                var quantity = 0;
                                if (self.parts()[i].foundryOrderPartId() === containerParts[j].FoundryOrderPartId && self.containerId() == containerParts[j].ContainerId) {
                                    quantity = parseInt(self.parts()[i].quantity()) + parseInt(containerParts[j].Quantity);
                                    containerParts[j].Quantity = quantity;
                                    found = true;
                                }
                                else {
                                    quantity = parseInt(self.parts()[i].quantity());
                                }
                            }

                            if (!found) {
                                containerParts.push({
                                    'PartId': self.parts()[i].partId(), 'PartNumber': self.parts()[i].partNumber(), 'FoundryOrderId': self.parts()[i].orderId(), 'OrderNumber': self.parts()[i].orderNumber(),
                                    'ContainerId': self.containerId(), 'ContainerNumber': self.containerNumber(), 'AvailableQuantity': self.parts()[i].availableQuantity(), 'Quantity': quantity, "Cost": self.parts()[i].cost(),
                                    "ShipCode": self.parts()[i].shipCode(), "ShipCodeNotes": "N/A", "PalletNumber": self.parts()[i].palletNumber(), "FoundryOrderPartId": self.parts()[i].foundryOrderPartId(),
                                    "Weight": self.parts()[i].weight(), "PalletWeight": 50,
                                });
                            }
                        }
                    }
                }
                else {
                    for (var j = 0; j < self.parts().length; j++) {
                        if (self.parts()[j].quantity() != 0 && self.parts()[j].quantity() <= self.parts()[j].availableQuantity()) {
                            containerParts.push({
                                'PartId': self.parts()[j].partId(), 'PartNumber': self.parts()[j].partNumber(), 'FoundryOrderId': self.parts()[j].orderId(), 'OrderNumber': self.parts()[j].orderNumber(),
                                'ContainerId': self.containerId(), 'ContainerNumber': self.containerNumber(), 'AvailableQuantity': self.parts()[j].availableQuantity(), 'Quantity': self.parts()[j].quantity(), "Cost": self.parts()[j].cost(),
                                "ShipCode": self.parts()[j].shipCode(), "ShipCodeNotes": "N/A", "PalletNumber": self.parts()[j].palletNumber(), "FoundryOrderPartId": self.parts()[j].foundryOrderPartId(),
                                "Weight": self.parts()[j].weight(), "PalletWeight": 50,
                            });
                        }
                    }
                }

                $('#containerParts').DataTable().clear().draw();
                $('#containerParts').DataTable().rows.add(containerParts); // Add new data
                $('#containerParts').DataTable().columns.adjust().draw();

                $.each(purchaseOrders, function (i, purchaseOrder) {
                    if (purchaseOrder.OrderNumber == self.parts()[0].orderNumber()) {
                        purchaseOrders.splice(i, 1);
                    }
                });

                purchaseOrders.push({
                    'FoundryOrderId': self.orderId(), 'ContainerId': self.containerId(), 'OrderNumber': self.parts()[0].orderNumber(), 'EstArrivalDate': "N/A",
                });

                $('#purchaseOrders').DataTable().clear().draw();
                $('#purchaseOrders').DataTable().rows.add(purchaseOrders); // Add new data
                $('#purchaseOrders').DataTable().columns.adjust().draw();

                self.orderId(null);
                self.orderNumber("");

                self.containerId(null);
                self.containerNumber("");

                if (self.parts().length > 0) {
                    self.parts.removeAll();
                    self.parts.splice(0, self.parts().length);
                }

                $.each(containerParts, function (i, part) {
                    var exist = false;
                    $.each(pallets, function (i, pallet) {
                        if (pallet.PalletNumber == part.PalletNumber)
                            exist = true;
                    })

                    if (!exist) {
                        pallets.push({
                            'PalletNumber': part.PalletNumber
                        });
                    }
                });

                $('#pallets').DataTable().clear().draw();
                $('#pallets').DataTable().rows.add(pallets); // Add new data
                $('#pallets').DataTable().columns.adjust().draw();

                $('#addPartsModal').modal('hide');
            }
        };

        var containersTable = $('#containers').DataTable({
            "autoWidth": false,
            "lengthChange": false,
            "searching": false,
            "paging": false,
            "info": false,
            "scrollY": 700,
            "scrollX": true,
            "scrollCollapse": true,
            "order": [1, 'asc'],
            "data": containers,
            "columns": [
            { "data": "ContainerId", "title": "ContainerId", "visible": false },
            { "data": "ContainerNumber", "title": "Number", "class": "center" },
            { "title": "Contents", "class": "center" },
            { "title": "Remove", "class": "center" },
            ],
            "columnDefs": [{
                "targets": 2,
                "title": "Contents",
                "width": "8%", "targets": 2,
                "data": null,
                "defaultContent":
                      "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='contentsBtn'></span>"
            },
             {
                 "targets": 3,
                 "title": "Remove",
                 "width": "8%", "targets": 3,
                 "data": null,
                 "defaultContent":
                     "<span class='glyphicon glyphicon-trash glyphicon-large' id='removeBtn'></span>"
             },
            ]
        });

        $('#containers tbody').on('click', '#contentsBtn', function () {
            var container = containersTable.row($(this).parents('tr')).data();
            var childData = containersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (container != null) {
                data = container;
            }
            else {
                data = childData;
            }

            var parts = containerParts.filter(function (n) {
                return n.ContainerId === data.ContainerId;
            });

            $('#containerParts').DataTable().clear().draw();
            $('#containerParts').DataTable().rows.add(parts); // Add new data
            $('#containerParts').DataTable().columns.adjust().draw();

            $('#billOfLadingTabs li:nth-child(1)').removeClass("active");
            $('#containersTab').removeClass("active");
            $('#billOfLadingTabs li:nth-child(2)').addClass("active")
            $('#partsTab').addClass("active");

            $('#containerParts').DataTable().columns.adjust().draw();
        });

        $('#containers tbody').on('click', '#removeBtn', function () {
            var container = containersTable.row($(this).parents('tr')).data();
            var childData = containersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (container != null) {
                data = container;
            }
            else {
                data = childData;
            }

            var orders = purchaseOrders.filter(function (n) {
                return n.ContainerId === data.ContainerId;
            });

            $('#purchaseOrders').DataTable().clear().draw();
            $('#purchaseOrders').DataTable().rows(orders).remove(); // remove data
            $('#purchaseOrders').DataTable().columns.adjust().draw();

            for (var i = 0; i < purchaseOrders.length; i++) {
                if (purchaseOrders[i].ContainerId === contdataainer.ContainerId) {
                    purchaseOrders.splice(i, 1);
                }
            }

            var parts = containerParts.filter(function (n) {
                return n.ContainerId === data.ContainerId;
            });

            $('#containerParts').DataTable().clear().draw();
            $('#containerParts').DataTable().rows(parts).remove(); // remove data
            $('#containerParts').DataTable().columns.adjust().draw();

            for (var i = 0; i < containerParts.length; i++) {
                if (containerParts[i].ContainerId === data.ContainerId) {
                    containerParts.splice(i, 1);
                }
            }

            containersTable.row($(this).parents('tr')).remove().draw();

            containers.splice(containers.indexOf(data.ContainerId), 1);

        });

        var containerPartsTable = $('#containerParts').DataTable({
            "autoWidth": false,
            "pageLength": 25,
            "lengthChange": false,
            "paging": false,
            "searching": false,
            "scrollY": 150,
            "scrollCollapse": true,
            "order": [1, 'asc'],
            "data": containerParts,
            "columns": [
            { "data": "FoundryOrderPartId", "title": "FoundryOrderPartId", "visible": false },
            { "data": "PartNumber", "title": "Number", "class": "center" },
            { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
            { "data": "OrderNumber", "title": "Purchase Order", "class": "center" },
            { "data": "ContainerId", "title": "ContainerId", "visible": false },
            { "data": "ShipCode", "title": "Ship Code", "class": "center" },
            { "data": "AvailableQuantity", "title": "AvailableQuantity", "visible": false },
            { "data": "Quantity", "title": "Quantity", "class": "center" },
            { "data": "Cost", "title": "Cost", "class": "center", "visible": false },
            {
                "data": null, "Content": null, "title": "Total Cost", "class": "center", "visible": false,
                render: function (data, type, row) {
                    var data = row.Cost * row.Quantity;
                    return parseFloat(data).toFixed(2);
                }
            },
            { "title": "Detail", "class": "center" },
            { "title": "Edit", "class": "center" },
            { "title": "Remove", "class": "center" }
            ],
            "columnDefs": [{
                "targets": 10,
                "title": "Detail",
                "width": "8%", "targets": 10,
                "data": null,
                "defaultContent":
                    "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='detailBtn'></span>"
            },
            {
                "targets": 11,
                "title": "Edit",
                "width": "8%", "targets": 11,
                "data": null,
                "defaultContent":
                    "<span class='glyphicon glyphicon-pencil glyphicon-large' id='editBtn'></span>"
            },
            {
                "targets": 12,
                "title": "Remove",
                "width": "8%", "targets": 12,
                "data": null,
                "defaultContent":
                    "<span class='glyphicon glyphicon-trash glyphicon-large' id='removeBtn'></span>"
            },
            ]
        });

        $('#containerParts tbody').on('click', '#detailBtn', function () {
            var containerPart = containerPartsTable.row($(this).parents('tr')).data();
            var childData = containerPartsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (containerPart != null) {
                data = containerPart;
            }
            else {
                data = childData;
            }

            var model = $.grep(containerParts, function (part, i) {
                return part.ContainerPartId == data.ContainerPartId;
            });

            _ViewContainerPartDetail(model[0]);
        });

        $('#containerParts tbody').on('click', '#editBtn', function () {
            var containerPart = containerPartsTable.row($(this).parents('tr')).data();
            var childData = containerPartsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (containerPart != null) {
                data = containerPart;
            }
            else {
                data = childData;
            }

            $.ajax({
                type: "GET",
                cache: false,
                url: "/SouthlandMetals/Operations/Shipment/_EditContainerPart",
                success: function (result) {
                    $('#editPartId').val(data.FoundryOrderPartId);
                    $('#partNumber').val(data.PartNumber);
                    self.containerId(data.ContainerId);
                    $('#editQuantity').val(data.Quantity);
                    $('#editShipCodeNotes').val(data.ShipCodeNotes);
                    $('#editContainerPartModal').modal('show');
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        });

        $('#containerParts tbody').on('click', '#removeBtn', function () {
            var containerPart = containerPartsTable.row($(this).parents('tr')).data();
            var childData = containerPartsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (containerPart != null) {
                data = containerPart;
            }
            else {
                data = childData;
            }

            containerPartsTable.row($(this).parents('tr')).remove().draw();

            for (var i = 0; i < containerParts.length; i++) {
                if (containerParts[i].FoundryOrderPartId == data.FoundryOrderPartId) {
                    containerParts.splice(i, 1);
                }
            }
        });

        $(document).on('click', '#updateContainerPartBtn', function () {
            var quantity = $('#editQuantity').val();
            var shipCodeNotes = $('#editShipCodeNotes').val();
            if (quantity < 1) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Part quantity must be greater than zero!</div>');
                $('.errorAlert').show();
            }
            else {
                var foundryOrderPartId = $('#editPartId').val();
                var containerId = self.containerId();
                var containerNumber = self.containerNumber();

                $.each(containerParts, function (n, part) {
                    if (part.FoundryOrderPartId === foundryOrderPartId && part.ContainerNumber == containerNumber) {
                        part.PartNumber = part.PartNumber;
                        part.FoundryOrderId = part.FoundryOrderId;
                        part.OrderNumber = part.OrderNumber;
                        part.ContainerId = containerId;
                        part.ContainerNumber = containerNumber;
                        part.AvailableQuantity = part.AvailableQuantity;
                        part.Quantity = quantity;
                        part.ShipCodeNotes = shipCodeNotes;
                    }
                });

                $('#containerParts').DataTable().clear().draw();
                $('#containerParts').DataTable().rows.add(containerParts); // Add new data
                $('#containerParts').DataTable().columns.adjust().draw();

                self.containerId(null);
                self.containerNumber("");

                $('#editContainerPartModal').modal('hide');
            }
        });

        var purchaseOrdersTable = $('#purchaseOrders').DataTable({
            "autoWidth": false,
            "lengthChange": false,
            "searching": false,
            "paging": false,
            "info": false,
            "scrollY": 700,
            "scrollX": true,
            "scrollCollapse": true,
            "order": [4, 'desc'],
            "data": purchaseOrders,
            "columns": [
            { "data": "FoundryOrderId", "title": "FoundryOrderId", "visible": false },
            { "data": "ContainerId", "title": "ContainerId", "visible": false },
            { "data": "OrderNumber", "title": "Number", "class": "center" },
            { "data": "EstArrivalDate", "name": "EstArrivalDate", "title": "ETA", "class": "center", render: dateFlag },
            { "title": "Contents", "class": "center" },
            { "title": "Edit", "class": "center" },
            { "title": "Remove", "class": "center" }
            ],
            "columnDefs": [
            {
                "targets": 4,
                "title": "Contents",
                "width": "8%", "targets": 4,
                "data": null,
                "defaultContent":
                "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='contentsBtn'></span>"
            },
            {
                "targets": 5,
                "title": "Edit",
                "width": "8%", "targets": 5,
                "data": null,
                "defaultContent":
                    "<span class='glyphicon glyphicon-pencil glyphicon-large' id='editBtn'></span>"
            },
            {
                "targets": 6,
                "title": "Remove",
                "width": "8%", "targets": 6,
                "data": null,
                "defaultContent":
                    "<span class='glyphicon glyphicon-trash' id='removeBtn'></span>"
            }
            ]
        });

        $('#purchaseOrders tbody').on('click', '#contentsBtn', function () {
            var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
            var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (purchaseOrder != null) {
                data = purchaseOrder;
            }
            else {
                data = childData;
            }

            var parts = containerParts.filter(function (n) {
                return n.FoundryOrderId == data.FoundryOrderId
            });

            $('#containerParts').DataTable().clear().draw();
            $('#containerParts').DataTable().rows.add(parts); // Add new data
            $('#containerParts').DataTable().columns.adjust().draw();

            $('#billOfLadingTabs li:nth-child(3)').removeClass("active");
            $('#purchaseOrdersTab').removeClass("active");
            $('#billOfLadingTabs li:nth-child(2)').addClass("active")
            $('#partsTab').addClass("active");

            $('#containerParts').DataTable().columns.adjust().draw();
        });

        $('#purchaseOrders tbody').on('click', '#notesBtn', function () {
            var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
            var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (purchaseOrder != null) {
                data = purchaseOrder;
            }
            else {
                data = childData;
            }
            _ViewShipCodeNotes(data.ShipCodeNotes);
        });

        $('#purchaseOrders tbody').on('click', '#editBtn', function () {
            var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
            var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (purchaseOrder != null) {
                data = purchaseOrder;
            }
            else {
                data = childData;
            }

            var model = {
                FoundryOrderId: data.FoundryOrderId,
                OrderNumber: data.OrderNumber,
                ShipCode: data.ShipCode,
                EstArrivalDate: data.EstArrivalDate,
                EstArrivalDateStr: data.EstArrivalDateStr,
                ShipCodeNotes: data.ShipCodeNotes
            };

            _EditPurchaseOrder(model);
        });

        $('#purchaseOrders tbody').on('click', '#removeBtn', function () {
            var purchaseOrder = purchaseOrdersTable.row($(this).parents('tr')).data();
            var childData = purchaseOrdersTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (purchaseOrder != null) {
                data = purchaseOrder;
            }
            else {
                data = childData;
            }

            var containersToRemove = containers.filter(function (n) {
                return n.ContainerId === data.ContainerId;
            });

            $('#containers').DataTable().clear().draw();
            $('#containers').DataTable().rows(containersToRemove).remove(); // remove data
            $('#containers').DataTable().columns.adjust().draw();

            for (var i = 0; i < containers.length; i++) {
                if (containers[i].ContainerId === data.ContainerId) {
                    containers.splice(i, 1);
                }
            }

            var parts = containerParts.filter(function (n) {
                return n.FoundryOrderId === data.FoundryOrderId;
            });

            $('#containerParts').DataTable().clear().draw();
            $('#containerParts').DataTable().rows(parts).remove(); // remove data
            $('#containerParts').DataTable().columns.adjust().draw();

            for (var i = 0; i < containerParts.length; i++) {
                if (containerParts[i].FoundryOrderId === data.FoundryOrderId) {
                    containerParts.splice(i, 1);
                }
            }

            purchaseOrdersTable.row($(this).parents('tr')).remove().draw();

            purchaseOrders.splice(purchaseOrders.indexOf(data.FoundryOrderId), 1);
        });

        $(document).on('click', '#updatePurchaseOrderBtn', function () {
            event.preventDefault();

            if (!$("#editPurchaseOrderForm")[0].checkValidity()) {
                $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('.errorAlert').show();
                $('#editPurchaseOrderForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });
            }
            else {
                var orderId = $('#editOrderId').val();
                var shipCode = $('#editShipCode').val();
                var eta = $('#editEstArrivalDate').val();
                var notes = $('#editShipCodeNotes').val();
                var oldShipCode = $('#oldShipCode').val();

                $.each(purchaseOrders, function (n, order) {
                    if (order.FoundryOrderId == orderId && order.ShipCode == oldShipCode) {
                        order.OrderNumber = order.OrderNumber;
                        order.ShipCode = shipCode;
                        order.EstArrivalDate = eta;
                        order.ShipCodeNotes = notes;
                    }
                });

                $('#purchaseOrders').DataTable().clear().draw();
                $('#purchaseOrders').DataTable().rows.add(purchaseOrders); // Add new data
                $('#purchaseOrders').DataTable().columns.adjust().draw();

                $('#editPurchaseOrderModal').modal('hide');
            }
        });

        var palletsTable = $('#pallets').DataTable({
            "autoWidth": false,
            "lengthChange": false,
            "searching": false,
            "paging": false,
            "scrollY": 150,
            "scrollCollapse": true,
            "info": false,
            "data": pallets,
            "order": [1, 'asc'],
            "columns": [
            { "data": "PalletNumber", "title": "Number", "class": "center" },
            { "title": "Contents", "class": "center" }
            ],
            "columnDefs": [{
                "targets": 1,
                "title": "Contents",
                "width": "8%", "targets": 1,
                "data": null,
                "defaultContent":
                      "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='palletsBtn'></span>"
            }
            ]
        });

        $('#pallets tbody').on('click', '#palletsBtn', function () {
            var pallet = palletsTable.row($(this).parents('tr')).data();
            var childData = palletsTable.row($(this).parents('tr').prev('tr')).data();
            var data;

            if (pallet != null) {
                data = pallet;
            }
            else {
                data = childData;
            }

            var parts = containerParts.filter(function (n) {
                return n.PalletNumber === data.PalletNumber;
            });

            $('#containerParts').DataTable().clear().draw();
            $('#containerParts').DataTable().rows.add(parts); // Add new data
            $('#containerParts').DataTable().columns.adjust().draw();

            $('#billOfLadingTabs li:nth-child(5)').removeClass("active");
            $('#palletTab').removeClass("active");
            $('#billOfLadingTabs li:nth-child(2)').addClass("active")
            $('#partsTab').addClass("active");

            $('#containerParts').DataTable().columns.adjust().draw();
        });

        $(document).on('click', '#saveBolBtn', function () {
            event.preventDefault();

            if (!$("#addBolForm")[0].checkValidity()) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
                $('#addBolForm input[required]').each(function () {
                    if ($(this).val() === "") {
                        $(this).addClass("form-error");
                    }
                });

                $('#addBolForm select[required]').each(function () {
                    if (!$(this).is(':selected')) {
                        $(this).addClass("form-error");
                    }
                });
            }
            else if (containers.length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Containers to this Bill of Lading!</div>');
            }
            else if (containerParts.length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Container Parts to this Bill of Lading!</div>');
            }
            else if (purchaseOrders.length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Purchase Orders to this Bill of Lading!</div>');
            }
            else if (buckets.length < 1) {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                  '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                  '<strong>Warning!</strong>&nbsp;Please enter Buckets for Foundry Invoice!</div>');
            }
            else {

                var foundryInvoice = {
                    FoundryId: $('#foundryId').val(),
                    InvoiceNumber: $('#invoiceNumber').val(),
                    InvoiceAmount: $('#invoiceAmount').val(),
                    ScheduledPaymentDate: $('#scheduledDate').val(),
                    ActualPaymentDate: $('#actualDate').val(),
                    Notes: $('#invoiceNotes').val(),
                    AirFreight: $('#airFreight').val(),
                    Buckets: buckets
                }

                var model = {
                    ShipmentId: $('#shipmentId').val(),
                    BolNumber: $('#bolNumber').val(),
                    BolDate: $('#bolDate').val(),
                    Description: $('#description').val(),
                    FoundryId: $('#foundryId').val(),
                    CustomsNumber: $('#customsNumber').val(),
                    IsCustomsLiquidated: $('#isCustomsLiquidated').prop("checked"),
                    PalletCount: $('#palletCount').val(),
                    GrossWeight: $('#grossWeight').val(),
                    NetWeight: $('#netWeight').val(),
                    HasLcl: $('#hasLcl').prop("checked"),
                    HasDoorMove: $('#hasDoorMove').prop("checked"),
                    HasArrivalNotice: $('#hasArrivalNotice').prop("checked"),
                    HasOriginalDocuments: $('#hasOriginals').prop("checked"),
                    BolNotes: $('#bolNotes').val(),
                    WireInstructions: $('#wireInstructions').val(),
                    HasBeenAnalyzed: false,
                    Containers: containers,
                    ContainerParts: containerParts,
                    PurchaseOrders: purchaseOrders,
                    FoundryInvoice: foundryInvoice
                };

                $.ajax({
                    type: "POST",
                    url: "/SouthlandMetals/Operations/Shipment/CreateBillofLading",
                    data: JSON.stringify(model),
                    dataType: "json",
                    contentType: "application/json",
                    success: function (result) {
                        if (result.Success) {
                            containers = [];
                            purchaseOrders = [];
                            bolParts = [];
                            window.location.href = "/SouthlandMetals/Operations/Shipment/Tracking"
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
    };

    $(document).on('click', '#cancelBolBtn', function () {
        window.history.back();
    });

    $('#addContainer').on('click', function () {
        _AddContainer();

        $('#containerNumber').focus();
    });

    $('#addParts').on('click', function () {
        _AddParts();
    });

    self.cancelAddParts = function () {
        if (self.parts().length > 0) {
            self.parts.removeAll();
            self.parts.splice(0, self.parts().length);
        }
    };

    $('#addDebitMemoBtn').on('click', function () {
        _AddDebitMemo();
    });

    var bm = new BolViewModel();
    ko.applyBindings(bm);

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $('#containerParts').DataTable().columns.adjust().draw();
        $('#containers').DataTable().columns.adjust().draw();
        $('#purchaseOrders').DataTable().columns.adjust().draw();
        $('#buckets').DataTable().columns.adjust().draw();
        $('#pallets').DataTable().columns.adjust().draw();
    });
});

function _AddContainer() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_AddContainer",
        success: function (result) {

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#addContainerModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _AddParts() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_AddParts",
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

function _ViewShipCodeNotes(notes) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_ViewShipCodeNotes",
        data: { "notes": notes },
        success: function (result) {

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#viewShipCodeNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _EditPurchaseOrder(model) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_EditPurchaseOrder",
        data: model,
        success: function (result) {

            $('#editPurchaseOrderDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            GetDatePicker();

            $('#editPurchaseOrderModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewContainerPartDetail(model) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Shipment/_ViewContainerPartDetails",
        data: model,
        success: function (result) {

            $('#viewContainerPartDetailDiv').html(result);

            $('.successAlert').hide();
            $('.errorAlert').hide();

            $('#viewContainerPartDetailModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};
