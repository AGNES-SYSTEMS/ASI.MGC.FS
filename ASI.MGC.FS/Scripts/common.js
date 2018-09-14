toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "showDuration": "300",
    "hideDuration": "10000",
    "timeOut": "50000",
    "extendedTimeOut": "10000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

$(document).ready(function () {
    $("input").keypress(function (e) {
        var key = e.keyCode;
        if (key == 13) //Enter
        {
            console.log("kuch na karo");
            return false;//return true to submit, false to do nothing
        }
    });
});