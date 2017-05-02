﻿using System.Collections.Immutable;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if User Control file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.USER_CONTROL_BASE;

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRulesProvider.GetRule(DIAGNOSTIC_ID, "User Control", "some abstract CMSUserControl");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = new NamedTypeSymbolDiagnosticFormatter();

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
                if (baseTypeSymbol == null || !baseTypeSymbol.ToString().Equals("System.Web.UI.UserControl"))
                {
                    return;
                }

                var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, namedTypeSymbol);
                symbolAnalysisContext.ReportDiagnostic(diagnostic);
            }, SymbolKind.NamedType);
        }
    }
}
