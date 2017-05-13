﻿using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestQueryStringAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpRequestQueryStringAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.HttpRequestQueryString;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "QueryString[]", "QueryHelper.Get<Type>()");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig ApiReplacementConfig = new ApiReplacementConfig(
            Rule,
            new []{ "System.Web.HttpRequest", "System.Web.HttpRequestBase"},
            new []{ "QueryString"});

        private static readonly ApiReplacementForMemberAnalyzer ApiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(ApiReplacementConfig);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            ApiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
