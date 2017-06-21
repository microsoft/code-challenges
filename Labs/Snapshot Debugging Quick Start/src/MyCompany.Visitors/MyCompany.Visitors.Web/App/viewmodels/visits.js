define(['services/dataservice', 'services/enums', 'durandal/app', 'viewmodels/viewModels', 'viewmodels/base', 'services/notifications'],
    function (dataservice, enums, app, viewModels, base, notifications) {
        var initialized = false,
            vmList = new viewModels.ListViewModel();

        var vm = {
            activate: activate,
            viewAttached: viewAttached,
            title: 'Visits view',
            visits: vmList.source,
            visitsCount: vmList.sourceItemsCount,
            refresh: vmList.refresh,
            filter: vmList.filter,
            pages: vmList.pages,
            paginate: vmList.paginate,
            nextPage: vmList.nextPage,
            previousPage: vmList.previousPage,
            anyRecord: vmList.anyRecord,
            add: add,
            edit: edit,
            deleteVisit: deleteVisit,
            openDetail: openDetail
        };

        return vm;

        function activate(routeData) {
            base.showLoading();

            if (!initialized || routeData && routeData.visitId) {
                vmList.initialize(dataservice.getVisits, dataservice.getVisitsCount);
                initialized = true;

                if (routeData && routeData.visitId) {
                    base.showLoading();
                    dataservice.getVisit(routeData.visitId, enums.pictureType.small).then(function (entity) {
                        openDetail(entity);
                        base.hideLoading();
                    });
                }
            }
            subscribeToNotifications();
            return vmList.refresh();
        }


        function viewAttached() {
            $('.multi-ellipsis').dotdotdot();
            base.hideLoading();
        }

        function add() {
            app.showModal('viewmodels/visitForm').then(function (visit) {
                if (visit) {
                    vmList.refresh();
                }
            });
        }

        function edit(entity) {
            app.showModal('viewmodels/visitForm', { visit: entity });
        }

        function deleteVisit(entity) {
            var message = 'Are you sure you want to delete the visit of ' + entity.visitor().fullName() + '?';
            app.showMessage(message, 'confirmation', [enums.options.yes, enums.options.no])
                .then(function (dialogResult) {
                    if (dialogResult == enums.options.yes) {
                        base.showLoading();
                        dataservice.deleteVisit(entity.visitId()).then(function () {
                            vmList.removeSourceItem(entity);
                            base.hideLoading();
                        });
                    }
                });
        }

        function openDetail(entity) {
            app.showModal('viewmodels/visitDetail', { visit: entity });
        }

        function subscribeToNotifications() {
            notifications.subscribe('visitorPicturesChanged','visits', function (visitorPictures) {
                if (!visitorPictures || !visitorPictures[1])
                    return;

                var smallPicture = visitorPictures[1].Content;
                var visitorId = visitorPictures[1].VisitorId;
                var visits = vmList.source();
                visits.forEach(function (visit) {
                    if (visit.visitor().visitorId() == visitorId)
                        visit.visitor().pictureContent(smallPicture);
                });
            });
        }
    });