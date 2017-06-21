define(['services/dataservice', 'services/enums', 'config', 'viewmodels/base', 'services/communicator', 'services/notifications'], function (dataservice, enums, config, base, communicator, notifications) {
    var visitorEntity = ko.observable(),
        visits = ko.observable(),
        visitor;

    var vm = {
        activate: activate,
        visitorEntity: visitorEntity,
        close: close,
        visits: visits,
        chooseVisitTemplate: chooseVisitTemplate,
        openLync: openLync,
        viewAttached: viewAttached
    };

    return vm;

    function activate(routeData) {
        if (routeData && routeData.visitor) {
            base.showLoading();
            visits(null);
            visitor = routeData.visitor;
            visitor.bigPicture = ko.observable(config.defaultBigPicture);
            visitorEntity(visitor);

            dataservice.getVisitorPicture(routeData.visitor.visitorId(), enums.pictureType.big).then(function (data) {
                if (data) {
                    visitor.bigPicture(data);
                }

                base.hideLoading();
            });

            subscribeToNotifications();
        } else {
            // show error message
        }
    };

    function close() {
        vm.modal.close();
    }

    function openLync(employee) {
        communicator.openLync(employee.email());
    }

    function viewAttached() {
        // to avoid parse foreach visits before durandal compose the view
        // and have the templates are loaded
        visits(visitor.visits());
    }

    function chooseVisitTemplate(visit) {
        return visit ? 'visit-template' : 'no-visit-template';
    }

    function subscribeToNotifications() {
        notifications.subscribe('visitorPicturesChanged', 'visitorDetail', function (visitorPictures) {
            if (!visitorPictures
                || !visitorPictures[0]
                || visitorPictures[0].VisitorId != visitorEntity().visitorId())
                return;

            var picture = 'data:image/jpeg;base64,' + visitorPictures[0].Content;
            visitorEntity().bigPicture(picture);
        });
    }
});