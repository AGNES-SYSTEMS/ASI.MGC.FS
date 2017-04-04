var arrAllocDetails = [];
var selectedRowId = "";
var receiverSelect = function (receiverId) {
    debugger;
    if (receiverId) {
        var ret = jQuery("#tblReceiverSearch").jqGrid('getRowData', receiverId);
        $("#txtReceivedFrom").val(ret.DESCRIPTION_ARM).change();
        $("#formBankReceipt").formValidation('revalidateField', 'NOTE_BT');
        $('#ReceiverSearchModel').modal('toggle');
    }
};
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
var delProduct = function (rowId) {
    if (rowId) {
        $('#tblAllocDetails').jqGrid('delRowData', rowId);
        $('#tblAllocDetails').trigger('reloadGrid');
        stringfyData();
        calculateNetAmount();
    }
};
var calculateNetAmount = function () {
    var totalGridPrdAmount = 0.0;
    for (var i = 0; i < arrAllocDetails.length; i++) {
        totalGridPrdAmount += parseFloat(arrAllocDetails[i]["Amount"]);
    }
    $("#txtAllocationTotal").val(totalGridPrdAmount);
    $("#formBankReceipt").formValidation('revalidateField', 'AllocationTotal');
};
var stringfyData = function () {
    $('#tblAllocDetails').trigger('reloadGrid');
    var jsonAllocs = JSON.stringify(arrAllocDetails);
    calculateNetAmount();
    $('#hdnAllocDetails').val(jsonAllocs);
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liBankReceipt").addClass("active");
    arrAllocDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtGLDate').datepicker();
    $('#txtChequeDate').datepicker();
    $('#txtClearanceDate').datepicker();
    jQuery("#tblAllocDetails").jqGrid({
        datatype: "local",
        data: arrAllocDetails,
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['AL Code', 'Account Code', 'Account Description', 'Amount', 'Narration', '', ''],
        colModel: [
            { name: 'AlCode', index: 'AlCode', width: 150, align: "center", sortable: false },
            { name: 'AccountCode', index: 'AccountCode', width: 150, align: "left", sortable: false },
            { name: 'Description', index: 'Description', width: 300, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "right", sortable: false },
            { name: 'Narration', index: 'Narration', width: 270, align: "left", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 25,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#allocationDetailsModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:editProduct(&apos;" + options.rowId + "&apos;);",
                        "%Id%": "id='" + options.rowId + "'"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 25,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href%><i class='fa fa-trash-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:delProduct(&apos;" + options.rowId + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
        ],
        multiselect: false,
        caption: "Allocation Details"
    });
    $("#btnNew").on("click", function () {
        location.reload();
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
        var $docType = "BRV";
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
                $("#formBankReceipt").formValidation('revalidateField', 'DOCNUMBER_BT');
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
        $("#formBankReceipt").formValidation('revalidateField', 'DocType');
    });
    $("#txtDocNo").on("change", function () {
        $("#formBankReceipt").formValidation('revalidateField', 'DOCNUMBER_BT');
    });
    $("#txtDocDetails").on("change", function () {
        $("#formBankReceipt").formValidation('revalidateField', 'DocDetails');
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
    $("#txtBankCode").on("change", function () {
        $("#formBankReceipt").formValidation('revalidateField', 'BANKCODE_BT');
    });
    $("#txtBankName").on("change", function () {
        $("#formBankReceipt").formValidation('revalidateField', 'BankName');
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
    function clearModalForm() {
        $("#txtAlCode").val("");
        $("#txtAlDesc").val("");
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtAmount").val("");
        $("#txtNarration").val("");
        $("#btnAccountSearch").prop('disabled', true);
        selectedRowId = "";
    }
    $("#allocationDetailsModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            selectedRowId = e.relatedTarget.id;
            var rowId = e.relatedTarget.id;
            var ret = $('#tblAllocDetails').jqGrid('getRowData', rowId);
            $("#txtAlCode").val(ret.AlCode);
            $("#txtAlDesc").val(ret.AccountCode);
            $("#txtAccountCode").val(ret.Description);
            $("#txtAccountDesc").val(ret.Description);
            $("#txtAmount").val(ret.Amount);
            $("#txtNarration").val(ret.Narration);
        }
    });
    $("#allocationDetailsModel").on('hide.bs.modal', function () {
        clearModalForm();
        stringfyData();
        calculateNetAmount();
        $(this).find('form')[0].reset();
    });
    $("#ReceiverSearchModel").on('show.bs.modal', function () {
        $("#tblReceiverSearch").jqGrid({
            url: '/Customer/GetCustomerDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Customer Code', 'Customer Name', ''],
            colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 400 },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 400 },
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
                        "%Href%": "href=javascript:receiverSelect(&apos;" + rowObject.ARCODE_ARM + "&apos;);"
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
            pager: jQuery('#receiverPager'),
            caption: "Receiver's List",
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
        var outerwidth = $('#receivedGrid').width();
        $('#tblReceiverSearch').setGridWidth(outerwidth);
    });
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#allocationDetailsModelform").valid()) {
            e.preventDefault();
            if (selectedRowId) {
                arrAllocDetails[parseInt(selectedRowId) - 1] = {
                    AlCode: $("#txtAlCode").val(),
                    AccountCode: $("#txtAccountCode").val(),
                    Description: $("#txtAccountDesc").val(),
                    Amount: $("#txtAmount").val(),
                    Narration: $("#txtNarration").val()
                };
            } else {
                var arrIndex = arrAllocDetails.length;
                arrAllocDetails[arrIndex] = {
                    AlCode: $("#txtAlCode").val(),
                    AccountCode: $("#txtAccountCode").val(),
                    Description: $("#txtAccountDesc").val(),
                    Amount: $("#txtAmount").val(),
                    Narration: $("#txtNarration").val()
                };
            }
            clearModalForm();
        }
        else {
            //$("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AlCode');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AlDesc');
            //$("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AccountCode');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'AccountDesc');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'Amount');
            $("#allocationDetailsModelform").bootstrapValidator('revalidateField', 'Narration');
        }
    });
    var searchGrid = function (searchById, searchByName, gridType) {
        if (gridType === "1") {
            var postData = $("#tblAccountSearch").jqGrid("getGridParam", "postData");
            postData["searchById"] = searchById;
            postData["searchByName"] = searchByName;
            $("#tblAccountSearch").setGridParam({ postData: postData });
            $("#tblAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
        } else if (gridType === "2") {
            postData = $("#tblReceiverSearch").jqGrid("getGridParam", "postData");
            postData["custId"] = searchById;
            postData["custName"] = searchByName;
            $("#tblReceiverSearch").setGridParam({ postData: postData });
            $("#tblReceiverSearch").trigger("reloadGrid", [{ page: 1 }]);
        }

    };
    $("#txtAccIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtAccIdSearch").val().length >= 1 || $("#txtAccIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtAccIdSearch").val(), $("#txtAccNameSearch").val(), "1");
        }
    });
    $("#txtAccNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtAccNameSearch").val().length >= 3 || $("#txtAccNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtAccIdSearch").val(), $("#txtAccNameSearch").val(), "1");
        }
    });
    $("#txtArIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtArIdSearch").val().length >= 1 || $("#txtArIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtArIdSearch").val(), $("#txtArNameSearch").val(), "2");
        }
    });
    $("#txtArNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtArNameSearch").val().length >= 3 || $("#txtArNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtArIdSearch").val(), $("#txtArNameSearch").val(), "2");
        }
    });
    $("#btnReceiverSelect").on("click", function (e) {
        debugger;
        var id = jQuery("#tblReceiverSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblReceiverSearch").jqGrid('getRowData', id);
            $("#txtReceivedFrom").val(ret.DESCRIPTION_ARM).change();
            $("#formBankReceipt").formValidation('revalidateField', 'NOTE_BT');
            $('#ReceiverSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $('#formBankReceipt').on('init.field.fv', function (e, data) {
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
            ReceivedFrom: {
                validators: {
                    notEmpty: {
                        message: 'Received From is required'
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
            DEBITAMOUT_BT: {
                validators: {
                    notEmpty: {
                        message: 'BR Amount is required'
                    },
                    identical: {
                        field: 'AllocationTotal',
                        message: 'Allocation Total must equel to Amount'
                    }
                }
            },
            AllocationTotal: {
                validators: {
                    notEmpty: {
                        message: 'Allocation Amount is required'
                    },
                    identical: {
                        field: 'DEBITAMOUT_BT',
                        message: 'Allocation Total must equel to Amount'
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
            },
            BENACCOUNT_BT: {
                validators: {
                    notEmpty: {
                        message: 'Ben Account is required'
                    }
                }
            },
            BENACNO_BT: {
                validators: {
                    notEmpty: {
                        message: 'Ben AC NO is required'
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
    }).off('success.form.fv').on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
    $("#formBankReceipt").formValidation();
});
