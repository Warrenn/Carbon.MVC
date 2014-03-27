;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('CustomScroller', [])
        .directive('ckScroll', ['$rootScope', function($rootScope) {
            return {
                restrict: 'A',
                scope: {
                    ckScroll: '='
                },
                link: function(scope, element) {
                    var options = scope.ckScroll;
                    element.mCustomScrollbar(options);
                    scope.$watch(function() { return element.val() || element.text(); }, function(newValue, oldValue, watchScope) {
                        element.mCustomScrollbar('update');
                    });
                    $rootScope.$on('ckScroll', function(event, data) {
                        element.mCustomScrollbar('scrollTo', data);
                    });
                    $rootScope.$on('ckScrollUpdate', function () {
                        element.mCustomScrollbar('update');
                    });
                }
            };
        }]);
}));
