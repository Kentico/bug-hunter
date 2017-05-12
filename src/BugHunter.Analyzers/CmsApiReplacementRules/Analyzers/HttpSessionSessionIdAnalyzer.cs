﻿using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.HttpSessionStateBase"/> or <see cref="System.Web.SessionState.HttpSessionState"/> and their access to SessionID member
    /// </summary>
    /// <remarks>
    /// Note that both classes need to be checked for access as they do not derive from one another and both can be used. For different scenarios check code in test file.
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionSessionIdAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpSessionSessionIdAnalyzer"/>
        /// </summary>
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_SESSION_SESSION_ID;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID, "Session.SessionId", "SessionHelper.GetSessionID()");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(Rule,
            new []{ "System.Web.SessionState.HttpSessionState", "System.Web.HttpSessionStateBase"},
            new []{ "SessionID"});

        private static readonly ApiReplacementForMemberAnalyzer apiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(apiReplacementConfig);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
