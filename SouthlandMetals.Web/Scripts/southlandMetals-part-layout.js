$(document).ready(function () {
    $('#operationsLink').addClass("category-current-link");
    $('#collapseOpsParts').addClass("in");
    $('#collapseOpsParts').attr("aria-expanded", "true");

    $('#opsPartsLink').addClass("category-current-link");
    $('#layoutLink').addClass("current-link");

    var layouts = [];

    $(document).on('click', '#searchBtn', function () {
        partId = null;
        var number = $('#partNumber').val();

        $.ajax({
            type: "GET",
            url: "/SouthlandMetals/Operations/Part/SearchProjectPart",
            data: { 'number': number },
            dataType: "json",
            success: function (result) {
                if (result.PartId != null) {
                    $('#partId').val(result.PartId);
                    getPartLayouts(result.PartId);
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

    function getPartLayouts(partId) {

        $.ajax({
            type: 'GET',
            url: "/SouthlandMetals/Operations/Part/GetPartLayouts",
            dataType: "json",
            contentType: "application/json",
            data: { 'partId': partId },
            cache: false,
            success: function (result) {

                $.each(result.Layouts, function (n, layout) {
                    var layoutString = '<div id="' + layout.LayoutId + '" class="col-md-3">' +
                                              '<a href="/SouthlandMetals/Operations/Part/GetLayout/?layoutId=' + layout.LayoutId + '&isProject=' + layout.IsProject + '" target="_blank"><img src="/Content/images/southland_pdf_48.png" class="img-responsive" alt="pdf" /></a>' +
                                              '<div style="display: inline-block;">' +
                                                  '<span class="pull-left">' + layout.Description + '</span>&nbsp;&nbsp;' +
                                                  '<span id="' + layout.LayoutId + '" class="hidden">' + layout.LayoutId + '</span>' +
                                                  '<span style="padding-top: 4px;" aria-hidden="true" class="glyphicon glyphicon-trash pull-right" onclick="deleteLayout(\'' + layout.LayoutId + '\')"></span>' +
                                              '</div>' +
                                          '</div>';

                    $('#layoutGallery').append(layoutString);

                    layouts.push({ 'Description': layout.Description })
                });
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };

    $(document).on('click', '#fileSubmitBtn', function () {

        var myform = document.getElementById("attachment");
        var formData = new FormData(myform);

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Part/GetLayoutImageData",
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

                    $('#layout').val("");
                }
                else {
                    if (layouts != null && layouts.length > 0) {
                        var existingLayout = layouts.filter(function (n) {
                            return n.Description === result.Description;
                        });
                        if (existingLayout.length > 0) {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                         '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                         '<strong>Warning!</strong>&nbsp;Duplicate layout!</div>');

                            $('#layout').val("");
                        }
                        else {
                            addLayout(formData);
                        }
                    }
                    else {
                        addLayout(formData);
                    }
                }
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    });


    function addLayout(formData) {
        formData: {
                partId: $('#partId').val();
        };

        var lastId = $('#layoutGallery').find(">:first-child").attr('id');

        $.ajax({
            type: 'POST',
            url: "/SouthlandMetals/Operations/Part/AddLayout",
            dataType: "json",
            data: formData,
            contentType: false,
            async: true,
            cache: false,
            processData: false,
            success: function (result) {
                if (result.SUCCESS) {
                    var layoutString = '<div id="' + result.LayoutId + '" class="col-md-3">' +
                                                  '<a href="/SouthlandMetals/Operations/Part/GetLayout/?layoutId=' + result.LayoutId + '&isProject=' + result.IsProject + '" target="_blank"><img src="/Content/images/southland_pdf_48.png" class="img-responsive" alt="pdf" /></a>' +
                                                  '<div style="display: inline-block;">' +
                                                      '<span class="pull-left">' + result.Description + '</span>&nbsp;&nbsp;' +
                                                      '<span id="' + result.LayoutId + '" class="hidden">' + result.LayoutId + '</span>' +
                                                      '<span style="padding-top: 4px;" aria-hidden="true" class="glyphicon glyphicon-trash pull-right" onclick="deleteLayout(\'' + result.LayoutId + '\')"></span>' +
                                                  '</div>' +
                                              '</div>';
                    $("#" + lastId + "").before(layoutString);
                    $('#layout').val("");
                    layouts.push({ 'Description': result.Description });
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

    window.deleteLayout = function (layoutId) {
        $.confirm({
            text: 'Are you sure you want to delete the layout?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function () {
                var partId = $('#partId').val();
                $.ajax({
                    type: "DELETE",
                    url: "/SouthlandMetals/Operations/Part/DeleteLayout",
                    data: { 'partId': partId, 'layoutId': layoutId },
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            layouts.splice(layouts.indexOf(layoutId), 1);
                            $("#" + layoutId).remove();
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
});
