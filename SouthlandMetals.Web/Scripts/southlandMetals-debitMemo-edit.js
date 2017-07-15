﻿$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#debitMemoLink').addClass("current-link");

    if (status === "Open") {
        $('#open').prop('checked', true);
    }
    else if (status === "Closed") {
        $('#closed').prop('checked', true);
    }

    var selectableParts = [];

    if (attachments == null)
    {
        attachments = [];
    }

    getSelectablePartsByCustomer(customerId);

    $.each(items, function (i, item) {
        item.DateCode = dateFlag(item.DateCode);
    });

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };
 
    var debitMemoItemsTable = $('#debitMemoItems').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "order": [3, 'asc'],
        "data": items,
        "columns": [
        { "data": "DebitMemoItemId", "title": "DebitMemoItemId", "visible": false },
        { "data": "DebitMemoId", "title": "DebitMemoId", "visible": false },
        { "data": "PartNumber", "title": "Part Number", "class": "center" },
        { "data": "ItemQuantity", "title": "Quantity", "class": "center" },
        { "data": "ItemDescription", "title": "Description", "class": "center" },
        { "data": "ItemCost", "title": "Unit Cost", "class": "center", "render": dollarFlag },
        { "data": "ExtendedCost", "title": "Extended Cost", "class": "center", "render": dollarFlag },
        { "data": "DateCode", "name": "DateCode", "title": "Date Code", "class": "center", render: dateFlag },
        { "data": "Reason", "title": "Reason", "class": "center" },
        { "title": "Edit", "class": "center" },
        { "title": "Delete", "class": "center" },
        ],
        "columnDefs": [{
            "targets": 9,
            "title": "Edit",
            "width": "8%", "targets": 9,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editItemBtn'></span>"
        },
        {
            "targets": 10,
            "title": "Delete",
            "width": "8%", "targets": 10,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-trash' id='deleteItemBtn'></span>"
        }]
    });

    $('#debitMemoItems tbody').on('click', '#editItemBtn', function () {
        var debitMemoItem = debitMemoItemsTable.row($(this).parents('tr')).data();
        var childData = debitMemoItemsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (debitMemoItem != null) {
            data = debitMemoItem;
        }
        else {
            data = childData;
        }
        _EditDebitMemoItem(data);
    });

    $('#debitMemoItems tbody').on('click', '#deleteItemBtn', function () {
        var debitMemoItem = debitMemoItemsTable.row($(this).parents('tr')).data();
        var childData = debitMemoItemsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (debitMemoItem != null) {
            data = debitMemoItem;
        }
        else {
            data = childData;
        }

        $.confirm({
            text: 'Are you sure you want to delete this item?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                debitMemoItemsTable.row(debitMemoItem).remove().draw();

                items.splice(items.findIndex(x=>x.ItemDescription == data.ItemDescription), 1);

                GetTotalCost(items);
            },
            cancel: function (button) {

            }
        });
    });

    $('#addItem').click(function () {
        if ($('#customerId option:selected').text() === "--Select Customer--") {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 '<strong>Warning!</strong>&nbsp;Please select a Customer!</div>');
        }
        else {
            _AddDebitMemoItem();
        }
    });

    $(document).on('click', '#cancelAddDebitMemoItemBtn', function () {
        $('#addDebitMemoItemModal').modal('hide');
    });

    $(document).on('change', '#parts', function () {
        $(this).find(":selected").each(function () {
            partId = $(this).val();

            getPartById(partId);
        });
    });

    $(document).on('click', '#saveDebitMemoItemBtn', function () {
        event.preventDefault();

        if (!$("#addDebitMemoItemForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#addDebitMemoItemForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var partNumber = $('#partNumber').val();
            var quantity = $('#itemQuantity').val();
            var description = $('#itemDescription').val();
            var cost = $('#itemCost').val();
            var extendedCost = parseFloat(quantity) * parseFloat(cost);
            var price = $('#itemPrice').val();
            var extendedPrice = parseFloat(quantity) * parseFloat(price);
            var dateCode = $('#itemDateCode').val();
            var reason = $('#itemReason').val();

            if (partNumber === "") {
                $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please enter an Item!</div>');
                $('.partError').show();
            }
            else {
                var existingItem = items.filter(function (n) {
                    return n.PartNumber === partNumber;
                });

                if (existingItem.length > 0) {
                    $('.partError').append('<div><strong>Warning!</strong>&nbsp;Item is already included in Debit Memo!</div>');
                    $('.partError').show();
                }
                else {
                    var debitMemoItemId = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();

                    items.push({
                        'DebitMemoItemId': debitMemoItemId, 'DebitMemoId': debitMemoId, 'ItemQuantity': quantity, 'ItemDescription': description,
                        'ItemCost': cost, 'ExtendedCost': extendedCost, 'ItemPrice': price, 'ExtendedPrice': extendedPrice, 'PartNumber': partNumber,
                        'DateCode': dateCode, 'Reason': reason
                    });

                    $('#debitMemoItems').DataTable().clear().draw();
                    $('#debitMemoItems').DataTable().rows.add(items); // Add new data
                    $('#debitMemoItems').DataTable().columns.adjust().draw();

                    GetTotalCost(items);

                    $('#addDebitMemoItemModal').modal('hide');
                }
            }
        }
    });

    $('#cancelEditDebitMemoItemBtn').on('click', function () {
        $('#editDebitMemoItemModal').modal('hide');
    });

    $(document).on('click', '#updateDebitMemoItemBtn', function () {

        event.preventDefault();

        if (!$("#editDebitMemoItemForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#editDebitMemoItemForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var debitMemoItemId = $('#editDebitMemoItemId').val();
            var quantity = $('#editItemQuantity').val();
            var description = $('#editItemDescription').val();
            var cost = $('#editItemCost').val();
            var extendedCost = parseFloat(quantity) * parseFloat(cost);
            var price = $('#editItemPrice').val();
            var extendedPrice = parseFloat(quantity) * parseFloat(price);
            var dateCode = $('#editItemDateCode').val();
            var reason = $('#editItemReason').val();
            var partNumber = $('#editPartNumber').val();

            $.each(items, function (n, item) {
                if (item.PartNumber == partNumber) {
                    item.ItemQuantity = quantity;
                    item.ItemDescription = description;
                    item.ItemCost = cost;
                    item.ExtendedCost = extendedCost;
                    item.ItemPrice = price;
                    item.ExtendedPrice = extendedPrice;
                    item.DateCode = dateCode;
                    item.Reason = reason;
                }
            });

            $('#debitMemoItems').DataTable().clear().draw();
            $('#debitMemoItems').DataTable().rows.add(items); // Add new data
            $('#debitMemoItems').DataTable().columns.adjust().draw();

            GetTotalCost(items);

            $('#editDebitMemoItemModal').modal('hide');
        }
    });

    function _AddDebitMemoItem() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/_AddDebitMemoItem",
            success: function (result) {

                $('#addDebitMemoItemDiv').html('');
                $('#addDebitMemoItemDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $.each(selectableParts, function (i, item) {
                    $('#parts').append($('<option>', {
                        value: item.Value,
                        text: item.Text
                    }));
                });

                $('.datepicker').datepicker({
                    format: 'm/dd/yyyy',
                    orientation: 'down'
                }).on('changeDate', function (e) {
                    $(this).datepicker('hide');
                });

                $('#addDebitMemoItemModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function _EditDebitMemoItem(debitMemoItem) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/_EditDebitMemoItem",
            success: function (result) {

                $('#editDebitMemoItemDiv').html('');
                $('#editDebitMemoItemDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();
                
                $('#editDebitMemoItemId').val(debitMemoItem.DebitMemoItemId);
                $('#editPartNumber').val(debitMemoItem.PartNumber);
                $('#editItemQuantity').val(debitMemoItem.ItemQuantity);
                $('#editItemDescription').val(debitMemoItem.ItemDescription);
                $('#editItemCost').val(debitMemoItem.ItemCost);
                $('#editItemPrice').val(debitMemoItem.ItemPrice);
                $('#editItemDateCode').val(dateFlag(debitMemoItem.DateCode));
                $('#editItemReason').val(debitMemoItem.Reason);
                
                GetDatePicker();

                $('#editDebitMemoItemModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    $(document).on('click', '#fileSubmitBtn', function () {

        if ($('#attachment').val() != "") {
            var myform = document.getElementById("debitMemoAttachment");
            var formData = new FormData(myform);

            $.ajax({
                type: 'POST',
                url: "/SouthlandMetals/Operations/Shipment/GetImageData",
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
                    }
                    else {
                        var existingAttachment = attachments.filter(function (n) {
                            return n.AttachmentName === result.AttachmentName;
                        });

                        if (existingAttachment.length > 0) {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                         '<strong>Warning!</strong>&nbsp;Duplicate attachment!</div>');
                        }
                        else {
                            addAttachment(formData);
                        }
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
        else {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
             '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
             '<strong>Warning!</strong>&nbsp;No attachment!</div>');
        }
    });

    function addAttachment(formData) {

        formData.append('DebitMemoId', debitMemoId);

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Shipment/AddAttachment",
            dataType: "json",
            data: formData,
            contentType: false,
            async: true,
            cache: false,
            processData: false,
            success: function (result) {
                if (result.Success) {
                    var attachmentString = '<div id="' + result.DebitMemoAttachmentId + '" class="col-md-3">' +
                                                  '<a href="/SouthlandMetals/Operations/Shipment/GetAttachment/?attachmentId=' + result.DebitMemoAttachmentId + '" target="_blank"><img src="/Content/images/southland_png_48.png" class="img-responsive" alt="png" /></a>' +
                                                  '<div style="display: inline-block;">' +
                                                      '<span class="pull-left">' + result.AttachmentName.substr(0, result.AttachmentName.indexOf('.')) + '</span>&nbsp;&nbsp;' +
                                                      '<span id="' + result.DebitMemoAttachmentId + '" class="hidden">' + result.DebitMemoAttachmentId + '</span>' +
                                                      '<span style="padding-top: 4px;" aria-hidden="true" class="glyphicon glyphicon-trash pull-right" onclick="deleteAttachment(\'' + result.DebitMemoAttachmentId + '\')"></span>' +
                                                  '</div>' +
                                              '</div>';
                    $('#attachmentGallery').append(attachmentString);
                    $('#attachment').val(null);
                    attachments.push({ 'DebitMemoAttachmentId': result.DebitMemoAttachmentId, 'AttachmentName': result.AttachmentName });
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

    window.deleteAttachment = function (attachmentId) {
        $.confirm({
            text: 'Are you sure you want to delete the attachment?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function () {
                $.ajax({
                    type: "DELETE",
                    url: "/SouthlandMetals/Operations/Shipment/DeleteAttachment",
                    data: { 'attachmentId': attachmentId },
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            attachments.splice(attachments.indexOf(attachmentId), 1);
                            $("#" + attachmentId).remove();
                            $('#attachment').val(null);
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
            },
            cancel: function () {

            }
        });
    };

    $('#closed').change(function () {

        $.confirm({
            text: 'There is no credit memo referenced, Are you sure you want to close?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $('#closed').prop('checked', true);
            },
            cancel: function (button) {
                $('#open').prop('checked', true);
            }
        });

    });

    $(document).on('click', '#updateMemoBtn', function () {
        event.preventDefault();

        if (!$("#editDebitMemoForm")[0].checkValidity()) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                           '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                           '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('#editDebitMemoForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var selectedVal = "";
            var selected = $("input[name='Status']:checked");
            if (selected.length > 0) {
                selectedVal = selected.val();
            }

            var creditAmount = GetTotalPrice(items);

            var model = {
                Status: selectedVal,
                DebitMemoId: debitMemoId,
                CreditMemoId: creditMemoId,
                FoundryInvoiceId: $('#editFoundryInvoiceId').val(),
                DebitMemoNumber: debitMemoNumber,
                DebitMemoDateStr: $('#editDebitMemoDate').val(),
                FoundryId: $('#editFoundryId').val(),
                CustomerId: $('#editCustomerId').val(),
                SalespersonId: $('#editSalespersonId').val(),
                RmaNumber: $('#editRmaNumber').val(),
                TrackingNumber: $('#editTrackingNumber').val(),
                DebitAmount: totalCost,
                DebitMemoNotes: $('#editNotes').val(),
                DebitMemoItems: items,
                CreditAmount: creditAmount
            };

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Operations/Shipment/EditDebitMemo",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {

                        window.location.href = '/SouthlandMetals/Operations/Shipment/DebitMemoDetail?debitMemoId=' + debitMemoId + '';
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

    $('#customerId').change(function () {
        customerId = $('#customerId').val();

        getSalespersonByCustomer(customerId);
    });

    $('#customerId').change(function () {
        var customerId = $('#customerId').val();
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/GetSalesmanByCustomer",
            data: { 'customerId': customerId },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                $('#salespersonId').val(result.SalespersonId);
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    $('#foundryId').change(function () {
        var foundryId = $('#foundryId').val();
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/GetSelectableFoundryInvoicesByFoundry",
            data: { 'foundryId': foundryId },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                if (result.SelectableFoundryInvoices.length > 0) {
                    $('#foundryInvoiceId').empty();

                    $.each(result.SelectableFoundryInvoices, function (n, invoice) {
                        $("#foundryInvoiceId").append($("<option />").val(invoice.Value).text(invoice.Text));
                    });
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    function getSalespersonByCustomer(customerId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/Customer/GetSalespersonByCustomer",
            data: { 'customerId': customerId },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                $('#salespersonId').val(result.SalespersonId);
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getSelectablePartsByCustomer(customerId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/Customer/GetSelectablePartsByCustomer",
            data: { "customerId": customerId },
            contentType: "application/json",
            success: function (result) {

                selectableParts = [];
                $.each(result, function (n, part) {
                    selectableParts.push({ Value: part.Value, Text: part.Text });
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    function getPartById(partId) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Part/GetPart",
            data: { "partId": partId },
            contentType: "application/json",
            success: function (result) {
                partId = result.PartId;

                $('#partNumber').val(result.PartNumber)
                $('#itemDescription').val(result.PartDescription);
                $('#itemCost').val(result.Cost);
                $('#itemPrice').val(result.Price);
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});