﻿using System.Collections.Immutable;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.SystemIO.Analyzers.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and analyzing Symbol directly
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V02_IdentifierName_SymbolAnalysis : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V02";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (symbol == null)
            {
                return;
            }

            var symbolContainingNamespace = symbol.ContainingNamespace;
            if (!symbolContainingNamespace.ToString().Equals("System.IO"))
            {
                return;
            }

            var exceptionType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.IO.IOException");
            var streamType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.IO.Stream");

            if (symbol.ConstructedFrom.IsDerivedFromClassOrInterface(exceptionType)
             || symbol.ConstructedFrom.IsDerivedFromClassOrInterface(streamType))
            {
                return;
            }

            var diagnostic = AnalyzerHelper.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
