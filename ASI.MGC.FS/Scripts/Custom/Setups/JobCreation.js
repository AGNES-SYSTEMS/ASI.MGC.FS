$(document).ready(function () {
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
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblJobDetails").jqGrid("getGridParam", "postData");
        postData["jobSearch"] = searchValue;

        $("#tblJobDetails").setGridParam({ postData: postData });
        $("#tblJobDetails").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtJobPrd").off().on("keyup", function () {

        var shouldSearch = $("#txtJobPrd").val().length >= 3 || $("#txtJobPrd").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtJobPrd").val());
        }
    });

    $("#JobDetailsModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });

    $('#formJobCreation').on('init.field.fv', function (e, data) {
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
            JOBID_JR: {
                validators: {
                    notEmpty: {
                        message: 'Job Code is required'
                    }
                }
            },
            JOBDESCRIPTION_JR: {
                validators: {
                    notEmpty: {
                        message: 'Description is required'
                    }
                }
            },
            RATE_RJ: {
                validators: {
                    notEmpty: {
                        message: 'Rate is required'
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