$(document).ready(function () {
    $('#frameWrap').show();
    var url = "/Reports/StockReport.aspx";
    $('#iframe').prop('src', url);

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });
});