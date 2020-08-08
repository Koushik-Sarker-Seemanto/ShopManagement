
    var total = 0.00;
    var discount = 0.00;
    var array = [];

    $(document).ajaxStart(function () {
        $("#loading").css("display", "block");
    });

    $(document).ajaxComplete(function(){
        $("#loading").css("display", "none");
    });

    $('#inbox').keyup(function (e) {
        if (e.keyCode == 13) {
            var str = $("#inbox").val();

            $.ajax({
        type: "GET",
                url: '@Url.Action("GetProduct", "SellerPanel")',
                contentType: "application/json; charset=utf-8",
                data: {
        id: $('#inbox').val()
                },
                success: function (response) {
                    var json_response = JSON.stringify(response);

                    $('#inbox').val('');
                    if (response.status === "Not Found") {
        alert("Product Not Found");
                    } else if (response.status === "Sold") {
        alert("Already Sold");
                    }
                    else {
        total = (parseFloat(total) + parseFloat(response.res.sellingPrice)).toFixed(1);
                        var trHTML = '';
                        trHTML += '<tr id="' + response.res.id +'" class="text-center"><td>' + response.res.productTitle + '</td><td>' + response.res.sellingPrice + '</td><td><a href="#" style="cursor: pointer" class="deleteIcon fa fa-trash"></i></td></tr>';
                        $('#sellerDataTable').append(trHTML);
                        array.push(response.res.id);
                        $('#total').html(total.toString());
                    }
                },
                error: function (response) {
        alert(response);
                }
            });
        }
    });


    $('#discount').keyup(function(e) {
        if (e.keyCode === 13) {
            var amountToShow = (parseFloat(total) - parseFloat($("#discount").val())).toFixed(1);
            discount =  parseFloat($("#discount").val());
            $('#total').html(amountToShow.toString());
        }
    });

    $('#checkout').click(function () {

        var data = {
        name: $('#name').val(),
            phone: $('#phone').val(),
            order: array
        }
        console.log(array.length);
        var due = "";
        var checkBox = document.getElementById("due");
        if (checkBox.checked === true) {
        due = "Due";
        } else {
        due = "Paid";
        }
        total = (parseFloat(total) - parseFloat(discount).toFixed(1));
        $.ajax({
        type: "POST",
            url: '@Url.Action("SellProduct", "SellerPanel")',
            datatype: "json",
            data: {
        //Passing Input parameter
        name: $('#name').val(),
                phone: $('#phone').val(),
                pay: due,
                total : total ,
                order: JSON.stringify(array)
            },
            success: function (response) {
                if (response.status === "Ok") {
        location.reload(true);
                } else {
        alert("Fail");
                }
            },
            error: function (response) {
        alert("Fail Req");
            }
        });
    });


