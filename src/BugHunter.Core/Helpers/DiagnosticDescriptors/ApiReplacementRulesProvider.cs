﻿using BugHunter.Core.Constants;
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
        /// 
        /// Analyzer will have provided diagnostic ID, CmsApiReplacements category, 
        /// Warning severity and will be enabled by default
        /// </summary>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <param name="forbiddenUsage">Message argument with forbidden usage</param>
        /// <returns>Diagnostic descriptor for API Replacement Analyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage)
            => new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessagesProvider.GetTitle(forbiddenUsage),
                messageFormat: ApiReplacementsMessagesProvider.GetMessageFormat(),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessagesProvider.GetDescription(forbiddenUsage),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));

        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for API Replacement Analyzer.
        /// 
        /// Analyzer will have provided diagnostic ID, CmsApiReplacements category, 
        /// Warning severity and will be enabled by default
        /// </summary>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <param name="forbiddenUsage">Message argument with forbidden usage</param>
        /// <param name="recommendedUsage">Message argument with recommended usage</param>
        /// <returns>Diagnostic descriptor for API Replacement Analyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage, string recommendedUsage)
           => new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessagesProvider.GetTitle(forbiddenUsage, recommendedUsage),
                messageFormat: ApiReplacementsMessagesProvider.GetMessageFormat(recommendedUsage),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessagesProvider.GetDescription(forbiddenUsage, recommendedUsage),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));
    }
}