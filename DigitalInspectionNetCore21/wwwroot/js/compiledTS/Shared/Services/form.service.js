var FormService = /** @class */ (function () {
    function FormService() {
    }
    FormService.triggerExternalSubmit = function (formId, resourceId, showProgress, urlParam) {
        var formElement = $('#' + formId);
        if (resourceId) {
            FormService.addUrlResourceId(formId, resourceId);
        }
        if (urlParam) {
            FormService.addUrlParameter(formElement, urlParam);
        }
        formElement.submit();
        if (showProgress) {
            FormService.showProgress();
        }
    };
    FormService.addUrlResourceId = function (formId, resourceId) {
        var formElement = $('#' + formId);
        var routingAction = formElement.attr('action');
        formElement.attr('action', routingAction + '/' + resourceId);
    };
    FormService.submit = function (formId, showProgress) {
        var formElement = $('#' + formId);
        formElement.submit();
        if (showProgress) {
            FormService.showProgress();
        }
    };
    FormService.showProgress = function () {
        $(document).ready(function () {
            $(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: 'visible' });
            var animateloop = function () {
                $(FormService.INNER_PROGRESSBAR_SELECTOR).css({ marginLeft: '-45%' });
                $(FormService.INNER_PROGRESSBAR_SELECTOR).animate({
                    marginLeft: '145%'
                }, 1200, function () { animateloop(); });
            };
            animateloop();
        });
    };
    FormService.hideProgress = function () {
        $(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: 'hidden' });
        $(FormService.INNER_PROGRESSBAR_SELECTOR).stop();
    };
    // TODO Move to the urlRewriterService
    /**
     * @param formElement - A jQuery wrapped form tag
     * @param urlParam
     */
    FormService.addUrlParameter = function (formElement, urlParam) {
        var routingAction = formElement.attr('action');
        var query = "";
        // If query parameter is missing, begin the query parameter list
        if (routingAction.indexOf('?') === -1) {
            query += '?';
        }
        // Else, tack on one add'l parameter to existing query string
        else {
            query += '&';
        }
        query += urlParam.name + "=" + urlParam.value;
        formElement.attr('action', routingAction + query);
    };
    FormService.OUTER_PROGRESSBAR_SELECTOR = '.ma-progress-bar';
    FormService.INNER_PROGRESSBAR_SELECTOR = '.ma-progress-bar .progress-bar[role="progressbar"]';
    return FormService;
}());
