$(document).ready(function () {
    var $status = "IP";
    jQuery("#tblPrdDetails").jqGrid({
        url: '/Product/GetProdsList?status=' + $status,
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        loadonce:true,
        colNames: [
            'Product Code', 'Product Name', 'Current Qty', 'Rate', 'Selling Price',
        'Purchase Unit', 'Sale Unit', 'Unit', 'Product Status', 'Edit Actions'],
        colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 80, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 150, align: "left", sortable: false },
            { key: false, name: 'CUR_QTY_PM', index: 'CUR_QTY_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'RATE_PM', index: 'RATE_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'SELLINGPRICE_RM', index: 'SELLINGPRICE_RM', width: 80, align: "left", sortable: false },
            { key: false, name: 'PURCHSEUNIT_PM', index: 'PURCHSEUNIT_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'SALESUNIT_PM', index: 'SALESUNIT_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'UNIT_PR', index: 'UNIT_PR', width: 80, align: "left", sortable: false },
            { key: false, name: 'STATUS_PM', index: 'STATUS_PM', width: 80, align: "left", sortable: false },
            {
                name: "actions",
                width: 100,
                formatter: "actions",
                formatoptions: {
                    keys: true,
                    editOptions: {},
                    addOptions: {},
                    delOptions: {}
                }
            }
        ],
        rowNum: 40,
        rowList: [40, 100, 500, 1000],
        mtype: 'GET',
        gridview: true,
        viewrecords: true,
        sortorder: "desc",
        pager: jQuery('#Pager'),
        caption: "Product Details",
        emptyrecords: "No Data to Display",
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false
        },
        multiselect: false
    }).navGrid("#Pager", {search:false, edit: false, add: false, del: false });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblPrdDetails').setGridWidth(outerwidth);
    });
});