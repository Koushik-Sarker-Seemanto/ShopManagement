
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
    
})(jQuery);