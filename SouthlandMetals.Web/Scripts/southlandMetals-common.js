$(document).ready(function () {
    $('.main-collapse').click(function () {
        $('span', this).toggleClass("glyphicon-menu-right glyphicon-menu-down");
    });

    $('.secondary-collapse').click(function () {
        $('span', this).toggleClass("glyphicon-menu-right glyphicon-menu-down");
    });

    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
        $(this).toggleClass("glyphicon-align-right glyphicon-align-left");
    });

    $('.panel-heading.parts').click(function () {
        $('i', this).toggleClass("glyphicon-menu-right");
    });

    $('.panel-heading').click(function () {
        $('i', this).toggleClass("glyphicon-menu-right glyphicon-menu-down");
    });

    $('#administrationLink').click(function () {
        $('#administrationLink span').first().toggleClass("glyphicon-menu-right");
    });

    $('.parts').click(function () {
        $('.parts span').first().toggleClass("glyphicon-menu-right");
    });

    $(window).scroll(function () {
        //if you hard code, then use console
        //.log to determine when you want the 
        //nav bar to stick.  
        if ($(window).scrollTop() > 10) {
            //$('#partsNav').removeClass('navbar-static-top').addClass('navbar-fixed-top');
            $('#partsNav').addClass('navbar-fixed-top');
        }
        if ($(window).scrollTop() < 11) {
            //$('#partsNav').removeClass('navbar-fixed-top').addClass('navbar-static-top');
            $('#partsNav').removeClass('navbar-fixed-top');
        }
    });

    $('.dropdown-menu').click(function (e) {
        e.stopPropagation();
    });

    window.opts = {
        lines: 13 // The number of lines to draw
        , length: 15 // The length of each line
        , width: 7 // The line thickness
        , radius: 23 // The radius of the inner circle
        , corners: 1 // Corner roundness (0..1)
        , color: '#000' // #rgb or #rrggbb or array of colors
        , rotate: 0 // The rotation offset
        , direction: 1 // 1: clockwise, -1: counterclockwise
        , speed: 1 // Rounds per second
        , trail: 60 // Afterglow percentage
        , zIndex: 2e9 // The z-index (defaults to 2000000000)
        , className: 'spinner' // The CSS class to assign to the spinner
        , top: '50%' // Top position relative to parent
        , left: '50%' // Left position relative to parent
        , shadow: false // Whether to render a shadow
        , hwaccel: false // Whether to use hardware acceleration
        , position: 'absolute' // Element positioning
    };

    function formatDates(dateToFormat) {

        var year = dateToFormat.getFullYear();
        var month = (dateToFormat.getMonth() + 1).toString();
        var day = dateToFormat.getDate().toString();

        var formattedDate = month + "/" + day + "/" + year;

        return formattedDate;
    };

    $.connection.hub.start();

    window.signalR = $.connection.notificationHub;

    signalR.client.refreshNotification = function (data) {
        var messages = data;

        $('#notificationCounter').empty();

        $('#notificationCounter')
          .css({ opacity: 0 })
          .text(messages.length)
          .animate({ top: '11px', opacity: 1 }, 500);

        $('#notificationTab').empty();

        for (var i = 0; i < messages.length; i++) {
            var date = new Date(messages[i].CreatedDate);
            var messageDate = formatDates(date);
            $("#notificationTab").append("<tr id='" + messages[i].NotificationId + "'><td> " + messages[i].Text + "</td> <td>" + messages[i].CreatedBy + "</td> <td>" + messageDate + "</td> <td><span class='glyphicon glyphicon-trash' id='deleteBtn'></span></td></tr>");
        }

        $('#notificationTab tr').on('click', '#deleteBtn', function () {
            var notificationId = $(this).closest($('#notificationTab tr')).attr('id');
            signalR.server.deleteNotification(notificationId);
            $('#' + notificationId).remove();
        });
    };

    $('.alert-success').delay(3000).hide("fast");
});

//window.dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

//function convertDate(dateToConvert) {
//    var x = dateToConvert.toString();
//    var y = x.substring(6, x.length - 2);
//    var z = new Date(parseFloat(y));
//    z.toUTCString();

//    var convertedDate = formatDate(z);

//    return convertedDate;
//};

//function convertChangedMonthDate(dateToConvert) {
//    var month = new Date(dateToConvert).getMonth() + 1;
//    var year = new Date(dateToConvert).getFullYear();

//    var lastDayOfMonth = getLastDayOfMonth(year, month - 1);
//    var changeYear = lastDayOfMonth.getFullYear();
//    var changeMonth = (lastDayOfMonth.getMonth() + 1).toString();
//    var changeDay = lastDayOfMonth.getDate().toString();

//    var convertedDate = changeMonth + "/" + changeDay + "/" + changeYear;

//    return convertedDate;
//};

//function convertCalendarDate(dateToConvert) {
//    var day = new Date(dateToConvert).getDate().toString();
//    var month = (new Date(dateToConvert).getMonth() + 1).toString();
//    var year = new Date(dateToConvert).getFullYear().toString();

//    var convertedDate = month + "/" + day + "/" + year;

//    return convertedDate;
//};

//function convertChangedDate(dateToConvert) {
//    var date = new Date(dateToConvert);
//    var convertedDate = formatDate(date);

//    return convertedDate;
//};

//function getLastDayOfMonth(Year, Month) {
//    return (new Date((new Date(Year, Month + 1, 1)) - 1));
//};

function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
};

$('.datepicker').datepicker({
    format: 'm/dd/yyyy',
    orientation: 'down'
}).on('changeDate', function (e) {
    $(this).datepicker('hide');
});

function GetTotalWeight(parts) {
    var totalWeight = 0;
    if (parts.length > 0) {
        for (var i = 0; i < parts.length; i++) {
            totalWeight += parts[i].Weight * parts[i].AnnualUsage;
        }
    }

    totalWeight = CurrencyFormatted(totalWeight);

    $('#totalWeight').text(totalWeight);
};

function CurrencyFormatted(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = minus + s;
    return s;
};

function GetTotalCost(items) {
    console.log(items);
    totalCost = 0;
    if (items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            totalCost += items[i].ExtendedCost;
        }
    }

    totalCost = CurrencyFormatted(totalCost);

    $('#debitAmount').val(totalCost);
    $('#debitAmount').text(totalCost);
};

function GetTotalPrice(items) {
    var totalPrice = 0;
    if (items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            totalPrice += items[i].ExtendedPrice;
        }
    }

    totalPrice = CurrencyFormatted(totalPrice);

    return totalPrice;
};

function GetDatePicker() {
    $('.datepicker').datepicker({
        format: 'm/dd/yyyy',
        orientation: 'down'
    }).on('changeDate', function (e) {
        $(this).datepicker('hide');
    });
};

function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

function removeNumberComma(number) {
    var s = parseFloat(number.toString().replace(/,/g, ''), 10);
    return s;
}

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

var dateFlag = function (data, type, row) {
    if (data != null && data.substring(0, 6) == "/Date(") {
        var date = new Date(parseInt(data.substr(6)));

        return date.toLocaleDateString();
    }
    return data;
}

var weightFlag = function (data, type, row) {
    return data + " lbs"
};