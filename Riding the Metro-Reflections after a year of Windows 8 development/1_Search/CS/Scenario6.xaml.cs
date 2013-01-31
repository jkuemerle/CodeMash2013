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

using SDKTemplate;

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchContract
{
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        private SearchPane searchPane;

        private static readonly string exampleResponse = @"xmlSuggestionService\exampleXmlResponse.xml";

        public Scenario6()
        {
            this.InitializeComponent();
            searchPane = SearchPane.GetForCurrentView();
        }

        private void AddSuggestionFromNode(IXmlNode node, SearchSuggestionCollection suggestions)
        {
            string text = "";
            string description = "";
            string url = "";
            string imageUrl = "";
            string imageAlt = "";

            foreach (IXmlNode subNode in node.ChildNodes)
            {
                if (subNode.NodeType != NodeType.ElementNode)
                {
                    continue;
                }
                if (subNode.NodeName.Equals("Text", StringComparison.CurrentCultureIgnoreCase))
                {
                    text = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Description", StringComparison.CurrentCultureIgnoreCase))
                {
                    description = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
                {
                    url = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Image", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (subNode.Attributes.GetNamedItem("source") != null)
                    {
                        imageUrl = subNode.Attributes.GetNamedItem("source").InnerText;
                    }
                    if (subNode.Attributes.GetNamedItem("alt") != null)
                    {
                        imageAlt = subNode.Attributes.GetNamedItem("alt").InnerText;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                // No proper suggestion item exists
            }
            else if (string.IsNullOrWhiteSpace(url))
            {
                suggestions.AppendQuerySuggestion(text);
            }
            else
            {
                // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
                Uri uri = string.IsNullOrWhiteSpace(imageUrl) ? new Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png") : new Uri(imageUrl);
                RandomAccessStreamReference imageSource = RandomAccessStreamReference.CreateFromUri(uri);
                suggestions.AppendResultSuggestion(text, description, url, imageSource, imageAlt);
            }
        }

        private async Task GenerateSuggestions(StorageFile file, SearchSuggestionCollection suggestions)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(await FileIO.ReadTextAsync(file));
            XmlNodeList nodes = doc.GetElementsByTagName("Section");
            if (nodes.Count > 0)
            {
                IXmlNode section = nodes[0];
                foreach (IXmlNode node in section.ChildNodes)
                {
                    if (node.NodeType != NodeType.ElementNode)
                    {
                        continue;
                    }
                    if (node.NodeName.Equals("Separator", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string title = node.Attributes.GetNamedItem("title").NodeValue.ToString();
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            suggestions.AppendSearchSeparator(title);
                        }
                    }
                    else
                    {
                        AddSuggestionFromNode(node, suggestions);
                    }
                }
            }
        }

        private async void OnSearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs e)
        {
            var queryText = e.QueryText;
            if (string.IsNullOrEmpty(queryText))
            {
                MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage);
            }
            else
            {
                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                var request = e.Request;
                var deferral = request.GetDeferral();

                StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(exampleResponse);
                await this.GenerateSuggestions(file, request.SearchSuggestionCollection);

                if (request.SearchSuggestionCollection.Size > 0)
                {
                    MainPage.Current.NotifyUser("Suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                }
                else
                {
                    MainPage.Current.NotifyUser("No suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                }

                deferral.Complete();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage);
            // This event should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
            // Typically this occurs in App.xaml.cs.
            searchPane.SuggestionsRequested += new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(OnSearchPaneSuggestionsRequested);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            searchPane.SuggestionsRequested -= new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(OnSearchPaneSuggestionsRequested);
        }
    }
}
