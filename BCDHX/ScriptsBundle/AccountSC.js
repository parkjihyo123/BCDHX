var Url = window.location.origin;

$("#uploadimge").dxFileUploader({
    selectButtonText: "Chọn Ảnh",
    labelText: "Có thể kéo thả",
    invalidFileExtensionMessage: "Không đúng định dạng file ảnh hỗ trợ!",
    readyToUploadMessage: "Sẵn sàng để up ảnh",
    uploadButtonText:
        "Up Ảnh"
    ,
    uploadedMessage:
        "Upload thành công!"
    ,
    uploadFailedMessage:
        "Upload thất bại!"
    ,
    multiple: true,
    uploadMethod:
        "POST"
    ,
    name: "ImageUp",
    uploadUrl: Url + '/Upload/UploadImage',
    uploadMode: "useButtons",

    allowedFileExtensions: [".jpg", ".jpeg", ".gif", ".png"]

});
////////////Create form with devexpress
//var dataForm = {
//    Email: "",
//    Password: "",
//    Repassword: "",
//    Address: "",
//    Fullname: "",
//    Repassword: ""
//};
//var formWidget = $("#form").dxForm({
//    formData: dataForm,
//    readOnly: false,
//    showColonAfterLabel: true,
//    showValidationSummary: true,
//    items: [{
//        dataField: "Email",
//        editorType: "dxTextBox",
//        editorOptions: {
//            //placeholder:"Email",
//            name: "Email"
//        },
//        validationRules: [{
//            type: "required",
//            message: "Phải nhập email"
//        }, {
//            type: "email",
//            message: "Vui Lòng Nhập Địa Chỉ Email Đúng !"
//        }]
//    },
//    {
//        dataField: "Mật Khẩu",
//        editorType: "dxTextBox",
//        editorOptions: {

//            mode: "password",
//            name: "Password"
//        },
//        validationRules: [{
//            type: "required",
//            message: "Phải Nhập Mật Khẩu!"
//        }, {
//            type: "pattern",
//            pattern: "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})",
//            message: "Mật khẩu phải chứa ít nhất một kí tự in hoa,gồm số và kí tự đặc biệt.Độ dài mật khẩu lớn hơn 8"
//        }
//        ]
//        },
//    {
//        dataField: "Nhập lại mật khẩu",
//        editorType: "dxTextBox",
//        editorOptions: {
//            mode: "password",
//            name: "Repassword"
//        },
//        validationRules: [{
//            type: "required",
//            message: "Phải nhập lại mật khẩu!"
//        }
//        ]
//    },
//    {
//        dataField: "Họ và tên",
//        editorType: "dxTextBox",
//        editorOptions: {
//            //placeholder:"Email",
//            name: "Fullname"
//        },
//        validationRules: [{
//            type: "required",
//            message: "Phải nhập họ và tên!"
//        }]
//    },
//    {
//        dataField: "Địa Chỉ",
//        editorType: "dxTextBox",
//        editorOptions: {
//            //placeholder:"Email",
//            name: "Address"
//        },
//        validationRules: [{
//            type: "required",
//            message: "Phải nhập vào địa chỉ!"
//        }]
//    }
//    ]
//}).dxForm("instance");
///////////////////////////////////////

////////////Validation of things

jQuery.validator.addMethod("validateEmail", function (value, element) {
    return this.optional(element) || /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i.test(value);
}, "Email phải đúng định dạng!");
jQuery.validator.addMethod("validatePassowrd", function (value, element) {
    return this.optional(element) || /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})/i.test(value);
}, "Mật khẩu phải chứa ít nhất một kí tự in hoa,gồm số và kí tự đặc biệt.Độ dài mật khẩu lớn hơn 8!");
/////////////////////////////////////

