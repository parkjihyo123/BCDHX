﻿let Url = window.location.origin + "/Product";
let UrlAdd = window.location.origin + "/WishListProduct/AddWishList";
let UrlAddToCart = window.location.origin + "/Cart/AddItemToCart";
let page;
let pre;
$("#selectPage,#selectSort").on("change", function () {
    var value = $("#selectPage option:selected").val();
    var valuescrsortby = $("#selectSort option:selected").val();
    $("#scrpagesize").val(value);
    $("#scrsortby").val(valuescrsortby);
    $("#ShowingForm").submit();

});

$(document).ready(function () {
    var scrpagesize = $("#scrpagesize").val();
    var scrsortby = $("#scrsortby").val();
    var selectMode = $("#SelectModeip").val();
    if (scrsortby == "") {
        scrsortby = "def"
    }
    $('#selectPage option[value=' + scrpagesize + ']').prop("selected", true);
    $('#selectSort option[value=' + scrsortby + ']').prop("selected", true);
    var shopProductWrap = $('.shop-product-wrap');
    shopProductWrap.removeClass('grid list').addClass(selectMode);
    $('.nice-select').niceSelect('update');
});

function SelectMode(Grid) {
    if (Grid == 'Grid') {
        $("#SelectModeip").val('grid');
    } else {
        $("#SelectModeip").val('list');
    }
    $("#ShowingForm").submit();
}
function GetCurrentPage(k) {
    return k;
}
function sendRequest(url, method, data) {
    var d = $.Deferred();
    method = method || "GET";
    $.ajax(url, {
        method: method || "GET",
        data: data,
        cache: false,

        xhrFields: { withCredentials: true }
    }).done(function (result) {
        d.resolve(method === "GET" ? result.data : result);
    }).fail(function (xhr) {
        d.reject(xhr.responseJSON ? xhr.responseJSON.Message : xhr.statusText);
    });
    return d.promise();
}

function GetProductIDForAddWishList(id) {
    var d = $.Deferred();
    let productId = id;
    $.ajax(UrlAdd, {
        method: "POST",
        data: { productID: productId },
        cache: false,
        xhrFields: { withCredentials: true }
    }).done(function (result) {
        d.resolve(ShowProcessAddWishList(result));
    }).fail(function (xhr) {
        d.reject(xhr.responseJSON ? xhr.responseJSON.Message : xhr.statusText);
    });
    return d.promise();
}

function ShowProcessAddWishList(rs) {
    if (rs.Status == 1) {
        Swal.fire({
            type: 'success',
            title: rs.Error,
            showConfirmButton: true,
            confirmButtonColor: '#F3D930',
            allowOutsideClick: false,
        }).then((result) => {
            if (result.value) {
                LoadCartAfterAdded();
            }
        })
    } else if (rs.Status == 2 || rs.Status == 3) {
        Swal.fire({
            type: 'error',
            title: rs.Error,
            showConfirmButton: true,
            confirmButtonColor: '#F3D930'
        })
    } else {
        Swal.fire({
            type: 'error',
            title: "Bạn phải đăng nhập mới thêm sản phẩm vào wishList của mình được!",
            showConfirmButton: true,
            confirmButtonColor: '#F3D930'
        }).then((result) => {
            if (result.value) {
                $("#elegantModalForm").modal('show');
               // window.location.href = window.location.origin + "/Account/Login";
            }
        })
    }
}
function AddToCardProductGrid(productidY, quantitY, optionY) {
    var d = $.Deferred();
    let temp = {
        productID: productidY,
        quantity: quantitY,
        option: optionY
    };
    $.ajax(UrlAddToCart, {
        method: "POST",
        data: temp,
        cache: false,
        xhrFields: { withCredentials: true }
    }).done(function (result) {
        d.resolve(showMessAddtoCart(result));
    }).fail(function (xhr) {
        d.reject(xhr.responseJSON ? xhr.responseJSON.Message : xhr.statusText);
    });
    return d.promise();
}
function showMessAddtoCart(rs) {
    if (rs.Status == 1) {
        Swal.fire({
            type: 'success',
            title: rs.Error,
            showConfirmButton: true,
            confirmButtonColor: '#F3D930',
            allowOutsideClick:false
        }).then((result) => {
            if (result.value) {
                LoadCartAfterAdded();
            }
        })
    } else {
        Swal.fire({
            type: 'error',
            title: rs.Error,
            showConfirmButton: true,
            confirmButtonColor: '#F3D930'
        })
    }
}

function SubmitFromAddToCart() {
    $("#AddtoCardProductDetail").submit();
}

$("#AddtoCardProductDetail").on("submit", function (event) {
    event.preventDefault();
    let quantityF = $("#quantityProductDetail").val();
    let productID = $("#ProductID").val();
    let option = $("#OptionVer option:selected").val();
    AddToCardProductGrid(productID, quantityF, option);
})
