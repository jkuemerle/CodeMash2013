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
    public sealed partial class ShareHtml : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        private DataTransferManager dataTransferManager;
        private const string htmlContent = @"<html><body><div id=""htmlFragment"" style=""width: 500px;background-color:#f2f2f2;padding: 1px 15px 20px 15px;margin-bottom:40px; font-family:'Segoe UI Semilight'"">
                            <h2 style=""margin-bottom:10px""><img id=""htmlFragmentImage"" src=""http://build.blob.core.windows.net/media/Default/home/dev-center_branding.png"" alt=""Windows Dev Center"" /></h2>
                            <p style=""margin-top:0"">Go to the new <a href=""http://msdn.microsoft.com/en-us/windows/home/"">Windows Dev Center</a> to get the Windows 8 Developer Preview, dev tools, samples, forums, docs and other resources to start building on Windows 8 now.</p>

                            <a href=""http://msdn.microsoft.com/en-us/windows/apps/br229516"" style=""padding-left:20px;""><span style=""font-size:14px"">Downloads</span></a>
                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>
                            <a href=""http://msdn.microsoft.com/library/windows/apps/br211386""><span style=""font-size:14px"">Getting started</span></a>
                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>
                            <a href=""http://code.msdn.microsoft.com/en-us/windowsapps""><span style=""font-size:14px"">App samples</span></a>
                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>
                            <a href=""http://social.msdn.microsoft.com/Forums/en-us/category/windowsapps""><span style=""font-size:14px"">Forums</span></a>
                        </div></body></html>";

        public ShareHtml()
        {
            this.InitializeComponent();
            ShareWebView.NavigateToString(htmlContent);
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
            // Get the user's selection from the WebView.
            DataPackage requestData = ShareWebView.DataTransferPackage;
            DataPackageView dataPackageView = requestData.GetView();

            if ((dataPackageView != null) && (dataPackageView.AvailableFormats.Count > 0))
            {
                string dataPackageTitle = TitleInputBox.Text;

                // The title is required.
                if (!String.IsNullOrEmpty(dataPackageTitle))
                {
                    requestData.Properties.Title = dataPackageTitle;

                    // The description is optional.
                    string dataPackageDescription = DescriptionInputBox.Text;
                    if (dataPackageDescription != null)
                    {
                        requestData.Properties.Description = dataPackageDescription;
                    }
                    e.Request.Data = requestData;
                }
                else
                {
                    e.Request.FailWithDisplayText(MainPage.MissingTitleError);
                }
            }
            else
            {
                e.Request.FailWithDisplayText("Make a selection in the HTML fragment and try again.");
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}