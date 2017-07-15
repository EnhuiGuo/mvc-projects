$(document).ready(function () {
    $(document).on('click', '#updateFoundryInvoiceBtn', function () {
        event.preventDefault();

        var schedulePaymentDate = $('#scheduledPaymentDate').val();
        var actualPaymentDate = $('#actualPaymentDate').val();

        if (schedulePaymentDate < actualPaymentDate) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                '<strong>Warning!</strong>&nbsp;Please review payment dates, actual cannot be applied without scheduling.</div>');
        }
        else {
            var model = {
                FoundryInvoiceId: foundryInvoiceId,
                FoundryId: foundryId,
                InvoiceNumber: $('#invoiceNumber').val(),
                InvoiceAmount: $('#invoiceAmount').val(),
                ScheduledPaymentDate: schedulePaymentDate,
                ActualPaymentDate: actualPaymentDate,
                Notes: $('#invoiceNotes').val(),
                AirFreight: airFreight,
                HasBeenProcessed: hasBeenProcessed,
                Buckets: buckets
            }

            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Accounting/Invoicing/UpdateFoundryInvoice",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    window.location.href = "/SouthlandMetals/Accounting/Invoicing/Invoices"
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });
});