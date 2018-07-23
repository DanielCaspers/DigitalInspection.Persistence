var ViewInspectionPhotosDialog = /** @class */ (function () {
    function ViewInspectionPhotosDialog() {
    }
    ViewInspectionPhotosDialog.onImageVisibilityToggle = function (element) {
        var formElement = $(element).closest('form');
        FormService.addUrlParameter(formElement, { name: 'isVisibleToCustomer', value: element.checked });
        formElement.submit();
    };
    ViewInspectionPhotosDialog.deletePhoto = function () {
        var imageId = $('.carousel-inner .item.active img').attr('data-image-id');
        FormService.triggerExternalSubmit('viewInspectionPhotosForm', null, false, { name: 'imageId', value: imageId });
    };
    return ViewInspectionPhotosDialog;
}());
