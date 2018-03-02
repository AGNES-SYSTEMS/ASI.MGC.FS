$(document).ready(function () {
    $('#frameWrap').show();
    var url = "";
    var isExportMode = getQueryStringByName("isExportMode", document.location.href);
    if (isExportMode !== null && isExportMode !== "") {
        url = "/Reports/StockReport.aspx?isExportMode=" + isExportMode;
    } else {
        url = "/Reports/StockReport.aspx";
    }
    $('#iframe').prop('src', url);

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });
});