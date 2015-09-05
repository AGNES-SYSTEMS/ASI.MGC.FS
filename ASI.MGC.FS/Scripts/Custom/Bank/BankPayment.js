$(document).ready(function () {
    var arrAllocDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtGLDate').datepicker();
    $('#txtChequeDate').datepicker();
    $('#txtClearanceDate').datepicker();
    jQuery("#tblAllocDetails").jqGrid({
        datatype: "local",
        height: 150,
        colNames: ['AL Code', 'Account Code', 'Account Description', 'Amount', 'Narration'],
        colModel: [
            { name: 'ALLCODE_VCD', index: 'ALLCODE_VCD', width: 150, align: "center", sortable: false },
            { name: 'ACCODE_VCD', index: 'ACCODE_VCD', width: 150, align: "left", sortable: false },
            { name: 'ACDESCRIPTION_VCD', index: 'ACDESCRIPTION_VCD', width: 320, align: "center", sortable: false },
            { name: 'DEBITAMOUNT_VCD', index: 'DEBITAMOUNT_VCD', width: 150, align: "left", sortable: false },
            { name: 'NARRATION_VCD', index: 'NARRATION_VCD', width: 300, align: "right", sortable: false }
        ],
        multiselect: false,
        caption: "Allocation Details"
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblAllocDetails').setGridWidth(outerwidth);
    });
    $("#alCodeSearchModel").on('show.bs.modal', function () {
        jQuery("#tblAlCodeSearch").jqGrid({
            url: '/AllocationMaster/GetAllocationMasterList',
            datatype: "json",
            colNames: ['AL Code', 'AL Code Description'],
            colModel: [
                { name: 'ALCODE_ALD', index: 'ALCODE_ALD', width: 400 },
                { name: 'ALDESCRIPTION', index: 'ALDESCRIPTION', width: 400 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#alCodePager'),
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
            var outerwidth = $('#alCodegrid').width();
            $('#tblAlCodeSearch').setGridWidth(outerwidth);
        });
    });
    $("#accountSearchModel").on('show.bs.modal', function () {
        jQuery("#tblAccountSearch").jqGrid({
            url: '/AllocationMaster/GetAccountDetailsList?accountType=' + $("#txtAlCode").val(),
            datatype: "json",
            colNames: ['Account Code', 'Account Description'],
            colModel: [
                { name: 'AccountCode', index: 'AccountCode', width: 400 },
                { name: 'AccountDetail', index: 'AccountDetail', width: 400 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
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
    $("#BankSearchModel").on('show.bs.modal', function () {
        var $bankType = "1.Bank";
        $("#tblBankSearch").jqGrid({
            url: '/Bank/GetBankDetailsByType?bankType=' + $bankType,
            datatype: "json",
            colNames: ['Bank Code', 'Bank Name'],
            colModel: [
            { key: true, name: 'BANKCODE_BM', index: 'BANKCODE_BM', width: 400 },
            { key: false, name: 'BANKNAME_BM', index: 'BANKNAME_BM', width: 400 }],
            rowNum: 20,
            rowList: [20, 30, 40],
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
    });
    $(window).resize(function () {
        var outerwidth = $('#bankGrid').width();
        $('#tblBankSearch').setGridWidth(outerwidth);
    });
    $("#btnBankSelect").on("click", function (e) {
        var id = jQuery("#tblBankSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblBankSearch").jqGrid('getRowData', id);
            $("#txtBankCode").val(ret.BANKCODE_BM).change();
            $("#txtBankName").val(ret.BANKNAME_BM).change();
            $('#BankSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#txtBankCode").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'BankCode');
    });
    $("#txtBankName").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'BankName');
    });
    $('#txtChequeDate').on("blur", function () {
        if ($('#txtChequeDate').val() == "") {
            var currDate = new Date();
            currDate = (currDate.getMonth() + 1) + '/' + currDate.getDate() + '/' + currDate.getFullYear();
            $('#txtChequeDate').val(currDate)
        }
    });
    $('#txtClearanceDate').on("blur", function () {
        if ($('#txtClearanceDate').val() == "") {
            var currDate = new Date();
            currDate = (currDate.getMonth() + 1) + '/' + currDate.getDate() + '/' + currDate.getFullYear();
            $('#txtClearanceDate').val(currDate)
        }
    });
    $("#btnAlCodeSelect").on("click", function (e) {
        var id = jQuery("#tblAlCodeSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblAlCodeSearch").jqGrid('getRowData', id);
            $("#txtAlCode").val(ret.ALCODE_ALD).change();
            alert($("#txtAlCode").val());
        }
    });
    function clearModalForm() {
        $("#txtAlCode").val("");
        $("#txtAlDesc").val("");
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtAmount").val("");
        $("#txtNarration").val("");
    }
    $("#allocationDetailsModel").on('hide.bs.modal', function () {
        clearModalForm();
        var totalGridPrdAmount = 0.0;
        for (i = 0; i < arrAllocDetails.length; i++) {
            totalGridPrdAmount += parseFloat(arrAllocDetails[i]["AMOUNT_VCD"]);
        }
        $("#txtAllocationTotal").val(totalGridPrdAmount);
    });
    $("#btnCancel").click(function (e) {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#allocationDetailsModelform").valid()) {
            e.preventDefault();
            var arrIndex = arrAllocDetails.length;
            arrAllocDetails[arrIndex] = {
                ALLCODE_VCD: $("#txtAlCode").val(), ACCODE_VCD: $("#txtAccountCode").val(), ACDESCRIPTION_VCD: $("#txtAccountDesc").val(),
                AMOUNT_VCD: $("#txtAmount").val(), NARRATION_VCD: $("#txtNarration").val()
            };
            var su = jQuery("#tblAllocDetails").jqGrid('addRowData', arrIndex, arrAllocDetails[arrIndex]);
            if (su) {
                var allocDetails = $('#tblAllocDetails').jqGrid('getGridParam', 'data');
                var jsonAllocs = JSON.stringify(allocDetails);
                $('#hdnAllocDetails').val(jsonAllocs);
                clearModalForm();
            }
        }
        else {
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AlCode');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AlDesc');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AccountCode');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AccountDesc');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'Amount');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'Narration');
        }
    });
    $('#formBankPayment').bootstrapValidator({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            DocType: {
                validators: {
                    notEmpty: {
                        message: 'Document Type is required'
                    }
                }
            },
            DocNo: {
                validators: {
                    notEmpty: {
                        message: 'Document No is required'
                    }
                }
            },
            DocDate: {
                validators: {
                    notEmpty: {
                        message: 'Document Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            GLDate: {
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
            DocDetails: {
                validators: {
                    notEmpty: {
                        message: 'Document Details is required'
                    }
                }
            },
            PaidTo: {
                validators: {
                    notEmpty: {
                        message: 'Paid To is required'
                    }
                }
            },
            BankCode: {
                validators: {
                    notEmpty: {
                        message: 'Bank Code is required'
                    }
                }
            },
            BankName: {
                validators: {
                    notEmpty: {
                        message: 'Bank Name is required'
                    }
                }
            },
            BRAmount: {
                validators: {
                    notEmpty: {
                        message: 'BR Amount is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            ChequeNo: {
                validators: {
                    notEmpty: {
                        message: 'Cheque No is required'
                    }
                }
            },
            ChequeDate: {
                validators: {
                    notEmpty: {
                        message: 'Cheque Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            ClearanceDate: {
                validators: {
                    notEmpty: {
                        message: 'Clearance Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            }
        }
    });
});

