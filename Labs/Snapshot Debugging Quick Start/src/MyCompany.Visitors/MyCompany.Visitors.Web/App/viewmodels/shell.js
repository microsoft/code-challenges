define(['durandal/system', 'services/logger', 'durandal/plugins/router', 'config', 'services/context', 'services/navigation', 'viewmodels/base'],
    function(system, logger, router, config, context, navigation, base) {
        var shell = {
            activate: activate,
            router: router,
            isBusy: base.isBusy,
            home: home
        };
        return shell;

        function activate() {
            logger.log('MyCompany Loaded', null, system.getModuleId(shell));

            router.map(config.routes);
            return router.activate(config.startModule);
        }

        function home() {
            handleNoAuthLink("", true);
        }

        function handleNoAuthLink(route, executeIfNoAuth) {
            var noauthIndex = window.location.href.indexOf("noauth");
            var url;
            if (noauthIndex != -1) {
                if (window.location.pathname)
                    url = window.location.protocol + "//" + window.location.host + window.location.pathname;
                else
                    url = window.location.protocol + "//" + window.location.host + "/noauth/";

                if (executeIfNoAuth) {
                    window.location.href = url;
                }
            }
            else {
                if (window.location.pathname)
                    url = window.location.protocol + "//" + window.location.host + window.location.pathname + route;
                else
                    url = window.location.protocol + "//" + window.location.host + "/" + route;

                window.location.href = url;
            }
        };
    }
);