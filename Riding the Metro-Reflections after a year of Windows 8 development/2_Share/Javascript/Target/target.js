//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    // Variable to store the ShareOperation object
    var shareOperation = null;

    // Variable to store the visibility of the Extended Sharing section
    var extendedSharingCollapsed = true;

    function id(elementId) {
        return document.getElementById(elementId);
    }

    // Variable to store the custom format string
    // TODO: Swap out "Windows-8-Preview-Book" for "http://schema.org/Book". The Beta manifest schema is unable to support URLs as data formats
    var customFormatName = "Windows-8-Preview-Book";

    /// <summary>
    /// Helper function to display received sharing content
    /// </summary>
    /// <param name="type">
    /// The type of content received
    /// </param>
    /// <param name="value">
    /// The value of the content
    /// </param>
    function displayContent(label, content, preformatted) {
        var labelNode = document.createElement("strong");
        labelNode.innerText = label;

        id("contentValue").appendChild(labelNode);

        if (preformatted) {
            var pre = document.createElement("pre");
            pre.innerHTML = content;
            id("contentValue").appendChild(pre);
        }
        else {
            id("contentValue").appendChild(document.createTextNode(content));
        }
        id("contentValue").appendChild(document.createElement("br"));
    }

    /// <summary>
    /// Handler executed on activation of the target
    /// </summary>
    /// <param name="eventArgs">
    /// Arguments of the event. In the case of the Share contract, it has the ShareOperation
    /// </param>
    function activatedHandler(eventArgs) {
        // TODO: 3 - In this sample we only do something if it was activated with the Share contract
        if (eventArgs.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.shareTarget) {
            // We receive the ShareOperation object as part of the eventArgs
            shareOperation = eventArgs.detail.shareOperation;
            id("title").innerText = shareOperation.data.properties.title;
            id("description").innerText = shareOperation.data.properties.description;
            // If this app was activated via a QuickLink, display the QuickLinkId
            if (shareOperation.quickLinkId !== "") {
                id("selectedQuickLinkId").innerText = shareOperation.quickLinkId;
                id("quickLinkArea").className = "hidden";
            }
            // Display a thumbnail if available
            if (shareOperation.data.properties.thumbnail) {
                shareOperation.data.properties.thumbnail.openReadAsync().then(function (thumbnailStream) {
                    var thumbnailBlob = MSApp.createBlobFromRandomAccessStream(thumbnailStream.contentType, thumbnailStream);
                    var thumbnailUrl = URL.createObjectURL(thumbnailBlob, false);
                    id("thumbnailImage").src = thumbnailUrl;
                    id("thumbnailArea").className = "unhidden";
                });
            }
            // Display the data received based on data type
            if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.text)) {
                shareOperation.data.getTextAsync().then(function (text) {
                    if( text !== null ) {
                        displayContent("Text: ", text, false);
                    }
                });
            }
            if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.uri)) {
                shareOperation.data.getUriAsync().then(function (uri) {
                    if (uri !== null) {
                        displayContent("Uri: ", uri.absoluteUri, false);
                    }
                });
            }
            if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.storageItems)) {
                shareOperation.data.getStorageItemsAsync(Windows.ApplicationModel.DataTransfer.StandardDataFormats.storageItems).then(function (storageItems) {
                    var fileList = "";
                    for (var i = 0; i < storageItems.size; i++) {
                        fileList += storageItems.getAt(i).name;
                        if (i < storageItems.size - 1) {
                            fileList += ", ";
                        }
                    }
                    displayContent("StorageItems: ", fileList, false);
                });
            }
            if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.bitmap)) {
                shareOperation.data.getBitmapAsync().then(function (streamRef) {
                    streamRef.openReadAsync().then(function (bitmapStream) {
                        if (bitmapStream) {
                            var blob = MSApp.createBlobFromRandomAccessStream(bitmapStream.contentType, bitmapStream);
                            id("imageHolder").src = URL.createObjectURL(blob, false);
                            id("imageArea").className = "unhidden";
                        }
                    });
                });
            }

            if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.html)) {
                shareOperation.data.getHtmlFormatAsync().then(function (htmlFormat) {
                    id("htmlContentArea").className = "unhidden";

                    // Extract the HTML fragment from the HTML format
                    var htmlFragment = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.getStaticFragment(htmlFormat);

                    // Set the innerHTML of the iframe to the HTML fragment
                    var iFrame = id("htmlContent");
                    iFrame.style.display = "";
                    iFrame.contentDocument.documentElement.innerHTML = htmlFragment;

                    // Now we loop through any images and use the resourceMap to map each image element's src
                    var images = iFrame.contentDocument.documentElement.getElementsByTagName("img");
                    var blob = null;
                    if (images.length > 0) {
                        shareOperation.data.getResourceMapAsync().then(function (resourceMap) {
                            for (var i = 0, len = images.length; i < len; i++) {
                                var streamReference = resourceMap[images[i].getAttribute("src")];
                                if (streamReference) {
                                    streamReference.openReadAsync().then(function (imageStream) {
                                        if (imageStream) {
                                            blob = MSApp.createBlobFromRandomAccessStream(imageStream.contentType, imageStream);
                                            images[i].src = URL.createObjectURL(blob, false);
                                        }
                                    });
                                }
                            }
                        });

                    }
                });
            }

            if (shareOperation.data.contains(customFormatName)) {
                shareOperation.data.getTextAsync(customFormatName).then(function (customFormatString) {

                    var customFormatObject = JSON.parse(customFormatString);

                    if (customFormatObject) {
                        // This sample expects the custom format to be of type http://schema.org/Book
                        if (customFormatObject.type === "http://schema.org/Book") {
                            customFormatString = "Type: " + customFormatObject.type;
                            if (customFormatObject.properties) {
                                customFormatString += "\nImage: " + customFormatObject.properties.image
                                                    + "\nName: " + customFormatObject.properties.name
                                                    + "\nBook Format: " + customFormatObject.properties.bookFormat
                                                    + "\nAuthor: " + customFormatObject.properties.author
                                                    + "\nNumber of Pages: " + customFormatObject.properties.numberOfPages
                                                    + "\nPublisher: " + customFormatObject.properties.publisher
                                                    + "\nDate Published: " + customFormatObject.properties.datePublished
                                                    + "\nIn Language: " + customFormatObject.properties.inLanguage
                                                    + "\nISBN: " + customFormatObject.properties.isbn;
                            }
                        }
                    }

                    displayContent("Custom data: ", customFormatString, true);

                });
            }
        }
    }

    /// <summary>
    /// Use to simulate that an extended share operation has started
    /// </summary>
    function reportStarted() {
        shareOperation.reportStarted();
    }

    /// <summary>
    /// Use to simulate that an extended share operation has retrieved the data
    /// </summary>
    function reportDataRetrieved() {
        shareOperation.reportDataRetrieved();
    }

    /// <summary>
    /// Use to simulate that an extended share operation has reached the status "submittedToBackgroundManager"
    /// </summary>
    function reportSubmittedBackgroundTask() {
        shareOperation.reportSubmittedBackgroundTask();
    }

    /// <summary>
    /// Submit for extended share operations. Can either report success or failure, and in case of success, can add a quicklink.
    /// This does NOT take care of all the prerequisites (such as calling reportExtendedShareStatus(started)) before submitting.
    /// </summary>
    function reportError() {
        var errorText = id("extendedShareErrorMessage").value;
        shareOperation.reportError(errorText);
        id("appBody").innerHTML = "<p>The sharing operation was reported with error:" + errorText + "</p><br/>";
    }

    /// <summary>
    /// Call the reportCompleted API with the proper quicklink (if needed)
    /// </summary>
    function reportCompleted() {
        var addQuickLink = id("addQuickLink").checked;
        if (addQuickLink) {
            var quickLink = new Windows.ApplicationModel.DataTransfer.ShareTarget.QuickLink();
            quickLink.title = id("quickLinkTitle").value;
            quickLink.id = id("quickLinkId").value;
            var dataFormats = Windows.ApplicationModel.DataTransfer.StandardDataFormats;
            // For quicklinks, the supported FileTypes and DataFormats are set independently from the manifest
            quickLink.supportedFileTypes.replaceAll(["*"]);
            quickLink.supportedDataFormats.replaceAll([dataFormats.text, dataFormats.uri, dataFormats.bitmap, dataFormats.storageItems, customFormatName]);

            // Prepare the icon for a QuickLink
            Windows.ApplicationModel.Package.current.installedLocation.getFileAsync("images\\user.png").then(function (iconFile) {
                quickLink.thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.createFromFile(iconFile);
                shareOperation.reportCompleted(quickLink);
                id("appBody").innerHTML = "<p>The sharing operation has been reported as complete with a QuickLink.</p>";
            });
        } else {
            shareOperation.reportCompleted();
            id("appBody").innerHTML = "<p>The sharing operation has been reported as complete.</p>";
        }
    }

    /// <summary>
    /// Expand/collapse the Extended Sharing div
    /// </summary>
    function expandoClick() {
        if (extendedSharingCollapsed) {
            id("extendedSharing").className = "unhidden";
            // Set expando glyph to up arrow
            id("expandoGlyph").innerHTML = "&#57360;";
            extendedSharingCollapsed = false;
        } else {
            id("extendedSharing").className = "hidden";
            // Set expando glyph to down arrow
            id("expandoGlyph").innerHTML = "&#57361;";
            extendedSharingCollapsed = true;
        }
    }

    /// <summary>
    /// Expand/collapse the QuickLink fields
    /// </summary>
    function addQuickLinkChanged() {
        if (id("addQuickLink").checked) {
            quickLinkFields.className = "unhidden";
        } else {
            quickLinkFields.className = "hidden";
        }
    }

    // TODO: 1 - Declare Share Target in package.appxmanifest/Declarations.
    // TODO: 2 - Initialize the activation handler to catch an activation via sharing.
    WinJS.Application.addEventListener("activated", activatedHandler, false);

    WinJS.Application.start();

    function initialize() {
        id("addQuickLink").addEventListener("change", /*@static_cast(EventListener)*/addQuickLinkChanged, false);
        id("reportCompleted").addEventListener("click", /*@static_cast(EventListener)*/reportCompleted, false);
        id("expandoClick").addEventListener("click", /*@static_cast(EventListener)*/expandoClick, false);
        id("reportStarted").addEventListener("click", /*@static_cast(EventListener)*/reportStarted, false);
        id("reportDataRetrieved").addEventListener("click", /*@static_cast(EventListener)*/reportDataRetrieved, false);
        id("reportSubmittedBackgroundTask").addEventListener("click", /*@static_cast(EventListener)*/reportSubmittedBackgroundTask, false);
        id("reportError").addEventListener("click", /*@static_cast(EventListener)*/reportError, false);
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();
