$(document).ready(function () {
    $("div.formmessage").slideUp();
    $("#statsalert").hide();
    $("#loadingmessage").hide();
    $("#sellerNotice").hide();
});

$(".submitonchange").on('change', function (event) {
    var form = $(event.target).parents('form');
    form.submit();
});

//Before opening modal to agree to terms, check if form is valid.
$("#validate-register-form").on('click', function () {
    if ($("#register-form").valid()) {
        $("#policy-modal").modal("show");
    }
});

$("#confirm-register-button").on('click', function () {
    $("#register-form").submit();
});

$("#BeSeller").on("click", function () {
    $("#validate-register-form").toggleClass("btn-primary");
    $("#validate-register-form").toggleClass("btn-warning");
    if (this.checked) {
        $("#validate-register-form").html("Become a SELLER!");
        $("#sellerNotice").show();
    } else {
        $("#validate-register-form").html("Become a Reviewer!");
        $("#sellerNotice").hide();
    }
});


$("#amazon-profile-check").on('change', function () {
});

$("button.btn-review").click(function () {
    var asin = $(this).attr("id").replace("Button", "");
    id = "#" + asin + "Form";
    $(id).slideToggle();
});

$("#salepricebox").on('input', function () {
    var rawprice = $("#retailprice").text();
    var price = rawprice.substr(rawprice.indexOf("$") + 1);
    var saleprice = $("#salepricebox").val();
    var final = "$" + ((price - saleprice).toFixed(2));
    $("#discount").html(final);
    var text = $("#textQty").text();
    var photo = $("#photoQty").text();
    var video = $("#videoQty").text();
    var total = Number(text) + Number(photo) + Number(video);
    total = total * (price - saleprice).toFixed(2);
    total = "$" + total.toFixed(2);
    $("#totalDiscount").html(total);
   });

$("#sortOrder").on('change', function (event) {
    var form = $(event.target).parents('form');
    form.submit();
});

$("td").on("click", ".btn-choose-customer", function () {
    $(this).parent("td").html("<a role='button' id='accept' class='btn btn-sm btn-success btn-customer-decision btn-actions'>Accept</a>\
<a role='button' id='deny' class='btn btn-sm btn-danger btn-customer-decision btn-actions'>Decline</a>\
<a role='button' class='btn btn-sm btn-default btn-cancel-customer btn-actions'>Cancel</a>")
});

$("td").on("click", ".btn-cancel-customer", function () {
    $(this).parent("td").html("<a role='button' class='btn btn-sm btn-primary btn-choose-customer'>Select Customer</a>")
});

$("td").on("click", ".btn-apply-code", function () {
    $(this).parent("td").html("<input type='text' name='Code'><a role='button' id='applyCode' class='btn btn-sm btn-primary btn-confirm-code'>Apply</a>")
});

//This will auto-size height of product boxes to be the same after loading.
//Not sure if this would be better approach than fixed height via css.
var heightTallest = Math.max.apply(null, $(".height-fix").map(function () {
    return $(this).outerHeight();
}).get());
$('.carousel-fix').css({ height: heightTallest + 'px' });

$(function () {
    $("button.btn-review-submit").click(function (e) {
        e.preventDefault();
        $(request).prop("disabled", true);
        var id = $(this).attr("id");
        var asin = id.replace("Request", "");
        var userid = "#" + asin + "userid";
        var reviewtype = "#" + asin + "reviewtype";
        var formmessage = "#" + asin + "FormMessage";
        var alert = "#" + asin + "Alert";
        var request = "#" + id;
        var campaignid = "#" + asin + "campaignid";
        var form = "#" + asin + "Form";
        $(request).html("<img src='/images/btn-loading.gif' />");
        $.ajax({
            type: "POST",
            url: "/Product/SubmitItemRequest",
            data: "{'campaignid':'" + $(campaignid).val() + "','userid':'" + $(userid).val() + "', 'reviewtype':'" + $(reviewtype).val() + "', 'asin':'" + asin + "'}",  //Parameter in this function, Is case sensitive and also type must be string
            contentType: "application/json; charset=utf-8",
            dataType: "json"

        }).done(function (data) {
            //Successfully pass to server and get response
            $(request).html("Sent!");
            $(formmessage).slideDown();
            $(form).slideUp();
            console.log(data);
            if (data.result == "OK") {
                $(alert).html("Your request was successfully submitted and will be reviewed as soon as possible!");
            } else if (data.result == "Duplicate") {
                $(alert).toggleClass("alert-success");
                $(alert).toggleClass("alert-danger")
                $(alert).html("You have already requested this deal!");
            } else if (data.result == "Exceed3") {
                $(alert).toggleClass("alert-success");
                $(alert).toggleClass("alert-danger")
                $(alert).html("You are limited to three deals at a time. Please visit your <a href='/Dashboard'>Dashboard</a> to see items you need to reviews.");
            } else if (data.result == "Overdue") {
                $(alert).toggleClass("alert-success");
                $(alert).toggleClass("alert-danger")
                $(alert).html("You currently have OVERDUE reviews. Please visit your <a href='/Dashboard'>Dashboard</a> to see items you need to review.");
            } else if (data.result == "Exceed1") {
                $(alert).toggleClass("alert-success");
                $(alert).toggleClass("alert-danger")
                $(alert).html("You are limited to two deals at a time. Please visit your <a href='/Dashboard'>Dashboard</a> to see items you need to review. After you have "+
                    "reviewed three items, you will be able to receive up to three deals at a time!");
            } else if (data.result == "Approved") {
                var customerMessage = "<p>Congratulations! You have been auto-selected to receive this deal!</p>" + "<p>Discount Code: <b class='alertDiscountCode'>" + data.code + "</b></p>";
                if (data.instructions != null) {
                    customerMessage = customerMessage + "<p>Purchase Instructions: <b class='alertDiscountCode'>" + data.instructions + "</b></p>";
                }
                if (data.customUrl != null) {
                    customerMessage = customerMessage + "<p><a href='" + data.customUrl + "' target='blank'>Click here to purchase immediately!</a>";
                } else {
                    customerMessage = customerMessage + "<p><a href='" + data.defaultUrl + "' target='blank'>Click here to purchase immediately!</a>";
                }
                $(alert).html(customerMessage);
            }
        }).fail(function (response) {
            if (response.status != 0) {
                $(alert).html("Failed: " + response.status + " " + response.statusText);
            }
        });
    });

});

