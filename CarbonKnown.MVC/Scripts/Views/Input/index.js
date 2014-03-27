;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';
    $(function() {
        if (!window.$$InitialData) return;
        $('#input-history-table')
            .dataTable({
                sDom: "frtiS",
                bServerSide: true,
                bProcessing: true,
                sScrollY: "400px",
                sAjaxSource: window.$$InitialData.historyUrl,
                aaSorting: [[1, "desc"]],
                aoColumns: [
                    {
                        sName: "NAME",
                        bSortable: true,
                        mRender: function(data, type, full) {
                            return '<a href="' + full[7] + '">' + full[0] + '</a>';
                        }
                    },
                    { sName: "EDIT_DATE", bSortable: true },
                    { sName: "USER_NAME", bSortable: true },
                    { sName: "TYPE", bSortable: true },
                    { sName: "STATUS", bSortable: true },
                    {
                        sName: "ACTION",
                        bSortable: false,
                        mRender: function (data, type, full) {
                            return '<a href="' + full[6] + '">Edit Source</a>';
                        }
                    }
                ]
            });
    });
}));
