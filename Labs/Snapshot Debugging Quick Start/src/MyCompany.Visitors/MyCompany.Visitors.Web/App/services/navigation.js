define(['services/logger', 'config', 'durandal/plugins/router'], function (logger, config, router) {
    function navigateTo(url) {
        if (!url) return;

        url = config.isNoAuth ? 'noauth' + url : url;
        router.navigateTo(url);
    }

    function back() {
        router.navigateBack();
    }

    var navigationService = {
        navigateTo: navigateTo,
        back: back
    };

    return navigationService;
});