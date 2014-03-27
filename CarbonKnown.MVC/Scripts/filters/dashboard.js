;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('DashboardFilters', [])
        .filter('ckCurrency', function() {
            return function(input, symbol, seperator, decimal) {
                if (!symbol) symbol = 'R';
                if (!seperator) seperator = ' ';
                var places = parseInt(decimal);
                if (!places) {
                    places = 2;
                }
                var value = parseFloat(input);
                if (!value) return input;
                value = value.toFixed(places);
                var parts = value.toString().split(".");
                parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, seperator);
                parts[0] = symbol + ' ' + parts[0];
                return parts.join(".");
            };
        })
        .filter('ckPercent', function() {
            return function(input, decimal) {
                var value = parseFloat(input);
                if (!value) return input;
                var seperator = ' ';
                var places = parseInt(decimal);
                if (!places) {
                    places = 2;
                }
                value = value.toFixed(places);
                var symbol = '';
                if (value > 0) {
                    symbol = '+';
                }
                var parts = value.toString().split(".");
                parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, seperator);
                return symbol + parts.join(".") + ' %';
            };
        });
}));
