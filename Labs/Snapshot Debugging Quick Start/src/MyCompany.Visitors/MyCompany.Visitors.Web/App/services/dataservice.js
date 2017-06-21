define(['services/logger', 'durandal/system', 'services/model'], function (logger, system, model) {
    var apiBaseUrl = 'api/';

    var getLoggedEmployeeInfo = function (pictureType) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                cache: false,
                url: apiBaseUrl + 'Employees/current/' + pictureType
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                var employee = new model.Employee(data);
                log('Retrieved current user from remote data source', data);
                deferred.resolve(employee);
            }
        }).promise();
    };

    var getEmployee = function (employeeId, pictureType) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                url: apiBaseUrl + 'employees/' + employeeId + '/' + pictureType
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                var employee = new model.Employee(data);
                deferred.resolve(employee);

                log('Retrieved employee', data);
            }
        }).promise();
    };

    var getVisitors = function (parameters, cache) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                url: apiBaseUrl + 'visitors?' +
                'filter=' + parameters.filter +
                '&pictureType=' + parameters.pictureType +
                '&pageSize=' + parameters.pageSize +
                '&pageCount=' + parameters.pageCount,
                cache: cache ? cache : true
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                var visitors = [];
                var length = data.length;
                for (var i = 0; i < length; i++) {
                    var visitor = new model.Visitor(data[i]);
                    visitors.push(visitor);
                }

                deferred.resolve(visitors);
                log('Retrieved visitors from remote data source', visitors);
            }
        }).promise();
    };

    var getVisitorsCount = function (filter) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                url: apiBaseUrl + 'visitors/count?' +
                'filter=' + filter
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                deferred.resolve(data);
                log('Retrieved visitors count from remote data source', data);
            }
        }).promise();
    };

    var deleteVisitor = function (visitorId) {
        return system.defer(function (deferred) {
            var options = {
                type: 'DELETE',
                url: apiBaseUrl + 'visitors/' + visitorId
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded() {
                deferred.resolve();
                log('Deleted visitor with id ' + visitorId, null);
            }
        }).promise();
    };

    var addVisitor = function (visitor, file) {
        return system.defer(function (deferred) {
            var options = {
                type: 'POST',
                data: ko.toJSON(visitor),
                url: apiBaseUrl + 'visitors'
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(id) {
                var v = visitor();
                v.visitorId(id);
                log('Added visitor', v);

                if (file && file.selection) {
                    uploadPicture(file, id)
                        .then(function () {
                            deferred.resolve(v);
                        }).fail(function () {
                            deferred.resolve();
                        });
                } else {
                    deferred.resolve(v);
                }
            }
        }).promise();
    };

    var updateVisitor = function (visitor, file) {
        return system.defer(function (deferred) {
            delete visitor().visits;

            var options = {
                type: 'PUT',
                data: JSON.stringify(ko.mapping.toJS(visitor)),
                url: apiBaseUrl + 'visitors'
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded() {
                log('Updated visitor with id ' + visitor().visitorId(), visitor);

                if (file && file.selection) {
                    uploadPicture(file, visitor().visitorId())
                        .then(function () {
                            deferred.resolve(visitor);
                        }).fail(function () {
                            deferred.resolve();
                        });
                } else {
                    deferred.resolve(visitor);
                }
            }
        }).promise();
    };

    var getVisitorPicture = function (visitorId, pictureType) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                url: apiBaseUrl + 'VisitorPictures/' + visitorId + '/' + pictureType
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                if (data) {
                    deferred.resolve('data:image/jpeg;base64,' + data);
                } else {
                    deferred.resolve();
                }
                log('Retrieved visitor picture', data);
            }
        }).promise();
    };

    var getVisits = function (parameters, cache) {
        return system.defer(function (deferred) {
            var localeDate = moment().subtract('hours', 1).toDate();
            var date = new Date(localeDate.getUTCFullYear(), localeDate.getUTCMonth(), localeDate.getUTCDate(), localeDate.getUTCHours(), localeDate.getUTCMinutes(), localeDate.getUTCSeconds());

            var options = {
                type: 'GET',
                url: apiBaseUrl + 'visits/user?' +
                'filter=' + parameters.filter +
                '&pictureType=' + parameters.pictureType +
                '&pageSize=' + parameters.pageSize +
                '&pageCount=' + parameters.pageCount +
                '&dateFilter=' + date.toJSON(),
                cache: cache ? cache : true
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                var visits = [];
                var length = data.length;
                for (var i = 0; i < length; i++) {
                    var visit = new model.Visit(data[i], true);
                    visits.push(visit);
                }

                deferred.resolve(visits);
                log('Retrieved visits from remote data source', visits);
            }
        }).promise();
    };

    var getVisit = function (visitId, pictureType) {
        return system.defer(function (deferred) {
            var options = {
                type: 'GET',
                url: apiBaseUrl + 'visits/' + visitId + '/' + pictureType
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                var visit = new model.Visit(data, true);

                deferred.resolve(visit);
                log('Retrieved visit from remote data source', visit);
            }
        }).promise();
    };

    var getVisitsCount = function (filter) {
        return system.defer(function (deferred) {
            var localeDate = moment().subtract('hours', 1).toDate();
            var date = new Date(localeDate.getUTCFullYear(), localeDate.getUTCMonth(), localeDate.getUTCDate(), localeDate.getUTCHours(), localeDate.getUTCMinutes(), localeDate.getUTCSeconds());

            var options = {
                type: 'GET',
                url: apiBaseUrl + 'visits/user/count?' +
                'filter=' + filter +
                '&dateFilter=' + date.toJSON()
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(data) {
                deferred.resolve(data);
                log('Retrieved visitors count from remote data source', data);
            }
        }).promise();
    };

    var deleteVisit = function (visitId) {
        return system.defer(function (deferred) {
            var options = {
                type: 'DELETE',
                url: apiBaseUrl + 'visits/' + visitId
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded() {
                deferred.resolve();
                log('Deleted visit with id ' + visitId, null);
            }
        }).promise();
    };

    var addVisit = function (visit) {
        return system.defer(function (deferred) {
            visit().employee = null;
            visit().visitor = null;

            var options = {
                type: 'POST',
                data: ko.toJSON(visit),
                url: apiBaseUrl + 'visits'
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded(id) {
                var v = visit();
                v.visitId(id);
                log('Added visit', v);
                deferred.resolve(v);
            }
        }).promise();
    };

    var updateVisit = function (visit) {
        return system.defer(function (deferred) {
            var v = ko.mapping.toJS(visit);
            v.employee = null;
            v.visitor = null;

            var options = {
                type: 'PUT',
                data: JSON.stringify(v),
                url: apiBaseUrl + 'visits'
            };
            $.ajax(options)
                .then(succeeded)
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                });

            function succeeded() {
                log('Updated visit with id ' + visit().visitId(), visit);
                deferred.resolve(visit);
            }
        }).promise();
    };

    //var getVisitorCRMDetails = function (visitorId) {
    //    return system.defer(function (deferred) {
    //        var options = {
    //            type: 'GET',
    //            url: apiBaseUrl + 'crm/' + visitorId
    //        };
    //        $.ajax(options)
    //            .then(succeeded)
    //            .fail(function (jqXHR, textStatus) {
    //                failed(jqXHR, textStatus);
    //                deferred.reject();
    //            });

    //        function succeeded(data) {
    //            deferred.resolve(data);
    //            log('Retrieved CRM data', data);
    //        }
    //    }).promise();
    //};

    var dataservice = {
        getLoggedEmployeeInfo: getLoggedEmployeeInfo,
        getEmployee: getEmployee,
        getVisitors: getVisitors,
        getVisitorsCount: getVisitorsCount,
        deleteVisitor: deleteVisitor,
        addVisitor: addVisitor,
        updateVisitor: updateVisitor,
        getVisitorPicture: getVisitorPicture,
        getVisits: getVisits,
        getVisitsCount: getVisitsCount,
        deleteVisit: deleteVisit,
        addVisit: addVisit,
        updateVisit: updateVisit,
        getVisit: getVisit,
        //getVisitorCRMDetails: getVisitorCRMDetails
    };

    return dataservice;

    //#region Internal methods

    function log(msg, data) {
        logger.log(msg, data, system.getModuleId(dataservice));
    }

    function failed(jqXHR, textStatus) {
        var msg = 'Error getting data. ' + textStatus;
        logger.log(msg, jqXHR, system.getModuleId(dataservice));
    }

    function uploadPicture(file, visitorId) {
        return system.defer(function (deferred) {
            var data = new FormData();
            data.append('image-file', file.data);
            if (visitorId)
                data.append('visitorId', visitorId);
            if (file.selection)
                data.append('image-crop', JSON.stringify(file.selection));

            var options = {
                type: 'POST',
                data: data,
                processData: false,
                contentType: false,
                url: apiBaseUrl + 'visitorpictures/pictures'
            };
            $.ajax(options)
                .then(function () {
                    deferred.resolve();
                    log('Upload picture');
                })
                .fail(function (jqXHR, textStatus) {
                    failed(jqXHR, textStatus);
                    deferred.reject();
                }
                );
        }).promise();
    }

    //#endregion
});