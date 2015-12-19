$(document).ready(function () {
    $('#txtPassIssue').datepicker();
    $('#txtPassExp').datepicker();
    $('#txtVisaIssue').datepicker();
    $('#txtVisaExp').datepicker();
    jQuery("#tblEmpDetails").jqGrid({
        url: '/EmployeeMaster/GetEmployeeDetailsList',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Employee Code', 'First Name', 'Last Name', 'Designation', 'Passport No', 'Phone', 'Pass Issue Date', 'Pass Exp Date', 'Visa Issue Date', 'Visa Exp Date', 'Edit Actions'],
        colModel: [
            { key: true, name: 'EMPCODE_EM', index: 'EMPCODE_EM', width: 80, align: "lefy", sortable: false },
            { key: false, name: 'EMPFNAME_EM', index: 'EMPFNAME_EM', width: 100, align: "left", sortable: false },
            { key: false, name: 'EMPSNAME_EM', index: 'EMPSNAME_EM', width: 100, align: "left", sortable: false },
            { key: false, name: 'DESIGNATION_EM', index: 'DESIGNATION_EM', width: 80, align: "left", sortable: false },
            { key: false, name: 'PASSPORTNO_EM', index: 'PASSPORTNO_EM', width: 80, align: "left", sortable: false },
            { key: false, name: 'PHONE_EM', index: 'PHONE_EM', width: 80, align: "left", sortable: false },
            { key: false, name: 'PASSPORTISSUEDATE_EM', index: 'PASSPORTISSUEDATE_EM', width: 80,formatter : 'date', align: "left", sortable: false },
            { key: false, name: 'PASSPORTEXPDATE_EM', index: 'PASSPORTEXPDATE_EM', width: 80, formatter: 'date', align: "left", sortable: false },
            { key: false, name: 'VISAISSUEDATE_EM', index: 'VISAISSUEDATE_EM', width: 80, formatter: 'date', align: "left", sortable: false },
            { key: false, name: 'VISAEXPIEARYDATE_EM', index: 'VISAEXPIEARYDATE_EM', width: 80, formatter: 'date', align: "left", sortable: false },
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
        caption: "Employee Details",
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
        $('#tblEmpDetails').setGridWidth(outerwidth);
    });
});