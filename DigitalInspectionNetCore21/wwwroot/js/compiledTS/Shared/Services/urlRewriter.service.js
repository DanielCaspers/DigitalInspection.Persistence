var UrlRewriterService = /** @class */ (function () {
    function UrlRewriterService() {
    }
    UrlRewriterService.addUrlResourceId = function (formId, resourceId) {
        var formElement = $('#' + formId);
        var routingAction = formElement.attr('href');
        if (formElement.attr('data-url-modified')) {
            var tokenizedURL = routingAction.split('/');
            tokenizedURL.pop(); // Get rid of previous resource
            var baseResource = tokenizedURL.join('/'); // Rebuild without the old ID
            formElement.attr('href', baseResource + '/' + resourceId);
        }
        else {
            formElement.attr('href', routingAction + '/' + resourceId);
            formElement.attr('data-url-modified', 'true');
        }
    };
    return UrlRewriterService;
}());
