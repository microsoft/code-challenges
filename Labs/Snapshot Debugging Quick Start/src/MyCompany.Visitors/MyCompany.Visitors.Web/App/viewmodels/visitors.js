define(['services/dataservice', 'services/enums', 'durandal/app', 'services/logger', 'viewmodels/viewModels', 'viewmodels/base', 'services/notifications'],
    function (dataservice, enums, app, logger, viewModels, base, notifications) {
        var initialized = false,
            vmList = new viewModels.ListViewModel();
        
        var vm = {
            activate: activate,
            title: 'Visitors view',
            visitors: vmList.source,
            visitorsCount: vmList.sourceItemsCount,
            refresh: vmList.refresh,
            filter: vmList.filter,
            pages: vmList.pages,
            paginate: vmList.paginate,
            nextPage: vmList.nextPage,
            previousPage: vmList.previousPage,
            anyRecord: vmList.anyRecord,
            add: add,
            edit: edit,
            deleteVisitor: deleteVisitor,
            openDetail: openDetail
        };

        return vm;

        function activate() {
            if (!initialized) {
                vmList.initialize(dataservice.getVisitors, dataservice.getVisitorsCount);
                initialized = true;
            }
            
            subscribeToNotifications();
            return vmList.refresh();
        }

        function add() {
            app.showModal('viewmodels/visitorForm').then(function (visitor) {
                if (visitor) {
                    vmList.refresh();
                }
            });
        }

        function edit(entity) {
            app.showModal('viewmodels/visitorForm', { visitor: entity }).then(function (visitor) {
                if (visitor) {
                    vmList.refresh();
                }
            });
        }

        function deleteVisitor(entity) {
            var message = 'Are you sure you want to delete ' + entity.fullName() + '?';
            app.showMessage(message, 'confirmation', [enums.options.yes, enums.options.no])
                .then(function (dialogResult) {
                    if (dialogResult == enums.options.yes) {
                        base.showLoading();
                        dataservice.deleteVisitor(entity.visitorId()).then(function () {
                            vmList.removeSourceItem(entity);
                            base.hideLoading();
                        });
                    }
                });
        }

        function openDetail(entity) {
            app.showModal('viewmodels/visitorDetail', { visitor: entity });
        }

        function subscribeToNotifications() {
            notifications.subscribe('visitorPicturesChanged', 'visitors', function (visitorPictures) {
                if (!visitorPictures || !visitorPictures[1])
                    return;

                var smallPicture = visitorPictures[1].Content;
                var visitorId = visitorPictures[1].VisitorId;
                var visitors = vmList.source();
                visitors.forEach(function (visitor) {
                    if (visitor.visitorId() == visitorId)
                        visitor.pictureContent(smallPicture);
                });
            });
        }
    });