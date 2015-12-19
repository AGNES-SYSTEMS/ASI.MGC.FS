﻿$(document).ready(function () {
    jQuery("#tblJobDetails").jqGrid({
        url: '/Job/GetJobDetailsList',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Job Code', 'Job Name', 'Job Rate', 'Edit Actions'],
        colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 100, align: "center", sortable: false },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 150, align: "left", sortable: false },
            { key: false, name: 'RATE_RJ', index: 'RATE_RJ', width: 80, align: "center", sortable: false },
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