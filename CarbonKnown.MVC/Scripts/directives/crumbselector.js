;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('CrumbSelector', [])
        .directive('ckCrumbSelector', function() {
            return {
                restrict: 'E',
                scope: {
                    options: '=',
                    treeWalkData: '=',
                    loadChildren: '=',
                    nodeSelected: '='
                },
                link: function(scope, element) {
                    var options = angular.copy(scope.options);
                    element
                        .crumbselector(options)
                        .bind('crumbselectornodeselected', function(event, data) {
                            if ((!scope.nodeSelected) || (typeof(scope.nodeSelected) !== 'function')) return;
                            scope.$apply(scope.nodeSelected(event, data));
                        });
                    scope.$watch('treeWalkData', function(newWalkData) {
                        element.crumbselector('reflowTreeWalk', newWalkData);
                    });
                    scope.$watch('loadChildren', function(newLoadChildren) {
                        element.crumbselector('option', 'loadChildren', newLoadChildren);
                    });
                }
            };
        });
}));
