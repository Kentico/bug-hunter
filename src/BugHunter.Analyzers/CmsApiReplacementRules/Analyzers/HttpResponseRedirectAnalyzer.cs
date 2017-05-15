// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Access to <c>Redirect</c> method of <c>System.Web.HttpResponse</c> or <c>System.Web.HttpResponseBase</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpResponseRedirectAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpResponseRedirectAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.HttpResponseRedirect;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "Response.Redirect()");

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "System.Web.HttpResponse", "System.Web.HttpResponseBase" },
            new[] { "Redirect" });

        private static readonly ApiReplacementForMethodAnalyzer Analyzer = new ApiReplacementForMethodAnalyzer(Config);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            Analyzer.RegisterAnalyzers(context);
        }
    }
}
