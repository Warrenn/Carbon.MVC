;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    modalResult.$inject = ['$scope', '$modalInstance', 'ResourceService', '$window' ,'files', '$q'];

    function modalResult($scope, $modalInstance, resourceService, $window ,files, $q) {
        var promises = [];
        $scope.showClose = false;
        $scope.fileNames = files.map(function(item) {
            return item.name;
        });
        for (var index in files) {
            var upload = files[index];
            var promise = resourceService.extractData({
                sourceId: upload.sourceId
            });
            promises.push(promise);
        }
        
        $q.all(promises).then(function () {
            $scope.showClose = true;
        });
        
        $scope.close = function() {
            $modalInstance.close();
            $window.location.href = $window.location.pathname;
            return;
        };
    }

    controller.$inject = ['$scope', '$upload', 'Urls', 'ResourceService', '$modal', 'FileTypes', '$q', '$timeout', '$window'];

    function controller($scope, $upload, urls, resourceService, $modal, fileTypes, $q, $timeout, $window) {

        function uploadFile(uploadData, file) {
            uploadData.upload = $upload.upload({
                url: urls.upload,
                file: file,
                method: 'POST',
                data: {
                    referenceNotes: $scope.referenceNotes,
                    fileHandler: $scope.fileHandler.handler,
                    mediaType: file.type
                }
            }).then(function (response) {
                $timeout(function () {
                    var data = response.data;
                    var sourceId = data.SourceId;
                    uploadData.isBusy = false;
                    uploadData.sourceId = sourceId;
                    uploadData.hasErrors = !data.Succeeded;
                    uploadData.errors = data.ErrorMessages.slice(0);
                    if ($scope.sourceIds.indexOf(sourceId) >= 0) {
                        uploadData.hasErrors = true;
                        uploadData.errors.push('Duplicate File');
                        uploadData.sourceId = null;
                    } else {
                        $scope.sourceIds.push(sourceId);
                    }
                });
            }, function(response) {
                $timeout(function () {
                    uploadData.isBusy = false;
                    uploadData.hasErrors = true;
                    uploadData.errors.push(response.status);
                    if (response.Data) {
                        uploadData.errors.push(response.Data);
                    }
                });
            }, function (evt) {
                $timeout(function () {
                    uploadData.progress = parseInt(100.0 * evt.loaded / evt.total);
                });
            });
        }

        function cancelItem(index) {
            var uploadItem = $scope.uploads[index];
            var sourceId = uploadItem.sourceId;
            if (!uploadItem.isBusy) {
                if (!sourceId) return;
                resourceService.cancel({ sourceId: sourceId });
                return;
            }
            uploadItem.upload.abort();
        }

        $scope.showhelp = false;
        $scope.fileHandler = fileTypes[0];
        $scope.fileTypes = fileTypes;
        $scope.uploads = [];
        $scope.sourceIds = [];
        $scope.scrollOptions = {
            autoHideScrollbar: true,
            updateOnContentResize: true,
            autoDraggerLength: true
        };

        $scope.toggleHelp = function() {
            $scope.showhelp = !$scope.showhelp;
        };

        $scope.showSubmit = function () {
            if (!$scope.uploads.length) return false;
            for (var index in $scope.uploads) {
                var upload = $scope.uploads[index];
                if ((upload.isBusy) || (upload.hasErrors)) return false;
            }
            return true;
        };

        $scope.panelClass = function(uploadItem) {
            if (uploadItem.isBusy) return 'ui-state-highlight';
            if (uploadItem.hasErrors) return 'ui-state-error';
            return 'ui-state-success';
        };

        $scope.progressType = function(uploadItem) {
            if (uploadItem.isBusy) return 'warning';
            if (uploadItem.hasErrors) return 'danger';
            return 'success';
        };

        $scope.showError = function (uploadItem) {
            return !uploadItem.isBusy && uploadItem.hasErrors;
        };

        $scope.cancelItem = function (index) {
            cancelItem(index);
            $scope.uploads.splice(index, 1);
        };

        $scope.cancelAll = function() {
            var index = 0;
            var sourceId = $scope.uploads[index].sourceId;
            var recurse = function() {
                index++;
                if (index === $scope.uploads.length) {
                    $window.location.href = urls.inputHistory;
                    return;
                }
                sourceId = $scope.uploads[index].sourceId;
                resourceService.cancel({ sourceId: sourceId }, recurse);
            };
            resourceService.cancel({ sourceId: sourceId }, recurse);
        };
        
        //this must change to first show the dialog then when all of the promises have been resolved 
        //to show the close and give a message
        $scope.submit = function () {
                $modal.open({
                    templateUrl: 'ModalResult.html',
                    controller: modalResult,
                    backdrop: 'static',
                    resolve: {
                        files : function() {
                            return $scope.uploads;
                        }
                    }
                });
        };
        
        $scope.onFileSelect = function ($files) {
            for (var i = 0; i < $files.length; i++) {
                var file = $files[i];
                if (((file.type !== 'application/vnd.ms-excel.sheet.macroEnabled.12') &&
                        (file.type !== 'application/vnd.ms-excel') &&
                        (file.type !== 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) ||
                    (file.size > (5 * 1048576))) continue;
                var uploadData = {
                    name: file.name,
                    progress: -1,
                    isBusy: true,
                    hasErrors: false,
                    sourceId: null,
                    upload: {},
                    errors: []
                };
                $scope.uploads.push(uploadData);
                uploadFile(uploadData, file);
            }
        };
    }

    angular
        .module('Upload', ['ngResource', 'CustomScroller', 'angularFileUpload', 'ui.bootstrap', 'ngSanitize', 'ngAnimate'])
        .controller('Controller', controller)
        .factory('ResourceService', ['$resource', 'Urls', function($resource, urls) {
            return $resource(urls.extract, {}, {
                'extractData': {
                    url: urls.extract,
                    params: { sourceId: '@sourceId' },
                    method: 'POST'
                },
                'cancel': {
                    url: urls.cancel,
                    params: { sourceId: '@sourceId' },
                    method: 'POST'
                }
            });
        }]);
}));

