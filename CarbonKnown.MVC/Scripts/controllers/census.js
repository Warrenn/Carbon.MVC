;
(function (readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function (window, document, angular, $, undefined) {
    'use strict';

    modalDelete.$inject = ['$scope', '$modalInstance', 'ResourceService', 'census'];

    function modalDelete($scope, $modalInstance, resourceService, census) {
        $scope.census = census;
        $scope.delete = function (censusData) {
            resourceService.delete({ id: censusData.id }, function () {
                $modalInstance.close(censusData);
            }, function () {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function () {
            $modalInstance.dismiss();
        };
    }

    modalUpsert.$inject = ['$scope', '$modalInstance', 'ResourceService', 'census', 'adding'];

    function modalUpsert($scope, $modalInstance, resourceService, census, adding) {
        $scope.census = census;
        $scope.action = adding ? 'add a new' : 'update';
        $scope.upsert = function (censusData) {
            resourceService.upsert(censusData, function () {
                resourceService.census({}, function(data) {
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
        var dateOptions = {
            changeYear: true,
            changeMonth: true,
            constrainInput: true,
            dateFormat: 'dd/mm/yy',
        };

        function resetForm() {
            $scope.adding = true;
            $scope.id = -1;
            $scope.displayName = '';
            $scope.startDate = initialData.startDate;
            $scope.endDate = initialData.endDate;
            $scope.scopeBoundries = '';
            $scope.employeesCovered = 0;
            $scope.totalEmployees = 0;
            $scope.squareMeters = 0;
        }

        resetForm();
        $scope.censusData = resourceService.census();
        $scope.scrollOptions = {
            autoHideScrollbar: true,
            autoDraggerLength: true
        };
        $scope.startDateOptions = angular.extend({}, dateOptions, {
            maxDate: $scope.endDate,
            onClose: function (newdate) {
                $scope.endDateOptions.minDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });
        $scope.endDateOptions = angular.extend({}, dateOptions, {
            minDate: $scope.startDate,
            onClose: function (newdate) {
                $scope.startDateOptions.maxDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });
        $scope.remove = function (census, index) {
            var instance = $modal.open({
                templateUrl: 'ModalDelete.html',
                controller: modalDelete,
                resolve: {
                    census: function () { return census; }
                },
                backdrop: 'static'
            });
            instance.result.then(function () {
                $scope.censusData.splice(index, 1);
            });
        };
        $scope.addNew = function () {
            resetForm();
        };
        $scope.selectCensus = function (census) {
            $scope.adding = false;
            $scope.displayName = census.displayName;
            $scope.startDate = new Date(parseInt(census.startDate.replace("/Date(", "").replace(")/", ""), 10));
            $scope.endDate = new Date(parseInt(census.endDate.replace("/Date(", "").replace(")/", ""), 10));
            $scope.scopeBoundries = census.scopeBoundries;
            $scope.employeesCovered = census.employeesCovered;
            $scope.totalEmployees = census.totalEmployees;
            $scope.squareMeters = census.squareMeters;
            $scope.id = census.id;
        };
        $scope.submitForm = function () {
            var census = {
                id: $scope.id,
                displayName: $scope.displayName,
                startDate: $scope.startDate,
                endDate: $scope.endDate,
                scopeBoundries: $scope.scopeBoundries,
                employeesCovered: $scope.employeesCovered,
                totalEmployees: $scope.totalEmployees,
                squareMeters: $scope.squareMeters
            };
            var instance = $modal.open({
                templateUrl: 'ModalUpsert.html',
                controller: modalUpsert,
                resolve: {
                    census: function () { return census; },
                    adding: function () { return $scope.adding; }
                },
                backdrop: 'static'
            });
            instance.result.then(function (data) {
                $scope.censusData = data;
                resetForm();
            });
        };
        $scope.cancelForm = function () {
            resetForm();
        };
    }

    angular
        .module('Census', ['ngResource', 'ui', 'CustomScroller', 'HoverClass', 'ui.bootstrap'])
        .controller('Controller', controller)
        .factory('ResourceService', ['$resource', 'Urls', function ($resource, urls) {
            return $resource('', {}, {
                census: {
                    method: 'GET',
                    isArray: true,
                    url: urls.census
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

