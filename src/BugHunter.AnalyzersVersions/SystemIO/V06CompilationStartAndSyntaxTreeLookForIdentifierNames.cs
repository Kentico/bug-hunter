﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
    /// </summary>
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class V06CompilationStartAndSyntaxTreeLookForIdentifierNames : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="V06CompilationStartAndSyntaxTreeLookForIdentifierNames"/>
        /// </summary>
        public const string DiagnosticId = "BHxV06";

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

            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var compilationAnalyzer = new CompilationAnalyzer(compilationStartAnalysisContext.Compilation);

                compilationStartAnalysisContext.RegisterSyntaxTreeAction(systaxTreeContext => compilationAnalyzer.Analyze(systaxTreeContext));

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationAnalyzer.Evaluate(compilationEndContext));
            });
        }

        private class CompilationAnalyzer
        {
            private readonly Compilation _compilation;
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly List<IdentifierNameSyntax> _badNodes;

            /// <summary>
            /// Initializes a new instance of the <see cref="CompilationAnalyzer"/> class.
            /// </summary>
            /// <param name="compilation">Current compilation</param>
            public CompilationAnalyzer(Compilation compilation)
            {
                _compilation = compilation;

                _whitelistedTypes = AnalyzerHelper.WhiteListedTypeNames
                    .Select(compilation.GetTypeByMetadataName)
                    .ToArray();

                _badNodes = new List<IdentifierNameSyntax>();
            }

            public void Analyze(SyntaxTreeAnalysisContext context)
            {
                var syntaxTree = context.Tree;
                var identifierNameSyntaxes = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<IdentifierNameSyntax>();
                var semanticModel = _compilation.GetSemanticModel(syntaxTree);

                foreach (var identifierNameSyntax in identifierNameSyntaxes)
                {
                    if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
                    {
                        continue;
                    }

                    var symbol = semanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    var symbolContainingNamespace = symbol.ContainingNamespace;
                    if (!symbolContainingNamespace.ToString().Equals("System.IO"))
                    {
                        continue;
                    }

                    if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFrom(allowedType)))
                    {
                        continue;
                    }

                    _badNodes.Add(identifierNameSyntax);
                }
            }

            public void Evaluate(CompilationAnalysisContext compilationEndContext)
            {
                Parallel.ForEach(_badNodes, (identifierNameSyntax) =>
                {
                    var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                });
            }
        }
    }
}
