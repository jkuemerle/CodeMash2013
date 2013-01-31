//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="base-sdk.js" />

(function () {
    var shareValue;
    var multipleFiles;
    var imageFile;
    var dataPackage;
    var noContentMessage;

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function initialize() {
        setupShare();
        id("scenario1SetButton").addEventListener("click", scenario1Set, false);
        id("scenario2SetButton").addEventListener("click", scenario2Set, false);
        id("scenario3SetButton").addEventListener("click", scenario3Set, false);
        id("scenario4SetButton").addEventListener("click", scenario4Set, false);
        id("scenario5SetButton").addEventListener("click", scenario5Set, false);
        id("scenario6SetButton").addEventListener("click", scenario6Set, false);
        id("scenario7SetButton").addEventListener("click", scenario7Set, false);
        id("scenario8SetButton").addEventListener("click", scenario8Set, false);
        id("scenario3ChooseImageButton").addEventListener("click", scenario3ChooseImage, false);
        id("scenario4ChooseFilesButton").addEventListener("click", scenario4ChooseFiles, false);
        id("scenario5ChooseImageButton").addEventListener("click", scenario5ChooseImage, false);
        id("shareButton").addEventListener("click", showShareUI, false);
        id("scenarios").addEventListener("change", onScenarioChanged, false);
    }

    function scenario1Set() {
        shareValue = id("textInputBox").value;
        sdkSample.displayStatus("Text has been set for sharing.");
        id("shareStep").className = "unhidden";
    }


    function scenario2Set() {
        shareValue = new Windows.Foundation.Uri(id("linkInputBox").value);
        sdkSample.displayStatus("Link has been set for sharing.");
        id("shareStep").className = "unhidden";
    }

    function scenario3Set() {
        if (imageFile) {
            shareValue = imageFile;
            sdkSample.displayStatus("Image has been set for sharing.");
            id("shareStep").className = "unhidden";
        } else {
            sdkSample.displayError("No image was selected.");
        }
    }

    function scenario4Set() {
        if (multipleFiles) {
            shareValue = multipleFiles;
            sdkSample.displayStatus(multipleFiles.size + " files have been set for sharing.");
            id("shareStep").className = "unhidden";
        } else {
            sdkSample.displayError("No files were selected.");
        }
    }
    function scenario5Set() {
        shareValue = imageFile;
        sdkSample.displayStatus("Image has been set for sharing.");
        id("shareStep").className = "unhidden";
    }
    function scenario6Set() {
        shareValue = id("htmlFragment").outerHTML;
        sdkSample.displayStatus("HTML fragment has been set for sharing.");
        id("shareStep").className = "unhidden";
    }
    function scenario7Set() {
        var book = {
            type : "http://schema.org/Book",
            properties : {
                image : "http://sourceurl.com/catcher-in-the-rye-book-cover.jpg",
                name : "The Catcher in the Rye",
                bookFormat : "http://schema.org/Paperback",
                author : "http://sourceurl.com/author/jd_salinger.html",
                numberOfPages : 224,
                publisher : "Little, Brown, and Company",
                datePublished : "1991-05-01",
                inLanguage : "English",
                isbn : "0316769487"
            }
        };

        shareValue = JSON.stringify(book);
        sdkSample.displayStatus("Custom data has been set for sharing.");
        id("shareStep").className = "unhidden";
    }

    function scenario8Set() {
        noContentMessage = id("displayTextInputBox").value;
        sdkSample.displayStatus("When the sharing window is invoked, the above display text will be shown.");
        id("shareStep").className = "unhidden";
    }

    function scenario3ChooseImage() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".bmp", ".gif", ".png"]);
        picker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
        picker.pickSingleFileAsync().then(function (file) {
            if (file) {
                // Display the image to the user
                id("scenario3ImageHolder").src = URL.createObjectURL(file);
                // The imageFile variable will be set to shareValue when the user clicks Set
                imageFile = file;
            } else {
                sdkSample.displayError("No file chosen.");
            }
        });
    }

    function scenario4ChooseFiles() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.append("*");
        picker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
        picker.pickMultipleFilesAsync().then(function (files) {
            if (files) {
                multipleFiles = files;
                selectedFiles.innerText = "";
                for (var i = 0; i < files.size; i++) {
                    selectedFiles.innerText += files[i].name;
                    if (i !== files.size - 1) {
                        selectedFiles.innerText += ", ";
                    }
                }
            } else {
                selectedFiles.innerText = "No files selected.";
                sdkSample.displayError("Invalid files.");
            }
        });
    }

    function scenario5ChooseImage() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".bmp", ".gif", ".png"]);
        picker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
        picker.pickSingleFileAsync().then(function (file) {
            if (file) {
                // Display the image to the user
                var imageUrl = URL.createObjectURL(file);
                id("scenario5ImageHolder").src = imageUrl;
                // The imageFile variable will be set to shareValue when the user clicks Set
                imageFile = file;
            } else {
                sdkSample.displayError("Invalid file.");
            }
        });
    }


    function onScenarioChanged() {
        // Do any necessary clean up on the output, the scenario id
        // can be obtained from sdkSample.scenarioId.
        sdkSample.displayStatus("");
        id("shareStep").className = "hidden";
    }

    // TODO: 2 - Register with DataTransferManager to receive datarequested event.
    function setupShare() {
        var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
        dataTransferManager.addEventListener("datarequested", function (e) {
            var request = e.request;

            switch (sdkSample.scenarioId) {
                case "1": //Text Sharing Example
                    textDataRequestedHandler(request);
                    break;

                case "2": // Link Sharing Example
                    uriDataRequestedHandler(request);
                    break;

                case "3": // Image Sharing Example
                    imageDataRequestedHandler(request);
                    break;

                case "4": // File Sharing Example
                    storageItemsDataRequestedHandler(request);
                    break;

                case "5": // Delayed Rendering Example
                    delayedRenderingDataRequestedHandler(request);
                    break;
                
                case "6": // HTML Fragment Example
                    htmlFragmentDataRequestedHandler(request);
                    break;

                case "7": // Custom Data Sharing Example
                    customDataRequestedHandler(request);
                    break;

                case "8": // Custom Error Message Example
                    noContentRequestedHandler(request);
                    break;

                default:
                    sdkSample.displayError("Invalid Share scenario selected.");
                    break;
            }
        });
    }

    // TODO: 3 - Populate the DataRequest with data when the datarequested event fires.
    function textDataRequestedHandler(request) {
        request.data.properties.title = id("titleInputBox1").value; // Title required
        request.data.properties.description = id("descriptionInputBox1").value; // Description optional
        request.data.setText(shareValue);
    }

    function uriDataRequestedHandler(request) {
        request.data.properties.title = id("titleInputBox2").value; // Title required
        request.data.properties.description = id("descriptionInputBox2").value; // Description optional
        request.data.setUri(shareValue);
    }

    function imageDataRequestedHandler(request) {
        request.data.properties.title = id("titleInputBox3").value; // Title required
        request.data.properties.description = id("descriptionInputBox3").value; // Description optional

        // When sharing an image, don't forget to set the thumbnail for the DataPackage
        var streamReference = Windows.Storage.Streams.RandomAccessStreamReference.createFromFile(shareValue);
        request.data.properties.thumbnail = streamReference;

        // It's recommended to always use both setBitmap and setStorageItems for sharing a single image
        // since the Target app may only support one or the other

        // Put the image file in an array and pass it to setStorageItems
        request.data.setStorageItems([shareValue]);

        // The setBitmap method requires a RandomAccessStreamReference
        request.data.setBitmap(streamReference);
    }

    function storageItemsDataRequestedHandler(request) {
        request.data.properties.title = id("titleInputBox4").value; // Title required
        request.data.properties.description = id("descriptionInputBox4").value; // Description optional

        request.data.setStorageItems(shareValue);
    }

    function delayedRenderingDataRequestedHandler(request) {
        request.data.properties.title = "Delay rendered image"; // Title required
        request.data.properties.description = "Resized image from the Share Source sample"; // Description optional
        // When sharing an image, don't forget to set the thumbnail for the DataPackage
        var streamReference = Windows.Storage.Streams.RandomAccessStreamReference.createFromFile(shareValue);
        request.data.properties.thumbnail = streamReference;
        request.data.setDataProvider(Windows.ApplicationModel.DataTransfer.StandardDataFormats.bitmap, onDeferredImageRequested);
    }

    // TODO: 4 - Create a DataPackage with the HTML content.
    function htmlFragmentDataRequestedHandler(request) {
        // We construct a DataPackage, prepopulated with HTML content, based on an HTML range
        var range = document.createRange();
        range.selectNode(id("htmlFragment"));
        request.data = MSApp.createDataPackage(range);

        request.data.properties.title = id("titleInputBox6").value; // Title required
        request.data.properties.description = id("descriptionInputBox6").value; // Description optional

        // Generate resource map for images. We use the image's relative src property as the key to the resourceMap item
        var imageUri = new Windows.Foundation.Uri(id("htmlFragmentImage").src);
        var streamRef = new Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(imageUri);
        var path = id("htmlFragmentImage").getAttribute("src");
        request.data.resourceMap[path] = streamRef;
   }

    function customDataRequestedHandler(request) {
        request.data.properties.title = id("titleInputBox7").value; // Title required
        request.data.properties.description = id("descriptionInputBox7").value; // Description optional
        var dataFormat = id("dataFormatInputBox7").value;
        request.data.setData(dataFormat, shareValue);
    }

    function noContentRequestedHandler(request) {
        request.failWithDisplayText(noContentMessage);
    }

    function showShareUI() {
        // TODO 1: how to display Share Charm by code
        Windows.ApplicationModel.DataTransfer.DataTransferManager.showShareUI();
    }

    /// <summary>
    /// Specific handler for requests that require the use of the deferral (Image only)
    /// </summary>
    function onDeferredImageRequested(request) {
        if (shareValue) {
            // Here we provide updated Bitmap data using delayed rendering
            var deferral = request.getDeferral();
            shareValue.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {
                // Decode the image
                Windows.Graphics.Imaging.BitmapDecoder.createAsync(stream).then(function (decoder) {
                    // Re-encode the image at 50% width and height
                    var inMemoryStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                    Windows.Graphics.Imaging.BitmapEncoder.createForTranscodingAsync(inMemoryStream, decoder).then(function (encoder) {
                        encoder.bitmapTransform.scaledWidth = decoder.orientedPixelWidth * 0.5;
                        encoder.bitmapTransform.scaledHeight = decoder.orientedPixelHeight * 0.5;
                        encoder.flushAsync().then(function () {
                            var streamReference = Windows.Storage.Streams.RandomAccessStreamReference.createFromStream(inMemoryStream);
                            request.setData(streamReference);
                            deferral.complete();
                        });
                    });
                });
            });
        }
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();