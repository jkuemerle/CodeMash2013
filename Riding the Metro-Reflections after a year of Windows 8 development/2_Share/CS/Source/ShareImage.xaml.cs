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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.DataTransfer;
using SDKTemplate;

namespace SDKTemplate
{
    public sealed partial class ShareImage : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;
        private StorageFile imageFile;

        public ShareImage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO 2: the DataRequested event is fired each time the Share Charm is activated by the user when the App is the active application
            //
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
                if (this.imageFile != null)
                {
                    // TODO 3.1: get the DataPackage from the event request
                    //           and set the Properties.Title/.Description
                    DataPackage requestData = e.Request.Data;
                    requestData.Properties.Title = dataPackageTitle;

                    // The description is optional.
                    string dataPackageDescription = DescriptionInputBox.Text;
                    if (dataPackageDescription != null)
                    {
                        requestData.Properties.Description = dataPackageDescription;
                    }

                    // TODO 3.2: get the DataPackage from the event request
                    //           and set real data to share
                    //
                    // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
                    // since the target app may only support one or the other.
                    List<IStorageItem> imageItems = new List<IStorageItem>();
                    imageItems.Add(this.imageFile);
                    requestData.SetStorageItems(imageItems);

                    RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(this.imageFile);
                    requestData.Properties.Thumbnail = imageStreamRef;
                    requestData.SetBitmap(imageStreamRef);
                }
                else
                {
                    e.Request.FailWithDisplayText("Select an image you would like to share and try again.");
                }
            }
            else
            {
                e.Request.FailWithDisplayText(MainPage.MissingTitleError);
            }
        }

        private async void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker imagePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".png", ".bmp", ".gif", ".tif" }
            };

            StorageFile pickedImage = await imagePicker.PickSingleFileAsync();

            if (pickedImage != null)
            {
                this.imageFile = pickedImage;

                // Display the image in the UI.
                IRandomAccessStream displayStream = await pickedImage.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(displayStream);
                ImageHolder.Source = bitmapImage;
                this.rootPage.NotifyUser("Selected " + pickedImage.Name + "." , NotifyType.StatusMessage);

                ShareStep.Visibility = Visibility.Visible;
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO 1: how to programmatically display the Share Charm
            DataTransferManager.ShowShareUI();
        }
    }
}