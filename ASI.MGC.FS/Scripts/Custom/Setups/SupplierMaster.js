$(document).ready(function () {
    $("#txtGlDate").datepicker();
    var $custType = "AP";
    jQuery("#tblSupplierDetails").jqGrid({
        url: '/Customer/GetAllCustomers?custType=' + $custType,
        datatype: "json",
        height: 450,
        autowidth: true,
        colNames: [
            'Customer Code', 'Customer Name', 'PO Box', 'Address', 'Telephone', 'Fax', 'Email', 'Contact Person'],
        colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 100, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'POBOX_ARM', index: 'POBOX_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'ADDRESS1_ARM', index: 'ADDRESS1_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TELEPHONE_ARM', index: 'TELEPHONE_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'FAX_ARM', index: 'FAX_ARM', width: 80, align: "left", sortable: false },
            { key: false, name: 'EMAIL_ARM', index: 'EMAIL_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'CONDACTPERSON_ARM', index: 'CONDACTPERSON_ARM', width: 150, align: "left", sortable: false }
        ],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "Supplier Details",
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
        $('#tblSupplierDetails').setGridWidth(outerwidth);
    });
});