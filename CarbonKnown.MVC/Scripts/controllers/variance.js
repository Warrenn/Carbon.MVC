;
(function (readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function (window, document, angular, $, undefined) {
    'use strict';

    modalDelete.$inject = ['$scope', '$modalInstance', 'ResourceService', 'variance'];

    function modalDelete($scope, $modalInstance, resourceService, variance) {
        $scope.variance = variance;
        $scope.delete = function (varianceData) {
            resourceService.delete({ id: varianceData.id }, function () {
                $modalInstance.close(varianceData);
            }, function () {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function () {
            $modalInstance.dismiss();
        };
    }

    modalUpsert.$inject = ['$scope', '$modalInstance', 'ResourceService', 'variance', 'adding'];

    function modalUpsert($scope, $modalInstance, resourceService, variance, adding) {
        $scope.variance = variance;
        $scope.action = adding ? 'add a new' : 'update';
        $scope.upsert = function (varianceData) {
            resourceService.upsert(varianceData, function () {
                resourceService.variances({}, function (data) {
                    $modalInstance.close(data);
                });
            }, function () {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function () {
            $modalInstance.dismiss();
        };
    }

    controller.$inject = ['$scope', 'ResourceService', 'InitialData', '$window', 'Urls', '$timeout', '$modal'];

    function controller($scope, resourceService, initialData, $window, urls, $timeout, $modal) {

        function resetForm() {
            $scope.adding = true;
            $scope.id = -1;
            $scope.columnName = 'Units';
            $scope.minValue = 0;
            $scope.maxValue = 0;
        }

        resetForm();

        $scope.variances = resourceService.variances();
        $scope.scrollOptions = {
            autoHideScrollbar: true,
            autoDraggerLength: true
        };
        $scope.calculationOptions = {
            data: resourceService.calculations()
        };
        $scope.columnNameOptions = resourceService.columnNames({ calculationId: initialData.calculationId });

        $scope.$watch('calculation', function (v) {
            if (!v || !v.id) return;
            $scope.columnNameOptions = resourceService.columnNames({ calculationId: v.id });
        });
        $scope.remove = function (variance, index) {
            var instance = $modal.open({
                templateUrl: 'ModalDelete.html',
                controller: modalDelete,
                resolve: {
                    variance: function () { return variance; }
                },
                backdrop: 'static'
            });
            instance.result.then(function () {
                $scope.variances.splice(index, 1);
            });
        };
        $scope.addNew = function () {
            resetForm();
        };
        $scope.selectVariance = function (variance) {
            $scope.adding = false;
            $scope.id = variance.id;
            $scope.calculation = variance.calculation;
            $scope.columnName = variance.columnName;
            $scope.minValue = variance.minValue;
            $scope.maxValue = variance.maxValue;
        };
        $scope.submitForm = function () {
            var variance = {
                id: $scope.id,
                calculation: $scope.calculation,
                columnName: $scope.columnName,
                minValue: $scope.minValue,
                maxValue: $scope.maxValue,
            };
            var instance = $modal.open({
                templateUrl: 'ModalUpsert.html',
                controller: modalUpsert,
                resolve: {
                    variance: function () { return variance; },
                    adding: function () { return $scope.adding; }
                },
                backdrop: 'static'
            });
            instance.result.then(function (data) {
                $scope.variances = data;
                resetForm();
            });
        };
        $scope.cancelForm = function () {
            resetForm();
        };
    }

    angular
        .module('Variance', ['ngResource', 'HoverClass', 'CustomScroller', 'ui.bootstrap', 'ui.select2'])
        .controller('Controller', controller)
        .factory('ResourceService', ['$resource', 'Urls', function ($resource, urls) {
            return $resource('', {}, {
                columnNames: {
                    method: 'GET',
                    cache: true,
                    url: urls.columnNames,
                    isArray: true
                },
                calculations: {
                    method: 'GET',
                    cache: true,
                    url: urls.calculations,
                    isArray: true
                },
                variances: {
                    method: 'GET',
                    isArray: true,
                    url: urls.variances
                },
                'delete': {
                    method: 'DELETE',
                    url: urls.delete
                },
                upsert: {
                    method: 'POST',
                    url: urls.upsert
                }
            });
        }]);
}));

