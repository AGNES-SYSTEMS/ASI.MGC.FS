var jobSelect = function (jobNo) {
    if (jobNo) {
        var ret = jQuery("#tblJobSearch").jqGrid('getRowData', jobNo);
        $("#txtJobNo").val(ret.JOBNO_JM);
        getJobDetail();
        $('#jobSearchModel').modal('toggle');
    }
};
var employeeSelect = function (empId) {
    if (empId) {
        var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', empId);
        $("#txtEmpCode").val(ret.EMPCODE_EM).change();
        $("#txtEmpName").val(ret.EMPFNAME_EM).change();
        $('#formJobCancellation').formValidation('revalidateField', 'EmpName');
        $('#EmployeeSearchModel').modal('toggle');
    }
};
function getJobDetail() {
    var jobCode = $("#txtJobNo").val();
    var data = JSON.stringify({ jobNo: jobCode });
    $.ajax({
        url: '/Job/getPendingJobDetails',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (jobDetails) {
            if (jobDetails !== null) {
                var date = formattedDate(jobDetails.docDate);
                $("#txtDocDate").val(date);
                $("#txtMRVNo").val(jobDetails.mrvNo);
                $("#txtProdCode").val(jobDetails.prodCode);
                $("#txtMRVProdDetail").val(jobDetails.prodName);
                $("#txtEmpCode").val(jobDetails.empCode);
                $("#txtEmpName").val(jobDetails.empName);
                $("#txtCustCode").val(jobDetails.custCode);
                $("#txtCustName").val(jobDetails.custName);
                $("#txtRemarks").val(jobDetails.details);
            }
        },
        complete: function () {
            $('#formJobCancellation').formValidation('revalidateField', 'DOCDATE_JM');
            $('#formJobCancellation').formValidation('revalidateField', 'MRVNO_JM');
            $('#formJobCancellation').formValidation('revalidateField', 'CustCode');
            $('#formJobCancellation').formValidation('revalidateField', 'CustName');
            $('#formJobCancellation').formValidation('revalidateField', 'PRODID_JIM');
            $('#formJobCancellation').formValidation('revalidateField', 'MRVProdDetail');
            $('#formJobCancellation').formValidation('revalidateField', 'EmpName');
        },
        error: function () {
        }
    });
};
function formattedDate(jsonDate) {
    var dateString = jsonDate.substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    return date = month + "/" + day + "/" + year;
};
$(document).ready(function () {
    $("#jobSearchModel").on('show.bs.modal', function () {
        jQuery("#tblJobSearch").jqGrid({
            url: '/Job/GetJobData',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Job No', 'Mrv No', ''],
            colModel: [
            { key: true, name: 'JOBNO_JM', index: 'JOBNO_JM', width: 400 },
            { key: false, name: 'MRVNO_JM', index: 'MRVNO_JM', width: 400 },
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
                        "%Href%": "href=javascript:jobSelect(&apos;" + rowObject.JOBNO_JM + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
            ],
            rowNum: 100,
            rowList: [100, 200, 500, 1000],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#Pager'),
            caption: "Pending Jobs List",
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
        var outerwidth = $('#jobGrid').width();
        $('#tblJobSearch').setGridWidth(outerwidth);
    });
    var searchGrid = function (jobNo) {
        var postData = $("#tblJobSearch").jqGrid("getGridParam", "postData");
        postData["jobNo"] = jobNo;
        $("#tblJobSearch").setGridParam({ postData: postData });
        $("#tblJobSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtJobSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtJobSearch").val().length >= 1 || $("#txtJobSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtJobSearch").val());
        }
    });
    $("#btnJobSelect").click(function (e) {
        var id = jQuery("#tblJobSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblJobSearch").jqGrid('getRowData', id);
            $("#txtJobNo").val(ret.JOBNO_JM);
            getJobDetail();
            $('#jobSearchModel').modal('toggle');
        }
        e.preventDefault();
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
    $("#btnNew").on("click", function () {
        location.reload();
    });
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
    $('#formJobCancellation').on('init.field.fv', function (e, data) {
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
            DOCDATE_JM: {
                validators: {
                    notEmpty: {
                        message: 'Doc Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            MRVNO_JM: {
                validators: {
                    notEmpty: {
                        message: 'Mrv No is required'
                    }
                }
            },
            CustCode: {
                validators: {
                    notEmpty: {
                        message: 'Cust Code is required'
                    }
                }
            },
            CustName: {
                validators: {
                    notEmpty: {
                        message: 'Cust Name is required'
                    }
                }
            },
            PRODID_JIM: {
                validators: {
                    notEmpty: {
                        message: 'Prod Id is required'
                    }
                }
            },
            MRVProdDetail: {
                validators: {
                    notEmpty: {
                        message: 'Prod Details is required'
                    }
                }
            },
            EmpName: {
                validators: {
                    notEmpty: {
                        message: 'Employee Name is required'
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