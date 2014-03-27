(function (readyfunction) {
    readyfunction(window, document, window.jQuery);
}(function (window, document, $, undefined) {
    'use strict';

    $.widget('carbonknown.daterange', {
        options: {
            useAbbr: true,
            readOnly: true,
            startTrigger: null,
            endSelector: null,
            endTrigger: null,
            easing: 'slow',
            customClass: 'daterange-custom',
            customSelected: 'ui-state-active',
            dateFormat: 'dd/mm/yy',
            initialEndDate: new Date(),
            settings: {
                prevText: 'Prev',
                nextText: 'Next',
                yearText: 'Year',
                monthText: 'Months between',
                shortYearCutoff: '+10', // Short year values < this are in the current century,
                // > this are in the previous century,
                // string value starting with '+' for current year + value
                monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'], // Names of months for drop-down and formatting
                monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'], // For formatting
                dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'], // For formatting
                dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'], // For formatting
            },
            ranges: [1, 3, 4, 6, 12],
            initialRange: 1
        },
        /* Format a date object into a string value.
         * The format can be combinations of the following:
         * d  - day of month (no leading zero)
         * dd - day of month (two digit)
         * o  - day of year (no leading zeros)
         * oo - day of year (three digit)
         * D  - day name short
         * DD - day name long
         * m  - month of year (no leading zero)
         * mm - month of year (two digit)
         * M  - month name short
         * MM - month name long
         * y  - year (two digit)
         * yy - year (four digit)
         * @ - Unix timestamp (ms since 01/01/1970)
         * ! - Windows ticks (100ns since 01/01/0001)
         * "..." - literal text
         * '' - single quote
         *
         * @param  format string - the desired format of the date
         * @param  date Date - the date value to format
         * @param  settings Object - attributes include:
         *					dayNamesShort	string[7] - abbreviated names of the days from Sunday (optional)
         *					dayNames		string[7] - names of the days from Sunday (optional)
         *					monthNamesShort string[12] - abbreviated names of the months (optional)
         *					monthNames		string[12] - names of the months (optional)
         * @return  string - the date in the above format
         */
        _ticksTo1970: (((1970 - 1) * 365 + Math.floor(1970 / 4) - Math.floor(1970 / 100) + Math.floor(1970 / 400)) * 24 * 60 * 60 * 10000000),
        formatDate: function(format, date, settings) {
            if (!date) {
                return "";
            }

            var iFormat,
                dayNamesShort = (settings ? settings.dayNamesShort : null) || this.option.settings.dayNamesShort,
                dayNames = (settings ? settings.dayNames : null) || this.option.settings.dayNames,
                monthNamesShort = (settings ? settings.monthNamesShort : null) || this.option.settings.monthNamesShort,
                monthNames = (settings ? settings.monthNames : null) || this.option.settings.monthNames,
                // Check whether a format character is doubled
                lookAhead = function(match) {
                    var matches = (iFormat + 1 < format.length && format.charAt(iFormat + 1) === match);
                    if (matches) {
                        iFormat++;
                    }
                    return matches;
                },
                // Format a number, with leading zero if necessary
                formatNumber = function(match, value, len) {
                    var num = "" + value;
                    if (lookAhead(match)) {
                        while (num.length < len) {
                            num = "0" + num;
                        }
                    }
                    return num;
                },
                // Format a name, short or long as requested
                formatName = function(match, value, shortNames, longNames) {
                    return (lookAhead(match) ? longNames[value] : shortNames[value]);
                },
                output = "",
                literal = false;

            if (date) {
                for (iFormat = 0; iFormat < format.length; iFormat++) {
                    if (literal) {
                        if (format.charAt(iFormat) === "'" && !lookAhead("'")) {
                            literal = false;
                        } else {
                            output += format.charAt(iFormat);
                        }
                    } else {
                        switch (format.charAt(iFormat)) {
                        case "d":
                            output += formatNumber("d", date.getDate(), 2);
                            break;
                        case "D":
                            output += formatName("D", date.getDay(), dayNamesShort, dayNames);
                            break;
                        case "o":
                            output += formatNumber("o",
                                Math.round((new Date(date.getFullYear(), date.getMonth(), date.getDate()).getTime() - new Date(date.getFullYear(), 0, 0).getTime()) / 86400000), 3);
                            break;
                        case "m":
                            output += formatNumber("m", date.getMonth() + 1, 2);
                            break;
                        case "M":
                            output += formatName("M", date.getMonth(), monthNamesShort, monthNames);
                            break;
                        case "y":
                            output += (lookAhead("y") ? date.getFullYear() :
                                (date.getYear() % 100 < 10 ? "0" : "") + date.getYear() % 100);
                            break;
                        case "@":
                            output += date.getTime();
                            break;
                        case "!":
                            output += date.getTime() * 10000 + this._ticksTo1970;
                            break;
                        case "'":
                            if (lookAhead("'")) {
                                output += "'";
                            } else {
                                literal = true;
                            }
                            break;
                        default:
                            output += format.charAt(iFormat);
                        }
                    }
                }
            }
            return output;
        },
        /* Handle switch to/from daylight saving.
         * Hours may be non-zero on daylight saving cut-over:
         * > 12 when midnight changeover, but then cannot generate
         * midnight datetime, so jump to 1AM, otherwise reset.
         * @param  date  (Date) the date to check
         * @return  (Date) the corrected date
         */
        _daylightSavingAdjust: function(date) {
            if (!date) {
                return null;
            }
            date.setHours(date.getHours() > 12 ? date.getHours() + 2 : 0);
            return date;
        },
        /* Find the number of days in a given month. */
        _getDaysInMonth: function(year, month) {
            return 32 - this._daylightSavingAdjust(new Date(year, month, 32)).getDate();
        },
        /* Parse a string value into a date object.
        * See formatDate below for the possible formats.
        *
         * @param  format string - the expected format of the date
         * @param  value string - the date in the above format
         * @param  settings Object - attributes include:
         *					shortYearCutoff  number - the cutoff year for determining the century (optional)
         *					dayNamesShort	string[7] - abbreviated names of the days from Sunday (optional)
         *					dayNames		string[7] - names of the days from Sunday (optional)
         *					monthNamesShort string[12] - abbreviated names of the months (optional)
         *					monthNames		string[12] - names of the months (optional)
         * @return  Date - the extracted date value or null if value is blank
         */
        parseDate: function(format, value, settings) {
            if (format == null || value == null) {
                throw "Invalid arguments";
            }

            value = (typeof value === "object" ? value.toString() : value + "");
            if (value === "") {
                return null;
            }

            var iFormat, dim, extra,
                iValue = 0,
                shortYearCutoffTemp = (settings ? settings.shortYearCutoff : null) || this.option.settings.shortYearCutoff,
                shortYearCutoff = (typeof shortYearCutoffTemp !== "string" ? shortYearCutoffTemp :
                    new Date().getFullYear() % 100 + parseInt(shortYearCutoffTemp, 10)),
                dayNamesShort = (settings ? settings.dayNamesShort : null) || this.option.settings.dayNamesShort,
                dayNames = (settings ? settings.dayNames : null) || this.option.settings.dayNames,
                monthNamesShort = (settings ? settings.monthNamesShort : null) || this.option.settings.monthNamesShort,
                monthNames = (settings ? settings.monthNames : null) || this.option.settings.monthNames,
                year = -1,
                month = -1,
                day = -1,
                doy = -1,
                literal = false,
                date,
                // Check whether a format character is doubled
                lookAhead = function(match) {
                    var matches = (iFormat + 1 < format.length && format.charAt(iFormat + 1) === match);
                    if (matches) {
                        iFormat++;
                    }
                    return matches;
                },
                // Extract a number from the string value
                getNumber = function(match) {
                    var isDoubled = lookAhead(match),
                        size = (match === "@" ? 14 : (match === "!" ? 20 :
                            (match === "y" && isDoubled ? 4 : (match === "o" ? 3 : 2)))),
                        digits = new RegExp("^\\d{1," + size + "}"),
                        num = value.substring(iValue).match(digits);
                    if (!num) {
                        throw "Missing number at position " + iValue;
                    }
                    iValue += num[0].length;
                    return parseInt(num[0], 10);
                },
                // Extract a name from the string value and convert to an index
                getName = function(match, shortNames, longNames) {
                    var index = -1,
                        names = $.map(lookAhead(match) ? longNames : shortNames, function(v, k) {
                            return [[k, v]];
                        }).sort(function(a, b) {
                            return -(a[1].length - b[1].length);
                        });

                    $.each(names, function(i, pair) {
                        var name = pair[1];
                        if (value.substr(iValue, name.length).toLowerCase() === name.toLowerCase()) {
                            index = pair[0];
                            iValue += name.length;
                            return false;
                        }
                    });
                    if (index !== -1) {
                        return index + 1;
                    } else {
                        throw "Unknown name at position " + iValue;
                    }
                },
                // Confirm that a literal character matches the string value
                checkLiteral = function() {
                    if (value.charAt(iValue) !== format.charAt(iFormat)) {
                        throw "Unexpected literal at position " + iValue;
                    }
                    iValue++;
                };

            for (iFormat = 0; iFormat < format.length; iFormat++) {
                if (literal) {
                    if (format.charAt(iFormat) === "'" && !lookAhead("'")) {
                        literal = false;
                    } else {
                        checkLiteral();
                    }
                } else {
                    switch (format.charAt(iFormat)) {
                    case "d":
                        day = getNumber("d");
                        break;
                    case "D":
                        getName("D", dayNamesShort, dayNames);
                        break;
                    case "o":
                        doy = getNumber("o");
                        break;
                    case "m":
                        month = getNumber("m");
                        break;
                    case "M":
                        month = getName("M", monthNamesShort, monthNames);
                        break;
                    case "y":
                        year = getNumber("y");
                        break;
                    case "@":
                        date = new Date(getNumber("@"));
                        year = date.getFullYear();
                        month = date.getMonth() + 1;
                        day = date.getDate();
                        break;
                    case "!":
                        date = new Date((getNumber("!") - this._ticksTo1970) / 10000);
                        year = date.getFullYear();
                        month = date.getMonth() + 1;
                        day = date.getDate();
                        break;
                    case "'":
                        if (lookAhead("'")) {
                            checkLiteral();
                        } else {
                            literal = true;
                        }
                        break;
                    default:
                        checkLiteral();
                    }
                }
            }

            if (iValue < value.length) {
                extra = value.substr(iValue);
                if (!/^\s+/.test(extra)) {
                    throw "Extra/unparsed characters found in date: " + extra;
                }
            }

            if (year === -1) {
                year = new Date().getFullYear();
            } else if (year < 100) {
                year += new Date().getFullYear() - new Date().getFullYear() % 100 +
                    (year <= shortYearCutoff ? 0 : -100);
            }

            if (doy > -1) {
                month = 1;
                day = doy;
                do {
                    dim = this._getDaysInMonth(year, month - 1);
                    if (day <= dim) {
                        break;
                    }
                    month++;
                    day -= dim;
                } while (true);
            }

            date = this._daylightSavingAdjust(new Date(year, month - 1, day));
            if (date.getFullYear() !== year || date.getMonth() + 1 !== month || date.getDate() !== day) {
                throw "Invalid date"; // E.g. 31/02/00
            }
            return date;
        },
        getStartDate: function() {
            return this.startDate;
        },
        getEndDate: function() {
            return this.endDate;
        },
        _calculateStartDate: function(endDate, range) {
            return new Date(endDate.getFullYear(), endDate.getMonth() - (range - 1), 1);
        },
        _calculateEndDate: function(year, month) {
            var result = new Date(year, month + 1, 1);
            result.setSeconds(0, -1);
            return result;
        },
        _monthDiff: function(startDate, endDate) {
            if (startDate > endDate) throw 'Start Date cannot be larger than End Date';
            var startYear = startDate.getFullYear(),
                startMonth = startDate.getMonth(),
                numberOfMonths = 1,
                enumDate = new Date(endDate),
                enumMonth = enumDate.getMonth(),
                enumYear = enumDate.getFullYear();
            while ((startYear != enumYear) || (startMonth != enumMonth)) {
                enumDate.setMonth(enumMonth - 1);
                enumMonth = enumDate.getMonth();
                enumYear = enumDate.getFullYear();
                numberOfMonths++;
            }
            return numberOfMonths;
        },
        _updateInputs: function() {
            this.startDate = this._calculateStartDate(this.endDate, this.selectedRange);
            var startDateText = this.formatDate(this.options.dateFormat, this.startDate, this.options.settings),
                endDateText = this.formatDate(this.options.dateFormat, this.endDate, this.options.settings);

            this.startDateInput.val(startDateText);
            this.endDateInput.val(endDateText);
            this._trigger('dateselected', this, { startDate: this.startDate, endDate: this.endDate });
        },
        _openSelection: function(panel, inputElement) {
            this._hidePanels();
            this.menuVisible = !this.menuVisible;
            if (!this.menuVisible) return false;
            var offset = inputElement.offset(),
                left = offset.left,
                farRight = left + panel.outerWidth(true),
                windowWidth = $(window).innerWidth(),
                top = inputElement.outerHeight(true) + offset.top;

            if (farRight > windowWidth) {
                left = left - (farRight - windowWidth);
                if (left < 0) left = 0;
            }

            panel.css({ top: top, left: left });
            panel.show(this.options.easing);
            return true;
        },
        _selectRangeTriggered: function(event) {
            event.stopPropagation();
            var widget = event.data,
                panel = widget.rangePanel;
            if (!widget._openSelection.call(widget, panel, widget.startDateInput)) return;
            panel
                .find('#daterange-cellvalue-' + widget.selectedRange)
                .addClass(widget.options.customSelected);
        },
        _setupYearSelect: function(element, newYear, configurePanel) {
            element
                .off('click')
                .on('click', null, { widget: this, newYear: newYear, configurePanel: configurePanel }, this._yearSelected);
        },
        _removeSelection: function(panel) {
            var selectClass = this.options.customSelected;
            panel
                .find('.' + selectClass)
                .removeClass(selectClass);
        },
        _configurePanelForMonths: function() {
            var panel = this.monthYearPanel,
                options = this.options,
                endDate = this.endDate,
                settings = options.settings,
                monthNames = settings.monthNames,
                selectedMonth = endDate.getMonth(),
                year = endDate.getFullYear(),
                monthName = monthNames[selectedMonth];

            this._removeSelection(panel);
            this._setupYearSelect(this.prevCell, year - 1, this._configurePanelForMonths);
            this._setupYearSelect(this.nextCell, year + 1, this._configurePanelForMonths);
            this.monthNameCell.text(monthName);
            if (options.useAbbr) {
                monthNames = settings.monthNamesShort;
            }
            this.yearCell.text(year);
            for (var index = 0; index < 12; index++) {
                var monthCell = this.monthCells[index];
                this._setupMonthCell(
                    monthCell,
                    monthNames[index],
                    (index == selectedMonth),
                    { widget: this, newMonth: index, year: year },
                    this._monthSelected);
            }
        },
        _monthSelected: function(event) {
            event.stopPropagation();
            var widget = event.data.widget,
                newMonth = event.data.newMonth,
                year = widget.endDate.getFullYear();
            widget.endDate = widget._calculateEndDate(year, newMonth);
            widget._closeSelection.call(widget);
        },
        _configurePanelForYears: function() {
            var panel = this.monthYearPanel,
                options = this.options,
                settings = options.settings,
                year = this.endDate.getFullYear();

            this._removeSelection(panel);
            this.monthNameCell.text(settings.yearText);
            this._setupYearSelect(this.prevCell, year - 12, this._configurePanelForYears);
            this._setupYearSelect(this.nextCell, year + 12, this._configurePanelForYears);
            this.yearCell.text(year);
            for (var index = 0; index < 12; index++) {
                var monthCell = this.monthCells[index],
                    newYear = year - (11 - index);
                this._setupMonthCell(
                    monthCell,
                    newYear,
                    (index == 11),
                    { widget: this, newYear: newYear, configurePanel: this._configurePanelForMonths },
                    this._yearSelected);
            }
        },
        _switchPanel: function(event) {
            event.stopPropagation();
            var widget = event.data.widget;
            widget._configurePanelForYears(widget);
        },
        _yearSelected: function(event) {
            event.stopPropagation();

            var widget = event.data.widget,
                newYear = event.data.newYear,
                configurePanel = event.data.configurePanel,
                month = widget.endDate.getMonth();

            widget.endDate = widget._calculateEndDate(newYear, month);
            configurePanel.call(widget);
        },
        _selectMonthTriggered: function(event) {
            event.stopPropagation();
            var widget = event.data;
            if (!widget._openSelection.call(widget, widget.monthYearPanel, widget.endDateInput)) return;
            widget._configurePanelForMonths.call(widget);
        },
        _hidePanels: function() {
            var easing = this.options.easing;
            this.monthYearPanel.hide(easing);
            this.rangePanel.hide(easing);
            this._removeSelection(this.monthYearPanel);
            this._removeSelection(this.rangePanel);
        },
        _closeSelection: function() {
            this._hidePanels();
            this.menuVisible = false;
            this._updateInputs();
        },
        _rangeSelected: function(event) {
            event.stopPropagation();

            var widget = event.data.widget,
                newRange = event.data.range;
            widget.selectedRange = newRange;
            widget._closeSelection.call(widget);
        },
        _setupTrigger: function(selector, element, selectEvent) {
            var inputElement = element;
            var selectElement = $(selector);
            if (selectElement.length) inputElement = selectElement;
            inputElement.on('click', null, this, selectEvent);
        },
        _setupHover: function(element) {
            element.hover(function() {
                $(this).addClass('ui-state-hover');
            }, function() {
                $(this).removeClass('ui-state-hover');
            });
        },
        _setupMonthCell: function(cell, name, selected, data, event) {
            cell.find('a').text(name);
            cell.off('click');
            cell.on('click', null, data, event);
            if (selected) {
                cell.addClass(this.options.customSelected);
            }
        },
        //Private Properies
        self: $.noop(),
        endDateInput: $.noop(),
        startDateInput: $.noop(),
        endDate: $.noop(),
        startDate: $.noop(),
        selectedRange: $.noop(),
        rangePanel: $.noop(),
        monthYearPanel: $.noop(),
        menuVisible: false,
        monthCells: [],
        prevCell: $.noop,
        nextCell: $.noop,
        monthPanel: $.noop,
        monthNameCell: $.noop,
        yearCell: $.noop,
        _create: function() {
            this.self = this;
            var options = this.options,
                dateFormat = options.dateFormat,
                settings = options.settings,
                initialEndDate = options.initialEndDate ?
                    options.initialEndDate :
                    new Date();

            this.endDateInput = $(options.endSelector);
            if (this.endDateInput.length <= 0) throw 'Must have a valid end date selector';
            if (!options.dateFormat) throw 'Must have a valid date format';

            this.startDateInput = this.element;

            var endateValue = this.endDateInput.val();
            if ((endateValue) && (endateValue.length > 0)) {
                initialEndDate = this.parseDate(dateFormat, endateValue, settings);
            }
            this.endDate = this._calculateEndDate(initialEndDate.getFullYear(), initialEndDate.getMonth());

            var startDateValue = this.startDateInput.val();
            if ((startDateValue) && (startDateValue.length > 0)) {
                this.startDate = this.parseDate(dateFormat, startDateValue, settings);
                this.startDate.setDate(1);
                this.selectedRange = this._monthDiff(this.startDate, this.endDate);
            } else {
                if (!options.initialRange) throw 'Must have a valid range in number of months';
                this.startDate = this._calculateStartDate(this.endDate, options.initialRange);
                this.selectedRange = options.initialRange;
            }

            this.rangePanel = $('<div class="' + options.customClass + ' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all" style="position: absolute; z-index: 1; display: block;"><div class="daterange-title ui-datepicker-header ui-widget-header ui-helper-clearfix ui-corner-all"><div class="ui-datepicker-title">' + options.settings.monthText + '</div></div><table class="ui-datepicker-calendar"><tbody><tr class="daterange-monthrange"></tr></tbody></table></div>');
            this.monthYearPanel = $('<div class="' + options.customClass + ' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all" style="position: absolute; z-index: 1; display: block;"><div class="daterange-title ui-datepicker-header ui-widget-header ui-helper-clearfix ui-corner-all"><a class="ui-datepicker-prev ui-corner-all " title="' + options.settings.prevText + '"><span class="ui-icon ui-icon-circle-triangle-w">' + options.settings.prevText + '</span></a><a class="ui-datepicker-next ui-corner-all" title="' + options.settings.nextText + '"><span class="ui-icon ui-icon-circle-triangle-e">' + options.settings.nextText + '</span></a><div class="ui-datepicker-title"><span class="ui-datepicker-month"></span>&nbsp;<span class="ui-datepicker-year"></span></div></div><table class="ui-datepicker-calendar"><tbody><tr><td class="daterange-calendar-0"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-1"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-2"><a class="ui-state-default" href="#"></a></td></tr><tr><td class="daterange-calendar-3"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-4"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-5"><a class="ui-state-default" href="#"></a></td></tr><tr><td class="daterange-calendar-6"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-7"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-8"><a class="ui-state-default" href="#"></a></td></tr><tr><td class="daterange-calendar-9"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-10"><a class="ui-state-default" href="#"></a></td><td class="daterange-calendar-11"><a class="ui-state-default" href="#"></a></td></tr></tbody></table></div>');

            this._setupTrigger(options.startTrigger, this.startDateInput, this._selectRangeTriggered);
            this._setupTrigger(options.endTrigger, this.endDateInput, this._selectMonthTriggered);

            if (options.readOnly) {
                this.endDateInput.attr("readonly", true);
                this.startDateInput.attr("readonly", true);
            }

            var rangeRow = this.rangePanel.find('.daterange-monthrange');
            for (var index = 0; index < options.ranges.length; index++) {
                var range = options.ranges[index];
                var rangeCell = $('<td id="daterange-cellvalue-' + range + '" ><a class="ui-state-default" href="#">' + range + '</a></td>')
                    .appendTo(rangeRow);
                this._setupHover(rangeCell);
                rangeCell.on('click', null, { widget: this, range: range }, this._rangeSelected);
            }

            for (var monthIndex = 0; monthIndex < 12; monthIndex++) {
                var monthCell = this.monthYearPanel.find('.daterange-calendar-' + monthIndex);
                this._setupHover(monthCell);
                this.monthCells.push(monthCell);
            }

            this.monthYearPanel
                .find('.daterange-title')
                .off('click')
                .on('click', null, { widget: this }, this._switchPanel);
            this.yearCell = this.monthYearPanel.find('.ui-datepicker-year');
            this.monthNameCell = this.monthYearPanel.find('.ui-datepicker-month');
            this.prevCell = this.monthYearPanel.find('.ui-datepicker-prev');
            this.nextCell = this.monthYearPanel.find('.ui-datepicker-next');
            this._setupHover(this.prevCell);
            this._setupHover(this.nextCell);
            this.rangePanel.hide();
            this.monthYearPanel.hide();
            this.element
                .parent()
                .append(this.rangePanel)
                .append(this.monthYearPanel);

            this._updateInputs();
        }
    });
}));
