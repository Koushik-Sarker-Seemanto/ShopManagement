
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

    $("#productDataTable").DataTable({
        "processing": true, // for show progress bar    
        "serverSide": true, // for process server side    
        "filter": true, // this is for disable filter (search box)    
        "orderMulti": false, // for disable multiple column at once    
        "ajax": {
            "url": "/ManagerPanel/Products",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [
            {
                "targets": "_all",
                "className": "text-center",
            },
            {
                "targets": [1, 4],
                "orderable": false,
            },
        ],
        "columns": [
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "stockWarning", "name": "Stock Warning", "autoWidth": true },
            { "data": "sellingPrice", "name": "SellingPrice", "autoWidth": true },
            { "data": "stock", "name": "Stock", "autoWidth": true },
            {
                "render": function (data, type, full, meta) { return '<a class="btn btn-info" href="ManagerPanel/ProductDetails?productId='+full.id+'">Details and Stock</a>'; }
            },
            {
                "render": function (data, type, full, meta) { return '<a class="btn btn-danger" href="ManagerPanel/EditProduct?id=' + full.id + '">Edit Product</a>'; }
            },
        ]

    });
    
})(jQuery);