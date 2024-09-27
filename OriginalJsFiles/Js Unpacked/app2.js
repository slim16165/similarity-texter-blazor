(function (global){
/* jshint undef:true, unused:true, node:true, browser:true */
'use strict';

var $   = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
var App = require("./app.js");

// Main execution entry point
$(window).load(function() {
	setTimeout(function() {
		$(".loader").addClass('shrinked');
		var app = new App('simtexter');
	}, 700);
});

}).call(this,typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})