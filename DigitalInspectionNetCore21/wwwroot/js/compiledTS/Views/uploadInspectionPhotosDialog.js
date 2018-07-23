var UploadInspectionPhotosDialog = /** @class */ (function () {
    function UploadInspectionPhotosDialog() {
    }
    UploadInspectionPhotosDialog.initialize = function () {
        UploadInspectionPhotosDialog.launchFilePicker();
        $("#fileInput").change(function () {
            UploadInspectionPhotosDialog.previewPhotoUpload(this);
            $("#uploadInspectionPhotosDialog_success").removeAttr("disabled");
        });
    };
    UploadInspectionPhotosDialog.previewPhotoUpload = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#camera-viewfinder').attr('src', e.target.result);
            };
            reader.readAsDataURL(input.files[0]);
        }
    };
    UploadInspectionPhotosDialog.launchFilePicker = function () {
        $('#fileInput').click();
    };
    return UploadInspectionPhotosDialog;
}());
