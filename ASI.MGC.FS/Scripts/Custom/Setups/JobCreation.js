$(document).ready(function () {
    jQuery("#tblJobDetails").jqGrid({
        url: '/Job/GetJobDetailsList',
        datatype: "json",
        height: 450,
        autowidth: true,
        colNames: [
            'Job Code', 'Job Name', 'Job Rate'],
        colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 100, align: "center", sortable: false },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 150, align: "left", sortable: false },
            { key: false, name: 'RATE_RJ', index: 'RATE_RJ', width: 80, align: "center", sortable: false }
        ],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "Job Details",
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
        $('#tblJobDetails').setGridWidth(outerwidth);
    });
});