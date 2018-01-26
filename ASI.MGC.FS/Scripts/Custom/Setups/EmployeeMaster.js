$(document).ready(function () {
    $('#txtPassIssue').datepicker({
        changeMonth: true,
        changeYear: true
    });
    $('#txtPassExp').datepicker({
        changeMonth: true,
        changeYear: true
    });
    $('#txtVisaIssue').datepicker({
        changeMonth: true,
        changeYear: true
    });
    $('#txtVisaExp').datepicker({
        changeMonth: true,
        changeYear: true
    });
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
            { key: false, name: 'PASSPORTISSUEDATE_EM', index: 'PASSPORTISSUEDATE_EM', width: 80, formatter: 'date', align: "left", sortable: false },
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
    var searchGrid = function (searchById, searchByName) {
        postData = $("#tblEmpDetails").jqGrid("getGridParam", "postData");
        postData["searchById"] = searchById;
        postData["searchByName"] = searchByName;
        $("#tblEmpDetails").setGridParam({ postData: postData });
        $("#tblEmpDetails").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtEmpIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtEmpIdSearch").val().length >= 1 || $("#txtEmpIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtEmpIdSearch").val(), $("#txtEmpNameSearch").val());
        }
    });
    $("#txtEmpNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtEmpNameSearch").val().length >= 3 || $("#txtEmpNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtEmpIdSearch").val(), $("#txtEmpNameSearch").val());
        }
    });
    $("#EmployeeMasterModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });
    $('#formEmployeeMaster').on('init.field.fv', function (e, data) {
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
            EMPCODE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Emp Code is required'
                    }
                }
            },
            EMPFNAME_EM: {
                validators: {
                    notEmpty: {
                        message: 'Emp Name is required'
                    }
                }
            },
            PASSPORTISSUEDATE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Passport Issue Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            }, PASSPORTEXPDATE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Passport Expiry Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            }, VISAISSUEDATE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Visa Issue Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            }, VISAEXPIEARYDATE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Visa Expiry Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            EMPSNAME_EM: {
                validators: {
                    notEmpty: {
                        message: 'Emp Surname is required'
                    }
                }
            },
            PASSPORTNO_EM: {
                validators: {
                    notEmpty: {
                        message: 'Passport No is required'
                    }
                }
            },
            PHONE_EM: {
                validators: {
                    notEmpty: {
                        message: 'Telephone is required'
                    }
                }
            },
            DESIGNATION_EM: {
                validators: {
                    notEmpty: {
                        message: 'Emp Designation is required'
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