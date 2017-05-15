﻿// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// A non-abstract class inherits directly from <c>System.Web.UI.UserControl</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="UserControlBaseAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.UserControlBase;

        private static readonly DiagnosticDescriptor Rule = BaseClassesInheritanceRulesProvider.GetRule(DiagnosticId, "User Control", "some abstract CMSUserControl");

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
