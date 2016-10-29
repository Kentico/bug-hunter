﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.HttpSessionStateBase"/> or <see cref="System.Web.SessionState.HttpSessionState"/> and their access to SessionID member
    /// </summary>
    /// <remarks>
    /// Note that both classes need to be checked for access as they do not derive from one another and both can be used. For different scenarios check code in test file.
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionSessionIdAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_SESSION_SESSION_ID;

        private static readonly DiagnosticDescriptor Rule = GetRule(DIAGNOSTIC_ID, "Session.SessionId", "SessionHelper.GetSessionID()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, typeof(System.Web.SessionState.HttpSessionState), nameof(System.Web.SessionState.HttpSessionState.SessionID));
            RegisterAction(Rule, context, typeof(System.Web.HttpSessionStateBase), nameof(System.Web.HttpSessionStateBase.SessionID));
        }
    }
}
