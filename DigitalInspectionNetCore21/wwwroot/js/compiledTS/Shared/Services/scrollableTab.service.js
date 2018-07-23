var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
;
var ScrollableTabService = /** @class */ (function () {
    function ScrollableTabService() {
    }
    ScrollableTabService.initialize = function (config) {
        $(ScrollableTabService.CONTAINER_SELECTOR)
            .scrollingTabs(config);
    };
    ScrollableTabService.getConfig = function (tabs) {
        return __assign({}, ScrollableTabService.CONFIG, { tabs: tabs });
    };
    ScrollableTabService.buildScrollTabArrow = function (direction) {
        var template = "\n\t\t\t<div class=\"scrtabs-tab-scroll-arrow scrtabs-tab-scroll-arrow-" + direction + "\">\n\t\t\t\t<a class=\"btn btn-primary no-margin no-padding-horizontal\">\n\t\t\t\t\t<i class=\"material-icons\">chevron_" + direction + "</i>\n\t\t\t\t</a>\n\t\t\t</div>\n\t\t";
        return template;
    };
    ScrollableTabService.CONTAINER_SELECTOR = '#ma-scrollable-tab-container';
    ScrollableTabService.CONFIG = {
        tabs: [],
        enableSwiping: true,
        ignoreTabPanes: true,
        scrollToTabEdge: true,
        disableScrollArrowsOnFullyScrolled: true,
        tabClickHandler: function (e) {
            var query = LocationService.parseQuery();
            // The tag ID is after the relative target anchor (#) in the full URL
            query.tagId = this.href.split('#')[1];
            LocationService.search(query);
        },
        leftArrowContent: ScrollableTabService.buildScrollTabArrow('left'),
        rightArrowContent: ScrollableTabService.buildScrollTabArrow('right')
    };
    return ScrollableTabService;
}());
