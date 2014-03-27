(function(readyfunction) {
    readyfunction(window, document, window.jQuery);
}(function(window, document, $, undefined) {
    'use strict';
    $.widget('carbonknown.crumbselector', {
        options: {
            menuEasing: 'slow',
            crumbEasing: { easing: 'linear', duration: 3000 },
            customClass: 'crumb-custom',
            selectedClass: 'crumb-selected',
            loadingContent: 'Loading...',
            scrollOptions: { autoHideScrollbar: true },
            loadWalkData: null,
            menuIndent: 30,
            loadChildren: null,
            sourceTimeout: 10000
        },
        _nodeSelected: function(event) {
            var nodeData = event.data.nodeData,
                widget = event.data.widget;

            widget._trigger('nodeselected', widget, nodeData.node);
            widget._toggleMenu();
        },
        _toggleMenu: function(menuButton, leftPosition) {
            var selectedClass = this.options.selectedClass,
                menuEasing = this.options.menuEasing,
                menuPanel = this.menuPanel;

            $('.crumb-panel').hide();
            $('.' + selectedClass).removeClass(selectedClass);
            menuPanel.empty();
            this.menuVisible = !this.menuVisible;
            if ((!this.menuVisible) || (!menuButton)) return false;

            menuButton.addClass(selectedClass);

            menuPanel.css({
                'left': leftPosition,
                'top': this.container.outerHeight(true) + this.container.offset().top
            });
            menuPanel.append('<div class="crumb-loading ' + this.options.customClass + '">' + this.options.loadingContent + '</div>');
            menuPanel.show(menuEasing);
            return true;
        },
        _loadMenu: function(menuItems) {
            var menuPanel = this.menuPanel;
            menuPanel.empty();
            menuPanel.height('auto');
            menuPanel.width('auto');
            var menuContainer = $('<div></div>').appendTo(menuPanel);
            if ((!menuItems) || (menuItems.length <= 0)) {
                this.reflowTreeWalk(this.treeWalkData);
                this._toggleMenu();
                return;
            }

            for (var indexChild = 0; indexChild < menuItems.length; indexChild++) {
                var childNode = menuItems[indexChild],
                    menuNode = $('<div class="crumb-menuitem ' + this.options.customClass + '" >' + childNode.node.contents + '</div>');
                menuNode
                    .on('mouseover', function() { $(this).addClass('hover'); })
                    .on('mouseout', function() { $(this).removeClass('hover'); })
                    .on('click', null, { nodeData: childNode, widget: this }, this._nodeSelected);
                menuContainer.append(menuNode);
            }
            var menuOffset = menuPanel.offset();
            if (this.options.scrollOptions) {
                var newHeight = ($(window).innerHeight() - menuOffset.top);
                if (newHeight < menuPanel.outerHeight(true)) {
                    menuContainer.height(newHeight);
                    menuContainer.mCustomScrollbar(this.options.scrollOptions);
                }
            }
            var windowWidth = $(window).innerWidth(),
                menuWidth = menuPanel.outerWidth(true),
                remainingWidth = windowWidth - menuOffset.left;
            if (remainingWidth < menuWidth) {
                var newLeft = windowWidth - menuWidth;
                if (newLeft < 0) newLeft = 0;
                menuPanel.css('left', newLeft);
            }
        },
        _prevClick: function(event) {
            event.stopPropagation();

            var $this = $(this),
                widget = event.data.widget;

            if (!widget._toggleMenu.call(widget, $this, widget.container.offset().left)) return;
            widget.menuLevel = 0;

            if (event.data.menuNodes) {
                widget._loadMenu.call(widget, event.data.menuNodes);
            }
        },
        _seperatorClick: function(event) {
            event.stopPropagation();

            var $this = $(this),
                widget = event.data.widget,
                button = $this.closest('.crumb-node-container'),
                menuLeft = $this.offset().left - (widget.options.menuIndent),
                sourceCallBack = widget.options.loadChildren;

            if (!sourceCallBack) return;
            if (!widget._toggleMenu.call(widget, button, menuLeft)) return;

            var level = event.data.nodeLevel + 1;
            widget.menuLevel = level;
            var clearTimeout = widget._clearTimeout(widget);
            clearTimeout();
            widget.timeoutKey = window.setInterval(clearTimeout, widget.options.sourceTimeout);
            sourceCallBack.call(widget, event.data.node, function(customNodes) {
                if (widget.menuLevel != level) return;
                if (widget.timeoutKey == 0) {
                    if (widget.menuVisible) {
                        widget._toggleMenu.call(widget, button, menuLeft);
                    }
                    return;
                }
                var menuNodes = [];
                for (var customIndex = 0; customIndex < customNodes.length; customIndex++) {
                    menuNodes.push({ nodeLevel: level, node: customNodes[customIndex] });
                }
                widget._loadMenu.call(widget, menuNodes);
            });
        },
        _clearTimeout: function(widget) {
            return function() {
                window.clearInterval(widget.timeoutKey);
                widget.timeoutKey = 0;
            };
        },
        reflowTreeWalk: function (treeWalk) {
            if ((!treeWalk || (treeWalk.length <= 0) || this.treeWalkData === treeWalk)) return;
            var container = this.container,
                options = this.options,
                prevButton = this.prevButton,
                totalWidth = 0,
                hiddenItems = [];

            this.treeWalkData = treeWalk;
            prevButton.appendTo(container);
            var prevButtonWidth = prevButton.outerWidth(true);

            container.width('100%');
            container.empty();
            var containerWidth = container.outerWidth(true);

            for (var index = 0; index < treeWalk.length; index++) {
                var crumb = treeWalk[index],
                    newNode = $('<div class="crumb-node-container" style="width:auto;height:auto;white-space:nowrap;overflow:hidden;float:right;"></div>')
                        .appendTo(container)
                        .on('mouseover', function() { $(this).addClass('hover'); })
                        .on('mouseout', function() { $(this).removeClass('hover'); })
                        .on('click', null, {
                            nodeData: {
                                nodeLevel: (treeWalk.length - index),
                                node: crumb
                            },
                            widget: this
                        }, this._nodeSelected),
                    seperator = $('<div class="crumb-seperator" style="float:right;width:auto;height:auto;"><div class="crumb-seperator-icon"></div></div>')
                        .appendTo(newNode)
                        .click({ nodeLevel: (treeWalk.length - index), node: crumb, widget: this }, this._seperatorClick),
                    seperatorWidth = seperator.outerWidth(true),
                    newContents = crumb.contents,
                    crumbnode = $('<div class="crumb-node" style="float:right;width:auto;height:auto;">' + newContents + '</div>')
                        .appendTo(newNode),
                    crumbNodeWidth = crumbnode.outerWidth(true),
                    newWidth = newNode.outerWidth(true);
                if (index == 0) {
                    newNode.addClass('first-of-type');
                }
                if ((totalWidth + newWidth + prevButtonWidth) > containerWidth) {
                    var newCrumbWidth = containerWidth - (totalWidth + seperatorWidth + prevButtonWidth + (crumbNodeWidth - crumbnode.width()));
                    if (newCrumbWidth >= 0) {
                        var offset = (crumbNodeWidth - newCrumbWidth);
                        crumbnode
                            .width(newCrumbWidth)
                            .css({ 'overflow': 'hidden' })
                            .html('<div style="position:relative">' + newContents + '</div>');
                        var wrap = crumbnode.find('div');

                        crumbnode.on('mouseenter', function() {
                            var animate = $.extend({}, options.crumbEasing);
                            animate.complete = function() {
                                wrap.animate({ left: 0 }, options.crumbEasing);
                            };
                            wrap.animate({ left: '-' + offset }, animate);
                        });

                        hiddenItems = treeWalk.slice(index + 1);
                    } else {
                        newNode.detach();
                        hiddenItems = treeWalk.slice(index);
                    }
                    break;
                }

                totalWidth += newWidth;
            }
            container.width('auto');
            containerWidth = container.outerWidth(true);
            if (hiddenItems.length) {
                var menuNodes = [];
                for (var hiddenIndex = 0; hiddenIndex < hiddenItems.length; hiddenIndex++) {
                    menuNodes.push({ nodeLevel: hiddenItems.length - hiddenIndex, node: hiddenItems[hiddenIndex] });
                }
                prevButton
                    .appendTo(container)
                    .click({ menuNodes: menuNodes, widget: this }, this._prevClick)
                    .on('mouseover', function() { $(this).addClass('hover'); })
                    .on('mouseout', function() { $(this).removeClass('hover'); });
                container.width(containerWidth + prevButtonWidth);
            }
            this.menuVisible = true;
            this._toggleMenu();
        },
        _callLoadWalkData: function(loadWalkData) {
            if (!loadWalkData || (typeof(loadWalkData) !== 'function')) return;
            var self = this;
            var reflow = this.reflowTreeWalk;
            loadWalkData(function(treeWalkData) {
                reflow.call(self, treeWalkData);
            });
        },
        _create: function() {
            var customClass = this.options.customClass;
            this.clone = this.element.clone();
            this.container = $('<div class="crumb-menubar ' + customClass + '" style="white-space:nowrap;height:auto;overflow:hidden;float:left;" ><div></div></div>');
            this.element.replaceWith(this.container);
            this.menuPanel = $('<div class="crumb-panel ' + customClass + '" style="position:absolute;padding:0;margin:0;z-index:9000;width:auto;height:auto;" ></div>').appendTo($('body'));
            this.prevButton = $('<div class="crumb-prev ' + customClass + '" style="float:left;width:auto;height:auto;position:absolute;z-index:1;" ><div class="crumb-prev-icon"></div></div>')
                .on('mouseover', function() { $(this).addClass('hover'); })
                .on('mouseout', function() { $(this).removeClass('hover'); })
                .appendTo(this.container);
            this.treeWalkData = [];
            this.timeoutKey = 0;
            this._callLoadWalkData(this.options.loadWalkData);
            $(window).on('resize', null, this, function(event) {
                var widget = event.data;
                widget.reflowTreeWalk.call(widget, widget.treeWalkData);
            });
        }
    });
}));