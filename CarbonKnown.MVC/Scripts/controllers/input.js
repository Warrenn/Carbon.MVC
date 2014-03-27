;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    modalResult.$inject = ['$scope', '$modalInstance', '$window', 'Urls', 'ResourceService', 'entry', 'manualEntry', 'referenceNotes'];

    function modalResult($scope, $modalInstance, $window, urls, resourceService, entry, manualEntry, referenceNotes) {
        $scope.entry = entry;
        $scope.referenceNotes = referenceNotes;
        $scope.processing = true;
        $scope.manualEntry = manualEntry;
        resourceService.upsert(entry,
            function(dataResult) {
                $scope.processing = false;
                $scope.result = dataResult;
                entry.entryId = dataResult.EntryId;
                if (!manualEntry) {
                    $window.location.href = urls.editSource;
                    return;
                }
            },
            function(err) {
                console.log(err);
                if (entry.entryId) {
                    $window.location.href = urls.editData + '?' + $.param({ entryId: entry.entryId });
                    return;
                }
                $window.location.href = window.location.href;
            });

        $scope.calculate = function(id) {
            if (!id) return;
            resourceService.calculate({ sourceId: id }, function() {
                $modalInstance.close();
                $window.location.href = urls.inputMain;
                return;
            });
        };
        $scope.close = function() {
            $modalInstance.close();
            return;
        };
    }

    modalRevert.$inject = ['$scope', '$modalInstance', '$window', 'ResourceService', 'sourceId'];

    function modalRevert($scope, $modalInstance, $window, resourceService, sourceId ) {
        $scope.sourceId = sourceId;
        $scope.revert = function(id) {
            resourceService.revert({ sourceId: id }, function() {
                $modalInstance.close();
                $window.location.href = $window.location.href;
                return;
            });
        };
        $scope.close = function() {
            $modalInstance.close();
            return;
        };
    }


    controller.$inject = ['$scope', 'ResourceService', 'InitialRequest', '$modal', '$window', 'Urls', '$http', 'TypeCalculationMapping'];

    function controller($scope, resourceService, initialRequest, $modal, $window, urls, $http, typeCalculationMapping) {
        var dateOptions = {
            changeYear: true,
            changeMonth: true,
            constrainInput: true,
            dateFormat: 'dd/mm/yy',
        };

        function changeCalculation(calculationId) {
            if ($scope.entryData.sourceId) return;
            $window.location.href = urls.enterData + '?' + $.param({ calculationId: calculationId });
        }

        function upsertData(dataEntry) {
            $modal.open({
                templateUrl: 'ModalResult.html',
                controller: modalResult,
                resolve: {
                    entry: function() { return dataEntry; },
                    manualEntry: function () { return $scope.manualEntry; },
                    referenceNotes: function () { return $scope.referenceNotes; }
                },
                backdrop: 'static'
            });
            return false;
        }

        angular.extend($scope, initialRequest);
        
        $scope.calculationOptions = function() {
            return window.$$CalculationOptions;
        };

        $scope.createOptions = function(key) {
            var settings = window.$$Select2Options[key];
            var defaultSettings = {
                initSelection: function() {
                }
            };

            function mapItem(item) {
                var newItem = { id: item[settings.id], text: item[settings.text] };
                return angular.extend(item, newItem);
            }

            if (!settings) return {};
            if (settings.minimumInputLength) {
                defaultSettings.minimumInputLength = settings.minimumInputLength;
            }
            if (settings.allowClear) {
                defaultSettings.allowClear = true;
            }

            $scope[key + 'Model'] = null;
            $scope.$watch(key + 'Model', function(newValue, oldValue) {
                if (!newValue) return;
                var idValue = newValue.id;
                if (!idValue) return;
                $scope.entryData[key] = idValue;
            });

            if ((settings.data) && (settings.data.length)) {
                $scope[key + 'Model'] = settings.data[0];
                if (!settings.initialValue) return angular.extend(defaultSettings, { data: settings.data });

                for (var index in settings.data) {
                    if (settings.data[index].id === settings.initialValue) {
                        $scope[key + 'Model'] = settings.data[index];
                        return angular.extend(defaultSettings, { data: settings.data });
                    }
                }
            }

            if (!settings.mapItem) {
                settings.id = (settings.id) ? settings.id : 'id';
                settings.text = (settings.text) ? settings.text : 'text';
                settings.mapItem = mapItem;
            }

            if (!settings.search) {
                settings.search = function(scope, term, page, id) {
                    if (term) return { term: term };
                    return { id: id };
                };
            }

            if ((settings.initialValue) && (settings.initialValue.text === undefined)) {
                $http({
                    url: settings.url,
                    method: "GET",
                    params: settings.search($scope, '', '', settings.initialValue)
                }).success(function(data) {
                    var newItem = settings.mapItem(data[0]);
                    $scope[key + 'Model'] = newItem;
                });
            }

            if ((settings.initialValue) &&
                (settings.initialValue.id) &&
                (settings.initialValue.text)) {
                $scope[key + 'Model'] = settings.initialValue;
            }

            return angular.extend(defaultSettings, {
                ajax:
                {
                    url: settings.url,
                    dataType: 'json',
                    data: function(term, page) { return settings.search($scope, term, page, ''); },
                    results: function(data, page) {
                        return {
                            results: data.map(settings.mapItem)
                        };
                    },
                }
            });
        };

        $scope.cancelForm = function() {
            if ($scope.manualEntry) {
                $window.location.href = urls.inputMain;
                return;
            }
            $window.location.href = urls.editSource;
        };

        $scope.revert = function(id) {
            $modal.open({
                templateUrl: 'Revert.html',
                controller: modalRevert,
                backdrop: 'static',
                resolve: {
                    sourceId: function() {
                        return id;
                    }
                }
            });
        };

        $scope.startDateOptions = angular.extend({}, dateOptions, {
            maxDate: $scope.endDate,
            onClose: function(newdate) {
                $scope.endDateOptions.minDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });
        $scope.endDateOptions = angular.extend({}, dateOptions, {
            minDate: $scope.startDate,
            onClose: function(newdate) {
                $scope.startDateOptions.maxDate = newdate;
                if (!$scope.$$phase) $scope.$apply();
            }
        });

        $scope.submitForm = function(entryData) {
            if (!$scope.canEdit) return;
            if ($scope.transformData) {
                entryData = $scope.transformData(entryData);
            }
            if (!entryData ||
                !entryData.sourceId ||
                (entryData.sourceId === '00000000-0000-0000-0000-000000000000') ||
                (entryData.sourceId === '{00000000-0000-0000-0000-000000000000}')) {
                resourceService.insertManual({ referenceNotes: $scope.referenceNotes, displayType: $scope.displayType }, function (resultData) {
                    angular.extend(entryData, { sourceId: resultData.SourceId });
                    upsertData(entryData);
                });
            } else {
                upsertData(entryData);
            }
        };

        $scope.changeCalculation = changeCalculation;

        $scope.selectConsumptionType = function(type) {
            if (!typeCalculationMapping ||
                !typeCalculationMapping[type]) return;

            changeCalculation(typeCalculationMapping[type]);
        };

        $scope.$watch('entryData.CalculationId', function(newValue, oldValue) {
            if ((!newValue) || (newValue === oldValue)) return newValue;
            changeCalculation(newValue);
        });

        $scope.$watch('FuelTypeModel', function(newValue) {
            if ((!newValue) || (!newValue.value)) return newValue;
            for (var index in newValue.value) {
                if (newValue.value[index].id === $scope.entryData.UOM) return newValue;
            }
            $scope.entryData.UOM = newValue.value[0].id;
        });
    }

    angular
        .module('Input', ['HoverClass', 'ui', 'ngResource', 'ui.bootstrap', 'ui.select2'])
        .controller('Controller', controller)
        .directive('routeValidation', function() {
            return {
                require: 'ngModel',
                link: function(scope, elem, attr, ngModel) {
                    var otherRoute = attr.routeValidation;

                    ngModel.$parsers.unshift(function (value) {
                        var valid = true;
                        if (scope.form[otherRoute] &&
                            scope.form[otherRoute].$viewValue &&
                            scope.form[otherRoute].$viewValue.id &&
                            value &&
                            value.id) {
                            valid = scope.form[otherRoute].$viewValue.id !== value.id;
                            if (valid) scope.form[otherRoute].$setValidity('routeValidation', true);
                        }
                        ngModel.$setValidity('routeValidation', valid);
                        return valid ? value : undefined;
                    });

                    ngModel.$formatters.unshift(function(value) {
                        var valid = true;
                        if (scope.form[otherRoute] &&
                            scope.form[otherRoute].$viewValue &&
                            scope.form[otherRoute].$viewValue.id &&
                            value &&
                            value.id) {
                            valid = scope.form[otherRoute].$viewValue.id !== value.id;
                            if (valid) scope.form[otherRoute].$setValidity('routeValidation', true);
                        }
                        ngModel.$setValidity('routeValidation', valid);
                        return value;
                    });
                }
            };
        })
        .factory('ResourceService', ['$resource', 'Urls', function($resource, urls) {
            return $resource('', {}, {
                'upsert': {
                    method: 'POST',
                    url: urls.upsert
                },
                'calculate': {
                    method: 'POST',
                    url: urls.calculate,
                    params: { sourceId: '@sourceId' }
                },
                'insertManual': {
                    method: 'POST',
                    url: urls.insertManual
                },
                'editData': {
                    method: 'GET',
                    url: urls.editData
                },
                'revert': {
                    method: 'POST',
                    url: urls.revert,
                    params: { sourceId: '@sourceId' }
                }
            });
        }]);
}));

