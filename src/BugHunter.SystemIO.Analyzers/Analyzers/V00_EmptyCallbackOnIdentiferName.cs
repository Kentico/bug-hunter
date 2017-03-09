﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.SystemIO.Analyzers.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V00_EmptyCallbackOnIdentiferName : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V00";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(c => Analyze(c, Rule), SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule)
        {
            // empty callback serves only as baseline for performance tests
        }
    }
}
