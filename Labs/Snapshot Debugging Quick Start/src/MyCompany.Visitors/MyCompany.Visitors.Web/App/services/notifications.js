define(['services/logger', 'services/navigation'], function (logger, navigation) {
    var notificationHub = $.connection.visitorsNotificationHub,
    notificationOptions = {
        "debug": false,
        "positionClass": "toast-top-full-width",
        "fadeIn": 300,
        "fadeOut": 1000,
        "timeOut": 0,
        "extendedTimeOut": 1000
    },
    subscriptions = {};

    function subscribe(event, source, callback) {
        if (!subscriptions[event])
            subscriptions[event] = {};

        subscriptions[event][source] = callback;
    };

    function executeCallbacks(event) {
        if (!subscriptions[event])
            return;

        var args = Array.prototype.slice.apply(arguments).slice(1, arguments.length);
        //var args = arguments.slice(1, arguments.length);

        for (var source in subscriptions[event]) {
            
            var func = subscriptions[event][source];
            if(func)
                func.apply(null, args);
        }
    }

    function notifyVisitArrived(visit) {
        toastr.options = notificationOptions;
        var url = '#/visits?visitId=' + visit.VisitId;
        toastr.options.onclick = function (e) {
            navigation.navigateTo(url);
            toastr.clear($toast);
            return true;
        };

        var message = visit.Visitor.FirstName + ' ' + visit.Visitor.LastName + ' has arrived.';
        var $toast = toastr["info"](message);

        executeCallbacks('visitArrived', visit);
    };

    function notifyVisitorPicturesChanged(visitorPictures) {
        executeCallbacks('visitorPicturesChanged', visitorPictures);
    };

    function startConnection() {
        if (notificationHub) {
            notificationHub.client.notifyVisitArrived = notifyVisitArrived;
            notificationHub.client.notifyVisitorPicturesChanged = notifyVisitorPicturesChanged;

            $.connection.hub.start().done(function (myHubConnection) {
                if ($.connection.hub.state == 1) {
                    logger.log("connected");
                }
            }).fail(function (err) {
                logger.log(err);
            });
        } else {
            logger.log('notification hub unavailable');
        }
    }

    var notificationService = {
        startConnection: startConnection,
        subscribe: subscribe
    };

    return notificationService;
});