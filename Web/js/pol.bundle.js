(function(f){if(typeof exports==="object"&&typeof module!=="undefined"){module.exports=f()}else if(typeof define==="function"&&define.amd){define([],f)}else{var g;if(typeof window!=="undefined"){g=window}else if(typeof global!=="undefined"){g=global}else if(typeof self!=="undefined"){g=self}else{g=this}g.pol = f()}})(function(){var define,module,exports;return (function(){function r(e,n,t){function o(i,f){if(!n[i]){if(!e[i]){var c="function"==typeof require&&require;if(!f&&c)return c(i,!0);if(u)return u(i,!0);var a=new Error("Cannot find module '"+i+"'");throw a.code="MODULE_NOT_FOUND",a}var p=n[i]={exports:{}};e[i][0].call(p.exports,function(r){var n=e[i][1][r];return o(n||r)},p,p.exports,r,e,n,t)}return n[i].exports}for(var u="function"==typeof require&&require,i=0;i<t.length;i++)o(t[i]);return o}return r})()({1:[function(require,module,exports){

let __core = require('./core');

__core.ajax   = require('./core.ajax');
__core.commands    = require('./core.commands');
__core.declarative = require('./core.declarative');
__core.dialogs     = require('./core.dialogs');
__core.events         = require('./core.events');
__core.include     = require('./core.include');
__core.paginator   = require('./core.paginator');
__core.pubSub      = require('./core.pub-sub');
__core.templates   = require('./core.templates');
__core.reportLoader = require('./core.tabbly.engine');
__core.reportEngine = require('./core.tabbly.loader');
__core.jsReportLoader = require('./core.tabbly.v2.engine');
__core.jsReportEngine = require('./core.tabbly.v2.loader');
__core.controls       = { 
  grid       : require('./controls.editable-grid'),
  textViewer : require('./controls.text-viewer'),
  collapsibleBox : require('./controls.collapsible-box'),
  charts: require('./charts/charts')
};
__core.tre            = require('./core.tree');

module.exports = __core;

},{"./charts/charts":3,"./controls.collapsible-box":6,"./controls.editable-grid":7,"./controls.text-viewer":8,"./core":16,"./core.ajax":10,"./core.commands":11,"./core.declarative":12,"./core.dialogs":13,"./core.events":14,"./core.include":15,"./core.paginator":17,"./core.pub-sub":18,"./core.tabbly.engine":19,"./core.tabbly.loader":20,"./core.tabbly.v2.engine":21,"./core.tabbly.v2.loader":22,"./core.templates":23,"./core.tree":24}],2:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var utils_1 = require("./utils");
var math_1 = require("../math");
var core_1 = require("../core");
var STEPS_SCALE_Y = 25;
var COLORS = utils_1.createColors(30);
var BarChart = (function () {
    function BarChart(width, height, data, o) {
        var _this = this;
        this.showBars = true;
        this.showDots = true;
        this.showLine = true;
        this.showValues = true;
        this.closeLines = true;
        this.worldToScreenY = function (y) { return y * _this.ratio * _this.bounds.height / 100; };
        this.getControl = function () { return _this.svg; };
        this.fonts = [];
        this.currentFont = { fontFamily: 'Verdana',
            fontSize: '11px',
            textAnchor: 'middle' };
        this.showBars = o && o.showBars != undefined ? o.showBars : true;
        this.showDots = o && o.showDots != undefined ? o.showDots : true;
        this.showLine = o && o.showLine != undefined ? o.showLine : true;
        this.showValues = o && o.showValues != undefined ? o.showValues : true;
        this.closeLines = o && o.closeLines != undefined ? o.closeLines : true;
        this.width = width;
        this.height = height;
        this.padding = (o && o.padding) ? o.padding : new math_1.Box(0, 0, 0, 0);
        this.bounds = new math_1.Rectangle(this.padding.left, this.padding.top, this.width - this.padding.left - this.padding.right, this.height - this.padding.top - this.padding.bottom);
        var __html = ('<svg viewbox ="0 0 {width} {height}">' +
            '  <defs>' +
            '    <clipPath id="JJJ">' +
            '      <rect y="{bounds.top}"' +
            '            x="{bounds.left}"' +
            '            width="{bounds.width}"' +
            '            height="{0}" />' +
            '     </clipPath>' +
            '  </defs>' +
            '  <g class="lines"></g>' +
            '  <g class="data" style="clip-path:url(#JJJ)"></g>' +
            '  <g class="text"></g>' +
            '</svg>').format(this.bounds.height - 1, this);
        this.svg = core_1.core.build('div', __html, true);
        this.data = data.map(function (values, i) {
            return {
                value: values[0],
                legend: values[1] || '',
                serie: values[2] || '',
                fill: values[3] || COLORS.next()
            };
        });
        this.maxValue = 1.05 * this.data.reduce(function (a, d) { return Math.max(d.value, a); }, -Infinity);
        this.ratio = 100.0 / this.maxValue;
        this.series = Object.entries(this.data.groupBy('serie'))
            .map(function (group) {
            return { key: group[0],
                text: group[0],
                fill: group[1][0].fill,
                rows: group[1] };
        })
            .where(function (s) { return s.key; });
        this.legends = Object.entries(this.data.groupBy('legend'))
            .map(function (group) {
            var __rows = group[1];
            var __color = __rows[0].fill;
            if (_this.series.length) {
                __rows.forEach(function (r) { return r.fill = __color; });
            }
            return { key: group[0],
                text: group[0],
                fill: __color,
                rows: __rows };
        });
        this.calcLayout();
    }
    BarChart.prototype.calcLayout = function () {
        var _this = this;
        var __totalBarWidth = this.bounds.width / this.data.length;
        var __margin = __totalBarWidth * .1;
        var __marginSerie = __margin * 2;
        var __barWidth = __totalBarWidth - __margin / this.data.length;
        var __offset = 0;
        var __computeBars = function (rows) {
            rows.forEach(function (value, i, self) {
                var __height = _this.worldToScreenY(value.value);
                var __top = _this.bounds.top + _this.bounds.height - __height;
                var __left = _this.bounds.left + __margin + __offset;
                ;
                value.bar = new math_1.Rectangle(__left, __top, __barWidth - __margin, __height);
                __offset += __barWidth;
            });
        };
        if (this.series.length) {
            __barWidth -= __marginSerie * (this.series.length) / this.data.length;
            this.series
                .forEach(function (serie) {
                serie.left = _this.bounds.left + __offset;
                __computeBars(serie.rows);
                __offset += __marginSerie;
                serie.width = _this.bounds.left + __offset - serie.left;
                serie.right = _this.bounds.left + __offset - __marginSerie;
            });
        }
        else {
            __computeBars(this.data);
        }
        this.draw();
    };
    BarChart.prototype.draw = function () {
        this.drawAxisY();
        this.drawVerticalLines();
        this.drawAxisX();
        this.drawBars();
        this.drawLine();
        this.drawSeries();
    };
    BarChart.prototype.drawAxisX = function () {
        var __html = ('<line x1="{0}" y1="{1}" x2="{2}" y2="{1}"' +
            '      stroke="black"' +
            '      stroke-width="2" />').format(this.bounds.left - 4, this.bounds.top + this.bounds.height, this.bounds.left + this.bounds.width + 4);
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
    };
    BarChart.prototype.drawAxisY = function () {
        var _this = this;
        var __html = '';
        var __template = '<line x1="{0}" y1="{1}" x2="{2}" y2="{1}"' +
            '      stroke="silver"' +
            '      stroke-width="1"/>';
        this.saveContext();
        this.currentFont.textAnchor = 'end';
        utils_1.niceScale(0, this.maxValue, STEPS_SCALE_Y)
            .values
            .forEach(function (value) {
            var __height = _this.worldToScreenY(value);
            var __top = _this.bounds.top + _this.bounds.height - __height;
            if (__top < _this.bounds.top)
                return;
            __html += __template.format(_this.bounds.left - 4, __top, _this.bounds.left + _this.bounds.width);
            _this.appendText(_this.bounds.left - 6, __top + 4, value.toFixed(0));
        });
        this.restoreContext();
        __html += ('<line x1="{0}" y1="{1}" x2="{0}" y2="{2}"' +
            '      stroke="black"' +
            '      stroke-width="2" />').format(this.bounds.left, this.bounds.top - 2, this.bounds.top + this.bounds.height + 4);
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
    };
    BarChart.prototype.drawVerticalLines = function () {
        var _this = this;
        var __template = '<line x1="{0}" y1="{1}" x2="{0}" y2="{2}"' +
            '      stroke="{3}"' +
            '      stroke-width="1" />';
        var __html = this.data
            .reduce(function (html, item, i) {
            var __point = item.bar.centerPoint();
            return html += __template.format(__point.x, _this.bounds.top - 4, _this.bounds.top + _this.bounds.height
                + 4, 'silver');
        }, '') +
            this.series
                .reduce(function (html, serie, i, self) {
                if (i == self.length - 1)
                    return html;
                return html += __template.format(serie.left + serie.width, _this.bounds.top - 4, _this.bounds.top + _this.bounds.height, 'black');
            }, '');
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
    };
    BarChart.prototype.drawBars = function () {
        var _this = this;
        var __template = '<rect x="{0}" y="{1}" width="{2}" height="{3}"' +
            '      stroke="black"' +
            '      stroke-width="2"' +
            '      fill="{4}"' +
            '      data-index="{5}" />';
        var __html = this.data
            .reduce(function (html, item, i) {
            var rec = item.bar;
            _this.appendText(rec.centerPoint().x, _this.bounds.top + _this.bounds.height + 14, item.legend);
            if (_this.showBars) {
                if (_this.showValues)
                    _this.appendText(rec.centerPoint().x, rec.centerPoint().y, item.value);
                return html += __template.format(rec.left, rec.top, rec.width, rec.height, item.fill, i);
            }
        }, '');
        this.svg
            .querySelector('g.data')
            .insertAdjacentHTML("beforeend", __html);
    };
    BarChart.prototype.drawSeries = function () {
        var _this = this;
        this.saveContext();
        this.currentFont.fontSize = '20px';
        this.currentFont.textAnchor = 'middle';
        this.series
            .forEach(function (serie, i) {
            _this.appendText(serie.left + serie.width / 2, _this.bounds.top + _this.bounds.height + 40, serie.text);
        });
        this.restoreContext();
    };
    BarChart.prototype.drawLine = function () {
        var _this = this;
        var __dots_template = '<circle cx="{0}" cy="{1}" r="3" ' +
            '        stroke="black"' +
            '        stroke-width="1" ' +
            '        fill="white" />';
        var __createDots = function (values, extra) {
            if (_this.showLine && _this.showValues) {
                values.forEach(function (item, i) {
                    var rec = item.bar;
                    _this.appendText(rec.centerPoint().x, rec.top - 12, item.value);
                });
            }
            var __points = values.map(function (v) { return v.bar; })
                .map(function (r) { return new math_1.Vector2(r.centerPoint().x, r.top); })
                .concat(_this.closeLines ? extra : []);
            var __path = utils_1.PathBuilder.createPath(__points, .1, _this.closeLines);
            var __line = ('<path d="{0}" fill="{1}"' +
                '      stroke-dasharray=""' +
                '      stroke="{2}"' +
                '      stroke-width="2"/>').format(__path, _this.closeLines ? COLORS.next() : 'none', 'black');
            var __dots = values.reduce(function (html, item, i) {
                var rec = item.bar;
                return html += __dots_template.format(rec.centerPoint().x, rec.top, item.fill);
            }, '');
            return (_this.showLine ? __line : '') +
                (_this.showDots ? __dots : '');
        };
        var __html = '';
        if (this.series.length) {
            __html = this.series
                .map(function (serie) {
                var extra = [new math_1.Vector2(serie.right, _this.bounds.top + _this.bounds.height),
                    new math_1.Vector2(serie.left, _this.bounds.top + _this.bounds.height)];
                return __createDots(serie.rows, extra);
            })
                .join('');
        }
        else {
            var extra = [new math_1.Vector2(this.bounds.left + this.bounds.width, this.bounds.top + this.bounds.height),
                new math_1.Vector2(this.bounds.left, this.bounds.top + this.bounds.height)];
            __html = __createDots(this.data, extra);
        }
        this.svg
            .querySelector('g.data')
            .insertAdjacentHTML("beforeend", __html);
    };
    BarChart.prototype.saveContext = function () {
        this.fonts.push(core_1.core.clone(this.currentFont));
    };
    BarChart.prototype.restoreContext = function () {
        if (this.fonts.length)
            this.currentFont = this.fonts.pop();
    };
    BarChart.prototype.appendText = function (x, y, text) {
        var __template = '<text x="{0}" y="{1}"' +
            ' font-family="{fontFamily}" ' +
            ' font-size="{fontSize}"' +
            ' text-anchor="{textAnchor}">{2}</text>';
        this.svg
            .querySelector('g.text')
            .insertAdjacentHTML("beforeend", __template.format(x, y, text, this.currentFont));
    };
    return BarChart;
}());
exports.default = BarChart;

},{"../core":16,"../math":25,"./utils":5}],3:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.utils = exports.createDocument = exports.LineChart = exports.BarChart = void 0;
var bars_1 = require("./bars");
exports.BarChart = bars_1.default;
var lines_1 = require("./lines");
exports.LineChart = lines_1.default;
var lines_2 = require("./lines");
Object.defineProperty(exports, "createDocument", { enumerable: true, get: function () { return lines_2.createDocument; } });
var utils = require("./utils");
exports.utils = utils;

},{"./bars":2,"./lines":4,"./utils":5}],4:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createDocument = void 0;
var utils_1 = require("./utils");
var math_1 = require("../math");
var core_1 = require("../core");
var core_pub_sub_1 = require("../core.pub-sub");
var STEPS_SCALE_Y = 8;
var COLORS = utils_1.createColors(30);
var LineChart = (function () {
    function LineChart(width, height, document, o) {
        var _this = this;
        this.getControl = function () { return _this.svg; };
        this.worldToScreenX = function (x) { return _this.bounds.left + (x * _this.ratio.x * _this.bounds.width / 100); };
        this.worldToScreenY = function (y) { return _this.bounds.top + _this.bounds.height - ((y - _this.document.view.y.min) * _this.ratio.y * _this.bounds.height / 100); };
        this.screenToWorldX = function (x) {
            var __x = x * (_this.width / _this.svg.clientWidth);
            return _this.document.view.x.min + (__x - _this.bounds.left) * 100 / (_this.bounds.width * _this.ratio.x);
        };
        this.indexPoinAt = function (distance) {
            var __i = -1;
            _this.document.distances.forEach(function (d) { if (d > distance)
                return; __i++; });
            return __i;
        };
        this.onMouseLeave = function (eventArg) {
            if (_this.mouse.mouseDown) {
                _this.mouse.mouseDown = false;
                _this.mouse.drag = false;
                _this.clearLayer();
            }
        };
        this.onTouchEnd = function (eventArg) {
            var __reset = function () {
                _this.mouse.mouseDown = false;
                _this.mouse.drag = false;
                _this.clearLayer();
                eventArg.preventDefault();
            };
            if (_this.mouse.drag) {
                core_pub_sub_1.default.publish('msg/line_chart/range', {
                    sender: _this,
                    start: _this.mouse.dragStart < 0 ? 0 : _this.mouse.dragStart,
                    end: _this.mouse.dragEnd
                });
                __reset();
                _this.updateLayer();
                return;
            }
            core_pub_sub_1.default.publish('msg/line_chart/tap', {
                sender: _this,
                x: _this.screenToWorldX(_this.mouse.mouseDownPosition.x)
            });
            __reset();
        };
        this.onMouseUp = function (eventArg) {
            var __pos = { x: eventArg.offsetX, y: eventArg.offsetY };
            var __reset = function () {
                _this.mouse.mouseDown = false;
                _this.mouse.drag = false;
                _this.clearLayer();
                eventArg.preventDefault();
            };
            if (_this.mouse.mouseDown && _this.mouse.mouseDownPosition.x == __pos.x
                && _this.mouse.mouseDownPosition.y == __pos.y) {
                core_pub_sub_1.default.publish('msg/line_chart/tap', {
                    sender: _this,
                    position: __pos,
                    x: _this.screenToWorldX(__pos.x)
                });
                return __reset();
            }
            if (_this.mouse.drag) {
                core_pub_sub_1.default.publish('msg/line_chart/range', {
                    sender: _this,
                    start: _this.mouse.dragStart < 0 ? 0 : _this.mouse.dragStart,
                    end: _this.mouse.dragEnd
                });
                __reset();
                _this.updateLayer();
                return;
            }
            __reset();
        };
        this.onTouchStart = function (eventArg) {
            var event = window.document.createEvent("MouseEvent");
            var touch = eventArg.touches[0];
            event.initMouseEvent('mousedown', true, true, window, 1, touch.screenX, touch.screenY, touch.clientX, touch.clientY, false, false, false, false, 0, null);
            touch.target.dispatchEvent(event);
            eventArg.preventDefault();
        };
        this.onMouseDown = function (eventArg) {
            _this.mouse.mouseDown = true;
            _this.mouse.mouseDownPosition = { x: eventArg.offsetX, y: eventArg.offsetY };
            _this.mouse.dragStart = _this.mouse.dragEnd = _this.indexPoinAt(_this.screenToWorldX(_this.mouse.mouseDownPosition.x));
            if (_this.mouse.dragStart == -1)
                _this.mouse.dragStart = 0;
            eventArg.preventDefault();
        };
        this.onTouchMove = function (eventArg) {
            var event = window.document.createEvent("MouseEvent");
            var touch = eventArg.touches[0];
            event.initMouseEvent('mousemove', true, true, window, 1, touch.screenX, touch.screenY, touch.clientX, touch.clientY, false, false, false, false, 0, null);
            touch.target.dispatchEvent(event);
            eventArg.preventDefault();
        };
        this.onMouseMove = function (eventArg) {
            var __pos = { x: eventArg.offsetX, y: eventArg.offsetY };
            _this.mouse.drag = _this.mouse.mouseDown;
            if (_this.mouse.drag) {
                _this.mouse.dragEnd = _this.indexPoinAt(_this.screenToWorldX(__pos.x));
                if (_this.mouse.dragEnd == -1)
                    _this.mouse.dragEnd = 0;
                _this.updateLayerDrag(__pos);
            }
            eventArg.preventDefault();
        };
        this.states = [];
        this.currentFont = { fontFamily: 'Verdana',
            fontSize: '11px',
            textAnchor: 'middle' };
        this.mouse = {};
        this.width = width;
        this.height = height;
        this.padding = (o && o.padding) ? o.padding : new math_1.Box(0, 0, 0, 0);
        this.bounds = new math_1.Rectangle(this.padding.left, this.padding.top, this.width - this.padding.left - this.padding.right, this.height - this.padding.top - this.padding.bottom);
        var __html = ('<svg viewbox ="0 0 {width} {height}">' +
            '  <defs>' +
            '    <clipPath id="JJJ">' +
            '      <rect y="0"' +
            '            x="{bounds.left}"' +
            '            width="{bounds.width}"' +
            '            height="{0}" />' +
            '     </clipPath>' +
            '  </defs>' +
            '  <g class="lines"></g>' +
            '  <g class="data" style="clip-path:url(#JJJ)"></g>' +
            '  <g class="layer" style="clip-path:url(#JJJ)">' +
            '    <rect x="0" y="0" width="0" height="{0}" stroke="none"' +
            '       fill="rgb(0,0,200,.5)" />' +
            '  </g>' +
            '  <g class="text"></g>' +
            '</svg>').format(this.bounds.height + this.bounds.top - 1, this);
        this.svg = core_1.core.build('div', __html, true);
        this.svg.onmousemove = this.onMouseMove;
        this.svg.onmouseup = this.onMouseUp;
        this.svg.onmousedown = this.onMouseDown;
        this.svg.onmouseleave = this.onMouseLeave;
        this.svg.ontouchstart = this.onTouchStart;
        this.svg.ontouchend = this.onTouchEnd;
        this.svg.ontouchmove = this.onTouchMove;
        this.layer = core_1.core.element('g.layer rect', this.svg);
        this.document = document;
        this.ratio = new math_1.Vector2(100.0 / this.document.view.x.range, 100.0 / this.document.view.y.range);
        this.draw();
    }
    LineChart.prototype.draw = function () {
        this.drawScaleY();
        this.drawScaleX();
        this.drawAxes();
        this.drawLines();
    };
    LineChart.prototype.drawAxes = function () {
        var __h_tmp = '<line x1="{0}" y1="{1}" x2="{2}" y2="{1}" stroke="black" stroke-width="2" />';
        var __v_tmp = '<line x1="{0}" y1="{1}" x2="{0}" y2="{2}" stroke="black" stroke-width="2" />';
        var __html = __h_tmp.format(this.bounds.left - 4, this.bounds.top + this.bounds.height, this.bounds.left + this.bounds.width + 4) +
            __v_tmp.format(this.bounds.left, this.bounds.top - 2, this.bounds.top + this.bounds.height + 4);
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
    };
    LineChart.prototype.drawScaleY = function () {
        var _this = this;
        var __html = '';
        var __template = '<line x1="{0}" y1="{1}" x2="{2}" y2="{1}" stroke="silver" stroke-width="1"/>';
        var __right = this.bounds.left + this.bounds.width;
        this.saveContext();
        this.currentFont.fontSize = '9px';
        this.currentFont.textAnchor = 'end';
        var __serie = this.document.series[0];
        var __scale = utils_1.niceScale(__serie.view.min, __serie.view.max, STEPS_SCALE_Y);
        __scale.values
            .forEach(function (value) {
            var __y = __serie.transform ? __serie.transform(_this, value) : _this.worldToScreenY(value);
            if (__y < _this.bounds.top)
                return;
            __html += __template.format(_this.bounds.left - 4, __y, __right);
            var __text = '{0} {1}'.format(value.toFixed(0), (__serie.unit || 'm'));
            _this.appendText(_this.bounds.left - 6, __y + 3, __text);
        });
        if (this.document.series[1]) {
            this.currentFont.textAnchor = 'start';
            __serie = this.document.series[1];
            __scale = utils_1.niceScale(__serie.view.min, __serie.view.max, STEPS_SCALE_Y);
            __scale.values
                .forEach(function (value) {
                var __y = __serie.transform ? __serie.transform(_this, value) : _this.worldToScreenY(value);
                if (__y < _this.bounds.top ||
                    __y > _this.bounds.top + _this.bounds.height)
                    return;
                __html += __template.format(__right - 4, __y, __right);
                var __text = '{0} {1}'.format(value.toFixed(0), (__serie.unit || 'm'));
                _this.appendText(__right + 2, __y + 3, __text);
            });
        }
        this.restoreContext();
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
    };
    LineChart.prototype.drawScaleX = function () {
        var _this = this;
        this.saveContext();
        var __v_tmp = '<line x1="{0}" y1="{1}" x2="{0}" y2="{2}" stroke="gray" stroke-width="1" />';
        var __offsetX = this.worldToScreenX(this.document.view.x.min) - this.bounds.left;
        var __html = '';
        var __scale = utils_1.niceScale(this.document.view.x.min, this.document.view.x.max, Math.floor(this.bounds.width / 50));
        this.currentFont.fontSize = '9px';
        __scale.values
            .forEach(function (value) {
            var __x_pos = _this.worldToScreenX(value) - __offsetX;
            if (__x_pos < _this.bounds.left ||
                __x_pos > _this.bounds.left + _this.bounds.width)
                return;
            __html += __v_tmp.format(__x_pos, _this.bounds.top - 4, _this.bounds.top + _this.bounds.height + 4);
            _this.appendText(__x_pos, _this.bounds.top + _this.bounds.height + 12, '{0} km'.format((value / 1000).toFixed(1)));
        });
        this.svg
            .querySelector('g.lines')
            .insertAdjacentHTML("beforeend", __html);
        this.restoreContext();
    };
    LineChart.prototype.drawLines = function () {
        var _this = this;
        var __offsetX = this.worldToScreenX(this.document.view.x.min) - this.bounds.left;
        var __html = '';
        var __dots_template = '<circle cx="{0}" cy="{1}" r="3" ' +
            '        stroke="black"' +
            '        stroke-width="1" ' +
            '        fill="white" />';
        this.document
            .series
            .forEach(function (serie, i) {
            var __points = _this.document[serie.name]
                .map(function (v, i) {
                var x = _this.worldToScreenX(_this.document.distances[i]);
                var y = serie.transform ? serie.transform(_this, v)
                    : _this.worldToScreenY(v);
                return new math_1.Vector2(x - __offsetX, y);
            });
            var __points_html = __points.reduce(function (html, item, i, self) {
                return html += __dots_template.format(item.x, item.y, 'white');
            }, '');
            var __y = _this.worldToScreenY(serie.view.min);
            var __x_min = _this.worldToScreenX(_this.document.view.x.min) - __offsetX;
            var __x_max = _this.worldToScreenX(_this.document.view.x.max) - __offsetX;
            __points = __points.concat(serie.closeLine ? [new math_1.Vector2(__x_max, __y),
                new math_1.Vector2(__x_min, __y)]
                : []);
            var __path = utils_1.PathBuilder.createPath(__points, .1, serie.closeLine);
            var __line_html = ('<path d="{0}" fill="{1}" ' +
                '      stroke-dasharray="" ' +
                '      stroke="{2}" ' +
                '      stroke-width="{3}" />').format(__path, serie.closeLine ? serie.fillStyle || COLORS.next() : 'none', serie.strokeStyle || 'black', serie.lineWidth || 1);
            __html += __line_html + __points_html;
        });
        this.svg
            .querySelector('g.data')
            .insertAdjacentHTML("beforeend", __html);
    };
    LineChart.prototype.updateLayerDrag = function (pos) {
        var x0 = (this.width / this.svg.clientWidth) * Math.min(pos.x, this.mouse.mouseDownPosition.x);
        var x1 = (this.width / this.svg.clientWidth) * Math.max(pos.x, this.mouse.mouseDownPosition.x);
        this.layer.setAttribute('x', x0.toString());
        this.layer.setAttribute('width', (x1 - x0).toString());
    };
    LineChart.prototype.updateLayer = function () {
        var x0 = this.worldToScreenX(this.document.distances[Math.min(this.mouse.dragStart, this.mouse.dragEnd)]);
        var x1 = this.worldToScreenX(this.document.distances[Math.max(this.mouse.dragStart, this.mouse.dragEnd)]);
        this.layer.setAttribute('x', x0.toString());
        this.layer.setAttribute('width', (x1 - x0).toString());
    };
    LineChart.prototype.clearLayer = function () {
        this.layer.setAttribute('width', "0");
    };
    LineChart.prototype.saveContext = function () {
        this.states.push(core_1.core.clone(this.currentFont));
    };
    LineChart.prototype.restoreContext = function () {
        if (this.states.length)
            this.currentFont = this.states.pop();
    };
    LineChart.prototype.appendText = function (x, y, text) {
        var __template = '<text x="{0}" y="{1}"' +
            ' font-family="{fontFamily}" ' +
            ' font-size="{fontSize}"' +
            ' text-anchor="{textAnchor}">{2}</text>';
        this.svg
            .querySelector('g.text')
            .insertAdjacentHTML("beforeend", __template.format(x, y, text, this.currentFont));
    };
    return LineChart;
}());
exports.default = LineChart;
function createDocument(dataset) {
    function __getRange(array, start, end) {
        var __res = {
            min: Number.POSITIVE_INFINITY,
            max: Number.NEGATIVE_INFINITY,
            range: undefined
        };
        var __current = start;
        while (__current <= end) {
            __res.min = Math.min(__res.min, array[__current]);
            __res.max = Math.max(__res.max, array[__current]);
            __current++;
        }
        __res.min = __res.min;
        __res.range = __res.max - __res.min;
        return __res;
    }
    var document = {
        series: [],
        length: dataset.streams.distance.data.length,
        distances: dataset.streams.distance.data,
        altitude: dataset.streams.altitude.data,
        s2: dataset.streams.s2.data,
        offset: 0.0,
        view: {},
        getRange: __getRange,
        configureView: function (start, end) {
            document.view.start = start;
            document.view.end = end;
            document.view.x = {};
            document.view.x.max = document.distances[end] * 1.005;
            document.view.x.min = document.distances[start];
            document.view.x.range = document.view.x.max - document.view.x.min;
            document.view.y = __getRange(document.altitude, start, end);
            document.view.h = __getRange(document.s2, start, end);
            document.series = [];
            document.series.push({ name: 'altitude',
                closeLine: true,
                showDots: true,
                unit: 'm',
                view: document.view.y,
                fillStyle: 'rgba(225,125,125,.8)',
                lineWidth: 1,
                strokeStyle: 'rgba(0,0,0,.8)',
                ratio: 100.0 / document.view.y.range });
            document.series.push({ name: 's2',
                closeLine: false,
                showDots: false,
                unit: 'ppm',
                view: document.view.h,
                fillStyle: 'rgba(150,0,0,.8)',
                lineWidth: 3,
                strokeStyle: 'rgba(00,0,250,.9)',
                ratio: 100.0 / document.view.h.range, transform: function (sender, v) {
                    return sender.bounds.top +
                        sender.bounds.height -
                        ((v - sender.document.view.h.min) *
                            this.ratio * sender.bounds.height / 120);
                } });
            return document;
        },
        resetView: function () {
            return document.configureView(0, document.length - 1);
        }
    };
    return document.resetView();
}
exports.createDocument = createDocument;

},{"../core":16,"../core.pub-sub":18,"../math":25,"./utils":5}],5:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.describeArc = exports.PathBuilder = exports.niceScale = exports.createColors = void 0;
var math_1 = require("../math");
exports.createColors = function (v) {
    var __v = [0, 51, 102, 153, 204, 255];
    var __l = __v.length - 1;
    var __c = [];
    var __x = 0;
    while (__x < v) {
        __c.add('rgba({0},{1},{2},.9)'.format(__v[~~math_1.Random(__l, 0)], __v[~~math_1.Random(__l, 0)], __v[~~math_1.Random(__l, 0)]));
        __x++;
    }
    return {
        current: -1,
        values: __c,
        next: function () {
            this.current = ++this.current % this.values.length;
            return this.values[this.current];
        }
    };
};
exports.niceScale = function (min, max, steps) {
    var range = __niceNum(max - min, false);
    var tickSpacing = __niceNum(range / (steps - 1), true);
    var niceMin = Math.floor(min / tickSpacing) * tickSpacing;
    var niceMax = Math.ceil(max / tickSpacing) * tickSpacing;
    var result = { range: range,
        min: niceMin,
        max: niceMax,
        tickSpacing: tickSpacing, values: Array() };
    function __niceNum(range, round) {
        var exponent = Math.floor(Math.log10(range));
        var fraction = range / Math.pow(10, exponent);
        var niceFraction;
        if (round) {
            if (fraction < 1.5)
                return Math.pow(10, exponent);
            else if (fraction < 3)
                return 2 * Math.pow(10, exponent);
            else if (fraction < 7)
                return 5 * Math.pow(10, exponent);
            else
                return 10 * Math.pow(10, exponent);
        }
        if (fraction <= 1)
            return Math.pow(10, exponent);
        else if (fraction <= 2)
            return 2 * Math.pow(10, exponent);
        else if (fraction <= 5)
            return 5 * Math.pow(10, exponent);
        else
            return 10 * Math.pow(10, exponent);
    }
    for (var x = result.max; x > result.min; x -= result.tickSpacing) {
        result.values.push(x);
    }
    return result;
};
var PathBuilder = (function () {
    function PathBuilder() {
    }
    PathBuilder.line = function (a, b) {
        var lengthX = b.x - a.x;
        var lengthY = b.y - a.y;
        return {
            length: Math.sqrt(Math.pow(lengthX, 2) + Math.pow(lengthY, 2)),
            angle: Math.atan2(lengthY, lengthX)
        };
    };
    PathBuilder.controlPoint = function (line, smooth) { return function (current, previous, next, reverse) {
        var p = previous || current;
        var n = next || current;
        var l = line(p, n);
        var angle = l.angle + (reverse ? Math.PI : 0);
        var length = l.length * smooth;
        var x = current.x + Math.cos(angle) * length;
        var y = current.y + Math.sin(angle) * length;
        return new math_1.Vector2(x, y);
    }; };
    PathBuilder.bezierCommand = function (controlPoint) { return function (point, i, a) {
        var cps = controlPoint(a[i - 1], a[i - 2], point);
        var cpe = controlPoint(point, a[i - 1], a[i + 1], true);
        return 'C {0},{1} {2},{3} {x},{y}'.format(cps.x, cps.y, cpe.x, cpe.y, point);
    }; };
    PathBuilder.svgPath = function (points, command, closePath) {
        return points.reduce(function (acc, e, i, a) {
            if (i == 0)
                return 'M {x},{y}'.format(e);
            if (closePath && i == a.length - 2 ||
                closePath && i == a.length - 1)
                return acc += ' L {x},{y}'.format(e);
            return acc += ' ' + command(e, i, a);
        }, '') + (closePath ? ' z' : '');
    };
    PathBuilder.createPath = function (points, smoothing, closePath) {
        if (closePath === void 0) { closePath = true; }
        var bezierCommandCalc = PathBuilder.bezierCommand(PathBuilder.controlPoint(PathBuilder.line, smoothing));
        return PathBuilder.svgPath(points, bezierCommandCalc, closePath);
    };
    return PathBuilder;
}());
exports.PathBuilder = PathBuilder;
exports.describeArc = function (x, y, radius, startAngle, endAngle) {
    var start = math_1.polarToCartesian(x, y, radius, endAngle - 90);
    var end = math_1.polarToCartesian(x, y, radius, startAngle - 90);
    var arcSweep = endAngle - startAngle <= 180 ? "0" : "1";
    var d = [
        "M", start.x, start.y,
        "A", radius, radius, 0, arcSweep, 0, end.x, end.y,
        "L", x, y,
        "L", start.x, start.y
    ].join(" ");
    return d;
};

},{"../math":25}],6:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CollapsibleBox = void 0;
var core_1 = require("./core");
var core_events_1 = require("./core.events");
var __template = '<div id="collapsible-box-{0}" class="w3-border">' +
    '  <button class="w3-block w3-border-0" style="outline-style:none">{1}<i style="margin: 2px;" class="fa fa-chevron-down w3-right"></i></button>' +
    '  <div class="w3-hide w3-border-top" style="overflow:auto"></div>' +
    '</div>';
