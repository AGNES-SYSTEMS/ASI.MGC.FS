$(document).ready(function () {
    jQuery("#tblUnitDetails").jqGrid({
        url: '/Product/GetAllUnitsList',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Unit Code', 'Unit Detail', 'Unit Type', 'Quantity', 'Basic Primary Unit',
        'Basic Primary Qty', 'Edit Actions'],
        colModel: [
            { key: true, name: 'UNIT_UM', index: 'UNIT_UM', width: 80, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_UM', index: 'DESCRIPTION_UM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TYPE_UM', index: 'TYPE_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'UNITQTY_UM', index: 'UNITQTY_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'BASICPRIMARYUNIT_UM', index: 'BASICPRIMARYUNIT_UM', width: 80, align: "left", sortable: false },
            { key: false, name: 'BASICPRIMARYQTY_UM', index: 'BASICPRIMARYQTY_UM', width: 80, align: "left", sortable: false },
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
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblUnitDetails").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblUnitDetails").setGridParam({ postData: postData });
        $("#tblUnitDetails").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtUnitSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtUnitSearch").val().length >= 3 || $("#txtUnitSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtUnitSearch").val());
        }
    });

    $("#UnitMeasureModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });

    $('#formUnitMeasure').on('init.field.fv', function (e, data) {
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
            UNIT_UM: {
                validators: {
                    notEmpty: {
                        message: 'Unit is required'
                    }
                }
            },
            DESCRIPTION_UM: {
                validators: {
                    notEmpty: {
                        message: 'Description is required'
                    }
                }
            },
            UNITQTY_UM: {
                validators: {
                    notEmpty: {
                        message: 'Unit Qty is required'
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
    });
});