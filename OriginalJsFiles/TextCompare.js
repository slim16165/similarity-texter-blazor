(function e(t, n, r) { function s(o, u) { if (!n[o]) { if (!t[o]) { var a = typeof require == "function" && require; if (!u && a) return a(o, !0); if (i) return i(o, !0); var f = new Error("Cannot find module '" + o + "'"); throw f.code = "MODULE_NOT_FOUND", f } var l = n[o] = { exports: {} }; t[o][0].call(l.exports, function (e) { var n = t[o][1][e]; return s(n ? n : e) }, l, l.exports, e, t, n, r) } return n[o].exports } var i = typeof require == "function" && require; for (var o = 0; o < r.length; o++)s(r[o]); return s })({
	1: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		var Controller = require('./controller.js');
		var Storage = require('./storage.js');
		var Template = require('./template.js');
		var View = require('./view.js');

		/**
		 * Creates an instance of the application.
		 * @constructor
		 * @this  {App}
		 * @param {String} namespace - the namespace of the app (i.e. "simtexter")
		 */
		function App(namespace) {
			// App's default settings (comparison & input reading options)
			var defaults = {
				'minMatchLength': { id: '#min-match-length', type: 'MyInputText', value: 4 },
				'ignoreFootnotes': { id: '#ignore-footnotes', type: 'checkbox', value: false },
				'ignoreLetterCase': { id: '#ignore-letter-case', type: 'checkbox', value: true },
				'ignoreNumbers': { id: '#ignore-numbers', type: 'checkbox', value: false },
				'ignorePunctuation': { id: '#ignore-punctuation', type: 'checkbox', value: true },
				'replaceUmlaut': { id: '#replace-umlaut', type: 'checkbox', value: true }
			};

			this.storage = new Storage(namespace, defaults);
			this.template = new Template();
			this.view = new View(this.template);
			this.controller = new Controller(this.storage, this.view);
		}

		module.exports = App;
	}, { "./controller.js": 2, "./storage.js": 4, "./template.js": 5, "./view.js": 6 }], 2: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var FileInputReader = require('../inputReader/fileInputReader.js');
			var MyInputText = require('./MyInputText.js');
			var SimTexter = require('../simtexter/simtexter.js');
			var TextInputReader = require('../inputReader/textInputReader.js');

			/**
			 * Creates an instance of a {Controller}, 
			 * which handles user interaction (data reading, input control, comparison).
			 * Interacts with the {View} object to render the final output.
			 * @constructor
			 * @this  {Controller}
			 * @param {Storage} storage - the object that holds the app's settings 
			 * @param {View}    view    - the app's view
			 */
			function Controller(storage, view) {
				this.storage = storage;
				this.view = view;
				this.maxCharactersPerPage = 1900;
				this.maxNumberOfPages = 500;
				this.MyInputTexts = [new MyInputText(), new MyInputText()];

				this._bindEvents();
				this._updateUI(this.storage.data);
			}

			/**
			 * Displays a warning message if input is too long (> maxNumberOfPages).
			 * @function
			 * @private
			 * @param {Number} idx - the index of the {MyInputText} object in MyInputTexts[]
			 */
			Controller.prototype._alertLongInput = function (idx) {
				var self = this;

				// Compute approximate number of pages for MyInputText
				var nrOfPages = self.MyInputTexts[idx].getNumberOfPages(self.maxCharactersPerPage);
				// If greater than maximum number of pages, display warning message
				if (nrOfPages > self.maxNumberOfPages) {
					var inputMode = self.MyInputTexts[idx].mode;
					var message = [
						inputMode, ' ', (idx + 1), ' is too long. To prevent visualization issues, please consider truncating this ', inputMode.toLowerCase(), '.'
					].join('');
					var delay = self._computeReadingSpeed(message);
					self.view.showAlertMessage('warning', message, delay);
				}
			};

			/**
			 * Binds events.
			 * @function
			 * @private
			 */
			Controller.prototype._bindEvents = function () {
				var self = this;

				self.view.bind('changeSpinnerInput', function (id, newValue) {
					self._updateStorage(id, newValue);
				});

				self.view.bind('compare', function () {
					self._compare();
				});

				self.view.bind('dismissAlert');
				self.view.bind('hidePrintDialog');
				self.view.bind('initBootstrap');

				self.view.bind('inputFile', function (file, idx, loadingElem, tabPaneId) {
					self._readFile(file, idx, loadingElem, tabPaneId);
				});

				self.view.bind('MyInputText', function (text, idx, tabPaneId) {
					self._readText(text, idx, tabPaneId);
				});

				self.view.bind('print', function (hideModalPromise) {
					self._print(hideModalPromise);
				});

				self.view.bind('resize');
				self.view.bind('scrollToMatch');
				self.view.bind('selectTab');

				self.view.bind('selectHTMLOption', function (idx, newValue, text) {
					self.MyInputTexts[idx].setHTMLOption(newValue);
					if (text) {
						self._readText(text, idx, self.MyInputTexts[idx].tabPaneId);
					}
				});

				self.view.bind('selectSettingsOption', function (id, newValue) {
					self._updateStorage(id, newValue);
				});

				self.view.bind('showPrintDialog');
				self.view.bind('toggleInputPanel');
				self.view.bind('toggleSettingsSidebar');
				self.view.bind('toggleSettingsSidebarPanes');
			};

			/**
			 * Initiates the comparison process.
			 * @function
			 * @private
			 */
			Controller.prototype._compare = function () {
				var self = this;

				if (self._isInputValid()) {
					self.view.toggleWaitingCursor('show');
					var simtexter = new SimTexter(self.storage);

					setTimeout(function () {
						simtexter.compare(self.MyInputTexts).then(
							// On success, update information nodes and display similarities
							function (nodes) {
								self.view.results = {
									texts: simtexter.texts,
									uniqueMatches: simtexter.uniqueMatches
								};

								self.view.createTemplates();
								self.view.showSimilarities(nodes);
								self.view.resetScrollbars();
							},
							// On error, clear output panel and display warning message
							function (message) {
								self.view.clearOutputPanel();
								var delay = self._computeReadingSpeed(message);
								self.view.showAlertMessage('info', message, delay);
							}
						);
					}, 200);
				}
			};

			/**
			 * Returns the amount of time in milliseconds
			 * that a user needs in order to read a message.
			 * @function
			 * @private
			 * @param {String} message - the message to be read
			 */
			Controller.prototype._computeReadingSpeed = function (message) {
				var minMS = 6000;
				var speed = Math.round(message.length / 40) * 4000;
				return (speed > minMS) ? speed : minMS;
			};

			/**
			 * Checks if the user has provided a valid input
			 * in both source and target input panes.
			 * If not, the user is prompted.
			 * @function
			 * @private
			 * @returns {Boolean} - true if input is valid, else false.
			 */
			Controller.prototype._isInputValid = function () {
				var self = this,
					isValid = true,
					activeTabPaneIds = self.view.getActiveTabPaneIds(),
					iTextsLength = self.MyInputTexts.length;

				for (var i = 0; i < iTextsLength; i++) {
					var MyInputText = self.MyInputTexts[i];
					var activeTabPaneId = activeTabPaneIds[i];

					var isMyInputTextValid = (MyInputText.text !== undefined && MyInputText.tabPaneId === activeTabPaneId);

					if (!isMyInputTextValid) {
						self.view.toggleErrorStatus('show', activeTabPaneId);
					} else {
						self.view.toggleErrorStatus('hide', activeTabPaneId);
					}

					isValid = isValid && isMyInputTextValid;
				}

				return isValid;
			};

			/**
			 * Sends the contents of the current window
			 * to the system's printer for printing. 
			 * @function
			 * @private
			 * @param {Promise} hideModalPromise - a promise that handles the hiding 
			 * 																		 of the 'PRINT OUTPUT' dialog. 
			 * 																		 When resolved, the current window 
			 * 																		 is sent to printing.
			 */
			Controller.prototype._print = function (hideModalPromise) {
				var success = function () {
					setTimeout(function () {
						window.print();
					}, 700);
				};

				$.when(hideModalPromise).then(success);
			};

			/**
			 * Extracts the contents of the selected file
			 * and updates the relevant fields of the {MyInputText} object.
			 * @function
			 * @private
			 * @param {FileList} file        - the file selected by the user
			 * @param {Number}   idx         - the index of the {MyInputText} object 
			 * 																 in MyInputTexts[] to be updated.
			 *                                 0: input in left-side pane 
			 *                                 1: input in right-side pane
			 * @param {Object}   loadingElem - the node element that shows 
			 *                                 the progress of reading  
			 * @param {String}   tabPaneId   - the id of the active tab pane
			 */
			Controller.prototype._readFile = function (file, idx, loadingElem, tabPaneId) {
				var self = this,
					ignoreFootnotes = self.storage.getItemValueByKey('ignoreFootnotes');

				var success = function (text) {
					// Update {MyInputText} object
					self.MyInputTexts[idx].setFileInput(file, text, tabPaneId);
					self.view.loading('done', loadingElem);
					self.view.clearTabPaneTextInput(idx);
					self._alertLongInput(idx);
				};

				var error = function (message) {
					self.MyInputTexts[idx].reset();
					self.view.loading('error', loadingElem);
					self.view.clearTabPaneTextInput(idx);

					var delay = self._computeReadingSpeed(message);
					self.view.showAlertMessage('error', message, delay);
				};

				if (file) {
					var loadingStarted = self.view.loading('start', loadingElem);
					var fileInputReader = new FileInputReader(file, ignoreFootnotes);
					fileInputReader.readFileInput(loadingStarted).then(success, error);
				} else {
					self.view.loading('cancel', loadingElem);
					self.MyInputTexts[idx].reset();
				}
			};

			/**
			 * Extracts the contents of the typed/pasted HTML/plain text
			 * and updates the relevant fields of the {MyInputText} object.
			 * @function
			 * @private
			 * @param {String} text      - the HTML/plain text provided by the user
			 * @param {Number} idx       - the index of the {MyInputText} object 
			 * 														 in MyInputTexts[] to be updated.
			 *                             0: input in left-side pane, 
			 *                             1: input in right-side pane
			 * @param {String} tabPaneId - the id of the active tab pane
			 */
			Controller.prototype._readText = function (text, idx, tabPaneId) {
				var self = this;

				var success = function (cleanedText) {
					// Update {MyInputText} object
					self.MyInputTexts[idx].setTextInput(cleanedText, tabPaneId);
					self.view.toggleCompareBtn('enable');
					self.view.clearTabPaneFileInput(idx);
					self._alertLongInput(idx);
				};

				var error = function (message) {
					self.MyInputTexts[idx].reset();
					self.view.toggleCompareBtn('enable');
					var delay = self._computeReadingSpeed(message);
					self.view.showAlertMessage('error', message, delay);
				};

				if (text.length > 0 && /\S/.test(text)) {
					if (self.MyInputTexts[idx].isHTML) {
						self.view.toggleCompareBtn('disable');
						var textInputReader = new TextInputReader();
						textInputReader.readTextInput(text).then(success, error);
					} else {
						success(text);
					}
				} else {
					self.MyInputTexts[idx].reset();
				}
			};

			/**
			 * Updates the value of a setting, stored in the {Storage} object.
			 * @function
			 * @private
			 * @param {String}           id       - the id of the setting
			 * @param {(Boolean|Number)} newValue - the new value of the setting
			 */
			Controller.prototype._updateStorage = function (id, newValue) {
				var self = this;
				self.storage.setItemValueById(id, newValue);
			};

			/**
			 * Updates the {View} object with the values of the settings,
			 * stored in the {Storage} object.
			 * @function
			 * @private
			 * @param {Object} data - the object that holds the storage's settings
			 */
			Controller.prototype._updateUI = function (data) {
				var self = this;

				for (var key in data) {
					var obj = data[key];
					self.view.updateUIOption(obj.id, obj.type, obj.value);
				}
			};

			module.exports = Controller;
		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, { "../inputReader/fileInputReader.js": 9, "../inputReader/textInputReader.js": 10, "../simtexter/simtexter.js": 14, "./MyInputText.js": 3 }], 3: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {MyInputText},
		 * which holds information on the user input.
		 * @constructor
		 * @this  {MyInputText}
		 * @param {String} mode      - the mode of input (i.e. "file" or "text")
		 * @param {File}   file      - the file selected by the user
		 * @param {String} text      - the input string
		 * @param {String} tabPaneId - the id of the tab pane
		 */
		function MyInputText(mode, file, text, tabPaneId) {
			this.tabPaneId = tabPaneId;
			this.mode = mode;
			this.isHTML = false;
			this.fileName = (file && file.name);
			this.text = text;
		}

		/**
		 * Returns the approximate number of pages of the input string.
		 * @function
		 * @param   {Number} maxCharactersPerPage - the maximum number of characters 
		 * 																					per page
		 * @returns {Number}                      - the ca. number of pages
		 */
		MyInputText.prototype.getNumberOfPages = function (maxCharactersPerPage) {
			return (this.text.length / maxCharactersPerPage);
		};

		/**
		 * Resets some fields of the {MyInputText}.
		 * @function
		 */
		MyInputText.prototype.reset = function () {
			this.tabPaneId = undefined;
			this.mode = undefined;
			this.fileName = undefined;
			this.text = undefined;
		};

		/**
		 * Sets the fields for the file input.
		 * @function
		 * @param {File}   file      - the file selected by the user
		 * @param {String} text      - the file input string
		 * @param {String} tabPaneId - the id of the tab pane
		 */
		MyInputText.prototype.setFileInput = function (file, text, tabPaneId) {
			this.tabPaneId = tabPaneId;
			this.mode = 'File';
			this.fileName = file.name;
			this.text = text;
		};

		/**
		 * Sets the fields for the text input.
		 * @function
		 * @param {String} text      - the text input string
		 * @param {String} tabPaneId - the id of the tab pane
		 */
		MyInputText.prototype.setTextInput = function (text, tabPaneId) {
			this.tabPaneId = tabPaneId;
			this.mode = 'Text';
			this.fileName = (this.isHTML) ? 'HTML text input' : 'Plain text input';
			this.text = text;
		};

		MyInputText.prototype.setHTMLOption = function (newValue) {
			this.isHTML = newValue;
		};

		module.exports = MyInputText;
	}, {}], 4: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {Storage},
		 * which stores the values of the app's settings.
		 * If local storage is supported by the browser,
		 * these settings are also stored under the specified namespace,
		 * thus providing the app with a state.
		 * The last stored settings will be restored 
		 * when refreshing the page or restarting the browser. 
		 * @constructor
		 * @this  {Storage}
		 * @param {String} namespace - the namespace of the app (i.e. "simtexter")
		 * @param {Object} data      - the object that holds the app's settings
		 */
		function Storage(namespace, data) {
			this._db = namespace;
			this.data = this._initialize(namespace, data);
		}

		/**
		 * Returns the value of a setting, retrieved by its key value.
		 * @function
		 * @param {String} key - the key value of the setting
		 */
		Storage.prototype.getItemValueByKey = function (key) {
			var self = this;
			return self._getItemByKey(key).value;
		};

		/**
		 * Sets the new value of a setting, retrieved by its id value.
		 * @function
		 * @param {String}           id       - the id of the setting
		 * @param {(Boolean|Number)} newValue - the new value of the setting
		 */
		Storage.prototype.setItemValueById = function (id, newValue) {
			var self = this,
				item = self._getItemById(id);

			item.value = newValue;
			self._save(self.data);
		};

		/**
		 * Retrieves a setting by its id value.
		 * @function
		 * @private
		 * @param {String} id - the id of the setting
		 */
		Storage.prototype._getItemById = function (id) {
			var self = this,
				data = self.data;

			for (var key in data) {
				var obj = data[key];
				if (obj.id === id) {
					return obj;
				}
			}

			return undefined;
		};

		/**
		 * Retrieves a setting by its key value.
		 * @function
		 * @private
		 * @param {String} key - the key value of the setting
		 */
		Storage.prototype._getItemByKey = function (key) {
			var self = this;
			return self.data[key];
		};

		/**
		 * Stores the app's settings in the web browser's local storage
		 * under the specified namespace.
		 * If local storage is not supported, stores the settings
		 * in {Storage.data}.
		 * @function
		 * @private
		 * @param {String} namespace - the namespace of the app
		 * @param {Object} data      - the object that holds the app's settings
		 */
		Storage.prototype._initialize = function (namespace, data) {
			if (localStorage) {
				if (!localStorage[namespace]) {
					localStorage.setItem(namespace, JSON.stringify(data));
				} else {
					var store = localStorage.getItem(namespace);
					return JSON.parse(store);
				}
			}

			return data;
		};

		/**
		 * Stores the settings in the local storage.
		 * @function
		 * @private
		 * @param {Object} data - the data (settings) to be updated
		 */
		Storage.prototype._save = function (data) {
			if (localStorage && localStorage[this._db]) {
				localStorage.setItem(this._db, JSON.stringify(data));
			}
			this.data = data;
		};

		module.exports = Storage;

	}, {}], 5: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {Template},
		 * which appends node elements in the DOM or updates their inner content. 
		 * @constructor
		 * @this {Template}
		 */
		function Template() {
		}

		/**
		 * Returns the node element of the template
		 * for displaying warning messages.
		 * @function
		 * @param   {String} type    - the type of warning
		 * @param   {String} message - the text of the warning message
		 * @returns {Object}         - the top node element
		 */
		Template.prototype.createAlertMessage = function (type, message) {
			var div = document.createElement('div');

			div.className = 'alert alert-warning';
			div.innerHTML = [
				'<table class="table table-condensed">',
				'<tbody>',
				'<tr>',
				'<td class="h5"><i class="fa fa-exclamation-circle"></i></td>',
				'<td>',
				'<h5>', type, '</h5>',
				'<p>', message, '</p>',
				'</td>',
				'</tr>',
				'</tbody>',
				'</table>'
			].join('');

			return div;
		};

		/**
		 * Updates the inner HTML content of the output titles.
		 * @function
		 * @param {Array} texts - the array that holds information about the user input
		 */
		Template.prototype.createOutputTitles = function (texts) {
			var targets = [document.getElementById('output-title-1'), document.getElementById('output-title-2')],
				tLength = targets.length;

			for (var i = 0; i < tLength; i++) {
				var fileName = texts[i].fileName || '';
				var mode = texts[i].inputMode;
				var target = targets[i];
				target.innerHTML = [
					'<p><b>', mode.toUpperCase(), ': </b>', fileName, '</p> ',
				].join('');
			}
		};

		/**
		 * Returns the node element of the template
		 * for displaying the "PRINT OUTPUT" dialog.
		 * @function
		 * @param   {Array} texts - the array that holds information 
		 * 													about the user input
		 * @returns {Object}      - the top node element
		 */
		Template.prototype.createPrintDialog = function (texts) {
			var section = document.createElement('section');

			section.id = 'modal-print';
			section.className = 'modal fade';
			section.setAttribute('tabindex', '-1');
			section.setAttribute('role', 'dialog');
			section.innerHTML = [
				'<div class="modal-dialog">',
				'<div class="modal-content">',
				'<div class="modal-header">',
				'<button type="button" class="close" data-dismiss="modal" aria-label="Close">',
				'<span aria-hidden="true">&times;</span>',
				'</button>',
				'<h4 class="modal-title">Print output</h4>',
				'</div>',
				'<div class="modal-body">',
				'<div class="row">',
				'<div class="col-xs-6">',
				'<div class="form-group form-group-sm">',
				'<label for="input-comment-1">1: Comment for ', texts[0].inputMode, '</label>',
				'<textarea id="input-comment-1" class="form-control" rows="5" autocomplete="off" placeholder="Type a comment"></textarea>',
				'</div>',
				'</div>',
				'<div class="col-xs-6">',
				'<div class="form-group form-group-sm">',
				'<label for="input-comment-2">2: Comment for ', texts[1].inputMode, '</label>',
				'<textarea id="input-comment-2" class="form-control" rows="5" autocomplete="off" placeholder="Type a comment"></textarea>',
				'</div>',
				'</div>',
				'</div>',
				'</div>',
				'<div class="modal-footer">',
				'<button type="button" class="btn btn-default btn-sm" data-dismiss="modal">Cancel</button>',
				'<button id="modal-print-btn" type="button" class="btn btn-primary btn-sm">Print</button>',
				'</div>',
				'</div>',
				'</div>'
			].join('');

			return section;
		};

		/**
		 * Updates the inner HTML content of the hidden, on screen, node element
		 * that holds the information (statistics & comments) to be printed.
		 * @function
		 * @param {Array}  texts         - the array that holds information 
		 * 																 about the user input
		 * @param {Number} uniqueMatches - the number of the unique matches found
		 */
		Template.prototype.createPrintSummary = function (texts, uniqueMatches) {
			var target = document.getElementById('print-summary');

			target.innerHTML = [
				'<h4>COMPARISON SUMMARY</h4>',
				'<h6>DATE/TIME: ', (new Date()).toUTCString(), '</h6>',
				'<table class="table table-condensed table-bordered">',
				'<thead>',
				'<tr>',
				'<th class="col-xs-2"></th>',
				'<th class="col-xs-5">', texts[0].fileName, '</th>',
				'<th class="col-xs-5">', texts[1].fileName, '</th>',
				'</tr>',
				'</thead>',
				'<tbody>',
				'<tr>',
				'<th>Comment</th>',
				'<td id="print-comment-1"></td>',
				'<td id="print-comment-2"></td>',
				'</tr>',
				'<tr>',
				'<th>Type</th>',
				'<td>', texts[0].inputMode, '</td>',
				'<td>', texts[1].inputMode, '</td>',
				'</tr>',
				'<tr>',
				'<th>Characters</th>',
				'<td>', texts[0].nrOfCharacters, '</td>',
				'<td>', texts[1].nrOfCharacters, '</td>',
				'</tr>',
				'<tr>',
				'<th>Words</th>',
				'<td>', texts[0].nrOfWords, '</td>',
				'<td>', texts[1].nrOfWords, '</td>',
				'</tr>',
				'<tr>',
				'<th>Unique matches</th>',
				'<td colspan="2">', uniqueMatches, '</td>',
				'</tr>',
				'</tbody>',
				'</table>'
			].join('');
		};

		/**
		 * Updates the inner HTML content
		 * of the node element that holds the statistical data. 
		 * @function
		 * @param {Array}  texts         - the array that holds information 
		 * 																 about the user input
		 * @param {Number} uniqueMatches - the number of the unique matches found
		 */
		Template.prototype.createStatistics = function (texts, uniqueMatches) {
			var target = document.getElementById('statistics');

			target.innerHTML = [
				'<table class="table table-condensed table-bordered">',
				'<thead>',
				'<tr>',
				'<th class="col-xs-2"></th>',
				'<th class="col-xs-5">', texts[0].fileName, '</th>',
				'<th class="col-xs-5">', texts[1].fileName, '</th>',
				'</tr>',
				'</thead>',
				'<tbody>',
				'<tr>',
				'<th>Type</th>',
				'<td>', texts[0].inputMode, '</td>',
				'<td>', texts[1].inputMode, '</td>',
				'</tr>',
				'<tr>',
				'<th>Characters</th>',
				'<td>', texts[0].nrOfCharacters, '</td>',
				'<td>', texts[1].nrOfCharacters, '</td>',
				'</tr>',
				'<tr>',
				'<th>Words</th>',
				'<td>', texts[0].nrOfWords, '</td>',
				'<td>', texts[1].nrOfWords, '</td>',
				'</tr>',
				'<tr>',
				'<th>Unique matches</th>',
				'<td colspan="2">', uniqueMatches, '</td>',
				'</tr>',
				'</tbody>',
				'</table>'
			].join('');
		};

		module.exports = Template;
	}, {}], 6: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var TargetMatch = require('../autoScroll/targetMatch.js');

			/**
			 * Creates an instance of a {View},
			 * which implements all the UI logic of the application.
			 * @constructor
			 * @this  {View}
			 * @param {Template} template - the object that appends/updates elements 
			 * 															in the DOM
			 */
			function View(template) {
				this.template = template;
				this.results = {};

				// Selectors
				this.$alertsPanel = $('#alerts-panel');
				this.$compareBtn = $('#compare-btn');
				this.$contentWrapper = $('#content-wrapper');
				this.$file = $(':file');
				this.$htmlOptions = $('#html-text-1, #html-text-2');
				this.$inputLnk = $('#input-lnk');
				this.$inputPanel = $('#input-panel');
				this.$inputPanes = $('#input-pane-1, #input-pane-2');
				this.$inputFiles = $('#input-file-1, #input-file-2');
				this.$MyInputTexts = $('#input-text-1, #input-text-2');
				this.$outputPanel = $('#output-panel');
				this.$outputTexts = $('#comparison-output-1, #comparison-output-2');
				this.$outputTextContainers = $('#comparison-output-1 > .comparison-output-container, #comparison-output-2 > .comparison-output-container');
				this.$outputParagraphs = $('#comparison-output-1 > .comparison-output-container > p, #comparison-output-2 > .comparison-output-container > p');
				this.$printBtn = $('#print-btn');
				this.$settingsSidebar = $('#settings-sidebar');
				this.$settingsSidebarLnk = $('#settings-sidebar-lnk');
				this.$settingsSidebarPanes = $('#comparison-options-pane, #input-options-pane');
				this.$spinner = $('#min-match-length-spinner');
				this.$tooltip = $('[data-toggle="tooltip"], [rel="tooltip"]');

				this._resetTextInputTabPanes();
				this._updateOutputPanelHeight();
				this._updateAlertsPanelWidth();
			}

			/**
			 * Binds events depending on the name specified.
			 * @function
			 * @param {String} event     - the name of the event
			 * @param {Function} handler - the callback function
			 */
			View.prototype.bind = function (event, handler) {
				var self = this;

				switch (event) {
					case 'changeSpinnerInput':
						self.$spinner
							.on('change mousewheel DOMMouseScroll', 'input[type="text"]', function (e) {
								var elem = e.target;
								var id = self._getId(elem);
								var minMatchLength = parseInt($(elem).val(), 10);

								if (e.type === 'mousewheel' || e.type === 'DOMMouseScroll') {
									// scrolling up
									if (e.originalEvent.wheelDelta > 0 || e.originalEvent.detail < 0) {
										minMatchLength += 1;
									}
									// scrolling down
									else {
										minMatchLength -= 1;
									}
								}

								minMatchLength = (minMatchLength < 1) ? 1 : minMatchLength;

								handler(id, minMatchLength);
								self.updateUIOption(id, 'MyInputText', minMatchLength);
							}
							)
							.on('click', '.btn', function (e) {
								e.stopPropagation();

								var $elem = $(e.delegateTarget).find('input[type="text"]');
								var id = self._getId($elem);
								var minMatchLength = parseInt($elem.val(), 10);

								if ($(e.currentTarget).hasClass('plus')) {
									minMatchLength += 1;
								} else {
									minMatchLength = (minMatchLength > 1) ? (minMatchLength - 1) : minMatchLength;
								}

								handler(id, minMatchLength);
								self.updateUIOption(id, 'MyInputText', minMatchLength);
							});
						break;

					case 'compare':
						self.$compareBtn.on('click', function (e) {
							e.stopPropagation();

							$(this).tooltip('hide');
							self.$settingsSidebar.removeClass('expanded');
							setTimeout(function () {
								handler();
							}, 200);
						});
						break;

					case 'dismissAlert':
						self.$alertsPanel.on('click', '.alert', function () {
							$(this).remove();
						});
						break;

					case 'initBootstrap':
						self.$tooltip.tooltip({
							container: 'body',
							delay: { "show": 800, "hide": 0 },
							html: true,
							placement: 'bottom',
							trigger: 'hover'
						});

						self.$file.filestyle({
							buttonName: "btn-primary",
							buttonText: "Browse file",
							placeholder: "No file selected",
							size: "sm"
						});
						break;

					case 'inputFile':
						self.$inputFiles.on('change', function (e) {
							var elem = e.target;
							var id = self._getId(elem);

							var tabPaneId = self._getId($(elem).parents('.tab-pane'));
							self.toggleErrorStatus('hide', tabPaneId);

							var file = elem.files[0];
							var idx = self._getIndex(id);
							var loadingElem = $(elem).parent();
							handler(file, idx, loadingElem, tabPaneId);
						});
						break;

					case 'MyInputText':
						self.$MyInputTexts.on('change input', function (e) {
							var elem = e.target;
							var $elem = $(elem);
							var tabPaneId = self._getId($elem.parents('.tab-pane'));

							if (e.type === 'input') {
								self.toggleErrorStatus('hide', tabPaneId);
							}

							if (e.type === 'change') {
								var id = self._getId(elem);
								var text = $elem.val();
								var idx = self._getIndex(id);
								handler(text, idx, tabPaneId);
							}
						});
						break;

					case 'hidePrintDialog':
						self.$contentWrapper.on('hide.bs.modal', '.modal', function (e) {
							self._togglePrintDialog('hide', e.target);
						});
						break;

					case 'print':
						self.$contentWrapper.on('click', '#modal-print-btn', function (e) {
							e.stopPropagation();

							var inputComment1 = $('#input-comment-1').val();
							var inputComment2 = $('#input-comment-2').val();
							$('#print-comment-1').text(inputComment1);
							$('#print-comment-2').text(inputComment2);

							var hideModalPromise = $('.modal').modal('hide').promise();
							handler(hideModalPromise);
						});
						break;

					case 'resize':
						$(window).on('resize', function () {
							self._updateOutputPanelHeight();
							self._updateAlertsPanelWidth();
						});
						break;

					case 'scrollToMatch':
						self.$outputTexts.on('click', 'a', function (e) {
							e.preventDefault();
							e.stopPropagation();

							var targetMatch = new TargetMatch(e.target);
							var scrollPosition = targetMatch.getScrollPosition();
							targetMatch.scroll(scrollPosition);
						});
						break;

					case 'selectHTMLOption':
						self.$inputPanel.on('change', 'input[type="checkbox"]', function (e) {
							var elem = e.target;
							var id = self._getId(elem);
							var idx = self._getIndex(id);
							var newValue = $(elem).prop('checked');
							var text = self.$MyInputTexts.eq(idx).val();
							handler(idx, newValue, text);
						});
						break;

					case 'selectSettingsOption':
						self.$settingsSidebarPanes.on('change', 'input[type="checkbox"]', function (e) {
							var elem = e.target;
							var id = self._getId(elem);
							var newValue = $(elem).prop('checked');
							handler(id, newValue);
						});
						break;

					case 'selectTab':
						self.$inputPanes.on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {
							var lastTabPaneId = $(e.relatedTarget).attr('href');
							self.toggleErrorStatus('hide', lastTabPaneId);
						});
						break;

					case 'showPrintDialog':
						self.$printBtn.on('click', function (e) {
							e.stopPropagation();
							self._togglePrintDialog('show');
						});
						break;

					case 'toggleInputPanel':
						self.$inputLnk.on('click', function (e) {
							e.preventDefault();
							e.stopPropagation();
							// Hide tooltip (if any)
							$(this).tooltip('hide');
							self._toggleInputPanel('toggle');
						});
						break;

					case 'toggleSettingsSidebar':
						self.$settingsSidebarLnk.on('click', function (e) {
							e.preventDefault();
							e.stopPropagation();
							// Hide tooltip (if any)
							$(this).tooltip('hide');
							self.$settingsSidebar.toggleClass('expanded');
						});

						// Hide settings sidebar when clicking inside the 'nav' and '#content-wrapper' elements
						$('body').on('click', 'nav, #content-wrapper', function () {
							self.$settingsSidebar.removeClass('expanded');
						});
						break;

					case 'toggleSettingsSidebarPanes':
						self.$settingsSidebar.on('click', '.panel-title', function () {
							$(this).toggleClass('active');
						});
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Removes all <p> nodes from each output pane
			 * and hides the output panel.
			 * @function
			 */
			View.prototype.clearOutputPanel = function () {
				var self = this;

				self.$outputParagraphs.each(function () {
					$(this).remove();
				});
				self._toggleOutputPanel('hide');
				self.toggleWaitingCursor('hide');
			};

			/**
			 * Clears all input from the "FILE" tab pane.
			 * @function
			 * @param {Number} idx - the number of the tab pane
			 *                       0: for left-side pane, 1: for right-side pane
			 */
			View.prototype.clearTabPaneFileInput = function (idx) {
				var self = this;
				var tabPaneId = '#tab-file-' + (idx + 1);
				$(tabPaneId + ' input').filestyle('clear');
				self.toggleErrorStatus('hide', tabPaneId);
				self.loading('cancel', tabPaneId);
			};

			/**
			 * Clears all input from the "TEXT" tab pane.
			 * @function
			 * @param {Number} idx - the number of the tab pane
			 *                       0: for left-side pane, 1: for right-side pane
			 */
			View.prototype.clearTabPaneTextInput = function (idx) {
				var self = this;
				var tabPaneId = '#tab-text-' + (idx + 1);
				$(tabPaneId + ' textarea').val('');
				self.toggleErrorStatus('hide', tabPaneId);
			};

			/**
			 * Creates the node templates.
			 * @function
			 */
			View.prototype.createTemplates = function () {
				var self = this;
				self.template.createPrintSummary(self.results.texts, self.results.uniqueMatches);
				self.template.createStatistics(self.results.texts, self.results.uniqueMatches);
				self.template.createOutputTitles(self.results.texts);
			};

			/**
			 * Returns the ids of active tab panes as an array of strings.
			 * @function
			 * @returns {Array<String>} - the ids of the active tab panes
			 */
			View.prototype.getActiveTabPaneIds = function () {
				var self = this,
					tabPaneIds = [];

				$('.tab-pane.active').each(function () {
					var tabPaneId = self._getId(this);
					tabPaneIds.push(tabPaneId);
				});
				return tabPaneIds;
			};

			/**
			 * Shows/hides an node element depending on the event specified.
			 * Used to show the progress of a process (e.g. input reading).
			 * @function
			 * @param {String} event  - the name of the event
			 * @param {Object} target - the id of the node element
			 */
			View.prototype.loading = function (event, target) {
				var self = this;

				switch (event) {
					case 'start':
						self.toggleCompareBtn('disable');
						$(target).find('.fa').addClass('hidden');
						$(target).find('.fa-spinner').removeClass('hidden');
						break;

					case 'done':
						self.toggleCompareBtn('enable');
						$(target).find('.fa').addClass('hidden');
						$(target).find('.fa-check').removeClass('hidden');
						break;

					case 'cancel':
						self.toggleCompareBtn('enable');
						$(target).find('.fa').addClass('hidden');
						break;

					case 'error':
						self.toggleCompareBtn('enable');
						$(target).find('.fa').addClass('hidden');
						$(target).find('.fa-times').removeClass('hidden');
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Resets the scroll bars.
			 * @function
			 */
			View.prototype.resetScrollbars = function () {
				var self = this;
				self.$outputTexts.scrollTop(0);
			};

			/**
			 * Clears text from textarea and unchecks checkboxes.
			 * Important for Internet Explorer, 
			 * since it does not recognize the "autocomplete='off'" attribute.
			 * @function
			 * @private
			 */
			View.prototype._resetTextInputTabPanes = function () {
				var self = this;
				self.$htmlOptions.prop('checked', false);
				self.$MyInputTexts.val('');
			};

			/**
			 * Displays a warning message.
			 * @function
			 * @param {String} type    - the type of the message
			 * @param {String} message - the text of the message
			 * @param {Number} delay   - the time in milliseconds, during which the message 
			 *                           should remain visible
			 */
			View.prototype.showAlertMessage = function (type, message, delay) {
				var self = this,
					alertMessage = self.template.createAlertMessage(type, message);

				self.$alertsPanel.append($(alertMessage));
				setTimeout(function () {
					self.$alertsPanel.children().eq(0).remove();
				}, delay);
			};

			/**
			 * Appends the array of nodes returned by the comparison 
			 * to the <p> node element of each output pane 
			 * and shows the output panel.
			 * @function
			 * @param {Array} nodes - the array of nodes returned by the comparison
			 */
			View.prototype.showSimilarities = function (nodes) {
				var self = this,
					nLength = nodes.length;

				for (var i = 0; i < nLength; i++) {
					var $p = $('<p>').append(nodes[i]);
					self.$outputTextContainers.eq(i).html($p);
				}

				self._toggleOutputPanel('show');
				setTimeout(function () {
					self._toggleInputPanel('hide');
				}, 100);

				self.toggleWaitingCursor('hide');
			};

			/**
			 * Enables/disables the compare button
			 * depending on the event specified.
			 * @function
			 * @param {String} event - the name of the event
			 */
			View.prototype.toggleCompareBtn = function (event) {
				var self = this;
				switch (event) {
					case 'enable':
						self.$compareBtn.prop('disabled', false);
						break;

					case 'disable':
						self.$compareBtn.prop('disabled', true);
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Toggles the class "has-error", 
			 * which applies a red border around input node elements,
			 * to prompt the user in case of erroneous input.
			 * @function
			 * @param {String} event     - the name of the event
			 * @param {String} tabPaneId - the id of the tab pane
			 */
			View.prototype.toggleErrorStatus = function (event, tabPaneId) {
				switch (event) {
					case 'show':
						$(tabPaneId + ' .apply-error').addClass('has-error');
						break;

					case 'hide':
						$(tabPaneId + ' .apply-error').removeClass('has-error');
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Toggles the style of the cursor (from "default" to "waiting", and vice versa)
			 * depending on the event specified.
			 * @function
			 * @param {String} event - the name of the event
			 */
			View.prototype.toggleWaitingCursor = function (event) {
				switch (event) {
					case 'show':
						document.body.className = 'waiting';
						break;

					case 'hide':
						document.body.className = '';
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Updates the value of a setting in the UI.
			 * @function
			 * @param {String}           id    - the id of the control element 
			 * @param {String}           type  - the type of the control element
			 * @param {(Boolean|Number)} value - the value of the setting
			 */
			View.prototype.updateUIOption = function (id, type, value) {
				switch (type) {
					case 'checkbox':
						$(id).prop('checked', value);
						break;
					case 'select':
						$(id).val(value);
						break;
					default:
						$(id).val(value);
				}
			};

			/**
			 * Calculates the height of the output pane
			 * so that it fits entirely in the window.
			 * @function
			 * @private
			 */
			View.prototype._computeOutputPanelHeight = function () {
				var self = this;
				var bodyHeight = $('body').outerHeight(true);
				var outputPos = self.$outputPanel.offset().top;
				var outputTopPadding = parseInt(self.$outputPanel.css('padding-top'), 10);
				var elemPos = self.$outputTexts.eq(0).offset().top;
				var posOffset = (elemPos - outputPos);
				return bodyHeight - outputPos - (posOffset + outputTopPadding);
			};

			/**
			 * Returns the id of a node element as a string (e.g. "#id").
			 * @function
			 * @param   {Object} target - the id of the node element
			 * @returns {String}        - the string of the node element's id 
			 */
			View.prototype._getId = function (target) {
				return '#' + $(target).attr('id');
			};

			/**
			 * Returns the number contained in the id of a node element.
			 * @function
			 * @private
			 * @param   {String} id - the id of the node element
			 * @returns {Number}    - the number of the id
			 */
			View.prototype._getIndex = function (id) {
				var tokens = id.split('-');
				var idx = tokens[tokens.length - 1];
				return parseInt(idx, 10) - 1;
			};

			View.prototype._toggleInputPanel = function (event) {
				var self = this;
				switch (event) {
					case 'toggle':
						$('.btn-group.open').removeClass('open');
						self.$inputPanel.toggleClass('expanded');
						break;

					case 'hide':
						self.$inputPanel.removeClass('expanded');
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Shows/hides the output panel depending on the event specified.
			 * @function
			 * @private
			 * @param {String} event - the name of the event
			 */
			View.prototype._toggleOutputPanel = function (event) {
				var self = this;
				switch (event) {
					case 'show':
						self.$outputPanel.removeClass('invisible');
						break;

					case 'hide':
						self.$outputPanel.addClass('invisible');
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Shows/hides the "PRINT OUTPUT" dialog depending on the event specified.
			 * @function
			 * @private
			 * @param {String} event  - the name of the event
			 * @param {Object} target - the node element to be removed
			 */
			View.prototype._togglePrintDialog = function (event, target) {
				var self = this;
				switch (event) {
					case 'show':
						var $printDialog = $(self.template.createPrintDialog(self.results.texts));
						self.$contentWrapper.append($printDialog);
						$printDialog.modal('show');
						break;

					case 'hide':
						$(target).remove();
						break;

					default:
						throw new Error('Event type not valid.');
				}
			};

			/**
			 * Updates the width of the alerts' panel.
			 * @function
			 * @private
			 */
			View.prototype._updateAlertsPanelWidth = function () {
				var self = this,
					marginLR = 3 * 2,
					navWidth = $('nav').width(),
					navLeftWidth = $('nav .pull-left').outerWidth(),
					navRightWidth = $('nav .pull-right').outerWidth(),
					maxWidth = navWidth - (navLeftWidth + navRightWidth + marginLR);

				self.$alertsPanel.css({
					'left': navLeftWidth + 'px',
					'max-width': maxWidth + 'px'
				});
			};

			/**
			 * Updates the height of each output pane.
			 * @function
			 * @private
			 */
			View.prototype._updateOutputPanelHeight = function () {
				var self = this,
					h = self._computeOutputPanelHeight();

				self.$outputTexts.each(function () {
					$(this).css('height', h + 'px');
				});
			};

			module.exports = View;

		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, { "../autoScroll/targetMatch.js": 8 }], 7: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {ScrollPosition}.
		 * @constructor
		 * @this  {ScrollPosition}
		 * @param {Number} topPadding    - the top padding
		 * @param {Number} bottomPadding - the bottom padding
		 * @param {Number} yPosition     - the vertical position of the scroll bar
		 */
		function ScrollPosition(topPadding, bottomPadding, yPosition) {
			this.topPadding = topPadding;
			this.bottomPadding = bottomPadding;
			this.yPosition = yPosition;
		}

		module.exports = ScrollPosition;

	}, {}], 8: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var ScrollPosition = require('./scrollPosition.js');

			/**
			 * Creates an instance of a {TargetMatch},
			 * which hold information on the target match node element.
			 * @constructor
			 * @this  {TargetMatch}
			 * @param {elem} elem - the source match node
			 */
			function TargetMatch(elem) {
				this.$srcElem = $(elem);
				this.$srcParent = $(this.$srcElem.parent().parent().parent());

				this.$elem = $(this.$srcElem.attr('href'));
				this.$wrapper = $(this.$elem.parent());
				this.$container = $(this.$wrapper.parent());
				this.$parent = $(this.$container.parent());

				this.parentHeight = this.$parent[0].getBoundingClientRect().height;
				this.containerTBPadding = parseInt(this.$container.css('padding-top'), 10) + parseInt(this.$container.css('padding-bottom'), 10);
				this.wrapperTopPadding = parseFloat(this.$wrapper.css('padding-top'));
				this.wrapperBottomPadding = parseFloat(this.$wrapper.css('padding-bottom'));
			}

			/**
			 * Returns the new scroll position of the target match node.
			 * @function
			 * @returns {ScrollPosition} - the new scroll position
			 */
			TargetMatch.prototype.getScrollPosition = function () {
				var self = this,
					wrapperBottom = self.$wrapper.outerHeight(true) + self.containerTBPadding,
					wrapperTopPadding = self.wrapperTopPadding,
					wrapperBottomPadding = self.wrapperBottomPadding,
					// Calculate difference on the y axis (relative to parent element)
					yPosDiff = (self.$srcElem.offset().top - self.$srcParent.offset().top) - (self.$elem.offset().top - self.$parent.offset().top);

				// Remove top padding
				if (wrapperTopPadding > 0) {
					yPosDiff += wrapperTopPadding;
					wrapperBottom -= wrapperTopPadding;
					wrapperTopPadding = 0;
				}

				// Remove bottom padding
				if (wrapperBottomPadding > 0) {
					wrapperBottom -= wrapperBottomPadding;
					wrapperBottomPadding = 0;
				}

				// Compute new scroll position
				var yScrollPos = self.$parent.scrollTop() - yPosDiff;

				// Add bottom padding, if needed
				if (yScrollPos > (wrapperBottom - self.parentHeight)) {
					var bottomOffset = (yScrollPos + self.parentHeight) - (wrapperBottom);
					wrapperBottomPadding = Math.abs(bottomOffset);
				}

				// Add top padding, if needed
				if (yScrollPos < 0) {
					var topOffset = yScrollPos;
					wrapperTopPadding = Math.abs(topOffset);
					yScrollPos -= topOffset;
				}

				return new ScrollPosition(wrapperTopPadding, wrapperBottomPadding, yScrollPos);
			};

			/**
			 * Animates scrolling to the new position.
			 * @function
			 * @param {ScrollPosition} scrollPosition - the new scroll position
			 */
			TargetMatch.prototype.scroll = function (scrollPosition) {
				var self = this;

				self.$wrapper.animate({
					'padding-top': scrollPosition.topPadding,
					'padding-bottom': scrollPosition.bottomPadding,
				}, 700);

				self.$parent.animate({
					'scrollTop': scrollPosition.yPosition,
				}, 700);

			};

			module.exports = TargetMatch;
		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, { "./scrollPosition.js": 7 }], 9: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var JSZip = (typeof window !== "undefined" ? window['JSZip'] : typeof global !== "undefined" ? global['JSZip'] : null);

			/**
			 * Creates an instance of a {FileInputReader},
			 * which parses and extracts the text contents of the DOCX, ODT and TXT files.
			 * @constructor
			 * @this  {FileInputReader}
			 * @param {File}    file            - the file selected by the user
			 * @param {Boolean} ignoreFootnotes - the option for including/excluding 
			 * 																		the document's footnotes from parsing
			 */
			function FileInputReader(file, ignoreFootnotes) {
				this.file = file;
				this.ignoreFootnotes = ignoreFootnotes;
			}

			/**
			 * Returns a promise that handles the file reading.
			 * When resolved, the contents of the file are returned as a string. 
			 * @function
			 * @param   {Function} loadingStarted - the callback function 
			 * 																		  for the onloadstart event
			 * @returns {Promise} 
			 */
			FileInputReader.prototype.readFileInput = function (loadingStarted) {
				var self = this,
					file = self.file,
					fileType = self._getFileType(),
					deferred = $.Deferred(),
					fr = new FileReader();

				fr.onerror = function (e) {
					var error = e.target.error;
					switch (error.code) {
						case error.NOT_FOUND_ERR:
							deferred.reject('File not found!');
							break;
						case error.NOT_READABLE_ERR:
							deferred.reject('File not readable.');
							break;
						case error.ABORT_ERR:
							deferred.reject('File reading aborted.');
							break;
						default:
							deferred.reject('An error occurred while reading this file.');
					}
				};

				fr.onloadstart = loadingStarted;

				switch (fileType) {
					case 'docx':
						fr.onload = function (e) {
							var docxText = self._readDOCX(e.target.result);

							if (docxText) {
								if (/\S/.test(docxText)) {
									deferred.resolve(docxText);
								} else {
									deferred.reject('The selected DOCX file is empty.');
								}
							} else {
								deferred.reject('The selected file is not a valid DOCX file.');
							}
						};
						fr.readAsArrayBuffer(file);
						break;

					case 'odt':
						fr.onload = function (e) {
							var odtText = self._readODT(e.target.result);

							if (odtText) {
								if (/\S/.test(odtText)) {
									deferred.resolve(odtText);
								} else {
									deferred.reject('The selected ODT file is empty.');
								}
							} else {
								deferred.reject('The selected file is not a valid ODT file.');
							}
						};
						fr.readAsArrayBuffer(file);
						break;

					case 'txt':
						fr.onload = function (e) {
							var txtText = e.target.result;

							if (txtText) {
								if (/\S/.test(txtText)) {
									// Mac uses carriage return, which is not processed correctly
									// Replace each carriage return, not followed by a line feed
									// with a line feed
									var crCleanedText = txtText.replace(/\r(?!\n)/g, '\n');
									deferred.resolve(crCleanedText);
								} else {
									deferred.reject('The selected TXT file is empty.');
								}
							}
						};
						fr.readAsText(file);
						break;

					default:
						deferred.reject('File type not supported.');
				}

				return deferred.promise();
			};

			/**
			 * Traverses recursively all children starting from the top XML node,
			 * irrespective of how deep the nesting is.
			 * Returns their text contents as a string.
			 * @function
			 * @private
			 * @param   {Object} node       - the top XML node element
			 * @param   {String} tSelector  - the selector for text elements
			 * @param   {String} brSelector - the selector for soft line breaks
			 * @returns {String}            - the text content of the node
			 */
			FileInputReader.prototype._extractTextFromNode = function (node, tSelector, brSelector) {
				var self = this,
					// Paragraph selectors for both DOCX and ODT, 
					// supported both by Chrome and other browsers
					// Chrome uses different selectors 
					delimeters = {
						'w:p': '\n',
						'text:p': '\n',
						'p': '\n'
					},
					delimeter = delimeters[node.nodeName] || '',
					str = '';

				if (node.hasChildNodes()) {
					var child = node.firstChild;

					while (child) {
						// These selectors apply only to the footnotes of ODT files
						// Footnotes should appear all together at the end of the extracted text 
						// and not inside the text at the point where the reference is.
						if (child.nodeName === 'text:note' || child.nodeName === 'note') {
							child = child.nextSibling;
							continue;
						}

						if (child.nodeName === tSelector) {
							str += child.textContent;
						} else if (child.nodeName === brSelector) {
							str += '\n';
						}
						else {
							str += self._extractTextFromNode(child, tSelector, brSelector);
						}

						child = child.nextSibling;
					}
				}

				return str + delimeter;
			};

			/**
			 * Returns the type of file depending on the file's extension.
			 * @function
			 * @private
			 * @param   {Object} file - the file selected by the user
			 * @returns {String}      - the type of file
			 */
			FileInputReader.prototype._getFileType = function () {
				var self = this,
					file = self.file;

				if (/docx$/i.test(file.name)) {
					return 'docx';
				}

				if (/odt$/i.test(file.name)) {
					return 'odt';
				}

				if (/txt$/i.test(file.name)) {
					return 'txt';
				}

				return undefined;
			};

			/**
			 * Returns the contents of all XML nodes as a string.
			 * 
			 * @function
			 * @private
			 * @param   {Object[]} nodes    - the array of XML nodes
			 * @param   {String} tSelector  - the selector for text elements
			 * @param   {String} brSelector - the selector for soft line breaks
			 * @returns {String}            - the text content of all XML nodes
			 */
			FileInputReader.prototype._getTextContent = function (nodes, tSelector, brSelector) {
				var self = this,
					nLength = nodes.length,
					textContent;

				for (var i = 0; i < nLength; i++) {
					var node = nodes[i];
					var nodeContent = self._extractTextFromNode(node, tSelector, brSelector);
					textContent = [textContent, nodeContent].join('');
				}

				return textContent;
			};

			/**
			 * Returns the contents of the DOCX file as a string.
			 * @function
			 * @private
			 * @param   {Object} fileContents - the contents of the file object
			 * @returns {String}              - the text of the DOCX file
			 */
			FileInputReader.prototype._readDOCX = function (fileContents) {
				var self = this,
					document,
					footnotes = '',
					xmlDoc,
					tSelector = 'w:t',
					brSelector = 'w:br',
					zip;

				// Unzip the file
				try {
					zip = new JSZip(fileContents);

					// Read the main text of the DOCX file
					var file = zip.files['word/document.xml'];

					if (file) {
						xmlDoc = $.parseXML(file.asText());
						var pNodes = $(xmlDoc).find('w\\:body, body').children();
						document = self._getTextContent(pNodes, tSelector, brSelector);
					}

					// Read footnotes/endnotes
					if (!self.ignoreFootnotes) {
						// Read footnotes
						file = zip.files['word/footnotes.xml'];
						if (file) {
							xmlDoc = $.parseXML(file.asText());
							var fNodes = $(xmlDoc).find('w\\:footnotes, footnotes').children('w\\:footnote:not([w\\:type]), footnote:not([type])');
							var fNodesText = self._getTextContent(fNodes, tSelector, brSelector);
							if (fNodesText) {
								footnotes = [footnotes, fNodesText].join('');
							}
						}

						// Read endnotes
						file = zip.files['word/endnotes.xml'];
						if (file) {
							xmlDoc = $.parseXML(file.asText());
							var eNodes = $(xmlDoc).find('w\\:endnotes, endnotes').children('w\\:endnote:not([w\\:type]), endnote:not([type])');
							var eNodesText = self._getTextContent(eNodes, tSelector, brSelector);
							if (eNodesText) {
								footnotes = [footnotes, eNodesText].join('');
							}
						}

						if (footnotes && footnotes.length) {
							document = [document, 'FOOTNOTES', footnotes].join('\n');
						}
					}
				} catch (error) {

				}

				return document;
			};

			/**
			 * Returns the contents of the ODT file as a string.
			 * @function
			 * @private
			 * @param   {Object} fileContents - the contents of the file object
			 * @returns {String}              - the text of the ODT file
			 */
			FileInputReader.prototype._readODT = function (fileContents) {
				var self = this,
					document,
					tSelector = '#text',
					brSelector = 'text:line-break',
					zip;

				// Unzip the file
				try {
					zip = new JSZip(fileContents);

					// Read the main text, as well as the footnotes/endnotes of the ODT file
					var file = zip.files['content.xml'];

					if (file) {
						var xmlDoc = $.parseXML(file.asText());
						var pNodes = $(xmlDoc).find('office\\:body, body').children();
						document = self._getTextContent(pNodes, tSelector, brSelector);

						if (!self.ignoreFootnotes) {
							var fNodes = $(pNodes).find('text\\:note-body, note-body');
							var footnotes = self._getTextContent(fNodes, tSelector, brSelector);

							if (footnotes && footnotes.length) {
								document = [document, 'FOOTNOTES', footnotes].join('\n');
							}
						}
					}
				} catch (error) {

				}

				return document;
			};

			module.exports = FileInputReader;

		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, {}], 10: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var XRegExp = (typeof window !== "undefined" ? window['XRegExp'] : typeof global !== "undefined" ? global['XRegExp'] : null);

			/**
			 * Creates an instance of a {TextInputReader},
			 * which parses and extracts the text contents of the HTML text input.
			 * @constructor
			 * @this {TextInputReader}
			 */
			function TextInputReader() {
			}

			/**
			 * Returns a promise that handles the HTML input reading.
			 * When resolved, the contents of the HTML text
			 * are returned as a string. 
			 * @function
			 * @param   {String} text - the HTML text input
			 * @returns {Promise}
			 */
			TextInputReader.prototype.readTextInput = function (text) {
				var self = this,
					deferred = $.Deferred();

				var cleanedText = '';
				var div = document.createElement('div');
				div.innerHTML = text;

				var textNode = self._extractTextFromNode(div);
				// If is not empty or not contains only white spaces
				if (textNode.length && /\S/.test(textNode)) {
					cleanedText = [cleanedText, textNode].join('');
					// Remove multiple white spaces
					cleanedText = cleanedText.replace(/\n[ \t\v]*/g, '\n');
					// Remove multiple newlines
					cleanedText = cleanedText.replace(/\n{3,}/g, '\n\n');

					// Resolve
					deferred.resolve(cleanedText);
				} else {
					// Reject
					deferred.reject('HTML input has no valid text contents.');
				}

				return deferred.promise();
			};

			/**
			 * Traverses recursively all child nodes, 
			 * irrespective of how deep the nesting is.
			 * Returns the HTML text contents as a string.
			 * @function
			 * @private
			 * @param   {Object} node - the parent HTML node element
			 * @returns {String}      - the text content of the HTML string
			 */
			TextInputReader.prototype._extractTextFromNode = function (node) {
				var self = this,
					// Match any letter
					letterRegex = XRegExp('^\\pL+$'),
					str = '';

				// Returns whether a node should be skipped
				var isValidNode = function (nodeName) {
					var skipNodes = ['IFRAME', 'NOSCRIPT', 'SCRIPT', 'STYLE'],
						skipNodesLength = skipNodes.length;

					for (var i = 0; i < skipNodesLength; i++) {
						if (nodeName === skipNodes[i]) {
							return false;
						}
					}
					return true;
				};

				if (isValidNode(node.nodeName) && node.hasChildNodes()) {
					var child = node.firstChild;

					while (child) {
						// If text node
						if (child.nodeType === 3) {
							var content = child.textContent;
							if (content.length) {
								str += content;
							}
						} else {
							var extractedContent = self._extractTextFromNode(child);
							// Add a space between text nodes that are not separated 
							// by a space or newline (e.g. as in lists)
							if (letterRegex.test(str[str.length - 1]) && letterRegex.test(extractedContent[0])) {
								str += ' ';
							}
							str += extractedContent;
						}

						child = child.nextSibling;
					}
				}

				return str;
			};

			module.exports = TextInputReader;

		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, {}], 11: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var App = require('./app/app.js');

			// Main execution entry point
			$(window).load(function () {
				setTimeout(function () {
					$(".loader").addClass('shrinked');
					var app = new App('simtexter');
				}, 700);
			});

		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, { "./app/app.js": 1 }], 12: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Records a match found in the source and the target text.
		 * @constructor
		 * @this  {Match}
		 * @param {Number} srcTxtIdx     - the index of the source text 
		 * 																 in {SimTexter.texts[]}, where the match 
		 * 																 is found
		 * @param {Number} srcTkBeginPos - the index of the source text's token 
		 * 																 in {SimTexter.tokens[]}, where the match 
		 * 																 starts 
		 * @param {Number} trgTxtIdx     - the index of the target text
		 * 																 in {SimTexter.texts[]}, where the match 
		 * 																 is found
		 * @param {Number} trgTkBeginPos - the index of the target text's token 
		 * 																 in {SimTexter.tokens[]}, where the match 
		 * 																 starts
		 * @param {Number} matchLength   - the length of the match 
		 */
		function Match(srcTxtIdx, srcTkBeginPos, trgTxtIdx, trgTkBeginPos, matchLength) {
			this.srcTxtIdx = srcTxtIdx;
			this.srcTkBeginPos = srcTkBeginPos;
			this.trgTxtIdx = trgTxtIdx;
			this.trgTkBeginPos = trgTkBeginPos;
			this.matchLength = matchLength;
		}

		module.exports = Match;

	}, {}], 13: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Records a match found in a text.
		 * @constructor
		 * @this  {MatchSegment}
		 * @param {Number} txtIdx      - the index of the text in {SimTexter.texts[]},
		 * 															 where the match has been found
		 * @param {Number} tkBeginPos  - the index of the token in {SimTexter.tokens[]},
		 * 															 where the match starts 
		 * @param {Number} matchLength - the length of the match
		 */
		function MatchSegment(txtIdx, tkBeginPos, matchLength) {
			this.txtIdx = txtIdx;
			this.tkBeginPos = tkBeginPos;
			this.matchLength = matchLength;
			this.styleClass = undefined;
		}

		/**
		 * Returns the match's link node.
		 * @function
		 * @param {String}       text            - the text content of the node 
		 * @param {MatchSegment} trgMatchSegment - the target match segment
		 * @returns                              - the match's link node
		 */
		MatchSegment.prototype.createLinkNode = function (text, trgMatchSegment) {
			var self = this,
				matchLink = document.createElement('a');

			matchLink.id = [self.txtIdx + 1, '-', self.tkBeginPos].join('');
			matchLink.className = self.styleClass;
			matchLink.href = ['#', trgMatchSegment.txtIdx + 1, '-', trgMatchSegment.tkBeginPos].join('');
			matchLink.textContent = text;
			return matchLink;
		};

		/**
		 * Returns the index of the token in {SimTexter.tokens[]},
		 * where the match ends.
		 * @function
		 * @returns {Number} - the last token position of the match (non-inclusive)
		 */
		MatchSegment.prototype.getTkEndPosition = function () {
			var self = this;
			return self.tkBeginPos + self.matchLength;
		};

		/**
		 * Returns the index of the character in the input string,
		 * where the match starts.
		 * @function
		 * @returns {Number} - the first character of the match in the input string 
		 */
		MatchSegment.prototype.getTxtBeginPos = function (tokens) {
			var self = this;
			return tokens[self.tkBeginPos].txtBeginPos;
		};

		/**
		 * Returns the index of the character in the input string,
		 * where the match ends.
		 * @function
		 * @returns {Number} - the last character of the match in the input string 
		 */
		MatchSegment.prototype.getTxtEndPos = function (tokens) {
			var self = this;
			return tokens[self.tkBeginPos + self.matchLength - 1].txtEndPos;
		};

		/**
		 * Sets the style class of the match segment.
		 * @function
		 * @param {(Number|String)} n - the style class to be applied
		 */
		MatchSegment.prototype.setStyleClass = function (n) {
			var self = this;
			if (typeof n === 'number') {
				self.styleClass = ['hl-', n % 10].join('');
			}

			if (typeof n === 'string') {
				self.styleClass = n;
			}
		};

		module.exports = MatchSegment;

	}, {}], 14: [function (require, module, exports) {
		(function (global) {
			/* jshint undef:true, unused:true, node:true, browser:true */
			'use strict';

			var $ = (typeof window !== "undefined" ? window['$'] : typeof global !== "undefined" ? global['$'] : null);
			var XRegExp = (typeof window !== "undefined" ? window['XRegExp'] : typeof global !== "undefined" ? global['XRegExp'] : null);
			var Match = require('./match.js');
			var MatchSegment = require('./matchSegment.js');
			var Text = require('./text.js');
			var Token = require('./token.js');

			/**
			 * Creates an instance of {SimTexter}.
			 * @constructor
			 * @param {this}        SimTexter
			 * @param {Object}      storage   - the object that holds the app's settings
			 */
			function SimTexter(storage) {
				this.ignoreLetterCase = storage.getItemValueByKey('ignoreLetterCase');
				this.ignoreNumbers = storage.getItemValueByKey('ignoreNumbers');
				this.ignorePunctuation = storage.getItemValueByKey('ignorePunctuation');
				this.replaceUmlaut = storage.getItemValueByKey('replaceUmlaut');
				this.minMatchLength = storage.getItemValueByKey('minMatchLength');

				this.texts = [];
				this.tokens = [new Token()];
				this.uniqueMatches = 0;
			}

			/**
			 * Returns a promise that handles the comparison process.
			 * When resolved, an array of nodes is returned,
			 * which holds the text and the highlighted matches.
			 * @function
			 * @param {Array<MyInputText>} MyInputTexts - the array of {MyInputText} objects 
			 *                                        which hold information about the user 
			 * 																 				input
			 */
			SimTexter.prototype.compare = function (MyInputTexts) {
				var self = this,
					deferred = $.Deferred(),
					forwardReferences = [],
					similarities = [];

				// Read input (i.e. cleaning, tokenization)
				self._readInput(MyInputTexts, forwardReferences);
				// Get matches
				similarities = self._getSimilarities(0, 1, forwardReferences);

				if (similarities.length) {
					// Return input string as HTML nodes
					deferred.resolve(self._getNodes(MyInputTexts, similarities));
				} else {
					deferred.reject('No similarities found.');
				}

				return deferred.promise();
			};

			/**
			 * Applies a style class to each match segment
			 * and removes duplicates from the array of matches.
			 * Duplicates or overlapping segments can be traced,
			 * if one observes the target {MatchSegment} objects 
			 * stored in the array matches.
			 * Sorting of matches by target {MatchSegment}, 
			 * with its tkBeginPos in ascending order 
			 * and its matchLength in descending order,
			 * makes removal of duplicates easy to handle.
			 * The first {MatchSegment} with a given tkBeginPos
			 * has the longest length. All others with the same tkBeginPos
			 * have the same or a smaller length, and thus can be discarded.
			 * @function
			 * @private
			 * @param   {Array} matches - the array that holds the match segments, 
			 * 														stored in pairs
			 * @returns {Array}         - the array of unique matches
			 */
			SimTexter.prototype._applyStyles = function (matches) {
				var self = this;

				// Sort matches by target {MatchSegment},
				// where tkBeginPos in ascending order and matchLength in descending order
				var sortedMatches = self._sortSimilarities(matches, 1);
				var sortedMatchesLength = sortedMatches.length;
				var styleClassCnt = 1;

				// Add first match in array of unique matches to have a starting point
				var uniqueMatch = [sortedMatches[0][0], sortedMatches[0][1]];
				uniqueMatch[0].setStyleClass(0);
				uniqueMatch[1].setStyleClass(0);
				var aUniqueMatches = [uniqueMatch];

				// For each match in sortedMatches[]
				for (var i = 1; i < sortedMatchesLength; i++) {
					var lastUniqueMatch = aUniqueMatches[aUniqueMatches.length - 1][1];
					var match = sortedMatches[i][1];

					// If not duplicate
					if (lastUniqueMatch.tkBeginPos != match.tkBeginPos) {
						// if not overlapping
						if (lastUniqueMatch.getTkEndPosition() - 1 < match.tkBeginPos) {
							uniqueMatch = [sortedMatches[i][0], sortedMatches[i][1]];
							uniqueMatch[0].setStyleClass(styleClassCnt);
							uniqueMatch[1].setStyleClass(styleClassCnt);
							aUniqueMatches.push(uniqueMatch);
							styleClassCnt++;
						} else {
							// end-to-start overlapping
							// end of lastUniqueMatch overlaps with start of match
							if (lastUniqueMatch.getTkEndPosition() < match.getTkEndPosition()) {
								var styleClass = (/overlapping$/.test(lastUniqueMatch.styleClass)) ? lastUniqueMatch.styleClass : lastUniqueMatch.styleClass + ' overlapping';
								// Overwrite the style of the last unique match segment 
								// and change its length accordingly
								aUniqueMatches[aUniqueMatches.length - 1][0].setStyleClass(styleClass);
								aUniqueMatches[aUniqueMatches.length - 1][1].setStyleClass(styleClass);
								aUniqueMatches[aUniqueMatches.length - 1][1].matchLength = match.tkBeginPos - lastUniqueMatch.tkBeginPos;

								// Add the new match segment
								uniqueMatch = [sortedMatches[i][0], sortedMatches[i][1]];
								uniqueMatch[0].setStyleClass(styleClass);
								uniqueMatch[1].setStyleClass(styleClass);
								aUniqueMatches.push(uniqueMatch);
							}
						}
					}
				}

				self.uniqueMatches = aUniqueMatches.length;
				return aUniqueMatches;
			};

			/**
			 * Returns a regular expression depending on the comparison options set.
			 * Uses the XRegExp category patterns.
			 * @function
			 * @private
			 * @returns {XRegExp} - the regular expression
			 */
			SimTexter.prototype._buildRegex = function () {
				var self = this,
					// XRegExp patterns
					NUMBERS = '\\p{N}',
					PUNCTUATION = '\\p{P}',
					regex = '';

				if (self.ignoreNumbers) {
					regex += NUMBERS;
				}

				if (self.ignorePunctuation) {
					regex += PUNCTUATION;
				}

				return (regex.length > 0) ? XRegExp('[' + regex + ']', 'g') : undefined;
			};

			/**
			 * Cleans the input string according to the comparison options set.
			 * @function
			 * @private
			 * @param   {String} MyInputText - the input string
			 * @returns {String}           - the cleaned input string
			 */
			SimTexter.prototype._cleanMyInputText = function (MyInputText) {
				var self = this,
					text = MyInputText;

				var langRegex = self._buildRegex();

				if (langRegex) {
					text = MyInputText.replace(langRegex, ' ');
				}

				if (self.ignoreLetterCase) {
					text = text.toLowerCase();
				}

				return text;
			};

			/**
			 * Returns a "cleaned" word, according to the comparison options set.
			 * @function
			 * @private
			 * @param   {String} word - a sequence of characters, separated by one 
			 *                          or more white space characters (space, tab, newline)
			 * @returns {String}      - the cleaned word
			 */
			SimTexter.prototype._cleanWord = function (word) {
				var self = this,
					umlautRules = {
						'ä': 'ae',
						'ö': 'oe',
						'ü': 'ue',
						'ß': 'ss',
						'æ': 'ae',
						'œ': 'oe',
						'Ä': 'AE',
						'Ö': 'OE',
						'Ü': 'UE',
						'Æ': 'AE',
						'Œ': 'OE'
					},
					token = word;

				if (self.replaceUmlaut) {
					token = word.replace(/ä|ö|ü|ß|æ|œ|Ä|Ö|Ü|Æ|Œ/g, function (key) {
						return umlautRules[key];
					});
				}

				return token;
			};

			/**
			 * Finds the longest common substring in the source and the target text
			 * and returns the best match.
			 * @function
			 * @private
			 * @param   {Number} srcTxtIdx     - the index of the source text in texts[] 
			 *                                   to be compared
			 * @param   {Number} trgTxtIdx     - the index of the target text in texts[] 
			 *                                   to be compared
			 * @param   {Number} srcTkBeginPos - the index of the token in tokens[] 
			 *                                   at which the comparison should start
			 * @param   {Array}  frwReferences - the array of forward references
			 * @returns {Match}                - the best match
			 */
			SimTexter.prototype._getBestMatch = function (srcTxtIdx, trgTxtIdx, srcTkBeginPos, frwReferences) {
				var self = this,
					bestMatch,
					bestMatchTkPos,
					bestMatchLength = 0,
					srcTkPos = 0,
					trgTkPos = 0;

				for (var tkPos = srcTkBeginPos;
					(tkPos > 0) && (tkPos < self.tokens.length);
					tkPos = frwReferences[tkPos]) {

					// If token not within the range of the target text  
					if (tkPos < self.texts[trgTxtIdx].tkBeginPos) {
						continue;
					}

					var minMatchLength = (bestMatchLength > 0) ? bestMatchLength + 1 : self.minMatchLength;

					srcTkPos = srcTkBeginPos + minMatchLength - 1;
					trgTkPos = tkPos + minMatchLength - 1;

					// Compare backwards
					if (srcTkPos < self.texts[srcTxtIdx].tkEndPos &&
						trgTkPos < self.texts[trgTxtIdx].tkEndPos &&
						(srcTkPos + minMatchLength) <= trgTkPos) { // check if they overlap
						var cnt = minMatchLength;

						while (cnt > 0 && self.tokens[srcTkPos].text === self.tokens[trgTkPos].text) {
							srcTkPos--;
							trgTkPos--;
							cnt--;
						}

						if (cnt > 0) {
							continue;
						}
					} else {
						continue;
					}

					// Compare forwards
					var newMatchLength = minMatchLength;
					srcTkPos = srcTkBeginPos + minMatchLength;
					trgTkPos = tkPos + minMatchLength;

					while (srcTkPos < self.texts[srcTxtIdx].tkEndPos &&
						trgTkPos < self.texts[trgTxtIdx].tkEndPos &&
						(srcTkPos + newMatchLength) < trgTkPos && // check if they overlap
						self.tokens[srcTkPos].text === self.tokens[trgTkPos].text) {
						srcTkPos++;
						trgTkPos++;
						newMatchLength++;
					}

					// Record match
					if (newMatchLength >= self.minMatchLength && newMatchLength > bestMatchLength) {
						bestMatchLength = newMatchLength;
						bestMatchTkPos = tkPos;
						bestMatch = new Match(srcTxtIdx, srcTkBeginPos, trgTxtIdx, bestMatchTkPos, bestMatchLength);
					}
				}

				return bestMatch;
			};

			/**
			 * Returns an array of HTML nodes, containing the whole text, 
			 * together with the hightlighted matches.
			 * The text content of each node is retrieved by slicing the input text
			 * at the first (txtBeginPos) and the last (txtEndPos) character position 
			 * of each match.
			 * @function
			 * @private
			 * @param   {Array} MyInputTexts - the array of {MyInputText} objects, 
			 * 															 which hold information about each user input
			 * @param   {Array} matches    - the array that holds the {MatchSegment} objects, 
			 * 															 stored in pairs
			 * @returns {Array}            - the array of HTML nodes, 
			 * 															 which holds the text and the highlighted matches
			 */
			SimTexter.prototype._getNodes = function (MyInputTexts, matches) {
				var self = this,
					iTextsLength = MyInputTexts.length,
					nodes = [];

				var styledMatches = self._applyStyles(matches);

				// For each input text
				for (var i = 0; i < iTextsLength; i++) {
					var MyInputText = MyInputTexts[i].text,
						chIdx = 0,
						chIdxLast = chIdx,
						chEndPos = MyInputText.length,
						mIdx = 0,
						trgIdxRef = (i == 0) ? (i + 1) : (i - 1);
					nodes[i] = [];

					// Sort array of similarities
					var sortedMatches = self._sortSimilarities(styledMatches, i);

					// For each character position in input text
					while (chIdx <= chEndPos) {
						if (sortedMatches.length && mIdx < sortedMatches.length) {
							var match = sortedMatches[mIdx][i];
							// Get start character position of match segment
							var mTxtBeginPos = match.getTxtBeginPos(self.tokens);
							// Get end character position of match segment
							var mTxtEndPos = match.getTxtEndPos(self.tokens);

							// Create text node
							var textNodeStr = MyInputText.slice(chIdxLast, mTxtBeginPos);
							var textNode = document.createTextNode(textNodeStr);
							nodes[i].push(textNode);

							// Create link node for match segment
							var linkNodeStr = MyInputText.slice(mTxtBeginPos, mTxtEndPos);
							var linkNode = match.createLinkNode(linkNodeStr, sortedMatches[mIdx][trgIdxRef]);
							nodes[i].push(linkNode);

							mIdx++;
							chIdx = mTxtEndPos;
							chIdxLast = chIdx;
						} else {
							var lastTextNodeStr = MyInputText.slice(chIdxLast, chEndPos);
							var lastTextNode = document.createTextNode(lastTextNodeStr);
							nodes[i].push(lastTextNode);
							chIdx = chEndPos;
							break;
						}
						chIdx++;
					}
				}

				return nodes;
			};

			/**
			 * Returns an array of matches,
			 * where each match is an array of two {MatchSegment} objects, stored in pairs.
			 * At index 0, the source {MatchSegment} object is stored,
			 * and at index 1, the target {MatchSegment} object.
			 * @function
			 * @param   {Number} srcTxtIdx     - the index of the source {Text} object 
			 * 																	 in texts[] to be compared
			 * @param   {Number} trgTxtIdx     - the index of the target {Text} object 
			 * 																	 in texts[] to be compared
			 * @param   {Array}  frwReferences - the array of forward references
			 * @returns {Array}                - the array that holds the {MatchSegment} 
			 * 																 	 objects, stored in pairs
			 */
			SimTexter.prototype._getSimilarities = function (srcTxtIdx, trgTxtIdx, frwReferences) {
				var self = this,
					similarities = [],
					srcTkPos = self.texts[srcTxtIdx].tkBeginPos,
					srcTkEndPos = self.texts[srcTxtIdx].tkEndPos;

				while ((srcTkPos + self.minMatchLength) <= srcTkEndPos) {
					var bestMatch = self._getBestMatch(srcTxtIdx, trgTxtIdx, srcTkPos, frwReferences);

					if (bestMatch && bestMatch.matchLength > 0) {
						similarities.push([
							new MatchSegment(bestMatch.srcTxtIdx, bestMatch.srcTkBeginPos, bestMatch.matchLength),
							new MatchSegment(bestMatch.trgTxtIdx, bestMatch.trgTkBeginPos, bestMatch.matchLength)
						]);
						srcTkPos += bestMatch.matchLength;
					} else {
						srcTkPos++;
					}
				}

				return similarities;
			};

			/**
			 * Creates the forward reference table.
			 * @function
			 * @private
			 * @param {Text}   text          - a {Text} object
			 * @param {Array}  frwReferences - the array of forward references 
			 * @param {Object} mtsTags       - the hash table of minMatchLength 
			 * 																 sequence of tokens (MTS)
			 */
			SimTexter.prototype._makeForwardReferences = function (text, frwReferences, mtsTags) {
				var self = this,
					txtBeginPos = text.tkBeginPos,
					txtEndPos = text.tkEndPos;

				// For each token in tokens[]
				for (var i = txtBeginPos; (i + self.minMatchLength - 1) < txtEndPos; i++) {
					// Concatenate tokens of minimum match length
					var tag = self.tokens.slice(i, i + self.minMatchLength).map(function (token) {
						return token.text;
					}).join('');

					// If hash table contains tag
					if (tag in mtsTags) {
						// Store current token position at index mtsTags[tag]
						frwReferences[mtsTags[tag]] = i;
					}
					// Add tag to hash table and assign current token position to it
					mtsTags[tag] = i;
				}
			};

			/**
			 * Reads the input string, and initializes texts[] and tokens[].
			 * Creates also the forward reference table.
			 * @function
			 * @private
			 * @param {Array} MyInputTexts    - the array of {MyInputText} objects
			 * 															  that hold information on the user input
			 * @param {Array} frwReferences - the array of forward references
			 */
			SimTexter.prototype._readInput = function (MyInputTexts, frwReferences) {
				var self = this,
					mtsHashTable = {},
					iLength = MyInputTexts.length;

				for (var i = 0; i < iLength; i++) {
					var MyInputText = MyInputTexts[i];
					// Compute text's words
					var nrOfWords = MyInputText.text.match(/[^\s]+/g).length;
					// Initialize texts[]
					self.texts.push(new Text(MyInputText.mode, MyInputText.text.length, nrOfWords, MyInputText.fileName, self.tokens.length));
					// Initialize tokens[]
					self._tokenizeInput(MyInputText.text);
					// Update text's last token position
					self.texts[i].tkEndPos = self.tokens.length;
					// Create array of forward references
					self._makeForwardReferences(self.texts[i], frwReferences, mtsHashTable);
				}
			};

			/**
			 * Sorts matches by source or target {MatchSegment},
			 * depending on the idx value.
			 * @function
			 * @private
			 * @param   {Array}  matches - the array of matches to be sorted
			 * @param   {Number} idx     - the index of the array of 
			 * 														 the {MatchSegment} objects
			 * @returns {Array}          - the sorted array of matches
			 */
			SimTexter.prototype._sortSimilarities = function (matches, idx) {
				var sortedSims = matches.slice(0);

				sortedSims.sort(function (a, b) {
					var pos = a[idx].tkBeginPos - b[idx].tkBeginPos;
					if (pos) {
						return pos;
					}
					return b[idx].matchLength - a[idx].matchLength;
				});

				return sortedSims;
			};

			/**
			 * Tokenizes the input string.
			 * @param {Object} MyInputText - the input string to be tokenized
			 */
			SimTexter.prototype._tokenizeInput = function (MyInputText) {
				var self = this,
					wordRegex = /[^\s]+/g,
					match;

				var cleanedText = self._cleanMyInputText(MyInputText);

				while (match = wordRegex.exec(cleanedText)) {
					var word = match[0];
					var token = self._cleanWord(word);

					if (token.length > 0) {
						var txtBeginPos = match.index;
						var txtEndPos = match.index + word.length;
						// Add token to tokens[]
						self.tokens.push(new Token(token, txtBeginPos, txtEndPos));
					}
				}
			};

			module.exports = SimTexter;

		}).call(this, typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})

	}, { "./match.js": 12, "./matchSegment.js": 13, "./text.js": 15, "./token.js": 16 }], 15: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {Text},
		 * which holds information on the input string.
		 * @constructor
		 * @this  {Text}
		 * @param {String} inputMode      - the mode of the input (i.e. 'File' 
		 * 																 	or 'Text')
		 * @param {Number} nrOfCharacters - the total number of characters 
		 * 																 	of the input string
		 * @param {Number} nrOfWords      - the total number of words 
		 * 																 	of the input string
		 * @param {String} fileName       - the name of the file
		 * @param {Number} tkBeginPos     - the index (inclusive) of the token
		 * 																	in {SimTexter.tokens[]}, at which 
		 * 																 	the input string starts 
		 * @param {Number} tkEndPos       - the index (non-inclusive) of the token
		 * 																  in {SimTexter.tokens[]}, at which 
		 * 																 	the input string ends 
		 */
		function Text(inputMode, nrOfCharacters, nrOfWords, fileName, tkBeginPos, tkEndPos) {
			this.inputMode = inputMode;
			this.fileName = fileName;
			this.tkBeginPos = tkBeginPos || 0;
			this.tkEndPos = tkEndPos || 0;
			this.nrOfCharacters = nrOfCharacters || 0;
			this.nrOfWords = nrOfWords || 0;
		}

		module.exports = Text;

	}, {}], 16: [function (require, module, exports) {
		/* jshint undef:true, unused:true, node:true, browser:true */
		'use strict';

		/**
		 * Creates an instance of a {Token}.
		 * A {Token} records the starting and ending character position 
		 * of a word in the input string, to facilitate reconstruction of the input
		 * during output of the comparison results.
		 * A word is a sequence of characters, 
		 * separated by one or more whitespaces or newlines.
		 * The text of the {Token} corresponds to the "cleaned" version of a word. 
		 * All characters, as defined by the comparison options set by the user,
		 * are removed/replaced from the token's text.
		 * @constructor
		 * @this  {Token}
		 * @param {String} text        - the text of the word after being "cleaned" 
		 *                               according to the comparison options 
		 *                               set by the user 
		 * @param {Number} txtBeginPos - the index of the word's first character 
		 * 															 (inclusive) in the input string
		 * @param {Number} txtEndPos   - the index of the word's last character 
		 * 															 (non-inclusive) in the input string
		 */
		function Token(text, txtBeginPos, txtEndPos) {
			this.text = text || '';
			this.txtBeginPos = txtBeginPos || 0;
			this.txtEndPos = txtEndPos || 0;
		}

		module.exports = Token;

	}, {}]
}, {}, [11])