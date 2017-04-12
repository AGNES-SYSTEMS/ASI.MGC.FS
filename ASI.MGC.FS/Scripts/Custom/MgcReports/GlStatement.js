var glCodeSelect = function (glCodeId) {
    if (glCodeId) {
        var ret = jQuery("#tblGlSearch").jqGrid('getRowData', glCodeId);
        $("#txtGLCode").val(ret.GLCODE_LM).change();
        $("#txtGLDesc").val(ret.GLDESCRIPTION_LM).change();
        $('#glSearchModel').modal('toggle');
    }
};
$(document).ready(function () {
    $("#txtStartDate").datepicker();
    $("#txtEndDate").datepicker();
    var startDate = "";
    var endDate = "";
    var glCode = "";
    $("#txtGLDesc").on("change", function () {
        $("#formGlStatement").formValidation('revalidateField', 'GlDesc');
    });
    var searchGrid = function (glId, glName) {
        var postData = $("#tblGlSearch").jqGrid("getGridParam", "postData");
        postData["searchById"] = glId;
        postData["searchByName"] = glName;
        $("#tblGlSearch").setGridParam({ postData: postData });
        $("#tblGlSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtGlIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtGlIdSearch").val().length >= 1 || $("#txtGlIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtGlIdSearch").val(), $("#txtGlNameSearch").val());
        }
    });
    $("#txtGlNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtGlNameSearch").val().length >= 3 || $("#txtGlNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtGlIdSearch").val(), $("#txtGlNameSearch").val());
        }
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
    var validateArguments = function () {
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        glCode = $("#txtGLCode").val();
        if (startDate === "" || endDate === "" || glCode === "") {
            return false;
        }
        return true;
    }

    $("#btnReportSubmit").on("click", function () {
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "/Reports/GLStatement.aspx?startDate=" + startDate + "&endDate=" + endDate + "&glCode=" + glCode;
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });

    $('#formGlStatement').on('init.field.fv', function (e, data) {
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
            GlDesc: {
                validators: {
                    notEmpty: {
                        message: 'GL Desc is required'
                    }
                }
            }, StartDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
                    }
                }
            },
            EndDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
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