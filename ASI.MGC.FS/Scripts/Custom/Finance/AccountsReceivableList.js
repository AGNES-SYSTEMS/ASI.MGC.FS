$(document).ready(function () {

    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblAccountsReceivable").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblAccountsReceivable").setGridParam({ postData: postData });
        $("#tblAccountsReceivable").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtAccountsReceivable").off().on("keyup", function () {

        var shouldSearch = $("#txtAccountsReceivable").val().length >= 3 || $("#txtAccountsReceivable").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtAccountsReceivable").val());
        }
    });

    var $custType = "AR";
    jQuery("#tblAccountsReceivable").jqGrid({
        url: '/Customer/GetAllCustomers?custType=' + $custType,
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Customer Code', 'Customer Name', 'PO Box', 'Address', 'Telephone', 'Fax', 'Email', 'Contact Person'],
        colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 100, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'POBOX_ARM', index: 'POBOX_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'ADDRESS1_ARM', index: 'ADDRESS1_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TELEPHONE_ARM', index: 'TELEPHONE_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'FAX_ARM', index: 'FAX_ARM', width: 80, align: "left", sortable: false },
            { key: false, name: 'EMAIL_ARM', index: 'EMAIL_ARM', width: 120, align: "center", sortable: false },
            { key: false, name: 'CONDACTPERSON_ARM', index: 'CONDACTPERSON_ARM', width: 150, align: "left", sortable: false }
        ],
        rowNum: 40,
        rowList: [40, 100, 500, 1000],
        mtype: 'GET',
        gridview: true,
        viewrecords: true,
        sortorder: "desc",
        pager: jQuery('#Pager'),
        caption: "Accounts Receivable",
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
        $('#tblAccountsReceivable').setGridWidth(outerwidth);
    });
});