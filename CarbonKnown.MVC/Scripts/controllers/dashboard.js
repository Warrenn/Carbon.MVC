; (function (readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function (window, document, angular, $, undefined) {
    'use strict';

    controller.$inject = ['$scope', 'ResourceService', 'RouteDataService', 'RedirectService', '$route', '$filter', '$window', '$q'];

    function controller($scope, resourceService, routeDataService, redirectService, $route, $filter, $window ,$q) {
        var dateOptions = {
            changeYear: true,
            changeMonth: true,
            constrainInput: true,
            dateFormat: 'dd/mm/yy'
        };

        var scrollOptions = {
            autoHideScrollbar: true,
            updateOnContentResize: true,
            autoDraggerLength: true
        };

        function print(url) {
            var search = redirectService.pathData($scope.request, {}).search;
            var params = angular.extend(search, { dimension: $scope.request.dimension, section: $scope.request.section, selectedSliceId: $scope.selectedSliceId });
            var newUrl = url + '?' + $.param(params);
            $window.location.href = newUrl;
        }

        function setDate(otherDate, newValue, yearDiff) {
            if (($scope.request.endDate - $scope.request.startDate) > 31536000000) {
                otherDate = new Date(newValue.getFullYear() + yearDiff, newValue.getMonth(), newValue.getDate());
            }
            return otherDate;
        }

        function loadChildren(dimension, request, response) {
            var newRequest = angular.extend({}, $scope.request, request.data, { dimension: dimension });
            resourceService.children(newRequest, function (childData) {
                response(childData);
            });
        }

        function redirect(newvalues) {
            redirectService.redirect($scope.request, newvalues);
            if (!$scope.$$phase) $scope.$apply();
        }

        function scrollTo(elmentId) {
            $scope.$emit('ckScroll', elmentId);
        }

        $scope.chart = {};
        $scope.animationLoading = true;
        $scope.loadingSummary = true;
        $scope.loadingGraphData = true;

        $scope.chartConfig = {
            options: {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: true
                },
                tooltip: {
                    enabled: false
                },
                credits: {
                    enabled: false
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        animation: {
                            duration: 500,
                            easing: 'swing',
                            complete: function () {
                                $scope.animationLoading = false;
                                if ((!$scope.chart) || (!$scope.chart.series) || (!$scope.chart.series[0])) return;
                                $scope.chart.series[0].points[0].select();
                            }
                        },
                        point: {
                            events: {
                                mouseOver: function () {
                                    if ($scope.animationLoading) return;
                                    this.select();
                                },
                                select: function () {
                                    if ($scope.animationLoading) return;
                                    var percentage = this.percentage.toFixed(2) + '%';
                                    var name = this.name;
                                    $scope.chartTooltipHeading = name;
                                    $scope.chartTooltipValue = percentage;
                                    if (!$scope.$$phase) $scope.$apply();
                                    $scope.selectedSliceId = this.id;
                                    if ($scope.doScroll) {
                                        $scope.scrollTo('#' + $scope.selectedSliceId);
                                    }
                                    $scope.doScroll = true;
                                },
                                click: function () {
                                    $scope.sliceClicked(this.requestData);
                                }
                            }
                        },
                    }
                }
            },
            loading: $scope.animationLoading,
            title: {
                text: '',
                style: {
                    fontSize: '14px',
                    lineHeight: '26px',
                    color: '#eb7a28',
                    width: '300px'
                }
            },
            series: [{
                type: 'pie',
                innerSize: '75%',
                dataLabels: {
                    enabled: false
                },
            }]
        };

        function reloadController() {
            if (!$route.current) return;
            $scope.summaryData = {};
            $scope.chartTooltipHeading = '';
            $scope.chartConfig.title.text = '';
            $scope.chartTooltipValue = '';
            $scope.chartConfig.series[0].data = [];
            $scope.slices = [];
            $scope.searchOpen = false;
            $scope.animationLoading = true;
            $scope.chartConfig.loading = true;
            $scope.loadingSummary = true;
            $scope.loadingGraphData = true;
            $scope.request = routeDataService($route.current.params);
            $scope.scrollTo = scrollTo;
            $scope.print = print;
            $scope.dateRange = $filter('date')($scope.request.startDate, 'dd/MM/yyyy') + ' - ' + $filter('date')($scope.request.endDate, 'dd/MM/yyyy');
            $scope.scrollOptions = scrollOptions;
            $scope.redirect = redirect;
            resourceService.slice($scope.request, function (data) {
                if (!data.slices.length) {
                    $scope.chartConfig.title.text = 'No recorded ' + data.activityGroup + ' emissions for ' + data.costCentre + ' ' + $scope.dateRange;
                    $scope.chartConfig.series[0].data = [];
                    $scope.summaryData = {};
                    $scope.dateRange = '';
                    $scope.slices = [];
                    $scope.chartConfig.options.now = new Date();
                    $scope.chartConfig.loading = false;
                    $scope.loadingGraphData = false;
                    $scope.loadingSummary = false;
                    return;
                }
                if ((data.slices.length === 1) &&
                    ($scope.request.dimension === 'ActivityGroup') &&
                    (data.slices[0].activityGroupId === $scope.request.activityGroupId)) {
                    redirect({ dimension: 'CostCentre' });
                    return;
                }
                var chartData = [];
                $scope.summaryData = data;
                for (var index = 0; index < data.slices.length; index++) {
                    var sliceData = data.slices[index];
                    chartData.push({
                        id: sliceData.sliceId,
                        y: (data.total <= 0) ? 0 : (sliceData.amount / data.total) * 100,
                        color: '#' + sliceData.color,
                        name: sliceData.title,
                        requestData: { activityGroupId: sliceData.activityGroupId, costCode: sliceData.costCode }
                    });
                }
                if (data.total && $scope.request.dimension === 'ActivityGroup') {
                    $scope.chartConfig.title.text = data.activityGroup + ' carbon emissions by ' + data.costCentre + ' ' + $scope.dateRange;
                }
                if (data.total && $scope.request.dimension === 'CostCentre') {
                    $scope.chartConfig.title.text = data.costCentre + '\'s divisions ' + data.activityGroup + ' emissions ' + $scope.dateRange;
                }

                $scope.slices = data.slices;
                $scope.chartConfig.options.now = new Date();
                $scope.chartConfig.series[0].data = chartData;
                $scope.chartConfig.loading = false;
                $scope.loadingGraphData = false;
                $scope.loadingSummary = false;
            });

            $scope.legendOver = function (index) {
                if ((!$scope.chart) ||
                    (!$scope.chart.series) ||
                    (!$scope.chart.series[0]) ||
                    ($scope.animationLoading)) return;
                $scope.doScroll = false;
                $scope.chart.series[0].points[index].select(true);
            };

            $scope.sliceClicked = function(dataRequest) {
                if ((!dataRequest) ||
                    (($scope.request.dimension === 'ActivityGroup') && (dataRequest.activityGroupId === $scope.request.activityGroupId)) ||
                    (($scope.request.dimension === 'CostCentre') && (dataRequest.costCode === $scope.request.costCode))) {
                    redirect({ dimension: ($scope.request.dimension === 'ActivityGroup') ? 'CostCentre' : 'ActivityGroup' });
                    return;
                }
                if ($scope.request.dimension === 'ActivityGroup') {
                    redirect({ activityGroupId: dataRequest.activityGroupId });
                } else {
                    redirect({ costCode: dataRequest.costCode });
                }
            };

            $scope.startDateOptions = angular.extend({}, dateOptions, {
                maxDate: $scope.request.endDate,
                onClose: function (newdate) {
                    $scope.endDateOptions.minDate = newdate;
                    redirect({ endDate: setDate($scope.request.endDate, $scope.request.startDate, 1) });
                }
            });

            $scope.endDateOptions = angular.extend({}, dateOptions, {
                minDate: $scope.request.startDate,
                onClose: function (newdate) {
                    $scope.startDateOptions.maxDate = newdate;
                    redirect({ startDate: setDate($scope.request.startDate, $scope.request.endDate, -1) });
                }
            });

            var activityRequest = angular.extend({}, $scope.request, { dimension: 'ActivityGroup' });
            resourceService.treeWalk(activityRequest, function (walkData) {
                $scope.activityTreeWalkData = walkData;
            });

            var costCentreRequest = angular.extend({}, $scope.request, { dimension: 'CostCentre' });
            resourceService.treeWalk(costCentreRequest, function (walkData) {
                $scope.costCentreTreeWalkData = walkData;
            });

            $scope.loadActivityChildren = function (request, response) {
                loadChildren('ActivityGroup', request, response);
            };

            $scope.loadCostCentreChildren = function (request, response) {
                loadChildren('CostCentre', request, response);
            };

            $scope.activityNodeSelected = function (event, data) {
                redirect({ dimension: 'ActivityGroup', activityGroupId: data.data.activityGroupId });
            };

            $scope.toggleSearch = function() {
                $scope.searchOpen = !$scope.searchOpen;
            };

            $scope.costCentreNodeSelected = function (event, data) {
                redirect({ dimension: 'CostCentre', costCode: data.data.costCode });
            };
        }

        if (!$route.current) {
            var initialrequest = routeDataService({});
            redirectService.redirect(initialrequest, {});
        }

        $scope.$on('$locationChangeSuccess', function () {
            reloadController();
            $route.reload();
        });
    }

    angular
        .module('Dashboard', [
            'DashboardDirectives',
            'CustomScroller',
            'HoverClass',
            'CrumbSelector',
            'DashboardFilters',
            'ngResource',
            'ngSanitize',
            'ui',
            'ngRoute',
            'ng',
            'highcharts-ng',
            'ngAnimate'])
        .config(['$routeProvider', '$httpProvider', '$locationProvider', 'DashboardUrl', function ($routeProvider, $httpProvider, $locationProvider, dashboardUrl) {
            $httpProvider.defaults.cache = true;
            $locationProvider.html5Mode(true);
            $routeProvider
                .when(dashboardUrl + '/:section/:dimension', {
                    controller: 'Controller',
                    template: '',
                    caseInsensitiveMatch: true
                });
        }])
        .factory('ResourceService', ['$resource', 'ApiUrl', function ($resource, apiUrl) {
            return $resource('', {}, {
                treeWalk: {
                    method: 'GET',
                    cache: true,
                    url: apiUrl + '/treewalk/:dimension',
                    isArray: true
                },
                children: {
                    method: 'GET',
                    cache: true,
                    url: apiUrl + '/treewalk/children/:dimension',
                    isArray: true
                },
                slice: {
                    method: 'GET',
                    cache: true,
                    url: apiUrl + '/slice/:dimension'
                }
            });
        }])
        .factory('RouteDataService', ['InitialRequest', function (initialRequest) {
            return function (routeParams) {
                return angular.extend(initialRequest, routeParams, {
                    startDate: routeParams.startDate ? new Date(routeParams.startDate) : initialRequest.startDate,
                    endDate: routeParams.endDate ? new Date(routeParams.endDate) : initialRequest.endDate
                });
            };
        }])
        .factory('RedirectService', ['$location', 'DashboardUrl', function ($location, dashboardUrl) {
            function createPathData(routeparams, newvalues) {
                var request = angular.extend(routeparams, newvalues, {
                    startDate: newvalues.startDate ? newvalues.startDate : routeparams.startDate,
                    endDate: newvalues.endDate ? newvalues.endDate : routeparams.endDate
                });
                return {
                    search: {
                        costCode: request.costCode,
                        activityGroupId: request.activityGroupId,
                        startDate: request.startDate.toJSON(),
                        endDate: request.endDate.toJSON()
                    },
                    path: dashboardUrl + '/' + request.section + '/' + request.dimension
                };
            }
            return {
                pathData: createPathData,
                redirect: function(routeparams, newvalues) {
                    var data = createPathData(routeparams, newvalues);
                    $location
                        .path(data.path)
                        .search(data.search);
                },
                newPath: function(routeparams, newvalues) {
                    var data = createPathData(routeparams, newvalues);
                    return data.path + '?' + $.param(data.search);
                }
            };
        }])
        .controller('Controller', controller);
}));