$(function () {
    $("td").on("click", ".btn-customer-decision", function (e) {
        e.preventDefault();
        var accept = $(this).attr("id");
        var id = $(this).parent("td").attr("id");

        $.ajax({
            type: "POST",
            url: "/Campaign/RequestAction",
            data: "{'requestid':'" + id + "','accept':'" + accept + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json"

        }).done(function (data) {
            //Successfully pass to server and get response
            if (data.result == "Selected") {
                $("#" + id).html("Selected - No Code Available!");
                $("#statsalert").slideDown();
            } else if (data.result == "codesent") {
                $("#" + id).html("Discount Code Sent!");
                $("#statsalert").slideDown();
            } else if (data.result == "Denied") {
                $("#" + id).html("Customer Denied!");
                $("#statsalert").slideDown();
            }
        }).fail(function (response) {
            if (response.status != 0) {
                console.log("Failed: " + response.status + " " + response.statusText);
                console.log(response);
            }
        });
    });

});

$(function () {
    $("td").on("click", ".btn-confirm-code", function (e) {
        e.preventDefault();
        var accept = $(this).attr("id");
        var reviewid = $(this).parent("td").attr("id");
        var code = $(this).prev("input").val();

        $.ajax({
            type: "POST",
            url: "/Campaign/ApplyCode",
            data: "{'reviewid':'" + reviewid + "','code':'" + code + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json"

        }).done(function (data) {
            //Successfully pass to server and get response
            if (data.result == "success") {
                $("#" + reviewid).html("Code Applied: " + code);
                $("#statsalert").slideDown();
            } else if (data.result == "fail") {
                $("#" + id).html("Customer Denied!");
                $("#statsalert").slideDown();
            }
        }).fail(function (response) {
            if (response.status != 0) {
                $(alert).html("Failed: " + response.status + " " + response.statusText);
            }
        });
    });

});

// Contact Form

$("#contactformsubmit").on("click", function () {
    var form = $("#contactform");
    var url = form.attr("action");
    var formdata = form.serialize();
    $("#contactformsubmit").hide();
    $("#helpform").hide();
    $("#loadingmessage").show()
    $.post(url, formdata, function (data) {
        $("#helpform").html(data.message);
        $("#helpform").show();
        $("#loadingmessage").hide()
    });
});

$("#sellerformsubmit").on("click", function () {
    var form = $("#sellerform");
    var url = form.attr("action");
    var formdata = form.serialize();
    $("#sellerformdiv").html("<img src='/images/loading.gif' /> - Sending Message");
    $.post(url, formdata, function (data) {
        $("#sellerformdiv").html("<h3>Message Sent! <br/>We will contact you shortly. Thank you!</h3>");
    });
});

$("#generalformsubmit").on("click", function () {
    var form = $("#generalform");
    var url = form.attr("action");
    var formdata = form.serialize();
    $("#generalformdiv").html("<img src='/images/loading.gif' /> - Sending Message");
    $.post(url, formdata, function (data) {
        $("#generalformdiv").html("<h3>Message Sent! <br/>We will contact you shortly. Thank you!</h3>");
    });
});

$("#helpform").on("click", "#ReviewedItem", function () {
    $('#ReviewedItemASIN').prop('disabled', !this.checked);
});
$("#helpform").on("click", "#DiscountNeeded", function () {
    $('#DiscountNeededASIN').prop('disabled', !this.checked);
});

$("#changeid").on("click", function () {
    $('#newid').prop('disabled', !this.checked);
});