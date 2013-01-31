//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/5-ProgrammaticInvocation.html", {
        ready: function (element, options) {
            document.getElementById("scenario5Add").addEventListener("click", scenario5AddSettingsFlyout, false);
            document.getElementById("scenario5Show").addEventListener("click", scenario5ShowSettingsFlyout, false);
        }
    });

    function scenario5AddSettingsFlyout() {
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = { "settingsDiv": { title: "Settings", href: "/html/5-SettingsFlyout-Settings.html" } };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Settings flyout added from 5-SettingsFlyout-Setting.html", "App settings SDK sample", "info");
    }

    function scenario5ShowSettingsFlyout() {
        WinJS.UI.SettingsFlyout.showSettings("settingsDiv", "/html/5-SettingsFlyout-Settings.html");

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Settings flyout showing", "App settings SDK sample", "info");
    }
})();
