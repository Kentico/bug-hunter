// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace BugHunter.Core.Helpers.DiagnosticDescriptors
{
    /// <summary>
    /// Helper class for providing URI links for analyzers
    /// </summary>
    public static class HelpLinkUriProvider
    {
        private static readonly string OnlineDocumentationUrl = @"http://kentico.github.io/bug-hunter/{0}.html";
        private static readonly Regex AnalyzerIdFormat = new Regex(@"^BH[0-9a-zA-Z]{4}$", RegexOptions.None);

        /// <summary>
        /// Returns an online documentation help link URI based on <paramref name="analyzerId" />
        /// </summary>
        /// <param name="analyzerId">ID of an analyzer to provide an URI for, must be non empty and in BHXXXX, where X is alphanumeric</param>
        /// <returns>URI with an online documentation for given analyzer ID, if ID is in correct format; throws otherwise</returns>
        public static string GetHelpLink(string analyzerId)
        {
            if (string.IsNullOrWhiteSpace(analyzerId))
            {
                throw new ArgumentException($"The {nameof(analyzerId)} must not be empty.", nameof(analyzerId));
            }

            if (analyzerId.Length != 6 || !AnalyzerIdFormat.IsMatch(analyzerId))
            {
                throw new ArgumentException($"The {nameof(analyzerId)} must be in format 'BHXXXX' where X are alphanumeric characters.", nameof(analyzerId));
            }

            return string.Format(OnlineDocumentationUrl, analyzerId);
        }
    }
}
