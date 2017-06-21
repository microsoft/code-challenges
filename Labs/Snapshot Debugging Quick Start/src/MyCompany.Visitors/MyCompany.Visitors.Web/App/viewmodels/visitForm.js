define(['services/model', 'services/dataservice', 'services/enums', 'services/context', 'viewmodels/base', 'config'],
    function (model, dataservice, enums, context, base, config) {
        var modes = {
            add: 1,
            update: 2
        };

        var visitEntity = ko.observable(),
            title = ko.observable(),
            actionText = ko.observable(),
            selectedVisitor,
            mode;

        var vm = {
            title: title,
            actionText: actionText,
            activate: activate,
            viewAttached: viewAttached,
            visitEntity: visitEntity,
            close: close,
            accept: accept
        };

        return vm;

        function activate(routeData) {
            if (routeData && routeData.visit) {
                mode = modes.update;
                title('Edit visit');
                actionText('Save changes');
                visitEntity(routeData.visit);
            } else {
                mode = modes.add;
                title('New visit');
                actionText('Save changes');
                visitEntity(new model.Visit());
            }

            visitEntity().plate.subscribe(function (data) {
                if (data) {
                    visitEntity().hasCar(true);
                } else {
                    visitEntity().hasCar(false);
                }
            });
        };

        function close() {
            vm.modal.close();
        }

        function accept() {
            if (!visitEntity().isValid()) {
                visitEntity().errors.showAllMessages(true);
                return;
            }

            if (mode == modes.add) {
                visitEntity().employeeId(context.currentUser.employeeId());

                base.showLoading();
                dataservice.addVisit(visitEntity).then(function (data) {
                    base.hideLoading();
                    vm.modal.close(data);
                });
            }
            else if (mode == modes.update) {
                base.showLoading();
                dataservice.updateVisit(visitEntity).then(function (data) {
                    if (selectedVisitor) {
                        // manage entity data in memory to avoid calling the server for refresh the list
                        visitEntity().visitor(new model.Visitor(selectedVisitor));
                    }
                    base.hideLoading();
                    vm.modal.close(visitEntity);
                });
            }
        }

        function viewAttached() {
            $('#visitor-id-input').select2({
                placeholder: 'Search for a visitor',
                ajax: {
                    url: 'api/visitors',
                    dataType: 'json',
                    data: function (term) {
                        return {
                            filter: term,
                            pageSize: config.pageSize,
                            pictureType: enums.pictureType.small,
                            pageCount: 0
                        };
                    },
                    results: function (data) {
                        var visitors = [];
                        for (var i = 0; i < data.length; i++) {
                            var visitor = new model.Visitor(data[i]);
                            visitors.push({
                                id: visitor.visitorId(),
                                text: visitor.fullName(),
                                picture: visitor.picture(),
                                firstName: visitor.firstName(),
                                lastName: visitor.lastName(),
                                pictureContent: visitor.pictureContent(),
                                company: visitor.company(),
                            });
                        }
                        return { results: visitors };
                    }
                },
                formatResult: function (visitor) {
                    return "<img class='picture' src='" + visitor.picture + "' />" + visitor.text;
                },
                initSelection: function (element, callback) {
                    var visitor = visitEntity().visitor();
                    callback({
                        id: visitor.visitorId(),
                        text: visitor.fullName(),
                        picture: visitor.picture()
                    });
                }
            });

            $('#visitor-id-input').on('change', function (e) {
                selectedVisitor = {
                    VisitorId: e.added.id,
                    FirstName: e.added.firstName,
                    LastName: e.added.lastName,
                    VisitorPictures: [{ Content: e.added.pictureContent }],
                    Company: e.added.company
                };
            });
        }
    });