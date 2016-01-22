//Disable the "Enter" key on Seller/Index page to prevent users from 
//accidentally submitting form prematurely.
$(document).ready(function () {
    $(window).keydown(function (event) {
        if (event.keyCode == 13 && current != null) {
            event.preventDefault();
            return false;
        }
    });
});

$(".reviewQty").on("change", function () {
    UpdateReviewOrder();
});


function UpdateReviewOrder() {
    //When page loaded, the "current" object was loaded with pricing in it.
    var textcount = Number($("#textReviews").val());
    var photocount = Number($("#photoReviews").val());
    var videocount = Number($("#videoReviews").val());

    var textPrice = 0;
    var photoPrice = 0;
    var videoPrice = 0;

    if (textcount > 100) {
        textPrice = textcount * current["T3"];
    }
    else if (textcount > 50) {
        textPrice = textcount * current["T2"];
    } else {
        textPrice = textcount * current["T1"];
    }

    if (photocount > 100) {
        photoPrice = photocount * current["P3"];
    }
    else if (photocount > 50) {
        photoPrice = photocount * current["P2"];
    } else {
        photoPrice = photocount * current["P1"];
    }

    if (videocount > 100) {
        videoPrice = videocount * current["V3"];
    }
    else if (videocount > 50) {
        videoPrice = videocount * current["V2"];
    } else {
        videoPrice = videocount * current["V1"];
    }
    var totalPrice = textPrice + photoPrice + videoPrice;
    
    $("#textReviewsPrice").html("$" + textPrice.toFixed(2));
    $("#photoReviewsPrice").html("$" + photoPrice.toFixed(2));
    $("#videoReviewsPrice").html("$" + videoPrice.toFixed(2));
    $("#totalReviewsPrice").html("$" + totalPrice.toFixed(2));

};