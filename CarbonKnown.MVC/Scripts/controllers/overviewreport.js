;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    controller.$inject = ['$scope',  '$window'];

    function controller($scope, $window) {
        $scope.print = function(printUrl) {
            $window.location.href = printUrl;
        };
        $scope.$watch('census', function (newvalue, oldvalue) {
            if (newvalue === oldvalue) return;
            $window.location.href = $window.location.pathname + '?id=' + newvalue;
        });
    }

    angular
        .module('OverviewReport', ['HoverClass'])
        .controller('Controller', controller);
}));

