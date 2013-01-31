//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/2-AddFlyoutToCharm.html", {
        ready: function (element, options) {
            document.getElementById("scenario2Add").addEventListener("click", scenario2AddSettingsFlyout, false);
            document.getElementById("scenario2Show").addEventListener("click", showSettingsCharm, false);
        }
    });

    function scenario2AddSettingsFlyout() {
        // TODO: 1 - When the page is loaded, add the settings commands contextual to the page.
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = { "helpDiv": { title: "Help", href: "/html/2-SettingsFlyout-Help.html" } };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Help command and settings flyout added from 2-SettingsFlyout-Help.html", "App settings SDK sample", "info");
    }

    function showSettingsCharm() {
        
        // TODO: 3 : Show the settings charm if not in snapped view
        if (Windows.UI.ViewManagement.ApplicationView.value !== Windows.UI.ViewManagement.ApplicationViewState.snapped) {
            Windows.UI.ApplicationSettings.SettingsPane.show();
        }

    }

})();
