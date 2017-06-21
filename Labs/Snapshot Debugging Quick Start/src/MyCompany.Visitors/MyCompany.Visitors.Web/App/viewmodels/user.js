define(['services/dataservice', 'services/context'], function (dataservice, context) {
    var initialized = false;
    
    var vm = {
        activate: activate,
        name: context.currentUser.fullName,
        picture: context.currentUser.picture,
        openFAQ: openFAQ,
        signout: signout
    };

    return vm;

    function activate() {
        if (initialized) return;

        initialized = true;
    }

    function openFAQ() {
        handleNoAuthLink("questions/index", true);
    }

    function signout() {
        handleNoAuthLink("account/signout", false);
    }

    function handleNoAuthLink(route, executeIfNoAuth) {
        var noauthIndex = window.location.href.indexOf("noauth");
        var url;
        if (noauthIndex != -1) {
            if ( window.location.pathname)
                url = window.location.protocol + "//" + window.location.host + window.location.pathname + "/" + route;
            else
                url = window.location.protocol + "//" + window.location.host + "/noauth/" + route;

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
});