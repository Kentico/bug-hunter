﻿using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.SystemIoRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer_V4_CompilationStart : DiagnosticAnalyzer
    {
        private static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream"
        };

        public const string DIAGNOSTIC_ID = DiagnosticIds.SYSTEM_IO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Title), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                messageFormat: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_MessageFormat), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                category: AnalyzerCategories.SystemIo,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Description), SystemIoResources.ResourceManager, typeof(SystemIoResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var whitelistedTypes = WhiteListedTypeNames
                    .Select(compilationStartAnalysisContext.Compilation.GetTypeByMetadataName)
                    .ToArray();

                compilationStartAnalysisContext.RegisterSyntaxNodeAction(nodeAnalyzsisContext => Analyze(nodeAnalyzsisContext, whitelistedTypes), SyntaxKind.IdentifierName);
            });
        }

        private void Analyze(SyntaxNodeAnalysisContext context, INamedTypeSymbol[] allowedSystemIoTypes)
        {
            var syntaxNode = (CompilationUnitSyntax)context.SemanticModel.SyntaxTree.GetRoot();
            if (!syntaxNode.ToString().Contains("System.IO"))
            {
                return;
            }


            if (!CheckPreConditions(context))
            {
                return;
            }

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

            if (allowedSystemIoTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFromClassOrInterface(allowedType)))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }

        private bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            // TODO check if file is generated
            return true;
        }

        private Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, IdentifierNameSyntax identifierName)
        {
            var rootIdentifierName = identifierName.AncestorsAndSelf().Last(n => n.IsKind(SyntaxKind.QualifiedName) || n.IsKind(SyntaxKind.IdentifierName));
            var diagnosedNode = rootIdentifierName;
            while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.ObjectCreationExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.InvocationExpression)))
            {
                diagnosedNode = diagnosedNode.Parent as ExpressionSyntax;
            }

            var usedAs = DiagnosticFormatter.GetDiagnosedUsage(diagnosedNode);
            var location = DiagnosticFormatter.GetLocation(diagnosedNode);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}
