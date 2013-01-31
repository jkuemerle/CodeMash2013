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

using System.Collections.Generic;
using System;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Share Source C#";
        public const string MissingTitleError = "Enter a title for what you are sharing and try again.";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Share text", ClassType = typeof(SDKTemplate.ShareText) },
            new Scenario() { Title = "Share link", ClassType = typeof(SDKTemplate.ShareLink) },
            new Scenario() { Title = "Share image", ClassType = typeof(SDKTemplate.ShareImage) },
            new Scenario() { Title = "Share files", ClassType = typeof(SDKTemplate.ShareFiles) },
            new Scenario() { Title = "Share delay rendered files", ClassType = typeof(SDKTemplate.ShareDelayRenderedFiles) },
            new Scenario() { Title = "Share HTML content", ClassType = typeof(SDKTemplate.ShareHtml) },
            new Scenario() { Title = "Share custom data", ClassType = typeof(SDKTemplate.ShareCustomData) },
            new Scenario() { Title = "Fail with display text", ClassType = typeof(SDKTemplate.SetErrorMessage) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}