///////////bấm nút đăng ký 
$("#dangkyform").validate({
    rules: {
        Fullname: "required",
        Email: {
            required: true,
            validateEmail: true
        },
        Password: {
            required: true,
            validatePassowrd: true
        },
        Repassword: {
            equalTo: "#Password",
            required: true,

        },
        Address: {
            required: true,
            minlength: 5
        }
    },
    messages: {
        Fullname: "Hãy cho chúng tôi biết họ và tên!",
        Email: {
            required: "Hãy điền vào email của bạn!"
        },
        Address: {
            required: "Hãy cho chúng tôi biết địa chỉ của bạn!",
            minlength: "Địa chỉ phải rõ ràng một chút!"
        },
        Password: {
            required: "Xin nhập vào password!"
        },
        Repassword: {
            required: "Xin nhập lại password!",

            equalTo: "Mật khẩu không trùng nhau!"
        }
    },

    submitHandler: function (form) {
        //form.submit();
        Dangky();

    }
});
function Dangky() {
    var fullname = $("input[name='Fullname']").val();
    var email = $("input[name='Email']").val();
    var address = $("input[name='Address']").val();
    var password = $("input[name='Password']").val();
    var object = {
        Username: email,
        Password: password,
        Fullname: fullname,
        Address: address,
    }
    $.ajax({
        type: 'POST',
        url: Url + "/Account/Register",
        data: JSON.stringify(object),
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            // setting a timeout
            $('body').loadingModal({
                position: 'auto',
                text: '',
                color: '#fff',
                opacity: '0.7',
                backgroundColor: 'rgb(0,0,0)',
                animation: 'circle'
            });
        },
        success: function (rs) {
            $('body').loadingModal('destroy');
            //alert(rs)
            if (rs == 1) {
                $("#sumerror").html("")
                Swal.fire({
                    type: 'success',
                    title: 'Đăng ký thành công, xin kiểm tra lại email để kích hoạt tài khoản ,để chính thức thành khách hàng quen thuộc của shop nhé!',
                    showConfirmButton: true,

                }).then((result) => {
                    if (result.value) {
                        window.location.href = Url + "/Account/Login";
                    }
                })
                //setTimeout(function () {
                //    window.location.href = Url + "/Account/Login";
                //}, 2000);
            }
            else {
                $("#sumerror").html("<span style='color:red'>" + rs + "</span>")
            }
        }, error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            alert('Error - ' + errorMessage);
        }
    });
}


/////////////////////////
//////////////////////Login progess

$("#LoginForm").on("submit", function myLogin(e) {
    e.preventDefault();
    
    var username = $("#Username").val();
    var password = $("#Password").val();
    var remmber = $("input[type='checkbox']").val();
    var temp =  {
        Email : username,
        Password: password,
        RememberMe: remmber
    }
    $.ajax({
        type: 'POST',
        url: Url + "/Account/Login",
        data: JSON.stringify(temp),
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            // setting a timeout
            $('body').loadingModal({
                position: 'auto',
                text: '',
                color: '#fff',
                opacity: '0.7',
                backgroundColor: 'rgb(0,0,0)',
                animation: 'circle'
            });
        },
        success: function (rs) {
            $('body').loadingModal('destroy');
            //alert(rs)
            if (rs.Status == 0 && rs.Error == "Done") {
                $("#sumerror").html("")
                Swal.fire({
                    type: 'success',
                    title: 'Đăng nhập thành công!',
                    showConfirmButton: false,
                    timer: 1500
                });
                setTimeout(function () {
                    window.location.href = Url + rs.ReturnUrl;
                }, 2000);
            }
            else if (rs.Status == 1) {
                $("#sumerror").html("<span style='color:red'>" + rs.Error + "</span>")
            } else if (rs.Status == 2 ) {
                $("#sumerror").html("<span style='color:red'>Tài khoản cần phải kích hoạt để trở thành khách hàng thân thiết của shop, xin lỗi vì sự bất tiện này. Xin kiểm tra lại hòm thư email để kích hoạt tài khoản!.Nếu không nhận được thư thì bấm vào đây để gửi lại:</span>"+"<a style='float:none' herf='#'onClick='return showModelReSend();'>Click!</a>")
            } else {
                $("#sumerror").html("<span style='color:red'>" + rs.Error + "</span>")
            }
        }, error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            alert('Error - ' + errorMessage);
        }
    });
});
function showModelReSend() {
    
    $('#ResendConfirmEmail').modal('show');
}
/////////////////////