﻿$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liCashPayments").addClass("active");
    var arrAllocDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtGLDate').datepicker();
    jQuery("#tblAllocDetails").jqGrid({
        datatype: "local",
        height: 150,
        colNames: ['AL Code', 'Account Code', 'Account Description', 'Amount', 'Narration'],
        colModel: [
            { name: 'AlCode', index: 'AlCode', width: 150, align: "center", sortable: false },
            { name: 'AccountCode', index: 'AccountCode', width: 150, align: "left", sortable: false },
            { name: 'Description', index: 'Description', width: 320, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "right", sortable: false },
            { name: 'Narration', index: 'Narration', width: 300, align: "left", sortable: false }
        ],
        multiselect: false,
        caption: "Allocation Details"
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblAllocDetails').setGridWidth(outerwidth);
    });
    var counter = 0;
    function populateAccountGrid() {
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
    }
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
        populateAccountGrid();
    });
    $("#BankSearchModel").on('show.bs.modal', function () {
        var $bankType = "2.Cash";
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
        $(window).resize(function () {
            var outerwidth = $('#bankGrid').width();
            $('#tblBankSearch').setGridWidth(outerwidth);
        });
    });
    $("#docTypeSearchModel").on('show.bs.modal', function () {
        var $docType = "CPAY";
        $("#tblDocSearch").jqGrid({
            url: '/DocumentMaster/GetByDocType?docType=' + $docType,
            datatype: "json",
            colNames: ['Document Type', 'Document Name'],
            colModel: [
            { key: true, name: 'DOCABBREVIATION_DM', index: 'DOCABBREVIATION_DM', width: 400 },
            { key: false, name: 'DESCRIPTION_DM', index: 'DESCRIPTION_DM', width: 400 }],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#docPager'),
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
            var outerwidth = $('#docGrid').width();
            $('#tblDocSearch').setGridWidth(outerwidth);
        });
    });
    $("#docTypeSearchModel").on('hide.bs.modal', function () {
        var docType = $("#txtDocType").val();
        var data = JSON.stringify({ docType: docType });
        $.ajax({
            url: '/DocumentMaster/GetDocNo',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (docNumber) {
                $("#txtDocNo").val(docNumber);
            },
            complete: function () {
            },
            error: function () {
            }
        });
    });
    $("#btnDocSelect").on("click", function (e) {
        var id = jQuery("#tblDocSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblDocSearch").jqGrid('getRowData', id);
            $("#txtDocType").val(ret.DOCABBREVIATION_DM).change();
            $("#txtDocDetails").val(ret.DESCRIPTION_DM).change();
            $('#docTypeSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#txtDocType").on("change", function () {
        $("#formCashPayments").formValidation('revalidateField', 'DocType');
    });
    $("#txtDocDetails").on("change", function () {
        $("#formCashPayments").formValidation('revalidateField', 'DocDetails');
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
    $("#btnAlCodeSelect").on("click", function (e) {
        var id = jQuery("#tblAlCodeSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblAlCodeSearch").jqGrid('getRowData', id);
            $("#txtAlCode").val(ret.ALCODE_ALD).change();
            $("#txtAlDesc").val(ret.ALDESCRIPTION).change();
            $('#alCodeSearchModel').modal('toggle');
        }
        e.preventDefault();
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
    $("#txtAlCode").on("change", function () {
        $("#btnAccountSearch").prop('disabled', false);
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AlCode');
        if (counter > 0) {
            counter += 1;
            $('#tblAccountSearch').setGridParam({ url: '/AllocationMaster/GetAccountDetailsList?accountType=' + $("#txtAlCode").val() });
            $('#tblAccountSearch').jqGrid('GridUnload');
        } else {
            counter += 1;
        }
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtAmount").val("");
        $("#txtNarration").val("");
    });
    $("#txtAlDesc").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AlDesc');
    });
    $("#txtAccountCode").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountCode');
    });
    $("#txtAccountDesc").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountDesc');
    });
    function clearModalForm() {
        $("#txtAlCode").val("");
        $("#txtAlDesc").val("");
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtAmount").val("");
        $("#txtNarration").val("");
        $("#btnAccountSearch").prop('disabled', true);
    }
    $("#allocationDetailsModel").on('hide.bs.modal', function () {
        clearModalForm();
        var totalGridPrdAmount = 0.0;
        for (var i = 0; i < arrAllocDetails.length; i++) {
            totalGridPrdAmount += parseFloat(arrAllocDetails[i]["Amount"]);
        }
        $("#txtAllocationTotal").val(totalGridPrdAmount);
    });
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#allocationDetailsModelform").valid()) {
            e.preventDefault();
            var arrIndex = arrAllocDetails.length;
            arrAllocDetails[arrIndex] = {
                AlCode: $("#txtAlCode").val(), AccountCode: $("#txtAccountCode").val(), Description: $("#txtAccountDesc").val(),
                Amount: $("#txtAmount").val(), Narration: $("#txtNarration").val()
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
    $('#formCashPayments').bootstrapValidator({
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
            DocDetails: {
                validators: {
                    notEmpty: {
                        message: 'Document Detail is required'
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
            GLDATE_VRPT: {
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
            BankCode: {
                validators: {
                    notEmpty: {
                        message: 'Bank Code is required'
                    }
                }
            },
            ReceivedFrom: {
                validators: {
                    notEmpty: {
                        message: 'Received From is required'
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
            CRAmount: {
                validators: {
                    notEmpty: {
                        message: 'CR Amount is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            }
        }
    });
});