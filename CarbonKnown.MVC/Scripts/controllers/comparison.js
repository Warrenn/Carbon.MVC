; (function (readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function (window, document, angular, $, undefined) {
    'use strict';

    modalResult.$inject = ['$scope', '$modalInstance', 'ResourceService', '$window', 'InitialData', 'targetType'];

    function modalResult($scope, $modalInstance, resourceService, $window, initialData, targetType) {

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

        $scope.submit = function () {
            resourceService.addSeries({
                    activityId: $scope.activityId,
                    name: $scope.$$childTail.name,
                    costCode: $scope.costCode,
                    target: $scope.$$childTail.target,
                    targetType: targetType,
                }, function(seriesModel) {
                    $modalInstance.close(seriesModel);
                });
        };

        $scope.showSubmit = function() {
            return $scope.$$childTail.name;
        };

        $scope.close = function() {
            $modalInstance.dismiss();
        };

        $scope.loadActivityChildren = function (request, response) {
            resourceService.activityChildren(request.data, function (childData) {
                response(childData);
            });
        };

        $scope.loadCostCentreChildren = function (request, response) {
            resourceService.costCentreChildren(request.data, function (childData) {
                response(childData);
            });
        };

        $scope.activityNodeSelected = function (event, data) {
            $scope.activityId = data.data.activityGroupId;
            $scope.hasTarget = resourceService.hasTarget({ activityId: $scope.activityId, costCode: $scope.costCode, targetType: targetType });
            loadActivityGroup(data.data.activityGroupId);
        };

        $scope.costCentreNodeSelected = function (event, data) {
            $scope.costCode = data.data.costCode;
            $scope.hasTarget = resourceService.hasTarget({ activityId: $scope.activityId, costCode: $scope.costCode, targetType: targetType });
            loadCostCentre(data.data.costCode);
        };

        $scope.costCode = initialData.rootCostCode;
        $scope.hasTarget = resourceService.hasTarget({ activityId: $scope.activityId, costCode: $scope.costCode, targetType: targetType });
        loadCostCentre(initialData.rootCostCode);
        loadActivityGroup(null);
    }

    controller.$inject = ['$scope', 'InitialData', 'ResourceService', '$window', '$filter', '$modal'];

    function controller($scope, initialData, resourceService, $window, $filter, $modal) {
        var dateOptions = {
            changeYear: true,
            changeMonth: true,
            constrainInput: true,
            dateFormat: 'dd/mm/yy'
        };

        function addSeriesToGraph(seriesData) {
            var uom = seriesData.uom ? decodeURIComponent(seriesData.uom) : '';
            if (!$scope.uom) {
                $scope.uom = uom;
            }
            if ($scope.uom !== uom) {
                $scope.uom = 'mulitple units';
            }
            var name = seriesData.target ? seriesData.name + ' - target' : seriesData.name;
            var addSeries = {
                seriesId: seriesData.id,
                name: name,
                tooltip: {
                    valueSuffix: ' ' + uom
                },
                data: []
            };
            resourceService.comparisonData({
                    costCode: seriesData.costCode,
                    activityId: seriesData.activityId,
                    startDate: $scope.startDate.toJSON(),
                    endDate: $scope.endDate.toJSON(),
                    targetType: $scope.targetType,
                    target: seriesData.target
                }, function(sdata) {
                    addSeries.data = sdata.map(function(v) { return v.value; });
                    $scope.chartConfig.series.push(addSeries);
                });
        }

        function reloadGraph(startDate, endDate, targetType) {
            $scope.uom = '';
            $scope.chartConfig = {
                options: {
                    chart: {
                        type: 'column',
                    },
                    tooltip: {
                        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}:</td>' +
                            '<td style="padding:0"><b>{point.y}</b></td></tr>',
                        footerFormat: '</table>',
                        shared: true,
                        valueDecimals: 2,
                        useHTML: true
                    },
                    credits: {
                        enabled: false
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0,
                            events: {
                                legendItemClick: function() {
                                    resourceService.removeSeries({ id: this.options.seriesId }, function() {
                                        reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
                                    });
                                    return false;
                                }
                            }
                        }
                    },
                    xAxis: {
                        min:0,
                        categories: []
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Measurement in '
                        }
                    }
                },
                title: {
                    style: {
                        fontSize: '14px',
                        lineHeight: '26px',
                        color: '#eb7a28',
                        width: '300px'
                    },
                    text: initialData.headings[targetType]  + ' ' + $filter('date')(startDate, 'dd/MM/yyyy') + ' - ' + $filter('date')(endDate, 'dd/MM/yyyy')
                },
                series: []
            };
            resourceService.comparisonChart({
                    startDate: startDate.toJSON(),
                    endDate: endDate.toJSON(),
                    targetType: targetType
                }, function(data) {
                    $scope.chartConfig.options.xAxis.categories = data.categories;
                    for (var index in data.series) {
                        var seriesData = data.series[index];
                        addSeriesToGraph(seriesData);
                    }
                    $scope.chartConfig.options.yAxis.title.text = $scope.chartConfig.options.yAxis.title.text + $scope.uom;
                });
        }

        $scope.uom = '';
        $scope.chartConfig = {};
        $scope.startDate = initialData.startDate;
        $scope.endDate = initialData.endDate;
        $scope.targetType = initialData.targetType;

        $scope.changeType = function(targetType) {
            $scope.targetType = targetType;
            reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
        };

        $scope.startDateOptions = angular.extend({}, dateOptions, {
            maxDate: $scope.endDate,
            onClose: function(newdate) {
                $scope.endDateOptions.minDate = newdate;
                reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
            }
        });

        $scope.print = function(href) {
            var params = {
                startDate: $scope.startDate.toJSON(),
                endDate: $scope.endDate.toJSON(),
                targetType: $scope.targetType
            };
            var url = href + '?' + $.param(params);
            $window.location.href = url;
        };

        $scope.endDateOptions = angular.extend({}, dateOptions, {
            minDate: $scope.startDate,
            onClose: function(newdate) {
                $scope.startDateOptions.maxDate = newdate;
                reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
            }
        });

        $scope.addNewSeries = function() {
            $modal.open({
                templateUrl: 'ModalResult.html',
                controller: modalResult,
                resolve: {
                    targetType : function() {
                        return $scope.targetType;
                    }
                },
                backdrop: 'static'
            }).result
                .then(function() {
                    reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
                });
            return false;
        };

        reloadGraph($scope.startDate, $scope.endDate, $scope.targetType);
    }

    angular
        .module('Comparison', [
            'HoverClass',
            'CrumbSelector',
            'ngResource',
            'ui',
            'ng',
            'highcharts-ng',
            'ui.bootstrap'])
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
                addSeries: {
                    method: 'POST',
                    url: urls.addSeries
                },
                removeSeries: {
                    method: 'POST',
                    url: urls.removeSeries
                },
                hasTarget: {
                    method: 'GET',
                    isArray: false,
                    url: urls.hasTarget
                },
                comparisonChart: {
                    method: 'GET',
                    url: urls.comparisonChart
                },
                comparisonData: {
                    method: 'GET',
                    isArray: true,
                    url: urls.comparisonData
                }
            });
        }])
        .controller('Controller', controller);
}));