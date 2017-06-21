define(['services/dataservice', 'services/enums', 'viewmodels/base', 'config', 'services/communicator', 'services/notifications'], function (dataservice, enums, base, config, communicator, notifications) {
    var visitEntity = ko.observable(),
        employeePicture = ko.observable();

    var vm = {
        activate: activate,
        visitEntity: visitEntity,
        employeePicture: employeePicture,
        openLync: openLync,
        close: close
    };

    return vm;

    function activate(routeData) {
        if (routeData && routeData.visit) {
            routeData.visit.visitor().bigPicture = ko.observable(config.defaultBigPicture);
            employeePicture(config.defaultBigPicture);
            visitEntity(routeData.visit);

            //base.showLoading();
            //dataservice.getVisitorCRMDetails(routeData.visit.visitor().visitorId()).then(function (data) {
            //    if (data) {
            //        visitEntity().visitor().crmAccountManager(data.CRMAccountManager);
            //        visitEntity().visitor().crmLeads(data.CRMLeads);
            //    }
            //    base.hideLoading();
            //});

            base.showLoading();
            dataservice.getVisitorPicture(routeData.visit.visitor().visitorId(), enums.pictureType.big).then(function (data) {
                if (data) {
                    visitEntity().visitor().bigPicture(data);
                }
                base.hideLoading();
            });
            base.showLoading();
            dataservice.getEmployee(routeData.visit.employee.employeeId(), enums.pictureType.big).then(function (data) {
                if (data) {
                    employeePicture(data.picture());
                }
                base.hideLoading();
            });
            subscribeToNotifications();
        } else {
            // show error message
        }
    };

    function subscribeToNotifications() {
        notifications.subscribe('visitorPicturesChanged', 'visitDetail', function (visitorPictures) {
            if (!visitorPictures
                || !visitorPictures[0]
                || visitorPictures[0].VisitorId != visitEntity().visitor().visitorId())
                return;

            var picture = 'data:image/jpeg;base64,' + visitorPictures[0].Content;
            visitEntity().visitor().bigPicture(picture);
        });
    }
    function close() {
        vm.modal.close();
    }

    function openLync(employee) {
        communicator.openLync(employee.email());
    }

});