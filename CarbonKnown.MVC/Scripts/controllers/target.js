;
(function (readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function (window, document, angular, $, undefined) {
    'use strict';

    modalDelete.$inject = ['$scope', '$modalInstance', 'ResourceService', 'target'];

    function modalDelete($scope, $modalInstance, resourceService, target) {
        $scope.target = target;
        $scope.delete = function (targetData) {
            resourceService.delete({ id: targetData.id }, function () {
                $modalInstance.close(targetData);
            }, function () {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function () {
            $modalInstance.dismiss();
        };
    }

    modalUpsert.$inject = ['$scope', '$modalInstance', 'ResourceService', 'target', 'adding'];

    function modalUpsert($scope, $modalInstance, resourceService, target, adding) {
        $scope.target = target;
        $scope.action = adding ? 'add a new' : 'update';
        $scope.upsert = function (targetData) {
            resourceService.upsert(targetData, function () {
                resourceService.targets({}, function (data) {
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
            $scope.initialDate = initialData.initialDate;
            $scope.targetDate = initialData.targetDate;
            $scope.initialAmount = 0;
            $scope.targetAmount = 0;
            $scope.targetType = 'Units';
            $scope.activityGroupId = null;
            $scope.costCode = initialData.rootCostCode;
            $scope.activityGroupName = 'Overview';
            $scope.costCentreName = initialData.rootCostCode;
            $scope.id = -1;
            if (!$scope.$$phase) $scope.$apply();
            loadActivityGroup(null);
            loadCostCentre(initialData.rootCostCode);
        }

        function loadCostCentre(costCode) {
            resourceService.costCentreTreeWalk({ costCode: costCode, section: 'Overview' }, function(walkData) {
                $scope.costCentreTreeWalkData = walkData;
            });
        }

        function loadActivityGroup(activityId) {
            resourceService.activityTreeWalk({ activityGroupId: activityId, section: 'Overview' }, function(walkData) {
                $scope.activityTreeWalkData = walkData;
            });
        }
        
        function mapDataFromMvc(data) {
            return data.map(function(m) {
                m.initialDate = new Date(parseInt(m.initialDate.replace("/Date(", "").replace(")/", ""), 10));
                m.targetDate = new Date(parseInt(m.targetDate.replace("/Date(", "").replace(")/", ""), 10));
                return m;
            });
        }

        $scope.targets = resourceService.targets({}, function(data) {
            $scope.targets = mapDataFromMvc(data);
        });
        $scope.scrollOptions = {
            autoHideScrollbar: true,
            autoDraggerLength: true
        };

        $scope.loadActivityChildren = function(request, response) {
            resourceService.activityChildren(request.data, function(childData) {
                response(childData);
            });
        };
        $scope.loadCostCentreChildren = function(request, response) {
            resourceService.costCentreChildren(request.data, function(childData) {
                response(childData);
            });
        };
        $scope.activityNodeSelected = function(event, data) {
            $scope.activityGroupId = data.data.activityGroupId;
            loadActivityGroup(data.data.activityGroupId);
            $scope.activityGroupName = data.contents;
        };
        $scope.costCentreNodeSelected = function(event, data) {
            $scope.costCode = data.data.costCode;
            loadCostCentre(data.data.costCode);
            $scope.costCentreName = data.contents;
        };
        $scope.startDateOptions = angular.extend({}, dateOptions, {
            maxDate: $scope.targetDate,
            onClose: function(newdate) {
                $scope.endDateOptions.minDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });
        $scope.endDateOptions = angular.extend({}, dateOptions, {
            minDate: $scope.initialDate,
            onClose: function(newdate) {
                $scope.startDateOptions.maxDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });
        $scope.remove = function(target, index) {
            var instance = $modal.open({
                templateUrl: 'ModalDelete.html',
                controller: modalDelete,
                resolve: {
                    target: function() { return target; }
                },
                backdrop: 'static'
            });
            instance.result.then(function() {
                $scope.targets.splice(index, 1);
            });
        };
        $scope.addNew = function() {
            resetForm();
        };
        $scope.selectTarget = function(target) {
            $scope.adding = false;
            $scope.initialDate = target.initialDate;
            $scope.targetDate = target.targetDate;
            $scope.initialAmount = target.initialAmount;
            $scope.targetAmount = target.targetAmount;
            $scope.targetType = target.targetType;
            $scope.activityGroupId = target.activityGroupId;
            $scope.costCode = target.costCode;
            $scope.id = target.id;
            loadActivityGroup(target.activityGroupId);
            loadCostCentre(target.costCode);
        };
        $scope.submitForm = function() {
            var target = {
                id: $scope.id,
                initialDate: $scope.initialDate,
                targetDate: $scope.targetDate,
                initialAmount: $scope.initialAmount,
                targetAmount: $scope.targetAmount,
                targetType: $scope.targetType,
                activityGroupId: $scope.activityGroupId,
                costCode: $scope.costCode,
                activityGroupName: $scope.activityGroupName,
                costCentreName: $scope.costCentreName
            };
            var instance = $modal.open({
                templateUrl: 'ModalUpsert.html',
                controller: modalUpsert,
                resolve: {
                    target: function() { return target; },
                    adding: function() { return $scope.adding; }
                },
                backdrop: 'static'
            });
            instance.result.then(function(data) {
                $scope.targets = mapDataFromMvc(data);
                resetForm();
            });
        };
        $scope.cancelForm = function() {
            resetForm();
        };

        resetForm();
    }

    angular
        .module('Targets', ['ngResource', 'HoverClass', 'ui', 'CrumbSelector', 'CustomScroller', 'ui.bootstrap', 'ui.select2'])
        .controller('Controller', controller)
        .factory('ResourceService', ['$resource', 'Urls', function ($resource, urls) {
            return $resource('', {}, {
                activityChildren: {
                    method: 'GET',
                    cache: true,
                    url: urls.activityChildren,
                    isArray: true
                },
                costCentreChildren: {
                    method: 'GET',
                    cache: true,
                    url: urls.costCentreChildren,
                    isArray: true
                },
                activityTreeWalk: {
                    method: 'GET',
                    cache: true,
                    url: urls.activityTreeWalk,
                    isArray: true
                },
                costCentreTreeWalk: {
                    method: 'GET',
                    cache: true,
                    url: urls.costCentreTreeWalk,
                    isArray: true
                },
                targets: {
                    method: 'GET',
                    isArray: true,
                    url: urls.targets
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

