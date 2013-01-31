﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace ApplicationSettings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddSettingsScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        bool isEventRegistered;

        public AddSettingsScenario()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage.NotifyUser("To show the settings charm window, invoke the charm bar by swiping your finger on the right edge of the screen or bringing your mouse to the lower right corner of the screen, then select Settings. Or you can just press Windows logo + i. To dismiss the settings charm, tap in the application, swipe a screen edge, right click, invoke another charm or application.", NotifyType.StatusMessage);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Added to make sure the event handler for CommandsRequested in cleaned up before other scenarios
            if (this.isEventRegistered)
            {
                SettingsPane.GetForCurrentView().CommandsRequested -= onCommandsRequested;
                this.isEventRegistered = false;
            }
        }

        /// <summary>
        /// This is the click handler for the 'addSettingsScenarioAdd' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void addSettingsScenarioAdd_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You selected the " + b.Content+ " button", NotifyType.StatusMessage);
                if (!this.isEventRegistered)
                {
                    // TODO 2.1: be notified before the Settings Charm appears to add our settings
                    SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
                    this.isEventRegistered = true;
                }
            }
        }

        void onSettingsCommand(IUICommand command)
        {
            // TODO 2.3: execute the corresponding setting
            //           --> usually based on the SettingsCommand.Id value
            SettingsCommand settingsCommand = (SettingsCommand)command;
            rootPage.NotifyUser("You selected the " + settingsCommand.Label + " settings command", NotifyType.StatusMessage);
        }

        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            // TODO 2.2: create each setting to add to the Charm
            //           --> will invoke the same handler but just for the purpose of the demo
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);

            SettingsCommand generalCommand = new SettingsCommand("generalSettings", "General", handler);
            eventArgs.Request.ApplicationCommands.Add(generalCommand);

            SettingsCommand helpCommand = new SettingsCommand("helpPage", "Help", handler);
            eventArgs.Request.ApplicationCommands.Add(helpCommand);
        }

        private void OnShowSettingsCharm(object sender, RoutedEventArgs e)
        {
            // TODO 1: show the Settings Charm by code
            SettingsPane.Show();
        }
    }
}
