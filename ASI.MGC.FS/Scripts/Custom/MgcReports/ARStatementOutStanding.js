var accountSelect = function (accountId) {
    if (accountId) {
        var ret = jQuery("#tblAccountSearch").jqGrid('getRowData', accountId);
        $("#txtAccountCode").val(ret.AccountCode).change();
        $("#txtAccountDesc").val(ret.AccountDetail).change();
        $('#accountSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    $("#txtStartDate").datepicker();
    $("#txtEndDate").datepicker();
    var startDate = "";
    var endDate = "";
    var acCode = "";
    $('#txtAccountDesc').on('change', function () {
        $('#formArStatementOutStanding').formValidation('revalidateField', 'AccountDesc');
    });
    $("#accountSearchModel").on('show.bs.modal', function () {
        jQuery("#tblAccountSearch").jqGrid({
            url: '/AllocationMaster/GetAccountDetailsList?accountType=AR',
            datatype: "json",
            styleUI: "Bootstrap",
            autoheight: true,
            colNames: ['Account Code', 'Account Description', ''],
            colModel: [
                { key: true, name: 'AccountCode', index: 'AccountCode', width: 400 },
                { key: false, name: 'AccountDetail', index: 'AccountDetail', width: 400 },
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
                        "%Href%": "href=javascript:accountSelect(&apos;" + rowObject.AccountCode + "&apos;);"
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
            pager: jQuery('#accPager'),
            caption: "Allocation Master List",
            emptyrecords: "No Data to Display",
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false
            },
            autowidth: true,
            multiselect: false
        });
        $(window).resize(function () {
            var outerwidth = $('#accgrid').width();
            $('#tblAccountSearch').setGridWidth(outerwidth);
        });
    });
    $("#btnAccountSelect").on("click", function (e) {
        var id = jQuery("#tblAccountSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblAccountSearch").jqGrid('getRowData', id);
            $("#txtAccountCode").val(ret.AccountCode).change();
            $("#txtAccountDesc").val(ret.AccountDetail).change();
            $('#accountSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    var searchGrid = function (accountId, accountName) {
        var postData = $("#tblAccountSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = accountId;
        postData["searchByName"] = accountName;
        $("#tblAccountSearch").setGridParam({ postData: postData });
        $("#tblAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtArIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtArIdSearch").val().length >= 1 || $("#txtArIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtArIdSearch").val(), $("#txtArNameSearch").val());
        }
    });
    $("#txtArNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtArNameSearch").val().length >= 3 || $("#txtArNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtArIdSearch").val(), $("#txtArNameSearch").val());
        }
    });
    var validateArguments = function () {
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        acCode = $("#txtAccountCode").val();
        if (startDate === "" || endDate === "" || acCode === "") {
            return false;
        }
        return true;
    }

    $("#btnReportSubmit").on("click", function () {
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "/Reports/ARStatementOutstanding.aspx?startDate=" + startDate + "&endDate=" + endDate + "&acCode=" + acCode;
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });

    $('#formArStatementOutStanding').on('init.field.fv', function (e, data) {
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
            AccountDesc: {
                validators: {
                    notEmpty: {
                        message: 'Account Desc is required'
                    }
                }
            },
            StartDate: {
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