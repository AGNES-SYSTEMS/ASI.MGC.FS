$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSearch").addClass("active");
    var searchDetails = [];
    jQuery("#tblSearchDetails").jqGrid({
        datatype: "local",
        height: 150,
        colNames: ['MRV No', 'Customer Code', 'Customer Name', 'Telephone'],
        colModel: [
            { name: 'PrCode', index: 'PrCode', width: 120, align: "center", sortable: false },
            { name: 'PrDesc', index: 'PrDesc', width: 120, align: "left", sortable: false },
            { name: 'JobId', index: 'JobId', width: 250, align: "center", sortable: false },
            { name: 'JobDesc', index: 'JobDesc', width: 120, align: "left", sortable: false }
        ],
        autowidth:true,
        multiselect: false,
        caption: "Search Details"
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblSearchDetails').setGridWidth(outerwidth);
    });
    $("#btnSearch").on("click", function() {
        var custCode = $('#txtCustCode').val();
        var custName = $('#txtCustName').val();
        var telephone = $('#txtCustName').val();
        var data = JSON.stringify({ custCode: custCode, custName: custName, telephone: telephone });
        $.ajax({
            url: '/Search/GetSearchDetails',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (searchDetails) {
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