var __counter = 0;
var CollapsibleBox = (function () {
    function CollapsibleBox(titulo, content, onexpand, height) {
        var _this = this;
        if (titulo === void 0) { titulo = 't�tulo'; }
        if (onexpand === void 0) { onexpand = function (sender) { }; }
        if (height === void 0) { height = '10em'; }
        this.loaded = false;
        this.collapsed = true;
        this.onexpand = new core_events_1.CoreEvent('CollapsibleBox.onexpand');
        this._onExpand = function (sender) { };
        this._control = core_1.core.build('div', { innerHTML: __template.format(++__counter, titulo) }, true);
        this._header = this._control.querySelector('button');
        this._body = this._control.querySelector('div');
        this._header.onclick = function (event) {
            _this.collapsed ? _this.expand() : _this.collapse();
        };
        if (height != '-') {
            this._body.style.height = height;
        }
        if (content) {
            this.setContent(content);
        }
        this._onExpand = onexpand;
    }
    CollapsibleBox.create = function (titulo, height) {
        if (titulo === void 0) { titulo = 't�tulo'; }
        if (height === void 0) { height = '10em'; }
        return new CollapsibleBox(titulo, '', undefined, height);
    };
    CollapsibleBox.prototype.hide = function () {
        this.collapse();
        this._header.classList.add('w3-hide');
        return this;
    };
    CollapsibleBox.prototype.show = function () {
        this._header.classList.remove('w3-hide');
        return this;
    };
    CollapsibleBox.prototype.appendTo = function (parent) {
        parent.appendChild(this._control);
        return this;
    };
    CollapsibleBox.prototype.collapse = function () {
        this._body.classList.add('w3-hide');
        var __e = this._header
            .querySelector('i');
        __e.classList.remove('fa-chevron-up');
        __e.classList.add('fa-chevron-down');
        this.collapsed = true;
        return this;
    };
    CollapsibleBox.prototype.expand = function () {
        this._body.classList.remove('w3-hide');
        var __e = this._header
            .querySelector('i');
        __e.classList.remove('fa-chevron-down');
        __e.classList.add('fa-chevron-up');
        if (this._onExpand)
            this._onExpand(this);
        this.onexpand.dispatch(this);
        this.collapsed = false;
        return this;
    };
    CollapsibleBox.prototype.getControl = function () {
        return this._control;
    };
    CollapsibleBox.prototype.getBody = function () {
        return this._body;
    };
    CollapsibleBox.prototype.setContent = function (value) {
        if (core_1.core.isString(value)) {
            this._body.innerHTML = value;
        }
        else {
            this._body.innerHTML = '';
            this._body.appendChild(value);
        }
        return this;
    };
    return CollapsibleBox;
}());
exports.CollapsibleBox = CollapsibleBox;

},{"./core":16,"./core.events":14}],7:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.EditableGrid = void 0;
var core_pub_sub_1 = require("./core.pub-sub");
var EditableGrid = (function () {
    function EditableGrid(table, onFocus, onChange) {
        var _this = this;
        this.currentIndex = -1;
        this.previous = undefined;
        this.table = table;
        var __onfocus = function (e) {
            var __div = e.target;
            var __td = __div.parentNode;
            var __tr = __td.parentNode;
            _this.previous = __div.textContent.trim();
            _this.currentIndex = __tr.rowIndex;
            var __eventArg = {
                tr: __tr,
                td: __td,
                target: __div,
                current: _this.previous
            };
            core_pub_sub_1.default.publish(EditableGrid.OnfocusMessage, __eventArg);
            if (onFocus)
                onFocus(_this, __eventArg);
        };
        var __onblur = function (e) {
            var __div = e.target;
            var __td = __div.parentNode;
            var __tr = __td.parentNode;
            if (_this.previous != undefined &&
                _this.previous != __td.textContent.trim()) {
                var __eventArg = {
                    tr: __tr,
                    td: __td,
                    target: __div,
                    previous: _this.previous,
                    current: __div.textContent.trim()
                };
                core_pub_sub_1.default.publish(EditableGrid.OnChangeMessage, __eventArg);
                if (onChange)
                    onChange(_this, __eventArg);
                _this.previous = undefined;
            }
            ;
            __div.style.outline = '1px solid transparent';
        };
        table.querySelectorAll('td div[contenteditable]')
            .toArray()
            .forEach(function (e) {
            e.onblur = __onblur;
            e.onfocus = __onfocus;
        });
        table.onkeypress = function (e) {
            if (e.keyCode == 13) {
                if (e.preventDefault)
                    e.preventDefault();
                return false;
            }
        };
        table.onkeydown = function (e) {
            var __res = true;
            var __sender = e.target;
            if (__sender.tagName == 'DIV' && [13, 37, 38, 39, 40].indexOf(e.keyCode) > -1) {
                var __div = __sender;
                var __td = __div.parentNode;
                var __row = __td.parentNode;
                var __pos = window.getSelection().getRangeAt(0).startOffset;
                var __focus = function (t, r, c) {
                    e.preventDefault();
                    try {
                        t.rows[r].cells[c].firstElementChild.focus();
                    }
                    catch (e) { }
                    __res = false;
                };
                if (e.keyCode == 13)
                    __focus(table, __row.rowIndex, __td.cellIndex + 1);
                if (e.keyCode == 38 && __row.rowIndex > 1)
                    __focus(table, __row.rowIndex - 1, __td.cellIndex);
                if (e.keyCode == 40 && __row.rowIndex < table.rows.length - 1)
                    __focus(table, __row.rowIndex + 1, __td.cellIndex);
                if (e.keyCode == 39 && __pos == __sender.textContent.length)
                    __focus(table, __row.rowIndex, __td.cellIndex + 1);
                if (e.keyCode == 37 && __pos == 0)
                    __focus(table, __row.rowIndex, __td.cellIndex - 1);
            }
            return __res;
        };
    }
    EditableGrid.OnfocusMessage = 'editable-grid/focus';
    EditableGrid.OnChangeMessage = 'editable-grid/change';
    return EditableGrid;
}());
exports.EditableGrid = EditableGrid;

},{"./core.pub-sub":18}],8:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TextViewer = void 0;
var core_1 = require("./core");
var core_events_1 = require("./core.events");
var controls_text_viewer_ts_css_1 = require("./controls.text-viewer.ts.css");
var __data_Uri = 'data:text/css;base64,' + window.btoa(controls_text_viewer_ts_css_1.default);
var __template = '<div class="scv_Main">' +
    '  <pre class="scv_LineContainer" id="svc_{0}_line"></pre>' +
    '  <pre class="scv_TextContainer" id="svc_{0}_code"></pre>' +
    '</div>';
