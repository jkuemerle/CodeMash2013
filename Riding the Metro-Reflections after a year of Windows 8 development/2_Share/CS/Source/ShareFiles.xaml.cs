//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.DataTransfer;
using SDKTemplate;

namespace SDKTemplate
{
    public sealed partial class ShareFiles : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;
        private IReadOnlyList<StorageFile> storageItems;

        public ShareFiles()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register this page as a share source.
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister this page as a share source.
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            string dataPackageTitle = TitleInputBox.Text;

            // The title is required.
            if (!String.IsNullOrEmpty(dataPackageTitle))
            {
                if (this.storageItems != null)
                {
                    DataPackage requestData = e.Request.Data;
                    requestData.Properties.Title = dataPackageTitle;

                    // The description is optional.
                    string dataPackageDescription = DescriptionInputBox.Text;
                    if (dataPackageDescription != null)
                    {
                        requestData.Properties.Description = dataPackageDescription;
                    }
                    requestData.SetStorageItems(this.storageItems);
                }
                else
                {
                    e.Request.FailWithDisplayText("Select the files you would like to share and try again.");
                }
            }
            else
            {
                e.Request.FailWithDisplayText(MainPage.MissingTitleError);
            }
        }

        private async void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { "*" }
            };

            IReadOnlyList<StorageFile> pickedFiles = await filePicker.PickMultipleFilesAsync();

            if (pickedFiles.Count > 0)
            {
                this.storageItems = pickedFiles;

                // Display the file names in the UI.
                string selectedFiles = String.Empty;
                for (int index = 0; index < pickedFiles.Count; index++)
                {
                    selectedFiles += pickedFiles[index].Name;

                    if (index != (pickedFiles.Count - 1))
                    {
                        selectedFiles += ", ";
                    }
                }
                this.rootPage.NotifyUser("Picked files: " + selectedFiles + ".", NotifyType.StatusMessage);

                ShareStep.Visibility = Visibility.Visible;
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}