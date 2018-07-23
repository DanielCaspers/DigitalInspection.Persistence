var DialogService = /** @class */ (function () {
    function DialogService() {
    }
    DialogService.confirmDelete = function (formName) {
        $('#confirmDelete').modal();
        $('#confirmDelete_success').click(function () {
            $('#' + formName).submit();
        });
        // $('#confirmDelete_cancel').click(() => {
        // });
    };
    DialogService.confirm = function (formToSubmitOnSuccess) {
        $('#confirmDialog').modal();
        $('#confirmDialog_success').click(function () {
            $("#" + formToSubmitOnSuccess).submit();
        });
    };
    DialogService.show = function (dialogId, formName, onShow) {
        var dialogElement = $('#' + dialogId);
        var formElement = null;
        var validator;
        if (formName) {
            formElement = $('#' + formName);
            validator = formElement.validate();
        }
        dialogElement.modal();
        $("#" + dialogId + "_success").click(function (e) {
            // Prevent stacking instances of submission if one after another occur without navigating away.
            // EG, submitting '1' returns 1, submitting '2' returns two '2's
            e.preventDefault();
            e.stopImmediatePropagation();
            if (formElement && formElement.valid()) {
                formElement.submit();
                dialogElement.modal('hide');
            }
        });
        dialogElement.on('hidden.bs.modal', function () {
            // Reset values in the form for next open
            if (formElement && formElement[0]) {
                formElement[0].reset();
                validator.resetForm();
            }
            var selectInputs = $('#' + dialogId).find('select');
            for (var _i = 0, selectInputs_1 = selectInputs; _i < selectInputs_1.length; _i++) {
                var selectInput = selectInputs_1[_i];
                $(selectInput).multiselect('deselectAll', false);
                $(selectInput).multiselect('updateButtonText');
            }
        });
        if (onShow) {
            dialogElement.on('shown.bs.modal', onShow);
        }
    };
    DialogService.confirmLeavingUnsavedChanges = function () {
        $(document).ready(function () {
            // This event binding must happen prior to initializing dirtyForms plugin for disabling save buttons
            $('form').on('dirty.dirtyforms clean.dirtyforms scan.dirtyforms', function (ev) {
                var $saveButton = $('#toolbarContainer')
                    .find('i.material-icons')
                    .filter(function ( /* index */) {
                    return this.innerHTML === 'save';
                })
                    .parent();
                if (ev.type === 'dirty') {
                    // Enable save button
                    $saveButton.removeAttr('disabled');
                }
                else {
                    // Disable save button on initialization and on subsequent cleans
                    $saveButton.attr('disabled', 'disabled');
                }
            });
            $('form').not('[ma-dirtyforms-ignore]').dirtyForms({
                dialog: {
                    title: 'Discard changes and leave page?',
                    proceedButtonText: 'Discard',
                    stayButtonText: 'Cancel'
                },
                message: ''
            });
            $(document).bind('defer.dirtyforms', function () {
                // Need to wrap in a timeout to allow click event of navigating away in app to process
                setTimeout(FormService.hideProgress, 10);
            });
            $(document).bind('proceed.dirtyforms', function () {
                if (typeof DialogService.confirmLeavingUnsavedChanges_onProceed === 'function') {
                    DialogService.confirmLeavingUnsavedChanges_onProceed();
                }
                else {
                    FormService.showProgress();
                }
            });
        });
    };
    return DialogService;
}());
// Auto execute to provide this behavior to all forms on load
DialogService.confirmLeavingUnsavedChanges();
