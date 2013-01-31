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
    public sealed partial class ShareCustomData : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;

        public ShareCustomData()
        {
            this.InitializeComponent();
            CustomDataTextBox.Text =
            @"{
               ""type"" : ""http://schema.org/Book"",
               ""properties"" :
               {
                ""image"" : ""http://sourceurl.com/catcher-in-the-rye-book-cover.jpg"",
                ""name"" : ""The Catcher in the Rye"",
                ""bookFormat"" : ""http://schema.org/Paperback"",
                ""author"" : ""http://sourceurl.com/author/jd_salinger.html"",
                ""numberOfPages"" : 224,
                ""publisher"" : ""Little, Brown, and Company"",
                ""datePublished"" : ""1991-05-01"",
                ""inLanguage"" : ""English"",
                ""isbn"" : ""0316769487""
                }
            }";
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
                string dataPackageFormat = DataFormatInputBox.Text;
                if (!String.IsNullOrEmpty(dataPackageFormat))
                {
                    string dataPackageText = CustomDataTextBox.Text;
                    if (!String.IsNullOrEmpty(dataPackageText))
                    {
                        DataPackage requestData = e.Request.Data;
                        requestData.Properties.Title = dataPackageTitle;

                        // The description is optional.
                        string dataPackageDescription = DescriptionInputBox.Text;
                        if (dataPackageDescription != null)
                        {
                            requestData.Properties.Description = dataPackageDescription;
                        }

                        // TODO 5: specify a custom format (here a JSON based serialization)
                        //         see the Share Target sample to consume it
                        requestData.SetData(dataPackageFormat, dataPackageText);
                    }
                    else
                    {
                        e.Request.FailWithDisplayText("Enter the custom data you would like to share and try again.");
                    }
                }
                else
                {
                    e.Request.FailWithDisplayText("Enter a custom data format and try again.");
                }
            }
            else
            {
                e.Request.FailWithDisplayText(MainPage.MissingTitleError);
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}