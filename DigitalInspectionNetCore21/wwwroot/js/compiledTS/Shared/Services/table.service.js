var TableService = /** @class */ (function () {
    function TableService() {
    }
    TableService.showTable = function (elementId, config, onSelect, onUserSelect) {
        $(document).ready(function () {
            var tableSelector = "#" + elementId;
            var searchInputSelector = tableSelector + "_searchInput";
            var clearSearchSelector = tableSelector + "_clearSearch";
            if (!config) {
                config = TableService.BASE_TABLE_CONFIG;
            }
            var table = $(tableSelector).DataTable(config);
            // TODO: Remove these assertions after migrating all CSHTML scripts to TypeScript
            if (typeof onSelect === 'function') {
                table.on('select', onSelect);
            }
            if (typeof onUserSelect === 'function') {
                table.on('user-select', onUserSelect);
            }
            $(searchInputSelector).on('keyup', function () {
                table.search(this.value).draw();
            });
            $(clearSearchSelector).click(function () {
                $(searchInputSelector).val('');
                table.search(this.value).draw();
            });
        });
    };
    TableService.toggleCheckboxesForColumn = function (index, checkAllCheckbox) {
        var jqSelector = "table tbody tr td:nth-child(" + index + ") input[type=checkbox]";
        // For strict toggling behavior without respect to the check all box
        // var isChecked = !$(jqSelector).prop('checked');
        // Toggles checkbox DIRECTLY related to parent state, and has better handling of indeterminate state
        $(jqSelector).prop('checked', checkAllCheckbox.checked);
    };
    TableService.BASE_TABLE_CONFIG = {
        dom: 't<"container-flex between"lip>',
        lengthMenu: [10, 20, 50, 100],
        columnDefs: [],
        language: {
            info: '_TOTAL_ results',
            infoFiltered: '(filtered from _MAX_)',
            infoEmpty: 'No results',
            lengthMenu: 'Page size _MENU_',
            zeroRecords: ''
        }
    };
    return TableService;
}());
