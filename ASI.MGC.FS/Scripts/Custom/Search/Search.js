$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSearch").addClass("active");
    jQuery("#tblSearchDetails").jqGrid({
        datatype: "local",
        height: 350,
        colNames: ['MRV No', 'Customer Code', 'Customer Name', 'Telephone'],
        colModel: [
            { name: 'MRVNO_MRV', index: 'MRVNO_MRV', width: 120, align: "left", sortable: false },
            { name: 'CUSTOMERCODE_MRV', index: 'CUSTOMERCODE_MRV', width: 120, align: "left", sortable: false },
            { name: 'CUSTOMERNAME_MRV', index: 'CUSTOMERNAME_MRV', width: 250, align: "left", sortable: false },
            { name: 'PHONE_MRV', index: 'PHONE_MRV', width: 120, align: "left", sortable: false }
        ],
        autowidth:true,
        multiselect: false,
        caption: "Search Details"
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblSearchDetails').setGridWidth(outerwidth);
    });
    $("#btnSearch").on("click", function () {
        var $searchType = 0;
        if ($('#ddlSearchType').val() === "other") {
            $searchType = 1;
        }
        var custCode = $('#txtCustCode').val();
        var custName = $('#txtCustName').val();
        var telephone = $('#txtCustName').val();
        var data = JSON.stringify({ custCode: custCode, custName: custName, telephone: telephone, searchType:$searchType });
        $.ajax({
            url: '/Search/GetSearchDetails',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (customerDetails) {
                $('#tblSearchDetails').jqGrid('clearGridData');;
                for (var i = 0; i < customerDetails.Result.length; i++) {
                    jQuery("#tblSearchDetails").jqGrid('addRowData', i, customerDetails.Result[i]);
                }
            },
            complete: function () {
            },
            error: function () {
            }
        });
    });
    $('#formSearch').bootstrapValidator({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Telephone: {
                integer: {
                    message: 'Integer Only'
                }
            }
        }
    });
});