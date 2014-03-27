;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';
    $(document).ready(function() {
        if (!window.$$InitialData) return;
        var datatables = [];

        $('.error-filter').click(function () {
            var errorType = $(this).data('error-type');
            if (!errorType) return;
            for (var table in datatables) {
                datatables[table].fnFilter(errorType);
            }
        });

        function showPopOut(data, columnIndex, element, css, placement,title) {
            var errors = [];
            for (var index in data) {
                if (data[index].index !== columnIndex) continue;
                errors.push(data[index]);
            }
            if (errors.length > 0) {
                var container = $(element);
                var ul = $('<ul></ul>');
                for (var errorIndex in errors) {
                    var li = ul.append('<li></li>');
                    li.text(errors[errorIndex].Message);
                }
                container.addClass(css);
                container.attr('title', title);
                container.popover({
                    html: true,
                    content: ul.html(),
                    title: 'error',
                    placement: placement,
                    trigger: 'hover',
                    container: 'body'
                });
            }
        }

        var tableDataOptions = {
            sDom: 'frtiS',
            bServerSide: true,
            bProcessing: true,
            sScrollY: '100px',
            aaSorting: [[2, 'asc']],
            fnCreatedRow: function (nRow, aData, iDataIndex) {
                var element = $(nRow);
                element.on('mouseenter', function() {
                    element.addClass('rowhover');
                }).on('mouseleave', function() {
                    element.removeClass('rowhover');
                    $('.popover').remove();
                }).on('click',function() {
                    window.$$EditEntry(aData[0]);
                });

                showPopOut(aData[1], 0, nRow, 'rowerror', 'bottom', 'Data Entry Error');
            },
            aoColumnDefs: [
                {
                    aTargets: [0, 1],
                    bSearchable: false,
                    bVisible: false
                },
                {
                    aTargets: ['sortable'],
                    bSortable: true,
                    fnCreatedCell: function(nTd, sData, oData, iRow, iCol) {
                        showPopOut(oData[1], iCol, nTd, 'cellerror', 'top','Column Value Error');
                    }
                }
            ]
        };
        for (var calculationId in window.$$InitialData) {
            var options = $.extend({}, tableDataOptions, { sAjaxSource: window.$$InitialData[calculationId].url, mapping: window.$$InitialData[calculationId].mapping });
            var datatable = $('#' + calculationId).dataTable(options);
            datatables.push(datatable);
        }
    });
}));
