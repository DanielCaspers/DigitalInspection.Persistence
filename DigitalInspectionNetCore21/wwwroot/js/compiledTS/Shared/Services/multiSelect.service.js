var MultiSelectService = /** @class */ (function () {
    function MultiSelectService() {
    }
    MultiSelectService.show = function (selector, config) {
        $(document).ready(function () {
            var selectedElements = $(selector);
            if (!config) {
                config = MultiSelectService.BASE_MULTISELECT_CONFIG;
            }
            selectedElements.multiselect(config);
        });
    };
    MultiSelectService.BASE_MULTISELECT_CONFIG = {
        buttonWidth: '100%',
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        onDropdownHide: function () {
            $('button.multiselect-clear-filter').click();
        }
    };
    return MultiSelectService;
}());
