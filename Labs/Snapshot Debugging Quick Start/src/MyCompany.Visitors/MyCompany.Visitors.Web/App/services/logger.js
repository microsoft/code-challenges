define(['durandal/system'],
    function (system) {
        var logger = {
            log: log
        };

        return logger;

        function log(message, data, source) {
            source = source ? '[' + source + '] ' : '';
            if (data) {
                system.log(source, message, data);
            } else {
                system.log(source, message);
            }
        }
    });