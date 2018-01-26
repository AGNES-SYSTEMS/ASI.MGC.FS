$(document).ready(function () {
    $("#txtNewFYFrom").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $("#txtNewFYTo").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $("#txtStartDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $("#txtEndDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $("#txtNewFYTo").change(function () {
        $('#formFinancialYear').formValidation('revalidateField', 'PERRIEDRTO_FM');
    });
    $("#txtNewFYFrom").change(function () {
        $('#formFinancialYear').formValidation('revalidateField', 'PERRIEDFROM_FM');
    });
    jQuery("#tblFinancialYearDetails").jqGrid({
        url: '/Setups/GetAllFinancialYears',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Financial Year From', 'Financial Year To', 'Current Financial Year', 'Edit Actions'
        ],
        colModel: [
            { key: true, name: 'PERRIEDFROM_FM', index: 'PERRIEDFROM_FM', width: 80, align: "center", formatter: 'date', sortable: false },
            { key: false, name: 'PERRIEDRTO_FM', index: 'PERRIEDRTO_FM', width: 150, align: "center", formatter: 'date', sortable: false },
            { key: false, name: 'CURRENTPERIOD_FM', index: 'CURRENTPERIOD_FM', width: 80, align: "center", formatter: "checkbox", sortable: false },
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
        caption: "Financial Year Details",
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
        $('#tblFinancialYearDetails').setGridWidth(outerwidth);
    });
    var searchGrid = function (searchById, searchByName) {
        postData = $("#tblEmpDetails").jqGrid("getGridParam", "postData");
        postData["searchById"] = searchById;
        postData["searchByName"] = searchByName;
        $("#tblEmpDetails").setGridParam({ postData: postData });
        $("#tblEmpDetails").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtStartDate").off().on("change", function () {
        searchGrid($("#txtStartDate").val(), $("#txtEndDate").val());
    });
    $("#txtEndDate").off().on("change", function () {
        searchGrid($("#txtStartDate").val(), $("#txtEndDate").val());
    });
    $("#FinancialYearModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });
    $('#formFinancialYear').on('init.field.fv', function (e, data) {
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
            PERRIEDFROM_FM: {
                validators: {
                    notEmpty: {
                        message: 'New FY From is required'
                    }
                }
            },
            PERRIEDRTO_FM: {
                validators: {
                    notEmpty: {
                        message: 'New FY To is required'
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