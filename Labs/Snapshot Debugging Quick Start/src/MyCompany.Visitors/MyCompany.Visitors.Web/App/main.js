require.config({
    paths: { "text": "durandal/amd/text" }
});

define(function (require) {
    var system = require('durandal/system'),
        app = require('durandal/app'),
        router = require('durandal/plugins/router'),
        viewLocator = require('durandal/viewLocator'),
        logger = require('services/logger'),
        dataservice = require('services/dataservice'),
        context = require('services/context'),
        base = require('viewmodels/base'),
        enums = require('services/enums'),
        notifications = require('services/notifications');

    system.debug(true);

    $.ajaxSetup({
        dataType: 'json',
        contentType: 'application/json',
        error: function (xmlHttpRequest, type, errorThrown) {
            if (xmlHttpRequest.status == 0) return;
            base.hideLoading(true);
            app.showMessage('There was an error, try again later', 'Error', [enums.options.ok]);
            logger.log(xmlHttpRequest.responseText, type, xmlHttpRequest);
        },
    });

    dataservice.getLoggedEmployeeInfo(enums.pictureType.small).then(function (employee) {
        context.currentUser = employee;
        
        app.start().then(function () {
            // route will use conventions for modules
            // assuming viewmodels/views folder structure
            router.useConvention();
            viewLocator.useConvention();

            notifications.startConnection();

            app.title = 'MyCompany';
            app.setRoot('viewmodels/shell');

            router.handleInvalidRoute = function (route, params) {
                logger.log('No route found', route, 'main');
            };

            // handle escape key for modal dialogs
            $(document).keyup(function (e) {
                // esc
                if (e.keyCode == 27) {
                    $(".button-close").last().click();
                }
            });
        });
    });
});