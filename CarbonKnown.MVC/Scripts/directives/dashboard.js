;
(function(readyfunction) {
    readyfunction(window, document, window.angular, window.jQuery);
}(function(window, document, angular, $, undefined) {
    'use strict';

    angular
        .module('DashboardDirectives', [])
        .directive('ckOverflow', function() {
            return {
                restrict: 'A',
                scope: {
                    easing: '='
                },
                link: function(scope, element, attrs) {
                    var width = attrs['ckOverflow'];
                    scope.$watch(function() { return element.val() || element.text(); }, function() {
                        var originalCss = element.data('originalstyle');
                        if (!originalCss) {
                            originalCss = element.attr('style');
                            element.data('originalstyle', originalCss);
                        } else {
                            element.attr('style', originalCss);
                        }
                        var wrapper = element.find('.ckOverflowWrapper');
                        if (wrapper.length) {
                            wrapper.contents().unwrap();
                            element.off('mouseenter');
                        }
                        var clone = element.clone();
                        clone.hide();
                        clone.appendTo('body');
                        var currentWidth = clone.outerWidth(true);
                        var newHeight = clone.outerHeight(true);
                        clone.detach();
                        if (currentWidth > width) {
                            element.css({
                                width: width,
                                height: newHeight,
                                overflow: 'hidden',
                                position: 'relative',
                                display: 'inline-block'
                            });
                            var wrap = element.wrapInner('<div class="ckOverflowWrapper" style="position:absolute;left:0px"></div>');
                            var inner = wrap.find('div');
                            var easing = scope.easing;
                            var animate = easing ? JSON.parse(easing) : { easing: 'linear', duration: 3000 };
                            var offset = currentWidth - width;
                            element.on('mouseenter', function() {
                                animate.complete = function() {
                                    inner.animate({ left: 0 });
                                };
                                inner.animate({ left: '-' + offset }, animate);
                            });
                        }
                    });
                }
            };
        })
        .directive('ckResize', function() {
            return {
                restrict: 'A',
                scope: {
                    ckResize: '='
                },
                link: function(scope, element) {
                    var padding = scope.ckResize ? scope.ckResize : 0;
                    scope.$watch(function() { return element.val() || element.text(); }, function() {
                        var text = element.val();
                        if (!text) text = element.text();

                        var elementFontSize = element.data('fontSize');
                        if (!elementFontSize) {
                            elementFontSize = element.css('fontSize');
                            element.data('fontSize', elementFontSize);
                        }
                        var fontWeight = element.css('fontWeight');
                        var fontFamily = element.css('fontFamily');
                        var resizeSpan = $('<span >' + text + '</span>');
                        resizeSpan.css({
                            'fontSize': elementFontSize,
                            'fontWeight': fontWeight,
                            'fontFamily': fontFamily,
                            'position': 'absolute',
                            'visibility': 'hidden',
                            'whitespace': 'nowrap'
                        });
                        resizeSpan.appendTo("body");

                        var fontSize = parseInt(resizeSpan.css('font-size'), 10);
                        var width = resizeSpan.outerWidth(true);
                        var targetwidth = element.innerWidth() - padding;

                        var retryCount = 0;
                        while (width > targetwidth) {
                            fontSize--;
                            resizeSpan.css({ 'fontSize': fontSize + 'px' });
                            var newWidth = resizeSpan.outerWidth(true);
                            if (fontSize == 0) break;
                            if (newWidth == width) retryCount++;
                            if (retryCount == 10) break;
                            width = newWidth;
                        }
                        resizeSpan.detach();
                        element.css({ fontSize: fontSize + 'px' });
                    });
                }
            };
        });
}));
