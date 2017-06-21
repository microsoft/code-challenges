define(function () {
    var routes = [{
        url: 'visitors',
        moduleId: 'viewmodels/visitors',
        name: 'Visitors',
        visible: true
    },
    {
        url: 'visits',
        moduleId: 'viewmodels/visits',
        name: 'My Visits',
        visible: true
    }];

    var isNoAuth = location.href.toLowerCase().indexOf('noauth') != -1;

    if (isNoAuth) {
        var hasHash = location.href.toLowerCase().indexOf('noauth#/') != -1;
        if (!hasHash) {
            location.href = location.href.toLowerCase().replace('noauth', 'noauth#/');
        }

        for (var i = 0; i < routes.length; i++) {
            routes[i].hash = 'noauth#/' + routes[i].url;
        }
    }

    var startModule = 'visits',
        pageSize = 10,
        dateFormat = 'MM/DD/YYYY',
        timeFormat = 'hh:mm A',
        defaultPicture = 'Content/Images/no-photo.jpg',
        defaultBigPicture = 'Content/Images/no-photo-big.png';

    return {
        routes: routes,
        startModule: startModule,
        pageSize: pageSize,
        dateFormat: dateFormat,
        timeFormat: timeFormat,
        defaultPicture: defaultPicture,
        defaultBigPicture: defaultBigPicture,
        isNoAuth: isNoAuth
    };
});


if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    };
}