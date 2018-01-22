var glCodeSelect = function (glCodeId) {
    if (glCodeId) {
        var ret = jQuery("#tblGlSearch").jqGrid('getRowData', glCodeId);
        $("#txtGLCode").val(ret.GLCODE_LM).change();
        $("#txtGLDesc").val(ret.GLDESCRIPTION_LM).change();
        $('#glSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    debugger;
    $('#txtGLDate').datepicker();

    function getGlCodeDetails() {
        var glCode = $("#txtGLCode").val();
        if (glCode !== "") {
            var data = JSON.stringify({ glCode: glCode });
            $.ajax({
                url: '/Finance/GetGlCodeDetails',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (glDetails) {
                    debugger;
                    $("#txtGLCode").val(glDetails.glCode).change();
                    $("#txtGLDesc").val(glDetails.glDesc).change();
                    $("#txtSubGLCode").val(glDetails.glSubCode).change();
                    $("#ddlMainCode").val(parseInt(glDetails.glMainCode)).change();
                    if (parseInt(glDetails.glMainCode) === 1 || parseInt(glDetails.glMainCode) === 4) {
                        $("#ddlBalanceType").val(1).change();
                        $("#txtOpenBalance").val(glDetails.debitAmount).change();
                    } else {
                        $("#ddlBalanceType").val(2).change();
                        $("#txtOpenBalance").val(glDetails.creditAmount).change();
                    }
                    $("#txtNote").val(glDetails.glNotes).change();
                    $("#txtGLDate").val(glDetails.glDate).change();
                    $("#ddlGLType").val(glDetails.glType).change();
                },
                complete: function () {
                },
                error: function () {
                }
            });
        }
    }

    $("#btnGLSelect").on("click", function () {
        var id = jQuery("#tblGlSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblGlSearch").jqGrid('getRowData', id);
            $("#txtGLCode").val(ret.GLCODE_LM).change();
            $("#txtGLDesc").val(ret.GLDESCRIPTION_LM).change();
            $('#glSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblGlSearch").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblGlSearch").setGridParam({ postData: postData });
        $("#tblGlSearch").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtglCodeSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtglCodeSearch").val().length >= 2 || $("#txtglCodeSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtglCodeSearch").val());
        }
    });
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $("#glSearchModel").on('show.bs.modal', function () {
        $("#tblGlSearch").jqGrid({
            url: '/Finance/GetGlCodes',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['GL Code', 'GL Details', ''],
            colModel: [
            { key: true, name: 'GLCODE_LM', index: 'GLCODE_LM', width: 250 },
            { key: false, name: 'GLDESCRIPTION_LM', index: 'GLDESCRIPTION_LM', width: 500 },
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
                        "%Href%": "href=javascript:glCodeSelect(&apos;" + rowObject.GLCODE_LM + "&apos;);"
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
            pager: jQuery('#Pager'),
            caption: "Genral Ledger Codes List",
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
    $("#glSearchModel").on('hide.bs.modal', function () {
        getGlCodeDetails();
    });
    $("#txtGLCode").on("change", function () {
        //getGlCodeDetails();
        $("#txtSubGLCode").val($("#txtGLCode").val()).change();
        $("#formGLCreation").formValidation('revalidateField', 'GLCODE_LM');
    });
    $("#ddlMainCode").on("change", function () {
        debugger;
        if (parseInt($("#ddlMainCode").val()) === 1 || parseInt($("#ddlMainCode").val()) === 4) {
            $("#ddlBalanceType").val(1).change();
        } else {
            $("#ddlBalanceType").val(2).change();
        }
    });
    $("#txtSubGLCode").on("change", function () {
        if ($("#txtSubGLDesc").val() === "") {
            $("#txtSubGLDesc").val($("#txtGLDesc").val());
        }
        $("#formGLCreation").formValidation('revalidateField', 'SUBCODE_LM');
    });
    $("#txtGLDesc").on("change", function () {
        $("#formGLCreation").formValidation('revalidateField', 'GLDESCRIPTION_LM');
    });
    $("#txtSubGLDesc").on("change", function () {
        $("#formGLCreation").formValidation('revalidateField', 'SubGLDesc');
    });
    $("#txtGLDate").on("change", function () {
        $("#formGLCreation").formValidation('revalidateField', 'GLDate');
    });
    $('#formGLCreation').on('init.field.fv', function (e, data) {
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
            GLCODE_LM: {
                validators: {
                    notEmpty: {
                        message: 'GL Code is required'
                    }
                }
            },
            GLDESCRIPTION_LM: {
                validators: {
                    notEmpty: {
                        message: 'GL Description is required'
                    }
                }
            },
            SUBCODE_LM: {
                validators: {
                    notEmpty: {
                        message: 'Sub GL Code is required'
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
            SubGLDesc: {
                validators: {
                    notEmpty: {
                        message: 'Sub GL Description is required'
                    }
                }
            },
            OpenBalance: {
                validators: {
                    notEmpty: {
                        message: 'Opening Balance is required'
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