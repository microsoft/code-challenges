define(function () {
    function openLync(email) {
        // open the sip link in a new window instead of put an href
        // to avoid IE11 open a blank page
        var newWindow = window.open('sip:' + email);
        if (newWindow)
            newWindow.close();
    }

    var communicatorService = {
        openLync: openLync
    };

    return communicatorService;
});