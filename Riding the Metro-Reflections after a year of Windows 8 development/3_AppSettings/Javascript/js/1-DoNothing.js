//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/1-DoNothing.html", {
        ready: function (element, options) {
            document.getElementById("scenario1Show").addEventListener("click", scenario1ShowSettingsCharm, false);
        }
    });

    function scenario1ShowSettingsCharm() {
        // Ensure no settings commands are specified in the settings charm in this scenario
        WinJS.Application.onsettings = function (e) { };

        // Show the settings charm if not in snapped view
        if (Windows.UI.ViewManagement.ApplicationView.value !== Windows.UI.ViewManagement.ApplicationViewState.snapped) {
            Windows.UI.ApplicationSettings.SettingsPane.show();
        }
    }
})();
