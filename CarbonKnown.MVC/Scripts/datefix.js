;
(function(readyfunction) {
    readyfunction(window, document);
}(function(window, document, undefined) {
    'use strict';
    Date.prototype.toJSON = function() {

        function f(n) {
            // Format integers to have at least two digits.
            return n < 10 ? '0' + n : n;
        }

        return this.getFullYear() + '-' +
            f(this.getMonth() + 1) + '-' +
            f(this.getDate()) + 'T00:00:00.00Z';
    };
}));