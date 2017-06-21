(function () {
    function handleNoAuthLink(route, executeIfNoAuth) {
        var noauthIndex = window.location.href.indexOf("noauth");
        var url;
        if (noauthIndex != -1) {
            if (window.location.pathname)
                url = window.location.protocol + "//" + window.location.host + window.location.pathname.toLowerCase().replace("/questions/index", "") + route;
            else
                url = window.location.protocol + "//" + window.location.host + "/noauth/" + route;

            if (executeIfNoAuth) {
                window.location.href = url;
            }
        }
        else {
            if (window.location.pathname)
                url = window.location.protocol + "//" + window.location.host + window.location.pathname.toLowerCase().replace("questions/index", "") + route;
            else
                url = window.location.protocol + "//" + window.location.host + "/" + route;

            window.location.href = url;
        }
    };

    $("#signout").click(function () {
        handleNoAuthLink("Account/Signout", false);
    });

    $("#homeLogo").click(function () {
        handleNoAuthLink("", true);
    });

    $(".answer").click(function () {
        $('#myModal').modal('show');
    });

    $("#closeModal").click(function () {
        $('#myModal').modal('hide');
    });
}());