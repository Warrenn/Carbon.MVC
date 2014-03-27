;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('PickAColor', [])
        .directive('pickAColor', ['$rootScope', function($rootScope) {
            return {
                restrict: 'A',
                require: '?ngModel',
                scope: {
                    pickAColor: '='
                },
                link: function (scope, element, attr, ngModel) {
                    element.pickAColor(scope.pickAColor);
                    $rootScope.$on('setColor', function (event,data) {
                        $('.color-preview.current-color').css('background-color', '#' + data);
                    });
                }
            };
        }]);
}));
