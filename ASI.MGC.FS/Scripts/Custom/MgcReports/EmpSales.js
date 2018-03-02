var employeeSelect = function (empId) {
    if (empId) {
        var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', empId);
        $("#txtEmpCode").val(ret.EMPCODE_EM).change();
        $("#txtEmpName").val(ret.EMPFNAME_EM).change();
        $('#EmployeeSearchModel').modal('toggle');
    }
}
$(document).ready(function () {
    $("#txtStartDate").datepicker({ changeMonth: true, changeYear: true });
    $("#txtEndDate").datepicker({ changeMonth: true, changeYear: true });
    var startDate = "";
    var endDate = "";
    var empCode = "";
    $('#txtEmpName').on('change', function () {
        $('#formEmpSales').formValidation('revalidateField', 'EmpName');
    });
    $('#txtStartDate').on('change', function () {
        $('#formEmpSales').formValidation('revalidateField', 'startDate');
    });
    $('#txtEndDate').on('change', function () {
        $('#formEmpSales').formValidation('revalidateField', 'endDate');
    });
    $("#EmployeeSearchModel").on('show.bs.modal', function () {
        $("#tblEmployeeSearch").jqGrid({
            url: '/EmployeeMaster/GetEmployeeDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Employee Code', 'Employee Name', ''],
            colModel: [
            { key: true, name: 'EMPCODE_EM', index: 'EMPCODE_EM', width: 400 },
            { key: false, name: 'EMPFNAME_EM', index: 'EMPFNAME_EM', width: 400 },
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
    var validateArguments = function () {
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        empCode = $("#txtEmpCode").val();
        if (startDate === "" || endDate === "" || empCode === "") {
            return false;
        }
        return true;
    }
    $("#btnEmpSelect").on("click", function (e) {
        var id = jQuery("#tblEmployeeSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', id);
            $("#txtEmpCode").val(ret.EMPCODE_EM).change();
            $("#txtEmpName").val(ret.EMPFNAME_EM).change();
            $('#EmployeeSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#btnReportSubmit").on("click", function () {
        $("#btnReportSubmit").prop('disabled', false);
        $("#btnReportSubmit").removeAttr('disabled');
        $("#btnReportSubmit").removeClass('disabled');
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "";
            var isExportMode = getQueryStringByName("isExportMode", document.location.href);
            if (isExportMode !== null && isExportMode !== "") {
                url = "/Reports/EmpSales.aspx?startDate=" + startDate + "&endDate=" + endDate + "&empCode=" + empCode + "&isExportMode=" + isExportMode;
            } else {
                url = "/Reports/EmpSales.aspx?startDate=" + startDate + "&endDate=" + endDate + "&empCode=" + empCode;
            }
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });
    var searchGrid = function (empId, empName) {
        var postData = $("#tblEmployeeSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = empId;
        postData["searchByName"] = empName;
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
    $('#iframe').on('load', function () {
        $('#loader').hide();
    });

    $('#formEmpSales').on('init.field.fv', function (e, data) {
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.addClass(options.icon.required).show();
        }
    }).formValidation({
        container: '#messages',
        icon: {
            required: 'fa fa-asterisk',
            valid: 'fa fa-check',
            invalid: 'fa fa-times',
            validating: 'fa fa-refresh'
        },
        fields: {
            EmpName: {
                validators: {
                    notEmpty: {
                        message: 'Employee Name is required'
                    }
                }
            }, StartDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
                    }
                }
            },
            EndDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
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