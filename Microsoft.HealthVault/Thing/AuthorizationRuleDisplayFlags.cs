// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// A class representing the display strategies for optional rules
    /// </summary>
    ///
    /// <remarks>
    /// Optional rules may be displayed in the AppAuth page in several ways.
    /// This class specifies the configuration to control that display.
    /// </remarks>
    ///
    [Flags]
    internal enum AuthorizationRuleDisplayFlags
    {
        /// <summary>
        /// No special display handling configured
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// If set, the rule is displayed when the person is initially
        /// authorizing the application.
        /// </summary>
        ///
        DisplayFirstTime = 0x1,

        /// <summary>
        /// If set, display the rule as checked when
        /// initially authorizing the application.
        /// </summary>
        ///
        CheckedFirstTime = 0x2,

        /// <summary>
        /// If set, the display the rule as checked.
        /// </summary>
        ///
        CheckedByDefault = 0x4
    }
}
