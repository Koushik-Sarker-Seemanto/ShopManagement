
(function($) {
    "use strict";

    let passwordElement = $("#password");
    let confirmElement = $("#confirmPassword");
    
    $("#addEmployeeForm").submit(function(e) {
        let password = passwordElement.val();
        let confirm = confirmElement.val();
        if (password !== confirm){
            e.preventDefault();
            passwordElement.css("border", "red solid 2px");
            $(".passwordError").text("Password not matched");
            confirmElement.css("border", "red solid 2px");
        }
    });
    passwordElement.on("input", function() {
        $("#password").css("border", "");
        $(".passwordError").text("");
        $("#confirmPassword").css("border", "");
    });
    confirmElement.on("input", function() {
        $("#password").css("border", "");
        $(".passwordError").text("");
        $("#confirmPassword").css("border", "");
    });


    $(document).ready(function() {
        $('#managerDataTable').DataTable();
        $('#sellerDataTable').DataTable();
    });
    
    $("#manager-table-header").click(function () {
        $("#manager-table-body").fadeToggle(500);
    });
    $("#seller-table-header").click(function () {
        $("#seller-table-body").fadeToggle(500);
    });
    
})(jQuery);