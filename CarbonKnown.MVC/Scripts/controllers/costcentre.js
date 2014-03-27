;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';
    
    modalDelete.$inject = ['$scope', '$modalInstance', 'ResourceService', 'costCentre'];

    function modalDelete($scope, $modalInstance, resourceService, costCentre) {
        $scope.costCentre = costCentre;
        $scope.delete = function (centre) {
            resourceService.delete({ costCode: centre.costCode }, function() {
                $modalInstance.close(costCentre);
            }, function() {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function() {
            $modalInstance.dismiss();
        };
    }

    modalUpsert.$inject = ['$scope', '$modalInstance', 'ResourceService', 'costCentre', 'addNew'];

    function modalUpsert($scope, $modalInstance, resourceService, costCentre, addNew) {
        $scope.costCentre = costCentre;
        $scope.action = addNew ? 'add a new' : 'update ';
        $scope.upsert = function(centre) {
            resourceService.upsert(centre, function() {
                $modalInstance.close(costCentre);
            }, function() {
                $modalInstance.dismiss();
            });
        };
        $scope.close = function() {
            $modalInstance.dismiss();
        };
    }

    controller.$inject = ['$scope', 'ResourceService', 'InitialRequest', '$window', 'Urls', '$timeout', '$modal'];

    function controller($scope, resourceService, initialRequest, $window, urls, $timeout, $modal) {

        function getCostCode(scopeData) {
            if (!scopeData || typeof(scopeData.itemData) !== 'function') return initialRequest.rootCostCode;
            var data = scopeData.itemData();
            if (!data) return initialRequest.rootCostCode;
            return data.costCode;
        }

        function resetForm() {
            $scope.addNew = true;
            $scope.parentArray = $scope.costCentres;
            $scope.parentCostCode = initialRequest.rootCostCode;
            $scope.costCode = '';
            $scope.name = '';
            $scope.color = '';
            $scope.consumptionTypes = [];
            $scope.description = '';
            $scope.orderId = -1;
        }

        $scope.treeWalk = [];
        $scope.costCentres = resourceService.childCostCentres({ costCode: initialRequest.rootCostCode });
        resetForm();
        $scope.select2ConsumptionTypes = {
            multiple: true,
            simple_tags: true,
            tags: initialRequest.tags
        };
        $scope.select2Currency = {
            ajax:
            {
                url: urls.currencies,
                dataType: 'json',
                results: function(data, page) {
                    return {
                        results: data
                    };
                }
            },
            initSelection: function() {
            }
        };
        $scope.colorOptions = {
            showSpectrum: true,
            showSavedColors: false,
            showAdvanced: false,
            saveColorsPerElement: false,
            fadeMenuToggle: false,
            showHexInput: false,
            showBasicColors: true,
            allowBlank: true
        };
        $scope.options = {
            accept: function(data, sourceItemScope, targetScope, destIndex) {
                var currentParent = getCostCode(sourceItemScope.parentItemScope());
                var newParent = getCostCode(targetScope);
                if (currentParent !== newParent) {
                    resourceService.reParent({
                        costCode: data.costCode,
                        newParent: newParent
                    });
                }
                return true;
            },
            orderChanged: function(scope, sourceItem, sourceIndex, destIndex) {
                resourceService.reOrder({
                    costCode: sourceItem.costCode,
                    index: destIndex
                });
            }
        };
        $scope.enter = function(costCentre) {
            costCentre.entered = true;
            costCentre.canDelete = false;
            resourceService.canDelete({ costCode: costCentre.costCode }, function(data) {
                costCentre.canDelete = data.canDelete;
            });
        };
        $scope.remove = function(scope, costCentre) {
            var index = scope.$index;
            if (index < 0) return false;

            var instance = $modal.open({
                templateUrl: 'ModalDelete.html',
                controller: modalDelete,
                resolve: {
                    costCentre: function() { return costCentre; }
                },
                backdrop: 'static'
            });
            instance.result.then(function() {
                scope.sortableModelValue.splice(index, 1);
            });
        };
        $scope.newSubItem = function(scope, costCentre) {
            var index = scope.$index;
            if (index < 0) return false;
            resetForm();
            $scope.parentArray = costCentre.children;
            $scope.parentCostCode = costCentre.costCode;
            resourceService.treeWalk({
                    costCode: costCentre.costCode,
                    section: 'Overview'
                }, function(data) {
                    $scope.treeWalk = data.reverse();
                });
        };
        $scope.leave = function(costCentre) {
            costCentre.entered = false;
            costCentre.canDelete = false;
        };
        $scope.toggle = function(scope) {
            scope.collapsed = !scope.collapsed;
            if (scope.collapsed) {
                var data = scope.itemData();
                data.children = resourceService.childCostCentres({ costCode: data.costCode });
                scope.entered = false;
            }
        };
        $scope.selectCentre = function(scope, costCentre) {
            resetForm();
            $scope.parentArray = scope.sortableModelValue;
            $scope.addNew = false;
            $scope.costCode = costCentre.costCode;
            $scope.name = costCentre.name;
            $scope.color = costCentre.color;
            $scope.currencyCode = costCentre.currencyCode;
            $scope.consumptionTypes = costCentre.consumptionTypes;
            $scope.description = costCentre.description;
            $scope.orderId = scope.$index;
            $scope.parentCostCode = costCentre.parentCostCode;
            $scope.$emit('setColor', $scope.color);
            resourceService.treeWalk({
                    costCode: costCentre.costCode,
                    section: 'Overview'
                }, function(data) {
                    var reverse = data.slice(1);
                    $scope.treeWalk = reverse.reverse();
                });
        };
        $scope.scrollOptions = {
            autoHideScrollbar: true,
            autoDraggerLength: true
        };
        $scope.submitForm = function() {
            var centre = {
                costCode: $scope.costCode,
                name: $scope.name,
                color: $scope.color,
                currencyCode: $scope.currencyCode,
                consumptionTypes: $scope.consumptionTypes,
                description: $scope.description,
                orderId: $scope.orderId,
                parentCostCode: $scope.parentCostCode
            };

            var instance = $modal.open({
                templateUrl: 'ModalUpsert.html',
                controller: modalUpsert,
                resolve: {
                    costCentre: function () { return centre; },
                    addNew: function () { return $scope.addNew; }
                },
                backdrop: 'static'
            });
            instance.result.then(function() {
                if (!$scope.parentArray) return;
                if ($scope.orderId < 0) {
                    $scope.parentArray.push(centre);
                    return;
                }
                $scope.parentArray[$scope.orderId] = centre;
            });
        };
        $scope.cancelForm = function() {
            resetForm();
        };
    }

    angular
        .module('CostCentre', ['ngResource', 'CustomScroller', 'ui.nestedSortable', 'ui.bootstrap', 'PickAColor', 'ui.select2'])
        .controller('Controller', controller)
        .factory('ResourceService', ['$resource', 'Urls', function($resource, urls) {
            return $resource('', {}, {
                childCostCentres: {
                    method: 'GET',
                    isArray: true,
                    url: urls.childCostCentres
                },
                canDelete: {
                    method: 'GET',
                    url: urls.canDelete
                },
                reOrder: {
                    method: 'PUT',
                    url: urls.reOrder
                },
                reParent: {
                    method: 'PUT',
                    url: urls.reParent
                },
                'delete': {
                    method: 'DELETE',
                    url: urls.delete
                },
                upsert: {
                    method: 'POST',
                    url: urls.upsert
                },
                treeWalk: {
                    method: 'GET',
                    url: urls.treeWalk + '/treewalk/costcentre',
                    isArray: true
                }
            });
        }]);
}));

