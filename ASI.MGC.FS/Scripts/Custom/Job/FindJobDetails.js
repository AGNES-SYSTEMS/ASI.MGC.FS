$(document).ready(function () {
    jQuery("#tblJobSearchDetails").jqGrid({
        datatype: "local",
        height: 175,
        width: 1050,
        shrinkToFit: false,
        styleUI: "Bootstrap",
        colNames: ['Mrv No', 'Date', 'Customer Name', 'Product', 'Delivery Note', 'Job No', 'Status', 'Telephone'],
        colModel: [
            { name: 'MRVNO_MRV', index: 'MRVNO_MRV', width: 150, align: "center", sortable: false },
            { name: 'MRVDATE_MRV', index: 'MRVDATE_MRV', width: 150, align: "center", sortable: false },
            { name: 'CUSTOMERNAME_MRV', index: 'CUSTOMERNAME_MRV', width: 200, align: "center", sortable: false },
            { name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 200, align: "center", sortable: false },
            { name: 'DELEVERNOTENO_JM', index: 'DELEVERNOTENO_JM', width: 150, align: "center", sortable: false },
            { name: 'JOBNO_JM', index: 'JOBNO_JM', width: 150, align: "center", sortable: false },
            { name: 'JOBSTATUS_JM', index: 'JOBSTATUS_JM', width: 50, align: "center", sortable: false },
            { name: 'PHONE_MRV', index: 'PHONE_MRV', width: 150, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "Materials Details"
    });
    $("#btnJobNoSearch").click(function (e) {
        $("#tblJobSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
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
                            $("#tblJobSearchDetails").jqGrid('addRowData', i + 1, jobDetails.searchResult[i]);
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
        $("#tblJobSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
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
                            $("#tblJobSearchDetails").jqGrid('addRowData', i + 1, mrvDetails.searchResult[i]);
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
        $("#tblJobSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
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
                            $("#tblJobSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
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
        $("#tblJobSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
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
                            $("#tblJobSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
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
        $("#tblJobSearchDetails").jqGrid("clearGridData", true).trigger("reloadGrid");
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
                            $("#tblJobSearchDetails").jqGrid('addRowData', i + 1, SearchDetails.searchResult[i]);
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

    $('#formFindJobDetails').on('init.field.fv', function (e, data) {
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