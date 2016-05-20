$(document).ready(function () {
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblGeneralLedger").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblGeneralLedger").setGridParam({ postData: postData });
        $("#tblGeneralLedger").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtglCodeSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtglCodeSearch").val().length >= 3 || $("#txtglCodeSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtglCodeSearch").val());
        }
    });

    $("#tblGeneralLedger").jqGrid({
        url: '/Finance/GetGlCodes',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['GL Code', 'GL Details'],
        colModel: [
        { key: true, name: 'GLCODE_LM', index: 'GLCODE_LM', width: 250 },
        { key: false, name: 'GLDESCRIPTION_LM', index: 'GLDESCRIPTION_LM', width: 500 }
        ],
        rowNum: 40,
        rowList: [40, 100, 500, 1000],
        mtype: 'GET',
        gridview: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "Genral Ledger List",
        emptyrecords: "No Data to Display",
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false
        },
        multiselect: false
    });
});