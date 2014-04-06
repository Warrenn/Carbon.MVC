;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    controller.$inject = ['$scope', 'ResourceService', 'InitialRequest', '$window', 'ApiUrl'];

    function controller($scope, resourceService, initialRequest, $window, apiUrl) {
        var dateOptions = {
            changeYear: true,
            changeMonth: true,
            constrainInput: true,
            dateFormat: 'dd/mm/yy'
        };

        function extendScope(newvalues) {
            var extended = angular.extend({
                costCode: $scope.costCode,
                activityGroupId: $scope.activityGroupId,
                startDate: $scope.startDate,
                endDate: $scope.endDate
            }, newvalues);
            return {
                costCode: extended.costCode,
                activityGroupId: extended.activityGroupId,
                startDate: extended.startDate.toJSON(),
                endDate: extended.endDate.toJSON(),
                section: 'Overview',
            };
        }

        function setDate(otherDate, newValue, yearDiff) {
            if (($scope.endDate - $scope.startDate) > 31536000000) {
                otherDate = new Date(newValue.getFullYear() + yearDiff, newValue.getMonth(), newValue.getDate());
            }
            return otherDate;
        }

        function redirect(newvalues) {
            var search = extendScope(newvalues);
            $window.location.href = $window.location.pathname + '?' + $.param(search);
        }

        $scope.dataTableSettings = {
            sDom: "frtiS",
            bServerSide: true,
            bProcessing: true,
            sScrollY: "400px",
            sAjaxSource: apiUrl.traceSourceUrl,
            aaSorting: [[1, "desc"]],
            aoColumns: [
                {
                    sName: "NAME",
                    bSortable: true,
                    mRender: function(data, type, full) {
                        return '<a href="' + full[8] + '">' + full[0] + '</a>';
                    }
                },
                { sName: "EDIT_DATE", bSortable: true },
                { sName: "USER_NAME", bSortable: true },
                { sName: "UNITS", bSortable: true },
                { sName: "EMISSIONS", bSortable: true },
                { sName: "COST", bSortable: true },
                {
                    sName: "HANDLER_NAME",
                    bSortable: true,
                    mRender: function(data, type, full) {
                        return '<a href="' + full[7] + '">' + full[6] + '</a>';
                    }
                }
            ]
        };

        $scope.startDateOptions = angular.extend({}, dateOptions, {
            maxDate: $scope.endDate,
            onClose: function(newdate) {
                $scope.endDateOptions.minDate = newdate;
                redirect({ endDate: setDate($scope.endDate, $scope.startDate, 1) });
            }
        });

        $scope.endDateOptions = angular.extend({}, dateOptions, {
            minDate: $scope.startDate,
            onClose: function(newdate) {
                $scope.startDateOptions.maxDate = newdate;
                redirect({ startDate: setDate($scope.startDate, $scope.endDate, -1) });
            }
        });

        $scope.costCode = initialRequest.costCode;
        $scope.activityGroupId = initialRequest.activityGroupId;
        $scope.startDate = initialRequest.startDate;
        $scope.endDate = initialRequest.endDate;

        var currentRequest = extendScope({});
        resourceService.activityTreeWalk(currentRequest, function(walkData) {
            $scope.activityTreeWalkData = walkData;
        });
        resourceService.costCentreTreeWalk(currentRequest, function(walkData) {
            $scope.costCentreTreeWalkData = walkData;
        });

        $scope.loadActivityChildren = function(request, response) {
            var walkRequest = extendScope({ activityGroupId: request.data.activityGroupId });
            resourceService.activityChildren(walkRequest, function(childData) {
                response(childData);
            });
        };

        $scope.loadCostCentreChildren = function(request, response) {
            var walkRequest = extendScope({ costCode: request.data.costCode });
            resourceService.costCentreChildren(walkRequest, function(childData) {
                response(childData);
            });
        };

        $scope.activityNodeSelected = function(event, data) {
            redirect({ activityGroupId: data.data.activityGroupId });
        };

        $scope.costCentreNodeSelected = function(event, data) {
            redirect({ costCode: data.data.costCode });
        };
    }

    angular
        .module('TraceSource', ['CrumbSelector', 'ngResource', 'ui', 'ng'])
        .factory('ResourceService', [
            '$resource', 'ApiUrl', function($resource, apiUrl) {
                return $resource(apiUrl + '/treewalk', {}, {
                    'activityTreeWalk': {
                        method: 'GET',
                        cache: true,
                        isArray: true,
                        url: apiUrl.activityTreeWalk
                    },
                    'activityChildren': {
                        method: 'GET',
                        cache: true,
                        isArray: true,
                        url: apiUrl.activityChildren
                    },
                    'costCentreTreeWalk': {
                        method: 'GET',
                        cache: true,
                        isArray: true,
                        url: apiUrl.costCentreTreeWalk
                    },
                    'costCentreChildren': {
                        method: 'GET',
                        cache: true,
                        isArray: true,
                        url: apiUrl.costCentreChildren
                    }
                });
            }
        ])
        .directive('datatable', function() {
            return {
                restrict: 'A',
                scope: {
                    datatable: '='
                },
                link: function(scope, element) {
                    if (scope.datatable === 'undefinded') return;
                    var options = {};
                    angular.extend(options, scope.datatable);
                    element.dataTable(options);
                }
            };
        })
        .controller('Controller', controller);
}));

