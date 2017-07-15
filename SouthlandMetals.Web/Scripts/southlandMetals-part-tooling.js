$(document).ready(function () {
    $(document).on('click', '#searchBtn', function () {

        var target = document.getElementById('spinnerDiv')
        var spinner = new Spinner(opts).spin(target);

        $("#page-content-wrapper").css({ opacity: 0.5 });
        $('#spinnerDiv').show();

        var number = $('#partNumber').val();

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Part/SearchProjectPart",
            data: { 'number': number },
            dataType: "json",
            success: function (result) {

                $("#page-content-wrapper").css({ opacity: 1.0 });
                spinner.stop(target);
                $('#spinnerDiv').hide();

                if (result.PartId != null) {
                    $('#partId').val(result.PartId);
                    getToolingInfo(result.PartId);
                }
                else {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                         '<strong>Warning!</strong>&nbsp;Unable to find part requested.</div>');
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });

    function getToolingInfo(partId) {
        $.ajax({
            type: 'GET',
            url: "/SouthlandMetals/Operations/Part/GetToolingInfo",
            cache: false,
            data: { 'partId': partId },
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                if (result.PartId != null) {
                    $('#partNumber').val(result.PartNumber);
                    $('#toolingOrderNumber').val(result.ToolingOrderNumber);
                    $('#toolingDescription').val(result.ToolingDescription);
                    $('#notes').val(result.Notes);
                }
                else {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                         '<strong>Warning!</strong>&nbsp;No tooling info for this part was found!</div>');
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    }
});