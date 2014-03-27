;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    controller.$inject = ['$scope',  '$window'];

    function controller($scope, $window) {
        $scope.$watch('month', function (newvalue, oldvalue) {
            if ((newvalue === oldvalue) || (!newvalue) || (!$scope.year)) return;
            $window.location.href = $window.location.pathname + '?month=' + newvalue + '&year=' + $scope.year;
        });
        $scope.$watch('year', function (newvalue, oldvalue) {
            if ((newvalue === oldvalue) || (!newvalue) || (!$scope.month)) return;
            $window.location.href = $window.location.pathname + '?month=' + $scope.month + '&year=' + newvalue;
        });
    }

    angular
        .module('Checklist', [])
        .controller('Controller', controller);
}));

