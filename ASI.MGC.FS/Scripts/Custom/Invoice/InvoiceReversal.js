var invSelect = function (invNo) {
    if (invNo) {
        var ret = jQuery("#tblInvSearch").jqGrid('getRowData', invNo);
        $("#txtInvoiceNo").val(ret.DOCNUMBER_ART);
        getInvDetail();
        $('#invSearchModel').modal('toggle');
    }
};
function getInvDetail() {
    var invCode = $("#txtInvoiceNo").val();
    var data = JSON.stringify({ invNo: invCode });
    $.ajax({
        url: '/Invoice/getPostedInvDetails',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (invDetails) {
            if (invDetails !== null) {
                if (invDetails.STATUS_ART !== "R") {
                    var date = formattedDate(invDetails.DODATE_ART);
                    $("#txtDocDate").val(date);
                    $("#txtOtherRef").val(invDetails.OTHERREF_ART);
                    $("#txtCustomer").val(invDetails.ARAPCODE_ART);
                    $("#txtAmount").val(invDetails.DEBITAMOUNT_ART);
                } else {
                    toastr.warning("Invoice is already reversed.");
                    $("#btnSave").hide();
                }
            } else {
                toastr.warning("Invoice number is invalid.");
            }
        },
        complete: function () {
        },
        error: function () {
        }
    });
};
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
    $("#invSearchModel").on('show.bs.modal', function () {
        jQuery("#tblInvSearch").jqGrid({
            url: '/Invoice/GetPostedInvoices',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Invoice No', 'Description', ''],
            colModel: [
            { key: true, name: 'DOCNUMBER_ART', index: 'DOCNUMBER_ART', width: 400 },
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
                        "%Href%": "href=javascript:invSelect(&apos;" + rowObject.DOCNUMBER_ART + "&apos;);"
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
            sortorder: "DESC",
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
        var outerwidth = $('#invGrid').width();
        $('#tblInvSearch').setGridWidth(outerwidth);
    });
    var searchGrid = function (invNo) {
        var postData = $("#tblInvSearch").jqGrid("getGridParam", "postData");
        postData["invNo"] = invNo;
        $("#tblInvSearch").setGridParam({ postData: postData });
        $("#tblInvSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtInvSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtInvSearch").val().length >= 1 || $("#txtInvSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtInvSearch").val());
        }
    });
    $("#btninvSelect").click(function (e) {
        var id = jQuery("#tblInvSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblInvSearch").jqGrid('getRowData', id);
            $("#txtInvoiceNo").val(ret.DOCNUMBER_ART);
            getInvDetail();
            $('#invSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $('#formInvoiceReversal').on('init.field.fv', function (e, data) {
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
            SALEDATE_SD: {
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
            Customer: {
                validators: {
                    notEmpty: {
                        message: 'Customer Code is required'
                    }
                }
            },
            CASHTOTAL_SD: {
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
        $("#divSaving").show();
    });
});