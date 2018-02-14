// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.BaseClassAnalyzers
{
    /// <summary>
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    ///
    /// Checks if Page file inherits from right class. -- register symbol action
    /// </summary>
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class PageBaseAnalyzerRegisterSymbolAction : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="PageBaseAnalyzerRegisterSymbolAction"/>
        /// </summary>
        public const string DiagnosticId = "BH3502";

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRulesProvider.GetRule(DiagnosticId, "Page", "some abstract CMSPage");

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = new NamedTypeSymbolDiagnosticFormatter();

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(symbolAnalysisContext =>
            {
                var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;
                if (namedTypeSymbol == null || namedTypeSymbol.IsAbstract)
                {
                    return;
                }

                var baseTypeSymbol = namedTypeSymbol.BaseType;
                if (baseTypeSymbol == null || !baseTypeSymbol.ToString().Equals("System.Web.UI.Page"))
                {
                    return;
                }

                var diagnostics = DiagnosticFormatter.CreateDiagnostics(Rule, namedTypeSymbol);
                symbolAnalysisContext.ReportDiagnostics(diagnostics);
            }, SymbolKind.NamedType);
        }
    }
}
