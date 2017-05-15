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
    /// Access to one of <c>RegisterArrayDeclaration</c>, <c>RegisterClientScriptBlock</c>, <c>RegisterClientScriptInclude</c>, or <c>RegisterStartupScript</c>
    /// methods of <c>System.Web.UI.ClientScriptManager</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ClientScriptMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ClientScriptMethods;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "ClientScript methods");

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "System.Web.UI.ClientScriptManager" },
            new[] { "RegisterArrayDeclaration", "RegisterClientScriptBlock", "RegisterClientScriptInclude", "RegisterStartupScript" });

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
