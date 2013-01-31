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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using SDKTemplate;

namespace SDKTemplate
{
    public sealed partial class ShareLink : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;

        public ShareLink()
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
                string uriValue = UriToShare.Text;
                if (!String.IsNullOrEmpty(uriValue))
                {
                    // If the URI is malformed, an exception may be thrown.
                    try
                    {
                        Uri dataPackageUri = new Uri(uriValue);
                        DataPackage requestData = e.Request.Data;
                        requestData.Properties.Title = dataPackageTitle;

                        // The description is optional.
                        string dataPackageDescription = DescriptionInputBox.Text;
                        if (dataPackageDescription != null)
                        {
                            requestData.Properties.Description = dataPackageDescription;
                        }
                        requestData.SetUri(dataPackageUri);
                    }
                    catch (Exception)
                    {
                        this.rootPage.NotifyUser("Exception occurred: the uri provided " + uriValue + " is not well formatted.", NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    e.Request.FailWithDisplayText("Enter the link you would like to share and try again.");
                }
            }
            else
            {
                e.Request.FailWithDisplayText(MainPage.MissingTitleError);
            }
        }

        private void UriToShare_LostFocus(object sender, RoutedEventArgs e)
        {
            // We'll validate the Uri in the data requested handler.
            this.rootPage.NotifyUser("", NotifyType.StatusMessage);
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}