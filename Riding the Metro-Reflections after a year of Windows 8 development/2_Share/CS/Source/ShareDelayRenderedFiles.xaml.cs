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
using Windows.Graphics.Imaging;
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
    public sealed partial class ShareDelayRenderedFiles : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;
        private StorageFile imageFile;

        public ShareDelayRenderedFiles()
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
            if (imageFile != null)
            {
                DataPackage requestData = e.Request.Data;
                requestData.Properties.Title = "Delay rendered image";
                requestData.Properties.Description = "Resized image from the Share Source sample";
                requestData.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(imageFile);
                
                // TODO 4: delayed rendering of the requested contents with a DataProvider
                requestData.SetDataProvider(StandardDataFormats.Bitmap, new DataProviderHandler(this.OnDeferredImageRequestedHandler));
            }
            else
            {
                e.Request.FailWithDisplayText("Select an image you would like to share and try again.");
            }
        }

        private async void OnDeferredImageRequestedHandler(DataProviderRequest request)
        {
            // In this delegate we provide updated Bitmap data using delayed rendering.

            if (this.imageFile != null)
            {
                // If the delegate is calling any asynchronous operations it needs to acquire the deferral first. This lets the
                // system know that you are performing some operations that might take a little longer and that the call to
                // SetData could happen after the delegate returns. Once you acquired the deferral object you must call Complete
                // on it after your final call to SetData.
                DataProviderDeferral deferral = request.GetDeferral();
                InMemoryRandomAccessStream inMemoryStream = new InMemoryRandomAccessStream();

                // Make sure to always call Complete when done with the deferral.
                try
                {
                    // Decode the image and re-encode it at 50% width and height.
                    IRandomAccessStream imageStream = await this.imageFile.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder imageDecoder = await BitmapDecoder.CreateAsync(imageStream);
                    BitmapEncoder imageEncoder = await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder);
                    imageEncoder.BitmapTransform.ScaledWidth = (uint)(imageDecoder.OrientedPixelWidth * 0.5);
                    imageEncoder.BitmapTransform.ScaledHeight = (uint)(imageDecoder.OrientedPixelHeight * 0.5);
                    await imageEncoder.FlushAsync();

                    request.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream));
                }
                finally
                {
                    deferral.Complete();
                }
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
                this.rootPage.NotifyUser("Selected " + pickedImage.Name + ".", NotifyType.StatusMessage);

                ShareStep.Visibility = Visibility.Visible;
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}