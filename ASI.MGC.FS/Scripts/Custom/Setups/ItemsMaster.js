$(document).ready(function () {
    var $status = "SP";
    jQuery("#tblItemDetails").jqGrid({
        url: '/Product/GetProdsList?status='+$status,
        datatype: "json",
        height: 450,
        autowidth: true,
        colNames: [
            'Item Code', 'Item Name', 'Item Status'],
        colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 100, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 150, align: "left", sortable: false },
            { key: false, name: 'STATUS_PM', index: 'STATUS_PM', width: 80, align: "center", sortable: false }
        ],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "Item Details",
        emptyrecords: "No Data to Display",
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false
        },
        multiselect: false
    }).navGrid("#Pager", { edit: false, add: false, del: false });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblItemDetails').setGridWidth(outerwidth);
    });
});