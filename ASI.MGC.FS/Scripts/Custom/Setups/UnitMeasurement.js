$(document).ready(function () {
    jQuery("#tblUnitDetails").jqGrid({
        url: '/Product/GetAllUnitsList',
        datatype: "json",
        height: 450,
        autowidth: true,
        colNames: [
            'Unit Code', 'Unit Detail', 'Unit Type', 'Quantity', 'Basic Primary Unit',
        'Basic Primary Qty'],
        colModel: [
            { key: true, name: 'UNIT_UM', index: 'UNIT_UM', width: 80, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_UM', index: 'DESCRIPTION_UM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TYPE_UM', index: 'TYPE_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'UNITQTY_UM', index: 'UNITQTY_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'BASICPRIMARYUNIT_UM', index: 'BASICPRIMARYUNIT_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'BASICPRIMARYQTY_UM', index: 'BASICPRIMARYQTY_UM', width: 80, align: "left", sortable: false }
        ],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "Unit Details",
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
        $('#tblUnitDetails').setGridWidth(outerwidth);
    });
});