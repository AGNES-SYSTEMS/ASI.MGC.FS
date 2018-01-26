$(document).ready(function () {
    var $status = "SP";
    jQuery("#tblItemDetails").jqGrid({
        url: '/Product/GetProdsList?status=' + $status,
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Item Code', 'Item Name', 'Item Status', 'Edit Actions'],
        colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 100, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 150, align: "left", sortable: false },
            { key: false, name: 'STATUS_PM', index: 'STATUS_PM', width: 80, align: "center", sortable: false },
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
        caption: "Item Details",
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
        $('#tblItemDetails').setGridWidth(outerwidth);
    });
    var searchGrid = function (searchById, searchByName) {
        var postData = $("#tblItemDetails").jqGrid("getGridParam", "postData");
        postData["prdCode"] = searchById;
        postData["prdName"] = searchByName;
        $("#tblItemDetails").setGridParam({ postData: postData });
        $("#tblItemDetails").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtPrdIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPrdIdSearch").val().length >= 1 || $("#txtPrdIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPrdIdSearch").val(), $("#txtPrdNameSearch").val());
        }
    });
    $("#txtPrdNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPrdNameSearch").val().length >= 3 || $("#txtPrdNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPrdIdSearch").val(), $("#txtPrdNameSearch").val());
        }
    });
    $("#ItemMasterModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });
    $('#formItemMaster').on('init.field.fv', function (e, data) {
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.addClass(options.icon.required).show();
        }
    }).formValidation({
        container: '#messages',
        framework: 'bootstrap',
        icon: {
            required: 'fa fa-asterisk',
            valid: 'fa fa-check',
            invalid: 'fa fa-times',
            validating: 'fa fa-refresh'
        },
        fields: {
            PROD_CODE_PM: {
                validators: {
                    notEmpty: {
                        message: 'Product Code is required'
                    }
                }
            },
            DESCRIPTION_PM: {
                validators: {
                    notEmpty: {
                        message: 'Description is required'
                    }
                }
            }
        }
    }).on('status.field.fv', function (e, data) {
        // Remove the required icon when the field updates its status
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),                      // Entire options
            validators = data.fv.getOptions(data.field).validators; // The field validators

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.removeClass(options.icon.required).addClass('fa');
        }
    }).on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0) {    // There is invalid field
            data.fv.disableSubmitButtons(true);
        }
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
        $("#divSaving").show();
    });
});