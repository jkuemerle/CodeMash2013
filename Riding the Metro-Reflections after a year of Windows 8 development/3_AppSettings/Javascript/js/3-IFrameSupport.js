//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3-IFrameSupport.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Add").addEventListener("click", scenario3AddSettingsFlyout, false);
            document.getElementById("scenario3Show").addEventListener("click", showSettingsCharm, false);
        }
    });

    function scenario3AddSettingsFlyout() {
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = { "legalNoticesDiv": { title: "Legal notices", href: "/html/3-SettingsFlyout-Legal.html" } };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Legal notices command and settings flyout added from 3-SettingsFlyout-Legal.html", "App settings SDK sample", "info");
    }

    function showSettingsCharm() {
        // Show the settings charm if not in snapped view
        if (Windows.UI.ViewManagement.ApplicationView.value !== Windows.UI.ViewManagement.ApplicationViewState.snapped) {
            Windows.UI.ApplicationSettings.SettingsPane.show();
        }
    }

})();
