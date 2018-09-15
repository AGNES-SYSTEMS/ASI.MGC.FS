$(document).ready(function () {
    jQuery("#tblMrvSearchDetails").jqGrid({
        datatype: "local",
        height: 175,
        width: 1080,
        rowNum: 1000,
        shrinkToFit: false,
        styleUI: "Bootstrap",
        colNames: ['MRV No', 'MRV Date', 'Job No', 'Product Code', 'SOW Code', 'QTY', 'Rate', 'Amount', 'Discount', 'Shipping Charges', 'Sales Date', 'User Id', 'Cash Total', 'Credit Total', 'CashRV No', 'INV No', 'Credit AcCode', 'Lpo No', 'DayEndDocNo'],
        colModel: [
            {
                name: 'MRVNO_SD', index: 'MRVNO_SD', width: 110, align: "center", sortable: false, formatter: function (cellValue, options) {
                    if (cellValue !== null && cellValue !== undefined) {
                        var markup = "<a %Href% target='_blank'>" + cellValue + "</a>";
                        var replacements = {
                            "%Href%": "href=/MRV/DuplicateMrvPrinting?code=" + cellValue
                        };
                        markup = markup.replace(/%\w+%/g, function (all) {
                            return replacements[all];
                        });
                        return markup;
                    } else {
                        return "";
                    }
                }
            },
            { name: 'MRVDate', index: 'MRVDate', width: 110, align: "center", sortable: false, formatter: 'date', formatoptions: { newformat: 'm/d/Y' } },
            { name: 'JOBNO_SD', index: 'JOBNO_SD', width: 110, align: "center", sortable: false },
            { name: 'PRCODE_SD', index: 'PRCODE_SD', width: 110, align: "center", sortable: false },
            { name: 'JOBID_SD', index: 'JOBID_SD', width: 100, align: "center", sortable: false },
            { name: 'QTY_SD', index: 'QTY_SD', width: 50, align: "center", sortable: false },
            { name: 'RATE_SD', index: 'RATE_SD', width: 80, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 80, align: "center", sortable: false },
            { name: 'DISCOUNT_SD', index: 'DISCOUNT_SD', width: 80, align: "center", sortable: false },
            { name: 'SHIPPINGCHARGES_SD', index: 'SHIPPINGCHARGES_SD', width: 80, align: "center", sortable: false },
            { name: 'SALEDATE_SD', index: 'SALEDATE_SD', width: 150, align: "center", sortable: false, formatter: 'date', formatoptions: { newformat: 'm/d/Y' } },
            { name: 'USERID_SD', index: 'USERID_SD', width: 50, align: "center", sortable: false },
            { name: 'CASHTOTAL_SD', index: 'CASHTOTAL_SD', width: 80, align: "center", sortable: false },
            { name: 'CREDITTOTAL_SD', index: 'CREDITTOTAL_SD', width: 80, align: "center", sortable: false },
            {
                name: 'CASHRVNO_SD', index: 'CASHRVNO_SD', width: 120, align: "center", sortable: false, formatter: function (cellValue, options) {
                    if (cellValue !== null && cellValue !== undefined) {
                        var markup = "<a %Href% target='_blank'>" + cellValue + "</a>";
                        var replacements = {
                            "%Href%": "href=/MRV/DuplicateMrvPrinting?code=" + cellValue
                        };
                        markup = markup.replace(/%\w+%/g, function (all) {
                            return replacements[all];
                        });
                        return markup;
                    } else {
                        return "";
                    }
                }
            },
            {
                name: 'INVNO_SD', index: 'INVNO_SD', width: 120, align: "center", sortable: false, formatter: function (cellValue, options) {
                    if (cellValue !== null && cellValue !== undefined) {
                        var markup = "<a %Href% target='_blank'>" + cellValue + "</a>";
                        var replacements = {
                            "%Href%": "href=/MRV/DuplicateMrvPrinting?code=" + cellValue
                        };
                        markup = markup.replace(/%\w+%/g, function (all) {
                            return replacements[all];
                        });
                        return markup;
                    } else {
                        return "";
                    }
                }
            },
            { name: 'CREDITACCODE_SD', index: 'CREDITACCODE_SD', width: 100, align: "center", sortable: false },
            { name: 'LPONO_SD', index: 'LPONO_SD', width: 100, align: "center", sortable: false },
            { name: 'DAYENDDOC_NO', index: 'DAYENDDOC_NO', width: 100, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "Materials Details"
    });
    $("#btnJobNoSearch").click(function (e) {
        $("#tblMrvSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
        var jobNo = $('#txtJobNo').val();
        if (jobNo !== "" && jobNo !== undefined) {
            var data = JSON.stringify({ jobNo: jobNo });
            $.ajax({
                url: '/Job/JobSearchDetails',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (jobDetails) {
                    if (jobDetails.searchHeader !== null && jobDetails.searchHeader !== undefined) {
                        $('#txtJobNo').val(jobDetails.searchHeader.JobNumber);
                        $('#txtMrvNo').val(jobDetails.searchHeader.MrvNumber);
                        $('#txtCustCode').val(jobDetails.searchHeader.CustomerCode);
                        $('#txtCustName').val(jobDetails.searchHeader.CustomerName);
                        $('#txtAddress').val(jobDetails.searchHeader.Address);
                        $('#txtTelephoneNo').val(jobDetails.searchHeader.Telephone);
                        $('#txtEmployee').val(jobDetails.searchHeader.Employee);
                    }
                    if (jobDetails.searchResult.length > 0) {
                        for (var i = 0; i <= jobDetails.searchResult.length; i++)
                            $("#tblMrvSearchDetails").jqGrid('addRowData', i + 1, jobDetails.searchResult[i]);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        } else {
            toastr.error("Invalid Job Number.");
        }
    });
    $("#btnMrvNoSearch").on("click", function (e) {
        $("#tblMrvSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
        var mrvNo = $('#txtMrvNo').val();
        if (mrvNo !== "" && mrvNo !== undefined) {
            var data = JSON.stringify({ mrvNo: mrvNo });
            $.ajax({
                url: '/MRV/MRVSearchDetails',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (mrvDetails) {
                    if (mrvDetails.searchHeader !== null && mrvDetails.searchHeader !== undefined) {
                        $('#txtJobNo').val(mrvDetails.searchHeader.JobNumber);
                        $('#txtMrvNo').val(mrvDetails.searchHeader.MrvNumber);
                        $('#txtCustCode').val(mrvDetails.searchHeader.CustomerCode);
                        $('#txtCustName').val(mrvDetails.searchHeader.CustomerName);
                        $('#txtAddress').val(mrvDetails.searchHeader.Address);
                        $('#txtTelephoneNo').val(mrvDetails.searchHeader.Telephone);
                        $('#txtEmployee').val(mrvDetails.searchHeader.Employee);
                    }
                    if (mrvDetails.searchResult.length > 0) {
                        for (var i = 0; i <= mrvDetails.searchResult.length; i++)
                            $("#tblMrvSearchDetails").jqGrid('addRowData', i + 1, mrvDetails.searchResult[i]);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        } else {
            toastr.error("Invalid MRV Number.");
        };
    });
    $("#btnCustCodeSearch").on("click", function (e) {
        $("#tblMrvSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
        var searchParam = $('#txtCustCode').val();
        if (searchParam !== "" && searchParam !== undefined) {
            var data = JSON.stringify({ type: 1, searchParam: searchParam });
            $.ajax({
                url: '/Customer/SearchDetailsByCustomer',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (SearchDetails) {
                    if (SearchDetails.searchHeader !== null && SearchDetails.searchHeader !== undefined) {
                        $('#txtJobNo').val(SearchDetails.searchHeader.JobNumber);
                        $('#txtMrvNo').val(SearchDetails.searchHeader.MrvNumber);
                        $('#txtCustCode').val(SearchDetails.searchHeader.CustomerCode);
                        $('#txtCustName').val(SearchDetails.searchHeader.CustomerName);
                        $('#txtAddress').val(SearchDetails.searchHeader.Address);
                        $('#txtTelephoneNo').val(SearchDetails.searchHeader.Telephone);
                        $('#txtEmployee').val(SearchDetails.searchHeader.Employee);
                    }
                    if (SearchDetails.searchResult.length > 0) {
                        for (var i = 0; i <= SearchDetails.searchResult.length; i++)
                            $("#tblMrvSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        } else {
            toastr.error("Invalid Customer Code.");
        };
    });
    $("#btnCustNameSearch").on("click", function (e) {
        $("#tblMrvSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
        var searchParam = $('#txtCustName').val();
        if (searchParam !== "" && searchParam !== undefined) {
            var data = JSON.stringify({ type: 2, searchParam: searchParam });
            $.ajax({
                url: '/Customer/SearchDetailsByCustomer',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (SearchDetails) {
                    if (SearchDetails.searchHeader !== null && SearchDetails.searchHeader !== undefined) {
                        $('#txtJobNo').val(SearchDetails.searchHeader.JobNumber);
                        $('#txtMrvNo').val(SearchDetails.searchHeader.MrvNumber);
                        $('#txtCustCode').val(SearchDetails.searchHeader.CustomerCode);
                        $('#txtCustName').val(SearchDetails.searchHeader.CustomerName);
                        $('#txtAddress').val(SearchDetails.searchHeader.Address);
                        $('#txtTelephoneNo').val(SearchDetails.searchHeader.Telephone);
                        $('#txtEmployee').val(SearchDetails.searchHeader.Employee);
                    }
                    if (SearchDetails.searchResult.length > 0) {
                        for (var i = 0; i <= SearchDetails.searchResult.length; i++)
                            $("#tblMrvSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        } else {
            toastr.error("Invalid Customer Name.");
        };
    });
    $("#btnTelephoneNoSearch").on("click", function (e) {
        $("#tblMrvSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
        var searchParam = $('#txtTelephoneNo').val();
        if (searchParam !== "" && searchParam !== undefined) {
            var data = JSON.stringify({ type: 3, searchParam: searchParam });
            $.ajax({
                url: '/Customer/SearchDetailsByCustomer',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (SearchDetails) {
                    if (SearchDetails.searchHeader !== null && SearchDetails.searchHeader !== undefined) {
                        $('#txtJobNo').val(SearchDetails.searchHeader.JobNumber);
                        $('#txtMrvNo').val(SearchDetails.searchHeader.MrvNumber);
                        $('#txtCustCode').val(SearchDetails.searchHeader.CustomerCode);
                        $('#txtCustName').val(SearchDetails.searchHeader.CustomerName);
                        $('#txtAddress').val(SearchDetails.searchHeader.Address);
                        $('#txtTelephoneNo').val(SearchDetails.searchHeader.Telephone);
                        $('#txtEmployee').val(SearchDetails.searchHeader.Employee);
                    }
                    if (SearchDetails.searchResult.length > 0) {
                        for (var i = 0; i <= SearchDetails.searchResult.length; i++)
                            $("#tblMrvSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        } else {
            toastr.error("Invalid Phone Number.");
        }
    });

    $('#formFindMrvDetails').on('init.field.fv', function (e, data) {
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