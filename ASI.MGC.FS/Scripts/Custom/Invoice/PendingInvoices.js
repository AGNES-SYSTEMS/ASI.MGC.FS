$(document).ready(function () {
    jQuery("#tblPendingInvoices").jqGrid({
        url: '/Invoice/GetPendingInvoices',
        datatype: "json",
        height: 250,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        rownumbers: true,
        styleUI: "Bootstrap",
        colNames: ['Sale Date', 'MRV No', 'Job No', 'Product Code', 'SoW Code'],
        colModel: [
            { name: 'SALEDATE_SD', index: 'SALEDATE_SD', width: 150, align: "center", sortable: true,formatter: 'date', formatoptions: { srcformat: 'd/m/Y', newformat: 'd/m/Y'} },
            { name: 'MRVNO_SD', index: 'MRVNO_SD', width: 150, align: "center", sortable: false },
            { name: 'JOBNO_SD', index: 'JOBNO_SD', width: 150, align: "center", sortable: false },
            { name: 'PRCODE_SD', index: 'PRCODE_SD', width: 150, align: "center", sortable: false },
            { name: 'JOBID_SD', index: 'JOBID_SD', width: 150, align: "center", sortable: false }
        ],
        rowNum: 100,
        rowList: [100, 200, 500, 1000],
        mtype: 'GET',
        gridview: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#invPager'),
        multiselect: false,
        caption: "Pending Invoices",
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblPendingInvoices').setGridWidth(outerwidth);
    });
});