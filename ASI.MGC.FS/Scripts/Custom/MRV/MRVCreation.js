$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liMrv").addClass("active");

    var arrMetarials = [];
    $('#txtDeleDate').datepicker();
    $('#txtMRVDate').datepicker();
    jQuery("#tblMetarials").jqGrid({
        datatype: "local",
        height: 150,
        colNames: ['PrCode', 'Product Description', 'Job ID', 'Job Description', 'Quantity', 'Rate', 'Amount'],
        colModel: [
            { name: 'PRODID_MRR', index: 'PRODID_MRR', width: 80, align: "center", sortable: false },
            { name: 'prdesc', index: 'prdesc', width: 300, align: "left", sortable: false },
            { name: 'JOBID_MRR', index: 'JOBID_MRR', width: 80, align: "center", sortable: false },
            { name: 'jobdesc', index: 'jobdesc', width: 300, align: "left", sortable: false },
            { name: 'QTY_MRR', index: 'QTY_MRR', width: 90, align: "right", sortable: false },
            { name: 'RATE_MRR', index: 'RATE_MRR', width: 100, align: "right", sortable: false },
            { name: 'AMOUNT_MRR', index: 'AMOUNT_MRR', width: 100, align: "right", sortable: false }
        ],
        multiselect: false,
        caption: "Materials Details"
    });

    $("#txtPrCode").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'PrCode');
    });

    $("#txtPrDesc").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'PrDesc');
    });

    $("#txtJobID").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'JobID');
    });

    $("#txtJobDesc").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'JobDesc');
    });

    $("#txtQuantity").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'Quantity');
    });

    $("#txtRate").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'Rate');
    });

    function clearModalForm() {
        $("#txtPrCode").val("");
        $("#txtPrDesc").val("");
        $("#txtJobID").val("");
        $("#txtJobDesc").val("");
        $("#txtQuantity").val("");
        $("#txtRate").val("");
        $("#txtAmount").val("0");
    }

    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#mrvProductModelform").valid()) {
            e.preventDefault();
            var arrIndex = arrMetarials.length;
            arrMetarials[arrIndex] = {
                PRODID_MRR: $("#txtPrCode").val(), prdesc: $("#txtPrDesc").val(), JOBID_MRR: $("#txtJobID").val(),
                jobdesc: $("#txtJobDesc").val(), QTY_MRR: $("#txtQuantity").val(), RATE_MRR: $("#txtRate").val(),
                AMOUNT_MRR: $("#txtAmount").val()
            };
            var su = jQuery("#tblMetarials").jqGrid('addRowData', arrIndex, arrMetarials[arrIndex]);
            if (su) {
                var mrvPrds = $('#tblMetarials').jqGrid('getGridParam', 'data');
                var jsonMrvPrds = JSON.stringify(mrvPrds);
                $('#mrvProds').val(jsonMrvPrds);
                clearModalForm();
            }
        }
        else {
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'PrCode');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'PrDesc');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'JobID');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'JobDesc');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Quantity');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Rate');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Amount');
        }
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblMetarials').setGridWidth(outerwidth);
    });
    $("#mrvProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        var totalGridPrdAmount = 0.0;
        for (var i = 0; i < arrMetarials.length; i++) {
            totalGridPrdAmount += parseFloat(arrMetarials[i]["AMOUNT_MRR"]);
        }
        $("#txtNetPrdAmount").val(totalGridPrdAmount);
    });
    $("#txtCustCode").autocomplete({
        source: '/Customer/GetCustomersCode',
        minLength: 0
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });
    $("#txtCustName").autocomplete({
        source: '/Customer/GetCustomersName',
        minLength: 0
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });

    $("#txtExeCode").autocomplete({
        source: '/EmployeeMaster/getEmployeeIDs',
        minLength: 0,
        change: function () {
            $('#formMRVCreation').bootstrapValidator('revalidateField', 'EXECODE_MRV');
        }
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });
    function getCustRecord() {
        var custCode = $('#txtCustCode').val();
        var custName = $('#txtCustName').val();
        var data = JSON.stringify({ custCode: custCode, custName: custName });
        $.ajax({
            url: '/Customer/getCustomerRecord',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (custDetails) {
                $('#txtCustCode').val(custDetails.ARCODE_ARM).change();
                $('#txtCustName').val(custDetails.DESCRIPTION_ARM).change();
                $('#txtMRVAddress').val(custDetails.ADDRESS1_ARM).change();
                $('#txtMRVTel').val(custDetails.TELEPHONE_ARM).change();
                $('#txtMRVAddress2').val(custDetails.ADDRESS2_ARM);
            },
            complete: function () {
            },
            error: function () {
            }
        });
    }
    $("#txtCustCode").blur(function () {
        $('#txtCustName').val("");
        getCustRecord();
    });

    $("#txtCustName").blur(function () {
        $('#txtCustCode').val("");
        getCustRecord();
    });

    $("#txtQuantity").change(function () {
        var totalAmount = $("#txtQuantity").val() * $("#txtRate").val();
        $("#txtAmount").val(totalAmount);
    });

    $("#txtRate").change(function () {
        var totalAmount = $("#txtQuantity").val() * $("#txtRate").val();
        $("#txtAmount").val(totalAmount);
    });

    $('#txtCustCode').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CUSTOMERCODE_MRV');
    });
    $('#txtCustName').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CUSTOMERNAME_MRV');
    });
    $('#txtMRVAddress').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'ADDRESS1_MRV');
    });
    $('#txtMRVTel').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'PHONE_MRV');
    });
    $("#mrvPrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetProductDetailsList',
            datatype: "json",
            colNames: ['Product Code', 'Product Details'],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 400 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 400 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#prdPager'),
            caption: "Product Details List",
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
        var outerwidth = $('#prdGrid').width();
        $('#tblProductSearch').setGridWidth(outerwidth);
    });

    $("#btnSearchPrd").on("click", function (e) {
        e.preventDefault();
        var $postDataValues = $("tblProductSearch").jqGrid('getGridParam', 'postData');
        $('.filterProduct').each(function (index, item) {
            $postDataValues[$(item).attr("id")] = $(item).val();
        });
        $("tblProductSearch").jqGrid().setGridParam({ postData: $postDataValues, page: 1 }).trigger('reloadGrid');
    });

    $("#mrvJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/GetJobDetailsList',
            datatype: "json",
            colNames: ['Product Code', 'Product Details', 'Rate'],
            colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 250 },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 400 },
            { key: false, name: 'RATE_RJ', index: 'RATE_RJ', width: 150 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#jobPager'),
            caption: "Job Details List",
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

    $("#btnProductSelect").on("click", function (e) {
        var id = jQuery("#tblProductSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblProductSearch").jqGrid('getRowData', id);
            $("#txtPrCode").val(ret.PROD_CODE_PM).change();
            $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
            $('#mrvPrdSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $("#btnJobSelect").on("click", function (e) {
        var id = jQuery("#tblJobSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblJobSearch").jqGrid('getRowData', id);
            $("#txtJobID").val(ret.JOBID_JR).change();
            $("#txtJobDesc").val(ret.JOBDESCRIPTION_JR).change();
            $("#txtRate").val(ret.RATE_RJ).change();
            $('#mrvJobSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $('#formMRVCreation').bootstrapValidator({
        container: '#messages',
        feedbackIcons: {
            required: 'fa fa-asterisk',
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            CUSTOMERCODE_MRV: {
                required: true,
                validators: {
                    notEmpty: {
                        message: 'Customer Code is required'
                    }
                }
            },
            CUSTOMERNAME_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Customer Name is required'
                    }
                }
            },
            MRVDATE_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            ADDRESS1_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Address is required'
                    }
                }
            },
            PHONE_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Phone Number is required'
                    },
                    digit: {
                        message: 'Enter Valid Phone Number'
                    }
                }
            },
            DELE_DATE_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            EXECODE_MRV: {
                validators: {
                }
            }
        }
    });
    $("#mrvProductModelform").formValidation();
});