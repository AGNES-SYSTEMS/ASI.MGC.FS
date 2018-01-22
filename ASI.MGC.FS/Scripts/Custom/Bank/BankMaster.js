var bankCodeSelect = function (bankCode) {
    if (bankCode) {
        var ret = jQuery("#tblBankSearch").jqGrid('getRowData', bankCode);
        $("#txtBankCode").val(ret.BANKCODE_BM).change();
        $("#txtBankName").val(ret.BANKNAME_BM).change();
        $('#BankSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    debugger;
    $('#txtBankDate').datepicker();

    function getBankCodeDetails() {
        var bankCode = $("#txtBankCode").val();
        if (bankCode !== "") {
            var data = JSON.stringify({ bankCode: bankCode });
            $.ajax({
                url: '/Bank/GetBankCodeDetails',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (bankDetails) {
                    debugger;
                    $("#txtBankCode").val(bankDetails.bankCode).change();
                    $("#txtBankName").val(bankDetails.bankDesc).change();
                    $("#txtAccNo").val(bankDetails.accountNo).change();
                    $("#txtODLimit").val(parseInt(bankDetails.ODLimit)).change();
                    $("#ddlMode").val(parseInt(bankDetails.Mode)).change();
                    $("#txtOpenBalance").val(bankDetails.debitAmount).change();
                    $("#txtNote").val(bankDetails.Notes).change();
                    $("#txtBankDate").val(bankDetails.bankDate).change();
                    $("#ddlBankStatus").val(parseInt(bankDetails.bankStatus)).change();
                },
                complete: function () {
                },
                error: function () {
                }
            });
        }
    }

    $("#btnBankSelect").on("click", function () {
        var id = jQuery("#tblBankSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblBankSearch").jqGrid('getRowData', id);
            $("#txtBankCode").val(ret.BANKCODE_BM).change();
            $("#txtBankName").val(ret.BANKNAME_BM).change();
            $('#BankSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblBankSearch").jqGrid("getGridParam", "postData");
        postData["bankName"] = searchValue;

        $("#tblBankSearch").setGridParam({ postData: postData });
        $("#tblBankSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $("#txtBankSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtBankSearch").val().length >= 3 || $("#txtBankSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtBankSearch").val());
        }
    });

    $("#BankSearchModel").on('show.bs.modal', function () {
        $("#tblBankSearch").jqGrid({
            url: '/Bank/GetBankDetailsList',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Bank Code', 'Bank Name', ''],
            colModel: [
            { key: true, name: 'BANKCODE_BM', index: 'BANKCODE_BM', width: 250 },
            { key: false, name: 'BANKNAME_BM', index: 'BANKNAME_BM', width: 500 },
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
                        "%Href%": "href=javascript:bankCodeSelect(&apos;" + rowObject.BANKCODE_BM + "&apos;);"
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
            pager: jQuery('#Pager'),
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
    });
    $("#BankSearchModel").on('hide.bs.modal', function () {
        getBankCodeDetails();
    });
    $("#txtBankDate").on("change", function () {
        $("#formBankMaster").formValidation('revalidateField', 'BankDate');
    });
    $('#formBankMaster').on('init.field.fv', function (e, data) {
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
            BANKCODE_BM: {
                validators: {
                    notEmpty: {
                        message: 'Bank Code is required'
                    }
                }
            },
            BANKNAME_BM: {
                validators: {
                    notEmpty: {
                        message: 'Bank Name is required'
                    }
                }
            },
            ACCOUNTNUMBER_BM: {
                validators: {
                    notEmpty: {
                        message: 'Account No is required'
                    }
                }
            },
            BankDate: {
                validators: {
                    notEmpty: {
                        message: 'GL Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            OpenBalance: {
                validators: {
                    notEmpty: {
                        message: 'Open Balance is required'
                    }, integer: {
                        message: 'Integer Only'
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
        $("#btnSave").hide();
    });
});