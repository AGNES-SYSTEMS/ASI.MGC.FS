var arrAllocDetails = [];
var selectedRowId = "";
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
var accountSelect = function (accountId) {
    if (accountId) {
        var ret = jQuery("#tblAccountSearch").jqGrid('getRowData', accountId);
        $("#txtAccountCode").val(ret.AccountCode).change();
        $("#txtAccountDesc").val(ret.AccountDetail).change();
        $('#accountSearchModel').modal('toggle');
    }
};
var calculateNetAmount = function () {
    var creditAmount = 0.0, debitAmount = 0.0, difference = 0.0;
    for (var i = 0; i < arrAllocDetails.length; i++) {
        creditAmount += parseFloat(arrAllocDetails[i]["Credit"]);
        debitAmount += parseFloat(arrAllocDetails[i]["Debit"]);
        difference = debitAmount - creditAmount;
    }
    $("#txtTotalCreditAmount").val(creditAmount);
    $("#txtTotalDebitAmount").val(debitAmount);
    $("#txtDifference").val(difference);
    $("#formJvCreation").formValidation('revalidateField', 'TotalCreditAmount');
    $("#formJvCreation").formValidation('revalidateField', 'TotalDebitAmount');
    $("#formJvCreation").formValidation('revalidateField', 'Difference');
}
var stringifyData = function () {
    var allocDetails = $('#tblAllocDetails').jqGrid('getGridParam', 'data');
    var jsonAllocs = JSON.stringify(allocDetails);
    $('#hdnAllocDetails').val(jsonAllocs);
};
var delAllocDetails = function (rowId) {
    if (rowId) {
        $('#tblAllocDetails').jqGrid('delRowData', rowId);
        $('#tblAllocDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    }
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    //$("#liBankReceipt").addClass("active");
    $('#txtDocDate').datepicker();
    $('#txtGlDate').datepicker();
    jQuery("#tblAllocDetails").jqGrid({
        datatype: "local",
        data: arrAllocDetails,
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['AL Code', '', 'Account Code', 'Account Description', 'Debit', 'Credit', 'Narration', '', ''],
        colModel: [
            { name: 'AlCode', index: 'AlCode', width: 150, align: "center", sortable: false },
            { name: 'AlDescription', index: 'AlDescription', width: 320, align: "center", sortable: false, hidden: true },
            { name: 'AccountCode', index: 'AccountCode', width: 150, align: "left", sortable: false },
            { name: 'Description', index: 'Description', width: 320, align: "center", sortable: false },
            { name: 'Debit', index: 'Debit', width: 150, align: "right", sortable: false },
            { name: 'Credit', index: 'Credit', width: 150, align: "right", sortable: false },
            { name: 'Narration', index: 'Narration', width: 300, align: "left", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#allocationDetailsModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:editAllocDetails(&apos;" + options.rowId + "&apos;);",
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
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href%><i class='fa fa-trash-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:delAllocDetails(&apos;" + options.rowId + "&apos;);"
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
    $(window).resize(function () {
        var outerwidth = $('#allocGrid').width();
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
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblAllocDetails').setGridWidth(outerwidth);
    });
    $("#docTypeSearchModel").on('show.bs.modal', function () {
        var $docType = "JV";
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
                    $("#formJvCreation").formValidation('revalidateField', 'DOCNUMBER_BT');
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
        $("#formJvCreation").formValidation('revalidateField', 'DocType');
    });
    $("#txtDocDetails").on("change", function () {
        $("#formJvCreation").formValidation('revalidateField', 'DocDetails');
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
    var searchAccountGrid = function (searchId, searchValue) {
        debugger;
        var postData = $("#tblAccountSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = searchId;
        postData["searchByName"] = searchValue;
        postData["accountType"] = $("#txtAlCode").val();
        $("#tblAccountSearch").setGridParam({ postData: postData });
        $("#tblAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtSearchId").off().on("keyup", function () {
        var shouldSearch = $("#txtSearchId").val().length >= 1 || $("#txtAccountSearch").val().length === 0;
        if (shouldSearch) {
            searchAccountGrid($("#txtSearchId").val(), $("#txtAccountSearch").val());
        }
    });
    $("#txtAccountSearch").off().on("keyup", function () {
        var shouldSearch = $("#txtAccountSearch").val().length >= 3 || $("#txtAccountSearch").val().length === 0;
        if (shouldSearch) {
            searchAccountGrid($("#txtSearchId").val(), $("#txtAccountSearch").val());
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
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AlCode');
        if (counter > 0) {
            counter += 1;
            $('#tblAccountSearch').setGridParam({ url: '/AllocationMaster/GetAccountDetailsList?accountType=' + $("#txtAlCode").val() });
            $.jgrid.gridUnload('#tblAccountSearch');
        } else {
            counter += 1;
        }
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtDebit").val("");
        $("#txtCredit").val("");
        $("#txtAccNarration").val("");
    });
    $("#txtAlDesc").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AlDesc');
    });
    $("#txtCredit").off().on("blur", function () {
        var creditAmount = $("#txtCredit").val();
        if (parseInt(creditAmount) !== 0 && creditAmount !== "") {
            $("#txtDebit").val("0");
            $("#txtDebit").prop('disabled', true);
            $("#allocationDetailsModelform").formValidation('revalidateField', 'Debit');
        }
        else {
            $("#txtDebit").prop('disabled', false);
        }
    });
    $("#txtDebit").off().on("blur", function () {
        var debitAmount = $("#txtDebit").val();
        if (parseInt(debitAmount) !== 0 && debitAmount !== "") {
            $("#txtCredit").val("0");
            $("#txtCredit").prop('disabled', true);
            $("#allocationDetailsModelform").formValidation('revalidateField', 'Credit');
        }
        else {
            $("#txtCredit").prop('disabled', false);
        }
    });
    $("#txtAccountDesc").on("change", function () {
        $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountDesc');
    });
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    function clearModalForm() {
        $("#txtAlCode").val("");
        $("#txtAlDesc").val("");
        $("#txtAccountCode").val("");
        $("#txtAccountDesc").val("");
        $("#txtDebit").val("");
        $("#txtCredit").val("");
        $("#txtAccNarration").val("");
        selectedRowId = "";
        $("#txtDebit").prop('disabled', false);
        $("#txtCredit").prop('disabled', false);
    }
    $("#allocationDetailsModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            selectedRowId = e.relatedTarget.id;
            var rowId = e.relatedTarget.id;
            var ret = $('#tblAllocDetails').jqGrid('getRowData', rowId);
            $("#txtAlCode").val(ret.AlCode);
            $("#txtAlDesc").val(ret.AlDescription);
            $("#txtAccountCode").val(ret.AccountCode);
            $("#txtAccountDesc").val(ret.Description);
            $("#txtDebit").val(ret.Debit);
            $("#txtCredit").val(ret.Credit);
            $("#txtAccNarration").val(ret.Narration);
        }
    });
    $("#allocationDetailsModel").on('hide.bs.modal', function () {
        clearModalForm();
        $('#tblAllocDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    });
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#allocationDetailsModelform").valid()) {
            e.preventDefault();
            if (selectedRowId) {
                arrAllocDetails[selectedRowId - 1] = {
                    AlCode: $("#txtAlCode").val(), AlDescription: $("#txtAlDesc").val(), AccountCode: $("#txtAccountCode").val(), Description: $("#txtAccountDesc").val(),
                    Debit: $("#txtDebit").val(), Credit: $("#txtCredit").val(), Narration: $("#txtAccNarration").val()
                };
            } else {
                var arrIndex = arrAllocDetails.length;
                arrAllocDetails[arrIndex] = {
                    AlCode: $("#txtAlCode").val(), AlDescription: $("#txtAlDesc").val(), AccountCode: $("#txtAccountCode").val(), Description: $("#txtAccountDesc").val(),
                    Debit: $("#txtDebit").val(), Credit: $("#txtCredit").val(), Narration: $("#txtAccNarration").val()
                };
                //var su = jQuery("#tblAllocDetails").jqGrid('addRowData', arrIndex, arrAllocDetails[arrIndex]);
            }
            clearModalForm();
        }
        else {
            $("#allocationDetailsModelform").formValidation('revalidateField', 'AlCode');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'AlDesc');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountCode');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'AccountDesc');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'Debit');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'Credit');
            $("#allocationDetailsModelform").formValidation('revalidateField', 'Narration');
        }
    });
    $('#formJvCreation').on('init.field.fv', function (e, data) {
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
            DOCNUMBER_BT: {
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
            TotalCreditAmount: {
                validators: {
                    notEmpty: {
                        message: 'Document Details is required'
                    },
                    integer: {
                        message: 'The value is not an integer',
                    },
                    greaterThan: {
                        value: 1,
                    },
                    identical: {
                        field: 'TotalDebitAmount',
                    }
                }
            },
            TotalDebitAmount: {
                validators: {
                    notEmpty: {
                        message: 'Document Details is required'
                    },
                    integer: {
                        message: 'The value is not an integer',
                    },
                    greaterThan: {
                        value: 1,
                    },
                    identical: {
                        field: 'TotalCreditAmount',
                    }
                }
            },
            Difference: {
                validators: {
                    notEmpty: {
                        message: 'Document Details is required'
                    },
                    integer: {
                        message: 'The value is not an integer',
                    },
                    greaterThan: {
                        value: -1,
                    },
                    lessThan: {
                        value: 1,
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
    $("#formJvCreation").formValidation();
});
