﻿//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// This file is a part of the SDK sample framework. For code demonstrating scenarios in this particular sample,
// please see the html, css and js folders.

(function () {

    //
    // Helper controls used in the sample pages
    //

    // The ScenarioInput control inserts the appropriate markup to get labels & controls
    // hooked into the input section of a scenario page so that it's not repeated in
    // every one.

    var ScenarioInput = WinJS.Class.define(
        function (element, options) {
        element.winControl = this;
        this.element = element;

        new WinJS.Utilities.QueryCollection(element)
                    .setAttribute("role", "main")
                    .setAttribute("aria-labelledby", "inputLabel");
        element.id = "input";

        this.addInputLabel(element);
        this.addDetailsElement(element);
        this.addScenariosPicker(element);
    }, {
        addInputLabel: function (element) {
            var label = document.createElement("h2");
            label.textContent = "Input";
            label.id = "inputLabel";
            element.parentNode.insertBefore(label, element);
        },
        addScenariosPicker: function (parentElement) {
            var scenarios = document.createElement("div");
            scenarios.id = "scenarios";
            var control = new ScenarioSelect(scenarios);

            parentElement.insertBefore(scenarios, parentElement.childNodes[0]);
        },

        addDetailsElement: function (sourceElement) {
            var detailsDiv = this._createDetailsDiv();
            while (sourceElement.childNodes.length > 0) {
                detailsDiv.appendChild(sourceElement.removeChild(sourceElement.childNodes[0]));
            }
            sourceElement.appendChild(detailsDiv);
        },
        _createDetailsDiv: function () {
            var detailsDiv = document.createElement("div");

            new WinJS.Utilities.QueryCollection(detailsDiv)
                        .addClass("details")
                        .setAttribute("role", "region")
                        .setAttribute("aria-labelledby", "descLabel")
                        .setAttribute("aria-live", "assertive");

            var label = document.createElement("h3");
            label.textContent = "Description";
            label.id = "descLabel";

            detailsDiv.appendChild(label);
            return detailsDiv;
        },
    }
    );

    // The ScenarioOutput control inserts the appropriate markup to get labels & controls
    // hooked into the output section of a scenario page so that it's not repeated in
    // every one.

    var ScenarioOutput = WinJS.Class.define(
        function (element, options) {
        element.winControl = this;
        this.element = element;
        new WinJS.Utilities.QueryCollection(element)
                    .setAttribute("role", "region")
                    .setAttribute("aria-labelledby", "outputLabel")
                    .setAttribute("aria-live", "assertive");
        element.id = "output";

        this._addOutputLabel(element);
        this._addStatusOutput(element);
    }, {
        _addOutputLabel: function (element) {
            var label = document.createElement("h2");
            label.id = "outputLabel";
            label.textContent = "Output";
            element.parentNode.insertBefore(label, element);
        },
        _addStatusOutput: function (element) {
            var statusDiv = document.createElement("div");
            statusDiv.id = "statusMessage";
            element.insertBefore(statusDiv, element.childNodes[0]);
        }
    }
    );


    // Sample infrastructure internals

    var currentScenarioUrl = null;

    WinJS.Navigation.addEventListener("navigating", function (evt) {
        currentScenarioUrl = evt.detail.location;
    });

    WinJS.log = function (message, tag, type) {
        var statusDiv = /* @type(HTMLElement) */ document.getElementById("statusMessage");
        var isError = type === "error";
        if (statusDiv) {
            statusDiv.innerText = message;
            if (isError) {
                statusDiv.style.color = "blue";
            } else {
                statusDiv.style.color = "green";
            }
        }
    };

    // Control that populates and runs the scenario selector

    var ScenarioSelect = WinJS.UI.Pages.define("/sample-utils/scenario-select.html", {
        ready: function (element, options) {
            var that = this;
            var selectElement = WinJS.Utilities.query("#scenarioSelect", element);
            this._selectElement = selectElement[0];

            SdkSample.scenarios.forEach(function (s, index) {
                that._addScenario(index, s);
            });

            selectElement.listen("change", function (evt) {
                var select = evt.target;
                var newUrl = select.options[select.selectedIndex].value;
                WinJS.Navigation.navigate(newUrl).then(function () {
                    msSetImmediate(function () {
                        document.getElementById("scenarioSelect").setActive();
                    });
                });
            });
            selectElement[0].size = (SdkSample.scenarios.length > 5 ? 5 : SdkSample.scenarios.length);
        },

        _addScenario: function (index, info) {
            var option = document.createElement("option");
            if (info.url === currentScenarioUrl) {
                option.selected = "selected";
            }
            option.text = (index + 1) + ") " + info.title;
            option.value = info.url;
            this._selectElement.appendChild(option);
        }
    });

    function activated(e) {
        WinJS.Utilities.query("#featureLabel")[0].textContent = SdkSample.sampleTitle;
    }

    WinJS.Application.addEventListener("activated", activated, false);

    // Export public methods & controls
    WinJS.Namespace.define("SdkSample", {
        ScenarioInput: ScenarioInput,
        ScenarioOutput: ScenarioOutput
    });
})();
