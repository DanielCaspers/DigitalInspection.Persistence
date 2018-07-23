var InspectionTable = /** @class */ (function () {
    function InspectionTable() {
    }
    InspectionTable.initialize = function () {
        InspectionTable.initializeTable();
        InspectionTable.initializeSelect();
        InspectionTable.initializeConditionGroupControls();
        $(InspectionTable.initializeScrollableTabs);
    };
    InspectionTable.onCustomerConcernToggle = function (element) {
        var formElement = $(element).closest('form');
        FormService.addUrlParameter(formElement, { name: 'isCustomerConcern', value: element.checked });
        formElement.submit();
    };
    InspectionTable.onSelect = function (e, dataTableInstance, type, indexes) {
        if (type === 'row') {
            var row = dataTableInstance.row(indexes).node();
            console.log('Selected ' + row, e);
            //window.scrollTo(0, (e.target as any).clientHeight);
            // Removed because not using buttons in toolbar - See #62
            //$('#inspectionToolbar').addClass('row-selected')
            //UrlRewriterService.addUrlResourceId('AddMeasurementDialogButton', row.attributes['data-checklistitem-id'].value);
        }
    };
    InspectionTable.onPreSelect = function (e, dataTableInstance, type, cell, originalEvent) {
        if ($(cell.node()).parent().hasClass('selected')) {
            e.preventDefault();
        }
    };
    InspectionTable.initializeTable = function () {
        InspectionTable.TABLE_CONFIG.select = true,
            InspectionTable.TABLE_CONFIG.paging = false,
            InspectionTable.TABLE_CONFIG.ordering = false;
        InspectionTable.TABLE_CONFIG.info = false;
        InspectionTable.TABLE_CONFIG.lengthMenu = [20];
        InspectionTable.TABLE_CONFIG.columnDefs = [];
        TableService.showTable('inspectionTable', InspectionTable.TABLE_CONFIG, InspectionTable.onSelect, InspectionTable.onPreSelect);
    };
    InspectionTable.initializeSelect = function () {
        $('.selectpicker').selectpicker({
            width: '100px'
        });
    };
    InspectionTable.initializeConditionGroupControls = function () {
        // TODO - WARNING - This might not work with pagination
        $(function () {
            $('.condition-group button.group-left, .condition-group button.group-right').click(function () {
                var sideButton = $(this);
                // Add selected style to side button
                sideButton.addClass('active').siblings().removeClass('active');
                var imposterSelect = sideButton.closest('.condition-group').find('.recommended-service-picker');
                // Remove selected style from button wrapping select
                imposterSelect.removeClass('active');
                // Reset value of select
                imposterSelect.siblings('.selectpicker').selectpicker('val', 0);
            });
            $('.selectpicker').on('loaded.bs.select', function (e) {
                if (e.currentTarget.selectedOptions) {
                    var imposterSelect = $(this);
                    // Add selected style to button wrapping select
                    imposterSelect.siblings('.recommended-service-picker').addClass('active');
                }
            });
            $('.selectpicker').on('changed.bs.select', function () {
                var imposterSelect = $(this);
                // Add selected style to button wrapping select
                imposterSelect.siblings('.recommended-service-picker').addClass('active');
                // Remove selected style from side buttons
                imposterSelect.closest('.condition-group').children('button.group-left, button.group-right').removeClass('active');
                imposterSelect.closest('form').submit();
            });
        });
    };
    InspectionTable.initializeScrollableTabs = function () {
        var config = ScrollableTabService.getConfig(InspectionTable.tabs);
        ScrollableTabService.initialize(config);
    };
    InspectionTable.tabs = [];
    InspectionTable.TABLE_CONFIG = TableService.BASE_TABLE_CONFIG;
    return InspectionTable;
}());
