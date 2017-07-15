$(document).ready(function () {
    $('#collapseAccounting').addClass("in");
    $('#collapseAccounting').attr("aria-expanded", "true");

    $('#accountingLink').addClass("category-current-link");
    $('#accountingReportsLink').addClass("current-link");

    $('#foundryInvoicesReport').click(function () {
        _FoundryInvoicesReport();
    });

    $("#unscheduled").change(function () {
        if (this.checked) {
            $('#startDate').prop('readonly', true);
            $('#endDate').prop('readonly', true);
        }
        else {
            $('#startDate').prop('readonly', false);
            $('#endDate').prop('readonly', false);
        }
    });

    $(document).on('click', '#foundryInvoicesReportBtn', function () {
        var startDate = $('#startDate').val();
        var endDate = $('#endDate').val();
        var unscheduled = $('#unscheduled').prop("checked");

        if (startDate > endDate) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                '<strong>Warning!</strong>&nbsp;Please check your date selection.</div>');
        }
        else {
            $('#foundryInvoicesReportModal').modal('hide');
            window.location.href = '/SouthlandMetals/Accounting/Reporting/FoundryInvoicesReport?startDate=' + startDate + '&endDate=' + endDate + '&unscheduled=' + unscheduled + '';
        }
    });
});

function _FoundryInvoicesReport() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Accounting/Report/_FoundryInvoiceReport",
        success: function (result) {
            $('#foundryInvoicesReportDiv').html('');
            $('#foundryInvoicesReportDiv').html(result);

            $('.datepicker').datepicker({
                format: 'm/dd/yyyy',
                orientation: 'down'
            }).on('changeDate', function (e) {
                $(this).datepicker('hide');
            });

            $('#foundryInvoicesReportModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};