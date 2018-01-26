var bankSelect = function (bankId) {
    if (bankId) {
        var ret = jQuery("#tblBankSearch").jqGrid('getRowData', bankId);
        $("#txtBankCode").val(ret.BANKCODE_BM).change();
        $("#txtBankName").val(ret.BANKNAME_BM).change();
        $('#BankSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    $("#txtStartDate").datepicker({changeMonth: true,changeYear: true});
    $("#txtEndDate").datepicker({changeMonth: true,changeYear: true});
    var startDate = "";
    var endDate = "";
    var bankCode = "";
    $("#txtBankName").on("change", function () {
        $("#formBankStatement").formValidation('revalidateField', 'BankName');
    });
    $("#BankSearchModel").on('show.bs.modal', function () {
        $("#tblBankSearch").jqGrid({
            url: '/Bank/GetBankDetailsByType',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Bank Code', 'Bank Name', ''],
            colModel: [
            { key: true, name: 'BANKCODE_BM', index: 'BANKCODE_BM', width: 400 },
            { key: false, name: 'BANKNAME_BM', index: 'BANKNAME_BM', width: 400 },
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
                        "%Href%": "href=javascript:bankSelect(&apos;" + rowObject.BANKCODE_BM + "&apos;);"
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
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#bankPager'),
            caption: "Bank Details List",
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
        $(window).resize(function () {
            var outerwidth = $('#bankGrid').width();
            $('#tblBankSearch').setGridWidth(outerwidth);
        });
    });
    var validateArguments = function () {
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        bankCode = $("#txtBankCode").val();
        if (startDate === "" || endDate === "" || bankCode === "") {
            return false;
        }
        return true;
    }
    var searchGrid = function (bnkId, bnkName) {
        var postData = $("#tblBankSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = bnkId;
        postData["searchByName"] = bnkName;
        $("#tblBankSearch").setGridParam({ postData: postData });
        $("#tblBankSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtIdSearch").val().length >= 1 || $("#txtIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtIdSearch").val(), $("#txtEmpNameSearch").val());
        }
    });
    $("#txtNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtNameSearch").val().length >= 3 || $("#txtNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtIdSearch").val(), $("#txtNameSearch").val());
        }
    });
    $("#btnReportSubmit").on("click", function () {
        $("#btnReportSubmit").prop('disabled', false);
        $("#btnReportSubmit").removeAttr('disabled');
        $("#btnReportSubmit").removeClass('disabled');
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "/Reports/BankStatement.aspx?startDate=" + startDate + "&endDate=" + endDate + "&bankCode=" + bankCode;
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });

    $('#formBankStatement').on('init.field.fv', function (e, data) {
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
            BankName: {
                validators: {
                    notEmpty: {
                        message: 'Bank Name is required'
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