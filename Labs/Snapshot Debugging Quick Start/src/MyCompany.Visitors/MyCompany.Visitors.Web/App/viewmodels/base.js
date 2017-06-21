define(function () {
    var isBusy = ko.observable(false).extend({ throttle: 300 }),
        pendingActions = 0;

    var vm = {
        showLoading: showLoading,
        hideLoading: hideLoading,
        isBusy: isBusy
    };

    return vm;

    function showLoading() {
        pendingActions++;
        isBusy(true);
    }

    function hideLoading(forceIgnoringPendingActions) {
        setTimeout(function () {
            if (forceIgnoringPendingActions) {
                pendingActions = 0;
            } else {
                if (pendingActions > 0)
                    pendingActions--;
            }
            
            if (pendingActions == 0) {
                isBusy(false);
            }
        }, 0);
    }
});