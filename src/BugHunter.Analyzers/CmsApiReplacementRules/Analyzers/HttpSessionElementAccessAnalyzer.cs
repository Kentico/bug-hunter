// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.Constants;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Read or write element access to <c>Session</c> collection of <c>System.Web.SessionState.HttpSessionState</c>
    /// or <c>System.Web.HttpSessionStateBase</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionElementAccessAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpSessionElementAccessAnalyzer"/> when element node is assess for reading
        /// </summary>
        public const string DiagnosticIdGet = DiagnosticIds.HttpSessionElementAccessGet;

        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpSessionElementAccessAnalyzer"/> when element node is assess for writing
        /// </summary>
        public const string DiagnosticIdSet = DiagnosticIds.HttpSessionElementAccessSet;

        private static readonly DiagnosticDescriptor RuleForGet = ApiReplacementRulesProvider.GetRule(DiagnosticIdGet, "Session[]", "SessionHelper.GetValue()");

        private static readonly DiagnosticDescriptor RuleForSet = ApiReplacementRulesProvider.GetRule(DiagnosticIdSet, "Session[]", "SessionHelper.SetValue()");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForGet, RuleForSet);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ElementAccessExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            if (elementAccess == null)
            {
                return;
            }

            var compilation = context.SemanticModel.Compilation;
            var accessedTypeSymbol = context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type as INamedTypeSymbol;
            if (accessedTypeSymbol == null || (!IsHttpSession(accessedTypeSymbol, compilation) && !IsHttpSessionBase(accessedTypeSymbol, compilation)))
            {
                return;
            }

            var diagnostic = GetDiagnostic(elementAccess);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsHttpSession(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
            => accessedTypeSymbol?.IsDerivedFrom("System.Web.SessionState.HttpSessionState", compilation) ?? false;

        private static bool IsHttpSessionBase(INamedTypeSymbol accessedTypeSymbol, Compilation compilation)
            => accessedTypeSymbol?.IsDerivedFrom("System.Web.HttpSessionStateBase", compilation) ?? false;

        private static Diagnostic GetDiagnostic(ElementAccessExpressionSyntax elementAccess)
        {
            if (!elementAccess.IsBeingAssigned())
            {
                return Diagnostic.Create(RuleForGet, elementAccess.GetLocation(), elementAccess);
            }

            var assignmentExpression = elementAccess.FirstAncestorOrSelf<AssignmentExpressionSyntax>();
            return Diagnostic.Create(RuleForSet, assignmentExpression.GetLocation(), elementAccess);
        }
    }
}
