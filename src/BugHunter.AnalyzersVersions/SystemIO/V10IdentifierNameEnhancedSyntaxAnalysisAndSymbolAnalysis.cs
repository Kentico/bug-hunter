﻿using System.Collections.Immutable;
using System.Linq;
using BugHunter.AnalyzersVersions.SystemIO.Helpers;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.SystemIO
{
    /// <summary>
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    ///
    /// Version with callback on IdentifierName and analyzing Symbol directly
    /// </summary>
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class V10IdentifierNameEnhancedSyntaxAnalysisAndSymbolAnalysis : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="V10IdentifierNameEnhancedSyntaxAnalysisAndSymbolAnalysis"/>
        /// </summary>
        public const string DiagnosticId = "BHxV10";

        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DiagnosticId);

        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
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

            // quick only syntax based analysis
            if (!FileContainsSystemIoUsing(identifierNameSyntax))
            {
                // identifier name must be fully qualified - look for System.IO there
                var rootIdentifierName = identifierNameSyntax.GetOuterMostParentOfDottedExpression();

                // if not System.IO or if exception or string are used, return
                if (!rootIdentifierName.ToString().Contains("System.IO")
                    || rootIdentifierName.ToString().Contains("IOException")
                    || rootIdentifierName.ToString().Contains("Stream")
                    || rootIdentifierName.ToString().Contains("SeekOrigin"))
                {
                    return;
                }
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

            if (symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.SeekOrigin", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool FileContainsSystemIoUsing(SyntaxNode identifierNameSyntax)
        {
            var allUsings = identifierNameSyntax
                .AncestorsAndSelf()
                .OfType<CompilationUnitSyntax>()
                .SingleOrDefault()
                .Usings;

            var systemIoUsings = allUsings
                .Where(u => u.ToString().Contains("System.IO"));

            return systemIoUsings.Any();
        }
    }
}
