$(document).ready(function () {
    jQuery("#tblMachineDetails").jqGrid({
        url: '/Setups/GetMesMachinesList',
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'ID', 'Machine Name', 'MAC Address', 'Active', 'Edit Actions'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', width: 100, align: "center", sortable: false },
            { key: false, name: 'MachineName', index: 'MachineName', width: 100, align: "center", sortable: false },
            { key: false, name: 'MacAddress', index: 'MacAddress', width: 150, align: "left", sortable: false },
            { key: false, name: 'IsActive', index: 'IsActive', width: 80, align: "center", sortable: false },
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
        sortorder: "asc",
        pager: jQuery('#Pager'),
        caption: "MES Machine Details",
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
        $('#tblMachineDetails').setGridWidth(outerwidth);
    });
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblMachineDetails").jqGrid("getGridParam", "postData");
        postData["machineSearch"] = searchValue;

        $("#tblMachineDetails").setGridParam({ postData: postData });
        $("#tblMachineDetails").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtMachine").off().on("keyup", function () {

        var shouldSearch = $("#txtMachine").val().length >= 3 || $("#txtMachine").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtMachine").val());
        }
    });

    $("#MachineDetailsModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });

    $('#formMachineCreation').on('init.field.fv', function (e, data) {
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
            MachineName: {
                validators: {
                    notEmpty: {
                        message: 'Machine Name is required'
                    }
                }
            },
            MacAddress: {
                validators: {
                    notEmpty: {
                        message: 'Mac Address is required'
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