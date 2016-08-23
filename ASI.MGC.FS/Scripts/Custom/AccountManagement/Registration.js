var employeeSelect = function (empId) {
    if (empId) {
        var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', empId);
        $("#txtUserName").val(ret.EMPCODE_EM).change();
        $("#txtFirstName").val(ret.EMPFNAME_EM).change();
        $("#txtLastName").val(ret.EMPSNAME_EM).change();
        $('#EmployeeSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    $("#EmployeeSearchModel").on('show.bs.modal', function () {
        $("#tblEmployeeSearch").jqGrid({
            url: '/EmployeeMaster/GetEmployeeDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Employee Code', 'Employee First Name', 'Employee Last Name', ''],
            colModel: [
            { key: true, name: 'EMPCODE_EM', index: 'EMPCODE_EM', width: 200 },
            { key: false, name: 'EMPFNAME_EM', index: 'EMPFNAME_EM', width: 300 },
            { key: false, name: 'EMPSNAME_EM', index: 'EMPSNAME_EM', width: 300 },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options, rowObject) {

                    var markup = "<a %Href%> <i class='fa fa-check-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:employeeSelect(&apos;" + rowObject.EMPCODE_EM + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
            ],
            rowNum: 40,
            rowList: [40, 100, 500, 1000],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#empPager'),
            caption: "Employee Details List",
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
    $(window).resize(function () {
        var outerwidth = $('#empGrid').width();
        $('#tblEmployeeSearch').setGridWidth(outerwidth);
    });
    var searchGrid = function (searchById, searchByName) {
        var postData = $("#tblEmployeeSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = searchById;
        postData["searchByName"] = searchByName;
        $("#tblEmployeeSearch").setGridParam({ postData: postData });
        $("#tblEmployeeSearch").trigger("reloadGrid", [{ page: 1 }]);
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
    $("#btnEmpSelect").on("click", function (e) {
        var id = jQuery("#tblEmployeeSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', id);
            $("#txtUserName").val(ret.EMPCODE_EM).change();
            $("#txtFirstName").val(ret.EMPFNAME_EM).change();
            $("#txtLastName").val(ret.EMPSNAME_EM).change();
            $('#EmployeeSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $('#txtUserName').on('change', function () {
        $('#formRegistration').formValidation('revalidateField', 'UserName');
    });
    $('#txtFirstName').on('change', function () {
        $('#formRegistration').formValidation('revalidateField', 'FirstName');
    });
    $('#txtLastName').on('change', function () {
        $('#formRegistration').formValidation('revalidateField', 'LastName');
    });
    $('#formRegistration').on('init.field.fv', function (e, data) {
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.addClass(options.icon.required).show();
        }
    }).formValidation({
        framework: 'bootstrap',
        icon: {
            required: 'fa fa-asterisk',
            valid: 'fa fa-check',
            invalid: 'fa fa-times',
            validating: 'fa fa-refresh'
        },
        fields: {
            'FirstName': {
                validators: {
                    notEmpty: {
                        message: 'The first name is required'
                    }
                }
            },
            'LastName': {
                validators: {
                    notEmpty: {
                        message: 'The last name is required'
                    }
                }
            },
            'UserName': {
                validators: {
                    notEmpty: {
                        message: 'The username is required'
                    },
                    //stringLength: {
                    //    min: 8,
                    //    max: 30,
                    //    message: 'The username must be more than 6 and less than 30 characters long'
                    //},
                    regexp: {
                        regexp: /^[a-zA-Z0-9_\.]+$/,
                        message: 'The username can only consist of alphabetical, number, dot and underscore'
                    }
                }
            },
            'Email': {
                validators: {
                    notEmpty: {
                        message: 'The email address is required'
                    },
                    emailAddress: {
                        message: 'The input is not a valid email address'
                    }
                }
            },
            Password: {
                validators: {
                    notEmpty: {
                        message: 'Password is required'
                    },
                    identical: {
                        field: 'ConfirmPassword',
                        message: 'The password and its confirm are not the same'
                    }
                }
            },
            ConfirmPassword: {
                validators: {
                    notEmpty: {
                        message: 'Confirm Password is required'
                    },
                    identical: {
                        field: 'Password',
                        message: 'The password and its confirm are not the same'
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