var __counter = 0;
var __css = false;
function __initCss() {
    if (__css)
        return;
    document.querySelector('head')
        .appendChild(core_1.core.build('link', { rel: 'stylesheet',
        type: 'text/css',
        href: __data_Uri }));
    __css = true;
}
var TextViewer = (function () {
    function TextViewer() {
        var _this = this;
        this.id = 'svc_{0}'.format(++__counter);
        __initCss();
        this._control = core_1.core.build('div', { className: 'svc_viewer',
            id: this.id,
            innerHTML: __template.format(__counter) });
        this._control
            .querySelector('.scv_Main')
            .onscroll = function (event) {
            var __target = event.target;
            _this._control
                .querySelector('.scv_LineContainer')
                .style.left = '{0}px'.format(__target.scrollLeft);
        };
        this.onclick = new core_events_1.CoreEvent('txt-viewer.onclick');
    }
    TextViewer.prototype.setContent = function (value) {
        var _this = this;
        this._control
            .querySelector('.scv_LineContainer')
            .innerHTML = value.replace(/(\r\n|\r|\n)/mg, '\n')
            .split('\n')
            .reduce(function (a, _, i) { return a += (i + 1) + '<br/>'; }, '');
        var __div = this._control.querySelector('.scv_TextContainer');
        __div.textContent = value;
        __div.onclick = function (e) { return _this.onclick.dispatch(e); };
        return this;
    };
    TextViewer.prototype.getControl = function () {
        return this._control;
    };
    return TextViewer;
}());
exports.TextViewer = TextViewer;

},{"./controls.text-viewer.ts.css":9,"./core":16,"./core.events":14}],9:[function(require,module,exports){
module.exports = '\r\n.svc_viewer{overflow:hidden;}\r\n.scv_Main  {position:absolute;top:0;left:0;right:0;bottom:0;overflow:auto;padding:0;}     \r\n.scv_TextContainer{ position:absolute;top:0;left:4.4em;right:0;height:auto;z-index:4;margin:0;user-select: text; }\r\n.scv_TextContainer{ padding:.4em;white-space:pre;overflow:initial;font-family:Monospace;tab-size:4;}\r\n.scv_LineContainer{ padding:.4em;position:absolute;top:0;left:0;height:auto;margin:0;z-index:5;overflow:hidden;background-color:lightyellow;border-right:solid 1px silver;}\r\n.scv_LineContainer{ font-weight:bold;font-family:Monospace;color:Gray;text-align:right;width:3.5em;box-sizing:border-box;user-select:none }\r\n';
},{}],10:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ajax = void 0;
var ajax = {
    get: function (url, interceptor) {
        return new Promise(function (resolve, reject) {
            var xml = ajax.createXMLHttpRequest();
            xml.open('GET', url, true);
            if (interceptor)
                interceptor(xml);
            xml.onreadystatechange = function () {
                if (xml.readyState == 4) {
                    resolve(xml.responseText);
                }
            };
            xml.onerror = function (e) { reject(e); };
            xml.send(null);
        });
    },
    post: function (url, body, interceptor) {
        return new Promise(function (resolve, reject) {
            var xml = ajax.createXMLHttpRequest();
            xml.open('POST', url, true);
            if (interceptor) {
                interceptor(xml);
            }
            else {
                xml.setRequestHeader('Content-type', 'application/x-www-form-urlencoded; charset:ISO-8859-1');
            }
            xml.onreadystatechange = function () { if (xml.readyState == 4)
                resolve(xml.responseText); };
            xml.send(body);
        });
    },
    callWebMethod: function (url, body, callBack) {
        var xml = ajax.createXMLHttpRequest();
        xml.open('POST', url, true);
        xml.onreadystatechange = function () { if (xml.readyState == 4)
            callBack(xml.responseText); };
        xml.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xml.send(body);
    },
    createXMLHttpRequest: function () {
        return new XMLHttpRequest();
    }
};
exports.ajax = ajax;

},{}],11:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CommandManager = void 0;
function CommandManager(doc) {
    var _this = {
        getDocument: function () { return doc; },
        undos: new Array(),
        redos: new Array(),
        clear: function () {
            _this.undos.length = 0;
            _this.redos.length = 0;
        },
        executeCommad: function (command) {
            try {
                _this.undos.push(command.execute(doc));
                _this.redos.length = 0;
            }
            catch (e) {
                console.error(e);
            }
        },
        undo: function () {
            if (_this.undos.length > 0) {
                _this.redos.push(_this.undos
                    .pop()
                    .undo(doc));
            }
        },
        redo: function () {
            if (_this.redos.length > 0) {
                _this.undos.push(_this.redos
                    .pop()
                    .execute(doc));
            }
        }
    };
    return _this;
}
exports.CommandManager = CommandManager;
;

},{}],12:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.addEventListeners = void 0;
var tslib_1 = require("tslib");
var core_1 = require("./core");
var core_pub_sub_1 = require("./core.pub-sub");
var EVENTS = ['[on-click]', '[on-publish]', '[route-link]', '[on-change]'];
function addEventListeners(container, handlers, context) {
    var fn = {
        innerHTML: function (e, value, mode) { return e.innerHTML = value; },
        innerText: function (e, value, mode) { return e.innerText = value; },
        className: function (e, value) { return e.className = value; },
        append: function (e, value, mode) { return e.innerHTML += value; },
    };
    EVENTS.forEach(function (selector, index) {
        container
            .querySelectorAll(selector)
            .toArray()
            .concat([container])
            .forEach(function (e) {
            var name = selector.replace('[', '').replace(']', '');
            if (!e.attributes.getNamedItem(name))
                return;
            var value = e.attributes.getNamedItem(name).value;
            var tokens = value.split(':');
            if (index === 0) {
                var fn_1 = handlers[tokens[0]] ||
                    core_1.core.getValue(tokens[0], context);
                e.onclick = function (event) {
                    var _args = tokens.slice(1)
                        .reduce(function (a, p) {
                        a.push(p.charAt(0) == '@'
                            ? core_1.core.getValue(p.slice(1), context)
                            : p);
                        return a;
                    }, [e, event]);
                    return fn_1.apply(context, _args);
                };
                return;
            }
            if (index === 1) {
                var topic = core_1.core.getValue(tokens[0], core_pub_sub_1.default);
                topic = topic === window ? tokens[0] : topic;
                core_pub_sub_1.default.subscribe(topic, function (message, data) {
                    var fnName = tokens[1];
                    if (fnName) {
                        var f = fn[fnName] ||
                            handlers[fnName] ||
                            core_1.core.getValue(fnName, context);
                        if (f)
                            f.apply(context, tslib_1.__spreadArrays([e, data], tokens.slice(2)));
                        return;
                    }
                    else {
                        fn.innerHTML(e, data, tokens[1]);
                    }
                });
            }
            if (index === 2) {
                e.onclick = function (e) {
                    return false;
                };
            }
            if (index === 3) {
                var select = e.tagName === 'SELECT';
                if (value === 'publish') {
                    if (select)
                        e.onchange = function () { return core_pub_sub_1.default.publish(core_pub_sub_1.default.TOPICS.VALUE_CHANGE, e); };
                    else
                        e.oninput = function () { return core_pub_sub_1.default.publish(core_pub_sub_1.default.TOPICS.VALUE_CHANGE, e); };
                    return;
                }
                var fn_2 = handlers[value] ||
                    core_1.core.getValue(value, context);
                if (select)
                    e.onchange = function () { return fn_2.apply(context, [e]); };
                else
                    e.oninput = function () { return fn_2.apply(context, [e]); };
                e.onblur = function () { return fn_2.apply(context, [e]); };
            }
        });
    });
}
exports.addEventListeners = addEventListeners;

},{"./core":16,"./core.pub-sub":18,"tslib":26}],13:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DialogHelper = void 0;
var DialogHelper = (function () {
    function DialogHelper() {
    }
    DialogHelper.getWrapper = function (id) {
        var __container = document.getElementById(id);
        var __dlg = { container: __container, title: __container.querySelector('.js-title'),
            body: __container.querySelector('.js-content'),
            closeButton: __container.querySelector('.js-close-button'),
            acceptButton: __container.querySelector('.js-accept-button'), close: function () {
                __container.style.display = 'none';
                return __dlg;
            }, show: function (onConfirm) {
                if (onConfirm) {
                    __dlg.acceptButton.onclick = function () {
                        onConfirm(__dlg);
                    };
                }
                __container.style.display = 'block';
                return __dlg;
            },
            init: function (onInit) {
                if (onInit)
                    onInit(__dlg);
                return __dlg;
            },
            setTitle: function (title) {
                __dlg.title.innerHTML = title;
                return __dlg;
            },
            setBody: function (content) {
                if (content.tagName) {
                    __dlg.body.innerHTML = '';
                    __dlg.body.appendChild(content);
                }
                else {
                    __dlg.body.innerHTML = content;
                }
                return __dlg;
            },
            disableClickOutside: function () {
                __dlg.container.onclick = function () { };
                return __dlg;
            }
        };
        __dlg.acceptButton.onclick = __dlg.close;
        __dlg.closeButton.onclick = __dlg.close;
        __dlg.container.onclick = function (sender) { if (sender.target === __dlg.container)
            __dlg.close(); };
        return __dlg;
    };
    return DialogHelper;
}());
exports.DialogHelper = DialogHelper;

},{}],14:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CoreEvent = void 0;
var counter = 0;
var CoreEvent = (function () {
    function CoreEvent(name) {
        this._name = name;
        this._subscribers = new Map();
    }
    CoreEvent.prototype.dispatch = function (eventArgs) {
        var _this = this;
        this._subscribers
            .forEach(function (callback) { return callback(_this._name, eventArgs); });
        return this;
    };
    CoreEvent.prototype.add = function (callback) {
        this._subscribers.set(++counter, callback);
        return counter;
    };
    CoreEvent.prototype.remove = function (id) {
        return this._subscribers.delete(id);
    };
    return CoreEvent;
}());
exports.CoreEvent = CoreEvent;

},{}],15:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var includes = [];
function include(url) {
    return new Promise(function (resolve) {
        function __resolve() {
            includes.push(url.toLowerCase());
            resolve();
        }
        if (includes.indexOf(url.toLowerCase()) > -1) {
            resolve();
            return;
        }
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.onload = function () {
            includes.push(url.toLowerCase());
            resolve(function () {
                document.getElementsByTagName("head")[0]
                    .removeChild(script);
                includes.remove(url.toLowerCase());
            });
        };
        script.src = url;
        document.getElementsByTagName("head")[0]
            .appendChild(script);
    });
}
exports.default = include;

},{}],16:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.core = void 0;
var tslib_1 = require("tslib");
var Core = (function () {
    function Core() {
    }
    Core.prototype.isNull = function (v) { return v === null; };
    Core.prototype.toArray = function (v) { return Array.from ? Array.from(v) : Array.prototype.slice.call(v); };
    Core.prototype.isArray = function (v) { return Array.isArray(v); };
    Core.prototype.isString = function (v) { return typeof v == 'string'; };
    Core.prototype.isBoolean = function (v) { return typeof v == 'boolean'; };
    Core.prototype.isNumber = function (v) { return typeof v == 'number'; };
    Core.prototype.isFunction = function (v) { return typeof v == 'function'; };
    Core.prototype.isObject = function (v) { return v && typeof v == 'object'; };
    Core.prototype.apply = function (a, b, d) {
        if (d)
            this.apply(a, d);
        if (a && b && this.isObject(b)) {
            for (var p in b) {
                if (this.isArray(b[p])) {
                    a[p] = this.clone(b[p]);
                }
                else if (this.isObject(b[p])) {
                    this.apply(a[p] = a[p] || {}, b[p]);
                }
                else {
                    a[p] = b[p];
                }
            }
        }
        return a;
    };
    ;
    Core.prototype.clone = function (o) {
        var _this = this;
        if (this.isArray(o))
            return o.slice(0);
        if (this.isObject(o) && o.clone)
            return o.clone();
        if (this.isObject(o)) {
            return Object.keys(o)
                .reduce(function (a, k) {
                a[k] = _this.clone(o[k]);
                return a;
            }, {});
        }
        return o;
    };
    Core.prototype.join = function (items, property, separator) {
        return items.reduce(function (a, o) { return a.append(o[property || 'id']); }, [])
            .join(separator === undefined ? '-' : (separator || ''));
    };
    Core.prototype.createStringBuilder = function (s) {
        return { value: s || '', append: function (s) { this.value = this.value + s; return this; },
            appendLine: function (s) { this.value = this.value + (s || '') + '\n'; return this; } };
    };
    Core.prototype.$ = function (e, control) {
        var __element = document.getElementById(e);
        if (__element)
            return __element;
        var __targets;
        if (control)
            __targets = control.querySelectorAll(e);
        else
            __targets = document.querySelectorAll(e);
        if (__targets.length)
            return __targets.toArray();
        return null;
    };
    ;
    Core.prototype.element = function (idOrSelector, targets) {
        return (document.getElementById(idOrSelector) ||
            this.elements(idOrSelector, targets)[0]);
    };
    ;
    Core.prototype.elements = function (selector, targets) {
        return (targets || document).querySelectorAll(selector)
            .toArray();
    };
    ;
    Core.prototype.build = function (tagName, options, firstElementChild) {
        var o = this.isString(options) ? { innerHTML: options } : options;
        var e = this.apply(document.createElement(tagName), o);
        return firstElementChild ? e.firstElementChild : e;
    };
    ;
    Core.prototype.parseQueryString = function () {
        return location.search
            .slice(1)
            .split('&').reduce(function (o, a) {
            o[a.split('=')[0]] = a.split('=')[1] || '';
            return o;
        }, {});
    };
    ;
    Core.prototype.config = function (name) {
        var __instance = {
            write: function (key, value) {
                localStorage.setItem('{0}.{1}'.format(name, key), value);
                return this;
            },
            read: function (key) {
                return localStorage.getItem('{0}.{1}'.format(name, key));
            }
        };
        return __instance;
    };
    Core.prototype.getValue = function (key, scope) {
        return key.split(/\.|\[|\]/)
            .reduce(function (a, b) {
            if (b === '')
                return a;
            if (b === 'this')
                return a;
            var name = b;
            var apply_proto = b.indexOf('|') > -1;
            var arg = [];
            if (apply_proto) {
                var tokens = String.trimValues(b.split('|'));
                name = tokens[0];
                arg = String.trimValues(tokens[1].split(','));
            }
            var value = a[name];
            if (value === undefined && a.outerScope) {
                value = exports.core.getValue(name, a.outerScope);
            }
            if (value != undefined) {
                return apply_proto ? value.__proto__[arg[0]]
                    .apply(value, arg.slice(1))
                    : value;
            }
            return a === self ? '' : self;
        }, scope || self);
    };
    return Core;
}());
exports.core = new Core();
String.leftPad = function (val, size, ch) {
    var result = '' + val;
    if (ch === null || ch === undefined || ch === '')
        ch = ' ';
    while (result.length < size)
        result = ch + result;
    return result;
};
String.trimValues = function (values) {
    return values.map(function (s) { return s.trim(); });
};
String.prototype.format = function () {
    var values = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        values[_i] = arguments[_i];
    }
    var __context = values[values.length - 1] || self;
    var __call_fn = function (fn, params, base) {
        var _args = String.trimValues(params)
            .reduce(function (a, p) {
            a.push(p.charAt(0) == '@' ? exports.core.getValue(p.slice(1), __context)
                : p);
            return a;
        }, base);
        return fn.apply(__context, _args);
    };
    return this.replace(/\{(\d+|[^{]+)\}/g, function (m, k) {
        var _a = String.trimValues(k.split(':')), key = _a[0], fnName = _a[1];
        var value;
        if (/^\d+/.test(key)) {
            var tokens = String.trimValues(key.split('|'));
            var index = ~~tokens[0];
            var name_1 = tokens.length == 0 ? 'data'
                : ['data'].concat(tokens.slice(1))
                    .join('|');
            var scope = { data: values[index], outerScope: __context };
            value = exports.core.getValue(name_1, scope);
        }
        else {
            value = exports.core.getValue(key, __context);
        }
        if (exports.core.isFunction(value)) {
            return __call_fn(value, fnName ? fnName.split(/\s|\;/)
                : [], []);
        }
        if (fnName) {
            var _b = String.trimValues(fnName.split(/=>/)), name_2 = _b[0], params = _b[1];
            return __call_fn(exports.core.getValue(name_2, __context), params ? params.split(/\s|\;/) : [], [value]);
        }
        return value;
    });
};
String.prototype.replaceAll = function (pattern, replacement) { return this.split(pattern).join(replacement); };
String.prototype.fixDate = function () { return this.split(' ')[0]; };
String.prototype.fixTime = function () { return this.split(' ')[1]; };
String.prototype.fixYear = function () { return this.fixDate().split('/')[2]; };
String.prototype.paddingLeft = function (v) { return (v + this).slice(-v.length); };
String.prototype.merge = function (context) {
    var __result = this.replace(/{([^{]+)?}/g, function (m, key) {
        if (key.indexOf(':') > 0) {
            var __tokens = key.split(':');
            var __fn = exports.core.getValue(__tokens[0], context);
            var __value = exports.core.getValue(__tokens[1], context);
            return __fn(__value, context);
        }
        var r = exports.core.getValue(key, context);
        return typeof (r) == 'function' ? r(context) : r;
    });
    return __result;
};
String.prototype.toXmlDocument = function () {
    return new DOMParser().parseFromString(this, "text/xml");
};
String.prototype.htmlDecode = function () {
    return new DOMParser().parseFromString(this, "text/html")
        .documentElement
        .textContent;
};
String.prototype.startsWith = String.prototype.startsWith || function (t) { return this.indexOf(t) == 0; };
Array.prototype.remove = function (o) {
    var index = this.indexOf(o);
    if (index != -1)
        this.splice(index, 1);
    return this;
};
Array.prototype.add = function (o) {
    this.push(o);
    return o;
};
Array.prototype.append = function (o) {
    this.push(o);
    return this;
};
Array.prototype.select = function (sentence) {
    return exports.core.isString(sentence) ? this.map(function (e) { return e[sentence]; })
        : this.map(sentence);
};
Array.prototype.item = function (propName, value, def) {
    return this.filter(function (v) {
        return v[propName] == value;
    })[0] || def;
};
Array.prototype.contains = function (propName, value) { return this.item(propName, value); };
Array.prototype.lastItem = function () { return this[this.length - 1]; };
Array.prototype.where = function (sentence) {
    if (exports.core.isFunction(sentence))
        return this.filter(sentence);
    if (exports.core.isObject(sentence)) {
        return this.filter(new Function('a', Object.keys(sentence)
            .reduce(function (a, propname, i) {
            return a + (i > 0 ? ' && ' : '')
                + (function () {
                    var __value = sentence[propname];
                    if (__value instanceof RegExp)
                        return '{1}.test(a.{0})'.format(propname, __value);
                    if (exports.core.isString(__value))
                        return 'a.{0} === \'{1}\''.format(propname, __value);
                    return 'a.{0} === {1}'.format(propname, __value);
                }());
        }, 'return ')));
    }
    return this;
};
Array.prototype.sortBy = function (propname, desc) {
    var __order = [];
    var __names = propname.split(',').map(function (token, i) {
        var __pair = token.split(' ');
        __order[i] = (__pair[1] && (__pair[1].toUpperCase() == 'DESC')) ? -1 : 1;
        return __pair[0];
    });
    __order[0] = (desc ? -1 : 1);
    this.sort(function (a, b) {
        var i = 0;
        var __fn = function (a, b) {
            var __x = a[__names[i]];
            var __y = b[__names[i]];
            if (__x < __y)
                return -1 * __order[i];
            if (__x > __y)
                return 1 * __order[i];
            i++;
            if (i < __names.length)
                return __fn(a, b);
            return 0;
        };
        return __fn(a, b);
    });
    return this;
};
Array.prototype.orderBy = function (sentence) {
    var __sentence = exports.core.isString(sentence) ? function (a) { return a[sentence]; }
        : sentence;
    return this.map(function (e) { return e; })
        .sort(function (a, b) {
        var x = __sentence(a);
        var y = __sentence(b);
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    });
};
Array.prototype.distinct = function (sentence) {
    if (sentence === void 0) { sentence = ''; }
    var __sentence = exports.core.isString(sentence) ? function (a) { return sentence ? a[sentence] : a; }
        : sentence;
    var r = [];
    this.forEach(function (item) {
        var _value = __sentence(item);
        if (r.indexOf(_value) == -1)
            r.push(_value);
    });
    return r;
};
Array.prototype.groupBy = function (prop) {
    var __buildKey = function (target) { return prop.split(',')
        .map(function (f) { return target[f]; })
        .join('__'); };
    return this.reduce(function (groups, item) {
        var key = __buildKey(item);
        (groups[key] = groups[key] || []).push(item);
        return groups;
    }, {});
};
Array.prototype.toGroupWrapper = function (ctx) {
    var dataSet = this;
    var __f = function (k, t, name) {
        ctx[name] = {};
        dataSet.distinct(function (v) { return v[k]; })
            .forEach(function (v) {
            ctx[name][v] = dataSet.reduce(function (p, c) {
                return (c[k] == v) ? p + c[t] : p;
            }, 0.0);
        });
        return __f;
    };
    return __f;
};
Array.prototype.sum = function (prop) {
    return this.reduce(function (a, item) { return a + item[prop]; }, 0.0);
};
Array.prototype.toDictionary = function (prop, value) {
    return this.reduce(function (a, d) {
        a[d[prop]] = value ? d[value] : d;
        return a;
    }, {});
};
Array.prototype.split = function (size) {
    return this.reduce(function (acc, current, i, self) {
        if (!(i % size))
            return tslib_1.__spreadArrays(acc, [self.slice(i, i + size)]);
        return acc;
    }, []);
};
Array.prototype.includes = Array.prototype.includes || function (searchElement, fromIndex) {
    return this.indexOf(searchElement) != -1;
};
NodeList.prototype.toArray = function () {
    return Array.from ? Array.from(this) : exports.core.toArray(this);
};
Object.entries = Object.entries || (function (o) { return Object.keys(o).map(function (k) { return [k, o[k]]; }); });

},{"tslib":26}],17:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Paginator = void 0;
var Paginator = (function () {
    function Paginator() {
    }
    Paginator.paginate = function (data, currentPage, pageSize, title) {
        if (currentPage === void 0) { currentPage = 1; }
        if (pageSize === void 0) { pageSize = 10; }
        var startPage, endPage;
        var totalPages = Math.ceil(data.length / pageSize);
        if (currentPage < 1) {
            currentPage = 1;
        }
        else if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.min(startIndex + pageSize - 1, data.length - 1);
        return { totalItems: data.length,
            currentPage: currentPage,
            pageSize: pageSize,
            totalPages: totalPages,
            startIndex: startIndex,
            endIndex: endIndex,
            allItems: data,
            visibleItems: data.slice(startIndex, endIndex + 1),
            title: title, getChecked: function () { return data.where({ '__checked': true })
                .map(function (item, i) {
                return { index: data.indexOf(item),
                    item: item };
            }); }
        };
    };
    return Paginator;
}());
exports.Paginator = Paginator;

},{}],18:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var tslib_1 = require("tslib");
var topics = new Map();
var subUid = -1;
var subscribe = function (topic, func) {
    if (!topics.has(topic)) {
        topics.set(topic, []);
    }
    var token = (++subUid).toString();
    topics.get(topic).push({
        token: token,
        func: func
    });
    return token;
};
var publish = function (topic) {
    var args = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        args[_i - 1] = arguments[_i];
    }
    if (!topics.has(topic)) {
        return false;
    }
    setTimeout(function () {
        var _a;
        var subscribers = topics.get(topic);
        var len = subscribers ? subscribers.length : 0;
        while (len--) {
            (_a = subscribers[len]).func.apply(_a, tslib_1.__spreadArrays([topic], args));
        }
    }, 0);
    return true;
};
var unsubscribe = function (token) {
    for (var m in topics.keys) {
        var subscribers = topics.get(m);
        if (subscribers) {
            for (var i = 0, j = subscribers.length; i < j; i++) {
                if (subscribers[i].token === token) {
                    subscribers.splice(i, 1);
                    return token;
                }
            }
        }
    }
    return false;
};
exports.default = {
    subscribe: subscribe,
    publish: publish,
    unsubscribe: unsubscribe,
    TOPICS: {
        VIEW_CHANGE: 'view:change',
        VALUE_CHANGE: 'value:change',
        MUNICIPIO_CHANGE: 'municipio:change',
        WINDOW_SCROLL: 'window.scroll',
        WINDOW_RESIZE: 'window.resize',
        NOTIFICATION: 'notification.show'
    }
};

},{"tslib":26}],19:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ReportEngine = void 0;
var core_1 = require("./core");
var core_templates_1 = require("./core.templates");
var ReportEngine = (function () {
    function ReportEngine() {
        this.BS = {};
        this.module_ReportEngine_Copy = function (source, dest) {
            for (var p in dest) {
                if (dest.hasOwnProperty(p)) {
                    if (source.hasOwnProperty(p)) {
                        dest[p] = source[p];
                        continue;
                    }
                    if (p === '_max_' || p === '_mim_') {
                        var __max = dest[p];
                        for (var m in __max) {
                            if (__max.hasOwnProperty(m) && source.hasOwnProperty(m))
                                __max[m] = source[m];
                        }
                    }
                    if (p === '_values_') {
                        var __agregate = dest[p];
                        for (var m in __agregate) {
                            if (__agregate.hasOwnProperty(m) && source.hasOwnProperty(m)) {
                                __agregate[m] = [source[m]];
                            }
                        }
                    }
                }
            }
        };
        this.module_ReportEngine_Sum = function (source, dest) {
            for (var p in dest) {
                if (dest.hasOwnProperty(p)) {
                    if (source.hasOwnProperty(p)) {
                        dest[p] += source[p];
                        continue;
                    }
                    if (p === '_max_' || p === '_min_') {
                        var __max = dest[p];
                        for (var m in __max) {
                            if (__max.hasOwnProperty(m) && source.hasOwnProperty(m)) {
                                if (p == '_max_')
                                    __max[m] = source[m] > __max[m] ? source[m] : __max[m];
                                else
                                    __max[m] = source[m] < __max[m] ? source[m] : __max[m];
                            }
                        }
                    }
                    if (p === '_values_') {
                        var __agregate = dest[p];
                        for (var m in __agregate) {
                            if (__agregate.hasOwnProperty(m) && source.hasOwnProperty(m))
                                __agregate[m].add(source[m]);
                        }
                    }
                }
            }
        };
    }
    ReportEngine.prototype.__cloneRowTemplate = function (e) {
        var __row = e.cloneNode(true);
        var __table = e.parentNode.parentNode;
        __table.deleteRow(e.rowIndex);
        return __row;
    };
    ReportEngine.prototype.__fillTemplate = function (e, scope) {
        var _elements = e.querySelectorAll('[xbind]')
            .toArray();
        if (e.attributes.getNamedItem('xbind'))
            _elements.push(e);
        _elements.forEach(function (child) {
            core_1.core.toArray(child.attributes)
                .where({ value: /{[^{]+?}/g })
                .map(function (a) { return a.value = core_templates_1.merge(a.value, scope); });
            core_1.core.toArray(child.childNodes)
                .where({ nodeType: 3 })
                .where({ textContent: /{[^{]+?}/g })
                .forEach(function (text) { return text.textContent = core_templates_1.merge(text.textContent, scope, text); });
            String.trimValues(child.attributes
                .getNamedItem('xbind')
                .value
                .split(';'))
                .forEach(function (token) {
                if (token === '')
                    return;
                var _tokens = String.trimValues(token.split(':'));
                var _params = String.trimValues(_tokens[1].split(/\s|\,/));
                var _value = core_1.core.getValue(_params[0], scope);
                if (core_1.core.isFunction(_value)) {
                    var _args = _params.slice(1)
                        .reduce(function (a, p) {
                        a.push(p.charAt(0) == '@' ? core_1.core.getValue(p.slice(1), scope) : p);
                        return a;
                    }, [scope, child]);
                    _value = _value.apply(scope, _args);
                }
                else if (_params[1]) {
                    var _func = core_1.core.getValue(_params[1], scope);
                    _value = _func(_value, _params[2], scope, child);
                }
                child[_tokens[0]] = _value;
            });
        });
        return e;
    };
    ReportEngine.prototype.__mergeTemplate = function (template, sb, context, onGroupFooter) {
        var _this = this;
        if (template.forEach)
            return template.forEach(function (t, i) { _this.__mergeTemplate(t, sb, context[i], onGroupFooter); });
        this.__fillTemplate(template, { BS: this.BS });
        if (context.tag || context.tag == 'nofooter')
            return;
        sb.append(template.outerHTML.replace(/xbind="[^"]*"/g, ''));
        if (onGroupFooter) {
            onGroupFooter({ "sb": sb, "section": context });
        }
    };
    ReportEngine.prototype.module_ReportEngine_processAll = function (o) {
        var _this = this;
        var __doc = document.createDocumentFragment();
        __doc.appendChild(core_1.core.build('div', { innerHTML: o.ReportTemplate }, false));
        o.DetailTemplate = this.__cloneRowTemplate(__doc.querySelector(o.DetailTemplateSelector));
        if (o.HideTotal) {
            var __row = __doc.querySelector(o.TotalTemplateSelector);
            __row.parentNode.removeChild(__row);
        }
        else {
            o.TotalTemplate = this.__cloneRowTemplate(__doc.querySelector(o.TotalTemplateSelector));
        }
        o.GroupsTemplates = [];
        o.GroupsTemplates = o.Grupos.map(function (g) { return _this.__cloneRowTemplate(__doc.querySelector(g.selector)); });
        var __that = this;
        var _g_id = -1;
        function __DoHeaders() {
            o.Grupos.forEach(function (grupo, ii) {
                if (ii < _g_id)
                    return;
                var g = o.Grupos[ii];
                if (g.header) {
                    var __header = core_1.core.getValue(g.header, __that)(g.current, g.name);
                    if (__header != 'hidden;') {
                        if (__header.text) {
                            _sb.append('<tr {0}>{1}</tr>'.format(__header.attributes, __header.text));
                        }
                        else if (__header.row) {
                            __that.BS.reportDefinition.dom_tbody.appendChild(__header.row);
                        }
                        else {
                            _sb.append('<tr class="group-header">{0}</tr>'.format(__header));
                        }
                    }
                    if (o.RepeatHeadersAfter == ii) {
                        o.RepeatHeaders.forEach(function (index) {
                            if (index != '')
                                _sb.append(o.Headers[index].html);
                        });
                    }
                }
            });
        }
        var _sb = core_1.core.createStringBuilder('');
        o.OnStart(o.DataSet);
        o.DataSet.forEach(function (r, i) {
            if (i == 0)
                __DoHeaders();
            o.OnRow(r);
            if (o.Grupos.every(function (g) { return g.test(r); })) {
                o.Grupos.forEach(function (g) {
                    g.sum(r);
                });
            }
            else {
                o.Grupos.some(function (g, i) {
                    if (!g.test(r)) {
                        _g_id = i;
                        var __templates = o.GroupsTemplates.map(function (t) { return t; });
                        __templates.splice(0, i);
                        __templates.reverse();
                        var __groups = o.Grupos.map(function (g) { return g; });
                        __groups.splice(0, i);
                        __groups.reverse();
                        _this.__mergeTemplate(__templates, _sb, __groups, o.OnGroupFooter);
                        o.Grupos.forEach(function (grupo, ii) {
                            if (ii >= i) {
                                grupo.init(r);
                                _g_id = i;
                            }
                            else {
                                grupo.sum(r);
                            }
                        });
                        return true;
                    }
                    return false;
                });
                o.OnRowEnd(r);
                __DoHeaders();
            }
            if (o.HideDetail)
                return;
            _this.__mergeTemplate(o.DetailTemplate, _sb, { name: 'detail' }, o.g);
        });
        if (o.DataSet.length > 0) {
            this.BS.previous = this.BS.data;
            var __templates = o.GroupsTemplates.map(function (t) { return t; });
            __templates.reverse();
            if (!o.HideTotal)
                __templates.push(o.TotalTemplate);
            var __groups = o.Grupos.map(function (g) { return g; });
            __groups.reverse();
            __groups.push({ name: 'summary' });
            this.__mergeTemplate(__templates, _sb, __groups, o.OnGroupFooter);
        }
        return __doc.querySelector(o.ReportTableSelector)
            .innerHTML
            .replace('<tbody>', '<tbody>' + _sb.value);
    };
    ReportEngine.prototype.fromReportDefinition = function (rd, data, callback) {
        var _this = this;
        var __that = this;
        this.BS = { reportDefinition: rd };
        if (rd.Iteratefn)
            data.forEach(rd.Iteratefn);
        if (rd.orderBy)
            data.sortBy(rd.orderBy, false);
        var __summary = JSON.parse(rd.summary || '{}');
        function __createGroups() {
            return rd.groups
                .where(function (g, i) { return i < rd.groups.length - 1; })
                .map(function (g, i) {
                return {
                    name: 'G' + (i + 1),
                    selector: '#' + g.id,
                    key: g.key,
                    tag: g.tag || '',
                    current: '',
                    header: g.header,
                    data: core_1.core.clone(__summary),
                    init: function (value) {
                        var __k = value[this.key].toString();
                        var __BS_Name = __that.BS[this.name];
                        __BS_Name.all[__k] = __BS_Name.all[__k] || [];
                        ;
                        __BS_Name.all[__k].push(value);
                        __BS_Name.recordCount = 1;
                        __that.module_ReportEngine_Copy(value, this.data);
                    },
                    sum: function (value) {
                        var __k = value[this.key].toString();
                        var __BS_Name = __that.BS[this.name];
                        __BS_Name.all[__k] = __BS_Name.all[__k] || [];
                        ;
                        __BS_Name.all[__k].push(value);
                        __BS_Name.recordCount += 1;
                        __that.module_ReportEngine_Sum(value, this.data);
                    },
                    test: function (value) { return value[this.key] == this.current; }
                };
            }) || [];
        }
        var __wrapper = {
            DataSet: data,
            HideDetail: rd.hideDetail == 'true' || false,
            HideTotal: rd.groups.length == 0 || rd.HideTotal == 'true' || false,
            ReportTemplate: rd.html,
            ReportTableSelector: '#' + rd.tableId,
            DetailTemplateSelector: '#' + rd.details[0].id,
            TotalTemplateSelector: rd.groups.length == 0 ? '' : '#' + rd.groups.lastItem().id,
            Grupos: __createGroups(),
            OnGroupFooter: rd.OnGroupFooter,
            Headers: rd.headers,
            RepeatHeaders: (rd.repeatHeader || '').split(','),
            RepeatHeadersAfter: rd.repeatHeaderAfter,
            OnRow: function (data) {
                __that.BS.recordCount += 1;
                __that.BS.previous = __that.BS.data || data;
                __that.BS.data = data;
                __wrapper.Grupos.forEach(function (g, i) { __that.BS[g.name].data = Object.create(g.data); });
                __that.module_ReportEngine_Sum(data, __that.BS.G0);
                if (rd.onRowfn)
                    (new Function('ctx', rd.onRowfn)(_this.BS));
            },
            OnStart: function (dataSet) {
                __that.BS = {
                    recordCount: 0,
                    G0: core_1.core.clone(__summary),
                    dataSet: dataSet,
                    reportDefinition: __that.BS.reportDefinition
                };
                __wrapper.Grupos.forEach(function (g, i) {
                    g.current = (dataSet && dataSet[0]) ? dataSet[0][g.key] : '';
                    __that.BS[g.name] = { recordCount: 0, all: {} };
                });
                if (rd.onStartfn)
                    rd.onStartfn(__that.BS);
            },
            OnRowEnd: function (data) {
                __wrapper.Grupos
                    .forEach(function (g) { g.current = data[g.key]; });
                if (rd.onRowEndfn)
                    (new Function('ctx', rd.onRowEndfn)(__that.BS));
            },
            PrintReport: function (callback) {
                if (callback)
                    callback(_this.module_ReportEngine_processAll(__wrapper));
                return _this;
            }
        };
        return __wrapper.PrintReport(callback);
    };
    return ReportEngine;
}());
exports.ReportEngine = ReportEngine;

},{"./core":16,"./core.templates":23}],20:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.loader = void 0;
function loadReport(code) {
    var __context = {
        headers: [],
        groups: [],
        details: []
    };
    var __cur = [{ columns: [] }];
    var __func = '';
    var __funcBody = '';
    function __getValue(value) {
        if (value && value.trim().startsWith('@')) {
            return __context[value.trim().split('@')[1].trim()] || '';
        }
        else if (value) {
            return value.trim();
        }
        return '';
    }
    function __parse_properties(value) {
        var __reg = /([a-zA-Z0-9_\-]*)\s*:\s*('[^']*'|[^\s]*)/g;
        var __o = {};
        var __match = __reg.exec(value);
        while (__match != null) {
            __o[__match[1].trim()] = __getValue(__match[2].trim().replace(/^'([^']*)'$/g, '$1'));
            __match = __reg.exec(value);
        }
        return __o;
    }
    function __parse_cell(value) {
        return __parse_properties(value);
    }
    function __parse_row(value) {
        var __properties = __parse_properties(value);
        __properties.columns = [];
        return __properties;
    }
    function __getAttributes(data) {
        var __attributes = [];
        Object.keys(data)
            .filter(function (key) { return key != 'columns' && key != 'html' && data.hasOwnProperty(key); })
            .forEach(function (key) {
            var __k = key == 'className' ? 'class' : key;
            __attributes.push('{0}="{1}"'.format(__k, __getValue(data[key])));
        });
        return __attributes.length > 0 ? ' ' + __attributes.join(' ') : '';
    }
    function __render() {
        function fill(data, hide, header) {
            var sb = '';
            var cellTag = header ? 'th' : 'td';
            (data || []).forEach(function (row, i) {
                var sb_row = '';
                sb_row += '\n      <tr{0}>'.format(__getAttributes(row));
                row.columns.forEach(function (col, i) {
                    sb_row += '\n        <{2}{0}>{1}</{2}>'.format(__getAttributes(col), __getValue(col.html), cellTag);
                });
                sb_row += '\n      </tr>';
                row.html = sb_row;
                if (hide && hide.indexOf(i.toString()) > -1)
                    return;
                sb += sb_row;
            });
            return sb;
        }
        return ('<div id="{3}">\n' +
            '  <table class= "w3-table-all" style = "width:100%;" id="table-{3}" >\n ' +
            '    <thead>{0}' +
            '    </thead>\n' +
            '    <tbody>{1}{2}' +
            '    </tbody>\n' +
            '  </table>\n' +
            '</div>').format(fill(__context.headers, (__context.hiddenHeaders || '').split(','), true), fill(__context.details), fill(__context.groups), __context.tableId || '');
    }
    function __parseLine(l, o) {
        if (!__func && !l.trim())
            return function () { };
        var __keys = /^(DEFINE|#|ADD_COL|CREATE|FUNCTION|END)/;
        if (__keys.test(l)) {
            if (/^#/.test(l)) {
                return function () { };
            }
            else if (/^FUNCTION (\w.*)/.test(l)) {
                var __tokens = l.match(/^FUNCTION (\w.*)$/);
                __func = __tokens[1].trim();
                __funcBody = '';
                return function () { };
            }
            else if (/^END/.test(l)) {
                var __body = __funcBody;
                var __name = __func;
                __func = __funcBody = '';
                return function () {
                    return function (ctx) { ctx[__name] = new Function('ctx', __body); };
                }();
            }
            else if (/^ADD_COL /.test(l)) {
                var __tokens = l.match(/ADD_COL (.*)$/);
                return function () {
                    var tokens = __tokens;
                    return function (ctx) { __cur[__cur.length - 1].columns.push(__parse_cell(tokens[1])); };
                }();
            }
            else if (/^DEFINE\s\s*(\w.*)\s*=\s*(.*)$/.test(l)) {
                var __tokens = l.match(/^DEFINE\s\s*([a-zA-Z0-9_\-]*)\s*=\s*(.*)$/);
                return function () {
                    var tokens = __tokens;
                    return function (ctx) { ctx[tokens[1].trim()] = tokens[2].trim(); };
                }();
            }
            else if (/^CREATE\s\s*(\w*) (.*)$/.test(l)) {
                var __tokens = l.match(/^CREATE\s\s*(\w*) (.*)$/);
                if (__tokens[1] == 'header') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) { ctx.headers.push(__parse_row(tokens[2])); __cur = ctx.headers; };
                    }();
                }
                else if (__tokens[1] == 'group') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) { ctx.groups.push(__parse_row(tokens[2])); __cur = ctx.groups; };
                    }();
                }
                else if (__tokens[1] == 'detail') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) { ctx.details.push(__parse_row(tokens[2])); __cur = ctx.details; };
                    }();
                }
                else {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) { ctx[tokens[1]] = tokens[2]; };
                    }();
                }
            }
            else {
                throw new Error('Tabbly : Unrecognized text after DEFINE');
            }
        }
        else {
            if (__func) {
                __funcBody += o;
                __funcBody += '\n';
                return function () { };
            }
            throw new Error('Tabbly : Unrecognized text');
        }
    }
    code.split('\n').forEach(function (l) {
        __parseLine(l.trim(), l)(__context);
    });
    __context.html = __render();
    return __context;
}
var loader = { load: loadReport };
exports.loader = loader;

},{}],21:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ReportEngine = void 0;
var core_1 = require("./core");
var ReportEngine = (function () {
    function ReportEngine() {
        this.BS = {};
    }
    ReportEngine.prototype.generateReport = function (rd, data, mediator) {
        var __that = this;
        if (mediator.clear)
            mediator.clear();
        mediator.message({ type: 'report.begin' });
        var __rd = rd;
        var __dataSet = __rd.parseData ? __rd.parseData(__rd, data, mediator.message)
            : data;
        mediator.message({ type: 'report.log.message', text: 'Inicializando...' });
        function __initContentProviders() {
            [__rd.sections, __rd.details, __rd.groups]
                .reduce(function (a, b) { return a.concat(b); }, [])
                .map(function (s) {
                if (s.valueProviderfn) {
                    s.valueProvider = core_1.core.getValue(s.valueProviderfn, { BS: __that.BS });
                    delete s.valueProviderfn;
                }
                if (s.footerValueProviderfn) {
                    s.footerValueProvider = core_1.core.getValue(s.footerValueProviderfn, { BS: __that.BS });
                    delete s.footerValueProviderfn;
                }
                if (s.headerValueProviderfn) {
                    s.headerValueProvider = core_1.core.getValue(s.headerValueProviderfn, { BS: __that.BS });
                    delete s.headerValueProviderfn;
                }
            });
        }
        var __MERGE_AND_SEND = function (t, p) {
            p.BS = __that.BS;
            mediator.send(t.format(p));
        };
        function __groupsHeaders() {
            __groups.forEach(function (g, ii) {
                if (ii < __breakIndex)
                    return;
                mediator.message({ type: 'report.sections.group.header', value: g.id });
                if (g.definition.header)
                    return __MERGE_AND_SEND(g.definition.header, g);
                if (g.definition.headerValueProvider)
                    return mediator.send(g.definition.headerValueProvider(g));
            });
        }
        function __groupsFooters(index) {
            var __gg = __groups.map(function (g) { return g; });
            if (index)
                __gg.splice(0, index);
            __gg.reverse().forEach(function (g) {
                mediator.message({ type: 'report.sections.group.footer', value: g.id });
                if (g.definition.footer)
                    return __MERGE_AND_SEND(g.definition.footer, g);
                if (g.definition.footerValueProvider)
                    return mediator.send(g.definition.footerValueProvider(g));
            });
        }
        function __detailsSections() {
            __details.forEach(function (d) {
                mediator.message({ type: 'report.sections.detail', value: d.id });
                if (d.template)
                    return __MERGE_AND_SEND(d.template, d);
                if (d.valueProvider)
                    return mediator.send(d.valueProvider(d));
            });
        }
        function __grandTotalSections() {
            __totals.forEach(function (t) {
                mediator.message({ type: 'report.sections.total', value: t.id });
                if (t.template)
                    return __MERGE_AND_SEND(t.template, t);
                if (t.valueProvider)
                    return mediator.send(t.valueProvider(t));
            });
        }
        function __reportHeaderSections() {
            __headers.forEach(function (t) {
                mediator.message({ type: 'report.sections.header', value: t });
                if (t.template)
                    return __MERGE_AND_SEND(t.template, t);
                if (t.valueProvider)
                    return mediator.send(t.valueProvider(t));
            });
        }
        function __resolveSummaryObject() {
            var __summary = JSON.parse(__rd.summary || '{}');
            if (__rd.onInitSummaryObject)
                return __rd.onInitSummaryObject(__summary);
            return __summary;
        }
        var __breakIndex = -1;
        var __summary = __resolveSummaryObject();
        var __headers = (__rd.sections || []).where({ type: 'header' });
        var __totals = (__rd.sections || []).where({ type: 'total' });
        var __footers = (__rd.sections || []).where({ type: 'footer' });
        var __details = __rd.details || [];
        var __groups = __rd.groups
            .map(function (g, i) {
            return { name: 'G' + (i + 1),
                id: g.id,
                rd: __rd,
                definition: g,
                current: '',
                data: core_1.core.clone(__summary), init: function (value) {
                    var __k = value[this.definition.key].toString();
                    var __Gx = __that.BS[this.name];
                    __Gx.all[__k] = __Gx.all[__k] || [];
                    __Gx.all[__k].push(value);
                    __Gx.recordCount = 1;
                    if (this.__resume === false)
                        return;
                    if (this.__resume) {
                        __that.copy(value, this.data);
                        return;
                    }
                    if (this.__resume = Object.keys(this.data).length > 0)
                        __that.copy(value, this.data);
                },
                sum: function (value) {
                    var __k = value[this.definition.key].toString();
                    var __Gx = __that.BS[this.name];
                    __Gx.all[__k] = __Gx.all[__k] || [];
                    __Gx.all[__k].push(value);
                    __Gx.recordCount += 1;
                    if (this.__resume === false)
                        return;
                    __that.sum(value, this.data);
                },
                test: function (value) {
                    return value[this.definition.key] == this.current;
                } };
        }) || [];
        __that.BS = { reportDefinition: __rd, mediator: mediator };
        if (__rd.iteratefn) {
            mediator.message({ type: 'report.log.message', text: 'Inicializando elementos...' });
            __dataSet.forEach(__rd.iteratefn);
        }
        if (__rd.orderBy) {
            mediator.message({ type: 'report.log.message', text: 'Ordenando datos...' });
            __dataSet.sortBy(__rd.orderBy, false);
        }
        __that.BS = { recordCount: 0,
            G0: core_1.core.clone(__summary),
            dataSet: __dataSet,
            reportDefinition: __rd,
            mediator: mediator };
        __groups.forEach(function (g, i) {
            g.current = (__dataSet && __dataSet[0]) ? __dataSet[0][g.definition.key] : '';
            __that.BS[g.name] = { recordCount: 0, all: {} };
        });
        if (__rd.onStartfn)
            __rd.onStartfn(__that.BS);
        __initContentProviders();
        mediator.message({ type: 'report.render.rows' });
        mediator.message({ type: 'report.log.message', text: 'Generando informe...' });
        __reportHeaderSections();
        if (__dataSet.length > 0)
            __groupsHeaders();
        __dataSet.forEach(function (r, i) {
            __that.BS.recordCount++;
            __that.BS.isLastRow = __dataSet.length === __that.BS.recordCount;
            __that.BS.isLastRowInGroup = __that.BS.isLastRow;
            __that.BS.percent = (__that.BS.recordCount / __dataSet.length) * 100;
            __that.BS.previous = __that.BS.data || r;
            __that.BS.data = r;
            __groups.forEach(function (g, i) {
                __that.BS[g.name].data = Object.create(g.data);
            });
            __that.sum(r, __that.BS.G0);
            if (__rd.onRowfn)
                __rd.onRowfn(__that.BS);
            mediator.message({ type: 'report.render.row',
                text: __that.BS.percent.toFixed(1) + ' %',
                value: __that.BS.percent });
            if (__groups.every(function (g) { return g.test(r); })) {
                __groups.forEach(function (g) { g.sum(r); });
            }
            else {
                __groups.some(function (g, i) {
                    if (!g.test(r)) {
                        __breakIndex = i;
                        __groupsFooters(__breakIndex);
                        __groups.forEach(function (grupo, ii) {
                            if (ii >= __breakIndex) {
                                grupo.init(r);
                                __breakIndex = i;
                            }
                            else {
                                grupo.sum(r);
                            }
                        });
                        return true;
                    }
                    return false;
                });
                __groups.forEach(function (g) {
                    g.current = r[g.definition.key];
                });
                if (__rd.onGroupChangefn)
                    __rd.onGroupChangefn(__that.BS);
                mediator.message({ type: 'report.sections.group.change',
                    value: __groups });
                __groupsHeaders();
            }
            if (__groups.length && !__that.BS.isLastRow) {
                var __next = __dataSet[__that.BS.recordCount];
                __that.BS.isLastRowInGroup = !__groups.every(function (g) {
                    var __k = g.definition.key;
                    return __next[__k] === __that.BS.data[__k];
                });
            }
            __detailsSections();
        });
        if (__dataSet.length > 0) {
            __that.BS.previous = __that.BS.data;
            __groupsFooters();
        }
        __grandTotalSections();
        mediator.message({ type: 'report.render.end' });
        mediator.message({ type: 'report.end' });
        if (mediator.flush)
            mediator.flush();
    };
    ReportEngine.prototype.merge = function (template, o) {
        return template.replace(/{([^{]+)?}/g, function (m, key) {
            if (key.indexOf(':') > 0) {
                var __fn = key.split(':');
                __fn[0] = core_1.core.getValue(__fn[0], o);
                __fn[1] = core_1.core.getValue(__fn[1], o);
                return __fn[0](__fn[1], o);
            }
            var r = core_1.core.getValue(key, o);
            return typeof (r) == 'function' ? r(o) : r;
        });
    };
    ReportEngine.prototype.copy = function (s, d) {
        Object.keys(d)
            .map(function (k) { d[k] = s[k]; });
    };
    ReportEngine.prototype.sum = function (s, d) {
        Object.keys(d)
            .map(function (k) { d[k] += s[k]; });
    };
    ReportEngine.prototype.compute = function (ds, name) {
        return ds.reduce(function (t, o) { return t + o[name]; }, 0.0);
    };
    ReportEngine.prototype.group = function (a, c) {
        var ds = a;
        var __f = function (k, t) {
            ds.distinct(function (v) { return v[k]; })
                .forEach(function (v) { c[v] = ds.reduce(function (p, c, i, a) { return (c[k] == v) ? p + c[t] : p; }, 0.0); });
            return __f;
        };
        return __f;
    };
    return ReportEngine;
}());
exports.ReportEngine = ReportEngine;
function onMessage(message) {
    var _this = this;
    if (message.type === 'report.content') {
        this._container.appendChild(this.build('div', message.content)
            .firstChild);
        return;
    }
    if (message.type === 'report.log.message') {
        this._progressBarMessage.innerHTML = message.text || '';
        return;
    }
    if (message.type === 'report.begin') {
        this._container.innerHTML = '';
        this._progressBarContainer.style.display = 'block';
        this._progressBarMessage.innerHTML = '';
        this._progressBar.style.width = '0%';
        return;
    }
    if (message.type === 'report.render.rows') {
        this._progressBar.style.width = '0%';
    }
    if (message.type === 'report.render.row') {
        this._progressBar.style.width = '{0}%'.format(message.value.toFixed(1));
        this._progressBar.innerHTML = message.text || '';
    }
    if (message.type === 'report.end') {
        setTimeout(function () {
            _this._progressBar.style.width = '100%';
            _this._progressBarMessage.innerHTML = '';
            _this._progressBarContainer.style.display = 'none';
        }, 250);
        return;
    }
}

},{"./core":16}],22:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.loader = void 0;
function loadReport(code) {
    var __context = {
        sections: [],
        groups: [],
        details: []
    };
    var __cur = {};
    var __func = '';
    var __funcBody = '';
    var __setState = false;
    function __get(value) {
        if (value && value.trim().startsWith('@')) {
            return __context[value.trim().split('@')[1].trim()] || '';
        }
        else if (value) {
            return value.trim();
        }
        return '';
    }
    function __parse_properties(value) {
        var __reg = /([a-zA-Z0-9_\-]*)\s*:\s*('[^']*'|[^\s]*)/g;
        var __o = {};
        var __match = __reg.exec(value);
        while (__match != null) {
            __o[__match[1].trim()] = __get(__match[2].trim().replace(/^'([^']*)'$/g, '$1'));
            __match = __reg.exec(value);
        }
        return __o;
    }
    function __parseLine(l, o) {
        if (!__func && !l.trim())
            return function () { };
        var __keys = /DEFINE|#|CREATE|SET|FUNCTION|END/;
        if (__keys.test(l)) {
            if (/^#/.test(l)) {
                return function () { };
            }
            else if (/^SET (\w.*)/.test(l)) {
                var __tokens = l.match(/^SET (\w.*)$/);
                __setState = true;
                __func = __tokens[1].trim();
                __funcBody = '';
                return function () { };
            }
            else if (/^FUNCTION (\w.*)/.test(l)) {
                var __tokens = l.match(/^FUNCTION (\w.*)$/);
                __setState = false;
                __func = __tokens[1].trim();
                __funcBody = '';
                return function () { };
            }
            else if (/^END/.test(l)) {
                var __body = __funcBody;
                var __name = __func;
                __func = __funcBody = '';
                if (__setState) {
                    __setState = false;
                    return function () {
                        return function (ctx) { __cur[__name] = __body.trim(); };
                    }();
                }
                else {
                    return function () {
                        return function (ctx) { ctx[__name] = new Function('ctx', __body); };
                    }();
                }
            }
            else if (/^DEFINE\s\s*(\w.*)\s*=\s*(.*)$/.test(l)) {
                var __tokens = l.match(/^DEFINE\s\s*([a-zA-Z0-9_\-]*)\s*=\s*(.*)$/);
                return function () {
                    var tokens = __tokens;
                    return function (ctx) { ctx[tokens[1].trim()] = tokens[2].trim(); };
                }();
            }
            else if (/^CREATE\s\s*(\w*) (.*)$/.test(l)) {
                var __tokens = l.match(/^CREATE\s\s*(\w*) (.*)$/);
                if (__tokens[1] == 'section') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) {
                            ctx.sections.push(__parse_properties(tokens[2]));
                            __cur = ctx.sections[ctx.sections.length - 1];
                        };
                    }();
                }
                else if (__tokens[1] == 'group') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) {
                            ctx.groups.push(__parse_properties(tokens[2]));
                            __cur = ctx.groups[ctx.groups.length - 1];
                        };
                    }();
                }
                else if (__tokens[1] == 'detail') {
                    return function () {
                        var tokens = __tokens;
                        return function (ctx) {
                            ctx.details.push(__parse_properties(tokens[2]));
                            __cur = ctx.details[ctx.details.length - 1];
                        };
                    }();
                }
            }
            else {
                throw new Error('Tabbly : Unrecognized text after DEFINE');
            }
        }
        else {
            if (__func) {
                __funcBody += o;
                __funcBody += '\n';
                return function () { };
            }
            throw new Error('Tabbly : Unrecognized text');
        }
    }
    code.split('\n').forEach(function (l) {
        __parseLine(l.trim(), l)(__context);
    });
    return __context;
}
var loader = { load: loadReport };
exports.loader = loader;

},{}],23:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fillTemplate = exports.executeTemplate = exports.merge = void 0;
var tslib_1 = require("tslib");
var core_1 = require("./core");
function __getValue(key, scope, def) {
    var v = core_1.core.getValue(key, scope);
    return v == window ? def : v;
}
function merge(template, o, HTMLElemnt) {
    var __call_fn = function (fn, params, base) {
        var _args = String.trimValues(params)
            .reduce(function (a, p) {
            a.push(p.charAt(0) == '@' ? core_1.core.getValue(p.slice(1), o)
                : p);
            return a;
        }, base);
        if (HTMLElemnt)
            _args.push(HTMLElemnt);
        return fn.apply(o, _args);
    };
    var __result = template.replace(/{([^{]+)?}/g, function (m, key) {
        if (key.indexOf(':') > 0) {
            var tokens = String.trimValues(key.split(':'));
            var value_1 = core_1.core.getValue(tokens[0], o);
            var _a = String.trimValues(tokens[1].split(/=>/)), name_1 = _a[0], params_1 = _a[1];
            var _params = params_1 ? String.trimValues(params_1.split(/\s|\;/))
                : [];
            return __call_fn(core_1.core.getValue(name_1, o), _params, [value_1]);
        }
        var _b = String.trimValues(key.split(/=>/)), name = _b[0], params = _b[1];
        var value = core_1.core.getValue(name, o);
        if (core_1.core.isFunction(value))
            return __call_fn(value, params.split(/\s|\;/), []);
        else
            return value;
    });
    return __result;
}
exports.merge = merge;
function fillTemplate(e, scope) {
    var _root = e;
    var _repeaters = _root.querySelectorAll('[xfor]')
        .toArray();
    var _repeatersElements = _repeaters.reduce(function (a, r) {
        return a.concat(core_1.core.$('[xbind]', r));
    }, tslib_1.__spreadArrays(_repeaters));
    var _elements = _root.querySelectorAll('[xbind]')
        .toArray()
        .filter(function (x) { return !_repeatersElements.includes(x); });
    if (_root.attributes.getNamedItem('xbind'))
        _elements.push(_root);
    _elements.forEach(function (child) {
        if (child.attributes.getNamedItem('xif')) {
            var fn = new Function('ctx', 'return {0};'.format(child.attributes
                .getNamedItem('xif')
                .value)
                .replaceAll('@', 'this.'));
            child.style.display = fn.apply(scope) ? '' : 'none';
        }
        core_1.core.toArray(child.attributes)
            .where({ value: /{[^{]+?}/g })
            .map(function (a) { return a.value = merge(a.value, scope); });
        core_1.core.toArray(child.childNodes)
            .where({ nodeType: 3 })
            .where({ textContent: /{[^{]+?}/g })
            .forEach(function (text) { return text.textContent = merge(text.textContent, scope, text); });
        String.trimValues(child.attributes
            .getNamedItem('xbind')
            .value
            .split(';'))
            .forEach(function (token) {
            if (token === '')
                return;
            var _a = String.trimValues(token.split(':')), name = _a[0], params = _a[1];
            var _b = String.trimValues(params.split(/=>/)), prop_name = _b[0], _params = _b[1];
            var _value = core_1.core.getValue(prop_name, scope);
            if (core_1.core.isFunction(_value)) {
                var _args = String.trimValues((_params || '').split(/\s|#/))
                    .reduce(function (a, p) {
                    a.push(p.charAt(0) == '@' ? core_1.core.getValue(p.slice(1), scope)
                        : p);
                    return a;
                }, []);
                _args.push(child);
                _value = _value.apply(scope, _args);
            }
            if (name)
                child[name] = _value;
        });
    });
    _repeaters.map(function (repeater) {
        var _a = String.trimValues(repeater.attributes
            .getNamedItem('xfor')
            .value
            .split(' in ')), itemName = _a[0], propname = _a[1];
        var data = core_1.core.getValue(propname, scope);
        if (data && data != window) {
            data.map(function (d, i) {
                var __scope = { index: i,
                    outerScope: scope };
                __scope[itemName] = core_1.core.clone(d);
                var node = fillTemplate(repeater.cloneNode(true), __scope);
                repeater.parentNode.insertBefore(node, repeater);
            });
        }
        return repeater;
    }).forEach(function (repeater) { return repeater.parentNode.removeChild(repeater); });
    return e;
}
exports.fillTemplate = fillTemplate;
function executeTemplate(e, values, dom) {
    var _template = core_1.core.isString(e) ? core_1.core.$(e) : e;
    var _result = values.reduce(function (a, v, i) {
        var _node = { index: i,
            data: v, node: fillTemplate(_template.cloneNode(true), v) };
        a.nodes.push(_node);
        if (!dom)
            a.html.push(_node.node.outerHTML.replace(/xbind="[^"]*"/g, ''));
        return a;
    }, { nodes: [], html: [] });
    return dom ? _result.nodes : _result.html.join('');
}
exports.executeTemplate = executeTemplate;

},{"./core":16,"tslib":26}],24:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TreeUtils = void 0;
var tslib_1 = require("tslib");
var core_templates_1 = require("./core.templates");
var core_1 = require("./core");
var TreeUtils = (function () {
    function TreeUtils() {
    }
    TreeUtils.createTree = function (data, propertyNames) {
        var groups = data.groupBy(propertyNames.join(','));
        return Object.entries(groups)
            .reduce(function (node, group) {
            group[0].split('__')
                .reduce(function (node, level, i, self) {
                return node[level] = node[level] ||
                    ((i == self.length - 1) ? { rows: tslib_1.__spreadArrays(group[1]) }
                        : {});
            }, node);
            return node;
        }, {});
    };
    TreeUtils.treeToHtml = function (tree, nodeTemplate, leafTemplate) {
        var deep = 0;
        var visitNode = function (node, nodeName, parent) {
            var __node = { name: nodeName,
                parent: parent,
                deep: deep++,
                rows: node.rows,
                innerHTML: '', children: Object.keys(node)
                    .where(function (property) { return property != 'rows'; })
                    .sort()
                    .map(function (g) { return ({ name: g, value: node[g] }); }) };
            if (node.rows) {
                deep--;
                return core_templates_1.executeTemplate(leafTemplate, [__node]);
            }
            else {
                __node.innerHTML = __node.children
                    .reduce(function (html, child) {
                    return html += visitNode(child.value, child.name, __node);
                }, '');
                deep--;
                return nodeTemplate.format(__node);
            }
        };
        return visitNode(tree, 'root');
    };
    TreeUtils.treeToText = function (tree) {
        var deep = 0;
        var visitNode = function (node, nodeName, parent) {
            var __node = { name: nodeName,
                parent: parent,
                deep: deep++,
                rows: node.rows,
                innerText: '', children: Object.keys(node)
                    .where(function (property) { return property != 'rows'; })
                    .sort()
                    .map(function (g) { return ({ name: g, value: node[g] }); }) };
            if (node.rows) {
                deep--;
                return '{0} {1}\n{0}  {2}\n'.format(' '.repeat(__node.deep * 2), __node.name, core_1.core.join(node.rows, 'ID', '\n' + ' '.repeat(2 + __node.deep * 2)));
            }
            else {
                __node.innerText = __node.children
                    .reduce(function (html, child) {
                    return html += visitNode(child.value, child.name, __node);
                }, '');
                deep--;
                return '{0} {name}\n{innerText}'.format(' '.repeat(__node.deep * 2), __node);
            }
        };
        return visitNode(tree, 'root');
    };
    return TreeUtils;
}());
exports.TreeUtils = TreeUtils;

},{"./core":16,"./core.templates":23,"tslib":26}],25:[function(require,module,exports){
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.NextPowerOfTwo = exports.IsPowerOfTwo = exports.polarToCartesian = exports.Degrees = exports.Radians = exports.Clamp = exports.Random = exports.Rectangle = exports.Box = exports.Vector2 = void 0;
var Vector2 = (function () {
    function Vector2(x, y) {
        this.x = x;
        this.y = y;
    }
    Vector2.fromArrayi = function (values) { return new Vector2(~~values[0], ~~values[1]); };
    ;
    Vector2.sum = function (a, b) { return new Vector2(a.x + b.x, a.y + b.y); };
    ;
    Vector2.difference = function (a, b) { return new Vector2(a.x - b.x, a.y - b.y); };
    ;
    Vector2.dot = function (a, b) { return a.x * b.x + a.y * b.y; };
    ;
    Vector2.cross = function (a, b) { return a.x * b.y - a.y * b.x; };
    ;
    Vector2.distance = function (a, b) { var dx = a.x - b.x; var dy = a.y - b.y; return Math.sqrt(dx * dx + dy * dy); };
    ;
    Vector2.distanceSquared = function (a, b) { var dx = a.x - b.x; var dy = a.y - b.y; return dx * dx + dy * dy; };
    ;
    Vector2.equals = function (a, b) { return a.x == b.x && a.y == b.y; };
    ;
    Vector2.lerp = function (a, b, f, resultVec) {
        var x = a.x, y = a.y;
        resultVec.x = (b.x - x) * f + x;
        resultVec.y = (b.y - y) * f + y;
        return resultVec;
    };
    Vector2.prototype.set = function (x, y) { this.x = x; this.y = y; return this; };
    ;
    Vector2.prototype.clone = function () { return new Vector2(this.x, this.y); };
    ;
    Vector2.prototype.length = function () { return Math.sqrt(this.x * this.x + this.y * this.y); };
    ;
    Vector2.prototype.lengthSquared = function () { return this.x * this.x + this.y * this.y; };
    ;
    Vector2.prototype.invert = function () { this.x = -this.x; this.y = -this.y; return this; };
    ;
    Vector2.prototype.cross = function (vector) { return this.x * vector.y - this.y * vector.x; };
    Vector2.prototype.dot = function (vector) { return this.x * vector.x + this.y * vector.y; };
    ;
    Vector2.prototype.scale = function (sx, sy) { this.x *= sx; this.y *= sy; return this; };
    ;
    Vector2.prototype.normalize = function () { var _d = 1 / this.length(); return this.scale(_d, _d); };
    ;
    Vector2.prototype.normalisedCopy = function () { return new Vector2(this.x, this.y).normalize(); };
    ;
    Vector2.prototype.add = function (vector) { this.x += vector.x; this.y += vector.y; return this; };
    ;
    Vector2.prototype.subtract = function (vector) { this.x -= vector.x; this.y -= vector.y; return this; };
    ;
    Vector2.prototype.mul = function (scalar) { this.x *= scalar; this.y *= scalar; return this; };
    ;
    Vector2.prototype.divide = function (scalar) { this.x /= scalar; this.y /= scalar; return this; };
    ;
    Vector2.prototype.equals = function (vector) { return this == vector || !!vector && this.x == vector.x && this.y == vector.y; };
    ;
    Vector2.prototype.rotate = function (angle) {
        var cos = Math.cos(angle);
        var sin = Math.sin(angle);
        var newX = this.x * cos - this.y * sin;
        var newY = this.y * cos + this.x * sin;
        this.x = newX;
        this.y = newY;
        return this;
    };
    ;
    Vector2.Vector2_ZERO = new Vector2(0.0, 0.0);
    Vector2.Vector2_UNIT_X = new Vector2(1.0, 0.0);
    Vector2.Vector2_UNIT_Y = new Vector2(0.0, 1.0);
    Vector2.Vector2_NEGATIVE_UNIT_X = new Vector2(-1.0, 0.0);
    Vector2.Vector2_NEGATIVE_UNIT_Y = new Vector2(0.0, -1.0);
    Vector2.Vector2_UNIT_SCALE = new Vector2(1.0, 1.0);
    Vector2.rotateAroundPoint = function (v, axisPoint, angle) {
        return v.clone()
            .subtract(axisPoint)
            .rotate(angle)
            .add(axisPoint);
    };
    return Vector2;
}());
exports.Vector2 = Vector2;
var Box = (function () {
    function Box(top, right, bottom, left) {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
    Box.prototype.clone = function () { return new Box(this.top, this.right, this.bottom, this.left); };
    ;
    Box.prototype.toRect = function () { return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top); };
    ;
    Box.prototype.centerPoint = function () { return new Vector2(this.left + ((this.right - this.left) >> 1), this.top + ((this.bottom - this.top) >> 1)); };
    ;
    return Box;
}());
exports.Box = Box;
var Rectangle = (function () {
    function Rectangle(left, top, width, height) {
        this.left = left;
        this.top = top;
        this.width = width;
        this.height = height;
    }
    Rectangle.prototype.clone = function () { return new Rectangle(this.left, this.top, this.width, this.height); };
    ;
    Rectangle.prototype.toBox = function () { return new Box(this.top, this.left + this.width, this.top + this.height, this.left); };
    ;
    Rectangle.prototype.centerPoint = function () { return new Vector2(this.left + (this.width >> 1), this.top + (this.height >> 1)); };
    ;
    Rectangle.prototype.contains = function (other) {
        if (other instanceof Rectangle) {
            return this.left <= other.left &&
                this.left + this.width >= other.left + other.width &&
                this.top <= other.top &&
                this.top + this.height >= other.top + other.height;
        }
        else {
            return other.x >= this.left &&
                other.x <= this.left + this.width &&
                other.y >= this.top &&
                other.y <= this.top + this.height;
        }
    };
    ;
    return Rectangle;
}());
exports.Rectangle = Rectangle;
function Random(max, min) { return Math.random() * (max - min + 1) + min; }
exports.Random = Random;
function Clamp(value, min, max) { return Math.min(Math.max(value, min), max); }
exports.Clamp = Clamp;
;
function Radians(degrees) { return degrees * Math.PI / 180; }
exports.Radians = Radians;
;
function Degrees(radians) { return radians * 180 / Math.PI; }
exports.Degrees = Degrees;
;
function IsPowerOfTwo(value) { return value > 0 && (value & (value - 1)) == 0; }
exports.IsPowerOfTwo = IsPowerOfTwo;
;
function NextPowerOfTwo(value) { var k = 1; while (k < value)
    k *= 2; return k; }
exports.NextPowerOfTwo = NextPowerOfTwo;
;
function polarToCartesian(x, y, r, angleInDegrees) {
    var __angleInRadians = Radians(angleInDegrees);
    return {
        x: x + (r * Math.cos(__angleInRadians)),
        y: y + (r * Math.sin(__angleInRadians))
    };
}
exports.polarToCartesian = polarToCartesian;

},{}],26:[function(require,module,exports){
(function (global){(function (){
/*! *****************************************************************************
Copyright (c) Microsoft Corporation.

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THIS SOFTWARE.
***************************************************************************** */

/* global global, define, System, Reflect, Promise */
var __extends;
var __assign;
var __rest;
var __decorate;
var __param;
var __metadata;
var __awaiter;
var __generator;
var __exportStar;
var __values;
var __read;
var __spread;
var __spreadArrays;
var __await;
var __asyncGenerator;
var __asyncDelegator;
var __asyncValues;
var __makeTemplateObject;
var __importStar;
var __importDefault;
var __classPrivateFieldGet;
var __classPrivateFieldSet;
var __createBinding;
(function (factory) {
    var root = typeof global === "object" ? global : typeof self === "object" ? self : typeof this === "object" ? this : {};
    if (typeof define === "function" && define.amd) {
        define("tslib", ["exports"], function (exports) { factory(createExporter(root, createExporter(exports))); });
    }
    else if (typeof module === "object" && typeof module.exports === "object") {
        factory(createExporter(root, createExporter(module.exports)));
    }
    else {
        factory(createExporter(root));
    }
    function createExporter(exports, previous) {
        if (exports !== root) {
            if (typeof Object.create === "function") {
                Object.defineProperty(exports, "__esModule", { value: true });
            }
            else {
                exports.__esModule = true;
            }
        }
        return function (id, v) { return exports[id] = previous ? previous(id, v) : v; };
    }
})
(function (exporter) {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };

    __extends = function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };

    __assign = Object.assign || function (t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p)) t[p] = s[p];
        }
        return t;
    };

    __rest = function (s, e) {
        var t = {};
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
            t[p] = s[p];
        if (s != null && typeof Object.getOwnPropertySymbols === "function")
            for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
                if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                    t[p[i]] = s[p[i]];
            }
        return t;
    };

    __decorate = function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };

    __param = function (paramIndex, decorator) {
        return function (target, key) { decorator(target, key, paramIndex); }
    };

    __metadata = function (metadataKey, metadataValue) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(metadataKey, metadataValue);
    };

    __awaiter = function (thisArg, _arguments, P, generator) {
        function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
        return new (P || (P = Promise))(function (resolve, reject) {
            function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
            function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
            function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
            step((generator = generator.apply(thisArg, _arguments || [])).next());
        });
    };

    __generator = function (thisArg, body) {
        var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
        return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
        function verb(n) { return function (v) { return step([n, v]); }; }
        function step(op) {
            if (f) throw new TypeError("Generator is already executing.");
            while (_) try {
                if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
                if (y = 0, t) op = [op[0] & 2, t.value];
                switch (op[0]) {
                    case 0: case 1: t = op; break;
                    case 4: _.label++; return { value: op[1], done: false };
                    case 5: _.label++; y = op[1]; op = [0]; continue;
                    case 7: op = _.ops.pop(); _.trys.pop(); continue;
                    default:
                        if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                        if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                        if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                        if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                        if (t[2]) _.ops.pop();
                        _.trys.pop(); continue;
                }
                op = body.call(thisArg, _);
            } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
            if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
        }
    };

    __createBinding = function(o, m, k, k2) {
        if (k2 === undefined) k2 = k;
        o[k2] = m[k];
    };

    __exportStar = function (m, exports) {
        for (var p in m) if (p !== "default" && !exports.hasOwnProperty(p)) exports[p] = m[p];
    };

    __values = function (o) {
        var s = typeof Symbol === "function" && Symbol.iterator, m = s && o[s], i = 0;
        if (m) return m.call(o);
        if (o && typeof o.length === "number") return {
            next: function () {
                if (o && i >= o.length) o = void 0;
                return { value: o && o[i++], done: !o };
            }
        };
        throw new TypeError(s ? "Object is not iterable." : "Symbol.iterator is not defined.");
    };

    __read = function (o, n) {
        var m = typeof Symbol === "function" && o[Symbol.iterator];
        if (!m) return o;
        var i = m.call(o), r, ar = [], e;
        try {
            while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
        }
        catch (error) { e = { error: error }; }
        finally {
            try {
                if (r && !r.done && (m = i["return"])) m.call(i);
            }
            finally { if (e) throw e.error; }
        }
        return ar;
    };

    __spread = function () {
        for (var ar = [], i = 0; i < arguments.length; i++)
            ar = ar.concat(__read(arguments[i]));
        return ar;
    };

    __spreadArrays = function () {
        for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
        for (var r = Array(s), k = 0, i = 0; i < il; i++)
            for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
                r[k] = a[j];
        return r;
    };

    __await = function (v) {
        return this instanceof __await ? (this.v = v, this) : new __await(v);
    };

    __asyncGenerator = function (thisArg, _arguments, generator) {
        if (!Symbol.asyncIterator) throw new TypeError("Symbol.asyncIterator is not defined.");
        var g = generator.apply(thisArg, _arguments || []), i, q = [];
        return i = {}, verb("next"), verb("throw"), verb("return"), i[Symbol.asyncIterator] = function () { return this; }, i;
        function verb(n) { if (g[n]) i[n] = function (v) { return new Promise(function (a, b) { q.push([n, v, a, b]) > 1 || resume(n, v); }); }; }
        function resume(n, v) { try { step(g[n](v)); } catch (e) { settle(q[0][3], e); } }
        function step(r) { r.value instanceof __await ? Promise.resolve(r.value.v).then(fulfill, reject) : settle(q[0][2], r);  }
        function fulfill(value) { resume("next", value); }
        function reject(value) { resume("throw", value); }
        function settle(f, v) { if (f(v), q.shift(), q.length) resume(q[0][0], q[0][1]); }
    };

    __asyncDelegator = function (o) {
        var i, p;
        return i = {}, verb("next"), verb("throw", function (e) { throw e; }), verb("return"), i[Symbol.iterator] = function () { return this; }, i;
        function verb(n, f) { i[n] = o[n] ? function (v) { return (p = !p) ? { value: __await(o[n](v)), done: n === "return" } : f ? f(v) : v; } : f; }
    };

    __asyncValues = function (o) {
        if (!Symbol.asyncIterator) throw new TypeError("Symbol.asyncIterator is not defined.");
        var m = o[Symbol.asyncIterator], i;
        return m ? m.call(o) : (o = typeof __values === "function" ? __values(o) : o[Symbol.iterator](), i = {}, verb("next"), verb("throw"), verb("return"), i[Symbol.asyncIterator] = function () { return this; }, i);
        function verb(n) { i[n] = o[n] && function (v) { return new Promise(function (resolve, reject) { v = o[n](v), settle(resolve, reject, v.done, v.value); }); }; }
        function settle(resolve, reject, d, v) { Promise.resolve(v).then(function(v) { resolve({ value: v, done: d }); }, reject); }
    };

    __makeTemplateObject = function (cooked, raw) {
        if (Object.defineProperty) { Object.defineProperty(cooked, "raw", { value: raw }); } else { cooked.raw = raw; }
        return cooked;
    };

    __importStar = function (mod) {
        if (mod && mod.__esModule) return mod;
        var result = {};
        if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
        result["default"] = mod;
        return result;
    };

    __importDefault = function (mod) {
        return (mod && mod.__esModule) ? mod : { "default": mod };
    };

    __classPrivateFieldGet = function (receiver, privateMap) {
        if (!privateMap.has(receiver)) {
            throw new TypeError("attempted to get private field on non-instance");
        }
        return privateMap.get(receiver);
    };

    __classPrivateFieldSet = function (receiver, privateMap, value) {
        if (!privateMap.has(receiver)) {
            throw new TypeError("attempted to set private field on non-instance");
        }
        privateMap.set(receiver, value);
        return value;
    };

    exporter("__extends", __extends);
    exporter("__assign", __assign);
    exporter("__rest", __rest);
    exporter("__decorate", __decorate);
    exporter("__param", __param);
    exporter("__metadata", __metadata);
    exporter("__awaiter", __awaiter);
    exporter("__generator", __generator);
    exporter("__exportStar", __exportStar);
    exporter("__createBinding", __createBinding);
    exporter("__values", __values);
    exporter("__read", __read);
    exporter("__spread", __spread);
    exporter("__spreadArrays", __spreadArrays);
    exporter("__await", __await);
    exporter("__asyncGenerator", __asyncGenerator);
    exporter("__asyncDelegator", __asyncDelegator);
    exporter("__asyncValues", __asyncValues);
    exporter("__makeTemplateObject", __makeTemplateObject);
    exporter("__importStar", __importStar);
    exporter("__importDefault", __importDefault);
    exporter("__classPrivateFieldGet", __classPrivateFieldGet);
    exporter("__classPrivateFieldSet", __classPrivateFieldSet);
});

}).call(this)}).call(this,typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})
},{}]},{},[1])(1)
});
