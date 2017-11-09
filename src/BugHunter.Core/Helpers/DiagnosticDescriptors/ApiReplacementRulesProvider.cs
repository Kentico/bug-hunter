// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.DiagnosticDescriptors
{
    /// <summary>
    /// Helper class for constructing <see cref="DiagnosticDescriptor"/> for API Replacement Analyzers
    /// </summary>
    public static class ApiReplacementRulesProvider
    {
        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for API Replacement Analyzer.
        /// </summary>
        /// <remarks>
        /// Analyzer will have provided diagnostic ID, CmsApiReplacements category,
        /// Warning severity, HelperLinkUri, and will be enabled by default
        /// </remarks>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <param name="forbiddenUsage">Message argument with forbidden usage</param>
        /// <returns>Diagnostic descriptor for API Replacement Analyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage)
            => new DiagnosticDescriptor(
                diagnosticId,
                title: ApiReplacementsMessagesProvider.GetTitle(forbiddenUsage),
                messageFormat: ApiReplacementsMessagesProvider.GetMessageFormat(),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessagesProvider.GetDescription(forbiddenUsage),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));

        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for API Replacement Analyzer.
        /// </summary>
        /// <remarks>
        /// Analyzer will have provided diagnostic ID, CmsApiReplacements category,
        /// Warning severity, HelperLinkUri, and will be enabled by default
        /// </remarks>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <param name="forbiddenUsage">Message argument with forbidden usage</param>
        /// <param name="recommendedUsage">Message argument with recommended usage</param>
        /// <param name="severity">Severity of the diagnostic. It is a warning by default.</param>
        /// <returns>Diagnostic descriptor for API Replacement Analyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage, string recommendedUsage, DiagnosticSeverity severity = DiagnosticSeverity.Warning)
           => new DiagnosticDescriptor(
               diagnosticId,
                title: ApiReplacementsMessagesProvider.GetTitle(forbiddenUsage, recommendedUsage),
                messageFormat: ApiReplacementsMessagesProvider.GetMessageFormat(recommendedUsage),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: severity,
                isEnabledByDefault: true,
                description: ApiReplacementsMessagesProvider.GetDescription(forbiddenUsage, recommendedUsage),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));
    }
}