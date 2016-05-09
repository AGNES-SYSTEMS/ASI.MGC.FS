$(document).ready(function () {
    $('#frameWrap').show();
    var url = "/Reports/StockReceipt.aspx";
    $('#iframe').prop('src', url);

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });
});