$(document).ready(function () {
    $("#txtNewFYFrom").datepicker();
    $("#txtNewFYTo").datepicker();
    jQuery("#tblFinancialYearDetails").jqGrid({
        url: '/Setups/GetAllFinancialYears',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Financial Year From', 'Financial Year To', 'Current Financial Year', 'Edit Actions'
        ],
        colModel: [
            { key: true, name: 'PERRIEDFROM_FM', index: 'PERRIEDFROM_FM', width: 80, align: "center", formatter: 'date', sortable: false },
            { key: false, name: 'PERRIEDRTO_FM', index: 'PERRIEDRTO_FM', width: 150, align: "center", formatter: 'date', sortable: false },
            { key: false, name: 'CURRENTPERIOD_FM', index: 'CURRENTPERIOD_FM', width: 80, align: "center", formatter: "checkbox", sortable: false },
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
        caption: "Financial Year Details",
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
        $('#tblFinancialYearDetails').setGridWidth(outerwidth);
    });
});