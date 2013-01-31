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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using System.Collections.Generic;

namespace SearchContract
{
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        private SearchPane searchPane;

        private static readonly string exampleResponse = @"jsonSuggestionService\exampleJsonResponse.json";

        public Scenario5()
        {
            this.InitializeComponent();
            searchPane = SearchPane.GetForCurrentView();
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
                string response = await FileIO.ReadTextAsync(file);
                JsonArray parsedResponse = JsonArray.Parse(response);
                if (parsedResponse.Count > 1)
                {
                    parsedResponse = JsonArray.Parse(parsedResponse[1].Stringify());
                    foreach (JsonValue value in parsedResponse)
                    {
                        request.SearchSuggestionCollection.AppendQuerySuggestion(value.GetString());
                        if (request.SearchSuggestionCollection.Size >= MainPage.SearchPaneMaxSuggestions)
                        {
                            break;
                        }
                    }
                }

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
