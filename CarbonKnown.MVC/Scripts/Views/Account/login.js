(function (readyfunction) {
    readyfunction(window.jQuery, window, document, undefined);
}(function ($, window, document) {
    $(function () {
        $('#btnLogin').click(function () {
            $('form').submit();
        });
    });
}));
