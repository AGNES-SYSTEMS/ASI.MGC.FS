var alCodeSelect = function (alCodeId) {
    if (alCodeId) {
        var ret = jQuery("#tblAlCodeSearch").jqGrid('getRowData', alCodeId);
        $("#txtAlCode").val(ret.ALCODE_ALD).change();
        $("#txtAlDesc").val(ret.ALDESCRIPTION).change();
        $('#alCodeSearchModel').modal('toggle');
    }
};
var docSelect = function (docId) {
    if (docId) {
        var ret = jQuery("#tblDocSearch").jqGrid('getRowData', docId);
        $("#txtDocType").val(ret.DOCABBREVIATION_DM).change();
        $("#txtDocDetails").val(ret.DESCRIPTION_DM).change();
        $('#docTypeSearchModel').modal('toggle');
    }
};
var bankSelect = function (bankId) {
    if (bankId) {
        var ret = jQuery("#tblBankSearch").jqGrid('getRowData', bankId);
        $("#txtBankCode").val(ret.BANKCODE_BM).change();
        $("#txtBankName").val(ret.BANKNAME_BM).change();
        $('#BankSearchModel').modal('toggle');
    }
};
var accountSelect = function (accountId) {
    if (accountId) {
        var ret = jQuery("#tblAccountSearch").jqGrid('getRowData', accountId);
        $("#txtAccountCode").val(ret.AccountCode).change();
        $("#txtAccountDesc").val(ret.AccountDetail).change();
        $('#accountSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    //$("#liBankReceipt").addClass("active");
    var arrAllocDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtGLDate').datepicker();
    $('#txtChequeDate').datepicker();
    $('#txtClearanceDate').datepicker();
    jQuery("#tblAllocDetails").jqGrid({
        datatype: "local",
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
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
            autoheight: true,
            styleUI: "Bootstrap",
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
    }
    $('#btnAlCodeSearch').click(function () {
        $('#alCodeSearchModel').modal({
            show: true
        });
    });
    $('#btnAccountSearch').click(function () {
        $('#accountSearchModel').modal({
            show: true
        });
    });
    $('#btnBankSearch').click(function () {
        $('#BankSearchModel').modal({
            show: true
        });
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblAllocDetails').setGridWidth(outerwidth);
    });
    $("#docTypeSearchModel").on('show.bs.modal', function () {
        var $docType = "PDC";
        $("#tblDocSearch").jqGrid({
            url: '/DocumentMaster/GetByDocType?docType=' + $docType,
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Document Type', 'Document Name', ''],
            colModel: [
            { key: true, name: 'DOCABBREVIATION_DM', index: 'DOCABBREVIATION_DM', width: 400 },
            { key: false, name: 'DESCRIPTION_DM', index: 'DESCRIPTION_DM', width: 400 },
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
                        "%Href%": "href=javascript:docSelect(&apos;" + rowObject.DOCABBREVIATION_DM + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
            ],
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
        if (docType !== "") {
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
        }
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
        $("#formPdcReceipt").formValidation('revalidateField', 'DocType');
    });
    $("#txtDocDetails").on("change", function () {
        $("#formPdcReceipt").formValidation('revalidateField', 'DocDetails');
    });
    $("#alCodeSearchModel").on('show.bs.modal', function () {
        jQuery("#tblAlCodeSearch").jqGrid({
            url: '/AllocationMaster/GetAllocationMasterList',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['AL Code', 'AL Code Description', ''],
            colModel: [
                { key: true, name: 'ALCODE_ALD', index: 'ALCODE_ALD', width: 400 },
                { key: false, name: 'ALDESCRIPTION', index: 'ALDESCRIPTION', width: 400 },
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
                        "%Href%": "href=javascript:alCodeSelect(&apos;" + rowObject.ALCODE_ALD + "&apos;);"
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
    var searchAccountGrid = function (searchValue) {
        debugger;
        var postData = $("#tblAccountSearch").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;
        postData["accountType"] = $("#txtAlCode").val();
        $("#tblAccountSearch").setGridParam({ postData: postData });
        $("#tblAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtAccountSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtAccountSearch").val().length >= 3 || $("#txtAccountSearch").val().length === 0;
        if (shouldSearch) {
            searchAccountGrid($("#txtAccountSearch").val());
        }
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
        //$("#allocationDetailsModelform").formValidation('revalidateField', 'AlCode');
        if (counter > 0) {
            counter += 1;
            $('#tblAccountSearch').setGridParam({ url: '/AllocationMaster/GetAccountDetailsList?accountType=' + $("#txtAlCode").val() });
            $.jgrid.gridUnload('#tblAccountSearch');
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
    $("#txtBankName").on("change", function () {
        $("#formPdcReceipt").formValidation('revalidateField', 'BankName');
    });
    $("#txtAccountDesc").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountDesc');
    });
    $("#BankSearchModel").on('show.bs.modal', function () {
        var $bankType = "1.Bank";
        $("#tblBankSearch").jqGrid({
            url: '/Bank/GetBankDetailsByType?bankType=' + $bankType,
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
    $('#txtChequeDate').on("blur", function () {
        if ($('#txtChequeDate').val() === "") {
            var currDate = new Date();
            currDate = (currDate.getMonth() + 1) + '/' + currDate.getDate() + '/' + currDate.getFullYear();
            $('#txtChequeDate').val(currDate);
        }
    });
    $('#txtClearanceDate').on("blur", function () {
        if ($('#txtClearanceDate').val() === "") {
            var currDate = new Date();
            currDate = (currDate.getMonth() + 1) + '/' + currDate.getDate() + '/' + currDate.getFullYear();
            $('#txtClearanceDate').val(currDate);
        }
    });
    $("#txtBRAmount").on("blur", function () {
        debugger;
        var totalAmount = parseInt($("#txtBRAmount").val());
        var allocationTotal = parseInt($("#txtAllocationTotal").val());
        if (totalAmount !== allocationTotal && $("#txtAllocationTotal").val() !== "") {
            toastr.info("Allocation Amount does not match with Total Amount");
            $("#btnSubmit").attr("disabled", true);
        } else {
            $("#btnSubmit").attr("disabled", false);
        }
    });
    $("#btnNew").on("click", function () {
        location.reload(true);
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
        $("#formPdcReceipt").formValidation('revalidateField', 'AllocationTotal');
        var totalAmount = parseInt($("#txtBRAmount").val());
        var allocationTotal = parseInt($("#txtAllocationTotal").val());
        if (totalAmount !== allocationTotal) {
            toastr.info("Allocation Amount does not match with Total Amount");
            $("#btnSubmit").attr("disabled", true);
        } else {
            $("#btnSubmit").attr("disabled", false);
        }
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
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblAccountSearch").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblAccountSearch").setGridParam({ postData: postData });
        $("#tblAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtAccountSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtAccountSearch").val().length >= 3 || $("#txtAccountSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtAccountSearch").val());
        }
    });
    $('#formPdcReceipt').on('init.field.fv', function (e, data) {
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
            DocNo: {
                validators: {
                    notEmpty: {
                        message: 'Document No is required'
                    }
                }
            },
            DOCDATE_BT: {
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
            GLDATE_BT: {
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
            NOTE_BT: {
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
            DEBITAMOUT_BT: {
                validators: {
                    notEmpty: {
                        message: 'BR Amount is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            AllocationTotal: {
                validators: {
                    notEmpty: {
                        message: 'Allocation Amount is required'
                    }
                }
            },
            CHQNO_BT: {
                validators: {
                    notEmpty: {
                        message: 'Cheque No is required'
                    }
                }
            },
            OTHERREF_BT: {
                validators: {
                    notEmpty: {
                        message: 'Other Ref is required'
                    }
                }
            },
            BENACNO_BT: {
                validators: {
                    notEmpty: {
                        message: 'Drw AC No is required'
                    }
                }
            }, BENACCOUNT_BT: {
                validators: {
                    notEmpty: {
                        message: 'Drawer Br is required'
                    }
                }
            },
            CHQDATE_BT: {
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
            CLEARANCEDATE_BT: {
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
    $("#formPdcReceipt").formValidation();
});
