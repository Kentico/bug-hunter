﻿using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usage of <see cref="System.Web.HttpCookie"/> as properties of <see cref="System.Web.HttpRequest"/>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestCookiesAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpRequestCookiesAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.HttpRequestCookies;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "Request.Cookies", "CookieHelper.RequestCookies");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig ApiReplacementConfig = new ApiReplacementConfig(
            Rule,
            new[] { "System.Web.HttpRequest", "System.Web.HttpRequestBase" },
            new[] { "Cookies" });

        private static readonly ApiReplacementForMemberAnalyzer ApiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(ApiReplacementConfig);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            ApiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
