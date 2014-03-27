;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('HoverClass', [])
        .directive('hoverClass', function() {
            return {
                restrict: 'A',
                scope: {
                    hoverClass: '@'
                },
                link: function(scope, element) {
                    element.on('mouseenter', function() {
                        element.addClass(scope.hoverClass);
                    });
                    element.on('mouseleave', function() {
                        element.removeClass(scope.hoverClass);
                    });
                }
            };
        });
}));
