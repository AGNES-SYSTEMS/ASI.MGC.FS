var arrAllocations = [];
var allocDocSelect = function (docId) {
    if (docId) {
        var ret = jQuery("#tblallocDocSearch").jqGrid('getRowData', docId);
        $("#txtDocNo").val(ret.DOCNUMBER_ART).change();
        $('#allocDocSearchModel').modal('toggle');
    }
};
var processAllocDetail = function (id, isRestored) {
    if (isRestored) {
        restoredRow = arrAllocations.filter(function (el) {
            return el.id !== id;
        });
        arrAllocations = restoredRow;
        $("#hdnAllocDetails").val(JSON.stringify(arrAllocations, null, ' '));
    }
    else {
        var i = 0;
        for (i = 0; i < arrAllocations.length; i++) {
            if (arrAllocations[i]["id"] === id) {
                var ret = jQuery("#tblAllocationDetails").jqGrid('getRowData', id);
                arrAllocations[i]["AMOUNT_ARM"] = $("#" + id + "_Match_Amount").val();
                $("#hdnAllocDetails").val(JSON.stringify(arrAllocations));
                return;
            }
        }
        var ret = jQuery("#tblAllocationDetails").jqGrid('getRowData', id);
        var index = arrAllocations.length;
        arrAllocations[index] = { id: id, DOCCNUMBER_ARM: ret.DocNo, AMOUNT_ARM: $("#" + id + "_Match_Amount").val() };
    }
};
var partySelect = function (partyId) {
    if (partyId) {
        var ret = jQuery("#tblPartyAccountSearch").jqGrid('getRowData', partyId);
        $("#txtPartyAccNo").val(ret.ARCODE_ARM).change();
        $("#txtPartyAccDetails").val(ret.DESCRIPTION_ARM).change();
        $("#formArApMatching").formValidation('revalidateField', 'PartyAccDetails');
        $('#partyAccountSearchModel').modal('toggle');
    }
};
function calculateTotalMatch() {
    var totalMatchVal = 0;
    var totalDocVal = $("#txtTotalDocValue").val();
    for (i = 0; i < arrAllocations.length; i++) {
        totalMatchVal = parseInt(totalMatchVal) + parseInt(arrAllocations[i]["AMOUNT_ARM"]);
    }
    $("#txtTotalMatchValue").val(totalMatchVal);
    $("#txtAllocatedTotal").val(parseInt(totalDocVal) - totalMatchVal);
    $("#formArApMatching").formValidation('revalidateField', 'AllocatedTotal');
    $("#formArApMatching").formValidation('revalidateField', 'AMOUNT_ARM');
};
function editRow(id) {
    var fieldName = "Match_Amount";
    var isRestored = false;
    if (id) {
        var grid = $("#tblAllocationDetails");
        grid.jqGrid('editRow', id, { keys: true });
        $("input[id^='" + id + "_" + fieldName + "']", "#tblAllocationDetails").bind('keyup', function () {
            var matchValSelector = $('#' + id + "_" + fieldName);
            var matchVal = Math.floor(matchValSelector.val());
            matchValSelector.val(matchVal);
            var ret = grid.jqGrid('getRowData', id);
            if (Math.floor(matchVal) > Math.floor(ret.Balance_Amount)) {
                matchValSelector.val(ret.Balance_Amount);
                matchVal = ret.Balance_Amount;
            }
            if (matchVal === "" || parseFloat(matchVal) === 0) {
                grid.jqGrid('restoreRow', id);
                isRestored = true;
            }
            processAllocDetail(id, isRestored);
            calculateTotalMatch();
        });
    }
};
$(document).ready(function () {
    jQuery("#tblAllocationDetails").jqGrid({
        url: '/Finance/GetArMatchAllocationDetails',
        datatype: "json",
        height: 250,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['Date', 'Doc No', 'Amount', 'Balance Amount', 'Match Amount', 'Other Reference'],
        colModel: [
            {
                name: 'Date', index: 'Date', width: 150, align: "center", sortable: false, formatter: 'date', formatoptions: { srcformat: 'd/m/Y', newformat: 'd/m/Y' }
            },
            { name: 'DocNo', index: 'DocNo', width: 150, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "left", sortable: false },
            { name: 'Balance_Amount', index: 'Balance_Amount', width: 150, align: "left", sortable: false },
            {
                name: 'Match_Amount', index: 'Match_Amount', width: 150, align: "left", edittype: 'number', editable: true, editrules: {
                    //custom rules
                    custom_func: validatePositive,
                    custom: true,
                    required: true
                }, sortable: false
            },
            { name: 'OTHERREF_ART', index: 'OTHERREF_ART', width: 500, align: "right", sortable: false }
        ],
        rowNum: 50000,
        multiselect: false,
        onSelectRow: editRow,
        caption: "Product Details"
    });
    $("#btnNew").on("click", function () {
        location.reload();
    });
    function validatePositive(value, column) {
        if (isNaN(value) && value < 0)
            return [false, "Please enter a positive value or correct value"];
        else
            return [true, ""];
    }
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblAllocationDetails').setGridWidth(outerwidth);
    });
    $("#partyAccountSearchModel").on('show.bs.modal', function () {
        $("#tblPartyAccountSearch").jqGrid({
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
                        "%Href%": "href=javascript:partySelect(&apos;" + rowObject.ARCODE_ARM + "&apos;);"
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
            pager: jQuery('#partyPager'),
            caption: "Party A/C List",
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
    $("#allocDocSearchModel").on('show.bs.modal', function () {
        $("#tblallocDocSearch").jqGrid({
            url: '/Finance/GetAllocationDetailsDocNo?partyId=' + $("#txtPartyAccNo").val(),
            datatype: "json",
            height: 250,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Code', 'Description', ''],
            colModel: [
            { key: true, name: 'DOCNUMBER_ART', index: 'DOCNUMBER_ART', width: 400 },
            { key: false, name: 'OTHERREF_ART', index: 'OTHERREF_ART', width: 400 },
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
                        "%Href%": "href=javascript:allocDocSelect(&apos;" + rowObject.DOCNUMBER_ART + "&apos;);"
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
            pager: jQuery('#partyPager'),
            caption: "Allocated Doc Details",
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
    $("#allocDocSearchModel").on('hide.bs.modal', function () {
        var isCredit = false;
        var partyCode = $('#txtPartyAccNo').val();
        var docId = $('#txtDocNo').val();
        var data = JSON.stringify({ partyCode: partyCode, docId: docId });
        $.ajax({
            url: '/Finance/GetPartyAllocationDocumentDetails',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (partyDetails) {
                if (parseFloat(partyDetails.DEBITAMOUNT_ART) !== 0) {
                    var amount = parseFloat(partyDetails.DEBITAMOUNT_ART) - parseFloat(partyDetails.CREDITAMOUNT_ART) - parseFloat(partyDetails.MATCHVALUE_AR);
                    $('#txtAmount').val(amount).change();
                    $('#txtTotalDocValue').val(amount).change();
                    isCredit = false;
                } else if (parseFloat(partyDetails.CREDITAMOUNT_ART) !== 0) {
                    var amount = parseFloat(partyDetails.CREDITAMOUNT_ART) - parseFloat(partyDetails.DEBITAMOUNT_ART) - parseFloat(partyDetails.MATCHVALUE_AR);
                    $('#txtAmount').val(amount).change();
                    $('#txtTotalDocValue').val(amount).change();
                    isCredit = true;
                }
                var allocDocData = $("#tblAllocationDetails").jqGrid("getGridParam", "postData");
                allocDocData["partyId"] = partyCode;
                allocDocData["isCredit"] = isCredit;
                $("#tblAllocationDetails").setGridParam({ postData: allocDocData });
                $("#tblAllocationDetails").trigger("reloadGrid", [{ page: 1 }]);
            },
            complete: function () {
                $("#formArApMatching").formValidation('revalidateField', 'Amount');
                $("#formArApMatching").formValidation('revalidateField', 'TotalDocValue');
            },
            error: function () {
            }
        });
    });
    var searchGrid = function (searchById, searchByName, type) {
        if (type == "1") {
            var partyPostData = $("#tblPartyAccountSearch").jqGrid("getGridParam", "postData");
            partyPostData["custId"] = searchById;
            partyPostData["custName"] = searchByName;
            $("#tblPartyAccountSearch").setGridParam({ postData: partyPostData });
            $("#tblPartyAccountSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
        else if (type == "2") {
            var allocDocPostData = $("#tblallocDocSearch").jqGrid("getGridParam", "postData");
            allocDocPostData["docId"] = searchById;
            $("#tblallocDocSearch").setGridParam({ postData: allocDocPostData });
            $("#tblallocDocSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
    };
    $("#txtPartyIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPartyIdSearch").val().length >= 1 || $("#txtPartyIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPartyIdSearch").val(), $("#txtPartyNameSearch").val(), "1");
        }
    });
    $("#txtPartyNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPartyNameSearch").val().length >= 3 || $("#txtPartyNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPartyIdSearch").val(), $("#txtPartyNameSearch").val(), "1");
        }
    });
    $("#btnPartySelect").on("click", function (e) {
        var id = jQuery("#tblPartyAccountSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblPartyAccountSearch").jqGrid('getRowData', id);
            $("#txtPartyAccNo").val(ret.ARCODE_ARM).change();
            $("#txtPartyAccDetails").val(ret.DESCRIPTION_ARM).change();
            $('#partyAccountSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#txtDocIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtDocIdSearch").val().length >= 1 || $("#txtDocIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtDocIdSearch").val(), null, "2");
        }
    });
    $("#btnAllocDocSelect").on("click", function (e) {
        var id = jQuery("#tblallocDocSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblallocDocSearch").jqGrid('getRowData', id);
            $("#txtDocNo").val(ret.DOCNUMBER_ART).change();
            $('#allocDocSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $('#formArApMatching').on('init.field.fv', function (e, data) {
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
            PartyAccDetails: {
                validators: {
                    notEmpty: {
                        message: 'Party Account No & details are required'
                    }
                }
            },
            Amount: {
                validators: {
                    notEmpty: {
                        message: 'Amount is required'
                    }
                }
            },
            TotalDocValue: {
                validators: {
                    notEmpty: {
                        message: 'Total document value is required'
                    }
                }
            },
            AllocatedTotal: {
                validators: {
                    notEmpty: {
                        message: 'Allocation total is required'
                    }
                }
            },
            AMOUNT_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Total Match Value is required'
                    },
                    lessThan: {
                        value: 'TotalDocValue',
                        message: 'Match value cannot be greater than Doc value'
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