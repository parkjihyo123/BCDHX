var Url = window.location.origin;
////////////////////////////////
/*
 For changing funtion UserGeneral
 */ 
$("#GeneralChange").on("submit", function (e) {
    e.preventDefault();
    var fullName = $("#GeneralFullNameChange").val();
    var Password = $("#GeneralPasswordChange").val();
    var Address = $("#GeneralAddressChange").val();
   
    var temp = {
        Fullname: fullName,
        Password: Password,
        Address: Address,
       
    }
    $.ajax({
        type: 'POST',
        url: Url + "/Manage/UpdateGeneralProfile",
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
            if (rs.Status == 0 ) {
                $("#SumErrorUpdateGeneral").html("")
                Swal.fire({
                    type: 'success',
                    title: rs.Error,
                    showConfirmButton: false,
                    timer: 1500
                });
                setTimeout(function () {
                    window.location.reload();                  
                }, 2000);

            }
            else if (rs.Status == 1) {
                $("#SumErrorUpdateGeneral").html("<span style='color:red'>" + rs.Error + "</span>")
            }  else {
                $("#SumErrorUpdateGeneral").html("<span style='color:red'>" + rs.Error + "</span>")
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            alert('Error - ' + errorMessage);
        }
    });
});
/*
 * Upload Anh Avatar trong thong tin chung
 */
$("#UploadImageAvatarZone").dxFileUploader({
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
    onUploaded: ReloadPage    
    ,
    multiple: true,
    uploadMethod:
        "POST"
    ,
    name: "ChangeAvatarImage",
    uploadUrl: Url + '/Upload/ProcessChangeAvatar',
    uploadMode: "useButtons",

    allowedFileExtensions: [".jpg", ".jpeg", ".gif", ".png"]
});
function ReloadPage() {
    window.location.reload();
}

$(document).ready(function () {
    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion           
        width: 'auto', //auto or any width like 600px
        fit: true   // 100% fit in a container
    });
});

$("#PaytoAccountForm").on("submit", function (e) {
    e.preventDefault();
    var money = $("#moneyAccount").val(); 
    $.ajax({
        type: 'POST',
        url: Url + "/Manage/UpdateGeneralProfile",
        data: {Money:money},
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
            if (rs.Status == 0) {
                $("#SumErrorUpdateGeneral").html("")
                Swal.fire({
                    type: 'success',
                    title: rs.Error,
                    showConfirmButton: false,
                    timer: 1500
                });
                setTimeout(function () {
                    window.location.reload();
                }, 2000);

            }
            else if (rs.Status == 1) {
                $("#SumErrorUpdateGeneral").html("<span style='color:red'>" + rs.Error + "</span>")
            } else {
                $("#SumErrorUpdateGeneral").html("<span style='color:red'>" + rs.Error + "</span>")
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            alert('Error - ' + errorMessage);
        }
    });
});