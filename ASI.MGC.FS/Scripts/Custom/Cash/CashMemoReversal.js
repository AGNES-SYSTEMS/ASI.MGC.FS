var cmSelect = function (cmNo) {
    if (cmNo) {
        var ret = jQuery("#tblCMSearch").jqGrid('getRowData', cmNo);
        $("#txtDocNo").val(ret.CASHRVNO_SD);
        getCMDetail();
        $('#cmSearchModel').modal('toggle');
    }
};
function getCMDetail() {
    var cmCode = $("#txtDocNo").val();
    var data = JSON.stringify({ cmNo: cmCode });
    $.ajax({
        url: '/Cash/getPostedCMDetails',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (cmDetails) {
            if (cmDetails !== null) {
                if (cmDetails.STATUS_BT !== "R") {
                    var date = formattedDate(cmDetails.DOCDATE_BT);
                    $("#txtDocDate").val(date);
                    $("#txtOtherRef").val(cmDetails.OTHERREF_BT);
                    $("#txtBankCode").val(cmDetails.BANKCODE_BT);
                    $("#txtAmount").val(cmDetails.DEBITAMOUT_BT);
                } else {
                    clearFields();
                    toastr.warning("Cash Memo is already reversed.");
                }
            } else {
                clearFields();
                toastr.warning("Receipt No is invalid.");
            }
        },
        complete: function () {
        },
        error: function () {
        }
    });
};
function clearFields() {
    $("#txtDocDate").val("");
    $("#txtOtherRef").val("");
    $("#txtBankCode").val("");
    $("#txtAmount").val("");

}
function formattedDate(jsonDate) {
    var dateString = jsonDate.substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    return date = month + "/" + day + "/" + year;
};
$(document).ready(function () {
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $("#cmSearchModel").on('show.bs.modal', function () {
        jQuery("#tblCMSearch").jqGrid({
            url: '/Cash/GetPostedCashMemo',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Receipt No', 'Description', ''],
            colModel: [
            { key: true, name: 'CASHRVNO_SD', index: 'CASHRVNO_SD', width: 400 },
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
                        "%Href%": "href=javascript:cmSelect(&apos;" + rowObject.CASHRVNO_SD + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
            ],
            rowNum: 100,
            rowList: [100, 200, 500, 1000],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#Pager'),
            caption: "Invoice List",
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
        var outerwidth = $('#cmGrid').width();
        $('#tblCMSearch').setGridWidth(outerwidth);
    });
    var searchGrid = function (cmNo) {
        var postData = $("#tblCMSearch").jqGrid("getGridParam", "postData");
        postData["cmNo"] = cmNo;
        $("#tblCMSearch").setGridParam({ postData: postData });
        $("#tblCMSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtCMSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCMSearch").val().length >= 1 || $("#txtCMSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtCMSearch").val());
        }
    });
    $("#btnCMSelect").click(function (e) {
        var id = jQuery("#tblCMSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblCMSearch").jqGrid('getRowData', id);
            $("#txtDocNo").val(ret.cmNo_SD);
            getCMDetail();
            $('#cmSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $('#formCashMemoReversal').on('init.field.fv', function (e, data) {
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
            DOCDATE_BT: {
                validators: {
                    notEmpty: {
                        message: 'Sale Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            BANKCODE_BT: {
                validators: {
                    notEmpty: {
                        message: 'Bank Code is required'
                    }
                }
            },
            Amount: {
                validators: {
                    notEmpty: {
                        message: 'Amount is required'
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