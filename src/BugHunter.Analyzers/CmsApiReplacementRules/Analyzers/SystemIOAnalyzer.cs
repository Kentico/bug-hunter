// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Access to any members of <see cref="System.IO"/> namespace other than <see cref="System.IO.IOException"/>
    /// or <see cref="System.IO.Stream"/> classes or their subclasses; or <see cref="System.IO.SeekOrigin"/> enumeration
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="SystemIOAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.SystemIO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Title), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_MessageFormat), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
            category: nameof(AnalyzerCategories.CmsApiReplacements),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Description), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                var filesWithSystemIoUsing = new ConcurrentDictionary<string, bool?>();

                compilationStartContext.RegisterSyntaxNodeAction(c => Analyze(c, filesWithSystemIoUsing), SyntaxKind.IdentifierName);
            });
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ConcurrentDictionary<string, bool?> filesWithSystemIoUsing)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            // quick only syntax based analysis
            var fileContainsSystemIoUsing = FileContainsSystemIoUsing(identifierNameSyntax, filesWithSystemIoUsing);
            if (fileContainsSystemIoUsing.HasValue && !fileContainsSystemIoUsing.Value)
            {
                // identifier name must be fully qualified - look for System.IO there
                var rootIdentifierName = identifierNameSyntax.GetOuterMostParentOfDottedExpression();

                // if not System.IO or if exception or string are used, return
                var rootIdentifierNameText = rootIdentifierName.ToString();
                if (!rootIdentifierNameText.Contains("System.IO")
                    || rootIdentifierNameText.Contains("IOException")
                    || rootIdentifierNameText.Contains("Stream")
                    || rootIdentifierNameText.Contains("SeekOrigin"))
                {
                    return;
                }
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (symbol == null)
            {
                return;
            }

            // leave like this, not StartsWith() - e.g. System.IO.Compression is okay
            var symbolContainingNamespace = symbol.ContainingNamespace;
            if (!symbolContainingNamespace.ToString().Equals("System.IO"))
            {
                return;
            }

            if (symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.SeekOrigin", context.Compilation))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool? FileContainsSystemIoUsing(
            SyntaxNode identifierNameSyntax,
            ConcurrentDictionary<string, bool?> filesWithSystemIoUsing)
        {
            var syntaxTree = identifierNameSyntax.SyntaxTree;

            bool? result;
            var contains = filesWithSystemIoUsing.TryGetValue(syntaxTree.FilePath, out result);

            if (contains)
            {
                return result;
            }

            var root = syntaxTree.HasCompilationUnitRoot ? syntaxTree.GetCompilationUnitRoot() : null;

            if (root == null)
            {
                return filesWithSystemIoUsing
                    .AddOrUpdate(syntaxTree.FilePath, key => null, (key, originalVal) => null);
            }

            var containsSystemIoUsing = root
                .Usings
                .Any(u => u.ToString().Contains("System.IO"));

            return filesWithSystemIoUsing
                .AddOrUpdate(syntaxTree.FilePath, key => containsSystemIoUsing, (key, originalVal) => containsSystemIoUsing);
        }

        private class SystemIoDiagnosticFormatter : ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax>
        {
            public Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, IdentifierNameSyntax identifierName)
            {
                var rootOfDottedExpression = identifierName.GetOuterMostParentOfDottedExpression();
                var diagnosedNode = rootOfDottedExpression.Parent.IsKind(SyntaxKind.ObjectCreationExpression) || rootOfDottedExpression.Parent.IsKind(SyntaxKind.InvocationExpression)
                    ? rootOfDottedExpression.Parent
                    : rootOfDottedExpression;

                return Diagnostic.Create(descriptor, GetLocation(diagnosedNode), GetDiagnosedUsage(diagnosedNode));
            }

            private Location GetLocation(SyntaxNode syntaxNode)
            => syntaxNode?.GetLocation();

            private string GetDiagnosedUsage(SyntaxNode syntaxNode)
                => syntaxNode?.ToString() ?? string.Empty;
        }
    }
}
