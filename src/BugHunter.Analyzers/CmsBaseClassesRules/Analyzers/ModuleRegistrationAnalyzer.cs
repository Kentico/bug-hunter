// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// The non-abstract <c>CMS.DataEngine.Module</c> or <c>CMS.Core.ModuleEntry</c> are not registered
    /// by <c>CMS.RegisterModule</c> assembly attribute in the same file where they are declared
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModuleRegistrationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="ModuleRegistrationAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ModuleRegistration;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_Title), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_MessageFormat), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)),
            category: nameof(AnalyzerCategories.CmsBaseClasses),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsBaseClassesResources.ModuleRegistration_Description), CmsBaseClassesResources.ResourceManager, typeof(CmsBaseClassesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            var diagnosticFormatter = new NamedTypeSymbolDiagnosticFormatter();
            context.RegisterSymbolAction(
                symbolAnalysisContext =>
            {
                var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;

                if (namedTypeSymbol == null ||
                    namedTypeSymbol.IsAbstract ||
                    !IsModuleOrModuleEntry(namedTypeSymbol, symbolAnalysisContext))
                {
                    return;
                }

                var syntaxTrees = namedTypeSymbol
                    .Locations
                    .Select(location => location.SourceTree);

                var registeredModuleTypes = syntaxTrees
                    .SelectMany(syntaxTree => GetRegisteredModuleTypeSymbols(syntaxTree, symbolAnalysisContext))
                    .ToList();

                if (IsModuleRegistered(registeredModuleTypes, namedTypeSymbol))
                {
                    return;
                }

                var diagnostic = diagnosticFormatter.CreateDiagnostic(Rule, namedTypeSymbol);
                symbolAnalysisContext.ReportDiagnostic(diagnostic);
            }, SymbolKind.NamedType);
        }

        private static bool IsModuleOrModuleEntry(INamedTypeSymbol namedTypeSymbol, SymbolAnalysisContext symbolAnalysisContext)
            => namedTypeSymbol.IsDerivedFrom("CMS.Core.ModuleEntry", symbolAnalysisContext.Compilation) ||
               namedTypeSymbol.IsDerivedFrom("CMS.DataEngine.Module", symbolAnalysisContext.Compilation);

        private static bool IsModuleRegistered(IList<ISymbol> registeredModuleTypes, INamedTypeSymbol moduleToBeChecked)
            => registeredModuleTypes.Any(registeredModule => registeredModule.Equals(moduleToBeChecked));

        private static IEnumerable<TypeSyntax> GetRegisteredModuleTypeSyntaxes(SyntaxTree syntaxTree)
        {
            var assemblyAttributeListSyntaxes = GetAttributeListSyntaxes(syntaxTree)
                .Where(attributeList => attributeList.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword));

            var registerModuleAssemblyAttributes = assemblyAttributeListSyntaxes
                .SelectMany(attributeList => attributeList.Attributes)
                .Where(attribute => attribute.Name.ToString() == "RegisterModule");

            var registeredModuleTypeSyntaxes = registerModuleAssemblyAttributes
                .SelectMany(attribute => attribute.ArgumentList.Arguments)
                .Select(argument => argument.Expression)
                .Where(argumentExpression => argumentExpression.IsKind(SyntaxKind.TypeOfExpression))
                .Cast<TypeOfExpressionSyntax>()
                .Select(typeOfExpression => typeOfExpression.Type);

            return registeredModuleTypeSyntaxes;
        }

        private static IEnumerable<AttributeListSyntax> GetAttributeListSyntaxes(SyntaxTree syntaxTree)
        {
            if (syntaxTree.HasCompilationUnitRoot)
            {
                return syntaxTree.GetCompilationUnitRoot().AttributeLists;
            }

            return syntaxTree
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<AttributeListSyntax>();
        }

        private static IEnumerable<ISymbol> GetRegisteredModuleTypeSymbols(SyntaxTree syntaxTree, SymbolAnalysisContext symbolAnalysisContext)
        {
            var registeredModuleTypeSyntaxes = GetRegisteredModuleTypeSyntaxes(syntaxTree);
            var semanticModel = symbolAnalysisContext.Compilation.GetSemanticModel(syntaxTree);
            return registeredModuleTypeSyntaxes.Select(typeSyntax => semanticModel.GetSymbolInfo(typeSyntax).Symbol);
        }
    }
}
