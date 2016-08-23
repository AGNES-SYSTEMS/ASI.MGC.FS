$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSearch").addClass("active");
    jQuery("#tblSearchDetails").jqGrid({
        datatype: "local",
        height: 350,
        colNames: ['MRV No', 'Job No', 'Customer Code', 'Customer Name', 'Telephone'],
        colModel: [
            { name: 'MRVNO_MRV', index: 'MRVNO_MRV', width: 120, align: "left", sortable: false },
            { name: 'JOBNO_JM', index: 'JOBNO_JM', width: 120, align: "left", sortable: false },
            { name: 'CUSTOMERCODE_MRV', index: 'CUSTOMERCODE_MRV', width: 120, align: "left", sortable: false },
            { name: 'CUSTOMERNAME_MRV', index: 'CUSTOMERNAME_MRV', width: 150, align: "left", sortable: false },
            { name: 'PHONE_MRV', index: 'PHONE_MRV', width: 120, align: "left", sortable: false }
        ],
        autowidth: true,
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
        var telephone = $('#txtTelephone').val();
        var jobNo = $('#txtJobNo').val();
        var mrvNo = $('#txtMrvNo').val();
        var data = JSON.stringify({ custCode: custCode, custName: custName, telephone: telephone, searchType: $searchType, mrvNo: mrvNo, jobNo: jobNo });
        $.ajax({
            url: '/Search/GetSearchDetails',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (customerDetails) {
                $('#tblSearchDetails').jqGrid('clearGridData');
                for (var i = 0; i < customerDetails.length; i++) {
                    jQuery("#tblSearchDetails").jqGrid('addRowData', i, customerDetails[i]);
                }
            },
            complete: function () {
            },
            error: function () {
            }
        });
    });
    $('#formSearch').formValidation({
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
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});