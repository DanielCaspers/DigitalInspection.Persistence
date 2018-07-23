var LocationService = /** @class */ (function () {
    function LocationService() {
    }
    LocationService.parseQuery = function () {
        // Search query w/o leading '?'
        var query = window.location.search.substring(1);
        var params = {};
        var queries = query.split("&");
        var i;
        for (i = 0; i < queries.length; i++) {
            var temp = queries[i].split('=');
            params[temp[0]] = temp[1];
        }
        return params;
    };
    LocationService.search = function (queryObj) {
        var queryFragments = [];
        for (var prop in queryObj) {
            if (queryObj.hasOwnProperty(prop)) {
                queryFragments.push(encodeURIComponent(prop) + "=" + encodeURIComponent(queryObj[prop]));
            }
        }
        var query = queryFragments.join('&');
        window.location.search = query;
    };
    return LocationService;
}());
