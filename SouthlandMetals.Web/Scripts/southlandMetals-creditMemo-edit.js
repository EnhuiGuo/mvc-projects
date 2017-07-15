$(document).ready(function () {

    $('#collapseShipments').addClass("in");
    $('#collapseShipments').attr("aria-expanded", "true");

    $('#shipmentsLink').addClass("category-current-link");
    $('#creditMemoLink').addClass("current-link");

    var dollarFlag = function (data, type, row) {
        return "$" + data;
    };

    $('#creditAmount').text(GetTotalPrice(items));

    var creditMemoItemsTable = $('#creditMemoItems').DataTable({
        "autoWidth": false,
        "searching": false,
        "ordering": false,
        "paging": false,
        "scrollY": 300,
        "scrollCollapse": true,
        "info": false,
        "data": items,
        "columns": [
        { "data": "CreditMemoItemId", "title": "CreditMemoItemId", "visible": false },
        { "data": "CreditMemoId", "title": "CreditMemoId", "visible": false },
        { "data": "ItemQuantity", "title": "Quantity", "class": "center" },
        { "data": "ItemDescription", "title": "Description", "class": "center" },
        { "data": "ItemPrice", "title": "Unit Price", "class": "center", "render": dollarFlag },
        { "data": "ExtendedPrice", "title": "Extended Price", "class": "center", "render": dollarFlag },
        { "title": "Edit", "class": "center" },
        ],
        "columnDefs": [{
            "targets": 6,
            "title": "Edit",
            "width": "8%", "targets": 6,
            "data": null,
            "defaultContent":
                "<span class='glyphicon glyphicon-pencil' id='editItemBtn'></span>"
        }]
    });

    $('#creditMemoItems tbody').on('click', '#editItemBtn', function () {
        var creditMemoItem = creditMemoItemsTable.row($(this).parents('tr')).data();
        var childData = creditMemoItemsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (creditMemoItem != null) {
            data = creditMemoItem;
        }
        else {
            data = childData;
        }
        _EditCreditMemoItem(data);
    });

    $(document).on('click', '#updateCreditMemoItemBtn', function () {

        event.preventDefault();

        if (!$("#editCreditMemoItemForm")[0].checkValidity()) {
            $('.partError').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.partError').show();
            $('#editCreditMemoItemForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var creditMemoItemId = $('#editCreditMemoItemId').val();
            var quantity = $('#editItemQuantity').val();
            var description = $('#editItemDescription').val();
            var price = $('#editItemPrice').val();
            var extendedPrice = parseFloat(quantity) * parseFloat(price);

            $.each(items, function (n, item) {
                if (item.CreditMemoItemId == creditMemoItemId) {
                    item.ItemQuantity = quantity;
                    item.ItemDescription = description;
                    item.ItemPrice = price;
                    item.ExtendedPrice = extendedPrice;
                }
            });

            $('#creditMemoItems').DataTable().clear().draw();
            $('#creditMemoItems').DataTable().rows.add(items); // Add new data
            $('#creditMemoItems').DataTable().columns.adjust().draw();

            $('#creditAmount').text(GetTotalPrice(items));

            $('#creditMemoItemModal').modal('hide');
        }
    });

    function _EditCreditMemoItem(creditMemoItem) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Operations/Shipment/_EditCreditMemoItem",
            success: function (result) {

                $('#editCreditMemoItemDiv').html('');
                $('#editCreditMemoItemDiv').html(result);

                $('.partSuccess').hide();
                $('.partError').hide();

                $('#editCreditMemoItemId').val(creditMemoItem.CreditMemoItemId);
                $('#editItemQuantity').val(creditMemoItem.ItemQuantity);
                $('#editItemDescription').val(creditMemoItem.ItemDescription);
                $('#editItemPrice').val(creditMemoItem.ItemPrice);

                $('#creditMemoItemModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };


    $(document).on('click', '#updateMemoBtn', function () {
        event.preventDefault();

        var creditAmount = GetTotalPrice(items);

        var model = {
            CreditMemoId: creditMemoId,
            DebitMemoId: debitMemoId,
            CreditMemoNumber: creditMemoNumber,
            CreditMemoDateStr: creditMemoDateStr,
            CustomerId: customerId,
            SalespersonId: salespersonId,
            CreditAmount: creditAmount,
            CreditMemoNotes: creditMemoNotes,
            CreditMemoItems: items,
        };

        $.ajax({
            type: "POST",
            url: "/SouthlandMetals/Operations/Shipment/UpdateCreditMemo",
            data: JSON.stringify(model),
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                if (result.Success) {

                    window.location.href = '/SouthlandMetals/Operations/Shipment/CreditMemo?creditMemoId=' + creditMemoId + '';
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
    });
});