var TabService = /** @class */ (function () {
    function TabService() {
    }
    TabService.changeTab = function (event) {
        // Remove selected state from the child that has it
        $('#tabContainer a.btn').removeClass(TabService.tabClasses);
        var sourceId = event.srcElement.id;
        // Add selected state to clicked tab
        $("#" + sourceId).addClass(TabService.tabClasses);
    };
    TabService.selectTab = function (tabId) {
        $("#" + tabId).addClass(TabService.tabClasses);
    };
    TabService.tabClasses = 'btn-info btn-raised';
    return TabService;
}());
